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
                LogService.Log("更新需求失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
    }
}
