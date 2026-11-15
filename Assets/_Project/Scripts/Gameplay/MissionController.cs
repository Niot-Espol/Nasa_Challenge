// Assets/_Project/Scripts/Gameplay/MissionController.cs
// Dirige la exploración en 04_Mars: crea el MissionState a partir del diseño,
// lo avanza cada frame, bloquea al rover en modo seguro, dispara los eventos
// y al terminar guarda el resultado en GameSession y pasa al Reporte.
using UnityEngine;

public class MissionController : MonoBehaviour
{
    [SerializeField] TopDownMover rover;
    [SerializeField] float timeLimit = MissionState.DefaultTimeLimit;
    [SerializeField] float moveCostPerSecond = MissionState.DefaultMoveCostPerSecond;
    [SerializeField] string reportScene = "05_Report";

    [SerializeField] DialogueData intro;          // BIMO al aterrizar (opcional)

    [Header("Eventos (segundos desde el inicio)")]
    [SerializeField] float dustStormAt = 45f;
    [SerializeField] float dustStormDuration = 30f;
    [SerializeField] float blackoutAt = 100f;   // la duración la decide la antena del diseño

    public MissionState State { get; private set; }
    public static MissionController Instance { get; private set; }

    // Último mensaje de BIMO para el HUD (escaneos, eventos).
    public string LastMessage { get; private set; } = "Estudia los 3 puntos científicos: acércate y presiona E.";
    public event System.Action<string> MessagePosted;

    bool ended;

    void Awake()
    {
        Instance = this;
        // Play directo en 04_Mars: si no hay diseño, usa el de prueba.
        State = new MissionState(GameSession.EnsureDesign(), timeLimit, moveCostPerSecond);
        State.SafeModeChanged += on => { if (on) AudioManager.Play(Sfx.SafeMode); };
        State.SafeModeChanged += on => Post(on
            ? "¡Sin energía! Modo seguro: el rover se detiene hasta recargar el 30 %."
            : "Energía recuperada. Podemos seguir.");
        State.MissionEnded += OnMissionEnded;
    }

    void Start()
    {
        if (intro != null && DialogueBox.Instance != null) DialogueBox.Instance.Show(intro);
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    void Update()
    {
        if (State.IsOver) return;

        State.Tick(Time.deltaTime, rover != null && rover.IsMoving);
        if (rover != null) rover.CanMove = !State.InSafeMode && !State.IsOver;

        UpdateEvents();
    }

    void UpdateEvents()
    {
        float t = State.Time;

        bool storm = t >= dustStormAt && t < dustStormAt + dustStormDuration;
        if (storm != State.DustStormActive)
        {
            State.SetDustStorm(storm);
            if (storm) AudioManager.Play(Sfx.DustStorm);
            if (storm) Post(State.Design.UsesSolar
                ? "¡Tormenta de polvo! Los paneles solares no recargan mientras dure."
                : "Tormenta de polvo. El RTG sigue dando energía sin problema.");
            else Post("La tormenta pasó.");
        }

        float blackoutEnd = blackoutAt + State.Design.BlackoutSeconds;
        bool down = t >= blackoutAt && t < blackoutEnd;
        if (down != State.CommsDown)
        {
            State.SetCommsDown(down);
            AudioManager.Play(down ? Sfx.CommsLost : Sfx.CommsBack);
            Post(down
                ? $"¡Perdimos la comunicación con la Tierra! ({State.Design.BlackoutSeconds:0} s sin poder escanear)"
                : "Comunicación restablecida.");
        }
    }

    public ScanStatus Scan(ScienceSite site)
    {
        var status = State.TryScan(site);
        AudioManager.Play(status == ScanStatus.Ok ? Sfx.SiteComplete : Sfx.ScanFail);
        Post(State.ScanMessage(status, site));
        if (status == ScanStatus.Ok && State.Result.ScienceCount == 3)
        {
            Post("¡Los 3 puntos completados! Transmitiendo los datos a la Tierra...");
            Invoke(nameof(FinishMission), 2.5f);   // tiempo escalado: espera a que se cierre el popup
        }
        return status;
    }

    // Botón "Terminar misión" (o fin de tiempo).
    public void FinishMission() => State.End(timedOut: false);

    void OnMissionEnded()
    {
        if (ended) return;
        ended = true;
        GameSession.Result = State.Result;
        if (rover != null) rover.CanMove = false;
        Post("Transmitiendo datos al centro de control...");
        Invoke(nameof(GoToReport), 1.5f);
    }

    void GoToReport() => SceneFlow.Go(reportScene);

    public void PostMessage(string msg) => Post(msg);

    void Post(string msg)
    {
        LastMessage = msg;
        MessagePosted?.Invoke(msg);
    }
}
