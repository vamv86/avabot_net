using Microsoft.AspNetCore.Mvc;
using AiAgentApi.DTOs;
using AiAgentApi.Services;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoldPaymentController : ControllerBase
{
    private readonly IBoldService _boldService;

    public BoldPaymentController(IBoldService boldService)
    {
        _boldService = boldService;
    }

    [HttpPost("signature")]
    public IActionResult GenerateSignature()
    {
        try
        {
            //string orderId = $"ORDER_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            string orderId = $"ORD{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"; // sin guion bajo, sin "ORDER_"

            string amount = "29000";
            string currency = "COP";

            var signature = _boldService.GenerateSignature(orderId, amount, currency);

            return Ok(new
            {
                orderId,
                amount,
                currency,
                signature
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}


