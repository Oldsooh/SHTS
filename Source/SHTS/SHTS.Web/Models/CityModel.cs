using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class CityModel
    {
        public List<City> Provinces { get; set; }

        public string ReturnUrl { get; set; }
    }
}