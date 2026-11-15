# Mars Crafts · Misión Marte: lo que está implementado

> Resumen de la misión que **ya funciona en Unity**, para que el equipo vea el flujo, la organización y el contenido, y pueda diseñar otras misiones a partir de ella (con este flujo o con otro).
> Estado: jugable de principio a fin. Arte de terreno integrado; UI, retratos y audio todavía provisionales.

---

## 1. La misión en una frase

El jugador **diseña un rover con recursos limitados, lo lanza a Marte, explora una región inspirada en el cráter Jezero y recibe un reporte que explica qué decisión de diseño causó cada resultado.**

- **Pregunta científica:** ¿qué podemos aprender sobre los antiguos ambientes con agua de Marte estudiando sus rocas?
- **Dilema central:** ciencia completa con energía frágil (paneles solares) **o** energía confiable con menos ciencia (RTG). El acoplamiento masa → lanzador → presupuesto obliga a elegir. **Ninguna configuración es perfecta.**
- **Duración:** ~6–8 minutos por partida.

---

## 2. Flujo de pantallas

```
Título → Centro espacial (Hub) → Taller → Lanzamiento → Marte → Reporte
                                    ↑                              │
                                    └──────── Rediseñar ───────────┘
                                                    Terminar → Título
```

| # | Pantalla | Qué hace el jugador | Qué aprende |
|---|---|---|---|
| 0 | **Título** | Elige misión: Marte disponible; Tierra y Luna aparecen bloqueadas ("Próximamente"). | Que el programa tiene más misiones (visión del juego completo). |
| 1 | **Centro espacial** | Camina (WASD) por una sala, habla con la Dra. Vega (briefing) y con BIMO (E). La puerta al taller solo se abre tras el briefing. | El objetivo científico y las reglas del taller. |
| 2 | **Taller** | Elige un componente por categoría. Ve en vivo presupuesto, masa y avisos de BIMO. Solo puede lanzar si el diseño es válido. | Restricciones de ingeniería: presupuesto, masa, energía, compromisos. |
| 3 | **Lanzamiento** | Mira la cuenta regresiva, el despegue y el viaje Tierra → Marte (~7 meses). Puede saltarlo. | El segmento de lanzamiento y la duración del viaje. |
| 4 | **Marte** | Maneja el rover top-down, gestiona energía y tiempo, escanea 3 puntos científicos (E), sobrevive a una tormenta y a un apagón. | Cómo cada decisión del taller se nota en la operación. |
| 5 | **Reporte** | Ve estrellas, insignias, recursos usados, **qué pasó y por qué**, y la comparación con Perseverance. Puede **Rediseñar** (vuelve al taller con su diseño cargado) o **Terminar**. | Causa → efecto de sus decisiones; iterar como en el diseño real. |

Extras: menú de pausa (Esc), transiciones con fundido.

---

## 3. Personajes

| Personaje | Rol en la misión | Estado del arte |
|---|---|---|
| **Comandante** (jugador) | Astronauta ingeniero; camina en el Hub. | Sprite y animaciones listos (AstronautStocky). |
| **Dra. Elena Vega** | Directora científica; da el briefing. | Provisional (astronauta "Agile"). |
| **BIMO** | Robot asistente; da consejos, avisa errores en el taller y datos NASA en Marte. | Provisional (cuadrado cian). |
| Mateo Rojas, Luna Torres | Aparecen en la historia, no en la misión implementada. | — |

---

## 4. Guion (textos que ya están en el juego)

### Centro espacial: briefing de la Dra. Vega
1. **Vega:** Bienvenido a Mars Crafts, comandante. Durante años hemos estudiado la Tierra, la Luna y Marte, pero aún quedan muchas preguntas.
2. **Vega:** Hoy empieza nuestra misión más importante: investigar una región inspirada en el cráter Jezero, donde la NASA encontró evidencias de un antiguo lago y un delta.
3. **Vega:** Nuestra pregunta científica: ¿qué podemos aprender sobre los antiguos ambientes con agua de Marte estudiando sus rocas?
4. **Vega:** Allá estudiarás tres puntos: el delta, un afloramiento mineral y una roca especial. Cada uno necesita un instrumento distinto.
5. **Vega:** En el taller diseñarás tu rover. Tienes 110 créditos, y cada componente suma masa: tu cohete solo puede llevar cierto peso.
6. **BIMO:** ¡Y yo te ayudaré, comandante! Mi último diagnóstico dice que funciono casi perfectamente.
7. **Vega:** Cuando estés listo, entra al taller por la puerta del fondo. Suerte.

