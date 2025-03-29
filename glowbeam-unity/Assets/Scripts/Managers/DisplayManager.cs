using UnityEngine;
using UnityEngine.UI;

public class DisplayManager : MonoBehaviour
{
    private static DisplayManager _instance;
    public static DisplayManager Instance => _instance;

    private GameObject _backgroundImageGO;
    private RawImage _backgroundImage;
    private TestCard _testCard;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Find the existing Canvas in the scene
        var canvas = GameObject.Find(Constants.CANVAS_GAMEOBJECT_NAME);
        if (canvas == null)
        {
            Debug.LogError("No object named 'Canvas' found in the scene!");
            return;
        }

        _backgroundImageGO = new GameObject("BackgroundImage", typeof(RectTransform));
        _backgroundImageGO.transform.SetParent(canvas.transform, false);

        // Stretch the RectTransform to fill the canvas
        var backgroundImageRect = _backgroundImageGO.GetComponent<RectTransform>();
        backgroundImageRect.anchorMin = Vector2.zero;
        backgroundImageRect.anchorMax = Vector2.one;
        backgroundImageRect.offsetMin = Vector2.zero;
        backgroundImageRect.offsetMax = Vector2.zero;

        _backgroundImage = _backgroundImageGO.AddComponent<RawImage>();

        // Create testcard display helper
        _testCard = new TestCard(canvas, Screen.width, Screen.height);
        
        ClearScreen();
        ShowTestCard();
    }

    /// <summary>
    /// Show or hide the test card (already implemented in your older code).
    /// </summary>
    public void ShowTestCard()
    {
        _testCard.Show();
    }

    /// <summary>
    /// Display an image full screen
    /// </summary>
    public void ShowImage(Texture2D texture)
    {
        _testCard.Hide();
        _backgroundImage.texture = texture;
    }

    /// <summary>
    /// Display an image full screen
    /// </summary>
    public void ClearScreen()
    {
        _testCard.Hide();
        // Set a 1x1 black texture. We could set the tex to null, but then we'd have to change the color (which gets multiplied by the image)
        Texture2D blackTexture = new(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();
        _backgroundImage.texture = blackTexture;
    }
}
