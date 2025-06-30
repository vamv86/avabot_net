namespace AiAgentApi.Services;

public interface IN8NService
{
    Task<bool> TriggerWorkflowAsync(string workflowId, object data);
    Task<bool> NotifyUserRegistrationAsync(int userId, string email, string whatsapp);
    Task<bool> NotifySubscriptionChangeAsync(int userId, string status);
}