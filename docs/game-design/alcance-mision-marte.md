# Alcance: Mars Crafts, solo la misión de Marte

> La Tierra y la Luna aparecen en el menú como misiones bloqueadas ("próximamente") y se mencionan en el pitch como parte del diseño completo.

## Flujo del juego (escenas)
1. Título: "Presiona para empezar" y menú de misiones (Tierra y Luna bloqueadas, Marte disponible).
2. Centro espacial: una sola sala top-down con la Dra. Vega y BIMO. Diálogo de briefing con la pregunta científica y una puerta al taller.
3. Taller (constructor): pantalla de UI para elegir componentes y lanzador. Vista previa del rover por capas. Verificación con avisos de BIMO.
4. Lanzamiento y viaje: animación corta con cuenta regresiva y la trayectoria Tierra–Marte como línea animada. Texto: el viaje dura unos 7 meses.
5. Aterrizaje (OPCIONAL): minijuego de descenso.
6. Exploración de Marte: mapa top-down con el rover, tres puntos científicos y uno o dos eventos.
7. Transmisión y reporte: qué se logró, qué falló, qué decisión lo causó, comparación con Perseverance, y botones Rediseñar o Terminar.

## Constructor: categorías y efecto en la exploración
Ver valores en `sistema-recursos-y-assets.md`. Cada decisión del taller debe notarse en Marte:
- Energía: paneles (la tormenta de polvo corta la recarga) o RTG (estable, caro y pesado).
- Batería: energía máxima del rover.
- Instrumentos: cada punto científico exige uno (cámara, espectrómetro, brazo con taladro).
- Comunicaciones: duración del apagón.
- Ruedas: el todoterreno cruza la zona rocosa (atajo).
- Lanzador: capacidad de masa frente a costo.

Verificación antes del lanzamiento: costo ≤ presupuesto, masa ≤ capacidad del lanzador, energía generada ≥ consumo base. Si algo falla, BIMO explica qué y sugiere qué cambiar. Ninguna configuración debe ser perfecta.

## Exploración de Marte
- Un mapa de unos 40×30 tiles, inspirado en Jezero: delta, cauce seco, zona rocosa y depósitos sedimentarios.
- Rover: se mueve en 4 direcciones. Moverse y escanear gastan energía.
- Tres puntos científicos:
  1. Delta / cauce antiguo: requiere cámara.
  2. Afloramiento mineral: requiere espectrómetro.
  3. Roca especial (el descubrimiento final): requiere brazo con taladro.
- Eventos (máximo 2): tormenta de polvo y pérdida de comunicación.
- En cada punto, un popup de BIMO con un dato real breve y su fuente NASA.

## Reporte final
- Puntos completados (0 a 3); presupuesto, masa, energía y comunicaciones usados frente a disponibles.
- Cada problema relacionado con su decisión. Ejemplo: "Elegiste paneles solares: la tormenta de polvo dejó al rover sin energía".
- Comparación con Perseverance (verificar cada dato con la fuente).
- Calificación, insignia y botón Rediseñar (vuelve al taller con el diseño anterior cargado).

## Trabajo por bloque
- **Integración + exploración (Danilo):** escenas y transiciones, mapa con Tilemap, rover, puntos científicos, eventos, cámara.
- **UI en Unity (segunda persona con Unity):** constructor, HUD, popups de BIMO, cuadro de diálogo y reporte, como prefabs.
- **Modelo en C# puro:** `RoverDesign`, `Verifier`, `MissionState`, `ReportBuilder`.
- **Contenido y datos:** valores y balance, diálogos, datos con fuente, textos del reporte, verificación.
- **Arte y audio:** ver la lista de assets.
- **Pitch, entrega y QA.**

## Contratos que hay que fijar primero
- Tamaño de tile y PPU.
- Formato de los datos de componentes (`ComponentData`).
- La interfaz pública del modelo.
- Nombres de las escenas y orden del flujo.

## Cronograma
- **Sábado hasta las 14:00:** contratos, flujo navegable con escenas vacías, mapa en greybox.
- **Sábado 14:00–20:00:** cada uno en su bloque.
- **Sábado ~20:00:** primera build de principio a fin con placeholders.
- **Sábado noche:** arte, modelo real y textos.
- **Domingo mañana:** eventos, reporte, balance, sonido, pulido.
- **Domingo 15:00:** congelamiento de features.
- **Domingo tarde:** build final, video, ensayo del pitch, entrega.

## Orden de recorte si falta tiempo
1. Minijuego de aterrizaje.
2. Segundo evento.
3. Categoría de ruedas.
4. Centro espacial caminable (reemplazar por diálogo + botón).
5. Comparación detallada con Perseverance.

No se recorta: constructor con verificación, exploración con tres puntos y reporte que explica las decisiones.
