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

        public List<MissionListModel> RelatedMissions { get; set; }

        public List<User> UserList { get; set; } = null;

        public List<Comment> ApprovedComments { get; set; }

        public List<MissionApplication> RecentVolunteers { get; set; }

        //to store data
        public MissionDetailsModel MissionDetails { get; set; }
    }

    public class MissionDetailsModel
    {
        public Mission mission { get; set; }

        public long missionId { get; set; }

        public string cityName { get; set; }

        public string themeName { get; set; }

        public bool isFavorite { get; set; }

        public double rating { get; set; }

        public int? ratedByVols { get; set; }

        public int? seatsLeft { get; set; }

        public bool hasDeadlinePassed { get; set; }

        public bool haEndDatePassed { get; set; }

        public bool isOngoing { get; set; }

        public bool hasApplied { get; set; }

        public bool hasAppliedApprove { get; set; }

        public bool hasAppliedPending { get; set; }

        public bool hasAppliedDecline { get; set; }

        public string? goalObjectiveText { get; set; }

        public int? totalGoal { get; set; }

        public int? achievedGoal { get; set; }

        public List<MissionMedium>? missionMedia { get; set; }

        public List<string>? skills { get; set; }

        public List<Comment>? ApprovedComments { get; set; }

        public List<MissionDocument>? MissionDocuments { get; set; }
    }
}
