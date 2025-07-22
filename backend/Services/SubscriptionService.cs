using Microsoft.EntityFrameworkCore;
using AiAgentApi.Data;
using AiAgentApi.DTOs;

namespace AiAgentApi.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        ApplicationDbContext context,
        IPaymentService paymentService,
        ILogger<SubscriptionService> logger)
    {
        _context = context;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> RenewSubscriptionAsync(int userId, string externalProductId)
    {
        try
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                          s.ExternalProductId == externalProductId &&
                                          s.Status == "active");

            if (subscription == null)
            {
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "No active subscription found for this product"
                };
            }

            if (subscription.PaymentMethodId.HasValue)
            {
                return await _paymentService.ProcessRecurringPaymentAsync(subscription.Id);
            }
            else
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var paymentRequest = new CreatePaymentRequestDto
                {
                    Email = user.Email,
                    WhatsApp = user.WhatsApp,
                    Name = user.Name,
                    Amount = (int)(subscription.Amount * 100),
                    Currency = subscription.Currency
                };

                return await _paymentService.CreateBoldPaymentAsync(paymentRequest);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error renewing subscription");
            return new PaymentResponseDto
            {
                Success = false,
                Message = "An error occurred while renewing the subscription"
            };
        }
    }

    public async Task<bool> CancelSubscriptionAsync(int userId, string externalProductId)
    {
        try
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                          s.ExternalProductId == externalProductId &&
                                          s.Status == "active");

            if (subscription == null)
            {
                return false;
            }

            subscription.Status = "canceled";
            subscription.EndDate = subscription.NextBillingDate ?? DateTime.UtcNow.AddDays(30);
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling subscription");
            return false;
        }
    }

    public async Task<bool> RemovePaymentMethodAsync(int userId, string externalProductId)
    {
        try
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                          s.ExternalProductId == externalProductId &&
                                          s.Status == "active");

            if (subscription == null)
            {
                return false;
            }

            subscription.PaymentMethodId = null;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing payment method");
            return false;
        }
    }
}
