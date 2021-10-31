using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Nero2021.BLL.Utilities;
using Nero2021.Data;
//using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
using System.Web;
using System.Web.Http;
using Nero2021;
using System.Text.RegularExpressions;
//using static TurkBelge.Controllers.PersonelController;

namespace Nero.Web.api
{
    //[Authorize]
    public class UserLevelsController : ApiController
    {
        //[HttpPost]
        //public IHttpActionResult BrowseList()
        //{
        //    try
        //    {
        //        var products = new ProductRepo().GetAll(q => (q.Deleted ?? false) == false).Select(s => new
        //        {
        //            s.PRODID,
        //            s.BUKOD,
        //            s.NAME
        //        }).AsQueryable();

        //        return Ok(new
        //        {
        //            Products = products
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Ürünler yüklenemedi.");
        //    }
        //}

        [HttpPost]
        public IHttpActionResult DTList()
        {
            try
            {
                return Ok(new UserLevelsRepo().ListDT());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult GetPermissions(UserLevelGetPermissionsDTO model)
        {
            try
            {
                return Ok(new UserLevelsRepo().GetPermissions(model));
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult SetPermissions(UserLevelSetPermissionsDTO model)
        {
            try
            {
                Result result = new UserLevelsRepo().SetPermissions(model);

                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest("Kayıt işlemi yapılırken bir hata oluştu.");
            }
        }



        [HttpPost]
        public IHttpActionResult Detail(UserLevelDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var userleveldetail = new UserLevelsRepo().FindByID(model.ID);

                return Ok(new
                {
                    UserLevelDetail = userleveldetail,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(UserLevelDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Yetki silindi.";

                var userlevel = new UserLevelsRepo().GetByID(model.ID);
                if (userlevel == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                userlevel.IsDeleted = true;
                userlevel.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                userlevel.DeletedOn = DateTime.Now;
                new UserLevelsRepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(UserLevelSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.UserLevelID == null)
                {
                    #region new item       

                    successMessage = "Yetki eklendi.";

                    var item = new UserLevels()
                    {
                        UserLevelName = model.UserLevelName,
                        IsActive = model.IsActive,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new UserLevelsRepo().Insert(item);

                    model.UserLevelID = item.UserLevelID;
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Yetki güncellendi.";

                    var item = new UserLevelsRepo().GetByID(model.UserLevelID ?? 0);
                    if (item == null)
                        return BadRequest("Yetki kaydına ulaşılamadı.");

                    item.UserLevelName = model.UserLevelName;
                    item.IsActive = model.IsActive;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new UserLevelsRepo().Update();
                    #endregion
                }

                return Ok(new { ID = model.UserLevelID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }

      
        [HttpPost]
        public IHttpActionResult FillAllCmb()
        {
            try
            {
                var productsections = new ProductSectionRepo().GetAll(q => q.ENABLED == true).Select(s => new { s.SECID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();
                var productmaincategories = new ProductMainCategoryRepo().GetAll(q => q.ENABLED == true).Select(s => new { s.SECID, s.MCATID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();
                var productcategories = new ProductCategoryRepo().GetAll(q => q.ENABLED == true).Select(s => new { s.CATID, s.SECID, s.MCATID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();
                var oemsupliers = new OEMSupplierRepo().GetAll().Select(s => new { s.SUPID, s.SUPNAME }).OrderBy(o => o.SUPNAME).AsQueryable();
                var musteriler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false && ((q.FIRMA_TIPI == (int)FirmaTipleri.Müşteri) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi))).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var tedarikciler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false && ((q.FIRMA_TIPI == (int)FirmaTipleri.Tedarikçi) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi))).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).OrderBy(o => o.ID).AsQueryable();

                return Ok(new
                {
                    ProductSections = productsections,
                    ProductMainCategories = productmaincategories,
                    ProductCategories = productcategories,
                    OEMSupliers = oemsupliers,
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
        public IHttpActionResult TUList(ProductTUListDTO model)
        {
            try
            {
                var tulist = new TProductsRepo().GetAll(q => q.ProductID == model.PRODID && (q.IsDeleted ?? false) == false)
                       .Select(s => new
                       {
                           s.TPID,
                           s.MusteriID,
                           s.MUSTERILER?.FIRMA_ADI,
                           s.ProductID,
                           s.BUKOD,
                           s.XPSUP,
                           s.XPSNO,
                           s.NAME,
                           s.EDITOR_TABLE,
                           s.NAME_EN,
                           s.NAME_DE,
                           s.PRICE,
                           s.CURRENCY,
                           CurrencyCode = s.Currency1?.Code,
                           //s.BUESKI,
                           s.OEM,
                           ADDED = s.ADDED != null ? s.ADDED.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                           UPDATED = s.UPDATED != null ? s.UPDATED.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                           s.CATNAME,
                           s.BRAND
                       });

                return Ok(new { TUList = tulist });
            }
            catch (Exception)
            {
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }

        }

        [HttpPost]
        public IHttpActionResult SaveTU(ProductTUSaveDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = String.Empty;

                if (model.TPID == 0)
                {
                    #region new item       
                    successMessage = "Ürün Tedarikçi kaydı eklendi.";

                    var item = new TPRODUCTS()
                    {
                        MusteriID = model.MusteriID,
                        ProductID = model.ProductID,
                        BUKOD = model.BUKOD,
                        XPSUP = model.MusteriID.ToString(), //model.XPSUP,
                        NAME = model.NAME,
                        EDITOR_TABLE = model.EDITOR_TABLE,
                        NAME_EN = model.NAME_EN,
                        NAME_DE = model.NAME_DE,
                        PRICE = model.PRICE,
                        CURRENCY = model.CURRENCY,
                        OEM = model.OEM,
                        XPSNO = model.XPSNO,
                        ADDED = DateTime.Now,
                        CATNAME = model.CATNAME,
                        BRAND = model.BRAND,
                        CreatedOn = DateTime.Now,
                        CreatedBy = HttpContext.Current.User.Identity.GetUserId()
                    };
                    new TProductsRepo().Insert(item);

                    model.TPID = item.TPID;
                    #endregion
                }
                else
                {
                    successMessage = "Ürün Tedarikçi kaydı güncellendi.";

                    var tproduct = new TProductsRepo().GetByID(model.TPID);

                    if (tproduct == null)
                        return BadRequest("Kayıt yapılırken ürün-müşteri kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                    tproduct.MusteriID = model.MusteriID;
                    //tproduct.ProductID = model.ProductID;
                    //tproduct.BUKOD = model.BUKOD;
                    tproduct.XPSUP = model.MusteriID.ToString(); //model.XPSUP;
                    tproduct.NAME = model.NAME;
                    tproduct.EDITOR_TABLE = model.EDITOR_TABLE;
                    tproduct.NAME_EN = model.NAME_EN;
                    tproduct.NAME_DE = model.NAME_DE;
                    tproduct.PRICE = model.PRICE;
                    tproduct.CURRENCY = model.CURRENCY;
                    tproduct.XPSNO = model.XPSNO;
                    tproduct.OEM = model.OEM;
                    tproduct.CATNAME = model.CATNAME;
                    tproduct.UPDATED = DateTime.Now;
                    tproduct.BRAND = model.BRAND;

                    new TProductsRepo().Update();
                }

                return Ok(new { ID = model.TPID, Message = successMessage, Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteTU(ProductTUDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Üründen tedarikçi silindi.";

                var tproduct = new TProductsRepo().GetByID(model.TPID);
                if (tproduct == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                tproduct.IsDeleted = true;
                tproduct.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                tproduct.DeletedOn = DateTime.Now;
                new TProductsRepo().Update();

                return Ok(new { ID = model.TPID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }



    }
}