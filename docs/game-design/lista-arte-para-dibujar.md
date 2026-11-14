# Mars Crafts: lista de arte para dibujar

> Lista para el equipo de arte. Marca `[x]` cuando esté entregado y pon tu nombre en **Quién** para no duplicar trabajo.
> **Regla para simplificar:** nada se dibuja por capas ni en variantes según el equipamiento. El rover es **un solo dibujo**, lleve lo que lleve; los componentes del taller son **íconos sueltos**.

**Prioridad:** **P1** = imprescindible para la demo · **P2** = importante · **P3** = solo si sobra tiempo.
Hacer primero todo lo P1 (meta: sábado por la noche), después P2 y por último P3.

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
| [ ] | P1 | **Dra. Elena Vega**, quieta | 32×32 | 2–4 frames mirando abajo | Directora científica. Bata o uniforme del centro espacial. Respiración leve. | |
| [ ] | P1 | **BIMO**, quieto | 32×32 | 2–4 frames | Robot asistente pequeño y simpático, flotando o balanceándose. | |
| [ ] | P2 | **BIMO**, moviéndose | 32×32 | 4 frames × 3 dir (abajo, arriba, derecha) | Para que siga al jugador en el centro espacial. | |
| [ ] | P3 | Mateo Rojas, quieto | 32×32 | 2 frames | Ingeniero del taller. | |
| [ ] | P3 | Luna Torres, quieta | 32×32 | 2 frames | Especialista de operaciones. | |

## 2. Retratos para diálogos (`Art/Sprites/Portraits/`)

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [ ] | P1 | Retratos de **Dra. Vega** × 3 | 64×64 | Expresiones: **neutral, contenta, preocupada**. Busto, fondo transparente. | |
| [ ] | P1 | Retratos de **BIMO** × 3 | 64×64 | Expresiones: **neutral, alegre, alarmado** (para cuando avisa de un problema). | |
| [ ] | P3 | Retratos de Mateo y Luna (1 c/u) | 64×64 | Neutral. | |

## 3. Rover (`Art/Sprites/Rover/`)

> **Un solo diseño de rover para todo el juego.** No cambia si lleva paneles, RTG o instrumentos.

| ✓ | Prio | Asset | Tamaño | Frames | Notas | Quién |
|---|---|---|---|---|---|---|
| [ ] | P1 | **Rover top-down** | 32×32 | 3 dir (abajo, arriba, derecha) × 2 frames | Visto desde arriba. Los 2 frames son para que las ruedas parezcan moverse. Inspirado en Perseverance pero simplificado. | |
| [ ] | P1 | **Rover de perfil** (vista previa del taller) | 96×96 | 1 | Dibujo grande y bonito del mismo rover, de lado. Es lo que se ve en el taller. | |

## 4. Mapa de Marte: tileset (`Art/Tilesets/Mars/`)

> Tiles de **16×16** en **una sola imagen**, en cuadrícula, sin espacios (o con el mismo espacio entre todos). Incluye una **leyenda** (aunque sea un texto: "fila 1 = arena…") para que Danilo sepa qué es cada tile.

| ✓ | Prio | Asset | Notas | Quién |
|---|---|---|---|---|
| [ ] | P1 | **Suelo de arena** + 3–4 variaciones | Variaciones sutiles (piedritas, grietas) para que el suelo no se vea repetido. | |
| [ ] | P1 | **Rocas sueltas** (obstáculos) | 3–4 rocas de 16×16 y 1–2 grandes de 32×32. Fondo transparente. | |
| [ ] | P1 | **Zona rocosa** | Terreno quebrado por donde solo pasa el rover con ruedas todoterreno. Debe distinguirse claramente del suelo normal. | |
| [ ] | P1 | **Cauce seco** con bordes | Antiguo río seco. Tiles de centro + bordes + esquinas (los 9 de un recuadro: 4 esquinas, 4 lados y el centro). | |
| [ ] | P1 | **Delta** (depósitos en abanico) | Zona de sedimentos en capas donde terminaba el río. Tonos más claros o estratificados. | |
| [ ] | P1 | **Borde del cráter** | Pared o elevación que limita el mapa. Centro + bordes + esquinas. | |
| [ ] | P2 | Depósitos sedimentarios | Rocas en capas horizontales (estratos). | |

