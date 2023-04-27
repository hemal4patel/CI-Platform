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
                skillId = s.SkillId,
                skillName = s.SkillName,
                status = s.Status,
            });

            return list.ToList();
        }

        public AdminSkillModel GetSkillToEdit (long skillId)
        {
            IQueryable<Skill> skill = _db.Skills.Where(s => s.SkillId == skillId);

            AdminSkillModel list = skill.Select(s => new AdminSkillModel()
            {
                skillId = s.SkillId,
                skillName = s.SkillName,
                status = s.Status,
            }).FirstOrDefault();

            return list;
        }

        public bool SkillExistsForNew (string skillName)
        {
            return _db.Skills.Any(s => s.SkillName.ToLower().Trim().Replace(" ", "") == skillName.ToLower().Trim().Replace(" ", "") && s.DeletedAt == null);
        }

        public bool SkillExistsForUpdate (long? skillId, string skillName)
        {
            return _db.Skills.Any(s => s.SkillName.ToLower().Trim().Replace(" ", "") == skillName.ToLower().Trim().Replace(" ", "") && s.SkillId != skillId && s.DeletedAt == null);
        }

        public void AddNewSkill (AdminSkillModel vm)
        {
            Skill newSkill = new Skill()
            {
                SkillName = vm.skillName,
                Status = vm.status,
                CreatedAt = DateTime.Now
            };

            _db.Skills.Add(newSkill);
            _db.SaveChanges();
        }

        public void UpdateSkill (AdminSkillModel vm)
        {
            Skill existingSkill = _db.Skills.Where(s => s.SkillId == vm.skillId).FirstOrDefault();

            existingSkill.SkillName = vm.skillName;
            existingSkill.Status = vm.status;
            existingSkill.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
        }

        public bool DeleteSkill (long skillId)
        {
            Skill skill = _db.Skills.FirstOrDefault(s => s.SkillId == skillId);

            //is used in mission or user
            if (_db.MissionSkills.Any(m => m.SkillId == skill.SkillId && m.DeletedAt == null) || _db.UserSkills.Any(u => u.SkillId == skill.SkillId && u.DeletedAt == null))
            {
                return false;
            }
            skill.DeletedAt = DateTime.Now;
            _db.SaveChanges();
            return true;
        }

    }
}
