using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GlowBeam
    {
    public static class Utilities
    {
        /// <summary>
        /// Loads an image from disk and creates a Texture2D using a texture format
        /// that best matches the image’s color channels (e.g. grayscale, RGB, or RGBA).
        /// Currently supports PNG detection; non-PNG images default to RGBA32.
        /// </summary>
        /// <param name="path">Full file path of the image.</param>
        /// <returns>A Texture2D loaded with the image data, or null if the file wasn’t found.</returns>
        public static Texture2D LoadTextureFromDisk(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File not found: " + path);
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);

            // Check for PNG header: first 4 bytes should be 137, 80, 78, 71
            if (fileData.Length >= 33 &&
                fileData[0] == 137 &&
                fileData[1] == 80 &&
                fileData[2] == 78 &&
                fileData[3] == 71)
            {
                // PNG file detected.
                // After the 8-byte signature, the first chunk should be IHDR.
                int pos = 8;
                // Read IHDR chunk length (big-endian)
                uint length = ((uint)fileData[pos] << 24) | ((uint)fileData[pos + 1] << 16) |
                            ((uint)fileData[pos + 2] << 8) | fileData[pos + 3];
                pos += 4;
                // Read the chunk type (should be "IHDR")
                string chunkType = Encoding.ASCII.GetString(fileData, pos, 4);
                pos += 4;
                if (chunkType == "IHDR")
                {
                    // IHDR chunk should be 13 bytes.
                    if (length != 13)
                        Debug.LogWarning("Unexpected IHDR length: " + length);

                    // IHDR data layout:
                    // 4 bytes: width
                    // 4 bytes: height
                    // 1 byte: bit depth
                    // 1 byte: color type
                    // 1 byte: compression method
                    // 1 byte: filter method
                    // 1 byte: interlace method
                    // Color type is the 10th byte of the IHDR data.
                    byte colorType = fileData[pos + 9];
                    TextureFormat format = TextureFormat.RGBA32; // default fallback

                    // Decide on the texture format based on PNG color type.
                    // See PNG spec: 0 = grayscale, 2 = RGB, 3 = indexed (treat as RGB), 
                    // 4 = grayscale+alpha, 6 = RGBA.
                    switch (colorType)
                    {
                        case 0: // Grayscale
                            // Use R8 (available in Unity 2018.3+); if not available, Alpha8 may be a fallback.
                            format = TextureFormat.R8;
                            break;
                        case 2: // Truecolor (RGB)
                            format = TextureFormat.RGB24;
                            break;
                        case 3: // Indexed (we treat as RGB)
                            format = TextureFormat.RGB24;
                            break;
                        case 4: // Grayscale with Alpha
                            // RG16 provides two channels (if supported), otherwise fallback.
                            format = TextureFormat.RG16;
                            break;
                        case 6: // Truecolor with Alpha (RGBA)
                            format = TextureFormat.RGBA32;
                            break;
                        default:
                            format = TextureFormat.RGBA32;
                            break;
                    }

                    // Create a temporary Texture2D.
                    // The dimensions provided here (2,2) are placeholders;
                    // LoadImage will automatically replace them with the correct values.
                    Texture2D texture = new Texture2D(2, 2, format, false);
                    texture.LoadImage(fileData);
                    return texture;
                }
            }

            // If not a PNG or if parsing failed, fallback to a default texture format.
            Texture2D fallbackTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            fallbackTexture.LoadImage(fileData);
            return fallbackTexture;
        }
    }
}