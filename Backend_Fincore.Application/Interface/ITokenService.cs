using Backend_Fincore.Models;
using System.Security.Claims;

namespace Backend_Fincore.Application.Interface
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user); // User model aapke Domain me hona chahiye
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}