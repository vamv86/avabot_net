using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AiAgentApi.DTOs;
using AiAgentApi.Services;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get user dashboard data
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDataDto>> GetDashboardData()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _userService.GetDashboardDataAsync(userId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<AuthResponseDto>> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _userService.UpdateProfileAsync(userId, request);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPut("change-password")]
    public async Task<ActionResult<AuthResponseDto>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _userService.ChangePasswordAsync(userId, request);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Export user data
    /// </summary>
    [HttpGet("export-data")]
    public async Task<IActionResult> ExportUserData()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _userService.ExportUserDataAsync(userId);
        
        if (result.Success)
        {
            return File(result.Data, "application/json", "user-data.json");
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Delete user account
    /// </summary>
    [HttpDelete("account")]
    public async Task<ActionResult<AuthResponseDto>> DeleteAccount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _userService.DeleteAccountAsync(userId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
}