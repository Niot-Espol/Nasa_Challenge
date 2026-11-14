// Assets/_Project/Scripts/Data/ComponentData.cs
// Un asset por componente del Taller (Create > MarsCrafts > Component),
// guardados en Assets/_Project/ScriptableObjects/Components/.
using UnityEngine;

public enum ComponentCategory { Launcher, Energy, Battery, Instrument, Comms, Wheels }

[CreateAssetMenu(menuName = "MarsCrafts/Component", fileName = "NewComponent")]
public class ComponentData : ScriptableObject
{
    public string id;                   // estable, p. ej. "instrument_camera"
    public string displayName;
    public ComponentCategory category;
    public int mass;
    public int cost;

    [Header("Stats (solo se usan los de su categoría)")]
    public float rechargePerSecond;     // Energy
    public PowerSource powerSource;     // Energy
    public float capacity;              // Battery: energía máxima / Launcher: masa máxima
    public Instrument instrument;       // Instrument
    public float scanCost;              // Instrument
    public float blackoutSeconds;       // Comms
    public bool crossesRockyZone;       // Wheels

    [Header("Presentación")]
    public Sprite icon;
    [TextArea] public string description;
    [TextArea] public string nasaFact;  // dato real breve, SIEMPRE con fuente NASA
    public string nasaSourceUrl;
}
