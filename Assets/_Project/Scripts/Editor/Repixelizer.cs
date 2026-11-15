// Assets/_Project/Scripts/Editor/Repixelizer.cs
// Convierte "pixel art" generado por IA (miles de colores, bordes borrosos) en pixel art real:
// 1) reduce la imagen por un factor (cada bloque f×f → 1 píxel),
// 2) reduce la paleta a N colores (k-means),
// 3) agrega un contorno oscuro de 1 px.
// Se usa desde código (IA vía MCP) o desde el menú Tools > Mars Crafts > Repixelizar carpeta.
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Repixelizer
{
    public static Texture2D Pixelize(Texture2D src, int factor, int colors, bool outline = true)
    {
        int w = Mathf.Max(1, src.width / factor), h = Mathf.Max(1, src.height / factor);
        var dst = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var srcPx = src.GetPixels();
        var outPx = new Color[w * h];
        var opaque = new List<Color>();
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                Color sum = Color.clear; int n = 0;
                for (int dy = 0; dy < factor; dy++)
                    for (int dx = 0; dx < factor; dx++)
                    {
                        int sx = x * factor + dx, sy = y * factor + dy;
                        if (sx >= src.width || sy >= src.height) continue;
                        var p = srcPx[sy * src.width + sx];
                        if (p.a > 0.5f) { sum += p; n++; }
                    }
                if (n * 2 >= factor * factor) { var c = sum / n; c.a = 1; outPx[y * w + x] = c; opaque.Add(c); }
                else outPx[y * w + x] = Color.clear;
            }

        if (colors > 0 && opaque.Count > 0)   // colors <= 0: no cuantizar
        {
            int k = Mathf.Min(colors, opaque.Count);
            var cent = new Color[k];
            for (int i = 0; i < k; i++) cent[i] = opaque[i * opaque.Count / k];
            for (int it = 0; it < 10; it++)
            {
                var acc = new Vector3[k]; var cnt = new int[k];
                foreach (var c in opaque) { int b = Nearest(c, cent); acc[b] += new Vector3(c.r, c.g, c.b); cnt[b]++; }
                for (int i = 0; i < k; i++) if (cnt[i] > 0) cent[i] = new Color(acc[i].x / cnt[i], acc[i].y / cnt[i], acc[i].z / cnt[i], 1);
            }
            for (int i = 0; i < outPx.Length; i++) if (outPx[i].a > 0.5f) outPx[i] = cent[Nearest(outPx[i], cent)];
        }

        if (outline)
        {
            var outl = new Color(0.11f, 0.07f, 0.09f, 1);
            var copy = (Color[])outPx.Clone();
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (copy[y * w + x].a > 0.5f) continue;
                    bool edge = (x > 0 && copy[y * w + x - 1].a > 0.5f) || (x < w - 1 && copy[y * w + x + 1].a > 0.5f)
                             || (y > 0 && copy[(y - 1) * w + x].a > 0.5f) || (y < h - 1 && copy[(y + 1) * w + x].a > 0.5f);
                    if (edge) outPx[y * w + x] = outl;
                }
        }
        dst.SetPixels(outPx);
        dst.Apply();
        return dst;
    }

    static int Nearest(Color c, Color[] cent)
    {
        int b = 0; float bd = float.MaxValue;
        for (int i = 0; i < cent.Length; i++)
        {
            float dr = c.r - cent[i].r, dg = c.g - cent[i].g, db = c.b - cent[i].b;
            float d = dr * dr * 0.3f + dg * dg * 0.59f + db * db * 0.11f;
            if (d < bd) { bd = d; b = i; }
        }
        return b;
    }

    public static Texture2D Load(string path)
    {
        var t = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        t.LoadImage(File.ReadAllBytes(path));
        return t;
    }

    // Igual que Pixelize, pero con UNA paleta común para varias imágenes (tiles que deben unirse sin costuras).
    public static List<Texture2D> PixelizeShared(List<Texture2D> srcs, int factor, int colors)
    {
        var small = new List<Texture2D>();
        var all = new List<Color>();
        foreach (var s in srcs)
        {
            var t = Pixelize(s, factor, 0, false);   // solo reduce, sin cuantizar todavía
            small.Add(t);
            foreach (var c in t.GetPixels()) if (c.a > 0.5f) all.Add(c);
        }
        int k = Mathf.Min(colors, all.Count);
        var cent = new Color[k];
        for (int i = 0; i < k; i++) cent[i] = all[i * all.Count / k];
        for (int it = 0; it < 10; it++)
        {
            var acc = new Vector3[k]; var cnt = new int[k];
            foreach (var c in all) { int b = Nearest(c, cent); acc[b] += new Vector3(c.r, c.g, c.b); cnt[b]++; }
            for (int i = 0; i < k; i++) if (cnt[i] > 0) cent[i] = new Color(acc[i].x / cnt[i], acc[i].y / cnt[i], acc[i].z / cnt[i], 1);
        }
        foreach (var t in small)
        {
            var px = t.GetPixels();
            for (int i = 0; i < px.Length; i++) if (px[i].a > 0.5f) px[i] = cent[Nearest(px[i], cent)];
            t.SetPixels(px); t.Apply();
        }
        return small;
    }

    // Procesa todos los PNG de una carpeta (incluye subcarpetas) y los guarda en otra con la misma estructura.
    public static int ProcessFolder(string srcFolder, string dstFolder, int factor, int colors, bool outline = true)
    {
        int n = 0;
        foreach (var f in Directory.GetFiles(srcFolder, "*.png", SearchOption.AllDirectories))
        {
            string rel = f.Substring(srcFolder.Length).TrimStart('/', '\\');
            string outPath = Path.Combine(dstFolder, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            File.WriteAllBytes(outPath, Pixelize(Load(f), factor, colors, outline).EncodeToPNG());
            n++;
        }
        return n;
    }
}
