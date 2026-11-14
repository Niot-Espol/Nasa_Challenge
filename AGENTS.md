# AGENTS.md — Guía para agentes de IA

> Este archivo es la fuente de contexto para cualquier asistente de IA (Claude Code, Codex, ChatGPT, Antigravity, Cursor, Copilot, etc.) que trabaje en este repositorio. Léelo completo antes de modificar nada.
> Idioma del proyecto: **español** (código en inglés, comentarios, commits y documentación en español).

---

## 1. Qué es este proyecto

**Nombre:** Diseña tu misión espacial
**Contexto:** proyecto para el **NASA Space Apps Challenge 2026**.
**Repositorio:** https://github.com/Niot-Espol/Nasa_Challenge
**Equipo:** Niot-Espol

### Objetivo

Un videojuego **2D hecho en Unity** donde el jugador **diseña, gestiona y simula una misión espacial completa a Marte**. Para hacerlo toma decisiones de ingeniería con **presupuesto, energía y recursos limitados**.

El juego es **educativo**: debe enseñar de forma jugable por qué las misiones reales hacen *trade-offs*, como masa frente a potencia o energía frente a distancia recorrida. Siempre que se pueda, las cifras y los datos deben basarse en fuentes reales de la NASA.

### Flujo del juego (una escena por etapa)

| Índice | Escena | Propósito (diseño previsto) |
|---|---|---|
| 0 | `00_Title` | Pantalla de título / inicio. |
| 1 | `01_Hub` | Centro de mando / base: punto de partida de la misión, briefing y objetivos. |
| 2 | `02_Workshop` | Taller: el jugador arma su nave o rover eligiendo componentes con presupuesto limitado. |
| 3 | `03_Launch` | Lanzamiento y viaje a Marte. |
| 4 | `04_Mars` | Exploración en Marte con un rover en vista cenital (top-down); el movimiento gasta energía. |
| 5 | `05_Report` | Reporte final: resultados de la misión. Al terminar, vuelve a `00_Title`. |

> ⚠️ Los propósitos de cada escena son la intención de diseño actual. El documento de diseño detallado irá en `docs/game-design/` (pendiente). Si algo aquí contradice ese documento, manda el documento.

---

## 2. Stack técnico

| Elemento | Valor |
|---|---|
| Motor | **Unity 6000.5.4f1** (Unity 6.5). No cambies de versión sin acordarlo con el equipo. |
| Render | **URP 2D** (Universal Render Pipeline con `Renderer2D`). |
| Input | **Input System nuevo únicamente** (`activeInputHandler: 1`). **No uses `UnityEngine.Input`** (el Input Manager antiguo): lanza una excepción. Usa `UnityEngine.InputSystem` (`Keyboard.current`, `Mouse.current`, Input Actions). |
| UI | **UGUI + TextMeshPro**. Los recursos esenciales de TMP están importados en `Assets/TextMesh Pro/`. Para textos nuevos usa `TextMeshProUGUI`, no `UnityEngine.UI.Text`. Los `EventSystem` deben usar `InputSystemUIInputModule`. |
| Física | **Physics 2D** (`Rigidbody2D`, `BoxCollider2D`…). En Unity 6 la velocidad es `rb.linearVelocity`; `rb.velocity` es obsoleto. |
| Paquetes destacados | 2D Animation, Aseprite Importer, PSD Importer, SpriteShape, Tilemap (+ Extras), Timeline, Test Framework, Visual Scripting, **MCP for Unity** (`com.coplaydev.unity-mcp`). |
| Lenguaje | C# |

### Integración con IA (MCP for Unity)

El proyecto incluye el paquete **MCP for Unity**. Con Unity abierto, este paquete expone un servidor MCP por HTTP, normalmente en `http://127.0.0.1:8080/mcp`. Los agentes compatibles con MCP pueden usarlo para crear objetos, editar escenas, leer la consola, entrar en Play, etc.

- Claude Code: `claude mcp add --transport http unityMCP http://127.0.0.1:8080/mcp`
- Otros clientes: configura un servidor MCP HTTP con esa URL. Comprueba el puerto en la ventana de MCP for Unity dentro del editor.

Si tu agente **no** tiene acceso al editor, edita solo archivos `.cs`. Pide al humano que haga en Unity los cambios de escena, o deja un script de editor (`[MenuItem]`) que los haga.

---

## 3. Estructura del repositorio

```text
Assets/
  _Project/                 ← TODO el contenido propio va aquí
    Art/                    Materials, Models, Sprites, Textures
    Audio/                  Music, SFX
    Data/                   Missions, Spacecraft, CelestialBodies (datos del juego)
    Prefabs/
    Scenes/                 00_Title … 05_Report
    Scripts/
      Core/                 Flujo del juego, managers globales (SceneFlow.cs)
      Gameplay/             Mecánicas jugables (movimiento del rover, interacción)
      Mission/              Lógica de la misión: objetivos, fases, resultados
      Simulation/           Simulación: energía, recursos, física de la misión
      Spacecraft/           Nave/rover: componentes, configuración
      Services/             Guardado, audio, servicios transversales
      UI/                   Controladores de interfaz
      Utilities/            Helpers genéricos
      Editor/               Scripts solo del editor (no entran en el build)
    ScriptableObjects/      Assets de datos (.asset) creados con ScriptableObjects
    Settings/               Configuración propia
    Tests/                  EditMode/ y PlayMode/
  TextMesh Pro/             Recursos esenciales de TMP (no modificar)
Packages/                   manifest.json y packages-lock.json
ProjectSettings/            Configuración de Unity (versionada)
docs/
  game-design/              Documento de diseño (GDD)
  architecture/             Decisiones técnicas
  research/                 Investigación y datos de la NASA
scripts/                    Automatización externa (CI, herramientas)
```

