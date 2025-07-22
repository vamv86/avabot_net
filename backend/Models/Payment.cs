using System.ComponentModel.DataAnnotations;

namespace AiAgentApi.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public int SubscriptionId { get; set; }
    public virtual Subscription Subscription { get; set; } = null!;

    [Required]
    public string PaymentId { get; set; } = string.Empty;

    [Required]
    public string TransactionId { get; set; } = string.Empty;

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = "USD";

    [Required]
    public string Status { get; set; } = "pending";

    // Método de pago usado en este pago específico
    public int? PaymentMethodId { get; set; }
    public virtual PaymentMethod? PaymentMethod { get; set; }

    public string PaymentProvider { get; set; } = "Bold";
    public string PaymentType { get; set; } = "subscription";
    public string FailureReason { get; set; } = string.Empty;

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
