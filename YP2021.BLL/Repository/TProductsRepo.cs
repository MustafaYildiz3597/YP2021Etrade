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
    public class TProductsRepo : RepositoryBase<TPRODUCTS, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public List<TProductDTListWithPage_Result> ListDT(TPListDTDTO model)
        {
            try
            {
                return db.TProductDTListWithPage(model.TedarikciID, model.BUKOD, model.pageNumber, model.pageSize, model.sortColumn, model.sortColumnDir, model.searchValue, model.searchitems.BUKod, model.searchitems.FIRMA_ADI, model.searchitems.XPSNO, model.searchitems.NAME, model.searchitems.CreatedOn, model.searchitems.UPDATED, model.searchitems.PRICE, model.searchitems.CurrencyCode).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<TProductDTListWithPageCount_Result> ListDTCount(TPListDTDTO model)
        {
            try
            {
                //db.Database.CommandTimeout = 60;
                return db.TProductDTListWithPageCount(model.TedarikciID, model.BUKOD, model.searchValue, model.searchitems.BUKod, model.searchitems.FIRMA_ADI, model.searchitems.XPSNO, model.searchitems.NAME, model.searchitems.CreatedOn, model.searchitems.UPDATED, model.searchitems.PRICE, model.searchitems.CurrencyCode).ToList();
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
                return (from tp in db.TPRODUCTS.Where(q => q.TPID == id)
                        join cr in db.Member on tp.CreatedBy equals cr.ID into crj
                        from cr in crj.DefaultIfEmpty()
                        join ur in db.Member on tp.UpdatedBy equals ur.ID into urj
                        from ur in crj.DefaultIfEmpty()
                        join dr in db.Member on tp.DeletedBy equals dr.ID into drj
                        from dr in drj.DefaultIfEmpty()
                        select new
                        {
                            tp.TPID,
                            tp.BUKOD,
                            CreatedBy = cr == null ? "" : cr.FirstName + " " + cr.LastName,
                            tp.CreatedOn,
                            tp.CURRENCY,
                            CurrencyCode = tp == null ? "" : tp.Currency1.Code ?? "",
                            DeletedMember = dr == null ? "" : dr.FirstName + " " + dr.LastName,
                            tp.DeletedOn,
                            tp.EDITOR_TABLE,
                            tp.MusteriID,
                            MusteriName = tp == null ? "" : tp.MUSTERILER.FIRMA_ADI,
                            tp.NAME,
                            tp.NAME_DE,
                            tp.NAME_EN,
                            tp.OEM,
                            tp.PRICE,
                            tp.ProductID,
                            ProductName = tp == null ? "" : tp.PRODUCTS.NAME,
                            tp.UPDATED,
                            UpdatedBy = ur == null ? "" : ur.FirstName + " " + ur.LastName,
                            tp.XPSNO,
                            tp.XPSUP
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Result ImportExcelData(TPImportExcelUploadDataDTO model)
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


                var currencies = new CurrencyRepo().GetAll().Select(s => new {s.ID, s.Code }).ToList();
                var userID = HttpContext.Current.User.Identity.GetUserId();

                var exceluploadinsert = new ExcelUploadLog()
                {
                    CreateDT = DateTime.Now,
                    CreateUserID = userID,
                    TotalRowCount = totalrowcount,
                    ExcelUploadEntity = "Tproducts"
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
                                /* BUKOD ürün tablosunda tanımmlı değilse TPRODUCTS tablosuna eklemiyoruz. */
                                retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = "Ürün kaydı bulunamadı." });
                                continue;
                            }
                            else
                            {
                                productID = product.PRODID;
                            }
                        }

                        #region old
                        //var currency = new CurrencyRepo().GetByFilter(q => q.Code == item.CURRENCY);
                        //if (currency == null)
                        //{
                        //    retval.StrObj.Add(new ResultRow() { Text = item.BUKOD, Description = "Geçersiz Para birimi." });
                        //    continue;
                        //}
                        #endregion

                        var currency = currencies.Where(q => q.Code == item.CURRENCY).Select(s => new { s.ID }).FirstOrDefault();

                        #region old
                        //var tproduct = new TProductsRepo().GetByFilter(q => (!string.IsNullOrWhiteSpace(item.BUKOD) && q.BUKOD == item.BUKOD && q.MusteriID == model.MusteriID) ||
                        //        (string.IsNullOrWhiteSpace(item.BUKOD) && q.XPSNO == item.XPSNO && q.MusteriID == model.MusteriID)
                        //);
                        #endregion

                        var tproduct = new TProductsRepo().GetByFilter(q => q.BUKOD == item.BUKOD && q.MusteriID == model.MusteriID && q.XPSNO == item.XPSNO);

                        if (tproduct == null)
                        {
                            query = "INSERT INTO TPRODUCTS(ADDED,BUKOD,CreatedBy,CreatedOn,CURRENCY,MusteriID,NAME,NAME_DE,NAME_EN,OEM,PRICE,ProductID,XPSNO,XPSUP,IsDeleted) " +
                                "VALUES (@ADDED,@BUKOD,@CreatedBy,@CreatedOn,@CURRENCY,@MusteriID,@NAME,@NAME_DE,@NAME_EN,@OEM,@PRICE,@ProductID,@XPSNO,@XPSUP,0); SELECT SCOPE_IDENTITY();";

                            using (SqlCommand sqlcommand = new SqlCommand(query))
                            {
                                sqlcommand.Connection = openCon;

                                sqlcommand.Parameters.Add("@ADDED", SqlDbType.DateTime).Value = DateTime.Now;
                                sqlcommand.Parameters.Add("@BUKOD", SqlDbType.VarChar, 50).Value = (object)item.BUKOD ?? DBNull.Value;
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

                            query = "INSERT INTO TProductsLog(ID,TPID,LogType,ADDED,BUKOD,CreatedBy,CreatedOn,CURRENCY,MusteriID,NAME,NAME_DE,NAME_EN,OEM,PRICE,ProductID,XPSNO,XPSUP,IsDeleted) " +
                              "VALUES (@ID,@TPID,@LogType,@ADDED,@BUKOD,@CreatedBy,@CreatedOn,@CURRENCY,@MusteriID,@NAME,@NAME_DE,@NAME_EN,@OEM,@PRICE,@ProductID,@XPSNO,@XPSUP,0)";

                            using (SqlCommand sqlcommand = new SqlCommand(query))
                            {
                                sqlcommand.Connection = openCon;

                                sqlcommand.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                sqlcommand.Parameters.Add("@TPID", SqlDbType.Int).Value = newID;
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

                            #region old
                            //var newTProduct = new TPRODUCTS()
                            //{
                            //    ADDED = DateTime.Now,
                            //    BUKOD = item.BUKOD,
                            //    CreatedBy = HttpContext.Current.User.Identity.GetUserId(),
                            //    CreatedOn = DateTime.Now,
                            //    CURRENCY = currency.ID,
                            //    MusteriID = model.MusteriID,
                            //    NAME = item.NAME,
                            //    NAME_DE = item.NAME_DE,
                            //    NAME_EN = item.NAME_EN,
                            //    OEM = item.OEM,
                            //    PRICE = item.PRICE,
                            //    ProductID = productID,
                            //    XPSNO = item.XPSNO,
                            //    XPSUP = model.MusteriID.ToString(),
                            //    IsDeleted = false
                            //};

                            //new TProductsRepo().Insert(newTProduct);


                            //new TProductsLogRepo().Insert(new TProductsLog()
                            //{
                            //    ADDED = newTProduct.ADDED,
                            //    BUKOD = newTProduct.BUKOD,
                            //    CreatedBy = newTProduct.CreatedBy,
                            //    CreatedOn = newTProduct.CreatedOn,
                            //    CURRENCY = newTProduct.CURRENCY,
                            //    ID = Guid.NewGuid(),
                            //    IsDeleted = newTProduct.IsDeleted,
                            //    TPID = newTProduct.TPID,
                            //    MusteriID = newTProduct.MusteriID,
                            //    NAME = newTProduct.NAME,
                            //    NAME_DE = newTProduct.NAME_DE,
                            //    NAME_EN = newTProduct.NAME_EN,
                            //    OEM = newTProduct.OEM,
                            //    LogType = "I",
                            //    ProductID = productID,
                            //    PRICE = newTProduct.PRICE,
                            //    XPSNO = newTProduct.XPSNO,
                            //    XPSUP = newTProduct.XPSUP
                            //});
                            #endregion
                        }
                        else
                        {
                            if (tproduct.CURRENCY != currency.ID ||
                                tproduct.NAME != item.NAME ||
                                tproduct.NAME_DE != item.NAME_DE ||
                                tproduct.NAME_EN != item.NAME_EN ||
                                tproduct.OEM != item.OEM ||
                                tproduct.PRICE != item.PRICE ||
                                tproduct.XPSNO != item.XPSNO ||
                                tproduct.XPSUP != model.MusteriID.ToString()
                                ) // kayıt değişmiş mi kontrolü
                            {
                                #region update query
                                query = "UPDATE t SET t.CURRENCY=@CURRENCY,t.NAME=@NAME,t.NAME_DE=@NAME_DE,t.NAME_EN=@NAME_EN,t.OEM=@OEM,t.PRICE=@PRICE,t.XPSUP=@XPSUP,t.UPDATED=@UPDATED,t.UpdatedBy=@UpdatedBy " +
                                    " FROM TPRODUCTS t " +
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

                                    sqlcommand.Parameters.Add("@BUKOD", SqlDbType.VarChar, 50).Value = (object)item.BUKOD ?? DBNull.Value;
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

                                query = "INSERT INTO TProductsLog(ID,TPID,LogType,ADDED,BUKOD,UpdatedBy,UPDATED,CURRENCY,MusteriID,NAME,NAME_DE,NAME_EN,OEM,PRICE,ProductID,XPSNO,XPSUP,IsDeleted) " +
                             "VALUES (@ID,@TPID,@LogType,@ADDED,@BUKOD,@UpdatedBy,@UPDATED,@CURRENCY,@MusteriID,@NAME,@NAME_DE,@NAME_EN,@OEM,@PRICE,@ProductID,@XPSNO,@XPSUP,0)";

                                using (SqlCommand sqlcommand = new SqlCommand(query))
                                {
                                    sqlcommand.Connection = openCon;

                                    sqlcommand.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                    sqlcommand.Parameters.Add("@TPID", SqlDbType.Int).Value = tproduct.TPID;
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

                                #region old
                                //tproduct.CURRENCY = currency.ID;
                                //tproduct.NAME = item.NAME;
                                //tproduct.NAME_DE = item.NAME_DE;
                                //tproduct.NAME_EN = item.NAME_EN;
                                //tproduct.OEM = item.OEM;
                                //tproduct.PRICE = item.PRICE;
                                //tproduct.XPSNO = item.XPSNO;
                                //tproduct.XPSUP = model.MusteriID.ToString();
                                //tproduct.UPDATED = DateTime.Now;
                                //tproduct.UpdatedBy = HttpContext.Current.User.Identity.GetUserId();

                                //new TProductsRepo().Update();

                                //new TProductsLogRepo().Insert(new TProductsLog()
                                //{
                                //    BUKOD = tproduct.BUKOD,
                                //    CURRENCY = tproduct.CURRENCY,
                                //    ID = Guid.NewGuid(),
                                //    TPID = tproduct.TPID,
                                //    MusteriID = tproduct.MusteriID,
                                //    NAME = tproduct.NAME,
                                //    NAME_DE = tproduct.NAME_DE,
                                //    NAME_EN = tproduct.NAME_EN,
                                //    OEM = tproduct.OEM,
                                //    LogType = "U",
                                //    ProductID = productID,
                                //    PRICE = tproduct.PRICE,
                                //    XPSNO = tproduct.XPSNO,
                                //    XPSUP = tproduct.XPSUP,
                                //    UPDATED = tproduct.UPDATED,
                                //    UpdatedBy = tproduct.UpdatedBy
                                //    // UpdatedUser = HttpContext.Current.User.Identity.GetUserId()
                                //});
                                #endregion

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

                // retval.SuccededCount = succededrowcount;
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

        }

        public Result SaveGridEdit(SaveTUGridEditDTO model)
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
                    var tproduct = new TProductsRepo().GetByID(item.TPID);
                    if (tproduct == null)
                    {
                        retval.StrObj.Add(new ResultRow() { ID = item.TPID, Text = "", Description = "Tedarikçi ürün kaydı bulunamadı." });
                        continue;
                    }

                    if (String.IsNullOrWhiteSpace(tproduct.BUKOD) && String.IsNullOrWhiteSpace(item.XPSNO))
                    {
                        retval.StrObj.Add(new ResultRow() { ID = item.TPID, Text = item.XPSNO, Description = "Aynı satırda Müşteri-Ürün Numarası ve ya BU Numarası boş olamaz!" });
                        continue;
                    }

                    //if (!String.IsNullOrWhiteSpace(item.XPSNO))
                    //{
                    var exists = new TProductsRepo().GetByFilter(q => q.MusteriID == tproduct.MusteriID && q.TPID != item.TPID && q.XPSNO == item.XPSNO && q.BUKOD == tproduct.BUKOD);
                    if (exists != null)
                    {
                        retval.StrObj.Add(new ResultRow() { ID = item.TPID, Text = item.XPSNO, Description = item.XPSNO + " Tedarikçi-Ürün Numarası bu müşteri için zaten tanımlı!" });
                        continue;
                    }
                    //}

                    tproduct.NAME = item.NAME;
                    tproduct.PRICE = item.PRICE;
                    tproduct.CURRENCY = item.CURRENCY;
                    tproduct.UpdatedBy = userID;
                    tproduct.UPDATED = DateTime.Now;

                    new TProductsRepo().Update();

                    #region insert log
                    new TProductsLogRepo().Insert(new TProductsLog()
                    {
                        BUKOD = tproduct.BUKOD,
                        CURRENCY = tproduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        TPID = tproduct.TPID,
                        MusteriID = tproduct.MusteriID,
                        NAME = tproduct.NAME,
                        NAME_DE = tproduct.NAME_DE,
                        NAME_EN = tproduct.NAME_EN,
                        OEM = tproduct.OEM,
                        LogType = "U",
                        ProductID = tproduct.ProductID,
                        PRICE = tproduct.PRICE,
                        XPSNO = tproduct.XPSNO,
                        XPSUP = tproduct.XPSUP,
                        UPDATED = tproduct.UPDATED,
                        UpdatedBy = tproduct.UpdatedBy
                        // UpdatesUser = userID
                    });

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

        public Result SaveGridAdd(SaveTUGridAddDTO model)
        {
            string userID = HttpContext.Current.User.Identity.GetUserId();
            Result retval = new Result()
            {
                IsSuccess = false,
                StrObj = new List<ResultRow>()
            };

            if (model.Tedarikciler.Count() == 0)
            {
                retval.Message = "Tedarikçi seçmelisiniz!";
                return retval;
            }
            if (model.Products.Count() == 0)
            {
                retval.Message = "Tedarikçi-Ürün girmelisiniz!";
                return retval;
            }

            foreach (var item in model.Products)
            {
                if (String.IsNullOrWhiteSpace(item.XPSNO) && String.IsNullOrWhiteSpace(item.BUKOD))
                {
                    retval.Message = "Aynı satırda Tedarikçi-Ürün Numarası ve BU Numarası boş olan kayıt(lar) var. Lütfen kontrol ediniz!";
                    return retval;
                }
            }

            int musteriID = model.Tedarikciler[0].ID ?? 0;

            try
            {
                retval.TotalCount = model.Products.Count();
                retval.SuccededCount = 0;

                foreach (var item in model.Products)
                {
                    //foreach (var musteriItem in model.Tedarikciler)
                    //{
                    item.XPSNO = String.IsNullOrWhiteSpace(item.XPSNO) ? null : item.XPSNO.Trim();
                    item.BUKOD = String.IsNullOrWhiteSpace(item.BUKOD) ? null : item.BUKOD.Trim();

                    int? productID = null;

                    if (!string.IsNullOrWhiteSpace(item.BUKOD))
                    {
                        var product = new ProductRepo().GetByFilter(q => q.BUKOD == item.BUKOD && (q.Deleted ?? false) == false);
                        if (product == null)
                        {
                            /* BUKOD ürün tablosunda tanımmlı değilse TPRODUCTS tablosuna eklemiyoruz. */
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

                    //var tproduct = new TProductsRepo().GetByFilter(q => (!string.IsNullOrWhiteSpace(item.BUKOD) && q.BUKOD == item.BUKOD && q.MusteriID == musteriItem.ID) ||
                    //        (string.IsNullOrWhiteSpace(item.BUKOD) && q.XPSNO == item.XPSNO && q.MusteriID == musteriItem.ID)
                    //);

                    //if (tproduct == null)
                    //{

                    //if (!String.IsNullOrWhiteSpace(item.XPSNO))
                    //{
                    var exists = new MProductsRepo().GetByFilter(q => q.MusteriID == musteriID && q.XPSNO == item.XPSNO && q.BUKOD == item.BUKOD);
                    if (exists != null)
                    {
                        retval.StrObj.Add(new ResultRow() { ID = item.TPID, Text = item.XPSNO, Description = item.XPSNO + " Tedarikçi-Ürün Numarası bu tedarikçi için zaten tanımlı!" });
                        continue;
                    }
                    //}

                    //if (!String.IsNullOrWhiteSpace(item.BUKOD))
                    //{
                    //    var exists = new MProductsRepo().GetByFilter(q => q.MusteriID == musteriID && q.BUKOD == item.BUKOD);
                    //    if (exists != null)
                    //    {
                    //        retval.StrObj.Add(new ResultRow() { ID = item.TPID, Text = item.BUKOD, Description = item.BUKOD + " BU Numarası bu tedarikçi için zaten tanımlı!" });
                    //        continue;
                    //    }
                    //}

                    var newTProduct = new TPRODUCTS()
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

                    new TProductsRepo().Insert(newTProduct);

                    new TProductsLogRepo().Insert(new TProductsLog()
                    {
                        ADDED = newTProduct.ADDED,
                        BUKOD = newTProduct.BUKOD,
                        CreatedBy = newTProduct.CreatedBy,
                        CreatedOn = newTProduct.CreatedOn,
                        CURRENCY = newTProduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        IsDeleted = newTProduct.IsDeleted,
                        TPID = newTProduct.TPID,
                        MusteriID = newTProduct.MusteriID,
                        NAME = newTProduct.NAME,
                        NAME_DE = newTProduct.NAME_DE,
                        NAME_EN = newTProduct.NAME_EN,
                        OEM = newTProduct.OEM,
                        LogType = "I",
                        ProductID = productID,
                        PRICE = newTProduct.PRICE,
                        XPSNO = newTProduct.XPSNO,
                        XPSUP = newTProduct.XPSUP
                    });

                    retval.SuccededCount++;
                    //}
                    // }
                }

                retval.IsSuccess = true;
                return retval;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Result Save(SaveTProductDTO model)
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

            if (String.IsNullOrWhiteSpace(model.BUKOD) && String.IsNullOrWhiteSpace(model.XPSNO))
            {
                retval.Message = "Ürün kodu ve müşteri ürün numarasından en az birisi eksik!";
                return retval;
            }

            try
            {
                var productitem = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == model.BUKOD.Trim());

                if (model.TPID == null)
                {
                    #region ol
                    //var tproduct = new TProductsRepo().GetByFilter(q => (!string.IsNullOrWhiteSpace(model.BUKOD) && q.BUKOD == model.BUKOD && q.MusteriID == model.MusteriID) ||
                    //            (string.IsNullOrWhiteSpace(model.BUKOD) && q.XPSNO == model.XPSNO && q.MusteriID == model.MusteriID)
                    //    );
                    #endregion

                    var tproduct = new TProductsRepo().GetByFilter(q => q.BUKOD == model.BUKOD && q.XPSNO == model.XPSNO && q.MusteriID == model.MusteriID);
                    if (tproduct != null)
                    {
                        retval.Message = "Tedarikçi ürünü daha önceden kayıtlı bulunduğundan işlem yapılamadı!";
                        return retval;
                    }

                    #region new item       

                    successMessage = "Tedarikçi Ürün eklendi.";

                    var newTProduct = new TPRODUCTS()
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
                        PRICE = model.PRICE,
                        ProductID = productitem?.PRODID, //model.ProductID,
                        XPSNO = model.XPSNO,
                        XPSUP = model.MusteriID.ToString(),
                        EDITOR_TABLE = model.EDITOR_TABLE,
                        IsDeleted = false
                    };
                    new TProductsRepo().Insert(newTProduct);

                    model.TPID = newTProduct.TPID;

                    new TProductsLogRepo().Insert(new TProductsLog()
                    {
                        ADDED = newTProduct.ADDED,
                        BUKOD = newTProduct.BUKOD,
                        CreatedBy = newTProduct.CreatedBy,
                        CreatedOn = newTProduct.CreatedOn,
                        CURRENCY = newTProduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        IsDeleted = newTProduct.IsDeleted,
                        TPID = newTProduct.TPID,
                        MusteriID = newTProduct.MusteriID,
                        NAME = newTProduct.NAME,
                        NAME_DE = newTProduct.NAME_DE,
                        NAME_EN = newTProduct.NAME_EN,
                        OEM = newTProduct.OEM,
                        LogType = "I",
                        ProductID = newTProduct.ProductID,
                        PRICE = newTProduct.PRICE,
                        XPSNO = newTProduct.XPSNO,
                        XPSUP = newTProduct.XPSUP,
                        EDITOR_TABLE = newTProduct.EDITOR_TABLE
                    });

                    #endregion
                }
                else
                {
                    #region old
                    //var othertproduct = new TProductsRepo().GetByFilter(q => ((!string.IsNullOrWhiteSpace(model.BUKOD) && q.BUKOD == model.BUKOD && q.MusteriID == model.MusteriID) ||
                    //            (string.IsNullOrWhiteSpace(model.BUKOD) && q.XPSNO == model.XPSNO && q.MusteriID == model.MusteriID))
                    //            && model.TPID != model.TPID
                    //            );
                    #endregion

                    var othertproduct = new TProductsRepo().GetByFilter(q => q.BUKOD == model.BUKOD && q.MusteriID == model.MusteriID && q.XPSNO == model.XPSNO && model.TPID != model.TPID);
                    if (othertproduct != null)
                    {
                        retval.Message = "Tedarikçi ürünü daha önceden kayıtlı bulunduğundan işlem yapılamadı!";
                        return retval;
                    }

                    #region update item

                    successMessage = "Tedarikçi Ürün güncellendi.";

                    // var item = new ProductRepo().GetByFilter(q => q.BUKOD.Trim() == model.BUKOD.Trim());
                    //if (item == null)
                    //{
                    //    retval.Message = "Ürün kaydına ulaşılamadı.";
                    //    return retval;
                    //}

                    var tproduct = new TProductsRepo().GetByID(model.TPID ?? 0);
                    if (tproduct == null)
                    {
                        //retval.StrObj.Add(new ResultRow() { ID = item.TPID, Text = "Tedarikçi kaydı bulunamadı." });
                        retval.Message = "Tedarikçi ürün kaydı bulunamadı.";
                        return retval;
                    }

                    tproduct.BUKOD = model.BUKOD;
                    tproduct.NAME = model.NAME;
                    tproduct.PRICE = model.PRICE;
                    tproduct.CURRENCY = model.CURRENCY;
                    tproduct.UpdatedBy = userID;
                    tproduct.UPDATED = DateTime.Now;
                    tproduct.MusteriID = model.MusteriID;
                    tproduct.NAME_DE = model.NAME_DE;
                    tproduct.NAME_EN = model.NAME_EN;
                    tproduct.OEM = model.OEM;
                    tproduct.PRICE = model.PRICE;
                    tproduct.ProductID = productitem?.PRODID; //model.ProductID;
                    tproduct.XPSNO = model.XPSNO;
                    tproduct.EDITOR_TABLE = model.EDITOR_TABLE;
                    tproduct.XPSUP = model.MusteriID.ToString();
                    new TProductsRepo().Update();
                    #endregion

                    #region insert log
                    new TProductsLogRepo().Insert(new TProductsLog()
                    {
                        BUKOD = tproduct.BUKOD,
                        CURRENCY = tproduct.CURRENCY,
                        ID = Guid.NewGuid(),
                        TPID = tproduct.TPID,
                        MusteriID = tproduct.MusteriID,
                        NAME = tproduct.NAME,
                        NAME_DE = tproduct.NAME_DE,
                        NAME_EN = tproduct.NAME_EN,
                        OEM = tproduct.OEM,
                        LogType = "U",
                        ProductID = tproduct.ProductID,
                        PRICE = tproduct.PRICE,
                        XPSNO = tproduct.XPSNO,
                        XPSUP = tproduct.XPSUP,
                        EDITOR_TABLE = tproduct.EDITOR_TABLE,
                        UPDATED = tproduct.UPDATED,
                        UpdatedBy = tproduct.UpdatedBy
                        // UpdatesUser = userID
                    });

                    #endregion
                }

                retval.NewID = model.TPID ?? 0;
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
