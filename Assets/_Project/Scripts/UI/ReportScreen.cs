// Assets/_Project/Scripts/UI/ReportScreen.cs
// Pantalla del Reporte (05_Report). Lee GameSession, arma el ReportData y lo muestra.
// El layout estático está en la escena; aquí solo se llenan los contenedores.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportScreen : MonoBehaviour
{
    [Header("Contenedores (en la escena)")]
    [SerializeField] TMP_Text gradeText;
    [SerializeField] Transform starsRow;
    [SerializeField] Transform badgesRow;
    [SerializeField] TMP_Text resourcesText;
    [SerializeField] Transform linesList;
    [SerializeField] Transform comparisonList;

    [Header("Diseño nuevo (opcionales)")]
    [SerializeField] Image roverImage;
    [SerializeField] ResourceBar budgetBar, massBar, timeBar, scienceBar;
    [SerializeField] TMP_Text budgetText, massText, timeText, scienceText;
    [SerializeField] Image vegaPortrait;
    [SerializeField] TMP_Text vegaText;
    [SerializeField] TMP_Text compareTitle;

    [SerializeField] float starSize = 72f;

    void Start()
    {
        var design = GameSession.EnsureDesign();
        var result = GameSession.EnsureResult();
        var mission = GameSession.EnsureMission();
        var rep = ReportBuilder.Build(design, result, mission);
        if (compareTitle != null)
        {
            compareTitle.text = mission.ComparisonTitle;
            // En inglés el título es más largo: se ajusta a una línea
            compareTitle.textWrappingMode = TextWrappingModes.NoWrap;
            compareTitle.enableAutoSizing = true; compareTitle.fontSizeMin = 18; compareTitle.fontSizeMax = 30;
        }
        Show(rep);
        StartCoroutine(RevealStars(rep.Stars));
    }

    readonly System.Collections.Generic.List<Image> stars = new System.Collections.Generic.List<Image>();

    // Las estrellas ganadas aparecen una por una, con un "ding" cada vez más agudo.
    System.Collections.IEnumerator RevealStars(int count)
    {
        var th = UITheme.Current;
        yield return new WaitForSecondsRealtime(0.6f);
        for (int i = 0; i < count && i < stars.Count; i++)
        {
            var img = stars[i];
            if (th != null && th.starFull != null) { img.sprite = th.starFull; img.color = Color.white; }
            else img.color = th != null ? th.accent : Color.yellow;
            AudioManager.Play(Sfx.StarDing, 1f + i * 0.12f);
            for (float t = 0; t < 0.25f; t += Time.unscaledDeltaTime)
            {
                float k = 1f + 0.35f * Mathf.Sin(t / 0.25f * Mathf.PI);
                img.transform.localScale = new Vector3(k, k, 1);
                yield return null;
            }
            img.transform.localScale = Vector3.one;
            yield return new WaitForSecondsRealtime(0.25f);
        }
        if (count == 3) AudioManager.Play(Sfx.Success);
    }

    public void Show(ReportData rep)
    {
        var th = UITheme.Current;

        if (gradeText != null) gradeText.text = rep.Grade;

        // Estrellas: sprite si existe, si no un "★" de texto
        if (starsRow != null)
        {
            UIKit.Clear(starsRow);
            stars.Clear();
            // Todas empiezan vacías; RevealStars enciende las ganadas.
            for (int i = 0; i < 3; i++)
            {
                Sprite s = th != null ? th.starEmpty : null;
                var img = UIKit.Icon(starsRow, "Star", s, new Vector2(starSize, starSize));
                img.enabled = true;
                img.color = s != null ? Color.white : new Color(0.3f, 0.34f, 0.42f);
                stars.Add(img);
            }
        }

        if (badgesRow != null)
        {
            UIKit.Clear(badgesRow);
            if (rep.Badges.Count == 0)
                UIKit.Size(UIKit.Text(badgesRow, "None", Loc.T("Sin insignias esta vez", "No badges this time"), 24, TextAlignmentOptions.Left, TextTone.Muted), 400, 40);
            foreach (var b in rep.Badges) AddBadge(b, th);
        }

        if (resourcesText != null)
        {
            int used = Mathf.RoundToInt(rep.TimeUsed);
            resourcesText.text =
                Loc.T("Presupuesto", "Budget") + $": {rep.CostUsed}/{rep.Budget}\n" +
                Loc.T("Masa", "Mass") + $": {rep.MassUsed}/{rep.MassCapacity} kg\n" +
                Loc.T("Energía gastada", "Energy used") + $": {rep.EnergyConsumed:0}\n" +
                Loc.T("Tiempo", "Time") + $": {used / 60}:{used % 60:00} / {Mathf.RoundToInt(rep.TimeLimit) / 60}:{Mathf.RoundToInt(rep.TimeLimit) % 60:00}";
        }

        // Recursos como barras
        int tUsed = Mathf.RoundToInt(rep.TimeUsed), tLim = Mathf.RoundToInt(rep.TimeLimit);
        SetBar(budgetBar, budgetText, rep.CostUsed, rep.Budget, Loc.T($"Presupuesto  <b>{rep.CostUsed}</b> / {rep.Budget} créditos", $"Budget  <b>{rep.CostUsed}</b> / {rep.Budget} credits"));
        SetBar(massBar, massText, rep.MassUsed, rep.MassCapacity, Loc.T("Masa", "Mass") + $"  <b>{rep.MassUsed}</b> / {rep.MassCapacity} kg");
        SetBar(timeBar, timeText, rep.TimeUsed, rep.TimeLimit, Loc.T("Tiempo", "Time") + $"  <b>{tUsed / 60}:{tUsed % 60:00}</b> / {tLim / 60}:{tLim % 60:00}");
        SetBar(scienceBar, scienceText, rep.Stars, 3, Loc.T("Puntos científicos", "Science sites") + $"  <b>{rep.Stars}</b> / 3");

        if (roverImage != null && th != null) { roverImage.sprite = th.roverPreview; roverImage.enabled = th.roverPreview != null; roverImage.preserveAspect = true; }

        // Comentario final de la Dra. Vega
        if (vegaText != null)
            vegaText.text = GameSession.EnsureMission().VegaClosing(rep.Stars);
        if (vegaPortrait != null)
        {
            var sp = DialogueData.Portrait(Speaker.Vega, rep.Stars >= 2 ? Expression.Happy : Expression.Worried);
            vegaPortrait.sprite = sp; vegaPortrait.enabled = sp != null; vegaPortrait.preserveAspect = true;
        }

        Fill(linesList, rep.Lines);
        Fill(comparisonList, rep.Comparison);
    }

    static void SetBar(ResourceBar bar, TMP_Text label, float value, float max, string text)
    {
        if (bar != null) { bar.useStateColors = false; bar.Set(value, max); }
        if (label != null) label.text = text;
    }

    void AddBadge(Badge b, UITheme th)
    {
        Sprite s = th == null ? null
                 : b == Badge.Commander ? th.badgeCommander
                 : b == Badge.Efficient ? th.badgeEfficient
                 : th.badgeNoSafeMode;

        var row = UIKit.NewRect("Badge_" + b, badgesRow);
        UIKit.HList(row.gameObject, 8);
        if (s != null) UIKit.Icon(row, "Icon", s, new Vector2(64, 64));
        var t = UIKit.Text(row, "Name", ReportBuilder.BadgeName(b), 26, TextAlignmentOptions.Left, TextTone.Accent);
        UIKit.Size(t, -1, 64);
    }

    static void Fill(Transform list, System.Collections.Generic.List<ReportLine> lines)
    {
        if (list == null) return;
        UIKit.Clear(list);
        foreach (var l in lines)
        {
            // Solo caracteres que tiene cualquier fuente (la pixel font puede no traer símbolos).
            string mark = l.Tone == ReportTone.Good ? "<color=#73E67F><b>+</b></color>  "
                        : l.Tone == ReportTone.Bad ? "<color=#FF6B6B><b>x</b></color>  "
                        : "<color=#A6B3CC>-</color>  ";
            var t = UIKit.Text(list, "Line", mark + l.Text, 22, TextAlignmentOptions.TopLeft);
            var fitter = t.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
