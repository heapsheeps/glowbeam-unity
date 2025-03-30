using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public static class Utilities
{
    public static Texture2D LoadTextureFromStreamingAssets(string fileName)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);

        // If the path includes '://', it's probably on Android (or a similar platform) inside the APK.
        // We must use UnityWebRequest to read the file.
        if (fullPath.Contains("://") || fullPath.Contains(":///"))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
            {
                // Synchronous call for demonstration:
                var op = request.SendWebRequest();
                while (!op.isDone) { }

#if UNITY_2020_2_OR_NEWER
                if (request.result == UnityWebRequest.Result.ConnectionError 
                    || request.result == UnityWebRequest.Result.ProtocolError)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    Debug.LogError($"[Utilities] Error loading file from: {fullPath} - {request.error}");
                    return null;
                }

                byte[] fileData = request.downloadHandler.data;
                return CreateTextureFromData(fileData);
            }
        }
        else
        {
            // On desktop or other platforms, we can do direct file I/O:
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"[Utilities] File not found: {fullPath}");
                return null;
            }
            byte[] fileData = File.ReadAllBytes(fullPath);
            return CreateTextureFromData(fileData);
        }
    }

    private static Texture2D CreateTextureFromData(byte[] fileData)
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!tex.LoadImage(fileData))
        {
            Debug.LogError("[Utilities] Failed to load image data into texture.");
            return null;
        }
        return tex;
    }
}
