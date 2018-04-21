using System;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class SubscribeController : AdminBaseController
    {
        protected const string USERINFO = "userinfo";
        private DemandSubscriptionManager subscriptionManager = new DemandSubscriptionManager();

        [Permission(EnumRole.Normal)]
        public ActionResult Subscription(string id)
        {
            SubscriptionModel model = new SubscriptionModel();

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;


            model.Subscriptions.AddRange(subscriptionManager.GetSubscriptions(15, page, out allCount));
            
            //分页
            if (model.Subscriptions != null && model.Subscriptions.Count > 0)
            {
                model.PageIndex = page;//当前页数
                model.PageSize = 15;//每页显示多少条
                model.PageStep = 10;//每页显示多少页码
                model.AllCount = allCount;//总条数
                if (model.AllCount % model.PageSize == 0)
                {
                    model.PageCount = model.AllCount / model.PageSize;
                }
                else
                {
                    model.PageCount = model.AllCount / model.PageSize + 1;
                }
            }

            return View(model);
        }

        [Permission(EnumRole.Normal)]
        public ActionResult PushHistory(string id)
        {
            SubscriptionModel model = new SubscriptionModel();

            //页码，总数重置
            int page = 1;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out page);
            }
            int allCount = 0;


            model.PushHistories.AddRange(subscriptionManager.GetSubscriptionPushHistories(15, page, out allCount));

            //分页
            if (model.Subscriptions != null && model.Subscriptions.Count > 0)
            {
                model.PageIndex = page;//当前页数
                model.PageSize = 15;//每页显示多少条
                model.PageStep = 10;//每页显示多少页码
                model.AllCount = allCount;//总条数
                if (model.AllCount % model.PageSize == 0)
                {
                    model.PageCount = model.AllCount / model.PageSize;
                }
                else
                {
                    model.PageCount = model.AllCount / model.PageSize + 1;
                }
            }

            return View(model);
        }
    }
}
