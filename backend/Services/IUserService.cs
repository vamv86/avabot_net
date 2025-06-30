using AiAgentApi.DTOs;

namespace AiAgentApi.Services;

public interface IUserService
{
    Task<DashboardDataDto> GetDashboardDataAsync(int userId);
    Task<AuthResponseDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
    Task<AuthResponseDto> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    Task<ExportDataResponseDto> ExportUserDataAsync(int userId);
    Task<AuthResponseDto> DeleteAccountAsync(int userId);
}