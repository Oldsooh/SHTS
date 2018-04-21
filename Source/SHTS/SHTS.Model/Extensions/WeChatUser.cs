namespace Witbird.SHTS.Model
{
    public partial class WeChatUser
    {
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsUserIdentified { get; set; }

        /// <summary>
        /// 是否是VIP
        /// </summary>
        public bool IsUserVip { get; set; }

        /// <summary>
        /// 是否绑定账号
        /// </summary>
        public bool IsUserLoggedIn { get; set; }
    }
}
