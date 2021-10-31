using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nero2021.Controllers
{
    
    public class SifremiUnuttumController : BaseController
    {
        // GET: SifremiUnuttum
        public ActionResult Index()
        {
            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            ViewBag.Title = "TürkBelge - Şifremi Unuttum";

            return View();
        }
    }
}
