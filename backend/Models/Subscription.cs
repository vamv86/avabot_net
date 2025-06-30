using System.ComponentModel.DataAnnotations;

namespace AiAgentApi.Models;

public class Subscription
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public string Status { get; set; } = "active"; // active, canceled, expired
    
    [Required]
    public string PlanName { get; set; } = "AI Agent Pro";
    
    [Required]
    public decimal Amount { get; set; } = 29.99m;
    
    [Required]
    public string Currency { get; set; } = "USD";
    
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    
    public string? PaymentProvider { get; set; } = "Bold";
    public string? PaymentMethodId { get; set; }
    public string? PaymentToken { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}