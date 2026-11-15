// Assets/_Project/Scripts/Model/MissionDefinition.cs
// C# puro: todo lo que cambia de una misión a otra (Marte, Luna...).
// El resto del juego (Taller, rover, HUD, reporte) es común y lee la misión activa en GameSession.Mission.
// Para añadir una misión: crear una subclase, registrarla en MissionCatalog y hacer su escena de exploración.
using System.Collections.Generic;

// Un punto científico de una misión: qué es y qué instrumento exige.
public class SiteInfo
{
    public ScienceSite Site;
    public Instrument Instrument;
    readonly string name, nameEn, shortName, shortNameEn;
    public string Name => Loc.T(name, nameEn);                 // en frases: "el delta del antiguo río"
    public string ShortName => Loc.T(shortName, shortNameEn);  // flechas del mapa: "Delta"

    public SiteInfo(ScienceSite site, string name, string nameEn, string shortName, string shortNameEn, Instrument instrument)
    {
        Site = site; this.name = name; this.nameEn = nameEn; this.shortName = shortName; this.shortNameEn = shortNameEn; Instrument = instrument;
    }
}

public abstract class MissionDefinition
{
    public abstract string Id { get; }               // "mars", "moon"
    public abstract string DestinationName { get; }  // "Marte"
    public abstract string PlaceName { get; }        // "Cráter Jezero"
    public abstract string SceneName { get; }        // escena de exploración

    // Recarga mínima en modo seguro (reserva de emergencia). 0 = solo la fuente normal.
    // En la Luna evita quedarse atrapado para siempre en la sombra con paneles solares.
    public virtual float SafeModeMinRecharge => 0f;
    public abstract List<SiteInfo> Sites { get; }

    // Viaje (03_Launch)
    public abstract string TravelLabel(float progress01);   // texto mientras viaja la nave
    public abstract string ArrivalText { get; }

    // Reporte (05_Report)
    public abstract string ComparisonTitle { get; }
    public abstract string VegaClosing(int stars);

    // Líneas propias del entorno (tormenta, sombra...) y del terreno; van en este orden en el reporte.
    public virtual void AddEnergyLines(ReportData rep, RoverDesign d, MissionResult r) { }
    public virtual void AddTerrainLines(ReportData rep, RoverDesign d, MissionResult r) { }

    // Comunicaciones: por defecto, apagón programado y su duración según la antena (Marte).
    public virtual void AddCommsLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        var blackout = ReportBuilder.Find(r, IncidentType.CommsBlackout);
        if (blackout != null)
            rep.Lines.Add(d.BlackoutSeconds > 20f
                ? new ReportLine(ReportTone.Bad,
                    Loc.T($"Con la antena directa perdiste la comunicación {d.BlackoutSeconds:0} s y no pudiste escanear. Un relé vía orbitador lo reduce a 10 s.", $"With the direct antenna you lost contact for {d.BlackoutSeconds:0} s and couldn't scan. An orbital relay cuts that to 10 s."))
                : new ReportLine(ReportTone.Good,
                    Loc.T($"El relé vía orbitador redujo la pérdida de comunicación a solo {d.BlackoutSeconds:0} s.", $"The orbital relay cut the communication loss to just {d.BlackoutSeconds:0} s.")));
    }

    // ¿Hubo modo seguro mientras duraba un incidente de este tipo (tormenta, sombra...)?
    protected static bool SafeModeDuring(MissionResult r, IncidentType type, float defaultDuration)
    {
        foreach (var s in r.Incidents)
        {
            if (s.Type != type) continue;
            float end = s.Time + (s.Duration > 0 ? s.Duration : defaultDuration);
            foreach (var m in r.Incidents)
                if (m.Type == IncidentType.SafeMode && m.Time >= s.Time && m.Time <= end + 1f) return true;
        }
        return false;
    }
    public abstract void AddComparison(ReportData rep, RoverDesign d, MissionResult r);

    public SiteInfo Find(ScienceSite s)
    {
        foreach (var i in Sites) if (i.Site == s) return i;
        return null;
    }

    // Ayudas comunes para las subclases
    protected static string Times(int n) => n == 1 ? Loc.T("una vez", "once") : n + Loc.T(" veces", " times");
}

public static class MissionCatalog
{
    public static readonly MissionDefinition Mars = new MarsMission();
    public static readonly MissionDefinition Moon = new MoonMission();

    static readonly List<MissionDefinition> all = new List<MissionDefinition> { Mars, Moon };
    public static IReadOnlyList<MissionDefinition> All => all;

    public static MissionDefinition Get(string id)
    {
        foreach (var m in all) if (m.Id == id) return m;
        return Mars;
    }

    // Busca un punto en cualquier misión (cada ScienceSite pertenece a una sola).
    public static SiteInfo Site(ScienceSite s)
    {
        foreach (var m in all) { var i = m.Find(s); if (i != null) return i; }
        return null;
    }

    public static void Register(MissionDefinition m)
    {
        if (!all.Contains(m)) all.Add(m);
    }
}
