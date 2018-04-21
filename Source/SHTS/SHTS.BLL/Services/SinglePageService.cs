using System;
using System.Collections.Generic;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    public class SinglePageService
    {
        //根据Id查询单页
        public SinglePage GetSingPageById(string id)
        {
            SinglePage result = null;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                if (!string.IsNullOrEmpty(id))
                {
                    result = SinglePageDao.SelectSinglePageById(Int32.Parse(id), conn);
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询单个单页失败", "SingePageId: " + id + "\r\n" +  e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        //根据单页类型查询单页列表
        public List<SinglePage> GetSinglePages(string entityType, string category, bool isActive, int pageCount, int pageIndex, out int count)
        {
            List<SinglePage> singlePages = new List<SinglePage>();

            count = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                int tempCount = 0;
                singlePages = SinglePageDao.SelectSinglePages(entityType, category, isActive, pageCount, pageIndex, out tempCount, conn);
                count = tempCount;
            }
            catch (Exception e)
            {
                LogService.Log("查询单页列表", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return singlePages;
        }

        /// <summary>
        /// 查询幻灯片
        /// </summary>
        /// <returns></returns>
        public List<SinglePage> GetSlides()
        {
            List<SinglePage> result = null;

            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = SinglePageDao.SelectSlides(conn);
            }
            catch (Exception e)
            {
                LogService.Log("查询幻灯片", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 更新单页
        /// </summary>
        /// <param name="singlgePage">单页实体</param>
        public bool EditSinglePage(SinglePage singlgePage)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                if (singlgePage != null && !string.IsNullOrEmpty(singlgePage.EntityType))
                {
                    result = SinglePageDao.UpdateSinglePage(singlgePage, conn);
                }
            }
            catch (Exception e)
            {
                LogService.Log("更新单页失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 删除单页
        /// </summary>
        public bool DeleteSinglePageById(int id)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = SinglePageDao.DeleteSinglePageById(id, conn);
            }
            catch (Exception e)
            {
                LogService.Log("删除单页失败", e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
    }
}
