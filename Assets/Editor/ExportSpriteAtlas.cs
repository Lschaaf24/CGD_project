using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.IO;
using System.Reflection;

public class ExportSpriteAtlas
{
    [MenuItem("Assets/Export Sprite Atlas to PNG", true)]
    private static bool ValidateExport() => Selection.activeObject is SpriteAtlas;

    [MenuItem("Assets/Export Sprite Atlas to PNG")]
    private static void Export()
    {
        SpriteAtlas atlas = Selection.activeObject as SpriteAtlas;
        SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

        // Access internal preview textures via reflection
        MethodInfo getPreviewTextures = typeof(SpriteAtlasExtensions).GetMethod("GetPreviewTextures", BindingFlags.NonPublic | BindingFlags.Static);
        Texture2D[] textures = (Texture2D[])getPreviewTextures.Invoke(null, new object[] { atlas });

        if (textures != null && textures.Length > 0)
        {
            Texture2D tex = textures[0];
            // Copy to a readable Texture2D
            RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height);
            Graphics.Blit(tex, rt);
            RenderTexture.active = rt;
            Texture2D readableTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
            readableTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            readableTex.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            string path = AssetDatabase.GetAssetPath(atlas).Replace(".spriteatlas", "_Exported.png");
            File.WriteAllBytes(path, readableTex.EncodeToPNG());
            AssetDatabase.Refresh();
            Debug.Log("Exported PNG to: " + path);
        }
    }
}