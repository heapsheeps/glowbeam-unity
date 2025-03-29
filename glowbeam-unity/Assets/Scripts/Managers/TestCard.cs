using System.Net;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCard
{
    private readonly GameObject _testCardParent;
    private readonly RawImage _testCardBackgroundImage;
    private readonly GameObject _qrCodePrefab;

    /// <summary>
    /// Constructor for TestCard (non-MonoBehaviour).
    /// </summary>
    /// <param name="width">Desired width (for reference or usage).</param>
    /// <param name="height">Desired height (for reference or usage).</param>
    public TestCard(GameObject canvas, int width, int height)
    {
        // 1) Find the existing Canvas in the scene
        var canvasObj = GameObject.Find(Constants.CANVAS_GAMEOBJECT_NAME);
        if (canvasObj == null)
        {
            Debug.LogError("[TestCard] No object named 'Canvas' found in the scene!");
            return;
        }
        var canvasTransform = canvasObj.transform;

        // 2) Create parent object for this "TestCard"
        _testCardParent = new GameObject("TestCard", typeof(RectTransform));
        _testCardParent.transform.SetParent(canvasTransform, false);

        // Stretch the RectTransform to fill the canvas
        var parentRect = _testCardParent.GetComponent<RectTransform>();
        parentRect.anchorMin = Vector2.zero; // bottom-left
        parentRect.anchorMax = Vector2.one;  // top-right
        parentRect.offsetMin = Vector2.zero;
        parentRect.offsetMax = Vector2.zero;

        // 3) Create a RawImage that fills this parent
        GameObject rawImageGO = new GameObject("TestCardRawImage", typeof(RectTransform));
        rawImageGO.transform.SetParent(_testCardParent.transform, false);

        var rawImageRect = rawImageGO.GetComponent<RectTransform>();
        rawImageRect.anchorMin = Vector2.zero;
        rawImageRect.anchorMax = Vector2.one;
        rawImageRect.offsetMin = Vector2.zero;
        rawImageRect.offsetMax = Vector2.zero;

        _testCardBackgroundImage = rawImageGO.AddComponent<RawImage>();

        // 4) Load the test card image from StreamingAssets, apply to the RawImage
        AspectRatio aspectRatio = AspectRatioUtility.GetAspectRatio(width, height);
        if(aspectRatio == AspectRatio.AR_Nonstandard)
        {
            LogManager.Log(LogLevel.Warning, $"[TestCard] Non-standard aspect ratio detected: {width}x{height}. Using default test card image.");
        }

        string testCardImagePath = Constants.GetTestcardImagePathForAspectRatio(aspectRatio);
        var testTexture = Utilities.LoadTextureFromStreamingAssets(testCardImagePath);
        if (testTexture != null)
        {
            _testCardBackgroundImage.texture = testTexture;
        }
        else
        {
            Debug.LogError("[TestCard] Could not load test card image from StreamingAssets.");
        }

        // 5) Instantiate the QrCodePrefab as a child of "TestCard"
        //    (Ensure you have a prefab in e.g. Resources/QrCodePrefab.prefab)
        var qrPrefab = Resources.Load<GameObject>(Constants.TESTCARD_QR_CODE_PREFAB);
        if (qrPrefab == null)
        {
            Debug.LogError("[TestCard] Could not find QrCodePrefab in Resources!");
        }
        else
        {
            _qrCodePrefab = Object.Instantiate(qrPrefab, _testCardParent.transform, false);
            _qrCodePrefab.name = "QrCodeInstance";
        }

        // 6) Attempt to set the IP address & generate a QR code
        SetIpAddress();
    }

    /// <summary>
    /// Finds the IP address or hostname, sets the prefab text,
    /// generates a QR code, and assigns it to the prefab’s image.
    /// </summary>
    public void SetIpAddress()
    {
        if (_qrCodePrefab == null) return;

        // (A) Find the local IP address / hostname.
        string hostName = Dns.GetHostName();
        string ipAddress = "127.0.0.1"; // fallback
        var hostEntry = Dns.GetHostEntry(hostName);
        foreach (var addr in hostEntry.AddressList)
        {
            // pick an IPv4 address (ignore IPv6)
            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipAddress = addr.ToString();
                break;
            }
        }

        ipAddress = $"{ipAddress}:{Constants.WEBSERVER_PORT}"; // Append the port number

        // (B) Set the text field
        List<GameObject> ipLabels = _testCardParent.FindAllChildrenByName("Label");
        foreach(GameObject ipLabel in ipLabels)
        {
            var ipLabelText = ipLabel.GetComponent<TMPro.TMP_Text>();
            if (ipLabelText != null)
            {
                ipLabelText.text = ipAddress;
            }
        }

        // (C) Generate a QR code from the IP (placeholder function)
        Texture2D qrTexture = QRCodes.GenerateQRCodeTexture("http://" + ipAddress);
        List<GameObject> ipQrImages = _testCardParent.FindAllChildrenByName("Image");
        foreach (GameObject ipQrImage in ipQrImages)
        {
            var ipQrRawImage = ipQrImage.GetComponent<RawImage>();
            if (ipQrRawImage != null)
            {
                ipQrRawImage.texture = qrTexture;
                //myRawImage.SetNativeSize();
            }
        }
    }

    /// <summary>
    /// Shows the entire TestCard object.
    /// </summary>
    public void Show()
    {
        if (_testCardParent != null)
            _testCardParent.Show();
    }

    /// <summary>
    /// Hides the entire TestCard object.
    /// </summary>
    public void Hide()
    {
        if (_testCardParent != null)
            _testCardParent.Hide();
    }
}
