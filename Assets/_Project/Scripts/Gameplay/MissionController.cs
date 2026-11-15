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
    [SerializeField] string missionId = "mars";    // misión de esta escena (para dar Play directo en ella)

    [Header("Eventos programados (Marte). En otras misiones se desactivan y los eventos vienen del mapa")]
    [SerializeField] bool timedDustStorm = true;
    [SerializeField] bool timedBlackout = true;
    [SerializeField] float dustStormAt = 35f;
    [SerializeField] float dustStormDuration = 35f;
    [SerializeField] float blackoutAt = 75f;   // lo más pronto que puede empezar; la duración la decide la antena
    [SerializeField] float minFreeGap = 25f;   // segundos de juego libre entre dos esperas (tormenta/modo seguro → apagón)
    [SerializeField] float blackoutLatest = 25f; // no empieza si quedan menos de estos segundos de misión

    float blackoutStart = -1f;   // < 0: aún no ha ocurrido
    float lastWaitEnd;           // último instante en que el jugador estaba esperando (tormenta con paneles o modo seguro)

    public MissionState State { get; private set; }
    public static MissionController Instance { get; private set; }

    // Último mensaje de BIMO para el HUD (escaneos, eventos).
    public string LastMessage { get; private set; } = Loc.T("Estudia los 3 puntos científicos: acércate y presiona E.", "Study the 3 science sites: get close and press E.");
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
        // Play directo en la escena (sin pasar por el título): la misión es la de esta escena
        if (GameSession.Mission == null || GameSession.Mission.SceneName != gameObject.scene.name)
            GameSession.Mission = MissionCatalog.Get(missionId);
        State = new MissionState(GameSession.EnsureDesign(), timeLimit, moveCostPerSecond, GameSession.Mission);
        State.SafeModeChanged += on => { if (on) { AudioManager.Play(Sfx.SafeMode); CameraShake.Shake(0.15f, 0.5f); } };
        State.SafeModeChanged += on =>
        {
            if (on) OnSafeModeEntered();
            else Post(Loc.T("Energía recuperada. ¡Podemos seguir!", "Energy restored. We can keep going!"));
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
    static string FastForwardTip => Loc.T("Consejo: mientras esperas, mantén presionada la barra ESPACIO (con el rover quieto) para adelantar el tiempo.", "Tip: while you wait, hold the SPACE bar (with the rover stopped) to fast-forward time.");

    // ¿Está el jugador en una situación en la que solo puede esperar? (solo aquí se permite adelantar el tiempo)
    // Modo seguro, apagón de comunicación o tormenta con paneles solares (no recargan).
    // (Una zona sin señal NO cuenta: ahí lo que toca es salir del cráter, no esperar.)
    public bool IsWaiting =>
        !State.IsOver && (State.InSafeMode || (State.CommsDown && !CommsZoneActive) || (State.DustStormActive && State.Design.UsesSolar));

    // Zonas del mapa (Luna): las controla EnvironmentZone
    public bool CommsZoneActive { get; set; }

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

        bool storm = timedDustStorm && t >= dustStormAt && t < dustStormAt + dustStormDuration;
        if (storm != State.DustStormActive)
        {
            State.SetDustStorm(storm);
            if (storm) { AudioManager.Play(Sfx.DustStorm); CameraShake.Shake(0.35f, 1.2f); }
            if (storm) Explain(State.Design.UsesSolar
                ? new[] {
                    Loc.T("¡Tormenta de polvo! El polvo tapa el Sol: nuestros paneles solares NO van a recargar durante unos " + dustStormDuration.ToString("0") + " segundos.", "Dust storm! The dust blocks the Sun: our solar panels will NOT recharge for about " + dustStormDuration.ToString("0") + " seconds."),
                    Loc.T("Si la energía llega a cero, el rover se detendrá hasta que pase la tormenta. ¡Muévete solo lo necesario!", "If energy hits zero, the rover will stop until the storm passes. Only move when you need to!"),
                    FastForwardTip }
                : new[] { Loc.T("¡Tormenta de polvo! Por suerte nuestro RTG no depende del Sol: seguimos recargando con normalidad.", "Dust storm! Luckily our RTG doesn't depend on the Sun: we keep recharging as usual.") });
            CameraShake.SetContinuous(storm ? 0.06f : 0f);
            if (storm) Post(State.Design.UsesSolar
                ? Loc.T("Tormenta: los paneles solares no recargan mientras dure.", "Storm: the solar panels won't recharge while it lasts.")
                : Loc.T("Tormenta: el RTG sigue dando energía.", "Storm: the RTG keeps supplying power."));
            else Post(Loc.T("La tormenta pasó. Los paneles vuelven a recargar.", "The storm has passed. The panels are recharging again."));
        }

        // El apagón nunca llega justo después de otra espera: necesita minFreeGap segundos de juego libre.
        if (State.InSafeMode || (State.DustStormActive && State.Design.UsesSolar)) lastWaitEnd = t;
        if (timedBlackout && blackoutStart < 0f && t >= blackoutAt && t - lastWaitEnd >= minFreeGap && t <= timeLimit - blackoutLatest)
            blackoutStart = t;
        bool down = blackoutStart >= 0f && t < blackoutStart + State.Design.BlackoutSeconds;
        if (timedBlackout && down != State.CommsDown)
        {
            State.SetCommsDown(down);
            AudioManager.Play(down ? Sfx.CommsLost : Sfx.CommsBack);
            if (down) CameraShake.Shake(0.1f, 0.3f);
            if (down) Explain(State.Design.BlackoutSeconds > 20f
                ? new[] { Loc.T("¡Perdimos la comunicación con la Tierra! Durante " + State.Design.BlackoutSeconds.ToString("0") + " segundos no podremos escanear.", "We lost contact with Earth! For " + State.Design.BlackoutSeconds.ToString("0") + " seconds we won't be able to scan."),
                          Loc.T("Con un relé vía orbitador en vez de la antena directa, el apagón duraría solo 10 segundos.", "With an orbital relay instead of the direct antenna, the blackout would last only 10 seconds."),
                          FastForwardTip }
                : new[] { Loc.T("Perdimos la comunicación, pero gracias al relé vía orbitador volverá en unos " + State.Design.BlackoutSeconds.ToString("0") + " segundos.", "We lost contact, but thanks to the orbital relay it'll be back in about " + State.Design.BlackoutSeconds.ToString("0") + " seconds.") });
            Post(down
                ? Loc.T($"¡Perdimos la comunicación con la Tierra! ({State.Design.BlackoutSeconds:0} s sin poder escanear)", $"We lost contact with Earth! ({State.Design.BlackoutSeconds:0} s unable to scan)")
                : Loc.T("Comunicación restablecida.", "Communication restored."));
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
        Post(Loc.T("Escaneando...", "Scanning..."));
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
        if (status == ScanStatus.Ok && State.Result.ScienceCount >= State.Mission.Sites.Count)
        {
            Post(Loc.T("¡Los 3 puntos completados! Transmitiendo los datos a la Tierra...", "All 3 sites complete! Sending the data to Earth..."));
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
        Post(Loc.T("Transmitiendo datos al centro de control...", "Sending data to mission control..."));
        Invoke(nameof(GoToReport), 1.5f);
    }

    void GoToReport() => SceneFlow.Go(reportScene);

    public void PostMessage(string msg) => Post(msg);

    // Explicación importante: abre el cuadro de diálogo de BIMO (pausa el juego) y deja el mensaje en el HUD.
    public void Explain(string[] lines)
    {
        Post(lines[0]);
        if (DialogueBox.Instance == null) return;
        var dl = new DialogueLine[lines.Length];
        for (int i = 0; i < lines.Length; i++) dl[i] = new DialogueLine { speaker = Speaker.Bimo, expression = Expression.Worried, text = lines[i] };
        DialogueBox.Instance.Show(dl);
    }

    public float StormTimeLeft => State.DustStormActive ? Mathf.Max(0f, dustStormAt + dustStormDuration - State.Time) : 0f;
    public float BlackoutTimeLeft => State.CommsDown ? Mathf.Max(0f, blackoutStart + State.Design.BlackoutSeconds - State.Time) : 0f;

    void OnSafeModeEntered()
    {
        bool stormBlocks = State.DustStormActive && State.Design.UsesSolar;
        if (stormBlocks)
            Explain(new[] {
                Loc.T("¡Nos quedamos sin energía y la tormenta sigue tapando el Sol! Los paneles no recargan: hay que esperar unos " + StormTimeLeft.ToString("0") + " segundos a que pase.", "We're out of energy and the storm is still blocking the Sun! The panels can't recharge: we have to wait about " + StormTimeLeft.ToString("0") + " seconds for it to pass."),
                Loc.T("Con un RTG en lugar de paneles solares, esto no habría pasado: el RTG recarga aunque no haya Sol. Lo anotaré para el reporte.", "With an RTG instead of solar panels, this wouldn't have happened: the RTG recharges even without sunlight. I'll note it for the report."),
                FastForwardTip });
        else
        {
            float secs = State.CurrentRecharge > 0 ? State.MaxEnergy * MissionState.SafeModeResumeFraction / State.CurrentRecharge : 0f;
            Explain(new[] {
                Loc.T("¡Sin energía! Modo seguro: el rover se detiene hasta recargar el 30 % (unos " + secs.ToString("0") + " segundos).", "Out of energy! Safe mode: the rover stops until it recharges to 30% (about " + secs.ToString("0") + " seconds)."),
                State.Design.BatteryCapacity < 100f
                    ? Loc.T("Moverse y escanear gastan energía. Con la batería grande tendríamos más margen.", "Driving and scanning use energy. With the large battery we'd have more margin.")
                    : Loc.T("Moverse y escanear gastan energía: conviene parar a recargar antes de llegar a cero.", "Driving and scanning use energy: it's best to stop and recharge before hitting zero."),
                FastForwardTip });
        }
    }

    void Post(string msg)
    {
        LastMessage = msg;
        MessagePosted?.Invoke(msg);
    }
}
