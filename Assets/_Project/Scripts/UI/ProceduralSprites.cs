// Assets/_Project/Scripts/UI/ProceduralSprites.cs
// Sprites y texturas generados por código para efectos que no necesitan arte dibujado:
// flecha, polvo (ruido repetible) y viñeta. Se crean una vez y se reutilizan.
using UnityEngine;

public static class ProceduralSprites
{
    static Sprite arrow;
    static Texture2D dust, vignette;

    // Triángulo apuntando a la derecha (32x32, borde oscuro).
    public static Sprite Arrow()
    {
        if (arrow != null) return arrow;
        int n = 32;
        var t = new Texture2D(n, n, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        var px = new Color[n * n];
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float half = (n - x) * 0.5f;            // ancho del triángulo en esa columna
                float dy = Mathf.Abs(y - (n - 1) * 0.5f);
                bool inside = x >= 2 && dy <= half - 1;
                bool border = !inside && x >= 1 && dy <= half + 0.5f;
                px[y * n + x] = inside ? Color.white : border ? new Color(0.08f, 0.06f, 0.08f, 1) : Color.clear;
            }
        t.SetPixels(px); t.Apply();
        arrow = Sprite.Create(t, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), 32);
        return arrow;
    }

    // Ruido suave repetible (para polvo que se desplaza).
    public static Texture2D Dust()
    {
        if (dust != null) return dust;
        int n = 128;
        dust = new Texture2D(n, n, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear };
        var px = new Color[n * n];
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float u = (float)x / n, v = (float)y / n;
                // Suma de senos con períodos enteros → se repite sin costura
                float a = Mathf.Sin(2 * Mathf.PI * (u * 3 + v * 1)) * 0.5f + Mathf.Sin(2 * Mathf.PI * (u * 1 - v * 4) + 1.3f) * 0.35f
                        + Mathf.Sin(2 * Mathf.PI * (u * 7 + v * 5) + 2.1f) * 0.2f + Mathf.Sin(2 * Mathf.PI * (u * 11 - v * 9) + 0.4f) * 0.12f;
                a = Mathf.Clamp01(a * 0.5f + 0.5f);
                a = Mathf.SmoothStep(0.5f, 1f, a);   // más contraste: vetas de polvo en lugar de niebla
                px[y * n + x] = new Color(0.86f, 0.55f, 0.32f, a);
            }
        dust.SetPixels(px); dust.Apply();
        return dust;
    }

    // Viñeta: transparente al centro, opaca en los bordes (se tiñe con el color del Image).
    public static Texture2D Vignette()
    {
        if (vignette != null) return vignette;
        int n = 128;
        vignette = new Texture2D(n, n, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        var px = new Color[n * n];
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x / (float)(n - 1)) * 2 - 1, dy = (y / (float)(n - 1)) * 2 - 1;
                float d = Mathf.Sqrt(dx * dx + dy * dy) / 1.41f;
                px[y * n + x] = new Color(1, 1, 1, Mathf.SmoothStep(0.45f, 1f, d));
            }
        vignette.SetPixels(px); vignette.Apply();
        return vignette;
    }
}
