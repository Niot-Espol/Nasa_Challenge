# Mars Crafts — Contexto general del proyecto

> **Corte de esta transferencia: 15 de noviembre de 2026.** Documento descriptivo, no una autorización para ejecutar cambios. Verifica `git status`, rama, remoto y documentación antes de continuar: el repositorio puede haber avanzado. Responde al equipo en español.

## 1. Descripción del proyecto

Mars Crafts es un videojuego educativo 2D para el reto **Space Mission Design Game** de NASA Space Apps Challenge 2026, equipo Niot-Espol (evento local ESPOL). Su objetivo es enseñar decisiones de ingeniería y ciencia espacial: elegir componentes de una misión, respetar restricciones, observar consecuencias y explicar resultados. Es un proyecto independiente, sin afiliación, patrocinio ni respaldo de NASA; datos reales deben citar sus fuentes y distinguirse de las simplificaciones y la ficción.

La visión narrativa comprende misiones a Tierra, Luna y Marte. El **alcance acordado para la demo es Marte**: Tierra y Luna aparecen bloqueadas. Combina aventura/exploración top-down, personajes interactivos y construcción de un rover mediante una interfaz; no presupongas que todos los sistemas imaginados para el juego completo están implementados. El destino solicitado es **web mediante Unity**, con una build WebGL pendiente de verificar.

- Motor: Unity **6000.5.4f1**, proyecto Unity 2D, URP 2D, Input System nuevo, UGUI/TextMeshPro, Cinemachine y Physics 2D. Las versiones concretas están en `ProjectSettings/ProjectVersion.txt` y `Packages/manifest.json`.
- Repositorio: <https://github.com/Niot-Espol/Nasa_Challenge>; rama principal: `main`.
- Danilo es el integrador y responsable de Unity según `AGENTS.md`; los demás nombres, funciones y disponibilidad del equipo no están confirmados. Coordinar con él los cambios en escenas. El usuario ha dirigido la selección y generación de arte; Codex generó/preparó varias entregas con PixelLab.
- Referencia exacta de la última entrega: rama publicada `art/componentes-recursos-lanzamiento-20261115`, commit `eedfb40`, basada en `83b9537`. Se hizo push de la rama; **no se comprobó su fusión en `main`**. Consultar GitHub y el estado remoto actual. Este documento se creó después del push y, hasta que el usuario lo suba, solo existe en su copia local.

## 2. Historia y personajes

La historia vigente es la **versión 1.65** de `docs/game-design/historia-mars-crafts.md`; la versión 1.0 de supervivencia y reparación de nave es una propuesta descartada para el alcance actual. En un 2045 ficticio, el jugador es un cadete/comandante del programa Mars Crafts, un centro de investigación espacial. La ficción se inspira en ciencia y misiones NASA sin afirmar que sus personajes, vehículos o resultados simulados sean reales.

- **Dra. Elena Vega**, directora científica: plantea objetivos, orienta el briefing y analiza resultados. Cuenta con recursos y retratos neutral, feliz y preocupada.
- **BIMO**, robot asistente: guía, explica restricciones y comunica advertencias, incluso en la operación de Marte. Tiene expresiones y animaciones disponibles.
- **Mateo Rojas**, ingeniero aeroespacial: asociado al taller y a los vehículos en la historia; su presencia funcional en la demo actual no está confirmada.
- **Luna Torres**, especialista en operaciones: asociada al control de misión, lanzamientos y comunicaciones en la historia; su presencia funcional en la demo actual no está confirmada.
- El protagonista jugable es el cadete; el proyecto también contiene astronautas y animaciones elegidas que se deben conservar. No asignar identidades adicionales sin evidencia.

La visión futura plantea **Guardianes de la Tierra** (satélite de observación), **El secreto de la Luna** (misión robótica y búsqueda de condiciones relacionadas con hielo) y **Marte: Huellas del pasado** (rover y geología inspirada en Jezero/Perseverance). Solo la última está en el alcance actual; esas otras misiones, sus minijuegos y progresión no deben tratarse como implementados.

