using UnityEngine;
using QRCoder;

public class QRCodes
{
    public static Texture2D GenerateQRCodeTexture(string content, int pixelsPerModule = 20, Color32 darkColor = default, Color32 lightColor = default)
    {
        if (darkColor.Equals(default)) darkColor = Color.black;
        if (lightColor.Equals(default)) lightColor = Color.white;

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            return GenerateTextureFromQRCodeData(qrCodeData, pixelsPerModule, darkColor, lightColor);
        }
    }

    private static Texture2D GenerateTextureFromQRCodeData(QRCodeData data, int pixelsPerModule, Color32 darkColor, Color32 lightColor)
    {
        int size = data.ModuleMatrix.Count * pixelsPerModule;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool module = data.ModuleMatrix[y / pixelsPerModule][x / pixelsPerModule];
                texture.SetPixel(x, size - y - 1, module ? darkColor : lightColor);
            }
        }

        texture.Apply();
        return texture;
    }
}
