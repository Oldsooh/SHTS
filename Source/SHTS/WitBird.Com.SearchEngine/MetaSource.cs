using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WitBird.Com.SearchEngine
{
    public class MetaSource
    {
        public string Title 
        {
            set;
            get;
        }

        public string Time
        {
            set;
            get;
        }

        public DateTime CreatedTime
        {
            get;
            set;
        }

        public string Url
        {
            set;
            get;
        }

        public string Content
        {
            set;
            get;
        }

        public string Imgs
        {
            set;
            get;
        }

        public void CheckFields(string defaulltUrl)
        {
            if (this.Title == null)
            {
                this.Title = string.Empty;
            }
            if (this.Imgs == null)
            {
                this.Imgs = string.Empty;
            }
            if (this.Content == null)
            {
                this.Content = string.Empty;
            }
            if (this.Time == null)
            {
                this.Time = DateTime.Now.ToShortDateString();
            }
            if (this.Url == null)
            {
                this.Url = defaulltUrl;
            }
        }

        private String[] _ImgList;
        public String[] ImgList
        {
            get
            {
                if (_ImgList != null)
                {
                    //Nothing
                }
                else if (!string.IsNullOrEmpty(this.Imgs))
                {
                    _ImgList = Imgs.Split(Constants.split.ToArray());
                }
                return _ImgList;
            }
        }

        public string ProvinceId { get; set; }

        public string CityId { get; set; }

        public string AreaId { get; set; }

        public SearchResultType ResultType { get; set; }

        public int ResourceId { get; set; }
    }
}
