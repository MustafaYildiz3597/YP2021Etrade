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
    public class UrunlerController : BaseController
    {
        // GET: Personel
        public ActionResult Index()
        {
            #region check authorize
            //var action = new ActionRepo().GetByFilter(q => q.Key == "urunler" && q.IsEnabled == true);
            //if (action == null)
            //{
            //    ViewBag.Message = "Sayfa aktif olmadığından giriş yapılamadı!";
            //    return View("~/Views/Error/Page404.cshtml");
            //}
            //var userpermissions = AppHelper.GetUserPermissions().Where(q => q.ID == action.ID).FirstOrDefault();
            //if (userpermissions == null)
            //    return new HttpForbiddenResult();

            //if (userpermissions.ViewPermission != 1)
            //    return new HttpForbiddenResult();

            //ViewBag.View = userpermissions.ViewPermission;
            //ViewBag.Add = userpermissions.AddPermission;
            //ViewBag.Update = userpermissions.UpdatePermission;
            //ViewBag.Delete = userpermissions.DeletePermission;

            string[] keys = new[] {"urunler", "musteriurun", "tedarikciurun", "urunexceldenyukle" }; 
            var actions = new ActionRepo().GetAll(q => keys.Contains(q.Key) && q.IsEnabled == true).Select(s => new { s.ID });

            var userpermissions = AppHelper.GetUserPermissions().Where(q => actions.Any(a => q.ID == a.ID))
                .Select(s => new {
                    s.Key,
                    s.ID,
                    s.ExecutePermission,
                    s.AddPermission,
                    s.DeletePermission,
                    s.SearchPermission,
                    s.UpdatePermission,
                    s.ViewPermission
                });
            #endregion

            if (userpermissions.Where(q => q.Key == "urunler").FirstOrDefault()?.ViewPermission != 1)
                return new HttpForbiddenResult();

            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //ViewBag.Title = "Kinder Çekiliş";
            //return View("NotStarted");
            ViewBag.Title = "NERO - Ürünler";

            ViewBag.Permissions = userpermissions;
            //ViewBag.Add = userpermissions.AddPermission;
            //ViewBag.Update = userpermissions.UpdatePermission;
            //ViewBag.Delete = userpermissions.DeletePermission;

            return View();
        }

        // GET: urunler/ExceldenYukle
        public ActionResult ExceldenYukle()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "urunexceldenyukle" && q.IsEnabled == true);
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

            ViewBag.Title = "NERO - Ürünler / Excelden Yükle";

            return View("ProductExcelImport");
        }

        // GET: urunler/oemexceldenyukle
        public ActionResult OEMExceldenYukle()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "urunexceldenyukle" && q.IsEnabled == true);
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

            ViewBag.Title = "NERO - Ürünler / OEM Excelden Yükle";

            return View("OEMExcelImport");
        }



    }
}