Flujo actual documentado: título → centro espacial y briefing de Vega/BIMO → taller y validación → lanzamiento y viaje → exploración de Marte → informe y opción de rediseñar. Un aterrizaje interactivo se considera opcional, no una escena confirmada.

## 3. Arquitectura técnica

Las rutas siguientes son relativas a la raíz del repositorio:

- `Assets/_Project/Scenes/`: seis escenas reales `00_Title`, `01_Hub`, `02_Workshop`, `03_Launch`, `04_Mars`, `05_Report`. No asumir que ya están dentro de `Scenes/Main/`: ese movimiento aparece como propuesta.
- `Assets/_Project/Scripts/Core/`: `SceneFlow`, `SceneFader`, `GameSession`, `AudioManager`, `AudioLibrary`, `YSortCamera`. `SceneFlow` navega y ofrece acciones para nueva partida, rediseño y retorno al título; `GameSession` transporta el diseño y resultado entre escenas. Las escenas pueden generar datos de depuración al abrirse directamente. No desactivar Domain Reload sin tratar correctamente los estáticos.
- `Scripts/Data/`: `ComponentData` (ScriptableObject, incluido su campo `icon`), `DialogueData` y `RoverDesignBuilder`. Los 14 componentes están en `Assets/_Project/ScriptableObjects/Components/`.
- `Scripts/Model/`: `RoverDesign`, `Verifier`, `MissionState`, `MissionResult` y `ReportBuilder`; contienen el modelo y reglas. Inspecciona el código real antes de cambiar cálculos.
- `Scripts/Gameplay/`: `TopDownMover`, `DirectionalSpriteAnimator`, `CharacterAnimator`, `NpcDialogue`, `Interactable`, `MissionController`, `ScienceSitePoint`, `RockyZone`, entre otros. `TopDownMover` usa Input System, lee entrada en `Update` y aplica física en `FixedUpdate`.
- `Scripts/UI/`: `WorkshopScreen`, `MissionHUD`, `LaunchSequence`, `ReportScreen`, `DialogueBox`, `ResourceBar`, `UITheme`, `UIKit`, `ThemedImage` y `ThemedText`. `Resources/UITheme.asset` centraliza recursos/colores del tema; no suponer que cada imagen generada ya está asignada.
- `Assets/_Project/Prefabs/Player.prefab` y `Prefabs/UI/DialogueBox.prefab` son prefabs observados. La arquitectura también menciona componentes compartidos; verificar su implementación real antes de asumir la existencia de otros prefabs concretos.
- `Assets/_Project/Art/`: sprites, UI, tilesets y reporte. `Elementos visuales/` conserva entregas y fuentes. `docs/game-design/`, `docs/architecture/`, `docs/art-review/` y `docs/research/` contienen diseño, arquitectura, composiciones y referencias científicas.
- `Scripts/Editor/` incluye `PixelArtImporter`, utilidades de escenas y `ApplyRoverDrivingArt`. Unity MCP (`com.coplaydev.unity-mcp`) figura como dependencia; su disponibilidad en la nueva computadora no está confirmada. El gestor de paquetes requiere acceso a la dependencia Git. Instalar el módulo WebGL correspondiente al editor si falta.

La documentación del repositorio pide no editar `.unity`, `.prefab` ni `.asset` como texto: usar Unity/MCP o scripts de editor y coordinar cambios. No modificar `Packages/` ni `ProjectSettings/` sin instrucción explícita. El inventario de carpetas anterior describe lo observado, no garantiza que la estructura remota no haya cambiado.

## 4. Mecánicas implementadas

Según `AGENTS.md`, los scripts y la documentación de diseño, existe una base jugable con:

