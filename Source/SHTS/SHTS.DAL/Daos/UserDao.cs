using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.Common;

namespace Witbird.SHTS.DAL.Daos
{
    public class UserDao
    {
        #region const

        //存储过程名称
        private const string sp_UserSelectById = "sp_UserSelectById";
        private const string SP_UserRegister = "sp_UserRegister";
        private const string SP_UserLogin = "sp_UserLogin";
        private const string sp_VerifyAccountInfo = "sp_VerifyAccountInfo";
        private const string sp_QueryUsers = "sp_QueryUsers";
        private const string sp_DeleteUser = "sp_DeleteUser";
        private const string sp_UpdateUser = "sp_UpdateUser";

        private const string SP_WeChatUserSelectByOpenId = "sp_WeChatUserSelectByOpenId";
        private const string SP_WeChatUserSelectByUserId = "sp_WeChatUserSelectByUserId";
        private const string SP_WeChatUserRegister = "sp_WeChatUserRegister";
        private const string SP_WeChatUserUpdate = "sp_WeChatUserUpdate";
        private const string SP_WeChatUserDeleteById = "sp_WeChatUserDeleteById";
        private const string SP_WeChatUserDeleteByOpenId = "sp_WeChatUserDeleteByOpenId";

        private const string SP_AddUserBankInfo = "sp_AddUserBankInfo";
        private const string SP_UpdateUserBankInfo = "sp_UpdateUserBankInfo";
        private const string SP_DeleteUserBankInfo = "sp_DeleteUserBankInfo";
        private const string SP_GetUserBankInfos = "sp_GetUserBankInfos";
        private const string SP_GetUserBankInfo = "sp_GetUserBankInfo";

        /// <summary>
        /// Required Parameters: @UserId
        /// </summary>
        private const string SP_SelectUserVipInfoByUserId = "sp_SelectUserVipInfoByUserId";
        /// <summary>
        /// Required Paraeters: @Id, @OrderId, @IdentityImg, @StartTime, @EndTime, @Duration, Amount, @State, @LastUpdatedTime
        /// </summary>
        private const string SP_UpdateUserVipInfoByUserId = "sp_UpdateUserVipInfoByUserId";

        //列名
        private const string column_UserId = "@UserId";

        #endregion

        #region User操作

        /// <summary>
        /// 根据Id查询用户
        /// </summary>
        /// <param name="conn">连接对象</param>
        /// <returns>用户实体</returns>
        public User GetUserById(int userId, SqlConnection conn)
        {
            User user = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(column_UserId, userId)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn, sp_UserSelectById, sqlParameters);
            while (reader.Read())
            {
                user = ConvertToUserObject(reader);
            }
            if (reader != null)
            {
                reader.Close();
            }
            return user;
        }

        /// <summary>
        /// 根据UserName查询用户
        /// </summary>
        /// <param name="conn">连接对象</param>
        /// <param name="username">用户名、用户注册邮箱、注册手机号</param>
        /// <returns>用户实体</returns>
        public User GetUserByUserName(string username, SqlConnection conn)
        {
            User user = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@username", username)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn, "sp_GetUserByUserName", sqlParameters);
            while (reader.Read())
            {
                user = ConvertToUserObject(reader);
            }
            if (reader != null)
            {
                reader.Close();
            }
            return user;
        }

