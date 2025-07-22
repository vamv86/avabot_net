using System.ComponentModel.DataAnnotations;

namespace AiAgentApi.Models;

public class PaymentMethod
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    [Required]
    public string Provider { get; set; } = "Bold"; // Ej: Bold, Stripe, PayPal

    [Required]
    public string ExternalId { get; set; } = string.Empty; // ID en el proveedor

    public string? Token { get; set; }

    public string? Brand { get; set; }
    public string? Last4 { get; set; }
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }
    public string? Type { get; set; } = "card"; // card, bank_account, paypal

    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relaciones
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}