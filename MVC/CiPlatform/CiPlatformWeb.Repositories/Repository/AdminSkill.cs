using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminSkill : IAdminSkill
    {
        private readonly ApplicationDbContext _db;

        public AdminSkill (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminSkillModel> GetSkills ()
        {
            IQueryable<Skill> skills = _db.Skills.Where(s => s.DeletedAt == null).AsQueryable();

            IQueryable<AdminSkillModel> list = skills.Select(s => new AdminSkillModel()
            {
                skillName = s.SkillName,
                status = s.Status,
            });

            return list.ToList();
        }
    }
}
