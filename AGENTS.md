# AGENTS.md — Mars Crafts (NASA Space Apps Challenge 2026)

> Fuente de contexto para cualquier asistente de IA (Claude Code, Codex, ChatGPT, Antigravity, Cursor, Copilot…). Léelo completo antes de tocar nada.
> **Responde siempre en español.** Trabaja paso a paso, con cambios pequeños, y explica brevemente las decisiones de diseño.

## 1. Qué es

**Mars Crafts**: juego educativo en **Unity 2D**, top-down, pixel art, para el reto **Space Mission Design Game** del NASA Space Apps Challenge 2026 (evento local ESPOL).

- **Hackatón:** sábado 14 y domingo 15 de noviembre de 2026. **Entrega global:** domingo 23:59 (hora de Ecuador). Hay que confirmar si el evento local pide algo antes.
- **Repo:** https://github.com/Niot-Espol/Nasa_Challenge · Equipo Niot-Espol.
- **Danilo** es el integrador y Unity lead.

### Documentos (lee el que aplique antes de trabajar en esa parte)
- [`docs/game-design/alcance-mision-marte.md`](docs/game-design/alcance-mision-marte.md): **alcance acordado** (solo Marte), flujo, qué entra y qué se recorta, y cronograma.
- [`docs/game-design/sistema-recursos-y-assets.md`](docs/game-design/sistema-recursos-y-assets.md): recursos, valores de los componentes, diseños objetivo, eventos, puntuación y lista de assets.
- [`docs/game-design/lista-arte-para-dibujar.md`](docs/game-design/lista-arte-para-dibujar.md): **lista de arte para el equipo** (tamaños, frames, prioridades). Sin capas: el rover es un solo dibujo y los componentes son íconos sueltos.
- [`docs/architecture/arquitectura-unity.md`](docs/architecture/arquitectura-unity.md): escenas, `GameSession`, `ComponentData` y prefabs.
- [`docs/game-design/historia-mars-crafts.md`](docs/game-design/historia-mars-crafts.md): historia del equipo. La versión vigente es la **1.65**; la 1.0 está descartada. Las tres misiones son la visión, pero **solo se construye Marte**: la Tierra y la Luna aparecen bloqueadas en el menú.

### Resumen del juego
`00_Title` → `01_Hub` (centro espacial, briefing con la Dra. Vega y BIMO) → `02_Workshop` (constructor UI: componentes y lanzador con presupuesto, masa y energía; BIMO avisa qué no cumple) → `03_Launch` (animación) → [`035_Landing`, opcional] → `04_Mars` (rover top-down, 3 puntos científicos, energía, tiempo, tormenta de polvo, pérdida de comunicación) → `05_Report` (qué salió bien o mal y **qué decisión lo causó**, comparación con Perseverance, botón Rediseñar).

**Dilema central:** ciencia completa con energía frágil (paneles solares) frente a energía confiable con menos ciencia (RTG). El acoplamiento masa → lanzador → presupuesto obliga a elegir. Ninguna configuración es perfecta y fallar nunca es un callejón sin salida (modo seguro en lugar de game over).

## 2. Stack técnico

| Elemento | Valor |
|---|---|
| Motor | **Unity 6000.5.4f1**. Todos usan la misma versión (`ProjectSettings/ProjectVersion.txt`). |
| Render | URP 2D (`Renderer2D`). |
| Input | **Solo el Input System nuevo** (`Keyboard.current`, Input Actions). `UnityEngine.Input` lanza una excepción. |
| UI | UGUI + **TextMeshPro** (TMP Essentials importado). `EventSystem` con `InputSystemUIInputModule`. |
| Física | Physics 2D. Usa `rb.linearVelocity`, no `velocity`. |
| Pixel art | **16 px = 1 unidad** (tile de 16×16 = 1 unidad). Filtro Point, sin compresión. Personajes con el pivote en los pies. |
| IA | **MCP for Unity** (`com.coplaydev.unity-mcp`). Con Unity abierto, sirve en `http://127.0.0.1:8080/mcp`. En Claude Code: `claude mcp add --transport http unityMCP http://127.0.0.1:8080/mcp`. |

