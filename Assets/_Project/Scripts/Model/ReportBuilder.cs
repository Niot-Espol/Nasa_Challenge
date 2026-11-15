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

        rep.Grade = r.Stars == 3 ? Loc.T("¡Misión cumplida!", "Mission accomplished!")
                  : r.Stars == 2 ? Loc.T("Buena misión", "Good mission")
                  : r.Stars == 1 ? Loc.T("Misión parcial", "Partial mission")
                  : Loc.T("La misión no obtuvo datos... pero aprendimos mucho", "No data this time... but we learned a lot");

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
                    Loc.T($"Estudiaste {info.Name} con {MissionState.InstrumentName(instr)}.", $"You studied {info.Name} with {MissionState.InstrumentName(instr)}.")));
            else if (!d.HasInstrument(instr))
                rep.Lines.Add(new ReportLine(ReportTone.Bad,
                    Loc.T($"No llevabas {MissionState.InstrumentName(instr)}: no pudiste estudiar {info.Name}.", $"You didn't bring {MissionState.InstrumentName(instr)}: you couldn't study {info.Name}.")));
            else
                rep.Lines.Add(new ReportLine(ReportTone.Bad,
                    Loc.T($"Llevabas {MissionState.InstrumentName(instr)}, pero no llegaste a estudiar {info.Name} a tiempo.", $"You had {MissionState.InstrumentName(instr)}, but didn't reach {info.Name} in time.")));
        }

        // --- Energía (propia de cada misión: tormenta, sombra...) ---
        mission.AddEnergyLines(rep, d, r);

        // --- Comunicaciones ---
        mission.AddCommsLines(rep, d, r);

        // --- Terreno (propio de cada misión) ---
        mission.AddTerrainLines(rep, d, r);

        // --- Presupuesto ---
        rep.Lines.Add(d.RemainingBudget >= 10
            ? new ReportLine(ReportTone.Good, Loc.T($"Gastaste {d.TotalCost} de {RoverDesign.Budget} créditos: te sobraron {d.RemainingBudget}.", $"You spent {d.TotalCost} of {RoverDesign.Budget} credits: {d.RemainingBudget} left over."))
            : new ReportLine(ReportTone.Info, Loc.T($"Gastaste {d.TotalCost} de {RoverDesign.Budget} créditos.", $"You spent {d.TotalCost} of {RoverDesign.Budget} credits.")));

        if (r.CountOf(IncidentType.TimeOut) > 0 && r.Stars < 3)
            rep.Lines.Add(new ReportLine(ReportTone.Info, Loc.T("Se acabó el tiempo de la misión antes de completar todos los puntos.", "Mission time ran out before completing all sites.")));

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
            case Badge.Commander: return Loc.T("Comandante Mars Crafts", "Mars Crafts Commander");
            case Badge.Efficient: return Loc.T("Eficiente", "Efficient");
            default: return Loc.T("Sin modo seguro", "No safe mode");
        }
    }

    public static MissionIncident Find(MissionResult r, IncidentType t)
    {
        foreach (var i in r.Incidents) if (i.Type == t) return i;
        return null;
    }
}
