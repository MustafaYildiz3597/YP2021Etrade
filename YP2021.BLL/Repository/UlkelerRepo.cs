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
    public class UlkelerRepo : RepositoryBase<ULKELER, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable ListDT()
        {
            try
            {
                return (from t in db.ULKELER.Where(q => (q.IsDeleted ?? false) == false).ToList()
                        select new
                        {
                            t.UID,
                            t.UNAME
                        }).AsQueryable();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
