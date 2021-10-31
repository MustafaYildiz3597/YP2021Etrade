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
    public class ProductRepo : RepositoryBase<PRODUCTS, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public List<ProductDTListWithPage_Result> ListDT(ProductDTListDTO model)
        {
            try
            {
                return db.ProductDTListWithPage(model.SecID, model.MCatID, model.CatID, model.OEMNo, model.pageNumber, model.pageSize, model.sortColumn, model.sortColumnDir, model.searchValue, model.searchitems.HasImage, model.searchitems.BUKod, model.searchitems.SectionName, model.searchitems.MCatName, model.searchitems.CatName, model.searchitems.ProductName, model.searchitems.Added, model.searchitems.Enabled).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ProductDTListWithPageCount_Result> ListDTCount(ProductDTListDTO model)
        {
            try
            {
                //db.Database.CommandTimeout = 60;
                return db.ProductDTListWithPageCount(model.SecID, model.MCatID, model.CatID, model.OEMNo, model.searchValue, model.searchitems.HasImage, model.searchitems.BUKod, model.searchitems.SectionName, model.searchitems.MCatName, model.searchitems.CatName, model.searchitems.ProductName, model.searchitems.Added, model.searchitems.Enabled).ToList();
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
                return (from p in db.PRODUCTS.Where(q => q.PRODID == id)
                        select new
                        {
                            p.PRODID,
                            p.BUKOD,
                            p.ADDED,
                            ADDEDBY = p.Member.FirstName + " " + p.Member.LastName,
                            p.CATGROUP,
                            p.CATID,
                            p.DESCC,
                            p.DESI,
                            p.EDITOR_TABLE,
                            p.EDITOR_TABLE_EN,
                            p.EDITOR_TABLE_FR,
                            p.EDITOR_TABLE_GR,
                            p.ENABLED,
                            p.MARKA,
                            p.MCATID,
                            p.NAME,
                            p.NAME_DE,
                            p.NAME_EN,
                            p.NAME_FR,
                            p.RESIM,
                            p.SECID,
                            p.SIRALAMA,
                            p.B2BBasePrice,
                            p.B2BCurrencyID,
                            B2BCurrencyCode = p.Currency != null ? p.Currency.Code : "",
                            p.B2BDiscountedPrice,
                            p.B2BIsNewProduct,
                            p.B2BIsOnSale,
                            p.B2BIsVisible,
                            p.B2BIsVisibleOnCategoryHomepage,
                            p.B2BIsVisibleOnHomepage,
                            p.B2BStockAmount,
                            p.CountInPalette,
                            p.EDITOR_B2B,
                            p.EDITOR_ManagerNote,
                            p.IsEuroPalette,
                            p.OpenForSale,
                            p.PackageDimensions,
                            p.PackagePieceCount,
                            p.TRESIM,
                            IsBundled = p.IsBundled ?? false,
                            p.UPDATED,
                            UPDATEDBY = p.Member1.FirstName + " " + p.Member1.LastName
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                //TODO: add to log
                throw;
            }

        }

        public Result ImportExcelData(ImportExcelUploadDataDTO model)
        {
            string userID = HttpContext.Current.User.Identity.GetUserId();

            Result retval = new Result() {
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
                    openCon.Open();

                    retval.TotalCount = model.Data.Count();
                    int succededcount = 0;

                    foreach (var item in model.Data)
                    {
                        var product = new ProductRepo().GetByFilter(q => q.BUKOD == item.BUKOD);

                        if (product == null)
                        {
                            string saveStaff = "INSERT into PRODUCTS ([BUKOD], [NAME], [NAME_EN], [NAME_DE], [NAME_FR], [EDITOR_TABLE], [EDITOR_TABLE_EN], [EDITOR_TABLE_GR], [EDITOR_TABLE_FR], ENABLED, SECID, MCATID, CATID, ADDED, ADDEDBY) " +
                                "VALUES (@BUKOD, @NAME, @NAME_EN, @NAME_DE, @NAME_FR, @EDITOR_TABLE, @EDITOR_TABLE_EN, @EDITOR_TABLE_GR, @EDITOR_TABLE_FR, @ENABLED, @SECID, @MCATID, @CATID, @ADDED, @ADDEDBY)";

                            using (SqlCommand queryProduct = new SqlCommand(saveStaff))
                            {
                                queryProduct.Connection = openCon;

                                queryProduct.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 50).Value = (object)item.BUKOD.Trim() ?? DBNull.Value;
                                queryProduct.Parameters.Add("@NAME", SqlDbType.NVarChar, 256).Value = (object)item.NAME ?? DBNull.Value;
                                queryProduct.Parameters.Add("@NAME_EN", SqlDbType.NVarChar, 256).Value =  (object)item.NAME_EN ?? DBNull.Value;
                                queryProduct.Parameters.Add("@NAME_DE", SqlDbType.NVarChar, 256).Value =  (object)item.NAME_DE ?? DBNull.Value;
                                queryProduct.Parameters.Add("@NAME_FR", SqlDbType.NVarChar, 256).Value = (object)item.NAME_FR ?? DBNull.Value;
                                queryProduct.Parameters.Add("@EDITOR_TABLE", SqlDbType.NText).Value = (object)item.EDITOR_TABLE ?? DBNull.Value;
                                queryProduct.Parameters.Add("@EDITOR_TABLE_EN", SqlDbType.NText).Value = (object)item.EDITOR_TABLE_EN ?? DBNull.Value;
                                queryProduct.Parameters.Add("@EDITOR_TABLE_GR", SqlDbType.NText).Value = (object)item.EDITOR_TABLE_GR ?? DBNull.Value;
                                queryProduct.Parameters.Add("@EDITOR_TABLE_FR", SqlDbType.NText).Value = (object)item.EDITOR_TABLE_FR ?? DBNull.Value;
                                queryProduct.Parameters.Add("@ENABLED", SqlDbType.Bit).Value = item.ENABLED == "1" ? 1 : 0;
                                queryProduct.Parameters.Add("@SECID", SqlDbType.Int).Value = model.Cats.SECID;
                                queryProduct.Parameters.Add("@MCATID", SqlDbType.Int).Value = model.Cats.MCATID;
                                queryProduct.Parameters.Add("@CATID", SqlDbType.Int).Value = model.Cats.CATID;
                                queryProduct.Parameters.Add("@ADDED", SqlDbType.DateTime).Value = DateTime.Now;
                                queryProduct.Parameters.Add("@ADDEDBY", SqlDbType.NVarChar, 128).Value = userID;

                               
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

                    if (openCon.State == System.Data.ConnectionState.Open) openCon.Close();
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
    }
}