*Si se le vuelve a hablar:* "Recuerda: ninguna configuración es perfecta. Piensa qué estás dispuesto a sacrificar: ciencia, energía o presupuesto."

### Centro espacial: BIMO
1. Entonces... ¿viajaremos millones de kilómetros para recoger piedras?
2. ¡Ah, ya sé! Las rocas pueden guardar la historia de un planeta. ¡Las piedras más importantes de mi carrera espacial!
3. Un consejo: los paneles solares son ligeros y baratos... pero en Marte hay tormentas de polvo. Piénsalo bien.

*Puerta bloqueada:* "¡Espera, comandante! Primero habla con la Dra. Vega para recibir el briefing de la misión."

### Marte: llegada (BIMO)
1. ¡Aterrizamos en Jezero, comandante! Hace miles de millones de años aquí hubo un lago.
2. Esta es nuestra energía. Moverse y escanear la gastan, y nuestra fuente la recarga. Si llega a cero, el rover entra en modo seguro. *(la barra de energía late)*
3. Tenemos solo dos minutos y medio de operación antes de que se cierre la ventana de la misión. Planifica la ruta. *(late la barra de tiempo)*
4. Hay tres puntos científicos. Las flechas en el borde de la pantalla te indican dónde están. Acércate y presiona E para escanear. ¡Vamos!

> Todos los diálogos son assets editables en `Assets/_Project/Data/Dialogues/` (no hace falta programar para cambiarlos).

---

## 5. Taller: decisiones y reglas

**Reglas duras:** costo total ≤ **110 créditos** · masa total ≤ **capacidad del lanzador**.
**Chasis base** (siempre incluido): 250 kg, 15 créditos.

| Categoría | Opción | Masa | Costo | Efecto en Marte |
|---|---|---|---|---|
| Lanzador | Ligero | — | 15 | Lleva hasta 500 kg |
| | Medio | — | 25 | Lleva hasta 700 kg |
| | Pesado | — | 40 | Lleva hasta 900 kg |
| Energía | Paneles solares | 40 | 10 | Recarga 2,5/s; **0 durante la tormenta** |
| | RTG | 120 | 30 | Recarga 1,5/s siempre |
| Batería | Pequeña | 30 | 5 | Energía máxima 60 |
| | Grande | 60 | 12 | Energía máxima 120 |
| Instrumentos (varios) | Cámara | 15 | 8 | Habilita el delta · escanear cuesta 10 |
| | Espectrómetro | 40 | 18 | Habilita el afloramiento mineral · cuesta 25 |
| | Brazo con taladro | 70 | 22 | Habilita la roca especial · cuesta 35 |
| Comunicaciones | Antena directa | 25 | 6 | Apagón de 40 s |
| | Relé vía orbitador | 10 | 15 | Apagón de 10 s |
| Ruedas | Estándar | 0 | 0 | No cruzan la zona rocosa |
| | Todoterreno | 40 | 10 | Cruzan la zona rocosa (atajo) |

**Verificación antes de lanzar (BIMO):**
- *Bloquean:* falta lanzador / energía / batería / antena / ruedas; presupuesto excedido; masa mayor que la capacidad del lanzador. Cada uno con un mensaje que dice qué cambiar.
- *Avisan (se puede lanzar igual):* sin instrumentos, o menos de 3 ("habrá puntos que no podremos completar").

**Diseños de referencia (verificados en el juego):**

| Diseño | Masa | Costo | Resultado |
|---|---|---|---|
| Paneles + batería pequeña + 3 instrumentos + antena directa + ligero | 470 | 99 | ✅ Ciencia completa, energía frágil |
| Igual con batería grande | 500 | 106 | ✅ |
| Igual con relé | 455 | 108 | ✅ |
| RTG + batería pequeña + cámara + espectrómetro + antena + ligero | 480 | 97 | ✅ Confiable, solo 2/3 de ciencia |
| RTG + 3 instrumentos (lanzador medio) | 550 | 129 | ❌ Presupuesto (a propósito) |
| RTG + cámara + taladro | 510 | 101 | ❌ Masa; con lanzador medio se pasa por 1 crédito |

---

## 6. Exploración en Marte

