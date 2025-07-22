namespace AiAgentApi.DTOs;

public class SubscriptionDto
{
    public int Id { get; set; }

    public string ExternalProductId { get; set; } // ID en Odoo
    public string ProductName { get; set; } = string.Empty; // Nombre del producto

    public string Status { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }

    public PaymentMethodDto? PaymentMethod { get; set; }
}

public class PaymentMethodDto
{
    public string LastFour { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
}

public class DashboardDataDto
{
    public bool Success { get; set; }
    public SubscriptionDto? Subscription { get; set; }
    public UsageStatsDto Stats { get; set; } = new();
}

public class UsageStatsDto
{
    public int MessagesThisMonth { get; set; }
    public int ResponsesThisMonth { get; set; }
    public double UptimePercentage { get; set; } = 99.9;
}
