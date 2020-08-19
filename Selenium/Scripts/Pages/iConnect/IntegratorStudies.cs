using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using Selenium.Scripts.Reusable.Generic;

namespace Selenium.Scripts.Pages.iConnect
{
    class IntegratorStudies : BasePage
    {
        //Studies table
        public IWebElement ListTable()
        {
            try { return Driver.FindElement(By.CssSelector("table#ctl00_ctl05_m_dataListGrid")); }
            catch (NoSuchElementException) { return Driver.FindElement(By.CssSelector("table#ctl00_ctl05_parentGrid")); }
        }
        public IList<IWebElement> Intgtr_Rows()
        {
            return ListTable().FindElements(By.CssSelector("tbody>tr[title]"));
        }
        public IList<IWebElement> HisIntgtr_Rows() 
        {
            return Driver.FindElements(By.CssSelector("#gridTablePatientHistory tr[id]"));
        }
        public IWebElement Intgr_HeaderRow() { return ListTable().FindElement(By.CssSelector("tbody>tr.listHeader")); }
        public IList<IWebElement> Intgtr_CheckBoxes() { return ListTable().FindElements(By.CssSelector("span>input")); }
        public IWebElement Intgr_ViewBtn() { return Driver.FindElement(By.CssSelector("input[id$='viewButton']")); }
        public IWebElement ExpandBtn() { return Driver.FindElement(By.Id("ExpandSearchPanelButton")); }
        public IWebElement expandTextlbl() { return Driver.FindElement(By.Id("ctl00_m_studySearchControl_ExpandSearchPanelText")); }
        public IWebElement searchBtn() { return Driver.FindElement(By.Id("ctl00_m_studySearchControl_m_searchButton")); }
        public IWebElement Intgr_HTML5Btn() { return Driver.FindElement(By.CssSelector("input[id$='html5ViewButton']")); }        
        public IWebElement CompressionLabel() { return Driver.FindElement(By.CssSelector("span[id$='compressionDiv']")); }
        public By Intgr_Header() { return By.CssSelector("th:first-child, th:first-child+th, th>span[title]"); }

        public By Intgr_Row() { return By.CssSelector("tr[style]"); }
        public By Intgr_Column() { return By.CssSelector("span"); } 
        public By CloseStudy1() { return By.CssSelector("img[title='Close']"); }
        public By CloseStudy2() { return By.CssSelector("img#DivCloseImg"); } 
        public By SearchPanel() { return By.CssSelector("div#SearchPanelDiv"); }
        public By CloseButton() { return By.CssSelector("input[id$='closeButton']"); }

        /// <summary>
        /// This function is to get all study details in Integrator Study list
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string[]> GetStudiesList()
        {
            Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();
            IList<IWebElement> rows = Intgtr_Rows();

            //Sync- up
            PageLoadWait.WaitForFrameLoad(20);

            int iterate = 0;
            String Location = "";
            foreach (IWebElement row in rows)
            {
                int intColumnIndex = 0;
                IList<IWebElement> columns = row.FindElements(By.CssSelector("td>span"));
                String[] StudyDetails = new string[columns.Count];
                foreach (IWebElement column in columns)
                {
                    String columnvalue = column.Text;

                    //Copy Location in first column
                    if (column.Equals(columns[0]) && !columnvalue.Equals(""))
                    {
                        StudyDetails[intColumnIndex] = Location = column.Text;
                        intColumnIndex++;
                        continue;
                    }
                    else if (column.Equals(columns[0]) && columnvalue.Equals(""))
                    {
                        StudyDetails[intColumnIndex] = Location;
                        intColumnIndex++;
                        continue;
                    }

                    //Copy other details in following columns
                    if (!columnvalue.Equals(""))
                    {
                        StudyDetails[intColumnIndex] = columnvalue;
                        intColumnIndex++;
                    }
                }
                Array.Resize(ref StudyDetails, intColumnIndex);
                searchresults.Add(iterate++, StudyDetails);
            }

            return searchresults;
        }

        /// <summary>
        /// This function will return the column names in Integrator list
        /// </summary>
        /// <returns></returns>
        public String[] Intgr_ColumnNames()
        {
            IList<IWebElement> columns = new List<IWebElement>();
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = Intgr_HeaderRow().FindElements(By.CssSelector("th>span"));
                columns = elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
            }
            else
            {
                columns = Intgr_HeaderRow().FindElements(By.CssSelector("th:not([style*='display: none'])>span"));
            }
            String[] HeaderResults = new String[columns.Count];

            int intColumnIndex = 0;
            foreach (IWebElement column in columns)
            {
                String columnvalue = column.Text;
                if (columnvalue.Equals(" ")) { continue; }
                if (column.Displayed)
                {
                    HeaderResults[intColumnIndex] = columnvalue;
                    intColumnIndex++;
                }
            }
            Array.Resize(ref HeaderResults, intColumnIndex);
            return HeaderResults;
        }

        /// <summary>
        /// This function selects and gets the details of the study queried
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        /// <returns></returns>
        public Dictionary<string, string> Intgr_GetorSelectRow(String columnname, String columnvalue, int ToSelect = 1)
        {
            //Sync-up
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);

            Dictionary<string, string> SearchResults = new Dictionary<string, string>();
            Dictionary<int, string[]> results = GetStudiesList();
            string[] columnnames = Intgr_ColumnNames();
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, columnvalue);

            if (rowindex >= 0)
            {
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    SearchResults.Add(columnnames[iterate], value);
                    iterate++;
                }

                if (ToSelect == 1)
                {
                    //Check the checkbox in the study row
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", Intgtr_CheckBoxes()[rowindex]);
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Intgtr_CheckBoxes()[rowindex]);
                    //BasePage.wait.Until();
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Study not found");
            }

            //Sync-up
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);

            return SearchResults;
        }

        /// <summary>
        /// This method will return the column values of the given column header name
        /// </summary>
        /// <param name="columnname"></param>
        /// <returns></returns>
        public string[] Intgr_ColumnValues(String columnname)
        {
            Dictionary<int, string[]> results = GetStudiesList();
            string[] columnnames = Intgr_ColumnNames();
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            return columnvalues;
        }
    }
}
