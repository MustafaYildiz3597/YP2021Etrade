using Microsoft.AspNet.Identity;
using Nero2021.BLL.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Nero2021.Controllers
{
    public class ReyonlarController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "reyonlar" && q.IsEnabled == true);
            if (action == null)
            {
                ViewBag.Message = "Sayfa aktif olmadığından giriş yapılamadı!";
                return View("~/Views/Error/Page404.cshtml");
            }

            var userpermissions = AppHelper.GetUserPermissions().Where(q => q.ID == action.ID).FirstOrDefault();
            if (userpermissions == null)
                return new HttpForbiddenResult();

            if (userpermissions.ViewPermission != 1)
                return new HttpForbiddenResult();
            #endregion

            ViewBag.View = userpermissions.ViewPermission;
            ViewBag.Add = userpermissions.AddPermission;
            ViewBag.Update = userpermissions.UpdatePermission;
            ViewBag.Delete = userpermissions.DeletePermission;

            return View();
        }
         
    }
}
