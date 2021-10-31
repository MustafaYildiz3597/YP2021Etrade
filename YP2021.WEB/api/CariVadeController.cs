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
    public class CariVadeController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList()
        {
            try
            {
                return Ok(new CariVadeRepo().ListDT());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }
  
        [HttpPost]
        public IHttpActionResult Detail(CariVadeDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var carivade = new CariVadeRepo().GetAll(q => q.ID == model.ID && (q.IsDeleted ?? false) == false).Select(s => new { s.ID, s.Name, s.IsActive, s.RankNumber }).FirstOrDefault();

                return Ok(new
                {
                    CariVade = carivade,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(CariVadeDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Kayıt silindi.";

                var carivade = new CariVadeRepo().GetByID(model.ID);
                if (carivade == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                carivade.IsDeleted = true;
                carivade.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                carivade.DeletedOn = DateTime.Now;
                new CariVadeRepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(CariVadeSaveDTO model)
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

                    successMessage = "Cari Vade eklendi.";

                    var item = new CariVade()
                    {
                        Name = model.Name,
                        IsActive = model.IsActive,
                        RankNumber = model.RankNumber,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new CariVadeRepo().Insert(item);

                    model.ID = item.ID;
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Cari Vade güncellendi.";

                    var item = new CariVadeRepo().GetByID(model.ID ?? 0);
                    if (item == null)
                        return BadRequest("Kayda ulaşılamadı.");

                    item.Name = model.Name;
                    item.RankNumber = model.RankNumber;
                    item.IsActive = model.IsActive;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new CariVadeRepo().Update();
                    #endregion
                }

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
          
         
    }
}