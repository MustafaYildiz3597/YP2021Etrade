using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Nero2021.BLL.Utilities;
using Nero2021.Data;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Text.RegularExpressions;


namespace Nero.Web.api
{
    //[Authorize]
    public class ProductController : ApiController
    {
        [HttpPost]
        public IHttpActionResult BrowseList()
        {
            try
            {
                var products = new ProductRepo().GetAll(q => (q.Deleted ?? false) == false).Select(s => new
                {
                    s.PRODID,
                    s.BUKOD,
                    s.NAME,
                    s.NAME_EN
                }).AsQueryable();

                return Ok(new
                {
                    Products = products
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Ürünler yüklenemedi.");
            }
        }

        [HttpPost]
        public IHttpActionResult DTList(ProductDTListDTO model)
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

                model.searchitems = new ProductDTListSearchItems();

                string hasimage = HttpContext.Current.Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                if (Int32.TryParse(hasimage, out int i))
                    model.searchitems.HasImage = i;

                model.searchitems.BUKod = HttpContext.Current.Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                model.searchitems.SectionName = HttpContext.Current.Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
                model.searchitems.MCatName = HttpContext.Current.Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
                model.searchitems.CatName = HttpContext.Current.Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
                model.searchitems.ProductName = HttpContext.Current.Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
                model.searchitems.Added = HttpContext.Current.Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
                model.searchitems.Enabled = HttpContext.Current.Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();

                model.pageNumber = skip / pageSize;
                model.pageSize = pageSize;
                model.sortColumn = sortColumn;
                model.sortColumnDir = sortColumnDir;
                model.searchValue = searchValue.ToString();

                var counts = new ProductRepo().ListDTCount(model);
                var data = new ProductRepo().ListDT(model);

                recordsTotal = data.Count();

                return Ok(new { draw, recordsFiltered = counts[0].RecordFilteredCount, recordsTotal = counts[0].RecordCount, data });
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }

        }


