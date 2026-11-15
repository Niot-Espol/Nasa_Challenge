# Interiores reutilizables de ARES

Abre index.html para ver y editar los modulos.

- laboratorio: Investigación, análisis de muestras y control ambiental del domo. 17 objetos.
- habitat: Cuatro plazas de descanso, comedor, atención médica y terminal de misión. 20 objetos.
- invernadero: Seis bancales reutilizables, riego, reserva de agua y control de atmósfera. 26 objetos.
- taller: Robótica, reparación del rover, energía, herramientas y almacén de carga. 21 objetos.
- modulo-vacio: Suelo, estructura y esclusa listos para duplicar y equipar. 9 objetos.

Cada modulo tiene HTML local, proyecto JSON, TMX para Tiled, Aseprite con un
objeto por capa y PNG de vista previa. Todos usan los mismos tilesets relativos
y las mismas imagenes de elementos/ que el exterior. Las tres piezas nuevas
(cama, cultivo y comedor) se crearon con imagegen integrado usando las hojas
aportadas como referencia de estilo y se guardaron en fuentes/.

Para reutilizar un modulo: abre el editor, cambia los objetos y guarda una copia
del proyecto JSON. Para montar otra sala desde cero, abre modulo-vacio.html.

Los cuatro edificios del exterior enlazan estos modulos en el inspector. Las
esclusas de los interiores enlazan de vuelta al mapa exterior. Son enlaces entre
editores y metadatos TMX, no logica de navegacion de un motor de juego.

Mantener el lenguaje visual: metal gris y marfil, franjas naranjas, equipos cian,
cultivos verdes solo en invernaderos y esclusas en todos los accesos.
