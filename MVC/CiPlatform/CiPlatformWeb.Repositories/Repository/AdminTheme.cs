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
                themeId = t.MissionThemeId,
                themeName = t.Title,
                status = t.Status,
            });

            return list.ToList();
        }

        public AdminThemeModel GetThemeToEdit (long themeId)
        {
            IQueryable<MissionTheme> theme = _db.MissionThemes.Where(t => t.MissionThemeId == themeId);

            AdminThemeModel list = theme.Select(t => new AdminThemeModel()
            {
                themeId = t.MissionThemeId,
                themeName = t.Title,
                status = t.Status
            }).FirstOrDefault();

            return list;
        }

        public bool ThemeExistsForNew (string themeName)
        {
            return _db.MissionThemes.Any(t => t.Title.ToLower().Trim().Replace(" ", "") == themeName.ToLower().Trim().Replace(" ", ""));
        }

        public bool ThemeExistsForUpdate (long? themeId, string themeName)
        {
            return _db.MissionThemes.Any(t => t.Title.ToLower().Trim().Replace(" ", "") == themeName.ToLower().Trim().Replace(" ", "") && t.MissionThemeId != themeId);
        }

        public void AddNewTheme (AdminThemeModel vm)
        {
            MissionTheme newTheme = new MissionTheme()
            {
                Title = vm.themeName,
                Status = vm.status,
                CreatedAt = DateTime.Now
            };

            _db.MissionThemes.Add(newTheme);
            _db.SaveChanges();
        }

        public void UpdateTheme (AdminThemeModel vm)
        {
            MissionTheme existingTheme = _db.MissionThemes.Where(s => s.MissionThemeId == vm.themeId).FirstOrDefault();

            existingTheme.Title = vm.themeName;
            existingTheme.Status = vm.status;
            existingTheme.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
        }

        public void DeleteTheme (long themeId)
        {
            MissionTheme theme = _db.MissionThemes.FirstOrDefault(t => t.MissionThemeId == themeId);
            theme.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
