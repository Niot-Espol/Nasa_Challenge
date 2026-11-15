// Assets/_Project/Scripts/UI/TitleScreen.cs
// Pantalla de título: logo, misiones (Tierra y Luna bloqueadas, Marte disponible) y aviso de la NASA.
// Aplica los sprites del UITheme cuando existan; si no, queda el texto/colores provisionales.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image logo;
    [SerializeField] TMP_Text logoFallback;     // "MARS CRAFTS" en texto mientras no haya logo
    [SerializeField] Image earthIcon, moonIcon, marsIcon;
    [SerializeField] Image earthLock, moonLock;

    void Start()
    {
        GameSession.Reset();
        LanguageButton();
        var th = UITheme.Current;
        if (th == null) return;
        Apply(background, th.titleBackground);
        Apply(logo, th.logo);
        if (logoFallback != null) logoFallback.gameObject.SetActive(th.logo == null);
        Apply(earthIcon, th.missionEarth);
        Apply(moonIcon, th.missionMoon);
        Apply(marsIcon, th.missionMars);
        Apply(earthLock, th.iconLock);
        Apply(moonLock, th.iconLock);
    }

    // Botón arriba a la derecha para cambiar de idioma (se recuerda la elección)
    void LanguageButton()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) return;
        var b = UIKit.Button(canvas.transform, "LanguageButton", Loc.IsEnglish ? "ESPAÑOL" : "ENGLISH", 24, UIElement.Button, () =>
        {
            LocSettings.Choose(Loc.IsEnglish ? Lang.Es : Lang.En);
            SceneFlow.Go(SceneFlow.Title);   // recarga el título en el nuevo idioma
        });
        var rt = (RectTransform)b.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(1, 1);
        rt.sizeDelta = new Vector2(220, 60); rt.anchoredPosition = new Vector2(-30, -30);
    }

    static void Apply(Image img, Sprite s)
    {
        if (img == null) return;
        img.sprite = s;
        img.enabled = s != null;
        img.gameObject.SetActive(s != null);
    }
}
