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
    public class ShareStoryViewModel
    {
        public List<MissionApplication>? MissionTitles { get; set; }

        [Required(ErrorMessage = "Mission is required.")]
        public long MissionId { get; set; }

        [Required(ErrorMessage = "Story title is required.")]
        [MinLength(3, ErrorMessage = "Story title must be atleat 3 characters long")]
        [MaxLength(255, ErrorMessage = "Story title cannot be longer than 255 characters")]
        public string StoryTitle { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Story description is required.")]
        public string StoryDescription { get; set; }

        public string[]? VideoUrl { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
