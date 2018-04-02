using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Witbird.SHTS.Web.Models
{
    public class ResourceViewModel
    {
        /// <summary>
        /// 编辑资源用到的
        /// </summary>
        public string ResId { get; set; }

        /// <summary>
        /// 资源类别
        /// 只能是 1 2 3 4
        /// </summary>
        [Required(ErrorMessage = "请选择资源类别")]
        [RegularExpression("[1234]", ErrorMessage = "资源类别选择有误")]
        public string ResType { get; set; }

        /// <summary>
        /// (标题)资源名称
        /// </summary>
        [Required(ErrorMessage = "必填，请认真填写，方便提供商看到.")]
        public string Title { get; set; }

        /// <summary>
        /// ProvinceId
        /// </summary>
        public string ddlProvince { get; set; }

        /// <summary>
        /// CityId
        /// </summary>
        public string ddlCity { get; set; }

        /// <summary>
        /// AreaId
        /// </summary>
        public string ddlArea { get; set; }

        /// <summary>
        /// 是否可以友情链接
        /// </summary>
        public bool CanFriendlyLink { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// 详细介绍
        /// </summary>
        [Required(ErrorMessage = "必填，1000字最佳，太少大多都不好. 用于列表页简短描述.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "请填写该资源的报价")]
        public int QuotePrice { get; set; }

        [Required(ErrorMessage = "请填写详细地址")]
        public string DetailAddress { get; set; }

        /// <summary>
        /// 资源图片列表
        /// 图片（最多可以上传8张图片），第一张默认为封面，每张最大10MB，支持jpg/gif/png格式
        /// <example>活动场地照片、演员照片</example>
        /// </summary>
        public string ImageUrls { get; set; }

        private List<string> _originalImgUrls;
        public List<string> OriginalImgUrls
        {
            get
            {
                if (_originalImgUrls == null)
                {
                    _originalImgUrls = new List<string>();
                    if (!string.IsNullOrEmpty(ImageUrls))
                    {
                        foreach (var item in ImageUrls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                        {
                            var img = item.Split('.');
                            if (img.Length >= 2)
                            {
                                _originalImgUrls.Add(string.Format("{0}.{1}", img[0], img[1]));
                            }
                            else
                            {
                                _originalImgUrls.Add(item);
                            }
                        }
                    }
                    else
                    {
                        _originalImgUrls.Add(string.Empty); ;
                    }
                }
                return _originalImgUrls;
            }
        }

        private List<string> _SmallImgUrls;
        public List<string> SmallImgUrls
        {
            get
            {
                if (_SmallImgUrls == null)
                {
                    _SmallImgUrls = new List<string>();
                    if (!string.IsNullOrEmpty(ImageUrls))
                    {
                        foreach (var item in ImageUrls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                        {
                            var img = item.Split('.');
                            if (img.Length >= 2)
                            {
                                _SmallImgUrls.Add(string.Format("{0}_small.{1}", img[0], img[1]));
                            }
                            else
                            {
                                _SmallImgUrls.Add(item);
                            }
                        }
                    }
                    else
                    {
                        _SmallImgUrls.Add(string.Empty); ;
                    }
                }
                return _SmallImgUrls;
            }
        }

        ///// <summary>
        ///// 开始时间
        ///// </summary>
        //[RegularExpression(@"\d{4}-\d{2}-\d{2}", ErrorMessage = "开始时间设置不正确")]
        //public string StartDate { get; set; }

        ///// <summary>
        ///// 结束时间
        ///// </summary>
        //[RegularExpression(@"\d{4}-\d{2}-\d{2}", ErrorMessage = "结束时间设置不正确")]
        //public string EndDate { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        public List<string> VideoUrl { get; set; }

        #region 联系信息
        /// <summary>
        /// 联系人
        /// </summary>
        [Required(ErrorMessage = "必填，请至少填写一个联系人")]
        public string Contact { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        //[Required(ErrorMessage = "请填写您的固定电话")]
        public string Telephone { get; set; }

        /// <summary>
        /// 移动电话
        /// </summary>
        //[Required(ErrorMessage = "请填写您的移动电话")]
        public string Mobile { get; set; }

        /// <summary>
        /// 微信账号
        /// </summary>
        public string WeChat { get; set; }
        #endregion

        #region 活动场地
        /// <summary>
        /// 场地类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "场地类型设置不正确")]
        public string SpaceTypeId { get; set; }

        /// <summary>
        /// 场地特点
        /// </summary>
        [RegularExpression(@"\d+(,\d+)*", ErrorMessage = "场地特点设置不正确")]
        public string SpaceFeatures { get; set; }

        /// <summary>
        /// 配套设备
        /// </summary>
        [RegularExpression(@"\d+(,\d+)*", ErrorMessage = "场地配套设备设置不正确")]
        public string SpaceFacilities { get; set; }

        /// <summary>
        /// 场地面积选项
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "场地面积填写不正确")]
        public string SpaceSizeId { get; set; }

        /// <summary>
        /// 容纳人数选项
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "容纳人数填写不正确")]
        public string SpacePeopleId { get; set; }

        /// <summary>
        /// 能否提供酒宴
        /// 1代表能提供 2代表不能提供
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "是否提供酒宴填写不正确")]
        public string SpaceTreat { get; set; }
        #endregion

        #region 演艺人员
        /// <summary>
        /// 演员类别
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "演员类别设置不正确")]
        public string ActorTypeId { get; set; }

        [RegularExpression(@"\d+", ErrorMessage = "演员来源设置不正确")]
        public string ActorFromId { get; set; }

        [RegularExpression(@"\d+", ErrorMessage = "演员性别设置不正确")]
        public string ActorSex { get; set; }
        #endregion

        #region 活动设备
        /// <summary>
        /// 设备类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "设备类型设置不正确")]
        public string EquipTypeId { get; set; }
        #endregion

        #region 其他资源
        /// <summary>
        /// “其他资源”的类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "此类型不存在")]
        public string OtherTypeId { get; set; }
        #endregion


    }
}