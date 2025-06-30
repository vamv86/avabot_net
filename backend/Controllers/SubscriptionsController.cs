using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AiAgentApi.DTOs;
using AiAgentApi.Services;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IN8NService _n8nService;

    public SubscriptionsController(ISubscriptionService subscriptionService, IN8NService n8nService)
    {
        _subscriptionService = subscriptionService;
        _n8nService = n8nService;
    }

    /// <summary>
    /// Renew user subscription
    /// </summary>
    [HttpPost("renew")]
    public async Task<ActionResult<PaymentResponseDto>> RenewSubscription()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _subscriptionService.RenewSubscriptionAsync(userId);
        
        if (result.Success)
        {
            await _n8nService.NotifySubscriptionChangeAsync(userId, "renewed");
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Cancel user subscription
    /// </summary>
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscription()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var success = await _subscriptionService.CancelSubscriptionAsync(userId);
        
        if (success)
        {
            await _n8nService.NotifySubscriptionChangeAsync(userId, "canceled");
            return Ok(new { success = true, message = "Subscription canceled successfully" });
        }
        
        return BadRequest(new { success = false, message = "Failed to cancel subscription" });
    }
}