        [HttpPost]
        public IHttpActionResult Detail(ProductDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var productdetail = new ProductRepo().FindByID(model.ID);
                var oemlist = new OEMRepo().GetAll(q => q.PRODID == model.ID && (q.IsDeleted ?? false) == false).Select(s => new { s.OEMID, s.OEMNR, s.BUKOD, s.PRODID, s.OEMSUPNAME?.SUPNAME });
                var mproductlist = new MProductsRepo().GetAll(q => q.ProductID == model.ID).Select(s => new { s.MPID, s.BUKOD, s.ProductID, s.MUSTERILER?.FIRMA_ADI, s.XPSNO, s.BUESKI, s.NAME, s.ADDED, s.UPDATED, s.PRICE, s.CURRENCY, CurrencyCode = s.Currency1?.Code });
                var tproductlist = new TProductsRepo().GetAll(q => q.ProductID == model.ID).Select(s => new { s.TPID, s.BUKOD, s.ProductID, s.MUSTERILER?.FIRMA_ADI, s.XPSNO, s.NAME, s.OEM, s.PRICE, s.CURRENCY, s.BRAND, s.CATNAME, CurrencyCode = s.Currency1?.Code, s.ADDED });
                var productimagelist = new ProductImageRepo().GetAll(q => q.ProductID == model.ID && q.ItemType == 1 && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.ID, s.Title, CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName, s.CreatedOn, s.Description, FileName = s.FilePath, FilePath = s.FilePath + "?t=" + DateTime.Now.Ticks.ToString(), s.ProductID, s.ItemType, s.RankNumber, s.ThumbnailPath });
                var producttimagelist = new ProductImageRepo().GetAll(q => q.ProductID == model.ID && q.ItemType == 2 && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.ID, s.Title, CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName, s.CreatedOn, s.Description, FileName = s.FilePath, FilePath = s.FilePath + "?t=" + DateTime.Now.Ticks.ToString(), s.ProductID, s.ItemType, s.RankNumber, s.ThumbnailPath });
                var b2bPriceList = new B2BDealerTypeRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new { s.ID, s.Name, s.SalesRate });
                var relatedProducts = new RelatedProductRepo().GetAll(q => q.ProductID == model.ID).Select(s => new { s.ID, RelatedBUKod = s.PRODUCTS1?.BUKOD, s.ProductID, ProductName = s.PRODUCTS?.NAME, s.RelatedID, RelatedProductName = s.PRODUCTS1?.NAME, s.Quantity, s.RankNumber });
                var bundledProducts = new ProductBundleRepo().GetAll(q => q.ProductID == model.ID).Select(s => new { s.ID, BundledBUKod = s.PRODUCTS1?.BUKOD, s.ProductID, ProductName = s.PRODUCTS?.NAME, s.RelatedID, BundledProductName = s.PRODUCTS1?.NAME, s.Quantity, s.RankNumber });

                return Ok(new
                {
                    ProductDetail = productdetail,
                    OemList = oemlist,
                    MProductlist = mproductlist,
                    TProductlist = tproductlist,
                    ProductImageList = productimagelist,
                    ProductTImageList = producttimagelist,
                    B2BPriceList = b2bPriceList,
                    RelatedProducts = relatedProducts,
                    BundledProducts = bundledProducts
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(ProductDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Ürün silindi.";

                var product = new ProductRepo().GetByID(model.ID);
                if (product == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                product.Deleted = true;
                product.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                product.DeletedOn = DateTime.Now;
                new ProductRepo().Update();

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(ProductSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (String.IsNullOrWhiteSpace(model.BUKOD))
                return BadRequest("BUKOD bilgisi eksik.");

            model.BUKOD = model.BUKOD.Trim();

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;
            #region old
            //Regex reg = new Regex("[*'\",_&#^@]");
            //string filename = reg.Replace(model.BUKOD + ".png", string.Empty);

            #region save image 
            //string filepath = Path.Combine(HttpContext.Current.Server.MapPath("~/upload/pimages/"), filename);

            //if (!String.IsNullOrWhiteSpace(model.RESIMDATA))
            //{
            //    using (FileStream fs = new FileStream(filepath, FileMode.Create))
            //    {
            //        using (BinaryWriter bw = new BinaryWriter(fs))
            //        {
            //            byte[] data = Convert.FromBase64String(model.RESIMDATA);
            //            bw.Write(data);
            //            bw.Close();
            //        }
            //    }

            //    if (!System.IO.File.Exists(filepath))
            //        return BadRequest("Resim kaydedilirken bir hata oluştu.");
            //}

            //if (model.DeletedResim == true)
            //{
            //    if (System.IO.File.Exists(filepath))
            //        File.Delete(filepath);
            //}
            #endregion

            #region save teknik resim image 
            //string trfilepath = Path.Combine(HttpContext.Current.Server.MapPath("~/upload/trimages/"), filename);

            //if (!String.IsNullOrWhiteSpace(model.TRESIMDATA))
            //{
            //    using (FileStream fs = new FileStream(trfilepath, FileMode.Create))
            //    {
            //        using (BinaryWriter bw = new BinaryWriter(fs))
            //        {
            //            byte[] data = Convert.FromBase64String(model.TRESIMDATA);
            //            bw.Write(data);
            //            bw.Close();
            //        }
            //    }

            //    if (!System.IO.File.Exists(trfilepath))
            //        return BadRequest("Resim kaydedilirken bir hata oluştu.");
            //}

            //if (model.DeletedTResim == true)
            //{
            //    if (System.IO.File.Exists(trfilepath))
            //        File.Delete(trfilepath);
            //}
            #endregion
            #endregion

            try
            {
                if (model.PRODID == null)
                {
                    #region check BUKOD 

                    var bukodrow = new ProductRepo().GetByFilter(q => q.BUKOD == model.BUKOD);
                    if (bukodrow != null)
                    {
                        if (bukodrow.Deleted == true)
                            return BadRequest("BU Numarası sistemde silinmiş durumda kayıtlı bulunduğundan işlem yapılamadı!");
                        else
                            return BadRequest("BU Numarası zaten sistemde kayıtlı olduğundan işlem yapılamadı!");
                    }
                    #endregion

                    #region new item       

                    successMessage = "Ürün eklendi.";

                    var item = new PRODUCTS()
                    {
                        BUKOD = model.BUKOD,
                        CATGROUP = model.CATGROUP,
                        CATID = model.CATID,
                        DESCC = model.DESCC,
                        DESI = model.DESI,
                        EDITOR_TABLE = model.EDITOR_TABLE,
                        EDITOR_TABLE_EN = model.EDITOR_TABLE_EN,
                        EDITOR_TABLE_FR = model.EDITOR_TABLE_FR,
                        EDITOR_TABLE_GR = model.EDITOR_TABLE_GR,
                        ENABLED = model.ENABLED,
                        MARKA = model.MARKA,
                        MCATID = model.MCATID,
                        NAME = model.NAME,
                        NAME_DE = model.NAME_DE,
                        NAME_EN = model.NAME_EN,
                        NAME_FR = model.NAME_FR,
                        //RESIM = !String.IsNullOrWhiteSpace(model.RESIMDATA) ? filename : null,
                        SECID = model.SECID,
                        SIRALAMA = model.SIRALAMA,
                        //TRESIM = !String.IsNullOrWhiteSpace(model.TRESIMDATA) ? filename : null,
                        ADDED = DateTime.Now,
                        ADDEDBY = userID,
                        Deleted = false,
                        B2BBasePrice = model.B2BBasePrice,
                        B2BCurrencyID = model.B2BCurrencyID,
                        B2BDiscountedPrice = model.B2BDiscountedPrice,
                        B2BIsNewProduct = model.B2BIsNewProduct,
                        B2BIsOnSale = model.B2BIsOnSale,
                        B2BIsVisible = model.B2BIsVisible,
                        B2BIsVisibleOnCategoryHomepage = model.B2BIsVisibleOnCategoryHomepage,
                        B2BIsVisibleOnHomepage = model.B2BIsVisibleOnHomepage,
                        B2BStockAmount = model.B2BStockAmount,
                        CountInPalette = model.CountInPalette,
                        EDITOR_B2B = model.EDITOR_B2B,
                        EDITOR_ManagerNote = model.EDITOR_ManagerNote,
                        IsEuroPalette = model.IsEuroPalette,
                        OpenForSale = model.OpenForSale,
                        PackageDimensions = model.PackageDimensions,
                        PackagePieceCount = model.PackagePieceCount,
                        IsBundled = model.IsBundled
                    };
                    new ProductRepo().Insert(item);

                    model.PRODID = item.PRODID;
                    #endregion
                }
                else
                {
                    #region check BUKOD 
                    var bukodrow = new ProductRepo().GetByFilter(q => q.BUKOD == model.BUKOD && q.PRODID != model.PRODID);
                    if (bukodrow != null)
                    {
                        if (bukodrow.Deleted == true)
                            return BadRequest("Değiştirdiğiniz BU Numarası sistemde silinmiş durumda kayıtlı bulunduğundan işlem yapılamadı!");
                        else
                            return BadRequest("Değiştirdiğiniz BU Numarası zaten sistemde kayıtlı olduğundan işlem yapılamadı!");
                    }
                    #endregion

                    #region update item

                    successMessage = "Ürün güncellendi.";

                    var item = new ProductRepo().GetByID(model.PRODID ?? 0);
                    if (item == null)
                        return BadRequest("Ürün kaydına ulaşılamadı.");

                    string oldBUKOD = item.BUKOD.Trim();

                    item.BUKOD = model.BUKOD;
                    item.CATGROUP = model.CATGROUP;
                    item.CATID = model.CATID;
                    item.DESCC = model.DESCC;
                    item.DESI = model.DESI;
                    item.EDITOR_TABLE = model.EDITOR_TABLE;
                    item.EDITOR_TABLE_EN = model.EDITOR_TABLE_EN;
                    item.EDITOR_TABLE_FR = model.EDITOR_TABLE_FR;
                    item.EDITOR_TABLE_GR = model.EDITOR_TABLE_GR;
                    item.ENABLED = model.ENABLED;
                    item.MARKA = model.MARKA;
                    item.MCATID = model.MCATID;
                    item.NAME = model.NAME;
                    item.NAME_DE = model.NAME_DE;
                    item.NAME_EN = model.NAME_EN;
                    item.NAME_FR = model.NAME_FR;

                    //if (model.DeletedResim == true)
                    //    item.RESIM = null;
                    //else
                    //    item.RESIM = !String.IsNullOrWhiteSpace(model.RESIMDATA) ? filename : item.RESIM;

                    item.SECID = model.SECID;
                    item.SIRALAMA = model.SIRALAMA;


                    //item.TRESIM = model.TRESIM;
                    //if (model.DeletedTResim == true)
                    //    item.TRESIM = null;
                    //else
                    //    item.TRESIM = !String.IsNullOrWhiteSpace(model.TRESIMDATA) ? filename : item.TRESIM;

                    item.B2BBasePrice = model.B2BBasePrice;
                    item.B2BCurrencyID = model.B2BCurrencyID;
                    item.B2BDiscountedPrice = model.B2BDiscountedPrice;
                    item.B2BIsNewProduct = model.B2BIsNewProduct;
                    item.B2BIsOnSale = model.B2BIsOnSale;
                    item.B2BIsVisible = model.B2BIsVisible;
                    item.B2BIsVisibleOnCategoryHomepage = model.B2BIsVisibleOnCategoryHomepage;
                    item.B2BIsVisibleOnHomepage = model.B2BIsVisibleOnHomepage;
                    item.B2BStockAmount = model.B2BStockAmount;
                    item.CountInPalette = model.CountInPalette;
                    item.EDITOR_B2B = model.EDITOR_B2B;
                    item.EDITOR_ManagerNote = model.EDITOR_ManagerNote;
                    item.IsEuroPalette = model.IsEuroPalette;
                    item.OpenForSale = model.OpenForSale;
                    item.PackageDimensions = model.PackageDimensions;
                    item.PackagePieceCount = model.PackagePieceCount;
                    item.IsBundled = model.IsBundled;

                    item.UPDATED = DateTime.Now;
                    item.UPDATEDBY = userID;

                    new ProductRepo().Update();

                    if (oldBUKOD != model.BUKOD.Trim())
                    {
                        #region change images 
                        var images = new ProductImageRepo().GetAll(q => q.ProductID == model.PRODID);

                        foreach (var imageitem in images)
                        {
                            if (!String.IsNullOrWhiteSpace(imageitem.FilePath))
                            {
                                string currentFileName = imageitem.FilePath.Substring(imageitem.FilePath.LastIndexOf("/") + 1);
                                string newfileName = currentFileName.Replace(oldBUKOD, model.BUKOD);

                                var prouctimage = new ProductImageRepo().GetByID(imageitem.ID);
                                prouctimage.FilePath = imageitem.FilePath.Substring(0, imageitem.FilePath.LastIndexOf("/") + 1) + newfileName;
                                new ProductImageRepo().Update();

                                string sourceFile = HttpContext.Current.Server.MapPath(imageitem.FilePath);
                                System.IO.FileInfo fi = new System.IO.FileInfo(sourceFile);
                                if (fi.Exists)
                                {
                                    fi.MoveTo(HttpContext.Current.Server.MapPath(prouctimage.FilePath));
                                    //Console.WriteLine("File Renamed.");
                                }
                            }
                        }
                        #endregion

                        #region oem
                        var oems = new OEMRepo().GetAll(q => q.PRODID == model.PRODID);
                        foreach (var oemitem in oems)
                        {
                            oemitem.BUKOD = model.BUKOD.Trim();
                        }
                        new OEMRepo().Update();
                        #endregion

                    }

                    #endregion
                }

                #region ilgili ürünler kayıt

                var existrows = new RelatedProductRepo().GetAll(q => q.ProductID == model.PRODID).ToList();
                var notexists = existrows.Where(q => !model.RelatedProducts.Where(es => es.ID == q.ID).Any()).ToList();
                var exists = model.RelatedProducts.Where(q => existrows.Where(es => es.ID == q.ID).Any()).ToList();

                foreach (var delitem in notexists)
                {
                    var deletingrow = new RelatedProductRepo().GetByID(delitem.ID);
                    new RelatedProductRepo().Delete(deletingrow);
                }

                foreach (var upditem in exists)
                {
                    var updrow = new RelatedProductRepo().GetByID(upditem.ID ?? 0);
                    updrow.RankNumber = upditem.RankNumber;
                    updrow.Quantity = upditem.Quantity;
                    new RelatedProductRepo().Update();
                }

                foreach (var insitem in model.RelatedProducts.Where(q => q.ID < 0).ToList())
                {
                    new RelatedProductRepo().Insert(new RelatedProduct()
                    {
                        ProductID = model.PRODID,
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        Quantity = insitem.Quantity,
                        RankNumber = insitem.RankNumber,
                        RelatedID = insitem.RelatedID
                    });
                }
                #endregion

                #region bundle ürünler kayıt

                var existrows2 = new ProductBundleRepo().GetAll(q => q.ProductID == model.PRODID).ToList();
                var notexists2 = existrows2.Where(q => !model.BundledProducts.Where(es => es.ID == q.ID).Any()).ToList();
                var exists2 = model.BundledProducts.Where(q => existrows2.Where(es => es.ID == q.ID).Any()).ToList();

                foreach (var delitem in notexists2)
                {
                    var deletingrow = new ProductBundleRepo().GetByID(delitem.ID);
                    new ProductBundleRepo().Delete(deletingrow);
                }

                foreach (var upditem in exists2)
                {
                    var updrow = new ProductBundleRepo().GetByID(upditem.ID ?? 0);
                    updrow.RankNumber = upditem.RankNumber;
                    updrow.Quantity = upditem.Quantity;
                    new ProductBundleRepo().Update();
                }

                foreach (var insitem in model.BundledProducts.Where(q => q.ID < 0).ToList())
                {
                    new ProductBundleRepo().Insert(new ProductBundle()
                    {
                        ProductID = model.PRODID,
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        Quantity = insitem.Quantity,
                        RankNumber = insitem.RankNumber,
                        RelatedID = insitem.RelatedID
                    });
                }
                #endregion

                var data = new ProductRepo().GetAll(q => q.PRODID == model.PRODID).Select(s => new
                {
                    s.PRODID,
                    RESIM = s.ProductImage.Where(pm => pm.ItemType == 1).ToList().OrderBy(o => o.CreatedOn).FirstOrDefault()?.FilePath,
                    s.BUKOD,
                    SectionName = s.SECTIONS?.NAME,
                    CatName = s.CATEGORIES?.NAME,
                    MCatName = s.MAINCATEGORIES?.NAME,
                    ProductName = s.NAME,
                    ADDED = s.ADDED,
                    s.ENABLED
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
        public IHttpActionResult SaveImage(ProductSaveImageDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.ProductID == null)
                return BadRequest("Ürün kaydına ulaşılamadı.");

            if (model.ItemType != 1 && model.ItemType != 2)
                return BadRequest("Geçersiz resim tipi.");

            var product = new ProductRepo().GetByID(model.ProductID ?? 0);

            if (product == null)
                return BadRequest("Ürün kaydına ulaşılamadı.");

            Regex reg = new Regex("[*'\",_&#^@]");
            string successMessage = String.Empty;

            string filename = string.Empty;

            string fileextension = String.Empty;

            if (!String.IsNullOrWhiteSpace(model.FileName))
            {
                int index_point = model.FileName.LastIndexOf(".") + 1;
                fileextension = model.FileName.Substring(index_point);
            }

            try
            {
                if (model.ID == null)
                {
                    if (String.IsNullOrWhiteSpace(model.ProductImageData))
                        return BadRequest("Resim yüklenemedi.");

                    int newitemindex = (new ProductImageRepo().GetAll(q => q.ProductID == model.ProductID && q.ItemType == model.ItemType).OrderByDescending(o => o.ItemIndex).FirstOrDefault()?.ItemIndex ?? 0) + 1;

                    if (!String.IsNullOrWhiteSpace(model.ProductImageData))
                    {
                        filename = "p" + newitemindex.ToString() + "_" + reg.Replace(product.BUKOD + "." + fileextension, string.Empty);

                        #region save image 
                        string folder = model.ItemType == 1 ? "/upload/pimages/" : "/upload/trimages/";
                        string imagefilemappath = Path.Combine(HttpContext.Current.Server.MapPath(folder), filename);
                        filename = Path.Combine(folder, filename);

                        using (FileStream fs = new FileStream(imagefilemappath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] data = Convert.FromBase64String(model.ProductImageData);
                                bw.Write(data);
                                bw.Close();
                            }
                        }
                        #endregion

                        if (!System.IO.File.Exists(imagefilemappath))
                            return BadRequest("Resim kaydedilirken bir hata oluştu.");
                    }


                    #region new item       
                    successMessage = "Resim eklendi.";

                    var item = new ProductImage()
                    {
                        FilePath = filename,
                        IsDeleted = false,
                        ItemIndex = newitemindex,
                        ItemType = model.ItemType,
                        RankNumber = model.RankNumber,
                        ThumbnailPath = "",
                        Title = model.Title,
                        Description = model.Description,
                        ProductID = model.ProductID,
                        CreatedOn = DateTime.Now,
                        CreatedBy = HttpContext.Current.User.Identity.GetUserId()
                    };
                    new ProductImageRepo().Insert(item);

                    model.ID = item.ID;
                    #endregion
                }
                else
                {
                    successMessage = "Ürün Resim güncellendi.";

                    var productimage = new ProductImageRepo().GetByID(model.ID ?? 0);

                    if (productimage == null)
                        return BadRequest("Ürün-resim kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                    filename = productimage.FilePath;

                    if (!String.IsNullOrWhiteSpace(model.ProductImageData))
                    {
                        #region save image 
                        string folder = model.ItemType == 1 ? "/upload/pimages/" : "/upload/trimages/";
                        string filemappath = HttpContext.Current.Server.MapPath(filename);
                        filename = Path.Combine(folder, filename);

                        using (FileStream fs = new FileStream(filemappath, FileMode.Truncate))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] data = Convert.FromBase64String(model.ProductImageData);
                                bw.Write(data);
                                bw.Close();
                            }
                        }
                        #endregion

                        if (!System.IO.File.Exists(filemappath))
                            return BadRequest("Resim kaydedilirken bir hata oluştu.");
                    }

                    productimage.RankNumber = model.RankNumber;
                    productimage.Title = model.Title;
                    productimage.Description = model.Description;
                    productimage.UpdatedOn = DateTime.Now;
                    productimage.UpdatedBy = HttpContext.Current.User.Identity.GetUserId();

                    new ProductImageRepo().Update();
                }

                var productimagelist = new ProductImageRepo().GetAll(q => q.ProductID == model.ProductID && q.ItemType == 1 && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.ID, s.Title, CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName, s.CreatedOn, s.Description, FileName = s.FilePath, FilePath = s.FilePath + "?t=" + DateTime.Now.Ticks.ToString(), s.ProductID, s.ItemType, s.RankNumber, s.ThumbnailPath });
                var producttimagelist = new ProductImageRepo().GetAll(q => q.ProductID == model.ProductID && q.ItemType == 2 && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.ID, s.Title, CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName, s.CreatedOn, s.Description, FileName = s.FilePath, FilePath = s.FilePath + "?t=" + DateTime.Now.Ticks.ToString(), s.ProductID, s.ItemType, s.RankNumber, s.ThumbnailPath });

                return Ok(new
                {
                    ID = model.ID ?? 0,
                    Message = successMessage,
                    ProductImageList = productimagelist,
                    ProductTImageList = producttimagelist
                });
            }
            catch (Exception ex)
            {
                // todo: add to log 
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteImage(ProductDeleteImageDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Resim silindi.";

                var productimage = new ProductImageRepo().GetByID(model.ID);
                if (productimage == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                productimage.IsDeleted = true;
                productimage.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                productimage.DeletedOn = DateTime.Now;
                new ProductImageRepo().Update();



                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
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
        public IHttpActionResult ExcelImportFillAllCmb()
        {
            try
            {
                var productsections = new ProductSectionRepo().GetAll(q => q.ENABLED == true).Select(s => new { s.SECID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();
                var productmaincategories = new ProductMainCategoryRepo().GetAll(q => q.ENABLED == true).Select(s => new { s.SECID, s.MCATID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();
                var productcategories = new ProductCategoryRepo().GetAll(q => q.ENABLED == true).Select(s => new { s.CATID, s.SECID, s.MCATID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();

                return Ok(new
                {
                    ProductSections = productsections,
                    ProductMainCategories = productmaincategories,
                    ProductCategories = productcategories,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult ImportExcelUploadData(ImportExcelUploadDataDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                if (model.Cats == null)
                    return BadRequest("Kategori seçimi yapmalısınız.");

                if (model.Data == null)
                    return BadRequest("Data bulunamadı.");

                if (model.Data.Count() == 0)
                    return BadRequest("Data bulunamadı.");

                Result retval = new ProductRepo().ImportExcelData(model);

                if (!retval.IsSuccess)
                    return BadRequest(retval.Message);

                return Ok(retval);
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult UploadProfileImage(UploadProfileImageDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası.");

                if (
                    string.IsNullOrWhiteSpace(model.ID) ||
                    string.IsNullOrWhiteSpace(model.FileName) ||
                    string.IsNullOrWhiteSpace(model.imgData)
                    )
                    return BadRequest("Parametre hatası.");


                var member = new MemberRepo().GetByID(model.ID);
                if (member == null)
                    return BadRequest("Personel kaydı bulunamadı.");


                #region save image 

                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/")))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/"));


                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/"), model.FileName);

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(model.imgData);
                        bw.Write(data);
                        bw.Close();
                    }
                }
                #endregion

                member.PhotoPath = Path.Combine("/FileUpload/ProfileImg/" + model.ID + "/", model.FileName);
                new MemberRepo().Update();

                return Ok("Resim yükleme işlemi tamamlandı.");
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu.");
            }

        }



        [HttpPost]
        public IHttpActionResult OEMList(ProductOEMListDTO model)
        {
            try
            {
                var oemlist = new OEMRepo().GetAll(q => q.PRODID == model.PRODID && (q.IsDeleted ?? false) == false)
                       .Select(s => new
                       {
                           s.OEMID,
                           s.BUKOD,
                           s.PRODID,
                           s.OEMNR,
                           s.SUPID,
                           s.OEMSUPNAME?.SUPNAME
                       });

                return Ok(new { OEMList = oemlist });
            }
            catch (Exception)
            {
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }

        }

        [HttpPost]
        public IHttpActionResult SaveOEM(ProductOEMSaveDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                if (model.PRODID == null)
                    return BadRequest("");

                var otherProducts = new OEMRepo().GetAll(q => q.OEMNR == model.OEMNR && q.PRODID != model.PRODID)
                    .Select(s => s.PRODUCTS.BUKOD);

                //return Ok(new { ID = model.OEMID, Message = "", OtherProducts = otherProducts });


                string successMessage = String.Empty;

                if (model.OEMID == null)
                {
                    #region new item       
                    successMessage = "OEM eklendi.";

                    var item = new OEM()
                    {
                        PRODID = model.PRODID,
                        BUKOD = model.BUKOD,
                        OEMNR = model.OEMNR,
                        SUPID = model.SUPID,
                        SUPNAME = model.SUPID.ToString(),
                        CreatedBy = HttpContext.Current.User.Identity.GetUserId(),
                        CreatedOn = DateTime.Now
                    };
                    new OEMRepo().Insert(item);

                    model.OEMID = item.OEMID;
                    #endregion
                }
                else
                {
                    successMessage = "OEM güncellendi.";

                    var oem = new OEMRepo().GetByID(model.OEMID ?? 0);

                    if (oem == null)
                        return BadRequest("Kayıt yapılırken OEM Kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                    oem.OEMNR = model.OEMNR;
                    oem.SUPID = model.SUPID;
                    oem.SUPNAME = model.SUPID.ToString();

                    new OEMRepo().Update();
                }

                return Ok(new { ID = model.OEMID, Message = successMessage, OtherProducts = otherProducts, Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteOEM(ProductOEMDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "OEM silindi.";

                var oem = new OEMRepo().GetByID(model.OEMID ?? 0);
                if (oem == null)
                    return BadRequest("Silme işlemi yapılırken OEM Kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                oem.IsDeleted = true;
                oem.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                oem.DeletedOn = DateTime.Now;
                new OEMRepo().Update();

                return Ok(new { ID = model.OEMID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }



        [HttpPost]
        public IHttpActionResult MUList(ProductMUListDTO model)
        {
            try
            {
                var mulist = new MProductsRepo().GetAll(q => q.ProductID == model.PRODID && (q.IsDeleted ?? false) == false)
                       .Select(s => new
                       {
                           s.MPID,
                           s.MusteriID,
                           s.MUSTERILER?.FIRMA_ADI,
                           s.ProductID,
                           s.BUKOD,
                           s.XPSUP,
                           s.NAME,
                           s.EDITOR_TABLE,
                           s.NAME_EN,
                           s.NAME_DE,
                           s.PRICE,
                           s.CURRENCY,
                           CurrencyCode = s.Currency1?.Code,
                           s.XPSNO,
                           s.BUESKI,
                           s.OEM,
                           ADDED = s.ADDED != null ? s.ADDED.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                           UPDATED = s.UPDATED != null ? s.UPDATED.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                           ADDEDBY = s.Member?.FirstName + " " + s.Member?.LastName,
                           UPDATEDBY = s.Member1?.FirstName + " " + s.Member1?.LastName
                           //s.UpdatesUser
                       });

                return Ok(new { MUList = mulist });
            }
            catch (Exception)
            {
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }

        }

        [HttpPost]
        public IHttpActionResult SaveMU(ProductMUSaveDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string userID = HttpContext.Current.User.Identity.GetUserId();

                string successMessage = String.Empty;

                if (model.MusteriID == null)
                    return BadRequest("Müşteri bilgisi tanımsız!");



                if (String.IsNullOrWhiteSpace(model.XPSNO) && String.IsNullOrWhiteSpace(model.BUKOD))
                    return BadRequest("Müşteri Numarası ve ya BU Numarası girilmelidir!");

                model.XPSNO = String.IsNullOrWhiteSpace(model.XPSNO.Trim()) ? null : model.XPSNO.Trim();
                model.BUKOD = String.IsNullOrWhiteSpace(model.BUKOD.Trim()) ? null : model.BUKOD.Trim();

                if (model.MPID == 0)
                {
                    if (!String.IsNullOrWhiteSpace(model.XPSNO))
                    {
                        var exists = new MProductsRepo().GetByFilter(q => q.MusteriID == model.MusteriID && q.XPSNO == model.XPSNO);
                        if (exists != null)
                            return BadRequest("Müşteri-Ürün Numarası bu müşteri için zaten tanımlı!");
                    }

                    if (!String.IsNullOrWhiteSpace(model.BUKOD))
                    {
                        var exists = new MProductsRepo().GetByFilter(q => q.MusteriID == model.MusteriID && q.BUKOD == model.BUKOD);
                        if (exists != null)
                            return BadRequest("BU Numarası bu müşteri için zaten tanımlı!");
                    }

                    #region new item       
                    successMessage = "Ürün Müşteri kaydı eklendi.";

                    var item = new MPRODUCTS()
                    {
                        MusteriID = model.MusteriID ?? 0,
                        ProductID = model.ProductID,
                        BUKOD = model.BUKOD,
                        XPSUP = model.MusteriID.ToString(), //model.XPSUP,
                        NAME = model.NAME,
                        EDITOR_TABLE = model.EDITOR_TABLE,
                        NAME_EN = model.NAME_EN,
                        NAME_DE = model.NAME_DE,
                        PRICE = model.PRICE,
                        CURRENCY = model.CURRENCY,
                        XPSNO = model.XPSNO,
                        BUESKI = model.BUESKI,
                        ADDED = DateTime.Now,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID
                    };
                    new MProductsRepo().Insert(item);

                    model.MPID = item.MPID;
                    #endregion
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(model.XPSNO))
                    {
                        var exists = new MProductsRepo().GetByFilter(q => q.MPID != model.MPID && q.MusteriID == model.MusteriID && q.XPSNO == model.XPSNO);
                        if (exists != null)
                            return BadRequest("Müşteri-Ürün Numarası bu müşteri için zaten tanımlı!");
                    }

                    if (!String.IsNullOrWhiteSpace(model.BUKOD))
                    {
                        var exists = new MProductsRepo().GetByFilter(q => q.MPID != model.MPID && q.MusteriID == model.MusteriID && q.BUKOD == model.BUKOD);
                        if (exists != null)
                            return BadRequest("BU Numarası bu müşteri için zaten tanımlı!");
                    }

                    successMessage = "Ürün Müşteri kaydı güncellendi.";

                    //if (!String.IsNullOrWhiteSpace(model.XPSNO.Trim()))
                    //{
                    //    var exists = new MProductsRepo().GetByFilter(q => q.XPSNO == model.XPSNO);
                    //    if (exists != null)
                    //        return BadRequest("Müşteri No daha önceden kayıtlı olduğundan kayıt işlemi yapılamadı!");
                    //}

                    var mproduct = new MProductsRepo().GetByID(model.MPID);

                    if (mproduct == null)
                        return BadRequest("Kayıt yapılırken ürün-müşteri kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                    mproduct.MusteriID = model.MusteriID ?? 0;
                    //mproduct.ProductID = model.ProductID;
                    //mproduct.BUKOD = model.BUKOD;
                    mproduct.XPSUP = model.MusteriID.ToString(); // model.XPSUP;
                    mproduct.NAME = model.NAME;
                    mproduct.EDITOR_TABLE = model.EDITOR_TABLE;
                    mproduct.NAME_EN = model.NAME_EN;
                    mproduct.NAME_DE = model.NAME_DE;
                    mproduct.PRICE = model.PRICE;
                    mproduct.CURRENCY = model.CURRENCY;
                    mproduct.XPSNO = model.XPSNO;
                    mproduct.BUESKI = model.BUESKI;
                    mproduct.UPDATED = DateTime.Now;
                    mproduct.UpdatedBy = userID;

                    new MProductsRepo().Update();
                }

                var savedBy = new MemberRepo().GetByFilter(q => q.ID == userID);

                return Ok(new { ID = model.MPID, Message = successMessage, SavedBy = savedBy?.FirstName + " " + savedBy?.LastName, Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteMU(ProductMUDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Üründen müşteri silindi.";

                var mproduct = new MProductsRepo().GetByID(model.MPID);
                if (mproduct == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                mproduct.IsDeleted = true;
                mproduct.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                mproduct.DeletedOn = DateTime.Now;
                new MProductsRepo().Update();

                return Ok(new { ID = model.MPID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
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
                           ADDEDBY = s.Member?.FirstName + " " + s.Member?.LastName,
                           UPDATEDBY = s.Member1?.FirstName + " " + s.Member1?.LastName,
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

                string userID = HttpContext.Current.User.Identity.GetUserId();

                if (model.MusteriID == null)
                    return BadRequest("Müşteri bilgisi tanımsız!");

                if (String.IsNullOrWhiteSpace(model.XPSNO) && String.IsNullOrWhiteSpace(model.BUKOD))
                    return BadRequest("Müşteri Numarası ve ya BU Numarası girilmelidir!");

                model.XPSNO = String.IsNullOrWhiteSpace(model.XPSNO.Trim()) ? null : model.XPSNO.Trim();
                model.BUKOD = String.IsNullOrWhiteSpace(model.BUKOD.Trim()) ? null : model.BUKOD.Trim();

                string successMessage = String.Empty;

                if (model.TPID == 0)
                {
                    #region old 
                    //if (!String.IsNullOrWhiteSpace(model.XPSNO))
                    //{
                    //    var exists = new TProductsRepo().GetByFilter(q => q.MusteriID == model.MusteriID && q.XPSNO == model.XPSNO);
                    //    if (exists != null)
                    //        return BadRequest("Tedarikçi-Ürün Numarası bu tedarikçi için zaten tanımlı!");
                    //}

                    //if (!String.IsNullOrWhiteSpace(model.BUKOD))
                    //{
                    //    var exists = new TProductsRepo().GetByFilter(q => q.MusteriID == model.MusteriID && q.BUKOD == model.BUKOD && q.XPSNO == model.XPSNO);
                    //    if (exists != null)
                    //        return BadRequest("BU Numarası bu tedarikçi için zaten tanımlı!");
                    //}
                    #endregion

                    var exists = new TProductsRepo().GetByFilter(q => q.MusteriID == model.MusteriID && q.BUKOD == model.BUKOD && q.XPSNO == model.XPSNO);
                    if (exists != null)
                        return BadRequest("Belirtilen tedarikçi için BU Numarası ve XPSNO kodu zaten tanımlı!");


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
                        CreatedBy = userID
                    };
                    new TProductsRepo().Insert(item);

                    model.TPID = item.TPID;
                    #endregion
                }
                else
                {
                    #region old 
                    //if (!String.IsNullOrWhiteSpace(model.XPSNO))
                    //{
                    //    var exists = new TProductsRepo().GetByFilter(q => q.TPID != model.TPID && q.MusteriID == model.MusteriID && q.XPSNO == model.XPSNO);
                    //    if (exists != null)
                    //        return BadRequest("Tedarikçi-Ürün Numarası bu tedarikçi için zaten tanımlı!");
                    //}

                    //if (!String.IsNullOrWhiteSpace(model.BUKOD))
                    //{
                    //    var exists = new TProductsRepo().GetByFilter(q => q.TPID != model.TPID && q.MusteriID == model.MusteriID && q.BUKOD == model.BUKOD);
                    //    if (exists != null)
                    //        return BadRequest("BU Numarası bu tedarikçi için zaten tanımlı!");
                    //}
                    #endregion

                    var exists = new TProductsRepo().GetByFilter(q => q.TPID != model.TPID && q.MusteriID == model.MusteriID && q.BUKOD == model.BUKOD && q.XPSNO == model.XPSNO);
                    if (exists != null)
                        return BadRequest("Belirtilen tedarikçi için BU Numarası ve XPSNO kodu zaten tanımlı!");

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
                    tproduct.UpdatedBy = userID;

                    new TProductsRepo().Update();
                }

                var savedBy = new MemberRepo().GetByFilter(q => q.ID == userID);

                return Ok(new { ID = model.TPID, Message = successMessage, SavedBy = savedBy?.FirstName + " " + savedBy?.LastName, Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
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