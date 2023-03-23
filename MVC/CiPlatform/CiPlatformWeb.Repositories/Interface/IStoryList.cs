using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IStoryList
    {
        public List<Story> GetStories (List<long> StoryIds);

        public List<Mission> GetMissions();

        public Story GetDraftedStory (long MissionId, long userId);

        public Story GetDraftedStory (long StoryId);

        public bool CheckPublishedStory (long MissionId, long userId);
    }
}