## 3. Arquitectura (resumen; detalle en `docs/architecture/`)
- Una escena por etapa. Las piezas que se repiten entre escenas se hacen como prefabs.
- `Scripts/Core/`: `GameSession` (estático, con `Design` y `Result`) y el flujo de escenas (`SceneFlow`).
- `Scripts/Model/`: C# puro, **sin `using UnityEngine`** (`RoverDesign`, `Verifier`, `MissionState`, `ReportBuilder`).
- `Scripts/Gameplay/`, `Scripts/UI/`, `Scripts/Data/` (ScriptableObjects como `ComponentData`).
- Toda escena que lee `GameSession` crea valores de depuración si vienen vacíos, para poder dar Play directamente en ella.
- No desactives el Domain Reload en *Enter Play Mode Options*: los estáticos no se reiniciarían.
- Movimiento: `TopDownMover`, que lo usan tanto el astronauta (Hub) como el rover (Mars).

### Escenas: sin dueños fijos
**Cualquier escena se puede modificar**, también la IA con el MCP. Para no pisarse, avisa al equipo antes de trabajar en una escena y haz commit y push pronto: dos personas editando la **misma** `.unity` a la vez generan conflictos de merge difíciles de resolver.

## 4. Estructura del repo
```text
Assets/_Project/
  Art/
    Sprites/Characters/<Personaje>/   Rotations/ (8 dir, idle) + Running/North|South|East|West/ (8 frames)
    Animations/Characters/            Astronaut.controller (base) + <Personaje>/ clips y override
  Audio/ Data/ Prefabs/ ScriptableObjects/ Settings/ Tests/
  Scenes/                             00_Title … 05_Report  (el doc pide Scenes/Main/ y Scenes/Sandbox/: pendiente de mover)
  Scripts/Core|Gameplay|UI|Editor/    (+ Model/ y Data/ cuando se creen)
  TextMesh Pro/                       (no modificar)
docs/game-design/ docs/architecture/ docs/research/
Elementos visuales/                   zips originales de los personajes (fuente; no los usa Unity)
```

## 5. Estado actual — checklist (actualízalo al avanzar)
- [x] Esqueleto: 6 escenas en Build Settings, en orden, cada una con Canvas, título TMP y botón "Siguiente" → `SceneFlow.Next()`. *(Falta moverlas a `Scenes/Main/`.)*
- [x] `04_Mars` en greybox: rover (cuadrado) con `TopDownMover` (`Rigidbody2D` con gravedad 0, rotación congelada, Interpolate) y 6 rocas con `BoxCollider2D`, fondo rojizo y botón en la esquina. Cámara: **Cinemachine 3.1.7**, con `CM Rover Camera` (`CinemachineCamera` + `CinemachinePositionComposer`, ortho size 6) siguiendo al rover y `CinemachineBrain` en la Main Camera. *(Pendiente: `CinemachineConfiner2D` cuando exista el mapa con Tilemap.)*
- [x] Sprites de 3 astronautas importados (`AstronautStocky`, `AstronautAgile`, `AstronautCute`; 32×32, 8 direcciones idle, carrera en 4 direcciones).
- [x] Animaciones de los astronautas: 6 clips por personaje (`Idle_Down/Up/Side`, `Run_Down/Up/Side`, 10 fps). `Astronaut.controller` usa blend trees 2D con los parámetros `FacingX`, `FacingY` e `IsMoving`, y hay override controllers para Agile y Cute. Para ir a la izquierda se usa el clip *Side* con `SpriteRenderer.flipX`.
- [x] **Protagonista: `AstronautStocky`.** Las otras dos variantes quedan disponibles cambiando el controller del Animator por su override.
- [x] Prefab `Assets/_Project/Prefabs/Player.prefab`: en la raíz tiene `Rigidbody2D`, un `BoxCollider2D` solo en los pies, `TopDownMover` y `CharacterAnimator`; el hijo `Sprite` tiene `SpriteRenderer` y `Animator`. `CharacterAnimator` pasa `Facing` e `IsMoving` al Animator, decide la dirección dominante y aplica `flipX` a la izquierda. Está colocado en `01_Hub` sobre un fondo greybox gris azulado.
- [x] `04_Mars` jugable (greybox):
  - `Mission` con `MissionController`: avanza el estado, bloquea al rover en modo seguro y dispara los eventos (tormenta en t=70 s durante 30 s; apagón en t=150 s con la duración de la antena).
  - 3 `ScienceSitePoint` (Delta azul, Mineral morado, Roca especial dorada) con trigger, indicador "[E]" y color al completarse.
  - HUD **provisional** de texto (`MissionHUD`) con energía, tiempo, puntos, estados y mensaje de BIMO.
  - Botón "Terminar misión" → `FinishMission()`: guarda `GameSession.Result` y carga `05_Report`. Al acabarse el tiempo también termina.
