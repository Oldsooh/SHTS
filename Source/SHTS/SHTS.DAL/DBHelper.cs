using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Witbird.SHTS.Common.Extensions;

namespace Witbird.SHTS.DAL
{
    public class DBHelper
    {
        /// <summary>
        /// Sql数据库连接字符串
        /// </summary>
        static string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["sqlConn"].ConnectionString;

        /// <summary>
        /// 获取Sql数据库连接对象
        /// </summary>
        public static SqlConnection GetSqlConnection()
        {
            return new SqlConnection(connStr);
        }

        public static string GetSqlConnectionString => connStr;

        #region DataSet
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="conn">连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <returns></returns>
        public static DataSet GetDataSetFromDB(SqlConnection conn, string spName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="conn">连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <returns></returns>
        public static DataSet GetDataSetFromDB(SqlConnection conn, string spName, SqlParameter[] sqlParams)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(sqlParams);
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取单表数据
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <returns></returns>
        public static DataTable GetSingleDataFromDB(SqlConnection conn, string spName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取单表数据(参数化)
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="sqlParams">参数</param>
        /// <returns></returns>
        public static DataTable GetSingleDataFromDB(SqlConnection conn, string spName, SqlParameter[] sqlParams)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(sqlParams);
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取多表数据
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <returns></returns>
        public static Dictionary<string, DataTable> GetMuiltiDataFromDB(SqlConnection conn, string spName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                Dictionary<string, DataTable> dataTableDictionary = new Dictionary<string, DataTable>();
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    dataTableDictionary.Add(i.ToString(), ds.Tables[i]);
                }
                return dataTableDictionary;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取多表数据(参数化)
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="sqlParams">参数</param>
        /// <returns></returns>
        public static Dictionary<string, DataTable> GetMuiltiDataFromDB(SqlConnection conn, string spName, SqlParameter[] sqlParams)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(sqlParams);
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                Dictionary<string, DataTable> dataViewDictionary = new Dictionary<string, DataTable>();
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    dataViewDictionary.Add(i.ToString(), ds.Tables[i]);
                }
                return dataViewDictionary;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 写入操作
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <returns></returns>
        public static bool SetDataToDB(SqlConnection conn, string spName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 写入操作(参数化)
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="sqlParams">参数</param>
        /// <returns></returns>
        public static bool SetDataToDB(SqlConnection conn, string spName, SqlParameter[] sqlParams)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(sqlParams);
                
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 把DataTable转移成List
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable dt)
        {
            List<T> list = new List<T>();
            T t = default(T);
            PropertyInfo[] propertypes = null;
            string tempName = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                t = Activator.CreateInstance<T>();
                propertypes = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertypes)
                {
                    tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        object value = row[tempName];
                        if (!value.ToString().Equals(""))
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                list.Add(t);
            }
            return list.Count == 0 ? null : list;
        }

        #endregion

        #region DataReader

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader RunProcedure(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlDataReader returnReader;
            SqlCommand command = connection.CreateCommand();
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            returnReader = command.ExecuteReader();
            return returnReader;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunNonQueryProcedure(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            int result;
            SqlCommand command = connection.CreateCommand();
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);
            result = command.ExecuteNonQuery();
            return result;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunScalarProcedure(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            int result;
            SqlCommand command = connection.CreateCommand();
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);
            result = command.ExecuteScalar().DBToInt32();
            return result;
        }

        /// <summary>
        /// 执行事务存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedureWithTransaction(SqlTransaction transaction, SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            int result;
            SqlCommand command = connection.CreateCommand();
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = transaction;
            command.Parameters.AddRange(parameters);
            result = command.ExecuteNonQuery();
            return result;
        }

        #endregion

        #region Paras

        public static void CheckSqlSpParameter(IDataParameter[] parameters)
        {
            if (parameters != null && parameters.Count() > 0)
            {
                foreach (var p in parameters)
                {
                    if(p.Value==null)
                    {
                        p.Value = Convert.DBNull;
                    }
                }
            }
        }

        #endregion
    }
}
