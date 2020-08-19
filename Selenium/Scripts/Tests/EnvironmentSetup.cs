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
using System.Xml.Linq;
using System.Diagnostics;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Xml;
using System.Net;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using Window = TestStack.White.UIItems.WindowItems.Window;


namespace Selenium.Scripts.Tests
{

    class EnvironmentSetup
    {
        //Fields
        ServiceTool tool;
        Login login;
        MpacLogin mpaclogin;
        MPHomePage mphomepage;
        MpacConfiguration mpacconfig;
        HPLogin hplogin;
        DomainManagement domainmanagement;
        UserManagement usermanagement;
        RoleManagement rolemanagement;
        POPUploader pop;
        WpfObjects wpfobject;
        ExamImporter ei;
        BasePage basepage;
        int DistanceCounter;
        String ServerName = null;

        //Email Details
        String EmailHeader = "Hi,\n\nFind below the Build Configuration Status.\n\n";
        String EmailFooter = "\r\n\nRegards,\nAutomation Team.";

        public string filepath { get; set; }
        UserPreferences userpref;

        public string TransferserviceAETitle;

        //Default Constructor
        public EnvironmentSetup(string classname)
        {
            this.tool = new ServiceTool();
            this.login = new Login();
            login.DriverGoTo(login.url);
            this.mpaclogin = new MpacLogin();
            this.mphomepage = new MPHomePage();
            this.hplogin = new HPLogin();
            this.domainmanagement = new DomainManagement();
            this.usermanagement = new UserManagement();
            this.pop = new POPUploader();
            this.ei = new ExamImporter();
            this.basepage = new BasePage();
            this.wpfobject = new WpfObjects();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            this.mpacconfig = new MpacConfiguration();
            ServerName = login.GetHostName(Config.IConnectIP);
            DistanceCounter = 1;

            this.userpref = new UserPreferences();
            this.TransferserviceAETitle = "TFR_" + new BasePage().GetHostName(Config.IConnectIP).Replace("-", "");
        }

        /// <summary>
        /// This Test method is  to install the Build and adding TestEHR files and adding the license.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_InstallBuild(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            //Current Directory path
            String currentDirectory = System.IO.Directory.GetCurrentDirectory();

            try
            {
                BasePage.Driver.Quit();
                BasePage.Driver = null;

                String Installer_Name = "";
                String InstallerEXE = "";
                String InstallBtn_Name = "";
                String FinishBtn_Name = "";
                String License_Name = "";
                //String Defaultwebsite = ""; 
                //String Customwebsite = ""; 
                //String NextBtn = ""; 
                DirectoryInfo BuildInfo = new DirectoryInfo(Config.BuildPath);

                //taking the latst build
                String SetupFilePath = "archive\\Output_ICA\\WebAccess_Release\\WebAccess";
                String EHRFilesPath = "archive\\Output_ICA\\TestTools";

                String ExePath = "";
                String EHRPath = "";

                string website = "default";

                //Getting Build details
                if (BuildInfo.Exists)
                {
                    Installer_Name = "IBM iConnect Access Setup";
                    InstallerEXE = "iCAInstaller";
                    InstallBtn_Name = "Install";
                    FinishBtn_Name = "Finish";
                    License_Name = "BluRingLicense.xml";
                    BasePage.LatestBuild_Path = login.LatestDirectory(Config.BuildPath);
                    ExePath = BasePage.LatestBuild_Path + "\\" + SetupFilePath;
                    EHRPath = BasePage.LatestBuild_Path + "\\" + EHRFilesPath;
                    //Defaultwebsite = "Default Web Site"; //Automation ID = 11259
                    //Customwebsite = "Custom Web Site"; //Automation ID = 309
                    //NextBtn = "Next"; //Automation ID = 11108
                }
                else
                {
                    Logger.Instance.ErrorLog("Build Directory not available");
                    System.Environment.Exit(-1);
                }

                if (String.IsNullOrEmpty(Config.FullUI_InstalltionMode.ToLower()) || Config.FullUI_InstalltionMode.ToLower().Equals("n"))
                {
                    Logger.Instance.InfoLog("Build Path is --" + ExePath + "\\" + InstallerEXE);
                    var psi = new ProcessStartInfo(ExePath + "\\" + InstallerEXE);
                    psi.UseShellExecute = true;
                    WpfObjects._application = TestStack.White.Application.AttachOrLaunch(psi);

                    Thread.Sleep(360000);
                    wpfobject.GetMainWindowByTitle(Installer_Name);
                    wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
                   // wpfobject.WaitForButtonExist(Installer_Name, NextBtn, 1);
                    Logger.Instance.InfoLog("Application launched : ");
                }
                else
                {
                    //Start process
                    var proc = new Process
                    {
                        StartInfo =
                        {
                            FileName = ExePath + "\\" + InstallerEXE,
                            Arguments = "FULLUI=Y",
                            WorkingDirectory = ExePath,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };
                    proc.Start();
                    Thread.Sleep(60000);
                    wpfobject.GetMainWindowByTitle(Installer_Name);
                    wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
                    Logger.Instance.InfoLog("Application launched in FULLUI mode");
                    //Set Shared DB details
                    wpfobject.SetText("DbInstance Name", Config.LB_SQLDBName.ToLower() + "\\WEBACCESS", byText: 1);
                    wpfobject.UnSelectCheckBox("Windows Authentication", byText: 1);
                    wpfobject.SetText("DbUserName", "sa", byText: 1);
                    wpfobject.SetText("DbPassword", "Cedara123", byText: 1);
                }
                Window window = wpfobject.GetMainWindowByTitle(Installer_Name);
                //if (string.Equals(website, "default"))
                //{
                //    //wpfobject.ClickButton(Defaultwebsite, 1);
                //    wpfobject.GetAnyUIItem<Window, RadioButton>(WpfObjects._mainWindow, Defaultwebsite, 1).Click();
                //    wpfobject.ClickButton(NextBtn, 1);
                //    wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
                //}
                //else
                //{
                //    wpfobject.GetAnyUIItem<Window, RadioButton>(WpfObjects._mainWindow, Customwebsite, 1).Click();
                //    wpfobject.ClickButton(NextBtn, 1);
                //    wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
                //}
                //wpfobject.GetMainWindowByTitle(Installer_Name);
                //wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
                //Thread.Sleep(60000);
                wpfobject.GetMainWindowByTitle(Installer_Name);
                wpfobject.ClickButton(InstallBtn_Name, 1);
                //Logger.Instance.InfoLog("Install button clicked for first time..");
                //try
                //{
                //    wpfobject.GetMainWindowByTitle(Installer_Name);
                //    wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
                //    Thread.Sleep(60000);
                //    wpfobject.GetMainWindowByTitle(Installer_Name);
                //    wpfobject.ClickButton(InstallBtn_Name, 1);
                //    Logger.Instance.InfoLog("Install button clicked for second time..");
                //}
                //catch (Exception e) { Logger.Instance.ErrorLog("Exception on clicking install button.." + e); }
                if (Config.FullUI_InstalltionMode.ToLower().Equals("y"))
                {
                    try
                    {
                        int loopCount = 0;
                        int WarningWindowCount = 0;
                        do
                        {
                            //Get all the windows on desktop
                            IList<Window> windows = TestStack.White.Desktop.Instance.Windows();
                            for (int i = 0; i < windows.Count; i++)
                            {
                                string winTitle = windows[i].Title.ToLower();
                                Logger.Instance.InfoLog("Window " + i + " title : " + winTitle);
                                if (winTitle.Equals(""))
                                {
                                    IList<Window> modalWindows = windows[i].ModalWindows();
                                    for (int j = 0; j < modalWindows.Count; j++)
                                    {
                                        string modalWinTitle = modalWindows[j].Title.ToLower();
                                        Logger.Instance.InfoLog("Modal Window " + j + " title : " + modalWinTitle);
                                        if (modalWinTitle.Equals("warning"))
                                        {
                                            Logger.Instance.InfoLog("Warning window found in loop: " + loopCount);
                                            modalWindows[j].Close();
                                            Logger.Instance.InfoLog("Warning window closed");
                                            WarningWindowCount++;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (WarningWindowCount == 0)
                                Thread.Sleep(30000);
                            if (WarningWindowCount == 1)
                                Thread.Sleep(15000);
                            loopCount++;
                        }
                        while (loopCount < 8 && WarningWindowCount < 2);
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.InfoLog("Exception while waiting for Warning window - " + e);
                    }
                }
                wpfobject.WaitForButtonExist(Installer_Name, FinishBtn_Name, 1);
                wpfobject.GetMainWindowByTitle(Installer_Name);
                wpfobject.ClickButton(FinishBtn_Name, 1);
                Thread.Sleep(10000);
                BasePage.Kill_EXEProcess(InstallerEXE);
                Thread.Sleep(10000);

                //Replacing the TestEHR Files
                String TestEHR_EXE_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.exe";
                String TestEHR_EXE_Config_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.exe.config";
                String TestEHR_pdp_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.pdb";
                String TestEHR_samlPolicy_config_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.samlPolicy.config";
                String SystemFactoryConfiguration_Path = EHRPath + Path.DirectorySeparatorChar + "SystemFactoryConfiguration.xml";
                String ServiceFactoryConfiguration_Path = EHRPath + Path.DirectorySeparatorChar + "ServiceFactoryConfiguration.xml";
                String PostFormTemplate_html = EHRPath + Path.DirectorySeparatorChar + "PostFormTemplate.html";

                //Destination
                String Destination_Path = "C:\\WebAccess\\WebAccess\\bin\\";
                File.Copy(TestEHR_EXE_Path, Destination_Path + Path.GetFileName(TestEHR_EXE_Path), true);
                File.Copy(TestEHR_EXE_Config_Path, Destination_Path + Path.GetFileName(TestEHR_EXE_Config_Path), true);
                File.Copy(TestEHR_pdp_Path, Destination_Path + Path.GetFileName(TestEHR_pdp_Path), true);
                File.Copy(TestEHR_samlPolicy_config_Path, Destination_Path + Path.GetFileName(TestEHR_samlPolicy_config_Path), true);
                File.Copy(SystemFactoryConfiguration_Path, Destination_Path + Path.GetFileName(SystemFactoryConfiguration_Path), true);
                File.Copy(ServiceFactoryConfiguration_Path, Destination_Path + Path.GetFileName(ServiceFactoryConfiguration_Path), true);
                File.Copy(PostFormTemplate_html, Destination_Path + Path.GetFileName(PostFormTemplate_html), true);

                //License update
                String LicensePath = "C:\\WebAccess\\WebAccess\\Config\\" + License_Name;

                //Config file Directory Path
                String ConfigFileDirectory = currentDirectory + Path.DirectorySeparatorChar + "ServerConfigFiles" +
                    Path.DirectorySeparatorChar + ServerName;

                //Config Files - File Path
                String License_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + License_Name;

                //Data Source Manager Configuration xml File
                try { File.Copy(License_Backup, LicensePath, true); }
                catch (Exception) { }

                //Run IISReset.exe file
                servicetool.RestartIISUsingexe();
                Thread.Sleep(10000);

                //Get the Build ID from Build.info 
                string line;
                string buildno = "";

                using (StreamReader sr = new StreamReader("C:\\WebAccess\\Build.Info"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("Build Number")) { buildno = line; break; }
                    }
                    buildno = buildno.Split(':')[1].Trim();
                }

                //Update config file with build ID
                Dictionary<String, String> buildnumber = new Dictionary<String, String>();
                buildnumber.Add("buildnumber", buildno);
                Dictionary<String, String> RDMBuildNo = new Dictionary<String, String>();
                RDMBuildNo.Add("buildnumber", buildno);
                ReadXML.UpdateXML(Config.inputparameterpath, buildnumber);
                ReadXML.UpdateXML(Config.inputparameterpath, RDMBuildNo);
                Config.buildnumber = buildno;
                Config.rdm = buildno;

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification -- ServerName + " - Build Installation Completed";
                String EmailSubject = Environment.MachineName.ToUpper() + " - Build Installation Completed";// " Server Name\t - " + ServerName + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Task\t - Build Installation\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                try { login.InvokeBrowser(Config.BrowserType); }
                catch (Exception) { }
                login.CreateNewSesion();
                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Build Installation Failed";
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Task\t - Build Installation\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                login.CreateNewSesion();
                return result;
            }
        }

        /// <summary>
        /// This Test method is  to Configure thr HTTPS in the server and add the node to host file.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_Configure_HTTPS(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                String HostName = servicetool.GetHostName(Config.IConnectIP);
                FileUtils.AddToHostsFile(Config.IConnectIP + " " + HostName.ToLower() + ".pqawhi.com");

                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolSecurityTab();
                servicetool.NavigateSubTab("General");
                servicetool.ClickModifyFromTab();
                servicetool.SetHTTPS(1);
                wpfobject.WaitTillLoad();
                servicetool.FQDN_txt().BulkText = HostName.ToLower() + ".pqawhi.com";
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                servicetool.AcceptDialogWindow();
                Thread.Sleep(5000);
                //Run IISReset.exe file
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                Thread.Sleep(1000);
                servicetool.CloseConfigTool();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - HTTPS Configuration Completed";// " Server Name\t - " + ServerName + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - HTTPS Configuration\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);
                login.CreateNewSesion();
                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - HTTPS Configuration Failed";// " Server Name\t - " + ServerName + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Build Installation\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                login.CreateNewSesion();
                return result;
            }
        }

        /// <summary>
        /// This Test method is to perform:
        /// 1. Data Cleanup in External Systems
        /// 2. Setup ImageSharing as Y in Web.config file
        /// 3. Adds the Data sources: Holding pen, EA and PACS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_PreCondition(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                //Set Enable ImageSharing value as true
                basepage.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");

                //***XDS Setup Commented for now***
                //Config Files - File Path
                //String XDSConfigPath = @"C:\WebAccess\WebAccess\Config\Xds\XdsConfiguration.xml";

                //Current Directory Path
                /*String currentDirectory = System.IO.Directory.GetCurrentDirectory();
                String ConfigFileDirectory = currentDirectory + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);

                //Config Files - File Path
                String XDSConfigPath_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + "XdsConfiguration.xml";

                //XDS Configuration xml file
                File.Copy(XDSConfigPath_Backup, XDSConfigPath, true); */

                //Enable Ldap Servers
                //tool.EnableLDAPConfigfile();
                //tool.RestartIIS();

                // Call function MinimizeAll
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //########## Adding Datasources ##############

                //Add EA holding pen as Datasource
                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                tool.AddEADatasource(Config.HoldingPenIP, Config.HoldingPenAETitle, "2", IsHoldingPen: 1);

                //Add Destination 1 - Pacs
                tool.AddPacsDatasource(Config.DestinationPACS, Config.DestinationPACSAETitle, "3", Config.pacsadmin, Config.pacspassword);

                //Add destination 2 - VNA
                tool.AddEADatasource(Config.DestEAsIp, Config.DestEAsAETitle, "4");

                //Add EA1 Datasource
                tool.AddEADatasource(Config.EA1, Config.EA1AETitle, "5");

                //Add Sanity PACS
                tool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "6", Config.pacsadmin, Config.pacspassword);

