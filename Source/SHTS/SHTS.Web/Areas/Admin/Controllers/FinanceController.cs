using SHTS.Finance;
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

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class FinanceController : AdminBaseController
    {
        FinanceManager _financeManager = null;
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
            var demandId = 0;
            var amount = decimal.MinusOne;
            var isSuccessful = false;
            var errorMessage = string.Empty;

            if (int.TryParse(demandIdString, out demandId) && decimal.TryParse(amountString, out amount))
            {
                var demand = new DemandManager().GetDemandById(demandId);
                if (demand == null)
                {
                    isSuccessful = false;
                    errorMessage = "需求记录不存在或已被删除";
                }
                else
                {
                    var rechargeOrder = new FinanceOrder()
                    {
                        UserId = demand.UserId,
                        Amount = amount,
                        Detail = "需求鼓励金，需求ID: " + demand.Id
                    };

                    isSuccessful = financeManager.RechargeUserBalance(rechargeOrder);
                    if (!isSuccessful)
                    {
                        errorMessage = "支付需求鼓励金失败，请稍后再试";
                    }
                }
            }
            else
            {
                isSuccessful = false;
                errorMessage = "参数错误，请刷新页面后重试";
            }

            var jsonData = new
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}
