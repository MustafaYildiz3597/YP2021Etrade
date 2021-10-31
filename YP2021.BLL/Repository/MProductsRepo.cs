using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nero2021.Data;
using System.ComponentModel;
using Nero2021.BLL.Models;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Nero2021.BLL.Repository
{
    public class MProductsRepo : RepositoryBase<MPRODUCTS, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public List<MProductDTListWithPage_Result> ListDT(MPListDTDTO model)
        {
            try
            {
                return db.MProductDTListWithPage(model.MusteriID, model.BUKOD, model.pageNumber, model.pageSize, model.sortColumn, model.sortColumnDir, model.searchValue, model.searchitems.BUKod, model.searchitems.FIRMA_ADI, model.searchitems.XPSNO, model.searchitems.NAME, model.searchitems.CreatedOn, model.searchitems.UPDATED, model.searchitems.PRICE, model.searchitems.CurrencyCode).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<MProductDTListWithPageCount_Result> ListDTCount(MPListDTDTO model)
        {
            try
            {
                //db.Database.CommandTimeout = 60;
                return db.MProductDTListWithPageCount(model.MusteriID, model.BUKOD, model.searchValue, model.searchitems.BUKod, model.searchitems.FIRMA_ADI, model.searchitems.XPSNO, model.searchitems.NAME, model.searchitems.CreatedOn, model.searchitems.UPDATED, model.searchitems.PRICE, model.searchitems.CurrencyCode).ToList();
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
                return (from mp in db.MPRODUCTS.Where(q => q.MPID == id)
                        join cr in db.Member on mp.CreatedBy equals cr.ID into crj
                        from cr in crj.DefaultIfEmpty()
                        join ur in db.Member on mp.UpdatedBy equals ur.ID into urj
                        from ur in urj.DefaultIfEmpty()
                        join dr in db.Member on mp.DeletedBy equals dr.ID into drj
                        from dr in drj.DefaultIfEmpty()
                        select new
                        {
                            mp.MPID,
                            mp.BUKOD,
                            CreatedBy = cr == null ? "" : cr.FirstName + " " + cr.LastName,
                            mp.CreatedOn,
                            mp.CURRENCY,
                            CurrencyCode = mp == null ? "" : mp.Currency1.Code ?? "",
                            DeletedMember = dr == null ? "" : dr.FirstName + " " + dr.LastName,
                            mp.DeletedOn,
                            mp.EDITOR_TABLE,
                            mp.MusteriID,
                            MusteriName = mp == null ? "" : mp.MUSTERILER.FIRMA_ADI,
                            mp.NAME,
                            mp.NAME_DE,
                            mp.NAME_EN,
                            mp.OEM,
                            mp.PRICE,
                            mp.ProductID,
                            ProductName = mp == null ? "" : mp.PRODUCTS.NAME,
                            mp.UPDATED,
                            UpdatedBy = ur == null ? "" : ur.FirstName + " " + ur.LastName,
                            mp.XPSNO,
                            mp.XPSUP
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Result ImportExcelData(MPImportExcelUploadDataDTO model)
        {
            int totalrowcount = model.Data.Count();

            Result retval = new Result()
            {
                IsSuccess = false,
                StrObj = new List<ResultRow>(),
                TotalCount = totalrowcount,
                UnProccessedCount = 0,
                SuccededCount = 0,
                AddedCount = 0,
                UpdatedCount = 0
            };

            int? exceluploadID = null;

            try
            {
                var nonexistsproducts = (from t in model.Data.Where(q => q.BUKOD != null && q.BUKOD != "")
                                         join p in db.PRODUCTS.Where(q => (q.Deleted ?? false) == false).ToList() on t.BUKOD equals p.BUKOD into p1
                                         from p in p1.DefaultIfEmpty()
                                         where p == null
                                         select t
                                         ).ToList();


                if (nonexistsproducts.Count() > 0)
                {
                    retval.Message = "Hatalı/Geçersiz kayıtlar görüldü. Lütfen veriyi kontrol ediniz.";

                    foreach (var item in nonexistsproducts)
                    {
                        retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = item.BUKOD + " Bu numaralı ürün kaydı bulunamadı." });
                    }

                    return retval;
                }

                var nonexistscurrencies = (from t in model.Data
                                           join c in db.Currency.ToList() on t.CURRENCY equals c.Code into c1
                                           from c in c1.DefaultIfEmpty()
                                           where c == null
                                           select t
                                        ).ToList();

                if (nonexistscurrencies.Count() > 0)
                {
                    retval.Message = "Para birimi bulunmayan ya da Para birimi geçersiz kayıtlar mevcut. Lütfen veriyi kontrol ediniz.";

                    return retval;
                }

                var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).ToList();
                string userID = HttpContext.Current.User.Identity.GetUserId();

                var exceluploadinsert = new ExcelUploadLog()
                {
                    CreateDT = DateTime.Now,
                    CreateUserID = userID,
                    TotalRowCount = totalrowcount,
                    ExcelUploadEntity = "Mproducts"
                };

                new ExcelUploadLogRepo().Insert(exceluploadinsert);

                exceluploadID = exceluploadinsert.ID;
                string query = String.Empty;
                int? newID = null;

                string sqlconnstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (SqlConnection openCon = new SqlConnection(sqlconnstr))
                {
                    openCon.Open();

                    foreach (var item in model.Data)
                    {
                        int? productID = null;

                        if (!string.IsNullOrWhiteSpace(item.BUKOD))
                        {
                            var product = new ProductRepo().GetByFilter(q => q.BUKOD == item.BUKOD && (q.Deleted ?? false) == false);
                            if (product == null)
                            {
                                /* BUKOD ürün tablosunda tanımmlı değilse MPRODUCTS tablosuna eklemiyoruz. */
                                retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = "Ürün kaydı bulunamadı." });
                                continue;
                            }
                            else
                            {
                                productID = product.PRODID;
                            }
                        }

                        var currency = currencies.Where(q => q.Code == item.CURRENCY).Select(s => new { s.ID }).FirstOrDefault();

                        var mproduct = new MProductsRepo().GetByFilter(q => q.BUKOD == item.BUKOD && q.MusteriID == model.MusteriID && q.XPSNO == item.XPSNO);

                        if (mproduct == null)
                        {
                            query = "INSERT INTO MPRODUCTS(ADDED,BUKOD,CreatedBy,CreatedOn,CURRENCY,MusteriID,NAME,NAME_DE,NAME_EN,OEM,PRICE,ProductID,XPSNO,XPSUP,IsDeleted) " +
                               "VALUES (@ADDED,@BUKOD,@CreatedBy,@CreatedOn,@CURRENCY,@MusteriID,@NAME,@NAME_DE,@NAME_EN,@OEM,@PRICE,@ProductID,@XPSNO,@XPSUP,0); SELECT SCOPE_IDENTITY();";

                            using (SqlCommand sqlcommand = new SqlCommand(query))
                            {
                                sqlcommand.Connection = openCon;

                                sqlcommand.Parameters.Add("@ADDED", SqlDbType.DateTime).Value = DateTime.Now;
                                sqlcommand.Parameters.Add("@BUKOD", SqlDbType.VarChar, 12).Value = (object)item.BUKOD ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 128).Value = (object)userID ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                                sqlcommand.Parameters.Add("@CURRENCY", SqlDbType.Int).Value = (object)currency.ID ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@MusteriID", SqlDbType.Int).Value = (object)model.MusteriID ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@NAME", SqlDbType.NVarChar, 256).Value = (object)item.NAME ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@NAME_DE", SqlDbType.NVarChar, 256).Value = (object)item.NAME_DE ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@NAME_EN", SqlDbType.NVarChar, 256).Value = (object)item.NAME_EN ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@OEM", SqlDbType.NVarChar, 32).Value = (object)item.OEM ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@PRICE", SqlDbType.Money).Value = (object)item.PRICE ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = (object)productID ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@XPSNO", SqlDbType.NVarChar, 250).Value = (object)item.XPSNO ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@XPSUP", SqlDbType.VarChar, 12).Value = (object)model.MusteriID.ToString() ?? DBNull.Value;

                                sqlcommand.CommandType = CommandType.Text;
                                newID = Convert.ToInt32(sqlcommand.ExecuteScalar());
                            }

                            query = "INSERT INTO MProductsLog(ID,MPID,LogType,ADDED,BUKOD,CreatedBy,CreatedOn,CURRENCY,MusteriID,NAME,NAME_DE,NAME_EN,OEM,PRICE,ProductID,XPSNO,XPSUP,IsDeleted) " +
                              "VALUES (@ID,@MPID,@LogType,@ADDED,@BUKOD,@CreatedBy,@CreatedOn,@CURRENCY,@MusteriID,@NAME,@NAME_DE,@NAME_EN,@OEM,@PRICE,@ProductID,@XPSNO,@XPSUP,0)";

                            using (SqlCommand sqlcommand = new SqlCommand(query))
                            {
                                sqlcommand.Connection = openCon;

                                sqlcommand.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                sqlcommand.Parameters.Add("@MPID", SqlDbType.Int).Value = newID;
                                sqlcommand.Parameters.Add("@LogType", SqlDbType.VarChar, 1).Value = "I";
                                sqlcommand.Parameters.Add("@ADDED", SqlDbType.DateTime).Value = DateTime.Now;
                                sqlcommand.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 50).Value = (object)item.BUKOD ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 128).Value = userID;
                                sqlcommand.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                                sqlcommand.Parameters.Add("@CURRENCY", SqlDbType.Int).Value = currency.ID;
                                sqlcommand.Parameters.Add("@MusteriID", SqlDbType.Int).Value = model.MusteriID;
                                sqlcommand.Parameters.Add("@NAME", SqlDbType.NVarChar, 256).Value = (object)item.NAME ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@NAME_DE", SqlDbType.NVarChar, 256).Value = (object)item.NAME_DE ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@NAME_EN", SqlDbType.NVarChar, 256).Value = (object)item.NAME_EN ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@OEM", SqlDbType.NVarChar, 32).Value = (object)item.OEM ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@PRICE", SqlDbType.Money).Value = (object)item.PRICE ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = (object)productID ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@XPSNO", SqlDbType.NVarChar, 250).Value = (object)item.XPSNO ?? DBNull.Value;
                                sqlcommand.Parameters.Add("@XPSUP", SqlDbType.VarChar, 12).Value = (object)model.MusteriID.ToString() ?? DBNull.Value;

                                sqlcommand.ExecuteNonQuery();
                            }

                            retval.AddedCount++;
                            retval.SuccededCount++;
                        }
                        else
                        {
                            if (mproduct.CURRENCY != currency.ID ||
                               mproduct.NAME != item.NAME ||
                               mproduct.NAME_DE != item.NAME_DE ||
                               mproduct.NAME_EN != item.NAME_EN ||
                               mproduct.OEM != item.OEM ||
                               mproduct.PRICE != item.PRICE ||
                               mproduct.XPSNO != item.XPSNO ||
                               mproduct.XPSUP != model.MusteriID.ToString()
                               ) // kayıt değişmiş mi kontrolü
                            {
                                #region update query
                                query = "UPDATE t SET t.CURRENCY=@CURRENCY,t.NAME=@NAME,t.NAME_DE=@NAME_DE,t.NAME_EN=@NAME_EN,t.OEM=@OEM,t.PRICE=@PRICE,t.XPSUP=@XPSUP,t.UPDATED=@UPDATED,t.UpdatedBy=@UpdatedBy " +
                                    " FROM MPRODUCTS t " +
                                    " WHERE t.MusteriID = @MusteriID ";

                                if (String.IsNullOrWhiteSpace(item.BUKOD))
                                    query += "AND coalesce(t.BUKOD, '') = '' ";
                                else
                                    query += " AND t.BUKOD = @BUKOD ";

                                if (String.IsNullOrWhiteSpace(item.XPSNO))
                                    query += " AND coalesce(t.XPSNO, '') = '' ";
                                else
                                    query += " AND t.XPSNO = @XPSNO ";
                                #endregion

                                using (SqlCommand sqlcommand = new SqlCommand(query))
                                {
                                    sqlcommand.Connection = openCon;

                                    sqlcommand.Parameters.Add("@BUKOD", SqlDbType.VarChar, 12).Value = (object)item.BUKOD ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@MusteriID", SqlDbType.Int).Value = (object)model.MusteriID ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@XPSNO", SqlDbType.NVarChar, 250).Value = (object)item.XPSNO ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@CURRENCY", SqlDbType.Int).Value = (object)currency.ID ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@NAME", SqlDbType.NVarChar, 256).Value = (object)item.NAME ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@NAME_DE", SqlDbType.NVarChar, 256).Value = (object)item.NAME_DE ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@NAME_EN", SqlDbType.NVarChar, 256).Value = (object)item.NAME_EN ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@OEM", SqlDbType.NVarChar, 32).Value = (object)item.OEM ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@PRICE", SqlDbType.Money).Value = (object)item.PRICE ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@XPSUP", SqlDbType.VarChar, 12).Value = (object)model.MusteriID.ToString() ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 128).Value = userID;
                                    sqlcommand.Parameters.Add("@UPDATED", SqlDbType.DateTime).Value = DateTime.Now;
                                    sqlcommand.ExecuteNonQuery();
                                }

                                query = "INSERT INTO MProductsLog(ID,MPID,LogType,ADDED,BUKOD,UpdatedBy,UPDATED,CURRENCY,MusteriID,NAME,NAME_DE,NAME_EN,OEM,PRICE,ProductID,XPSNO,XPSUP,IsDeleted) " +
                             "VALUES (@ID,@MPID,@LogType,@ADDED,@BUKOD,@UpdatedBy,@UPDATED,@CURRENCY,@MusteriID,@NAME,@NAME_DE,@NAME_EN,@OEM,@PRICE,@ProductID,@XPSNO,@XPSUP,0)";

                                using (SqlCommand sqlcommand = new SqlCommand(query))
                                {
                                    sqlcommand.Connection = openCon;

                                    sqlcommand.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                    sqlcommand.Parameters.Add("@MPID", SqlDbType.Int).Value = mproduct.MPID;
                                    sqlcommand.Parameters.Add("@LogType", SqlDbType.VarChar, 1).Value = "U";
                                    sqlcommand.Parameters.Add("@ADDED", SqlDbType.DateTime).Value = DateTime.Now;
                                    sqlcommand.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 50).Value = (object)item.BUKOD ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 128).Value = userID;
                                    sqlcommand.Parameters.Add("@UPDATED", SqlDbType.DateTime).Value = DateTime.Now;
                                    sqlcommand.Parameters.Add("@CURRENCY", SqlDbType.Int).Value = currency.ID;
                                    sqlcommand.Parameters.Add("@MusteriID", SqlDbType.Int).Value = model.MusteriID;
                                    sqlcommand.Parameters.Add("@NAME", SqlDbType.NVarChar, 256).Value = (object)item.NAME ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@NAME_DE", SqlDbType.NVarChar, 256).Value = (object)item.NAME_DE ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@NAME_EN", SqlDbType.NVarChar, 256).Value = (object)item.NAME_EN ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@OEM", SqlDbType.NVarChar, 32).Value = (object)item.OEM ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@PRICE", SqlDbType.Money).Value = (object)item.PRICE ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = (object)productID ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@XPSNO", SqlDbType.NVarChar, 250).Value = (object)item.XPSNO ?? DBNull.Value;
                                    sqlcommand.Parameters.Add("@XPSUP", SqlDbType.VarChar, 12).Value = (object)model.MusteriID.ToString() ?? DBNull.Value;

                                    sqlcommand.ExecuteNonQuery();
                                }

                                retval.UpdatedCount++;
                                retval.SuccededCount++;
                            }
                            else
                            {
                                retval.UnProccessedCount++;
                            }
                        }
                    }

                    if (openCon.State == System.Data.ConnectionState.Open) openCon.Close();
                }

                var exceluploadlog = new ExcelUploadLogRepo().GetByID(exceluploadID ?? 0);
                if (exceluploadlog != null)
                {
                    exceluploadlog.CompletionDT = DateTime.Now;
                    exceluploadlog.TransferedRowCount = retval.SuccededCount;

                    var errtext = string.Empty;
                    foreach (var item in retval.StrObj)
                    {
                        errtext += item.Text + " " + item.Description + " ";
                    }

                    exceluploadlog.ErrorText = errtext;

                    new ExcelUploadLogRepo().Update();
                }
                
                retval.IsSuccess = true;
                return retval;
            }
            catch (Exception ex)
            {
                var exceluploadlog = new ExcelUploadLogRepo().GetByID(exceluploadID ?? 0);
                if (exceluploadlog != null)
                {
                    exceluploadlog.CompletionDT = DateTime.Now;
                    exceluploadlog.TransferedRowCount = retval.SuccededCount;
                    exceluploadlog.HasError = true;
                    exceluploadlog.ErrorText = ex.GetBaseException().Message;
                    new ExcelUploadLogRepo().Update();
                }

                throw;
            }
              
            ////Result retval = new Result()
            ////{
            ////    IsSuccess = false,
            ////    StrObj = new List<ResultRow>()
            ////};

            ////int totalrowcount = model.Data.Count();
            //int succededrowcount = 0;

            //try
            //{
            //    foreach (var item in model.Data)
            //    {
            //        int? productID = null;

            //        if (!string.IsNullOrWhiteSpace(item.BUKOD))
            //        {
            //            var product = new ProductRepo().GetByFilter(q => q.BUKOD == item.BUKOD && (q.Deleted ?? false) == false);
            //            if (product == null)
            //            {
            //                /* BUKOD ürün tablosunda tanımmlı değilse MPRODUCTS tablosuna eklemiyoruz. */
            //                retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = "Ürün kaydı bulunamadı." });
            //                continue;
            //            }
            //            else
            //            {
            //                productID = product.PRODID;
            //            }
            //        }

            //        var currency = new CurrencyRepo().GetByFilter(q => q.Code == item.CURRENCY);
            //        if (currency == null)
            //        {
            //            retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = "Geçersiz Para birimi." });
            //            continue;
            //        }

            //        var mproduct = new MProductsRepo().GetByFilter(q => (!string.IsNullOrWhiteSpace(item.BUKOD) && q.BUKOD == item.BUKOD && q.MusteriID == model.MusteriID) ||
            //                (string.IsNullOrWhiteSpace(item.BUKOD) && q.XPSNO == item.XPSNO && q.MusteriID == model.MusteriID)
            //        );

            //        if (mproduct == null)
            //        {
            //            var newMProduct = new MPRODUCTS()
            //            {
            //                ADDED = DateTime.Now,
            //                BUKOD = item.BUKOD,
            //                CreatedBy = userID,
            //                CreatedOn = DateTime.Now,
            //                CURRENCY = currency.ID,
            //                MusteriID = model.MusteriID ?? 0,
            //                NAME = item.NAME,
            //                NAME_DE = item.NAME_DE,
            //                NAME_EN = item.NAME_EN,
            //                OEM = item.OEM,
            //                PRICE = item.PRICE,
            //                ProductID = productID,
            //                XPSNO = item.XPSNO,
            //                XPSUP = model.MusteriID.ToString(),
            //                IsDeleted = false
            //            };

            //            new MProductsRepo().Insert(newMProduct);

            //            new MProductsLogRepo().Insert(new MProductsLog()
            //            {
            //                ADDED = newMProduct.ADDED,
            //                BUKOD = newMProduct.BUKOD,
            //                CreatedBy = newMProduct.CreatedBy,
            //                CreatedOn = newMProduct.CreatedOn,
            //                CURRENCY = newMProduct.CURRENCY,
            //                ID = Guid.NewGuid(),
            //                IsDeleted = newMProduct.IsDeleted,
            //                MPID = newMProduct.MPID,
            //                MusteriID = newMProduct.MusteriID,
            //                NAME = newMProduct.NAME,
            //                NAME_DE = newMProduct.NAME_DE,
            //                NAME_EN = newMProduct.NAME_EN,
            //                OEM = newMProduct.OEM,
            //                LogType = "I",
            //                ProductID = productID,
            //                PRICE = newMProduct.PRICE,
            //                XPSNO = newMProduct.XPSNO,
            //                XPSUP = newMProduct.XPSUP
            //            });

            //        }
            //        else
            //        {
            //            if (mproduct.CURRENCY != currency.ID ||
            //                mproduct.NAME != item.NAME ||
            //                mproduct.NAME_DE != item.NAME_DE ||
            //                mproduct.NAME_EN != item.NAME_EN ||
            //                mproduct.OEM != item.OEM ||
            //                mproduct.PRICE != item.PRICE ||
            //                mproduct.XPSNO != item.XPSNO ||
            //                mproduct.XPSUP != model.MusteriID.ToString()
            //                ) // kayıt değişmiş mi kontrolü
            //            {
            //                mproduct.CURRENCY = currency.ID;
            //                mproduct.NAME = item.NAME;
            //                mproduct.NAME_DE = item.NAME_DE;
            //                mproduct.NAME_EN = item.NAME_EN;
            //                mproduct.OEM = item.OEM;
            //                mproduct.PRICE = item.PRICE;
            //                mproduct.XPSNO = item.XPSNO;
            //                mproduct.XPSUP = model.MusteriID.ToString();
            //                mproduct.UPDATED = DateTime.Now;
            //                mproduct.UpdatedBy = userID;
            //                new MProductsRepo().Update();

            //                new MProductsLogRepo().Insert(new MProductsLog()
            //                {
            //                    BUKOD = mproduct.BUKOD,
            //                    CURRENCY = mproduct.CURRENCY,
            //                    ID = Guid.NewGuid(),
            //                    MPID = mproduct.MPID,
            //                    MusteriID = mproduct.MusteriID,
            //                    NAME = mproduct.NAME,
            //                    NAME_DE = mproduct.NAME_DE,
            //                    NAME_EN = mproduct.NAME_EN,
            //                    OEM = mproduct.OEM,
            //                    LogType = "U",
            //                    ProductID = productID,
            //                    PRICE = mproduct.PRICE,
            //                    XPSNO = mproduct.XPSNO,
            //                    XPSUP = mproduct.XPSUP,
            //                    UPDATED = mproduct.UPDATED,
            //                    UpdatedBy = mproduct.UpdatedBy

            //                    // UpdatesUser = userID
            //                });

            //            }
            //        }

            //        succededrowcount++;
            //    }

            //    retval.SuccededCount = succededrowcount;
            //    retval.IsSuccess = true;
            //    return retval;

            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

        }

        public Result SaveGridEdit(SaveMUGridEditDTO model)
        {
            string userID = HttpContext.Current.User.Identity.GetUserId();
            Result retval = new Result()
            {
                IsSuccess = false,
                StrObj = new List<ResultRow>()
            };

            int totalrowcount = model.Items.Count();
            int succededrowcount = 0;

            try
            {
                foreach (var item in model.Items)
                {
                    var mproduct = new MProductsRepo().GetByID(item.MPID);
                    if (mproduct == null)
                    {
                        retval.StrObj.Add(new ResultRow() { ID = item.MPID, Text = "", Description = "Müşteri ürün kaydı bulunamadı." });
                        continue;
                    }

                    if (String.IsNullOrWhiteSpace(mproduct.BUKOD) && String.IsNullOrWhiteSpace(item.XPSNO))
                    {
                        retval.StrObj.Add(new ResultRow() { ID = item.MPID, Text = item.XPSNO, Description = "Aynı satırda Müşteri-Ürün Numarası ve ya BU Numarası boş olamaz!" });
                        continue;
                    }

                    if (!String.IsNullOrWhiteSpace(item.XPSNO))
                    {
                        var exists = new MProductsRepo().GetByFilter(q => q.MusteriID == mproduct.MusteriID  && q.MPID != item.MPID && q.XPSNO == item.XPSNO);
                        if (exists != null)
                        {
                            retval.StrObj.Add(new ResultRow() { ID = item.MPID, Text = item.XPSNO, Description = item.XPSNO + " Müşteri-Ürün Numarası bu müşteri için zaten tanımlı!" });
                            continue;
                        }
                    }

                    mproduct = new MProductsRepo().GetByID(item.MPID);
                    mproduct.NAME = item.NAME;
                    mproduct.PRICE = item.PRICE;
                    mproduct.CURRENCY = item.CURRENCY;
                    mproduct.UpdatedBy = userID;
                    mproduct.UPDATED = DateTime.Now;

                    new MProductsRepo().Update();

                    #region insert log
                    new MProductsLogRepo().Insert(new MProductsLog()
                    {
                        BUKOD = mproduct.BUKOD,
                        CURRENCY = mproduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        MPID = mproduct.MPID,
                        MusteriID = mproduct.MusteriID,
                        NAME = mproduct.NAME,
                        NAME_DE = mproduct.NAME_DE,
                        NAME_EN = mproduct.NAME_EN,
                        OEM = mproduct.OEM,
                        LogType = "U",
                        ProductID = mproduct.ProductID,
                        PRICE = mproduct.PRICE,
                        XPSNO = mproduct.XPSNO,
                        XPSUP = mproduct.XPSUP,
                        UPDATED = mproduct.UPDATED,
                        UpdatedBy = mproduct.UpdatedBy
                        // UpdatesUser = userID
                    });

                    //retval.StrObj.Add(new ResultRow() { ID = item.MPID, Text = "OK" });

                    #endregion
                    succededrowcount++;
                }

                retval.SuccededCount = succededrowcount;
                retval.TotalCount = totalrowcount;
                retval.IsSuccess = true;
                return retval;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Result SaveGridAdd(SaveMUGridAddDTO model)
        {
            string userID = HttpContext.Current.User.Identity.GetUserId();
            Result retval = new Result()
            {
                IsSuccess = false,
                StrObj = new List<ResultRow>()
            };

            if (model.Musteriler.Count() == 0)
            {
                retval.Message = "Müşteri seçmelisiniz!";
                return retval;
            }
            if (model.Products.Count() == 0)
            {
                retval.Message = "Müşteri-Ürün girmelisiniz!";
                return retval;
            }

            foreach (var item in model.Products)
            {
                if (String.IsNullOrWhiteSpace(item.XPSNO) && String.IsNullOrWhiteSpace(item.BUKOD))
                {
                    retval.Message = "Aynı satırda Müşteri-Ürün Numarası ve BU Numarası boş olan kayıt(lar) var. Lütfen kontrol ediniz!";
                    return retval;
                }
            }

            int musteriID = model.Musteriler[0].ID ?? 0;

            try
            {
                retval.TotalCount = model.Products.Count();
                retval.SuccededCount = 0;

                foreach (var item in model.Products)
                {
                    //foreach (var musteriItem in model.Musteriler)
                    //{
                    item.XPSNO = String.IsNullOrWhiteSpace(item.XPSNO) ? null : item.XPSNO.Trim();
                    item.BUKOD = String.IsNullOrWhiteSpace(item.BUKOD) ? null : item.BUKOD.Trim();

                    int? productID = null;

                    if (!string.IsNullOrWhiteSpace(item.BUKOD))
                    {
                        var product = new ProductRepo().GetByFilter(q => q.BUKOD == item.BUKOD && (q.Deleted ?? false) == false);
                        if (product == null)
                        {
                            /* BUKOD ürün tablosunda tanımmlı değilse Products tablosuna eklemeden kayıt işlemine devam ediyoruz.  */
                            //retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = "Ürün kaydı bulunamadı." });
                            //continue;
                        }
                        else
                        {
                            productID = product.PRODID;
                        }
                    }

                    //var currency = new CurrencyRepo().GetByFilter(q => q.Code == item.CURRENCY);
                    //if (currency == null)
                    //{
                    //    retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = "Geçersiz Para birimi." });
                    //    continue;
                    //}

                    //var mproduct = new MProductsRepo().GetByFilter(q => (!string.IsNullOrWhiteSpace(item.BUKOD) && q.BUKOD == item.BUKOD && q.MusteriID == musteriItem.ID) ||
                    //        (string.IsNullOrWhiteSpace(item.BUKOD) && q.XPSNO == item.XPSNO && q.MusteriID == musteriItem.ID)
                    //);

                    //if (mproduct == null)
                    //{

                    if (!String.IsNullOrWhiteSpace(item.XPSNO))
                    {
                        var exists = new MProductsRepo().GetByFilter(q => q.MusteriID == musteriID && q.XPSNO == item.XPSNO);
                        if (exists != null)
                        {
                            retval.StrObj.Add(new ResultRow() { ID = item.MPID, Text = item.XPSNO, Description = item.XPSNO + " Müşteri-Ürün Numarası bu müşteri için zaten tanımlı!" });
                            continue;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(item.BUKOD))
                    {
                        var exists = new MProductsRepo().GetByFilter(q => q.MusteriID == musteriID && q.BUKOD == item.BUKOD);
                        if (exists != null)
                        {
                            retval.StrObj.Add(new ResultRow() { ID = item.MPID, Text = item.BUKOD, Description = item.BUKOD + " BU Numarası bu müşteri için zaten tanımlı!" });
                            continue;
                        }
                    }

                    var newMProduct = new MPRODUCTS()
                    {
                        ADDED = DateTime.Now,
                        BUKOD = item.BUKOD,
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        CURRENCY = item.CURRENCY,
                        MusteriID = musteriID,
                        NAME = item.NAME,
                        //NAME_DE = item.NAME_DE,
                        //NAME_EN = item.NAME_EN,
                        //OEM = item.OEM,
                        PRICE = item.PRICE,
                        ProductID = productID,
                        XPSNO = item.XPSNO,
                        XPSUP = musteriID.ToString(),
                        IsDeleted = false
                    };

                    new MProductsRepo().Insert(newMProduct);

                    new MProductsLogRepo().Insert(new MProductsLog()
                    {
                        ADDED = newMProduct.ADDED,
                        BUKOD = newMProduct.BUKOD,
                        CreatedBy = newMProduct.CreatedBy,
                        CreatedOn = newMProduct.CreatedOn,
                        CURRENCY = newMProduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        IsDeleted = newMProduct.IsDeleted,
                        MPID = newMProduct.MPID,
                        MusteriID = newMProduct.MusteriID,
                        NAME = newMProduct.NAME,
                        NAME_DE = newMProduct.NAME_DE,
                        NAME_EN = newMProduct.NAME_EN,
                        OEM = newMProduct.OEM,
                        LogType = "I",
                        ProductID = productID,
                        PRICE = newMProduct.PRICE,
                        XPSNO = newMProduct.XPSNO,
                        XPSUP = newMProduct.XPSUP
                    });

                    //}
                    //}
                    retval.SuccededCount++;
                }

                retval.IsSuccess = true;
                return retval;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public Result Save(SaveMProductDTO model)
        {
            string successMessage = String.Empty;

            string userID = HttpContext.Current.User.Identity.GetUserId();
            Result retval = new Result()
            {
                IsSuccess = false,
                StrObj = new List<ResultRow>()
            };

            if (model == null)
            {
                retval.Message = "Parametre hatası!";
                return retval;
            }

            try
            {
                var productitem = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == model.BUKOD.Trim());

                if (model.MPID == null)
                {
                    var mproduct = new MProductsRepo().GetByFilter(q => (!string.IsNullOrWhiteSpace(model.BUKOD) && q.BUKOD == model.BUKOD && q.MusteriID == model.MusteriID) ||
                                (string.IsNullOrWhiteSpace(model.BUKOD) && q.XPSNO == model.XPSNO && q.MusteriID == model.MusteriID)
                        );

                    if (mproduct != null)
                    {
                        retval.Message = "Müşteri ürünü daha önceden kayıtlı bulunduğundan işlem yapılamadı!";
                        return retval;
                    }

                    #region new item       

                    successMessage = "Müşteri Ürün eklendi.";

                    var newMProduct = new MPRODUCTS()
                    {
                        ADDED = DateTime.Now,
                        BUKOD = model.BUKOD,
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        CURRENCY = model.CURRENCY,
                        MusteriID = model.MusteriID,
                        NAME = model.NAME,
                        NAME_DE = model.NAME_DE,
                        NAME_EN = model.NAME_EN,
                        OEM = model.OEM,
                        EDITOR_TABLE = model.EDITOR_TABLE,
                        PRICE = model.PRICE,
                        ProductID = productitem?.PRODID, //model.ProductID,
                        XPSNO = model.XPSNO,
                        XPSUP = model.MusteriID.ToString(),
                        IsDeleted = false
                    };
                    new MProductsRepo().Insert(newMProduct);

                    model.MPID = newMProduct.MPID;

                    new MProductsLogRepo().Insert(new MProductsLog()
                    {
                        ADDED = newMProduct.ADDED,
                        BUKOD = newMProduct.BUKOD,
                        CreatedBy = newMProduct.CreatedBy,
                        CreatedOn = newMProduct.CreatedOn,
                        CURRENCY = newMProduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        IsDeleted = newMProduct.IsDeleted,
                        MPID = newMProduct.MPID,
                        MusteriID = newMProduct.MusteriID,
                        NAME = newMProduct.NAME,
                        NAME_DE = newMProduct.NAME_DE,
                        NAME_EN = newMProduct.NAME_EN,
                        OEM = newMProduct.OEM,
                        LogType = "I",
                        ProductID = newMProduct.ProductID,
                        PRICE = newMProduct.PRICE,
                        XPSNO = newMProduct.XPSNO,
                        XPSUP = newMProduct.XPSUP,
                        EDITOR_TABLE = newMProduct.EDITOR_TABLE
                    });

                    #endregion
                }
                else
                {
                    var othermproduct = new MProductsRepo().GetByFilter(q => ((!string.IsNullOrWhiteSpace(model.BUKOD) && q.BUKOD == model.BUKOD && q.MusteriID == model.MusteriID) ||
                                (string.IsNullOrWhiteSpace(model.BUKOD) && q.XPSNO == model.XPSNO && q.MusteriID == model.MusteriID))
                                && model.MPID != model.MPID
                                );

                    if (othermproduct != null)
                    {
                        retval.Message = "Müşteri ürünü daha önceden kayıtlı bulunduğundan işlem yapılamadı!";
                        return retval;
                    }


                    #region update item

                    successMessage = "Müşteri Ürün güncellendi.";


                    //if (item == null)
                    //{
                    //    retval.Message = "Ürün kaydına ulaşılamadı.";
                    //    return retval;
                    //}

                    var mproduct = new MProductsRepo().GetByID(model.MPID ?? 0);
                    if (mproduct == null)
                    {
                        //retval.StrObj.Add(new ResultRow() { ID = item.MPID, Text = "Müşteri kaydı bulunamadı." });
                        retval.Message = "Müşteri ürün kaydı bulunamadı.";
                        return retval;
                    }

                    mproduct.BUKOD = model.BUKOD;
                    mproduct.NAME = model.NAME;
                    mproduct.PRICE = model.PRICE;
                    mproduct.CURRENCY = model.CURRENCY;
                    mproduct.UpdatedBy = userID;
                    mproduct.UPDATED = DateTime.Now;
                    mproduct.MusteriID = model.MusteriID;
                    mproduct.NAME_DE = model.NAME_DE;
                    mproduct.NAME_EN = model.NAME_EN;
                    mproduct.OEM = model.OEM;
                    mproduct.PRICE = model.PRICE;
                    mproduct.ProductID = productitem?.PRODID; // model.ProductID;
                    mproduct.XPSNO = model.XPSNO;
                    mproduct.XPSUP = model.MusteriID.ToString();
                    mproduct.EDITOR_TABLE = model.EDITOR_TABLE;
                    new MProductsRepo().Update();
                    #endregion

                    #region insert log
                    new MProductsLogRepo().Insert(new MProductsLog()
                    {
                        BUKOD = mproduct.BUKOD,
                        CURRENCY = mproduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        MPID = mproduct.MPID,
                        MusteriID = mproduct.MusteriID,
                        NAME = mproduct.NAME,
                        NAME_DE = mproduct.NAME_DE,
                        NAME_EN = mproduct.NAME_EN,
                        OEM = mproduct.OEM,
                        LogType = "U",
                        ProductID = mproduct.ProductID,
                        PRICE = mproduct.PRICE,
                        XPSNO = mproduct.XPSNO,
                        XPSUP = mproduct.XPSUP,
                        EDITOR_TABLE = mproduct.EDITOR_TABLE,
                        UPDATED = mproduct.UPDATED,
                        UpdatedBy = mproduct.UpdatedBy
                        // UpdatesUser = userID
                    });

                    #endregion
                }

                retval.NewID = model.MPID ?? 0;
                retval.Message = successMessage;
                retval.IsSuccess = true;
                retval.TRNDate = DateTime.Now;
                return retval;
            }
            catch (Exception ex)
            {
                //todo: add to log
                retval.IsSuccess = false;
                retval.Message = "İşlem yapılırken bir hata oluştu.";
                return retval;
            }
        }
    }
}