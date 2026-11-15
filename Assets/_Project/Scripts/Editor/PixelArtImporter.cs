using UnityEditor;
using UnityEngine;

// Configura automáticamente los sprites pixel art al importarlos (filtro Point, sin compresión).
// La escala depende de la carpeta:
//   Todo el mundo usa 16 px = 1 unidad (misma densidad de píxel en personajes, suelo y objetos):
//   Art/Sprites/Characters/   → 16 px = 1 unidad, pivote en los pies   (astronautas 32x32 = 2 unidades)
//   Art/Tilesets/             → 16 px = 1 unidad, pivote al centro     (tiles de 16x16 = 1 celda)
//   Art/Sprites/Mars|Hub/     → 16 px = 1 unidad, pivote abajo-centro  (objetos: rocas, consolas...)
//   Art/Sprites/Mars/Vehicles → 32 px = 1 unidad, pivote al centro     (rover de 65 px ≈ 2 unidades)
// El arte de la entrega de terrenos (IA) se "repixeliza" antes (Repixelizer: ÷2 y paleta reducida).
public class PixelArtImporter : AssetPostprocessor
{
    const string ArtRoot = "Assets/_Project/Art/";

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ArtRoot)) return;

        int ppu; SpriteAlignment pivot;
        if (assetPath.StartsWith(ArtRoot + "Sprites/Characters/")) { ppu = 16; pivot = SpriteAlignment.BottomCenter; }
        else if (assetPath.StartsWith(ArtRoot + "Tilesets/")) { ppu = 16; pivot = SpriteAlignment.Center; }
        else if (assetPath.StartsWith(ArtRoot + "Sprites/Mars/Vehicles/")) { ppu = 32; pivot = SpriteAlignment.Center; }
        else if (assetPath.StartsWith(ArtRoot + "Sprites/Mars/") || assetPath.StartsWith(ArtRoot + "Sprites/Hub/")) { ppu = 16; pivot = SpriteAlignment.BottomCenter; }
        else return;

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = ppu;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteAlignment = (int)pivot;
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
    }
}
