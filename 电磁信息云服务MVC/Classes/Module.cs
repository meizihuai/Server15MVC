using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Data;

namespace DianCiXinXiYunFuWuMVC
{
    public class Module
    {
       // public static MysqlHelper mysql = new MysqlHelper(System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"]);
        public static MysqlHelper mysql = new MysqlHelper(System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectionString"]);
       
    }
}