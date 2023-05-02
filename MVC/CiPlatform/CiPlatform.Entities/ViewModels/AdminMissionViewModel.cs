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

        public List<City> cityList { get; set; }

        public List<MissionTheme> themeList { get; set; }

        public List<Skill> skillList { get; set; }

        public IFormFile[]? images { get; set; }

        public int? defaultImage { get; set; }

        public IFormFile[]? documents { get; set; }

        public string[]? videos { get; set; }

        public string description { get; set; }

        public string orgDetail { get; set; }
    }

    public class AdminMissionList
    {

        public long missionId { get; set; }

        [Required(ErrorMessage = "Mission title is required")]
        public string misssionTitle { get; set; }

        [MinLength(10, ErrorMessage = "Short description must be atleat 10 characters long")]
        [MaxLength(300, ErrorMessage = "Short description cannot be longer than 300 characters")]
        [Required(ErrorMessage = "Short description is required")]
        public string shortDescription { get; set; }

        public string? missionDescription { get; set; }

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

        [Required(ErrorMessage = "Registration date is required")]
        public DateTime? registrationDeadline { get; set; }

        [Required(ErrorMessage = "Mission type is required")]
        public string missionType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total seats cannot be 0.")]
        [Required(ErrorMessage = "Total seats is required")]
        public int? totalSeats { get; set; }

        [Required(ErrorMessage = "Goal objective text is required")]
        public string? goalObjectiveText { get; set; }

        [Required(ErrorMessage = "Goal value is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Goal value cannot be 0.")]
        public int goalValue { get; set; }

        [Required(ErrorMessage = "Mission theme is required")]
        public long missionTheme { get; set; }

        [Required(ErrorMessage = "Mission skills are required")]
        public string missionSkills { get; set; }

        [Required(ErrorMessage = "Mission availability is required")]
        public string availability { get; set; }

        public string? imageName { get; set; }

        public string? videosUrl { get; set; }

        public string? documentName { get; set; }

        [Required(ErrorMessage = "Status is requred")]
        public int? status { get; set; }
    }
}
