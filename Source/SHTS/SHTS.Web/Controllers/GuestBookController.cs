using System.Web.Mvc;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Controllers
{
    public class GuestBookController : PCBaseController
    {
        public ActionResult Index()
        {
            GuestBookViewModel model = new GuestBookViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(GuestBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Success");
            }

            return View(model);
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}
