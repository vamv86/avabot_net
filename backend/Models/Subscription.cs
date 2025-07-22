using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace AiAgentApi.Models;

public class Subscription
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public int? PaymentMethodId { get; set; }
    public virtual PaymentMethod? PaymentMethod { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "active"; // active, canceled, expired

    // Información del producto externo (Odoo)
    [Required]
    public string ExternalProductId { get; set; } // ID del producto en Odoo

    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PlanName { get; set; } = string.Empty;

    [Required]
    [Precision(10, 2)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

