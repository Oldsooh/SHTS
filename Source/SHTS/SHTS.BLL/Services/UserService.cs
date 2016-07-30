using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Services
{
    public class UserService
    {
        private UserDao userDao;

        public UserService()
        {
            userDao = new UserDao();
        }

        #region User

        /// <summary>
        /// 根据Id得到用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUserById(int id)
        {
            User user = new User();
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                user = userDao.GetUserById(id, conn);
            }
            catch (Exception e)
            {
                LogService.Log("查询用户失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return user;
        }

        /// <summary>
        /// 根据UserName查询用户
        /// </summary>
        /// <param name="username">用户名、用户注册邮箱、注册手机号</param>
        /// <returns></returns>
        public User GetUserByUserName(string username)
        {
            User user = new User();
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                user = userDao.GetUserByUserName(username, conn);
            }
            catch (Exception e)
            {
                LogService.Log("查询用户失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return user;
        }

        /// <summary>
        /// 用户登录，成功返回User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User Login(string username, string password)
        {
            User user = null;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                password = password.ToMD5();
                user = userDao.Login(conn, username, password);
            }
            catch (Exception e)
            {
                LogService.Log("用户登录失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
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
        public bool VerifyUserInfo(string column, string value)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = userDao.VerifyUserInfo(conn, column, value);
            }
            catch (Exception e)
            {
                LogService.Log("用户注册信息验证失败-" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 用户注册，成功返回True
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UserRegister(User user)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                user.Vip = (int)VipState.Normal;
                user.EncryptedPassword = user.EncryptedPassword.ToMD5();

                int userId;
                result = userDao.UserRegister(conn, user, out userId);
            }
            catch (Exception e)
            {
                LogService.Log("用户注册失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 获取用户profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserProfile> GetUserProfiles(int userId)
        {
            List<UserProfile> result = null;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = userDao.GetUserProfiles(conn, userId);
            }
            catch (Exception e)
            {
                LogService.Log("获取用户profile--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 更新用户信息。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUserProfile(List<UserProfile> UserProfiles)
        {
            bool result = false;
            if (UserProfiles == null || UserProfiles.Count == 0)
            {
                return result;
            }
            var conn = DBHelper.GetSqlConnection();
            SqlTransaction tran = null;
            try
            {
                conn.Open();
                using (tran = conn.BeginTransaction())
                {
                    foreach (var profile in UserProfiles)
                    {
                        result = userDao.UpdateUserProfile(tran, conn, profile);
                    }
                    tran.Commit();
                }
            }
            catch (Exception e)
            {
                tran.Rollback();
                LogService.Log("修改用户信息失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 用户修改基础信息，成功返回True
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UserUpdate(User user)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = userDao.UpdateUser(conn, user);
            }
            catch (Exception e)
            {
                LogService.Log("修改用户失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 找回密码，成功返回True
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ResetUserPassword(User user)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = userDao.ResetUserPasswordById(conn, user);
            }
            catch (Exception e)
            {
                LogService.Log("用户注册失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 删除用户，成功返回True
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(int id)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = userDao.DeleteUser(conn, id);
            }
            catch (Exception e)
            {
                LogService.Log("删除用户失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 分页查询会员信息
        /// </summary>
        /// <param name="startPageIndex">开始页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="queryType">查询类型</param>
        /// <returns></returns>
        public List<User> QuersyUsers(string cityid, int resourceid,
            int startRowIndex, int pageSize, int queryType, string Keyword, out int totalCount)
        {
            List<User> users = null;
            totalCount = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                users = userDao.QuersyUsers(conn, cityid, resourceid,
                    startRowIndex, pageSize, queryType, Keyword, out totalCount);
            }
            catch (Exception e)
            {
                LogService.Log("分页查询会员信息--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
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
        public List<User> QuersyUsersByCityAndResource(string cityid, int resourceid,
            int startRowIndex, int pageSize, int queryType, string Keyword, out int totalCount)
        {
            List<User> users = null;
            totalCount = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                users = userDao.QuersyUsersByCityAndResource(conn, cityid, resourceid,
                    startRowIndex, pageSize, queryType, Keyword, out totalCount);
            }
            catch (Exception e)
            {
                LogService.Log("分页查询会员信息--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return users;
        }

        #endregion User

        #region WeChat User

        /// <summary>
        /// 微信端用户注册，成功后自动绑定该微信账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool WeChatUserRegister(User user, string openId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                user.Vip = (int)VipState.Normal;
                user.EncryptedPassword = user.EncryptedPassword.ToMD5();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    conn.Open();
                    int userId;
                    result = userDao.UserRegister(conn, user, out userId);

                    if (result)
                    {
                        WeChatUser wechatUser = userDao.GetWeChatUser(openId, conn);
                        wechatUser.UserId = userId;

                        result = userDao.UpdateWeChatUser(conn, wechatUser);

                        if (result)
                        {
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("用户注册失败--" + e.Message, e.ToString().ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 微信用户关注，注册。无失败操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool WeChatUserSubscribe(string openId, bool hasSubscribed = true, bool hasAuthorized = false)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                WeChatUser weChatUser = userDao.GetWeChatUser(openId, conn);

                if (weChatUser != null)
                {
                    weChatUser.HasSubscribed = hasSubscribed;
                    result = userDao.UpdateWeChatUser(conn, weChatUser);
                }
                else
                {
                    weChatUser = new WeChatUser();

                    weChatUser.OpenId = openId;
                    weChatUser.HasSubscribed = hasSubscribed;
                    weChatUser.HasAuthorized = hasAuthorized;
                    weChatUser.CreatedTime = DateTime.Now;

                    result = userDao.WeChatUserRegister(conn, weChatUser);
                }
            }
            catch (Exception e)
            {
                LogService.Log("微信用户注册失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 根据绑定的会员账号Id查询微信用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WeChatUser GetWeChatUser(int userId)
        {
            WeChatUser user = null;

            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();

                user = userDao.GetWeChatUser(userId, conn);
            }
            catch (Exception ex)
            {
                LogService.Log("根据userId获取微信用户失败， userId=" + userId, ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return user;
        }

        /// <summary>
        /// 根据绑定的wechatuserid获取已关注的微信用户列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<WeChatUser> GetWeChatUserOnlySubscribed()
        {
            List<WeChatUser> users = new List<WeChatUser>();

            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();

                users = userDao.GetWeChatUserOnlySubscribed(conn);
            }
            catch (Exception ex)
            {
                LogService.Log("根据绑定的wechatuserid获取已关注的微信用户列表", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return users;
        }

        /// <summary>
        /// Gets wechat user information by open id.
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public WeChatUser GetWeChatUser(string openId)
        {
            WeChatUser user = null;

            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();

                if (!string.IsNullOrEmpty(openId))
                {
                    user = userDao.GetWeChatUser(openId, conn);
                }
            }
            catch (Exception ex)
            {
                LogService.Log("根据openId获取微信用户失败， openId=" + openId, ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return user;
        }

        /// <summary>
        /// Updates wechat user information.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateWeChatUser(WeChatUser user)
        {
            bool result = false;

            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                result = userDao.UpdateWeChatUser(conn, user);
            }
            catch (Exception ex)
            {
                LogService.Log("更新微信用户信息失败", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Updates wechat user state as deleted when user unsubscribed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UnSubscribeWeChatUser(int id)
        {
            bool result = false;

            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                result = userDao.DeleteWeChatUserById(conn, id);
            }
            catch (Exception ex)
            {
                LogService.Log("删除微信用户信息失败", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Updates wechat user state as deleted when user unsubscribed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UnSubscribeWeChatUser(string openId)
        {
            bool result = false;

            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                result = userDao.DeleteWeChatUserByOpenId(conn, openId);
            }
            catch (Exception ex)
            {
                LogService.Log("删除微信用户信息失败", ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        #endregion WeChat User

        #region User Bank Info

        /// <summary>
        /// Adds a new user bank info.
        /// </summary>
        /// <param name="bankInfo"></param>
        /// <returns></returns>
        public bool AddUserBankInfo(UserBankInfo bankInfo)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                conn.Open();
                result = userDao.AddUserBankInfo(conn, bankInfo);
            }
            catch (Exception e)
            {
                LogService.Log("添加用户银行信息失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Updates a new user bank info.
        /// </summary>
        /// <param name="bankInfo"></param>
        /// <returns></returns>
        public bool UpdateUserBankInfo(UserBankInfo bankInfo)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                conn.Open();
                result = userDao.UpdateUserBankInfo(conn, bankInfo);
            }
            catch (Exception e)
            {
                LogService.Log("更新用户银行信息失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Deletes user bank info by bank id.
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public bool DeleteUserBankInfo(int bankId)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                conn.Open();
                result = userDao.DeleteUserBankInfo(conn, bankId);
            }
            catch (Exception e)
            {
                LogService.Log("删除用户银行信息失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// Gets user bank info list by user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBankInfo> GetUserBankInfos(int userId)
        {
            var conn = DBHelper.GetSqlConnection();
            List<UserBankInfo> bankInfos = new List<UserBankInfo>();

            try
            {
                conn.Open();
                bankInfos = userDao.GetUserBankInfos(conn, userId);
            }
            catch (Exception e)
            {
                LogService.Log("获取用户银行信息失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return bankInfos;
        }

        /// <summary>
        /// Gets user bank info by bank id.
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public UserBankInfo GetUserBankInfo(int bankId)
        {
            var conn = DBHelper.GetSqlConnection();
            UserBankInfo bankInfo = new UserBankInfo();

            try
            {
                conn.Open();
                bankInfo = userDao.GetUserBankInfo(conn, bankId);
            }
            catch (Exception e)
            {
                LogService.Log("获取用户银行信息失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return bankInfo;
        }


        #endregion User Bank Info

        #region User Vip Info

        /// <summary>
        /// Gets user vip info by user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserVip GetUserVipInfoByUserId(int userId)
        {
            var conn = DBHelper.GetSqlConnection();
            UserVip result = null;

            try
            {
                conn.Open();
                result = userDao.SelectUserVipInfoByUserId(conn, userId);
            }
            catch (Exception e)
            {
                LogService.Log("查询用户VIP信息失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result ?? new UserVip();
        }

        /// <summary>
        /// Updates user vip bank info
        /// </summary>
        /// <param name="vipInfoId"></param>
        /// <param name="orderId"></param>
        /// <param name="identifyImg"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="duration"></param>
        /// <param name="amount"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateUserVipInfo(int vipInfoId, string orderId, string identifyImg,
            DateTime? startTime, DateTime? endTime, int? duration, decimal? amount, VipState? state)
        {
            var conn = DBHelper.GetSqlConnection();
            bool result = false;

            try
            {
                conn.Open();
                UserVip vipInfo = new UserVip
                {
                    Id = vipInfoId,
                    OrderId = orderId,
                    IdentifyImg = identifyImg,
                    StartTime = startTime,
                    EndTime = endTime,
                    Duration = duration,
                    Amount = amount,
                    State = (int)state,
                    LastUpdatedTime = DateTime.Now
                };

                result = userDao.UpdateUserVipInfo(conn, vipInfo);
            }
            catch (Exception e)
            {
                LogService.Log("更新用户VIP信息失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 获取所有VIP申请审核
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<UserVip> GetVipUserReviewList(int startRowIndex, int pageSize, out int totalCount)
        {
            List<UserVip> vips = null;
            totalCount = 0;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                vips = userDao.SelectVipUserReviewList(conn,
                    startRowIndex, pageSize, out totalCount);
            }
            catch (Exception e)
            {
                LogService.Log("分页查询VIP会员审核信息--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return vips;
        }

        /// <summary>
        /// 删除VIP申请审核，未通过
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteVipInfo(int id, int userId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                User user = userDao.GetUserById(userId, conn);

                if (user != null)
                {
                    user.Vip = (int)VipState.Normal;
                    if (userDao.UpdateUser(conn, user))
                    {
                        result = userDao.UpdateVipInfoState(conn, id, (int)VipState.Invalid);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("删除VIP审核失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 审核VIP申请资料
        /// </summary>
        /// <param name="vipId"></param>
        /// <returns></returns>
        public bool ReviewedVipInfo(int vipId, int userId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                result = userDao.UpdateVipInfoState(conn, vipId, (int)VipState.Identified);
                if (result)
                {
                    result = userDao.UpdateUserVipState(conn, userId, (int)VipState.Identified);
                }
            }
            catch (Exception e)
            {
                LogService.Log("审核VIP审核失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }


        /// <summary>
        /// 后台设置会员1年Vip。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SetUserToVip(int userId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                UserVip vipInfo = userDao.SelectUserVipInfoByUserId(conn, userId);

                vipInfo.State = (int)VipState.VIP;
                vipInfo.StartTime = DateTime.Now;
                vipInfo.EndTime = DateTime.Now.AddYears(1);
                vipInfo.LastUpdatedTime = DateTime.Now;
                vipInfo.Duration = 1;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    result = userDao.UpdateUserVipState(conn, userId, (int)VipState.VIP);
                    if (result)
                    {
                        result = userDao.UpdateUserVipInfo(conn, vipInfo);
                    }

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                LogService.Log("升级用户到Vip--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 取消会员认证
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CancelIdentify(int userId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                UserVip vipInfo = userDao.SelectUserVipInfoByUserId(conn, userId);
                User user = userDao.GetUserById(userId, conn);

                user.IdentiyImg = string.Empty;
                user.Vip = (int)VipState.Normal;
                vipInfo.State = (int)VipState.Normal;
                vipInfo.IdentifyImg = string.Empty;
                vipInfo.StartTime = DateTime.Now;
                vipInfo.EndTime = DateTime.Now;
                vipInfo.LastUpdatedTime = DateTime.Now;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    result = userDao.UpdateUser(conn, user);
                    if (result)
                    {
                        result = userDao.UpdateUserVipInfo(conn, vipInfo);
                    }

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                LogService.Log("升级用户到Vip--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 后台强制取消设置会员Vip。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CancelUserVip(int userId)
        {
            bool result = false;
            var conn = DBHelper.GetSqlConnection();
            try
            {
                conn.Open();
                UserVip vipInfo = userDao.SelectUserVipInfoByUserId(conn, userId);
                User user = userDao.GetUserById(userId, conn);

                // 不是VIP
                if (user.Vip != (int)VipState.VIP)
                {
                    result = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(user.IdentiyImg))
                    {
                        vipInfo.IdentifyImg = string.Empty;
                        vipInfo.State = (int)VipState.Normal;
                        user.Vip = (int)VipState.Normal;
                    }
                    else
                    {
                        vipInfo.State = (int)VipState.Identified;
                        user.Vip = (int)VipState.Identified;
                    }
                    vipInfo.StartTime = DateTime.Now;
                    vipInfo.EndTime = DateTime.Now;
                    vipInfo.LastUpdatedTime = DateTime.Now;

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        result = userDao.UpdateUserVipState(conn, user.UserId, user.Vip.Value);
                        if (result)
                        {
                            result = userDao.UpdateUserVipInfo(conn, vipInfo);
                        }

                        scope.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("取消Vip会员失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        /// <summary>
        /// 检查用户VIP过期时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string CheckUserVipEndTimeWithStatusMessage(int userId)
        {
            string message = string.Empty;

            SqlConnection conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                User user = userDao.GetUserById(userId, conn);
                UserVip vipInfo = userDao.SelectUserVipInfoByUserId(conn, userId);

                if (user != null && vipInfo != null && user.Vip.HasValue && user.Vip.Value == (int)VipState.VIP)
                {
                    DateTime nowTime = DateTime.Now;
                    DateTime endTime = vipInfo.EndTime.Value;
                    int tipDays = 15;

                    if (nowTime > endTime)
                    {
                        message = "您的VIP时间已经到期，请重新申请";

                        vipInfo.State = (int)VipState.Identified;
                        vipInfo.StartTime = DateTime.Now;
                        vipInfo.EndTime = DateTime.Now;
                        vipInfo.LastUpdatedTime = DateTime.Now;

                        userDao.UpdateUserVipInfo(conn, vipInfo);
                        userDao.UpdateUserVipState(conn, user.UserId, (int)VipState.Identified);
                    }
                    else
                    {
                        int countDown = (endTime - nowTime).Days;
                        if (countDown < tipDays)
                        {
                            //message = "您的VIP会员时间还有 " + countDown + " 天即将到期，过期请重新申请";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Log("检查VIP用户过期时间", e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return message;
        }

        #endregion User Vip Info

        public bool ValidateUserResoureCommentPermission(int userId, int resourceId)
        {
            bool hasPermission = false;
            var conn = DBHelper.GetSqlConnection();

            try
            {
                conn.Open();
                hasPermission = userDao.SelectTradeRecordForSpecifiedResourceByUserId(conn, userId, resourceId) > 0;
            }
            catch (Exception e)
            {
                LogService.Log("检查用户评论中介资源权限失败--" + e.Message, e.ToString());
            }
            finally
            {
                conn.Close();
            }

            return hasPermission;
        }
    }
}
