using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models.User
{
    public class UserViewModel
    {
        public Model.User UserEntity { set; get; }

        public Dictionary<string,UserProfile> UserProfiles { set; get; }

        private SinglePage _ToVipNotice;
        public SinglePage ToVipNotice { 
            get { return _ToVipNotice ?? new SinglePage(); } 
            set { this._ToVipNotice = value; } }

        #region 参数

        private string _UCard;
        public string UCard {
            set { _UCard = value; }
            get
            {
                if (string.IsNullOrEmpty(_UCard)&&
                    UserProfiles.ContainsKey("UCard"))
                {
                    _UCard = UserProfiles["UCard"].Value;
                }
                return _UCard;
            }
        }
        public string IdentiyImgFile { set; get; }

        private string _description;
        public string description
        {
            set { _description = value; }
            get
            {
                if (string.IsNullOrEmpty(_description) && 
                    UserProfiles.ContainsKey("description"))
                {
                    _description = UserProfiles["description"].Value;
                }
                return _description;
            }
        }

        #endregion

        public string ErrorMsg { set; get; }

        /// <summary>
        /// 省份
        /// </summary>
        public List<City> Provinces { get; set; }

        /// <summary>
        /// 一级城市
        /// </summary>
        public List<City> Cities { get; set; }

        /// <summary>
        /// 二级城市、区域、商圈
        /// </summary>
        public List<City> Areas { get; set; }

        /// <summary>
        /// 用户升级成为VIP的信息
        /// </summary>
        public UserVip VipInfo { get; set; }
    }
}