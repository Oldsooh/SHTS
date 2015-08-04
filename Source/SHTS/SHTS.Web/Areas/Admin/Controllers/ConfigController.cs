using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Areas.Admin.Authorize;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    public class ConfigController : Controller
    {
        //
        // GET: /Admin/Config/

        [Permission(EnumRole.Editer)]
        public ActionResult Index()
        {
            Config model = new Config();

            model = ConfigManager.GetConfig();

            if (model == null)
            {
                model = new Config();
            }
            return View(model);
        }

        [Permission(EnumRole.Editer)]
        public ActionResult Edit(Config config)
        {
            if (config != null)
            {
                if (!string.IsNullOrEmpty(config.Name) &&
                    !string.IsNullOrEmpty(config.Title))
                {
                    Config model = ConfigManager.GetConfig();
                    if (model != null)
                    {
                        model.Name = config.Name;
                        model.Title = config.Title;
                        model.Keywords = config.Keywords;
                        model.Description = config.Description;
                        model.Tel = config.Tel;
                        model.Phone = config.Phone;
                        model.Domain = config.Domain;
                        model.Email = config.Email;
                        model.QQ1 = config.QQ1;
                        model.QQ2 = config.QQ2;
                        model.QQ3 = config.QQ3;
                        model.Address = config.Address;
                        model.Company = config.Company;
                        model.Weixin = config.Weixin;
                        model.ICP = config.ICP;
                        model.StatisticalCode = config.StatisticalCode;
                        ConfigManager.EditConfig(model);
                        Witbird.SHTS.Web.Public.StaticUtility.UpdateConfig();
                    }
                }
            }
            return Redirect("/admin/config/index");
        }

    }
}
