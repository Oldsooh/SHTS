using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Common;
using Witbird.SHTS.Web.Areas.Wechat.Models;
using WitBird.Common;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class QuoteController : WechatBaseController
    {
        DemandQuoteService quoteService = new DemandQuoteService();
        DemandService demandService = new DemandService();
        UserService userService = new UserService();

        //
        // GET: /Wechat/Qoute/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Quote(int demandId)
        {
            Demand demand = demandService.GetDemandById(demandId);
            if (demand.IsNotNull() && demand.UserId != (CurrentWeChatUser.UserId ?? -1))
            {
                var quote = quoteService.SelectDemandQuoteByDemandIdAndWeChatUserId(demand.Id, CurrentWeChatUser.Id);
                if (quote.IsNotNull())
                {
                    demand.QuoteEntities.Add(quote);
                }
            }
            return View(demand);
        }

        [HttpPost]
        public ActionResult Quote(int demandId, string contactName, string contactTitle, //string contactPhone,
            decimal quotePrice, string quoteDetail)
        {
            string errorMessage = string.Empty;
            int quoteId = 0;

            try
            {
                // Check parameters.
                if (string.IsNullOrWhiteSpace(contactName) ||
                    //string.IsNullOrWhiteSpace(contactPhone) ||
                    string.IsNullOrWhiteSpace(quoteDetail))
                {
                    errorMessage = "请输入您的联系方式及报价细则！";
                }
                else if (quotePrice < 0)
                {
                    errorMessage = "请输入正确的报价金额或报名人数！";
                }
                else
                {
                    var demand = demandService.GetDemandById(demandId);

                    if (demand.IsNotNull())
                    {
                        contactTitle = contactTitle.Equals("2", StringComparison.InvariantCultureIgnoreCase) ? "女士" : "先生";
                        DemandQuote quote = new DemandQuote()
                        {
                            ContactName = contactName + contactTitle,
                            ContactPhoneNumber = string.Empty,//contactPhone,
                            DemandId = demandId,
                            WeChatUserId = CurrentWeChatUser.Id,
                            QuotePrice = quotePrice
                        };

                        DemandQuoteHistory quoteHistory = new DemandQuoteHistory()
                        {
                            Comments = quoteDetail,
                            Operation = Operation.Add,
                            WeChatUserId = quote.WeChatUserId
                        };

                        quote.QuoteHistories.Add(quoteHistory);

                        // Savas to database.
                        quote = quoteService.NewQuoteRecord(quote);
                        quoteId = quote.QuoteId;

                        // Sends notification to wechat client.
                        var message = quote.ContactName + "报价/报名" + (int)quote.QuotePrice + "元/人";
                        Task.Factory.StartNew(() =>
                        {
                            var toWeChatUser = userService.GetWeChatUser(demand.UserId);
                            if (toWeChatUser.IsNotNull())
                            {
                                SendNotification(message, quote.QuoteId, toWeChatUser.OpenId);
                            }
                        });
                    }
                    else
                    {
                        errorMessage = "需求不存在或已删除！";
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("需求报价失败", ex.ToString());
                errorMessage = "报价失败";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewData["ErrorMessage"] = errorMessage;
                return View();
            }

            return Redirect("/wechat/quote/detail?quoteId=" + quoteId);
        }

        /// <summary>
        /// 我发出的需求报价列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PostedQuotes(int pageIndex = 1)
        {
            DemandQuoteModel model = new DemandQuoteModel();

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            int totalCount = 0;
            var allQuotes = quoteService.GetPostedQuotes(CurrentWeChatUser.Id, 15, pageIndex, out totalCount);

            //分页
            if (allQuotes != null && allQuotes.Count > 0)
            {
                model.Quotes = allQuotes;

                model.PageIndex = pageIndex;//当前页数
                model.PageSize = 15;//每页显示多少条
                model.PageStep = 10;//每页显示多少页码
                model.AllCount = totalCount;//总条数
                if (model.AllCount % model.PageSize == 0)
                {
                    model.PageCount = model.AllCount / model.PageSize;
                }
                else
                {
                    model.PageCount = model.AllCount / model.PageSize + 1;
                }

                model.ActionName = "PostedQuotes";
            }

            return View(model);
        }

        /// <summary>
        /// 我收到的需求报价列表
        /// </summary>
        /// <returns></returns>
        public ActionResult RecievedQuotes(int pageIndex = 1)
        {
            DemandModel model = new DemandModel();

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            int totalCount = 0;
            var demands = quoteService.GetRecievedQuotes(CurrentWeChatUser.Id, 15, pageIndex, out totalCount);

            //分页
            if (demands != null && demands.Count > 0)
            {
                model.Demands = demands;

                model.PageIndex = pageIndex;//当前页数
                model.PageSize = 15;//每页显示多少条
                model.PageStep = 10;//每页显示多少页码
                model.AllCount = totalCount;//总条数
                if (model.AllCount % model.PageSize == 0)
                {
                    model.PageCount = model.AllCount / model.PageSize;
                }
                else
                {
                    model.PageCount = model.AllCount / model.PageSize + 1;
                }

                model.ActionName = "RecievedQuotes";
            }

            return View(model);
        }

        public ActionResult Show(int demandId)
        {
            Demand demand = null;

            try
            {
                demand = demandService.GetDemandById(demandId);

                if (demand.IsNotNull())
                {
                    if (demand.UserId == (CurrentWeChatUser.UserId ?? -1))
                    {
                        var quotes = quoteService.GetAllDemandQuotesForOneDemand(demandId);
                        if (quotes.HasItem())
                        {
                            demand.QuoteEntities.AddRange(quotes);
                        }
                    }
                    else
                    {
                        demand = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("查看需求报价列表", ex.ToString());
            }

            return View(demand);
        }

        public ActionResult Detail(int quoteId)
        {
            DemandQuote quote = quoteService.GetDemandQuote(quoteId, true);
            if (quote.IsNotNull())
            {
                quote.Demand = demandService.GetDemandById(quote.DemandId);

                // 如果是自己发布的需求，无需购买即可查看
                var isPostedByMyself = (quote.Demand.UserId == CurrentWeChatUser.UserId);
                quote.HasWeChatUserBoughtForDemand = isPostedByMyself || demandService.HasWeChatUserBoughtForDemand(CurrentWeChatUser.OpenId, quote.Demand.Id);
                
                // 屏蔽联系方式
                foreach (var history in quote.QuoteHistories)
                {
                    history.Comments = FilterHelper.Filter(FilterLevel.PhoneAndEmail, history.Comments);
                }
            }

            return View(quote);
        }

        public ActionResult QuoteListByType()
        {
            return View();
        }

        public ActionResult UpdateQuoteStatus(int quoteId, string statusId)
        {
            string errorMessage = string.Empty;

            if (!statusId.Equals(DemandQuoteStatus.Denied.ToString(), StringComparison.CurrentCultureIgnoreCase) &&
                !statusId.Equals(DemandQuoteStatus.Accept.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                errorMessage = "检测到非法报价状态值！请刷新页面重新进行操作";
            }
            else
            {
                DemandQuote quote = quoteService.GetDemandQuote(quoteId, false);
                if (!quote.IsNotNull())
                {
                    errorMessage = "报价记录不存在或已被删除！";
                }
                else
                {
                    var message = "您的报价/报名已被";
                    if (statusId.Equals(DemandQuoteStatus.Accept.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        message += "接受！";
                    }
                    else
                    {
                        message += "拒绝！";
                    }

                    //if (statusId.Equals(DemandQuoteStatus.Accept.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    var demandQuotes = quoteService.GetAllDemandQuotesForOneDemand(quote.DemandId);
                    //    var acceptQuote = demandQuotes.FirstOrDefault(x => x.HandleStatus &&
                    //        x.AcceptStatus.Equals(DemandQuoteStatus.Accept.ToString(), StringComparison.CurrentCultureIgnoreCase));

                    //    if (acceptQuote.IsNotNull())
                    //    {
                    //        errorMessage = "对该需求您已采纳了如下报价，请驳回其他报价信息：";
                    //        errorMessage += "\r\n联系人：" + acceptQuote.ContactName;
                    //        errorMessage += "\r\n联系电话：" + acceptQuote.ContactPhoneNumber;
                    //        errorMessage += "\r\n报价金额：" + acceptQuote.QuotePrice + "元";
                    //    }
                    //    else
                    //    {
                    //        var result = quoteService.UpdateAllQuotesStatus(quote.DemandId, quote.QuoteId);
                    //        if (!result)
                    //        {
                    //            errorMessage = "更新报价状态失败，请重新尝试！";
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    quote.HandleStatus = true;
                    quote.AcceptStatus = statusId;
                    quote.QuoteHistories.Add(new DemandQuoteHistory()
                    {
                        QuoteId = quote.QuoteId,
                        Comments = message,
                        WeChatUserId = CurrentWeChatUser.Id,
                        Operation = Operation.Add
                    });

                    quoteService.UpdateQuoteRecord(quote);
                    //}

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        // Sends notification to wechat client.
                        Task.Factory.StartNew(() =>
                        {
                            var toWeChatUser = userService.GetWeChatUserByWeChatUserId(quote.WeChatUserId);
                            if (toWeChatUser.IsNotNull())
                            {
                                SendNotification(message, quote.QuoteId, toWeChatUser.OpenId);
                            }
                        });
                    }
                }
            }

            return Content(errorMessage);
        }

        public ActionResult AddQuoteHistory(int quoteId, string comments)
        {
            string errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(comments))
            {
                errorMessage = "请输入留言内容！";
            }
            else
            {
                DemandQuote quote = quoteService.GetDemandQuote(quoteId, false);
                if (!quote.IsNotNull())
                {
                    errorMessage = "报价记录不存在或已被删除！";
                }
                else
                {
                    var history = new DemandQuoteHistory()
                    {
                        Comments = comments,
                        Operation = Operation.Add,
                        QuoteId = quoteId,
                        WeChatUserId = CurrentWeChatUser.Id
                    };

                    var result = quoteService.NewQuoteHistory(history);

                    if (!result)
                    {
                        errorMessage = "留言消息发送失败!";
                    }
                    else
                    {
                        if (quote.WeChatUserId == CurrentWeChatUser.Id)
                        {
                            // Sends notification to wechat client.
                            Task.Factory.StartNew(() =>
                            {
                                var demand = demandService.GetDemandById(quote.DemandId);

                                if (demand.IsNotNull())
                                {
                                    var toWeChatUser = userService.GetWeChatUser(demand.UserId);
                                    if (toWeChatUser.IsNotNull())
                                    {
                                        SendNotification(comments, quote.QuoteId, toWeChatUser.OpenId);
                                    }
                                }
                            });
                        }
                        else
                        {
                            Task.Factory.StartNew(() =>
                            {
                                var toWeChatUser = userService.GetWeChatUserByWeChatUserId(quote.WeChatUserId);
                                if (toWeChatUser.IsNotNull())
                                {
                                    SendNotification(comments, quote.QuoteId, toWeChatUser.OpenId);
                                }
                            });
                        }
                    }
                }
            }

            return Content(errorMessage);
        }

        private void SendNotification(string message, int quoteId, string openId)
        {
            try
            {
                var viewUrl = "您收到了新的报价/报名提醒:{0}\r\n <a href=\"http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain +
            "/wechat/quote/detail?quoteId={1}\">点击这里，立即查看</a>";

                WeChatClient.Sender.SendText(openId, string.Format(viewUrl, message, quoteId));
            }
            catch (Exception ex)
            {
                LogService.LogWexin("Failed to send notification", ex.ToString());
            }
        }
    }
}
