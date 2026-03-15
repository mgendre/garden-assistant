using GardenAssistant.Data.Entities;

namespace GardenAssistant.Services.Interfaces;

public interface IAuthService
{
    Task<(string accessToken, string refreshToken)> GetDevelopmentTokenAsync();
    Task<(string accessToken, string refreshToken)?> RefreshAsync(string refreshToken);
}
