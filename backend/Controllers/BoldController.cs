using Microsoft.AspNetCore.Mvc;
using AiAgentApi.DTOs;
using AiAgentApi.Services;

namespace AiAgentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoldPaymentController : ControllerBase
{
    private readonly IBoldService _boldService;
    private readonly IOdooService _odooService;

    public BoldPaymentController(
        IBoldService boldService,
        IOdooService odooService
        )
    {
        _boldService = boldService;
        _odooService = odooService;
    }

    [HttpPost("signature")]
    public async Task<IActionResult> GenerateSignature()
    {
        try
        {
            //string orderId = $"ORDER_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            string orderId = $"ORD{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"; // sin guion bajo, sin "ORDER_"

            var product = await _odooService.ObtenerProductoPorNombreAsync("AvaBot");

            string amount = product.ListPrice.ToString().Replace(",",".");
            string currency = product.CurrencyId[1].ToString();

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


    [HttpPost("price-ava-bot")]
    public async Task<IActionResult> GeneratePriceAvaBot()
    {
        try
        {
            var product = await _odooService.ObtenerProductoPorNombreAsync("AvaBot");

            string amount = product.ListPrice.ToString().Replace(",", ".");
            string currency = product.CurrencyId[1].ToString();
     
            return Ok(new
            {
                amount,
                currency
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}


