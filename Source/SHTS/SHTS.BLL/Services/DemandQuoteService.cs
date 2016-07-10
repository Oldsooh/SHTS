using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Service
{
    /// <summary>
    /// 微信报价业务逻辑处理类
    /// </summary>
    public class DemandQuoteService
    {
        #region Constants

        #endregion Constants

        #region Public methods

        /// <summary>
        /// 新建报价
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public DemandQuotes NewQuoteRecord(DemandQuotes quote)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改报价状态及回复留言
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public DemandQuotes UpdateQuoteRecordWithComments(DemandQuotes quote)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 回复报价记录
        /// </summary>
        /// <param name="quoteHistory"></param>
        /// <returns></returns>
        public bool NewQuoteComments(DemandQuoteHistory quoteHistory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除报价记录及回复记录
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public bool DeleteQuoteAndCommentsHistories(int quoteId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取报价详细信息及历史回复
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public DemandQuotes GetQuoteDetails(int demandId, int pageSize, int pageIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户ID获取发出的报价记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<DemandQuotes> GetAllQuotes(int userId, int pageSize, int pageIndex)
        {
            throw new NotImplementedException();
        }

        #endregion Public methods

        #region Private methods

        #endregion Private methods
    }
}
