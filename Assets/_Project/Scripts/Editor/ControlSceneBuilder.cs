// Assets/_Project/Scripts/Editor/ControlSceneBuilder.cs
// Construye 06_Control a partir del Hub: tres salas en fila (telemetría | centro de control | Mars Yard),
// unidas por pasillos, con cámara por pantallas (RoomCamera) y las consolas de la misión.
// Menú: Mars Crafts › Centro de control › Construir escena 06_Control
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class ControlSceneBuilder
{
    const string Hub = "Assets/_Project/Scenes/01_Hub.unity";
    public const string Scene = "Assets/_Project/Scenes/06_Control.unity";
    const float RoomStep = 23.5f;           // distancia entre centros de sala (= ancho de la cámara)
    const float GapBottom = -3.7f, GapTop = 1.0f;   // hueco de las puertas laterales (y)

    [MenuItem("Mars Crafts/Centro de control/Construir escena 06_Control")]
    public static void BuildMenu() => Debug.Log("[Control] " + Build());

    public static string Build()
    {
        if (EditorApplication.isPlaying) return "Sal de Play primero.";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Scene) == null) AssetDatabase.CopyAsset(Hub, Scene);
        else { System.IO.File.Copy(Hub, Scene, true); AssetDatabase.ImportAsset(Scene, ImportAssetOptions.ForceUpdate); }
        var scene = EditorSceneManager.OpenScene(Scene, OpenSceneMode.Single);

        // Puerta del Hub: solo decorado. Vega y BIMO: la historia la cuenta la misión.
        var door = GameObject.Find("Door");
        Object.DestroyImmediate(door.GetComponent<HubDoor>());
        foreach (var c in door.GetComponents<Collider2D>()) if (c.isTrigger) Object.DestroyImmediate(c);
        var vega = GameObject.Find("DraVega"); var bimo = GameObject.Find("BIMO");
        var promptTemplate = vega.transform.Find("Prompt").gameObject;
        foreach (var npc in new[] { vega, bimo })
        {
            Object.DestroyImmediate(npc.GetComponent<NpcDialogue>());
            foreach (var c in npc.GetComponents<Collider2D>()) if (c.isTrigger) Object.DestroyImmediate(c);
        }
        var doorPrompt = door.transform.Find("Prompt"); if (doorPrompt) doorPrompt.gameObject.SetActive(false);
        foreach (var npc in new[] { vega, bimo }) { var p = npc.transform.Find("Prompt"); if (p) p.gameObject.SetActive(false); }

        // --- Salas: la del Hub en el centro y dos copias a los lados ---
        var center = GameObject.Find("Room");
        var left = Object.Instantiate(center); left.name = "Room_Telemetry"; left.transform.position = new Vector3(-RoomStep, 0, 0);
        var right = Object.Instantiate(center); right.name = "Room_MarsYard"; right.transform.position = new Vector3(RoomStep, 0, 0);
        center.name = "Room_Control";
        // Muebles (vienen dentro de cada copia): telemetría sin mesa de herramientas; Mars Yard sin pantalla ni monitores
        Remove(left.transform, "desk_tools");
        Remove(right.transform, "big_screen_mars"); Remove(right.transform, "desk_monitors");

        // Puertas entre salas
        Doorway(center, "Wall_Left"); Doorway(center, "Wall_Right");
        Doorway(left, "Wall_Right"); Doorway(right, "Wall_Left");
        Corridor(center, +1); Corridor(center, -1);

        // --- Consolas ---
        Console(Find(left.transform, "big_screen_mars"), ConsoleStation.Kind.Telemetry, new Vector2(0, -1.2f), new Vector2(6.5f, 2.6f), new Vector2(0, 0.6f), 1.6f, promptTemplate);
        Console(Find(center.transform, "big_screen_mars"), ConsoleStation.Kind.DeepSpaceNetwork, new Vector2(0, -1.2f), new Vector2(6.5f, 2.6f), new Vector2(0, 0.6f), 1.6f, promptTemplate);

        // --- Mars Yard: arenero con la réplica del rover ---
        var replica = MarsYard(right.transform, promptTemplate);

        // --- Misión y cámara ---
        var m = new GameObject("ControlMission").AddComponent<ControlRoomMission>();
        var mso = new SerializedObject(m); mso.FindProperty("replicaRover").objectReferenceValue = replica; mso.ApplyModifiedPropertiesWithoutUndo();
        var cam = Camera.main;
        var rc = cam.GetComponent<RoomCamera>();
        if (rc == null) rc = cam.gameObject.AddComponent<RoomCamera>();
        var cso = new SerializedObject(rc); cso.FindProperty("player").objectReferenceValue = GameObject.Find("Player").transform; cso.ApplyModifiedPropertiesWithoutUndo();

        var zb = GameObject.Find("ZoneBanner");
        if (zb) { var tx = zb.GetComponentInChildren<TMPro.TMP_Text>(true); tx.text = "Centro de control de misión"; EditorUtility.SetDirty(tx); }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        return "06_Control construida (3 salas)";
    }

    static void Remove(Transform root, string name)
    {
        var t = Find(root, name);
        if (t != null) Object.DestroyImmediate(t.gameObject);
    }

    static Transform Find(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true)) if (t.name == name) return t;
        return null;
    }

    // Parte una pared lateral en dos trozos dejando un hueco para pasar
    static void Doorway(GameObject room, string wallName)
    {
        var wall = room.transform.Find(wallName);
        var sr = wall.GetComponent<SpriteRenderer>();
        var b = sr.bounds;
        float x = b.center.x;
        // Columnas decorativas (wall_narrow) y plantas a la altura de la puerta: fuera
        foreach (var r in room.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (r == null || r.gameObject == wall.gameObject) continue;
            var rb = r.bounds;
            bool sameSide = Mathf.Abs(rb.center.x - x) < 1.6f;
            bool inGap = rb.center.y > GapBottom - 0.2f && rb.center.y < GapTop + 0.2f;
            if (sameSide && inGap && (r.name.StartsWith("wall_narrow") || r.name.StartsWith("plant"))) Object.DestroyImmediate(r.gameObject);
        }
        var top = wall.gameObject; var bottom = Object.Instantiate(top, wall.parent); bottom.name = wallName + "_Bottom";
        Segment(top, x, GapTop, b.max.y); Segment(bottom, x, b.min.y, GapBottom);
    }

    static void Segment(GameObject wall, float x, float yMin, float yMax)
    {
        var sr = wall.GetComponent<SpriteRenderer>();
        float h = yMax - yMin;
        var s = wall.transform.localScale;
        float spriteH = sr.sprite.bounds.size.y;
        wall.transform.localScale = new Vector3(s.x, h / spriteH, s.z);
        wall.transform.position = new Vector3(x, (yMin + yMax) * 0.5f, 0);
        var bc = wall.GetComponent<BoxCollider2D>(); if (bc) { bc.size = sr.sprite.bounds.size; bc.offset = Vector2.zero; }
    }

    // Suelo del pasillo entre salas + paredes invisibles arriba y abajo del pasillo
    static void Corridor(GameObject centerRoom, int side)
    {
        var floor = centerRoom.GetComponentInChildren<Tilemap>();
        var tile = floor.GetTile(floor.cellBounds.min + new Vector3Int(2, 2, 0));
        float x0 = side > 0 ? 10.2f : -RoomStep + 10.2f, x1 = x0 + 3.1f;
        for (float x = x0; x <= x1; x += 0.5f)
            for (float y = GapBottom; y <= GapTop; y += 0.5f)
            {
                var cell = floor.WorldToCell(new Vector3(x, y, 0));
                if (floor.GetTile(cell) == null) floor.SetTile(cell, tile);
            }
        var blockers = new GameObject("CorridorWalls_" + (side > 0 ? "R" : "L"));
        blockers.transform.SetParent(centerRoom.transform, false);
        foreach (var yy in new[] { new Vector2(GapTop + 0.25f, 0.5f), new Vector2(GapBottom - 0.25f, 0.5f) })
        {
            var g = new GameObject("Block"); g.transform.SetParent(blockers.transform, false);
            g.transform.position = new Vector3((x0 + x1) * 0.5f, yy.x, 0);
            g.AddComponent<BoxCollider2D>().size = new Vector2(x1 - x0 + 1f, yy.y);
        }
    }

    static Transform MarsYard(Transform room, GameObject promptTemplate)
    {
        var yard = new GameObject("MarsYard").transform; yard.SetParent(room, false);
        Vector3 c = room.position + new Vector3(1.5f, -1.4f, 0);
        // Arena (tiles de Marte, 1 unidad)
        var gridGo = new GameObject("SandGrid", typeof(Grid)); gridGo.transform.SetParent(yard, false);
        var tmGo = new GameObject("Sand", typeof(Tilemap), typeof(TilemapRenderer)); tmGo.transform.SetParent(gridGo.transform, false);
        var tm = tmGo.GetComponent<Tilemap>(); tmGo.GetComponent<TilemapRenderer>().sortingOrder = -19;
        var sand = new TileBase[4];
        for (int i = 0; i < 4; i++) sand[i] = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/_Project/Art/Tilesets/Mars/MarsTiles/sand_" + (char)('a' + i) + ".asset");
        var rnd = new System.Random(7);
        Vector3 lc = c - room.position;   // la rejilla cuelga de la sala: coordenadas locales
        for (int x = -4; x < 4; x++) for (int y = -2; y < 2; y++)
            tm.SetTile(new Vector3Int(Mathf.FloorToInt(lc.x) + x, Mathf.FloorToInt(lc.y) + y, 0), sand[rnd.Next(4)]);
        // Réplica del rover
        var rover = new GameObject("ReplicaRover"); rover.transform.SetParent(yard, false); rover.transform.position = c + new Vector3(0.5f, 0.2f, 0);
        var sr = rover.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/Rover/DrivingV2/East/drive_east_000.png");
        sr.sortingOrder = 5; sr.spriteSortPoint = SpriteSortPoint.Pivot;
        // Consola alrededor del arenero
        Console(yard, ConsoleStation.Kind.TestBench, (Vector2)(c - yard.position) + new Vector2(0, 0.2f), new Vector2(10f, 6f), (Vector2)(c - yard.position) + new Vector2(0.5f, 2.2f), 3.2f, promptTemplate);
        return rover.transform;
    }

    static void Console(Transform host, ConsoleStation.Kind kind, Vector2 offset, Vector2 size, Vector2 promptOff, float tipH, GameObject promptTemplate)
    {
        if (host == null) return;
        var t = new GameObject("Console_" + kind); t.transform.SetParent(host, false); t.transform.localPosition = offset;
        var bc = t.AddComponent<BoxCollider2D>(); bc.isTrigger = true; bc.size = size;
        var cs = t.AddComponent<ConsoleStation>(); bc.isTrigger = true;
        var pr = Object.Instantiate(promptTemplate, t.transform); pr.name = "Prompt"; pr.transform.localPosition = promptOff - offset + new Vector2(0, 0); pr.SetActive(false);
        pr.transform.position = host.position + (Vector3)promptOff;
        var so = new SerializedObject(cs);
        so.FindProperty("kind").enumValueIndex = (int)kind;
        so.FindProperty("interactPrompt").objectReferenceValue = pr;
        so.FindProperty("tooltipHeight").floatValue = tipH;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
