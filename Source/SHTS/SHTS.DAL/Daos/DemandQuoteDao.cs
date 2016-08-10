using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.Common;

namespace Witbird.SHTS.DAL.Daos
{
    public class DemandQuoteDao
    {
        #region Constants

        const string Parameter_QuoteId = "@QuoteId";
        const string Parameter_WeChatUserId = "@WeChatUserId";
        const string Parameter_DemandId = "@DemandId";
        const string Parameter_ContactName = "@ContactName";
        const string Parameter_ContactPhoneNumber = "@ContactPhoneNumber";
        const string Parameter_QuotePrice = "@QuotePrice";
        const string Parameter_HandleStatus = "@HandleStatus";
        const string Parameter_AcceptStatus = "@AcceptStatus";
        const string Parameter_InsertedTimestamp = "@InsertedTimestamp";
        const string Parameter_LastUpdatedTimestamp = "@LastUpdatedTimestamp";
        const string Parameter_IsActive = "@IsActive";
        const string Parameter_HistoryId = "@HistoryId";
        const string Parameter_HasRead = "@HasRead";
        const string Parameter_Comments = "@Comments";

        const string SP_InsertOrUpdateDemandQuote = "sp_InsertOrUpdateDemandQuote";
        const string SP_InsertDemandQuoteHistory = "sp_InsertDemandQuoteHistory";
        const string SP_SelectDemandQuoteWithoutHistories = "sp_SelectDemandQuoteWithoutHistories";
        const string SP_SelectDemandQuoteWithHistories = "sp_SelectDemandQuoteWithHistories";
        const string SP_DeleteDemandQuote = "sp_DeleteDemandQuote";
        const string SP_SelectDemandQuotesByDemandId = "sp_SelectDemandQuotesByDemandId";
        const string SP_SelectDemandQuotesByWeChatUserId = "sp_SelectDemandQuotesByWeChatUserId";
        const string SP_SelectDemandQuoteByDemandIdAndWeChatUserId = "sp_SelectDemandQuoteByDemandIdAndWeChatUserId";

        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts or update demand quote entity and return.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public DemandQuote InsertOrUpdateDemandQuote(SqlConnection conn, DemandQuote quote)
        {
            var quoteId = quote.QuoteId;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_QuoteId, quote.QuoteId),
                new SqlParameter(Parameter_WeChatUserId, quote.WeChatUserId),
                new SqlParameter(Parameter_DemandId, quote.DemandId),
                new SqlParameter(Parameter_ContactName, quote.ContactName),
                new SqlParameter(Parameter_ContactPhoneNumber, quote.ContactPhoneNumber),
                new SqlParameter(Parameter_QuotePrice, quote.QuotePrice),
                new SqlParameter(Parameter_HandleStatus, quote.HandleStatus),
                new SqlParameter(Parameter_AcceptStatus, quote.AcceptStatus),
                new SqlParameter(Parameter_InsertedTimestamp, quote.InsertedTimestamp),
                new SqlParameter(Parameter_LastUpdatedTimestamp, quote.LastUpdatedTimestamp),
                new SqlParameter(Parameter_IsActive, quote.IsActive)
            };

            using (var reader = DBHelper.RunProcedure(conn, SP_InsertOrUpdateDemandQuote, parameters))
            {
                while(reader.Read())
                {
                    quoteId = reader[0].DBToInt32();
                }
            }

            quote.QuoteId = quoteId;

