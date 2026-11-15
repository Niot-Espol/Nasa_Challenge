# Dra. Elena Vega — generaciones de PixelLab

Recursos aportados por el usuario y organizados el 15 de noviembre de 2026. Los PNG y el ZIP se conservan byte por byte, con sus tamaños y transparencia originales.

## Retratos

Ruta: `Assets/_Project/Art/Sprites/Portraits/DrElenaVega/`.

| Archivo | Expresión | Tamaño |
|---|---|---|
| `vega-portrait-neutral.png` | Neutral | 128 × 128 |
| `vega-portrait-happy.png` | Feliz | 128 × 128 |
| `vega-portrait-worried.png` | Preocupada | 128 × 128 |

La asignación se hizo según la expresión visible: sonrisa para feliz y gesto de inquietud para preocupada. Son retratos de 128 × 128, no versiones reducidas a 64 × 64.

## Sprite de personaje

Ruta: `Assets/_Project/Art/Sprites/Characters/DrElenaVega/`.

- `Rotations/vega-idle-<dirección>.png`: ocho orientaciones de 32 × 32, conservadas completas.
- `Idle/South/vega-idle-south_000.png` a `_008.png`: nueve fotogramas de respiración mirando al sur, de **44 × 44**. PixelLab exportó el idle con un lienzo mayor que las orientaciones de 32 × 32; se conserva tal como fue aportado, sin recortar ni reescalar.
- No hay animación de caminar/correr en este ZIP. No se añadieron ni se descartaron frames.
- No hay FPS confirmado en los metadatos originales; no se creó un clip con una velocidad supuesta.

Cada PNG y carpeta nuevos dentro de `Assets/` incluye `.meta` con GUID propio. Se usa Sprite Single, lienzo completo, filtro Point, sin compresión ni mipmaps; pivote inferior central para el personaje y central para retratos. Los PNG se conservan intactos. Los personajes siguen la configuración efectiva de `PixelArtImporter.cs` del repositorio (16 PPU en esta revisión); no se cambió ese importador ni la configuración del proyecto.

## Fuentes y trazabilidad

Esta carpeta conserva `DrElenaVega-PixelLab-Original.zip` y `pixellab-metadata.original.json`. Los nombres y rutas originales de PixelLab siguen intactos dentro del ZIP. `manifest.json` relaciona cada archivo de origen con su ruta nueva y hash SHA-256.

## Integración pendiente

Esta entrega guarda recursos; no sustituye el sprite de Vega en escenas, ni crea Animator, clips o campos de expresión. El integrador puede asignar el retrato neutral a `UITheme.portraitVega`, ampliar el sistema para las otras expresiones y crear el idle usando los nueve fotogramas. Antes de crear clips, comprobar la alineación de pies y el encuadre entre orientaciones de 32 × 32 e idle de 44 × 44: un pivote inferior central no garantiza por sí solo que lienzos distintos coincidan visualmente. Revisar `01_Hub` en Play después de conectarlos. No se ejecutó Unity ni un build WebGL como parte de esta entrega.
