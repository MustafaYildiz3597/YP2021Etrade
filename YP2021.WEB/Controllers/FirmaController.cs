using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nero2021.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FirmaController : BaseController
    {
        // GET: firma
        public ActionResult Index()
        {
            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //ViewBag.Title = "Kinder Çekiliş";
            //return View("NotStarted");
            ViewBag.Title = "TürkBelge - Firma";

            return View();
        }
    }
}
