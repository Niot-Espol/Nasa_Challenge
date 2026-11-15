# Mars Crafts — recursos generados con PixelLab MCP

Fecha: 15 de noviembre de 2026. Rama local: `art/pixellab-npcs-rover-20261115`. Base: `1e5aa58ead0aa62c18ecc1fd86c9a41af809c953`. **Sin push.**

## Resultado

- BIMO: diseño pulido manteniendo el preliminar, tres orientaciones de referencia y **16 frames finales** de 32×32: idle South (4), Move South/North/East (4 por dirección).
- BIMO: tres retratos de **64×64**, neutral, alegre y alarmado; misma carcasa y encuadre.
- Mateo: sprite base South y **2 frames idle de 32×32**, más retrato neutral de **64×64**.
- Luna: sprite base South y **2 frames idle de 32×32**, más retrato neutral de **64×64**.
- Rover: **6 frames de 32×32**, dos por South/North/East, más perfil de **96×96** para el taller.
- En total: **32 PNG de entrega** (26 frames animados, 5 retratos y un perfil), más cinco referencias base de personajes. No se generó West, diagonales, mapas, interfaces ni equipamiento adicional.

## Inspección previa y recursos reutilizados

Se localizaron los tres astronautas en `Sprites/Characters/AstronautStocky`, `AstronautAgile` y `AstronautCute`; Vega en `Sprites/Characters/DrElenaVega` y sus retratos en `Sprites/Portraits/DrElenaVega`. Se verificaron por SHA-256 **143 archivos protegidos**, sin cambios. Sus .meta tampoco forman parte del diff.

La referencia artística directa de sprites fue `DrElenaVega/Rotations/vega-idle-south.png`. Para retratos se usó una copia de referencia recortada, sin interpolación, de su retrato neutral original de 128×128: recorte (32,2,64,64). El original no se modificó. En las composiciones de referencia, Vega aporta estilo y el sprite definitivo del NPC aporta identidad. Las composiciones se enviaron realmente a PixelLab como PNG base64 a través del MCP.

BIMO aún era un placeholder en la escena, pero se encontró su generación preliminar `gallery:1794720437047` y un idle previo `gallery:1794720551111` en la cuenta conectada. El primero fue la referencia de identidad del pulido; no se sustituyó por otro concepto. Mateo y Luna no tenían sprites o retratos propios en el repositorio ni en la galería inspeccionada.

El rover existente es `Sprites/Mars/Vehicles/rover-exploracion.png`, con fuente mayor en `Elementos visuales/Mars-Crafts-Terrenos/elementos/vehiculos/rover-exploracion.png`. Su PNG y GUID originales se conservan. No se encontró una imagen de perfil independiente y `UITheme.roverPreview` estaba vacío: el perfil nuevo se derivó de la referencia del vehículo existente y de las nuevas vistas. No se afirma haber utilizado un perfil previo inexistente.

Se inspeccionaron `01_Hub`, `04_Mars`, `Player.prefab`, los controladores y clips de astronautas, `NpcDialogue`, `Bobbing`, `DirectionalSpriteAnimator`, `DialogueData` y `UITheme`. El rover ya usa `DirectionalSpriteAnimator`, con tres listas y reflejo de East para West. BIMO tiene diálogo y flotación, sin controlador de personaje propio. No se encontró actor propio de Mateo o Luna.

## Herramientas y preparación

Se consultaron los esquemas reales del MCP. Se usaron `edit_image_pro_flash` para pulir/rotar BIMO y sus expresiones; `animate_image` para idle y desplazamientos; `create_image_pro_flash` para sprites humanos y retratos con referencias; `edit_image` para mejorar el rover; `get_image` para comprobar trabajos y recuperar los PNG. La ruta de personaje multidireccional solo admite 4/8 vistas, por lo que se usaron imágenes individuales referenciadas para no gastar en West ni diagonales. No se creó un personaje distinto por cada orientación.

