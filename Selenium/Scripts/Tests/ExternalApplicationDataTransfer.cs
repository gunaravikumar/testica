using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using System.Diagnostics;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;

namespace Selenium.Scripts.Tests
{
    class ExternalApplicationDataTransfer : BasePage
    {

        public Login login { get; set; }
        public string filepath { get; set; }

        Studies studies = new Studies();
        ServiceTool servicetool = new ServiceTool();

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public ExternalApplicationDataTransfer(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Data Transfer - Initial Setup
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27564(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data                

                //Step 1 - Pre-conditions
                ExecutedSteps++;

                //Step 2 - Enable Data Transfer                
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableDataTransfer();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.SetEnableFeaturesTransferService();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.EnableTransferService();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 3 - Pre-conditions             
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Close service tool
                servicetool.CloseServiceTool();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Data Transfer - 1.0 Data Transfer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27565(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            StudyViewer viewer;

            WebDriverWait wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 300));

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data                
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");                
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientLastName");

                String[] Accessions = AccessionList.Split(':');                         

                //Step 1 - Pre-conditions - Initial setup
                ExecutedSteps++;

                //Step 2 - Login WebAccess as Administrator and select Domain Management tab               
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");          
                ExecutedSteps++;

                //Step 3 - Edit the current Default System Domain, select the Enable Data Transfer box..
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                ExecutedSteps++;

                //Step 4 - Remove all but keep one Data Sources from the Connected box
                domainmanagement.ConnectAllDataSources();
                string[] connectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceConnectedListBox']");
                if (connectedAllOptions.Length > 1)
                {
                    for (int i = 0; i < connectedAllOptions.Length - 1; i++)
                    {
                        domainmanagement.DatasourceConnectedDropDown().SelectByIndex(0);
                        Click("cssselector", "input[id$='_DataInfo_DisconnectDataSource']");
                    }
                }
                domainmanagement.AddToolsToToolbarByName(new string[] { "Transfer Study" });
                ExecutedSteps++;

                //Step 5 - Save the settings
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step 6 - Go to Role Management tab, ensure option "Allow Transfer" is enabled for the default role.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(DefaultDomain);
                rolemanagement.SearchRole(DefaultRoleName);
                rolemanagement.SelectRole(DefaultRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.ClickSaveEditRole();
                studies = login.Navigate<Studies>();
                PageLoadWait.WaitForFrameLoad(10);
                if (IsElementPresent(By.CssSelector("#m_transferButton")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Go Back to the Domain Management page.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step 8 - Edit the current Default System Domain and add all the Data Sources.
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.ConnectAllDataSources();
                ExecutedSteps++;

                //Step 9 - Save the settings
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step 10 - Go back to the Studies page.
                studies = login.Navigate<Studies>();
                PageLoadWait.WaitForFrameLoad(10);
                if (studies.Btn_StudyPageTransfer().Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 - Select a study from a DICOM datasource by a single click
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accessions[0]);
                ExecutedSteps++;

                //Step 12 - Click "Transfer" button
                Driver.FindElement(By.CssSelector("div#ButtonsDiv table td>div>input#m_transferButton")).Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.FindElement(By.CssSelector("div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")).Click();                
                ExecutedSteps++;

                //Step 13 - Select Transfer button.
                studies.Dropdown_TransferTo().SelectByText(Config.SanityPACSAETitle);
                studies.Btn_StudyPageTransferBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Succeeded']")));
                string status = BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span")).GetAttribute("title");
                if (status == "Succeeded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.TransferStatusClose();

                //Step 14 - Click Transfer Study icon from toolbar
                viewer = LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.TransferStudy);
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#m_transferDrawer_StudyTransferControl_NewDestinationButton")));
                PageLoadWait.WaitForFrameLoad(20);                
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (studies.Btn_StudyTransfer().Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - Select a datasource 
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                new SelectElement(Driver.FindElement(studies.By_TransferTo())).SelectByText(Config.SanityPACSAETitle);
                ExecutedSteps++;

                //Step 16 - Select Transfer button.
                studies.Btn_StudyTransfer().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_transferDrawer_TransferStatusDiv")));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_transferDrawer_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Succeeded']")));
                String tStatus = Driver.FindElement(By.CssSelector("#m_transferDrawer_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Succeeded']")).Text;                
                if(tStatus ==  "Succeeded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                studies.TransferStatusClose(true);

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Data Transfer - 2.0 DICOM Transfer from I-Store online Data source (configure systems)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27566(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            DomainManagement domainmanagement;
            UserManagement usermanagement;
            StudyViewer viewer;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data                
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String DataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientLastName");

                String[] Accessions = AccessionList.Split(':');
                String[] datasource = DataSource.Split(':');

                String U1 = "U1_27566_" + new Random().Next(1, 1000);
                String D1 = "Domain_27566_" + new Random().Next(1, 1000);
                String Role1 = "Role_27566_" + new Random().Next(1, 1000);

                //Step 1 - Pre-conditions 
                ExecutedSteps++;

                //Step 2 - Pre-conditions 
                ExecutedSteps++;

                //Step 3 - Pre-conditions 
                ExecutedSteps++;

                //Step 4 - Pre-conditions 
                ExecutedSteps++;

                //Step 5 - Pre-conditions 
                ExecutedSteps++;

                //Step 6 - Pre-conditions 
                ExecutedSteps++;

                //Step 7 - Pre-conditions 
                ExecutedSteps++;

                //Step 8 - Pre-conditions 
                ExecutedSteps++;

                //Step 9 - Pre-conditions 
                ExecutedSteps++;

                //Step 10 - Pre-conditions 
                ExecutedSteps++;

                //Step 11 - Pre-conditions                 
                ExecutedSteps++;

                //Step 12 - Log in iConnect Access as administrator               
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step 13 - Edit the current Default System Domain, select the Enable Data Transfer box..
                domainmanagement.ClickNewDomainBtn();
                ExecutedSteps++;

                //Step 14 
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForFrameLoad(10);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", D1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", D1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", D1 + "Inst");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", D1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", D1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", D1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", D1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", D1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", Role1);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", Role1);
                string[] DisConnectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceDisconnectedListBox']");
                if (Array.IndexOf(DisConnectedAllOptions, "DVTk") > -1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - In the datasources section select each datasource and click on "Connect" to move them all to the right hand side.
                domainmanagement.ConnectAllDataSources();
                ExecutedSteps++;

                //Step 16 - Select Save
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step 17                
                ExecutedSteps++;

                //Step 18 - Select the User Management tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 19 - Select the Domain from the dropdown list "D1"
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.DomainSelector_InUserSearch().SelectByText(D1);
                ExecutedSteps++;

                //Step 20 - Select the 'New User' button
                usermanagement.NewUsrBtn().Click();
                ExecutedSteps++;

                //Step 21 - Create User U1
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewUserButton")));
                ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID");
                SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", U1);
                ClearText("id", "m_sharedNewUserControl_UserInfo_LastName");
                SetText("id", "m_sharedNewUserControl_UserInfo_LastName", U1);
                ClearText("id", "m_sharedNewUserControl_UserInfo_FirstName");
                SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", U1);
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", U1);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", U1);
                SelectElement RoleDropDown = new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ChooseRoleDropDownList']")));
                RoleDropDown.SelectByText(Role1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                this.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 22 - Log out as Administrator
                login.Logout();
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Data Transfer - 3.0 DICOM Transfer from I-Store online Data source
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27567(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            StudyViewer viewer;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data                
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String DataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientLastName");

                String[] Accessions = AccessionList.Split(':');
                String[] datasource = DataSource.Split(':');

                String U1 = "U1_27566_" + new Random().Next(1, 1000);
                String D1 = "Domain_27566_" + new Random().Next(1, 1000);
                String R1 = "Role_27566_" + new Random().Next(1, 1000);

                //Step 1 - Preconditions
                ExecutedSteps++;
                
                //Step 2 - Login in as user U1 in i-Connect Access                          
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                //Domain
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(D1, D1, 0);
                domainmanagement.SearchDomain(D1);
                domainmanagement.SelectDomain(D1);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0);
                PageLoadWait.WaitForFrameLoad(5);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                //Role
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.ClickNewRoleBtn();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.CreateRole(D1, R1, DataTransfer: 1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                //User
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(U1, D1, R1);
                login.Logout();

                login.LoginIConnect(U1, U1);
                ExecutedSteps++;

                //Step 3 - Go to the destination email and select the URL                
                ExecutedSteps++;

                //Step 3
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);

                //Step 4
                studies.Btn_StudyPageTransfer().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                studies.Dropdown_TransferTo().SelectByText(datasource[0]);
                studies.Btn_StudyPageTransferBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                ReadOnlyCollection<IWebElement> tr = Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid tbody tr"));
                IList<IWebElement> td = tr[1].FindElements(By.CssSelector("td"));
                int sec = 0;
                while (sec < 10 && td[5].Text.Equals("Succeeded"))
                {
                    Thread.Sleep(5000);
                }
                if (td[5].Text.Equals("Succeeded"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17                
                ExecutedSteps++;

                //Step 18 - Select the User Management tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 19 - Select the Domain from the dropdown list "D1"
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.DomainSelector_InUserSearch().SelectByText(D1);
                ExecutedSteps++;

                //Step 20 - Select the 'New User' button
                usermanagement.NewUsrBtn().Click();
                ExecutedSteps++;

                //Step 21 - Create User U1
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewUserButton")));
                ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID");
                SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", U1);
                ClearText("id", "m_sharedNewUserControl_UserInfo_LastName");
                SetText("id", "m_sharedNewUserControl_UserInfo_LastName", U1);
                ClearText("id", "m_sharedNewUserControl_UserInfo_FirstName");
                SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", U1);
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", U1);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", U1);
                SelectElement RoleDropDown = new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ChooseRoleDropDownList']")));
                RoleDropDown.SelectByText(R1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                this.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 22 - Log out as Administrator
                login.Logout();
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Data Transfer - 7.0 Patient Search
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27569(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data      
                String PatientNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                String[] PatientNames = PatientNameList.Split(':');
                String[] Accessions = AccessionList.Split(':');

                //Step 1 - Pre-conditions
                ExecutedSteps++;

                //Step 2 - Enable Patients search                
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                servicetool.EnablePatient();
                servicetool.wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                //MPI
                servicetool.SetEnableFeaturesMPI();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickRadioButton("RB_MergeEMPI");
                servicetool.wpfobject.ClickRadioButton("Attribute based search", 1);
                servicetool.wpfobject.ClickButton("Apply", 1);
                servicetool.wpfobject.WaitTillLoad();
                //PMJ
                servicetool.NavigateToPMJFeaturesTab();
                wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableOtherDocumentsTab, 1);
                //XDS
                servicetool.NavigateToXDSTab();
                servicetool.wpfobject.SelectCheckBox("Enable XCA", 1);
                servicetool.wpfobject.ClickButton("Apply", 1);
                servicetool.wpfobject.WaitTillLoad();
                //Restart
                servicetool.RestartService();
                servicetool.wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                ExecutedSteps++;

                //Step 3 - Pre-conditions - The datasources have been setup    
                ExecutedSteps++;

                //Step 4 - Setup the following Data sources for MPI and XDS in the i-Connect service tool.
                ExecutedSteps++;

                //Step 5 - Change the status of transfers interval time
                String file = Config.ImageTransferExeConfigPath;
                var keyvalues = new Dictionary<String, String>();
                keyvalues.Add("setting", "<value>5</value>");
                ReadXML.UpdateXML(file, keyvalues, "name", "TemporaryJobRetryIntervalInSeconds");
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step 6 - From Patient tab in the Last Name box (e.g. John). Select Search button.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var patient = (Patients)login.Navigate("Patients");
                patient.InputData(PatientNames[0].Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (patient.PatientExists(PatientNames[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Select (e.g. John, PQA, or John, Doe Homer) by double clicks and go to Patients > Patient Record > Studies tab.
                patient.LoadStudyInPatientRecord(PatientNames[0].Trim());
                if (patient.IsElementPresent(By.CssSelector("#PmjPatientDemographicFrame")) &&
                    patient.TransferBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Select Other Documents tab
                result.steps[++ExecutedSteps].status = "Hold";

                //Step 9 - Select Xds tab > Documents sub-tab, Select a non-dicom file 
                NavigateToXDSTabs("Xds");
                NavigateToXsdDocumentsPatients();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.XPath("//*[@id='XdsPageDocsGrid']/tbody/tr[6]/td[1]/span")).Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (!patient.TransferToBtn().Enabled &&
                    !BasePage.Driver.FindElement(By.CssSelector("#m_destinationDataSources")).Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 - Go back to Patients > Patient Record > Studies tab and select a study by single click. click Transfer button
                NavigateToXDSTabs("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.XPath("//*[@id='RadiologyStudiesListControl_parentGrid']/tbody/tr[2]/td[3]")).Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                patient.TransferBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                if (IsElementVisible(By.CssSelector("#ctl00_StudyTransferControl_m_destinationSources")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 - From the drop down menu, select one a datasource
                studies.Dropdown_TransferTo().SelectByText("EA-116");
                studies.Btn_StudyPageTransferBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForElement(By.CssSelector("[id$='_TransferJobsListControl_RefreshTrasferButton']"), WaitTypes.Visible, 20);
                BasePage.Driver.FindElement(By.CssSelector("[id$='_TransferJobsListControl_RefreshTrasferButton']")).Click();
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Succeeded']")));
                //IWebElement succeed = Driver.FindElement(By.CssSelector(" #ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Succeeded']"));
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Failed']")));
                IWebElement succeed = Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Failed']"));
                if (succeed.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Go back to the Patients > Patient Record > Studies tab, and select a study. But before selecting Transfer button, make the destination datasource offline (network disconnected) Select Transfer button            ExecutedSteps++;
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 13
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 14 
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 15
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 16
                result.steps[++ExecutedSteps].status = "Not Automated";

                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Data Transfer - 8.0 Testing for "Temporary Failure"
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27570(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data      
                String PatientNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String DataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");

                String[] PatientNames = PatientNameList.Split(':');
                String[] Accessions = AccessionList.Split(':');
                String[] datasource = DataSource.Split(':');

                //Step 1 - pre-condition
                ExecutedSteps++;

                //Step 2 - Confirm the data transfer flag is true in WebAccess\WebAccess\Web.config
                //login.SetWebConfigValue(Config.webconfig, "EnableDataTransfer", "true");
                ExecutedSteps++;

                //Step 3 - In the DICOM tab unselect the WADO Base URL- box                
                //servicetool.LaunchServiceTool();
                //servicetool.NavigateToConfigToolDataSourceTab();
                //wpfobject.WaitTillLoad();
                //servicetool.wpfobject.SelectTabFromTabItems("Data Source");
                //wpfobject.WaitTillLoad();
                //wpfobject.SelectFromListView(0, "VMSSA-4-38-131");
                //wpfobject.WaitTillLoad();
                //wpfobject.GetMainWindowByTitle("Detail of the data source");
                //servicetool.wpfobject.WaitTillLoad();
                //servicetool.NavigateToDataSourceDicomTab();
                //wpfobject.UnSelectCheckBox("WadoBaseUrlCheckBox");
                //wpfobject.ClickButton("Button_DataSourceOK");
                //wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                //wpfobject.WaitTillLoad();
                //servicetool.RestartIISandWindowsServices();
                //wpfobject.WaitTillLoad();
                //servicetool.CloseConfigTool();
                ExecutedSteps++;

                //Step 4 - change the time interval (60 in second) to a new interval time ( 0 sec.)      
                //String file = @"D:\Merge\Automation\ICA_6.2\ServiceFactoryConfiguration.xml";
                //var keyvalues = new Dictionary<String, String>();
                //keyvalues.Add("add", "<parameter name=\"timeout\" class=\"int\" value=\"5\" />" 
                //                    + "\n" 
                //                    + "<parameter name=\"scuAETitle\" class=\"string\" value=\"TFRSCU_{LOCALHOST_UPPERCASE}\" />"
                //                    + "\n");
                //ReadXML.UpdateXML(file, keyvalues, "key", "RadiologyData.Store");
                //servicetool.RestartIIS();
                ExecutedSteps++;

                //Step 5 - Login to iConnect and select Patients tab, enter "jo" in Last Name field and select search.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var patient = (Patients)login.Navigate("Patients");
                patient.InputData(PatientNames[0].Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (patient.PatientExists(PatientNames[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Select John, PQA, or John, Doe Homer by double clicking on the name
                patient.LoadStudyInPatientRecord(PatientNames[0].Trim());
                if (patient.IsElementPresent(By.CssSelector("#PmjPatientDemographicFrame")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - transfer the DICOM file to the configured DICOM data source.
                NavigateToXDSTabs("Xds");
                NavigateToXsdDocumentsPatients();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.XPath("//*[@id='XdsPageDocsGrid']/tbody/tr[7]/td[1]/span")).Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                studies.Dropdown_TransferTo().SelectByText(datasource[1]);
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_PatientMasterJacketControl_DataTransferControl_m_transferDataButton")).Click();

                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForElement(By.CssSelector("[id$='_TransferJobsListControl_RefreshTrasferButton']"), WaitTypes.Visible, 20);
                BasePage.Driver.FindElement(By.CssSelector("[id$='_TransferJobsListControl_RefreshTrasferButton']")).Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Retry']")));
                IWebElement retry = Driver.FindElement(By.CssSelector(" #ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Retry']"));
                if (retry.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - change the time interval (0 in second) to a new interval time ( 60 sec.). and restart the window service.
                //String file = @"D:\Merge\Automation\ICA_6.2\ServiceFactoryConfiguration.xml";
                //var keyvalues = new Dictionary<String, String>();
                //keyvalues.Add("add", "<parameter name=\"timeout\" class=\"int\" value=\"60\" />"
                //                    + "\n"
                //                    + "<parameter name=\"scuAETitle\" class=\"string\" value=\"TFRSCU_{LOCALHOST_UPPERCASE}\" />"
                //                    + "\n");
                //ReadXML.UpdateXML(file, keyvalues, "key", "RadiologyData.Store");
                //servicetool.RestartIIS();
                ExecutedSteps++;

                //Step 9 - Select Refresh button in the Transfer Jobs Status page and wait, in 1 minute.
                Thread.Sleep(60000);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.Driver.FindElement(By.CssSelector("[id$='_TransferJobsListControl_RefreshTrasferButton']")).Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Succeeded']")));
                IWebElement succeed = Driver.FindElement(By.CssSelector(" #ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Succeeded']"));
                if (succeed.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }
    }
}
