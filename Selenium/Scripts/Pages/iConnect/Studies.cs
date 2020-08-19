using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using System.Collections.ObjectModel;
using Selenium.Scripts.Reusable.Generic;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;
using Selenium.Scripts.Pages.iConnect;

namespace Selenium.Scripts.Pages
{
    class Studies : BasePage
    {
        /// <Default Constructor>
        /// 
        /// </summary>
        public Studies() { }

        
        public SelectElement Dropdown_TransferTo() { return new SelectElement(Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_m_destinationSources"))); }
        public IWebElement StudyPerformedDropDown() { return Driver.FindElement(By.CssSelector("#searchStudyMainText")); }
        public IWebElement RadioBtn_MyPatientOnly() { return Driver.FindElement(By.CssSelector("#m_studySearchControl_UseRefPhysMe")); }
        public static String SearchGridBody = "#gview_gridTableStudyList > div.ui-jqgrid-bdiv";
        public static String DivSearchPanel = "#SearchPanelDiv";
        public static String DivConnectionTest = "#ConnectionTestDiv";
        public static String SearchPageViewText = "#gridPagerDivStudyList_right>div";
		public static String searchPriroStudiesText = "//td[contains(text(),'Searching for related studies.')]";

		//Buttons        
		public IWebElement Btn_StudyPageTransfer() { return Driver.FindElement(By.CssSelector("#m_transferButton")); }
        public IWebElement Btn_StudyPageTransferBtn() { return Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_TransferButton")); }
        public IWebElement Btn_StudyTransfer() { return Driver.FindElement(By.CssSelector("#m_transferDrawer_StudyTransferControl_TransferButton")); }
        public IWebElement Btn_TransferStatus() { return Driver.FindElement(By.CssSelector("#m_transferDrawer_StudyTransferControl_transferStatusButton")); }
        public IWebElement Btn_CreateNewDestination() { return Driver.FindElement(By.CssSelector("#m_transferDrawer_StudyTransferControl_NewDestinationButton")); }
        public IWebElement ClearSearchBtn() { return Driver.FindElement(By.CssSelector("#m_studySearchControl_m_clearButton")); }
        public IWebElement TransferButton() { return Driver.FindElement(By.CssSelector("div#ButtonsDiv table td>div>input#m_transferButton")); }
        public IWebElement QCSubmitButton() { return Driver.FindElement(By.CssSelector("div#dataQCDiv input#ctl00_DataQCControl_m_submitButton")); }
        public IWebElement DownloadButton() { return Driver.FindElement(By_DownloadButton()); }
        public IWebElement DownloadPackagesButton() { return Driver.FindElement(By_DownloadPackagesButton()); }
        public IWebElement SelectAllButton() { return Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")); }
        public IWebElement CloseDownloadButton() { return Driver.FindElement(By.CssSelector("#ctl00_TransferJobPackagesListControl_m_closeDialogButton")); }
        public IWebElement ConfirmAllButton() { return Driver.FindElement(By.CssSelector("#ctl00_DataQCControl_m_confirmAllButton")); }

        //By Objects
        public By By_StudyTransferDialogDiv() { return By.CssSelector("div#DialogContentDiv"); }
        public By By_TransferStatusDiv() { return By.CssSelector("#DialogDiv"); }
        public By By_TransferTo() { return By.CssSelector("#m_transferDrawer_StudyTransferControl_m_destinationSources"); }
        public By StudySearch() { return By.CssSelector("input#m_studySearchControl_m_searchButton"); }
        public By By_Status(String Status) { return By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='" + Status + "']"); }
        public By By_DownloadButton() { return By.CssSelector("#ctl00_TransferJobsListControl_m_submitButton"); }
        public By By_DownloadPackagesButton() { return By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton"); }
        public By By_RelatedStudy(int rowNumber) { return By.CssSelector("#ctl00_StudyTransferControl_relatedDataGrid>tbody>tr:nth-child(" + rowNumber + ")>td:nth-child(5)"); }
        public IWebElement ViewButton() { return Driver.FindElement(By.CssSelector("#m_viewStudyButton")); }
        public By By_CloseTransferBtn() { return By.CssSelector("#StudySharingDialogTitlebar > span"); }
		public By RelatedStudiesUsingAcc(string Accession) { return By.CssSelector("table#ctl00_StudyTransferControl_relatedDataGrid td:nth-child(5) span[title='" + Accession + "']"); }
        public By By_PatientListTable() { return By.CssSelector("#gridTableStudyList tr:nth-child(2) td"); }
        
        /// <summary>
        /// This function retuen the MainDataSourceList_Name
        /// </summary>
        /// <returns></returns>
        public ISet<String> GetMainDataSourceList_Name()
        {
            IList<IWebElement> Main_DS_span = Driver.FindElements(By.CssSelector("div[id='sub_menu_multiselect']>div>a>span"));
            IList<IWebElement> Main_DS_div = Driver.FindElements(By.CssSelector("div[id='sub_menu_multiselect']>div>a>div"));
            ISet<String> Main_DS_List_Name = new HashSet<String>();

            foreach (IWebElement ele in Main_DS_span)
                Main_DS_List_Name.Add(ele.GetAttribute("innerText"));
            foreach (IWebElement ele in Main_DS_div)
                Main_DS_List_Name.Add(ele.GetAttribute("innerText"));

            return Main_DS_List_Name;
        }


        /// <summary>
        /// This function retuen the ChildDataSourceList_Name
        /// </summary>
        /// <returns></returns>
        public ISet<String> GetChildDataSourceList_Name()
        {
            IList<IWebElement> Child_DS_span = Driver.FindElements(By.CssSelector("div[id='child_menu'] div>a>span"));
            IList<IWebElement> Child_DS_div = Driver.FindElements(By.CssSelector("div[id='child_menu'] div>a>div"));

            ISet<String> Main_DS_List_Name = new HashSet<String>();
            foreach (IWebElement ele in Child_DS_span)
                Main_DS_List_Name.Add(ele.GetAttribute("innerText"));
            foreach (IWebElement ele in Child_DS_div)
                Main_DS_List_Name.Add(ele.GetAttribute("innerText"));

            return Main_DS_List_Name;
        }


        /// <summary>
        /// This function will check the given datasource is selected or not
        /// </summary>
        /// <param name="datasourcename"></param>
        public Boolean ISDataSourceSelected(String datasourcename)
        {
            //RDM Mouse Hover
            if (datasourcename.ToLower().Contains("rdm") || datasourcename.Contains("."))
            {
                RDM_MouseHover(datasourcename.Split('.')[0]);
            }
            var items = Driver.FindElements(By.CssSelector("div#dataSource_right div>div>a"));

            //Select All Datasource 
            PageLoadWait.WaitForPageLoad(10);

            foreach (IWebElement item in items)
            {
                IWebElement DS = item.FindElement(By.CssSelector("span,div"));
                if (DS.GetAttribute("innerHTML").Equals(datasourcename) && item.FindElement(OpenQA.Selenium.By.CssSelector("img")).GetAttribute("src").Contains("Selected"))
                    return true;
            }
            return false;
        }



        /// <This is to search study>
        /// 
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="data"></param>
        new public void SearchStudy(string Field, string data)
        {
            base.SearchStudy(Field, data);
        }

        /// <To Select a study based on a specific column value>
        /// 
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="value"></param>
        new public void SelectStudy(string columnname, string value)
        {
            base.SelectStudy(columnname, value);
        }

        /// <This is to launch the study>
        /// 
        /// </summary>
        public StudyViewer LaunchStudy()
        {
            return base.LaunchStudy();
        }

        /// <This is to close  the study viewer>
        /// 
        /// </summary>
        new public void CloseStudy()
        {
            base.CloseStudy();
        }

        new public void SelectAllDateAndData()
        {
            base.SelectAllDateAndData();
        }

        new public void ClickSearchBtn()
        {
            base.ClickSearchBtn();
        }


        /// <This method is to get the Matching Row object in Search results>
        /// 
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        /// <returns></returns>
        new public Dictionary<string, string> GetMatchingRow(String columnname, String columnvalue)
        {
            return base.GetMatchingRow(columnname, columnvalue);
        }

        /// <This si to select study based on matching record>
        /// 
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        new public void SelectStudy1(String columnname, String columnvalue)
        {
            base.SelectStudy1(columnname, columnvalue);
        }

        /// <Gets the matching record object based on mutiple matching column values>
        /// 
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        /// <returns></returns>
        new public Dictionary<string, string> GetMatchingRow(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
            return base.GetMatchingRow(matchcolumnnames, matchcolumnvalues);
        }

        /// <Selecting study based on mtuiple matching column values>
        /// 
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        new public void SelectStudy1(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
            base.SelectStudy1(matchcolumnnames, matchcolumnvalues);
        }

        /// <Gets the matching record object based on mutiple matching column values>
        /// 
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        /// <returns></returns>
        new public Dictionary<string, string> GetMatchingRow(String[] matchcolumnnames, String[] matchcolumnvalues, bool compareCase = true)
        {
            return base.GetMatchingRow(matchcolumnnames, matchcolumnvalues, compareCase);
        }

        /// <summary>
        /// This function is check if any studies listed in Studies tab. Returns count of the number of search result appeared.
        /// </summary>
        public int CheckStudyListCount()
        {
            return base.CheckStudyListCount();
        }
        /// <summary>
        /// This function will get the study details from Study listing on STudies tab based on column provided.
        /// </summary>
        /// <param name="columnname">Provide the column name from the LIST(First Name:Patient Name:Modality:Study Date:Patient ID:Description:Accession Number:Referring Physician:Images:Institutions:Middle Name:Last Name:Patient DOB:Gender:Gender:Study ID:Data Source:Insititutions:Issuer of PID:Study UID:Body Part:Procedure:Procedure)</param>
        /// <returns>The required data as string array</returns>
        public string[] GetStudyDetails(string columnname)
        {
            string colname = GetStudyListColumnID(columnname);
            //***************Refer Column names below to be provided as input to this function
            //First Name:Patient Name:Modality:Study Date:Patient ID:Description:Accession Number:Referring Physician:Images:Institutions:Middle Name:Last Name:Patient DOB:Gender:Gender:Study ID:Data Source:Insititutions:Issuer of PID:Study UID:Body Part:Procedure:Procedure
            //**********************************************************
            int columnno = 0;
            IWebElement table = Driver.FindElement(By.Id(Locators.ID.StudyListTable));
            List<IWebElement> tr = table.FindElements(By.TagName("tr")).ToList();
            //Get Column number to avoid double loop
            if (tr.Capacity>1)
            {
                List<IWebElement> data = tr[1].FindElements(By.TagName("td")).ToList();
                for (columnno = 1; columnno < data.Capacity; columnno++)
                {
                    if (data[columnno].GetAttribute("aria-describedby").Equals(colname))
                    {
                        break;
                    }
                } 
            }
            string[] result = new string[tr.Capacity - 1];
            for (int i = 1; i < tr.Capacity; i++)
            {
                result[i - 1] = tr[i].FindElements(By.TagName("td")).ElementAt(columnno).Text;
            }
            return result;
        }

        /// <summary>
        /// This function returns the column IDs for Study List on STudies page
        /// </summary>
        /// <param name="columnname"></param>
        /// <returns></returns>
        public string GetStudyListColumnID(string columnname)
        {
            string col;
            //***************Refer Column names below to be provided as input
            if (columnname == "First Name") { col = "gridTableStudyList_firstName"; }
            else if (columnname == "Patient Name") { col = "gridTableStudyList_patName"; }
            else if (columnname == "Modality") { col = "gridTableStudyList_modality"; }
            else if (columnname == "Study Date") { col = "gridTableStudyList_studyDateTime"; }
            else if (columnname == "Patient ID") { col = "gridTableStudyList_patientID"; }
            else if (columnname == "Description") { col = "gridTableStudyList_description"; }
            else if (columnname == "Accession Number" || columnname == "Accession") { col = "gridTableStudyList_accession"; }
            else if (columnname == "Referring Physician" || columnname == "Refer. Physician") { col = "gridTableStudyList_referringPhysicianName"; }
            else if (columnname == "Images" || columnname == "# Images") { col = "gridTableStudyList_numberOfImages"; }
            else if (columnname == "Institutions") { col = "gridTableStudyList_institutions"; }
            else if (columnname == "Middle Name") { col = "gridTableStudyList_middleName"; }
            else if (columnname == "Last Name") { col = "gridTableStudyList_lastName"; }
            else if (columnname == "Patient DOB") { col = "gridTableStudyList_patientDOB"; }
            else if (columnname == "Gender") { col = "gridTableStudyList_gender"; }
            else if (columnname == "Gender") { col = "gridTableStudyList_gender"; }
            else if (columnname == "Study ID") { col = "gridTableStudyList_studyID"; }
            else if (columnname == "Data Source") { col = "gridTableStudyList_dataSourceUIStr"; }
            else if (columnname == "Insititutions") { col = "gridTableStudyList_institutions"; }
            else if (columnname == "Issuer of PID") { col = "gridTableStudyList_pidIssuer"; }
            else if (columnname == "Study UID") { col = "gridTableStudyList_studyUid"; }
            else if (columnname == "Body Part") { col = "gridTableStudyList_bodyPart"; }
            else if (columnname == "Procedure") { col = "gridTableStudyList_procedure"; }
            //else return First Name
            else { col = "gridTableStudyList_firstName"; }

            return col;
        }

        /// <summary>
        /// Saves preset on studies page
        /// </summary>
        /// <param name="presetName">Enters the Name of the preset that should be saved</param>
        /// <returns></returns>
        public void SavePreset(string presetName)
        {
            PageLoadWait.WaitForElement(By.Id(Locators.ID.SavePresetButton), WaitTypes.Visible);
            Click("id", Locators.ID.SavePresetButton);
            PageLoadWait.WaitForElement(By.Id(Locators.ID.PresetTextbox), WaitTypes.Visible);
            SetText("id", Locators.ID.PresetTextbox, presetName);
            Click("id", Locators.ID.PresetSaveButton);
            PageLoadWait.WaitForPageLoad(5);
        }
        /// <summary>
        /// This function returns the main heading of each Group (when grouped by a certain value)
        /// </summary>
        /// <returns>string[] </returns>
        public string[] GetGroupByStudyListHeading()
        {
            List<IWebElement> element = Driver.FindElements(By.CssSelector(Locators.CssSelector.GroupByPlusMinusHeading)).ToList();
            string[] result = new string[element.Capacity];
            for (int i = 0; i < element.Capacity; i++)
            {
                result[i] = element[i].FindElement(By.XPath("..")).Text;
            }
            return result;
        }

        /// <summary>
        /// Gets the names of all STudylist column names
        /// </summary>
        /// <returns></returns>
        public string[] GetStudyListColumnNames()
        {
            IWebElement table = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyListColumnTable));
            List<IWebElement> th = table.FindElements(By.TagName("th")).ToList();
            List<string> result = new List<string>();
            for (int i = 1; i < th.Capacity; i++)
            {
                if (!th[i].GetAttribute("style").Contains("display: none;"))
                {
                    result.Add(th[i].Text.Trim());
                }
            }
            string[] final = result.ToArray();
            return final;
        }

        /// <summary>
        ///     This function selects the specified data source
        /// </summary>
        /// <param name="dataSource">The data source to be selected</param>
        public void SelectDataSource(string dataSource)
        {
            Click("cssselector", Locators.CssSelector.DataSourceList);
            PageLoadWait.WaitForElement(By.LinkText(dataSource), WaitTypes.Visible);
            Driver.FindElement(By.LinkText(dataSource)).Click();

        }
        public Boolean PatientExistsinTransfer(string patientname)
        {
            try
            {
                ReadOnlyCollection<IWebElement> elements =
                   Driver.FindElements(
                       By.XPath("//*[@id='ctl00_TransferJobsListControl_parentGrid']/tbody/tr"));


                //ReadOnlyCollection<IWebElement> elements =
                //    Driver.FindElements(
                //        By.XPath("//table[@id='ctl00_DataQCControl_datagrid']/tbody/tr"));
                for (int i = 1; i < elements.Count + 1; i++)
                {
                    if
                         (
                           //*[@id="ctl00_DataQCControl_datagrid"]/tbody/tr[2]/td[1]/span
                           elements[i].FindElement(By.XPath("//*[@id='ctl00_DataQCControl_datagrid']/tbody/tr[2]")).Text.Contains(patientname)
                         )
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + patientname +
                                         " for exception : " + ex.Message);
            }

            return false;
        }

        public Boolean PatientExists(string accno)
        {
            try
            {
                ReadOnlyCollection<IWebElement> elements =
                    Driver.FindElements(
                        By.XPath("//table[@id='gridTableStudyList']/tbody/tr"));

                for (int i = 1; i < elements.Count + 1; i++)
                {
                    if
                        (
                          //*[@id="1"]/td[7]
                          elements[i - 1].FindElement(By.XPath("//*[@id=" + i + "]/td[7]")).Text.Contains(accno)
                        )
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + accno +
                                         " for exception : " + ex.Message);
            }

            return false;
        }
        /// <summary>
        ///     This function checks in the from calendar on Studies tab if the date is today's date
        /// </summary>
        /// <param name="filePath">Physical path of the web.config file</param>
        /// <param name="key">The key that needs to be updated</param>
        /// <param name="value">The value with which the key has to be updated</param>
        public bool CheckFromCalendarCurrentDate()
        {
            PageLoadWait.WaitForElement(By.Id("DateRangeSelectorCalendarFrom_calcells"), WaitTypes.Visible);
            IWebElement CalendarTable = BasePage.Driver.FindElement(By.Id("DateRangeSelectorCalendarFrom_calcells"));
            List<IWebElement> CalendarTD = CalendarTable.FindElements(By.TagName("td")).ToList();
            string date = null;
            foreach (var item in CalendarTD)
            {
                if (item.GetAttribute("class").Contains("curdate"))
                {
                    date = item.Text;
                    break;
                }
            }
            if (Convert.ToInt32(date) == DateTime.Now.Day)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This function clicks the column heading name (used for sorting the column)
        /// </summary>
        /// <param name="columnname">Specify column name from List (First Name:Patient Name:Modality:Study Date:Patient ID:Description:Accession Number:Referring Physician:Images:Institutions:Middle Name:Last Name:Patient DOB:Gender:Gender:Study ID:Data Source:Insititutions:Issuer of PID:Study UID:Body Part:Procedure:Procedure)</param>
        public void ClickColumnHeading(string columnname)
        {
            //***************Refer Column names below to be provided as input to this function
            //First Name:Patient Name:Modality:Study Date:Patient ID:Description:Accession Number:Referring Physician:Images:Institutions:Middle Name:Last Name:Patient DOB:Gender:Gender:Study ID:Data Source:Insititutions:Issuer of PID:Study UID:Body Part:Procedure:Procedure
            //**********************************************************
            columnname = columnname.Trim();
            string colname = "jqgh_" + GetStudyListColumnID(columnname);
            IWebElement ColumnElement = Driver.FindElement(By.Id(colname));
            ColumnElement.Click();
            PageLoadWait.WaitForPageLoad(10);
        }


        /// <summary>
        /// To add  study to a study folder with specific datasources from Studies Tab
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="Accessions"></param>
        /// <param name="Datasources"></param>
        public void AddDatasourceSpecificStudyToStudyFolder(String folderpath, String Accession, String Datasource = "All", String StudyNotes = null, int locale = 0)
        {
            StudyViewer studyviewer;
            SearchStudy(AccessionNo: Accession, Datasource: Datasource);
            if (locale == 0) SelectStudy("Accession", Accession);            
            else SelectStudy(GetStudyGridColName("Accession"), Accession);
            
            studyviewer = LaunchStudy();
            if (locale == 0) studyviewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
            else studyviewer.SelectToolInToolBar(ReadDataFromResourceFile(Localization.Tooltip, "data", "AddConferenceStudy"), "review", 1);
            
            studyviewer.AddStudyToStudyFolder(folderpath, notes: StudyNotes);
            CloseStudy();
        }

        public void CloseTransferStatusWindow()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_transferDrawer_TransferStatusDiv")));
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('#m_transferDrawer_TransferJobsListControl_m_closeDialogButton').click()");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
        }

        public bool VerifyStudyPerformed(String date, String datetype)
        {
            bool value = false;
            DateTime CurrentDate = DateTime.Now.Date;
            DateTime GivenDate = Convert.ToDateTime(date);
            DateTime Difference = DateTime.Now;
            int res1 = 0;
            int res2 = 0;
            switch (datetype.ToLowerInvariant())
            {
                case "last month":
                    Difference = DateTime.Now.AddMonths(-1);
                    res1 = DateTime.Compare(CurrentDate.Date, GivenDate.Date);
                    res2 = DateTime.Compare(Difference.Date, GivenDate.Date);
                    if (res1 > 0 && res2 < 0)
                    {
                        value = true;
                    }
                    else if (res1 == 0 || res2 == 0)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    break;

                case "last 2 months":
                    Difference = DateTime.Now.AddMonths(-2);
                    res1 = DateTime.Compare(CurrentDate.Date, GivenDate.Date);
                    res2 = DateTime.Compare(Difference.Date, GivenDate.Date);
                    if (res1 > 0 && res2 < 0)
                    {
                        value = true;
                    }
                    else if (res1 == 0 || res2 == 0)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    break;

                case "last 2 years":
                    Difference = DateTime.Now.AddYears(-2);
                    res1 = DateTime.Compare(CurrentDate.Date, GivenDate.Date);
                    res2 = DateTime.Compare(Difference.Date, GivenDate.Date);
                    if (res1 > 0 && res2 < 0)
                    {
                        value = true;
                    }
                    else if (res1 == 0 || res2 == 0)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    break;
            }
            return value;
        }

        public string TransferStudy(String Datasource, int TimeOut = 120)
        {
            WebDriverWait wait = new WebDriverWait(Driver, new TimeSpan(0, 0, TimeOut));
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                ClickButton("div#ButtonsDiv table td>div>input#m_transferButton");
            else
                Driver.FindElement(By.CssSelector("div#ButtonsDiv table td>div>input#m_transferButton")).Click();
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                ClickButton("div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton");
            else
                Driver.FindElement(By.CssSelector("div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")).Click();
            Dropdown_TransferTo().SelectByText(Datasource);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                ClickButton("#ctl00_StudyTransferControl_TransferButton");
            else
                Btn_StudyPageTransferBtn().Click();
            PageLoadWait.WaitForFrameLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid tr:nth-of-type(2) span[title='Succeeded']")));
            string status = Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid tr:nth-of-type(2) span[title]"))[3].GetAttribute("title");
            TransferStatusClose();
            return status;
        }


        /// <summary>
        ///   This Method to Grant access to a user.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="user"></param>
        public void GrantAccessToUsers(String domainName, string user)
        {
            try
            {
                try
                {
                    GrantAccessBtn().Click();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Clicking grant access via js. " + e);
                    ClickElement(GrantAccessBtn());
                }
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));
                try
                {
                    //Select domain from dropdown
                    new SelectElement(PageLoadWait.WaitForElement(By.CssSelector("[id$='StudySharingControl_m_domainSelector']"), BasePage.WaitTypes.Visible, 15)).SelectByText(domainName);
                }
                catch { }
                //Enter "tes" in filter users
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_userFilterInput']")));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_userFilterInput']\").click()");
                BasePage.Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).SendKeys(user);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_m_userlist_hierarchyUserList_itemList']")));
                IWebElement table = BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_userlist_hierarchyUserList_itemList']"));
                table.Click();
                BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_userlist_Button_Add']")).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_GrantAccessButton']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_GrantAccessButton']")).Click();
                //BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DialogContentDiv")));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error while grant access" + ex);
            }


        }


        /// <summary>
        /// To get the row values from search result.       
        /// </summary>
        ///  <param name="RowNumber"> It should start from 1 </param>
        /// <returns></returns>
        public IList<String> GetRowValuesInStudyList(int RowNumber)
        {
            IList<String> demographics = new List<String>();
            demographics = Driver.FindElements(By.CssSelector(Studies.SearchGridBody + " > div table tr[id='" + RowNumber + "'] td")).Select<IWebElement, String>
                        (graphics => graphics.Text).ToList();
            return demographics.Where(s => !String.IsNullOrEmpty(s)).ToList();
        }




    }
}

