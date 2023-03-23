using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class StoryList : IStoryList
    {
        private readonly ApplicationDbContext _db;

        public StoryList (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Story> GetStories (List<long> StoryIds)
        {
            return _db.Stories.Where(s => StoryIds.Contains(s.StoryId))
                .Include(s => s.StoryMedia)
                .Include(s => s.Mission)
                .ThenInclude(s => s.Theme)
                .Include(s => s.User)
                .ToList();
        }

        public List<Mission> GetMissions ()
        {
            return _db.Missions.ToList();
        }

        public Story GetDraftedStory (long missionId, long userId)
        {
            return _db.Stories.Where(s => s.MissionId == missionId && s.UserId == userId && s.Status == "DRAFT").FirstOrDefault();
        }

        public Story GetDraftedStory (long StoryId)
        {
            return _db.Stories.Where(s => s.StoryId == StoryId).FirstOrDefault();
        }

        public bool CheckPublishedStory (long MissionId, long userId)
        {
            return _db.Stories.Any(s => s.MissionId == MissionId && s.UserId == userId && s.Status == "PUBLISHED");
        }


    }
}
