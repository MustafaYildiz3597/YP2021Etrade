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
    public class MusterilerRepo : RepositoryBase<MUSTERILER, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable CarilerPageDTList(CarilerPageDTListDTO model)
        {
            try
            {
                return (from m in db.MUSTERILER.Where(q => (q.IsDeleted ?? false) == false &&
                                (q.FIRMA_TIPI == model.FirmaTipiID || model.FirmaTipiID == null) &&
                                (q.ULKE == model.UlkeID || model.UlkeID == null)
                            )
                        join ft in db.FIRMA_TIPLERI on m.FIRMA_TIPI equals ft.ID into ftj
                        from ft in ftj.DefaultIfEmpty()
                        join ul in db.ULKELER on m.ULKE equals ul.UID into ulj
                        from ul in ulj.DefaultIfEmpty()
                        join sh in db.SEHIRLER on m.SEHIR equals sh.ID into shj
                        from sh in shj.DefaultIfEmpty()
                        select new
                        {
                            m.ID,
                            FirmaTipi = ft == null ? "" : ft.TITLE,
                            m.FIRMA_ADI,
                            Ulke = ul == null ? "" : ul.UNAME,
                            Sehir = sh == null ? "" : sh.NAME,
                            m.WEB_STESI,
                            m.FIRMNICK,
                            YetkiliKisiCount = m.YETKILI_KISILER.Where(q => (q.IsDeleted ?? false) == false).Count(),
                            AdresCount = m.ADRESLER.Where(q => (q.IsDeleted ?? false) == false).Count(),
                            ToplantiCount = m.TOPLANTILAR.Where(q => (q.IsDeleted ?? false) == false).Count()
                        }).AsQueryable();

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
                return (from m in db.MUSTERILER.Where(q => q.ID == id)
                        join ft in db.FIRMA_TIPLERI on m.FIRMA_TIPI equals ft.ID into ftj
                        from ft in ftj.DefaultIfEmpty()
                        join ul in db.ULKELER on m.ULKE equals ul.UID into ulj
                        from ul in ulj.DefaultIfEmpty()
                        join sh in db.SEHIRLER on m.SEHIR equals sh.ID into shj
                        from sh in shj.DefaultIfEmpty()
                        join nk in db.CariNakliyeOdemesi on m.CariNakliyeOdemesiID equals nk.ID into nkj
                        from nk in nkj.DefaultIfEmpty()
                        join os in db.CariOdemeSekli on m.CariOdemeSekliID equals os.ID into osj
                        from os in osj.DefaultIfEmpty()
                        join ts in db.CariTeslimatSekli on m.CariTeslimatSekliID equals ts.ID into tsj
                        from ts in tsj.DefaultIfEmpty()
                        join vd in db.CariVade on m.CariVadeID equals vd.ID into vdj
                        from vd in vdj.DefaultIfEmpty()
                        join vl in db.CariVadeAltRakam on m.CariVadeAltRakamID equals vl.ID into vlj
                        from vl in vlj.DefaultIfEmpty()
                        join cr in db.Member on m.CreatedBy equals cr.ID into crj
                        from cr in crj.DefaultIfEmpty()
                        join up in db.Member on m.UpdatedBy equals up.ID into upj
                        from up in upj.DefaultIfEmpty()
                        join dt in db.B2BDealerType on m.B2BDealerTypeID equals dt.ID into dtj
                        from dt in dtj.DefaultIfEmpty()
                        select new
                        {
                            m.ID,
                            m.FIRMA_TIPI,
                            FirmaTipiTitle = ft == null ? "" : ft.TITLE,
                            m.FIRMA_ADI,
                            m.FATURA_ADRESI,
                            m.TESLIMAT_ADRESI,
                            m.ADRES,
                            m.POSTA_KODU,
                            m.VERGI_DAIRESI,
                            m.VERGI_NUMARASI,
                            m.MARKA,
                            m.MAIL_ADRESI,
                            m.WEB_STESI,
                            m.GSM,
                            m.TEL_1,
                            m.TEL_2,
                            m.IMAGE,
                            m.NOT,
                            m.TARIH,
                            m.ULKE,
                            UlkeAd = ul == null ? "" : ul.UNAME,
                            m.SEHIR,
                            SehirAd = sh == null ? "" : sh.NAME,
                            m.CariVadeAltRakamID,
                            VADE_ALT_RAKAM = vl == null ? "" : vl.Name,
                            m.CariVadeID,
                            VADE = vd == null ? "" : vd.Name,
                            m.CariOdemeSekliID,
                            ODEME_SEKLI = os == null ? "" : os.Name,
                            m.CariTeslimatSekliID,
                            TESLIMAT_SEKLI = ts == null ? "" : ts.Name,
                            m.CariNakliyeOdemesiID,
                            NAKLIYE_ODEMESI = nk == null ? "" : nk.Name,
                            m.FIRMNICK,
                            m.IsB2BDealer,
                            m.B2BDealerTypeID,
                            B2BDealerType = dt == null ? "" : dt.Name,
                            m.CreatedOn,
                            CreatedBy = cr == null ? "" : cr.FirstName + " " + cr.LastName,
                            m.UpdatedOn,
                            UpdatedBy = up == null ? "" : up.FirstName + " " + up.LastName

                        }
                    ).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //todo: ad to log
                throw;
            }
        }

    }

}
