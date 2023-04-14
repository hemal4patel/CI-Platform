using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminThemeViewModel
    {
        public List<AdminThemeModel> themes { get; set; }
    }

    public class AdminThemeModel
    {
        public string themeName { get; set; }

        public int status { get; set; }

    }
}