        /// <summary>
        /// 登录.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User Login(SqlConnection conn, string username, string password)
        {
            User user = null;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@UserName", username),
			   new SqlParameter("@EncryptedPassword", password)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn, SP_UserLogin, sqlParameters);
            while (reader.Read())
            {
                user = ConvertToUserObject(reader);
            }
            if (reader != null)
            {
                reader.Close();
            }
            return user;
        }

        /// <summary>
        /// 验证用户信息是否已经存在。
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool VerifyUserInfo(SqlConnection conn, string column, string value)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@columnname", column),
			   new SqlParameter("@value", value)
            };
            return DBHelper.RunScalarProcedure(conn,
                sp_VerifyAccountInfo, sqlParameters) == 0;
        }

        /// <summary>
        /// 注册用户。返回UserId
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserRegister(SqlConnection conn, User user, out int userId)
        {
            bool result = false;
            userId = -1;
            SqlParameter[] parameters = 
            {
                new SqlParameter(column_UserId, SqlDbType.Int,4),
				new SqlParameter("@UserName", user.UserName),
				new SqlParameter("@EncryptedPassword", user.EncryptedPassword),
				new SqlParameter("@UserType", user.UserType),
				new SqlParameter("@Adress", user.Adress),
				new SqlParameter("@LocationId", user.LocationId),
				new SqlParameter("@Cellphone", user.Cellphone),
				new SqlParameter("@Email", user.Email),
				new SqlParameter("@QQ", user.QQ),
				new SqlParameter("@UCard", user.UCard),
				new SqlParameter("@SiteUrl", user.SiteUrl),
				new SqlParameter("@LoginIdentiy", user.LoginIdentiy),
				new SqlParameter("@IdentiyImg", user.IdentiyImg),
				new SqlParameter("@Vip", user.Vip),
				new SqlParameter("@CreateTime", DateTime.Now),
				new SqlParameter("@LastUpdatedTime", DateTime.Now)
           };
            parameters[0].Direction = ParameterDirection.Output;
            DBHelper.CheckSqlSpParameter(parameters);

            result = DBHelper.RunNonQueryProcedure(conn, SP_UserRegister, parameters) > 0;
            userId = Convert.ToInt32(parameters[0].Value);

            return result;
        }

        /// <summary>
        /// 更新用户信息。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUser(SqlConnection conn, User user)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter(Constants.column_UserId, user.UserId),
				new SqlParameter("@Adress", user.Adress),
				new SqlParameter(Constants.column_cellphone, user.Cellphone),
				new SqlParameter("@LocationId", user.LocationId),
				new SqlParameter("@Email", user.Email),
				new SqlParameter("@QQ", user.QQ),
				new SqlParameter("@UCard", user.UCard),
				new SqlParameter("@SiteUrl", user.SiteUrl),
				new SqlParameter("@Vip", user.Vip),
                new SqlParameter("@Photo", user.Photo),
                new SqlParameter("@IdentiyImg", user.IdentiyImg)
           };
            DBHelper.CheckSqlSpParameter(parameters);
            return DBHelper.RunNonQueryProcedure(conn, sp_UpdateUser, parameters) > 0;
        }

        /// <summary>
        /// 更新用户详细信息。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUserProfile(SqlTransaction transaction, SqlConnection conn, UserProfile user)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter(Constants.column_UserId, user.UserId),
				new SqlParameter("@Id", user.Id),
				new SqlParameter("@ProfileType", user.ProfileType),
                new SqlParameter("@Value", user.Value),
                new SqlParameter("@State", user.State)
           };
            DBHelper.CheckSqlSpParameter(parameters);
            return DBHelper.RunProcedureWithTransaction(transaction, conn, "sp_CreateOrUpdateProfile", parameters) > 0;
        }

        /// <summary>
        /// 获取用户profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserProfile> GetUserProfiles(SqlConnection conn, int userId)
        {
            List<UserProfile> users = new List<UserProfile>();
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(Constants.column_UserId, userId.ToString())
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn,
                "sp_GetUserProfiles", sqlParameters);
            while (reader.Read())
            {
                users.Add(new UserProfile
                {
                    Id = reader["Id"].DBToInt32(),
                    UserId = reader["UserId"].DBToString(),
                    ProfileType = reader["ProfileType"].DBToString(),
                    Value = reader["Value"].DBToString()
                });
            }
            if (reader != null)
            {
                reader.Close();
            }
            return users;
        }

        /// <summary>
        /// 更新会员等级状态。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUserVipState(SqlConnection conn, int userId, int vipState)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter(Constants.column_UserId, userId),
				new SqlParameter("@Vip", vipState)
           };
            DBHelper.CheckSqlSpParameter(parameters);
            return DBHelper.RunNonQueryProcedure(conn, "sp_SetUserToVip", parameters) > 0;
        }


        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetBackPasswordByCellphone(SqlConnection conn, User user)
        {
            const string sp_GetBackPassword = "sp_GetBackPassword";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter(Constants.column_cellphone, user.Cellphone),
               new SqlParameter("@EncryptedPassword", user.EncryptedPassword)
            };
            return DBHelper.RunNonQueryProcedure(conn,
                sp_GetBackPassword, sqlParameters) > 0;
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool DeleteUser(SqlConnection conn, int id)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter(Constants.column_UserId, id)
            };
            return DBHelper.RunNonQueryProcedure(conn,
                sp_DeleteUser, sqlParameters) > 0;
        }

        /// <summary>
        /// 分页查询会员信息
        /// </summary>
        /// <param name="startPageIndex">开始页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="queryType">查询类型</param>
        /// <returns></returns>
        public List<User> QuersyUsers(SqlConnection conn, string cityid, int resourceid,
            int startRowIndex, int pageSize, int queryType, string Keyword, out int totalCount)
        {
            List<User> users = new List<User>();
            totalCount = 0;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(Constants.StartRowIndex, startRowIndex),
                new SqlParameter(Constants.PageSize, pageSize),
                new SqlParameter(Constants.QueryType, queryType)
                ,new SqlParameter("@cityid", cityid)
                ,new SqlParameter("@resourceid", resourceid)
                ,new SqlParameter("@Keyword", Keyword)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn,
                sp_QueryUsers, sqlParameters);
            while (reader.Read())
            {
                users.Add(ConvertToUserObject(reader));
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    totalCount = reader[Constants.column_totalCount].DBToInt32();
                }
            }
            if (reader != null)
            {
                reader.Close();
            }
            return users;
        }

        /// <summary>
        /// 分页查询会员信息
        /// </summary>
        /// <param name="startPageIndex">开始页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="queryType">查询类型</param>
        /// <returns></returns>
        public List<User> QuersyUsersByCityAndResource(SqlConnection conn, string cityid, int resourceid,
            int startRowIndex, int pageSize, int queryType, string Keyword, out int totalCount)
        {
            List<User> users = new List<User>();
            totalCount = 0;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(Constants.StartRowIndex, startRowIndex),
                new SqlParameter(Constants.PageSize, pageSize),
                new SqlParameter(Constants.QueryType, queryType)
                ,new SqlParameter("@cityid", cityid)
                ,new SqlParameter("@resourceid", resourceid)
                ,new SqlParameter("@Keyword", Keyword)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn,
                "sp_QuersyUsersByCityAndResource", sqlParameters);
            while (reader.Read())
            {
                users.Add(ConvertToUserObject(reader));
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    totalCount = reader[Constants.column_totalCount].DBToInt32();
                }
            }
            if (reader != null)
            {
                reader.Close();
            }
            return users;
        }

        #endregion

        #region WeChat User

        /// <summary>
        /// 根据绑定的会员账号Id查询微信用户
        /// </summary>
        /// <param name="conn">连接对象</param>
        /// <returns>用户实体</returns>
        public WeChatUser GetWeChatUser(int userId, SqlConnection conn)
        {
            WeChatUser user = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

            using (SqlDataReader reader = DBHelper.RunProcedure(conn, SP_WeChatUserSelectByUserId, sqlParameters))
            {
                while (reader.Read())
                {
                    user = ConvertToWeChatUserObject(reader);
                }
            }

            return user;
        }

        public WeChatUser GetWeChatUser(string openId, SqlConnection conn)
        {
            WeChatUser user = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@OpenId", openId)
            };

            using (SqlDataReader reader = DBHelper.RunProcedure(conn, SP_WeChatUserSelectByOpenId, sqlParameters))
            {
                while (reader.Read())
                {
                    user = ConvertToWeChatUserObject(reader);
                }
            }

            return user;
        }

        /// <summary>
        /// 注册微信用户。如果用户属于取消关注后重新关注，那么更新以前微信数据信息，不重新增加新数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool WeChatUserRegister(SqlConnection conn, WeChatUser user)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter("@Id", SqlDbType.Int, 4),
				new SqlParameter("@UserId", user.UserId),
				new SqlParameter("@OpenId", user.OpenId),
				new SqlParameter("@NickName", user.NickName),
				new SqlParameter("@Sex", user.Sex),
				new SqlParameter("@Province", user.Province),
				new SqlParameter("@City", user.City),
				new SqlParameter("@County", user.County),
				new SqlParameter("@Photo", user.Photo),
				new SqlParameter("@AccessToken", user.AccessToken),
				new SqlParameter("@AccessTokenExpired", user.AccessTokenExpired),
				new SqlParameter("@AccessTokenExpireTime", user.AccessTokenExpireTime),
				new SqlParameter("@HasSubscribed", user.HasSubscribed),
                new SqlParameter("@HasAuthorized", user.HasAuthorized),
				new SqlParameter("@CreatedTime", user.CreatedTime)
            };

            parameters[0].Direction = ParameterDirection.Output;
            DBHelper.CheckSqlSpParameter(parameters);

            return DBHelper.RunNonQueryProcedure(conn, SP_WeChatUserRegister, parameters) > 0;
        }

        /// <summary>
        /// 更新微信用户信息。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateWeChatUser(SqlConnection conn, WeChatUser user)
        {
            SqlParameter[] parameters = 
            {
                new SqlParameter("@Id", user.Id),
				new SqlParameter("@UserId", user.UserId),
				new SqlParameter("@OpenId", user.OpenId),
				new SqlParameter("@NickName", user.NickName),
				new SqlParameter("@Sex", user.Sex),
				new SqlParameter("@Province", user.Province),
				new SqlParameter("@City", user.City),
				new SqlParameter("@County", user.County),
				new SqlParameter("@Photo", user.Photo),
				new SqlParameter("@AccessToken", user.AccessToken),
				new SqlParameter("@AccessTokenExpired", user.AccessTokenExpired),
				new SqlParameter("@AccessTokenExpireTime", user.AccessTokenExpireTime),
				new SqlParameter("@HasSubscribed", user.HasSubscribed),
                new SqlParameter("@HasAuthorized", user.HasAuthorized)
            };

            DBHelper.CheckSqlSpParameter(parameters);
            return DBHelper.RunNonQueryProcedure(conn, SP_WeChatUserUpdate, parameters) > 0;
        }

        /// <summary>
        /// Updates WeChat user state as deleted(state = 1).
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool DeleteWeChatUserById(SqlConnection conn, int id)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@Id", id)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                SP_WeChatUserDeleteById, sqlParameters) > 0;
        }

        /// <summary>
        /// Updates WeChat user state as deleted(state = 1).
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool DeleteWeChatUserByOpenId(SqlConnection conn, string openId)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@OpenId", openId)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                SP_WeChatUserDeleteByOpenId, sqlParameters) > 0;
        }


        #endregion WeChat User

        #region User Bank Info

        /// <summary>
        /// Adds a new user bank info.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="bankInfo"></param>
        /// <returns></returns>
        public bool AddUserBankInfo(SqlConnection conn, UserBankInfo bankInfo)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@UserId", bankInfo.UserId),
			   new SqlParameter("@BankName", bankInfo.BankName ?? string.Empty),
               new SqlParameter("@BankAccount", bankInfo.BankAccount ?? string.Empty),
               new SqlParameter("@BankUserName", bankInfo.BankUserName ?? string.Empty),
               new SqlParameter("@BankAddress", bankInfo.BankAddress ?? string.Empty),
               new SqlParameter("@IsDefault", bankInfo.IsDefault),
               new SqlParameter("@CreatedTime", bankInfo.CreatedTime),
               new SqlParameter("@LastUpdatedTime", bankInfo.LastUpdatedTime)
            };

            return DBHelper.RunScalarProcedure(conn,
                SP_AddUserBankInfo, sqlParameters) > 0;
        }

        /// <summary>
        /// Updates user bank info
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="bankInfo"></param>
        /// <returns></returns>
        public bool UpdateUserBankInfo(SqlConnection conn, UserBankInfo bankInfo)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@BankId", bankInfo.BankId),
               new SqlParameter("@UserId", bankInfo.UserId),
			   new SqlParameter("@BankName", bankInfo.BankName),
               new SqlParameter("@BankAccount", bankInfo.BankAccount),
               new SqlParameter("@BankUserName", bankInfo.BankUserName),
               new SqlParameter("@BankAddress", bankInfo.BankAddress),
               new SqlParameter("@IsDefault", bankInfo.IsDefault),
               new SqlParameter("@LastUpdatedTime", bankInfo.LastUpdatedTime)
            };

            return DBHelper.RunScalarProcedure(conn,
                SP_UpdateUserBankInfo, sqlParameters) > 0;
        }

        /// <summary>
        /// Deletes user bank info by bank id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public bool DeleteUserBankInfo(SqlConnection conn, int bankId)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@BankId", bankId)
            };

            return DBHelper.RunScalarProcedure(conn,
                SP_DeleteUserBankInfo, sqlParameters) > 0;
        }

        /// <summary>
        /// Gets user bank info list.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBankInfo> GetUserBankInfos(SqlConnection conn, int userId)
        {
            List<UserBankInfo> bankInfos = new List<UserBankInfo>();

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, SP_GetUserBankInfos, sqlParameters);
            while (reader.Read())
            {
                bankInfos.Add(ConvertToBankInfoObject(reader));
            }
            if (reader != null)
            {
                reader.Close();
            }

            return bankInfos;
        }

        /// <summary>
        /// Gets user bank info by bank id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public UserBankInfo GetUserBankInfo(SqlConnection conn, int bankId)
        {
            UserBankInfo bankInfo = new UserBankInfo();

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@BankId", bankId)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, SP_GetUserBankInfo, sqlParameters);
            while (reader.Read())
            {
                bankInfo = ConvertToBankInfoObject(reader);
            }
            if (reader != null)
            {
                reader.Close();
            }

            return bankInfo;
        }

        #endregion User Bank Info

        #region User Vip Methods

        /// <summary>
        /// Selects user vip info by user id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserVip SelectUserVipInfoByUserId(SqlConnection conn, int userId)
        {
            UserVip vipInfo = null;

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };

            SqlDataReader reader = DBHelper.RunProcedure(conn, SP_SelectUserVipInfoByUserId, sqlParameters);
            while (reader.Read())
            {
                vipInfo = ConvertToUserVipObject(reader);
            }
            if (reader != null)
            {
                reader.Close();
            }

            return vipInfo ?? new UserVip();
        }

        /// <summary>
        /// Updates user vip info
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vipInfo"></param>
        /// <returns></returns>
        public bool UpdateUserVipInfo(SqlConnection conn, UserVip vipInfo)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@Id", vipInfo.Id),
               new SqlParameter("@OrderId", vipInfo.OrderId?? string.Empty),
			   new SqlParameter("@IdentifyImg", vipInfo.IdentifyImg?? string.Empty),
               new SqlParameter("@StartTime", vipInfo.StartTime?? DateTime.Now),
               new SqlParameter("@EndTime", vipInfo.EndTime?? DateTime.Now),
               new SqlParameter("@Duration", vipInfo.Duration??0),
               new SqlParameter("@Amount", vipInfo.Amount??0),
               new SqlParameter("@State", vipInfo.State??0),
               new SqlParameter("@LastUpdatedTime", vipInfo.LastUpdatedTime?? DateTime.Now)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                SP_UpdateUserVipInfoByUserId, sqlParameters) > 0;
        }

        public List<UserVip> SelectVipUserReviewList(SqlConnection conn, int startRowIndex, int pageSize, out int totalCount)
        {
            List<UserVip> vips = new List<UserVip>();
            totalCount = 0;
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(Constants.StartRowIndex, startRowIndex),
                new SqlParameter(Constants.PageSize, pageSize)
            };
            SqlDataReader reader = DBHelper.RunProcedure(conn,
                "sp_SelectVipUserReviewList", sqlParameters);
            while (reader.Read())
            {
                vips.Add(ConvertToUserVipObject(reader));
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    totalCount = reader[Constants.column_totalCount].DBToInt32();
                }
            }
            if (reader != null)
            {
                reader.Close();
            }

            return vips;
        }

        public bool DeleteVipInfo(SqlConnection conn, int id)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@Id", id)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                "sp_DeleteVipInfoById", sqlParameters) > 0;
        }

        public bool UpdateVipInfoState(SqlConnection conn, int id, int state)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@Id", id),
               new SqlParameter("@State",state)
            };

            return DBHelper.RunNonQueryProcedure(conn,
                "sp_ReviewedVipInfoById", sqlParameters) > 0;
        }

        #endregion User Vip Methods

        #region Common

        private User ConvertToUserObject(SqlDataReader reader)
        {
            User user = new User();
            user.UserId = reader["UserId"].DBToInt32();
            user.UserName = reader["UserName"].DBToString();
            user.Adress = reader["Adress"].DBToString();
            user.Cellphone = reader["Cellphone"].DBToString();
            user.CreateTime = reader["CreateTime"].DBToDateTime();
            user.Email = reader["Email"].DBToString();
            user.IdentiyImg = reader["IdentiyImg"].DBToString();
            user.LastUpdatedTime = reader["LastUpdatedTime"].DBToDateTime();
            user.EncryptedPassword = reader["EncryptedPassword"].DBToString();
            user.LocationId = reader["LocationId"].DBToString();
            user.LoginIdentiy = reader["LoginIdentiy"].DBToString();
            user.QQ = reader["QQ"].DBToString();
            user.SiteUrl = reader["SiteUrl"].DBToString();
            user.State = reader["State"].DBToInt32();
            user.UCard = reader["UCard"].DBToString();
            user.UserType = reader["UserType"].DBToInt32();
            user.Vip = reader["Vip"].DBToInt32();
            user.Photo = reader["Photo"].DBToString();
            return user;
        }

        private UserBankInfo ConvertToBankInfoObject(SqlDataReader reader)
        {
            UserBankInfo bankInfo = new UserBankInfo();

            bankInfo.BankId = reader["BankId"].DBToInt32();
            bankInfo.UserId = reader["UserId"].DBToInt32();
            bankInfo.BankName = reader["BankName"].DBToString();
            bankInfo.BankAccount = reader["BankAccount"].DBToString();
            bankInfo.BankUserName = reader["BankUserName"].DBToString();
            bankInfo.BankAddress = reader["BankAddress"].DBToString();
            bankInfo.IsDefault = reader["IsDefault"].DBToBoolean();
            bankInfo.CreatedTime = reader["CreatedTime"].DBToDateTime().Value;
            bankInfo.LastUpdatedTime = reader["LastUpdatedTime"].DBToDateTime().Value;

            return bankInfo;
        }

        private UserVip ConvertToUserVipObject(SqlDataReader reader)
        {
            UserVip vipInfo = new UserVip
            {
                Amount = reader["Amount"].DBToDecimal(0),
                CreatedTime = reader["CreatedTime"].DBToDateTime(DateTime.Now),
                Duration = reader["Duration"].DBToInt32(0),
                EndTime = reader["EndTime"].DBToDateTime(),
                Id = reader["Id"].DBToInt32(0),
                IdentifyImg = reader["IdentifyImg"].DBToString(string.Empty),
                LastUpdatedTime = reader["LastUpdatedTime"].DBToDateTime(DateTime.MinValue),
                OrderId = reader["OrderId"].DBToString(string.Empty),
                StartTime = reader["StartTime"].DBToDateTime(DateTime.MinValue),
                State = reader["State"].DBToInt32(0),
                UserId = reader["UserId"].DBToInt32(0)
            };
            return vipInfo;
        }

        private WeChatUser ConvertToWeChatUserObject(SqlDataReader reader)
        {
            WeChatUser weChatUser = new WeChatUser
            {
                AccessToken = reader["AccessToken"].DBToString(),
                AccessTokenExpired = reader["AccessTokenExpired"].DBToBoolean(),
                AccessTokenExpireTime = reader["AccessTokenExpireTime"].DBToDateTime(),
                City = reader["City"].DBToString(),
                County = reader["County"].DBToString(),
                CreatedTime = reader["CreatedTime"].DBToDateTime(),
                Id = reader["Id"].DBToInt32(),
                NickName = reader["NickName"].DBToString(),
                OpenId = reader["OpenId"].DBToString(),
                Photo = reader["Photo"].DBToString(),
                Province = reader["Province"].DBToString(),
                Sex = reader["Sex"].DBToInt32(),
                UserId = reader["UserId"].DBToNullableInt32(),
                HasSubscribed = reader["HasSubscribed"].DBToBoolean(),
                HasAuthorized = reader["HasAuthorized"].DBToBoolean()
            };

            return weChatUser;
        }

        #endregion

        /// <summary>
        /// 检查用户是否对某个资源进行过中介申请
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userId"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public int SelectTradeRecordForSpecifiedResourceByUserId(SqlConnection conn, int userId, int resourceId)
        {
            SqlParameter[] parameters = new SqlParameter[] 
            { 
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ResourceId", resourceId),
                new SqlParameter("@RecordCount", SqlDbType.Int, 4)
            };

            parameters[2].Direction = ParameterDirection.Output;
            DBHelper.CheckSqlSpParameter(parameters);

            SqlCommand cmd = new SqlCommand("sp_SelectTradeRecordForSpecifiedResourceByUserId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();

            return parameters[2].Value.DBToInt32(0);
        }
    }
}
