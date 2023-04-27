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

        [MinLength(3, ErrorMessage = "Name must be between 3 to 16 characters")]
        [MaxLength(16, ErrorMessage = "Name must be between 3 to 16 characters")]
        [Required(ErrorMessage = "Name is required.")]
        public string FirstName { get; set; }

        [MinLength(3, ErrorMessage = "Surname must be between 3 to 16 characters")]
        [MaxLength(16, ErrorMessage = "Surname must be between 3 to 16 characters")]
        [Required(ErrorMessage = "Surname is required.")]
        public string LastName { get; set; }

        public string? AvatarName { get; set; }

        public IFormFile? AvatarImage { get; set; }

        public string? WhyIVolunteer { get; set; }

        [MinLength(6, ErrorMessage = "Employee Id must be between 6 to 16 characters")]
        [MaxLength(16, ErrorMessage = "Employee Id must be between 6 to 16 characters")] 
        public string? EmployeeId { get; set; }

        [MinLength(3, ErrorMessage = "Manager Name must be between 3 to 30 characters")]
        [MaxLength(16, ErrorMessage = "Manager Name must be between 3 to 30 characters")]
        public string? Manager { get; set; }

        [MinLength(2, ErrorMessage = "Department must be between 2 to 16 characters")]
        [MaxLength(16, ErrorMessage = "Department must be between 2 to 16 characters")] 
        public string? Department { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public long? CityId { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public long? CountryId { get; set; }

        [MinLength(3, ErrorMessage = "Profile Text must more then 10 characters")]
        public string? ProfileText { get; set; }

        [RegularExpression(@"^https?://(?:www\.)?linkedin\.com/(?:in|company)/[\w-]+/?$", ErrorMessage = "Invalid LinkedIn url.")] 
        public string? LinkedInUrl { get; set; }

        [MinLength(10, ErrorMessage = "Title must more then 10 characters")]
        [MaxLength(16, ErrorMessage = "Title must be of less than 255 characters")]
        public string? Title { get; set; }

        public string? Availability { get; set; }

        public string? UserSelectedSkills { get; set; }

        public List<Country>? CountryList { get; set; }

        public List<City>? CityList { get; set; }

        public List<Skill>? SkillList {  get; set; }

        public List<UserSkill>? UserSkills { get; set; }

        public List<CmsPage>? PolicyPages { get; set; }


        //change password
        [Required(ErrorMessage = "Old password is required")]
        public string? oldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MaxLength(16, ErrorMessage = "New password must be between 8 to 16 characters")]
        [MinLength(8, ErrorMessage = "New password must be between 8 to 16 characters")]
        public string? newPassword { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [Compare("newPassword", ErrorMessage = "Password doesn't match")]
        public string? cnfPassword { get; set; } = null!;
    }
}
