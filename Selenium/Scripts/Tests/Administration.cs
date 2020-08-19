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
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Data;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.eHR;
using System.Diagnostics;

namespace Selenium.Scripts.Tests
{
    class Administration : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public EHR ehr { get; set; }
        public WpfObjects wpfobject;
        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Administration(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            ehr = new EHR();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
        }

        /// <summary> 
        /// Test 162873 - Password Policy Applied
        /// </summary>
        public TestCaseResult Test_162873(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BluRingViewer bluringviewer = new BluRingViewer();
            UserManagement usermanagement = null;
            UserPreferences userpreferences = new UserPreferences();
            string PatientName = string.Empty;
            string URL = string.Empty;
            string[] Users = null;
            string[] SecurityID = null;
            int resultcount = 0;
            try
            {
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName"));
                Users = new string[] { Config.adminUserName, GetUniqueUserId("SAUser"), GetUniqueUserId("DAUser"), GetUniqueUserId("User") };
                SecurityID = new string[] { Config.adminUserName + "-" + Config.adminPassword, Users[1] + "-" + Users[1], Users[2] + "-" + Users[2], Users[3] + "-" + Users[3] };

                //PreCondition
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing");
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateSystemAdminUser(Users[1], Config.adminGroupName);
                usermanagement.CreateDomainAdminUser(Users[2], Config.adminGroupName);
                usermanagement.CreateUser(Users[3], Config.adminGroupName, Config.adminRoleName);
                login.Logout();

                login.LoginIConnect(Users[1], Users[1]);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();

                login.LoginIConnect(Users[2], Users[2]);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();

                login.LoginIConnect(Users[3], Users[3]);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();

                //Step 1:
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[0], SecurityID: SecurityID[0], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
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

                //Step 2:
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[0], SecurityID: SecurityID[0] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
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

                //Step 3: 
                login.CommentXMLnode("id", "Bypass");
                login.UncommentXMLnode("id", "iCADatabase");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 4: 
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[0], SecurityID: SecurityID[0], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
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

                //Step 5:
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[0], SecurityID: SecurityID[0] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToIntegratorURL(URL);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                if (bluringviewer.AuthenticationErrorMsg().Text.Contains("Client authentication failed"))
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

                //Step 6:

                //System Admin
                login.CommentXMLnode("id", "iCADatabase");
                login.UncommentXMLnode("id", "Bypass");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Bypass Mode using Valid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[1], SecurityID: SecurityID[1], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System Admin viewed study Using ByPass Mode with Valid Security ID");
                }

                //Bypass Mode using Invalid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[1], SecurityID: SecurityID[1] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System Admin viewed study Using ByPass Mode with Invalid Security ID");
                }

