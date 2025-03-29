using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.IO.Compression;

public class ScanRoutes : WebApiController
{
    private readonly string _storagePath;

    public ScanRoutes()
    {
        _storagePath = Path.Combine(Application.persistentDataPath, "current_scan");
        Directory.CreateDirectory(_storagePath);
    }

    // POST /api/v1/scan/upload_scan
    [Route(HttpVerbs.Post, "/v1/scan/upload_scan")]
    public async Task UploadScan()
    {
        if (!HttpContext.Request.HasEntityBody)
        {
            HttpContext.Response.StatusCode = 400;
            var errorResp = ApiResponseFactory.Error("No file(s) uploaded. The request body is empty.");
            await HttpContext.SendDataAsync(errorResp);
            return;
        }

        // Just for demonstration: store raw body
        var filePath = Path.Combine(_storagePath, "upload_raw.bin");
        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await HttpContext.Request.InputStream.CopyToAsync(fs);
        }

        // In real code, parse multipart form files here and store them individually
        var success = ApiResponseFactory.Success("Upload successful.");
        await HttpContext.SendDataAsync(success);
    }

    // GET /api/v1/scan/download_scan
    // Streams a ZIP file back to the client.
    [Route(HttpVerbs.Get, "/v1/scan/download_scan")]
    public async Task DownloadScan()
    {
        var zipFilePath = Path.Combine(Application.temporaryCachePath, "scan_export.zip");
        if (File.Exists(zipFilePath))
            File.Delete(zipFilePath);

        ZipFile.CreateFromDirectory(_storagePath, zipFilePath);

        if (!File.Exists(zipFilePath))
        {
            HttpContext.Response.StatusCode = 404;
            var errorResp = ApiResponseFactory.Error("No scan data found to download.");
            await HttpContext.SendDataAsync(errorResp);
            return;
        }

        // Return a binary file
        HttpContext.Response.ContentType = "application/zip";
        HttpContext.Response.StatusCode = 200;
        // No 'AddHeader' method, so we do:
        HttpContext.Response.Headers["Content-Disposition"] = "attachment; filename=\"scan_export.zip\"";

        using (var fs = File.OpenRead(zipFilePath))
        {
            // Stream the bytes directly into the response
            await fs.CopyToAsync(HttpContext.Response.OutputStream);
        }

        // Clean up the temporary zip after streaming is done
        File.Delete(zipFilePath);
    }

    // GET /api/v1/scan/last_image
    // Streams a PNG image if it exists; otherwise returns JSON error
    [Route(HttpVerbs.Get, "/v1/scan/last_image")]
    public async Task GetLastImage()
    {
        var imagePath = Path.Combine(_storagePath, "scan_result.png");
        if (!File.Exists(imagePath))
        {
            HttpContext.Response.StatusCode = 404;
            var errorResp = ApiResponseFactory.Error("No last image found.");
            await HttpContext.SendDataAsync(errorResp);
            return;
        }

        // Return binary PNG
        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = "image/png";
        // Optional header
        HttpContext.Response.Headers["Content-Disposition"] = "inline; filename=\"scan_result.png\"";

        using (var fs = File.OpenRead(imagePath))
        {
            await fs.CopyToAsync(HttpContext.Response.OutputStream);
        }
    }
}