- Movimiento top-down del astronauta en el Hub y del rover en Marte; entrada por WASD/flechas, interacción contextual y animación direccional. Se usa la vista derecha volteada para la izquierda cuando corresponda.
- Interacción con NPCs y diálogos basados en datos. El briefing habilita el acceso al taller; BIMO comunica avisos. El detalle exacto de diálogos y conexiones visuales debe verificarse en Unity.
- Taller con categorías energía, batería, instrumentos, comunicaciones, ruedas y lanzador. El jugador selecciona componentes; el verificador comprueba restricciones y explica problemas. La vista previa del rover conserva su diseño único. El botón de lanzar depende de la validez de la configuración.
- Recursos de juego: presupuesto, masa, capacidad de lanzamiento, energía/recarga y condiciones de comunicación. Solar ofrece más ciencia con vulnerabilidad durante tormentas; RTG ofrece energía estable con otras compensaciones. Son valores educativos de juego, no especificaciones reales.
- Cuenta regresiva, despegue y viaje Tierra→Marte mediante `LaunchSequence`; se documenta omisión con espacio/clic. El arte nuevo de esta transferencia todavía no estaba conectado a la secuencia.
- Exploración de Marte con tres puntos científicos, popup de datos y fuente, HUD, área rocosa condicionada por ruedas todoterreno y eventos de tormenta y pérdida de comunicación. La documentación registra tormenta a 45 s durante 30 s, apagón a 100 s y misión de 240 s; confirmar valores en configuración vigente antes de usarlos como constantes. El avance temporal tiene condiciones específicas en el código.
- Informe con puntuación, estrellas/insignias, explicación de causas y decisiones, comparación con Perseverance y regreso al taller. Modo seguro busca evitar un bloqueo definitivo.

Estos puntos describen sistemas presentes/documentados, no una prueba recién ejecutada: no se ha verificado en esta transferencia el flujo completo en Play ni en WebGL. La versión 1.65 también describe ideas de construcción modular visible, otras misiones y minijuegos que no deben confundirse con la demo actual.

## 5. Dirección artística

Pixel art auténtico de estética 16 bits/Game Boy Advance, contornos oscuros, píxeles nítidos, paleta limitada y siluetas claras. Mundo en vista top-down baja; los iconos de componentes usan presentación industrial en tres cuartos; el cohete usa vista lateral vertical y los fondos de lanzamiento son panorámicos. No cambiar la perspectiva de forma indiscriminada ni regenerar diseños aprobados.

Referencias: astronautas existentes, rover lateral elegido, `Art/Tilesets/Hub/LabTileset.png`, `Art/Tilesets/Mars/MarsTileset.png`, UI existente, tarjetas planetarias, tecnología científica y capturas/escenarios del proyecto. Paleta orientativa: contorno `#111923`, azul `#101725`, panel `#202E42`, metales `#435367`, `#617487`, `#8295A5`, `#C1CDD4`, cian `#58D9EE`, naranja `#E98B35`, amarillo `#EAC65B`, verde `#72C88B` y rojo `#D65C57`; Marte usa óxidos y ocres. Son referencias, no una obligación de incluir todos los colores en cada pieza.

- Componentes: 32×32, PNG transparente, un icono por componente. Recursos/HUD: 16×16. Fondos panorámicos: 320×180. Animaciones: PNG por frame, nombre con sufijo `_000`, `_001`, etc.
- Rover de conducción elegido: DrivingV2 **80×64**, cuatro frames para cada una de las direcciones Sur/Norte/Este; izquierda mediante volteo. Perfil de taller: 96×96. Los objetos no se pegan visualmente sobre el rover.
- No existe una escala universal fiable: la lista de arte menciona tiles de 16 px, `AGENTS.md` menciona 32 PPU para mundo y el tileset marciano integrado se documenta con cuadrícula de 22 px. Los nuevos iconos/lanzamiento se prepararon a 25 PPU para coincidir con la UI existente; DrivingV2 tiene 32 PPU según su reporte. Comprobar cada asset, importador, pivote y escena; no normalizar la escala del proyecto por suposición.
- Nuevos PNG de la última entrega: Sprite Single, filtro Point, sin compresión ni mipmaps, alfa, 25 PPU; fondo espacial con Repeat. Assets ya existentes pueden tener otros ajustes.
- Los valores, etiquetas, precios, estados, datos científicos, colisiones y lógica pertenecen a Unity/datos, no a los sprites. El fuego, humo, nave, planetas y fondos son recursos separados.

