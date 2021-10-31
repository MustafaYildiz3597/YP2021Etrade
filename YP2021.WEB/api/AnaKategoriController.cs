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
    public class AnaKategoriController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList(AnaKategoriListDTDTO model)
        {
            try
            {
                return Ok(new MAINCATEGORIESRepo().ListDT(model));
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

                return Ok(new
                {
                    ProductSections = productsections
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Detail(AnaKategoriDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var maincategory = new MAINCATEGORIESRepo().GetAll(q => q.MCATID == model.MCATID && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.MCATID, s.SECID, SECNAME = s.SECTIONS?.NAME, s.NAME, s.NAME_DE, s.NAME_EN, s.NAME_FR, s.ENABLED, s.SORT }).FirstOrDefault();

                return Ok(new
                {
                    MainCategory = maincategory,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(AnaKategoriDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Kayıt silindi.";

                var maincategory = new MAINCATEGORIESRepo().GetByID(model.MCATID);
                if (maincategory == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                maincategory.IsDeleted = true;
                maincategory.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                maincategory.DeletedOn = DateTime.Now;
                new MAINCATEGORIESRepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(AnaKategoriSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.MCATID == null)
                {

                    var checkanakategori = new MAINCATEGORIESRepo().GetByFilter(q => q.SECID == model.SECID &&  q.NAME.ToLower() == model.NAME.ToLower() && (q.IsDeleted ?? false) == false);

                    if (checkanakategori != null)
                        return BadRequest("Eklemek istediğiniz ana kategori adı zaten tanımlı.");


                    #region new item       

                    successMessage = "Ana Kategori eklendi.";

                    var item = new MAINCATEGORIES()
                    {
                        SECID = model.SECID ?? 0,
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
                    new MAINCATEGORIESRepo().Insert(item);

                    model.MCATID = item.MCATID;
                    #endregion
                }
                else
                {
                    var checkanakategori = new MAINCATEGORIESRepo().GetByFilter(q => q.SECID == model.SECID && q.MCATID != model.MCATID && q.NAME.ToLower() == model.NAME.ToLower() && (q.IsDeleted ?? false) == false);

                    if (checkanakategori != null)
                        return BadRequest("Değiştirdiğiniz ana kategori adı zaten tanımlı.");

                    #region update item

                    successMessage = "Ana Kategori güncellendi.";

                    var item = new MAINCATEGORIESRepo().GetByID(model.MCATID ?? 0);
                    if (item == null)
                        return BadRequest("Kayda ulaşılamadı.");

                    //item.MCATID = model.MCATID ?? 0;
                    item.SECID = model.SECID ?? 0;
                    item.NAME = model.NAME;
                    item.NAME_EN = model.NAME_EN;
                    item.NAME_DE = model.NAME_DE;
                    item.NAME_FR = model.NAME_FR;
                    item.ENABLED = model.ENABLED ?? false;
                    item.SORT = model.SORT;
                    item.UPDATED = DateTime.Now;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new MAINCATEGORIESRepo().Update();
                    #endregion
                }

                var data = new SECTIONSRepo().GetAll(q => q.SECID == model.SECID).Select(s => new { s.NAME }).FirstOrDefault();

                return Ok(new { ID = model.MCATID, data, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
          
         
    }
}