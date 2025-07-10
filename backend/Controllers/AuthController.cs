using Microsoft.AspNetCore.Mvc;
using AiAgentApi.DTOs;
using AiAgentApi.Services;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IN8NService _n8nService;

    public AuthController(IAuthService authService, IN8NService n8nService)
    {
        _authService = authService;
        _n8nService = n8nService;
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// User registration
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        
        if (result.Success && result.User != null)
        {
            // Notify N8N about new user registration
            await _n8nService.NotifyUserRegistrationAsync(
                result.User.Id, 
                result.User.Email, 
                result.User.WhatsApp);
            
            return Ok(result);
        }
        
        return BadRequest(result);
    }


    /// <summary>
    /// User registration when Payment
    /// </summary>
    [HttpPost("register-user-payment")]
    public async Task<ActionResult<AuthResponseDto>> RegisterUserPayment([FromBody] RegisterUserPaymentRequestDto request)
    {
        var result = await _authService.RegisterUserPaymentAsync(request);

        if (result.Success && result.User != null)
        {
            // Notify N8N about new user registration
            await _n8nService.NotifyUserRegistrationAsync(
                result.User.Id,
                result.User.Email,
                result.User.WhatsApp);

            return Ok(result);
        }

        return BadRequest(result);
    }
}