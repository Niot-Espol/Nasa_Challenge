# Diseña tu misión espacial

Videojuego desarrollado con Unity para diseñar, gestionar y simular una misión espacial completa tomando decisiones de ingeniería con presupuesto, energía y recursos limitados.

## Estructura

```text
Assets/_Project/
  Art/                    Materiales, modelos, sprites y texturas
  Audio/                  Música y efectos de sonido
  Data/                   Misiones, naves y cuerpos celestes
  Prefabs/                Objetos reutilizables de Unity
  Scenes/                 Escenas del juego
  Scripts/                Código C# organizado por responsabilidad
  ScriptableObjects/      Recursos de datos creados con Unity
  Settings/               Configuración propia del proyecto
  Tests/                  Pruebas EditMode y PlayMode
Packages/                 Paquetes administrados por Unity
ProjectSettings/          Configuración del proyecto Unity
docs/                     Diseño, arquitectura e investigación
scripts/                  Automatización externa
```

Las carpetas vacías contienen un archivo `.gitkeep` para que Git pueda conservarlas. Unity reemplazará gradualmente estos marcadores con archivos reales y sus correspondientes archivos `.meta`.

## Configuración recomendada de Unity

Después de abrir el proyecto, configura:

- `Version Control Mode`: **Visible Meta Files**.
- `Asset Serialization Mode`: **Force Text**.

Los archivos `.meta`, `Packages/manifest.json`, `Packages/packages-lock.json` y el contenido de `ProjectSettings/` deben conservarse en Git.
