using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Selenium.Scripts.Reusable.Generic;

namespace UpgradeUtility
{
    /// <summary>
    /// DatabaseComparisonUtility
    /// </summary>
    class DatabaseComparisonUtility
    {
        private string m_outputPath;
        private string m_connectionString = string.Empty;
        private string m_IgnoredTablesListName = Constants.Input.IgnoredTablesList;
        private bool m_useIgnoredTablesList = false;
        private List<string> m_IgnoredTablesList = new List<string>();

        /// <summary>
        /// DatabaseComparisonUtility
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="connectionString"></param>
        /// <param name="useIgnoredTablesList"></param>
        /// <param name="ignoredTablesList"></param>
        public DatabaseComparisonUtility(string outputPath, string connectionString, 
            bool useIgnoredTablesList = false, string ignoredTablesList = null)
        {
            m_connectionString = connectionString;
            m_outputPath = outputPath;
            m_useIgnoredTablesList = useIgnoredTablesList;
            
            if (!string.IsNullOrEmpty(ignoredTablesList))
            {
                m_IgnoredTablesListName = ignoredTablesList;
            }

            //create output folder
            if (!Directory.Exists(m_outputPath))
            {
                Directory.CreateDirectory(m_outputPath);
            }            

            if (m_useIgnoredTablesList)
            {
                //read ignored tables list
                ReadIgnoredTablesList();
            }
        }


