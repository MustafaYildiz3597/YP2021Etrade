using Microsoft.AspNet.Identity;
using Nero2021.BLL.Models;
using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nero2021.BLL.Repository
{
    public class UserLevelsRepo : RepositoryBase<UserLevels, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable ListDT()
        {
            try
            {
                int order1 = 1;

                return (from ul in db.UserLevels.Where(q => (q.IsDeleted ?? false) == false).ToList()
                        orderby ul.UserLevelID <= 0 ? ul.UserLevelID : order1, ul.UserLevelName
                        select new
                        {
                            ul.UserLevelID,
                            ul.UserLevelName,
                            ul.IsActive
                            // orderkey = ul.UserLevelID <= 0 ? ul.UserLevelID : order1
                        }).AsQueryable();
                //    .OrderBy(o => o.orderkey)
                //.ThenBy(o => o.UserLevelName).AsQueryable();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Object FindByID(int id)
        {
            try
            {
                return (from ul in db.UserLevels.Where(q => q.UserLevelID == id)
                        select new
                        {
                            ul.UserLevelID,
                            ul.UserLevelName,
                            ul.IsActive
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                //TODO: add to log
                throw;
            }

        }

        public IQueryable GetPermissions(UserLevelGetPermissionsDTO model)
        {
            try
            {
                int order1 = 1;

                var list = (from a in db.Action.Where(q => q.IsEnabled == true)
                            join p in db.UserLevelPermission.Where(q => q.UserLevelID == model.UserLevelID) on a.ID equals p.ActionID into pj
                            from p in pj.DefaultIfEmpty()

                            select new
                            {
                                a.ID,
                                Name = (a.MenuItemLevel == 1 ? "" : "---") + a.Name,
                                a.MenuItemLevel,
                                a.ParentRankNumber,
                                a.RankNumber,
                                a.HasAddPermission,
                                p.AddPermission,
                                a.HasDeletePermission,
                                p.DeletePermission,
                                a.HasExecutePermission,
                                p.ExecutePermission,
                                a.HasSearchPermission,
                                p.SearchPermission,
                                a.HasUpdatePermission,
                                p.UpdatePermission,
                                a.HasViewPermission,
                                p.ViewPermission
                            }).AsQueryable().OrderBy(o => o.ParentRankNumber).ThenBy(o => o.RankNumber);

                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Result SetPermissions(UserLevelSetPermissionsDTO model)
        {
            try
            {
                Result retval = new Result();

                foreach (var item in model.UserLevelPermissions.ToList())
                {
                    var permission = new UserLevelPermissionRepo().GetByFilter(q => q.ActionID == item.ID && q.UserLevelID == model.UserLevelID);
                    if (permission == null)
                    {
                        var newitem = new UserLevelPermission()
                        {
                            ID = Guid.NewGuid(),
                            ActionID = item.ID,
                            AddPermission = item.AddPermission,
                            DeletePermission = item.DeletePermission,
                            ExecutePermission = item.ExecutePermission,
                            SearchPermission = item.SearchPermission,
                            UpdatePermission = item.UpdatePermission,
                            UserLevelID = model.UserLevelID,
                            ViewPermission = item.ViewPermission
                        };
                        new UserLevelPermissionRepo().Insert(newitem);
                    }
                    else
                    {

                        permission.AddPermission = item.AddPermission;
                        permission.DeletePermission = item.DeletePermission;
                        permission.ExecutePermission = item.ExecutePermission;
                        permission.SearchPermission = item.SearchPermission;
                        permission.UpdatePermission = item.UpdatePermission;
                        permission.ViewPermission = item.ViewPermission;

                        new UserLevelPermissionRepo().Update();
                    }
                }

                retval.Message = "Yetki izinleri kaydedildi.";

                return retval;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<GetUserPermissions_Result> GetUserPermissions()
        {
            string userID = HttpContext.Current.User.Identity.GetUserId();
            return db.GetUserPermissions(userID).ToList();
        }

    }
}
