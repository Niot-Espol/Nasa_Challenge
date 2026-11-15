using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// Prepara clips estándar de Unity; no cambia escenas, prefabs ni sistemas de movimiento.
public static class GeneratedNpcArtClips
{
    const string Art = "Assets/_Project/Art/";

    [MenuItem("Mars Crafts/Art/Prepare generated NPC clips")]
    static void Prepare()
    {
        AssetDatabase.Refresh();
        foreach (string action in new[] { "Idle/South", "Move/South", "Move/North", "Move/East" })
        {
            string[] parts = action.Split('/');
            CreateClip("BIMO", action, parts[0].ToLowerInvariant() + "_" + parts[1].ToLowerInvariant(), 4,
                parts[0] == "Idle" ? 4f : 6f);
        }
        CreateClip("Mateo", "Idle/South", "idle_south", 2, 2f);
        CreateClip("Luna", "Idle/South", "idle_south", 2, 2f);
        AssetDatabase.SaveAssets();
        Debug.Log("Clips NPC creados. No se modificaron escenas ni prefabs. Consultar GENERATED_ASSETS_REPORT.md.");
    }

    static void CreateClip(string character, string action, string prefix, int count, float fps)
    {
        string folder = Art + "Animations/Characters/" + character;
        EnsureFolder(folder);
        string name = character + "_" + action.Replace('/', '_');
        string destination = folder + "/" + name + ".anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(destination);
        if (clip == null)
        {
            if (File.Exists(destination)) throw new InvalidOperationException("El destino no es un clip: " + destination);
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, destination);
        }
        clip.frameRate = fps;
        var keys = new ObjectReferenceKeyframe[count + 1];
        for (int i = 0; i <= count; i++)
        {
            string file = Art + "Sprites/Characters/" + character + "/" + action + "/" + prefix + "_" + (i % count).ToString("000") + ".png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(file);
            if (sprite == null) throw new InvalidOperationException("No se pudo cargar " + file);
            keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = sprite };
        }
        AnimationUtility.SetObjectReferenceCurve(clip,
            new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" }, keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);

        if (action != "Idle/South") return;
        string controllerPath = folder + "/" + character + "Idle.controller";
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null) return;
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var state = controller.layers[0].stateMachine.AddState("Idle");
        state.motion = clip;
        controller.layers[0].stateMachine.defaultState = state;
        EditorUtility.SetDirty(controller);
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = path.Substring(0, path.LastIndexOf('/'));
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, path.Substring(path.LastIndexOf('/') + 1));
    }
}
