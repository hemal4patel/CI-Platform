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
using static CiPlatformWeb.Repositories.EnumStats;
using CiPlatformWeb.Entities.Auth;
using Microsoft.Extensions.Options;

namespace CiPlatformWeb.Repositories.Repository
{
    public class MissionList : IMissionList
    {
        private readonly ApplicationDbContext _db;
        private readonly EmailConfiguration _emailConfig;

        public MissionList (ApplicationDbContext db, IOptions<EmailConfiguration> emailConfig)
        {
            _db = db;
            _emailConfig = emailConfig.Value;
        }

        public User sessionUser (long userId)
        {
            User sessionUser = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
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
            return _db.MissionThemes.Where(m => m.DeletedAt == null && m.Status == 1).ToList();
        }

        public List<Skill> GetSkillList ()
        {
            return _db.Skills.Where(m => m.DeletedAt == null && m.Status == 1).ToList();
        }

        public (List<MissionListModel> missions, int count) GetMissions (MissionQueryParams viewmodel, long userId)
        {
            IQueryable<Mission> missions = _db.Missions.Where(m => m.DeletedAt == null && m.Status == 1).AsQueryable();

            List<long> themes = missions.GroupBy(m => m.ThemeId).OrderByDescending(g => g.Count()).Take(1).Select(g => g.Key).ToList();


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

            switch (viewmodel.exploreOption)
            {
                case 1:
                    missions = missions.Where(m => themes.Contains(m.ThemeId));
                    break;

                case 2:
                    missions = missions.OrderByDescending(m => m.MissionRatings.Average(r => r.Rating));
                    break;

                case 3:
                    missions = missions.OrderByDescending(m => m.FavouriteMissions.Count());
                    break;

                case 4:
                    missions = missions.OrderByDescending(m => m.MissionId);
                    break;
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
                    missions = missions.Where(m => m.MissionType == missionType.Time.ToString()).OrderBy(m => (m.TotalSeats - m.MissionApplications.Count(ma => ma.ApprovalStatus == applicationStatus.approve.ToString().ToUpper())));
                    break;

                case 4:
                    missions = missions.Where(m => m.MissionType == missionType.Time.ToString()).OrderByDescending(m => (m.TotalSeats - m.MissionApplications.Count(ma => ma.ApprovalStatus == applicationStatus.approve.ToString().ToUpper())));
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

            IQueryable<MissionListModel> list = missions.Select(m => new MissionListModel()
            {
                mission = m,
                missionId = m.MissionId,
                cityName = m.City.Name,
                themeName = m.Theme.Title,
                isFavorite = m.FavouriteMissions.Any(m => m.UserId == userId),
                rating = m.MissionRatings.Any() ? m.MissionRatings.Average(m => m.Rating) : 0,
                seatsLeft = m.TotalSeats - m.MissionApplications.Where(m => m.ApprovalStatus == applicationStatus.approve.ToString().ToUpper()).Count(),
                hasDeadlinePassed = m.StartDate.Value.AddDays(-1) < DateTime.Now,
                haEndDatePassed = m.EndDate < DateTime.Now,
                isOngoing = (m.StartDate < DateTime.Now) && (m.EndDate > DateTime.Now),
                hasApplied = m.MissionApplications.Any(m => m.UserId == userId),
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                totalGoal = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                achievedGoal = m.Timesheets.Where(m => m.DeletedAt == null && m.Status == timesheetStatus.approved.ToString().ToUpper()).Sum(m => m.Action),
                mediaPath = m.MissionMedia.Where(m => m.Default == 1 && m.DeletedAt == null).Select(m => m.MediaPath).FirstOrDefault(),
                skill = m.MissionSkills.Where(m => m.DeletedAt == null).Select(m => m.Skill.SkillName).FirstOrDefault(),
                totalVolunteers = m.MissionApplications.Where(m => m.ApprovalStatus == applicationStatus.approve.ToString().ToUpper()).Count()
            });

            return (list.ToList(), count);
        }

        public List<User> GetUserList (long userId)
        {
            List<User> recentVolunteers = _db.Users.Where(u => u.UserId != userId && u.DeletedAt == null && u.Role == userRole.user.ToString()).Include(u => u.MissionInviteFromUsers).Include(u => u.MissionInviteToUsers).ToList();

            return recentVolunteers;
        }

        public MissionInvite HasAlreadyInvited (long ToUserId, long MissionId, long FromUserId)
        {
            return _db.MissionInvites.Where(m => m.MissionId == MissionId && m.ToUserId == ToUserId && m.FromUserId == FromUserId).FirstOrDefault();
        }

        public async Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link)
        {
            User Email = await _db.Users.Where(u => u.UserId == ToUserId).FirstOrDefaultAsync();

            User Sender = await _db.Users.Where(s => s.UserId == FromUserId).FirstOrDefaultAsync();

            EmailConfiguration EmailConfiguration = _emailConfig;
            MailAddress fromEmail = new MailAddress(EmailConfiguration.fromEmail);
            string fromEmailPassword = EmailConfiguration.fromEmailPassword;
            MailAddress toEmail = new MailAddress(Email.Email);
            string subject = "Mission Invitation";
            string body = "You Have Recieved Mission Invitation From " + Sender.FirstName + " " + Sender.LastName + " For:\n\n" + link;

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            MailMessage message = new MailMessage(fromEmail, toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            await smtp.SendMailAsync(message);
        }

        public void AddToFavorites (long missionId, long userId)
        {
            if (_db.FavouriteMissions.Any(fm => fm.MissionId == missionId && fm.UserId == userId))
            {
                FavouriteMission FavoriteMissionId = _db.FavouriteMissions.Where(fm => fm.MissionId == missionId && fm.UserId == userId).FirstOrDefault();
                _db.FavouriteMissions.Remove(FavoriteMissionId);
                _db.SaveChanges();
            }
            else
            {
                FavouriteMission favoriteMission = new FavouriteMission { MissionId = missionId, UserId = userId };
                _db.FavouriteMissions.Add(favoriteMission);
                _db.SaveChanges();
            }
        }

        public void RateMission (long missionId, long userId, int rating)
        {
            MissionRating alredyRated = _db.MissionRatings.SingleOrDefault(mr => mr.MissionId == missionId && mr.UserId == userId);

            if (alredyRated != null)
            {
                alredyRated.Rating = rating;
                alredyRated.UpdatedAt = DateTime.Now;
                _db.Update(alredyRated);
                _db.SaveChanges();
            }
            else
            {
                MissionRating newRating = new MissionRating { UserId = userId, MissionId = missionId, Rating = rating };
                _db.MissionRatings.Add(newRating);
                _db.SaveChangesAsync();
            }
        }

        public List<MissionInvite> GetMissionInvites (long userId)
        {
            List<MissionInvite> missionInvites = _db.MissionInvites.Where(m => m.ToUserId == userId && m.Mission.DeletedAt == null && m.Mission.Status == 1).Include(m => m.FromUser).Include(m => m.Mission).ToList();
            return missionInvites;
        }

        public List<StoryInvite> GetStoryInvites (long userId)
        {
            List<StoryInvite> storyInvites = _db.StoryInvites.Where(m => m.ToUserId == userId && m.Story.DeletedAt == null && m.Story.Status == storyStatus.published.ToString().ToUpper()).Include(m => m.FromUser).Include(m => m.Story).ToList();
            return storyInvites;
        }

        public List<NotificationParams> GetAllNotifications (long userId)
        {
            IQueryable<UserNotification> notifications = _db.UserNotifications.Where(u => u.ToUserId == userId && u.DeletedAt == null && u.UserSetting.IsEnabled == true).OrderByDescending(u => u.CreatedAt).AsQueryable();

            var list = notifications.Select(n => new NotificationParams()
            {
                notificationId = n.NotificationId,
                toUserId = userId,
                fromUserId = n.FromUserId.HasValue ? n.FromUserId : 0,
                recommendedMissionId = n.RecommendedMissionId.HasValue ? n.RecommendedMissionId : 0,
                recommendedStoryId = n.RecommendedStooyId.HasValue ? n.RecommendedStooyId : 0,
                newMissionId = n.NewMissionId.HasValue ? n.NewMissionId : 0,
                storyId = n.StoryId.HasValue ? n.StoryId : 0,
                timesheetId = n.TimesheetId.HasValue ? n.TimesheetId : 0,
                commentId = n.CommentId.HasValue ? n.CommentId : 0,
                MissionApplicationId = n.MissionApplicationId.HasValue ? n.MissionApplicationId : 0,
                status = n.Status,
                createdAt = n.CreatedAt,
                fromUserName = n.FromUserId.HasValue ? n.FromUser.FirstName + " " + n.FromUser.LastName : "",
                fromUserAvatar = n.FromUserId.HasValue ? n.FromUser.Avatar : "",
                newMissionTitle = n.NewMissionId.HasValue ? n.NewMission.Title : "",
                recommendedMissionTitle = n.RecommendedMissionId.HasValue ? n.RecommendedMission.Title : "",
                timesheetMissionTitle = n.TimesheetId.HasValue ? n.Timesheet.Mission.Title : "",
                commentMissionTitle = n.CommentId.HasValue ? n.Comment.Mission.Title : "",
                applicationMissionTitle = n.MissionApplicationId.HasValue ? n.MissionApplication.Mission.Title : "",
                approvedStoryTitle = n.StoryId.HasValue ? n.Story.Title : "",
                recommendedStoryTitle = n.RecommendedStooyId.HasValue ? n.RecommendedStooy.Title : "",
                recommendedStoryMissionId = n.RecommendedStooyId.HasValue ? n.RecommendedStooy.Mission.MissionId : 0,
                recommendedStoryUserId = n.RecommendedStooyId.HasValue ? n.RecommendedStooy.User.UserId : 0,
                timesheetApprovalStatus = n.TimesheetId.HasValue ? n.Timesheet.Status : "",
                commentApprovalStatus = n.CommentId.HasValue ? n.Comment.ApprovalStatus : "",
                applicationApprovalStatus = n.MissionApplicationId.HasValue ? n.MissionApplication.ApprovalStatus : "",
                storyapprovalStatus = n.StoryId.HasValue ? n.Story.Status : ""
            });            

            return list.ToList();
        }


        public int ChangeNotificationStatus (long id)
        {
            int flag = 1;
            UserNotification notification = _db.UserNotifications.FirstOrDefault(n => n.NotificationId == id);
            if (notification.Status == true)
            {
                flag = 0;
            }
            notification.Status = true;
            _db.SaveChanges();
            return flag;
        }

        public void ClearAllNotifications (long userId)
        {
            List<UserNotification> notifications = _db.UserNotifications.Where(u => u.ToUserId == userId).ToList();
            foreach (var n in notifications)
            {
                n.DeletedAt = DateTime.Now;
            }
            _db.SaveChanges();
        }

        public long[] GetUserNotificationChanges (long userId)
        {
            long[] settingIds = _db.UserSettings.Where(u => u.UserId == userId && u.IsEnabled == true).Select(u => u.SettingId).ToArray();
            return settingIds;
        }

        public void SaveUserNotificationChanges (long userId, long[] settingIds)
        {
            for (int i = 1; i <= 7; i++)
            {
                UserSetting setting = _db.UserSettings.FirstOrDefault(u => u.UserId == userId && u.SettingId == i);
                if (settingIds.Contains(setting.SettingId))
                {
                    setting.IsEnabled = true;
                }
                else
                {
                    setting.IsEnabled = false;
                }
            }
            _db.SaveChanges();
        }
    }
}
