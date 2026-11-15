// Assets/_Project/Scripts/Editor/SceneBuildUtil.cs
// Utilidades de editor para (re)armar las pantallas de forma consistente.
// Las usa la IA vía MCP y se pueden llamar desde otros scripts de editor.
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SceneBuildUtil
{
    public const string ScenesFolder = "Assets/_Project/Scenes/";

    public static Scene Open(string sceneName)
    {
        EditorSceneManager.SaveOpenScenes();
        return EditorSceneManager.OpenScene(ScenesFolder + sceneName + ".unity", OpenSceneMode.Single);
    }

    public static void Save()
    {
        var s = SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(s);
        EditorSceneManager.SaveScene(s);
    }

    // Deja un Canvas 1920x1080 vacío (borra sus hijos), EventSystem y un objeto "Flow" con SceneFlow.
    public static Canvas PrepareCanvas(bool clearChildren = true)
    {
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            var go = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.layer = 5;
            canvas = go.GetComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        if (clearChildren) UIKit.Clear(canvas.transform);

        if (Object.FindAnyObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        // Objetos viejos del esqueleto
        var old = GameObject.Find("MoveScene");
        if (old != null) Object.DestroyImmediate(old);

        Flow();
        return canvas;
    }

    public static SceneFlow Flow()
    {
        var f = Object.FindAnyObjectByType<SceneFlow>();
        if (f != null) return f;
        return new GameObject("Flow").AddComponent<SceneFlow>();
    }

    public static void SetBackground(Color c)
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = c;
    }

    // OnClick persistente (visible en el Inspector).
    public static void OnClick(Button b, UnityAction action)
    {
        for (int i = b.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            UnityEventTools.RemovePersistentListener(b.onClick, i);
        UnityEventTools.AddPersistentListener(b.onClick, action);
    }

    public static void OnClick(Button b, UnityAction<string> action, string arg)
    {
        for (int i = b.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            UnityEventTools.RemovePersistentListener(b.onClick, i);
        UnityEventTools.AddStringPersistentListener(b.onClick, action, arg);
    }

    public static void Ref(Object target, string field, Object value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
