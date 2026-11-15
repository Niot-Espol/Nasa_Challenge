// Assets/_Project/Scripts/UI/UIHighlight.cs
// Hace "latir" un elemento de la UI mientras un diálogo lo menciona.
// En el diálogo, la línea lleva highlight = id (p. ej. "energy") y este componente con el mismo id se anima.
using UnityEngine;

public class UIHighlight : MonoBehaviour
{
    [SerializeField] string id = "energy";
    [SerializeField] float scaleAmount = 0.12f;
    [SerializeField] float speed = 6f;

    Vector3 baseScale;
    bool active;
    float t;

    void Awake() => baseScale = transform.localScale;
    void OnEnable() => DialogueBox.HighlightChanged += OnHighlight;
    void OnDisable() { DialogueBox.HighlightChanged -= OnHighlight; transform.localScale = baseScale; }

    void OnHighlight(string current) { active = current == id; if (!active) transform.localScale = baseScale; }

    void Update()
    {
        if (!active) return;
        t += Time.unscaledDeltaTime * speed;          // funciona con el juego pausado por el diálogo
        float s = 1f + scaleAmount * (0.5f + 0.5f * Mathf.Sin(t));
        transform.localScale = baseScale * s;
    }
}
