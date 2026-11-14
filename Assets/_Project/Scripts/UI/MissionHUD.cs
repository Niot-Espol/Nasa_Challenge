// Assets/_Project/Scripts/UI/MissionHUD.cs
// HUD PROVISIONAL de 04_Mars (solo texto) para probar la exploración.
// La persona de UI lo reemplazará por barras e íconos; lee todo desde MissionController.
using TMPro;
using UnityEngine;

public class MissionHUD : MonoBehaviour
{
    [SerializeField] TMP_Text statsText;     // energía, tiempo, puntos, estados
    [SerializeField] TMP_Text messageText;   // mensajes de BIMO

    void Start()
    {
        var mc = MissionController.Instance;
        if (mc == null) return;
        mc.MessagePosted += m => { if (messageText != null) messageText.text = "BIMO: " + m; };
        if (messageText != null) messageText.text = "BIMO: " + mc.LastMessage;
    }

    void Update()
    {
        var mc = MissionController.Instance;
        if (mc == null || statsText == null) return;
        var s = mc.State;

        int secs = Mathf.CeilToInt(s.TimeLeft);
        string status = "";
        if (s.InSafeMode) status += "  <color=#FF5555>MODO SEGURO</color>";
        if (s.DustStormActive) status += "  <color=#FFAA44>TORMENTA</color>";
        if (s.CommsDown) status += "  <color=#AAAAFF>SIN SEÑAL</color>";

        statsText.text =
            $"Energía: {s.Energy:0}/{s.MaxEnergy:0}  (+{s.CurrentRecharge:0.#}/s)\n" +
            $"Tiempo: {secs / 60}:{secs % 60:00}\n" +
            $"Puntos: {s.Result.ScienceCount}/3" + status;
    }
}
