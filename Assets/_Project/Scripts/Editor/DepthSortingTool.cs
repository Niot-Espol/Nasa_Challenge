// Assets/_Project/Scripts/Editor/DepthSortingTool.cs
// Menú "Mars Crafts/Orden por profundidad": deja la escena abierta lista para ordenar por Y.
//  - Añade YSortCamera a la cámara principal.
//  - Los sprites del mundo (sorting order 0..10: personajes, muebles, rocas, rover) comparan por su pivote.
//  - El rover tiene el pivote en el centro del dibujo: se separa el dibujo en un hijo "Visual" y la raíz
//    (en la base, donde toca el suelo) lleva un SortingGroup, que ordena por la posición de la raíz.
// Se puede volver a lanzar cuando se añadan objetos nuevos. Para deshacerlo todo: revertir el commit.
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class DepthSortingTool
{
    const int MinOrder = 0, MaxOrder = 10;   // fuera de este rango: suelo, paredes e indicadores (no se tocan)

    [MenuItem("Mars Crafts/Orden por profundidad/Aplicar en la escena abierta")]
    public static void ApplyToOpenScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        int sprites = 0;

        var cam = Camera.main;
        if (cam != null && cam.GetComponent<YSortCamera>() == null) Undo.AddComponent<YSortCamera>(cam.gameObject);

        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
        {
            if (sr.GetComponentInParent<Canvas>() != null) continue;
            if (sr.sortingOrder < MinOrder || sr.sortingOrder > MaxOrder) continue;
            if (sr.spriteSortPoint == SpriteSortPoint.Pivot) continue;
            Undo.RecordObject(sr, "Orden por profundidad");
            sr.spriteSortPoint = SpriteSortPoint.Pivot;
            sprites++;
        }

        bool rover = FixRover();
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log($"[Orden por profundidad] {scene.name}: cámara lista, {sprites} sprites ahora usan su pivote" + (rover ? ", rover ajustado." : "."));
    }

    // Rover: raíz en la base (centro del collider) + SortingGroup; el dibujo pasa a un hijo "Visual".
    static bool FixRover()
    {
        var mover = Object.FindFirstObjectByType<DirectionalSpriteAnimator>();
        if (mover == null) return false;
        var root = mover.gameObject;
        var sr = root.GetComponent<SpriteRenderer>();
        if (sr == null) return false;   // ya está separado
        var col = root.GetComponent<BoxCollider2D>();

        // Cuánto bajar la raíz para que quede en la base del rover
        float dy = col != null ? col.offset.y : 0f;

        var visual = new GameObject("Visual");
        Undo.RegisterCreatedObjectUndo(visual, "Rover visual");
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = new Vector3(0f, -dy, 0f);
        var vsr = Undo.AddComponent<SpriteRenderer>(visual);
        EditorUtility.CopySerialized(sr, vsr);
        vsr.spriteSortPoint = SpriteSortPoint.Pivot;
        Undo.DestroyObjectImmediate(sr);

        Undo.RecordObject(root.transform, "Rover base");
        root.transform.position += new Vector3(0f, dy, 0f);
        if (col != null) { Undo.RecordObject(col, "Rover base"); col.offset = new Vector2(col.offset.x, 0f); }

        var so = new SerializedObject(mover);
        so.FindProperty("target").objectReferenceValue = vsr;
        so.ApplyModifiedProperties();

        var group = root.GetComponent<SortingGroup>();
        if (group == null) group = Undo.AddComponent<SortingGroup>(root);
        group.sortingOrder = vsr.sortingOrder;
        return true;
    }
}
