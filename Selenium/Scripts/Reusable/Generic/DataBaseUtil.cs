using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Selenium.Scripts.Reusable.Generic
{
    public class DataBaseUtil
    {
        private SqlConnection connection;
        public String connetionString;

        /// <summary>
        /// Constructor to setup the Connecttion string
        /// </summary>
        public DataBaseUtil(String dbType, string InstanceName = "WEBACCESS", string DataSourceIP = null)
        {
            if (DataSourceIP == null)
            {
                DataSourceIP = Config.IConnectIP;
            }
            if (dbType.Equals("sqlserver"))
            {
                if (Config.IConnect_dbversion.Equals("2014"))
                {
                    this.connetionString = @"Data Source=" + DataSourceIP + @"\" + InstanceName + ";Initial Catalog=IRWSDB;Integrated Security=False;User ID=sa;Password=welcome@123";
                }
                else
                {
                    this.connetionString = @"Data Source=" + DataSourceIP + @"\" + InstanceName + ";Initial Catalog=IRWSDB;Integrated Security=False;User ID=sa;Password=welcome@123";
                }
            }

                
                
        }

        /// <summary>
        /// Constructor to setup the Connecttion string with DB Name
        /// </summary>
        public DataBaseUtil(String dbType, string DBName, string InstanceName = "WEBACCESS", string DataSourceIP=null)
        {
            if (DataSourceIP == null)
            {
                DataSourceIP = Config.IConnectIP;
            }
            if (dbType.Equals("sqlserver"))
            {
                if (Config.IConnect_dbversion.Equals("2014"))
                {
                    this.connetionString = @"Data Source=" + DataSourceIP + @"\" + InstanceName + ";Initial Catalog="+ DBName + ";Integrated Security=False;User ID=sa;Password=welcome@123";
                }
                else
                {
                    this.connetionString = @"Data Source=" + DataSourceIP + @"\" + InstanceName + ";Initial Catalog=IRWSDB;Integrated Security=False;User ID=sa;Password=welcome@123";
                }
            }



        }

        /// <summary>
        /// This method is to connect to DB
        /// </summary>
        public void ConnectSQLServerDB()
        {
            try
            {                                                     
                this.connection = new SqlConnection(this.connetionString);
                this.connection.Open();                
            }
            catch (Exception e) { throw new Exception("Not able to open DB Connecttion", e); }
        }

        /// <summary>
        /// This method is to execute given SQL.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList<String> ExecuteQuery(String sql)
        {
            IList<String> result = new List<String>();            

            try
            {              
                SqlDataAdapter adapter = new SqlDataAdapter(sql, this.connection);                
                var dataset = new System.Data.DataSet();
                adapter.Fill(dataset);
                int rowcount = dataset.Tables[0].Rows.Count;
                for (int iterate =0; iterate<rowcount; iterate++)
                {
                    result.Add(dataset.Tables[0].Rows[iterate][0].ToString());
                }
                return result;
            }
            catch (Exception e) { Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace); throw new Exception("Error in Executing sql query", e); }
        }

        /// <summary>
        /// Method To Read records in DB and store it in a Data Table
        /// </summary>
        public DataTable ReadTable(string sql)
        {
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, this.connection);
            adapter.Fill(table);
            adapter.Dispose();
            return table;
        }

        /// <summary>
        /// This method is to set given database to offline
        /// </summary>
        public void SetOffline(String DatabaseName = "IRWSDB")
        {
            try
            {
                this.connection = new SqlConnection(this.connetionString);
                this.connection.Open();
                String sqlCommandText = @"USE master;
                ALTER DATABASE " + DatabaseName + @" SET OFFLINE WITH ROLLBACK IMMEDIATE";
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, this.connection);
                sqlCommand.ExecuteNonQuery();
                this.connection.Close();
            }
            catch (Exception e) { throw new Exception("Unable to set Database to offline", e); }
        }

        /// <summary>
        /// This method is to set given database to online
        /// </summary>
        public void SetOnline(String DatabaseName = "IRWSDB")
        {
            try
            {
                this.connection = new SqlConnection(this.connetionString);
                this.connection.Open();
                String sqlCommandText = @"USE master;
                ALTER DATABASE " + DatabaseName + @" SET ONLINE";
                SqlCommand sqlCommand = new SqlCommand(sqlCommandText, this.connection);
                sqlCommand.ExecuteNonQuery();
                this.connection.Close();
            }
            catch (Exception e) { throw new Exception("Unable to set Database to online", e); }
        }
    }
}
