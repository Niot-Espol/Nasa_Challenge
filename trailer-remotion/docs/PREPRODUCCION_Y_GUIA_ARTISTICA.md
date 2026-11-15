# Mars Crafts — Guía de Dirección Artística, Storyboard (45s) y Plan de Grabación

## 1. Guía de Dirección Artística Cinematográfica (16-Bit Pixel Art)

- **Estética base:** Pixel art nítido de 16 bits inspirado en *Eastward*, *Little-Known Galaxy*, *Stardew Valley* y clásicos de *Game Boy Advance*.
- **Escalado estricto:** `image-rendering: pixelated; image-rendering: crisp-edges;`. Prohibido usar ilustraciones vectoriales lisas, anime o estilos incompatibles.
- **Tipografías oficiales del juego:**
  - Títulos, palabras clave (`DISEÑA`, `EXPLORA`, `DESCUBRE`, `RESUELVE`) y telemetría: `PixelifySans.ttf`.
  - Diálogos de personajes y subtítulos explicativos: `Nunito.ttf`.
- **Paletas por entorno:**
  - **Espacio y Centro Espacial (`01_Hub` / `02_Workshop`):** `#06080E`, `#0B0E16`, `#141926`, `#2A3856`, cian tecnológico `#38E8FF`, dorado aeroespacial `#FFB300`.
  - **Marte (`04_Mars` — Cráter Jezero):** `#3B1712`, `#782C1E`, `#B94A2C`, `#E07A47`, `#F4B87A`.
  - **Luna (`04_Moon` — Polo Sur):** `#0B0E16`, `#262B36`, `#4E5562`, `#858B96`, `#C4C8CF`, `#EEF0F3` + hielo de agua `#3FA3D6`, `#8FE3F7`, `#E8FBFF`.

---

## 2. Storyboard Implementado (Exactamente 45.00 s · 1350 fotogramas @ 30 FPS)

| Escena | Tiempo | Fotogramas | Componente Remotion | Palabra Clave | Contenido Visual y Sonoro |
|---|---|---|---|---|---|
| **1. El Universo Nos Espera** | `00–04 s` | `0–119` (`120f`) | `Scene01_UniverseAwaits.tsx` | — | Campo estelar con parallax de 3 capas, amanecer solar sobre la Tierra (`planet_earth.png`), la Luna (`planet_moon.png`) y Marte (`planet_mars.png`), sonda en arco de transferencia y tesis: *«Cada gran descubrimiento comienza con una decisión.»* |
| **2. El Comienzo de la Misión** | `04–09 s` | `120–269` (`150f`) | `Scene02_MissionBegins.tsx` | — | El comandante (`AstronautStocky`) camina por el Centro Espacial hacia la pantalla holográfica con las 3 tarjetas de misión; lo reciben la Dra. Elena Vega, BIMO, Mateo Rojas y Luna Torres. Diálogo animado con retratos `128×128`. |
| **3. Diseña Tu Misión** | `09–15 s` | `270–449` (`180f`) | `Scene03_DesignYourMission.tsx` | **DISEÑA** | 3 cortes rápidos en el Taller (`02_Workshop`): comparación Paneles Solares vs RTG, alerta de exceso de masa de BIMO (`150/120 kg`) con el Lanzador Mediano y resolución al elegir el Lanzador Pesado (`150/170 kg`). |
| **4. El Lanzamiento** | `15–22 s` | `450–659` (`210f`) | `Scene04_TheLaunch.tsx` | — | Cuenta regresiva (`countdown_beep.wav`), encendido (`rocket_flame_000..002.png`), doble columna de humo expansivo (`launch_smoke_000..003.png`), vibración de cámara (`shakeIntensity=14`), ascenso a la estratosfera y crucero espacial (`cruise_spacecraft.png`). |
| **5. Exploración de Marte** | `22–28 s` | `660–839` (`180f`) | `Scene05_ExploreMars.tsx` | **EXPLORA** | El rover (`DrivingV2`) parte del módulo marciano en el Cráter Jezero, escanea el Delta del Río (`scan_000..003.png` + popup NASA Perseverance) y resiste la tormenta de polvo (`mars_dust_tile.png`) gracias al RTG. |
| **6. El Secreto de la Luna** | `28–33 s` | `840–989` (`150f`) | `Scene06_MoonSecret.tsx` | **DESCUBRE** | Contraste térmico hacia el Polo Sur Lunar: módulo con MLI dorado (`landing_module_moon.png`), regolito y rocas angulares, descenso al Cráter en Sombra Permanente (`-240 °C`) y hallazgo del depósito de hielo de agua (`science_point_ice.png`). |
| **7. Diagnóstico y Rescate** | `33–40 s` | `990–1199` (`210f`) | `Scene07_ControlRescue.tsx` | **RESUELVE** | Fase A: Rover en Modo Seguro y sin señal (`safe_mode.png` + `no_signal.png`), alerta de BIMO y Dra. Vega preocupada. Fase B: Diagnóstico causal en el Centro de Control / Reporte (`05_Report`) y conmutación a Relé Vía Orbitador + RTG. Fase C: Señal restablecida (`comms_back.wav`), 3 estrellas y medalla de Comandante. |
| **8. Cierre Cinematográfico** | `40–45 s` | `1200–1349` (`150f`) | `Scene08_GrandFinale.tsx` | **MARS CRAFTS** | El comandante, la Dra. Vega, BIMO y el rover frente al horizonte marciano observando la Tierra y la Luna; entrada con resorte del logo `mars_crafts_logo.png` y lema *«Diseña. Explora. Descubre.»* |

