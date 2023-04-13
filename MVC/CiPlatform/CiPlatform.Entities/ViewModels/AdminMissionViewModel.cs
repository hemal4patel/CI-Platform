using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminMissionViewModel
    {
        public List<AdminMissionList> missions { get; set; }        
    }

    public class AdminMissionList {

        public string misssionTitle { get; set; }

        public string missionType { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }
    }
}
