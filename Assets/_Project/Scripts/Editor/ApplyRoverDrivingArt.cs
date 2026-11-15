using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Asigna los frames de conducción al rover de Marte sin cambiar su movimiento ni colisiones.
public static class ApplyRoverDrivingArt
{
    const string ScenePath = "Assets/_Project/Scenes/04_Mars.unity";
    const string ArtRoot = "Assets/_Project/Art/Sprites/Rover/DrivingV2/";

    [MenuItem("Mars Crafts/Art/Aplicar animación del rover a Marte")]
    public static void Apply()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        DirectionalSpriteAnimator animator = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            var candidate = root.GetComponentsInChildren<DirectionalSpriteAnimator>(true);
            foreach (var item in candidate)
                if (item.gameObject.name == "Rover") animator = item;
        }
        if (animator == null)
        {
            Debug.LogError("No se encontró el DirectionalSpriteAnimator del Rover en 04_Mars.");
            return;
        }

        var directions = new[] { "South", "North", "East" };
        var fields = new[] { "down", "up", "side" };
        var frames = new Sprite[3][];
        for (int dir = 0; dir < 3; dir++)
        {
            frames[dir] = new Sprite[4];
            for (int i = 0; i < 4; i++)
            {
                string path = ArtRoot + directions[dir] + "/drive_" + directions[dir].ToLowerInvariant() + "_" + i.ToString("000") + ".png";
                frames[dir][i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (frames[dir][i] != null) continue;
                Debug.LogError("Falta el sprite: " + path);
                return;
            }
        }

        Undo.RecordObject(animator, "Aplicar animación del rover");
        var serialized = new SerializedObject(animator);
        for (int dir = 0; dir < 3; dir++)
        {
            var array = serialized.FindProperty(fields[dir]);
            array.arraySize = 4;
            for (int i = 0; i < 4; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = frames[dir][i];
        }
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(animator);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Rover: 4 frames por dirección asignados en 04_Mars. La izquierda usa flipX.");
    }
}
