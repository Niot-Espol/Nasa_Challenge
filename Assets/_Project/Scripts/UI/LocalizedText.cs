// Assets/_Project/Scripts/UI/LocalizedText.cs
// Texto fijo de una escena con su versión en inglés. El español es el texto original del TMP.
// Si un script cambia el texto en tiempo de ejecución, ese script manda (este componente solo actúa al activarse).
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [TextArea] public string english;
    [SerializeField, HideInInspector] string spanish;

    TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        if (string.IsNullOrEmpty(spanish)) spanish = text.text;
    }

    void OnEnable() { Apply(); Loc.Changed += Apply; }
    void OnDisable() => Loc.Changed -= Apply;

    void Apply()
    {
        if (text == null) return;
        text.text = Loc.T(spanish, english);
    }

#if UNITY_EDITOR
    public void SetSpanish(string es) => spanish = es;
#endif
}
