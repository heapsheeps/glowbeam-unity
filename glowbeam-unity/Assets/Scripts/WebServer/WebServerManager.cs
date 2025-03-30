using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using EmbedIO.Files;

public class WebServerManager : MonoBehaviour
{
    private static WebServerManager _instance;
    public static WebServerManager Instance => _instance;

    private WebServer _webServer;
    private Thread _webServerThread;
    private CancellationTokenSource _cancellationTokenSource;

    // We’ll store the final folder path we serve from
    private string _htmlRootPath;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        LogManager.Log(LogLevel.Info, "WebServerManager initialized.");
    }

    private void Start()
    {
        // On Android, copy from StreamingAssets to persistentDataPath.
        // Else, just use the normal path in StreamingAssets/WebRoot.
#if UNITY_ANDROID
        // Copy the entire "WebRoot" folder from streaming assets to persistentDataPath
        // so EmbedIO can read from an actual filesystem location.
        string localFolder = Path.Combine(Application.persistentDataPath, "WebRoot");
        if (!Directory.Exists(localFolder))
            Directory.CreateDirectory(localFolder);

        // Copy synchronously here for simplicity. If your project is large, consider doing this in a coroutine.
        CopyWebRootToPersistentDataPath("WebRoot", localFolder);

        _htmlRootPath = localFolder;
#else
        // Non-Android: we can serve directly from StreamingAssets
        _htmlRootPath = Path.Combine(Application.streamingAssetsPath, "WebRoot");
#endif

        StartServer();
    }

    private void OnDestroy()
    {
        StopServer();
    }

    private void StartServer()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        int port = Constants.WEBSERVER_PORT;
        _webServer = CreateWebServer(port);

        // Launch in background thread so Unity’s main thread is free
        _webServerThread = new Thread(() =>
        {
            try
            {
                _webServer.RunAsync(_cancellationTokenSource.Token).Wait();
            }
            catch (Exception e)
            {
                LogManager.Log(LogLevel.Error, $"WebServer error: {e.Message}");
            }
        });
        _webServerThread.IsBackground = true;
        _webServerThread.Start();

        LogManager.Log(LogLevel.Info, $"WebServer started on port {port}.");
    }

    private void StopServer()
    {
        if (_webServer != null)
        {
            _cancellationTokenSource.Cancel();
            if (_webServerThread != null && _webServerThread.IsAlive)
            {
                _webServerThread.Join();
                _webServerThread = null;
            }
            _webServer.Dispose();
            _webServer = null;
            LogManager.Log(LogLevel.Info, "WebServer stopped.");
        }
    }

    private WebServer CreateWebServer(int port)
    {
        return new WebServer(o => o
                .WithUrlPrefix($"http://*:{port}/")
                .WithMode(HttpListenerMode.EmbedIO))
            .WithLocalSessionManager()
            .WithWebApi("/api", m =>
            {
                m.WithController<DisplayRoutes>();
                m.WithController<ScanRoutes>();
            })
            .WithStaticFolder("/", _htmlRootPath, true, m => m.WithContentCaching(true))
            .WithModule(new ActionModule("/", HttpVerbs.Any, ctx =>
            {
                ctx.Response.StatusCode = 404;
                return Task.CompletedTask;
            }));
    }

#if UNITY_ANDROID
    /// <summary>
    /// Copies the files from StreamingAssets/WebRoot into persistentDataPath/WebRoot,
    /// so EmbedIO can serve them from a real filesystem path on Android.
    /// </summary>
    private void CopyWebRootToPersistentDataPath(string sourceDirName, string destDirName)
    {
        // For demonstration, let's assume we have a known list of files, or a small set.
        // If you have subfolders or multiple files, adapt accordingly.
        // If you want to do this dynamically, you'll need a manifest or a known set of files.

        string[] files = { "index.html", "styles.css", "scripts.js" };
        
        foreach (var file in files)
        {
            // Build the streaming assets path
            string streamingPath = Path.Combine(Application.streamingAssetsPath, sourceDirName);
            streamingPath = Path.Combine(streamingPath, file);

            // On Android, streamingPath likely has jar:file://..., so we use UnityWebRequest
            if (streamingPath.Contains("://"))
            {
                // Copy using UnityWebRequest synchronously for demonstration:
                var request = UnityEngine.Networking.UnityWebRequest.Get(streamingPath);
                var op = request.SendWebRequest();
                while (!op.isDone) { }

#if UNITY_2020_2_OR_NEWER
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError
                    || request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    Debug.LogError($"Failed to copy {streamingPath}: {request.error}");
                    continue;
                }

                byte[] data = request.downloadHandler.data;
                string outFile = Path.Combine(destDirName, file);
                File.WriteAllBytes(outFile, data);
            }
            else
            {
                // On desktop (unlikely to hit this block in #if UNITY_ANDROID, but just in case)
                if (!File.Exists(streamingPath))
                {
                    Debug.LogWarning($"[CopyWebRootToPersistentDataPath] File not found: {streamingPath}");
                    continue;
                }
                byte[] data = File.ReadAllBytes(streamingPath);
                string outFile = Path.Combine(destDirName, file);
                File.WriteAllBytes(outFile, data);
            }
        }
    }
#endif
}
