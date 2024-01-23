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
    private readonly IEmailService EmailService;
    private readonly ITokenService TokenService;

    public UsersController(IUserService userService, IMapper mapper, IEmailService emailService,
        ITokenService tokenService)
    {
        _userService = userService;
        _mapper = mapper;
        EmailService = emailService;
        TokenService = tokenService;
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
            var token = await TokenService.GenerateRegistrationInvitationTokenAsync(result);
            var confirmationLink =
                Url.Action("ConfirmEmail", "Users", new { userId = result.Id, token }, Request.Scheme);

            await EmailService.SendEmailAsync(result.Email, "Confirm Email", confirmationLink);

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

            var token = await TokenService.GenerateRegistrationInvitationTokenAsync(user);
            var confirmationLink =
                Url.Action("ConfirmEmail", "Users", new { userId = user.Id, token }, Request.Scheme);

            await EmailService.SendEmailAsync(user.Email, "Confirm Email", confirmationLink);
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

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
    {
        try
        {
            var response = await _userService.Authenticate(model);
            if (response is null)
                return NotFound("User not found");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
        var result = await TokenService.ConfirmRegistrationAsync(user.Id, token);

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