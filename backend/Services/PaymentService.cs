using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using AiAgentApi.Data;
using AiAgentApi.DTOs;
using AiAgentApi.Models;

namespace AiAgentApi.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IEmailService _emailService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        ApplicationDbContext context,
        IConfiguration configuration,
        HttpClient httpClient,
        IEmailService emailService,
        ILogger<PaymentService> logger)
    {
        _context = context;
        _configuration = configuration;
        _httpClient = httpClient;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> CreateBoldPaymentAsync(CreatePaymentRequestDto request)
    {
        try
        {
            // Bold.co API configuration
            var boldApiKey = _configuration["Bold:ApiKey"];
            var boldBaseUrl = _configuration["Bold:BaseUrl"] ?? "https://api.bold.co";
            var successUrl = _configuration["Bold:SuccessUrl"] ?? "http://localhost:3000/purchase?payment=success";
            var failureUrl = _configuration["Bold:FailureUrl"] ?? "http://localhost:3000/purchase?payment=failed";

            // Create payment request for Bold.co
            var paymentRequest = new
            {
                amount = request.Amount,
                currency = request.Currency,
                description = "AI Agent Pro - Monthly Subscription",
                customerEmail = request.Email,
                customerName = request.Name,
                customerPhone = request.WhatsApp,
                redirectUrl = successUrl,
                webhookUrl = $"{_configuration["App:BaseUrl"]}/api/payments/webhook",
                reference = Guid.NewGuid().ToString()
            };

            var jsonContent = JsonSerializer.Serialize(paymentRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", boldApiKey);

            var response = await _httpClient.PostAsync($"{boldBaseUrl}/payments", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var paymentResponse = JsonSerializer.Deserialize<dynamic>(responseContent);

                // For now, we'll simulate a successful payment URL
                // In a real implementation, you would parse the actual Bold.co response
                var paymentUrl = $"{boldBaseUrl}/checkout/{paymentRequest.reference}";

                return new PaymentResponseDto
                {
                    Success = true,
                    Message = "Payment created successfully",
                    PaymentUrl = paymentUrl,
                    PaymentId = paymentRequest.reference
                };
            }
            else
            {
                _logger.LogError($"Bold.co payment creation failed: {response.StatusCode}");
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Failed to create payment"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Bold payment");
            return new PaymentResponseDto
            {
                Success = false,
                Message = "An error occurred while creating the payment"
            };
        }
    }

    public async Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto webhook)
    {
        try
        {
            if (webhook.Status.ToLower() != "completed")
            {
                return false;
            }

            // Find or create user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == webhook.UserEmail.ToLower());

            if (user == null)
            {
                // Create user with temporary password
                var tempPassword = GenerateRandomPassword();
                user = new User
                {
                    Email = webhook.UserEmail.ToLower(),
                    Name = "New User", // This should ideally come from the payment data
                    WhatsApp = "", // This should ideally come from the payment data
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Send welcome email with credentials
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Name, tempPassword);
            }

            // Create or update subscription
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.Status == "active");

            if (subscription == null)
            {
                subscription = new Subscription
                {
                    UserId = user.Id,
                    Status = "active",
                    PlanName = "AI Agent Pro",
                    Amount = webhook.Amount,
                    Currency = webhook.Currency,
                    StartDate = DateTime.UtcNow,
                    NextBillingDate = DateTime.UtcNow.AddMonths(1),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Subscriptions.Add(subscription);
            }
            else
            {
                // Extend existing subscription
                subscription.NextBillingDate = DateTime.UtcNow.AddMonths(1);
                subscription.Status = "active";
                subscription.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment webhook");
            return false;
        }
    }

    public async Task<PaymentResponseDto> ProcessRecurringPaymentAsync(int subscriptionId)
    {
        try
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription == null)
            {
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Subscription not found"
                };
            }

            // For recurring payments, you would use Bold.co's recurring payment API
            // This is a simplified implementation
            var boldApiKey = _configuration["Bold:ApiKey"];
            var boldBaseUrl = _configuration["Bold:BaseUrl"] ?? "https://api.bold.co";

            var recurringPaymentRequest = new
            {
                amount = (int)(subscription.Amount * 100), // Convert to cents
                currency = subscription.Currency,
                customerId = subscription.User.Id.ToString(),
                description = $"AI Agent Pro - Monthly Subscription Renewal"
            };

            var jsonContent = JsonSerializer.Serialize(recurringPaymentRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", boldApiKey);

            var response = await _httpClient.PostAsync($"{boldBaseUrl}/payments/recurring", content);

            if (response.IsSuccessStatusCode)
            {
                // Update subscription
                subscription.NextBillingDate = DateTime.UtcNow.AddMonths(1);
                subscription.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new PaymentResponseDto
                {
                    Success = true,
                    Message = "Recurring payment processed successfully"
                };
            }
            else
            {
                _logger.LogError($"Bold.co recurring payment failed: {response.StatusCode}");
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = "Recurring payment failed"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing recurring payment");
            return new PaymentResponseDto
            {
                Success = false,
                Message = "An error occurred while processing the recurring payment"
            };
        }
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}