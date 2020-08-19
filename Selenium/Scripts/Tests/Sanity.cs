using System;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.Configuration;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml;
using System.Windows;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using TestStack.White.Factory;
using System.Text.RegularExpressions;
using Selenium.Scripts.Pages.eHR;
using Ranorex;
using Ranorex.Core;
using Ranorex.Controls;
using RXButton = Ranorex.Button;
using TestStack.White.UIItems.ListBoxItems;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Selenium.Scripts.Tests
{
    class Sanity
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public Web_Uploader webuploader { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        WpfObjects wpfobject;
        public RanorexObjects rnxobject { get; set; }
        public DomainManagement domain { get; set; }
        public RoleManagement role { get; set; }
        public UserManagement user { get; set; }
        public StudyViewer studyviewer { get; set; }
        String TestUser = "User_" + new Random().Next(1, 10000);
        public ServiceTool servicetool { get; set; }
        public EHR ehr { get; set; }
        public String rxpathmainwindow { get; set; }
        public bool IsHTML5 { get; private set; }
        //String User1 = "User1_9602";
        //String User2 = "User2_9602";
        String User1 = "User1_" + new Random().Next(1, 10000);
        String User2 = "User2_" + new Random().Next(1, 10000);
        //String User3 = "User3_" + new Random().Next(1, 10000);
        //String User5 = "User5_" + new Random().Next(1, 10000);
        //String User6 = "User6_" + new Random().Next(1, 10000);
        //String Group1 = "Group1_" + new Random().Next(1, 10000);
        //String Group2 = "Group2_" + new Random().Next(1, 10000);
        Dictionary<Object, String> domainattr;
        public Viewer viewer { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public Sanity(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();
            role = new RoleManagement();
            user = new UserManagement();
            studyviewer = new StudyViewer();
            viewer = new Viewer();
            domainattr = domain.CreateDomainAttr();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            hplogin = new HPLogin();
            ei = new ExamImporter();
            webuploader = new Web_Uploader();
            hphomepage = new HPHomePage();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            ehr = new EHR();
            rnxobject = new RanorexObjects();
            rxpathmainwindow = "/dom[@domain='localhost']";

        }

        /// <summary>
        /// Test case number 27824 - Initial Setup
        /// </summary>
        public TestCaseResult Test_27824(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                //
                //String username1 = Config.ar1UserName;
                //String password1 = Config.ar1Password;
                //String EmailID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                //String DataSourceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceDetailsList");
                //String XDSServerURL = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "XDSServerURL");

                //String[] DataSourceDetails = DataSourceList.Split('=');

                //Steps 1 - 35 : Performed as part of initial setup
                for (int i = 0; i < 35; i++)
                {
                    ExecutedSteps++;
                }

                try
                {
                    //Login as Administrator
                    login.LoginIConnect(adminUserName, adminPassword);

                    //Create a domain with Physicain,archivist  and Staff users
                    domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                    domainmgmt.CreateDomain(domainattr);
                    String NewDomain = domainattr[DomainManagement.DomainAttr.DomainName];
                    String NewRole = domainattr[DomainManagement.DomainAttr.RoleName];
                    usermgmt = (UserManagement)login.Navigate("UserManagement");
                    usermgmt.CreateUser(User1, AdminDomain, DefaultRole, 1, Config.emailid, 1, User1);
                    usermgmt.CreateUser(User2, NewDomain, NewRole, 1, Config.emailid, 1, User2);

                    //Logout
                    login.Logout();
                }
                catch (Exception)
                {
                    throw new Exception("Exception in Domain or one of the user creation in Sanity VP Initial Setup");
                }

                //Steps 36 - 44 : Create a new domain, create user under superadmingroup and create user under new domain
                for (int i = 0; i < 9; i++)
                {
                    ExecutedSteps++;
                }

                //Step 45 - 49 : Group creation - Done by the following test case in the test method level
                for (int i = 0; i < 5; i++)
                {
                    ExecutedSteps++;
                }
                                
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            //finally
            //{
            //    //PreCondition
            //    taskbar = new Taskbar();
            //    taskbar.Show();
            //}
        }

        /// <summary>
        /// Test case number 27825 - Licensing
        /// </summary>
        public TestCaseResult Test_27825(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            //Login firefox;
            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                //
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String BrowserList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BrowserList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String OriginalLicensePath = Config.OriginalLicensePath;
                String User4LicensePath = Config.User4LicensePath;
                String BackupLicensePath = Config.BackupLicensePath;

                String[] Browser = BrowserList.Split(':');
                String[] Patient = PatientIDList.Split(':');

                //login.Deletefile(@"C:\Users\saqibd\Desktop\results\temp\final.txt");
                //Creating backup of original license file
                File.Copy(OriginalLicensePath, BackupLicensePath, true);
                //Replacing the original license with 4 user license
                File.Copy(User4LicensePath, OriginalLicensePath, true);
                login.RestartIISUsingexe();

                //Step 1 - Already automated in Initial setup
                //precondition - Only instructions, hence not automated
                ExecutedSteps++;

                //Step 2
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                BasePage.MultiDriver.Add(BasePage.Driver);      // Save current State of Driver to 1st Element of Webdriver Array
                //Open Another Browser instance and perform operations
                BasePage.MultiDriver.Add(login.InvokeBrowser(Browser[0]));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Step 3:
                login.Navigate("Studies");
                login.ClearFields();
                int counter = 0;
                string[] step3list = new string[1] { "Patient ID" };
                while (!login.CheckStudyListColumnNames(step3list) && counter < 10)
                {
                    login.SetStudyListLayout(step3list, 1);
                    counter++;
                }
                login.SearchStudy("patientID", Patient[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Patient ID", Patient[0]);
                login.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                bool step3_1 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step3_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //login.CloseStudy();
                //Step 4
                //Switch to first browser instance
                BasePage.Driver = BasePage.MultiDriver[0];
                BasePage.wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 35));
                IAlert messagebox = PageLoadWait.WaitForAlert(BasePage.Driver);
                if (messagebox != null)
                {
                    messagebox.Accept();
                }
                //login.SetDriver(BasePage.MultiDriver[0]);
                login.Navigate("Studies");
                login.ClearFields();
                counter = 0;
                step3list = new string[1] { "Patient ID" };
                while (!login.CheckStudyListColumnNames(step3list) && counter < 10)
                {
                    login.SetStudyListLayout(step3list, 1);
                    counter++;
                }
                login.SearchStudy("patientID", Patient[1]);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Patient ID", Patient[1]);
                login.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                bool step3_2 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step3_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.CloseStudy();
                //Step 5
                login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement ele = BasePage.Driver.FindElement(By.Id(Locators.ID.LicenseTable));
                List<IWebElement> Licenselist = ele.FindElements(By.TagName("span")).ToList();
                int admincount = 0, usercount = 0;
                foreach (var item in Licenselist)
                {
                    if (item.Text.Contains(username))
                    {
                        admincount++;
                    }
                    else if (item.Text.Contains(User1))
                    {
                        usercount++;
                    }
                }
                if (admincount == 1 && usercount == 1)
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
                //Step 6
                BasePage.MultiDriver.Add(login.InvokeBrowser(Browser[1]));
                BasePage.Driver = BasePage.MultiDriver[2];
                BasePage.wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 35));
                //login.SetDriver(BasePage.Multi Driver[2]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;
                //Step 7
                BasePage.MultiDriver.Add(login.InvokeBrowser(Browser[2]));
                BasePage.Driver = BasePage.MultiDriver[3];
                BasePage.wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 35));
                //login.SetDriver(BasePage.MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(User2, User2);

                //Switch to 1st Admin window to check licenses
                BasePage.Driver = BasePage.MultiDriver[0];
                BasePage.wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 35));
                //login.SetDriver(BasePage.MultiDriver[0]);
                login.Navigate("Studies");
                login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                ele = BasePage.Driver.FindElement(By.Id(Locators.ID.LicenseTable));
                Licenselist = ele.FindElements(By.TagName("span")).ToList();
                admincount = 0; usercount = 0;
                foreach (var item in Licenselist)
                {
                    if (item.Text.Contains(username))
                    {
                        admincount++;
                    }
                    else if (item.Text.Contains(User1))
                    {
                        usercount++;
                    }
                    else if (item.Text.Contains(User2))
                    {
                        usercount++;
                    }
                }
                if (admincount == 2 && usercount == 2)
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
                //Step 8
                BasePage.MultiDriver.Add(login.InvokeBrowser(Browser[3]));
                BasePage.Driver = BasePage.MultiDriver[4];
                BasePage.wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 35));
                //login.SetDriver(BasePage.MultiDriver[4]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(username1, password1);
                bool message = false;
                try
                {
                    message = BasePage.Driver.FindElement(By.CssSelector("span[id$='_LoginMasterContentPlaceHolder_ErrorMessage']")).GetAttribute("innerHTML").ToLower().Contains("system cannot log you in");
                }
                catch (Exception) { }
                //Fails if logged in
                if (message)
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
                login.Logout();

                //Replacing back the 4 user license with original license
                File.Copy(BackupLicensePath, OriginalLicensePath, true);
                login.RestartIISUsingexe();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                login.ResetDriver();
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                login.ResetDriver();
                login.Logout();

            }


        }

        /// <summary>
        /// Test case number 27826 - Administration
        /// </summary>
        public TestCaseResult Test_27826(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            UserManagement usermanagement;
            RoleManagement role;
            Studies study;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            //Login firefox;
            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;

                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String BrowserList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BrowserList");
                String ToolsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ToolsList");
                String StudyListColumns = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyListColumns");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String WarningMsg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WarningMsg");
                String EmergencySearchList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmergencySearch");
                String EmailID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String TabNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TabNames");
                String FilterValueList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilterValueList");
                String FilterName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilterName");
                String Step14_Study = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Studies");
                String StudyYear = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyYear");
                String SearchStudy = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyList");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");

                String[] Browser = BrowserList.Split(':');
                String[] Tools = ToolsList.Split(':');
                String[] FilterValue = FilterValueList.Split(':');
                String[] StudyColumns = StudyListColumns.Split(':');
                String[] EmergencySearch = EmergencySearchList.Split(':');
                String[] PID = PatientID.Split(':');
                String[] ICATabs = TabNames.Split(':');
                String[] Filter = FilterName.Split(':');
                String[] Step14_Studies = Step14_Study.Split(':');


                //Step 1 - Already automated in Initial setup
                //precondition - Only instructions, hence not automated
                ExecutedSteps++;

                //Step 2
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser(User1, DomainName);
                usermanagement.SelectUser(User1);
                usermanagement.ClickEditUser();
                usermanagement.SetText("id", Locators.ID.EmailIDUserMgmt, "test@test.com");
                usermanagement.Click("id", Locators.ID.UserSaveUserMgmt);
                usermanagement.SearchUser(User1, DomainName);
                usermanagement.SelectUser(User1);
                usermanagement.ClickEditUser();
                if (usermanagement.GetTextFromTextBox("id", Locators.ID.EmailIDUserMgmt) == "test@test.com")
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
                usermanagement.Click("id", Locators.ID.UserSaveUserMgmt);
                //Step 3
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, DomainName, 1);
                PageLoadWait.WaitForPageLoad(10);
                role.SearchRole(RoleName);
                role.SelectRole(RoleName);
                role.ClickEditRole();
                bool step3 = role.VerifyElementSelected("cssselector", Locators.CssSelector.ToolbarUseDomainCheckboxRoleMgmt);
                if (step3)
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
                // Step 4
                int counter = 0;
                role.UnCheckCheckbox("cssselector", Locators.CssSelector.ToolbarUseDomainCheckboxRoleMgmt);
                role.RemoveAllToolsFromToolBar();
                while (role.VerifyToolsMoved(Tools) && counter < 5)
                {
                    role.MoveToolsToToolbarSection(Tools);
                    counter++;
                }
                bool step4_1 = !role.VerifyToolsMoved(Tools);
                role.UnCheckCheckbox("cssselector", Locators.CssSelector.StudyListUseDomainRoleMgmt);
                counter = 0;
                role.SetStudyListLayout(StudyColumns);
                while (!role.CheckStudyListColumnNames(StudyColumns) && counter < 10)
                {
                    role.SetStudyListLayout(StudyColumns);
                    counter++;
                }
                bool step4_2 = role.CheckStudyListColumnNames(StudyColumns);

                role.SetCheckbox("cssselector", Locators.CssSelector.AllowEmergencyAccessCheckboxRole);
                role.ClickSaveRole();
                if (step4_1 && step4_2)
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

                //Step 5
                study = (Studies)login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                bool step5_1 = study.CheckStudyListColumnNames(StudyColumns);
                bool step5_2 = study.VerifyElementPresence("id", Locators.ID.EmergencySearchRadio);

                if (step5_1 && step5_2)
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

                //Step 6
                study.ClearFields();
                study.SearchStudy("patientID", PID[0]);
                counter = 0;
                string[] columnlist = new string[1] { "First Name" };
                while (!login.CheckStudyListColumnNames(columnlist) && counter < 10)
                {
                    login.SetStudyListLayout(columnlist, 1);
                    counter++;
                }
                study.SelectStudy(StudyColumns[0], FirstName);
                login.LaunchStudy(3);
                //Validate if all tools are present in toolbar
                bool[] step6 = new bool[Tools.Length];
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement toolbar = BasePage.Driver.FindElement(By.Id("reviewToolbar"));
                List<IWebElement> toollist = toolbar.FindElements(By.TagName("img")).ToList();
                for (int i = 0; i < Tools.Length; i++)
                {
                    foreach (var item in toollist)
                    {
                        string check = item.GetAttribute("title");
                        if (check.Equals(Tools[i]))
                        {
                            step6[i] = true;
                            break;
                        }
                    }
                }
                bool step6_1 = false;
                foreach (bool res in step6)
                {
                    if (!res)
                    {
                        step6_1 = false;
                        break;
                    }
                    else
                    {
                        step6_1 = true;
                    }
                }
                if (step6_1)
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
                //Step 7
                study.CloseStudy();
                ExecutedSteps++;
                //Step 8

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                study.SetRadioButton("id", "m_studySearchControl_m_emergencySearchRadio");
                PageLoadWait.WaitForElement(By.Id("EmergencySearchWarningText"), BasePage.WaitTypes.Visible);
                study.Click("id", "m_studySearchControl_EmergencyAcceptButton");
                PageLoadWait.WaitForElement(By.Id("m_studySearchControl_m_searchInputPatientLastName"), BasePage.WaitTypes.Clickable);
                string step8 = study.GetText("id", "SearchWarningSpan");
                if (step8 == WarningMsg)
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
                // Step 9

                PageLoadWait.WaitForElement(By.Id("StudySearchDimmerDiv"), BasePage.WaitTypes.Invisible);
                PageLoadWait.WaitForElement(By.Id("m_studySearchControl_m_searchInputPatientLastName"), BasePage.WaitTypes.Visible);
                study.SelectFromList("id", "m_studySearchControl_m_searchInputPatientGender", EmergencySearch[2], 1);
                study.SetText("id", "m_studySearchControl_PatientDOB", EmergencySearch[3]);
                study.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", EmergencySearch[0]);
                study.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", EmergencySearch[1]);
                study.Click("id", "m_studySearchControl_m_searchButton");
                if (study.CheckStudy(StudyColumns[0], EmergencySearch[1]))
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
                //Step 10
                login.Logout();
                login.LoginIConnect(User1, User1);
                //Navigate to studies and check number of columns
                study = (Studies)login.Navigate("Studies");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                bool step10_1 = study.CheckStudyListColumnNames(StudyColumns);

                //Load a study and check the number of tools
                study.ClearFields();
                counter = 0;
                while (!login.CheckStudyListColumnNames(columnlist) && counter < 10)
                {
                    login.SetStudyListLayout(columnlist, 1);
                    counter++;
                }
                study.SearchStudy("patientID", PID[0]);
                study.SelectStudy(StudyColumns[0], FirstName);
                login.LaunchStudy(3);
                bool[] step10 = new bool[Tools.Length];
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                toolbar = BasePage.Driver.FindElement(By.Id("reviewToolbar"));
                toollist = toolbar.FindElements(By.TagName("img")).ToList();
                for (int i = 0; i < Tools.Length; i++)
                {
                    foreach (var item in toollist)
                    {
                        string check = item.GetAttribute("title");
                        if (check.Contains(Tools[i]))
                        {
                            step10[i] = true;
                            break;
                        }
                    }
                }
                bool step10_2 = false;
                foreach (bool res in step10)
                {
                    if (!res)
                    {
                        step10_2 = false;
                    }
                    else
                    {
                        step10_2 = true;
                    }
                }
                if (step10_1 && step10_2)
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
                study.CloseStudy();
                login.Logout();
                //Step 11
                login.LoginIConnect(domainattr[DomainManagement.DomainAttr.UserID], domainattr[DomainManagement.DomainAttr.Password]);
                user = (UserManagement)login.Navigate("UserManagement");
                user.SearchUser(domainattr[DomainManagement.DomainAttr.FirstName]);
                user.SelectUser(domainattr[DomainManagement.DomainAttr.FirstName]);
                user.ClickEditUser();
                PageLoadWait.WaitForElement(By.Id(Locators.ID.EmailIDUserMgmt), BasePage.WaitTypes.Visible);
                user.SetText("id", Locators.ID.EmailIDUserMgmt, EmailID);
                //Save
                //user.Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");
                IWebElement SaveBtn = BasePage.Driver.FindElement(By.CssSelector("[id$='_SaveButton']"));
                user.Click("id", Locators.ID.UserSaveUserMgmt);
                //SaveBtn.Click();                
                BasePage.wait.Until(ExpectedConditions.StalenessOf(SaveBtn));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (user.VerifyElementPresence("id", "searchDiv"))
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
                login.Logout();
                //Step 12
                bool[] step12_1 = new bool[ICATabs.Length];
                bool step12_result = false;
                login.LoginIConnect(User1, User1);
                study = (Studies)login.Navigate("Studies");
                string[] step12 = study.GetAvailableTabs();
                if (step12.Length >= ICATabs.Length)
                {
                    for (int i = 0; i < ICATabs.Length; i++)
                    {
                        for (int j = 0; j < step12.Length; j++)
                        {
                            if (step12[i] == ICATabs[j])
                            {
                                step12_1[i] = true;
                                break;
                            }
                            else
                            {
                                step12_1[i] = false;
                            }

                        }
                    }
                    //Validate if all elements are true
                    foreach (bool res in step12_1)
                    {
                        if (!res)
                        {
                            step12_result = false;
                            break;
                        }
                        else
                        {
                            step12_result = true;
                        }
                    }
                    if (step12_result)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();
                //Step 13
                login.LoginIConnect(username, password);
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, domainattr[DomainManagement.DomainAttr.DomainName], 1);
                role.SearchRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.SelectRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.ClickEditRole();
                PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                role.SelectFromList("id", Locators.ID.DomainMgmtFilter, Filter[0], 1);
                role.SetText("id", Locators.ID.DomainMgmtFilterText, FilterValue[0]);
                role.Click("id", Locators.ID.DomainMgmtFilterAddButton);
                //BasePage.wait.Until<Boolean>((d) => { return d.FindElement(By.Id(Locators.ID.DomainMgmtFilterText)).GetAttribute("value").Length == 0; });
                PageLoadWait.WaitForPageLoad(3);
                role.SelectFromList("id", Locators.ID.DomainMgmtFilter, Filter[0], 1);
                role.SetText("id", Locators.ID.DomainMgmtFilterText, FilterValue[1]);
                role.Click("id", Locators.ID.DomainMgmtFilterAddButton);
                role.ClickSaveEditRole();
                login.Logout();
                ExecutedSteps++;
                //Step 14
                string[] step14_inst = new string[] { FilterValue[0], FilterValue[1] };
                login.LoginIConnect(User2, User2);
                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                study.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(30);
                study.ChooseColumns(new string[] { "Patient Name", "Institutions" });
                string[] step14 = study.GetStudyDetails("Institutions");
                //bool step14_res = (step14 == null || step14.Length == 0) ? false : Step14_Studies.Where(z => step14.Any(q => q.ToLower().Contains(z.ToLower()))).Count() == Step14_Studies.Length && step14.Where(z => Step14_Studies.Any(q => z.ToLower().Contains(q.ToLower()))).Count() == step14.Length;
                bool step14_res = (step14 == null || step14.Length == 0) ? false : step14_inst.Where(z => step14.Any(q => q.ToLower().Contains(z.ToLower()))).Count() == step14_inst.Length && step14.Where(z => step14_inst.Any(q => z.ToLower().Contains(q.ToLower()))).Count() == step14.Length;
                if (step14_res)
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
                //Step 15
                study.SelectStudy("Patient Name", SearchStudy);
                study.LaunchStudy();
                string step15 = study.GetText("id", Locators.ID.StudyNameDivViewer);
                if (step15.Contains(Step14_Studies[0]))
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
                study.CloseStudy();
                login.Logout();
                //Step 16
                login.LoginIConnect(username, password);
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, domainattr[DomainManagement.DomainAttr.DomainName], 1);
                role.SearchRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.SelectRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.ClickEditRole();
                PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                counter = 0;
                while (counter < 3)
                {
                    role.Click("xpath", "//*[@id='ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_SelectedFilterCriteriaListBox']/option[1]");
                    role.Click("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_RemoveButton");
                    counter++;
                }
                role.SelectFromList("id", Locators.ID.DomainMgmtFilter, Filter[1], 1);
                string[] Modality = new string[2] { FilterValue[2], FilterValue[3] };
                role.SelectFromMultipleList("id", Locators.ID.ModalityListBox, Modality);
                role.Click("id", Locators.ID.DomainMgmtFilterAddButton);
                PageLoadWait.WaitForPageLoad(3);
                role.ClickSaveEditRole();
                ExecutedSteps++;
                login.Logout();

                //Step 17
                login.LoginIConnect(User2, User2);
                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                study.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(15);
                string[] step17 = study.GetStudyDetails("Modality");

                bool step17_res = (step17 == null || step17.Length == 0) ? false : Modality.Where(z => step17.Any(q => q.ToLower().Contains(z.ToLower()))).Count() == Modality.Length && step17.Where(z => Modality.Any(q => z.ToLower().Contains(q.ToLower()))).Count() == step17.Length;
                if (step17_res)
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
                login.Logout();
                // Step 18
                //Perform Change in role filter - Accession, Patient ID, Refer. Physician in Loop
                bool[] step18_res = new bool[4];
                for (int i = 0; i < 4; i++)
                {
                    login.LoginIConnect(username, password);
                    role = (RoleManagement)login.Navigate("RoleManagement");
                    role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, domainattr[DomainManagement.DomainAttr.DomainName], 1);
                    role.SearchRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                    role.SelectRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                    role.ClickEditRole();
                    PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                    counter = 0;
                    while (counter < 2)
                    {
                        role.Click("xpath", "//*[@id='ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_SelectedFilterCriteriaListBox']/option[1]");
                        role.Click("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_RemoveButton");
                        counter++;
                    }
                    role.SelectFromList("id", Locators.ID.DomainMgmtFilter, Filter[i + 2], 1);
                    if (i == 2)   // A different textbox used for refering physician
                    {
                        role.SetText("id", Locators.ID.DomainMgmtReferPhysFilterText, FilterValue[i + 4]);
                    }
                    else
                    {
                        role.SetText("id", Locators.ID.DomainMgmtFilterText, FilterValue[i + 4]);
                    }
                    role.Click("id", Locators.ID.DomainMgmtFilterAddButton);
                    PageLoadWait.WaitForPageLoad(3);
                    role.ClickSaveEditRole();
                    login.Logout();

                    //verify Details:
                    login.LoginIConnect(User2, User2);
                    study = (Studies)login.Navigate("Studies");
                    study.ClearFields();
                    study.SearchStudy("Last Name", "*");
                    PageLoadWait.WaitForLoadingMessage(30);
                    string[] collist = new string[4] { "Body Part", "Refer. Physician", "Patient ID", "Patient Name" };
                    while (!login.CheckStudyListColumnNames(collist) && counter < 10)
                    {
                        login.SetStudyListLayout(collist, 1);
                        counter++;
                    }
                    string[] step18 = study.GetStudyDetails(Filter[i + 2]);
                    step18_res[i] = (step18 == null || step18.Length == 0) ? false : step18.Where(q => q.ToLower().Contains(FilterValue[i + 4].ToLower())).Count() == step18.Length;
                    login.Logout();
                }
                bool step18_final = false;
                foreach (bool res in step18_res)
                {
                    if (!res)
                    {
                        step18_final = false;
                        break;
                    }
                    else
                    {
                        step18_final = true;
                    }
                }
                if (step18_final)
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
                login.Logout();
                //Step 19
                login.LoginIConnect(username, password);
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, domainattr[DomainManagement.DomainAttr.DomainName], 1);
                role.SearchRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.SelectRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.ClickEditRole();
                counter = 0;
                while (counter < 2)
                {
                    role.Click("xpath", "//*[@id='ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_SelectedFilterCriteriaListBox']/option[1]");
                    role.Click("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_RemoveButton");
                    counter++;
                }
                PageLoadWait.WaitForPageLoad(3);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(3);
                ExecutedSteps++;
                login.Logout();

                //Step 20
                login.LoginIConnect(User2, User2);
                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                study.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step20_1 = study.GetStudyDetails(Filter[5]);
                bool step20 = (step20_1 == null || step20_1.Length == 0) ? false : step20_1.Where(q => q.ToLower().Contains(FilterValue[7].ToLower())).Count() == step20_1.Length;

                if (!step20)
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
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, domainattr[DomainManagement.DomainAttr.DomainName], 1);
                role.SearchRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.SelectRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                role.ClickEditRole();
                int counter = 0;
                while (counter < 2)
                {
                    role.Click("xpath", "//*[@id='ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_SelectedFilterCriteriaListBox']/option[1]");
                    role.Click("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_RemoveButton");
                    counter++;
                }
                PageLoadWait.WaitForPageLoad(3);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(3);
                role.SearchRole("SuperRole", "SuperAdminGroup");
                role.SelectRole("SuperRole");
                role.ClickEditRole();
                role.SetCheckbox("cssselector", Locators.CssSelector.ToolbarUseDomainCheckboxRoleMgmt);
                PageLoadWait.WaitForPageLoad(3);
                role.ClickSaveEditRole();
                login.Logout();
            }


        }

        /// <summary>
        /// Test case number 27827 - Studylist
        /// </summary>
        public TestCaseResult Test_27827(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            RoleManagement role;
            Studies study;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            //Login firefox;
            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;

                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String BrowserList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BrowserList");
                String StudyListColumns = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyListColumns");
                String DefaultStudyListColumns = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DefaultStudyListColumns");
                String StudySearch = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyList");
                String StudyName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String StudyYear = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyYear");
                String PresetNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PresetNames");
                String PriorityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String DataSourceNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceName");
                String FilterName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilterName");


                String[] Browser = BrowserList.Split(':');
                String[] StudyList = StudyListColumns.Split(':');
                String[] DefaultStudyList = DefaultStudyListColumns.Split(':');
                String[] Search_Step7 = StudySearch.Split(':');
                String[] Priority = PriorityList.Split(':');
                String[] FieldNames = FilterName.Split(':');
                String[] DataSourceName = DataSourceNames.Split(':');


                //Step 1 
                login.LoginIConnect(username, password);
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, DomainName, 1);
                role.SearchRole(RoleName);
                role.SelectRole(RoleName);
                role.ClickEditRole();
                role.SetCheckbox("cssselector", Locators.CssSelector.AllowStudyListSaveCheckboxRole);
                role.SelectAllListItems(By.Id(Locators.ID.SearchFieldSelector));
                role.Click("id", Locators.ID.ElementAddbutton);
                PageLoadWait.WaitForPageLoad(10);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(6);
                ExecutedSteps++;

                //Step 2 and 3
                ExecutedSteps++;

                study = (Studies)login.Navigate("Studies");
                if (study.CheckStudyListCount() == 0)
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

                //Step 4:
                study.SetStudyListLayout(StudyList, 1);
                int counter = 0;
                while (!study.CheckStudyListColumnNames(StudyList) && counter < 10)
                {
                    study.SetStudyListLayout(StudyList, 1);
                    counter++;
                }
                bool step4 = study.CheckStudyListColumnNames(StudyList);
                if (step4)
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

                //Step 5
                study.ResetStudyListLayout();
                bool step5 = study.CheckStudyListColumnNames(StudyList);
                if (!step5)
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
                //Step 6
                //Reset in domain mgmt
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                PageLoadWait.WaitForElement(By.XPath(Locators.Xpath.ResetColumnsDiv), BasePage.WaitTypes.Visible);
                domain.Click("xpath", Locators.Xpath.ResetColumnsDiv);
                //Get Default Column layout
                string[] DefaultLayout = domain.GetStudyListColumnNames();
                domain.ClickSaveEditDomain();
                //Reset in role
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SearchRole(RoleName);
                role.SelectRole(RoleName);
                role.ClickEditRole();
                PageLoadWait.WaitForElement(By.CssSelector(Locators.CssSelector.ToolbarUseDomainCheckboxRoleMgmt), BasePage.WaitTypes.Visible);
                role.SetCheckbox("cssselector", Locators.CssSelector.ToolbarUseDomainCheckboxRoleMgmt);
                role.SetCheckbox("cssselector", Locators.CssSelector.StudyListUseDomainRoleMgmt);
                role.ClickSaveEditRole();
                //navigate to studylst
                study = (Studies)login.Navigate("Studies");
                string[] test = study.GetStudyListColumnNames();

                bool step6 = study.CompareStringArrays(DefaultLayout, test);
                //step6 = DefaultLayout.Count() == study.GetCurrentStudyListLayout().Count() && DefaultLayout.All(q => study.GetCurrentStudyListLayout().Contains(q.Trim()));
                if (step6)
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
                //Step 7
                study.ClearFields();
                study.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", Search_Step7[0]);
                study.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", Search_Step7[1]);
                study.SearchStudy("Patient ID", Search_Step7[2]);
                PageLoadWait.WaitForLoadingMessage(30);
                counter = 0;
                string[] step7list = new string[2] { "Patient Name", "Study Date" };
                while (!study.CheckStudyListColumnNames(step7list) && counter < 10)
                {
                    study.SetStudyListLayout(step7list, 1);
                    counter++;
                }
                string[] step7 = study.GetStudyDetails("Patient Name");
                bool step7_res = (step7 == null || step7.Length == 0) ? false : step7.Where(q => q.ToLower().Contains(StudyName.ToLower())).Count() == step7.Length;
                if (step7_res)
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

                //Step 8
                study.ClearFields();
                counter = 0;
                study.SetStudyListLayout(FieldNames, 1);
                while (!study.CheckStudyListColumnNames(FieldNames) && counter < 10)
                {
                    study.SetStudyListLayout(FieldNames, 1);
                    counter++;
                }
                study.SetText("id", "m_studySearchControl_m_patientIPID", Search_Step7[3]);
                study.SetText("id", "m_studySearchControl_PatientDOB", Search_Step7[5]);
                //study.SetText("id", "m_studySearchControl_PatientDOB", Search_Step7[5]);
                study.SearchStudy("mod", Search_Step7[4]);
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step8_res1 = study.GetStudyDetails("Issuer of PID");
                string[] step8_res2 = study.GetStudyDetails("Patient DOB");
                bool step8_1 = (step8_res1 == null || step8_res1.Length == 0) ? false : step8_res1.Where(q => q.ToLower().Contains(Search_Step7[3].ToLower())).Count() == step8_res1.Length;
                bool step8_2 = (step8_res2 == null || step8_res2.Length == 0) ? false : step8_res2.Where(q => q.ToLower().Contains(Search_Step7[5].ToLower())).Count() == step8_res2.Length;
                if (step8_1 && step8_2)
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

                //Step 9
                study.ClearFields();
                study.SearchStudy(Search_Step7[6], "", "", "", "", "", Search_Step7[7], "");
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step9_res1 = study.GetStudyDetails("Patient Name");
                string[] step9_res2 = study.GetStudyDetails("Study Date");
                bool step9_1 = (step9_res1 == null || step9_res1.Length == 0) ? false : step9_res1.Where(q => q.ToLower().Contains(Search_Step7[6].ToLower())).Count() == step9_res1.Length;
                bool step9_2 = (step9_res2 == null || step9_res2.Length == 0) ? false : step9_res2.Where(q => q.ToLower().Contains(StudyYear.ToLower())).Count() == step9_res2.Length;
                if (step9_1 && step9_2)
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

                //Step 10
                //Save preset & perform blank search
                study.SavePreset(PresetNames);
                study.ClearFields();
                study.Click("id", "m_studySearchControl_m_searchButton");
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForSearchLoad();
                // Select preset and search to check if preset is working correctly
                study.SelectFromList("id", Locators.ID.PresetDropdown, PresetNames, 1);
                //study.Click("id", "m_studySearchControl_m_searchButton");
                study.SearchStudy("first", "");
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForSearchLoad();
                string[] step10_res1 = study.GetStudyDetails("Patient Name");
                bool step10_1 = (step10_res1 == null || step10_res1.Length == 0) ? false : step10_res1.Where(q => q.ToLower().Contains(Search_Step7[6].ToLower())).Count() == step10_res1.Length;
                if (step10_1)
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
                //Step 11
                study.SelectFromList("id", Locators.ID.GroupByDropdown, DefaultStudyList[1], 1);
                string[] step11 = study.GetGroupByStudyListHeading();
                bool step11_1 = step11.Where(q => q.ToLower().Contains(Search_Step7[10].ToLower())).Count() == step11.Length;
                if (step11_1)
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
                //Step 12
                study.SelectFromList("id", Locators.ID.GroupByDropdown, "No Grouping", 1);
                study.ClearFields();
                study.SearchStudy("Last Name", "*");
                PageLoadWait.WaitForLoadingMessage(30);
                study.ClickColumnHeading(study.GetStudyListColumnNames()[0]);
                study.Click("id", "gridTableStudyList_patName");
                string[] step12 = study.GetStudyDetails("Patient Name");
                //CHeck the order
                bool step12_res = (step12 == null || step12.Length == 0) ? false : step12.SequenceEqual((step12.OrderBy(q => q)));
                if (step12_res)
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

                //Step 13
                //Get current list dynamically and do drag and drop
                DefaultLayout = study.GetStudyListColumnNames();
                IWebElement target = study.GetElement("id", "jqgh_" + study.GetStudyListColumnID(DefaultLayout[DefaultLayout.Length - 2].Trim()));
                IWebElement source = study.GetElement("id", "jqgh_" + study.GetStudyListColumnID(DefaultLayout[DefaultLayout.Length - 1].Trim()));

                //study.JSDragandDrop("jqgh_gridTableStudyList_patientDOB", "jqgh_gridTableStudyList_pidIssuer");
                study.ActionsDragAndDrop(source, target);

                //action.Perform();
                Thread.Sleep(2000);
                //login.Navigate("DomainManagement");
                //study = (Studies)login.Navigate("Studies");
                string[] step13 = study.GetStudyListColumnNames();
                int step13_1 = study.GetStringIndex(step13, DefaultLayout[DefaultLayout.Length - 1]);
                int step13_2 = study.GetStringIndex(step13, DefaultLayout[DefaultLayout.Length - 2]);
                if (step13_1 < step13_2)
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
                //Step 14
                study.ClearFields();
                //study.SelectDataSource(DataSourceName[0]);
                study.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", Search_Step7[8]);
                study.SearchStudy("Last Name", Search_Step7[9]);
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step14 = study.GetStudyDetails("Patient Name");
                bool step14_res = (step14 == null || step14.Length == 0) ? false : step14.Where(q => q.ToLower().Contains(Search_Step7[8].ToLower())).Count() == step14.Length;
                if (step14_res)
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
                //Step 15
                login.Logout();
                login.LoginIConnect(User2, User2);
                ExecutedSteps++;
                //Step 16
                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                //study.SearchStudy("Last Name", "");
                string[] DSName = new string[1] { "Data Source" };
                counter = 0;
                study.SetStudyListLayout(DSName, 1);
                while (!study.CheckStudyListColumnNames(DSName) && counter < 10)
                {
                    study.SetStudyListLayout(DSName, 1);
                    counter++;
                }

                //Search Study
                study.SearchStudy("LastName", "");

                //PageLoadWait.WaitForPageLoad(3);
                //study.Click("id", "m_studySearchControl_m_searchButton");
                ////Click not happening in batch run so giving workaround:
                //Thread.Sleep(1000);
                //study.Click("id", "m_studySearchControl_m_searchButton");
                //BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchButton")).Click();
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step16 = study.GetStudyDetails("Data Source");//config.rdm
                //bool step16_res = (step16 == null || step16.Length == 0) ? false : DataSourceName.Where(z => step16.Any(q => q.ToLower().Contains(z.ToLower()))).Count() == DataSourceName.Length && step16.Where(z => DataSourceName.Any(q => z.ToLower().Contains(q.ToLower()))).Count() == step16.Length;
                bool step16_res = step16.All(datasource => datasource.Contains(DataSourceNames[0]) || datasource.Contains(DataSourceNames[1]));
                if (step16_res)
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
                //Step 17
                study.ClearFields();
                study.SearchStudy("Last Name", "a");
                PageLoadWait.WaitForLoadingMessage(30);
                string[] step17 = study.GetStudyDetails("Patient Name");
                bool step17_res = (step17 == null || step17.Length == 0) ? false : step17.Where(q => q.ToLower().StartsWith("a")).Count() == step17.Length;
                if (step17_res)
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
                //Step 18
                login.Logout();
                ExecutedSteps++;
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Test case number 27828 - Viewer and Lossy/Lossless check
        /// </summary>
        public TestCaseResult Test_27828(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables          
            Studies studies = null;
            studies = new Studies();
            DomainManagement domainmanagement;

            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Studies study;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            //Login firefox;
            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;

                //Fetch the data
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] Patient = PatientIDList.Split(':');

                String ToolsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ToolsList");
                String PresetNamesList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PresetNames");
                String[] PresetNames = PresetNamesList.Split(':');
                String lastnameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] lastname = lastnameList.Split(':');
                String firstnameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] firstname = firstnameList.Split(':');
                String modalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] modality = modalityList.Split(':');
                String descriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String[] description = descriptionList.Split(':');
                String EA1 = login.GetHostName(Config.EA1);

                //Step 1 -
                //precondition - Only instructions, hence not automated
                ExecutedSteps++;

                //Step 2
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();

                domainmanagement.AddPreset("CR", PresetNames[0], "1200", "-20", "2x3");
                domainmanagement.AddPreset("CR", PresetNames[1], "900", "900", "2x3");
                domainmanagement.AddPreset("MR", PresetNames[2], "1", "100", "2x2");
                domainmanagement.AddPreset("MR", PresetNames[3], "250", "-70", "2x2");

                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(5);

                //Enabling Email Study in Role
                role = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                role.DomainDropDown().SelectByValue(DomainName);
                role.SearchRole("SuperRole");
                role.SelectRole("SuperRole");
                role.EditRoleBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                role.SetCheckboxInEditRole("email", 0);
                role.ClickSaveEditRole();

                ExecutedSteps++;

                //Step 3
                login.Logout();
                login.LoginIConnect(User1, User1);
                login.Navigate("Studies");
                login.ClearFields();
                //login.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", lastname[0]);
                //login.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", firstname[0]);

                login.SearchStudy(Modality: modality[0], LastName: lastname[0], FirstName: firstname[0], Description: description[0], Datasource: EA1);
                PageLoadWait.WaitForLoadingMessage(30);
                int counter = 0;
                string[] step3list = new string[1] { "Description" };
                while (!login.CheckStudyListColumnNames(step3list) && counter < 10)
                {
                    login.SetStudyListLayout(step3list, 1);
                    counter++;
                }
                login.SelectStudy("Description", description[0]);
                login.LaunchStudy(2);
                //PageLoadWait.WaitForFrameLoad(10);
                //PageLoadWait.WaitForPageLoad(40);
                //IWebElement viewstudy = null;
                //try { viewstudy = BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton")); }
                //catch (NoSuchElementException) { viewstudy = new IntegratorStudies().Intgr_ViewBtn(); }
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", viewstudy);
                //Logger.Instance.InfoLog("View Study button clicked.");
                //PageLoadWait.WaitForPageLoad(10);
                //PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);

                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step3 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4
                ExecutedSteps++;

                //Step 5
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step5 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6
                if (studyviewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80"))
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

                //Step 7
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step7 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step7)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8
                bool step8 = studyviewer.windowPresetStatus();
                if (step8)
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

                //Step 9
                var element = login.GetElement("id", login.GetControlId("SeriesViewer1-1X1"));
                int h1 = element.Size.Height;
                int w1 = element.Size.Width;

                login.Click("id", login.GetControlId("SeriesViewer1-1X1"));

                studyviewer.DrawLine(element, w1 / 2, h1 / 2);

                //Get detail of Viewport
                String ZoomseriesUID_1 = studyviewer.GetInnerAttribute(login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")), "src", '&', "seriesUID");

                //Perform Zoom
                studyviewer.SelectToolInToolBar("Zoom");
                studyviewer.DragMovement(login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")));

                //Get detail of Viewport
                String ZoomseriesUID_2 = studyviewer.GetInnerAttribute(login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")), "src", '&', "seriesUID");

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool ZoomImage = studies.CompareImage(result.steps[ExecutedSteps], login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")));

                //Perform Zoom
                if (ZoomImage)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 11
                studyviewer.CloseStudy();

                login.ClearText("id", "m_studySearchControl_m_searchInputPatientLastName");
                login.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", lastname[0]);
                login.ClearText("id", "m_studySearchControl_m_searchInputPatientFirstName");
                login.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", firstname[0]);

                login.SearchStudy("modality", modality[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Description" });
                login.SelectStudy("Description", description[0]);

                login.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step11 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step11)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12
                login.Click("id", login.GetControlId("SeriesViewer1-1X1"));

                studyviewer.selectPreset("CR1:1200/-20");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step12 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step12)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13
                login.Click("id", login.GetControlId("SeriesViewer1-1X1"));

                studyviewer.selectPreset("CR2:900/900");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step13 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step13)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14
                studyviewer.NavigateToHistoryPanel();
                ExecutedSteps++;

                //Step 15
                IWebElement elementRecord = studyviewer.GetElement("xpath", "//table[@id='gridTablePatientHistory']/tbody/tr[3]");
                var action = new Actions(BasePage.Driver);
                if (elementRecord != null)
                {
                    action.DoubleClick(elementRecord).Build().Perform();
                }
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                Thread.Sleep(5000);

                var viewer2 = studyviewer.GetElement("id", "studyPanelDiv_2");
                if (viewer2 != null)
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

                //Step 16
                element = login.GetElement("id", login.GetControlId("2SeriesViewer1-2X2"));
                element.Click();

                login.ClickElement("Series Viewer 2x2");
                login.ClickElement("Series Scope");

                studyviewer.DrawLine(element, element.Size.Width / 2, element.Size.Height / 2);
                studyviewer.PerformWindowLevel(element);

                login.ClickElement("Image Scope");

                studyviewer.PerformPan(element);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(login.GetControlId("2SeriesViewer1-2X2")));
                bool step16 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step16)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17
                studyviewer.CloseStudy();

                login.ClearText("id", "m_studySearchControl_m_searchInputPatientLastName");
                login.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", lastname[0]);
                login.ClearText("id", "m_studySearchControl_m_searchInputPatientFirstName");
                login.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", firstname[0]);

                login.SearchStudy("modality", modality[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Description" });
                login.SelectStudy("Description", description[0]);

                login.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step17 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step17)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18
                login.ClickElement("Email Study");
                login.SetText("id", "EmailStudyControl_m_emailToTextBox", "shalaka.shivtarkar@citiustech.com");
                login.SetText("id", "EmailStudyControl_m_nameToTextBox", "SanityStep-18");
                login.SetText("id", "EmailStudyControl_m_reasonToTextBox", "Automation");
                login.Click("id", "EmailStudyControl_SendStudy");
                if (login.GetText("id", "EmailStudyControl_PinCode_Label") != null)
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

                login.Click("xpath", "//*[@id='PinCodeDialogDiv']/div[1]/span[1]");
                login.Click("xpath", "//*[@id='EmailStudyDialogDiv']/div[1]/span[1]");
                studyviewer.CloseStudy();

                //Steps:-19 and 20 Not Automated Steps              
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 21
                login.Navigate("Studies");
                //login.ClearText("id", "m_studySearchControl_m_searchInputPatientLastName");
                //login.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", lastname[1]);
                //login.ClearText("id", "m_studySearchControl_m_searchInputPatientFirstName");
                //login.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", firstname[1]);

                login.SearchStudy(LastName: lastname[1], FirstName: firstname[1], Datasource: EA1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Description" });
                login.SelectStudy("Description", description[1]);

                login.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step21 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step21)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 22
                ExecutedSteps++;

                //Step 23
                Thread.Sleep(4000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step23 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step23)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24
                if (studyviewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80"))
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

                //Step 25
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step25 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step25)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26
                //studyviewer.DragThumbnailToViewport(6, Locators.ID.SeriesViewer1_1x1);
                Thread.Sleep(2000);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                bool step26 = studyviewer.windowPresetStatus();
                if (step26)
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

                //Step 27
                element = login.GetElement("id", login.GetControlId("SeriesViewer1-1X1"));
                h1 = element.Size.Height;
                w1 = element.Size.Width;

                login.Click("id", login.GetControlId("SeriesViewer1-1X1"));

                studyviewer.DrawLine(element, w1 / 2, h1 / 2);
                //Get detail of Viewport
                ZoomseriesUID_1 = studyviewer.GetInnerAttribute(login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")), "src", '&', "seriesUID");

                //Perform Zoom
                studyviewer.SelectToolInToolBar("Zoom");
                studyviewer.DragMovement(login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")));

                //Get detail of Viewport
                ZoomseriesUID_2 = studyviewer.GetInnerAttribute(login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")), "src", '&', "seriesUID");

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                ZoomImage = studies.CompareImage(result.steps[ExecutedSteps], login.GetElement("id", login.GetControlId("SeriesViewer1-1X1")));

                //Perform Zoom
                if (ZoomImage)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 28
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 29
                studyviewer.CloseStudy();

                //login.ClearText("id", "m_studySearchControl_m_searchInputPatientLastName");
                //login.SetText("id", "m_studySearchControl_m_searchInputPatientLastName", lastname[1]);
                //login.ClearText("id", "m_studySearchControl_m_searchInputPatientFirstName");
                //login.SetText("id", "m_studySearchControl_m_searchInputPatientFirstName", firstname[1]);
                login.Navigate("Studies");
                login.ClearFields();
                login.SearchStudy(LastName: lastname[1], FirstName: firstname[1], Datasource: EA1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Description" });
                login.SelectStudy("Description", description[1]);

                login.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.Allviewports));
                bool step29 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step29)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 30
                login.Click("id", login.GetControlId("SeriesViewer1-1X1"));

                studyviewer.selectPreset("MR1:1/100");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step30 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step30)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 31
                login.Click("id", login.GetControlId("SeriesViewer1-1X1"));

                studyviewer.selectPreset("MR2:250/-70");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_1x1));
                bool step31 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step31)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 32
                login.Click("id", Locators.ID.SeriesViewer1_2X2);
                //Apply auto window level
                login.ClickElement("Auto Window Level");
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step32 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step32)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 33
                login.Click("id", Locators.ID.SeriesViewer1_2X2);
                studyviewer.selectPreset("MR1:1/100");
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step33 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step33)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.CloseStudy();
                //Step 34
                //login.LoginIConnect(username, password);
                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                study.SearchStudy(patientID: Patient[0], Datasource: EA1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Patient ID" });
                study.SelectStudy("Patient ID", Patient[0]);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                IWebElement TargetElement = study.GetElement("id", Locators.ID.SeriesViewer1_2X2);
                IWebElement SourceElement = study.GetElement("Xpath", Locators.Xpath.MRThumbnailDiv);
                action = new Actions(BasePage.Driver);
                action.DragAndDrop(SourceElement, TargetElement).Build().Perform();
                PageLoadWait.WaitForPageLoad(5);
                study.Click("id", Locators.ID.SeriesViewer1_2X2);
                study.ClickElement("Series Viewer 1x1");
                study.ClickElement("Series Scope");
                viewer.ClickElement("Image Layout 3x3");

                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step34 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step34)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 35
                study.Click("id", Locators.ID.SeriesViewer1_2X2);
                //Invert
                study.ClickElement("Invert");
                //WW
                element = study.GetElement("id", study.GetControlId("SeriesViewer1-2X2"));
                IWebElement viewportload = study.GetElement("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel");
                action = new Actions(BasePage.Driver);

                int h = 0;
                int w = 0;

                if (element != null)
                {
                    h = element.Size.Height;
                    w = element.Size.Width;

                    study.ClickElement("Window Level");
                    int n = 0;
                    while (n < 2)
                    {
                        action.MoveToElement(element, w - (w / 6), h - (h / 8)).ClickAndHold().MoveToElement(element, w - (w / 3), h - (h / 3)).Build().Perform();
                        Thread.Sleep(1000);
                        action.Release().Build().Perform();
                        PageLoadWait.WaitForLoadInViewport(3, viewportload);
                        n++;
                    }
                }
                //Pan
                element = study.GetElement("id", study.GetControlId("SeriesViewer1-2X2"));
                study.ClickElement("Pan");
                var action1 = new Actions(BasePage.Driver);
                study.Click("id", study.GetControlId("SeriesViewer1-2X2"));

                int j = 0;
                while (j < 2)
                {
                    action1.MoveToElement(element, w / 2, h / 2)
                           .ClickAndHold()
                           .MoveToElement(element, w / 2, h / 3)
                           .Build()
                           .Perform();
                    Thread.Sleep(1000);
                    action1.Release().Build().Perform();
                    PageLoadWait.WaitForLoadInViewport(3, viewportload);
                    j++;
                }
                //Zoom
                study.ClickElement("Zoom");
                PageLoadWait.WaitForLoadInViewport(3, viewportload);
                if (element != null)
                {
                    int i = 0;
                    while (i < 2)
                    {
                        action.MoveToElement(element, w / 6, h / 8).ClickAndHold().MoveToElement(element, w / 2, h - (h / 6)).Build().Perform();
                        Thread.Sleep(1000);
                        action.Release().Build().Perform();
                        Thread.Sleep(1000);
                        PageLoadWait.WaitForLoadInViewport(3, viewportload);
                        i++;
                    }
                }
                PageLoadWait.WaitForLoadInViewport(3, viewportload);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step35 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step35)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 36
                study.Click("id", study.GetControlId("SeriesViewer1-2X2"));
                study.ClickElement("Reset");
                PageLoadWait.WaitForPageLoad(4);
                viewer.ClickScrollDown("id", Locators.ID.ScrollNext1_1X1, 1);
                Thread.Sleep(2000);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step36 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step36)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 37
                study.Click("id", Locators.ID.SeriesViewer1_2X2);
                study.ClickElement("Image Scope");
                viewer.ClickElement("Image Layout 2x2");
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step37 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step37)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 38
                viewer.ClickScrollDown("id", Locators.ID.ScrollNext1_1X1, 1);
                study.ClickElement("Rotate Clockwise");
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step38 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step38)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 39,40 - Save series and reload
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 41 - Not automated since we cannot record the WW/WL values
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step 42
                study.ClickElement("Reset");
                PageLoadWait.WaitForPageLoad(4);
                viewer.ClickScrollDown("id", Locators.ID.ScrollNext1_1X1, 1);
                Thread.Sleep(2000);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step42 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step42)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 43
                study.Click("id", Locators.ID.SeriesViewer1_2X2);
                IWebElement element43 = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel"));
                viewportload = study.GetElement("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel");
                study.ClickElement("Global Stack");
                PageLoadWait.WaitForLoadInViewport(15, viewportload);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step43 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step43)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 44
                viewer.ClickScrollDown("id", Locators.ID.ScrollNext1_1X1, 6);
                Thread.Sleep(3000);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step44 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step44)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 45
                study.Click("id", Locators.ID.SeriesViewer1_2X2);
                viewer.ClickElement("Series Viewer 1x2");
                IWebElement element45 = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel"));
                study.ClickElement("Global Stack");
                PageLoadWait.WaitForLoadInViewport(15, viewportload);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.SeriesViewer1_2X2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step45 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step45)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                study.CloseStudy();
                //Step 46
                login.Logout();
                ExecutedSteps++;
                //Step 47
                login.LoginIConnect(username, password);
                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                string[] step47list = new string[1] { "Patient ID" };
                counter = 0;
                while (!login.CheckStudyListColumnNames(step47list) && counter < 10)
                {
                    login.SetStudyListLayout(step47list, 1);
                    counter++;
                }
                login.SearchStudy(LastName: lastname[1], FirstName: firstname[1], Datasource: EA1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Description" });
                login.SelectStudy("Description", description[1]);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step47 = study.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step47)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 48
                viewer.ClickElement("User Preference");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_UserprefFrame");
                if (study.GetElement("id", Locators.ID.UserPrefImageFormatJPG).Selected)
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
                //STep 49
                //Set PNG as Image format
                study.SetRadioButton("id", Locators.ID.UserPrefImageFormatPNG);
                //Set ROle as default page
                study.SetRadioButton("cssselector", Locators.CssSelector.UserPrefStartPageRole);
                study.Click("id", Locators.ID.UserPrefSaveButton);
                PageLoadWait.WaitForElement(By.Id("CloseResultButton"), BasePage.WaitTypes.Visible);
                study.Click("id", "CloseResultButton");
                //PageLoadWait.WaitForElement(By.Id(Locators.ID.UserPreferenceDiv), BasePage.WaitTypes.Invisible);
                study.CloseStudy();
                login.Logout();
                login.LoginIConnect(username, password);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                bool step49_1 = login.VerifyElementPresence("id", Locators.ID.RoleSearchTextBox);
                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                study.SearchStudy(patientID: Patient[0], Datasource: EA1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Patient ID" });
                study.SelectStudy("Patient ID", Patient[0]);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(30);
                bool step49_2 = login.VerifyElementPresence("id", Locators.ID.LossyCompressedDiv);
                if (step49_1 && !step49_2)
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

                //Step 50
                IWebElement element50 = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel"));
                viewportload = study.GetElement("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_SeriesViewer_ImagesPanel");
                viewer.DrawTextAnnotation(element50, 150, 150, By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox"), "test");
                PageLoadWait.WaitForLoadInViewport(5, viewportload);
                bool step50 = login.VerifyElementPresence("id", Locators.ID.LossyCompressedDiv);
                if (!step50)
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

                //Step 51
                string[] WindowHandles = viewer.OpenPrintViewandSwitchtoIT();
                bool step51 = viewer.VerifyElementPresence("id", Locators.ID.LossyCompressedTextPrint);
                if (!step51)
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

                //Step 52
                viewer.ClosePrintView(WindowHandles[1], WindowHandles[0]);
                viewer.ClickElement("User Preference");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_UserprefFrame");

                //Set Jpg as Image format
                study.SetRadioButton("id", Locators.ID.UserPrefImageFormatJPG);

                study.Click("id", Locators.ID.UserPrefSaveButton);
                PageLoadWait.WaitForElement(By.Id("CloseResultButton"), BasePage.WaitTypes.Visible);
                study.Click("id", "CloseResultButton");
                PageLoadWait.WaitForFrameLoad(5);
                study.CloseStudy();

                study = (Studies)login.Navigate("Studies");
                study.ClearFields();
                study.SearchStudy(patientID: Patient[0], Datasource: EA1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.ChooseColumns(new string[] { "Patient ID" });
                study.SelectStudy("Patient ID", Patient[0]);
                study.LaunchStudy();
                bool step52 = login.VerifyElementPresence("id", Locators.ID.LossyCompressedDiv);
                if (step52)
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                //Re-Enabling Jpg if there was any failure in above steps
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                //Open User Preference
                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                login.SetRadioButton("id", Locators.ID.UserPrefImageFormatJPG);
                login.CloseUserPreferences();
                login.Logout();
            }


        }

        /// <summary>
        /// Patient Record Search (EMPI enabled)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27829(String testid, String teststeps, int stepcount)
        {
            DomainManagement domainmanagement;
            Patients patients;
            patients = new Patients();

            TestCaseResult result;

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFirstName");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFullName");
                String[] patientname = PatientName.Split(':');
                String ErrorMessage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ErrorMessage");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] name = Name.Split(':');
                String ExpectedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ExpectedData");
                String[] expecteddata = ExpectedData.Split(':');
                String DataSent = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSent");
                String[] dataSent = DataSent.Split(':');
                String DataSourceManagement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceManagement");
                String XdsConfiguration = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "XdsConfiguration");
                String ResourceConfiguration = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ResourceConfiguration");

                //Step-1
                //Initial Set-up is done
                try
                {
                    //Adding Data Source details to "C:\WebAccess\WebAccess\Config\DataSource\DataSourceManagement.xml" file
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Config.DSManagerFilePath);
                    XmlDocumentFragment xfrag = doc.CreateDocumentFragment();
                    xfrag.InnerXml = DataSourceManagement;
                    doc.DocumentElement.AppendChild(xfrag);
                    doc.Save(Config.DSManagerFilePath);
                    // Adding XDS Configuration details to "C:\WebAccess\WebAccess\Config\Xds\XdsConfiguration.xml" file
                    doc.Load(Config.XDSConfigFilePath);
                    doc.RemoveAll();
                    doc.InnerXml = XdsConfiguration;
                    doc.Save(Config.XDSConfigFilePath);
                    //Adding XDS entry in "C:\WebAccess\WebAccess\Config\ResourceConfiguration.xml" file
                    doc.Load(Config.ResourceConfigFilePath);
                    xfrag.InnerXml = ResourceConfiguration;
                    XmlElement xmlEle = doc.DocumentElement["PatientIdDomains"];
                    xmlEle.AppendChild(xfrag);
                    doc.Save(Config.ResourceConfigFilePath);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error Occurred in Initial Setup due to: " + ex.Message);
                    //throw ex;
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2
                //pre-codition
                patients.RestartIISUsingexe();
                ExecutedSteps++;

                //Step-3
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain(DomainName);

                //Click Edit in DomainManagement Tab
                domainmanagement.ClickEditDomain();
                domainmanagement.ConnectAllDatasourcesEditDomain();
                domainmanagement.ClickSaveDomain();

                //Open User Preference
                patients.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                patients.SetRadioButton("id", "PRLiveSearchCB");
                patients.CloseUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (BasePage.Driver.FindElement(By.CssSelector("#m_domainSearchControl_m_searchButton")).Displayed)
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

                BasePage.Driver.SwitchTo().DefaultContent();
                login.Logout();

                //Step-4
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);
                patients = (Patients)login.Navigate("Patients");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (BasePage.Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchLabel")).Text.Equals("Find a patient"))
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

                //Step-5
                patients.InputData(dataSent[0]);
                PageLoadWait.WaitForPageLoad(2);
                if (patients.PatientExists(name[0]))
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



                //Step-6
                patients = (Patients)login.Navigate("Patients");
                patients.InputData(dataSent[2]);
                patients.InputData(dataSent[2]);
                PageLoadWait.WaitForPageLoad(2);
                if (patients.PatientExists(name[2]))
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

                //Step-7             
                patients.LoadStudyInPatientRecord(patientname[0]);
                patients.NavigateToXdsStudies();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForElement(By.Id("RadiologyStudiesListControl_parentGrid"), BasePage.WaitTypes.Visible);
                var res1 = patients.PatientExistsInLiveSearch(expecteddata[1]);
                var res2 = patients.PatientExistsInLiveSearch(expecteddata[2]);
                var res3 = patients.PatientExistsInLiveSearch(expecteddata[3]);
                var res4 = patients.PatientExistsInLiveSearch(expecteddata[4]);

                if (res1 && res2 && res3 && res4)
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

                patients.ClosePatientRecord();

                //Step-8
                patients = (Patients)login.Navigate("Patients");
                patients.InputData(dataSent[15]);
                PageLoadWait.WaitForPageLoad(2);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PatientExists(name[7]))
                //FOR JOHN--- if (patients.PatientExists(name[2]))
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

                //sTEP-9
                patients.LoadStudyInPatientRecord(patientname[1]);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForElement(By.Id("RadiologyStudiesListControl_parentGrid"), BasePage.WaitTypes.Visible);
                res1 = patients.PatientExistsInLiveSearch(expecteddata[8]);

                if (res1)
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

                patients.ClosePatientRecord();

                //Step-10
                patients = (Patients)login.Navigate("Patients");
                patients.InputData(dataSent[5]);
                patients.InputData(dataSent[5]);
                PageLoadWait.WaitForPageLoad(2);
                PageLoadWait.WaitForFrameLoad(2);
                PageLoadWait.WaitForElement(By.Id("gridTablePatientRecords"), BasePage.WaitTypes.Visible);
                if (patients.PatientExists(name[4]))
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

                //Step-11
                Thread.Sleep(6000);
                patients.LoadStudyInPatientRecord(patientname[2]);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForElement(By.Id("RadiologyStudiesListControl_parentGrid"), BasePage.WaitTypes.Visible);
                bool res5 = patients.PatientExistsInLiveSearch(expecteddata[5]);
                bool res6 = patients.PatientExistsInLiveSearch(expecteddata[6]);
                bool res7 = patients.PatientExistsInLiveSearch(expecteddata[7]);
                if (res5 && res6 && res7)
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


                patients.ClosePatientRecord();

                //Step-12
                patients = (Patients)login.Navigate("Patients");
                patients.InputData(dataSent[15]);
                PageLoadWait.WaitForPageLoad(2);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PatientExists(name[7]))
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

                //Step-13
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-14
                patients = (Patients)login.Navigate("Patients");
                patients.HoverElement(By.XPath("*[@id='ExpandSearchPanelButton']"));

                IWebElement elements = BasePage.Driver.FindElement(By.Id("ExpandSearchPanelButton"));

                if (elements.GetAttribute("title").Equals("Attribute based search"))
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

                //Step-15
                patients.Click("id", "ExpandSearchPanelButton");

                PageLoadWait.WaitForElement(By.Id("AdvancedSearchControl_PatientLastNameLabel"), BasePage.WaitTypes.Visible);
                if (BasePage.Driver.FindElement(By.XPath("//*[@id='AdvancedSearchControl_PatientLastNameLabel']")).Displayed)
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

                //step-16
                patients.AttributeSearch("date", dataSent[7]);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PatientExists(name[5]))
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

                //Step-17
                patients.ClickClear();
                patients.AttributeSearch("lastname", dataSent[8]);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PatientExists(name[6]))
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

                //Step-18
                patients.ClickClear();
                patients.AttributeSearch("firstname", dataSent[9]);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PatientExists(name[4]))
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

                //Step-19
                patients.ClickClear();
                patients.AttributeSearch("date", dataSent[10]);
                patients.AttributeSearch("lastname", dataSent[11]);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PatientExists(name[2]))
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

                //Step-20
                patients.ClickClear();
                patients.AttributeSearch("line", dataSent[12]);
                patients.AttributeSearch("firstname", dataSent[13]);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PatientExists(name[2]))
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

                //Step-21
                patients.ClickClear();
                if (BasePage.Driver.FindElement(By.CssSelector("#AdvancedSearchControl_FirstName")).Text.Equals(""))
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

                //Step-22
                string xmlfile = Config.FileLocationPath;
                if (File.Exists(xmlfile))
                {
                    XDocument doc1 = XDocument.Load(xmlfile);
                    XElement nodeToEdit = doc1.XPathSelectElement("Configuration/PatientRecord/ResultList/MaxRecords");
                    nodeToEdit.SetValue("1");

                    doc1.Save(xmlfile);
                }

                patients.RestartIISUsingexe();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                patients = (Patients)login.Navigate("Patients");
                PageLoadWait.WaitForElement(By.Id("ExpandSearchPanelButton"), BasePage.WaitTypes.Visible);
                patients.Click("id", "ExpandSearchPanelButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                patients.AttributeSearch("firstname", dataSent[14]);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                if (BasePage.Driver.FindElement(By.CssSelector("#PatientRecordGridControl1_m_messageLabel")).Text.Contains(ErrorMessage))
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

                //Step-23
                xmlfile = Config.FileLocationPath;
                if (File.Exists(xmlfile))
                {
                    XDocument doc2 = XDocument.Load(xmlfile);
                    XElement nodeToEdit = doc2.XPathSelectElement("Configuration/PatientRecord/ResultList/MaxRecords");
                    nodeToEdit.SetValue("200");

                    doc2.Save(xmlfile);
                }

                ExecutedSteps++;
                patients.RestartIISUsingexe();

                //Step-24

                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                patients = (Patients)login.Navigate("Patients");
                PageLoadWait.WaitForElement(By.Id("ExpandSearchPanelButton"), BasePage.WaitTypes.Visible);
                patients.Click("id", "ExpandSearchPanelButton");
                patients.AttributeSearch("firstname", dataSent[14]);
                PageLoadWait.WaitForPageLoad(2);
                PageLoadWait.WaitForFrameLoad(2);
                if (patients.PqaExists(dataSent[14]))
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Patient Record Loading/reviewing and Embedded Viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27830(String testid, String teststeps, int stepcount)
        {
            Viewer viewer = null;
            viewer = new Viewer();

            Patients patients;
            patients = new Patients();

            TestCaseResult result;

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFirstName");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] name = Name.Split(':');
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFullName");
                String[] patientname = PatientName.Split(':');
                String DataSent = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSent");
                String[] dataSent = DataSent.Split(':');

                //Step-1
                ExecutedSteps++;

                //Step 2
                //bitmap image for bob
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                patients = (Patients)login.Navigate("Patients");

                //For patient BOB-pqa
                patients.InputData(dataSent[0]);
                PageLoadWait.WaitForFrameLoad(5);
                patients.LoadStudyInPatientRecord(patientname[0]);
                PageLoadWait.WaitForPageLoad(5);

                patients.NavigateToXdsPatients();
                patients.NavigateToXsdDocumentsPatients();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                patients.Doubleclick("xpath", "//*[@id='XdsPageDocsGrid']/tbody/tr[6]/td[1]/span");
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");



                var element = viewer.GetElement("xpath", "//img [@title='Localizer Line']");

                var element1 = viewer.GetElement("xpath", "//img [@title='Save Series']");

                var element2 = viewer.GetElement("xpath", "//img [@title='Save Annotated Images']");
                bool value = false;

                if (element != null)
                {
                    var classText = element.GetAttribute("class");

                    if (!classText.Equals("notSelected32 enabledOnCine"))
                    {
                        value = true;
                    }

                    else
                    {
                        value = false;
                    }
                }

                if (element1 != null)
                {
                    var classText = element.GetAttribute("class");

                    if (!classText.Equals("notSelected32 enabledOnCine"))
                    {
                        value = true;
                    }

                    else
                    {
                        value = false;
                    }
                }

                if (element2 != null)
                {
                    var classText = element.GetAttribute("class");

                    if (!classText.Equals("notSelected32 enabledOnCine"))
                    {
                        value = true;
                    }

                    else
                    {
                        value = false;
                    }
                }

                if (value)
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

                PageLoadWait.WaitForPageLoad(5);

                //Step-3
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-4
                patients.ClickElement("Print View");
                //new window opens--valiadte it
                PageLoadWait.WaitForPageLoad(5);
                int timeout = 0;
                string ParentWindowID = BasePage.Driver.CurrentWindowHandle;

                while (BasePage.Driver.WindowHandles.Count == 1 && timeout < 5)
                {
                    patients.ClickElement("Print View");
                    PageLoadWait.WaitForFrameLoad(2);
                    timeout = timeout + 1;
                }

                if (BasePage.Driver.WindowHandles.Count > 1)
                {
                    string PreviewWindowID = BasePage.Driver.WindowHandles[0].Equals(ParentWindowID, StringComparison.InvariantCultureIgnoreCase)
                                                 ? BasePage.Driver.WindowHandles[1]
                                                 : BasePage.Driver.WindowHandles[0];

                    BasePage.Driver.SwitchTo().Window(PreviewWindowID);

                    var viewport2 = BasePage.Driver.FindElement(By.Id("SeriesViewersDiv"));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step27830_5 = patients.CompareImage(result.steps[ExecutedSteps], viewport2);

                    if (step27830_5)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    BasePage.Driver.Close();

                    BasePage.Driver.SwitchTo().Window(ParentWindowID);
                    BasePage.Driver.SwitchTo().DefaultContent();

                }



                //Step-5
                patients.CloseStudy();

                BasePage.Driver.SwitchTo().DefaultContent();

                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                if (BasePage.Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
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

                //Step-6
                //CT MR head image not found..so using john

                patients.CloseStudy();
                patients.ClosePatientRecord();
                patients = (Patients)login.Navigate("Patients");

                patients.InputData(dataSent[2]);
                PageLoadWait.WaitForFrameLoad(5);

                //For patient john-pqa
                patients.LoadStudyInPatientRecord(patientname[1]);
                PageLoadWait.WaitForPageLoad(5);

                patients.NavigateToXdsStudies();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForFrameLoad(10);
                patients.Doubleclick("xpath", "//*[@id='RadiologyStudiesListControl_parentGrid']/tbody/tr[2]/td[3]");

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_CompositeViewer_SeriesViewersDiv"));
                bool step27830_7 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27830_7)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7

                ReadOnlyCollection<IWebElement> elements = BasePage.Driver.FindElements(By.TagName("li"));
                bool step2 = false;
                foreach (IWebElement t in elements)
                {
                    if (t.GetAttribute("title").Equals("Localizer Line"))
                    {
                        step2 = true;
                        break;
                    }
                    else
                    {
                        step2 = false;
                    }
                }

                foreach (IWebElement t in elements)
                {
                    if (t.GetAttribute("title").Equals("Image Scope"))
                    {
                        step2 = true;
                        break;
                    }
                    else
                    {
                        step2 = false;
                    }
                }

                foreach (IWebElement t in elements)
                {
                    if (t.GetAttribute("title").Equals("Save Annotated Images"))
                    {
                        step2 = true;
                        break;
                    }
                    else
                    {
                        step2 = false;
                    }
                }

                foreach (IWebElement t in elements)
                {
                    if (t.GetAttribute("title").Equals("Save Series"))
                    {
                        step2 = true;
                        break;
                    }
                    else
                    {
                        step2 = false;
                    }
                }

                if (step2)
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

                //Step-8
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_thumbnailContent"));
                bool step27830_8 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27830_8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                patients.Click("id", login.GetControlId("SeriesViewer1-1X2"));

                element = patients.GetElement("id", login.GetControlId("SeriesViewer1-1X2"));

                if (element != null)
                {

                    patients.ClickElement("Zoom");

                    int h = element.Size.Height;
                    int w = element.Size.Width;

                    var action = new Actions(BasePage.Driver);

                    int i = 0;
                    if (IsHTML5)
                    {
                        while (i < 2)
                        {
                            action.MoveToElement(element, w / 9, h / 9)
                                  .ClickAndHold()
                                  .MoveToElement(element, w / 3, h / 3)
                                  .Build()
                                  .Perform();
                            Thread.Sleep(1500);
                            action.Release().Build().Perform();
                            i++;
                        }
                    }
                    else
                    {
                        while (i < 2)
                        {
                            action.MoveToElement(element, w / 6, h / 8)
                                  .ClickAndHold()
                                  .MoveToElement(element, w / 2, h - (h / 6))
                                  .Build()
                                  .Perform();
                            action.Release(element).Build().Perform();
                            i++;
                        }
                    }

                    PageLoadWait.WaitForFrameLoad(2);

                    patients.ClickElement("Pan");
                    int j = 0;
                    while (j < 1)
                    {
                        action.MoveToElement(element, w - (w / 6), h - (h / 8))
                              .ClickAndHold()
                              .MoveToElement(element, w - (w / 3), h - (h / 3))
                              .Build()
                              .Perform();
                        Thread.Sleep(2000);
                        action.Release().Build().Perform();
                        j++;
                    }
                }
                var viewport10 = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27830_10 = patients.CompareImage(result.steps[ExecutedSteps], viewport10);

                if (step27830_10)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10,11
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                patients.CloseStudy();

                //Step-12
                //jpeg
                patients.NavigateToXdsPatients();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                patients.Doubleclick("xpath", "//*[@id='XdsPageDocsGrid']/tbody/tr[3]/td[1]/span");

                BasePage.Driver.SwitchTo().DefaultContent();

                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                element = viewer.GetElement("xpath", "//img [@title='Localizer Line']");

                element1 = viewer.GetElement("xpath", "//img [@title='Save Series']");

                element2 = viewer.GetElement("xpath", "//img [@title='Save Annotated Images']");
                value = false;
                bool step12_1 = false, step12_2 = false, step12_3 = false;
                if (element != null)
                {
                    var classText = element.GetAttribute("class");

                    if (!classText.Equals("notSelected32 enabledOnCine"))
                    {
                        step12_1 = true;
                    }

                    else
                    {
                        step12_1 = false;
                    }
                }

                if (element1 != null)
                {
                    var classText = element.GetAttribute("class");

                    if (!classText.Equals("notSelected32 enabledOnCine"))
                    {
                        step12_2 = true;
                    }

                    else
                    {
                        step12_2 = false;
                    }
                }

                if (element2 != null)
                {
                    var classText = element.GetAttribute("class");

                    if (!classText.Equals("notSelected32 enabledOnCine"))
                    {
                        step12_3 = true;
                    }

                    else
                    {
                        step12_3 = false;
                    }
                }

                if (step12_1 && step12_2 && step12_3)
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

                //Step-13,14
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-15
                //png
                patients.CloseStudy();
                PageLoadWait.WaitForPageLoad(5);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");


                patients.Doubleclick("xpath", "//*[@id='XdsPageDocsGrid']/tbody/tr[14]/td[1]/span");

                BasePage.Driver.SwitchTo().DefaultContent();

                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                element = viewer.GetElement("xpath", "//img [@title='Print View']");
                if (element != null)
                {
                    var classText = element.GetAttribute("class");

                    if (classText.Equals("notSelected32 enabledOnCine"))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }

                    if (classText.Equals("notSelected32 enabledOnCine disableOnCine"))
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step-16
                //rotate not enabled
                result.steps[++ExecutedSteps].status = "Not Automated";


                //step-17

                patients.ClickElement("Print View");

                PageLoadWait.WaitForPageLoad(5);
                timeout = 0;
                ParentWindowID = BasePage.Driver.CurrentWindowHandle;

                while (BasePage.Driver.WindowHandles.Count == 1 && timeout < 5)
                {
                    patients.ClickElement("Print View");
                    PageLoadWait.WaitForFrameLoad(2);
                    timeout = timeout + 1;
                }

                if (BasePage.Driver.WindowHandles.Count > 1)
                {
                    string PreviewWindowID = BasePage.Driver.WindowHandles[0].Equals(ParentWindowID, StringComparison.InvariantCultureIgnoreCase)
                                                 ? BasePage.Driver.WindowHandles[1]
                                                 : BasePage.Driver.WindowHandles[0];

                    BasePage.Driver.SwitchTo().Window(PreviewWindowID);

                    var viewport17 = BasePage.Driver.FindElement(By.Id("SeriesViewersDiv"));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step27830_17 = patients.CompareImage(result.steps[ExecutedSteps], viewport17);

                    if (step27830_17)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    BasePage.Driver.Close();

                    BasePage.Driver.SwitchTo().Window(ParentWindowID);
                    BasePage.Driver.SwitchTo().DefaultContent();

                }




                //Step-18

                patients.CloseStudy();
                patients.ClosePatientRecord();
                patients = (Patients)login.Navigate("Patients");

                patients.InputData(dataSent[1]);
                PageLoadWait.WaitForFrameLoad(5);

                //For patient fred-pqa
                patients.LoadStudyInPatientRecord(patientname[2]);
                patients.NavigateToXsdDocumentsPatients();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                patients.Doubleclick("xpath", "//*[@id='XdsPageDocsGrid']/tbody/tr[15]/td[1]/span");
                PageLoadWait.WaitForPageLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();

                result.steps[++ExecutedSteps].status = "Not Automated";

                //test case needing update


                //Step-19
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-20
                patients.CloseStudy();

                BasePage.Driver.SwitchTo().DefaultContent();

                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                if (BasePage.Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
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



                //Step-21             
                patients.ClosePatientRecord();
                patients = (Patients)login.Navigate("Patients");

                patients.InputData(dataSent[2]);
                PageLoadWait.WaitForFrameLoad(5);

                patients.LoadStudyInPatientRecord(patientname[1]);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                patients.Doubleclick("xpath", "//*[@id='XdsPageDocsGrid']/tbody/tr[13]/td[1]/span");
                BasePage.Driver.SwitchTo().DefaultContent();

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-22

                patients.ClickElement("Full Screen");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");


                if (BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_patientInfoDiv")).Text.Contains(dataSent[2]))
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


                //Step-23
                patients.GetElement("id", "recallToolsDiv").Click();


                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                if (BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_thumbnailContent")).Displayed)
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

                patients.CloseStudy();

                //Step-24  
                patients.NavigateToXdsPatients();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patients.Doubleclick("xpath", "//*[@id='XdsPageDocsGrid']/tbody/tr[2]/td[1]/span");

                PageLoadWait.WaitForPageLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                element = viewer.GetElement("xpath", "//img [@title='Print View']");
                if (element != null)
                {
                    var classText = element.GetAttribute("class");

                    if (classText.Equals("notSelected32 enabledOnCine"))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }

                    if (classText.Equals("notSelected32 enabledOnCine disableOnCine"))
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }



                //Step-25
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-26
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-27
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-28.29
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-30              
                patients.ClickElement("Full Screen");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                if (BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_patientInfoDiv")).Text.Contains(dataSent[2]))
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

                //Step-31
                patients.GetElement("id", "recallToolsDiv").Click();
                patients.CloseStudy();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                if (BasePage.Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
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

                patients.ClosePatientRecord();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Hosted Integration Mode
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27832(String testid, String teststeps, int stepcount)
        {
            Viewer viewer = null;
            viewer = new Viewer();
            Patients patients = null;
            patients = new Patients();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Assembly assembly;
                //Type type;
                //object obj; 

                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Filepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilePath");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFirstName");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String ErrorMessage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ErrorMessage");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String[] name = Name.Split(':');
                String[] Description = DescriptionList.Split(':');
                String Link = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Link");
                String[] link = Link.Split(':');

                //String HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");

                String ExpectedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ExpectedData");
                String[] expecteddata = ExpectedData.Split(':');


                String DataSent = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSent");
                String[] dataSent = DataSent.Split(':');

                //Step-1
                //Initial Set-up is done
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.Integrator_Tab);
                wpfobject.WaitTillLoad();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL determined", shadowuser: "enable");
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.AllowShowSelector().Checked = true;
                servicetool.AllowShowSelectorSearch().Checked = true;
                servicetool.ApplyEnableFeatures();
                wpfobject.ClickOkPopUp();
                servicetool.EnableHTML5(false);
                //servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Disabling HTML5 from user preferences
                login.LoginIConnect(Username, Password);
                //Open User Preference
                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                login.SetRadioButton("id", "DefaultViewerSettingRadioButtonList_1");
                login.CloseUserPreferences();
                login.Logout();

                //Step2 - Copy EHR files                
                ExecutedSteps++;

                //Step 3
                login.UncommentXMLnode("id", "Bypass");
                ExecutedSteps++;

                //Step-4         
                login.NavigateToIntegratorURL("http://" + Config.IConnectIP + link[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_m_studySearchControl_m_searchInputPatientID")));
                if (patients.PatientExistsEHR(expecteddata[0]))
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

                //Step-5
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                patients.SetCheckbox("id", "ctl00_ctl05_m_dataListGrid_check_0_1"); ;
                patients.LaunchStudy();

                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"), BasePage.WaitTypes.Visible);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                var viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                bool step27832_5 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.CloseBrowser();
                patients.InvokeBrowser(Config.BrowserType);

                //Step-6             
                patients.NavigateToIntegratorURL("http://" + Config.IConnectIP + link[1]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(10);
                //PageLoadWait.WaitForElement(By.Id("Viewport_One_1_0"), BasePage.WaitTypes.Visible);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_1"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_6 = login.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_6)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.CloseBrowser();
                patients.InvokeBrowser(Config.BrowserType);

                //Step-7             
                patients.NavigateToIntegratorURL("http://" + Config.IConnectIP + link[5]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                //patients.SetCheckbox("id", "ctl00_ctl05_m_dataListGrid_check_0_1");
                //patients.SetCheckbox("id", "ctl00_ctl05_m_dataListGrid_check_1_1");
                //patients.SetCheckbox("id", "ctl00_ctl05_m_dataListGrid_check_2_1");


                //patients.ClickUrlViewStudyBtn();
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"), BasePage.WaitTypes.Visible);
                PageLoadWait.WaitForFrameLoad(10);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#Label1")).Text.Contains(ErrorMessage))
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

                //Step-8
                BasePage.Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForPageLoad(2);
                patients.SwitchTo("index", "0");
                PageLoadWait.WaitForFrameLoad(2);
                patients.ClickPatientHistoryTab();
                Thread.Sleep(3000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame");

                patients.SelectStudy("Modality", "MG");

                if (BasePage.Driver.FindElement(By.Id("m_patientHistory_currentStudyPatientIDTextBox")).Displayed)
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

                //step-9
                viewer.OpenPriors(new string[] { "Modality" }, new string[] { "MG" });
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");

                viewport = BasePage.Driver.FindElement(By.Id("studyPanelDiv_2"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_9_1 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_9_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.CloseBrowser();
                patients.InvokeBrowser(Config.BrowserType);


                //Step-10

                login.NavigateToIntegratorURL("http://" + Config.IConnectIP + link[3]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");


                if (patients.PatientExistsEHR(expecteddata[1]))
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

                //Step-11              
                patients.SetCheckbox("id", "ctl00_ctl05_m_dataListGrid_check_0_1");
                patients.SetCheckbox("id", "ctl00_ctl05_m_dataListGrid_check_1_1");
                //patients.SetCheckbox("id", "ctl00_ctl05_m_dataListGrid_check_3_1");


                patients.LaunchStudy();
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"), BasePage.WaitTypes.Visible);
                PageLoadWait.WaitForFrameLoad(10);

                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                //PageLoadWait.WaitForPageLoad(2);
                //patients.SwitchTo("index", "0");
                bool step11_text = BasePage.Driver.FindElement(By.CssSelector("#Label1")).Text.Contains(ErrorMessage);
                patients.NavigateToHistoryPanel();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame");


                //IWebElement historyPanel = BasePage.Driver.FindElement(By.CssSelector("div#patientHistoryDrawerContent"));//div[id='gridDivPatientHistory']
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //bool step27832_11 = patients.CompareImage(result.steps[ExecutedSteps], historyPanel);
                //viewer.ChooseColumns(new String[] { "Accession", "Data Source" });
                Dictionary<int, string[]> ResultsInHistoryPanel = BasePage.GetSearchResults();
                String[] ColumnValuesInHistoryPanel = BasePage.GetColumnValues(ResultsInHistoryPanel, "Study Description", BasePage.GetColumnNames());

                bool step11_value = (ColumnValuesInHistoryPanel == null || ColumnValuesInHistoryPanel.Length == 0) ? false : Description.Where(z => ColumnValuesInHistoryPanel.Any(q => q.ToLower().Contains(z.ToLower()))).Count() == Description.Length && ColumnValuesInHistoryPanel.Where(z => Description.Any(q => z.ToLower().Contains(q.ToLower()))).Count() == ColumnValuesInHistoryPanel.Length;
                if (!step11_text && step11_value)//Array.Exists(ColumnValuesInHistoryPanel, s => s.Equals(ColumnValuesInHistoryPanel[0])))
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


                //Step-12
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                //viewer.ChooseColumns(new String[] { "Accession"});
                Dictionary<int, string[]> SearchResults = BasePage.GetSearchResults();
                String[] ColumnValues = BasePage.GetColumnValues(SearchResults, "Study Date", BasePage.GetColumnNames());
                for (int i = 1; i < ColumnValues.Length; i++)
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    viewer.OpenPriors(new string[] { "Study Date" }, new string[] { ColumnValues[i] });
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                    if (i == ColumnValues.Length - 1) { break; }
                    patients.ClickPatientHistoryTab();
                    PageLoadWait.WaitForPageLoad(20);
                }

                //viewer.Doubleclick("xpath", "//*[@id='2']/td[4]");
                //PageLoadWait.WaitForFrameLoad(10);
                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                //PageLoadWait.WaitForPageLoad(2);
                //patients.SwitchTo("index", "0");
                //patients.ClickPatientHistoryTab();
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame");
                //viewer.Doubleclick("xpath", "//*[@id='3']/td[4]");
                //PageLoadWait.WaitForFrameLoad(10);
                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");

                viewport = BasePage.Driver.FindElement(By.Id("StudyPanelContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_12 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_12)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-13
                if (IsHTML5)
                {
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                    patients.ClickElement("Print View");
                    //new window opens
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                    patients.Click("xpath", "/html/body/div[2]/div[1]/a/span");

                    viewport = BasePage.Driver.FindElement(By.Id("image-dialog"));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step27832_13 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                    if (step27832_13)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                else
                {
                    int timeout = 0;
                    string ParentWindowID = BasePage.Driver.CurrentWindowHandle;

                    while (BasePage.Driver.WindowHandles.Count == 1 && timeout < 5)
                    {
                        patients.ClickElement("Print View");
                        PageLoadWait.WaitForFrameLoad(2);
                        timeout = timeout + 1;
                    }

                    if (BasePage.Driver.WindowHandles.Count > 1)
                    {
                        string PreviewWindowID = BasePage.Driver.WindowHandles[0].Equals(ParentWindowID,
                                                                                                 StringComparison
                                                                                                     .InvariantCultureIgnoreCase)
                                                     ? BasePage.Driver.WindowHandles[1]
                                                     : BasePage.Driver.WindowHandles[0];

                        BasePage.Driver.SwitchTo().Window(PreviewWindowID);

                        var viewport8 = BasePage.Driver.FindElement(By.Id("SeriesViewersDiv"));
                        result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                        bool step27832_13 = patients.CompareImage(result.steps[ExecutedSteps], viewport8);

                        if (step27832_13)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        BasePage.Driver.Close();

                        BasePage.Driver.SwitchTo().Window(ParentWindowID);
                        BasePage.Driver.SwitchTo().DefaultContent();

                    }
                }

                //Step-14
                result.steps[++ExecutedSteps].status = "Not Automated";

                login.CloseBrowser();
                patients.InvokeBrowser(Config.BrowserType);

                //    Step-15
                patients.NavigateToIntegratorURL("http://" + Config.IConnectIP + link[4]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"), BasePage.WaitTypes.Visible);
                PageLoadWait.WaitForFrameLoad(10);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");

                patients.Click("id", login.GetControlId("SeriesViewer2-1X2"));

                var element = patients.GetElement("id", login.GetControlId("SeriesViewer2-1X2"));

                if (element != null)
                {
                    patients.ClickElement("Image Layout 2x2");
                    PageLoadWait.WaitForFrameLoad(5);


                    patients.ClickElement("Image Scope");
                    patients.ClickElement("Zoom");

                    int h = element.Size.Height;
                    int w = element.Size.Width;

                    var action = new Actions(BasePage.Driver);

                    int i = 0;
                    if (IsHTML5)
                    {
                        while (i < 2)
                        {
                            action.MoveToElement(element, w / 9, h / 9)
                                  .ClickAndHold()
                                  .MoveToElement(element, w / 3, h / 3)
                                  .Build()
                                  .Perform();
                            Thread.Sleep(1500);
                            action.Release().Build().Perform();
                            i++;
                        }
                    }
                    else
                    {
                        while (i < 2)
                        {
                            action.MoveToElement(element, w / 6, h / 8)
                                  .ClickAndHold()
                                  .MoveToElement(element, w / 2, h - (h / 6))
                                  .Build()
                                  .Perform();
                            action.Release(element).Build().Perform();
                            i++;
                        }
                    }

                    PageLoadWait.WaitForFrameLoad(2);

                    patients.ClickElement("Pan");
                    int j = 0;
                    while (j < 1)
                    {
                        action.MoveToElement(element, w - (w / 6), h - (h / 8))
                              .ClickAndHold()
                              .MoveToElement(element, w - (w / 3), h - (h / 3))
                              .Build()
                              .Perform();
                        Thread.Sleep(2000);
                        action.Release().Build().Perform();
                        j++;
                    }

                    patients.ClickElement("Rotate Clockwise");

                    action.MoveToElement(element, w - (w / 4), h / 4).Click().Build().Perform();

                    patients.ClickElement("Invert");

                    action.MoveToElement(element, w - (w / 6), h / 6).Click().Build().Perform();

                    patients.ClickElement("Flip Vertical");
                }

                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_15 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_15)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-16
                patients.Click("id", login.GetControlId("SeriesViewer2-1X2"));

                patients.ClickElement("Series Scope");

                patients.ClickElement("Reset");

                PageLoadWait.WaitForFrameLoad(5);

                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_16 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_16)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step-17
                patients.Click("id", login.GetControlId("SeriesViewer2-1X2"));
                var element1 = patients.GetElement("id", login.GetControlId("SeriesViewer2-1X2"));

                int h1 = element1.Size.Height;
                int w1 = element1.Size.Width;

                var action1 = new Actions(BasePage.Driver);

                if (element1 != null)
                {
                    patients.ClickElement("Image Layout 2x2");

                    patients.ClickElement("Zoom");
                    int i = 0;
                    if (IsHTML5)
                    {
                        while (i < 2)
                        {
                            action1.MoveToElement(element, w1 / 9, h1 / 9)
                                  .ClickAndHold()
                                  .MoveToElement(element, w1 / 3, h1 / 3)
                                  .Build()
                                  .Perform();
                            Thread.Sleep(1500);
                            action1.Release().Build().Perform();
                            i++;
                        }
                    }
                    else
                    {
                        while (i < 2)
                        {
                            action1.MoveToElement(element, w1 / 6, h1 / 8)
                                  .ClickAndHold()
                                  .MoveToElement(element, w1 / 2, h1 - (h1 / 6))
                                  .Build()
                                  .Perform();
                            action1.Release(element).Build().Perform();
                            i++;
                        }
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Element not found" + patients.GetControlId("SeriesViewer2-1X2"));
                }

                PageLoadWait.WaitForFrameLoad(5);

                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_17_1 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_17_1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-18
                patients.ClickElement("Pan");

                if (element != null)
                {
                    int j = 0;
                    while (j < 1)
                    {

                        action1.MoveToElement(element, w1 - (w1 / 6), h1 - (h1 / 8))
                              .ClickAndHold()
                              .MoveToElement(element, w1 - (w1 / 3), h1 - (h1 / 3))
                              .Build()
                              .Perform();
                        Thread.Sleep(2000);
                        action1.Release().Build().Perform();
                        j++;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Element not found" + patients.GetControlId("SeriesViewer2-1X2"));
                }

                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_17__pan = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_17__pan)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-19
                patients.ClickElement("Rotate Clockwise");

                if (element != null)
                {
                    action1.MoveToElement(element, w1 - (w1 / 4), h1 / 4).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                }
                else
                {
                    Logger.Instance.ErrorLog("Element not found" + patients.GetControlId("SeriesViewer2-1X2"));
                }


                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_17__rotate = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_17__rotate)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step20
                patients.ClickElement("Invert");

                if (element != null)
                {
                    action1.MoveToElement(element, w1 - (w1 / 6), h1 / 6).Click().Build().Perform();
                }
                else
                {
                    Logger.Instance.ErrorLog("Element not found" + patients.GetControlId("SeriesViewer2-1X2"));
                }

                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_17__invert = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_17__invert)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21
                patients.ClickElement("Flip Vertical");

                PageLoadWait.WaitForFrameLoad(5);

                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_17__flip = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_17__flip)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-22 [QAC step-18]
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                patients.Click("id", login.GetControlId("SeriesViewer2-1X2"));

                patients.ClickElement("Image Scope");

                patients.ClickElement("Reset");
                PageLoadWait.WaitForFrameLoad(5);

                element = patients.GetElement("id", login.GetControlId("SeriesViewer2-1X2"));

                if (element != null)
                {
                    patients.ClickElement("Image Layout 2x2");
                    PageLoadWait.WaitForFrameLoad(5);


                    patients.ClickElement("Image Scope");
                    patients.ClickElement("Zoom");

                    int h = element.Size.Height;
                    int w = element.Size.Width;

                    var action = new Actions(BasePage.Driver);

                    int i = 0;
                    if (IsHTML5)
                    {
                        while (i < 2)
                        {
                            action.MoveToElement(element, w / 9, h / 9)
                                  .ClickAndHold()
                                  .MoveToElement(element, w / 3, h / 3)
                                  .Build()
                                  .Perform();
                            Thread.Sleep(1500);
                            action.Release().Build().Perform();
                            i++;
                        }
                    }
                    else
                    {
                        while (i < 2)
                        {
                            action.MoveToElement(element, w / 6, h / 8)
                                  .ClickAndHold()
                                  .MoveToElement(element, w / 2, h - (h / 6))
                                  .Build()
                                  .Perform();
                            action.Release(element).Build().Perform();
                            i++;
                        }
                    }

                    PageLoadWait.WaitForFrameLoad(2);

                    patients.ClickElement("Pan");
                    int j = 0;
                    while (j < 1)
                    {
                        action.MoveToElement(element, w - (w / 6), h - (h / 8))
                              .ClickAndHold()
                              .MoveToElement(element, w - (w / 3), h - (h / 3))
                              .Build()
                              .Perform();
                        Thread.Sleep(2000);
                        action.Release().Build().Perform();
                        j++;
                    }

                    patients.ClickElement("Rotate Clockwise");

                    action.MoveToElement(element, w - (w / 4), h / 4).Click().Build().Perform();

                    patients.ClickElement("Invert");

                    action.MoveToElement(element, w - (w / 6), h / 6).Click().Build().Perform();

                    patients.ClickElement("Flip Vertical");
                }

                viewport = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_22 = patients.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step27832_22)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23 [QAC step-19]
                result.steps[++ExecutedSteps].status = "Not Automated";

                login.CloseBrowser();


                //step-24  [QAC step-20]        
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.wpfobject.GetTabWpf(1).SelectTabPage(2);
                servicetool.wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);

                servicetool.wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.ID.EnableAttachment);
                //servicetool.wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.IntegratorAllowed,1);
                servicetool.wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.ID.UploadAllowed);
                servicetool.wpfobject.ClickRadioButton(ServiceTool.EnableFeatures.ID.StoreOriginalStudy);

                if (servicetool.wpfobject.VerifyIfChecked(ServiceTool.EnableFeatures.ID.StoreOriginalStudy))
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

                servicetool.ApplyEnableFeatures();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step-25 [QAC step-21]
                patients.InvokeBrowser(Config.BrowserType);
                patients.NavigateToIntegratorURL("http://" + Config.IConnectIP + link[4]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                PageLoadWait.WaitForElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"), BasePage.WaitTypes.Visible);
                PageLoadWait.WaitForFrameLoad(10);

                var viewport17 = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27832_21 = patients.CompareImage(result.steps[ExecutedSteps], viewport17);

                if (step27832_21)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-26 [QAC step-22]
                patients.ClickPatientHistoryTab();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame").SwitchTo().Frame("iframeAttachment");

                var attachment = patients.GetElement("id", "inputAttachment");

                if (attachment != null)
                {
                    attachment.SendKeys(Filepath);
                    //  attachment.SendKeys(@"D:\Test\TestFile.txt");
                    //attachment.SendKeys(@"D:\Selenium_executable\Release\TestFile.txt");
                    patients.Click("id", "m_sendAttachmentButton");
                }
                Thread.Sleep(4000);
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame");

                if (patients.DocumentExists(expecteddata[2]))
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

                //step-27,28 [23,24--QAC steps]
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                login.CloseBrowser();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                //Post-Conditions: Disabling HTML4 as default and setting HTML5 as default
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.EnableHTML5();
                //servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Re-Enabling HTML5 from user preferences
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                //Open User Preference
                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                login.SetRadioButton("id", "DefaultViewerSettingRadioButtonList_0");
                login.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary>
        /// Grant Access - Study Share
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27834(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            Studies studies = null;
            DomainManagement domainmgmt = null;
            UserManagement usermgmt = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionIDList.Split(':');
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                int randomNumber = new Random().Next(1000);
                String DomainUser = "DomainUser_" + randomNumber;
                String Group = "Group_" + randomNumber;
                String GroupUser = "GroupUser_" + randomNumber;

                //PreCondition
                //Login as Administrator
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //Navigate to domain management
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");

                //Create new domain
                Dictionary<object, string> domainattr1;
                domainattr1 = domainmgmt.CreateDomainAttr();
                String Domain = domainattr1[DomainManagement.DomainAttr.DomainName];
                domainmgmt.CreateDomain(domainattr1);

                //Edit domain - Enable grant access
                domainmgmt.SearchDomain(Domain);
                domainmgmt.SelectDomain(Domain);
                domainmgmt.ClickEditDomain();
                domainmgmt.ModifyStudySearchFields();
                domainmgmt.SetCheckBoxInEditDomain("grant", 0);
                domainmgmt.ClickSaveDomain();

                //Navigate to user management
                usermgmt = (UserManagement)login.Navigate("UserManagement");

                //Create new user for the domain
                usermgmt.CreateUser(DomainUser, Domain, domainattr1[DomainManagement.DomainAttr.RoleName], 1, Config.emailid, 1, DomainUser);
                PageLoadWait.WaitForFrameLoad(10);

                //Create new group for the domain
                usermgmt.CreateGroup(Domain, Group, Group, domainattr1[DomainManagement.DomainAttr.RoleName], IsManaged: 1, UserName: Group);

                //Create new user for the group created above
                usermgmt.SelectGroup(Group, Domain);
                usermgmt.CreateUserForGroup(Group, GroupUser, Domain, domainattr1[DomainManagement.DomainAttr.RoleName], 1, Config.emailid, 1, GroupUser);

                //Allow grant access to everyone
                usermgmt.SearchUser(GroupUser);
                usermgmt.SelectUser(GroupUser);
                usermgmt.ClickEditUser();
                PageLoadWait.WaitForFrameLoad(10);
                usermgmt.AllowGrantToAnyOne().Click();
                usermgmt.SaveBtn().Click();

                //Logout as Admin
                login.Logout();

                //Upload Study
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.DestinationPACS);

                //Step 1:- Initial Setup
                ExecutedSteps++;

                //Step 2 :- Login as User "U2"
                login.LoginIConnect(DomainUser, DomainUser);
                ExecutedSteps++;

                //Navigate to inbounds
                studies = (Studies)login.Navigate("Studies");

                //Search studies
                studies.SearchStudy("PatientID", PatientID);

                //Select all studies
                Boolean StudiesSelected = true;
                foreach (String Accession in AccessionNumbers)
                {
                    studies.SelectStudy1("Accession", Accession, true);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitHomePage();

                    if (studies.SelectedStudyrow(Accession) == null)
                    {
                        StudiesSelected = false;
                        break;
                    }
                }

                //Step 3 :- Validate selected studies are highlighted
                if (StudiesSelected)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click Grant Access Button
                studies.GrantAccessBtn().Click();

                //Sync-up
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.GrantAccessBtn_GAwindow()));

                IList<Boolean> sharedStudies = new List<Boolean>();
                foreach (String Accession in AccessionNumbers)
                {
                    Boolean rowStatus = studies.ShareGridTable().FindElement(By.CssSelector("span[title='" + Accession + "']")).Displayed;
                    sharedStudies.Add(rowStatus);
                }

                //Step 4 :- Validate selected studies are listed in grant access window
                if (studies.ShareGridTable().Displayed && !sharedStudies.Contains(false))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search group in the selected domain
                studies.GroupFilterTextbox().SendKeys(Group);
                studies.GroupSearchBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Get filtered group name
                IWebElement groupListed_searched = studies.GroupListTable().FindElement(By.CssSelector("span"));

                //Step 5 :- Validate Searched group is listed in list box
                if (groupListed_searched.GetAttribute("innerHTML").Equals(Group))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Add group
                groupListed_searched.Click();
                PageLoadWait.WaitForPageLoad(20);
                studies.GroupListAddBtn().Click();

                //Get filtered group name
                IWebElement groupListed_selected = studies.GroupList_Selected().FindElement(By.CssSelector("span"));

                //Step 6 :- Validate selected group is listed in selected groups list box
                if (groupListed_selected.GetAttribute("innerHTML").Equals(Group))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click grant access button
                studies.GrantAccessBtn_GAwindow().Click();

                //Sync-up 
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DialogContentDiv")));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitHomePage();

                //Step 7 :- Validate email is sent to shared users 
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Navigate to outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Step 8 :- Validate shared studies are listed in outbounds tab
                ExecutedSteps++;
                foreach (String Accession in AccessionNumbers)
                {
                    //Search study
                    outbounds.SearchStudy("Accession", Accession);

                    //Validation
                    if (outbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" }) != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("One of the study not shared");
                    }
                }

                //Logout
                login.Logout();

                //Step 9 :- Login as User "U5"
                login.LoginIConnect(Group, Group);
                ExecutedSteps++;

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search studies
                inbounds.SearchStudy("PatientID", PatientID);

                //Search all studies
                Boolean StudiesListed = true;
                foreach (String Accession in AccessionNumbers)
                {
                    if (inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" }) == null)
                    {
                        StudiesListed = false;
                        break;
                    }
                }

                //Step 10 :- Validate shared studies are listed
                if (StudiesListed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select and Launch Study
                inbounds.SelectStudy1("Accession", AccessionNumbers[0]);
                inbounds.LaunchStudy();

                //Step 11 :- Validate images are displayed
                ExecutedSteps++;

                //Close study and logout
                inbounds.CloseStudy();
                login.Logout();

                //Step 12 :- Login as User "U6"
                login.LoginIConnect(GroupUser, GroupUser);
                ExecutedSteps++;

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search studies
                inbounds.SearchStudy("PatientID", PatientID);

                //Search all studies
                Boolean StudiesListed_2 = true;
                foreach (String Accession in AccessionNumbers)
                {
                    if (inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" }) == null)
                    {
                        StudiesListed_2 = false;
                        break;
                    }
                }

                //Step 13 :- Validate shared studies are listed
                if (StudiesListed_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select and Launch Study
                inbounds.SelectStudy1("Accession", AccessionNumbers[0]);
                inbounds.LaunchStudy();

                //Step 14 :- Validate images are displayed
                ExecutedSteps++;

                //Step 15 :- Close study and logout
                inbounds.CloseStudy();
                login.Logout();
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
        /// Simple Download to local machine
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27836(String testid, String teststeps, int stepcount)
        {
            Studies studies = null;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;


            Patients patients = null;
            patients = new Patients();

            TestCaseResult result;

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientFirstName");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String RoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Rolemanagement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String ErrorMessage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ErrorMessage");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] name = Name.Split(':');
                String Link = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Link");
                String[] link = Link.Split(':');

                String StudyList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Patient Name");
                String[] studyList = StudyList.Split(':');

                String[] array = new String[] { "Patient Name" };

                String HostName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "HostName");

                String ExpectedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ExpectedData");
                String[] expecteddata = ExpectedData.Split(':');


                String DataSent = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSent");
                String[] dataSent = DataSent.Split(':');

                //Step-1
                //initial set up done
                ExecutedSteps++;

                //Step-2 
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.EnableDataDownloader();
                servicetool.ApplyEnableFeatures();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.wpfobject.WaitTillLoad();
                servicetool.SetEnableFeaturesTransferService();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.EnableTransferService();
                servicetool.ModifyPackagerDetails("5");
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step-3
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();

                //select data download flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(5);

                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");

                //Click Edit in RoleManagement Tab     
                rolemanagement.SearchRole(RoleName, "SuperAdminGroup");
                rolemanagement.SelectRole(DomainName);
                rolemanagement.ClickEditRole();

                //select allow download flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(5);

                //Open User Preference
                rolemanagement.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                rolemanagement.SetRadioButton("id", "DownloadRadioButtonList_0");
                rolemanagement.CloseUserPreferences();
                ExecutedSteps++;

                //Step-4
                //Navigate to Studies Tab
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;
                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchButton")));
                //patients.Click("cssselector", "#m_studySearchControl_m_searchButton");

                //PageLoadWait.WaitForFrameLoad(5);

                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                //PageLoadWait.WaitForElement(By.Id("m_studyGrid_m_errorDetail"), BasePage.WaitTypes.Visible);
                //if (BasePage.Driver.FindElement(By.Id("m_studyGrid_groupByDiv")).Text.Contains(ErrorMessage))
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //Step-5
                //studies.SetStudyListLayout(array, 1);
                //int counter = 0;
                //while (!studies.CheckStudyListColumnNames(array) && counter < 10)
                //{
                //    studies.SetStudyListLayout(array, 1);
                //    counter++;
                //}
                studies.SearchStudy("Last Name", studyList[2].Split(',')[0].Trim());
                studies.ChooseColumns(array);
                studies.SelectStudy1("Patient Name", studyList[2]);
                ExecutedSteps++;

                //Dictionary<int, string[]> results = BasePage.GetSearchResults();
                //string[] columnnames = BasePage.GetColumnNames();
                //string[] columnvalues = BasePage.GetColumnValues(results, studyList[0], columnnames);
                //int rowindex = BasePage.GetMatchingRowIndex(columnvalues, studyList[2]);

                //IList<IWebElement> rows = BasePage.Driver.FindElements(By.CssSelector("[id^='gridTable']>tbody>tr"));

                //if (rows != null)
                //{
                //    var classText = rows[rowindex + 1].GetAttribute("class");

                //    if (classText.Equals("ui-widget-content jqgrow ui-row-ltr ui-state-highlight"))
                //    {
                //        result.steps[++ExecutedSteps].status = "Pass";
                //        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //    }

                //    else
                //    {
                //        result.steps[++ExecutedSteps].status = "Fail";
                //        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //        result.steps[ExecutedSteps].SetLogs();
                //    }
                //}

                //step-6
                studies.Click("id", "m_transferButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");

                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_transferDataGrid")).Text.Contains(studyList[2]))
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

                //Step-7
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                studies.SelectFromList("id", "ctl00_StudyTransferControl_m_destinationSources", "Local System", 1);
                studies.Click("id", "ctl00_StudyTransferControl_TransferButton");
                BasePage.Driver.SwitchTo().DefaultContent();
                studies.SwitchTo("index", "0");
                if (studies.PatientExistsinTransfer(studyList[2]))
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

                //Step 8 - Select Confirm-all in Quality Control Window
                studies.ClickConfirm_allInQCWindow();

                IList<IWebElement> tablerows = BasePage.Driver.FindElements(By.CssSelector("#ctl00_DataQCControl_datagrid>tbody>tr[title='']"));

                foreach (IWebElement row in tablerows)
                {
                    if (row.FindElement(By.CssSelector("td>.QCData_Confirm")).Displayed == true)
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
                }
                //Step-9
                patients.Click("id", "ctl00_DataQCControl_m_submitButton");
                PageLoadWait.WaitForFrameLoad(2);
                patients.Click("id", "ctl00_TransferJobsListControl_m_closeDialogButton");
                ExecutedSteps++;

                //Step 10 - Select one study with ready status
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                new WebDriverWait(BasePage.Driver, new TimeSpan(0, 1, 0)).Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid > tbody > tr:nth-child(2) > td:nth-child(11) > span[title*='Ready']")).Click();
                ExecutedSteps++;

                //Step 11 - Click download button in transfer status window
                IWebElement downloadButton = BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_m_submitButton"));
                downloadButton.Click();
                ExecutedSteps++;
             

                //Step-12
                studies.ClickButtonInDownloadPackagesWindow("Download");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step-13 to 14: select location and save from browser - already taken care as part of download
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 15: Click Back
                studies.ClickButtonInDownloadPackagesWindow("back");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;


                //Step 16: Select the study again that was previously downloaded
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                new WebDriverWait(BasePage.Driver, new TimeSpan(0, 1, 0)).Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Done']")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid > tbody > tr:nth-child(2) > td:nth-child(11) > span[title*='Done']")).Click();
                ExecutedSteps++;

                //Step 17: Select Download
                downloadButton = BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_m_submitButton"));
                downloadButton.Click();
                ExecutedSteps++; 
                
                //Step 18: Download
                studies.ClickButtonInDownloadPackagesWindow("Download");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 19: Click Save to another location
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 20: Click close
                studies.ClickButtonInDownloadPackagesWindow("close");
                ExecutedSteps++;

                // Step-21

                patients.TransferStatus();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                new WebDriverWait(BasePage.Driver, new TimeSpan(0, 1, 0)).Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span")));
                string status = BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid > tbody > tr:nth-child(2) > td:nth-child(11) > span")).GetAttribute("title");

                if (status == "Ready" || status == "Done" || status == "Expired")
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

                patients.Click("id", "ctl00_TransferJobsListControl_m_closeDialogButton");

                //Step-22-24
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                login.Logout();
                login.CloseBrowser();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///Active Directory LDAP Identity map
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27837(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            UserManagement usermanagement;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String DomainNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String[] domainname = DomainNames.Split(':');
                String datasource = login.GetHostName(Config.SanityPACS);
                string DomainName = "TestDomain";
                string Role = "AdminRole1";
            
                

                //Step 1 :- Precondition - Initial Setup - Enable LDAP
                servicetool.EnableLDAPConfigfile();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.LDAPSetup();                
                ExecutedSteps++;

                //step-2 :in service tool Go to user management
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(1);
                ExecutedSteps++;

                //step-3:In the service tool select the LDAP tab and in the Servers Tab select the ServerID.
                servicetool.NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //step-4:Select ica-ldap.merge.ad and click on the Detail button
                string serverName = "ica.ldap.merge.ad";
                GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
                foreach (var row in datagrid1.Rows)
                {
                    if (row.Cells[0].Text.ToLower().Equals(serverName.ToLower()))
                    {
                        row.Focus();
                        wpfobject.WaitTillLoad();
                        row.Click();
                        wpfobject.WaitTillLoad();                      
                        break;
                    }
                }                
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                wpfobject.WaitForPopUp();                
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                Thread.Sleep(5000);
                GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
                datagrid.Rows[0].Cells[0].Focus();
                var u = datagrid.Rows[0].Cells[0].Text;            
                GroupBox siteDomain_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.LDAP.Name.SiteDomainNamesGrp, 1);
                TextBox tb = wpfobject.GetAnyUIItem<GroupBox, TextBox>(siteDomain_grp, ServiceTool.LDAP.ID.SiteDomainNamesTxt);
                if (tb.Text.Contains(domainname[0]) && tb.Text.Contains(domainname[1]) && tb.Text.Contains(domainname[2]))
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

                //step-5:Select the Mapping Details tab then Select Identity from the Type drop down menu.
                /*try
                {
                    Thread.Sleep(3000);
                    servicetool.NavigateToTab(ServiceTool.LDAP.Name.MappingDetailsTab);
                }
                catch(Exception e)
                {
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                    servicetool.NavigateToTab(ServiceTool.LDAP.Name.MappingDetailsTab);
                }
                //ITabPage tab = WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.LDAP.Name.MappingDetailsTab));
                //tab.Focus();
                //tab.Click();
                //wpfobject.SelectTabFromTabItems(ServiceTool.LDAP.Name.MappingDetailsTab);
                Thread.Sleep(5000);                
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.LDAP.Name.LdaporLocalMapsSubTab);
                GroupBox Valuemap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(subtab, ServiceTool.LDAP.Name.ValueMapGrp, 1);
                ComboBox comboBox = wpfobject.GetUIItem<GroupBox, ComboBox>(Valuemap_grp, "", 1, "0");
                //ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("identity");
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                var value = comboBox.SelectedItem;
                if (value.Name.Equals("identity"))
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
                */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-6:Click on OK , apply , Restart IIS
                wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step-7:Login iCA as a registered user <br>UID = ica.administrator<br>PID = admin.13579
                login.LoginIConnect(Config.LdapAdminUserName, Config.LdapAdminPassword);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                IWebElement tabSelected = BasePage.Driver.FindElement(By.CssSelector("td[id^=TabMid] div[class='TabText TabSelected']"));
                string currenttab = tabSelected.GetAttribute("innerHTML");               
                domainmanagement = login.Navigate<DomainManagement> ();
                domainmanagement.SearchDomain("SuperAdminGroup");
                PageLoadWait.WaitForFrameLoad(20);
                //BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement superadmin = BasePage.Driver.FindElement(By.CssSelector("div[class='row'] tr td>span[title='SuperAdminGroup']"));
                if (currenttab.Equals("Domain Management") && superadmin != null)
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

                //Precondition
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.SetCheckBoxInEditDomain("imagesharing", 0);
                domainmanagement.DisConnectDataSource(Config.EA91);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //step-8:Double click on the SuperAdminGroup
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                domainmanagement.SearchDomain("SuperAdminGroup");
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement superadmin1 = BasePage.Driver.FindElement(By.CssSelector("div[class='row'] tr td>span[title='SuperAdminGroup']"));
                Actions action = new Actions(BasePage.Driver);
                action.DoubleClick(superadmin1).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement SAGwindow = BasePage.Driver.FindElement(By.CssSelector("div[id='EditDomain_Content']"));
                if (SAGwindow != null)
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
                

                //step-9:Select a Data Source and move it to the connected side.
                domainmanagement.ConnectDataSource(login.GetHostName(Config.EA91));
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement connected = BasePage.Driver.FindElement(By.CssSelector("select[id='ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox'] option[value='" + domainmanagement.GetHostName(Config.SanityPACS) + "']"));
                if (connected != null)
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


                //step-10:click on Save
                //domainmanagement.ClickSaveDomain();
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                IWebElement tabSelected10 = BasePage.Driver.FindElement(By.CssSelector("td[id^=TabMid] div[class='TabText TabSelected']"));
                string currenttab10 = tabSelected10.GetAttribute("innerHTML");       
                PageLoadWait.WaitForFrameLoad(20);
                if (currenttab10.Equals("Domain Management"))
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

                //step-11:Select the User management tab and select the SuperAdminGroup Domain click on the Search button
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser("*", domainname[0]);
                //IWebElement user = BasePage.Driver.FindElement(By.CssSelector("div#m_groupListControlDiv div#groupListDiv div>div>div>table tr>td>span"));
                IWebElement newUsrBtn = usermanagement.NewUsrBtn();
                IWebElement editUsrbtn = usermanagement.EditUsrBtn();
                IWebElement delUsrbtn = usermanagement.DelUsrBtn();
                IWebElement newGrpBtn = usermanagement.NewGrpBtn();
                IWebElement newSubGrpBtn = usermanagement.NewSubGrpBtn();
                IWebElement editGrpBtn = usermanagement.EditGrpBtn();
                IWebElement delGrpBtn = usermanagement.DelGrpBtn();
                IWebElement moveGrpBtn = usermanagement.MoveGrpBtn();
                IWebElement dataMappingBtn = usermanagement.DataMappingBtn();
                Boolean AllBtnStatus = newUsrBtn.Displayed == false && editUsrbtn.Displayed == false &&
                    delUsrbtn.Displayed == true && newGrpBtn.Displayed == true && newSubGrpBtn.Displayed == true &&
                    editGrpBtn.Displayed == true && delGrpBtn.Displayed == true && moveGrpBtn.Displayed == true &&
                    dataMappingBtn.Displayed == true;
                bool u1 = usermanagement.IsUserExist(Config.LdapAdminUserName.Split('.')[1], domainname[0]);
                bool u2 = usermanagement.IsUserExist(Config.ldapuser1.Split('.')[0], domainname[0]);
                bool u3 = usermanagement.IsUserExist(Config.ldapuser2.Split('.')[0], domainname[0]);
                //IWebElement moveUsrBtn = usermanagement.MoveUsrBtn();                
                if (u1 == true && u2 == true && u3 == true && AllBtnStatus)
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

                //step-12:Logout and log back in as victoria.dassen/.vcd.13579
                login.Logout();
                login.LoginIConnect(Config.ldapuser1, Config.ldappass1);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                IWebElement tabSelected2 = BasePage.Driver.FindElement(By.CssSelector("td[id=TabMid0] div[class='TabText TabSelected']"));
                string currenttab2 = tabSelected2.GetAttribute("innerHTML");
                if (currenttab2.Equals("Studies"))
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

                //step-13:Change the Study Performed box from Last 2 Days to All Dates and click the search button
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy();
                IList<IWebElement> rows = BasePage.Driver.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content']"));
                int count = rows.Count;
                if (count != 0)
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

                //step-14:Click on options User Preferences
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.OpenUserPreferences();
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                IWebElement usrPreference = BasePage.Driver.FindElement(By.CssSelector("form[name='form1'] div[id='PreferencesDiv']"));
                if (usrPreference.Displayed == true)
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


                //step-15:Edit some parameter and click OK to Close
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ScopeRadioButtons_1")).Click();
                //studies.SetUserPreferences();
                studies.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                IWebElement tabSelected3 = BasePage.Driver.FindElement(By.CssSelector("td[id=TabMid0] div[class='TabText TabSelected']"));
                string currenttab3 = tabSelected3.GetAttribute("innerHTML");
                if (currenttab3.Equals("Studies"))
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

                //step-16:Reopen the user preferences page
                studies.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                IWebElement download = BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ScopeRadioButtons_1"));
                if (download.Selected == true)
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
                studies.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //step-17:Logout and login as ica.administrator/admin.13579
                login.Logout();
                login.LoginIConnect(Config.LdapAdminUserName, Config.LdapAdminPassword);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement home = BasePage.Driver.FindElement(By.CssSelector("div[id='ctl00_FunctionalDiv']"));
                if (home != null)
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
                //Logout as PACS user               
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Reset LDAP settings
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                //wpfobject.ClickButton("md_modifyBtn");
                servicetool.SetMode(2);
                servicetool.CloseServiceTool();

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

                //reset LDAp settings
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.CloseServiceTool();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Work List Display From MergePacs
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27838(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String MergePacsUserName = Config.mergepacsuser;
                String MergePacsUserPassword = Config.mergepacspassword;
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionIDList.Split('=');
                String[] WorklistAccession_1 = AccessionNumbers[0].Split(':');
                String[] WorklistAccession_2 = AccessionNumbers[1].Split(':');
                String[] WorklistAccession_3 = AccessionNumbers[2].Split(':');
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String WorkList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WorkList");
                String[] WorkListNames = WorkList.Split('=');
                String datasource = login.GetHostName(Config.SanityPACS);

                //Step 1 :- Precondition - Initial Setup
                servicetool.LaunchServiceTool();
                servicetool.WaitWhileBusy();
                servicetool.SetWorkListInPACS(datasource);
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to Domain Management tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Select domain
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);

                //Edit the report view option and Save
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("breifcase", 0);
                domainmanagement.ConnectDataSource(datasource.ToUpper());
                domainmanagement.ClickSaveDomain();

                //Navigate to User Management tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //User Status
                Boolean UserStatus = usermanagement.SearchUser(MergePacsUserName, DomainName);

                //Create new user for the SuperAdmin domain if not exists               
                if (!UserStatus)
                {
                    usermanagement.CreateUser(MergePacsUserName, DomainName, "Staff");
                    UserStatus = usermanagement.SearchUser(MergePacsUserName, DomainName);
                }

                //Step 2 :- Validate user is created with same user name in merge pacs
                if (UserStatus)
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

                //Logout as Admin
                login.Logout();

                //Step 3 :- Login as PACS user
                login.LoginIConnect(MergePacsUserName, MergePacsUserPassword);
                ExecutedSteps++;

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);

                //Step 4 :- Validate brief case button is displayed
                if (studies.BriefCaseBtn().Enabled)
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

                //Click BriefCase button
                studies.BriefCaseBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.BriefCaseBtn()));

                SelectElement selector = new SelectElement(studies.BriefCaseDropdown());
                String selectedWorklist_1 = selector.SelectedOption.Text;

                //Get Study Details in the selected worklist
                Dictionary<int, string[]> results = BasePage.GetSearchResults();
                string[] columnnames_1 = BasePage.GetColumnNames();
                string[] AccessionList_1 = BasePage.GetColumnValues(results, "Accession", columnnames_1);

                //Step 5 :- Validate worklist displays the default(first) worklist
                int CountIndex = 0;
                if (selectedWorklist_1.Equals(selector.Options[1].Text)
                    && Array.Exists(WorklistAccession_1, s => s.Equals(AccessionList_1[CountIndex++])))
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

                //Step 6 :- Launch a study and valiadate images are displayed
                studies.SelectStudy1("Accession", WorklistAccession_1[0]);
                studies.LaunchStudy();
                ExecutedSteps++;

                //Close Study
                studies.CloseStudy();

                //Choose first worklist and get selected worklist name
                selector = new SelectElement(studies.BriefCaseDropdown());
                selector.SelectByIndex(1);
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.BriefCaseBtn()));
                String selectedWorklist_2 = selector.SelectedOption.Text;

                //Step 7 :- Validate selected worklist displays the same name as above
                if (selectedWorklist_1.Equals(selectedWorklist_2))
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

                //Choose second worklist
                selector = new SelectElement(studies.BriefCaseDropdown());
                selector.SelectByIndex(2);
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.BriefCaseBtn()));

                //Get Study Details in the second worklist
                Dictionary<int, string[]> results_2 = BasePage.GetSearchResults();
                string[] columnnames_2 = BasePage.GetColumnNames();
                string[] AccessionList_2 = BasePage.GetColumnValues(results_2, "Accession", columnnames_2);

                //Choose third worklist
                selector = new SelectElement(studies.BriefCaseDropdown());
                selector.SelectByIndex(3);
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.BriefCaseBtn()));

                //Get Study Details in the third worklist
                Dictionary<int, string[]> results_3 = BasePage.GetSearchResults();
                string[] columnnames_3 = BasePage.GetColumnNames();
                string[] AccessionList_3 = BasePage.GetColumnValues(results_3, "Accession", columnnames_3);

                //Compare studies
                CountIndex = 0;
                Boolean WorklistMatch_2 = Array.Exists(WorklistAccession_2, s => s.Equals(AccessionList_2[CountIndex++]));
                CountIndex = 0;
                Boolean WorklistMatch_3 = Array.Exists(WorklistAccession_3, s => s.Equals(AccessionList_3[CountIndex++]));

                //Step 8 :- Validate selected worklist displays the same name as above
                if (WorklistMatch_2 && WorklistMatch_3)
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

                //Logout as PACS user               
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

        ///// <summary>
        ///// Integrator User Sharing
        ///// </summary>
        ///// <param name="testid"></param>
        ///// <param name="teststeps"></param>
        ///// <param name="stepcount"></param>
        ///// <returns></returns>
        //public TestCaseResult Test_27839(String testid, String teststeps, int stepcount)
        //{
        //    //Declare and initialize variables            
        //    DomainManagement domainmgmt = null;
        //    RoleManagement rolemgmt = null;
        //    UserManagement usermgmt = null;
        //    Random randomnumber = new Random();

        //    //Domain-1 Users and Role
        //    Dictionary<object, string> domainattr1;
        //    String D1Physician = "Physician1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
        //    String D1ph = "ph1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
        //    TestCaseResult result;
        //    result = new TestCaseResult(stepcount);
        //    int executedSteps = -1;

        //    try
        //    {

        //        //Get Test Data
        //        String modalitytype = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
        //        String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
        //        String unknownuser1 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
        //        String unknownuser2 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 1000);
        //        String unknownuser3 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 1000);
        //        String unknownuser4 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 1000);

        //        //Set up Validation Steps
        //        result.SetTestStepDescription(teststeps);

        //        //Step-1 - Service Tool Settings.
        //        servicetool.LaunchServiceTool();
        //        servicetool.NavigateToTab("Integrator");
        //        servicetool.WaitWhileBusy();
        //        servicetool.EanbleUserSharing_ShadowUser(usersharing: "disable", shadowuser: "disable");
        //        servicetool.NavigateToTab("Viewer");
        //        servicetool.WaitWhileBusy();
        //        servicetool.NavigateSubTab("Protocols");
        //        servicetool.WaitWhileBusy();
        //        servicetool.MoadalitySetting();
        //        servicetool.CloseServiceTool();
        //        executedSteps++;

        //        //Step-2 - Create Domain, Role, User and setup viewer layout                
        //        login.LoginIConnect(Config.adminUserName, Config.adminPassword);
        //        domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
        //        domainattr1 = domainmgmt.CreateDomainAttr();
        //        domainmgmt.CreateDomain(domainattr1, new String[] { domainmgmt.GetHostName(Config.SanityPACS) });
        //        rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
        //        rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, "Physician");
        //        usermgmt = (UserManagement)login.Navigate("UserManagement");
        //        usermgmt.CreateUser(D1ph, domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, 1, Config.emailid, 1, D1ph);
        //        domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
        //        domainmgmt.SearchDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
        //        domainmgmt.SelectDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
        //        domainmgmt.ClickEditDomain();
        //        PageLoadWait.WaitForPageLoad(30);
        //        BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
        //        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scroll(0, 1000)");
        //        domainmgmt.ModalityDropDown().SelectByText(modalitytype);
        //        domainmgmt.LayoutDropDown().SelectByText("1x1");
        //        domainmgmt.ClickSaveDomain();
        //        executedSteps++;

        //        //Step-3 
        //        ehr.LaunchEHR();
        //        ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: D1ph);
        //        ehr.SetSelectorOptions("Study");
        //        ehr.SetSearchKeys_Study(accession);
        //        ehr.Load();
        //        BasePage.Driver.Quit();
        //        BasePage.Driver = null;

        //        //Validate Study Displayed in iConnect 
        //        Thread.Sleep(20000);
        //        rnxobject.WaitForElementTobeVisible(rxpathmainwindow);
        //        WebDocument browser = rxpathmainwindow;
        //        Element el = Host.Local.FindSingle(new RxPath("/dom[@domain='localhost']"));
        //        String rxpathstudypanel = ".//iframe[#'IntegratorHomeFrame']//div[@id='studyPanelDiv_1']";
        //        browser.WaitForDocumentLoaded(new Duration(20000));
        //        ehr.CloseEHR();
        //        Thread.Sleep(10000);
        //        rnxobject.WaitForElementTobeVisible(rxpathstudypanel);
        //        Element viewport = browser.FindSingle(new RxPath(rxpathstudypanel));
        //        executedSteps++;
        //        result.steps[executedSteps].SetPath(testid, executedSteps);
        //        Boolean isstudycorrect = RanorexObjects.CompareImage(result.steps[executedSteps], viewport);
        //        browser.Close();
        //        login = new Login();
        //        if (isstudycorrect)
        //        {
        //            result.steps[executedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[executedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
        //            result.steps[executedSteps].SetLogs();
        //        }

        //        //step-4-Enable shadow user, User sharing.
        //        servicetool.LaunchServiceTool();
        //        servicetool.NavigateToTab("Integrator");
        //        servicetool.WaitWhileBusy();
        //        servicetool.EanbleUserSharing_ShadowUser(usersharing: "enable", shadowuser: "enable");
        //        servicetool.CloseServiceTool();
        //        executedSteps++;

        //        //Step-5
        //        ehr.LaunchEHR();
        //        ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser1);
        //        ehr.SetSelectorOptions("Study");
        //        ehr.SetSearchKeys_Study(accession);
        //        ehr.Load();
        //        BasePage.Driver.Quit();
        //        BasePage.Driver = null;

        //        //Validate Study Displayed in iConnect
        //        Thread.Sleep(20000);
        //        rnxobject.WaitForElementTobeVisible(rxpathmainwindow);
        //        browser = rxpathmainwindow;
        //        browser.WaitForDocumentLoaded(new Duration(20000));
        //        ehr.CloseEHR();
        //        Thread.Sleep(10000);
        //        rnxobject.WaitForElementTobeVisible(rxpathstudypanel);
        //        viewport = browser.FindSingle(new RxPath(rxpathstudypanel));
        //        executedSteps++;
        //        result.steps[executedSteps].SetPath(testid, executedSteps);
        //        isstudycorrect = RanorexObjects.CompareImage(result.steps[executedSteps], viewport);
        //        browser.Close();

        //        //Validate shadow user created
        //        login = new Login();
        //        login.LoginIConnect(Config.adminUserName, Config.adminPassword);
        //        usermgmt = (UserManagement)login.Navigate("UserManagement");
        //        usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
        //        Boolean isuserpresent = usermgmt.IsUserPresent(unknownuser1);

        //        if (isstudycorrect && isuserpresent)
        //        {
        //            result.steps[executedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[executedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
        //            result.steps[executedSteps].SetLogs();
        //        }

        //        //Step-6 Change User sharing to URL determined               
        //        servicetool.LaunchServiceTool();
        //        servicetool.NavigateToTab("Integrator");
        //        servicetool.WaitWhileBusy();
        //        servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined", shadowuser: "enable");
        //        servicetool.CloseServiceTool();
        //        executedSteps++;

        //        //Step-7
        //        ehr.LaunchEHR();
        //        ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser2, usersharing: "True");
        //        ehr.SetSelectorOptions("Study");
        //        ehr.SetSearchKeys_Study(accession);
        //        ehr.Load();
        //        BasePage.Driver.Quit();
        //        BasePage.Driver = null;


        //        //Validate Study Displayed in iConnect                           
        //        Thread.Sleep(20000);
        //        rnxobject.WaitForElementTobeVisible(rxpathmainwindow);
        //        browser = rxpathmainwindow;
        //        browser.WaitForDocumentLoaded(new Duration(20000));
        //        ehr.CloseEHR();
        //        Thread.Sleep(10000);
        //        rnxobject.WaitForElementTobeVisible(rxpathstudypanel);
        //        viewport = browser.FindSingle(new RxPath(rxpathstudypanel));
        //        executedSteps++;
        //        result.steps[executedSteps].SetPath(testid, executedSteps);
        //        isstudycorrect = RanorexObjects.CompareImage(result.steps[executedSteps], viewport);
        //        browser.Close();

        //        //Validate shadow user created
        //        login = new Login();
        //        login.LoginIConnect(Config.adminUserName, Config.adminPassword);
        //        usermgmt = (UserManagement)login.Navigate("UserManagement");
        //        usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
        //        isuserpresent = usermgmt.IsUserPresent(unknownuser2);

        //        if (isstudycorrect && isuserpresent)
        //        {
        //            result.steps[executedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[executedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
        //            result.steps[executedSteps].SetLogs();
        //        }


        //        //Step-8
        //        ehr.LaunchEHR();
        //        ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser3, usersharing: "False");
        //        ehr.SetSelectorOptions("Study");
        //        ehr.SetSearchKeys_Study(accession);
        //        ehr.Load();
        //        BasePage.Driver.Quit();
        //        BasePage.Driver = null;


        //        //Validate Study Displayed in iConnect           
        //        Thread.Sleep(20000);
        //        rnxobject.WaitForElementTobeVisible(rxpathmainwindow);
        //        browser = rxpathmainwindow;
        //        browser.WaitForDocumentLoaded(new Duration(20000));
        //        ehr.CloseEHR();
        //        Thread.Sleep(10000);
        //        rnxobject.WaitForElementTobeVisible(rxpathstudypanel);
        //        viewport = browser.FindSingle(new RxPath(rxpathstudypanel));
        //        executedSteps++;
        //        result.steps[executedSteps].SetPath(testid, executedSteps);
        //        isstudycorrect = RanorexObjects.CompareImage(result.steps[executedSteps], viewport);
        //        browser.Close();
        //        //Validate sadow user created
        //        login = new Login();
        //        login.LoginIConnect(Config.adminUserName, Config.adminPassword);
        //        usermgmt = (UserManagement)login.Navigate("UserManagement");
        //        usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
        //        isuserpresent = usermgmt.IsUserPresent(unknownuser3);

        //        if (isstudycorrect && !isuserpresent)
        //        {
        //            result.steps[executedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[executedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
        //            result.steps[executedSteps].SetLogs();
        //        }


        //        //Step-9
        //        ehr.LaunchEHR();
        //        ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser4, usersharing: "False");
        //        ehr.SetSelectorOptions("Study");
        //        ehr.SetSearchKeys_Study(accession);
        //        ehr.Load();
        //        BasePage.Driver.Quit();
        //        BasePage.Driver = null;


        //        //Validate Study Displayed in iConnect           
        //        Thread.Sleep(20000);
        //        rnxobject.WaitForElementTobeVisible(rxpathmainwindow);
        //        browser = rxpathmainwindow;
        //        browser.WaitForDocumentLoaded(new Duration(20000));
        //        ehr.CloseEHR();
        //        Thread.Sleep(10000);
        //        rnxobject.WaitForElementTobeVisible(rxpathstudypanel);
        //        viewport = browser.FindSingle(new RxPath(rxpathstudypanel));
        //        executedSteps++;
        //        result.steps[executedSteps].SetPath(testid, executedSteps);
        //        isstudycorrect = RanorexObjects.CompareImage(result.steps[executedSteps], viewport);
        //        browser.Close();
        //        //Validate sadow user created
        //        login = new Login();
        //        login.LoginIConnect(Config.adminUserName, Config.adminPassword);
        //        usermgmt = (UserManagement)login.Navigate("UserManagement");
        //        usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
        //        isuserpresent = usermgmt.IsUserPresent(unknownuser4);

        //        if (isstudycorrect && !isuserpresent)
        //        {
        //            result.steps[executedSteps].status = "Pass";
        //            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
        //        }
        //        else
        //        {
        //            result.steps[executedSteps].status = "Fail";
        //            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
        //            result.steps[executedSteps].SetLogs();
        //        }


        //        //Report Result
        //        result.FinalResult(executedSteps);
        //        Logger.Instance.InfoLog("Overall Test status--" + result.status);

        //        //Return Result
        //        return result;
        //    }

        //    catch (Exception e)
        //    {
        //        //Log Exception
        //        Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

        //        //Report Result
        //        result.FinalResult(e, executedSteps);
        //        Logger.Instance.ErrorLog("Overall Test status--" + result.status);

        //        //Logout
        //        login = new Login();
        //        login.Logout();

        //        //Return Result
        //        return result;
        //    }

        //}

        /// <summary>
        /// Session Timeout
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27840(String testid, String teststeps, int stepcount)
        {
            //Variable Declaration
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int executedSteps = -1;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            StudyViewer viewer = new StudyViewer();
            String timeoutmessage = "You have not logged in yet or your session has expired. Please log in again.";

            try
            {

                //Get Test Data               
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Set up Validation Steps
                result.SetTestStepDescription(teststeps);

                //Step-1 - Domian and other setup already done.
                executedSteps++;

                //Step-2 - Set Session Timeout
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                servicetool.SetTimeout(timeout.Minutes);
                servicetool.CloseServiceTool();
                executedSteps++;

                //Step-3 - Login and Launch study and stay idle
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Studies studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new String[] { "Accession" });
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                //IWebElement viewstudy;
                //try { viewstudy = BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton")); }
                //catch (NoSuchElementException) { viewstudy = new IntegratorStudies().Intgr_ViewBtn(); }
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", viewstudy);
                //Logger.Instance.InfoLog("View Study button clicked.");
                //PageLoadWait.WaitForPageLoad(10);
                //PageLoadWait.WaitForFrameLoad(10);
                studies.LaunchStudy();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                int actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                bool message = BasePage.Driver.FindElement(By.CssSelector("span[id$='_LoginMasterContentPlaceHolder_ErrorMessage']")).GetAttribute("innerHTML").Equals(timeoutmessage);
                if (message && (actualtimeout == timeout.Minutes))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-4
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new String[] { "Accession" });
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                //try { viewstudy = BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton")); }
                //catch (NoSuchElementException) { viewstudy = new IntegratorStudies().Intgr_ViewBtn(); }
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", viewstudy);
                //Logger.Instance.InfoLog("View Study button clicked.");
                //PageLoadWait.WaitForPageLoad(10);
                //PageLoadWait.WaitForFrameLoad(10);
                studies.LaunchStudy();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { viewer.SelectToolInToolBar(StudyViewer.ViewerTools.Zoom); viewer.DragMovement(BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"))); }
                stopwatch.Stop();
                stopwatch.Reset();
                bool message1;
                try { message1 = BasePage.Driver.FindElement(By.CssSelector("span[id$='_LoginMasterContentPlaceHolder_ErrorMessage']")).GetAttribute("innerHTML").Equals(timeoutmessage); }
                catch (Exception) { message1 = false; }
                if (!message1)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5 Stay Idle for 2 minutes.
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                message = BasePage.Driver.FindElement(By.CssSelector("span[id$='_LoginMasterContentPlaceHolder_ErrorMessage']")).GetAttribute("innerHTML").Equals(timeoutmessage);
                if (message && (actualtimeout == timeout.Minutes))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6, 7, 8 - Not Automated
                result.steps[++executedSteps].status = "Not Automated";
                result.steps[++executedSteps].status = "Not Automated";
                result.steps[++executedSteps].status = "Not Automated";

                //Step-9 - Automated as part of before test
                executedSteps++;
                                
                //Step-10 -  Apply viewer tools for 2 minutes
                ehr.LaunchEHR();
                //ehr.SetCommonParameters(domain: "SuperAdminGroup", role: "SuperRole", user: "Administrator");
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions("Study");
                ehr.SetSearchKeys_Study(accession);
                String url_10 = ehr.clickCmdLine("ImageLoad");
                //ehr.Load();

                //Validate Study Displayed in iConnect           
                //Thread.Sleep(20000);
                //rnxobject.WaitForElementTobeVisible(rxpathmainwindow);
                //WebDocument browser = rxpathmainwindow;
                //browser.WaitForDocumentLoaded();
                login = new Login();
                login.NavigateToIntegratorURL(url_10);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                ehr.CloseEHR();
                //Thread.Sleep(5000);

                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { viewer.SelectToolInToolBar(StudyViewer.ViewerTools.Zoom); viewer.DragMovement(BasePage.Driver.FindElement(By.CssSelector("div#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_CompositeViewportDiv"))); }
                //{  RanorexObjects.ApplyTool(browser); }               
                stopwatch.Stop();
                stopwatch.Reset();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                //try { message1 = ((SpanTag)browser.FindSingle(new RxPath(".//*[@id='m_title']"))).GetInnerHtml().Contains(timeoutmessage); }
                try { message1 = viewer.AuthenticationErrorMsg().Text.ToLower().Contains(timeoutmessage); }
                catch (Exception) { message1 = false; }
                if (!message1)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-11 -- Stay Idle for 2 minutes
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                actualtimeout = stopwatch.Elapsed.Minutes;
                stopwatch.Stop();
                stopwatch.Reset();
                Thread.Sleep(20000);
                //PageLoadWait.WaitForPageLoad(10);
                //login.NavigateToIntegratorFrame();
                //rnxobject.WaitForElementTobeVisible(".//*[@id='m_title']");
                //message1 = ((SpanTag)browser.FindSingle(new RxPath(".//*[@id='m_title']"))).GetInnerHtml().Contains(timeoutmessage);
                message1 = viewer.AuthenticationErrorMsg().Text.Contains(timeoutmessage); 
                if (message1 && (actualtimeout == timeout.Minutes))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                //browser.Close();

                //Step- 12 to 17 - Not Automated (Email related, and  other are device related)
                result.steps[++executedSteps].status = "Not Automated";
                result.steps[++executedSteps].status = "Not Automated";
                result.steps[++executedSteps].status = "Not Automated";
                result.steps[++executedSteps].status = "Not Automated";
                result.steps[++executedSteps].status = "Not Automated";
                result.steps[++executedSteps].status = "Not Automated";

                //Set Session Timeout
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                TimeSpan timeout_1 = new TimeSpan(0, 30, 0);
                servicetool.SetTimeout(timeout_1.Minutes);
                servicetool.CloseServiceTool();

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                new Login();
                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Set Session Timeout
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                TimeSpan timeout_1 = new TimeSpan(0, 30, 0);
                servicetool.SetTimeout(timeout_1.Minutes);
                servicetool.CloseServiceTool();

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login = new Login();
                login.Logout();

                //Return Result
                return result;
            }


        }

        /// <summary>
        /// CD Uploader
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27841(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Taskbar taskbar = null;
            TestCaseResult result;
            StudyViewer viewer = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PhysicianUser = Config.phUserName;
                String PhysicianPassword = Config.phPassword;
                String ArchivistUser = Config.arUserName;
                String StaffUser = Config.stUserName;
                String StaffPassword = Config.stPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String AttachmentPathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentPath");
                String[] AttachmentPath = AttachmentPathList.Split('=');
                int SeriesCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfSeries"));
                String InstName = "Inst_" + new Random().Next(1000);
                String DestName = "Dest_" + new Random().Next(1000);
                String IPID = "IPID_" + new Random().Next(1000);
                String eiWindow = "ExamImporter_" + new Random().Next(1000);
                String datasource = login.GetHostName(Config.DestinationPACS).ToUpper();
                String Domain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Comments = "Test comments for Upload Entire CD";

                //Step 1 :- Initial Setup 
                ExecutedSteps++;

                //Step 2 :- Precondition - Create a set of ph, ar & st users for SuperAdmingroup and Enable some features
                ExecutedSteps++;

                //Step 3 :- Check webaccess login page is displayed or not
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Institution inst = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");

                //Search Institution
                Boolean InstFound = inst.SearchInstitution(InstName);

                if (!InstFound)
                {
                    //Add an Institution 
                    inst.CreateInstituition(InstName, IPID);
                    InstFound = inst.SearchInstitution(InstName);
                }

                //Step 4 :- Create Institution as Admin
                if (InstFound)
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

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");

                //Search Destination 
                Boolean DestFound = dest.SearchDestination(Domain, DestName);

                if (!DestFound)
                {
                    //Add an Destination 
                    //dest.CreateDestination(datasource, PhysicianUser, ArchivistUser, DestName, Domain);
                    dest.AddDestination(Domain, DestName, datasource, PhysicianUser, ArchivistUser);
                    DestFound = dest.SearchDestination(Domain, DestName);
                }

                //Get Destination List
                IList<string> DestList_1 = dest.GetDestinationList(Domain);

                //Step 5 :- Create Destination as Admin
                if (DestFound)
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

                //Logout as                
                login.Logout();

                //Step 6 :- PreCondition - Generate Exam Importer
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(Domain, eiWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                BasePage.Kill_EXEProcess("UploaderTool");
                ExecutedSteps++;

                //Delete existing installers
                new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
                {
                    if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                        File.Delete(file);
                });

                //Download CD Uploader
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());

                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    SelectElement selector = new SelectElement(login.DomainNameDropdown());
                    selector.SelectByText(Domain);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());
                }
                catch (NoSuchElementException e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }
                catch (WebDriverTimeoutException e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }

                String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (browsername.Equals("internet explorer"))
                {
                    BasePage.HandleIENotifyPopup("Save");
                }

                //Check whether the file is present
                Boolean installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");

                int counter = 0;
                while (!installerdownloaded && counter++ < 10)
                {
                    PageLoadWait.WaitForDownload(Config.eiInstaller, Config.downloadpath, "msi");
                    installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");
                    Thread.Sleep(1000);
                }

                //Step 7 :- Check installer is downloaded or not
                if (installerdownloaded)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("CDUploader installer not downloaded..");
                }

                //Launch installer tool
                login._examImporterInstance = eiWindow;
                wpfobject.InvokeApplication(Config.downloadpath + @"\" + Config.eiInstaller + ".msi");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(login._examImporterInstance + " Setup", "Cancel", 1);

                //Step 8 :- Validate "End User License Agreement" window
                if (ei.AcceptCheckbox().Visible && !ei.NextBtn().Enabled)
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

                //Click Accept and Next
                CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;
                ei.AcceptCheckbox().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                ei.NextBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                try
                {
                    //Choose install for all users and Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.InstallForAdministrator().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();

                    //Choose default destination and click Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                }
                catch (Exception) { }

                //Step 9 :- Validate User name and password fields
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                if (ei.UserNameTextbox().Visible && ei.PasswordTextbox().Visible)
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

                //Enter credentials
                ei.UserNameTextbox().BulkText = PhysicianUser;
                ei.PasswordTextbox().BulkText = PhysicianPassword;
                ei.InstallBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                //wait until installation completes
                int installWindowTimeOut = 0;
                try
                {
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    while (ei.InstallingText(eiWindow).Visible && installWindowTimeOut++ < 15)
                    {
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    if (installWindowTimeOut == 0)
                    {
                        throw new Exception("Exception in CD Uploader installation window -- " + e);
                    }
                }

                //Step 10 :- Check finish button is displayed
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                if (ei.FinishBtn().Enabled)
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

                //Uncheck "Launch application when setup exists" and click Finish
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.LaunchAppCheckbox().Click();
                ei.FinishBtn().Click();

                counter = 0;
                while (WpfObjects._mainWindow.Visible && counter++ < 20)
                {
                    Thread.Sleep(1000);
                }

                //Step 11 :- Validate installer window
                if (WpfObjects._mainWindow.IsClosed)
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

                String[] EIPath = Config.EIFilePath.Split('\\');
                EIPath[Array.FindIndex(EIPath, folder => folder.Equals("Apps")) + 1] = eiWindow;
                String UploaderToolPath = string.Join("\\", EIPath);
                ei.LaunchEI(UploaderToolPath);
                wpfobject.GetMainWindow(eiWindow);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Step 12 :- Validate user should able to give credentials
                if (ei.UserNameTextbox_EI().Visible && ei.PasswordTextbox_EI().Visible && ei.EmailTextbox_EI().Visible)
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

                //Step 13 :- Enter Credentials
                ei.UserNameTextbox_EI().BulkText = StaffUser;
                ei.PasswordTextbox_EI().BulkText = StaffPassword;
                ExecutedSteps++;

                //Step 14 & 15:- Click Sign-in and Validate user is logged in
                ei.EI_ClickSignIn(eiWindow);
                ExecutedSteps++;
                ExecutedSteps++;

                //Choose institution
                wpfobject.GetMainWindow(eiWindow);
                ei.SettingsTab().Focus();
                ei.ExistingInstitution().Click();
                ei.InstitutionDropdown().Select(InstName);
                ei.SaveBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(eiWindow);

                //Step 16 :- Verify Welcome text with username in top RHS
                wpfobject.GetMainWindow(eiWindow);
                if (ei.welcomeText(StaffUser).Visible)
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

                //Step 17 :- Check recipients section is expanded or not
                if (ei.DestinationDropdown().Visible)
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

                //Get all destinations listed
                String[] DestinationList_2 = ei.DestinationList(eiWindow);

                //Step 18 :- Valiadate all possible destinations are displayed
                counter = 0;
                if (Array.Exists(DestinationList_2, Dest => Dest.Equals(DestList_1.Cast<string>().ToArray()[counter++])))
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

                //Select Destination with physician-1 as receiver
                ei.eiWinName = eiWindow;
                //ei.DestinationDropdown().SetValue("");
                ei.DestinationDropdown().EditableText = "";
                ei.DestinationDropdown().Focus();
                ei.DestinationDropdown().Select(DestName);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Step 19 :- Valiadate selected destination is displayed
                if (ei.DestinationDropdown().SelectedItemText.Equals(DestName))
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

                //Select study in the specified path
                ei.SelectFileFromHdd(StudyPath);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Get all patient details
                string[] PatientDetails = ei.AllPatientDetails(eiWindow);

                //Step 20 :- Check patient info are displayed correctly with selected test data
                if (Array.Exists(PatientDetails, detail => detail.Contains(LastName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(FirstName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(PatientID)))
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

                //Step 21 & 22 :- Attach image
                ei.AttachImage(AttachmentPath[0]);
                ExecutedSteps++;
                Boolean status = true;

                if (status)
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

                //Step 23 & 24 :- Attach PDF
                ei.AttachPDF(AttachmentPath[1]);
                ExecutedSteps++;

                if (status)
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

                //Add comments in upload comments section
                ei.UploadComments(Comments);

                //Step 25 :- Upload comments and validate it's display
                if (ei.CommentsTextBox().Text.Equals(Comments))
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

                //Click Send
                ei.SelectAllPatientsToUpload();
                ei.Send();

                //Steps 26,27 & 28 - Click send, verify upload progress bar and click ok
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;

                //Logout and Close Exam importer
                ei.EI_Logout();
                ei.CloseUploaderTool();

                //Step 29 :- Check webaccess login page 
                if (login.UserIdTxtBox().Displayed)
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

                //Step 30 :- Login as Administrator
                login.LoginIConnect(PhysicianUser, PhysicianPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Step 31 :- Monitor adding of exam to inbounds by iCA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Search Study
                inbounds.SearchStudy("PatientID", PatientID);

                //Step 32 :- Validate study is displayed or not
                Dictionary<string, string> studyrow = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status", "Comments" }, new string[] { PatientID, "Uploaded", Comments });
                if (studyrow != null)
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

                //Select Study
                inbounds.SelectStudy1("Patient ID", PatientID);
                int Imagescount = Int32.Parse(inbounds.GetMatchingRow("Patient ID", PatientID)["Number of Images"].Split('/')[1]);

                //Step 33 :- Launch Study
                viewer = StudyViewer.LaunchStudy();
                ExecutedSteps++;

                //Get Thumbnail Caption details
                String[] CaptionDetails = viewer.CaptionDetails();

                //Step 34 :- Validate attached image is displayed as seperate series under OT modality
                if (Imagescount == SeriesCount + 2 && CaptionDetails[SeriesCount].Contains("OT"))
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

                //Navigate to history Panel
                viewer.NavigateToHistoryPanel();

                //Get Report details
                Dictionary<string, string> StudywithReport = viewer.GetMatchingRow(new string[] { "Patient ID", "Report" }, new string[] { PatientID, "Yes" });

                //Step 35 :- Validate report is displayed
                if (StudywithReport != null)
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

                //Close Study and Logout as Physician 
                PageLoadWait.WaitForFrameLoad(10);
                login.CloseStudy();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
        /// URL-EMR EI
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27842(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Outbounds outbounds = null;
            UserManagement usermanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Username = Config.stUserName;
                String Password = Config.stPassword;
                String AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                //String[] AccessionNumbers = AccessionNumber.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String PatientDOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String PatientGender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String Ipid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string windowname = Config.eiwindow;
                string DestName = Config.Dest1;
                string Comments = "Test comments for Upload TestEHR";
                String[] FieldNames = { "MRN:", "Patient Name:", "DOB:", "Gender:", "Issuer of PID:" };
                String[] FieldValues = { PatientID, PatientName, PatientDOB, "Male", Ipid };
                //step-1:Pre-Condition
                login.UncommentXMLnode("id", "Bypass");

                //Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                IList<string> DestList_1 = dest.GetDestinationList("SuperAdminGroup");

                ExecutedSteps++;


                //step-2:In Merge iConnect Access Service Tool --Integrator tab: 1.User Sharing = URL determined IISRESET
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined");
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step-3:Launch EMR application
                ehr.LaunchEHR();

                //Validate whether EHR application is launched or not
                if (WpfObjects._mainWindow.Visible)
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

                //step-4:Navigate to Launch Exam Importer tab
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Launch Exam Importer");
                wpfobject.WaitTillLoad();

                //Get Tab name
                String tabname = WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.Tab>(SearchCriteria.All).SelectedTab.Name;

                //Validate navigation of "Launch Exam importer" 
                if (tabname.Equals("Launch Exam Importer"))
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

                //step-5:Enter the following:
                //Address = http://*^<^*server IP*^>^*/WebAccess
                //User ID= st
                //Enable User Sharing = True
                //Auto End Session=True
                //Auth Provider=ByPass
                //Other fields are set to default
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", user: Config.stUserName, usersharing: "True", domain: "SuperAdminGroup");
                ITabPage currenttab = ehr.GetCurrentTabItem();
                CheckBox AuthProvider = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(currenttab, "m_authProviderCheckBox");
                if (!(AuthProvider.Checked))
                {
                    AuthProvider.Click();
                }
                ExecutedSteps++;

                //step-6:Fill in the patient contents:
                //Patient Name:"TestEHR01"
                //Patient ID:"TestEHR01"
                //Patient DOB:"2001/12/20"
                //Issuer of PatientID:"Test_TestEHR01"
                //Patient Gender:"M"
                ehr.PatientName().BulkText = PatientName;
                ehr.PatientID().BulkText = PatientID;
                ehr.PatientDOB().BulkText = PatientDOB;
                ehr.IPID().BulkText = Ipid;
                ehr.Gender().BulkText = PatientGender;
                ExecutedSteps++;

                //step-7:From a Server machine
                //Click Cmd Line button to generate the EMR link and copy the url.
                string url = ehr.clickCmdLine();
                if (url != null)
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


                //step-8:The user (st) launches the Uploader Tool desktop shortcut titled"Send Exam to --INSTITUTION_NAME--on their system.
                ei.LaunchEI(Config.EIFilePath);
                wpfobject.GetMainWindow(windowname);
                WpfObjects._mainWindow.WaitWhileBusy();
                ExecutedSteps++;

                //step-9:Do not logon. Minimize Exam Importer window.
                Button minimize = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("Minimize"));
                minimize.Click();
                ExecutedSteps++;
                ehr.CloseEHR();

                //step-10:Launch Browser copy paste the link generated in step 7 and hit Enter key.
                //url = "http://10.5.39.26/webaccess/Default.ashx?RequestClass=Integrator&AuthProvider=Bypass&AutoEndSession=True&Culture=en-us&UserID=st&EnableUserSharing=True&SecurityID=Administrator-Administrator&operation=LaunchUploader&patientID=TestEHR01&patientName=TestEHR01&patientGender=M&patientDOB=12%2f20%2f2001&patientIDIssuer=Test_TestEHR01";
                login.NavigateToIntegratorURL(url);
                //wpfobject.WaitTillLoad();
                Thread.Sleep(10000);
                WpfObjects._mainWindow = WpfObjects._application.GetWindow(SearchCriteria.ByText(windowname), InitializeOption.NoCache);
                wpfobject.GetMainWindow(windowname);
                GroupBox demographics = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("New Patient Demographics"));
                if (ei.welcomeText(Username).Visible && demographics.Visible)
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

                //stepcount-11:Check the fields displayed in New Patient Demographics section
                GroupBox demographics1 = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("New Patient Demographics"));
                IList<IUIItem> list = demographics1.GetMultiple(SearchCriteria.All);
                int fieldcount = (list.Count / 2) + 1, j, k = 0;
                string[] fields = new string[list.Count / 2];
                for (j = 1; j < fieldcount; j++, k++)
                {
                    Label labelname = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, FieldNames[k], 1);
                    fields[j - 1] = labelname.Text;
                }
                string[] values = new string[list.Count / 2];
                for (int t = 0; t < list.Count / 2; t++)
                {
                    Label labelname = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, FieldValues[t], 1);
                    values[t] = labelname.Text;
                }

                if (fields[0].Equals(FieldNames[0]) && fields[1].Equals(FieldNames[1]) && fields[2].Equals(FieldNames[2]) &&
                    fields[3].Equals(FieldNames[3]) && fields[4].Equals(FieldNames[4]) && values[0].Equals(FieldValues[0]) &&
                    values[1].Equals(FieldValues[1]) && values[2].Equals(FieldValues[2]) && values[3].Equals(FieldValues[3]) &&
                    values[4].Equals(FieldValues[4]))
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
                /*  Label mrn= wpfobject.GetAnyUIItem<GroupBox,Label>(demographics1,"MRN:",1);
                  Label name = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Patient Name:", 1);
                  Label dob = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "DOB:", 1);
                  Label gender = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Gender:", 1);
                  Label ipid = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Issuer of PID:", 1);                 
                  IUIItem mrnvalue = wpfobject.GetAnyUIItem<GroupBox, IUIItem>(demographics1, "TestEHR01:", 1);
                  Label namevalue = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "TestEHR01:", 1);
                  TextBox dobvalue = wpfobject.GetAnyUIItem<GroupBox, TextBox>(demographics1, "12/20/2001:", 1);
                  Label gendervalue = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Male:", 1);
                  Label ipidvalue = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Test_TestEHR01:", 1);
                  if(mrn!=null&& name != null && dob != null && gender != null && ipid != null &&
                      mrnvalue.GetType().Name.Equals(PatientID) && namevalue.Text.Equals(PatientName) && dobvalue.Text.Equals(PatientDOB)
                      && ipidvalue.Text.Equals(Ipid) )
                  {
                      result.steps[++ExecutedSteps].status = "Pass";
                      Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                  }
                  else
                  {
                      result.steps[++ExecutedSteps].status = "Fail";
                      Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                      result.steps[ExecutedSteps].SetLogs();
                  }*/


                //step-12:Check the New MRN button
                TextBox newMrn = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("TxtDestinationMrn"));
                if (newMrn.Enabled == false)
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
                //Get all destinations listed
                String[] DestinationList = ei.DestinationList(windowname);
                int i = 0;
                foreach (string destlist in DestinationList)
                {
                    DestinationList[i] = destlist.ToUpper();
                    i++;
                }

                //Step 13 :- In"To"section Uploader Tool presents the list of possible destinations.

                int counter = 0;

                if (Array.Exists(DestinationList, Dest => Dest.Equals(DestList_1.Cast<string>().ToArray()[counter++])))
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

                //step-14:User  selects the appropriate destination
                ei.eiWinName = windowname;
                //ei.DestinationDropdown().SetValue("");
                //ei.DestinationDropdown().EditableText = "";
                //ei.DestinationDropdown().Focus();
                //ei.DestinationDropdown().Select(DestName);
                //WpfObjects._mainWindow.WaitWhileBusy();
                ei.EI_SelectDestination(DestName);

                //  Valiadate selected destination is displayed
                if (ei.DestinationDropdown().SelectedItemText.Equals(DestName))
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

                //Step 15 : Select study in the specified path
                ei.SelectFileFromHdd(StudyPath);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Get all patient details
                string[] PatientDetails = ei.AllPatientDetails(windowname);

                //Check patient info are displayed correctly with selected test data
                if (Array.Exists(PatientDetails, detail => detail.Contains(LastName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(FirstName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(PatientID)))
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



                //Step 16 :- Upload comments and validate it's display

                //Add comments in upload comments section
                ei.UploadComments(Comments);
                if (ei.CommentsTextBox().Text.Equals(Comments))
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

                //Step-17: User click on the Patient in select Patient drop down
                if (!Array.Exists(PatientDetails, detail => detail.Contains('^'))
                                    && !Array.Exists(PatientDetails, detail => String.IsNullOrEmpty(detail)))
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

                //Click Send
                ei.SelectAllPatientsToUpload();
                ei.Send();

                //Steps 18,19 & 20 - Click send, verify upload progress bar and click ok
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                //Logout and Close Exam importer
                ei.EI_Logout();
                ei.CloseUploaderTool();
                login.CloseBrowser();

                //Step 21 :- Check webaccess login page 
                //BasePage.Driver.Navigate().GoToUrl(Config.IConnectIP+"/webaccess");
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);
                if (login.UserIdTxtBox().Displayed)
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

                //Step 22 :- Login as Administrator
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                //Step-23:Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");
                ExecutedSteps++;

                //Step 24 :- Validate study is displayed or not
                //StepSearch Study
                outbounds.SearchStudy("PatientID", PatientID);
                outbounds.ChooseColumns(new String[] { "Comments" });
                Dictionary<string, string> studyrow = outbounds.GetMatchingRow(new string[] { "Patient ID", "Comments" }, new string[] { PatientID, Comments });
                if (studyrow != null)
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


                //Logout as PACS user               
                login.Logout();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
        /// URL-EMR WebUploader
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27843(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Outbounds outbounds = null;
            Studies studies = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            //User Credentials
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String arUser = Config.ar1UserName;
            String arPassword = Config.ar1Password;
            String DefaultBrowser = Config.BrowserType;

            try
            {
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientDOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String PatientGender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Domain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String Comments = "Test comments for Upload TestEHR";
                String IPID = Config.ipid1;
                String DestName = Config.Dest1;
                String datasource = login.GetHostName(Config.DestinationPACS).ToUpper();

                String[] PatientFieldValues = { LastName + "^" + FirstName, PatientID, PatientDOB, IPID, PatientGender };

                //Pre-Condition
                login.UncommentXMLnode("id", "Bypass");

                //Switch to Firefox browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = "firefox";
                login = new Login();
                login.DriverGoTo(login.url);

                login.LoginIConnect(arUser, arPassword);

                //Step-1:Set userpreference to Launch Webuploader
                studies = (Studies)login.Navigate("Studies");

                //Open Preferences and Check the "Make Java Exam Importer as default Exam Importer" checkbox
                studies.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#DefaultToJavaExamImporterDiv")));
                if (BasePage.Driver.FindElement(By.CssSelector("input#defaultToJavaExamImporterCB")).Selected == false)
                {
                    studies.SetCheckbox("cssselector", "input#defaultToJavaExamImporterCB");
                }

                //Close Preferences
                studies.CloseUserPreferences();

                //Logout
                login.Logout();

                //Step 1 (PreCondition) :- Enable ByPass mode
                ExecutedSteps++;

                //Step 2 (PreCondition) :-  Enable User sharing.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined");
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Clear Cookies
                BasePage.Driver.Manage().Cookies.DeleteAllCookies();

                //Launch EMR application
                ehr.LaunchEHR();

                //Step 3 :- Validate whether EHR application is launched or not
                if (WpfObjects._mainWindow.Visible)
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

                //Navigate to Launch Exam Importer tab
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Launch Exam Importer");
                wpfobject.WaitTillLoad();

                //Get Tab name
                String tabname = WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.Tab>(SearchCriteria.All).SelectedTab.Name;

                //Step 4 :- Validate navigation of "Launch Exam importer" 
                if (tabname.Equals("Launch Exam Importer"))
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

                //Step 5 :- Set Common Parameters
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", domain: Domain, role: "Archivist", user: arUser, usersharing: "True");
                ExecutedSteps++;

                //Step 6 :- Set Patient field values and validate
                ehr.PatientName().BulkText = PatientFieldValues[0];
                ehr.PatientID().BulkText = PatientFieldValues[1];
                ehr.PatientDOB().BulkText = PatientFieldValues[2];
                ehr.IPID().BulkText = PatientFieldValues[3];
                ehr.Gender().BulkText = PatientFieldValues[4];
                ExecutedSteps++;

                //Click Cmd Line button and get eHR url
                String eHRurl = ehr.clickCmdLine();

                //Step 7 :- Validate clicking command line generates an url or not
                if (!String.IsNullOrEmpty(eHRurl))
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

                //Close eHR
                ehr.CloseEHR();

                //Copy that url in browser and click enter
                login.NavigateToIntegratorURL(eHRurl);

                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("ToDestination"));

                //Step 8 :- Check webuploader window is displayed with new patient demographics section
                if (webuploader.NewPatientDemographics().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("WebUploader not gets loaded");
                }

                //Get Patient DOB
                String date = webuploader.PatientDOBTxt().TextValue.Replace(", ", ",");
                System.DateTime wuDOB = System.DateTime.ParseExact(date, "MMM dd,yyyy", CultureInfo.InvariantCulture);
                System.DateTime ehrDOB = System.DateTime.ParseExact(PatientDOB, "dd-MMM-yyyy", CultureInfo.InvariantCulture);

                //Step 9 :- Validate Webuploader gets loaded with entered details corrrectly
                if (webuploader.PatientNameTxt().TextValue.Trim().Equals(PatientFieldValues[0].Replace('^', ' ')) &&
                    webuploader.IPIDTxt().TextValue.Trim().Equals(IPID) && wuDOB.Equals(ehrDOB) &&
                    webuploader.PatientGenderTxt().TextValue.Trim().StartsWith(PatientGender) &&
                    webuploader.PatientMRNTxt().TextValue.Trim().Equals(PatientID))
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

                //Step 10 :- Verify that new MRN textbox is disabled
                if (!webuploader.NewMRNTxt().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("WebUploader not gets loaded");
                }

                //Take Destination from dropdown
                rnxobject.Click(webuploader.ToDestination());
                rnxobject.WaitForElementTobeVisible("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']");
                Ranorex.ListItem Destination = webuploader.GetDestination(DestName);

                //Step 11 :- Check the listed possible destinations 
                if (Destination != null)
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

                //Select Destination
                rnxobject.Click(webuploader.ToDestination());
                rnxobject.Click(Destination);

                //Step 12 :- Validate selected destination is displayed 
                if (webuploader.ToDestination().FindSingle<Ranorex.Text>(".//text").TextValue.Equals(DestName))
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

                //Select study in the specified location
                webuploader.SelectFileFromHdd(StudyPath);

                //Step 13 :- Validate selected patient details are displayed 
                if (webuploader.PatientDetailLabel().TextValue.Contains(LastName) && webuploader.PatientDetailLabel().TextValue.Contains(FirstName)
                     && webuploader.PatientDetailLabel().TextValue.Contains(PatientID))
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

                //Enter Comments
                webuploader.CommentsTxtbox().TextValue = Comments;

                //Step 14 :- Validate text in the comments box
                if (webuploader.CommentsTxtbox().TextValue.Equals(Comments))
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

                //Click Patient Detail dropdown
                rnxobject.Click(webuploader.PatientDetailLabel());
                rnxobject.WaitForElementTobeVisible("/form[@processname='jp2launcher']/?/?/list[@type='JList']");

                //Get Patient details List
                IList<Ranorex.ListItem> List = webuploader.PatientsList().Items;

                //Step 15 :- Validate that the patient details in drop down should not contain '^' / null string
                ExecutedSteps++;
                foreach (Ranorex.ListItem item in List)
                {
                    if (!item.Text.Contains('^') && !String.IsNullOrEmpty(item.Text))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                //Step 16 :- Check every checkbox before the study in Patient list
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                //rnxobject.Click(webuploader.SendBtn());
                Ranorex.Mouse.ScrollWheel(-20.0);
                webuploader.SendBtn().EnsureVisible();
                rnxobject.Click(webuploader.SendBtn());

                //Sync - up
                int timeout = 0;
                while (timeout++ < 20)
                {
                    try
                    {
                        if (!webuploader.UploadProgressOKBtn().Visible)
                        {
                            Thread.Sleep(1000);
                            Logger.Instance.InfoLog("Waiting for upload progress pop up to display");
                        }
                        else { break; }
                    }
                    catch (Exception) { Thread.Sleep(1000); }
                }

                //Step 17 :- Validate Upload progress window is displayed with OK button
                if (webuploader.UploadProgressOKBtn().Visible)
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

                //Sync - up
                timeout = 0;
                while (timeout++ < 30)
                {
                    try
                    {
                        if (!webuploader.UploadProgressOKBtn().Enabled)
                        {
                            Thread.Sleep(1000);
                            Logger.Instance.InfoLog("Waiting for studies to be uploaded");
                        }
                        else { break; }
                    }
                    catch (Exception) { Thread.Sleep(1000); }
                }

                //Step 18 :- Validate "Study Uploaded successfully" message displayed
                if (webuploader.UploadProgressOKBtn().Enabled)
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

                // Click Ok button and study should reach iCA
                rnxobject.Click(webuploader.UploadProgressOKBtn());

                //Navigate to iConnect url
                if (BasePage.Driver.WindowHandles.Count != 1)
                {
                    webuploader.WUMainForm().Close();
                }
                login.DriverGoTo(login.url);

                //Step 19 :- Check webaccess login page is displayed or not
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Step 20 :- Login as Physician-2
                login.LoginIConnect(arUser, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Step 21 :- Monitor adding of exam to inbounds by iCA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Search Study
                outbounds.SearchStudy(LastName: LastName, FirstName: FirstName, patientID: PatientID,
                    Gender: PatientGender, IPID: IPID, DOB: PatientDOB, Datasource: "", Date: "");

                //Choose Columns
                outbounds.ChooseColumns(new string[] { "Comments", "Status" });

                //Step 22 :- Validate study is displayed or not
                PageLoadWait.WaitforStudyInStatus(Accession, new Inbounds(), "Study Reconciled");
                Dictionary<string, string> studyrow = outbounds.GetMatchingRow(new string[] { "Patient ID", "Status", "Comments" }, new string[] { PatientID, "Study Reconciled", Comments });
                if (studyrow != null)
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

                //Logout 
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
            finally
            {
                //Switch back to Default browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = DefaultBrowser;
                login.InvokeBrowser(DefaultBrowser);
                login.DriverGoTo(login.url);
                Thread.Sleep(10000);
                try
                {
                    Window pluginWindow = new WpfObjects().GetMainWindowByTitle("Plugin Container for Firefox");
                    pluginWindow.Get<Button>(SearchCriteria.ByText("Debug the program")).Click();
                    BasePage.KillProcess("plugin-container");
                }
                catch (Exception) { }
            }
        }

        /// <summary> 
        /// This Test case is to Nominate and Archive a Study.
        /// </summary>
        public TestCaseResult Test_27844(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int executedSteps = -1;

            //Set validation steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch Test Data
                String phusername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String nominationreason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String orderasccession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String updatedgender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String iconnectstudypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String pacsstudypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PACSStudyPath");
                String iconnectaccession = AccessionID.Split(':')[0];
                String pacsaccession = AccessionID.Split(':')[1];
                Boolean studyfound = false;

                String currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                /*Precondition -- Load a Study to iConnect,
                Load Stucy to Destination PACs
                Send Order to MWL Pacs*/
                ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, iconnectstudypath, 1);
                BasePage.RunBatchFile(Config.batchfilepath, currentDirectory + Path.DirectorySeparatorChar + pacsstudypath + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                new Inbounds().SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), currentDirectory + Path.DirectorySeparatorChar + orderpath);

                //User Laucnghes the browser - Step-1
                executedSteps++;

                //Login as physician, select Study-Step2
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", iconnectaccession);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { iconnectaccession, "Uploaded" });
                executedSteps++;

                //Nominate study -- Step-3, 4
                inbounds.NominateForArchive("Testing");
                executedSteps++;
                executedSteps++;

                //Logout ICA -Step-5
                login.Logout();
                executedSteps++;

                //Login as AR -Step-6
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);
                executedSteps++;

                //Select Study and Click Archive Button -- Step-7
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", iconnectaccession);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { iconnectaccession, "Nominated For Archive" });
                BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationControlDialogDiv")));
                executedSteps++;

                //Perform Patient Search - Step-8
                inbounds.ArchiveSearch("Patient", Lastname: lastname);
                BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_ButtonShowAll")).Click();
                var results = BasePage.GetSearchResultsinReconcile();
                inbounds.ClickOkInShowAll();
                foreach (String value in results[0]) { if (value.Contains(lastname)) { studyfound = true; } }
                if (studyfound)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Perform Order Search -Step-9
                inbounds.ArchiveSearch("Order", Lastname: lastname);
                BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_ButtonShowAll")).Click();
                results = BasePage.GetSearchResultsinReconcile();
                inbounds.ClickOkInShowAll();
                if (results[0].Contains(orderasccession))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Update PID - Step-10
                String newpid = "PID" + new Random(1000).Next();
                inbounds.EditFinalDetailsInArchive("PID", newpid);
                executedSteps++;

                //Update Patient/Study attribute - Step-11
                String newstudydesc = "Desc" + new Random(1000).Next();
                inbounds.EditFinalDetailsInArchive("Description", newstudydesc);
                inbounds.EditFinalDetailsInArchive("Gender", updatedgender);
                executedSteps++;

                //Archive Study - Step-12
                inbounds.ClickArchive();
                login.Logout();
                login.DriverGoTo(login.mpacdesturl);
                MPHomePage mphphome = (MPHomePage)mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool mptool = (Tool)mphphome.NavigateTopMenu("Tools");
                mptool.NavigateToSendStudy();
                mptool.SearchStudy("Accession", iconnectaccession, 0);
                var mpacresult = Tool.MPacGetSearchResults();
                mpaclogin.LogoutPacs();
                if (mpacresult != null)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //ICEA Notification-Step-13
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", iconnectaccession);
                var archivedstudy = inbounds.GetMatchingRow(new String[] { "Accession", "Status" }, new String[] { iconnectaccession, "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Web Uploader
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27845(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Domain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String Comments = "Test comments for Upload Entire CD";

                //PrecCondition :- Switch to Firefox browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("firefox");
                login.DriverGoTo(login.url);

                //Step 1 :- Check webaccess login page is displayed with CD uploader install button and web upload button
                if (login.CDUploaderInstallBtn().Enabled && login.WebUploadBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Click Web Upload button
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.WebUploadBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.WebUploadBtn());

                try
                {
                    //Choose domain if multiple domain exists
                    //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));

                    SelectElement selector = new SelectElement(login.DomainNameDropdown());
                    selector.SelectByText(Domain);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());

                }
                catch (WebDriverTimeoutException e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }

                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("LoginUserName"));

                //Step 2 :- Check webuploader window is displayed
                if (webuploader.LoginPanel().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Login as anonymous user
                webuploader.UserNameTxt().TextValue = Config.stUserName;
                webuploader.PasswordTxt().TextValue = Config.stPassword;
                rnxobject.WaitForElementTobeEnabled(webuploader.SignInBtn());
                rnxobject.Click(webuploader.SignInBtn());

                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("ToDestination"));

                webuploader.PriorityBox().Click();
                //Step 3 :- Validate all sections in webuploader are displayed
                if (webuploader.ToDestination().Enabled && webuploader.DefaultRecipientTxt().TextValue != null
                    && webuploader.AddittionalRecipientTxt().TextValue != null
                    && webuploader.PriorityBox().FindSingle<Ranorex.Text>(".//text").TextValue.Equals("ROUTINE")
                    && webuploader.StudyTable().Enabled && webuploader.CommentsTxtbox().Enabled
                    && webuploader.SendBtn().Enabled && webuploader.ClearBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Select Destination
                webuploader.SelectDestination(Config.Dest1);

                //Set Priority
                webuploader.SelectPriority("ROUTINE");

                //Select study in the specified location
                webuploader.SelectFileFromHdd(StudyPath);

                //Enter Comments
                webuploader.CommentsTxtbox().TextValue = Comments;

                //Step 4 :- 
                if (webuploader.PatientDetailLabel().TextValue.Contains(LastName) && webuploader.PatientDetailLabel().TextValue.Contains(FirstName)
                     && webuploader.PatientDetailLabel().TextValue.Contains(PatientID) && webuploader.CommentsTxtbox().TextValue.Equals(Comments))
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

                //Step 5 :- Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                Ranorex.Mouse.ScrollWheel(-20.0);
                webuploader.SendBtn().EnsureVisible();
                rnxobject.Click(webuploader.SendBtn());
                ExecutedSteps++;

                //Close Web Uploader
                webuploader.CloseUploader();

                //Login as Physician
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("PatientID", PatientID);

                //Step 6 :- Validate study is displayed or not
                Dictionary<string, string> studyrow = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status", "Comments" }, new string[] { PatientID, "Uploaded", Comments });
                if (studyrow != null)
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

                //Logout 
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
            finally
            {
                //Switch back to Default browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser(login.browserName);
                login.DriverGoTo(login.url);
                Thread.Sleep(10000);
                try
                {
                    Window pluginWindow = new WpfObjects().GetMainWindowByTitle("Plugin Container for Firefox");
                    pluginWindow.Get<Button>(SearchCriteria.ByText("Debug the program")).Click();
                    BasePage.KillProcess("plugin-container");
                }
                catch (Exception) { }
            }
        }

        /// <summary> 
        /// Multiple Domain setup
        /// </summary>
        public TestCaseResult Test_27846(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            DomainManagement domainmgmt = null;
            RoleManagement rolemgmt = null;
            UserManagement usermgmt = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int executedSteps = -1;
            Random randomnumber = new Random();

            //Domain-1 Users and Role
            Dictionary<object, string> domainattr1;
            String D1Physician = "Physician1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1Archivist = "Archivist1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1Staff = "Staff1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1ph = "ph1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1ar = "ar1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1st = "st1" + new System.DateTime().Second + randomnumber.Next(1, 1000);

            //Domain-2 Users and Role
            Dictionary<object, string> domainattr2;
            String D2Physician = "Physician2" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D2Archivist = "Archivist2" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D2Staff = "Staff2" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D2ph = "ph2" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D2ar = "ar2" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D2st = "st2" + new System.DateTime().Second + randomnumber.Next(1, 1000);


            //Set validation steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch Test Data -- NA                    

                //Login into iConnect and Create Domain-1, role and users - Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainattr1 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr1);
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, "Physician");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Archivist, "Archivist");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Staff, "Staff");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D1ph, domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, 1, Config.emailid, 1, D1ph);
                usermgmt.CreateUser(D1ar, domainattr1[DomainManagement.DomainAttr.DomainName], D1Archivist, 1, Config.emailid, 1, D1ar);
                usermgmt.CreateUser(D1st, domainattr1[DomainManagement.DomainAttr.DomainName], D1Staff, 1, Config.emailid, 1, D1st);
                executedSteps++;

                //Step-2-Create Domain-2, Users and role
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainattr2 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr2);
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr2[DomainManagement.DomainAttr.DomainName], D2Physician, "Physician");
                rolemgmt.CreateRole(domainattr2[DomainManagement.DomainAttr.DomainName], D2Archivist, "Archivist");
                rolemgmt.CreateRole(domainattr2[DomainManagement.DomainAttr.DomainName], D2Staff, "Staff");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D2ph, domainattr2[DomainManagement.DomainAttr.DomainName], D2Physician, 1, Config.emailid, 1, D2ph);
                usermgmt.CreateUser(D2ar, domainattr2[DomainManagement.DomainAttr.DomainName], D2Archivist, 1, Config.emailid, 1, D2ar);
                usermgmt.CreateUser(D2st, domainattr2[DomainManagement.DomainAttr.DomainName], D2Staff, 1, Config.emailid, 1, D2st);
                executedSteps++;

                //Step-3-Enable Features in Edit Domain-1 (Other Preonditions are already enabled in environment setup)
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainmgmt.SearchDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domainmgmt.SelectDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domainmgmt.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(10);
                domainmgmt.SetCheckbox("cssselector", "input[id$='_DataTransferEnabledCB']");
                domainmgmt.SetCheckbox("cssselector", "input[id$='_DataDownloadEnabledCB']");
                domainmgmt.SetCheckbox("cssselector", "input[id$='_GrantAccessEnabledCB']");
                domainmgmt.ClickSaveEditDomain();

                //Enable Features in Edit Domain-2
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainmgmt.SearchDomain(domainattr2[DomainManagement.DomainAttr.DomainName]);
                domainmgmt.SelectDomain(domainattr2[DomainManagement.DomainAttr.DomainName]);
                domainmgmt.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(10);
                domainmgmt.SetCheckbox("cssselector", "input[id$='_DataTransferEnabledCB']");
                domainmgmt.SetCheckbox("cssselector", "input[id$='_DataDownloadEnabledCB']");
                domainmgmt.SetCheckbox("cssselector", "input[id$='_GrantAccessEnabledCB']");
                domainmgmt.ClickSaveEditDomain();
                login.Logout();
                executedSteps++;

                //Step-4
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                executedSteps++;

                //Step-5
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Institution inst = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");
                inst.CreateInstituition();
                executedSteps++;

                //Step-6-Create Another institution
                inst.CreateInstituition();
                executedSteps++;

                //Step-7- Create Another institution
                inst.CreateInstituition();
                executedSteps++;

                //Step-8 - Create Destination - SuperAdminGroup
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.SelectDomain("SuperAdminGroup");
                //This step is performed as part of initial setup                
                executedSteps++;
                result.steps[executedSteps].status = "Not Automated";

                //Step-9 - Create Destination - Domain-1
                dest.SelectDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                dest.CreateDestination(dest.GetHostName(Config.DestEAsIp), D1ph, D1ar, domain: domainattr1[DomainManagement.DomainAttr.DomainName]);
                executedSteps++;

                //Step-10 - Create Destination - Domain-2
                dest.SelectDomain(domainattr2[DomainManagement.DomainAttr.DomainName]);
                dest.CreateDestination(dest.GetHostName(Config.DestEAsIp), D2ph, D2ar, domain: domainattr2[DomainManagement.DomainAttr.DomainName]);
                executedSteps++;

                //Step-11 - Lauch Service Tool and Generate Installer
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain("SuperAdminGroup");
                executedSteps++;

                //Step-12    
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                executedSteps++;

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Multiple Domain - CDUploader
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27847(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Taskbar taskbar = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            DomainManagement domainmgmt = null;
            RoleManagement rolemgmt = null;
            UserManagement usermgmt = null;
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Comments = "Test comments for Upload Entire CD";
                String datasource = login.GetHostName(Config.DestinationPACS).ToUpper();
                int randomnumber = new Random().Next(1, 1000);
                String eiWindow = "ExamImporter_" + randomnumber;
                String InstName = "Inst_" + randomnumber;
                String IPID = "IPID_" + randomnumber;
                String DestName = "Dest_" + randomnumber;

                //Domain-1 Users and Role
                Dictionary<object, string> domainattr1;
                String D1Physician = "Physician1" + randomnumber;
                String D1Archivist = "Archivist1" + randomnumber;
                String D1Staff = "Staff1" + randomnumber;
                String D1ph = "ph1" + randomnumber;
                String D1ar = "ar1" + randomnumber;
                String D1st = "st1" + randomnumber;

                //Pre-Condition
                //Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);

                //Create a domain with Physicain,archivist  and Staff users
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainattr1 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr1);
                String Domain = domainattr1[DomainManagement.DomainAttr.DomainName];
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(Domain, D1Physician, "Physician");
                rolemgmt.CreateRole(Domain, D1Archivist, "Archivist");
                rolemgmt.CreateRole(Domain, D1Staff, "Staff");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D1ph, Domain, D1Physician, 1, Config.emailid, 1, D1ph);
                usermgmt.CreateUser(D1ar, Domain, D1Archivist, 1, Config.emailid, 1, D1ar);
                usermgmt.CreateUser(D1st, Domain, D1Staff, 1, Config.emailid, 1, D1st);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Institution inst = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");

                //Add an Institution 
                inst.CreateInstituition(InstName, IPID);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");

                //Add Destination
                dest.CreateDestination(datasource, D1ph, D1ar, DestName, Domain);

                //Logout iCA                
                login.Logout();

                //Generate Exam Importer for non default domain
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(Domain, eiWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();

                //Step 1 :- Check webaccess login page is displayed or not
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Delete existing installers
                new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
                {
                    if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                        File.Delete(file);
                });

                //Click Install button
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#ImageSharingDomainsDiv")));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));

                //Step 2 :- Validate choose domain option is enabled
                if (login.ChooseDomainGoBtn().Displayed)
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

                SelectElement selector = new SelectElement(login.DomainNameDropdown());
                selector.SelectByText(Domain);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());

                String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (browsername.Equals("internet explorer"))
                {
                    BasePage.HandleIENotifyPopup("Run");
                }

                //Check whether the file is present
                Boolean installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");

                int counter = 0;
                while (!installerdownloaded && counter++ < 10)
                {
                    PageLoadWait.WaitForDownload(Config.eiInstaller, Config.downloadpath, "msi");
                    installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");
                    Thread.Sleep(1000);
                }

                //Step 3 :- Check installer is downloaded or not
                if (installerdownloaded)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("CDUploader installer not downloaded..");
                }

                //Launch installer tool
                login._examImporterInstance = eiWindow;
                wpfobject.InvokeApplication(Config.downloadpath + @"\" + Config.eiInstaller + ".msi");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(login._examImporterInstance + " Setup", "Cancel", 1);

                //Step 4 :- Validate "End User License Agreement" window
                if (ei.AcceptCheckbox().Visible && !ei.NextBtn().Enabled)
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

                //Click Accept and Next
                CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;
                ei.AcceptCheckbox().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                ei.NextBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                try
                {
                    //Choose install for all users and Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.InstallForAdministrator().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();

                    //Choose default destination and click Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                }
                catch (Exception) { }

                //Step 5 :- Validate User name and password fields
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                if (ei.UserNameTextbox().Visible && ei.PasswordTextbox().Visible)
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

                //Enter credentials
                ei.UserNameTextbox().BulkText = D1ph;
                ei.PasswordTextbox().BulkText = D1ph;
                ei.InstallBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                //wait until installation completes
                int installWindowTimeOut = 0;
                try
                {
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    while (ei.InstallingText(eiWindow).Visible && installWindowTimeOut++ < 15)
                    {
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    if (installWindowTimeOut == 0)
                    {
                        throw new Exception("Exception in CD Uploader installation window -- " + e);
                    }
                }

                //Step 6 :- Check finish button is displayed
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                if (ei.FinishBtn().Enabled)
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

                //Uncheck "Launch application when setup exists" and click Finish
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.LaunchAppCheckbox().Click();
                ei.FinishBtn().Click();

                counter = 0;
                while (WpfObjects._mainWindow.Visible && counter++ < 20)
                {
                    Thread.Sleep(1000);
                }

                //Step 7 :- Validate installer window
                if (WpfObjects._mainWindow.IsClosed)
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

                String[] EIPath = Config.EIFilePath.Split('\\');
                EIPath[Array.FindIndex(EIPath, folder => folder.Equals("Apps")) + 1] = eiWindow;
                String UploaderToolPath = string.Join("\\", EIPath);
                ei.LaunchEI(UploaderToolPath);
                wpfobject.GetMainWindow(eiWindow);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Step 8 :- Verify CD uploader window is launched
                if (WpfObjects._mainWindow.Visible)
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

                wpfobject.GetMainWindow(eiWindow);
                //Step 9 :- Validate user should able to give credentials
                if (ei.UserNameTextbox_EI().Visible && ei.PasswordTextbox_EI().Visible && ei.EmailTextbox_EI().Visible)
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

                //Step 10 :- Enter Credentials
                ei.UserNameTextbox_EI().BulkText = D1st;
                ei.PasswordTextbox_EI().BulkText = D1st;
                ExecutedSteps++;

                //Step 11 :- Click Sign-in and Validate user is logged in
                ei.EI_ClickSignIn(eiWindow);
                ExecutedSteps++;

                //Choose institution
                wpfobject.GetMainWindow(eiWindow);
                ei.SettingsTab().Focus();
                ei.ExistingInstitution().Click();
                ei.InstitutionDropdown().Select(InstName);
                ei.SaveBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(eiWindow);

                //Step 12 & 13:- Verify Welcome text with username in top RHS / User loggon status
                wpfobject.GetMainWindow(eiWindow);
                if (ei.welcomeText(D1st).Visible)
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
                ExecutedSteps++;

                //Step 14 :- Check recipients section is expanded or not
                if (ei.DestinationDropdown().Visible)
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

                //Get all destinations listed
                String[] DestinationList = ei.DestinationList(eiWindow);

                //Step 15 :- Valiadate all possible destinations are displayed
                counter = 0;
                if (Array.Exists(DestinationList, Dest => Dest.Contains(DestName)))
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

                //Select Destination with physician-1 as receiver
                ei.eiWinName = eiWindow;
                //ei.DestinationDropdown().SetValue("");
                ei.DestinationDropdown().EditableText = "";
                ei.DestinationDropdown().Focus();
                ei.DestinationDropdown().Select(DestName);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Step 16 :- Valiadate User able to select a destination
                if (ei.DestinationDropdown().SelectedItemText.Equals(DestName))
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

                //Select study in the specified path
                ei.SelectFileFromHdd(StudyPath);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Get all patient details
                string[] PatientDetails = ei.AllPatientDetails(eiWindow);

                //Step 17 :- Check patient info are displayed correctly with selected test data
                if (Array.Exists(PatientDetails, detail => detail.Contains(LastName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(FirstName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(PatientID)))
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

                //Add comments in upload comments section
                ei.UploadComments(Comments);

                //Step 18 :- Upload comments and validate it's display
                if (ei.CommentsTextBox().Text.Equals(Comments))
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

                //Click Send
                ei.SelectAllPatientsToUpload();
                ei.Send();

                //Steps 19,20 & 21 - Click send, verify upload progress bar and click ok
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;

                //Logout and Close Exam importer
                ei.EI_Logout();
                ei.CloseUploaderTool();

                //Step 22 :- Check webaccess login page 
                if (login.UserIdTxtBox().Displayed)
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

                //Step 23 :- Login as Administrator
                login.LoginIConnect(D1ph, D1ph);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Step 24 :- Monitor adding of exam to inbounds by iCA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Search Study
                inbounds.SearchStudy("PatientID", PatientID);

                //Step 25 :- Validate study is displayed or not
                Dictionary<string, string> studyrow = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status", "Comments", "From Institution(s)" }, new string[] { PatientID, "Uploaded", Comments, InstName });
                if (studyrow != null)
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

                //Close Study and Logout as Physician 
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
        /// Multiple Domain WebUploader
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27848(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            StudyViewer viewer = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            DomainManagement domainmgmt = null;
            RoleManagement rolemgmt = null;
            UserManagement usermgmt = null;
            int ExecutedSteps = -1;
            String DefaultBrowser = Config.BrowserType;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                int SeriesCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfSeries"));
                String AttachmentPathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentPath");
                String[] AttachmentPath = AttachmentPathList.Split('=');
                String Comments = "Test comments for Upload Entire CD";
                String datasource = login.GetHostName(Config.DestinationPACS).ToUpper();
                int randomnumber = new Random().Next(1, 1000);
                String eiWindow = "ExamImporter_" + randomnumber;
                String InstName = "Inst_" + randomnumber;
                String IPID = "IPID_" + randomnumber;
                String DestName = "Dest_" + randomnumber;

                //PrecCondition :- Switch to Firefox browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = "firefox";
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);

                //Domain-1 Users and Role
                Dictionary<object, string> domainattr1;
                String D1Physician = "Physician1" + randomnumber;
                String D1Archivist = "Archivist1" + randomnumber;
                String D1Staff = "Staff1" + randomnumber;
                String D1ph = "ph1" + randomnumber;
                String D1ar = "ar1" + randomnumber;
                String D1st = "st1" + randomnumber;

                //Domain-2 Users and Role
                Dictionary<object, string> domainattr2;
                String D2Physician = "Physician2" + randomnumber;
                String D2Archivist = "Archivist2" + randomnumber;
                String D2Staff = "Staff2" + randomnumber;
                String D2ph = "ph2" + randomnumber;
                String D2ar = "ar2" + randomnumber;
                String D2st = "st2" + randomnumber;

                //Pre-Condition
                //Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to domain management
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");

                //Create domain-1 with Physicain,archivist  and Staff users
                domainattr1 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr1);
                String Domain = domainattr1[DomainManagement.DomainAttr.DomainName];
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, "Physician");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Archivist, "Archivist");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Staff, "Staff");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D1ph, domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, 1, Config.emailid, 1, D1ph);
                usermgmt.CreateUser(D1ar, domainattr1[DomainManagement.DomainAttr.DomainName], D1Archivist, 1, Config.emailid, 1, D1ar);
                usermgmt.CreateUser(D1st, domainattr1[DomainManagement.DomainAttr.DomainName], D1Staff, 1, Config.emailid, 1, D1st);

                //Create domain-2 with Physicain,archivist  and Staff users
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainattr2 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr2);
                String Domain2 = domainattr2[DomainManagement.DomainAttr.DomainName];
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(Domain2, D2Physician, "Physician");
                rolemgmt.CreateRole(Domain2, D2Archivist, "Archivist");
                rolemgmt.CreateRole(Domain2, D2Staff, "Staff");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D2ph, Domain2, D2Physician, 1, Config.emailid, 1, D2ph);
                usermgmt.CreateUser(D2ar, Domain2, D2Archivist, 1, Config.emailid, 1, D2ar);
                usermgmt.CreateUser(D2st, Domain2, D2Staff, 1, Config.emailid, 1, D2st);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Institution inst = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");

                //Add an Institution 
                inst.CreateInstituition(InstName, IPID);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");

                //Add Destination
                dest.CreateDestination(datasource, D1ph, D1ar, DestName, Domain);
                dest.CreateDestination(datasource, D2ph, D2ar, DestName, Domain2);

                //Logout iCA                
                login.Logout();

                //Step 1 :- Check webaccess login page is displayed or not
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Click Web Upload button
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.WebUploadBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.WebUploadBtn());

                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#ImageSharingDomainsDiv")));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));

                //Step 2 :- Validate choose domain option is enabled
                if (login.ChooseDomainGoBtn().Displayed)
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

                SelectElement selector = new SelectElement(login.DomainNameDropdown());
                selector.SelectByText(Domain2);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());

                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("LoginUserName"));

                //Step 3 :- Check webuploader window is displayed
                if (webuploader.LoginPanel().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Step 4 :- Validate user Credentials
                if (webuploader.UserNameTxt().Enabled && webuploader.PasswordTxt().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Enter user credentials
                webuploader.UserNameTxt().TextValue = D1st;
                webuploader.PasswordTxt().TextValue = D1st;
                rnxobject.WaitForElementTobeEnabled(webuploader.SignInBtn());

                //Step 5 :- Validate user able to enter credentials
                if (webuploader.UserNameTxt().TextValue.Equals(D1st)
                    && webuploader.PasswordTxt().TextValue.Equals(D1st))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Step 6 :- Click Sign in button
                rnxobject.Click(webuploader.SignInBtn());
                ExecutedSteps++;

                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.ToDestination());

                //Step 7 & 8 :- Verify welcome text shows Username
                ExecutedSteps++;
                if (webuploader.WelcomeText().TextValue.Equals("Welcome " + D1st))
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

                //Step 9 :- Validate recipients section is expanded or not
                if (webuploader.ToDestination().Visible)
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

                //Take Destination from dropdown
                rnxobject.Click(webuploader.ToDestination());
                rnxobject.WaitForElementTobeVisible("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']");
                Ranorex.ListItem Destination = webuploader.GetDestination(DestName);

                //Step 10 :- Check the listed possible destinations 
                if (Destination != null)
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

                //Select Destination
                rnxobject.Click(webuploader.ToDestination());
                rnxobject.Click(Destination);

                //Step 11 :- Validate selected destination is displayed 
                if (webuploader.ToDestination().FindSingle<Ranorex.Text>(".//text").TextValue.Equals(DestName))
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

                //Select study in the specified location
                webuploader.SelectFileFromHdd(StudyPath);

                //Step 12 :- Validate selected patient details are displayed 
                if (webuploader.PatientDetailLabel().TextValue.Contains(LastName) && webuploader.PatientDetailLabel().TextValue.Contains(FirstName)
                     && webuploader.PatientDetailLabel().TextValue.Contains(PatientID))
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

                //Select study table
                rnxobject.Click(webuploader.StudyTable());

                //Get the attach image cell and click
                Dictionary<string, int> tableColumns = webuploader.GetStudyTableColumnIndex();
                Ranorex.Cell AttachImgCell = webuploader.GetCellInTable(tableColumns["Attach Image"] + 2);
                rnxobject.Click(AttachImgCell);

                //Sync-up 
                rnxobject.WaitForElementTobeVisible(webuploader.WUSelectImgForm());

                //Select Image and click select
                webuploader.FileNameTxt().TextValue = AttachmentPath[0];
                rnxobject.Click(webuploader.SelectBtn());

                //Sync-up 
                rnxobject.WaitForElementTobeVisible(webuploader.WUAttachImgForm());

                //Step 13 :- Validate Attach image overlay is displayed or not
                if (webuploader.WUAttachImgForm().EnsureVisible())
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

                //Click Attach button
                rnxobject.Click(webuploader.AttachBtn());

                //Sync-up 
                rnxobject.WaitForElementTobeVisible(webuploader.WUMainForm());

                //Get Series count
                Ranorex.Cell SeriesCountCell_1 = webuploader.GetCellInTable(tableColumns["# of Series/Images"] + 2);
                int SeriesCount_1 = Int32.Parse(SeriesCountCell_1.Text);

                //Step 14 :- Validate attached non dicom image is added as a seperate series
                if (SeriesCount_1 == SeriesCount + 1)
                {
                    SeriesCount++;
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get the attach image cell and click
                Ranorex.Cell AttachPDFCell = webuploader.GetCellInTable(tableColumns["Attach PDF"] + 2);
                rnxobject.Click(AttachPDFCell);

                //Sync-up 
                rnxobject.WaitForElementTobeVisible(webuploader.WUSelectPDFForm());

                //Step 15 :- Validate Attach PDF overlay is displayed or not
                if (webuploader.WUSelectPDFForm().EnsureVisible())
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

                //Select PDF
                webuploader.FileNameTxt(webuploader.WUSelectPDFForm()).TextValue = AttachmentPath[1];

                //Click Select button
                rnxobject.Click(webuploader.SelectBtn(webuploader.WUSelectPDFForm()));

                //Sync-up 
                rnxobject.WaitForElementTobeVisible(webuploader.WUMainForm());

                //Get Series count
                Ranorex.Cell SeriesCountCell_2 = webuploader.GetCellInTable(tableColumns["# of Series/Images"] + 2);

                //Step 16 :- Validate attached pdf file is added as a seperate series
                if (Int32.Parse(SeriesCountCell_2.Text) == SeriesCount + 1)
                {
                    SeriesCount++;
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Enter Comments
                webuploader.CommentsTxtbox().TextValue = Comments;

                //Step 17 :- Validate text in the comments box
                if (webuploader.CommentsTxtbox().TextValue.Equals(Comments))
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

                //Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                //rnxobject.Click(webuploader.SendBtn());
                Ranorex.Mouse.ScrollWheel(-20.0);
                webuploader.SendBtn().EnsureVisible();
                webuploader.SendBtn().Click();

                //Sync - up
                int timeout = 0;
                while (timeout++ < 20)
                {
                    try
                    {
                        if (!webuploader.UploadProgressOKBtn().Visible)
                        {
                            Thread.Sleep(1000);
                            Logger.Instance.InfoLog("Waiting for upload progress pop up to display");
                        }
                        else { break; }
                    }
                    catch (Exception) { Thread.Sleep(1000); }
                }

                //Step 18 :- Validate Upload progress window is displayed with OK button
                if (webuploader.UploadProgressOKBtn().Visible)
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

                //Sync - up
                timeout = 0;
                while (timeout++ < 30)
                {
                    try
                    {
                        if (!webuploader.UploadProgressOKBtn().Enabled)
                        {
                            Thread.Sleep(1000);
                            Logger.Instance.InfoLog("Waiting for studies to be uploaded");
                        }
                        else { break; }
                    }
                    catch (Exception) { Thread.Sleep(1000); }
                }

                //Step 19 :- Validate Upload progress bar is fully loaded with enabled OK button
                if (webuploader.UploadProgressOKBtn().Enabled)
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

                //Step 20 :- Click Ok button and study should reach iCA
                webuploader.UploadProgressOKBtn().Click();
                ExecutedSteps++;

                //Refresh iCA Home Page
                login.DriverGoTo(login.url);

                //Step 21 :- Check webaccess login page is displayed or not
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Step 22 :- Login as Physician-2
                login.LoginIConnect(D2ph, D2ph);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Step 23 :- Monitor adding of exam to inbounds by iCA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Search Study
                inbounds.SearchStudy("PatientID", PatientID);

                //Step 24 :- Validate study is displayed or not
                Dictionary<string, string> studyrow = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status", "Comments" }, new string[] { PatientID, "Uploaded", Comments });
                if (studyrow != null)
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

                //Select Study
                inbounds.SelectStudy1("Patient ID", PatientID);
                int Imagescount = Int32.Parse(inbounds.GetMatchingRow("Patient ID", PatientID)["Number of Images"].Split('/')[1]);

                //Step 25 :- Launch Study
                viewer = StudyViewer.LaunchStudy();
                ExecutedSteps++;

                //Get Thumbnail Caption details
                String[] CaptionDetails = viewer.CaptionDetails();

                //Step 26 :- Validate attached image is displayed as seperate series under OT modality
                if (Imagescount == SeriesCount && CaptionDetails[SeriesCount_1 - 1].Contains("OT"))
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

                //Navigate to history Panel
                viewer.NavigateToHistoryPanel();

                //Get Report details
                Dictionary<string, string> StudywithReport = viewer.GetMatchingRow(new string[] { "Patient ID", "Report" }, new string[] { PatientID, "Yes" });

                //Step 27 :- Validate report is displayed
                if (StudywithReport != null)
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

                //Logout 
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
            finally
            {
                //Switch back to Default browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = DefaultBrowser;
                login.InvokeBrowser(DefaultBrowser);
                login.DriverGoTo(login.url);
                Thread.Sleep(20000);
                try
                {
                    Window pluginWindow = new WpfObjects().GetMainWindowByTitle("Plugin Container for Firefox");
                    pluginWindow.Get<Button>(SearchCriteria.ByText("Debug the program")).Click();
                    BasePage.KillProcess("plugin-container");
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Multiple domain URL-EMR(EI)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27849(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Outbounds outbounds = null;
            UserManagement usermanagement;
            DomainManagement domainmgmt = null;
            RoleManagement rolemgmt = null;
            Taskbar taskbar = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Username = Config.stUserName;
                String Password = Config.stPassword;


                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String PatientDOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String PatientGender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String Ipid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String datasource = login.GetHostName(Config.DestinationPACS).ToUpper();
                string Comments = "Test comments for Upload TestEHR";
                String[] FieldNames = { "MRN:", "Patient Name:", "DOB:", "Gender:", "Issuer of PID:" };

                int randomnumber = new Random().Next(1, 1000);
                String eiWindow = "ExamImporter_" + randomnumber;
                String InstName = "Inst_" + randomnumber;
                String IPID = "IPID_" + randomnumber;
                String[] FieldValues = { PatientID, PatientName, PatientDOB, "Male", IPID };
                String DestName = "Dest_" + randomnumber;
                String eipath = "C:\\ProgramData\\Apps\\" + eiWindow + "\\bin\\UploaderTool.exe";
                //Domain-User and Role
                Dictionary<object, string> domainattr2;
                String D2Physician = "Physician2" + randomnumber;
                String D2Archivist = "Archivist2" + randomnumber;
                String D2ph = "ph2" + randomnumber;
                String D2ar = "ar2" + randomnumber;
                String D2Staff = "Staff2" + randomnumber;
                String D2st = "st2" + randomnumber;
                //step-1:Pre-Condition
                login.UncommentXMLnode("id", "Bypass");

                //Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                //Navigate to Image Sharing-->Institution tab


                //Create domain-2 with Staff users
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainattr2 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr2);

                String Domain2 = domainattr2[DomainManagement.DomainAttr.DomainName];
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");

                //Create Roles
                rolemgmt.CreateRole(Domain2, D2Staff, "Staff");
                rolemgmt.CreateRole(Domain2, D2Physician, "Physician");
                rolemgmt.CreateRole(Domain2, D2Archivist, "Archivist");

                //Create Users
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(D2ph, Domain2, D2Physician, 1, Config.emailid, 1, D2ph);
                usermanagement.CreateUser(D2ar, Domain2, D2Archivist, 1, Config.emailid, 1, D2ar);
                usermanagement.CreateUser(D2st, Domain2, D2Staff, 1, Config.emailid, 1, D2st);


                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Institution inst = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");

                //Add an Institution 
                inst.CreateInstituition(InstName, IPID);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");

                //Add Destination
                dest.CreateDestination(datasource, D2ph, D2ar, DestName, Domain2);
                //Logout iCA                
                login.Logout();

                //PreCondition - Generate Exam Importer
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(Domain2, eiWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();


                //Delete existing installers
                new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
                {
                    if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                        File.Delete(file);
                });

                //Download CD Uploader
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());

                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    SelectElement selector = new SelectElement(login.DomainNameDropdown());
                    selector.SelectByText(Domain2);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }

                String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (browsername.Equals("internet explorer"))
                {
                    BasePage.HandleIENotifyPopup("Save");
                }

                //Check whether the file is present
                Boolean installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");

                int counter = 0;
                while (!installerdownloaded && counter++ < 10)
                {
                    PageLoadWait.WaitForDownload(Config.eiInstaller, Config.downloadpath, "msi");
                    installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");
                    Thread.Sleep(1000);
                }




                //Launch installer tool
                login._examImporterInstance = eiWindow;
                wpfobject.InvokeApplication(Config.downloadpath + @"\" + Config.eiInstaller + ".msi");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(login._examImporterInstance + " Setup", "Cancel", 1);

                //Click Accept and Next
                CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;
                ei.AcceptCheckbox().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                ei.NextBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                try
                {
                    //Choose install for all users and Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.InstallForAllUsers().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();

                    //Choose default destination and click Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                }
                catch (Exception) { }


                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");

                //Enter credentials
                ei.UserNameTextbox().BulkText = Username;
                ei.PasswordTextbox().BulkText = Password;
                ei.InstallBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                //wait until installation completes
                int installWindowTimeOut = 0;
                try
                {
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    while (ei.InstallingText(eiWindow).Visible && installWindowTimeOut++ < 15)
                    {
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    if (installWindowTimeOut == 0)
                    {
                        throw new Exception("Exception in CD Uploader installation window -- " + e);
                    }
                }

                //Check finish button is displayed
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");

                //Uncheck "Launch application when setup exists" and click Finish
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.LaunchAppCheckbox().Click();
                ei.FinishBtn().Click();

                counter = 0;
                while (WpfObjects._mainWindow.Visible && counter++ < 20)
                {
                    Thread.Sleep(1000);
                }


                //String[] EIPath = Config.EIFilePath.Split('\\');
                //EIPath[Array.FindIndex(EIPath, folder => folder.Equals("Apps")) + 1] = eiWindow;
                //String UploaderToolPath = string.Join("\\", EIPath);
                ei.LaunchEI(eipath);
                wpfobject.GetMainWindow(eiWindow);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Enter Credentials
                ei.UserNameTextbox_EI().BulkText = Username;
                ei.PasswordTextbox_EI().BulkText = Password;

                //Click Sign-in and Validate user is logged in
                ei.EI_ClickSignIn(eiWindow);

                //Choose institution
                wpfobject.GetMainWindow(eiWindow);
                ei.SettingsTab().Focus();
                ei.ExistingInstitution().Click();
                ei.InstitutionDropdown().Select(InstName);
                ei.SaveBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(eiWindow);

                login.LoginIConnect(adminUserName, adminPassword);
                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing1 = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest1 = (Image_Sharing.Destination)imagesharing1.NavigateToSubTab("Destination");
                IList<string> DestList_1 = dest1.GetDestinationList(Domain2);
                ExecutedSteps++;


                //step-2:In Merge iConnect Access Service Tool --:-Enable User sharing.

                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined");
                servicetool.CloseServiceTool();
                ExecutedSteps++;


                //step-3:Launch EMR application
                ehr.LaunchEHR();

                //Validate whether EHR application is launched or not
                if (WpfObjects._mainWindow.Visible)
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

                //step-4:Navigate to Launch Exam Importer tab
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Launch Exam Importer");
                wpfobject.WaitTillLoad();

                //Get Tab name
                String tabname = WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.Tab>(SearchCriteria.All).SelectedTab.Name;

                //Validate navigation of "Launch Exam importer" 
                if (tabname.Equals("Launch Exam Importer"))
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

                //step-5:Enter the following:
                //Address = http://*^<^*server IP*^>^*/WebAccess
                //User ID= st1
                //Domain- D1
                //Enable User Sharing = True
                //Auto End Session=True
                //Auth Provider=ByPass
                //Other fields are set to default
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", domain: Domain2, user: D2st, usersharing: "True");
                ITabPage currenttab = ehr.GetCurrentTabItem();
                CheckBox AuthProvider = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(currenttab, "m_authProviderCheckBox");
                if (!(AuthProvider.Checked))
                {
                    AuthProvider.Click();
                }
                ExecutedSteps++;

                //step-6:Fill in the patient contents:
                //Patient Name:"TestEHR01"
                //Patient ID:"TestEHR01"
                //Patient DOB:"2001/12/20"
                //Issuer of PatientID:"Test_TestEHR01"
                //Patient Gender:"M"
                ehr.PatientName().BulkText = PatientName;
                ehr.PatientID().BulkText = PatientID;
                ehr.PatientDOB().BulkText = PatientDOB;
                ehr.IPID().BulkText = IPID;
                ehr.Gender().BulkText = PatientGender;
                ExecutedSteps++;




                //step-7:The user (st) launches the Uploader Tool desktop shortcut titled"Send Exam to --INSTITUTION_NAME--on their system.
                ei.LaunchEI(eipath);
                wpfobject.GetMainWindow(eiWindow);
                WpfObjects._mainWindow.WaitWhileBusy();
                var y = WpfObjects._application;
                ExecutedSteps++;

                //step-8:Do not logon. Minimize Exam Importer window.
                Button minimize = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("Minimize"));
                minimize.Click();
                ExecutedSteps++;


                //step-9:click on load in testEHR

                var x = Process.GetProcessesByName("TestEHR")[0].Id;

                WpfObjects._application = Application.Attach(x);
                wpfobject.GetMainWindow("Test WebAccess EHR");
                String ehrURL = ehr.clickCmdLine();
                login.NavigateToIntegratorURL(ehrURL);
                ehr.CloseEHR();
                /*
                
                                //Get the main window
                                Window mainWindow = null;
                                IList<Window> windows = TestStack.White.Desktop.Instance.Windows();
                                int i;//Get all the windows on desktop
                                for (i = 0; i < windows.Count; i++)
                                {
                                    string str = windows[i].Title.ToLower();
                                    if (str.Contains(eiWindow)) //compare which window title is matching to your string
                                    {
                                        mainWindow = windows[i];
                                        Logger.Instance.InfoLog("Window with title " + str + " is set as the working window");
                                        break;
                                    }
                                }
                                mainWindow.WaitWhileBusy();
                
                */
                //   x = Process.GetProcessesByName("UploaderTool")[1].Id;

                Thread.Sleep(10000);
                WpfObjects._application = y;
                WpfObjects._mainWindow = WpfObjects._application.GetWindow(SearchCriteria.ByText(eiWindow), InitializeOption.NoCache);
                wpfobject.GetMainWindow(eiWindow);
                GroupBox demographics = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("New Patient Demographics"));
                if (ei.welcomeText(D2st).Visible && demographics.Visible)
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

                //stepcount-10:Check the fields displayed in New Patient Demographics section
                GroupBox demographics1 = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("New Patient Demographics"));
                IList<IUIItem> list = demographics1.GetMultiple(SearchCriteria.All);
                int fieldcount = (list.Count / 2) + 1, j, k = 0;
                string[] fields = new string[list.Count / 2];
                for (j = 1; j < fieldcount; j++, k++)
                {
                    Label labelname = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, FieldNames[k], 1);
                    fields[j - 1] = labelname.Text;
                }
                string[] values = new string[list.Count / 2];
                for (int t = 0; t < list.Count / 2; t++)
                {
                    Label labelname = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, FieldValues[t], 1);
                    values[t] = labelname.Text;
                }

                if (fields[0].Equals(FieldNames[0]) && fields[1].Equals(FieldNames[1]) && fields[2].Equals(FieldNames[2]) &&
                    fields[3].Equals(FieldNames[3]) && fields[4].Equals(FieldNames[4]) && values[0].Equals(FieldValues[0]) &&
                    values[1].Equals(FieldValues[1]) && values[2].Equals(FieldValues[2]) && values[3].Equals(FieldValues[3]) &&
                    values[4].Equals(FieldValues[4]))
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
                /*  Label mrn= wpfobject.GetAnyUIItem<GroupBox,Label>(demographics1,"MRN:",1);
                  Label name = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Patient Name:", 1);
                  Label dob = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "DOB:", 1);
                  Label gender = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Gender:", 1);
                  Label ipid = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Issuer of PID:", 1);                 
                  IUIItem mrnvalue = wpfobject.GetAnyUIItem<GroupBox, IUIItem>(demographics1, "TestEHR01:", 1);
                  Label namevalue = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "TestEHR01:", 1);
                  TextBox dobvalue = wpfobject.GetAnyUIItem<GroupBox, TextBox>(demographics1, "12/20/2001:", 1);
                  Label gendervalue = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Male:", 1);
                  Label ipidvalue = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, "Test_TestEHR01:", 1);
                  if(mrn!=null&& name != null && dob != null && gender != null && ipid != null &&
                      mrnvalue.GetType().Name.Equals(PatientID) && namevalue.Text.Equals(PatientName) && dobvalue.Text.Equals(PatientDOB)
                      && ipidvalue.Text.Equals(Ipid) )
                  {
                      result.steps[++ExecutedSteps].status = "Pass";
                      Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                  }
                  else
                  {
                      result.steps[++ExecutedSteps].status = "Fail";
                      Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                      result.steps[ExecutedSteps].SetLogs();
                  }*/


                //step-11:Check the New MRN button
                TextBox newMrn = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("TxtDestinationMrn"));
                if (newMrn.Enabled == false)
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
                //Get all destinations listed
                String[] DestinationList = ei.DestinationList(eiWindow);
                int i1 = 0;
                foreach (string destlist in DestinationList)
                {
                    DestinationList[i1] = destlist.ToUpper();
                    i1++;
                }

                //Step 12 :- In"To"section Uploader Tool presents the list of possible destinations.

                int counter1 = 0;

                if (Array.Exists(DestinationList, Dest => Dest.Equals(DestList_1.Cast<string>().ToArray()[counter1++])))
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

                //step-13:User  selects the appropriate destination
                ei.eiWinName = eiWindow;
                //ei.DestinationDropdown().SetValue("");
                ei.DestinationDropdown().EditableText = "";
                ei.DestinationDropdown().Focus();
                wpfobject.WaitTillLoad();
                ei.DestinationDropdown().Select(DestName);
                WpfObjects._mainWindow.WaitWhileBusy();

                //  Valiadate selected destination is displayed
                if (ei.DestinationDropdown().SelectedItemText.Equals(DestName))
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

                //Step 14 : Select study in the specified path
                ei.SelectFileFromHdd(StudyPath);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Get all patient details
                string[] PatientDetails = ei.AllPatientDetails(eiWindow);

                //Check patient info are displayed correctly with selected test data
                if (Array.Exists(PatientDetails, detail => detail.Contains(LastName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(FirstName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(PatientID)))
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



                //Step 15 :- Upload comments and validate it's display

                //Add comments in upload comments section
                ei.UploadComments(Comments);
                if (ei.CommentsTextBox().Text.Equals(Comments))
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

                //Step-16: User click on the Patient in select Patient drop down
                if (!Array.Exists(PatientDetails, detail => detail.Contains('^'))
                                    && !Array.Exists(PatientDetails, detail => String.IsNullOrEmpty(detail)))
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

                //Click Send
                ei.SelectAllPatientsToUpload();
                ei.Send();

                //Steps 17,18 & 19 - Click send, verify upload progress bar and click ok
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                //Logout and Close Exam importer
                ei.EI_Logout();
                ei.CloseUploaderTool();
                login.CloseBrowser();

                //Step 20 :- Check webaccess login page 
                //BasePage.Driver.Navigate().GoToUrl(Config.IConnectIP+"/webaccess");
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);
                if (login.UserIdTxtBox().Displayed)
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

                //Step 21 :- Login as Administrator
                login.LoginIConnect(D2st, D2st);
                ExecutedSteps++;

                //Step-22:Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");
                ExecutedSteps++;

                //Step 23 :- Validate study is displayed or not
                //StepSearch Study
                outbounds.SearchStudy("PatientID", PatientID);
                outbounds.ChooseColumns(new String[] { "Comments" });
                Dictionary<string, string> studyrow = outbounds.GetMatchingRow(new string[] { "Patient ID", "Comments" }, new string[] { PatientID, Comments });
                if (studyrow != null)
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


                //Logout as PACS user               
                login.Logout();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
        /// Multiple domain URL-EMR(WebUploader)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27850(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Outbounds outbounds = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            DomainManagement domainmgmt = null;
            RoleManagement rolemgmt = null;
            UserManagement usermgmt = null;
            int ExecutedSteps = -1;
            String DefaultBrowser = Config.BrowserType;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientDOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB");
                String PatientGender = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Comments = "Test comments for Upload TestEHR";
                String datasource = login.GetHostName(Config.DestinationPACS).ToUpper();
                int randomnumber = new Random().Next(1, 1000);
                String eiWindow = "ExamImporter_" + randomnumber;
                String InstName = "Inst_" + randomnumber;
                String IPID = "IPID_" + randomnumber;
                String DestName = "Dest_" + randomnumber;

                //Domain-2 Users and Role
                Dictionary<object, string> domainattr2;
                String D2Physician = "Physician2" + randomnumber;
                String D2Archivist = "Archivist2" + randomnumber;
                String D2Staff = "Staff2" + randomnumber;
                String D2ph = "ph2" + randomnumber;
                String D2ar = "ar2" + randomnumber;
                String D2st = "st2" + randomnumber;

                String[] PatientFieldValues = { LastName + "^" + FirstName, PatientID, PatientDOB, IPID, PatientGender };

                //PrecCondition :- Switch to Firefox browser
                login.UncommentXMLnode("id", "Bypass");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = "firefox";
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);

                //Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);

                //Create domain-2 with Physicain,archivist  and Staff users
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainattr2 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr2);
                String Domain2 = domainattr2[DomainManagement.DomainAttr.DomainName];

                //Edit domain - Make Java EI as default exam importer
                domainmgmt.SearchDomain(Domain2);
                domainmgmt.SelectDomain(Domain2);
                domainmgmt.ClickEditDomain();
                domainmgmt.ModifyStudySearchFields();
                domainmgmt.SetCheckBoxInEditDomain("defaultJavaEI", 0);
                domainmgmt.ClickSaveDomain();

                //Create Roles
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(Domain2, D2Physician, "Physician");
                rolemgmt.CreateRole(Domain2, D2Archivist, "Archivist");
                rolemgmt.CreateRole(Domain2, D2Staff, "Staff");

                //Create Users
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D2ph, Domain2, D2Physician, 1, Config.emailid, 1, D2ph);
                usermgmt.CreateUser(D2ar, Domain2, D2Archivist, 1, Config.emailid, 1, D2ar);
                usermgmt.CreateUser(D2st, Domain2, D2Staff, 1, Config.emailid, 1, D2st);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Institution inst = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");

                //Add an Institution 
                inst.CreateInstituition(InstName, IPID);

                //Navigate to Image Sharing-->Institution tab
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");

                //Add Destination
                dest.CreateDestination(datasource, D2ph, D2ar, DestName, Domain2);

                //Logout iCA                
                login.Logout();

                //Launch EMR application
                ehr.LaunchEHR();

                //Step 1 :- Validate whether EHR application is launched or not
                if (WpfObjects._mainWindow.Visible)
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

                //Navigate to Launch Exam Importer tab
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Launch Exam Importer");
                wpfobject.WaitTillLoad();

                //Get Tab name
                String tabname = WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.Tab>(SearchCriteria.All).SelectedTab.Name;

                //Step 2 :- Validate navigation of "Launch Exam importer" 
                if (tabname.Equals("Launch Exam Importer"))
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

                //Step 3 :- Set Common Parameters
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", domain: Domain2, role: D2Archivist, user: D2ar, usersharing: "True");
                ExecutedSteps++;

                //Step 4 :- Set Patient field values and validate
                ehr.PatientName().BulkText = PatientFieldValues[0];
                ehr.PatientID().BulkText = PatientFieldValues[1];
                ehr.PatientDOB().BulkText = PatientFieldValues[2];
                ehr.IPID().BulkText = PatientFieldValues[3];
                ehr.Gender().BulkText = PatientFieldValues[4];
                ExecutedSteps++;

                //Click Load
                String eHRUrl = ehr.clickCmdLine();
                login.NavigateToIntegratorURL(eHRUrl);

                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("ToDestination"));

                //Close eHR
                ehr.CloseEHR();

                //Step 5 :- Check webuploader window is displayed
                if (webuploader.NewPatientDemographics().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("WebUploader not gets loaded");
                }

                //Get Patient DOB
                String date = webuploader.PatientDOBTxt().TextValue.Replace(", ", ",");
                System.DateTime wuDOB = System.DateTime.ParseExact(date, "MMM dd,yyyy", CultureInfo.InvariantCulture);
                System.DateTime ehrDOB = System.DateTime.ParseExact(PatientDOB, "d-MMM-yyyy", CultureInfo.InvariantCulture);

                //Step 6 :- Validate Webuploader gets loaded with entered details corrrectly
                if (webuploader.PatientNameTxt().TextValue.Trim().Equals(PatientFieldValues[0].Replace('^', ' ')) &&
                    webuploader.IPIDTxt().TextValue.Trim().Equals(IPID) && wuDOB.Equals(ehrDOB) &&
                    webuploader.PatientGenderTxt().TextValue.Trim().StartsWith(PatientGender) &&
                    webuploader.PatientMRNTxt().TextValue.Trim().Equals(PatientID))
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

                //Step 7 :- Verify that new MRN textbox is disabled
                if (!webuploader.NewMRNTxt().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("WebUploader not gets loaded");
                }

                //Take Destination from dropdown
                rnxobject.Click(webuploader.ToDestination());
                rnxobject.WaitForElementTobeVisible("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']");
                Ranorex.ListItem Destination = webuploader.GetDestination(DestName);

                //Step 8 :- Check the listed possible destinations 
                if (Destination != null)
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

                //Select Destination
                rnxobject.Click(webuploader.ToDestination());
                rnxobject.Click(Destination);

                //Step 9 :- Validate selected destination is displayed 
                if (webuploader.ToDestination().FindSingle<Ranorex.Text>(".//text").TextValue.Equals(DestName))
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

                //Select study in the specified location
                webuploader.SelectFileFromHdd(StudyPath);

                //Step 10 :- Validate selected patient details are displayed 
                if (webuploader.PatientDetailLabel().TextValue.Contains(LastName) && webuploader.PatientDetailLabel().TextValue.Contains(FirstName)
                     && webuploader.PatientDetailLabel().TextValue.Contains(PatientID))
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

                //Enter Comments
                webuploader.CommentsTxtbox().TextValue = Comments;

                //Step 11 :- Validate text in the comments box
                if (webuploader.CommentsTxtbox().TextValue.Equals(Comments))
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

                //Click Patient Detail dropdown
                rnxobject.Click(webuploader.PatientDetailLabel());
                rnxobject.WaitForElementTobeVisible("/form[@processname='jp2launcher']/?/?/list[@type='JList']");

                //Get Patient details List
                IList<Ranorex.ListItem> List = webuploader.PatientsList().Items;

                //Step 12 :- Validate that the patient details in drop down should not contain '^' / null string
                ExecutedSteps++;
                foreach (Ranorex.ListItem item in List)
                {
                    if (!item.Text.Contains('^') && !String.IsNullOrEmpty(item.Text))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                //Step 13 :- Check every checkbox before the study in Patient list
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                //rnxobject.Click(webuploader.SendBtn());
                Ranorex.Mouse.ScrollWheel(-20.0);
                webuploader.SendBtn().EnsureVisible();
                rnxobject.Click(webuploader.SendBtn());

                //Sync - up
                int timeout = 0;
                while (timeout++ < 20)
                {
                    try
                    {
                        if (!webuploader.UploadProgressOKBtn().Visible)
                        {
                            Thread.Sleep(1000);
                            Logger.Instance.InfoLog("Waiting for upload progress pop up to display");
                        }
                        else { break; }
                    }
                    catch (Exception) { Thread.Sleep(1000); }
                }

                //Step 14 :- Validate Upload progress window is displayed with OK button
                if (webuploader.UploadProgressOKBtn().Visible)
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

                //Sync - up
                timeout = 0;
                while (timeout++ < 30)
                {
                    try
                    {
                        if (!webuploader.UploadProgressOKBtn().Enabled)
                        {
                            Thread.Sleep(1000);
                            Logger.Instance.InfoLog("Waiting for studies to be uploaded");
                        }
                        else { break; }
                    }
                    catch (Exception) { Thread.Sleep(1000); }
                }

                //Step 15 :- Validate "Study Uploaded successfully" message displayed
                if (webuploader.UploadProgressOKBtn().Enabled)
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

                // Click Ok button and study should reach iCA
                rnxobject.Click(webuploader.UploadProgressOKBtn());

                //Navigate to iConnect url
                login.DriverGoTo(login.url);

                //Step 16 :- Check webaccess login page is displayed or not
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Login Page not displayed");
                }

                //Step 17 :- Login as Physician-2
                login.LoginIConnect(D2ar, D2ar);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Step 18 :- Monitor adding of exam to inbounds by iCA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Search Study
                outbounds.SearchStudy(LastName: LastName, FirstName: FirstName, patientID: PatientID,
                    Gender: PatientGender, IPID: IPID, DOB: PatientDOB, Datasource: "", Date: "");

                //Choose Columns
                outbounds.ChooseColumns(new string[] { "Comments" });

                //Step 19 :- Validate study is displayed or not
                Dictionary<string, string> studyrow = outbounds.GetMatchingRow(new string[] { "Patient ID", "Status", "Comments" }, new string[] { PatientID, "Study Reconciled", Comments });
                if (studyrow != null)
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

                //Logout 
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
            finally
            {
                //Switch back to Default browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = DefaultBrowser;
                login.InvokeBrowser(DefaultBrowser);
                login.DriverGoTo(login.url);
                Thread.Sleep(10000);
                try
                {
                    Window pluginWindow = new WpfObjects().GetMainWindowByTitle("Plugin Container for Firefox");
                    pluginWindow.Get<Button>(SearchCriteria.ByText("Debug the program")).Click();
                    BasePage.KillProcess("plugin-container");
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Email Study
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_68661(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            RoleManagement rolemanagement = null;
            Maintenance maintenance;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String MergePacsUserName = Config.mergepacsuser;
                String MergePacsUserPassword = Config.mergepacspassword;
                String AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                //String[] AccessionNumbers = AccessionNumber.Split(':');
                String emailids = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String[] emailid = emailids.Split('=');
                String EmailReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailReason");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                string msg1 = "The Name cannot be empty.";
                string msg2 = "The reason cannot be empty.";
                string msg3 = "Could not send the email.Please contact system administrator.";
                string msg4 = "The email address cannot be empty.";
                string EnableEmailmsg = "This action enables the email study feature. As a result, protected health information may be sent to non-registered users. Do you want to continue?";
                String logopath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LogoImagePath");
                String[] logo = logopath.Split('=');

                //Step 1 :- Precondition - Initial Setup
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ModifyEnableFeatures();
                servicetool.NavigateSubTab("General");
                CheckBox EmailStudy1 = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                if (EmailStudy1.Checked)
                {
                    EmailStudy1.Checked = false;
                }
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                servicetool.NavigateToTab("E-mail Notification");
                servicetool.NavigateSubTab("General");
                servicetool.ResetEmailNotificationForPOP();
                // servicetool.CickApplyButton();
                // servicetool.RestartService();
                ExecutedSteps++;

                //step-2: Setup an email account to send studies to as a destination email address
                ExecutedSteps++;

                //step-3:Run Merge iConnect Access Service Tool.
                //Verify the"Enable Email Study check box" is not selected.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ModifyEnableFeatures();
                servicetool.NavigateSubTab("General");

                CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                if (!EmailStudy.Checked)
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

                servicetool.CloseServiceTool();

                //step-4:Login iConnect Access as Administrator and select Studies tab.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //step-5:Load a study
                studies.SearchStudy("Accession", AccessionNumber);
                studies.ChooseColumns(new String[]{"Accession"});
                studies.SelectStudy1("Accession", AccessionNumber);
                studies.LaunchStudy();
                int counter = 0;
                IList<String> tools = studies.GetReviewToolsFromviewer();
                foreach (string tool in tools)
                {
                    if (tool != "EmailStudy")
                    {
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                }
                if (counter == 0)
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
                studies.CloseStudy();
                login.Logout();
                
                //step-6:Run Merge iConnect Access Service Tool."From the Enable Features tab --General"sub tab --Select Enable Email Study check box.
                /*From the Email Study tab, check for:
                a.Enable Showing Message Page is selected.
                b.Logo Path: WebAccessLoginLogo.png
                c.Enable PIN system to Non-Registed User is selected
                d.PIN Charactor Set: Mixed, PIN Length: 6 and Is Case Sensitive is selected. */
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ModifyEnableFeatures();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.General);
                CheckBox EmailStudy2 = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                string msg = "";
                if (!EmailStudy2.Checked)
                {
                    CheckBox EmailStudyChkBox = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    EmailStudyChkBox.Checked = true;
                    Window dialog = WpfObjects._mainWindow.MessageBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy);
                    msg = wpfobject.GetAnyUIItem<Window, Label>(dialog, "65535").Text;
                    wpfobject.GetAnyUIItem<Window, Button>(dialog, "6").Click();
                    wpfobject.WaitTillLoad();
                    servicetool.ApplyEnableFeatures();
                    servicetool.AcceptDialogWindow();
                }
                else
                {
                    throw new Exception("Email Study checkbox already enabled in Service tool");
                }
                //servicetool.ModifyEnableFeatures();
                //servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                //servicetool.ModifyEnableFeatures();
                //WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                subtab.Click();
                servicetool.WaitWhileBusy();

                //a.Enable Showing Message Page is selected.      
                servicetool.ModifyEnableFeatures();
                servicetool.WaitWhileBusy();
                CheckBox EnableMessage = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Enable Message Page", 1);
                EnableMessage.Checked = true;
                WpfObjects._mainWindow.WaitWhileBusy();

                //b.Logo Path: WebAccessLoginLogo.png
                TextBox logoPath = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "", 0, "1");
                //GroupBox group1 = wpfobject.GetAnyUIItem<GroupBox, GroupBox>(group, "emailStudyMsgPage_panel");
                //TextBox logoPath = wpfobject.GetAnyUIItem<GroupBox, TextBox>(group1, "textBox_logoPath");
                if (logoPath.Text != "WebAccessLoginLogo.png")
                {
                    int count = logoPath.Text.Count();
                    for (int i = 0; i < count; i++)
                    {
                        logoPath.Focus();
                        System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
                    }
                    this.wpfobject.WaitTillLoad();
                    logoPath.Text = "WebAccessLoginLogo.png";

                }
                //c.Enable PIN system to Non-Registed User is selected
                CheckBox Enablepin = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Enable PIN System for Non-Registered Users", 1);
                Enablepin.Checked = true;

                // d.PIN Charactor Set: Mixed, PIN Length: 6 and Is Case Sensitive is selected.
                // wpfobject.SelectFromComboBox("comboBox_pinCharactorSet", "2", 1);
                //ComboBox setPin = wpfobject.GetUIItem<ITabPage, ComboBox>(subtab, 0);
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Mixed");
                TextBox pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "6";
                CheckBox caseSensitive = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Is Case Sensitive", 1);
                caseSensitive.Click();

                if (msg == EnableEmailmsg)//msg
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

                //step-7:Select Yes button and then Apply button.IISRESET. 
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step-8:Login iConnect Access as Administrator and select Studies tab 
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);

                //precondition
                DomainManagement domainmanagement = null;
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.AddToolsToToolbarByName(new string[] { "Email Study"});
                domainmanagement.ClickSaveEditDomain();
                role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectFromList("id", Locators.ID.DomainSelector_RoleMgmt, "SuperAdminGroup", 1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                role.SearchRole("SuperRole","SuperAdminGroup");
                role.SelectRole("SuperRole");
                role.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement emailcb = BasePage.Driver.FindElement(By.CssSelector("input[id$='_AllowEmailCB']"));
                if (!emailcb.Selected) { emailcb.Click(); }
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);

                //navigate to studies
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //step-9:Load a study
                studies.SearchStudy("Accession", AccessionNumber);
                studies.SelectStudy1("Accession", AccessionNumber);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                ExecutedSteps++;

                //step-10:Select Email Study button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                IWebElement emailWindow = BasePage.Driver.FindElement(By.CssSelector("div[id='EmailStudyDialogDiv']"));
                IWebElement toEmail = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_emailToLabel']"));
                IWebElement toName = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_nameToLabel']"));
                IWebElement reason = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_reasonToLabel']"));

                if (emailWindow.Displayed == true && toEmail.Displayed == true && toName.Displayed == true && reason.Displayed == true)
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
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();

                //step-11:Enter a destination email. Select Send Email button	
                viewer.EmailStudy(emailid[0], "", "", 1);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");

                if (message.Contains(msg1))
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

                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();

                //step-12:Enter a name in To Name box. Select Send Email button	
                viewer.EmailStudy(emailid[0], Name, "", 1);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message2 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                if (message2.Contains(msg2))
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

                //step-13:Enter a reason in Reason box.Select Send Email button
                viewer.EmailStudy(emailid[0], Name, EmailReason, 1);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message3 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                if (message3.Contains(msg3))
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

                //step-14:Enter a name and Reason without giving email address.Select Send Email button
                viewer.EmailStudy("", Name, EmailReason, 1);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message4 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                if (message4.Contains(msg4))
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

                //step-15:Close the Email study window.
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                IWebElement emailWindow1 = BasePage.Driver.FindElement(By.CssSelector("div[id='EmailStudyDialogDiv']"));
                if (!(emailWindow1.Displayed))
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
                studies.CloseStudy();
                login.Logout();
                /*	step-16:Run Merge iConnect Access Service Tool -- Select E-mail Notification tab:
                 * 
                Administration E-mail: a valid email address as a source email address.
                Web Application URL: http://TestServerName/WebAccess (example: http://rinox/WebAccess)
                Server Host/IP = dinobot.win.cedara.com
                Select Apply and then IISRESET. */
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("E-mail Notification");
                servicetool.NavigateSubTab("General");
                servicetool.SetEmailNotificationForPOP();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step-17 and 18: Login iConnect Access as Administrator Navigate to the Studies tab and load a study.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;
                studies.SearchStudy("Accession", AccessionNumber);
                studies.SelectStudy1("Accession", AccessionNumber);
                studies.LaunchStudy();
                ExecutedSteps++;

                //step-19: Select Email Study icon
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                IWebElement emailWindow2 = BasePage.Driver.FindElement(By.CssSelector("div[id='EmailStudyDialogDiv']"));
                IWebElement toEmail2 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_emailToLabel']"));
                IWebElement toName2 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_nameToLabel']"));
                IWebElement reason2 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_reasonToLabel']"));
                if (emailWindow2.Displayed == true && toEmail2.Displayed == true && toName2.Displayed == true && reason2.Displayed == true)
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
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();

                //step:20: Enter a name,reason and an invalid email address.Sent Email button
                viewer.EmailStudy(emailid[1], Name, EmailReason, 1);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message5 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                if (message5.Contains(msg3))
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

                //step-21: Enter two valid email addresses with separating semicolon 
                viewer.EmailStudy(emailid[2], Name, EmailReason, 1);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message6 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                if (message6.Contains(msg3))
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


                //step-22: Enter two valid email addresses with separating comma
                viewer.EmailStudy(emailid[3], Name, EmailReason, 1);
                String pinnumber = "";
                pinnumber = studies.FetchPin();
                Regex r = new Regex("^[a-zA-Z0-9]*$");
                if (r.IsMatch(pinnumber) && pinnumber.Length == 6)
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




                //step-23: Remove one of the valid email addresses.<br/-Select Sent Email button
                viewer.EmailStudy(emailid[0], Name, EmailReason, 1);
                System.DateTime today = System.DateTime.Today;
                System.DateTime now = System.DateTime.Now;
                String format = "MM/dd/yyyy";
                string t1 = now.ToString(format);
                string[] st = t1.Split(' ');
                String pinnumber1 = "";
                pinnumber1 = studies.FetchPin();
                Regex r1 = new Regex("^[a-zA-Z0-9]*$");
                if (r1.IsMatch(pinnumber1) && pinnumber.Length == 6)
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
                studies.CloseStudy();

                //step-24: Write down this pin number on a piece of paper.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-25: Select Maintenance tab and then select Audit tab.<br/-Check for Email Study To Guest/Email Study Sent entry.
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                maintenance.SelectEventID("Email Study To Guest");
                //Validate Email Study To Guest/Email Study Sent entry.(whether it is logged)
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_mSearchButton")).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> logs = BasePage.Driver.FindElements(By.CssSelector("table#m_listControl_m_dataListGrid tr"));
                int noOfLogs = logs.Count;
                IWebElement log = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(" + noOfLogs + ")>td>span"));
                IWebElement success = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(" + noOfLogs + ") td:nth-child(2)>span"));
                string time = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(" + noOfLogs + ") td:nth-child(4)>span")).GetAttribute("innerHTML"); ;
                string studydate = System.DateTime.ParseExact(time.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                var today1 = System.DateTime.ParseExact(st[0], "MM/dd/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                if (log.Displayed == true && success.Displayed == true && studydate.Contains(today1))
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

                //step-26:Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-27:Type a bogus PIN number in the PIN code box.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-28:Type the PIN number that was generated from the previous steps in the PIN code box with lower case.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-29:Type the PIN number that was generated from the steps above with case sensitive.
                result.steps[++ExecutedSteps].status = "Not Automated";


                login.Logout();


                //step-30:Logon iConnect Access.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                //Select Maintenance tab and then select Audit tab.
                //Check for Email Study To Guest/Guest Login Review Study.
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                maintenance.SetCheckBoxInAudit();
                maintenance.SelectEventID("Email Study To Guest");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.CssSelector("input#m_maintenanceSearchControl_mSearchButton")).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> logs1 = BasePage.Driver.FindElements(By.CssSelector("table#m_listControl_m_dataListGrid tr"));
                int noOfLogs1 = logs.Count;
                IWebElement log1 = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(" + noOfLogs + ")>td>span"));
                IWebElement success1 = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(" + noOfLogs + ") td:nth-child(2)>span"));
                string time3 = BasePage.Driver.FindElement(By.CssSelector("table#m_listControl_m_dataListGrid tr:nth-child(" + noOfLogs + ") td:nth-child(4)>span")).GetAttribute("innerHTML"); ;
                string studydate3 = System.DateTime.ParseExact(time3.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();

                var today13 = System.DateTime.ParseExact(st[0], "M/dd/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                if (log1.Displayed == true && success1.Displayed == true && studydate3.Contains(today13))
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


                login.Logout();
                
                /*step-31:Run Merge iConnect Access Service Tool.
                From the Enable Featuers -- Email Study tab, check for:
                Enable Showing Message Page is selected.
                In the Message box: remove the current message and type the following"Message is entered here.".
                Logo Path:
                Change the logo Path to another png or JPG file.
                Note: The png or JPG file must be copied to ..\WebAccess\WebAccess\Images folder
                Enable PIN system to Non-Registed User is selected
                PIN Charactor Set: Numeric, PIN Length: 8 and Is Case Sensitive is selected.
                Select Apply button and IISRESET.*/
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ModifyEnableFeatures();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                servicetool.ModifyEnableFeatures();
                WpfObjects._mainWindow.WaitWhileBusy();
                ITabPage subtab2 = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);

                //a.Enable Showing Message Page is selected.
                //wpfobject.GetMainWindow("Merge iConnect Access Service Tool");           
                CheckBox EnableMessage2 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab2, "Enable Message Page", 1);
                EnableMessage2.SetValue(true);



                //b.In the Message box: remove the current message and type the following
                //"Message is entered here.".
                TextBox logoPath3 = wpfobject.GetUIItem<ITabPage, TextBox>(subtab2, "", 0, "1");
                //GroupBox group1 = wpfobject.GetAnyUIItem<GroupBox, GroupBox>(group, "emailStudyMsgPage_panel");
                //TextBox logoPath = wpfobject.GetAnyUIItem<GroupBox, TextBox>(group1, "textBox_logoPath");
                if (logoPath3.Text != "WebAccessLoginLogo.png")
                {
                    int count = logoPath3.Text.Count();
                    for (int i = 0; i < count; i++)
                    {
                        logoPath3.Focus();
                        System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
                    }
                    this.wpfobject.WaitTillLoad();
                    logoPath3.Text = "WebAccessLoginLogo.png";

                }


                //c.Logo Path:Change the logo Path to another png or JPG file.
                logoPath3.Text = logo[0];

                //d.Enable PIN system to Non-Registed User is selected
                CheckBox Enablepin1 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab2, "Enable PIN System for Non-Registered Users", 1);
                Enablepin1.SetValue(true);

                //e.PIN Charactor Set: Numeric, PIN Length: 8 and Is Case Sensitive is selected.
                ComboBox comboBox1 = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox1.Select("Numeric");
                TextBox pinsize1 = wpfobject.GetUIItem<ITabPage, TextBox>(subtab2, "AutoSelectTextBox", 0, "1");
                pinsize1.Text = "8";
                CheckBox caseSensitive1 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab2, "Is Case Sensitive", 1);
                caseSensitive1.SetValue(true);
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;


                //step-32:Login iConnect Access as Administrator and select Studies tab
                //login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //step-33:Load a study
                studies.SearchStudy("Accession", AccessionNumber);
                studies.SelectStudy1("Accession", AccessionNumber);
                studies.LaunchStudy();
                ExecutedSteps++;

                //step-34:Select Email Study button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                IWebElement emailWindow3 = BasePage.Driver.FindElement(By.CssSelector("div[id='EmailStudyDialogDiv']"));
                IWebElement toEmail3 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_emailToLabel']"));
                IWebElement toName3 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_nameToLabel']"));
                IWebElement reason3 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_reasonToLabel']"));
                if (emailWindow3.Displayed == true && toEmail3.Displayed == true && toName3.Displayed == true && reason3.Displayed == true)
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
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();


                //step-35:Enter a valid email addresses.Select Sent Email button
                viewer.EmailStudy(emailid[0], Name, EmailReason, 1);
                System.DateTime today2 = System.DateTime.Today;
                System.DateTime now2 = System.DateTime.Now;
                string t2 = today.ToString();
                String pinnumber2 = "";
                pinnumber2 = studies.FetchPin();
                Regex r2 = new Regex("^[0-9]*$");
                if (r2.IsMatch(pinnumber2) && pinnumber2.Length == 8)
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

                studies.CloseStudy();
                login.Logout();
                
                //step-36: Write down this pin number on a piece of paper.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-37:Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-38:Type a bogus PIN number in the PIN code box.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-39:Type the PIN number that was generated from the previous steps in the PIN code box with lower case.
                result.steps[++ExecutedSteps].status = "Not Automated";

                /*step-40:Run Merge iConnect Access Service Tool.
               From the Enable Featuers -- Email Study tab, check for:
               Enable Showing Message Page is selected.
               In the Message box: remove the current message and type the following"Message is entered here.".
               Logo Path:
               Change the logo Path to another png or JPG file.
               Note: The png or JPG file must be copied to ..\WebAccess\WebAccess\Images folder
               Enable PIN system to Non-Registed User is selected
               PIN Charactor Set: Alphabetic, PIN Length: 8 and Is Case Sensitive is selected.
               Select Apply button and IISRESET.*/
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ModifyEnableFeatures();
                servicetool.NavigateSubTab("Email Study");
                servicetool.ModifyEnableFeatures();
                WpfObjects._mainWindow.WaitWhileBusy();
                ITabPage subtab3 = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);

                //a.Enable Showing Message Page is selected.
                //wpfobject.GetMainWindow("Merge iConnect Access Service Tool");           
                CheckBox EnableMessage3 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab3, "Enable Message Page", 1);
                EnableMessage3.SetValue(true);

                //b.In the Message box: remove the current message and type the following
                //"Message is entered here.".
                TextBox logoPath4 = wpfobject.GetUIItem<ITabPage, TextBox>(subtab3, "", 0, "1");
                //GroupBox group1 = wpfobject.GetAnyUIItem<GroupBox, GroupBox>(group, "emailStudyMsgPage_panel");
                //TextBox logoPath = wpfobject.GetAnyUIItem<GroupBox, TextBox>(group1, "textBox_logoPath");
                if (logoPath4.Text != "WebAccessLoginLogo.png")
                {
                    int count = logoPath4.Text.Count();
                    for (int i = 0; i < count; i++)
                    {
                        logoPath4.Focus();
                        System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
                    }
                    this.wpfobject.WaitTillLoad();
                    logoPath4.Text = "WebAccessLoginLogo.png";

                }

                //c.Logo Path:Change the logo Path to another png or JPG file.                
                logoPath4.Text = logo[1];

                //d.Enable PIN system to Non-Registed User is selected
                CheckBox Enablepin2 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab3, "Enable PIN System for Non-Registered Users", 1);
                Enablepin2.SetValue(true);

                //e.PIN Charactor Set: Alphabetic, PIN Length: 8 and Is Case Sensitive is selected.
                ComboBox comboBox2 = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox2.Select("Alphabetic");
                TextBox pinsize2 = wpfobject.GetUIItem<ITabPage, TextBox>(subtab3, "AutoSelectTextBox", 0, "1");
                pinsize2.Text = "8";

                CheckBox caseSensitive2 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab3, "Is Case Sensitive", 1);
                caseSensitive2.SetValue(true);
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;


                //step-41:Login iConnect Access as Administrator and select Studies tab
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //step-42:Load a study
                studies.SearchStudy("Accession", AccessionNumber);
                studies.SelectStudy1("Accession", AccessionNumber);
                studies.LaunchStudy();
                ExecutedSteps++;

                //step-43:Select Email Study button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                IWebElement emailWindow4 = BasePage.Driver.FindElement(By.CssSelector("div[id='EmailStudyDialogDiv']"));
                IWebElement toEmail4 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_emailToLabel']"));
                IWebElement toName4 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_nameToLabel']"));
                IWebElement reason4 = BasePage.Driver.FindElement(By.CssSelector("div span[id='EmailStudyControl_m_reasonToLabel']"));
                if (emailWindow4.Displayed == true && toEmail4.Displayed == true && toName4.Displayed == true && reason4.Displayed == true)
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
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();

                //step-44:Enter a valid email addresses.Select Sent Email button
                viewer.EmailStudy(emailid[0], Name, EmailReason, 1);
                System.DateTime today3 = System.DateTime.Today;
                System.DateTime now3 = System.DateTime.Now;
                string t3 = today.ToString();
                String pinnumber3 = "";
                pinnumber3 = studies.FetchPin();
                Regex r3 = new Regex("^[a-zA-Z]*$");
                if (r3.IsMatch(pinnumber3) && pinnumber3.Length == 8)
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


                //step-45: Write down this pin number on a piece of paper.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-46:Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-47:Type a bogus PIN number in the PIN code box.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step-48:Type the PIN number that was generated from the previous steps in the PIN code box with lower case.
                result.steps[++ExecutedSteps].status = "Not Automated";


                studies.CloseStudy();
                //Logout as PACS user               
                login.Logout();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
        /// Integrator User Sharing
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27839(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            DomainManagement domainmgmt = null;
            RoleManagement rolemgmt = null;
            UserManagement usermgmt = null;
            StudyViewer viewer = null;
            Random randomnumber = new Random();

            //Domain-1 Users and Role
            Dictionary<object, string> domainattr1;
            String D1Physician = "Physician1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            String D1ph = "ph1" + new System.DateTime().Second + randomnumber.Next(1, 1000);
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int executedSteps = -1;

            try
            {

                //Get Test Data
                String modalitytype = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String unknownuser1 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 100);
                String unknownuser2 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 1000);
                String unknownuser3 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 1000);
                String unknownuser4 = login.RandomString(4, true) + new System.DateTime().Second + randomnumber.Next(1, 1000);

                //Set up Validation Steps
                result.SetTestStepDescription(teststeps);

                //Step-1 - Service Tool Settings.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "disable", shadowuser: "disable");
                servicetool.NavigateToTab("Viewer");
                servicetool.WaitWhileBusy();
                servicetool.NavigateSubTab("Protocols");
                servicetool.WaitWhileBusy();
                servicetool.MoadalitySetting();
                servicetool.CloseServiceTool();
                executedSteps++;

                //Step-2 - Create Domain, Role, User and setup viewer layout                
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainattr1 = domainmgmt.CreateDomainAttr();
                domainmgmt.CreateDomain(domainattr1, new String[] { domainmgmt.GetHostName(Config.SanityPACS) });
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, "Physician");
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(D1ph, domainattr1[DomainManagement.DomainAttr.DomainName], D1Physician, 1, Config.emailid, 1, D1ph);
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");
                domainmgmt.SearchDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domainmgmt.SelectDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domainmgmt.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scroll(0, 1000)");
                domainmgmt.ModalityDropDown().SelectByText(modalitytype);
                domainmgmt.LayoutDropDown().SelectByText("1x1");
                domainmgmt.ClickSaveDomain();
                executedSteps++;

                //Step-3 
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: D1ph);
                ehr.SetSelectorOptions("Study");
                ehr.SetSearchKeys_Study(accession);
                String url_3 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();

                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_3);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();

                //Validate Study Displayed in iConnect 
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                Boolean isstudycorrect = viewer.CompareImage(result.steps[executedSteps], viewer.ViewerContainer());
                
                if (isstudycorrect)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-4-Enable shadow user, User sharing.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "enable", shadowuser: "enable");
                servicetool.CloseServiceTool();
                executedSteps++;

                //Step-5
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser1);
                ehr.SetSelectorOptions("Study");
                ehr.SetSearchKeys_Study(accession);
                String url_5 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();

                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_5);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();

                //Validate Study Displayed in iConnect 
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                Boolean StudyStatus_5 = viewer.CompareImage(result.steps[executedSteps], viewer.ViewerContainer());
                
                //Validate shadow user created
                login = new Login();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
                Boolean isuserpresent = usermgmt.IsUserPresent(unknownuser1);

                if (StudyStatus_5 && isuserpresent)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6 Change User sharing to URL determined               
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined", shadowuser: "enable");
                servicetool.CloseServiceTool();
                executedSteps++;

                //Step-7
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser2, usersharing: "True");
                ehr.SetSelectorOptions("Study");
                ehr.SetSearchKeys_Study(accession);
                String url_7 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();

                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_7);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();

                //Validate Study Displayed in iConnect 
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                Boolean StudyStatus_7 = viewer.CompareImage(result.steps[executedSteps], viewer.ViewerContainer());
                
                //Validate shadow user created
                login = new Login();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
                isuserpresent = usermgmt.IsUserPresent(unknownuser2);

                if (StudyStatus_7 && isuserpresent)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-8
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser3, usersharing: "False");
                ehr.SetSelectorOptions("Study");
                ehr.SetSearchKeys_Study(accession);
                String url_8 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();

                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_8);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();

                //Validate Study Displayed in iConnect 
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                Boolean StudyStatus_8 = viewer.CompareImage(result.steps[executedSteps], viewer.ViewerContainer());
                
                //Validate sadow user created
                login = new Login();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
                isuserpresent = usermgmt.IsUserPresent(unknownuser3);

                if (StudyStatus_8 && !isuserpresent)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-9
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: domainattr1[DomainManagement.DomainAttr.DomainName], role: D1Physician, user: unknownuser4, usersharing: "False");
                ehr.SetSelectorOptions("Study");
                ehr.SetSearchKeys_Study(accession);
                String url_9 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();

                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_9);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();

                //Validate Study Displayed in iConnect 
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                Boolean StudyStatus_9 = viewer.CompareImage(result.steps[executedSteps], viewer.ViewerContainer());
                
                //Validate sadow user created
                login = new Login();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.SearchUser("*", domainattr1[DomainManagement.DomainAttr.DomainName]);
                isuserpresent = usermgmt.IsUserPresent(unknownuser4);

                if (StudyStatus_9 && !isuserpresent)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login = new Login();
                login.Logout();

                //Return Result
                return result;
            }

        }


        /// <summary>
        /// Cleanup script to close browser
        /// </summary>
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }

    }
}
