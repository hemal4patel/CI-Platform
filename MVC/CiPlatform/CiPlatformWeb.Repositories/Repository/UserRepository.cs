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
            byte[] encData_byte = new byte[obj.Password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(obj.Password);
            string encodedData = Convert.ToBase64String(encData_byte);           

            User newUser = new User()
            {
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email,
                Password = encodedData,
                PhoneNumber = obj.PhoneNumber,
                Role = "user",
                CreatedAt = DateTime.Now,
            };
            _db.Users.Add(newUser);
            _db.SaveChanges();
        }

        public User CheckUser (string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email && u.DeletedAt == null);
        }

        public void UpdatePassword (ResetPasswordValidation obj)
        {
            byte[] encData_byte = new byte[obj.Password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(obj.Password);
            string encodedData = Convert.ToBase64String(encData_byte);

            User x = _db.Users.FirstOrDefault(e => e.Email == obj.Email);
            x.Password = encodedData;
            x.UpdatedAt = DateTime.Now;
            _db.Users.Update(x);
            _db.SaveChanges();
        }

        public bool verifyPassword (string objPassword, string userPassword)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(userPassword);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);

            if(objPassword.Equals(result))
            {
                return true;
            }
            return false;
        }


        public void expireLink (string email, string token)
        {
            PasswordReset data = _db.PasswordResets.Where(p => p.Email == email && p.Token == token).FirstOrDefault();
            data.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

        public CmsPage GetCmsPage (long id)
        {
            return _db.CmsPages.Where(c => c.CmsPageId == id && c.Status == 1).FirstOrDefault();
        }

        public List<Banner> GetBanners ()
        {
            return _db.Banners.Where(b => b.DeletedAt == null).OrderBy(b => b.SortOrder).ToList();
        }
    }
}
