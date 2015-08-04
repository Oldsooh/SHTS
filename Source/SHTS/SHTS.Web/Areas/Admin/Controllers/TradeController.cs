using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;
using Witbird.SHTS.Web.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class TradeController : AdminBaseController
    {
        //
        // GET: /Admin/Trade/

        PublicConfigManager publicConfigManager;
        TradeService tradeService;

        public TradeController()
        {
            publicConfigManager = new PublicConfigManager();
            tradeService = new TradeService();
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Show(string id)
        {
            TradeModel model = new TradeModel();
            int tradeId = -1;
            int.TryParse(id, out tradeId);
            model.CurrentTrade = tradeService.GetTradeByTradeId(tradeId);
            model.CurrentTrade.Seller = model.CurrentTrade.Seller ?? new User();
            model.CurrentTrade.Buyer = model.CurrentTrade.Buyer ?? new User();

            return View(model);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Config()
        {
            PublicConfigModel model = new PublicConfigModel();
            PublicConfig tradeReminding = null;
            PublicConfig payCommissionPercent = null;
            PublicConfig minPayCommission = null;
            try
            {
                tradeReminding = publicConfigManager.GetConfigValue("TradeReminding");
                payCommissionPercent = publicConfigManager.GetConfigValue("PayCommissionPercent");
                minPayCommission = publicConfigManager.GetConfigValue("MinPayCommission");
            }
            catch (Exception e)
            {
                LogService.Log("获取中介参数失败", e.ToString());
            }

            model.MultipleConfigs.Add("TradeReminding", tradeReminding ?? new PublicConfig());
            model.MultipleConfigs.Add("PayCommissionPercent", payCommissionPercent ?? new PublicConfig());
            model.MultipleConfigs.Add("MinPayCommission", minPayCommission ?? new PublicConfig());

            return View(model);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult BankConfig()
        {
            PublicConfigModel model = new PublicConfigModel();
            try
            {
                List<PublicConfig> configs = publicConfigManager.GetConfigValues(Constant.TradeBankInfoConfig);

                if (configs != null && configs.Count > 0)
                {
                    for (int i = 0; i < configs.Count; i++)
                    {
                        var config = configs[i];
                        if (config != null)
                        {
                            model.MultipleConfigs.Add(config.ConfigName + i, config);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("获取线下支付银行帐号信息失败", e.ToString());
            }

            return View(model);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult List(string page, string filter)
        {
            TradeModel model = new TradeModel();

            int pageIndex = 1;
            int allCount = 0;
            // Returns all trades.
            int tradeState = -1;
            if (!string.IsNullOrEmpty(page))
            {
                Int32.TryParse(page, out pageIndex);
            }
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

            model.TradeList = tradeService.GetTradeList(20, pageIndex, tradeState, out allCount);

            model.PageIndex = pageIndex;//当前页数
            model.PageSize = 20;//每页显示多少条
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateTradeReminding(string configId, string tradeReminding)
        {
            string result = "中介交易提醒更新失败";
            bool isSuccess = false;

            try
            {
                if (string.IsNullOrWhiteSpace(tradeReminding))
                {
                    result = "中介交易提醒内容不能为空";
                }
                else
                {
                    PublicConfig config = new PublicConfig();

                    config.ConfigId = int.Parse(configId);
                    config.ConfigName = "TradeReminding";
                    config.ConfigValue = tradeReminding;
                    config.CreatedTime = DateTime.Now;
                    config.LastUpdatedTime = DateTime.Now;

                    isSuccess = publicConfigManager.AddOrUpdateConfigValue(config);

                    if (isSuccess)
                    {
                        result = "success";
                    }
                    else
                    {
                        result = "中介交易提醒更新失败";
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("更新中介交易提醒失败", ex.ToString());
                result = "更新中介交易提醒失败";
            }

            return Content(result);
        }

        [HttpPost]
        public ActionResult UpdatePayCommissionPercent(string configId, string paycommissonpercent)
        {
            string result = "中介手续费更新失败";
            bool isSuccess = false;

            try
            {
                int percent = -1;

                if (string.IsNullOrWhiteSpace(paycommissonpercent) ||
                    !int.TryParse(paycommissonpercent, out percent) ||
                    percent < 0 || percent >= 100)
                {
                    result = "中介手续费应为0到100之间的整数，包括0";
                }
                else
                {
                    PublicConfig config = config = new PublicConfig();

                    config.ConfigId = int.Parse(configId);
                    config.ConfigName = "PayCommissionPercent";
                    config.ConfigValue = paycommissonpercent;
                    config.CreatedTime = DateTime.Now;
                    config.LastUpdatedTime = DateTime.Now;

                    isSuccess = publicConfigManager.AddOrUpdateConfigValue(config);

                    if (isSuccess)
                    {
                        result = "success";
                    }
                    else
                    {
                        result = "中介手续费更新失败";
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("中介手续费更新失败", ex.ToString());
                result = "中介手续费更新失败";
            }

            return Content(result);
        }

        [HttpPost]
        public ActionResult UpdateMinPayCommission(string configId, string minpaycommisson)
        {
            string result = "最低中介手续费更新失败";
            bool isSuccess = false;

            try
            {
                int percent = -1;

                if (string.IsNullOrWhiteSpace(minpaycommisson) ||
                    !int.TryParse(minpaycommisson, out percent) ||
                    percent < 0)
                {
                    result = "最低中介手续费应为不小于0的整数，包括0";
                }
                else
                {
                    PublicConfig config = config = new PublicConfig();

                    config.ConfigId = int.Parse(configId);
                    config.ConfigName = "MinPayCommission";
                    config.ConfigValue = minpaycommisson;
                    config.CreatedTime = DateTime.Now;
                    config.LastUpdatedTime = DateTime.Now;

                    isSuccess = publicConfigManager.AddOrUpdateConfigValue(config);

                    if (isSuccess)
                    {
                        result = "success";
                    }
                    else
                    {
                        result = "最低中介手续费更新失败";
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("最低中介手续费更新失败", ex.ToString());
                result = "最低中介手续费更新失败";
            }

            return Content(result);
        }

        [HttpPost]
        public ActionResult AddTradeBankInfo(string bankName, string bankAccount, string bankUserName, string bankAddress)
        {
            string result = "添加中介手续费线下支付银行帐号失败";
            bool isSuccess = false;

            try
            {
                if (string.IsNullOrEmpty(bankName) ||
                    string.IsNullOrEmpty(bankAccount) ||
                    string.IsNullOrEmpty(bankUserName) ||
                    string.IsNullOrEmpty(bankAddress))
                {
                    result = "银行信息不能为空";
                }
                else
                {
                    PublicConfig config = config = new PublicConfig();

                    config.ConfigName = Constant.TradeBankInfoConfig;
                    config.ConfigValue = bankName + Constant.TradeBankInfoConfigSeperator + 
                        bankAccount + Constant.TradeBankInfoConfigSeperator + 
                        bankUserName + Constant.TradeBankInfoConfigSeperator + bankAddress;
                    config.CreatedTime = DateTime.Now;
                    config.LastUpdatedTime = DateTime.Now;

                    isSuccess = publicConfigManager.AddConfigValueIgnoreExists(config);

                    if (isSuccess)
                    {
                        result = "success";
                    }
                    else
                    {
                        result = "添加中介手续费线下支付银行帐号失败";
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("添加中介手续费线下支付银行帐号失败", ex.ToString());
                result = "添加中介手续费线下支付银行帐号失败";
            }

            return Content(result);
        }

        [HttpPost]
        public ActionResult DeleteTradeBankInfo(string configId)
        {
            string result = "删除中介手续费线下支付银行帐号失败";
            bool isSuccess = false;

            try
            {
                isSuccess = publicConfigManager.DeleteConfig(int.Parse(configId));

                if (isSuccess)
                {
                    result = "success";
                }
            }
            catch (Exception e)
            {
                LogService.Log("删除中介手续费线下支付银行帐号失败", e.ToString());
            }

            return Content(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReplyTradeWithOperation(string id, string operation, string content)
        {
            string result = "回复失败";
            bool isValid = true;
            if (UserInfo == null)
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

                    tradeState = trade.State;
                    trade.State = tradeService.ConvertToTradeStateFromOperation(operation, tradeState);
                    // Checks trade state.
                    tradeService.CheckTradeState(tradeState, trade.State);

                    bool isBuyerPaidInOfflineMode = false;
                    string historySubject = string.Empty;
                    if (tradeState != trade.State)
                    {
                        historySubject = "管理员" + UserInfo.UserName + "将交易状态从 " + TradeService.ConvertStateToDisplayMode(tradeState) +
                            " 改变为 " + TradeService.ConvertStateToDisplayMode(trade.State);
                    }
                    else if (operation == "paid")
                    {
                        historySubject = "管理员" + UserInfo.UserName + "已确认已收到买家线下支付交易款项";
                        isBuyerPaidInOfflineMode = true;
                    }

                    if (!trade.IsBuyerPaid)
                    {
                        isValid = tradeService.UpdateTradeOrderId(tradeId, trade.TradeOrderId, isBuyerPaidInOfflineMode);
                    }

                    if (isValid)
                    {
                        isValid = tradeService.ReplyTradeWithOperation(historySubject, content, tradeId, UserInfo.AdminId, UserInfo.UserName, true, trade.State, DateTime.Now);
                    }

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
    }
}
