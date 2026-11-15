// Assets/_Project/Scripts/UI/UIKit.cs
// Fábrica de elementos de UI con el tema aplicado. La usan las pantallas para
// crear listas dinámicas en runtime y los scripts de editor para armar escenas.
// Todo lo que crea lleva ThemedImage/ThemedText → cambiar el arte no requiere tocar esto.
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIKit
{
    public static RectTransform NewRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = 5; // UI
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    // Coloca un RectTransform: anchors (min/max), pivot, posición y tamaño.
    public static RectTransform Place(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax,
                                      Vector2 pivot, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.pivot = pivot;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return rt;
    }

    // Estira para llenar al padre con márgenes (izq, abajo, der, arriba).
    public static RectTransform Fill(RectTransform rt, float l = 0, float b = 0, float r = 0, float t = 0)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(l, b); rt.offsetMax = new Vector2(-r, -t);
        return rt;
    }

    public static Image Panel(Transform parent, string name, UIElement element = UIElement.Panel)
    {
        var rt = NewRect(name, parent);
        var img = rt.gameObject.AddComponent<Image>();
        var th = rt.gameObject.AddComponent<ThemedImage>();
        th.SetElement(element);
        return img;
    }

    public static TextMeshProUGUI Text(Transform parent, string name, string text, float size,
                                       TextAlignmentOptions align = TextAlignmentOptions.TopLeft,
                                       TextTone tone = TextTone.Normal)
    {
        var rt = NewRect(name, parent);
        var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = size; t.alignment = align;
        t.textWrappingMode = TextWrappingModes.Normal;
        t.raycastTarget = false;
        var th = rt.gameObject.AddComponent<ThemedText>();
        th.tone = tone; th.Apply();
        return t;
    }

    public static Button Button(Transform parent, string name, string label, float fontSize = 28,
                                UIElement element = UIElement.Button, UnityAction onClick = null)
    {
        var img = Panel(parent, name, element);
        var btn = img.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        var colors = btn.colors;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.85f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        btn.colors = colors;
        var txt = Text(img.transform, "Label", label, fontSize, TextAlignmentOptions.Center);
        Fill(txt.rectTransform, 8, 4, 8, 4);
        if (onClick != null) btn.onClick.AddListener(onClick);
        return btn;
    }

    public static Image Icon(Transform parent, string name, Sprite sprite, Vector2 size)
    {
        var rt = NewRect(name, parent);
        rt.sizeDelta = size;
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;
        img.enabled = sprite != null;
        var le = rt.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = size.x; le.preferredHeight = size.y;
        return img;
    }

    public static VerticalLayoutGroup VList(GameObject go, float spacing = 10, int pad = 0,
                                            TextAnchor align = TextAnchor.UpperLeft, bool expandWidth = true)
    {
        var v = go.AddComponent<VerticalLayoutGroup>();
        v.spacing = spacing; v.padding = new RectOffset(pad, pad, pad, pad);
        v.childAlignment = align;
        v.childControlWidth = expandWidth; v.childControlHeight = true;
        v.childForceExpandWidth = expandWidth; v.childForceExpandHeight = false;
        return v;
    }

    public static HorizontalLayoutGroup HList(GameObject go, float spacing = 10, int pad = 0,
                                              TextAnchor align = TextAnchor.MiddleLeft)
    {
        var h = go.AddComponent<HorizontalLayoutGroup>();
        h.spacing = spacing; h.padding = new RectOffset(pad, pad, pad, pad);
        h.childAlignment = align;
        h.childControlWidth = true; h.childControlHeight = true;
        h.childForceExpandWidth = false; h.childForceExpandHeight = false;
        return h;
    }

    public static LayoutElement Size(Component c, float prefW = -1, float prefH = -1, float flexW = -1)
    {
        var le = c.GetComponent<LayoutElement>() ?? c.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = prefW; le.preferredHeight = prefH; le.flexibleWidth = flexW;
        return le;
    }

    public static void Clear(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying) Object.Destroy(t.GetChild(i).gameObject);
            else Object.DestroyImmediate(t.GetChild(i).gameObject);
        }
    }

    public static ResourceBar Bar(Transform parent, string name, Vector2 size)
    {
        var frame = Panel(parent, name, UIElement.BarFrame);
        frame.rectTransform.sizeDelta = size;
        Size(frame, size.x, size.y);
        var fill = Panel(frame.transform, "Fill", UIElement.BarFill);
        Fill(fill.rectTransform, 3, 3, 3, 3);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        var bar = frame.gameObject.AddComponent<ResourceBar>();
        bar.fill = fill;
        return bar;
    }
}
