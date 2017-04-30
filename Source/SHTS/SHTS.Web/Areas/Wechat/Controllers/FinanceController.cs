using SHTS.Finance;
using System;
using System.Web.Mvc;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Models;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class FinanceController : WechatBaseController
    {
        FinanceManager financeManager = new FinanceManager();

        /// <summary>
        /// View my finance balance.
        /// </summary>
        /// <returns></returns>
        public ActionResult MyBalance()
        {
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                return Redirect("/wechat/account/login");
            }

            var model = new FinanceModel();

            var myBalance = new FinanceBalance();
            myBalance = financeManager.GetFinanceBalance(CurrentWeChatUser.UserId.Value);

            if (myBalance == null)
            {
                myBalance = financeManager.AddFinanceBalance(CurrentUser.UserId);
            }

            model.CurrentWechatUser = CurrentWeChatUser;
            model.CurrentUser = CurrentUser;
            model.UserBalance = myBalance;
            return View(model);
        }

        [HttpGet]
        public ActionResult Withdraw()
        {
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                return Redirect("/wechat/account/login");
            }

            var model = new FinanceModel();
            var myBalance = new FinanceBalance();
            myBalance = financeManager.GetFinanceBalance(CurrentUser.UserId);

            if (myBalance == null)
            {
                myBalance = financeManager.AddFinanceBalance(CurrentUser.UserId);
            }

            model.UserBalance = myBalance;
            model.CurrentUser = CurrentUser;
            model.CurrentWechatUser = CurrentWeChatUser;

            return View(model);
        }

        [HttpPost]
        public ActionResult Withdraw(string amountString, string wechatPayQRCodeImgUrl)
        {
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                return Redirect("/wechat/account/login");
            }

            string errorMessage = "提现操作未成功";
            bool isSuccessful = false;

            try
            {
                decimal withdrawAmount = 0;
                decimal minWithdrawAmount = 10; //最小体现金额为10元

                if (!FinanceHelper.TryParseAmount(amountString, minWithdrawAmount, out withdrawAmount))
                {
                    isSuccessful = false;
                    errorMessage = "提现金额输入不正确，请重新输入";
                }
                else if (string.IsNullOrEmpty(wechatPayQRCodeImgUrl))
                {
                    isSuccessful = false;
                    errorMessage = "请上传您的微信收款二维码图片";
                }
                else
                {
                    FinanceBalance balance = financeManager.GetFinanceBalance(CurrentUser.UserId);

                    if (balance == null)
                    {
                        balance = financeManager.AddFinanceBalance(CurrentUser.UserId);
                    }

                    if (balance.AvailableBalance < withdrawAmount)
                    {
                        isSuccessful = false;
                        errorMessage = "账户余额不足，提现操作未成功";
                    }
                    else
                    {

                        FinanceWithdrawRecord record = new FinanceWithdrawRecord();

                        record.UserId = CurrentUser.UserId;
                        record.Amount = withdrawAmount;
                        record.BankInfo = wechatPayQRCodeImgUrl;
                        record.WithdrawStatus = WithdrawStatus.New.ToString();
                        record.InsertedTimestamp = DateTime.Now;
                        record.LastUpdatedTimestamp = DateTime.Now;

                        if (financeManager.CreateWithdrawRecord(record))
                        {
                            isSuccessful = true;
                            errorMessage = "提现操作成功，请耐心等待客服人员确认";
                        }
                        else
                        {
                            isSuccessful = false;
                            errorMessage = "提现操作未成功";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                isSuccessful = false;
                errorMessage = "提现操作未成功";
                LogService.Log("Withdraw", e.ToString());
            }

            var data = new
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WithdrawRecord()
        {
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                return Redirect("/wechat/account/login");
            }

            FinanceModel model = new FinanceModel();
            model.CurrentWechatUser = CurrentWeChatUser;
            model.CurrentUser = CurrentUser;
            model.WithdrawRecords = financeManager.GetFianceWithdrawRecords(CurrentUser.UserId);

            return View(model);
        }

        //取消申请，只有新增未处理的才能取消，且只能取消自己的
        public ActionResult CancelWithdraw(string id)
        {
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                return Redirect("/wechat/account/login");
            }

            var operationResult = new OperationResult();

            int recordId = -1;
            if (int.TryParse(id, out recordId))
            {
                FinanceWithdrawRecord record = financeManager.GetWithdrawRecordDetail(recordId);

                if (record != null && record.UserId == CurrentUser.UserId)
                {
                    operationResult = financeManager.UpdateWithdrawRecordStatus(recordId, WithdrawStatus.Cancelled);
                    if (operationResult.IsSuccessful)
                    {
                        operationResult.ErrorMessage = "取消提现操作成功";
                    }
                    else if (string.IsNullOrWhiteSpace(operationResult.ErrorMessage))
                    {
                        operationResult.ErrorMessage = "取消提现操作未成功";
                    }
                    else
                    {
                        // nothing to do.
                    }
                }
                else
                {
                    operationResult.IsSuccessful = false;
                    operationResult.ErrorMessage = "系统不存在该提现记录，取消提现操作未成功";
                }
            }

            FinanceModel model = new FinanceModel();
            model.CurrentWechatUser = CurrentWeChatUser;
            model.CurrentUser = CurrentUser;
            model.WithdrawRecords = financeManager.GetFianceWithdrawRecords(CurrentUser.UserId);

            //return View("WithdrawRecord", model);

            return Redirect("/wechat/finance/withdrawrecord");
        }

        public ActionResult History()
        {
            if (!CurrentWeChatUser.IsUserLoggedIn)
            {
                return Redirect("/wechat/account/login");
            }

            var model = new FinanceModel();

            model.FinanceRecords = financeManager.GetFinanceRecords(CurrentUser.UserId);
            model.CurrentUser = CurrentUser;
            model.CurrentWechatUser = CurrentWeChatUser;

            return View(model);
        }
    }
}
