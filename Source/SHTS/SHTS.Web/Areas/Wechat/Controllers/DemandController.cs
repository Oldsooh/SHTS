using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Models;
using Witbird.SHTS.Web.Public;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    public class DemandController : WechatBaseController
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

        DemandManager demandManager = new DemandManager();
        DemandService demandService = new DemandService();
        OrderService orderService = new OrderService();
        CityService cityService = new CityService();

        public ActionResult Index(string page)
        {
            //LogService.LogWexin("DemandIndex", "Enter");
            DemandModel model = new DemandModel();
            model.DemandCategories = demandManager.GetDemandCategories();

            string city = string.Empty;
            if (Session["CityId"] != null)
            {
                city = Session["CityId"].ToString();
            }
            //LogService.LogWexin("DemandIndex", "City = " + city);
            //页码，总数重置
            int pageIndex = 1;
            if (!string.IsNullOrEmpty(page))
            {
                Int32.TryParse(page, out pageIndex);
            }
            int allCount = 0;
            if (string.IsNullOrEmpty(city))
            {
                model.Demands = demandService.GetDemands(10, pageIndex, out allCount);//每页显示10条
            }
            else
            {
                model.Demands = demandService.GetDemandsByCity(10, pageIndex, city, out allCount);//每页显示10条
            }
            //LogService.LogWexin("DemandIndex", "Demands.Count = " + model.Demands.Count);
            //分页
            if (model.Demands != null && model.Demands.Count > 0)
            {
                model.PageIndex = pageIndex;//当前页数
                model.PageSize = 10;//每页显示多少条
                model.PageStep = 5;//每页显示多少页码
                model.AllCount = allCount;//总条数
                if (model.AllCount % model.PageSize == 0)
                {
                    model.PageCount = model.AllCount / model.PageSize;
                }
                else
                {
                    model.PageCount = model.AllCount / model.PageSize + 1;
                }
            }
            //LogService.LogWexin("DemandIndex", "End");
            return View(model);
        }


        public ActionResult Show(string id)
        {
            DemandModel model = new DemandModel();

            int tempId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                Int32.TryParse(id, out tempId);
            }

            if (tempId != 0)
            {
                bool hasWeChatUserBoughtForDemand = false;
                var demand = demandService.GetDemandById(tempId);

                if (demand != null)
                {
                    hasWeChatUserBoughtForDemand = demandService.HasWeChatUserBoughtForDemand(CurrentWeChatUser.OpenId, demand.Id);

                    if (!hasWeChatUserBoughtForDemand)
                    {
                        demand.Phone = "未购买查看权限";
                        demand.QQWeixin = "未购买查看权限";
                        demand.Email = "未购买查看权限";
                        demand.Address = "未购买查看权限";
                    }
                }

                model.HasCurrentWeChatUserBought = hasWeChatUserBoughtForDemand;
                model.Demand = demand;
            }

            if (model.Demand == null)
            {
                model.Demand = new Demand();
                model.Demand.Title = "该需求不存在或已被删除";
                model.Demand.StartTime = DateTime.Now;
                model.Demand.EndTime = DateTime.Now;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!IsUserLogin)
            {
                return Redirect("/Wechat/account/login");
            }
            DemandModel model = new DemandModel();
            model.DemandCategories = demandManager.GetDemandCategories();
            model.Provinces = cityService.GetProvinces(true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(Demand demand)
        {
            string result = "发布失败";
            if (!IsUserLogin)
            {
                result = "长时间为操作，请重新登录";
            }
            else if (demand != null)
            {
                if (!string.IsNullOrEmpty(demand.Title) &&
                    !string.IsNullOrEmpty(demand.ContentText) &&
                    demand.CategoryId != 0)
                {
                    User user = Session[USERINFO] as User;
                    demand.UserId = user.UserId;
                    demand.IsActive = true;
                    demand.InsertTime = DateTime.Now;
                    demand.StartTime = demand.InsertTime;
                    demand.EndTime = demand.StartTime;
                    demand.ContentStyle = demand.ContentText;
                    if (demand.ContentText.Length > 291)
                    {
                        demand.Description = demand.ContentText.Substring(0, 290);
                    }
                    else
                    {
                        demand.Description = demand.ContentText;
                    }
                    if (demand.Budget == null)
                    {
                        demand.Budget = 0;
                    }
                    if (demandManager.AddDemand(demand))
                    {
                        result = "success";
                    }
                }
                else
                {
                    result = "必须项不能为空";
                }
            }
            return Content(result);
        }

        /// <summary>
        /// 购买需求查看权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Buy(int id)
        {
            bool isSuccessFul = false;
            string message = "";

            string appId = string.Empty;
            string timeStamp = string.Empty;
            string nonceStr = string.Empty;
            string package = string.Empty;
            string paySign = string.Empty;
            
            try
            {
                Demand demand = demandService.GetDemandById(id);

                if (demand == null)
                {
                    message = "该需求不存在或已被删除，请刷新页面后重试。";
                }
                else
                {
                    bool hasWeChatUserBoughtForDemand = demandService.HasWeChatUserBoughtForDemand(CurrentWeChatUser.OpenId, id);

                    if (hasWeChatUserBoughtForDemand)
                    {
                        message = "您已购买该需求联系方式及详细地址的永久查看权限，无需重复购买。请刷新页面后查看。";
                    }
                    else
                    {
                        TradeOrder order = orderService.GetOrderByOpenIdAndDemandIdForWeChatClient(CurrentWeChatUser.OpenId, id);

                        if (order == null)
                        {
                            string orderId = orderService.GenerateNewOrderNumber();
                            string subject = "微信用户购买需求联系方式永久权限";
                            string body = "微信用户" + CurrentWeChatUser.NickName + "购买需求（" + demand.Title + "）联系方式的永久查看权限";
                            string username = CurrentWeChatUser.OpenId;
                            decimal amount = 1m;
                            int state = (int)OrderState.New;
                            string resourceUrl = "http://" + StaticUtility.Config.Domain + "/wechat/demand/show/" + id;

                            isSuccessFul = orderService.AddNewOrder(
                                orderId, subject, body, amount, state, username, resourceUrl, (int)OrderType.WeChatDemand, id);
                        }
                        else
                        {
                            isSuccessFul = true;
                        }

                        if (isSuccessFul)
                        {
                            isSuccessFul = PreparePaySign(order, out appId, out timeStamp, out nonceStr, out package, out paySign);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("购买需求联系方式", ex.ToString());
                isSuccessFul = false;
                message = "购买失败，请稍后再试！";
            }

            var jsonResult = new
            {
                IsSuccessFul = isSuccessFul,
                Message = message,
                AppId = appId,
                TimeStamp = timeStamp,
                NonceStr = nonceStr,
                Package = package,
                PaySign = paySign
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuyDemandPayNotifyUrl()
        {
            ResponseHandler resHandler = new ResponseHandler(null);

            string return_code = resHandler.GetParameter("return_code");
            string return_msg = resHandler.GetParameter("return_msg");

            try
            {
                resHandler.SetKey(TenPayV3Info.Key);
                //验证请求是否从微信发过来（安全）
                if (resHandler.IsTenpaySign())
                {
                    //正确的订单处理

                    string out_trade_no = resHandler.GetParameter("out_trade_no");
                    string total_fee = resHandler.GetParameter("total_fee");
                    //微信支付订单号
                    string transaction_id = resHandler.GetParameter("transaction_id");
                    //支付完成时间
                    string time_end = resHandler.GetParameter("time_end");

                    LogService.LogWexin("处理需求购买结果通知", "订单号：" + out_trade_no + "   交易流水号：" + transaction_id + "    支付完成时间：" + time_end);

                    TradeOrder order = orderService.GetOrderByOrderId(out_trade_no);

                    if (order == null)
                    {
                        return_code = "FAIL";
                        return_msg = "根据返回的订单编号(" + out_trade_no  +")未查询到相应交易订单。";
                    }
                    else if (order.State == (int)OrderState.Succeed)
                    {
                        return_code = "SUCCESS";
                        return_msg = "OK";
                    }
                    else if ((order.Amount * 100) != Convert.ToDecimal(total_fee))
                    {
                        //无效支付结果
                        orderService.UpdateOrderState(order.OrderId, (int)OrderState.Invalid);

                        return_code = "FAIL";
                        return_msg = "交易金额与订单金额不一致";
                    }
                    else
                    {
                        //交易成功
                        if (orderService.UpdateOrderState(order.OrderId, (int)OrderState.Succeed))
                        {
                            return_code = "SUCCESS";
                            return_msg = "OK";
                        }
                        else
                        {
                            return_code = "FAIL";
                            return_msg = "更新订单失败";
                        }
                    }
                }
                else
                {
                    return_code = "FAIL";
                    return_msg = "非法支付结果通知";

                    //错误的订单处理
                    LogService.LogWexin("接收到非法微信支付结果通知", return_msg);
                }
            }
            catch (Exception ex)
            {
                return_code = "FAIL";
                return_msg = ex.ToString();

                LogService.LogWexin("处理需求购买结果通知", ex.ToString());
            }

            string xml = string.Format(@"<xml>
   <return_code><![CDATA[{0}]]></return_code>
   <return_msg><![CDATA[{1}]]></return_msg>
</xml>", return_code, return_msg);

            LogService.LogWexin("处理需求购买结果通知", xml);
            return Content(xml, "text/xml");
        }

        private bool PreparePaySign(TradeOrder order, out string appId, out string timeStamp, out string nonceStr, out string package, out string paySign)
        {
            bool isSuccessFul = false;

            appId = string.Empty;
            timeStamp = string.Empty;
            nonceStr = string.Empty;
            package = string.Empty;
            paySign = string.Empty;

            try
            {

                //创建支付应答对象
                RequestHandler packageReqHandler = new RequestHandler(null);
                //初始化
                packageReqHandler.Init();

                appId = TenPayV3Info.AppId;
                timeStamp = TenPayV3Util.GetTimestamp();
                nonceStr = TenPayV3Util.GetNoncestr();

                //设置package订单参数
                packageReqHandler.SetParameter("appid", TenPayV3Info.AppId);		  //公众账号ID
                packageReqHandler.SetParameter("mch_id", TenPayV3Info.MchId);		  //商户号
                packageReqHandler.SetParameter("nonce_str", nonceStr);                    //随机字符串
                packageReqHandler.SetParameter("body", order.Body);    //商品信息
                packageReqHandler.SetParameter("out_trade_no", order.OrderId);		//商家订单号
                packageReqHandler.SetParameter("total_fee", (Convert.ToInt32(order.Amount * 100).ToString()));			        //商品金额,以分为单位(money * 100).ToString()
                packageReqHandler.SetParameter("spbill_create_ip", Request.UserHostAddress);   //用户的公网ip，不是商户服务器IP
                packageReqHandler.SetParameter("notify_url", TenPayV3Info.TenPayV3Notify);		    //接收财付通通知的URL
                packageReqHandler.SetParameter("trade_type", TenPayV3Type.JSAPI.ToString());	                    //交易类型
                packageReqHandler.SetParameter("openid", CurrentWeChatUser.OpenId);	                    //用户的openId

                string sign = packageReqHandler.CreateMd5Sign("key", TenPayV3Info.Key);
                packageReqHandler.SetParameter("sign", sign);	                    //签名

                string data = packageReqHandler.ParseXML();

                var result = TenPayV3.Unifiedorder(data);
                var res = XDocument.Parse(result);
                string prepayId = res.Element("xml").Element("prepay_id").Value;

                //设置支付参数
                RequestHandler paySignReqHandler = new RequestHandler(null);
                paySignReqHandler.SetParameter("appId", TenPayV3Info.AppId);
                paySignReqHandler.SetParameter("timeStamp", timeStamp);
                paySignReqHandler.SetParameter("nonceStr", nonceStr);
                paySignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
                paySignReqHandler.SetParameter("signType", "MD5");

                paySign = paySignReqHandler.CreateMd5Sign("key", TenPayV3Info.Key);
                package = string.Format("prepay_id={0}", prepayId);
            }
            catch (Exception ex)
            {
                LogService.LogWexin("构建支付参数出错", ex.ToString());
            }

            if (!string.IsNullOrEmpty(appId) &&
                !string.IsNullOrEmpty(timeStamp) &&
                !string.IsNullOrEmpty(nonceStr) &&
                !string.IsNullOrEmpty(package) &&
                !string.IsNullOrEmpty(paySign))
            {
                isSuccessFul = true;
            }

            return isSuccessFul;
        }
    }
}
