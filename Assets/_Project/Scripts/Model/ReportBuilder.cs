// Assets/_Project/Scripts/Model/ReportBuilder.cs
// C# puro: convierte Design + Result en el contenido del Reporte.
// Lo importante: cada problema se explica con la DECISIÓN del Taller que lo causó.
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
    public List<ReportLine> Comparison = new List<ReportLine>();     // tu rover vs Perseverance

    // Resumen de recursos
    public int CostUsed, Budget, MassUsed, MassCapacity;
    public float EnergyConsumed, TimeUsed, TimeLimit;
}

public static class ReportBuilder
{
    public static ReportData Build(RoverDesign d, MissionResult r)
    {
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
        foreach (ScienceSite site in new[] { ScienceSite.Delta, ScienceSite.MineralOutcrop, ScienceSite.SpecialRock })
        {
            var instr = MissionState.RequiredInstrument(site);
            if (r.CompletedSites.Contains(site))
                rep.Lines.Add(new ReportLine(ReportTone.Good,
                    $"Estudiaste {SiteName(site)} con {MissionState.InstrumentName(instr)}."));
            else if (!d.HasInstrument(instr))
                rep.Lines.Add(new ReportLine(ReportTone.Bad,
                    $"No llevabas {MissionState.InstrumentName(instr)}: no pudiste estudiar {SiteName(site)}."));
            else
                rep.Lines.Add(new ReportLine(ReportTone.Bad,
                    $"Llevabas {MissionState.InstrumentName(instr)}, pero no llegaste a estudiar {SiteName(site)} a tiempo."));
        }

        // --- Energía: tormenta y modo seguro ---
        int safeModes = r.CountOf(IncidentType.SafeMode);
        bool storm = r.CountOf(IncidentType.DustStorm) > 0;
        bool safeDuringStorm = SafeModeDuringStorm(r);

        if (d.UsesSolar && safeDuringStorm)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                "Elegiste paneles solares: la tormenta de polvo cortó la recarga y el rover entró en modo seguro."));
        else if (d.UsesSolar && storm)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                "La tormenta apagó tus paneles solares, pero tu batería aguantó hasta que pasó."));
        else if (d.Power == PowerSource.RTG && storm)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                "Tu RTG siguió dando energía durante la tormenta de polvo."));

        if (safeModes > 0 && !(d.UsesSolar && safeDuringStorm))
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                $"Te quedaste sin energía {Times(safeModes)}. " +
                (d.BatteryCapacity < 100 ? "Con una batería más grande habrías tenido más margen."
                                         : "Moverse y escanear gastan energía: planifica la ruta.")));

        // --- Comunicaciones ---
        var blackout = Find(r, IncidentType.CommsBlackout);
        if (blackout != null)
            rep.Lines.Add(d.BlackoutSeconds > 20f
                ? new ReportLine(ReportTone.Bad,
                    $"Con la antena directa perdiste la comunicación {d.BlackoutSeconds:0} s y no pudiste escanear. Un relé vía orbitador lo reduce a 10 s.")
                : new ReportLine(ReportTone.Good,
                    $"El relé vía orbitador redujo la pérdida de comunicación a solo {d.BlackoutSeconds:0} s."));

        // --- Presupuesto ---
        rep.Lines.Add(d.RemainingBudget >= 10
            ? new ReportLine(ReportTone.Good, $"Gastaste {d.TotalCost} de {RoverDesign.Budget} créditos: te sobraron {d.RemainingBudget}.")
            : new ReportLine(ReportTone.Info, $"Gastaste {d.TotalCost} de {RoverDesign.Budget} créditos."));

        if (r.CountOf(IncidentType.TimeOut) > 0 && r.Stars < 3)
            rep.Lines.Add(new ReportLine(ReportTone.Info, "Se acabó el tiempo de la misión antes de completar todos los puntos."));

        // --- Comparación con Perseverance (datos NASA: verificar en docs/research) ---
        rep.Comparison.Add(new ReportLine(d.Power == PowerSource.RTG ? ReportTone.Good : ReportTone.Info,
            d.Power == PowerSource.RTG
                ? "Energía: igual que Perseverance, usaste un generador de radioisótopos (Perseverance lleva un MMRTG)."
                : "Energía: tú usaste paneles solares, como Spirit y Opportunity. Perseverance usa un MMRTG para no depender del Sol."));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            $"Masa: tu rover pesó {d.TotalMass} kg (unidades del juego). Perseverance pesa unos 1.025 kg."));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            $"Instrumentos: llevaste {d.InstrumentCount} de 3. Perseverance lleva 7, entre ellos Mastcam-Z, SuperCam, PIXL y SHERLOC, y un taladro para sacar muestras."));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            "Comunicaciones: la mayoría de los datos de Perseverance llegan a la Tierra a través de orbitadores de relé."));

        return rep;
    }

    public static string SiteName(ScienceSite s)
    {
        switch (s)
        {
            case ScienceSite.Delta: return "el delta del antiguo río";
            case ScienceSite.MineralOutcrop: return "el afloramiento mineral";
            default: return "la roca especial";
        }
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

    static string Times(int n) => n == 1 ? "una vez" : n + " veces";

    static MissionIncident Find(MissionResult r, IncidentType t)
    {
        foreach (var i in r.Incidents) if (i.Type == t) return i;
        return null;
    }

    static bool SafeModeDuringStorm(MissionResult r)
    {
        foreach (var s in r.Incidents)
        {
            if (s.Type != IncidentType.DustStorm) continue;
            float end = s.Time + (s.Duration > 0 ? s.Duration : 30f);
            foreach (var m in r.Incidents)
                if (m.Type == IncidentType.SafeMode && m.Time >= s.Time && m.Time <= end + 1f) return true;
        }
        return false;
    }
}
