// Assets/_Project/Scripts/UI/ThemedText.cs
// Aplica la fuente del UITheme (y opcionalmente su color) a un texto TMP.
using TMPro;
using UnityEngine;

public enum TextTone { Normal, Muted, Accent, Good, Warning, Error, Keep }

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class ThemedText : MonoBehaviour
{
    public TextTone tone = TextTone.Normal;

    void OnEnable() => Apply();
#if UNITY_EDITOR
    void OnValidate() => Apply();
#endif

    public void Apply()
    {
        var theme = UITheme.Current;
        var t = GetComponent<TMP_Text>();
        if (theme == null || t == null) return;
        if (theme.font != null) t.font = theme.font;
        if (tone != TextTone.Keep) t.color = ToneColor(theme, tone);
    }

    public static Color ToneColor(UITheme theme, TextTone tone)
    {
        switch (tone)
        {
            case TextTone.Muted: return theme.textMuted;
            case TextTone.Accent: return theme.accent;
            case TextTone.Good: return theme.good;
            case TextTone.Warning: return theme.warning;
            case TextTone.Error: return theme.error;
            default: return theme.textColor;
        }
    }
}
