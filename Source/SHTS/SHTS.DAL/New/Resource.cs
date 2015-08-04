using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.DAL.New
{
    public partial class Resource
    {
        public const string SPLT = ",";
        public const string SPLTH = "|";
        public const char SPLTD = '.';
        public const string SMALL = "{0}_small.{1}";
        public const string BIG = "{0}_big.{1}";

        private string[] _ImgUrls;
        public string[] ImgUrls
        {
            get
            {
                if (_ImgUrls == null)
                {
                    if (!string.IsNullOrEmpty(ImageUrls))
                    {
                        _ImgUrls = ImageUrls.Split(SPLTH.ToCharArray());
                    }
                    else
                    {
                        _ImgUrls = new string[1] { string.Empty };
                    }
                }
                return _ImgUrls;
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
                    if (ImgUrls != null && ImgUrls.Length > 0)
                    {
                        foreach (var item in ImgUrls)
                        {
                            var img = item.Split(SPLTD);
                            if (img.Length >= 2)
                            {
                                _SmallImgUrls.Add(string.Format(SMALL, img[0], img[1]));
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

        private List<string> _BigImgUrls;
        public List<string> BigImgUrls
        {
            get
            {
                if (_BigImgUrls == null)
                {
                    _BigImgUrls = new List<string>();
                    if (ImgUrls != null && ImgUrls.Length > 0)
                    {
                        foreach (var item in ImgUrls)
                        {
                            var img = item.Split(SPLTD);
                            if (img.Length >= 2)
                            {
                                _BigImgUrls.Add(string.Format(BIG, img[0], img[1]));
                            }
                            else
                            {
                                _BigImgUrls.Add(item);
                            }
                        }
                    }
                    else
                    {
                        _BigImgUrls.Add(string.Empty); ;
                    }
                }
                return _BigImgUrls;
            }
        }

        public List<Comment> CommentList { get; set; }

        public string StateView
        {
            get
            {
                string state = "非法状态";
                switch (State)
                {
                    case 0x0001:
                        state = "已创建";
                        break;
                    case 0x0010:
                        state = "已审核";
                        break;
                    case -1:
                        state = "已删除";
                        break;
                    default:
                        break;
                }
                return state;
            }
        }
    }
}
