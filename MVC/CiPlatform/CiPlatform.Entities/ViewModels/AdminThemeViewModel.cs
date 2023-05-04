using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminThemeViewModel
    {
        public List<AdminThemeModel> themes { get; set; }

        public AdminThemeModel newTheme { get; set; }
    }

    public class AdminThemeModel
    {
        public long? themeId { get; set; }

        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z ]+$", ErrorMessage = "Theme name is required.")]
        [Required(ErrorMessage = "Theme name is required.")]
        public string themeName { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public byte status { get; set; }

    }
}
