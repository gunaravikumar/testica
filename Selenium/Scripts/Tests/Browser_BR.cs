using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
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
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Remote;
using TestStack.White.UIItems.WindowItems;
using TestStack.White;
using CheckBox = TestStack.White.UIItems.CheckBox;
using System.ServiceProcess;
using System.Diagnostics;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;
using GroupBox = TestStack.White.UIItems.GroupBox;
using TextBox = TestStack.White.UIItems.TextBox;
using Tab = TestStack.White.UIItems.TabItems.Tab;
using ITabPage = TestStack.White.UIItems.TabItems.ITabPage;
using TestStack.White.UIItems.Finders;

namespace Selenium.Scripts.Tests
{
    class Browser_BR
    {
        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPLogin hplogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public ExamImporter ei { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public UserPreferences userpref{ get; set; }
        public StudyViewer viewer { get; set; }
        String TestUser = "User_" + new Random().Next(1, 10000);
        public WpfObjects wpfobject;
        public Browser_BR(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();
            viewer = new StudyViewer();
            ei = new ExamImporter();
            BasePage.InitializeControlIdMap();
            wpfobject = new WpfObjects();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        /// <summary>
        /// Connectivity Tool
        /// </summary>
        public TestCaseResult Test_162130(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables        
            Studies studies = null;
            studies = new Studies();
            Patients patients = null;
            patients = new Patients();
            Viewer viewer = null;
            viewer = new Viewer();
            DomainManagement domainmanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] lastName = LastName.Split(':');
                String BrowserList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BrowserList");
                String[] Browser = BrowserList.Split(':');


                //Step-1
                //Service tool precondition coded in Browser setup
                login.DriverGoTo(login.url);
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(login.ConnectionTestTool())).Displayed)
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


