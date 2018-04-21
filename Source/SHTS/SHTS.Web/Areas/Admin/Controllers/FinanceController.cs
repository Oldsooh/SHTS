using SHTS.Finance;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class FinanceController : AdminBaseController
    {
        FinanceManager _financeManager = null;
        OrderService _orderManager = null;

        private FinanceManager financeManager
        {
            get
            {
                if (_financeManager == null)
                {
                    _financeManager = new FinanceManager();
                }

                return _financeManager;
            }
        }

        private OrderService orderManager
        {
            get
            {
                if (_orderManager == null)
                {
                    _orderManager = new OrderService();
                }

                return _orderManager;
            }
        }

        [Permission(EnumRole.Editer)]
        public ActionResult WithdrawRecord()
        {
            var model = new FinanceModel();

            model.WithdrawRecords = financeManager.GetFianceWithdrawRecords();

            return View(model);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult CancelWithdraw(string id)
        {
            var operationResult = new OperationResult();
            var recordId = 0;

            if (int.TryParse(id, out recordId))
            {
                var withdrawRecord = financeManager.GetWithdrawRecordDetail(recordId);
                if (withdrawRecord != null)
                {
                    if (withdrawRecord.WithdrawStatus == WithdrawStatus.Complete.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已付款完成，无法取消";
                    }
                    else if (withdrawRecord.WithdrawStatus == WithdrawStatus.Confirmed.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已被确认，正在付款过程中，无法取消";
                    }
                    else if (withdrawRecord.WithdrawStatus == WithdrawStatus.Cancelled.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已被取消，无法再次取消";
                    }
                    else
                    {
                        operationResult = financeManager.UpdateWithdrawRecordStatus(recordId, WithdrawStatus.Cancelled);
                    }
                }
                else
                {
                    operationResult.IsSuccessful = false;
                    operationResult.ErrorMessage = "该提现申请记录不存在";
                }
            }
            else
            {
                operationResult.IsSuccessful = false;
                operationResult.ErrorMessage = "参数错误，请刷新页面后重试";
            }

            var jsonData = new
            {
                IsSuccessful = operationResult.IsSuccessful,
                ErrorMessage = operationResult.ErrorMessage
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult ConfirmWithdraw(string id)
        {
            var operationResult = new OperationResult();
            var recordId = 0;

            if (int.TryParse(id, out recordId))
            {
                var withdrawRecord = financeManager.GetWithdrawRecordDetail(recordId);
                if (withdrawRecord != null)
                {
                    if (withdrawRecord.WithdrawStatus == WithdrawStatus.Complete.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已付款完成，无需确认";
                    }
                    else if (withdrawRecord.WithdrawStatus == WithdrawStatus.Cancelled.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已被取消，无需确认";
                    }
                    else if (withdrawRecord.WithdrawStatus == WithdrawStatus.Confirmed.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已被确认，正在付款过程中，无需再次确认";
                    }
                    else
                    {
                        operationResult = financeManager.UpdateWithdrawRecordStatus(recordId, WithdrawStatus.Confirmed);
                    }
                }
                else
                {
                    operationResult.IsSuccessful = false;
                    operationResult.ErrorMessage = "该提现申请记录不存在";
                }
            }
            else
            {
                operationResult.IsSuccessful = false;
                operationResult.ErrorMessage = "参数错误，请刷新页面后重试";
            }

            var jsonData = new
            {
                IsSuccessful = operationResult.IsSuccessful,
                ErrorMessage = operationResult.ErrorMessage
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult CompleteWithdraw(string id)
        {
            var operationResult = new OperationResult();
            var recordId = 0;

            if (int.TryParse(id, out recordId))
            {
                var withdrawRecord = financeManager.GetWithdrawRecordDetail(recordId);
                if (withdrawRecord != null)
                {
                    if (withdrawRecord.WithdrawStatus == WithdrawStatus.Complete.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已付款完成，无需再次确认";
                    }
                    else if (withdrawRecord.WithdrawStatus == WithdrawStatus.Cancelled.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请已被取消，无法确认完成";
                    }
                    else if (withdrawRecord.WithdrawStatus == WithdrawStatus.New.ToString())
                    {
                        operationResult.IsSuccessful = false;
                        operationResult.ErrorMessage = "该提现申请还未被确认，请先确认并付款成功后进行操作";
                    }
                    else
                    {
                        operationResult = financeManager.UpdateWithdrawRecordStatus(recordId, WithdrawStatus.Complete);
                    }
                }
                else
                {
                    operationResult.IsSuccessful = false;
                    operationResult.ErrorMessage = "该提现申请记录不存在";
                }
            }
            else
            {
                operationResult.IsSuccessful = false;
                operationResult.ErrorMessage = "参数错误，请刷新页面后重试";
            }

            var jsonData = new
            {
                IsSuccessful = operationResult.IsSuccessful,
                ErrorMessage = operationResult.ErrorMessage
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Recharge(string demandIdString, string amountString)
        {
            var isSuccessful = false;
            var errorMessage = string.Empty;
            var successDemands = new List<int>();
            var failedDemands = new List<int>();

            try
            {
                var amount = decimal.MinusOne;
                var tempDemandId = 0;
                var demandIdArray = (demandIdString ?? string.Empty).Split(',');
                var demandIdList = new List<int>();

                foreach (var idString in demandIdArray)
                {
                    if (int.TryParse(idString, out tempDemandId))
                    {
                        demandIdList.Add(tempDemandId);
                    }
                }

                if (demandIdList.Count == 0 || !decimal.TryParse(amountString, out amount))
                {
                    isSuccessful = false;
                    errorMessage = "参数错误，请刷新页面后重试";
                }
                else
                {
                    foreach (var demandId in demandIdList)
                    {
                        try
                        {
                            var demand = new DemandManager().GetDemandById(demandId, false);
                            if (demand == null || orderManager.CheckOrderForDemandBonusByDemandId(demandId))
                            {
                                failedDemands.Add(demandId);
                            }
                            else
                            {
                                var subject = "需求发布鼓励金";
                                var detail = "管理员(" + UserInfo.UserName + ")给用户(" + demand.UserId + ")发放需求鼓励金";
                                isSuccessful = financeManager.RechargeUserBalance(demandId,
                                    demand.UserId, UserInfo.UserName, amount, subject, detail);
                                if (isSuccessful)
                                {
                                    successDemands.Add(demandId);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            LogService.Log("需求鼓励金发放", ex.ToString());
                            failedDemands.Add(demandId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                errorMessage = "发放需求鼓励金遇到错误，请稍后再试";
                LogService.Log("发放需求鼓励金", ex.ToString());
            }

            var jsonData = new
            {
                IsSuccessful = true,
                ErrorMessage = errorMessage,
                FailedDemands = failedDemands.ToArray(),
                SuccessDemands = successDemands.ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}
