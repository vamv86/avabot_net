using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using AiAgentApi.Data;
using AiAgentApi.DTOs;
using AiAgentApi.Models;
using System.Net.Http;

namespace AiAgentApi.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(
        ApplicationDbContext context,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            var token = GenerateJwtToken(user.Id, user.Email);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
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
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            // Check if WhatsApp number already exists
            var existingWhatsApp = await _context.Users
                .FirstOrDefaultAsync(u => u.WhatsApp == request.WhatsApp);

            if (existingWhatsApp != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User with this WhatsApp number already exists"
                };
            }

            // Create new user
            var user = new User
            {
                Name = request.Name,
                Email = request.Email.ToLower(),
                WhatsApp = request.WhatsApp,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully",
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
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during registration"
            };
        }
    }


    public async Task<AuthResponseDto> RegisterUserPaymentAsync(RegisterUserPaymentRequestDto request)
    {
        try
        {
            // Find or create user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                // Create user with temporary password
                var tempPassword = GenerateRandomPassword();
                user = new User
                {
                    Email = request.Email.ToLower(),
                    Name = request.Name.ToLower(), // This should ideally come from the payment data
                    WhatsApp = request.WhatsApp.ToLower(), // This should ideally come from the payment data
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Send welcome email with credentials
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Name, tempPassword);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User registered successfully",
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
            else
            {
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User with this WhatsApp number already exists"
                };

            }    

        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "An error occurred during registration"
            };
        }
    }

    public string GenerateJwtToken(int userId, string email)
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}