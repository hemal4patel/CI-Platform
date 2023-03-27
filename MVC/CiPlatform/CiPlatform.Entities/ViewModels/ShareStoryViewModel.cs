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

        [Required]
        public long MissionId { get; set; }

        [Required]
        public string StoryTitle { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string StoryDescription { get; set;}

        public string[]? VideoUrl { get; set; }   

        public List<IFormFile>? Images { get; set; }
    }
}
