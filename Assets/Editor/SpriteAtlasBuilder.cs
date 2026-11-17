using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using System.IO;

public class SpriteAtlasBuilder
{
    [MenuItem("Tools/Pack Selected Sprites for TMP")]
    public static void PackSprites()
    {
        Object[] selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        if (selection.Length == 0)
        {
            Debug.LogWarning("Select your individual sprite files in the Project window first.");
            return;
        }

        // Convert each source texture into an uncompressed RGBA32 Texture2D
        Texture2D[] readableTextures = new Texture2D[selection.Length];
        for (int i = 0; i < selection.Length; i++)
        {
            Texture2D src = (Texture2D)selection[i];
            RenderTexture rt = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(src, rt);
            RenderTexture.active = rt;

            Texture2D copy = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false);
            copy.name = src.name;
            copy.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0);
            copy.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            readableTextures[i] = copy;
        }

        // Pack into an uncompressed atlas (makeNoLongerReadable = false)
        Texture2D atlas = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
        Rect[] rects = atlas.PackTextures(readableTextures, 4, 2048, false);

        // Save PNG to Assets root
        string savePath = "Assets/Combined_TMP_Atlas.png";
        File.WriteAllBytes(savePath, atlas.EncodeToPNG());
        AssetDatabase.Refresh();

        // Configure texture as Multiple Sprite
        TextureImporter atlasImporter = (TextureImporter)AssetImporter.GetAtPath(savePath);
        atlasImporter.textureType = TextureImporterType.Sprite;
        atlasImporter.spriteImportMode = SpriteImportMode.Multiple;
        atlasImporter.SaveAndReimport();

        // Slice using Unity 6 ISpriteEditorDataProvider
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(atlasImporter);
        dataProvider.InitSpriteEditorDataProvider();

        SpriteRect[] spriteRects = new SpriteRect[rects.Length];
        for (int i = 0; i < rects.Length; i++)
        {
            spriteRects[i] = new SpriteRect
            {
                name = readableTextures[i].name,
                spriteID = GUID.Generate(),
                alignment = SpriteAlignment.BottomLeft,
                pivot = new Vector2(0f, 0f),
                rect = new Rect(
                    Mathf.Round(rects[i].x * atlas.width),
                    Mathf.Round(rects[i].y * atlas.height),
                    Mathf.Round(rects[i].width * atlas.width),
                    Mathf.Round(rects[i].height * atlas.height)
                )
            };
        }

        dataProvider.SetSpriteRects(spriteRects);
        dataProvider.Apply();
        atlasImporter.SaveAndReimport();

        Debug.Log($"Success! Packed and sliced {readableTextures.Length} sprites into: {savePath}");
    }
}
