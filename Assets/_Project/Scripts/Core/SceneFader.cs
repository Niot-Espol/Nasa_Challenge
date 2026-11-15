// Assets/_Project/Scripts/Core/SceneFader.cs
// Fundido a negro entre escenas. Se crea solo la primera vez y sobrevive a los cambios de escena.
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [SerializeField] float duration = 0.35f;
    Image overlay;
    bool busy;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("SceneFader");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<SceneFader>();
        Instance.Build();
    }

    void Build()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        var img = new GameObject("Overlay", typeof(RectTransform)).AddComponent<Image>();
        img.transform.SetParent(transform, false);
        var rt = img.rectTransform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
        img.color = new Color(0, 0, 0, 0);
        img.raycastTarget = false;
        overlay = img;
    }

    public void FadeTo(string sceneName)
    {
        if (busy) return;
        StartCoroutine(Run(sceneName));
    }

    IEnumerator Run(string sceneName)
    {
        busy = true;
        overlay.raycastTarget = true;
        yield return Fade(0f, 1f);
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return Fade(1f, 0f);
        overlay.raycastTarget = false;
        busy = false;
    }

    IEnumerator Fade(float from, float to)
    {
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            overlay.color = new Color(0, 0, 0, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        overlay.color = new Color(0, 0, 0, to);
    }
}
