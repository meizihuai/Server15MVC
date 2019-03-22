using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DianCiXinXiYunFuWuMVC
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "default", action = "Test",id = RouteParameter.Optional }
            );
        }
    }
}
