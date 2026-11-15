// Assets/_Project/Scripts/UI/ResourceBar.cs
// Barra de recurso (energía, tiempo, presupuesto, masa). El relleno es un Image "Filled".
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Image fill;
    public bool useStateColors = false;  // rojo si se pasa del máximo, amarillo si queda poco

    public void Set(float value, float max)
    {
        if (fill == null) return;
        float t = max <= 0f ? 0f : value / max;
        float c = Mathf.Clamp01(t);
        fill.fillAmount = c;
        // El ancho del relleno es el porcentaje (así se ve aunque el relleno no tenga sprite).
        var rt = fill.rectTransform;
        rt.anchorMin = new Vector2(0f, rt.anchorMin.y);
        rt.anchorMax = new Vector2(c, rt.anchorMax.y);
        fill.enabled = c > 0.01f;   // vacío: no dibujar (evita un relleno de ancho negativo)
        if (!useStateColors) return;
        var th = UITheme.Current;
        if (th == null) return;
        fill.color = t > 1f ? th.error : t > 0.85f ? th.warning : th.good;
    }

    public void SetColor(Color c) { if (fill != null) fill.color = c; }
}
