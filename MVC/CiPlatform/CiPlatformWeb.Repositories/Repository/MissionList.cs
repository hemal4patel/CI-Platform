using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CiPlatformWeb.Entities.ViewModels;
using System.Drawing.Printing;

namespace CiPlatformWeb.Repositories.Repository
{
    public class MissionList : IMissionList
    {
        private readonly ApplicationDbContext _db;

        public MissionList (ApplicationDbContext db)
        {
            _db = db;
        }

        public User sessionUser (long userId)
        {
            var sessionUser = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            return sessionUser;
        }


        public List<Country> GetCountryList ()
        {
            return _db.Countries.ToList();
        }

        public List<City> GetCityList (long countryId)
        {
            return _db.Cities.Where(c => c.CountryId == countryId).ToList();
        }

        public List<MissionTheme> GetThemeList ()
        {
            return _db.MissionThemes.ToList();
        }

        public List<Skill> GetSkillList ()
        {
            return _db.Skills.ToList();
        }

        public (List<MissionListModel> missions, int count) GetMissions (MissionQueryParams viewmodel, long userId)
        {

            var missions = _db.Missions.AsQueryable();

            if (viewmodel.CountryId != null)
            {
                missions = missions.Where(m => m.CountryId == viewmodel.CountryId);
            }

            if (viewmodel.CityId != null)
            {
                missions = missions.Where(m => viewmodel.CityId.Contains(m.CityId));
            }

            if (viewmodel.ThemeId != null)
            {
                missions = missions.Where(m => viewmodel.ThemeId.Contains(m.ThemeId));
            }

            if (viewmodel.SkillId != null)
            {
                missions = missions.Where(m => m.MissionSkills.Any(s => viewmodel.SkillId.Contains(s.SkillId)));
            }

            if (viewmodel.searchText != null)
            {
                missions = missions.Where(m => m.Title.ToLower().Replace(" ", "").Contains(viewmodel.searchText) || m.ShortDescription.ToLower().Contains(viewmodel.searchText) || m.Description.ToLower().Contains(viewmodel.searchText));
            }


            switch (viewmodel.sortCase)
            {
                case 1:
                    missions = missions.OrderByDescending(m => m.CreatedAt);
                    break;

                case 2:
                    missions = missions.OrderBy(m => m.CreatedAt);
                    break;

                case 3:
                    missions = missions.OrderBy(m => (m.TotalSeats - m.MissionApplications.Count(ma => ma.ApprovalStatus == "APPROVE")));
                    break;

                case 4:
                    missions = missions.OrderByDescending(m => (m.TotalSeats - m.MissionApplications.Count(ma => ma.ApprovalStatus == "APPROVE")));
                    break;

                case 5:
                    missions = missions.OrderByDescending(m => m.EndDate);
                    break;

                case 6:
                    missions = missions.Where(m => m.FavouriteMissions.Any(f => f.UserId == userId));
                    break;
            }

            int count = missions.Count();

            missions = missions
            .Skip(Math.Max((viewmodel.pageNo - 1) * viewmodel.pagesize, 0))
            .Take(viewmodel.pagesize);

            var list = missions.Select(m => new MissionListModel()
            {
                mission = m,
                missionId = m.MissionId,
                cityName = m.City.Name,
                themeName = m.Theme.Title,
                isFavorite = m.FavouriteMissions.Any(m => m.UserId == userId),
                rating = m.MissionRatings.Select(m => m.Rating).FirstOrDefault(),
                seatsLeft = m.TotalSeats - m.MissionApplications.Where(m => m.ApprovalStatus == "APPROVE").Count(),
                hasDeadlinePassed = m.StartDate.Value.AddDays(-1) < DateTime.Now,
                haEndDatePassed = m.EndDate < DateTime.Now,
                isOngoing = (m.StartDate < DateTime.Now) && (m.EndDate > DateTime.Now),
                hasApplied = m.MissionApplications.Any(m => m.UserId == userId),
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                totalGoal = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                achievedGoal = m.Timesheets.Sum(m => m.Action),
                mediaPath = m.MissionMedia.Where(m => m.Default == 1).Select(m => m.MediaPath).FirstOrDefault(),
                skill = m.MissionSkills.Select(m => m.Skill.SkillName).FirstOrDefault()
            });


            return (list.ToList(), count);
        }

        public List<User> GetUserList (long userId)
        {
            var recentVolunteers = _db.Users.Where(u => u.UserId != userId).ToList();

            return recentVolunteers;
        }

        public MissionInvite HasAlreadyInvited (long ToUserId, long MissionId, long FromUserId)
        {
            return _db.MissionInvites.Where(m => m.MissionId == MissionId && m.ToUserId == ToUserId && m.FromUserId == FromUserId).FirstOrDefault();
        }

        public async Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link)
        {
            var Email = await _db.Users.Where(u => u.UserId == ToUserId).FirstOrDefaultAsync();

            var Sender = await _db.Users.Where(s => s.UserId == FromUserId).FirstOrDefaultAsync();

            var fromEmail = new MailAddress("ciplatformdemo@gmail.com");
            var toEmail = new MailAddress(Email.Email);
            var fromEmailPassword = "pdckerdmuutmdzhz";
            string subject = "Mission Invitation";
            string body = "You Have Recieved Mission Invitation From " + Sender.FirstName + " " + Sender.LastName + " For:\n\n" + link;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            var message = new MailMessage(fromEmail, toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            await smtp.SendMailAsync(message);
        }

        public void AddToFavorites (long missionId, long userId)
        {
            if (_db.FavouriteMissions.Any(fm => fm.MissionId == missionId && fm.UserId == userId))
            {
                var FavoriteMissionId = _db.FavouriteMissions.Where(fm => fm.MissionId == missionId && fm.UserId == userId).FirstOrDefault();
                _db.FavouriteMissions.Remove(FavoriteMissionId);
                _db.SaveChanges();
            }
            else
            {
                var favoriteMission = new FavouriteMission { MissionId = missionId, UserId = userId };
                _db.FavouriteMissions.Add(favoriteMission);
                _db.SaveChanges();
            }
        }

        public void RateMission (long missionId, long userId, int rating)
        {
            var alredyRated = _db.MissionRatings.SingleOrDefault(mr => mr.MissionId == missionId && mr.UserId == userId);

            if (alredyRated != null)
            {
                alredyRated.Rating = rating;
                alredyRated.UpdatedAt = DateTime.Now;
                _db.Update(alredyRated);
                _db.SaveChanges();
            }
            else
            {
                var newRating = new MissionRating { UserId = userId, MissionId = missionId, Rating = rating };
                _db.MissionRatings.Add(newRating);
                _db.SaveChangesAsync();
            }
        }

        public List<MissionInvite> GetMissionInvites (long userId)
        {
            var missionInvites = _db.MissionInvites.Where(m => m.ToUserId == userId).Include(m => m.FromUser).Include(m => m.Mission).ToList();
            return missionInvites;
        }

        public List<StoryInvite> GetStoryInvites (long userId)
        {
            var storyInvites = _db.StoryInvites.Where(m => m.ToUserId == userId).Include(m => m.FromUser).Include(m => m.Story).ToList();
            return storyInvites;
        }
    }
}
