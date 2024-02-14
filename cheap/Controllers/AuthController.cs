using AutoMapper;
using cheap.Models;
using cheap.Models.Users;
using cheap.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace cheap.Controllers;

[EnableRateLimiting("fixed")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private IMapper _mapper;
    private ITokenService _tokenService;

    public AuthController(IMapper mapper, ITokenService tokenService)
    {
        _mapper = mapper;
        _tokenService = tokenService;
    }

    //Authorize
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
    {
        try
        {
            var response = await _tokenService.Authenticate(model);
            if (response is null)
                return NotFound("User not found");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _tokenService.RefreshToken(request.UserId, request.RefreshToken);
            if (response is null)
                return NotFound("User not found");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}