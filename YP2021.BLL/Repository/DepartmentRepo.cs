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
    public class DepartmentRepo : RepositoryBase<Department, string>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable List()
        {
            var list = (from d in db.Department 
                        select new
                        {
                            d.ID,
                            d.Name
                        }).OrderBy(o=>o.Name).AsQueryable();

            return list;
        }


    }

}
