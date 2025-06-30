using System.Text;
using System.Text.Json;

namespace AiAgentApi.Services;

public class N8NService : IN8NService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<N8NService> _logger;

    public N8NService(HttpClient httpClient, IConfiguration configuration, ILogger<N8NService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> TriggerWorkflowAsync(string workflowId, object data)
    {
        try
        {
            var n8nBaseUrl = _configuration["N8N:BaseUrl"];
            var n8nApiKey = _configuration["N8N:ApiKey"];

            if (string.IsNullOrEmpty(n8nBaseUrl) || string.IsNullOrEmpty(n8nApiKey))
            {
                _logger.LogWarning("N8N configuration is incomplete. Workflow not triggered.");
                return false;
            }

            var webhookUrl = $"{n8nBaseUrl}/webhook/{workflowId}";
            
            var jsonContent = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Add API key if required
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", n8nApiKey);

            var response = await _httpClient.PostAsync(webhookUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("N8N workflow {WorkflowId} triggered successfully", workflowId);
                return true;
            }
            else
            {
                _logger.LogError("Failed to trigger N8N workflow {WorkflowId}. Status: {StatusCode}", 
                    workflowId, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering N8N workflow {WorkflowId}", workflowId);
            return false;
        }
    }

    public async Task<bool> NotifyUserRegistrationAsync(int userId, string email, string whatsapp)
    {
        var data = new
        {
            @event = "user_registered",
            userId = userId,
            email = email,
            whatsapp = whatsapp,
            timestamp = DateTime.UtcNow
        };

        return await TriggerWorkflowAsync("user-registration", data);
    }

    public async Task<bool> NotifySubscriptionChangeAsync(int userId, string status)
    {
        var data = new
        {
            @event = "subscription_changed",
            userId = userId,
            status = status,
            timestamp = DateTime.UtcNow
        };

        return await TriggerWorkflowAsync("subscription-change", data);
    }
}