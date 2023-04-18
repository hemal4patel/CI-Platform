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
                cmsId = c.CmsPageId,
                title = c.Title,
                status = c.Status
            });

            return list.ToList();
        }

        public bool CmsExistsForNew (string slug)
        {
            return _db.CmsPages.Any(c => c.Slug.ToLower().Trim().Replace(" ", "") == slug.ToLower().Trim().Replace(" ", ""));
        }

        public bool CmsExistsForUpdate (long? cmsId, string slug)
        {
            return _db.CmsPages.Any(c => c.Slug.ToLower().Trim().Replace(" ", "") == slug.ToLower().Trim().Replace(" ", "") && c.CmsPageId != cmsId);
        }

        public void AddCmsPage (AdminCmsModel vm)
        {
            CmsPage newCmsPage = new CmsPage()
            {
                Title = vm.title,
                Description= vm.description,
                Slug= vm.slug,
                Status= vm.status,
                CreatedAt = DateTime.Now
            };

            _db.CmsPages.Add(newCmsPage);
            _db.SaveChanges();
        }

        public AdminCmsModel GetCmsToEdit (long cmsId)
        {
            IQueryable<CmsPage> cmsPage = _db.CmsPages.Where(c => c.CmsPageId == cmsId);

            AdminCmsModel list = cmsPage.Select(c => new AdminCmsModel()
            {
                cmsId = c.CmsPageId,
                title = c.Title,
                description = c.Description,
                slug = c.Slug,
                status = c.Status,
            }).FirstOrDefault();

            return list;
        }

        public void EditCmsPage (AdminCmsModel vm)
        {
            CmsPage existingCms = _db.CmsPages.Where(c => c.CmsPageId == vm.cmsId).FirstOrDefault();

            existingCms.Title = vm.title; 
            existingCms.Description = vm.description;
            existingCms.Slug = vm.slug.ToLower().Trim().Replace(" ","-");
            existingCms.Status = vm.status;
            existingCms.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
        }

        public void DeleteCmsPage (long cmsId)
        {
            CmsPage cmsPage = _db.CmsPages.FirstOrDefault(c => c.CmsPageId==cmsId);
            cmsPage.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
