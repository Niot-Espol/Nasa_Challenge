// Assets/_Project/Scripts/Gameplay/ScienceSitePoint.cs
// Un punto científico del mapa. Con el rover cerca, E intenta escanearlo.
// Si el escaneo sale bien, BIMO muestra un dato real con su fuente NASA.
using UnityEngine;

public class ScienceSitePoint : Interactable
{
    [SerializeField] ScienceSite site;
    [SerializeField] SpriteRenderer body;          // se aclara al completar el punto
    [SerializeField] Sprite completedSprite;       // arte pendiente (opcional)
    [SerializeField] Color completedTint = new Color(0.55f, 1f, 0.55f);
    [SerializeField] GameObject completedMarker;   // banderita "completado" (opcional)

    [Header("Dato NASA (se muestra al completar)")]
    [SerializeField] string factTitle = "Dato NASA";
    [SerializeField, TextArea(3, 6)] string fact;
    [SerializeField] string factSource;

    public ScienceSite Site => site;

    protected override void Start()
    {
        base.Start();
        if (completedMarker != null) completedMarker.SetActive(false);
        var mc = MissionController.Instance;
        if (mc != null) mc.State.SiteCompleted += OnSiteCompleted;
    }

    void OnDestroy()
    {
        var mc = MissionController.Instance;
        if (mc != null) mc.State.SiteCompleted -= OnSiteCompleted;
    }

    protected override void Interact()
    {
        var mc = MissionController.Instance;
        if (mc == null || mc.IsScanning) return;
        mc.StartCoroutine(mc.ScanRoutine(site, status =>
        {
            if (status != ScanStatus.Ok) return;
            if (!string.IsNullOrEmpty(fact) && DialogueBox.Instance != null)
                DialogueBox.Instance.ShowFact(factTitle, fact, factSource, () => AudioManager.Play(Sfx.SiteComplete));
            else
                AudioManager.Play(Sfx.SiteComplete);
        }));
    }

    void OnSiteCompleted(ScienceSite s)
    {
        if (s != site) return;
        if (body != null)
        {
            if (completedSprite != null) body.sprite = completedSprite;
            else body.color = completedTint;
        }
        if (completedMarker != null) completedMarker.SetActive(true);
    }
}
