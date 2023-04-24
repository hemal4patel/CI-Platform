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

        //[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Image is required.")]
        public IFormFile? image { get; set; }

        public string? imageName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Text is required.")]
        public string? text { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Sort order is required.")]
        public int? sortOrder { get; set; }
    }

}
