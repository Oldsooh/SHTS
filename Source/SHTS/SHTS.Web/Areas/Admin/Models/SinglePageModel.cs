﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class SinglePageModel : BaseModel
    {
        public SinglePage SinglePage { get; set; }

        public List<SinglePage> SinglePages { get; set; }

        public List<string> Cagetories { get; set; }
    }
}