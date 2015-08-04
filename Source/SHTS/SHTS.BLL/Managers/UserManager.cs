using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.New;
using Witbird.SHTS.Model;
using UserProfile = Witbird.SHTS.Model.UserProfile;

namespace Witbird.SHTS.BLL.Managers
{
    public class UserManager
    {
        private UserRepository userRepository;

        public UserManager()
        {
            userRepository = new UserRepository();
        }

        public User GetUserById(int id)
        {
            User user = new User();
            try
            {
                user = userRepository.FindOne(u => u.UserId.Equals(id));
            }
            catch (Exception e)
            {
                LogService.Log("查询用户失败", e.ToString());
            }
            return user;
        }
    }
}
