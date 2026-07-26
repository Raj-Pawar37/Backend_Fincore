using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Auth;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Application.Response;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend_Fincore.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableRateLimiting("fixed")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var user = await _authService.LoginAsync(dto);

            var response = new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Username and password verified successfully.",
                Data = new LoginResponseDto
                {
                    UserId = user.UserId,
                    Is2FAEnabled = user.Is2FAEnabled,
                    Requires2FA = true,
                    Message = user.Is2FAEnabled ? "Enter OTP from authenticator app." : "Two-factor authentication setup is required."
                }
            };

            return Ok(response);

        }


        [HttpPost("setup2fa")]
        public async Task<IActionResult> SetupTwoFactor(SetupTwoFactorRequestDto dto)
        {
            var response = await _authService.SetupTwoFactorAsync(dto);

            return Ok(new ApiResponse<SetupTwoFactorResponseDto>
            {
                Success = true,
                Message = "QR Code generated successfully.",
                Data = response,
            });
        }


        [HttpPost("verify2fa")]
        public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorRequestDto dto)
        {
            var tokens = await _authService.VerifyTwoFactorAsync(dto);

            SetRefreshTokenCookie(tokens.RefreshToken, tokens.RefreshTokenExpiry);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Two-factor authentication verified successfully.",
                Data = new
                {
                    tokens.AccessToken,
                    tokens.AccessTokenExpiry
                }
            });
        }


        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken)) throw new UnauthorizedAccessException("Refresh token cookie is missing.");

            var dto = new RefreshTokenRequestDto() { RefreshToken = refreshToken };
            var tokens = await _authService.RefreshTokenAsync(dto);

            SetRefreshTokenCookie(tokens.RefreshToken, tokens.RefreshTokenExpiry);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Access token refreshed successfully.",
                Data = new
                {
                    tokens.AccessToken,
                    tokens.AccessTokenExpiry
                }
            });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var dto = new LogoutRequestDto() { RefreshToken = refreshToken };
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(dto);
            }

            DeleteRefreshTokenCookie();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logged out successfully.",
                Data = null
            });
        }


        [Authorize]
        [HttpPost("reset2fa")]
        public async Task<IActionResult> ResetTwoFactor(ResetTwoFactorRequestDto dto)
        {

            await _authService.ResetTwoFactorAsync(dto);
            DeleteRefreshTokenCookie();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Two-factor authentication reset successfully. Please log in and configure it again.",
                Data = null
            });
        }



        //Temp 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginDto registerDto)
        {

            var result = await _authService.RegisterAsync(registerDto);
            return Ok(result);

        }
















        // Helper Fucntions 
        private void SetRefreshTokenCookie(string refreshToken, DateTime refreshTokenExpiry)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenExpiry,
                Path = "/api/auth"
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }


        private void DeleteRefreshTokenCookie()
        {
            Response.Cookies.Delete("refreshToken",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/auth"
                });
        }

    }
}