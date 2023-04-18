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
    public class AdminMissionViewModel
    {
        public List<AdminMissionList> missions { get; set; }

        public AdminMissionList newMission { get; set; }

        public List<Country> countryList { get; set; }

        public List<MissionTheme> themeList { get; set; }

        public List<Skill> skillList { get; set; }
    }

    public class AdminMissionList
    {

        public long? missionId { get; set; }

        [Required(ErrorMessage = "Mission title is required")]
        public string misssionTitle { get; set; }

        [Required(ErrorMessage = "Short description is required")]
        public string shortDescription { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string description { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public long countryId { get; set; }

        [Required(ErrorMessage = "City is required")]
        public long cityId { get; set; }

        [Required(ErrorMessage = "Organization name is required")]
        public string organizationName { get; set; }

        [Required(ErrorMessage = "Organization deatils is required")]
        public string organizationDetail { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime? startDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime? endDate { get; set; }

        [Required(ErrorMessage = "Mission type is required")]
        public string missionType { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Actions must contain only digits.")]
        public int totalSeats { get; set; }

        public DateTime registrationDeadline { get; set; }

        public string goalObjectiveText { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Actions must contain only digits.")]
        public int goalValue { get; set; }

        [Required(ErrorMessage = "Mission theme is required")]
        public long missionTheme { get; set; }

        [Required(ErrorMessage = "Mission skills are required")]
        public long[] missionSkills { get; set; }

        [Required(ErrorMessage = "Mission availability is required")]
        public string availability { get; set; }

        public IFormFile[]? images { get; set; }

        public string? documents { get; set; }

        public string? videos { get; set; }
    }
}
