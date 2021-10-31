using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web.Http;

namespace Nero.Web.api
{
    //[Authorize]
    public class OEMController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ImportExcelUploadData(OEMImportExcelUploadDataDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");
                
                if (model.Data == null)
                    return BadRequest("Data bulunamadı.");

                if (model.Data.Count() == 0)
                    return BadRequest("Data bulunamadı.");

                Result retval = new OEMRepo().ImportExcelData(model);

                return Ok(retval);
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

         

    }
}