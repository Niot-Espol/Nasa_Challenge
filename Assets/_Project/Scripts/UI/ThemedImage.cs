// Assets/_Project/Scripts/UI/ThemedImage.cs
// Pone en un Image el sprite y color del UITheme según su tipo de elemento.
// Así, cambiar el arte = cambiar el UITheme, sin tocar escenas ni prefabs.
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class ThemedImage : MonoBehaviour
{
    public UIElement element = UIElement.Panel;

    void OnEnable() => Apply();
#if UNITY_EDITOR
    void OnValidate() => Apply();
#endif

    public void SetElement(UIElement e)
    {
        element = e;
        Apply();
    }

    public void Apply()
    {
        var theme = UITheme.Current;
        var img = GetComponent<Image>();
        if (theme == null || img == null) return;
        var style = theme.Get(element);
        if (style == null) return;

        img.sprite = style.sprite;
        img.color = style.color;
        if (style.sprite != null)
        {
            img.type = style.slice == Image9Slice.Sliced ? Image.Type.Sliced
                     : style.slice == Image9Slice.Tiled ? Image.Type.Tiled
                     : Image.Type.Simple;
        }
        else if (img.type != Image.Type.Filled)
        {
            img.type = Image.Type.Simple;
        }
    }
}
