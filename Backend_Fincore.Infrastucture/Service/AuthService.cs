using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Auth;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Application.Response;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using QRCoder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend_Fincore.Infrastucture.Service
{
    public class AuthService : IAuthService
    {

        private readonly AppDbContext db;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService currentUser;

        public AuthService(AppDbContext db, IConfiguration configuration, ICurrentUserService currentUser)
        {
            this.db = db;
            _configuration = configuration;
            this.currentUser = currentUser;
        }


        public async Task<User> LoginAsync(LoginRequestDto dto)
        {

            var user = await db.User.FirstOrDefaultAsync(x => x.Username == dto.Username);
            if (user == null) throw new UnauthorizedAccessException("userName or Password doesnt matched");
            if (user.IsActive == 0) throw new UnauthorizedAccessException("User has been deactived due to multiple Password Try");

            bool match = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!match)
            {
                user.FailedLoginAttempts += 1;

                if (user.FailedLoginAttempts >= 3)
                {
                    user.IsActive = 0;
                    user.ModifiedAt = DateTime.Now;
                    user.ModifiedBy = user.UserId;
                    await db.SaveChangesAsync();
                    throw new UnauthorizedAccessException($"{user.Username} has been deactived due multiple Password");
                }

                int remainingAttempts = 3 - user.FailedLoginAttempts;
                user.ModifiedAt = DateTime.Now;
                user.ModifiedBy = user.UserId;
                await db.SaveChangesAsync();
                throw new UnauthorizedAccessException($"userName or Password doesnt matched {remainingAttempts} attempts left");


            }

            user.FailedLoginAttempts = 0;
            user.ModifiedAt = DateTime.Now;
            user.ModifiedBy = user.UserId;
            await db.SaveChangesAsync();


            return user;


        }

        public async Task<SetupTwoFactorResponseDto> SetupTwoFactorAsync(SetupTwoFactorRequestDto dto)
        {

            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == dto.UserId);
            if (user == null) throw new UnauthorizedAccessException("User not found");
            if (user.IsActive == 0) throw new UnauthorizedAccessException("User has been deactived");
            if (user.Is2FAEnabled) throw new UnauthorizedAccessException("2FA has been already done");

            var key = KeyGeneration.GenerateRandomKey(20);
            var secretKey = Base32Encoding.ToString(key);

            var otpAuthUrl = $"otpauth://totp/BackendFincore:{Uri.EscapeDataString(user.Email)}" + $"?secret={secretKey}&issuer=BackendFincore";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(otpAuthUrl, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            user.TotpSecretKey = secretKey;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = user.UserId;

            await db.SaveChangesAsync();

            File.WriteAllBytes(@"D:\temp\QRCode.png", qrCodeBytes);
            return new SetupTwoFactorResponseDto
            {
                UserId = user.UserId,
                QrCodeBase64 = Convert.ToBase64String(qrCodeBytes),
                Message = "Scan this QR code using Google Authenticator."
            };


        }

        public async Task<AuthTokenResponseDto> VerifyTwoFactorAsync(VerifyTwoFactorRequestDto dto)
        {
            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == dto.UserId);
            if (user == null) throw new KeyNotFoundException("User Not Found");
            if (user.IsActive == 0) throw new UnauthorizedAccessException("User ID has been Deactiveted");

            byte[] secretKey;
            try
            {
                secretKey = Base32Encoding.ToBytes(user.TotpSecretKey);
            }
            catch
            {
                throw new InvalidOperationException("Invalid two-factor authentication configuration.");
            }

            var totp = new Totp(secretKey);
            bool isValidOtp = totp.VerifyTotp(dto.Otp, out long timeStepMatched, new VerificationWindow(previous: 1, future: 1));

            if (!isValidOtp) throw new UnauthorizedAccessException("Invalid or expired authentication code.");

            user.Is2FAEnabled = true;
            user.LastLoginDate = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = user.UserId;




            //refresh Token 
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            string accessToken = GenerateAccessToken(user, accessTokenExpiry);
            string refreshToken = GenerateRefreshToken();

            var existingToken = await db.UserToken.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.TokenType == "RefreshToken");

            if (existingToken != null)
            {
                existingToken.Token = refreshToken;
                existingToken.ExpiryDate = refreshTokenExpiry;
                existingToken.IsActive = 1;
                existingToken.ModifiedAt = DateTime.UtcNow;
                existingToken.ModifiedBy = user.UserId;
            }
            else
            {
                await db.UserToken.AddAsync(new UserToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    TokenType = "RefreshToken",
                    ExpiryDate = refreshTokenExpiry,
                    IsActive = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.UserId
                });
            }

            await db.SaveChangesAsync();


            return new AuthTokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry
            };


        }


        public async Task<AuthTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken)) throw new UnauthorizedAccessException("Refresh TOken not Provided");

            var storedToken = await db.UserToken.Include(x => x.User).FirstOrDefaultAsync(x => x.Token == dto.RefreshToken && x.TokenType == "RefreshToken");
            if (storedToken == null) throw new UnauthorizedAccessException("Refresh Token Doent found");
            if (storedToken?.IsActive == 0) throw new UnauthorizedAccessException("Refresh Token is InActive");
            if (storedToken?.ExpiryDate <= DateTime.UtcNow)
            {
                storedToken.IsActive = 0;
                storedToken.ModifiedAt = DateTime.UtcNow;
                storedToken.ModifiedBy = storedToken.UserId;
                await db.SaveChangesAsync();

                throw new UnauthorizedAccessException("Refresh token has expired. Please log in again.");
            }

            var user = storedToken?.User;

            if (user == null) throw new KeyNotFoundException("User not found.");
            if (user.IsActive != 1) throw new UnauthorizedAccessException("User account is inactive.");



            //Tokens 

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var newAccessToken = GenerateAccessToken(user, accessTokenExpiry);
            var newRefreshToken = GenerateRefreshToken();

            storedToken.Token = newRefreshToken;
            storedToken.ExpiryDate = refreshTokenExpiry;
            storedToken.IsActive = 1;
            storedToken.ModifiedAt = DateTime.UtcNow;
            storedToken.ModifiedBy = user.UserId;

            await db.SaveChangesAsync();

            return new AuthTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry
            };

        }






        public async Task LogoutAsync(LogoutRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken)) return;

            var storedToken = await db.UserToken.FirstOrDefaultAsync(x => x.Token == dto.RefreshToken && x.TokenType == "RefreshToken");
            if (storedToken == null) return;


            storedToken.IsActive = 0;
            storedToken.ModifiedAt = DateTime.UtcNow;
            storedToken.ModifiedBy = storedToken.UserId;

            await db.SaveChangesAsync();

        }

        public async Task ResetTwoFactorAsync(ResetTwoFactorRequestDto dto)
        {
            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == currentUser.UserId);
            if (user == null) throw new KeyNotFoundException("User not found.");
            if (user.IsActive != 1) throw new UnauthorizedAccessException("User account is inactive.");
            if (!user.Is2FAEnabled && string.IsNullOrWhiteSpace(user.TotpSecretKey)) throw new InvalidOperationException("Two-factor authentication is not enabled.");


            bool passwordMatched = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            if (!passwordMatched) throw new UnauthorizedAccessException("Current password is incorrect.");

            // Remove the old authenticator configuration
            user.Is2FAEnabled = false;
            user.TotpSecretKey = null;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = currentUser.UserId;

            var userToken = await db.UserToken.FirstOrDefaultAsync(x => x.UserId == currentUser.UserId && x.TokenType == "RefreshToken");
            if (userToken != null)
            {
                userToken.IsActive = 0;
                userToken.ModifiedAt = DateTime.UtcNow;
                userToken.ModifiedBy = currentUser.UserId;
            }

            await db.SaveChangesAsync();



        }













        //Temp 
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
                RoleId = 2,               // Employee RoleId
                MasterId = 1,             // Temporary or actual EmployeeId
                MasterType = "Employee",

                Username = username,
                PasswordHash = passwordHash,
                Email = $"{username}@domain.com",

                FailedLoginAttempts = 0,
                IsEmailVerified = 0,
                IsActive = 1,
                CreatedAt = DateTime.UtcNow
            };
            db.User.Add(user);

            await db.SaveChangesAsync();

            return "User Registered Successfully.";
        }


        public async Task<AuthTokenResponseDto> DeveloperLoginAsync(LoginRequestDto dto)
        {
            var user = await LoginAsync(dto);
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var accessToken = GenerateAccessToken(user, accessTokenExpiry);
            var refreshToken = GenerateRefreshToken();

            var existingToken = await db.UserToken.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.TokenType == "RefreshToken");

            if (existingToken != null)
            {
                existingToken.Token = refreshToken;
                existingToken.ExpiryDate = refreshTokenExpiry;
                existingToken.IsActive = 1;
                existingToken.ModifiedAt = DateTime.UtcNow;
                existingToken.ModifiedBy = user.UserId;
            }
            else
            {
                await db.UserToken.AddAsync(new UserToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    TokenType = "RefreshToken",
                    ExpiryDate = refreshTokenExpiry,
                    IsActive = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.UserId
                });
            }

            user.LastLoginDate = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return new AuthTokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry
            };
        }




























        //Helper Functions 
        private string GenerateAccessToken(User user, DateTime expiry)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("MasterId", user.MasterId.ToString()),
                new Claim("MasterType", user.MasterType)
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private static string GenerateRefreshToken()
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }


    }
}

