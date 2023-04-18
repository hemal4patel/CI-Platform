using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminTheme
    {
        public List<AdminThemeModel> GetThemes();

        public AdminThemeModel GetThemeToEdit (long themeId);

        public bool ThemeExistsForNew (string themeName);

        public bool ThemeExistsForUpdate(long? themeId, string themeName);

        public void AddNewTheme (AdminThemeModel vm);

        public void UpdateTheme (AdminThemeModel vm);

        public void DeleteTheme (long themeId);
    }
}
