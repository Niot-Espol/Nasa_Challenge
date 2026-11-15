# Mars Crafts: lista de arte para dibujar

> Lista para el equipo de arte. Marca `[x]` cuando esté entregado y pon tu nombre en **Quién** para no duplicar trabajo.
> **Regla para simplificar:** nada se dibuja por capas ni en variantes según el equipamiento. El rover es **un solo dibujo**, lleve lo que lleve; los componentes del taller son **íconos sueltos**.

**Prioridad:** **P1** = imprescindible para la demo · **P2** = importante · **P3** = solo si sobra tiempo.
Hacer primero todo lo P1 (meta: sábado por la noche), después P2 y por último P3.

> **Estado (domingo 15, mañana):** ✅ todo el arte de Marte (personajes, retratos, tilesets, rover, objetos, íconos, lanzamiento e interfaz) está **dentro del juego**.
> **Nuevo: segunda misión, la Luna (sección 11).** Es lo que hay que generar ahora. Ver "Qué falta" al final.

---

## 0. Reglas técnicas (léelas antes de empezar)

| Regla | Valor |
|---|---|
| Estilo | Pixel art estilo **Game Boy Advance**: contorno oscuro limpio, paleta limitada, vista **top-down baja** (como el astronauta que ya existe). |
| Escala | **1 tile = 16×16 px.** Todo el arte del mundo se mide en múltiplos de 16. |
| Formato | **PNG** con fondo **transparente**, sin antialiasing ni bordes difuminados. |
| Personajes | Lienzo de **32×32 px** por frame, con el personaje centrado y **los pies tocando el borde inferior**. |
| Direcciones | Solo **abajo, arriba y derecha**. La izquierda la hace el juego volteando el dibujo de la derecha, así que **no la dibujes**. |
| Animaciones | Un PNG por frame, todos del mismo tamaño y numerados: `nombre_000.png`, `nombre_001.png`… |
| Pantallas completas | **320×180 px** (el juego lo escala ×6 a 1920×1080 sin deformar). |
| Paleta de referencia | Los colores del astronauta protagonista: blanco y naranja industrial, visor azul oscuro. Marte: ocres, rojos óxido y marrones. |

**Entrega:** súbelo al repo en `Assets/_Project/Art/` (en la subcarpeta indicada en cada sección) o pásaselo a Danilo en un **.zip**. Si lo subes desde la web de GitHub, recuerda que solo acepta **100 archivos por subida**: mejor un zip.

---

## 1. Personajes (`Art/Sprites/Characters/`)

| ✓ | Prio | Asset | Tamaño | Frames | Notas | Quién |
|---|---|---|---|---|---|---|
| [x] | P1 | **Protagonista** (AstronautStocky) | 32×32 | quieto + correr en 3 dir × 8 | **Ya está hecho**, es la referencia de estilo. | — |
| [x] | P1 | **Dra. Elena Vega**, quieta | 32×32 (idle 44×44) | 9 frames + 8 orientaciones | **En el juego** (Hub, con animación de reposo). | Equipo |
| [x] | P1 | **BIMO**, quieto | 32×32 | 4 frames | **En el juego** (Hub). | PixelLab / Codex |
| [x] | P2 | **BIMO**, moviéndose | 32×32 | 4 frames × 3 dir (abajo, arriba, derecha) | Para que siga al jugador en el centro espacial. | PixelLab / Codex |
| [x] | P3 | Mateo Rojas, quieto | 32×32 | 2 frames | Entregado; no aparece en la misión actual. | PixelLab / Codex |
| [x] | P3 | Luna Torres, quieta | 32×32 | 2 frames | Entregado; no aparece en la misión actual. | PixelLab / Codex |

## 2. Retratos para diálogos (`Art/Sprites/Portraits/`)

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [x] | P1 | Retratos de **Dra. Vega** × 3 | 128×128 | Neutral, contenta, preocupada. **En el juego** (diálogos con expresión). | Equipo |
| [x] | P1 | Retratos de **BIMO** × 3 | 64×64 | Neutral, alegre, alarmado. **En el juego** (alarmado en los avisos de emergencia). | PixelLab / Codex |
| [x] | P3 | Retratos de Mateo y Luna (1 c/u) | 64×64 | Neutral. | PixelLab / Codex |

