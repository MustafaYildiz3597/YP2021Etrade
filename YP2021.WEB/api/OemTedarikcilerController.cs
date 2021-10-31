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
    public class OemTedarikcilerController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList()
        {
            try
            {
                return Ok(new OEMSUPNAMERepo().ListDT());
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }
  
        [HttpPost]
        public IHttpActionResult Detail(OEMSUPNAMEDetailDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var oemsupname = new OEMSUPNAMERepo().GetAll(q => q.SUPID == model.SUPID && (q.IsDeleted ?? false) == false).Select(s => new { s.SUPID, s.SUPNAME }).FirstOrDefault();

                return Ok(new
                {
                    OEMSupName = oemsupname,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(OEMSUPNAMEDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Kayıt silindi.";

                var oemsupname = new OEMSUPNAMERepo().GetByID(model.SUPID);
                if (oemsupname == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                oemsupname.IsDeleted = true;
                oemsupname.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                oemsupname.DeletedOn = DateTime.Now;
                new OEMSUPNAMERepo().Update();

                return Ok(successMessage);
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult Save(OEMSUPNAMESaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.SUPID == null)
                {
                    #region new item       

                    successMessage = "OEM Tedarikçisi eklendi.";

                    var item = new OEMSUPNAME()
                    {
                        SUPNAME = model.SUPNAME,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new OEMSUPNAMERepo().Insert(item);

                    model.SUPID = item.SUPID;
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "OEM Tedarikçisi güncellendi.";

                    var item = new OEMSUPNAMERepo().GetByID(model.SUPID ?? 0);
                    if (item == null)
                        return BadRequest("Kayda ulaşılamadı.");

                    item.SUPNAME = model.SUPNAME;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;

                    new OEMSUPNAMERepo().Update();
                    #endregion
                }

                return Ok(new { ID = model.SUPID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
          
         
    }
}