# Cómo reemplazar el arte y el audio

Todo el juego funciona con placeholders (cuadrados de color y texto). El arte y el audio se conectan en **pocos lugares centrales**, sin tocar código ni rearmar escenas.

> Antes de conectar sprites: deben estar importados como **Sprite**, con filtro **Point**, **sin compresión** y **16 px = 1 unidad**. Todo lo que entra en `Art/Sprites/Characters/` se configura solo (`PixelArtImporter`). Para otras carpetas, copia esa configuración o amplía `PixelArtImporter.Folder`.

## 1. Interfaz → `Assets/_Project/Resources/UITheme.asset`
Selecciona el asset y arrastra los sprites en el Inspector:

| Campo del tema | Pieza de la lista de arte | Notas |
|---|---|---|
| `Elements → Panel / PanelDark` | Marco de panel (9-slice) | Define los bordes en el Sprite Editor (Border) para que se estire bien. |
| `Elements → Button / ButtonPrimary` | Botón (normal) | El hover y el presionado los da el tinte del Button. |
| `Elements → DialogueBox` | Caja de diálogo | |
| `Elements → FactPopup` | Popup de dato NASA | |
| `Elements → Slot / SlotSelected` | Tarjeta del taller (normal / elegida) | |
| `Elements → BarFrame / BarFill` | Barra (marco + relleno) | El relleno se recorta solo (Image Filled). |
| `Elements → Overlay` | — | Fondo oscuro del menú de pausa. |
| `font` | Fuente pixel libre | Crea el **TMP Font Asset** (Window → TextMeshPro → Font Asset Creator) y arrástralo. |
| `iconBudget … iconLock` | Íconos de recursos (16×16), aviso y candado | |
| `starFull / starEmpty`, `badge…` | Estrellas e insignias del reporte | |
| `portraitVega / portraitBimo` | Retratos (64×64) | Hoy hay 1 por personaje; si llegan 3 expresiones, se agrega el campo. |
| `logo`, `titleBackground`, `missionEarth/Moon/Mars` | Pantalla de título | |
| `launchBackground`, `spaceBackground`, `rocket`, `rocketFlame[]`, `earth`, `mars`, `cruiseShip` | Lanzamiento y viaje | |
| `roverPreview` | Rover de perfil (96×96) | Vista previa del taller. |

Con un sprite asignado, cada elemento lo usa; si el campo está vacío, se usa el color plano. Los colores (`accent`, `good`, `warning`, `error`, `textColor`) también se cambian aquí.

## 2. Íconos de los componentes → `Assets/_Project/ScriptableObjects/Components/*.asset`
Cada componente tiene un campo **`icon`** (32×32). Las tarjetas del taller lo muestran automáticamente.

## 3. Personajes
- **Protagonista:** ya animado (`Art/Animations/Characters/Astronaut.controller`).
- **Dra. Vega / BIMO** (`01_Hub` → `NPCs`): reemplaza el sprite del hijo `Sprite`. Si llegan animaciones, crea los clips igual que los del astronauta.
- **Otro protagonista:** en `Prefabs/Player.prefab` → hijo `Sprite` → Animator → cambia el controller por el `.overrideController` del personaje.

## 4. Rover (`04_Mars` → `Rover`)
El componente **`DirectionalSpriteAnimator`** tiene 3 listas: **Down, Up y Side** (Side mirando a la derecha; la izquierda se voltea sola). Arrastra los 2 frames de cada dirección. Mientras estén vacías, se ve el cuadrado.

## 5. Mapa de Marte
- **Tileset:** se arma un Tilemap nuevo encima del greybox (`04_Mars` → `Map`). Después se borran los cuadrados de `Map` y se ajustan los colliders. *(Pídeselo a la IA con el tileset en el repo.)*
- **Puntos científicos** (`Map/ScienceSites/Site_*`): el sprite va en el hijo `Body`. El campo `completedSprite` es opcional (dibujo al completarse). El hijo `Done` es la marca de "completado".
- **Indicador "[E]"**: los hijos `Prompt` son texto; se pueden cambiar por un SpriteRenderer con el sprite.
- **Módulo de aterrizaje:** `Map/Lander`.

## 6. Centro espacial (`01_Hub` → `Room`)
Piso, paredes, pantalla, consolas y escritorio son cuadrados en `Room`. Se reemplazan por sprites o por un Tilemap con el tileset de interior. La puerta (`Door/Sprite`) tiene en `HubDoor` un campo `openSprite`.

## 7. Audio → `Assets/_Project/Resources/AudioLibrary.asset`
Hay una entrada por sonido (`Click, Select, Error, Launch, Scan, ScanFail, SiteComplete, SafeMode, DustStorm, CommsLost, CommsBack, Dialogue, DoorOpen, Success`) y por música (`Title, Hub, Workshop, Mars, Report`). Solo hay que arrastrar el clip y ajustar el volumen. El código ya los llama; si un clip falta, no pasa nada.

## 8. Textos
- **Diálogos:** `Assets/_Project/Data/Dialogues/*.asset` (líneas con quién habla y el texto).
- **Datos NASA de los puntos:** en cada `Site_*`, campos `fact` y `factSource`.
- **Descripciones de los componentes:** `description` y `nasaFact` en cada `ComponentData`.
- **Frases del reporte:** `Scripts/Model/ReportBuilder.cs`.
