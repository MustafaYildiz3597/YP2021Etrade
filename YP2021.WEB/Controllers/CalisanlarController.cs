using Nero2021.BLL.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nero2021.Controllers
{
    public class CalisanlarController : BaseController
    {
        // GET: Calisanlar
        public ActionResult Index(string id)
        {
            return View();
        }

        // GET: Calisanlar/Detay
        public ActionResult Detay(string id)
        {
            return View();
        }


    }
}
