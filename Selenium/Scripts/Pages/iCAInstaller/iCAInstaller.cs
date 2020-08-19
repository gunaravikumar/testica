using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using Application = TestStack.White.Application;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Button = TestStack.White.UIItems.Button;
using RadioButton = TestStack.White.UIItems.RadioButton;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.iConnect;
using TestStack.White.UIItems.Finders;
using System.Net;

namespace Selenium.Scripts.Pages.iCAInstaller
{
    class iCAInstaller : BasePage
    {

        //Properties
        public String toolapppath;
        public String servicetoolProcessname;
        public ServiceTool servicetool;
        public Login login;
        public DomainManagement domainmanagement;
        public RoleManagement rolemanagement;
        public UserManagement usermanagement;  
        public int DistanceCounter;
        public ExamImporter ei;

        //Constrcutor
        public iCAInstaller()
        {
            toolapppath = "C:\\Program Files (x86)\\Cedara\\WebAccess\\ConfigTool.exe";
            servicetoolProcessname = "ConfigTool";
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            login = new Login();
            DistanceCounter = 0;
            ei = new ExamImporter();
        }

        //Constant
        public static string version = (string)Registry.GetValue(Registry.LocalMachine + @"\SOFTWARE\Wow6432Node\Cedara\WebAccess", "Version", null) ?? string.Empty;
        public static string[] arrUpgradepath = Config.UpgradePath.Split('=');
        public const String Installer_Name = "IBM iConnect Access Setup";
        public const String Installer_Name_v6 = "Merge iConnect Access Setup";
        public const String Installer_Title = "IBM iConnect Access";
        public const String Installer_Title_v6 = "Merge iConnect Access Setup";
        public const String InstallerEXE = "iCAInstaller";
        public const String InstallerEXE_v6 = "iCAInstaller";
        public const String IcaInstallerEXE = "iCAInstaller";
        public const String IcaInstallerEXE_v6 = "iCAInstaller";
        public const String W3WPEXE = "w3wp";
        public String Upgrade_Wndw1 = version.Contains("6.5") ? "Merge iConnect Access " + arrUpgradepath[1] + " Setup" : "IBM iConnect Access " + arrUpgradepath[1] + " Setup";
        public const String InstallBtn_Name = "Install";
        public const String FinishBtn_Name = "Finish";
        //public const String Defaultwebsite = "Default Web Site"; //Automation ID = 11259
        //public const String Customwebsite = "Custom Web Site"; //Automation ID = 309
        //public const String NextBtn = "Next"; //Automation ID = 11108

        //License
        public static String ServerName =  new Login().GetHostName(Config.IConnectIP);
        public static String License_Name = version.Contains("6.5") ? "License.xml":"BluRingLicense.xml";
        public static String LicensePath = "C:\\WebAccess\\WebAccess\\Config\\" + License_Name;
        public static String currentDirectory = System.IO.Directory.GetCurrentDirectory();
        public static String ConfigFileDirectory = currentDirectory + Path.DirectorySeparatorChar + "ServerConfigFiles" +
                Path.DirectorySeparatorChar + ServerName;
        public static String License_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + License_Name;

        /// <summary>
        /// Enum - Window Type
        /// </summary>
        public enum GetWindowType : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        /// <summary>
        /// This method is to invoke the iCA setup exe
        /// </summary>
        /// <param name="Release">0-indicates previous release/1-indicates current release</param>
        /// <param name="isAttach"></param>
        public void invokeiCAInstaller(int Release = 1, int isAttach = 0, bool isUpgrade = false, string installerFilePath = null)
        {
            try
            {
                KillProcess("msiexec.exe");
                if (Release == 0)
                {
                    if (isAttach != 0)
                    {
                        var x = Process.GetProcessesByName(string.IsNullOrEmpty(installerFilePath) ? Config.PrevReleaseFilePath : installerFilePath)[0].Id;
                        Logger.Instance.InfoLog("Application's process ID : " + x);
                        WpfObjects._application = Application.Attach(x);
                    }
                    else
                    {
                        var psi = new ProcessStartInfo(string.IsNullOrEmpty(installerFilePath) ? Config.PrevReleaseFilePath : installerFilePath);
                        psi.UseShellExecute = true;
                        WpfObjects._application = Application.AttachOrLaunch(psi);
                    }
                    Thread.Sleep(60000);
                    wpfobject.GetMainWindowByTitle(Installer_Name_v6);
                    wpfobject.WaitForButtonExist(Installer_Name_v6, InstallBtn_Name, 1);
                    Logger.Instance.InfoLog("Application launched : ");
                }
                else
                {
                    if (isAttach != 0)
                    {
                        var x = Process.GetProcessesByName(string.IsNullOrEmpty(installerFilePath) ? Config.CurrReleaseFilePath : installerFilePath)[0].Id;
                        Logger.Instance.InfoLog("Application's process ID : " + x);
                        WpfObjects._application = Application.Attach(x);
                    }
                    else
                    {
                        var psi = new ProcessStartInfo(string.IsNullOrEmpty(installerFilePath) ? Config.CurrReleaseFilePath : installerFilePath);
                        psi.UseShellExecute = true;
                        WpfObjects._application = Application.AttachOrLaunch(psi);
                    }
                    Thread.Sleep(60000);
                    if (!isUpgrade)
                    {
                        wpfobject.GetMainWindowByTitle(Installer_Name);
                        //if (installerFilePath.Contains("7.1"))
                        //{
                        //    wpfobject.WaitForButtonExist(Installer_Name, NextBtn, 1);
                        //}
                        //else
                        //{
                            wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
                        //}
                        Logger.Instance.InfoLog("Application launched : ");
                    }
                    else
                    {
                        wpfobject.StopService("iConnect Access MeaningfulUse Service");
                        wpfobject.StopService("iConnect Access Image Pre-fetch Service");
                        wpfobject.StopService("iConnect Access Part 10 Import Service");
                        wpfobject.StopService("iConnect Access Image Transfer Service");
                        Kill_EXEProcess(W3WPEXE);
                        wpfobject.GetMainWindowByTitle("iConnect Access");
                        wpfobject.ClickButton("Yes", 1);
                        Kill_EXEProcess(W3WPEXE);
                        wpfobject.GetMainWindowByTitle("iConnect Access");
                        wpfobject.ClickButton("OK", 1);
                    }
                }
            }
            catch (Exception ex)
            {
                KillProcess("msiexec.exe");
                Logger.Instance.ErrorLog("Exception in launching application from :  due to :" + ex);
            }
        }

