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
            return _db.MissionThemes.Where(t => t.DeletedAt == null).ToList();
        }

        public List<Skill> GetSkills ()
        {
            return _db.Skills.Where(s => s.DeletedAt == null).ToList();
        }

        public AdminMissionList GetMissionToEdit (long missionId)
        {
            IQueryable<Mission> mission = _db.Missions.Where(m => m.MissionId == missionId);

            AdminMissionList? list = mission.Select(m => new AdminMissionList()
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
                registrationDeadline = m.RegistrationDeadline,
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                goalValue = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                missionTheme = m.Theme.MissionThemeId,
                missionSkills = string.Join(",", m.MissionSkills.Where(s => s.DeletedAt == null).Select(m => m.Skill.SkillId)),
                availability = m.Availability,
                videosUrl = string.Join("\n", m.MissionMedia.Where(m => m.MediaType== "vid" && m.DeletedAt == null).Select(m => m.MediaPath)),
                imageName = string.Join(",", m.MissionMedia.Where(m => m.MediaType == "img" && m.DeletedAt == null).Select(m => $"{m.MediaPath}:{m.Default}")),
                documentName = string.Join(",", m.MissionDocuments.Where(m => m.DeletedAt == null).Select(m => m.DocumentPath)),
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
                Description = vm.description,
                CountryId = vm.newMission.countryId,
                CityId = vm.newMission.cityId,
                OrganizationName = vm.newMission.organizationName,
                OrganizationDetail = vm.orgDetail,
                StartDate = vm.newMission.startDate,
                EndDate = vm.newMission.endDate,
                MissionType = vm.newMission.missionType,
                ThemeId = vm.newMission.missionTheme,
                Availability = vm.newMission.availability,
                Status = vm.newMission.status,
                CreatedAt = DateTime.Now
            };
            //time
            if (mission.MissionType == "Time")
            {
                mission.TotalSeats = vm.newMission.totalSeats;
                mission.RegistrationDeadline = vm.newMission.registrationDeadline;
            }
            else
            {
                mission.TotalSeats = null;
                mission.RegistrationDeadline = null;
            }
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

            //images
            int count = 0;
            if (vm.images is not null)
            {
                foreach (IFormFile u in vm.images)
                {
                    if (u is not null)
                    {
                        string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + u.FileName;
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionPhotos", fileName);
                        var onlyName = Path.GetFileNameWithoutExtension(fileName);
                        MissionMedium newMedia = new MissionMedium()
                        {
                            MissionId = missionId,
                            MediaType = "img",
                            MediaPath = fileName,
                            MediaName = onlyName,
                            CreatedAt = DateTime.Now,
                        };
                        if (count == vm.defaultImage)
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
                    if (u is not null)
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
            if (vm.documents is not null)
            {
                foreach (IFormFile d in vm.documents)
                {
                    if (d is not null)
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

        public void EditMission (AdminMissionViewModel vm)
        {
            Mission mission = _db.Missions.Where(m => m.MissionId == vm.newMission.missionId).FirstOrDefault();
            mission.Title = vm.newMission.misssionTitle;
            mission.ShortDescription = vm.newMission.shortDescription;
            mission.Description = vm.description;
            mission.CountryId = vm.newMission.countryId;
            mission.CityId = vm.newMission.cityId;
            mission.OrganizationName = vm.newMission.organizationName;
            mission.OrganizationDetail = vm.orgDetail;
            if (vm.newMission.startDate is not null)
            {
                mission.StartDate = vm.newMission.startDate;
            }
            if (vm.newMission.endDate is not null)
            {
                mission.EndDate = vm.newMission.endDate;
            }
            if (mission.MissionType == "Time")
            {
                if (vm.newMission.registrationDeadline is not null)
                {
                    mission.RegistrationDeadline = vm.newMission.registrationDeadline;
                }
                mission.TotalSeats = vm.newMission.totalSeats;
            }
            else
            {
                mission.TotalSeats = null;
                mission.RegistrationDeadline = null;
            }
            mission.ThemeId = vm.newMission.missionTheme;
            mission.Availability = vm.newMission.availability;
            mission.Status = vm.newMission.status;
            mission.UpdatedAt = DateTime.Now;
            _db.SaveChanges();

            //goal
            if (vm.newMission.missionType == "Goal")
            {
                GoalMission goal = _db.GoalMissions.Where(g => g.MissionId == mission.MissionId).FirstOrDefault();
                goal.GoalValue = vm.newMission.goalValue;
                goal.GoalObjectiveText = vm.newMission.goalObjectiveText;
                goal.UpdatedAt = DateTime.Now;
                _db.SaveChanges();
            }

            //remove skills
            IQueryable<MissionSkill> skills = _db.MissionSkills.Where(s => s.MissionId == vm.newMission.missionId);
            if (skills.Any())
            {
                foreach (MissionSkill skill in skills)
                {
                    skill.DeletedAt = DateTime.Now;
                }
                _db.SaveChanges();
            }
            //skills
            string[] userSelectedSkillsStr = vm.newMission.missionSkills.Split(',');
            long[] userSelectedSkills = new long[userSelectedSkillsStr.Length];
            for (int i = 0; i < userSelectedSkillsStr.Length; i++)
            {
                userSelectedSkills[i] = long.Parse(userSelectedSkillsStr[i]);
                MissionSkill newSkill = new MissionSkill()
                {
                    MissionId = vm.newMission.missionId,
                    SkillId = userSelectedSkills[i],
                    CreatedAt = DateTime.Now,
                };
                _db.MissionSkills.Add(newSkill);
            }
            _db.SaveChanges();

            //delete old medias
            IQueryable<MissionMedium> media = _db.MissionMedia.Where(m => m.MissionId == vm.newMission.missionId);
            foreach (MissionMedium m in media)
            {
                if (m is not null)
                {
                    if (m.MediaType == "img")
                    {
                        string fileName = m.MediaPath;
                        File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionPhotos", fileName));
                    }
                    m.DeletedAt = DateTime.Now;
                }
            }
            IQueryable<MissionDocument> documents = _db.MissionDocuments.Where(m => m.MissionId == vm.newMission.missionId);
            foreach (MissionDocument d in documents)
            {
                if (d is not null)
                {
                    string fileName = d.DocumentPath;
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionDocuments", fileName));
                }
                d.DeletedAt = DateTime.Now;
            }
            _db.SaveChanges();

            //images
            int count = 0;
            if (vm.images is not null)
            {
                foreach (IFormFile u in vm.images)
                {
                    if (u is not null)
                    {
                        string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + u.FileName;
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionPhotos", fileName);
                        var onlyName = Path.GetFileNameWithoutExtension(fileName);
                        MissionMedium newMedia = new MissionMedium()
                        {
                            MissionId = vm.newMission.missionId,
                            MediaType = "img",
                            MediaPath = fileName,
                            MediaName = onlyName,
                            CreatedAt = DateTime.Now,
                        };
                        if (count == vm.defaultImage)
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
                    if (u is not null)
                    {
                        MissionMedium newMedia = new MissionMedium()
                        {
                            MissionId = vm.newMission.missionId,
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
            if (vm.documents is not null)
            {
                foreach (IFormFile d in vm.documents)
                {
                    if (d is not null)
                    {
                        string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + d.FileName;
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "MissionDocuments", fileName);
                        MissionDocument document = new MissionDocument()
                        {
                            MissionId = vm.newMission.missionId,
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
