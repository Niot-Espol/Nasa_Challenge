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