Las carpetas vacías tienen un `.gitkeep` para que Git las conserve.

---

## 4. Estado actual (noviembre 2026)

### Hecho
- Estructura de carpetas y configuración del proyecto.
- **6 escenas esqueleto** en Build Settings, en orden (`SampleScene` está desactivada, así que `00_Title` es el índice 0).
- En **cada escena**: un `Canvas` con un título (TextMeshPro) arriba, un `Button` para avanzar, un objeto `MoveScene` con el componente `SceneFlow` y un `EventSystem`.
- `Assets/_Project/Scripts/Core/SceneFlow.cs`: `Next()` carga la siguiente escena según el orden de Build Settings y, desde la última, vuelve a la 0.

### En curso — Paso 2: el rover en Marte (`04_Mars`)
Greybox, sin arte todavía:
- Fondo rojizo, un cuadrado como **Rover** y varios cuadrados como **Rocas**.
- Rover: `Rigidbody2D` (Gravity Scale = 0, Freeze Rotation Z, Interpolate = Interpolate) + `BoxCollider2D` + `TopDownMover`.
- Rocas: solo `BoxCollider2D`.
- El botón de avanzar se quita o se deja en una esquina para probar.
- Script a crear en `Assets/_Project/Scripts/Gameplay/TopDownMover.cs`: movimiento WASD/flechas con el Input System. La entrada se normaliza (en diagonal no va más rápido) y la velocidad se aplica en `FixedUpdate`. Expone `Facing` (para animación e interacción), `IsMoving` (para gastar energía) y `CanMove` (para bloquear el movimiento en diálogos o en modo seguro).

### Pendiente / ideas
- Sistema de **energía** del rover: `IsMoving` gasta batería.
- **Taller**: selección de componentes con presupuesto.
- **Reporte**: puntuación o resultados según las decisiones.
- GDD en `docs/game-design/`.
- Arte, audio y datos reales (NASA) en `Data/` y `ScriptableObjects/`.

---

## 5. Convenciones de código

- **Nombres** en inglés para clases, métodos y variables (`TopDownMover`, `SceneFlow`). **Comentarios en español.**
- Un `MonoBehaviour` por archivo; el nombre del archivo es igual al de la clase.
- Usa `[SerializeField] private` (o sin modificador) en lugar de campos `public` para configurar desde el Inspector.
- Usa `[RequireComponent]` cuando un script dependa de otro componente.
- Física en `FixedUpdate`, input en `Update`.
- Datos configurables (misiones, componentes, cuerpos celestes) → **ScriptableObjects** en `Data/` o `ScriptableObjects/`, no valores fijos en el código.
- Sistemas desacoplados: el gameplay expone propiedades o eventos (como `IsMoving`) y otros sistemas, como energía o UI, los consumen.
- Los scripts de editor van en una carpeta `Editor/`.
- No introduzcas dependencias o paquetes nuevos sin justificarlo.

---

## 6. Reglas para agentes de IA

1. **Nunca borres ni ignores archivos `.meta`.** Cada asset necesita su `.meta`; si mueves o renombras un asset, mueve también su `.meta` (o hazlo desde Unity).
2. **No toques** `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/` ni los `*.csproj`/`*.sln`: los genera Unity y están en `.gitignore`.
3. **Escenas (`.unity`) y prefabs** son YAML. Evita editarlos a mano: usa el editor, MCP for Unity o un script de editor. Si no queda más remedio, respeta los `fileID` y `guid`.
4. Todo el contenido nuevo va dentro de `Assets/_Project/`, en la subcarpeta que le corresponda.
5. Input **solo** con el Input System nuevo. Texto **solo** con TextMeshPro.
6. Después de crear o editar un script, comprueba que compila (consola de Unity) antes de usarlo en una escena.
7. Si cambias el orden o el número de escenas, actualiza también **Build Settings** y la tabla de la sección 1.
8. Commits pequeños, en español y descriptivos (por ejemplo, `Rover greybox en 04_Mars`). Trabaja en `main` salvo que el equipo indique otra cosa.
9. **Actualiza la sección 4 (Estado actual)** de este archivo cuando completes o empieces una tarea importante.
10. Ante dudas de diseño (qué hace una escena, reglas del juego), pregunta al humano antes de inventar mecánicas.

---

## 7. Cómo abrir y probar

1. Instala **Unity 6000.5.4f1** desde Unity Hub.
2. Clona el repo y ábrelo con Unity Hub (*Add project from disk*). La primera vez Unity regenera `Library/`, lo que puede tardar varios minutos.
3. Abre `Assets/_Project/Scenes/00_Title.unity` y pulsa **Play**. El botón de cada escena lleva a la siguiente.
4. Configuración recomendada (ya aplicada): *Version Control Mode* = **Visible Meta Files**, *Asset Serialization* = **Force Text**.
