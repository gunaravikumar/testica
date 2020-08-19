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
using UpgradeUtility;

namespace Selenium.Scripts.Tests
{
    class Installation_BR
    {   
        
        //Properties
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }
        public iCAInstaller icainstaller { get; set; }
        public WpfObjects wpfobject { get; set; }
        public ServiceTool servicetool { get; set; }
        BasePage basepage;

        //Fields
        ServiceTool tool;       
        MpacLogin mpaclogin;
        MPHomePage mphomepage;
        MpacConfiguration mpacconfig;
        HPLogin hplogin;
        DomainManagement domainmanagement;
        RoleManagement rolemanagement;
        UserManagement usermanagement;        
        POPUploader pop;        
        ExamImporter ei;   
        String ServerName = null;
        UserPreferences userpref;
        int DistanceCounter;
        public string TransferserviceAETitle = "TFR_" + new BasePage().GetHostName(Config.IConnectIP).Replace("-", "");

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Installation_BR(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
            icainstaller = new iCAInstaller();
            servicetool = new ServiceTool();
            ehr = new EHR();
            this.tool = new ServiceTool();           
            login.DriverGoTo(login.url);
            this.mpaclogin = new MpacLogin();
            this.mphomepage = new MPHomePage();
            this.hplogin = new HPLogin();
            this.domainmanagement = new DomainManagement();
            this.usermanagement = new UserManagement();
            this.pop = new POPUploader();
            this.ei = new ExamImporter();
            this.basepage = new BasePage();           
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            this.mpacconfig = new MpacConfiguration();
            ServerName = login.GetHostName(Config.IConnectIP);
            DistanceCounter = 1;
            this.userpref = new UserPreferences();

        }

