using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminSkill
    {
        public List<AdminSkillModel> GetSkills ();

        public AdminSkillModel GetSkillToEdit (long skillId);

        public bool SkillExistsForNew (string skillName);

        public void AddNewSkill (AdminSkillModel vm);

        public bool SkillExistsForUpdate (long? skillId, string skillName);

        public void UpdateSkill (AdminSkillModel vm);

        public void DeleteSkill (long skillId);
    }
}
