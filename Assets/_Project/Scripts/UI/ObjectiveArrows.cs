// Assets/_Project/Scripts/UI/ObjectiveArrows.cs
// Flechas en el borde de la pantalla que apuntan a los puntos científicos pendientes.
// Se ocultan cuando el punto está en pantalla o ya se completó. Muestran nombre y distancia.
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveArrows : MonoBehaviour
{
    [SerializeField] RectTransform container;     // ocupa toda la pantalla
    [SerializeField] float edgeMargin = 70f;
    [SerializeField] Sprite arrowSprite;          // opcional (arte); si no, se genera

    class Marker { public ScienceSitePoint site; public RectTransform root; public RectTransform arrow; public TMP_Text label; public string name; }
    readonly List<Marker> markers = new List<Marker>();
    Camera cam;

    static string ShortName(ScienceSite s) =>
        s == ScienceSite.Delta ? "Delta" : s == ScienceSite.MineralOutcrop ? "Mineral" : "Roca especial";

    static Color SiteColor(ScienceSite s) =>
        s == ScienceSite.Delta ? new Color(0.45f, 0.7f, 1f) : s == ScienceSite.MineralOutcrop ? new Color(1f, 0.45f, 0.4f) : new Color(0.4f, 0.95f, 1f);

    void Start()
    {
        cam = Camera.main;
        if (container == null) container = (RectTransform)transform;
        var sprite = arrowSprite != null ? arrowSprite : ProceduralSprites.Arrow();
        foreach (var site in FindObjectsByType<ScienceSitePoint>(FindObjectsSortMode.None))
        {
            var m = new Marker { site = site, name = ShortName(site.Site) };
            m.root = UIKit.NewRect("Arrow_" + m.name, container);
            m.root.sizeDelta = new Vector2(160, 90);
            var a = UIKit.NewRect("Arrow", m.root);
            a.sizeDelta = new Vector2(46, 46);
            var img = a.gameObject.AddComponent<Image>();
            img.sprite = sprite; img.color = SiteColor(site.Site); img.raycastTarget = false;
            m.arrow = a;
            m.label = UIKit.Text(m.root, "Label", m.name, 22, TextAlignmentOptions.Center);
            m.label.rectTransform.sizeDelta = new Vector2(220, 30);
            m.label.outlineWidth = 0.25f; m.label.outlineColor = new Color32(0, 0, 0, 200);
            markers.Add(m);
        }
    }

    void LateUpdate()
    {
        var mc = MissionController.Instance;
        if (cam == null || mc == null) return;
        Vector2 size = container.rect.size;
        Vector2 half = size * 0.5f;
        Vector3 roverPos = cam.transform.position;

        foreach (var m in markers)
        {
            bool done = mc.State.Result.CompletedSites.Contains(m.site.Site) || mc.State.IsOver;
            Vector3 vp = cam.WorldToViewportPoint(m.site.transform.position);
            bool onScreen = vp.z > 0 && vp.x > 0.03f && vp.x < 0.97f && vp.y > 0.05f && vp.y < 0.95f;
            m.root.gameObject.SetActive(!done && !onScreen);
            if (done || onScreen) continue;

            // Dirección desde el centro de la pantalla hacia el punto
            Vector2 p = new Vector2((vp.x - 0.5f) * size.x, (vp.y - 0.5f) * size.y);
            if (vp.z < 0) p = -p;
            Vector2 dir = p.normalized;
            // Llevarlo al borde (rectángulo con margen)
            float sx = (half.x - edgeMargin) / Mathf.Max(0.0001f, Mathf.Abs(dir.x));
            float sy = (half.y - edgeMargin) / Mathf.Max(0.0001f, Mathf.Abs(dir.y));
            Vector2 pos = dir * Mathf.Min(sx, sy);
            m.root.anchoredPosition = pos;
            m.arrow.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            // Texto un poco hacia adentro de la flecha
            m.label.rectTransform.anchoredPosition = -dir * (48f + Mathf.Abs(dir.x) * 95f) + new Vector2(0f, Mathf.Abs(dir.x) > 0.7f ? -30f : 0f);
            float dist = Vector2.Distance(roverPos, m.site.transform.position);
            m.label.text = $"{m.name} · {dist:0} m";
            // Latido suave
            float s = 1f + 0.08f * Mathf.Sin(Time.unscaledTime * 4f);
            m.arrow.localScale = new Vector3(s, s, 1);
        }
    }
}
