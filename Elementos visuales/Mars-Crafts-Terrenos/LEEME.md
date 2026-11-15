# ARES · Base exterior editable

Abre **editor-mapa.html** con doble clic. Funciona sin internet ni servidor.

- 182 elementos separados en PNG transparente y Aseprite RGBA.
- Mapa de 48 x 32 celdas de 32px (1536 x 1024px).
- 79 objetos movibles; 4 entradas enlazadas con interiores editables.
- mapa-base.tmx: abrelo en Tiled; todos sus tilesets y PNG usan rutas relativas.
- mapa-base.aseprite: terreno, caminos y un objeto por capa (81 capas).
- mapa-base.mars.json: proyecto que puedes importar en el editor incluido.
- catalogo.html: catalogo visual con descargas PNG y Aseprite por elemento.
- elementos/: originales separados por categoria, a resolucion nativa.
- tiles32/: copias a 32px para pintar el mapa.
- fuentes/: las dos hojas originales y sus versiones limpiadas con imagegen.

## Editar el mapa

Selecciona y arrastra objetos. Usa la biblioteca para colocar mas. Activa la rejilla
y el ajuste para alinear. El inspector permite cambiar posicion, tamano, categoria
y colision. Doble clic en un elemento de la biblioteca abre el editor de pixeles.
Guarda el proyecto JSON para conservar tanto el mapa como los sprites modificados;
los navegadores no sobrescriben automaticamente los archivos del disco.

## Editar elementos

Cada PNG tiene un archivo .aseprite hermano y conserva su resolucion original.
El editor de pixeles incluido tiene lapiz, borrador, cuentagotas y deshacer.
El boton aplicar actualiza todas las instancias del mismo elemento en el mapa.
Puedes descargar el PNG modificado, o guardar el proyecto completo con esos cambios.

## Interiores reutilizables

Las entradas tienen interior_id y building_id. Los mapas interiores estan en
interiores/ y se abren desde el inspector del edificio. La logica de transicion
del motor de juego queda por implementar.
Las colisiones son rectangulos iniciales aproximados y se pueden editar en Tiled.

## Procedencia

Las dos hojas fueron aportadas por el usuario. Se limpiaron mediante la herramienta
imagegen integrada, preservando la disposicion para separar los sprites. Las salidas
generativas pueden introducir pequenas variaciones respecto a las hojas originales.
La separacion, alfa binario, exportacion nativa y composicion del mapa se hicieron
de forma determinista, sin modificar las fuentes originales. No se presupone una
licencia de redistribucion de las imagenes aportadas.

Prompts de limpieza: retirar solo el fondo cuadriculado, mantener canvas 1536x1024,
posiciones, escala y colores; fondo con alfa real y superficies de objetos opacas.

## Regenerar

Desde la raiz: npm install y npm run build:mars-base. Los recortes se definen en
scripts/mars-asset-specs.mjs; el mapa en scripts/build-mars-base.mjs.


Abre index.html para ver la base completa y sus cuatro interiores.

## Interiores incluidos

Laboratorio del domo, habitat de tripulacion, invernadero hidroponico y taller.
Hay tambien una plantilla vacia reutilizable. Abre interiores/index.html.

Los tres muebles nuevos se generaron con imagegen integrado siguiendo las hojas
aportadas. Sus prompts y referencias estan en fuentes/prompts-interiores.json.
Las fuentes originales y las versiones completas generadas se conservan alli.

Se comprobaron las rutas de los seis mapas TMX, sus referencias a PNG y tilesets,
los 188 archivos Aseprite y los 182 PNG individuales. Tambien se probaron en el
navegador la edicion de objetos, pixeles, guardado y reapertura de proyectos y
los enlaces entre exterior e interiores.
