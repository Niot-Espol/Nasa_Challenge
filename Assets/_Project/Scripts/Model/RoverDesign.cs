// Assets/_Project/Scripts/Model/RoverDesign.cs
// C# puro (sin UnityEngine): el diseño que el jugador arma en el Taller.
// Lo escribe 02_Workshop, lo lee 04_Mars (y 05_Report para comparar).
using System.Collections.Generic;

public enum PowerSource { None, Solar, RTG }

public enum Instrument { Camera, Spectrometer, Drill }

public class RoverDesign
{
    // Reglas fijas del taller (unidades de juego).
    public const int Budget = 110;      // créditos disponibles
    public const int ChassisMass = 250; // chasis base, siempre incluido
    public const int ChassisCost = 15;

    // Ids de los ComponentData elegidos (para "Rediseñar": el Taller los vuelve a marcar).
    public List<string> ComponentIds = new List<string>();

    // Totales (ya incluyen el chasis).
    public int TotalMass = ChassisMass;
    public int TotalCost = ChassisCost;

    // Lanzador
    public string LauncherId;
    public int LauncherCapacity;        // masa máxima que puede llevar

    // Energía
    public PowerSource Power = PowerSource.None;
    public float RechargePerSecond;     // paneles: 0 durante la tormenta
    public float BatteryCapacity;       // energía máxima del rover

    // Instrumentos: cada uno habilita un punto científico y tiene su costo de escaneo.
    public Dictionary<Instrument, float> ScanCosts = new Dictionary<Instrument, float>();

    // Comunicaciones
    public string CommsId;
    public float BlackoutSeconds;       // duración del apagón de comunicación

    // Ruedas
    public string WheelsId;
    public bool AllTerrainWheels;       // cruza la zona rocosa

    public bool HasInstrument(Instrument i) => ScanCosts.ContainsKey(i);
    public int InstrumentCount => ScanCosts.Count;
    public int RemainingBudget => Budget - TotalCost;
    public bool UsesSolar => Power == PowerSource.Solar;

    // Diseño de prueba para dar Play directo en 04_Mars sin pasar por el Taller.
    // = "Ciencia completa, energía frágil": paneles + batería pequeña + 3 instrumentos
    //   + antena directa + lanzador ligero + ruedas estándar → 470 kg, 99 créditos.
    public static RoverDesign CreateDebugDefault()
    {
        var d = new RoverDesign
        {
            LauncherId = "launcher_light", LauncherCapacity = 500,
            Power = PowerSource.Solar, RechargePerSecond = 3f,
            BatteryCapacity = 60f,
            CommsId = "comms_direct", BlackoutSeconds = 40f,
            WheelsId = "wheels_standard", AllTerrainWheels = false,
            TotalMass = ChassisMass + 40 + 30 + 15 + 40 + 70 + 25 + 0,
            TotalCost = ChassisCost + 15 + 10 + 5 + 8 + 18 + 22 + 6 + 0,
        };
        d.ScanCosts[Instrument.Camera] = 10f;
        d.ScanCosts[Instrument.Spectrometer] = 25f;
        d.ScanCosts[Instrument.Drill] = 35f;
        d.ComponentIds.AddRange(new[] {
            "launcher_light", "power_solar", "battery_small",
            "instrument_camera", "instrument_spectrometer", "instrument_drill",
            "comms_direct", "wheels_standard" });
        return d;
    }
}
