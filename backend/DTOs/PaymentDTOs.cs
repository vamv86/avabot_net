namespace AiAgentApi.DTOs;


// ------------------------------
// PAYMENT - CLIENT TO API
// ------------------------------

public class CreatePaymentRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public int Amount { get; set; } // En centavos

    public string Currency { get; set; } = "USD";

    public int ExternalProductId { get; set; } // Producto en Odoo
    public int? SubscriptionId { get; set; } // Si ya está creada
}

public class PaymentResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public string? PaymentUrl { get; set; }
    public string? PaymentId { get; set; }

    public int? ExternalProductId { get; set; }
}

// ------------------------------
// PAYMENT - WEBHOOK
// ------------------------------

public class PaymentWebhookDto
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public int ExternalProductId { get; set; }
    public int? SubscriptionId { get; set; }
}