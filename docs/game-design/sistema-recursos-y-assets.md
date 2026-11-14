# Sistema de recursos y lista de assets — Misión Marte

> Los números son un punto de partida para ajustar jugando, no valores finales.

## Principios de diseño
1. Pocos recursos, con un papel claro cada uno. Presupuesto y masa son límites duros del taller. Energía y tiempo se juegan en la exploración. Comunicaciones se nota en un evento.
2. El acoplamiento masa → lanzador → presupuesto es el dilema central.
3. Ninguna opción es mejor en todo.
4. Se balancea al revés: primero se definen los diseños objetivo con sus debilidades y después se ajustan los números.
5. Fallar es parcial, nunca un callejón sin salida (modo seguro en vez de game over).
6. Realismo en las proporciones, no en las cifras. Unidades de juego.

## Recursos
| Recurso | Dónde se juega | Regla |
|---|---|---|
| Presupuesto | Taller | Costo total ≤ 110 créditos |
| Masa | Taller | Masa total ≤ capacidad del lanzador |
| Energía | Exploración | Batería = máximo. Moverse y escanear gastan. La fuente recarga. En 0, modo seguro. |
| Tiempo (soles) | Exploración | Duración fija (por ejemplo, 4 minutos reales). El modo seguro y los apagones consumen tiempo. |
| Comunicaciones | Exploración | Durante el apagón no se puede escanear ni transmitir |
| Datos científicos | Resultado | 1 por punto completado (máximo 3) |

## Componentes (valores iniciales)
Chasis base (siempre incluido): 250 kg, 15 créditos.

| Categoría | Opción | Masa | Costo | Efecto |
|---|---|---|---|---|
| Lanzador | Ligero | — | 15 | Capacidad 500 kg |
| Lanzador | Medio | — | 25 | Capacidad 700 kg |
| Lanzador | Pesado | — | 40 | Capacidad 900 kg |
| Energía | Paneles solares | 40 | 10 | Recarga 3/s; 0 durante la tormenta |
| Energía | RTG | 120 | 30 | Recarga 1,5/s siempre |
| Batería | Pequeña | 30 | 5 | Capacidad 60 |
| Batería | Grande | 60 | 12 | Capacidad 120 |
| Instrumento | Cámara | 15 | 8 | Punto 1. Escanear cuesta 10 |
| Instrumento | Espectrómetro | 40 | 18 | Punto 2. Escanear cuesta 25 |
| Instrumento | Brazo con taladro | 70 | 22 | Punto 3. Escanear cuesta 35 |
| Comunicaciones | Antena directa | 25 | 6 | Apagón de 40 s |
| Comunicaciones | Relé vía orbitador | 10 | 15 | Apagón de 10 s |
| Ruedas | Estándar | 0 | 0 | No cruza la zona rocosa |
| Ruedas | Todoterreno | 40 | 10 | Cruza la zona rocosa |

Moverse: 1 de energía por segundo.

### Diseños objetivo
- Ciencia completa, energía frágil: paneles + batería pequeña + 3 instrumentos + antena directa + ligero = 470 kg, 99 créditos. ✓
- Ciencia completa, batería grande: 500 kg, 106 créditos. ✓
- Ciencia completa con relé: 455 kg, 108 créditos. ✓
- Confiable: RTG + batería pequeña + cámara + espectrómetro + antena directa + ligero = 480 kg, 97 créditos. ✓ (2/3)
- Lo quiero todo: RTG + 3 instrumentos → 550 kg o más, lanzador medio, 129 créditos o más. ✗ (a propósito)
- Revisar casos que fallan por 1 o 2 créditos (RTG + cámara + brazo = 111).

## Eventos
- Tormenta de polvo: una vez, unos 30 s. Con paneles, la recarga baja a 0.
- Pérdida de comunicación: una vez. La duración depende de la antena.
- Modo seguro: energía en 0 → el rover se detiene hasta recargar 30 %.

## Puntuación
- 1 estrella por punto científico; insignias "Eficiente" (sobran 10 o más créditos) y "Sin modo seguro".

## Datos NASA (VERIFICAR con fuente)
- Spirit y Opportunity usaban paneles solares; la misión de Opportunity terminó tras la tormenta de polvo global de 2018.
- Curiosity y Perseverance usan un MMRTG.
- Perseverance (unos 1.025 kg) se lanzó en un Atlas V.
- La mayoría de los datos se envían a través de orbitadores de relé como MRO.
- Instrumentos de Perseverance: Mastcam-Z, SuperCam, PIXL, SHERLOC y un brazo con taladro de núcleos.

## Lista de assets
Especificaciones: tiles de 16×16, personajes de 16×32, rover top-down de 32×32, vista previa del taller a 96×96 (lateral). P1 = imprescindible, P2 = importante, P3 = si sobra tiempo.

- **Personajes:** P1 protagonista (idle + caminar 4 dir), BIMO (idle), Dra. Vega (idle), retratos de Vega y BIMO (3 expresiones cada uno). P2 BIMO siguiendo. P3 retratos de Mateo y Luna.
- **Rover y taller:** P1 vista previa por capas (chasis, paneles, RTG, cámara, espectrómetro, brazo, antena directa, relé, ruedas estándar/todoterreno), rover top-down 4 dir, ~13 íconos de componentes, 3 lanzadores, 6 íconos de recursos. P2 variante solar/RTG top-down, fondo del taller.
- **Tilesets:** P1 interior del centro (piso, paredes, puerta, consolas, pantalla, escritorio), Marte (suelo, arena, rocas, zona rocosa, delta, cauce, borde del cráter), módulo de aterrizaje.
- **Objetos y efectos:** P1 tres puntos científicos distinguibles, indicador de interacción. P2 escaneo, brazo, tormenta, estática, modo seguro.
- **Lanzamiento:** P1 cohete, llama, humo, fondo, Tierra, Marte, nave en crucero. P3 cohete por lanzador y aterrizaje.
- **UI:** P1 marcos, diálogo, botones, barras, HUD, popup de dato, reporte con estrellas, insignia Comandante, menú con candados, logo, fuente pixel libre. P2 insignias extra.
- **Audio:** P1 efectos. P2 dos pistas de música.
