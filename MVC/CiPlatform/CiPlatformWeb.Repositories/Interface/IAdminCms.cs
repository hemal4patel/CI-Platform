using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminCms
    {
        public List<AdminCmsModel> GetCmsPages ();

        public bool CmsExistsForNew (string slug);

        public bool CmsExistsForUpdate (long? cmsId, string slug);

        public void AddCmsPage (AdminCmsModel model);

        public AdminCmsModel GetCmsToEdit (long cmsId);

        public void EditCmsPage (AdminCmsModel model);
    }
}
