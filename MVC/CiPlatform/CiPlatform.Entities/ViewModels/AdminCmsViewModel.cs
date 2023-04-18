using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminCmsViewModel
    {
        public List<AdminCmsModel> cmsPages { get; set; }

        public AdminCmsModel newCms { get; set; }
    }

    public class AdminCmsModel
    {
        public long? cmsId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string description { get; set; }

        [Required(ErrorMessage = "Slug is required.")]
        public string slug { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int? status { get; set; }
    }
}
