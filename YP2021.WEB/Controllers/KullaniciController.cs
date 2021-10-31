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
   
    public class KullaniciController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("SifreDegistir");
        }

        [Authorize]
        public ActionResult SifreDegistir()
        {
            ViewBag.Id = User.Identity.GetUserId();
            return View();
        }

        
    }
}