PixelLab devolvió cinco imágenes por animación (entrada más cuatro generadas). En BIMO se seleccionaron índices 0–3; en Mateo y Luna, 0 y 2 para el idle de dos fases. Se conservaron los resultados descargados en `Elementos visuales/PixelLab-NPCs-Rover-20261115/raw/`. Cuando el servidor devolvió frames individuales no se inventó una hoja original; la hoja general es una composición de revisión.

Los PNG finales son RGBA8, alfa binario, máximo 32 colores por imagen, tamaño exacto y filtro de presentación nearest/Point. Se fijó la paleta de cada ciclo a la de su sprite base y se alinearon los humanos al centro y al pie y=30; BIMO mantiene su anclaje inferior y=27. Los originales sin normalizar se conservan en fuentes. Las expresiones de BIMO se compusieron únicamente dentro de su pantalla (x=21..43, y=20..35), conservando todos los píxeles externos del retrato neutral. El rover usa dos fases de dibujo de bandas de ruedas: el chasis, el alfa y los componentes no cambian entre frames; no se regeneró el vehículo entero para mover las ruedas.

## Unity y pruebas pendientes

Se incluyen .meta nuevos con GUID únicos: Sprite Single, Point, sin compresión ni mipmaps, transparencia activa y lienzo completo. Personajes: **16 PPU**, pies al centro inferior; retratos: centro; rover: **32 PPU**, centro, según los importadores efectivos existentes. AGENTS.md menciona otra escala, pero se tomó como evidencia la configuración real de los .meta y `PixelArtImporter.cs`. No se modificó la escala global.

No se ejecutó Unity, Play ni build WebGL. **No se modificaron escenas, prefabs, controladores existentes, movimiento, diálogos o misiones.** Los PNG y secuencias están completos; su asignación a la escena sigue pendiente. No se afirma que el juego ya los muestre.

Se añadió `Scripts/Editor/GeneratedNpcArtClips.cs`, una utilidad de editor que usa el sistema Animation/Animator existente, sin código runtime ni ejecución automática. Al abrir Unity, usar **Mars Crafts → Art → Prepare generated NPC clips**: genera clips cíclicos estándar de BIMO, Mateo y Luna y controladores idle, sin tocar escenas. El menú no se ha ejecutado aquí. Velocidades de revisión configurables: BIMO idle 4 fps, movimiento 6 fps; humanos 2 fps.

Para integrar: asignar los dos frames por dirección del rover a las listas Down/Up/Side del `DirectionalSpriteAnimator` existente; asignar el perfil a `UITheme.roverPreview` y el neutral de BIMO a `portraitBimo`. Revisar la escala visual del rover de 32 px frente al anterior de 65 px antes de sustituirlo; no se cambió su tamaño global ni física. Adjuntar los clips idle al mismo objeto que tiene SpriteRenderer, sin reemplazar los controladores de los protagonistas. El sistema actual de diálogos solo contiene campos de retrato para Vega y BIMO: los retratos de Mateo/Luna y expresiones adicionales quedan preparados, sin introducir un sistema nuevo. No se implementó seguimiento de BIMO; se entregaron sus frames de movimiento.

## Correcciones y límites

La primera vista East de BIMO resultó demasiado frontal. Se descartó y se corrigió a perfil derecho; la animación iniciada sobre la vista descartada no forma parte de los PNG finales. No hubo fallos finales del servicio que dejaran recursos gráficos pendientes. Las limitaciones restantes son integración y verificación dentro de Unity. Las imágenes de revisión no acreditan pruebas de motor.

No se almacenó el token en el repositorio ni se incluyeron URLs firmadas o credenciales en la procedencia. Los identificadores siguientes sirven para trazabilidad; los archivos PNG reales están guardados localmente.

## Archivos finales

Las rutas son relativas a `Assets/_Project/Art/`. Los frames se numeran desde cero.

