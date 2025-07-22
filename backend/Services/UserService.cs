using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using BCrypt.Net;
using AiAgentApi.Data;
using AiAgentApi.DTOs;

namespace AiAgentApi.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, IEmailService emailService, ILogger<UserService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<DashboardDataDto> GetDashboardDataAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Subscriptions.Where(s => s.Status == "active"))
                    .ThenInclude(s => s.PaymentMethod)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new DashboardDataDto { Success = false };
            }

            var activeSubscription = user.Subscriptions
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();

            SubscriptionDto? subscriptionDto = null;
            if (activeSubscription != null)
            {
                subscriptionDto = new SubscriptionDto
                {
                    Id = activeSubscription.Id,
                    Status = activeSubscription.Status,
                    PlanName = activeSubscription.PlanName,
                    Amount = activeSubscription.Amount,
                    Currency = activeSubscription.Currency,
                    StartDate = activeSubscription.StartDate,
                    EndDate = activeSubscription.EndDate,
                    NextBillingDate = activeSubscription.NextBillingDate,
                    PaymentMethod = activeSubscription.PaymentMethod != null
                        ? new PaymentMethodDto
                        {
                            LastFour = activeSubscription.PaymentMethod.Last4 ?? "****",
                            ExpiryDate = activeSubscription.PaymentMethod.ExpiryMonth.HasValue && activeSubscription.PaymentMethod.ExpiryYear.HasValue
                                ? $"{activeSubscription.PaymentMethod.ExpiryMonth:00}/{activeSubscription.PaymentMethod.ExpiryYear}"
                                : "N/A",
                            Brand = activeSubscription.PaymentMethod.Brand ?? "Unknown"
                        }
                        : null
                };
            }

            var stats = new UsageStatsDto
            {
                MessagesThisMonth = 142,
                ResponsesThisMonth = 138,
                UptimePercentage = 99.9
            };

            return new DashboardDataDto
            {
                Success = true,
                Subscription = subscriptionDto,
                Stats = stats
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data for user {UserId}", userId);
            return new DashboardDataDto { Success = false };
        }
    }

    public async Task<AuthResponseDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Check if email is already taken by another user
            if (request.Email.ToLower() != user.Email.ToLower())
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != userId);

                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email is already taken by another user"
                    };
                }
            }

            // Check if WhatsApp is already taken by another user
            if (request.WhatsApp != user.WhatsApp)
            {
                var existingWhatsApp = await _context.Users
                    .FirstOrDefaultAsync(u => u.WhatsApp == request.WhatsApp && u.Id != userId);

                if (existingWhatsApp != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "WhatsApp number is already taken by another user"
                    };
                }
            }

            // Update user information
            user.Name = request.Name;
            user.Email = request.Email.ToLower();
            user.WhatsApp = request.WhatsApp;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Profile updated successfully",
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    WhatsApp = user.WhatsApp,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred while updating the profile"
            };
        }
    }

    public async Task<AuthResponseDto> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Current password is incorrect"
                };
            }

            // Validate new password
            if (request.NewPassword.Length < 6)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "New password must be at least 6 characters long"
                };
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Password changed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred while changing the password"
            };
        }
    }

    public async Task<ExportDataResponseDto> ExportUserDataAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Subscriptions)
                    .ThenInclude(s => s.PaymentMethod)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new ExportDataResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var exportData = new
            {
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.WhatsApp,
                    user.CreatedAt,
                    user.UpdatedAt
                },
                Subscriptions = user.Subscriptions.Select(s => new
                {
                    s.Id,
                    s.ExternalProductId,
                    s.ProductName,
                    s.PlanName,
                    s.Status,
                    s.Amount,
                    s.Currency,
                    s.StartDate,
                    s.EndDate,
                    s.NextBillingDate,
                    PaymentMethod = s.PaymentMethod != null
                        ? new
                        {
                            s.PaymentMethod.Provider,
                            s.PaymentMethod.Brand,
                            s.PaymentMethod.Last4,
                            Expiry = s.PaymentMethod.ExpiryMonth.HasValue && s.PaymentMethod.ExpiryYear.HasValue
                                ? $"{s.PaymentMethod.ExpiryMonth:00}/{s.PaymentMethod.ExpiryYear}"
                                : "N/A"
                        }
                        : null,
                    s.CreatedAt,
                    s.UpdatedAt
                }).ToList(),
                ExportedAt = DateTime.UtcNow
            };

            var jsonData = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var dataBytes = Encoding.UTF8.GetBytes(jsonData);

            return new ExportDataResponseDto
            {
                Success = true,
                Message = "Data exported successfully",
                Data = dataBytes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data for user {UserId}", userId);
            return new ExportDataResponseDto
            {
                Success = false,
                Message = "An error occurred while exporting data"
            };
        }
    }

    public async Task<AuthResponseDto> DeleteAccountAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Cancel active subscriptions
            var activeSubscriptions = user.Subscriptions.Where(s => s.Status == "active").ToList();
            foreach (var subscription in activeSubscriptions)
            {
                subscription.Status = "canceled";
                subscription.EndDate = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;
            }

            // Send account deletion notification email
            await _emailService.SendSubscriptionCanceledAsync(user.Email, user.Name);

            // Remove user and all related data (cascading delete will handle subscriptions)
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Account deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account for user {UserId}", userId);
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred while deleting the account"
            };
        }
    }
}