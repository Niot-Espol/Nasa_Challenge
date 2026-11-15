# Arquitectura en Unity — Mars Crafts

## Escenas (una por etapa, en Build Settings en este orden)
| Escena | Contenido |
|---|---|
| `00_Title` | Título y menú de misiones (Tierra/Luna bloqueadas) |
| `01_Hub` | Centro espacial, briefing con la Dra. Vega y BIMO |
| `02_Workshop` | Constructor (UI), verificación, lanzador |
| `03_Launch` | Animación de lanzamiento y viaje |
| `04_Mars` | Exploración, rover, puntos, eventos, HUD |
| `05_Report` | Reporte, Rediseñar y Terminar |
| (`035_Landing`) | Opcional |

Las escenas no tienen dueño fijo: cualquiera del equipo (o la IA con el MCP) puede modificarlas. Para evitar conflictos de merge en los `.unity`, avisa antes de editar una escena, haz `git pull` antes de empezar y commit + push al terminar. Las escenas Sandbox siguen para experimentar.

## Estado compartido
```csharp
// Scripts/Core/GameSession.cs
public static class GameSession
{
    public static RoverDesign Design;    // lo escribe el Taller, lo lee Marte
    public static MissionResult Result;  // lo escribe Marte, lo lee el Reporte
}
```
- Las clases estáticas sobreviven a los cambios de escena.
- Cada escena que lee la sesión crea valores de depuración si vienen vacíos (`RoverDesign.CreateDebugDefault()`), para poder dar Play directamente.
- Rediseñar: Report → Workshop, que preselecciona `GameSession.Design`.
- No desactivar el Domain Reload en Enter Play Mode Options (los estáticos no se reiniciarían).

## Flujo de escenas
Esqueleto inicial: `SceneFlow.Next()` carga `buildIndex + 1` (vuelve a 0 al final). Más adelante se agregan métodos específicos (por ejemplo, Rediseñar → Workshop).

## Datos
```csharp
// Scripts/Data/ComponentData.cs
using UnityEngine;

public enum ComponentCategory { Energy, Battery, Instrument, Comms, Wheels, Launcher }

[CreateAssetMenu(menuName = "MarsCrafts/Component")]
public class ComponentData : ScriptableObject
{
    public string id;
    public string displayName;
    public ComponentCategory category;
    public int mass;
    public int cost;

    [Header("Stats (solo se usan los de su categoría)")]
    public float rechargePerSecond;   // Energy
    public float capacity;            // Battery: energía máxima / Launcher: masa máxima
    public float scanCost;            // Instrument
    public float blackoutSeconds;     // Comms

    [Header("Presentación")]
    public Sprite icon;
    public Sprite previewLayer;
    [TextArea] public string description;
    [TextArea] public string nasaFact;
}
```
- Un asset por componente en `ScriptableObjects/Components/`.
- El modelo (`Scripts/Model/`, sin UnityEngine) trabaja con números. El Taller convierte los assets elegidos en un `RoverDesign`.

## Movimiento
`Scripts/Gameplay/TopDownMover.cs`: Rigidbody2D (gravedad 0, Freeze Rotation Z, Interpolate). Lee el teclado en `Update` con `Keyboard.current` y aplica `rb.linearVelocity` en `FixedUpdate`. Expone `Facing`, `IsMoving` y `CanMove` para animación, consumo de energía y bloqueo durante diálogos, el apagón o el modo seguro. Lo usan el astronauta (Hub) y el rover (Mars).

## Prefabs compartidos
- `DialogueBox`, `FactPopup`, barras de recursos, `SceneFader` (los puede hacer cualquiera).

## Carpetas
- `Scenes/Main/` y `Scenes/Sandbox/<Nombre>.unity`.
- `Scripts/Model/`, `Scripts/Core/`, `Scripts/UI/`, `Scripts/Gameplay/`, `Scripts/Data/`.
