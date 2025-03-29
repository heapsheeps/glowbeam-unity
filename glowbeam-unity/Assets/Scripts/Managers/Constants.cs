// Constants.cs
public static class Constants
{
    public const string CANVAS_GAMEOBJECT_NAME = "Canvas";
    public const string TESTCARD_QR_CODE_PREFAB = "Prefabs/QrCodePrefab";
    public static readonly int WEBSERVER_PORT = 8080;


    public static string GetTestcardImagePathForAspectRatio(AspectRatio aspectRatio)
    {
        switch(aspectRatio) 
        {
            case AspectRatio.AR_16x9:
                return "Resources/testcard-16x9.png";
            case AspectRatio.AR_16x10:
                return "Resources/testcard-16x10.png";
            case AspectRatio.AR_4x3:
                return "Resources/testcard-4x3.png";
            case AspectRatio.AR_Nonstandard:
                break;
        }
        return "Resources/testcard-16x9.png";
    }
}
