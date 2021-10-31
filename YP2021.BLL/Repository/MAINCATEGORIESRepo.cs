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
    public class MAINCATEGORIESRepo : RepositoryBase<MAINCATEGORIES, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable ListDT(AnaKategoriListDTDTO model)
        {
            try
            {
                return (from t in db.MAINCATEGORIES.Where(q => (q.SECID == model.SECID || (model.SECID ?? 0) == 0) && (q.IsDeleted ?? false) == false ).ToList()
                        select new
                        {
                            t.MCATID,
                            t.SECID,
                            SECNAME = t.SECTIONS == null ? "" : t.SECTIONS.NAME,
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
