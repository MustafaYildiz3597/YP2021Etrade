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
//using static TurkBelge.Controllers.PersonelController;

namespace Nero.Web.api
{
    //[Authorize]
    public class B2BController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DealerTypeList()
        {
            try
            {
                return Ok(new B2BDealerTypeRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new
                {
                    s.ID,
                    s.Name,
                    s.SalesRate
                }).AsQueryable());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult SaveDealerTypeList(System.Collections.Generic.List<SaveDealerTypeListDTO> model)
        {
            try
            {
                string userID = HttpContext.Current.User.Identity.GetUserId();

                foreach (var item in model)
                {
                    var dealertype = new B2BDealerTypeRepo().GetByID(item.ID ?? 0);

                    if (dealertype == null)
                    {
                        if (item.IsDeleted != true)
                        {
                            new B2BDealerTypeRepo().Insert(
                                new B2BDealerType()
                                {
                                    CreatedBy = userID,
                                    CreatedOn = DateTime.Now,
                                    Name = item.Name,
                                    SalesRate = item.SalesRate
                                });
                        }
                    }
                    else
                    {
                        dealertype.IsDeleted = item.IsDeleted;
                        dealertype.Name = item.Name;
                        dealertype.SalesRate = item.SalesRate;
                        dealertype.IsDeleted = item.IsDeleted;
                        dealertype.UpdatedBy = userID;
                        dealertype.UpdatedOn = DateTime.Now;

                        new B2BDealerTypeRepo().Update();
                    }
                }

                var list = new B2BDealerTypeRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new
                {
                    s.ID,
                    s.Name,
                    s.SalesRate
                }).AsQueryable();

                return Ok(new { Message = "Kayıt işlemi tamamlandı.", List = list });
            }
            catch (Exception ex)
            {
                return BadRequest("Kayıt işlemi yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult GetStocks()
        {
            try
            {
                return Ok(new ProductRepo().GetAll(q => q.B2BIsVisible == true && (q.Deleted ?? false) == false).Select(s => new
                {
                    s.PRODID,
                    s.NAME,
                    s.BUKOD,
                    s.B2BBasePrice,
                    s.B2BDiscountedPrice,
                    s.B2BCurrencyID,
                    CurrencyCode = s.Currency?.Code,
                    s.B2BIsNewProduct,
                    s.B2BIsOnSale,
                    s.B2BIsVisibleOnCategoryHomepage,
                    s.B2BIsVisibleOnHomepage,
                    s.B2BStockAmount
                }));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public IHttpActionResult StockDetail(B2BStockDetailDTO model)
        {
            try
            {
                var product = new ProductRepo().FindByID(model.ID ?? 0);

                return Ok(new
                {
                    ProductDetail = product
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult StoklarFillAllCmb()
        {
            try
            {
                var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).OrderBy(o => o.ID).AsQueryable();

                return Ok(new
                {
                    Currencies = currencies
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult StoklarSave(B2BStoklarSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;
  
            try
            {
                if (model.PRODID == null)
                {
                    /*ürün mutlaka olmalı! çünkü mevcut ürün zaten b2b'ye set edilerek ekleme işlemi yapılıyor... */
                    return BadRequest("Gönderilen Ürün kaydı hatalı!");
                }
                else
                {
                    #region update item

                    successMessage = "Ürün güncellendi.";

                    var item = new ProductRepo().GetByID(model.PRODID ?? 0);
                    if (item == null)
                        return BadRequest("Ürün kaydına ulaşılamadı.");

                    //item.BUKOD = model.BUKOD;
                   
                    item.B2BBasePrice = model.B2BBasePrice;
                    item.B2BCurrencyID = model.B2BCurrencyID;
                    item.B2BDiscountedPrice = model.B2BDiscountedPrice;
                    item.B2BIsNewProduct = model.B2BIsNewProduct;
                    item.B2BIsOnSale = model.B2BIsOnSale;
                    //item.B2BIsVisible = model.B2BIsVisible;
                    item.B2BIsVisibleOnCategoryHomepage = model.B2BIsVisibleOnCategoryHomepage;
                    item.B2BIsVisibleOnHomepage = model.B2BIsVisibleOnHomepage;
                    item.B2BStockAmount = model.B2BStockAmount;

                    item.UPDATED = DateTime.Now;
                    item.UPDATEDBY = userID;

                    new ProductRepo().Update();
                    #endregion
                }

                var data = new ProductRepo().GetAll(q => q.PRODID == model.PRODID).Select(s => new
                {
                    s.PRODID,
                    s.B2BBasePrice,
                    s.B2BCurrencyID,
                    CurrencyCode = s.Currency?.Code,
                    s.B2BDiscountedPrice,
                    s.B2BIsNewProduct,
                    s.B2BIsOnSale,
                    s.B2BIsVisibleOnCategoryHomepage,
                    s.B2BIsVisibleOnHomepage,
                    s.B2BStockAmount
                }).ToList();

                return Ok(new { ID = model.PRODID, Data = data, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult StoklarDelete(B2BStoklarDeleteDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.ID == null)
                {
                    /*ürün mutlaka olmalı! çünkü mevcut ürün zaten b2b'ye set edilerek ekleme işlemi yapılıyor... */
                    return BadRequest("Gönderilen Ürün kaydı hatalı!");
                }
                else
                {
                    #region update item

                    successMessage = "Ürün B2B den çıkartıldı.";

                    var item = new ProductRepo().GetByID(model.ID ?? 0);
                    if (item == null)
                        return BadRequest("Ürün kaydına ulaşılamadı.");

                    //item.BUKOD = model.BUKOD;

                    //item.B2BBasePrice = model.B2BBasePrice;
                    //item.B2BCurrencyID = model.B2BCurrencyID;
                    //item.B2BDiscountedPrice = model.B2BDiscountedPrice;
                    //item.B2BIsNewProduct = model.B2BIsNewProduct;
                    //item.B2BIsOnSale = model.B2BIsOnSale;
                    ////item.B2BIsVisible = model.B2BIsVisible;
                    //item.B2BIsVisibleOnCategoryHomepage = model.B2BIsVisibleOnCategoryHomepage;
                    //item.B2BIsVisibleOnHomepage = model.B2BIsVisibleOnHomepage;
                    //item.B2BStockAmount = model.B2BStockAmount;

                    item.B2BIsVisible = false;
                    item.UPDATED = DateTime.Now;
                    item.UPDATEDBY = userID;

                    new ProductRepo().Update();
                    #endregion
                }

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }

    }
}