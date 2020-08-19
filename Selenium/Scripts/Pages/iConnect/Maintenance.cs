using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System.Configuration;

namespace Selenium.Scripts.Pages.iConnect
{
    class Maintenance : BasePage
    {
        public Maintenance() { }

        public const String LogMSG = "DICOM Instances Accessed";

        public static string textarea_auditActiveParticipantDetail = "textarea#m_messageDetail_ActiveParticipantDetail";

        #region WebElements
        public IWebElement Edt_FromeDate() { return BasePage.Driver.FindElement(By.CssSelector("input#masterDateFrom")); }
        public IWebElement Edt_ToDate() { return BasePage.Driver.FindElement(By.CssSelector("input#masterDateTo")); }
        public IWebElement Btn_Search() { return BasePage.Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_mSearchButton")); }
        public IWebElement Tbl_EvemtsTable() { return BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid")); }
        public IWebElement Tbl_ViewerServicesTable() { return BasePage.Driver.FindElement(By.CssSelector("table#m_viewerServicesListControl_ViewerServicesGridView")); }
        public IWebElement Tbl_LicenseTable() { return BasePage.Driver.FindElement(By.CssSelector("table#m_licenseUsageListControl_m_dataListGrid"));  }

        public IWebElement Txt_AccessionNumber() { return Driver.FindElement(By.CssSelector("#m_maintenanceSearchControl_m_searchInputAccessionNumber")); }
        public By By_MessageDetailsDiv() { return By.CssSelector("#MessageDetailDiv"); }
        public IList<IWebElement> AuditListTable() { return Driver.FindElements(By.CssSelector("table#m_listControl_m_dataListGrid tbody tr")); }
        public IList<IWebElement> InnerTab() { return Driver.FindElements(By.CssSelector("div.TabText")); }
        public IWebElement SelectedInnerTab() { return Driver.FindElement(By.CssSelector("div.TabText.TabSelected")); }
        public IWebElement ResultLabel() { return Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultsLabel")); }
        public IList<IWebElement> StatisticsDetails() { return Driver.FindElements(By.CssSelector("span.fixedLengthLabel")); }
        public IList<IWebElement> TableHeadings() {return Driver.FindElements(By.CssSelector("span[title ^= 'Sort']")); }
        public IList<IWebElement> AuditDetalisLabel() { return Driver.FindElements(By.CssSelector("*[id^='m_maintenance']")); }
        public By TableHeader() { return By.CssSelector("th[sortmethod]"); }
        public By TableRow() { return By.CssSelector("tr[style]"); }
        public By TableColumn() { return By.CssSelector("span"); }
        public By TableHeader1() { return By.CssSelector("tr[valign] td"); }
        public IWebElement StatisticsTable() { return Driver.FindElement(By.CssSelector("table[id='m_licenseUsageListControl_m_dataListGrid']>tbody")); }
        public IWebElement AuditDateColumn() { return Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr>td:nth-child(4)>span")); }
        public IWebElement LogDateColumn() { return Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr>td:nth-child(5)>span")); }
        public By TableHeaderInViewerServices() { return By.CssSelector("td[sortmethod]"); }
        public By TableRowInViewerServices() { return By.CssSelector("tr[style]"); }
        public By TableColumnInViewerServices() { return By.CssSelector("td[title]"); }

        //Emergency access logs
        public By By_EmergencyLog() { return By.CssSelector("table#m_listControl_m_dataListGrid tr span[title*='Emergency']"); }
        public IWebElement EmergencyLog() { return BasePage.Driver.FindElement(By_EmergencyLog()); }
        public By By_DICOMAccesslog() { return By.CssSelector("table#m_listControl_m_dataListGrid tr span[title*='" + LogMSG + "']"); }
        public IWebElement DICOMAccesslog() { return BasePage.Driver.FindElement(By_DICOMAccesslog()); }

        //Viewer Service
        public IWebElement refreshBtn() { return BasePage.Driver.FindElement(By.CssSelector("#m_viewerServicesListControl_RefreshViewerServicesListButton")); }
        public IList<IWebElement> ViewerServiceGrid() { return BasePage.Driver.FindElements(By.CssSelector("#ResultList table[id*='ViewerServicesGridView'] tr")); }
        public By Viewerstatus() { return By.CssSelector("#ResultList table[id*='ViewerServicesGridView'] tr:nth-child(2) td:nth-child(2)[title*='Enable']"); }

        public static String span_noOfUsers = "span[id='noOfUsersLabel']";

        #endregion WebELements

        /// <summary>
        /// Gets the sequence Id of Tab based on Tab Name
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>                 
        public int GetTabIndex(String tabname)
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame(Driver.FindElement(By.CssSelector("iframe[id='UserHomeFrame']"))).SwitchTo().Frame(Driver.FindElement(By.CssSelector("iframe[id='TabContent']")));
            //PageLoadWait.WaitForFrameLoad(10);
            IList<IWebElement> tabs = BasePage.Driver.FindElements(By.CssSelector("#maintenanceMainTabBarTabControl td[id^='TabMid'] div"));
            int counter = 0;
            foreach (IWebElement tab in tabs)
            {
                if (tab.GetAttribute("title").Equals(tabname))
                {
                    break;
                }
                counter++;
            }

            return counter;
        }

        /// <summary>
        /// Navigate to Inner Sub Tab
        /// </summary>
        /// <param name="Tabname"></param>
        /// <param name="Page">0-For Other Modules/1-For Internationalization Module</param>
        /// <param name="subtab">1-for selecting subtab</param>
        public void Navigate(String Tabname, int Page = 0, int subtab = 0, string parenttab = null)
        {
            //Get the TabIndex
            int tabindex;
            if (Page == 0)
            {
                tabindex = this.GetTabIndex(Tabname);
            }
            else
            {
                String Tabvalue = GetRespectivePage(Tabname, subtab, parenttab);
                tabindex = this.GetTabIndex(Tabvalue);
            }
            String property = "#TabText" + tabindex;
            String script = "document.querySelector(" + "\"" + property + "\"" + ").click()";

            switch (Tabname)
            {

                case "Audit":
                    PageLoadWait.WaitForPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    //this.Click("Id",GetTabId());
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Audit Tab");
                    break;
                case "Log":
                    PageLoadWait.WaitForPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    //this.Click("Id",GetTabId());
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Log Tab");
                    break;
                case "Statistics":
                    PageLoadWait.WaitForPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    //this.Click("Id",GetTabId());
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Statistics Tab");
                    break;
                case "Viewer Services":
                    PageLoadWait.WaitForPageLoad(20);
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    //this.Click("Id",GetTabId());
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to ViewerServices Tab");
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// This is to set all check boxes in Audit tab
        /// </summary>
        public void SetCheckBoxInAudit()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
            PageLoadWait.WaitForFrameLoad(10);

            String ident1 = "#m_maintenanceSearchControl_OutcomeSeriousFailureSearch";
            if (Driver.FindElement(By.CssSelector(ident1)).Selected != true)
            {
                this.SetCheckbox("CssSelector", ident1);
            }

            String ident2 = "#m_maintenanceSearchControl_OutcomeMajorFailureSearch";
            if (Driver.FindElement(By.CssSelector(ident2)).Selected != true)
            {
                this.SetCheckbox("CssSelector", ident2);
            }

            String ident3 = "#m_maintenanceSearchControl_OutcomeMinorFailureSearch";
            if (Driver.FindElement(By.CssSelector(ident3)).Selected != true)
            {
                this.SetCheckbox("CssSelector", ident3);
            }

            String ident4 = "#m_maintenanceSearchControl_OutcomeSuccessSearch";
            if (Driver.FindElement(By.CssSelector(ident4)).Selected != true)
            {
                this.SetCheckbox("CssSelector", ident4);
            }
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);

        }

        /// <summary>
        /// This is to set checkboxes in Audit tab
        /// </summary>
        /// <param name="FieldName"></param>
        public void SetCheckBoxInAudit(String FieldName, bool bSelected = true)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
            PageLoadWait.WaitForFrameLoad(10);

            FieldName = FieldName.ToLower();
            if (FieldName.Contains("serious"))
            {
                FieldName = "SeriousFailure";
            }
            else if (FieldName.Contains("major"))
            {
                FieldName = "MajorFailure";
            }
            else if (FieldName.Contains("minor"))
            {
                FieldName = "MinorFailure";
            }
            else if (FieldName.Contains("success"))
            {
                FieldName = "Success";
            }
            else
            {
                FieldName = "";
            }
            string ident;
            switch (FieldName)
            {
                case "SeriousFailure":
                    ident = "#m_maintenanceSearchControl_Outcome" + FieldName + "Search";
                    if (bSelected)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", ident);
                    }
                    break;
                case "MajorFailure":
                    ident = "#m_maintenanceSearchControl_Outcome" + FieldName + "Search";
                    if (bSelected)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", ident);
                    }
                    break;
                case "MinorFailure":
                    ident = "#m_maintenanceSearchControl_Outcome" + FieldName + "Search";
                    if (bSelected)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", ident);
                    }
                    break;
                case "Success":
                    ident = "#m_maintenanceSearchControl_Outcome" + FieldName + "Search";
                    if (bSelected)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", ident);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This is to search in Audit Tab
        /// </summary>
        public void SearchInAuditTab(String uid="",String pid="",String pname="",String AccNo="",String EID="")
        {
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#SearchPanelDiv")));
            SendKeys(Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_m_searchInputUserId")), uid);
            SendKeys(Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_m_searchInputPatientId")),pid);
            SendKeys(Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_m_searchInputPatientName")),pname);
            SendKeys(Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_m_searchInputAccessionNumber")),AccNo);
            if (string.IsNullOrWhiteSpace(EID))
            {
                SelectElement select = new SelectElement(Driver.FindElement(By.CssSelector("div#SearchPanelDiv select")));
                select.SelectByValue("0");
            }
            else
            {
                SelectElement select = new SelectElement(Driver.FindElement(By.CssSelector("div#SearchPanelDiv select")));
                select.SelectByText(EID);
            }
           
            if (Config.BrowserType == "Internet Explorer")
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#SearchPanelDiv div input[id='m_maintenanceSearchControl_mSearchButton']\").click()");
            }
            else
            {
                Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_mSearchButton")).Click();

            }
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// This method is to search Audit based on From and To Dates only
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        public void SearchInAudit(String fromdate = null, String todate = null, String timezone = "EST", String eventID = null, int byvalue=0)
        {   
            if(timezone.Equals("EST"))
            {
                var timeUtc = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
                fromdate = easternTime.ToString("dd-MMM-yyyy");
                todate = easternTime.ToString("dd-MMM-yyyy");
            }
            else
            {
                if (String.IsNullOrEmpty(fromdate)) { fromdate = DateTime.Now.ToString("dd-MMM-yyyy"); }
                if (String.IsNullOrEmpty(todate)) { todate = DateTime.Now.ToString("dd-MMM-yyyy"); }
            }            
            this.Edt_FromeDate().Clear();
            this.Edt_FromeDate().SendKeys(fromdate);
            this.Edt_ToDate().Clear();
            this.Edt_ToDate().SendKeys(todate);
            if (SBrowserName.ToLower().Equals("internet explorer"))
                Click("cssselector", "input#m_maintenanceSearchControl_mSearchButton", true);
            else
                this.Btn_Search().Click();
            if (eventID != null)
            {
                SelectEventID(eventID, byvalue);
                Btn_Search().Click();
            }

            //Syncup
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// To slect Event Id from Drop down
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="byvalue">0-bytext/1-byvalue</param>
        public void SelectEventID(String EID, int byvalue = 1)
        {
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            var menuPer = Driver.FindElement(By.CssSelector("div#SearchPanelDiv select"));
            //new Actions(Driver).MoveToElement(menuPer).Click().Build().Perform();
            //Driver.FindElement(By.LinkText(EID)).Click();
            this.SelectFromList(menuPer, EID, byvalue);
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// Returns the Event Count
        /// </summary>
        public Dictionary<String, IList<String>> GetEventsData()
        {
            int iterate = 0;           
            Dictionary<String, IList<String>> eventdata = new Dictionary<String, IList<String>>();
            IList<IWebElement> rows = new List<IWebElement>();
            IList<IWebElement> headers = new List<IWebElement>();            
            rows = BasePage.Driver.FindElements(By.CssSelector("table#m_listControl_m_dataListGrid tr"));            
            int intpagecount = 1;
            try
            {
                intpagecount = BasePage.Driver.FindElements(By.CssSelector("#m_listControl_m_dataListGridPager > span>span")).Count;
                if (intpagecount != 0) { intpagecount = (intpagecount - 4); } else { intpagecount = 1; }
            }
            catch (Exception) { intpagecount = 1; }

            //Get all Event Data column header
            foreach (IWebElement row in rows)
            {
                //Set all the keys 
                if (iterate == 0)
                {
                    headers = row.FindElements(By.CssSelector("th span"));
                    foreach (IWebElement header in headers)
                    {
                        if(!header.GetAttribute("innerHTML").Contains("&nbsp;"))
                        eventdata.Add(header.GetAttribute("innerHTML").Split('<')[0], null);
                    }
                    iterate++;
                    break;
                }
            }

            //For each Columns Take all Event Values
            int iterate1 = 0;            
            foreach(String key in eventdata.Keys.ToList())
            {
                iterate1++;
                String cssselector = "table#m_listControl_m_dataListGrid tr>td:nth-of-type(" + (iterate1) + ")";                
                IList<String> values = new List<String>();

                //Navigate to all pages and fetch the value
                for (int currentpage = 1; currentpage <= intpagecount; currentpage++)
                {
                    IList<IWebElement> columnvalues = BasePage.Driver.FindElements(By.CssSelector(cssselector));

                    foreach (IWebElement columnvalue in columnvalues)
                    {                        
                        String value = columnvalue.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Replace("&nbsp;", " ");
                        values.Add(value);
                    }

                    //Check pagingation and navigate to next page
                    if ((intpagecount == 1) || (currentpage == intpagecount)) { break; }
                    else
                    {
                        String script = "document.querySelector(\'#m_listControl_m_dataListGridPager > span > span:nth-child(" + (intpagecount+3) + ")'"+").click();";
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                        PageLoadWait.WaitForPageLoad(10);
                        PageLoadWait.WaitForFrameLoad(10);
                    }
                }

                //Set the Event Data
                eventdata[key] = values;

                //Navigate back to first page
                if (intpagecount == 1) { continue; }
                else
                {
                    try { ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#m_listControl_m_dataListGridPager > span > span:nth-child(3)\").click();"); }
                    catch (Exception) { Logger.Instance.InfoLog("No additional pages"); }
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                }
            }    
                      
          return eventdata;
        }
        
        /// <summary>
        /// Gets the count of events for the given event type
        /// </summary>
        /// <param name="eventname"></param>
        /// <returns></returns>
        public int GetEventCount(String eventname, int locale = 0, String eventID = null,string columnname="Audit Event",int byvalue=0)
        {
            int eventcount = 0;

            //Navigate to Audit Tab
            if(locale == 0)
            {
                this.Navigate("Audit"); 
            }
            else
            {
                this.Navigate("Audit", 1, 1, "Maintenance");
            }

            this.SearchInAudit(eventID: eventID,byvalue:byvalue);
            var eventdata = this.GetEventsData();

            //Get the total event count
            foreach (String eventtype in eventdata[columnname])
            {
                if (eventtype.ToLower().Equals(eventname.ToLower())) { eventcount++; }
            }

            return eventcount;
        }

        /// <summary>
        /// This method will take the number of events logged from DB for a event type uid
        /// </summary>
        /// <param name="eventtypecodeuid"></param>
        /// <returns></returns>
        public int GetEventCount(int eventtypecodeuid, String username="Administrator")

        {
            int eventcount = 0;
            String sql = "Select Count(*) from AuditMessage where userid=" + "'"+ username + "'" + "and  EventTypeCodeUid='"+eventtypecodeuid+"'";
            var dbutil = new DataBaseUtil("sqlserver");
            dbutil.ConnectSQLServerDB();
            var eventdata = dbutil.ExecuteQuery(sql);
            eventcount = int.Parse(eventdata[0]);
            return eventcount;
        }
    }
}
