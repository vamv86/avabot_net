using Microsoft.AspNetCore.Mvc;
using AiAgentApi.DTOs;
using AiAgentApi.Services;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Create a Bold.co payment
    /// </summary>
    [HttpPost("create-bold-payment")]
    public async Task<ActionResult<PaymentResponseDto>> CreateBoldPayment([FromBody] CreatePaymentRequestDto request)
    {
        var result = await _paymentService.CreateBoldPaymentAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Handle Bold.co payment webhook
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookDto webhook)
    {
        try
        {
            var success = await _paymentService.ProcessPaymentWebhookAsync(webhook);
            
            if (success)
            {
                return Ok(new { message = "Webhook processed successfully" });
            }
            
            return BadRequest(new { message = "Failed to process webhook" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment webhook");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}