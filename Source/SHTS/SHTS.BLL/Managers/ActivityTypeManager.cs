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
    /// 活动类型
    /// </summary>
    public class ActivityTypeManager
    {
        /// <summary>
        /// 获取所有可用的活动类型
        /// </summary>
        /// <returns></returns>
        public List<ActivityType> GetAllActivityTypes()
        {
            List<ActivityType> allActivityTypes = null;
            try
            {
                allActivityTypes = Caching.Get("allActivityTypes") as List<ActivityType>;
                if (allActivityTypes == null)
                {
                    ActivityTypeRespoitory repository=new ActivityTypeRespoitory();
                    allActivityTypes = repository.FindAll(r => r.State == 0, r => r.CreatedTime, true).ToList();
                    Caching.Set("allActivityTypes", allActivityTypes);// 写入缓存
                }
            }
            catch (Exception e)
            {
                LogService.Log("获取所有可用的活动类型", e.ToString());
            }
            return allActivityTypes;
        }
    }
}
