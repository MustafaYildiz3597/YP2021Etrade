using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Nero2021.BLL.Utilities;
using Nero2021.Data;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
using System.Web;
using System.Web.Http;

//using static TurkBelge.Controllers.PersonelController;

namespace Nero2021.Web.api
{
    //[Authorize]
    public class MProductsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult BrowseList(MPBrowseList model)
        {
            if (model == null)
                return BadRequest("Parametre hatası!.");

            if (model.MusteriID == null)
                return BadRequest("Parametre hatası!.");

            try
            {
                var mproducts = new MProductsRepo().GetAll(q => q.MusteriID == model.MusteriID && (q.IsDeleted ?? false) == false).Select(s => new
                {
                    s.MPID,
                    s.BUKOD,
                    s.NAME,
                    s.XPSNO,
                    s.OEM,
                    s.PRICE,
                    s.CURRENCY,
                    CurrencyCode = s.Currency1?.Code
                }).AsQueryable();

                return Ok(new
                {
                    MProducts = mproducts
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Veriler yüklenemedi.");
            }
        }

        [HttpPost]
        public IHttpActionResult ExcelImportFillAllCmb()
        {
            try
            {
                var oemsupliers = new OEMSupplierRepo().GetAll().Select(s => new { s.SUPID, s.SUPNAME }).OrderBy(o => o.SUPNAME).AsQueryable();
                var musteriler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false && ((q.FIRMA_TIPI == (int)FirmaTipleri.Müşteri) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi))).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var tedarikciler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false && ((q.FIRMA_TIPI == (int)FirmaTipleri.Tedarikçi) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi))).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).OrderBy(o => o.ID).AsQueryable();

                return Ok(new
                {
                    Oemsupliers = oemsupliers,
                    Musteriler = musteriler,
                    Tedarikciler = tedarikciler,
                    Currencies = currencies
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult ImportExcelUploadData(MPImportExcelUploadDataDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                if (model.MusteriID == null)
                    return BadRequest("Müşteri seçimi yapmalısınız.");

                if (model.Data == null)
                    return BadRequest("Data bulunamadı.");

                if (model.Data.Count() == 0)
                    return BadRequest("Data bulunamadı.");

                var musteri = new MusterilerRepo().GetByFilter(q => q.ID == (model.MusteriID ?? 0) && (q.IsDeleted ?? false) == false);
                if (musteri == null)
                    return BadRequest("Müşteri kaydına ulaşılamadı.");

                Result retval = new MProductsRepo().ImportExcelData(model);

                //if (!retval.IsSuccess)
                //    return BadRequest(retval.Message);

                return Ok(retval);
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult DTList(MPListDTDTO model)
        {
            try
            {
                var draw = HttpContext.Current.Request.Form.GetValues("draw").FirstOrDefault();
                //paging parameter
                var start = HttpContext.Current.Request.Form.GetValues("start").FirstOrDefault();
                var length = HttpContext.Current.Request.Form.GetValues("length").FirstOrDefault();
                //sorting parameter
                var sortColumn = HttpContext.Current.Request.Form.GetValues("columns[" + HttpContext.Current.Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var sortColumnDir = HttpContext.Current.Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //filter parameter
                var searchValue = HttpContext.Current.Request.Form.GetValues("search[value]").FirstOrDefault();
                //var allCustomer = new System.Collections.Generic.List();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                model.searchitems = new MPDTListSearchItems();

                //string hasimage = HttpContext.Current.Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                //if (Int32.TryParse(hasimage, out int i))
                //    model.searchitems.HasImage = i;

                model.searchitems.BUKod = HttpContext.Current.Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                model.searchitems.FIRMA_ADI = HttpContext.Current.Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                model.searchitems.XPSNO = HttpContext.Current.Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
                model.searchitems.NAME = HttpContext.Current.Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
                model.searchitems.CreatedOn = HttpContext.Current.Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
                model.searchitems.UPDATED = HttpContext.Current.Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
                model.searchitems.PRICE = HttpContext.Current.Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
                model.searchitems.CurrencyCode = HttpContext.Current.Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();

                model.pageNumber = skip / pageSize;
                model.pageSize = pageSize;
                model.sortColumn = sortColumn;
                model.sortColumnDir = sortColumnDir;
                model.searchValue = searchValue.ToString();

                var counts = new MProductsRepo().ListDTCount(model);
                var data = new MProductsRepo().ListDT(model);

                recordsTotal = data.Count();

                return Ok(new { draw, recordsFiltered = counts[0].RecordFilteredCount, recordsTotal = counts[0].RecordCount, data });

            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }

            #region old list
            //try
            //{
            //    var mulist = new MProductsRepo().GetAll(q => (q.IsDeleted ?? false) == false && 
            //        (q.MusteriID == model.MusteriID || model.MusteriID == null) && 
            //        (q.BUKOD.Contains(model.ProductSearchText) || q.XPSNO.Contains(model.ProductSearchText) || string.IsNullOrEmpty(model.ProductSearchText))
            //        ).Select(s => new
            //    {
            //        s.MPID,
            //        BUKOD = s.BUKOD ?? "",
            //        s.ProductID,
            //        s.MUSTERILER?.FIRMA_ADI,
            //        s.XPSNO,
            //        s.NAME,
            //            CreatedOn =  s.CreatedOn == null ? "" : s.CreatedOn.Value.ToString("yyyy-MM-dd"),
            //        s.UPDATED,
            //        s.PRICE,
            //        s.CURRENCY,
            //        CurrencyCode = s.Currency1?.Code
            //    }).AsQueryable();

            //    return Ok(new
            //    {
            //        MUlist = mulist
            //    });
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest("Bir hata oluştu. " + ex.Message);
            //}
            #endregion
        }

        [HttpPost]
        public IHttpActionResult ListPageFillAllCmb()
        {
            try
            {
                // var oemsupliers = new OEMSupplierRepo().GetAll().Select(s => new { s.SUPID, s.SUPNAME }).OrderBy(o => o.SUPNAME).AsQueryable();
                var musteriler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false && ((q.FIRMA_TIPI == (int)FirmaTipleri.Müşteri) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi))).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                // var tedarikciler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false && ((q.FIRMA_TIPI == (int)FirmaTipleri.Tedarikçi) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi))).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).OrderBy(o => o.ID).AsQueryable();
                //var mulist = new MProductsRepo().GetAll().Select(s => new
                //{
                //    s.MPID,
                //    BUKOD = s.BUKOD ?? "",
                //    s.ProductID,
                //    s.MUSTERILER?.FIRMA_ADI,
                //    XPSNO = s.XPSNO ?? "",
                //    NAME = s.NAME ?? "",
                //    CreatedOn = s.CreatedOn == null ? "" : s.CreatedOn.Value.ToString("yyyy-MM-dd"),
                //    UPDATED = s.UPDATED == null ? "" : s.UPDATED.Value.ToString("yyyy-MM-dd"),
                //    s.PRICE,
                //    s.CURRENCY,
                //    CurrencyCode = s.Currency1?.Code
                //}).AsQueryable();

                //var products = new ProductRepo().GetAll(q => (q.Deleted ?? false) == false).Select(s => new
                //{
                //    s.PRODID,
                //    s.BUKOD,
                //    s.NAME
                //}).AsQueryable();

                return Ok(new
                {
                    // Oemsupliers = oemsupliers,
                    Musteriler = musteriler,
                    // Tedarikciler = tedarikciler,
                    Currencies = currencies 
                    //MUlist = mulist
                    //Products = products
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        public IHttpActionResult SaveGridEdit(SaveMUGridEditDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            try
            {
                Result retval = new MProductsRepo().SaveGridEdit(model);

                if (!retval.IsSuccess)
                    return BadRequest(retval.Message);

                return Ok(retval);
            }
            catch (Exception ex)
            {
                //TODO: loga ekle
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }

        }

        public IHttpActionResult SaveGridAdd(SaveMUGridAddDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            try
            {
                Result retval = new MProductsRepo().SaveGridAdd(model);

                if (!retval.IsSuccess)
                    return BadRequest(retval.Message);

                return Ok(retval);
            }
            catch (Exception ex)
            {
                //TODO: loga ekle
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult Detail(MProductsDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var mproduct = new MProductsRepo().FindByID(model.ID ?? 0);

                return Ok(new
                {
                    MProductDetail = mproduct
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        public IHttpActionResult Save(SaveMProductDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            try
            {
                Result retval = new MProductsRepo().Save(model);

                if (!retval.IsSuccess)
                    return BadRequest(retval.Message);

                return Ok(retval);
            }
            catch (Exception ex)
            {
                //TODO: loga ekle
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult Delete(MProductDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Müşteri Ürün silindi.";

                var mproduct = new MProductsRepo().GetByID(model.ID ?? 0);
                if (mproduct == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                //mproduct.IsDeleted = true;
                //mproduct.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                //mproduct.DeletedOn = DateTime.Now;
                new MProductsRepo().Delete(mproduct);

                new MProductsLogRepo().Insert(new MProductsLog()
                {
                    ADDED = mproduct.ADDED,
                    BUKOD = mproduct.BUKOD,
                    CreatedBy = mproduct.CreatedBy,
                    CreatedOn = mproduct.CreatedOn,
                    CURRENCY = mproduct.CURRENCY,
                    ID = Guid.NewGuid(),
                    IsDeleted = mproduct.IsDeleted,
                    MPID = mproduct.MPID,
                    MusteriID = mproduct.MusteriID,
                    NAME = mproduct.NAME,
                    NAME_DE = mproduct.NAME_DE,
                    NAME_EN = mproduct.NAME_EN,
                    OEM = mproduct.OEM,
                    LogType = "D",
                    ProductID = mproduct.ProductID,
                    PRICE = mproduct.PRICE,
                    XPSNO = mproduct.XPSNO,
                    XPSUP = mproduct.XPSUP
                });

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

    }
}