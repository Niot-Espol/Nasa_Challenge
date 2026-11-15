// Assets/_Project/Scripts/UI/IntroCinematic.cs
// Cinemática inicial (escena 00_Intro): se reproduce al abrir el juego y lleva al título.
// Todo se construye en código con el arte del UITheme (fondo espacial, planetas, nave, paisaje, rover, retratos, logo).
// Espacio / Enter / Esc / clic: saltar.
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IntroCinematic : MonoBehaviour
{
    [SerializeField] float charsPerSecond = 60f;

    RectTransform stage;          // contenedor de cada plano
    Image fade;                   // negro para fundidos
    TMP_Text caption;
    bool skipping, picking;
    UITheme th;

    void Start()
    {
        th = UITheme.Current;
        Build();
        if (LocSettings.HasChoice) StartCoroutine(Play());
        else StartCoroutine(PickLanguage());
    }

    void Update()
    {
        var k = Keyboard.current; var m = Mouse.current;
        if (skipping || picking) return;
        if ((k != null && (k.spaceKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame || k.escapeKey.wasPressedThisFrame))
            || (m != null && m.leftButton.wasPressedThisFrame))
        {
            skipping = true;
            StopAllCoroutines();
            SceneFlow.Go(SceneFlow.Title);
        }
    }

    // ---------- Guion ----------
    IEnumerator Play()
    {
        // 1) Espacio y la Tierra
        Clear();
        var bg = Img(th.spaceBackground, Vector2.zero, new Vector2(1920, 1080), stretch: true);
        var earth = Img(th.earth, new Vector2(-420, 40), new Vector2(300, 300));
        StartCoroutine(Zoom(bg.rectTransform, 1f, 1.08f, 6f));
        StartCoroutine(Drift(earth.rectTransform, new Vector2(-420, 40), new Vector2(-380, 50), 6f));
        yield return Fade(1, 0, 0.6f);
        yield return Say(Loc.T("La humanidad lleva décadas explorando el sistema solar.", "For decades, humanity has been exploring the solar system."), 2.1f);

        // 2) Hacia la Luna
        var moon = Img(th.moon != null ? th.moon : th.earth, new Vector2(460, 120), new Vector2(170, 170));
        moon.color = new Color(1, 1, 1, 0);
        StartCoroutine(FadeImg(moon, 0, 1, 0.6f));
        var ship = Img(th.cruiseShip, new Vector2(-260, 40), new Vector2(110, 110));
        StartCoroutine(Fly(ship.rectTransform, new Vector2(-260, 40), new Vector2(380, 120), 2.6f));
        yield return Say(Loc.T("Observamos la Tierra. Buscamos hielo en la Luna.", "We watch the Earth. We search for ice on the Moon."), 2.2f);
        yield return Fade(0, 1, 0.45f);

        // 3) Marte crece en pantalla
        Clear();
        bg = Img(th.spaceBackground, Vector2.zero, new Vector2(1920, 1080), stretch: true);
        var mars = Img(th.mars, new Vector2(0, 40), new Vector2(180, 180));
        StartCoroutine(Grow(mars.rectTransform, 180f, 620f, 3.6f));
        yield return Fade(1, 0, 0.45f);
        yield return Say(Loc.T("Y en Marte hubo ríos y lagos. ¿Qué guardan sus rocas?", "And Mars once had rivers and lakes. What do its rocks hold?"), 2.3f);
        yield return Fade(0, 1, 0.45f);

        // 4) Superficie: el rover cruza el paisaje
        Clear();
        bg = Img(th.titleBackground != null ? th.titleBackground : th.spaceBackground, Vector2.zero, new Vector2(1920, 1080), stretch: true);
        StartCoroutine(Zoom(bg.rectTransform, 1.05f, 1f, 4f));
        var rover = Img(th.roverPreview, new Vector2(-1100, -250), new Vector2(330, 330));
        StartCoroutine(Fly(rover.rectTransform, new Vector2(-1100, -250), new Vector2(150, -250), 3.4f, bob: true));
        yield return Fade(1, 0, 0.45f);
        yield return Say(Loc.T("Cada misión se diseña con presupuesto, masa y energía limitados.", "Every mission is designed with limited budget, mass and energy."), 2.3f);
        yield return Fade(0, 1, 0.45f);

        // 5) El centro espacial: Vega y BIMO
        Clear();
        bg = Img(th.workshopBackground != null ? th.workshopBackground : th.spaceBackground, Vector2.zero, new Vector2(1920, 1080), stretch: true);
        bg.color = new Color(0.45f, 0.48f, 0.55f);
        Frame(new Vector2(-480, 50), new Vector2(440, 440));
        Frame(new Vector2(480, 50), new Vector2(440, 440));
        var vega = Img(DialogueData.Portrait(Speaker.Vega, Expression.Happy), new Vector2(-480, 50), new Vector2(380, 380));
        var bimo = Img(DialogueData.Portrait(Speaker.Bimo, Expression.Happy), new Vector2(480, 50), new Vector2(340, 340));
        vega.color = bimo.color = new Color(1, 1, 1, 0.35f);
        yield return Fade(1, 0, 0.4f);
        yield return Talk(vega, bimo, DialogueData.SpeakerName(Speaker.Vega), Loc.T("Bienvenido a Mars Crafts, comandante. Te estábamos esperando.", "Welcome to Mars Crafts, commander. We've been waiting for you."));
        yield return Talk(bimo, vega, "BIMO", Loc.T("¡Por fin! Llevo tres horas cargado al 100 %... y dos revisando que no exploto.", "Finally! I've been charged to 100% for three hours... and spent two checking I won't explode."));
        yield return Talk(vega, bimo, DialogueData.SpeakerName(Speaker.Vega), Loc.T("Aquí diseñarás y lanzarás tus propias misiones. ¿Listo?", "Here you'll design and launch your own missions. Ready?"));
        yield return Fade(0, 1, 0.45f);

        // 6) Logo
        Clear();
        bg = Img(th.titleBackground != null ? th.titleBackground : th.spaceBackground, Vector2.zero, new Vector2(1920, 1080), stretch: true);
        bg.color = new Color(0.6f, 0.6f, 0.7f);
        var logo = Img(th.logo, new Vector2(0, 80), new Vector2(900, 300));
        caption.text = "";
        yield return Fade(1, 0, 0.35f);
        AudioManager.Play(Sfx.StarDing);
        yield return Pop(logo.rectTransform, 0.6f);
        caption.text = Loc.T("<size=80%>Presiona cualquier tecla</size>", "<size=80%>Press any key</size>");
        for (float t = 0; t < 2f; t += Time.deltaTime) { caption.alpha = 0.5f + 0.5f * Mathf.Sin(t * 4f); yield return null; }
        skipping = true;
        SceneFlow.Go(SceneFlow.Title);
    }

    // ---------- Selector de idioma (solo la primera vez) ----------
    IEnumerator PickLanguage()
    {
        picking = true;
        var canvas = GameObject.Find("Canvas").transform;
        fade.color = new Color(0, 0, 0, 1);
        var panel = UIKit.Panel(canvas, "LanguagePicker", UIElement.Panel);
        UIKit.Place(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760, 420));
        UIKit.VList(panel.gameObject, 22, 44, TextAnchor.UpperCenter);
        var title = UIKit.Text(panel.transform, "Title", "Elige tu idioma\n<size=75%>Choose your language</size>", 40, TextAlignmentOptions.Center, TextTone.Accent);
        title.GetComponent<ThemedText>().title = true; title.GetComponent<ThemedText>().Apply(); UIKit.Size(title, -1, 110);
        bool chosen = false;
        UIKit.Size(UIKit.Button(panel.transform, "Es", "ESPAÑOL", 34, UIElement.ButtonPrimary, () => { LocSettings.Choose(Lang.Es); chosen = true; }), -1, 84);
        UIKit.Size(UIKit.Button(panel.transform, "En", "ENGLISH", 34, UIElement.ButtonPrimary, () => { LocSettings.Choose(Lang.En); chosen = true; }), -1, 84);
        while (!chosen) yield return null;
        AudioManager.Play(Sfx.Click);
        Destroy(panel.gameObject);
        foreach (var t in canvas.GetComponentsInChildren<TMP_Text>(true)) if (t.name == "Skip") t.text = Loc.T("Espacio: saltar", "Space: skip");
        yield return null;
        picking = false;
        StartCoroutine(Play());
    }

    // ---------- Construcción ----------
    void Build()
    {
        var canvas = GameObject.Find("Canvas").transform;
        stage = UIKit.NewRect("Stage", canvas); UIKit.Fill(stage);
        // Franjas de cine
        foreach (var top in new[] { true, false })
        {
            var bar = UIKit.Panel(canvas, top ? "BarTop" : "BarBottom", UIElement.Overlay);
            bar.color = Color.black; bar.raycastTarget = false;
            var rt = bar.rectTransform;
            rt.anchorMin = new Vector2(0, top ? 1 : 0); rt.anchorMax = new Vector2(1, top ? 1 : 0); rt.pivot = new Vector2(0.5f, top ? 1 : 0);
            rt.sizeDelta = new Vector2(0, 120); rt.anchoredPosition = Vector2.zero;
        }
        caption = UIKit.Text(canvas, "Caption", "", 34, TextAlignmentOptions.Center);
        var crt = caption.rectTransform; crt.anchorMin = new Vector2(0.5f, 0); crt.anchorMax = new Vector2(0.5f, 0); crt.pivot = new Vector2(0.5f, 0);
        crt.sizeDelta = new Vector2(1500, 110); crt.anchoredPosition = new Vector2(0, 8);
        var hint = UIKit.Text(canvas, "Skip", Loc.T("Espacio: saltar", "Space: skip"), 20, TextAlignmentOptions.Right, TextTone.Muted);
        var hrt = hint.rectTransform; hrt.anchorMin = hrt.anchorMax = new Vector2(1, 1); hrt.pivot = new Vector2(1, 1); hrt.sizeDelta = new Vector2(400, 40); hrt.anchoredPosition = new Vector2(-30, -40);
        fade = UIKit.Panel(canvas, "Fade", UIElement.Overlay); fade.color = Color.black; fade.raycastTarget = false; UIKit.Fill(fade.rectTransform);
    }

    // Marco tipo videollamada detrás de un retrato
    void Frame(Vector2 pos, Vector2 size)
    {
        var f = UIKit.Panel(stage, "Frame", UIElement.PanelDark);
        f.raycastTarget = false;
        var rt = f.rectTransform; rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    void Clear() { for (int i = stage.childCount - 1; i >= 0; i--) Destroy(stage.GetChild(i).gameObject); caption.text = ""; }

    Image Img(Sprite s, Vector2 pos, Vector2 size, bool stretch = false)
    {
        var img = UIKit.Icon(stage, s != null ? s.name : "Img", s, size);
        img.enabled = s != null; img.raycastTarget = false; img.preserveAspect = !stretch;
        var rt = img.rectTransform; rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.anchoredPosition = pos; rt.sizeDelta = size;
        return img;
    }

    // ---------- Animaciones ----------
    IEnumerator Fade(float from, float to, float secs)
    {
        for (float t = 0; t < secs; t += Time.deltaTime) { fade.color = new Color(0, 0, 0, Mathf.Lerp(from, to, t / secs)); yield return null; }
        fade.color = new Color(0, 0, 0, to);
    }

    IEnumerator Say(string text, float hold)
    {
        caption.alpha = 1f; caption.text = text; caption.maxVisibleCharacters = 0;
        for (float shown = 0; shown < text.Length; shown += charsPerSecond * Time.deltaTime)
        {
            int n = (int)shown;
            if (n != caption.maxVisibleCharacters && n % 3 == 0 && n < text.Length && text[n] != ' ') AudioManager.PlayTyping(1f);
            caption.maxVisibleCharacters = n; yield return null;
        }
        caption.maxVisibleCharacters = text.Length;
        yield return new WaitForSeconds(hold);
    }

    // Diálogo: ilumina a quien habla, atenúa al otro
    IEnumerator Talk(Image speaker, Image other, string name, string text)
    {
        speaker.color = Color.white; other.color = new Color(1, 1, 1, 0.35f);
        StartCoroutine(Pop(speaker.rectTransform, 0.25f, 1.06f));
        yield return Say($"<color=#FF9A3C>{name}:</color> {text}", 2.4f);
    }

    IEnumerator Zoom(RectTransform rt, float a, float b, float secs)
    {
        for (float t = 0; t < secs && rt != null; t += Time.deltaTime) { rt.localScale = Vector3.one * Mathf.Lerp(a, b, t / secs); yield return null; }
    }

    IEnumerator Drift(RectTransform rt, Vector2 a, Vector2 b, float secs)
    {
        for (float t = 0; t < secs && rt != null; t += Time.deltaTime) { rt.anchoredPosition = Vector2.Lerp(a, b, t / secs); yield return null; }
    }

    IEnumerator Fly(RectTransform rt, Vector2 a, Vector2 b, float secs, bool bob = false)
    {
        for (float t = 0; t < secs && rt != null; t += Time.deltaTime)
        {
            float u = Mathf.SmoothStep(0, 1, t / secs);
            Vector2 p = Vector2.Lerp(a, b, u);
            if (bob) p.y += Mathf.Abs(Mathf.Sin(t * 9f)) * 6f;
            else p.y += Mathf.Sin(u * Mathf.PI) * 60f;
            rt.anchoredPosition = p; yield return null;
        }
    }

    IEnumerator Grow(RectTransform rt, float a, float b, float secs)
    {
        for (float t = 0; t < secs && rt != null; t += Time.deltaTime) { float s = Mathf.Lerp(a, b, Mathf.SmoothStep(0, 1, t / secs)); rt.sizeDelta = new Vector2(s, s); yield return null; }
    }

    IEnumerator FadeImg(Image img, float a, float b, float secs)
    {
        for (float t = 0; t < secs && img != null; t += Time.deltaTime) { var c = img.color; c.a = Mathf.Lerp(a, b, t / secs); img.color = c; yield return null; }
    }

    IEnumerator Pop(RectTransform rt, float secs, float peak = 1.15f)
    {
        for (float t = 0; t < secs && rt != null; t += Time.deltaTime)
        {
            float u = t / secs;
            rt.localScale = Vector3.one * (u < 0.6f ? Mathf.Lerp(0.6f, peak, u / 0.6f) : Mathf.Lerp(peak, 1f, (u - 0.6f) / 0.4f));
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }
}
