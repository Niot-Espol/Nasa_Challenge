// Assets/_Project/Scripts/UI/LaunchSequence.cs
// 03_Launch: cuenta regresiva, despegue y viaje Tierra → Marte (~7 meses), luego pasa a Marte.
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

    bool skip;

    void Start()
    {
        ApplyTheme();
        var d = GameSession.EnsureDesign();
        if (designText != null)
            designText.text = $"{LauncherName(d.LauncherId)} · {d.TotalMass}/{d.LauncherCapacity} kg · {d.InstrumentCount} instrumentos";
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

        // 1) Cuenta regresiva
        for (int i = 3; i >= 1 && !skip; i--)
        {
            countdownText.text = i.ToString();
            AudioManager.Play(Sfx.Countdown);
            yield return Wait(1f);
        }
        countdownText.text = "¡DESPEGUE!";
        AudioManager.Play(Sfx.Countdown, 1.5f);
        AudioManager.SetLoop(Loop.RocketRoar, true);
        RectTransform shakeTarget = (RectTransform)launchGroup.transform;
        Vector2 groupStart = shakeTarget.anchoredPosition;

        // 2) Despegue
        if (flame != null) flame.enabled = true;
        Vector2 start = rocket.anchoredPosition;
        for (float t = 0; t < 2.2f && !skip; t += Time.deltaTime)
        {
            float e = t * t * 0.5f;                                  // acelera
            rocket.anchoredPosition = start + new Vector2(Random.Range(-3f, 3f), e * 420f);
            float k = Mathf.Lerp(14f, 3f, t / 2.2f);   // sacudida de pantalla que se va calmando
            shakeTarget.anchoredPosition = groupStart + new Vector2(Random.Range(-k, k), Random.Range(-k, k));
            if (flame != null) flame.transform.localScale = new Vector3(1, Random.Range(0.8f, 1.25f), 1);
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
            ship.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(ahead.y - p.y, ahead.x - p.x) * Mathf.Rad2Deg - 90f);
            if (u >= nextDot) { Dot(p); nextDot += 0.04f; }
            int month = Mathf.Clamp(1 + (int)(u * 7f), 1, 7);
            cruiseText.text = $"Viaje Tierra → Marte · mes {month} de ~7";
            yield return null;
        }
        cruiseText.text = "Llegando a Marte: entrada, descenso y aterrizaje...";
        yield return Wait(1.3f);
        SceneFlow.Go(SceneFlow.Mars);
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
        SetSprite(mars.GetComponent<Image>(), th.mars);
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
            case "launcher_light": return "Lanzador ligero";
            case "launcher_medium": return "Lanzador medio";
            case "launcher_heavy": return "Lanzador pesado";
            default: return "Lanzador";
        }
    }
}
