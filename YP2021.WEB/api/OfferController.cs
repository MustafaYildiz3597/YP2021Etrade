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
    public class OfferController : ApiController
    {
        [HttpPost]
        public IHttpActionResult SorguPageDTList()
        {
            try
            {
                return Ok(new OfferRepo().SorguPageDTList());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult DTList(OfferDTListDTO model)
        {
            try
            {
                return Ok(new OfferRepo().ListDT(model));
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult Detail(OfferDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var offerdetail = new OfferRepo().FindByID(model.ID);

                return Ok(new
                {
                    OfferDetail = offerdetail,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(OfferDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string userID = HttpContext.Current.User.Identity.GetUserId();

                var item = new OfferRepo().GetByID(model.TID);
                if (item == null)
                    return BadRequest("Teklif kaydına ulaşılamadı.");

                item.IsDeleted = true;
                item.DeletedBy = userID;
                item.DeletedOn = DateTime.Now;

                new OfferRepo().Update();

                return Ok("Teklif silindi");

            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Save(OfferSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.TDURUMU == null)
                return BadRequest("Teklif durumu belirtilmemiş. Lütfen kontrol ediniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;
            string bucode = string.Empty;
            int? productID = null;
            var product = new PRODUCTS() { };

            try
            {
                if (model.TID == null)
                {
                    #region new item       

                    successMessage = "Teklif eklendi.";

                    string day = DateTime.Now.ToString("yyyyMMdd");
                    string lastTeklifNoOfDay = new OfferRepo().GetAll(q => q.TEKLIFNO.StartsWith(day)).OrderByDescending(o => o.TEKLIFNO).FirstOrDefault()?.TEKLIFNO;

                    if (lastTeklifNoOfDay == "99")
                        return BadRequest("Teklif No - Günlük 99 aşımı. Lütfen yönetinize haber veriniz.");
                    char pad = '0';
                    string newTeklifNo = day + (lastTeklifNoOfDay == null || lastTeklifNoOfDay == "" ? "01" : Convert.ToString(Convert.ToInt32(lastTeklifNoOfDay) + 1)).PadLeft(2, pad);

                    var newteklif = new TEKLIFLER()
                    {
                        TEKLIFNO = newTeklifNo,
                        TTIPI = model.TTIPI,
                        MusteriID = model.MusteriID,
                        CustomerID = model.MusteriID.ToString(),
                        FIRMPID = model.YetkiliKisiID.ToString(),
                        YetkiliKisiID = model.YetkiliKisiID,
                        TITLE = model.TITLE,
                        ICERIK = model.ICERIK,
                        ADD_DATE = DateTime.Now,
                        TDURUMU = model.TDURUMU ?? 0,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new OfferRepo().Insert(newteklif);

                    model.TID = newteklif.TID;

                    foreach (var teklifitem in model.TeklifItems)
                    {
                        // bucode = teklifitem.BuCode.Trim();
                        bucode = teklifitem.BuCode == null ? null : teklifitem.BuCode.Trim();

                        productID = null;

                        product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                        if (product != null)
                            productID = product.PRODID;

                        var newteklifitem = new TEKLIFLER_DETAY()
                        {
                            BuCode = teklifitem.BuCode,
                            CURRENCY = teklifitem.CURRENCY,
                            CustomerCode = teklifitem.CustomerCode,
                            Detay = teklifitem.Detay,
                            Name = teklifitem.Name,
                            Oem = teklifitem.Oem,
                            Oem1 = teklifitem.Oem1,
                            PRODID = productID, //teklifitem.ProductID,
                            ProductID = teklifitem.ProductID.ToString(),
                            Quantity = teklifitem.Quantity,
                            TID = model.TID,
                            UnitPrice = teklifitem.UnitPrice,
                            CreatedBy = userID,
                            CreatedOn = DateTime.Now
                        };
                        new OfferItemRepo().Insert(newteklifitem);
                    }
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Teklif güncellendi.";

                    var item = new OfferRepo().GetByID(model.TID ?? 0);
                    if (item == null)
                        return BadRequest("Teklif kaydına ulaşılamadı.");

                    item.TTIPI = model.TTIPI;
                    item.MusteriID = model.MusteriID;
                    item.CustomerID = model.MusteriID.ToString();
                    item.FIRMPID = model.YetkiliKisiID.ToString();
                    item.YetkiliKisiID = model.YetkiliKisiID;
                    item.TITLE = model.TITLE;
                    item.ICERIK = model.ICERIK;
                    item.TDURUMU = model.TDURUMU ?? 0;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;
                    new OfferRepo().Update();

                    foreach (var teklifitem in model.TeklifItems)
                    {
                        if (teklifitem.TDID < 0)
                        {
                            //bucode = teklifitem.BuCode.Trim();
                            bucode = teklifitem.BuCode == null ? null : teklifitem.BuCode.Trim();

                            productID = null;

                            product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                            if (product != null)
                                productID = product.PRODID;

                            var newteklifitem = new TEKLIFLER_DETAY()
                            {
                                BuCode = teklifitem.BuCode,
                                CURRENCY = teklifitem.CURRENCY,
                                CustomerCode = teklifitem.CustomerCode,
                                Detay = teklifitem.Detay,
                                Name = teklifitem.Name,
                                Oem = teklifitem.Oem,
                                Oem1 = teklifitem.Oem1,
                                PRODID = productID, //teklifitem.ProductID,
                                ProductID = teklifitem.ProductID.ToString(),
                                Quantity = teklifitem.Quantity,
                                TID = model.TID,
                                UnitPrice = teklifitem.UnitPrice,
                                CreatedBy = userID,
                                CreatedOn = DateTime.Now
                            };
                            new OfferItemRepo().Insert(newteklifitem);
                        }
                        else
                        {
                            var offeritem = new OfferItemRepo().GetByID(teklifitem.TDID);

                            if (offeritem != null)
                            {
                                if (offeritem.BuCode.Trim() != teklifitem.BuCode.Trim())
                                {
                                    bucode = teklifitem.BuCode.Trim();
                                    productID = null;

                                    product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                                    if (product != null)
                                        productID = product.PRODID;

                                    offeritem.BuCode = teklifitem.BuCode;
                                    offeritem.PRODID = productID;
                                    offeritem.ProductID = productID.ToString();
                                }

                                //offeritem.BuCode = teklifitem.BuCode;
                                offeritem.CURRENCY = teklifitem.CURRENCY;
                                offeritem.CustomerCode = teklifitem.CustomerCode;
                                offeritem.Detay = teklifitem.Detay;
                                offeritem.Name = teklifitem.Name;
                                offeritem.Oem = teklifitem.Oem;
                                offeritem.Oem1 = teklifitem.Oem1;
                                // offeritem.PRODID = teklifitem.ProductID;
                                offeritem.ProductID = teklifitem.ProductID.ToString();
                                offeritem.Quantity = teklifitem.Quantity;
                                offeritem.TID = model.TID;
                                offeritem.UnitPrice = teklifitem.UnitPrice;
                                offeritem.UpdatedBy = userID;
                                offeritem.UpdatedOn = DateTime.Now;

                                new OfferItemRepo().Update();
                            }
                            else
                            {
                                bucode = offeritem.BuCode.Trim();
                                productID = null;

                                product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                                if (product != null)
                                    productID = product.PRODID;

                                var newteklifitem = new TEKLIFLER_DETAY()
                                {
                                    BuCode = teklifitem.BuCode,
                                    CURRENCY = teklifitem.CURRENCY,
                                    CustomerCode = teklifitem.CustomerCode,
                                    Detay = teklifitem.Detay,
                                    Name = teklifitem.Name,
                                    Oem = teklifitem.Oem,
                                    Oem1 = teklifitem.Oem1,
                                    PRODID = productID, //teklifitem.ProductID,
                                    ProductID = teklifitem.ProductID.ToString(),
                                    Quantity = teklifitem.Quantity,
                                    TID = model.TID,
                                    UnitPrice = teklifitem.UnitPrice,
                                    CreatedBy = userID,
                                    CreatedOn = DateTime.Now
                                };
                                new OfferItemRepo().Insert(newteklifitem);
                            }
                        }
                    }

                    if (model.DeletedItems != null)
                    {
                        foreach (var deleteditem in model.DeletedItems)
                        {
                            var deletedofferitem = new OfferItemRepo().GetByID(deleteditem.TDID ?? 0);
                            if (deletedofferitem != null)
                            {
                                deletedofferitem.DeletedBy = userID;
                                deletedofferitem.DeletedOn = DateTime.Now;
                                deletedofferitem.IsDeleted = true;
                                new OfferItemRepo().Update();
                            }
                        }
                    }
                    #endregion
                }

                return Ok(new { ID = model.TID, Message = successMessage });
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
                var firmalar = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false //&& ((q.FIRMA_TIPI == (int)FirmaTipleri.Müşteri) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi)) 
                ).Select(s => new { s.ID, s.FIRMA_ADI, s.FIRMA_TIPI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).OrderBy(o => o.ID).AsQueryable();
                var yetkilikisiler = new YetkiliKisilerRepo().GetAll().Select(s => new { s.ID, s.ADI, s.SOYADI, s.FIRMA_ID }).AsQueryable();

                return Ok(new
                {
                    Firmalar = firmalar,
                    Currencies = currencies,
                    YetkiliKisiler = yetkilikisiler
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }



    }
}