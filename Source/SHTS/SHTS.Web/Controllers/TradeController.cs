using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Public;
using Witbird.SHTS.Common;
using System.Text.RegularExpressions;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Controllers
{
    public class TradeController : BaseController
    {
        TradeService tradeService;
        OrderService orderService;
        PublicConfigService configService;
        UserService userService;

        public TradeController()
        {
            tradeService = new TradeService();
            orderService = new OrderService();
            configService = new PublicConfigService();
            userService = new UserService();
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

        /// <summary>
        /// New a third-part trade
        /// </summary>
        /// <returns></returns>
        public ActionResult New()
        {
            if (!IsUserLogin)
            {
                return Redirect("/account/login");
            }

            PublicConfigModel configModel = new PublicConfigModel();
            TradeModel model = new TradeModel();

            configModel.MultipleConfigs.Add("TradeReminding", configService.GetConfigValue("TradeReminding") ?? new PublicConfig());
            configModel.MultipleConfigs.Add("PayCommissionPercent", configService.GetConfigValue("PayCommissionPercent") ?? new PublicConfig());
            configModel.MultipleConfigs.Add("MinPayCommission", configService.GetConfigValue("MinPayCommission") ?? new PublicConfig());
            model.TradeConfig = configModel;
            model.CurrentUser = UserInfo ?? new User();
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
            string agreerule,
            string tradesubject,
            string tradedetail,
            string resourceurl
        )
        {
            if (!IsUserLogin)
            {
                return Content("当前未登录");
            }
            string result = "中介申请失败";
            bool valid = true;
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
                agreerule.CheckEmptyString("同意中介规则");
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
            }
            catch (ArgumentNullException e)
            {
                result = e.ParamName + "不能为空";
                valid = false;
            }
            catch (ArgumentException e)
            {
                result = e.Message;
                valid = false;
            }

            #endregion Checks request parameter value with get operation

            if (valid)
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
                    valid = tradeService.AddNewTradeRecord(tradeEntity);

                    if (valid)
                    {
                        result = "success";
                    }
                    else
                    {
                        if (isNewBankInfo)
                        {
                            userService.DeleteUserBankInfo(bankInfo.BankId);
                        }
                        result = "中介申请失败，请重新尝试";
                    }

                }
                catch (Exception e)
                {
                    result = "中介申请失败，请重新尝试或联系管理员";
                    LogService.Log("中介申请发生异常", e.ToString());
                }
            }
            return Content(result);
        }

        public ActionResult Show(string id)
        {
            if (UserInfo == null)
            {
                return Redirect("/common/ErrorAccessDenied");
            }
            
            TradeModel model = new TradeModel();

            try
            {
                int tradeId = -1;
                int.TryParse(id, out tradeId);
                model.CurrentTrade = tradeService.GetTradeByTradeId(tradeId);
                
                // The current trade does not exist.
                if (model.CurrentTrade == null)
                {
                    return Redirect("/common/ErrorPageNotFound");
                }
                // Only the buyer or seller has permission to view the trade detail.
                else if (model.CurrentTrade.SellerId != UserInfo.UserId && model.CurrentTrade.BuyerId != UserInfo.UserId)
                {
                    return Redirect("/common/ErrorAccessDenied");
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

        public ActionResult MyTradeList(string id)
        {
            TradeModel model = new TradeModel();
            if (IsUserLogin)
            {
                //页码，总数重置
                int page = 1;
                if (!string.IsNullOrEmpty(id))
                {
                    Int32.TryParse(id, out page);
                }
                int allCount = 0;
                model.TradeList = tradeService.GetTradeListByUserId(UserInfo.UserId, 20, page, out allCount);

                model.PageIndex = page;//当前页数
                model.PageSize = 20;//每页显示多少条
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
                                url = url + "/trade/" + tradeId + ".html";

                                if (!string.IsNullOrEmpty(trade.TradeOrderId))
                                {
                                    order = orderService.GetOrderByOrderId(trade.TradeOrderId);
                                }
                                if (order != null && order.UserName == UserInfo.UserName && order.Amount == trade.BuyerPay)
                                {
                                    //result = "success&orderid=" + order.OrderId + "&returnurl=" + Request.UrlReferrer.AbsoluteUri;
                                    result = string.Format(Constant.PostPayInfoFormat, order.OrderId, url);
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
                                    string body = "买家" + UserInfo.UserName + "支付中介交易款项, 中介交易摘要：" + trade.TradeSubject;
                                    //int userId = UserInfo.UserId;
                                    string username = UserInfo.UserName;
                                    decimal amount = trade.BuyerPay;
                                    int state = (int)OrderState.New;
                                    string resourceUrl = url;

                                    order = orderService.AddNewOrder(orderId, subject, body, amount, state, username, resourceUrl, (int)OrderType.Trade, trade.TradeId);
                                    bool success = tradeService.UpdateTradeOrderId(trade.TradeId, orderId, trade.IsBuyerPaid); ;

                                    if (success && order != null)
                                    {
                                        //result = "success&orderid=" + orderId + "&returnurl=" + Request.UrlReferrer.AbsoluteUri;
                                        result = string.Format(Constant.PostPayInfoFormat, orderId, url);
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
            string result = "回复失败";
            bool isValid = true;
            if (!IsUserLogin)
            {
                result = "当前未登录或登录超时，请重新登录";
                isValid = false;
            }

            if (isValid)
            {
                try
                {
                    int tradeId = -1;
                    int tradeState = -1;

                    tradeId = int.Parse(id);
                    tradeService.CheckReplyTradeParameters(operation, content);
                    Trade trade = tradeService.GetTradeByTradeId(tradeId);

                    if (trade == null)
                    {
                        throw new ArgumentException("交易信息不存在");
                    }

                    string roleName = ValidateUserReplyTradePermission(UserInfo.UserId, trade.SellerId, trade.BuyerId);

                    // Checks trade state.
                    tradeState = trade.State;
                    trade.State = tradeService.ConvertToTradeStateFromOperation(operation, tradeState);
                    tradeService.CheckTradeState(tradeState, trade.State);

                    string historySubject = string.Empty;
                    if (tradeState != trade.State)
                    {
                        historySubject = roleName + UserInfo.UserName + "将交易状态从 " + TradeService.ConvertStateToDisplayMode(trade.State) +
                            " 改变为 " + TradeService.ConvertStateToDisplayMode(tradeState);
                    }

                    isValid = tradeService.ReplyTradeWithOperation(historySubject, content, tradeId, UserInfo.UserId, UserInfo.UserName, false, tradeState, DateTime.Now);
                    
                    if (isValid)
                    {
                        result = "success";
                    }
                    else
                    {
                        result = "回复交易失败";
                    }
                }
                catch (Exception e)
                {
                    LogService.Log("回复交易失败", e.ToString());
                    result = e.Message;
                }
            }

            return Content(result);
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
                throw new ArgumentException("交易另一方用户名不存在，请仔细检查");
            }

            if (UserInfo == null)
            {
                throw new ArgumentException("当前未登录，您可以在新窗口中登录后返回继续申请");
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
    }
}
