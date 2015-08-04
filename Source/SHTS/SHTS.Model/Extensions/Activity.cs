using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    public partial class Activity
    {
        public const string SPLT = ",";
        public const string SPLTH = "|";
        public const char SPLTD = '.';
        public const string SMALL = "{0}_small.{1}";

        private string[] _ImgUrls;
        public string[] ImgUrls
        {
            get
            {
                if (_ImgUrls == null)
                {
                    if (!string.IsNullOrEmpty(ImageUrl))
                    {
                        _ImgUrls = ImageUrl.Split(SPLTH.ToCharArray());
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
                    _SmallImgUrls=new List<string>();
                    if (ImgUrls!=null && ImgUrls.Length>0)
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

        private string[] geos;
        public string Province {
            get
            {
                if (geos==null||geos.Length==0)
                {
                    if (!string.IsNullOrEmpty(LocationId))
                    {
                        geos = LocationId.Split(SPLT.ToCharArray());
                    }
                    else
                    {
                        geos=new string[1]{string.Empty};
                    }
                }
                return geos[0];
            }
        }

        public string City
        {
            get
            {
                if (geos == null || geos.Length == 0)
                {
                    if (!string.IsNullOrEmpty(LocationId))
                    {
                        geos = LocationId.Split(SPLT.ToCharArray());
                    }
                    else
                    {
                        geos = new string[1] { string.Empty };
                    }
                }
                return geos.Length >= 2 ? geos[1] : string.Empty;
            }
        }

        public string Area
        {
            get
            {
                if (geos == null || geos.Length == 0)
                {
                    if (!string.IsNullOrEmpty(LocationId))
                    {
                        geos = LocationId.Split(SPLT.ToCharArray());
                    }
                    else
                    {
                        geos = new string[1] { string.Empty };
                    }
                }
                return geos.Length>=3 ? geos[2]:string.Empty;
            }
        }

        public string UserName { set; get; }
    }
}