        /// <summary>
        /// This method is to install iCA build
        /// </summary>
        /// <param name="Release">1-current build</param>
        public string installiCA(int Release = 1, String window_name= Installer_Name, string installerFilePath = null)//, string website = "default")
        {
            invokeiCAInstaller(Release, installerFilePath: installerFilePath);
            wpfobject.GetMainWindowByTitle(window_name);
            //if (installerFilePath.Contains("7.1"))
            //{
            //    if (string.Equals(website, "default"))
            //    {
            //        //wpfobject.ClickButton(Defaultwebsite, 1);
            //        wpfobject.GetAnyUIItem<Window, RadioButton>(WpfObjects._mainWindow, Defaultwebsite, 1).Click();
            //        wpfobject.ClickButton(NextBtn, 1);
            //        wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
            //    }
            //    else
            //    {
            //        wpfobject.GetAnyUIItem<Window, RadioButton>(WpfObjects._mainWindow, Customwebsite, 1).Click();
            //        wpfobject.ClickButton(NextBtn, 1);
            //        wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
            //    }
            //}
            //wpfobject.GetMainWindowByTitle(window_name);
            //wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);            
            //Thread.Sleep(60000);
            wpfobject.GetMainWindowByTitle(window_name);
            wpfobject.ClickButton(InstallBtn_Name, 1);
            //Logger.Instance.InfoLog("Install button clicked for first time..");
            //try
            //{
            //    wpfobject.GetMainWindowByTitle(window_name);
            //    wpfobject.WaitForButtonExist(Installer_Name, InstallBtn_Name, 1);
            //    Thread.Sleep(60000);
            //    wpfobject.GetMainWindowByTitle(window_name);
            //    wpfobject.ClickButton(InstallBtn_Name, 1);
            //    Logger.Instance.InfoLog("Install button clicked for second time..");
            //}
            //catch(Exception e) { Logger.Instance.ErrorLog("Exception on clicking install button.."+e); }
            wpfobject.GetMainWindowByTitle(window_name);
            wpfobject.WaitForButtonExist(window_name, FinishBtn_Name, 1);
            wpfobject.GetMainWindowByTitle(window_name);
            wpfobject.ClickButton(FinishBtn_Name, 1);
            Thread.Sleep(10000);
            Kill_EXEProcess(InstallerEXE);
            Thread.Sleep(10000);
            return getiCAVersion();
        }

        /// <summary>
        /// This is to uninstall iCA build
        /// </summary>
        /// <param name="Build">0=previous build/1=currentbuild</param>
        public bool uninstalliCA(int Build = 0)
        {
            bool un = false;
            bool st = false;
            bool iCA = false;
            try
            {
                string configtoolpath;
                string installerpath;
                if (Build == 0)
                {
                    configtoolpath = Config.PrevBuildConfigToolPath;
                    installerpath = Config.PrevBuildWebaccessInstallerPath + @"\WebAccessInstaller.msi";
                }
                else
                {
                    configtoolpath = Config.CurrBuildConfigToolPath;
                    installerpath = Config.CurrBuildWebaccessInstallerPath + @"\WebAccessInstaller.msi";
                }
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-x " + configtoolpath + " -quiet /L*v 'log.log'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                proc.Start();
                int i = 0;
                while (i < 30 && !proc.HasExited)
                {
                    st = true;
                    Thread.Sleep(500);
                    i++;
                }
                Logger.Instance.InfoLog("iCA Service Tool uninstalled succesfully");
                proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-x " + installerpath + " -quiet /L*v 'log.log'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                proc.Start();
                i = 0;
                while (i < 30 && !proc.HasExited)
                {
                    iCA = true;
                    Thread.Sleep(500);
                    i++;
                }
                Logger.Instance.InfoLog("iCA uninstalled succesfully");
                deleteDB();
                try
                {
                    Directory.Delete(Config.ServiceToolInstalledPath, true);
                    Logger.Instance.InfoLog("Cedara folder deleted succesfully");
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Cedara folder not deleted due to " + ex);
                }
                try
                {
                    Directory.Delete(Config.iCAInstalledPath, true);
                    Logger.Instance.InfoLog("Webaccess folder deleted succesfully");
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Webaccess folder not deleted due to " + ex);
                }
                if (st && iCA)
                    un = true;
                Thread.Sleep(5000);
                KillProcess("msiexec.exe");
                return un;

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while uninstalling iCA due to :" + ex);
                deleteDB();
                Logger.Instance.InfoLog("Database deleted succesfully");
                try
                {
                    Directory.Delete(Config.ServiceToolInstalledPath, true);
                    Logger.Instance.InfoLog("Cedara folder deleted succesfully");
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Cedara folder not deleted");
                }
                try
                {
                    Directory.Delete(Config.iCAInstalledPath, true);
                    Logger.Instance.InfoLog("Webaccess folder deleted succesfully");
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Webaccess folder not deleted");
                }
                Thread.Sleep(5000);
                KillProcess("msiexec.exe");
                return un;

            }
        }

