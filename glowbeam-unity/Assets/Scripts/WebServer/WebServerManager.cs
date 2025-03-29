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

        // Build path to your StreamingAssets for the static site
        _htmlRootPath = Path.Combine(Application.streamingAssetsPath, "WebRoot");
    }

    private void Start()
    {
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
                // Wait for the background thread to finish
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
        // Using a simple console logger as an example
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
}
