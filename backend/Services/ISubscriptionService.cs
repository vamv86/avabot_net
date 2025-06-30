using AiAgentApi.DTOs;

namespace AiAgentApi.Services;

public interface ISubscriptionService
{
    Task<PaymentResponseDto> RenewSubscriptionAsync(int userId);
    Task<bool> CancelSubscriptionAsync(int userId);
    Task<bool> RemovePaymentMethodAsync(int userId);
}