namespace Witbird.SHTS.Web.Areas.Admin.Authorize
{
    public enum EnumBusinessPermission
    {
        [EnumTitle("[无]", IsDisplay = false)]
        None = 0,
        /// <summary>
        /// 管理用户
        /// </summary>
        [EnumTitle("管理管理员")]
        AccountManage_User = 101,

        /// <summary>
        /// 管理角色
        /// </summary>
        [EnumTitle("管理权限")]
        AccountManage_Role = 102,

         /// <summary>
        /// 管理用户
        /// </summary>
        [EnumTitle("删除会员")]
        MemberManage_DeleteMember = 103,

        /// <summary>
        /// 修改用户信息
        /// </summary>
        [EnumTitle("修改用户信息")]
        MemberManage_UpdateMemberInfo = 103

    }
}
