// Assets/_Project/Scripts/Editor/MoonArtGenerator.cs
// Genera el arte PROVISIONAL de la Luna recoloreando el de Marte con una paleta lunar.
// Menú: Mars Crafts › Luna › Generar arte provisional
//
// Regla importante: NUNCA sobrescribe un PNG que ya exista. El equipo de arte entrega sus versiones
// con el MISMO nombre y en la MISMA carpeta (ver docs/game-design/lista-arte-para-dibujar.md, sección 11),
// y esas tienen prioridad. Para regenerar un provisional, borra el PNG y vuelve a lanzar el menú.
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MoonArtGenerator
{
    const string Art = "Assets/_Project/Art/";
    public const string MoonTilesPng = Art + "Tilesets/Moon/MoonTilesPNG/";
    public const string MoonTiles = Art + "Tilesets/Moon/MoonTiles/";
    public const string MoonRocks = Art + "Sprites/Moon/Rocks/";   // una roca por PNG (mismo nombre que en Marte)

    // Paleta Luna (regolito): negro azulado → gris → blanco frío
    static readonly Color[] Regolith = { Hex("0B0E16"), Hex("262B36"), Hex("4E5562"), Hex("858B96"), Hex("C4C8CF"), Hex("EEF0F3") };
    // Hielo: azul profundo → cian → blanco
    static readonly Color[] Ice = { Hex("0A1830"), Hex("1C4E7A"), Hex("3FA3D6"), Hex("8FE3F7"), Hex("E8FBFF") };

    [MenuItem("Mars Crafts/Luna/Generar arte provisional")]
    public static void GenerateMenu() => Debug.Log("[Luna] " + Generate());

    public static string Generate()
    {
        int made = 0, kept = 0;
        var log = new List<string>();

        // 1) Suelo: los 18 tiles que usa Marte
        foreach (var f in Directory.GetFiles(Art + "Tilesets/Mars/MarsTilesPNG", "*.png"))
        {
            string dst = MoonTilesPng + Path.GetFileName(f);
            if (Recolor(f, dst, Regolith, 0f, 0.10f, 0.78f)) made++; else kept++;
        }
        // 2) Rocas y piedras: cada pieza de la hoja de Marte se guarda como un PNG suelto
        var sheetTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        sheetTex.LoadImage(File.ReadAllBytes(Art + "Tilesets/Mars/MarsTileset.png"));
        foreach (var o in AssetDatabase.LoadAllAssetsAtPath(Art + "Tilesets/Mars/MarsTileset.png"))
        {
            var sp = o as Sprite;
            if (sp == null || !(sp.name.StartsWith("rock") || sp.name.StartsWith("pebbles"))) continue;
            string dst = MoonRocks + sp.name + ".png";
            if (File.Exists(dst)) { kept++; continue; }
            var r = sp.rect;
            var crop = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGBA32, false);
            crop.SetPixels(sheetTex.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height));
            crop.Apply();
            Directory.CreateDirectory(MoonRocks);
            string tmp = "Temp/moon_rock_tmp.png";
            File.WriteAllBytes(tmp, crop.EncodeToPNG());
            Object.DestroyImmediate(crop);
            Recolor(tmp, dst, Regolith, 0f, 0.08f, 0.80f);
            made++;
        }
        Object.DestroyImmediate(sheetTex);

        // 3) Piezas que el equipo de arte debe sustituir (provisionales con el nombre definitivo)
        var jobs = new[] {
            // origen Marte, destino Luna, paleta, cuánto color original conservar, negro, blanco
            new Job("Sprites/Mars/SciencePoints/science_point_special_rock.png", "Sprites/Moon/SciencePoints/science_point_rim_rock.png", Regolith, 0f, 0.02f, 0.62f),
            new Job("Sprites/Mars/SciencePoints/science_point_mineral.png", "Sprites/Moon/SciencePoints/science_point_regolith.png", Regolith, 0.12f, 0.08f, 0.80f),
            new Job("Sprites/Mars/SciencePoints/science_point_delta.png", "Sprites/Moon/SciencePoints/science_point_ice.png", Ice, 0f, 0.05f, 0.70f),
            new Job("Sprites/Mars/Landing/landing_module.png", "Sprites/Moon/Landing/landing_module_moon.png", Regolith, 0.45f, 0.05f, 0.80f),
            new Job("Sprites/Launch/Planets/planet_mars.png", "Sprites/Launch/Planets/planet_moon.png", Regolith, 0f, 0.10f, 0.75f),
        };
        foreach (var j in jobs)
        {
            if (Recolor(Art + j.src, Art + j.dst, j.ramp, j.keepColor, j.black, j.white)) { made++; log.Add("nuevo " + j.dst); }
            else { kept++; log.Add("se conserva " + j.dst); }
        }

        AssetDatabase.Refresh();

        // 4) Tiles de Unity para el suelo lunar (copias de los de Marte con el sprite lunar)
        int tiles = 0;
        Directory.CreateDirectory(MoonTiles);
        foreach (var f in Directory.GetFiles(Art + "Tilesets/Mars/MarsTiles", "*.asset"))
        {
            var src = AssetDatabase.LoadAssetAtPath<Tile>(f.Replace('\\', '/'));
            if (src == null) continue;
            string name = Path.GetFileNameWithoutExtension(f);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoonTilesPng + name + ".png");
            if (sprite == null) continue;
            string dst = MoonTiles + name + ".asset";
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(dst);
            if (tile == null) { tile = ScriptableObject.CreateInstance<Tile>(); AssetDatabase.CreateAsset(tile, dst); }
            tile.sprite = sprite; tile.color = src.color; tile.colliderType = src.colliderType; tile.flags = src.flags;
            EditorUtility.SetDirty(tile); tiles++;
        }
        AssetDatabase.SaveAssets();
        return $"{made} PNG generados, {kept} ya existían (no se tocan), {tiles} tiles. " + string.Join(" · ", log);
    }

    class Job
    {
        public string src, dst; public Color[] ramp; public float keepColor, black, white;
        public Job(string s, string d, Color[] r, float k, float b, float w) { src = s; dst = d; ramp = r; keepColor = k; black = b; white = w; }
    }

    // Recolorea por luminancia sobre una rampa de colores. No sobrescribe.
    // black/white: rango de luminancia de origen que se estira a la rampa completa (fijo, no por imagen,
    // para que los tiles vecinos queden coherentes).
    static bool Recolor(string srcPath, string dstPath, Color[] ramp, float keepColor, float black, float white)
    {
        if (File.Exists(dstPath)) return false;
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(File.ReadAllBytes(srcPath));
        var px = tex.GetPixels32();
        for (int i = 0; i < px.Length; i++)
        {
            var c = px[i];
            if (c.a == 0) continue;
            float lum = (0.299f * c.r + 0.587f * c.g + 0.114f * c.b) / 255f;
            float t = Mathf.Clamp01((lum - black) / Mathf.Max(0.01f, white - black));
            Color g = Sample(ramp, t);
            Color o = c;
            Color outc = Color.Lerp(g, o, keepColor);
            outc.a = c.a / 255f;
            px[i] = outc;
        }
        tex.SetPixels32(px);
        Directory.CreateDirectory(Path.GetDirectoryName(dstPath));
        File.WriteAllBytes(dstPath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        return true;
    }

    static Color Sample(Color[] ramp, float t)
    {
        float x = t * (ramp.Length - 1);
        int i = Mathf.Clamp(Mathf.FloorToInt(x), 0, ramp.Length - 2);
        return Color.Lerp(ramp[i], ramp[i + 1], x - i);
    }

    static Color Hex(string h)
    {
        Color c; ColorUtility.TryParseHtmlString("#" + h, out c); return c;
    }
}
