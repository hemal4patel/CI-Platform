using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminStoryViewModel
    {
        public List<AdminStoryModel> stories { get; set; }
    }

    public class AdminStoryModel
    {
        public string storyTitle { get; set; }

        public string userName { get; set; }

        public string missionTitle { get; set; }

        public string status { get; set; }
    }
}
