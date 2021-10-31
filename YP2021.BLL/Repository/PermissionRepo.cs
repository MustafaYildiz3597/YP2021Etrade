using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nero2021.Data;
using System.ComponentModel;
using Nero2021.BLL.Models;

namespace Nero2021.BLL.Repository
{
    public class PermissionRepo : RepositoryBase<Permission, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable<Nero2021.Data.Action> MenuItemList(string userId, bool alllist)
        {
            try
            {
                var list = (from a in db.Action.ToList()
                            join p in db.Permission.Where(q => q.UserID == userId) on a.ID equals p.ActionID into p1
                            from p in p1.DefaultIfEmpty()
                            where (p != null || alllist == true)
                            select new Nero2021.Data.Action()
                            {
                                ID = a.ID,
                                PageUrl = a.PageUrl,
                                Name = a.Name,
                                RankNumber = a.RankNumber
                            }).OrderBy(o=>o.RankNumber).AsQueryable<Nero2021.Data.Action>();

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
