# Guion Técnico y Descripción Segundo a Segundo — Tráiler Oficial de *Mars Crafts* (Edición V4)

> **Duración total:** `45.0 segundos` exactos (`1350 frames` @ `30 FPS`, resolución `1920×1080` Full HD)  
> **Tecnología:** Remotion + React + TypeScript (`trailer-remotion/`)  
> **Frase conceptual:** *«Cada gran descubrimiento comienza con una decisión.»*

![Vista previa de los 12 planos del tráiler](./TRAILER_STORYBOARD_PREVIEW.png)

---

## Cómo abrir, previsualizar y editar el tráiler

```bash
cd trailer-remotion
npm install
node node_modules/esbuild/install.js
npm start
```

- Se abrirá **Remotion Studio** en `http://localhost:3000`.
- Cada escena es un componente independiente dentro de `src/scenes/`:
  - `Scene01_UniverseAwaits.tsx` (`00:00 – 00:04`)
  - `Scene02_MissionBegins.tsx` (`00:04 – 00:09`)
  - `Scene03_DesignYourMission.tsx` (`00:09 – 00:15`)
  - `Scene04_TheLaunch.tsx` (`00:15 – 00:22`)
  - `Scene05_ExploreMars.tsx` (`00:22 – 00:28`)
  - `Scene06_MoonSecret.tsx` (`00:28 – 00:33`)
  - `Scene07_ControlRescue.tsx` (`00:33 – 00:40`)
  - `Scene08_GrandFinale.tsx` (`00:40 – 00:45`)
- Para exportar el video `.mp4` final en `out/MarsCrafts_Official_Trailer_1080p.mp4`:
  ```bash
  npm run render
  ```

---

## Descripción Detallada de lo que Ocurre en el Video (Segundo a Segundo)

### **ESCENA 1 — El Universo y las 3 Misiones (`00:00 – 00:04` | Frames `0 – 120`)**
- **Archivo:** `src/scenes/Scene01_UniverseAwaits.tsx`
- **Toma 1A (`00:00 – 00:02.5`):**
  - **Visual:** Fondo espacial del juego (`space_background.png`) con un resplandor solar a la izquierda. Se ven los tres planetas del juego alineados: **Tierra** (`planet_earth.png` a la izquierda), **Luna** (`planet_moon.png` al centro-arriba) y **Marte** (`planet_mars.png` a la derecha). Una línea punteada cian traza la trayectoria orbital y la nave (`cruise_spacecraft.png`) viaja de la Tierra hacia Marte mientras la cámara hace un zoom lento.
  - **Texto inferior:** Recuadro oscuro con el subtítulo *«NASA SPACE APPS CHALLENGE • TRES EXPEDICIONES CIENTÍFICAS»* y la frase *«Cada gran descubrimiento comienza con una decisión.»*
- **Toma 1B (`00:02.5 – 00:04.0`):**
  - **Visual:** Flash azulado de transición. Sobre el espacio aparecen en fila horizontal las **3 tarjetas oficiales de misión del juego**:
    1. **Misión 1 // Órbita LEO:** *Guardianes de la Tierra* (`mission_earth.png` — *Satélite Terra • Clima y Océanos*).
    2. **Misión 2 // Polo Sur (-240 °C):** *El Secreto de la Luna* (`mission_moon.png` — *LRO & VIPER • Hielo en Sombra*).
    3. **Misión 3 // Cráter Jezero:** *Marte: Huellas del Pasado* (`mission_mars.png` — *Perseverance • Antiguo Delta*).

---

### **ESCENA 2 — El Centro Espacial (`01_Hub`) (`00:04 – 00:09` | Frames `120 – 270`)**
- **Archivo:** `src/scenes/Scene02_MissionBegins.tsx`
- **Visual (plano continuo con paneo de cámara de izquierda a derecha sobre el escenario real del juego):**
  - Piso metálico del Hub (`floor_panels.png`) con un pasillo central de rejilla (`floor_grate.png`) y pared superior con paneles y dos puertas/esclusas (`esclusa-cian.png` y `esclusa-naranja.png`).
  - **Sector izquierdo:** El reactor cian (`reactor-cian.png`) pulsando con energía, un servidor, tanques gemelos y el ingeniero **Mateo Rojas** parado al lado.
  - **Sector central:** Consolas de control e investigación, mesa de laboratorio y la **Dra. Elena Vega** en el centro.
  - **Sector derecho:** Estación de diagnóstico, cultivo hidropónico, consola logística y **Luna Torres**.
  - **Acción:** El **Comandante (protagonista)** y **BIMO** caminan de izquierda a derecha por el pasillo de rejilla desde la zona del reactor hacia la Dra. Vega y la puerta del Taller.
