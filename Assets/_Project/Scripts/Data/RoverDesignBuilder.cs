// Assets/_Project/Scripts/Data/RoverDesignBuilder.cs
// Puente entre los assets (ComponentData) y el modelo (RoverDesign).
// El Taller llama a Build() cada vez que el jugador cambia una selección.
using System.Collections.Generic;

public static class RoverDesignBuilder
{
    public static RoverDesign Build(IEnumerable<ComponentData> selected)
    {
        var d = new RoverDesign();
        foreach (var c in selected)
        {
            if (c == null) continue;
            d.ComponentIds.Add(c.id);
            d.TotalMass += c.mass;
            d.TotalCost += c.cost;

            switch (c.category)
            {
                case ComponentCategory.Launcher:
                    d.LauncherId = c.id;
                    d.LauncherCapacity = (int)c.capacity;
                    break;
                case ComponentCategory.Energy:
                    d.Power = c.powerSource;
                    d.RechargePerSecond = c.rechargePerSecond;
                    break;
                case ComponentCategory.Battery:
                    d.BatteryCapacity = c.capacity;
                    break;
                case ComponentCategory.Instrument:
                    d.ScanCosts[c.instrument] = c.scanCost;
                    break;
                case ComponentCategory.Comms:
                    d.CommsId = c.id;
                    d.BlackoutSeconds = c.blackoutSeconds;
                    break;
                case ComponentCategory.Wheels:
                    d.WheelsId = c.id;
                    d.AllTerrainWheels = c.crossesRockyZone;
                    break;
            }
        }
        return d;
    }
}