## 3. Rover (`Art/Sprites/Rover/`)

> **Un solo diseño de rover para todo el juego.** No cambia si lleva paneles, RTG o instrumentos.

| ✓ | Prio | Asset | Tamaño | Frames | Notas | Quién |
|---|---|---|---|---|---|---|
| [x] | P1 | **Rover de conducción** (DrivingV2) | 80×64 | 3 direcciones × 4 frames | **En el juego** (Marte). Sustituye al rover provisional. | PixelLab / Codex |
| [x] | P1 | **Rover de perfil** (vista previa del taller) | 96×96 | 1 | **En el juego** (Taller). | PixelLab / Codex |

## 4. Mapa de Marte: tileset (`Art/Tilesets/Mars/`)

> ✅ **Entregado y en el juego:** `Art/Tilesets/Mars/MarsTileset.png` (cuadrícula de 22 px, pixel art real). Suelos sin costuras, rocas, pedruscos, cristales y cráteres.

| ✓ | Prio | Asset | Notas | Quién |
|---|---|---|---|---|
| [x] | P1 | **Suelo de arena** + 3–4 variaciones | Arena ondulada con 4 variantes. | Equipo |
| [x] | P1 | **Rocas sueltas** (obstáculos) | ~28 rocas y 9 grupos de pedruscos. | Equipo |
| [x] | P1 | **Zona rocosa** | Textura de cascajo + rocas encima. | Equipo |
| [~] | P1 | **Cauce seco** con bordes | Hecho con barro seco agrietado. **Faltan los bordes/esquinas** (transición arena ↔ barro): hoy el borde es escalonado. P2. | |
| [~] | P1 | **Delta** (depósitos en abanico) | Igual que el cauce: funciona, sin bordes suaves. | |
| [x] | P1 | **Borde del cráter** | Resuelto con doble fila de rocas. | Equipo |
| [ ] | P2 | Depósitos sedimentarios | Rocas en capas horizontales (estratos). | |

## 5. Objetos del mapa de Marte (`Art/Sprites/Mars/`)

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [x] | P1 | **Punto científico 1: delta** | 32×32 | Sedimentos en capas. **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Punto científico 2: afloramiento mineral** | 32×32 | Roca roja con vetas. **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Punto científico 3: roca especial** | 32×32 | Roca erosionada con espiral. **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Marca de "punto completado"** | 16×16 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Indicador de interacción** | 16×16, 2 frames | **En el juego** (Marte y Hub). | PixelLab / Codex |
| [x] | P1 | **Módulo de aterrizaje** | 48×48 | **En el juego**, junto al rover. | PixelLab / Codex |
| [x] | P2 | Efecto de escaneo | 32×32, 4 frames | **En el juego**, sobre el punto mientras se escanea. | PixelLab / Codex |
| [x] | P2 | Partícula de polvo de la tormenta | 64×64, repetible | **En el juego** (capas de la tormenta). | PixelLab / Codex |

## 6. Centro espacial: tileset de interior (`Art/Tilesets/Hub/`)

> ✅ **Entregado y en el juego:** `Art/Tilesets/Hub/LabTileset.png` (34 piezas, pixel art real).

| ✓ | Prio | Asset | Notas | Quién |
|---|---|---|---|---|
| [x] | P1 | **Piso** + 1–2 variaciones | Placas con remaches (+ paneles y rejilla). | Equipo |
| [x] | P1 | **Paredes** con bordes y esquinas | Paneles con cables, pantallas, barandas y columnas. | Equipo |
| [x] | P1 | **Puerta al taller** | Esclusa cerrada y abierta. | Equipo |
| [x] | P1 | **Consolas de control** | 5 variantes (se quitaron de la sala para no saturar). | Equipo |
| [x] | P1 | **Pantalla grande** | Con Marte. | Equipo |
| [x] | P1 | **Escritorio** | 2 escritorios. | Equipo |
| [x] | P2 | Decoración | Plantas (en uso), pósters, estantes y casilleros. | Equipo |

## 7. Taller: íconos de componentes (`Art/Sprites/Icons/Components/`)

> Arte generado y comprobado técnicamente; revisión visual e integración en Unity pendientes.

