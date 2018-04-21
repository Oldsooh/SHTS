using System;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Models.ActivityModel;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class ActivityController : WechatBaseController
    {
        //
        // GET: /M/Activity/


        ActivityService service = new ActivityService();

        public ActionResult Index(int page = 1)
        {
            ActivitysViewModel model = new ActivitysViewModel();
            try
            {
                QueryActivityCriteria queryActivityCriteria = new QueryActivityCriteria
                {
                    PageSize = 10,
                    StartRowIndex = page,
                    QueryType = -1
                };
                model.ActivityList = service.QueryActivities(queryActivityCriteria);

                if (model.ActivityList != null)
                {
                    model.TotalCount =
                        queryActivityCriteria.ResultTotalCount;
                }
                model.PageSize = 10;
                model.PageStep = 5;
                model.PageIndex = page;

                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            }
            catch (Exception e)
            {
                LogService.Log("Activitys List 出错了！", e.ToString());
            }
            return View(model);
        }

        public ActionResult Show(int id)
        {
            ShowActivityViewModel model = new ShowActivityViewModel();
            try
            {
                var activity = service.GetActivityById(id);

                model.Activity = activity;
                model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();

                if (activity.IsNotNull())
                {
                    ActivityVote vote = service.SelectActivityVote(activity.Id, CurrentWeChatUser.OpenId);

                    if (vote.IsNotNull())
                    {
                        model.ActivityVoteTotalCount = vote.ActivityTotalVoteCount;
                        model.IsCurrentUserVoted = vote.IsVoted;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("Show Activity出错了！", e.ToString());
            }
            return View(model);
        }

        public ActionResult ShareActivity()
        {
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                return Redirect("/wechat/account/login");
            }
            ShareActivityViewModel model = new ShareActivityViewModel();
            model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();

            ViewData["CurrentWeChatUser"] = CurrentWeChatUser;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ShareActivity(Activity activity)
        {
            ShareActivityViewModel model = new ShareActivityViewModel();
            model.ActivityTypes = (new ActivityTypeManager()).GetAllActivityTypes();
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                model.ErrorMsg = "未登录或登录超时";
                model.ErrorCode = "401";
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(activity.Title) ||
                        string.IsNullOrEmpty(activity.Description) || 
                        string.IsNullOrEmpty(activity.Adress))
                    {
                        model.ErrorMsg = "标题或内容不能为空";
                        model.ErrorCode = "400";
                    }
                    else
                    {
                        activity.UserId = UserInfo.UserId;
                        activity.LocationId = Request.Form["LocationId[]"];
                        activity.State = 3;
                        activity.IsFromMobile = true;
                        //activity.Description = Witbird.SHTS.Web.Public.StaticUtility.FilterSensitivewords(activity.Description);
                        activity.ContentStyle = activity.Description;
                        activity.ContentText = activity.Description;
                        service.CreateOrUpdateActivity(activity);
                        model.ErrorMsg = "发布成功！";
                        model.ErrorCode = "200";
                        return RedirectToAction("Index", "Activity", new { Area = "Wechat" });
                    }
                }
                catch (Exception e)
                {
                    LogService.Log("ShareActivity 出错了！", e.ToString());
                }
            }
            model.Activity = activity;
            return View(model);
        }

        public ActionResult UpdateVoteStatus(int activityId, bool isVoted)
        {
            var vote = new ActivityVote()
            {
                ActivityId = activityId,
                InsertedTimestamp = DateTime.Now,
                IsVoted = isVoted,
                LastUpdatedTimestamp = DateTime.Now,
                UserId = CurrentWeChatUser.UserId,
                WechatUserOpenId = CurrentWeChatUser.OpenId
            };
            var result = service.AddOrUpdateActivityVoteRecord(vote);
            var data = new
            {
                IsSuccessFul = result
            };
            return Json(data);
        }
    }
}
