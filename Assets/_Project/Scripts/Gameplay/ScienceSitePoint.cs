// Assets/_Project/Scripts/Gameplay/ScienceSitePoint.cs
// Un punto científico del mapa. Cuando el rover entra en su zona (trigger),
// muestra el indicador "E"; al presionar E intenta escanearlo.
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class ScienceSitePoint : MonoBehaviour
{
    [SerializeField] ScienceSite site;
    [SerializeField] GameObject interactPrompt;   // indicador "E" (se muestra con el rover cerca)
    [SerializeField] SpriteRenderer body;         // se aclara al completar el punto
    [SerializeField] Color completedTint = new Color(0.55f, 1f, 0.55f);

    bool roverInRange;

    public ScienceSite Site => site;

    void Reset() => GetComponent<Collider2D>().isTrigger = true;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        var mc = MissionController.Instance;
        if (mc != null) mc.State.SiteCompleted += OnSiteCompleted;
    }

    void OnDestroy()
    {
        var mc = MissionController.Instance;
        if (mc != null) mc.State.SiteCompleted -= OnSiteCompleted;
    }

    void Update()
    {
        if (!roverInRange || Keyboard.current == null) return;
        if (Keyboard.current.eKey.wasPressedThisFrame && MissionController.Instance != null)
            MissionController.Instance.Scan(site);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<TopDownMover>() == null) return;
        roverInRange = true;
        if (interactPrompt != null) interactPrompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<TopDownMover>() == null) return;
        roverInRange = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void OnSiteCompleted(ScienceSite s)
    {
        if (s != site || body == null) return;
        body.color = completedTint;
    }
}
