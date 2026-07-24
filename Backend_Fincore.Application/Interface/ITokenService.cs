using Backend_Fincore.Models;
using System.Security.Claims;

namespace Backend_Fincore.Application.Interface
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user); 
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}