                //Add PACS-2
                tool.AddPacsDatasource(Config.PACS2, Config.PACS2AETitle, "7", Config.pacsadmin, Config.pacspassword);

                //Add EA-77 Datasource
                tool.AddEADatasource(Config.EA77, Config.EA77AETitle, "8");

                //Add EA-91 Datasource
                tool.AddEADatasource(Config.EA91, Config.EA91AETitle, "10");

                /*//Add EA-46 Datasource
                tool.AddEADatasource("10.4.38.46", "ECM_ARC_46", "11", dataSourceName: "EA-46");

                //***XDS Setup Commented for now***
                //Add Patient ID domains and Other Identifiers
                tool.SelectDataSource("EA-46");
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name,1);
                wpfobject.WaitTillLoad();
                tool.SetDataSourcePatientIDDomain("NYH", "NYH", "NYH&amp;&amp;");
                tool.SetOtherIdentifiers("REF_AE_1");
                tool.SetOtherIdentifiers("REF_AE_2");
                tool.SetOtherIdentifiers("REF_AE_3");
                tool.SetOtherIdentifiers("REF_AE_46");
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();

                //Add EA-116 Datasource
                tool.AddEADatasource("10.4.38.116", "ECM_ARC_116", "12", dataSourceName: "EA-116");

                //Add other identifiers
                tool.SelectDataSource("EA-116");
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name,1);
                wpfobject.WaitTillLoad();
                tool.SetDataSourcePatientIDDomain("TOH", "TOH", "TOH&amp;&amp;");
                tool.SetOtherIdentifiers("REF_AE_1");
                tool.SetOtherIdentifiers("REF_AE_2");
                tool.SetOtherIdentifiers("REF_AE_3");
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();

                //Add RDM DataSource
                tool.AddRDMDatasource("10.4.39.163", "13");

                //Add XDS Datasource
                tool.AddXDSDatasource("XDS_DS", "ECM_XDS_40", "");
                tool.SelectDataSource("XDS_DS");
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                tool.SetDataSourcePatientIDDomain("XDS", "XDS Affinity Domain", "&amp;2.16.840.1.113883.9.185&amp;ISO");
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();*/

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                login.CreateNewSesion();
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
                login.CreateNewSesion();
                return result;
            }
        }

        /// <summary>
        /// This Test method is to perform Steps:
        /// 1. Update Domain Management page in Iconnect
        /// 2. Set the holding pen flag as true
        /// 3. Enable different features in Service tool
        /// 4. Setup Transfer Service AE Title and port
        /// 5. Enable Image Sharing in domain management page
        /// 6. Generate Exam Importer
        /// 7. Create Users, Roles, Instituition, and Destination
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_GeneratePopandExamImporter(String testid, String teststeps, int stepcount)
        {

            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;
            Taskbar taskbar = null;

            try
            {

                bool blnUpdateDomainCheck = false;
                const string domainName = "SuperAdminGroup";

                //Update Super Admin Group in Domain Management (Connect all DataSource and set Institution name)            
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                blnUpdateDomainCheck = basepage.UpdateGivenDomain(domainName);
                login.Logout();

                //Hide TaskBar
                taskbar = new Taskbar();
                taskbar.Hide();

                //Set Holding Pen as True
                tool.InvokeServiceTool();

                //Setup Email Notification
                tool.SetEmailNotificationForPOP();

                //Enable Different Features
                tool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.EnablePatient();
                tool.EnableStudySharing();
                tool.EnableDataDownloader();
                tool.EnableDataTransfer();
                tool.EnableEmailStudy();
                tool.EnablePDFReport();
                tool.EnableRequisitionReport();
                tool.EnableSelfEnrollment();
                tool.EnableEmergencyAccess();
                tool.EnableBriefcase();
                tool.EnableConferenceLists();
                wpfobject.WaitTillLoad();
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                //Enable Merge EMPI
                //tool.EnableMergeEMPI();

                //Enable encapsulated report
                tool.EnableReports(false);
                //tool.EnableStudyAttachements(); This functionality has been removed
                tool.EnableHTML5();

                //Setup Transfer Service Config
                tool.SetTransferserviceAETitle(TransferserviceAETitle);

                //Generate Exam Importer and POP
                tool.UpdateInstallerUrl();
                tool.GenerateInstallerPOP("SuperAdminGroup", "");
                tool.GenerateInstallerAllDomain(domainName, Config.eiwindow);
                wpfobject.WaitTillLoad();
                tool.RestartService();

                //Enable LDAP setup
                tool.LaunchServiceTool();
                tool.NavigateToConfigToolUserMgmtDatabaseTab();
                tool.SetMode(2);
                tool.LDAPSetup();

                tool.CloseServiceTool();
                taskbar.Show();

                //Enable other otpions in Domain Management Screen
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                //domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("emergency", 0);
                domainmanagement.SetCheckBoxInEditDomain("breifcase", 0);
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();

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
                //usermanagement.CreateUser(Config.stUserName, "SuperAdminGroup", "Staff", 1, Config.emailid,1,Config.stPassword);
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
                basepage.AddInstitution(Config.Inst1, Config.ipid1);
                basepage.AddInstitution(Config.Inst2, Config.ipid2);

                //Create Destinations
                if (String.IsNullOrWhiteSpace(Config.DestinationPACS))
                {
                    Logger.Instance.ErrorLog("'Config.DestinationPACS' is Empty, So Creating only one destination using Destination EA");
                    if (!String.IsNullOrWhiteSpace(Config.DestEAsIp))
                        basepage.AddDestination("SuperAdminGroup", Config.Dest1, basepage.GetHostName(Config.DestEAsIp), Config.ph1UserName, Config.ar1UserName);
                    else
                    {
                        Logger.Instance.ErrorLog("'Config.DestEAsIp' is also empty. Cannot create Destination. Check the AutomationConfig.xml");
                        throw new Exception("Cannot create Destination as 'Config.DestinationPACS' and 'Config.DestEAsIp' are empty. Check the AutomationConfig.xml");
                    }
                }
                else
                {
                    basepage.AddDestination("SuperAdminGroup", Config.Dest1, basepage.GetHostName(Config.DestinationPACS), Config.ph1UserName, Config.ar1UserName);
                    basepage.AddDestination("SuperAdminGroup", Config.Dest2, basepage.GetHostName(Config.DestEAsIp), Config.ph2UserName, Config.ar2UserName);
                }
                basepage.CloseBrowser();

                //Enable Bypass mode
                login.UncommentXMLnode("id", "Bypass");

                //Enable Bluring Viewer
                //    TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing");
                //tool.LaunchServiceTool();
                //wpfobject.WaitTillLoad();
                //tool.RestartIISandWindowsServices();
                //tool.CloseServiceTool();

                //login.DriverGoTo(login.url);
                //login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                //userpref.OpenUserPreferences();
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                //userpref.BluringViewerRadioBtn().Click();
                //userpref.CloseUserPreferences();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                login.CreateNewSesion();
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
                login.CreateNewSesion();
                return result;
            }
        }

        /// <summary>
        /// This test method is to generate Exam Importer-2
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_GenerateExamImporter2(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                tool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                string Domain = "SuperAdminGroup";
                tool.GenerateInstallerAllDomain(Domain, Config.eiwindow2);
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification 
                String EmailSubject = Environment.MachineName.ToUpper() + " - Generate Exam Importer-2 Completed";// + "\r\n Server IP - " + Config.IConnectIP 
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Generate Exam Importer-2\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

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

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Generate Exam Importer-2 Failed";//  + "\r\n Server IP - " + Config.IConnectIP
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Generate Exam Importer-2\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test method is to trigger Pop installation in client machine belongs to step 13
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_TriggerPopInstall1(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                String response = SocketClient.Send(Config.Popclient1, 7777, @"D:\ProjectFiles\iConnectScripts\Selenium\bin\Release\Selenium.exe -file D:\ProjectFiles\iConnectScripts\Automation_Config.xml");
                SocketClient.Close();

                //Update Result
                if (response.Equals("33Ended"))
                { result.steps[++executedSteps].status = "Pass"; }
                else
                { result.steps[++executedSteps].status = "Fail"; }
                result.FinalResult(executedSteps);
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
        /// This will install the POP client of Institution-2 in a different machine
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_TriggerPopInstallInRemote(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                String response = SocketClient.Send(Config.Popclient1, 7777, @"D:\BatchExecution\Selenium\bin\Debug\Selenium.exe -configfile D:\BatchExecution\Automation_Config.xml");
                SocketClient.Close();

                //Update Result
                if (response.Equals("33Ended"))
                { result.steps[++executedSteps].status = "Pass"; }
                else
                { result.steps[++executedSteps].status = "Fail"; }

                //update result
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Trigger Remote POP Installation Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Trigger Remote POP Installation\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

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
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Trigger Remote POP Installation Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Trigger Remote POP Installation\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;
            }
        }

        /// <summary>
        /// This Test method is to trigger Exam Importer installation in client machine belongs to step 12
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_TriggerExamInstaller1(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = @"D:\Selenium_executable\Release\psexec.exe",
                        Arguments =
                            @"\\10.4.13.65 -u Administrator -p Cedara99 -accepteula -i 1 D:\Selenium_executable\Release\Selenium.exe -file D:\Selenium_executable\Release\POP.xls tab POP -capture d -browser chrome -POPServerAddress 10.4.13.66",
                        WorkingDirectory = "",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                proc.Start();
                int i = 0;
                while (!proc.HasExited && i < 30)
                {
                    Thread.Sleep(60000);
                    i++;
                }
                Logger.Instance.InfoLog("Standard output message from  PSEXEC.exe : " + proc.StandardOutput.ReadToEnd());
                Logger.Instance.InfoLog("Standard error messsage from  PSEXEC.exe : " + proc.StandardError.ReadToEnd());
                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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
        /// This Test method is to trigger Exam Importer installation in client machine belongs to step 12
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_TriggerExamInstaller2(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = @"D:\Selenium_executable\Release\psexec.exe",
                        Arguments =
                            @"\\10.4.13.65 -u Administrator -p Cedara99 -accepteula -i 1 D:\Selenium_executable\Release\Selenium.exe -file D:\Selenium_executable\Release\POP.xls tab POP -capture d -browser chrome -POPServerAddress 10.4.13.66",
                        WorkingDirectory = "",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                proc.Start();
                int i = 0;
                while (!proc.HasExited && i < 30)
                {
                    Thread.Sleep(60000);
                    i++;
                }
                Logger.Instance.InfoLog("Standard output message from  PSEXEC.exe : " + proc.StandardOutput.ReadToEnd());
                Logger.Instance.InfoLog("Standard error messsage from  PSEXEC.exe : " + proc.StandardError.ReadToEnd());
                //Update Result
                result.steps[++executedSteps].status = "Pass";
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
        /// This Test method is to perform step 14
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_UpdateDeviceID1(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Update POP Device ID        
                pop.ReadPacsGatewayConfigForAETitles();

                //need to update DEVICE ID in Pacs                        
                mpaclogin.DriverGoTo(login.mpacstudyurl);
                mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                mpacconfig.NavigateToDicomDevices();
                mpacconfig.AddDicomDevice(Config.Popclient1, pop.AETitlesExposed[0]);

                //Update Result
                result.steps[++executedSteps].status = "Pass";
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
        /// This Test method is to perform step 14
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_UpdateDeviceID2(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                pop.ReadPacsGatewayConfigForAETitles();

                //need to update DEVICE ID in Pacs                
                mpaclogin.InvokeBrowser(Config.BrowserType);
                mpaclogin.DriverGoTo(login.mpacstudyurl);
                mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                mpacconfig.NavigateToDicomDevices();
                mpacconfig.AddDicomDevice(Config.Popclient2, pop.AETitlesExposed[1]);

                //Update Result
                result.steps[++executedSteps].status = "Pass";
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
        /// This Test method is to perform step 13
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_InstallPop1(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                basepage.GetInstitutionPin(Config.Inst1);
                //preparing pop setup
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);

                if (pop.IsPACSGatewayInstalled())
                {
                    pop.UnInstallPACSGateway();
                }

                new Login().Logout();
                basepage.DriverGoTo(basepage.url);
                basepage.DownloadInstaller(basepage.url, "POP", basepage.PACSGatewayInstallerPath, "SuperAdminGroup");
                new POPUploader().InstallPACSGateway(Pin: basepage.pin, InstallerLocation: basepage.PACSGatewayInstallerPath);
                bool status;
                status = VerifyServiceStatus(1);
                //Update Result
                result.steps[++executedSteps].status = "Pass";
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
        /// This Test method is to perform step 13
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_InstallPop2(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                basepage.GetInstitutionPin(Config.Inst2);
                //preparing pop setup
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);

                if (pop.IsPACSGatewayInstalled())
                {
                    pop.UnInstallPACSGateway();
                }

                basepage.InvokeBrowser(Config.BrowserType);
                basepage.DriverGoTo(basepage.url);
                basepage.DownloadInstaller(basepage.url, "POP", basepage.PACSGatewayInstallerPath, "SuperAdminGroup");
                basepage.CloseBrowser();
                wpfobject.InvokeApplication(basepage.PACSGatewayInstallerPath);
                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.FocusWindow();
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.ClickButton("Next", 1); //Next button

                int ctr = 0;
                if (wpfobject.GetCheckBox(0) == null && ctr < 10)
                {
                    wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                    wpfobject.ClickButton("Next", 1);
                    ctr++;
                }

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.SelectCheckBox(0);
                wpfobject.WaitTillLoad();
                wpfobject.SelectCheckBox(0);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton("Next", 1); //Next button
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.WaitTillLoad();
                wpfobject.SetText("PIN:", basepage.pin, 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindowFromDesktop(basepage.PacsGatewayInstance);
                wpfobject.FocusWindow();
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowFromDesktop(basepage.PacsGatewayInstance);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowFromDesktop(basepage.PacsGatewayInstance);
                wpfobject.SetText("emailTextBox", "valarmathi.murugesan@aspiresys.com");
                // 1 signifies that the indentifier is Automation ID
                for (int i = 0; i < 3; i++)
                {
                    wpfobject.SelectTableCheckBox(i, 2);
                }
                wpfobject.WaitTillLoad();


                wpfobject.GetMainWindowFromDesktop(basepage.PacsGatewayInstance);

                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.ClickButton("Next");
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.ClickButton("Install", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.WaitTillLoad();


                wpfobject.GetMainWindow(basepage.PacsGatewayInstance + " Setup");
                wpfobject.WaitForButtonExist(basepage.PacsGatewayInstance + " Setup", "Finish", 1);
                wpfobject.WaitForButtonExist(basepage.PacsGatewayInstance + " Setup", "Finish", 1);
                wpfobject.ClickButton("Finish", 1);
                bool status;
                status = VerifyServiceStatus(1);
                //Update Result
                result.steps[++executedSteps].status = "Pass";
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
        /// This Test method is to perform step 12
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_InstallExamImporter1(String testid, String teststeps, int stepcount)
        {
            basepage._examImporterInstance = Config.eiwindow;
            ei._examImporterInstance = basepage._examImporterInstance;
            //basepage.InvokeBrowser(Config.BrowserType);
            //basepage.CloseBrowser();

            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                //Delete existing installer file
                new Login();
                basepage.DriverGoTo(basepage.url);
                try
                {
                    File.Delete(basepage.InstallerPath + @"\Installer.UploaderTool.msi");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Problems in deleting the previous installer file due to : " + e);
                }

                //Download new installer file and Minimize all apps
                basepage.DownloadInstaller(basepage.url, "CDUpload", basepage.InstallerPath + @"\Installer.UploaderTool.msi", "SuperAdminGroup");
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);

                //Uninstall App if already installed
                if (ei.IsEiInstalled())
                {
                    ei.UnInstallEI();
                }

                //Proceed with installation
                ei.LaunchEiInstaller(basepage.InstallerPath);
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(basepage._examImporterInstance + " Setup", "Cancel", 1);
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                wpfobject.ClickRadioButton(0);

                wpfobject.WaitTillLoad();

                wpfobject.ClickButton("Next", 1);

                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(Config.ph2UserName, Config.ph2Password);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(basepage._examImporterInstance + " Setup", "Finish", 1);

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                ei.EI_SelectAutoLaunchOption(false);

                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                ei.EI_FinishInstallation();

                wpfobject.WaitTillLoad();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                return result;

            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                //BasePage.KillProcess("Windows Installer");
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test method is to perform step 12
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_InstallExamImporter2(String testid, String teststeps, int stepcount)
        {
            basepage._examImporterInstance = Config.eiwindow2;
            ei._examImporterInstance = basepage._examImporterInstance;
            basepage.InvokeBrowser(Config.BrowserType);
            basepage.CloseBrowser();

            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {


                //Taskbar taskbar = new Taskbar();
                //taskbar.Hide();
                new Login();
                basepage.DriverGoTo(basepage.url);

                try
                {
                    File.Delete(basepage.InstallerPath + @"\Installer.UploaderTool.msi");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Problems in deleting the previous installer file due to : " + e);
                }

                basepage.DownloadInstaller(basepage.url, "CDUpload", basepage.InstallerPath + @"\Installer.UploaderTool.msi", "SuperAdminGroup");

                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);


                if (ei.IsEiInstalled())
                {
                    ei.UnInstallEI();
                }

                ei.LaunchEiInstaller(basepage.InstallerPath);
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(basepage._examImporterInstance + " Setup", "Cancel", 1);
                ei.EI_AcceptEulaInstaller();

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                wpfobject.ClickRadioButton(0);

                wpfobject.WaitTillLoad();

                wpfobject.ClickButton("Next", 1);

                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(Config.ph2UserName, Config.ph2Password);

                ei.EI_SubmitRegistrationDetails();

                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(basepage._examImporterInstance + " Setup", "Finish", 1);

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                ei.EI_SelectAutoLaunchOption(false);

                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(basepage._examImporterInstance + " Setup");

                ei.EI_FinishInstallation();

                wpfobject.WaitTillLoad();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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
        /// This is to setup Dates to All Dates in System Settings Tab        
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_SelectALLDAtes(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Set the Date
                new Login();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();


                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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
        /// This test will update the build id in the Automation_Config file 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_UpdateBuildID(String testid, String teststeps, int stepcount)
        {

            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Get the Build ID from Build.info 
                string line;
                //string buildid = "";
                string buildno = "";
                //using (StreamReader sr = new StreamReader("C:\\WebAccess\\Build.Info"))
                //{                    
                //    while((line = sr.ReadLine())!= null)
                //    {
                //        if (line.Contains("Build ID")) { buildid = line; break; }
                //    }
                //    buildid = buildid.Split(':')[1].Trim();                   
                //}

                using (StreamReader sr = new StreamReader("C:\\WebAccess\\Build.Info"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("Build Number")) { buildno = line; break; }
                    }
                    buildno = buildno.Split(':')[1].Trim();
                }

                //Update config file with build ID
                Dictionary<String, String> buildnumber = new Dictionary<String, String>();
                buildnumber.Add("buildnumber", buildno);
                Dictionary<String, String> RDMBuildNo = new Dictionary<String, String>();
                RDMBuildNo.Add("buildnumber", buildno);
                ReadXML.UpdateXML(Config.inputparameterpath, buildnumber);
                ReadXML.UpdateXML(Config.inputparameterpath, RDMBuildNo);
                Config.buildnumber = buildno;
                Config.rdm = buildno;

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Update Build ID in Config file Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Update Build ID in Config file\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.steps[++executedSteps].status = "Fail";
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Update Build ID in Config file Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Update Build ID in Config file\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test will update device id for POP tool in the Automation_Config.file
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_UpdateDeviceID(String testid, String teststeps, int stepcount)
        {

            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;
            String popaetitel1 = "";
            String popaetitel2 = "";

            try
            {
                //Get PACS Device ID 1 From iConnect
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.UploadDevice devices = (Image_Sharing.UploadDevice)imagesharing.NavigateToSubTab("Upload Device");
                popaetitel1 = devices.SearchDevice(Institutioname: Config.Inst1, devicetype: "PACS Gateway", domain: "SuperAdminGroup");
                Dictionary<String, String> aetitles = new Dictionary<string, string>();
                aetitles.Add("pacsgateway1", popaetitel1);
                Config.pacsgatway1 = popaetitel1;
                ReadXML.UpdateXML(Config.inputparameterpath, aetitles);

                //Update DEVICE ID-1 in Pacs                        
                mpaclogin.DriverGoTo(login.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                MpacConfiguration mpacconfig = (MpacConfiguration)mpachome.NavigateTopMenu("Configuration");
                mpacconfig.NavigateToDicomDevices();
                mpacconfig.AddDicomDevice(Config.IConnectIP, popaetitel1);
                mpaclogin.LogoutPacs();

                try
                {
                    //Get PACS Device ID 1 From iConnect
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                    devices = (Image_Sharing.UploadDevice)imagesharing.NavigateToSubTab("Upload Device");
                    popaetitel2 = devices.SearchDevice(Institutioname: Config.Inst2, devicetype: "PACS Gateway", domain: "SuperAdminGroup");
                    aetitles = new Dictionary<string, string>();

                    //Update xml file
                    aetitles.Add("pacsgateway2", popaetitel2);
                    ReadXML.UpdateXML(Config.inputparameterpath, aetitles);
                    Config.pacsgatway2 = popaetitel2;

                    //Update DEVICE ID-2 in Pacs                        
                    mpaclogin.DriverGoTo(login.mpacstudyurl);
                    mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                    mpacconfig = (MpacConfiguration)mpachome.NavigateTopMenu("Configuration");
                    mpacconfig.NavigateToDicomDevices();
                    mpacconfig.AddDicomDevice(Config.Popclient1, popaetitel2);
                    mpaclogin.LogoutPacs();
                }
                catch (Exception e) { Logger.Instance.ErrorLog("Error occurred while adding POP device ID 2"); }

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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
        /// This Test 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_UpdateExamImporter(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Launch and Login Exam Importer-1
                ei.LaunchEI(Config.EIFilePath);
                ei.LoginToEi(Config.ph1UserName, Config.ph1Password);

                //Update Institution, Save and close
                Thread.Sleep(5000);
                wpfobject.ClickRadioButton("RdBtnExistingInstitution");
                wpfobject.SelectFromComboBox("CmbInstitution", Config.Inst1, 0, 1);
                wpfobject.ClickButton("BtnSave");
                Thread.Sleep(7000);
                ei.EI_Logout();
                ei.CloseUploaderTool();

                //Launch and Login Exam Importer-2   
                if (File.Exists(Config.EIFilePath2))
                {
                    ei.LaunchEI(Config.EIFilePath2);
                    ei.LoginToEi(Config.ph1UserName, Config.ph1Password, 2);

                    //Update Institution, Save and close
                    Thread.Sleep(5000);
                    wpfobject.ClickRadioButton("RdBtnExistingInstitution");
                    wpfobject.SelectFromComboBox("CmbInstitution", Config.Inst2, 0, 1);
                    wpfobject.ClickButton("BtnSave");
                    Thread.Sleep(7000);
                    ei.EI_Logout();
                    ei.CloseUploaderTool(2);
                }

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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
        /// 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_NonImageSharingSetup(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Current Directory path
                String currentDirectory = System.IO.Directory.GetCurrentDirectory();

                //Install iCA - run iCAInstall.pl file
                String InstallationScriptPath = currentDirectory + Path.DirectorySeparatorChar + "iCAInstall.pl";
                //Start process
                var proc = new Process
                {
                    StartInfo =
                {
                    FileName = InstallationScriptPath,
                    Arguments = "",
                    WorkingDirectory = currentDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
                };
                //proc.Start();

                //wait
                //Thread.Sleep(60000);

                /*****Modify/Update Config Files******/
                //Config Files - File Path
                String DataSourceManagerConfigPath = @"C:\WebAccess\WebAccess\Config\DataSource\DataSourceManagerConfiguration.xml";
                String ResourceConfigurationPath = @"C:\WebAccess\WebAccess\Config\ResourceConfiguration.xml";
                String XDSConfigPath = @"C:\WebAccess\WebAccess\Config\Xds\XdsConfiguration.xml";

                //Config file Directory Path
                String ConfigFileDirectory = currentDirectory + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);

                //Config Files - File Path
                String DataSourceManagerConfigPath_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + "DataSourceManagerConfiguration" + Path.DirectorySeparatorChar + "DataSourceManagerConfiguration.xml";
                String ResourceConfigurationPath_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + "ResourceConfiguration" + Path.DirectorySeparatorChar + "ResourceConfiguration.xml";
                String XDSConfigPath_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + "XdsConfiguration.xml";

                //Data Source Manager Configuration xml File
                File.Copy(DataSourceManagerConfigPath_Backup, DataSourceManagerConfigPath, true);

                //XDS Configuration xml file
                File.Copy(XDSConfigPath_Backup, XDSConfigPath, true);

                //ResourceConfiguration.xml file
                if (File.Exists(ResourceConfigurationPath_Backup))
                {
                    File.Copy(ResourceConfigurationPath_Backup, ResourceConfigurationPath, true);
                }

                //Run IISReset.exe file
                servicetool.RestartIISUsingexe();
                Thread.Sleep(10000);

                //UnComment LDAP directories from LDAP Config file
                //tool.EnableLDAPConfigfile();

                //Enable Different Features
                servicetool.InvokeServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.EnablePatient();
                tool.EnableStudySharing();
                tool.EnableDataDownloader();
                tool.EnableDataTransfer();
                tool.EnableEmailStudy();
                tool.EnablePDFReport();
                tool.EnableRequisitionReport();
                tool.EnableSelfEnrollment();
                tool.EnableEmergencyAccess();
                tool.EnableBriefcase();
                tool.EnableConferenceLists();
                wpfobject.WaitTillLoad();
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                //Setup Email Notification
                tool.SetEmailNotificationForPOP();

                //Setup Transfer Service Config
                tool.SetTransferserviceAETitle(TransferserviceAETitle);

                //Enable encapsulated report
                tool.EnableReports(false);

                //Enable Merge EMPI
                tool.EnableMergeEMPI();

                //Enable Study attachments and HTML 5 viewer
                //tool.EnableStudyAttachements(); //Study attachment feature is being obsoleted and will not be available in the new viewer.
                //tool.EnableHTML5();
                servicetool.RestartService();

                //Choosing BluRing as default viewer from Servicetool
                servicetool.NavigateToTab("Viewer");
                servicetool.NavigateSubTab("Miscellaneous");
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.SetBluringViewer();
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                servicetool.RestartService();

                //Enable LDAP setup
                /*tool.NavigateToConfigToolUserMgmtDatabaseTab();
                tool.SetMode(2);
                tool.LDAPSetup();
                wpfobject.WaitTillLoad();*/
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                //domainmanagement.SetCheckBoxInEditDomain("attachment", 0); //Study attachment feature is being obsoleted and will not be available in the new viewer.
                //domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("emergency", 0);
                domainmanagement.SetCheckBoxInEditDomain("breifcase", 0);
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();

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

                //Create physician and archivist users
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Config.phUserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.phPassword);
                usermanagement.CreateUser(Config.ph1UserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.ph1Password);
                usermanagement.CreateUser(Config.arUserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.arPassword);
                usermanagement.CreateUser(Config.ar1UserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.ar1Password);

                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();

                //Enable Bypass mode
                login.UncommentXMLnode("id", "Bypass");

                //Logout
                login.Logout();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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
        /// This Test method is to perform:
        /// 1. Data Cleanup in External Systems
        /// 2. Setup ImageSharing as Y in Web.config file
        /// 3. Adds the Data sources: Holding pen, EA and PACS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_RDMChild1DataSourceSetup(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Set Enable ImageSharing value as true
                basepage.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");

                // Call function MinimizeAll
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //########## Adding Datasources ##############

                //Add EA holding pen as Datasource
                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                tool.AddEADatasource(Config.HoldingPenIP, Config.HoldingPenAETitle, "1", IsHoldingPen: 1);

                //Add Destination 1 - Pacs
                tool.AddPacsDatasource(Config.DestinationPACS, Config.DestinationPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);

                //Add Sanity PACS
                tool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "3", Config.pacsadmin, Config.pacspassword);

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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
        /// This Test method is to perform:
        /// 1. Data Cleanup in External Systems
        /// 2. Setup ImageSharing as Y in Web.config file
        /// 3. Adds the Data sources: Holding pen, EA and PACS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_RDMMainDataSourceSetup(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Set Enable ImageSharing value as true
                basepage.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");

                // Call function MinimizeAll
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //########## Adding Datasources ##############

                //Add EA holding pen as Datasource
                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                tool.AddEADatasource(Config.HoldingPenIP, Config.HoldingPenAETitle, "1", IsHoldingPen: 1);

                //Add Destination 1 - Pacs
                tool.AddPacsDatasource(Config.DestinationPACS, Config.DestinationPACSAETitle, "2", Config.pacsadmin, Config.pacspassword);

                //Add Sanity PACS
                tool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "3", Config.pacsadmin, Config.pacspassword);

                //Add 131 EA Data source
                tool.AddEADatasource(Config.EA1, Config.EA1AETitle, "4", IsHoldingPen: 0);

                //Add EA-46 Datasource
                tool.AddEADatasource("10.4.38.46", "ECM_ARC_46", "5", dataSourceName: "EA-46");

                //Add Patient ID domains and Other Identifiers
                tool.SelectDataSource("EA-46");
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                tool.SetDataSourcePatientIDDomain("NYH", "NYH", "NYH&&");
                tool.SetOtherIdentifiers("REF_AE_1");
                tool.SetOtherIdentifiers("REF_AE_2");
                tool.SetOtherIdentifiers("REF_AE_3");
                tool.SetOtherIdentifiers("REF_AE_46");
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();

                //Add EA-116 Datasource
                tool.AddEADatasource("10.4.38.116", "ECM_ARC_116", "6", dataSourceName: "EA-116");

                //Add other identifiers
                tool.SelectDataSource("EA-116");
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                tool.SetDataSourcePatientIDDomain("TOH", "TOH", "TOH&&");
                tool.SetOtherIdentifiers("REF_AE_1");
                tool.SetOtherIdentifiers("REF_AE_2");
                tool.SetOtherIdentifiers("REF_AE_3");
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();

                //Add XDS Datasource
                tool.AddXDSDatasource("XDS_DS", "ECM_40_XDS", "");
                tool.SelectDataSource("XDS_DS");
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                tool.SetDataSourcePatientIDDomain("XDS", "XDS Affinity Domain", "&2.16.840.1.113883.9.185&ISO");
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();

                //Add RDM Child-1 data source
                tool.AddRDMDatasource("10.9.37.102", "7");

                //Add RDM Child-1 data source
                tool.AddRDMDatasource("10.9.37.104", "8");

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
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

        private bool VerifyServiceStatus(int IsStart = 0)
        {
            bool status;
            status = wpfobject.ServiceStatus(basepage.PacsGatewayInstance + " Service", IsStart != 0 ? "Running" : "Stopped");
            return status;
        }

        /* Updated reusables to increase usability factor and efficiency */

        /// <summary>
        /// This Test method is to perform:
        /// 1. Adding the required Data sources in the respective execution servers: Holding pen, EA, PACS, XDS and RDM
        /// 2. Setup ImageSharing as Y in Web.config file, if image sharing setup is needed in the respective server
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_AddDataSources(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Get all additional server details from Config file
                Dictionary<string, string> AddittionalServers_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress");
                Dictionary<string, string> AddittionalServers_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle");
                Dictionary<string, string> AddittionalServers_DataSourceNames = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/DataSourceNames");

                //Get PACS data source details from Config file
                Dictionary<string, string> PACS_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/PACSDataSources");
                Dictionary<string, string> PACS_DataSources_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle/PACSDataSources");

                //Get EA data source details from Config file
                Dictionary<string, string> EA_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/EADataSources");
                Dictionary<string, string> EA_DataSources_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle/EADataSources");

                //Get XDS related EA data source details from Config file
                Dictionary<string, string> XDSEA_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/XDSEADataSources");
                Dictionary<string, string> XDSEA_DataSources_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle/XDSEADataSources");

                //Get RDM data source details from Config file
                Dictionary<string, string> RDM_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/RDMDataSources");

                //Get XDS-EA Datasource Names
                Dictionary<string, string> XDSEA_DataSource_Names = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/DataSourceNames/XDSEADataSources");

                // Call function MinimizeAll
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //Launch Service tool
                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

                //Adding Holding Pen as data source                
                if (!String.IsNullOrEmpty(AddittionalServers_IP["HoldingPen"]))
                {
                    //Set Enable ImageSharing value as true
                    basepage.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");

                    DistanceCounter++;
                    tool.AddEADatasource(AddittionalServers_IP["HoldingPen"], AddittionalServers_AETitle["HoldingPen"], DistanceCounter.ToString(), IsHoldingPen: 1);
                }

                //Adding EA data sources
                foreach (String EAIP in EA_DataSources_IP.Keys)
                {
                    var isDeidentificationEnable = false;
                    if (!String.IsNullOrEmpty(EA_DataSources_IP[EAIP]))
                    {
                        DistanceCounter++;
                        if (EA_DataSources_IP[EAIP] == Config.EA7)
                        {
                            isDeidentificationEnable = true;
                        }
                        tool.AddEADatasource(EA_DataSources_IP[EAIP], EA_DataSources_AETitle[EAIP], DistanceCounter.ToString(), EnableDeidentification: isDeidentificationEnable);
                    }
                }

                //Adding PACS data sources
                foreach (String PACSIP in PACS_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(PACS_DataSources_IP[PACSIP]))
                    {
                        DistanceCounter++;
                        tool.AddPacsDatasource(PACS_DataSources_IP[PACSIP], PACS_DataSources_AETitle[PACSIP], DistanceCounter.ToString(), Config.pacsadmin, Config.pacspassword);
                    }
                }

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Add Data Sources Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Add Data Sources\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

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

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Add Data Sources Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Add Data Sources\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test method is to perform:
        /// 1. Adding the required Data sources in the respective execution servers: XDS and XDS related EA data sources
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_AddXDSDataSources(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Get all additional server details from Config file
                Dictionary<string, string> AddittionalServers_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle");
                Dictionary<string, string> AddittionalServers_DataSourceNames = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/DataSourceNames");

                //Get XDS related EA data source details from Config file
                Dictionary<string, string> XDSEA_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/XDSEADataSources");
                Dictionary<string, string> XDSEA_DataSources_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle/XDSEADataSources");

                //Get XDS-EA Datasource Names
                Dictionary<string, string> XDSEA_DataSource_Names = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/DataSourceNames/XDSEADataSources");

                // Call function MinimizeAll
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //Launch Service tool
                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

                //Adding XDS data source                
                if (!String.IsNullOrEmpty(AddittionalServers_DataSourceNames["XDSDataSource"]))
                {
                    tool.AddXDSDatasource(AddittionalServers_DataSourceNames["XDSDataSource"], AddittionalServers_AETitle["XDSDataSource"], "");
                }

                //Adding XDS related EA data sources
                foreach (String XDSEAIP in XDSEA_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(XDSEA_DataSources_IP[XDSEAIP]))
                    {
                        DistanceCounter++;
                        tool.AddEADatasource(XDSEA_DataSources_IP[XDSEAIP], XDSEA_DataSources_AETitle[XDSEAIP], DistanceCounter.ToString(), dataSourceName: XDSEA_DataSource_Names[XDSEAIP]);
                    }
                }

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Add XDS Data Sources Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Add XDS Data Sources\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                ++executedSteps;
                result.steps[executedSteps].status = "Fail";
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Add XDS Data Sources Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Add XDS Data Sources\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test method is to perform:
        /// 1. Adding the required RDM Data sources in the respective execution servers
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_AddRDMDataSources(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Get RDM data source details from Config file
                Dictionary<string, string> RDM_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/RDMDataSources");

                // Call function MinimizeAll
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //Launch Service tool
                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

                //Adding RDM data sources
                foreach (String RDMIP in RDM_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(RDM_DataSources_IP[RDMIP]))
                    {
                        DistanceCounter++;
                        tool.AddRDMDatasource(RDM_DataSources_IP[RDMIP], DistanceCounter.ToString());
                    }
                }

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Add RDM Data Sources Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Add RDM Data Sources\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                ++executedSteps;
                result.steps[executedSteps].status = "Fail";
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Add RDM Data Sources Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Add RDM Data Sources\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This function is to enable all the needed features in service tool and in domain, role & user levels.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_EnableGeneralFeatures(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                bool blnUpdateDomainCheck = false;
                const string domainName = "SuperAdminGroup";

                //Update Super Admin Group in Domain Management (Connect all DataSource and set Institution name)            
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                blnUpdateDomainCheck = basepage.UpdateGivenDomain(domainName);
                login.Logout();

                //UnComment LDAP directories from LDAP Config file
                tool.EnableLDAPConfigfile();

                //Enable Different Features
                servicetool.InvokeServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.EnablePatient();
                tool.EnableStudySharing();
                tool.EnableDataDownloader();
                tool.EnableDataTransfer();
                tool.EnableEmailStudy();
                tool.EnablePDFReport();
                tool.EnableRequisitionReport();
                tool.EnableSelfEnrollment();
                tool.EnableEmergencyAccess();
                tool.EnableBriefcase();
                tool.EnableConferenceLists();
                wpfobject.WaitTillLoad();
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                //Setup Email Notification
                tool.SetEmailNotificationForPOP();

                //Setup Transfer Service Config
                tool.SetTransferserviceAETitle(TransferserviceAETitle);

                //Enable encapsulated report
                //tool.EnableReports(true);

                //Enable Study attachments 
                servicetool.RestartService();

                //Prefetch settings
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 5, timerange: 60, cleanupthreshold: 60, AEtitle: basepage.PrefetchAETitle);
                servicetool.RestartService();

                //Enable Prefetch cache - Datasource
                if (!String.IsNullOrEmpty(Config.DestEAsIp))
                {
                    servicetool.EnableCacheForDataSource(login.GetHostName(Config.DestEAsIp));
                }
                servicetool.RestartService();

                //Enable LDAP setup
                tool.NavigateToConfigToolUserMgmtDatabaseTab();
                tool.SetMode(2);
                tool.LDAPSetup();
                servicetool.CloseServiceTool();

                //Enable Bluring Viewer
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                    TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing");
                tool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                tool.CloseServiceTool();

                //Update default viewer in Webaccess UI
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("SuperAdminGroup_Inst");
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.SetCheckBoxInEditDomain("datatransfer", 0);
                domainmanagement.SetCheckBoxInEditDomain("datadownload", 0);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("emergency", 0);
                domainmanagement.SetCheckBoxInEditDomain("breifcase", 0);
                domainmanagement.SetCheckBoxInEditDomain("universalviewer", 0);
                if (Environment.MachineName.ToLower().Equals("exe-ica3-ws12")) { domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0); }
                //domainmanagement.ConnectAllDataSources();
                domainmanagement.ModifyStudySearchFields();
                String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
                domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
                domainmanagement.ClickSaveEditDomain();

                //Create Different Role and uers
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("universalviewer", 0);
                rolemanagement.ClickSaveEditRole();
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

                //Change System Settings to All Dates
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();

                //Enable Bypass mode for Integrator mode
                login.UncommentXMLnode("id", "Bypass");

                //Create physician and archivist users
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Config.stUserName, "SuperAdminGroup", "Staff", 1, Config.st1Email, 1, Config.stPassword);
                usermanagement.CreateUser(Config.phUserName, "SuperAdminGroup", "Physician", 1, Config.CustomUser1Email, 1, Config.phPassword);
                usermanagement.CreateUser(Config.ph1UserName, "SuperAdminGroup", "Physician", 1, Config.ph1Email, 1, Config.ph1Password);
                usermanagement.CreateUser(Config.arUserName, "SuperAdminGroup", "Archivist", 1, Config.CustomUser2Email, 1, Config.arPassword);
                usermanagement.CreateUser(Config.ar1UserName, "SuperAdminGroup", "Archivist", 1, Config.ar1Email, 1, Config.ar1Password);

                //Logout
                login.Logout();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Enable General Features Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Enable General Features\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.steps[++executedSteps].status = "Fail";
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Enable General Features Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Enable General Features\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;

            }
        }

        /// <summary>
        /// This function performs
        /// 1. Modifying XDS Configuration file / adding affinity domains in XDS tab
        /// 2. Adding Patient ID domains and Other identifiers to XDS data sources
        /// 3. Other XDS Config related settings
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_ConfigureXDS(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Get all additional server details from Config file
                Dictionary<string, string> AddittionalServers_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle");
                Dictionary<string, string> AddittionalServers_DataSourceNames = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/DataSourceNames");

                //Get XDS related EA data source details from Config file
                Dictionary<string, string> XDSEA_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/XDSEADataSources");
                Dictionary<string, string> XDSEA_DataSources_AETitle = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle/XDSEADataSources");

                //Get XDS-EA Datasource Names
                Dictionary<string, string> XDSEA_DataSource_Names = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/DataSourceNames/XDSEADataSources");

                // Call function MinimizeAll
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //Launch Service tool
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

                //Adding XDS data source                
                if (!String.IsNullOrEmpty(AddittionalServers_DataSourceNames["XDSDataSource"]))
                {
                    servicetool.AddXDSDatasource(AddittionalServers_DataSourceNames["XDSDataSource"], AddittionalServers_AETitle["XDSDataSource"], this.DistanceCounter++.ToString());
                }

                //Adding XDS related EA data sources
                foreach (String XDSEAIP in XDSEA_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(XDSEA_DataSources_IP[XDSEAIP]))
                    {
                        this.DistanceCounter++;
                        servicetool.AddEADatasource(XDSEA_DataSources_IP[XDSEAIP], XDSEA_DataSources_AETitle[XDSEAIP], "", dataSourceName: XDSEA_DataSource_Names[XDSEAIP]);
                    }
                }

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                Taskbar taskbar1 = new Taskbar();
                taskbar1.Show();

                taskbar = new Taskbar();
                taskbar.Hide();
                string XDSType = Config.SetXDS.ToLower();
                //Enable local related studies in study search tab
                servicetool.LaunchServiceTool();
                servicetool.NavigateToStudySearch();
                servicetool.modifyBtn().Click();
                if (!servicetool.EnableIncludelocalrelatedstudies().IsSelected)
                {
                    servicetool.EnableIncludelocalrelatedstudies().Click();
                    servicetool.ApplyBtn().Click();
                    wpfobject.WaitTillLoad();
                }
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();

                //Add Patient ID domains and Other Identifiers
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.DataSource_Tab);
                if (string.Equals(XDSType, "pix"))
                {
                    servicetool.SelectDataSource(Config.XDS_EA2);
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.SetDataSourcePatientIDDomain("NYH", "New York Hospital", "NYH&2.16.840.1.113883.9.187&ISO", DicomIPID: "IPID-NYH", TypeCode: "PI");
                    servicetool.SetOtherIdentifiers("REF_AE_1");
                    servicetool.SetOtherIdentifiers("REF_AE_2");
                    servicetool.SetOtherIdentifiers("REF_AE_3");
                    servicetool.SetOtherIdentifiers("REF_AE_46");
                    wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                    wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                    wpfobject.WaitTillLoad();

                    servicetool.SelectDataSource(Config.XDS_EA1);
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.SetDataSourcePatientIDDomain("TOH", "Toronto Hospital", "TOH&2.16.840.1.113883.9.188&ISO", DicomIPID: "IPID-TOH", TypeCode: "PI");
                    servicetool.SetOtherIdentifiers("REF_AE_1");
                    servicetool.SetOtherIdentifiers("REF_AE_2");
                    servicetool.SetOtherIdentifiers("REF_AE_3");
                    wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                    wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                    wpfobject.WaitTillLoad();

                    servicetool.SelectDataSource(login.GetHostName(Config.EA1));
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.SetDataSourcePatientIDDomain("CLE", "Cleveland Clinic", "CLE&2.16.840.1.113883.9.186&ISO", DicomIPID: "IPID-CLE", TypeCode: "PI");
                    wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                    wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                    wpfobject.WaitTillLoad();

                    //Add XDS Datasource
                    servicetool.SelectDataSource(Config.XDS);
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.SetDataSourcePatientIDDomain("XDS", "XDS Affinity Domain", "&2.16.840.1.113883.9.185&ISO", TypeCode: "PI");
                    wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                    wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                    wpfobject.WaitTillLoad();
                }
                else if (string.Equals(XDSType, "sad"))
                {
                    //Add Patient ID domains and Add other identifiers
                    servicetool.SelectDataSource(Config.XDS_EA2);
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.SetDataSourcePatientIDDomain("NYH", "NYH", "NYH&&");
                    servicetool.SetOtherIdentifiers("REF_AE_1");
                    servicetool.SetOtherIdentifiers("REF_AE_2");
                    servicetool.SetOtherIdentifiers("REF_AE_3");
                    servicetool.SetOtherIdentifiers("REF_AE_46");
                    wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                    wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                    wpfobject.WaitTillLoad();

                    //Add Patient ID domains and Add other identifiers
                    servicetool.SelectDataSource(Config.XDS_EA1);
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.SetDataSourcePatientIDDomain("TOH", "TOH", "TOH&&");
                    servicetool.SetOtherIdentifiers("REF_AE_1");
                    servicetool.SetOtherIdentifiers("REF_AE_2");
                    servicetool.SetOtherIdentifiers("REF_AE_3");
                    wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                    wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                    wpfobject.WaitTillLoad();

                    //Add XDS Datasource
                    servicetool.SelectDataSource(Config.XDS);
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.SetDataSourcePatientIDDomain("XDS", "XDS Affinity Domain", "&2.16.840.1.113883.9.185&ISO");
                    wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                    wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                    wpfobject.WaitTillLoad();
                }

                //Restart the service
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();

                if (string.Equals(XDSType, "pix"))
                {
                    //Update XDS Tab to PDQ-PIX Config
                    servicetool.LaunchServiceTool();
                    servicetool.XDSTabConfig(Address_URL: "http://10.5.33.73:8081/index/services/registry", ID1: "1.3.6.1.4.1.21367.0.2.21", Address1: "http://10.5.37.21:12310/iti43", ID2: "1.3.6.1.4.1.21367.13.40.157", Address2: "http://10.5.33.142:12310/iti43");
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    wpfobject.WaitTillLoad();
                    servicetool.CloseConfigTool();
                    servicetool.LaunchServiceTool();

                    //Add PDQ-PIX Config
                    servicetool.AddPDQPIXConfig(PDQHost: "10.5.37.21", PDQEAPortval: "12999", strTyPeCode: "PI");
                    servicetool.CloseConfigTool();
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/PatientDemographicsConsumer/Configuration/TRACE_SEND", "no");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/PatientDemographicsConsumer/Configuration/TRACE_RECEIVE", "no");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PIXConfiguration.xml", "/Configuration/TRACE_SEND", "no");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PIXConfiguration.xml", "/Configuration/TRACE_RECEIVE", "no");
                    login.ChangeAttributeValue("C:\\WebAccess\\WebAccess\\Web.config", "/add[@key='Application.QueryPatientIDInCFIND']", "value", "true");
                    servicetool.RestartIISUsingexe();
                }

                else if (string.Equals(XDSType, "sad"))
                {
                    //Update XDS Tab to PDQ-SAD Config
                    servicetool.LaunchServiceTool();
                    servicetool.XDSTabConfig(Address_URL: "http://10.5.33.73:8081/index/services/registry", ID2: "1.3.6.1.4.1.21367.13.40.157", Address2: "http://10.5.33.142:12310/iti43", PDQPIX: false);
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    wpfobject.WaitTillLoad();
                    servicetool.CloseConfigTool();

                    //Add PDQ-SAD Config
                    servicetool.LaunchServiceTool();
                    servicetool.AddPDQ(PDQHost: "10.5.33.73", PDQEAPortval: "2576");
                    servicetool.ModifyEnableFeatures();
                    servicetool.EditSingleAffinity(true);
                    servicetool.SetMasterPID();
                    servicetool.CloseConfigTool();
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/PatientDemographicsConsumer/Configuration/TRACE_SEND", "no");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/PatientDemographicsConsumer/Configuration/TRACE_RECEIVE", "no");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/Segment[@Name='MSH']/Field[@Name='Sending Application']", "HL7TOOL");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/Segment[@Name='MSH']/Field[@Name='Sending Facility']", "LOCAL");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/Segment[@Name='MSH']/Field[@Name='Receiving Application']", "FORINDEX");
                    servicetool.ChangeNodeValue("C:\\WebAccess\\WebAccess\\Config\\PDQConfiguration.xml", "/Segment[@Name='MSH']/Field[@Name='Receiving Facility']", "LOCAL");
                    login.ChangeAttributeValue("C:\\WebAccess\\WebAccess\\Web.config", "/add[@key='Application.QueryPatientIDInCFIND']", "value", "true");
                    servicetool.RestartIISUsingexe();
                }

                taskbar1 = new Taskbar();
                taskbar1.Show();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ConnectAllDatasourcesEditDomain();
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - XDS Configuration Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - XDS Configuration\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.steps[++executedSteps].status = "Fail";
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - XDS Configuration Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - XDS Configuration\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;

            }
        }

        /// <summary>
        /// This Test method is to perform Steps:
        /// 1. Generate Exam Importer and POP for SuperAdminGroup
        /// 2. Create Users, Instituition, and Destination
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_GeneratePOPandExamImporter(String testid, String teststeps, int stepcount)
        {

            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;
            Taskbar taskbar = null;

            try
            {
                const string domainName = "SuperAdminGroup";

                //Hide TaskBar
                taskbar = new Taskbar();
                taskbar.Hide();

                //Generate Exam Importer and POP
                tool.InvokeServiceTool();
                tool.UpdateInstallerUrl();
                tool.GenerateInstallerPOP("SuperAdminGroup", "");
                tool.GenerateInstallerAllDomain(domainName, Config.eiwindow);
                wpfobject.WaitTillLoad();
                tool.RestartService();
                tool.CloseServiceTool();

                //Show Taskbar
                taskbar.Show();

                //Login iConnect Access UI
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //Creating new users for Image Sharing setup
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Config.ph2UserName, "SuperAdminGroup", "Physician", 1, Config.ph2Email, 1, Config.ph2Password);
                usermanagement.CreateUser(Config.ar2UserName, "SuperAdminGroup", "Archivist", 1, Config.ar2Email, 1, Config.ar2Password);
                usermanagement.CreateUser(Config.newUserName, "SuperAdminGroup", "Staff", 1, Config.stEmail, 1, Config.newPassword);

                //Create Instituitions
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.SetDateRange();
                settings.SaveSystemSettings();
                basepage.AddInstitution(Config.Inst1, Config.ipid1);
                basepage.AddInstitution(Config.Inst2, Config.ipid2);

                //Create Destinations               
                if (String.IsNullOrWhiteSpace(Config.DestinationPACS))
                {
                    Logger.Instance.ErrorLog("'Config.DestinationPACS' is Empty, So Creating only one destination using Destination EA");
                    if (!String.IsNullOrWhiteSpace(Config.DestEAsIp))
                        basepage.AddDestination("SuperAdminGroup", Config.Dest1, basepage.GetHostName(Config.DestEAsIp), Config.ph1UserName, Config.ar1UserName);
                    else
                    {
                        Logger.Instance.ErrorLog("'Config.DestEAsIp' is also empty. Cannot create Destination. Check the AutomationConfig.xml");
                        throw new Exception("Cannot create Destination as 'Config.DestinationPACS' and 'Config.DestEAsIp' are empty. Check the AutomationConfig.xml");
                    }
                }
                else
                {
                    basepage.AddDestination("SuperAdminGroup", Config.Dest1, basepage.GetHostName(Config.DestinationPACS), Config.ph1UserName, Config.ar1UserName);
                    basepage.AddDestination("SuperAdminGroup", Config.Dest2, basepage.GetHostName(Config.DestEAsIp), Config.ph2UserName, Config.ar2UserName);
                }
                basepage.CloseBrowser();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Generate Exam Importer & PACS Gateway Installers Completed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Generate Exam Importer & PACS Gateway Installers\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;

            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.steps[++executedSteps].status = "Fail";
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Send Email Notification
                String EmailSubject = Environment.MachineName.ToUpper() + " - Generate Exam Importer & PACS Gateway Installers Failed";// + "\r\n Server IP - " + Config.IConnectIP +
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Server IP - " + Config.IConnectIP + "\r\n\n Task\t - Generate Exam Importer & PACS Gateway Installers\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_PreCondition_3D(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;
            try
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetReceivingInstitution("3DInstitution");
                domainmanagement.ConnectDataSources();
                domainmanagement.SetViewerTypeInNewDomain();
                domainmanagement.Enable3DView();
                IWebElement group = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-last-of-type(1)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Redo Segmentation", group);
                dictionary.Add("Undo Segmentation", group);
                domainmanagement.AddToolsToToolbox(dictionary, "3D");
                domainmanagement.SaveButton().Click();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("universalviewer", 0);
                rolemanagement.SetCheckboxInEditRole("3dview", 0);
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.steps[++executedSteps].status = "Fail";
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
            }
            return result;
        }

        public TestCaseResult Test_PreCondition_CopyDatasourceFromController(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);

            int executedSteps = -1;

            //Current Directory path
            String currentDirectory = System.IO.Directory.GetCurrentDirectory();

            try
            {    //Add Data Source
                String DataSourceXmlPath = @"\\" + Config.IConnectIP + @"\c$\WebAccess\WebAccess\Config\DataSource\DataSourceManagerConfiguration.xml";
                if (File.Exists(DataSourceXmlPath))
                {
                    // Create an XmlDocument
                    XmlDocument xmlDocument = new XmlDocument();

                    // Load the XML file in to the document
                    if (Dns.GetHostName().ToUpper().Equals(new BasePage().GetHostName(Config.IConnectIP)))
                    {
                        String newpath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + new BasePage().GetHostName(Config.IConnectIP) + Path.DirectorySeparatorChar + "DataSourceManagerConfiguration" + Path.DirectorySeparatorChar + DataSourceXmlPath.Split('\\').LastOrDefault();
                        Logger.Instance.InfoLog("datasource xml path: " + newpath);
                        //BasePage.CopyFileFromAnotherMachine(Config.IConnectIP, newpath, DataSourceXmlPath);
                        Logger.Instance.InfoLog("Data Source Copied to" + DataSourceXmlPath);
                    }
                }

                tool.RestartIIS();
                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test method is to perform:
        /// 1.Use to configure the IntegratorAuthentication Project 
        /// </summary>
        public TestCaseResult Test_IntegratorForAuthentication(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            //ServiceTool servicetool = new ServiceTool();
            int executedSteps = -1;

            try
            {
                Logger.Instance.InfoLog("PreCondition_MergePacs Method is running..!");

                //Launching Service Tool
                tool.LaunchServiceTool();
                Logger.Instance.InfoLog("Service Tool Launched");

                //Clicking on Integrator Tab
                //tool.NavigateToIntegratorTab();
                tool.NavigateToTab(ServiceTool.Integrator_Tab);
                Logger.Instance.InfoLog("Clicked Integrator Tab");

                //Enable Merge PACS Integrator Authentication and Add PACS IP
                tool.AddPACSIPInIntegratorTab(Config.MergePACsIP);

                //Closeing Service Tool
                tool.CloseServiceTool();

                //Enabling DataSources in WadoWS tab
                tool.WadoWSSetup();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test method is to enable universal viewere in Domain Management and Role Management pages:
        /// 1.Use to configure the IntegratorAuthentication Project 
        /// </summary>
        public TestCaseResult Test_EnableUniversalViewer(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                Logger.Instance.InfoLog("To Enable universal viewer in domain and role management pages");

                //Update default viewer in Webaccess UI
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                bool blnUpdateDomainCheck = basepage.UpdateGivenDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("universalviewer", 0);
                domainmanagement.ClickSaveEditDomain();

                //Different Role and uers
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("universalviewer", 0);
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// This Test method is to set High availability related environment setup
        /// </summary>
        public TestCaseResult Test_HighAvailability(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                if (Config.HighAvilabilitySetUp.ToLower().Equals("y"))
                {
                    String iConnectIP = Config.IConnectIP;
                    String HostName = basepage.GetHostName(iConnectIP).ToLower();
                    String ModifiedHostName = HostName + ".pqawhi.com";
                    String iConnectURL = "https://" + ModifiedHostName;
                    string installerICAurl = Config.LB_InstallerURL; //@"https://icaf5ssh.pqawhi.com";
                    string dbInstance = Config.LB_SQLDBName.ToUpper() + @"\WEBACCESS"; //ICA-SQLDB\WEBACCESS
                    string dbUserId = "sa";
                    string dbPassword = "Cedara123";

                    //Update value in XML(c:/Webaccess/Webaccess/web.config) as <add key="Application.EnableValidateServerCertificate" value="NameMismatch" />
                    string xmlPath = @"c:\Webaccess\Webaccess\web.config";
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlPath);
                    XmlElement nodeToUpdate = (XmlElement)xmlDoc.SelectSingleNode("//add[@key='Application.EnableValidateServerCertificate']");
                    if (nodeToUpdate != null)
                    {
                        string oldVal = nodeToUpdate.GetAttribute("value");
                        Logger.Instance.InfoLog(@"c:\Webaccess\Webaccess\web.config" + " Old value is :" + "<add key=\"Application.EnableValidateServerCertificate\" value=\"" + oldVal + "\" />");
                        nodeToUpdate.SetAttribute("value", "NameMismatch");
                        xmlDoc.Save(xmlPath);
                        Logger.Instance.InfoLog(@"c:\Webaccess\Webaccess\web.config" + " value updated as :" + "<add key=\"Application.EnableValidateServerCertificate\" value=\"NameMismatch\" />");
                    }
                    else
                        throw new Exception("<add key=\"Application.EnableValidateServerCertificate\" ...> not found in c:\\Webaccess\\Webaccess\\web.config file");

                    Logger.Instance.InfoLog("Set Service tool - Security with http & https: Started");
                    //Update security with both http and https --"ica-s12-r3.pqawhi.com"                    
                    ServiceTool servicetool = new ServiceTool();
                    servicetool.LaunchServiceTool();
                    servicetool.NavigateToTab(ServiceTool.Security_Tab);
                    servicetool.ClickModifyButton();
                    wpfobject.WaitTillLoad();
                    CheckBox HTTPCheckbox = servicetool.HTTPChkbox();
                    CheckBox HTTPSCheckbox = servicetool.HTTPSChkbox();
                    HTTPCheckbox.Checked = true;
                    HTTPSCheckbox.Checked = true;
                    wpfobject.WaitTillLoad();
                    servicetool.FQDN_txt().BulkText = ModifiedHostName;
                    FileUtils.AddToHostsFile(iConnectIP + " \t " + ModifiedHostName);
                    servicetool.CickApplyButton();
                    wpfobject.WaitTillLoad();
                    try
                    {
                        WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Yes")).Click();
                        Logger.Instance.InfoLog("yes clicked : 1");
                        wpfobject.WaitTillLoad();
                    }
                    catch (Exception err)
                    {
                        Logger.Instance.ErrorLog("Error in Clicking Yes button. " + err.Message);
                    }
                    try
                    {
                        WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("2")).Click();
                        Logger.Instance.InfoLog("ok clicked: 1");
                        wpfobject.WaitTillLoad();
                    }
                    catch (Exception err)
                    {
                        Logger.Instance.ErrorLog("Error in Clicking ok button. " + err.Message);
                    }
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    servicetool.RestartIIS();
                    Logger.Instance.InfoLog("Set Service tool - Security with http & https: Completed");

                    Logger.Instance.InfoLog("Set Service tool - Imagesharing - Installer ICA url: Started");
                    wpfobject.WaitTillLoad();
                    servicetool.NavigateToTab(ServiceTool.ImageSharing_Tab);
                    servicetool.NavigateSubTab(ServiceTool.ImageSharing.Name.UploadDeviceSettings_tab);
                    servicetool.WaitWhileBusy();
                    //Get Upload device Settings tab
                    ITabPage DeviceSettingsTab = servicetool.GetCurrentTabItem();
                    Button ModifyBtn = wpfobject.GetAnyUIItem<ITabPage, Button>(DeviceSettingsTab, ServiceTool.ModifyBtn_Name, 1);
                    TextBox iConnectURLTxtBox = wpfobject.GetAnyUIItem<ITabPage, TextBox>(DeviceSettingsTab, ServiceTool.ImageSharing.ID.iConnectURL);
                    wpfobject.WaitTillLoad();
                    //Click Modify Btn                
                    ModifyBtn.Click();
                    wpfobject.WaitTillLoad();
                    iConnectURLTxtBox.BulkText = installerICAurl;
                    wpfobject.WaitTillLoad();
                    //Click Apply button
                    wpfobject.GetAnyUIItem<ITabPage, Button>(DeviceSettingsTab, ServiceTool.ApplyBtn_Name, 1).Click();
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    try
                    {
                        wpfobject.WaitForPopUp();
                        //Click OK button
                        wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, ServiceTool.OkBtn_Name, 1).Click();
                        wpfobject.WaitTillLoad();
                    }
                    catch (Exception) { }
                    servicetool.RestartIISandWindowsServices();
                    servicetool.RestartIIS();
                    Logger.Instance.InfoLog("Set Service tool - Imagesharing - Installer ICA url: Completed");

                    Logger.Instance.InfoLog("Set Service tool - High Availability - Enable: started");
                    servicetool.NavigateToTab(ServiceTool.HighAvailability_Tab);
                    servicetool.modifyBtn().Click();
                    wpfobject.WaitTillLoad();
                    CheckBox HighAvailability_CB = servicetool.HighAvailability_CB(); //Enable checkbox
                    HighAvailability_CB.Checked = true;
                    wpfobject.WaitTillLoad();
                    servicetool.ApplyBtn().Click();
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    servicetool.RestartIIS();
                    Logger.Instance.InfoLog("Set Service tool - High Availability - Enable: Completed");

                    Logger.Instance.InfoLog("Set Service tool - Change DB name: started");
                    servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
                    wpfobject.WaitTillLoad();
                    servicetool.modifyBtn().Click();
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickRadioButton(ServiceTool.UserManagementDataBase.ID.SQLServerAuthentication, 0);
                    servicetool.SQLServerInstance_TxtBx().BulkText = dbInstance;
                    servicetool.SQLUserID_TxtBx().BulkText = dbUserId; //sa                    
                    servicetool.SQLPassword_TxtBx().BulkText = dbPassword; //Cedara123
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickOkPopUp();
                    wpfobject.WaitTillLoad();
                    //wpfobject.ClickButton("OK", 1);                                      
                    //wpfobject.WaitTillLoad();                                      
                    servicetool.RestartIISandWindowsServices();
                    servicetool.RestartIIS();
                    Logger.Instance.InfoLog("Set Service tool - Change DB name: completed");

                    Logger.Instance.InfoLog("Set Service tool - Enable reports: started");
                    servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                    wpfobject.WaitTillLoad();
                    servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.GetButton(ServiceTool.ModifyBtn_Name, 1).Click();
                    wpfobject.WaitTillLoad();
                    //Enable Encapsulated PDF
                    servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = true;
                    wpfobject.WaitTillLoad();
                    //Enable Cardio reports
                    servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.MergeCardioReport).Checked = true;
                    wpfobject.WaitTillLoad();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    servicetool.RestartIIS();
                    Logger.Instance.InfoLog("Set Service tool - Enable reports: completed");

                    Logger.Instance.InfoLog("Set Service tool - Configure Z3D server configuration: started");
                    servicetool.NavigateToTab("Viewer");
                    servicetool.NavigateSubTab("3D Viewer");
                    servicetool.Select3DConfiguration("Z3D");
                    wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.Add3DURLItem(Config.IConnectIP, https: false);
                    servicetool.Add3DURLItem(new BasePage().GetHostName(Config.IConnectIP), https: false);
                    servicetool.Add3DURLItem(new BasePage().GetHostName(Config.IConnectIP) + ".pqawhi.com", https: true);
                    servicetool.SetDefault3DURLItem(Config.IConnectIP);
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    servicetool.RestartIISandWindowsServices();
                    servicetool.RestartIIS();
                    Logger.Instance.InfoLog("Set Service tool - Configure Z3D server configuration: completed");
                    servicetool.CloseServiceTool();

                    Logger.Instance.InfoLog("Run Z3D_UpdateConfigFromICA.ps1 : started");
                    var proc = new Process
                    {
                        StartInfo =
                        {
                            FileName = "Z3DConfigUpdate.bat",
                            WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles",
                            UseShellExecute = true,
                        }
                    };
                    proc.Start();
                    proc.WaitForExit(180000);
                    if (!proc.HasExited) { proc.CloseMainWindow(); }
                    Logger.Instance.InfoLog("Run Z3D_UpdateConfigFromICA.ps1 : completed");
                }
                else
                    Logger.Instance.InfoLog("High avalability set up not required");

                //Update Result
                ++executedSteps;
                result.steps[executedSteps].status = "Pass";
                result.FinalResult(executedSteps);
                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_Install3D(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String Z3DBuildPath = Config.Z3DBuildPath;
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory( Z3DBuildPath);
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory(BasePage.LatestZ3DBuild_Path);
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory(BasePage.LatestZ3DBuild_Path);
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory(BasePage.LatestZ3DBuild_Path);
               // string SetupFilePath = "\\AssembleBuild\\AssembleBuild";
                
                String Z3dBuilds = BasePage.LatestZ3DBuild_Path + "\\Installer";
                String Z3DInstaller = "Z3D_ICAinstaller.msi";
                var psi = new ProcessStartInfo(Z3dBuilds + "\\" + Z3DInstaller);
                psi.UseShellExecute = true;
                WpfObjects._application = TestStack.White.Application.AttachOrLaunch(psi);
                WpfObjects._application.WaitWhileBusy();
                Thread.Sleep(30000);
                ProcessStartInfo procStartInfo = new ProcessStartInfo();
                procStartInfo.FileName = Directory.GetCurrentDirectory() + "\\OtherFiles\\Z3DInstall.bat";
                procStartInfo.Arguments = "";
                procStartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\OtherFiles";
                procStartInfo.UseShellExecute = true;
                Process proc = Process.Start(procStartInfo);
                proc.WaitForExit();

                result.steps[++ExecutedSteps].status = "Pass";

                //Return Result
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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }
        /// <summary>
        /// Method to install Z3D build, add 3D data source in service tool, Enable 3D in domain and role management page
        /// <creator>RAV<creator>
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_PreCondition3D(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //STep 1
                var result1 = Test_Install3D(testid, result.steps[0].description + "=" + result.steps[0].expectedresult, 1);
                if (result1.status.Equals("Pass"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Error in Installing 3D Build");
                }

                //Step2
                //Update in compression.config file
                String CompressionFilepath = "C:\\drs\\sys\\data\\3D\\compression.config";
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Poor']/quality/mpr", "i", "60");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Poor']/quality/mpr", "f", "90");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Poor']/quality/_3d", "i", "60");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Poor']/quality/_3d", "f", "90");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Poor']/scaling", "i", ".25");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Poor']/scaling", "f", "1.0");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Medium']/quality/mpr", "i", "60");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Medium']/quality/mpr", "f", "90");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Medium']/quality/_3d", "i", "60");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Medium']/quality/_3d", "f", "90");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Medium']/scaling", "i", ".25");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Medium']/scaling", "f", "1.0");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Good']/quality/mpr", "i", "60");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Good']/quality/mpr", "f", "90");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Good']/quality/_3d", "i", "60");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Good']/quality/_3d", "f", "90");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Good']/scaling", "i", ".25");
                login.ChangeAttributeValue(CompressionFilepath, "/adaptive_compression/range[@label='Good']/scaling", "f", "1.0");

                tool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

                tool.AddEADatasource(Config.DestEAsIp, Config.AETitleDestEA, dataSourceName: Config.AETitleDestEA);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                if (wpfobject.VerifyIfTextExists(Config.AETitleDestEA))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Error in Adding Data source in Service tool");
                }
                tool.NavigateToTab("Viewer");
                tool.NavigateSubTab("3D Viewer");
                tool.Select3DConfiguration("Z3D");
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                tool.Add3DURLItem(Config.IConnectIP, https: false);
                tool.SetDefault3DURLItem(Config.IConnectIP);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();

                //Step3 
                result1 = Test_PreCondition_3D(testid, result.steps[2].description + "=" + result.steps[2].expectedresult, 1);
                if (result1.status.Equals("Pass"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Error in Enableing 3D in domain and role management ");
                }
               
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Send Email Notification -- ServerName + " - Build Installation Completed";
                String EmailSubject = Environment.MachineName.ToUpper() + " -Z3D Build Installation Completed";
                String EmailBody = EmailHeader + " Server Name\t - " + Environment.MachineName + "\r\n Task\t - Z3D Build Installation\r\n Status\t - " + result.status.ToUpper() + EmailFooter;
                EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// This function is to Connect all DataSource and set Institution name
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_ConnectDataSource(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                const string domainName = "SuperAdminGroup";

                //Update Super Admin Group in Domain Management (Connect DataSource and set Institution name)            
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SelectDomain(domainName);
                domainmanagement.ClickEditDomain();
                domainmanagement.ReceivingInstTxtBox().Clear();
                domainmanagement.ReceivingInstTxtBox().SendKeys("Added DataSource");
                string[] disconnectedAllOptions = domainmanagement.GetValuesfromDropDown("cssselector", "select[id$='DataSourceDisconnectedListBox']");
                
                if (disconnectedAllOptions.Count().Equals(2))
                {
                    if (Config.IConnectIP.Equals("10.9.38.70"))
                        domainmanagement.ConnectAllDataSources();
                    else
                        domainmanagement.ConnectDataSource("EA");
                }
                else
                    domainmanagement.ConnectDataSource("DATASOURCE1");

                // Adding Save Series tool
                var precond2 = login.GetConfiguredToolsInToolBoxConfig();
                if (!(precond2.Contains("Save Series")) || !(precond2.Contains("Save Annotated Images")))
                {
                    IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                    var dictionary = new Dictionary<String, IWebElement>();
                    if (!(precond2.Contains("Save Series")))
                    {
                        dictionary.Add("Save Series", group1);
                        domainmanagement.AddToolsToToolbox(dictionary);
                        Logger.Instance.InfoLog("Save Series is configured in the ToolBox");
                    }
                    if (!(precond2.Contains("Save Annotated Images")))
                    {
                        dictionary.Add("Save Annotated Images", group1);
                        domainmanagement.AddToolsToToolbox(dictionary);
                        Logger.Instance.InfoLog("Save Annotated Images is configured in the ToolBox");
                    }
                }

                domainmanagement.ClickSaveEditDomain();

                //Logout
                login.Logout();

                //Update Result
                result.steps[++executedSteps].status = "Pass";
                result.FinalResult(executedSteps);

                return result;
            }

            catch (Exception e)
            {

                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
        }

    }

}
