using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;
using WitBird.Common;

namespace Witbird.SHTS.BLL.Services
{
    public class DemandService
    {
        private DemandDao demandDao;

        public DemandService()
        {
            demandDao = new DemandDao();
        }

        /// <summary>
        /// 查询需求列表
        /// </summary>
        public List<Demand> GetDemands(int pageCount, int pageIndex, out int count)
        {
            List<Demand> result = new List<Demand>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = demandDao.SelectDemands(pageCount, pageIndex, out tempCount, conn);
                count = tempCount;
                if (result != null && result.Count > 0)
                {
                    List<DemandCategory> categories = demandDao.SelectDemandCategories(conn);
                    if (categories != null && categories.Count > 0)
                    {
                        foreach (var demand in result)
                        {
                            foreach (var category in categories)
                            {
                                if (demand.CategoryId == category.Id)
                                {
                                    demand.CategoryName = category.Name;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询需求列表", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 查询需求列表
        /// </summary>
        public List<Demand> GetDemandsByCity(string city, int pageCount, int pageIndex, out int count)
        {
            List<Demand> result = new List<Demand>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = demandDao.SelectDemandsByCity(pageCount, pageIndex, city, out tempCount, conn);
                count = tempCount;
                if (result != null && result.Count > 0)
                {
                    List<DemandCategory> categories = demandDao.SelectDemandCategories(conn);
                    if (categories != null && categories.Count > 0)
                    {
                        foreach (var demand in result)
                        {
                            foreach (var category in categories)
                            {
                                if (demand.CategoryId == category.Id)
                                {
                                    demand.CategoryName = category.Name;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询需求列表", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 据城市查询需求列表
        /// </summary>
        public List<Demand> GetDemandsByParameters(DemandParameters parameters, out int count)
        {
            List<Demand> result = new List<Demand>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = demandDao.SelectDemandsByParameters(parameters, out tempCount, conn);
                FillDemandCategoryName(result, conn);
                count = tempCount;
            }
            catch (Exception e)
            {
                LogService.Log("查询需求列表", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        private void FillDemandCategoryName(List<Demand> result, System.Data.SqlClient.SqlConnection conn)
        {
            if (result != null && result.Count > 0)
            {
                List<DemandCategory> categories = demandDao.SelectDemandCategories(conn);
                if (categories != null && categories.Count > 0)
                {
                    foreach (var demand in result)
                    {
                        foreach (var category in categories)
                        {
                            if (demand.CategoryId == category.Id)
                            {
                                demand.CategoryName = category.Name;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据用户ID查询需求列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Demand> GetDemandsByUserId(int userId, int pageCount, int pageIndex, out int count)
        {
            List<Demand> result = new List<Demand>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = demandDao.SelectDemandsByUserId(userId, pageCount, pageIndex, out tempCount, conn);
                FillDemandCategoryName(result, conn);
                count = tempCount;
            }
            catch (Exception e)
            {
                LogService.Log("根据用户ID查询需求列表", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 根据用户ID查询需求列表并包含报价记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Demand> GetDemandsWithQuotesByUserId(int userId, int pageCount, int pageIndex, out int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据城市查询需求列表
        /// </summary>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="city"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<Demand> GetDemandsByCity(int pageCount, int pageIndex, string city, out int count)
        {
            List<Demand> result = new List<Demand>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                result = demandDao.SelectDemandsByCity(pageCount, pageIndex, city, out tempCount, conn);
                count = tempCount;
                if (result != null && result.Count > 0)
                {
                    List<DemandCategory> categories = demandDao.SelectDemandCategories(conn);
                    if (categories != null && categories.Count > 0)
                    {
                        foreach (var demand in result)
                        {
                            foreach (var category in categories)
                            {
                                if (demand.CategoryId == category.Id)
                                {
                                    demand.CategoryName = category.Name;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("根据城市查询需求列表", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 根据Id查询需求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Demand GetDemandById(int id)
        {
            Demand result = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = demandDao.SelectDemandById(id, conn);
                if (result != null)
                {
                    result.ContentStyle = FilterHelper.Filter(result.ContentStyle, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    result.ContentText = FilterHelper.Filter(result.ContentText, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    result.Description = FilterHelper.Filter(result.Description, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                }
            }
            catch (Exception e)
            {
                LogService.Log("根据Id查询需求", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 更新需求
        /// </summary>
        /// <param name="demand"></param>
        /// <returns></returns>
        public bool EditDemand(Demand demand)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                if (demand != null)
                {
                    result = DemandDao.UpdateDemand(demand, conn);
                }
            }
            catch (Exception e)
            {
                LogService.Log("更新需求失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 更新需求联系方式购买金额
        /// </summary>
        /// <param name="demandIds"></param>
        /// <param name="weixinBuyFee"></param>
        /// <returns></returns>
        public bool UpdateWexinBuyFee(List<int> demandIds, int weixinBuyFee)
        {
            bool result = false;

            if (demandIds == null || demandIds.Count == 0)
            {
                return result;
            }

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var demandId in demandIds)
                    {
                        result = DemandDao.UpdatesDemandWeixinBuyFee(conn, demandId, weixinBuyFee);

                        if (!result)
                        {
                            break;
                        }
                    }

                    if (result)
                    {
                        scope.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("更新需求联系方式购买金额失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 检查用户是否已购买需求的联系方式查看权限
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public bool HasWeChatUserBoughtForDemand(string openId, int demandId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                if (!string.IsNullOrEmpty(openId))
                {
                    result = DemandDao.SelectTradeOrderByOpenIdAndDemandId(conn, openId, demandId) > 0;
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询已购买需求失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// Updates demand status with specified status value.
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        public bool UpdateDemandStatus(int demandId, DemandStatus statusValue)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                if (demandId > 0)
                {
                    result = DemandDao.UpdateDemandStatus(conn, demandId, statusValue);
                }
            }
            catch (Exception e)
            {
                LogService.Log("更新需求状态失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
    }
}