---

## 3. Nuevos Recursos Cinematográficos Propuestos para la Etapa 3 (PixelLab MCP)

Antes de gastar créditos adicionales de PixelLab, la Etapa 2 ya funciona al 100 % con los recursos del juego. Para elevar aún más las tomas cinematográficas en la **Etapa 3**, proponemos generar únicamente estas **4 piezas de alto impacto** usando los sprites actuales como `style_images`:

1. **`commander_visor_closeup.png` (`128×128` px):** Retrato cinematográfico del Comandante (`AstronautStocky`) con el reflejo del planeta Marte y la Luna en el visor dorado (para el contraplano de la Escena 2 en `07.5 s`).
2. **`launchpad_gantry_foreground.png` (`1920×1080` o tileable `320×180`):** Torre umbilical de lanzamiento en primer plano con brazos retráctiles para añadir una capa frontal de parallax en la Escena 4 (`15–19 s`).
3. **`rover_crisis_sand_trap.png` (`160×96` px):** Viñeta lateral/3/4 del rover semienterrado en duna marciana/cráter oscuro con chispa de antena y luz ámbar de Modo Seguro para el inicio dramático de la Escena 7 (`33.0–35.2 s`).
4. **`finale_mars_horizon_panorama.png` (`384×216` px escalado `5×` a `1920×1080`):** Panorama pixel art de 3 capas de parallax del horizonte del Cráter Jezero al atardecer con silueta de risco donde se sitúan el comandante, BIMO y el rover en la Escena 8 (`40–45 s`).

---

## 4. Lista Precisa de Grabaciones de Gameplay Real (Etapa 4)

Coloca los archivos MP4 capturados en `Mars_Crafts_Trailer/public/gameplay/` con estos nombres exactos:

| Prioridad | Archivo de video (`public/gameplay/`) | Escena Unity | Duración a grabar | Fragmento en el tráiler | Acción a realizar y encuadre |
|---|---|---|---|---|---|
| **P1** | `01_hub_briefing.mp4` | `01_Hub.unity` | `6 s` | Escena 2 (`06.8–08.8 s`, `2.0 s`) | Camina con el astronauta hacia la Dra. Elena Vega y BIMO frente a la pantalla central y pulsa `E` para abrir el diálogo del briefing (1920×1080, Game View Maximized). |
| **P1** | `02_workshop_design.mp4` | `02_Workshop.unity` | `12 s` | Escena 3 (`09.5–14.5 s`, 3 cortes rápidos) | Selecciona Generador RTG, Batería Grande, Ruedas Todoterreno, Relé Vía Orbitador y los 3 instrumentos; muestra el aviso de BIMO por masa y cambia al Lanzador Pesado para activar el botón *Lanzar*. |
| **P1** | `04_mars_exploration.mp4` | `04_Mars.unity` | `15 s` | Escena 5 (`22.5–27.8 s`, 3 cortes rápidos) | Conduce el rover hacia el Delta del Río, pulsa `E` para escanear (mostrando la onda de escaneo y el popup de NASA Perseverance) y conduce unos segundos bajo la tormenta de polvo (`t=45 s`). |
| **P1** | `04_moon_ice_crater.mp4` | `04_Moon.unity` | `12 s` | Escena 6 (`28.5–32.8 s`, 2 cortes rápidos) | Conduce desde la zona iluminada hacia el interior del cráter en sombra permanente (abajo a la izquierda) y escanea el depósito de hielo de agua azul brillante. |
| **P2** | `05_report_and_rescue.mp4` | `05_Report.unity` | `10 s` | Escena 7 (`35.2–39.2 s`, 2 cortes rápidos) | Muestra el Reporte de misión con las líneas de causalidad (*"Qué pasó y por qué"*) y la comparación con Perseverance/LRO, y haz clic en el botón **Rediseñar**. |
