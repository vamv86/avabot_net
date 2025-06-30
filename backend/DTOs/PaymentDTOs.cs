namespace AiAgentApi.DTOs;

public class CreatePaymentRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; } // Amount in cents
    public string Currency { get; set; } = "USD";
}

public class PaymentResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? PaymentUrl { get; set; }
    public string? PaymentId { get; set; }
}

public class PaymentWebhookDto
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}