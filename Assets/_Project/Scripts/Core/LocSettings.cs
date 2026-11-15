// Assets/_Project/Scripts/Core/LocSettings.cs
// Guarda y carga el idioma elegido (PlayerPrefs; en la build web se guarda en el navegador).
using UnityEngine;

public static class LocSettings
{
    const string Key = "mc_lang";

    public static bool HasChoice => PlayerPrefs.HasKey(Key);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Load()
    {
        if (PlayerPrefs.HasKey(Key)) Loc.Set(PlayerPrefs.GetInt(Key) == 1 ? Lang.En : Lang.Es);
    }

    public static void Choose(Lang lang)
    {
        PlayerPrefs.SetInt(Key, lang == Lang.En ? 1 : 0);
        PlayerPrefs.Save();
        Loc.Set(lang);
    }
}
