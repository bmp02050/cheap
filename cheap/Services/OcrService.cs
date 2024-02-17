using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace cheap.Services;

public interface IOcrService
{
    Task<OcrResult> GetOcrResult(byte[] imageData);
}
public class OcrService : IOcrService
{
    private OcrClient _ocr;
    public OcrService(OcrClient ocr)
    {
        _ocr = ocr;
    }

    public async Task<OcrResult> GetOcrResult(byte[] imageData)
    {
        return await _ocr.GetOcrResult(imageData);
    }
}