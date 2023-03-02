using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class FiltersModel
    {
        public long CountryId { get; set; }

        public string Name { get; set; }

        public long CityId { get; set; }

        public long SkillId { get; set; }

        public string? SkillName { get; set; }

        public long MissionThemeId { get; set; }

        public string? Title { get; set; }
    }
}
