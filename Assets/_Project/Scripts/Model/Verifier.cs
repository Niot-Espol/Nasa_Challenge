// Assets/_Project/Scripts/Model/Verifier.cs
// C# puro: revisa si un RoverDesign puede lanzarse. Lo usa el Taller antes del botón "Lanzar".
// Cada problema trae un mensaje listo para que BIMO lo diga.
using System.Collections.Generic;

public enum IssueSeverity { Error, Warning } // Error bloquea el lanzamiento; Warning solo avisa.

public class VerificationIssue
{
    public string Code;            // id estable, p. ej. "over_budget" (útil para UI y tests)
    public IssueSeverity Severity;
    public string Message;         // texto de BIMO

    public VerificationIssue(string code, IssueSeverity severity, string message)
    {
        Code = code; Severity = severity; Message = message;
    }
}

public class VerificationResult
{
    public List<VerificationIssue> Issues = new List<VerificationIssue>();

    public bool CanLaunch
    {
        get
        {
            foreach (var i in Issues) if (i.Severity == IssueSeverity.Error) return false;
            return true;
        }
    }
}

public static class Verifier
{
    public static VerificationResult Verify(RoverDesign d)
    {
        var r = new VerificationResult();

        // Falta elegir algo obligatorio
        if (string.IsNullOrEmpty(d.LauncherId))
            r.Issues.Add(new VerificationIssue("no_launcher", IssueSeverity.Error,
                "¡Comandante, no tenemos cohete! Elige un lanzador."));
        if (d.Power == PowerSource.None)
            r.Issues.Add(new VerificationIssue("no_power", IssueSeverity.Error,
                "El rover no tiene fuente de energía. Elige paneles solares o un RTG."));
        if (d.BatteryCapacity <= 0f)
            r.Issues.Add(new VerificationIssue("no_battery", IssueSeverity.Error,
                "Necesitamos una batería para guardar energía."));
        if (string.IsNullOrEmpty(d.CommsId))
            r.Issues.Add(new VerificationIssue("no_comms", IssueSeverity.Error,
                "Sin antena no podremos enviar los datos a la Tierra."));
        if (string.IsNullOrEmpty(d.WheelsId))
            r.Issues.Add(new VerificationIssue("no_wheels", IssueSeverity.Error,
                "Un rover sin ruedas... no es un rover. Elige unas ruedas."));

        // Límites duros
        if (d.TotalCost > RoverDesign.Budget)
            r.Issues.Add(new VerificationIssue("over_budget", IssueSeverity.Error,
                $"Nos pasamos del presupuesto: {d.TotalCost} de {RoverDesign.Budget} créditos. " +
                "Prueba con componentes más baratos o un lanzador más pequeño."));
        if (!string.IsNullOrEmpty(d.LauncherId) && d.TotalMass > d.LauncherCapacity)
            r.Issues.Add(new VerificationIssue("over_mass", IssueSeverity.Error,
                $"El rover pesa {d.TotalMass} kg y el lanzador solo lleva {d.LauncherCapacity} kg. " +
                "Quita peso o elige un lanzador más grande."));

        // Avisos (se puede lanzar igual)
        if (d.InstrumentCount == 0)
            r.Issues.Add(new VerificationIssue("no_instruments", IssueSeverity.Warning,
                "Sin instrumentos no podremos estudiar ningún punto científico."));
        else if (d.InstrumentCount < 3)
            r.Issues.Add(new VerificationIssue("missing_instruments", IssueSeverity.Warning,
                $"Llevamos {d.InstrumentCount} de 3 instrumentos: habrá puntos científicos que no podremos completar."));

        // TODO (diseño): regla "energía generada ≥ consumo base" cuando se defina el consumo base.

        return r;
    }
}
