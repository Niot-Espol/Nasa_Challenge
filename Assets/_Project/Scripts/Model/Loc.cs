// Assets/_Project/Scripts/Model/Loc.cs
// C# puro: idioma del juego (español / inglés).
// Uso en código: Loc.T("Texto en español", "Text in English").
// El idioma se elige al primer arranque (00_Intro) y se guarda (LocSettings, en Core).
public enum Lang { Es, En }

public static class Loc
{
    public static Lang Current = Lang.Es;
    public static bool IsEnglish => Current == Lang.En;
    public static event System.Action Changed;

    public static string T(string es, string en) => Current == Lang.En && !string.IsNullOrEmpty(en) ? en : es;

    public static void Set(Lang lang)
    {
        if (Current == lang) return;
        Current = lang;
        Changed?.Invoke();
    }
}
