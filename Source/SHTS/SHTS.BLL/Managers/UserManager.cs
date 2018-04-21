using System;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

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
