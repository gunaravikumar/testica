using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using Microsoft.Win32;
using Selenium.Scripts.Reusable.Generic;
using System.Diagnostics;
using System.Threading;
using TestStack.White.UIItems;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
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
using System.Windows.Automation;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White;

namespace Selenium.Scripts.Pages
{
     class POPUploader : BasePage
    {
        public String pacswinName { get; set; }
        //public WpfObjects wpfobject { get; set; }
        //Constrcutor
        public POPUploader()
        {
            wpfobject = new WpfObjects();
            pacswinName = Config.pacswindow;          
        }

        public List<string> AETitlesExposed = new List<string>();
        public List<string> AETitlesNonExposed = new List<string>();
        public String PACSGatewayInstallerName = "Installer.Pop.msi";
        //public String PACSGatewayInstallerName2 = "Installer.Pop(1).msi";
        public String PACSGatewayWelcomeMessage = "125";//automation ID 
        public String PACSGatewaySetupWizard = "11";//automation ID 
        public String PACSGatewayNTP = "725";//automation ID 
        public String PACSGatewaySetupTitleBar = "TitleBar";//automation ID 
        public String PACSGatewaySetupNext = "685";//automation ID 
        public String PACSGatewaySetupEULA = "125";//automation ID 
        public String PACSGatewaySetupEULAChckbx = "728";//automation ID 
        public String PACSGatewaySetupRgstrtionPIN = "681";//automation ID 
        public String PACSGatewaySetupSCPPort = "scpPortTextBox";//automation ID 
        public String PACSGatewaySetupEmail = "emailTextBox";//automation ID 
        public String PACSGatewaySetupSelectAllDest = "checkAllCheckBox";//automation ID 
        public String PACSGatewaySetupExposedDest = "Exposed Row ";//name -- concate name with checkbox row number 
        public String PACSGatewaySetupDestFoldr = "697";//automation ID 
        public String PACSGatewaySetupInstall = "741";//automation ID 
        public String PACSGatewaySetupInstallCmplt = "125";//automation ID 
        public String PACSGatewaySetupFinish = "655";//automation ID 
        public String PacsGatewaySetupWndwName = new BasePage().PacsGatewayInstance + " Setup";
        public String PacsGatewaySetupWndwName2 = new BasePage().PacsGatewayInstance2 + " Setup";
        public String PACSGatewaySetupPrint = "732";//automation ID -------//727 Old
        public String PACSGatewaySetupBack = "658";//automation ID ----------//653 Old
        public String PACSGatewaySetupCancelt = "647";//automation ID -------//642

        public String PACSGatewayPort = "textBoxPort";//automation ID 
        public String PACSGatewayRetry = "textBoxRetryNumber";//automation ID 
        public String PACSGatewayRetryIntrvl = "textBoxRetryInterval";//automation ID 
        public String PACSGatewayChunkSize = "textBoxChunkSize";//automation ID 
        public String PACSGatewayDelayBfrStudy = "textBoxDelayStudyComplete";//automation ID 
        public String PACSGatewayMaxIn = "textBoxMaxIncomingConnections";//automation ID 
        public String PACSGatewayMaxOut = "textBoxMaxOutgoingConnections";//automation ID 
        public String GeneralSettingsTab = "General Settings";//name 
        public String SystemSettingsTab = "System Settings";//name 
        public String ActiveTransfersTab = "Active Transfers";//name 
        public String TransferHistoryTab = "Transfer History";//name 
        public String EmailNotificationTab = "Email Notification";//name 
        public String PACSGatewayLocalCachePath = "textBoxLocalCachePath";//automation ID 
        public String PACSGatewayLocalCachePathChange = "buttonSelectFolder";//automation ID 
        public String PACSGatewayiCAURL = "textBoxAdminServiceURL";//automation ID 
        public String PACSGatewayJobs = "textBoxNoOfJobsThresholdForRollingLog";//automation ID 
        public String HereLogLink = "Click here to view logs";//name 
        public String PACSGatewaySave = "buttonSave";//automation ID 
        public String PACSGatewayCanceltransfer = "buttonCancel";//automation ID 
        public String PACSGatewayRefreshtransfer = "buttonRefresh";//automation ID 
        public String PACSGatewayAdminEmail = "textBoxEmail";//automation ID 
        public String PACSGatewayReset = "buttonReset";//automation ID 
        public String PACSGatewaySearch = "buttonSearch";//automation ID 