## 5. Objetos del mapa de Marte (`Art/Sprites/Mars/`)

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [ ] | P1 | **Punto científico 1: delta** | 32×32 | Formación del antiguo delta. Tiene que verse "interesante" y distinto de lo demás. | |
| [ ] | P1 | **Punto científico 2: afloramiento mineral** | 32×32 | Roca con vetas o cristales de color. | |
| [ ] | P1 | **Punto científico 3: roca especial** | 32×32 | El gran descubrimiento final. La más llamativa de las tres. | |
| [ ] | P1 | **Marca de "punto completado"** | 16×16 | Banderita o check que aparece encima al terminar un punto. | |
| [ ] | P1 | **Indicador de interacción** | 16×16, 2 frames | Tecla **"E"** flotando que rebota, sobre lo que se puede usar. | |
| [ ] | P1 | **Módulo de aterrizaje** | 48×48 | Donde empieza el rover. Decorativo. | |
| [ ] | P2 | Efecto de escaneo | 32×32, 4 frames | Onda o rayo cuando el rover analiza un punto. | |
| [ ] | P2 | Partícula de polvo de la tormenta | 64×64, repetible | Textura de polvo que se pueda repetir en mosaico sobre la pantalla. | |

## 6. Centro espacial: tileset de interior (`Art/Tilesets/Hub/`)

| ✓ | Prio | Asset | Notas | Quién |
|---|---|---|---|---|
| [ ] | P1 | **Piso** + 1–2 variaciones | Tiles 16×16. | |
| [ ] | P1 | **Paredes** con bordes y esquinas | Vista top-down baja (se ve la cara frontal de la pared de arriba). | |
| [ ] | P1 | **Puerta al taller** | 32×32 (o 16×32). Cerrada y abierta. | |
| [ ] | P1 | **Consolas de control** | 2–3 variantes, 32×32. | |
| [ ] | P1 | **Pantalla grande** | 64×32, con Marte en la pantalla. | |
| [ ] | P1 | **Escritorio** | 32×16 o 32×32. | |
| [ ] | P2 | Decoración | Plantas, cajas, póster de cohete, estanterías. | |

## 7. Taller: íconos de componentes (`Art/Sprites/Icons/Components/`)

> **Íconos sueltos de 32×32**, todos con el mismo encuadre y estilo. Son botones de la interfaz y **no se pegan sobre el rover**.

| ✓ | Prio | Ícono | Quién |
|---|---|---|---|
| [ ] | P1 | Paneles solares | |
| [ ] | P1 | RTG (generador nuclear: cilindro con aletas) | |
| [ ] | P1 | Batería pequeña | |
| [ ] | P1 | Batería grande | |
| [ ] | P1 | Cámara | |
| [ ] | P1 | Espectrómetro | |
| [ ] | P1 | Brazo con taladro | |
| [ ] | P1 | Antena directa (parabólica) | |
| [ ] | P1 | Relé vía orbitador (satélite pequeño) | |
| [ ] | P1 | Ruedas estándar | |
| [ ] | P1 | Ruedas todoterreno | |
| [ ] | P1 | Lanzador ligero (cohete pequeño) | |
| [ ] | P1 | Lanzador medio | |
| [ ] | P1 | Lanzador pesado (cohete grande) | |
| [ ] | P2 | Fondo del taller (320×180) | |

## 8. Íconos de recursos (`Art/Sprites/Icons/Resources/`)

16×16, para las barras y el HUD.

| ✓ | Prio | Ícono | Quién |
|---|---|---|---|
| [ ] | P1 | Presupuesto (moneda o créditos) | |
| [ ] | P1 | Masa (pesa o kg) | |
| [ ] | P1 | Energía (rayo) | |
| [ ] | P1 | Tiempo / soles (sol o reloj) | |
| [ ] | P1 | Comunicaciones (antena con ondas) | |
| [ ] | P1 | Datos científicos (lupa o disco) | |
| [ ] | P2 | Modo seguro (escudo o batería vacía) | |
| [ ] | P2 | Sin señal (antena tachada) | |

