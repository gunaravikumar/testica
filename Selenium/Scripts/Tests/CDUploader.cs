using System;
using System.Threading;
using System.Collections.Generic;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.ServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using System.Diagnostics;

namespace Selenium.Scripts.Tests
{
    class CDUploader : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public ServiceTool servicetool { get; set; }
        string FolderPath = "";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public CDUploader(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            ei = new ExamImporter();
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
        }

        /// <summary>
        /// Helper execution case for Admin install - All users install/uninstall
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test1_91564(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String FolderPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FolderPath");
                String ClientIPAddress = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ClientIPAddress");

                ExecuteMethodOnClient(ClientIPAddress, FolderPath, "CDUploader", "Test2_91564");
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
        /// Admin install - All users install/uninstall
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_91564(String testid, String teststeps, int stepcount)
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
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String EIWindowName = Config.eiwindow;
                //String DesktopPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DesktopPath");
                String DownloadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string PassString = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                string UserString = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UserName");
                string UserDesktopPath = @"C:\Users\" + UserString + @"\Desktop";
                String ShortcutName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ShortcutName");

                //Step-1: The user runs the installation for the EI from the Login screen or from the downloaded MSI
                //Delete existing installer file
                login.DriverGoTo(login.url);
                try
                {
                    File.Delete(DownloadFilePath + @"\Installer.UploaderTool.msi");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Problems in deleting the previous installer file due to : " + e);
                }

                ei._examImporterInstance = EIWindowName;
                //Download new installer file and Minimize all apps
                login.DownloadInstaller(login.url, "CDUpload", DownloadFilePath + @"\Installer.UploaderTool.msi", "SuperAdminGroup");
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);

                //Uninstall App if already installed
                if (ei.IsEiInstalled())
                {
                    ei.UnInstallEI();
                }

                //Proceed with installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-2: User(st) checks the "I accept.." checkbox
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step2_1 = WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install just for you (Administrator)")).Visible;
                bool step2_2 = WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install for all users of this machine")).Visible; 
                
                if (step2_1 && step2_2)
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

                //Step-3: Select for all users radio button 
                wpfobject.ClickRadioButton(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step3_1 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Email:")).Visible;
                bool step3_2 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Password:")).Visible;

                if (step3_1 && step3_2)
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

                //Step-4: Selects Registered user Enters iCA credentials (ph/ph)
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(Config.phUserName, Config.phPassword);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step4 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible; 

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
                //Step-5: Uncheck Launch application when setup exits and Finish
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-6: Check the EI shortcut on desktop
                //Shortcut name to be discussed
                string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                string Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-7: Open control panel and check for EI
                ProcessStartInfo startInfo = new ProcessStartInfo("appwiz.cpl");
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                wpfobject.WaitForButtonExist("Programs and Features", "Organize", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                bool step7 = wpfobject.GetTextbox(ei._examImporterInstance, 1).Visible;
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

                ////Step-8 to 19: Steps involve Login in client system as Standard user, hence marking as Not Automated
                //for (int i = 0; i < 12; i++)
                //    result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-8: Check the EI shortcut on desktop
                DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-9: Open control panel and check for EI 
                // Since this deals with checking Control of standard user, we cannot do this with administrator login
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-10: Run the EI Installation
                ei.LaunchEiInstallerAsDifferentUser(DownloadFilePath, "Installer.UploaderTool.msi", UserString, PassString);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-11: Click next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step11_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step11_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step11_1 && step11_2)
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

                //Step-12: User clicks on Repair button
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step12 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
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

                //Step-13: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-14: Uninstall the EI from COntrol panel
                //Step Marked as NA since we cannot open standard user Control panel. Hence uninstalling using silent uninstaller
                result.steps[++ExecutedSteps].status = "Not Automated";
                ei.UnInstallEI(DownloadFilePath, UserString, PassString);

                //Step-15: The user runs the Exam Importer installation
                ei.LaunchEiInstallerAsDifferentUser(DownloadFilePath, "Installer.UploaderTool.msi", UserString, PassString);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-16: User clicks on Next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step16_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step16_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step16_1 && step16_2)
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

                //Step-17: User clicks on Remove button
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Remove", 1);
                wpfobject.WaitTillLoad();
                bool step17 = wpfobject.VerifyIfTextExists("Ready to remove " + ei._examImporterInstance);
                if (step17)
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

