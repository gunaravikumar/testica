using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Reusable;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using System.IO;
using Dicom;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Selenium.Scripts.Pages.MergeServiceTool;

namespace Selenium.Scripts.Tests
{
    class AdministrationBlueRing : BasePage
    {
        public Login Login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ExamImporter { get; set; }
        public Inbounds Inbounds { get; set; }
        public UserPreferences userpref { get; set; }
        ServiceTool servicetool = new ServiceTool();
        public ExamImporter ei { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public AdministrationBlueRing(string classname)
        {
            Login = new Login();
            ExamImporter = new ExamImporter();
            Inbounds = new Inbounds();
            userpref = new UserPreferences();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        ///<summary>
        ///Images sharing: View Exam button in Inbounds/OutBounds Study List Page
        ///</summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162301(string testid, string teststeps, int stepcount )
        {
            //Declare and initialize variables 
            TestCaseResult result;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Fetch required Test data                       
                String Username = Config.ph1UserName;
                String Password = Config.ph1Password;
                string stUsername = Config.stUserName;
                string stPassword = Config.stPassword;
                String TempleteFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string DestinationEA = Config.DestEAsIp;
                string ExamImportfileLocation = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ExamImportfileLocation");
                string eiWindow = string.Concat("EI_142653", System.DateTime.Now.ToString("MMddHHmm"));

                //Precondition
                //Create New Dicom Image and Upload using EamImport from ST user
                string staff = BasePage.GetUniqueUserId("St_");
                
                Login.DriverGoTo(Driver.Url);
                Login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                UserManagement usermanagement = (UserManagement)Login.Navigate("UserManagement");
                usermanagement.CreateUser(staff, Config.adminGroupName, "Staff");
                Login.Logout();
                string newFileToUpload = CreateNewDicomStudy(TempleteFilePath);
                string FinalStudy2path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + newFileToUpload;
                string TemDirectory = BasePage.CopyFilesToTempFolders(newFileToUpload, FinalStudy2path, "TempDicom", true);
                string dESKTOP  = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string TempExamImportfileLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + ExamImportfileLocation;
                BasePage.DeleteAllFileFolder(TempExamImportfileLocation);
                basepage.CopyFiles(TemDirectory, TempExamImportfileLocation);

                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(Config.adminGroupName, eiWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Login = new Login();
                Login.DriverGoTo(Login.url);
                ei.eiWinName = eiWindow;
                string EIPath = ei.EI_Installation(Config.adminGroupName, eiWindow, Config.Inst1, Config.ph1UserName, Config.ph1Password);
                ExamImporter.EIDicomUpload(staff, staff, Config.Dest1, EIPath);
                string accession = BasePage.ReadDicomFile<String>(newFileToUpload, DicomTag.AccessionNumber);
                 
                //Login as physician 
                Login.LoginIConnect(Username, Password);
                Inbounds = (Inbounds)Login.Navigate("Inbounds");
                Inbounds.SearchStudy("Accession", accession);
                Dictionary<string, string> study1 = Inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Uploaded" });
                if (study1 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 2 - Set BlueRing as Default viewer in user Preference
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                // Step3 - 
                Inbounds = (Inbounds)Login.Navigate("Inbounds");
                Inbounds.SearchStudy("Accession", accession);
                Dictionary<string, string> study2 = Inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Uploaded" });
                if (study2 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 4 - "View Exam" button should be visible 
                IWebElement ViewButton = Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                if(ViewButton.Displayed)
                { 
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 5
                IWebElement HTML5Button = null;
                try
                {
                    HTML5Button = HTML5ViewStudyBtn();
                }
                catch(Exception ex)
                {
                    Logger.Instance.InfoLog("Unable to find the HTML 5 Button");
                }
                if(HTML5Button == null)
                {
                    result.steps[++ExecutedSteps].StepPass("HTML5 button is not displayed");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("HTML5 button is displayed");
                }

                //Step 6
                Inbounds.SelectStudy1("Accession", accession);
                BluRingViewer bluering=  BluRingViewer.LaunchBluRingViewer();
                bluering.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 7
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step 8
                Inbounds.SelectStudy1("Accession", accession);
                StudyViewer StudyViewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                result.steps[++ExecutedSteps].StepPass();

                //Step 9
                Login.Logout();
                Login.LoginIConnect(staff, staff);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step 10
                Outbounds outbounds = (Outbounds)Login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", accession);
                Dictionary<string, string> study3 = outbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Uploaded" });
                if (study3 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step -11
                IWebElement ViewButton2 = Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                if (ViewButton2.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 12
                HTML5Button = null;
                try
                {
                    HTML5Button = HTML5ViewStudyBtn();
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Unable to find the HTML 5 Button");
                }
                if (HTML5Button == null)
                {
                    result.steps[++ExecutedSteps].StepPass("HTML5 button is not displayed");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("HTML5 button is displayed");
                }

                //Step -13
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step -14
                outbounds = (Outbounds)Login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", accession);
                IWebElement ViewButton3 = Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                if (ViewButton3.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

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

        ///<summary>
        ///Image sharing and Grant Access : View Exam button in Inbounds/OutBounds Study List Page
        ///</summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162302(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            
            try
            {
                UserManagement usermanagement = new UserManagement();
                DomainManagement domain = new DomainManagement();
                RoleManagement rolemanagemnet = new RoleManagement();

                //Fetch required Test data                       
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String User1 = "USer_" + new Random().Next(1, 1000);
                String TempleteFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string DestinationEA = Config.DestEAsIp;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Pre-Condition
                Login.DriverGoTo(Login.url);
                Login.LoginIConnect(Username, Password);
                Login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("grant", 0);
                domain.ClickSaveDomain();
                rolemanagemnet = (RoleManagement)Login.Navigate("RoleManagement");
                rolemanagemnet.SearchRole(Config.adminRoleName, Config.adminGroupName);
                rolemanagemnet.SelectRole(Config.adminRoleName);
                rolemanagemnet.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagemnet.GrantAccessRadioBtn_Anyone().Click();
                rolemanagemnet.ClickSaveEditRole();
                Login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.SetCheckBoxInEditDomain("grant", 0);
                domain.ClickSaveNewDomain();
                rolemanagemnet = (RoleManagement)Login.Navigate("RoleManagement");
                rolemanagemnet.SearchRole(role1,domain1);
                rolemanagemnet.SelectRole(role1);
                rolemanagemnet.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagemnet.GrantAccessRadioBtn_Anyone().Click();
                rolemanagemnet.ClickSaveEditRole();
                Login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, domain1, role1);
                Login.Logout();

                //Step 1 
                Login.DriverGoTo(Login.url);
                Login.LoginIConnect(Username, Password);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                Studies study = (Studies)Login.Navigate("Studies");
                SearchStudy(AccessionNo: Accession);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3,4,5,6
                SelectStudy("Accession", Accession);
                study.ShareStudy(false, new string[] { User1 }, domainName: domain1);
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();

                //Step 7 
                Login.Logout();
                Login.LoginIConnect(User1, User1);
                Inbounds = (Inbounds)Login.Navigate("Inbounds");
                Inbounds.SearchStudy("Accession", Accession);
                Dictionary<string, string> study2 = Inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" });
                if (study2 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step 8
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step 9
                Inbounds = (Inbounds)Login.Navigate("Inbounds");
                Inbounds.SearchStudy("Accession", Accession);
                study2 = null;
                study2 = Inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" });
                if (study2 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step 10 
                IWebElement ViewButton = Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                if (ViewButton.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 11
                IWebElement HTML5Button = null;
                try
                {
                    HTML5Button = HTML5ViewStudyBtn();
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Unable to find the HTML 5 Button");
                }
                if (HTML5Button == null)
                {
                    result.steps[++ExecutedSteps].StepPass("HTML5 button is not displayed");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("HTML5 button is displayed");
                }

                //Step 12
                Inbounds.SelectStudy1("Accession", Accession);
                BluRingViewer BlueRing =  BluRingViewer.LaunchBluRingViewer();
                BlueRing.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 13
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step 14
                Inbounds = (Inbounds)Login.Navigate("Inbounds");
                Inbounds.SearchStudy("Accession", Accession);
                study2 = null;
                study2 = Inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" });
                if (study2 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step 15
                ViewButton = Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                if (ViewButton.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 16
                Inbounds.SelectStudy1("Accession", Accession);
                StudyViewer StudyViewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                result.steps[++ExecutedSteps].StepPass();

                //Step 17
                Login.Logout();
                Login.LoginIConnect(Username, Password);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step 18
                Outbounds outbounds = (Outbounds)Login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", Accession);
                Dictionary<string, string> study3 = outbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" });
                if (study3 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step-19
                ViewButton = Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
                if (ViewButton.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 20
                HTML5Button = null;
                try
                {
                    HTML5Button = HTML5ViewStudyBtn();
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Unable to find the HTML 5 Button");
                }
                if (HTML5Button == null)
                {
                    result.steps[++ExecutedSteps].StepPass("HTML5 button is not displayed");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("HTML5 button is displayed");
                }

                //Step 21
                outbounds.SelectStudy1("Accession", Accession);
                BlueRing = BluRingViewer.LaunchBluRingViewer();
                BlueRing.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 22
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.HTML4RadioBtn().Click();
                userpref.CloseUserPreferences();
                result.steps[++ExecutedSteps].StepPass();

                //Step 23
                outbounds = (Outbounds)Login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", Accession);
                study2 = null;
                study2 = outbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Shared" });
                if (study2 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step 24
                outbounds.SelectStudy1("Accession", Accession);
                StudyViewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.CloseStudy();
                result.steps[++ExecutedSteps].StepPass();

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
        /// View Exam button in Study List Page.
        /// </summary>
        public TestCaseResult Test_162300(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables               
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserPreferences userPrefer = new UserPreferences();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNumber");
                String[] Accession = AccessionList.Split(':');

                BluRingViewer viewer = new BluRingViewer();

                //step1 Launch the iCA application with a client browser.
                Login.DriverGoTo(Login.url);
                if (Login.UserIdTxtBox().Displayed && Login.PasswordTxtBox().Displayed &&
                    Login.LoginBtn().Displayed)
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

                //Step-2  Login to WebAccess site with any privileged user.
                Login.LoginIConnect(adminUserName, adminPassword);
                if (Login.IsTabPresent("Studies") && Login.IsTabPresent("Patients")
                    && Login.IsTabPresent("Domain Management"))
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

                //step3  Go to Options-> User Preferences-> and set the Default Viewer Setting as 'BluRing' and then click on 'Save' button.
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.BluringViewerRadioBtn().Click();
                userPrefer.CloseUserPreferences();
                ExecutedSteps++;

                //step4  Select Studies tab.
                var study = (Studies)Login.Navigate("Studies");
                ExecutedSteps++;

                //step5  Verify that the "View Exam" button should be displayed available at the bottom right of the study list page.
                IWebElement Button = BasePage.Driver.FindElement(By.CssSelector("div#ButtonsDiv"));
                bool step5_1 = Button.GetCssValue("float").Equals("right");
                int step5_2 = Convert.ToInt32(Button.Location.Y.ToString());
                int step5_3 = Convert.ToInt32(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_studySearchResult)).Location.Y.ToString());
                if (step5_1 && (step5_2 > step5_3))
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

                //step6 Ensure that the "HTML5 View" button should not present in study list page.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //step7 Select any Study and Click on 'View Exam' button study to load it.
                study.SearchStudy(AccessionNo: Accession[0]);
                study.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (viewer.IsElementPresent(By.CssSelector(BluRingViewer.userSettings_Icon)))
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

                //step8 Click on 'EXIT' button.
                viewer.CloseBluRingViewer();
                if (Login.IsTabPresent("Studies"))
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

                //step9 Go to Options-> User Preferences-> and set the Default Viewer Setting as 'HTML4' and then click on 'Save' button.
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.HTML4RadioBtn().Click();
                userPrefer.CloseUserPreferences();
                ExecutedSteps++;

                //step10 Select any Study and Click on 'View Exam' button study to load it.
                study.SearchStudy(AccessionNo: Accession[0]);
                study.SelectStudy("Accession", Accession[0]);
                StudyViewer StudyViewer = StudyViewer.LaunchStudy();
                if (!viewer.IsElementPresent(By.CssSelector(BluRingViewer.userSettings_Icon)))
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

                //Logout Application.
                Login.CloseStudy();

                //changing User Preferences to Blu-Ring Viewer.
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.BluringViewerRadioBtn().Click();
                userPrefer.CloseUserPreferences();
                Login.Logout();

                //Return Result.
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
            finally
            {
                Login.LoginIConnect("Administrator", "Administrator");
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                userPrefer.BluringViewerRadioBtn().Click();
                userPrefer.CloseUserPreferences();
                Login.Logout();
            }
        }
    }
}
