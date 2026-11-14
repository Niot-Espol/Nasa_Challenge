using UnityEditor;
using UnityEngine;

// Configura automáticamente los sprites pixel art de personajes al importarlos:
// 16 px = 1 unidad (= 1 tile de 16x16), sin filtrado (nítido), sin compresión, pivote abajo-centro (pies).
// Aplica a todo lo que esté en Assets/_Project/Art/Sprites/Characters/.
public class PixelArtImporter : AssetPostprocessor
{
    const string Folder = "Assets/_Project/Art/Sprites/Characters/";
    const int PixelsPerUnit = 16;

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(Folder)) return;

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
    }
}
