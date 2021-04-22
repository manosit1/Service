using System.Threading;
using System.Security.Cryptography;
using System.Data;
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;  

namespace workshop.DatabaseModel
{
    public class DatabaseConnect
    {
        public  string configTarget="";
        private SqlConnection cn = null;
        private SqlDataAdapter da = null;
        private SqlCommand cm = null;
        private DataTable dt = null;

        public DatabaseConnect(string key =""){
               dynamic configItem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText("DatabaseConfig.json").ToString());

            if(key==""){
               string getConfigString ="";
               if(Convert.ToBoolean(configItem["useDeveloper"])){
                    getConfigString = configItem["ConnectStringDeveloper"];
                    this.configTarget="Developer";
               }
               if(Convert.ToBoolean(configItem["useProduction"])){
                    getConfigString = configItem["ConnectStringProduction"];
                    this.configTarget="Production";
               }
                if(getConfigString!=""){
                cn = new SqlConnection (getConfigString.ToString());
                }else{
                new Exception("cannot use connectstring");
                this.configTarget="";
                }
            }else{
            bool ischeck = false;
            if(key=="ConnectStringDeveloper"){
                if(Convert.ToBoolean(configItem["useDeveloper"])){
                    ischeck = true;
                    this.configTarget="Developer";
                }
            }

            if(key=="ConnectStringProduction"){
                if(Convert.ToBoolean(configItem["useProduction"])){
                    ischeck = true;
                    this.configTarget="Production";
                }
            }
        if(ischeck==true){
            cn = new SqlConnection (configItem[key].ToString());
        }else{
            new Exception("cannot use connectstring");
            this.configTarget="";
        }
            }
        }

        public Models.DataTableString Query(string sql,List<ModelsParam> param = null){
            da = new SqlDataAdapter (sql,cn);
            dt = new  DataTable();
            if(param==null){
            da.Fill(dt);
            }else{
                    da = new SqlDataAdapter (sql,cn);
                    da.SelectCommand.CommandType  = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Clear();
                    foreach(var dataparqam in param){
                        da.SelectCommand.Parameters.AddWithValue(dataparqam.ParamName,dataparqam.Value);
                    }
                    da.Fill(dt);
            }            
            Models.DataTableString resp = new Models.DataTableString (dt);
            
            return resp;
        }
      
        public void Execute(string sql,List<ModelsParam> param = null){
            cm = new SqlCommand (sql,cn);
            if(cn.State==ConnectionState.Open){
                cn.Close();
            }
      

            cn.Open();
            if(param==null){
            cm.ExecuteNonQuery();
            }else{
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = sql;
            cm.Parameters.Clear();
            foreach(var dataparam in param){
                cm.Parameters.AddWithValue(dataparam.ParamName,dataparam.Value);
            }
            cm.ExecuteNonQuery();
            }
            cn.Close();
        }
    }

    public class ModelsParam{
        public string ParamName{get;set;}
        public string Value{get;set;}
    }
}