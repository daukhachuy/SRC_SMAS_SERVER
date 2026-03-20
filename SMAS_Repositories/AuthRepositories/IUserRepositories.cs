
    using SMAS_BusinessObject.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace SMAS_Repositories.AuthRepositories
    {
        public interface IUserRepositories
        {
            Task<User?> GetByUsernameAsync(string email);
            Task<User?> GetActiveUserByEmailAsync(string email);
            Task UpdatePasswordAsync(int userId, string passwordHash);
            Task CreateAsync(User user);
            Task<User?> GetByIdAsync(int userId);
            Task UpdateProfileAsync(User user);
            Task<bool> UpdateStatusUserAsync(int userId);
        }
    }
