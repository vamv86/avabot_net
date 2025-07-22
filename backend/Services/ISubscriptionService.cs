using AiAgentApi.DTOs;

namespace AiAgentApi.Services;

public interface ISubscriptionService
{
    Task<PaymentResponseDto> RenewSubscriptionAsync(int userId, string externalProductId);
    Task<bool> CancelSubscriptionAsync(int userId, string externalProductId);
    Task<bool> RemovePaymentMethodAsync(int userId, string externalProductId);
}
