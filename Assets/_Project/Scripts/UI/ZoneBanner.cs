// Assets/_Project/Scripts/UI/ZoneBanner.cs
// Nombre de la zona al entrar: aparece grande arriba al centro, se mantiene unos segundos y se desvanece.
// Usa tiempo sin escala (los diálogos pausan el juego) y no estorba a la caja de diálogo, que está abajo.
using System.Collections;
using UnityEngine;

// Va en el panel del cartel (con el texto como hijo); se desvanece con un CanvasGroup.
public class ZoneBanner : MonoBehaviour
{
    [SerializeField] float delay = 0.4f;
    [SerializeField] float fadeIn = 0.6f;
    [SerializeField] float hold = 3.5f;
    [SerializeField] float fadeOut = 1.2f;
    [SerializeField] float slide = 24f;     // baja un poco al aparecer

    IEnumerator Start()
    {
        var group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        var rt = (RectTransform)transform;
        Vector2 target = rt.anchoredPosition;
        group.alpha = 0f;

        yield return Wait(delay);
        for (float t = 0; t < fadeIn; t += Time.unscaledDeltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / fadeIn);
            group.alpha = k;
            rt.anchoredPosition = target + Vector2.up * slide * (1f - k);
            yield return null;
        }
        group.alpha = 1f; rt.anchoredPosition = target;
        yield return Wait(hold);
        for (float t = 0; t < fadeOut; t += Time.unscaledDeltaTime)
        {
            group.alpha = 1f - t / fadeOut;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    static IEnumerator Wait(float s)
    {
        for (float t = 0; t < s; t += Time.unscaledDeltaTime) yield return null;
    }
}
