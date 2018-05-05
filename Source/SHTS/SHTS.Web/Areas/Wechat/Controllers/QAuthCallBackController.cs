using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Wechat.Common;
using Witbird.SHTS.Web.Controllers;

namespace Witbird.SHTS.Web.Areas.Wechat.Controllers
{
    /// <summary>
    /// 单独定义这个Controller得原因是绕过WeChatBaseController的ActionExcuting的权限检查，因为他本身就是检查中的一环。不然会造成死循环，一直在请求授权。
    /// </summary>
    public class QAuthCallBackController : BaseController
    {
        //与WeChatBaseController中定义的应该一致
        public const string WeChatUserInfo = "WeChatUserInfo";
        public const string WeChatOpenIdCookieName = "WeChatOpenId";

        /// <summary>
        /// 微信授权验证返回
        /// </summary>
        /// <param name="code">微信验证code</param>
        /// <param name="state">用户原始访问地址</param>
        /// <returns></returns>

        public ActionResult CallBack(string code, string state)
        {
            ActionResult result = null;
            bool isSuccessFul = false;
            var callBackUrl = state;

            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    result = Content("您拒绝了授权");
                }
                else
                {
                    OAuthAccessTokenResult accessTokenResult = null;

                    //通过，用code换取access_token
                    accessTokenResult = OAuthApi.GetAccessToken(WeChatClient.App.AppId, WeChatClient.App.AppSecret, code);

                    //LogService.LogWexin("AccessToken请求状态", accessTokenResult.errcode.ToString());
                    if (accessTokenResult.errcode == ReturnCode.请求成功)
                    {
                        //更新用户Open id Cookie
                        Response.Cookies.Add(new HttpCookie(WeChatOpenIdCookieName, accessTokenResult.openid));
                        
                        //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
                        UserService userService = new UserService();
                        OAuthUserInfo userInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                        WeChatUser wechatUser = userService.GetWeChatUser(accessTokenResult.openid);

                        if (wechatUser == null)
                        {
                            userService.WeChatUserSubscribe(accessTokenResult.openid, false, true);
                            wechatUser = userService.GetWeChatUser(accessTokenResult.openid);
                        }
                        else
                        {
                            wechatUser.HasAuthorized = true;
                        }

                        if (wechatUser != null)
                        {
                            // Updates wechat user info
                            wechatUser.AccessToken = accessTokenResult.access_token;
                            wechatUser.AccessTokenExpired = false;
                            wechatUser.AccessTokenExpireTime = DateTime.Now.AddSeconds(accessTokenResult.expires_in);
                            wechatUser.City = userInfo.city;
                            wechatUser.County = userInfo.country;
                            wechatUser.NickName = userInfo.nickname;
                            wechatUser.Photo = userInfo.headimgurl;
                            wechatUser.Province = userInfo.province;
                            wechatUser.Sex = userInfo.sex;

                            if (userService.UpdateWeChatUser(wechatUser))
                            {
                                //Updates current wechat user
                                Session[WeChatUserInfo] = wechatUser;

                                //LogService.LogWexin("保存微信用户资料成功", "");
                                isSuccessFul = true;
                            }
                            else
                            {
                                LogService.LogWexin("保存微信用户资料失败", wechatUser.OpenId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogWexin("QAuth获取用户信息失败", ex.ToString());
                isSuccessFul = false;
            }

            if (isSuccessFul)
            {
                if (string.IsNullOrWhiteSpace(callBackUrl))
                {
                    callBackUrl = "/wechat/user/index";
                }

                // 跳转到用户一开始要进入的页面
                result = Redirect(callBackUrl);
            }
            else
            {
                // 授权失败，重新发发起用户授权
                var redirectUrl = GetUrl("/wechat/QAuthCallBack/CallBack");
                var appId = WeChatClient.App.AppId;
                var authUrl = OAuthApi.GetAuthorizeUrl(appId, redirectUrl, callBackUrl, OAuthScope.snsapi_userinfo);

                result = Redirect(authUrl);
            }

            return result;
        }

        #region 微信支付结果通知

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

        [HttpPost]
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

                    OrderService orderService = new OrderService();
                    TradeOrder order = orderService.GetOrderByOrderId(out_trade_no);

                    if (order == null)
                    {
                        return_code = "FAIL";
                        return_msg = "根据返回的订单编号(" + out_trade_no + ")未查询到相应交易订单。";
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

            string xml = string.Format(@"
<xml>
   <return_code><![CDATA[{0}]]></return_code>
   <return_msg><![CDATA[{1}]]></return_msg>
</xml>", return_code, return_msg);

            LogService.LogWexin("处理需求购买结果通知", xml);
            return Content(xml, "text/xml");
        }

        #endregion

        public ActionResult Test()
        {
            return View();
        }
        
        /// <summary>
        /// 自定义关注页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SubscribeRequired()
        {
            return View();
        }
    }
}
