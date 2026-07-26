using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Auth;
using Backend_Fincore.Application.Response;
using Backend_Fincore.Models;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IAuthService
    {
        Task<User> LoginAsync(LoginRequestDto dto);
        Task<SetupTwoFactorResponseDto> SetupTwoFactorAsync(SetupTwoFactorRequestDto dto);
        Task<AuthTokenResponseDto> VerifyTwoFactorAsync(VerifyTwoFactorRequestDto dto);
        Task<AuthTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task LogoutAsync(LogoutRequestDto dto);
        Task ResetTwoFactorAsync(ResetTwoFactorRequestDto dto);


        //Temp 
        Task<string> RegisterAsync(LoginDto registerDto);
    }
}