            return quote;
        }

        /// <summary>
        /// Inserts demand quote history.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public bool InsertDemandQuoteHistory(SqlConnection conn, DemandQuoteHistory history)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_QuoteId, history.QuoteId),
                new SqlParameter(Parameter_WeChatUserId, history.WeChatUserId),
                new SqlParameter(Parameter_Comments, history.Comments),
                new SqlParameter(Parameter_HasRead, history.HasRead),
                new SqlParameter(Parameter_InsertedTimestamp, history.InsertedTimestamp),
                new SqlParameter(Parameter_IsActive, history.IsActive),
            };

            return DBHelper.RunNonQueryProcedure(conn, SP_InsertDemandQuoteHistory, parameters) > 0;
        }

        /// <summary>
        /// Selects demand quote by quoteId.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="quoteId"></param>
        /// <param name="isSelectHistories"></param>
        /// <returns></returns>
        public DemandQuote SelectDemandQuoteByQuoteId(SqlConnection conn, int quoteId, bool isSelectHistories)
        {
            DemandQuote quote = null;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_QuoteId, quoteId)
            };

            string spName = string.Empty;

            if (isSelectHistories)
            {
                spName = SP_SelectDemandQuoteWithHistories;
            }
            else
            {
                // Selects quote history will update its HasRead status to true.
                spName = SP_SelectDemandQuoteWithoutHistories;
            }

            using (var reader = DBHelper.RunProcedure(conn, spName, parameters))
            {
                while (reader.Read())
                {
                    quote = ConvertToDemandQuote(reader);
                }

                if (reader.NextResult())
                {
                    while(reader.Read())
                    {
                        var history = ConvertToDemandQuoteHistory(reader);
                        quote.QuoteHistories.Add(history);
                    }
                }
            }

            return quote;
        }

        /// <summary>
        /// Selects all demand quotes by demand id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public List<DemandQuote> SelectDemandQuotesByDemandId(SqlConnection conn, int demandId)//, int pageSize, int pageIndex, out int totalCount)
        {
            var quotes = new List<DemandQuote>();
            var quoteHistories = new List<DemandQuoteHistory>();
            //totalCount = 0;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_DemandId, demandId)//,
                //new SqlParameter("@PageIndex", pageIndex),
                //new SqlParameter("@PageSize", pageSize)
            };

            using (var reader = DBHelper.RunProcedure(conn, SP_SelectDemandQuotesByDemandId, parameters))
            {
                //while (reader.Read())
                //{
                //    //totalCount = reader["TotalCount"].DBToInt32();
                //}

                //if (reader.NextResult())
                //{
                    while (reader.Read())
                    {
                        var quote = ConvertToDemandQuote(reader);
                        quotes.Add(quote);
                    }
                //}

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        var history = ConvertToDemandQuoteHistory(reader);
                        var quote = quotes.FirstOrDefault(x => x.QuoteId == history.QuoteId);
                        if (quote.IsNotNull())
                        {
                            quote.QuoteHistories.Add(history);
                        }
                    }
                }
            }

            return quotes;
        }

        /// <summary>
        /// Selects all demand quotes by wechat user id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public List<DemandQuote> SelectDemandQuotesByWechatUserId(SqlConnection conn, int wechatUserId, int pageSize, int pageIndex, out int totalCount)
        {
            var quotes = new List<DemandQuote>();
            var demands = new List<Demand>();
            totalCount = 0;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_WeChatUserId, wechatUserId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize)
            };

            var dts = DBHelper.GetMuiltiDataFromDB(conn, SP_SelectDemandQuotesByWeChatUserId, parameters);

            totalCount = Int32.Parse(dts["0"].Rows[0][0].ToString());
            quotes = DBHelper.DataTableToList<DemandQuote>(dts["1"]);
            demands = DBHelper.DataTableToList<Demand>(dts["2"]);

            if (quotes.HasItem() && demands.HasItem())
            {
                foreach (var quote in quotes)
                {
                    var demand = demands.FirstOrDefault(x => x.Id == quote.DemandId);
                    if (demand.IsNotNull())
                    {
                        quote.Demand = demand;
                    }
                }
            }

            return quotes;
        }

        /// <summary>
        /// Selects all demand quotes by wechat user id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public List<Demand> SelectRecievedQuotes(SqlConnection conn, int wechatUserId, int pageSize, int pageIndex, out int totalCount)
        {
            var quotes = new List<DemandQuote>();
            var demands = new List<Demand>();
            totalCount = 0;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_WeChatUserId, wechatUserId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize)
            };

            var dts = DBHelper.GetMuiltiDataFromDB(conn, "sp_SelectRecievedQuotes", parameters);

            totalCount = Int32.Parse(dts["0"].Rows[0][0].ToString());
            demands = DBHelper.DataTableToList<Demand>(dts["1"]);
            quotes = DBHelper.DataTableToList<DemandQuote>(dts["2"]);

            if (quotes.HasItem() && demands.HasItem())
            {
                foreach (var quote in quotes)
                {
                    var demand = demands.FirstOrDefault(x => x.Id == quote.DemandId);
                    if (demand.IsNotNull())
                    {
                        demand.QuoteEntities.Add(quote);
                    }
                }
            }

            return demands;
        }

        public DemandQuote SelectDemandQuoteByDemandIdAndWeChatUserId(SqlConnection conn, int demandId, int wechatUserId)
        {
            DemandQuote quote = null;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_DemandId, demandId),
                new SqlParameter(Parameter_WeChatUserId, wechatUserId)
            };

            using (var reader = DBHelper.RunProcedure(conn, SP_SelectDemandQuoteByDemandIdAndWeChatUserId, parameters))
            {
                while (reader.Read())
                {
                    quote = ConvertToDemandQuote(reader);
                }
            }

            return quote;
        }

        /// <summary>
        /// Sets demand quote and histories to delete status(set IsActive to false).
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public bool DeleteDemandQuote(SqlConnection conn, int quoteId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_QuoteId, quoteId)
            };

            return DBHelper.RunNonQueryProcedure(conn, SP_DeleteDemandQuote, parameters) > 0;
        }

        public bool UpdateAllQuotesStatus(SqlConnection conn, int demandId, int quoteId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(Parameter_DemandId, demandId),
                new SqlParameter(Parameter_QuoteId, quoteId)
            };

            return DBHelper.RunNonQueryProcedure(conn, "sp_UpdateAllQuotesStatus", parameters) > 0;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts object to DemandQuote entity.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DemandQuote ConvertToDemandQuote(SqlDataReader reader)
        {
            return new DemandQuote()
            {
                AcceptStatus = reader["AcceptStatus"].DBToString(),
                ContactName = reader["ContactName"].DBToString(),
                ContactPhoneNumber = reader["ContactPhoneNumber"].DBToString(),
                DemandId = reader["DemandId"].DBToInt32(),
                HandleStatus = reader["HandleStatus"].DBToBoolean(),
                InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime().Value,
                IsActive = reader["IsActive"].DBToBoolean(),
                LastUpdatedTimestamp = reader["LastUpdatedTimestamp"].DBToDateTime().Value,
                QuoteId = reader["QuoteId"].DBToInt32(),
                QuotePrice = reader["QuotePrice"].DBToDecimal(),
                WeChatUserId = reader["WeChatUserId"].DBToInt32()
            };
        }

        /// <summary>
        /// Converts object to DemandQuoteHistory entity.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DemandQuoteHistory ConvertToDemandQuoteHistory(SqlDataReader reader)
        {
            return new DemandQuoteHistory()
            {
                Comments = reader["Comments"].DBToString(),
                HasRead = reader["HasRead"].DBToBoolean(),
                HistoryId = reader["HistoryId"].DBToInt32(),
                InsertedTimestamp = reader["InsertedTimestamp"].DBToDateTime().Value,
                IsActive = reader["IsActive"].DBToBoolean(),
                QuoteId = reader["QuoteId"].DBToInt32(),
                WeChatUserId = reader["WeChatUserId"].DBToInt32()
            };
        }

        #endregion
    }
}
