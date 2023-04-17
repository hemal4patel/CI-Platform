using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminApplicationViewModel
    {
        public List<AdminApplicationModel> applications { get; set; }
    }

    public class AdminApplicationModel
    {
        public long? applicationId { get; set; }

        public string missionTitle { get; set; }

        public long missionId { get; set; }

        public long userId { get; set; }

        public string userName { get; set; }

        public string appliedAt { get; set; }

        public string status { get; set; }

    }
}
