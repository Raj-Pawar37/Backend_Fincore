using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Response;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto);
        Task<string> RegisterAsync(LoginDto registerDto);

           Task LogoutAsync(int id);
    }
}