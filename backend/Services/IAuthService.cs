using AiAgentApi.DTOs;

namespace AiAgentApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> RegisterUserPaymentAsync(RegisterUserPaymentRequestDto request);
    string GenerateJwtToken(int userId, string email);
}