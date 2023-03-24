using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
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

        public List<MissionApplication> GetMissions(long userId);

        public Story GetDraftedStory (long MissionId, long userId);

        public bool CheckPublishedStory (long MissionId, long userId);

        //public void DeleteVideoUrls (long storyId);

        //public void AddVideoUrls(long storyId, string[] url);

        public void UpdateDraftedStory (ShareStoryViewModel viewmodel, Story draftedStory);

        public void UpdateStoryImages (long storyId, string[] images);

        public void UpdateStoryUrls (long storyId, string[] url);

        public void AddNewStory (ShareStoryViewModel viewmodel, long userId);


        public void SubmitStory (ShareStoryViewModel viewmodel, Story draftedStory);
    }
}
