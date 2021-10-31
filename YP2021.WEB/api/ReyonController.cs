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
//using static TurkBelge.Controllers.PersonelController;

namespace Nero.Web.api
{
    //[Authorize]
    public class ReyonController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList()
        {
            try
            {
                return Ok(new SECTIONSRepo().ListDT());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }
  
        [HttpPost]
        public IHttpActionResult Detail(ReyonDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var reyon = new SECTIONSRepo().GetAll(q => q.SECID == model.SECID && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.SECID, s.NAME, s.NAME_DE, s.NAME_EN, s.NAME_FR, s.ENABLED, s.SORT }).FirstOrDefault();

                return Ok(new
                {
                    Reyon = reyon,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(ReyonDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Kayıt silindi.";

                var reyon = new SECTIONSRepo().GetByID(model.SECID);
                if (reyon == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                reyon.IsDeleted = true;
                reyon.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                reyon.DeletedOn = DateTime.Now;
                new SECTIONSRepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(ReyonSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.SECID == null)
                {
                    var checkreyon = new SECTIONSRepo().GetByFilter(q => q.NAME.ToLower() == model.NAME.ToLower() && (q.IsDeleted ?? false) == false);

                    if (checkreyon != null)
                        return BadRequest("Eklemek istediğiniz reyon adı zaten tanımlı.");

                    #region new item       

                    successMessage = "Reyon eklendi.";

                    var item = new SECTIONS()
                    {
                        NAME = model.NAME,
                        NAME_EN = model.NAME_EN,
                        NAME_DE = model.NAME_DE,
                        NAME_FR = model.NAME_FR,
                        ENABLED = model.ENABLED ?? false,
                        SORT = model.SORT,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new SECTIONSRepo().Insert(item);

                    model.SECID = item.SECID;
                    #endregion
                }
                else
                {
                    var checkreyon = new SECTIONSRepo().GetByFilter(q => q.SECID != model.SECID && q.NAME.ToLower() == model.NAME.ToLower() && (q.IsDeleted ?? false) == false);

                    if (checkreyon != null)
                        return BadRequest("Değiştirdiğiniz reyon adı zaten tanımlı.");

                    #region update item

                    successMessage = "Reyon güncellendi.";

                    var item = new SECTIONSRepo().GetByID(model.SECID ?? 0);
                    if (item == null)
                        return BadRequest("Kayda ulaşılamadı.");

                    item.NAME = model.NAME;
                    item.NAME_EN = model.NAME_EN;
                    item.NAME_DE = model.NAME_DE;
                    item.NAME_FR = model.NAME_FR;
                    item.ENABLED = model.ENABLED ?? false;
                    item.SORT = model.SORT;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new SECTIONSRepo().Update();
                    #endregion
                }

                return Ok(new { ID = model.SECID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
          
         
    }
}