| Archivo | Resolución | Dirección | Frame |
|---|---|---|---|
| `Sprites/Characters/BIMO/Rotations/south.png` | 32×32 | south | — |
| `Sprites/Characters/BIMO/Rotations/north.png` | 32×32 | north | — |
| `Sprites/Characters/BIMO/Rotations/east.png` | 32×32 | east | — |
| `Sprites/Characters/BIMO/Idle/South/idle_south_000.png` | 32×32 | south | 0 |
| `Sprites/Characters/BIMO/Idle/South/idle_south_001.png` | 32×32 | south | 1 |
| `Sprites/Characters/BIMO/Idle/South/idle_south_002.png` | 32×32 | south | 2 |
| `Sprites/Characters/BIMO/Idle/South/idle_south_003.png` | 32×32 | south | 3 |
| `Sprites/Characters/BIMO/Move/South/move_south_000.png` | 32×32 | south | 0 |
| `Sprites/Characters/BIMO/Move/South/move_south_001.png` | 32×32 | south | 1 |
| `Sprites/Characters/BIMO/Move/South/move_south_002.png` | 32×32 | south | 2 |
| `Sprites/Characters/BIMO/Move/South/move_south_003.png` | 32×32 | south | 3 |
| `Sprites/Characters/BIMO/Move/North/move_north_000.png` | 32×32 | north | 0 |
| `Sprites/Characters/BIMO/Move/North/move_north_001.png` | 32×32 | north | 1 |
| `Sprites/Characters/BIMO/Move/North/move_north_002.png` | 32×32 | north | 2 |
| `Sprites/Characters/BIMO/Move/North/move_north_003.png` | 32×32 | north | 3 |
| `Sprites/Characters/BIMO/Move/East/move_east_000.png` | 32×32 | east | 0 |
| `Sprites/Characters/BIMO/Move/East/move_east_001.png` | 32×32 | east | 1 |
| `Sprites/Characters/BIMO/Move/East/move_east_002.png` | 32×32 | east | 2 |
| `Sprites/Characters/BIMO/Move/East/move_east_003.png` | 32×32 | east | 3 |
| `Sprites/Portraits/BIMO/bimo_neutral.png` | 64×64 | — | — |
| `Sprites/Portraits/BIMO/bimo_happy.png` | 64×64 | — | — |
| `Sprites/Portraits/BIMO/bimo_alert.png` | 64×64 | — | — |
| `Sprites/Characters/Mateo/Rotations/south.png` | 32×32 | south | — |
| `Sprites/Characters/Mateo/Idle/South/idle_south_000.png` | 32×32 | south | 0 |
| `Sprites/Characters/Mateo/Idle/South/idle_south_001.png` | 32×32 | south | 1 |
| `Sprites/Portraits/Mateo/mateo_neutral.png` | 64×64 | — | — |
| `Sprites/Characters/Luna/Rotations/south.png` | 32×32 | south | — |
| `Sprites/Characters/Luna/Idle/South/idle_south_000.png` | 32×32 | south | 0 |
| `Sprites/Characters/Luna/Idle/South/idle_south_001.png` | 32×32 | south | 1 |
| `Sprites/Portraits/Luna/luna_neutral.png` | 64×64 | — | — |
| `Sprites/Rover/TopDown/South/move_south_000.png` | 32×32 | south | 0 |
| `Sprites/Rover/TopDown/South/move_south_001.png` | 32×32 | south | 1 |
| `Sprites/Rover/TopDown/North/move_north_000.png` | 32×32 | north | 0 |
| `Sprites/Rover/TopDown/North/move_north_001.png` | 32×32 | north | 1 |
| `Sprites/Rover/TopDown/East/move_east_000.png` | 32×32 | east | 0 |
| `Sprites/Rover/TopDown/East/move_east_001.png` | 32×32 | east | 1 |
| `Sprites/Rover/Workshop/rover_side.png` | 96×96 | — | — |

## Trabajos de PixelLab

