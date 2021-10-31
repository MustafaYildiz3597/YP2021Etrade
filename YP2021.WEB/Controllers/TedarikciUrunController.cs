using Nero2021.BLL.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Nero2021.Controllers
{
    [Authorize] 
    public class TedarikciUrunController : BaseController
    {
        // GET: Index
        public ActionResult Index()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "tedarikciurun" && q.IsEnabled == true);
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
            ViewBag.Title = "NERO -Tedarikçi Ürünleri";

            ViewBag.View = userpermissions.ViewPermission;
            ViewBag.Add = userpermissions.AddPermission;
            ViewBag.Update = userpermissions.UpdatePermission;
            ViewBag.Delete = userpermissions.DeletePermission;

            return View();
        }

        // GET: VeriYukle
        public ActionResult VeriYukle()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "tedarikciurunexceldenyukle" && q.IsEnabled == true);
            if (action == null)
            {
                ViewBag.Message = "Sayfa aktif olmadığından giriş yapılamadı!";
                return View("~/Views/Error/Page404.cshtml");
            }

            var userpermissions = AppHelper.GetUserPermissions().Where(q => q.ID == action.ID).FirstOrDefault();
            if (userpermissions == null)
                return new HttpForbiddenResult();

            if (userpermissions.ExecutePermission != 1)
                return new HttpForbiddenResult();
            #endregion

            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //string IdentityUserID = User.Identity.GetUserId();
            //ViewBag.FirmID = new MemberRepo().GetByID(IdentityUserID)?.FirmID.ToString();

            ViewBag.Title = "NERO - TEdarikçi Ürünleri / Excelden Yükle";
            return View();
        }



    }
}
