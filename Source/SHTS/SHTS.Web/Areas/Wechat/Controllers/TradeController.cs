using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;
using Witbird.SHTS.Web.Areas.Wechat.Models;
using Witbird.SHTS.BLL.Managers;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class TradeController : WechatBaseController
    {
        TradeService tradeService;
        OrderService orderService;
        PublicConfigService configService;
        UserService userService;
        DemandService demandService;
        ResourceManager resourceManager;

        public TradeController()
        {
            tradeService = new TradeService();
            orderService = new OrderService();
            configService = new PublicConfigService();
            userService = new UserService();
            demandService = new DemandService();
            resourceManager = new ResourceManager();
        }

        //
        // GET: /Trade/

        /// <summary>
        /// Trades list.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string page, string filter)
        {
            TradeModel model = new TradeModel();

            int pageIndex = 1;
            int allCount = 0;
            int tradeState = TranslateFilterToState(filter);

            if (!string.IsNullOrEmpty(page))
            {
                Int32.TryParse(page, out pageIndex);
            }

            model.TradeList = tradeService.GetTradeList(15, pageIndex, tradeState, out allCount);

            model.PageIndex = pageIndex;//当前页数
            model.PageSize = 15;//每页显示多少条
            model.PageStep = 10;//每页显示多少页码
            model.AllCount = allCount;//总条数
            model.Filter = filter;

            if (model.AllCount % model.PageSize == 0)
            {
                model.PageCount = model.AllCount / model.PageSize;
            }
            else
            {
                model.PageCount = model.AllCount / model.PageSize + 1;
            }

            return View(model);
        }

        public ActionResult TradeRule(int id, string type)
        {
            TradeParameter parameter = null;
            var result = ValidateTradeParameters(id, type, out parameter);

            if (result != null)
            {
                return result;
            }

            try
            {
                var count = 0;
                var myTradeList = tradeService.GetTradeListByUserId(CurrentUser.UserId, int.MaxValue, 1, out count);
                if (myTradeList.IsNotNull())
                {
                    var trade = myTradeList.FirstOrDefault(item => item.ResourceUrl.Replace("/wechat", string.Empty).Equals(parameter.TradeResourceUrl, StringComparison.CurrentCultureIgnoreCase));
                    if (trade != null)
                    {
                        result = new RedirectResult("/wechat/trade/show/" + trade.TradeId);
                        return result;
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return View(parameter);
        }

        /// <summary>
        /// New a third-part trade
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult New(int id, string type)
        {
            TradeParameter parameter = new TradeParameter();
            var result = ValidateTradeParameters(id, type, out parameter);

            if (result != null)
            {
                return result;
            }

            PublicConfigModel configModel = new PublicConfigModel();
            WeChatTradeModel model = new WeChatTradeModel();
            model.TradeParameter = parameter;

            configModel.MultipleConfigs.Add("TradeReminding", configService.GetConfigValue("TradeReminding") ?? new PublicConfig());
            configModel.MultipleConfigs.Add("PayCommissionPercent", configService.GetConfigValue("PayCommissionPercent") ?? new PublicConfig());
            configModel.MultipleConfigs.Add("MinPayCommission", configService.GetConfigValue("MinPayCommission") ?? new PublicConfig());

            var tradeCommissionRule = new SinglePageService().GetSingPageById("8").ContentStyle;// Id = 8 是中介手续费介绍
            configModel.MultipleConfigs.Add("TradeCommissionRule", new PublicConfig() { ConfigName = "TradeCommissionRule", ConfigValue = tradeCommissionRule });

            model.TradeConfig = configModel;
            model.CurrentUser = UserInfo ?? new User();
            model.CurrentWeChatUser = CurrentWeChatUser;
            model.BankInfos = (UserInfo == null ? new List<UserBankInfo>() : userService.GetUserBankInfos(UserInfo.UserId));

            if (model.BankInfos != null && model.BankInfos.Count > 0)
            {
                var bankInfo = model.BankInfos.FirstOrDefault(c => c.IsDefault);
                if (bankInfo == null)
                {
                    model.BankInfos[0].IsDefault = true;
                }
            }

            return View(model);
        }

        /// <summary>
        /// New a third-part trade
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New
        (
            string qq,
            string phone,
            string email,
            string relationship,
            string username,
            string amount,
            string bankid,
            string bankname,
            string bankaccount,
            string bankusername,
            string bankaddress,
            string address,
            string tradesubject,
            string tradedetail,
            string resourceurl
        )
        {
            var isSuccessful = false;
            var errorMessage = string.Empty;

            if (!IsUserLogin)
            {
                isSuccessful = false;
                errorMessage = "您没有绑定会员账户，不能进行中介申请操作！";
            }
            else
            {
                Model.User toUser = null;
                UserBankInfo bankInfo = null;
                int payCommissionPercent = -1;
                int minPayCommission = -1;
                decimal outAmount = -1;
                //如果用户新增的银行信息，但是后来中介申请失败，那么需要删除掉这条记录，这里不使用transaction
                bool isNewBankInfo = false;
                #region Checks request parameter value with get operation

                try
                {
                    #region Null check

                    qq.CheckEmptyString("QQ");
                    phone.CheckEmptyString("联系手机");
                    email.CheckEmptyString("联系邮箱");
                    relationship.CheckEmptyString("买卖关系");
                    username.CheckEmptyString("对方用户名");
                    amount.CheckEmptyString("交易金额");
                    bankname.CheckEmptyString("银行名称");
                    bankaccount.CheckEmptyString("银行账号");
                    bankusername.CheckEmptyString("银行用户名");
                    address.CheckEmptyString("收货地址");
                    tradesubject.CheckEmptyString("交易标题");
                    tradedetail.CheckEmptyString("交易详情");
                    resourceurl.CheckEmptyString("资源链接地址");

                    #endregion Null check

                    toUser = ValidateUserWithGet(username);
                    ValidatePayerRelationship(relationship);//, payer);
                    bankInfo = ValidateBankInfoWithGet(bankid, bankname, bankaccount, bankusername, bankaddress, out isNewBankInfo);
                    payCommissionPercent = ValidatePayCommissionWithGet("PayCommissionPercent");
                    minPayCommission = ValidatePayCommissionWithGet("MinPayCommission");
                    outAmount = ValidateAmountWithGet(amount, minPayCommission);

                    isSuccessful = true;
                }
                catch (ArgumentNullException e)
                {
                    errorMessage = "中介参数错误：" + e.ParamName + "不能为空";
                    isSuccessful = false;
                }
                catch (ArgumentException e)
                {
                    errorMessage = "中介参数错误：" + e.Message;
                    isSuccessful = false;
                }

                #endregion Checks request parameter value with get operation

                if (isSuccessful)
                {
                    try
                    {
                        Trade tradeEntity = new Trade();

                        #region 构造中介申请对象

                        tradeEntity.UserId = UserInfo == null ? -1 : UserInfo.UserId;
                        tradeEntity.UserQQ = qq;
                        tradeEntity.UserCellPhone = phone;
                        tradeEntity.UserEmail = email;
                        tradeEntity.UserBankInfo = "银行名称：" + bankInfo.BankName + "\r\n银行账号："
                            + bankInfo.BankAccount + "\r\n用户姓名：" + bankInfo.BankUserName + "\r\n开户行地址：" + bankInfo.BankAddress;
                        tradeEntity.UserAddress = address;

                        // 我是卖家
                        if (relationship.Equals("seller"))
                        {
                            tradeEntity.SellerId = tradeEntity.UserId;
                            tradeEntity.BuyerId = toUser.UserId;
                        }
                        //我是买家
                        else if (relationship.Equals("buyer"))
                        {
                            tradeEntity.SellerId = toUser.UserId;
                            tradeEntity.BuyerId = tradeEntity.UserId;
                        }

                        tradeEntity.TradeAmount = outAmount;
                        tradeEntity.TradeSubject = tradesubject;
                        tradeEntity.TradeBody = tradedetail;

                        tradeEntity.PayCommissionPercent = double.Parse(payCommissionPercent.ToString()) / 100;
                        tradeEntity.PayCommission = tradeEntity.TradeAmount * (Convert.ToDecimal(tradeEntity.PayCommissionPercent));
                        //Sets the min pay commission value here if it is less than the min value.
                        if (tradeEntity.PayCommission < minPayCommission)
                        {
                            tradeEntity.PayCommission = minPayCommission;
                        }

                        // 中介手续费支付方
                        #region old code
                        //if (payer.Equals("buyer", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    tradeEntity.Payer = (int)Payer.Buyer;
                        //    tradeEntity.BuyerPay = tradeEntity.TradeAmount + tradeEntity.PayCommission;
                        //    tradeEntity.SellerGet = tradeEntity.TradeAmount;
                        //}
                        //else if (payer.Equals("seller", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    tradeEntity.Payer = (int)Payer.Seller;
                        //    tradeEntity.BuyerPay = tradeEntity.TradeAmount;
                        //    tradeEntity.SellerGet = tradeEntity.TradeAmount - tradeEntity.PayCommission;
                        //}
                        //else if (payer.Equals("both", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    tradeEntity.Payer = (int)Payer.Both;
                        //    tradeEntity.BuyerPay = tradeEntity.TradeAmount + tradeEntity.PayCommission / 2;
                        //    tradeEntity.SellerGet = tradeEntity.TradeAmount - tradeEntity.PayCommission / 2;
                        //}
                        #endregion

                        // 中介手续费支付方为卖方
                        tradeEntity.Payer = (int)Payer.Seller;
                        tradeEntity.BuyerPay = tradeEntity.TradeAmount;
                        tradeEntity.SellerGet = tradeEntity.TradeAmount - tradeEntity.PayCommission;

                        tradeEntity.CreatedTime = DateTime.Now;
                        tradeEntity.LastUpdatedTime = DateTime.Now;
                        tradeEntity.State = (int)TradeState.New;
                        tradeEntity.ViewCount = 0;
                        tradeEntity.ResourceUrl = resourceurl;
                        tradeEntity.IsBuyerPaid = false;
                        tradeEntity.TradeOrderId = string.Empty;

                        #endregion 构造中介申请对象

                        //添加中介申请记录到数据库
                        isSuccessful = tradeService.AddNewTradeRecord(tradeEntity);

                    }
                    catch (Exception e)
                    {
                        isSuccessful = false;
                        errorMessage = "系统异常，请重新尝试或联系管理员";
                        LogService.Log("中介申请发生异常", e.ToString());
                    }
                }
            }

            var data = new
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Show(string id)
        {
            if (UserInfo == null)
            {
                var errorMessage = new ErrorMessage() { Title = "权限不足", Detail = "您没有权限查看该记录!" };
                return RedirectToAction("Index", "Error", errorMessage);
            }

            WeChatTradeModel model = new WeChatTradeModel();
            model.CurrentWeChatUser = CurrentWeChatUser;

            try
            {
                int tradeId = -1;
                int.TryParse(id, out tradeId);
                model.CurrentTrade = tradeService.GetTradeByTradeId(tradeId);

                // The current trade does not exist.
                if (model.CurrentTrade == null)
                {
                    var errorMessage = new ErrorMessage() { Title = "404-中介记录不存在", Detail = "您查看的记录不存在或已被删除!" };
                    return RedirectToAction("Index", "Error", errorMessage);
                }
                // Only the buyer or seller has permission to view the trade detail.
                else if (model.CurrentTrade.SellerId != UserInfo.UserId && model.CurrentTrade.BuyerId != UserInfo.UserId)
                {
                    var errorMessage = new ErrorMessage() { Title = "权限不足", Detail = "您没有权限查看该记录!" };
                    return RedirectToAction("Index", "Error", errorMessage);
                }

                model.CurrentTrade.Seller = model.CurrentTrade.Seller ?? new User();
                model.CurrentTrade.Buyer = model.CurrentTrade.Buyer ?? new User();
                model.CurrentTrade.CreatedUserName =
                    model.CurrentTrade.Seller.UserId == model.CurrentTrade.UserId ?
                    model.CurrentTrade.Seller.UserName : model.CurrentTrade.Buyer.UserName;
                model.CurrentUser = UserInfo;
            }
            catch
            {
                model.CurrentTrade = new Trade();
            }

            return View(model);
        }

        public ActionResult MyTradeList(string page)
        {
            WeChatTradeModel model = new WeChatTradeModel();
            model.CurrentWeChatUser = CurrentWeChatUser;

            if (IsUserLogin)
            {
                //页码，总数重置
                int pageIndex = 1;
                if (!string.IsNullOrEmpty(page))
                {
                    Int32.TryParse(page, out pageIndex);
                }
                int allCount = 0;
                model.TradeList = tradeService.GetTradeListByUserId(UserInfo.UserId, 10, pageIndex, out allCount);

                model.PageIndex = pageIndex;//当前页数
                model.PageSize = 10;//每页显示多少条
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

                model.CurrentUser = UserInfo;
            }
            else
            {
                var errorMessage = new ErrorMessage() { Title = "权限错误", Detail = "您还没有绑定PC端会员帐号！请先绑定会员帐号后查看！" };
                return RedirectToAction("Index", "Error", errorMessage);
            }

            return View(model);
        }

        /// <summary>
        /// 生成中介交易支付订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Pay(string id)
        {
            string result = "";
            try
            {
                if (UserInfo != null)
                {
                    int tradeId = -1;
                    if (int.TryParse(id, out tradeId))
                    {
                        Trade trade = tradeService.GetTradeByTradeId(tradeId);

                        if (trade != null && trade.TradeId > 0)
                        {
                            if (UserInfo.UserId != trade.BuyerId)
                            {
                                //throw new ArgumentException("只有买家能够支付中介交易金额");
                            }

                            if (trade.IsBuyerPaid)
                            {
                                throw new ArgumentException("该中介交易已付款，请勿重复付款");
                            }
                            else
                            {
                                TradeOrder order = null;
                                string url = Request.Url.Authority;
                                if (url.IndexOf("http://") < 0)
                                {
                                    url = "http://" + url;
                                }
                                url = url + "/trade/show/" + tradeId;

                                if (!string.IsNullOrEmpty(trade.TradeOrderId))
                                {
                                    order = orderService.GetOrderByOrderId(trade.TradeOrderId);
                                }
                                if (order != null && order.UserName == UserInfo.UserName && order.Amount == trade.BuyerPay)
                                {
                                    //result = "success&orderid=" + order.OrderId + "&returnurl=" + Request.UrlReferrer.AbsoluteUri;
                                    result = string.Format(Constant.PostPayInfoFormatForWechat, order.OrderId, url);
                                }
                                else
                                {
                                    // 删掉原来的订单
                                    if (order != null)
                                    {
                                        orderService.DeleteOrderById(order.OrderId);
                                    }

                                    string orderId = orderService.GenerateNewOrderNumber();
                                    string subject = "活动在线网 | 支付中介交易款项";
                                    string body = "中介内容：" + trade.TradeSubject;
                                    //int userId = UserInfo.UserId;
                                    string username = UserInfo.UserName;
                                    decimal amount = trade.BuyerPay;
                                    string resourceUrl = url;

                                    order = orderService.AddNewOrder(orderId, subject, body, amount, OrderState.New, username, resourceUrl, (int)OrderType.Trade, trade.TradeId);
                                    bool success = tradeService.UpdateTradeOrderId(trade.TradeId, orderId, trade.IsBuyerPaid); ;

                                    if (success && order != null)
                                    {
                                        //result = "success&orderid=" + orderId + "&returnurl=" + Request.UrlReferrer.AbsoluteUri;
                                        result = string.Format(Constant.PostPayInfoFormatForWechat, orderId, url);
                                    }
                                    else
                                    {
                                        result = "中介交易在线支付订单页面生成失败，请重新尝试";
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("中介交易信息不存在");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("中介交易信息不存在");
                    }
                }
                else
                {
                    result = "当前未登录或登录超时";
                }

            }
            catch (ArgumentException e)
            {
                result = e.Message;
            }
            catch (Exception e)
            {
                LogService.Log("生成中介交易支付订单信息", e.ToString());
                result = "中介交易在线支付订单页面生成失败，请重新尝试";
            }

            return Content(result);
        }

        /// <summary>
        /// 买卖双方回复交易
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ReplyTradeWithOperation(string id, string operation, string content)
        {
            bool isSuccessful = false;
            string errorMessage = string.Empty;

            if (!IsUserLogin)
            {
                errorMessage = "当前未登录或登录超时，请刷新页面后重试！";
                isSuccessful = false;
            }
            else
            {
                try
                {
                    int tradeId = -1;
                    int oldTradeState = -1;
                    int newState = -1;

                    tradeId = int.Parse(id);
                    tradeService.CheckReplyTradeParameters(operation, content);
                    Trade trade = tradeService.GetTradeByTradeId(tradeId);

                    if (trade == null)
                    {
                        throw new ArgumentException("中介交易信息不存在或已删除！");
                    }

                    string roleName = ValidateUserReplyTradePermission(UserInfo.UserId, trade.SellerId, trade.BuyerId);

                    // Checks trade state.
                    oldTradeState = trade.State;
                    newState = tradeService.ConvertToTradeStateFromOperation(operation, oldTradeState);
                    tradeService.CheckTradeState(oldTradeState, newState);

                    string historySubject = string.Empty;
                    if (oldTradeState != newState)
                    {
                        historySubject = roleName + UserInfo.UserName + "将交易状态从 " + TradeService.ConvertStateToDisplayMode(oldTradeState) +
                            " 改变为 " + TradeService.ConvertStateToDisplayMode(newState);
                    }

                    isSuccessful = tradeService.ReplyTradeWithOperation(historySubject, content, tradeId, UserInfo.UserId, UserInfo.UserName, false, (TradeState)newState, DateTime.Now);

                    if (!isSuccessful)
                    {
                        errorMessage = "回复失败，请重新尝试！";
                    }
                }
                catch (Exception e)
                {
                    LogService.Log("回复交易失败", e.ToString());
                    errorMessage = e.Message;
                }
            }

            var data = new
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Translates the filter condition from UI to a corresponding trade state in system. Such as filter 'new' means TradeState.New.
        /// </summary>
        private int TranslateFilterToState(string filter)
        {
            int tradeState = -1;

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
            }

            switch (filter)
            {
                case "new":
                    tradeState = (int)TradeState.New;
                    break;
                case "inprogress":
                    tradeState = (int)TradeState.InProgress;
                    break;
                case "completed":
                    tradeState = (int)TradeState.Completed;
                    break;
                case "finished":
                    tradeState = (int)TradeState.Finished;
                    break;
                case "invalid":
                    tradeState = (int)TradeState.Invalid;
                    break;
                case "all":
                default:
                    tradeState = -1;
                    break;
            }

            return tradeState;
        }

        /// <summary>
        /// Validates user permission.
        /// </summary>
        private User ValidateUserWithGet(string username)
        {
            Model.User toUser = userService.GetUserByUserName(username);
            if (toUser == null)
            {
                throw new ArgumentException("交易另一方用户名不存在");
            }

            if (UserInfo == null)
            {
                throw new ArgumentException("您没绑定会员账户");
            }

            if (UserInfo.UserId == toUser.UserId)
            {
                throw new ArgumentException("不能自己对自己发起中介申请");
            }

            return toUser;
        }

        /// <summary>
        /// Validates payer information.
        /// </summary>
        private void ValidatePayerRelationship(string relationship)//, string payer)
        {
            if (!relationship.Equals("seller", StringComparison.CurrentCultureIgnoreCase)
                    && !relationship.Equals("buyer", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("请确认您是卖家还是买家");
            }

            //if (!payer.Equals("seller", StringComparison.CurrentCultureIgnoreCase) &&
            //    !payer.Equals("buyer", StringComparison.CurrentCultureIgnoreCase) &&
            //    !payer.Equals("both", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    throw new ArgumentException("请确认中介手续费支付方");
            //}
        }

        /// <summary>
        /// Validates bank info with get. We need to add a new bank record to database if user select new bank info.
        /// </summary>
        private UserBankInfo ValidateBankInfoWithGet(string bankid, string bankname, string bankaccount, string bankusername, string bankaddress, out bool isNewBankInfo)
        {
            UserBankInfo bankInfo = null;
            isNewBankInfo = false;

            if (bankid.Equals("-1") && UserInfo != null)
            {
                bankInfo = new UserBankInfo();

                bankInfo.UserId = UserInfo.UserId;
                bankInfo.BankName = bankname ?? "信息缺失";
                bankInfo.BankAccount = bankaccount ?? "信息缺失";
                bankInfo.BankUserName = bankusername ?? "信息缺失";
                bankInfo.BankAddress = bankaddress ?? "信息缺失";
                bankInfo.CreatedTime = DateTime.Now;
                bankInfo.LastUpdatedTime = DateTime.Now;
                bankInfo.IsDefault = false;

                if (!userService.AddUserBankInfo(bankInfo))
                {
                    throw new ArgumentException("新增银行信息失败");
                }
                else
                {
                    isNewBankInfo = true;
                }
            }
            else
            {
                bankInfo = userService.GetUserBankInfo(int.Parse(bankid));
            }

            bankInfo.CheckNullObject("银行信息");

            return bankInfo;
        }

        /// <summary>
        /// Validates pay commission configuration.
        /// </summary>
        private int ValidatePayCommissionWithGet(string configName)
        {
            int payCommissionValue = -1;
            PublicConfig config = tradeService.GetTradeConfig(configName);

            if (config == null ||
                !int.TryParse(config.ConfigValue, out payCommissionValue) ||
                payCommissionValue < 0)
            {
                throw new ArgumentException("中介手续费设置不正确，请联系系统管理员");
            }

            return payCommissionValue;
        }

        /// <summary>
        /// Validates user input amount.
        /// </summary>
        private decimal ValidateAmountWithGet(string amount, decimal minTradeAmount)
        {
            decimal outAmount = -1;
            if (!decimal.TryParse(amount, out outAmount) ||
                outAmount < 0)
            {
                throw new ArgumentException("交易金额不正确，请重新输入");
            }

            if (outAmount < minTradeAmount)
            {
                throw new ArgumentException("交易金额不能少于最低中介手续费");
            }

            return outAmount;
        }

        /// <summary>
        /// Validates user reply trade permission and returns role name if valid permission
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="sellerId"></param>
        /// <param name="buyerId"></param>
        /// <returns>Role name.</returns>
        private string ValidateUserReplyTradePermission(int currentUserId, int sellerId, int buyerId)
        {
            if (currentUserId != sellerId && currentUserId != buyerId)
            {
                throw new ArgumentException("只有中介交易买卖双方能够回复交易");
            }

            if (currentUserId == sellerId)
            {
                return "卖家";
            }
            else
            {
                return "买家";
            }
        }

        private ActionResult ValidateTradeParameters(int id, string type, out TradeParameter parameter)
        {
            ActionResult result = null;
            parameter = new TradeParameter();

            if (!IsUserLogin)
            {
                result = Redirect("/wechat/account/login");
            }
            else
            {
                int toUserId = -1;

                if (string.IsNullOrEmpty(type) || (!"demand".Equals(type.ToLower()) && !"resource".Equals(type.ToLower())))
                {
                    var errorMessage = new ErrorMessage() { Title = "中介参数错误", Detail = "中介申请参数错误，请返回刷新页面后重试！" };
                    result = RedirectToAction("Index", "Error", errorMessage);
                }

                if ("demand".Equals(type.ToLower()))
                {
                    parameter.TradeRelationShip = "seller";

                    var demand = demandService.GetDemandById(id, false);
                    if (demand == null)
                    {
                        var errorMessage = new ErrorMessage() { Title = "中介参数错误", Detail = "中介需求不存在或已被删除，请返回刷新页面后重试！" };
                        result = RedirectToAction("Index", "Error", errorMessage);
                    }
                    else if (demand.Status == (int)DemandStatus.Complete)
                    {
                        var errorMessage = new ErrorMessage() { Title = "中介参数错误", Detail = "中介需求已找到资源供应商！" };
                        result = RedirectToAction("Index", "Error", errorMessage);
                    }
                    else
                    {
                        toUserId = demand.UserId;
                    }
                }
                else if ("resource".Equals(type.ToLower()))
                {
                    parameter.TradeRelationShip = "buyer";

                    var resource = resourceManager.GetResourceById(id, false);
                    if (resource == null)
                    {
                        var errorMessage = new ErrorMessage() { Title = "中介参数错误", Detail = "中介资源不存在或已被删除，请返回刷新页面后重试！" };
                        result = RedirectToAction("Index", "Error", errorMessage);
                    }
                    else
                    {
                        toUserId = resource.UserId;
                    }
                }
                else
                {
                    // noting to do
                }

                if (toUserId != -1)
                {
                    if (CurrentUser.UserId == toUserId)
                    {
                        var errorMessage = new ErrorMessage() { Title = "中介参数错误", Detail = "不能对自己发布的资源或需求进行中介申请！" };
                        result = RedirectToAction("Index", "Error", errorMessage);
                    }
                    else
                    {
                        var toUser = userService.GetUserById(toUserId);
                        if (toUser == null)
                        {
                            var errorMessage = new ErrorMessage() { Title = "中介参数错误", Detail = "中介对象账户不存在或已被注销！" };
                            result = RedirectToAction("Index", "Error", errorMessage);
                        }
                        else
                        {
                            parameter.TradeUserId = toUserId;
                            parameter.TradeUserName = toUser.UserName;
                        }
                    }
                }
            }

            parameter.TradeResourceId = id;
            parameter.TradeType = type;

            return result;
        }
    }
}
