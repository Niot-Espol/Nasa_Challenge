// Assets/_Project/Scripts/Model/MissionState.cs
// C# puro: el estado EN VIVO de la exploración en Marte (energía, tiempo, eventos, puntos).
// MissionController (Unity) lo avanza cada frame con Tick() y al final entrega Result.
using System;

public enum ScanStatus
{
    Ok,                 // escaneo hecho
    AlreadyDone,        // ese punto ya estaba completo
    MissingInstrument,  // el diseño no lleva el instrumento que pide el punto
    NotEnoughEnergy,    // no alcanza la energía para escanear
    CommsDown,          // durante el apagón no se puede escanear ni transmitir
    SafeMode,           // el rover está detenido recargando
    MissionOver,
}

public class MissionState
{
    // Reglas de exploración (unidades de juego, ver docs/game-design/sistema-recursos-y-assets.md)
    public const float DefaultMoveCostPerSecond = 2f;   // doc decía 1; con 1 los paneles (3/s) nunca se vacían
    public const float SafeModeResumeFraction = 0.3f;   // sale del modo seguro al recargar 30 %
    public const float DefaultTimeLimit = 240f;          // 4 minutos reales

    public readonly RoverDesign Design;
    public readonly MissionResult Result = new MissionResult();

    public float Energy { get; private set; }
    public float MaxEnergy => Design.BatteryCapacity;
    public float Time { get; private set; }
    public float TimeLimit { get; }
    public float MoveCostPerSecond { get; }
    public float TimeLeft => Math.Max(0f, TimeLimit - Time);

    public bool InSafeMode { get; private set; }
    public bool DustStormActive { get; private set; }
    public bool CommsDown { get; private set; }
    public bool IsOver { get; private set; }

    // Recarga actual: los paneles no recargan durante la tormenta.
    public float CurrentRecharge => (DustStormActive && Design.UsesSolar) ? 0f : Design.RechargePerSecond;

    // Avisos para la UI / efectos (no son obligatorios de escuchar).
    public event Action<bool> SafeModeChanged;
    public event Action<ScienceSite> SiteCompleted;
    public event Action MissionEnded;

    MissionIncident safeModeIncident, stormIncident, blackoutIncident;

    public MissionState(RoverDesign design, float timeLimit = DefaultTimeLimit,
                        float moveCostPerSecond = DefaultMoveCostPerSecond)
    {
        Design = design;
        TimeLimit = timeLimit;
        MoveCostPerSecond = moveCostPerSecond;
        Energy = design.BatteryCapacity;   // empieza con la batería llena
        Result.TimeLimitSeconds = timeLimit;
    }

    // Instrumento que exige cada punto científico.
    public static Instrument RequiredInstrument(ScienceSite site)
    {
        switch (site)
        {
            case ScienceSite.Delta: return Instrument.Camera;
            case ScienceSite.MineralOutcrop: return Instrument.Spectrometer;
            default: return Instrument.Drill;
        }
    }

    public void Tick(float dt, bool isMoving)
    {
        if (IsOver) return;

        Time += dt;
        Result.TimeUsedSeconds = Time;

        float spent = (isMoving && !InSafeMode) ? MoveCostPerSecond * dt : 0f;
        Result.EnergyConsumed += spent;
        Energy = Clamp(Energy - spent + CurrentRecharge * dt);

        if (!InSafeMode && Energy <= 0f) EnterSafeMode();
        else if (InSafeMode && Energy >= MaxEnergy * SafeModeResumeFraction) ExitSafeMode();

        if (Time >= TimeLimit) End(timedOut: true);
    }

    public ScanStatus CanScan(ScienceSite site)
    {
        if (IsOver) return ScanStatus.MissionOver;
        if (Result.CompletedSites.Contains(site)) return ScanStatus.AlreadyDone;
        if (!Design.HasInstrument(RequiredInstrument(site))) return ScanStatus.MissingInstrument;
        if (InSafeMode) return ScanStatus.SafeMode;
        if (CommsDown) return ScanStatus.CommsDown;
        if (Energy < Design.ScanCosts[RequiredInstrument(site)]) return ScanStatus.NotEnoughEnergy;
        return ScanStatus.Ok;
    }