| ID | Operación registrada | Generaciones cobradas |
|---|---|---|
| `869f4d57-7380-455d-9f64-deb4d99bacc4` | bimo-base-result | 5 |
| `f44a7ad2-80f3-4d1b-a237-fadfbd1a744b` | bimo-east-corrected-queue | 5 |
| `0112b7f5-f29d-4643-ac92-559f889cf380` | bimo-east-queue | 5 |
| `65028ac8-97ce-49d8-9770-b619fd59e319` | bimo-idle-queue | ver recibo de PixelLab |
| `8874a2c6-dbe0-4aa3-9e1b-6c1ae593b8f5` | bimo-move-east-corrected-queue | ver recibo de PixelLab |
| `02a91c49-2a38-4a6a-a83d-a5a7f1947399` | bimo-move-east-queue | ver recibo de PixelLab |
| `66daea97-7dd1-43e1-ba83-94bf118a6681` | bimo-move-north-queue | ver recibo de PixelLab |
| `f1c7ffa9-7a2f-40db-9b82-a844167b87c7` | bimo-move-south-queue | ver recibo de PixelLab |
| `566fda17-1240-4bae-a536-c3ec0173fca6` | bimo-north-queue | 5 |
| `c910f72c-832f-45fb-8740-e2bea26f41d3` | bimo-portrait-alert-queue | 5 |
| `64452e08-5179-4c31-8ed7-e67741b16d8c` | bimo-portrait-happy-queue | 5 |
| `a30eae25-3d33-4940-bde6-0a3ebaa26709` | bimo-portrait-neutral-queue | 5 |
| `4203d70f-b1d4-4f7a-923a-daff948b6403` | luna-base-queue | 5 |
| `a402c29a-e565-4bfb-9c63-8a1e6fadac93` | luna-idle-queue | ver recibo de PixelLab |
| `034d465e-8309-4850-8d6f-bbe46c5dc4d1` | luna-portrait-queue | 5 |
| `f586ccc4-764a-4142-9b94-ad75825915df` | mateo-base-queue | 5 |
| `59bfdaa1-7aa7-47cf-a19c-defd71f5aa29` | mateo-idle-queue | ver recibo de PixelLab |
| `4b72f5ac-cf8c-4b1a-b4dd-e28e445a8ae8` | mateo-portrait-queue | 5 |
| `26ef3a58-92b5-45be-9119-a1e59c756ff4` | rover-east-queue | 5 |
| `cc10a0c7-e3cf-4e3c-9cce-50e88c5e0c38` | rover-north-queue | 5 |
| `1758469c-5723-4899-8e9a-676e58f6bea7` | rover-side-corrected-queue | 5 |
| `7c8efa0a-6f84-46b9-861d-767904dffa11` | rover-side-queue | 10 |
| `e0674bc9-caf8-4090-aba1-8ebc4453b791` | rover-south-polished-queue | 5 |
| `72b15c3a-767d-4c6b-a929-c0992f970405` | rover-south-queue | 10 |

## Revisión artística

Abrir `GENERATED_ASSETS_PREVIEW.html` para nombres y ciclos, o `GENERATED_ASSETS_PREVIEW.png` para la hoja estática. Su orden es por filas: BIMO South/North/East y sus tres retratos; Mateo sprite/retrato, Luna sprite/retrato, rover South/North; rover East y perfil. `GENERATED_ASSETS_MANIFEST.json` contiene rutas, hashes, anclajes de caja visible, trabajos y comprobaciones.

## Consumo observado

La cuenta pas? de 0 a 102 generaciones usadas durante esta tarea; al cierre quedan 1898 de 2000. Esto incluye las correcciones y el ciclo descartado. No se compraron cr?ditos ni se modific? la suscripci?n. Los recibos de cada trabajo completado se incluyen arriba.


## Objetos del mapa de Marte — Punto 5

Entrega local del 15 de noviembre de 2026. **Los ocho recursos solicitados están entregados como 12 PNG finales**, generados con PixelLab MCP, revisados visualmente y con metadatos Unity. No se modificaron escenas, prefabs, protagonistas, NPCs, retratos ni rover. La conexión y la prueba en Unity siguen pendientes; no se hizo push de esta entrega.

### Inspección y reutilización

