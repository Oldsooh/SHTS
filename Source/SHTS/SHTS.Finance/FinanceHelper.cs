using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHTS.Finance
{
    public static class FinanceHelper
    {
        /// <summary>
        /// 对一位用户的账户余额加锁， key: 用户ID, value: 锁对象
        /// </summary>
        private static Dictionary<int, object> UserBalanceLockObjects = new Dictionary<int, object>();

        /// <summary>
        /// 根据获取用户ID获取用户账户余额对象锁。如果存在，则返回；如果不存在，则新建一个锁对象并返回。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static object GetUserBalanceLockObject(int userId)
        {
            object lockObj = null;

            if (UserBalanceLockObjects == null)
            {
                UserBalanceLockObjects = new Dictionary<int, object>();
            }

            if (UserBalanceLockObjects.Keys.Contains(userId))
            {
                lockObj = UserBalanceLockObjects[userId];

                if (lockObj == null)
                {
                    lockObj = new object();
                    UserBalanceLockObjects[userId] = lockObj;
                }
            }
            else
            {
                lockObj = new object();
                UserBalanceLockObjects[userId] = lockObj;
            }

            return lockObj;
        }

        /// <summary>
        /// 检查输入的金额是否是正确的，1： 大于或等于0， 2：最多两位小数
        /// </summary>
        /// <param name="amount">输入金额字符</param>
        /// <param name="minAmount">最低金额</param>
        /// <returns></returns>
        public static bool TryParseAmount(string amount, decimal minAmount, out decimal money)
        {
            bool isValid = false;
            money = 0;

            if (string.IsNullOrWhiteSpace(amount))
            {
                isValid = false;
            }
            else if (decimal.TryParse(amount, out money))
            {
                if (amount.Contains(".") && amount.Substring(amount.IndexOf('.') + 1).Length > 2)
                {
                    isValid = false;
                }
                else if (money <= 0)
                {
                    isValid = false;
                }
                else if (money < minAmount)
                {
                    isValid = false;
                }
                else
                {
                    isValid = true;
                }
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
