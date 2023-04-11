using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class VolunteeringMissionViewModel
    {
        //to store data
        public Mission MissionDetails { get; set; }        

        public List<Mission> RelatedMissions { get; set; }

        public List<MissionApplication> RecentVolunteers { get; set; }

        public List<Comment> ApprovedComments { get; set; }

        public List<User> UserList { get; set; } = null;

        public List<MissionDocument> MissionDocuments { get; set; } = null;
    }
}
