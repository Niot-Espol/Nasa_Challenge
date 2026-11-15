# Mars Crafts: diseños para armar terrenos

Recursos existentes entregados al equipo el 14 de noviembre de 2026. Esta carpeta contiene fuentes de arte, mapas editables y revisiones HTML; todavía no está conectada a las escenas de Unity del repositorio.

## Por dónde empezar

1. Descarga o clona el repositorio completo, conservando esta estructura de carpetas.
2. Abre `catalogo.html` para consultar las 182 piezas y sus versiones PNG/Aseprite.
3. Abre `index.html` para revisar el exterior y los interiores; `editor-mapa.html` permite componer un mapa y guardar/exportar los cambios.
4. Para pintar terreno, revisa primero `elementos/terreno/`, `elementos/rocas/`, `elementos/relieve/`, `elementos/crateres/` y `tiles32/`.
5. `mapa-base.tmx` se abre en Tiled; conserva junto a él `terreno.tsx`, `elementos.tsx`, `elementos/` y `tiles32/` para que funcionen las rutas relativas.

## Contenido

- `elementos/`: 182 piezas PNG transparentes con sus 182 editables Aseprite, organizadas por categoría. Incluye terreno, rocas, cráteres, relieve, pavimento, edificios, energía, estructuras, vehículos y equipamiento.
- `tiles32/`: 49 imágenes de 32 × 32 px para pintar el mapa.
- `catalogo.json`: dimensiones nativas, categoría y rutas de cada pieza. Los objetos grandes ocupan varias celdas; no todos miden 32 × 32.
- `mapa-base.*`: exterior de referencia de 48 × 32 celdas (1536 × 1024 px), en PNG, TMX, Aseprite y JSON editable.
- `interiores/`: laboratorio, hábitat, invernadero, taller y módulo vacío; cada mapa incluye PNG, TMX, Aseprite, JSON y revisión HTML.
- `fuentes/`: hojas originales, versiones con transparencia y fuentes de los muebles.
- `SHA256SUMS.txt`: hashes SHA-256 de los archivos entregados, incluida esta guía.

## Escala e integración en Unity

La biblioteca original usa una cuadrícula de **32 × 32 px**. El proyecto Unity compartido documenta tiles de **16 × 16 px** y **16 píxeles por unidad**. No son la misma escala: importar un tile de 32 px a 16 PPU produce un sprite de 2 unidades de ancho. El integrador debe revisar la relación entre sprites, Tilemap y personajes antes de conectarlos. Los originales se entregan sin reducir ni redibujar.

Usar Sprite, filtro Point y sin compresión según las reglas del repositorio. Importar mediante Unity los archivos elegidos en `Assets/_Project/Art/` para que genere sus `.meta`; esta carpeta es una entrega de fuentes fuera de `Assets/` y no cambia escenas, prefabs ni configuración.

El mapa compuesto sirve como referencia y biblioteca: no reemplaza automáticamente `04_Mars`. Al integrar, conservar las zonas de juego, los tres puntos científicos y sus posiciones. Las colisiones de Tiled son aproximaciones; deben revisarse en Unity. No dar por completados los tiles específicos de cauce, delta, transiciones u otros pendientes solo por recibir esta biblioteca.

Conservar los personajes y sus animaciones ya elegidos. Los tres ZIP originales de astronautas siguen en la carpeta superior `Elementos visuales/`.

## Comprobación

Los archivos copiados de la biblioteca se verificaron contra sus originales mediante SHA-256. Se conservan sus rutas relativas. No se ha probado esta entrega integrada en Play ni en un build WebGL; la integración corresponde al siguiente trabajo del equipo.

Consulta `LEEME.md` para detalles del editor y la procedencia de los recursos. Esta entrega no establece una licencia nueva para las hojas aportadas.