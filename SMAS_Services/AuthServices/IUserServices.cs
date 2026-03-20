using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.DTOs.Profile;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.AuthServices
{
    public interface IUserServices
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<LoginResponse> LoginGoogleAsync(string  email);
        Task<LoginResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> RegisterGoogleAsync(string email, string fullname);

        Task<LoginResponse> SendForgotPasswordOtpAsync(string email);

        Task<LoginResponse> VerifyOtpAsync(string email, string otp);

        Task<LoginResponse> ResetPasswordAsync(string email, string otp, string newPassword);
        Task<LoginResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<CustomerDetailResponse?> GetUserProfileAsync(int userId);
        Task<bool> UpdateStatusUserAsync(int userId);
    }
}
