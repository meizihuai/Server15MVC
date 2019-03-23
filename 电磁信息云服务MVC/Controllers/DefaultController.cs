using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DianCiXinXiYunFuWuMVC.Controllers
{
    /// <summary>
    /// 主要API
    /// </summary>
    public class DefaultController : ApiController
    {
        private readonly MysqlHelper mysql = Module.mysql;
        private readonly string serverUrl = WebConfigurationManager.AppSettings["serverUrl"];
        private readonly string token = "928453310";
        /// <summary>
        /// 网络测试，接口返回值测试，等
        /// </summary>
        /// <returns>返回NormalResponse</returns>
        [HttpGet]
        public NormalResponse Test()
        {
            return new NormalResponse(true, "网络测试成功！");
        }
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public NormalResponse GetDeviceTable()
        {
            string sql = "select * from DeviceTable";
            DataTable dt = mysql.SQLGetDT(sql);
            return new NormalResponse(true,"GetDeviceTable", dt);
        }
        /// <summary>
        /// 获取历史任务列表
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public NormalResponse GetHistroyTask(string taskName, string startTime, string endTime)
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
                return new NormalResponse(false, "起始时间或者结束时间不合法");
            }
            string sql = "select * from UserTaskTable where taskName='{0}' and startTime between '{1}' and '{2}'";
            sql = string.Format(sql, new string[] { taskName, startTime, endTime });
            DataTable dt = mysql.SQLGetDT(sql);
            return new NormalResponse(true, "UserTaskTable", dt);
        }
        /// <summary>
        /// MZH类
        /// </summary>
        public class Mzh
        {
            /// <summary>
            /// ID
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
        }
        /// <summary>
        /// Post测试
        /// </summary>
        /// <param name="Mzh">Class Mzh</param>
        /// <returns></returns>
        [HttpPost]
        public NormalResponse PostTest([FromBody]Mzh Mzh)
        {
            return new NormalResponse(true, "", Mzh);
        }
        /// <summary>
        /// 获取监测网关列表
        /// </summary>
        /// <returns></returns>
        public NormalResponse GetDSGWGDeviceList()
        {
            string url = serverUrl + "?func=getalldevlist&token=928453310";
            string msg = HTTPHelper.GetH(url);
            try
            {
                List<DeviceInfo> list = JsonConvert.DeserializeObject<List<DeviceInfo>>(msg);
                if (list == null)
                {
                    return new NormalResponse(false,"没有任何数据");
                }
                List<DeviceInfo> newList = (from s in list where s.Kind== "TSSGateWay" select s).ToList<DeviceInfo>();
                return new NormalResponse(true, "", newList);
            }
            catch (Exception e)
            {
                return new NormalResponse(false, e.ToString());
            }
           // return new NormalResponse(false, "没有任何数据");
        }
        /// <summary>
        /// 进程守护助手替TekDeviceControler上报电压值
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <param name="deviceId">设备ID</param>
        /// <returns></returns>
        [HttpGet]
        public NormalResponse UploadTekVoltageToServer(double voltage, string deviceId)
        {
            try
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "update busLineTable set shutdownTime='{0}',shutdownVoltage='{1}' where deviceId='{2}'";
                sql = string.Format(sql, new string[] { time, voltage + "", deviceId });
                mysql.SQLCmd(sql);
                double shutDownVoltage = 10.5;
                double.TryParse(WebConfigurationManager.AppSettings["shutDownVoltage"], out shutDownVoltage);
                string order = "";
                string msg = "已收到, deviceId = " + deviceId + "; voltage = " + voltage;
                if (voltage <= shutDownVoltage)
                {
                    order = "shutdown";
                    msg = "已收到, deviceId = " + deviceId + "; voltage = " + voltage+"，服务器规定最低电压="+ shutDownVoltage;
                }
                return new NormalResponse(true, msg, order);
            }
            catch (Exception e)
            {
                return new NormalResponse(false, e.ToString());
            }
        }
        /// <summary>
        /// 进程守护助手替TekDeviceControler上报windows即将关机
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <param name="deviceId">设备ID</param>
        /// <returns></returns>
        [HttpGet]
        public NormalResponse UploadTekWindowsShutdownToServer(double voltage, string deviceId,string msg="")
        {
            try
            {
                //日志代码 101上线，100下线，201命令，200停止,400关机
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string lastTime= DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "select * from devicelogTable where deviceId='{0}' and code=400 and time>='{1}' order by time desc ";
                sql = string.Format(sql, new string[] { deviceId, lastTime });
                DataTable dt = mysql.SQLGetDT(sql);
                if(dt!=null && dt.Rows.Count > 0)
                {
                    Task.Run<string>(()=>
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<h2>电磁信息云服务系统[重要提醒]！<h2>"+ "<br>");
                        sb.Append("<ul>");
                        sb.Append("<li>设备 " + deviceId + "在近一分钟内，反复尝试关机 " + dt.Rows.Count + " 次" + "</li>");
                        sb.Append("<li>最近一次关机请求时，电压为 <label style='color:red'>" + voltage + " V</label>" + "</li>");
                        sb.Append("<li>请登录系统，或远程登录工控机查看并解决！" + "</li>");
                        sb.Append("</ul>");
                        MailHelper.SendMail("619498477@qq.com", "设备电压过低告警！", sb.ToString());
                        return "";
                    });                   
                }
                sql = "insert into devicelogTable (time,deviceId,deviceNickName,Address,Log,Result,Status,Code) values " +
                             "('{0}','{1}','','','{2}','成功','在线',400)";
                string recordMsg = "电压过低，windows即将主动关机，电压为" + voltage;
                if (msg != "") recordMsg = recordMsg +",设备消息:"+ msg;
                 sql = string.Format(sql, new string[] { time,deviceId, recordMsg });
                string result=  mysql.SQLCmd(sql);
                string shutdownDevicelogSql = sql;
                sql = "update busLineTable set shutdownTime='{0}',shutdownVoltage='{1}' where deviceId='{2}'";
                sql = string.Format(sql, new string[] { time, voltage+"", deviceId });
                mysql.SQLCmd(sql);
                if (result == "success")
                {
                    return new NormalResponse(true, "朕已收到关机命令,deviceId=" + deviceId + ";voltage=" + voltage, result);
                }
                else
                {
                    return new NormalResponse(false, result, shutdownDevicelogSql);
                }
               
            }
            catch (Exception e)
            {
                return new NormalResponse(false, e.ToString());
            }
        }
        /// <summary>
        /// 设置监测网关状态
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="key">状态名称</param>
        /// <param name="value">状态值</param>
        /// <returns></returns>
        [HttpGet]
        public NormalResponse SetGateWayStatus(string deviceId,string key,string value)
        {         
            try
            {             
                string url = HTTPHelper.GetH(serverUrl, "?func=GetHttpMsgUrlById&deviceID=" + deviceId + "&token=" + token);
                if (url == "")
                {
                    return new NormalResponse(false, "设备不在线");
                }
                string result = "";
                string param= "";
                if (key == "power")
                {
                    param = (value == "off" ? "func=poweroff" : "func=poweron");                   
                }
                if (key == "net")
                {
                    param = (value == "in" ? "func=netswitchin" : "func=netswitchout");
                }
                if (param == "")
                {
                    return new NormalResponse(true, "未知命令！");
                }
                param = param + "&token=" + token;
                url = url + "?" + param;
                result = HTTPHelper.GetH(url, "");
                if (GetNorResult("result", result) == "success")
                {
                    return new NormalResponse(true, "操作成功！", url);
                }
                else
                {
                    return new NormalResponse(true, "操作失败！"+ GetNorResult("msg", result), url);
                }
            }
            catch (Exception e)
            {
                return new NormalResponse(false, e.ToString());
            }
        }
        //获取旧版NorResult result=success;msg=这是消息;
        private string GetNorResult(string key,string str)
        {
            if (!str.Contains(";")) return "";
            foreach(string itm in str.Split(';'))
            {
                if (itm.Contains("="))
                {
                    string k = itm.Split('=')[0];
                    string v = itm.Split('=')[1];
                    if (k == key)
                    {
                        return v;
                    }
                }
            }
            return "";
        }
    }
}
