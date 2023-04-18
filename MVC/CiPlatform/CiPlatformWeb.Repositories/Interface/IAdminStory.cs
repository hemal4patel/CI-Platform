using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminStory
    {
        public List<AdminStoryModel> GetStories();

        public void ChangeStoryStatus (long storyId, int status);

        public void DeleteStory (long storyId);
    }
}
