using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.Enums;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.AuthRepositories;
using SMAS_Services.EmailServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.AuthServices
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepositories _userRepositories;
        private readonly TokenService _tokenService;
        private readonly RestaurantDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        private const string OtpCacheKeyPrefix = "ForgotPasswordOtp:";
        private static readonly TimeSpan OtpExpiry = TimeSpan.FromMinutes(5);

        public UserServices(
            IUserRepositories userRepositories,
            TokenService tokenService,
            RestaurantDbContext context,
            IMemoryCache cache,
            IEmailService emailService)
        {
            _userRepositories = userRepositories;
            _tokenService = tokenService;
            _context = context;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {

            var user = await _userRepositories.GetByUsernameAsync(request.Email);

            if (user == null)
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_001.ToString()
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_002.ToString()
                };
            }

            var token = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                MsgCode = MSGCode.MSG_003.ToString()
            };
        }

        public async Task<LoginResponse> LoginGoogleAsync(string  email)
        {
            var user = await _userRepositories.GetByUsernameAsync(email);
            if (user == null)
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_001.ToString()
                };
            }

            var token = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                MsgCode = MSGCode.MSG_003.ToString()
            };
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Email &&
                u.IsDeleted == false
            );

            if (existingUser != null)
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_005.ToString() // Email exists
                };
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                Fullname = request.Fullname,
                Role = "Customer",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,

                PasswordHash = passwordHash,
                PasswordSalt = string.Empty // BCrypt đã gộp salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                MsgCode = MSGCode.MSG_003.ToString() // Register success
            };
        }
        public async Task<LoginResponse> RegisterGoogleAsync(string email, string fullname)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email && u.IsDeleted == false
            );

            // ĐÃ TỒN TẠI → KHÔNG CHO REGISTER
            if (existingUser != null)
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_005.ToString() // Email exists
                };
            }

            var user = new User
            {
                Email = email,
                Fullname = fullname,
                Role = "Customer",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,

                PasswordHash = string.Empty,
                PasswordSalt = string.Empty
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                MsgCode = MSGCode.MSG_003.ToString() // Register success
            };
        }

        public async Task<LoginResponse> SendForgotPasswordOtpAsync(string email)
        {
            var user = await _userRepositories.GetByUsernameAsync(email);
            if (user == null || user.IsDeleted == true)
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_001.ToString() // Email không tồn tại
                };
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var cacheKey = OtpCacheKeyPrefix + email.Trim().ToLowerInvariant();
            _cache.Set(cacheKey, otp, OtpExpiry);

            await _emailService.SendOtpEmailAsync(user.Email!, otp);

            return new LoginResponse
            {
                Token = null,
                MsgCode = MSGCode.MSG_006.ToString() // OTP đã gửi
            };
        }

        public Task<LoginResponse> VerifyOtpAsync(string email, string otp)
        {
            var cacheKey = OtpCacheKeyPrefix + email.Trim().ToLowerInvariant();
            if (!_cache.TryGetValue(cacheKey, out string? storedOtp) || storedOtp != otp)
            {
                return Task.FromResult(new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_007.ToString() // OTP sai hoặc hết hạn
                });
            }

            return Task.FromResult(new LoginResponse
            {
                Token = null,
                MsgCode = MSGCode.MSG_009.ToString() // Xác minh OTP thành công 
            });
        }

        public async Task<LoginResponse> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var cacheKey = OtpCacheKeyPrefix + email.Trim().ToLowerInvariant();
            if (!_cache.TryGetValue(cacheKey, out string? storedOtp) || storedOtp != otp)
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_007.ToString() // OTP sai hoặc hết hạn
                };
            }

            var user = await _userRepositories.GetActiveUserByEmailAsync(email);
            if (user == null)
            {
                return new LoginResponse
                {
                    Token = null,
                    MsgCode = MSGCode.MSG_001.ToString()
                };
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepositories.UpdatePasswordAsync(user.UserId, passwordHash);

            _cache.Remove(cacheKey);

            return new LoginResponse
            {
                Token = null,
                MsgCode = MSGCode.MSG_008.ToString() // Đổi mật khẩu thành công
            };
        }
    }
}
