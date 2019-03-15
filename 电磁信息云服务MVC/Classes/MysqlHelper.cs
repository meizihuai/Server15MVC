using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace 电磁信息云服务MVC
{
    public class MysqlHelper
    {
        private string ConnectSQL = "";
        public MysqlHelper(string ConnectSQL)
        {
            this.ConnectSQL = ConnectSQL;
        }
        public MysqlHelper(string ip,string databaseName,string usr,string pwd,int port=3306)
        {
            string str = "server={0};Port={1};DataBase={2};User Id={3};Pwd={4}.;CharSet=utf8";
            str = string.Format(str, ip, port, databaseName, usr, pwd);
            this.ConnectSQL = str;
        }
        public string SQLCmd(string CmdString)
        {
            using (MySqlConnection SQL = new MySqlConnection(ConnectSQL))
            {
                try
                {
                    SQL.Open();
                    MySqlCommand SQLCommand = new MySqlCommand(CmdString, SQL);
                    int ResultRowInt = SQLCommand.ExecuteNonQuery();
                    SQL.Close();
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public DataTable SQLGetDT(string CmdString)
        {
            using (MySqlConnection SQL = new MySqlConnection(ConnectSQL))
            {
                DataTable dt = new DataTable();
                try
                {
                    SQL.Open();
                    MySqlCommand SQLCommand = new MySqlCommand(CmdString, SQL);
                    SQLCommand.CommandTimeout = 900000;
                    // Dim ResultRowInt As Integer = SQLCommand.ExecuteNonQuery()
                    MySqlDataAdapter SQLDataAdapter = new MySqlDataAdapter(SQLCommand);
                    SQLDataAdapter.Fill(dt);
                    SQL.Close();
                    return dt;
                }
                catch (Exception ex)
                {
                    // MsgBox(ex.ToString)
                    return dt;
                }
            }
        }
        public string SQLInfo(string CmdString)
        {
            using (MySqlConnection SQL = new MySqlConnection(ConnectSQL))
            {
                try
                {
                    SQL.Open();
                    MySqlCommand SQLCommand = new MySqlCommand(CmdString, SQL);
                    string str = SQLCommand.ExecuteScalar().ToString();
                    SQL.Close();
                    return str;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public bool SQLIsIn(string CmdString)
        {
            DataTable dt = SQLGetDT(CmdString);
            if (dt == null) return false;
            return dt.Rows.Count > 0;
        }


    }
}