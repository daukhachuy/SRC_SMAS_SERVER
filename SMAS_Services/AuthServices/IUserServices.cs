using SMAS_BusinessObject.DTOs.Auth;
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

    }
}
