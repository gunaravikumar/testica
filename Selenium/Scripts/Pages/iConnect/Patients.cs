using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

using Selenium.Scripts.Reusable.Generic;

namespace Selenium.Scripts.Pages.iConnect
{
    class Patients : BasePage
    {
        #region Constructor

        public Patients()
        {
        }

        #endregion Constructor

        #region Variables

        Patients patients = null;
        public enum PatientColumns { ID = 2, DateTime, Description, Accession, Modalities, DataSource }

        #endregion Variables

        #region WebElements
        //To Enter Searchcontent Patient Search
        public IWebElement PatientSearch() { return BasePage.Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchText")); }

        //To identify Patient Details on Left Panel
        public IWebElement PatientName() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientDemographic_m_patientName']")); }
        public IWebElement PatientDOBInfo() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_patientDOBInfo']")); }
        public IWebElement PatientAddress1() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_address1")); }
        public IWebElement PatientAddress2() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_address2")); }
        public IWebElement PatientCity() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_city")); }
        public IWebElement PatientState() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_state")); }
        public IWebElement PatientCountry() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_country")); }
        public IWebElement PatientGender() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_gender")); }
        public IWebElement PatientMaritalStatus() { return BasePage.Driver.FindElement(By.CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_maritalStatus")); }
        
        
        //To identify the rows in Table after Search
        public IList<IWebElement> TableRows() { return BasePage.Driver.FindElements(By.CssSelector("table[id='gridTablePatientRecords'] tbody tr:not(.jqgfirstrow)")); }
        public IList<IWebElement> TableColumnNames() { return BasePage.Driver.FindElements(By.CssSelector("table[aria-labelledby='gbox_gridTablePatientRecords'] th:not([style*='none']) div")); }

        //To identify the First Record in Table after Search
        public IWebElement FirstRecordAfterSearch() { return BasePage.Driver.FindElement(By.CssSelector("tr[id='1'] td[style='text-align:left;']")); }
        public IList<IWebElement> VisitRows()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            return BasePage.Driver.FindElements(By.CssSelector("table#XdsPageVisitsListControl_parentGrid tr[style]"));
        }

        //To identify Selected Date in XDS
        public IWebElement XDSDate() { return BasePage.Driver.FindElement(By.CssSelector("td#pmjDropDownMenu span")); }
        public IWebElement XDSVisitRecord()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until<IWebElement>(ExpectedConditions.ElementIsVisible(By.CssSelector("table#XdsPageVisitsListControl_parentGrid tr:not([style^='font-family']) tr[title^='Folder:;']")));
            return BasePage.Driver.FindElement(By.CssSelector("table#XdsPageVisitsListControl_parentGrid tr:not([style^='font-family']) tr[title^='Folder:;']"));
        }

        public IList<IWebElement> ReviewToolBar() { return BasePage.Driver.FindElements(By.CssSelector("div[id='reviewToolbar'] img")); }
        public IWebElement TransferBtn() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_PatientMasterJacketControl_m_transferButton")); }
        public IWebElement TransferToBtn() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_PatientMasterJacketControl_DataTransferControl_m_transferDataButton")); }

        public IList<IWebElement> GetRowValues() { return BasePage.Driver.FindElements(By.CssSelector("div[class='ui-jqgrid-bdiv'] tr")); }
        public IList<IWebElement> PatientRecordTabs() { return BasePage.Driver.FindElements(By.CssSelector("td[id*=TabMid] div[id*='TabText']")); }

        public IWebElement NameHeadinginSearchResult() { return Driver.FindElement(By.Id("jqgh_gridTablePatientRecords_name")); }
        public IWebElement DOBHeadinginSearchResult() { return Driver.FindElement(By.Id("jqgh_gridTablePatientRecords_dateOfBirth")); }
        public IWebElement GenderHeadinginSearchResult() { return Driver.FindElement(By.Id("jqgh_gridTablePatientRecords_gender")); }
        public IWebElement AddressHeadinginSearchResult() { return Driver.FindElement(By.Id("jqgh_gridTablePatientRecords_address")); }
        public IWebElement PhoneHeadinginSearchResult() { return Driver.FindElement(By.Id("jqgh_gridTablePatientRecords_homePhone")); }
        public IWebElement ClickSearchClearButton() { return Driver.FindElement(By.Id("FreeTextSearchControl_FreeTextClearButton")); }
        public IWebElement PatientLoadTable() { return Driver.FindElement(By.CssSelector("#LoadingDiv")); }
        public IWebElement PatientSearchTableErrorMsg() { return Driver.FindElement(By.CssSelector("#PatientRecordGridControl1_m_messageLabel")); }
        public IWebElement ExpandPanelDiv() { return BasePage.Driver.FindElement(By.CssSelector("#AdvancedSearchControl_ExpandPanel>div")); }

        public IWebElement table_VisistList() { return BasePage.Driver.FindElement(By.CssSelector("#XdsPageVisitsListControl_parentGrid")); }
        public IWebElement table_FoldersList() { return BasePage.Driver.FindElement(By.CssSelector("#XdsPageFoldersListControl_parentGrid")); }
        public IWebElement table_DocumentList() { return BasePage.Driver.FindElement(By.CssSelector("#XdsPageDocsGrid")); }
        public IWebElement table_StudyList() { return BasePage.Driver.FindElement(By.CssSelector("#RadiologyStudiesListControl_parentGrid")); }
        public IWebElement Btn_TransferTo() { BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame"); return BasePage.Driver.FindElement(By.CssSelector("input[id$='_transferDataButton']")); }

        //Patient XDS Studies Table elements
        public IWebElement XDSStudiesPatientTable() { return BasePage.Driver.FindElement(By.CssSelector("#RadiologyStudiesListControl_parentGrid")); }
        public By XDSStudiesPatientheader() { return By.CssSelector("tr[valign] td[title]"); }
        public By XDSStudiesPatientrow() { return By.CssSelector("tr[title]");}
        public By XDSStudiesPatientcolumn() { return By.CssSelector("td[title]:not(:first-of-type");}

        public IWebElement FreeSearchErrorMsg() { return Driver.FindElement(By.CssSelector("span#FreeTextSearchControl_ErrorMessageLabel")); }
        public String SubmissionDate_VisitsTab() { return Driver.FindElement(By.CssSelector("table#XdsPageVisitsListControl_parentGrid tr:nth-child(2) td:nth-child(6)>span")).GetAttribute("title"); }
        public String CreatedDate_DocsTab() { return Driver.FindElement(By.CssSelector("table#XdsPageDocsGrid tr:nth-child(2) td:nth-child(4)>span")).GetAttribute("title"); }
        public String Date_time_StudiesTab() { return Driver.FindElement(By.CssSelector("table#RadiologyStudiesListControl_parentGrid tr:nth-child(2) td:nth-child(4)")).GetAttribute("title"); }

        #endregion WebElements

        #region ReusableComponents

        public void InputData(string name)
        {
            Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchText")).Clear();
            PageLoadWait.WaitForPageLoad(4);
            Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchText")).SendKeys(name);
            PageLoadWait.WaitForPageLoad(4);
            Logger.Instance.InfoLog(name + " entered in the Patient Name field");
        }

        /// <summary>
        /// This method will expand the study search grid in pateints Tab
        /// </summary>
        public void ExpandPanel()
        {
            this.ExpandPanelDiv().Click();
            BasePage.wait.Until<Boolean>((d) =>
            {
                if (!d.FindElement(By.CssSelector("div#SearchPanelDiv")).GetAttribute("style").Contains("display: none;"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            });
        }

        /// <summary>
        /// This function is used to click Patient search button. Explicitly called when not using Patient live search
        /// </summary>
        public void ClickPatientSearch(int adv = 0)
        {
            PageLoadWait.WaitForFrameLoad(20);
            if (adv == 0)
            {
                Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchButton")).Click();
            }
            else
            {
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_m_searchButton")).Click();
            }
            PageLoadWait.WaitForFrameLoad(20);
        }

        public Boolean PatientExists(string name)
        {
            try
            {
                ReadOnlyCollection<IWebElement> elements =
                    Driver.FindElements(
                        By.XPath("//table[@id='gridTablePatientRecords']/tbody/tr"));

                for (int i = 1; i <= elements.Count; i++)
                {
                    //*[@id="1"]/td[1]
                    if (elements[i - 1].FindElement(By.XPath("//*[@id=" + i + "]/td[1]")).Text.ToLower().Contains(name.ToLower()))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + name +
                                         " for exception : " + ex.Message);
            }

            return false;

        }

        public Boolean PatientExistsInLiveSearch(string expecteddata)
        {
            try
            {
                IWebElement ResultList = Driver.FindElement(By.Id("ResultList"));
                List<IWebElement> trList = ResultList.FindElements(By.TagName("tr")).ToList();
                foreach (var item in trList)
                {
                    List<IWebElement> tdList = ResultList.FindElements(By.TagName("td")).ToList();
                    for (int i = 0; i < tdList.Capacity; i++)
                    {
                        if (tdList[i].Text.Equals(expecteddata))
                        {
                            return true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + expecteddata +
                                         " for exception : " + ex.Message);
            }

            return false;
        }

        public Boolean PqaExists(string name)
        {
            try
            {
                IWebElement ResultList = Driver.FindElement(By.Id("gridTablePatientRecords"));
                List<IWebElement> trList = ResultList.FindElements(By.TagName("tr")).ToList();
                foreach (var item in trList)
                {
                    //List<IWebElement> tdList = ResultList.FindElements(By.TagName("td")).ToList();
                    for (int i = 0; i < trList.Capacity; i++)
                    {
                        if (trList[i].Text.Contains(name))
                        {
                            return true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + name +
                                         " for exception : " + ex.Message);
            }

            return false;
        }

        public Boolean DocumentExists(string name)
        {
            try
            {
                IWebElement ResultList = Driver.FindElement(By.Id("m_patientHistory_m_attachmentViewer_attachmentList"));
                List<IWebElement> trList = ResultList.FindElements(By.TagName("tr")).ToList();
                foreach (var item in trList)
                {
                    //  List<IWebElement> tdList = ResultList.FindElements(By.TagName("td")).ToList();
                    for (int i = 0; i < trList.Capacity; i++)
                    {
                        if (trList[i].Text.Contains(name))
                        {
                            return true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + name +
                                         " for exception : " + ex.Message);
            }

            return false;

        }

        new public void Doubleclick(string id, string value)
        {
            var element = this.GetElement(id, value);
            // var element = this.GetElement("xpath", "//*[@id='XdsPageDocsGrid']/tbody/tr[7]/td[1]/span");
            //*[@id="XdsPageDocsGrid"]/tbody/tr[7]/td[1]/span
            if (element != null)
            {
                var action = new Actions(Driver);

                action.DoubleClick(element).Build().Perform();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForElement(By.Id("reviewToolbar"), BasePage.WaitTypes.Visible);
                //  PageLoadWait.WaitForPageLoad(20);
            }
        }

        /// <summary>
        /// This function Loads the study in Patient record page 
        /// </summary>
        /// <param name="name"></param>
        public void LoadStudyInPatientRecord(string name)
        {
            int columnno = 0;
            IWebElement newelement = Driver.FindElement(By.Id("gridTablePatientRecords"));
            List<IWebElement> tr = newelement.FindElements(By.TagName("tr")).ToList();
            List<IWebElement> data = tr[1].FindElements(By.TagName("td")).ToList();
            for (columnno = 0; columnno < data.Capacity; columnno++)
            {
                if (data[columnno].GetAttribute("aria-describedby").Equals("gridTablePatientRecords_name"))
                {
                    break;
                }
            }
            for (int i = 1; i < tr.Capacity; i++)
            {
                data = tr[i].FindElements(By.TagName("td")).ToList();


                if (data[columnno].Text.ToLower().Replace(" ", "").Equals(name.ToLower().Replace(" ", "")))
                {
                    var element = tr[i];
                    var action = new Actions(Driver);
                    action.DoubleClick(element).Build().Perform();
                    break;
                }
            }
            this.SwitchToDefault();
            this.SwitchTo("index", "0");
            try
            {
                var js = Driver as IJavaScriptExecutor;
                if (js != null)
                {
                    js.ExecuteScript("pmjStudySearchMenuControl.dropDownMenuItemClick(\'0\')");
                    js.ExecuteScript("pmjStudySearchMenuControl.dropDownMenuItemClick(\'0\')");
                }
            }
            catch (Exception)
            {
                Logger.Instance.ErrorLog("Exception in selecting All Dates for Patient Records");
            }

        }

        public void ClosePatientRecord()
        {
            this.SwitchToDefault();
            this.SwitchTo("index", "0");
            this.Click("id", "ctl00_MasterContentPlaceHolder_Image2");
            Logger.Instance.InfoLog(" Patient details closed successfully");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(15);
        }

        /// <summary>
        ///     This function clicks the close image to close the study
        /// </summary>
        new public void CloseStudy()
        {
            base.CloseStudy();
        }

        public void ClickClear()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_ClearButton")));
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('#AdvancedSearchControl_ClearButton').click()");

        }

        public void ApplyAnnotations()
        {
            this.SwitchToDefault();
            this.SwitchTo("index", "0");

            IWebElement element = GetElement("id", "ctl00_MasterContentPlaceHolder_ImageDisplay_CompViewer_SeriesViewer_1_1_viewerImg");
            //  m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg
            if (element != null)
            {
                ToolBarSetWindowLevelTool();
                var action = new Actions(Driver);
                action.MoveToElement(element, 20, 20).Click().Build().Perform();
                PageLoadWait.WaitForPageLoad(3);
                action.MoveToElement(element, 40, 40).Click().Build().Perform();
            }
            else
            {
                Logger.Instance.ErrorLog("Element not found to Window level");
            }

        }

        public void AttributeSearch(string Field, string data)
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            Field = Field.ToLowerInvariant();

            if (Field.Contains("date"))
            {
                Field = "date";
            }
            else if (Field.Contains("lastname"))
            {
                Field = "lastname";
            }
            else if (Field.Contains("firstname"))
            {
                Field = "firstname";
            }
            else if (Field.Contains("line"))
            {
                Field = "line";
            }


            switch (Field)
            {

                case "lastname":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_LastName")));
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_LastName")).Clear();
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_LastName")).SendKeys(data);
                    break;

                case "date":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_DOB")));
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_DOB")).Clear();
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_DOB")).SendKeys(data);
                    break;

                case "firstname":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_FirstName")));
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_FirstName")).Clear();
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_FirstName")).SendKeys(data);
                    break;

                case "line":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_Line1")));
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_Line1")).Clear();
                    Driver.FindElement(By.CssSelector("#AdvancedSearchControl_Line1")).SendKeys(data);
                    break;

                default:
                    break;
            }

            //Press Search Button            
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_m_searchButton")));
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('#AdvancedSearchControl_m_searchButton').click()");

            //Synch for Search Results to Load
            //PageLoadWait.WaitForLoadingMessage();
            //PageLoadWait.WaitForPageLoad(20);
            //PageLoadWait.WaitForSearchLoad();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_FirstName")));

            Logger.Instance.InfoLog("Search Performed with Data--" + data);
        }

        /// <summary>
        /// This method will wait for Loading message to Appear
        /// </summary>
        public void WaitForLoadingDiv_Appear()
        {
            try
            {
                var wait = new DefaultWait<IWebDriver>(BasePage.Driver);
                wait.Timeout = new TimeSpan(0, 0, 5);
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.Until<Boolean>((driver) =>
                {
                    if (!driver.FindElement(By.CssSelector("div#LoadingDiv")).GetAttribute("style").Contains("display: none"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception) { }
        }

        /// <summary>
        /// This method is to wait Loading div to disappear
        /// </summary>
        /// <param name="timeout"></param>
        public void WaitForLoadingDiv_Disappear(int timeout)
        {
            var wait = new DefaultWait<IWebDriver>(BasePage.Driver);
            wait.Timeout = new TimeSpan(0, 0, timeout);
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.Until<Boolean>((driver) =>
            {
                if (driver.FindElement(By.CssSelector("div#LoadingDiv")).GetAttribute("style").Contains("display: none"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

        }

        /// <summary>
        /// This method is to do adavnced search on patients using combination of parameter.
        /// The number of search parameter can be increased based on the need.
        /// </summary>
        /// <param name="lastname"></param>
        /// <param name="firtsname"></param>
        /// <param name="middlename"></param>
        public void AdvancedSearch(string lastname = "", string firtsname = "", String middlename = "", String dob = "", String streetaddress = "")
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);

            if (!String.IsNullOrEmpty(lastname))
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_LastName")));
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_LastName")).Clear();
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_LastName")).SendKeys(lastname);
            }

            if (!String.IsNullOrEmpty(dob))
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_DOB")));
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_DOB")).Clear();
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_DOB")).SendKeys(dob);
            }

            if (!String.IsNullOrEmpty(firtsname))
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_FirstName")));
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_FirstName")).Clear();
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_FirstName")).SendKeys(firtsname);
            }

            if (!String.IsNullOrEmpty(streetaddress))
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#AdvancedSearchControl_Line1")));
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_Line1")).Clear();
                Driver.FindElement(By.CssSelector("#AdvancedSearchControl_Line1")).SendKeys(streetaddress);
            }

            //Perfrom Search
            Driver.FindElement(By.CssSelector("#AdvancedSearchControl_FirstName")).SendKeys(Keys.Enter);
            this.WaitForLoadingDiv_Appear();
            this.WaitForLoadingDiv_Disappear(30);
            Logger.Instance.InfoLog("Search Performed with Data");
        }

        public Boolean PatientExistsEHR(string id)
        {
            try
            {
                ReadOnlyCollection<IWebElement> elements = Driver.FindElements(By.CssSelector("#ctl00_ctl05_m_dataListGrid>tbody>tr"));
                for (int i = 1; i <= elements.Count; i++)
                {
                    List<IWebElement> span = elements[i - 1].FindElements(By.TagName("span")).ToList();
                    foreach (var item in span)
                    {
                        if (item.Text.Contains(id))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in PatientExistsEHR method due to: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Launch study from studies tab in Patient Record page
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public StudyViewer LaunchStudy(PatientColumns column, string value, int toolscount = 20)
        {   
            //Synch up
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);          
            PageLoadWait.WaitForElement(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr"), WaitTypes.Visible, 30);

            //Launch Study
            List<IWebElement> tr = Driver.FindElements(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr")).ToList();          
            for (int i = 1; i < tr.Capacity; i++)
            {
                if (tr[i].Displayed)
                {
                    List<IWebElement> data = tr[i].FindElements(By.TagName("td")).ToList();
                    if (data[(int)column].Text.ToLower().Equals(value.ToLower()))
                    {
                        var element = tr[i];
                        var action = new Actions(Driver);
                        action.DoubleClick(element).Build().Perform();
                        break;
                    }
                }
            }

            //Synch up
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            WebDriverWait elementsload = new WebDriverWait(Driver, TimeSpan.FromSeconds(60));
            elementsload.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            elementsload.PollingInterval = TimeSpan.FromSeconds(4);
            elementsload.Until<Boolean>((d) =>
            {
                PageLoadWait.WaitForFrameLoad(10);
                if ((Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"))).Enabled && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"))).Displayed)// && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed)
                {
                    Logger.Instance.InfoLog("Study viewer images are loaded");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Study viewer images are getting loaded");
                    return false;
                }

                //catch (Exception e) { Logger.Instance.InfoLog("Exception while weaiting for Image-" + e.Message); return false; }


            });
            //Wait for all Top elements to load
            elementsload.Until<Boolean>((d) =>
            {
                try
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    IList<IWebElement> elements = Driver.FindElements(By.CssSelector("div#reviewToolbar>ul>li"));
                    int elementfound = 0;
                    foreach (IWebElement element in elements)
                    {
                        if ((element.Enabled == true) && (element.Displayed == true))
                        {
                            elementfound++;
                        }
                    }

                    if (elementfound >= toolscount) { Logger.Instance.InfoLog("Top Elements in Study viewer loaded"); return true; } else { Logger.Instance.InfoLog("Waiting for Top elements in study viewer to be loaded"); return false; }
                }
                catch (Exception e)
                { Logger.Instance.InfoLog("Exception caught while waiting for study viewer " + e.Message); return false; }

            });
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForThumbnailsToLoad(60);
            //PageLoadWait.WaitForAllViewportsToLoad(60);
            PageLoadWait.WaitForFrameLoad(10);
            return new StudyViewer();
        }

        public void InputDataWithoutClear(string name)
        {
            Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchText")).SendKeys(name);
            PageLoadWait.WaitForPageLoad(4);
            Logger.Instance.InfoLog(name + " entered in the Patient Name field");
        }

        public Boolean NameExistsInLiveSearch(string expecteddata)
        {
            bool temp = false;
            IWebElement ResultList = Driver.FindElement(By.Id("gridTablePatientRecords"));
            List<IWebElement> trList = ResultList.FindElements(By.TagName("tr")).ToList();
            for (int i = 1; i < trList.Capacity; i++)
            {
                List<IWebElement> tdList = trList[i].FindElements(By.TagName("td")).ToList();
                if (tdList[0].Text.Equals(expecteddata))
                {
                    temp = true;
                }
            }
            return temp;
        }

        public Boolean AddressExistsInLiveSearch(string expecteddata)
        {
            bool temp = false;
            IWebElement ResultList = Driver.FindElement(By.Id("gridTablePatientRecords"));
            List<IWebElement> trList = ResultList.FindElements(By.TagName("tr")).ToList();

            for (int i = 1; i < trList.Capacity; i++)
            {
                List<IWebElement> tdList = trList[i].FindElements(By.TagName("td")).ToList();
                if (tdList[3].Text.Contains(expecteddata))
                {
                    temp = true;
                }
            }
            return temp;
        }

        public Boolean PhoneNumberExistsInLiveSearch(string expecteddata)
        {
            bool temp = false;
            IWebElement ResultList = Driver.FindElement(By.Id("gridTablePatientRecords"));
            List<IWebElement> trList = ResultList.FindElements(By.TagName("tr")).ToList();
            for (int i = 1; i < trList.Capacity; i++)
            {
                List<IWebElement> tdList = trList[i].FindElements(By.TagName("td")).ToList();
                if (tdList[4].Text.Contains(expecteddata))
                {
                    temp = true;
                }
            }
            return temp;
        }

        /// <summary>
        /// This function Loads the file from XDS Visit tab based on File Format 
        /// </summary>
        public bool LoadImageXdsVisitsPatients(string FileFormat)
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool executionloop = false;
            IWebElement table = Driver.FindElement(By.CssSelector("table#XdsPageVisitsListControl_parentGrid"));
            do
            {
                int rowcount = BasePage.Driver.FindElements(By.CssSelector("table#XdsPageVisitsListControl_parentGrid tr[style]")).Count;
                for (int i = 0; i < rowcount; i++)
                {
                    table.FindElement(By.XPath("//tr[@style][" + (i + 1) + "]/td/span/img")).Click();
                    IWebElement file = table.FindElement(By.XPath("(//tr[not(starts-with(@style,'font-family'))][not(@valign)]/td/table/tbody/tr[starts-with(@title,'Folder:;')])[" + (i + 1) + "]"));
                    if (file.GetAttribute("title").ToLowerInvariant().Contains(FileFormat.ToLowerInvariant()))
                    {
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", file.FindElement(By.CssSelector("span[title^='Folder:;']")));
                        PageLoadWait.WaitForPageLoad(20);
                        PageLoadWait.WaitForFrameLoad(20);
                        PageLoadWait.WaitForAllViewportsToLoad(20);
                        return true;
                    }
                }
                IList<IWebElement> PageVisitsList = Driver.FindElements(By.CssSelector("span[style*='underline;'] "));
                if (PageVisitsList.Count > 0 && PageVisitsList[PageVisitsList.Count - 1].Text.Equals("Next"))
                {
                    PageVisitsList[PageVisitsList.Count - 1].Click();
                    executionloop = true;
                }
                else
                {
                    executionloop = false;
                }
            }
            while (executionloop);
            return false;
        }

        /// <summary>
        /// This function compare records between two Dictionary to verify it is same or different
        /// </summary>
        public bool CompareDictionary(Dictionary<int, string[]> table1, Dictionary<int, string[]> table2)
        {
            if (table1.Count != table2.Count)
            {
                return false;
            }
            for (int i = 0; i < table1.Count; i++)
            {
                string[] table1records = table1[i];
                string[] table2records = table2[i];
                if (!table1records.SequenceEqual(table2records))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsToolDisabled(string ToolName)
        {
            IList<IWebElement> columnnames = ReviewToolBar();
            foreach (IWebElement column in columnnames)
            {
                if (column.GetAttribute("title").ToLowerInvariant().Equals(ToolName.ToLowerInvariant()))
                {
                    string status = column.GetAttribute("class").ToLowerInvariant().Trim();
                    if (status.Contains("disableoncine"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool LoadImageXdsDocumentPatients(string FileFormat)
        {
            bool test = false;
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            IWebElement table = Driver.FindElement(By.CssSelector("table#XdsPageDocsGrid"));
            List<IWebElement> allRows = table.FindElements(By.TagName("tr")).ToList();
            for (int i = 0; i < allRows.Count; i++)
            {
                IList<IWebElement> allColumns = allRows[i + 1].FindElements(By.CssSelector("td span"));
                if (allColumns[2].GetAttribute("title").ToLowerInvariant().Contains(FileFormat.ToLowerInvariant()))
                {
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", allColumns[3]);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                    test = true;
                    // return true;
                }
                else
                {
                    test = true;
                    // return false;
                }
            }
            return test;
        }

        /// <summary>
        /// This method will navigate to sub tabs
        /// </summary>
        /// <param name="tabname"></param>
        public void NavigateToTabs(String tabname, int locale = 0)
        {
            //Switch frame
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");


            if (locale != 0)
            {
                tabname = GetRespectivePage(tabname);
            }

            var tabs = BasePage.Driver.FindElements
            (By.CssSelector("[id$='_tabBarPmjTabControl'] > tbody > tr > td:nth-child(1) > table div")).
            Select<IWebElement, IWebElement>((element) =>
              {
                  if (element.GetAttribute("innerHTML").ToLower().Equals(tabname.ToLower()))
                  {
                      return element;
                  }
                  else
                  {
                      return null;
                  }
              }).ToList();
            tabs.RemoveAll(element => element == null);
            this.ClickElement(tabs[0]);

            //Synch Up
            BasePage.wait.Until<Boolean>(d => 
            {
             var tabs_after =   d.FindElements
            (By.CssSelector("[id$='_tabBarPmjTabControl'] > tbody > tr > td:nth-child(1) > table div")).
            Select<IWebElement, IWebElement>((element) =>
            {
                if (element.GetAttribute("innerHTML").ToLower().Equals(tabname.ToLower()))
                {
                    return element;
                }
                else
                {
                    return null;
                }
            }).ToList();
            tabs_after.RemoveAll(element => element == null);
            if (tabs_after[0].GetAttribute("class").Contains("Selected"))
              return true;
            else
              return false;
            });
            PageLoadWait.WaitForFrameLoad(5);

            //Synch up for studies table to load
            if(tabname.ToLower().Replace(" ", "").Equals("studies"))
                this.WaitForSearchProgress(this.table_StudyList());
        }

        /// <summary>
        /// This method will navigate to sub tabs
        /// </summary>
        /// <param name="tabname"></param>
        public void NavigateToSubTabs(String tabname, int locale = 0)
        {
            PageLoadWait.WaitForFrameLoad(5);
            if (locale != 0)
            {
                tabname = GetRespectivePage(tabname);
            }
            var tabs = BasePage.Driver.FindElements
            (By.CssSelector("#tabBarXdsTabControl > tbody > tr:nth-child(1) > td:nth-child(1) td div"));
            var tabelements = tabs.Select<IWebElement, IWebElement>((element) =>
            {
                if (element.GetAttribute("innerHTML").ToLower().Equals(tabname.ToLower()))
                {
                    return element;
                }
                else
                {
                    return null;
                }
            }).ToList();
            tabelements.RemoveAll(element => element == null);
            this.ClickElement(tabelements[0]);

            //Synch Up
            BasePage.wait.Until<Boolean>(d =>
            {
                var tabs_after = d.FindElements
               (By.CssSelector("#tabBarXdsTabControl > tbody > tr:nth-child(1) > td:nth-child(1) td div")).
               Select<IWebElement, IWebElement>((element) =>
               {
                   if (element.GetAttribute("innerHTML").ToLower().Equals(tabname.ToLower()))
                   {
                       return element;
                   }
                   else
                   {
                       return null;
                   }
               }).ToList();
                tabs_after.RemoveAll(element => element == null);
                if (tabs_after[0].GetAttribute("class").Contains("Selected"))
                    return true;
                else
                    return false;
            });
            BasePage.Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

            //Synchup-Tables to load
            if(tabname.ToLower().Equals("visits"))
            this.WaitForSearchProgress(this.table_VisistList());
            else if(tabname.ToLower().Equals("documents"))
            this.WaitForSearchProgress(this.table_DocumentList());
            else
            this.WaitForSearchProgress(this.table_FoldersList());
        }               

        /// <summary>
        /// This method will wait till the search in progress disappears
        /// </summary>
        public void WaitForSearchProgress(IWebElement table)
        {
            
            var wait = new DefaultWait<IWebElement>(table);
            wait.Timeout = new TimeSpan(0, 0, 30);
            wait.IgnoreExceptionTypes(new Type[] {new StaleElementReferenceException().GetType()});
            wait.Until<Boolean>(table1=>
            {
                var column = table1.FindElement(By.CssSelector("tr:nth-of-type(2) td"));
                if (column.GetAttribute("innerHTML").ToLower().Replace(" ", "").Contains("searchinprogress"))
                    return false;
                else
                    return true;
            });
        }

        public StudyViewer LaunchStudy(String[] column, string value, int toolscount = 20)
        {
            //Synch up
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForElement(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr"), WaitTypes.Visible, 30);

            //Launch Study
            List<IWebElement> tr = Driver.FindElements(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr")).ToList();
            for (int i = 1; i <= tr.Capacity; i++)
            {
                List<IWebElement> data = tr[i].FindElements(By.TagName("td")).ToList();
                if (tr[i].Displayed)
                {
                    foreach (IWebElement d in data)
                    {
                        if (d.Text.ToLower().Equals(value.ToLower()))
                        {
                            var element = tr[i];
                            var action = new Actions(Driver);
                            action.DoubleClick(element).Build().Perform();
                            break;
                        }
                    }
                    break;
                }

            }

            //Synch up
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            WebDriverWait elementsload = new WebDriverWait(Driver, TimeSpan.FromSeconds(60));
            elementsload.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            elementsload.PollingInterval = TimeSpan.FromSeconds(4);
            elementsload.Until<Boolean>((d) =>
            {
                PageLoadWait.WaitForFrameLoad(10);
                if ((Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"))).Enabled && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"))).Displayed)// && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed)
                {
                    Logger.Instance.InfoLog("Study viewer images are loaded");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Study viewer images are getting loaded");
                    return false;
                }

                //catch (Exception e) { Logger.Instance.InfoLog("Exception while weaiting for Image-" + e.Message); return false; }


            });
            //Wait for all Top elements to load
            elementsload.Until<Boolean>((d) =>
            {
                try
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    IList<IWebElement> elements = Driver.FindElements(By.CssSelector("div#reviewToolbar>ul>li"));
                    int elementfound = 0;
                    foreach (IWebElement element in elements)
                    {
                        if ((element.Enabled == true) && (element.Displayed == true))
                        {
                            elementfound++;
                        }
                    }

                    if (elementfound >= toolscount) { Logger.Instance.InfoLog("Top Elements in Study viewer loaded"); return true; } else { Logger.Instance.InfoLog("Waiting for Top elements in study viewer to be loaded"); return false; }
                }
                catch (Exception e)
                { Logger.Instance.InfoLog("Exception caught while waiting for study viewer " + e.Message); return false; }

            });
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForThumbnailsToLoad(60);
            //PageLoadWait.WaitForAllViewportsToLoad(60);
            PageLoadWait.WaitForFrameLoad(10);
            return new StudyViewer();
        }

       


        #endregion ReusableComponents
    }
}
