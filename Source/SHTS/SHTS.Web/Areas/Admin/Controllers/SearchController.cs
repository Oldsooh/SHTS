﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using Newtonsoft.Json;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Criteria;
using WitBird.Com.SearchEngine;
using Constants = WitBird.Com.SMS.Constants;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class SearchController : AdminBaseController
    {
        #region 搜索

        public const string IndexPath = "~/IndexData";
        public const string DictPath = @"~/Config/PanGu.xml";
        public const string HSplit = "|";
        public const string split = ",";

        #endregion

        /// <summary>
        /// 生成搜索索引库
        /// </summary>
        /// <returns></returns>
        public ActionResult SiteSerach()
        {
            PublicConfig serachConfig = null;
            try
            {
                serachConfig = GetConfig();
            }
            catch (Exception e)
            {
                LogService.Log("Cretae Serach index", e.StackTrace);
            }
            return View(serachConfig);
        }

        [HttpPost]
        public ActionResult SiteSerach(FormCollection form)
        {
            PublicConfig serachConfig = null;
            try
            {
                DateTime LastUpdateTime;
                string result = string.Empty;
                const string http = "http://{0}/";
                string baseroot = string.Format(http, Request.Url.Authority);

                serachConfig = GetConfig();

                LastUpdateTime = serachConfig.LastUpdatedTime;
                IndexManager indexManager =
                    new IndexManager(Server.MapPath(IndexPath), Server.MapPath(DictPath));
                
                List<MetaSource> sourceList = new List<MetaSource>();
                MetaSource source = null;

                int newAddIndex = 0;

                #region 活动

                ActivityService activityService=new ActivityService();
                List<Activity> activities = activityService.QueryActivities(
                    new QueryActivityCriteria
                    {
                        QueryType = 6,
                        LastUpdatedTime = LastUpdateTime,
                        PageSize = int.MaxValue,
                        StartRowIndex = 1,
                        
                    });
                if (activities != null && activities.Count > 0)
                {
                    foreach (Activity n in activities)
                    {
                        source = new MetaSource()
                        {
                            Title = n.Title,
                            Time = n.CreatedTime.Value.ToShortDateString()
                        };
                        source.Url = GenFullUrl(baseroot, n.Id, "activity");
                        source.Imgs = string.Empty;
                        if (n.ImgUrls != null && n.ImgUrls.Length > 0)
                        {
                            foreach (var img in n.ImgUrls)
                            {
                                source.Imgs = source.Imgs + split + img;
                            }
                            source.Imgs = source.Imgs.TrimStart(split.ToCharArray());
                        }
                        // 过滤内容
                        source.Content = ParseTags(n.ContentStyle);
                        source.CheckFields(baseroot);
                        sourceList.Add(source);
                        newAddIndex++;
                    }
                }

                #endregion

                #region 资源

                ResourceManager resourceManager = new ResourceManager();
                var resources = resourceManager.GetResourcesByTime(LastUpdateTime);
                if (resources != null && resources.Count > 0)
                {
                    foreach (Witbird.SHTS.DAL.New.Resource n in resources)
                    {
                        source = new MetaSource()
                        {
                            Title = n.Title,
                            Time = n.LastUpdatedTime.ToShortDateString()
                        };
                        source.Url = GenFullUrl(baseroot, n.Id, "Resource");
                        source.Imgs = string.Empty;
                        if (n.ImgUrls != null && n.ImgUrls.Length > 0)
                        {
                            foreach (var img in n.ImgUrls)
                            {
                                source.Imgs = source.Imgs + split + img;
                            }
                            source.Imgs = source.Imgs.TrimStart(split.ToCharArray());
                        }
                        // 过滤内容
                        source.Content = ParseTags(n.Description);
                        source.CheckFields(baseroot);
                        sourceList.Add(source);
                    }
                }

                #endregion

                #region 需求

                DemandManager demandManager = new DemandManager();
                var demands = demandManager.QueryDemandsByTime(LastUpdateTime);
                if (demands != null && demands.Count > 0)
                {
                    foreach (Demand n in demands)
                    {
                        source = new MetaSource()
                        {
                            Title = n.Title,
                            Time = n.InsertTime.ToShortDateString()
                        };
                        source.Url = GenFullUrl(baseroot, n.Id, "Demand");
                        source.Imgs = string.Empty;
                        // 过滤内容
                        source.Content = ParseTags(n.Description);
                        source.CheckFields(baseroot);
                        sourceList.Add(source);
                    }
                }

                #endregion

                indexManager.AddIndexByData(sourceList);
                serachConfig.LastUpdatedTime=DateTime.Now;
                UpdateConfig(serachConfig);
            }
            catch (Exception e)
            {
                LogService.Log("Cretae Serach index", e.StackTrace);
            }
            return View(serachConfig);
        }

        private PublicConfig GetConfig()
        {
            PublicConfig serachConfig = null;
            FileConfigService fileConfigService = new FileConfigService();
            var SMSConfig = fileConfigService.GetConfig("SiteSerach.json");
            if (!string.IsNullOrEmpty(SMSConfig))
            {
                serachConfig =
                    JsonConvert.DeserializeObject<PublicConfig>(SMSConfig);
            }
            else
            {
                serachConfig = new PublicConfig();
                serachConfig.ConfigName = "站内搜索配置";
                serachConfig.ConfigValue = "0";
                serachConfig.CreatedTime = DateTime.Now;
                serachConfig.LastUpdatedTime = new DateTime(2014,1,1);
            }
            return serachConfig;
        }

        private void UpdateConfig(PublicConfig serachConfig)
        {
            FileConfigService fileConfigService = new FileConfigService();
            fileConfigService.SaveConfig("SiteSerach.json",serachConfig.ToJson());
        }

        private String GenFullUrl(string baseUrl,int itemId,
            string channel,bool isDetails=false)
        {
            if (isDetails)
            {
                return string.Concat(baseUrl, CreateDetailsUrl(channel, itemId));
            }
            return string.Concat(baseUrl, CreateShowUrl(channel,itemId));
        }

        public static string CreateShowUrl(string channel, int id)
        {
            const string htmlUrl = "{0}/{1}.html";
            const string sharp = "#";
            if (string.IsNullOrEmpty(channel) || id == 0)
            {
                return sharp;
            }
            return string.Format(htmlUrl, channel, id.ToString());
        }
        public static string CreateDetailsUrl(string channel, int id)
        {
            const string htmlUrl = "/resource/{0}/{1}";
            const string sharp = "#";
            if (string.IsNullOrEmpty(channel) || id == 0)
            {
                return sharp;
            }
            return string.Format(htmlUrl, channel, id.ToString());
        }
        /// <summary>
        /// 去除Html标签
        /// </summary>
        /// <param name="HTMLStr"></param>
        /// <returns></returns>
        public static string ParseTags(string HTMLStr)
        {
            if (string.IsNullOrEmpty(HTMLStr))
            {
                return string.Empty;
            }
            return System.Text.RegularExpressions.Regex.Replace(HTMLStr, "<[^>]*>", "");
        }
    }
}