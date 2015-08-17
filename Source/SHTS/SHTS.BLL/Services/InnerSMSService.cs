using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    /// <summary>
    /// 短信管理
    /// </summary>
    public class InnerSMSService
    {
        private SMSDao smsDao;

        public InnerSMSService()
        {
            smsDao = new SMSDao();
        }

        /// <summary>
        /// 记录短信发送。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public void AddSMSRecord(ShortMessage message)
        {
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                smsDao.AddSMSRecord(conn,message);
            }
            catch (Exception e)
            {
                LogService.Log("记录短信发送", e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public void AddSMSRecordAsync(ShortMessage message)
        {
            ThreadPool.QueueUserWorkItem(delegate(object state) 
            {
                AddSMSRecord(message);
            });
        }
    }
}
