using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class MissionQueryParams
    {
        //linq filter params
        public string? searchText { get; set; }

        public long? CountryId { get; set; }

        public long[]? CityId { get; set; }

        public long[]? ThemeId { get; set; }

        public long[]? SkillId { get; set; }

        public int? sortCase { get; set; }

        public int pageNo { get; set; }

        public int pagesize { get; set; }

    }
}
