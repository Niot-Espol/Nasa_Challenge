// Assets/_Project/Scripts/UI/InteractionTooltip.cs
// Cartel contextual: aparece sobre el objeto con el que puedes interactuar ("E · Hablar con la Dra. Vega").
// Se crea solo en el Canvas de la escena la primera vez que hace falta. Aspecto: UITheme (PanelDark).
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionTooltip : MonoBehaviour
{
    static InteractionTooltip instance;
    Interactable target;
    RectTransform rect, canvasRect;
    TMP_Text label;
    Camera cam;

    public static void Show(Interactable who)
    {
        var t = Get();
        if (t == null) return;
        t.target = who;
        t.gameObject.SetActive(true);
        t.Refresh();
    }

    public static void Hide(Interactable who)
    {
        if (instance != null && instance.target == who) { instance.target = null; instance.gameObject.SetActive(false); }
    }

    static InteractionTooltip Get()
    {
        if (instance != null) return instance;
        var canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null) return null;
        var panel = UIKit.Panel(canvasGO.transform, "InteractionTooltip", UIElement.PanelDark);
        panel.raycastTarget = false;
        var h = UIKit.HList(panel.gameObject, 0, 0, TextAnchor.MiddleCenter);
        h.padding = new RectOffset(26, 26, 14, 14);
        var fit = panel.gameObject.AddComponent<ContentSizeFitter>();
        fit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var txt = UIKit.Text(panel.transform, "Text", "", 28, TextAlignmentOptions.Center);
        txt.textWrappingMode = TextWrappingModes.NoWrap;
        instance = panel.gameObject.AddComponent<InteractionTooltip>();
        instance.rect = panel.rectTransform;
        instance.rect.pivot = new Vector2(0.5f, 0f);
        instance.canvasRect = (RectTransform)canvasGO.transform;
        instance.label = txt;
        instance.cam = Camera.main;
        // Debajo de la caja de diálogo
        int dialogueIndex = -1;
        foreach (Transform c in canvasGO.transform) if (c.GetComponent<DialogueBox>() != null) dialogueIndex = c.GetSiblingIndex();
        if (dialogueIndex >= 0) panel.transform.SetSiblingIndex(dialogueIndex);
        panel.gameObject.SetActive(false);
        return instance;
    }

    void Refresh()
    {
        if (target == null || label == null) return;
        label.text = target.PromptText;   // la "E" animada ya está sobre el objeto, justo debajo del cartel
    }

    void LateUpdate()
    {
        if (target == null) { gameObject.SetActive(false); return; }
        if (DialogueBox.IsOpen) { rect.localScale = Vector3.zero; return; }   // oculto durante los diálogos
        rect.localScale = Vector3.one;
        Refresh();
        if (cam == null) cam = Camera.main;
        Vector3 screen = cam.WorldToScreenPoint(target.TooltipAnchor);
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, null, out local);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = local + new Vector2(0f, 12f);
    }

    void OnDestroy() { if (instance == this) instance = null; }
}
