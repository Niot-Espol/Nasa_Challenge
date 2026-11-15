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
    public override string DestinationName => "la Luna";
    public override string PlaceName => "Polo sur lunar";
    public override string SceneName => "04_Moon";
    public override float SafeModeMinRecharge => 0.8f;

    static readonly List<SiteInfo> sites = new List<SiteInfo>
    {
        new SiteInfo(ScienceSite.RimRock, "la roca del borde iluminado", "Borde iluminado", Instrument.Camera),
        new SiteInfo(ScienceSite.Regolith, "el regolito con esferas de vidrio", "Regolito", Instrument.Spectrometer),
        new SiteInfo(ScienceSite.Ice, "el hielo del cráter en sombra", "Hielo", Instrument.Drill),
    };
    public override List<SiteInfo> Sites => sites;

    public override string TravelLabel(float u)
    {
        int day = System.Math.Max(1, System.Math.Min(3, 1 + (int)(u * 3f)));
        return $"Viaje Tierra → Luna · día {day} de unos 3";
    }
    public override string ArrivalText => "Llegando a la Luna: descenso al polo sur...";

    public override string ComparisonTitle => "Tu rover vs. VIPER y LCROSS";

    public override string VegaClosing(int stars) =>
        stars == 3 ? "¡Lo lograste, comandante! Encontramos hielo en la sombra permanente. Ese agua podría ser vital para futuras bases en la Luna."
      : stars == 2 ? "Buen trabajo. Tenemos datos valiosos del polo sur, aunque nos faltó un punto. Revisa qué decisión lo causó y vuelve a intentarlo."
      : stars == 1 ? "Conseguimos algo de ciencia, pero el polo sur es exigente: frío, oscuridad y sin señal en los cráteres. ¡Rediseña el rover!"
      : "Esta vez no obtuvimos datos... La Luna parece cercana, pero sus cráteres en sombra son de los lugares más difíciles de explorar. Prueba otro diseño.";

    // Energía: sombra permanente (sin Sol y con frío) y modo seguro
    public override void AddEnergyLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        int safeModes = r.CountOf(IncidentType.SafeMode);
        bool shadow = r.CountOf(IncidentType.Shadow) > 0;
        bool safeInShadow = SafeModeDuring(r, IncidentType.Shadow, 20f);

        if (d.UsesSolar && safeInShadow)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                "Elegiste paneles solares: en la sombra permanente no llega el Sol y el frío vació tu batería, así que el rover quedó en modo seguro. Con un RTG habría seguido recargando."));
        else if (d.UsesSolar && shadow)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                "Entraste en la sombra con paneles solares y tu batería aguantó el frío hasta salir. ¡Bien calculado!"));
        else if (d.Power == PowerSource.RTG && shadow)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                "Tu RTG siguió dando energía en la oscuridad: en la sombra permanente es la mejor opción."));

        if (safeModes > 0 && !(d.UsesSolar && safeInShadow))
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                $"Te quedaste sin energía {Times(safeModes)}. " +
                (d.BatteryCapacity < 100 ? "Con una batería más grande habrías aguantado más tiempo en el frío."
                                         : "Moverse, escanear y el frío gastan energía: planifica la ruta.")));
    }

    // Comunicaciones: dentro del cráter la Tierra no se ve
    public override void AddCommsLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        bool lost = r.CountOf(IncidentType.CommsBlackout) > 0;
        bool relay = d.BlackoutSeconds <= 20f;   // el relé vía orbitador es la antena de apagón corto
        if (lost && !relay)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                "Con la antena directa no había línea de vista con la Tierra dentro del cráter: desde ahí no podías enviar datos. Un relé vía orbitador mantiene la señal."));
        else if (relay)
            rep.Lines.Add(new ReportLine(ReportTone.Good,
                "El relé vía orbitador mantuvo la señal dentro del cráter, donde la Tierra no se ve."));
    }

    // Terreno: campo de bloques (ruedas todoterreno)
    public override void AddTerrainLines(ReportData rep, RoverDesign d, MissionResult r)
    {
        if (r.CountOf(IncidentType.RockyZoneBlocked) > 0)
            rep.Lines.Add(new ReportLine(ReportTone.Bad,
                "Con ruedas estándar no pudiste cruzar el campo de bloques y tuviste que rodearlo. Las ruedas todoterreno abren ese atajo."));
        else if (d.AllTerrainWheels)
            rep.Lines.Add(new ReportLine(ReportTone.Info, "Tus ruedas todoterreno te permitían cruzar el campo de bloques."));
    }

    // Comparación con misiones NASA reales (verificar en docs/research)
    public override void AddComparison(ReportData rep, RoverDesign d, MissionResult r)
    {
        rep.Comparison.Add(new ReportLine(d.Power == PowerSource.RTG ? ReportTone.Good : ReportTone.Info,
            "Frío: Diviner (LRO) midió unos -240 °C en cráteres en sombra permanente, de lo más frío del sistema solar."));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            "Hielo: en 2009, LCROSS impactó el cráter Cabeus y detectó agua en el material que levantó."));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            $"Instrumentos: llevaste {d.InstrumentCount} de 3. VIPER, el rover de la NASA para buscar hielo, lleva un taladro (TRIDENT) y espectrómetros."));
        rep.Comparison.Add(new ReportLine(ReportTone.Info,
            "Señal: desde el fondo de un cráter polar la Tierra no se ve; hacen falta orbitadores de relé."));
    }
}
