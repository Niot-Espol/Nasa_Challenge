// Assets/_Project/Scripts/Model/MissionDefinition.cs
// C# puro: todo lo que cambia de una misión a otra (Marte, Luna...).
// El resto del juego (Taller, rover, HUD, reporte) es común y lee la misión activa en GameSession.Mission.
// Para añadir una misión: crear una subclase, registrarla en MissionCatalog y hacer su escena de exploración.
using System.Collections.Generic;

// Un punto científico de una misión: qué es y qué instrumento exige.
public class SiteInfo
{
    public ScienceSite Site;
    public string Name;          // en frases: "el delta del antiguo río"
    public string ShortName;     // flechas del mapa: "Delta"
    public Instrument Instrument;

    public SiteInfo(ScienceSite site, string name, string shortName, Instrument instrument)
    {
        Site = site; Name = name; ShortName = shortName; Instrument = instrument;
    }
}

public abstract class MissionDefinition
{
    public abstract string Id { get; }               // "mars", "moon"
    public abstract string DestinationName { get; }  // "Marte"
    public abstract string PlaceName { get; }        // "Cráter Jezero"
    public abstract string SceneName { get; }        // escena de exploración
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
    public abstract void AddComparison(ReportData rep, RoverDesign d, MissionResult r);

    public SiteInfo Find(ScienceSite s)
    {
        foreach (var i in Sites) if (i.Site == s) return i;
        return null;
    }

    // Ayudas comunes para las subclases
    protected static string Times(int n) => n == 1 ? "una vez" : n + " veces";
}

public static class MissionCatalog
{
    public static readonly MissionDefinition Mars = new MarsMission();

    static readonly List<MissionDefinition> all = new List<MissionDefinition> { Mars };
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
