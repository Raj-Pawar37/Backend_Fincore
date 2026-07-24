using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Application.Response;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BCrypt.Net;

namespace Backend_Fincore.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext db;
        private readonly ITokenService tokenService;

      
        private static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(1);

        public AuthService(AppDbContext db, ITokenService tokenService)
        {
            this.db = db;
            this.tokenService = tokenService;
        }

       
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null ||
                string.IsNullOrWhiteSpace(loginDto.Username) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new ArgumentException("Username and Password are required.");
            }

            var username = loginDto.Username.Trim();

         
            var user = await db.User
                .FirstOrDefaultAsync(x =>
                    x.Username == username &&
                    x.IsActive == 1);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid Username.");

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!passwordMatch)
                throw new UnauthorizedAccessException("Invalid Password.");

            
            string accessToken = tokenService.GenerateAccessToken(user);
            string refreshToken = tokenService.GenerateRefreshToken();

            DateTime expiryDate = DateTime.UtcNow.Add(RefreshTokenExpiry);

            var existingToken = await db.UserToken
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.UserId &&
                    x.TokenType == "RefreshToken");

            if (existingToken == null)
            {
                UserToken token = new UserToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    TokenType = "RefreshToken",
                    ExpiryDate = expiryDate,
                    IsActive = 1,
                    CreatedBy = user.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                db.UserToken.Add(token);
            }
            else
            {
                existingToken.Token = refreshToken;
                existingToken.ExpiryDate = expiryDate;
                existingToken.IsActive = 1;
                existingToken.ModifiedAt = DateTime.UtcNow;
                existingToken.ModifiedBy = user.UserId;

                db.UserToken.Update(existingToken);
            }

            await db.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = expiryDate
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto)
        {
            if (tokenRequestDto == null ||
                string.IsNullOrWhiteSpace(tokenRequestDto.AccessToken) ||
                string.IsNullOrWhiteSpace(tokenRequestDto.RefreshToken))
            {
                throw new ArgumentException("Invalid Request.");
            }

            ClaimsPrincipal principal;

            try
            {
                principal = tokenService.GetPrincipalFromExpiredToken(tokenRequestDto.AccessToken);
            }
            catch
            {
                throw new ArgumentException("Invalid Access Token.");
            }

            int userId = Convert.ToInt32(
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var refreshToken = await db.UserToken
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Token == tokenRequestDto.RefreshToken &&
                    x.TokenType == "RefreshToken" &&
                    x.IsActive == 1);

            if (refreshToken == null)
                throw new UnauthorizedAccessException("Invalid Refresh Token.");

            if (refreshToken.ExpiryDate <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh Token Expired.");

            var user = await db.User.FindAsync(userId);

            if (user == null || user.IsActive != 1)
                throw new UnauthorizedAccessException("User Not Found.");

            string newAccessToken = tokenService.GenerateAccessToken(user);
            string newRefreshToken = tokenService.GenerateRefreshToken();

            DateTime newExpiryDate = DateTime.UtcNow.Add(RefreshTokenExpiry);

            refreshToken.Token = newRefreshToken;
            refreshToken.ExpiryDate = newExpiryDate;
            refreshToken.IsActive = 1;
            refreshToken.ModifiedAt = DateTime.UtcNow;
            refreshToken.ModifiedBy = userId;

            db.UserToken.Update(refreshToken);

            await db.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = newExpiryDate
            };
        }


        public async Task<string> RegisterAsync(LoginDto registerDto)
        {
            if (registerDto == null ||
                string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Password))
            {
                throw new ArgumentException("Username and Password are required.");
            }

            string username = registerDto.Username.Trim();

            bool exists = await db.User
                .AnyAsync(x => x.Username == username);

            if (exists)
                throw new Exception("Username already exists.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            User user = new User
            {
                RoleId = 2,
                MasterId = 1,
                MasterType ="Employee",
                Username = username,
                PasswordHash = passwordHash,
                
                Email = $"{username}@domain.com",
                IsEmailVerified = 0,
                FailedLoginAttempts = 0,
                IsActive = 1,
                CreatedAt = DateTime.UtcNow
            };

            db.User.Add(user);

            await db.SaveChangesAsync();

            return "User Registered Successfully.";
        }
    }
}