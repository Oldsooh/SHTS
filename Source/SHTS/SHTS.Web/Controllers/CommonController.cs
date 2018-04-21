using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.User;

namespace Witbird.SHTS.Web.Controllers
{
    public class CommonController : PCBaseController
    {
        //
        // GET: /Common/

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult Header()
        {
            if (CurrentCityName == null)
            {
                CurrentCityId = null;
                CurrentCityName = "全国";
            }
            UserViewModel model = new UserViewModel();
            if (UserInfo != null)
            {
                model.UserEntity = UserInfo;
                model.ErrorMsg = new UserService().CheckUserVipEndTimeWithStatusMessage(UserInfo.UserId);
            }

            return PartialView(model);
        }

        public ActionResult Footer()
        {
            return PartialView();
        }

        public ActionResult Right()
        {
            RightModel model = new RightModel();

            CommonService commonService = new CommonService();
            model.Right = commonService.GetRight();

            return PartialView(model);
        }

        public ActionResult FileUpload()
        {
            return View();
        }

        public ActionResult ErrorAccessDenied()
        {
            return View();
        }

        public ActionResult ErrorPageNotFound()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}
