using System.Net.Http.Headers;
using System.Web;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;

namespace cheap.Services;

public class OcrClient
{
    private readonly OcrSettings _ocrSettings;

    public OcrClient(IOptions<OcrSettings> ocrSettings)
    {
        _ocrSettings = ocrSettings.Value;
    }

    public async Task<OcrResult> GetOcrResult(byte[] imageData)
    {
        var client = new HttpClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);

        // Request headers
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{_ocrSettings.OcrKey}");

        // Request parameters
        queryString["language"] = "unk";
        queryString["detectOrientation"] = "true";
        queryString["model-version"] = "latest";
        var uri = $"{_ocrSettings.OcrEndpoint}vision/v3.2/ocr?" + queryString;

        HttpResponseMessage response;

        // Request body
        using (var content = new ByteArrayContent(imageData))
        {
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(uri, content);
        }

        var stuff = await response.Content.ReadAsAsync<OcrResult>();
        return stuff;
    }
}