                login.CommentXMLnode("id", "Bypass");
                login.UncommentXMLnode("id", "iCADatabase");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //ICA Database Mode using Valid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[1], SecurityID: SecurityID[1], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System Admin viewed study Using iCADatabase Mode with Valid Security ID");
                }

                //ICA Database Mode using Invalid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[1], SecurityID: SecurityID[1] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToIntegratorURL(URL);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                if (bluringviewer.AuthenticationErrorMsg().Text.Contains("Client authentication failed"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("System Admin viewed Error message Client authentication failed by Using iCADatabase Mode with InValid Security ID");
                }

                //Domain Admin
                login.CommentXMLnode("id", "iCADatabase");
                login.UncommentXMLnode("id", "Bypass");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Bypass Mode using Valid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[2], SecurityID: SecurityID[2], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Domain Admin viewed study Using ByPass Mode with Valid Security ID");
                }

                //Bypass Mode using Invalid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[2], SecurityID: SecurityID[2] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Domain Admin viewed study Using ByPass Mode with Invalid Security ID");
                }

                login.CommentXMLnode("id", "Bypass");
                login.UncommentXMLnode("id", "iCADatabase");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //ICA Database Mode using Valid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[2], SecurityID: SecurityID[2], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Domain Admin viewed study Using iCADatabase Mode with Valid Security ID");
                }

                //ICA Database Mode using Invalid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[2], SecurityID: SecurityID[2] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToIntegratorURL(URL);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                if (bluringviewer.AuthenticationErrorMsg().Text.Contains("Client authentication failed"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Domain Admin viewed Error message Client authentication failed by Using iCADatabase Mode with InValid Security ID");
                }

                //User
                login.CommentXMLnode("id", "iCADatabase");
                login.UncommentXMLnode("id", "Bypass");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Bypass Mode using Valid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[3], SecurityID: SecurityID[3], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("User viewed study Using ByPass Mode with Valid Security ID");
                }

                //Bypass Mode using Invalid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "Bypass", user: Users[3], SecurityID: SecurityID[3] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("User viewed study Using ByPass Mode with Invalid Security ID");
                }

                login.CommentXMLnode("id", "Bypass");
                login.UncommentXMLnode("id", "iCADatabase");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //ICA Database Mode using Valid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[3], SecurityID: SecurityID[3], usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (bluringviewer.studyPanel().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("User viewed study Using iCADatabase Mode with Valid Security ID");
                }

                //ICA Database Mode using Invalid Security ID
                ehr.LaunchEHR();
                ehr.SetCommonParameters(AuthProvider: "iCADatabase", user: Users[3], SecurityID: SecurityID[3] + "123", usersharing: "True", autoendsession: "True");
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(PatientName, "Patient_Name");
                ehr.SetSearchKeys_Study(login.GetHostName(Config.SanityPACS), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToIntegratorURL(URL);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                if (bluringviewer.AuthenticationErrorMsg().Text.Contains("Client authentication failed"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("User viewed Error message Client authentication failed by Using iCADatabase Mode with InValid Security ID");
                }
                if (resultcount == 12)
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
                return result;
            }
            finally
            {
                login.CommentXMLnode("id", "iCADatabase");
                login.UncommentXMLnode("id", "Bypass");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary> 
        /// Test 162874 - Validation of Last Login Date
        /// </summary>
        public TestCaseResult Test_162874(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variable
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BasePage basepage = new BasePage();
            UserManagement usermanagement = null;
            BluRingViewer viewer = null;
            UserPreferences userpreferences = new UserPreferences();
            string Accession = string.Empty;
            string ServerName = string.Empty;
            string LocalUser = GetUniqueUserId("User");
            string LdapUserName = Config.LdapAdminUserName;
            string LdapPassword = Config.LdapAdminPassword;
            DataBaseUtil DBUtil = new DataBaseUtil("sqlserver");
            string LastDateTime = string.Empty;
            string LocalUserDateTime = string.Empty;
            string LDAPUserDateTime = string.Empty;
            int resultcount = 0;
            string CloseURL = "http://" + Config.IConnectIP + "/webaccess/";
            string URL = string.Empty;
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 1, 0);
            try
            {
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                ServerName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ServerName");
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(LocalUser, Config.adminGroupName, Config.adminRoleName);
                login.Logout();
                DBUtil.ConnectSQLServerDB();
                //PreCondition
                try { DBUtil.ExecuteQuery("delete from IRUser WHERE USERID = " + "'" + LdapUserName + "';"); }
                catch { }
                basepage.ChangeAttributeValue(Config.DSAServerManagerConfiguration, "/server[@id='" + ServerName + "']", "enabled", "True");
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                //Enable Local and LDAP Database 
                servicetool.SetMode(2);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                login.LoginIConnect(LdapUserName, LdapPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                login.Logout();
                try { DBUtil.ExecuteQuery("update IRUser set LastLogonDate = NULL where USERID=" + "'" + LdapUserName + "';"); }
                catch { }

                //Step 1: 
                LocalUserDateTime = DBUtil.ExecuteQuery("SELECT LastLogonDate FROM IRUser WHERE USERID = " + "'" + LocalUser + "';")[0];
                if (string.IsNullOrWhiteSpace(LocalUserDateTime))
                {
                    Logger.Instance.InfoLog("Local User " + LocalUser + " LastLogonDate is set to NULL");
                    resultcount++;
                }
                LDAPUserDateTime = DBUtil.ExecuteQuery("SELECT LastLogonDate FROM IRUser WHERE USERID = " + "'" + LdapUserName + "';")[0];
                if (string.IsNullOrWhiteSpace(LDAPUserDateTime))
                {
                    Logger.Instance.InfoLog("LDAP User " + LdapUserName + " LastLogonDate is set to NULL");
                    resultcount++;
                }
                if (resultcount == 2)
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

                //Step 2: 
                LastDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff");
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 1 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                login.LoginIConnect(LocalUser, LocalUser);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step 3:
                LocalUserDateTime = DBUtil.ExecuteQuery("SELECT LastLogonDate FROM IRUser WHERE USERID = " + "'" + LocalUser + "';")[0];
                if (string.IsNullOrWhiteSpace(LocalUserDateTime))
                {
                    Logger.Instance.ErrorLog("Last Logon Date for Local User " + LocalUser + " is not updated");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    DateTime d1 = DateTime.Parse(LastDateTime);
                    DateTime d2 = DateTime.Parse(LocalUserDateTime);
                    if (d1 < d2)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("LastDateTime = " + LastDateTime);
                        Logger.Instance.InfoLog("LastDateTime = " + LocalUserDateTime);
                        Logger.Instance.ErrorLog("Last Logon Date for Local User " + LocalUser + " is not Properly Updated");
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step 4:
                login.Logout();
                ExecutedSteps++;

                //Step 5:
                LastDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.fff");
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 1 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                login.LoginIConnect(LdapUserName, LdapPassword);
                ExecutedSteps++;

                //Step 6:
                LDAPUserDateTime = DBUtil.ExecuteQuery("SELECT LastLogonDate FROM IRUser WHERE USERID = " + "'" + LdapUserName + "';")[0];
                if (string.IsNullOrWhiteSpace(LDAPUserDateTime))
                {
                    Logger.Instance.ErrorLog("Last Logon Date for LDAP User " + LdapUserName + " is not updated");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    DateTime d1 = DateTime.Parse(LastDateTime);
                    DateTime d2 = DateTime.Parse(LDAPUserDateTime);
                    if (d1 < d2)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("LastDateTime = " + LastDateTime);
                        Logger.Instance.InfoLog("LastDateTime = " + LDAPUserDateTime);
                        Logger.Instance.ErrorLog("Last Logon Date for LDAP User " + LdapUserName + " is not Properly Updated");
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step 7:
                login.Logout();
                ExecutedSteps++;

                //Step 8:
                LastDateTime = LocalUserDateTime;
                ehr.LaunchEHR();
                ehr.SetCommonParameters(user: LocalUser, closeurl: CloseURL, SecurityID: LocalUser + "-" + LocalUser, domain: Config.adminGroupName);
                ehr.SetSelectorOptions(showSelector: "False", showReport: "True");
                ehr.SetSearchKeys_Study(Accession);
                ehr.SetSearchKeys_Study(login.GetHostName(Config.EA91), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step 9:
                login.CreateNewSesion();
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (viewer.studyPanel().Displayed)
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

                //Step 10:
                LocalUserDateTime = DBUtil.ExecuteQuery("SELECT LastLogonDate FROM IRUser WHERE USERID = " + "'" + LocalUser + "';")[0];
                if (string.IsNullOrWhiteSpace(LocalUserDateTime))
                {
                    Logger.Instance.ErrorLog("Last Logon Date for Local User " + LocalUser + " is not updated");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    DateTime d1 = DateTime.Parse(LastDateTime);
                    DateTime d2 = DateTime.Parse(LocalUserDateTime);
                    if (d1 < d2)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("LastDateTime = " + LastDateTime);
                        Logger.Instance.InfoLog("LastDateTime = " + LocalUserDateTime);
                        Logger.Instance.ErrorLog("Last Logon Date for Local User " + LocalUser + " is not Properly Updated");
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step 11:
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                if (BasePage.Driver.FindElements(login.By_UserIdTxtBox()).Count == 1)
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

                //Step 12:
                LastDateTime = LDAPUserDateTime;
                ehr.LaunchEHR();
                ehr.SetCommonParameters(user: LdapUserName, closeurl: CloseURL, SecurityID: LdapUserName + "-" + LdapPassword, domain: Config.adminGroupName);
                ehr.SetSelectorOptions(showSelector: "False", showReport: "True");
                ehr.SetSearchKeys_Study(Accession);
                ehr.SetSearchKeys_Study(login.GetHostName(Config.EA91), "Datasource");
                URL = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: URL);
                if (viewer.studyPanel().Displayed)
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

                //Step 13:
                LDAPUserDateTime = DBUtil.ExecuteQuery("SELECT LastLogonDate FROM IRUser WHERE USERID = " + "'" + LdapUserName + "';")[0];
                if (string.IsNullOrWhiteSpace(LDAPUserDateTime))
                {
                    Logger.Instance.ErrorLog("Last Logon Date for LDAP User " + LdapUserName + " is not updated");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    DateTime d1 = DateTime.Parse(LastDateTime);
                    DateTime d2 = DateTime.Parse(LDAPUserDateTime);
                    if (d1 < d2)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("LastDateTime = " + LastDateTime);
                        Logger.Instance.InfoLog("LastDateTime = " + LDAPUserDateTime);
                        Logger.Instance.ErrorLog("Last Logon Date for LDAP User " + LdapUserName + " is not Properly Updated");
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step 14:
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                if (BasePage.Driver.FindElements(login.By_UserIdTxtBox()).Count == 1)
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
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                ehr.CloseEHR();
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
            }
        }

        /// <summary Testing0001>
        /// This Test is to validate Study Attachment
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29310(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Inbounds inbounds = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data                       
                String Username = Config.ph1UserName;
                String Password = Config.ph1Password;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] FilePaths = UploadFilePath.Split('=');
                String reportPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReportPath");
                String[] reportPaths = reportPath.Split('=');

                //Initial Setup - Step 1
                ExecutedSteps++;

                //Upload a DICOMstudy - Step 2
                ei.EIDicomUpload(Username, Password, Config.Dest1, FilePaths[0]);

                //Login as physician 
                login.LoginIConnect(Username, Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession[0]);

                Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[0], "Uploaded" });

                if (study1 != null)
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

                //Logout as User
                login.Logout();

                //Attach PDF with size above 2MB(between 1.5MB and 2MB) to a dicom file -step 3
                ei.EI_UploadDicomWithAttachment(Username, Password, Config.Dest1, FilePaths[1], reportPaths[0], "PDF");

                //Login as physician 
                login.LoginIConnect(Username, Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession[1]);

                Dictionary<string, string> study2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[1], "Uploaded" });

                if (study2 != null && study2["Number of Images"].Contains("2"))
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

                //Logout as User
                login.Logout();

                //Attach PDF with size below 2MB(between 1.5MB and 2MB) to a dicom file -step 4
                ei.EI_UploadDicomWithAttachment(Username, Password, Config.Dest1, FilePaths[2], reportPaths[1], "PDF");

                //Login as physician 
                login.LoginIConnect(Username, Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession[2]);

                Dictionary<string, string> study3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[2], "Uploaded" });

                if (study3 != null && study3["Number of Images"].Contains("2"))
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

                //Logout as User
                login.Logout();

                //Attach PDF with small size -step 5
                ei.EI_UploadDicomWithAttachment(Username, Password, Config.Dest1, FilePaths[3], reportPaths[2], "PDF");

                //Login as physician 
                login.LoginIConnect(Username, Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession[3]);

                Dictionary<string, string> study4 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[3], "Uploaded" });

                if (study4 != null && study4["Number of Images"].Contains("2"))
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

                //Logout as User
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Domain Edit
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29312(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String DomainName = "TestDomain" + new Random().Next(1000);
                String Description = "TestDescription" + new Random().Next(1000);
                String Institution = "Inst" + new Random().Next(1000);
                //String datasource = login.GetHostName(Config.DestinationPACS).ToUpper();
                String datasource = "VMSSA-5-38-91";
                String FirstName = "FirstName" + new Random().Next(1000);
                String LastName = "LastName" + new Random().Next(1000);
                String AdminPassword = "admPass" + new Random().Next(1000);
                String Rolename = "role" + new Random().Next(1000);
                String RoleDescription = "RoleDescription" + new Random().Next(1000);

                //Login as Administrator - step 1
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                //Navigate to Domain Management - step 2
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Click New domain - step 3
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#NewDomainButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#NewDomainButton")).Click();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForFrameLoad(10);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewDomain_Content")));

                //Validate Headings in New domain creation page
                if (BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text.ToLower().Equals("domain management") == true
                    && BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>span")).Text.ToLower().Equals("new domain"))
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

                //Click save without giving any details - step 4
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("Domain name is required."))
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

                //Enter Detail in Domain name and click Save - step 5
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Name")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Name")).SendKeys(DomainName);
                Logger.Instance.InfoLog("Domain Name --" + DomainName + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("Domain description is required."))
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

                //Enter Detail in Domain Description and click Save - step 6
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Description")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Description")).SendKeys(Description);
                Logger.Instance.InfoLog("Domain Description --" + Description + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                ///BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("Receiving Institution is required."))
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

                //Enter Detail in Receiving Institution and click Save - step 7
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution")).SendKeys(Institution);
                Logger.Instance.InfoLog("Receiving Institution --" + Institution + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("At least one data source is required."))
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

                //Connect a DataSource and click Save - step 8
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_DataInfo_DataSourceDisconnectedListBox")));
                domainmanagement.ConnectDataSource(datasource);
                Logger.Instance.InfoLog("DataSource --" + datasource + "is Connected.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("User ID must not be empty."))
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

                //Enter User ID and click Save - step 9
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")).SendKeys(DomainName);
                Logger.Instance.InfoLog("User ID --" + DomainName + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("User first name must be provided."))
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

                //Enter First name and click Save - step 10
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")).SendKeys(FirstName);
                Logger.Instance.InfoLog("First name --" + FirstName + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("User last name must be provided."))
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

                //Enter Last name and click Save - step 11
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName")).SendKeys(LastName);
                Logger.Instance.InfoLog("Last name --" + LastName + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("A password or E-mail address is required."))
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

                //Enter Password and click Save - step 12
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")).SendKeys(AdminPassword);
                Logger.Instance.InfoLog("Password --" + AdminPassword + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("Passwords do not match."))
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

                //Enter Confirm Password and click Save - step 13
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword")).SendKeys(AdminPassword);
                Logger.Instance.InfoLog("Confirm Password --" + AdminPassword + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("Role name must be specified."))
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

                //Enter Role Name and click Save - step 14
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name")).SendKeys(Rolename);
                Logger.Instance.InfoLog("Role Name --" + Rolename + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.Equals("Role description must be specified."))
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

                //Enter Role Description and click Save - step 15
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description")).SendKeys(RoleDescription);
                Logger.Instance.InfoLog("Role Description --" + RoleDescription + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Search domain
                domainmanagement.SearchDomain(DomainName);

                //Validate domain creation
                if (domainmanagement.DomainExists(DomainName))
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

                //Create Domain with already existing domain name - step 16 & 17
                try
                {
                    domainmanagement.CreateDomain(DomainName, Description, Institution, DomainName, datasource, LastName, FirstName,
                        AdminPassword, Rolename, RoleDescription);
                }
                catch (Exception) { }
                ExecutedSteps++;
                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.
                    Contains("The domain name you have chosen already exists."))
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

                String DomainName1 = DomainName + "D1";
                //Rename the domain and click save - step 18

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Name")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Name")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Name")).SendKeys(DomainName1);
                Logger.Instance.InfoLog("Domain Name --" + DomainName1 + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                //Validate error message showing "The Specified role already exists"
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.
                    Equals("The specified Role already exists."))
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

                String RoleName1 = Rolename + "R1";
                //Rename the admin role and click save - step 19
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name")).SendKeys(RoleName1);
                Logger.Instance.InfoLog("Role Name --" + RoleName1 + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                //Validate error message as "The specified username already exists"
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.
                    Equals("Failed to register user because user ID already exists."))
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

                //Rename the User ID and click save - step 20
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")));
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")).SendKeys(DomainName1 + "M");
                Logger.Instance.InfoLog("User ID --" + DomainName1 + "M" + "is entered.");
                domainmanagement.ClickSaveDomain();

                //Search domain
                domainmanagement.SearchDomain(DomainName1);

                //Validate domain creation
                if (domainmanagement.DomainExists(DomainName1))
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

                //Search and Select domain
                Actions action = new Actions(BasePage.Driver);
                domainmanagement.SelectDomain(DomainName1);

                //Right click the domain and select new domain option - step 21
                IList<IWebElement> d = Driver.FindElements(By.CssSelector("div.row tr td>span[title='" + DomainName1 + "']"));
                IWebElement DomainElement_RightClick = null;

                foreach (IWebElement elm in d)
                {
                    if (elm.Text.Equals(DomainName1))
                    {
                        DomainElement_RightClick = elm;
                        break;
                    }
                }

                action.ContextClick(DomainElement_RightClick).Build().Perform();

                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#GlobalGridContextMenuDiv div")));
                IList<IWebElement> DomainOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                int counter = 0;
                String[] options = new String[DomainOptions.Count];
                foreach (IWebElement option in DomainOptions)
                {
                    options[counter] = option.Text;
                    counter++;
                }

                if (options[0].Equals("Add New Domain") && options[1].Equals("Edit Domain") && options[2].Equals("Delete Domain"))
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

                //Choose the "New domain" option - Step 22
                action.MoveToElement(DomainOptions[0]).Click().Build().Perform();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewDomain_Content")));

                //Validate Headings in New domain creation page
                if (BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text.ToLower().Equals("domain management") == true
                    && BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>span")).Text.ToLower().Equals("new domain"))
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

                //Fill all details and create one new domain - step 23
                String DomainName2 = DomainName + "D2";
                String Rolename2 = Rolename + "R2";

                domainmanagement.ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name");
                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", DomainName2);
                Logger.Instance.InfoLog("Domain Name --" + DomainName2 + "is entered.");

                domainmanagement.ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description");
                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", Description);
                Logger.Instance.InfoLog("Domain Description --" + Description + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", Institution);
                Logger.Instance.InfoLog("Receiving Institution --" + Institution + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", DomainName2);
                Logger.Instance.InfoLog("User ID --" + DomainName2 + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", LastName);
                Logger.Instance.InfoLog("Last name --" + LastName + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", FirstName);
                Logger.Instance.InfoLog("First name --" + FirstName + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", AdminPassword);
                Logger.Instance.InfoLog("Password --" + AdminPassword + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", AdminPassword);
                Logger.Instance.InfoLog("Confirm Password --" + AdminPassword + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", Rolename2);
                Logger.Instance.InfoLog("Role Name --" + Rolename2 + "is entered.");

                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", RoleDescription);
                Logger.Instance.InfoLog("Role Description --" + RoleDescription + "is entered.");

                PageLoadWait.WaitForPageLoad(20);
                domainmanagement.ConnectDataSource(datasource);
                Logger.Instance.InfoLog("DataSource --" + datasource + "is Connected.");
                PageLoadWait.WaitForPageLoad(30);

                //Click save button
                domainmanagement.ClickSaveDomain();

                //Search domain
                domainmanagement.SearchDomain(DomainName2);

                //Validate domain creation
                if (domainmanagement.DomainExists(DomainName2))
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

                //Steps:-24 & 25 Not Automated Steps -- Test Case needs to be updated
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                /*
                //Select domain
                domainmanagement.SelectDomain(DomainName2);
                
                //Right click the domain and select edit domain option - step 24
                DomainOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                action.MoveToElement(DomainOptions[0]).Click().Build().Perform();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id*='Domain_Content']")));

                //Validate Headings in New domain creation page
                if (BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text.ToLower().Equals("domain management")
                    && BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>span")).Text.ToLower().Equals("edit domain"))
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

                //Edit already existing Domain name and Save changes- step 25
                String DomainName3 = DomainName + "D22";
                domainmanagement.ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name");
                domainmanagement.SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", DomainName3);
                Logger.Instance.InfoLog("Domain Name --" + DomainName3 + "is entered.");

                PageLoadWait.WaitForPageLoad(30);
                //Click save button
                domainmanagement.ClickSaveDomain();

                //Search domain
                domainmanagement.SearchDomain(DomainName3);

                //Validate domain creation
                if (domainmanagement.DomainExists(DomainName3))
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
                //Select domain
                domainmanagement.SelectDomain(DomainName2);

                //Right click the domain and select edit main option - step 26

                IList<IWebElement> dd = Driver.FindElements(By.CssSelector("div.row tr td>span[title='" + DomainName2 + "']"));
                DomainElement_RightClick = null;

                foreach (IWebElement elm in dd)
                {
                    if (elm.Text.Equals(DomainName2))
                    {
                        DomainElement_RightClick = elm;
                        break;
                    }
                }

                action.ContextClick(DomainElement_RightClick).Build().Perform();

                DomainOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                action.MoveToElement(DomainOptions[2]).Click().Build().Perform();

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv #ConfirmationDiv")));

                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

                //Click Confirm button to delete the domain - Step 27
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='ConfirmButton']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='ConfirmButton']")).Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[id$='ConfirmButton']")));

                //Search domain
                domainmanagement.SearchDomain(DomainName2);

                //Validate domain creation
                if (!domainmanagement.DomainExists(DomainName2))
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

                //Logout as User
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Role Edit
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29313(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String RoleName = "Role" + new Random().Next(1000);
                String RoleDescription = "Description" + new Random().Next(1000);

                //Login as Administrator - step 1
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                //Navigate to Domain Management - step 2
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                ExecutedSteps++;

                //Click New Role -step 3
                rolemanagement.ClickButtonInRole("new");

                //Sync up
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_NameDropDownList']")));

                //Validate Headings in New role creation page
                if (BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text.ToLower().Equals("role management")
                    && BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>span")).Text.ToLower().Equals("new role"))
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

                //Click Save without giving any details - step 4
                rolemanagement.ClickSaveRole();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.
                    Equals("Role name must be specified."))
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

                //Enter Detail in Role name and click Save - step 5
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Name']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Name']")).SendKeys(RoleName);
                Logger.Instance.InfoLog("Role Name --" + RoleName + "is entered.");
                rolemanagement.ClickSaveRole();

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.
                    Equals("Role description must be specified."))
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

                //Enter Detail in Role description and click Save - step 6
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Description']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Description']")).SendKeys(RoleDescription);
                Logger.Instance.InfoLog("Role Description --" + RoleDescription + "is entered.");
                rolemanagement.ClickSaveRole();

                //Search Created Role
                rolemanagement.SearchRole(RoleName);

                if (rolemanagement.RoleExists(RoleName))
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

                //Navigate to Domain Management - step 7
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                ExecutedSteps++;

                //Click New Role -step 8
                rolemanagement.ClickButtonInRole("new");//login.ClickButton("NewRoleButton");                

                //Sync up
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_NameDropDownList']")));

                //Validate Headings in New role creation page
                if (BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text.ToLower().Equals("role management")
                    && BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>span")).Text.ToLower().Equals("new role"))
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

                //Enter all details and click save - step 9
                //Enter Role name 
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Name']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Name']")).SendKeys(RoleName);
                Logger.Instance.InfoLog("Role Name --" + RoleName + "is entered.");

                //Enter Role description
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Description']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Description']")).SendKeys(RoleDescription);
                Logger.Instance.InfoLog("Role Description --" + RoleDescription + "is entered.");

                //Click Save
                rolemanagement.ClickSaveRole();

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")));
                //Validate Error message
                if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")).Text.ToLower().
                    Equals("the specified role name already exists."))
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

                //Rename role and save - Step 10
                String RoleName1 = RoleName + "R1";
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Name']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Name']")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Name']")).SendKeys(RoleName1);
                Logger.Instance.InfoLog("Role Name --" + RoleName1 + "is entered.");

                //Click Save
                rolemanagement.ClickSaveRole();

                //Validation
                if (rolemanagement.RoleExists(RoleName1))
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

                //Right click the domain and select new domain option - step 11
                rolemanagement.SelectRole(RoleName1);
                Actions action = new Actions(BasePage.Driver);
                action.ContextClick().Build().Perform();

                IList<IWebElement> RoleOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                int counter = 0;
                String[] options = new String[RoleOptions.Count];
                foreach (IWebElement option in RoleOptions)
                {
                    options[counter] = option.Text;
                    counter++;
                }

                if (options[0].Equals("Add New Role") && options[1].Equals("Edit Role") && options[2].Equals("Delete Role"))
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

                //Choose the "New domain" option - Step 12                
                RoleOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                action.MoveToElement(RoleOptions[0]).Click().Build().Perform();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRole_Content")));

                //Validate Headings in New domain creation page
                if (BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text.ToLower().Equals("role management")
                    && BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>span")).Text.ToLower().Equals("new role"))
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

                //Enter all details and click save - step 13
                String RoleName2 = RoleName + "R2";

                //Enter Role name 
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Name']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Name']")).SendKeys(RoleName2);
                Logger.Instance.InfoLog("Role Name --" + RoleName2 + "is entered.");

                //Enter Role description
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Description']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Description']")).SendKeys(RoleDescription);
                Logger.Instance.InfoLog("Role Description --" + RoleDescription + "is entered.");

                //Click Save
                rolemanagement.ClickSaveRole();

                //Validation
                if (rolemanagement.RoleExists(RoleName2))
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

                //Steps:-24 & 25 Not Automated Steps -- Test Case needs to be updated
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                /*
                //Right click the role and select edit role option - step 14
                action.ContextClick().Build().Perform();
                RoleOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                action.MoveToElement(RoleOptions[1]).Click().Build().Perform();

                //Sync-up
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EditRole_Content")));

                //Validate Headings in New domain creation page
                if (BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text.ToLower().Equals("role management")
                    && BasePage.Driver.FindElement(By.CssSelector("#Container_Heading>span")).Text.ToLower().Equals("edit role"))
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
                
                //Rename role and Save Changes - step 15
                String RoleName3 = RoleName + "R22";
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_RoleAccessFilter_Name']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Name']")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_Name']")).SendKeys(RoleName3);
                Logger.Instance.InfoLog("Role Name --" + RoleName3 + "is entered.");

                //Click Save
                rolemanagement.ClickSaveRole();

                //Search Role
                rolemanagement.SearchRole(RoleName3);
                rolemanagement.SelectRole(RoleName3);

                //Validation
                if (rolemanagement.RoleExists(RoleName3))
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

                //Right click the role and select delete role option - step 16
                rolemanagement.SelectRole(RoleName2);
                action.ContextClick().Build().Perform();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#GlobalGridContextMenuDiv div")));
                RoleOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                action.MoveToElement(RoleOptions[2]).Click().Build().Perform();

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv #ConfirmationDiv")));

                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

                //Click Confirm button to delete the domain - Step 17
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='ConfirmButton']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='ConfirmButton']")).Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[id$='ConfirmButton']")));

                //Validation
                if (!rolemanagement.RoleExists(RoleName2))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// User Edit
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29314(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            UserManagement usermanagement = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String UserID = "User" + new Random().Next(1000);
                String LastName = "LastName" + new Random().Next(1000);
                String FirstName = "FirstName" + new Random().Next(1000);
                String UserPassword = "UserPassword" + new Random().Next(1000);
                String roleName = "Staff";
                String DomainName = "SuperAdminGroup";

                //Login as Administrator - step 1
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                //Navigate to Domain Management - step 2
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Create a new user
                usermanagement.CreateUser(UserID, DomainName, roleName, 0, "", 1, UserPassword);

                //Click New User -step 3
                usermanagement.ClickButtonInUser("new");

                //Sync up
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewUesrDialogDiv")));

                //Validate Headings in New role creation page
                if (BasePage.Driver.FindElement(By.CssSelector("span[id$='NewUserLabel']")).Text.ToLower().Equals("new user"))
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

                //Click Create button without giving any details - Step 4
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_sharedNewUserControl_Button1")));
                BasePage.Driver.FindElement(By.CssSelector("input#m_sharedNewUserControl_Button1")).Click();

                if (BasePage.Driver.FindElement(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")).Text.
                    Equals("User ID must not be empty."))
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

                //Enter Details in all fields with already existing user name - step 5
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                login.SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", UserID);
                login.SetText("id", "m_sharedNewUserControl_UserInfo_LastName", LastName);
                login.SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", FirstName);
                login.SetText("id", "m_sharedNewUserControl_UserInfo_Password", UserPassword);
                login.SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", UserPassword);
                login.Click("cssselector", " #m_sharedNewUserControl_ChooseRoleDropDownList>option[value='" + roleName + "']");

                //Sync-Up
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);

                //Click create button
                login.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")));

                if (BasePage.Driver.FindElement(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")).Text.
                    Contains("user ID already exists"))
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

                //Click close button - Step 6
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewUesrDialogDiv .buttonRounded_small_blue")));
                BasePage.Driver.FindElement(By.CssSelector("#NewUesrDialogDiv .buttonRounded_small_blue")).Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#NewUesrDialogDiv")));

                if (!BasePage.Driver.FindElement(By.CssSelector("#NewUesrDialogDiv")).Displayed)
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

                //Click New User -step 7
                usermanagement.ClickButtonInUser("new");

                //Sync up
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewUesrDialogDiv")));

                //Validate Headings in New role creation page
                if (BasePage.Driver.FindElement(By.CssSelector("span[id$='NewUserLabel']")).Text.ToLower().Equals("new user"))
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

                //Click Create button without giving any details - Step 8
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_sharedNewUserControl_Button1")));
                BasePage.Driver.FindElement(By.CssSelector("input#m_sharedNewUserControl_Button1")).Click();

                if (BasePage.Driver.FindElement(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")).Text.
                    Equals("User ID must not be empty."))
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

                //Enter User ID - step 9
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                String UserID1 = UserID + "D11";
                login.SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", UserID1);
                Logger.Instance.InfoLog("User ID -->" + UserID1 + " is entered.");

                //Click create button
                login.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);

                //Validate error message
                if (BasePage.Driver.FindElement(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")).Text.
                    Equals("User first name must be provided."))
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

                //Enter First name - Step 10
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                login.SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", FirstName + "F");
                Logger.Instance.InfoLog("First Name -->" + FirstName + " is entered.");

                //Click create button
                login.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);

                //Validate error message
                if (BasePage.Driver.FindElement(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")).Text.
                    Equals("User last name must be provided."))
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

                //Enter Last name and Create - Step 11
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                login.SetText("id", "m_sharedNewUserControl_UserInfo_LastName", LastName + "L");
                Logger.Instance.InfoLog("Last Name -->" + LastName + " is entered.");

                //Click create button
                login.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);

                //Validate error message
                if (BasePage.Driver.FindElement(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")).Text.
                    Equals("A password or E-mail address is required."))
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

                //Enter Password and click create - Step 12
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                login.SetText("id", "m_sharedNewUserControl_UserInfo_Password", UserPassword);
                Logger.Instance.InfoLog("User Password -->" + UserPassword + " is entered.");

                //Click create button
                login.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);

                //Validate error message
                if (BasePage.Driver.FindElement(By.CssSelector("span#m_sharedNewUserControl_ErrorMessage")).Text.
                    Equals("Passwords do not match."))
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

                //Enter confirm password and click create - Step 13
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                login.SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", UserPassword);
                Logger.Instance.InfoLog("User Password for comparison-->" + UserPassword + " is entered.");

                //Click create button
                login.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);

                //Search User
                usermanagement.SearchUser(FirstName, DomainName);

                //Search user and validate       
                if (usermanagement.SearchUser(FirstName, DomainName))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }


        }

        public TestCaseResult Test_29301(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            UserManagement usermanagement = null;
            DomainManagement domainmanagement = null;
            UserPreferences userpreferences = null;
            MyProfile profile = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                string email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                //Step 1: "Login as System Administrator or Domain Administrator Account User Management Tab *^>^ *Select""New User"".User ID: testuser Password / Confirm Password: testuser."
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                string RoleName = createDomain[DomainManagement.DomainAttr.RoleName];
                string UserID = GetUniqueUserId();
                if (domainmanagement.SearchDomain(DomainName))
                {
                    domainmanagement.SelectDomain(DomainName);
                    domainmanagement.ClickEditDomain();
                    domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                    domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                    domainmanagement.ClickSaveEditDomain();
                }
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList(DomainName);
                usermanagement.CreateUser(UserID, RoleName, Password: UserID);
                if (usermanagement.IsUserExist(UserID, DomainName))
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
                //Step 2: Navigate to http//*^<^*server_hostname*^>^*/WebAccess/Default.ashx
                login.Logout();
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(20);
                if (login.LoginBtn().Displayed)
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

                //Step 3: Log in using the username and password: testuser/testuser
                login.LoginIConnect(UserID, UserID);
                int tabcount = 0;
                string[] Tabs = { "Studies", "Patients", "Inbounds", "Outbounds" };
                string[] Tablist = login.TabsList().Select(tab => tab.Text).ToArray();
                foreach (string Tab in Tabs)
                {
                    if (Tablist.Contains(Tab))
                        tabcount++;
                }
                if (tabcount == 4)
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

                //Step 4: Hover over the ?Options? item in the top right corner
                if (BasePage.IsClickElementsExists(new string[] { "User Preferences", "My Profile", "Transfer Status" }))
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

                //Step 5: Choose ?User Preferences?.
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                bool options = true;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (!userpreferences.GrantYesEmailNotificationBtn().Selected)
                {
                    Logger.Instance.ErrorLog("Receive Email Notification is not Yes by default");
                    options = false;
                }
                if (!userpreferences.EmailFormatHTML().Selected)
                {
                    Logger.Instance.ErrorLog("EmailFormat HTML is not set by default");
                    options = false;
                }
                if (!userpreferences.JPEGRadioBtn().Selected)
                {
                    Logger.Instance.ErrorLog("JPEG Image Format is not set by default");
                    options = false;
                }
                if (!userpreferences.DefaultStartPageStudies().Selected)
                {
                    Logger.Instance.ErrorLog("Studies Start PAge is not set by default");
                    options = false;
                }
                if (!userpreferences.CineDefaultFrameRate().GetAttribute("value").Equals("20"))
                {
                    Logger.Instance.ErrorLog("CineDefaultFrameRate is not set to 20");
                    options = false;
                }
                if (!userpreferences.CineMaxMemory().GetAttribute("value").Equals("0"))
                {
                    Logger.Instance.ErrorLog("CineMaxMemory is not set to 0");
                    options = false;
                }
                if (userpreferences.EnableConnectionTestTool().Selected)
                {
                    Logger.Instance.ErrorLog("EnableConnectionTestTool is Selected by default");
                    options = false;
                }
                if (!userpreferences.DownloadStudiesAsZipFiles().Selected)
                {
                    Logger.Instance.ErrorLog("DownloadStudiesAsZipFiles is not set by default");
                    options = false;
                }
                if (!string.Equals(userpreferences.AllinOneLMB().SelectedOption.Text, "Window Level"))
                {
                    Logger.Instance.ErrorLog("All In One tool Left Button is not set Window Level by default");
                    options = false;
                }
                if (!string.Equals(userpreferences.AllinOneMMB().SelectedOption.Text, "Zoom"))
                {
                    Logger.Instance.ErrorLog("All In One tool Middle Button is not set Window Level by default");
                    options = false;
                }
                if (!string.Equals(userpreferences.AllinOneRMB().SelectedOption.Text, "Pan"))
                {
                    Logger.Instance.ErrorLog("All In One tool Right Button is not set Window Level by default");
                    options = false;
                }
                if (options)
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

                //Step 6: Change some of the values, and select OK
                userpreferences.PNGRadioBtn().Click();
                userpreferences.GrantNoEmailNotificationBtn().Click();
                userpreferences.EmailFormatText().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;
                //Step 7: Select Close
                ExecutedSteps++;
                //Step 8: Hover over the ?Options? item in the top right corner and choose"User Preferences".
                options = true;
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                if (!userpreferences.PNGRadioBtn().Selected)
                {
                    Logger.Instance.ErrorLog("PNGRadioBtn is not Selected");
                    options = false;
                }
                if (!userpreferences.GrantNoEmailNotificationBtn().Selected)
                {
                    Logger.Instance.ErrorLog("Email Notification which sets No is not Selected");
                    options = false;
                }
                if (!userpreferences.EmailFormatText().Selected)
                {
                    Logger.Instance.ErrorLog("EmailFormatText is not Selected");
                    options = false;
                }
                if (options)
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
                userpreferences.CloseUserPreferences();

                //Step 9: Hover over the ?Options? item in the top right corner and choose"My Profile".
                profile = new MyProfile();
                profile.OpenMyProfile();
                string soo = profile.UserLastName().GetAttribute("value");
                if (profile.UserLastName().Displayed && profile.UserFirstName().Displayed && profile.UserMiddleName().Displayed && profile.UserEmail().Displayed && profile.UserPwdTxtBox().Displayed && profile.UserConfirmPwdTxtBox().Displayed)
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

                //Step 10: Change some of the values, and select"Save"

                profile.UserEmail().Clear();
                profile.UserEmail().SendKeys(email);
                profile.SaveBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 11: Hover over the ?Options? item in the top right corner and choose"My Profile".
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                profile.OpenMyProfile();
                if (string.Equals(profile.UserEmail().GetAttribute("value"), email))
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
                profile.UserEmail().Clear();

                //Step 12: Change the Password/Confirm Password values, and select"Save".
                string newpassword = string.Concat("Test", UserID);
                profile.ChangePassword(newpassword);
                ExecutedSteps++;

                //Step 13: Logout and try to log back in as the same user with the old password.
                login.Logout();
                login.DriverGoTo(login.url);
                login.UserIdTxtBox().SendKeys(UserID);
                login.PasswordTxtBox().SendKeys(UserID);
                login.LoginBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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
                //Step 14: Logout and try to log back in as the same user with the new password.
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserID, newpassword);
                ExecutedSteps++;
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_29300(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            SystemSettings systemsettings = null;
            DomainManagement domainmanagement = null;
            Studies studies = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                //Precondition Create a domain

                login.LoginIConnect(Username, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                var domainattrA = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(domainattrA, null, true);
                string domadminuser = domainattrA[DomainManagement.DomainAttr.UserID];
                string domadminpwd = domainattrA[DomainManagement.DomainAttr.Password];

                login.Logout();


                //Step -1 Login as Domain Administrator
                login.LoginIConnect(domadminuser, domadminpwd);
                ExecutedSteps++;

                // Step 2 - Get all tab List
                if (!login.IsTabPresent("System Settings"))
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

                //Step 3 Logout and Login as Administrator
                login.Logout();
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;


                //Step 4 Select System Settings tab.
                systemsettings = (SystemSettings)login.Navigate("SystemSettings");

                if (!systemsettings.ShowOtherDocument().Selected)
                {
                    systemsettings.ShowOtherDocument().Click();
                }


                string gettheselect = systemsettings.DefaultStudySearchDateRange().SelectedOption.Text.Trim();
                string getLoginMsgAddressvalue = BasePage.Driver.FindElement(By.CssSelector("#LoginMessageAddressTB")).Text;
                bool res = (string.Equals(gettheselect.ToLower(), "Last 2 Days".ToLower()) || string.Equals(gettheselect.ToLower(), "All Dates".ToLower())) && string.IsNullOrWhiteSpace(getLoginMsgAddressvalue) && systemsettings.AllowUsertoSupperLoginMsg().Selected
                     && systemsettings.ShowRadiologyStudies().Selected && systemsettings.ShowXDS().Selected && systemsettings.ShowOtherDocument().Selected
                     && string.Equals(systemsettings.DefaultSearchDateRange().SelectedOption.Text.ToLower().Trim(), ("Last 18 Months".ToLower().Trim())) && systemsettings.SystemSettingsSaveButton().Displayed
                     && systemsettings.SystemSettingsCancelButton().Displayed && systemsettings.AllowUsertoSuppressLoginMsgResetButton().Displayed;
                //if (res)
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

                //We update the default setting in Environment set up.
                result.steps[++ExecutedSteps].status = "Not Automated";

                // Step 5 Change the Default Study Search Date Range to last Month, and select Save.	
                systemsettings.DefaultStudySearchDateRange().SelectByText("Last Month");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForFrameLoad(20);
                Driver.FindElement(By.CssSelector("#SaveSystemConfigButton")).Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#CloseResultButton")));
                if (Driver.FindElement(By.CssSelector("#CloseResultButton")).Displayed)
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

                // Step 6 
                // Select Close
                Driver.FindElement(By.CssSelector("#CloseResultButton")).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#SaveSystemConfigButton")));
                ExecutedSteps++;

                //// Step 7
                //result.steps[++ExecutedSteps].status = "Not Automated";

                // Select Studies Tab, observe the Study Performed value.Value is changed to the value set (Last Month)
                //studies = (Studies)login.Navigate("Studies");
                //if (studies.StudyPerformedDropDown().Text.Equals("Last Month"))
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


                // Step 8
                // Login as previously created user.
                login.Logout();
                login.LoginIConnect(domadminuser, domadminpwd);
                ExecutedSteps++;

                //Navigate to Studies tab
                studies = (Studies)login.Navigate("Studies");

                // Step 9
                // Select Studies Tab, observe the Study Performed value. 
                if (studies.StudyPerformedDropDown().Text.Equals("Last Month"))
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

                // Step 10
                // Select Search.	Only studies matching the criteria are displayed.
                studies.ClickButton("#m_studySearchControl_m_searchButton");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForSearchLoad();
                // Get the last month 
                string[] columnvalues = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Study Date", BasePage.GetColumnNames());
                int columnValuescount = columnvalues.Length;
                bool temp = true;
                foreach (string column in columnvalues)
                {
                    if (!studies.VerifyStudyPerformed(column, "Last Month"))
                    {
                        temp = false;
                        break;
                    }
                }

                if (temp)
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

                // Step 11 
                // Change the Study Performed value to something else, and select Search.
                // For testing purpose we have changed the select Last 2 month value from drop down
                studies.SearchStudy(Study_Performed_Period: "Last 2 Months");
                // verify that the table is displaying last two months value
                columnvalues = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Study Date", BasePage.GetColumnNames());
                columnValuescount = columnvalues.Length;
                temp = true;
                foreach (string column in columnvalues)
                {
                    if (!studies.VerifyStudyPerformed(column, "Last 2 Months"))
                    {
                        temp = false;
                        break;
                    }
                }

                if (temp)
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

                // Step 12
                //Logout as User
                login.Logout();
                login.LoginIConnect(domadminuser, domadminpwd);
                ExecutedSteps++;

                //Navigate to Studies tab
                studies = (Studies)login.Navigate("Studies");

                // Step 13
                // Select Studies Tab, observe the Study Performed value.Value is changed to the value set (Last Month)
                if (studies.StudyPerformedDropDown().Text.Equals("Last 2 Months"))
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                login.LoginIConnect(Username, Password);
                PageLoadWait.WaitForPageLoad(10);

                systemsettings = (SystemSettings)login.Navigate("SystemSettings");
                PageLoadWait.WaitForPageLoad(10);
                systemsettings.SetDateRange();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForFrameLoad(20);
                Driver.FindElement(By.CssSelector("#SaveSystemConfigButton")).Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#CloseResultButton")));
            }
        }

        public TestCaseResult Test_29298(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            UserManagement usermanagement = null;
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            Maintenance maintenance = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string RoleDA = string.Empty;
            string RoleU = string.Empty;
            string[] CoupleRole = Enumerable.Repeat(string.Empty, 2).ToArray();
            string[] Users = Enumerable.Repeat(string.Empty, 2).ToArray();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                //Step 1: Login as System Administrator and Create a Domain Administrator Account for Test Domain User Management Tab *^>^ *Select New Domain Admin. Login as System Administrator and Create a Domain Administrator Account for Test Domain User Management Tab*^>^*Select New Domain Admin. User ID -> DomAdmin Password/Confirm Password DomAdmin Enter any value for other required fields (Last Name, Role).User ID -> DomAdmin Password / Confirm Password DomAdmin Enter any value for other required fields(Last Name, Role).
                login.LoginIConnect(Username, Password);
                if (login.IsTabSelected("Domain Management"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                RoleDA = createDomain[DomainManagement.DomainAttr.RoleName];
                string DomainID = GetUniqueUserId("Admin");
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                RoleU = GetUniqueRole();
                usermanagement.CreateDomainAdminUser(DomainID, DomainName, RoleName: RoleU);
                PageLoadWait.WaitForPageLoad(30);

                ExecutedSteps++;

                //Step 2: Navigate to http//*^<^*server_hostname*^>^*WebAccess/Default.ashx
                login.Logout();
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step 3: Log in using the username and password: DomAdmin/DomAdmin
                login.LoginIConnect(DomainID, DomainID);
                string[] Tabs = { "Studies", "Patients", "Domain Management", "Role Management", "User Management", "Maintenance", "Inbounds", "Outbounds", "Image Sharing" };
                //string[] Tabs = { "Studies" };
                int tabcount = 0;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IList<IWebElement> tablist = login.TabsList();
                foreach (string tabs in Tabs)
                {
                    foreach (IWebElement tab in tablist)
                    {
                        if (tab.GetAttribute("innerHTML").ToLower().Equals(tabs.ToLower()))
                        {
                            tabcount++;
                            break;
                        }
                    }
                }
                if (tabcount == 9)
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

                //Step 4: Click on the Role Management tab.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (rolemanagement.SearchRoleBtn().Displayed && rolemanagement.ShowAllRoles().Displayed && rolemanagement.NewRoleBtn().Displayed && rolemanagement.EditRoleBtn().Displayed && rolemanagement.DeleteRoleBtn().Displayed && rolemanagement.LdapDataMapButtonRoleBtn().Displayed)
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

                //Step 5: Click on the New Role button.
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                rolemanagement.ClickButtonInRole("new");
                bool exists = true;
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                if (!string.Equals(rolemanagement.RoleHeading().Text, "New Role"))
                {
                    exists = false;
                }
                string[] DisplayItems = { "Enter Role Information", "Domain Information", " Role Information", "Access Filters Information", "Data Sources", "External Applications", "Filter Data Sources", "Selected Filter Criteria:", "Study Search Fields", "iPad Study List Fields", "Toolbar Configuration", "PatientHistory Layout", "StudyList Layout" };
                string labelvalues = string.Join(";", rolemanagement.Labels().Select(label => label.Text));
                foreach (string item in DisplayItems)
                {
                    if (!labelvalues.Contains(string.Concat(item, ";")))
                    {
                        exists = false;
                    }
                }
                if (exists)
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

                //Step 6: Fill in all of the fields, and click save.
                bool domaindropdown = !rolemanagement.Domainnamedropdown().Enabled;
                string newrole = GetUniqueRole();
                rolemanagement.RoleNameTxt().SendKeys(newrole);
                rolemanagement.RoleDescriptionTxt().SendKeys(newrole);
                rolemanagement.SaveBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                if (domaindropdown)
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

                //Step 7: Select the newly created Role.
                if (rolemanagement.RoleExists(newrole))
                {
                    rolemanagement.SelectRole(newrole);
                    ExecutedSteps++;
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8: select RoleNoUsers, select Delete
                rolemanagement.DeleteRoleBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForElementToDisplay(rolemanagement.ConfirmRoleDeletion(), 10);
                if (rolemanagement.ConfirmRoleDeletion().Displayed)
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

                //Step 9: Select Ok.
                rolemanagement.ConfirmRoleDeletion().Click();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                if (rolemanagement.RoleExists(newrole))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 10: Select RoleDA, and select Delete.
                rolemanagement.SearchRole(RoleDA);
                rolemanagement.SelectRole(RoleDA);
                rolemanagement.DeleteRoleBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (rolemanagement.AlertText().GetAttribute("innerHTML").Contains("Cannot delete the selected role"))
                {
                    rolemanagement.CloseAlertBox().Click();
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11: Select RoleU, and select Delete.
                rolemanagement.SearchRole(RoleU);
                rolemanagement.SelectRole(RoleU);
                rolemanagement.DeleteRoleBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (rolemanagement.AlertText().GetAttribute("innerHTML").Contains("Cannot delete the selected role"))
                {
                    rolemanagement.CloseAlertBox().Click();
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12: Select a role, and click on the Edit Role button.
                exists = true;
                rolemanagement.SearchRole(RoleDA);
                rolemanagement.SelectRole(RoleDA);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                if (!string.Equals(rolemanagement.RoleHeading().Text, "Edit Role"))
                {
                    exists = false;
                }
                string[] displayItems = { "Enter Role Information", "Domain Information", " Role Information", "Access Filters Information", "Data Sources", "External Applications", "Filter Data Sources", "Selected Filter Criteria:", "Study Search Fields", "iPad Study List Fields", "Toolbar Configuration", "PatientHistory Layout", "StudyList Layout" };
                labelvalues = string.Join(";", rolemanagement.Labels().Select(label => label.Text));
                foreach (string item in displayItems)
                {
                    if (!labelvalues.Contains(string.Concat(item, ";")))
                    {
                        exists = false;
                    }
                }
                if (!(!rolemanagement.Domainnamedropdown().Enabled) && (!rolemanagement.RoleNameTxt().Enabled))
                {
                    domaindropdown = !rolemanagement.Domainnamedropdown().Enabled;
                    exists = false;
                }
                if (exists)
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
                //Step 13: Modify the Role Description along with other settings and click the Save button.
                SendKeys(rolemanagement.RoleDescriptionTxt(), "RoleU");
                rolemanagement.ClickSaveEditRole();
                DataTable Role = CollectRecordsInTable(rolemanagement.RoleTable(), Row: rolemanagement.RoleTableColumn());
                string[] Description = GetColumnValues(Role, "Role Description");
                if (Description.Contains("RoleU"))
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
                //Step 14: Select the same role and click the Edit Role button.
                rolemanagement.SearchRole(RoleDA);
                rolemanagement.SelectRole(RoleDA);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                if (string.Equals(rolemanagement.RoleHeading().Text, "Edit Role") && string.Equals(rolemanagement.RoleDescriptionTxt().GetAttribute("value"), "RoleU"))
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
                //Step 15: Modify the Role Access Filter and click the Save button.
                string AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                rolemanagement.AccessFiltersInformation().SelectByValue("Accession Number");
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), AccessionNumber);
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;
                //Step 16: Select the same role and click the Edit Role button.
                rolemanagement.SearchRole(RoleDA);
                rolemanagement.SelectRole(RoleDA);
                rolemanagement.ClickButtonInRole("edit");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                if (string.Equals(rolemanagement.RoleHeading().Text, "Edit Role") && (rolemanagement.SelectedFilterCriteria().Options.Select(filter => filter.Text).ToArray().Contains(string.Concat("Accession Number = ", AccessionNumber))))
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
                //Step 17: Click the Close button.
                rolemanagement.CloseRoleManagement();
                if (login.IsTabSelected("Role Management"))
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
                //Step 18: Create a couple other Roles.
                CoupleRole[0] = GetUniqueRole();
                rolemanagement.CreateRole(DomainName, CoupleRole[0], "both", domainadmin: true);
                CoupleRole[1] = GetUniqueRole();
                rolemanagement.CreateRole(DomainName, CoupleRole[1], "both", domainadmin: true);
                ExecutedSteps++;
                //Step 19:In the Role Name search field, enter first few chars of a Role name that was created and click the Search button.
                rolemanagement.SearchRole("Role");
                bool RoleCheckBox = !rolemanagement.ShowAllRoles().Selected;
                Role = CollectRecordsInTable(rolemanagement.RoleTable(), Row: rolemanagement.RoleTableColumn());
                string[] RoleName = GetColumnValues(Role, "Role Name");
                bool Rolesearch = RoleName.All(rolename => rolename.StartsWith("Role"));
                bool RoleCount = Convert.ToInt32(rolemanagement.RoleCount().Text.Trim().Split(' ')[0]) == RoleName.Length ? true : false;
                if (RoleCheckBox && Rolesearch && RoleCount)
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
                //Step 20: Click on each of the titles of the fields (Role Name, Role Description, and Domain)
                rolemanagement.ShowAllRoles().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                rolemanagement.RoleSort()[2].Click();
                bool RoleNameSort = false;
                bool RoleDescriptionSort = false;
                rolemanagement.RoleSort()[0].Click();
                string[] AscRoleName = GetColumnValues(CollectRecordsInTable(rolemanagement.RoleTable(), Row: rolemanagement.RoleTableColumn()), "Role Name");
                rolemanagement.RoleSort()[0].Click();
                string[] DescRoleName = GetColumnValues(CollectRecordsInTable(rolemanagement.RoleTable(), Row: rolemanagement.RoleTableColumn()), "Role Name");
                rolemanagement.RoleSort()[1].Click();
                string[] AscRoleDescription = GetColumnValues(CollectRecordsInTable(rolemanagement.RoleTable(), Row: rolemanagement.RoleTableColumn()), "Role Description");
                rolemanagement.RoleSort()[1].Click();
                string[] DescRoleDescription = GetColumnValues(CollectRecordsInTable(rolemanagement.RoleTable(), Row: rolemanagement.RoleTableColumn()), "Role Description");
                if (AscRoleName.SequenceEqual((AscRoleName.OrderBy(c => c).ToArray())) && DescRoleName.SequenceEqual(DescRoleName.OrderByDescending(c => c).ToArray()))
                {
                    RoleNameSort = true;
                }
                if (AscRoleDescription.SequenceEqual((AscRoleDescription.OrderBy(c => c).ToArray())) && DescRoleDescription.SequenceEqual(DescRoleDescription.OrderByDescending(c => c).ToArray()))
                {
                    RoleDescriptionSort = true;
                }
                if (RoleNameSort && RoleDescriptionSort)
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
                //Step 21: Select the Maintenance tab.
                maintenance = (Maintenance)login.Navigate("Maintenance");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                if (string.Equals(string.Join(";", maintenance.InnerTab().Select(tab => tab.Text).ToArray()), "Audit;Log"))
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
                //Step 22: Naviagte to the User Management page and Click on the New User button.
                //Step 23: Fill in all of the fields and click save.
                login.Navigate("UserManagement");
                Users[0] = GetUniqueUserId();
                usermanagement.CreateUser(Users[0], CoupleRole[0]);
                ExecutedSteps++;
                if (login.IsTabSelected("User Management"))
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
                //Step 24: Click on the New User button.
                //Step 25: Fill in all of the fields, and select a role from the Role drop down list, and click save.
                Users[1] = GetUniqueUserId();
                usermanagement.CreateUser(Users[1], CoupleRole[1]);
                ExecutedSteps++;
                if (login.IsTabSelected("User Management"))
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
                //Step 26: Verify there is no"New Domain Admin"button
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (IsElementVisible(By.Id("NewDomainAdminButon")))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 27: Logout of Domain Administrator account.
                login.Logout();
                ExecutedSteps++;
                //Step 28: Login as System Administrator.
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                //Step 29: Select User Management, select TestDomain then select the Search button with nothing entered in the Filter Group/Users
                login.Navigate("UserManagement");
                usermanagement.SearchWithoutFilter(DomainName);
                string[] users = usermanagement.UserList().Select(user => user.Text).ToArray();
                bool user1 = users.Any(user => user.Contains(Users[0]));
                bool user2 = users.Any(user => user.Contains(Users[1]));
                if (user1 && user2)
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
                //Step 30: Verify the user created by Domain Administrator is only associated to the Test Domain.
                int usermapping = 0;
                foreach (string user in Users)
                {
                    usermanagement.UserControl(user, "view", DomainName);
                    if (string.Equals(usermanagement.DomainAdmin_DomainName_Dropdown().SelectedOption.Text, DomainName))
                    {
                        usermapping++;
                    }
                    usermanagement.CloseBtn().Click();
                }
                if (usermapping == Users.Length)
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
                //Step 31: Select the newly created User
                //Step 32: Click on Edit Button
                usermanagement.UserControl(Users[0], "edit", DomainName);
                ExecutedSteps++;
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(usermanagement.PageHeaderLabel().Text, "Edit User") && (!usermanagement.DomainDropDownName().Enabled))
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
                //Step 33: Modify the User's first name along with other details and click the Save button.
                SendKeys(usermanagement.FirstNameTxtBox(), DomainName);
                usermanagement.SaveBtn().Click();
                usermanagement.SearchWithoutFilter(DomainName);
                users = usermanagement.UserList().Select(user => user.Text).ToArray();
                exists = false;
                foreach (string user in users)
                {
                    if (user.Contains(Users[0]) && user.Contains(DomainName))
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
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
                //Step 34: Select the same user, and click Edit button.
                usermanagement.UserControl(Users[0], "edit", DomainName);
                if (string.Equals(usermanagement.PageHeaderLabel().Text, "Edit User") && (!usermanagement.DomainDropDownName().Enabled))
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
                //Step 35: Select Use/Modify Existing Role, and select a role from the Choose Role drop down list.
                usermanagement.UseModifyRoleDropDown().Click();
                string[] Roles = usermanagement.GetUseModifyRoleDropDown().Options.Select(role => role.Text).ToArray();
                exists = true;
                foreach (string role in Roles)
                {
                    if (!(string.Equals(RoleDA, role) || string.Equals(RoleU, role) || CoupleRole.Contains(role)))
                    {
                        exists = false;
                        break;
                    }
                }
                if (exists)
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
                //Step 36: Click the Save button.
                usermanagement.SaveBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (login.IsTabSelected("User Management"))
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
                //Step 37: search and Select the newly created User.
                //Step 38: Click on Edit Button
                usermanagement.UserControl(Users[0], "edit", DomainName);
                ExecutedSteps++;
                if (string.Equals(usermanagement.PageHeaderLabel().Text, "Edit User") && (!usermanagement.DomainDropDownName().Enabled))
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
                //Step 39: Modify the User's password.
                usermanagement.EditUser(DomainName);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (login.IsTabSelected("User Management"))
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
                //Step 40: Logout of System Administrator account.
                login.Logout();
                ExecutedSteps++;
                //Step 41: Login as the new user with the old password.
                login.DriverGoTo(login.url);
                login.UserIdTxtBox().SendKeys(Users[0]);
                login.PasswordTxtBox().SendKeys(Users[0]);
                login.LoginBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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
                //Step 42: Login as the new user with the new password.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Users[0], DomainName);
                ExecutedSteps++;
                //Step 43: Logout as new user, and Login as DomAdmin.
                login.Logout();
                login.LoginIConnect(DomainID, DomainID);
                ExecutedSteps++;
                //Step 44: Select the User that was created and click the Deactive button.
                login.Navigate("UserManagement");
                usermanagement.UserControl(Users[0], "deactivate");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ExecutedSteps++;
                //Step 45: Log out and try logging in the account that is deactivated.
                login.Logout();
                login.DriverGoTo(login.url);
                login.UserIdTxtBox().SendKeys(Users[0]);
                login.PasswordTxtBox().SendKeys(DomainName);
                login.LoginBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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
                //Step 46: Log back in using the username and password: DomAdmin/DomAdmin
                login.DriverGoTo(login.url);
                login.LoginIConnect(DomainID, DomainID);
                ExecutedSteps++;
                //Step 47: Click on the User Management tab, select the deactivated User and click the Activate button.
                login.Navigate("UserManagement");
                usermanagement.UserControl(Users[0], "activate");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ExecutedSteps++;
                //Step 48: Log out and try logging in the account that is re-activated.
                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Users[0], DomainName);
                ExecutedSteps++;
                //Step 49: Log out and log back in using the username and password: DomAdmin/DomAdmin
                login.Logout();
                login.LoginIConnect(DomainID, DomainID);
                ExecutedSteps++;
                //Step 50: Click on the User Management tab, select the re-activated User and click the Delete button.
                login.Navigate("UserManagement");
                usermanagement.UserControl(Users[0], "delete");

                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (usermanagement.OkButtonConfirmGroupDeletionMsgBox().Displayed)
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
                //Step 51: Click Ok in the Confirmation dialog.
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                ExecutedSteps++;
                //Step 52: Logout of DomAdmin account.
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                login.Logout();
                ExecutedSteps++;
                //Step 53: Login as the deleted user.
                login.DriverGoTo(login.url);
                login.UserIdTxtBox().SendKeys(Users[0]);
                login.PasswordTxtBox().SendKeys(DomainName);
                login.LoginBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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
                //Step 54: Login as DomAdmin.
                login.DriverGoTo(login.url);
                login.LoginIConnect(DomainID, DomainID);
                ExecutedSteps++;
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }


        public TestCaseResult Test2_29302(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            DomainManagement domainmanagement = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            string selectedvalue = string.Empty;
            string firstname = string.Empty;
            bool dataexists = false;
            IList<IWebElement> SelectedFilter;
            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                login.LoginIConnect(username, password);
                if (login.IsTabSelected("DomainManagement"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                login.Logout();
                //Step 1: Login as the Domain Administrator.
                string DomainUserName = createDomain[DomainManagement.DomainAttr.UserID];
                string DomainPassword = createDomain[DomainManagement.DomainAttr.Password];
                login.LoginIConnect(DomainUserName, DomainPassword);
                string[] Tabs = { "User Management", "Role Management" };
                int tabcount = 0;
                foreach (string tab in Tabs)
                {
                    if (login.IsTabPresent(tab))
                    {
                        tabcount++;
                    }
                }
                if (tabcount == 2)
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
                //Step 2: Select Role Management, and Select New Role.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.NewRoleBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(rolemanagement.NewRoleLabel().Text, "New User"))
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
                // Step 3: Enter any value for all required fields.
                string Role = GetUniqueRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                rolemanagement.RoleNameTxt().SendKeys(Role);
                rolemanagement.RoleDescriptionTxt().SendKeys(Role);
                if (string.Equals(rolemanagement.RoleNameTxt().GetAttribute("value"), Role) && string.Equals(rolemanagement.RoleDescriptionTxt().GetAttribute("value"), Role))
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
                //Step 4: Select Access Filter drop down, and select Institution.
                rolemanagement.AccessFiltersInformation().SelectByValue("Institution");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 5: Type first two characters, select a value from the results, and select Add
                string Institution = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), Institution.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Institution = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 6: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Institution"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), Institution);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Institution = ", Institution)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 7: Select Access Filter drop down, and select Referring Physician.
                rolemanagement.AccessFiltersInformation().SelectByValue("Referring Physician");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 8: Type first two characters of Last Name, select a value from the results, and select Add
                string ReferringPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), ReferringPhysician.Split(':')[0].Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname).Replace(" ", "");
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("ReferringPhysician=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Referring Physician"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersLastName(), ReferringPhysician.Split(':')[0]);
                SendKeys(rolemanagement.RoleAccessFiltersFirstName(), ReferringPhysician.Split(':')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Referring Physician = ", ReferringPhysician.Split(':')[0], ", ", ReferringPhysician.Split(':')[1])))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 10 : Select Access Filter drop down, and select Modality
                rolemanagement.AccessFiltersInformation().SelectByValue("Modality");
                string[] modalities = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':')[0].Split(',');
                SelectedFilter = rolemanagement.ModalityFilter().Options;
                int count = 0;
                foreach (string Modality in modalities)
                {
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, Modality))
                        {
                            count++;
                            break;
                        }
                    }
                }
                if (count == modalities.Length)
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
                //Step 11: Select from list of modalities, and select Add.
                string modality = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':')[1];
                rolemanagement.ModalityFilter().DeselectAll();
                rolemanagement.ModalityFilter().SelectByValue(modality);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Modality = ", modality)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 12: Select Access Filter drop down, and select Body Part.
                rolemanagement.AccessFiltersInformation().SelectByValue("Body Part");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 13: Type first two characters, select a value from the results, and select Add
                string BodyPart = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BodyPart");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), BodyPart.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Body Part = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 14: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Body Part"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), BodyPart);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Body Part = ", BodyPart)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 15: Select Access Filter drop down, and select Study Entered Date.
                rolemanagement.AccessFiltersInformation().SelectByValue("Study Entered Date");
                if (rolemanagement.DateLastBtn().Displayed && rolemanagement.DateFromTo().Displayed && rolemanagement.DateAll().Displayed)
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
                //Step 16: Select Last, enter a numeric value in text field, select a value from the drop down, and select Add
                rolemanagement.DateLastBtn().Click();
                string StudyEnteredDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyEnteredDate");
                string LastDateResult = string.Concat("Study Entered Date = Last: ", StudyEnteredDate.Split(':')[0].Split(',')[0], " ", StudyEnteredDate.Split(':')[0].Split(',')[1]);
                rolemanagement.DateLastText().SendKeys(StudyEnteredDate.Split(':')[0].Split(',')[0]);
                rolemanagement.DateDropDown().SelectByValue(StudyEnteredDate.Split(':')[0].Split(',')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 17: Select All, and select Add
                rolemanagement.DateAll().Click();
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 18: Select From/To, manually enter values in form mm/dd/yyyy, and select Add
                string Date = String.Format("{0:dd-MMM-yyyy}", DateTime.ParseExact(StudyEnteredDate.Split(':')[1], "d-MMM-yyyy", CultureInfo.InvariantCulture));
                rolemanagement.RoleDateFrom().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerFrom()));
                BasePage.EnterDate_CustomSearch(Date, rolemanagement: true);
                rolemanagement.RoleDateTo().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerTo()));
                BasePage.EnterDate_CustomSearch(Date, "To", rolemanagement: true);
                rolemanagement.AddAccessFilters().Click();
                LastDateResult = string.Concat("Study Entered Date = From: ", Date, " To: ", Date);
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 19: Select Access Filter drop down, and select Study Date.
                rolemanagement.AccessFiltersInformation().SelectByValue("Study Date");
                if (rolemanagement.DateLastBtn().Displayed && rolemanagement.DateFromTo().Displayed && rolemanagement.DateAll().Displayed)
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
                //Step 20: Select Last, enter a numeric value in text field, select a value from the drop down, and select Add
                rolemanagement.DateLastBtn().Click();
                string StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                LastDateResult = string.Concat("Study Date = Last: ", StudyDate.Split(':')[0].Split(',')[0], " ", StudyDate.Split(':')[0].Split(',')[1]);
                rolemanagement.DateLastText().SendKeys(StudyDate.Split(':')[0].Split(',')[0]);
                rolemanagement.DateDropDown().SelectByValue(StudyDate.Split(':')[0].Split(',')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 21: Select All, and select Add
                rolemanagement.DateAll().Click();
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 22: Select From/To, manually enter values in form mm/dd/yyyy, and select Add
                Date = String.Format("{0:dd-MMM-yyyy}", DateTime.ParseExact(StudyDate.Split(':')[1], "d-MMM-yyyy", CultureInfo.InvariantCulture));
                rolemanagement.RoleDateFrom().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerFrom()));
                BasePage.EnterDate_CustomSearch(Date, rolemanagement: true);
                rolemanagement.RoleDateTo().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerTo()));
                BasePage.EnterDate_CustomSearch(Date, "To", rolemanagement: true);
                rolemanagement.AddAccessFilters().Click();
                LastDateResult = string.Concat("Study Date = From: ", Date, " To: ", Date);
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 23: Select Access Filter drop down, and select Patient Name.
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient Name");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 24: Type first two characters of Last Name, select a value from the results, and select Add
                string patientname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), patientname.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname);
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("PatientName=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 25: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient Name"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersLastName(), patientname.Split(':')[0]);
                SendKeys(rolemanagement.RoleAccessFiltersFirstName(), patientname.Split(':')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Patient Name = ", patientname.Split(':')[0], ", ", patientname.Split(':')[1])))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 26: Select Access Filter drop down, and select Accession Number.
                rolemanagement.AccessFiltersInformation().SelectByValue("Accession Number");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 27: Type first two characters, select a value from the results, and select Add
                string AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), AccessionNumber.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Accession Number = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Accession Number"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), AccessionNumber);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Accession Number = ", AccessionNumber)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 29: Select Access Filter drop down, and select Patient ID.
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient ID");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 30: Type first two characters, select a value from the results, and select Add
                string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), PatientID.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Patient ID = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 31: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient ID"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), PatientID);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Patient ID = ", PatientID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 32: Select Access Filter drop down, and select Reading Physician.
                rolemanagement.AccessFiltersInformation().SelectByValue("Reading Physician");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 33: Type first two characters of Last Name, select a value from the results, and select Add
                string ReadingPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReadingPhysician");
                rolemanagement.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), ReadingPhysician.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname);
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("ReadingPhysician=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 34: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Reading Physician"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersLastName(), ReadingPhysician.Split(':')[0]);
                SendKeys(rolemanagement.RoleAccessFiltersFirstName(), ReadingPhysician.Split(':')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Reading Physician = ", ReadingPhysician.Split(':')[0], ", ", ReadingPhysician.Split(':')[1])))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 35: Select Close
                rolemanagement.CloseRoleManagement();
                if (login.IsTabPresent("User Management"))
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
                //result.steps[++ExecutedSteps].status = "Not Automated";
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }



        public TestCaseResult Test_29299(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            SystemSettings systemsettings = null;
            DomainManagement domainmanagement = null;
            Studies studies = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String Domain1 = "Domain1_" + new Random().Next(1, 1000);
                String Role1 = "Role1_" + new Random().Next(1, 1000);
                String Group1 = "Group1_" + new Random().Next(1, 1000);
                String Group2 = "Group2_" + new Random().Next(1, 1000);
                String Group3 = "Group3_" + new Random().Next(1, 1000);
                String Group4 = "Group4_" + new Random().Next(1, 1000);

                String SubGroup1 = "SubGroup1_" + new Random().Next(1, 1000);

                String User1 = "User1_" + new Random().Next(1, 1000);
                String User2 = "User2_" + new Random().Next(1, 1000);
                String User3 = "User3_" + new Random().Next(1, 1000);
                String User4 = "User4_" + new Random().Next(1, 1000);

                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String LDAPDomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP DomainName");
                String LDAPDomainAdmin = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP DomainAdmin");
                String LDAPInstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP InstituitionName");
                String LDAPDomainDesc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP DomainDescription");
                String LDAPPassword = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LDAP Password");

                String EA91 = login.GetHostName(Config.EA91);
                String PACSA7 = login.GetHostName(Config.SanityPACS);
                String HoldingPen = "DATASOURCE1";

                //Step 1
                //Login as System Admin
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                // Verify the domain exists in the application
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //if (!domainmanagement.SearchDomain("D1"))
                if (!domainmanagement.SearchDomain(Domain1))
                {
                    domainmanagement.CreateDomain(Domain1, LDAPDomainDesc, LDAPInstName, BasePage.GetUniqueUserId(), null, LDAPDomainAdmin,
                    LDAPDomainAdmin, LDAPPassword, Role1, Role1 + "_Description", 1, new string[] { EA91, PACSA7, HoldingPen });
                }


                //Verify the role exists in the application
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists(Role1, Domain1))
                {
                    rolemanagement.CreateRole(Domain1, Role1, "both");
                }

                //Step 2
                //Select User Management tab. and select test domain D1 created
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                ExecutedSteps++;

                //Step 3 & 4
                //Select New Group button G1 
                // Enter Name your Group = G1 Group Description = G1 Select yes in the Group management section In the Group is managed by: Select New User in the drop down list. In the New user popup enter the following. User ID = U4 Last Name = U4 First Name = U4 E - mail Address: emailName@merge.com    Password = U4 Confirm Password = U4
                // Role = Role1 Select Create Select the new user in the Group Manangement dropdown.  Select Save and view my Groups
                usermanagement.CreateGroup(Domain1, Group1, password: User1, rolename: Role1, email: Email, GroupUser: User1, IsManaged: 1, rolenames: new string[] { Role1 }, selectalldatasources: 1, selectallroles: 1);
                ExecutedSteps++;
                if (usermanagement.SearchGroup(Group1, Domain1, 0))
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



                //Step 5 & 6
                //Select New Group button G2
                // EnterName your Group = G2Group Description = G2Select yes in the Group management section In the Group is managed by: Select *^<^ *New User *^>^ * in the drop down list.              In the New user popup enter the following.             User ID = U5             Last Name = U5             First Name = U5             E - mail Address: emailName @merge.com Password = U5             Confirm Password = U5             Role = Role1             Select CreateSelect the new user in the Group Manangement dropdown. Select Save and view my Groups
                usermanagement.CreateGroup(Domain1, Group2, password: User2, rolename: Role1, email: Email, GroupUser: User2, IsManaged: 1, rolenames: new string[] { Role1 }, selectalldatasources: 1, selectallroles: 1);
                ExecutedSteps++;
                if (usermanagement.SearchGroup(Group2, Domain1, 0))
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
                // Select"G2"in the listSelect"New User"button. In the New user popup  enter the following. User ID = U6 Last Name = U6 First Name = U6             E-mail Address: emailName@merge.com             Password = U6             Confirm Password  = U6             Role  = Role1             Select Create
                usermanagement.SelectGroup(Group2, Domain1);
                usermanagement.CreateUserForGroup(Group2, User3, Domain1, Role1, hasEmail: 1, emailId: Email, hasPass: 1, Password: User3);
                if (usermanagement.SearchUser(User3))
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


                //Step 8
                // Click on user group G2  and select the Move Users tab
                usermanagement.ClearBtn();
                usermanagement.SearchGroup(Group2, Domain1, 0);
                usermanagement.SelectGroup(Group2, Domain1);
                usermanagement.MoveUsrBtn().Click();
                if (usermanagement.ManageUserPopupWindow().Displayed)
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
                // search and select user u6 and select ADD button
                usermanagement.FilterUserNameTextBox().Clear();
                usermanagement.FilterUserNameTextBox().SendKeys(User3);
                usermanagement.FilterUserNameSearchButton().Click();
                usermanagement.SearchAndSelectUser(User3);
                usermanagement.AddButtonInManageUsersPopup().Click();
                if (Driver.FindElement(By.CssSelector("#SelectedUsersTableBody tr[id^='Selected_'] span")).Text.ToLowerInvariant().Contains(User3.ToLowerInvariant()))
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


                // Step 10
                // Select the Group destination on the bottom left under Move To: (click on G1)
                usermanagement.SelectMoveToElement(Group1);
                if (Driver.FindElement(By.CssSelector("#hierarchyList_destination div[class$='itemListHighlight'] div[class='groupListTitleDiv'] ")).Text.ToLowerInvariant().Contains(Group1.ToLowerInvariant()))
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


                // Step 11
                // Select move
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.MoveButtonInManageUsersPopup()));
                PageLoadWait.WaitForPageLoad(20);
                usermanagement.MoveButtonInManageUsersPopup().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (Driver.FindElements(By.CssSelector("#SelectedUsersTableBody tr")).Count == 0)
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

                // Step 12
                // Click on done
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.DoneButtonInManageUsersPopup()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click();", usermanagement.DoneButtonInManageUsersPopup());
                if (login.IsTabPresent("User Management"))
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


                // Step 13 
                // Click on G1 under groups.and search for user u6
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchGroup(Group1, Domain1, 0);
                usermanagement.SelectGroup(Group1, Domain1);
                if (usermanagement.SearchUser(User3))
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

                // Step 14 
                // Click on G2 under groups and search for user u6
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.SearchGroup(Group2, Domain1, 0);
                usermanagement.SelectGroup(Group2, Domain1);
                if (!usermanagement.SearchUser(User3))
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



                // Step 15
                //while G2 is still selected  click on Edit Group
                PageLoadWait.WaitForPageLoad(10);
                usermanagement.EditGrpBtn().Click();
                // BasePage.Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                // BasePage.Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (usermanagement.CreateAndEditGroupPopupWindow().Displayed)
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


                // Step 16 
                // Change some of the fields and click on"Save and View my Groups"
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.GroupDescTxtBox().SendKeys(" adding description");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.SaveAndViewMyGroupBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click();", usermanagement.SaveAndViewMyGroupBtn());
                PageLoadWait.WaitForFrameLoad(10);
                // BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.SearchGroup(Group2, Domain1, 0);
                usermanagement.SelectGroup(Group2, Domain1);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement ta = Driver.FindElements(By.CssSelector("#hierarchyList_0 div[class ='groupListTitleDiv']")).Single<IWebElement>(d => d.Displayed);
                if (ta.Text.ToLowerInvariant().Contains(" adding description"))
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




                // Step 17
                // Click on Edit Group again
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.EditGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.GroupDescTxtBox().SendKeys(" another change");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(usermanagement.SaveAndViewMyGroupBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click();", usermanagement.SaveAndViewMyGroupBtn());
                usermanagement.SearchGroup(Group2, Domain1, 0);
                IWebElement ta1 = Driver.FindElements(By.CssSelector("#hierarchyList_0 div[class ='groupListTitleDiv']")).Single<IWebElement>(d => d.Displayed);
                if (ta1.Text.ToLowerInvariant().Contains(" another change"))
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


                // Step 18 
                // while G2 is still selected , click on the Delete group button.
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.SelectGroup(Group2, Domain1);
                usermanagement.DelGrpBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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



                // Step 19
                // Select Cancel
                usermanagement.CancelButtonInConfirmGroupDeletionMsgBox().Click();
                if (usermanagement.SearchGroup(Group2, Domain1, 0))
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

                // Step 20 
                // Select Delete group again
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.SelectGroup(Group2, Domain1);
                usermanagement.DelGrpBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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



                // Step 21
                // Click on Ok in the Confirmation dialog.
                PageLoadWait.WaitForPageLoad(20);
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                if (!usermanagement.SearchGroup(Group2, Domain1, 0))
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


                //Step 22
                // Do not select any group and search for G2 group admin U5
                usermanagement.SearchGroup(Group2, Domain1, 0);
                if (!usermanagement.SearchUser(User2))
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

                //Step 23 
                // Create two new groups G3 
                usermanagement.CreateGroup(Domain1, Group3);
                // Create two new groups G4
                usermanagement.CreateGroup(Domain1, Group4);
                if (usermanagement.SearchGroup(Group3, Domain1, 0) && usermanagement.SearchGroup(Group4, Domain1, 0))
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


                // Step 24
                //Select the group G3  and click on the Move Group button
                usermanagement.SearchGroup(Group3, Domain1, 0);
                usermanagement.SelectGroup(Group3, Domain1);
                usermanagement.MoveGrpBtn().Click();
                bool ta2 = Driver.FindElements(By.CssSelector("#destGroupsDiv div[class ='groupListTitleDiv']")).Any(d1 => d1.Text.ToLowerInvariant().Contains("ungrouped".ToLowerInvariant()));
                if (ta2)
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



                // Step 25
                // Select the G1 group and click on Move
                usermanagement.SelectMoveToElement(Group1);
                usermanagement.MoveButtonInManageUsersPopup().Click();
                if (usermanagement.SearchGroup(Group1, Domain1, check: 0))
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


                //Step 26
                // Click on the Triangle ajacent to G1, the triangle points UP/ or DOWN.Click several times notice display changes.
                usermanagement.SearchGroup(Group1, Domain1, 0);
                if (usermanagement.SelectSubGroup(Group1, Group3))
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

                // Step 27
                // Select a Subgroup under G1 and click Delete Group button
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                usermanagement.DelGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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


                // Step 28
                // Click on Ok in the Confirmation dialog.
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                if (!usermanagement.SearchGroup(Group3, Domain1, check: 1))
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

                //Step 29 & Step 30
                // Select the G4 group and click on the New SubGroup  button
                // Fill in all the fields and click on Save and View My groups
                usermanagement.SearchGroup(Group4, Domain1, 0);
                usermanagement.CreateSubGroup(Group4, SubGroup1, IsManaged: 0);
                if (usermanagement.SearchGroup(SubGroup1, Domain1, check: 1) && usermanagement.SearchGroup(Group4, Domain1, check: 0))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 31
                // Select the SuperAdminGroup
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                SelectElement srte = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_listResultsControl_m_resultsSelectorControl_m_selectorList")));
                if (srte.SelectedOption.Text.ToLowerInvariant().Contains("SuperAdminGroup".ToLowerInvariant()))
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

                //Step 32
                //Enter a G in the search field then Click Search
                usermanagement.SearchGroup("G", Domain1, 0);
                bool ta2as = Driver.FindElements(By.CssSelector("#hierarchyList_0 div[class ='groupListTitleDiv']")).Any(d1 => d1.Text.ToLowerInvariant().Contains("ungrouped".ToLowerInvariant()));
                if (ta2as)
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

                //Step 33
                // Change the Domain to D1
                usermanagement.SelectDomainFromDropdownList(Domain1);
                SelectElement srte1 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_listResultsControl_m_resultsSelectorControl_m_selectorList")));
                if (srte1.SelectedOption.Text.ToLowerInvariant().Contains(Domain1.ToLowerInvariant()))
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

                //Step 34
                // Clear the search field, Click the search button,
                usermanagement.ClearBtn().Click();
                usermanagement.SearchBtn().Click();
                if (usermanagement.SearchGroup(SubGroup1, Domain1, check: 1) && usermanagement.SearchGroup("Ungrouped", Domain1, check: 0))
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


                //Step 35
                // Enter G2 in the search field and click on Search
                usermanagement.ClearBtn();
                if (!usermanagement.SearchGroup(Group2, Domain1, check: 0))
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

                //Step 36
                // Enter G4 in the field and click on Search
                usermanagement.ClearBtn();
                if (usermanagement.SearchGroup(Group4, Domain1, check: 0))
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

                //Step 37
                // Enter U in the search field and click on Search
                usermanagement.ClearUsrBtn();
                if (usermanagement.SearchUser("Us", Domain1))
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

                //Step 38
                // Enter U5 in the search field and click on Search
                usermanagement.ClearUsrBtn();
                if (!usermanagement.SearchUser(User2, Domain1))
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test1_29302(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            DomainManagement domainmanagement = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            IList<IWebElement> SelectedFilter;
            string DataSource = string.Empty;
            string ConnectDataSource = string.Empty;
            string filterDataSource = string.Empty;
            bool dataexists = false;
            string selectedvalue = string.Empty;
            string firstname = string.Empty;
            try
            {
                //Step 1: Login as the System Administrator.
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                login.LoginIConnect(username, password);
                if (login.IsTabSelected("DomainManagement"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                if (domainmanagement.SearchDomain(DomainName))
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
                //Step 2: Select Role Management, and Select New Role.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.NewRoleBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(rolemanagement.NewRoleLabel().Text, "New Role"))
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
                // Step 3: Enter any value for all required fields.
                rolemanagement.DomainNameDropDown().SelectByValue(DomainName);
                string Role = GetUniqueRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                rolemanagement.RoleNameTxt().SendKeys(Role);
                rolemanagement.RoleDescriptionTxt().SendKeys(Role);
                if (string.Equals(rolemanagement.RoleNameTxt().GetAttribute("value"), Role) && string.Equals(rolemanagement.RoleDescriptionTxt().GetAttribute("value"), Role))
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
                //Step 4: Select Access Filter drop down, and select Institution.
                rolemanagement.AccessFiltersInformation().SelectByValue("Institution");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 5: Type first two characters of an institution, select a value from the results. In the filter DataSources select one of the datasources. and select Add
                string Institution = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                DataSource = rolemanagement.List_ConnectDatasource().FirstOrDefault().Text;
                rolemanagement.ConnectDataSource(DataSource);
                ConnectDataSource = string.Join(",", rolemanagement.List_ConnectedDatasource().Select(cds => cds.Text));
                filterDataSource = string.Join(",", rolemanagement.List_FilterDatasource().Select(fds => fds.Text));
                if (ConnectDataSource.Contains(DataSource) && filterDataSource.Contains(DataSource))
                {
                    BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), Institution.Substring(1, 2));
                    if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                    {
                        rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                        selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                        rolemanagement.AddAccessFilters().Click();
                        SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                        dataexists = false;
                        foreach (IWebElement filter in SelectedFilter)
                        {
                            if (string.Equals(filter.Text, string.Concat("Institution = ", selectedvalue)))
                            {
                                dataexists = true;
                                break;
                            }
                        }
                        if (dataexists)
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
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 6: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Institution"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), Institution);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Institution = ", Institution)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 7: Select Access Filter drop down, and select Referring Physician.
                rolemanagement.AccessFiltersInformation().SelectByValue("Referring Physician");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 8: Type first two characters of Last Name, select a value from the results, and select Add
                string ReferringPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), ReferringPhysician.Split(':')[0].Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname).Replace(" ", "");
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("ReferringPhysician=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Referring Physician"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersLastName(), ReferringPhysician.Split(':')[0]);
                SendKeys(rolemanagement.RoleAccessFiltersFirstName(), ReferringPhysician.Split(':')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Referring Physician = ", ReferringPhysician.Split(':')[0], ", ", ReferringPhysician.Split(':')[1])))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 10 : Select Access Filter drop down, and select Modality
                rolemanagement.AccessFiltersInformation().SelectByValue("Modality");
                string[] modalities = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':')[0].Split(',');
                SelectedFilter = rolemanagement.ModalityFilter().Options;
                int count = 0;
                foreach (string Modality in modalities)
                {
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, Modality))
                        {
                            count++;
                            break;
                        }
                    }
                }
                if (count == modalities.Length)
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
                //Step 11: Select from list of modalities, and select Add.
                string modality = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':')[1];
                rolemanagement.ModalityFilter().DeselectAll();
                rolemanagement.ModalityFilter().SelectByValue(modality);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Modality = ", modality)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 12: Select Access Filter drop down, and select Body Part.
                rolemanagement.AccessFiltersInformation().SelectByValue("Body Part");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 13: Type first two characters, select a value from the results, and select Add
                string BodyPart = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BodyPart");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), BodyPart.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Body Part = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 14: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Body Part"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), BodyPart);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Body Part = ", BodyPart)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 15: Select Access Filter drop down, and select Study Entered Date.
                rolemanagement.AccessFiltersInformation().SelectByValue("Study Entered Date");
                if (rolemanagement.DateLastBtn().Displayed && rolemanagement.DateFromTo().Displayed && rolemanagement.DateAll().Displayed)
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
                //Step 16: Select Last, enter a numeric value in text field, select a value from the drop down, and select Add
                rolemanagement.DateLastBtn().Click();
                string StudyEnteredDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyEnteredDate");
                string LastDateResult = string.Concat("Study Entered Date = Last: ", StudyEnteredDate.Split(':')[0].Split(',')[0], " ", StudyEnteredDate.Split(':')[0].Split(',')[1]);
                rolemanagement.DateLastText().SendKeys(StudyEnteredDate.Split(':')[0].Split(',')[0]);
                rolemanagement.DateDropDown().SelectByValue(StudyEnteredDate.Split(':')[0].Split(',')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 17: Select All, and select Add
                rolemanagement.DateAll().Click();
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 18: Select From/To, manually enter values in form mm/dd/yyyy, and select Add
                string Date = String.Format("{0:dd-MMM-yyyy}", DateTime.ParseExact(StudyEnteredDate.Split(':')[1], "d-MMM-yyyy", CultureInfo.InvariantCulture));
                rolemanagement.RoleDateFrom().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerFrom()));
                BasePage.EnterDate_CustomSearch(Date, rolemanagement: true);
                rolemanagement.RoleDateTo().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerTo()));
                BasePage.EnterDate_CustomSearch(Date, "To", rolemanagement: true);
                rolemanagement.AddAccessFilters().Click();
                LastDateResult = string.Concat("Study Entered Date = From: ", Date, " To: ", Date);
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 19: Select Access Filter drop down, and select Study Date.
                rolemanagement.AccessFiltersInformation().SelectByValue("Study Date");
                if (rolemanagement.DateLastBtn().Displayed && rolemanagement.DateFromTo().Displayed && rolemanagement.DateAll().Displayed)
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
                //Step 20: Select Last, enter a numeric value in text field, select a value from the drop down, and select Add
                rolemanagement.DateLastBtn().Click();
                string StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                LastDateResult = string.Concat("Study Date = Last: ", StudyDate.Split(':')[0].Split(',')[0], " ", StudyDate.Split(':')[0].Split(',')[1]);
                rolemanagement.DateLastText().SendKeys(StudyDate.Split(':')[0].Split(',')[0]);
                rolemanagement.DateDropDown().SelectByValue(StudyDate.Split(':')[0].Split(',')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 21: Select All, and select Add
                rolemanagement.DateAll().Click();
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 22: Select From/To, manually enter values in form mm/dd/yyyy, and select Add
                Date = String.Format("{0:dd-MMM-yyyy}", DateTime.ParseExact(StudyDate.Split(':')[1], "d-MMM-yyyy", CultureInfo.InvariantCulture));
                rolemanagement.RoleDateFrom().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerFrom()));
                BasePage.EnterDate_CustomSearch(Date, rolemanagement: true);
                rolemanagement.RoleDateTo().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerTo()));
                BasePage.EnterDate_CustomSearch(Date, "To", rolemanagement: true);
                rolemanagement.AddAccessFilters().Click();
                LastDateResult = string.Concat("Study Date = From: ", Date, " To: ", Date);
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 23: Select Access Filter drop down, and select Patient Name.
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient Name");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 24: Type first two characters of Last Name, select a value from the results, and select Add
                string patientname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), patientname.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname);
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("PatientName=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 25: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient Name"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersLastName(), patientname.Split(':')[0]);
                SendKeys(rolemanagement.RoleAccessFiltersFirstName(), patientname.Split(':')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Patient Name = ", patientname.Split(':')[0], ", ", patientname.Split(':')[1])))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 26: Select Access Filter drop down, and select Accession Number.
                rolemanagement.AccessFiltersInformation().SelectByValue("Accession Number");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 27: Type first two characters, select a value from the results, and select Add
                rolemanagement.RemoveDataSource();
                string AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                DataSource = rolemanagement.List_ConnectDatasource().FirstOrDefault().Text;
                rolemanagement.ConnectDataSource(DataSource);
                ConnectDataSource = string.Join(",", rolemanagement.List_ConnectedDatasource().Select(cds => cds.Text));
                filterDataSource = string.Join(",", rolemanagement.List_FilterDatasource().Select(fds => fds.Text));
                if (ConnectDataSource.Contains(DataSource) && filterDataSource.Contains(DataSource))
                {
                    BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), AccessionNumber.Substring(1, 2));
                    if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                    {
                        rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                        selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                        rolemanagement.AddAccessFilters().Click();
                        SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                        dataexists = false;
                        foreach (IWebElement filter in SelectedFilter)
                        {
                            if (string.Equals(filter.Text, string.Concat("Accession Number = ", selectedvalue)))
                            {
                                dataexists = true;
                                break;
                            }
                        }
                        if (dataexists)
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
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Accession Number"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), AccessionNumber);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Accession Number = ", AccessionNumber)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 29: Select Access Filter drop down, and select Patient ID.
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient ID");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 30: Type first two characters, select a value from the results, and select Add
                rolemanagement.RemoveDataSource();
                string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                DataSource = rolemanagement.List_ConnectDatasource().FirstOrDefault().Text;
                rolemanagement.ConnectDataSource(DataSource);
                ConnectDataSource = string.Join(",", rolemanagement.List_ConnectedDatasource().Select(cds => cds.Text));
                filterDataSource = string.Join(",", rolemanagement.List_FilterDatasource().Select(fds => fds.Text));
                if (ConnectDataSource.Contains(DataSource) && filterDataSource.Contains(DataSource))
                {
                    BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), PatientID.Substring(1, 2));
                    if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                    {
                        rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                        selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                        rolemanagement.AddAccessFilters().Click();
                        SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                        dataexists = false;
                        foreach (IWebElement filter in SelectedFilter)
                        {
                            if (string.Equals(filter.Text, string.Concat("Patient ID = ", selectedvalue)))
                            {
                                dataexists = true;
                                break;
                            }
                        }
                        if (dataexists)
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
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 31: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient ID"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), PatientID);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Patient ID = ", PatientID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 32: Select Access Filter drop down, and select Reading Physician.
                rolemanagement.AccessFiltersInformation().SelectByValue("Reading Physician");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 33: Type first two characters of Last Name, select a value from the results, and select Add
                string ReadingPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReadingPhysician");
                rolemanagement.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), ReadingPhysician.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname);
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("ReadingPhysician=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 34: Enter text manually, and select Add
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Reading Physician"))
                    {
                        filter.Click();
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                SendKeys(rolemanagement.RoleAccessFiltersLastName(), ReadingPhysician.Split(':')[0]);
                SendKeys(rolemanagement.RoleAccessFiltersFirstName(), ReadingPhysician.Split(':')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Reading Physician = ", ReadingPhysician.Split(':')[0], ", ", ReadingPhysician.Split(':')[1])))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 35: Select Close
                rolemanagement.CloseRoleManagement();
                if (login.IsTabPresent("Role Management"))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_29295(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            Patients patients = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);

                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String EA131 = login.GetHostName(Config.EA1);
                String PACSA7 = login.GetHostName(Config.SanityPACS);

                //Step 1

                ExecutedSteps++;
                //Step 2
                //Login as system administrator in ICA.
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;



                //Step 3
                //Select the Domain Management tab. Click on New Domain.
                // Step 4
                //In the name field enter DomainTestA. Fill in the remaining required fields and uncheck the"Allow Upload"checkbox. Save the changes then logout of WebAccess.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;
                domainmanagement.CreateDomain(DomainName, RoleName, DS: new string[] { "attachmentupload" }, check: 1);
                login.Logout();
                ExecutedSteps++;


                // Step 5
                //Login as the DomainTestA domain administrator and load a dataset from the studylist.
                login.LoginIConnect(DomainName, DomainName);
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                //Verify whether study loads into viewer
                if (viewer.ViewStudy())
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

                //Step-6 
                // Expand the History panel.
                viewer.NavigateToHistoryPanel();
                bool IsUploadLabelPresent = viewer.IsElementVisible(viewer.UploadLabelByObject());
                if (!IsUploadLabelPresent)
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

                // Step 7 
                // Close the viewer and select the Patients Tab.
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                patients = (Patients)login.Navigate("Patients");
                Boolean istabpresent = login.IsTabPresent("Patients");
                if (istabpresent)
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

                //// Step 8
                //// Search for a patient in the data source then load the patient into the PMJ. On the Radiology tab of the PMJ, select a study a load it.
                //// Test step needs update
                //result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 9
                login.Logout();
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.EditDomainButton().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (domainmanagement.PageHeaderLabel().Displayed)
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


                // Step 10
                // Uncheck the"Enable Attachments"checkbox.Save changes and logout of WebAccess.
                domainmanagement.SetCheckBoxInEditDomain("attachment", 1);
                domainmanagement.ClickSaveEditDomain();
                login.Logout();
                ExecutedSteps++;

                // Step 11
                //Login as DomainTestA administrator and load a dataset which should have attachments from the studylist.
                login.LoginIConnect(DomainName, DomainName);
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                // Expand the History panel.
                viewer.NavigateToHistoryPanel();
                bool IsAttachmentButtonPresent = viewer.IsElementVisible(viewer.AttachmentTabByObj());
                if (!IsAttachmentButtonPresent)
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
                // Close the viewer and select the Patients Tab.
                studies.CloseStudy();
                patients = (Patients)login.Navigate("Patients");
                Boolean ispatienttabpresent = login.IsTabPresent("Patients");
                if (istabpresent)
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

                //// Step 13
                //// Search for a patient in the data source then load the patient into the PMJ. On the Radiology tab of the PMJ, select a study a load it.
                //// Test step needs update
                //result.steps[++ExecutedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test3_29302(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            IList<IWebElement> SelectedFilter = null;
            bool dataexists = false;
            int ExecutedSteps = -1;
            string AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
            string IssuerofAdmissionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IssuerofAdmissionID");
            string StudyDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: login as Administrator. Navigate to role management page. edit test users role. remove all filters(if any)
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                login.LoginIConnect(username, password);
                if (login.IsTabSelected("DomainManagement"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                string RoleName = createDomain[DomainManagement.DomainAttr.RoleName];
                string user = GetUniqueUserId();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user, DomainName, RoleName);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    filter.Click();
                    rolemanagement.RemoveButton().Click();
                }
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                if (SelectedFilter.Count == 0)
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

                //Step 2 : from access filter drop down verify if Issuer of Admission ID" filter exist
                string[] AccessFilters = rolemanagement.AccessFiltersInformation().Options.Select(cds => cds.Text).ToArray();
                if (AccessFilters.Contains("Issuer of Admission ID"))
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

                //Step 3: Select "Issuer of Admission ID" filter and add some valid filter (for which study exist in data source)
                rolemanagement.AccessFiltersInformation().SelectByValue("Issuer of Admission ID");
                rolemanagement.RoleAccessFiltersTextBox().SendKeys(IssuerofAdmissionID);
                if (string.Equals(rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value"), IssuerofAdmissionID))
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
                //Step 4: Click on "Add" button to add the filter
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Issuer of Admission ID = ", IssuerofAdmissionID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 5: Click on "Save" button to save the filters
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Issuer of Admission ID = ", IssuerofAdmissionID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 6: "Login in icA as test user navigate to studies tab and perform *search"
                login.Logout();
                login.LoginIConnect(user, user);
                PageLoadWait.WaitForPageLoad(20);
                login.Navigate("Studies");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                SearchStudy(LastName: "*", Study_Performed_Period: "All Dates");
                Dictionary<int, string[]> SearchResults = GetSearchResults();
                if (SearchResults.Count > 0)
                {
                    string[] ColValue1 = GetColumnValues(SearchResults, "Accession", GetColumnNames());
                    string[] ColValue2 = GetColumnValues(SearchResults, "Description", GetColumnNames());
                    if (ColValue1.Contains(AccessionNumber) && ColValue2.Contains(StudyDescription))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test4_29302(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            IList<IWebElement> SelectedFilter = null;
            bool dataexists = false;
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: login as Administrator. Navigate to role management page. edit test users role. remove all filters(if any)
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                login.LoginIConnect(username, password);
                if (login.IsTabSelected("DomainManagement"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                string RoleName = createDomain[DomainManagement.DomainAttr.RoleName];
                string user = GetUniqueUserId();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user, DomainName, RoleName);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    filter.Click();
                    rolemanagement.RemoveButton().Click();
                }
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                if (SelectedFilter.Count == 0)
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

                //Step 2 : from access filter drop down verify if Issuer of Admission ID" filter exist
                string[] AccessFilters = rolemanagement.AccessFiltersInformation().Options.Select(cds => cds.Text).ToArray();
                if (AccessFilters.Contains("Issuer of Admission ID"))
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

                //Step 3: Select "Issuer of Admission ID" filter and add some valid filter (for which study does not exist in data source)
                string IssuerofAdmissionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IssuerofAdmissionID");
                rolemanagement.AccessFiltersInformation().SelectByValue("Issuer of Admission ID");
                rolemanagement.RoleAccessFiltersTextBox().SendKeys(IssuerofAdmissionID);
                if (string.Equals(rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value"), IssuerofAdmissionID))
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
                //Step 4: Click on "Add" button to add the filter
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Issuer of Admission ID = ", IssuerofAdmissionID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 5: Click on "Save" button to save the filters
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Issuer of Admission ID = ", IssuerofAdmissionID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 6:
                login.Logout();
                login.LoginIConnect(user, user);
                PageLoadWait.WaitForPageLoad(20);
                login.Navigate("Studies");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                SearchStudy(LastName: "*", Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForSearchLoad();
                Dictionary<int, string[]> SearchResults = GetSearchResults();
                if (SearchResults.Count == 0)
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
                //result.steps[++ExecutedSteps].status = "Not Automated";
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test5_29302(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            IList<IWebElement> SelectedFilter = null;
            bool dataexists = false;
            int ExecutedSteps = -1;
            IList<string> InputData = new List<string>();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: login as Administrator. Navigate to role management page. edit test users role. remove all filters(if any)
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                login.LoginIConnect(username, password);
                if (login.IsTabSelected("DomainManagement"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                string RoleName = createDomain[DomainManagement.DomainAttr.RoleName];
                string user = GetUniqueUserId();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user, DomainName, RoleName);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    filter.Click();
                    rolemanagement.RemoveButton().Click();
                }
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                if (SelectedFilter.Count == 0)
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

                //Step 2 : from access filter drop down verify if Issuer of Admission ID" filter exist
                string[] AccessFilters = rolemanagement.AccessFiltersInformation().Options.Select(cds => cds.Text).ToArray();
                if (AccessFilters.Contains("Issuer of Admission ID"))
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

                //Step 3: Select "Issuer of Admission ID" filter and add some valid filter (for which study does not exist in data source)
                string IssuerofAdmissionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IssuerofAdmissionID");
                rolemanagement.AccessFiltersInformation().SelectByValue("Issuer of Admission ID");
                rolemanagement.RoleAccessFiltersTextBox().SendKeys(IssuerofAdmissionID);
                if (string.Equals(rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value"), IssuerofAdmissionID))
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
                //Step 4: Click on "Add" button to add the filter
                rolemanagement.AddAccessFilters().Click();
                InputData.Add(string.Concat("IssuerofAdmissionID=", IssuerofAdmissionID));
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Issuer of Admission ID = ", IssuerofAdmissionID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 5: Select Access Filter drop down, and select Institution.
                rolemanagement.AccessFiltersInformation().SelectByValue("Institution");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 6: Type first two characters, select a value from the results, and select Add
                string Institution = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), Institution.Substring(1, 2));
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    string selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    rolemanagement.AddAccessFilters().Click();
                    InputData.Add(string.Concat("Institution=", selectedvalue.Replace(" ", "")));
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Institution = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 7: Click on "Save" button to save the filters
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                IList<string> accessfilters = SelectedFilter.Select(filt => filt.Text.Replace(" ", "")).ToList();
                dataexists = true;
                foreach (string data in InputData)
                {
                    if (!accessfilters.Contains(data))
                    {
                        dataexists = false;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 8: Login in icA as test user navigate to studies tab and perform * search
                login.Logout();
                login.LoginIConnect(user, user);
                PageLoadWait.WaitForPageLoad(20);
                login.Navigate("Studies");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                SearchStudy(LastName: "*", Study_Performed_Period: "All Dates");
                Dictionary<int, string[]> SearchResults = GetSearchResults();
                if (SearchResults.Count == 0)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test6_29302(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            BasePage BasePage = new BasePage();
            result = new TestCaseResult(stepcount);
            IList<IWebElement> SelectedFilter = null;
            bool dataexists = false;
            string selectedvalue = string.Empty;
            string firstname = string.Empty;
            int ExecutedSteps = -1;
            IList<string> InputData = new List<string>();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: login as Administrator. Navigate to role management page. edit test users role. remove all filters(if any)
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                login.LoginIConnect(username, password);
                if (login.IsTabSelected("DomainManagement"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                string RoleName = createDomain[DomainManagement.DomainAttr.RoleName];
                string user = GetUniqueUserId();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user, DomainName, RoleName);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    filter.Click();
                    rolemanagement.RemoveButton().Click();
                }
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                if (SelectedFilter.Count == 0)
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
                //Step 2 : from access filter drop down verify if Issuer of Admission ID" filter exist
                string[] AccessFilters = rolemanagement.AccessFiltersInformation().Options.Select(cds => cds.Text).ToArray();
                if (AccessFilters.Contains("Issuer of Admission ID"))
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

                //Step 3: Select "Issuer of Admission ID" filter and add some valid filter (for which study does not exist in data source)
                string IssuerofAdmissionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IssuerofAdmissionID");
                rolemanagement.AccessFiltersInformation().SelectByValue("Issuer of Admission ID");
                rolemanagement.RoleAccessFiltersTextBox().SendKeys(IssuerofAdmissionID);
                if (string.Equals(rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value"), IssuerofAdmissionID))
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
                //Step 4: Click on "Add" button to add the filter
                InputData.Add(string.Concat("IssuerofAdmissionID=", IssuerofAdmissionID));
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Issuer of Admission ID = ", IssuerofAdmissionID)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 5 : Select Access Filter drop down, and select Modality
                rolemanagement.AccessFiltersInformation().SelectByValue("Modality");
                string[] modalities = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':')[0].Split(',');
                SelectedFilter = rolemanagement.ModalityFilter().Options;
                int count = 0;
                foreach (string Modality in modalities)
                {
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, Modality))
                        {
                            count++;
                            break;
                        }
                    }
                }
                if (count == modalities.Length)
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
                //Step 6: Select from list of modalities, and select Add.
                string modality = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':')[1];
                InputData.Add(string.Concat("Modality=", modality));
                rolemanagement.ModalityFilter().DeselectAll();
                rolemanagement.ModalityFilter().SelectByValue(modality);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, string.Concat("Modality = ", modality)))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 7: Select Access Filter drop down, and select Referring Physician.
                rolemanagement.AccessFiltersInformation().SelectByValue("Referring Physician");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 8: Type first two characters of Last Name, select a value from the results, and select Add
                string ReferringPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), ReferringPhysician);
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname).Replace(" ", "");
                    InputData.Add(string.Concat("ReferringPhysician=", selectedvalue));
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("ReferringPhysician=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Select Access Filter drop down, and select Study Entered Date.
                rolemanagement.AccessFiltersInformation().SelectByValue("Study Entered Date");
                if (rolemanagement.DateLastBtn().Displayed && rolemanagement.DateFromTo().Displayed && rolemanagement.DateAll().Displayed)
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
                //Step 10: Select Last, enter a numeric value in text field, select a value from the drop down, and select Add
                rolemanagement.DateLastBtn().Click();
                string StudyEnteredDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyEnteredDate");
                string LastDateResult = string.Concat("Study Entered Date = Last: ", StudyEnteredDate.Split(',')[0], " ", StudyEnteredDate.Split(',')[1]);
                InputData.Add(LastDateResult.Replace(" ", ""));
                rolemanagement.DateLastText().SendKeys(StudyEnteredDate.Split(',')[0]);
                rolemanagement.DateDropDown().SelectByValue(StudyEnteredDate.Split(',')[1]);
                rolemanagement.AddAccessFilters().Click();
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 11:Select Access Filter drop down, and select Body Part.
                rolemanagement.AccessFiltersInformation().SelectByValue("Body Part");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 12:Type first two characters, select a value from the results, and select Add
                string BodyPart = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BodyPart");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), BodyPart);
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    InputData.Add(string.Concat("BodyPart=", selectedvalue.Replace(" ", "")));
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Body Part = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 13:Select Access Filter drop down, and select Study Date.
                rolemanagement.AccessFiltersInformation().SelectByValue("Study Date");
                if (rolemanagement.DateLastBtn().Displayed && rolemanagement.DateFromTo().Displayed && rolemanagement.DateAll().Displayed)
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
                //Step 14:Select From/To, manually enter values in form mm/dd/yyyy, and select Add
                string StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                string FromDate = String.Format("{0:dd-MMM-yyyy}", DateTime.ParseExact(StudyDate.Split(':')[0], "dd-MMM-yyyy", CultureInfo.InvariantCulture));
                string fromdate = (DateTime.ParseExact(FromDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture)).ToString("M'/'d'/'yyyy");
                rolemanagement.RoleDateFrom().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerFrom()));
                BasePage.EnterDate_CustomSearch(FromDate, rolemanagement: true);
                string ToDate = String.Format("{0:dd-MMM-yyyy}", DateTime.ParseExact(StudyDate.Split(':')[1], "dd-MMM-yyyy", CultureInfo.InvariantCulture));
                string todate = (DateTime.ParseExact(ToDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture)).ToString("M'/'d'/'yyyy");
                rolemanagement.RoleDateTo().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(rolemanagement.RoleDatePickerTo()));
                BasePage.EnterDate_CustomSearch(ToDate, "To", rolemanagement: true);
                rolemanagement.AddAccessFilters().Click();
                LastDateResult = string.Concat("Study Date = From: ", FromDate, " To: ", ToDate);
                InputData.Add(string.Concat("Study Date = From: ", fromdate, " To: ", todate).Replace(" ", ""));
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                dataexists = false;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (string.Equals(filter.Text, LastDateResult))
                    {
                        dataexists = true;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 15:Select Access Filter drop down, and select Patient Name.
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient Name");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 16:Type first two characters of Last Name, select a value from the results, and select Add
                string patientname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), patientname);
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname);
                    InputData.Add(string.Concat("PatientName=", selectedvalue));
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("PatientName=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17:Select Access Filter drop down, and select Accession Number.
                rolemanagement.AccessFiltersInformation().SelectByValue("Accession Number");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 18:Type first two characters, select a value from the results, In the filter DataSources select one of the datasources and select Add
                string AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                string DataSource = rolemanagement.List_ConnectDatasource().FirstOrDefault().Text;
                rolemanagement.ConnectDataSource(DataSource);
                string ConnectDataSource = string.Join(",", rolemanagement.List_ConnectedDatasource().Select(cds => cds.Text));
                string filterDataSource = string.Join(",", rolemanagement.List_FilterDatasource().Select(fds => fds.Text));
                if (ConnectDataSource.Contains(DataSource) && filterDataSource.Contains(DataSource))
                {
                    BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), AccessionNumber);
                    if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                    {
                        rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                        selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                        InputData.Add(string.Concat("AccessionNumber=", selectedvalue));
                        rolemanagement.AddAccessFilters().Click();
                        SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                        dataexists = false;
                        foreach (IWebElement filter in SelectedFilter)
                        {
                            if (string.Equals(filter.Text, string.Concat("Accession Number = ", selectedvalue)))
                            {
                                dataexists = true;
                                break;
                            }
                        }
                        if (dataexists)
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
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                SetCheckbox(rolemanagement.UseAllDataSource());
                //Step 19:Select Access Filter drop down, and select Patient ID.
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient ID");
                if (rolemanagement.RoleAccessFiltersTextBox().Displayed)
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
                //Step 20:Type first two characters, select a value from the results, and select Add
                string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                BasePage.SendKeysInStroke(rolemanagement.RoleAccessFiltersTextBox(), PatientID);
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterTxtBoxDropDown().SelectByIndex(0);
                    selectedvalue = rolemanagement.RoleAccessFiltersTextBox().GetAttribute("value");
                    InputData.Add(string.Concat("PatientID=", selectedvalue));
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text, string.Concat("Patient ID = ", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 21:Select Access Filter drop down, and select Reading Physician.
                rolemanagement.AccessFiltersInformation().SelectByValue("Reading Physician");
                if (rolemanagement.RoleAccessFiltersLastName().Displayed && rolemanagement.RoleAccessFiltersFirstName().Displayed && rolemanagement.RoleAccessFiltersMiddleName().Displayed && rolemanagement.AccessFiltersPrefix().Displayed && rolemanagement.RoleAccessFiltersSuffix().Displayed)
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
                //Step 22:Type first two characters of Last Name, select a value from the results, and select Add
                string ReadingPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReadingPhysician");
                rolemanagement.SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), ReadingPhysician);
                if (PageLoadWait.WaitForWebElement(rolemanagement.RoleAccessFilter_AutoCompleteNameDiv(), "visible"))
                {
                    rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                    firstname = string.IsNullOrWhiteSpace(rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value")) ? string.Empty : rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                    selectedvalue = string.Concat(rolemanagement.RoleAccessFiltersLastName().GetAttribute("value"), ",", firstname);
                    InputData.Add(string.Concat("ReadingPhysician=", selectedvalue.Replace(" ", "")));
                    rolemanagement.AddAccessFilters().Click();
                    SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                    dataexists = false;
                    foreach (IWebElement filter in SelectedFilter)
                    {
                        if (string.Equals(filter.Text.Replace(" ", ""), string.Concat("ReadingPhysician=", selectedvalue)))
                        {
                            dataexists = true;
                            break;
                        }
                    }
                    if (dataexists)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 23:Click on 'Save' button to save the filters
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                dataexists = false;
                IList<string> accessfilters = SelectedFilter.Select(filt => filt.Text.Replace(" ", "")).ToList();
                dataexists = true;
                foreach (string data in InputData)
                {
                    if (!accessfilters.Contains(data))
                    {
                        dataexists = false;
                        break;
                    }
                }
                if (dataexists)
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
                //Step 24:Login in icA as test user. navigate to studies tab and perform * search
                login.Logout();
                login.LoginIConnect(user, user);
                PageLoadWait.WaitForPageLoad(20);
                login.Navigate("Studies");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                SearchStudy(LastName: "*", Study_Performed_Period: "All Dates");
                Dictionary<int, string[]> SearchResults = GetSearchResults();
                if (SearchResults.Count == 0)
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
                //result.steps[++ExecutedSteps].status = "Not Automated";
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Create user with long user ID
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_70224(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables      
            UserManagement usermanagement;
            TestCaseResult result;

            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PateintID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRole = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                string userId = "testuserforlonguserID32chare" + random.Next(1, limit);
                string phone = "555";
                string domain = "SuperAdminGroup";


                //Step-1: From the WebAccess login screen click on the Register link.
                //Step-2: Fill out the form with the following information then click on submit- User ID- testuserforlonguserID32charenrol-
                //-Last name- M--First Name- A--Phone Number- 555--Email Address- valid email address.--DOmain--Group+
                login.DriverGoTo(login.url);
                bool enrolldiv = login.FillEnrollForm(userId, domain, "", Lastname, firstname, Email, "", phone);
                if (enrolldiv)
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

                //Step-3: Login in iCA with the Administrator account.
                login.LoginIConnect(adminUserName, adminPassword);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.LogoutBtn()));
                if (login.LogoutBtn().Displayed)
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


                //Step-4: Click on the Users Tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.SearchBtn().Displayed && usermanagement.SearchUsrBtn().Displayed)
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


                //Step-5: Click on the Requests Tab
                //Step-6: Select pending request do not provide password and complete the enrollment.
                //Approve the Request from Request subtab
                bool userexist = usermanagement.AcceptRequest(userId, "", 1);
                if (userexist == true)
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
        /// Role Management
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29293(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            TestCaseResult result;

            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PateintID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRole = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                string domain = "SuperAdminGroup";
                string RoleName = "SuperRole";
                string description = "description";
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String roleB1 = "roleB1_29293_" + random.Next(1, limit);
                String roleB2 = "roleB2_29293_" + random.Next(1, limit);
                String roleB3 = "roleB3_29293_" + random.Next(1, limit);
                String roleB4 = "roleB4_29293_" + random.Next(1, limit);

                //Step-1: Create a test domain.
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainAdminID = domainattr[DomainManagement.DomainAttr.UserID];
                String passwordB = domainattr[DomainManagement.DomainAttr.Password];
                String domainAdminName = domainattr[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool bDomainB = domainmanagement.IsDomainExist(DomainB);
                if (bDomainB)
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

                //Step-2: Click on the Role Management tab.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (rolemanagement.EditRoleBtn().Displayed)
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

                //Step-3: From the Show Roles From Domain drop down, select the SuperAdminGroup Domain.
                rolemanagement.SelectDomainfromDropDown(domain);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                if (rolemanagement.RoleExists(RoleName))
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


                //Step-4: From the Show Roles From Domain drop down, select the Test Domain that was created as a Pre-condition.
                rolemanagement.SelectDomainfromDropDown(DomainB);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (rolemanagement.RoleExists(domainAdminName))
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


                //Step-5: Click on the New Role button.
                rolemanagement.ClickButtonInRole("new");
                if (rolemanagement.RoleManagemantTitle().Text.Equals("Role Management") && rolemanagement.SubHeading().Text.Equals("New Role")
                    && rolemanagement.DomainInformation().Text.Equals("Domain Information") && rolemanagement.RoleInformationNewRole().Text.Equals("Role Information")
                    && rolemanagement.DataSources().Text.Equals("Data Sources") && rolemanagement.AccessFilter().Text.Equals("Access Filters Information")
                    && rolemanagement.StudySearchFieldsDiv().Displayed && rolemanagement.iPadStudyListFieldsDiv().Displayed && rolemanagement.PatientHistoryLayout().Text.Equals("PatientHistory Layout")
                    && rolemanagement.ToolBarConfig().Text.Equals("Toolbar Configuration") && rolemanagement.StudyListLayout().Text.Equals("StudyList Layout"))
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
                rolemanagement.ClickCloseButton();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);


                //Step-6 & 7: Fill in all of the fields, selecting the Test Domain for the Domain and click save.
                rolemanagement.CreateRole(DomainB, roleB1, "Conference=Physician");
                bool step6 = rolemanagement.RoleExists(roleB1, DomainB);
                if (step6)
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


                //Step-8: Select the newly created Role.
                //DomainB = "SuperAdminGroup";
                //roleB1 = "SuperRole";
                rolemanagement.SelectRole(roleB1);
                IList<IWebElement> d = Driver.FindElements(By.CssSelector("div.row tr[style*='font-weight: bold;']"));
                IWebElement td = Driver.FindElement(By.CssSelector("div.row tr td>span[title='" + roleB1 + "']"));
                if (d.Count != 0 && td.Displayed)
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


                //Step-9: Click on the Edit Role button.
                rolemanagement.ClickEditRole();
                IList<IWebElement> domainname = Driver.FindElements(By.CssSelector("select[id$='DomainDropDown_NameDropDownList']>option"));
                var roleName = BasePage.Driver.FindElement(By.CssSelector("[id$='_EditRolePreferenceConfig_RoleAccessFilter_Name']")).GetAttribute("disabled");
                if (rolemanagement.RoleManagemantTitle().Text.Equals("Role Management") && rolemanagement.SubHeading().Text.Equals("Edit Role")
                      && rolemanagement.DomainInformation().Text.Equals("Domain Information") && rolemanagement.RoleInformationEditRole().Text.Equals("Role Information")
                      && rolemanagement.DataSources().Text.Equals("Data Sources") && rolemanagement.AccessFilter().Text.Equals("Access Filters Information")
                      && rolemanagement.StudySearchFieldsDiv().Displayed && rolemanagement.iPadStudyListFieldsDiv().Displayed && rolemanagement.PatientHistoryLayout().Text.Equals("PatientHistory Layout")
                      && rolemanagement.ToolBarConfig().Text.Equals("Toolbar Configuration") && rolemanagement.StudyListLayout().Text.Equals("StudyList Layout")
                      && domainname.Count == 1 && roleName.Equals("true"))
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


                //Step-10: Modify the Role Description along with other settings and click the Save button.
                rolemanagement.RoleDescriptionTxt().Click();
                SendKeys(rolemanagement.RoleDescriptionTxt(), description);
                rolemanagement.SetCheckboxInEditRole("receiveexam", 0);
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (rolemanagement.NewRoleBtn().Displayed)
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


                //Step-11: From the Show Roles From Domain drop down, select the Test Domain.
                rolemanagement.SelectDomainfromDropDown(DomainB);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.SelectRole(roleB1);
                IWebElement desc = Driver.FindElement(By.CssSelector("div.row tr td>span[title='" + description + "']"));
                if (desc != null)
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

                //Step-12: Select the role and click the Edit Role button.
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                var descText = rolemanagement.RoleDescriptionTxt().GetAttribute("value");
                if (rolemanagement.SubHeading().Text.Equals("Edit Role") && descText.Equals(description))
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

                //Step-13: Click the Close button.
                rolemanagement.CloseRoleManagement();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (rolemanagement.NewRoleBtn().Displayed)
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

                //Step-14: Create a couple other Roles for the Test Domain.
                //Step-15: On the Role Management page, select the Test Domain from the Show Roles From Domain drop down.
                rolemanagement.CreateRole(DomainB, roleB2, "Conference=Physician");
                rolemanagement.CreateRole(DomainB, roleB3, "Conference=Physician");
                ExecutedSteps++;
                bool step14_1 = rolemanagement.RoleExists(roleB2, DomainB);
                bool step14_2 = rolemanagement.RoleExists(roleB3, DomainB);
                if (step14_1 && step14_2)
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

                //Step-16: In the Role Name search field, enter first few chars of a Role name that was created and click the Search button.
                rolemanagement.SearchRole("roleB", DomainB);
                bool flag = false;
                foreach (IWebElement role in rolemanagement.RoleDetails())
                {
                    if (role.Text.Equals(roleB1))
                    {
                        flag = true;
                    }
                    else if (role.Text.Equals(roleB1))
                    {
                        flag = true;
                    }
                    else if (role.Text.Equals(roleB1))
                    {
                        flag = true;
                    }

                }

                if (!(rolemanagement.ShowAllRoles().Selected) && flag == true)
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

                //Step-17: Check the Show All Roles check box.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.ShowAllRoles().Click();
                var str = rolemanagement.RoleNameTb().GetAttribute("value");
                if (str.Equals("") && rolemanagement.RoleList().Count == 5)
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


                //Step-18: Click on each of the titles of the fields (Role Name, Role Description, and Domain)
                int count17 = 0;
                int i = 0;
                foreach (IWebElement heading in rolemanagement.RoleDetailsortBy())
                {
                    rolemanagement.ColumnHeadings()[i].Click();
                    if (!heading.GetAttribute("style").Contains("display: none"))
                    {
                        count17++;
                    }
                    i++;
                }
                if (count17 == 3)
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

                //Step-19: Select one of the Roles not associated with any users and click the Delete Role button.
                //Step-20:Click Ok in the Confirmation dialog.
                rolemanagement.SelectRole(roleB2);
                rolemanagement.ClickButtonInRole("delete");
                ExecutedSteps++;
                if (!(rolemanagement.RoleExists(roleB2)))
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

                //Step-21: Click on the New Role button.
                rolemanagement.ClickButtonInRole("new");
                if (rolemanagement.RoleManagemantTitle().Text.Equals("Role Management") && rolemanagement.SubHeading().Text.Equals("New Role")
                    && rolemanagement.DomainInformation().Text.Equals("Domain Information") && rolemanagement.RoleInformationNewRole().Text.Equals("Role Information")
                    && rolemanagement.DataSources().Text.Equals("Data Sources") && rolemanagement.AccessFilter().Text.Equals("Access Filters Information")
                    && rolemanagement.StudySearchFieldsDiv().Displayed && rolemanagement.iPadStudyListFieldsDiv().Displayed && rolemanagement.PatientHistoryLayout().Text.Equals("PatientHistory Layout")
                    && rolemanagement.ToolBarConfig().Text.Equals("Toolbar Configuration") && rolemanagement.StudyListLayout().Text.Equals("StudyList Layout"))
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
                rolemanagement.ClickCloseButton();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);


                //Step-22 & 23: Fill in all of the fields, selecting the Test Domain for the Domain and click save.
                rolemanagement.CreateRole(DomainB, roleB4, "Conference=Archivist");
                bool step23 = rolemanagement.RoleExists(roleB4, DomainB);
                if (step23)
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

        public TestCaseResult Test_29296(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            Patients patients = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            String DatasourceAutoSSA = login.GetHostName(Config.EA1);
            String DatasourceVMSSA131 = login.GetHostName(Config.EA77);
            String DatasourceVMSSA91 = login.GetHostName(Config.EA91);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);

                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");

                //Step 1
                //Login as system administrator in ICA.
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, RoleName, DS: new string[] { "attachmentupload" }, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91 }, check: 1);
                // Step 2
                //In the name field enter DomainTestA. Fill in the remaining required fields and uncheck the"Allow Upload"checkbox. Save the changes then logout of WebAccess.
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.EditDomainButton().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (domainmanagement.PageHeaderLabel().Displayed)
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

                // Step 3 
                // Uncheck the"Enable Print"checkbox.Save changes and logout.
                domainmanagement.SetCheckBoxInEditDomain("print", 1);
                domainmanagement.ClickSaveEditDomain();
                login.Logout();
                ExecutedSteps++;


                // Step 4
                // Login as DomainTestA administrator and load a dataset from the studylist.
                login.LoginIConnect(DomainName, DomainName);
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                try
                {
                    if (viewer.GetReviewTool("Print View").Displayed == false)
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
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }


                //Step-5 
                // Close the viewer and select the Patients Tab.
                studies.CloseStudy();
                patients = (Patients)login.Navigate("Patients");
                Boolean istabpresent = login.IsTabPresent("Patients");
                if (istabpresent)
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

                //// Step 6
                //// Search for a patient in the data source then load the patient into the PMJ. On the Radiology tab of the PMJ, select a study a load it.
                //// Test step needs update
                //result.steps[++ExecutedSteps].status = "Not Automated";

                //// Step 7
                //result.steps[++ExecutedSteps].status = "Not Automated";
                ////Step 8
                ////Return to the XDS tab in the PMJ and load a text document.
                //result.steps[++ExecutedSteps].status = "Not Automated";

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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// DICOM DataSource Configurations
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29305(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            WpfObjects wpfobject = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);

                String DatasourceAutoSSA = login.GetHostName(Config.EA77);
                String DatasourceVMSSA131 = login.GetHostName(Config.EA1);
                String DatasourceVMSSA91 = login.GetHostName(Config.EA91);

                String LastNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String RefPhysicainData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                String PatientIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ModalityData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String IPIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String StudyDescriptionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
                String DOBData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOBData");
                String InstitutionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                String AccessionNumberData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                String StudyPerformedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedData");

                // Step 1               
                ExecutedSteps++;

                //Step 2
                //Navigate to Iconnect 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                studies = (Studies)login.Navigate("Studies");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, RoleName, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91 });
                domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(UName, DomainName, RoleName);
                PageLoadWait.WaitForPageLoad(10);
                login.Logout();

                // Step 3
                // Log in using a user account existing in the previously modified Domain.
                login.LoginIConnect(UName, UName);
                ExecutedSteps++;

                // Step 4
                // Observe the number of tabs displayed.
                if (login.IsTabPresent("studies") && login.IsTabPresent("patients"))
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


                // Step 5
                // Observe the search results and available search options.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                string[] columnlist = BasePage.GetColumnNames();
                string[] columntobeverified = { "Study Date", "Modality", "Patient Name", "Patient ID", "Description", "Accession", "Refer. Physician", "# Images" };

                bool compareStudy = columnlist.OrderBy(s => s).SequenceEqual(columntobeverified.OrderBy(t => t));
                bool searchfields = LastName().Displayed && FirstName().Displayed && PatientID().Displayed && Accession().Displayed &&
                           Modality().Displayed && RefPhysician().Displayed && StudyPerformed().Displayed && StudyID().Displayed && Instituition().Displayed &&
                           Gender().Displayed && PatientDOB().Displayed && IPID().Displayed && StudyDescription().Displayed && DataSource().Displayed &&
                           MyPatients().Displayed;

                if (compareStudy & searchfields)
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
                login.LoginIConnect(Username, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                studies.ClickChooseColumns(section: "Others1");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));

                studies.SelectColumns(new string[] { "Last Name", "First Name", "Patient DOB", "Gender", "Study ID", "Data Source", "Institutions", "Issuer of PID", "Study UID" }, "Add", isRerarrange: false);
                studies.OKButton_ChooseColumns().Click();
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);
                login.LoginIConnect(UName, UName);
                PageLoadWait.WaitForPageLoad(10);

                // Step 6 
                // Enter a value into Patient last Name name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastNameData, Study_Performed_Period: "All Dates");
                //Dictionary<string, string> LastNameResult = studies.GetMatchingRow(new string[] { "Last Name" }, new string[] { LastNameData });
                //if (LastNameResult != null)
                if (VerifyStudiesSearch(new string[] { "Last Name" }, new string[] { LastNameData }))
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

                // Step 7 
                // Enter a value into Patient first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> FirstNameResult = studies.GetMatchingRow(new string[] { "First Name" }, new string[] { FirstNameData });
                //if (FirstNameResult != null)
                if (VerifyStudiesSearch(new string[] { "First Name" }, new string[] { FirstNameData }))
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

                // Step 8 
                // Enter a value into Patient last and first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(LastName: LastNameData, FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> FirstNameandLastNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { LastNameData, FirstNameData });
                //if (FirstNameandLastNameResult != null)
                if (VerifyStudiesSearch(new string[] { "Last Name", "First Name" }, new string[] { LastNameData, FirstNameData }))
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


                // Step 9 
                // Enter a value into Referring Physician name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(physicianName: RefPhysicainData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> PhysicanResult = studies.GetMatchingRow(new string[] { "Refer. Physician" }, new string[] { RefPhysicainData });
                //if (PhysicanResult != null)
                if (VerifyStudiesSearch(new string[] { "Refer. Physician" }, new string[] { RefPhysicainData }))
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

                // Step 10
                // Enter a value into Patient ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(patientID: PatientIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> PatientIDResult = studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { PatientIDData });
                //if (PatientIDResult != null)
                if (VerifyStudiesSearch(new string[] { "Patient ID" }, new string[] { PatientIDData }))
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

                //Step 11
                // Enter a value into Modality field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Modality: ModalityData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> ModalityResult = studies.GetMatchingRow(new string[] { "Modality" }, new string[] { ModalityData });
                //string[] ListedStudies = BasePage.GetColumnValues("Modality");
                //if (ListedStudies.All(study => study.Contains(ModalityData)))
                if (VerifyStudiesSearch(new string[] { "Modality" }, new string[] { ModalityData }))
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

                // Step 12
                // Enter a value into Study ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(studyID: StudyIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> StudyIDResult = studies.GetMatchingRow(new string[] { "Study ID" }, new string[] { StudyIDData });
                //if (StudyIDResult != null)
                if (VerifyStudiesSearch(new string[] { "Study ID" }, new string[] { StudyIDData }))
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

                // Step 13
                // Enter a value into IPID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(IPID: IPIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> IPIDResult = studies.GetMatchingRow(new string[] { "Issuer of PID" }, new string[] { IPIDData });
                //if (IPIDResult != null)
                if (VerifyStudiesSearch(new string[] { "Issuer of PID" }, new string[] { IPIDData }))
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

                // Step 14
                // Enter a value into Study Description field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Description: StudyDescriptionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> StudyDescriptionResult = studies.GetMatchingRow(new string[] { "Description" }, new string[] { StudyDescriptionData });
                //if (StudyDescriptionResult != null)
                if (VerifyStudiesSearch(new string[] { "Description" }, new string[] { StudyDescriptionData }))
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

                // Step 15
                // Enter a value into DOB field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                DateTime _date;
                string FormatReplacedDOBDate = "";
                _date = DateTime.Parse(DOBData);
                FormatReplacedDOBDate = _date.ToString("dd-MMM-yyyy");
                studies.SearchStudy(DOB: FormatReplacedDOBDate, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> DOBResult = studies.GetMatchingRow(new string[] { "Patient DOB" }, new string[] { FormatReplacedDOBDate });
                //if (DOBResult != null)
                if (VerifyStudiesSearch(new string[] { "Patient DOB" }, new string[] { FormatReplacedDOBDate }))
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

                // Step 16 
                // Enter a value into Institution field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Institution: InstitutionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(30);
                //Dictionary<string, string> InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });
                //if (InstitutionResult != null)
                if (VerifyStudiesSearch(new string[] { "Institutions" }, new string[] { InstitutionData }))
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

                // Step 17 
                // Enter a value into Accession Number field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(AccessionNo: AccessionNumberData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                //Dictionary<string, string> AccessionNumberResult = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumberData });
                //if (AccessionNumberResult != null)
                if (VerifyStudiesSearch(new string[] { "Accession" }, new string[] { AccessionNumberData }))
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

                // Step 18
                // Select a drop down value from 'Study Performed' field, clear all other fields, and select Search
                studies.SearchStudy(Study_Performed_Period: "Last Month", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                string[] columnvalues = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Study Date", BasePage.GetColumnNames());
                int columnValuescount = columnvalues.Length;
                bool temp = true;
                foreach (string column in columnvalues)
                {
                    if (!studies.VerifyStudyPerformed(column, "Last Month"))
                    {
                        temp = false;
                        break;
                    }
                }

                if (temp)
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


                // Step 19
                // Clear all fields, select All Dates, and a Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourceVMSSA131);
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> DatasourceResult = studies.GetMatchingRow(new string[] { "Data Source" }, new string[] { DatasourceVMSSA131 });
                if (DatasourceResult != null)
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

                //  Step-20 - Precondition to added access filter for Patient name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient Name");
                PageLoadWait.WaitForPageLoad(20);

                // Filter Data Sources
                SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), LastNameData.ToLowerInvariant());
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                PageLoadWait.WaitForPageLoad(10);
                String RunTimeLastName = rolemanagement.RoleAccessFiltersLastName().GetAttribute("value");
                String RunTimeFirstName = rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                login.LoginIConnect(UName, UName);
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 21
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourceVMSSA131);
                PageLoadWait.WaitForLoadingMessage(10);
                //FirstNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { RunTimeLastName, RunTimeFirstName });
                //if (FirstNameResult != null)
                if (VerifyStudiesSearch(new string[] { "Last Name", "First Name" }, new string[] { RunTimeLastName, RunTimeFirstName }))
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


                // Step-22 - Precondition to added access filter for Insitution Name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> SelectedFilter;
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient Name"))
                    {
                        PageLoadWait.WaitForPageLoad(20);
                        filter.Click();
                        PageLoadWait.WaitForPageLoad(20);
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Institution");
                PageLoadWait.WaitForElementToDisplay(rolemanagement.RoleAccessFiltersTextBox());
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), InstitutionData);
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                login.LoginIConnect(UName, UName);
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 23
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourceVMSSA131);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForLoadingMessage(10);
                //InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });
                //var intlist = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Institutions", BasePage.GetColumnNames()).ToList<String>();
                //var step23 = intlist.TrueForAll(institution => institution.Equals(InstitutionData));

                //if (InstitutionResult != null)
                //if (step23)
                if (VerifyStudiesSearch(new string[] { "Institutions" }, new string[] { InstitutionData }))
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
                login.Logout();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_29306(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            WpfObjects wpfobject = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);
                ServiceTool tool = null;
                // string[] Datasource = { "AUTO-SSA-001", "VMSSA-4-38-131", "VMSSA-5-38-91" };

                String DatasourceSanityPACS = login.GetHostName(Config.SanityPACS);
                String DatasourcePACS2 = login.GetHostName(Config.PACS2);

                String LastNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String RefPhysicainData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                String PatientIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ModalityData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String IPIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String StudyDescriptionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
                String DOBData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOBData");
                String InstitutionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                String AccessionNumberData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                String StudyPerformedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedData");

                // Step 1
                // 1.Configure / Start a Dicom data source (on another system or Virtual Machine)
                // 2.Modify the application Configuration files to add the Dicom Data Source
                // 3.Login as Administrator, and add the data source to a domain.Select / Create a user 
                // account in this domain, and configure the role filter so that studies from this data source will be retrieved.


                // // Invoke service tools
                // tool.InvokeServiceTool();

                //// Enable service tools
                //tool.SetEnableFeaturesGeneral();
                //wpfobject.WaitTillLoad();
                //tool.ModifyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //tool.EnablePatient();
                //wpfobject.WaitTillLoad();
                //tool.ApplyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //wpfobject.ClickOkPopUp();
                //wpfobject.WaitTillLoad();

                ExecutedSteps++;

                //Step 2
                //Navigate to Iconnect 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                studies = (Studies)login.Navigate("Studies");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, RoleName, datasources: new string[] { DatasourcePACS2, DatasourceSanityPACS });
                domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(UName, DomainName, RoleName);
                PageLoadWait.WaitForPageLoad(10);
                login.Logout();

                // Step 3
                // Log in using a user account existing in the previously modified Domain.
                login.LoginIConnect(UName, UName);
                ExecutedSteps++;

                // Step 4
                // Observe the number of tabs displayed.
                if (login.IsTabPresent("studies") && login.IsTabPresent("patients"))
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


                // Step 5
                // Observe the search results and available search options.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                string[] columnlist = BasePage.GetColumnNames();
                string[] columntobeverified = { "Study Date", "Modality", "Patient Name", "Patient ID", "Description", "Accession", "Refer. Physician", "# Images" };

                bool compareStudy = columnlist.OrderBy(s => s).SequenceEqual(columntobeverified.OrderBy(t => t));
                bool searchfields = LastName().Displayed && FirstName().Displayed && PatientID().Displayed && Accession().Displayed &&
                           Modality().Displayed && RefPhysician().Displayed && StudyPerformed().Displayed && StudyID().Displayed && Instituition().Displayed &&
                           Gender().Displayed && PatientDOB().Displayed && IPID().Displayed && StudyDescription().Displayed && DataSource().Displayed &&
                           MyPatients().Displayed;

                if (compareStudy & searchfields)
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
                login.LoginIConnect(Username, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                studies.ClickChooseColumns(section: "Others1");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));

                studies.SelectColumns(new string[] { "Last Name", "First Name", "Patient DOB", "Gender", "Study ID", "Data Source", "Institutions", "Issuer of PID", "Study UID" }, "Add", isRerarrange: false);
                studies.OKButton_ChooseColumns().Click();
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);
                login.LoginIConnect(UName, UName);
                PageLoadWait.WaitForPageLoad(10);

                // Step 6 
                // Enter a value into Patient last Name name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastNameData, Study_Performed_Period: "All Dates");
                Dictionary<string, string> LastNameResult = studies.GetMatchingRow(new string[] { "Last Name" }, new string[] { LastNameData });

                if (LastNameResult != null)
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

                // Step 7 
                // Enter a value into Patient first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> FirstNameResult = studies.GetMatchingRow(new string[] { "First Name" }, new string[] { FirstNameData });
                if (FirstNameResult != null)
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

                // Step 8 
                // Enter a value into Patient last and first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search

                studies.SearchStudy(LastName: LastNameData, FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> FirstNameandLastNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { LastNameData, FirstNameData });
                if (FirstNameandLastNameResult != null)
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


                // Step 9 
                // Enter a value into Referring Physician name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                String[] RefPhysicianSplit = RefPhysicainData.Split(',');
                studies.SearchStudy(physicianName: RefPhysicianSplit[0], Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> PhysicanResult = studies.GetMatchingRow(new string[] { "Refer. Physician" }, new string[] { RefPhysicainData });
                if (PhysicanResult != null)
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

                // Step 10
                // Enter a value into Patient ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(patientID: PatientIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> PatientIDResult = studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { PatientIDData });
                if (PatientIDResult != null)
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

                //Step 11
                // Enter a value into Modality field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Modality: ModalityData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> ModalityResult = studies.GetMatchingRow(new string[] { "Modality" }, new string[] { ModalityData });
                if (ModalityResult != null)
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

                // Step 12
                // Enter a value into Study ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(studyID: StudyIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> StudyIDResult = studies.GetMatchingRow(new string[] { "Study ID" }, new string[] { StudyIDData });
                if (StudyIDResult != null)
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

                // Step 13
                // Enter a value into IPID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(IPID: IPIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> IPIDResult = studies.GetMatchingRow(new string[] { "Issuer of PID" }, new string[] { IPIDData });

                if (IPIDResult != null)
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

                // Step 14
                // Enter a value into Study Description field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Description: StudyDescriptionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> StudyDescriptionResult = studies.GetMatchingRow(new string[] { "Description" }, new string[] { StudyDescriptionData });
                if (StudyDescriptionResult != null)
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

                // Step 15
                // Enter a value into DOB field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(DOB: DOBData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> DOBResult = studies.GetMatchingRow(new string[] { "Patient DOB" }, new string[] { DOBData });
                if (DOBResult != null)
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

                // Step 16 
                // Enter a value into Institution field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Institution: InstitutionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });

                if (InstitutionResult != null)
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

                // Step 17 
                // Enter a value into Accession Number field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(AccessionNo: AccessionNumberData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> AccessionNumberResult = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumberData });

                if (AccessionNumberResult != null)
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

                // Step 18
                // Select a drop down value from 'Study Performed' field, clear all other fields, and select Search
                studies.SearchStudy(Study_Performed_Period: "Last 2 Years", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                string[] columnvalues = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Study Date", BasePage.GetColumnNames());
                int columnValuescount = columnvalues.Length;
                bool temp = true;
                foreach (string column in columnvalues)
                {
                    if (!studies.VerifyStudyPerformed(column, "Last 2 Years"))
                    {
                        temp = false;
                        break;
                    }
                }

                if (temp)
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


                // Step 19
                // Clear all fields, select All Dates, and a Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> DatasourceResult = studies.GetMatchingRow(new string[] { "Data Source" }, new string[] { DatasourcePACS2 });
                if (DatasourceResult != null)
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

                //  Precondition to added access filter for Patient name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.EditRoleBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient Name");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForElementToDisplay(rolemanagement.RoleAccessFiltersLastName());
                SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), LastNameData.ToLowerInvariant());
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                PageLoadWait.WaitForPageLoad(10);
                String RunTimeLastName = rolemanagement.RoleAccessFiltersLastName().GetAttribute("value");
                String RunTimeFirstName = rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();

                login.Logout();
                login.LoginIConnect(UName, UName);

                studies = (Studies)login.Navigate("Studies");

                //Step 20
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                FirstNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { RunTimeLastName, RunTimeFirstName });
                if (FirstNameResult != null)
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


                // Precondition to added access filter for Insitution Name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.EditRoleBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                IList<IWebElement> SelectedFilter;
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient Name"))
                    {
                        PageLoadWait.WaitForPageLoad(20);
                        filter.Click();
                        PageLoadWait.WaitForPageLoad(20);
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Institution");
                PageLoadWait.WaitForElementToDisplay(rolemanagement.RoleAccessFiltersTextBox());
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), InstitutionData);
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();

                login.Logout();
                login.LoginIConnect(UName, UName);

                studies = (Studies)login.Navigate("Studies");

                //Step 21
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Institution: InstitutionData, Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });

                if (InstitutionResult != null)
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_29307(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            WpfObjects wpfobject = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);
                ServiceTool tool = null;
                // string[] Datasource = { "AUTO-SSA-001", "VMSSA-4-38-131", "VMSSA-5-38-91" };

                String DatasourceSanityPACS = login.GetHostName(Config.SanityPACS);
                String DatasourcePACS2 = login.GetHostName(Config.PACS2);

                String LastNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String RefPhysicainData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                String PatientIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ModalityData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String IPIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String StudyDescriptionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
                String DOBData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOBData");
                String InstitutionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                String AccessionNumberData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                String StudyPerformedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedData");

                // Step 1
                // 1.Configure / Start a Dicom data source (on another system or Virtual Machine)
                // 2.Modify the application Configuration files to add the Dicom Data Source
                // 3.Login as Administrator, and add the data source to a domain.Select / Create a user 
                // account in this domain, and configure the role filter so that studies from this data source will be retrieved.


                // // Invoke service tools
                // tool.InvokeServiceTool();

                //// Enable service tools
                //tool.SetEnableFeaturesGeneral();
                //wpfobject.WaitTillLoad();
                //tool.ModifyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //tool.EnablePatient();
                //wpfobject.WaitTillLoad();
                //tool.ApplyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //wpfobject.ClickOkPopUp();
                //wpfobject.WaitTillLoad();

                ExecutedSteps++;

                //Step 2
                //Navigate to Iconnect 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                studies = (Studies)login.Navigate("Studies");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, RoleName, datasources: new string[] { DatasourcePACS2, DatasourceSanityPACS });
                domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(UName, DomainName, RoleName);
                PageLoadWait.WaitForPageLoad(10);
                login.Logout();

                // Step 3
                // Log in using a user account existing in the previously modified Domain.
                login.LoginIConnect(UName, UName);
                ExecutedSteps++;

                // Step 4
                // Observe the number of tabs displayed.
                if (login.IsTabPresent("studies") && login.IsTabPresent("patients"))
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


                // Step 5
                // Observe the search results and available search options.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                string[] columnlist = BasePage.GetColumnNames();
                string[] columntobeverified = { "Study Date", "Modality", "Patient Name", "Patient ID", "Description", "Accession", "Refer. Physician", "# Images" };

                bool compareStudy = columnlist.OrderBy(s => s).SequenceEqual(columntobeverified.OrderBy(t => t));
                bool searchfields = LastName().Displayed && FirstName().Displayed && PatientID().Displayed && Accession().Displayed &&
                           Modality().Displayed && RefPhysician().Displayed && StudyPerformed().Displayed && StudyID().Displayed && Instituition().Displayed &&
                           Gender().Displayed && PatientDOB().Displayed && IPID().Displayed && StudyDescription().Displayed && DataSource().Displayed &&
                           MyPatients().Displayed;

                if (compareStudy & searchfields)
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
                login.LoginIConnect(Username, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                studies.ClickChooseColumns(section: "Others1");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));

                studies.SelectColumns(new string[] { "Last Name", "First Name", "Patient DOB", "Gender", "Study ID", "Data Source", "Institutions", "Issuer of PID", "Study UID" }, "Add", isRerarrange: false);
                studies.OKButton_ChooseColumns().Click();
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);
                login.LoginIConnect(UName, UName);
                PageLoadWait.WaitForPageLoad(10);

                // Step 6 
                // Enter a value into Patient last Name name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastNameData, Study_Performed_Period: "All Dates");
                Dictionary<string, string> LastNameResult = studies.GetMatchingRow(new string[] { "Last Name" }, new string[] { LastNameData });

                if (LastNameResult != null)
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

                // Step 7 
                // Enter a value into Patient first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> FirstNameResult = studies.GetMatchingRow(new string[] { "First Name" }, new string[] { FirstNameData });
                if (FirstNameResult != null)
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

                // Step 8 
                // Enter a value into Patient last and first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search

                studies.SearchStudy(LastName: LastNameData, FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> FirstNameandLastNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { LastNameData, FirstNameData });
                if (FirstNameandLastNameResult != null)
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


                // Step 9 
                // Enter a value into Referring Physician name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                String[] RefPhysicianSplit = RefPhysicainData.Split(',');
                studies.SearchStudy(physicianName: RefPhysicianSplit[0], Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> PhysicanResult = studies.GetMatchingRow(new string[] { "Refer. Physician" }, new string[] { RefPhysicainData });
                if (PhysicanResult != null)
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

                // Step 10
                // Enter a value into Patient ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(patientID: PatientIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> PatientIDResult = studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { PatientIDData });
                if (PatientIDResult != null)
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

                //Step 11
                // Enter a value into Modality field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Modality: ModalityData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> ModalityResult = studies.GetMatchingRow(new string[] { "Modality" }, new string[] { ModalityData });
                if (ModalityResult != null)
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

                // Step 12
                // Enter a value into Study ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(studyID: StudyIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> StudyIDResult = studies.GetMatchingRow(new string[] { "Study ID" }, new string[] { StudyIDData });
                if (StudyIDResult != null)
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

                // Step 13
                // Enter a value into IPID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(IPID: IPIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> IPIDResult = studies.GetMatchingRow(new string[] { "Issuer of PID" }, new string[] { IPIDData });

                if (IPIDResult != null)
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

                // Step 14
                // Enter a value into Study Description field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Description: StudyDescriptionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> StudyDescriptionResult = studies.GetMatchingRow(new string[] { "Description" }, new string[] { StudyDescriptionData });
                if (StudyDescriptionResult != null)
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

                // Step 15
                // Enter a value into DOB field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(DOB: DOBData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> DOBResult = studies.GetMatchingRow(new string[] { "Patient DOB" }, new string[] { DOBData });
                if (DOBResult != null)
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

                // Step 16 
                // Enter a value into Institution field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Institution: InstitutionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });

                if (InstitutionResult != null)
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

                // Step 17 
                // Enter a value into Accession Number field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(AccessionNo: AccessionNumberData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> AccessionNumberResult = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumberData });

                if (AccessionNumberResult != null)
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

                // Step 18
                // Select a drop down value from 'Study Performed' field, clear all other fields, and select Search
                studies.SearchStudy(Study_Performed_Period: "Last 2 Years", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                string[] columnvalues = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Study Date", BasePage.GetColumnNames());
                int columnValuescount = columnvalues.Length;
                bool temp = true;
                foreach (string column in columnvalues)
                {
                    if (!studies.VerifyStudyPerformed(column, "Last 2 Years"))
                    {
                        temp = false;
                        break;
                    }
                }

                if (temp)
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


                // Step 19
                // Clear all fields, select All Dates, and a Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> DatasourceResult = studies.GetMatchingRow(new string[] { "Data Source" }, new string[] { DatasourcePACS2 });
                if (DatasourceResult != null)
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

                //  Precondition to added access filter for Patient name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.EditRoleBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient Name");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForElementToDisplay(rolemanagement.RoleAccessFiltersLastName());
                SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), LastNameData.ToLowerInvariant());
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                PageLoadWait.WaitForPageLoad(10);
                String RunTimeLastName = rolemanagement.RoleAccessFiltersLastName().GetAttribute("value");
                String RunTimeFirstName = rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();

                login.Logout();
                login.LoginIConnect(UName, UName);

                studies = (Studies)login.Navigate("Studies");

                //Step 20
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                FirstNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { RunTimeLastName, RunTimeFirstName });
                if (FirstNameResult != null)
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


                // Precondition to added access filter for Insitution Name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.EditRoleBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                IList<IWebElement> SelectedFilter;
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient Name"))
                    {
                        PageLoadWait.WaitForPageLoad(20);
                        filter.Click();
                        PageLoadWait.WaitForPageLoad(20);
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Institution");
                PageLoadWait.WaitForElementToDisplay(rolemanagement.RoleAccessFiltersTextBox());
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), InstitutionData);
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();

                login.Logout();
                login.LoginIConnect(UName, UName);

                studies = (Studies)login.Navigate("Studies");

                //Step 21
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Institution: InstitutionData, Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });

                if (InstitutionResult != null)
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_29308(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            WpfObjects wpfobject = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);
                ServiceTool tool = null;
                // string[] Datasource = { "AUTO-SSA-001", "VMSSA-4-38-131", "VMSSA-5-38-91" };

                String DatasourceSanityPACS = login.GetHostName(Config.SanityPACS);
                String DatasourcePACS2 = login.GetHostName(Config.PACS2);

                String LastNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstNameData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String RefPhysicainData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                String PatientIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ModalityData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String IPIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String StudyDescriptionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
                String DOBData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOBData");
                String InstitutionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                String AccessionNumberData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                String StudyPerformedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedData");

                // Step 1
                // 1.Configure / Start a Dicom data source (on another system or Virtual Machine)
                // 2.Modify the application Configuration files to add the Dicom Data Source
                // 3.Login as Administrator, and add the data source to a domain.Select / Create a user 
                // account in this domain, and configure the role filter so that studies from this data source will be retrieved.


                // // Invoke service tools
                // tool.InvokeServiceTool();

                //// Enable service tools
                //tool.SetEnableFeaturesGeneral();
                //wpfobject.WaitTillLoad();
                //tool.ModifyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //tool.EnablePatient();
                //wpfobject.WaitTillLoad();
                //tool.ApplyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //wpfobject.ClickOkPopUp();
                //wpfobject.WaitTillLoad();

                ExecutedSteps++;

                //Step 2
                //Navigate to Iconnect 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                studies = (Studies)login.Navigate("Studies");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, RoleName, datasources: new string[] { DatasourcePACS2, DatasourceSanityPACS });
                domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(UName, DomainName, RoleName);
                PageLoadWait.WaitForPageLoad(10);
                login.Logout();

                // Step 3
                // Log in using a user account existing in the previously modified Domain.
                login.LoginIConnect(UName, UName);
                ExecutedSteps++;

                // Step 4
                // Observe the number of tabs displayed.
                if (login.IsTabPresent("studies") && login.IsTabPresent("patients"))
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


                // Step 5
                // Observe the search results and available search options.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                string[] columnlist = BasePage.GetColumnNames();
                string[] columntobeverified = { "Study Date", "Modality", "Patient Name", "Patient ID", "Description", "Accession", "Refer. Physician", "# Images" };

                bool compareStudy = columnlist.OrderBy(s => s).SequenceEqual(columntobeverified.OrderBy(t => t));
                bool searchfields = LastName().Displayed && FirstName().Displayed && PatientID().Displayed && Accession().Displayed &&
                           Modality().Displayed && RefPhysician().Displayed && StudyPerformed().Displayed && StudyID().Displayed && Instituition().Displayed &&
                           Gender().Displayed && PatientDOB().Displayed && IPID().Displayed && StudyDescription().Displayed && DataSource().Displayed &&
                           MyPatients().Displayed;

                if (compareStudy & searchfields)
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
                login.LoginIConnect(Username, Password);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                studies.ClickChooseColumns(section: "Others1");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));

                studies.SelectColumns(new string[] { "Last Name", "First Name", "Patient DOB", "Gender", "Study ID", "Data Source", "Institutions", "Issuer of PID", "Study UID" }, "Add", isRerarrange: false);
                studies.OKButton_ChooseColumns().Click();
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);

                login.Logout();
                PageLoadWait.WaitForPageLoad(10);
                login.LoginIConnect(UName, UName);
                PageLoadWait.WaitForPageLoad(10);

                // Step 6 
                // Enter a value into Patient last Name name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: LastNameData, Study_Performed_Period: "All Dates");
                Dictionary<string, string> LastNameResult = studies.GetMatchingRow(new string[] { "Last Name" }, new string[] { LastNameData });

                if (LastNameResult != null)
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

                // Step 7 
                // Enter a value into Patient first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> FirstNameResult = studies.GetMatchingRow(new string[] { "First Name" }, new string[] { FirstNameData });
                if (FirstNameResult != null)
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

                // Step 8 
                // Enter a value into Patient last and first name field, clear all other fields, and select All Dates for 'Study Performed' and select Search

                studies.SearchStudy(LastName: LastNameData, FirstName: FirstNameData, Study_Performed_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> FirstNameandLastNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { LastNameData, FirstNameData });
                if (FirstNameandLastNameResult != null)
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


                // Step 9 
                // Enter a value into Referring Physician name field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                String[] RefPhysicianSplit = RefPhysicainData.Split(',');
                studies.SearchStudy(physicianName: RefPhysicianSplit[0], Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> PhysicanResult = studies.GetMatchingRow(new string[] { "Refer. Physician" }, new string[] { RefPhysicainData });
                if (PhysicanResult != null)
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

                // Step 10
                // Enter a value into Patient ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(patientID: PatientIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> PatientIDResult = studies.GetMatchingRow(new string[] { "Patient ID" }, new string[] { PatientIDData });
                if (PatientIDResult != null)
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

                //Step 11
                // Enter a value into Modality field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Modality: ModalityData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> ModalityResult = studies.GetMatchingRow(new string[] { "Modality" }, new string[] { ModalityData });
                if (ModalityResult != null)
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

                // Step 12
                // Enter a value into Study ID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(studyID: StudyIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> StudyIDResult = studies.GetMatchingRow(new string[] { "Study ID" }, new string[] { StudyIDData });
                if (StudyIDResult != null)
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

                // Step 13
                // Enter a value into IPID field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(IPID: IPIDData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> IPIDResult = studies.GetMatchingRow(new string[] { "Issuer of PID" }, new string[] { IPIDData });

                if (IPIDResult != null)
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

                // Step 14
                // Enter a value into Study Description field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Description: StudyDescriptionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> StudyDescriptionResult = studies.GetMatchingRow(new string[] { "Description" }, new string[] { StudyDescriptionData });
                if (StudyDescriptionResult != null)
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

                // Step 15
                // Enter a value into DOB field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                DateTime _date;
                string FormatReplacedDOBDate = "";

                _date = DateTime.Parse(DOBData);
                FormatReplacedDOBDate = _date.ToString("dd-MMM-yyyy");
                studies.SearchStudy(DOB: FormatReplacedDOBDate, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> DOBResult = studies.GetMatchingRow(new string[] { "Patient DOB" }, new string[] { FormatReplacedDOBDate });
                if (DOBResult != null)
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

                // Step 16 
                // Enter a value into Institution field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(Institution: InstitutionData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });

                if (InstitutionResult != null)
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

                // Step 17 
                // Enter a value into Accession Number field, clear all other fields, and select All Dates for 'Study Performed' and select Search
                studies.SearchStudy(AccessionNo: AccessionNumberData, Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> AccessionNumberResult = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumberData });

                if (AccessionNumberResult != null)
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

                // Step 18
                // Select a drop down value from 'Study Performed' field, clear all other fields, and select Search
                studies.SearchStudy(Study_Performed_Period: "Last 2 Years", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                string[] columnvalues = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Study Date", BasePage.GetColumnNames());
                int columnValuescount = columnvalues.Length;
                bool temp = true;
                foreach (string column in columnvalues)
                {
                    if (!studies.VerifyStudyPerformed(column, "Last 2 Years"))
                    {
                        temp = false;
                        break;
                    }
                }

                if (temp)
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


                // Step 19
                // Clear all fields, select All Dates, and a Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> DatasourceResult = studies.GetMatchingRow(new string[] { "Data Source" }, new string[] { DatasourcePACS2 });
                if (DatasourceResult != null)
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

                //  Precondition to added access filter for Patient name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Patient Name");
                PageLoadWait.WaitForPageLoad(20);
                // Filter Data Sources
                SendKeysInStroke(rolemanagement.RoleAccessFiltersLastName(), LastNameData.ToLowerInvariant());
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.RoleAccessFilterLastNameTxtBoxDropDown().SelectByIndex(0);
                PageLoadWait.WaitForPageLoad(10);
                String RunTimeLastName = rolemanagement.RoleAccessFiltersLastName().GetAttribute("value");
                String RunTimeFirstName = rolemanagement.RoleAccessFiltersFirstName().GetAttribute("value");
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();

                login.Logout();
                login.LoginIConnect(UName, UName);

                studies = (Studies)login.Navigate("Studies");

                //Step 20
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                FirstNameResult = studies.GetMatchingRow(new string[] { "Last Name", "First Name" }, new string[] { RunTimeLastName, RunTimeFirstName });
                if (FirstNameResult != null)
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


                // Precondition to added access filter for Insitution Name
                login.Logout();
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainDropDown().SelectByValue(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.EditRoleByName(RoleName);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> SelectedFilter;
                SelectedFilter = rolemanagement.SelectedFilterCriteria().Options;
                foreach (IWebElement filter in SelectedFilter)
                {
                    if (filter.Text.StartsWith("Patient Name"))
                    {
                        PageLoadWait.WaitForPageLoad(20);
                        filter.Click();
                        PageLoadWait.WaitForPageLoad(20);
                        rolemanagement.RoleAccessFilterRemoveBtn().Click();
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.AccessFiltersInformation().SelectByValue("Institution");
                PageLoadWait.WaitForElementToDisplay(rolemanagement.RoleAccessFiltersTextBox());
                SendKeys(rolemanagement.RoleAccessFiltersTextBox(), InstitutionData);
                rolemanagement.AddAccessFilters().Click();
                rolemanagement.ClickSaveEditRole();

                login.Logout();
                login.LoginIConnect(UName, UName);

                studies = (Studies)login.Navigate("Studies");

                //Step 21
                // Clear all fields, select All Dates, and the Dicom data source and do a Search
                studies.SearchStudy(Institution: InstitutionData, Study_Performed_Period: "All Dates", LastName: "*", Datasource: DatasourcePACS2);
                PageLoadWait.WaitForLoadingMessage(10);
                InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { InstitutionData });

                if (InstitutionResult != null)
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_161147(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Maintenance maintenance;
            Studies studies;
            string[] PID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
            string PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
            string AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
            DomainManagement domainmanagement = null;
            try
            {
                //Step 1: Navigate to http://*^<^*server_hostname*^>^*/WebAccess/Default.ashx
                //Step 2: Log in using the username and password: Administrator/Administrator
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                ExecutedSteps++;
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainAdminUsername = createDomain[DomainManagement.DomainAttr.UserID];
                string DomainAdminPassword = createDomain[DomainManagement.DomainAttr.Password];
                ExecutedSteps++;
                //Step 3: Select Maintenance tab
                maintenance = (Maintenance)login.Navigate("Maintenance");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                int resultcount = 0;
                if (string.Equals(maintenance.SelectedInnerTab().Text, "Statistics"))
                {
                    resultcount++;
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (string.Equals(maintenance.ResultLabel().Text, "System Statistics Details"))
                {
                    resultcount++;
                }
                string[] ExpectedValue = new string[] { "Number of Users:", "License Usage:" };
                string[] ActualValue = maintenance.StatisticsDetails().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
                {
                    resultcount++;
                }
                ExpectedValue = new string[] { "User ID", "Host Name", "Feature Name", "Expiry Date/Time" };
                ActualValue = maintenance.TableHeadings().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
                {
                    resultcount++;
                }
                if (resultcount == 4)
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
                //Step 4: Select Audit link.
                maintenance.Navigate("Audit");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                resultcount = 0;
                if (string.Equals(maintenance.SelectedInnerTab().Text, "Audit"))
                {
                    resultcount++;
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                ExpectedValue = new string[] { "From:", "(dd-MMM-yyyy)", "To:", "Accession Number:", "User ID:", "Patient ID:", "Patient Name:", "Event ID:", "Event Outcomes:" };
                ActualValue = maintenance.AuditDetalisLabel().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Any(actualvalue => actualvalue.Contains(expectedvalue))))
                {
                    resultcount++;
                }
                if (string.Equals(maintenance.ResultLabel().Text, "Audit List"))
                {
                    resultcount++;
                }
                ExpectedValue = new string[] { "Audit Event", "Outcome", "User ID", "Date/Time" };
                ActualValue = maintenance.TableHeadings().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
                {
                    resultcount++;
                }
                if (resultcount == 4)
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
                //Step 5: Verify that correct actions were tracked and viewable by the Administrator
                login.Logout();
                login.LoginIConnect(DomainAdminUsername, DomainAdminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionNumber, LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                SelectStudy1("Accession", AccessionNumber);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PID[1], LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                SelectStudy1("Patient ID", PID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                maintenance.Btn_Search().Click();
                login.Logout();
                RestartIISUsingexe();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                maintenance.Btn_Search().Click();
                //PageLoadWait.WaitForSearchLoad();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                DataTable TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                ExpectedValue = new string[] { "Audit Log Used", "User Authentication/Login", "User Authentication/Logout", "DICOM Instances Accessed", "Application Activity/Application Start", "Application Activity/Application Stop" };
                ActualValue = GetColumnValues(TableValue, "Audit Event");
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 6: Select some values for To and From fields and select Search button.
                //Step 7: Verify the results displayed are within the search range.
                string currentDate = DateTime.Now.ToString("dd-MMM-yyyy");
                //maintenance.SearchInAudit(timezone: "");
                //PageLoadWait.WaitForSearchLoad();
                ExecutedSteps++;
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] DateColumn = GetColumnValues(TableValue, "Date/Time");
                if (DateColumn.All(datecolumn => datecolumn.StartsWith(currentDate)))
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
                //Step 8: Enter an ID in the PatientID field for a patient who's study was previously loaded into the viewer.
                maintenance.SearchInAuditTab(pid: PID[0]);
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                ExpectedValue = new string[] { "DICOM Instances Accessed" };
                ActualValue = GetColumnValues(TableValue, "Audit Event");
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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

                //Step 9:
                maintenance.SearchInAuditTab(pid: PID[1]);
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                ExpectedValue = new string[] { "DICOM Instances Accessed" };
                ActualValue = GetColumnValues(TableValue, "Audit Event");
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 10: Clear the PatientID field then enter a name of a user which was previously created in the User ID field.
                maintenance.SearchInAuditTab(pname: PatientName);
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                ExpectedValue = new string[] { "DICOM Instances Accessed" };
                ActualValue = GetColumnValues(TableValue, "Audit Event");
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 11: CLear all fields Enter UserID for a user who logged into the application
                maintenance.SearchInAuditTab(uid: username);
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                ExpectedValue = new string[] { "Audit Log Used", "User Authentication/Login", "User Authentication/Logout" };
                ActualValue = GetColumnValues(TableValue, "Audit Event");
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 12: CLear all fields Enter Accession for a study that was loaded into the viewer
                maintenance.SearchInAuditTab(AccNo: AccessionNumber);
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                ExpectedValue = new string[] { "DICOM Instances Accessed" };
                ActualValue = GetColumnValues(TableValue, "Audit Event");
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 13: CLear all fields select Event ID fron drop down list
                maintenance.SearchInAuditTab(EID: "DICOM Instances Accessed");
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                ExpectedValue = new string[] { "DICOM Instances Accessed" };
                ActualValue = GetColumnValues(TableValue, "Audit Event");
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 14: Select one of the displayed audit logs.
                maintenance.SearchInAuditTab(EID: "Audit Log Used", uid: username);
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                bool status = true;
                if (TableValue.Rows.Count > 0)
                {
                    foreach (DataRow row in TableValue.Rows)
                    {
                        string[] values = Array.ConvertAll(row.ItemArray, x => Convert.ToString(x));
                        bool[] s = Enumerable.Repeat(false, 4).ToArray();
                        s[0] = string.Equals(values[0], "Audit Log Used");
                        s[1] = string.Equals(values[1], "Success");
                        s[2] = values[2].Contains("Administrator");
                        s[3] = values[3].StartsWith(currentDate);
                        if (s.Contains(false))
                        {
                            status = false;
                            break;
                        }
                    }
                }
                else
                {
                    status = false;
                }
                if (status)
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
                //Step 15: Click on the arrow up beside Hide Search Criteria.
                maintenance.Navigate("Audit");
                ShowHideSearchFields().Click();
                if (SearchPanelDiv().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                ShowHideSearchFields().Click();
                //Step 16: Click on each of the titles of the fields(Audit Event Name, Outcome, User ID, and Date / Time)
                bool AuditEvent = false;
                bool Outcome = false;
                bool UserID = false;
                bool datetime = false;
                maintenance.SearchInAuditTab(AccNo: AccessionNumber, uid: DomainAdminUsername);
                PageLoadWait.WaitForSearchLoad();
                maintenance.TableHeadings()[3].Click();
                maintenance.TableHeadings()[0].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] AscAuditEventName = GetColumnValues(TableValue, "Audit Event");
                maintenance.TableHeadings()[0].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] DescAuditEventName = GetColumnValues(TableValue, "Audit Event");
                maintenance.TableHeadings()[1].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] AscOutcome = GetColumnValues(TableValue, "Outcome");
                maintenance.TableHeadings()[1].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] DescOutcome = GetColumnValues(TableValue, "Outcome");
                maintenance.TableHeadings()[2].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] AscUserID = GetColumnValues(TableValue, "User ID");
                maintenance.TableHeadings()[2].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] DescUserID = GetColumnValues(TableValue, "User ID");
                maintenance.TableHeadings()[3].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] AscDateTime = GetColumnValues(TableValue, "Date/Time");
                AscDateTime = AscDateTime.Select(w => string.IsNullOrWhiteSpace(w.ToString()) ? string.Empty : (DateTime.ParseExact(w.ToString(), "dd-MMM-yyyy h:m:s tt", null).ToString("ddMMyyyyHHmm"))).ToArray();
                maintenance.TableHeadings()[3].Click();
                PageLoadWait.WaitForFrameLoad(20);
                TableValue = CollectRecordsInTable(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                string[] DescDateTime = GetColumnValues(TableValue, "Date/Time");
                DescDateTime = DescDateTime.Select(w => string.IsNullOrWhiteSpace(w.ToString()) ? string.Empty : (DateTime.ParseExact(w.ToString(), "dd-MMM-yyyy h:m:s tt", null).ToString("ddMMyyyyHHmm"))).ToArray();
                if (AscAuditEventName.SequenceEqual((AscAuditEventName.OrderBy(c => c).ToArray())) && DescAuditEventName.SequenceEqual(DescAuditEventName.OrderByDescending(c => c).ToArray()))
                {
                    AuditEvent = true;
                    Logger.Instance.InfoLog("AuditEvent is passed");
                }

                if (AscOutcome.SequenceEqual((AscOutcome.OrderBy(c => c).ToArray())) && DescOutcome.SequenceEqual(DescOutcome.OrderByDescending(c => c).ToArray()))
                {
                    Outcome = true;
                    Logger.Instance.InfoLog("Outcome is passed");
                }
                if (AscUserID.SequenceEqual((AscUserID.OrderBy(c => c).ToArray())) && DescUserID.SequenceEqual(DescUserID.OrderByDescending(c => c).ToArray()))
                {
                    UserID = true;
                    Logger.Instance.InfoLog("UserID is passed");
                }
                if (AscDateTime.SequenceEqual((AscDateTime.OrderBy(c => c).ToArray())) && DescDateTime.SequenceEqual(DescDateTime.OrderByDescending(c => c).ToArray()))
                {
                    datetime = true;
                    Logger.Instance.InfoLog("datetime is passed");
                }
                if (AuditEvent && Outcome && UserID && datetime)
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
                //Step 17: Select Log link.
                maintenance.Navigate("Log");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (ShowHideSearchFields().GetAttribute("src").ToLowerInvariant().Contains("downarrows"))
                {
                    ShowHideSearchFields().Click();
                    PageLoadWait.WaitForFrameLoad(20);
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                resultcount = 0;
                if (string.Equals(maintenance.SelectedInnerTab().Text, "Log"))
                {
                    resultcount++;
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (maintenance.Edt_FromeDate().Displayed && maintenance.Edt_ToDate().Displayed && maintenance.Btn_Search().Displayed)
                {
                    resultcount++;
                }
                ExpectedValue = new string[] { "Log Name", "Log Level", "User ID", "Log Message", "Log Date/Time" };
                ActualValue = maintenance.TableHeadings().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
                {
                    resultcount++;
                }
                if (resultcount == 3)
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
                //Step 18: Select some values for To and From fields and select Search button.
                //Step 19: Verify the results displayed are within the search range.
                if (!string.Equals(maintenance.Edt_FromeDate().GetAttribute("value"), currentDate))
                {
                    maintenance.SearchInAudit(timezone: "");
                }
                if (!string.Equals(maintenance.Edt_FromeDate().GetAttribute("value"), currentDate))
                {
                    maintenance.SearchInAudit(timezone: "");
                }
                maintenance.Btn_Search().Click();
                //PageLoadWait.WaitForSearchLoad();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;
                TableValue = CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                if (TableValue.Rows.Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    DateColumn = GetColumnValues(TableValue, "Log Date/Time");
                    if (DateColumn.All(datecolumn => datecolumn.StartsWith(currentDate)))
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_29304(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = new UserManagement();
            EnrollNewUser EnrollUser = new EnrollNewUser();
            MyProfile profile = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                string email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                //Step 1: 
                //1. Using the WebAccess configuration tool, setup an email address for the Administrator account in order to receive user enrollment requests. Modify accordingly the Web Application URL and the SMTP = DINOBOT Port = 25 Server values.
                //2. In the service tool enable the Self Enrol user Flag located in the Enable Features tab. Restart IIS.
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                string DomainAdminUsername = createDomain[DomainManagement.DomainAttr.UserID];
                string DomainAdminPassword = createDomain[DomainManagement.DomainAttr.Password];
                login.Logout();
                ExecutedSteps++;
                //Step 2: From the WebAccess login screen click on the Register link.
                //Step 3: Fill out the form with the following information then click on submit:
                string User = GetUniqueUserId("Z");
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email))
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
                //Step 4: In the email that was received, click on the url to enroll the current pending request.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 5: Login with the Administrator account.
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.RequestUserExist(User))
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
                //Step 6: Click the refresh button on the page.A list of current pending requests are shown. Select testuser1 from the list and click on Reject.
                if (usermanagement.RequestUser(User, "reject"))
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
                //Step 7: Logout from WebAccess then from the WebAccess login screen click on the Enroll link.
                //Step 8: Fill out the form with the following information then click on submit:
                login.Logout();
                ExecutedSteps++;
                User = GetUniqueUserId();
                if (login.FillEnrollForm(User, DomainName, Email: email))
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
                //Step 9: In the email that was received, click on the url to enroll the current pending request then login with the Administrator account.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 10: Complete the user enrollment but do not provide a password.*/
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                if (usermanagement.RequestUser(User, "accept"))
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
                //Step 11: Click on the link in the email that was sent to the user.
                //Step 12: Complete the enrollment by providing a password then select Save.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 13: Logout from WebAccess then from the WebAccess login screen click on the Enroll link.
                //Step 14 : Fill out the form with the following information then click on submit: for same user
                login.Logout();
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 15: Fill out the form with the following information then click on submit:
                login.DriverGoTo(login.url);
                User = GetUniqueUserId();
                if (login.FillEnrollForm(User, DomainName, Email: email))
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
                //Step 16: In the email that was received, click on the url to enroll the current pending request then login with the Administrator account.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 17 :Change the user First Name to CCC do not provide password then complete the enrollment.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                if (usermanagement.RequestUser(User, "accept", firstname: "firstname"))
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
                //Step 18: Click on the link in the email that was sent to the user.
                //Step 19: Complete the enrollment by providing a password then select Save.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 20: Logout from WebAccess then from the WebAccess login screen click on the Enroll link.
                //Step 21: Fill out the form with the following information then click on submit:
                login.Logout();
                ExecutedSteps++;
                User = GetUniqueUserId();
                if (login.FillEnrollForm(User, DomainName, Email: email))
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
                //Step 22: Send two more enrollment requests.
                string User1 = GetUniqueUserId();
                bool u1 = login.FillEnrollForm(User1, DomainName, Email: email);
                string User2 = GetUniqueUserId();
                bool u2 = login.FillEnrollForm(User2, DomainName, Email: email);
                if (u1 && u2)
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
                //Step 23: In the last email that was received, click on the url to view the list of all pending requests.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 24: Login with the Administrator account.
                login.LoginIConnect(Username, Password);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.RequestUserExist(User) && usermanagement.RequestUserExist(User1) && usermanagement.RequestUserExist(User2))
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
                //Step 25: Click on the Users Tab
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                usermanagement.UserTab().Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                string[] ExpectedValue = new string[] { "Groups", "Users" };
                string[] ActualValue = usermanagement.Userlist().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 26: Click on the Requests Tab
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.RequestUserExist(User) && usermanagement.RequestUserExist(User1) && usermanagement.RequestUserExist(User2))
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
                //Step 27: Click on the Role Management Tab
                login.Navigate("RoleManagement");
                ExecutedSteps++;
                //Step 28: Click on the User Management Tab
                login.Navigate("UserManagement");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                ExpectedValue = new string[] { "Groups", "Users" };
                ActualValue = usermanagement.Userlist().Select(value => value.Text).ToArray();
                if (ExpectedValue.All(expectedvalue => ActualValue.Contains(expectedvalue)))
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
                //Step 29: Click on the Request Tab
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (usermanagement.RequestUserExist(User) && usermanagement.RequestUserExist(User1) && usermanagement.RequestUserExist(User2))
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
                //Step 30: Select a pending request and reject it.
                if (usermanagement.RequestUser(User, "reject"))
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
                //Step 31: Select another pending request do not provide password and complete the enrollment.
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                if (usermanagement.RequestUser(User1, "accept"))
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
                //Step 32: In the email sent to the user click on the url. Provide a password for the user.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 33: At the WebAccess login screen, login with the newly enrolled user.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 34: Logout Login as Domain admin edit domain admin user account with valid email address
                login.Logout();
                login.LoginIConnect(DomainAdminUsername, DomainAdminPassword);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                profile = new MyProfile();
                profile.OpenMyProfile();
                profile.UserEmail().Clear();
                profile.UserEmail().SendKeys(email);
                profile.SaveBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                profile.OpenMyProfile();
                if (string.Equals(profile.UserEmail().GetAttribute("value"), email))
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
                //Step 35: Create a group PG1 *^>^*Create a subgroup SG1
                profile.CloseProfile().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                string PG1 = GetUniqueGroupId();
                usermanagement.CreateGroup(DomainName, PG1);
                string SG1 = GetUniqueGroupId();
                usermanagement.CreateSubGroup(PG1, SG1);
                ExecutedSteps++;
                //Step 36: Create a managed group PG2 with user UPG2 with valid email address*^>^*Create a subgroup SG2
                string PG2 = GetUniqueGroupId();
                string UPG2 = GetUniqueUserId();
                usermanagement.CreateGroup(DomainName, PG2, password: UPG2, email: email, GroupUser: UPG2, IsManaged: 1);
                string SG2 = GetUniqueGroupId();
                usermanagement.CreateSubGroup(PG2, SG2);
                ExecutedSteps++;
                //Step 37: Create a group PG3 *^>^*Create a managed subgroup SG3 with user USG3 with valid email address
                string PG3 = GetUniqueGroupId();
                string USG3 = GetUniqueUserId();
                usermanagement.CreateGroup(DomainName, PG3);
                string SG3 = GetUniqueGroupId();
                usermanagement.CreateSubGroup(PG3, SG3, NewUserName: USG3, IsManaged: 1, email: email);
                ExecutedSteps++;
                //Step 38: logout
                login.Logout();
                ExecutedSteps++;
                //Step 39: From the WebAccess login screen click on the Register link.
                //Step 40: Fill out the form with the following information then click on submit - Group: PG1
                User = GetUniqueUserId();
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email, group: PG1))
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
                //Step 41: In the email that was received on Domain Admin's email, click on the url to enroll the current pending request.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 42: Login with the Domain Admin account.
                //Step 43: Click the refresh button on the page. A list of current pending requests are shown. Select user from the list and click on Reject.
                login.LoginIConnect(DomainAdminUsername, DomainAdminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                ExecutedSteps++;
                if (usermanagement.RequestUser(User, "reject"))
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
                //Step 44: Logout From the WebAccess login screen click on the Register link.
                //Step 45: Fill out the form with the following information then click on submit - Group: SG1
                login.Logout();
                User = GetUniqueUserId();
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email, group: string.Concat(PG1, "\\", SG1)))
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
                //Step 46: In the email that was received on Domain Admin's email, click on the url to enroll the current pending request.
                //Step 47: Complete the user enrollment but do not provide a password.
                //Step 48: Click on the link in the email that was sent to the user.
                //Step 49: Complete the enrollment by providing a password then select Save.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 50: Logout - From the WebAccess login screen click on the Register link.
                //Step 51: Fill out the form with the following information then click on submit Group: PG2
                login.DriverGoTo(login.url);
                User = GetUniqueUserId();
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email, group: PG2))
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
                //Step 52: In the email that was received on UPG2 email, click on the url to enroll the current pending request.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 53: Login with the UPG2 account.
                //Step 54: Click the refresh button on the page. A list of current pending requests are shown. Select user from the list and click on Reject.
                login.LoginIConnect(UPG2, UPG2);
                ExecutedSteps++;
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                if (usermanagement.RequestUser(User, "reject"))
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
                //Step 55: From the WebAccess login screen click on the Register link.
                //Step 56: Fill out the form with the following information then click on submit - Group: SG2
                login.Logout();
                User = GetUniqueUserId();
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email, group: string.Concat(PG2, "\\", SG2)))
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
                //Step 57: In the email that was received on UPG2 email, click on the url to enroll the current pending request.
                //Step 58: Complete the user enrollment but do not provide a password.
                //Step 59: Click on the link in the email that was sent to the user.
                //Step 60: Complete the enrollment by providing a password then select Save.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 61: From the WebAccess login screen click on the Register link.
                //Step 62: Fill out the form with the following information then click on submit - Group: PG3
                login.DriverGoTo(login.url);
                User = GetUniqueUserId();
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email, group: PG3))
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
                //Step 63: In the email that was received on Domain Admin's email, click on the url to enroll the current pending request.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 64: Login with the Domain Admin account.
                //Step 65: Click the refresh button on the page. A list of current pending requests are shown. Select user from the list and click on Reject.
                login.LoginIConnect(DomainAdminUsername, DomainAdminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                NavigateToRequestsUserManagementTab();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermanagement.RefreshBtn().Click();
                ExecutedSteps++;
                if (usermanagement.RequestUser(User, "reject"))
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
                //Step 66: From the WebAccess login screen click on the Register link.
                //Step 67: Fill out the form with the following information then click on submit - Group: SG3
                login.Logout();
                User = GetUniqueUserId();
                ExecutedSteps++;
                if (login.FillEnrollForm(User, DomainName, Email: email, group: string.Concat(PG3, "\\", SG3)))
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
                //Step 68: In the email that was received on USG3 email, click on the url to enroll the current pending request.
                //Step 69: Complete the user enrollment but do not provide a password.
                //Step 70: Click on the link in the email that was sent to the user.
                //Step 71: Complete the enrollment by providing a password then select Save.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }


        public TestCaseResult Test_29303(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            WpfObjects wpfobject = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);
                ServiceTool tool = null;
                // string[] Datasource = { "AUTO-SSA-001", "VMSSA-4-38-131", "VMSSA-5-38-91" };

                String DatasourceAutoSSA = login.GetHostName(Config.EA77);
                String DatasourceVMSSA131 = login.GetHostName(Config.EA1);
                String DatasourceVMSSA91 = login.GetHostName(Config.EA91);
                String RefPhysicainData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                String PatientIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ModalityData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String IPIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String InstitutionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                String AccessionNumberData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                String StudyPerformedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedData");

                // Step 1
                // 1.Configure / Start a Dicom data source (on another system or Virtual Machine)
                // 2.Modify the application Configuration files to add the Dicom Data Source
                // 3.Login as Administrator, and add the data source to a domain.Select / Create a user 
                // account in this domain, and configure the role filter so that studies from this data source will be retrieved.


                // // Invoke service tools
                // tool.InvokeServiceTool();

                //// Enable service tools
                //tool.SetEnableFeaturesGeneral();
                //wpfobject.WaitTillLoad();
                //tool.ModifyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //tool.EnablePatient();
                //wpfobject.WaitTillLoad();
                //tool.ApplyEnableFeatures();
                //wpfobject.WaitTillLoad();
                //wpfobject.ClickOkPopUp();
                //wpfobject.WaitTillLoad();

                //Step 2
                //Navigate to Iconnect 
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                studies = (Studies)login.Navigate("Studies");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, RoleName, datasources: new string[] { DatasourceAutoSSA, DatasourceVMSSA131, DatasourceVMSSA91 });
                domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(UName, DomainName, RoleName);
                PageLoadWait.WaitForPageLoad(10);
                login.Logout();

                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.AddInsitutionInEditPage(DomainName, "Univ");
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                studies.ClickChooseColumns(section: "Others1");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));
                studies.SelectColumns(new string[] { "Institutions" }, "Add", isRerarrange: false);
                studies.OKButton_ChooseColumns().Click();
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);

                //Step 2 

                login.Logout();
                login.LoginIConnect(UName, UName);
                ExecutedSteps++;


                // Step 3
                // Select Study Performed = All Dates  Clear all other search fields 
                // Search all studies.
                // Verify that only studies available to the Domain are displayed.

                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                Dictionary<string, string> InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { "UNIV" });
                if (InstitutionResult != null)
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

                // Step 4 
                // Logout as Administrator and Login as U1 user in the domain edited above
                login.Logout();
                PageLoadWait.WaitForPageLoad(10);
                login.LoginIConnect(Username, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.AddInsitutionInEditPage(DomainName, "UN");
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);
                login.Logout();
                login.LoginIConnect(UName, UName);
                ExecutedSteps++;

                // Step 5
                // Select Study Performed = All Dates  Clear all other search fieldsSearch all studies.  Verify that only studies available to the Domain are displayed.

                studies.SearchStudy(Study_Performed_Period: "All Dates", LastName: "*");
                PageLoadWait.WaitForLoadingMessage(10);
                InstitutionResult = studies.GetMatchingRow(new string[] { "Institutions" }, new string[] { "UN" });
                if (InstitutionResult != null)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }


        public TestCaseResult Test_29291(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;

                //Step 1
                //Navigate to http//<serverip>/WebAccess/Default.ashx
                // Log in using the username and password: Administrator/Administrator
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management") &&
                    login.IsTabPresent("Role Management") && login.IsTabPresent("User Management")
                    && login.IsTabPresent("System Settings") && login.IsTabPresent("Maintenance") && login.IsTabPresent("Inbounds")
                    && login.IsTabPresent("Outbounds") && login.IsTabPresent("Image Sharing"))
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Domain Management-Split1
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test1_29292(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;

            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');
                String Domains = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames");
                String[] DomainName = Domains.Split(':');
                string message = "Cannot delete the selected domain as the domain is currently assigned to one or more users.";
                string description = "description";
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));

                String user1 = "user_" + random.Next(1, limit);

                //Step-1: Click on the Domain Management tab.
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool bDomainB = domainmanagement.IsDomainExist(DomainName[0]);
                if (bDomainB)
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


                //Step-2: Click on the New Domain button.
                domainmanagement.ClickNewDomainBtn();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.DomainManagementHeaderLabel().Text.Equals("Domain Management") && domainmanagement.NewDomainHeaderLabel().Text.Equals("New Domain") &&
                    domainmanagement.DomaininfoLabel().Text.Equals("Domain Information") && domainmanagement.EnterDomaininfoLabel().Text.Equals("Enter Domain Information") &&
                    domainmanagement.InstitutionLabel().Text.Equals("Institutions") && domainmanagement.DataSourceLabel().Text.Equals("Data Sources") && domainmanagement.AccessFilterLabel().Text.Equals("Access Filters Information")
                    && domainmanagement.StudySearchLabel().Text.Equals("Study Search Fields") && domainmanagement.IpadstudyLabel().Text.Equals("iPad Study List Fields") && domainmanagement.PatientHistoryLayoutLabel().Text.Equals("Patient History Layout")
                    && domainmanagement.ToolbarConfigLabel().Text.Equals("Toolbar Configuration") && domainmanagement.StudyListLayoutLabel().Text.Equals("Study List Layout") && domainmanagement.ExternalApplicationsLabel().Text.Equals("External Applications") &&
                    domainmanagement.DefaultModalitySettingLabel().Text.Equals("Default Settings Per Modality") && domainmanagement.ContactLabel().Text.Equals("Contacts") && domainmanagement.ArchiveNominationLabel().Text.Equals("Archive Nomination Reasons") &&
                    domainmanagement.DomainAdminRoleinfoLabel().Text.Equals("Domain Admin Role Information") && domainmanagement.DomainAdminUserinfoLabel().Text.Equals("Domain Admin User Information"))
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
                domainmanagement.CloseDomainManagement();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);


                //Step-3: Fill in all of the fields, add at least one modality alias, select at least one Data Source and click save.(Use D1 as the domain name)
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainAdminID = domainattr[DomainManagement.DomainAttr.UserID];
                String passwordB = domainattr[DomainManagement.DomainAttr.Password];
                String domainAdminName = domainattr[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool bDomainB3 = domainmanagement.IsDomainExist(DomainB);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.SelectDomainfromDropDown(DomainB);
                if (bDomainB3 && rolemanagement.RoleExists(domainAdminName))
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


                //Step-4: Navigate to UserManagement tab- select newly created domain from drop down
                //click new user-fill all fields-click create - 
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user1, DomainB, domainAdminName);

                if (usermanagement.SearchUser(user1, DomainB))
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


                //Step-5: navigate to DomainManagement & Select the newly created Domain.   
                //Verify The Domain is highlighted and its text is bolded.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                string[] CurrentlyListedDomain5 = domainmanagement.DomainNameList();
                bool flag5 = false;
                int noOfPages5 = domainmanagement.PageandNext().Count;
                int noOfDomain5 = 0;
                int j = 0;
                if (noOfPages5 == 0)
                {
                    CurrentlyListedDomain5 = domainmanagement.DomainNameList();
                    if (CurrentlyListedDomain5.Contains(DomainB))
                    {
                        domainmanagement.SelectDomain(DomainB);
                        flag5 = true;
                    }
                    noOfDomain5 = domainmanagement.DomainList().Count;
                }
                else
                {
                    for (j = 0; j < noOfPages5; j++)
                    {

                        CurrentlyListedDomain5 = domainmanagement.DomainNameList();
                        noOfDomain5 = noOfDomain5 + domainmanagement.DomainList().Count;
                        if (CurrentlyListedDomain5.Contains(DomainB))
                        {
                            domainmanagement.SelectDomain(DomainB);
                            flag5 = true;
                            break;
                        }
                        else
                        {
                            domainmanagement.PageandNext()[noOfPages5 - 1].Click();
                        }

                    }

                }

                IWebElement d1 = BasePage.Driver.FindElement(By.CssSelector("span[title='" + DomainB + "']"));
                if (flag5 == true && d1.Text.Equals(DomainB) && d1.GetCssValue("font-weight").Equals("bold"))
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


                //Step-6: Click on the Delete button. 
                domainmanagement.ClickDeleteDomainBtn();

                domainmanagement.SwitchToDefault();
                domainmanagement.SwitchTo("id", "UserHomeFrame");

                if (domainmanagement.GetElement("id", "ctl00_AlertText").Text.Equals(message))
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

                //Step-7: Click OK on dialog -- 
                //-Verify domain is not deleted
                domainmanagement.ClickCloseAlertButton();
                //--change the frame
                domainmanagement.SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                if (domainmanagement.DomainExists(DomainB))
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


                //Step-8: With the new domain still selected, click on the Edit Domain button.
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.DomainManagementHeaderLabelEditDomain().Text.Equals("Domain Management") && domainmanagement.PageHeaderLabel().Text.Equals("Edit Domain") &&
                    domainmanagement.DomaininfoLabelEditDomain().Text.Equals("Domain Information") && domainmanagement.EnterDomaininfoLabelEditDomain().Text.Equals("Enter Domain Information") &&
                    domainmanagement.InstitutionLabelED().Text.Equals("Institutions") && domainmanagement.DataSourceLabel().Text.Equals("Data Sources")
                    && domainmanagement.StudySearchLabel().Text.Equals("Study Search Fields") && domainmanagement.IpadstudyLabelED().Text.Equals("iPad Study List Fields") && domainmanagement.PatientHistoryLayoutLabel().Text.Equals("Patient History Layout")
                    && domainmanagement.ToolbarConfigLabel().Text.Equals("Toolbar Configuration") && domainmanagement.StudyListLayoutLabel().Text.Equals("Study List Layout") &&
                    domainmanagement.DefaultModalitySettingLabel().Text.Equals("Default Settings Per Modality") && domainmanagement.ContactLabel().Text.Equals("Contacts") && domainmanagement.ArchiveNominationLabel().Text.Equals("Archive Nomination Reasons"))
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


                //Step-9: Modify the Domain Description and click on the Save button.
                domainmanagement.EditDomainDescription().Clear();
                domainmanagement.EditDomainDescription().Click();
                domainmanagement.EditDomainDescription().SendKeys(description);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                bool step9 = PageLoadWait.WaitForWebElement(By.CssSelector("div.row tr td>span[title='" + description + "']"), "exists", 90);
                //IWebElement desc = Driver.FindElement(By.CssSelector("div.row tr td>span[title='" + description + "']"));
                if (step9 == true)
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

                //Step-10: In the Domain Name search field, enter first char of the Domain name that was created and click the Search button.
                domainmanagement.SearchDomain("D");
                bool flag = false;
                foreach (IWebElement domain in domainmanagement.DomainDetails())
                {
                    if (domain.Text.Equals(DomainB))
                    {
                        flag = true;
                    }
                }
                string[] CurrentlyListedDomain = domainmanagement.DomainNameList();

                int noOfPages = domainmanagement.PageandNext().Count;
                int noOfDomain = 0;
                int i = 0;
                if (noOfPages == 0)
                {
                    CurrentlyListedDomain = domainmanagement.DomainNameList();
                    if (CurrentlyListedDomain.Contains(DomainB))
                    {
                        flag = true;
                    }

                }
                else
                {
                    for (i = 0; i < noOfPages; i++)
                    {

                        CurrentlyListedDomain = domainmanagement.DomainNameList();
                        if (CurrentlyListedDomain.Contains(DomainB))
                        {
                            flag = true;
                            break;
                        }
                        else
                        {
                            domainmanagement.PageandNext()[noOfPages - 1].Click();
                        }

                    }

                }

                for (i = 0; i < noOfPages; i++)
                {
                    noOfDomain = noOfDomain + domainmanagement.DomainList().Count;
                    domainmanagement.PageandNext()[noOfPages - 1].Click();
                }


                string domaincountlabel = domainmanagement.ResultcountLabel().Text;
                String[] domaincount = domaincountlabel.Split(' ');
                if (!(domainmanagement.ShowAllDomainCB().Selected) && flag == true && (noOfDomain - (i)) == int.Parse(domaincount[0]))
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


                //Step-11: Check the Show All Domains check box.
                domainmanagement.ShowAllDomainCB().Click();
                var str = domainmanagement.DoaminNameTb().GetAttribute("value");
                if (str.Equals(""))
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
        ///Domain Management-Split2
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_29292(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;

            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');
                String Domains = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames");
                String[] DomainName = Domains.Split(':');
                string message1 = "Window Width and/or Level has an invalid value.";
                string message2 = "Width and/or Level values are invalid or empty.";
                string message3 = "The characters '#', '@', ':', '{', '}' and '/' are not allowed.";
                string Institutuon = "Institutuon";
                String[] PresetNames = { "a", "step26$", "b" };
                String[] InvalidPresetNames = { "#a" };

                string removeddatasourceName = Config.PACS2AETitle;
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));

                String user1 = "user_" + random.Next(1, limit);

                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainAdminID = domainattr[DomainManagement.DomainAttr.UserID];
                String passwordB = domainattr[DomainManagement.DomainAttr.Password];
                String domainAdminName = domainattr[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user1, DomainB, domainAdminName);

                //Step-1: With the new domain still selected, click on the Edit  Domain button.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.DomainManagementHeaderLabelEditDomain().Text.Equals("Domain Management") && domainmanagement.PageHeaderLabel().Text.Equals("Edit Domain"))
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


                //Step-2: Modify the Data Sources available to the domain and click on the Save button.
                domainmanagement.DisConnectDataSourcesConsolidated_EditDomain(removeddatasourceName);
                domainmanagement.ClickSaveEditDomain();
                var datasorcelist = new List<String>();



                if (domainmanagement.verifyDomainDatasources(DomainB, removeddatasourceName) == false)
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


                //Step-3: With the new domain still selected, click on the Edit  Domain button.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.DomainManagementHeaderLabelEditDomain().Text.Equals("Domain Management") && domainmanagement.PageHeaderLabel().Text.Equals("Edit Domain"))
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



                //Step-4: Select CT modality, change the layout, add a new modality alias named test, 
                //window width as 100 and level as 500, and click on the Close button.
                domainmanagement.AddPresetForDomain("CT", "test", "100", "500");
                domainmanagement.ClickCloseEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.EditDomainButton().Displayed)
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


                //Step-5: With the new domain still selected, click on the Edit  Domain button.  Verify the 
                //previous modality viewing protocol settings were not saved.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step16 = domainmanagement.VerifyPresetsInDomain("CT", "auto", "test");
                if (step16 == false)
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

                //Step-6: Select CT modality, change the layout, add a new modality alias named test, window
                //width as 100 and level as 500 and click on the save button.                
                domainmanagement.AddPresetForDomain("CT", "test", "100", "500");
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.EditDomainButton().Displayed)
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


                //Step-7: With the new domain still selected, click on the Edit  Domain button.  Verify the 
                //previous modality viewing protocol settings were not saved.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-8: Select CT modality, change the layout, add a new modality alias named test, window 
                //width as 100 and level as 500, select Add/Modify button, and select save button.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-9: With the new domain still selected, click on the Edit  Domain button.  Verify the 
                //previous modality viewing protocol settings were saved.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step9 = domainmanagement.VerifyPresetsInDomain("CT", "auto", "test");
                if (step9 == true)
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

                //Step-10: Add a CT modality alias named "a", -br/-window width as"--333"and level"--333", and 
                //click on the Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", PresetNames[0], "---333", "---333");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_m_viewingProtocolsControl_ValidationErrorLabel']")));
                if (domainmanagement.PresetInvalidMsg().Text.Equals(message1))
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

                //Step-11: Add a CT modality alias named a, window width as blank value and level -333, 
                //and click on the Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", PresetNames[0], "", "-333");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_m_viewingProtocolsControl_ValidationErrorLabel']")));
                if (domainmanagement.PresetInvalidMsg().Text.Equals(message2))
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

                //Step-12: Add a CT modality alias named a, window width as 123 and level as blank value, 
                //and click on the Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", PresetNames[0], "123", "");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_m_viewingProtocolsControl_ValidationErrorLabel']")));
                if (domainmanagement.PresetInvalidMsg().Text.Equals(message2))
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

                //Step-13: Add a CT modality alias named a, window width and level as some invalid values (including special characters),
                //and click on the Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", PresetNames[0], "123$", "#%34");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_m_viewingProtocolsControl_ValidationErrorLabel']")));
                if (domainmanagement.PresetInvalidMsg().Text.Equals(message1))
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

                //Step-14: Add a CT modality alias using invalid special characters in the name, window width 
                //and level as 100, and click on the Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", InvalidPresetNames[0], "100", "100");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_m_viewingProtocolsControl_ValidationErrorLabel']")));
                if (domainmanagement.PresetInvalidMsg().Text.Equals(message3))
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

                //Step-15: Add a CT modality alias using valid special characters in the name, window width and
                //level as 100, and click on the Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", PresetNames[1], "100", "100");
                bool step26 = domainmanagement.VerifyPresetsInDomain("CT", "auto", "step26$");
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


                //Step-16: Add a CT modality alias named a, window width as 123 and level as 456, and click on 
                //the Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", PresetNames[0], "123", "456");
                bool step27 = domainmanagement.VerifyPresetsInDomain("CT", "auto", "a");
                if (step27)
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

                //step-17: Add a CT modality alias named b, window width as 222 and level as -456.78, and click on the
                //Add/Modify button.
                domainmanagement.AddPresetForDomain("CT", PresetNames[2], "222", "-456.78");
                bool step28 = domainmanagement.VerifyPresetsInDomain("CT", "auto", "a");
                if (step28)
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

                //step-18: Add a CT modality alias named a, window width as 222 and level as -456, and 
                //click on the Add/Modify button.
                domainmanagement.ModalityDropDown().SelectByText("CT");
                domainmanagement.PresetSelect().SelectByText(PresetNames[0]);
                domainmanagement.AddPresetForDomain("CT", PresetNames[0], "222", "-456.78");
                bool step18 = domainmanagement.VerifyPresetsInDomain("CT", "auto", "a");
                if (step18)
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

                //Step-19: Add a MR modality alias named a, window width as 222 and level as -456, 
                //and click on the Add/Modify button.-br/-click Save
                domainmanagement.AddPresetForDomain("MR", PresetNames[0], "222", "-456.78");
                bool step30 = domainmanagement.VerifyPresetsInDomain("CT", "auto", "a");
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.EditDomainButton().Displayed && step30)
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

                //step-20: Edit the new domain Select CT modality from the drop down list.-br/-Select the alias 
                //drop down list, and verify the entries.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.ModalityDropDown().SelectByText("CT");
                int count = domainmanagement.PresetDropdown().Count;
                bool step31 = false;
                /*if (count == 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        step31 = domainmanagement.VerifyPresetsInDomain("CT", "auto", PresetNames[i]);
                    }
                }*/
                step31 = domainmanagement.VerifyPresetsInDomain("CT", "auto", PresetNames[0]);
                bool step31_2 = domainmanagement.VerifyPresetsInDomain("CT", "auto", PresetNames[1]);
                bool step31_3 = domainmanagement.VerifyPresetsInDomain("CT", "auto", PresetNames[2]);

                if (step31 && step31_2 && step31_3)
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

                //Step-21: Select Save, and select the same domain for editing again.
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                IWebElement td = Driver.FindElement(By.CssSelector("div.row tr[style*='font-weight: bold;'] span[title='" + DomainB + "']"));
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (td != null)
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


                //Step-22: Select CT modality from the drop down list.-br/-Select the alias drop down list, and verify the entries.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-23: Select Save, and select the same domain for editing again.
                domainmanagement.ModalityDropDown().SelectByText("CT");
                domainmanagement.LayoutDropDown().SelectByText("1x3");
                domainmanagement.ModalityDropDown().SelectByText("MR");
                domainmanagement.LayoutDropDown().SelectByText("2x2");
                domainmanagement.ModalityDropDown().SelectByText("CR");
                domainmanagement.LayoutDropDown().SelectByText("2x3");
                ExecutedSteps++;

                //Step-24: Select each of the modalities where the layout was changed, and verify the change was preserved.

                domainmanagement.ModalityDropDown().SelectByText("CT");
                IWebElement layoutSelected1 = BasePage.Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']>option[selected='selected']"));
                var layout1 = domainmanagement.LayoutDropDown().SelectedOption.Text;
                domainmanagement.ModalityDropDown().SelectByText("MR");
                IWebElement layoutSelected2 = BasePage.Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']>option[selected='selected']"));
                var layout2 = domainmanagement.LayoutDropDown().SelectedOption.Text;
                domainmanagement.ModalityDropDown().SelectByText("CR");
                IWebElement layoutSelected3 = BasePage.Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']>option[selected='selected']"));
                var layout3 = domainmanagement.LayoutDropDown().SelectedOption.Text;
                if (layout1.Equals("1x3") && layout2.Equals("2x2") && layout3.Equals("2x3"))
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


                //Step-25: Select Save, and select the same domain for editing again.
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                IWebElement td1 = Driver.FindElement(By.CssSelector("div.row tr[style*='font-weight: bold;'] span[title='" + DomainB + "']"));
                if (domainmanagement.EditDomainButton().Displayed && td1 != null)
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


                //Step-26: Select each of the modalities where the layout was changed, and verify the change was preserved.
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.ModalityDropDown().SelectByText("CT");
                IWebElement layoutSelected4 = BasePage.Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']>option[selected='selected']"));
                var layout4 = domainmanagement.LayoutDropDown().SelectedOption.Text;
                domainmanagement.ModalityDropDown().SelectByText("MR");
                IWebElement layoutSelected5 = BasePage.Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']>option[selected='selected']"));
                var layout5 = domainmanagement.LayoutDropDown().SelectedOption.Text;
                domainmanagement.ModalityDropDown().SelectByText("CR");
                IWebElement layoutSelected6 = BasePage.Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']>option[selected='selected']"));
                var layout6 = domainmanagement.LayoutDropDown().SelectedOption.Text;
                if (layout1.Equals("1x3") && layout2.Equals("2x2") && layout3.Equals("2x3"))
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
                domainmanagement.ClickCloseEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-27: With the new domain still selected, click on the Edit Domain button.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.DomainManagementHeaderLabelEditDomain().Text.Equals("Domain Management") && domainmanagement.PageHeaderLabel().Text.Equals("Edit Domain"))
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

                //Step-28: Remove a modality alias, and select Close.
                domainmanagement.RemovePreset("CT", PresetNames[0]);
                domainmanagement.ClickCloseEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.EditDomainButton().Displayed)
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

                //Step-29: With the new domain still selected, click on the Edit  Domain button.
                //Verify if the alias was removed.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step40 = domainmanagement.VerifyPresetsInDomain("CT", "1x3", PresetNames[0]);
                if (step40 == true)
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

                //Step-30: Remove a modality alias, and select Save.
                domainmanagement.RemovePreset("CT", PresetNames[0]);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.EditDomainButton().Displayed)
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

                //Step-31: With the new domain still selected, click on the Edit Domain button.
                //-br/-Verify if the alias was removed.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step42 = domainmanagement.VerifyPresetsInDomain("CT", "1x3", PresetNames[0]);
                if (step42 == false)
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


                //Step-32: Remove multiple modality aliases, and select Close
                domainmanagement.RemoveAllPresets("CT");
                domainmanagement.ClickCloseEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.EditDomainButton().Displayed)
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

                //Step-33: With the new domain still selected, click on the Edit  Domain button.
                //-br/-Verify if the aliases were added.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step44_1 = domainmanagement.VerifyPresetsInDomain("CT", "1x3", PresetNames[1]);
                bool step44_2 = domainmanagement.VerifyPresetsInDomain("CT", "1x3", PresetNames[2]);
                if (step44_1 == true && step44_2 == true)
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


                //Step-34: Remove multiple modality aliases, and select Save.
                domainmanagement.RemoveAllPresets("CT");
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.EditDomainButton().Displayed)
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


                //Step-35: With the new domain still selected, click on the Edit  Domain button.
                //br/-Verify if the aliases were removed.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step46_1 = domainmanagement.VerifyPresetsInDomain("CT", "1x3", PresetNames[1]);
                bool step46_2 = domainmanagement.VerifyPresetsInDomain("CT", "1x3", PresetNames[2]);
                if (step46_1 == false && step46_2 == false)
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

                //Step-36: Modify the Included Institutions list so that it will match some different institution 
                //values (e.g.. Tor), and click on the Save button.
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("Institutuon");
                domainmanagement.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                string[] Tabs = login.GetAvailableTabs();
                bool step36_1 = login.CheckStringinStringArray(Tabs, "Domain Management");
                PageLoadWait.WaitForFrameLoad(20);
                if (step36_1)
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

                //Step-37: Logout 
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
        /// Domain Management-Split3
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test3_29292(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;

            TestCaseResult result;

            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accession = AccessionList.Split(':');
                String Domains = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainNames");
                String[] DomainName = Domains.Split(':');
                string Institutuon = "Institution";
                String[] ContactNames = { "Bob", "steve" };
                String[] ContactNumbers = { "3333", "435456" };
                string description = "description";
                string errormsg = "At least one data source is required.";
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));

                String user1 = "user_" + random.Next(1, limit);

                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainAdminID = domainattr[DomainManagement.DomainAttr.UserID];
                String passwordB = domainattr[DomainManagement.DomainAttr.Password];
                String domainAdminName = domainattr[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                //usermanagement = (UserManagement)login.Navigate("UserManagement");
                //usermanagement.CreateUser(user1, DomainB, domainAdminName);
                login.Logout();

                //Step-1: Login as Domain Administrator (of previously modified domain).
                login.LoginIConnect(domainAdminID, passwordB);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (login.IsTabPresent("User Management"))
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

                //Step-2: From Role Management tab, Select New Role button.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.ClickButtonInRole("new");
                if (rolemanagement.RoleManagemantTitle().Text.Equals("Role Management") && rolemanagement.SubHeading().Text.Equals("New Role"))
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

                //Step-3: Select the Institution Access Filter.
                rolemanagement.AccessFiltersInformation().SelectByValue(Institutuon);
                IList<IWebElement> SelectedFilter = null;
                SelectedFilter = rolemanagement.AccessFilterBox().Options;
                if (SelectedFilter.Count == 0)
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

                //Step-4: From Domain Management , -br/-1. Change the Domain description-br/-2. Disable Report Viewing, 
                //Attachment Uploading, E-Mail Study, and Saving GSPS -br/-3. Set the default layout for CR to 2x2-br/-4. 
                //Add a couple W/L Presets-br/-5. Add a couple Contacts-br/-Click Save.
                domainmanagement.CloseDomainManagement();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                //domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.EditDomainDescription().Clear();
                domainmanagement.EditDomainDescription().Click();
                domainmanagement.EditDomainDescription().SendKeys(description);
                domainmanagement.SetCheckBoxInEditDomain("reportview", 1);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 1);
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 1);
                domainmanagement.SetCheckBoxInEditDomain("savegsps", 1);
                domainmanagement.ModalityDropDown().SelectByText("CT");
                domainmanagement.LayoutDropDown().SelectByText("2x2");
                domainmanagement.AddPresetForDomain("CT", "test1", "100", "500");
                domainmanagement.AddPresetForDomain("CT", "test2", "200", "600");
                domainmanagement.AddContactEditDomain(ContactNames[0], ContactNumbers[0]);
                domainmanagement.AddContactEditDomain(ContactNames[1], ContactNumbers[1]);
                domainmanagement.ClickSaveEditDomain();

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                ExecutedSteps++;


                //Step-5: Verify the previous changes were saved.                
                //domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step5_1 = domainmanagement.VerifyPresetsInDomain("CT", "2x2", "test1");
                bool step5_2 = domainmanagement.VerifyPresetsInDomain("CT", "2x2", "test2");
                bool step5_3 = !domainmanagement.VerifyCheckBoxInEditDomain("reportview");
                bool step5_4 = !domainmanagement.VerifyCheckBoxInEditDomain("attachmentupload");
                bool step5_5 = !domainmanagement.VerifyCheckBoxInEditDomain("emailstudy");
                bool step5_6 = !domainmanagement.VerifyCheckBoxInEditDomain("savegsps");
                bool step5_7 = domainmanagement.ContactsAddedTb()[0].Text.Contains(ContactNames[0] + ": " + ContactNumbers[0]);
                bool step5_8 = domainmanagement.ContactsAddedTb()[1].Text.Contains(ContactNames[1] + ": " + ContactNumbers[1]);

                if (step5_1 && step5_2 && step5_3 && step5_4 && step5_5 && step5_6 && step5_7 && step5_8)
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


                //Step-6: Logout of the Domain Administrator Account
                login.Logout();
                ExecutedSteps++;

                //Step-7: Login as System Administrator.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-8: Click on each of the titles of the fields (Domain Name, Domain Description, and Data Sources)
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                int count17 = 0;
                int i = 0;
                foreach (IWebElement heading in domainmanagement.DomainDetailsortBy())
                {
                    domainmanagement.ColumnHeadings()[i].Click();
                    if (!heading.GetAttribute("style").Contains("display: none"))
                    {
                        count17++;
                    }
                    i++;
                }
                if (count17 == 4)
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

                //Step-9: Select any domain, and select Edit.
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                string datasources = null;
                IList<String> row = new List<String>();
                foreach (IWebElement record in domainmanagement.DomainDetails())
                {
                    row.Add(record.Text);
                }
                for (int k = 0; k < row.Count; k++)
                {
                    if (row[k].Contains(DomainB))
                    {
                        datasources = row[k + 2];
                    }
                }
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (domainmanagement.DomainManagementHeaderLabelEditDomain().Text.Equals("Domain Management") && domainmanagement.PageHeaderLabel().Text.Equals("Edit Domain"))
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

                //Step-10: Disconnect all datasources, and select Save.
                domainmanagement.DisConnectAllDataSources();
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.ClickSaveEditDomain();
                string msg = domainmanagement.ErrorMessage();
                if (msg.Equals(errormsg))
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

                //Step-11: Select Close. -br/-Logout
                domainmanagement.CloseDomainManagement();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.SearchDomain(DomainB);
                PageLoadWait.WaitForFrameLoad(30);
                bool step11 = false;
                IList<String> rows = new List<String>();
                foreach (IWebElement record in domainmanagement.DomainDetails())
                {
                    rows.Add(record.Text);
                }
                for (int k = 0; k < rows.Count; k++)
                {
                    if (rows[k].Contains(DomainB) && rows[k + 2].Contains(datasources))
                    {
                        step11 = true;
                    }
                }

                //login.Logout();
                if (step11 == true)
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

        /// <summary>
        ///  Domain Management-Split4
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test4_29292(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            servicetool = new ServiceTool();
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String roleB1 = "TestRole_29292_" + random.Next(1, limit);
                String user1 = "user_" + random.Next(1, limit);

                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainAdminID = domainattr[DomainManagement.DomainAttr.UserID];
                String passwordB = domainattr[DomainManagement.DomainAttr.Password];
                String domainAdminName = domainattr[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user1, DomainB, domainAdminName);
                login.Logout();
                string message = "Do you want to delete domain " + DomainB + " ? Ldap users in domain " + DomainB + " may not be able to login if the domain is deleted.";
                string message1 = "Cannot delete the selected role as the role is currently assigned to an user.";


                //Step-1: Pre-Condition-br/-1. In the service tool -*^-^* Integrator Tab set UserSharing and Shadow User 
                //to Always Disabled-br/-2- Create another role for the Admin domain (testrole) and save it.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Disabled", shadowuser: "Always Disabled");
                servicetool.WaitWhileBusy();
                servicetool.CloseServiceTool();
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DomainB, roleB1, "Conference=Physician");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                ExecutedSteps++;


                //Step-2: Login as admin and edit the Default System domain management page.
                login.LoginIConnect(domainAdminID, passwordB);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step7 = PageLoadWait.WaitForWebElement(By.CssSelector("[id$='EditDomainControl_DefaultRoleLabel']"), "exists");
                if (!step7)
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

                //Step-3: Logout of the Domain Administrator Account              
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                login.Logout();
                ExecutedSteps++;

                //Step-4: in the service tool -*^-^* Integrator Tab set UserSharing and Shadow User to Always Enabled,  Apply and reset IIS
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled", shadowuser: "Always Enabled");
                servicetool.WaitWhileBusy();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-5: Login as System Administrator.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-6: Open the Domain Management page for the Default System domain and at the bottom change the Default Role to the new role created, Save
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.EditDomainDefaultRole().SelectByText(roleB1);
                domainmanagement.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step-7: Go back and edit the same domain and verify the Default Role changes were saved.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                IWebElement selected = BasePage.Driver.FindElement(By.CssSelector("select[id$='EditDomainControl_m_DefaultRoleInputControl_m_selectorList'] option[selected='selected']"));
                if (selected.Text.Equals(roleB1))
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
                domainmanagement.ClickCloseEditDomain();

                //Step-8: Delete all Users and Roles associated with the Domain that was created.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.UserControl(user1, "delete");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                bool buserB1 = usermanagement.SearchUser(user1, DomainB);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(roleB1);
                rolemanagement.SelectRole(roleB1);
                rolemanagement.ClickButtonInRole("delete");
                bool broleB1 = rolemanagement.RoleExists(roleB1, DomainB);
                if (buserB1 == false && broleB1 == false)
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



                //Step-9: Select the Domain and click on the Delete Domain Button.
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DomainB);
                domainmanagement.SelectDomain(DomainB);
                domainmanagement.ClickDeleteDomainBtn();
                domainmanagement.SwitchToDefault();
                domainmanagement.SwitchTo("id", "UserHomeFrame");
                bool step9 = domainmanagement.GetElement("id", "ctl00_ConfirmationText").Text.Equals(message);
                if (step9)
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


                //Step-10: Click Ok in the Confirmation dialog.
                domainmanagement.ConfirmDeleteDomain();
                bool bDomainB = domainmanagement.IsDomainExist(DomainB);
                if (bDomainB == false)
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


                //Step-11: Select SuperRole, and select Delete.
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Click("id", "DeleteRoleButton");
                PageLoadWait.WaitForPageLoad(10);
                rolemanagement.SwitchToDefault();
                rolemanagement.SwitchTo("id", "UserHomeFrame");
                bool step11 = rolemanagement.GetElement("id", "ctl00_AlertText").Text.Equals(message1);
                if (step11)
                {
                    rolemanagement.CloseAlertBox().Click();
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    rolemanagement.CloseAlertBox().Click();
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

        public TestCaseResult Test1_29294(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            WpfObjects wpfobject = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                //String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);
                ServiceTool tool = null;
                // string[] Datasource = { "AUTO-SSA-001", "VMSSA-4-38-131", "VMSSA-5-38-91" };
                String DefaultDomain = "SuperAdminGroup";
                String DomainAdminName = "DomainAdmin_" + new Random().Next(1, 1000);
                String DomainSystemAdminName = "DomainSysAdm_" + new Random().Next(1, 1000);
                String DatasourceAutoSSA = login.GetHostName(Config.EA77);
                String DatasourceVMSSA131 = login.GetHostName(Config.EA1);
                String DatasourceVMSSA91 = login.GetHostName(Config.EA91);
                String RefPhysicainData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReferringPhysician");
                String PatientIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ModalityData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String IPIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String InstitutionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                String AccessionNumberData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                String StudyPerformedData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedData");


                //Step 1
                //Create a Test Domain
                login.LoginIConnect(Username, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainName = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainAdminID = domainattr[DomainManagement.DomainAttr.UserID];
                String OriginaldomainAdminName = domainattr[DomainManagement.DomainAttr.LastName];
                String passwordB = domainattr[DomainManagement.DomainAttr.Password];
                String domainAdminRoleName = domainattr[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                /*domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomain();
                PageLoadWait.WaitForPageLoad(10);*/
                if (domainmanagement.SearchDomain(DomainName))
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

                // Step 2
                // Click on the User Management tab.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(10);
                bool GroupSideElements = usermanagement.NewGrpBtn().Displayed && usermanagement.NewSubGrpBtn().Displayed &&
                            usermanagement.EditGrpBtn().Displayed && usermanagement.DelGrpBtn().Displayed && usermanagement.MoveGrpBtn().Displayed &&
                            usermanagement.MoveUsrBtn().Displayed && usermanagement.DataMappingBtn().Displayed;
                bool UserSideElements = usermanagement.NewUsrBtn().Displayed && usermanagement.NewDomainAdminBtn().Displayed &&
                    usermanagement.NewSysAdminBtn().Displayed && usermanagement.ActivateUsrBtn().Displayed && usermanagement.DeactiveUsrBtn().Displayed &&
                    usermanagement.EditUsrBtn().Displayed && usermanagement.DelUsrBtn().Displayed && usermanagement.ViewUsrpBtn().Displayed;
                if (GroupSideElements && UserSideElements)
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


                // Step 3 
                // Select the Test Domain from the Domain drop down.
                usermanagement.SelectDomainFromDropdownList(DomainName);
                if (usermanagement.SearchGroup("Ungrouped", DomainName, 0))
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

                // Step 4 & 5 
                // Click on the New System Admin button.
                // Fill in all of the fields and click save.
                usermanagement.CreateSystemAdminUser(DomainSystemAdminName, DefaultDomain, 0, "", 0);
                ExecutedSteps++;

                if (usermanagement.SearchUser(DomainSystemAdminName, DefaultDomain))
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

                // Step 6
                // Search and Select the newly created System Admin.
                usermanagement.SelectDomainFromDropdownList(DefaultDomain);
                usermanagement.SearchUser(DomainSystemAdminName, DefaultDomain);
                usermanagement.SelectUser(DomainSystemAdminName);
                ExecutedSteps++;

                // Step 7
                // Click on the Edit button.
                usermanagement.EditUsrBtn().Click();
                SwitchToDefault();
                // Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForFrameLoad(10);
                String UserManagementEditPageBreadCumbs = Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text;
                String EditSystemAdminEditPageBreadCumbs = Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserTypeLabel")).Text;
                bool domaindropdown = usermanagement.DomainDropDownName().Enabled;

                if (UserManagementEditPageBreadCumbs.Equals("User Management") && EditSystemAdminEditPageBreadCumbs.Equals("Edit System Admin") && !domaindropdown)
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

                // Step 8
                // Modify the System Admin's first name along with other details and click the Save button.
                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", DomainSystemAdminName + " Changed");

                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", DomainSystemAdminName + " Changed");

                PageLoadWait.WaitForFrameLoad(20);
                //Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('#ctl00_MasterContentPlaceHolder_SaveButton').click()");

                //Get User details from User Management tab
                usermanagement.SearchUser(DomainSystemAdminName, DefaultDomain);
                IList<String> UsersList = usermanagement.ListedUsers().Select(d => d.Text).ToList();

                if (login.IsTabPresent("User Management") && UsersList[0].Contains(DomainSystemAdminName + " Changed")) //(user4, DomainC, Searchstring: "*");
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

                // Step 9
                // search and Select the System Admin that was created and click the Deactive button.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SearchUser(DomainSystemAdminName);
                usermanagement.SelectUser(DomainSystemAdminName);
                usermanagement.ClickButtonInUser("deactivate");
                PageLoadWait.WaitForPageLoad(20);
                usermanagement.SearchUser(DomainSystemAdminName);
                usermanagement.SelectUser(DomainSystemAdminName);
                if (usermanagement.VerifyUserDeactivated("Deactivated"))
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

                // Step 10 
                // Log out and try logging in the  account that is deactivated.
                login.Logout();
                login.LoginIConnect(DomainSystemAdminName, DomainSystemAdminName);
                PageLoadWait.WaitForPageLoad(10);

                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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

                // Step 11
                // Log back in using the username and password: Administrator/Administrator
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                // Step 12
                // Click on the User Management tab, select the deactivated System Admin and click the Activate button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SearchUser(DomainSystemAdminName);
                usermanagement.SelectUser(DomainSystemAdminName);
                usermanagement.ClickButtonInUser("activate");
                usermanagement.SearchUser(DomainSystemAdminName);
                usermanagement.SelectUser(DomainSystemAdminName);
                if (usermanagement.VerifyUserDeactivated("Activated"))
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

                // Step 13
                // Log out and try logging in the  account that is re-activated.
                login.Logout();
                PageLoadWait.WaitForPageLoad(10);
                login.LoginIConnect(DomainSystemAdminName, DomainSystemAdminName);

                if (login.IsTabPresent("User Management"))
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


                // Step 14
                // Click on the User Management tab, select the re-activated System Admin and click the Delete button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SearchUser(DomainSystemAdminName);
                usermanagement.SelectUser(DomainSystemAdminName);
                usermanagement.DelUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

                // Step 15
                // Click Ok in the Confirmation dialog.
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (usermanagement.DeleteUserErrorLabel().Text.Contains("You cannot delete your own user account."))
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

                // Step 16
                // Log out and log back in using the username and password: Administrator/Administrator
                login.Logout();
                login.LoginIConnect(Username, Password);
                if (login.IsTabPresent("User Management"))
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

                // Step 17
                // Click on the User Management tab, select the re-activated System Admin and click the Delete button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SearchUser(DomainSystemAdminName);
                usermanagement.SelectUser(DomainSystemAdminName);
                usermanagement.DelUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

                // Step 18
                // Click Ok in the Confirmation dialog.
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                PageLoadWait.WaitForFrameLoad(10);
                if (!usermanagement.SearchUser(DomainSystemAdminName, DefaultDomain))
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

                // Step 19 and Step 20
                // Click on the New Domain Admin button.
                if (!usermanagement.SearchUser(DomainAdminName, DomainName)) usermanagement.CreateDomainAdminUser(DomainAdminName, DomainName, 0, "", 0, "", 0);
                ExecutedSteps++;
                if (usermanagement.SearchUser(DomainAdminName, DomainName))
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

                // Step 21
                // Search and Select the newly created Domain Admin.
                usermanagement.SearchUser(DomainAdminName, DomainName);
                usermanagement.SelectUser(DomainAdminName);
                ExecutedSteps++;

                // Step 22 
                // Click on the Edit button.
                usermanagement.EditUsrBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList")));
                UserManagementEditPageBreadCumbs = Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text;
                Logger.Instance.InfoLog("Tab Name - " + UserManagementEditPageBreadCumbs);
                EditSystemAdminEditPageBreadCumbs = Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserTypeLabel")).Text;
                Logger.Instance.InfoLog("User Level - " + EditSystemAdminEditPageBreadCumbs);
                domaindropdown = Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList")).GetAttribute("disabled").Equals("true");
                Logger.Instance.InfoLog("Domain dropdown status - " + Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList")).GetAttribute("disabled"));

                if (UserManagementEditPageBreadCumbs.Equals("User Management") && EditSystemAdminEditPageBreadCumbs.Equals("Edit Domain Admin") && domaindropdown)
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

                // Step 23
                // Modify the Domain Admin's first name along with other details and click the Save button.
                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", DomainAdminName + " Changed");

                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", DomainAdminName + " Changed");

                PageLoadWait.WaitForFrameLoad(20);
                //Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('#ctl00_MasterContentPlaceHolder_SaveButton').click()");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Get User details from User Management tab
                usermanagement.SearchUser(DomainAdminName, DomainName);
                UsersList = usermanagement.ListedUsers().Select(d => d.Text).ToList();

                if (login.IsTabPresent("User Management") && UsersList[0].Contains(DomainAdminName + " Changed")) //(user4, DomainC, Searchstring: "*");
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

                // Step 24
                // Select the Domain Admin Account that was created and click the Deactive button.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.SearchUser(DomainAdminName, DomainName);
                usermanagement.SelectUser(DomainAdminName);
                usermanagement.ClickButtonInUser("deactivate");
                PageLoadWait.WaitForPageLoad(20);
                usermanagement.SearchUser(DomainAdminName);
                usermanagement.SelectUser(DomainAdminName);
                if (usermanagement.VerifyUserDeactivated("Deactivated"))
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

                // Step 25
                // Log out and try logging in the  account that is deactivated.
                login.Logout();
                login.LoginIConnect(DomainAdminName, DomainAdminName);
                PageLoadWait.WaitForPageLoad(10);

                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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

                // Step 26
                // Log back in using the username and password: Administrator/Administrator
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;

                // Step 27
                // Click on the User Management tab, search and select the deactivated Domain Admin and click the Activate button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.SelectDomainFromDropdownList(DomainName);
                usermanagement.SearchUser(DomainAdminName);
                usermanagement.SelectUser(DomainAdminName);
                usermanagement.ClickButtonInUser("activate");
                usermanagement.SearchUser(DomainAdminName);
                usermanagement.SelectUser(DomainAdminName);
                if (usermanagement.VerifyUserDeactivated("Activated"))
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

                // Step 28
                // Log out and try logging in the  account that is re-activated.
                login.Logout();
                login.LoginIConnect(DomainAdminName, DomainAdminName);

                if (login.IsTabPresent("User Management"))
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


                // Step 29
                // Click on the User Management tab, Verify the Domain Admin account is not listed.
                // Todo
                result.steps[++ExecutedSteps].status = "Not Automated";


                // Step 30
                // Log out and log back in using the username and password: Administrator/Administrator
                login.Logout();
                login.LoginIConnect(Username, Password);
                if (login.IsTabPresent("User Management"))
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

                // Step 31
                // Click on the User Management tab, select the re-activated Domain Admin and click the Delete button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.SelectDomainFromDropdownList(DomainName);
                usermanagement.SearchUser(DomainAdminName);
                usermanagement.SelectUser(DomainAdminName);
                usermanagement.DelUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

                // Step 32
                // Click Ok in the Confirmation dialog.
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (!usermanagement.SearchUser(DomainAdminName))
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

                // Step 33
                // Select the original Domain Admin for the domain and click the Delete button.                
                usermanagement.SearchUser(OriginaldomainAdminName);
                usermanagement.SelectUser(OriginaldomainAdminName);
                usermanagement.DelUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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



                // Step 34
                // Click Ok in the Confirmation dialog.
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                PageLoadWait.WaitForPageLoad(20);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if (usermanagement.DeleteUserErrorLabel().Text.Contains("The selected user is the last Domain Admin user and cannot be deleted."))
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

                //Logout iCA
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test2_29294(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            Studies studies = null;
            StudyViewer viewer = null;
            WpfObjects wpfobject = null;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                       
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);
                ServiceTool tool = null;
                // string[] Datasource = { "AUTO-SSA-001", "VMSSA-4-38-131", "VMSSA-5-38-91" };
                String DefaultDomain = "SuperAdminGroup";
                String DomainAdminName = "Domain_" + new Random().Next(1, 1000);
                String DomainSystemAdminName = "DomainSystemAdmin_" + new Random().Next(1, 1000);
                String DatasourceAutoSSA = login.GetHostName(Config.EA77);
                String DatasourceVMSSA131 = login.GetHostName(Config.EA1);
                String DatasourceVMSSA91 = login.GetHostName(Config.EA91);
                String PatientIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ModalityData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String IPIDData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
                String InstitutionData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Institution");
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String roleB1 = "TestRole1_29292_" + random.Next(1, limit);
                String roleC1 = "TestRole2_29292_" + random.Next(1, limit);
                String user1 = "user1_" + random.Next(1, limit);
                String user2 = "user2_" + random.Next(1, limit);
                String user3 = "user3_" + random.Next(1, limit);
                String user4 = "user4_" + random.Next(1, limit);
                String Firstname = "Bob" + random.Next(1, limit);
                String Lastname = "Marley" + random.Next(1, limit);

                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainAdminID = domainattr[DomainManagement.DomainAttr.UserID];
                String passwordB = domainattr[DomainManagement.DomainAttr.Password];
                String domainAdminName = domainattr[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DomainB, roleB1, "Conference=Physician");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                login.Logout();



                //Step 1 
                //Click on the New User button.
                login.LoginIConnect(adminUserName, adminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.ClickButtonInUser("new");
                SwitchToDefault();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                bool newUserDiv = usermanagement.NewUserDiv().Displayed;
                string newUserLabel = usermanagement.NewUserLabel();
                usermanagement.XBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (newUserDiv == true && newUserLabel.Equals("New User"))
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

                // Step 2
                //Fill in all of the fields and click save.   
                usermanagement.CreateUser(user1, DomainB, roleB1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                IWebElement tabSelected = BasePage.Driver.FindElement(By.CssSelector("td[id^=TabMid] div[class='TabText TabSelected']"));
                string currenttab = tabSelected.GetAttribute("innerHTML");
                if (currenttab.Equals("User Management"))
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


                // Step 3 
                //Search and Select the newly created User.
                bool buser1 = usermanagement.SearchUser(user1, DomainB);
                usermanagement.SelectUser(user1);
                IWebElement userSelected = BasePage.Driver.FindElement(By.CssSelector("tr.itemListHighlight"));
                if (userSelected.Text.Contains(user1))
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

                // Step 4 
                // Click on the Edit button.       
                usermanagement.ClickButtonInUser("edit");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (usermanagement.UserManagementLbl().Equals("User Management") && usermanagement.EditUserLbl().Equals("Edit User"))
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

                // Step 5
                // Modify the User's first name along with other details and click the Save button.
                usermanagement.EditUser(firstname: Firstname, lastname: Lastname);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("td[id^=TabMid] div[class='TabText TabSelected']")));
                IWebElement tabSelected5 = BasePage.Driver.FindElement(By.CssSelector("td[id^=TabMid] div[class='TabText TabSelected']"));
                string currenttab5 = tabSelected5.GetAttribute("innerHTML");
                bool step5_1 = usermanagement.SearchUser(Firstname, DomainB);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step5_2 = usermanagement.SearchUser(Lastname, DomainB);
                if (currenttab5.Equals("User Management") && step5_1 && step5_2)
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

                //Step 6: Select the User that was created and click the Deactive button.
                usermanagement.SearchUser(Firstname, DomainB);
                usermanagement.SelectUser(Firstname);
                usermanagement.ClickButtonInUser("deactivate");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.SearchUser(Firstname, DomainB);
                if (usermanagement.VerifyUserDeactivated("Deactivated"))
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

                //step 7: Log out and try logging in the  account that is deactivated.
                login.Logout();
                login.LoginIConnect(user1, user1);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[id='ctl00_LoginMasterContentPlaceHolder_ErrorMessage']")));
                if (login.LoginErrorMsgLabel().Text.Equals("The system cannot log you in. Please try again.:"))
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


                //Step 8: Log back in using the username and password- Administrator/Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("User Management"))
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

                //Step 9: Click on the User Management tab, select the deactivated User and click the Activate button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser(Firstname, DomainB);
                usermanagement.SelectUser(Firstname);
                usermanagement.ClickButtonInUser("activate");
                usermanagement.SearchUser(Firstname, DomainB);
                if (usermanagement.VerifyUserDeactivated("Activated"))
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


                //Step 10: Log out and try logging in the  account that is re-activated.
                login.Logout();
                login.LoginIConnect(user1, user1);
                if (login.IsTabPresent("Studies"))
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

                //Step 11: Log out and log back in using the username and password- Administrator/Administrator
                login.Logout();
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("User Management"))
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

                //Step 12: Click on the User Management tab, select the re-activated User and click the Delete button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SearchUser(Firstname, DomainB);
                usermanagement.SelectUser(Firstname);
                usermanagement.ClickButtonInUser("delete");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogDiv #ConfirmationDiv")).Displayed)
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

                //Step 13: Click Ok in the Confirmation dialog.               
                usermanagement.OkButtonConfirmGroupDeletionMsgBox().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step13 = usermanagement.SearchUser(Firstname, DomainB);
                if (step13 == false)
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

                //Step 14: Create additional users for the Test Domain.
                usermanagement.CreateUser(user2, DomainB, roleB1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step14_1 = usermanagement.SearchUser(user2, DomainB);
                usermanagement.CreateUser(user3, DomainB, roleB1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step14_2 = usermanagement.SearchUser(user3, DomainB);
                if (step14_1 && step14_2)
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


                //Step 15: On the User Management page select the Test Domain from the Domain drop down. 
                //Perform * search in Users
                bool step15_1 = usermanagement.SearchUser(user2, DomainB, Searchstring: "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step15_2 = usermanagement.SearchUser(user3, DomainB, Searchstring: "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (step15_1 && step15_2)
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


                //Step 16: In the Filter Users field enter part of the User's  name that was 
                //created and click on the search button.
                bool step16_1 = usermanagement.SearchUser(user2, DomainB, Searchstring: "user");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step16_2 = usermanagement.SearchUser(user3, DomainB, Searchstring: "user");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                String userno = usermanagement.NoOfUserLbl();
                String[] number = userno.Split(' ');
                if (step16_1 && step16_2 && number[0] == "2")
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


                //PreCondition
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                var domainattr1 = domainmanagement.CreateDomainAttr();
                String DomainC = domainattr1[DomainManagement.DomainAttr.DomainName];
                String domainAdminID1 = domainattr1[DomainManagement.DomainAttr.UserID];
                String passwordB1 = domainattr1[DomainManagement.DomainAttr.Password];
                String domainAdminName1 = domainattr1[DomainManagement.DomainAttr.RoleName];
                domainmanagement.CreateDomain(domainattr1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DomainC, roleC1, "Conference=Physician");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-17: From User Management, choose the second domain Click on the New User button.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                usermanagement.ClickButtonInUser("new");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool newUserDiv17 = usermanagement.NewUserDiv().Displayed;
                string newUserLabel17 = usermanagement.NewUserLabel();
                usermanagement.XBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (newUserDiv17 == true && newUserLabel17.Equals("New User"))
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

                // Step 18 & 19
                //Select use an existing role, and select from the list.
                //Fill in all of the fields and click save.   
                usermanagement.CreateUser(user4, DomainC, roleC1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                ExecutedSteps++;
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo();
                IWebElement tabSelected18 = BasePage.Driver.FindElement(By.CssSelector("td[id^=TabMid] div[class='TabText TabSelected']"));
                string currenttab18 = tabSelected18.GetAttribute("innerHTML");
                if (currenttab18.Equals("User Management"))
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


                // Step-20: Search and select new user.
                bool step20 = usermanagement.SearchUser(user4, DomainC);
                usermanagement.SelectUser(user4);
                if (step20)
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

                //Step-21: Logout of System Administrator account.
                login.Logout();
                ExecutedSteps++;

                //Step-22: Login as the Domain Administrator for second domain.
                login.LoginIConnect(domainAdminID1, passwordB1);
                if (login.IsTabPresent("User Management"))
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


                //Step-23: Select User Management. Perform * search in users.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step23 = usermanagement.SearchUser(user4, DomainC, Searchstring: "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (step23)
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

                //Step-24: Logout of Domain Administrator account.
                login.Logout();
                ExecutedSteps++;

                //Step-25: 
                login.LoginIConnect(domainAdminID, passwordB);
                if (login.IsTabPresent("User Management"))
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


                //Step-26: Select User Management, and show all users checkbox. Perform * search in users.
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step26_1 = usermanagement.SearchUser(user2, DomainB, Searchstring: "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool step26_2 = usermanagement.SearchUser(user3, DomainB, Searchstring: "*");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                if (step26_1 && step26_2)
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Cleanup script to close browser
        /// </summary>
        /// 
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }



    }
}


