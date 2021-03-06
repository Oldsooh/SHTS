﻿using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
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
        private ResourceService resourceService = new ResourceService();
        ResourceManager resourceManager = new ResourceManager();

        //
        // GET: /Wechat/Qoute/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Quote(int demandId)
        {
            DemandQuoteModel model = new DemandQuoteModel();

            // Gets demand
            model.Demand = demandService.GetDemandById(demandId);

            // Gets quote history
            if (model.Demand.IsNotNull() && model.Demand.UserId != (CurrentWeChatUser.UserId ?? -1))
            {
                var quote = quoteService.SelectDemandQuoteByDemandIdAndWeChatUserId(model.Demand.Id, CurrentWeChatUser.Id);
                if (quote.IsNotNull())
                {
                    model.Demand.QuoteEntities.Add(quote);
                }
            }

            // Gets posted resouces for current user
            //if (CurrentWeChatUser.IsUserLoggedIn)
            //{
            //    model.PostedResources.AddRange(resourceService
            //        .GetResourceByUser(CurrentWeChatUser.UserId.Value, 0, 1000).Items);
            //}

            return View(model);
        }

        [HttpPost]
        public ActionResult Quote(int demandId, string contactName, string contactTitle, //string contactPhone,
            decimal quotePrice, string quoteDetail, int selectedResourceId = -1)
        {
            string errorMessage = string.Empty;
            int quoteId = 0;

            try
            {
                // Check parameters.
                // 资源信息不是必选的
                //if (selectedResourceId < 1)
                //{
                //    errorMessage = "请选择您参与报价对应的资源信息。如您还未发布资源，请点击【立即发布】按钮。";
                //}
                //else
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

                        // 展示报价放联系信息
                        DemandQuoteHistory quoteHistory = new DemandQuoteHistory()
                        {
                            Comments = quoteDetail,
                            Operation = Operation.Add,
                            WeChatUserId = quote.WeChatUserId
                        };

                        quote.QuoteHistories.Add(quoteHistory);


                        DAL.New.Resource selectedResource = null;
                        if (selectedResourceId > 0)
                        {
                            selectedResource = resourceManager.GetResourceById(selectedResourceId, false, false);

                            //if (selectedResource == null)
                            //{
                            //    errorMessage = "您选择的资源信息不存在或已被删除，请刷新页面后重试";
                            //}

                            if (selectedResource != null)
                            {
                                quote.ResourceId = selectedResource.Id;
                                // 展示报价放选择的资源信息
                                var resourceHistory = new DemandQuoteHistory()
                                {
                                    Comments = $"点击查看报价资源信息：<a href='/resource/show/{selectedResource.Id}'>{selectedResource.Title}</a>",
                                    Operation = Operation.Add,
                                    WeChatUserId = quote.WeChatUserId
                                };
                                quote.QuoteHistories.Add(resourceHistory);
                            }

                        }
                        // Savas to database.
                        quote = quoteService.NewQuoteRecord(quote);
                        quoteId = quote.QuoteId;

                        // Sends notification to wechat client.
                        //var message = quote.ContactName + "报价/报名" + (int)quote.QuotePrice + "元/人";
                        var message = string.Empty;
                        Task.Factory.StartNew(() =>
                        {
                            var toWeChatUser = userService.GetWeChatUser(demand.UserId);
                            if (toWeChatUser.IsNotNull())
                            {
                                var messageData = ConstrunctNewQuoteData(quoteId, CurrentWeChatUser.NickName, quote.InsertedTimestamp, quote.QuotePrice.ToString());
                                SendNotification(quoteId, toWeChatUser.OpenId, messageData);
                            }
                        });
                    }
                    else
                    {
                        errorMessage = "需求信息不存在或已删除！";
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
            DemandQuoteModel model = new DemandQuoteModel();
            model.DemandQuote = quoteService.GetDemandQuote(quoteId, true);
            if (model.DemandQuote.IsNotNull())
            {
                // model.DemandQuote.Demand = demandService.GetDemandById(model.DemandQuote.DemandId);

                if (model.DemandQuote.Demand.IsNotNull())
                {
                    // 如果是自己发布的需求，无需购买即可查看
                    var isPostedByMyself = (model.DemandQuote.Demand.UserId == CurrentWeChatUser.UserId);
                    model.DemandQuote.HasWeChatUserBoughtForDemand = isPostedByMyself || demandService.HasWeChatUserBoughtForDemand(CurrentWeChatUser.OpenId, model.DemandQuote.Demand.Id);
                    model.DemandQuote.HasWeChatUserSharedForDemand = isPostedByMyself || demandService.HasWeChatUserSharedForDemand(CurrentWeChatUser.OpenId, model.DemandQuote.Demand.Id);
                }
                // 屏蔽联系方式
                foreach (var history in model.DemandQuote.QuoteHistories)
                {
                    if (!history.Comments.Contains("<img") && !history.Comments.Contains("<video"))
                    {
                        // history.Comments = FilterHelper.Filter(FilterLevel.PhoneAndEmail, history.Comments);
                    }
                }
            }

            model.WechatParameters = PrepareWechatShareParameter(model.DemandQuote?.Demand?.Title);
            // Gets posted resouces for current user
            if (CurrentWeChatUser.IsUserLoggedIn)
            {
                model.PostedResources.AddRange(resourceService.GetResourceByUser(CurrentWeChatUser.UserId.Value, 0, 1000).Items);
            }

            return View(model);
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
                                object messageData = null;
                                if (statusId.Equals(DemandQuoteStatus.Accept.ToString()))
                                {
                                    messageData = ConstrunctAcceptQuoteData(quoteId, quote.InsertedTimestamp);
                                }
                                else
                                {
                                    messageData = ConstrunctRejectQuoteData(quoteId, quote.InsertedTimestamp);
                                }

                                SendNotification(quoteId, toWeChatUser.OpenId, messageData);
                            }
                        });
                    }
                }
            }

            return Content(errorMessage);
        }

        [ValidateInput(false)]
        public ActionResult AddQuoteHistory(int quoteId, string comments)
        {
            bool isSuccessful = false;
            string errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(comments))
            {
                isSuccessful = false;
                errorMessage = "请输入留言内容！";
            }
            else
            {
                DemandQuote quote = quoteService.GetDemandQuote(quoteId, false);
                if (!quote.IsNotNull())
                {
                    isSuccessful = false;
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

                    isSuccessful = quoteService.NewQuoteHistory(history);

                    if (!isSuccessful)
                    {
                        errorMessage = "留言消息发送失败!";
                    }
                    else
                    {
                        try
                        {
                            if (comments.Contains("<video"))
                            {
                                comments = "对方给您发了一个视频消息";
                            }
                            else if (comments.Contains("<img"))
                            {
                                comments = "对方给您发了一个图片消息";
                            }
                            else if (comments.Contains("<a"))
                            {
                                comments = "对方给您发了一个资料链接";
                            }
                            else
                            {
                                comments = FilterHelper.Filter(FilterLevel.PhoneAndEmail, comments);
                            }

                            WeChatUser toWeChatUser = null;
                            if (quote.WeChatUserId == CurrentWeChatUser.Id)
                            {
                                var demand = demandService.GetDemandById(quote.DemandId);
                                if (demand.IsNotNull())
                                {
                                    toWeChatUser = userService.GetWeChatUser(demand.UserId);
                                }
                            }
                            else
                            {
                                toWeChatUser = userService.GetWeChatUserByWeChatUserId(quote.WeChatUserId);
                            }

                            if (toWeChatUser.IsNotNull())
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    var messageData = ConstrunctQuoteCommentsData(quoteId, quote.InsertedTimestamp, comments);
                                    SendNotification(quoteId, toWeChatUser.OpenId, messageData);
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.LogWexin("推送报价消息失败", ex.ToString());
                        }
                    }
                }
            }

            var data = new
            {
                IsSuccessful = isSuccessful,
                ErrorMessge = errorMessage
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private void SendNotification(int quoteId, string openId, object messageData)
        {
            try
            {
                var viewUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain + "/wechat/quote/detail?quoteId=" + quoteId;
                WeChatClient.Sender.SendTemplateMessage(openId, WeChatClient.Constant.TemplateMessage.QuoteRemind, messageData, viewUrl);
            }
            catch (Exception ex)
            {
                LogService.LogWexin("Failed to send notification", ex.ToString());
            }
        }

        private object ConstrunctNewQuoteData(int quoteId, string fromUserName, DateTime quoteTime, string quotePrice)
        {
            var data = new
            {
                first = new TemplateDataItem("您收到了新的报价！"),
                keyword1 = new TemplateDataItem("XQBJ" + quoteId.ToString().PadLeft(8, '0')),
                keyword2 = new TemplateDataItem(quoteTime.ToString("yyyy-MM-dd HH:mm:ss")),
                remark = new TemplateDataItem("报价方： " + fromUserName + "\r\n报价金额: " + quotePrice + "\r\n\r\n点击详情立即查看报价内容。")
            };

            return data;
        }

        private object ConstrunctQuoteCommentsData(int quoteId, DateTime quoteTime, string message)
        {
            var data = new
            {
                first = new TemplateDataItem("您收到了新的需求报价留言！"),
                keyword1 = new TemplateDataItem("XQBJ" + quoteId.ToString().PadLeft(8, '0')),
                keyword2 = new TemplateDataItem(quoteTime.ToString("yyyy-MM-dd HH:mm:ss")),
                remark = new TemplateDataItem("回复内容: " + message + "\r\n\r\n点击详情立即查看并回复留言。")
            };

            return data;
        }

        private object ConstrunctAcceptQuoteData(int quoteId, DateTime quoteTime)
        {
            var data = new
            {
                first = new TemplateDataItem("恭喜您！客户同意并接受了您的报价！"),
                keyword1 = new TemplateDataItem("XQBJ" + quoteId.ToString().PadLeft(8, '0')),
                keyword2 = new TemplateDataItem(quoteTime.ToString("yyyy-MM-dd HH:mm:ss")),
                remark = new TemplateDataItem("\r\n点击详情立即查看报价详情并联系客户。")
            };

            return data;
        }

        private object ConstrunctRejectQuoteData(int quoteId, DateTime quoteTime)
        {
            var data = new
            {
                first = new TemplateDataItem("您的报价未被客户接受！"),
                keyword1 = new TemplateDataItem("XQBJ" + quoteId.ToString().PadLeft(8, '0')),
                keyword2 = new TemplateDataItem(quoteTime.ToString("yyyy-MM-dd HH:mm:ss")),
                remark = new TemplateDataItem("\r\n点击详情立即联系客户并再次洽谈。")
            };

            return data;
        }
    }
}

