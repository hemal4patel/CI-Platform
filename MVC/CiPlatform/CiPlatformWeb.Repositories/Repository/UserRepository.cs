using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository (ApplicationDbContext db) 
        {
            _db = db;
        }

        public void RegisterUser (User obj)
        {
            _db.Add(obj);
            _db.SaveChanges();
        }

        public User CheckUser (User obj)
        {
            return _db.Users.FirstOrDefault(u => u.Email == obj.Email);
        }

    }
}
