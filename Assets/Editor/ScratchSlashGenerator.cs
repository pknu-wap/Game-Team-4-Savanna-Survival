using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScratchSlashGenerator
{
    [MenuItem("Tools/Generate ScratchSlash Sprite")]
    public static void Generate()
    {
        int s = 16;
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        var px = new Color32[s * s];
        for (int i = 0; i < px.Length; i++)
            px[i] = new Color32(0, 0, 0, 0);

        for (int x = 0; x < s; x++)
            for (int d = -1; d <= 1; d++)
            {
                int y = s - 1 - x + d;
                if (y >= 0 && y < s)
                    px[y * s + x] = new Color32(255, 255, 255, 255);
            }

        tex.SetPixels32(px);
        tex.Apply();

        string dir = Application.dataPath + "/Sprites/VFX";
        Directory.CreateDirectory(dir);
        File.WriteAllBytes(dir + "/ScratchSlash.png", tex.EncodeToPNG());
        AssetDatabase.Refresh();

        string ap = "Assets/Sprites/VFX/ScratchSlash.png";
        var imp = (TextureImporter)AssetImporter.GetAtPath(ap);
        if (imp != null)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.filterMode = FilterMode.Point;
            imp.spritePixelsPerUnit = 16;
            AssetDatabase.ImportAsset(ap);
        }

        Debug.Log("[ScratchSlash] 스프라이트 생성 완료: " + ap);
    }
}