- **Texto superior izquierdo:** Cartel *«01_HUB // CENTRO ESPACIAL MARS CRAFTS (REACTOR • CONTROL • TALLER)»*.
- **Cuadro de diálogo inferior:**
  - **Mitad 1 (`00:04 – 00:06.5`):** Retrato de la **Dra. Elena Vega**: *«Bienvenido al Centro Espacial. Estudiaremos la Tierra con el satélite Terra, el hielo polar de la Luna y el cráter Jezero en Marte.»*
  - **Mitad 2 (`00:06.5 – 00:09.0`):** Retrato de **BIMO**: *«¡Reactor en línea y esclusa abierta, Comandante! Vamos al Taller a ensamblar nuestro satélite y el rover.»*

---

### **ESCENA 3 — Diseña tu Misión (`02_Workshop`) (`00:09 – 00:15` | Frames `270 – 450`)**
- **Archivo:** `src/scenes/Scene03_DesignYourMission.tsx`
- **Visual:**
  - Fondo real del Taller del juego (`workshop_background.png`).
  - En el centro de la pantalla está el **rover de perfil** (`rover_side.png`) sobre un anillo holográfico azul, mientras un haz láser vertical azul lo escanea de izquierda a derecha.
  - A los lados del rover están **Mateo Rojas** (izquierda) y **BIMO** (derecha).
  - Flotando arriba a la izquierda y derecha del rover hay dos recuadros pequeños: uno con el **Satélite Terra** (*Misión 1 // Órbita*) y otro con el **Lanzador Pesado** (*Capacidad 110 kg*).
- **Texto superior izquierdo:** Sello grande amarillo **«DISEÑA»** con el subtítulo *«02_WORKSHOP // DECISIONES DE INGENIERÍA Y RECURSOS»*.
- **Panel superior derecho:** Indicadores que van subiendo dinámicamente:
  - *Presupuesto:* pasa de `95` → `115` → `130` → `145 / 160 M$`.
  - *Energía:* `+2.5/s`.
  - *Masa del Rover:* pasa de `68` → `84` → `94` → `106 / 110 kg`.
  - En el último segundo aparece el botón verde *«✓ ¡LANZAR MISIÓN!»*.
- **Franja inferior:** 4 tarjetas horizontales con íconos reales del juego que se van iluminando una por una cada ~1.2 segundos (con un destello en pantalla):
  1. **Energía:** *Generador RTG* (`+2.5 E/s constante en tormenta y sombra`).
  2. **Locomoción:** *Ruedas Todoterreno* (`Cruza zonas rocosas y cráteres sin daño`).
  3. **Comunicación:** *Relé Vía Orbitador* (`Mantiene señal en cráter lunar y tormenta`).
  4. **Instrumentos + Lanzador:** *Taladro + Espectrómetro + Pesado* (`Capacidad 110 kg • 3/3 instrumentos listos`).

---

### **ESCENA 4 — El Lanzamiento y Misión 1: La Tierra (`00:15 – 00:22` | Frames `450 – 660`)**
- **Archivo:** `src/scenes/Scene04_TheLaunch.tsx`
- **Toma 4A (`00:15 – 00:16.8` — Primer plano de ignición):**
  - **Visual:** Ilustración pixel art en contrapicado extremo de las **3 toberas del cohete encendiéndose** (`rocket_engine_ignition_closeup.png`) con fuego blanco-naranja, humo, líneas de velocidad verticales y fuerte vibración de cámara.
  - **Texto superior izquierdo:** *«03_LAUNCH // IGNICIÓN DE MOTORES — ¡EMPUJE AL 108 % — FUEGO EN PLATAFORMA!»*.
- **Toma 4B (`00:16.8 – 00:19.3` — Despegue en el escenario del juego):**
  - **Visual:** Corte con flash blanco al escenario real de lanzamiento (`launchpad_background.png`). El cohete del juego (`launch_rocket.png`) asciende verticalmente con su llama animada (`rocket_flame`) mientras nubes de humo (`launch_smoke`) se expanden en la base.
  - **Texto superior izquierdo:** *«03_LAUNCH // ASCENSO DESDE PLATAFORMA — ¡DESPEGUE RUMBO AL SISTEMA SOLAR!»*.
- **Toma 4C (`00:19.3 – 00:22.0` — Misión 1: Guardianes de la Tierra en órbita):**
  - **Visual:** Corte al espacio (`space_background.png`) con el planeta Tierra gigante abajo (`planet_earth.png`). El sprite del **Satélite Terra** (`earth_satellite_sprite.png`) se desplaza por la parte superior proyectando un cono de luz cian hacia la Tierra. Arriba a la derecha hay un recuadro pequeño (Picture-in-Picture) con la ilustración del satélite saliendo de la cofia del cohete sobre un huracán (`earth_mission_satellite_deploy.png`).
  - **Texto superior izquierdo:** *«MISIÓN 1 // GUARDIANES DE LA TIERRA (SATÉLITE NASA TERRA) — ESCANEO ORBITAL DE CLIMA, OCÉANOS Y VEGETACIÓN»*.
  - **Recuadro inferior:** *«MISIÓN 1 COMPLETADA — OBSERVATORIO TERRESTRE NASA TERRA (LEO): Datos atmosféricos y oceánicos transmitidos al Centro Espacial. ¡Misión 2 (La Luna) desbloqueada! | DATOS TERRA: 3 / 3»*.

---

### **ESCENA 5 — Exploración de Marte (`04_Mars`) (`00:22 – 00:28` | Frames `660 – 840`)**
- **Archivo:** `src/scenes/Scene05_ExploreMars.tsx`
- **Visual (escenario real de Marte con seguimiento de cámara):**
  - Suelo rojizo del juego (`dirt_a.png`), franja diagonal de arena del cauce/delta (`sand_a.png`) y zona rocosa abajo (`rocky_ground.png`).
  - Alrededor hay mesetas y acantilados del juego (`meseta-grande.png`, `cantil-largo.png`, `meseta-alta.png`, `aguja-rocosa.png`), cráteres, rocas y cristales.
  - A la izquierda está el módulo de aterrizaje (`landing_module.png`).
  - Hay **3 puntos científicos** en el mapa: el primero ya tiene el check verde (`science_completed.png`), el segundo (`science_point_special_rock.png`) activa el anillo azul de escaneo (`scan_000..003.png`) cuando el rover pasa junto a él, y el tercero muestra el ícono `[E]` parpadeando.
  - El **rover del juego** (`drive_east_000..003.png`) conduce de izquierda a derecha. A mitad de la escena (`00:25.5`) entra una **tormenta de polvo** (`mars_dust_texture.png` moviéndose por pantalla).
- **Texto superior izquierdo:** Sello naranja **«EXPLORA»** con el subtítulo *«04_MARS // CRÁTER JEZERO, DELTA FLUVIAL Y TORMENTA DE POLVO»*.
- **HUD superior derecho:** *«RTG 92 % | 2 / 3 PUNTOS | ⚠ TORMENTA DE POLVO»*.
- **Recuadro inferior:** *«DATO CIENTÍFICO NASA — DELTA FLUVIAL DEL CRÁTER JEZERO: Perseverance confirmó que el Cráter Jezero albergó un antiguo lago y un delta hace más de 3.500 millones de años.»*

---

### **ESCENA 6 — El Secreto de la Luna (`04_Moon`) (`00:28 – 00:33` | Frames `840 – 990`)**
- **Archivo:** `src/scenes/Scene06_MoonSecret.tsx`
- **Visual (escenario real de la Luna con seguimiento de cámara):**
  - Suelo gris iluminado a la izquierda (`MoonTilesPNG/dirt_a.png`) con el módulo lunar (`landing_module_moon.png`) y rocas lunares (`rock_big`, `rock_spike`).
  - A la derecha hay un gran óvalo oscuro con borde punteado azul que representa el **cráter en sombra permanente (`-240 °C`)**.
  - El **rover** entra en la zona oscura proyectando un cono de luz azul (faros) hacia el punto de hielo (`science_point_ice.png`) y activa el anillo de escaneo.
  - A mitad de la escena (`00:30.2`) aparece arriba a la derecha un **recuadro pequeño (Picture-in-Picture)** con la ilustración del rover perforando un géiser de hielo azul (`moon_lava_tube_ice_drill.png`) con el texto *«¡TALADRO PERCUTOR: HIELO DE AGUA (H₂O) DETECTADO!»*.
- **Texto superior izquierdo:** Sello azul **«DESCUBRE»** con el subtítulo *«04_MOON // SOMBRA PERMANENTE (-240 °C) Y HIELO POLAR»*.
- **HUD superior derecho:** *«RELÉ ORBITAL ACTIVO | ❄ FRÍO EXTREMO -2.0 E/s»*.
- **Recuadro inferior:** *«HALLAZGO LUNAR — DEPÓSITOS DE HIELO EN CRÁTER CABEUS (NASA LRO / LCROSS / VIPER): En las regiones polares en sombra permanente se conserva hielo de agua clave para futuras bases espaciales.»*

---

### **ESCENA 7 — Crisis y Rescate desde el Centro de Control (`00:33 – 00:40` | Frames `990 – 1200`)**
- **Archivo:** `src/scenes/Scene07_ControlRescue.tsx`
- **Toma 7A (`00:33 – 00:36.0` — Avería del rover en la tormenta):**
  - **Visual:** Ilustración dramática a pantalla completa del **rover inclinado en la tormenta de arena marciana** (`rover_sandstorm_crisis.png`), con luz roja de emergencia y chispas eléctricas saltando de su antena doblada, líneas de velocidad rojas diagonales y sacudida de cámara.
  - **Texto superior izquierdo:** *«¡ALERTA: SIN SEÑAL Y PANELES A 0 E/s!»*.
  - **Texto superior derecho:** Sello rojo **«RESUELVE»** (*«¡CRISIS EN MARTE: TORMENTA Y APAGÓN!»*).
  - **Diálogo inferior (retrato de BIMO en alerta):** *«¡Comandante! El polvo bloqueó los paneles y saltan chispas de la antena. ¡Activando Modo Seguro!»*
- **Toma 7B (`00:36.0 – 00:40.0` — Rescate en pantalla dividida):**
  - **Visual (pantalla dividida 50/50):**
    - **Mitad izquierda (`01_Hub`):** Piso del Centro de Control con las consolas, el reactor y **Luna Torres**, **Mateo Rojas** y la **Dra. Vega**, más un recuadro abajo con la ilustración de la **antena parabólica gigante de 70m** (`dsn_mars_yard_rescue.png`) disparando un haz láser cian al cielo nocturno.
    - **Mitad derecha (`04_Mars`):** Suelo marciano donde el rover aparece rodeado por un círculo verde (escudo de Modo Seguro) y termina de escanear el punto científico con el check verde.
  - **Texto superior izquierdo:** *«ENLACE DSN 70m + RESERVA DE MODO SEGURO (+0.8 E/s)»*.
  - **Texto superior derecho:** Sello verde **«RESUELVE»** (*«01_HUB ↔ 04_MARS // ¡RESCATE EN MODO SEGURO!»*).
  - **Diálogo inferior (retrato de Luna Torres):** *«¡Red de Espacio Profundo conectada! El rover recupera energía con su reserva y completa el escaneo 3/3.»*

---

### **ESCENA 8 — Gran Final (`00:40 – 00:45` | Frames `1200 – 1350`)**
- **Archivo:** `src/scenes/Scene08_GrandFinale.tsx`
- **Visual:**
  - Fondo panorámico del atardecer en los cañones de Marte (`finale_commander_mars_horizon.png`) con la Tierra y la Luna brillando arriba en el cielo.
  - En la esquina inferior izquierda está el sprite del **rover**; en la esquina inferior derecha están los sprites de la **Dra. Elena Vega**, el **Comandante** y **BIMO**.
  - En el centro de la pantalla aparecen animados en orden:
    1. **3 estrellas doradas** (`star_full.png`).
    2. El **logo oficial de Mars Crafts** (`mars_crafts_logo.png`).
    3. El texto **«DISEÑA • EXPLORA • DESCUBRE • RESUELVE»**.
    4. Un recuadro con borde amarillo: **«Cada gran descubrimiento comienza con una decisión.»**
    5. **3 recuadros de insignias** en fila:
       - *MISIÓN 1: TIERRA — Satélite Terra (LEO)*
       - *MISIÓN 2: LUNA — Hielo Polar (-240 °C)*
       - *MISIÓN 3: MARTE — Comandante Jezero*

---

## Lámina de Recursos Adicionales Generados (`public/cinematic/` y `unity-expansion-pack/`)

![Lámina de 9 recursos generados con PixelLab](./CINEMATIC_PIXELLAB_PREVIEW.png)
