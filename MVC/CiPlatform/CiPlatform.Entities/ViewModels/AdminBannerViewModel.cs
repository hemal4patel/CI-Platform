using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminBannerViewModel
    {
        public List<AdminBannerModel> banners { get; set; }

        public AdminBannerModel newBanner { get; set; }

        public IFormFile newImage { get; set; }
    }

    public class AdminBannerModel
    {
        public long? bannerId { get; set; }

        public IFormFile? image { get; set; }

        public string? imageName { get; set; }

        [MinLength(10, ErrorMessage = "Text must more then 10 characters")]
        [MaxLength(16, ErrorMessage = "Text must be of less than 255 characters")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Text is required.")]
        public string? text { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sort order cannot be 0.")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Sort order is required.")]
        public int? sortOrder { get; set; }
    }

}
