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
    public class AdminStory : IAdminStory
    {
        private readonly ApplicationDbContext _db;

        public AdminStory (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminStoryModel> GetStories ()
        {
            IQueryable<Story> stories = _db.Stories.Where(s => s.Status != "DRAFT" && s.DeletedAt == null).AsQueryable();

            IQueryable<AdminStoryModel> list = stories.Select(s => new AdminStoryModel()
            {
                storyTitle = s.Title,
                userName = s.User.FirstName + " " + s.User.LastName,
                missionTitle = s.Mission.Title,
                status = s.Status
            });

            return list.ToList();
        }
    }
}
