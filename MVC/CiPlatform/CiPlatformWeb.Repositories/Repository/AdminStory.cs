using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CiPlatformWeb.Repositories.EnumStats;
using static System.Net.Mime.MediaTypeNames;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminStory : IAdminStory
    {
        private readonly ApplicationDbContext _db;

        public AdminStory (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminStoryModel> GetStories ()
        {
            IQueryable<Story> stories = _db.Stories.Where(s => s.Status != storyStatus.draft.ToString().ToUpper() && s.DeletedAt == null).AsQueryable();

            IQueryable<AdminStoryModel> list = stories.Select(s => new AdminStoryModel()
            {
                storyId = s.StoryId,
                storyTitle = s.Title,
                userName = s.User.FirstName + " " + s.User.LastName,
                missionTitle = s.Mission.Title,
                status = s.Status
            });

            return list.ToList();
        }

        public Story GetStoryDetails (long storyId)
        {
            return _db.Stories.Where(s => s.StoryId == storyId)
                .Include(s => s.User)
                .Include(s => s.Mission)
                .Include(s => s.StoryMedia.Where(s => s.DeletedAt == null))
                .FirstOrDefault();
        }

        public void ChangeStoryStatus (long storyId, int status)
        {
            Story story = _db.Stories.Where(s => s.StoryId == storyId).FirstOrDefault();

            if (status == 0)
            {
                story.Status = storyStatus.declined.ToString().ToUpper();
                story.PublishedAt = null;
            }
            else
            {
                story.Status = storyStatus.published.ToString().ToUpper();
                story.PublishedAt = DateTime.Now;
            }
            story.UpdatedAt = DateTime.Now;

            UserSetting userSettingId = _db.UserSettings.Where(u => u.UserId == story.UserId && u.SettingId == (long) notifications.story).FirstOrDefault();

            if (_db.UserNotifications.Any(u => u.UserSettingId == userSettingId.UserSettingId && u.DeletedAt == null && u.StoryId == storyId))
            {
                UserNotification notification = _db.UserNotifications.FirstOrDefault(u => u.UserSettingId == userSettingId.UserSettingId && u.DeletedAt == null && u.StoryId == storyId);
                notification.Status = false;
                notification.CreatedAt = DateTime.Now;
            }
            else
            {
                UserNotification notification = new UserNotification()
                {
                    ToUserId = story.UserId,
                    StoryId = storyId,
                    Status = false,
                    CreatedAt = DateTime.Now,
                    UserSettingId = userSettingId.UserSettingId
                };
                _db.UserNotifications.Add(notification);
            }

            _db.SaveChanges();
        }

        public void DeleteStory (long storyId)
        {
            Story story = _db.Stories.FirstOrDefault(s => s.StoryId == storyId);
            story.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
