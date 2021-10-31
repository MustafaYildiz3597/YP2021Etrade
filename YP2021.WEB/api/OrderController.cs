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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
//using static TurkBelge.Controllers.PersonelController;

namespace Nero.Web.api
{
    //[Authorize]
    public class OrderController : ApiController
    {
        [HttpPost]
        public IHttpActionResult SorguPageDTList()
        {
            try
            {
                return Ok(new OrderRepo().SorguPageDTList());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult SiparislerPageDTList(SiparislerPageDTListDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası!");

            try
            {
                return Ok(new OrderRepo().SiparislerPageDTList(model));
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult Detail(OrderDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var orderdetail = new OrderRepo().FindByID(model.ID);

                return Ok(new
                {
                    OrderDetail = orderdetail,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(OrderDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string userID = HttpContext.Current.User.Identity.GetUserId();

                var item = new OrderRepo().GetByID(model.ID);
                if (item == null)
                    return BadRequest("Sipariş kaydına ulaşılamadı.");

                item.IsDeleted = true;
                item.DeletedBy = userID;
                item.DeletedOn = DateTime.Now;

                new OrderRepo().Update();

                return Ok("Sipariş silindi");
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Save(OrderSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            //if (model.TDURUMU == null)
            //    return BadRequest("Teklif durumu belirtilmemiş. Lütfen kontrol ediniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;
            string bucode = string.Empty;
            int? productID = null;
            var product = new PRODUCTS() { };

            try
            {
                if (model.OrderID == null)
                {
                    #region new item       

                    successMessage = "Sipariş eklendi.";

                    var neworder = new Orders()
                    {
                        AddDate = DateTime.Now,
                        EmployeeID = model.EmployeeID,
                        OrderDate = model.OrderDateF,
                        DeletedBy = null,
                        DeletedOn = null,
                        OrderNo = model.OrderNo,
                        Status = null,
                        UpdatedBy = null,
                        UpdatedOn = null,
                        //Updates = null,
                        //UpdatesUser = null,
                        MusteriID = model.MusteriID,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new OrderRepo().Insert(neworder);

                    model.OrderID = neworder.OrderID;

                    foreach (var orderitem in model.OrderItems)
                    {
                        bucode = orderitem.BuCode == null ? null : orderitem.BuCode.Trim();

                        productID = null;

                        product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                        if (product != null)
                            productID = product.PRODID;

                        var neworderitem = new ODetay()
                        {
                            BuCode = orderitem.BuCode,
                            CurrencyID = orderitem.CurrencyID,
                            CustomerCode = orderitem.CustomerCode,
                            NAME_EN = orderitem.NAME_EN,
                            NAME_DE = orderitem.NAME_DE,
                            ProductID = productID, // orderitem.ProductID,
                            Quantity = orderitem.Quantity,
                            OrderID = model.OrderID,
                            UnitPrice = orderitem.UnitPrice,
                            CreatedBy = userID,
                            CreatedOn = DateTime.Now
                        };
                        new OrderItemRepo().Insert(neworderitem);
                    }
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Sipariş güncellendi.";

                    var item = new OrderRepo().GetByID(model.OrderID ?? 0);
                    if (item == null)
                        return BadRequest("Sipariş kaydına ulaşılamadı.");

                    item.EmployeeID = model.EmployeeID;
                    item.OrderDate = model.OrderDateF;
                    item.OrderNo = model.OrderNo;
                    item.Status = null;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;
                    item.Updates = DateTime.Now;
                    //item.UpdatesUser = userID;
                    item.MusteriID = model.MusteriID;
                    new OrderRepo().Update();

                    foreach (var orderitem in model.OrderItems)
                    {
                        if (orderitem.ORDID < 0)
                        {
                            bucode = orderitem.BuCode == null ? null : orderitem.BuCode.Trim();

                            productID = null;

                            product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                            if (product != null)
                                productID = product.PRODID;

                            var neworderitem = new ODetay()
                            {
                                BuCode = orderitem.BuCode,
                                CurrencyID = orderitem.CurrencyID,
                                CustomerCode = orderitem.CustomerCode,
                                NAME_EN = orderitem.NAME_EN,
                                NAME_DE = orderitem.NAME_DE,
                                ProductID = productID, //orderitem.ProductID,
                                Quantity = orderitem.Quantity,
                                OrderID = model.OrderID,
                                UnitPrice = orderitem.UnitPrice,
                                CreatedBy = userID,
                                CreatedOn = DateTime.Now
                            };
                            new OrderItemRepo().Insert(neworderitem);
                        }
                        else
                        {
                            var orderItem = new OrderItemRepo().GetByID(orderitem.ORDID);

                            if (orderItem != null)
                            {
                                if (orderItem.BuCode.Trim() != orderitem.BuCode.Trim())
                                {
                                    bucode = orderitem.BuCode.Trim();
                                    productID = null;

                                    product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                                    if (product != null)
                                        productID = product.PRODID;

                                    orderItem.BuCode = orderitem.BuCode;
                                    orderItem.ProductID = productID;
                                }

                                orderItem.CurrencyID = orderitem.CurrencyID;
                                orderItem.CustomerCode = orderitem.CustomerCode;
                                orderItem.NAME_EN = orderitem.NAME_EN;
                                orderItem.NAME_DE = orderitem.NAME_DE;
                                orderItem.Quantity = orderitem.Quantity;
                                orderItem.OrderID = model.OrderID;
                                orderItem.UnitPrice = orderitem.UnitPrice;
                                orderItem.UpdatedBy = userID;
                                orderItem.UpdatedOn = DateTime.Now;

                                new OrderItemRepo().Update();
                            }
                            else
                            {
                                bucode = orderitem.BuCode.Trim();
                                productID = null;

                                product = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == bucode);
                                if (product != null)
                                    productID = product.PRODID;

                                var neworderitem = new ODetay()
                                {
                                    BuCode = orderitem.BuCode,
                                    CurrencyID = orderitem.CurrencyID,
                                    CustomerCode = orderitem.CustomerCode,
                                    NAME_EN = orderitem.NAME_EN,
                                    NAME_DE = orderitem.NAME_DE,
                                    ProductID = orderitem.ProductID,
                                    Quantity = orderitem.Quantity,
                                    OrderID = model.OrderID,
                                    UnitPrice = orderitem.UnitPrice,
                                    CreatedBy = userID,
                                    CreatedOn = DateTime.Now
                                };
                                new OrderItemRepo().Insert(neworderitem);
                            }
                        }
                    }

                    if (model.DeletedItems != null)
                    {
                        foreach (var deleteditem in model.DeletedItems)
                        {
                            var deletedorderitem = new OrderItemRepo().GetByID(deleteditem.ORDID ?? 0);
                            if (deletedorderitem != null)
                            {
                                deletedorderitem.DeletedBy = userID;
                                deletedorderitem.DeletedOn = DateTime.Now;
                                deletedorderitem.IsDeleted = true;
                                new OrderItemRepo().Update();
                            }
                        }
                    }

                    #endregion
                }

                return Ok(new { ID = model.OrderID, Message = successMessage });
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
                //var firmalar = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false //&& ((q.FIRMA_TIPI == (int)FirmaTipleri.Müşteri) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi)) 
                //).Select(s => new { s.ID, s.FIRMA_ADI, s.FIRMA_TIPI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var musteriler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false && ((q.FIRMA_TIPI == (int)FirmaTipleri.Müşteri) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi))).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();
                var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).OrderBy(o => o.ID).AsQueryable();
                var yetkilikisiler = new YetkiliKisilerRepo().GetAll().Select(s => new { s.ID, s.ADI, s.SOYADI, s.FIRMA_ID }).AsQueryable();

                return Ok(new
                {
                    Musteriler = musteriler,
                    Currencies = currencies,
                    YetkiliKisiler = yetkilikisiler
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult ExcelImportFillAllCmb()
        {
            try
            {
                var musteriler = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new { s.ID, s.FIRMA_ADI }).OrderBy(o => o.FIRMA_ADI).AsQueryable();


                return Ok(new
                {
                    Musteriler = musteriler,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult ImportExcelUploadData(OrderImportExcelUploadDataDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.Data == null)
                return BadRequest("Data bulunamadı.");

            if (model.Data.Count() == 0)
                return BadRequest("Data bulunamadı.");

            if (model.MusteriID == null)
                return BadRequest("Müşteri Kodu parametre hatası! Müşteri seçip tekrar deneyiniz.");

            try
            {
                var musteri = new MusterilerRepo().GetByID(model.MusteriID ?? 0);

                if (musteri == null)
                    return BadRequest("Müşteri bulunamadı! Kontrol ettikten sonra tekrar deneyiniz.");

                Result retval = new Result()
                {
                    IsSuccess = false,
                    StrList = new List<string>()
                };

                string retmessage = String.Empty;

                Guid batchID = Guid.NewGuid();
                string ordernums = "";
                string userID = HttpContext.Current.User.Identity.GetUserId();

                var orders = model.Data.Where(q => q.OrderNo != "" && q.OrderNo != null).Select(s => new { s.OrderNo, s.OrderDate }).Distinct();

                //var tmp1 = orders.GroupBy(c => c.OrderNo).Where(grp => grp.Count() > 1).Select(grp => grp.Key).ToList();

                //if (tmp1.Count() > 0)
                //    return BadRequest("Sipariş no ve sipariş tarihi verileri tutarsız!. Aktarılacak veriyi excel dosyasından kontrol ediniz.");

                retval.TotalCount = orders.Count();

                int succededcount = 0;

                string sqlconnstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (SqlConnection openCon = new SqlConnection(sqlconnstr))
                {
                    openCon.Open();

                    #region check siparis_no rows
                    foreach (var item in orders)
                    {
                        if (item.OrderDate == null)
                        {
                            retval.StrList.Add(item.OrderNo + " numaralı siparişin sipariş tarihi eksik olduğundan aktarılamadı.<br />");
                            break;
                        }

                        var order = new OrderRepo().GetAll(q => q.OrderNo == item.OrderNo && q.OrderDate == item.OrderDate && q.MusteriID == musteri.ID && (q.IsDeleted ?? false) == false);

                        if (order.Count() > 0)
                        {
                            retval.StrList.Add(item.OrderNo + " numaralı sipariş zaten kayıtlı olduğundan aktarılamadı.<br />");
                        }
                        else
                        {
                            var inserted = model.Data.Where(q => q.OrderNo == item.OrderNo && q.OrderDate == item.OrderDate);

                            bool isvalid = true;

                            foreach (var checkitem in inserted)
                            {
                                if (
                                     (String.IsNullOrWhiteSpace(checkitem.BUKOD) && String.IsNullOrWhiteSpace(checkitem.PCustomerCode)) ||
                                      String.IsNullOrWhiteSpace(checkitem.ProductName) ||
                                        String.IsNullOrWhiteSpace(checkitem.Quantity.ToString()) ||
                                         String.IsNullOrWhiteSpace(checkitem.SalesPrice.ToString())
                                    )
                                {
                                    isvalid = false;
                                    retval.StrList.Add(checkitem.OrderNo + " nolu siparişe ait geçersiz veri(ler) bulunduğundan aktarılamadı.");
                                    break;
                                }
                            }

                            if (isvalid == true)
                            {
                                string query = String.Empty;

                                query = "INSERT INTO Orders(AddDate, CreatedBy, CreatedOn, IsDeleted, MusteriID, OrderDate, OrderNo) " +
                                    "VALUES(@AddDate, @CreatedBy, @CreatedOn, 0, @MusteriID, @OrderDate, @OrderNo)";

                                int? newOrderID = null;

                                using (SqlCommand sqlcommand = new SqlCommand(query))
                                {
                                    sqlcommand.Connection = openCon;

                                    sqlcommand.Parameters.Add("@AddDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    sqlcommand.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 128).Value = userID;
                                    sqlcommand.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                                    sqlcommand.Parameters.Add("@MusteriID", SqlDbType.Int).Value = musteri.ID;
                                    sqlcommand.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = (object)item.OrderDate ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@OrderNo", SqlDbType.NVarChar, 32).Value = (object)item.OrderNo ?? DBNull.Value;

                                    newOrderID = (int)sqlcommand.ExecuteScalar();
                                }

                                #region old
                                //var neworderitem = new Orders()
                                //{
                                //    EmployeeID = null,
                                //    AddDate = DateTime.Now,
                                //    CreatedBy = userID,
                                //    CreatedOn = DateTime.Now,
                                //    IsDeleted = false,
                                //    MusteriID = musteri.ID,
                                //    OrderDate = item.OrderDate,
                                //    OrderNo = item.OrderNo
                                //};

                                //var neworder = new OrderRepo().Insert(neworderitem);
                                #endregion

                                foreach (var inserteditem in inserted)
                                {
                                    int? productID = null;

                                    if (!String.IsNullOrWhiteSpace(inserteditem.BUKOD))
                                    {
                                        var product = new ProductRepo().GetByFilter(q => q.BUKOD == inserteditem.BUKOD.Trim());

                                        if (product == null)
                                        {
                                            //var newproduct = new PRODUCTS()
                                            //{
                                            //    BUKOD = inserteditem.BUKOD,
                                            //    ADDED = DateTime.Now,
                                            //    ADDEDBY = userID,
                                            //    Deleted = false,
                                            //    NAME = inserteditem.ProductName
                                            //};

                                            //new ProductRepo().Insert(newproduct);

                                            //productID = newproduct.PRODID;

                                            productID = null;

                                        }
                                        else
                                        {
                                            /*silinmiş ise aktif hale alınır*/
                                            if (product.Deleted == true)
                                            {
                                                //product.Deleted = false;
                                                //product.DeletedBy = null;
                                                //product.DeletedOn = null;

                                                //new ProductRepo().Update();
                                            }
                                            else
                                            {
                                                productID = product.PRODID;
                                            }
                                        }

                                        #region 
                                        //new OrderItemRepo().Insert(new ODetay()
                                        //{
                                        //    BuCode = inserteditem.BUKOD,
                                        //    ProductID = productID,
                                        //    CreatedBy = userID,
                                        //    CreatedOn = DateTime.Now,
                                        //    CustomerCode = inserteditem.PCustomerCode,
                                        //    NAME_EN = inserteditem.ProductName,
                                        //    IsDeleted = false,
                                        //    OrderNO = inserteditem.OrderNo,
                                        //    OrderID = neworderitem.OrderID,
                                        //    CurrencyID = 2,
                                        //    Quantity = inserteditem.Quantity,
                                        //    UnitPrice = inserteditem.SalesPrice
                                        //});
                                        #endregion

                                        query = "INSERT INTO ODetay(BuCode,ProductID,CreatedBy,CreatedOn,CustomerCode,NAME_EN,IsDeleted,OrderNO,OrderID,CurrencyID,Quantity,UnitPrice) " +
                                            "VALUES(@BuCode,@ProductID,@CreatedBy,@CreatedOn,@CustomerCode,@NAME_EN,0,@OrderNO,@OrderID,@CurrencyID,@Quantity,@UnitPrice)";

                                        using (SqlCommand sqlcommand = new SqlCommand(query))
                                        {
                                            sqlcommand.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 50).Value = (object)inserteditem.BUKOD ?? DBNull.Value;
                                            sqlcommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = (object)productID ?? DBNull.Value;
                                            sqlcommand.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 128).Value = userID;
                                            sqlcommand.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                                            sqlcommand.Parameters.Add("@CustomerCode", SqlDbType.NVarChar, 50).Value = (object)inserteditem.PCustomerCode ?? DBNull.Value;
                                            sqlcommand.Parameters.Add("@NAME_EN", SqlDbType.NVarChar, 256).Value = (object)inserteditem.ProductName ?? DBNull.Value;
                                            sqlcommand.Parameters.Add("@OrderNo", SqlDbType.NVarChar, 32).Value = (object)item.OrderNo ?? DBNull.Value;
                                            sqlcommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = (object)newOrderID ?? DBNull.Value;
                                            sqlcommand.Parameters.Add("@CurrencyID", SqlDbType.Int).Value = 2;
                                            sqlcommand.Parameters.Add("@Quantity", SqlDbType.Int).Value = (object)inserteditem.Quantity ?? DBNull.Value;
                                            sqlcommand.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = (object)inserteditem.SalesPrice ?? DBNull.Value;

                                            sqlcommand.ExecuteNonQuery();
                                        }

                                    }

                                }

                                succededcount++;
                            }
                        }
                    }
                    #endregion

                    if (openCon.State == System.Data.ConnectionState.Open) openCon.Close();
                }


                retval.SuccededCount = succededcount;

                return Ok(retval);
            }
            catch (Exception)
            {
                return BadRequest("Aktarım işlemi yapılırken bir hata oluştu.");

            }

        }

    }
}