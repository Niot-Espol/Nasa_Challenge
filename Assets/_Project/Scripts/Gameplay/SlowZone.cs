// Assets/_Project/Scripts/Gameplay/SlowZone.cs
// Zona de sedimento blando: el rover avanza más lento, salvo con ruedas todoterreno.
// Necesita un Collider2D marcado como trigger.
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SlowZone : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1f)] float speedFactor = 0.55f;
    [SerializeField] string message = "Sedimento blando: el rover avanza más lento. Con ruedas todoterreno no sería problema.";

    static bool warned;   // un solo aviso por misión

    void Reset() => GetComponent<Collider2D>().isTrigger = true;
    void Start() => warned = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        var mover = other.GetComponentInParent<TopDownMover>();
        if (mover == null || GameSession.EnsureDesign().AllTerrainWheels) return;
        mover.SpeedMultiplier = speedFactor;
        if (!warned && MissionController.Instance != null) { warned = true; MissionController.Instance.PostMessage(message); }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var mover = other.GetComponentInParent<TopDownMover>();
        if (mover != null) mover.SpeedMultiplier = 1f;
    }
}
