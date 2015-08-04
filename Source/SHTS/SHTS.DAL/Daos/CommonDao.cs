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
    public class CommonDao
    {
        private const string sp_SelectRight = "sp_SelectRight";

        public Right SelectRight(SqlConnection conn)
        {
            Right result = new Right();

            Dictionary<string, DataTable> dts;
            dts = DBHelper.GetMuiltiDataFromDB(conn, sp_SelectRight);
            result.Demands = DBHelper.DataTableToList<Demand>(dts["0"]);
            result.Spaces = DBHelper.DataTableToList<Resource>(dts["1"]);
            result.Actors = DBHelper.DataTableToList<Resource>(dts["2"]);
            result.Equipments = DBHelper.DataTableToList<Resource>(dts["3"]);
            result.Others = DBHelper.DataTableToList<Resource>(dts["4"]);

            return result;
        }
    }
}
