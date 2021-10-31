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
    public class SehirlerRepo : RepositoryBase<SEHIRLER, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable ListDT(SehirListDTDTO model)
        {
            try
            {
                return (from t in db.SEHIRLER.Where(q => (q.UID == model.UID || (model.UID ?? 0) == 0) && (q.IsDeleted ?? false) == false).ToList()
                        select new
                        {
                            t.ID,
                            t.UID,
                            CityName = t.NAME,
                            CountryName = t.ULKELER == null ? "" : t.ULKELER.UNAME
                        }).AsQueryable();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
