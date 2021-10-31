using Nero2021.BLL.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nero2021.Controllers
{
    [Authorize]
    public class B2bController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Fiyatlandirma");
        }

        // GET: b2b
        public ActionResult Fiyatlandirma()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "b2bpricing" && q.IsEnabled == true);
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

            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //ViewBag.Title = "Kinder Çekiliş";
            //return View("NotStarted");
            ViewBag.Title = "NERO - B2B Fiyatlandırma";

            ViewBag.View = userpermissions.ViewPermission;
            ViewBag.Add = userpermissions.AddPermission;
            ViewBag.Update = userpermissions.UpdatePermission;
            ViewBag.Delete = userpermissions.DeletePermission;

            return View();
        }


        public ActionResult Stoklar()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "b2bstocks" && q.IsEnabled == true);
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

            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //ViewBag.Title = "Kinder Çekiliş";
            //return View("NotStarted");
            ViewBag.Title = "NERO - B2B Stoklar";

            ViewBag.View = userpermissions.ViewPermission;
            ViewBag.Add = userpermissions.AddPermission;
            ViewBag.Update = userpermissions.UpdatePermission;
            ViewBag.Delete = userpermissions.DeletePermission;

            return View();
        }
    }
}
