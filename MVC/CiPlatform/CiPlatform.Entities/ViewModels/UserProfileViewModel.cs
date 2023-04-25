using CiPlatformWeb.Entities.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class UserProfileViewModel
    {
        public long UserId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public string LastName { get; set; }

        public string? AvatarName { get; set; }

        public IFormFile? AvatarImage { get; set; }

        public string? WhyIVolunteer { get; set; }

        public string? EmployeeId { get; set; }

        public string? Manager { get; set; }

        public string? Department { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public long? CityId { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public long? CountryId { get; set; }

        public string? ProfileText { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? Title { get; set; }

        public int? Availability { get; set; }

        public string? UserSelectedSkills { get; set; }

        public List<Country>? CountryList { get; set; }

        public List<City>? CityList { get; set; }

        public List<Skill>? SkillList {  get; set; }

        public List<UserSkill>? UserSkills { get; set; }

        public List<CmsPage>? PolicyPages { get; set; }
    }
}
