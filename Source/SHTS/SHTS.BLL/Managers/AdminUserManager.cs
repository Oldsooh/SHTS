using System;
using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.Common;
using Witbird.SHTS.Common.Extensions;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    /// <summary>
    /// 后台管理员操作类
    /// </summary>
    public class AdminUserManager
    {
        private AdminUserRepository userRepository;

        public AdminUserManager()
        {
            userRepository = new AdminUserRepository();
        }

        /// <summary>
        /// 管理员登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public List<AdminUser> GetALl()
        {
            List<AdminUser> users = null;
            try
            {
                users = userRepository.FindAll(u=>u.State == 0,u=>u.CreateTime,true).ToList();
            }
            catch (Exception e)
            {
                LogService.Log("查询用户失败", e.ToString());
            }
            return users;
        }

        /// <summary>
        /// 管理员登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AdminUser Login(string username, string password)
        {
            AdminUser user = new AdminUser();
            try
            {
                password = password.ToMD5();
                user = userRepository.FindOne(u =>
                    string.Equals(username, u.UserName) &&
                    string.Equals(password, u.EncryptedPassword)&&
                    u.State==0
                    );
            }
            catch (Exception e)
            {
                LogService.Log("查询用户失败", e.ToString());
            }
            return user;
        }

        /// <summary>
        /// 增加管理员
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public bool AddAdminUser(AdminUser admin)
        {
            bool result = false;
            try
            {
                result = userRepository.AddEntitySave(admin);
            }
            catch (Exception e)
            {
                LogService.Log("增加管理员失败", e.ToString().ToString());
                throw;
            }
            return result;
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public bool UpdateAdminUser(AdminUser admin)
        {
            bool result = false;
            try
            {
                var olgadmin = userRepository.FindOne(u => u.AdminId == admin.AdminId);
                olgadmin.LastUpdatedTime = DateTime.Now;
                olgadmin.State = admin.State;
                olgadmin.Role = admin.Role;
                olgadmin.UserName = admin.UserName;
                olgadmin.EncryptedPassword = admin.EncryptedPassword;
                userRepository.UpdateEntitySave(olgadmin);
            }
            catch (Exception e)
            {
                LogService.Log("跟新管理员失败", e.ToString().ToString());
                throw;
            }
            return result;
        }

        public bool deleteAdminUser(int id)
        {
            bool result = false;
            try
            {
                var olgadmin = userRepository.FindOne(u => u.AdminId == id);
                result = userRepository.DeleteEntitySave(olgadmin);
            }
            catch (Exception e)
            {
                LogService.Log("增加管理员失败", e.ToString().ToString());
                throw;
            }
            return result;
        }
    }
}
