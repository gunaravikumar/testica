using System;
using System.Globalization;
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
    class ExternalApplicationEmailStudy : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        //public ServiceTool servicetool { get; set; }
        Studies studies = new Studies();
        ServiceTool servicetool = new ServiceTool();

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public ExternalApplicationEmailStudy(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Email Study - Disable Email Study function.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27584(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            UserManagement usermanagement;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");

                String[] Accessions = AccessionList.Split(':');

                String u1 = "u1_27584_" + new Random().Next(1, 1000);

                //Step 1 - Unselect Enable Email Study option
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), "Enable Email Study", 1);
                if (EmailStudy.Checked)
                {
                    EmailStudy.Click();
                    wpfobject.WaitTillLoad();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.RestartService();
                    wpfobject.WaitTillLoad();
                }
                servicetool.CloseServiceTool();
                Executedsteps++;

                //Step 2 - Login iConnect Access as Administrator and select Studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                Executedsteps++;

                //Step 3 - Load a study
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer viewer = LaunchStudy();
                if (!IsElementVisible(By.CssSelector("div[id=reviewToolbar] li[title='Email Study']")))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step 4 - Login iConnect Access as a user (u1) and select Studies tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(u1, DefaultDomain, DefaultRoleName);
                login.Logout();

                login.LoginIConnect(u1, u1);
                //studies = login.Navigate<Studies>();
                Executedsteps++;

                //Step 5 - Load a study
                studies.SearchStudy("studyPerformed", "All Dates");
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer viewer1 = LaunchStudy();
                if (!IsElementVisible(By.CssSelector("div[id=reviewToolbar] li[title='Email Study']")))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }
                viewer1.CloseStudy();

                //Report Result
                result.FinalResult(Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableEmailStudy();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.SetEmailNotification();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary>
        /// Email Study - Initial Setup
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27585(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRoleName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");

                String[] Accessions = AccessionList.Split(':');

                //Step 1 - Pre-condition               
                Executedsteps++;

                //Step 2 - Setup an email account as a destination email address
                Executedsteps++;

                //Step 3 - After a fresh iConnect Access installation, run Merge iConnect Access Service Tool.
                //Select Enable Features tab => General sub tab => Click on Modify and Verify the "Enable Email Study check box"                               
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), "Enable Email Study", 1);
                if (EmailStudy.Checked)
                {
                    EmailStudy.Click();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.RestartService();
                    wpfobject.WaitTillLoad();
                }
                servicetool.CloseServiceTool();
                Executedsteps++;

                //Step 4 - Login iConnect Access as Administrator and select Studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                Executedsteps++;

                //Step 5 - Load a study                
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer viewer = LaunchStudy();
                if (!IsElementVisible(By.CssSelector("div[id=reviewToolbar] li[title='Email Study']")))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step 6  
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableEmailStudy();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                //Logo path
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                TextBox logoPath = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "", 0, "1");
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
                //Enable PIN
                CheckBox Enablepin = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Enable PIN System for Non-Registered Users", 1);
                Enablepin.Checked = true;
                //Mixed
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Mixed");
                //Set pinsize to 6
                TextBox pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "6";
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();

                servicetool.NavigateToTab("E-mail Notification");
                servicetool.NavigateSubTab("General");
                servicetool.SetEmailNotification(SMTPHost: "test");
                Executedsteps++;

                //Step 7
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                //Enable email study.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.AddToolsToToolbarByName(new string[] { "Email Study" });
                domainmanagement.ClickSaveEditDomain();

                //PreCondition - Enable Email Study for SuperRole User
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(DefaultDomain);
                rolemanagement.SearchRole(DefaultRoleName);
                rolemanagement.SelectRole(DefaultRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveEditRole();

                login.Logout();

                Executedsteps++;

                //Setp 8
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                Executedsteps++;

                //Step 9
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer studyviewer1 = LaunchStudy();
                Executedsteps++;

                //Step 10
                studyviewer1.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(studyviewer1.By_EmailStudyDiv()));
                if (IsElementVisible(studyviewer1.By_ToEmail()) &&
                    IsElementVisible(studyviewer1.By_ToName()) &&
                    IsElementVisible(studyviewer1.By_Reason()))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step 11
                studyviewer1.ToEmailTxtBox().SendKeys("shikander.raja@aspiresys.com");
                studyviewer1.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(studyviewer1.By_EmailStudyErrMsgLbl()));
                if (studyviewer1.EmailStudyErrorMsgLbl().Text.Equals("The Name cannot be empty."))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step 12
                studyviewer1.ToNameTxtBox().SendKeys("Shikander Raja");
                studyviewer1.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(studyviewer1.By_EmailStudyErrMsgLbl()));
                if (studyviewer1.EmailStudyErrorMsgLbl().Text.Equals("The reason cannot be empty."))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step 13
                studyviewer1.ReasonTxtBox().SendKeys("Test");
                studyviewer1.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(studyviewer1.By_EmailStudyErrMsgLbl()));
                if (studyviewer1.EmailStudyErrorMsgLbl().Text.Equals("Could not send the email.Please contact system administrator."))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step 14
                studyviewer1.ToEmailTxtBox().Clear();
                studyviewer1.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(studyviewer1.By_EmailStudyErrMsgLbl()));
                if (studyviewer1.EmailStudyErrorMsgLbl().Text.Equals("The email address cannot be empty."))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step 15
                studyviewer1.EmailStudyXBtn().Click();
                if (!IsElementVisible(studyviewer1.By_EmailStudyDiv()))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step 16
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("E-mail Notification");
                servicetool.NavigateSubTab("General");
                servicetool.SetEmailNotification();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                Executedsteps++;

                //Report Result
                result.FinalResult(Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Email Study - Administrator send a study to a Guest by Email without PIN enabled.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27582(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Maintenance maintenance;

            DateTime now = DateTime.Now;
            string t1 = now.ToString();
            string[] st = t1.Split(' ');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accessions = AccessionList.Split(':');

                //Step 1                                                           
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                CheckBox Enablepin = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Enable PIN System for Non-Registered Users", 1);
                if (Enablepin.Checked)
                {
                    Enablepin.Click();
                }
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 2 - Login iConnect Access as Administrator and select Studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step 3 - Load a study                
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer viewer = LaunchStudy();
                ExecutedSteps++;

                //Step 4               
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                if (IsElementVisible(viewer.By_ToEmail()) &&
                    IsElementVisible(viewer.By_ToName()) &&
                    IsElementVisible(viewer.By_Reason()))
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

                //Step 5
                viewer.ToEmailTxtBox().SendKeys("shikander.raja@aspiresys.com");
                viewer.ToNameTxtBox().SendKeys("Shikander");
                viewer.ReasonTxtBox().SendKeys("Test");
                viewer.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyErrMsgLbl()));
                if (viewer.EmailStudyErrorMsgLbl().Text.Equals("The study has been sent successfully."))
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
                viewer.CloseStudy();

                //Step 6
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                //maintenance.Txt_AccessionNumber().SendKeys(Accessions[0]);
                maintenance.SelectEventID("Email Study To Guest", 0);
                PageLoadWait.WaitForFrameLoad(20);
                maintenance.Btn_Search().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                int count = maintenance.AuditListTable().Count;
                IList<IWebElement> AuditEvent = maintenance.AuditListTable()[1].FindElements(By.CssSelector("td"));
                string Datetime = AuditEvent[3].Text;
                string studydate = System.DateTime.ParseExact(Datetime.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                var day = System.DateTime.ParseExact(st[0], "M/d/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                maintenance.AuditListTable()[1].Click();
                wait.Until(ExpectedConditions.ElementIsVisible(maintenance.By_MessageDetailsDiv()));
                bool IsMessageDivPresent = IsElementPresent(maintenance.By_MessageDetailsDiv());

                if (AuditEvent[1].Text.Equals("Success") && studydate.Contains(day)
                     && AuditEvent[2].Text.Split('/')[1].Equals("SuperAdminGroup")
                    && AuditEvent[2].Text.Split('/')[2].Equals("Administrator") && IsMessageDivPresent)
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

                //Step 7 - Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 8 - Select OK button.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9 - Check for Email Study To Guest/Guest Login Review Study.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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
        /// Email Study - A user sends a study to a Guest by Email without PIN enabled.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27583(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Maintenance maintenance;
            UserManagement usermanagement;

            DateTime now = DateTime.Now;
            string t1 = now.ToString();
            string[] st = t1.Split(' ');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');

                String u1 = "u1_27584_" + new Random().Next(1, 1000);

                //Step 1 - Create a user belongs to SuperAdminGroup        
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(u1, "SuperAdminGroup", "SuperRole");
                login.Logout();
                ExecutedSteps++;

                //Step 2 - Login iConnect Access as a new created user (example u1) and select Studies tab                
                login.LoginIConnect(u1, u1);
                //studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step 3 - Load a study    
                studies.SearchStudy("studyPerformed", "All Dates");
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer viewer = LaunchStudy();
                ExecutedSteps++;

                //Step 4 - Select Email Study button               
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                if (IsElementVisible(viewer.By_ToEmail()) &&
                    IsElementVisible(viewer.By_ToName()) &&
                    IsElementVisible(viewer.By_Reason()))
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

                //Step 5 - Enter a valid email addresses, Name and Reason, Select Sent Email button
                viewer.ToEmailTxtBox().SendKeys("shikander.raja@aspiresys.com");
                viewer.ToNameTxtBox().SendKeys("Shikander");
                viewer.ReasonTxtBox().SendKeys("Test");
                viewer.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyErrMsgLbl()));
                if (viewer.EmailStudyErrorMsgLbl().Text.Equals("The study has been sent successfully."))
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
                viewer.CloseStudy();

                //Step 6 - Logout u1 and login as Administrator.
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                //maintenance.Txt_AccessionNumber().SendKeys(Accessions[0]);
                maintenance.SelectEventID("Email Study To Guest", 0);
                PageLoadWait.WaitForFrameLoad(20);
                maintenance.Btn_Search().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                int count = maintenance.AuditListTable().Count;
                IList<IWebElement> AuditEvent = maintenance.AuditListTable()[1].FindElements(By.CssSelector("td"));
                string Datetime = AuditEvent[3].Text;
                string studydate = System.DateTime.ParseExact(Datetime.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                var day = System.DateTime.ParseExact(st[0], "M/d/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                maintenance.AuditListTable()[1].Click();
                wait.Until(ExpectedConditions.ElementIsVisible(maintenance.By_MessageDetailsDiv()));
                bool IsMessageDivPresent = IsElementPresent(maintenance.By_MessageDetailsDiv());

                if (AuditEvent[1].Text.Equals("Success") && studydate.Contains(day)
                     && AuditEvent[2].Text.Split('/')[1].Equals("SuperAdminGroup")
                    && AuditEvent[2].Text.Split('/')[2].Equals(u1) && IsMessageDivPresent)
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

                //Step 7 - Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 8 - Select OK button.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9 - Check for Email Study To Guest/Guest Login Review Study.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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
        /// Email Study - A User send a study with report to a Guest.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_70000(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            StudyViewer viewer;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");

                String[] Accessions = AccessionList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Precondition
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                CheckBox Enablepin = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Enable PIN System for Non-Registered Users", 1);
                Enablepin.Checked = true;
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                //Step 1 - Logon to iCA as Administrator         
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 2 - Load a study with SR and Audio report                
                studies = login.Navigate<Studies>();
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = LaunchStudy();
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));
                if (viewer.TitlebarReportIcon().Displayed)
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

                //Step 3 - Click on Report icon and verify that the sent reports are loaded properly 
                viewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_reportViewerContainer']")));
                //Get Report list details                
                viewer.ViewerReportListButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id$='studyPanel_1_m_reportViewer_reportListContainer']")));
                Dictionary<int, string[]> ReportListDetails = viewer.StudyViewerListResults("StudyPanel", "report", 1);

                //Get report type
                string SRreport = "";
                foreach (int key in ReportListDetails.Keys)
                {
                    SRreport = Array.Find(ReportListDetails[key], t => t.EndsWith("SR"));
                    if (SRreport != null)
                    {
                        break;
                    }
                }

                Dictionary<string, string> SRreportDetails = viewer.StudyViewerListMatchingRow("Type", SRreport, "StudyPanel", "report");
                viewer.SelectItemInStudyViewerList("Type", SRreport, "StudyPanel", "report");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ViewerReportListButton()));

                Dictionary<string, string> ReportDetails = viewer.ReportDetails("studypanel");
                if (ReportDetails["MRN"].Equals(PatientID[0]))
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

                //Step 4 - Click on email Study icon from review toolbar and enter the valid guest mail address, name and reason then click on "Send Email" button
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                viewer.ToEmailTxtBox().SendKeys("shikander.raja@aspiresys.com");
                viewer.ToNameTxtBox().SendKeys("Shikander");
                viewer.ReasonTxtBox().SendKeys("Test");
                viewer.EmailStudySendBtn().Click();
                ExecutedSteps++;

                //Step 5 - Note down the generated PIN number.
                string pinnumber = studies.FetchPin();
                if (!(pinnumber == ""))
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
                viewer.CloseStudy();

                //Step 6 - Go to the destination Email and Check that the "Emailed Study" notification is received.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 7 - Click the link "Click here" from the mail.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 8 - Enter the noted PIN and click on "OK".0
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9 - Click on Report icon and verify that the sent reports [SR and Audio] are loaded properly
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 10 - Repeat the above steps with the Requisition Dataset
                studies.ClearSearchBtn().Click();
                studies.SearchStudy(LastName: LastName[0]);
                studies.SelectStudy("Patient Name", LastName[0] + ", ");
                viewer = LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                viewer.ToEmailTxtBox().SendKeys("shikander.raja@aspiresys.com");
                viewer.ToNameTxtBox().SendKeys("Shikander");
                viewer.ReasonTxtBox().SendKeys("Test");
                viewer.EmailStudySendBtn().Click();
                if (!(studies.FetchPin() == ""))
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
                viewer.CloseStudy();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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
        /// Email Study - Administrator sends a study to a Guest (non-registered user)  by Email.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27581(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            UserManagement usermanagement;
            Maintenance maintenance;
            StudyViewer viewer;

            DateTime now = DateTime.Now;
            string t1 = now.ToString();
            string[] st = t1.Split(' ');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String EmailIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String NameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String ReasonList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");

                String[] Accessions = AccessionList.Split(':');
                String[] Name = NameList.Split(':');
                String[] Email = EmailIDList.Split(':');
                String[] Reason = ReasonList.Split(':');

                //Step 1 - Login iConnect Access as Administrator    
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();

                //Enable PIN
                ITabPage subtab2 = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                CheckBox Enablepin2 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab2, "Enable PIN System for Non-Registered Users", 1);
                Enablepin2.Checked = true;
                //Mixed
                ComboBox comboBox2 = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox2.Select("Mixed");
                //Set pinsize to 8
                TextBox pinsize2 = wpfobject.GetUIItem<ITabPage, TextBox>(subtab2, "AutoSelectTextBox", 0, "1");
                pinsize2.Text = "6";

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 2 - Go to the User Management tab                
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (usermanagement.WarningMsgLbl().Text.Equals("Be advised that the email study feature is enabled. As a result, protected health information may be sent to non-registered users."))
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

                //Step 3 - Select Studies tab and load a study.
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                viewer = LaunchStudy();
                ExecutedSteps++;

                //Step 4 - Select Email Study button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                if (IsElementVisible(viewer.By_ToEmail()) &&
                    IsElementVisible(viewer.By_ToName()) &&
                    IsElementVisible(viewer.By_Reason()))
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

                //Step 5 - Enter a bogus email in To Email box. Example:aaaa.aaaa.aaaa
                viewer.ToEmailTxtBox().SendKeys(Email[0]);
                viewer.ToNameTxtBox().SendKeys(Name[0]);
                viewer.ReasonTxtBox().SendKeys(Reason[0]);
                viewer.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyErrMsgLbl()));
                if (viewer.EmailStudyErrorMsgLbl().Text.Equals("Could not send the email.Please contact system administrator."))
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

                //Step 6 - Enter two valid email addresses with separating semicolon 
                viewer.ToEmailTxtBox().Clear();
                viewer.ToEmailTxtBox().SendKeys(Email[1]);
                viewer.ToNameTxtBox().SendKeys(Name[0]);
                viewer.ReasonTxtBox().SendKeys(Reason[0]);
                viewer.EmailStudySendBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyErrMsgLbl()));
                if (viewer.EmailStudyErrorMsgLbl().Text.Equals("Could not send the email.Please contact system administrator."))
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

                //Step 7 - Enter two valid email addresses with separating comma                
                //viewer.ToEmailTxtBox().Clear();
                //viewer.ToEmailTxtBox().SendKeys(Email[2]);
                //viewer.ToNameTxtBox().Clear();
                //viewer.ToNameTxtBox().SendKeys(Name[0]);
                //viewer.ReasonTxtBox().Clear();
                //viewer.ReasonTxtBox().SendKeys(Reason[0]);
                //viewer.EmailStudySendBtn().Click();
                //string pin = studies.FetchPin();
                //if (!(pin == "") && pin.Length == 6)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";
                viewer.EmailStudyXBtn().Click();

                //Step 8 - Remove one of the valid email addresses.
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                viewer.ToEmailTxtBox().SendKeys(Email[4]);
                viewer.ToNameTxtBox().SendKeys(Name[0]);
                viewer.ReasonTxtBox().SendKeys(Reason[0]);
                viewer.EmailStudySendBtn().Click();
                String pin1 = studies.FetchPin();
                if (!(pin1 == "") && pin1.Length == 6)
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
                viewer.CloseStudy();

                //Step 9 - Write down this pin number on a piece of paper.
                ExecutedSteps++;

                //Step 10 - Select Maintenance tab and then select Audit tab.
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                //maintenance.Txt_AccessionNumber().SendKeys(Accessions[0]);
                maintenance.SelectEventID("Email Study To Guest", 0);
                PageLoadWait.WaitForFrameLoad(20);
                maintenance.Btn_Search().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                int count = maintenance.AuditListTable().Count;
                IList<IWebElement> AuditEvent = maintenance.AuditListTable()[1].FindElements(By.CssSelector("td"));
                string date = AuditEvent[3].Text;
                string studydate = System.DateTime.ParseExact(date.ToString(), "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                var day = System.DateTime.ParseExact(st[0], "M/d/yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                maintenance.AuditListTable()[1].Click();
                wait.Until(ExpectedConditions.ElementIsVisible(maintenance.By_MessageDetailsDiv()));
                bool IsMessageDivPresent = IsElementPresent(maintenance.By_MessageDetailsDiv());

                if (AuditEvent[1].Text.Equals("Success") && studydate.Contains(day) && IsMessageDivPresent)
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

                //Step 11 - Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 12 - Type a bogus PIN number in the PIN code box.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 13 - Type the PIN number that was generated from the previous steps in the PIN code box with lower case.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 14 - Type the PIN number that was generated from the steps above with case sensitive.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 15 - Check for Email Study To Guest/Guest Login Review Study.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 16 - PIN number with different length and Numerical
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                //Enable PIN
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                CheckBox Enablepin = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab, "Enable PIN System for Non-Registered Users", 1);
                Enablepin.Checked = true;
                //Mixed
                ComboBox comboBox = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox.Select("Numeric");
                //Set pinsize to 6
                TextBox pinsize = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, "AutoSelectTextBox", 0, "1");
                pinsize.Text = "8";
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 17 - Login iConnect Access as Administrator and select Studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step 18 - Load a study
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer viewer1 = LaunchStudy();
                ExecutedSteps++;

                //Step 19 - Select Email Study button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(viewer1.By_EmailStudyDiv()));
                if (IsElementVisible(viewer1.By_ToEmail()) &&
                    IsElementVisible(viewer1.By_ToName()) &&
                    IsElementVisible(viewer1.By_Reason()))
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

                //Step 20 - Enter a valid email addresses
                viewer1.ToEmailTxtBox().SendKeys(Email[4]);
                viewer1.ToNameTxtBox().SendKeys(Name[0]);
                viewer1.ReasonTxtBox().SendKeys(Reason[0]);
                viewer1.EmailStudySendBtn().Click();
                String pin2 = studies.FetchPin();
                if (!(pin2 == "") && pin2.All(char.IsDigit) && pin2.Length == 8)
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
                viewer1.CloseStudy();
                login.Logout();

                //Step 21 - Write down this pin number on a piece of paper.
                ExecutedSteps++;

                //Step 22 - Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 23 - Type a bogus PIN number in the PIN code box.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 24 - Type the PIN number that was generated (8 digit numbers) from the steps above.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 25 - PIN number with different length, Alphabetic and non case sensitive
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();

                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();

                //Enable PIN
                ITabPage subtab1 = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                CheckBox Enablepin1 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(subtab1, "Enable PIN System for Non-Registered Users", 1);
                Enablepin1.Checked = true;
                //Alphabetic
                ComboBox comboBox1 = WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                comboBox1.Select("Alphabetic");
                //Set pinsize to 8
                TextBox pinsize1 = wpfobject.GetUIItem<ITabPage, TextBox>(subtab1, "AutoSelectTextBox", 0, "1");
                pinsize1.Text = "8";

                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 26 - Login iConnect Access as Administrator and select Studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step 27 - Load a study
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                viewer = LaunchStudy();
                ExecutedSteps++;

                //Step 28 - Select Email Study button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                if (IsElementVisible(viewer.By_ToEmail()) &&
                    IsElementVisible(viewer.By_ToName()) &&
                    IsElementVisible(viewer.By_Reason()))
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

                //Step 29 - Enter a valid email addresses
                viewer.ToEmailTxtBox().SendKeys(Email[4]);
                viewer.ToNameTxtBox().SendKeys(Name[0]);
                viewer.ReasonTxtBox().SendKeys(Reason[0]);
                viewer.EmailStudySendBtn().Click();
                String pin3 = studies.FetchPin();
                if (!(pin3 == "") && !pin3.Any(char.IsDigit) && pin3.Length == 8)
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

                //Step 30 - Write down this pin number on a piece of paper.
                ExecutedSteps++;

                //Step 31 - Go to the destination email and select the URL
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 32 - Type a bogus PIN number in the PIN code box.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 33 - Type the PIN number that was generated (8 letter numbers) from the steps above with upper case.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Logout 
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
