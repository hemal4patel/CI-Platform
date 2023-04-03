using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class UserProfileViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Avatar { get; set; }

        public string? WhyIVolunteer { get; set; }

        public string? EmployeeId { get; set; }

        public string? Manager { get; set; }

        public string? Department { get; set; }

        public long? CityId { get; set; }

        public long CountryId { get; set; }

        public string ProfileText { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? Title { get; set; }

        public int? Availability { get; set; }

        public DateTime? Created { get; set; }

        public User UserDetails { get; set; }

        public List<Country> CountryList { get; set; }

        public List<City>? CityList { get; set; }

        public List<Skill> SkillList {  get; set; }

        public List<Skill> UserSkills { get; set; }
    }
}
