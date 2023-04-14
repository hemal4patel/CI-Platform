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
    public class AdminTheme : IAdminTheme
    {
        private readonly ApplicationDbContext _db;

        public AdminTheme (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminThemeModel> GetThemes ()
        {
            IQueryable<MissionTheme> themes = _db.MissionThemes.Where(t => t.DeletedAt == null).AsQueryable();

            IQueryable<AdminThemeModel> list = themes.Select(t => new AdminThemeModel()
            {
                themeName = t.Title,
                status = t.Status,
            });

            return list.ToList();
        }

    }
}
