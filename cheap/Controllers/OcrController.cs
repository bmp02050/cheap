using AutoMapper;
using cheap.Entities;
using cheap.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace cheap.Controllers;

[EnableRateLimiting("fixed")]
//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OcrController : Controller
{
    private IMapper _mapper;
    private readonly IOcrService _ocrService;

    public OcrController(IMapper mapper, IOcrService ocrService)
    {
        _mapper = mapper;
        _ocrService = ocrService;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> GetOcrData([FromBody] String imageData)
    {
        try
        {
            var imageBytes = Convert.FromBase64String(imageData);

            return Ok(await _ocrService.GetOcrResult(imageBytes));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}