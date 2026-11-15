// Assets/_Project/Scripts/Model/MoonMission.cs
// C# puro: misión 2 de la historia, "El secreto de la Luna" (polo sur, hielo de agua).
// Referencias NASA: Lunar Reconnaissance Orbiter (LRO, instrumento Diviner), LCROSS y el rover VIPER.
// Diferencias con Marte (no hay eventos programados; todo depende de DÓNDE vas):
//   · Zonas en sombra permanente: los paneles no recargan y el frío gasta energía.
//   · Dentro del cráter no se ve la Tierra: sin relé vía orbitador, no hay señal (no se puede escanear).
using System.Collections.Generic;

public class MoonMission : MissionDefinition
{
    public override string Id => "moon";
    public override string DestinationName => Loc.T("la Luna", "the Moon");
    public override string PlaceName => Loc.T("Polo sur lunar", "Lunar south pole");
    public override string SceneName => "04_Moon";
    public override float SafeModeMinRecharge => 0.8f;

    static readonly List<SiteInfo> sites = new List<SiteInfo>
    {
        new SiteInfo(ScienceSite.RimRock, "la roca del borde iluminado", "the sunlit rim rock", "Borde iluminado", "Sunlit rim", Instrument.Camera),
        new SiteInfo(ScienceSite.Regolith, "el regolito con esferas de vidrio", "the regolith with glass beads", "Regolito", "Regolith", Instrument.Spectrometer),
        new SiteInfo(ScienceSite.Ice, "el hielo del cráter en sombra", "the ice in the shadowed crater", "Hielo", "Ice", Instrument.Drill),
    };
    public override List<SiteInfo> Sites => sites;

    public override string TravelLabel(float u)
    {
        int day = System.Math.Max(1, System.Math.Min(3, 1 + (int)(u * 3f)));
        return Loc.T($"Viaje Tierra → Luna · día {day} de unos 3", $"Earth → Moon trip · day {day} of about 3");
    }
    public override string ArrivalText => Loc.T("Llegando a la Luna: descenso al polo sur...", "Arriving at the Moon: descent to the south pole...");

    public override string ComparisonTitle => Loc.T("Tu rover vs. VIPER y LCROSS", "Your rover vs. VIPER and LCROSS");

    public override string VegaClosing(int stars) =>
        stars == 3 ? Loc.T("¡Lo lograste, comandante! Encontramos hielo en la sombra permanente. Ese agua podría ser vital para futuras bases en la Luna.", "You did it, commander! We found ice in the permanent shadow. That water could be vital for future bases on the Moon.")
      : stars == 2 ? Loc.T("Buen trabajo. Tenemos datos valiosos del polo sur, aunque nos faltó un punto. Revisa qué decisión lo causó y vuelve a intentarlo.", "Good work. We have valuable data from the south pole, although we missed one site. Check which decision caused it and try again.")
      : stars == 1 ? Loc.T("Conseguimos algo de ciencia, pero el polo sur es exigente: frío, oscuridad y sin señal en los cráteres. ¡Rediseña el rover!", "We got some science, but the south pole is tough: cold, dark and no signal in the craters. Redesign the rover!")
      : Loc.T("Esta vez no obtuvimos datos... La Luna parece cercana, pero sus cráteres en sombra son de los lugares más difíciles de explorar. Prueba otro diseño.", "No data this time... The Moon seems close, but its shadowed craters are among the hardest places to explore. Try another design.");

    // Energía: sombra permanente (sin Sol y con frío) y modo seguro
    public override void AddEnergyLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        int safeModes = r.CountOf(IncidentType.SafeMode);
        bool shadow = r.CountOf(IncidentType.Shadow) > 0;
        bool safeInShadow = SafeModeDuring(r, IncidentType.Shadow, 20f);

