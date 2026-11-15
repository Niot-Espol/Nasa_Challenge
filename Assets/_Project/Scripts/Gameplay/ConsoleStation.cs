// Assets/_Project/Scripts/Gameplay/ConsoleStation.cs
// Consola del centro de control (misión "Rescate desde el centro de control").
// Al pulsar E avisa a ControlRoomMission, que decide qué pasa según el paso de la misión.
using UnityEngine;

public class ConsoleStation : Interactable
{
    public enum Kind { Telemetry, TestBench, DeepSpaceNetwork }
    [SerializeField] Kind kind;

    public Kind Station => kind;
    public override string PromptText => ControlRoomMission.Instance != null ? ControlRoomMission.Instance.PromptFor(kind) : promptText;

    protected override void Interact()
    {
        if (ControlRoomMission.Instance != null) ControlRoomMission.Instance.Use(kind);
    }
}
