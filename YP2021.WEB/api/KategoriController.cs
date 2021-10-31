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
 
namespace Nero.Web.api
{
    //[Authorize]
    public class KategoriController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList(KategoriListDTDTO model)
        {
            try
            {
                return Ok(new CATEGORIESRepo().ListDT(model));
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult FillAllCmb()
        {
            try
            {
                var productsections = new ProductSectionRepo().GetAll(q => (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.SECID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();
                var productmaincategories = new ProductMainCategoryRepo().GetAll(q => q.ENABLED == true)
                    .Select(s => new { s.SECID, s.MCATID, s.NAME }).OrderBy(o => o.NAME).AsQueryable();

                return Ok(new
                {
                    ProductSections = productsections,
                    ProductMainCategories = productmaincategories
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Detail(KategoriDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var category = new CATEGORIESRepo().GetAll(q => q.CATID == model.CATID && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.CATID, s.MCATID, s.SECID, SECNAME = s.SECTIONS?.NAME, MCATNAME = s.MAINCATEGORIES?.NAME, s.NAME, s.NAME_DE, s.NAME_EN, s.NAME_FR, s.ENABLED, s.SORT }).FirstOrDefault();

                return Ok(new
                {
                    Category = category,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(KategoriDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Kayıt silindi.";

                var category = new CATEGORIESRepo().GetByID(model.CATID);
                if (category == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                category.IsDeleted = true;
                category.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                category.DeletedOn = DateTime.Now;
                new CATEGORIESRepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(KategoriSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.CATID == null)
                {
                    var checkkategori = new CATEGORIESRepo().GetByFilter(q => q.MCATID == model.MCATID && q.NAME.ToLower() == model.NAME.ToLower() && (q.IsDeleted ?? false) == false);

                    if (checkkategori != null)
                        return BadRequest("Eklemek istediğiniz kategori adı zaten tanımlı.");

                    #region new item       
                    successMessage = "Kategori eklendi.";

                    var item = new CATEGORIES()
                    {
                        SECID = model.SECID ?? 0,
                        MCATID = model.MCATID ?? 0,
                        NAME = model.NAME,
                        NAME_EN = model.NAME_EN,
                        NAME_DE = model.NAME_DE,
                        NAME_FR = model.NAME_FR,
                        ENABLED = model.ENABLED ?? false,
                        SORT = model.SORT,
                        ADDED = DateTime.Now,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new CATEGORIESRepo().Insert(item);

                    model.CATID = item.CATID;
                    #endregion
                }
                else
                {
                    var checkkategori = new CATEGORIESRepo().GetByFilter(q => q.MCATID == model.MCATID && q.CATID != model.CATID && q.NAME.ToLower() == model.NAME.ToLower() && (q.IsDeleted ?? false) == false);

                    if (checkkategori != null)
                        return BadRequest("Değiştirdiğiniz kategori adı zaten tanımlı.");

                    #region update item
                    successMessage = "Kategori güncellendi.";

                    var item = new CATEGORIESRepo().GetByID(model.CATID ?? 0);
                    if (item == null)
                        return BadRequest("Kayda ulaşılamadı.");

                    item.SECID = model.SECID ?? 0;
                    item.MCATID = model.MCATID ?? 0;
                    item.NAME = model.NAME;
                    item.NAME_EN = model.NAME_EN;
                    item.NAME_DE = model.NAME_DE;
                    item.NAME_FR = model.NAME_FR;
                    item.ENABLED = model.ENABLED ?? false;
                    item.SORT = model.SORT;
                    item.UPDATED = DateTime.Now;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new CATEGORIESRepo().Update();
                    #endregion
                }

                var data = new CATEGORIESRepo().GetAll(q => q.CATID == model.CATID)
                    .Select(s => new { SECNAME = s.SECTIONS?.NAME, MCATNAME = s.MAINCATEGORIES?.NAME }).FirstOrDefault();

                return Ok(new { ID = model.CATID, data, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
    }
}