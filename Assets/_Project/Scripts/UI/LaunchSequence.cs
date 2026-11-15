// Assets/_Project/Scripts/UI/LaunchSequence.cs
// 03_Launch: cuenta regresiva, despegue y viaje al destino de la misión activa, luego pasa a su escena.
// Todo es UI; los sprites salen del UITheme (rocket, earth, mars, cruiseShip...). Espacio/clic salta.
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LaunchSequence : MonoBehaviour
{
    [Header("Despegue")]
    [SerializeField] CanvasGroup launchGroup;
    [SerializeField] Image launchBackground;
    [SerializeField] RectTransform rocket;
    [SerializeField] Image rocketImage;
    [SerializeField] Image flame;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] TMP_Text designText;
    [SerializeField] Image[] smoke;                 // nubes de humo en la base (se animan con UITheme.launchSmoke)
    [SerializeField] float flameFps = 14f;

    [Header("Viaje")]
    [SerializeField] CanvasGroup cruiseGroup;
    [SerializeField] Image spaceBackground;
    [SerializeField] RectTransform earth;
    [SerializeField] RectTransform mars;
    [SerializeField] RectTransform ship;
    [SerializeField] RectTransform trailParent;
    [SerializeField] TMP_Text cruiseText;
    [SerializeField] TMP_Text skipHint;

    [SerializeField] float cruiseSeconds = 5f;
    [SerializeField] float shipAngleOffset = 0f;    // 0 = el sprite de la nave mira a la derecha; -90 = mira arriba

    bool skip;

    void Start()
    {
        ApplyTheme();
        var d = GameSession.EnsureDesign();
        if (designText != null)
            designText.text = $"{LauncherName(d.LauncherId)} · {d.TotalMass}/{d.LauncherCapacity} kg · {d.InstrumentCount} " + Loc.T("instrumentos", "instruments");
        StartCoroutine(Run());
    }

    void Update()
    {
        var k = Keyboard.current; var m = Mouse.current;
        if ((k != null && (k.spaceKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame || k.escapeKey.wasPressedThisFrame))
            || (m != null && m.leftButton.wasPressedThisFrame))
            skip = true;
    }

    IEnumerator Run()
    {
        launchGroup.alpha = 1; cruiseGroup.alpha = 0;
        if (flame != null) flame.enabled = false;
        SetSmoke(0f, 0f);

        // 1) Cuenta regresiva
        for (int i = 3; i >= 1 && !skip; i--)
        {
            countdownText.text = i.ToString();
            AudioManager.Play(Sfx.Countdown);
            yield return Wait(1f);
        }
        countdownText.text = Loc.T("¡DESPEGUE!", "LIFTOFF!");
        AudioManager.Play(Sfx.Countdown, 1.5f);
        AudioManager.SetLoop(Loop.RocketRoar, true);
        RectTransform shakeTarget = (RectTransform)launchGroup.transform;
        Vector2 groupStart = shakeTarget.anchoredPosition;

        // 2) Despegue
        if (flame != null) flame.enabled = true;
        Vector2 start = rocket.anchoredPosition;
        // Encendido: humo y llama antes de que el cohete se mueva
        for (float t = 0; t < 0.8f && !skip; t += Time.deltaTime)
        {
            AnimateFlame(t, 0.45f);   // en la plataforma la llama es corta (el humo la tapa)
            SetSmoke(t / 0.8f, t);
            rocket.anchoredPosition = start + new Vector2(Random.Range(-2f, 2f), 0f);
            yield return null;
        }
        for (float t = 0; t < 2.6f && !skip; t += Time.deltaTime)
        {
            float e = t * t * 0.5f;                                  // acelera
            rocket.anchoredPosition = start + new Vector2(Random.Range(-3f, 3f), e * 420f);
            float k = Mathf.Lerp(14f, 3f, t / 2.6f);   // sacudida de pantalla que se va calmando
            shakeTarget.anchoredPosition = groupStart + new Vector2(Random.Range(-k, k), Random.Range(-k, k));
            AnimateFlame(t, Mathf.Lerp(0.45f, 1f, t / 0.6f));
            SetSmoke(1f - t / 2.6f * 0.6f, 0.8f + t);
            yield return null;
        }

        shakeTarget.anchoredPosition = groupStart;

        // 3) Viaje
        yield return Fade(launchGroup, 1, 0, 0.4f);
        yield return Fade(cruiseGroup, 0, 1, 0.4f);
        Vector2 a = earth.anchoredPosition, b = mars.anchoredPosition;
        Vector2 ctrl = (a + b) * 0.5f + new Vector2(0, 260f);           // arco de la trayectoria
        float nextDot = 0;
        for (float t = 0; t < cruiseSeconds && !skip; t += Time.deltaTime)
        {
            float u = t / cruiseSeconds;
            Vector2 p = Bezier(a, ctrl, b, u);
            ship.anchoredPosition = p;
            Vector2 ahead = Bezier(a, ctrl, b, Mathf.Min(1, u + 0.01f));
            ship.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(ahead.y - p.y, ahead.x - p.x) * Mathf.Rad2Deg + shipAngleOffset);
            if (u >= nextDot) { Dot(p); nextDot += 0.04f; }
            cruiseText.text = GameSession.EnsureMission().TravelLabel(u);
            yield return null;
        }
        cruiseText.text = GameSession.EnsureMission().ArrivalText;
        yield return Wait(1.3f);
        SceneFlow.Go(SceneFlow.MissionScene);
    }

    // Llama: recorre los frames del tema y "respira" un poco
    void AnimateFlame(float t, float size = 1f)
    {
        if (flame == null) return;
        var th = UITheme.Current;
        if (th != null && th.rocketFlame != null && th.rocketFlame.Length > 0)
            flame.sprite = th.rocketFlame[(int)(t * flameFps) % th.rocketFlame.Length];
        flame.transform.localScale = new Vector3(size, size * Random.Range(0.9f, 1.15f), 1);
    }

    // Humo: frames del tema, crece y se expande hacia los lados; alpha = intensidad
    void SetSmoke(float alpha, float t)
    {
        if (smoke == null) return;
        var th = UITheme.Current;
        var frames = th != null ? th.launchSmoke : null;
        for (int i = 0; i < smoke.Length; i++)
        {
            var img = smoke[i];
            if (img == null) continue;
            img.enabled = alpha > 0.01f && frames != null && frames.Length > 0;
            if (!img.enabled) continue;
            img.sprite = frames[Mathf.Min(frames.Length - 1, (int)(t * 3f) + (i % 2))];
            img.color = new Color(1, 1, 1, Mathf.Clamp01(alpha));
            float side = smoke.Length == 1 ? 0f : (i / (float)(smoke.Length - 1) - 0.5f) * 2f;   // -1..1
            float grow = 1f + Mathf.Min(t, 2f) * 0.35f;
            img.rectTransform.localScale = Vector3.one * grow;
            img.rectTransform.anchoredPosition = new Vector2(side * (60f + t * 70f), 0f);
        }
    }

    IEnumerator Wait(float s)
    {
        for (float t = 0; t < s && !skip; t += Time.deltaTime) yield return null;
    }

    static IEnumerator Fade(CanvasGroup g, float from, float to, float s)
    {
        for (float t = 0; t < s; t += Time.deltaTime) { g.alpha = Mathf.Lerp(from, to, t / s); yield return null; }
        g.alpha = to;
    }

    void Dot(Vector2 p)
    {
        if (trailParent == null) return;
        var img = UIKit.Icon(trailParent, "Dot", null, new Vector2(8, 8));
        img.enabled = true;
        img.color = new Color(1f, 0.6f, 0.25f, 0.8f);
        ((RectTransform)img.transform).anchoredPosition = p;
    }

    static Vector2 Bezier(Vector2 a, Vector2 c, Vector2 b, float t)
    {
        float u = 1 - t;
        return u * u * a + 2 * u * t * c + t * t * b;
    }

    void ApplyTheme()
    {
        var th = UITheme.Current;
        if (th == null) return;
        SetSprite(launchBackground, th.launchBackground);
        SetSprite(spaceBackground, th.spaceBackground);
        SetSprite(rocketImage, th.rocket);
        if (flame != null && th.rocketFlame != null && th.rocketFlame.Length > 0) flame.sprite = th.rocketFlame[0];
        SetSprite(earth.GetComponent<Image>(), th.earth);
        // Destino según la misión activa (el objeto se llama "Mars" por historia, pero es el destino)
        var mission = GameSession.EnsureMission();
        SetSprite(mars.GetComponent<Image>(), mission.Id == "moon" && th.moon != null ? th.moon : th.mars);
        var destLabel = mars.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (destLabel != null) destLabel.text = mission.Id == "moon" ? Loc.T("Luna", "Moon") : Loc.T("Marte", "Mars");
        if (mission.Id == "moon") mars.sizeDelta *= 0.8f;   // la Luna se ve más pequeña
        SetSprite(ship.GetComponent<Image>(), th.cruiseShip);
    }

    // Con sprite: lo pone en blanco (sin tinte). Sin sprite: deja el color provisional.
    static void SetSprite(Image img, Sprite s)
    {
        if (img == null || s == null) return;
        img.sprite = s; img.color = Color.white; img.preserveAspect = true;
    }

    static string LauncherName(string id)
    {
        switch (id)
        {
            case "launcher_light": return Loc.T("Lanzador ligero", "Light launcher");
            case "launcher_medium": return Loc.T("Lanzador medio", "Medium launcher");
            case "launcher_heavy": return Loc.T("Lanzador pesado", "Heavy launcher");
            default: return Loc.T("Lanzador", "Launcher");
        }
    }
}
