using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminSkillViewModel
    {
        public List<AdminSkillModel> skills { get; set; }

        public AdminSkillModel newSkill { get; set; }
    }

    public class AdminSkillModel
    {
        public long? skillId { get; set; }

        [Required(ErrorMessage = "Skill name is required.")]
        public string skillName { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public byte status { get; set; }
    }
}
