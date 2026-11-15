// Assets/_Project/Scripts/UI/PauseMenu.cs
// Menú de pausa con Esc (en todas las escenas menos el Título y el Lanzamiento).
// Se crea solo; su aspecto sale del UITheme.
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    static PauseMenu instance;
    GameObject root;
    bool paused;
    float prevTimeScale = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (instance != null) return;
        var go = new GameObject("PauseMenu");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<PauseMenu>();
        instance.Build();
        SceneManager.sceneLoaded += (s, m) => instance.SetPaused(false);
    }

    void Build()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 900;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        gameObject.AddComponent<GraphicRaycaster>();

        var overlay = UIKit.Panel(transform, "Overlay", UIElement.Overlay);
        UIKit.Fill(overlay.rectTransform);
        root = overlay.gameObject;

        var panel = UIKit.Panel(overlay.transform, "Panel", UIElement.Panel);
        UIKit.Place(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560, 420));
        UIKit.VList(panel.gameObject, 18, 36, TextAnchor.UpperCenter);
        UIKit.Size(UIKit.Text(panel.transform, "Title", "PAUSA", 48, TMPro.TextAlignmentOptions.Center, TextTone.Accent), -1, 70);
        UIKit.Size(UIKit.Button(panel.transform, "Resume", "Continuar", 30, UIElement.ButtonPrimary, () => SetPaused(false)), -1, 76);
        UIKit.Size(UIKit.Button(panel.transform, "Restart", "Reiniciar esta etapa", 30, UIElement.Button, Restart), -1, 76);
        UIKit.Size(UIKit.Button(panel.transform, "Title", "Volver al título", 30, UIElement.Button, BackToTitle), -1, 76);
        root.SetActive(false);
    }

    void Update()
    {
        var k = Keyboard.current;
        if (k == null || !k.escapeKey.wasPressedThisFrame) return;
        string scene = SceneManager.GetActiveScene().name;
        if (scene == SceneFlow.Title || scene == SceneFlow.Launch) return;
        if (DialogueBox.IsOpen && !paused) return;
        SetPaused(!paused);
    }

    void SetPaused(bool p)
    {
        if (p == paused) return;
        paused = p;
        if (p) { prevTimeScale = Time.timeScale; Time.timeScale = 0f; }
        else Time.timeScale = prevTimeScale <= 0f && !DialogueBox.IsOpen ? 1f : prevTimeScale;
        if (root != null) root.SetActive(p);
        AudioManager.Play(Sfx.Click);
    }

    void Restart()
    {
        SetPaused(false);
        Time.timeScale = 1f;
        SceneFlow.Go(SceneManager.GetActiveScene().name);
    }

    void BackToTitle()
    {
        SetPaused(false);
        Time.timeScale = 1f;
        GameSession.Reset();
        SceneFlow.Go(SceneFlow.Title);
    }
}
