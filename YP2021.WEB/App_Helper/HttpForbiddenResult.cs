using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Nero2021
{
    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            base.ExecuteResult(context);

            // creates the ViewResult adding ViewData and TempData parameters
            ViewResult result = new ViewResult
            {
                ViewName = "AccessDenied",
                ViewData = context.Controller.ViewData,
                TempData = context.Controller.TempData
            };

            result.ExecuteResult(context);
        }

        // calls the base constructor with 403 status code
        public HttpForbiddenResult()
            : base(HttpStatusCode.Forbidden, "Forbidden")
        {
        }
    }
}