## 6. Recursos gráficos y PixelLab MCP

PixelLab MCP se utilizó de manera específica para generar sprites, efectos y fondos, con referencias reales del proyecto y seguimiento de identificadores hasta descargar los PNG; no regenerar automáticamente piezas existentes. Las familias reutilizaron referencias: panel solar y rover para componentes, batería pequeña para grande, rueda estándar para todoterreno, lanzador ligero para medio/pesado, comunicación activa para sin señal y los efectos base para animaciones. El usuario pidió PixelLab solo para generaciones específicas. Cualquier configuración o credencial debe obtenerse de forma segura fuera del repositorio; este documento no incluye secretos.

Estado según la lista vigente en la rama de arte y los reportes (un `[x]` puede significar entregado, no necesariamente conectado):

| Sección | Estado conocido y cautela |
|---|---|
| 1 Personajes | Protagonista, Vega y BIMO aparecen descritos como en juego; BIMO movimiento y sprites de Mateo/Luna están entregados. Mateo y Luna no aparecen en la misión actual según lista. |
| 2 Retratos | Vega y BIMO aparecen descritos como en juego; Mateo/Luna entregados. |
| 3 Rover | La lista más reciente declara DrivingV2 y perfil en juego. Un texto anterior de `AGENTS.md` y el reporte todavía dicen conexión pendiente: hay contradicción temporal; comprobar escena y último historial antes de afirmar cuál estado prevalece. |
| 4 Marte | Tileset/suelo, rocas y borde del cráter reportados integrados. Cauce y delta funcionan con bordes escalonados; faltan transiciones/esquinas y depósitos sedimentarios como pulido. |
| 5 Objetos Marte | La lista más reciente declara integrados puntos, módulo, indicadores, escaneo y polvo; notas anteriores del reporte dicen pendiente. Verificar escena/historial antes de modificar. |
| 6 Hub | `LabTileset.png` y elementos de interior reportados en juego; conservar construcciones y arquitectura visual elegidas. |
| 7 Taller | 14 iconos de 32×32 y fondo opcional de 320×180 generados, con `.meta` y validación técnica; **revisión visual e integración pendientes**. En `Assets/_Project/Art/Sprites/Icons/Components/` y `Art/UI/Workshop/`. El manifiesto vincula los iconos a los IDs reales de `ComponentData`. |
| 8 Recursos | 8 iconos de 16×16, incluidos modo seguro y sin señal, en `Art/Sprites/Icons/Resources/`; validados técnicamente, sin asignación funcional confirmada. Sin señal deriva de comunicaciones mediante una línea roja. |
| 9 Lanzamiento | Cohete 32×64; 3 frames de llama 16×32; 4 de humo 32×32; plataforma y espacio 320×180; Tierra/Marte 64×64 y nave 32×32 en `Art/Sprites/Launch/`. PNG separados, revisión visual e integración pendientes. Tierra y Marte reutilizan las tarjetas existentes; el fondo espacial tiene bordes coincidentes y la plataforma recibió un inpaint localizado para retirar un cohete incrustado. |
| 10 Interfaz | Botones, marcos, barras, diálogos, popup científico, estrellas, insignias, tarjetas, candado, logo, fondo de título y aviso BIMO aparecen como **en juego** en la lista más reciente. Reportes anteriores y cambios locales de una copia indican tareas pendientes; verificar conexiones reales en Unity y el historial actual antes de rehacer o reasignar. |

