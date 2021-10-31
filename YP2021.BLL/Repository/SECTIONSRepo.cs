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
    public class SECTIONSRepo : RepositoryBase<SECTIONS, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable ListDT()
        {
            try
            {
                return (from t in db.SECTIONS.Where(q => (q.IsDeleted ?? false) == false).ToList()
                        select new
                        {
                            t.SECID,
                            t.NAME,
                            t.NAME_DE,
                            t.NAME_EN,
                            t.NAME_FR,
                            t.ENABLED,
                            t.SORT
                        }).AsQueryable();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
