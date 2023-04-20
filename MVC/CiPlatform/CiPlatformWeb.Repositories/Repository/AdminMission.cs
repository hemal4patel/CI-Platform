using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminMission : IAdminMission
    {
        private readonly ApplicationDbContext _db;

        public AdminMission (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminMissionList> GetMissions ()
        {
            IQueryable<Mission> missions = _db.Missions.Where(m => m.DeletedAt == null).AsQueryable();

            IQueryable<AdminMissionList> list = missions.Select(m => new AdminMissionList()
            {
                missionId = m.MissionId,
                misssionTitle = m.Title,
                missionType = m.MissionType,
                startDate = m.StartDate,
                endDate = m.EndDate
            });

            return list.ToList();
        }

        public List<MissionTheme> GetThemes ()
        {
            return _db.MissionThemes.ToList();
        }

        public List<Skill> GetSkills ()
        {
            return _db.Skills.ToList();
        }

        public AdminMissionList GetMissionToEdit (long missionId)
        {
            IQueryable<Mission> mission = _db.Missions.Where(m => m.MissionId == missionId);

            AdminMissionList list = mission.Select(m => new AdminMissionList()
            {
                missionId = m.MissionId,
                misssionTitle = m.Title,
                shortDescription = m.ShortDescription,
                missionDescription = m.Description,
                countryId = m.CountryId,
                cityId = m.CityId,
                organizationName = m.OrganizationName,
                organizationDetail = m.OrganizationDetail,
                startDate = m.StartDate,
                endDate = m.EndDate,
                missionType = m.MissionType,
                totalSeats = m.TotalSeats,
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                goalValue = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                missionTheme = m.Theme.MissionThemeId,
                missionSkills = string.Join(",", m.MissionSkills.Select(m => m.Skill.SkillId)),
                availability = m.Availability,
                imageName = string.Join(",", m.MissionMedia.Where(m => m.MediaType == "img").Select(m => $"{m.MediaPath}:{m.Default}")),
                documentName = string.Join(",", m.MissionDocuments.Select(m => m.DocumentPath)),
                status = m.Status
            }).FirstOrDefault();

            return list;
        }

        public bool MissionExistsForNew (string title, string organizationName)
        {
            return _db.Missions.Any(m => m.Title.ToLower().Trim().Replace(" ", "") == title.ToLower().Trim().Replace(" ", "") && m.OrganizationName.ToLower().Trim().Replace(" ", "") == organizationName.ToLower().Trim().Replace(" ", ""));
        }

        public bool MissionExistsForUpdate (long? missionId, string title, string organizationName)
        {
            return _db.Missions.Any(m => m.Title.ToLower().Trim().Replace(" ", "") == title.ToLower().Trim().Replace(" ", "") && m.OrganizationName.ToLower().Trim().Replace(" ", "") == organizationName.ToLower().Trim().Replace(" ", "") && m.MissionId != missionId);
        }

        public void AddMission (AdminMissionViewModel vm)
        {
            //mission
            Mission mission = new Mission()
            {
                Title = vm.newMission.misssionTitle,
                ShortDescription = vm.newMission.shortDescription,
                Description = vm.newMission.missionDescription,
                CountryId = vm.newMission.countryId,
                CityId = vm.newMission.cityId,
                OrganizationName = vm.newMission.organizationName,
                OrganizationDetail = vm.newMission.organizationDetail,
                StartDate = vm.newMission.startDate,
                EndDate = vm.newMission.endDate,
                MissionType = vm.newMission.missionType,
                TotalSeats = vm.newMission.totalSeats,
                ThemeId = vm.newMission.missionTheme,
                Availability = vm.newMission.availability,
                Status = vm.newMission.status,
                CreatedAt = DateTime.Now
            };
            //mission.Description = vm.missionDescription;
            _db.Missions.Add(mission);
            _db.SaveChanges();
            long missionId = mission.MissionId;

            //goal
            if (mission.MissionType == "Goal")
            {
                GoalMission goal = new GoalMission()
                {
                    MissionId = missionId,
                    GoalObjectiveText = vm.newMission.goalObjectiveText,
                    GoalValue = vm.newMission.goalValue,
                    CreatedAt = DateTime.Now
                };
                _db.GoalMissions.Add(goal);
                _db.SaveChanges(); 
            }

            //remove skills
            //IQueryable<MissionSkill> skills = _db.MissionSkills.Where(s => s.MissionId == missionId);
            //if (skills.Any())
            //{
            //    foreach (MissionSkill skill in skills)
            //    {
            //        skill.DeletedAt = DateTime.Now;
            //    }
            //    _db.SaveChanges();
            //}

            //skills
            string[] userSelectedSkillsStr = vm.newMission.missionSkills.Split(',');
            long[] userSelectedSkills = new long[userSelectedSkillsStr.Length];
            for (int i = 0; i < userSelectedSkillsStr.Length; i++)
            {
                userSelectedSkills[i] = long.Parse(userSelectedSkillsStr[i]);
                MissionSkill newSkill = new MissionSkill()
                {
                    MissionId = missionId,
                    SkillId = userSelectedSkills[i],
                    CreatedAt = DateTime.Now,
                };
                _db.MissionSkills.Add(newSkill);
            }
            _db.SaveChanges();

            //delete old medias
            //var media = _db.MissionMedia.Where(m => m.MissionId == missionId);
            //foreach (var m in media)
            //{
            //    if (m != null)
            //    {
            //        if (m.MediaType == "img")
            //        {
            //            var fileName = m.MediaPath;
            //            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionPhotos", fileName));
            //        }
            //        m.DeletedAt = DateTime.Now;
            //    }
            //}

            //images
            int count = 0;
            if (vm.images != null)
            {
                foreach (IFormFile u in vm.images)
                {
                    if (u != null)
                    {
                        string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + u.FileName;
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionPhotos", fileName);
                        MissionMedium newMedia = new MissionMedium()
                        {
                            MissionId = missionId,
                            MediaType = "img",
                            MediaPath = fileName,
                            CreatedAt = DateTime.Now,
                        };
                        if(count == vm.defaultImage)
                        {
                            newMedia.Default = 1;
                        }
                        else
                        {
                            newMedia.Default = 0;
                        }
                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            u.CopyTo(stream);
                        }
                        _db.MissionMedia.Add(newMedia);
                        count++;
                    }
                }
                _db.SaveChanges();
            }

            //url
            if (vm.videos is not null)
            {
                foreach (string u in vm.videos)
                {
                    if (u != null)
                    {
                        MissionMedium newMedia = new MissionMedium()
                        {
                            MissionId = missionId,
                            MediaType = "vid",
                            MediaPath = u,
                            Default = 0,
                            CreatedAt = DateTime.Now,
                        };
                        _db.MissionMedia.Add(newMedia);
                    }
                    _db.SaveChanges();
                }
            }

            //documents
            if(vm.documents is not null)
            {
                foreach(IFormFile d in vm.documents)
                {
                    if(d is not null)
                    {
                        string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + d.FileName;
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionDocuments", fileName);
                        MissionDocument document = new MissionDocument()
                        {
                            MissionId = missionId,
                            DocumentName = fileName,
                            DocumentType = Path.GetExtension(fileName).Trim('.'),
                            DocumentPath = fileName
                        };
                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            d.CopyTo(stream);
                        }
                        _db.MissionDocuments.Add(document);                        
                    }
                }
                _db.SaveChanges();
            }
        }

        public void DeleteMission (long missionId)
        {
            Mission mission = _db.Missions.FirstOrDefault(m => m.MissionId == missionId);
            mission.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
