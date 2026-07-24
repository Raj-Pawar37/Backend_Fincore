using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Application.Response;
using Backend_Fincore.Infrastructure.Service;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend_Fincore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);

                Response.Cookies.Append("accessToken", response.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = response.RefreshTokenExpiry
                });

                var authData = new AuthResponseDto
                {
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
                    RefreshTokenExpiry = response.RefreshTokenExpiry
                };

                return Ok(new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = authData
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Authentication failed",
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDto tokenRequestDto)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(tokenRequestDto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Token refresh failed",
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid request",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Registration successful",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Registration failed",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("sub")?.Value;

                if (int.TryParse(userIdClaim, out int userId))
                {
                    await _authService.LogoutAsync(userId);
                }

                Response.Cookies.Delete("accessToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logged out successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during logout",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("GenerateOTP")]
        public async Task<IActionResult> GenerateQRCode([FromBody] GenerateOtpRequest request)
        {
            var result = await _authService.GenerateQRCode(request.Email);

            if (string.IsNullOrEmpty(result))
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found or QR generation failed."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "QR CODE DONE",
                Data = new { qrCode = result }
            });
        }

        [HttpPost("verifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOtpRequest request)
        {
            var result = await _authService.VerifyOTP(request.Email, request.Otp);

            if (string.IsNullOrEmpty(result))
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found or 2FA not initialized."
                });
            }

            if (result == "invalid otp")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid OTP provided.",
                    Error = "invalid_otp"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "OTP verified successfully",
                Data = result
            });
        }
    }

    public class GenerateOtpRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}