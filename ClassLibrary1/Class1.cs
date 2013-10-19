using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;


namespace MainClass
{
    public class MainClassF

    {
        //SqlConnection conn = new SqlConnection(@"Data Source=ZIL-JAY\SQLEXPRESS;Initial Catalog=itsassetdb;User ID=sa;Password=abc@123");
        //SqlConnection connChange = new SqlConnection(@"Data Source=ZIL-JAY\SQLEXPRESS;Initial Catalog=itsassetchangedb;User ID=sa;Password=abc@123");

      


        
         
        public    void      AssetChangeRec(int Type , long FromRegid , long ToRegid )  
        {

            //SqlConnection conn = new SqlConnection(@"Data Source=192.168.100.184;Initial Catalog=itsassetdb;User ID=sa;Password=x7cr13a");
            //SqlConnection connChange = new SqlConnection(@"Data Source=192.168.100.186;Initial Catalog=ITSAsset_Changes;User ID=sa;Password=x7cr13a");

              SqlConnection conn = new SqlConnection(@"Data Source=10.1.4.118;Initial Catalog=itsassetdb;User ID=sa;Password=");
        SqlConnection connChange = new SqlConnection(@"Data Source=10.1.4.118;Initial Catalog=ITSAsset_Changes;User ID=sa;Password=");
            StreamWriter SW;
            SW=File.CreateText("c:\\log.txt");

            conn.Open();
            connChange.Open();

            SW.WriteLine("Application Started on " + DateTime.Now);

            SqlCommand CMD;
            SqlCommand CMD2;
            SqlDataAdapter sqldata;
            DataSet dt;
            
            //string str; 
           

            CMD = new SqlCommand();
            CMD.Connection = conn;
            CMD.CommandType=CommandType.StoredProcedure;
            if (Type==1)
            {
                CMD.CommandText="USP_ScanTimeLookUp";
            }
             
            CMD.Parameters.AddWithValue("@INREPTYPE", Type);
            CMD.Parameters.AddWithValue("@INFRMREGID", FromRegid);
            CMD.Parameters.AddWithValue("@INTOREGID", ToRegid);
             

            CMD2 = new SqlCommand();
            CMD2.Connection = connChange;

            dt=new DataSet();
            sqldata = new SqlDataAdapter(CMD);
            sqldata.Fill(dt,"1");

             foreach (DataRow dr in dt.Tables["1"].Rows)
             {
                 //  if (Convert.ToDateTime(dr["Frmdt"]) != Convert.ToDateTime(dr["todt"])) ;
                 //{
                     AssetChangeCheck(Convert.ToDateTime(dr["Frmdt"]), Convert.ToDateTime(dr["todt"]), Convert.ToInt64(dr["regid"]), conn, connChange,SW);
                     AssetChangeLookupUpdate(Type, Convert.ToInt64(dr["regid"]), Convert.ToDateTime(dr["todt"]),CMD);
                  //}
              }
              
              
             CMD.Dispose();
             SW.WriteLine("Application Ended on " + DateTime.Now);   
             SW.Close();
            
         }
        private static void AssetChangeCheck(DateTime fromdt,DateTime todate,long regid, SqlConnection cn,SqlConnection cn1,StreamWriter st)
        {
            SqlCommand CMDa;
            SqlCommand CMD2a;
            SqlDataAdapter sqldata1;
            DataSet dt2;
            CMDa=new SqlCommand();
            CMD2a=new SqlCommand();

            CMDa.Connection = cn;
            CMD2a.Connection = cn1;

            CMDa.CommandType = CommandType.StoredProcedure;
            CMDa.CommandText = "DC_CHANGE_SOFTWARE_WEB_Changed";
            CMDa.Parameters.AddWithValue("@FromDate", fromdt);
            CMDa.Parameters.AddWithValue("@ToDate", todate);
            CMDa.Parameters.AddWithValue("@RegId", regid);
            sqldata1 = new SqlDataAdapter(CMDa);
            dt2 = new DataSet();

            sqldata1.Fill(dt2, "1");

            CMD2a.Parameters.Clear();
            CMD2a.CommandType = CommandType.StoredProcedure;
            CMD2a.CommandText = "USP_AP_SoftwareChangeInsert";

          

            foreach (DataRow dr in dt2.Tables["1"].Rows)
            {
                
                AssetChangeinsertSoftware ( Convert.ToInt64(dr["regid"]),dr["MachineName"].ToString(),dr["IPAddresses"].ToString(),Convert.ToDateTime(dr["ScanDateTime"]),dr["Type"].ToString(),dr["APP_NAME"].ToString(),CMD2a);
            }
            string valueString = "Asset Change  Regid :" + regid.ToString() + " From date :" + fromdt.ToString() + " To Date: " + todate.ToString() + " " + DateTime.Now; 
            st.WriteLine(valueString);
           

          }
        private static void AssetChangeinsertSoftware(long REGID, string machinename, string IPAddress,
                DateTime ScanDateTime,string Type,string appname,SqlCommand CMD2)
        {
            CMD2.Parameters.Clear();

            CMD2.Parameters.AddWithValue("@INRegID", REGID);
            CMD2.Parameters.AddWithValue("@INMachineName", machinename);
            CMD2.Parameters.AddWithValue("@INIPAddresses", IPAddress);
            CMD2.Parameters.AddWithValue("@INScanDateTime", ScanDateTime);
            CMD2.Parameters.AddWithValue("@INType", Type);
            CMD2.Parameters.AddWithValue("@INApp_Name", appname);
            CMD2.Parameters.Add("@ERROUTPUT", DbType.Int64);
            CMD2.Parameters["@ERROUTPUT"].Direction = ParameterDirection.Output;

            CMD2.ExecuteNonQuery();

            //Errornumber = (int)CMD2.Parameters["@ERROUTPUT"].Value;
        }
        private static void AssetChangeLookupUpdate(int Type, long regid, DateTime LastScanDateTime, SqlCommand CMD)
        {
          
            CMD.Parameters.Clear();
            CMD.CommandText = "USP_ScanTimeLookUP_Update";
            CMD.Parameters.AddWithValue("@INRegID", regid);
            CMD.Parameters.AddWithValue("@INREPTYPE", Type);
            CMD.Parameters.AddWithValue("@INLastScandateTime", LastScanDateTime);
            CMD.Parameters.Add("@ERROUTPUT", DbType.Int64);
            CMD.Parameters["@ERROUTPUT"].Direction = ParameterDirection.Output;

            CMD.ExecuteNonQuery();

            //Errornumber = (int)CMD2.Parameters["@ERROUTPUT"].Value;
        }         
     }
}
