// Assets/_Project/Scripts/UI/MissionHUD.cs
// HUD de 04_Mars: barras de energía y tiempo, puntos científicos, estados y mensaje de BIMO.
// El aspecto (marcos, rellenos, íconos) sale del UITheme.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionHUD : MonoBehaviour
{
    [SerializeField] ResourceBar energyBar;
    [SerializeField] TMP_Text energyText;
    [SerializeField] Image energyIcon;
    [SerializeField] ResourceBar timeBar;
    [SerializeField] TMP_Text timeText;
    [SerializeField] Image timeIcon;
    [SerializeField] TMP_Text scienceText;
    [SerializeField] Image scienceIcon;
    [SerializeField] TMP_Text statusText;        // MODO SEGURO / TORMENTA / SIN SEÑAL
    [SerializeField] TMP_Text messageText;       // mensajes de BIMO

    // compatibilidad con la versión anterior (solo texto)
    [SerializeField] TMP_Text statsText;

    void Start()
    {
        var th = UITheme.Current;
        if (th != null)
        {
            SetIcon(energyIcon, th.iconEnergy);
            SetIcon(timeIcon, th.iconTime);
            SetIcon(scienceIcon, th.iconScience);
        }
        var mc = MissionController.Instance;
        if (mc == null) return;
        mc.MessagePosted += OnMessage;
        OnMessage(mc.LastMessage);
    }

    void OnDestroy()
    {
        var mc = MissionController.Instance;
        if (mc != null) mc.MessagePosted -= OnMessage;
    }

    void OnMessage(string m)
    {
        if (messageText != null) messageText.text = "<color=#FF9A3C>BIMO:</color> " + m;
    }

    static void SetIcon(Image img, Sprite s)
    {
        if (img == null) return;
        img.sprite = s;
        img.gameObject.SetActive(s != null);
    }

    void Update()
    {
        var mc = MissionController.Instance;
        if (mc == null) return;
        var s = mc.State;
        var th = UITheme.Current;

        if (energyBar != null)
        {
            energyBar.Set(s.Energy, s.MaxEnergy);
            if (th != null) energyBar.SetColor(s.InSafeMode ? th.error : s.Energy < s.MaxEnergy * 0.3f ? th.warning : th.good);
        }
        if (energyText != null)
            energyText.text = $"Energía {s.Energy:0}/{s.MaxEnergy:0}   <size=80%>(+{s.CurrentRecharge:0.#}/s)</size>";

        int secs = Mathf.CeilToInt(s.TimeLeft);
        if (timeBar != null)
        {
            timeBar.Set(s.TimeLeft, s.TimeLimit);
            if (th != null) timeBar.SetColor(s.TimeLeft < 30f ? th.warning : th.accent);
        }
        if (timeText != null) timeText.text = $"Tiempo {secs / 60}:{secs % 60:00}";

        if (scienceText != null) scienceText.text = $"Puntos científicos: {s.Result.ScienceCount}/3";

        string status = "";
        if (s.InSafeMode) status += "<color=#FF5555>MODO SEGURO</color>   ";
        if (s.DustStormActive) status += "<color=#FFAA44>TORMENTA DE POLVO</color>   ";
        if (s.CommsDown) status += "<color=#9FA8FF>SIN SEÑAL</color>";
        if (statusText != null) statusText.text = status;

        if (statsText != null)
            statsText.text = $"Energía: {s.Energy:0}/{s.MaxEnergy:0}\nTiempo: {secs / 60}:{secs % 60:00}\nPuntos: {s.Result.ScienceCount}/3";
    }
}
