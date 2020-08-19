using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.IO;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using System.Configuration;

namespace Selenium.Scripts.Tests
{
    class AuthenticationandAuthorization : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());

        public AuthenticationandAuthorization(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163542(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 ::Z3D is installed in iCA server
                result.steps[++ExecutedSteps].status = "Pass";
                //Step2 :: Launch iCA service tool -> Security tab -> General.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.ClickModifyButton();
                wpfobject.SetSpinner(ServiceTool.Spinner_ID, "2");
                Thread.Sleep(3000);
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].StepPass();
                //Step3 :: Log in to iCA and navigate to studies tab.
                //Step4 :: Search and load a 3D supported study in universal viewer.
                //Step5 :: From the Universal viewer , Select a 3D supported series and Select the MPR view option from the smart view drop down. 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings: "No");
                if (step1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step6 :: Wait for 2 minutes without making any intereactions.
                PauseTimer pt = new PauseTimer();
                pt.PauseExecution(2);
                //Verification :: Z3D session is timed out and user is logged out of iCA
                Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.UserNamefield)));
                string ErrMsg = Driver.FindElement(By.Id(Locators.ID.ErrMsg)).Text;
                if (ErrMsg.Equals("You have not logged in yet or your session has expired. Please log in again."))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step7 :: Change the value of "User web session timeout" back to default value of 30 minutes. Save changes and restart IIS and windows service.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.ClickModifyButton();
                wpfobject.SetSpinner(ServiceTool.Spinner_ID, "30");
                Thread.Sleep(3000);
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].StepPass();
                //Step8 :: Login to iCA and launch any study in 3D viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step8 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings : "No");
                if (step8)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step9 :: Leave session inactive for more than 10 minutes and try to perform any actions
                pt.PauseExecution(10);
                z3dvp.CloseViewer();
                //Verification :: Z3D session is not timed out
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                try
                {
                    servicetool.LaunchServiceTool();
                    servicetool.NavigateToTab("Security");
                    servicetool.ClickModifyButton();
                    wpfobject.SetSpinner(ServiceTool.Spinner_ID, "30");
                    Thread.Sleep(3000);
                    servicetool.ClickApplyButtonFromTab();
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    servicetool.CloseServiceTool();
                }catch(Exception ex)
                {
                    Logger.Instance.ErrorLog("Error in Reverting Study time out in case 163542" + ex.ToString());
                }
            }
        }

        public TestCaseResult Test_163543(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            UserManagement Users = new UserManagement();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String user1 = TestDataRequirements.Split('|')[0];
            String user2 = TestDataRequirements.Split('|')[1];
            String user3 = TestDataRequirements.Split('|')[2];
            String user4 = TestDataRequirements.Split('|')[3];
            String SuperRole = TestDataRequirements.Split('|')[4];
            String SuperAdminGroup = TestDataRequirements.Split('|')[5];
            String UserPath = TestDataRequirements.Split('|')[6];
            String AdminPath = TestDataRequirements.Split('|')[7];
            String AppgatePath = TestDataRequirements.Split('|')[8];
            String Stat1000Path = TestDataRequirements.Split('|')[9];
            String ReleaseclientAffinity = TestDataRequirements.Split('|')[10];//ReleaseClientAffinity
            String DropAllVolumes = TestDataRequirements.Split('|')[11];//DropAllVolumes() - dropped 1 volume(s)
            String Free3DResources = TestDataRequirements.Split('|')[12];//Free3DResources All volumes dropped
            String Closinglog = TestDataRequirements.Split('|')[13];//Closing log
            String Duration = TestDataRequirements.Split('|')[14];//Duration
            String ModalityVolumes = TestDataRequirements.Split('|')[15];//Modality volumes: CT
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Pre-Condition :: Create 4 Users(User1 , User2 , User3 , User4)
                //var config = ConfigurationManager.OpenExeConfiguration(@"C:\drs\wwwroot\WebAccess3D\custom3D.config");
                login.ChangeAttributeValue(@"C:\drs\wwwroot\DRPACS\Z3D\custom.config", "appSettings" ,"LogLevel", "2", true);
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                servicetool.EnablePatient();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Users = (UserManagement)login.Navigate("UserManagement");
                Users.CreateUser(user1, SuperAdminGroup, SuperRole);
                PageLoadWait.WaitForFrameLoad(10);
                Users.CreateUser(user2, SuperAdminGroup, SuperRole);
                PageLoadWait.WaitForFrameLoad(10);
                Users.CreateUser(user3, SuperAdminGroup, SuperRole);
                PageLoadWait.WaitForFrameLoad(10);
                Users.CreateUser(user4, SuperAdminGroup, SuperRole);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                //Step1 :: Launch the iCA web page in First browser.
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                //Verification :: Log in page should be displayed.
                IWebElement UserName = Driver.FindElement(By.CssSelector(Locators.CssSelector.UserNamefield));
                if (UserName.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step2 :: Log in as User 1.
                //Step3 :: From iCA, Load a study in the Z3D viewer from the universal viewer.
                login.LoginIConnect(user1, user1);
                bool step2and3 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings: "No");
                if (step2and3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step4 :: Launch two iCA sessions in the separate browser.
                //Step5 :: Log in as User 2 == Second Browser and User3 == Third Browser
                //Step6 :: Load a study in the Z3D viewer from universal viewer.
                BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(user2, user2);
                Thread.Sleep(5000);
                bool step456_1 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings: "No");
                // Config.node = "10.9.39.190";
                //BasePage.MultiDriver.Add(login.InvokeBrowser("remote-chrome"));
                BasePage.MultiDriver.Add(login.InvokeBrowser("ie"));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(user3, user3);
                Thread.Sleep(5000);
                bool step456_2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings: "No");
                if (step456_1 && step456_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step7 :: Launch the iCA web page in the Fourth browser.
                //Step8 :: Log in as User 4
                //Step9 :: From iCA, Load a study in the Z3D viewer from universal viewer..
                //Config.node = "10.9.39.190";
                //BasePage.MultiDriver.Add(login.InvokeBrowser("remote-wires-firefox"));
                BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                login.SetDriver(BasePage.MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(user4, user4);
                Thread.Sleep(5000);
                //bool step789 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                String FieldName = z3dvp.GetFieldName("patient");
                login.SearchStudy("patient", Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool thumbnailselction = z3dvp.selectthumbnail(ImageCount, 0);
                if (thumbnailselction)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step10 :: Close any one of the Active Z3D session from User 1 or User 2 or User 3 .
                login.SetDriver(BasePage.MultiDriver[0]);
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step11 ::  From the User 4, load a study in Z3D viewer. 
                login.SetDriver(BasePage.MultiDriver[3]);
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, "y");
                if (res)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step12 :: Log in to iCA viewer as Administrator. Navigate to studies tab. Search and load a study that has 3D supported study in the universal viewer.
                //Step13 :: Select a 3D supported series and load it in the 3D viewer.
                BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                login.SetDriver(BasePage.MultiDriver[4]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step12and13 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings: "No");
                if (step12and13)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
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
                //Step14 :: Click on the close button from the top corner of the universal viewer.
                z3dvp.CloseViewer();
                Thread.Sleep(5000);
                //Verification :: Universal viewer should be closed
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step15 ::  Examine the following logs in the server side. C:\Drs\LOGS\
                //Web_PACS_Z3.log issue in this log file=====session id is not available inside the file====
                String WebPacsZ3d = @"C:\Drs\LOGS\STAT1000\Web_PACS_Z3.log";
                DateTime today = DateTime.Today;
                string ServerDateFormat = (Convert.ToDateTime(today)).ToString("MM/dd/yy").ToString();
                string[] readText = File.ReadAllLines(WebPacsZ3d);
                int Step15_1 = 0;
                for (int i = 0; i < readText.Length; i++)
                {
                    if (readText[i].Contains(ServerDateFormat))
                    {
                        if (readText[i].Contains("Administrator") && readText[i].Contains("Free3DSession") && readText[i].Contains("ProcessRequest") && readText[i].Contains("SessionID"))
                        {
                            Step15_1++;
                        }

                    }
                }

                readText = File.ReadAllLines(AppgatePath);
                int Step15_2 = 0;
                for (int i = 0; i < readText.Length; i++)
                {
                    if (readText[i].Contains(ServerDateFormat))
                    {
                        if(readText[i].Contains(ReleaseclientAffinity))
                        {
                            Step15_2++;
                        }
                       
                    }
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(Stat1000Path);
                //var AppGateClient = directoryInfo.GetFiles("Z3DGate*.log", SearchOption.AllDirectories).OrderBy(t => t.LastWriteTime).ToList();
                var AppGateClient = directoryInfo.GetFiles("Z3DGate*.log", SearchOption.AllDirectories).OrderBy(t => t.LastWriteTime).ToList();
                String pathname = AppGateClient[AppGateClient.Count - 1].FullName;
                string[] readText1 = File.ReadAllLines(pathname);
                int Step15_3 = 0;
                for (int i = 0; i < readText1.Length; i++)
                {
                    if (readText1[i].Contains(ServerDateFormat))
                    {
                        if (readText1[i].Contains(DropAllVolumes) || readText1[i].Contains(Free3DResources))
                        {
                            Step15_3++;
                        }
                    }
                    
                }
                Logger.Instance.InfoLog("Step15_1 : "+ Step15_1+ " Step15_2 :"+ Step15_2+ " Step15_3 :"+ Step15_3);
                if(Step15_1 > 0 && Step15_2 > 0 && Step15_3 > 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step16 :: Examine the following logs in the server side.C:\drs\logs\users\<your logged in user name>\Z3d Integrated ICA ClientViewer.log
                string[] User1Text = File.ReadAllLines(UserPath);
                int counter2 = 0;
                for (int i = 0; i < User1Text.Length; i++)
                {
                    if (User1Text[i].Contains(ServerDateFormat))
                    {
                        if (User1Text[i].Contains(Closinglog) || User1Text[i].Contains(Duration) || User1Text[i].Contains(ModalityVolumes))
                        {
                            counter2++;
                            break;
                        }

                    }
                }
                string[] AdminText = File.ReadAllLines(AdminPath);
                int counter3 = 0;
                for (int i = 0; i < AdminText.Length; i++)
                {
                    if (AdminText[i].Contains(ServerDateFormat))
                    {
                        if (AdminText[i].Contains(Closinglog) || AdminText[i].Contains(Duration) || AdminText[i].Contains(ModalityVolumes))
                        {
                            counter3++;
                            break;
                        }

                    }
                }
                if (counter2 > 0 && counter3 > 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                closeallbrowser();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                login.InvokeBrowser("chrome");
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate<UserManagement>();
                Users.DeleteUser(SuperAdminGroup, user1);
                Users.DeleteUser(SuperAdminGroup, user2);
                Users.DeleteUser(SuperAdminGroup, user3);
                Users.DeleteUser(SuperAdminGroup, user4);
                login.Logout();
                login.ChangeAttributeValue(@"C:\drs\wwwroot\DRPACS\Z3D\custom.config", "appSettings", "LogLevel", "0", true);
                //After changing the custom3D.config file need to restart the services
                RestartIISUsingexe();
            }
        }

        public TestCaseResult Test_163544(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string ErrorMsg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //PreCondition :: 
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Security");
                servicetool.ClickModifyButton();
                wpfobject.SetSpinner(ServiceTool.Spinner_ID, "2");
                Thread.Sleep(3000);
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Step1 :: Login in iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step2 :: Navigate to studies tab.
                //Step3 :: Search a study with below Criteria::CT,MR and PT 
                //Step4 :: Load a 3D supported study in universal viewer.
                //Step5 :: Select a 3D supported series and Select the MPR view option from the smart view drop down. 
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings: "No");
                if (step1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step6 :: Keep the Z3D session idle for more than two minutes.
                PauseTimer pt = new PauseTimer();
                pt.PauseExecution(2);
                Driver.SwitchTo().DefaultContent();
                //Verification :: Z3D session should be closed.
                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.UserNamefield)));
                }
                catch(Exception ex)
                { }
                string ErrMsg = Driver.FindElement(By.Id(Locators.ID.ErrMsg)).Text;
                if (ErrMsg.Equals(ErrorMsg))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step7 :: Login in iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step8 :: Navigate to studies tab.
                //Step9 :: Search a study with below Criteria: 
                //Step10 :: Load a 3D supported study in universal viewer.
                //Step11 :: Select the MPR view option from the smart view drop down.
                bool step11 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings: "No");
                if (step11)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step12 :: Keep the Z3D session Interactive for more than two minutes by Applying various tool operation to the controls.
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 2, Nav1.Size.Width / 2, Nav1.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).ClickAndHold().
                //    MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 2).Release().Build().Perform();
                z3dvp.select3DTools(Z3DTools.Window_Level);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 2, Nav1.Size.Width / 2, Nav1.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).ClickAndHold().
                //       MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 2).Release().Build().Perform();
                z3dvp.select3DTools(Z3DTools.Pan);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 2, Nav1.Size.Width / 2, Nav1.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).ClickAndHold().
                //    MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 2).Release().Build().Perform();
                bool Reset = z3dvp.select3DTools(Z3DTools.Reset);
                if (Reset)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step13 :: Click on the close button from the Global tool bar.
                z3dvp.CloseViewer();
                //Verification :: Z3D session is not timed out
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step14 :: Log out iCA.
                login.Logout();
                Thread.Sleep(3000);
                Driver.SwitchTo().DefaultContent();
                //Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.UserNamefield)));
                IWebElement UserLogin = Driver.FindElement(By.CssSelector(Locators.CssSelector.UserNamefield));
                if (UserLogin.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

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
                    servicetool.NavigateToTab("Security");
                    servicetool.ClickModifyButton();
                    wpfobject.SetSpinner(ServiceTool.Spinner_ID, "30");
                    Thread.Sleep(3000);
                    servicetool.ClickApplyButtonFromTab();
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    servicetool.CloseServiceTool();
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Error in reverting Study time out in case 163544 " + ex.ToString());
                }
                login.Logout();
            }
        }

        public TestCaseResult Test_164660(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingViewer Viewer = new BluRingViewer();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patient = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Patientid = Patient.Split('|')[0];
            String Acc1 = Patient.Split('|')[1];
            String Acc2 = Patient.Split('|')[2];
            string ThumbnailDesc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String Series6 = ThumbnailDesc.Split('|')[0];
            String Series7 = ThumbnailDesc.Split('|')[1];
            String Series3 = ThumbnailDesc.Split('|')[2];
            String Series4 = ThumbnailDesc.Split('|')[3];
            string PatientDetail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Pt1Name = PatientDetail.Split('|')[0];
            String Pt1Gender = PatientDetail.Split('|')[1];
            String Pt1DOB = PatientDetail.Split('|')[2];
            String Pt1Series6 = PatientDetail.Split('|')[3];
            String Pt1Series7 = PatientDetail.Split('|')[4];
            String Pt2Name = PatientDetail.Split('|')[5];
            String Pt2Gender = PatientDetail.Split('|')[6];
            String Pt2DOB = PatientDetail.Split('|')[7];
            String Pt2Series3 = PatientDetail.Split('|')[8];
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                z3dvp.DeletePriorsInEA("10.9.37.82", "2009000041", Acc2);
                //Step 1 ::Search and load a 3D supported study with multiple valid 3D series in the universal viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                String FieldName = z3dvp.GetFieldName("patient");
                login.SearchStudy("patient", Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: Study is available under the Exam lists on the left hand side.
                IWebElement StudyExamlist = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyExamlist));
                if(StudyExamlist.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: Click on the study in Exam list to load the same study in the second panel.
                Viewer.OpenPriors(0);
                //Verification::Same study loaded in the second study panel.
                IList<IWebElement> PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if(PanelCount.Count.Equals(2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3:: From the first study panel thumbnail bar, Select and load 3D supported series (Eg : series 6) in the Active viewport. Select the MPR option from the smart view drop down.
                //Viewer.SetViewPort(0, 1);
                //BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).Click();
                Click("cssselector", Locators.CssSelector.FirstPanelFirstViewport);
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series6);
                bool MPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR , panel: 1);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 :: Verify the DICOM annotations over the images in 3D controls.
                string NavAnnotation = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                string NavSeriesNo = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                if(NavAnnotation.Contains(Pt1Name) && NavAnnotation.Contains(Pt1Gender) && NavAnnotation.Contains(Pt1DOB) && NavSeriesNo.Contains(Pt1Series6))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: From the second panel thumbnail bar, Select and load the same series (Eg : series 6) in the Active viewport and verify the smart view drop down.
                IWebElement SecondPanelnav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondPanelnav1).Click().Build().Perform();
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series6, panel:2);
                IList<IWebElement> ThreeD = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdown));
                if (ThreeD[1].GetAttribute("aria-disabled").Equals("true"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: From the second panel thumbnail bar, Select and load a different series (Eg : series 7) in the Active viewport and Select the MPR option from the smart view drop down.
                SecondPanelnav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondPanelnav1).Click().Build().Perform();
                SwitchToDefault();
                SwitchToUserHomeFrame();
                //z3dvp.selectthumbnail(Series7 , panel:2);
                z3dvp.DragandDropThumbnail("S7", "MR", "64", SecondPanelnav1, 2);
                SecondPanelnav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondPanelnav1).Click().Build().Perform();
                MPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 2);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Verify the DICOM annotations over the images in 3D controls.
                IWebElement NavigationElement = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                String Navtopright = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightTop)).GetAttribute("innerHTML");
                String Navtopleft = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (Navtopright.Contains(Pt1Name) && Navtopright.Contains(Pt1Gender) && Navtopright.Contains(Pt1DOB) && Navtopleft.Contains(Pt1Series7))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: Ensure that both the study panels has loaded in the 3D view.
                NavigationElement = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                NavSeriesNo = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Navtopleft = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (NavSeriesNo.Contains(Pt1Series6) && Navtopleft.Contains(Pt1Series7))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: Select the 3D 4:1 layout from the smart view drop down of the First study panel.
                bool  ThreeD4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, panel: 1);
                bool Result = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel , panel:2).Text.Contains(BluRingZ3DViewerPage.ResultPanel);
                if(ThreeD4x1 && Result)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10:: Select the 3D 6:1 layout from the smart view drop down of the Second panel. 
                bool ThreeD6x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, panel: 2);
                IList<IWebElement> tilelist = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(1)" + " " + z3dvp.ControlViewContainer + " " + Locators.CssSelector.ControlImage));
                if(ThreeD6x1 && tilelist.Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 :: Select the Curved MPR layout from the smart view drop down of the First study panel.
                bool CurvedMpr = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, panel: 1);
                bool ThreeD2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2, panel: 2).Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                if (CurvedMpr && ThreeD2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 :: Select the Calcium scoring layout from the smart view drop down of the Second panel.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring, panel: 2);
                z3dvp.checkerrormsg("y");
                bool NavCurvedmpr = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR, panel: 1).Text.Contains(BluRingZ3DViewerPage.CurvedMPR);
                bool CalciumScoring = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring, panel: 2).Text.Contains(BluRingZ3DViewerPage.CalciumScoring);
                if (CalciumScoring && NavCurvedmpr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: Select the 2D from the smart view drop down of the First study panel.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Two_2D, panel: 1);
                IList<IWebElement> viewmode = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdownText));
                string Viewmodetext = viewmode[0].GetAttribute("innerText");
                bool NavCalcium = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring, panel: 2).Text.Contains(BluRingZ3DViewerPage.CalciumScoring);
                //Handling Calcium tool Box
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (Viewmodetext.Equals(BluRingZ3DViewerPage.Two_2D) && NavCalcium)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14 :: From the first study panel thumbnail bar, Select and load a series which is currently loaded in 3D of second panel (Eg : series 7) in the Active viewport and verify the smart view drop down.
                IWebElement FirstPanelnav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstPanelnav1).Click().Build().Perform();
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series7);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                ThreeD = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdown));
                Logger.Instance.InfoLog("Get Attribute value : " + ThreeD[0].GetAttribute("aria-disabled"));
                if (ThreeD[0].GetAttribute("aria-disabled").ToString().Equals("true"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 :: Select the 2D from the smart view drop down of the Second study panel.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Two_2D, panel: 2);
                viewmode = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdownText));
                Viewmodetext = viewmode[1].GetAttribute("innerText");
                if (Viewmodetext.Equals(BluRingZ3DViewerPage.Two_2D))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: From the first study panel thumbnail bar, Select a series which is previously loaded in 3D of second panel (Eg : series 7) in the Active viewport and verify the smart view drop down.
                FirstPanelnav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstPanelnav1).Click().Build().Perform();
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series6);
                //z3dvp.selectthumbnail(Series7);
                MPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 1);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17 ::Click on the exit button from global tool bar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 18 :: Search and load a 3D supported study that has priors in the universal viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                FieldName = z3dvp.GetFieldName("Accession");
                login.SearchStudy("Accession", Acc1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName, Acc1);
                PageLoadWait.WaitForFrameLoad(5);
                viewer = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                StudyExamlist = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyExamlist));
                if (StudyExamlist.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 19 :: Select and load a prior study from the Exam lists panel.
                Viewer.OpenPriors(accession: Acc2);
                //Verification::Same study loaded in the second study panel.
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if (PanelCount.Count.Equals(2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 20 :: From the first study panel thumbnail bar, Select and load 3D supported series in the Active viewport . Select the MPR option from the smart view drop down.
                Viewer.SetViewPort(0, 1);
                BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).Click();
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series3);
                MPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 1);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 21 :: From the second study panel thumbnail bar, Select and load 3D supported series in the Active viewport . Select the MPR option from the smart view drop down.
                SecondPanelnav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondPanelnav1).Click().Build().Perform();
                SwitchToDefault();
                SwitchToUserHomeFrame();
                //z3dvp.selectthumbnail(Series4, panel: 2);
                SecondPanelnav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                //z3dvp.DragandDropThumbnail("S4", "CT", "465", SecondPanelnav1, panel: 2);
                PageLoadWait.WaitForFrameLoad(10);
                MPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 2);
                PageLoadWait.WaitForFrameLoad(10);                
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 22 :: Try switching to each and every viewing modes of the 3D viewer under both panels.
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 1)).Build().Perform();
                Thread.Sleep(2000);
                ThreeD4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, panel: 1);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2)).Build().Perform();
                Thread.Sleep(2000);
                ThreeD6x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, panel: 2);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 1)).Build().Perform();
                Thread.Sleep(2000);
                CurvedMpr = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, panel: 1);
                if (ThreeD4x1 && ThreeD6x1 && CurvedMpr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 23 :: Verify the DICOM annotations over the images in 3D controls.
                NavAnnotation = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                NavSeriesNo = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                if (NavAnnotation.Contains(Pt2Name) && NavAnnotation.Contains(Pt2Gender) && NavAnnotation.Contains(Pt2DOB) && NavSeriesNo.Contains(Pt2Series3))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 24 :: Click on the exit button from global tool bar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                  result.steps[++ExecutedSteps].status = "Pass";
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
            }
        }

        public TestCaseResult Test_164668(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            BluRingViewer Viewer = new BluRingViewer();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDesc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String Series6 = ThumbnailDesc.Split('|')[0];
            String Series7 = ThumbnailDesc.Split('|')[1];
            string ErrorMsg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //Step 1 ::Search and load a 3D supported study with multiple valid 3D series in the universal viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                String FieldName = z3dvp.GetFieldName("patient");
                login.SearchStudy("patient", Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: Study is available under the Exam lists on the left hand side.
                IWebElement StudyExamlist = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyExamlist));
                if (StudyExamlist.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: Click on the study in Exam list to load the same study in the second panel.
                Viewer.OpenPriors(0);
                //Verification::Same study loaded in the second study panel.
                IList<IWebElement> PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if (PanelCount.Count.Equals(2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3 :: From the first study panel thumbnail bar, Select and load 3D supported series (Eg : series 6) in the Active viewport. Select the MPR option from the smart view drop down.
                IWebElement FirstpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav1).Click().Build().Perform();
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series6);
                bool MPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 1);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 :: From the second panel thumbnail bar, Select and load a different series (Eg : series 7) in the Active viewport and verify. Select the MPR option from the smart view drop down.
                Thread.Sleep(10000);
                IWebElement SecondpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondpanelNav1).Click().Build().Perform();
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series7 , panel: 2);
                MPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 2);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: From the 3D viewer in the first panel, Select and apply the tools on the controls. Window level, scroll, Zoom, Pan, cut, tissue selection etc.
                IWebElement Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Nav1).SendKeys("X").Build().Perform();
                //==============================Window Level=============================================
                z3dvp.select3DTools(Z3DTools.Window_Level);
                //z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Panel1WLValue = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                String Panel2WLValue = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                //==============================Intractivezoom==============================================
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                String Panel1Zoom = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Panel2Zoom = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                //==============================Scrolling===============================================
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                String Panel1Scroll = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Panel2Scroll = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                //===============================Pan Tool==============================================
                z3dvp.select3DTools(Z3DTools.Pan);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                String Panel1pan = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Panel2pan = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                z3dvp.select3DTools(Z3DTools.Reset);
                //=============================PolygonCutTool=================================================
                z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                IList<IWebElement> ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                IWebElement Panel1Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Panel1Result = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                IWebElement Panel2Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                IWebElement Panel2Result = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel , panel: 2);

                int BeforeP1Nav1 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 3, 0, 0, 0, 2);
                int BeforeP1NavRes = z3dvp.LevelOfSelectedColor(Panel1Result, testid, ExecutedSteps + 4, 0, 0, 0, 2);
                int BeforeP2Nav1 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 5, 0, 0, 0, 2);
                int BeforeP2NavRes = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 6, 0, 0, 0, 2);

                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 5, (Nav1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width - 40, (Nav1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);

                int AfterP1Nav1 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 7, 0, 0, 0, 2);
                int AfterP1NavRes = z3dvp.LevelOfSelectedColor(Panel1Result, testid, ExecutedSteps + 8, 0, 0, 0, 2);
                int AfterP2Nav1 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 9, 0, 0, 0, 2);
                int AfterP2NavRes = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 10, 0, 0, 0, 2);

                bool Step5 = false;
                if(BeforeP1Nav1!=AfterP1Nav1 && BeforeP1NavRes!= AfterP1NavRes && BeforeP2Nav1== AfterP2Nav1 &&  BeforeP2NavRes== AfterP2NavRes)
                {
                    Step5 = true;
                }
                //==============================Tissue Selection Tool======================================================
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PageLoadWait.WaitForFrameLoad(5);
                int TissueSelectionP1B = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 11, 0, 0, 255, 2);
                int TissueSelectionP2B = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 12, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 6, Nav1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForFrameLoad(10);
                int TissueSelectionP1A = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 113, 0, 0, 255, 2);
                int TissueSelectionP2A = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 114, 0, 0, 255, 2);
                if (Panel1WLValue != Panel2WLValue && Panel1Zoom!= Panel2Zoom && Panel1Scroll!= Panel2Scroll && Panel1pan!= Panel2pan && Panel1Zoom!= Panel1Scroll && Panel1Scroll != Panel1pan
                    && Step5 && TissueSelectionP1B<TissueSelectionP1A && TissueSelectionP2B == TissueSelectionP2A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Reset);
                //Step 6 :: From the 3D viewer in the second panel, Select and apply the tools on the controls. Window level, scroll, Zoom, Pan, cut, tissue selection etc.
                Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone, panel:2);
                //==============================Window Level=============================================
                z3dvp.select3DTools(Z3DTools.Window_Level , panel:2);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                Panel1WLValue = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                Panel2WLValue = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                //==============================Intractivezoom==============================================
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom , panel:2);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                Panel1Zoom = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Panel2Zoom = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                //==============================Scrolling===============================================
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool , panel:2);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                Panel1Scroll = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Panel2Scroll = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                //===============================Pan Tool==============================================
                z3dvp.select3DTools(Z3DTools.Pan , panel:2);
                z3dvp.Performdragdrop(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4, Nav1.Size.Width / 2, (Nav1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                Panel1pan = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Panel2pan = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                z3dvp.select3DTools(Z3DTools.Reset , panel: 2);
                //=============================PolygonCutTool=================================================
                z3dvp.select3DTools(Z3DTools.Reset, panel: 2);
                z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon , panel:2);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                Panel1Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Panel1Result = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Panel2Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                Panel2Result = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel, panel: 2);

                BeforeP1Nav1 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 3, 0, 0, 0, 2);
                BeforeP1NavRes = z3dvp.LevelOfSelectedColor(Panel1Result, testid, ExecutedSteps + 4, 0, 0, 0, 2);
                BeforeP2Nav1 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 5, 0, 0, 0, 2);
                BeforeP2NavRes = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 6, 0, 0, 0, 2);

                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 5, (Nav1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width - 40, (Nav1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 2, Nav1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);

                AfterP1Nav1 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 7, 0, 0, 0, 2);
                AfterP1NavRes = z3dvp.LevelOfSelectedColor(Panel1Result, testid, ExecutedSteps + 8, 0, 0, 0, 2);
                AfterP2Nav1 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 9, 0, 0, 0, 2);
                AfterP2NavRes = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 10, 0, 0, 0, 2);
                bool Step6 = false;
                if (BeforeP1Nav1 == AfterP1Nav1 && BeforeP1NavRes == AfterP1NavRes && BeforeP2Nav1 != AfterP2Nav1 && BeforeP2NavRes != AfterP2NavRes)
                {
                    Step6 = true;
                }
                //==============================Tissue Selection Tool======================================================
                z3dvp.select3DTools(Z3DTools.Reset , panel:2);
                z3dvp.select3DTools(Z3DTools.Selection_Tool , panel:2);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PageLoadWait.WaitForFrameLoad(5);
                TissueSelectionP1B = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 11, 0, 0, 255, 2);
                TissueSelectionP2B = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 12, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Nav1, Nav1.Size.Width / 6, Nav1.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionP1A = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 13, 0, 0, 255, 2);
                TissueSelectionP2A = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 14, 0, 0, 255, 2);
                if (Panel1WLValue!=Panel2WLValue && Panel1Zoom!= Panel2Zoom && Panel1Scroll!= Panel2Scroll && Panel1pan!= Panel2pan && Step6 &&
                    TissueSelectionP1B == TissueSelectionP1A && TissueSelectionP2B< TissueSelectionP2A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Select the Reset button from the 3D tool box in first panel 3D viewer.
                bool Reset = z3dvp.select3DTools(Z3DTools.Reset);
                string Step7 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (Reset && Step7.Equals("Loc: 0.0, 0.0, 0.0 mm"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: Select the Reset button from the 3D tool box in second panel 3D viewer.
                z3dvp.select3DTools(Z3DTools.Reset , panel:2);
                Reset = z3dvp.select3DTools(Z3DTools.Reset , panel: 2);
                string Step8 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                if (Reset && Step8.Equals("Loc: 0.0, 0.0, 0.0 mm"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: Select the 3D 4:1 layout from the smart view drop down of the First study panel.
                bool Layout4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                bool Result = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel, panel: 2).Text.Contains(BluRingZ3DViewerPage.ResultPanel);
                if (Layout4x1 && Result)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
     //Step 10 :: From the 3D viewer in the first panel, Select and apply the tools on the controls Window level, scroll, Zoom, Pan, cut, tissue selection etc.
                IWebElement ThreeD1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //==============================Window Level=============================================
                z3dvp.select3DTools(Z3DTools.Window_Level);
                int WlBeforepanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                int WlBeforepane2 = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                //z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2, (ThreeD1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2, ThreeD1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int WLAfterpanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int WLAfterpanel2 = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 4, 0, 0, 0, 2);
                //==============================Intractive_zoom==============================================
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                int ZoomBeforepanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 5, 133, 133, 131, 2);
                int ZoomBeforepane2 = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                int ZoomAfterpanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int ZoomAfterpanel2 = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 8, 0, 0, 0, 2);
                //==============================Scrolling===============================================
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                string ScrollBeforepanel1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                string ScrollBeforepane2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel , panel:2);
                z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                string ScrollAfterpanel1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                string ScrollAfterpanel2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel , panel:2);
                //=============================PolygonCutTool=================================================
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ToolBox[0].Click();
                ToolBox[1].Click();
                int PolygoncutBeforeP1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 9, 133, 133, 131, 2);
                int PolygoncutBeforeP2 = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 10, 0, 0, 0, 2);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2 - 15, ThreeD1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 5, (ThreeD1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width - 40, (ThreeD1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2, ThreeD1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                int PolygoncutAfterP1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 11, 133, 133, 131, 2);
                int PolygoncutAfterP2 = z3dvp.LevelOfSelectedColor(Panel2Result, testid, ExecutedSteps + 12, 0, 0, 0, 2);
                //==============================Tissue Selection Tool======================================================
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PageLoadWait.WaitForFrameLoad(5);
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                TissueSelectionP1B = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 13, 0, 0, 255, 2);
                TissueSelectionP2B = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 14, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2 -50, ThreeD1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionP1A = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 17, 0, 0, 255, 2);
                TissueSelectionP2A = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 18, 0, 0, 255, 2);
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                if (WlBeforepanel1!=WLAfterpanel1 && WlBeforepane2== WLAfterpanel2 && ZoomBeforepanel1!= ZoomAfterpanel1 && ZoomBeforepane2== ZoomAfterpanel2 &&
                    !ScrollBeforepanel1.Equals(ScrollAfterpanel1) && ScrollBeforepane2.Equals(ScrollAfterpanel2) && PolygoncutBeforeP1!= PolygoncutAfterP1 &&
                    PolygoncutBeforeP2 == PolygoncutAfterP2 && TissueSelectionP1B!= TissueSelectionP1A && TissueSelectionP2B == TissueSelectionP2A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
        //Step 11 :: Select the 3D 4:1 layout from the smart view drop down of the Second study panel.
                Layout4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4 , panel:2);
                bool ThreeDcntrlp1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1).Text.Contains(BluRingZ3DViewerPage.Navigation3D1);
                bool ThreeDcntrlp2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1, panel: 2).Text.Contains(BluRingZ3DViewerPage.Navigation3D1);
                if (Layout4x1 && ThreeDcntrlp1 && ThreeDcntrlp2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 :: Adjust the clipping lines in 3D navigation controls in the first study panel 3D viewer.
                Panel1Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                ScrollBeforepane2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1 , panel:2);
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                DownloadImageFile(Panel1Nav1, BeforeImagePath);
                z3dvp.PerformDragAndDropWithDelay(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2, (Panel1Nav1.Size.Width - 220), Panel1Nav1.Size.Height / 2, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2 , 15);
                PageLoadWait.WaitForFrameLoad(10);
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                DownloadImageFile(Panel1Nav1, AfterImagePath);
                ScrollAfterpanel2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                if (!CompareImage(BeforeImagePath, AfterImagePath) && ScrollBeforepane2 == ScrollAfterpanel2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13:: Rotate the 3D hotspots (x,y,z) in 3D1 control in second study panel 3D viewer.
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, panel: 2);
                IWebElement Nav3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1, panel:2);
                string OrientationBeforeP2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                string OrientationBeforeP1 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.Performdragdrop(Nav3D1 , (Nav3D1.Size.Width - 218), Nav3D1.Size.Height / 2, (Nav3D1.Size.Width - 10) , Nav3D1.Size.Height/2 );
                string OrientationAfterP2 =z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1 , panel:2);
                string OrientationAfterP1 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (OrientationBeforeP2!= OrientationAfterP2 && OrientationBeforeP1.Equals(OrientationAfterP1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14 :: From the 3D viewer in the first panel, Select and apply the tools on the 3D 1 control.Window level, scroll, Zoom, Pan, cut, tissue selection etc.
                ThreeD1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ThreeD1P2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1 , panel:2);
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Reset , panel:2);
                //==============================Window Level=============================================
                z3dvp.select3DTools(Z3DTools.Window_Level);
                WlBeforepanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                WlBeforepane2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                //z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                WLAfterpanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                WLAfterpanel2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 4, 0, 0, 0, 2);
                //==============================Intractive_zoom==============================================
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                ZoomBeforepanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 5, 133, 133, 131, 2);
                ZoomBeforepane2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                //z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ZoomAfterpanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                ZoomAfterpanel2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 8, 0, 0, 0, 2);
                //==============================Scrolling===============================================
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                ScrollBeforepanel1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                ScrollBeforepane2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                //z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ScrollAfterpanel1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                ScrollAfterpanel2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                //=============================PolygonCutTool=================================================
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ToolBox[0].Click();
                ToolBox[1].Click();
                PolygoncutBeforeP1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 09, 133, 133, 131, 2);
                PolygoncutBeforeP2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 010, 0, 0, 0, 2);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2, ThreeD1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 5, (ThreeD1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width - 40, (ThreeD1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2, ThreeD1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                PolygoncutAfterP1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 11, 133, 133, 131, 2);
                PolygoncutAfterP2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 12, 0, 0, 0, 2);
                //==============================Tissue Selection Tool======================================================
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PageLoadWait.WaitForFrameLoad(5);
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                TissueSelectionP1B = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 13, 0, 0, 255, 2);
                TissueSelectionP2B = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 14, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2 - 50, ThreeD1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionP1A = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 18, 0, 0, 255, 2);
                TissueSelectionP2A = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 19, 0, 0, 255, 2);
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                if (WlBeforepanel1 != WLAfterpanel1 && WlBeforepane2 == WLAfterpanel2 && ZoomBeforepanel1 != ZoomAfterpanel1 && ZoomBeforepane2 == ZoomAfterpanel2 &&
                    !ScrollBeforepanel1.Equals(ScrollAfterpanel1) && ScrollBeforepane2.Equals(ScrollAfterpanel2) && PolygoncutBeforeP1 != PolygoncutAfterP1 &&
                    PolygoncutBeforeP2 == PolygoncutAfterP2 && TissueSelectionP1B != TissueSelectionP1A && TissueSelectionP2B == TissueSelectionP2A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 :: Select the 3D 6:1 layout from the smart view drop down of the First study panel.
                bool ThreeD6x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                ThreeDcntrlp1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2).Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                IList<IWebElement> weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(2)" + " " + z3dvp.ControlViewContainer + " " + Locators.CssSelector.ControlImage));
                if(ThreeD6x1 && ThreeDcntrlp1 && weli.Count.Equals(4))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: Rotate the 3D hotspots (x,y,z) in 3D1 and 3D 2 control in first study panel 3D viewer.
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                IWebElement Nav3D1P1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Nav3D2P1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);

                String BeforeOri3D1  = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String BeforeOri3D1P2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1 , panel:2);
                z3dvp.Performdragdrop(Nav3D1P1, (Nav3D1P1.Size.Width - 218), Nav3D1P1.Size.Height / 2, (Nav3D1P1.Size.Width - 10), Nav3D1P1.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                String AfterOri3D1 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String AfterOri3D1P2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1 , panel:2);

                String BeforeOri3D2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                String BeforeOri3D1P22 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                z3dvp.Performdragdrop(Nav3D2P1, (Nav3D2P1.Size.Width - 218), Nav3D2P1.Size.Height / 2, (Nav3D2P1.Size.Width - 10), Nav3D2P1.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                String AfterOri3D2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                String AfterOri3D1P22 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1 , panel:2);
                if(BeforeOri3D1!=AfterOri3D1 && BeforeOri3D1P2 == AfterOri3D1P2 && BeforeOri3D2 != AfterOri3D2 && BeforeOri3D1P22 == AfterOri3D1P22)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17 :: Select the 3D 6:1 layout from the smart view drop down of the second study panel.
                ThreeD6x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6 , panel:2);
                ThreeDcntrlp1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2 , panel:2).Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(1)" + " " + z3dvp.ControlViewContainer + " " + Locators.CssSelector.ControlImage));
                if (ThreeD6x1 && ThreeDcntrlp1 && weli.Count.Equals(6))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 18 :: Select the Toggle MPR/3D option in the 3D1 control hover bar in first study panel 3D viewer.
                bool Toggle3DMPR = z3dvp.ChangeViewMode();
                if(Toggle3DMPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 19 :: Select the Toggle MPR/3D option in the 3D1 control hover bar in second study panel 3D viewer.
                Toggle3DMPR = z3dvp.ChangeViewMode(panel:2);
                if(Toggle3DMPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 20 :: Adjust the clipping lines in 3D navigation controls in the second study panel 3D viewer.
                Panel2Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                IWebElement Panel23D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                IWebElement Panel23D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2, panel: 2);
                IWebElement Panel13D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Panel13D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                int ThreeD6x1D1_1 = z3dvp.LevelOfSelectedColor(Panel23D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                int ThreeD6x1D2_2 = z3dvp.LevelOfSelectedColor(Panel23D2, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int ThreeD6x1D1_3 = z3dvp.LevelOfSelectedColor(Panel13D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int ThreeD6x1D2_4 = z3dvp.LevelOfSelectedColor(Panel13D2, testid, ExecutedSteps + 4, 0, 0, 0, 2);
                z3dvp.PerformDragAndDropWithDelay(Panel2Nav1, Panel2Nav1.Size.Width / 2, Panel2Nav1.Size.Height / 2, (Panel2Nav1.Size.Width - 220), Panel2Nav1.Size.Height / 2, Panel2Nav1.Size.Width / 2, Panel2Nav1.Size.Height / 2, 15);
                PageLoadWait.WaitForFrameLoad(10);
                int ThreeD6x1D1_5 = z3dvp.LevelOfSelectedColor(Panel23D1, testid, ExecutedSteps + 5, 133, 133, 131, 2);
                int ThreeD6x1D2_6 = z3dvp.LevelOfSelectedColor(Panel23D2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int ThreeD6x1D1_7 = z3dvp.LevelOfSelectedColor(Panel13D1, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int ThreeD6x1D2_8 = z3dvp.LevelOfSelectedColor(Panel13D2, testid, ExecutedSteps + 8, 0, 0, 0, 2);
                if (ThreeD6x1D1_1!= ThreeD6x1D1_5 && ThreeD6x1D2_2!= ThreeD6x1D2_6 && ThreeD6x1D1_3 == ThreeD6x1D1_7 && ThreeD6x1D2_4 == ThreeD6x1D2_8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 21 :: Rotate the 3D hotspots (x,y,z) in 3D1 and 3D 2 control in second study panel 3D viewer.
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                Nav3D1P1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1 , panel:2);
                Nav3D2P1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2 , panel:2);

                BeforeOri3D1 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                BeforeOri3D1P2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                z3dvp.Performdragdrop(Nav3D1P1, (Nav3D1P1.Size.Width - 218), Nav3D1P1.Size.Height / 2, (Nav3D1P1.Size.Width - 10), Nav3D1P1.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                AfterOri3D1 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                AfterOri3D1P2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);

                BeforeOri3D2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                BeforeOri3D1P22 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                z3dvp.Performdragdrop(Nav3D2P1, (Nav3D2P1.Size.Width - 218), Nav3D2P1.Size.Height / 2, (Nav3D2P1.Size.Width - 10), Nav3D2P1.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                AfterOri3D2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                AfterOri3D1P22 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                if (BeforeOri3D1 == AfterOri3D1 && BeforeOri3D1P2 != AfterOri3D1P2 && BeforeOri3D2 == AfterOri3D2 && BeforeOri3D1P22 != AfterOri3D1P22)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 22 :: From the 3D viewer in the first panel, Select and apply the tools on the 3D 1 control. Window level, scroll, Zoom, Pan, cut, tissue selection etc.
                z3dvp.select3DTools(Z3DTools.Reset, panel: 2);
                ThreeD1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                ThreeD1P2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                //==============================Window Level=============================================
                z3dvp.select3DTools(Z3DTools.Window_Level);
                WlBeforepanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                WlBeforepane2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 2, 133, 133, 131, 2);
                //z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                WLAfterpanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                WLAfterpanel2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 4, 133, 133, 131, 2);
                //==============================Intractive_zoom==============================================
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                ZoomBeforepanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 5, 133, 133, 131, 2);
                ZoomBeforepane2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 6, 133, 133, 131, 2);
                z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                ZoomAfterpanel1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                ZoomAfterpanel2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 8, 133, 133, 131, 2);
                //==============================Scrolling===============================================
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                ScrollBeforepanel1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                ScrollBeforepane2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                z3dvp.Performdragdrop(ThreeD1, ThreeD1.Size.Width / 4, ThreeD1.Size.Height / 4, ThreeD1.Size.Width / 4, (ThreeD1.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                ScrollAfterpanel1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                ScrollAfterpanel2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                //=============================PolygonCutTool=================================================
                z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PolygoncutBeforeP1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 9, 133, 133, 131, 2);
                PolygoncutBeforeP2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 10, 133, 133, 131, 2);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2, ThreeD1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 5, (ThreeD1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width - 40, (ThreeD1.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2, ThreeD1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                PolygoncutAfterP1 = z3dvp.LevelOfSelectedColor(ThreeD1, testid, ExecutedSteps + 11, 133, 133, 131, 2);
                PolygoncutAfterP2 = z3dvp.LevelOfSelectedColor(ThreeD1P2, testid, ExecutedSteps + 12, 133, 133, 131, 2);
                //==============================Tissue Selection Tool======================================================
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Selection_Tool);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PageLoadWait.WaitForFrameLoad(5);
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                TissueSelectionP1B = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 13, 0, 0, 255, 2);
                TissueSelectionP2B = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 14, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(ThreeD1, ThreeD1.Size.Width / 2 - 50, ThreeD1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionP1A = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 18, 0, 0, 255, 2);
                TissueSelectionP2A = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 19, 0, 0, 255, 2);
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                if (WlBeforepanel1 != WLAfterpanel1 && WlBeforepane2 == WLAfterpanel2 && ZoomBeforepanel1 != ZoomAfterpanel1 && ZoomBeforepane2 == ZoomAfterpanel2 &&
                    !ScrollBeforepanel1.Equals(ScrollAfterpanel1) && ScrollBeforepane2.Equals(ScrollAfterpanel2) && PolygoncutBeforeP1 != PolygoncutAfterP1 &&
                    PolygoncutBeforeP2 == PolygoncutAfterP2 && TissueSelectionP1B != TissueSelectionP1A && TissueSelectionP2B == TissueSelectionP2A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 23 :: From the 3D viewer in the second panel, Select and apply the tools on the 3D 2 control. Window level, scroll, Zoom, Pan, cut, tissue selection etc.
                z3dvp.select3DTools(Z3DTools.Reset);
                IWebElement ThreeD2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2 , panel:2);
                IWebElement ThreeD2Panel1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                //==============================Window Level=============================================
                z3dvp.select3DTools(Z3DTools.Window_Level , panel:2);
                int WLP2B = z3dvp.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 1, 64, 64, 63, 2);
                int WLP1B = z3dvp.LevelOfSelectedColor(ThreeD2Panel1, testid, ExecutedSteps + 2, 64, 64, 63, 2);
                //z3dvp.Performdragdrop(ThreeD2, ThreeD2.Size.Width / 4, ThreeD2.Size.Height / 4, ThreeD2.Size.Width / 4, (ThreeD2.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(ThreeD2, ThreeD2.Size.Width / 4, (ThreeD2.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD2, ThreeD2.Size.Width / 4, ThreeD2.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int WLP2A = z3dvp.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 3, 64, 64, 63, 2);
                int WLP1A = z3dvp.LevelOfSelectedColor(ThreeD2Panel1, testid, ExecutedSteps + 4, 64, 64, 63, 2);
                Logger.Instance.InfoLog("--->Step 23 Window Level---" + WLP2B.ToString() + ","+ WLP1B.ToString() + "," + WLP2A.ToString() + "," + WLP1A.ToString());
                //==============================Intractive_zoom==============================================
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom , panel: 2);
                int ZoomP2B = z3dvp.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 5, 64, 64, 63, 2);
                int ZoomP1B = z3dvp.LevelOfSelectedColor(ThreeD2Panel1, testid, ExecutedSteps + 6, 64, 64, 63, 2);
                z3dvp.Performdragdrop(ThreeD2, ThreeD2.Size.Width / 4, ThreeD2.Size.Height / 4, ThreeD2.Size.Width / 4, (ThreeD2.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                int ZoomP2A = z3dvp.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 7, 64, 64, 63, 2);
                int ZoomP1A = z3dvp.LevelOfSelectedColor(ThreeD2Panel1, testid, ExecutedSteps + 8, 64, 64, 63, 2);
                Logger.Instance.InfoLog("--->Step 23 Zoom Value---" + ZoomP2B.ToString() + "," + ZoomP1B.ToString() + "," + ZoomP2A.ToString() + "," + ZoomP1A.ToString());
                //==============================Scrolling===============================================
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool , panel:2);
                string ScrollP1B = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                string ScrollP2B = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2, panel: 2);
                z3dvp.Performdragdrop(ThreeD2, ThreeD2.Size.Width / 4, ThreeD2.Size.Height / 4, ThreeD2.Size.Width / 4, (ThreeD2.Size.Height) * 3 / 4);
                PageLoadWait.WaitForFrameLoad(10);
                string ScrollP1A = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                string ScrollP2A = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2, panel: 2);
                Logger.Instance.InfoLog("--->Step 23 Scrolling---" + ScrollP1B + "," + ScrollP2B + "," + ScrollP1A + "," + ScrollP2A);
                //=============================PolygonCutTool=================================================
                z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PolygoncutBeforeP2 = z3dvp.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 9, 64, 64, 63, 2);
                PolygoncutBeforeP1 = z3dvp.LevelOfSelectedColor(ThreeD2Panel1, testid, ExecutedSteps + 10, 64, 64, 63, 2);
                new Actions(Driver).MoveToElement(ThreeD2, ThreeD2.Size.Width / 2, ThreeD2.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD2, ThreeD2.Size.Width / 5, (ThreeD2.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD2, ThreeD2.Size.Width - 40, (ThreeD2.Size.Height) * 3 / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(ThreeD2, ThreeD2.Size.Width / 2, ThreeD2.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                PolygoncutAfterP2 = z3dvp.LevelOfSelectedColor(ThreeD2, testid, ExecutedSteps + 11, 64, 64, 63, 2);
                PolygoncutAfterP1 = z3dvp.LevelOfSelectedColor(ThreeD2Panel1, testid, ExecutedSteps + 12, 64, 64, 63, 2);
                Logger.Instance.InfoLog("--->Step 23 Polygon---" + PolygoncutBeforeP2.ToString() + "," + PolygoncutBeforeP1.ToString() + "," + PolygoncutAfterP2.ToString() + "," + PolygoncutAfterP1.ToString());
                //==============================Tissue Selection Tool======================================================
                z3dvp.select3DTools(Z3DTools.Reset , panel:2);
                z3dvp.select3DTools(Z3DTools.Selection_Tool , panel:2);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                ClickElement(ToolBox[1]);
                PageLoadWait.WaitForFrameLoad(5);
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                TissueSelectionP1B = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 13, 0, 0, 255, 2);
                TissueSelectionP2B = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 14, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(ThreeD2, ThreeD2.Size.Width / 2 - 50, ThreeD2.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionP1A = z3dvp.LevelOfSelectedColor(PanelCount[0], testid, ExecutedSteps + 18, 0, 0, 255, 2);
                TissueSelectionP2A = z3dvp.LevelOfSelectedColor(PanelCount[1], testid, ExecutedSteps + 19, 0, 0, 255, 2);
                Logger.Instance.InfoLog("---->Step 23 Tissue Selection----" + TissueSelectionP1B.ToString() + "," + TissueSelectionP2B.ToString() + "," + TissueSelectionP1A.ToString() + "," + TissueSelectionP2A.ToString());
                z3dvp.select3DTools(Z3DTools.Reset, panel:2);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                if (WLP2B!= WLP2A && WLP1B == WLP1A && ZoomP2B!= ZoomP2A && ZoomP1B== ZoomP1A && ScrollP1B== ScrollP1A && ScrollP2B!= ScrollP2A &&
                    PolygoncutBeforeP2!= PolygoncutAfterP2 && /*PolygoncutBeforeP1 == PolygoncutAfterP1 &&*/ TissueSelectionP1B == TissueSelectionP1A && TissueSelectionP2B!= TissueSelectionP2A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 24 :: Select the Curved MPR layout from the smart view drop down of both the panels.
                bool Curvedpanel1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                bool Curvedpanel2 = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, panel:2);
                if (Curvedpanel1 && Curvedpanel2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step25 :: Using the curve drawing tool , create a path in the navigation controls of the 3D viewer in first panel.
                Panel1Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Panel2Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                int ColorValBefore1 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int ColorValBefore2 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                z3dvp.MoveAndClick(Panel1Nav1, (Panel1Nav1.Size.Width / 2) , (Panel1Nav1.Size.Height / 2));
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.MoveAndClick(Panel1Nav1, (Panel1Nav1.Size.Width / 2), (Panel1Nav1.Size.Height / 4));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_1 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 3, 0, 0, 255, 2);
                int ColorValAfter_2 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if (ColorValBefore1!= ColorValAfter_1 && ColorValBefore2 == ColorValAfter_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 26 :: Using the curve drawing tool , create a path in the navigation controls of the 3D viewer in second panel.
                int ColorValBefore11 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int ColorValBefore22 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                z3dvp.MoveAndClick(Panel2Nav1, (Panel2Nav1.Size.Width / 2), (Panel2Nav1.Size.Height / 2));
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.MoveAndClick(Panel2Nav1, (Panel2Nav1.Size.Width / 2), (Panel2Nav1.Size.Height / 4));
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_11 = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 3, 0, 0, 255, 2);
                int ColorValAfter_22 = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if (ColorValBefore11 == ColorValAfter_11 && ColorValBefore22 != ColorValAfter_22)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 27 :: Select the Reset button from the 3D tool box in second panel 3D viewer.
                int ColorValBeforeReset = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Reset = z3dvp.select3DTools(Z3DTools.Reset , panel:2);
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfterReset = z3dvp.LevelOfSelectedColor(Panel2Nav1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValBeforeReset!= ColorValAfterReset)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28 :: Select the Reset button from the 3D tool box in first panel 3D viewer.
                ColorValBeforeReset = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Reset = z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                ColorValAfterReset = z3dvp.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValBeforeReset != ColorValAfterReset)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 29 :: Select the Calcium scoring layout from the smart view drop down of both the panels.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.checkerrormsg("y");
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring , panel:2);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.checkerrormsg("y");
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                bool Panelcal1 = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring).Text.Contains(BluRingZ3DViewerPage.CalciumScoring);
                bool Panelcal2 = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring, panel: 2).Text.Contains(BluRingZ3DViewerPage.CalciumScoring);
                if (Panelcal1 && Panelcal2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 30 :: Apply the calcium scoring tool in the 3D viewer of the first study panel.
                IWebElement Calpan1 = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                IWebElement Calpan2 = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring , panel:2);
                int GreenColorBeforeP1 = z3dvp.LevelOfSelectedColor(Calpan1, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                int GreenColorBeforep2 = z3dvp.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 20, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan1, Calpan1.Size.Width / 2, (Calpan1.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan1, (Calpan1.Size.Width - 10), (Calpan1.Size.Height / 4)).
                                    MoveToElement(Calpan1, (Calpan1.Size.Width - 10), Calpan1.Size.Height / 2).
                                    MoveToElement(Calpan1, (Calpan1.Size.Width /2), Calpan1.Size.Height / 2).
                                    MoveToElement(Calpan1, Calpan1.Size.Width / 2, (Calpan1.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP1 = z3dvp.LevelOfSelectedColor(Calpan1, testid, ExecutedSteps + 30, 0, 255, 0, 2);
                int GreenColorAfterp2 = z3dvp.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 40, 0, 255, 0, 2);
                if(GreenColorBeforeP1!= GreenColorAfterP1 && GreenColorBeforep2 == GreenColorAfterp2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                z3dvp.select3DTools(Z3DTools.Reset);
                //Step 31 :: Apply the calcium scoring tool in the 3D viewer of the second study panel.
                GreenColorBeforeP1 = z3dvp.LevelOfSelectedColor(Calpan1, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                GreenColorBeforep2 = z3dvp.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 22, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4)).ClickAndHold().
                                   MoveToElement(Calpan2, (Calpan2.Size.Width - 10), (Calpan2.Size.Height / 4)).
                                   MoveToElement(Calpan2, (Calpan2.Size.Width - 10), Calpan2.Size.Height / 2).
                                   MoveToElement(Calpan2, (Calpan2.Size.Width / 2), Calpan2.Size.Height / 2).
                                   MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                GreenColorAfterP1 = z3dvp.LevelOfSelectedColor(Calpan1, testid, ExecutedSteps + 33, 0, 255, 0, 2);
                GreenColorAfterp2 = z3dvp.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 44, 0, 255, 0, 2);
                if (GreenColorBeforeP1 == GreenColorAfterP1 && GreenColorBeforep2 != GreenColorAfterp2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 32 :: Close and reopen the panel. Select the same series and verify.
                z3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                FieldName = z3dvp.GetFieldName("patient");
                login.SearchStudy("patient", Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                viewer = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                Viewer.OpenPriors(0);
                //Verification::Same study loaded in the second study panel.
                FirstpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav1).Click().Build().Perform();
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series6);
                PageLoadWait.WaitForFrameLoad(5);
                bool MPrFirstpanel = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 1);
                SecondpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondpanelNav1).Click().Build().Perform();
                Thread.Sleep(2000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                z3dvp.selectthumbnail(Series7, panel: 2);
                PageLoadWait.WaitForFrameLoad(5);
                bool MPrSecondpanel = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 2);
                if (MPrFirstpanel && MPrSecondpanel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

    }
}
