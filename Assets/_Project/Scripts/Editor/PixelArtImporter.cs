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
        if (assetPath.StartsWith(ArtRoot + "UI/")) { ImportUI(); return; }

        int ppu; SpriteAlignment pivot;
        if (assetPath.StartsWith(ArtRoot + "Sprites/Characters/")) { ppu = 16; pivot = SpriteAlignment.BottomCenter; }
        else if (assetPath.StartsWith(ArtRoot + "Tilesets/")) { ppu = 16; pivot = SpriteAlignment.Center; }
        else if (assetPath.StartsWith(ArtRoot + "Sprites/Mars/Vehicles/")) { ppu = 32; pivot = SpriteAlignment.Center; }
        else if (assetPath.StartsWith(ArtRoot + "Sprites/Mars/") || assetPath.StartsWith(ArtRoot + "Sprites/Hub/")) { ppu = 16; pivot = SpriteAlignment.BottomCenter; }
        else return;

        // Los indicadores y efectos flotan alrededor de su origen; los sólidos conservan sus pies.
        bool marsOverlay = assetPath.StartsWith(ArtRoot + "Sprites/Mars/Indicators/")
            || assetPath.StartsWith(ArtRoot + "Sprites/Mars/Effects/");
        bool repeatingDust = assetPath.StartsWith(ArtRoot + "Sprites/Mars/Effects/Dust/");
        if (marsOverlay) pivot = SpriteAlignment.Center;

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        // Las hojas cortadas en varias piezas (Multiple) se respetan; lo demás es un sprite único.
        if (importer.spriteImportMode != SpriteImportMode.Multiple) importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = ppu;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = repeatingDust ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        if (importer.spriteImportMode == SpriteImportMode.Multiple) { importer.SetTextureSettings(settings); return; }
        settings.spriteAlignment = (int)pivot;
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
    }

    // Interfaz: 25 px = 1 unidad de UI → en un Canvas de referencia 100 PPU se ve a ×4 (bordes gruesos y nítidos).
    // Los marcos llevan bordes 9-slice para estirarse sin deformar las esquinas.
    void ImportUI()
    {
        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 25;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        string name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        Vector4 border = Vector4.zero;   // (izq, abajo, der, arriba)
        if (name.EndsWith("_frame") && !name.StartsWith("resource_bar")) border = new Vector4(16, 16, 16, 16);
        else if (name.StartsWith("button_")) border = new Vector4(8, 3, 8, 3);
        else if (name == "resource_bar_frame") border = new Vector4(4, 2, 4, 2);
        importer.spriteBorder = border;
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteAlignment = (int)SpriteAlignment.Center;
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
    }
}
