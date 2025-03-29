using System.IO;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static Texture2D LoadTextureFromStreamingAssets(string fileName)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        byte[] fileData = null;

        // On some platforms, especially Android, streaming assets may require special handling,
        // but if it’s a normal desktop or iOS build, direct File.ReadAllBytes works fine.
        fileData = File.ReadAllBytes(fullPath);

        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!tex.LoadImage(fileData))
        {
            Debug.LogError("Failed to load image data into texture.");
            return null;
        }
        return tex;
    }

}
