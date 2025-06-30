using AiAgentApi.DTOs;

namespace AiAgentApi.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreateBoldPaymentAsync(CreatePaymentRequestDto request);
    Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto webhook);
    Task<PaymentResponseDto> ProcessRecurringPaymentAsync(int subscriptionId);
}