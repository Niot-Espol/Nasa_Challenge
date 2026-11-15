// Assets/_Project/Scripts/UI/WorkshopScreen.cs
// Taller (02_Workshop): el jugador elige componentes; los totales y los avisos de BIMO
// se actualizan en vivo. Las tarjetas se generan desde los ComponentData asignados.
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopScreen : MonoBehaviour
{
    [Header("Datos")]
    [SerializeField] ComponentData[] components;       // los 14 assets de ScriptableObjects/Components

    [Header("Contenedores (en la escena)")]
    [SerializeField] Transform categoriesList;          // filas de categorías con sus tarjetas
    [SerializeField] ResourceBar budgetBar;
    [SerializeField] TMP_Text budgetText;
    [SerializeField] ResourceBar massBar;
    [SerializeField] TMP_Text massText;
    [SerializeField] TMP_Text statsText;                // resumen del rover
    [SerializeField] Image roverPreview;
    [SerializeField] TMP_Text roverPreviewPlaceholder;
    [SerializeField] Image bimoPortrait;
    [SerializeField] TMP_Text bimoText;                 // avisos del Verifier o descripción del componente
    [SerializeField] Button launchButton;

    [SerializeField] float cardWidth = 370f;
    [SerializeField] float cardHeight = 112f;

    // Orden y nombre de las categorías en pantalla
    // Propiedad (no campo estático): así los títulos salen en el idioma actual
    static (ComponentCategory cat, string title, bool multi)[] Categories =>
    new[]
    {
        (ComponentCategory.Launcher,   Loc.T("Lanzador", "Launcher"),        false),
        (ComponentCategory.Energy,     Loc.T("Energía", "Power"),         false),
        (ComponentCategory.Battery,    Loc.T("Batería", "Battery"),         false),
        (ComponentCategory.Instrument, Loc.T("Instrumentos (elige los que quieras)", "Instruments (pick as many as you like)"), true),
        (ComponentCategory.Comms,      Loc.T("Comunicaciones", "Communications"),  false),
        (ComponentCategory.Wheels,     Loc.T("Ruedas", "Wheels"),          false),
    };

    readonly HashSet<ComponentData> selected = new HashSet<ComponentData>();
    readonly Dictionary<ComponentData, ThemedImage> cards = new Dictionary<ComponentData, ThemedImage>();
    RoverDesign design;
    VerificationResult verification;
    string infoOverride;   // descripción del último componente tocado

    void Start()
    {
        Preselect();
        BuildCards();
        if (launchButton != null) launchButton.onClick.AddListener(Launch);

        var th = UITheme.Current;
        if (th != null)
        {
            if (roverPreview != null) { roverPreview.sprite = th.roverPreview; roverPreview.enabled = th.roverPreview != null; }
            if (roverPreviewPlaceholder != null) roverPreviewPlaceholder.gameObject.SetActive(th.roverPreview == null);
            if (bimoPortrait != null) { bimoPortrait.sprite = th.portraitBimo; bimoPortrait.enabled = th.portraitBimo != null; bimoPortrait.gameObject.SetActive(th.portraitBimo != null); }
        }
        Refresh();
    }

    // "Rediseñar": marcar el diseño anterior. Si no hay, solo las ruedas estándar (gratis).
    void Preselect()
    {
        var prev = GameSession.Design;
        if (prev != null)
        {
            foreach (var c in components)
                if (c != null && prev.ComponentIds.Contains(c.id)) selected.Add(c);
        }
        else
        {
            var std = components.FirstOrDefault(c => c != null && c.id == "wheels_standard");
            if (std != null) selected.Add(std);
        }
    }

    void BuildCards()
    {
        UIKit.Clear(categoriesList);
        cards.Clear();
        foreach (var (cat, title, multi) in Categories)
        {
            var row = UIKit.NewRect("Row_" + cat, categoriesList);
            UIKit.VList(row.gameObject, 6);
            var catTitle = UIKit.Text(row, "Title", title, 26, TextAlignmentOptions.Left, TextTone.Accent);
            catTitle.GetComponent<ThemedText>().title = true; catTitle.GetComponent<ThemedText>().Apply();
            UIKit.Size(catTitle, -1, 34);

            var cardsRow = UIKit.NewRect("Cards", row);
            UIKit.HList(cardsRow.gameObject, 14);
            UIKit.Size(cardsRow, -1, cardHeight);

            foreach (var c in components.Where(x => x != null && x.category == cat))
                cards[c] = MakeCard(cardsRow, c, multi);
        }
    }

    ThemedImage MakeCard(Transform parent, ComponentData c, bool multi)
    {
        var btn = UIKit.Button(parent, "Card_" + c.id, "", 1, UIElement.Slot);
        UIKit.Size(btn, cardWidth, cardHeight);
        Destroy(btn.transform.Find("Label").gameObject);
        Destroy(btn.GetComponent<ButtonSound>());

        var content = UIKit.NewRect("Content", btn.transform);
        UIKit.Fill(content, 12, 8, 12, 8);
        UIKit.HList(content.gameObject, 12, 0, TextAnchor.MiddleLeft);

        if (c.icon != null) UIKit.Icon(content, "Icon", c.icon, new Vector2(64, 64));

        var texts = UIKit.NewRect("Texts", content);
        UIKit.VList(texts.gameObject, 2);
        UIKit.Size(texts, -1, -1, 1);
        UIKit.Size(UIKit.Text(texts, "Name", c.Name, 25), -1, 30);
        UIKit.Size(UIKit.Text(texts, "Cost", CostLine(c), 20, TextAlignmentOptions.Left, TextTone.Muted), -1, 26);
        UIKit.Size(UIKit.Text(texts, "Effect", EffectLine(c), 19, TextAlignmentOptions.Left, TextTone.Normal), -1, 26);

        btn.onClick.AddListener(() => Toggle(c, multi));
        return btn.GetComponent<ThemedImage>();
    }

    void Toggle(ComponentData c, bool multi)
    {
        if (multi)
        {
            if (!selected.Remove(c)) selected.Add(c);
        }
        else
        {
            selected.RemoveWhere(x => x.category == c.category);
            selected.Add(c);
        }
        infoOverride = $"<b>{c.Name}</b>: {c.Description}";
        AudioManager.Play(Sfx.Select);
        Refresh();
    }

    void Refresh()
    {
        design = RoverDesignBuilder.Build(selected);
        verification = Verifier.Verify(design);

        foreach (var kv in cards)
            kv.Value.SetElement(selected.Contains(kv.Key) ? UIElement.SlotSelected : UIElement.Slot);

        var th = UITheme.Current;
        if (budgetBar != null) { budgetBar.useStateColors = true; budgetBar.Set(design.TotalCost, RoverDesign.Budget); }
        if (budgetText != null) budgetText.text = Loc.T($"Presupuesto: {design.TotalCost} / {RoverDesign.Budget} créditos", $"Budget: {design.TotalCost} / {RoverDesign.Budget} credits");

        int cap = design.LauncherCapacity;
        if (massBar != null) { massBar.useStateColors = true; massBar.Set(design.TotalMass, cap > 0 ? cap : design.TotalMass); }
        if (massText != null) massText.text = cap > 0
            ? Loc.T($"Masa: {design.TotalMass} / {cap} kg", $"Mass: {design.TotalMass} / {cap} kg")
            : Loc.T($"Masa: {design.TotalMass} kg (elige un lanzador)", $"Mass: {design.TotalMass} kg (pick a launcher)");

        if (statsText != null)
        {
            string power = design.Power == PowerSource.Solar ? Loc.T("Paneles solares", "Solar panels")
                         : design.Power == PowerSource.RTG ? "RTG" : "—";
            var instr = new List<string>();
            if (design.HasInstrument(Instrument.Camera)) instr.Add(Loc.T("Cámara", "Camera"));
            if (design.HasInstrument(Instrument.Spectrometer)) instr.Add(Loc.T("Espectrómetro", "Spectrometer"));
            if (design.HasInstrument(Instrument.Drill)) instr.Add(Loc.T("Taladro", "Drill"));
            string instrList = instr.Count > 0 ? string.Join(", ", instr) : Loc.T("ninguno", "none");
            string blackout = design.CommsId != null ? design.BlackoutSeconds.ToString("0") + " s" : "—";
            string rocky = design.AllTerrainWheels ? Loc.T("sí la cruza", "can cross it") : Loc.T("no la cruza", "can't cross it");
            statsText.text = Loc.T(
                $"Energía: {power}  (+{design.RechargePerSecond:0.#}/s)\nBatería: {design.BatteryCapacity:0}\nInstrumentos: {instrList}\nApagón de comunicación: {blackout}\nZona rocosa: {rocky}",
                $"Power: {power}  (+{design.RechargePerSecond:0.#}/s)\nBattery: {design.BatteryCapacity:0}\nInstruments: {instrList}\nComms blackout: {blackout}\nRocky zone: {rocky}");
        }

        if (bimoText != null)
        {
            var lines = new List<string>();
            if (!string.IsNullOrEmpty(infoOverride)) lines.Add(infoOverride);
            foreach (var i in verification.Issues)
            {
                string col = i.Severity == IssueSeverity.Error ? "#FF6B6B" : "#FFCC4D";
                lines.Add($"<color={col}>{i.Message}</color>");
            }
            if (verification.CanLaunch && verification.Issues.Count == 0)
                lines.Add(Loc.T("<color=#73E67F>¡Todo en orden, comandante! Listos para lanzar.</color>", "<color=#73E67F>All good, commander! Ready for launch.</color>"));
            else if (verification.CanLaunch)
                lines.Add(Loc.T("<color=#73E67F>Podemos lanzar, pero revisa los avisos.</color>", "<color=#73E67F>We can launch, but check the warnings.</color>"));
            bimoText.text = string.Join("\n", lines);
        }

        if (launchButton != null) launchButton.interactable = verification.CanLaunch;
    }

    void Launch()
    {
        if (!verification.CanLaunch) { AudioManager.Play(Sfx.Error); return; }
        GameSession.Design = design;
        GameSession.Result = null;
        SceneFlow.Go(SceneFlow.Launch);
    }

    static string CostLine(ComponentData c) =>
        c.category == ComponentCategory.Launcher ? Loc.T($"{c.cost} créditos", $"{c.cost} credits") : Loc.T($"{c.mass} kg · {c.cost} créditos", $"{c.mass} kg · {c.cost} credits");

    static string EffectLine(ComponentData c)
    {
        switch (c.category)
        {
            case ComponentCategory.Launcher: return Loc.T($"Lleva hasta {c.capacity:0} kg", $"Carries up to {c.capacity:0} kg");
            case ComponentCategory.Energy:
                return c.powerSource == PowerSource.Solar
                    ? Loc.T($"Recarga {c.rechargePerSecond:0.#}/s · 0 en tormenta", $"Recharge {c.rechargePerSecond:0.#}/s · 0 in storms")
                    : Loc.T($"Recarga {c.rechargePerSecond:0.#}/s siempre", $"Recharge {c.rechargePerSecond:0.#}/s always");
            case ComponentCategory.Battery: return Loc.T($"Energía máxima {c.capacity:0}", $"Max energy {c.capacity:0}");
            case ComponentCategory.Instrument: return Loc.T($"Escanear: {c.scanCost:0} de energía", $"Scan: {c.scanCost:0} energy");
            case ComponentCategory.Comms: return Loc.T($"Apagón de {c.blackoutSeconds:0} s", $"{c.blackoutSeconds:0} s blackout");
            default: return c.crossesRockyZone ? Loc.T("Cruza la zona rocosa", "Crosses the rocky zone") : Loc.T("No cruza la zona rocosa", "Can't cross the rocky zone");
        }
    }
}