Las imágenes finales de la entrega 7–9, las fuentes y solicitudes sin credenciales se rastrean en `Elementos visuales/PixelLab-Componentes-Lanzamiento-20261115/manifest.json` y `raw/`. Las composiciones están en `docs/art-review/componentes-recursos-lanzamiento/index.html`; son montajes de revisión con sprites reales, no capturas ni evidencia de integración. `Assets/_Project/Art/GENERATED_ASSETS_REPORT.md` documenta rutas, referencias, jobs y limitaciones. Otras entregas y fuentes aparecen bajo `Elementos visuales/`; la biblioteca de terrenos anterior contiene 182 piezas y prototipos/editables, pero esos prototipos no equivalen a implementación Unity.

## 7. Estado actual del desarrollo

- **Arte de última tanda:** 36 PNG descargados y organizados (14 componentes, 8 recursos, fondo opcional de taller y 13 piezas/frames de lanzamiento), con fuentes y `.meta`; validación automatizada técnica y composiciones realizadas. No se ha recibido aprobación de integración visual.
- **Código y flujo:** hay una demo estructurada en seis escenas y mecánicas documentadas. No se ha comprobado aquí su funcionamiento completo actual en Unity, ni una build web. No describir la demo como terminada.
- **UI, personajes y mapa:** la lista de arte reciente declara varios activos integrados. Hay informes históricos que conservan el estado anterior «pendiente»; examinar escena, historial y rama actual en lugar de mezclar fechas. Una etiqueta de completado de una generación no demuestra integración funcional.
- **Último trabajo exacto:** commit `eedfb40` publicado en `art/componentes-recursos-lanzamiento-20261115`; no se modificaron escenas, prefabs, ScriptableObjects ni mecánicas para integrarlo. Su página de revisión es `docs/art-review/componentes-recursos-lanzamiento/index.html`. No hay confirmación de merge a `main`.
- **Copia de trabajo original:** al momento del corte, otra copia local `Nasa_Challenge` estaba en `main`, por detrás del `origin/main` conocido y con cambios previos preparados, en particular metadatos y documentación/revisiones de UI. No descartarlos, sobrescribirlos ni trasladarlos sin inspección y acuerdo; no asumir que están en GitHub. Esos cambios podrían no viajar al nuevo equipo si no se respaldan o publican. Esta observación es un estado histórico local, no una afirmación sobre el estado futuro del remoto.

## 8. Decisiones y restricciones

1. Conservar personajes, animaciones, construcciones, rover y recursos aprobados. Antes de generar, buscar equivalentes y consultar la lista actual. Evitar duplicación de trabajo.
2. El rover tiene un diseño visual único, independientemente del equipamiento. Los 14 componentes son iconos de UI, no piezas superpuestas ni variaciones del rover.
3. No modificar mecánicas, cálculos, misiones, diálogos, reglas de comunicación ni condiciones de lanzamiento sin autorización concreta. Respetar los sistemas existentes.
4. Cohete único para la secuencia; los tres lanzadores del taller son iconos de selección, no tres secuencias diferentes. Mantener cohete, llama, humo, planetas, nave, fondo y trayectoria dinámica independientes.
5. **La integración funcional de la entrega de arte 7–9 requiere aprobación del resultado visual**. No sustituir masivamente escenas ni prefabs antes de esa revisión. Esta transferencia no concede dicha aprobación.
6. Leer `AGENTS.md`; consultar estado de Git y coordinar con Danilo. No sobrescribir trabajo ajeno ni realizar operaciones destructivas. La política del repositorio pide sincronizar y coordinar escenas; prevalece cualquier instrucción más reciente del usuario, incluida su decisión sobre commit/push.
7. No editar como texto `.unity`, `.prefab` ni `.asset`; usar Unity/MCP o scripts de editor. Los assets nuevos o movidos requieren `.meta`; no borrar ni duplicar GUID. No tocar configuraciones de paquete/proyecto sin autorización.
8. Datos NASA verificables y con fuente; valores de simulación y ficción se identifican como tales. La interfaz no debe hornear cantidades o estados dinámicos.
9. Conservar resolución, escala, pivote y coherencia por recurso; usar Point, sin filtros borrosos. No imponer una cuadrícula o PPU única cuando las referencias discrepan.
10. Alcance actual: Marte para demo web. La narrativa de Tierra/Luna, antiguos prototipos de supervivencia y el antiguo vault son contexto histórico, no instrucciones actuales para construir otra versión del juego.

