using System.IO;
using System.IO.Compression;
using UnityEngine;

public static class Zipper
{
    /// <summary>
    /// Creates a zip archive from the specified folder and outputs to a destination file.
    /// Overwrites if a file already exists at destinationZipPath.
    /// </summary>
    public static bool CreateZipFromFolder(string sourceFolder, string destinationZipPath)
    {
        try
        {
            if (File.Exists(destinationZipPath))
                File.Delete(destinationZipPath);

            ZipFile.CreateFromDirectory(sourceFolder, destinationZipPath);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Zipper] Could not create zip from {sourceFolder}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Extracts a zip archive to the specified output folder.
    /// Overwrites existing files if they have the same name.
    /// </summary>
    public static bool ExtractZipToFolder(string zipPath, string outputFolder)
    {
        try
        {
            if (!File.Exists(zipPath))
            {
                Debug.LogError($"[Zipper] Zip file not found at {zipPath}");
                return false;
            }

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            ZipFile.ExtractToDirectory(zipPath, outputFolder, true);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Zipper] Could not extract zip {zipPath}: {ex.Message}");
            return false;
        }
    }
}
