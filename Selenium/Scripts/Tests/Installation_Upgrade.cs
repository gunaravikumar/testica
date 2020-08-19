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
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using System.Diagnostics;
using System.Xml.Serialization;
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.iCAInstaller;


namespace Selenium.Scripts.Tests
{
    class Installation_Upgrade : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }      
        public EHR ehr { get; set; }
        public iCAInstaller icainstaller { get; set; }       
        public WpfObjects wpfobject { get; set; }
        public ServiceTool servicetool { get; set; }

        //SuperAdminGroup
        String TestRoleSAG1 = "RoleSAG1_641_" + new Random().Next(1, 10000);
        //TestDomainA
        String TestdomainA = "DomainA_641_" + new Random().Next(1, 10000);
        String TestdomainAdminA = "DomainAdminA_641_" + new Random().Next(1, 10000);
        String TestRoleA1 = "RoleA1_641_" + new Random().Next(1, 10000);
        //TestDomainB
        String TestdomainB = "DomainB_641_" + new Random().Next(1, 10000);
        String TestdomainAdminB = "DomainAdminB_641_" + new Random().Next(1, 10000);
        String TestRoleB1 = "RoleB1_641_" + new Random().Next(1, 10000);
        String TestuserB1email = Config.emailid;
        //TestDomainC
        String TestdomainC = "DomainC_641_" + new Random().Next(1, 10000);
        String TestdomainAdminC = "DomainAdminC_641_" + new Random().Next(1, 10000);




        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Installation_Upgrade(String classname)
        {
            login = new Login();           
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";            
            wpfobject = new WpfObjects();            
            icainstaller = new iCAInstaller();            
            servicetool = new ServiceTool();
            ehr = new EHR();

        }

        /// <summary>
        /// Installation_Upgrade - Service Tool setup
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27635(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            string licensefilepath = Config.licensefilepath;
            ServiceTool servicetool = new ServiceTool();
            Studies studies = null;
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement = null;
            UserPreferences userPreferences = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                ////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////

                //Pre-condition for fresh install
                taskbar = new Taskbar();
                taskbar.Hide();
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                icainstaller.installiCA();
                taskbar.Show();

                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String UserName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Username");
                String Password = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSources");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String ConfigFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                String licencePath = ConfigFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";

                //step 1
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddEADatasource(Config.EA1, Config.EA1AETitle, "99", dataSourceName: Datasource);
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                bool datasourceAdded = wpfobject.VerifyIfTextExists(Datasource);
                if (datasourceAdded)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Datasource is not added in Service tool");
                }

                //step 2
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    servicetool.AddLicenseInServiceTool(licencePath);
                    ExecutedSteps++;
                }
                else
                {
                    string licenseAdded = servicetool.AddLicenseInConfigTool();
                    if (licenseAdded != null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("License is not added in Service tool");
                    }
                }

                //step 3
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToEnableFeatures();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }
                else
                {
                    bool studyAttachEnabled = servicetool.EnableStudyAttachements();
                    servicetool.RestartIISandWindowsServices();
                    if (studyAttachEnabled)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study attachment is not enabled in Service tool");
                    }
                }
                servicetool.RestartIIS();
                servicetool.CloseServiceTool();
                taskbar.Show();

                //step 4
                login.ChangeAttributeValue(Config.WebConfigPath, "configuration/appSettings", "Application.Culture", "en-US,zh-TW", true);
                string val = login.GetNodeValue(Config.WebConfigPath, "Application.Culture", true);
                if (val == "en-US,zh-TW")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Attribute value not updated");
                }

                //step 5
                login.DriverGoTo(login.url);
                BasePage.Driver.SwitchTo().DefaultContent();
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

                //step 6
                Driver.FindElement(login.ConnectionTestTool()).Click();
                PageLoadWait.WaitForElementToDisplay(Driver.FindElement(login.Bandwidth()));
                bool connTime = Driver.FindElement(login.CurrentConnectionTime()).Displayed;
                bool bandWidth = Driver.FindElement(login.Bandwidth()).Displayed;
                if (connTime && bandWidth)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 7
                Driver.FindElement(login.CloseConnectionRating()).Click();
                connTime = Driver.FindElement(login.CurrentConnectionTime()).Displayed;
                bandWidth = Driver.FindElement(login.Bandwidth()).Displayed;
                if (!connTime && !bandWidth)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 8
                login.LoginIConnect(UserName, Password);
                bool errMsg = login.LoginErrorMsgLabel().Displayed;
                // need to check color of error message
                bool errMsgClr = Driver.FindElement(By.XPath("//*[@id='ctl00_LoginMasterContentPlaceHolder_ErrorMessage']")).Displayed;
                if (errMsg && errMsgClr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 9
                login.LoginIConnect(username, password);
                Boolean istabpresent = login.IsTabPresent("Domain Management");
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

                //step 10
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                domainmanagement.ConnectDataSource(Datasource);
                domainmanagement.ReceivingInstTxtBox().SendKeys(DomainName);
                domainmanagement.ClickSaveEditDomain();
                //domainmanagement.SearchDomain(DomainName);
                //domainmanagement.SelectDomain(DomainName);
                //domainmanagement.ClickEditDomain();
                bool data = domainmanagement.verifyDomainDatasources(DomainName, Datasource);
                //domainmanagement.ClickCloseEditDomain();
                if (data)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11
                studies = login.Navigate<Studies>();
                bool studySearch = studies.IsElementVisible(studies.StudySearch());
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                bool connRate = studies.IsElementVisible(login.ConnectionTestTool());
                PageLoadWait.WaitForElementToDisplay(Driver.FindElement(login.Bandwidth()));
                BasePage.Driver.SwitchTo().DefaultContent();
                if (!connRate && studySearch)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12
                studies = login.Navigate<Studies>();
                userPreferences = studies.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userPreferences.EnableConnectionTestTool().Click();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    userPreferences.BluringViewerRadioBtn().Click();
                }
                userPreferences.SavePreferenceBtn().Click();
                userPreferences.CloseUserPreferences();
                userPreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool connectionToolEnabled = userPreferences.EnableConnectionTestTool().Selected;
                userPreferences.CancelUserPreferences();
                studySearch = studies.IsElementVisible(studies.StudySearch());
                if (connectionToolEnabled && studySearch)
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

                //step 13
                domainmanagement = login.Navigate<DomainManagement>();
                studies = login.Navigate<Studies>();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
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

                //step 14
                Driver.FindElement(login.ConnectionTestTool()).Click();
                PageLoadWait.WaitForElementToDisplay(Driver.FindElement(login.Bandwidth()));
                connTime = Driver.FindElement(login.CurrentConnectionTime()).Displayed;
                bandWidth = Driver.FindElement(login.Bandwidth()).Displayed;
                if (connTime && bandWidth)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 15
                Driver.FindElement(login.CloseConnectionRating()).Click();
                connTime = studies.IsElementVisible(login.CurrentConnectionTime());
                bandWidth = studies.IsElementVisible(login.Bandwidth());
                if (!connTime && !bandWidth)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 16
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("last", LastName);
                studies.SelectStudy("Accession", Accession);
                bool step_16 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    studies.LaunchStudy();
                    StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                    StudyVw.DragMovement(StudyVw.SeriesViewer_1X2());
                    StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                    StudyVw.DragMovement(StudyVw.SeriesViewer_1X2());
                    StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                    StudyVw.DrawLineMeasurement(StudyVw.SeriesViewer_1X1(), 50, 100);
                    StudyVw.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                    StudyVw.DrawROI(StudyVw.SeriesViewer_1X2(), 200, 34, 400, 210, 190, 280, 150, 330);
                    StudyVw.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_16 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_2X2());
                    StudyVw.CloseStudy();
                }
                else
                {
                    var Bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    Bluringviewer.SelectViewerTool(BluRingTools.Pan);
                    Bluringviewer.ApplyTool_Pan();
                    Bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement);
                    Bluringviewer.ApplyTool_LineMeasurement();
                    Bluringviewer.ApplyTool_FlipHorizontal();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, Bluringviewer.Activeviewport));
                    Bluringviewer.CloseBluRingViewer();
                }
                if (step_16)
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


                //step 17
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 18
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 19
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                OnlineHelp onlinehelp = new OnlineHelp().OpenHelpandSwitchtoIT(0);
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
                if (onlinehelp.OnlineHelpVersion().Text.Contains(Config.prevbuildversion))
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
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);

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
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }

        /// <summary>
        /// Installation_Upgrade - iConnect Access uninstallation
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27636(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            string licensefilepath = Config.licensefilepath;
            ServiceTool servicetool = new ServiceTool();
            Studies studies = null;
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                ////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////

                //Pre-condition for fresh install
                taskbar = new Taskbar();
                taskbar.Hide();
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                icainstaller.installiCA();
                //taskbar.Show();

                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String UserName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Username");
                String Password = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSources");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //taskbar = new Taskbar();
                //taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddLicenseInConfigTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.AddEADatasource(Config.EA1, Config.EA1AETitle, "99", dataSourceName: Datasource);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                taskbar.Show();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                domainmanagement.ConnectDataSource(Datasource);
                domainmanagement.ReceivingInstTxtBox().SendKeys(DomainName);
                domainmanagement.ClickSaveEditDomain();
                login.Logout();

                //Files
                String file1 = "install.log";
                String file2 = "uninstall.log";

                //step 1
                ProcessStartInfo startInfo = new ProcessStartInfo("appwiz.cpl");
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                wpfobject.WaitForButtonExist("Programs and Features", "Organize", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.GetTextbox("IBM iConnect Access", 1).Click();
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("Uninstall", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                bool confirm = wpfobject.VerifyElement("CommandButton_6", "Yes");
                if (confirm)

                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not present");
                }

                //step 2
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("CommandButton_7");
                wpfobject.GetMainWindowByTitle("Programs and Features");
                bool uninstall = wpfobject.VerifyIfTextExists("IBM iConnect Access");
                if (uninstall)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA uninstalled");
                }

                //step 3
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.GetTextbox("IBM iConnect Access", 1).Click();
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("Uninstall", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("CommandButton_6");
                //wpfobject.WaitForButtonExist("IBM iConnect Access", "Cancel", 1);
                wpfobject.GetMainWindowByTitle("IBM iConnect Access");
                wpfobject.ClickButton("Cancel", 1);
                Thread.Sleep(6000);
                bool cancel = true;
                do
                {
                    Thread.Sleep(5000);
                    cancel = wpfobject.CheckWindowExists("IBM iConnect Access");
                }
                while (cancel);

                wpfobject.GetMainWindowByTitle("Programs and Features");
                uninstall = wpfobject.VerifyIfTextExists("IBM iConnect Access");
                if (uninstall)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA uninstalled");
                }

                //step 4
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                Boolean istabpresent = login.IsTabPresent("Domain Management");
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

                //step 5
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("last", LastName);
                studies.SelectStudy("Accession", Accession);
                studies.LaunchStudy();
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                StudyVw.DragMovement(StudyVw.SeriesViewer_1X2());
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                StudyVw.DragMovement(StudyVw.SeriesViewer_1X2());
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                StudyVw.DrawLineMeasurement(StudyVw.SeriesViewer_1X1(), 50, 100);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                StudyVw.DrawROI(StudyVw.SeriesViewer_1X2(), 200, 34, 400, 210, 190, 280, 150, 330);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.SeriesViewer_1X2());
                if (step_5)
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
                StudyVw.CloseStudy();
                login.Logout();

                //step 6
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.GetTextbox("IBM iConnect Access", 1).Click();
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("Uninstall", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("CommandButton_6");
                wpfobject.GetMainWindowByTitle("IBM iConnect Access");
                wpfobject.WaitForButtonExist("IBM iConnect Access", "OK", 1);
                wpfobject.GetMainWindowByTitle("IBM iConnect Access");
                wpfobject.ClickButton("OK", 1);
                Thread.Sleep(10000);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                uninstall = wpfobject.VerifyIfTextExists("IBM iConnect Access");
                if (!uninstall)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not uninstalled");
                }

                //step 7
                bool file1AfterUninstall = File.Exists(Config.iCAInstalledPath + Path.DirectorySeparatorChar + file1);
                bool file2AfterUninstall = File.Exists(Config.iCAInstalledPath + Path.DirectorySeparatorChar + file2);
                if (file1AfterUninstall && file2AfterUninstall)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Files not present after iCA uninstalled");
                }

                //step 8
                bool directoryExists = icainstaller.ListVirtualDirectories();
                if (!directoryExists)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA virtual directory is not removed from server");
                }

                //step 9
                bool service1 = wpfobject.ServiceStatus("ImagePrefetchService", "Stopped");
                bool service2 = wpfobject.ServiceStatus("ImageTransferService", "Stopped");
                bool service3 = wpfobject.ServiceStatus("MeaningfulUseService", "Stopped");
                bool service4 = wpfobject.ServiceStatus("Part10ImportService", "Stopped");
                if (!service1 && !service2 && !service3 && !service4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA service still running");
                }
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("Close");

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
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }

        /// <summary>
        /// Install to a folder with a space on a non-default partition - to be executed on server without SQL
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27639(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            string licensefilepath = Config.licensefilepath;
            ServiceTool servicetool = new ServiceTool();
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                ////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String SQLsaPassword = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SQL sa password");
                String DBNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SQL path");
                String[] DBName = DBNameList.Split(':');
                String SQLsaUserID = "sa";
                String SQLIP = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SQLIP");
                String SQLInstanceName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SQLInstanceName");

                //Pre-condition for fresh install
                taskbar = new Taskbar();
                taskbar.Hide();
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                taskbar.Show();

                //Fetch required Test data

                //step 1
                //taskbar = new Taskbar();
                //taskbar.Hide();
                //icainstaller.invokeiCAInstaller();
                //wpfobject.GetMainWindowByTitle(Installer_Name);
                //bool installer = wpfobject.VerifyIfTextExists("Select Installation Folder");
                //if (installer)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //    throw new Exception("iCA not present");
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 2
                //wpfobject.GetMainWindowByTitle(Installer_Name);
                //wpfobject.ClearText("10428");
                //wpfobject.SetText("10428", @"C:\Web Access D\");
                //wpfobject.ClickButton("10425");
                //bool cancel = wpfobject.GetButton("Cancel", 1).Enabled;
                //bool next = !wpfobject.GetButton("Next", 1).Enabled;
                //bool back = !wpfobject.GetButton("Back", 1).Enabled;
                ////need to handle database warning window
                //if (cancel && next && back)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //    throw new Exception("iCA not present");
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 3
                //wpfobject.GetMainWindowByTitle(Installer_Name);
                //wpfobject.ClickButton("10368");
                //wpfobject.WaitForButtonExist(Installer_Name, "10380");
                //wpfobject.ClickButton("10380");
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 4
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 5
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 6
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 7
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 8
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 9
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 10
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 11
                taskbar.Hide();
                icainstaller.invokeiCAFullUi();
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                bool wndw = wpfobject.VerifyIfTextExists(iCAInstaller.InstallBtn_Name);
                if (wndw)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA fullUI mode installation screen not displayed");
                }

                //step 12
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                //wpfobject.GetMainWindow(iCAInstaller.Installer_Name);
                wpfobject.UnSelectCheckBox("Windows Authentication", 1);
                bool UN = wpfobject.GetTextbox("DbUserName", 1).Enabled;
                bool PW = wpfobject.GetTextbox("DbPassword", 1).Enabled;
                if (UN && PW)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Username and password not enabled");
                }

                //step 13
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wpfobject.SetText("DbUserName", SQLsaUserID, 1);
                wpfobject.SetText("DbPassword", SQLsaPassword, 1);
                wpfobject.SetText("DbInstance Name", DBName[0], 1);
                wpfobject.ClickButton("Install", 1);
                wpfobject.WaitForButtonExist(iCAInstaller.Installer_Name, "Finish", 1);
                if (wpfobject.GetButton("Finish", 1).Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not installed");
                }
                wpfobject.ClickButton("Finish", 1);
                //step 14
                var dbutil = new DataBaseUtil("sqlserver", DataSourceIP: SQLIP);
                dbutil.ConnectSQLServerDB();
                IList<String> databases = dbutil.ExecuteQuery("select * from master.sys.databases;");
                bool IR = false;
                bool MU = false;
                foreach (string dbName in databases)
                {
                    if (dbName == "IRWSDB")
                    {
                        IR = true;
                        Logger.Instance.InfoLog(dbName + "exists" + result.steps[ExecutedSteps].description);
                    }
                    else if (dbName == "MU2")
                    {
                        MU = true;
                        Logger.Instance.InfoLog(dbName + "exists" + result.steps[ExecutedSteps].description);
                    }
                }
                if (IR && MU)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("database not created");
                }

                //step 15
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                UN = icainstaller.uninstalliCA(1);
                if (UN)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA uninstalled");
                }

                //step 16
                dbutil = new DataBaseUtil("sqlserver", DataSourceIP: SQLIP);
                dbutil.ConnectSQLServerDB();
                databases = dbutil.ExecuteQuery("select * from master.sys.databases;");
                IR = false;
                MU = false;
                foreach (string dbName in databases)
                {
                    if (dbName == "IRWSDB")
                    {
                        IR = true;
                        Logger.Instance.InfoLog(dbName + "exists" + result.steps[ExecutedSteps].description);
                    }
                    else if (dbName == "MU2")
                    {
                        MU = true;
                        Logger.Instance.InfoLog(dbName + "exists" + result.steps[ExecutedSteps].description);
                    }
                }
                if (IR && MU)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("database not deleted");
                }

                //step 17
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 18
                //icainstaller.installiCAFullUi();
                //wndw = wpfobject.VerifyWindowExist(iCAInstaller.Installer_Name);
                //if (wndw)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //    throw new Exception("iCA fullUI mode installation screen not displayed");
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 19
                //wpfobject.UnSelectCheckBox("10446");
                // UN = wpfobject.GetTextbox("10450").Enabled;
                // PW = wpfobject.GetTextbox("10454").Enabled;
                //if (UN && PW)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //    throw new Exception("Username and password not enabled");
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 20
                //wpfobject.SetText("10443", @"ICA-UP4-W2K8\TESTINST2");
                //bool text = wpfobject.VerifyTextExists("10443", @"ICA-UP4-W2K8\TESTINST2");
                //if(text)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //    throw new Exception("DB instance not updated");
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 21
                //wpfobject.SetText("10450", "sa");
                //wpfobject.SetText("10454", "hello");
                //wpfobject.ClickButton("10425");
                //wpfobject.WaitForButtonExist(iCAInstaller.Installer_Name, "10380");
                //if (wpfobject.GetButton("10380").Enabled)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //    throw new Exception("iCA not installed");
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 22
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 23
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 24
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step 25
                icainstaller.invokeiCAFullUi();
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wndw = wpfobject.VerifyIfTextExists(iCAInstaller.InstallBtn_Name);
                if (wndw)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA fullUI mode installation screen not displayed");
                }

                //step 26
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wpfobject.UnSelectCheckBox("Windows Authentication", 1);
                UN = wpfobject.GetTextbox("DbUserName", 1).Enabled;
                PW = wpfobject.GetTextbox("DbPassword", 1).Enabled;
                if (UN && PW)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Username and password not enabled");
                }

                //step 27
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wpfobject.SetText("DbInstance Name", DBName[1], 1);
                TextBox d = wpfobject.GetTextbox("DbInstance Name", 1);
                string DB = d.Text;
                if (DB.Equals(DBName[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("DB instance not updated");
                }

                //step 28
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wpfobject.SetText("DbUserName", SQLsaUserID, 1);
                wpfobject.SetText("DbPassword", SQLsaPassword, 1);
                wpfobject.ClickButton("Install", 1);
                wpfobject.WaitForButtonExist(iCAInstaller.Installer_Name, "Finish", 1);
                if (wpfobject.GetButton("Finish", 1).Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not installed");
                }
                wpfobject.ClickButton("Finish", 1);

                //step 29
                dbutil = new DataBaseUtil("sqlserver", InstanceName: SQLInstanceName, DataSourceIP: SQLIP);
                dbutil.ConnectSQLServerDB();
                databases = dbutil.ExecuteQuery("select * from master.sys.databases;");
                IR = false;
                MU = false;
                foreach (string dbName in databases)
                {
                    if (dbName == "IRWSDB")
                    {
                        IR = true;
                        Logger.Instance.InfoLog(dbName + "exists" + result.steps[ExecutedSteps].description);
                    }
                    else if (dbName == "MU2")
                    {
                        MU = true;
                        Logger.Instance.InfoLog(dbName + "exists" + result.steps[ExecutedSteps].description);
                    }
                }
                if (IR && MU)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("database not deleted");
                }
                taskbar.Show();
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                taskbar.Show();
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
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }

        /// <summary>
        /// Upgrade Merge iConnect WebAccess setup
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27641(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            string licensefilepath = Config.licensefilepath;
            ServiceTool servicetool = new ServiceTool();
            Studies studies = null;
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            UserPreferences userPreferences = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                ////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////

                //Pre-condition for fresh install
                taskbar = new Taskbar();
                taskbar.Hide();
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                taskbar.Show();

                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                //String UserName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Username");
                //String Password = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String DatasourceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSources");
                String[] Datasource = DatasourceList.Split(':');
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string[] datasources = null;
                String RefPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RefPhysician");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                //TestDomainB
                String TestuserB1 = RefPhysician + new Random().Next(1, 1000);
                String SABUserName = "SAB_" + TestdomainB;

                //step 1
                taskbar = new Taskbar();
                taskbar.Hide();
                string version41 = icainstaller.installiCA(0);
                if (version41.Equals(Config.prevbuildversion))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not present");
                }

                //step 2
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.AddEADatasource(Config.EA1, Config.EA1AETitle, "99", dataSourceName: Datasource[0]);
                servicetool.AddEADatasource(Config.EA91, Config.EA91AETitle, "100", dataSourceName: Datasource[1]);
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                bool datasource1Added = wpfobject.VerifyIfTextExists(Datasource[0]);
                bool datasource2Added = wpfobject.VerifyIfTextExists(Datasource[1]);
                if (datasource1Added && datasource2Added)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Datasource is not added in Service tool");
                }
                servicetool.RestartIISandWindowsServices();

                //step 3
                servicetool.AddLicenseInConfigTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ClickModifyButton();
                servicetool.EnableEmailStudy();
                servicetool.ClickApplyButtonFromTab();
                Thread.Sleep(1000);
                wpfobject.ClickOkPopUp();
                Thread.Sleep(2000);
                servicetool.EnableReports();
                servicetool.EnableStudyAttachements();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                taskbar.Show();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.CreateDomain(TestdomainA, TestdomainAdminA, Datasource[0]);
                domainmanagement.ClickSaveNewDomain();
                domainmanagement.SearchDomain(TestdomainA);
                domainmanagement.SelectDomain(TestdomainA);
                domainmanagement.ClickEditDomain();
                domainmanagement.ReportviewCB().Click();
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.SearchDomain(TestdomainA);
                domainmanagement.SelectDomain(TestdomainA);
                domainmanagement.ClickEditDomain();
                bool report1 = domainmanagement.ReportviewCB().Selected;
                bool attach1 = domainmanagement.AttachmentUploadCB().Selected;
                bool email1 = domainmanagement.EmailStudyCB().Selected;
                domainmanagement.ClickCloseEditDomain();
                domainmanagement.CreateDomain(TestdomainB, TestdomainAdminB, datasources);
                domainmanagement.ClickSaveNewDomain();
                domainmanagement.SearchDomain(TestdomainB);
                domainmanagement.SelectDomain(TestdomainB);
                domainmanagement.ClickEditDomain();
                domainmanagement.EmailStudyCB().Click();
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.SearchDomain(TestdomainB);
                domainmanagement.SelectDomain(TestdomainB);
                domainmanagement.ClickEditDomain();
                bool report2 = domainmanagement.ReportviewCB().Selected;
                bool email2 = domainmanagement.EmailStudyCB().Selected;
                bool attach2 = domainmanagement.AttachmentUploadCB().Selected;
                domainmanagement.ClickCloseEditDomain();
                domainmanagement.CreateDomain(TestdomainC, TestdomainAdminC, Datasource[0]);
                domainmanagement.ClickSaveNewDomain();
                domainmanagement.SearchDomain(TestdomainC);
                domainmanagement.SelectDomain(TestdomainC);
                domainmanagement.ClickEditDomain();
                bool report3 = domainmanagement.ReportviewCB().Selected;
                bool email3 = !domainmanagement.EmailStudyCB().Selected;
                bool attach3 = domainmanagement.AttachmentUploadCB().Selected;
                domainmanagement.ClickCloseEditDomain();
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainA, TestRoleA1, RoleCred: 99, GrantAccess: 0);
                rolemanagement.SelectDomainfromDropDown(TestdomainA);
                rolemanagement.SearchRole(TestRoleA1, TestdomainA);
                rolemanagement.SelectRole(TestRoleA1);
                rolemanagement.ClickEditRole();
                rolemanagement.AddFilterinRole("Patient Name", LastName);
                PageLoadWait.WaitForFrameLoad(5);
                rolemanagement.RoleSelfStudyFilter().Click();
                rolemanagement.ClickSaveEditRole();
                rolemanagement.SelectDomainfromDropDown(TestdomainA);
                rolemanagement.SearchRole(TestRoleA1, TestdomainA);
                rolemanagement.SelectRole(TestRoleA1);
                rolemanagement.ClickEditRole();
                bool studyFilter1 = rolemanagement.AccessFilterElement(1).Displayed;
                bool selfStudyFilter1 = rolemanagement.RoleSelfStudyFilter().Selected;
                rolemanagement.CloseRoleManagement();
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainB, TestRoleB1, RoleCred: 99, GrantAccess: 0);
                rolemanagement.SelectDomainfromDropDown(TestdomainB);
                rolemanagement.SearchRole(TestRoleB1, TestdomainB);
                rolemanagement.SelectRole(TestRoleB1);
                rolemanagement.ClickEditRole();
                rolemanagement.AddFilterinRole("Referring Physician", RefPhysician);
                PageLoadWait.WaitForFrameLoad(5);
                rolemanagement.ClickSaveEditRole();
                rolemanagement.SelectDomainfromDropDown(TestdomainB);
                rolemanagement.SearchRole(TestRoleB1, TestdomainB);
                rolemanagement.SelectRole(TestRoleB1);
                rolemanagement.ClickEditRole();
                bool studyFilter2 = rolemanagement.AccessFilterElement(1).Displayed;
                bool selfStudyFilter2 = !rolemanagement.RoleSelfStudyFilter().Selected;
                rolemanagement.CloseRoleManagement();
                if (!report1 && !email1 && report2 && email2 && attach2 && attach1 && report3 && email3 && attach3 && studyFilter1 && selfStudyFilter1 && studyFilter2 && selfStudyFilter2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Domain and/Role not created");
                }
                login.Logout();

                //step 4
                login.LoginIConnect(username, password);
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.SelectDomainFromDropdownList(TestdomainB);
                usermanagement.CreateSystemAdminUser(SABUserName, TestdomainB);
                bool userSAB = usermanagement.SearchUser(SABUserName);
                usermanagement.CreateUser(TestuserB1, TestRoleB1, 1, TestuserB1email);
                bool user = usermanagement.SearchUser(TestuserB1);
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.AddFilterinRole("Accession Number", Accession);
                PageLoadWait.WaitForFrameLoad(5);
                rolemanagement.ClickSaveRole();
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickEditRole();
                bool studyFilter3 = rolemanagement.AccessFilterElement(1).Displayed;
                rolemanagement.CloseRoleManagement();
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(DomainName, TestRoleSAG1, RoleCred: 99, GrantAccess: 0);
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SearchRole(TestRoleSAG1);
                rolemanagement.SelectRole(TestRoleSAG1);
                rolemanagement.ClickEditRole();
                rolemanagement.AddFilterinRole("Patient ID", PatientID);
                PageLoadWait.WaitForFrameLoad(5);
                rolemanagement.RoleSelfStudyFilter().Click();
                rolemanagement.ClickSaveRole();
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SearchRole(TestRoleSAG1);
                rolemanagement.SelectRole(TestRoleSAG1);
                rolemanagement.ClickEditRole();
                bool studyFilter4 = rolemanagement.AccessFilterElement(1).Displayed;
                selfStudyFilter2 = rolemanagement.RoleSelfStudyFilter().Selected;
                rolemanagement.CloseRoleManagement();
                if (studyFilter3 && studyFilter4 && selfStudyFilter2 && userSAB && user)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Users not created");
                }
                login.Logout();

                //step 5
                login.LoginIConnect(SABUserName, SABUserName);
                userPreferences = domainmanagement.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userPreferences.PNGRadioBtn().Click();
                userPreferences.SavePreferenceBtn().Click();
                userPreferences.CloseUserPreferences();
                userPreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool png1 = userPreferences.PNGRadioBtn().Selected;
                userPreferences.CancelPreferenceBtn().Click();
                studies = login.Navigate<Studies>();
                studies.ClearFields();
                studies.SelectAllDateAndData();
                studies.SearchStudy("last", LastName);
                studies.SelectStudy("Accession", Accession);
                bool studylisted1 = false;
                if (studies.SelectedStudyrow(Accession).Text == Accession)
                    studylisted1 = true;
                login.Logout();
                login.LoginIConnect(TestuserB1, TestuserB1);
                userPreferences = domainmanagement.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userPreferences.PNGRadioBtn().Click();
                userPreferences.SavePreferenceBtn().Click();
                userPreferences.CloseUserPreferences();
                userPreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool png2 = userPreferences.PNGRadioBtn().Selected;
                userPreferences.CancelPreferenceBtn().Click();
                //studies = login.Navigate<Studies>();
                studies.ClearFields();
                studies.SelectAllDateAndData();
                studies.SearchStudy("last", LastName);
                studies.SelectStudy("Accession", Accession);
                bool studylisted2 = false;
                if (studies.SelectedStudyrow(Accession).Text == Accession)
                    studylisted2 = true;
                login.Logout();
                if (png1 && studylisted1 && png2 && studylisted2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("User preferences not saved");
                }

                //step 6
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEncryption();
                servicetool.SetEncryptionEncryptionService();
                GroupBox GrpBx_6 = WpfObjects._application.GetWindows()[0].Get<GroupBox>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                Panel Pane_6 = wpfobject.GetUIItem<GroupBox, Panel>(GrpBx_6, 0);
                ListView LstVw_6 = wpfobject.GetUIItem<Panel, ListView>(Pane_6, "ListView");
                int encryption = LstVw_6.Items.Count;
                                if (encryption == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Encryption is added");
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
        /// UI Upgrading
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27642(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            string licensefilepath = Config.licensefilepath;
            ServiceTool servicetool = new ServiceTool();
            Studies studies = null;
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            UserPreferences userPreferences = null;
            UserManagement usermanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                ////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////////

                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String UserName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Username");
                String Password = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastName = LastNameList.Split(':');
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientID = PatientIDList.Split(':');
                string iCAversion = icainstaller.getiCAVersion();
                String DatasourceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSources");
                String[] Datasource = DatasourceList.Split(':');
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentPath");
                String RefPhysician = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RefPhysician");
                String SABUserName = "SAB_" + TestdomainB;
                String TestuserB1 = RefPhysician;

                //step 1
                taskbar = new Taskbar();
                taskbar.Hide();
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                wpfobject.StopService("iConnect Access MeaningfulUse Service");
                wpfobject.StopService("iConnect Access Image Pre-fetch Service");
                wpfobject.StopService("iConnect Access Part 10 Import Service");
                wpfobject.StopService("iConnect Access Image Transfer Service");
                icainstaller.invokeiCAInstaller();
                Thread.Sleep(60000);
                wpfobject.GetMainWindowByTitle(icainstaller.Upgrade_Wndw1);
                bool step1 = wpfobject.GetElement<Label>("65535").Name.StartsWith("Setup has detected an existing installation");
                //String Message = wpfobject.GetTextfromElement("65535", "").Trim();
                //string[] mes = Message.Split('.');
                //String messageText = "Setup has detected an existing installation";
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not present");
                }

                //step 2
                wpfobject.StopService("iConnect Access MeaningfulUse Service");
                wpfobject.StopService("iConnect Access Image Pre-fetch Service");
                wpfobject.StopService("iConnect Access Part 10 Import Service");
                wpfobject.StopService("iConnect Access Image Transfer Service");
                wpfobject.GetMainWindowByTitle(icainstaller.Upgrade_Wndw1);
                wpfobject.ClickButton("6");
                wpfobject.GetMainWindowByTitle(icainstaller.Upgrade_Wndw1);
                bool step2 = wpfobject.GetElement<Label>("65535").Name.StartsWith("Setup will automatically backup the configuration data");
                //Message = wpfobject.GetTextfromElement("65535", "").Trim();
                //mes = Message.Split(',');
                //messageText = "Setup will automatically backup the configuration data";
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
                    throw new Exception("iCA not present");
                }

                //step 3
                wpfobject.StopService("iConnect Access MeaningfulUse Service");
                wpfobject.StopService("iConnect Access Image Pre-fetch Service");
                wpfobject.StopService("iConnect Access Part 10 Import Service");
                wpfobject.StopService("iConnect Access Image Transfer Service");
                Kill_EXEProcess(iCAInstaller.W3WPEXE);
                wpfobject.GetMainWindowByTitle(icainstaller.Upgrade_Wndw1);
                wpfobject.GetElement<Button>("OK", 1).Click();
                wpfobject.WaitTillLoad();
                Thread.Sleep(90000);
                wpfobject.GetMainWindowByTitle(icainstaller.Upgrade_Wndw1);
                wpfobject.GetElement<Button>("OK", 1).Click();
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                //Handle Popup which appears sometimes
                try
                {
                    wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                    wpfobject.GetElement<Button>("Retry", 1).Click();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Popup not found: " + ex);
                }
                try
                {
                    wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                    wpfobject.GetElement<Button>("OK", 1).Click();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Popup not found: " + ex);
                }
                wpfobject.WaitForButtonExist(icainstaller.Upgrade_Wndw1, "OK", 1);
                if (icainstaller.getiCAVersion().Contains(Config.currbuildversion))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not upgraded");
                }
                taskbar.Show();

                //stpe 4
                result.steps[++ExecutedSteps].status = "Pass";

                //step 5
                login.LoginIConnect(TestdomainA, TestdomainA);
                bool TestdomainAlogd = login.LogoutBtn().Displayed;
                login.Logout();
                login.LoginIConnect(TestdomainB, TestdomainB);
                bool TestdomainBlogd = login.LogoutBtn().Displayed;
                login.Logout();
                login.LoginIConnect(TestuserB1, TestuserB1);
                bool TestuserB1logd = login.LogoutBtn().Displayed;
                userPreferences = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool png2 = userPreferences.PNGRadioBtn().Selected;
                userPreferences.CancelPreferenceBtn().Click();
                login.Logout();
                login.LoginIConnect(SABUserName, SABUserName);
                bool SABUserNamelogd = login.LogoutBtn().Displayed;
                userPreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool png1 = userPreferences.PNGRadioBtn().Selected;
                userPreferences.CancelPreferenceBtn().Click();
                login.Logout();
                login.LoginIConnect(TestdomainC, TestdomainC);
                bool TestdomainClogd = login.LogoutBtn().Displayed;
                login.Logout();
                if (TestdomainAlogd && TestdomainBlogd && TestuserB1logd && png2 && SABUserNamelogd && png1 && TestdomainClogd)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA not upgraded");
                }
                login.Logout();
                login.LoginIConnect(SABUserName, SABUserName);

                //step 6
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: LastName[0], Datasource: Datasource[0]);
                studies.SelectStudy("Accession", Accession);
                bool step7 = false;
                BluRingViewer bluViewer = null;
                bool step_6_1 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_6_1 = bluViewer.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport));
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_6_1 = StudyVw.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                }
                //StudyVw.CloseStudy();
                //studies.SearchStudy(LastName: LastName[1], Datasource: Datasource[1]);
                //studies.SelectStudy("last", LastName[1]);
                //StudyVw = studies.LaunchStudy();
                //bool step_6_2 = StudyVw.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step_6_1/* && step_6_2*/)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("study not listed");
                }

                //step 7
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw.NavigateToHistoryPanel();
                    StudyVw.NavigateTabInHistoryPanel("Attachment");
                    bool attach7 = StudyVw.UploadAttachment(AttachmentFilePath);
                    if (attach7)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("study not listed");
                    }
                    StudyVw.CloseStudy();
                }

                //step 8               
                login.Logout();
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                bool domain8_1 = domainmanagement.DomainExists(TestdomainA);
                bool domain8_2 = domainmanagement.DomainExists(TestdomainB);
                bool domain8_3 = domainmanagement.DomainExists(TestdomainC);
                if (domain8_1 && domain8_2 && domain8_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("study not listed");
                }

                //step 9
                domainmanagement.SearchDomain(TestdomainA);
                domainmanagement.SelectDomain(TestdomainA);
                domainmanagement.ClickEditDomain();
                bool report1 = domainmanagement.ReportviewCB().Selected;
                bool attach1 = domainmanagement.AttachmentUploadCB().Selected;
                bool email1 = domainmanagement.EmailStudyCB().Selected;
                domainmanagement.ClickCloseEditDomain();
                domainmanagement.SearchDomain(TestdomainB);
                domainmanagement.SelectDomain(TestdomainB);
                domainmanagement.ClickEditDomain();
                bool report2 = domainmanagement.ReportviewCB().Selected;
                bool email2 = domainmanagement.EmailStudyCB().Selected;
                bool attach2 = domainmanagement.AttachmentUploadCB().Selected;
                domainmanagement.ClickCloseEditDomain();
                domainmanagement.SearchDomain(TestdomainC);
                domainmanagement.SelectDomain(TestdomainC);
                domainmanagement.ClickEditDomain();
                bool report3 = domainmanagement.ReportviewCB().Selected;
                bool email3 = !domainmanagement.EmailStudyCB().Selected;
                bool attach3 = domainmanagement.AttachmentUploadCB().Selected;
                domainmanagement.ClickCloseEditDomain();
                if (!report1 && !email1 && report2 && email2 && attach2 && attach1 && report3 && email3 && attach3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Domain and/Role not created");
                }

                //step 10
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SelectDomainfromDropDown(TestdomainA);
                bool role1 = rolemanagement.RoleExists(TestRoleA1);
                bool role3 = rolemanagement.RoleExists(TestdomainAdminA);
                rolemanagement.SelectDomainfromDropDown(TestdomainB);
                bool role2 = rolemanagement.RoleExists(TestRoleB1);
                bool role4 = rolemanagement.RoleExists(TestdomainAdminB);
                rolemanagement.SelectDomainfromDropDown(TestdomainC);
                bool role5 = rolemanagement.RoleExists(TestdomainAdminC);
                rolemanagement.SelectDomainfromDropDown(DomainName);
                bool role6 = rolemanagement.RoleExists(TestRoleSAG1);
                if (role1 && role2 && role3 && role4 && role5 && role6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Domain and/Role not created");
                }

                //step 11
                rolemanagement.SelectDomainfromDropDown(TestdomainA);
                rolemanagement.SearchRole(TestRoleA1, TestdomainA);
                rolemanagement.SelectRole(TestRoleA1);
                rolemanagement.ClickEditRole();
                bool studyFilter1 = rolemanagement.AccessFilterElement(1).Displayed;
                bool selfStudyFilter1 = rolemanagement.RoleSelfStudyFilter().Selected;
                rolemanagement.CloseRoleManagement();
                rolemanagement.SelectDomainfromDropDown(TestdomainB);
                rolemanagement.SearchRole(TestRoleB1, TestdomainB);
                rolemanagement.SelectRole(TestRoleB1);
                rolemanagement.ClickEditRole();
                bool studyFilter2 = rolemanagement.AccessFilterElement(1).Displayed;
                bool selfStudyFilter2 = !rolemanagement.RoleSelfStudyFilter().Selected;
                rolemanagement.CloseRoleManagement();
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SearchRole(RoleName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.ClickEditRole();
                bool studyFilter3 = rolemanagement.AccessFilterElement(1).Displayed;
                rolemanagement.CloseRoleManagement();
                rolemanagement.SelectDomainfromDropDown(DomainName);
                rolemanagement.SearchRole(TestRoleSAG1);
                rolemanagement.SelectRole(TestRoleSAG1);
                rolemanagement.ClickEditRole();
                bool studyFilter4 = rolemanagement.AccessFilterElement(1).Displayed;
                bool selfStudyFilter3 = rolemanagement.RoleSelfStudyFilter().Selected;
                rolemanagement.CloseRoleManagement();
                if (studyFilter1 && selfStudyFilter1 && studyFilter2 && selfStudyFilter2 && studyFilter3 && studyFilter4 && selfStudyFilter3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Domain and/Role not created");
                }

                //step 12
                usermanagement = login.Navigate<UserManagement>();
                bool user12_1 = usermanagement.IsUserExist(TestdomainA, TestdomainA);
                bool user12_2 = usermanagement.IsUserExist(TestdomainB, TestdomainB);
                bool user12_3 = usermanagement.IsUserExist(TestdomainC, TestdomainC);
                bool user12_4 = usermanagement.IsUserExist(TestuserB1, Config.adminGroupName);
                bool user12_5 = usermanagement.IsUserExist(SABUserName, Config.adminGroupName);
                if (user12_1 && user12_2 && user12_3 && user12_4 && user12_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("users not present");
                }
                login.Logout();

                //Step 13
                login.LoginIConnect(SABUserName, SABUserName);
                userPreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool png13_1 = userPreferences.PNGRadioBtn().Selected;
                userPreferences.CancelPreferenceBtn().Click();
                studies = login.Navigate<Studies>();
                //studies.ClearFields();
                //studies.SelectAllDateAndData();
                studies.SearchStudy("last", "*");
                studies.SelectStudy("Accession", Accession);
                bool studylisted13_1 = false;
                if (studies.SelectedStudyrow(Accession).Text == Accession)
                    studylisted13_1 = true;
                login.Logout();
                login.LoginIConnect(TestuserB1, TestuserB1);
                userPreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool png13_2 = userPreferences.PNGRadioBtn().Selected;
                userPreferences.CancelPreferenceBtn().Click();
                studies.SearchStudy("last", "*");
                studies.SelectStudy("Accession", Accession);
                bool studylisted13_2 = false;
                if (studies.SelectedStudyrow(Accession).Text == Accession)
                    studylisted13_2 = true;
                login.Logout();
                if (png13_1 && studylisted13_1 && png13_2 && studylisted13_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("User preferences not saved");
                }

                //step 14
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEncryption();
                servicetool.SetEncryptionEncryptionService();
                GroupBox GrpBx_13 = WpfObjects._application.GetWindows()[0].Get<GroupBox>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                Panel Pane_13 = wpfobject.GetUIItem<GroupBox, Panel>(GrpBx_13, 0);
                ListView LstVw_13 = wpfobject.GetUIItem<Panel, ListView>(Pane_13, "ListView");
                int encryption13 = LstVw_13.Items.Count;
                if (encryption13 == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Encryption is added");
                }

                //step 15
                servicetool.Enc_ServiceTab().Focus();
                servicetool.Enc_ServiceTab().Click();
                servicetool.EnterServiceEntry(Key: "TripleDES", Assembly: "OpenContent.Generic.Core.dll",
                                              Class: "OpenContent.Core.Security.Services.TripleDES");
                //servicetool.EnterServiceParameters("configFile", "string", "Config\\EmergeServicesConfiguration.xml");
                //servicetool.EnterServiceEntry();
                servicetool.EnterServiceParameters("key", "string", "");
                servicetool.EnterServiceParameters("iv", "string", "");
                servicetool.EnterServiceParameters("characterSet", "string", "Windows-1252");
                servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                taskbar.Show();
                if (servicetool.Grid().Rows.Count == 1)
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

                //step 16
                string[] Keys = servicetool.GenerateEncryptionKeys("mergehealthcare");
                if (Keys != null)
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

                //step 17
                servicetool.EditServiceParameters("Key", "TripleDES", "Name", "key", Value: Keys[0]);
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                ListViewRow servicerow1 = servicetool.ServiceParams_Grid().Row("Value", Keys[0]);
                if (servicerow1 != null)
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
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();

                //step 18
                servicetool.EnterEncryptionProviders("arg", "args", "Cryptographic.TripleDES");
                servicetool.ClickApplyButtonFromTab();
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.WaitWhileBusy();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListViewRow serprorow1 = servicetool.ServicePro_Grid().Row("Id", "arg");
                if (serprorow1 != null)
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

                //step 19
                servicetool.Enc_ServiceTab().Focus();
                servicetool.Enc_ServiceTab().Click();
                servicetool.EnterServiceEntry(Key: "TripleDES-1", Assembly: "OpenContent.Generic.Core.dll",
                                              Class: "OpenContent.Core.Security.Services.TripleDES");
                //servicetool.EnterServiceParameters("configFile", "string", "Config\\EmergeServicesConfiguration.xml");
                //servicetool.EnterServiceEntry();
                servicetool.EnterServiceParameters("key", "string", "");
                servicetool.EnterServiceParameters("iv", "string", "");
                servicetool.EnterServiceParameters("characterSet", "string", "Windows-1252");
                servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                taskbar.Show();
                if (servicetool.Grid().Rows.Count > 1)
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

                //step 20
                string[] Keys20 = servicetool.GenerateEncryptionKeys("mergehealthcare-1");
                if (Keys20 != null)
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

                //step 21
                servicetool.EditServiceParameters("Key", "TripleDES-1", "Name", "key", Value: Keys20[0]);
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                ListViewRow servicerow2111 = servicetool.ServiceParams_Grid().Row("Value", Keys20[0]);
                //servicetool.EditServiceParameters("Key", "TripleDES", "Name", "key", Value: Keys20[0]);
                //wpfobject.GetMainWindowByTitle("Service Entry Form");
                //ListViewRow servicerow21 = servicetool.ServiceParams_Grid().Row("Value", Keys20[0]);
                ListViewRow servicerow21 = servicetool.ServiceParams_Grid().Row("Name", "key");
                if (servicerow21 != null) { servicerow21.Click(); } else { Logger.Instance.InfoLog(servicerow21 + " is not found"); }
                servicetool.ServiceParams_detail().Click();
                Thread.Sleep(10000);
                wpfobject.GetMainWindowByTitle("Service Parameter Entry Form");
                bool step21 = false;
                if (servicetool.Value_txt().Text != null) step21 = true;
                wpfobject.GetButton("OK", 1).Click();
                Thread.Sleep(10000);
                if (step21)
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
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();

                //step 22
                servicetool.EnterEncryptionProviders("Test-1", "args", "Cryptographic.TripleDES-1");
                servicetool.ClickApplyButtonFromTab();
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.WaitWhileBusy();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListViewRow serprorow22 = servicetool.ServicePro_Grid().Row("Id", "arg");
                if (serprorow22 != null)
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

                //step 23
                //servicetool.DefaultSerProvider_txt().Text = "Test";
                //Thread.Sleep(3000);
                //servicetool.ClickApplyButtonFromTab();
                //servicetool.ClickApplyButtonFromTab();
                //servicetool.AcceptDialogWindow();
                //servicetool.RestartService();
                ////servicetool.WaitWhileBusy();
                //servicetool.EditServiceParameters("Key", "TripleDES", "Name", "key", Value: Keys20[0]);
                //wpfobject.GetMainWindowByTitle("Service Entry Form");
                //ListViewRow servicerow23 = servicetool.ServiceParams_Grid().Row("Value", Keys20[0]);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListViewRow serprorow23 = servicetool.ServicePro_Grid().Row("Id", "arg");
                if (serprorow23 != null)
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
                servicetool.CloseServiceTool();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();

                //step 24
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a" });
                ehr.EncryptionCB().Checked = true;
                String url_8 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url_8);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    ExecutedSteps++;
                }
                else
                {
                    StudyVw = (StudyViewer)login.NavigateToIntegratorFrame();
                    if (ehr.VerifyPatientList())
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

                //step 25              
                bool step25 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {                  
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step25 = bluViewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport));
                }
                else
                {
                    ehr.selectPatient(LastName[1]);
                    login.ClickUrlViewStudyBtn();
                    step25 = StudyVw.PatientDetailsInViewer()["LastName"].ToLower().Equals(LastName[1].ToLower()) &&
                    StudyVw.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PatientID[1].ToLower());
                }
                if (step25)
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
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();

                //step 26
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a*" });
                ehr.EncryptionCB().Checked = false;
                String url_26 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url_26);
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                if (StudyVw.AuthenticationErrorMsg().Text.ToLower().Contains("url is not properly encrypted"))
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
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();

                //step 27
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a" });
                ehr.EncryptionCB().Checked = true;
                String url_27 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url_27);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    ExecutedSteps++;
                }
                else
                {
                    StudyVw = (StudyViewer)login.NavigateToIntegratorFrame();
                    if (ehr.VerifyPatientList())
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

                //step 28
                ehr.selectPatient(LastName[1]);
                bool step28 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step28 = StudyVw.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport));
                }
                else
                {
                    login.ClickUrlViewStudyBtn();
                    step28 = StudyVw.PatientDetailsInViewer()["LastName"].ToLower().Equals(LastName[1].ToLower()) &&
                    StudyVw.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PatientID[1].ToLower());
                }
                if (step28)
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
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();

                //step 29
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a" });
                ehr.EncryptionCB().Checked = true;
                wpfobject.SelectCheckBox("useNoEncryption");
                String url_29 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url_29);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                if (StudyVw.AuthenticationErrorMsg().Text.ToLower().Contains("requested study doesn't match security claims provided"))
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

                //step 30
                login.CloseBrowser();
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
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }



        /// <summary>
        /// Upgrade iConnect Access with LDAP Configuration
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27645(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            StudyViewer viewer = new StudyViewer();
            BluRingViewer bluringviewer = new BluRingViewer();
            Studies studies = null;
            UserPreferences userPreferences = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String LDAPadminUserName = Config.LdapAdminUserName;
                String LDAPadminPassword = Config.LdapAdminPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                //icainstaller.uninstalliCA(1);

                //Step-1:Install and configure a previous version of Merge iConnect WebAccess:
                //1. Run Merge iConnect Access Service Tool --*^>^* LDAP tab.
                //2. In Global Options tab, select Enable Ldap option and select Apply button.
                //3. In Servers tab, select ica.ldap.merge.ad option and then Detail button.
                //4. In Standard Settings tab, select the whole row iCA-LDAP in Server Hosts box and select Test Connection button. Ensure that the connection to host iCA-LDAP:389 succeeded.

                //Make a note of the changes made as this will be verified after the upgrade.               
                icainstaller.installiCA(0);
                Thread.Sleep(10000);
                servicetool.EnableLDAPConfigfile();
                ExecutedSteps++;

                //Step-2: Add few datasources by using the iConnect Access Service Tool 
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddLicenseInConfigTool(); //License file should be in the path as a testdata
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                //Add EA1 Datasource
                servicetool.AddEADatasource(Config.EA1, Config.EA1AETitle, "1");
                //Add Sanity PACS
                servicetool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);
                //LDAP setup
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.LDAPSetup();
                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();
                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                login.Logout();
                ExecutedSteps++;


                //Step-3:Login to Merge iConnect Access as ica.administrator/admin.13579
                login.DriverGoTo(login.url);
                login.LoginIConnect(LDAPadminUserName, LDAPadminPassword);
                if (login.IsTabPresent("Studies"))
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

                //Step-4:Load a study and perform few operations.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionID);
                studies.SelectStudy("Accession", AccessionID);
                bool step4 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {                   
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    bluringviewer.ApplyTool_FlipHorizontal();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step4 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport));
                    bluringviewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = StudyViewer.LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.AutoWindowLevel);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    IWebElement image1 = BasePage.Driver.FindElement(By.CssSelector("img[src*='autoWlImage']"));
                    step4 = image1.Displayed;
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                    viewer.CloseStudy();
                }
                //Validate Image 
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

                //Step-5:Select Contents from the Help icon.
                OnlineHelp onlinehelp = new OnlineHelp().OpenHelpandSwitchtoIT(0);
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
                if (onlinehelp.OnlineHelpVersion().Text.Contains(Config.prevbuildversion))
                {
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

                //Step-6:Run a current test build and upgrade the iCA to a current version. Check the software version from the registry tree.
                icainstaller.upgradeiCA();
                ExecutedSteps++;

                //Step-7:Login to Merge iConnect Access as ica.administrator/admin.13579
                login.DriverGoTo(login.url);
                login.LoginIConnect(LDAPadminUserName, LDAPadminPassword);
                if (login.IsTabPresent("Studies"))
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

                //Step-8:Load a study and perform few operations.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionID);
                studies.SelectStudy("Accession", AccessionID);
                bool step8 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    bluringviewer.ApplyTool_FlipHorizontal();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step8 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport));
                    bluringviewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = StudyViewer.LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.AutoWindowLevel);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    IWebElement image1 = BasePage.Driver.FindElement(By.CssSelector("img[src*='autoWlImage']"));
                    step8 = image1.Displayed;
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                    viewer.CloseStudy();
                }
                //Validate Image 
                if (step8)
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

                //Step-9:Select Contents from the Help icon.
                new OnlineHelp().OpenHelpandSwitchtoIT(0);
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
                if (onlinehelp.OnlineHelpVersion().Text.Contains(Config.currbuildversion))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10:Logout
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

                //Return Result
                return result;
            }
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }

        /// <summary>
        /// Upgrade iConnect Access with LDAP Configuration-invalid login
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27650(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            StudyViewer viewer = new StudyViewer();
            Studies studies = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            BluRingViewer bluringviewer = new BluRingViewer();
            UserPreferences userPreferences = null;

            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String LDAPadminUserName = Config.LdapAdminUserName;
                String LDAPadminPassword = Config.LdapAdminPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();


                //Step-1:Install and configure a previous version of Merge iConnect WebAccess:
                //1. Run Merge iConnect Access Service Tool --*^>^* LDAP tab.
                //2. In Global Options tab, select Enable Ldap option and select Apply button.
                //3. In Servers tab, select ica.ldap.merge.ad option and then Detail button.
                //4. In Standard Settings tab, select the whole row iCA-LDAP in Server Hosts box and select Test Connection button. Ensure that the connection to host iCA-LDAP:389 succeeded.

                //Make a note of the changes made as this will be verified after the upgrade.               
                icainstaller.installiCA(0);
                servicetool.EnableLDAPConfigfile();
                ExecutedSteps++;

                //Step-2: Add few datasources by using the iConnect Access Service Tool 
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddLicenseInConfigTool(); //License file should be in the path as a testdata
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                //Add EA1 Datasource
                servicetool.AddEADatasource(Config.EA1, Config.EA1AETitle, "1");
                //Add Sanity PACS
                servicetool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);
                //LDAP setup
                servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                servicetool.SetMode(2);
                servicetool.LDAPSetup();
                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();
                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                login.Logout();
                ExecutedSteps++;


                //Step-3:Login to Merge iConnect Access as ica.administrator/admin.13579
                login.DriverGoTo(login.url);
                login.LoginIConnect(LDAPadminUserName, LDAPadminPassword);
                if (login.IsTabPresent("Studies"))
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

                //Step-4:Load a study and perform few operations.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionID);
                studies.SelectStudy("Accession", AccessionID);
                bool step4 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {                   
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    bluringviewer.ApplyTool_FlipHorizontal();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step4 = studies.CompareImage(result.steps[ExecutedSteps], bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport));
                    bluringviewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = StudyViewer.LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.AutoWindowLevel);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    IWebElement image1 = BasePage.Driver.FindElement(By.CssSelector("img[src*='autoWlImage']"));
                    step4 = image1.Displayed;
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                    viewer.CloseStudy();
                }
                //Validate Image 
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

                //Step-5:Select Contents from the Help icon.
                OnlineHelp onlinehelp = new OnlineHelp().OpenHelpandSwitchtoIT(0);
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
                if (onlinehelp.OnlineHelpVersion().Text.Contains(Config.prevbuildversion))
                {
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

                //Step-6:Run a current test build and upgrade the iCA to a current version. Check the software version from the registry tree.
                icainstaller.upgradeiCA();
                ExecutedSteps++;

                //Step-7:Login to Merge iConnect Access as ica.administrator/admin.13578
                login.DriverGoTo(login.url);
                login.LoginIConnect(LDAPadminUserName, "admin.13578");
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


                //Return Result
                return result;
            }
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }


        /// <summary>
        /// Verify Dignity LDAP features
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27646(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables    
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String LDAPadminUserName = Config.LdapAdminUserName;
                String LDAPadminPassword = Config.LdapAdminPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();

                //Precondition
                icainstaller.installiCA(0);

                //Step-1:Install and configure a previous version of Merge iConnect WebAccess:
                //1. Run Merge iConnect Access Service Tool --*^>^* LDAP tab.
                //2. In Global Options tab, select Enable Ldap option and select Apply button.
                //3. In Servers tab, select ica.ldap.merge.ad option and then Detail button.
                //4. In Standard Settings tab, select the whole row iCA-LDAP in Server Hosts box and select Test Connection button. Ensure that the connection to host iCA-LDAP:389 succeeded.
                Thread.Sleep(10000);
                servicetool.EnableLDAPConfigfile();
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.LDAPSetup(restart: false);
                ExecutedSteps++;

                //Step-2: Select Data Model tab --*^>^* select box Enable Role Management Rules
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                wpfobject.WaitTillLoad();
                Thread.Sleep(10000);
                ITabPage tab = WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.LDAP.Name.DataModel));
                tab.Focus();
                tab.Click();
                CheckBox Rules = wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, "EnableRoleManagementRules");
                Rules.Checked = true;
                wpfobject.WaitForButtonExist("LDAP Server Control Form", "Browse", 1);
                Button GenerateRules = wpfobject.GetButton("Generate Rules", 1);
                Button Browse = wpfobject.GetButton("Browse", 1);
                if (GenerateRules.Enabled && Browse.Enabled)
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


                //Step-3:Select Generate Rules button.
                GenerateRules.Click();
                servicetool.WaitWhileBusy();
                Window NewWindow = wpfobject.GetMainWindowByTitle("Ldap Role Management Form");
                if (NewWindow.Enabled)
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

                servicetool.CloseConfigTool();

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


                //Return Result
                return result;
            }
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }
        /// <summary>
        /// Upgrade iConnect Access with existing URL Encryption
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27647(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");

                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();

                //Precondition
                icainstaller.installiCA(0);

                //Step-1:Add few datasources by using the iConnect Access Service Tool
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddLicenseInConfigTool(); //License file should be in the path as a testdata
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToTab("Integrator");
                wpfobject.WaitTillLoad();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined");
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.SelectCheckBox("CB_AllowShowSelectorSearch");
                wpfobject.SelectCheckBox("CB_AllowShowSelector");
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseConfigTool();
                //iCA application setting
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();
                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                login.Logout();
                ExecutedSteps++;

                //Step-2:Check the Encryption Service.-Steps-- Run Merge iCA Service Tool --*^-^* Encryption tab --*^-^* Encryption Service tab.
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                Thread.Sleep(10000);
                wpfobject.SelectTabFromTabItems(ServiceTool.Encryption.Name.Encryption_tab);
                Thread.Sleep(10000);
                servicetool.Enc_ServiceTab().Focus();
                servicetool.Enc_ServiceTab().Click();
                if (servicetool.Grid().Rows.Count == 0)
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

                //Step-3:Enable and configure an encryption provided by the followings:
                //Encrypstion Service tab-- *^>^ *select Add button and enter:
                //Key = TripleDES
                //Assembly = OpenContent.Generic.Core.dll
                //Class = OpenContent.Core.Security.Services.TripleDES
                //Select Add botton and enter the following Service Parameters:
                // Name Class Value
                // ------------------
                //key string Leave Blank
                //iv string Leave Blank
                //characterSet string Windows-1252
                //operationMode string CBC
                //paddingMode string Zeros
                //Click on OK and OK and reset IISserver.  
                servicetool.EnterServiceEntry();
                servicetool.EnterServiceParameters("key", "string", "");
                servicetool.EnterServiceParameters("iv", "string", "");
                servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
                servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();
                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();
                if (servicetool.Grid().Rows.Count == 1)
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

                //Step-4:Create an encryption key using the Key Converter tab.-Steps--Enter Plain Text Key - mergehealthcare-Select Convert button                
                //IUIItem[] elements = servicetool.KeyGeneratorTab().GetMultiple(SearchCriteria.ByClassName("TextBox"));
                string[] Keys = servicetool.GenerateEncryptionKeys("mergehealthcare");
                if (Keys != null)
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


                //Step-5:Copy the encryption key in the Base64 box and enter it in the Value of Sevice Parameters above ("key"value field that was"Leave Blank") by selecting Detail button.-Select OK button and IISRESET
                servicetool.EditServiceParameters("Key", "TripleDES", "Name", "key", Value: Keys[0]);
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                ListViewRow servicerow1 = servicetool.ServiceParams_Grid().Row("Value", Keys[0]);
                if (servicerow1 != null)
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
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();

                //Step-6:Select Integrator URL tab--Select URL Encryption Enabled box.-Argument Name - args-Encryption service - Cryptographic.TripleDES-Apply button-IISRESET
                servicetool.EnterEncryptionProviders("arg", "args", "Cryptographic.TripleDES");
                servicetool.ClickApplyButtonFromTab();
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.WaitWhileBusy();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListViewRow serprorow1 = servicetool.ServicePro_Grid().Row("Id", "arg");
                if (serprorow1 != null)
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
                servicetool.CloseServiceTool();

                //Step-7:Integration must be setup. "ByPass"must be enabled in ..\WebAccess\IntegratorAuthenticationSTS\Web.config               
                login.UncommentXMLnode("id", "Bypass");
                icainstaller.CopyFiles(System.IO.Directory.GetCurrentDirectory() + @"\TestEHRfiles", @"C:\WebAccess\WebAccess\bin");
                ExecutedSteps++;

                //Step-8:Run TestEHR--Address - http-//localhost/WebAccess-Show Selector-True-Show Selector Search-True-Full Name-a*-Select box Use Encryption (TimeStamp)-Leave others default.-Select Load button
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a*" });
                ehr.EncryptionCB().Checked = true;
                String url_8 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step-9:Select one study and click View button to load to viewer.
                //Navigate to url generated in test eHR
                login = new Login();
                StudyViewer studyviewer = new StudyViewer();
                BluRingViewer bluViewer = new BluRingViewer();
                bool step9 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_8);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step9 = bluViewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname.ToLower()) &&
                     bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PatientID.ToLower());
                }
                else
                {
                    login.NavigateToIntegratorURL(url_8);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    ehr.selectPatient(Lastname);
                    login.ClickUrlViewStudyBtn();
                    step9 = studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname.ToLower()) &&
                    studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PatientID.ToLower());
                }
                if (step9)
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

                //Step-10:In the TestEHR----Address - http-//localhost/WebAccess---Show Selector-True---Show Selector Search-True---Full Name-a*---Un-select box Use Encryption (TimeStamp)---Select Load button
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a*" });
                ehr.EncryptionCB().Checked = false;
                String url_10 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                //Navigate to url generated in test eHR
                login = new Login();
                if(Config.isEnterpriseViewer.ToLower().Equals("y"))
                    bluViewer.NavigateToBluringIntegratorURL(url_10);
                else
                    login.NavigateToIntegratorURL(url_10);
                String ErrorMessage = "Error Occurred in operation: URL is not properly encrypted";
                if (ehr.ErrorMsg().Contains(ErrorMessage))
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

                //Step-11:Select one study and click View button to load to viewer.                
                ExecutedSteps++;

                //Step-12:Run a current test build and upgrade the iCA to a current version. -Check the software version from the registry tree.
                icainstaller.upgradeiCA();
                ExecutedSteps++;

                //Step-13:Check the Encryption Service.-Steps-- Run Merge iCA Service Tool --*^-^* Encryption tab --*^-^* Encryption Service tab.
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                Thread.Sleep(10000);
                wpfobject.SelectTabFromTabItems(ServiceTool.Encryption.Name.Encryption_tab);
                Thread.Sleep(10000);
                servicetool.Enc_ServiceTab().Focus();
                servicetool.Enc_ServiceTab().Click();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListViewRow RowVerified = servicetool.Grid().Row("Key", "Cryptographic.TripleDES");
                if (servicetool.Grid().Rows.Count == 1 && RowVerified != null)
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

                //Step-14:Select Integrator URL tab
                servicetool.IntegratorUrlTab().Focus();
                servicetool.IntegratorUrlTab().Click();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListViewRow serprorow2 = servicetool.ServicePro_Grid().Row("Id", "arg");
                if (serprorow2 != null)
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
                servicetool.CloseServiceTool();


                //Step-15:Run TestEHR--Address - http-//localhost/WebAccess-Show Selector-True-Show Selector Search-True-Full Name-a*-Select box Use Encryption (TimeStamp)-Leave others default.-Select Load button
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a*" });
                ehr.EncryptionCB().Checked = true;
                String url_15 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step-16:Select one study and click View button to load to viewer.
                //Navigate to url generated in test eHR
                login = new Login();                
                bool step16 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_15);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    step16 = bluViewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname.ToLower()) &&
                    bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PatientID.ToLower());
                }
                else
                {
                    login.NavigateToIntegratorURL(url_15);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    ehr.selectPatient(Lastname);
                    login.ClickUrlViewStudyBtn();
                    step16 = studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname.ToLower()) &&
                    studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PatientID.ToLower());
                }               
                if (step16)
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
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }

        /// <summary>
        /// Upgrade iConnect Access with existing URL Encryption-invalid
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27649(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables           
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");

                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                //icainstaller.uninstalliCA(1);

                //Precondition               
                icainstaller.installiCA(0);


                //Step-1:Add few datasources by using the iConnect Access Service Tool
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddLicenseInConfigTool(); //License file should be in the path as a testdata
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToTab("Integrator");
                wpfobject.WaitTillLoad();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined");
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.SelectCheckBox("CB_AllowShowSelectorSearch");
                wpfobject.SelectCheckBox("CB_AllowShowSelector");
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseConfigTool();
                //iCA application setting
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();
                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                login.Logout();
                ExecutedSteps++;

                //Step-2:Check the Encryption Service.-Steps-- Run Merge iCA Service Tool --*^-^* Encryption tab --*^-^* Encryption Service tab.
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                Thread.Sleep(10000);
                wpfobject.SelectTabFromTabItems(ServiceTool.Encryption.Name.Encryption_tab);
                Thread.Sleep(10000);
                servicetool.Enc_ServiceTab().Focus();
                servicetool.Enc_ServiceTab().Click();
                if (servicetool.Grid().Rows.Count == 0)
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

                //Step-3:Enable and configure an encryption provided by the followings:
                //Encrypstion Service tab-- *^>^ *select Add button and enter:
                //Key = TripleDES
                //Assembly = OpenContent.Generic.Core.dll
                //Class = OpenContent.Core.Security.Services.TripleDES
                //Select Add botton and enter the following Service Parameters:
                // Name Class Value
                // ------------------
                //key string Leave Blank
                //iv string Leave Blank
                //characterSet string Windows-1252
                //operationMode string CBC
                //paddingMode string Zeros
                //Click on OK and OK and reset IISserver.  
                servicetool.EnterServiceEntry();
                servicetool.EnterServiceParameters("key", "string", "");
                servicetool.EnterServiceParameters("iv", "string", "");
                servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
                servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();
                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();
                if (servicetool.Grid().Rows.Count == 1)
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

                //Step-4:Create an encryption key using the Key Converter tab.-Steps--Enter Plain Text Key - mergehealthcare-Select Convert button                
                //IUIItem[] elements = servicetool.KeyGeneratorTab().GetMultiple(SearchCriteria.ByClassName("TextBox"));
                string[] Keys = servicetool.GenerateEncryptionKeys("mergehealthcare");
                if (Keys != null)
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


                //Step-5:Copy the encryption key in the Base64 box and enter it in the Value of Sevice Parameters above ("key"value field that was"Leave Blank") by selecting Detail button.-Select OK button and IISRESET
                servicetool.EditServiceParameters("Key", "TripleDES", "Name", "key", Value: Keys[0]);
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                ListViewRow servicerow1 = servicetool.ServiceParams_Grid().Row("Value", Keys[0]);
                if (servicerow1 != null)
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
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();

                //Step-6:Select Integrator URL tab--Select URL Encryption Enabled box.-Argument Name - args-Encryption service - Cryptographic.TripleDES-Apply button-IISRESET
                servicetool.EnterEncryptionProviders("arg", "args", "Cryptographic.TripleDES");
                servicetool.ClickApplyButtonFromTab();
                servicetool.ClickApplyButtonFromTab();
                servicetool.AcceptDialogWindow();
                servicetool.RestartService();
                servicetool.WaitWhileBusy();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                ListViewRow serprorow1 = servicetool.ServicePro_Grid().Row("Id", "arg");
                if (serprorow1 != null)
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
                servicetool.CloseServiceTool();

                //Step-7:Integration must be setup. "ByPass"must be enabled in ..\WebAccess\IntegratorAuthenticationSTS\Web.config               
                login.UncommentXMLnode("id", "Bypass");
                icainstaller.CopyFiles(System.IO.Directory.GetCurrentDirectory() + @"\TestEHRfiles", @"C:\WebAccess\WebAccess\bin");
                ExecutedSteps++;

                //Step-8:Run TestEHR--Address - http-//localhost/WebAccess121-Show Selector-True-Show Selector Search-True-Full Name-a*-Select box Use Encryption (TimeStamp)-Leave others default.-Select Load button
                ehr.LaunchEHR();
                ehr.SetCommonParameters(address: "http://localhost/WebAccess121");
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "fullname" }, new String[] { "a*" });
                ehr.EncryptionCB().Checked = true;
                String url_8 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();

                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_8);
                String ErrorMessage = "Server Error in '/' Application.";
                if (ehr.ServerErrorMsg().Contains(ErrorMessage))
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
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }

        /// <summary>
        /// Upgrade iConnect Access with Hosted Configuration testing
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27644(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables    
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PIDList = PatientID.Split(':');
                String Lastnames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] Lastname = Lastnames.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                BluRingViewer bluViewer = new BluRingViewer();
                StudyViewer studyviewer = new StudyViewer();

                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                
                //Step-1:Install and configure a previous ICA-- license iCA- add test datasource- enable integrator mode ( Enable byPass from C-\WebAccess\IntegratorAuthenticationSTS\web.config)
                icainstaller.installiCA(0);
                login.UncommentXMLnode("id", "Bypass");
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddLicenseInConfigTool(); //License file should be in the path as a testdata
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddPacsDatasource(Config.PACS2, Config.PACS2AETitle, "1", Config.pacsadmin, Config.pacspassword);
                servicetool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToTab("Integrator");
                wpfobject.WaitTillLoad();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined");
                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.SelectCheckBox("CB_AllowShowSelectorSearch");
                wpfobject.SelectCheckBox("CB_AllowShowSelector");
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseConfigTool();
                //iCA application setting
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();
                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                login.Logout();
                ExecutedSteps++;

                //Step-2:Copy the listed files from current build test tool folder  to C:\WebAccess\WebAccess\bin:-TestEHR.exe-TestEHR.exe.config-TestEHR.pdb-TestEHR.samlPolicy.config-SystemFactoryConfiguration.xml-ServiceFactoryConfiguration.xml                
                icainstaller.CopyFiles(System.IO.Directory.GetCurrentDirectory() + @"\TestEHRfiles", @"C:\WebAccess\WebAccess\bin");
                ExecutedSteps++;

                //Step-3:Launch TestEHR from bin folder TestEHR.exe
                ehr.LaunchEHR();
                ExecutedSteps++;

                //Step-4:Set parameters:-Address = http:/localhost/WebAccess-Show Selector = True-Show Report = True-Auto End Session = True-Enter search keys: such as lastname/first name -click"Cmd line"button to create the test URL.
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "lastname" }, new String[] { "a" });
                String url_4 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step-5:Option 1: copy the URL to a browser and launch it. -Option 2: Click Load button to launch the default browser in server.
                //Step-6:Select one study and click View button to load to viewer.
                login = new Login();
                bool step6 = false;               
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_4);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    ehr.selectPatient(Lastname[0]);
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    step6 = bluViewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[0].ToLower()) &&
                    bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[0].ToLower());                   
                }
                else
                {
                    login.NavigateToIntegratorURL(url_4);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    ehr.selectPatient(Lastname[0]);
                    login.ClickUrlViewStudyBtn();
                    step6 = studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[0].ToLower()) &&
                    studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[0].ToLower());
                }
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

                //Step-7:load the URL to browser again. -Select more than one study that may not belong to same patient. Click View button.
                login = new Login();
                bool step7 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_4);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    ehr.selectPatient(Lastname[1]);
                    ehr.selectPatient(Lastname[2]);
                    ehr.selectPatient(Lastname[3]);
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    step7 = bluViewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[1].ToLower()) &&
                            bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[1].ToLower());
                }
                else
                {
                    login.NavigateToIntegratorURL(url_4);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    ehr.selectPatient(Lastname[1]);
                    ehr.selectPatient(Lastname[2]);
                    ehr.selectPatient(Lastname[3]);
                    login.ClickUrlViewStudyBtn();
                    step7 = studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[1].ToLower()) &&
                    studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[1].ToLower()) &&
                     studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[0].ToLower());
                }
                if (step7)
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

                //Step-8:Apply tool : measurement, annotation.
                bool step_8 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.SelectViewerTool(BluRingTools.Line_Measurement);
                    bluViewer.ApplyTool_LineMeasurement();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_8 = studyviewer.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport));
                }
                else
                {
                    studyviewer.DrawLineMeasurement(studyviewer.SeriesViewer_1X1(), 50, 100);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_8 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.studyPanel());
                }
                if (step_8)
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

                //Step-9:Go to the patient history drawer and click another study.
                bool step9 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.OpenPriors(1);
                    step9 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2)"));
                }
                else
                {
                    studyviewer.NavigateToHistoryPanel();
                    studyviewer.ChooseColumns(new string[] { "Accession" });
                    Dictionary<string, string> secondstudy = studyviewer.GetMatchingRow("Accession", Accessions[1]);
                    studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                    PageLoadWait.WaitForFrameLoad(20);
                    String Studyinfo9 = studyviewer.StudyInfo(2);
                    step9 = studyviewer.studyPanel(2).Displayed && secondstudy["Accession"].Equals(Studyinfo9.Split(',')[0]);
                }
                if (step9)
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

                //Step-10:Go to the patient history drawer and click another study.
                bool step10 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.OpenPriors(2);
                    step10 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(3)"));
                }
                else
                {
                    studyviewer.NavigateToHistoryPanel();
                    Dictionary<string, string> thirdstudy = studyviewer.GetMatchingRow("Accession", Accessions[2]);
                    studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[2] });
                    PageLoadWait.WaitForFrameLoad(20);
                    String Studyinfo10 = studyviewer.StudyInfo(3);
                    step10 = studyviewer.studyPanel(3).Displayed && thirdstudy["Accession"].Equals(Studyinfo10.Split(',')[0]);
                }
                if (step10)
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

                //Step-11:From TestEHR load study with report to viewer and check the report.
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True");
                ehr.SetSearchKeys_Study(Accessions[3]);
                String url_11 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                //Navigate to url generated in test eHR
                login = new Login();               
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_11);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    bluViewer.OpenReport_BR(0);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    var ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                    if (ReportContainer.Displayed && bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[1].ToLower()))
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
                    login.NavigateToIntegratorURL(url_11);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    studyviewer.TitlebarReportIcon().Click();
                    if (studyviewer.ReportContainer().Displayed && studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[4]) &&
                       studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[3].ToLower()))
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

                //Step-12:open the TestEHR app.
                ehr.LaunchEHR();
                ExecutedSteps++;

                //Step-13:Set parameters:-Address = http:/localhost/WebAccess-Show Selector = false-Show Report = True-Auto End Session = True-Enter search keys: -either study UID or Accession number. (study UID / Accession number must be unique) -click"Cmd line"button to create the test URL.
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "False", showReport: "True");
                ehr.SetSearchKeys_Study(Accessions[1]);
                String url_13 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step-14:Option 1: copy the URL to a browser and launch it. -Option 2: Click Load button to launch the default browser in server.
                login = new Login();               
                bool step14 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_13);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    step14 = bluViewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[2].ToLower()) &&
                            bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[2].ToLower());
                }
                else
                {
                    login.NavigateToIntegratorURL(url_13);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    step14 = studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[2].ToLower()) &&
                             studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[2].ToLower()) &&
                              studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[1].ToLower());                   
                }
                if(step14)
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

                //Step-15:Upgrade to a Current build. -Run current test build and upgrade the iCA to current version. -Check the software version from registry tree.
                icainstaller.upgradeiCA();
                ExecutedSteps++;

                //Step-16:Go to the service tool-*^>^* viewer-*^>^* Miscellaneous to enable the HTML5 Viewer Support and select HTML4 Viewer.
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.EnableHTML5(false);
                servicetool.CloseConfigTool();
                ExecutedSteps++;

                //Step-17:Launch TestEHR from bin folder TestEHR.exe
                ehr.LaunchEHR();
                ExecutedSteps++;

                //Step-18:Set parameters:-Address = http:/localhost/WebAccess-Show Selector = True-Show Report = True-Auto End Session = True-Enter search keys: such as lastname/first name -click"Cmd line"button to create the test URL.-
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "lastname" }, new String[] { "a" });
                String url_18 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step-19:Option 1: copy the URL to a browser and launch it. -Option 2: Click Load button to launch the default browser in server.
                login = new Login();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_18);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                }
                else
                {
                    login.NavigateToIntegratorURL(url_18);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                }
                if (!(new IntegratorStudies().Intgr_HTML5Btn().Enabled) && !(new IntegratorStudies().Intgr_ViewBtn().Enabled))
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

                //Step-20:Select one study and click View button to load to viewer.
                ehr.selectPatient(Lastname[0]);
                bool step20 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    step20 = bluViewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[0].ToLower()) &&
                                bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[0].ToLower());
                }
                else
                {
                    login.ClickUrlViewStudyBtn();
                    step20 = studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[0].ToLower()) &&
                    studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[0].ToLower());
                }
                if (step20)
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

                //Step-21:Close the browser and load the URL to a browser again. -Select more than one study that may not belong to same patient. Click View button.
                login = new Login();
                bool step21 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_18);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "BluRingViewer");
                    ehr.selectPatient(Lastname[1]);
                    ehr.selectPatient(Lastname[2]);
                    ehr.selectPatient(Lastname[3]);
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    step21 = bluViewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[1].ToLower()) &&
                          bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[1].ToLower());
                }
                else
                {
                    login.NavigateToIntegratorURL(url_18);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    ehr.selectPatient(Lastname[1]);
                    ehr.selectPatient(Lastname[2]);
                    ehr.selectPatient(Lastname[3]);
                    login.ClickUrlViewStudyBtn();
                    step21 = studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[1].ToLower()) &&
                    studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[1].ToLower()) &&
                     studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[0].ToLower());
                }
                if (step21)
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


                //Step-22:Apply tool : measurement, annotation.
                bool step_22 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.SelectViewerTool(BluRingTools.Line_Measurement);
                    bluViewer.ApplyTool_LineMeasurement();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_22 = bluViewer.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport));
                }
                else
                {
                    studyviewer.DrawLineMeasurement(studyviewer.SeriesViewer_1X1(), 50, 100);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_22 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.studyPanel());
                }
                if (step_22)
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

                //Step-23:Go to the patient history drawer and click another study.
                bool step23 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.OpenPriors(1);
                    step23 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2)"));
                }
                else
                {
                    studyviewer.NavigateToHistoryPanel();
                    studyviewer.ChooseColumns(new string[] { "Accession" });
                    Dictionary<string, string> secondstudy23 = studyviewer.GetMatchingRow("Accession", Accessions[1]);
                    studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                    PageLoadWait.WaitForFrameLoad(20);
                    String Studyinfo23 = studyviewer.StudyInfo(2);
                    step23 = studyviewer.studyPanel(2).Displayed && secondstudy23["Accession"].Equals(Studyinfo23.Split(',')[0]);
                }
                if (step23)
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

                //Step-24:Go to the patient history drawer and click another study.
                bool step24 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.OpenPriors(2);
                    step24 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(3)"));
                }
                else
                {
                    studyviewer.NavigateToHistoryPanel();
                    Dictionary<string, string> thirdstudy24 = studyviewer.GetMatchingRow("Accession", Accessions[2]);
                    studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[2] });
                    PageLoadWait.WaitForFrameLoad(20);
                    String Studyinfo24 = studyviewer.StudyInfo(3);
                    step24 = studyviewer.studyPanel(3).Displayed && thirdstudy24["Accession"].Equals(Studyinfo24.Split(',')[0]);
                }
                if (step24)
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

                //Step-25:Close the browser.  From TestEHR load study with report to viewer and check the report.
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True");
                ehr.SetSearchKeys_Study(Accessions[3]);
                String url_25 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                //Navigate to url generated in test eHR
                login = new Login();
               
                bool step25 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer.NavigateToBluringIntegratorURL(url_25);
                    bluViewer = (BluRingViewer)login.NavigateToIntegratorFrame();
                    bluViewer.OpenReport_BR(0);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    var ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                    step25 = ReportContainer.Displayed && bluViewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[1].ToLower());                   
                }
                else
                {
                    login.NavigateToIntegratorURL(url_25);
                    studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                    studyviewer.TitlebarReportIcon().Click();
                    step25 = studyviewer.ReportContainer().Displayed && studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[4].ToLower()) &&
                       studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[3].ToLower());                   
                }
                if(step25)
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

                ExecutedSteps += ExecutedSteps + 14;

                //Step-26:Close the browser and load the URL to a browser again. -Select more than one study, may not belong to same patient. Click HTML5 Viewer button.
          /*      login = new Login();
                login.NavigateToIntegratorURL(url_18);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                ehr.selectPatient(Lastname[1]);
                ehr.selectPatient(Lastname[2]);
                ehr.selectPatient(Lastname[3]);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome"))
                {
                    studyviewer.LaunchStudyHTML5();
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(40);
                    PageLoadWait.WaitForFrameLoad(20);
                    if (studyviewer.html5seriesViewer_1X1().Displayed)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                }

                //Step-27:Apply tool : measurement, annotation.
                bool step_27 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                    viewer.ApplyTool_LineMeasurement();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_27 = studyviewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                }
                else
                {
                    studyviewer.DrawLineMeasurement(studyviewer.html5seriesViewer_1X1(), 50, 100);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    step_27 = studyviewer.CompareImage(result.steps[ExecutedSteps], studyviewer.studyPanel());
                }
                if (step_27)
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


                //Step-28:Go to the patient history drawer and click another study.
                bool step28 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    viewer.OpenPriors(1);
                    step28 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2)"));
                }
                else
                {
                    studyviewer.NavigateToHistoryPanel();
                    studyviewer.ChooseColumns(new string[] { "Accession" });
                    Dictionary<string, string> secondstudy28 = studyviewer.GetMatchingRow("Accession", Accessions[1]);
                    studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                    PageLoadWait.WaitForFrameLoad(20);
                    String Studyinfo28 = studyviewer.StudyInfo(2);
                    step28 = studyviewer.studyPanel(2).Displayed && secondstudy28["Accession"].Equals(Studyinfo28.Split(',')[0]);
                }
                if (step28)
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

                //Step-29:Go to the patient history drawer and click another study.
                bool step29 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    viewer.OpenPriors(2);
                    step29 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(3)"));
                }
                else
                {
                    studyviewer.NavigateToHistoryPanel();
                    Dictionary<string, string> thirdstudy29 = studyviewer.GetMatchingRow("Accession", Accessions[2]);
                    studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[2] });
                    PageLoadWait.WaitForFrameLoad(20);
                    String Studyinfo29 = studyviewer.StudyInfo(3);
                    step29 = studyviewer.studyPanel(3).Displayed && thirdstudy29["Accession"].Equals(Studyinfo29.Split(',')[0]);
                }
                if (step29)
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

                //Step-30:Close the browser.  From TestEHR load study with report to HTML5 viewer and check the report.
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True", viewName: "HTML5");
                ehr.SetSearchKeys_Study(Accessions[3]);
                String url_30 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_30);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                studyviewer.TitlebarReportIcon().Click();
                if (studyviewer.html5seriesViewer_1X1().Displayed && studyviewer.ReportContainer().Displayed && studyviewer.PatientDetailsInViewer()["PatientID"].ToLower().Equals(PIDList[4].ToLower()) &&
                   studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[3].ToLower()))
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

                //Step-31:Close Browser.
                ExecutedSteps++;

                //Step-32:In TESTEHR window, Set parameters:-Address = http:/localhost/WebAccess-Show Selector = false-Show Report = True-Auto End Session = True-for View Name field leave it as default (blank)-Enter search keys: -either study UID or Accession number. (study UID / Accession number must be unique) -click"Cmd line"button to create the test URL.
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False");
                ehr.SetSearchKeys_Study(Accessions[1]);
                String url_32 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step-33:Option 1: copy the URL to a browser and launch it. -Option 2: Click Load button to launch the default browser in server.
                login = new Login();
                login.NavigateToIntegratorURL(url_32);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (studyviewer.SeriesViewer_1X1().Displayed && studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[2].ToLower()) &&
                   studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[1].ToLower()))
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

                //Step-34:From the drop down menu of the View Name field, select"Study.review.start.HTML5"option. -Click"Load"button.
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", viewName: "HTML5");
                ehr.SetSearchKeys_Study(Accessions[1]);
                String url_34 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login = new Login();
                login.NavigateToIntegratorURL(url_34);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (studyviewer.html5seriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-35:Close Browser.
                ExecutedSteps++;

                //Step-36:Go to the service tool-*^>^* viewer-*^>^* Miscellaneous to enable the HTML5 Viewer Support and select HTML5 Viewer.
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.EnableHTML5();
                servicetool.CloseConfigTool();
                ExecutedSteps++;

                //Step-37:Launch TestEHR.exe from bin folder.
                ehr.LaunchEHR();
                ExecutedSteps++;

                //Step-38:In TESTEHR window, set parameters:-Address = http:/localhost/WebAccess-Show Selector = false-Show Report = True-Auto End Session = True-for View Name field leave it as default (blank)-Enter search keys: -either study UID or Accession number. (study UID / Accession number must be unique) -click"Cmd line"button to create the test URL.-
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False");
                ehr.SetSearchKeys_Study(Accessions[1]);
                String url_38 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //Step-39:Option 1: copy the URL to a browser and launch it. -Option 2: Click Load button to launch the default browser in server.
                login = new Login();
                login.NavigateToIntegratorURL(url_38);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (studyviewer.html5seriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-40:Close browser and back to TestEHR. From the drop down menu of the View Name field,  select"Integrator.study.review.start"option. -Click"Load"button.
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True", showSelector: "False", viewName: "HTML4");
                ehr.SetSearchKeys_Study(Accessions[1]);
                String url_40 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login = new Login();
                login.NavigateToIntegratorURL(url_40);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (studyviewer.SeriesViewer_1X1().Displayed && studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname[2].ToLower()) &&
                   studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[1].ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                } */


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


                //Return Result
                return result;
            }
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }


        /// <summary>
        /// Upgrade iCA only with RDM Datasource
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_87644(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables    
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIDs = PatientID.Split(':');
                String RDM_MP = login.GetHostName(Config.SanityPACS);
                String RDM_EA = login.GetHostName(Config.EA1);
                String EA1 = login.GetHostName(Config.EA1);
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] Lastnames = Lastname.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] Firstnames = Firstname.Split(':');
                String ConfigFileDirectory = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                String licencePath = ConfigFileDirectory + Path.DirectorySeparatorChar + "BluRingLicense.xml";

                //Precondition
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                icainstaller.installiCA(0);

                //Step-1:Launch iCA Service tool -> Navigate to Datasource tab -> Click on Add button. Select Remote Data Manager from the list ->Navigate to "Remote Data Manager" tab -> Provide the hostname -> Click on Apply -> Restart ServicesNote: No other datasources selected               
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    servicetool.AddLicenseInServiceTool(licencePath);
                }
                else
                    servicetool.AddLicenseInConfigTool(); //License file should be in the path as a testdata
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddRDMDatasource("10.9.37.108", "5");
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();
                ExecutedSteps++;

                //Step-2:Login as Admin user -> Navigate to Domain Management -> Connect the RDM -> Save changes           
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();
                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                RoleManagement rolemngt = (RoleManagement)login.Navigate("RoleManagement");
                rolemngt.CreateRole("SuperAdminGroup", "Physician", "physician");
                UserManagement usermngt = (UserManagement)login.Navigate("UserManagement");
                usermngt.CreateUser(Config.ph1UserName, "Physician", hasPass: 1, Password: Config.ph1Password);
                ExecutedSteps++;

                //Step-3:Navigate to Studies tab -> Search for any study and launch the study
                var studies = login.Navigate<Studies>();
                studies.RDM_MouseHover();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: Config.rdm1 + "." + RDM_MP);
                studies.SelectStudy("Patient ID", PatientIDs[0]);
                BluRingViewer Viewer = new BluRingViewer();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Equals(Lastnames[0] + ", " + Firstnames[0]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();

                }
                else
                {
                    var viewer = studies.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[0]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[0]))
                    {
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
                }
                login.Logout();

                //Step-4:Login as non-admin user and launch any study from studies list
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                login.RDM_MouseHover();
                login.SearchStudy(AccessionNo: Accessions[0], Datasource: Config.rdm1 + "." + RDM_MP);
                login.SelectStudy("Patient ID", PatientIDs[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Equals(Lastnames[0] + ", " + Firstnames[0]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    var viewer = login.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[0]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[0]))
                    {
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
                }
                login.Logout();

                //Step-5:Upgrade iCA to newer version (if 6.1 has been installed-> Upgrade it to 6.2)
                icainstaller.upgradeiCA();
                ExecutedSteps++;

                //Step-6:Login as Admin user -> Navigate to Studies tab -> Search for any study and launch the study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.RDM_MouseHover();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: Config.rdm1 + "." + RDM_MP);
                studies.SelectStudy("Patient ID", PatientIDs[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Equals(Lastnames[0] + ", " + Firstnames[0]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    var viewer = studies.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[0]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[0]))
                    {
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
                }
                login.Logout();

                //Step-7:Login as Non-Admin user -> Navigate to Studies tab -> Search for any study and launch the study
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                login.RDM_MouseHover();
                login.SearchStudy(AccessionNo: Accessions[0], Datasource: Config.rdm1 + "." + RDM_MP);
                login.SelectStudy("Patient ID", PatientIDs[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Equals(Lastnames[0] + ", " + Firstnames[0]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    var viewer = login.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[0]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[0]))
                    {
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
                }
                login.Logout();

                //Step-8:Uninstall iCA-> Install a previous/old version
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                icainstaller.installiCA(0);
                ExecutedSteps++;

                //Step-9:Launch iCA Service tool -> Navigate to Datasource tab -> Click on Add button. Select Remote Data Manager from the list ->Navigate to "Remote Data Manager" tab -> Provide the hostname -> Click on Apply -> Restart Services
                //Note: RDM should contain multiple datasources connected
                //Note: Datasources D1 and D2 are also configured
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    servicetool.AddLicenseInServiceTool(licencePath);
                }
                else
                    servicetool.AddLicenseInConfigTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.AddRDMDatasource("10.9.37.108", "5");
                servicetool.AddEADatasource(Config.EA1, Config.EA1AETitle, "1");
                servicetool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                taskbar1 = new Taskbar();
                taskbar1.Show();
                ExecutedSteps++;

                //Step-10:Login as Admin user -> Navigate to Domain Management -> Connect the RDM and other datasources-> Save changes
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();
                //Change System Settings to All Dates
                settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                rolemngt = (RoleManagement)login.Navigate("RoleManagement");
                rolemngt.CreateRole("SuperAdminGroup", "Physician", "physician");
                usermngt = (UserManagement)login.Navigate("UserManagement");
                usermngt.CreateUser(Config.ph1UserName, "Physician", hasPass: 1, Password: Config.ph1Password);
                ExecutedSteps++;

                //Step-11:Navigate to Studies tab -> Select RDM -> Verify the multiple datasources of RDM are listed -> Launch any study from RDM datasource
                studies = login.Navigate<Studies>();
                studies.RDM_MouseHover();
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: Config.rdm1 + "." + RDM_EA);
                studies.SelectStudy("Accession", Accessions[1]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Contains(Lastnames[1] + ", " + Firstnames[1]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    var viewer = studies.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[1]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[1]))
                    {
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
                }
                login.Logout();

                //Step-12:Login as Non-admin user -> Navigate to Studies tab -> Select RDM -> Verify the multiple datasources of RDM are listed -> Launch any study from RDM datasource
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                login.RDM_MouseHover();
                login.SearchStudy(AccessionNo: Accessions[1], Datasource: Config.rdm1 + "." + RDM_EA);
                login.SelectStudy("Accession", Accessions[1]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Contains(Lastnames[1] + ", " + Firstnames[1]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    var viewer = login.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[1]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[1]))
                    {
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
                }


                //Step-13:Select any other datasources other than RDM and verify studies are loaded
                login.SearchStudy(AccessionNo: Accessions[1], Datasource: EA1);
                login.SelectStudy("Accession", Accessions[1]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Contains(Lastnames[1] + ", " + Firstnames[1]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    var viewer = login.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[1]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[1]))
                    {
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
                }
                login.Logout();

                //Step-14:Upgrade iCA to newer version (if 6.1 has been installed-> Upgrade it to 6.2)
                icainstaller.upgradeiCA();
                ExecutedSteps++;

                //Step-15:Login as Admin user-> Navigate to Studies tab -> Select RDM -> Verify the multiple datasources of RDM are listed -> Launch any study from RDM datasource
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.RDM_MouseHover();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: Config.rdm1 + "." + RDM_MP);
                studies.SelectStudy("Patient ID", PatientIDs[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Equals((Lastnames[0] + ", " + Firstnames[0])))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();

                }
                else
                {
                    var viewer = studies.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[0]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[0]))
                    {
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
                }
                login.Logout();

                //Step-16:Login as Non-Admin User -> Navigate to Studies tab -> Select RDM -> Verify the multiple datasources of RDM are listed -> Launch any study from RDM datasource
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                login.RDM_MouseHover();
                login.SearchStudy(AccessionNo: Accessions[0], Datasource: Config.rdm1 + "." + RDM_MP);
                login.SelectStudy("Patient ID", PatientIDs[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Equals((Lastnames[0] + ", " + Firstnames[0])))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else

                {
                    var viewer = login.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[0]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[0]))
                    {
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
                }



                //Step-17:Select any other datasources other than RDM and verify studies are loaded
                login.SearchStudy(AccessionNo: Accessions[1], Datasource: EA1);
                login.SelectStudy("Accession", Accessions[1]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer();
                    String PatientName = Viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                    if (PatientName.Contains(Lastnames[1] + ", " + Firstnames[1]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    Viewer.CloseBluRingViewer();
                }
                else
                {
                    var viewer = login.LaunchStudy();
                    if (viewer.ViewStudy() && viewer.PatientDetailsInViewer()["LastName"].Equals(Lastnames[1]) && viewer.PatientDetailsInViewer()["FirstName"].Equals(Firstnames[1]))
                    {
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
                }
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


                //Return Result
                return result;
            }
            finally
            {
                //Uninstalling the existing build
                Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
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