        /// <summary>
        /// This is to delete DB of iCA
        /// 1=local server WEBACCESS INSTANCE, 2=another server WEBACCESS instance, 3=another server TEST instance
        /// </summary>
        public void deleteDB(int DBServer = 1, String RemoteHostName = null)
        {
            if (DBServer == 1)
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\DB_delete.bat";
                proc.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
                proc.Start();
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("iCA DB deleted");
            }
            else if (DBServer == 2)
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\delete_DB_2_WA.bat " + RemoteHostName;
                proc.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
                proc.Start();
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("iCA DB deleted");
            }
            else if (DBServer == 3)
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\delete_DB_2_TI.bat " + RemoteHostName;
                proc.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
                proc.Start();
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("iCA DB deleted");
            }
        }
        
        /// <summary>
        /// Run off.Bat file
        /// </summary>
        /// <param name="status"></param>
        public void features(string status)
        {
            if (status == "OFF")
            {
                Process batchprocess = new Process();
                batchprocess.StartInfo.FileName = @"D:\imgdrv\features-OFF.bat";
                batchprocess.Start();
                batchprocess.WaitForExit(120000);
                if (!batchprocess.HasExited) { batchprocess.CloseMainWindow(); }
            }
            else
            {
                Process.Start(@"D:\imgdrv\features-ON.bat");
            }
        }

        /// <summary>
        /// This is to upgrade to current build from previous build of iCA
        /// </summary>
        public string upgradeiCA(String windowname=null, string installerFilePath = null)
        {
            if (windowname == null) { windowname = Upgrade_Wndw1; }
            wpfobject.StopService("iConnect Access MeaningfulUse Service");
            wpfobject.StopService("iConnect Access Image Pre-fetch Service");
            wpfobject.StopService("iConnect Access Part 10 Import Service");
            wpfobject.StopService("iConnect Access Image Transfer Service");
            KillProcess("msiexec.exe");
            invokeiCAInstaller(isUpgrade:true, installerFilePath: installerFilePath);
            /*Thread.Sleep(30000);
            wpfobject.GetMainWindowByTitle(windowname);
            wpfobject.ClickButton("6");*/
            wpfobject.GetMainWindowByTitle("iConnect Access");
            wpfobject.WaitTillLoad();
            wpfobject.WaitForButtonExist("iConnect Access", "OK", 1);
            wpfobject.GetMainWindowByTitle("iConnect Access");
            Kill_EXEProcess(W3WPEXE);
            var btnok = wpfobject.GetButton("OK", 1);
            try { btnok.Click(); } catch (Exception) { }
            Thread.Sleep(30000);
            /*wpfobject.GetMainWindowByTitle("iConnect Access");
            wpfobject.ClickButton("OK", 1);*/
            return getiCAVersion();
        }

        /// <summary>
        /// This is to check if the virtual directories exist
        /// </summary>
        public bool ListVirtualDirectories()
        {
            DirectoryEntry webService = new DirectoryEntry("IIS://localhost/W3SVC/1/ROOT");
            bool directory = false;
            try
            {
                foreach (DirectoryEntry webDir in webService.Children)
                {
                    if (webDir.Name.ToLower().Contains("webaccess"))
                    {
                        directory = true;
                        break;
                    }
                    else
                    {
                        directory = false;
                    }
                }
                return directory;
            }
            catch
            {
                return directory;
            }
        }

        /// <summary>
        /// Returns window Handle
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public int findWindow(string windowName, bool wait)
        {
            int hWnd = FindWindow(null, windowName);

            while (wait && hWnd == 0)
            {
                System.Threading.Thread.Sleep(500);
                hWnd = FindWindow(null, windowName);
            }

            return hWnd;
        }

        /// <summary>
        /// Gets Window handle
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public IntPtr getWindow(string windowName)
        {
            IntPtr dlgHndl = IntPtr.Zero;
            IntPtr hWnd = (IntPtr)FindWindow(null, windowName);
            dlgHndl = GetWindow(hWnd, GetWindowType.GW_ENABLEDPOPUP);
            return dlgHndl;
        }    

        //[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, long nMaxCount);
        public string GetCaptionOfWindow(IntPtr hwnd)
        {
            string caption = "";
            StringBuilder windowText = null;
            try
            {
                int max_length = GetWindowTextLength(hwnd);
                windowText = new StringBuilder("", max_length + 5);
                GetWindowText(hwnd, windowText, max_length + 2);

                if (!String.IsNullOrEmpty(windowText.ToString()) && !String.IsNullOrWhiteSpace(windowText.ToString()))
                    caption = windowText.ToString();
            }
            catch (Exception ex)
            {
                caption = ex.Message;
            }
            finally
            {
                windowText = null;
            }
            return caption;
        }        

        /// <summary>
        /// This is to iCA in FullUI mode
        /// </summary>
        public void invokeiCAFullUi(int release = 1)
        {
            KillProcess("msiexec.exe");
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = @"D:\imgdrv\fullUI.bat";
            if (release == 1)
                proc.StartInfo.WorkingDirectory = Config.CurrBuildWebaccessInstallerPath;
            else if (release == 2)
                proc.StartInfo.WorkingDirectory = Config.BuildPath + @"\archive\Output_ICA\WebAccess_Release\WebAccess\en-US";
            else
                proc.StartInfo.WorkingDirectory = Config.PrevBuildWebaccessInstallerPath;
            proc.Start();
            Thread.Sleep(2000);
            Logger.Instance.InfoLog("iCA fullUI mode launched");
        }        

        /// <summary>
        /// Uninstall ICA Build
        /// </summary>
        public String UninstalliCA(bool deleteWebAccessPath = true)
        {
            String uninstalllog = String.Empty;

            try
            {
                //Stop Processes
                wpfobject.StopService("iConnect Access MeaningfulUse Service");
                wpfobject.StopService("iConnect Access Image Pre-fetch Service");
                wpfobject.StopService("iConnect Access Part 10 Import Service");
                wpfobject.StopService("iConnect Access Image Transfer Service");
                KillProcess("msiexec.exe");

                //Uninstall iCA iConnect Access
                System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
                proc1.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\UninstalliConnectAccess.bat";
                proc1.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";                
                proc1.Start();
                proc1.WaitForExit(600000);
                Thread.Sleep(5000);
                Logger.Instance.InfoLog("iCA uninstalled succesfully");

                //Uninstall Service Tool
                System.Diagnostics.Process proc2 = new System.Diagnostics.Process();
                proc2.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\UninstallServiceTool.bat";
                proc2.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
                proc2.Start();
                proc2.WaitForExit(600000);                
                Thread.Sleep(5000);
                Logger.Instance.InfoLog("iCA Service Tool uninstalled succesfully");


                deleteDB();
                try
                {
                    Directory.Delete(Config.ServiceToolInstalledPath, true);
                    Logger.Instance.InfoLog("Cedara folder deleted succesfully");
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Cedara folder not deleted due to " + ex);
                }

                uninstalllog = File.ReadAllText(Config.iCAInstalledPath + Path.DirectorySeparatorChar + "uninstall.log");
                if (deleteWebAccessPath)
                {
                    try
                    {
                        Directory.Delete(Config.iCAInstalledPath, true);
                        Logger.Instance.InfoLog("Webaccess folder deleted succesfully");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.InfoLog("Webaccess folder not deleted due to " + ex);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while uninstalling iCA due to :" + ex);
                deleteDB();
                Logger.Instance.InfoLog("Database deleted succesfully");
                try
                {
                    Directory.Delete(Config.ServiceToolInstalledPath, true);
                    Logger.Instance.InfoLog("Cedara folder deleted succesfully");
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Cedara folder not deleted");
                }
                try
                {
                    Directory.Delete(Config.iCAInstalledPath, true);
                    Logger.Instance.InfoLog("Webaccess folder deleted succesfully");
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Webaccess folder not deleted");
                }

            }

            return uninstalllog;
        }

        /// <summary>
        /// Delete Db for instance 
        /// </summary>
        /// <param name="RemoteHostName"></param>
        /// <param name="InstanceName"></param>
        public void deleteDB(String RemoteHostName, String InstanceName = "WebAccess")
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\Instance_delete_DB.bat";
            proc.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
            proc.StartInfo.Arguments = RemoteHostName + "\\" + InstanceName;
            proc.Start();
            Thread.Sleep(2000);
            Logger.Instance.InfoLog("iCA DB deleted");
        }

        /// <summary>
        /// This is to install License
        /// </summary>
        public void InstallLicense()
        {
            if (this.getiCAVersion().Contains("6.5") || this.getiCAVersion().Contains("6.5.1"))
            {
                try { File.Copy(ConfigFileDirectory + "\\License.xml", "C:\\WebAccess\\WebAccess\\Config\\License.xml", true); }
                catch (Exception) { }
            }
            else
            {
                try { File.Copy(License_Backup, LicensePath, true); }
                catch (Exception) { }
            }
            RestartIISUsingexe();
        }

        /// <summary>
        /// This Test method is to perform:
        /// 1. Adding the required Data sources in the respective execution servers: Holding pen, EA, PACS, XDS and RDM
        /// 2. Setup ImageSharing as Y in Web.config file, if image sharing setup is needed in the respective server
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public void AddDataSources()
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
            servicetool.LaunchServiceTool();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

            //Adding Holding Pen as data source                
            if (!String.IsNullOrEmpty(AddittionalServers_IP["HoldingPen"]))
            {
                //Set Enable ImageSharing value as true
                login.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");

                DistanceCounter++;
                servicetool.AddEADatasource(AddittionalServers_IP["HoldingPen"], AddittionalServers_AETitle["HoldingPen"], "" , IsHoldingPen: 1);
            }

            //Adding EA data sources
            foreach (String EAIP in EA_DataSources_IP.Keys)
            {
                var isDeidentificationEnable = false;
                if (!String.IsNullOrEmpty(EA_DataSources_IP[EAIP]))
                {
                    this.DistanceCounter++;
                    if (EA_DataSources_IP[EAIP] == Config.EA7)
                    {
                        isDeidentificationEnable = true;
                    }
                    servicetool.AddEADatasource(EA_DataSources_IP[EAIP], EA_DataSources_AETitle[EAIP], "" , EnableDeidentification: isDeidentificationEnable);
                }
            }

            //Adding PACS data sources
            foreach (String PACSIP in PACS_DataSources_IP.Keys)
            {
                if (!String.IsNullOrEmpty(PACS_DataSources_IP[PACSIP]))
                {
                    this.DistanceCounter++;
                    servicetool.AddPacsDatasource(PACS_DataSources_IP[PACSIP], PACS_DataSources_AETitle[PACSIP], "", Config.pacsadmin, Config.pacspassword);
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
            
        }

        /// <summary>
        /// This function is to enable all the needed features in service tool and in domain, role & user levels.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public void EnableGeneralFeatures()
        {
            bool blnUpdateDomainCheck = false;
            const string domainName = "SuperAdminGroup";
            String TransferserviceAETitle = "TFR_" + new BasePage().GetHostName(Config.IConnectIP).Replace("-", "");

            //Update Super Admin Group in Domain Management (Connect all DataSource and set Institution name)     
            login = new Login();
            login.DriverGoTo(login.url);
            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
            blnUpdateDomainCheck = login.UpdateGivenDomain(domainName);
            login.Logout();

            //UnComment LDAP directories from LDAP Config file
            servicetool.EnableLDAPConfigfile();

            //Enable Different Features
            servicetool.InvokeServiceTool();
            servicetool.SetEnableFeaturesGeneral();
            wpfobject.WaitTillLoad();
            servicetool.SetEnableFeaturesGeneral();
            wpfobject.WaitTillLoad();
            servicetool.ModifyEnableFeatures();
            wpfobject.WaitTillLoad();
            servicetool.EnablePatient();
            servicetool.EnableStudySharing();
            servicetool.EnableDataDownloader();
            servicetool.EnableDataTransfer();
            servicetool.EnableEmailStudy();
            servicetool.EnablePDFReport();
            servicetool.EnablePDFReport();
            servicetool.EnableRequisitionReport();
            servicetool.EnableSelfEnrollment();
            servicetool.EnableEmergencyAccess();
            servicetool.EnableBriefcase();
            servicetool.EnableConferenceLists();
            wpfobject.WaitTillLoad();
            servicetool.ApplyEnableFeatures();
            wpfobject.WaitTillLoad();
            wpfobject.ClickOkPopUp();
            wpfobject.WaitTillLoad();

            //Setup Email Notification
            servicetool.SetEmailNotificationForPOP();

            //Setup Transfer Service Config
            servicetool.SetTransferserviceAETitle(TransferserviceAETitle);

            //Enable encapsulated report
            //tool.EnableReports(true);

            //Enable Study attachments 
            servicetool.RestartService();

            //Prefetch settings
            servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
            servicetool.NavigateSubTab("Pre-fetch Cache Service");
            servicetool.ClickModifyButton();
            servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 5, timerange: 60, cleanupthreshold: 60, AEtitle: login.PrefetchAETitle);
            servicetool.RestartService();

            //Enable Prefetch cache - Datasource
            if (!String.IsNullOrEmpty(Config.DestEAsIp))
            {
            servicetool.EnableCacheForDataSource(login.GetHostName(Config.DestEAsIp));
            }
            servicetool.RestartService();

            //Enable LDAP setup
            /*servicetool.NavigateToConfigToolUserMgmtDatabaseTab();
            servicetool.SetMode(2);
            servicetool.LDAPSetup();
            servicetool.CloseServiceTool();*/

            if (!getiCAVersion().Contains("6.5"))
            {
                //Enable Bluring Viewer
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                    TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing", restart: true);
            }

            RestartIISUsingexe();

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
            if (!getiCAVersion().Contains("6.5"))
            {
                domainmanagement.SetCheckBoxInEditDomain("universalviewer", 0);
            }
            if (Environment.MachineName.ToLower().Equals("exe-ica3-ws12")) { domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0); }
            //domainmanagement.ConnectAllDataSources();
            domainmanagement.ModifyStudySearchFields();
            String[] availableTools = domainmanagement.GetToolsFromAvailableSection().Cast<String>().ToArray();
            domainmanagement.MoveToolsToToolbarSection(availableTools, 21);
            domainmanagement.ClickSaveEditDomain();

            //Create Different Role and uers
            rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
            if (!getiCAVersion().Contains("6.5"))
            {               
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("universalviewer", 0);
                rolemanagement.ClickSaveEditRole();
            }
            if (!rolemanagement.RoleExists("Staff"))
            {
            rolemanagement.CreateRole("SuperAdminGroup", "Staff", "staff");
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
            //login.UncommentXMLnode("id", "Bypass");

            //Create physician and archivist users
            usermanagement = (UserManagement)login.Navigate("UserManagement");
            usermanagement.CreateUser(Config.stUserName, "SuperAdminGroup", "Staff", 1, Config.emailid, 1, Config.stPassword);
            usermanagement.CreateUser(Config.phUserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.phPassword);
            usermanagement.CreateUser(Config.ph1UserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.ph1Password);
            usermanagement.CreateUser(Config.arUserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.arPassword);
            usermanagement.CreateUser(Config.ar1UserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.ar1Password);

            //Logout
            login.Logout();
          
        }

        /// <summary>
        /// This is to enable Integrator mode
        /// </summary>
        public void SetupIntegrator(String path, String mode = "install")
        {
            //Replacing the TestEHR Files

            int startindex  = path.IndexOf("Output_ICA");
            path = path.Substring(0, startindex);
            String EHRPath = path + "Output_ICA" + Path.DirectorySeparatorChar + "TestTools";
            String TestEHR_EXE_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.exe";
            String TestEHR_EXE_Config_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.exe.config";
            String TestEHR_pdp_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.pdb";
            String TestEHR_samlPolicy_config_Path = EHRPath + Path.DirectorySeparatorChar + "TestEHR.samlPolicy.config";
            String SystemFactoryConfiguration_Path = EHRPath + Path.DirectorySeparatorChar + "SystemFactoryConfiguration.xml";
            String ServiceFactoryConfiguration_Path = EHRPath + Path.DirectorySeparatorChar + "ServiceFactoryConfiguration.xml";
            String PostFormTemplate_html = EHRPath + Path.DirectorySeparatorChar + "PostFormTemplate.html";
            String Destination_Path = "C:\\WebAccess\\WebAccess\\bin\\";

            //Copy Files
            File.Copy(TestEHR_EXE_Path, Destination_Path + Path.GetFileName(TestEHR_EXE_Path), true);
            File.Copy(TestEHR_EXE_Config_Path, Destination_Path + Path.GetFileName(TestEHR_EXE_Config_Path), true);
            File.Copy(TestEHR_pdp_Path, Destination_Path + Path.GetFileName(TestEHR_pdp_Path), true);
            File.Copy(TestEHR_samlPolicy_config_Path, Destination_Path + Path.GetFileName(TestEHR_samlPolicy_config_Path), true);
            File.Copy(SystemFactoryConfiguration_Path, Destination_Path + Path.GetFileName(SystemFactoryConfiguration_Path), true);
            File.Copy(ServiceFactoryConfiguration_Path, Destination_Path + Path.GetFileName(ServiceFactoryConfiguration_Path), true);
            File.Copy(PostFormTemplate_html, Destination_Path + Path.GetFileName(PostFormTemplate_html), true);

            //Enable Bypass mode for Integrator mode
            if(!mode.Equals("upgrade"))
            login.UncommentXMLnode("id", "Bypass");

            servicetool.LaunchServiceTool();
            servicetool.RestartService();
            servicetool.CloseServiceTool();
        }

        /// <summary>
        /// This method is to setup XDS
        /// </summary>
        public void SetupXDS()
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
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.DataSource_Tab);
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
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.DataSource_Tab);
                servicetool.SelectDataSource(login.GetHostName(Config.EA1));
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.SetDataSourcePatientIDDomain("CLE", "Cleveland Clinic", "CLE&2.16.840.1.113883.9.186&ISO", DicomIPID: "IPID-CLE", TypeCode: "PI");
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Add XDS Datasource
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.DataSource_Tab);
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
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Add Patient ID domains and Add other identifiers
                servicetool.LaunchServiceTool();             
                servicetool.NavigateToTab(ServiceTool.DataSource_Tab);
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
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Add XDS Datasource
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.DataSource_Tab);
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
        }

        /// <summary>
        /// Setup RDM Data Source
        /// </summary>
        public void SetupRDM()
        {
            //Get RDM data source details from Config file
            Dictionary<string, string> RDM_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/RDMDataSources");

            // Call function MinimizeAll
            Taskbar taskbar = new Taskbar();
            taskbar.Hide();

            //Launch Service tool
            servicetool.LaunchServiceTool();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);

            //Adding RDM data sources
            foreach (String RDMIP in RDM_DataSources_IP.Keys)
            {
                if (!String.IsNullOrEmpty(RDM_DataSources_IP[RDMIP]))
                {
                    DistanceCounter++;
                    servicetool.AddRDMDatasource(RDM_DataSources_IP[RDMIP], "");
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

        }

        /// <summary>
        /// This method is to enable HTTPS
        /// </summary>
        public void EnableHTTPS()
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
            wpfobject.WaitTillLoad();
            Thread.Sleep(2000);
            try { WpfObjects._mainWindow.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByText("OK")).Click(); } catch (Exception) { }                    
            wpfobject.WaitTillLoad();
            servicetool.RestartIISandWindowsServices();
            Thread.Sleep(1000);
            servicetool.CloseConfigTool();
        }
            
        /// <summary>
        /// This method is to setup Image sharing
        /// </summary>
        public void SetupImageSharing()
        {

            const string domainName = "SuperAdminGroup";

            //Hide TaskBar
            var taskbar = new Taskbar();
            taskbar.Hide();

            //Generate Exam Importer and POP
            servicetool.InvokeServiceTool();
            servicetool.UpdateInstallerUrl();
            servicetool.GenerateInstallerPOP("SuperAdminGroup","");
            servicetool.GenerateInstallerAllDomain(domainName, Config.eiwindow);
            wpfobject.WaitTillLoad();
            servicetool.RestartService();
            servicetool.CloseServiceTool();

            //Show Taskbar
            taskbar.Show();

            //Login iConnect Access UI
            login.DriverGoTo(login.url);
            login.LoginIConnect(Config.adminUserName, Config.adminPassword);

            //Creating new users for Image Sharing setup
            usermanagement = (UserManagement)login.Navigate("UserManagement");
            usermanagement.CreateUser(Config.ph2UserName, "SuperAdminGroup", "Physician", 1, Config.emailid, 1, Config.ph2Password);
            usermanagement.CreateUser(Config.ar2UserName, "SuperAdminGroup", "Archivist", 1, Config.emailid, 1, Config.ar2Password);
            usermanagement.CreateUser(Config.newUserName, "SuperAdminGroup", "Staff", 1, Config.emailid, 1, Config.newPassword);

            //Create Instituitions
            SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
            settings.SetDateRange();
            settings.SaveSystemSettings();
            login.AddInstitution(Config.Inst1, Config.ipid1);            

            //Create Destinations
            login.AddDestination("SuperAdminGroup", Config.Dest1, login.GetHostName(Config.DestinationPACS), Config.ph1UserName, Config.ar1UserName);
            //login.CloseBrowser();

            login._examImporterInstance = Config.eiwindow;
            ei._examImporterInstance = login._examImporterInstance;                       

            //Delete existing installer file                
            login.DriverGoTo(login.url);
            try
            {
                File.Delete(login.InstallerPath + @"\Installer.UploaderTool.msi");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Problems in deleting the previous installer file due to : " + e);
            }

            //Download new installer file and Minimize all apps
            //login.DownloadInstaller(login.url, "CDUpload", login.InstallerPath + @"\Installer.UploaderTool.msi", "SuperAdminGroup");
            //Type typeShell = Type.GetTypeFromProgID("Shell.Application");
            //object objShell = Activator.CreateInstance(typeShell);
            //typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);

            //Uninstall App if already installed
            //if (ei.IsEiInstalled())
            //{
            //    ei.UnInstallEI();
            //}

            ei.EI_Installation("SuperAdminGroup", Config.eiwindow, Config.Inst1, Config.ph1UserName, Config.ph1Password);
                      

            //login.DriverGoTo(login.url);
            //try
            //{
            //    File.Delete(login.InstallerPath + @"\Installer.UploaderTool.msi");
            //}
            //catch (Exception e)
            //{
            //    Logger.Instance.ErrorLog("Problems in deleting the previous installer file due to : " + e);
            //}      

        }

        /// <summary>
        /// Update Exam Importer Details
        /// </summary>
        public void UpdateEI()
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
        }

        /// <summary>
        /// This method is to setup Localization
        /// </summary>
        public void SetupLocalization()
        {

        }

        /// <summary>
        /// This method is to setup password policy
        /// </summary>
        public void SetupPasswordPolicy()
        {
            String contactinfo = "Name:Administrator=Phonenumber:0000000000=EmailId:admin@aspiresys.com=ExtnNo:1234=Address:1/W Delhi=AltNumber:000000000";
            String[] info = contactinfo.Split('=');

            servicetool.LaunchServiceTool();
            servicetool.NavigateToTab(ServiceTool.UserManagement_Tab);   
            servicetool.NavigateToConfigToolSecurityTab();
            wpfobject.GetTabWpf(1).SelectTabPage(1);
            Thread.Sleep(2500);
            servicetool.ClickModifyFromTab();
            servicetool.SetPassWordPolicy(true);
            servicetool.UpdateAdminContact(info[0] + Environment.NewLine + info[1] + Environment.NewLine + info[2]);
            servicetool.ClickApplyButtonFromTab();
            servicetool.AcceptDialogWindow();
            servicetool.RestartService();
            servicetool.CloseServiceTool();
        }

        /// <summary>
        /// Method is to enable encapsulated report
        /// </summary>
        public void SetupEncapsulatedReports()
        {

            ServiceTool tool = new ServiceTool();
            tool.LaunchServiceTool();
            tool.NavigateToEnableFeatures();
            wpfobject.WaitTillLoad();
            tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
            wpfobject.WaitTillLoad();
            tool.ModifyEnableFeatures();
            WpfObjects._mainWindow.Get<TestStack.White.UIItems.CheckBox>(TestStack.White.UIItems.Finders.SearchCriteria.ByAutomationId(ServiceTool.EnableFeatures.ID.EncapsulatedPDF)).Checked = true;
            tool.ApplyEnableFeatures();
            wpfobject.WaitTillLoad();
            tool.RestartService();
            tool.CloseServiceTool();
        }

        /// <summary>
        /// Configure Language culture in iCA
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public void Setup_LocalizationSetup(String type = "install")
        {
            string dataSourceName = "DCM4CHEE";
            string specificCharacterSet = @",specificCharacterSet\";
            String ICA_MappingFilePath = Config.TestSuitePath + Path.DirectorySeparatorChar + Config.ica_Mappingfilepath;
            String BluringViewer_MappingFilePath = Config.BluringViewer_Mappingfilepath;

            try
            {
                //Fetch required Test data                
                string ZipPath = Config.zipPath;
                string ExtractPath = Config.extractpath;
                string defaultPath = Config.defaultpath;
                string commonpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "OtherFiles" + Path.DirectorySeparatorChar;
                string LocalizationPrepareFile = commonpath + "Localization_Prepare.bat";
                string LocalizationCompleteFile = commonpath + "Localization_Complete.bat";

                string TranslationEXEpath = commonpath + "TranslationTool.exe";
                string GlobalResourcePath = @"This PC\Local Disk (C:)\WebAccess\LocalizationSDK\" + Config.Locale;
                string PrepareOutputPath = ExtractPath + Path.DirectorySeparatorChar + "Prepareoutput.txt";
                string CompleteOutputPath = ExtractPath + Path.DirectorySeparatorChar + "Completeoutput.txt";

                String EIWix = ExtractPath + Path.DirectorySeparatorChar + Config.Locale + @"\UploaderTool_Resources\WixLocalization\Language_" + Config.Locale + ".wxl";
                String POPWix = ExtractPath + Path.DirectorySeparatorChar + Config.Locale + @"\PopConfigurationTool_Resources\WixLocalization\Language_" + Config.Locale + ".wxl";
                String EIBoot = ExtractPath + Path.DirectorySeparatorChar + Config.Locale + @"\UploaderTool_Resources\BootStrapperLocalization\Theme_" + Config.Locale + ".wxl";
                String POPBoot = ExtractPath + Path.DirectorySeparatorChar + Config.Locale + @"\PopConfigurationTool_Resources\BootStrapperLocalization\Theme_" + Config.Locale + ".wxl";
                String LCIDCode = "1041";

                String xmlFilePath = @"C:\WebAccess\WebAccess\web.Config";
                String NodePath = "configuration/appSettings/add";
                String FirstAttribute = "key";
                String AttValue = "Application.Culture";
                String SecondAttribute = "value";

                //Step 2 - Precondition
                bool UnzipFolder = false;
                if (!Directory.Exists(defaultPath))
                {
                    UnzipFolder = UnZipSDKFolder(ZipPath, ExtractPath, defaultPath);
                }
                else
                {
                    UnzipFolder = true;
                    Logger.Instance.InfoLog("Folder is already unzipped");
                }
                ServiceTool servicetool = new ServiceTool();
                bool Step1 = servicetool.Prepare_CompleteLocalization(Config.Locale, LocalizationPrepareFile, PrepareOutputPath);
                servicetool.Translation(TranslationEXEpath, GlobalResourcePath, Config.Locale.Split('-')[0], Config.Locale.Split('-')[1]);
                if (UnzipFolder && Step1 && File.Exists(EIWix)) { }

                //Step 3 - Precondition
                ChangeAttributeValue(EIBoot, "/WixLocalization", "Culture", Config.Locale, encoding: true); //Theme.wxl
                ChangeAttributeValue(EIBoot, "/WixLocalization", "Language", LCIDCode, encoding: true);
                ChangeAttributeValue(POPBoot, "/WixLocalization", "Culture", Config.Locale, encoding: true);
                ChangeAttributeValue(POPBoot, "/WixLocalization", "Language", LCIDCode, encoding: true);
                ChangeAttributeValue(EIWix, "/WixLocalization", "Culture", Config.Locale, encoding: true); //Language.wxl                
                ChangeAttributeValue(POPWix, "/WixLocalization", "Culture", Config.Locale, encoding: true);

                //Step 4 - Precondition               
                bool Step2 = servicetool.Prepare_CompleteLocalization(Config.Locale, LocalizationCompleteFile, CompleteOutputPath);
                String ExistingValue = GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);
                if (!ExistingValue.Contains(Config.Locale))
                {
                    SetWebConfigValue(xmlFilePath, AttValue, ExistingValue + "," + Config.Locale);
                }
                String NewValue = GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);

                //Add DCM4CHEE datasource
                if (!type.Equals("upgrade"))
                {
                    ServiceTool st = new ServiceTool();
                    st.LaunchServiceTool();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    st.AddDCMDataSource(dataSourceName, "Dicom", "10.4.39.48", dataSourceName, "11112");
                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.WaitTillLoad();
                    st.RestartIISandWindowsServices();
                    wpfobject.WaitTillLoad();
                    st.CloseConfigTool();

                    //Add setting in DCM4CHEE
                    String DSAttribute = GetNodeValue(Config.DSManagerFilePath, "/add[@id='" + dataSourceName + "" + "']/parameters/excludedAttributes");
                    String ExcludedAttribute = DSAttribute.Replace(specificCharacterSet, "");
                    ChangeNodeValue(Config.DSManagerFilePath, "/add[@id='" + dataSourceName + "" + "']/parameters/excludedAttributes", ExcludedAttribute);
                    servicetool.RestartIISUsingexe();

                    //Add the datasource to superadmingroup
                    login.DriverGoTo(url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                    domain.SearchDomain(Config.adminGroupName);
                    domain.SelectDomain(Config.adminGroupName);
                    domain.ClickEditDomain();
                    domain.ConnectAllDataSources();
                    domain.ClickSaveEditDomain();
                    login.Logout();
                }

                //Check if other language is configured
                login.DriverGoTo(url);
                login.PreferredLanguageSelectList().SelectByValue(Config.Locale);
                Thread.Sleep(5000);
                bool UIValidate4 = ValidateLocalization(ICA_MappingFilePath, "LoginPage");
                if (Step2 && UIValidate4 && NewValue.Contains(Config.Locale)) { }                

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
            }
        }

        /// <summary>
        /// This method is to setup Encryption in service tool
        /// </summary>
        public void Setup_Encryption()
        {
            servicetool.LaunchServiceTool();
            wpfobject.SelectTabFromTabItems(ServiceTool.Encryption.Name.Encryption_tab);
            servicetool.EnterServiceEntry();
            servicetool.EnterServiceParameters("key", "string", "");
            servicetool.EnterServiceParameters("iv", "string", "");
            servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
            servicetool.EnterServiceParameters("operationMode", "string", "CBC");
            servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.GetButton("OK", 1).Click();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            wpfobject.WaitTillLoad();
            servicetool.RestartIISandWindowsServices();
            wpfobject.WaitTillLoad();
            string[] Keys = servicetool.GenerateEncryptionKeys("mergehealthcare");
            servicetool.EditServiceParameters("Key", "TripleDES", "Name", "key", Value: Keys[0]);
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.GetButton("OK", 1).Click();

            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            wpfobject.SelectTabFromTabItems(ServiceTool.Encryption.Name.Encryption_tab);
            servicetool.EnterServiceEntry("TripleDES-A");
            servicetool.EnterServiceParameters("key", "string", "");
            servicetool.EnterServiceParameters("iv", "string", "");
            servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
            servicetool.EnterServiceParameters("operationMode", "string", "CBC");
            servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.GetButton("OK", 1).Click();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            wpfobject.WaitTillLoad();
            servicetool.RestartIISandWindowsServices();
            wpfobject.WaitTillLoad();
            Keys = servicetool.GenerateEncryptionKeys("cedaracare");
            servicetool.EditServiceParameters("Key", "TripleDES-A", "Name", "key", Value: Keys[0]);
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.GetButton("OK", 1).Click();

            servicetool.EnterEncryptionProviders("ID-456", "args", "Cryptographic.TripleDES-A");
            servicetool.ClickApplyButtonFromTab();
            servicetool.AcceptDialogWindow();
            servicetool.EnterEncryptionProviders("ID-123", "args", "Cryptographic.TripleDES");
            servicetool.ClickApplyButtonFromTab();
            servicetool.AcceptDialogWindow();
            servicetool.RestartService();
            servicetool.CloseServiceTool();

        }

        /// <summary>
        /// This method will Downaload the required GA build
        /// </summary>
        /// <param name="release"></param>
        /// <param name="downloadpath"></param>
        public void DownloadICABuild(String release, String downloadpath)
        {

            downloadpath = downloadpath.Split(new string[] { "archive" }, StringSplitOptions.None)[0];

            //Deleting existing files and  folders
            new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
            {
                if (file.IndexOf("archive", StringComparison.OrdinalIgnoreCase) >= 0)
                    File.Delete(file);
            });

            //Dowload File
            if (release.Contains("6.5.1"))
            {
                BasePage.Driver.Navigate().GoToUrl("http://ica-build:8080/job/iCA_6.5.1_Python/1544/artifact/*zip*/archive.zip");
            }
            else if (release.Contains("6.5"))
            {
                BasePage.Driver.Navigate().GoToUrl("http://10.4.13.86:8080/job/iCA_CURR_python/1532/artifact/*zip*/archive.zip");
            }
            else if (release.Contains("7.0"))
            {
                BasePage.Driver.Navigate().GoToUrl("https://ica-build-w2016.products.network.internal:8443/job/iCA_Python/1626/artifact/*zip*/archive.zip");
            }
            else if (release.Contains("7.1"))
            {
                BasePage.Driver.Navigate().GoToUrl("https://ica-build-w2016.products.network.internal:8443/job/iCA_Python/1946/artifact/*zip*/archive.zip");
            } 
            else
            {
                BasePage.Driver.Navigate().GoToUrl("https://ica-build-w2016.products.network.internal:8443/job/iCA_Python/lastSuccessfulBuild/artifact/*zip*/archive.zip");
            }

            //Wait Till Download
            Boolean installerdownloaded = BasePage.CheckFile("archive", Config.downloadpath, "zip");
            int counter = 0;
            while (!installerdownloaded && counter++ < 20)
            {
                PageLoadWait.WaitForDownload("archive", Config.downloadpath, "zip", 130);
                installerdownloaded = BasePage.CheckFile("archive", Config.downloadpath, "zip");                
                Thread.Sleep(1000);
            }
            
            //Deleting existing files and folders and unzip files here
            new List<string>(Directory.EnumerateDirectories(downloadpath)).ForEach(directory =>
            {
                Directory.Delete(directory, true);
            });
            System.IO.Compression.ZipFile.ExtractToDirectory(Config.downloadpath + Path.DirectorySeparatorChar + "archive.zip",
                downloadpath);
        }

        /// <summary>
        /// Configure Expernal application
        /// </summary>
        public void Setup_ExternalApplication()
        {
            //HALO
            String haloIP = Config.HaloIp;
            String haloPort = Config.HaloPort;

            //Hide TaskBar
            var taskbar = new Taskbar();
            taskbar.Hide();

            //Setting Encryption for HALO PACS
            servicetool = new ServiceTool();
            servicetool.LaunchServiceTool();
            servicetool.NavigateToEncryption();
            servicetool.SetEncryptionEncryptionService();
            servicetool.Add().Click();
            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.MoveWindowToDesktopTop("Service Entry Form");
            servicetool.key_txt().Text = "HALO.RC4";
            servicetool.assembly_txt().Text = "OpenContent.Generic.Core.dll";
            servicetool.class_txt().Text = "OpenContent.Core.Security.Services.HaloCryptographyWrapper";
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("HALO Encryption service entry started");
            wpfobject.GetButton("Apply", 1).Click();
            servicetool.EnterServiceParameters("password", "string", "amicas");
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.GetButton("OK", 1).Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("HALO Encryption service entry completed");

            //Add External application HALO
            servicetool.NavigateToExternalApplication();
            wpfobject.ClickButton("Add", 1);
            wpfobject.WaitTillLoad();
            wpfobject.MoveWindowToDesktopTop("External Application URL Configuration");
            servicetool.EnterExternalApplicationSettingsParameters(Config.HaloId, Config.HaloName, haloIP, haloPort, false, "servlet/com.amicas.servlet.integration.EmbeddedGateway", "Browser", "unixTicks,milliseconds,0", false);
            servicetool.EnterExternalApplicationEncryptionParameters("auth", "Cryptographic.HALO.RC4");
            servicetool.EnterExternalApplicationUrlParameterEntryForm("TS", "Dynamic", "Application.TimeStamp", true);
            servicetool.EnterExternalApplicationUrlParameterEntryForm("SID", "Dynamic", "Study.UID", true);
            servicetool.EnterExternalApplicationUrlParameterEntryForm("LOGIN", "Dynamic", "Application.User.Id", true);
            wpfobject.GetMainWindowByTitle("External Application URL Configuration");
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton("OK", 1);
            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            servicetool.RestartIISandWindowsServices();
            wpfobject.WaitTillLoad();
            servicetool.CloseServiceTool();

            RestartIISUsingexe();

            //Show Taskbar
            taskbar.Show();

            //Add external application to be visible in role management
            login.DriverGoTo(login.url);
            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
            RoleManagement roleManagement = (RoleManagement)login.Navigate("RoleManagement");
            roleManagement.SelectDomainfromDropDown(Config.adminGroupName);
            PageLoadWait.WaitForFrameLoad(10);
            roleManagement.SearchRole("SuperRole");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            roleManagement.EditRoleByName("SuperRole");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame(0);
            bool saveNeeded = false;
            if (roleManagement.List_ConnectExternalApplications().Count != 0)
            {
                roleManagement.ConnectExternalApplications();
                saveNeeded = true;
            }
            if (saveNeeded)
                roleManagement.ClickSaveEditRole();
            else
                roleManagement.CloseRoleManagement();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            Logger.Instance.InfoLog("External Applications are connected in Role Management");
            login.Logout();

            //Hide TaskBar
            taskbar = new Taskbar();
            taskbar.Hide();

            //Add HALO as datasource in Service tool
            servicetool.LaunchServiceTool();
            if (!servicetool.IsDataSourceExists(GetHostName(Config.HaloIp)))
            { servicetool.AddPacsDatasource(Config.HaloIp, GetHostName(Config.HaloIp), "12", Config.HaloUser, Config.HaloPass); }          
            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            servicetool.RestartService();
            servicetool.CloseServiceTool();
            Logger.Instance.InfoLog("External Applications data sources are added in service tool");

            //Add Exception Site For JavaSecurity
            FileUtils.AddExceptionSiteForJavaSecurity("http://" + Config.HaloIp);
            RestartIISUsingexe();

            //Show Taskbar
            taskbar.Show();

            //Connect Datasources in Domain Management 
            login.DriverGoTo(login.url);
            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
            DomainManagement domainManagement = (DomainManagement)login.Navigate("DomainManagement");
            domainManagement.SearchDomain(Config.adminGroupName);
            PageLoadWait.WaitForFrameLoad(20);
            domainManagement.SelectDomain(Config.adminGroupName);
            domainManagement.ClickEditDomain();
            domainManagement.ConnectAllDataSources();
            PageLoadWait.WaitForFrameLoad(20);
            string receivingInst = domainManagement.ReceivingInstTxtBox().GetAttribute("value");
            if (receivingInst == "")
                domainManagement.ReceivingInstTxtBox().SendKeys("Institutuon");
            PageLoadWait.WaitForFrameLoad(20);
            domainManagement.ClickSaveEditDomain();
            PageLoadWait.WaitForPageLoad(30);
            PageLoadWait.WaitForFrameLoad(20);
            Logger.Instance.InfoLog("External Applications data sources are connected in domain management");
            login.Logout();

        }       

        /// <summary>
        /// This method will setup Remote Rendering
        /// </summary>
        public void SetupRemoteRendering()
        {
            try
            {
                servicetool.LaunchServiceTool();
                servicetool.WaitWhileBusy();
                servicetool.NavigateToViewerTab();
                servicetool.WaitWhileBusy();
                servicetool.NavigateSubTab("Viewer Service");
                servicetool.WaitWhileBusy();
                servicetool.EnableMonitoringViewerService("10.9.39.181", "120", true);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
            }

            catch (Exception e) { Logger.Instance.InfoLog("Exception in Settingup Remote rendering" + e.Message); }
        }

        /// <summary>
        /// Creates the new Domain, Role and Users.
        /// </summary>
        public void SetupDomainRoleUsers()
        {
            try
            {   
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String domainA = String.Empty;
                String domainadminA = String.Empty;
                String passwordA = String.Empty;
                String roleA = "RoleA" + random.Next(1, limit);
                String userA = "UserA" + random.Next(1, limit);

                //Create DomainA, RoleA and User A
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domain = login.Navigate<DomainManagement>();
                var domainattr = domain.CreateDomainAttr();
                domainA = domainattr[DomainManagement.DomainAttr.DomainName];
                domainadminA = domainattr[DomainManagement.DomainAttr.UserID];
                passwordA = domainattr[DomainManagement.DomainAttr.Password];
                domain.CreateDomain(domainattr, isconferenceneeded: true);
               
                //Create Role with Conference enabled
                var rolemgmt = login.Navigate<RoleManagement>();
                rolemgmt.CreateRole(domainA, roleA, "physician");

                //Create UserA of RoleA
                var usermgmt = login.Navigate<UserManagement>();
                usermgmt.CreateUser(userA, domainA, roleA, 1, Config.emailid, 1, userA);                
            }

            catch (Exception e) { Logger.Instance.InfoLog("Exception in setting up users" + e.Message); }
        }
        
        //Helper Methods - User32--Dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetWindowText(IntPtr hwnd, StringBuilder lpString, long cch);

    }

}