        if (d.UsesSolar && safeInShadow)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                Loc.T("Elegiste paneles solares: en la sombra permanente no llega el Sol y el frío vació tu batería, así que el rover quedó en modo seguro. Con un RTG habría seguido recargando.", "You chose solar panels: no sunlight reaches the permanent shadow and the cold drained your battery, so the rover went into safe mode. With an RTG it would have kept recharging.")));
        else if (d.UsesSolar && shadow)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                Loc.T("Entraste en la sombra con paneles solares y tu batería aguantó el frío hasta salir. ¡Bien calculado!", "You went into the shadow with solar panels and your battery survived the cold until you got out. Well planned!")));
        else if (d.Power == PowerSource.RTG && shadow)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                Loc.T("Tu RTG siguió dando energía en la oscuridad: en la sombra permanente es la mejor opción.", "Your RTG kept supplying power in the dark: in permanent shadow it's the best choice.")));

        if (safeModes > 0 && !(d.UsesSolar && safeInShadow))
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                Loc.T($"Te quedaste sin energía {Times(safeModes)}. ", $"You ran out of energy {Times(safeModes)}. ") +
                (d.BatteryCapacity < 100 ? Loc.T("Con una batería más grande habrías aguantado más tiempo en el frío.", "A bigger battery would have lasted longer in the cold.")
                                         : Loc.T("Moverse, escanear y el frío gastan energía: planifica la ruta.", "Driving, scanning and the cold use energy: plan your route."))));
    }

    // Comunicaciones: dentro del cráter la Tierra no se ve
    public override void AddCommsLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        bool lost = r.CountOf(IncidentType.CommsBlackout) > 0;
        bool relay = d.BlackoutSeconds <= 20f;   // el relé vía orbitador es la antena de apagón corto
        if (lost && !relay)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                Loc.T("Con la antena directa no había línea de vista con la Tierra dentro del cráter: desde ahí no podías enviar datos. Un relé vía orbitador mantiene la señal.", "With the direct antenna there was no line of sight to Earth inside the crater: you couldn't send data from there. An orbital relay keeps the signal.")));
        else if (relay)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                Loc.T("El relé vía orbitador mantuvo la señal dentro del cráter, donde la Tierra no se ve.", "The orbital relay kept the signal inside the crater, where Earth can't be seen.")));
    }

    // Terreno: campo de bloques (ruedas todoterreno)
    public override void AddTerrainLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        if (r.CountOf(IncidentType.RockyZoneBlocked) > 0)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                Loc.T("Con ruedas estándar no pudiste cruzar el campo de bloques y tuviste que rodearlo. Las ruedas todoterreno abren ese atajo.", "With standard wheels you couldn't cross the boulder field and had to go around it. All-terrain wheels open that shortcut.")));
        else if (d.AllTerrainWheels)
            rep.Lines.Add(new ReportLine(ReportTone.Info, Loc.T("Tus ruedas todoterreno te permitían cruzar el campo de bloques.", "Your all-terrain wheels let you cross the boulder field.")));
    }

    // Comparación con misiones NASA reales (verificar en docs/research)
    public override void AddComparison(ReportData rep, RoverDesign d, MissionResult r)
    {
        rep.Comparison.Add(new ReportLine(d.Power == PowerSource.RTG ? ReportTone.Good : ReportTone.Info,
            Loc.T("Frío: Diviner (LRO) midió unos -240 °C en cráteres en sombra permanente, de lo más frío del sistema solar.", "Cold: Diviner (LRO) measured about -240 °C in permanently shadowed craters, among the coldest places in the solar system.")));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            Loc.T("Hielo: en 2009, LCROSS impactó el cráter Cabeus y detectó agua en el material que levantó.", "Ice: in 2009, LCROSS struck Cabeus crater and detected water in the material it kicked up.")));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            Loc.T($"Instrumentos: llevaste {d.InstrumentCount} de 3. VIPER, el rover de la NASA para buscar hielo, lleva un taladro (TRIDENT) y espectrómetros.", $"Instruments: you carried {d.InstrumentCount} of 3. VIPER, NASA's ice-hunting rover, carries a drill (TRIDENT) and spectrometers.")));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            Loc.T("Señal: desde el fondo de un cráter polar la Tierra no se ve; hacen falta orbitadores de relé.", "Signal: Earth can't be seen from the floor of a polar crater; relay orbiters are needed.")));
    }
}
