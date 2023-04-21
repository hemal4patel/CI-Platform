using BCrypt.Net;
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

        public void RegisterUser (RegistrationValidation obj)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(obj.Password);
            var newUser = new User()
            {
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email,
                Password = hashedPassword,
                PhoneNumber = obj.PhoneNumber,
                CreatedAt = DateTime.Now,
            };
            _db.Users.Add(newUser);
            _db.SaveChanges();
        }

        public User CheckUser (string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email);
        }

        public void UpdatePassword (ResetPasswordValidation obj)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(obj.Password);
            var x = _db.Users.FirstOrDefault(e => e.Email == obj.Email);
            x.Password = hashedPassword;
            x.UpdatedAt = DateTime.Now;
            _db.Users.Update(x);
            _db.SaveChanges();
        }

    }
}