- Documento vigente: `docs/game-design/lista-arte-para-dibujar.md`, sección 5. Se completan únicamente sus ocho filas.
- Terreno realmente importado: `Art/Tilesets/Mars/suelo-1-1.png`, `suelo-1-2.png`, `suelo-liso.png` y `arena-clara.png` (16×16). Se usan como fondo de revisión; no se generan ni alteran tilesets o mapas.
- `04_Mars/Map/ScienceSites/Site_Delta/Body` usa `Mars/Rocks/roca-escalonada.png` (45×39); `Site_Mineral/Body`, `Mars/Resources/deposito-rojo.png` (33×29); `Site_SpecialRock/Body`, `Mars/Resources/deposito-cristal.png` (47×39). Son recursos compartidos de la biblioteca de terreno, por lo que se conservan originales y GUID; las mejoras son sprites independientes.
- Delta: se conserva la identidad cálida escalonada, refinada como abanico bajo con estratos. Mineral: se conserva el rojo óxido, con superficie angular y vetas claras. Roca especial: se conserva el acento cian identificativo, reducido a inclusiones minerales en una roca erosionada con abertura; sin grandes cristales fantásticos.
- Plataforma fuente encontrada: `Elementos visuales/Mars-Crafts-Terrenos/elementos/edificios/plataforma-aterrizaje.png` (194×192). El módulo mantiene plataforma gris y marcas naranjas, incorpora rampa y cuatro apoyos. No se encontró objeto `Lander` ni equivalente conectado en la escena actual.
- Los indicadores actuales son hijos TMP `Prompt` (`[E]`) y `Done` (`OK`); no había sprites de estos indicadores en el proyecto Unity. Se inspeccionó como referencia de interfaz el icono de escaneo existente en el paquete local HUD (`assets/mars-crafts/hud-v1/pieces/icon-scan-normal.png`), conservado en las fuentes de esta entrega.
- Rover actual de la escena: `Mars/Vehicles/rover-exploracion.png` (65×58, 32 PPU). Se inspeccionó también el nuevo rover de 32×32, sin regenerarlo ni cambiarlo. La hoja usa el rover conectado y sus PPU reales para comparar proporciones físicas.

### Archivos finales

Todas las rutas siguientes son relativas a `Assets/_Project/Art/Sprites/Mars/`.

| Recurso | Ruta | Dimensiones | Frames |
|---|---|---|---|
| Delta sedimentario | `SciencePoints/science_point_delta.png` | 32×32 | 1 |
| Afloramiento mineral | `SciencePoints/science_point_mineral.png` | 32×32 | 1 |
| Roca especial | `SciencePoints/science_point_special_rock.png` | 32×32 | 1 |
| Punto completado | `Indicators/science_completed.png` | 16×16 | 1 |
| Interacción E | `Indicators/Interaction/interaction_e_000.png` y `001` | 16×16 | 2 |
| Módulo | `Landing/landing_module.png` | 48×48 | 1 |
| Escaneo | `Effects/Scan/scan_000.png` a `003` | 32×32 | 4 |
| Polvo | `Effects/Dust/mars_dust_texture.png` | 64×64 | 1 |

### Generación y ajustes

Catálogo real consultado: 154 herramientas. Se eligió `create_image_pro_flash` para ocho diseños individuales con referencia, tamaños nativos y fondo transparente. `animate_image` generó las fases del escaneo; `get_image` descargó los PNG reales. No había herramienta dedicada de textura transparente repetible en el catálogo: se generó la textura de polvo como imagen y se verificaron sus bordes y mosaico.

