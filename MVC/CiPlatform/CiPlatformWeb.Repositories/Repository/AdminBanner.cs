using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminBanner : IAdminBanner
    {
        private readonly ApplicationDbContext _db;

        public AdminBanner (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminBannerModel> GetBanners ()
        {
            IQueryable<Banner> banners = _db.Banners.Where(b => b.DeletedAt == null).AsQueryable();

            IQueryable<AdminBannerModel> list = banners.Select(b => new AdminBannerModel
            {
                bannerId = b.BannerId,
                imageName = b.Image,
                title= b.Title,
                text = b.Text,
                sortOrder = b.SortOrder,
            });

            return list.ToList();
        }

        public AdminBannerModel GetBannerToEdit (long bannerId)
        {
            IQueryable<Banner> Banner = _db.Banners.Where(b => b.BannerId == bannerId);
            AdminBannerModel list = Banner.Select(b => new AdminBannerModel()
            {
                bannerId = b.BannerId,
                title = b.Title,
                imageName = b.Image,
                sortOrder = b.SortOrder,
                text = b.Text

            }).FirstOrDefault();
            return list;
        }

        public void AddNewBanner (AdminBannerModel vm)
        {
            string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + vm.image.FileName;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Banner", fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                vm.image.CopyTo(stream);
            }

            Banner banner = new Banner()
            {
                Image = fileName,
                Title = vm.title,
                Text = vm.text,
                SortOrder = vm.sortOrder,
                CreatedAt = DateTime.Now,
            };
            _db.Banners.Add(banner);
            _db.SaveChanges();
        }

        public void UpdateBanner (AdminBannerModel vm)
        {
            Banner banner = _db.Banners.FirstOrDefault(b => b.BannerId == vm.bannerId);
            banner.Title= vm.title;
            banner.Text = vm.text;
            banner.SortOrder = vm.sortOrder;
            banner.UpdatedAt = DateTime.Now;
            if (vm.image is not null)
            {
                string deleteFile = banner.Image;
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Banner", deleteFile));
                string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + vm.image.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Banner", fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    vm.image.CopyTo(stream);
                }
                banner.Image = fileName;
            }
            _db.SaveChanges();
        }

        public void DeleteBanner (long bannerId)
        {
            Banner banner = _db.Banners.FirstOrDefault(b => b.BannerId == bannerId);
            banner.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }
    }
}
