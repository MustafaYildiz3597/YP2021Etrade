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
    public class SehirController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList(SehirListDTDTO model)
        {
            try
            {
                return Ok(new SehirlerRepo().ListDT(model));
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
                var countries = new UlkelerRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new { s.UID, s.UNAME }).OrderBy(o => o.UNAME).AsQueryable();

                return Ok(new
                {
                   Countries = countries
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Detail(SehirDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var sehir = new SehirlerRepo().GetAll(q => q.ID == model.ID && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.ID, s.NAME, s.ULKELER?.UID, s.ULKELER?.UNAME }).FirstOrDefault();

                return Ok(new
                {
                    Sehir = sehir
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(SehirDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Kayıt silindi.";

                var sehir = new SehirlerRepo().GetByID(model.ID);
                if (sehir == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                sehir.IsDeleted = true;
                sehir.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                sehir.DeletedOn = DateTime.Now;
                new SehirlerRepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(SehirSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.ID == null)
                {
                    #region new item       

                    successMessage = "Şehir eklendi.";

                    var item = new SEHIRLER()
                    {
                        NAME = model.Name,
                        UID = model.UID ?? 0,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new SehirlerRepo().Insert(item);

                    model.ID = item.ID;
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Şehir güncellendi.";

                    var item = new SehirlerRepo().GetByID(model.ID ?? 0);
                    if (item == null)
                        return BadRequest("Kayda ulaşılamadı.");

                    item.UID = model.UID ?? 0;
                    item.NAME = model.Name;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new SehirlerRepo().Update();
                    #endregion
                }

                var data = new SehirlerRepo().GetAll(q => q.ID == model.ID).Select(s => new { s.ULKELER?.UNAME }).FirstOrDefault();

                return Ok(new { model.ID, data, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
          
         
    }
}