using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    /// <summary>
    /// 微信订阅业务逻辑处理类
    /// </summary>
    public class DemandSubscriptionService
    {
        #region Constants

        #endregion Constants
        
        #region Public methods

        /// <summary>
        /// 获取用户微信订阅详细信息
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<DemandSubscription> GetSubscriptions(int pageSize, int pageIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取已订阅需求推送的用户详细订阅信息
        /// </summary>
        /// <returns></returns>
        public List<DemandSubscription> GetSubscriptionsOnlySubscribed()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户ID获取用户需求订阅信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DemandSubscription GetSubscription(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新用户需求订阅信息并返回更新后的结果
        /// </summary>
        /// <param name="subcription"></param>
        /// <returns></returns>
        public DemandSubscription UpdateSubscription(DemandSubscription subcription)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新最后推送时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastPushTimestamp(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新最后推送时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastPushTimestamp(List<int> userIds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新最后主动请求交互时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastRequestTimestamp(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添加默认的需求订阅信息当新用户关注时
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool AddDefautlSubcription(int userId)
        {
            throw new NotImplementedException();
        }

        #endregion Public methods


        #region Private methods

        /// <summary>
        /// 当用户更新订阅信息时，先删除老订阅信息，然后在添加
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool DeleteSubscritpionDetails(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添加需求订阅规则信息
        /// </summary>
        /// <param name="subscriptionDetail"></param>
        /// <returns></returns>
        private bool AddSubscrptionDetails(DemandSubscriptionDetail subscriptionDetail)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新用户订阅信息概要
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        private DemandSubscription UpdateSubscriptionInternal(DemandSubscription subscription)
        {
            throw new NotImplementedException();
        }


        #endregion Private methods

    }
}
