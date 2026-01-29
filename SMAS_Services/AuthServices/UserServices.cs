
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.Enums;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.AuthRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.AuthServices
{
    public  class UserServices : IUserServices
    {
        public readonly IUserRepositories _userRepositories;
        private readonly TokenService _tokenService;
        private readonly SmasDatabaseContext _context;


        public UserServices(IUserRepositories userRepositories , TokenService tokenService, SmasDatabaseContext context)
        {
            _userRepositories = userRepositories;
            _tokenService = tokenService;
            _context = context;
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

        

    }
}
