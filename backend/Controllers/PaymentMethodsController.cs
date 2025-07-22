using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AiAgentApi.Services;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentMethodsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public PaymentMethodsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Remove payment method for a specific product subscription
    /// </summary>
    [HttpDelete("{externalProductId}")]
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
