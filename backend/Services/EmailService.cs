using MailKit.Net.Smtp;
using MimeKit;

namespace AiAgentApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendWelcomeEmailAsync(string email, string name, string password)
    {
        try
        {
            var subject = "Welcome to AI Agent - Your Account is Ready!";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #003B49;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #003B49, #F37021); padding: 30px; border-radius: 10px; text-align: center; margin-bottom: 30px;'>
                            <h1 style='color: white; margin: 0; font-size: 28px;'>Welcome to AI Agent!</h1>
                        </div>
                        
                        <h2>Hello {name},</h2>
                        
                        <p>Thank you for your purchase! Your AI Agent account has been successfully created and your subscription is now active.</p>
                        
                        <div style='background: #FAFAFA; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #F37021;'>Your Login Credentials:</h3>
                            <p><strong>Email:</strong> {email}</p>
                            <p><strong>Password:</strong> {password}</p>
                        </div>
                        
                        <p>You can now log in to your dashboard to:</p>
                        <ul>
                            <li>View your subscription status</li>
                            <li>Monitor your AI agent usage</li>
                            <li>Manage your account settings</li>
                            <li>Access support resources</li>
                        </ul>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='http://localhost:3000/login' style='background: #F37021; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;'>Login to Dashboard</a>
                        </div>
                        
                        <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                        
                        <p>Best regards,<br>The AI Agent Team</p>
                        
                        <hr style='border: none; border-top: 1px solid #CCCCCC; margin: 30px 0;'>
                        <p style='font-size: 12px; color: #666;'>This email was sent because you purchased an AI Agent subscription. If you didn't make this purchase, please contact support immediately.</p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendSubscriptionExpiryReminderAsync(string email, string name, int daysLeft)
    {
        try
        {
            var subject = $"AI Agent Subscription Expires in {daysLeft} Days";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #003B49;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #003B49, #F37021); padding: 30px; border-radius: 10px; text-align: center; margin-bottom: 30px;'>
                            <h1 style='color: white; margin: 0; font-size: 28px;'>Subscription Reminder</h1>
                        </div>
                        
                        <h2>Hello {name},</h2>
                        
                        <p>This is a friendly reminder that your AI Agent subscription will expire in <strong>{daysLeft} days</strong>.</p>
                        
                        <p>To ensure uninterrupted service, please renew your subscription before it expires.</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='http://localhost:3000/dashboard' style='background: #F37021; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;'>Renew Subscription</a>
                        </div>
                        
                        <p>If you have any questions, please contact our support team.</p>
                        
                        <p>Best regards,<br>The AI Agent Team</p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send expiry reminder email to {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendSubscriptionCanceledAsync(string email, string name)
    {
        try
        {
            var subject = "AI Agent Subscription Canceled";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #003B49;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: #003B49; padding: 30px; border-radius: 10px; text-align: center; margin-bottom: 30px;'>
                            <h1 style='color: white; margin: 0; font-size: 28px;'>Subscription Canceled</h1>
                        </div>
                        
                        <h2>Hello {name},</h2>
                        
                        <p>We're sorry to see you go! Your AI Agent subscription has been successfully canceled.</p>
                        
                        <p>Your service will remain active until the end of your current billing period.</p>
                        
                        <p>If you change your mind, you can reactivate your subscription at any time from your dashboard.</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='http://localhost:3000/dashboard' style='background: #F37021; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;'>Go to Dashboard</a>
                        </div>
                        
                        <p>Thank you for using AI Agent. We hope to serve you again in the future!</p>
                        
                        <p>Best regards,<br>The AI Agent Team</p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cancellation email to {Email}", email);
            return false;
        }
    }

    private async Task<bool> SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            var emailSettings = _configuration.GetSection("Email");
            var smtpHost = emailSettings["Host"];
            var smtpPort = int.Parse(emailSettings["Port"] ?? "587");
            var smtpUser = emailSettings["Username"];
            var smtpPass = emailSettings["Password"];
            var fromName = emailSettings["FromName"] ?? "AI Agent";
            var fromEmail = emailSettings["FromEmail"] ?? smtpUser;

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
            {
                _logger.LogWarning("Email configuration is incomplete. Email not sent.");
                return false;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            return false;
        }
    }
}