        /// <summary>
        /// GetDatabaseInfo
        /// </summary>
        /// <param name="databaseName"></param>
        public void GetDatabaseInfo(string databaseName)
        {
            try
            {
                //create output folder
                string schemaOutputPath = Path.Combine(m_outputPath, "Schema");
                if (!Directory.Exists(schemaOutputPath))
                {
                    Directory.CreateDirectory(schemaOutputPath);
                }

                string command = string.Format("USE {0};", databaseName);
                ExecuteNonQuery(command, System.Data.CommandType.Text, null);

                //get contraints info
                command = string.Format(Constants.CommandStrings.SCHEMA_TABLE_CONSTRAINTS, databaseName);
                DataSet ds = ExecuteQuery(command, System.Data.CommandType.Text, null);
                SaveDataSet(ds, (Path.Combine(schemaOutputPath, "SCHEMA_TABLE_CONSTRAINTS.txt")));

                //get procedures info
                ds = ExecuteQuery(Constants.CommandStrings.SYS_PROCEDURES, System.Data.CommandType.Text, null);
                SaveDataSet(ds, (Path.Combine(schemaOutputPath, "SYS_PROCEDURES.txt")));

                //get sys.tables
                DataSet sysTables = ExecuteQuery(Constants.CommandStrings.SYS_TABLES, System.Data.CommandType.Text, null);
                SaveDataSet(sysTables, (Path.Combine(schemaOutputPath, "SYS_TABLES.txt")));

                //get schema columns
                command = string.Format(Constants.CommandStrings.SCHEMA_COLUMNS, databaseName);
                DataSet schemaColumns = ExecuteQuery(command, System.Data.CommandType.Text, null);
                SaveDataSet(schemaColumns, (Path.Combine(schemaOutputPath, "SCHEMA_COLUMNS.txt")));

                //get all tables info
                GetDatabaseTablesInfo(databaseName, sysTables, schemaColumns);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }

        /// <summary>
        /// GetDatabaseTablesInfo
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="sysTables"></param>
        /// <param name="schemaColumns"></param>
        private void GetDatabaseTablesInfo(string databaseName, DataSet sysTables, DataSet schemaColumns)
        {
            try
            {
                //create output folder
                string dbOutputPath = Path.Combine(m_outputPath, databaseName);
                if (!Directory.Exists(dbOutputPath))
                {
                    Directory.CreateDirectory(dbOutputPath);
                }

                DataTableCollection tables = sysTables.Tables;
                foreach (DataTable table in tables)
                {
                    DataRowCollection rows = table.Rows;

                    foreach (DataRow row in rows)
                    {
                        var items = row.ItemArray;
                        foreach (var item in items)
                        {
                            if (!m_IgnoredTablesList.Contains(item.ToString()))
                            {
                                DataSet ds = ExecuteQuery(GetTableCommandStr(schemaColumns, item.ToString()), System.Data.CommandType.Text, null);
                                SaveDataSet(ds, (Path.Combine(dbOutputPath, string.Format("{0}.txt", item.ToString()))));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }
        /// <summary>
        /// GetTableCommandStr
        /// </summary>
        /// <param name="schemaColumns"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetTableCommandStr(DataSet schemaColumns, string tableName)
        {
            string selectStr = string.Empty;
            string orderStr = string.Empty;
            string command = string.Empty;
            try
            {
                if (schemaColumns != null)
                {
                    string[] excludeOrder = { "text", "ntext", "datetime" };
                    string[] excludeSelect = { "datetime" };

                    int tableNameIndex = schemaColumns.Tables[0].Columns.IndexOf("TABLE_NAME");
                    int columnNameIndex = schemaColumns.Tables[0].Columns.IndexOf("COLUMN_NAME");
                    int dataTypeIndex = schemaColumns.Tables[0].Columns.IndexOf("DATA_TYPE");

                    DataRowCollection rows = schemaColumns.Tables[0].Rows;
                    foreach (DataRow row in rows)
                    {
                        var items = row.ItemArray;
                        if (items[tableNameIndex].ToString().Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!excludeSelect.Contains(items[dataTypeIndex].ToString().ToLower()))
                            {
                                if (string.IsNullOrEmpty(selectStr))
                                {
                                    selectStr = string.Format("select {0}", items[columnNameIndex].ToString());
                                }
                                else
                                {
                                    selectStr += string.Format(", {0}", items[columnNameIndex].ToString());

                                }

                                if (!excludeOrder.Contains(items[dataTypeIndex].ToString().ToLower()))
                                {
                                    if (string.IsNullOrEmpty(orderStr))
                                    {
                                        orderStr = string.Format("order by {0} asc", items[columnNameIndex].ToString());
                                    }
                                    else
                                    {
                                        orderStr += string.Format(", {0} asc", items[columnNameIndex].ToString());

                                    }
                                }
                            }
                        }
                    }

                    command = string.Format("{0} from [dbo].[{1}] {2}", selectStr, tableName, orderStr);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }

            return command;
        }

        /// <summary>
        /// ReadIgnoredTablesList
        /// </summary>
        private void ReadIgnoredTablesList()
        {
            try
            {
                string line;
                System.IO.StreamReader file =
                    new System.IO.StreamReader(m_IgnoredTablesListName);
                m_IgnoredTablesList = new List<string>();

                while ((line = file.ReadLine()) != null)
                {
                    if ((!line.StartsWith(Constants.Input.CommentedLineCharacter)) && (!string.IsNullOrEmpty(line)))
                    {
                        m_IgnoredTablesList.Add(line);
                    }
                }

                file.Close();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }

        /// <summary>
        /// SaveDataSet
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="fileName"></param>
        private void SaveDataSet(DataSet ds, string fileName)
        {
            try
            {
                Logger.Instance.InfoLog("Save " + Path.GetFileName(fileName));

                DataTableCollection tables = ds.Tables;

                string tableStr = string.Empty;
                foreach (DataTable table in tables)
                {
                    DataRowCollection rows = table.Rows;

                    string rowStr = string.Empty;
                    foreach (DataRow row in rows)
                    {
                        var items = row.ItemArray;

                        string itemstr = string.Empty;
                        foreach (var item in items)
                        {
                            itemstr += (item.ToString() + "\t");
                        }

                        rowStr += (itemstr + "\n");
                    }

                    tableStr += (rowStr + "\n\n");
                }

                System.IO.File.WriteAllText(fileName, tableStr);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }
        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandParameters"></param>
        private void ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] commandParameters)
        {
            try
            {
                Logger.Instance.InfoLog(commandText);

                using (var connection = new SqlConnection(m_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    if (commandParameters != null)
                    {
                        command.Parameters.AddRange(commandParameters);
                    }
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }
        /// <summary>
        /// ExecuteQuery
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private DataSet ExecuteQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();

            try
            {
                Logger.Instance.InfoLog(commandText);

                using (var connection = new SqlConnection(m_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }

            return ds;
        }
    }

    #region constants
    /// <summary>
    /// Constants
    /// </summary>
    static class Constants
    {
        public static class CommandStrings
        {
            public const string SCHEMA_TABLE_CONSTRAINTS = "select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where table_catalog='{0}' order by CONSTRAINT_NAME asc";
            public const string SYS_PROCEDURES = "select name from Sys.Procedures order by name asc";
            public const string SYS_TABLES = "select name from Sys.Tables order by name asc";
            public const string SCHEMA_COLUMNS = "select * from INFORMATION_SCHEMA.COLUMNS where table_catalog='{0}' order by TABLE_NAME asc, ORDINAL_POSITION asc";
        }
        public static class Input
        {
            public const string IgnoredTablesList = @".\Scripts\UpgradeUtility\IgnoredTablesList.txt";
            public const string CommentedLineCharacter = "#";
        }
    }

    #endregion
}
