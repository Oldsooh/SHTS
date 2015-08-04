using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL.Repositories;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    /// <summary>
    /// 后台管理员操作类
    /// </summary>
    public class AdminRoleManager
    {
        private AdminRoleRepository roleRepository;

        public AdminRoleManager()
        {
            this.roleRepository=new AdminRoleRepository();
        }

        /// <summary>
        /// 获取所有可用的角色
        /// </summary>
        /// <returns></returns>
        public List<AdminRole> GetAllRoles()
        {
            List<AdminRole> allRoles = null;
            try
            {
                allRoles = Caching.Get("AllAdminRole") as List<AdminRole>;
                if (allRoles == null)
                {
                    allRoles = roleRepository.FindAll(r => r.State == 0, r => r.LastUpdateTime, true).ToList();
                    Caching.Set("AllAdminRole", allRoles);// 写入缓存
                }
            }
            catch (Exception e)
            {
                LogService.Log("查询用户角色失败", e.ToString());
            }
            return allRoles;
        }
    }
}
