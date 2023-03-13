using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class MissionListingModel
    {
        public long MissionId { get; set; }

        public string ThemeName { get; set; }

        public long CountryId { get; set; }

        public string CityName { get; set; }

        public string? MissionTitle { get; set; }

        public string? ShortDescription { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string MissionType { get; set; } 

        public string? OrganizationName { get; set; }

        public string GoalObjectiveText { get; set; }

        public string? MediaPath { get; set; }
    }
}
