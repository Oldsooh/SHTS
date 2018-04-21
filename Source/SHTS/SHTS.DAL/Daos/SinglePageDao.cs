using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL.Daos
{
    public class SinglePageDao
    {
        //存储过程名称
        private const string sp_SinglePageSelectById = "sp_SinglePageSelectById";
        private const string sp_SinglePageSelect = "sp_SinglePageSelect";
        private const string sp_SinglePageSelectSlide = "sp_SinglePageSelectSlide";
        private const string sp_SinglePageUpdate = "sp_SinglePageUpdate";
        private const string sp_SinglePageDeleteById = "sp_SinglePageDeleteById";


        /// <summary>
        /// 根据Id查询单页
        /// </summary>
        public static SinglePage SelectSinglePageById(int id, SqlConnection conn)
        {
            SinglePage rersult = null;

            List<SinglePage> list = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_SinglePageSelectById, sqlParameters);
            list = DBHelper.DataTableToList<SinglePage>(dts["0"]);
            if (list != null && list.Count > 0)
            {
                rersult = list[0];
            }
            return rersult;
        }

        /// <summary>
        /// 根据单页类型查询单页列表
        /// </summary>
        /// <param name="entityType">单页类型</param>
        /// <param name="category">分类</param>
        /// <param name="isActive"></param>
        /// <param name="pageCount">每页显示条数</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="count">该分类专辑总数</param>
        /// <param name="conn">连接对象</param>
        /// <returns>单页列表</returns>
        public static List<SinglePage> SelectSinglePages(string entityType, string category, bool isActive, int pageCount, int pageIndex, out int count, SqlConnection conn)
        {
            List<SinglePage> result = null;
            object categoryParam = null;

            if (string.IsNullOrEmpty(category))
            {
                categoryParam = DBNull.Value;
            }
            else
            {
                categoryParam = category;
            }

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@EntityType", entityType),
                new SqlParameter("@Category", categoryParam),
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@PageCount", pageCount),
                new SqlParameter("@PageIndex", pageIndex)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_SinglePageSelect, sqlParameters);
            count = Int32.Parse(dts["0"].Rows[0][0].ToString());
            result = DBHelper.DataTableToList<SinglePage>(dts["1"]);
            return result;
        }

        /// <summary>
        /// 查询幻灯片
        /// </summary>
        public static List<SinglePage> SelectSlides(SqlConnection conn)
        {
            List<SinglePage> result = null;

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_SinglePageSelectSlide);
            result = DBHelper.DataTableToList<SinglePage>(dts["0"]);

            return result;
        }

        /// <summary>
        /// 更新单页
        /// </summary>
        public static bool UpdateSinglePage(SinglePage singlePage, SqlConnection conn)
        {
            object ParentId = DBNull.Value;
            object Category = DBNull.Value;
            object Title = DBNull.Value;
            object Keywords = DBNull.Value;
            object Description = DBNull.Value;
            object ContentStyle = DBNull.Value;
            object ContentText = DBNull.Value;
            object ImageUrl = DBNull.Value;
            object Link = DBNull.Value;
            object ViewCount = DBNull.Value;

            #region 转化DBNull
            if (null != singlePage.ParentId && 0 != singlePage.ParentId)
            {
                ParentId = singlePage.ParentId;
            }
            if (!string.IsNullOrEmpty(singlePage.Category))
            {
                Category = singlePage.Category;
            }
            if (!string.IsNullOrEmpty(singlePage.Title))
            {
                Title = singlePage.Title;
            }
            if (!string.IsNullOrEmpty(singlePage.Keywords))
            {
                Keywords = singlePage.Keywords;
            }
            if (!string.IsNullOrEmpty(singlePage.Description))
            {
                Description = singlePage.Description;
            }
            if (!string.IsNullOrEmpty(singlePage.ContentStyle))
            {
                ContentStyle = singlePage.ContentStyle;
            }
            if (!string.IsNullOrEmpty(singlePage.ContentText))
            {
                ContentText = singlePage.ContentText;
            }
            if (!string.IsNullOrEmpty(singlePage.ImageUrl))
            {
                ImageUrl = singlePage.ImageUrl;
            }
            if (!string.IsNullOrEmpty(singlePage.Link))
            {
                Link = singlePage.Link;
            }
            if (null != singlePage.ViewCount && 0 != singlePage.ViewCount)
            {
                ViewCount = singlePage.ViewCount;
            }
            #endregion

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id", singlePage.Id),
                new SqlParameter("@ParentId", ParentId),
                new SqlParameter("@EntityType", singlePage.EntityType),
                new SqlParameter("@Category", Category),
                new SqlParameter("@Title", Title),
                new SqlParameter("@Keywords", Keywords),
                new SqlParameter("@Description", Description),
                new SqlParameter("@ContentStyle", ContentStyle),
                new SqlParameter("@ContentText", ContentText),
                new SqlParameter("@ImageUrl", ImageUrl),
                new SqlParameter("@Link", Link),
                new SqlParameter("@ViewCount", ViewCount)
            };
            return DBHelper.SetDataToDB(conn, sp_SinglePageUpdate, sqlParameters);
        }

        /// <summary>
        /// 删除单页
        /// </summary>
        public static bool DeleteSinglePageById(int id, SqlConnection conn)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            return DBHelper.SetDataToDB(conn, sp_SinglePageDeleteById, sqlParameters);
        }
    }
}