**Mapa:** 40 × 30 unidades dentro de un cráter rodeado de rocas. El rover aterriza abajo a la izquierda. Los tres puntos están en las otras tres esquinas: el **delta** (barro seco agrietado al final de un cauce) arriba a la izquierda; el **afloramiento mineral** arriba a la derecha, detrás de una **cresta rocosa** que obliga a rodear; y la **roca especial** abajo a la derecha, protegida por una **zona rocosa** (atajo solo con todoterreno) y una **muralla** con un paso estrecho.

**Recursos en vivo:**
- **Energía:** empieza llena (según la batería). Moverse gasta **3 por segundo**; escanear gasta lo que cueste el instrumento; la fuente recarga continuamente.
- **Tiempo:** 2 minutos y medio (150 s).
- **Escanear** detiene al rover 2,5 s.
- **Sedimento blando** (delta y cauce): el rover avanza al 55 % de velocidad, salvo con ruedas todoterreno.
- **Modo seguro:** si la energía llega a 0, el rover se detiene hasta recargar el 30 %. No hay game over, pero se pierde tiempo.

**Tres puntos científicos** (acercarse + E). Si se tiene el instrumento, la energía alcanza y hay comunicación, se completa y BIMO muestra un **dato NASA con su fuente**:

| Punto | Instrumento | Dato que muestra |
|---|---|---|
| **Delta del antiguo río** | Cámara | Hace más de 3.500 millones de años, un río desembocaba en un lago dentro del cráter Jezero y formó un delta. Los deltas acumulan sedimentos en capas que pueden conservar señales de ambientes habitables. *(NASA · Mars 2020 Perseverance)* |
| **Afloramiento mineral** | Espectrómetro | Algunos minerales, como las arcillas y los carbonatos, se forman en presencia de agua; por eso cuentan cómo era el ambiente del pasado. Perseverance analiza su composición con SuperCam y PIXL. *(NASA · Perseverance Science Objectives)* |
| **Roca especial** (descubrimiento final) | Brazo con taladro | Perseverance extrae núcleos de roca y los guarda en tubos sellados. Las rocas sedimentarias pueden conservar información sobre el agua y quizá rastros de vida microbiana antigua... aunque aún no se ha confirmado. *(NASA · Mars 2020 Perseverance)* |

Si falta algo, BIMO lo explica ("No traemos el espectrómetro...", "Necesitamos 35 de energía...", "Sin comunicación no podemos escanear").

**Eventos:**
- **Tormenta de polvo** (segundo 35, dura 35 s): con paneles solares la recarga baja a 0.
- **Pérdida de comunicación** (segundo 75): dura 40 s con antena directa o 10 s con relé; durante el apagón no se puede escanear.
- **Zona rocosa:** con ruedas estándar es una pared (hay que rodearla); con todoterreno se cruza.

**Fin de la misión:** automático al completar los 3 puntos, al acabarse el tiempo, o con el botón "Terminar misión".

---

## 7. Reporte

- **Calificación:** 1 estrella por punto (0–3) y un titular ("¡Misión cumplida!", "Buena misión", "Misión parcial"...).
- **Insignias:** *Comandante Mars Crafts* (3 puntos), *Eficiente* (sobraron ≥10 créditos), *Sin modo seguro*.
- **Recursos:** presupuesto, masa, energía gastada y tiempo usado.
- **Qué pasó y por qué** (cada problema con la decisión que lo causó). Ejemplos reales del juego:
  - "Elegiste paneles solares: la tormenta de polvo cortó la recarga y el rover entró en modo seguro."
  - "No llevabas el brazo con taladro: no pudiste estudiar la roca especial."
  - "Con la antena directa perdiste la comunicación 40 s y no pudiste escanear. Un relé vía orbitador lo reduce a 10 s."
  - "Con ruedas estándar no pudiste cruzar la zona rocosa y tuviste que rodearla."
  - "Tu RTG siguió dando energía durante la tormenta de polvo."
- **Tu rover vs. Perseverance (NASA):** energía (Perseverance usa un MMRTG; Spirit y Opportunity usaban paneles), masa (~1.025 kg), instrumentos (Mastcam-Z, SuperCam, PIXL, SHERLOC y taladro) y comunicaciones (relé por orbitadores).
- **Rediseñar** vuelve al taller con el diseño anterior marcado → el jugador prueba otra estrategia.

**Resultados simulados** (para entender el balance):

