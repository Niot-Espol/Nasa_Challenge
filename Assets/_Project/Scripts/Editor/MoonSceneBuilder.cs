// Assets/_Project/Scripts/Editor/MoonSceneBuilder.cs
// Construye 04_Moon a partir de 04_Mars: mismo mapa y sistemas, pero con arte lunar, puntos de la Luna,
// sin tormenta ni apagón programados, y con un cráter en sombra permanente + zona sin señal alrededor del hielo.
// Menú: Mars Crafts › Luna › Construir escena 04_Moon   (sobrescribe 04_Moon; antes genera el arte provisional)
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MoonSceneBuilder
{
    const string MarsScene = "Assets/_Project/Scenes/04_Mars.unity";
    public const string MoonScene = "Assets/_Project/Scenes/04_Moon.unity";
    const string Art = "Assets/_Project/Art/";
    const string ShadowSpritePath = Art + "Sprites/Moon/Effects/shadow_soft.png";

    // Cráter en sombra (abajo a la derecha del mapa, donde está el hielo)
    static readonly Vector2 CraterCenter = new Vector2(15.5f, -11f);
    static readonly Rect ShadowRect = new Rect(8.6f, -18f, 14.4f, 13.4f);   // x, y, ancho, alto (mundo)
    const float NoSignalRadius = 5f;

    [MenuItem("Mars Crafts/Luna/Construir escena 04_Moon")]
    public static void BuildMenu() => Debug.Log("[Luna] " + Build());

    public static string Build()
    {
        if (EditorApplication.isPlaying) return "Sal de Play primero.";
        MoonArtGenerator.Generate();
        MakeShadowSprite();

        AssetDatabase.DeleteAsset(MoonScene);
        AssetDatabase.CopyAsset(MarsScene, MoonScene);
        var scene = EditorSceneManager.OpenScene(MoonScene, OpenSceneMode.Single);
        var log = new List<string>();

        // --- 1) Suelo: cada tile de Marte por su versión lunar (mismo nombre) ---
        int swapped = 0;
        foreach (var tm in Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include))
        {
            var used = new TileBase[tm.GetUsedTilesCount()];
            tm.GetUsedTilesNonAlloc(used);
            foreach (var t in used)
            {
                if (t == null) continue;
                var moon = AssetDatabase.LoadAssetAtPath<TileBase>(MoonArtGenerator.MoonTiles + t.name + ".asset");
                if (moon != null) { tm.SwapTile(t, moon); swapped++; }
            }
        }
        log.Add(swapped + " tiles cambiados");

        // --- 2) Objetos: rocas de la hoja, módulo, puntos científicos ---
        int rocks = 0;
        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
        {
            if (sr.sprite == null) continue;
            string path = AssetDatabase.GetAssetPath(sr.sprite);
            Sprite moon;
            if (path.EndsWith("MarsTileset.png") && (moon = AssetDatabase.LoadAssetAtPath<Sprite>(MoonArtGenerator.MoonRocks + sr.sprite.name + ".png")) != null) { sr.sprite = moon; rocks++; }
            else if (path.EndsWith("landing_module.png")) sr.sprite = Load("Sprites/Moon/Landing/landing_module_moon.png") ?? sr.sprite;
        }
        log.Add(rocks + " rocas");

        // Puntos científicos: Delta → borde iluminado, Mineral → regolito, Roca especial → hielo
        foreach (var sp in Object.FindObjectsByType<ScienceSitePoint>(FindObjectsInactive.Include))
        {
            var so = new SerializedObject(sp);
            var siteProp = so.FindProperty("site");
            var old = (ScienceSite)siteProp.enumValueIndex;
            ScienceSite site; string sprite, fact, source;
            switch (old)
            {
                case ScienceSite.Delta:
                    site = ScienceSite.RimRock; sprite = "science_point_rim_rock";
                    fact = "Cerca del polo sur, algunas zonas altas reciben luz del Sol casi todo el tiempo, mientras que el fondo de muchos cráteres no la recibe nunca. Por eso los bordes iluminados son candidatos para futuras bases lunares.";
                    source = "NASA · Lunar Reconnaissance Orbiter (LRO) — science.nasa.gov/mission/lro";
                    break;
                case ScienceSite.MineralOutcrop:
                    site = ScienceSite.Regolith; sprite = "science_point_regolith";
                    fact = "El regolito es la capa de polvo y fragmentos de roca que cubre la Luna, creada por miles de millones de años de impactos. Las muestras de las misiones Apolo incluyen pequeñas esferas de vidrio formadas por impactos y antiguas erupciones volcánicas.";
                    source = "NASA · Muestras lunares del programa Apolo — curator.jsc.nasa.gov/lunar";
                    break;
                default:
                    site = ScienceSite.Ice; sprite = "science_point_ice";
                    fact = "En 2009, la nave LCROSS impactó el cráter Cabeus, cerca del polo sur, y detectó agua en la nube de material que levantó. En los cráteres en sombra permanente la temperatura baja a unos -240 °C, así que el hielo puede conservarse durante miles de millones de años.";
                    source = "NASA · LCROSS y LRO (Diviner) — science.nasa.gov/mission/lcross";
                    break;
            }
            siteProp.enumValueIndex = (int)site;
            so.FindProperty("fact").stringValue = fact;
            so.FindProperty("factSource").stringValue = source;
            so.ApplyModifiedPropertiesWithoutUndo();
            sp.gameObject.name = "Site_" + site;
            var body = sp.transform.Find("Body");
            var s = Load("Sprites/Moon/SciencePoints/" + sprite + ".png");
            if (body != null && s != null) body.GetComponent<SpriteRenderer>().sprite = s;
        }

        // --- 3) Misión: sin eventos programados, intro lunar, sin polvo de Marte ---
        var mc = Object.FindAnyObjectByType<MissionController>();
        var mso = new SerializedObject(mc);
        mso.FindProperty("timedDustStorm").boolValue = false;
        mso.FindProperty("timedBlackout").boolValue = false;
        mso.FindProperty("missionId").stringValue = "moon";
        var intro = AssetDatabase.LoadAssetAtPath<DialogueData>("Assets/_Project/Data/Dialogues/Moon_Intro.asset");
        if (intro != null) mso.FindProperty("intro").objectReferenceValue = intro;
        mso.ApplyModifiedPropertiesWithoutUndo();
        foreach (var fx in Object.FindObjectsByType<MarsScreenEffects>(FindObjectsInactive.Include)) fx.enabled = false;

        // Cielo negro (fuera del mapa) y nombre de la zona
        if (Camera.main != null) Camera.main.backgroundColor = new Color(0.01f, 0.012f, 0.025f);
        SetText("ZoneBanner", "Luna · Polo sur");
        SetText("ZoneTag", "Polo sur lunar");

        // --- 4) Cráter en sombra + zona sin señal ---
        var map = GameObject.Find("Map");
        var zones = new GameObject("MoonZones");
        if (map != null) zones.transform.SetParent(map.transform, false);

        var shadow = new GameObject("ShadowZone_Crater");
        shadow.transform.SetParent(zones.transform, false);
        shadow.transform.position = ShadowRect.center;
        var box = shadow.AddComponent<BoxCollider2D>(); box.isTrigger = true; box.size = ShadowRect.size - new Vector2(1.2f, 1.2f);
        var ez = shadow.AddComponent<EnvironmentZone>();
        SetKind(ez, EnvironmentZone.Kind.Shadow, 2f);
        var dark = new GameObject("Darkness");
        dark.transform.SetParent(shadow.transform, false);
        var dsr = dark.AddComponent<SpriteRenderer>();
        dsr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShadowSpritePath);
        dsr.color = new Color(0.005f, 0.01f, 0.03f, 0.93f);   // espacio de color lineal: alfa alto para que se note
        dsr.sortingOrder = 12;   // por encima del suelo, rocas y rover; debajo de los indicadores (E, completado)
        if (dsr.sprite != null)
        {
            var size = dsr.sprite.bounds.size;
            dark.transform.localScale = new Vector3(ShadowRect.width / size.x, ShadowRect.height / size.y, 1f);
        }

        var nosig = new GameObject("NoSignalZone_CraterFloor");
        nosig.transform.SetParent(zones.transform, false);
        nosig.transform.position = CraterCenter;
        var circle = nosig.AddComponent<CircleCollider2D>(); circle.isTrigger = true; circle.radius = NoSignalRadius;
        SetKind(nosig.AddComponent<EnvironmentZone>(), EnvironmentZone.Kind.NoSignal, 0f);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        return "04_Moon construida: " + string.Join(", ", log);
    }

    static void SetKind(EnvironmentZone z, EnvironmentZone.Kind kind, float drain)
    {
        var so = new SerializedObject(z);
        so.FindProperty("kind").enumValueIndex = (int)kind;
        so.FindProperty("coldDrainPerSecond").floatValue = drain;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetText(string objectName, string text)
    {
        var go = GameObject.Find(objectName);
        if (go == null) return;
        var t = go.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (t != null) { t.text = text; EditorUtility.SetDirty(t); }
    }

    static Sprite Load(string rel) => AssetDatabase.LoadAssetAtPath<Sprite>(Art + rel);

    // Mancha de sombra pixelada: centro opaco y borde en escalones (estilo pixel art)
    static void MakeShadowSprite()
    {
        if (System.IO.File.Exists(ShadowSpritePath)) return;
        const int W = 48, H = 48;
        var tex = new Texture2D(W, H, TextureFormat.RGBA32, false);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                float dx = Mathf.Abs((x + 0.5f) / W - 0.5f) * 2f, dy = Mathf.Abs((y + 0.5f) / H - 0.5f) * 2f;
                float d = Mathf.Pow(Mathf.Pow(dx, 4f) + Mathf.Pow(dy, 4f), 0.25f);   // rectángulo redondeado
                float a = d < 0.70f ? 1f : d < 0.80f ? 0.75f : d < 0.88f ? 0.5f : d < 0.95f ? 0.25f : 0f;
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
        tex.Apply();
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ShadowSpritePath));
        System.IO.File.WriteAllBytes(ShadowSpritePath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(ShadowSpritePath);
    }
}