                //Step-18: User clicks on Remove
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "OK", 1);
                bool step18 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("OK")).Visible;
                if (step18)
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
                WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("OK")).Click();

                //Step-19: Click Finish
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-20: Login in client system as Administrator & Check for EI Shortcut
                DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-21: Run the EI Installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-22: Click next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step22_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible; 
                bool step22_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible; 
                if (step22_1 && step22_2)
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

                //Step-23: User clicks on Repair button
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Repair", 1);
                wpfobject.WaitTillLoad();
                bool step23 = wpfobject.VerifyIfTextExists("Ready to repair " + ei._examImporterInstance);
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

                //Step-24: User clicks on Repair
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step24 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
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

                //Step-25: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-26: Uninstall the EI from COntrol panel
                //Kill Uploader Process
                login.KillProcessByName("explorer");
                Thread.Sleep(3000);
                login.KillProcessByName("UploaderTool");
                Thread.Sleep(3000);
                startInfo = new ProcessStartInfo("appwiz.cpl");
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                wpfobject.WaitForButtonExist("Programs and Features", "Organize", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.GetTextbox(ei._examImporterInstance, 1).Click();
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("Uninstall", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                bool confirm = wpfobject.VerifyElement("CommandButton_6", "Yes");
                wpfobject.ClickButton("CommandButton_6");
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
                Thread.Sleep(40000);
                //try
                //{
                //    wpfobject.GetMainWindowByTitle("Windows Installer");
                //    while (WpfObjects._mainWindow.IsClosed) { }
                //}
                //catch (Exception ex) { Logger.Instance.ErrorLog("Exception in step 26 due to : " + ex);  }

                //try
                //{
                //    wpfobject.GetMainWindowByTitle(ei._examImporterInstance);
                //    while (WpfObjects._mainWindow.IsClosed) { }
                //}
                //catch (Exception ex) { Logger.Instance.ErrorLog("Exception in step 26 due to : " + ex); }

                //Step-27: Install EI for All users
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);

                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.ClickRadioButton(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(Config.phUserName, Config.phPassword);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step27 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible; 
                
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();

                if (step27)
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

                //Step-28: The user runs the Exam Importer installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-29: User clicks on Next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step29_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible; 
                bool step29_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible; 
                if (step29_1 && step29_2)
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

                //Step-30: User clicks on Remove button
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Remove", 1);
                wpfobject.WaitTillLoad();
                bool step30 = wpfobject.VerifyIfTextExists("Ready to remove " + ei._examImporterInstance);
                if (step30)
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

                //Step-31: User clicks on Remove
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step31 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible; 
                if (step31)
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

                //Step-32: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
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
        /// Helper execution case for Admin install - Current user
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test1_91565(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String FolderPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FolderPath");
                String ClientIPAddress = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ClientIPAddress");

                ExecuteMethodOnClient(ClientIPAddress, FolderPath, "CDUploader", "Test2_91565");
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
        /// Admin install - Current user
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_91565(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Studies studies = null;
            StudyViewer StudyVw;
            Viewer viewer = new Viewer();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String EIWindowName = Config.eiwindow;
                String PHUsername = Config.phUserName;
                String PHPassword = Config.phPassword;
                String EmailAddress = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String DownloadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string PassString = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                string UserString = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UserName");
                string UserDesktopPath = @"C:\Users\" + UserString + @"\Desktop";
                String ShortcutName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ShortcutName");

                //Step-1: The user runs the installation for the EI from the Login screen or from the downloaded MSI
                //Delete existing installer file
                login.DriverGoTo(login.url);
                try
                {
                    File.Delete(DownloadFilePath + @"\Installer.UploaderTool.msi");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Problems in deleting the previous installer file due to : " + e);
                }

                ei._examImporterInstance = EIWindowName;
                //Download new installer file and Minimize all apps
                login.DownloadInstaller(login.url, "CDUpload", DownloadFilePath + @"\Installer.UploaderTool.msi", "SuperAdminGroup");
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);

                //Uninstall App if already installed
                if (ei.IsEiInstalled())
                {
                    ei.UnInstallEI();
                }

                //Proceed with installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-2: User(st) checks the "I accept.." checkbox
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step2_1 = WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install just for you (Administrator)")).Visible;
                bool step2_2 = WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install for all users of this machine")).Visible;

                if (step2_1 && step2_2)
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

                //Step-3: Select for current users radio button 
                wpfobject.ClickRadioButton(0);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step3_1 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Email:")).Visible;
                bool step3_2 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Password:")).Visible;

                if (step3_1 && step3_2)
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

                //Step-4: Selects UnRegistered user and provide a valid email address and then clicks on Install
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(EmailAddress, "", 0);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step4 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;

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
                //Step-5: Uncheck Launch application when setup exits and Finish
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-6: Check the EI shortcut on desktop
                //Shortcut name to be discussed
                string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-7: Open control panel and check for EI
                ProcessStartInfo startInfo = new ProcessStartInfo("appwiz.cpl");
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                wpfobject.WaitForButtonExist("Programs and Features", "Organize", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                bool step7 = wpfobject.GetTextbox(ei._examImporterInstance, 1).Visible;
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

                ////Step-8 to 14: Steps involve Login in client system as Standard user, hence marking as Not Automated
                //for (int i = 0; i < 7; i++)
                //    result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-8: Check the EI shortcut on desktop - STD user
                DesktopPath = UserDesktopPath;
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (!File.Exists(Shortcutfile))
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

                //Step-9: Open control panel and check for EI 
                // Since this deals with checking Control of standard user, we cannot do this with administrator login
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-10: Run the installer
                ei.LaunchEiInstallerAsDifferentUser(DownloadFilePath, "Installer.UploaderTool.msi", UserString, PassString);
                ExecutedSteps++;

                //Step-11: User(st) checks the "I accept.." checkbox
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step11_1 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Email:")).Visible;
                bool step11_2 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Password:")).Visible;

                if (step11_1 && step11_2)
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

                //Step-12: Selects Registered user Enters iCA credentials (ph/ph)
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(PHUsername, PHPassword);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step12 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;

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

                //Step-13: Uncheck Launch application when setup exits and Finish
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-14: Open control panel and check for EI 
                // Since this deals with checking Control of standard user, we cannot do this with administrator login
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-15: Login in client system as Administrator & Check for EI Shortcut
                DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-16: Run the EI Installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-17: Click next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step17_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step17_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step17_1 && step17_2)
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

                //Step-18: User clicks on Repair button
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Repair", 1);
                wpfobject.WaitTillLoad();
                bool step18 = wpfobject.VerifyIfTextExists("Ready to repair " + ei._examImporterInstance);
                if (step18)
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

                //Step-19: User clicks on Repair
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step19 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
                if (step19)
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

                //Step-20: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-21: Uninstall the EI from COntrol panel
                //Kill Uploader Process
                login.KillProcessByName("explorer");
                Thread.Sleep(3000);
                login.KillProcessByName("UploaderTool");
                Thread.Sleep(3000);
                startInfo = new ProcessStartInfo("appwiz.cpl");
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                wpfobject.WaitForButtonExist("Programs and Features", "Organize", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.GetTextbox(ei._examImporterInstance, 1).Click();
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("Uninstall", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                bool confirm = wpfobject.VerifyElement("CommandButton_6", "Yes");
                wpfobject.ClickButton("CommandButton_6");
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
                Thread.Sleep(40000);

                //Step-22: Install EI for Current users
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);

                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.ClickRadioButton(0);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(EmailAddress, "", 0);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step22 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();

                if (step22)
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

                //Step-23: The user runs the Exam Importer installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-24: User clicks on Next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step24_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step24_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step24_1 && step24_2)
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

                //Step-25: User clicks on Remove button
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Remove", 1);
                wpfobject.WaitTillLoad();
                bool step25 = wpfobject.VerifyIfTextExists("Ready to remove " + ei._examImporterInstance);
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

                //Step-26: User clicks on Remove
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step26 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
                if (step26)
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

                //Step-27: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Clean-up: Post conditions - removing EI from standard user account
                ei.UnInstallEI(DownloadFilePath, UserString, PassString);


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
        /// Helper execution case for Standard user - EI install
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test1_91845(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String FolderPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FolderPath");
                String ClientIPAddress = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ClientIPAddress");

                ExecuteMethodOnClient(ClientIPAddress, FolderPath, "CDUploader", "Test2_91845");
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
        /// Standard user - EI install
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_91845(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            Studies studies = null;
            StudyViewer StudyVw;
            Viewer viewer = new Viewer();
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
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String DownloadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string PassString = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Password");
                string UserString = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UserName");
                string UserDesktopPath = @"C:\Users\" + UserString + @"\Desktop";
                String ShortcutName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ShortcutName");

                String EIWindowName = Config.eiwindow;
                ei._examImporterInstance = EIWindowName;

                //Step-1: Standard User runs installation
                Directory.CreateDirectory(DownloadFilePath);
                login.DriverGoTo(login.url);
                try
                {
                    File.Delete(DownloadFilePath + @"\Installer.UploaderTool.msi");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Problems in deleting the previous installer file due to : " + e);
                }

                //Download new installer file and Minimize all apps
                login.DownloadInstaller(login.url, "CDUpload", DownloadFilePath + @"\Installer.UploaderTool.msi", "SuperAdminGroup");
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null);

                //Uninstall App if already installed
                if (ei.IsEiInstalled())
                {
                    ei.UnInstallEI();
                }

                ei.LaunchEiInstallerAsDifferentUser(DownloadFilePath, "Installer.UploaderTool.msi", UserString, PassString);
                ExecutedSteps++;

                //Step-2: User(st) checks the "I accept.." checkbox
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step2_1 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Email:")).Visible;
                bool step2_2 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Password:")).Visible;

                if (step2_1 && step2_2)
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

                //Step-3: Selects Registered user Enters iCA credentials (ph/ph)
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(Config.phUserName, Config.phPassword);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step3 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;

                if (step3)
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
                //Step-4: Uncheck Launch application when setup exits and Finish
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-5: Check the EI shortcut on desktop
                //Shortcut name to be discussed
                string DesktopPath = UserDesktopPath;
                string Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-6: Open control panel and check for EI 
                // Since this deals with checking Control of standard user, we cannot do this with administrator login
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-7: Login in client system as Administrator & Check for EI Shortcut
                DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (!File.Exists(Shortcutfile))
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

                //Step-8: Open control panel and check for EI
                ProcessStartInfo startInfo = new ProcessStartInfo("appwiz.cpl");
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                wpfobject.WaitForButtonExist("Programs and Features", "Organize", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                TextBox step8 = null;
                try { step8 = wpfobject.GetTextbox(ei._examImporterInstance, 1);  }
                catch (Exception ex){ Logger.Instance.ErrorLog(ex.Message);  }
                if (step8==null)
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
                //Step-9: Run the EI Installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-10: User(st) checks the "I accept.." checkbox
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step10_1 = WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install just for you (Administrator)")).Visible;
                bool step10_2 = WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install for all users of this machine")).Visible;

                if (step10_1 && step10_2)
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

                //Step-11: Select for all users radio button 
                wpfobject.ClickRadioButton(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step11_1 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Email:")).Visible;
                bool step11_2 = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Password:")).Visible;

                if (step11_1 && step11_2)
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

                //Step-12: Selects Registered user Enters iCA credentials (ph/ph)
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(Config.phUserName, Config.phPassword);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                bool step12 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;

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
                //Step-13: Uncheck Launch application when setup exits and Finish
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-14: Check the EI shortcut on desktop
                //Shortcut name to be discussed
                DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-15: Run the EI Installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-16: Click next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step16_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step16_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step16_1 && step16_2)
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

                //Step-17: User clicks on Repair button
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Repair", 1);
                wpfobject.WaitTillLoad();
                bool step17 = wpfobject.VerifyIfTextExists("Ready to repair " + ei._examImporterInstance);
                if (step17)
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

                //Step-18: User clicks on Repair
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step18 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
                if (step18)
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

                //Step-19: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-20: Uninstall the EI from COntrol panel
                //Kill Uploader Process
                login.KillProcessByName("explorer");
                Thread.Sleep(3000);
                login.KillProcessByName("UploaderTool");
                Thread.Sleep(3000);
                startInfo = new ProcessStartInfo("appwiz.cpl");
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                wpfobject.WaitForButtonExist("Programs and Features", "Organize", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.GetTextbox(ei._examImporterInstance, 1).Click();
                wpfobject.GetMainWindowByTitle("Programs and Features");
                wpfobject.ClickButton("Uninstall", 1);
                wpfobject.GetMainWindowByTitle("Programs and Features");
                bool step20 = wpfobject.VerifyElement("CommandButton_6", "Yes");
                wpfobject.ClickButton("CommandButton_6");
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
                    throw new Exception("iCA not present");
                }
                Thread.Sleep(40000);

                //Step-21: Check EI shortcut on Standard user desktop
                DesktopPath = UserDesktopPath;
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-22: Check EI shortcut on Administrator desktop
                DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (!File.Exists(Shortcutfile))
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

                //Step-23: Install EI for all users

                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);

                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.ClickRadioButton(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.ClickButton("Next", 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                ei.EI_InputRegistrationDetails(Config.phUserName, Config.phPassword);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step23 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();

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

                //Step-24: The user runs the Exam Importer installation
                ei.LaunchEiInstaller(DownloadFilePath);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-25: User clicks on Next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step25_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step25_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step25_1 && step25_2)
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

                //Step-26: User clicks on Remove button
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Remove", 1);
                wpfobject.WaitTillLoad();
                bool step26 = wpfobject.VerifyIfTextExists("Ready to remove " + ei._examImporterInstance);
                if (step26)
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

                //Step-27: User clicks on Remove
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step27 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
                if (step27)
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

                //Step-28: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-29: Check standard user desktop for EI shortcut
                DesktopPath = UserDesktopPath;
                Shortcutfile = DesktopPath + Path.DirectorySeparatorChar + ShortcutName;
                if (File.Exists(Shortcutfile))
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

                //Step-30: Run the EI Installation
                ei.LaunchEiInstallerAsDifferentUser(DownloadFilePath, "Installer.UploaderTool.msi", UserString, PassString);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-31: Click next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step31_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step31_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step31_1 && step31_2)
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

                //Step-32: User clicks on Repair button
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Repair", 1);
                wpfobject.WaitTillLoad();
                bool step32 = wpfobject.VerifyIfTextExists("Ready to repair " + ei._examImporterInstance);
                if (step32)
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

                //Step-33: User clicks on Repair
                wpfobject.ClickButton("Repair", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step33 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
                if (step33)
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

                //Step-34: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step-35: Uninstall the EI from COntrol panel
                //Step Marked as NA since we cannot open standard user Control panel. Hence uninstalling using silent uninstaller
                result.steps[++ExecutedSteps].status = "Not Automated";
                ei.UnInstallEI(DownloadFilePath, UserString, PassString);
                

                //Step-36: Install EI 

                ei.LaunchEiInstallerAsDifferentUser(DownloadFilePath, "Installer.UploaderTool.msi", UserString, PassString);

                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                ei.EI_InputRegistrationDetails(Config.phUserName, Config.phPassword);

                ei.EI_SubmitRegistrationDetails();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step36 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;

                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_SelectAutoLaunchOption(false);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                ei.EI_FinishInstallation();
                wpfobject.WaitTillLoad();

                if (step36)
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

                //Step-37: The user runs the Exam Importer installation
                ei.LaunchEiInstallerAsDifferentUser(DownloadFilePath, "Installer.UploaderTool.msi", UserString, PassString);
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Cancel", 1);
                ExecutedSteps++;

                //Step-38: User clicks on Next
                ei.EI_AcceptEulaInstaller();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ei._examImporterInstance + " Setup");
                wpfobject.WaitTillLoad();

                bool step38_1 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Repair")).Visible;
                bool step38_2 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Remove")).Visible;
                if (step38_1 && step38_2)
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

                //Step-39: User clicks on Remove button
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Remove", 1);
                wpfobject.WaitTillLoad();
                bool step39 = wpfobject.VerifyIfTextExists("Ready to remove " + ei._examImporterInstance);
                if (step39)
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

                //Step-40: User clicks on Remove
                wpfobject.ClickButton("Remove", 1);
                wpfobject.WaitTillLoad();
                wpfobject.WaitForButtonExist(ei._examImporterInstance + " Setup", "Finish", 1);
                bool step40 = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")).Visible;
                if (step40)
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

                //Step-41: Click Finish
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
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
        /// Cleanup script to close browser
        /// </summary>
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }

    }
}