> **Íconos sueltos de 32×32**, todos con el mismo encuadre y estilo. Son botones de la interfaz y **no se pegan sobre el rover**.

| ✓ | Prio | Ícono | Quién |
|---|---|---|---|
| [x] | P1 | Paneles solares | |
| [x] | P1 | RTG (generador nuclear: cilindro con aletas) | |
| [x] | P1 | Batería pequeña | |
| [x] | P1 | Batería grande | |
| [x] | P1 | Cámara | |
| [x] | P1 | Espectrómetro | |
| [x] | P1 | Brazo con taladro | |
| [x] | P1 | Antena directa (parabólica) | |
| [x] | P1 | Relé vía orbitador (satélite pequeño) | |
| [x] | P1 | Ruedas estándar | |
| [x] | P1 | Ruedas todoterreno | |
| [x] | P1 | Lanzador ligero (cohete pequeño) | |
| [x] | P1 | Lanzador medio | |
| [x] | P1 | Lanzador pesado (cohete grande) | |
| [x] | P2 | Fondo del taller (320×180) | |

## 8. Íconos de recursos (`Art/Sprites/Icons/Resources/`)

> Arte generado y comprobado técnicamente; revisión visual e integración en Unity pendientes.

16×16, para las barras y el HUD.

| ✓ | Prio | Ícono | Quién |
|---|---|---|---|
| [x] | P1 | Presupuesto (moneda o créditos) | |
| [x] | P1 | Masa (pesa o kg) | |
| [x] | P1 | Energía (rayo) | |
| [x] | P1 | Tiempo / soles (sol o reloj) | |
| [x] | P1 | Comunicaciones (antena con ondas) | |
| [x] | P1 | Datos científicos (lupa o disco) | |
| [x] | P2 | Modo seguro (escudo o batería vacía) | |
| [x] | P2 | Sin señal (antena tachada) | |

## 9. Lanzamiento y viaje (`Art/Sprites/Launch/`)

> **En el juego** (15 nov).

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [x] | P1 | **Cohete** | 32×64 | **Uno solo**, genérico, de perfil. | |
| [x] | P1 | **Llama del motor** | 16×32, 3 frames | Va debajo del cohete. | |
| [x] | P1 | **Humo** | 32×32, 3–4 frames | Nubes de humo en el despegue. | |
| [x] | P1 | **Fondo de la plataforma de lanzamiento** | 320×180 | Cielo, plataforma y torre. | |
| [x] | P1 | **Tierra** | 64×64 | Planeta visto desde el espacio. | |
| [x] | P1 | **Marte** | 64×64 | Planeta visto desde el espacio. | |
| [x] | P1 | **Nave en crucero** | 32×32 | La nave viajando entre planetas. | |
| [x] | P1 | **Fondo espacial** | 320×180 | Estrellas, repetible. | |

## 10. Interfaz (`Art/UI/`)

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [x] | P1 | **Marco de panel** | 48×48 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Caja de diálogo** | 48×48 (9-slice) | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Botón** | 48×16 (9-slice) × 3 estados | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Barra** (marco + relleno) | 64×8 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Popup de dato NASA** | 48×48 (9-slice) | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Estrella** llena y vacía | 16×16 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Insignia Comandante Mars Crafts** | 64×64 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Candado** | 16×16 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Tarjetas de misión**: Tierra, Luna y Marte | 64×64 c/u | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Logo "Mars Crafts"** | ~192×64 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | **Fondo de la pantalla de título** | 320×180 | **En el juego.** | PixelLab / Codex |
| [x] | P1 | Ícono de **aviso de BIMO** | 16×16 | **En el juego.** | PixelLab / Codex |
| [x] | P2 | Insignias "Eficiente" y "Sin modo seguro" | 32×32 | **En el juego.** | PixelLab / Codex |

> **Fuente:** no se dibuja. Se usará una fuente pixel gratuita (por ejemplo, de Google Fonts o itch.io con licencia libre).

---

## 11. Misión 2: la Luna (`Art/Sprites/Moon/`)

