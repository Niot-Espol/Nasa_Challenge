// Assets/_Project/Scripts/Model/MissionResult.cs
// C# puro: lo que pasó en la exploración. Lo escribe la escena de la misión al terminar, lo lee 05_Report.
using System.Collections.Generic;

// Puntos científicos de todas las misiones (cada uno pertenece a una misión y exige un instrumento).
// Marte: Delta, MineralOutcrop, SpecialRock · Luna: RimRock, Regolith, Ice. No cambiar el orden (se guarda en escenas).
public enum ScienceSite { Delta, MineralOutcrop, SpecialRock, RimRock, Regolith, Ice }

// Cosas que pasaron durante la exploración y que el Reporte relaciona con una decisión del Taller.
public enum IncidentType
{
    DustStorm,          // tormenta de polvo (con paneles: recarga 0)
    CommsBlackout,      // pérdida de comunicación (duración según la antena)
    SafeMode,           // energía en 0: el rover se detuvo hasta recargar
    MissingInstrument,  // llegó a un punto sin el instrumento necesario
    RockyZoneBlocked,   // no pudo cruzar la zona rocosa (ruedas estándar)
    TimeOut,            // se acabó el tiempo de la misión
    Shadow,             // entró en una zona en sombra (Luna: sin Sol y con frío)
}

public class MissionIncident
{
    public IncidentType Type;
    public float Time;          // segundo de la misión en que ocurrió
    public float Duration;      // cuánto duró (0 si fue instantáneo)
    public ScienceSite? Site;   // punto relacionado, si aplica

    public MissionIncident(IncidentType type, float time, float duration = 0f, ScienceSite? site = null)
    {
        Type = type; Time = time; Duration = duration; Site = site;
    }
}

public class MissionResult
{
    public HashSet<ScienceSite> CompletedSites = new HashSet<ScienceSite>();
    public List<MissionIncident> Incidents = new List<MissionIncident>();

    public float TimeLimitSeconds;   // duración total de la misión
    public float TimeUsedSeconds;
    public float EnergyConsumed;     // energía total gastada (moverse + escanear)
    public bool DataTransmitted;     // se enviaron los datos al final

    public int ScienceCount => CompletedSites.Count;     // 0 a 3 = estrellas
    public int Stars => CompletedSites.Count;

    public int CountOf(IncidentType t)
    {
        int n = 0;
        foreach (var i in Incidents) if (i.Type == t) n++;
        return n;
    }

    public bool NeverEnteredSafeMode => CountOf(IncidentType.SafeMode) == 0;

    // Resultado de prueba para dar Play directo en 05_Report sin jugar Marte.
    // Caso típico con paneles: 2 de 3 puntos, la tormenta dejó al rover en modo seguro.
    public static MissionResult CreateDebugDefault()
    {
        var r = new MissionResult
        {
            TimeLimitSeconds = 150f,
            TimeUsedSeconds = 150f,
            EnergyConsumed = 180f,
            DataTransmitted = true,
        };
        r.CompletedSites.Add(ScienceSite.Delta);
        r.CompletedSites.Add(ScienceSite.MineralOutcrop);
        r.Incidents.Add(new MissionIncident(IncidentType.DustStorm, 35f, 35f));
        r.Incidents.Add(new MissionIncident(IncidentType.SafeMode, 50f, 25f));
        r.Incidents.Add(new MissionIncident(IncidentType.CommsBlackout, 75f, 40f));
        r.Incidents.Add(new MissionIncident(IncidentType.TimeOut, 150f));
        return r;
    }
}
