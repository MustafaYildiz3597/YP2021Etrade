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
    public class OrderRepo : RepositoryBase<Orders, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable SorguPageDTList()
        {
            try
            {
                return (from od in db.ODetay
                        join o in db.Orders.Where(q => (q.IsDeleted ?? false) == false) on (od.OrderID ?? 0) equals o.OrderID
                        join m in db.MUSTERILER on o.MusteriID equals m.ID
                        join p in db.PRODUCTS on od.ProductID equals p.PRODID into pj
                        from p in pj.DefaultIfEmpty()
                        join cr in db.Currency on od.CurrencyID equals cr.ID into crj
                        from cr in crj.DefaultIfEmpty()
                        select new
                        {
                            o.OrderNo,
                            m.FIRMA_ADI,
                            ProductName = p == null ? "" : p.NAME,
                            od.CustomerCode,
                            p.BUKOD,
                            od.BUESKI,
                            od.Quantity,
                            od.UnitPrice,
                            CurrencyCode = cr == null ? "" : cr.Code,
                            o.OrderDate
                        }).AsQueryable();

            }
            catch (Exception)
            {

                throw;
            }

        }

        public IQueryable SiparislerPageDTList(SiparislerPageDTListDTO model)
        {
            try
            {
                //string criOrderNo = model.OrderNo ?? "";
                //string cricust

                DateTime criBeginDate = model.CriBeginDate == null ? DateTime.MinValue : (model.CriBeginDate ?? DateTime.MinValue);
                DateTime criEndDate = model.CriEndDate == null ? DateTime.MaxValue : (model.CriEndDate ?? DateTime.MaxValue);

                return (from o in db.Orders.Where(q => (q.IsDeleted ?? false) == false && (
                                (q.OrderNo.Contains(model.OrderNo) || (model.OrderNo ?? "") == "")
                                && q.OrderDate >= criBeginDate
                                && q.OrderDate <= criEndDate
                                )
                            )
                        join m in db.MUSTERILER on o.MusteriID equals m.ID into mj
                        from m in mj.DefaultIfEmpty()
                        join yk in db.YETKILI_KISILER on o.EmployeeID equals yk.ID into ykj
                        from yk in ykj.DefaultIfEmpty()
                            //where (m.FIRMA_ADI.Contains(model.CustomerName) || (model.CustomerName ?? "") == "")
                        where (m.ID == (model.CustomerID ?? 0) || model.CustomerID == null)
                        select new
                        {
                            o.OrderID,
                            o.OrderNo,
                            o.MusteriID,
                            FIRMA_ADI = m == null ? "" : m.FIRMA_ADI,
                            o.EmployeeID,
                            MTemsilcisi = yk == null ? "" : yk.ADI + " " + yk.SOYADI,
                            o.OrderDate,
                            o.AddDate,
                            OrderItemCount = o.ODetay.Count()
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
                return (from o in db.Orders.Where(q => q.OrderID == id)
                        join m in db.MUSTERILER on o.MusteriID equals m.ID into mj
                        from m in mj.DefaultIfEmpty()
                        join yk in db.YETKILI_KISILER on m.ID equals yk.FIRMA_ID into ykj
                        from yk in ykj.DefaultIfEmpty()
                        join mc in db.Member on o.CreatedBy equals mc.ID into mcj
                        from mc in mcj.DefaultIfEmpty()
                        join mu in db.Member on o.UpdatedBy equals mu.ID into muj
                        from mu in muj.DefaultIfEmpty()
                        select new
                        {
                            o.OrderID,
                            OrderNo = o.OrderNo.Trim(),
                            o.MusteriID,
                            FIRMA_ADI = m == null ? "" : m.FIRMA_ADI,
                            o.EmployeeID,
                            MTemsilcisi = yk == null ? "" : yk.ADI + " " + yk.SOYADI,
                            o.OrderDate,
                            o.AddDate,
                            o.CreatedOn,
                            o.UpdatedOn,
                            Hazirlayan = mc == null ? "" : mc.FirstName + " " + mc.LastName,
                            Guncelleyen = mu == null ? "" : mu.FirstName + " " + mu.LastName,
                            OrderItems = o.ODetay.Where(q => (q.IsDeleted ?? false) == false)
                            .Select(s => new
                            {
                                s.ORDID,
                                s.OrderID,
                                s.CustomerCode,
                                s.ProductID,
                                s.BuCode,
                                s.NAME_EN,
                                s.NAME_DE,
                                s.Quantity,
                                s.UnitPrice,
                                s.CurrencyID,
                                CurrencyCode = s.Currency == null ? "" : s.Currency.Code
                            })
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                //TODO: add to log
                throw;
            }

        }

        public Result ImportExcelData(OrderImportExcelUploadDataDTO model)
        {
            string userID = HttpContext.Current.User.Identity.GetUserId();

            Result retval = new Result()
            {
                IsSuccess = false,
                StrList = new List<string>()
            };

            try
            {
                string sqlconnstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                //#if DEBUG
                //            sqlconnstr = @"Server=DESKTOP-QPGN981\SS17;Database=TurkBelgeKEPDB;User Id=;Password=;";
                //#else
                //            sqlconnstr = "Server=test.kep-ik.com;Database=ilekacom_kepiktestdb;User Id=;Password=;";
                //#endif

                using (SqlConnection openCon = new SqlConnection(sqlconnstr))
                {
                    retval.TotalCount = model.Data.Count();
                    int succededcount = 0;

                    foreach (var item in model.Data)
                    {
                        var product = new ProductRepo().GetByFilter(q => q.BUKOD == item.BUKOD);

                        if (product == null)
                        {
                            string saveStaff = "INSERT into PRODUCTS ([BUKOD], [NAME], [NAME_EN], [NAME_DE], [NAME_FR], SECID, MCATID, CATID, ADDED, ADDEDBY) " +
                                "VALUES (@BUKOD, @NAME, @NAME_EN, @NAME_DE, @NAME_FR, @SECID, @MCATID, @CATID, @ADDED, @ADDEDBY)";

                            using (SqlCommand queryProduct = new SqlCommand(saveStaff))
                            {
                                queryProduct.Connection = openCon;

                                //queryProduct.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 50).Value = (object)item.BUKOD ?? DBNull.Value;
                                //queryProduct.Parameters.Add("@NAME", SqlDbType.NVarChar, 256).Value = (object)item.NAME ?? DBNull.Value;
                                //queryProduct.Parameters.Add("@NAME_EN", SqlDbType.NVarChar, 256).Value = (object)item.NAME_EN ?? DBNull.Value;
                                //queryProduct.Parameters.Add("@NAME_DE", SqlDbType.NVarChar, 256).Value = (object)item.NAME_DE ?? DBNull.Value;
                                //queryProduct.Parameters.Add("@NAME_FR", SqlDbType.NVarChar, 256).Value = (object)item.NAME_FR ?? DBNull.Value;
                                //queryProduct.Parameters.Add("@SECID", SqlDbType.Int).Value = model.Cats.SECID;
                                //queryProduct.Parameters.Add("@MCATID", SqlDbType.Int).Value = model.Cats.MCATID;
                                //queryProduct.Parameters.Add("@CATID", SqlDbType.Int).Value = model.Cats.CATID;
                                //queryProduct.Parameters.Add("@ADDED", SqlDbType.DateTime).Value = DateTime.Now;
                                //queryProduct.Parameters.Add("@ADDEDBY", SqlDbType.NVarChar, 128).Value = userID;

                                openCon.Open();
                                queryProduct.ExecuteNonQuery();
                            }

                            succededcount++;
                        }
                        else
                        {
                            /* ürün zaten var.*/

                            // ekli ürün listesine ekle 
                            retval.StrList.Add(item.BUKOD);
                        }

                    }

                    retval.SuccededCount = succededcount;
                }

                retval.IsSuccess = true;
                retval.Message = "Yükleme işlemi başarı ile tamamlandı.";

                return retval;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public Result LoadExcelDataToTemp(OrderImportExcelUploadDataDTO model, Guid batchID)
        {
            string userID = HttpContext.Current.User.Identity.GetUserId();

            Result retval = new Result()
            {
                IsSuccess = false,
                StrList = new List<string>(),
            };

            try
            {
                string sqlconnstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                //#if DEBUG
                //            sqlconnstr = @"Server=DESKTOP-QPGN981\SS17;Database=TurkBelgeKEPDB;User Id=;Password=;";
                //#else
                //            sqlconnstr = "Server=test.kep-ik.com;Database=ilekacom_kepiktestdb;User Id=;Password=;";
                //#endif

                //var carilist = model.Data.Select(s => new { s.CustomerTitle }).Distinct();

                //foreach (var item in carilist)
                //{
                //    var musteri = new MusterilerRepo().GetAll(q => q.FIRMA_ADI == item.CustomerTitle).FirstOrDefault();
                //    if (musteri == null)
                //    {
                //        retval.Message = item.CustomerTitle + " isimli cari kayıtlarda bulunamadı!";
                //    }
                //}


                // var siparislist = model.Data.Select(s=>new { s.CustomerTitle, s.OrderNo } ).(from t in model.Data)


                using (SqlConnection openCon = new SqlConnection(sqlconnstr))
                {
                    retval.TotalCount = model.Data.Count();
                    int succededcount = 0;

                    openCon.Open();

                    foreach (var item in model.Data)
                    {

                        if (
                            String.IsNullOrWhiteSpace(item.OrderNo) ||
                             String.IsNullOrWhiteSpace(item.BUKOD) ||
                              String.IsNullOrWhiteSpace(item.ProductName) ||
                               String.IsNullOrWhiteSpace(item.PCustomerCode) ||
                                String.IsNullOrWhiteSpace(item.Quantity.ToString()) ||
                                 String.IsNullOrWhiteSpace(item.SalesPrice.ToString())
                            )
                        {
                            continue;
                        }


                        string query = "INSERT into OrderTransferTemp ([ID], [BatchID], [RowNum], [OrderDate], [OrderNo], [CustomerTitle], [BUKOD], [ProductName], [PCustomerCode], [Quantity], [SalesPrice]) " +
                            "VALUES (@ID, @BatchID, @RowNum, @OrderDate, @OrderNo, @CustomerTitle, @BUKOD, @ProductName, @PCustomerCode, @Quantity, @SalesPrice)";

                        using (SqlCommand sqlcommand = new SqlCommand(query))
                        {
                            sqlcommand.Connection = openCon;

                            sqlcommand.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                            sqlcommand.Parameters.Add("@BatchID", SqlDbType.UniqueIdentifier).Value = batchID;
                            sqlcommand.Parameters.Add("@RowNum", SqlDbType.Int).Value = succededcount + 1;
                            sqlcommand.Parameters.Add("@OrderDate", SqlDbType.NVarChar, 32).Value = (object)item.OrderDate ?? DBNull.Value;
                            sqlcommand.Parameters.Add("@OrderNo", SqlDbType.NVarChar, 32).Value = (object)item.OrderNo ?? DBNull.Value;
                            sqlcommand.Parameters.Add("@CustomerTitle", SqlDbType.NVarChar, 128).Value = (object)model.MusteriID ?? DBNull.Value;
                            sqlcommand.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 32).Value = (object)item.BUKOD ?? DBNull.Value;
                            sqlcommand.Parameters.Add("@ProductName", SqlDbType.NVarChar, 256).Value = (object)item.ProductName ?? DBNull.Value;
                            sqlcommand.Parameters.Add("@PCustomerCode", SqlDbType.NVarChar, 50).Value = (object)item.PCustomerCode ?? DBNull.Value;
                            sqlcommand.Parameters.Add("@Quantity", SqlDbType.Int).Value = (object)item.Quantity ?? DBNull.Value;
                            sqlcommand.Parameters.Add("@SalesPrice", SqlDbType.Decimal).Value = (object)item.SalesPrice ?? DBNull.Value;

                            sqlcommand.ExecuteNonQuery();
                        }

                        succededcount++;
                        //}
                        //else
                        //{
                        //    /* ürün zaten var.*/

                        //    // ekli ürün listesine ekle 
                        //    retval.StrList.Add(item.BUKOD);
                        //}

                    }

                    retval.SuccededCount = succededcount;
                }

                retval.IsSuccess = true;
                retval.Message = "Yükleme işlemi başarı ile tamamlandı.";
            }
            catch (Exception ex)
            {
                retval.Message = "Veri yüklenirken bir hata oluştu.";
            }

            return retval;
        }


        public void RemoveAllExcelDataByBatchID(Guid batchID)
        {
            try
            {
                string sqlconnstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

                using (SqlConnection openCon = new SqlConnection(sqlconnstr))
                {
                    openCon.Open();

                    string query = "DELETE FROM OrderTransferTemp WHERE BatchID = @BatchID";

                    using (SqlCommand sqlcommand = new SqlCommand(query))
                    {
                        sqlcommand.Connection = openCon;

                        sqlcommand.Parameters.Add("@BatchID", SqlDbType.UniqueIdentifier).Value = batchID;

                        sqlcommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
