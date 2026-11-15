// Assets/_Project/Scripts/Gameplay/RockyZone.cs
// Zona rocosa: con ruedas todoterreno se cruza; con ruedas estándar es una pared.
// Si el rover choca sin poder cruzarla, BIMO lo avisa y queda registrado para el Reporte.
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RockyZone : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] visuals;
    [SerializeField] Color passableTint = new Color(1f, 1f, 1f, 0.75f);

    bool warned;

    void Start()
    {
        bool canCross = GameSession.EnsureDesign().AllTerrainWheels;
        GetComponent<Collider2D>().isTrigger = canCross;   // trigger = se atraviesa
        if (canCross && visuals != null)
            foreach (var v in visuals) if (v != null) v.color *= passableTint;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (warned || c.collider.GetComponentInParent<TopDownMover>() == null) return;
        warned = true;
        var mc = MissionController.Instance;
        if (mc == null) return;
        mc.State.RecordIncident(IncidentType.RockyZoneBlocked);
        mc.PostMessage(Loc.T("Zona rocosa: con ruedas estándar no podemos cruzarla. Hay que rodearla.", "Rocky zone: with standard wheels we can't cross it. We have to go around."));
    }
}
