using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;

namespace 电磁信息云服务MVC.Controllers
{
    public class DefaultController : ApiController
    {
        private MysqlHelper mysql = Module.mysql;
        [HttpGet]
        public HttpResponseMessage Test()
        {
            return Module.Response(true, "网络测试成功！", "", true);
        }
        struct Student
        {
            public string name;
            public int age;
        }
        public HttpResponseMessage GetShit(string value)
        {
            Student student = new Student()
            {
                name = value,
                age = 12
            };
            return Module.Response(true, "", student, true);
        }
        //获取设备列表
        public HttpResponseMessage GetDeviceTable()
        {
            string sql = "select * from DeviceTable";
            DataTable dt = mysql.SQLGetDT(sql);
            return Module.Response(dt, "GetDeviceTable", false);
        }

        public HttpResponseMessage GetHistroyTask(string taskName, string startTime, string endTime)
        {
            try
            {
                DateTime startDate = DateTime.Parse(startTime);
                DateTime endDate = DateTime.Parse(endTime);
                startTime = startDate.ToString("yyyy-MM-dd 00:00:00");
                endTime = endDate.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            }
            catch (Exception e)
            {
                return Module.Response(false, "起始时间或者结束时间不合法");
            }
            string sql = "select * from UserTaskTable where taskName='{0}' and startTime between '{1}' and '{2}'";
            sql = string.Format(sql, new string[] { taskName, startTime, endTime });
            DataTable dt = mysql.SQLGetDT(sql);
            return Module.Response(dt, "UserTaskTable", false);
        }




        public class Mzh
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        [HttpPost]
        public HttpResponseMessage PostTest([FromBody]Mzh Mzh)
        {
            return Module.Response(true, "", Mzh, true);
        }
    }
}
