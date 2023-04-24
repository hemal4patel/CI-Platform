using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminBanner
    {
        public List<AdminBannerModel> GetBanners ();

        public AdminBannerModel GetBannerToEdit (long bannerId);

        public void AddNewBanner(AdminBannerModel vm);

        public void UpdateBanner(AdminBannerModel vm);

        public void DeleteBanner (long bannerId);
    }
}
