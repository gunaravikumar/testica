using System;
using System.Threading;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.eHR;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using System.ServiceProcess;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Selenium.Scripts.Tests
{
    class eMixGatewayIntegration : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public POPUploader pop { get; set; }
        public MpacLogin mpaclogin { get; set; }
        String User1 = "User1_" + new Random().Next(1, 10000);
        //public ServiceConfigTool servicetool { get; set; }
        public Taskbar taskbar { get; set; }
        public eMix emix { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public eMixGatewayIntegration(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Helper execution case for Admin install - This Test Case is From Access Transfer to eMix Gateway
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_166850(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            DomainManagement domainmanagement = new DomainManagement();
            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            Studies studies = new Studies();
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            string domain = Config.adminGroupName;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String FolderPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FolderPath");
                //String ClientIPAddress = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ClientIPAddress");
                String clientIP = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "clientIP");
                String eMixAETList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "eMixAET");
                String[] eMixAET = eMixAETList.Split(':');
                String DatasourceIPList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DatasourceIP");
                String[] DatasourceIP = DatasourceIPList.Split(':');
                string TransferserviceAETitle = "TFR_45";
                String PACSUsername = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PACSUsername");
                String PACSPassword = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PACSPassword");
                String DatasourceNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DatasourceName");
                String[] DatasourceName = DatasourceNameList.Split(':');
                String Testdomain = "Test478_DomainA" + new Random().Next(1, 1000);
                String TestdomainAdmin = "Test478_DomainAdminA" + new Random().Next(1, 1000);
                String Testrole1 = "Test478_Role1A" + new Random().Next(1, 1000);
                String Testuser1 = "Test478_User1A" + new Random().Next(1, 1000);
                String[] datasources = null;
                //Step-1: Pre-conditions - Systems needed: 
                //1.iCA Server under test with eMix Gateway EMIX_STORE_SCP
                //2.ICA client system
                //3.eMix client system (Gateway EMIX_STORE_SCP1, EMIX_STORE_SCP2)
                //4.A MergePACS connected to eMix gateway as destinations, not visible in iCA server under test
                ExecutedSteps++;

                //Step-2: "Pre-conditions - eMix gateway setups if eMix gateway has not installed.
                //1.Ensure eMix users (eMixUser1, eMixUser2) with a valid email address are available
                //2.On a system login eMix Website(e.g., https://emix.emix.com/) using eMix user account 
                //3.Refer 'eMix Gateway Implementation Guide' to complete the installation and configurations:
                //a.Install the eMix Gateway if not done yet
                //b.Configure SCP with AE and port number, e.g.EMIX_STORE_SCP, EMIX_STORE_SCP1 on iCA server and eMix client system (refer next step for adding more SCPs in the same gateway)
                //c.On the eMix client system configure storage SCU DICOM node (i.e. using MergePACS) "
                SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.AddEADatasource(clientIP, eMixAET[0], "87", dataSourceName: eMixAET[0], port: "104");
                servicetool.AddEADatasource(clientIP, eMixAET[1], "88", dataSourceName: eMixAET[1], port: "105");
                servicetool.AddEADatasource(clientIP, eMixAET[2], "89", dataSourceName: eMixAET[2], port: "106");
                servicetool.AddPacsDatasource(DatasourceIP[0], DatasourceName[0], "90", PACSUsername, PACSPassword);
                servicetool.AddPacsDatasource(DatasourceIP[1], DatasourceName[1], "91", PACSUsername, PACSPassword);
                servicetool.AddEADatasource(DatasourceIP[2], DatasourceName[2], "92", dataSourceName: DatasourceName[2]);
                servicetool.AddEADatasource(DatasourceIP[3], DatasourceName[3], "93", dataSourceName: DatasourceName[3], IsHoldingPen: 1);
                servicetool.AddPacsDatasource(DatasourceIP[4], DatasourceName[4], "94", PACSUsername, PACSPassword);
                //wpfobject.GetMainWindowByTitle(servicetool.ConfigTool_Name);
                bool datasourceAdded2_1 = wpfobject.VerifyIfTextExists(eMixAET[0]);
                bool datasourceAdded2_2 = wpfobject.VerifyIfTextExists(eMixAET[1]);
                bool datasourceAdded2_3 = wpfobject.VerifyIfTextExists(eMixAET[2]);
                if (datasourceAdded2_1 && datasourceAdded2_2 && datasourceAdded2_3)
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

                //step 3: "Pre-conditions: - iCA configurations
                //1.From ICA Service Tool add both eMix SCPs in Data Source tab as DICOM type.
                //e.g.EMIX_STORE_SCP, EMIX_STORE_SCP1, EMIX_STORE_SCP2
                //2.Enable Data Transfer in Service Tool\Enable Features\General sub-tab
                //3.Enable Transfer Service in Service Tool\Enable Features\Transfer Service sub - tab
                //Restart IIS and Windows Services
                //From ICA website login as Administrator, create a new domain/ user role / user for test eMix, ensure the user has access to all configured data sources"
                servicetool.RestartIISandWindowsServices();
                //wpfobject.GetMainWindowByTitle(ServiceConfigTool.ConfigTool_Name);
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableDataTransfer();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.SetTransferserviceAETitle(TransferserviceAETitle);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                taskbar.Show();

                //step 4: "Pre-conditions: - iCA configurations
                //From ICA website login as Administrator, create a new domain/ user role / user for test eMix, ensure the user has access to all configured data sources"
                //Testuser1 = "Test478_User1A782";
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(domain);
                domainmanagement.SelectDomain(domain);
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().SendKeys("eMixTest");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.SaveDomainButtoninEditPage().Click();
                domainmanagement.CloseAlertButton().Click();
                //domainmanagement.ClickSaveDomain();

                domainmanagement.CreateDomain(Testdomain, TestdomainAdmin, datasources: datasources);
                domainmanagement.ClickSaveNewDomain();
                bool domainA = domainmanagement.DomainExists(Testdomain);
                domainmanagement.SearchDomain(Testdomain);
                domainmanagement.SelectDomain(Testdomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.AddAllToolsToToolBar();
                domainmanagement.ClickSaveEditDomain();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(Testdomain, Testrole1, "any");
                rolemanagement.SelectDomainfromDropDown(Testdomain);
                bool role1 = rolemanagement.RoleExists(Testrole1);
                rolemanagement.SelectDomainfromDropDown(Testdomain);
                rolemanagement.SearchRole(Testrole1);
                rolemanagement.SelectRole(Testrole1);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.ClickSaveEditRole();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Testuser1, Testdomain, Testrole1);
                bool userA1 = usermanagement.SearchUser(Testuser1, Testdomain);
                login.Logout();
                ExecutedSteps++;
                // Adding Pre-Condition to generate POP Gateway
                taskbar = new Taskbar();
                taskbar.Hide();
                //servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.UpdateInstallerUrl();
                servicetool.GenerateInstallerPOP("SuperAdminGroup");
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
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

                //Return Result
                return result;
            }
        }

        
        
        /// <summary> 
        /// This Test Case is From eMix Gateway push data to Access POP
        /// </summary>
        public TestCaseResult Test_163186(String testid, String teststeps, int stepcount)
        {
            //Initial Setup
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //ServiceConfigTool servicetool = new ServiceConfigTool();
            DomainManagement domainmanagement = new DomainManagement();
            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            Studies studies = new Studies();
            StudyViewer studyViewer = new StudyViewer();
            eMix emix = new eMix();
            Inbounds inbounds = new Inbounds();
            POPUploader pop = new POPUploader();
            Taskbar taskbar = new Taskbar();
            login = new Login();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String clientIP = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "clientIP");
            String emixUserList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "emixUser");
            String[] emixUser = emixUserList.Split(':');
            String emixpaswrd = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "emixpaswrd");
            String PatientNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
            String[] PatientName = PatientNameList.Split(':');
            String eMixAETList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "eMixAET");
            String[] eMixAET = eMixAETList.Split(':');
            String POPDestList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "POPDestList");
            String[] POPDest = POPDestList.Split(':');
            String Port = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Port");
            String IP = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IP");
            string phusrnm = Config.ph1UserName;
            string phpswrd = Config.ph1Password;
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String[] Patientid = PatientID.Split(':');
            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
            String StudyDateTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDateTime");
            String[] Accession = AccessionList.Split(':');
            string TransferserviceAETitle = "TFR_45";
            String Testdomain = "Test479_DomainA" + new Random().Next(1, 1000);
            String TestdomainAdmin = "Test479_DomainAdminA" + new Random().Next(1, 1000);
            String Testrole1 = "Test479_Role1A" + new Random().Next(1, 1000);
            String Testuser1 = "Test479_User1A" + new Random().Next(1, 1000);
            String[] datasources = null;
            String DatasourceNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DatasourceName");
            String[] DatasourceName = DatasourceNameList.Split(':');
            //String[] Patientid = PatientID.Split(':');



            try
            {
                //Step-1: "Pre-conditions - Systems needed: 
                //1.iCA Server under test with eMix Gateway and POP installed
                //2.EA as Holding Pen
                //3.A client system to be used as 2nd ICA POP
                //4.A MergePACS and an EA that are connected iCA server as destinations, they are not visible in eMix Gateway"
                ExecutedSteps++;

                
                //Step-2: "Pre-conditions - iCA server and POP clients setups
                //1.Install and configure iCA server with image sharing enabled
                //2.Two destinations are created(e.g.one ICAD1 to MergePACS, ICAD2 to EA storage)
                //3.On ICA server and client machines, POP is installed
                //4.Ensure the iCA image sharing workflow are working as expected, e.g.study can be uploaded via the POP to its destination."
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                //Create Different Role and uers
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (!rolemanagement.RoleExists("Staff"))
                {
                    rolemanagement.CreateRole("SuperAdminGroup", "Staff", "");
                }
                if (!rolemanagement.RoleExists("Physician"))
                {
                    rolemanagement.CreateRole("SuperAdminGroup", "Physician", "physician");
                }
                if (!rolemanagement.RoleExists("Archivist"))
                {
                    rolemanagement.CreateRole("SuperAdminGroup", "Archivist", "archivist");
                }

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Config.stUserName, "SuperAdminGroup", "Staff", 1, Config.emailid, 1, Config.stPassword);
                usermanagement.CreateUser(Config.phUserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.phPassword);
                usermanagement.CreateUser(Config.ph1UserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.ph1Password);
                usermanagement.CreateUser(Config.ph2UserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.ph2Password);
                usermanagement.CreateUser(Config.arUserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.arPassword);
                usermanagement.CreateUser(Config.ar1UserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.ar1Password);
                usermanagement.CreateUser(Config.ar2UserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.ar2Password);
                usermanagement.CreateUser(Config.newUserName, "SuperAdminGroup", "Staff", 1, Config.emailid, 1, Config.newPassword);

                //Create Instituitions
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                AddInstitution(Config.Inst1, Config.ipid1);
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                AddDestination(Config.adminGroupName, POPDest[0], GetHostName(Config.DestinationPACS), Config.ph1UserName, Config.ar1UserName);
                AddDestination(Config.adminGroupName, POPDest[1], DatasourceName[2], Config.ph1UserName, Config.ar1UserName);
                login.Logout();
                taskbar = new Taskbar();
                taskbar.Hide();
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);
                //Delete installer if already exists
                if (pop.IsPACSGatewayInstalled())
                {
                    pop.UnInstallPACSGateway();
                }
                try
                {
                    var dir = new DirectoryInfo(InstallerPath);
                    foreach (var file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    DownloadInstaller(login.url, "POP", InstallerPath + "\\" + pop.PACSGatewayInstallerName, Config.adminGroupName);
                }
                catch (Exception ex)
                {
                    DownloadInstaller(login.url, "POP", InstallerPath + "\\" + pop.PACSGatewayInstallerName, Config.adminGroupName);
                }
                wpfobject.InvokeApplication(InstallerPath + "\\" + pop.PACSGatewayInstallerName);
                Thread.Sleep(10000);
                //Thread.Sleep(50000);
                wpfobject.GetMainWindow(PacsGatewayInstance + " Setup");
                wpfobject.WaitTillLoad();
                wpfobject.FocusWindow();
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(PacsGatewayInstance + " Setup");
                wpfobject.ClickButton("Next", 1); //Next button
                //int ctr = 0;
                wpfobject.GetMainWindow(PacsGatewayInstance + " Setup");
                wpfobject.SelectCheckBox(pop.PACSGatewaySetupEULAChckbx);
                wpfobject.GetMainWindow(PacsGatewayInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                //wpfobject.GetMainWindowByTitle(pop.PacsGatewaySetupWndwName);
                GetInstitutionPin(Config.Inst1);
                wpfobject.GetMainWindow(PacsGatewayInstance + " Setup");
                wpfobject.SetText(pop.PACSGatewaySetupRgstrtionPIN, pin);
                wpfobject.GetMainWindow(PacsGatewayInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                Thread.Sleep(500);
                wpfobject.GetMainWindowByTitle(PacsGatewayInstance);
                //wpfobject.GetMainWindowByTitle(base.PacsGatewayInstance);
                wpfobject.SetText(pop.PACSGatewaySetupEmail, Config.emailid);
                wpfobject.SetText(pop.PACSGatewaySetupSCPPort, "107");
                wpfobject.SelectTableCheckBox(0, 2);
                wpfobject.SelectTableCheckBox(1, 2);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(PacsGatewayInstance);
                wpfobject.ClickButton("nextButton");
                Thread.Sleep(500);
                wpfobject.GetMainWindowByTitle(PacsGatewayInstance + " Setup");
                //wpfobject.GetMainWindowByTitle(pop.PacsGatewaySetupWndwName);
                wpfobject.ClickButton("Next", 1);
                wpfobject.GetMainWindowByTitle(PacsGatewayInstance + " Setup");
                wpfobject.ClickButton(pop.PACSGatewaySetupInstall);
                pop.POP_WaitTillInstallationFinishes();
                wpfobject.GetMainWindowByTitle(PacsGatewayInstance + " Setup");
                //wpfobject.GetMainWindowByTitle(pop.PacsGatewaySetupWndwName);
                wpfobject.ClickButton("Finish", 1);
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                bool step2_1 = dest.SearchDestination(Config.adminGroupName, POPDest[1]);
                bool step2_2 = dest.SearchDestination(Config.adminGroupName, POPDest[0]);
                if(step2_1 && step2_2)
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

                //string devID = pop.GetPOPDeviceID(pop.GetPACSGatewayInstalledPath());
                _installedPath = pop.GetPACSGatewayInstalledPath() + @"\ConfigTool\" + PacsGatewayInstance + " ConfigTool.exe";
                wpfobject.InvokeApplication(_installedPath);
                Thread.Sleep(50000);
                wpfobject.GetMainWindowByTitle("PACS Gateway Configuration");
                var destlist = WpfObjects._mainWindow.Get<ListView>(SearchCriteria.Indexed(0));
                if (destlist != null)
                {
                    var cell = destlist.Rows[0].Cells[1];
                    if (cell != null)
                    cell.SetValue("ICAD1");
                    {
                        cell.DoubleClick();
                        for (int g = 1; g <= 8; g++)
                        {
                            cell.KeyIn(KeyboardInput.SpecialKeys.DELETE);
                        }
                        for (int o = 1; o <= 8; o++)
                        {
                            cell.KeyIn(KeyboardInput.SpecialKeys.BACKSPACE);
                        }

                        //cell.KeyIn("a");
                        cell.Enter("ICAD1");
                    }
                }
                    destlist = WpfObjects._mainWindow.Get<ListView>(SearchCriteria.Indexed(0));
                    if (destlist != null)
                    {
                        var cell2 = destlist.Rows[1].Cells[1];
                    if (cell2 != null)
                        cell2.DoubleClick();
                    for (int g = 1; g <= 8; g++)
                    {
                        cell2.KeyIn(KeyboardInput.SpecialKeys.DELETE);
                    }
                    for (int o = 1; o <= 8; o++)
                    {
                        cell2.KeyIn(KeyboardInput.SpecialKeys.BACKSPACE);
                    }

                    //cell.KeyIn("a");
                    cell2.Enter("ICAD2");
                }
                wpfobject.GetMainWindowByTitle("PACS Gateway Configuration");
                wpfobject.ClickButton(pop.PACSGatewaySave);
                wpfobject.ClickButton("okButton");

                //step-3: "Pre-conditions - eMix gateway setups if eMix gateway has not installed.
                //1.Ensure eMix users (eMixUser1, eMixUser2) with a valid email address is available
                //2.On the same client machine as POP login eMix Website(e.g., https://emix.emix.com/) using eMix user account, eMIxUser1
                //3.Refer 'eMix Gateway Implementation Guide' to complete the installation and configurations:
                //a.Install the eMix Gateway if not done yet
                //b.Configure SCU1 to push data to one of destination in the POP (e.g.ICAD1's AE, IP and Port#)
                //c.Ensure DICOM connection is passed test
                //d.Logout
                //4.Login the eMix Gateway(e.g.eMixUser2) configure SCU2 to push data to the 2nd destination in the POP(e.g.ICAD2's AE, IP(same) and Port#)
                //5.Ensure  both eMix Users has studies stored at eMix Local gateway and on eMix Cloud"
                emix.KilleMix();
                Thread.Sleep(1000);
                wpfobject.InvokeApplication(emix.eMixAppPath);
                Thread.Sleep(9000);
                wpfobject.GetMainWindowByTitle(emix.eMixWinName);
                wpfobject.SetText(emix.eMixLoginMail, emixUser[0]);
                wpfobject.SetText(emix.eMixPassword, emixpaswrd);
                wpfobject.ClickButton(emix.eMixLoginBtn, 1);
                Thread.Sleep(5000);
                wpfobject.ClickButton(emix.FileMenu, 1);
                wpfobject.ClickButton(emix.OptionsMenu);
                wpfobject.SelectCheckBox(emix.SendImags);
                wpfobject.ClickButton(emix.eMixGatewayNext);
                wpfobject.GetMainWindowByTitle("Setup DICOM Send Destination");
                wpfobject.SetText(emix.SCUDesc, POPDest[0]);
                wpfobject.SetText(emix.SCUAET, POPDest[0]);
                wpfobject.SetText(emix.SCUIP, Config.Popclient1);
                wpfobject.SetText(emix.SCUPort, Port);
                wpfobject.ClickButton(emix.eMixGatewayNext);
                bool status18_1 = wpfobject.GetElement<Label>(emix.SCUStatus).Text.Contains("Passed");
                wpfobject.ClickButton(emix.eMixGatewayNext);
                wpfobject.GetMainWindowByTitle("eMix Gateway Setup Finish");
                wpfobject.ClickButton(emix.SCUApply, 1);
                emix.KilleMix();
                Thread.Sleep(1000);
                wpfobject.InvokeApplication(emix.eMixAppPath);
                Thread.Sleep(9000);
                wpfobject.GetMainWindowByTitle(emix.eMixWinName);
                wpfobject.SetText(emix.eMixLoginMail, emixUser[1]);
                wpfobject.SetText(emix.eMixPassword, emixpaswrd);
                wpfobject.ClickButton(emix.eMixLoginBtn, 1);
                Thread.Sleep(5000);
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                wpfobject.ClickButton(emix.FileMenu, 0);
                wpfobject.ClickButton(emix.OptionsMenu);
                wpfobject.SelectCheckBox(emix.SendImags);
                wpfobject.ClickButton(emix.eMixGatewayNext);
                wpfobject.GetMainWindowByTitle("Setup DICOM Send Destination");
                wpfobject.SetText(emix.SCUDesc, POPDest[1]);
                wpfobject.SetText(emix.SCUAET, POPDest[1]);
                wpfobject.SetText(emix.SCUIP, Config.Popclient1);
                wpfobject.SetText(emix.SCUPort, Port);
                wpfobject.ClickButton(emix.eMixGatewayNext);
                bool status18_2 = wpfobject.GetElement<Label>(emix.SCUStatus).Text.Contains("Passed");
                wpfobject.ClickButton(emix.eMixGatewayNext);
                wpfobject.GetMainWindowByTitle("eMix Gateway Setup Finish");
                wpfobject.ClickButton(emix.SCUApply, 1);
                Kill_EXEProcess(emix.eMixExe);
                if (status18_1 && status18_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found");
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //step 4: "[Push study from eMix Gateway Local studies to iCA POP destination = MergePACS]
                //1.Login eMix Gateway as a user, e.g.eMixUser1
                //2.Select study from eMix gateway Local Studies
                //3.Select DICOM Send
                //4.Select configured SCU1, e.g.ICAD1
                //5.Send the study
                //6.Go to the iCA POP configured in the Pre-conditions
                //7.Select Active Transfers tab
                //8.Select Transfer History tab"
                emix.KilleMix();
                Thread.Sleep(1000);
                wpfobject.InvokeApplication(emix.eMixAppPath);
                Thread.Sleep(9000);
                wpfobject.GetMainWindowByTitle(emix.eMixWinName);
                wpfobject.SetText(emix.eMixLoginMail, emixUser[0]);
                wpfobject.SetText(emix.eMixPassword, emixpaswrd);
                wpfobject.ClickButton(emix.eMixLoginBtn, 1);
                Thread.Sleep(5000);
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                wpfobject.SelectTabFromTabItems(emix.LocalStdudiesTab);
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                emix.LocalSTudiesGrid().Rows[2].Click();
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                wpfobject.ClickButton(emix.DICOMSend);
                Thread.Sleep(1000);
                wpfobject.SelectFromComboBox(emix.DestCombobox, POPDest[0], byoption: 1);
                Thread.Sleep(1000);
                wpfobject.GetMainWindowByTitle(emix.DICOMSendWndw);
                wpfobject.ClickButton(emix.SendBtn);
                Thread.Sleep(1000);
                wpfobject.InvokeApplication(pop.GetPACSGatewayInstalledPath());
                Thread.Sleep(50000);
                wpfobject.GetMainWindowByTitle(pop.PACSConfigTool);
                wpfobject.SelectTabFromTabItems(pop.ActiveTransfersTab);
                wpfobject.ClickButton(pop.PACSGatewayRefreshtransfer);
                bool uploading = false;
                string trnsfrstatus = null;
                var uploadlist = WpfObjects._mainWindow.Get<ListView>(SearchCriteria.Indexed(0));
                if (uploadlist != null)
                {
                    var cell = uploadlist.Rows[0].Cells[5];
                    if (cell != null)
                        trnsfrstatus = cell.Name;
                }
                if (trnsfrstatus.Equals("Validating") || trnsfrstatus.Equals("In progress"))
                    uploading = true;
                Thread.Sleep(1000);
                wpfobject.SelectTabFromTabItems(pop.TransferHistoryTab);
                wpfobject.ClickButton(pop.PACSGatewaySearch);
                string trnsfrstatus29 = null;
                bool imag29 = false;
                var uploadlist29 = WpfObjects._mainWindow.Get<ListView>(SearchCriteria.Indexed(0));
                if (uploadlist29 != null)
                {
                    var cell29 = uploadlist29.Rows[0].Cells[4];
                    if (cell29 != null)
                        trnsfrstatus29 = cell29.Name;
                }
                if (trnsfrstatus29.Equals("Success"))
                    imag29 = true;
                if (imag29 && uploading)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found");
                }

                //step 5: Login ICA as a user who is able to see studies in Inbounds list.
                login.DriverGoTo(login.url);
                login.LoginIConnect(phusrnm, phpswrd);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllInboundData();
                inbounds.SearchStudy(AccessionNo: Accession[2]);
                inbounds.SelectStudy("Accession", Accession[2]);
                string[] list30_1 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Status", BasePage.GetColumnNames());
                string[] list30_2 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Number of Images", BasePage.GetColumnNames());
                bool chk30_1 = list30_1[0].ToString().Equals("Uploaded");
                if (chk30_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found");
                }

                //step 6: Load the study and verify the patient info and image displaying with its source on eMix Gateway.
                //studies.SelectAllDateAndData();
                //studies.SearchStudy("Last Name", PatientID);
                //studies.SelectStudy("Patient ID", PatientID);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                var view = Driver.FindElement(By.CssSelector(BluRingViewer.div_studypanel));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step1 = false;
                step1 = studies.CompareImage(result.steps[ExecutedSteps], view);
                if (step1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study transferred from iCA");
                }
                studyViewer.CloseStudy();

                //step 7: From iCA Inbounds page archive the study to its destination, e.g. ICAD1: MergePACS
                inbounds.SelectStudy("Accession", Accession[2]);
                inbounds.NominateForArchive("108478");
                login.Logout();
                login.LoginIConnect(Config.ar1UserName, Config.arPassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllDateAndData();
                inbounds.SearchStudy(AccessionNo: Accession[2]);
                IWebElement uploadComments, archiveOrder;
                inbounds.SelectStudy("Accession", Accession[2]);
                inbounds.ClickArchiveStudy(out uploadComments, out archiveOrder);
                inbounds.ClickArchive();
                inbounds.SearchStudy(AccessionNo: Accession[2]);
                Dictionary<string, string> studyrow17 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Routing Completed" });
                if (studyrow17 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study recieved in eMix Gateway");
                }

                //step 8: Verify study that is archived and stored on the destination, e.g., MergePACS.
                //studies.SearchStudy("Last Name", PatientID);
                studies.SelectStudy("Patient ID", Patientid[2]);
                BluRingViewer.LaunchBluRingViewer();
                var view8 = Driver.FindElement(By.CssSelector(BluRingViewer.div_studypanel));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], view8);
                if (step8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study recieved in eMix Gateway");
                }
                CloseStudy();
                login.Logout();

            //step 9: "[Push study from eMix Cloud to iCA POP destination = EA]
            //1.Login eMix via it website https://emix.emix.com/ e.g. eMixUser2
            //2.Select study from My eMix/ Inbox
            //3.Select DICOM Send To...
            //4.Select configured SCU2, e.g.ICAD2
            //5.Send the study
            //6.Go to the iCA POP configured in the Pre-conditions
            //7.Select Active Transfers tab
            //8.Select Transfer History tab"
            emix.KilleMix();
                Thread.Sleep(1000);
                wpfobject.InvokeApplication(emix.eMixAppPath);
                Thread.Sleep(9000);
                wpfobject.GetMainWindowByTitle(emix.eMixWinName);
                wpfobject.SetText(emix.eMixLoginMail, emixUser[1]);
                wpfobject.SetText(emix.eMixPassword, emixpaswrd);
                wpfobject.ClickButton(emix.eMixLoginBtn, 1);
                Thread.Sleep(5000);
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                wpfobject.SelectTabFromTabItems(emix.LocalStdudiesTab);
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                emix.LocalSTudiesGrid().Rows[2].Click();
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                wpfobject.ClickButton(emix.DICOMSend);
                wpfobject.SelectFromComboBox(emix.DestCombobox, POPDest[1]);
                wpfobject.GetMainWindowByTitle(emix.DICOMSendWndw);
                wpfobject.ClickButton(emix.SendBtn);
                wpfobject.InvokeApplication(pop.GetPACSGatewayInstalledPath());
                Thread.Sleep(50000);
                wpfobject.GetMainWindowByTitle(pop.PACSConfigTool);
                wpfobject.SelectTabFromTabItems(pop.ActiveTransfersTab);
                wpfobject.ClickButton(pop.PACSGatewayRefreshtransfer);
                bool uploading9 = false;
                var uploadlist9 = WpfObjects._mainWindow.Get<ListView>(SearchCriteria.Indexed(0));
                if (uploadlist9 != null)
                {
                    var cell = uploadlist.Rows[0].Cells[5];
                    if (cell != null)
                        trnsfrstatus = cell.Name;
                }
                if (trnsfrstatus.Equals("Validating") || trnsfrstatus.Equals("In progress"))
                    uploading = true;
                wpfobject.SelectTabFromTabItems(pop.TransferHistoryTab);
                Thread.Sleep(1000);
                wpfobject.ClickButton(pop.PACSGatewaySearch);
                string trnsfrstatus9 = null;
                bool imag9 = false;
                uploadlist9 = WpfObjects._mainWindow.Get<ListView>(SearchCriteria.Indexed(0));
                if (uploadlist9 != null)
                {
                    var cell29 = uploadlist29.Rows[1].Cells[4];
                    if (cell29 != null)
                        trnsfrstatus29 = cell29.Name;
                }
                if (trnsfrstatus29.Equals("Success"))
                    imag29 = true;
                if (imag9 && uploading9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found");
                }

                //step 10: Login ICA as a user who is able to see studies in Inbounds list.
                login.DriverGoTo(login.url);
                login.LoginIConnect(phusrnm, phpswrd);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy(AccessionNo: Accession[0]);
                inbounds.SelectStudy("Accession", Accession[0]);
                string[] list10_1 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Status", BasePage.GetColumnNames());
                string[] list10_2 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Number of Images", BasePage.GetColumnNames());
                bool chk10_1 = list30_1[0].ToString().Equals("Uploaded");
                if (chk10_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not found");
                }

                //step 11: Load the study and verify the patient info and image displaying with its source on eMix Gateway.
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                BluRingViewer.LaunchBluRingViewer();
                var view11 = Driver.FindElement(By.CssSelector(BluRingViewer.div_studypanel));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step11 = false;
                step11 = studies.CompareImage(result.steps[ExecutedSteps], view11);
                if (step11)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study transferred from iCA");
                }
                studyViewer.CloseStudy();

                //step 12: From iCA Inbounds page archive the study to its destination, e.g. ICAD2: EA
                inbounds.NominateForArchive("108479");
                login.Logout();
                login.LoginIConnect(Config.ar1UserName, Config.arPassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllDateAndData();
                inbounds.SearchStudy(patientID: PatientID);
                IWebElement uploadComments12, archiveOrder12;
                inbounds.ClickArchiveStudy(out uploadComments, out archiveOrder);
                inbounds.ClickArchive();
                inbounds.SearchStudy("PatientID", PatientID);
                Dictionary<string, string> studyrow12 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Routing Completed" });
                if (studyrow12 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study recieved in eMix Gateway");
                }

                //step 13: Verify study that is archived and stored on the destination, e.g., EA
                studies.SearchStudy("Last Name", PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                studyViewer = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                var view13 = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step13 = false;
                step1 = studies.CompareImage(result.steps[ExecutedSteps], view13);
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
                    throw new Exception("Study recieved in eMix Gateway");
                }
                CloseStudy();
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
                emix.KilleMix();
                wpfobject.InvokeApplication(emix.eMixAppPath);
                Thread.Sleep(9000);
                wpfobject.GetMainWindowByTitle(emix.eMixWinName);
                wpfobject.SetText(emix.eMixLoginMail, emixUser[0]);
                wpfobject.SetText(emix.eMixPassword, emixpaswrd);
                wpfobject.ClickButton(emix.eMixLoginBtn, 1);
                Thread.Sleep(5000);
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                wpfobject.SelectTabFromTabItems(emix.LocalStdudiesTab);
                wpfobject.GetMainWindowByTitle(emix.eMixGateway);
                emix.LocalSTudiesGrid().Rows[0].Click();
                wpfobject.ClickButton(emix.DeleteStudies);
                wpfobject.GetMainWindowByTitle(emix.DeleteStudyWndw);
                wpfobject.ClickButton(emix.SelectAll);
                wpfobject.ClickButton(emix.DeleteAllSTudies);
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton(emix.DeleteYes);
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton(emix.DeleteOK);
                emix.KilleMix();
            }

        }

    }
}
