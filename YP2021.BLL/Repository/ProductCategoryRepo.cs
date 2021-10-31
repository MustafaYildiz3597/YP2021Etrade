using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nero2021.BLL.Repository
{
    public class ProductCategoryRepo : RepositoryBase<CATEGORIES, int>
    {
        //private NeroDBEntities _db = new NeroDBEntities();

        //public IQueryable SectionList()
        //{
        //    return (
        //        from p in _db.ProductCategory.Where(q => q.NodeLevel == 1)
        //        select new
        //        {
        //            p.ID,
        //            p.Name
        //        }).OrderBy(o => o.Name);
        //}

        //public IQueryable CategoryListByParentID(int parentID)
        //{
        //    return (
        //        from p in _db.ProductCategory.Where(q => (q.ParentID ?? 0) == parentID && (q.IsDeleted ?? false) == false)
        //        select new
        //        {
        //            p.ID,
        //            p.Name
        //        }).OrderBy(o => o.Name);
        //}

         

    }
}
