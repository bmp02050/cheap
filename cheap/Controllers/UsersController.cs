using AutoMapper;
using cheap.Entities;
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
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly IUserPreferenceService _userPreferenceService;

    public UsersController(IUserService userService, IMapper mapper, IEmailService emailService,
        ITokenService tokenService, IUserPreferenceService userPreferenceService)
    {
        _userService = userService;
        _mapper = mapper;
        _emailService = emailService;
        _tokenService = tokenService;
        _userPreferenceService = userPreferenceService;
    }
    //User Preferences
    [HttpPost("preferences")]
    public async Task<IActionResult> Preferences([FromBody] UserPreference preferences)
    {
        try
        {
            var result = await _userPreferenceService.Update(preferences);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // map model to entity
        var user = _mapper.Map<User>(model);

        try
        {
            // create user
            var result = await _userService.Create(user, model.Password);
            var token = await _tokenService.GenerateRegistrationInvitationTokenAsync(result);
            var confirmationLink =
                Url.Action("ConfirmEmail", "Users", new { userId = result.Id, token }, Request.Scheme);

            await _emailService.SendEmailAsync(result.Email, "Confirm Email", confirmationLink);

            return Ok(_mapper.Map<UserModel>(result));
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(ex);
        }
    }

    [AllowAnonymous]
    [HttpGet("{id}/token")]
    public async Task<IActionResult> GenerateNewRegistrationToken(Guid id)
    {
        try
        {
            var user = await _userService.GetById(id);

            var token = await _tokenService.GenerateRegistrationInvitationTokenAsync(user);
            var confirmationLink =
                Url.Action("ConfirmEmail", "Users", new { userId = user.Id, token }, Request.Scheme);

            await _emailService.SendEmailAsync(user.Email, "Confirm Email", confirmationLink);
            return Ok("Email verification resent");
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] UpdateModel model, Guid id)
    {
        var userId = User.FindFirst("Id")?.Value;
        if (!String.IsNullOrEmpty(userId) && id != new Guid(userId))
            throw new UnauthorizedAccessException("You are not this person or the ID is missing");
        if (model.Id == Guid.Empty)
            model.Id = id;
        // map model to entity and set id
        var user = _mapper.Map<User>(model);

        try
        {
            // update user 
            return Ok(_mapper.Map<UserModel>(
                await _userService.Update(user, model.Password)));
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        try
        {
            // update user 
            return Ok(await _userService.GetById(id));
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.Delete(id);
        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [EnableRateLimiting("fixed")]
    [Authorize]
    [HttpGet]
    [Route("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst("Id")?.Value;
        if (String.IsNullOrEmpty(userId))
            return BadRequest("UserID is not found");

        var user = await _userService.GetById(new Guid(userId));
        var userModel = _mapper.Map<UserModel>(user);
        return Ok(userModel);
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
        {
            return BadRequest("Invalid confirmation link.");
        }

        var user = await _userService.GetById(new Guid(userId));
        var result = await _tokenService.ConfirmRegistrationAsync(user.Id, token);

        if (result)
        {
            // Email confirmed successfully
            user.VerifiedEmail = result;
            await _userService.Update(user);
            return Ok("Email confirmed. You can now log in.");
        }

        // Handle confirmation failure
        return BadRequest("Email confirmation failed.");
    }
}