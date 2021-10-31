using Microsoft.AspNet.Identity;
using Nero2021.BLL.Models;
using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nero2021.BLL.Repository
{
    public class OfferRepo : RepositoryBase<TEKLIFLER, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable SorguPageDTList()
        {
            try
            {
                return (from od in db.TEKLIFLER_DETAY
                        join p in db.PRODUCTS on od.PRODID equals p.PRODID into pj
                        from p in pj.DefaultIfEmpty()
                        join o in db.TEKLIFLER on (od.TID ?? 0) equals o.TID
                        join m in db.MUSTERILER on o.MusteriID equals m.ID
                        join cr in db.Currency on od.CURRENCY equals cr.ID into crj
                        from cr in crj.DefaultIfEmpty()
                        select new
                        {
                            o.TEKLIFNO,
                            m.FIRMA_ADI,
                            ProductName = p == null ? "" : p.NAME,
                            od.CustomerCode,
                            p.BUKOD,
                            //od.BUESKI,
                            od.Quantity,
                            od.UnitPrice,
                            CurrencyCode = cr == null ? "" : cr.Code,
                            o.ADD_DATE
                        }).AsQueryable();

            }
            catch (Exception)
            {

                throw;
            }

        }


        public IQueryable ListDT(OfferDTListDTO model)
        {
            try
            {
                var list = (from t in db.TEKLIFLER.Where(q => (q.IsDeleted ?? false) == false)
                            join tt in db.TeklifTipi on (t.TTIPI ?? 0) equals tt.ID into tt1
                            from tt in tt1.DefaultIfEmpty()
                            join f in db.MUSTERILER on t.MusteriID equals f.ID into f1
                            from f in f1.DefaultIfEmpty()
                            join m in db.Member on t.CreatedBy equals m.ID into m1
                            from m in m1.DefaultIfEmpty()
                            join td in db.TeklifDurumu on t.TDURUMU equals td.ID into td1
                            from td in td1.DefaultIfEmpty()
                            where (t.TTIPI == model.TeklifTipiID || model.TeklifTipiID == null)
                                && (t.MusteriID == model.FirmID || model.FirmID == null)
                            select new
                            {
                                t.TID,
                                t.TEKLIFNO,
                                TeklifTipi = tt == null ? "" : tt.Name,
                                FirmaAdi = f == null ? "" : f.FIRMA_ADI,
                                Konu = t.TITLE,
                                Hazirlayan = m == null ? "" : m.FirstName + " " + m.LastName,
                                EklenmeTarihi = t.CreatedOn,
                                TeklifDurumu = td == null ? "" : td.Name
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
                return (from t in db.TEKLIFLER.Where(q => q.TID == id)
                        join tt in db.TeklifTipi on (t.TTIPI ?? 0) equals tt.ID into tt1
                        from tt in tt1.DefaultIfEmpty()
                        join f in db.MUSTERILER on t.MusteriID equals f.ID into f1
                        from f in f1.DefaultIfEmpty()
                        join m in db.Member on t.CreatedBy equals m.ID into m1
                        from m in m1.DefaultIfEmpty()
                        join mu in db.Member on t.UpdatedBy equals mu.ID into muj
                        from mu in muj.DefaultIfEmpty()

                        join td in db.TeklifDurumu on t.TDURUMU equals td.ID into td1
                        from td in td1.DefaultIfEmpty()
                        join yk in db.YETKILI_KISILER on t.YetkiliKisiID equals yk.ID into yk1
                        from yk in yk1.DefaultIfEmpty()
                        select new
                        {
                            t.TID,
                            t.TEKLIFNO,
                            TeklifTipi = tt == null ? "" : tt.Name,
                            FirmaAdi = f == null ? "" : f.FIRMA_ADI,
                            t.TITLE,
                            Hazirlayan = m == null ? "" : m.FirstName + " " + m.LastName,
                            Guncelleyen = mu == null ? "" : mu.FirstName + " " + mu.LastName,
                            t.CreatedOn,
                            t.UpdatedOn,
                            TeklifDurumu = td == null ? "" : td.Name,
                            t.ICERIK,
                            t.MusteriID,
                            t.TDURUMU,
                            t.TTIPI,
                            t.YetkiliKisiID,
                            YetkiliKisi = yk == null ? "" : yk.ADI + " " + yk.SOYADI,
                            TeklifItems = t.TEKLIFLER_DETAY.Where(q => (q.IsDeleted ?? false) == false)
                            .Select(s => new
                            {
                                s.TID,
                                s.TDID,
                                s.CustomerCode,
                                s.ProductID,
                                s.BuCode,
                                s.Oem,
                                s.Oem1,
                                s.Name,
                                s.Detay,
                                s.Quantity,
                                s.UnitPrice,
                                s.CURRENCY,
                                CurrencyCode = s.Currency1 == null ? "" : s.Currency1.Code
                            })
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                //TODO: add to log
                throw;
            }

        }


    }
}
