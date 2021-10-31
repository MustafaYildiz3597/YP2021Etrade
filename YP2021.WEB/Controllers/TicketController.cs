using Nero2021.BLL.Repository;
using System.Linq;
using System.Web.Mvc;

namespace Nero2021.Controllers
{

    public class TicketController : BaseController
    {
        [Authorize]
        // GET: ticket
        public ActionResult Index()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "ticket" && q.IsEnabled == true);
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
            ViewBag.Title = "NERO - Ticket";

            ViewBag.View = userpermissions.ViewPermission;
            ViewBag.Add = userpermissions.AddPermission;
            ViewBag.Update = userpermissions.UpdatePermission;
            ViewBag.Delete = userpermissions.DeletePermission;

            var tickettypes = new TicketTypeRepo().GetAll().Select(s => new { s.ID, value = s.Name, label = s.Name }).OrderBy(o => o.ID).AsEnumerable();
            ViewBag.TicketTypes = tickettypes;

            return View();
        }
    }
}
