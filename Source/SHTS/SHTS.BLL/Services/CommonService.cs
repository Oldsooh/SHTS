using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    public class CommonService
    {
        private CommonDao commonDao;
        public CommonService()
        {
            commonDao = new CommonDao();
        }

        /// <summary>
        /// 查询公共右侧
        /// </summary>
        /// <returns></returns>
        public Right GetRight()
        {
            Right result = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = commonDao.SelectRight(conn);
            }
            catch (Exception e)
            {
                LogService.Log("查询公共右侧", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
    }
}
