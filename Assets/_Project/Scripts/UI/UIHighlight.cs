// Assets/_Project/Scripts/UI/UIHighlight.cs
// Resalta un elemento de la UI mientras un diálogo lo menciona (modo tutorial):
//  - el elemento "late" (escala);
//  - el resto de la pantalla se oscurece con un panel negro semitransparente;
//  - el elemento se dibuja por encima de ese panel (Canvas anidado con orden propio).
// En el diálogo, la línea lleva highlight = id (p. ej. "energy") y este componente con el mismo id se activa.
using UnityEngine;
using UnityEngine.UI;

public class UIHighlight : MonoBehaviour
{
    [SerializeField] string id = "energy";
    [SerializeField] float scaleAmount = 0.12f;
    [SerializeField] float speed = 6f;

    [Header("Oscurecer el resto")]
    [SerializeField] bool dimOthers = true;
    [SerializeField, Range(0, 1)] float dimAlpha = 0.8f;   // en espacio de color lineal 0.8 se ve como un oscurecido medio

    Vector3 baseScale;
    bool active;
    float t;
    Canvas raised;

    static Image dim;
    static float dimTarget;
    const float DimFadeSpeed = 4f;

    void Awake() => baseScale = transform.localScale;
    void OnEnable() => DialogueBox.HighlightChanged += OnHighlight;
    void OnDisable()
    {
        DialogueBox.HighlightChanged -= OnHighlight;
        transform.localScale = baseScale;
        Lower();
        if (active) dimTarget = 0f;
        active = false;
    }

    void OnHighlight(string current)
    {
        bool was = active;
        active = current == id;
        if (active && !was) { Raise(); if (dimOthers) ShowDim(dimAlpha); }
        if (!active && was) { transform.localScale = baseScale; Lower(); }
        if (current == null) dimTarget = 0f;
    }

    void Update()
    {
        // El oscurecido se anima desde cualquier UIHighlight (con tiempo sin escala: el diálogo pausa el juego)
        if (dim != null)
        {
            var c = dim.color;
            c.a = Mathf.MoveTowards(c.a, dimTarget, Time.unscaledDeltaTime * DimFadeSpeed * Mathf.Max(dimAlpha, 0.1f));
            dim.color = c;
            dim.enabled = c.a > 0.001f;
        }

        if (!active) return;
        t += Time.unscaledDeltaTime * speed;
        float s = 1f + scaleAmount * (0.5f + 0.5f * Mathf.Sin(t));
        transform.localScale = baseScale * s;
    }

    // El elemento se dibuja por encima de toda la UI de la escena (y del oscurecido)
    void Raise()
    {
        var root = GetComponentInParent<Canvas>()?.rootCanvas;
        if (root == null || GetComponent<Canvas>() != null) return;
        raised = gameObject.AddComponent<Canvas>();
        raised.overrideSorting = true;
        raised.sortingLayerID = root.sortingLayerID;
        raised.sortingOrder = root.sortingOrder + 1;
    }

    void Lower()
    {
        if (raised != null) Destroy(raised);
        raised = null;
    }

    // Panel negro sobre el mundo y la UI, pero debajo de la caja de diálogo
    void ShowDim(float alpha)
    {
        var root = GetComponentInParent<Canvas>()?.rootCanvas;
        if (root == null) return;
        if (dim == null)
        {
            var go = new GameObject("TutorialDim", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(root.transform, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
            dim = go.GetComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0f);
            dim.raycastTarget = false;
        }
        // Justo antes de la caja de diálogo (que queda visible encima)
        var box = root.GetComponentInChildren<DialogueBox>(true);
        Transform top = box != null ? box.transform : null;
        while (top != null && top.parent != root.transform) top = top.parent;
        dim.transform.SetAsLastSibling();
        if (top != null) top.SetAsLastSibling();   // la caja de diálogo siempre encima del oscurecido
        dimTarget = alpha;
    }
}
