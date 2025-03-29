using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public class DisplayRoutes : WebApiController
{
    // e.g. GET /api/v1/display/info
    [Route(HttpVerbs.Get, "/v1/display/info")]
    public async Task GetDisplayInfo()
    {
        var width = Screen.width;
        var height = Screen.height;

        var data = new
        {
            width,
            height
        };

        // Return JSON with a standard success structure
        var response = ApiResponseFactory.Success("Display info fetched.", data);
        await HttpContext.SendDataAsync(response);
    }

    // GET /api/v1/display/testcard/show_testcard
    [Route(HttpVerbs.Post, "/v1/display/show_testcard")]
    public async Task ShowTestCard()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            DisplayManager.Instance.ShowTestCard();
        });
        var response = ApiResponseFactory.Success("Testcard is now shown.");
        await HttpContext.SendDataAsync(response);
    }

    // POST /api/v1/display/show_image
    [Route(HttpVerbs.Post, "/v1/display/show_image")]
    public async Task ShowImage()
    {
        // 1) Check for a request body
        if (!HttpContext.Request.HasEntityBody)
        {
            HttpContext.Response.StatusCode = 400;
            var errorResp = ApiResponseFactory.Error("No image data provided.");
            await HttpContext.SendDataAsync(errorResp);
            return;
        }

        // 2) Read the entire body (raw PNG/JPEG)
        byte[] imageData;
        using (var ms = new MemoryStream())
        {
            await HttpContext.Request.InputStream.CopyToAsync(ms);
            imageData = ms.ToArray();
        }

        // 3) We'll use a TaskCompletionSource to wait for the main thread to finish
        var tcs = new TaskCompletionSource<ApiResponse>();

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            // Must be on main thread to call Unity APIs
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            bool loaded = tex.LoadImage(imageData);
            if (!loaded)
            {
                tcs.SetResult(ApiResponseFactory.Error("Failed to interpret image data as PNG/JPEG."));
                return;
            }

            // 4) If loaded, show it via your manager
            DisplayManager.Instance.ShowImage(tex);

            // Indicate success
            tcs.SetResult(ApiResponseFactory.Success("Image displayed successfully."));
        });

        // 5) Wait asynchronously for the main-thread action to complete
        var result = await tcs.Task;

        // 6) Finally, send JSON back to the client
        await HttpContext.SendDataAsync(result);
    }

    // POST /api/v1/display/clear
    [Route(HttpVerbs.Post, "/v1/display/clear")]
    public async Task Clear()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            DisplayManager.Instance.ClearScreen();
        });
        var response = ApiResponseFactory.Success("Screen cleared.");
        await HttpContext.SendDataAsync(response);
    }
}
