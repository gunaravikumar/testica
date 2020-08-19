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
using System.Data;
using System.Windows;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;
using System.ServiceProcess;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace Selenium.Scripts.Tests
{
    class DevelopersLog
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        string updatedateandtimebatchfile = string.Empty;
        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public DevelopersLog(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            hphomepage = new HPHomePage();
            mpaclogin = new MpacLogin();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            updatedateandtimebatchfile = string.Concat(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "OtherFiles\\UpdateDatetime.bat");
        }


        Studies studies = new Studies();
        StudyViewer StudyViewer = new StudyViewer();
        UserPreferences UserPref = new UserPreferences();

        RoleManagement rolemanagement = new RoleManagement();
        UserManagement usermanagement = new UserManagement();
        DomainManagement domainmanagement = new DomainManagement();

        /// <summary>
        /// Rolling Text Trace Listener - Hourly Logs with File Size Limit on UI
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161143(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            BasePage basepage = new BasePage();
            TestCaseResult result;

            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String VadidationPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ValidationPath");
            String[] validationPath = VadidationPath.Split('=');
            string TempLog = Config.TestDataPath + (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadPath");

            int ExecutedSteps = -1;
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step -1:  From iCA server, search for the following property name in C-\WebAccess\WebAccess\Web.config and update the value as "Verbose"
                //-add name - "DeveloperTraceSwitch" value - "Verbose" 
                basepage.SetWebConfigValue(Config.webconfig, "DeveloperTraceSwitch", "Verbose");
                ExecutedSteps++;

                //Step-2 : From iCA service tool, Click on Developer Log tab and modify the settings as below.
                //Log Type-WebAccess Developer
                //Log Path - Enter valid path say as "C-\WebAccess Logs\WebAccessDeveloper.log" and test the path as well
                //Creation Rule-Hourly
                //UTC DateTime-Unchecked
                //File Size Limit - 1
                //Purge Rule-Default
                //Click on Apply and Restart the services.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Developer Logs");
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.WaitWhileBusy();
                servicetool.LogType().SetValue("WebAccess Developer");//Anonymous STS
                servicetool.LogPath().SetValue(validationPath[0] + "\\WebAccessDeveloper.log");
                servicetool.Creationrule().SetValue("Hourly");
                servicetool.FileSize().SetValue("1");
                //servicetool.PurgeRule().SetValue("0");
                servicetool.UTCDateTimeCb().UnSelect();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;


                //Step 3: Open the file "C-\WebAccess\WebAccess\Web.config" and ensure that following property values are modified based on the above Developer log tab settings.
                //-add name - "DeveloperLog" type - "OpenContent.Core.Log.Utility.RollingTextTraceListener, OpenContent.Generic.Core" initializeData - "C-\WebAccess Logs\WebAccessDeveloper.log" UtcDatetime - "False" creationRule - "Hourly" fileSizeLimitMB - "1" purgeRule -”0”/ -
                String NodePath = "configuration/system.diagnostics/sharedListeners/add";
                bool Step3_1 = basepage.GetNodeValue(Config.webconfig, NodePath, "DeveloperLog", "type", "OpenContent.Core.Log.Utility.RollingTextTraceListener, OpenContent.Generic.Core");
                bool Step3_2 = basepage.GetNodeValue(Config.webconfig, NodePath, "DeveloperLog", "initializeData", validationPath[0]);
                bool Step3_3 = basepage.GetNodeValue(Config.webconfig, NodePath, "DeveloperLog", "creationRule", "Hourly");
                bool Step3_4 = basepage.GetNodeValue(Config.webconfig, NodePath, "DeveloperLog", "maxFileSizeMB", "1");
                bool Step3_5 = basepage.GetNodeValue(Config.webconfig, NodePath, "DeveloperLog", "purgeRule", "0");
                bool Step3_6 = basepage.GetNodeValue(Config.webconfig, NodePath, "DeveloperLog", "UtcDatetime", "False");
                if (Step3_1 && Step3_2 && Step3_3 && Step3_4 && Step3_5 && Step3_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4: Goto the log path and verify that a subfolder is created based on given log file name with 
                //current date say as "WebAccessDeveloper-20160502" and under the folder a log file is created based 
                //on given file name with Current date and hour in 24 format starts with 00 say as "WebAccessDeveloper-20160502-00(1).log"
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                DateTime localDate4 = DateTime.Now;
                login.DriverGoTo(login.url);
                String format4 = "yyyyMMdd";
                string d = localDate4.ToString(format4);
                String hourFormat = "HH";
                string t = localDate4.ToString(hourFormat);
                String DirectoryPath = validationPath[0] + Path.DirectorySeparatorChar + "WebAccessDeveloper-" + d;
                String FilePath = validationPath[0] + "\\WebAccessDeveloper-" + d + Path.DirectorySeparatorChar + "WebAccessDeveloper-" + d + "-";
                bool step4_1 = Directory.Exists(DirectoryPath);
                bool step4_2 = File.Exists(FilePath + t + "(1).log");
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step 5: Leave ICA server running for few hours [or change system time for testing]and Verify that for every one hour, a log file is created under the subfolder "WebAccessDeveloper-20160502" with date and time say as 
                //WebAccessDeveloper - 20160502 - 01(1).log,WebAccessDeveloper - 20160502 - 02(1).log, WebAccessDeveloper - 20160502 - 03(1).log
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                studies = (Studies)login.Navigate("Studies");
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                login.Logout();
                //new log1
                DateTime localDate5_1 = DateTime.Now.AddHours(1);
                String format5_1 = "hh:mm:ss tt";
                string Time = localDate5_1.ToString(format5_1);
                String format5_2 = "MM/dd/yyyy";
                string Date = localDate5_1.ToString(format5_2);
                BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + Date);
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + Time);

                DateTime localDate5_2 = DateTime.Now;
                string t5_1 = localDate5_2.ToString(hourFormat);
                login.DriverGoTo(login.url);
                bool step5_1 = File.Exists(FilePath + t5_1 + "(1).log");

                //new log2
                DateTime localDate5_3 = DateTime.Now.AddHours(1);
                Time = localDate5_3.ToString(format5_1);
                Date = localDate5_3.ToString(format5_2);
                BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + Date);
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + Time);
                DateTime localDate5_4 = DateTime.Now;
                string t5_2 = localDate5_4.ToString(hourFormat);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step5_2 = File.Exists(FilePath + t5_2 + "(1).log");

                if (step5_1 && step5_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 6:From iCA webaccess, perform many functions like study viewing, study search etc upto the 
                //log size exceeds the setlimit of 1 MB and Verify the log file.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Developer Logs");
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.WaitWhileBusy();
                servicetool.LogType().SetValue("WebAccess Developer");//Anonymous STS
                servicetool.LogPath().SetValue("c:\\temp\\WebAccessDeveloper.log");
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                Thread.Sleep(2000);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                StreamWriter writer = new StreamWriter(FilePath + t5_2 + "(1).log");
                string[] text = File.ReadAllLines(TempLog);
                foreach (string line in text)
                {
                    writer.WriteLine(line);
                }
                writer.Close();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Developer Logs");
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.WaitWhileBusy();
                servicetool.LogType().SetValue("WebAccess Developer");//Anonymous STS
                servicetool.LogPath().SetValue(validationPath[0] + "\\WebAccessDeveloper.log");
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                Thread.Sleep(2000);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                int count = 0;
                while (count < 10)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        login.DriverGoTo(login.url);
                        login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                        studies = (Studies)login.Navigate("Studies");
                        studies.SearchStudy("Accession", Accession[0]);
                        studies.SelectStudy("Accession", Accession[0]);
                        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                        {

                            var viewer = BluRingViewer.LaunchBluRingViewer();
                            PageLoadWait.WaitForPageLoad(20);
                            viewer.CloseBluRingViewer();
                        }
                        else
                        {
                            StudyViewer.LaunchStudy();
                            PageLoadWait.WaitForPageLoad(10);
                            StudyViewer.CloseStudy();
                        }
                        studies.SearchStudy("Accession", Accession[1]);
                        studies.SelectStudy("Accession", Accession[1]);
                        if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                        {

                            var viewer = BluRingViewer.LaunchBluRingViewer();
                            PageLoadWait.WaitForPageLoad(20);
                            viewer.CloseBluRingViewer();
                        }
                        else
                        {
                            StudyViewer.LaunchStudy();
                            PageLoadWait.WaitForPageLoad(10);
                            StudyViewer.CloseStudy();
                            PageLoadWait.WaitForPageLoad(10);
                        }
                        login.Logout();
                    }
                    if (File.Exists(FilePath + t5_2 + "(2).log"))
                    {
                        break;
                    }
                    count++;
                }
                bool step6_1 = File.Exists(FilePath + t5_2 + "(2).log");
                if (step6_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                /*DateTime localDate = DateTime.Now;
                System.DateTime now = System.DateTime.Now;
                String format = "MM/dd/yyyy";
                string t1 = now.ToString(format);
                string[] st2 = t1.Split(' ');*/


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

                //Return Result
                return result;

            }
            finally
            {
                try
                {
                    servicetool.LaunchServiceTool();
                    servicetool.NavigateToTab("Developer Logs");
                    wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                    servicetool.WaitWhileBusy();
                    servicetool.LogType().SetValue("WebAccess Developer");
                    servicetool.Creationrule().SetValue("Daily");
                    servicetool.FileSize().SetValue("1");
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.CloseServiceTool();
                    basepage.ChangeAttributeValue(Config.WebConfigPath, "/sharedListeners/add[@name='DeveloperLog']", "initializeData", @"C:\temp\webdeveloper.log");
                    servicetool.RestartIISUsingexe();
                    System.IO.DirectoryInfo di = new DirectoryInfo(validationPath[0]);
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    basepage.ChangeAttributeValue(Config.WebConfigPath, "/sharedListeners/add[@name='DeveloperLog']", "initializeData", @"C:\Windows\Temp\WebAccessDeveloper.log");
                    servicetool.RestartIISUsingexe();

                }
                catch (Exception) { }
                try
                {
                    string[] currentdatetime = basepage.GetCurrentDateAndTimeFromInternet().Split(' ');
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + currentdatetime[0]);
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + currentdatetime[1]);
                }
                catch (Exception) { }
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
