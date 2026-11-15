// Assets/_Project/Scripts/Model/MarsMission.cs
// C# puro: misión 3 de la historia, "Marte: Cráter Jezero" (inspirada en Perseverance).
// Eventos propios: tormenta de polvo (paneles sin recarga) y zona rocosa (ruedas todoterreno).
using System.Collections.Generic;

public class MarsMission : MissionDefinition
{
    public override string Id => "mars";
    public override string DestinationName => Loc.T("Marte", "Mars");
    public override string PlaceName => Loc.T("Cráter Jezero", "Jezero Crater");
    public override string SceneName => "04_Mars";

    static readonly List<SiteInfo> sites = new List<SiteInfo>
    {
        new SiteInfo(ScienceSite.Delta, "el delta del antiguo río", "the ancient river delta", "Delta", "Delta", Instrument.Camera),
        new SiteInfo(ScienceSite.MineralOutcrop, "el afloramiento mineral", "the mineral outcrop", "Mineral", "Mineral", Instrument.Spectrometer),
        new SiteInfo(ScienceSite.SpecialRock, "la roca especial", "the special rock", "Roca especial", "Special rock", Instrument.Drill),
    };
    public override List<SiteInfo> Sites => sites;

    public override string TravelLabel(float u)
    {
        int month = System.Math.Max(1, System.Math.Min(7, 1 + (int)(u * 7f)));
        return Loc.T($"Viaje Tierra → Marte · mes {month} de unos 7", $"Earth → Mars trip · month {month} of about 7");
    }
    public override string ArrivalText => Loc.T("Llegando a Marte: entrada, descenso y aterrizaje...", "Arriving at Mars: entry, descent and landing...");

    public override string ComparisonTitle => Loc.T("Tu rover vs. Perseverance", "Your rover vs. Perseverance");

    public override string VegaClosing(int stars) =>
        stars == 3 ? Loc.T("¡Excelente trabajo, comandante! Con estos datos podemos reconstruir parte de la historia del agua en Jezero. Así se diseña una misión.", "Excellent work, commander! With this data we can rebuild part of the history of water in Jezero. That's how you design a mission.")
      : stars == 2 ? Loc.T("Buen trabajo. Obtuvimos datos valiosos, aunque nos faltó un punto. Revisa qué decisión lo causó y vuelve a intentarlo.", "Good work. We got valuable data, although we missed one site. Check which decision caused it and try again.")
      : stars == 1 ? Loc.T("Conseguimos algo de ciencia, pero la misión se quedó corta. Cada problema tiene una causa en el diseño: ¡rediséñalo!", "We got some science, but the mission fell short. Every problem has a cause in the design: redesign it!")
      : Loc.T("Esta vez no obtuvimos datos... pero en las misiones reales también se aprende de los fallos. Revisa el reporte y prueba otro diseño.", "No data this time... but real missions also learn from failures. Check the report and try another design.");

    // Energía: tormenta de polvo y modo seguro
    public override void AddEnergyLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        int safeModes = r.CountOf(IncidentType.SafeMode);
        bool storm = r.CountOf(IncidentType.DustStorm) > 0;
        bool safeDuringStorm = SafeModeDuring(r, IncidentType.DustStorm, 30f);

        if (d.UsesSolar && safeDuringStorm)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                Loc.T("Elegiste paneles solares: la tormenta de polvo tapó el Sol, cortó la recarga y el rover quedó detenido en modo seguro. Con un RTG habría seguido recargando.", "You chose solar panels: the dust storm blocked the Sun, cut the recharge and left the rover stuck in safe mode. With an RTG it would have kept recharging.")));
        else if (d.UsesSolar && storm)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                Loc.T("La tormenta apagó tus paneles solares, pero tu batería aguantó hasta que pasó.", "The storm shut down your solar panels, but your battery lasted until it passed.")));
        else if (d.Power == PowerSource.RTG && storm)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                Loc.T("Tu RTG siguió dando energía durante la tormenta de polvo.", "Your RTG kept supplying power during the dust storm.")));

        if (safeModes > 0 && !(d.UsesSolar && safeDuringStorm))
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                Loc.T($"Te quedaste sin energía {Times(safeModes)}. ", $"You ran out of energy {Times(safeModes)}. ") +
                (d.BatteryCapacity < 100 ? Loc.T("Con una batería más grande habrías tenido más margen.", "A bigger battery would have given you more margin.")
                                         : Loc.T("Moverse y escanear gastan energía: planifica la ruta.", "Driving and scanning use energy: plan your route."))));
    }

    // Terreno: zona rocosa
    public override void AddTerrainLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        if (r.CountOf(IncidentType.RockyZoneBlocked) > 0)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                Loc.T("Con ruedas estándar no pudiste cruzar la zona rocosa y tuviste que rodearla. Las ruedas todoterreno abren ese atajo.", "With standard wheels you couldn't cross the rocky zone and had to go around it. All-terrain wheels open that shortcut.")));
        else if (d.AllTerrainWheels)
            rep.Lines.Add(new ReportLine(ReportTone.Info, Loc.T("Tus ruedas todoterreno te permitían cruzar la zona rocosa.", "Your all-terrain wheels let you cross the rocky zone.")));
    }

    // Comparación con Perseverance (datos NASA: verificar en docs/research)
    public override void AddComparison(ReportData rep, RoverDesign d, MissionResult r)
    {
        rep.Comparison.Add(new ReportLine(d.Power == PowerSource.RTG ? ReportTone.Good : ReportTone.Info,
            d.Power == PowerSource.RTG
                ? Loc.T("Energía: igual que Perseverance, usaste un generador de radioisótopos (Perseverance lleva un MMRTG).", "Power: just like Perseverance, you used a radioisotope generator (Perseverance carries an MMRTG).")
                : Loc.T("Energía: tú usaste paneles solares, como Spirit y Opportunity. Perseverance usa un MMRTG para no depender del Sol.", "Power: you used solar panels, like Spirit and Opportunity. Perseverance uses an MMRTG so it doesn't depend on the Sun.")));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            Loc.T($"Masa: tu rover pesó {d.TotalMass} kg (unidades del juego). Perseverance pesa unos 1.025 kg.", $"Mass: your rover weighed {d.TotalMass} kg (game units). Perseverance weighs about 1,025 kg.")));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            Loc.T($"Instrumentos: llevaste {d.InstrumentCount} de 3. Perseverance lleva 7, entre ellos Mastcam-Z, SuperCam, PIXL y SHERLOC, y un taladro para sacar muestras.", $"Instruments: you carried {d.InstrumentCount} of 3. Perseverance carries 7, including Mastcam-Z, SuperCam, PIXL and SHERLOC, plus a drill to collect samples.")));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            Loc.T("Comunicaciones: la mayoría de los datos de Perseverance llegan a la Tierra a través de orbitadores de relé.", "Communications: most of Perseverance's data reaches Earth through relay orbiters.")));
    }
}