## 9. Errores y limitaciones

- La primera batería grande y el primer icono de soles tenían recortes; se generaron versiones corregidas. El primer fondo de plataforma incorporó elementos indeseados y otra versión incluyó un cohete; se corrigió localmente con inpaint para dejar cohete y fondo separados. Los originales descartados se conservan en las fuentes.
- La llama conserva el mismo anclaje superior en tres frames; el último frame de humo se eligió entre los devueltos para mostrar dispersión. El fondo espacial ajustó sus bordes para repetir. Los planetas se aislaron de las tarjetas, no se generaron de nuevo.
- Comprobación documentada de la última tanda: 36 PNG legibles, dimensiones, transparencia binaria y márgenes de sprites, `.meta` presentes, bordes espaciales coincidentes y anclaje de llama igual. Resultado: sin errores automáticos en `docs/art-review/componentes-recursos-lanzamiento/validation.json`. Esto **no** sustituye revisión artística, importación real ni prueba de juego.
- No se ejecutaron Unity Play ni build WebGL para la última entrega. Deben revisarse alineación del cohete/llama/humo en la escena, escala, visibilidad y comportamiento en el dispositivo/navegador.
- Se documentan bordes de cauce y delta escalonados por falta de tiles de transición; depósitos sedimentarios pendientes. Confirmar licencias de audio/fuentes, fuentes NASA y el `nasaFact` de componentes; definir consumo base pendiente de `Verifier` si se solicita.
- Documentación histórica contradictoria: `AGENTS.md`, lista de arte y reportes pueden reflejar momentos distintos; el texto «sin push» en notas anteriores no invalida el push posterior de la rama `eedfb40`. Tampoco prueba un merge a `main`. Verificar antes de intervenir.
- La información sobre fecha límite del evento y disponibilidad de Unity/WebGL en la nueva computadora no está confirmada. No afirmar que hubo una prueba nueva durante esta transferencia.

## 10. Plan de continuación

1. **Identificar la base correcta:** clonar/actualizar de forma segura, inspeccionar `git status`, ramas y PR/merge de `art/componentes-recursos-lanzamiento-20261115`. Asegurar que los cambios locales de la otra computadora estén respaldados; coordinar con Danilo antes de tocar escenas. No ejecutar un pull que sobrescriba cambios.
2. **Próxima acción concreta:** abrir `docs/art-review/componentes-recursos-lanzamiento/index.html`, revisar con el usuario las láminas de componentes, HUD, plataforma, despegue y viaje, y obtener su aprobación para integración o sus correcciones. Ver también `Art/GENERATED_ASSETS_REPORT.md` y `manifest.json`. La revisión visual precede cualquier cambio funcional.
3. **Tras aprobación:** inspeccionar `ComponentData`, `WorkshopScreen`, `MissionHUD`, `UITheme` y `LaunchSequence` y asignar arte con Unity de manera incremental: primero iconos según IDs reales, luego indicadores, luego elementos independientes del lanzamiento. Mantener datos, posiciones/eventos y trayectoria existente; no añadir reglas nuevas. El manifiesto de la entrega proporciona el mapeo de componentes.
4. **Verificar junto al equipo:** revisar si DrivingV2, Vega/BIMO, mapa y punto 10 ya están asignados en la rama real; evitar repetir trabajo. Resolver únicamente las conexiones faltantes autorizadas.
5. **Probar en Unity cuando se solicite:** título → Hub/briefing → taller con diseño válido → lanzamiento → Marte y eventos → reporte/rediseño. Comprobar cálculo y visuales con diferentes componentes, estados de comunicación/seguridad, efectos y referencias. Registrar resultados reales.
6. **Demo web:** instalar/verificar módulo WebGL, crear una build de prueba y comprobar en navegador controles, audio activado por interacción, presentación y flujo completo. Determinar alojamiento/despliegue con el equipo. Verificar fuentes NASA, licencias y posibles errores antes de entregar.
7. **Pulido posterior:** bordes de cauce/delta y depósitos sedimentarios, solo si hay tiempo y autorización; las misiones Tierra/Luna o aterrizaje son alcance futuro/opcional.

