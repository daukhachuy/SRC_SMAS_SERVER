using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class AuthDAO
    {
        private readonly SmasDatabaseContext _context;

        public  AuthDAO(SmasDatabaseContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
              return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }


    }
}
