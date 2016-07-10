using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    /// <summary>
    /// 单页实体类型
    /// </summary>
    public enum EnumEntityType
    {
        /// <summary>
        /// about
        /// </summary>
        About = 1,
        /// <summary>
        /// news
        /// </summary>
        News = 2
    }

    public enum EnumNews
    {
        /// <summary>
        /// 网站公告
        /// </summary>
        Notice = 1,
        /// <summary>
        /// 公司新闻
        /// </summary>
        Company = 2,
        /// <summary>
        /// 行业新闻
        /// </summary>
        Industry = 3,
        /// <summary>
        /// 资源新闻
        /// </summary>
        Resource = 4,
        /// <summary>
        /// 供求新闻
        /// </summary>
        Supplydemand = 5
    }


    /// <summary>
    /// 确定由来支付中介手续费
    /// </summary>
    public enum Payer
    {
        /// <summary>
        /// 卖家支付
        /// </summary>
        Seller,
        /// <summary>
        /// 买家支付
        /// </summary>
        Buyer,
        /// <summary>
        /// 双方平摊
        /// </summary>
        Both
    }

    /// <summary>
    /// 中介申请状态
    /// </summary>
    public enum TradeState
    {
        /// <summary>
        /// 中介申请
        /// </summary>
        New,
        /// <summary>
        /// 正在处理
        /// </summary>
        InProgress,
        /// <summary>
        /// 交易完成
        /// </summary>
        Completed,
        /// <summary>
        /// 交易终止
        /// </summary>
        Finished,
        /// <summary>
        /// 违规交易
        /// </summary>
        Invalid
    }

    /// <summary>
    /// 订单支付状态
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// 新创建的订单，等待支付
        /// </summary>
        New,
        /// <summary>
        /// 订单支付成功
        /// </summary>
        Succeed,
        /// <summary>
        /// 订单支付失败
        /// </summary>
        Failed,
        /// <summary>
        /// 无效的支付订单
        /// </summary>
        Invalid
    }

    /// <summary>
    /// 订单类型，中介交易订单,升级为VIP订单, 微信购买需求联系方式订单
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 中介交易订单
        /// </summary>
        Trade,
        /// <summary>
        /// 升级为VIP会员订单
        /// </summary>
        ToVip,
        /// <summary>
        /// 微信用户购买查看需求联系方式的永久权限
        /// </summary>
        WeChatDemand
    }

    /// <summary>
    /// 资源状态
    /// </summary>
    public enum ResourceState : int
    {
        /// <summary>
        /// 创建
        /// </summary>
        Created = 1,

        /// <summary>
        /// 审核通过
        /// </summary>
        Approved = 2,

        /// <summary>
        /// 被删除
        /// </summary>
        Deleted = 3
    }

    /// <summary>
    /// 会员等级状态
    /// </summary>
    public enum VipState
    {
        /// <summary>
        /// 普通会员
        /// </summary>
        Normal,
        /// <summary>
        /// 认证会员，可以申请VIP会员
        /// </summary>
        Identified,
        /// <summary>
        /// 认证图片审核失败，需要重新审核，相当于普通会员
        /// </summary>
        Invalid,
        /// <summary>
        /// VIP会员
        /// </summary>
        VIP
    }

    /// <summary>
    /// Demand status
    /// </summary>
    public enum DemandStatus : int
    {
        /// <summary>
        /// In progress
        /// </summary>
        InProgress = 1,
        /// <summary>
        /// Complete
        /// </summary>
        Complete = 2
    }

    /// <summary>
    /// Demand subscription types
    /// </summary>
    public enum DemandSubscriptionType
    {
        /// <summary>
        /// By demand category
        /// </summary>
        Category,
        /// <summary>
        /// By specified location area
        /// </summary>
        Area,
        /// <summary>
        /// By specified keywords
        /// </summary>
        Keywords
    }

    /// <summary>
    /// Demand quotes status
    /// </summary>
    public enum DemandQuoteStatus
    {
        /// <summary>
        /// Waiting for response
        /// </summary>
        Wait,
        /// <summary>
        /// Accept quoting.
        /// </summary>
        Accept,
        /// <summary>
        /// Don't agree quote.
        /// </summary>
        Denied
    }
}
