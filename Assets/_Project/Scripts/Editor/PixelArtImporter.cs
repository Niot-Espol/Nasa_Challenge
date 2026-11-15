using UnityEditor;
using UnityEngine;

// Configura automáticamente los sprites pixel art al importarlos (filtro Point, sin compresión).
// La escala depende de la carpeta:
//   Art/Sprites/Characters/   → 32 px = 1 unidad, pivote en los pies   (astronautas, 32x32 → misma densidad que el terreno)
//   Art/Tilesets/             → 32 px = 1 unidad, pivote al centro     (tiles del suelo = 1 celda)
//   Art/Sprites/Mars|Hub/     → 32 px = 1 unidad, pivote abajo-centro  (objetos: rocas, consolas...)
//   Art/Sprites/Mars/Vehicles → 64 px = 1 unidad, pivote al centro     (rover de 130 px ≈ 2 unidades)
public class PixelArtImporter : AssetPostprocessor
{
    const string ArtRoot = "Assets/_Project/Art/";

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ArtRoot)) return;

        int ppu; SpriteAlignment pivot;
        if (assetPath.StartsWith(ArtRoot + "Sprites/Characters/")) { ppu = 32; pivot = SpriteAlignment.BottomCenter; }
        else if (assetPath.StartsWith(ArtRoot + "Tilesets/")) { ppu = 32; pivot = SpriteAlignment.Center; }
        else if (assetPath.StartsWith(ArtRoot + "Sprites/Mars/Vehicles/")) { ppu = 64; pivot = SpriteAlignment.Center; }
        else if (assetPath.StartsWith(ArtRoot + "Sprites/Mars/") || assetPath.StartsWith(ArtRoot + "Sprites/Hub/")) { ppu = 32; pivot = SpriteAlignment.BottomCenter; }
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
