using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.AuthRepositories
{
    public class UserRepositories : IUserRepositories
    {
        private readonly AuthDAO _authDAO;

        public UserRepositories(AuthDAO authDAO)
        {
            _authDAO = authDAO;
        }
        public async Task<User?> GetByUsernameAsync(string email) { 
            return await _authDAO.GetByEmailAsync(email);
        }
    }
}
