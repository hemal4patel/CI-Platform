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
    public class AdminCms : IAdminCms
    {
        public readonly ApplicationDbContext _db;

        public AdminCms (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminCmsModel> GetCmsPages ()
        {
            IQueryable<CmsPage> pages = _db.CmsPages.Where(c => c.DeletedAt == null).AsQueryable();

            IQueryable<AdminCmsModel> list = pages.Select(c => new AdminCmsModel()
            {
                title = c.Title,
                status = c.Status
            });

            return list.ToList();
        }

    }
}
