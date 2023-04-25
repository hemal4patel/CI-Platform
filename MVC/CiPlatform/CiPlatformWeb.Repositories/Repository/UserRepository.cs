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

        public void expireLink (string email, string token)
        {
            PasswordReset data = _db.PasswordResets.Where(p => p.Email== email && p.Token == token).FirstOrDefault();
            data.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

        public CmsPage GetCmsPage (long id)
        {
            return _db.CmsPages.Where(c => c.CmsPageId == id).FirstOrDefault();
        }

        public List<Banner> GetBanners ()
        {
            return _db.Banners.Where(b => b.DeletedAt == null).OrderByDescending(b => b.SortOrder).ToList();
        }

        
    }
}
