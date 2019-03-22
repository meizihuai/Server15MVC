using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace DianCiXinXiYunFuWuMVC
{
    public class HTTPHelper
    {
        public static string GetH(string uri, string msg="")
        {
            int num = 0;
            while (num<3)
            {
                try
                {
                    HttpWebRequest req =(HttpWebRequest) WebRequest.Create(uri + msg);
                    req.Accept = "*/*";
                    req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
                    req.CookieContainer = new CookieContainer();
                    req.KeepAlive = true;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Method = "GET";
                    req.Timeout = 3000;
                    req.ReadWriteTimeout = 3000;
                    byte[] b = Encoding.Default.GetBytes(msg);
                    HttpWebResponse rp =(HttpWebResponse) req.GetResponse();
                    string str = new StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    b = Encoding.Default.GetBytes(str);
                    return str;
                }
                catch (Exception ex)
                {
                }
                num = num + 1;               
            }
            return "";
        }
        public static string PostH(string uri, string msg)
        {
            // While True
            try
            {
                // ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf CheckValidationResult)
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Accept = "*/*";
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
                req.KeepAlive = true;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                string data = msg;
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                Stream st = req.GetRequestStream();
                st.Write(bytes, 0, bytes.Length);
                st.Flush();
                st.Close();
                byte[] b = Encoding.Default.GetBytes(msg);
                HttpWebResponse rp = (HttpWebResponse)req.GetResponse();
                string str = new StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                b = Encoding.Default.GetBytes(str);
                return str;
            }
            catch (Exception ex)
            {
            }
            return "";
        }


    }
}