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
    [SerializeField] float scanDuration = 2.5f;     // segundos detenido escaneando
    [SerializeField] float fastForwardSpeed = 4f;   // Espacio con el rover detenido: el tiempo pasa más rápido
    [SerializeField] string reportScene = "05_Report";

    [SerializeField] DialogueData intro;          // BIMO al aterrizar (opcional)

    [Header("Eventos (segundos desde el inicio)")]
    [SerializeField] float dustStormAt = 35f;
    [SerializeField] float dustStormDuration = 35f;
    [SerializeField] float blackoutAt = 75f;   // la duración la decide la antena del diseño

    public MissionState State { get; private set; }
    public static MissionController Instance { get; private set; }

    // Último mensaje de BIMO para el HUD (escaneos, eventos).
    public string LastMessage { get; private set; } = "Estudia los 3 puntos científicos: acércate y presiona E.";
    public event System.Action<string> MessagePosted;

    bool ended;
    float lowEnergyBeep;
    float motorLevel;   // 0 = en reposo, 1 = moviéndose (suavizado)

    [Header("Motor del rover")]
    [SerializeField, Range(0, 1)] float motorIdleVolume = 0.25f;
    [SerializeField, Range(0, 1)] float motorMovingVolume = 0.8f;
    [SerializeField] float motorIdlePitch = 0.85f;
    [SerializeField] float motorMovingPitch = 1.08f;

    void Awake()
    {
        Instance = this;
        // Play directo en 04_Mars: si no hay diseño, usa el de prueba.
        State = new MissionState(GameSession.EnsureDesign(), timeLimit, moveCostPerSecond);
        State.SafeModeChanged += on => { if (on) { AudioManager.Play(Sfx.SafeMode); CameraShake.Shake(0.15f, 0.5f); } };
        State.SafeModeChanged += on =>
        {
            if (on) OnSafeModeEntered();
            else Post("Energía recuperada. ¡Podemos seguir!");
        };
        State.MissionEnded += OnMissionEnded;
    }

    void Start()
    {
        if (intro != null && DialogueBox.Instance != null) DialogueBox.Instance.Show(intro);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (IsFastForwarding) Time.timeScale = 1f;
    }

    // --- Adelantar el tiempo (mantener Espacio con el rover detenido) ---
    public bool IsFastForwarding { get; private set; }
    public float FastForwardSpeed => fastForwardSpeed;
    const string FastForwardTip = "Consejo: mientras esperas, mantén presionada la barra ESPACIO (con el rover quieto) para adelantar el tiempo.";

    // ¿Está el jugador en una situación en la que solo puede esperar? (solo aquí se permite adelantar el tiempo)
    // Modo seguro, apagón de comunicación o tormenta con paneles solares (no recargan).
    public bool IsWaiting =>
        !State.IsOver && (State.InSafeMode || State.CommsDown || (State.DustStormActive && State.Design.UsesSolar));

    void UpdateFastForward()
    {
        // Diálogos y pausa controlan el tiempo (lo ponen en 0): no interferir
        if (DialogueBox.IsOpen || Time.timeScale == 0f) { IsFastForwarding = false; return; }
        var k = UnityEngine.InputSystem.Keyboard.current;
        bool held = k != null && k.spaceKey.isPressed;
        bool stopped = rover == null || !rover.IsMoving;
        bool ff = held && stopped && IsWaiting && DialogueBox.InputFree;
        IsFastForwarding = ff;
        Time.timeScale = ff ? fastForwardSpeed : 1f;
    }

    void Update()
    {
        if (State.IsOver) return;

        State.Tick(Time.deltaTime, rover != null && rover.IsMoving);
        if (rover != null) rover.CanMove = !State.InSafeMode && !State.IsOver && !IsScanning;

        UpdateEvents();
        UpdateAudio();
        UpdateFastForward();
    }

    void UpdateEvents()
    {
        float t = State.Time;

        bool storm = t >= dustStormAt && t < dustStormAt + dustStormDuration;
        if (storm != State.DustStormActive)
        {
            State.SetDustStorm(storm);
            if (storm) { AudioManager.Play(Sfx.DustStorm); CameraShake.Shake(0.35f, 1.2f); }
            if (storm) Explain(State.Design.UsesSolar
                ? new[] {
                    "¡Tormenta de polvo! El polvo tapa el Sol: nuestros paneles solares NO van a recargar durante unos " + dustStormDuration.ToString("0") + " segundos.",
                    "Si la energía llega a cero, el rover se detendrá hasta que pase la tormenta. ¡Muévete solo lo necesario!",
                    FastForwardTip }
                : new[] { "¡Tormenta de polvo! Por suerte nuestro RTG no depende del Sol: seguimos recargando con normalidad." });
            CameraShake.SetContinuous(storm ? 0.06f : 0f);
            if (storm) Post(State.Design.UsesSolar
                ? "Tormenta: los paneles solares no recargan mientras dure."
                : "Tormenta: el RTG sigue dando energía.");
            else Post("La tormenta pasó. Los paneles vuelven a recargar.");
        }

        float blackoutEnd = blackoutAt + State.Design.BlackoutSeconds;
        bool down = t >= blackoutAt && t < blackoutEnd;
        if (down != State.CommsDown)
        {
            State.SetCommsDown(down);
            AudioManager.Play(down ? Sfx.CommsLost : Sfx.CommsBack);
            if (down) CameraShake.Shake(0.1f, 0.3f);
            if (down) Explain(State.Design.BlackoutSeconds > 20f
                ? new[] { "¡Perdimos la comunicación con la Tierra! Durante " + State.Design.BlackoutSeconds.ToString("0") + " segundos no podremos escanear.",
                          "Con un relé vía orbitador en vez de la antena directa, el apagón duraría solo 10 segundos.",
                          FastForwardTip }
                : new[] { "Perdimos la comunicación, pero gracias al relé vía orbitador volverá en unos " + State.Design.BlackoutSeconds.ToString("0") + " segundos." });
            Post(down
                ? $"¡Perdimos la comunicación con la Tierra! ({State.Design.BlackoutSeconds:0} s sin poder escanear)"
                : "Comunicación restablecida.");
        }
    }

    void UpdateAudio()
    {
        bool running = Time.timeScale > 0f && !State.IsOver;
        bool moving = running && rover != null && rover.IsMoving && rover.CanMove;
        motorLevel = Mathf.MoveTowards(motorLevel, moving ? 1f : 0f, Time.unscaledDeltaTime * 3f);
        float vol = running ? Mathf.Lerp(motorIdleVolume, motorMovingVolume, motorLevel) : 0f;
        AudioManager.SetLoopLevel(Loop.RoverMotor, vol, Mathf.Lerp(motorIdlePitch, motorMovingPitch, motorLevel));
        AudioManager.SetLoop(Loop.Wind, running && State.DustStormActive);
        AudioManager.SetLoop(Loop.Static, running && State.CommsDown);

        // Alarma intermitente con energía baja (no en modo seguro: ahí suena el apagado)
        bool low = running && !State.InSafeMode && State.Energy < State.MaxEnergy * LowEnergyFraction;
        lowEnergyBeep -= Time.deltaTime;
        if (low && lowEnergyBeep <= 0f) { AudioManager.Play(Sfx.LowEnergy); lowEnergyBeep = 1.6f; }
    }

    public const float LowEnergyFraction = 0.25f;

    // --- Escaneo con duración: el rover se detiene; el costo y el resultado se aplican al final ---
    public bool IsScanning { get; private set; }
    public ScienceSite ScanningSite { get; private set; }
    public float ScanProgress { get; private set; }
    public float ScanDuration => scanDuration;

    public System.Collections.IEnumerator ScanRoutine(ScienceSite site, System.Action<ScanStatus> done)
    {
        var pre = State.CanScan(site);
        if (pre != ScanStatus.Ok) { done?.Invoke(Scan(site)); yield break; }
        IsScanning = true; ScanProgress = 0f; ScanningSite = site;
        AudioManager.Play(Sfx.Scan);
        Post("Escaneando...");
        while (ScanProgress < 1f)
        {
            if (State.IsOver) { IsScanning = false; yield break; }
            ScanProgress += Time.deltaTime / Mathf.Max(0.1f, scanDuration);
            yield return null;
        }
        IsScanning = false; ScanProgress = 0f;
        done?.Invoke(Scan(site));
    }

    public ScanStatus Scan(ScienceSite site)
    {
        var status = State.TryScan(site);
        if (status != ScanStatus.Ok) AudioManager.Play(Sfx.ScanFail);
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
        AudioManager.SetLoopLevel(Loop.RoverMotor, 0f, 1f);
        IsFastForwarding = false; Time.timeScale = 1f;
        AudioManager.SetLoop(Loop.Wind, false);
        AudioManager.SetLoop(Loop.Static, false);
        CameraShake.SetContinuous(0f);
        Post("Transmitiendo datos al centro de control...");
        Invoke(nameof(GoToReport), 1.5f);
    }

    void GoToReport() => SceneFlow.Go(reportScene);

    public void PostMessage(string msg) => Post(msg);

    // Explicación importante: abre el cuadro de diálogo de BIMO (pausa el juego) y deja el mensaje en el HUD.
    void Explain(string[] lines)
    {
        Post(lines[0]);
        if (DialogueBox.Instance == null) return;
        var dl = new DialogueLine[lines.Length];
        for (int i = 0; i < lines.Length; i++) dl[i] = new DialogueLine { speaker = Speaker.Bimo, expression = Expression.Worried, text = lines[i] };
        DialogueBox.Instance.Show(dl);
    }

    public float StormTimeLeft => State.DustStormActive ? Mathf.Max(0f, dustStormAt + dustStormDuration - State.Time) : 0f;
    public float BlackoutTimeLeft => State.CommsDown ? Mathf.Max(0f, blackoutAt + State.Design.BlackoutSeconds - State.Time) : 0f;

    void OnSafeModeEntered()
    {
        bool stormBlocks = State.DustStormActive && State.Design.UsesSolar;
        if (stormBlocks)
            Explain(new[] {
                "¡Nos quedamos sin energía y la tormenta sigue tapando el Sol! Los paneles no recargan: hay que esperar unos " + StormTimeLeft.ToString("0") + " segundos a que pase.",
                "Con un RTG en lugar de paneles solares, esto no habría pasado: el RTG recarga aunque no haya Sol. Lo anotaré para el reporte.",
                FastForwardTip });
        else
        {
            float secs = State.CurrentRecharge > 0 ? State.MaxEnergy * MissionState.SafeModeResumeFraction / State.CurrentRecharge : 0f;
            Explain(new[] {
                "¡Sin energía! Modo seguro: el rover se detiene hasta recargar el 30 % (unos " + secs.ToString("0") + " segundos).",
                State.Design.BatteryCapacity < 100f
                    ? "Moverse y escanear gastan energía. Con la batería grande tendríamos más margen."
                    : "Moverse y escanear gastan energía: conviene parar a recargar antes de llegar a cero.",
                FastForwardTip });
        }
    }

    void Post(string msg)
    {
        LastMessage = msg;
        MessagePosted?.Invoke(msg);
    }
}
