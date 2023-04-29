using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminTimesheetViewModel
    {
        public List<AdminTimesheetModel> timesheets { get; set; }
    }

    public class AdminTimesheetModel
    {
        public long timesheetId { get; set; }

        public string mission { get; set; }

        public string user { get; set; }

        public string missionType { get; set; }

        public DateTime dateVolunteered { get; set; }

        public int? action { get; set; }

        public TimeSpan? timeVolunteered { get; set; }

        public string status { get; set; }
    }
}