## 11. Instrucciones para el siguiente agente

- Lee `AGENTS.md`, la lista vigente y el reporte; inspecciona Git y los archivos reales antes de concluir qué falta. Compara fechas y commits cuando haya contradicciones. El remoto puede haber cambiado tras esta transferencia.
- Trabaja por tareas concretas y cambios pequeños. Conserva recursos originales y variantes; usa las composiciones para comunicar propuestas, no como pruebas de Play. No inventes rutas, herramientas, aprobaciones o datos.
- Para escenas, prefabs y ScriptableObjects usa Unity o herramientas de editor, nunca edición textual; coordina con Danilo y conserva `.meta` y referencias. Revisa las reglas del repositorio antes de cualquier cambio y da pasos de verificación.
- Los personajes, terrenos, rover y UI no requieren regeneración salvo una solicitud concreta. PixelLab se reserva para piezas específicas y no se deben incluir credenciales en archivos o mensajes.
- Si falta una decisión necesaria, indica la incertidumbre; no conviertas propuestas narrativas ni documentos de un estado anterior en funcionalidades implementadas. Pregunta por autorización de integración visual si sigue pendiente.
- No hacer commit, push ni publicar por cuenta propia si las instrucciones actuales del usuario lo prohíben; si se autoriza, identificar exactamente rama y cambios. Las restricciones específicas de una tarea prevalecen sobre pautas generales del repositorio.

## 12. Documentación de referencia

- `AGENTS.md`: normas del equipo, arquitectura y estado histórico; leer completo y contrastar con el historial actual.
- `docs/game-design/alcance-mision-marte.md`: alcance de la demo.
- `docs/game-design/historia-mars-crafts.md`: narrativa 1.65 y referencias; contiene también la versión 1.0 obsoleta.
- `docs/game-design/sistema-recursos-y-assets.md`: recursos, balance y componentes.
- `docs/game-design/mision-marte-implementada.md`: diseño de la misión implementada.
- `docs/game-design/lista-arte-para-dibujar.md`: lista de arte actual; sus marcas de las secciones 7–9 representan generación/validación técnica, no integración.
- `docs/game-design/UI_ART_PLAN.md`: planificación de la interfaz.
- `docs/architecture/arquitectura-unity.md` y `docs/architecture/como-reemplazar-arte-y-audio.md`: organización y puntos de conexión; contrastar rutas de propuestas con archivos reales.
- `Assets/_Project/Art/GENERATED_ASSETS_REPORT.md`: entregas gráficas y estado reportado en cada momento.
- `docs/art-review/componentes-recursos-lanzamiento/index.html` y `validation.json`: revisión y comprobaciones de la última tanda.
- `Elementos visuales/PixelLab-Componentes-Lanzamiento-20261115/manifest.json` y `raw/`: rutas, IDs, hashes, fuentes y referencias de las generaciones.
- `docs/art-review/ui/`: composiciones de interfaz; `docs/creditos.md` y `docs/research/`: créditos y referencias científicas.
- `Packages/manifest.json` y `ProjectSettings/ProjectVersion.txt`: dependencias y versión realmente fijadas; consultar sin modificarlas salvo autorización.
