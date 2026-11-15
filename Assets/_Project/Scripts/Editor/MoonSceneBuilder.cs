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

    // Distribución propia de la Luna
    static readonly Vector2 StartPos = new Vector2(0f, 12.6f);   // módulo arriba al centro
    static readonly Vector2 IcePos = new Vector2(-14.6f, -10.0f); // centro del cráter (separado del borde del mapa)
    static readonly Vector2 ShadowSize = new Vector2(13.6f, 12.4f);  // sombra alrededor del hielo
    const float NoSignalRadius = 4.4f;                           // fondo del cráter (sin señal)
    const float RingRadius = 5.0f;                               // anillo de rocas del cráter
    const float RingGapAngle = 45f;                              // entrada del cráter (grados, 0 = derecha)
    const float RingGapHalf = 30f;

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

        // --- 4) Distribución distinta a Marte: espejo, salida arriba, anillo de rocas en el cráter ---
        var map = GameObject.Find("Map");
        Vector2 ice = Relayout(map, log);
        Rect ShadowRect = new Rect(ice - ShadowSize * 0.5f, ShadowSize);
        Vector2 CraterCenter = ice;

        // --- 5) Cráter en sombra + zona sin señal ---
        var zones = new GameObject("MoonZones");

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

    // Refleja el mapa (izquierda ↔ derecha), sube la salida al centro, quita las barreras de Marte
    // y con sus rocas forma un anillo alrededor del cráter del hielo. Devuelve la posición del hielo.
    static Vector2 Relayout(GameObject map, List<string> log)
    {
        // Tiles en espejo
        foreach (var tm in map.GetComponentsInChildren<Tilemap>(true))
        {
            var cells = new List<KeyValuePair<Vector3Int, TileBase>>();
            foreach (var c in tm.cellBounds.allPositionsWithin) { var t = tm.GetTile(c); if (t != null) cells.Add(new KeyValuePair<Vector3Int, TileBase>(c, t)); }
            tm.ClearAllTiles();
            foreach (var kv in cells) tm.SetTile(new Vector3Int(-kv.Key.x - 1, kv.Key.y, kv.Key.z), kv.Value);
        }
        // Objetos en espejo (grupos y sus hijos), y dibujos volteados para que no se reconozcan
        foreach (Transform group in map.transform)
        {
            if (group.GetComponent<Grid>() != null) continue;
            MirrorX(group);
            bool decor = group.name == "CraterRim" || group.name == "Rocks" || group.name == "RockyZone" || group.name == "Ridge" || group.name == "BoulderWall";
            foreach (Transform child in group)
            {
                MirrorX(child);
                if (decor) { var sr = child.GetComponent<SpriteRenderer>(); if (sr != null) sr.flipX = !sr.flipX; }
            }
        }

        // Salida arriba al centro: módulo y rover
        var lander = GameObject.Find("Lander");
        if (lander != null) lander.transform.position = StartPos;
        var rover = GameObject.Find("Rover");
        if (rover != null) rover.transform.position = StartPos + new Vector2(2.6f, -0.9f);

        // Hielo (tras el espejo) = centro del cráter
        Vector2 ice = IcePos;
        foreach (var sp in map.GetComponentsInChildren<ScienceSitePoint>(true)) if (sp.Site == ScienceSite.Ice) sp.transform.position = IcePos;

        // Barreras de Marte (cresta y muro) → anillo del cráter, con una entrada hacia arriba-derecha
        var ringRocks = new List<Transform>();
        foreach (var n in new[] { "Ridge", "BoulderWall" })
        {
            var g = map.transform.Find(n);
            if (g == null) continue;
            foreach (Transform t in g) ringRocks.Add(t);
        }
        var ring = new GameObject("CraterRing").transform;
        ring.SetParent(map.transform, false);
        var angles = new List<float>();
        for (float a = 0; a < 360f; a += 360f / 26f)
            if (Mathf.Abs(Mathf.DeltaAngle(a, RingGapAngle)) > RingGapHalf) angles.Add(a);
        for (int i = 0; i < ringRocks.Count && i < angles.Count; i++)
        {
            int k = Mathf.RoundToInt(i * (angles.Count - 1f) / Mathf.Max(1, ringRocks.Count - 1));
            float rad = angles[k] * Mathf.Deg2Rad;
            ringRocks[i].SetParent(ring, true);
            ringRocks[i].position = (Vector3)(ice + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad) * 0.85f) * RingRadius);
        }
        foreach (var n in new[] { "Ridge", "BoulderWall" }) { var g = map.transform.Find(n); if (g != null) Object.DestroyImmediate(g.gameObject); }

        // Despejar: nada de rocas sueltas encima de la salida ni dentro del cráter
        int removed = 0;
        var rocks = map.transform.Find("Rocks");
        if (rocks != null)
            for (int i = rocks.childCount - 1; i >= 0; i--)
            {
                Vector2 p = rocks.GetChild(i).position;
                if (Vector2.Distance(p, StartPos) < 4.2f || Vector2.Distance(p, ice) < RingRadius + 0.8f) { Object.DestroyImmediate(rocks.GetChild(i).gameObject); removed++; }
            }
        log.Add("mapa en espejo, salida arriba, anillo de " + Mathf.Min(ringRocks.Count, angles.Count) + " rocas, " + removed + " rocas despejadas");
        return ice;
    }

    static void MirrorX(Transform t)
    {
        var p = t.localPosition; p.x = -p.x; t.localPosition = p;
        foreach (var bc in t.GetComponents<BoxCollider2D>()) { var o = bc.offset; o.x = -o.x; bc.offset = o; }
        foreach (var cc in t.GetComponents<CircleCollider2D>()) { var o = cc.offset; o.x = -o.x; cc.offset = o; }
        foreach (var pc in t.GetComponents<PolygonCollider2D>())
            for (int i = 0; i < pc.pathCount; i++)
            {
                var path = pc.GetPath(i);
                for (int j = 0; j < path.Length; j++) path[j].x = -path[j].x;
                System.Array.Reverse(path);
                pc.SetPath(i, path);
            }
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
