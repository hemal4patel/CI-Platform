using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Http;
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

        public User CheckUser (string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email);
        }

        public void UpdatePassword (ResetPasswordValidation obj)
        {
            var x = _db.Users.FirstOrDefault(e => e.Email == obj.Email);
            x.Password = obj.Password;
            _db.Users.Update(x);
            _db.SaveChanges();
        }

    }
}