| Trabajo | Identificador PixelLab |
|---|---|
| Delta | `0fc6139f-ce77-4d75-acd1-561f66f3a3a7` |
| Mineral | `90daf0e9-d259-4906-af55-b470e28d888d` |
| Roca especial | `d120b480-b901-4bc7-bd5b-017b017eb136` |
| Completado | `3ebe8e57-47da-436c-95f5-1e1b55e60d3e` |
| Interacción | `263ce9eb-2786-4cc5-8a8b-10bc0510ceae` |
| Módulo | `6c62ef2c-e6e8-4d90-9272-223e0ed3f1f8` |
| Escaneo base | `4843da62-ec33-4fd8-a9b0-0b9e9b524900` |
| Animación de escaneo | `a7091468-09f6-4a0e-b2ea-8160980217a1` |
| Polvo | `4501380d-83f5-4ea8-80df-cc4f7740de88` |

- Las referencias grandes se ajustaron con Nearest Neighbor y margen transparente para cumplir el límite de referencia de MCP. Los originales permanecen intactos.
- Puntos científicos alineados a una base común en y=30, sin recortar siluetas; módulo contenido con margen de dos píxeles. El módulo se ajustó ligeramente con Nearest Neighbor para recuperar ese margen.
- La letra generada parecía un 8: se conservó el marco y se reparó únicamente el área del glifo con una E explícita de 3×5 píxeles. El segundo frame usa una traslación vertical exacta de un píxel, sin cambiar el dibujo.
- La animación devuelve la entrada más cuatro frames. Se seleccionaron índices 0, 2, 3 y 4, se recentraron y se ajustaron con Nearest Neighbor dentro de cuadrados de 12, 20, 26 y 28 px. Progresión: onda pequeña, mediana, expandida y partículas dispersas; el centro sigue transparente. No se dibujó una animación sustitutiva mediante código.
- Polvo: se mantuvo la distribución generada y se cambiaron destellos casi blancos por ocre apagado. El 96,7 % del lienzo es transparente; los cuatro bordes son transparentes e idénticos a sus opuestos. Revisado como mosaico 3×3 y sobre terreno.
- Coste real: **41 generaciones**, saldo de 1898 a **1857**. No se generaron variantes adicionales ni se alteró la suscripción.
- Fuentes PNG originales, cinco frames originales del escaneo, prompts sin credenciales, referencias exactas y procedencia: `Elementos visuales/PixelLab-Mars-Objects-20261115/`. MCP no proporcionó un spritesheet de animación; se conservaron todos los PNG devueltos.

### Importación y conexión pendiente

Se respeta `PixelArtImporter.cs` y los metadatos efectivos: **16 PPU** para objetos y tiles. La descripción general de 32 PPU en el encabezado de AGENTS está desactualizada respecto al importador; no se cambia la escala global. PNG individuales con Sprite Single, filtro Point, compresión desactivada, sin mipmaps y alfa real. Pivote abajo-centro para puntos y módulo; centro para indicadores y efectos. Se amplía únicamente el importador de estas carpetas para que conserve esos pivotes y `Repeat` en polvo al reimportar; el resto mantiene su configuración.

No se modifican escenarios, colliders, identificadores, datos científicos, campos de diálogo, tiempos ni lógica. No hay prefabs independientes de los puntos científicos. Para integrar respetando la escena existente:

1. En `04_Mars/Map/ScienceSites/Site_*`, asignar el nuevo PNG correspondiente al SpriteRenderer de `Body`. Conservar transform, collider, `site`, `completedTint`, referencias y datos. Los originales medían más de 32 px: revisar su huella al sustituir, sin aplicar automáticamente cambios de escala o colisión.
2. Para `Done` y `Prompt`, conservar el mismo GameObject y las referencias `completedMarker` / `interactPrompt`. Sustituir solo la representación TMP por SpriteRenderer, dejando intacta la activación existente. `ScienceSitePoint.Start` oculta Done; `OnSiteCompleted` lo activa. `Interactable` controla Prompt por proximidad y mantiene E. Los dos frames de E pueden usarse en el sistema de animación habitual.
3. El documento de arquitectura menciona `Map/Lander`, pero ese objeto no está en la escena actual. Módulo entregado como recurso; colocación y colisiones quedan para el integrador, sin inventar una posición ni un nuevo objeto de escena aquí.
4. El escaneo actual es instantáneo (`MissionController.Scan` / `MissionState.TryScan`); no existe consumidor visual o animación de análisis. El polvo solo está representado en HUD/estado y audio. Los efectos se entregan listos; su conexión requiere aprovechar los eventos existentes (`SiteCompleted`, `DustStormActive`) cuando se autorice integración visual. No se añade un sistema de investigación ni una nueva lógica de tormentas.
5. Para probar en Play tras conectar: acercarse a cada punto, verificar que E aparece solo dentro de su rango; completar el análisis y comprobar que Done aparece solo al éxito; verificar escaneo solo en análisis válido y polvo solo durante tormenta. Repetir recorrido normal y fracaso por falta de energía/equipamiento. Las duraciones, consecuencias y condiciones deben permanecer iguales.

