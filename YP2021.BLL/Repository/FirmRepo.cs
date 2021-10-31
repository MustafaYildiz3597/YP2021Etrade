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
    public class FirmRepo : RepositoryBase<Nero2021.Data.Firm, Guid>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable List()
        {
            DateTime criDT = DateTime.Now.Date.AddDays(1);
            var list = (from f in db.Firm
                        select new
                        {
                            f.ID,
                            f.Title,
                            f.ContactName,
                            f.ContactGSMNo,
                            f.ContactEmail,
                            KEPStatus = (f.KEPExpirationDT ?? DateTime.MinValue) <= criDT ? "Süre Sonu" : f.KEPStatus == true ? "Aktif" : "Hizmet Dışı",
                            f.KEPExpirationDT,
                            f.KEPMemberMaxLimit,
                            f.CreateDT,
                            CreateFullName = f.Member.FirstName + " " + f.Member.LastName
                        }).OrderBy(o => o.Title
                       ).AsQueryable();

            return list;
        }

        public object GetRow(Guid id)
        {
            var list = (from f in db.Firm
                        where f.ID == id
                        select new
                        {
                            f.ID,
                            f.Title,
                            f.Address,
                            f.City,
                            f.ContactName,
                            f.ContactGSMNo,
                            f.ContactEmail,
                            f.KEPEmail,
                            f.PhoneNo,
                            f.TaxNumber,
                            f.TaxOffice,
                            f.KEPMemberMaxLimit,
                            f.KEPMemberCount,
                            f.KEPExpirationDT, 
                            //= f.KEPExpirationDT.HasValue ? 
                            //    ("00" + f.KEPExpirationDT.Value.Day).Substring(("00" + f.KEPExpirationDT.Value.Day).Length - 2) + "." + 
                            //    ("00" + f.KEPExpirationDT.Value.Month).Substring(("00" + f.KEPExpirationDT.Value.Month).Length - 2) + "." + 
                            //    f.KEPExpirationDT.Value.Year : "", //f.KEPExpirationDT != null ? String.Format("{0}:dd/MM/yyyy ", f.KEPExpirationDT) : "", //.HasValue ? f.KEPExpirationDT.Value.ToString("dd-MM-yyyy") : "[N/A]",
                            f.KEPStatus,
                            f.CreateDT,
                            CreateFullName = f.Member.FirstName + " " + f.Member.LastName
                        }).AsQueryable().FirstOrDefault();

            return list;
        }


    }

}
