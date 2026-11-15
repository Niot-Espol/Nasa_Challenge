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
- [`docs/architecture/como-reemplazar-arte-y-audio.md`](docs/architecture/como-reemplazar-arte-y-audio.md): **dónde se conecta cada sprite, sonido y texto** cuando llegue el arte.
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
| Pixel art | **32 px = 1 unidad** para todo (tiles de 32×32 = 1 celda, personajes 32×32, objetos a su tamaño nativo); el rover a 64 px = 1 unidad. Filtro Point, sin compresión. `PixelArtImporter` lo aplica según la carpeta. Orden por Y: `YSortCamera` en la cámara (`CustomAxis` (0,1,0)) y los sprites del mundo con *Sprite Sort Point = Pivot* (pivote en los pies/base). Objetos nuevos: menú *Mars Crafts › Orden por profundidad › Aplicar en la escena abierta*. |
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
  Scripts/Core|Model|Data|Gameplay|UI|Editor/
  Resources/                          UITheme.asset y AudioLibrary.asset (arte y audio centralizados)
  Data/Dialogues/                     diálogos editables
  TextMesh Pro/                       (no modificar)
docs/game-design/ docs/architecture/ docs/research/
Elementos visuales/                   zips originales de los personajes (fuente; no los usa Unity)
```

## 5. Estado actual — checklist (actualízalo al avanzar)
**El juego se puede jugar de principio a fin con placeholders.** Flujo: Título → Hub → Taller → Lanzamiento → Marte → Reporte → (Rediseñar → Taller | Terminar → Título).

### Hecho
- **Pulido de UI (noche del 14 al 15 de nov):** arte de UI del equipo integrado (marcos 9-slice, botones, barras, estrellas, insignias, título); fuentes **Nunito** (textos) y **Pixelify Sans** (títulos) en `Art/Fonts` (OFL); carteles contextuales al acercarse (`InteractionTooltip`) en lugar de nombres flotantes; reporte rediseñado (barras, rover, comentario final de Vega); diálogos con letra más grande y retrato que no se encoge; nombre de zona como cartel de llegada (`ZoneBanner`) en Hub y Marte + etiqueta fija "Cráter Jezero"; sin textos de controles en pantalla; adelantar el tiempo (Espacio) solo en tormenta con paneles, apagón o modo seguro; orden por profundidad (Y) con `YSortCamera` + pivote.
- **Rover de conducción según la referencia del usuario:** 12 frames de 80×64 para S/N/E (izquierda con flipX) en `Art/Sprites/Rover/DrivingV2`, con vista previa y comando de Editor para conectarlo a `04_Mars`. Conexión y prueba en Unity pendientes.
- **Objetos del mapa de Marte (punto 5):** 12 PNG de puntos científicos, indicadores, módulo, escaneo y polvo repetible entregados localmente con `.meta`, hoja de revisión e informe. Ver `Assets/_Project/Art/MARS_OBJECTS_PREVIEW.html`; conexión y prueba en Unity pendientes, sin cambios de escena ni lógica y sin push.
- **Arte NPCs/rover preparado localmente:** BIMO (idle + movimiento S/N/E y tres retratos), Mateo y Luna (idle + retrato) y rover (dos frames por S/N/E + perfil). Ver `Assets/_Project/Art/GENERATED_ASSETS_REPORT.md`; PNG completos y metadatos incluidos, integración en Unity pendiente, sin cambios en los protagonistas/Vega ni push.
- **Dra. Elena Vega: recursos aportados guardados:** ocho orientaciones y nueve frames de idle en `Art/Sprites/Characters/DrElenaVega/`; retratos neutral, feliz y preocupada (128×128) en `Art/Sprites/Portraits/DrElenaVega/`, todos con `.meta`. Fuentes y manifiesto en `Elementos visuales/DrElenaVega/`. Pendiente conectar a escenas, diálogos y animación.
- **Fuentes de terrenos entregadas:** `Elementos visuales/Mars-Crafts-Terrenos/` contiene la biblioteca de 182 piezas, tiles de 32 px, exterior, interiores, catálogo y editables; ver su README para la diferencia con la escala de 16 px del juego. Entrega de fuentes; la integración del Tilemap sigue pendiente.
- **Base:** 6 escenas en Build Settings, en orden. `SceneFlow` navega por nombre (`Go`, `NewGame`, `Redesign`, `BackToTitle`). `SceneFader` (fundido) y `PauseMenu` (Esc) se crean solos. `AudioManager` + `Resources/AudioLibrary.asset` con todos los sonidos y músicas ya llamados desde el código.
- **UI con tema:** `Resources/UITheme.asset` define sprites y colores de toda la UI. `ThemedImage`/`ThemedText` aplican el tema; `UIKit` crea elementos ya tematizados; `ResourceBar` para barras. Cambiar el arte = rellenar el tema (ver `docs/architecture/como-reemplazar-arte-y-audio.md`).
- **Modelo (C# puro, `Scripts/Model/`):** `RoverDesign`, `Verifier` (mensajes de BIMO), `MissionState` (energía, recarga, tiempo, modo seguro, tormenta, apagón, escaneos), `MissionResult` (puntos + incidentes), `ReportBuilder` (estrellas, insignias, causa → decisión, comparación con Perseverance). `GameSession` con datos de depuración en cada escena.
- **Datos:** 14 `ComponentData` en `ScriptableObjects/Components/` (valores del doc de recursos); diálogos como `DialogueData` en `Data/Dialogues/`.
- **00_Title:** logo (texto provisional), tarjetas Tierra/Luna bloqueadas y Marte → nueva partida; aviso de no afiliación a la NASA.
- **01_Hub:** sala greybox con paredes, pantalla, consolas; Dra. Vega (sprite provisional: AstronautAgile) con briefing; BIMO con diálogo; puerta al Taller bloqueada hasta el briefing. Protagonista `Player.prefab` (AstronautStocky) animado.
- **02_Workshop:** tarjetas generadas desde `ComponentData` (una por categoría; instrumentos múltiples), barras de presupuesto y masa, resumen del rover, avisos del `Verifier` como BIMO, botón Lanzar solo si es válido. Rediseñar precarga el diseño anterior.
- **03_Launch:** cuenta regresiva, despegue y viaje Tierra → Marte animado (~7 meses); Espacio/clic salta.
- **04_Mars:** mapa greybox 40×30 (cauce, delta, sedimentos, zona rocosa, muralla, rocas, borde del cráter, módulo de aterrizaje), cámara Cinemachine con confiner, intro de BIMO, 3 puntos científicos con popup de dato NASA y fuente, `RockyZone` (solo con ruedas todoterreno), HUD con barras. Eventos: tormenta en t=45 s (30 s) y apagón en t=100 s (duración según antena). Al completar los 3 puntos se termina sola; también por tiempo (240 s) o botón.
- **05_Report:** estrellas, insignias, recursos, "qué pasó y por qué", comparación con Perseverance, Rediseñar/Terminar.
- **Balance:** moverse cuesta **2/s** (el doc decía 1; con 1 los paneles nunca se vaciaban). Se cambia en `MissionController` (Inspector). Simulado: solar + batería pequeña → modo seguro en la tormenta; RTG + 2 instrumentos → 2 estrellas; batería grande → sin modo seguro.

- **Arte de componentes, recursos y lanzamiento integrado (15 nov):** 14 íconos en `ComponentData.icon` (tarjetas del Taller), íconos de recursos en `UITheme` (HUD de Marte, presupuesto/masa del Taller, escudo/sin señal junto al estado), fondo del Taller, y `03_Launch` con plataforma, cohete, llama y humo animados (`LaunchSequence`), fondo espacial, planetas y nave. Fuentes en `docs/art-review/componentes-recursos-lanzamiento/`.

### Pendiente
- [ ] Reemplazar placeholders por arte (tileset de Marte e interior, rover, puntos, UI, íconos, retratos, fuente pixel) y audio.
- [x] Arte de terreno integrado (entrega `Elementos visuales/Mars-Crafts-Terrenos`, generada con IA): Marte con Tilemap de suelo + arena (cauce/delta), mesetas en el borde, rocas, cráteres, escombros en la zona rocosa, sprites de los 3 puntos y del rover; Hub con piso metálico, paneles, consolas y esclusa como puerta. **No hay tiles de transición**: los bordes de arena son escalonados (decisión aceptada).
- [ ] Verificar los datos NASA con su fuente (`docs/research/`) y rellenar `nasaFact` de los componentes.
- [ ] Definir el "consumo base" de energía para la regla de verificación (TODO en `Verifier`).
- [ ] **Build WebGL (objetivo de entrega)**: hacer una build de prueba temprano (el módulo WebGL está instalado; cambiar de plataforma reimporta todo y tarda). Probar en el navegador el flujo completo, audio (los navegadores lo activan tras el primer clic) y Esc (en el navegador sale de pantalla completa).
- [ ] Bordes del cauce/delta (único arte pedido que falta); revisarlo antes de integrarlo.
- [ ] Confirmar licencias del audio (`docs/creditos.md`) y añadir las fuentes (OFL).
- [ ] Opcional: mover escenas a `Scenes/Main/`, minijuego de aterrizaje, más retratos/expresiones.

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
