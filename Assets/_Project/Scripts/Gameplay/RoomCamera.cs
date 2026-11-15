// Assets/_Project/Scripts/Gameplay/RoomCamera.cs
// Cámara "por pantallas" (estilo Zelda clásico): muestra una sala completa y, cuando el jugador
// cruza a la sala vecina, se desliza hasta ella. Mientras se desliza, el jugador no se mueve.
// Va en la cámara principal. Las salas se definen por su centro X (todas en fila horizontal).
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RoomCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float[] roomCentersX = { -23.5f, 0f, 23.5f };
    [SerializeField] string[] roomNames = { "Sala de telemetría", "Centro de control", "Mars Yard" };
    [SerializeField] float cameraY = 0.3f;
    [SerializeField] float slideSeconds = 0.45f;

    public int CurrentRoom { get; private set; } = -1;
    public static event Action<string> RoomChanged;

    TopDownMover mover;
    float slideT = 1f;
    Vector3 from, to;

    void Start()
    {
        if (player == null) { var p = FindAnyObjectByType<TopDownMover>(); if (p != null) player = p.transform; }
        if (player != null) mover = player.GetComponent<TopDownMover>();
        CurrentRoom = RoomOf(player != null ? player.position.x : 0f);
        transform.position = Target(CurrentRoom);
        RoomChanged?.Invoke(RoomName(CurrentRoom));
    }

    int RoomOf(float x)
    {
        int best = 0;
        for (int i = 1; i < roomCentersX.Length; i++)
            if (Mathf.Abs(x - roomCentersX[i]) < Mathf.Abs(x - roomCentersX[best])) best = i;
        return best;
    }

    // Nombres en inglés (mismo orden que roomNames)
    [SerializeField] string[] roomNamesEn = { "Telemetry room", "Mission control", "Mars Yard" };
    string RoomName(int i) => Loc.T(roomNames[i], i < roomNamesEn.Length ? roomNamesEn[i] : null);

    Vector3 Target(int room) => new Vector3(roomCentersX[room], cameraY, transform.position.z);

    void LateUpdate()
    {
        if (player == null) return;
        int room = RoomOf(player.position.x);
        if (room != CurrentRoom)
        {
            CurrentRoom = room;
            from = transform.position; to = Target(room); slideT = 0f;
            RoomChanged?.Invoke(RoomName(room));
        }
        if (slideT < 1f)
        {
            slideT = Mathf.Min(1f, slideT + Mathf.Max(Time.unscaledDeltaTime, Time.deltaTime) / slideSeconds);
            transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, slideT));
            if (mover != null) mover.CanMove = slideT >= 1f;
        }
    }
}
