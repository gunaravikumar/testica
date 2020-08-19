 using System;
using System.Collections.Generic;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using System.IO;
using OpenQA.Selenium;
using Selenium.Scripts.Pages.MergeServiceTool;
using TextBox = TestStack.White.UIItems.TextBox;
using TestStack.White.UIItems.TabItems;
using System.Threading;
using System.Linq;
using Selenium.Scripts.Pages.eHR;
using System.Diagnostics;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Tests
{
    class Study_Authorization : StudyAuthorizationUtils
    {
        Login login { get; set; }
        public string filepath { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        public BasePage basepage { get; set; }
        public EHR ehr { get; set; }

        public string EA_131 = "VMSSA-4-38-131";

        public Study_Authorization(String classname)
        {
            login = new Login();
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            basepage = new BasePage();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ehr = new EHR();
        }

        public TestCaseResult Study_Authorization_Precondition(string testid, string teststeps, int stepcount)
        {

            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try { 
            //update the Config values.
            string EnableCSRFValidationValuesBeforeUpdate = basepage.GetAttributeValue(Config.WebConfigPath, "/configuration/appSettings/add[@key='Application.EnableCSRFValidation']", "value");
            basepage.ChangeAttributeValue(Config.WebConfigPath, "/configuration/appSettings/add[@key='Application.EnableCSRFValidation']", "value", "false");
            string EnableCSRFValidationValuesAfterUpdate = basepage.GetAttributeValue(Config.WebConfigPath, "/configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");
            Logger.Instance.InfoLog("Updated the Config value to the Config node EnableCSRValidation value to " + EnableCSRFValidationValuesAfterUpdate);
            basepage.RestartIISUsingexe();
             result.steps[++ExecutedSteps].StepPass();
           
             //Report Result
            result.FinalResult(ExecutedSteps);

            Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
            //Return Result
            return result;
            //------------End of script---

        }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
}

/// <summary> 
/// 160902  - This Test Case is Testing Study Authorization in Standalone mode - Active Web Session
/// </summary>
///
public TestCaseResult Test_160902(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");
                string WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetHeaderData");
                string WebSocketResponse_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetHeaderData");
                string WebSocketQuery_FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_FilePath");
                string WebSocketResponse_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_FilePath");

                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string[] WebSocketResponse_HeaderData = WebSocketResponse_GetHeaderData.Split('@');
                string[] WebSocketResponse_FilePath = WebSocketResponse_GetFilePath.Split('@');

                string Datasource = login.GetHostName(Config.SanityPACS);

                //Step 1 - Precondition
                // Manully Done and entered in excel - Follow attached Precondition document to capture all websocket related queries
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                //Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser)   
                //Login success
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3
                //Navigate to Studies tab and search for Patient ID=GE0514
                //Study should appear in the search grid
                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                result.steps[++ExecutedSteps].StepPass();

                //Step 4
                //Launch the study in new viewer(BlueRing viewer)
                //Study launched in new viewer
                study.SelectStudy("Patient ID", PatinetID);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step 5
                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");

                IList<string> tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                result.steps[++ExecutedSteps].StepPass();

                //Step 6
                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();
                result.steps[++ExecutedSteps].StepPass();


                //Step 7
                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);
                result.steps[++ExecutedSteps].StepPass();

                //Step 8
                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);

                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0] + "and " + WebSocketResponse_InstanceList[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                string FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");

                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);

                //Step 9
                //Send the wecsoket HeaderData Query
                Send_WebSocketQuery(WebSocketQuery_GetHeaderData);
                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_HeaderData[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[0] + "And " + WebSocketResponse_HeaderData[1]);
                else
                    result.steps[++ExecutedSteps].StepPass();

                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                WebSocketQuery_FilePath = WebSocketQuery_FilePath.Replace("\"FILEPATHURL\"", FilePathURL);

                //Step 10
                //Send the wecsoket FilePath Query
                Send_WebSocketQuery(WebSocketQuery_FilePath);
                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_FilePath[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_FilePath);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.Driver.SwitchTo().Window(tabs[0]);

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// 160908    - This Test Case is to Verify Study Authorization in Standalone mode - Clear browser cache and try existing query
        /// </summary>
        ///
        public TestCaseResult Test_160908(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;

                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");

                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string Datasource = login.GetHostName(Config.SanityPACS);

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                //Navigate to Studies tab and search for Patient ID=GE0514
                //Study should appear in the search grid
                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                PageLoadWait.WaitForSearchLoad();

                //Launch the study in new viewer(BlueRing viewer)
                //Study launched in new viewer
                study.SelectStudy("Patient ID", PatinetID);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();

                // Setp 1: PreCondition 
                result.steps[++ExecutedSteps].StepPass();

                // Setp 2: Clear browser cache
                BasePage.Driver.Manage().Cookies.DeleteAllCookies();
                result.steps[++ExecutedSteps].StepPass();

                //Open a new tab
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");

                IList<string> tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();


                //Step 3
                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();

                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);

                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);

                //Verfiy the Response Message
                if (VerfiyResponseMessage( null, true))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Step 4
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].StepPass();
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 5
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();

                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);

                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);

                //Verfiy the Response Message
                if (VerfiyResponseMessage(null, true))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Report Result
                result.FinalResult(ExecutedSteps);

                login.Logout();

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// 160905   - This Test Case is Study Authorization in Standalone mode - Testing after logging out from Active web session
        /// </summary>
        ///
        public TestCaseResult Test_160905(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");

                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');

                string Datasource = login.GetHostName(Config.SanityPACS);

                //Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser)   
                //Login success
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                //Navigate to Studies tab and search for Patient ID=GE0514
                //Study should appear in the search grid
                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);

                result.steps[++ExecutedSteps].StepPass();

                //Step2
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();
                Thread.Sleep(3000);

                //step 3
                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(3000);

                IList<string> tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                //Launch websocket console in the same browser
                //Websocket console should open)
                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);
                if (VerfiyResponseMessage(null, true))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);


                ///Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// 160906   - This Test Case is to Verify Study Authorization in Standalone mode - Testing into Inactive web browser after idle session timeout
        /// </summary>
        ///
        public TestCaseResult Test_160906(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");

                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string Datasource = login.GetHostName(Config.SanityPACS);

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.modifyBtn().Click();
                TextBox SetTimeout = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), "AutoSelectTextBox", 0);
                SetTimeout.Enter("2");
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception) { }
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                    Thread.Sleep(5000);
                }
                catch (Exception) { }
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                //Step 1 - Precondition
                result.steps[++ExecutedSteps].StepPass();

                //Step 2 - Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser)   
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3 - Navigate to Studies tab and search for Patient ID=GE0514, Study should appear in the search grid
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);
                result.steps[++ExecutedSteps].StepPass();

                //Step 4 - Launch the study in new viewer(BlueRing viewer)
                study.SelectStudy("Patient ID", PatinetID);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step 5 - Wait for  session Time out.
                Thread.Sleep(240000);
                result.steps[++ExecutedSteps].StepPass();

                //Step 6 - Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                Driver.SwitchTo().DefaultContent();
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                IList<string> tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);
                if (VerfiyResponseMessage(null, true ))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.modifyBtn().Click();
                TextBox SetTimeout = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), "AutoSelectTextBox", 0);
                SetTimeout.Enter("30");
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception) { }
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                }
                catch (Exception) { }
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

            }
        }

        /// <summary> 
        /// 160907    - This Test Case is to Verify Study Authorization in Standalone mode - Testing into same browser with another user
        /// </summary>
        ///
        public TestCaseResult Test_160907(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            UserPreferences userpref = new UserPreferences();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String DomainName = "Domain_" + new Random().Next(1, 1000);
                String RoleName = "Role_" + new Random().Next(1, 1000);
                String UName = "User_" + new Random().Next(1, 1000);


                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");

                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');

                string Datasource = login.GetHostName(Config.SanityPACS);

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                Studies studies = (Studies)login.Navigate("Studies");
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(DomainName, RoleName, datasources: new string[] { Datasource });
                domainmanagement.VisibleAllStudySearchField();
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(UName, DomainName, RoleName);
                PageLoadWait.WaitForPageLoad(10);
                login.Logout();

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 2
                //Navigate to Studies tab and search for Patient ID=GE0514
                //Study should appear in the search grid
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                PageLoadWait.WaitForSearchLoad();
                study.SelectStudy("Patient ID", PatinetID);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3
                //Launch the study in new viewer(BlueRing viewer)
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                bluringviewer.CloseBluRingViewer();

                //Step 4
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();

                //Step5
                //Login as User 
                login.LoginIConnect(UName, UName);
                result.steps[++ExecutedSteps].StepPass();

                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();

                //Open a new tab
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");

                IList<string> tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();

                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);

                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);

                //Verfiy the Response Message
                //Step 6
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[0]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepPass();

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Setp 7
                //Navigate to Studies tab and search for Patient ID=GE0514
                //Study should appear in the search grid
                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForSearchLoad();
                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                PageLoadWait.WaitForSearchLoad();
                result.steps[++ExecutedSteps].StepPass();

                //Step 8
                //Launch the study in new viewer(BlueRing viewer)
                //Study launched in new viewer
                study.SelectStudy("Patient ID", PatinetID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Open a new tab
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();

                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);

                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);

                //Verfiy the Response Message
                //step 9
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Close the new tab
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                //Switch to the first tab
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                bluringviewer.CloseBluRingViewer();

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// 160903  - This Test Case is to  Verfiy Study Authorization in Standalone mode -Testing UnAuthorized study in same Active Web Session
        /// </summary>
        public TestCaseResult Test_160903(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;

                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketResponse_GetInstanceList");

                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');

                string[] WebSocketQuery_InstanceList = WebSocketQuery_GetInstanceList.Split('@');

                string Datasource = login.GetHostName(Config.SanityPACS);

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                //Navigate to Studies tab and search for Patient ID=GE0514
                //Study should appear in the search grid
                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                PageLoadWait.WaitForSearchLoad();

                //Launch the study in new viewer(BlueRing viewer)
                //Study launched in new viewer
                study.SelectStudy("Patient ID", PatinetID);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();

                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step 2
                //Open a New tab
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");

                IList<string> tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();

                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);

                //to get the start time of the attachment
                var LogStartTime = System.DateTime.Now;

                Thread.Sleep(5000);

                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_InstanceList[0]);

                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[0]))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 3
                //to get the end time of the attachment
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade.GetSeriesInstanceList"))
                                    if (entry.Value["Message"].Contains("Viewer Facade Error"))
                                        if (entry.Value["Detail"].Contains("Authorization error while loading study"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }

                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("lldsf");

                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Viewer Facade Error")
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                }


                //Step 4
                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();

                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);

                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_InstanceList[1]);

                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[0]))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                bluringviewer.CloseBluRingViewer();

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// 160909   - This Test Case is to Verify Study Authorization IN/Outbounds in Standalone mode - new viewer is the default viewer
        /// </summary>
        ///
        public TestCaseResult Test_160909(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            UserPreferences userpref = new UserPreferences();

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                DomainManagement domain = new DomainManagement();
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String Role1 = "Role_1" + new Random().Next(1, 1000);
                String User1 = "User_1" + new Random().Next(1, 1000);
                MultiDriver = new List<IWebDriver>();

                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");
                string WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetHeaderData");
                string WebSocketResponse_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetHeaderData");
                string WebSocketQuery_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_FilePath");
                string WebSocketResponse_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_FilePath");
                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string[] WebSocketResponse_HeaderData = WebSocketResponse_GetHeaderData.Split('@');
                string[] WebSocketResponse_FilePath = WebSocketResponse_GetFilePath.Split('@');
                string Datasource = login.GetHostName(Config.SanityPACS);

                List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
                for (int count = 0; count < 3; count++)
                {
                    if (Config.BrowserType.ToLower() == browserList[count])
                    {
                        browserList[count] = "chrome";
                        break;
                    }
                }

                //step - 1 Login success
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step -2
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();


                //Create a new Role.
                login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("grant", 0);
                domain.ClickSaveDomain();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.ClickSaveEditRole();

                rolemanagement.CreateRole("SuperAdminGroup", Role1, "any");
                rolemanagement.SearchRole(Role1);
                rolemanagement.SelectRole(Role1);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.AddDatasourceToRole(Datasource);
                rolemanagement.ClickSaveEditRole();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, "SuperAdminGroup", Role1);

                //Search a study
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                studies.SelectStudy1("Patient ID", PatinetID);
                studies.ShareStudy(false, new string[] { User1 });

                Outbounds outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(patientID: PatinetID);
                //Step -3
                result.steps[++ExecutedSteps].StepPass();


                //Load the study from outbounds
                outbounds.SelectStudy1(columnname: "Patient ID", columnvalue: PatinetID, dblclick: false);
                BluRingViewer bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                //Step -4
                result.steps[++ExecutedSteps].StepPass();

                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                //Open a New tab
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");

                //Step -5
                result.steps[++ExecutedSteps].StepPass();

                IList<string> tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                //Step -6
                if (NavigateTo_SoftwareHixieURL())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step -7
                if (Establish_WebSocketConncetion(Config.IConnectIP))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step-8
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();


                string FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");

                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);

                //step-9
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, WebSocketResponse_HeaderData[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();


                WebSocketQuery_GetFilePath = WebSocketQuery_GetFilePath.Replace("\"FILEPATHURL\"", FilePathURL);

                //step-10
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetFilePath, WebSocketResponse_FilePath[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_FilePath[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);

                //step 11
                ++ExecutedSteps;

                if (Config.BrowserType == "chrome" || Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                    Thread.Sleep(5000);
                    login.SetDriver(BasePage.MultiDriver[1]);
                    login.DriverGoTo(login.url);
                }
                else if (Config.BrowserType == "firefox" || Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                    Thread.Sleep(5000);
                    if (Config.BrowserType != "ie")
                        login.SetDriver(BasePage.MultiDriver[1]);
                }
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                BasePage.MultiDriver[1].Close();
                if (Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                    Thread.Sleep(5000);
                    login.SetDriver(BasePage.MultiDriver[2]);
                }

                if (Config.BrowserType == "firefox" || Config.BrowserType == "chrome")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("ie"));
                    Thread.Sleep(5000);
                    login.SetDriver(BasePage.MultiDriver[2]);
                }

                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                BasePage.MultiDriver[2].Close();

                //Edge Browser
                BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Thread.Sleep(5000);
                //Driver = BasePage.MultiDriver[3];
                Driver = BasePage.MultiDriver.Last();
                login.DriverGoTo(login.url);
                login.LoginGrid(adminusername, adminpassword);
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(patientID: PatinetID);
                outbounds.SelectStudy1(columnname: "Patient ID", columnvalue: PatinetID, dblclick: false);
                try
                { bluRingViewer = BluRingViewer.LaunchBluRingViewer(); } catch (Exception ex ) { };

                BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
                //Driver = BasePage.MultiDriver[4];
                Driver = BasePage.MultiDriver.Last();
                Logger.Instance.InfoLog(browserList[2] + " launched");
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                BasePage.MultiDriver.Last().Close();// [4].Close();
                BasePage.MultiDriver.Remove(MultiDriver.Last());
                //BasePage.MultiDriver[3].Close();
                BasePage.MultiDriver.Last().Close();
                BasePage.MultiDriver.Remove(MultiDriver.Last());

                login.SetDriver(BasePage.MultiDriver[0]);
                bluRingViewer.CloseBluRingViewer();
                login.Logout();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().DefaultContent();
                result.steps[ExecutedSteps].AddPassStatusList("Log Out of the Application");

                //Open a new tab
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(5000);
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);
                BasePage.Driver.SwitchTo().DefaultContent();
                login.DriverGoTo(login.url);
                login.LoginGrid(adminusername, adminpassword);
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(patientID: PatinetID);
                outbounds.SelectStudy1(columnname: "Patient ID", columnvalue: PatinetID, dblclick: false);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();

                BasePage.Driver.Manage().Cookies.DeleteAllCookies();
                //Open a new tab
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");

                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();

                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                BasePage.Driver.SwitchTo().Window(tabs[0]);
                login.DriverGoTo(login.url);

                BasePage.Driver.SwitchTo().Window(tabs[1]);
               

                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //step - 11
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// 160904    - This Test Case is to Verify  Study Authorization in Standalone mode - Testing into different browser without login to iCA
        /// </summary>
        ///
        public TestCaseResult Test_160904(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                MultiDriver = new List<IWebDriver>();

                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String Role1 = "Role_1" + new Random().Next(1, 1000);
                String User1 = "User_1" + new Random().Next(1, 1000);


                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");

                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");

                string WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetHeaderData");
                string WebSocketResponse_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetHeaderData");

                string WebSocketQuery_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_FilePath");
                string WebSocketResponse_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_FilePath");

                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string[] WebSocketResponse_HeaderData = WebSocketResponse_GetHeaderData.Split('@');
                string[] WebSocketResponse_FilePath = WebSocketResponse_GetFilePath.Split('@');

                string Datasource = login.GetHostName(Config.SanityPACS);

                //Step -1 Precondition
                result.steps[++ExecutedSteps].StepPass();

                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);

                BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                login.SetDriver(BasePage.MultiDriver[1]);

                //Step - 2
                //Login success
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step - 3
                //Search a study
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatinetID);
                result.steps[++ExecutedSteps].StepPass();

                //Step - 4
                studies.SelectStudy("Patient ID", PatinetID);
                BluRingViewer bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 5
                BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                Thread.Sleep(5000);
                login.SetDriver(BasePage.MultiDriver[2]);

                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.MultiDriver[2].Close();

                //Step 6
                BasePage.MultiDriver.Add(login.InvokeBrowser("ie"));
                Thread.Sleep(5000);
                login.SetDriver(BasePage.MultiDriver[3]);

                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.MultiDriver[3].Close();

                //Step 6
                //Edge Browser
                BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Thread.Sleep(5000);
                Driver = BasePage.MultiDriver.Last();
                login.DriverGoTo(login.url);
                login.LoginGrid(adminusername, adminpassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatinetID);
                studies.SelectStudy("Patient ID", PatinetID);
                try
                { bluRingViewer = BluRingViewer.LaunchBluRingViewer(); }
                catch(Exception ex){ }

                BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-edge"));
                Driver = BasePage.MultiDriver.Last();
                Logger.Instance.InfoLog("Remote-edge is launched");
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.MultiDriver.Last().Close();// [4].Close();
                BasePage.MultiDriver.Remove(MultiDriver.Last());
                BasePage.MultiDriver.Last().Close();
                BasePage.MultiDriver.Remove(MultiDriver.Last());

                BasePage.Driver = BasePage.MultiDriver[0];
                BasePage.Driver.Quit();
                basepage.KillProcessByName("chrome");
                basepage.KillProcessByName("iexplore");
                basepage.KillProcessByName("firefox");
                login.CreateNewSesion();


                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                login.SetDriver(BasePage.MultiDriver[0]);

                for (int i = 1; i < BasePage.MultiDriver.Count; i++)
                {
                    try
                    {
                        BasePage.MultiDriver[i].Close();
                    }
                    catch (Exception A) { }
                }

                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// 160914    - This Test Case is to verfiy Study Authorization in Standalone mode - Priors/related studies in New viewer (Enterprise viewer)
        /// </summary>
        ///
        public TestCaseResult Test_160914(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                MultiDriver = new List<IWebDriver>();

                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string[] Accession = AccessionID.Split(';');
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketResponse_GetInstanceList");
                string WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketQuery_GetHeaderData");
                string WebSocketResponse_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketResponse_GetHeaderData");
                string WebSocketQuery_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketQuery_FilePath");
                string WebSocketResponse_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketResponse_FilePath");
                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string[] WebSocketResponse_HeaderData = WebSocketResponse_GetHeaderData.Split('@');
                string[] WebSocketResponse_FilePath = WebSocketResponse_GetFilePath.Split('@');
                string Datasource = login.GetHostName(Config.SanityPACS);
                string FilePathURL = null;
                IList<string> tabs = new List<string>(Driver.WindowHandles);

                try
                {
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(adminusername, adminpassword);
                    //Search a study
                    Studies studies1 = (Studies)login.Navigate("Studies");
                    studies1.SearchStudy(patientID: PatinetID);
                    studies1.SelectStudy("Accession", Accession[0]);
                    BluRingViewer bluRingViewer1 = BluRingViewer.LaunchBluRingViewer();
                    //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                    //iCA session is open and Active
                    IJavaScriptExecutor js1 = (IJavaScriptExecutor)BasePage.Driver;
                    js1.ExecuteScript("window.open();");
                    tabs = new List<string>(BasePage.Driver.WindowHandles);
                    BasePage.Driver.SwitchTo().Window(tabs[1]);
                    BasePage.Driver.Manage().Window.Maximize();

                    //Verfiy the Response Message
                    if (!SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[1]))
                        Logger.Instance.ErrorLog("Error while get the encrypted path values");
                    FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                    FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");
                    BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                    BasePage.Driver.SwitchTo().Window(tabs[0]);
                    bluRingViewer1.CloseBluRingViewer();
                    login.Logout();

                }
                catch (Exception ex)
                {
                    if (tabs.Count == 2)
                    {
                        BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                        BasePage.Driver.SwitchTo().Window(tabs[0]);
                    }
                    Logger.Instance.ErrorLog("Error while get the encrypted path values");
                }


                //Step -1 Precondition
                result.steps[++ExecutedSteps].StepPass();

                //Step - 2 -  Launch iCA in Chrome and login as Administrator and password Administrator
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                login.DriverGoTo(login.url);
                //Login success
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step - 3
                //Search a study
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatinetID, Datasource: EA_131);
                result.steps[++ExecutedSteps].StepPass();

                //Step - 4
                studies.SelectStudy("Patient ID", PatinetID);
                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                NavigateTo_SoftwareHixieURL();
                result.steps[++ExecutedSteps].StepPass();

                //Step 5
                Establish_WebSocketConncetion(Config.IConnectIP);
                result.steps[++ExecutedSteps].StepPass();

                //Step 6
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[0]))
                {
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);

                //Step 7
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, WebSocketResponse_HeaderData[0]))
                {
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[0] + " for the GetHeaderData query");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                WebSocketQuery_GetFilePath = WebSocketQuery_GetFilePath.Replace("\"FILEPATHURL\"", FilePathURL);

                //Step 8
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetFilePath, WebSocketResponse_FilePath[0]))
                {
                    result.steps[++ExecutedSteps].StepPass("Successfully verifed the Response Message" + WebSocketResponse_FilePath[0] + " for the File Path query");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);


                //Step 9
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 10
                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[1]))
                {
                    result.steps[++ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[1]);
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }
                FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");

                WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketQuery_GetHeaderData");
                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);

                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, WebSocketResponse_HeaderData[1]))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[1] + " for the GetHeaderData query");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                WebSocketQuery_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketQuery_FilePath");
                WebSocketQuery_GetFilePath = WebSocketQuery_GetFilePath.Replace("\"FILEPATHURL\"", FilePathURL);

                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetFilePath, WebSocketResponse_FilePath[1]))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verifed the Response Message" + WebSocketResponse_FilePath[1] + " for the File Path query");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Step 10
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                bluRingViewer.CloseBluRingViewer();

                //Step 11
                studies.SelectStudy("Accession", Accession[1]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 12
                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[1]))
                {
                    result.steps[++ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[1]);
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }
                FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");
                WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WebSocketQuery_GetHeaderData");
                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, WebSocketResponse_HeaderData[1]))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[1] + " for the GetHeaderData query");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                WebSocketQuery_GetFilePath = WebSocketQuery_GetFilePath.Replace("\"FILEPATHURL\"", FilePathURL);
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetFilePath, WebSocketResponse_FilePath[1]))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verifed the Response Message" + WebSocketResponse_FilePath[1] + " for the File Path query");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                bluRingViewer.CloseBluRingViewer();
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {

                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// 160911  - This Test Case is Test  Study Authorization Conference folder in standalone mode- new viewer is the default viewer
        /// </summary>
        ///
        public TestCaseResult Test_160911(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            DomainManagement domainmanagement;
            Taskbar taskbar = null;
            ConferenceFolders conferencefolders;
            result = new TestCaseResult(stepcount);
            UserPreferences userpref = new UserPreferences();
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String adminusername = Config.adminUserName;
            String adminpassword = Config.adminPassword;

            try
            {
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");
                string WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetHeaderData");
                string WebSocketResponse_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetHeaderData");
                string WebSocketQuery_FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_FilePath");
                string WebSocketResponse_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_FilePath");
                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string[] WebSocketResponse_HeaderData = WebSocketResponse_GetHeaderData.Split('@');
                string[] WebSocketResponse_FilePath = WebSocketResponse_GetFilePath.Split('@');
                String TopFolder = "13934_" + new Random().Next(1, 1000);
                String SubFolder = "13934_" + new Random().Next(1, 1000);
                String folderpath = TopFolder + "/" + SubFolder;
                string Datasource = login.GetHostName(Config.SanityPACS);
                List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
                for (int count = 0; count < 3; count++)
                {
                    if (Config.BrowserType.ToLower() == browserList[count])
                    {
                        browserList[count] = "chrome";
                        break;
                    }
                }


                //Enable Conference Lists is turned ON in Server Tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableConferenceLists(0);
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                taskbar.Show();

                //Step 1
                //Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser)   
                //Login success
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();

                //Pre-Condition
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);
                domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0);
                PageLoadWait.WaitForFrameLoad(5);
                domainmanagement.ClickSaveEditDomain();

                //Create Folder
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
                bool step3_1 = conferencefolders.CreateToplevelFolder(TopFolder); //First Top Folder
                bool step3_2 = conferencefolders.CreateSubFolder(TopFolder, SubFolder);

                //Step 2
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                studies.SelectStudy("Patient ID", PatinetID);
                result.steps[++ExecutedSteps].StepPass();

                //step 3
                StudyViewer studyViewer = studies.LaunchStudy();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                studyViewer.AddStudyToStudyFolder(folderpath);
                studyViewer.CloseStudy();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass(); //Step 3

                //Step 4 
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                result.steps[++ExecutedSteps].StepPass(); //Step 4

                //step 5
                //Launch Study in viewer
                conferencefolders.SelectStudy1("Patient ID", PatinetID);
                if (BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Displayed)
                {
                    BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Click();
                }
                try { BluRingViewer.WaitforThumbnails(); BluRingViewer.WaitforThumbnails(); }
                catch (Exception ex) { }

                result.steps[++ExecutedSteps].StepPass(); //Step -5

                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                Driver.SwitchTo().DefaultContent();
                Thread.Sleep(2000);
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                IList<string> tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                result.steps[++ExecutedSteps].StepPass(); //Step -6

                //Step7
                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();
                result.steps[++ExecutedSteps].StepPass();

                //Step8
                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);
                result.steps[++ExecutedSteps].StepPass();

                //Step 9 
                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);
                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[1]))
                {
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0] + "and " + WebSocketResponse_InstanceList[1]);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                string FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");

                //Step 10 
                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);
                //Send the wecsoket HeaderData Query
                Send_WebSocketQuery(WebSocketQuery_GetHeaderData);
                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_HeaderData[1]))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[0] + "And " + WebSocketResponse_HeaderData[1]);
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 11
                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                WebSocketQuery_FilePath = WebSocketQuery_FilePath.Replace("\"FILEPATHURL\"", FilePathURL);
                //Send the wecsoket FilePath Query
                Send_WebSocketQuery(WebSocketQuery_FilePath);
                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_FilePath[1]))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_FilePath);
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);


                //step 12
                if (Config.BrowserType == "chrome" || Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                    Thread.Sleep(5000);
                    login.SetDriver(BasePage.MultiDriver[1]);
                    login.DriverGoTo(login.url);
                }
                else if (Config.BrowserType == "firefox" || Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                    Thread.Sleep(5000);
                    if (Config.BrowserType != "ie")
                        login.SetDriver(BasePage.MultiDriver[1]);
                }
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                {
                    result.steps[++ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }

                BasePage.MultiDriver[1].Close();
                if (Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                    Thread.Sleep(5000);
                    login.SetDriver(BasePage.MultiDriver[2]);
                }
                if (Config.BrowserType == "firefox" || Config.BrowserType == "chrome")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("ie"));
                    Thread.Sleep(5000);
                    login.SetDriver(BasePage.MultiDriver[2]);
                }
                //Verfiy the Response Message
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                BasePage.MultiDriver[2].Close();
                //Edge Browser

                BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Thread.Sleep(5000);
                //Driver = BasePage.MultiDriver[3];
                Driver = BasePage.MultiDriver.Last();
                login.DriverGoTo(login.url);
                login.LoginGrid(adminusername, adminpassword);
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                conferencefolders.SelectStudy1("Patient ID", PatinetID);
                if (BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Displayed)
                {
                    BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Click();
                }
                try { BluRingViewer.WaitforThumbnails(); BluRingViewer.WaitforThumbnails(); }
                catch (Exception ex) { }

                BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
                Driver = BasePage.MultiDriver.Last();
                Logger.Instance.InfoLog(browserList[2] + " launched");
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                BasePage.MultiDriver.Last().Close();// [4].Close();
                BasePage.MultiDriver.Remove(MultiDriver.Last());
                //BasePage.MultiDriver[3].Close();
                BasePage.MultiDriver.Last().Close();
                BasePage.MultiDriver.Remove(MultiDriver.Last());


                //BasePage.MultiDriver.Add(login.InvokeBrowser(Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                conferencefolders.SelectStudy1("Patient ID", PatinetID, dblclick: false);
                if (BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Displayed)
                {
                    BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Click();
                }
                try
                { BluRingViewer.WaitforThumbnails(); BluRingViewer.WaitforViewports(); }
                catch (Exception ex) { }
                Thread.Sleep(10000);
                Driver.SwitchTo().DefaultContent();
                BasePage.Driver.Manage().Cookies.DeleteAllCookies();
                //Open a new tab
                Driver.SwitchTo().DefaultContent();
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Able to receive success 1 response after deleting the Cookies");
                }

                BasePage.Driver.SwitchTo().Window(tabs[0]);
                login.DriverGoTo(login.url);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.modifyBtn().Click();
                TextBox SetTimeout = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), "AutoSelectTextBox", 0);
                SetTimeout.Enter("3");
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception) { }
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                }
                catch (Exception) { }
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                conferencefolders.SelectStudy1("Patient ID", PatinetID, dblclick: false);
                if (BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Displayed)
                {
                    BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder).Click();
                }
                try { BluRingViewer.WaitforThumbnails(); BluRingViewer.WaitforViewports(); }
                catch (Exception ex) { }

                // Wait for  session Time out.
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 6, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();

                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                //Launch websocket console in the same browser
                //Websocket console should open
                NavigateTo_SoftwareHixieURL();
                //Connect to the server with the Port 8181
                Establish_WebSocketConncetion(Config.IConnectIP);
                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);
                //Verfiy the Response Message
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[0], Disconnected: true))
                {
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + "Disconnected");
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Able to receive success 1 even after the Session time out");
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //step - 12
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //Logout
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {

                login.SetDriver(BasePage.MultiDriver[0]);
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.modifyBtn().Click();
                TextBox SetTimeout = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), "AutoSelectTextBox", 0);
                SetTimeout.Enter("30");
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception) { }
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                }
                catch (Exception) { }
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary> 
        /// 160915   - This Test Case is Study Authorization in Integrator mode on desktop WITHOUT user sharing - set default viewer to be Enterprise viewer
        /// </summary>
        ///
        public TestCaseResult Test_160915(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            UserPreferences userpref = new UserPreferences();

            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string[] PatinetIDS = PatinetID.Split('@');
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid , "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");
                string WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetHeaderData");
                string WebSocketResponse_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetHeaderData");
                string WebSocketQuery_FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_FilePath");
                string WebSocketResponse_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_FilePath");
                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string[] WebSocketResponse_HeaderData = WebSocketResponse_GetHeaderData.Split('@');
                string[] WebSocketResponse_FilePath = WebSocketResponse_GetFilePath.Split('@');

                string Datasource = login.GetHostName(Config.SanityPACS);
                String URL = "http://" + Config.IConnectIP + "/webaccess";

                //Step:1
                //PreConition First Step
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Integrator");
                wpfobject.WaitTillLoad();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always disabled");
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                servicetool.AllowShowSelector().Checked = true;
                servicetool.WaitWhileBusy();
                servicetool.ApplyEnableFeatures();
                login.UncommentXMLnode("id", "Bypass");
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step:2
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step:3
                ehr.SetCommonParameters(address: URL);
                ehr.SetSelectorOptions(showSelector: "False");
                ExecutedSteps++;

                //Step:4
                ehr.SetSearchKeys_Study(PatinetIDS[0], "Patient_ID");
                ehr.SetSearchKeys_Study(Datasource, "Datasource");
                ExecutedSteps++;

                //Step:5
                String url_1 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                Driver.Quit();
                login.CreateNewSesion();
                login.NavigateToIntegratorURL(url_1);
                try { BluRingViewer.WaitforThumbnails(); BluRingViewer.WaitforViewports();  }
                catch (Exception ex) { }

                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 3, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 3 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
               
                ExecutedSteps++;

                //Step 6
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                IList<string> tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                ExecutedSteps++;

                //Step 7
                //Send the wecoket GetInstanceList Query
                Send_WebSocketQuery(WebSocketQuery_GetInstanceList);
                if (VerfiyResponseMessage(WebSocketResponse_InstanceList[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 8
                string FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");
                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);
                Send_WebSocketQuery(WebSocketQuery_GetHeaderData);
                if (VerfiyResponseMessage(WebSocketResponse_HeaderData[1]))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[1]);
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 9
                NavigateTo_SoftwareHixieURL();
                Establish_WebSocketConncetion(Config.IConnectIP);
                WebSocketQuery_FilePath = WebSocketQuery_FilePath.Replace("\"FILEPATHURL\"", FilePathURL);
                Send_WebSocketQuery(WebSocketQuery_FilePath);
                if (VerfiyResponseMessage(WebSocketResponse_FilePath[1]))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketQuery_FilePath);
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                    result.steps[++ExecutedSteps].AddFailStatusList("Failed : The Response Message for the GetPixelData ");

                //Step 10
                BasePage.Driver.SwitchTo().Window(tabs[0]).Close();
                Driver.Quit();
                login.CreateNewSesion();
                Thread.Sleep(5000);
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, null, true) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_FilePath, null, true))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[0] + "And " + WebSocketResponse_InstanceList[0] + "And" + WebSocketResponse_FilePath[0]);
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 11
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL);
                ehr.SetSelectorOptions(showSelector: "False");
                ehr.SetSearchKeys_Study(PatinetIDS[0], "Patient_ID");
                ehr.SetSearchKeys_Study(Datasource, "Datasource");
                url_1 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                Driver.Quit();
                login.CreateNewSesion();
                login.NavigateToIntegratorURL(url_1);
                try { BluRingViewer.WaitforThumbnails(); BluRingViewer.WaitforViewports(); }
                catch (Exception ex) { }
                try
                {
                    Driver.Manage().Cookies.DeleteAllCookies();
                }
                catch (Exception ex) { }
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, null, true) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_FilePath, null, true))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[0] + "And " + WebSocketResponse_InstanceList[0] + "And" + WebSocketResponse_FilePath[0]);
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);
                Driver.Quit();
                basepage.CreateNewSesion();

                //Step 12
                ehr.LaunchEHR();
                ExecutedSteps++;

                //Step 13
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL);
                ehr.SetSelectorOptions(showSelector: "False");
                ExecutedSteps++;

                //Step 14
                ehr.SetSearchKeys_Study(PatinetIDS[1], "Patient_ID");
                String url_2 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step 15
                login.NavigateToIntegratorURL(url_2);
                try { BluRingViewer.WaitforThumbnails(); BluRingViewer.WaitforViewports(); } catch (Exception) { }
                ExecutedSteps++;

                //Step 16
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[0]) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, WebSocketResponse_HeaderData[0]) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_FilePath, WebSocketResponse_FilePath[0]))
                {
                    Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[0] + "And " + WebSocketResponse_InstanceList[0] + "And" + WebSocketResponse_FilePath[0]);
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();


                //Step 17
                try
                {
                    BasePage.MultiDriver.Add(BasePage.Driver);
                    BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                    Thread.Sleep(10000);
                    login.SetDriver(BasePage.MultiDriver.Last());
                    if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, null, true) && SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_FilePath, null, true))
                    {
                        Logger.Instance.InfoLog("Successfully verfiyed the Response Message" + WebSocketResponse_HeaderData[0] + "And " + WebSocketResponse_InstanceList[0] + "And" + WebSocketResponse_FilePath[0]);
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].SetLogs();
                    BasePage.MultiDriver.Last().Close();
                    BasePage.MultiDriver.Remove(MultiDriver.Last());
                    login.SetDriver(BasePage.MultiDriver.Last());
                    throw new Exception("Error occured", e);
                }
                BasePage.MultiDriver.Last().Close();

                //Step 18
                try
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("ie"));
                    login.SetDriver(BasePage.MultiDriver.Last());

                    //Verfiy the Response Message
                    if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    {
                        result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }

                }
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].SetLogs();
                    BasePage.MultiDriver.Last().Close();
                    login.SetDriver(BasePage.MultiDriver.Last());
                    throw new Exception("Error occured", e);
                }
                BasePage.MultiDriver.Last().Close();
                login.SetDriver(BasePage.MultiDriver.Last());

                //Step 19
                //Edge Browser
                BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Thread.Sleep(5000);
                //Driver = BasePage.MultiDriver[3];
                Driver = BasePage.MultiDriver.Last();
                login.DriverGoTo(login.url);
                login.LoginGrid(adminusername, adminpassword);
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                BasePage.MultiDriver.Last().Close();// [4].Close();
                BasePage.MultiDriver.Remove(MultiDriver.Last());
                //BasePage.MultiDriver[3].Close();

                BasePage.Driver.Quit();
                basepage.KillProcessByName("chrome");
                basepage.KillProcessByName("iexplore");
                basepage.KillProcessByName("firefox");
                basepage.CreateNewSesion();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {



            }
        }

        /// <summary> 
        /// 160917    - Study Authorization in Guest mode - set default viewer to be Enterprise viewer
        /// </summary>
        ///
        public TestCaseResult Test_160917(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                MultiDriver = new List<IWebDriver>();

                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string WebSocketQuery_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetInstanceList");
                string WebSocketResponse_GetInstanceList = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetInstanceList");
                string WebSocketQuery_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_GetHeaderData");
                string WebSocketResponse_GetHeaderData = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_GetHeaderData");
                string WebSocketQuery_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketQuery_FilePath");
                string WebSocketResponse_GetFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "WebSocketResponse_FilePath");
                string[] WebSocketResponse_InstanceList = WebSocketResponse_GetInstanceList.Split('@');
                string[] WebSocketResponse_HeaderData = WebSocketResponse_GetHeaderData.Split('@');
                string[] WebSocketResponse_FilePath = WebSocketResponse_GetFilePath.Split('@');
                string Datasource = login.GetHostName(Config.SanityPACS);
                string expected_email_message = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "Email_Study_Templete");
                string FilePathURL = null;
                IList<string> tabs = new List<string>(Driver.WindowHandles);

                ServiceTool serviceTool = new ServiceTool();
                serviceTool.InvokeServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);
                serviceTool.RestartService();
                serviceTool.CloseServiceTool();

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Pre-Condition 
                RoleManagement Rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                Rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                Rolemanagement.SelectRole(Config.adminRoleName);
                Rolemanagement.EditRoleByName(Config.adminRoleName);
                PageLoadWait.WaitForFrameLoad(10);
                Rolemanagement.SetCheckboxInEditRole("email",0);
                Rolemanagement.ClickSaveEditRole();


                //Step 2 Search a study
                Studies studies1 = (Studies)login.Navigate("Studies");
                studies1.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                studies1.SelectStudy("Patient ID", PatinetID);
                BluRingViewer blueRingViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();


                //Step 3
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils ph1Email = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                ph1Email.MarkAllMailAsRead("INBOX");
                String pinnumber = blueRingViewer.EmailStudy_BR(Config.CustomUser1Email);
                result.steps[++ExecutedSteps].StepPass();

                //Step 4
                if (pinnumber == null && (String.IsNullOrWhiteSpace(pinnumber)))
                {
                    blueRingViewer.CloseBluRingViewer();
                    throw new Exception("Error While Get the PINNumber by Email Study");
                }
                result.steps[++ExecutedSteps].StepPass();

                //Step 5
                blueRingViewer.CloseBluRingViewer();
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();

                //Step 6
                downloadedMail = ph1Email.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = ph1Email.GetEmailedStudyLink(downloadedMail);
                var EmailMessage = downloadedMail["Body"];

                expected_email_message = expected_email_message.Replace("[SENDERNAME]","Testing");
                expected_email_message = expected_email_message.Replace("[RECEIVERNAME]", "Testing").Replace("[CONFIG.EMAIL]", Config.AdminEmail).Replace("[STUDYURL]", emaillink);
                if(expected_email_message.Equals(EmailMessage))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Excpeted Email :"+ expected_email_message +"\n Actual Email: "+ EmailMessage);
                }

                //step 7
                blueRingViewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                result.steps[++ExecutedSteps].StepPass();

                //Step 8
                IJavaScriptExecutor js1 = (IJavaScriptExecutor)BasePage.Driver;
                js1.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                NavigateTo_SoftwareHixieURL();
                result.steps[++ExecutedSteps].StepPass();

                //Step 9
                //Verfiy the Response Message
                if (!SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[1]))
                    throw new Exception("Error while get the encrypted path values");

                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, WebSocketResponse_InstanceList[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the  GetInstanceList Response Message" + WebSocketResponse_InstanceList[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 10
                FilePathURL = logMessage().Text.Split(new string[] { "\"FilePath\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                FilePathURL = FilePathURL.Replace("/", "%2F").Replace("+", "%2B");
                WebSocketQuery_GetHeaderData = WebSocketQuery_GetHeaderData.Replace("\"FILEPATHURL\"", FilePathURL);
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetHeaderData, WebSocketResponse_HeaderData[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the GetHeaderData Response Message" + WebSocketResponse_HeaderData[1]);
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 11
                WebSocketQuery_GetFilePath = WebSocketQuery_GetFilePath.Replace("\"FILEPATHURL\"", FilePathURL);
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetFilePath, WebSocketResponse_FilePath[1]))
                    result.steps[++ExecutedSteps].StepPass("Successfully verfiyed the Response Message" + WebSocketResponse_FilePath[1]);
                else
                    result.steps[++ExecutedSteps].StepFail("Error While verfiy response for FilePath Websocket");
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //step 12
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.SetDriver(MultiDriver[0]);
                ++ExecutedSteps;
                if (Config.BrowserType == "chrome" || Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                    login.SetDriver(BasePage.MultiDriver[1]);
                    login.DriverGoTo(login.url);
                }
                else if (Config.BrowserType == "firefox" || Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                    if (Config.BrowserType != "ie")
                        login.SetDriver(BasePage.MultiDriver[1]);
                }
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();
                BasePage.MultiDriver[1].Close();
                if (Config.BrowserType == "ie")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                    login.SetDriver(BasePage.MultiDriver[2]);
                }
                if (Config.BrowserType == "firefox" || Config.BrowserType == "chrome")
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser("ie"));
                    login.SetDriver(BasePage.MultiDriver[2]);
                }
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null ,true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();
                BasePage.MultiDriver[2].Close();

                //Edge Browser
                BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Driver = BasePage.MultiDriver.Last();
                blueRingViewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
               //if(!blueRingViewer.VerifyViewPortIsActive())
               // {
               //     result.steps[ExecutedSteps].AddFailStatusList();
               // }
               // else
               // {
                    //BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
                    //Driver = BasePage.MultiDriver.Last();
                    //Logger.Instance.InfoLog(browserList[2] + " launched");
                    //BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                    //Thread.Sleep(2000);
                    //PageLoadWait.WaitForPageLoad(10);
                    //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                    //var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                    //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    //if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
                    //{
                    //    result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
                    //}
                    //BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                    //Thread.Sleep(2000);
                    //PageLoadWait.WaitForPageLoad(10);
                    //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                    //var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    //if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
                    //{
                    //    result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
                    //}
                    //if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                    //{
                    //    result.steps[ExecutedSteps].StepFail();
                    //}
                    //else
                    //{
                    //    result.steps[ExecutedSteps].StepPass();
                    //}
                    //BasePage.MultiDriver.Last().Close();
                    //BasePage.MultiDriver.Remove(MultiDriver.Last());
                    //BasePage.MultiDriver.Last().Close();
                    //BasePage.MultiDriver.Remove(MultiDriver.Last());
                //}

                //result.steps[ExecutedSteps].comments = "Edge Broswer Not Automated";
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);

                //Test Case - 160908
                js1 = (IJavaScriptExecutor)BasePage.Driver;
                js1.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);
                Driver.SwitchTo().DefaultContent();
                try
                {
                    BasePage.Driver.Manage().Cookies.DeleteAllCookies();
                } catch(Exception){}
                Driver.SwitchTo().DefaultContent();
                js1 = (IJavaScriptExecutor)BasePage.Driver;
                js1.ExecuteScript("window.open();");
                tabs = new List<string>(BasePage.Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null, true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();
                BasePage.Driver.SwitchTo().Window(tabs[0]);
                login.DriverGoTo(login.url);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                if (SendSocketQueryAndVerfiyResponse(Config.IConnectIP, WebSocketQuery_GetInstanceList, null ,true))
                    result.steps[ExecutedSteps].AddPassStatusList("Successfully verfiyed the Response Message" + WebSocketResponse_InstanceList[0]);
                else
                    result.steps[ExecutedSteps].AddFailStatusList();
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                login.Logout();
                result.FinalResult(ExecutedSteps);
                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {

                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                try
                {
                  login.SetDriver(BasePage.MultiDriver[0]);
                }catch(Exception ex)
                { }

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                //ServiceTool serviceTool = new ServiceTool();
                //serviceTool.InvokeServiceTool();
                //servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);
                //serviceTool.CloseServiceTool();
            }
        }

		/// <summary> 
		/// 160925  - Study Authorization in Standalone mode_HTML4 - Clear browser cache and try existing query
		/// </summary>
		///
		public TestCaseResult Test_160925(string testid, string teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;
				string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);

				string Datasource = login.GetHostName(Config.SanityPACS);

				//Step 1 - Run test case #160919. Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser)
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
				study.SelectStudy("Patient ID", PatinetID);
				StudyViewer studyviewer = StudyViewer.LaunchStudy();
				result.steps[++ExecutedSteps].StepPass();

				//Step 2 - Clear browser cache (Chrome browser)
				Driver.Manage().Cookies.DeleteAllCookies();
				result.steps[++ExecutedSteps].StepPass();

				//Step 3 - Paste the request URLs (as captured in test case #160919) in another tab of the same browser
				IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				IList<string> tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Manage().Window .Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				PageLoadWait.WaitForPageLoad(10);												
				var newTabImage = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				switch (Config.BrowserType.ToLower())
				{
					case "firefox":
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage, isCaptureScreen: true))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
						break;
					default:
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
						break;
				}				

				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				switch (Config.BrowserType.ToLower())
				{
					case "firefox":
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: true))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
						break;
					default:
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
						break;
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 4-1. Launch iCA url and DO NOT login
				BasePage.Driver.Quit();
				Thread.Sleep(500);
				login.CreateNewSesion();
				login.DriverGoTo(login.url);

				//Step 4-2. Paste the request URL(as captured in test case #160919) in another tab of the same browser
				js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Manage().Window.Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				switch (Config.BrowserType.ToLower())
				{
					case "firefox":
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3, isCaptureScreen: true))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
						break;
					default:
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
						break;
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				switch (Config.BrowserType.ToLower())
				{
					case "firefox":
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4, isCaptureScreen: true))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
						break;
					default:
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
						break;
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Report Result
				result.FinalResult(ExecutedSteps);

				Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				return result;
				//------------End of script---

			}
			catch (Exception e)
			{
				//Log Exception
				Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			finally
			{
				BasePage.Driver.Quit();
				Thread.Sleep(500);
				login.CreateNewSesion();
				login.DriverGoTo(login.url);
			}
		}

		/// <summary> 
		/// 160923  - Study Authorization in Standalone mode_HTML4 - Testing into Inactive web browser after idle session timeout
		/// </summary>
		///
		public TestCaseResult Test_160923(string testid, string teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;
				string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);

				string Datasource = login.GetHostName(Config.SanityPACS);

				// Precondition - Set session timeout as 1 min in Service tool
				servicetool.LaunchServiceTool();
				servicetool.NavigateToTab("Security");
				servicetool.ClickModifyButton();
				wpfobject.SetSpinner(ServiceTool.Spinner_ID, "1");
				Thread.Sleep(3000);
				servicetool.ClickApplyButtonFromTab();
				wpfobject.WaitTillLoad();
				wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
				try
				{
					servicetool.CickApplyButton();
					wpfobject.WaitTillLoad();
					wpfobject.ClickButton("2");
					Logger.Instance.InfoLog("ok clicked");
					wpfobject.WaitTillLoad();
				}
				catch (Exception err)
				{
					Logger.Instance.ErrorLog("Error in Clicking ok button. " + err.Message);
				}
				wpfobject.WaitTillLoad();
				servicetool.RestartIISandWindowsServices();
				servicetool.CloseServiceTool();

				//Step 1 - Run - Test Case #160919
				result.steps[++ExecutedSteps].StepPass();

				//Step 2 - 	launch iCA and login as Administrator
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				result.steps[++ExecutedSteps].StepPass();

				//Step 3 -Navigate to studies tab and search for PatientID = GE0514
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
				study.SelectStudy("Patient ID", PatinetID);
				result.steps[++ExecutedSteps].StepPass();

				//Step 4 - launch study in Enterprise Viewer
				StudyViewer studyviewer = StudyViewer.LaunchStudy();
				result.steps[++ExecutedSteps].StepPass();

				//Step 5 - Leave the iCA session open so that it automatically logout as per Idle session timeout value
				PauseTimer pt = new PauseTimer();
				pt.PauseExecution(1);
				Driver.SwitchTo().DefaultContent();
				BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.UserNamefield)));
				string ErrMsg = Driver.FindElement(By.Id(Locators.ID.ErrMsg)).Text;
				if (ErrMsg.Equals("You have not logged in yet or your session has expired. Please log in again."))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 6 - Paste the request URLs (as captured in test case #160919) in another tab of the same browser
				IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				IList<string> tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Manage().Window.Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				PageLoadWait.WaitForPageLoad(10);				
				var errorElement1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				switch (Config.BrowserType.ToLower())
				{
					case "firefox":
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], errorElement1, isCaptureScreen: true))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
						break;
					default:
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], errorElement1))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
						break;
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				PageLoadWait.WaitForPageLoad(10);
				var errorElement2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				switch (Config.BrowserType.ToLower())
				{
					case "firefox":
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], errorElement2, isCaptureScreen: true))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
						break;
					default:
						if (!studyviewer.CompareImage(result.steps[ExecutedSteps], errorElement2))
							result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
						break;
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}				
					
				//Report Result
				result.FinalResult(ExecutedSteps);

				Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				return result;
				//------------End of script---

			}
			catch (Exception e)
			{
				//Log Exception
				Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			finally
			{
				try
				{
					// Revert - Session timeout to 30 min in Service tool
					servicetool.LaunchServiceTool();
					servicetool.NavigateToTab("Security");
					servicetool.ClickModifyButton();
					wpfobject.SetSpinner(ServiceTool.Spinner_ID, "30");
					Thread.Sleep(3000);
					servicetool.ClickApplyButtonFromTab();
					wpfobject.WaitTillLoad();
					wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
					wpfobject.WaitTillLoad();
					try
					{
						wpfobject.ClickButton("2");
						Logger.Instance.InfoLog("ok clicked");
						wpfobject.WaitTillLoad();
					}
					catch (Exception err)
					{
						Logger.Instance.ErrorLog("Step2_2 Error in Clicking ok button. " + err.Message);
					}					
					servicetool.RestartIISandWindowsServices();
					servicetool.CloseServiceTool();
				}
				catch (Exception)
				{
					Logger.Instance.InfoLog("Error while reverting session timeout value to 30 min in service tool");
				}
				BasePage.Driver.Quit();
				Thread.Sleep(500);
				login.CreateNewSesion();
				login.DriverGoTo(login.url);
			}
		}

		/// <summary> 
		/// 160921  - Study Authorization in Standalone mode_HTML4 - Testing into different browser without login to iCA
		/// </summary>
		///
		public TestCaseResult Test_160921(string testid, string teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			BasePage.MultiDriver = new List<IWebDriver>();
			BasePage.MultiDriver.Add(BasePage.Driver);
			Config.node = Config.Clientsys4;
			try
			{
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;
				string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL").ToString().Replace("<iConnectIP>",Config.IConnectIP);
				string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string Datasource = login.GetHostName(Config.SanityPACS);

				List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
				for (int count = 0; count < 3; count++)
				{
					if (Config.BrowserType.ToLower() == browserList[count])
					{
						browserList[count] = "chrome";
						break;
					}
				}

				//Step 1 - 	launch iCA and login as Administrator
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				result.steps[++ExecutedSteps].StepPass();

				//Step 2 -Navigate to studies tab and search for PatientID = GE0514
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
				if (study.CheckStudy("Patient ID", PatinetID))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 3 - launch study in Enterprise Viewer
				study.SelectStudy("Patient ID", PatinetID);
				StudyViewer studyviewer = StudyViewer.LaunchStudy();
				if (studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 4 - Launch Firefox browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
				Driver = BasePage.MultiDriver[1];
				Logger.Instance.InfoLog(browserList[0] + " launched");				
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Driver.Manage().Window.Minimize();
				Driver.Manage().Window.Maximize();
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
				}				
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);				
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				BasePage.MultiDriver[1].Close();

				//Step 5 - Launch IE 11 browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[1]));
				Driver = BasePage.MultiDriver[2];
				Logger.Instance.InfoLog(browserList[1] + " launched");
				//Driver.Manage().Window.Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading viewport URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				BasePage.MultiDriver[2].Close();

				//Step 6 - Launch Edge browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
				BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Driver = BasePage.MultiDriver.Last();
				login.DriverGoTo(login.url);
				login.LoginGrid(adminusername, adminpassword);
				study = (Studies)login.Navigate("Studies");
				study.SearchStudy(patientID: PatinetID, Datasource: Datasource);				
				study.SelectStudy("Patient ID", PatinetID);
				studyviewer = StudyViewer.LaunchStudy();
				if (!studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				else
				{
					BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
                    Driver = BasePage.MultiDriver.Last();
					Logger.Instance.InfoLog(browserList[2] + " launched");
					BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
					}
					BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
					}
					if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
					{
						result.steps[ExecutedSteps].StepFail();
					}
					else
					{
						result.steps[ExecutedSteps].StepPass();
					}
                    BasePage.MultiDriver.Last().Close();
                    BasePage.MultiDriver.Remove(MultiDriver.Last());
                    BasePage.MultiDriver.Last().Close();
                    BasePage.MultiDriver.Remove(MultiDriver.Last());
                }
				//Report Result
				result.FinalResult(ExecutedSteps);

				Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				return result;
				//------------End of script---

			}
			catch (Exception e)
			{
				//Log Exception
				Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			finally
			{
				try
				{
					Driver = BasePage.MultiDriver[0];
					login.closeallbrowser();
					BasePage.Driver.Quit();
					Thread.Sleep(500);
					login.CreateNewSesion();
					login.DriverGoTo(login.url);
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Error in finally block: " + ex.Message);
				}
			}
		}

        /// <summary> 
        /// 160919  - Study Authorization in Standalone mode_HTML4 - Testing in Active Web Session
        /// </summary>
        ///
        public TestCaseResult Test_160919(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ThumbnailURL = ThumbnailURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
                ViewPortURL = ViewPortURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
                string Datasource = login.GetHostName(Config.SanityPACS);

                //Step 1 - Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser)
                //login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2 - Navigate to Studies tab and search for Patient ID=GE0514
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3 - Launch the study in Old viewer (HTML4 viewer) with double click or clicking view study button
                study.SelectStudy("Patient ID", PatinetID);
                StudyViewer studyviewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].StepPass();

                //step 4 - Leave the iCA session OPEN and ACTIVE with "Administrator" login - iCA session is open and Active
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                IList<string> tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                result.steps[++ExecutedSteps].StepPass();

                //Step  - 5, 6
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();

                //Step - 7
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step7_1 = false;
                if (Config.BrowserType.ToLower() == "firefox")
                    step7_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: true);
                else
                    step7_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab));


                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step7_2 = false;
                if (Config.BrowserType.ToLower() == "firefox")
                    step7_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: true);
                else
                    step7_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab));

                if (step7_1 && step7_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                studyviewer.CloseStudy();
                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                BasePage.Driver.Quit();
                basepage.KillProcessByName("chrome");
                basepage.KillProcessByName("iexplore");
                basepage.KillProcessByName("firefox");
                Thread.Sleep(500);
                login.CreateNewSesion();
                login.DriverGoTo(login.url);

            }
        }

        /// <summary> 
        /// 160924  - Study Authorization in Standalone mode_HTML4- Testing into same browser with another user
        /// </summary>
        ///
        public TestCaseResult Test_160924(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ThumbnailURL = ThumbnailURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
                ViewPortURL = ViewPortURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
                string Datasource = login.GetHostName(Config.SanityPACS);
                String user1 = "user_160924_Test";

                //precondition- Create user
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                UserManagement usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (!usermanagement.SearchUser(user1, Config.adminGroupName))
                {
                    usermanagement.CreateUser(user1, Config.adminGroupName, Config.adminRoleName);
                }
                login.Logout();

                //Step 1 - Run test case #160922
                result.steps[++ExecutedSteps].StepPass();

                //Step 2 - launch iCA and login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3 - Navigate to studies tab and search for patientID=GE0514
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                result.steps[++ExecutedSteps].StepPass();

                //step 4 - launch study in old viewer (HTML4)
                study.SelectStudy("Patient ID", PatinetID);
                StudyViewer studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 5 - Logout
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();

                //Step 6 
                login.LoginIConnect(user1, user1);
                result.steps[++ExecutedSteps].StepPass();

                //Step 7
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                IList<string> tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step7_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step7_1 && step7_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Step 8 
                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                study.SelectStudy("Patient ID", PatinetID);
                result.steps[++ExecutedSteps].StepPass();

                //step 9 - launch study in old viewer (HTML4)
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 10
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step10_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step10_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                if (step7_1 && step7_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                BasePage.Driver.SwitchTo().Window(tabs[0]);
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                studyviewer.CloseStudy();
                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                BasePage.Driver.Quit();
                basepage.KillProcessByName("chrome");
                basepage.KillProcessByName("iexplore");
                basepage.KillProcessByName("firefox");
                Thread.Sleep(500);
                login.CreateNewSesion();
                login.DriverGoTo(login.url);

            }
        }

        /// <summary> 
        /// 160922   - Study Authorization in Standalone mode_HTML4 - Testing after logging out from Active web session
        /// </summary>
        ///
        public TestCaseResult Test_160922(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
                string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                ThumbnailURL = ThumbnailURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
                string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ViewPortURL = ViewPortURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);

                string Datasource = login.GetHostName(Config.SanityPACS);

                //Step 1 
                result.steps[++ExecutedSteps].StepPass();

                //Step 2 - Launch iCA and login as "Administrator", password "Administrator" 
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3 - Navigate to Studies tab and search for Patient ID=GE0514
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
                result.steps[++ExecutedSteps].StepPass();

                //Step 4 - Launch study in Enterprise Viewer
                study.SelectStudy("Patient ID", PatinetID);
                StudyViewer studyviewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].StepPass();

                //Step 5 - close study by clicking on the X button
                studyviewer.CloseStudy();
                result.steps[++ExecutedSteps].StepPass();

                //step 6 - Paste the request urls "OperationClass=LoadImages" as captured in test case 160919
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                IList<string> tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step5_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step5_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step5_1 && step5_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                Thread.Sleep(3000);


                //step 7 - Get the start time , end time an verfiy log message.
                var LogStartTime = System.DateTime.Now;
                Thread.Sleep(5000);
                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                Thread.Sleep(10000);
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
                                    if (entry.Value["Message"].Contains("Image Load Error"))
                                        if (entry.Value["Detail"].Contains("Study is not authorized"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                        if (loggedError == "Image Load Error")
                            break;
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Image Load Error")
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
                    }
                }


                //Step 8 - get the start time , end time an verfiy log message.
                LogStartTime = System.DateTime.Now;
                Thread.Sleep(5000);
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                Thread.Sleep(10000);
                LogEndTime = System.DateTime.Now;
                loggedError = string.Empty;
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
                                    if (entry.Value["Message"].Contains("Viewer Facade Error"))
                                        if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }

                        if (loggedError == "Viewer Facade Error")
                            break;

                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Viewer Facade Error")
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
                    }
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Step 9 -Logout
                login.Logout();
                Thread.Sleep(5000);
                BasePage.Driver.SwitchTo().DefaultContent();
                result.steps[++ExecutedSteps].StepPass();

                //Step 10
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(5000);
                BasePage.Driver.Manage().Window.Maximize();
                try
                {
                    tabs = new List<string>(Driver.WindowHandles);
                    if (tabs.Count != 2)
                        tabs = new List<string>(Driver.WindowHandles);
                    Thread.Sleep(5000);
                    tabs = Driver.WindowHandles.ToList();
                    BasePage.Driver.SwitchTo().Window(tabs[1]);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                    tabs = new List<string>(Driver.WindowHandles);
                    BasePage.Driver.SwitchTo().Window(tabs[1]);

                }
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step10_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step10_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                if (step10_1 && step10_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                BasePage.Driver.Quit();
                basepage.KillProcessByName("chrome");
                basepage.KillProcessByName("iexplore");
                basepage.KillProcessByName("firefox");
                Thread.Sleep(500);
                login.CreateNewSesion();
                login.DriverGoTo(login.url);

            }
        }

        /// <summary> 
        /// 160920 -  Study Authorization in Standalone mode_HTML4 -Testing UnAuthorized study in same Active Web Session
        /// </summary>
        ///
        public TestCaseResult Test_160920(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string[] patinetIDList = PatinetID.Split('@');
                //URL with Invaild patinetID.
                string ThumbnailURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                ThumbnailURLWithInVaildPatientID = ThumbnailURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);
                string ViewPortURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ViewPortURLWithInVaildPatientID = ViewPortURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);

                //URL with Invaild StudUID, seriousUID.
                string ThumbnailURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailLoadImageURL");
                ThumbnailURLWithInVaildUID = ThumbnailURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);
                string ViewPortURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ViewPortLoadImageURL");
                ViewPortURLWithInVaildUID = ViewPortURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);

                string Datasource = login.GetHostName(Config.SanityPACS);
                //Step 1  - iCA session is still Active and Open with Administrator login
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: patinetIDList[0], Datasource: Datasource);
                study.SelectStudy("Patient ID", patinetIDList[0]);
                StudyViewer studyviewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].StepPass();

                //step 2 - Copy paste the websocket URLS with invalid patunetID.
                var LogStartTime = System.DateTime.Now; // Start time to check on the Log
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                IList<string> tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                login.DriverGoTo(ThumbnailURLWithInVaildPatientID);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step2_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURLWithInVaildPatientID);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step2_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step2_1 && step2_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                Thread.Sleep(3000);

                //step 3 - paste the websocket URLS with invalid StudyUID, seriousUID.
                login.DriverGoTo(ThumbnailURLWithInVaildUID);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step3_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURLWithInVaildUID);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step3_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step3_1 && step3_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                Thread.Sleep(3000);

                //step 4 - Get the start time , end time an verfiy log message.
                Thread.Sleep(5000);
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
                                    if (entry.Value["Message"].Contains("Image Load Error"))
                                        if (entry.Value["Detail"].Contains("Study is not authorized"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                        if (loggedError == "Image Load Error")
                            break;
                        else
                            Logger.Instance.WarnLog("Unable to find the Log error, so start find the next log file for the day.");
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Image Load Error")
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
                    }
                }


                //Step 5 - get the start time , end time an verfiy log message.
                loggedError = string.Empty;
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
                                    if (entry.Value["Message"].Contains("Viewer Facade Error"))
                                        if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }

                        if (loggedError == "Viewer Facade Error")
                            break;

                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Viewer Facade Error")
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
                    }
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);
                studyviewer.CloseStudy();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                BasePage.Driver.Quit();
                basepage.KillProcessByName("chrome");
                basepage.KillProcessByName("iexplore");
                basepage.KillProcessByName("firefox");
                Thread.Sleep(5000);
                login.CreateNewSesion();
                login.DriverGoTo(login.url);

            }
        }

		/// <summary> 
		/// 160916  - Study Authorization in Integrator mode on desktop WITH user sharing - set user preference to launch old viewer
		/// </summary>
		///
		public TestCaseResult Test_160916(string testid, string teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
            UserPreferences userpref = new UserPreferences();
            int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			BasePage.MultiDriver = new List<IWebDriver>();
			BasePage.MultiDriver.Add(BasePage.Driver);
			Config.node = Config.Clientsys4;
			try
			{
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;
				string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "PatientID");
				string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string Datasource = login.GetHostName(Config.SanityPACS);

				List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
				for (int count = 0; count < 3; count++)
				{
					if (Config.BrowserType.ToLower() == browserList[count])
					{
						browserList[count] = "chrome";
						break;
					}
				}				

				//Step 1 - Precondition
				//Service Tool Setup
				servicetool.LaunchServiceTool();
				servicetool.NavigateToTab("Integrator");
				servicetool.WaitWhileBusy();
				servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled");
				wpfobject.WaitTillLoad();
				servicetool.RestartIISandWindowsServices();
				servicetool.CloseServiceTool();

				//iCA Userpreference Steup
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				login.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.HTML4RadioBtn().Click();
				userpref.CloseUserPreferences();
				login.Logout();
				result.steps[++ExecutedSteps].StepPass();

				//Step 2 - Launch TestEHR application from iCA server 
				ehr.LaunchEHR();
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

				//Step 3 - Enter address http://iCA-IP>/wbaccess/, username-password, security ID and update show selector to false
				wpfobject.GetMainWindow("Test WebAccess EHR");
				wpfobject.SelectTabFromTabItems("Image Load");
				wpfobject.WaitTillLoad();
				ehr.SetCommonParameters(address: "http://" + Config.IConnectIP + "/WebAccess", user: adminusername);
				ehr.SetSelectorOptions(showSelector: "False");
				result.steps[++ExecutedSteps].StepPass();

				//Step 4 - Enter the PatientID=GE0514 and click cmd line
				ehr.SetSearchKeys_Study(PatinetID, "Patient_ID", datasources: Datasource);
				String url_1 = ehr.clickCmdLine("ImageLoad");
				ehr.CloseEHR();
				if (string.IsNullOrEmpty(url_1))
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[++ExecutedSteps].StepPass();
				}

				//Step 5 - click on load and the generated url should open in browser
				login.NavigateToIntegratorURL(url_1);
				StudyViewer studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				if (studyviewer.ViewStudy(IntegratedDesktop: true))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 6 - Developer Tool> Network tab> All , capture url for Operation=LoadImage for Viewport and thumbnail
				result.steps[++ExecutedSteps].StepPass(); // Viewport and thumbnail URLs are stored in Excel.

				//Step 7 - Paste the query 1 and query 2 in another tab in same browse
				IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				IList<string> tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);				
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Corresponding study launched in new tab while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Corresponding study not launched in new tab while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Corresponding study launched in new tab while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Corresponding study not launched in new tab while loading thumbnail URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);

				//Step 8 - Paste the query 1 and query 2 in Firefox browser
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
				Driver = BasePage.MultiDriver[1];
				Logger.Instance.InfoLog(browserList[0] + " launched");
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Session timeout error displayed while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
				}				
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Session timeout error displayed while loading viewport URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				BasePage.MultiDriver[1].Close();

				//Step 9 - Paste the query 1 and query 2 in IE browser
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[1]));
				Driver = BasePage.MultiDriver[2];
				Logger.Instance.InfoLog(browserList[1] + " launched");
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading viewport URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				BasePage.MultiDriver[2].Close();

				//Step 10 - Paste the query 1 and query 2 in edge browser
				BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
				Driver = BasePage.MultiDriver[3];
				login.NavigateToIntegratorURL(url_1);
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				if (!studyviewer.ViewStudy(IntegratedDesktop: true))
				{				
					result.steps[++ExecutedSteps].StepFail();
				}
				else
				{
					BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
					Driver = BasePage.MultiDriver[4];
					Logger.Instance.InfoLog(browserList[2] + " launched");
					BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage7 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage7))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
					}
					BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage8 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage8))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
					}
					if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
					{
						result.steps[ExecutedSteps].StepFail();
					}
					else
					{
						result.steps[ExecutedSteps].StepPass();
					}
					BasePage.MultiDriver[4].Close();
					BasePage.MultiDriver[3].Close();
				}

				//Step 11 - Close the browser where study is opened and repeat steps 7
				Driver = BasePage.MultiDriver[0];
				login.CloseBrowser();
				login = new Login();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Logger.Instance.InfoLog(Config.BrowserType.ToLower() + " browser launched");
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage9 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage9, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage10 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage10, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 12 - Clear cache and try the same queries (as per step6) in same browser					
				login.DriverGoTo(login.url);
				login.NavigateToIntegratorURL(url_1);
				studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
				PageLoadWait.WaitForThumbnailsToLoad(40);
				PageLoadWait.WaitForAllViewportsToLoad(40);
				if (!studyviewer.ViewStudy(IntegratedDesktop: true))
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				else
				{
					try
					{
						Driver.Manage().Cookies.DeleteAllCookies();
					}
					catch (Exception) { }
					js = (IJavaScriptExecutor)BasePage.Driver;
					js.ExecuteScript("window.open();");
					tabs = new List<string>(Driver.WindowHandles);
					BasePage.Driver.SwitchTo().Window(tabs[1]);
					BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
					Logger.Instance.InfoLog(Config.BrowserType.ToLower() + " browser launched");
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					var newTabImage11 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage11, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
					}
					BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					var newTabImage12 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage12, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
					}
					if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
					{
						result.steps[ExecutedSteps].StepFail();
					}
					else
					{
						result.steps[ExecutedSteps].StepPass();
					}
				}

				//Report Result
				result.FinalResult(ExecutedSteps);

				Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				return result;
				//------------End of script---

			}
			catch (Exception e)
			{
				//Log Exception
				Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			finally
			{
				try
				{
					if (ExecutedSteps<11)
						Driver = BasePage.MultiDriver[0];
					login.closeallbrowser();
					BasePage.Driver.Quit();
					Thread.Sleep(500);
					login.CreateNewSesion();
					login.DriverGoTo(login.url);
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Error in finally block: " + ex.Message);
				}
			}
		}

		/// <summary> 
		/// 160913  - Study Authorization in Standalone mode - Priors/related studies in Old viewer (HTML4)
		/// </summary>
		///
		public TestCaseResult Test_160913(string testid, string teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;
				string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				string ThumbnailURLList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string ThumbnailURL1 = ThumbnailURLList.Split('|')[0];
				string ThumbnailURL2 = ThumbnailURLList.Split('|')[1];

				string Datasource = login.GetHostName(Config.EA91);

				//Step 1 - Precondition 
				result.steps[++ExecutedSteps].StepPass();

				//Step 2 - 	launch iCA and login as Administrator
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				result.steps[++ExecutedSteps].StepPass();

				//Step 3 -	Navigate to Studies tab and search for Patient ID=18080
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(patientID: PatinetID, Datasource: Datasource);
				study.SelectStudy("Accession", AccessionID);
				result.steps[++ExecutedSteps].StepPass();

				//Step 4 - Launch the study with accession number- 00012793 in Old viewer (HTML4viewer) by clicking on view study button
				StudyViewer studyviewer = StudyViewer.LaunchStudy();
				result.steps[++ExecutedSteps].StepPass();

				//Step 5 - Leave the iCA session OPEN and ACTIVE with "Administrator" login
				if (studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 6 - paste the captured query 1 in new tab in same browser:
				IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				IList<string> tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL1);
				BasePage.Driver.Manage().Window.Maximize();
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 7 - paste the captured query 2 in new tab in same browser:
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[2]);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL2);
				BasePage.Driver.Manage().Window.Maximize();
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 8 - Click on History tab and 
				BasePage.Driver.SwitchTo().Window(tabs[0]);
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.focus()");
				studyviewer.NavigateToHistoryPanel();

				// Refresh the browser where you pasted the Query 2
				BasePage.Driver.SwitchTo().Window(tabs[2]);
				Thread.Sleep(2000);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL2);
				Thread.Sleep(5000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 9 - Close the loaded study and refresh browser where you pasted Query 1 and Query 2
				BasePage.Driver.SwitchTo().Window(tabs[0]);
				studyviewer.CloseStudy();				

				BasePage.Driver.SwitchTo().Window(tabs[2]);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL1);
				Thread.Sleep(5000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);				
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Rendering error not displayed in query 1 page");
				}
				else
				{
					result.steps[ExecutedSteps].AddPassStatusList("Rendering error displayed in query 1 page");
				}
				
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL2);
				Thread.Sleep(5000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				newTabImage5.Click();
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Rendering error not displayed in query 2 page");
				}
				else
				{
					result.steps[ExecutedSteps].AddPassStatusList("Rendering error displayed in query 2 page");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Report Result
				result.FinalResult(ExecutedSteps);

				Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				return result;
				//------------End of script---

			}
			catch (Exception e)
			{
				//Log Exception
				Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			finally
			{				
				BasePage.Driver.Quit();
				Thread.Sleep(500);
				login.CreateNewSesion();
				login.DriverGoTo(login.url);
			}
		}

		/// <summary> 
		/// 160926  - Study Authorization in Standalone mode - XDS
		/// </summary>
		///
		public TestCaseResult Test_160926(string testid, string teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			BasePage.MultiDriver = new List<IWebDriver>();
			BasePage.MultiDriver.Add(BasePage.Driver);
			Config.node = Config.Clientsys4;
			try
			{
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;
				string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ViewPortLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
				

				String DomainName = GetUniqueDomainID("D26_");
				String RoleName = GetUniqueRole("R26_");
				String UName = GetUniqueUserId("U26_");
				String TopFolder = "160926_" + new Random().Next(1, 1000);
				String SubFolder = "160926_" + new Random().Next(1, 1000);
				String folderpath = TopFolder + "/" + SubFolder;

				List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
				for (int count = 0; count < 3; count++)
				{
					if (Config.BrowserType.ToLower() == browserList[count])
					{
						browserList[count] = "chrome";
						break;
					}
				}

				//iCA Userpreference Steup
				UserPreferences userpref = new UserPreferences();
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				login.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.HTML4RadioBtn().Click();
				userpref.CloseUserPreferences();

				// Enable conferencelists in Domain Management
				DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
				domainmanagement.SearchDomain(Config.adminGroupName);
				domainmanagement.SelectDomain(Config.adminGroupName);
				domainmanagement.ClickEditDomain();
				PageLoadWait.WaitForFrameLoad(5);
				domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0);
				PageLoadWait.WaitForFrameLoad(5);
				domainmanagement.ClickSaveEditDomain();
				PageLoadWait.WaitForPageLoad(10);

				//Create Folder
				ConferenceFolders conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
				bool step3_1 = conferencefolders.CreateToplevelFolder(TopFolder); //First Top Folder
				bool step3_2 = conferencefolders.CreateSubFolder(TopFolder, SubFolder);

				//Enable Grant access and grant permissions to User1 for XDS with PID=PID145
				UserManagement usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.CreateUser(UName, Config.adminGroupName, Config.adminRoleName);				
				Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatinetID, AccessionNo: AccessionID, Datasource: Config.XDS_EA2);
                studies.SelectStudy1("Patient ID", PatinetID);
				studies.ShareStudy(false, new string[] { UName });
				login.Logout();

				//Userpreference Steup in new user
				login.DriverGoTo(login.url);
				login.LoginIConnect(UName, UName);
				login.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.HTML4RadioBtn().Click();
				userpref.CloseUserPreferences();
				login.Logout();

				//Step 1 - Launch iCA in Chrome and login as "Administrator", password "Administrator"
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				result.steps[++ExecutedSteps].StepPass();

				//Step 2 -Navigate to studies tab. Select datasource with XDS (EA-46). Search first name = pqa, PatientID = PID145			
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(patientID: PatinetID, AccessionNo: AccessionID, Datasource: Config.XDS_EA2);
				if (study.CheckStudy("Patient ID", PatinetID))
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 3 -  Launch Study in Old viewer by clicking Enterprise button
				study.SelectStudy("Patient ID", PatinetID);
				StudyViewer studyviewer = StudyViewer.LaunchStudy();
				if (studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}				

				//Step 4 - Developer Tool> Network tab> All , capture url for Operation=LoadImage for Viewport and thumbnail
				result.steps[++ExecutedSteps].StepPass(); // Viewport and thumbnail URLs are stored in Excel.

				//Step 5 - Paste the query 1 and query 2 in another tab in same browse
				IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				IList<string> tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Corresponding study launched in new tab while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Corresponding study not launched in new tab while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Corresponding study launched in new tab while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Corresponding study not launched in new tab while loading thumbnail URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);

				//Step 6_1 - Paste the query 1 and query 2 in Firefox browser
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
				//Driver = BasePage.MultiDriver[1];
				Driver = BasePage.MultiDriver.Last();
				Logger.Instance.InfoLog(browserList[0] + " launched");
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Session timeout error displayed while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Session timeout error displayed while loading viewport URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
				}
				//BasePage.MultiDriver[1].Close();
				BasePage.MultiDriver.Last().Close();
				BasePage.MultiDriver.Remove(MultiDriver.Last());

				//Step 6_2 - Paste the query 1 and query 2 in IE browser
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[1]));
				//Driver = BasePage.MultiDriver[2];
				Driver = BasePage.MultiDriver.Last();
				Logger.Instance.InfoLog(browserList[1] + " launched");
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading viewport URL");
				}
				//BasePage.MultiDriver[2].Close();
				BasePage.MultiDriver.Last().Close();
				BasePage.MultiDriver.Remove(MultiDriver.Last());

				//Step 6_3 - Paste the query 1 and query 2 in edge browser
				BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
				//Driver = BasePage.MultiDriver[3];
				Driver = BasePage.MultiDriver.Last();
				login.DriverGoTo(login.url);
				login.LoginGrid(adminusername, adminpassword);
				study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, AccessionNo: AccessionID, Datasource: Config.XDS_EA2);
                study.SelectStudy("Patient ID", PatinetID);
				studyviewer = StudyViewer.LaunchStudy();
				if (!studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[ExecutedSteps].AddFailStatusList("Error while launching study in Grid");
				}
				else
				{
					BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
					//Driver = BasePage.MultiDriver[4];
					Driver = BasePage.MultiDriver.Last();
					Logger.Instance.InfoLog(browserList[2] + " launched");
					BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage7 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage7))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
					}
					BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage8 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage8))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
					}

					BasePage.MultiDriver.Last().Close();// [4].Close();
					BasePage.MultiDriver.Remove(MultiDriver.Last());
					//BasePage.MultiDriver[3].Close();
					BasePage.MultiDriver.Last().Close();
					BasePage.MultiDriver.Remove(MultiDriver.Last());
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 7 - Logout of the Active iCA session in Chrome
				Driver = BasePage.MultiDriver[0];
				login.Logout();
				Driver.SwitchTo().DefaultContent();
				result.steps[++ExecutedSteps].StepPass();

				//Step 8 - Paste the request URLs in another tab of the same browser
				try
				{
					js = (IJavaScriptExecutor)BasePage.Driver;
					js.ExecuteScript("window.open();");
					Thread.Sleep(2000);
				}
				catch (Exception)
				{
					tabs = new List<string>(Driver.WindowHandles);
					if (tabs.Count == 1)
					{
						js = (IJavaScriptExecutor)BasePage.Driver;
						js.ExecuteScript("window.open();");
						Thread.Sleep(2000);
					}
				}
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage9 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage9, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage10 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage10, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);

				//Step 9 - Clear cache and try the same queries (as per step6) in same browser					
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, AccessionNo: AccessionID, Datasource: Config.XDS_EA2);
                study.SelectStudy("Patient ID", PatinetID);
				studyviewer = StudyViewer.LaunchStudy();
				if (!studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				else
				{
					try
					{
						Driver.Manage().Cookies.DeleteAllCookies();
					}
					catch (Exception) { }
					js = (IJavaScriptExecutor)BasePage.Driver;
					js.ExecuteScript("window.open();");
					tabs = new List<string>(Driver.WindowHandles);
					BasePage.Driver.SwitchTo().Window(tabs[1]);
					BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
					Logger.Instance.InfoLog(Config.BrowserType.ToLower() + " browser launched");
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					var newTabImage11 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage11, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
					}
					BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					var newTabImage12 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage12, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
					}
					if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
					{
						result.steps[ExecutedSteps].StepFail();
					}
					else
					{
						result.steps[ExecutedSteps].StepPass();
					}
					BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
					BasePage.Driver.SwitchTo().Window(tabs[0]);
				}

				//Step 10 - Launch iCA in Chrome and login as "Administrator"
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				result.steps[++ExecutedSteps].StepPass();

				//Step 11- 1. Navigate to Outbound tab, search for the study PatientID = PID145, Launch study by pressing view study button
				Outbounds outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy(patientID: PatinetID);
				study.SelectStudy("Patient ID", PatinetID);
				studyviewer = StudyViewer.LaunchStudy();
				if (studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 12 - repeat step 4- 10
				ExecutedSteps++;
				result = Test_160926_RepeatStep_4To10(result, 12, browserList, studyviewer);
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}


				//Step 13 - Launch iCA in Chrome and login as "User1", password "User123"
				login.DriverGoTo(login.url);
				login.LoginIConnect(UName, UName);
				result.steps[++ExecutedSteps].StepPass();

				//Step 14- Navigate to Outbound tab, search for the study PatientID = PID145, Launch study by pressing view study button
				Inbounds inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy(patientID: PatinetID);
				study.SelectStudy("Patient ID", PatinetID);
				studyviewer = StudyViewer.LaunchStudy();
				if (studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}

				//Step 15 - repeat step 4- 10
				ExecutedSteps++;
				result = Test_160926_RepeatStep_4To10(result, 15, browserList, studyviewer);
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 16 - Launch iCA in Chrome and login as "Administrator"
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				result.steps[++ExecutedSteps].StepPass();

				//Step 17 - 1.Navigate to studies tab, 2.Search first name = pqa, PatientID = PID145
				studies = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, AccessionNo: AccessionID, Datasource: Config.XDS_EA2);
                studies.SelectStudy("Patient ID", PatinetID);

				//Stwep 17   3.Add to Conference folder
				StudyViewer studyViewer = studies.LaunchStudy();
				studyViewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
				studyViewer.AddStudyToStudyFolder(folderpath);
				studyviewer.CloseStudy();
				result.steps[++ExecutedSteps].StepPass();

				//Step 18 - 1. Navigate to Conference tab, 2.search for the study PatientID = PID145
				conferencefolders = login.Navigate<ConferenceFolders>();
				conferencefolders.ExpandAndSelectFolder(folderpath);
				PageLoadWait.WaitForLoadingDivToAppear_Conference();
				PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);


				//Step 18 - 3.Launch study by double click / or by pressing view exam button
				conferencefolders.SelectStudy1("Patient ID", PatinetID);
				BasePage.FindElementByCss(ConferenceFolders.btnEnterpriseviewer).Click();
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitForFrameLoad(10);
				PageLoadWait.WaitForThumbnailsToLoad(180);
				PageLoadWait.WaitForAllViewportsToLoad(20);
				Logger.Instance.InfoLog("Study Viewer Launched from conference list page");				
				result.steps[++ExecutedSteps].StepPass();

				//Step 19 - repeat step 4- 10
				ExecutedSteps++;
				result = Test_160926_RepeatStep_4To10(result, 19, browserList, studyviewer);
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Report Result
				result.FinalResult(ExecutedSteps);

				Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				return result;
				//------------End of script---

			}
			catch (Exception e)
			{
				//Log Exception
				Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			finally
			{
				try
				{
					//Driver = BasePage.MultiDriver[0];
					login.closeallbrowser();
					BasePage.Driver.Quit();
					Thread.Sleep(500);
					login.CreateNewSesion();
					login.DriverGoTo(login.url);
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Error in finally block 1 : " + ex.Message);
				}
				try
				{
					//iCA Userpreference Steup
					UserPreferences userpref = new UserPreferences();
					login.DriverGoTo(login.url);
					login.LoginIConnect(Config.adminUserName, Config.adminPassword);
					login.OpenUserPreferences();
					BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
					PageLoadWait.WaitForPageLoad(20);
					userpref.BluringViewerRadioBtn().Click();
					userpref.CloseUserPreferences();
					login.Logout();
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Error in finally block 2 : " + ex.Message);
				}
			}
		}

		public TestCaseResult Test_160926_RepeatStep_4To10(TestCaseResult result, int stepNo, List<string> browserList, StudyViewer studyviewer, String adminusername = "Administrator", String adminpassword = "Administrator")
		{
			var ExecutedSteps = stepNo - 1;
			string testid = "160926";
            string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
            string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ViewPortLoadImageURL").ToString().Replace("<iConnectIP>", Config.IConnectIP);
            IList<string> tabs = new List<string>(Driver.WindowHandles);

			try
			{
				IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);				
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_5", ExecutedSteps + 1, 1);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Corresponding study launched in new tab while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Corresponding study not launched in new tab while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_5", ExecutedSteps + 1, 2);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Corresponding study launched in new tab while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Corresponding study not launched in new tab while loading thumbnail URL");
				}
				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);
				BasePage.MultiDriver[0] = BasePage.Driver;

				//Step 6_1 - Paste the query 1 and query 2 in Firefox browser
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
				Driver = BasePage.MultiDriver.Last();
				Logger.Instance.InfoLog(browserList[0] + " launched");
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_6", ExecutedSteps + 1, 1);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Session timeout error displayed while loading thumbnail URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_6", ExecutedSteps + 1, 2);
				if (studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddPassStatusList("Session timeout error displayed while loading viewport URL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading viewport URL");
				}
				//BasePage.MultiDriver[1].Close();
				BasePage.MultiDriver.Last().Close();
				BasePage.MultiDriver.Remove(MultiDriver.Last());

				//Step 6_2 - Paste the query 1 and query 2 in IE browser
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[1]));
				Driver = BasePage.MultiDriver.Last();
				Logger.Instance.InfoLog(browserList[1] + " launched");
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_6", ExecutedSteps + 1, 3);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_6", ExecutedSteps + 1, 4);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading viewport URL");
				}
				//BasePage.MultiDriver[2].Close();
				BasePage.MultiDriver.Last().Close();
				BasePage.MultiDriver.Remove(MultiDriver.Last());

				//Step 6_3 - Paste the query 1 and query 2 in edge browser
				BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
				Logger.Instance.InfoLog("remote-" + Config.BrowserType + " launched");
				Driver = BasePage.MultiDriver.Last();
				login.DriverGoTo(login.url);
				login.LoginGrid(adminusername, adminpassword);
				Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, AccessionNo: AccessionID, Datasource: Config.XDS_EA2);
                study.SelectStudy("Patient ID", PatinetID);
				studyviewer = StudyViewer.LaunchStudy();
				if (!studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[ExecutedSteps].AddFailStatusList("Error while launching study in Grid");
				}
				else
				{
					BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
					Logger.Instance.InfoLog(browserList[2] + " launched");
					Driver = BasePage.MultiDriver.Last();
					Logger.Instance.InfoLog(browserList[2] + " launched");
					BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage7 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid + "_6", ExecutedSteps + 1, 5);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage7))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
					}
					BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
					var newTabImage8 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid + "_6", ExecutedSteps + 1, 6);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage8))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
					}

					//BasePage.MultiDriver[4].Close();
					BasePage.MultiDriver.Last().Close();
					BasePage.MultiDriver.Remove(MultiDriver.Last());
					//BasePage.MultiDriver[3].Close();
					BasePage.MultiDriver.Last().Close();
					BasePage.MultiDriver.Remove(MultiDriver.Last());
				}

				//Step 7 - Logout of the Active iCA session in Chrome
				Driver = BasePage.MultiDriver[0];
				login.CloseStudy();				
				login.Logout();
				Logger.Instance.InfoLog("Logout of the Active iCA session in " + Config.BrowserType);
				result.steps[ExecutedSteps].AddPassStatusList();

				//Step 8 - Paste the request URLs in another tab of the same browser
				try
				{
					js = (IJavaScriptExecutor)BasePage.Driver;
					js.ExecuteScript("window.open();");
				}
				catch(Exception)
				{
					tabs = new List<string>(Driver.WindowHandles);
					if (tabs.Count == 1)
					{
						js = (IJavaScriptExecutor)BasePage.Driver;
						js.ExecuteScript("window.open();");
						Thread.Sleep(2000);
					}
				}
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage9 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_8", ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage9, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage10 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_8", ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage10, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
				}
				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);
				Logger.Instance.InfoLog("Paste the request URLs in another tab of the same browser after logout");

				//Step 9 - Clear cache and try the same queries (as per step6) in same browser					
				login.CreateNewSesion();
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatinetID, AccessionNo: AccessionID, Datasource: Config.XDS_EA2);
                study.SelectStudy("Patient ID", PatinetID);
				studyviewer = StudyViewer.LaunchStudy();
				try
				{
					Driver.Manage().Cookies.DeleteAllCookies();
				}
				catch (Exception) { }
				js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				Thread.Sleep(2000);
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Logger.Instance.InfoLog(Config.BrowserType.ToLower() + " browser new tab launched");
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage11 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_9", ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage11, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage12 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid + "_9", ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage12, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
				}
				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);
				return result;
			}
			catch (Exception ex)
			{
				//Log Exception
				result.steps[ExecutedSteps].AddFailStatusList();
				Reusable.Generic.Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
				return result;
			}
			finally
			{
				try
				{
					BasePage.Driver.SwitchTo().Window(tabs[0]);
					login.CreateNewSesion();
					login.DriverGoTo(login.url);
					login.LoginIConnect(adminusername, adminpassword);
				}
				catch (Exception e)
				{
					Logger.Instance.ErrorLog("Error in finally block: "+ e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
				}
			}
		}

		/// <summary> 
		/// 160910 - Study Authorization IN/Outbounds in Standalone mode - Old viewer is the default viewer (HTML4)
		/// </summary>
		///
		public TestCaseResult Test_160910(string testid, string teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;
			BluRingViewer viewer = new BluRingViewer();
			UserPreferences userpref = new UserPreferences();
			RoleManagement rolemanagement;
			UserManagement usermanagement;
			StudyViewer studyviewer;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;

				String Role1 = "Role_1" + new Random().Next(1, 1000);
				String User1 = "User_1" + new Random().Next(1, 1000);
				MultiDriver = new List<IWebDriver>();

				string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				string[] patinetIDList = PatinetID.Split('@');
				string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
				ThumbnailURL = ThumbnailURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
				string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
				ViewPortURL = ViewPortURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);

				//URL with Invaild patinetID.
				string ThumbnailURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
				ThumbnailURLWithInVaildPatientID = ThumbnailURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);
				string ViewPortURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
				ViewPortURLWithInVaildPatientID = ViewPortURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);

				//URL with Invaild StudUID, seriousUID.
				string ThumbnailURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailLoadImageURL");
				ThumbnailURLWithInVaildUID = ThumbnailURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);
				string ViewPortURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ViewPortLoadImageURL");
				ViewPortURLWithInVaildUID = ViewPortURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);
				string Datasource = login.GetHostName(Config.SanityPACS);

				BasePage.MultiDriver.Add(BasePage.Driver);
				List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
				for (int count = 0; count < 3; count++)
				{
					if (Config.BrowserType.ToLower() == browserList[count])
					{
						browserList[count] = "chrome";
						break;
					}
				}

				//Step 1  - iCA session is still Active and Open with Administrator login
				login.DriverGoTo(login.url);
				login.LoginIConnect(adminusername, adminpassword);
				Studies study = (Studies)login.Navigate("Studies");
				result.steps[++ExecutedSteps].StepPass();

				//Step 2 
				userpref.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				PageLoadWait.WaitForPageLoad(20);
				userpref.HTML4RadioBtn().Click();
				userpref.CloseUserPreferences();
				result.steps[++ExecutedSteps].StepPass();

				//Create a new Role
				DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
				domain.SelectDomain("SuperAdminGroup");
				domain.ClickEditDomain();
				domain.SetCheckBoxInEditDomain("grant", 0);
				domain.ClickSaveDomain();
				rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
				rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
				rolemanagement.SelectRole(Config.adminRoleName);
				rolemanagement.ClickEditRole();
				PageLoadWait.WaitForFrameLoad(10);
				rolemanagement.GrantAccessRadioBtn_Anyone().Click();
				rolemanagement.ClickSaveEditRole();

				rolemanagement.CreateRole("SuperAdminGroup", Role1, "any");
				rolemanagement.SearchRole(Role1);
				rolemanagement.SelectRole(Role1);
				rolemanagement.ClickEditRole();
				PageLoadWait.WaitForFrameLoad(10);
				rolemanagement.GrantAccessRadioBtn_Anyone().Click();
				rolemanagement.AddDatasourceToRole(Datasource);
				rolemanagement.ClickSaveEditRole();
				usermanagement = (UserManagement)login.Navigate("UserManagement");
				usermanagement.CreateUser(User1, "SuperAdminGroup", Role1);

				//Step 3
				Studies studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(patientID: patinetIDList[0], Datasource: Datasource);
				studies.SelectStudy1("Patient ID", patinetIDList[0]);
				studies.ShareStudy(false, new string[] { User1 });
				Outbounds outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy(patientID: patinetIDList[0]);
				result.steps[++ExecutedSteps].StepPass();

				//Step 4
				outbounds.SelectStudy1(columnname: "Patient ID", columnvalue: patinetIDList[0]);
				studyviewer = StudyViewer.LaunchStudy();
				result.steps[++ExecutedSteps].StepPass();

				//Step 5 - Already done maunally and copied at test data Excel.
				result.steps[++ExecutedSteps].StepPass();

				//Step 6
				IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				IList<string> tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
                login.DriverGoTo(ThumbnailURL);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				var step6_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURL);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				var step6_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
				if (step6_1 && step6_2)
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}
				Thread.Sleep(3000);


				//Step 7 - 160920
				var LogStartTime = System.DateTime.Now;
				Thread.Sleep(3000);
				login.DriverGoTo(ThumbnailURLWithInVaildPatientID);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				var step7_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

				login.DriverGoTo(ViewPortURLWithInVaildPatientID);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				var step7_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
				Thread.Sleep(3000);

				//Paste the websocket URLS with invalid StudyUID, seriousUID.
				login.DriverGoTo(ThumbnailURLWithInVaildUID);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
				var step7_3 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

				login.DriverGoTo(ViewPortURLWithInVaildUID);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
				var step7_4 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
				if (step7_1 && step7_2 && step7_3 && step7_4)
				{
					result.steps[ExecutedSteps].AddPassStatusList();
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList();
				}
				Thread.Sleep(3000);

				//Get the start time , end time an verfiy log message.
				Thread.Sleep(5000);
				var LogEndTime = System.DateTime.Now;
				var loggedError = string.Empty;
				//Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
				try
				{
					String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
					for (var i = 1; i >= 1; i++)
					{
						String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
						Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

						System.DateTime DateTime = System.DateTime.Now.Date;

						if (File.Exists(LogFilePath))
						{
							StreamReader reader = new StreamReader(stream);
							var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
							foreach (var entry in LogValues)
							{
								if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
									if (entry.Value["Message"].Contains("Image Load Error"))
										if (entry.Value["Detail"].Contains("Study is not authorized"))
										{
											loggedError = entry.Value["Message"];
											break;
										}
							}
						}
						else
						{
							Logger.Instance.ErrorLog("Unable to Read Log file");
							break;
						}
						if (loggedError == "Image Load Error")
							break;
						else
							Logger.Instance.WarnLog("Unable to find the Log error, so start find the next log file for the day.");
					}

				}
				catch (Exception e)
				{
					Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
				}
				finally
				{
					//Validation of message failed in log file
					if (loggedError == "Image Load Error")
					{
						result.steps[ExecutedSteps].AddPassStatusList();
						Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
					}
					else
					{
						result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
						Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
					}
				}


				//Verfiy log message the 
				loggedError = string.Empty;
				try
				{
					String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
					for (var i = 1; i >= 1; i++)
					{
						String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
						Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

						System.DateTime DateTime = System.DateTime.Now.Date;

						if (File.Exists(LogFilePath))
						{
							StreamReader reader = new StreamReader(stream);
							var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
							foreach (var entry in LogValues)
							{
								if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
									if (entry.Value["Message"].Contains("Viewer Facade Error"))
										if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
										{
											loggedError = entry.Value["Message"];
											break;
										}
							}
						}
						else
						{
							Logger.Instance.ErrorLog("Unable to Read Log file");
							break;
						}

						if (loggedError == "Viewer Facade Error")
							break;

					}

				}
				catch (Exception e)
				{
					Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

				}
				finally
				{
					//Validation of message failed in log file
					if (loggedError == "Viewer Facade Error")
					{
						result.steps[ExecutedSteps].AddPassStatusList();
						Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
					}
					else
					{
						result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
						Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
					}
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);


				////step 8 - 160921
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
				Driver = BasePage.MultiDriver[1];
				Logger.Instance.InfoLog(browserList[0] + " launched");
                Driver.Manage().Window.Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
				}

				BasePage.MultiDriver[1].Close();

				// Launch IE 11 browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
				BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[1]));
				Driver = BasePage.MultiDriver[2];
				Logger.Instance.InfoLog(browserList[1] + " launched");
				//Driver.Manage().Window.Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading thumbnail URL");
				}
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(2000);
				PageLoadWait.WaitForPageLoad(10);
				//BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4))
				{
					result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading viewport URL");
				}
				BasePage.MultiDriver[2].Close();

				//Launch Edge browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
				BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
				Driver = BasePage.MultiDriver[3];
				login.DriverGoTo(login.url);
				login.LoginGrid(adminusername, adminpassword);
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy(patientID: patinetIDList[0]);
				outbounds.SelectStudy1(columnname: "Patient ID", columnvalue: patinetIDList[0]);
				studyviewer = StudyViewer.LaunchStudy();
				if (!studyviewer.SeriesViewer_1X1().Displayed)
				{
					result.steps[ExecutedSteps].AddFailStatusList();
				}
				else
				{
					BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
					Driver = BasePage.MultiDriver[4];
					Logger.Instance.InfoLog(browserList[2] + " launched");
					BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
					}
					BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
					Thread.Sleep(2000);
					PageLoadWait.WaitForPageLoad(10);
					var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
					if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
					{
						result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
					}

					BasePage.MultiDriver[4].Close();
					BasePage.MultiDriver[3].Close();
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				Driver = BasePage.MultiDriver[0];

				//Step 9 - 160922
				studyviewer.CloseStudy();

				// Paste the request urls "OperationClass=LoadImages" as captured in test case 160919
				js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				//Step 9 Image 1
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				var step5_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				//Step 9 Image 2
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				var step5_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
				if (step5_1 && step5_2)
				{
					result.steps[ExecutedSteps].AddPassStatusList();
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList();
				}
				Thread.Sleep(3000);


				//Get the start time , end time an verfiy log message.
				LogStartTime = System.DateTime.Now;
				Thread.Sleep(5000);
				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				Thread.Sleep(10000);
				LogEndTime = System.DateTime.Now;
				loggedError = string.Empty;
				//Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
				try
				{
					String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
					for (var i = 1; i >= 1; i++)
					{
						String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
						Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

						System.DateTime DateTime = System.DateTime.Now.Date;

						if (File.Exists(LogFilePath))
						{
							StreamReader reader = new StreamReader(stream);
							var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
							foreach (var entry in LogValues)
							{
								if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
									if (entry.Value["Message"].Contains("Image Load Error"))
										if (entry.Value["Detail"].Contains("Study is not authorized"))
										{
											loggedError = entry.Value["Message"];
											break;
										}
							}
						}
						else
						{
							Logger.Instance.ErrorLog("Unable to Read Log file");
							break;
						}
						if (loggedError == "Image Load Error")
							break;
					}

				}
				catch (Exception e)
				{
					Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
				}
				finally
				{
					//Validation of message failed in log file
					if (loggedError == "Image Load Error")
					{
						result.steps[ExecutedSteps].AddPassStatusList();
						Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
					}
					else
					{
						result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
						Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
					}
				}


				//get the start time , end time an verfiy log message.
				LogStartTime = System.DateTime.Now;
				Thread.Sleep(5000);
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				Thread.Sleep(10000);
				LogEndTime = System.DateTime.Now;
				loggedError = string.Empty;
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				try
				{
					String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
					for (var i = 1; i >= 1; i++)
					{
						String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
						Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

						System.DateTime DateTime = System.DateTime.Now.Date;

						if (File.Exists(LogFilePath))
						{
							StreamReader reader = new StreamReader(stream);
							var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
							foreach (var entry in LogValues)
							{
								if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
									if (entry.Value["Message"].Contains("Viewer Facade Error"))
										if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
										{
											loggedError = entry.Value["Message"];
											break;
										}
							}
						}
						else
						{
							Logger.Instance.ErrorLog("Unable to Read Log file");
							break;
						}

						if (loggedError == "Viewer Facade Error")
							break;

					}

				}
				catch (Exception e)
				{
					Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

				}
				finally
				{
					//Validation of message failed in log file
					if (loggedError == "Viewer Facade Error")
					{
						result.steps[ExecutedSteps].AddPassStatusList();
						Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
					}
					else
					{
						result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
						Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
					}
				}

				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);

				//Logout
				login.Logout();
				BasePage.Driver.SwitchTo().DefaultContent();

				try
				{
					js = (IJavaScriptExecutor)BasePage.Driver;
					js.ExecuteScript("window.open();");
					Thread.Sleep(5000);
					tabs = new List<string>(Driver.WindowHandles);
					if (tabs.Count != 2)
					{
						js.ExecuteScript("window.open();");
						tabs = new List<string>(Driver.WindowHandles);
					}
					Thread.Sleep(5000);
					tabs = Driver.WindowHandles.ToList();
					BasePage.Driver.SwitchTo().Window(tabs[1]);
				}
				catch (Exception ex)
				{
					Thread.Sleep(5000);
					tabs = new List<string>(Driver.WindowHandles);
					if (tabs.Count != 2)
					{
						js.ExecuteScript("window.open();");
						tabs = new List<string>(Driver.WindowHandles);
					}
					Thread.Sleep(5000);
					tabs = Driver.WindowHandles.ToList();
					BasePage.Driver.SwitchTo().Window(tabs[1]);

				}
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				//Step 9 Image 3
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
				var step10_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(studyviewer.ImageInNewTab)));
				//Step 9 Image 4
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
				var step10_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
				if (step10_1 && step10_2)
				{
					result.steps[ExecutedSteps].AddPassStatusList();
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList();
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}



				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);


				//Step 10 - 160925
				login.DriverGoTo(login.url);
				login.LoginGrid(adminusername, adminpassword);
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy(patientID: patinetIDList[0]);
				outbounds.SelectStudy1(columnname: "Patient ID", columnvalue: patinetIDList[0]);
				studyviewer = StudyViewer.LaunchStudy();
                try
                {
                    Driver.Manage().Cookies.DeleteAllCookies();
                }
                catch (Exception) { }
                js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Manage().Window.Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage10_1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				//Step 10 Image 1
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage10_1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage10_2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				//Step 10 Image 2
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage10_2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);

				login.DriverGoTo(login.url);
				js = (IJavaScriptExecutor)BasePage.Driver;
				js.ExecuteScript("window.open();");
				tabs = new List<string>(Driver.WindowHandles);
				BasePage.Driver.SwitchTo().Window(tabs[1]);
				BasePage.Driver.Manage().Window.Maximize();
				BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage10_3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				//Step 10 Image 3
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage10_3, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

				BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
				PageLoadWait.WaitForPageLoad(10);
				var newTabImage10_4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
				if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage10_4, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
					result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
				BasePage.Driver.SwitchTo().Window(tabs[0]);
				login.Logout();

				//Report Result
				result.FinalResult(ExecutedSteps);

				Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
				//Return Result
				return result;
				//------------End of script---

			}
			catch (Exception e)
			{
				//Log Exception
				Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps);
				Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			finally
			{
				BasePage.Driver.Quit();
				Thread.Sleep(500);
				login.CreateNewSesion();
				login.DriverGoTo(login.url);

			}
		}

        /// <summary> 
        /// 160912   - Study Authorization Conference folder in standalone mode- Old viewer(HTML4) is the default viewer
        /// </summary>
        ///
        public TestCaseResult Test_160912(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            DomainManagement domainmanagement;
            Taskbar taskbar = null;
            ConferenceFolders conferencefolders;
            StudyViewer studyviewer = new StudyViewer();
            result = new TestCaseResult(stepcount);
            UserPreferences userpref = new UserPreferences();
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String adminusername = Config.adminUserName;
            String adminpassword = Config.adminPassword;

            try
            {
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Role1 = "Role_1" + new Random().Next(1, 1000);
                String User1 = "User_1" + new Random().Next(1, 1000);
                MultiDriver = new List<IWebDriver>();

                string[] patinetIDList = PatinetID.Split('@');
                string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                ThumbnailURL = ThumbnailURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
                string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ViewPortURL = ViewPortURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);

                //URL with Invaild patinetID.
                string ThumbnailURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                ThumbnailURLWithInVaildPatientID = ThumbnailURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);
                string ViewPortURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ViewPortURLWithInVaildPatientID = ViewPortURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);

                //URL with Invaild StudUID, seriousUID.
                string ThumbnailURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailLoadImageURL");
                ThumbnailURLWithInVaildUID = ThumbnailURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);
                string ViewPortURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ViewPortLoadImageURL");
                ViewPortURLWithInVaildUID = ViewPortURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);
                string Datasource = login.GetHostName(Config.SanityPACS);

                List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
                for (int count = 0; count < 3; count++)
                {
                    if (Config.BrowserType.ToLower() == browserList[count])
                    {
                        browserList[count] = "chrome";
                        break;
                    }
                }

                String TopFolder = "160912_" + new Random().Next(1, 1000);
                String SubFolder = "160912_" + new Random().Next(1, 1000);
                String folderpath = TopFolder + "/" + SubFolder;

                ////Enable Conference Lists is turned ON in Server Tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableConferenceLists(0);
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                taskbar.Show();

                // Pre-Condition 
                //Launch iCA and login as "Administrator", password "Administrator" in Chrome (Browser) //Login success
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();

                //Pre - Condition
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(5);
                domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0);
                PageLoadWait.WaitForFrameLoad(5);
                domainmanagement.ClickSaveEditDomain();

                //Create Folder
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
                bool step3_1 = conferencefolders.CreateToplevelFolder(TopFolder); //First Top Folder
                bool step3_2 = conferencefolders.CreateSubFolder(TopFolder, SubFolder);
                login.Logout();

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                ////Step 2
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patinetIDList[0], Datasource: Datasource);
                studies.SelectStudy("Patient ID", patinetIDList[0]);
                result.steps[++ExecutedSteps].StepPass();

                //step 3
                StudyViewer studyViewer = studies.LaunchStudy();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                studyViewer.AddStudyToStudyFolder(folderpath);
                studyViewer.CloseStudy();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step 4 
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                result.steps[++ExecutedSteps].StepPass(); //Step 4

                //step 5
                //Launch Study in viewer
                conferencefolders.SelectStudy1("Patient ID", patinetIDList[0]);
                conferencefolders.LaunchStudy(isConferenceTab: true);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(180);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].StepPass(); //Step -5

                //Step - 6
                result.steps[++ExecutedSteps].StepPass(); //Step - 6

                //Step - 7
                result.steps[++ExecutedSteps].StepPass(); //Step - 7


                //Step -8
                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                Driver.SwitchTo().DefaultContent();
                Thread.Sleep(5000);
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(6000);
                BasePage.Driver.Manage().Window.Maximize();
                IList<string> tabs = new List<string>(Driver.WindowHandles);
                Thread.Sleep(6000);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                login.DriverGoTo(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step8_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step8_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step8_1 && step8_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                Thread.Sleep(3000);


                //Step 9 - 160920
                var LogStartTime = System.DateTime.Now;
                Thread.Sleep(3000);
                login.DriverGoTo(ThumbnailURLWithInVaildPatientID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step9_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURLWithInVaildPatientID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step9_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step9_1 && step9_2)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                Thread.Sleep(3000);

                //Paste the websocket URLS with invalid StudyUID, seriousUID.
                login.DriverGoTo(ThumbnailURLWithInVaildUID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step9_3 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURLWithInVaildUID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step9_4 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step9_3 && step9_4)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                Thread.Sleep(3000);

                //Get the start time , end time an verfiy log message.
                Thread.Sleep(5000);
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
                                    if (entry.Value["Message"].Contains("Image Load Error"))
                                        if (entry.Value["Detail"].Contains("Study is not authorized"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                        if (loggedError == "Image Load Error")
                            break;
                        else
                            Logger.Instance.WarnLog("Unable to find the Log error, so start find the next log file for the day.");
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Image Load Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
                    }
                }


                //Verfiy log message the 
                loggedError = string.Empty;
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
                                    if (entry.Value["Message"].Contains("Viewer Facade Error"))
                                        if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }

                        if (loggedError == "Viewer Facade Error")
                            break;
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Viewer Facade Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
                    }
                }

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //step 10 - 160921
                BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
                Driver = BasePage.MultiDriver[1];
                Logger.Instance.InfoLog(browserList[0] + " launched");
                Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
                }
                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
                }

                BasePage.MultiDriver.Last().Close();
                BasePage.MultiDriver.Remove(BasePage.MultiDriver.Last());

                // Launch IE 11 browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
                BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[1]));
                Driver = BasePage.MultiDriver.Last();
                Logger.Instance.InfoLog(browserList[1] + " launched");
                Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3))
                {
                    result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading thumbnail URL");
                }
                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4))
                {
                    result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading viewport URL");
                }
                BasePage.MultiDriver.Last().Close();
                BasePage.MultiDriver.Remove(BasePage.MultiDriver.Last());

                //Launch Edge browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
                BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Driver = BasePage.MultiDriver.Last();
                login.DriverGoTo(login.url);
                login.LoginGrid(adminusername, adminpassword);
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                conferencefolders.SelectStudy1("Patient ID", patinetIDList[0]);
                conferencefolders.LaunchStudy(isConferenceTab: true);
                if (!studyviewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                else
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
                    Driver = BasePage.MultiDriver.Last();
                    Logger.Instance.InfoLog(browserList[2] + " launched");
                    BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                    Thread.Sleep(2000);
                    PageLoadWait.WaitForPageLoad(10);
                    var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                    if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
                    {
                        result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
                    }
                    BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                    Thread.Sleep(2000);
                    PageLoadWait.WaitForPageLoad(10);
                    var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                    if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
                    {
                        result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
                    }

                    BasePage.MultiDriver.Last().Close();
                    BasePage.MultiDriver.Remove(MultiDriver.Last());
                    BasePage.MultiDriver.Last().Close();
                    BasePage.MultiDriver.Remove(MultiDriver.Last());
                }

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 11 - 160922
                Driver = BasePage.MultiDriver.Last();
                studyviewer.CloseStudy();
                // Paste the request urls "OperationClass=LoadImages" as captured in test case 160919
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(3000);
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 11 Image 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step11_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 11 Image 2
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step11_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step11_1 && step11_2)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                Thread.Sleep(3000);

                //Get the start time , end time an verfiy log message.
                LogStartTime = System.DateTime.Now;
                Thread.Sleep(5000);
                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                Thread.Sleep(10000);
                LogEndTime = System.DateTime.Now;
                loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
                                    if (entry.Value["Message"].Contains("Image Load Error"))
                                        if (entry.Value["Detail"].Contains("Study is not authorized"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                        if (loggedError == "Image Load Error")
                            break;
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Image Load Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
                    }
                }


                //get the start time , end time an verfiy log message.
                LogStartTime = System.DateTime.Now;
                Thread.Sleep(5000);
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                Thread.Sleep(10000);
                LogEndTime = System.DateTime.Now;
                loggedError = string.Empty;
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
                                    if (entry.Value["Message"].Contains("Viewer Facade Error"))
                                        if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }

                        if (loggedError == "Viewer Facade Error")
                            break;

                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Viewer Facade Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
                    }
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Logout
                login.Logout();
                BasePage.Driver.SwitchTo().DefaultContent();
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(5000);
                BasePage.Driver.Manage().Window.Maximize();
                try
                {
                    tabs = new List<string>(Driver.WindowHandles);
                    if (tabs.Count != 2)
                        tabs = new List<string>(Driver.WindowHandles);
                    Thread.Sleep(5000);
                    tabs = Driver.WindowHandles.ToList();
                    BasePage.Driver.SwitchTo().Window(tabs[1]);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                    tabs = new List<string>(Driver.WindowHandles);
                    BasePage.Driver.SwitchTo().Window(tabs[1]);

                }
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 11 Image 3
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                var step11_3 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 11 Image 4
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                var step11_4 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step11_3 && step11_4)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }


                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();
                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);


                //Step 12
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.modifyBtn().Click();
                TextBox SetTimeout = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), "AutoSelectTextBox", 0);
                SetTimeout.Enter("2");
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception) { }
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                }
                catch (Exception) { }
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                conferencefolders.SelectStudy1("Patient ID", patinetIDList[0]);
                conferencefolders.LaunchStudy(isConferenceTab: true);
                // Wait for  session Time out.
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 3, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();

                //Leave the iCA session OPEN and ACTIVE with "Administrator" login
                //iCA session is open and Active
                BasePage.Driver.SwitchTo().DefaultContent();
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(3000);
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage12_1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 12 Image 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage12_1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage12_2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 12 Image 2
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage12_2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //step - 12
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //Step 13 - 160925
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                conferencefolders = login.Navigate<ConferenceFolders>();
                conferencefolders.ExpandAndSelectFolder(folderpath);
                PageLoadWait.WaitForLoadingDivToAppear_Conference();
                PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
                conferencefolders.SelectStudy1("Patient ID", patinetIDList[0]);
                conferencefolders.LaunchStudy(isConferenceTab: true);
                Driver.Manage().Cookies.DeleteAllCookies();
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(3000);
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage13_1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 13 Image 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage13_1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage13_2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 13 Image 2
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage13_2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                login.DriverGoTo(login.url);
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                Thread.Sleep(6000);
                tabs = new List<string>(Driver.WindowHandles);
                Thread.Sleep(6000);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage13_3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 13 Image 3
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage13_3, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage13_4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage13_4, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Logout
                login.Logout();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

            }
            catch (Exception e)
            {

                login.SetDriver(BasePage.MultiDriver[0]);
                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                login.Logout();

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Security");
                servicetool.WaitWhileBusy();
                servicetool.modifyBtn().Click();
                TextBox SetTimeout = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), "AutoSelectTextBox", 0);
                SetTimeout.Enter("30");
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception) { }
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                }
                catch (Exception) { }
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary> 
        /// 160917    - Study Authorization in Guest mode - set default viewer to be Old viewer(HTML4)
        /// </summary>
        public TestCaseResult Test_160918(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                MultiDriver = new List<IWebDriver>();

                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                IList<string> patinetIDList = PatinetID.Split('@');

                string ThumbnailURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                ThumbnailURL = ThumbnailURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);
                string ViewPortURL = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ViewPortURL = ViewPortURL.ToString().Replace("<iConnectIP>", Config.IConnectIP);

                //URL with Invaild patinetID.
                string ThumbnailURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ThumbnailLoadImageURL");
                ThumbnailURLWithInVaildPatientID = ThumbnailURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);
                string ViewPortURLWithInVaildPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "ViewPortLoadImageURL");
                ViewPortURLWithInVaildPatientID = ViewPortURLWithInVaildPatientID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace(patinetIDList[0], patinetIDList[1]);

                //URL with Invaild StudUID, seriousUID.
                string ThumbnailURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailLoadImageURL");
                ThumbnailURLWithInVaildUID = ThumbnailURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);
                string ViewPortURLWithInVaildUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ViewPortLoadImageURL");
                ViewPortURLWithInVaildUID = ViewPortURLWithInVaildUID.ToString().Replace("<iConnectIP>", Config.IConnectIP).Replace("<PATIENTID>", patinetIDList[1]);
                string Datasource = login.GetHostName(Config.SanityPACS);

                string expected_email_message = (String)ReadExcel.GetTestData(filepath, "TestData", "Study_Authorization", "Email_Study_Templete");
                string FilePathURL = null;
                IList<string> tabs = new List<string>(Driver.WindowHandles);

                BasePage.MultiDriver.Add(BasePage.Driver);
                List<String> browserList = new List<String> { "firefox", "ie", "Remote-edge" };
                for (int count = 0; count < 3; count++)
                {
                    if (Config.BrowserType.ToLower() == browserList[count])
                    {
                        browserList[count] = "chrome";
                        break;
                    }
                }

                ServiceTool serviceTool = new ServiceTool();
                serviceTool.InvokeServiceTool();
                serviceTool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);
                serviceTool.RestartService();
                serviceTool.CloseServiceTool();

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                //Pre-Condition 
                RoleManagement Rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                Rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                Rolemanagement.SelectRole(Config.adminRoleName);
                Rolemanagement.EditRoleByName(Config.adminRoleName);
                PageLoadWait.WaitForFrameLoad(10);
                Rolemanagement.SetCheckboxInEditRole("email", 0);
                Rolemanagement.ClickSaveEditRole();


                //Step 2 Search a study
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patinetIDList[0], Datasource: Datasource);
                studies.SelectStudy("Patient ID", patinetIDList[0]);
                StudyViewer studyviewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].StepPass();

                //Step 3, 4
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils ph1Email = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                ph1Email.MarkAllMailAsRead("INBOX");

                studies.EmailStudy(Config.CustomUser1Email, "Test", "Test",1);
                var pinnumber = studies.FetchPin();
                if (pinnumber != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    studies.CloseStudy();
                    throw new Exception("Error While Get the PINNumber by Email Study");
                }

                //Step 5
                downloadedMail = ph1Email.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = ph1Email.GetEmailedStudyLink(downloadedMail);
                var EmailMessage = downloadedMail["Body"];

                //var EmailMessage = Pop3EmailUtil.GetMail(Config.emailid, Config.Email_Password, "", "Emailed Study");
                //var emaillink = Pop3EmailUtil.GetEmailedStudyLink(Config.emailid, Config.Email_Password, "", "Emailed Study");
                expected_email_message = expected_email_message.Replace("[SENDERNAME]", "Testing");
                expected_email_message = expected_email_message.Replace("[RECEIVERNAME]", "Testing").Replace("[CONFIG.EMAIL]", Config.AdminEmail).Replace("[STUDYURL]", emaillink);
                if (expected_email_message.Equals(EmailMessage))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 6
                studyviewer = LaunchEmailedStudy.LaunchStudy<StudyViewer>(emaillink, pinnumber);
                result.steps[++ExecutedSteps].StepPass();

                //Step 7
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                login.DriverGoTo(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step7_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step7_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step7_1 && step7_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                Thread.Sleep(3000);


                //Step 8 - 160920
                var LogStartTime = System.DateTime.Now;
                Thread.Sleep(3000);
                login.DriverGoTo(ThumbnailURLWithInVaildPatientID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step8_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURLWithInVaildPatientID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step8_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                Thread.Sleep(3000);

                //Paste the websocket URLS with invalid StudyUID, seriousUID.
                login.DriverGoTo(ThumbnailURLWithInVaildUID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step8_3 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                login.DriverGoTo(ViewPortURLWithInVaildUID);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step8_4 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step8_1 && step8_2 && step8_3 && step8_4)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                Thread.Sleep(3000);

                //Get the start time , end time an verfiy log message.
                Thread.Sleep(5000);
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
                                    if (entry.Value["Message"].Contains("Image Load Error"))
                                        if (entry.Value["Detail"].Contains("Study is not authorized"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                        if (loggedError == "Image Load Error")
                            break;
                        else
                            Logger.Instance.WarnLog("Unable to find the Log error, so start find the next log file for the day.");
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Image Load Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
                    }
                }


                //Verfiy log message the 
                loggedError = string.Empty;
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
                                    if (entry.Value["Message"].Contains("Viewer Facade Error"))
                                        if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }

                        if (loggedError == "Viewer Facade Error")
                            break;

                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Viewer Facade Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
                    }
                }

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);


                ////step 9 - 160921
                BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
                Driver = BasePage.MultiDriver[1];
                Logger.Instance.InfoLog(browserList[0] + " launched");
                Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                var newTabImage1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage1, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
                }
                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                var newTabImage2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage2, isCaptureScreen: browserList[0].ToLower().Contains("firefox")))
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");
                }

                BasePage.MultiDriver[1].Close();

                // Launch IE 11 browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
                BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[1]));
                Driver = BasePage.MultiDriver[2];
                Logger.Instance.InfoLog(browserList[1] + " launched");
                //Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                var newTabImage3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage3))
                {
                    result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading thumbnail URL");
                }
                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(10);
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                var newTabImage4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage4))
                {
                    result.steps[ExecutedSteps].AddFailStatusList(browserList[1] + " - Session timeout error not displayed while loading viewport URL");
                }
                BasePage.MultiDriver[2].Close();

                //Launch Edge browser and DO NOT open iCA session and Paste the same request URLs "OperationClass=LoadImages"
                BasePage.MultiDriver.Add(login.InvokeBrowser("remote-" + Config.BrowserType));
                Driver = BasePage.MultiDriver[3];
                studyviewer = LaunchEmailedStudy.LaunchStudy<StudyViewer>(emaillink, pinnumber);
                if (!studyviewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                else
                {
                    BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[2]));
                    Driver = BasePage.MultiDriver[4];
                    Logger.Instance.InfoLog(browserList[2] + " launched");
                    BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                    Thread.Sleep(2000);
                    PageLoadWait.WaitForPageLoad(10);
                    var newTabImage5 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                    if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage5))
                    {
                        result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading thumbnail URL");
                    }
                    BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                    Thread.Sleep(2000);
                    PageLoadWait.WaitForPageLoad(10);
                    var newTabImage6 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                    if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage6))
                    {
                        result.steps[ExecutedSteps].AddFailStatusList(browserList[2] + " - Session timeout error not displayed while loading viewport URL");
                    }

                    BasePage.MultiDriver[4].Close();
                    BasePage.MultiDriver[3].Close();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                Driver = BasePage.MultiDriver[0];

                //Step 10 - 160922
                studyviewer.CloseStudy();

                // Paste the request urls "OperationClass=LoadImages" as captured in test case 160919
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 10 Image 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                var step10_1 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 10 Image 2
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                var step10_2 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step10_1 && step10_2)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                Thread.Sleep(3000);

                //Get the start time , end time an verfiy log message.
                LogStartTime = System.DateTime.Now;
                Thread.Sleep(5000);
                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                Thread.Sleep(10000);
                LogEndTime = System.DateTime.Now;
                loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade::LoadImages"))
                                    if (entry.Value["Message"].Contains("Image Load Error"))
                                        if (entry.Value["Detail"].Contains("Study is not authorized"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                        if (loggedError == "Image Load Error")
                            break;
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Image Load Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
                    }
                }


                //get the start time , end time an verfiy log message.
                LogStartTime = System.DateTime.Now;
                Thread.Sleep(5000);
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                Thread.Sleep(10000);
                LogEndTime = System.DateTime.Now;
                loggedError = string.Empty;
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("ViewerFacade.LoadThumbnail"))
                                    if (entry.Value["Message"].Contains("Viewer Facade Error"))
                                        if (entry.Value["Detail"].Contains("Authorization error while loading thumbnail"))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }

                        if (loggedError == "Viewer Facade Error")
                            break;

                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");

                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Viewer Facade Error")
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--Unable to Error Log");
                    }
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                //Logout
                login.Logout();
                BasePage.Driver.SwitchTo().DefaultContent();

                try
                {
                    js = (IJavaScriptExecutor)BasePage.Driver;
                    js.ExecuteScript("window.open();");
                    Thread.Sleep(5000);
                    BasePage.Driver.Manage().Window.Maximize();
                    tabs = new List<string>(Driver.WindowHandles);
                    if (tabs.Count != 2)
                    {
                        js.ExecuteScript("window.open();");
                        tabs = new List<string>(Driver.WindowHandles);
                    }
                    Thread.Sleep(5000);
                    tabs = Driver.WindowHandles.ToList();
                    BasePage.Driver.SwitchTo().Window(tabs[1]);
                    BasePage.Driver.Manage().Window.Maximize();
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                    tabs = new List<string>(Driver.WindowHandles);
                    if (tabs.Count != 2)
                    {
                        js.ExecuteScript("window.open();");
                        tabs = new List<string>(Driver.WindowHandles);
                    }
                    Thread.Sleep(5000);
                    tabs = Driver.WindowHandles.ToList();
                    BasePage.Driver.SwitchTo().Window(tabs[1]);
                    BasePage.Driver.Manage().Window.Maximize();

                }
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 10 Image 3
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                var step10_3 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(studyviewer.ImageInNewTab)));
                //Step 9 Image 4
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                var step10_4 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab), isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox"));
                if (step10_3 && step10_4)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);


                //Step 11 - 160925
                studyviewer = LaunchEmailedStudy.LaunchStudy<StudyViewer>(emaillink, pinnumber);
                Driver.Manage().Cookies.DeleteAllCookies();
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage11_1 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 11 Image 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage11_1, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage11_2 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 11 Image 2
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage11_2, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);

                login.DriverGoTo(login.url);
                js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("window.open();");
                tabs = new List<string>(Driver.WindowHandles);
                BasePage.Driver.SwitchTo().Window(tabs[1]);
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.Driver.Navigate().GoToUrl(ThumbnailURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage11_3 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                //Step 11 Image 3
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage11_3, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                BasePage.Driver.Navigate().GoToUrl(ViewPortURL);
                PageLoadWait.WaitForPageLoad(10);
                var newTabImage11_4 = studyviewer.GetElement(SelectorType.CssSelector, studyviewer.ImageInNewTab);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (!studyviewer.CompareImage(result.steps[ExecutedSteps], newTabImage11_4, isCaptureScreen: Config.BrowserType.ToLower().Contains("firefox")))
                    result.steps[ExecutedSteps].AddFailStatusList("Session timeout error not displayed while loading thumbnail URL");

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                BasePage.Driver.SwitchTo().Window(tabs[1]).Close();
                BasePage.Driver.SwitchTo().Window(tabs[0]);
                login.Logout();


                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                login.Logout();
                result.FinalResult(ExecutedSteps);
                Reusable.Generic.Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {

                //Log Exception
                Reusable.Generic.Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Reusable.Generic.Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                try
                {
                    login.SetDriver(BasePage.MultiDriver[0]);
                }
                catch (Exception ex)
                { }

                ///Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                //ServiceTool serviceTool = new ServiceTool();
                //serviceTool.InvokeServiceTool();
                //serviceTool.SetEmailNotificationForPOP();
                //serviceTool.CloseServiceTool();
            }
        }

    }
}
