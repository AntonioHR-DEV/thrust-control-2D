#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class GradientTextureCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Low Fuel Gradient Texture")]
    static void CreateGradient()
    {
        int width = 800;
        int height = 100; // height doesn't matter, it'll be stretched
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            // Transparent → Red → Transparent (centered peak)
            float t = x / (float)(width - 1);

            // Bell curve: peaks at center (t = 0.5)
            float alpha = Mathf.Sin(t * Mathf.PI);

            Color col = new Color(1f, 0f, 0f, alpha);

            for (int y = 0; y < height; y++)
                tex.SetPixel(x, y, col);
        }

        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string path = "Assets/Textures/LowFuelGradient.png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        // Set texture import settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.wrapMode = TextureWrapMode.Clamp;
        AssetDatabase.ImportAsset(path);

        Debug.Log("Gradient texture created at: " + path);
    }
}
#endif