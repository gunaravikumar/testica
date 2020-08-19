using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;


namespace Selenium.Scripts.Pages.MPAC
{
    class Tool : BasePage
    {

        /// <summary>
        /// <This function navigates to the SendStudy side tab>
        /// </summary>        
        /// <returns></returns>
        public void NavigateToSendStudy()
        {
            PageLoadWait.MPacPageLoadWait();
            PageLoadWait.MPWaitForFrameLoad(10);
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("navigation");
            //BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("html>frameset>frameset>frame:nth-child(1)")));
            BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#navTools tr#sendstudies")));
            IWebElement send = Driver.FindElement(By.CssSelector("div#navTools tr#sendstudies"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
            js.ExecuteScript("arguments[0].click()", send);
            //send.Click();
            Driver.SwitchTo().DefaultContent();
            Logger.Instance.InfoLog("******Navigated to send study tab Successfully******");
        }

        public enum StudyInfo { PatientName, PatientID, IPID, DOB, Sex, Accession, StudyDate, StudyDescription, Images };

        /// <summary>
        /// This function searches study
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public void SearchStudy(String option, String value, int period)
        {
            PageLoadWait.MPacPageLoadWait();
            PageLoadWait.MPWaitForFrameLoad(10);
            Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("frame[name='content']")));
            int timeout = 0;
            while (true && timeout < 2)
            {
                timeout++;
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("select[name='qType']")));
                new SelectElement(Driver.FindElement(By.CssSelector("select[name='qType']"))).SelectByText(option);
                Driver.FindElement(By.CssSelector("input[name='textVal']")).Clear();
                Driver.FindElement(By.CssSelector("input[name='textVal']")).SendKeys(value);
                new SelectElement(Driver.FindElement(By.CssSelector("select#period"))).SelectByIndex(period);
                Driver.FindElement(By.CssSelector("input[name='submitbutton']")).Click();
                PageLoadWait.MPacPageLoadWait();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form.frmRoundedCorner")));
                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form.frmRoundedCorner[name='sendForm']")));
                    IList<IWebElement> details = BasePage.Driver.FindElements(By.CssSelector("form.frmRoundedCorner[name='sendForm']>table>tbody>tr.even,tr.odd>td"));
                    foreach (IWebElement detail in details)
                    {
                        if (detail.Text.Contains(value))
                        {
                            break;
                        }
                    }
                    if (timeout > 1)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(60000);
                    }
                }
                catch (WebDriverTimeoutException e)
                {
                    Logger.Instance.InfoLog("No matching studies found" + e);
                    Thread.Sleep(60000);
                }
            }
            BasePage.Driver.SwitchTo().DefaultContent();
            Logger.Instance.InfoLog("******Search study performed Successfully******");
        }


        public static Dictionary<int, string[]> MPacGetSearchResults()
        {
            try
            {

                PageLoadWait.MPacPageLoadWait();
                PageLoadWait.MPWaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("frame[name='content']")));
                Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();

                String[] rowvalues;
                IWebElement table = Driver.FindElement(By.CssSelector("form.frmRoundedCorner[name='sendForm']"));
                IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody>tr.odd,tbody>tr.even"));
                int rowsCount = rows.Count;
                rowvalues = new String[rowsCount];
                int iterate = 0;
                int intColumnIndex = 0;

                foreach (IWebElement row in rows)
                {
                    IList<IWebElement> columns = row.FindElements(By.TagName("td"));
                    String[] columnvalues = new string[columns.Count];
                    intColumnIndex = 0;
                    columnvalues = new String[columns.Count];

                    foreach (IWebElement column in columns)
                    {
                        if (column.Displayed == true)
                        {
                            string columnvalue = column.GetAttribute("innerHTML");
                            columnvalues[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                            intColumnIndex++;
                        }
                    }
                    //Trim Array and put it in dictionary               
                    Array.Resize(ref columnvalues, intColumnIndex);
                    searchresults.Add(iterate, columnvalues);
                    iterate++;

                }
                Logger.Instance.InfoLog("********study recorded successfully*********");
                return searchresults;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Study not Found" + e);
                return null;
            }

        }

        public static string[] MpacGetColumnNames()
        {
            //IWebElement table = Driver.FindElement(By.CssSelector("[name='sendForm'] th"));
            IList<IWebElement> columns = Driver.FindElements(By.CssSelector("[name='sendForm'] th"));
            string[] columnnames = new string[columns.Count];
            int intColumnIndex = 0;

            foreach (IWebElement column in columns)
            {
                if (column.Displayed == true)
                {
                    string columnvalue = column.Text;
                    //columnnames[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                    columnnames[intColumnIndex] = columnvalue;
                    intColumnIndex++;
                }
            }

            //Trim Array and put it in dictionary               
            Array.Resize(ref columnnames, intColumnIndex);
            return columnnames;

        }

        public void MpacSelectStudy1(String columnname, String columnvalue)
        {
            Dictionary<int, string[]> results = MPacGetSearchResults();
            string[] columnnames = MpacGetColumnNames();
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, columnvalue);

            if (rowindex >= 0)
            {
                //Select the appropriate row
                IWebElement table = Driver.FindElement(By.CssSelector("form.frmRoundedCorner[name='sendForm']"));
                IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody>tr.odd>td input,tbody>tr.even>td input"));
                if (rows[rowindex].Displayed == true)
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                    js.ExecuteScript("arguments[0].click()", rows[rowindex]);
                }                
            }
            else
            {
                throw new Exception("Item not found in search results");
            }


        }

        public void MpacSelectStudy(string columnname, string data)
        {

            this.MpacSelectStudy1(columnname, data);
        }

        public static Dictionary<String, String> MPacGetSearchResults(Dictionary<int, string[]> searchresults)
        {
            Dictionary<String, String> results = new Dictionary<string, string>();
            if (searchresults == null)
            {
                return null;
            }
            foreach (int index in searchresults.Keys)
            {
                int counter = 0;
                int ignoreindex = 0;
                foreach (String value in searchresults[index])
                {
                    if (ignoreindex == 0) { ignoreindex++; continue; }
                    Array studyinfo = Enum.GetValues(typeof(StudyInfo));
                    Object key = (Object)studyinfo.GetValue(counter);
                    results.Add(key.ToString(), value);
                    Logger.Instance.InfoLog("Study info from MPAC" + "Column Name is--" + key.ToString() + "--Value is--" + value);
                    counter++;
                }
            }

            return results;
        }

        /// <summary>
        /// <This function sends study>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Boolean SendStudy(int index, String pacsgateway = "", bool closeSentStudies = true, int waitTime = 1)
        {
            PageLoadWait.MPacPageLoadWait();
            PageLoadWait.MPWaitForFrameLoad(10);
            Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("frame[name='content']")));
            BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[value='Send']")));

            //Select the Pacs Gateway
            //new SelectElement(Driver.FindElement(By.CssSelector("div#content td>select[name='target']"))).SelectByIndex(index);
            if (String.IsNullOrEmpty(pacsgateway))
            {
                new SelectElement(Driver.FindElement(By.CssSelector("select[name='target']"))).SelectByValue(Config.pacsgatway1);
            }
            else
            {
                new SelectElement(Driver.FindElement(By.CssSelector("select[name='target']"))).SelectByValue(pacsgateway);
            }

            IWebElement sendbtn = Driver.FindElement(By.CssSelector("input[value='Send']"));
            //get the current window handles 
            var currentWindow = Driver.CurrentWindowHandle;
            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
            js.ExecuteScript("arguments[0].click()", sendbtn);
            PageLoadWait.MPacPageLoadWait();
            //get the new window handles 
            var newwindow = Driver.WindowHandles.Last();
            int count = 0;
            while (newwindow == currentWindow)
            {
                if (Driver.WindowHandles.Count == 1)
                {
                    sendbtn.Click();
                    Thread.Sleep(1000);
                    newwindow = Driver.WindowHandles.Last();
                }
                if (count > 20)
                {
                    throw new Exception("Sending Failed");
                }

                foreach (var window in Driver.WindowHandles)
                {
                    Driver.SwitchTo().Window(window);
                    if (Driver.Title.ToLower().Equals("studies you have sent"))
                    {
                        newwindow = window;
                        break;
                    }
                }

                Thread.Sleep(1000);
                count++;
                //newwindow = Driver.WindowHandles.Last();
            }

            Driver.SwitchTo().Window(newwindow);
            PageLoadWait.MPacPageLoadWait();
            BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#header>span")));

            if (Driver.Title.ToLower().Equals("studies you have sent"))
            {
                Logger.Instance.InfoLog("*****Study sent successfully*****");
                if (closeSentStudies)
                    Driver.Close();
                Driver.SwitchTo().Window(currentWindow);
                PageLoadWait.MPacPageLoadWait();
                PageLoadWait.MPWaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent();
                Thread.Sleep(waitTime * 1000); //Wait for studies to be sent
                return true;
            }
            else
            {
                Driver.SwitchTo().Window(currentWindow);
                PageLoadWait.MPacPageLoadWait();
                PageLoadWait.MPWaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent();
                Logger.Instance.InfoLog("******Error in Send study*******");
                return false;
            }

        }

    }
}
