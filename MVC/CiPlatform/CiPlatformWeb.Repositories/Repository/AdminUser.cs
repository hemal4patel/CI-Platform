using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminUser : IAdminUser
    {
        private readonly ApplicationDbContext _db;

        public AdminUser(ApplicationDbContext db) {
            _db = db;
        }

        public List<User> GetUsers ()
        {
            List<User> users = _db.Users.ToList();
            return users;
        }
    }
}
