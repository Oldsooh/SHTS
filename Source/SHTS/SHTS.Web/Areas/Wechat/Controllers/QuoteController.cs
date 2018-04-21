using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
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
        private static TenPayV3Info _tenPayV3Info;

        /// <summary>
        /// 微信支付相关配置信息
        /// </summary>
        public static TenPayV3Info TenPayV3Info
        {
            get
            {
                if (_tenPayV3Info == null)
                {
                    _tenPayV3Info =
                        TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
                }
                return _tenPayV3Info;
            }
        }

        DemandQuoteService quoteService = new DemandQuoteService();
        DemandService demandService = new DemandService();
        UserService userService = new UserService();
        private ResourceService resourceService = new ResourceService();

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
            if (CurrentWeChatUser.IsUserLoggedIn)
            {
                model.PostedResources.AddRange(resourceService
                    .GetResourceByUser(CurrentWeChatUser.UserId.Value, 0, 1000).Items);
            }

            return View(model);
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
                quote.HasWeChatUserSharedForDemand = isPostedByMyself || demandService.HasWeChatUserSharedForDemand(CurrentWeChatUser.OpenId, quote.Demand.Id);
                quote.WechatShareParametersForQuote = PrepareWechatShareParameter(quote.Demand.Title ?? "");
                // 屏蔽联系方式
                foreach (var history in quote.QuoteHistories)
                {
                    if (!history.Comments.Contains("<img") && !history.Comments.Contains("<video"))
                    {
                        history.Comments = FilterHelper.Filter(FilterLevel.PhoneAndEmail, history.Comments);
                    }
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
                                comments = "视频消息";
                            }
                            else if (comments.Contains("<img"))
                            {
                                comments = "图片消息";
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
                var viewUrl = "http://" + Witbird.SHTS.Web.Public.StaticUtility.Config.Domain +"/wechat/quote/detail?quoteId=" + quoteId;
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
        private WechatShareParametersForQuote PrepareWechatShareParameter(string title)
        {
            string message = string.Empty;
            var appId = string.Empty;
            var timestamp = string.Empty;
            var nonceStr = string.Empty;
            var pageurl = Request.Url.AbsoluteUri;
            var ticket = string.Empty;
            var signature = string.Empty;

            appId = TenPayV3Info.AppId;
            nonceStr = TenPayV3Util.GetNoncestr();
            TimeSpan ts = DateTime.Now - DateTime.Parse("1970-01-01 00:00:00");
            timestamp = ts.TotalSeconds.ToString().Split('.')[0];

            //微信access_token，用于获取微信jsapi_ticket  
            string token = GetAccess_token(appId, TenPayV3Info.AppSecret);
            //微信jsapi_ticket  
            ticket = GetTicket(token);

            //对所有待签名参数按照字段名的ASCII 码从小到大排序（字典序）后，使用URL键值对的格式（即key1=value1&key2=value2…）拼接成字符串  
            string str = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + pageurl;
            //签名,使用SHA1生成  
            signature = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToLower();


            var param = new WechatShareParametersForQuote
            {
                AppId = appId,
                Timestamp = timestamp,
                NonceStr = nonceStr,
                Link = pageurl,
                Signature = signature,
                Title = title
            };

            return param;
        }

        /// <summary>  
        /// 获取微信jsapi_ticket  
        /// </summary>  
        /// <param name="token">access_token</param>  
        /// <returns>jsapi_ticket</returns>  
        public string GetTicket(string token)
        {
            string ticketUrl = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + token + "&type=jsapi";
            string jsonresult = HttpGet(ticketUrl, "UTF-8");
            WX_Ticket wxTicket = JsonDeserialize<WX_Ticket>(jsonresult);
            return wxTicket.ticket;
        }

        /// <summary>  
        /// 获取微信access_token  
        /// </summary>  
        /// <param name="appid">公众号的应用ID</param>  
        /// <param name="secret">公众号的应用密钥</param>  
        /// <returns>access_token</returns>  
        private string GetAccess_token(string appid, string secret)
        {
            string tokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
            string jsonresult = HttpGet(tokenUrl, "UTF-8");
            WX_Token wx = JsonDeserialize<WX_Token>(jsonresult);
            return wx.access_token;
        }

        /// <summary>  
        /// JSON反序列化  
        /// </summary>  
        /// <typeparam name="T">实体类</typeparam>  
        /// <param name="jsonString">JSON</param>  
        /// <returns>实体类</returns>  
        private T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>  
        /// HttpGET请求  
        /// </summary>  
        /// <param name="url">请求地址</param>  
        /// <param name="encode">编码方式：GB2312/UTF-8</param>  
        /// <returns>字符串</returns>  
        private string HttpGet(string url, string encode)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=" + encode;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encode));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}

