// Assets/_Project/Scripts/Model/ReportBuilder.cs
// C# puro: convierte Design + Result en el contenido del Reporte.
// Lo importante: cada problema se explica con la DECISIÓN del Taller que lo causó.
// Lo común a todas las misiones está aquí; lo propio (eventos, terreno, comparación) en MissionDefinition.
using System.Collections.Generic;

public enum ReportTone { Good, Bad, Info }

public class ReportLine
{
    public ReportTone Tone;
    public string Text;
    public ReportLine(ReportTone tone, string text) { Tone = tone; Text = text; }
}

public enum Badge { Commander, Efficient, NoSafeMode }

public class ReportData
{
    public int Stars;                 // 0..3 (1 por punto científico)
    public string Grade;              // titular ("¡Misión cumplida!")
    public List<Badge> Badges = new List<Badge>();
    public List<ReportLine> Lines = new List<ReportLine>();          // qué pasó y por qué
    public List<ReportLine> Comparison = new List<ReportLine>();     // tu rover vs la misión real de referencia

    // Resumen de recursos
    public int CostUsed, Budget, MassUsed, MassCapacity;
    public float EnergyConsumed, TimeUsed, TimeLimit;
}

public static class ReportBuilder
{
    // mission = null → Marte (compatibilidad con el código anterior)
    public static ReportData Build(RoverDesign d, MissionResult r, MissionDefinition mission = null)
    {
        mission = mission ?? MissionCatalog.Mars;
        var rep = new ReportData
        {
            Stars = r.Stars,
            CostUsed = d.TotalCost, Budget = RoverDesign.Budget,
            MassUsed = d.TotalMass, MassCapacity = d.LauncherCapacity,
            EnergyConsumed = r.EnergyConsumed, TimeUsed = r.TimeUsedSeconds, TimeLimit = r.TimeLimitSeconds,
        };

        rep.Grade = r.Stars == 3 ? "¡Misión cumplida!"
                  : r.Stars == 2 ? "Buena misión"
                  : r.Stars == 1 ? "Misión parcial"
                  : "La misión no obtuvo datos... pero aprendimos mucho";

        // --- Insignias ---
        if (r.Stars == 3) rep.Badges.Add(Badge.Commander);
        if (d.RemainingBudget >= 10) rep.Badges.Add(Badge.Efficient);
        if (r.NeverEnteredSafeMode) rep.Badges.Add(Badge.NoSafeMode);

        // --- Puntos científicos: logrados y no logrados, con su causa ---
        foreach (var info in mission.Sites)
        {
            var site = info.Site;
            var instr = info.Instrument;
            if (r.CompletedSites.Contains(site))
                rep.Lines.Add(new ReportLine(ReportTone.Good,
                    $"Estudiaste {info.Name} con {MissionState.InstrumentName(instr)}."));
            else if (!d.HasInstrument(instr))
                rep.Lines.Add(new ReportLine(ReportTone.Bad,
                    $"No llevabas {MissionState.InstrumentName(instr)}: no pudiste estudiar {info.Name}."));
            else
                rep.Lines.Add(new ReportLine(ReportTone.Bad,
                    $"Llevabas {MissionState.InstrumentName(instr)}, pero no llegaste a estudiar {info.Name} a tiempo."));
        }

        // --- Energía (propia de cada misión: tormenta, sombra...) ---
        mission.AddEnergyLines(rep, d, r);

        // --- Comunicaciones ---
        mission.AddCommsLines(rep, d, r);

        // --- Terreno (propio de cada misión) ---
        mission.AddTerrainLines(rep, d, r);

        // --- Presupuesto ---
        rep.Lines.Add(d.RemainingBudget >= 10
            ? new ReportLine(ReportTone.Good, $"Gastaste {d.TotalCost} de {RoverDesign.Budget} créditos: te sobraron {d.RemainingBudget}.")
            : new ReportLine(ReportTone.Info, $"Gastaste {d.TotalCost} de {RoverDesign.Budget} créditos."));

        if (r.CountOf(IncidentType.TimeOut) > 0 && r.Stars < 3)
            rep.Lines.Add(new ReportLine(ReportTone.Info, "Se acabó el tiempo de la misión antes de completar todos los puntos."));

        // --- Comparación con la misión real de referencia ---
        mission.AddComparison(rep, d, r);

        return rep;
    }

    public static string SiteName(ScienceSite s)
    {
        var info = MissionCatalog.Site(s);
        return info != null ? info.Name : s.ToString();
    }

    public static string BadgeName(Badge b)
    {
        switch (b)
        {
            case Badge.Commander: return "Comandante Mars Crafts";
            case Badge.Efficient: return "Eficiente";
            default: return "Sin modo seguro";
        }
    }

    public static MissionIncident Find(MissionResult r, IncidentType t)
    {
        foreach (var i in r.Incidents) if (i.Type == t) return i;
        return null;
    }
}