## 9. Lanzamiento y viaje (`Art/Sprites/Launch/`)

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [ ] | P1 | **Cohete** | 32×64 | **Uno solo**, genérico, de perfil. | |
| [ ] | P1 | **Llama del motor** | 16×32, 3 frames | Va debajo del cohete. | |
| [ ] | P1 | **Humo** | 32×32, 3–4 frames | Nubes de humo en el despegue. | |
| [ ] | P1 | **Fondo de la plataforma de lanzamiento** | 320×180 | Cielo, plataforma y torre. | |
| [ ] | P1 | **Tierra** | 64×64 | Planeta visto desde el espacio. | |
| [ ] | P1 | **Marte** | 64×64 | Planeta visto desde el espacio. | |
| [ ] | P1 | **Nave en crucero** | 32×32 | La nave viajando entre planetas. | |
| [ ] | P1 | **Fondo espacial** | 320×180 | Estrellas, repetible. | |

## 10. Interfaz (`Art/UI/`)

| ✓ | Prio | Asset | Tamaño | Notas | Quién |
|---|---|---|---|---|---|
| [ ] | P1 | **Marco de panel** | 48×48 | 9-slice: esquinas de 16×16 que se estiran. Estilo consola de nave. | |
| [ ] | P1 | **Caja de diálogo** | 48×48 (9-slice) | Con espacio a la izquierda para el retrato. | |
| [ ] | P1 | **Botón** | 48×16 (9-slice) × 3 estados | Normal, encima (hover) y presionado. | |
| [ ] | P1 | **Barra** (marco + relleno) | 64×8 | Para energía y tiempo. El relleno es un color plano que se estira. | |
| [ ] | P1 | **Popup de dato NASA** | 48×48 (9-slice) | Para los datos que da BIMO. Diferente del diálogo (más "pantalla científica"). | |
| [ ] | P1 | **Estrella** llena y vacía | 16×16 | Calificación del reporte (0–3). | |
| [ ] | P1 | **Insignia Comandante Mars Crafts** | 64×64 | Premio final. | |
| [ ] | P1 | **Candado** | 16×16 | Para las misiones bloqueadas (Tierra, Luna). | |
| [ ] | P1 | **Tarjetas de misión**: Tierra, Luna y Marte | 64×64 c/u | Para el menú del título. | |
| [ ] | P1 | **Logo "Mars Crafts"** | ~192×64 | Para la pantalla de título. | |
| [ ] | P1 | **Fondo de la pantalla de título** | 320×180 | Paisaje marciano o el centro espacial. | |
| [ ] | P1 | Ícono de **aviso de BIMO** | 16×16 | Signo de alerta para cuando algo no cumple en el taller. | |
| [ ] | P2 | Insignias "Eficiente" y "Sin modo seguro" | 32×32 | Premios extra del reporte. | |

> **Fuente:** no se dibuja. Se usará una fuente pixel gratuita (por ejemplo, de Google Fonts o itch.io con licencia libre).

---

## Resumen de cantidades (solo P1)

| Bloque | Piezas aprox. |
|---|---|
| Personajes + retratos | 2 personajes animados + 6 retratos |
| Rover | 6 frames top-down + 1 vista de perfil |
| Tileset de Marte | ~25–35 tiles |
| Objetos de Marte | 6 piezas |
| Tileset del centro espacial | ~20–25 tiles y objetos |
| Íconos | 14 componentes + 6 recursos |
| Lanzamiento | 8 piezas |
| Interfaz | ~13 piezas |

**Audio:** no entra en esta lista. Los efectos son P1 y la música P2, y se buscarán en bancos libres.

**Dudas sobre estilo, tamaños o prioridades:** pregunta a Danilo antes de empezar una pieza grande.
