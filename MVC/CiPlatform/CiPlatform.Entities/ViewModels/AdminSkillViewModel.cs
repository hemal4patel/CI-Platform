using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminSkillViewModel
    {
        public List<AdminSkillModel> skills { get; set; }
    }

    public class AdminSkillModel
    {
        public string skillName { get; set; }

        public int status { get; set; }
    }
}