> **Contexto:** "El secreto de la Luna" (historia 1.65). El rover explora el **polo sur lunar** buscando **hielo de agua** (misiones de referencia: LRO y LCROSS).
> Se juega como Marte (Taller → lanzamiento → 3 puntos → reporte), pero el mapa tiene **zonas iluminadas y zonas en sombra permanente** (frío, sin Sol) y un **cráter donde se pierde la señal**.
> **Qué NO hay que dibujar:** rover, personajes, retratos, Hub, Taller, íconos, UI, cohete, humo, nave ni Tierra (se reutilizan).
> El suelo, las rocas y los cráteres los puede sacar Danilo/IA **recoloreando a gris** el arte de Marte; el suelo propio (P2) solo mejora el resultado.
> Las **sombras** se hacen por código: **no las dibujes** dentro de los sprites.

**Paleta Luna:** grises neutros a gris azulado (regolito), negro azulado para lo que está en sombra, blanco frío para lo iluminado y **azul hielo / cian** solo para el hielo (que sea lo único con color fuerte del mapa).

| ✓ | Prio | Asset | Tamaño | Notas | Carpeta | Quién |
|---|---|---|---|---|---|---|
| [ ] | **P1** | **Depósito de hielo** (punto científico estrella) | 48×48 | Cristales azul-blanco **semienterrados** en regolito oscuro, con algún destello. Es el clímax de la misión: tiene que verse especial incluso en la sombra. | `Moon/SciencePoints/science_point_ice.png` | |
| [ ] | **P1** | **Roca del borde iluminado** (punto científico) | 32×32 | Afloramiento de roca clara (anortosita, casi blanca) con una cara muy iluminada. | `Moon/SciencePoints/science_point_rim_rock.png` | |
| [ ] | **P1** | **Muestra de regolito** (punto científico) | 32×32 | Parche de polvo gris fino con pequeñas esferas de vidrio brillantes (vidrio de impacto). Distinto a las rocas normales. | `Moon/SciencePoints/science_point_regolith.png` | |
| [ ] | **P1** | **Luna para el viaje** | 64×64 | Igual que `planet_earth`/`planet_mars`: gris con cráteres, vista desde el espacio. | `Launch/Planets/planet_moon.png` | |
| [ ] | P2 | **Módulo de aterrizaje lunar** | 48×48 | Distinto al de Marte: patas largas, tanques dorados (estilo Apolo o lander comercial). | `Moon/Landing/landing_module_moon.png` | |
| [ ] | P2 | **Suelo lunar** (3 variantes + 1 liso) | tiles 16×16 | Regolito gris fino, con microcráteres. Repetible sin costuras. Si no llega, se recolorea el de Marte. | `Tilesets/Moon/` | |
| [ ] | P2 | **Cráteres lunares grandes** | 64×64 y 96×96 | Borde elevado claro y fondo más oscuro (sin sombra proyectada: la pone el juego). | `Moon/Craters/` | |
| [ ] | P3 | **Pantalla grande del Hub con la Luna** | igual que la pantalla de Marte del laboratorio | Mismo marco, mostrando la Luna y el polo sur marcado. Para el briefing de Vega. | `Hub/` | |
| [ ] | P3 | **Rocas lunares** (3–4) | 16–32 px | Grises, más angulosas que las de Marte. Si no llegan, se recolorean las de Marte. | `Moon/Rocks/` | |

**Mínimo imprescindible para la demo:** las 4 piezas **P1**. Con eso la misión ya se ve distinta a Marte en el video.

---

## Qué falta (en orden de impacto)

| # | Bloque | Piezas | Por qué importa |
|---|---|---|---|
| 1 | **Luna: 3 puntos científicos + Luna del viaje** (sección 11, P1) | 4 | Sin ellos la segunda misión parece Marte en gris. |
| 2 | **Luna: módulo de aterrizaje y suelo** (sección 11, P2) | 2 bloques | Más identidad visual propia. |
| 3 | Luna: cráteres, pantalla del Hub, rocas (P2/P3) | — | Pulido. |
| 4 | Bordes de cauce/delta (Marte) | P2 | Pulido. |

**Audio:** no entra en esta lista. Los efectos son P1 y la música P2, y se buscarán en bancos libres.

**Dudas sobre estilo, tamaños o prioridades:** pregunta a Danilo antes de empezar una pieza grande.
