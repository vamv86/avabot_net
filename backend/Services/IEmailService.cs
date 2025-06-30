namespace AiAgentApi.Services;

public interface IEmailService
{
    Task<bool> SendWelcomeEmailAsync(string email, string name, string password);
    Task<bool> SendSubscriptionExpiryReminderAsync(string email, string name, int daysLeft);
    Task<bool> SendSubscriptionCanceledAsync(string email, string name);
}