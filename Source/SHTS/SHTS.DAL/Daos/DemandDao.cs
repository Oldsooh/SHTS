﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL.Daos
{
    public class DemandDao
    {
        private const string sp_DemandSelect = "sp_DemandSelect";
        private const string sp_DemandSelectByParameters = "sp_DemandSelectByParameters";
        private const string sp_DemandSelectById = "sp_DemandSelectById";
        private const string sp_DemandSelectByUserId = "sp_DemandSelectByUserId";
        private const string sp_DemandSelectByCity = "sp_DemandSelectByCity";
        private const string sp_DemandCategory_Select = "sp_DemandCategory_Select";
        private const string sp_DemandUpdate = "sp_DemandUpdate";

        public List<Demand> SelectDemands(int pageCount, int pageIndex, out int count, SqlConnection conn)
        {
            List<Demand> result = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageCount", pageCount),
                new SqlParameter("@PageIndex", pageIndex)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_DemandSelect, sqlParameters);
            count = Int32.Parse(dts["0"].Rows[0][0].ToString());
            result = DBHelper.DataTableToList<Demand>(dts["1"]);
            return result;
        }

        public List<Demand> SelectDemandsByParameters(DemandParameters parameters, out int count, SqlConnection conn)
        {
            List<Demand> result = null;

            object Province = DBNull.Value;
            object City = DBNull.Value;
            object Area = DBNull.Value;
            object Category = DBNull.Value;
            object StartBudget = DBNull.Value;
            object EndBudget = DBNull.Value;
            object StartTime = DBNull.Value;
            object EndTime = DBNull.Value;

            #region 转化DBNull
            if (!string.IsNullOrEmpty(parameters.Province))
            {
                Province = parameters.Province;
            }
            if (!string.IsNullOrEmpty(parameters.City))
            {
                City = parameters.City;
            }
            if (!string.IsNullOrEmpty(parameters.Area))
            {
                Area = parameters.Area;
            }
            if (!string.IsNullOrEmpty(parameters.Category))
            {
                Category = parameters.Category;
            }
            if (!string.IsNullOrEmpty(parameters.StartBudget))
            {
                StartBudget = parameters.StartBudget;
            }
            if (!string.IsNullOrEmpty(parameters.EndBudget))
            {
                EndBudget = parameters.EndBudget;
            }
            if (!string.IsNullOrEmpty(parameters.StartTime))
            {
                StartTime = parameters.StartTime;
            }
            if (!string.IsNullOrEmpty(parameters.EndTime))
            {
                EndTime = parameters.EndTime;
            }
            #endregion

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageCount", parameters.PageCount),
                new SqlParameter("@PageIndex", parameters.PageIndex),
                new SqlParameter("@Province", Province),
                new SqlParameter("@City", City),
                new SqlParameter("@Area", Area),
                new SqlParameter("@Category", Category),
                new SqlParameter("@StartBudget", StartBudget),
                new SqlParameter("@EndBudget", EndBudget),
                new SqlParameter("@StartTime", StartTime),
                new SqlParameter("@EndTime", EndTime)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_DemandSelectByParameters, sqlParameters);
            count = Int32.Parse(dts["0"].Rows[0][0].ToString());
            result = DBHelper.DataTableToList<Demand>(dts["1"]);
            return result;
        }

        /// <summary>
        /// 根据用户ID查询需求列表
        /// </summary>
        public List<Demand> SelectDemandsByUserId(int userId, int pageCount, int pageIndex, out int count, SqlConnection conn)
        {
            List<Demand> result = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@PageCount", pageCount),
                new SqlParameter("@PageIndex", pageIndex)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_DemandSelectByUserId, sqlParameters);
            count = Int32.Parse(dts["0"].Rows[0][0].ToString());
            result = DBHelper.DataTableToList<Demand>(dts["1"]);
            return result;
        }

        /// <summary>
        /// 根据城市查询需求列表
        /// </summary>
        public List<Demand> SelectDemandsByCity(int pageCount, int pageIndex, string city, out int count, SqlConnection conn)
        {
            List<Demand> result = null;

            object City = DBNull.Value;
            if (!string.IsNullOrEmpty(city))
            {
                City = city;
            }

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@City", City),
                new SqlParameter("@PageCount", pageCount),
                new SqlParameter("@PageIndex", pageIndex)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_DemandSelectByCity, sqlParameters);
            count = Int32.Parse(dts["0"].Rows[0][0].ToString());
            result = DBHelper.DataTableToList<Demand>(dts["1"]);
            return result;
        }

        /// <summary>
        /// 根据Id查询需求
        /// </summary>
        public Demand SelectDemandById(int id, SqlConnection conn)
        {
            Demand demand = null;

            List<Demand> demands = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_DemandSelectById, sqlParameters);
            demands = DBHelper.DataTableToList<Demand>(dts["0"]);
            if (demands != null && demands.Count > 0)
            {
                demand = demands[0];
            }
            return demand;
        }

        /// <summary>
        /// 查询分类
        /// </summary>
        public List<DemandCategory> SelectDemandCategories(SqlConnection conn)
        {
            List<DemandCategory> result = null;

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_DemandCategory_Select);
            result = DBHelper.DataTableToList<DemandCategory>(dts["0"]);

            return result;
        }

        /// <summary>
        /// 更新需求
        /// </summary>
        public static bool UpdateDemand(Demand demand, SqlConnection conn)
        {
            object Description = DBNull.Value;
            object ContentStyle = DBNull.Value;
            object ContentText = DBNull.Value;
            object Province = DBNull.Value;
            object CityEntity = DBNull.Value;
            object Area = DBNull.Value;
            object Address = DBNull.Value;
            object Phone = DBNull.Value;
            object QQWeixin = DBNull.Value;
            object Email = DBNull.Value;
            object TimeLength = DBNull.Value;
            object PeopleNumber = DBNull.Value;

            #region 转化DBNull
            if (!string.IsNullOrEmpty(demand.Description))
            {
                Description = demand.Description;
            }
            if (!string.IsNullOrEmpty(demand.ContentStyle))
            {
                ContentStyle = demand.ContentStyle;
            }
            if (!string.IsNullOrEmpty(demand.ContentText))
            {
                ContentText = demand.ContentText;
            }
            if (!string.IsNullOrEmpty(demand.Province))
            {
                Province = demand.Province;
            }
            if (!string.IsNullOrEmpty(demand.City))
            {
                CityEntity = demand.City;
            }
            if (!string.IsNullOrEmpty(demand.Area))
            {
                Area = demand.Area;
            }
            if (!string.IsNullOrEmpty(demand.Address))
            {
                Address = demand.Address;
            }
            if (!string.IsNullOrEmpty(demand.Phone))
            {
                Phone = demand.Phone;
            }
            if (!string.IsNullOrEmpty(demand.QQWeixin))
            {
                QQWeixin = demand.QQWeixin;
            }
            if (!string.IsNullOrEmpty(demand.Email))
            {
                Email = demand.Email;
            }
            if (!string.IsNullOrEmpty(demand.TimeLength))
            {
                TimeLength = demand.TimeLength;
            }
            if (!string.IsNullOrEmpty(demand.PeopleNumber))
            {
                PeopleNumber = demand.PeopleNumber;
            }
            #endregion

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id", demand.Id),
                new SqlParameter("@CategoryId", demand.CategoryId),
                new SqlParameter("@Title", demand.Title),
                new SqlParameter("@Description", Description),
                new SqlParameter("@ContentStyle", ContentStyle),
                new SqlParameter("@ContentText", ContentText),
                new SqlParameter("@Province", Province),
                new SqlParameter("@City", CityEntity),
                new SqlParameter("@Area", Area),
                new SqlParameter("@Address", Address),
                new SqlParameter("@Phone", Phone),
                new SqlParameter("@QQWeixin", QQWeixin),
                new SqlParameter("@Email", Email),
                new SqlParameter("@StartTime", demand.StartTime),
                new SqlParameter("@EndTime", demand.EndTime),
                new SqlParameter("@TimeLength", TimeLength),
                new SqlParameter("@PeopleNumber", PeopleNumber),
                new SqlParameter("@Budget", demand.Budget),
                new SqlParameter("@IsActive", demand.IsActive)
            };
            return DBHelper.SetDataToDB(conn, sp_DemandUpdate, sqlParameters);
        }

    }
}