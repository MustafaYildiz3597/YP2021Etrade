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
    public class UlkeController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList()
        {
            try
            {
                return Ok(new UlkelerRepo().ListDT());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }
  
        [HttpPost]
        public IHttpActionResult Detail(UlkeDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var ulke = new UlkelerRepo().GetAll(q => q.UID == model.UID && (q.IsDeleted ?? false) == false).Select(s => new { s.UID, s.UNAME }).FirstOrDefault();

                return Ok(new
                {
                    Ulke = ulke,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(UlkeDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Kayıt silindi.";

                var ulke = new UlkelerRepo().GetByID(model.UID);
                if (ulke == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                ulke.IsDeleted = true;
                ulke.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                ulke.DeletedOn = DateTime.Now;
                new UlkelerRepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(UlkeSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.UID == null)
                {
                    #region new item       

                    successMessage = "Ülke eklendi.";

                    var item = new ULKELER()
                    {
                        UNAME = model.UNAME,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new UlkelerRepo().Insert(item);

                    model.UID = item.UID;
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Ülke güncellendi.";

                    var item = new UlkelerRepo().GetByID(model.UID ?? 0);
                    if (item == null)
                        return BadRequest("Kayda ulaşılamadı.");

                    item.UNAME = model.UNAME;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new UlkelerRepo().Update();
                    #endregion
                }

                return Ok(new { ID = model.UID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
          
         
    }
}