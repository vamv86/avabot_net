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
    /// Renew a specific product subscription
    /// </summary>
    [HttpPost("{externalProductId}/renew")]
    public async Task<ActionResult<PaymentResponseDto>> RenewSubscription(string externalProductId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _subscriptionService.RenewSubscriptionAsync(userId, externalProductId);

        if (result.Success)
        {
            await _n8nService.NotifySubscriptionChangeAsync(userId, "renewed");
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Cancel a specific product subscription
    /// </summary>
    [HttpPost("{externalProductId}/cancel")]
    public async Task<IActionResult> CancelSubscription(string externalProductId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var success = await _subscriptionService.CancelSubscriptionAsync(userId, externalProductId);

        if (success)
        {
            await _n8nService.NotifySubscriptionChangeAsync(userId, "canceled");
            return Ok(new { success = true, message = "Subscription canceled successfully" });
        }

        return BadRequest(new { success = false, message = "Failed to cancel subscription" });
    }

    /// <summary>
    /// Remove payment method from a specific product subscription
    /// </summary>
    [HttpPost("{externalProductId}/remove-payment-method")]
    public async Task<IActionResult> RemovePaymentMethod(string externalProductId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var success = await _subscriptionService.RemovePaymentMethodAsync(userId, externalProductId);

        if (success)
        {
            return Ok(new { success = true, message = "Payment method removed successfully" });
        }

        return BadRequest(new { success = false, message = "Failed to remove payment method" });
    }
}
