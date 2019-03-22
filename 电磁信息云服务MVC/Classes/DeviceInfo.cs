using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DianCiXinXiYunFuWuMVC
{
    public class DeviceInfo
    {
        public string DeviceID;
        public string Name;
        public string Address;
        public string Kind;
        public string OnlineTime;
        public string Statu;
        public string Lng;
        public string Lat;
        public string IP;
        public int Port;
        public string RunKind;
        public string sbzt;
        public string Func;
        public bool isNetGateWay;
        public string NetDeviceID;
        public string NetGateWayID;
        public int NetSwitch;
        public string HTTPMsgUrl;
        public GateWayStatusInfo DSGWGstatus;
    }
    public class GateWayStatusInfo
    {
        public string net;
        public string power;
        public double lon;
        public double lat;
        public double voltage;
    }
}