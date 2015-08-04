using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL.Daos
{
    public class CityDao
    {
        //存储过程名称
        private const string sp_CitySelect = "sp_CitySelect";
        private const string sp_CitySelectProvince = "sp_CitySelectProvince";
        private const string sp_CitySelectByProvinceId = "sp_CitySelectByProvinceId";
        private const string sp_CitySelectByCityId = "sp_CitySelectByCityId";

        /// <summary>
        /// 查询所有省，市，区
        /// </summary>
        public List<City> SelectCites(SqlConnection conn)
        {
            List<City> result = null;
            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_CitySelect);
            result = DBHelper.DataTableToList<City>(dts["0"]);

            return result;
        }

        /// <summary>
        /// 查询所有省
        /// </summary>
        public List<City> SelectProvinces(SqlConnection conn)
        {
            List<City> result = null;

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_CitySelectProvince);
            result = DBHelper.DataTableToList<City>(dts["0"]);

            return result;
        }

        /// <summary>
        /// 根据省份Id查询该省所有一级城市
        /// </summary>
        public List<City> SelectCitesByProvinceId(string provinceId, SqlConnection conn)
        {
            List<City> result = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ProvinceId", provinceId)
            };

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_CitySelectByProvinceId, sqlParameters);
            result = DBHelper.DataTableToList<City>(dts["0"]);

            return result;
        }

        /// <summary>
        /// 根据一级城市Id查询所有该市下的二级城市、区、商圈
        /// </summary>
        public List<City> SelectCitesByCityId(string cityId, SqlConnection conn)
        {
            List<City> result = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@CityId", cityId)
            };

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_CitySelectByCityId, sqlParameters);
            result = DBHelper.DataTableToList<City>(dts["0"]);

            return result;
        }
    }
}