| Diseño | Estrellas | Qué pasó |
|---|---|---|
| Paneles + batería pequeña + 3 instrumentos | ⭐⭐⭐ | La tormenta lo dejó en modo seguro (fragilidad) |
| RTG + 2 instrumentos | ⭐⭐ | Sin taladro no hizo la roca especial (menos ciencia) |
| Batería grande | ⭐⭐⭐ | Sin modo seguro |

---

## 8. Datos NASA que usa la misión

| Tema | Uso en el juego | Fuente |
|---|---|---|
| Proceso de diseño de misiones (6 etapas) y 4 segmentos | Estructura del flujo: necesidad → concepto → requisitos → diseño → verificación → implementación | NASA *Design and Develop Science Missions* |
| Cráter Jezero (lago y delta antiguos) | Escenario, delta y briefing | NASA Mars 2020 Perseverance |
| Objetivos de Perseverance (habitabilidad, muestras) | Puntos científicos y datos de BIMO | NASA Perseverance Science Objectives |
| Paneles (Spirit/Opportunity) vs. MMRTG (Curiosity/Perseverance); tormenta de 2018 | Dilema energético, tormenta, reporte | NASA Mars Exploration |
| Relé por orbitadores (p. ej. MRO) | Opción de comunicaciones | NASA Mars 2020 |
| Atmósfera, gravedad, polvo, viaje de ~7 meses | Textos y lanzamiento | NASA Mars Facts |

> ⚠️ Los datos deben verificarse con su enlace exacto antes del pitch. Los costos y masas del juego son **unidades de juego**, no especificaciones reales.

---

## 9. Cómo está organizado (para reutilizarlo en otras misiones)

La misión está separada en piezas; varias sirven tal cual para otra misión.

| Pieza | Qué es | ¿Reutilizable? |
|---|---|---|
| **Componentes** (`ScriptableObjects/Components/`) | Cada opción del taller es un asset con masa, costo y efecto. El taller genera sus tarjetas automáticamente. | ✅ Una misión nueva = otro set de componentes (p. ej. satélite: órbita, cámara, panel...). |
| **Verificador** (`Verifier`) | Reglas de presupuesto/masa/categorías con mensajes de BIMO. | ✅ Las reglas cambian poco entre misiones. |
| **Diálogos** (`Data/Dialogues/`) | Líneas con quién habla y el texto. | ✅ Solo escribir nuevos. |
| **Caja de diálogo / popup NASA** | Retrato, texto letra a letra, dato con fuente. | ✅ |
| **Estado de la misión** (`MissionState`) | Energía, tiempo, eventos, puntos científicos. | 🟡 Sirve para misiones de exploración (Luna); una de satélite necesitaría otra mecánica. |
| **Reporte** (`ReportBuilder`) | Estrellas, insignias y frases causa → decisión. | 🟡 El formato se reutiliza; las frases son de Marte. |
| **Mapa, rover, puntos científicos** | Escena de Marte. | 🟡 La Luna podría reutilizar casi todo con otro mapa. |
| **Tema visual y audio** (`UITheme`, `AudioLibrary`) | Toda la UI y los sonidos en dos assets. | ✅ |

---

## 10. Para diseñar otra misión: qué hay que definir

Usando Marte como plantilla, cada misión necesita (no tiene que seguir el mismo flujo):

1. **Pregunta científica** y misión NASA real que la inspira (p. ej. Tierra → Terra; Luna → LRO/LCROSS).
2. **Vehículo y decisiones de diseño:** categorías y 2–3 opciones por categoría, cada una con masa, costo y un efecto que **se note después**.
3. **Dilema central:** al menos dos estrategias válidas con debilidades distintas; ninguna perfecta.
4. **Fase de operación:** qué hace el jugador con su diseño (explorar, orbitar y fotografiar, aterrizar...).
5. **Objetivos científicos** (como los 3 puntos), cada uno ligado a un instrumento o decisión.
6. **Eventos** que pongan a prueba decisiones concretas (tormenta ↔ energía, apagón ↔ antena).
7. **Reporte:** frases "Elegiste X → pasó Y" y comparación con la misión real.
8. **Datos NASA** con su fuente para cada objetivo.

Ideas de la historia del equipo (versión 1.65) que ya están planteadas:
- **Misión 1 · Guardianes de la Tierra** (Terra): diseñar un satélite de observación, elegir órbita (altitud ↔ período), transmitir datos. Incluye reparar un generador en el centro espacial.
- **Misión 2 · El secreto de la Luna** (LRO, LCROSS): aterrizaje con minijuego de descenso, rover en el polo sur, zonas de luz y sombra, hielo de agua.