                //step-2 - Check Connection test tool is visible 
                login.LoginIConnect(username, password);
                userpref = studies.OpenUserPreferences();
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.SwitchToUserPrefFrame();
                if (!userpref.EnableConnectionTestTool().Selected)
                {
                    userpref.EnableConnectionTestTool().Click();
                }
                studies.CloseUserPreferences();
                //domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                studies = (Studies)login.Navigate("Studies");
                studies.SwitchToDefault();
                studies.SwitchToUserHomeFrame();
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Studies.DivConnectionTest))).Displayed)
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


                //step-3-Verify Connect Test tool visible in after Study search
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy("last", lastName[0]);
                studies.SelectStudy("Patient Name", Name);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(5);
                studies.SwitchToDefault();
                studies.SwitchToUserHomeFrame();
                BasePage.wait.Until<Boolean>(d =>
                {
                    if (!d.FindElement(By.CssSelector(BluRingViewer.div_ConnectionTool)).GetAttribute("style").Contains("display:none"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_ConnectionTool))).Displayed)
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


                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
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
                //Remove Connection test tool
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                userpref = studies.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                if (userpref.EnableConnectionTestTool().Selected)
                {
                    userpref.EnableConnectionTestTool().Click();
                }
                studies.CloseUserPreferences();

                //Logout
                login.Logout();
            }
        }

        /// <summary>
        /// Keyboard Shortcuts
        /// </summary>
        public TestCaseResult Test_161357(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables        
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Step-1: Login as Admin                
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2: Click on "Studies" tab.
                studies = (Studies)login.Navigate("Studies");
                IWebElement SearchPanel = PageLoadWait.WaitForElement(By.CssSelector(Studies.DivSearchPanel), BasePage.WaitTypes.Visible, 15);
                bool step2 = SearchPanel.Displayed;
                if (step2 && BasePage.GetSearchResults().Count == 0)
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

                //Step-3: Add a * in the Last name field and click on search
                studies.SearchStudy("last", "*");
                PageLoadWait.WaitForLoadingMessage(30);
                bool isscrollbar = studies.IsVerticalScrollBarPresent(BasePage.Driver.FindElement(By.CssSelector(Studies.SearchGridBody)));
                if (isscrollbar)
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

                //Step-4: Press down/up arrow to see if scrolling is done using keyboard
                Int64 scrollpostion_before = (Int64)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return(document.querySelector('" + Studies.SearchGridBody + "').scrollTop)");
                for (int i = 0; i < 5; i++)
                {
                    BasePage.Driver.FindElement(By.Id("gridTableStudyList")).SendKeys(Keys.ArrowDown);
                    //BasePage.mouse_event(0x0800, 0, 0, -100, 0);
                }
                Int64 scrollpostion_after = (Int64)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return(document.querySelector('" + Studies.SearchGridBody + "').scrollTop)");
                if ((scrollpostion_before == 0) && (scrollpostion_after > scrollpostion_before))
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

                //Step 5: Load a study with multiple series.
                studies.ClearFields();
                studies.SearchStudy("accession", AccessionID);
                studies.SelectStudy("Accession", AccessionID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6: Click on any series and scroll up/down the mouse wheel.
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                var action = new TestCompleteAction();
                action.MouseScroll(element, "down", "3").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport));
                if (step6)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7: Press the keyboard up/down arrows.
                element = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                //element.Click();
                bluringviewer.KeyboardArrowScroll(By.CssSelector(bluringviewer.Activeviewport), 2, Keys.ArrowDown);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step7_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 1);

                //Press Up Arrow Keys - twice
                //element.Click();
                bluringviewer.KeyboardArrowScroll(By.CssSelector(bluringviewer.Activeviewport), 2, Keys.ArrowUp);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step7_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step7_1 && step7_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Close study and logout from ICA
                bluringviewer.CloseBluRingViewer();
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
        /// Window Resizing
        /// </summary>
        public TestCaseResult Test_161358(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            StudyViewer StudyVw;
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Names = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String ModalityToolbarList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ModalityToolbar");
                String PIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                 String[] ModalityToolbar = ModalityToolbarList.Split(':');
                String[] PID = PIDList.Split(':');
                String[] Name = Names.Split(':');
                String[] LastName = LastNameList.Split(':');
                String EA131 = login.GetHostName(Config.EA1);
                String EA91 = login.GetHostName(Config.EA91);

                //Pre-condition: Enabling reports:
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                tool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(ServiceTool.EnableFeatures.ID.EncapsulatedPDF)).Checked = true;
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.RestartService();
                tool.CloseServiceTool();

                //Step-1: Logon to iCA as Administrator
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2: Load a Multi-frame study.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", LastName[0]);
                studies.SelectStudy("Patient Name", Name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Enable 1x1 layout and resize the browser window. Repeat the step with all the available layouts.
                //1x1
                bluringviewer.ChangeViewerLayout("1x1", viewport: 1);
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_1 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 1);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //1x2
                bluringviewer.ChangeViewerLayout("1x2", viewport: 1);
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_2 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //1x3
                bluringviewer.ChangeViewerLayout("1x3", viewport: 1);
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_3 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 3);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //2x2
                bluringviewer.ChangeViewerLayout("2x2", viewport: 1);
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_4 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 4);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                //2x3
                bluringviewer.ChangeViewerLayout("2x3", viewport: 1);
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_5 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 5, 1);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);

                if (step3_1 && step3_2 && step3_3 && step3_4 && step3_5)
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
                bluringviewer.CloseBluRingViewer();

                //Step-4: Load a Multi-frame study with prior studies say as US/XA modality in the Universal viewer.
                studies.ClearFields();
                studies.SearchStudy("last", LastName[1]);
                studies.SelectStudy("Patient Name", Name[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Resize the window horizontally (drag the browser to left/right).
                BasePage.Driver.Manage().Window.Size = new Size(800, 1000);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Resize the window vertically (drag the browser to Top/Bottom).
                BasePage.Driver.Manage().Window.Size = new Size(1000, 700);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step6 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step6)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-7: Load a related study from Exam List
                bluringviewer.OpenPriors(0);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step9 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(2));
                if (step9)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                bluringviewer.CloseStudypanel(2);
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);

                //Step8-: Click on Help icon and select a content and resize the browser.
                string[] windowhandle = bluringviewer.OpenHelpandSwitchtoIT();
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                bool step10 = false;
                try
                {
                    var viewport = BasePage.Driver.FindElement(By.CssSelector("html > frameset"));
                    step10 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                }
                catch (Exception ex) { Logger.Instance.InfoLog("Error while trying to view Help window " + ex);  }
                if (step10)
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
                bluringviewer.CloseHelpView(windowhandle[1], windowhandle[0]);
                bluringviewer.CloseBluRingViewer();

                //Step 9: Load a study that has report in the universal viewer.
                studies.ClearFields();
                studies.SearchStudy("last", LastName[2]);
                studies.SelectStudy("Patient Name", Name[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool IsReportIconPresent = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_activeReportIcon), BasePage.WaitTypes.Visible).Displayed;
                bool step11 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (IsReportIconPresent && bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_PatientInfoContainer).Text.ToLower().Contains(FirstName.ToLower()))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10: From the exam list, click on report icon to view the report. Then resize the browser .
                IList<IWebElement> ReportIcon = BasePage.FindElementsByCss(BluRingViewer.report_icon);
                bluringviewer.OpenReport_BR(0);
                //ReportIcon[0].Click();
                Thread.Sleep(2000);
                bluringviewer.NavigateToReportFrame(0);
                //BasePage.Driver.SwitchTo().Frame("reportIframe");
                IWebElement reportcontainer_before = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.Id(BluRingViewer.reportContainerID)));
                int width_before = reportcontainer_before.Size.Width;
                int height_before = reportcontainer_before.Size.Height;
                BasePage.Driver.Manage().Window.Size = new Size(800, 800);
                Thread.Sleep(2000);
                //PageLoadWait.WaitForFrameLoad(10);
                bluringviewer.NavigateToReportFrame(0);
                var reportcontainer = BasePage.Driver.FindElement(By.Id(BluRingViewer.reportContainerID));
                int width_after = reportcontainer.Size.Width;
                int height_after = reportcontainer.Size.Height;
                bool step12 = ((width_before > width_after) || (height_after < height_before)) ? true : false;
                if (step12)
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
                BasePage.Driver.Manage().Window.Maximize();
                bluringviewer.CloseBluRingViewer();

                //Step-11: Load a study with multiple KO's [e.g. Abdomen, CT (1205937)10211067, 26-Apr-1999 9:41:11 AM]
                studies.ClearFields();
                studies.SearchStudy(LastName: LastName[3], Description: Description, Datasource: EA91);
                PageLoadWait.WaitForLoadingMessage(60);
                studies.SelectStudy("Description", Description);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step13 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step13)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Load the KO series image in 1x1 layout
                bluringviewer.ChangeViewerLayout();

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step14 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step14)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Scroll the KO images downwards to Series 2 Image 36
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                var action = new TestCompleteAction();
                action.MouseScroll(element, "down", "3").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step15)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Resize the browser size by Restoring it or manually.
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step16)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Maximize the browser again
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step17)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Select Series 2 (Not KO) which has more image in 1x1 layout in the universal viewer.
                bluringviewer.CloseBluRingViewer();
                studies.ClearFields();
                studies.SearchStudy("last", LastName[1]);
                studies.SelectStudy("Patient Name", Name[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                //bluringviewer.ClickOnViewPort(1, 2);
                bluringviewer.ChangeViewerLayout();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step18)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Scroll the images downwards to a Series 2 Image 21
                element = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(element, "down", "3").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step20)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18: Resize the browser size by Restoring it or manually.
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step21 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step21)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19: Maximize the browser again
                BasePage.Driver.Manage().Window.Maximize();
                Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step22)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Report Result
                login.Logout();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
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
    }
}
