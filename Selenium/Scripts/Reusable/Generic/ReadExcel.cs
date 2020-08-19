using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace Selenium.Scripts.Reusable.Generic
{
    internal static class ReadExcel
    {
        #region Static Methods

        /// <summary>
        ///     This function will read an excele sheet and retrives the data
        /// </summary>
        /// <param name="fileName">Path of the physical excel file from which the data has to be read.</param>
        /// <param name="sheetName">Name of the sheet in the excel file from which the data has to be read.</param>
        /// <returns>Data from the sheet in a String 2D array</returns>
        public static string[,] ReadData(string fileName, string sheetName)
        {
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Application xlApp = null;
            string[,] data, updateddata;
            try
            {
                int rCnt;
                xlApp = new Excel.Application();

                xlWorkBook = xlApp.Workbooks.Open(fileName, 0, true, 5, "", "", true, XlPlatform.xlWindows,
                                                  "\t",
                                                  false, false, 0, true);

                xlWorkSheet = xlWorkBook.Worksheets.Item[sheetName] as Excel.Worksheet;

                Excel.Range range = xlWorkSheet.UsedRange;
                data = new string[range.Rows.Count,range.Columns.Count];
                for (rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
                {
                    int cCnt;
                    for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                    {
                        var range1 = range.Cells[rCnt, cCnt] as Range;
                        if (range1 != null)
                        {
                            string str = range1.Text.ToString();
                            data[rCnt - 1, cCnt - 1] = str;
                        }
                    }
                    if (String.IsNullOrEmpty(data[rCnt - 1, 0]))
                    {
                        updateddata = new string[rCnt, range.Columns.Count];
                        Array.Copy(data, updateddata, updateddata.Length);
                        data = updateddata;
                        break;
                    }
                }               
            }
            finally
            {
                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(false, null, null);
                    Marshal.ReleaseComObject(xlWorkBook);
                    Marshal.FinalReleaseComObject(xlWorkBook);
                    xlWorkBook = null;
                    GC.Collect();
                }
                if (xlApp != null)
                {
                    xlApp.Quit();
                    Marshal.ReleaseComObject(xlApp);
                }
                if (xlWorkSheet != null)
                {
                    Marshal.ReleaseComObject(xlWorkSheet);
                    xlWorkSheet = null;
                }
            }

            return data;
        }

        /// <This method is to return a dictionary object which has all the values in array with column name as key>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Dictionary<string, object[]> GetData(string fileName, string sheetName)
        {
            Dictionary<string, object[]> data = new Dictionary<string, object[]>();            
            String[,] data1 = ReadData(fileName, sheetName);
            object[] arrcolumnvalues = new object[data1.GetUpperBound(1)];

            for (int rowcount = 0; rowcount < data1.GetLength(0); rowcount++)
            {
                for (int columncount = 0; columncount < data1.GetLength(1); columncount++)
                {
                    for (int columncount1 = 1; columncount1 < data1.GetLength(1); columncount1++)
                    {
                        arrcolumnvalues[columncount1 - 1] = data1[rowcount, columncount1];
                    }
                    data.Add(data1[0,columncount], arrcolumnvalues);
                }
            }         
           return data;
        }         

        /// <This method would return value for the given test data, column name>
        /// 
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sheetname"></param>
        /// <param name="testID"></param>
        /// <param name="columnname"></param>
        /// <returns></returns>
        public static object GetTestData(string filename, string sheetname, string testID, string columnname)
        {
            int rownumber = 0;
            int columnnumber=0;
            String[,] data;
            data = ReadData(filename, sheetname);

            //find the row number Test of id passed
            for (int i = 0; i <= data.GetUpperBound(0); i++)
            {
               if (data[i,0]==testID)
               {
                   rownumber = i;
               }
             }

            //find the column number column name passed
            for (int i = 0; i <= data.GetUpperBound(1); i++)
            {
                if (data[0, i] == columnname)
                {
                    columnnumber = i;
                }
            }

            object value = data[rownumber, columnnumber];
            return value;
        }

        /// <This method would write the value for the given test data, column name>
        /// 
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sheetname"></param>
        /// <param name="testID"></param>
        /// <param name="columnname"></param>
        /// <returns></returns>
        public static void UpdateTestData(string filename, string sheetname, string testID, string columnname, string columnvalue)
        {
            int rownumber = 0;
            int columnnumber = 0;
            String[,] data;
            data = ReadData(filename, sheetname);

            //find the row number Test of id passed
            for (int i = 0; i <= data.GetUpperBound(0); i++)
            {
                if (data[i, 0] == testID)
                {
                    rownumber = i;
                }
            }

            //find the column number column name passed
            for (int i = 0; i <= data.GetUpperBound(1); i++)
            {
                if (data[0, i] == columnname)
                {
                    columnnumber = i;
                }
            }

            UpdateCellValue(filename, sheetname, rownumber, columnnumber , columnvalue);            
        }

        /// <summary>
        /// This will update the cell value in excel
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sheetname"></param>
        /// <param name="rownumber"></param>
        /// <param name="columnnumber"></param>
        /// <param name="columnvalue"></param>
        public static void UpdateCellValue(string filename, string sheetname, int rownumber, int columnnumber, string columnvalue)
        {
            var xlApp = new Excel.Application();
            var xlWorkBook = xlApp.Workbooks.Open(filename);
            var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item[sheetname];            
            xlWorkSheet.Cells[rownumber+1, columnnumber+1] = columnvalue;
            xlWorkBook.Close(true);
            xlApp.Quit();
        }

        /// <summary>
        /// This would the required value specific to row and column.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="testID"></param>
        /// <param name="columnname"></param>
        /// <returns></returns>
        public static object GetTestData(string[,] data, string testID, string columnname)
        {
            int rownumber = 0;
            int columnnumber = 0;            

            //find the row number Test of id passed
            for (int i = 0; i <= data.GetUpperBound(0); i++)
            {
                if (data[i, 0] == testID)
                {
                    rownumber = i;
                }
            }

            //find the column number column name passed
            for (int i = 0; i <= data.GetUpperBound(1); i++)
            {
                if (data[0, i] == columnname)
                {
                    columnnumber = i;
                }
            }

            object value = data[rownumber, columnnumber];
            return value;
        }

        /// <summary>
        ///     This function releases the objects in use by the INTEROPSERVICE
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion Static Methods
    }
}