        public String PACSGatewayAnonymizeEmail = "AnonymizeEmailChk";//automation ID 
        public String PACSGatewayServerHost = "textBoxServerHostEmail";//automation ID 
        public String PACSGatewaySMTPPort = "textBoxPortEmail";//automation ID 
        public String PACSGatewaySSL = "SslChk";//automation ID 
        public String PACSGatewaySMTPUsername = "textBoxUserNameEmail";//automation ID 
        public String PACSGatewaySMTPPassword = "textBoxPasswordEmail";//automation ID 
        public String PACSGatewaySendTestEmail = "buttonTestEmail";//automation ID 
        public String PACSGatewayDeviceDeactivated = "DeviceChk";//automation ID 
        public String PACSGatewaySCPModified = "ScpChk";//automation ID 
        public String PACSGatewayAETitleModified = "AetitleChk";//automation ID 
        public String PACSGatewayDestModified = "DestinationChk";//automation ID 
        public String PACSGatewayHostSettingUpdated = "HostSettingsChk";//automation ID 
        public String PACSGatewayStudyFails = "StudyFailedChk";//automation ID 
        public String PACSGatewayDuplicateStudy = "DuplicateStudyUploadedChk";//automation ID                                                                               
        public String PACSConfigTool = "PACS Gateway Configuration";
        public string pinlabeltext = string.Empty;
        //UI Objects
        public Tab GeneralSettings_Tab() { return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.ByText("General Settings")); }
        public Tab SystemSettings_Tab() { return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.ByText("System Settings")); }
        public Tab ActiveTransfers_Tab() { return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.ByText("Active Transfers")); }
        public Tab TransferHistory_Tab() { return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.ByText("Transfer History")); }
        public Tab EmailNotification_Tab() { return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.ByText("Email Notification")); }

        //General Settings tab
        public TextBox PACSGatewayPort_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxPort")); }
        public TextBox PACSGatewayRetry_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxRetryNumber")); }
        public TextBox PACSGatewayRetryIntrvl_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxRetryInterval")); }
        public TextBox PACSGatewayChunkSize_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxChunkSize")); }
        public TextBox PACSGatewayDelayBfrStudy_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxDelayStudyComplete")); }
        public TextBox PACSGatewayMaxIn_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxMaxIncomingConnections")); }
        public TextBox PACSGatewayMaxOut_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxMaxOutgoingConnections")); }

        //System settings tab
        public TextBox PACSGatewayLocalCachePath_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxLocalCachePath")); }
        public TextBox PACSGatewayiCAURL_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxAdminServiceURL")); }
        public TextBox PACSGatewayJobs_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("textBoxNoOfJobsThresholdForRollingLog")); }
        public Button BtnSave() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("buttonSave")); }

        //Active Transfer tab
        public Button PACSGatewayCanceltransfer_Btn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("buttonCancel")); }
        public Button PACSGatewayRefreshtransfer_Btn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("buttonRefresh")); }

        //Transfer history tab
        public Button PACSGatewayReset_Btn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("buttonReset")); }
        public Button PACSGatewaySearch_Btn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("buttonSearch")); }
        public TextBox TxtAccession_TxtBx() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("txtAccessionNo")); }
        public ListView datagrid() { return wpfobject.GetUIItem<ITabPage, ListView>(this.GetCurrentTabItem(), "dataGridPopService"); }
        public ListView HisDataGrid() { return wpfobject.GetUIItem<ITabPage, ListView>(this.GetCurrentTabItem(), "dataGridHistoricalTransfer"); }

        //Email Settings Tab
        public CheckBox sslCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewaySSL)); }
        public CheckBox anonymizeEmailCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewayAnonymizeEmail)); }
        public Label testEmailResponseTxt() { return WpfObjects._mainWindow.ModalWindow("MessageBox").Get<Label>(SearchCriteria.ByAutomationId("MessageText")); }

        //Notifications checkboxes
        public CheckBox deviceDeactivatedCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewayDeviceDeactivated)); }
        public CheckBox scpModifiedCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewaySCPModified)); }
        public CheckBox aeTitleModifyCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewayAETitleModified)); }
        public CheckBox destinationModiyCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewayDestModified)); }
        public CheckBox hostSettingsCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewayHostSettingUpdated)); }
        public CheckBox studyFailsCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewayStudyFails)); }
        public CheckBox duplicateStudyCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewayDuplicateStudy)); }

        //Taskbar
        public Button BtnMinimize() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("Minimize")); }
        public Button BtnMaximize() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("Maximize")); }

        public void SMTP12Server()
        {
            POPUploader popu = new POPUploader();
            String o = popu.PACSGatewayNTP;
        }     
                
        public bool IsPACSGatewayInstalled()
        {
            if (GetPACSGatewayInstalledPath().Equals(String.Empty))
            {
                return false;
            }
            return true;
        }

        public ITabPage GetCurrentTabItem()
        {
            return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
        }

        public string GetPACSGatewayInstalledPath()
        {
            string installedPath = string.Empty;
            try
            {
                RegistryKey localMachine = Registry.LocalMachine;
                RegistryKey fileKey =
                    localMachine.OpenSubKey(@"Software\Merge HeathCare\" + PacsGatewayInstance + @"\") ??
                    localMachine.OpenSubKey(@"Software\Merge HealthCare\" + PacsGatewayInstance + @"\");

                object result = null;

                if (fileKey != null)
                {
                    result = fileKey.GetValue("InstallDir");
                }

                if (fileKey != null) fileKey.Close();

                installedPath = (string)result;

                Logger.Instance.ErrorLog("Installed path is : " + installedPath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step GetPACSGatewayInstalledPath due to : " + ex);
            }

            return installedPath ?? string.Empty;
        }

        public void InstallPACSGateway()
        {
            try
            {
                //msiexec -i Installer.UploaderTool.msi -quiet
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-i D:\Installers\Installer.Pop.msi -quiet /L*v 'log.log'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                proc.Start();
                Thread.Sleep(2000);

                int i = 0;

                while (i < 30 && !proc.HasExited)
                {
                    Thread.Sleep(4000);
                    i++;
                }
                Logger.Instance.InfoLog("PACS Gateway installed succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while installing PACS Gateway due to :" + ex);
            }
        }

        public void UnInstallPACSGateway(String InstallerName = " ")
        {
            try
            {
                if (InstallerName.Equals(" "))
                {
                    InstallerName = PACSGatewayInstallerName;
                }
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-x " + InstallerName + " -quiet /L*v 'log.log'",
                        WorkingDirectory = InstallerPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                proc.Start();
                Thread.Sleep(2000);

                int i = 0;

                while (i < 30 && !proc.HasExited)
                {
                    Thread.Sleep(4000);
                    i++;
                }

                Logger.Instance.InfoLog("PACS Gateway uninstalled succesfully");
            }

            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while uninstalling PACS Gateway due to :" + ex);
            }
        }

        public void POP_WaitTillInstallationFinishes(String GateWayInstanceName = "")
        {
            if (GateWayInstanceName == null || GateWayInstanceName == "")
                GateWayInstanceName = PacsGatewayInstance;

            Window window = wpfobject.GetMainWindowByTitle(GateWayInstanceName + " Setup");
            try
            {
                Button buttonNext = wpfobject.GetButton("Finish", 1);

                int installWindowTimeOut = 0;
                while (buttonNext == null && installWindowTimeOut < 40)
                {
                    Thread.Sleep(5000);
                    if (window == null || window.IsClosed)
                        window = wpfobject.GetMainWindowByTitle(GateWayInstanceName + " Setup");

                    buttonNext = wpfobject.GetButton("Finish", 1);
                    installWindowTimeOut++;
                }
                Logger.Instance.InfoLog("Installation finished in " +
                                        (installWindowTimeOut * 5).ToString(CultureInfo.InvariantCulture) + " seconds");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step POP_WaitTillInstallation finishes due to : " + ex);
            }
        }

        public void POP_WaitTillRetryOccur(String GateWayInstanceName = "")
        {
            if (GateWayInstanceName == null || GateWayInstanceName == "")
                GateWayInstanceName = PacsGatewayInstance;

            wpfobject.GetMainWindowByTitle(GateWayInstanceName + " Setup");
            try
            {
                Button buttonRetry = wpfobject.GetButton("Retry", 1);

                int RetryWindowTimeOut = 0;
                while (buttonRetry == null && RetryWindowTimeOut < 40)
                {
                    Thread.Sleep(5000);
                    buttonRetry = wpfobject.GetButton("Retry", 1);
                    RetryWindowTimeOut++;
                }
                Logger.Instance.InfoLog("Installation finished in " +
                                        (RetryWindowTimeOut * 5).ToString(CultureInfo.InvariantCulture) + " seconds");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step POP_WaitTillRetryOccur finishes due to : " + ex);
            }
        }

        public string GetDeviceIdFromLog(string folderPath)
        {
            try
            {
                string[] readData = File.ReadAllLines(GetLatestEiLog(folderPath));
                string deviceId = string.Empty;

                foreach (string t in readData)
                {
                    if (t.StartsWith("Device Id:"))
                    {
                        deviceId = t;
                        break;
                    }
                }

                Logger.Instance.InfoLog("Device Id found : " + deviceId.Split(':')[1].Trim());
                return deviceId.Split(':')[1].Trim();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step GetDeviceIdFromLog due to :  " + ex);
                return string.Empty;
            }
        }

        public void ReadPacsGatewayConfigForAETitles()
        {
            try
            {
                AETitlesExposed.Clear();
                AETitlesExposed.TrimExcess();
                AETitlesNonExposed.Clear();
                AETitlesNonExposed.TrimExcess();
                string name = string.Empty;
                XDocument doc = XDocument.Load(GetPACSGatewayInstalledPath() + @"Config\PacsGatewayConfiguration.xml");
                IEnumerable<XElement> m = doc.Descendants();

                foreach (XElement xElement in m)
                {
                    if (xElement.Name.ToString().Equals("AETitle", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name = xElement.Value;
                        Logger.Instance.InfoLog("name : " + name);
                        if (
                            (xElement.NextNode.ToString()
                                     .Replace("<IsDestinationExposed>", "")
                                     .Replace("</IsDestinationExposed>", "")).Trim()
                                                                             .Equals("true",
                                                                                     StringComparison
                                                                                         .InvariantCultureIgnoreCase))
                        {
                            AETitlesExposed.Add(name);
                        }
                        else
                        {
                            AETitlesNonExposed.Add(name);
                        }
                        Logger.Instance.InfoLog("added name : " + name);
                    }
                }
                Logger.Instance.InfoLog(AETitlesExposed.Count.ToString());

                //doc.Save(@"C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetWebAccessConfigValue due to : " + ex.Message);
            }
        }

        private static string GetLatestEiLog(string folderPath)
        {
            try
            {
                string[] fileNames = Directory.GetFiles(folderPath);

                string logFileName = string.Empty;

                var logFileNames = new string[10];

                int j = 0;
                foreach (string t in fileNames)
                {
                    if (t.Contains("UploaderTool"))
                    {
                        logFileNames[j] = t.Replace("-", "");
                        logFileNames[j] = logFileNames[j].Replace(folderPath + "\\UploaderTool", "");
                        logFileNames[j] = logFileNames[j].Replace(@".log", "");
                        j++;
                    }
                }

                Array.Sort(logFileNames);

                foreach (string t in fileNames)
                {
                    if (t.Replace("-", "").Contains(logFileNames[logFileNames.Length - 1]))
                    {
                        logFileName = t;
                        break;
                    }
                }
                Logger.Instance.InfoLog("Latest log found at : " + logFileName);
                return logFileName;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step GetLatestEiLog due to :  " + ex);
                return string.Empty;
            }
        }

        public void SetEmailForPOP(String SMTPHost = "mail.products.network.internal")
        {
            try
            {
                wpfobject.SelectTabFromTabItems(EmailNotificationTab);
                Thread.Sleep(2500);
                wpfobject.SetText(PACSGatewayServerHost, SMTPHost);
                wpfobject.SetText(PACSGatewaySMTPPort, "25");
                wpfobject.ClickButton(PACSGatewaySave);
                wpfobject.ClickButton("okButton");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceHoldingPen due to : " + ex);
            }
        }

        public void SetEmailForPOP(String emailId, String host, String port, String userName, String password, bool ssl = true, bool anonymizeEmail = true, String PACSWinName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(PACSWinName)) { wpfobject.GetMainWindowByTitle(Config.pacswindow); }
                else { wpfobject.GetMainWindowByTitle(PACSWinName); }

                wpfobject.SelectTabFromTabItems(EmailNotificationTab);
                wpfobject.WaitTillLoad();
                wpfobject.SetText(PACSGatewayAdminEmail, emailId);
                if (anonymizeEmail ^ anonymizeEmailCheckbox().Checked)
                    anonymizeEmailCheckbox().Click();                
                wpfobject.SetText(PACSGatewayServerHost, host);
                wpfobject.SetText(PACSGatewaySMTPPort, port);
                if (ssl ^ sslCheckbox().Checked)                
                    sslCheckbox().Click();                
                wpfobject.SetText(PACSGatewaySMTPUsername, userName);
                wpfobject.SetText(PACSGatewaySMTPPassword, password);
                wpfobject.ClickButton(PACSGatewaySave);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("okButton");
                wpfobject.WaitTillLoad();
                Logger.Instance.InfoLog("SMTP Details set successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while setting SMTP details: " + ex + ex);
            }
        }

        public void SelectEmailNotifications(String checkboxes = "", bool selectFlag=true, bool allCheckboxes = false, String PACSWinName = "")
        {           
            if (String.IsNullOrEmpty(PACSWinName)) { wpfobject.GetMainWindowByTitle(Config.pacswindow); }
            else { wpfobject.GetMainWindowByTitle(PACSWinName); }

            String[] arrCheckboxes = checkboxes.Split(':');
            if (allCheckboxes)
                arrCheckboxes = new String[] { "DeviceDeactivate", "SCPPortModified", "AETitleModified", "DestinationModified", "HostUpdated", "StudyFail", "DuplicateUpload" };                    

            wpfobject.SelectTabFromTabItems(EmailNotificationTab);
            wpfobject.WaitTillLoad();

            for (int counterI=0; counterI < arrCheckboxes.Count(); counterI++ )
            {
                switch (arrCheckboxes[counterI])
                {
                    case "DeviceDeactivate":
                        if (selectFlag ^ deviceDeactivatedCheckbox().Checked)
                            deviceDeactivatedCheckbox().Click();
                        break;
                    case "SCPPortModified":
                        if (selectFlag ^ scpModifiedCheckbox().Checked)
                            scpModifiedCheckbox().Click();
                        break;
                    case "AETitleModified":
                        if (selectFlag ^ aeTitleModifyCheckbox().Checked)
                            aeTitleModifyCheckbox().Click();
                        break;
                    case "DestinationModified":
                        if (selectFlag ^ destinationModiyCheckbox().Checked)
                            destinationModiyCheckbox().Click();
                        break;
                    case "HostUpdated":
                        if (selectFlag ^ hostSettingsCheckbox().Checked)
                            hostSettingsCheckbox().Click();
                        break;
                    case "StudyFail":
                        if (selectFlag ^ studyFailsCheckbox().Checked)
                            studyFailsCheckbox().Click();
                        break;
                    case "DuplicateUpload":
                        if (selectFlag ^ duplicateStudyCheckbox().Checked)
                            duplicateStudyCheckbox().Click();
                        break;
                }
            }
            wpfobject.ClickButton(PACSGatewaySave);
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton("okButton");
            wpfobject.WaitTillLoad();                      
            Logger.Instance.InfoLog("Email Notifications selection/unselection done" );                     
        }

        public bool SendTestEmail(String PACSWinName = "")
        {
            if (String.IsNullOrEmpty(PACSWinName)) { wpfobject.GetMainWindowByTitle(Config.pacswindow); }
            else { wpfobject.GetMainWindowByTitle(PACSWinName); }

            wpfobject.SelectTabFromTabItems(EmailNotificationTab);
            wpfobject.WaitTillLoad();
            //Click test email
            wpfobject.ClickButton(PACSGatewaySendTestEmail);
            wpfobject.WaitTillLoad();
            wpfobject.WaitForPopUp();
            //Check response
            String response = testEmailResponseTxt().Text;
            Logger.Instance.InfoLog("Test email button clicked. Response is " + response);
            wpfobject.ClickButton("okButton");
            wpfobject.WaitTillLoad();
            if (response.ToLower().Contains("mail send successfully"))
                return true;
            else
                return false;            
        }

        public string GetPOPDeviceID(string path)
        {
            string[] deviceID = null;
            string AE = "";
            wpfobject.InvokeApplication(path);
            Thread.Sleep(50000);
            wpfobject.GetMainWindowByTitle("PACS Gateway Configuration");
            var destlist = WpfObjects._mainWindow.Get<ListView>(SearchCriteria.Indexed(0));
            if (destlist != null)
            {
                var cell = destlist.Rows[0].Cells[1];
                if (cell != null)
                    AE = cell.Name;
                deviceID = AE.Split('_');
            }
            wpfobject.GetMainWindowByIndex(0);
            wpfobject.GetMainWindowByTitle("PACS Gateway Configuration");
            wpfobject.CloseWindow();
            return AE;
        }

        public void refreshTransfers(String path = "", int device = 1)
        {
            if (device == 2)
            {
                path = Config.PACSFilePath;
                pacswinName = Config.pacswindow;
            }
            else
            {
                path = Config.PACSFilePath;
                pacswinName = Config.pacswindow;

            }
            String tabname = "Active Transfers";
            //Launch PACS
            this.LaunchPACS();

            //navigate to Active Transfers
            this.navigateTab(tabname);

            //click refresh transfers
            if (PACSGatewayRefreshtransfer_Btn().Enabled)
            {
                for (int i = 0; i < 5; i++)
                {
                    PACSGatewayRefreshtransfer_Btn().Click();
                    Logger.Instance.InfoLog("Refresh Transfer button is clicked");
                    Thread.Sleep(1000);
                }
            }
        }

        public void LaunchPACS(String path = "", String PACSWinName = "")
        {
            BasePage.KillProcessByPartialName("PACS Gateway");
            if (String.IsNullOrEmpty(PACSWinName))
            { PACSWinName = pacswinName; }
            if (String.IsNullOrEmpty(path))
            { wpfobject.InvokeApplication(Config.PACSFilePath); }
            else { wpfobject.InvokeApplication(path); }

            //To handle Job service transfer Error
            try
            {
                int counterI = 0;
                while (!wpfobject.VerifyWindowExist(PACSWinName) && counterI < 10)
                {
                    Thread.Sleep(1000);
                    counterI++;
                }
                wpfobject.GetMainWindowByTitle(PACSWinName);
                wpfobject.WaitForPopUp();
                wpfobject.GetButton("okButton").Click();
                wpfobject.WaitTillLoad();
            }
            catch (Exception) { }       
        }

        public void navigateTab(String tab, String PACSWinName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(PACSWinName)) { wpfobject.GetMainWindowByTitle(pacswinName); }
                else { wpfobject.GetMainWindowByTitle(PACSWinName); }
                wpfobject.SelectTabFromTabItems(tab);
                Logger.Instance.InfoLog("Navigated to" + tab + "tab");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(tab + "tab is not clicked due to" + ex);
            }
        }

        public void ClickMinimise(String PACSWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(PACSWindowName)) { wpfobject.GetMainWindowByTitle(pacswinName); }
                else { wpfobject.GetMainWindowByTitle(PACSWindowName); }

                wpfobject.ClickButton("Minimize");

                //Thread.Sleep(2000);

                Logger.Instance.InfoLog("Uploader Tool minimized successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step Uploader tool minimize due to : " + ex);
            }

        }

        public void ClosePACSTool(int devicetype = 1, String PACSWinName = "")
        {

            try
            {
                String windowname = "";
                if (devicetype == 1) { windowname = pacswinName; } else { windowname = Config.pacswindow; }
                if (String.IsNullOrEmpty(PACSWinName)) { wpfobject.GetMainWindowByTitle(pacswinName); }
                else { wpfobject.GetMainWindowByTitle(PACSWinName); }

                Thread.Sleep(5000);

                wpfobject.KillProcess();
                BasePage.KillProcess("WerFault");
                try
                {
                    WpfObjects wpfObject1 = new WpfObjects();
                    Window crashWindow = wpfObject1.GetMainWindowByTitle("Client.Windows.PopConfigurationTool");
                    Button closeProgramButton = crashWindow.Get<Button>(SearchCriteria.ByText("Close the program"));
                    closeProgramButton.Click();
                    Logger.Instance.InfoLog("Client.Windows.PopConfigurationTool crash window closed successfully");
                    BasePage.KillProcess("WerFault");
                }
                catch (Exception)
                {
                    BasePage.KillProcess("WerFault");
                }                

                Logger.Instance.InfoLog("PACS Gateway closed successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step ClosePACSTool due to : " + ex);
            }
        }

        public String InstallPACSGateway(String eMailId = "", String Pin = "", String InstallerLocation = "", String Port = "104", String GateWayInstanceName = "")
        {
            try
            {
                if (Pin == null || Pin == "") { Pin = pin; }
                if (eMailId == null || eMailId == "") { eMailId = Config.emailid; }
                if (InstallerLocation == null || InstallerLocation == "")
                {
                    InstallerLocation = InstallerPath + "\\" + PACSGatewayInstallerName;
                }
                if (GateWayInstanceName == null || GateWayInstanceName == "")
                    GateWayInstanceName = base.PacsGatewayInstance;

                String SetUpWindowName = GateWayInstanceName + " Setup";
                String SCPport = Port;
                String InstallPath = "";            

                wpfobject.InvokeApplication(InstallerLocation);
                Thread.Sleep(5000);
                Window window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                if (window == null || !window.Visible)
                    throw new Exception("PACS GateWay Installation window not opened");

                Button NextButton = window.Get<Button>(SearchCriteria.ByText("Next"));
                NextButton.Click();

                window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                CheckBox EulaCheckBox = window.Get<CheckBox>(SearchCriteria.ByControlType(ControlType.CheckBox));
                if (!EulaCheckBox.Name.ToString().Contains("I accept"))
                {
                    Logger.Instance.WarnLog("EULA Checkbox not found to agree License Agreement");
                }

                if (!EulaCheckBox.Checked)
                    EulaCheckBox.Click();

                window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                NextButton = window.Get<Button>(SearchCriteria.ByText("Next"));
                if (NextButton.Enabled && NextButton.Visible)
                    NextButton.Click();
                else
                    throw new Exception("Next button is not enabled even when EULA was accepted");

                window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                Label Registration = window.Get<Label>(SearchCriteria.ByControlType(ControlType.Text).AndByText("Registration Information"));
                if (Registration == null || !Registration.Visible)
                    Logger.Instance.WarnLog("Expected window is not Registration window");

                TextBox PinTextbox = window.Get<TextBox>(SearchCriteria.ByControlType(ControlType.Edit).AndByText("PIN:"));
                if (PinTextbox == null || !PinTextbox.Enabled || !PinTextbox.Visible)
                    throw new Exception("Textbox to enter pin is not exists or enabled");

                PinTextbox.Text = Pin;

                int WindowCountBefore = Desktop.Instance.Windows().Count;
                NextButton = window.Get<Button>(SearchCriteria.ByText("Next"));
                NextButton.Click();

                window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                window.WaitWhileBusy();

                Window childWindow = null;
                if (window.ModalWindows().Any())
                {
                    childWindow = window.ModalWindow(SearchCriteria.ByClassName("MsiDialogCloseClass"));
                    if (childWindow != null && childWindow.Visible && childWindow.IsModal)
                    {
                        Button ReturnButton = childWindow.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText("Return"));
                        ReturnButton.Click();
                        throw new Exception("Unknown Username or Incorrect Password");
                    }
                }

                int WaitCount = 10;
                int WindowCountAfter = Desktop.Instance.Windows().Count;
                while (WindowCountAfter <= WindowCountBefore && WaitCount > 0)
                {
                    WaitCount--;
                    Thread.Sleep(5000);
                    WindowCountAfter = Desktop.Instance.Windows().Count;
                }

                Window DestinationWnd = wpfobject.GetMainWindowByTitle(GateWayInstanceName, strictCompare: true);              
                if (DestinationWnd == null || !DestinationWnd.Visible)
                    throw new Exception("PACS Gateway Setup screen not as expected, Destination Selection window not opened or found");

                TextBox SCPTextbox = DestinationWnd.Get<TextBox>(SearchCriteria.ByAutomationId(PACSGatewaySetupSCPPort));
                if (SCPTextbox != null && SCPTextbox.Enabled)
                {
                    SCPTextbox.Text = SCPport;
                    Logger.Instance.InfoLog("Port used for POP install-" + SCPport);
                }
                else
                    Logger.Instance.ErrorLog("SCP Port Textbox not found");

                DestinationWnd = wpfobject.GetMainWindowByTitle(GateWayInstanceName, strictCompare: true);               
                TextBox EmailTextbox = DestinationWnd.Get<TextBox>(SearchCriteria.ByControlType(ControlType.Edit).AndAutomationId(PACSGatewaySetupEmail));
                if (EmailTextbox != null && EmailTextbox.Enabled)
                {
                    EmailTextbox.Text = eMailId;
                }
                else
                    Logger.Instance.ErrorLog("Email Id Textbox not found");

                CheckBox SelectAllCheckBox = DestinationWnd.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewaySetupSelectAllDest));
                if (SelectAllCheckBox != null && SelectAllCheckBox.Enabled)
                {
                    SelectAllCheckBox.Checked = true;
                }

                Button nextButton = DestinationWnd.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndAutomationId("nextButton"));
                nextButton.Click();

                if (!DestinationWnd.IsClosed)
                {
                    Thread.Sleep(5000);
                }
                try
                {
                    window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                }
                catch (Exception)
                {
                    wpfobject = new WpfObjects();
                    window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                }
                Label ChooseFolder = window.Get<Label>(SearchCriteria.ByControlType(ControlType.Text).AndByText("Destination Folder"));
                if (ChooseFolder == null || !ChooseFolder.Visible)
                    Logger.Instance.WarnLog("Expected window is not Destination folder Selection window");

                TextBox PathTextbox = window.Get<TextBox>(SearchCriteria.ByControlType(ControlType.Edit).AndByClassName("RichEdit20W"));
                if (PathTextbox == null || !PathTextbox.Enabled || !PathTextbox.Visible)
                    throw new Exception("Choose Destination Location Textbox is not exists or enabled");

                InstallPath = PathTextbox.Text;
                NextButton = window.Get<Button>(SearchCriteria.ByText("Next"));
                NextButton.Click();

                window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                window.WaitWhileBusy();

                //Label ReadyToInstall = window.Get<Label>(SearchCriteria.ByControlType(ControlType.Text).AndByText("Ready to install*"));
                var ReadyToInstall = window.GetMultiple(SearchCriteria.ByControlType(ControlType.Text)).Select(item => item.Visible && item.Name.ToString().Contains("Ready to install"));
                if (ReadyToInstall == null || !ReadyToInstall.Any())
                    Logger.Instance.WarnLog("Expected window is not Ready to Install Final window");

                Button InstallButton = window.Get<Button>(SearchCriteria.ByText("Install"));
                InstallButton.Click();

                POP_WaitTillInstallationFinishes(GateWayInstanceName: GateWayInstanceName);

                try
                {
                    window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                }
                catch (Exception)
                {
                    wpfobject = new WpfObjects();
                    window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                }
                var FinalWizard = window.GetMultiple(SearchCriteria.ByControlType(ControlType.Text)).Select(item => item.Visible && item.Name.ToString().Contains("Completed the PACS Gateway"));
                if (FinalWizard == null || !FinalWizard.Any())
                    Logger.Instance.WarnLog("Expected window is not Setup wizard completed window");

                ListBox DestinationListbox = window.Get<ListBox>(SearchCriteria.ByControlType(ControlType.List).AndByClassName("ListBox"));
                if (DestinationListbox == null || DestinationListbox.Items.Count <= 0)
                {
                    throw new Exception("PACS Gateway Setup screen not as expected, Selected AETitle not displayed in Listbox");
                }

                Button FinishButton = window.Get<Button>(SearchCriteria.ByText("Finish"));
                FinishButton.Click();

                if (!window.IsClosed)
                {
                    Thread.Sleep(5000);
                    window = wpfobject.GetMainWindowByTitle(SetUpWindowName, strictCompare: true);
                    if (!window.IsClosed)
                        throw new Exception("PACS Gateway Setup Screen still exists after finished Installation");
                }

                bool srvc = wpfobject.ServiceStatus(GateWayInstanceName + " Service", "Running");
                if (!srvc)
                {
                    throw new Exception("PACS Gateway Service is not running after installation Completes");
                }
                return InstallPath;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in installing Pop Tool: " + ex.Message);
                return null;
            }
        }

        public void CheckPopupinPOPTool(String PACSWinName)
        {
            try
            {
                if (String.IsNullOrEmpty(PACSWinName)) { wpfobject.GetMainWindowByTitle(pacswinName); }
                else { wpfobject.GetMainWindowByTitle(PACSWinName); }

                IList<TestStack.White.UIItems.WindowItems.Window> windows = WpfObjects._application.GetWindows();
                if (windows.Count == 2)
                {
                    wpfobject.GetMainWindowByTitle(PACSWinName);
                    windows[1].Focus(); windows[1].Click();
                    wpfobject.ClickButton("OK", 1);
                    Logger.Instance.InfoLog("Error Message POP Displayed in POP Config Tool. Hence Clicked OK Button");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in clicking Ok Button in Error Message popup in PACSTool due to : " + ex);
            }
        }

        public String waitForStudyEntryInPOP(string PatientName, string PACSWindowName = "")
        {
            int loop = 0;
            bool entryFound = false;
            string PatientNameinPOP = "";
            string StudyTransferStatus = "";
            try
            {
                while (loop++ < 300)
                {
                    Thread.Sleep(1000);
                    PACSGatewayRefreshtransfer_Btn().Click();
                    CheckPopupinPOPTool(PACSWindowName);
                    if ((datagrid().Items.Count()) != 0)
                    {
                        foreach (var row in datagrid().Rows)
                        {
                            PatientNameinPOP = row.Cells[1].Text;
                            StudyTransferStatus = row.Cells[5].Text;
                            Logger.Instance.InfoLog("Patient Name in POP Config Tool----------> " + PatientNameinPOP);
                            Logger.Instance.InfoLog("Study Transfer Status in POP Config Tool----------> " + StudyTransferStatus);
                            if ((PatientNameinPOP.ToLower()).Contains(PatientName.ToLower()))
                            {
                                entryFound = true;
                                if (StudyTransferStatus.ToLower() == "validating")
                                {
                                    int j = 0;
                                    while (j++ < 300)
                                    {
                                        Thread.Sleep(1000);
                                        PACSGatewayRefreshtransfer_Btn().Click();
                                        CheckPopupinPOPTool(PACSWindowName);
                                        StudyTransferStatus = datagrid().Rows[0].Cells[5].Text;//row.Cells[5].Text;
                                        Logger.Instance.InfoLog("Study Transfer Status ----------> " + StudyTransferStatus);
                                        if (StudyTransferStatus.ToLower() == "in progress") break;
                                    }
                                }
                                break;
                            }
                            else continue;
                        }
                    }
                    if (entryFound) break;
                }
                //Check whether the entry is there in Active Transfers tab
                if (!entryFound)
                { throw new Exception("Waited for 5 minutes. Uploaded study entry is not listed in Active Transfers tab"); }
                return StudyTransferStatus;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step waitForStudyEntryInPOP due to : " + ex);
                throw ex;
            }
        }

        public String InstallPACSGatewayWithLocale(int locale = 0, string eMailId = "", String Pin = "", String InstallerLocation = "", String Port = "104", String GateWayInstanceName = "")
        {
            try
            {
                if (Pin == null || Pin == "") { Pin = pin; }
                if (eMailId == null || eMailId == "") { eMailId = Config.emailid; }
                if (InstallerLocation == null || InstallerLocation == "")
                {
                    InstallerLocation = InstallerPath + "\\" + PACSGatewayInstallerName;
                }
                if (GateWayInstanceName == null || GateWayInstanceName == "")
                    GateWayInstanceName = base.PacsGatewayInstance;

                String SetUpWindowName = GateWayInstanceName + " Setup";
                String SCPport = Port;
                String InstallPath = "";

                wpfobject.InvokeApplication(InstallerLocation, locale: 1);
                Thread.Sleep(5000);
                Window window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                if (window == null || !window.Visible)
                    throw new Exception("PACS GateWay Installation window not opened");
                wpfobject.ClickRadioButton(SelectLanguage(), 1);
                wpfobject.ClickButton("Install", 1);
                wpfobject.WaitTillLoad();
                int waittime = 0;
                while (waittime < 10)
                {
                    try
                    {
                        window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                        Button next = window.Get<Button>(SearchCriteria.ByText(Next()));
                        if (next != null)
                            waittime = 10;
                        else
                        {
                            waittime++;
                            Thread.Sleep(5000);
                        }
                    }
                    catch (Exception)
                    {
                        waittime++;
                        Thread.Sleep(5000);
                    }
                }
                wpfobject.ClickButton(Next(), 1);
                window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                CheckBox EulaCheckBox = window.Get<CheckBox>(SearchCriteria.ByControlType(ControlType.CheckBox));
                if (!EulaCheckBox.Checked)
                    EulaCheckBox.Click();
                wpfobject.ClickButton(Next(), 1);
                Label Registration = window.Get<Label>(SearchCriteria.ByControlType(ControlType.Text).AndByText("Registration Information"));
                if (Registration == null || !Registration.Visible)
                    Logger.Instance.WarnLog("Expected window is not Registration window");

                TextBox PinTextbox = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByClassName("RichEdit20W"));
                if (PinTextbox == null || !PinTextbox.Enabled || !PinTextbox.Visible)
                    throw new Exception("Textbox to enter pin is not exists or enabled");

                PinTextbox.Text = Pin;
                try
                {
                    pinlabeltext = PinTextbox.Name;
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
                int WindowCountBefore = Desktop.Instance.Windows().Count;
                wpfobject.ClickButton(Next(), 1);
                while (WindowCountBefore == Desktop.Instance.Windows().Count)
                {
                    Thread.Sleep(10000);
                }
                window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                window.WaitWhileBusy();
                Window DestinationWnd = wpfobject.GetMainWindowByTitle(GateWayInstanceName);
                if (DestinationWnd == null || !DestinationWnd.Visible)
                    throw new Exception("PACS Gateway Setup screen not as expected, Destination Selection window not opened or found");

                TextBox SCPTextbox = DestinationWnd.Get<TextBox>(SearchCriteria.ByAutomationId(PACSGatewaySetupSCPPort));
                if (SCPTextbox != null && SCPTextbox.Enabled)
                {
                    SCPTextbox.Text = SCPport;
                }
                else
                    Logger.Instance.ErrorLog("SCP Port Textbox not found");

                DestinationWnd = wpfobject.GetMainWindowByTitle(GateWayInstanceName);
                TextBox EmailTextbox = DestinationWnd.Get<TextBox>(SearchCriteria.ByControlType(ControlType.Edit).AndAutomationId(PACSGatewaySetupEmail));
                if (EmailTextbox != null && EmailTextbox.Enabled)
                {
                    EmailTextbox.Text = eMailId;
                }
                else
                    Logger.Instance.ErrorLog("Email Id Textbox not found");

                CheckBox SelectAllCheckBox = DestinationWnd.Get<CheckBox>(SearchCriteria.ByAutomationId(PACSGatewaySetupSelectAllDest));
                if (SelectAllCheckBox != null && SelectAllCheckBox.Enabled)
                {
                    SelectAllCheckBox.Checked = true;
                }

                Button nextButton = DestinationWnd.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndAutomationId("nextButton"));
                nextButton.Click();

                if (!DestinationWnd.IsClosed)
                {
                    Thread.Sleep(5000);
                }
                wpfobject = new WpfObjects();
                window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                TextBox PathTextbox = window.Get<TextBox>(SearchCriteria.ByClassName("RichEdit20W"));
                if (PathTextbox == null || !PathTextbox.Enabled || !PathTextbox.Visible)
                    throw new Exception("Choose Destination Location Textbox is not exists or enabled");

                InstallPath = PathTextbox.Text;
                wpfobject.ClickButton(Next(), 1);

                window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                window.WaitWhileBusy();

                //Label ReadyToInstall = window.Get<Label>(SearchCriteria.ByControlType(ControlType.Text).AndByText("Ready to install*"));
                var ReadyToInstall = window.GetMultiple(SearchCriteria.ByControlType(ControlType.Text)).Select(item => item.Visible && item.Name.ToString().Contains("インストールを開始するには"));
                if (ReadyToInstall == null || !ReadyToInstall.Any())
                    Logger.Instance.WarnLog("Expected window is not Ready to Install Final window");
                wpfobject.ClickButton(Install(), 1);
                waittime = 0;
                while (waittime < 10)
                {
                    try
                    {
                        wpfobject = new WpfObjects();
                        window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                        Button Final = window.Get<Button>(SearchCriteria.ByText(Finish()));
                        if (Final == null)
                        {
                            waittime++;
                            Thread.Sleep(5000);
                        }
                        else
                        {
                            waittime = 10;
                        }
                    }
                    catch (Exception)
                    {
                        waittime++;
                        Thread.Sleep(5000);
                    }
                }
                wpfobject = new WpfObjects();
                window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                var FinalWizard = window.GetMultiple(SearchCriteria.ByControlType(ControlType.Text)).Select(item => item.Visible && item.Name.ToString().Contains("successfully installed"));
                if (FinalWizard == null || !FinalWizard.Any())
                    Logger.Instance.WarnLog("Expected window is not Setup wizard completed window");

                ListBox DestinationListbox = window.Get<ListBox>(SearchCriteria.ByControlType(ControlType.List).AndByClassName("ListBox"));
                if (DestinationListbox == null || DestinationListbox.Items.Count <= 0)
                {
                    throw new Exception("PACS Gateway Setup screen not as expected, Selected AETitle not displayed in Listbox");
                }
                wpfobject.ClickButton(Finish(), 1);
                Thread.Sleep(5000);
                wpfobject.ClickButton("Close", 1);
                if (!window.IsClosed)
                {
                    Thread.Sleep(5000);
                    window = wpfobject.GetMainWindowByTitle(SetUpWindowName);
                    if (!window.IsClosed)
                        throw new Exception("PACS Gateway Setup Screen still exists after finished Installation");
                }

                bool srvc = wpfobject.ServiceStatus(GateWayInstanceName + " Service", "Running");
                if (!srvc)
                {
                    throw new Exception("PACS Gateway Service is not running after installation Completes");
                }
                return InstallPath;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in installing Pop Tool: " + ex.Message);
                return null;
            }
        }

        #region LocaleElements

        public string Setup()
        {
            string setup = null;
            if (Config.Locale.Equals("ja-JP"))
                setup = " セットアップ";
            return setup;
        }

        public string Accept()
        {
            string accept = null;
            if (Config.Locale.Equals("ja-JP"))
                accept = "使用許諾契約書に同意します(A)";

            return accept;
        }

        public string Next()
        {
            string next = null;
            if (Config.Locale.Equals("ja-JP"))
                next = "次へ(N)";

            return next;
        }

        public string AdministratorOption()
        {
            string admin = null;
            if (Config.Locale.Equals("ja-JP"))
                admin = "自分のみを対象にインストール (Administrator)(J)";

            return admin;
        }

        public string Install()
        {
            string install = null;
            if (Config.Locale.Equals("ja-JP"))
                install = "インストール(I)";

            return install;
        }

        public string SelectLanguage()
        {
            string lanuage = null;
            if (Config.Locale.Equals("ja-JP"))
                lanuage = "日本語 (日本)";

            return lanuage;
        }

        public string Print()
        {
            string print = null;
            if (Config.Locale.Equals("ja-JP"))
                print = "印刷(P)";

            return print;
        }

        public string Back()
        {
            string back = null;
            if (Config.Locale.Equals("ja-JP"))
                back = "戻る(B)";

            return back;
        }

        public string Cancel()
        {
            string cancel = null;
            if (Config.Locale.Equals("ja-JP"))
                cancel = "キャンセル";

            return cancel;
        }

        public string Return()
        {
            string rturn = null;
            if (Config.Locale.Equals("ja-JP"))
                rturn = "戻る(R)";

            return rturn;
        }

        public string Finish()
        {
            string finish = null;
            if (Config.Locale.Equals("ja-JP"))
                finish = "完了(F)";

            return finish;
        }

        public string FolderChoose()
        {
            string choosefolder = null;
            if (Config.Locale.Equals("ja-JP"))
                choosefolder = "変更(C)...";
            return choosefolder;
        }
        #endregion LocaleElements

    }
}
