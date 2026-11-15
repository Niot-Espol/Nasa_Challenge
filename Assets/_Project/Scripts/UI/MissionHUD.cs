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
    [SerializeField] RectTransform messagePanel;  // se anima al llegar un mensaje
    [SerializeField] UnityEngine.UI.Image messagePanelImage;
    [SerializeField] float messageDimAfter = 6f;  // segundos hasta atenuarse

    float messageAge = 999f;
    Color panelBase;

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
        messageAge = 0f;
        AudioManager.Play(Sfx.Typing, 1.6f);
    }

    void AnimateMessage()
    {
        if (messagePanel == null) return;
        if (messagePanelImage != null && panelBase.a == 0f) panelBase = messagePanelImage.color;
        messageAge += Time.unscaledDeltaTime;
        // Golpe de escala al llegar
        float pop = messageAge < 0.35f ? 1f + 0.12f * Mathf.Sin(messageAge / 0.35f * Mathf.PI) : 1f;
        messagePanel.localScale = new Vector3(pop, pop, 1f);
        // Destello naranja que se desvanece
        if (messagePanelImage != null)
        {
            var th = UITheme.Current;
            Color flash = th != null ? th.accent : new Color(1f, 0.55f, 0.15f);
            float f = Mathf.Clamp01(1f - messageAge / 0.8f);
            Color c = Color.Lerp(panelBase, flash, f * 0.7f);
            // Pasado un rato se atenúa para no molestar (el próximo mensaje vuelve a destacar)
            float dim = messageAge > messageDimAfter ? Mathf.Lerp(1f, 0.45f, Mathf.Clamp01((messageAge - messageDimAfter) / 1.5f)) : 1f;
            c.a = panelBase.a * dim;
            messagePanelImage.color = c;
            if (messageText != null) messageText.alpha = dim;
        }
    }

    // Ícono a la izquierda del estado: escudo en modo seguro, antena tachada sin señal
    Image statusIcon;
    void UpdateStatusIcon(MissionState s, UITheme th, string status)
    {
        if (statusText == null || th == null) return;
        Sprite sp = s.InSafeMode ? th.iconSafeMode : s.CommsDown ? th.iconNoSignal : null;
        if (statusIcon == null)
        {
            if (sp == null) return;
            statusIcon = UIKit.Icon(statusText.transform.parent, "StatusIcon", sp, new Vector2(48, 48));
            statusIcon.preserveAspect = true;
            statusIcon.raycastTarget = false;
            var srt = statusText.rectTransform;
            var irt = statusIcon.rectTransform;
            irt.anchorMin = irt.anchorMax = srt.anchorMin;
            irt.pivot = new Vector2(1f, 1f);
        }
        statusIcon.enabled = sp != null;
        statusIcon.gameObject.SetActive(sp != null);
        if (sp == null) return;
        statusIcon.sprite = sp;
        string first = status.Split('\n')[0];
        float w = statusText.GetPreferredValues(first).x;
        var pos = statusText.rectTransform.anchoredPosition;
        statusIcon.rectTransform.anchoredPosition = new Vector2(pos.x - w * 0.5f - 10f, pos.y + 2f);
    }

    static void SetIcon(Image img, Sprite s)
    {
        if (img == null) return;
        img.sprite = s;
        img.enabled = s != null;
        img.gameObject.SetActive(s != null);
    }

    void Update()
    {
        AnimateMessage();
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
        if (s.InSafeMode)
            status += s.DustStormActive && s.Design.UsesSolar
                ? $"<color=#FF5555>MODO SEGURO · sin Sol, espera {mc.StormTimeLeft:0} s</color>   "
                : $"<color=#FF5555>MODO SEGURO · recargando {Mathf.Clamp01(s.Energy / (s.MaxEnergy * MissionState.SafeModeResumeFraction)) * 100f:0}%</color>   ";
        if (s.DustStormActive) status += $"<color=#FFAA44>TORMENTA {mc.StormTimeLeft:0} s</color>   ";
        if (s.CommsDown) status += $"<color=#9FA8FF>SIN SEÑAL {mc.BlackoutTimeLeft:0} s</color>   ";
        if (mc.IsScanning) status += $"<color=#7FE8FF>ESCANEANDO {mc.ScanProgress * 100f:0}%</color>   ";
        if (mc.IsFastForwarding) status += $"<color=#FFFFFF><b>>> x{mc.FastForwardSpeed:0}</b></color>";
        else if (mc.IsWaiting) status += "\n<size=75%><color=#C8D2E6>Mantén ESPACIO (rover quieto) para adelantar el tiempo</color></size>";
        if (statusText != null) statusText.text = status;
        UpdateStatusIcon(s, th, status);

        if (statsText != null)
            statsText.text = $"Energía: {s.Energy:0}/{s.MaxEnergy:0}\nTiempo: {secs / 60}:{secs % 60:00}\nPuntos: {s.Result.ScienceCount}/3";
    }
}
