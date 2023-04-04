using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class UserProfile : IUserProfile
    {
        private readonly ApplicationDbContext _db;

        public UserProfile (ApplicationDbContext db)
        {
            _db = db;
        }

        public UserProfileViewModel GetUserDetails (long userId)
        {
            User user = _db.Users.Where(u => u.UserId == userId).Include(u => u.UserSkills).FirstOrDefault();
            var vm = new UserProfileViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar,
                WhyIVolunteer = user.WhyIVolunteer,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                CityId = user.CityId,
                CountryId = user.CountryId,
                ProfileText = user.ProfileText,
                LinkedInUrl = user.LinkedInUrl,
                Title = user.Title
            };
            return vm;
        }

        public List<Country> GetCountryList ()
        {
            return _db.Countries.ToList();
        }


        public List<City> GetCityList (long countryId)
        {
            return _db.Cities.Where(c => c.CountryId == countryId).ToList();
        }

        public List<Skill> GetSkillList ()
        {
            return _db.Skills.ToList();
        }

        public User CheckPassword (long userId, string oldPassoword)
        {
            return _db.Users.Where(u => u.UserId == userId && u.Password == oldPassoword).FirstOrDefault();
        }

        public void UpdatePassword (User user, string newPassoword)
        {
            user.Password = newPassoword;
            user.UpdatedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
