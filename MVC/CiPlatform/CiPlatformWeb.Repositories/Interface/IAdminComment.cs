using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminComment
    {
        public List<AdminCommentModel> GetComments ();

        public void ChangeCommentStatus (long commentId, int status);
    }
}
