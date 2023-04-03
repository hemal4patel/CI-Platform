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
    public class UserProfile : IUserProfile
    {
        private readonly ApplicationDbContext _db;

        public UserProfile (ApplicationDbContext db)
        {
            _db = db;
        }

        public User GetUserDetails (long userId)
        {
            return _db.Users.Where(u => u.UserId == userId).Include(u => u.UserSkills).FirstOrDefault();
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

    }
}
