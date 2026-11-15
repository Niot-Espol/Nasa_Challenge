// Assets/_Project/Scripts/Gameplay/Interactable.cs
// Base para todo lo que se usa acercándose y presionando E (NPCs, puertas, puntos científicos).
// Necesita un Collider2D marcado como trigger. Muestra el indicador "[E]" mientras el jugador está cerca.
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject interactPrompt;   // indicador "E" sobre el objeto
    [SerializeField] protected string promptText = "Interactuar";   // texto descriptivo (ya no se muestra: basta la "E")
    [SerializeField] protected float tooltipHeight = 2.6f;          // altura del cartel sobre el objeto

    public virtual string PromptText => promptText;
    public Vector3 TooltipAnchor => transform.position + Vector3.up * tooltipHeight;

    protected bool playerInRange;

    protected virtual void Reset() => GetComponent<Collider2D>().isTrigger = true;

    protected virtual void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    protected virtual void Update()
    {
        if (!playerInRange || Keyboard.current == null || !DialogueBox.InputFree) return;
        if (Keyboard.current.eKey.wasPressedThisFrame) Interact();
    }

    protected abstract void Interact();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<TopDownMover>() == null) return;
        playerInRange = true;
        if (interactPrompt != null) interactPrompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<TopDownMover>() == null) return;
        playerInRange = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}