        /// <summary>
        /// Verifying Installation complete process
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_141028(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            StudyViewer viewer = null;
            Studies studies = null;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Uninstalling the existing build
                BasePage.Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();


                //Step-1:After a successful iCA installation, Navigate to C-\WebAccess\ and launch install.log file in text editor
                icainstaller.installiCA(1);
                bool LogFile = File.Exists("C:\\WebAccess\\install.log");
                if (LogFile)
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

                //Step-2: Verify the unwanted messages are not displayed like "Standard error-" and "Standard output-" in the log file
                StreamReader readfile = new StreamReader("C:\\WebAccess\\install.log");
                String msg = readfile.ReadToEnd();
                if (!msg.Contains("standard error: "))
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


                //Step-3:Verify the contents of the messages in the log file
                ExecutedSteps++;

                //Step-4:Verify the successful installation message is received at the end of the log file
                if (msg.Contains("Installation committed."))
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


                //Step-5:Verify the following files are available in the respective location Files and deliverables-
                //a.StudyDigestAsmx.cs(C -\WebAccess\WebAccess\App_Code)
                //b.samlPolicy.config(C -\WebAccess\WindowsService\Preprocessing\bin)
                //c.Web.config(C -\WebAccess\WebAccess\StudyDigest) - StudyDigest.asmx(C -\WebAccess\WebAccess\StudyDigest)
                bool File1 = File.Exists("C:\\WebAccess\\WebAccess\\App_Code\\StudyDigestAsmx.cs");
                bool File2 = File.Exists("C:\\WebAccess\\WindowsService\\Preprocessing\\bin\\samlPolicy.config");
                bool File3 = File.Exists("C:\\WebAccess\\WebAccess\\Web.config");
                bool File4 = File.Exists("C:\\WebAccess\\WebAccess\\StudyDigest\\StudyDigest.asmx");
                if (File1 && File2 && File3 && File4)
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

                //Step-6:Uninstall the application and remove the install.log file
                BasePage.Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
                ExecutedSteps++;

                //Step-7: Install iCA again but interrupt the installation in the middle and make sure iCA installation is stopped
                icainstaller.invokeiCAInstaller(1);
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wpfobject.ClickButton(iCAInstaller.InstallBtn_Name, 1);
                wpfobject.WaitForButtonExist(iCAInstaller.Installer_Name, "Cancel", 1);
                wpfobject.ClickButton("Cancel", 1);
                wpfobject.WaitForButtonExist(iCAInstaller.Installer_Name, "Yes", 1);
                wpfobject.ClickButton("Yes", 1);
                wpfobject.WaitForButtonExist(iCAInstaller.Installer_Name, iCAInstaller.FinishBtn_Name, 1);
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wpfobject.ClickButton(iCAInstaller.FinishBtn_Name, 1);
                BasePage.Kill_EXEProcess(iCAInstaller.InstallerEXE);
                ExecutedSteps++;

                //Step-8: Launch install.log file in text editor and verify successful installation message is not displayed in the log file
                if (!File.Exists("C:\\WebAccess"))
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
                BasePage.Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller.UninstalliCA();
            }
        }

        /// <summary>
        /// ConfigureSettings
        /// </summary>
        /// <param name="releaseFilePath"></param>
        private void ConfigureSettings(string releaseFilePath)
        {
            if (Config.IsConfigForUpgrade.ToLower().Equals("y"))
            {
                //Install License - Tested
                icainstaller.InstallLicense();

                //Setup HTTPS
                if (Config.HTTPSmode.ToLower().Equals("y"))
                {
                    icainstaller.EnableHTTPS();
                }

                //Add Data Sources - Tested
                icainstaller.AddDataSources();

                //Enable Features
                icainstaller.EnableGeneralFeatures();
                icainstaller.SetupEncapsulatedReports();

                //Setup EHR
                icainstaller.SetupIntegrator(releaseFilePath);

                //Add RDM Datasource
                icainstaller.SetupRDM();

                //Setup Image Sharing
                if (Config.SetImageSharing.ToLower().Equals("y"))
                {
                    icainstaller.SetupImageSharing();
                }                              

                //Setup Password Policy
                //icainstaller.SetupPasswordPolicy();

                //Setup Localization 
                //icainstaller.Setup_LocalizationSetup();

                //Encryption    
                icainstaller.Setup_Encryption();

                //Setup XDS
                if (!Config.SetXDS.ToLower().Equals("n"))
                {
                    icainstaller.SetupXDS();
                }

                //External Application
                icainstaller.Setup_ExternalApplication();             

            }

        }

        /// <summary>
        /// ConfigureSettingsPostUpgrade
        /// </summary>
        /// <param name="releaseFilePath"></param>
        private void ConfigureSettingsPostUpgrade(string releaseFilePath)
        {
            //Include settings that must be applied after upgrade, otherwise these will not be valid.

            if (Config.IsConfigForUpgrade.ToLower().Equals("y"))
            {
                //Setup License
                icainstaller.InstallLicense();

                //Setup EHR
                icainstaller.SetupIntegrator(releaseFilePath, "upgrade");

                //Setup Localization 
                //icainstaller.Setup_LocalizationSetup("upgrade");
            }
        }

        /// <summary>
        /// CopyFolders
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="destPath"></param>
        /// <param name="excludedFolders"></param>
        private void CopyFolders(string srcPath, string destPath, string [] excludedFolders)
        {
            foreach (string dirPath in Directory.GetDirectories(srcPath, "*",
                    SearchOption.AllDirectories))
            {
                if(!excludedFolders.Any(dirPath.Contains))
                {
                    Directory.CreateDirectory(dirPath.Replace(srcPath, destPath));
                }
            }
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(srcPath, "*.*",
                SearchOption.AllDirectories))
            {
                if (!excludedFolders.Any(newPath.Contains))
                {
                    File.Copy(newPath, newPath.Replace(srcPath, destPath), true);
                }
            }
        }

        private string GetWindowName(string buildversion)
        {
            if (buildversion.Contains("6.5"))
            {
                return iCAInstaller.Installer_Title_v6;
            }
            else
            {
                return iCAInstaller.Installer_Name;
            }
        }

        private bool UninstallWebAccess()
        {
            var installer = new iCAInstaller();
            BasePage.KillProcessByPartialName("UploaderTool");
            var uninstall = installer.UninstalliCA(true);
            try
            {
                for (int counterI = 0; counterI < 2; counterI++)
                {
                    Thread.Sleep(10000);
                    BasePage.RunRemoteCMDUsingPsExec("localhost", "Administrator", "PQAte$t123-"+login.GetHostName(Config.IConnectIP).ToLower(), "del /q/f/s %TEMP%\\*");
                    Thread.Sleep(10000);
                }
            }
            catch (Exception) { }
            if (uninstall.Contains("uninstall has completed"))
            {
                return true;
            }

            return false;
        }

        private void DownLoadInstallers(string [] arrUpgradepath, string downloadPath)
        {
            //Download installers.
            var installer = new iCAInstaller();
            if (Config.DownloadFreshInstaller.ToLower() == "y")
            {
                for (int i = 0; i < arrUpgradepath.Length; i++)
                {
                    string versionPath = Path.Combine(downloadPath, arrUpgradepath[i]);
                    //Always download current installer. But if previous installers already exist, do not download again.
                    if (i == arrUpgradepath.Length - 1)
                    {
                        if (Directory.Exists(versionPath))
                        {
                            Directory.Delete(versionPath, true);
                        }
                    }
                    if (!Directory.Exists(versionPath))
                    {
                        Directory.CreateDirectory(versionPath);
                        installer.DownloadICABuild(arrUpgradepath[i], versionPath);
                    }
                }
            }
        }

        /// <summary>
        /// This Test methis will install and upgrade ICA build
        /// And perform validation using new approach
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_InstallUpgrade(String testid, String teststeps, int stepcount)
        {
            //Set up Validation Steps
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;                 
            DirectoryInfo BuildInfo = new DirectoryInfo(Config.BuildPath);
            string basePath = Path.Combine(@"D:\Test_InstallUpgrade");
            string artifactPath = Path.Combine(basePath, "LastArtifacts");
            var arrUpgradepath = Config.UpgradePath.Split('=');
            string parentPath = Path.Combine(basePath + @"\UpgradePath_" + string.Join("_", arrUpgradepath), 
                string.Format(@"{0:yyyy-MM-dd}", DateTime.Now));
            string outPutPath = Path.Combine(parentPath, "Output");
            string inPutPath = Path.Combine(parentPath, "Input");
            String destWebAccesspathFresh = Path.Combine(inPutPath, "FreshInstall");
            String destWebAccesspathUpgrade = Path.Combine(inPutPath, "UpgradeInstall");
            string downloadPath = @"D:\WebAccess_build";
            string icaInstallerPath = @"archive\Output_ICA\WebAccess_Release\WebAccess\iCAInstaller.exe";
            var serverHostname = Environment.MachineName;
            string dbConnectionString = @"Data Source="+ serverHostname + @"\WEBACCESS;Initial Catalog=IRWSDB;User id ="
                                         +Config.DbUserName+";Password="+Config.DbPassword;         
            var installer = new iCAInstaller();

            try
            {
                /* Run the script with following command line parameters. Substitute appropriate values as needed.
 
                    -upgradepath "7.0_7.1"
                    provide previous version(s) and latest version for the upgrade to take place. For example 6.5_7.0_7.1 means it will upgrade starting from 6.5 to 7.0 to 7.1.

                    -setupconfigforupgrade "N"
                    Use this flag to turn configurations on/off
 
                    -downloadfreshinstaller "N"
                    Download the fresh installers if this flag is enabled. Note the latest installer build is always downloaded, only previously released installers will not be downloaded again.

                    -upgradecomparisonlevel
                    A value 0-3 for the upgrade comparisions. Default is 2.

                    -dbusername "sa"
                    Provide database login
 
                    -dbpassword "Cedara123"
                    Provide database password
                 */

                //Step1 - Delete the Output folders. For example depending on the upgrade path it would be a path like
                //    \Test_InstallUpgrade\UpgradePath_x_y_z
                //    where x, y and z are version numbers such as 6.5, 7.0 and 7.1 etc.                
                try
                {
                    //Delete the input/output folder if it already exists
                    if (Directory.Exists(parentPath))
                    {
                        Directory.Delete(parentPath, true);
                    }
                    if (Directory.Exists(artifactPath))
                    {
                        Directory.Delete(artifactPath, true);
                    }
                    
                    result.steps[++executedSteps].status = "Pass";
                }
                catch (Exception ex)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                }

                // Step2. Uninstall WebAccess if it already exists. Delete the installation path C:\WebAccess
                this.UninstallWebAccess();
                executedSteps++;
             

                //Step3 - Download installers if needed.
                this.DownLoadInstallers(arrUpgradepath, downloadPath);
                result.steps[++executedSteps].status = "Pass";


                //Step-4 - Run the fresh Install for the latest version/build of iCA.      
                var installversion1 = arrUpgradepath.Last<String>();                
                string installerFilePath = Path.Combine(Path.Combine(downloadPath, installversion1), icaInstallerPath);
                installer.installiCA(window_name: this.GetWindowName(installversion1), installerFilePath: installerFilePath);
                var installlog  = File.ReadAllText(Config.iCAInstalledPath + Path.DirectorySeparatorChar + "install.log");
                if(installlog.Contains("Installation committed"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    Logger.Instance.ErrorLog("InstallLog"+installlog);
                    result.steps[executedSteps].SetLogs();                    
                }

                //Step-5 Configure all features
                this.ConfigureSettings(installerFilePath);
                result.steps[++executedSteps].status = "Pass";

                //Step-6 -- Save DB Info
                //Use Database Comparison Utility to save database info to a folder named as "Fresh Install"
                try
                {
                    DatabaseComparisonUtility dbCompUtil = new DatabaseComparisonUtility(Path.Combine(destWebAccesspathFresh, "Database"), dbConnectionString, true);
                    dbCompUtil.GetDatabaseInfo("IRWSDB");
                    result.steps[++executedSteps].status = "Pass";
                }
                catch (Exception ex)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                }

                //Step-7 -- Save WebAccess to the output folder
                this.CopyFolders(Config.iCAInstalledPath, Path.Combine(destWebAccesspathFresh, "WebAccess"), new string[] { "ConfigBackup" });
                result.steps[++executedSteps].status = "Pass";


                //Step-8 Uninstall webaccess
                if (this.UninstallWebAccess())
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();                    
                }

                //Step-9 Install Previous Version
                var installversion6 = arrUpgradepath.First<String>();
                installerFilePath = Path.Combine(Path.Combine(downloadPath, installversion6), icaInstallerPath);
                installer.installiCA(0, window_name: this.GetWindowName(installversion6), installerFilePath: installerFilePath);
                installlog = File.ReadAllText(Config.iCAInstalledPath + Path.DirectorySeparatorChar + "install.log");
                if (installlog.Contains("Installation committed"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();                    
                }

                //Step-10   Enable and configure all features    
                this.ConfigureSettings(installerFilePath);
                result.steps[++executedSteps].status = "Pass";

                //Step-11 Upgrade to latest version
                installlog = "";
                for (int i = 1; i < arrUpgradepath.Length; i++)
                {
                    installerFilePath = Path.Combine(Path.Combine(downloadPath, arrUpgradepath[i]), icaInstallerPath);
                    //Upgrade
                    installlog = installlog + installer.upgradeiCA(this.GetWindowName(arrUpgradepath[i]), installerFilePath);
                }
                installlog = File.ReadAllText(Config.iCAInstalledPath + Path.DirectorySeparatorChar + "install.log");
                this.ConfigureSettingsPostUpgrade(installerFilePath);
                icainstaller.RestartIISUsingexe();
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
                Logger.Instance.InfoLog("Upgraded version of iCA is: "+icainstaller.getiCAVersion());
                if (icainstaller.getiCAVersion().Equals(arrUpgradepath.Last()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();                    
                }

                //Step-12
                //Use Database Comparison Utility to save database info to a folder named as "Upgrade Install"
                try
                {
                    DatabaseComparisonUtility dbCompUtil = new DatabaseComparisonUtility(Path.Combine(destWebAccesspathUpgrade, "Database"), dbConnectionString, true);
                    dbCompUtil.GetDatabaseInfo("IRWSDB");
                    result.steps[++executedSteps].status = "Pass";
                }
                catch(Exception ex)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                }

                //Step-13 save webaccess folder
                this.CopyFolders(Config.iCAInstalledPath, Path.Combine(destWebAccesspathUpgrade, "WebAccess"), new string[] { "ConfigBackup" });
                result.steps[++executedSteps].status = "Pass";

                //Step-14
                //Compare the WebAccess Folders and Files from Fresh Install and Upgrade Install using the Utility to identify any discrepancies
                try
                {
                    FileComparisonUtility compUtil = new FileComparisonUtility(Path.Combine(outPutPath, @"WebAccessComparison\Level0"));
                    bool compResult = compUtil.CompareFolders(Path.Combine(destWebAccesspathFresh, "WebAccess"), Path.Combine(destWebAccesspathUpgrade, "WebAccess"));

                    if (Convert.ToInt32(Config.UpgradeComparisonLevel) >= 1)
                    {
                        compUtil = new FileComparisonUtility(Path.Combine(outPutPath, @"WebAccessComparison\Level1"), true);
                        compResult = compUtil.CompareFolders(Path.Combine(destWebAccesspathFresh, "WebAccess"), Path.Combine(destWebAccesspathUpgrade, "WebAccess"));
                    }
                    if (Convert.ToInt32(Config.UpgradeComparisonLevel) >= 2)
                    {
                        compUtil = new FileComparisonUtility(Path.Combine(outPutPath, @"WebAccessComparison\Level2"), true, false, null, true);
                        compResult = compUtil.CompareFolders(Path.Combine(destWebAccesspathFresh, "WebAccess"), Path.Combine(destWebAccesspathUpgrade, "WebAccess"));
                    }
                    if (Convert.ToInt32(Config.UpgradeComparisonLevel) >= 3)
                    {
                        compUtil = new FileComparisonUtility(Path.Combine(outPutPath, @"WebAccessComparison\Level3"), true, true, null, true);
                        compResult = compUtil.CompareFolders(Path.Combine(destWebAccesspathFresh, "WebAccess"), Path.Combine(destWebAccesspathUpgrade, "WebAccess"));
                    }
                    result.steps[++executedSteps].status = compResult ? "Pass" : "Fail";
                }
                catch (Exception ex)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                }


                //Step-15
                //Compare the databases from Fresh Install and Upgrade Install using the Utility to identify any discrepancies
                try
                {
                    FileComparisonUtility compUtil = new FileComparisonUtility(Path.Combine(outPutPath, "DatabaseComparison"), false, true, "*.txt");
                    bool compResult = compUtil.CompareFolders(Path.Combine(destWebAccesspathFresh, "Database"), Path.Combine(destWebAccesspathUpgrade, "Database"));
                    result.steps[++executedSteps].status = compResult ? "Pass" : "Fail";
                }
                catch (Exception ex)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                }

                //Step-16 - copy results to artifacts folders
                this.CopyFolders(outPutPath, artifactPath, new string[] { });
                result.steps[++executedSteps].status = "Pass";

                //Report Result
                result.FinalResult(executedSteps);
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
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }            
        }
    }
}
