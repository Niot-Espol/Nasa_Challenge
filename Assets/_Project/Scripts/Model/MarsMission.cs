// Assets/_Project/Scripts/Model/MarsMission.cs
// C# puro: misión 3 de la historia, "Marte: Cráter Jezero" (inspirada en Perseverance).
// Eventos propios: tormenta de polvo (paneles sin recarga) y zona rocosa (ruedas todoterreno).
using System.Collections.Generic;

public class MarsMission : MissionDefinition
{
    public override string Id => "mars";
    public override string DestinationName => "Marte";
    public override string PlaceName => "Cráter Jezero";
    public override string SceneName => "04_Mars";

    static readonly List<SiteInfo> sites = new List<SiteInfo>
    {
        new SiteInfo(ScienceSite.Delta, "el delta del antiguo río", "Delta", Instrument.Camera),
        new SiteInfo(ScienceSite.MineralOutcrop, "el afloramiento mineral", "Mineral", Instrument.Spectrometer),
        new SiteInfo(ScienceSite.SpecialRock, "la roca especial", "Roca especial", Instrument.Drill),
    };
    public override List<SiteInfo> Sites => sites;

    public override string TravelLabel(float u)
    {
        int month = System.Math.Max(1, System.Math.Min(7, 1 + (int)(u * 7f)));
        return $"Viaje Tierra → Marte · mes {month} de unos 7";
    }
    public override string ArrivalText => "Llegando a Marte: entrada, descenso y aterrizaje...";

    public override string ComparisonTitle => "Tu rover vs. Perseverance";

    public override string VegaClosing(int stars) =>
        stars == 3 ? "¡Excelente trabajo, comandante! Con estos datos podemos reconstruir parte de la historia del agua en Jezero. Así se diseña una misión."
      : stars == 2 ? "Buen trabajo. Obtuvimos datos valiosos, aunque nos faltó un punto. Revisa qué decisión lo causó y vuelve a intentarlo."
      : stars == 1 ? "Conseguimos algo de ciencia, pero la misión se quedó corta. Cada problema tiene una causa en el diseño: ¡rediséñalo!"
      : "Esta vez no obtuvimos datos... pero en las misiones reales también se aprende de los fallos. Revisa el reporte y prueba otro diseño.";

    // Energía: tormenta de polvo y modo seguro
    public override void AddEnergyLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        int safeModes = r.CountOf(IncidentType.SafeMode);
        bool storm = r.CountOf(IncidentType.DustStorm) > 0;
        bool safeDuringStorm = SafeModeDuring(r, IncidentType.DustStorm, 30f);

        if (d.UsesSolar && safeDuringStorm)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                "Elegiste paneles solares: la tormenta de polvo tapó el Sol, cortó la recarga y el rover quedó detenido en modo seguro. Con un RTG habría seguido recargando."));
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
    }

    // Terreno: zona rocosa
    public override void AddTerrainLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        if (r.CountOf(IncidentType.RockyZoneBlocked) > 0)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                "Con ruedas estándar no pudiste cruzar la zona rocosa y tuviste que rodearla. Las ruedas todoterreno abren ese atajo."));
        else if (d.AllTerrainWheels)
            rep.Lines.Add(new ReportLine(ReportTone.Info, "Tus ruedas todoterreno te permitían cruzar la zona rocosa."));
    }

    // Comparación con Perseverance (datos NASA: verificar en docs/research)
    public override void AddComparison(ReportData rep, RoverDesign d, MissionResult r)
    {
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
    }
}
