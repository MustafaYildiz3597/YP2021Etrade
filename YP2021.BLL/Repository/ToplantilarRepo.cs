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
    public class ToplantilarRepo : RepositoryBase<TOPLANTILAR, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable ListDT(MeetingDTListDTO model)
        {
            try
            {
                var list = new ToplantilarRepo().GetAll(q => (q.FIRMID == model.FirmID || model.FirmID == null) && (q.IsDeleted ?? false) == false)
                    .Select(s => new
                    {
                        s.ID,
                        s.FIRMID,
                        s.MUSTERILER.FIRMA_ADI,
                        ToplantiMembers = s.ToplantiMember.Select(s1 => new { FullName = s1.Member?.FirstName + " " + s1.Member?.LastName }),
                        ToplantiYetkiliKisiler = s.ToplantiYetkiliKisi.Select(s2 => new { s2.ID, FullName = s2.YETKILI_KISILER?.ADI + " " + s2.YETKILI_KISILER?.SOYADI }),
                        s.TARIH,
                        s.TITLE,
                        s.SEBEP,
                        //s.ICERIK,
                        ToplantiSebep = s.ToplantiSebep?.Title,
                        s.CreatedOn
                    }).AsQueryable();

                return list;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Object FindByID(int id)
        {
            try
            {
                return (new ToplantilarRepo().GetAll(q => q.ID == id)
                    .Select(s => new
                    {
                        s.ID,
                        s.FIRMID,
                        s.MUSTERILER?.FIRMA_ADI,
                        ToplantiMembers = s.ToplantiMember.Select(s1 => new { s1.ID, s1.MemberID, FullName = s1.Member?.FirstName + " " + s1.Member?.LastName }),
                        ToplantiYetkiliKisiler = s.ToplantiYetkiliKisi.Select(s2 => new { s2.ID, s2.YetkiliKisiID, FullName = s2.YETKILI_KISILER?.ADI + " " + s2.YETKILI_KISILER?.SOYADI }),
                        s.TARIH,
                        s.TITLE,
                        s.SEBEP,
                        s.ICERIK,
                        ToplantiSebep = s.ToplantiSebep?.Title,
                        s.CreatedOn,
                        CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName,
                        s.UpdatedOn,
                        UpdatedBy = s.Member1?.FirstName + " " + s.Member1?.LastName
                    }).FirstOrDefault()
                    );
            }
            catch (Exception ex)
            {
                //todo: ad to log
                throw;
            }
        }

    }
}