- [x] Contrato del modelo:
  - `Scripts/Model/` (C# puro): `RoverDesign`, `Verifier` (con `VerificationResult` y `VerificationIssue`, mensajes de BIMO) y `MissionResult` (`ScienceSite`, `MissionIncident`).
  - `Scripts/Core/GameSession.cs`, con `EnsureDesign()`, `EnsureResult()` y `Reset()`.
  - `Scripts/Data/`: `ComponentData` (ScriptableObject) y `RoverDesignBuilder`.
  - 14 assets en `ScriptableObjects/Components/`.
  - Verificado: los diseños objetivo del doc de recursos dan exactamente 470/99, 500/106, 455/108 y 480/97 (✓), y "Lo quiero todo" da 550/129 (✗).
  - **Pendiente de diseño:** definir el "consumo base" de energía (regla de verificación) y los `nasaFact` con fuente de cada componente.
- [x] `MissionState` (`Scripts/Model/`, C# puro): estado en vivo de Marte.
  - Energía: moverse gasta 1/s, escanear lo que cueste el instrumento, la fuente recarga; los paneles no recargan durante la tormenta.
  - Tiempo de 240 s y modo seguro (energía en 0 → detenido hasta el 30 %).
  - Escaneo con `ScanStatus` y mensajes de BIMO; va registrando incidentes en `Result`.
- [ ] `ReportBuilder` (textos del reporte a partir de `Design` + `Result`) y pantalla de Reporte que los muestre.
- [ ] Primera build de principio a fin con placeholders (meta: sábado ~20:00).

## 6. Reglas para agentes (importante)
1. **No edites `.unity`, `.prefab` ni `.asset` como texto.** Haz los cambios de escena, prefab o Inspector con el MCP de Unity, con un script de editor, o explícaselos a Danilo.
2. Se puede modificar cualquier escena. Antes de editar una, haz `git pull` para partir de lo último, y al terminar haz commit y push enseguida para evitar conflictos en los `.unity`.
3. **No toques `ProjectSettings/` ni `Packages/`** sin que te lo pidan explícitamente.
4. Assets nuevos o movidos siempre con su `.meta`. Mover o renombrar, solo desde Unity (o con `AssetDatabase.MoveAsset`).
5. Cambios pequeños y verificables. Después de cada paso, di cómo probarlo en Play.
6. `main` siempre debe abrir y correr en Play. Ramas cortas por persona; integrar cada 2–3 horas.
7. Datos científicos solo con fuente NASA. Los valores del juego son unidades de juego, no especificaciones reales.
8. No ignores ni borres `.meta`. No toques `Library/`, `Temp/`, `Logs/`, `UserSettings/` ni `*.csproj`.
9. Código en inglés y comentarios en español. Física en `FixedUpdate`, input en `Update`. Datos configurables como ScriptableObjects.
10. Actualiza la **sección 5** cuando completes o empieces algo.

## 7. Cómo abrir y probar
1. Instala Unity 6000.5.4f1 (Unity Hub) y clona el repo. La primera apertura regenera `Library/`.
2. Abre `Assets/_Project/Scenes/00_Title.unity` y dale Play. El botón de cada escena lleva a la siguiente.
3. En `04_Mars` el rover se mueve con WASD o las flechas.
