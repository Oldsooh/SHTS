using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Common;
using System.Transactions;
using Witbird.SHTS.DAL.Daos;

namespace Witbird.SHTS.BLL.Services
{
    /// <summary>
    /// 微信报价业务逻辑处理类
    /// </summary>
    public class DemandQuoteService
    {
        #region Constants

        private DemandQuoteRepository quoteRepository = new DemandQuoteRepository();
        private DemandQuoteHistoryRepository historyRepository = new DemandQuoteHistoryRepository();

        private DemandQuoteDao quoteDao = new DemandQuoteDao();

        #endregion Constants

        #region Public methods

        /// <summary>
        /// Adds new demand quote with comments.
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public DemandQuote NewQuoteRecord(DemandQuote quote)
        {
            ParameterChecker.Check(quote, "Quote");
            ParameterChecker.Check(quote.ContactName, "Contact Name");
            ParameterChecker.Check(quote.ContactPhoneNumber, "Contact Phone Numer");
            var conn = DBHelper.GetSqlConnection();

            try
            {
                var currentTime = DateTime.Now;

                using (TransactionScope scope = new TransactionScope())
                {
                    conn.Open();
                    quote.InsertedTimestamp = currentTime;
                    quote.LastUpdatedTimestamp = currentTime;
                    quote.HandleStatus = false;
                    quote.AcceptStatus = DemandQuoteStatus.Wait.ToString();
                    quote.IsActive = true;

                    quote = quoteDao.InsertOrUpdateDemandQuote(conn, quote);
                    if (quote.QuoteHistories.HasItem())
                    {
                        foreach (var item in quote.QuoteHistories)
                        {
                            if (item.Operation == Operation.Add)
                            {
                                item.Operation = Operation.None;
                                item.QuoteId = quote.QuoteId;
                                item.InsertedTimestamp = currentTime;
                                item.HasRead = false;
                                item.IsActive = true;

                                quoteDao.InsertDemandQuoteHistory(conn, item);
                            }
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                LogService.Log("Adds new demand quote with comments.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return quote;
        }

        /// <summary>
        /// Updates demand quote with comments.
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public DemandQuote UpdateQuoteRecord(DemandQuote quote)
        {
            ParameterChecker.Check(quote, "Quote");
            ParameterChecker.Check(quote.ContactName, "Contact Name");
            ParameterChecker.Check(quote.ContactPhoneNumber, "Contact Phone Numer");
            var conn = DBHelper.GetSqlConnection();

            try
            {
                var currentTime = DateTime.Now;

                using (TransactionScope scope = new TransactionScope())
                {
                    conn.Open();
                    quote.LastUpdatedTimestamp = currentTime;
                    quote = quoteDao.InsertOrUpdateDemandQuote(conn, quote);

                    if (quote.QuoteHistories.HasItem())
                    {
                        foreach (var item in quote.QuoteHistories)
                        {
                            if (item.Operation == Operation.Add)
                            {
                                item.QuoteId = quote.QuoteId;
                                item.InsertedTimestamp = currentTime;
                                item.HasRead = false;
                                item.IsActive = true;

                                quoteDao.InsertDemandQuoteHistory(conn, item);
                            }
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                LogService.Log("Updates demand quote with comments.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return quote;
        }

        /// <summary>
        /// Adds new demand comments.
        /// </summary>
        /// <param name="quoteHistory"></param>
        /// <returns></returns>
        public bool NewQuoteHistory(DemandQuoteHistory quoteHistory)
        {
            ParameterChecker.Check(quoteHistory, "Quote History");
            ParameterChecker.Check(quoteHistory.Comments, "Quote History comments");
            var isSuccessful = false;

            var conn = DBHelper.GetSqlConnection();

            try
            {
                var currentTime = DateTime.Now;

                using (TransactionScope scope = new TransactionScope())
                {
                    conn.Open();
                    var parentQuote = quoteDao.SelectDemandQuoteByQuoteId(conn, quoteHistory.QuoteId, false);

                    if (parentQuote.IsNotNull())
                    {
                        // Updates parent quote.
                        parentQuote.LastUpdatedTimestamp = currentTime;
                        quoteDao.InsertOrUpdateDemandQuote(conn, parentQuote);

                        quoteHistory.InsertedTimestamp = currentTime;
                        quoteHistory.HasRead = false;
                        quoteHistory.IsActive = true;

                        // Inserts quote history entity.
                        isSuccessful = quoteDao.InsertDemandQuoteHistory(conn, quoteHistory);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                LogService.Log("Failed to add new demand comments.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return isSuccessful;
        }

        /// <summary>
        /// Deletes quote and corresponding histories.
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public bool DeleteQuoteAndCommentsHistories(int quoteId)
        {
            var isSuccessful = false;
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                isSuccessful = quoteDao.DeleteDemandQuote(conn, quoteId);
            }
            catch (Exception ex)
            {
                LogService.Log("Deletes quote and corresponding histories.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return isSuccessful;
        }

        /// <summary>
        /// Selects quotes without histories for one specified demand.
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        //public List<DemandQuote> GetAllDemandQuotesForOneDemand(int demandId, int pageSize, int pageIndex, out int totalCount)
        //{
        //    var quotes = new List<DemandQuote>();
        //    var conn = DBHelper.GetSqlConnection();
        //    totalCount = 0;

        //    try
        //    {
        //        conn.Open();
        //        quotes = quoteDao.SelectDemandQuotesByDemandId(conn, demandId, pageSize, pageIndex, out totalCount);

        //        if (quotes.HasItem())
        //        {
        //            foreach (var quote in quotes)
        //            {
        //                var history = GetDemandQuote(quote.QuoteId, true);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Log("Failed to select quotes without histories for one demand.", ex.ToString());
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //    return quotes;
        //}

        /// <summary>
        /// Selects quotes without histories for one specified demand.
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<DemandQuote> GetAllDemandQuotesForOneDemand(int demandId)
        {
            var quotes = new List<DemandQuote>();
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                quotes = quoteDao.SelectDemandQuotesByDemandId(conn, demandId);

                if (quotes.HasItem())
                {
                    foreach (var quote in quotes)
                    {
                        var history = GetDemandQuote(quote.QuoteId, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Log("Failed to select quotes without histories for one demand.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return quotes;
        }

        /// <summary>
        /// Selects all demand quotes which posted by speficied user.
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<DemandQuote> GetPostedQuotes(int wechatUserId, int pageSize, int pageIndex, out int totalCount)
        {
            var quotes = new List<DemandQuote>();
            var conn = DBHelper.GetSqlConnection();
            totalCount = 0;

            try
            {
                conn.Open();
                quotes = quoteDao.SelectDemandQuotesByWechatUserId(conn, wechatUserId, pageSize, pageIndex, out totalCount);
            }
            catch (Exception ex)
            {
                LogService.Log("Selects all demand quotes which posted by speficied user.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return quotes;
        }

        /// <summary>
        /// Selects all demand which has qoute record.
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Demand> GetRecievedQuotes(int wechatUserId, int pageSize, int pageIndex, out int totalCount)
        {
            var demands = new List<Demand>();
            var conn = DBHelper.GetSqlConnection();
            totalCount = 0;

            try
            {
                conn.Open();
                demands = quoteDao.SelectRecievedQuotes(conn, wechatUserId, pageSize, pageIndex, out totalCount);
            }
            catch (Exception ex)
            {
                LogService.Log("Selects all demand quotes which posted by speficied user.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return demands;
        }

        /// <summary>
        /// Selects demand quote by demandId and wechatuserid.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public DemandQuote SelectDemandQuoteByDemandIdAndWeChatUserId(int demandId, int wechatUserId)
        {
            DemandQuote quote = null;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                quote = quoteDao.SelectDemandQuoteByDemandIdAndWeChatUserId(conn, demandId, wechatUserId);
            }
            catch (Exception ex)
            {
                LogService.Log("SelectDemandQuoteByDemandIdAndWeChatUserId", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return quote;
        }

        /// <summary>
        /// Selects demand quote with details.
        /// </summary>
        /// <returns></returns>
        public DemandQuote GetDemandQuote(int quoteId, bool searchHistories)
        {
            DemandQuote quote = null;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                quote = quoteDao.SelectDemandQuoteByQuoteId(conn, quoteId, searchHistories);
            }
            catch (Exception ex)
            {
                LogService.Log("Selects demand quote with details.", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return quote;
        }

        public bool UpdateAllQuotesStatus(int demandId, int quoteId)
        {
            var isSuccessFul = false;
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                isSuccessFul = quoteDao.UpdateAllQuotesStatus(conn, demandId, quoteId);
            }
            catch(Exception ex)
            {
                LogService.Log("UpdateAllQuotesStatus", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return isSuccessFul;
        }

        #endregion Public methods

        #region Private methods

        #endregion Private methods
    }
}