    public ScanStatus TryScan(ScienceSite site)
    {
        var status = CanScan(site);
        if (status == ScanStatus.MissingInstrument && !HasIncident(IncidentType.MissingInstrument, site))
            Result.Incidents.Add(new MissionIncident(IncidentType.MissingInstrument, Time, 0f, site));
        if (status != ScanStatus.Ok) return status;

        float cost = Design.ScanCosts[RequiredInstrument(site)];
        Energy = Clamp(Energy - cost);
        Result.EnergyConsumed += cost;
        Result.CompletedSites.Add(site);
        SiteCompleted?.Invoke(site);
        return ScanStatus.Ok;
    }

    public void SetDustStorm(bool active)
    {
        if (active == DustStormActive || IsOver) return;
        DustStormActive = active;
        if (active) { stormIncident = new MissionIncident(IncidentType.DustStorm, Time); Result.Incidents.Add(stormIncident); }
        else if (stormIncident != null) stormIncident.Duration = Time - stormIncident.Time;
    }

    public void SetCommsDown(bool down)
    {
        if (down == CommsDown || IsOver) return;
        CommsDown = down;
        if (down) { blackoutIncident = new MissionIncident(IncidentType.CommsBlackout, Time); Result.Incidents.Add(blackoutIncident); }
        else if (blackoutIncident != null) blackoutIncident.Duration = Time - blackoutIncident.Time;
    }

    public void RecordIncident(IncidentType type, ScienceSite? site = null)
    {
        if (!HasIncident(type, site)) Result.Incidents.Add(new MissionIncident(type, Time, 0f, site));
    }

    // Termina la misión (por tiempo o porque el jugador pulsa "Terminar").
    public void End(bool timedOut)
    {
        if (IsOver) return;
        if (InSafeMode) ExitSafeMode();
        SetDustStorm(false);
        if (CommsDown) SetCommsDown(false);
        if (timedOut) Result.Incidents.Add(new MissionIncident(IncidentType.TimeOut, Time));
        Result.DataTransmitted = Result.ScienceCount > 0;
        IsOver = true;
        MissionEnded?.Invoke();
    }

    // Texto de BIMO para cada resultado de escaneo.
    public string ScanMessage(ScanStatus status, ScienceSite site)
    {
        switch (status)
        {
            case ScanStatus.Ok: return $"¡Datos obtenidos! Punto completado ({Result.ScienceCount}/3).";
            case ScanStatus.AlreadyDone: return "Ya estudiamos este punto, comandante.";
            case ScanStatus.MissingInstrument: return $"No traemos {InstrumentName(RequiredInstrument(site))}. No podemos estudiar este punto.";
            case ScanStatus.NotEnoughEnergy: return $"Necesitamos {Design.ScanCosts[RequiredInstrument(site)]:0} de energía para escanear. Esperemos a recargar.";
            case ScanStatus.CommsDown: return "Sin comunicación con la Tierra: no podemos escanear ahora.";
            case ScanStatus.SafeMode: return "Modo seguro: el rover está recargando.";
            default: return "La misión terminó.";
        }
    }

    public static string InstrumentName(Instrument i)
    {
        switch (i)
        {
            case Instrument.Camera: return "la cámara";
            case Instrument.Spectrometer: return "el espectrómetro";
            default: return "el brazo con taladro";
        }
    }

    void EnterSafeMode()
    {
        InSafeMode = true;
        Energy = 0f;
        safeModeIncident = new MissionIncident(IncidentType.SafeMode, Time);
        Result.Incidents.Add(safeModeIncident);
        SafeModeChanged?.Invoke(true);
    }

    void ExitSafeMode()
    {
        InSafeMode = false;
        if (safeModeIncident != null) safeModeIncident.Duration = Time - safeModeIncident.Time;
        SafeModeChanged?.Invoke(false);
    }

    bool HasIncident(IncidentType type, ScienceSite? site)
    {
        foreach (var i in Result.Incidents) if (i.Type == type && i.Site == site) return true;
        return false;
    }

    float Clamp(float e) => Math.Max(0f, Math.Min(MaxEnergy, e));
}
