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

    static void Apply(Image img, Sprite s)
    {
        if (img == null) return;
        img.sprite = s;
        img.gameObject.SetActive(s != null);
    }
}