### Revisión y límites

Abrir `MARS_OBJECTS_PREVIEW.html` para animaciones y `MARS_OBJECTS_PREVIEW.png` para la hoja de comparación sobre el suelo actual, con rover a su proporción física. `MARS_OBJECTS_MANIFEST.json` registra tamaños, hashes, transparencia, frames, trabajos y referencias; `MARS_OBJECTS_VERIFICATION.json` contiene las comprobaciones técnicas.

Los ocho ítems de arte están completos. **Pendiente: conectar y validar en Unity Play y en build WebGL.** La revisión de sprites y la hoja no equivalen a una prueba de ejecución del juego. No se hizo push automático.


## Rover de conducción — referencia del usuario (15 noviembre 2026)

El usuario eligió el rover de seis ruedas que se ve en su captura de `04_Mars` y proporcionó su PNG original de 96×96. La figura ocupa 76×58 px. Se recortó solo el margen transparente y se centró en lienzos de **80×64 px** para preservar su tamaño visible en el juego, en lugar de los 32×32 px de la propuesta anterior. El frame `East/drive_east_000.png` contiene exactamente los píxeles visibles de la referencia del usuario.

PixelLab MCP `edit_image_pro_flash` generó las vistas hacia abajo (`South`) y arriba (`North`) a partir de ese diseño. `animate_image` generó cuatro frames de conducción para cada dirección: movimiento de ruedas y rebote leve. La izquierda la cubre el `flipX` existente de `DirectionalSpriteAnimator` al usar `East`. El proceso devolvió cinco imágenes por animación (base más cuatro generadas); se eligieron las primeras cuatro para ciclos cortos y se conservaron todas las respuestas originales en `Elementos visuales/PixelLab-Rover-Driving-20261115/raw/`.

Archivos finales: `Assets/_Project/Art/Sprites/Rover/DrivingV2/{South,North,East}/drive_{south,north,east}_{000..003}.png`, **12 PNG** de 80×64, RGBA transparente, Point, sin compresión ni mipmaps, pivote central y 32 PPU para igualar el rover ya asignado en `04_Mars`. Las versiones anteriores permanecen disponibles. Jobs MCP: South `a7865b6f-9678-402a-8f49-514addb40bed`, North `a938cc77-d4e0-4dc3-9178-ba98c00fd97e`, animación East `5fad21bd-f6a3-4445-bab9-b3000a1cc122`, South `928754a0-5084-4519-b729-07346c703916`, North `c30c00bd-01de-4970-99a4-2f77fff897e8`.

`ROVER_DRIVING_PREVIEW.html` reproduce las cuatro direcciones y `ROVER_DRIVING_PREVIEW.png` las muestra sobre el suelo existente. Para asignar los frames al rover de la escena se agregó el comando del Editor **Mars Crafts → Art → Aplicar animación del rover a Marte**. Ejecutarlo en Unity abre `04_Mars`, rellena las tres listas del `DirectionalSpriteAnimator` y guarda la escena conservando controlador, posición, collider y movimiento. No se ejecutó aquí porque Unity no está instalado en este entorno; la prueba en Play y WebGL queda pendiente. En Play: mover con WASD/flechas hacia los cuatro lados y detenerse; las ruedas deben animarse al mover y volver al frame 000 en reposo.
