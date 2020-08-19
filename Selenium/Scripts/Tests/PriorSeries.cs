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
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
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
using System.Diagnostics;


namespace Selenium.Scripts.Tests
{
    class PriorSeries
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        WpfObjects wpfobject;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public PriorSeries(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            hphomepage = new HPHomePage();
            wpfobject = new WpfObjects();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
        }

        String Role1 = "Role_1" + new Random().Next(1, 1000);
        String Role2 = "Role_2" + new Random().Next(1, 1000);
        String Role2_1 = "Role_21" + new Random().Next(1, 1000);
        String Role3 = "Role_3" + new Random().Next(1, 1000);
        String Role4 = "Role_4" + new Random().Next(1, 1000);
        String R1 = "R_1" + new Random().Next(1, 1000);
        String R2 = "R_2" + new Random().Next(1, 1000);
        String R2_1 = "R_21" + new Random().Next(1, 1000);
        String R3 = "R_3" + new Random().Next(1, 1000);
        String User1 = "User_1" + new Random().Next(1, 1000);
        public string rgbavalue = "rgba(255, 160, 0, 1)";


        /// <summary>
        /// Image and Series Selection
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28016(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String pinnumber = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String DOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDOB");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String Reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");
                String EA91 = login.GetHostName(Config.EA91);

                /*PreCondition-> Enable Email Study*/
                //Setting EmailStudy to SuperRole
                login.LoginIConnect(UserName, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole", "SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                
                //Search and Load a prior study
                login.LoginIConnect(UserName, Password);
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");
                //Search,Select,Launch study
                studies.SearchStudy(patientID:PatientID,Datasource:EA91);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);

                //Step-1:Double click on a Series Thumbnail
                viewer.DoubleClick(viewer.Thumbnails()[0]);

                //Verify whether Image is displayed and scroll bar is in the top most position
                String ThumbnailSeriesUID = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String ViewerSeriesUID = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "seriesUID");
                if (ViewerSeriesUID.Equals(ThumbnailSeriesUID) && viewer.ViewportScrollHandle(1,1,1).GetAttribute("Style").ToLower().Contains("top: 0px"))
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

                //Step-2:Scroll by clicking down arrow button

                for (int i = 0; i < 7; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                }
                
                //Verify whether Images are scrolled
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                bool status2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1withScroll(1));
                if (status2 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("8"))
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

                //Step-3:Scroll by middle wheel
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-4:Scroll by up arrow key
                viewer.Scroll(1,1,3, "up", "key");
                //Verify whether Images are scrolled
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                bool status4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1withScroll(1));
                if (status4 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("5"))
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
                viewer.Scroll(1,1, 4, "down", "click");


                //Step-5:Scroll by pressing and holding up arrow key  
 
                //String ThumbnailSeriesUID5 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                //String ViewerSeriesUID5 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "seriesUID");
                //while (!(ThumbnailSeriesUID5 == ViewerSeriesUID5))

                //viewer.SeriesViewer_1X1(1).Click();
                //var action1 = new Actions(BasePage.Driver);
                //action1.KeyDown(Keys.ArrowUp);

                //action1.KeyDown(Keys.Shift).KeyDown(Keys.ArrowUp).Build().Perform();
                //    PageLoadWait.WaitForFrameLoad(10);

                //var SIMULATE_KEY = "var e = new Event(arguments[0]);" +
                //  "e.key = arguments[1];" +
                //  "e.keyCode = 38;" +
                //  "e.which = e.keyCode;" +
                //  "e.altKey = false;" +
                //  "e.ctrlKey = false;" +
                //  "e.shiftKey = false;" +
                //  "e.metaKey = false;" +
                //  "e.bubbles = true;" +
                //  "arguments[2].dispatchEvent(e);";

                //var target = BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg']"));

                // press the key "a"
                //browser.executeScript(SIMULATE_KEY, "keydown", "a", target);

                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(SIMULATE_KEY,"keydown",38,target);

                //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ViewportScrollHandle(1, 1, 1).GetAttribute("Style").Contains("top: 0px")));
                //String script = "";
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);                

                //Verify whether Images are scrolled
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //PageLoadWait.WaitForFrameLoad(10);
                //bool status5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1withScroll(1));
                //if (status5)
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                

                result.steps[++ExecutedSteps].status = "In Hold";


                //Step-6:Close and Load the study with atleast 1 prior
                viewer.CloseStudy();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: EA91);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.ViewStudy(1,1,1))
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

                //Step-7:Open the History Panel and validate priorstudy is listed
                viewer.NavigateToHistoryPanel();
                if (viewer.Study(1).Displayed && viewer.Study(2).Displayed)
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

                //Step-8:Mail a study to valid email
                viewer.ChooseColumns(new string[]{"Accession"});
                viewer.OpenPriors(new string[]{"Accession"},new string[]{Accessions[1]});
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SeriesViewer_1X1(2).Click();
                viewer.EmailStudy(Email, LastName + FirstName, Reason, 1);
                pinnumber = viewer.FetchPin();
                ExecutedSteps++;

                //Step-9 & 10: Click the URL from Mail and submit the pin
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step-11:Open History Panel and validate that priorstudy is not listed
                result.steps[++ExecutedSteps].status = "Not Automated";

                
                viewer.CloseStudy();
                login.Logout();

                //ReSetting EmailStudy to SuperRole
                login.LoginIConnect(UserName, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 1);
                rolemanagement.ClickSaveEditRole();
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

                //ReSetting EmailStudy to SuperRole
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 1);
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Viewing shared priors
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28017(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            StudyViewer viewer = null;            
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Studies studies;
            Outbounds outbounds;
            Inbounds inbounds;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;                
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                //String[] Lastnames = LastName.Split(':');
                String DOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDOB");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                //String[] FirstNames = FirstName.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');               
                String datasource1 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)
                String datasource2 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
                String EA91 = login.GetHostName(Config.EA91);

                //Step-1:Login as Administrator
                login.LoginIConnect(UserName,Password);
                ExecutedSteps++;

                //Step-2:Create a role Role1 with DS1 connected and grant access enabled to anyone                
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain,Role1,"any");
                rolemanagement.SearchRole(Role1);
                rolemanagement.SelectRole(Role1);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn().Click();
                rolemanagement.AddDatasourceToRole(datasource1);
                rolemanagement.ClickSaveEditRole();

                if (rolemanagement.RoleExists(Role1))
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

                //Step-3:Create a role Role2 with DS2 connected and grant access enabled to anyone                
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain, Role2, "any");
                rolemanagement.SearchRole(Role2);
                rolemanagement.SelectRole(Role2);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.AddDatasourceToRole(datasource2);
                rolemanagement.ClickSaveEditRole();

                if (rolemanagement.RoleExists(Role2))
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

                //Step-4:Create a user R1 with role Role1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(R1,DefaultDomain,Role1);
                if (usermanagement.IsUserExist(R1,DefaultDomain))
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


                //Step-5:Create a user R2 with role Role2
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(R2, DefaultDomain, Role2);
                if (usermanagement.IsUserExist(R2, DefaultDomain))
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
                
                //Step-6:Logout iCA and Login as R1
                login.Logout();
                login.LoginIConnect(R1,R1);
                ExecutedSteps++;

                //Step-7:Search a study that have priors in DS1 not in DS2
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientIds[0]);
                Dictionary<int, string[]> results = BasePage.GetSearchResults();
                int totalpriors = results.Count;
                Dictionary<string, string> row = studies.GetMatchingRow(new string[] { "Patient ID"}, new string[] { PatientIds[0]});
                if (!(row == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].SetLogs();                    
                }
                               

                //Step-8:Select a study and Grant Access to R2
                studies.SelectStudy1("Accession", Accessions[0]);
                studies.ShareStudy(false,new string[]{R2});
                ExecutedSteps++;

                //Step-9:Check the shared study in outbounds of R1
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("AccessionNo",Accessions[0]);
                Dictionary<string, string> studyshared = outbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions[0], "Shared" });
                if (!(studyshared == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10:Load the study from outbounds
                outbounds.SelectStudy1("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy(1,1,1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11:Open PatientHistory and validate all priors are listed
                viewer.NavigateToHistoryPanel();
                int priorscount1 = viewer.CountPriorsInHistory();
                viewer.ChooseColumns(new string[] {"Accession"});
                if (priorscount1==totalpriors)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12:Load a prior and validate its display
                Dictionary<string, string> secondstudy = viewer.GetMatchingRow("Accession", Accessions[1]);                
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && secondstudy["Accession"].Equals(Studyinfo.Split(',')[0]))
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
                viewer.CloseStudy();

                //Step-13:Logout and Login as R2
                login.Logout();
                login.LoginIConnect(R2,R2);
                ExecutedSteps++;

                //Step-14:Check the study in inbounds of R2
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("AccessionNo",Accessions[0]);
                Dictionary<string, string> studyshared2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions[0], "Shared" });
                if (!(studyshared2 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-15:Load the study in viewer
                inbounds.SelectStudy1("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy(1,1,1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16:OpenHistrory Panel and validate that priors are not listed
                viewer.NavigateToHistoryPanel();
                int priorcount = viewer.CountPriorsInHistory();
                if (priorcount<=totalpriors)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();

                //Step-17:Search a study in DS2 with its prior in DS1
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID:PatientIds[1],Datasource:datasource2);
                Dictionary<int, string[]> results17 = BasePage.GetSearchResults();
                int totalpriors17 = results17.Count;
                Dictionary<string, string> row17 = studies.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientIds[1], Accessions[2] });
                if (!(row17 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].SetLogs();
                }


                //Step-18:Select a study and Grant Access to R1
                studies.SelectStudy1("Accession", Accessions[2]);
                studies.ShareStudy(false, new string[] { R1 });
                ExecutedSteps++;

                //Step-19:Check the shared study in outbounds of R2
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("AccessionNo", Accessions[2]);
                Dictionary<string, string> studyshared19 = outbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[1], Accessions[2], "Shared" });
                if (!(studyshared19 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20:Load the study from outbounds
                outbounds.SelectStudy1("Accession", Accessions[2]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy(1,1,1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Open PatientHistory and validate no priors should be listed(since DS2 has no prior)
                viewer.NavigateToHistoryPanel();
                int priorscount21 = viewer.CountPriorsInHistory();
                if (priorscount21 == totalpriors17)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22:Logout and Login as R1
                login.Logout();
                login.LoginIConnect(R1,R1);
                ExecutedSteps++;

                //Step-23:Check the study in inbounds of R1
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("patientID", PatientIds[1]);
                Dictionary<string, string> studyshared23 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[1], Accessions[2], "Shared" });
                if (!(studyshared23 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-24:Load the study in viewer
                inbounds.SelectStudy1("Accession",Accessions[2]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy(1,1,1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25:OpenHistrory Panel and validate that prior from DS1 listed
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<string, string> prior = viewer.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientIds[1], Accessions[3] });
                if (!(prior == null))                
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26:Load a Prior
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[3] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo26 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && prior["Accession"].Equals(Studyinfo26.Split(',')[0]))
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
                viewer.CloseStudy();
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

        }

        /// <summary>
        /// Grant Access to Study all the priors
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_89618(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            StudyViewer viewer = null;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Studies studies;
            Outbounds outbounds;
            Inbounds inbounds;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String DOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDOB");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String EA91 = login.GetHostName(Config.EA91);

                //Setting Grant Access to SuperRole
                login.LoginIConnect(UserName, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole", "SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                //Step-1:Login as Administrator
                login.LoginIConnect(UserName, Password);                

                //Create a role Role4 with grant access enabled to anyone                
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain, Role4, "any");
                rolemanagement.SearchRole(Role4);
                rolemanagement.SelectRole(Role4);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                //rolemanagement.AddDatasourceToRole(datasource1);
                rolemanagement.ClickSaveEditRole();
                bool roleexist = rolemanagement.RoleExists(Role4);
                //Create a user User1 with role Role4
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, DefaultDomain, Role4);
                bool userexist = usermanagement.IsUserExist(User1, DefaultDomain);

                if (roleexist && userexist)
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

                login.Logout();

                //Step-2:Login as Administrator
                login.LoginIConnect(UserName, Password);
                ExecutedSteps++;

                //Stpe-3:Search,select and click on grant access a prior to User1
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID:PatientID,Datasource:EA91);
                Dictionary<int, string[]> results3 = BasePage.GetSearchResults();
                int totalpriors3 = results3.Count;
                Logger.Instance.InfoLog("Priors count : " + totalpriors3);
                studies.SelectStudy1("Accession", AccessionID);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GrantAccessBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));
                int counter = 0;
                while (true)
                {
                    counter++;
                    if (counter > 4)
                    {
                        break;
                    }

                    try
                    {
                        if (studies.PriorList_GAwinddow().Count.Equals(totalpriors3 - 1))
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.InfoLog("Waiting for priors to be listed");
                    }
                    Thread.Sleep(5000);

                }
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogContentDiv")).Displayed && studies.ShareGridTable().Displayed && studies.PriorList_GAwinddow().Count == totalpriors3 - 1)
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


                //Step-4:Select "SuperAdminGroup" from Select Domain
                //PageLoadWait.WaitForFrameLoad(10);
                studies.DomainSelector_GAwindow().SelectByText(DefaultDomain);
                ExecutedSteps++;

                //Step-5:Select All button is clicked to select all prior studies
                studies.SelectAllBtn().Click();
                ExecutedSteps++;


                //Step-6:Click User-Search button and validate users are listed
                studies.UserSearchBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.UserSearchBtn()));
                BasePage.wait.Until<Boolean>((d) =>
                {
                    if (studies.UsersList_GAwindow().Count > 0)
                    {
                        Logger.Instance.InfoLog("Users are listing");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for users to list..");
                        return false;
                    }
                });
                if (studies.UsersList_GAwindow().Count != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7:Select User1 and click on Grant Access
                studies.UserFilterTextbox().SendKeys(User1);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.UserSearchBtn()));
                studies.UserSearchBtn().Click();                
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studies.UserListTable()));                
                IWebElement row = BasePage.Driver.FindElement(By.CssSelector("table#ctl00_StudySharingControl_m_userlist_hierarchyUserList_itemList>tbody>tr>td>span"));
                if (row.GetAttribute("innerHTML")==(User1 + " " + "(" + User1 + " " + User1 + ")"))
                {
                    row.Click();
                    studies.UserListAddBtn().Click();
                }
             
                studies.GrantAccessBtn_GAwindow().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DialogContentDiv")));
                if (BasePage.Driver.FindElement(By.CssSelector("#DialogContentDiv")).Displayed==false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-8:Check the shared study in outbounds of R1 with all priors
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("patientID",PatientID);
                int count = 0;
                foreach(string Acc in Accessions)
                {
                    Dictionary<string, string> sharedstudies8 = outbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Acc, "Shared" });
                    count++;
                }               
               
                
                if (count==totalpriors3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-9:Load the study from outbounds
                outbounds.SelectStudy1("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy(1,1,1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-10:Open PatientHistory and validate all priors are listed
                viewer.NavigateToHistoryPanel();
                int priorscount10 = viewer.CountPriorsInHistory();                
                if (priorscount10 == totalpriors3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-11:Login as User1                
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Step-12:Check the study in inbounds of User1
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("patientID",PatientID);
                Dictionary<int, string[]> results12 = BasePage.GetSearchResults();
                int totalpriors12 = results12.Count;
                if (totalpriors12==totalpriors3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-13:Load the study in viewer
                inbounds.SelectStudy1("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy(1,1,1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:OpenHistrory Panel and validate that priors are listed
                viewer.NavigateToHistoryPanel();
                int priorcount14 = viewer.CountPriorsInHistory();
                if (priorcount14 == totalpriors3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();                
                login.Logout();

                //ReSetting Grant Access to SuperRole
                login.LoginIConnect(UserName, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole", "SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Disabled().Click();
                rolemanagement.ClickSaveEditRole();
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

                //ReSetting Grant Access to SuperRole
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(10);
                rolemanagement.GrantAccessRadioBtn_Disabled().Click();
                rolemanagement.ClickSaveEditRole();
                login.Logout();
            }

        }

        /// <summary>
        /// Viewing  priors from holding pen
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28018(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            StudyViewer viewer = null;
            Taskbar taskbar = null;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Studies studies = null;
            Outbounds outbounds;
            Inbounds inbounds;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PhUsername = Config.ph1UserName;
                String PhPassword = Config.ph1Password;
                String D1 = "D_1" + new Random().Next(1, 1000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String datasource3 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)
                String datasource2 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
                String eiWindow = "ExamImporter_" + new Random().Next(1000);
                String Destination = Config.Dest1;

                /*Creating Role21 and R21*/
                login.LoginIConnect(UserName, Password);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain, Role2_1, "any");
                rolemanagement.SearchRole(Role2_1);
                rolemanagement.SelectRole(Role2_1);
                rolemanagement.ClickEditRole();
                rolemanagement.GrantAccessRadioBtn().Click();
                rolemanagement.AddDatasourceToRole(datasource2);
                rolemanagement.ClickSaveEditRole();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(R2_1, DefaultDomain, Role2_1);
                login.Logout();

                //Step-1:Login as Administrator
                login.LoginIConnect(UserName, Password);
                ExecutedSteps++;

                //Step-2:Create a role Role3 with DS3 connected and grant access enabled to anyone                
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(DefaultDomain, Role3, "both");
                rolemanagement.SearchRole(Role3);
                rolemanagement.SelectRole(Role3);
                rolemanagement.ClickEditRole();
                rolemanagement.AddDatasourceToRole(datasource3);
                //rolemanagement.ReceiveExamCB().Click();
                //rolemanagement.ArchiveToPacsCB().Click();
                rolemanagement.ClickSaveEditRole();

                if (rolemanagement.RoleExists(Role3))
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


                //Step-3:Create a user R3 with role Role1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(R3, DefaultDomain, Role3);

                if (usermanagement.IsUserExist(R3, DefaultDomain))
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


                //Step-4:Create destination D1 with R3 as receiver and archiver                 
                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.AddDestination(DefaultDomain, D1, Config.DestinationPACS, R3, R3);
                if (dest.SearchDestination(DefaultDomain, D1))
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
                login.Logout();


                //Step-5:Logout and Generate ExamIporter from Service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(DefaultDomain, eiWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                ExecutedSteps++;

                //Step-6:iCA HomePage display
                login.DriverGoTo(login.url);
                if (login.UserIdTxtBox().Displayed && login.PasswordTxtBox().Displayed)
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

                //Step-7:Click Install button
                //Deleting existing installers
                new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
                {
                    if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                        File.Delete(file);
                });
                //Download CD Uploader
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());

                try
                {
                    //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    SelectElement selector = new SelectElement(login.DomainNameDropdown());
                    selector.SelectByText(DefaultDomain);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());
                }
                catch (Exception) { }

                String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (browsername.Equals("internet explorer"))
                {
                    var x = Process.GetProcessesByName("iexplore")[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    WpfObjects._application = Application.Attach(x);
                    wpfobject.GetMainWindowByIndex(0);

                    Panel pane = WpfObjects._application.GetWindows()[0].Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                    wpfobject.WaitTillLoad();
                    bool buttonexists = wpfobject.VerifyElement<TestStack.White.UIItems.Panel>(pane, "Open", "Open", 1);
                    if (buttonexists)
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
                    //Click at location where Save button is present
                    TestStack.White.InputDevices.Mouse.Instance.Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1), ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                    wpfobject.WaitTillLoad();


                }

                //Check whether the file is present
                Boolean installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");

                int counter = 0;
                while (!installerdownloaded && counter++ < 10)
                {
                    PageLoadWait.WaitForDownload(Config.eiInstaller, Config.downloadpath, "msi", 130);
                    installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");
                    Thread.Sleep(1000);
                }

                //Check installer is downloaded or not
                if (installerdownloaded)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("CDUploader installer not downloaded..");
                }

                //Step-8:Launch installer tool
                login._examImporterInstance = eiWindow;
                wpfobject.InvokeApplication(Config.downloadpath + @"\" + Config.eiInstaller + ".msi");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(login._examImporterInstance + " Setup", "Cancel", 1);

                //Validate "End User License Agreement" window
                if (ei.AcceptCheckbox().Visible && !ei.NextBtn().Enabled)
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

                //Step-9:Click Accept and Next
                CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;
                ei.AcceptCheckbox().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                ei.NextBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                try
                {
                    //Choose install for all users and Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.InstallForAdministrator().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();

                    //Choose default destination and click Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                }
                catch (Exception) { }

                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                if (ei.RegUser().Visible && ei.UnRegUser().Visible)
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

                //Step-10:Enter credentials and click Install button
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.UserNameTextbox().BulkText = PhUsername;
                ei.PasswordTextbox().BulkText = PhPassword;
                //String PasswordEncrypt = ei.PasswordTextbox().Text;                

                ei.InstallBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                //wait until installation completes
                int installWindowTimeOut = 0;
                try
                {
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    while (ei.InstallingText(eiWindow).Visible && installWindowTimeOut++ < 15)
                    {
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    if (installWindowTimeOut == 0)
                    {
                        throw new Exception("Exception in CD Uploader installation window -- " + e);
                    }
                }

                //Step Check finish button is displayed
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                if (ei.FinishBtn().Enabled)
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


                //Step-11:Uncheck "Launch application when setup exists" and click Finish
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.LaunchAppCheckbox().Click();
                //WpfObjects._mainWindow.WaitWhileBusy();
                //wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.FinishBtn().Click();

                counter = 0;
                while (WpfObjects._mainWindow.Visible && counter++ < 20)
                {
                    Thread.Sleep(1000);
                }

                //Validate installer window
                if (WpfObjects._mainWindow.IsClosed)
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

                //Step-12:Launch the application
                String[] EIPath = Config.EIFilePath.Split('\\');
                EIPath[Array.FindIndex(EIPath, folder => folder.Equals("Apps")) + 1] = eiWindow;
                String UploaderToolPath = string.Join("\\", EIPath);
                ei.LaunchEI(UploaderToolPath);
                wpfobject.GetMainWindow(eiWindow);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Validate user should able to give credentials
                if (ei.UserNameTextbox_EI().Visible && ei.PasswordTextbox_EI().Visible && ei.EmailTextbox_EI().Visible)
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

                //Step-13:Enter Credentials and sign in
                ei.UserNameTextbox_EI().BulkText = R2_1;
                ei.PasswordTextbox_EI().BulkText = R2_1;
                ei.EI_ClickSignIn(eiWindow);
                ExecutedSteps++;

                //Step-14:Click on 'Don't ask me again' button
                wpfobject.GetMainWindow(eiWindow);
                ei.SettingsTab().Focus();
                ei.DontAskBtn().Click();
                ExecutedSteps++;

                //Select Destination with 
                ei.eiWinName = eiWindow;
                ei.DestinationDropdown().EditableText = "";
                ei.DestinationDropdown().Focus();
                ei.DestinationDropdown().Select(Destination);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Step-15:Select study in the specified path
                WpfObjects._mainWindow.WaitWhileBusy();
                ei.SelectFileFromHdd(StudyPath);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Get all patient details
                string[] PatientDetails = ei.AllPatientDetails(eiWindow);

                //Check patient info are displayed correctly with selected test data
                if (Array.Exists(PatientDetails, detail => detail.Contains(LastName.ToUpper()))
                    && Array.Exists(PatientDetails, detail => detail.Contains(FirstName.ToUpper()))
                    && Array.Exists(PatientDetails, detail => detail.Contains(PatientID)))
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

                //Step-16:Click Send
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.GetMainWindow(eiWindow);
                ei.eiWinName = eiWindow;
                ei.PatientListDropdown().Click();
                ei.SelectAllPatientsToUpload();
                ei.Send();
                ExecutedSteps++;

                //Step-17:Progress bar showing the transfer process as 100%
                ExecutedSteps++;
                //Step-18:Click OK on transfer
                ExecutedSteps++;

                //Logout and Close Exam importer
                ei.EI_Logout();
                ei.CloseUploaderTool();

                //Step-19:Launch iCA 
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-20:Login as R2
                login.LoginIConnect(R2_1, R2_1);
                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (login.StudiesTab().GetAttribute("innerHTML").Equals("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Search the study whose prior is uploaded by EI
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo:Accessions[0],Datasource:datasource2);
                Dictionary<string, string> studylisted = studies.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientIds[0], Accessions[0] });
                if (!(studylisted == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22:Load the study
                studies.SelectStudy1("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23:Open the History Panel and validate that Study uploaded by EI also listed
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<string, string> secondstudy = viewer.GetMatchingRow("Accession", Accessions[1]);
                if (secondstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-24:Load the prior
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                if (viewer.studyPanel(2).Displayed)
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
                viewer.CloseStudy();
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

        }

        /// <summary>
        /// Load Study Priors
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28014(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String DOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDOB");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String[] Accdoblist = { Accessions[0], Accessions[1], Accessions[2] };
                String[] Accdefaultlist = { Accessions[0], Accessions[1], Accessions[2],Accessions[3]};
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String PACSA6 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
                String PACSA7 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)

                /*Disable the EMPI function.
                  In Domain Management, leave the default configuration for Query Related Study Parameter: Patient ID + Patient Full Name*/

                /*Enabling PatientId and Patient FullName checkboxes in Query Related */
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID,Datasource: PACSA6);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                int viewports = viewer.SeriesViewPorts(1).Count;
                viewer.CloseStudy();
                login.Logout();


                //Step-1: Search and Load a prior study
                login.LoginIConnect(UserName, Password);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                studies.SelectStudy1("Accession", AccessionID);
                Dictionary<int, string[]> results = BasePage.GetSearchResults();
                string[] columnnames = BasePage.GetColumnNames();
                string[] columnvalues = BasePage.GetColumnValues(results, "Accession", columnnames);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);

                //Verify whether study loads into viewer with default layout
                int viewports1 = viewer.SeriesViewPorts(1).Count;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean defaultlayout = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewports1 == viewports && defaultlayout)
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

                //Step-2:Open HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results0 = BasePage.GetSearchResults();
                string[] columnnames0 = BasePage.GetColumnNames();
                string[] columnvalues0 = BasePage.GetColumnValues(results0, "Patient ID", columnnames0);
                string[] columnvalues1 = BasePage.GetColumnValues(results0, "Accession", columnnames0);  
                int cnt = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(columnvalues0, s => s.Equals(PatientID)) && Array.Exists(columnvalues1, s => s.Equals(Accdefaultlist[cnt++])))
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

                //Step-3:Double Click one of the priors                
                Dictionary<string, string> secondstudy = viewer.GetMatchingRow("Accession", Accessions[1]);
                //viewer.DoubleClick(viewer.Study(4));
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && secondstudy["Accession"].Equals(Studyinfo.Split(',')[0]))
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

                //Step-4:Double Click another prior
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> thirdstudy = viewer.GetMatchingRow("Accession", Accessions[2]);
                //viewer.DoubleClick(viewer.Study(3));
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo1 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && thirdstudy["Accession"].Equals(Studyinfo1.Split(',')[0]))
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


                //Step-5: Apply Review Tool in each study panel
                viewer.SeriesViewer_1X1(1).Click();
                viewer.SelectToolInToolBar(StudyViewer.ViewerTools.Invert, "review");
                viewer.SeriesViewer_1X1(2).Click();
                viewer.SelectToolInToolBar(StudyViewer.ViewerTools.Invert, "review");
                viewer.SeriesViewer_1X1(3).Click();
                viewer.SelectToolInToolBar(StudyViewer.ViewerTools.Invert, "review");

                if (viewer.SeriesViewer_1X1Invert(1).Displayed && viewer.SeriesViewer_1X1Invert(2).Displayed && viewer.SeriesViewer_1X1Invert(3).Displayed)
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


                //Step-6:Close the study
                viewer.CloseStudy();
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.StudyGrid().Displayed)
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


                //Step-7:Set PatientID as QuerySearch Paramater
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-8:Search and load the study
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study               
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports8 = viewer.SeriesViewPorts(1).Count;
                if (viewports8 == viewports)
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

                //Step-9:Open HistoryPanel
                viewer.NavigateToHistoryPanel();
                Dictionary<int, string[]> results2 = BasePage.GetSearchResults();
                string[] columnnames2 = BasePage.GetColumnNames();
                string[] columnvalues2 = BasePage.GetColumnValues(results2, "Patient ID", columnnames0);

                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(columnvalues2, s => s.Equals(PatientID)))
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

                //Step-10:Double Click one of the priors
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<string, string> secondstudy0 = viewer.GetMatchingRow("Accession", Accessions[1]);
                //viewer.DoubleClick(viewer.Study(4));
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo0 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && secondstudy0["Accession"].Equals(Studyinfo0.Split(',')[0]))
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

                //Step-11:Close the study
                viewer.CloseStudy();
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.StudyGrid().Displayed)
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

                //Step-12:Set PatientFullName as QuerySearch Paramater
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-13:Search and load the study
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study               
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                studies.SelectStudy1("Accession", AccessionID);
                Dictionary<int, string[]> results3 = BasePage.GetSearchResults();
                string[] columnnames3 = BasePage.GetColumnNames();
                string[] columnvalues3 = BasePage.GetColumnValues(results3, "Accession", columnnames);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports13 = viewer.SeriesViewPorts(1).Count;
                if (viewports13 == viewports)
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


                //Step-14:Open HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results4 = BasePage.GetSearchResults();
                string[] columnnames4 = BasePage.GetColumnNames();
                string[] columnvalues4 = BasePage.GetColumnValues(results4, "Accession", columnnames4);
                int cnt1 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(columnvalues3, s => s.Equals(columnvalues4[cnt1])))
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

                //Step-15:Double Click one of the priors                
                Dictionary<string, string> secondstudy2 = viewer.GetMatchingRow("Accession", Accessions[1]);
                //viewer.DoubleClick(viewer.Study(4));
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo2 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && secondstudy2["Accession"].Equals(Studyinfo2.Split(',')[0]))
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

                //Step-16:Set PatientLastName as QuerySearch Paramater
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-17:Search and load the study
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study               
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                studies.SelectStudy1("Accession", AccessionID);
                Dictionary<int, string[]> results5 = BasePage.GetSearchResults();
                string[] columnnames5 = BasePage.GetColumnNames();
                string[] columnvalues5 = BasePage.GetColumnValues(results5, "Accession", columnnames5);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports17 = viewer.SeriesViewPorts(1).Count;
                if (viewports17 == viewports)
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


                //Step-18:Open HistoryPanel 
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results6 = BasePage.GetSearchResults();
                string[] columnnames6 = BasePage.GetColumnNames();
                string[] columnvalues6 = BasePage.GetColumnValues(results6, "Accession", columnnames6);
                int cnt2 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(columnvalues5, s => s.Equals(columnvalues6[cnt2])))
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

                //Step-19:Double Click one of the priors               
                Dictionary<string, string> secondstudy3 = viewer.GetMatchingRow("Accession", Accessions[1]);
                //viewer.DoubleClick(viewer.Study(4));
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo3 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && secondstudy3["Accession"].Equals(Studyinfo3.Split(',')[0]))
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

                //Step-20:Set PatientLastName as QuerySearch Paramater
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                ExecutedSteps++;

                //Step-21:Try setting PatientFullName as QuerySearch Paramater
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                if (domainmanagement.PatientLastnameCheckbox().Selected == false && domainmanagement.PatientFullnameCheckbox().Selected)
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

                //Step-22:Try setting PatientDOB as QuerySearch Paramater
                String Errormsg = "At least Patient ID, Patient Full Name or Patient Last Name is required as query related study parameters.";
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 0);
                domainmanagement.ClickSaveDomain();
                if (domainmanagement.ErrorMessage().Equals(Errormsg))
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

                //Step-23:Try setting PatientIPID as QuerySearch Paramater
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveDomain();
                if (domainmanagement.ErrorMessage().Equals(Errormsg))
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


                //Step-24:Try setting PatientIPID and PatientDOB as QuerySearch Paramater
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveDomain();
                if (domainmanagement.ErrorMessage().Equals(Errormsg))
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

                //Step-25:Try setting nothing as QuerySearch Paramater
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveDomain();
                if (domainmanagement.ErrorMessage().Equals(Errormsg))
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

                //Step-26:Set PatientID and PatientLastName as QuerySearch Paramater               
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                Dictionary<int, string[]> results26 = BasePage.GetSearchResults();
                string[] columnnames26 = BasePage.GetColumnNames();
                string[] Accessions26 = BasePage.GetColumnValues(results26, "Accession", columnnames26);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports26 = viewer.SeriesViewPorts(1).Count;
                if (viewports26 == viewports)
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

                //Step-27:Validate display in HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results27 = BasePage.GetSearchResults();
                string[] columnnames27 = BasePage.GetColumnNames();
                string[] Accessions27 = BasePage.GetColumnValues(results27, "Accession", columnnames27);
                string[] PID27 = BasePage.GetColumnValues(results27, "Patient ID", columnnames27);
                int cnt3 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(Accessions27, s => s.Equals(Accessions[cnt3])) && Array.Exists(PID27, s => s.Equals(PatientID)))
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

                //Step-28:Set PatientID and PatientDOB as QuerySearch Paramater and Validate display in HistoryPanel
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                Dictionary<int, string[]> results28 = BasePage.GetSearchResults();
                string[] columnnames28 = BasePage.GetColumnNames();
                string[] Accessions28 = BasePage.GetColumnValues(results28, "Accession", columnnames28);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports28 = viewer.SeriesViewPorts(1).Count;
                if (viewports28 == viewports)
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

                //Step-29:Validate display in HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results29 = BasePage.GetSearchResults();
                string[] columnnames29 = BasePage.GetColumnNames();
                string[] Accessions29 = BasePage.GetColumnValues(results29, "Accession", columnnames29);
                string[] PID29 = BasePage.GetColumnValues(results29, "Patient ID", columnnames29);
                int cnt29 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(Accessions29, s => s.Equals(Accdoblist[cnt29])) && Array.Exists(PID29, s => s.Equals(PatientID)))
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

                //Step-30:Set PatientID and PatientIPID as QuerySearch Paramater and Validate display in HistoryPanel
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                Dictionary<int, string[]> results30 = BasePage.GetSearchResults();
                string[] columnnames30 = BasePage.GetColumnNames();
                string[] Accessions30 = BasePage.GetColumnValues(results30, "Accession", columnnames30);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports30 = viewer.SeriesViewPorts(1).Count;
                if (viewports30 == viewports)
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


                //Step-31:Validate display in HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                viewer.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<int, string[]> results31 = BasePage.GetSearchResults();
                string[] columnnames31 = BasePage.GetColumnNames();
                string[] Accessions31 = BasePage.GetColumnValues(results31, "Accession", columnnames31);
                string[] PID31 = BasePage.GetColumnValues(results31, "Patient ID", columnnames31);
                string[] IPID31 = BasePage.GetColumnValues(results31, "Issuer of PID", columnnames31);
                int cnt31 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(Accessions31, s => s.Equals(Accessions[cnt31]))
                    && Array.Exists(PID31, s => s.Equals(PatientID)) && Array.Exists(IPID31, s => s.Equals(Config.ipid1)))
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

                //Step-32:Set PatientDOB and PatientFullName as QuerySearch Paramater and Validate display in HistoryPanel
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                Dictionary<int, string[]> results32 = BasePage.GetSearchResults();
                string[] columnnames32 = BasePage.GetColumnNames();
                string[] Accessions32 = BasePage.GetColumnValues(results32, "Accession", columnnames32);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports32 = viewer.SeriesViewPorts(1).Count;
                if (viewports32 == viewports)
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

                //Step-33:Validate display in HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results33 = BasePage.GetSearchResults();
                string[] columnnames33 = BasePage.GetColumnNames();
                string[] Accessions33 = BasePage.GetColumnValues(results33, "Accession", columnnames33);
                string[] PID33 = BasePage.GetColumnValues(results33, "Patient ID", columnnames33);
                int cnt33 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(Accessions33, s => s.Equals(Accdoblist[cnt33])) && Array.Exists(PID33, s => s.Equals(PatientID)))
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

                //Step-34:Set PatientIPID and PatientFullName as QuerySearch Paramater and Validate display in HistoryPanel
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                Dictionary<int, string[]> results34 = BasePage.GetSearchResults();
                string[] columnnames34 = BasePage.GetColumnNames();
                string[] Accessions34 = BasePage.GetColumnValues(results34, "Accession", columnnames34);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports34 = viewer.SeriesViewPorts(1).Count;
                if (viewports34 == viewports)
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

                //Step-35:Validate display in HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results35 = BasePage.GetSearchResults();
                string[] columnnames35 = BasePage.GetColumnNames();
                string[] Accessions35 = BasePage.GetColumnValues(results35, "Accession", columnnames35);
                string[] PID35 = BasePage.GetColumnValues(results35, "Patient ID", columnnames35);
                int cnt35 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(Accessions35, s => s.Equals(Accessions[cnt35])) && Array.Exists(PID35, s => s.Equals(PatientID)))
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

                //Step-36:Set PatientDOB and PatientLastName as QuerySearch Paramater and Validate display in HistoryPanel
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                Dictionary<int, string[]> results36 = BasePage.GetSearchResults();
                string[] columnnames36 = BasePage.GetColumnNames();
                string[] Accessions36 = BasePage.GetColumnValues(results36, "Accession", columnnames36);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports36 = viewer.SeriesViewPorts(1).Count;
                if (viewports36 == viewports)
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

                //Step-37:Validate display in HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results37 = BasePage.GetSearchResults();
                string[] columnnames37 = BasePage.GetColumnNames();
                string[] Accessions37 = BasePage.GetColumnValues(results37, "Accession", columnnames37);
                string[] PID37 = BasePage.GetColumnValues(results37, "Patient ID", columnnames37);
                int cnt37 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(Accessions37, s => s.Equals(Accdoblist[cnt37])) && Array.Exists(PID37, s => s.Equals(PatientID)))
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

                //Step-38:Set PatientIPID and PatientLastName as QuerySearch Paramater and Validate display in HistoryPanel
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                Dictionary<int, string[]> results38 = BasePage.GetSearchResults();
                string[] columnnames38 = BasePage.GetColumnNames();
                string[] Accessions38 = BasePage.GetColumnValues(results38, "Accession", columnnames38);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                //Verify whether study loads into viewer with default layout
                int viewports38 = viewer.SeriesViewPorts(1).Count;
                if (viewports38 == viewports)
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

                //Step-39:Validate display in HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                viewer.ChooseColumns(new string[] { "Issuer of PID" });
                Dictionary<int, string[]> results39 = BasePage.GetSearchResults();
                string[] columnnames39 = BasePage.GetColumnNames();
                string[] Accessions39 = BasePage.GetColumnValues(results39, "Accession", columnnames39);
                string[] PID39 = BasePage.GetColumnValues(results39, "Patient ID", columnnames39);
                string[] IPID39 = BasePage.GetColumnValues(results39, "Issuer of PID", columnnames39);
                int cnt39 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(Accessions39, s => s.Equals(Accessions[cnt39])) 
                    && Array.Exists(PID39, s => s.Equals(PatientID)) && Array.Exists(IPID39, s => s.Equals(Config.ipid1)))
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
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();



                /*Enabling PatientId and Patient FullName checkboxes in Query Related */
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveDomain();
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
                /*Enabling PatientId and Patient FullName checkboxes in Query Related */
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveDomain();
                login.Logout();

            }
        }
       
        /// <summary>
        /// Priors with Reports
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_91428(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String pinnumber = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;                
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String PACSA6 = login.GetHostName(Config.PACS2);

                /*PreCondition-> Enable Report view*/
                //Setting Report view
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);                
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Search and Load a prior study with report
                login.LoginIConnect(UserName, Password);                
                studies = (Studies)login.Navigate("Studies");                

                //Step-1:Launch the study containing Multiple priors
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewport1)
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

                //Step-2:Load any prior study containing report from History tab
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[]{"Accession"});
                viewer.OpenPriors(new string[] {"Accession"},new string[] {Accessions[1]});
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                
                if (viewport2 && viewer.TitlebarReportIcon(2).Displayed)
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
                

                //Step-3:Open the report
                PageLoadWait.WaitForFrameLoad(20);
                viewer.TitlebarReportIcon(2).Click();
                bool report = viewer.ReportContainer(2).Displayed;
                //Get Patient info in study panel title bar
                String PatientInfo3 = viewer.PatientInfoTab();               
                //Get Report details
                viewer.SwitchToReportFrame("studypanel",2);
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ViewerDisplay iframe")));
                else
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ViewerDisplay object")));

                Dictionary<string, string> reportDetails3 = viewer.ReportDetails("studypanel",2);

                //Verify whether Report is displayed
                if (report && (PatientInfo3.Split(',')[0].ToUpper()).Equals(reportDetails3["Last Name"].ToUpper()) && (PatientInfo3.Split(',')[1].ToUpper()).Equals(reportDetails3["First Name"].ToUpper()) && PatientID.Equals(reportDetails3["MRN"]))
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

                //Step-4:Close the study and open another one from priors and ensure that it loads correct report for study
                viewer.CloseStudy();
                studies.SearchStudy(patientID: PatientID, Datasource: PACSA6);
                studies.SelectStudy1("Accession", Accessions[1]);
                viewer = StudyViewer.LaunchStudy();
                //Get Patient info in study panel title bar
                String PatientInfo = viewer.PatientInfoTab();
                viewer.TitlebarReportIcon(1).Click();
                //Get Report details
                viewer.SwitchToReportFrame("studypanel");
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ViewerDisplay iframe")));
                else
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ViewerDisplay object")));

                Dictionary<string, string> reportDetails = viewer.ReportDetails("studypanel");

                //check it's correctness with the study
                if ((PatientInfo.Split(',')[0].ToUpper()).Equals(reportDetails["Last Name"].ToUpper()) && (PatientInfo.Split(',')[1].ToUpper()).Equals(reportDetails["First Name"].ToUpper()) && PatientID.Equals(reportDetails["MRN"]))
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



             
                viewer.CloseStudy();
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

        }

        /// <summary>
        /// Multiple Series_  Studies
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28015(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            StudyViewer viewer = null;
            Studies studies;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String EA91 = login.GetHostName(Config.EA91);

                /*PreCondition-->login as administrator 
                 Load a study from a patient that has multiple studies with multiple series that contain multiple images. 
                 Change the view to the single series display if it is not already.*/

                //Login as Administrator and change the view to SingleSeries display               
                login.LoginIConnect(UserName, Password);
                //Search,select and load a study with multiple studies,multiple series and multiple images
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID:PatientID,Datasource:EA91);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);

                //Step-1:Scroll on the image in the current series and apply tools and validate
                viewer.Scroll(1, 1, 1, "arrow", "click");
                PageLoadWait.WaitForFrameLoad(5);
                viewer.SeriesViewer_1X1(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert = viewer.SeriesViewer_Invert(1, 1, 1).Displayed;
                viewer.SeriesViewer_1X1(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                bool reset = viewer.SeriesViewer_Reset(1, 1, 1).Displayed;
                if (invert && reset && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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


                //Step-2:Change the view to 2 series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForFrameLoad(20);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed)
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

                //Step-3:Verify the image load
                String ThumbnailSeriesUID = viewer.GetInnerAttribute(viewer.Thumbnails()[1], "src", '&', "seriesUID");
                String ViewerSeriesUID = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "seriesUID");
                if (ThumbnailSeriesUID.Equals(ViewerSeriesUID))
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


                //Step-4:Load a series to the right viewer               
                viewer.SeriesViewer_1X2(1).Click();
                PageLoadWait.WaitForFrameLoad(40);
                viewer.DoubleClick(viewer.Thumbnails()[0]);
                //Actions action = new Actions(BasePage.Driver);
                //action.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X2()).Build().Perform();
                Thread.Sleep(20000);
                PageLoadWait.WaitForThumbnailsToLoad(20);               
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                String ThumbnailSeriesUID4 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                PageLoadWait.WaitForThumbnailsToLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                String ViewerSeriesUID4 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "seriesUID");

                if (ThumbnailSeriesUID4.Equals(ViewerSeriesUID4))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5:Click the printable view link
                viewer.SeriesViewer_1X2(1).Click();
                PageLoadWait.WaitForFrameLoad(10);
                viewer.DoubleClick(viewer.Thumbnails()[1]);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                var currentWindow = BasePage.Driver.CurrentWindowHandle;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.PrintView, "review");
                var newwindow = BasePage.Driver.WindowHandles.Last();
                int count = 0;
                while (newwindow == currentWindow)
                {
                    if (count > 20)
                    {
                        throw new Exception("Error in Print Window");
                    }

                    foreach (var window in BasePage.Driver.WindowHandles)
                    {
                        BasePage.Driver.SwitchTo().Window(window);
                        if (BasePage.Driver.Url.Contains("OperationClass=imagePrintView"))
                        {
                            newwindow = window;
                            break;
                        }
                    }

                    Thread.Sleep(3000);
                    count++;

                }

                BasePage.Driver.SwitchTo().Window(newwindow);
                WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 180));
                wait1.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='hidePrint']")));
                wait1.Until(ExpectedConditions.ElementExists(By.CssSelector("#PrintButton")));
                wait1.Until(ExpectedConditions.ElementToBeClickable(viewer.Image(1, 1)));
                if (viewer.Image(1, 1).Displayed && viewer.Image(1, 2).Displayed)
                {
                    Logger.Instance.InfoLog("*****Print Window opened with the image*****");
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {

                    Logger.Instance.InfoLog("******Error in Print WIndow*******");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-6:Close the Printable view link and Turn off TextDisplay(select ToggleText)
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(currentWindow);
                PageLoadWait.WaitForFrameLoad(10);
                // BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("off"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7:Turn on TextDisplay(off ToggleText)
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("on") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("on"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-8:Apply Tools and validate
                viewer.SeriesViewer_1X2(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert8 = viewer.SeriesViewer_Invert(1, 1, 2).Displayed;
                if (invert8)
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


                //Step-9:Save the series in right
                result.steps[++ExecutedSteps].status = "No Automation";
                /*   viewer.SeriesViewer_1X2(1).Click();
                   viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                   BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#m_studyPanels_m_studyPanel_1_saveProgressImg")));
                   String ThumbnailSeriesUID9 = viewer.GetInnerAttribute(viewer.Thumbnails()[2], "src", '&', "seriesUID");
                   String ViewerSeriesUID9 = viewer.GetInnerAttribute(viewer.SeriesViewer_(), "src", '&', "seriesUID");

                   if ()
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



                //Step-10:Change the view to 4 series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed && viewer.SeriesViewer_2X1().Displayed && viewer.SeriesViewer_2X2().Displayed)
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

                //Step-11:Verify the image loaded 
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewport11 && viewer.ViewStudy(1, 1, 1) && viewer.ViewStudy(1, 1, 2) && 
                    viewer.ViewStudy(1, 2, 1) && viewer.ViewStudy(1, 2, 2) &&
                    viewer.SeriesViewer_1X1(1).GetAttribute("imagenum").Equals("2"))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12:Text display off
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("off"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13:Load any series to the viewer and validate
                viewer.SeriesViewer_2X1().Click();
                viewer.DoubleClick(viewer.Thumbnails()[2]);
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("off")
                    && viewer.SeriesViewer_2X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_2X2().GetAttribute("src").Contains("off"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14:Text Display on
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("on") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("on"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:Click printable view link
                viewer.SeriesViewer_1X2(1).Click();
                viewer.DoubleClick(viewer.Thumbnails()[1]);
                var currentWindow15 = BasePage.Driver.CurrentWindowHandle;
                viewer.SelectToolInToolBar(StudyViewer.ViewerTools.PrintView, "review");
                var newwindow15 = BasePage.Driver.WindowHandles.Last();
                int count15 = 0;
                while (newwindow15 == currentWindow15)
                {
                    if (count15 > 20)
                    {
                        throw new Exception("Error in Print Window");
                    }

                    foreach (var window in BasePage.Driver.WindowHandles)
                    {
                        BasePage.Driver.SwitchTo().Window(window);
                        if (BasePage.Driver.Url.Contains("OperationClass=imagePrintView"))
                        {
                            newwindow15 = window;
                            break;
                        }
                    }

                    Thread.Sleep(40000);
                    count++;

                }

                BasePage.Driver.SwitchTo().Window(newwindow15);
                WebDriverWait wait15 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 180));
                wait15.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='hidePrint']")));
                wait15.Until(ExpectedConditions.ElementExists(By.CssSelector("#PrintButton")));
                wait1.Until(ExpectedConditions.ElementToBeClickable(viewer.Image(1, 1)));
                if (viewer.Image(1, 1).Displayed && viewer.Image(1, 2).Displayed && viewer.Image(2, 1).Displayed && viewer.Image(2, 2).Displayed)
                {
                    Logger.Instance.InfoLog("*****Print Window opened with the image*****");
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {

                    Logger.Instance.InfoLog("******Error in Print WIndow*******");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-16:Close the Printable view link and Turn off TextDisplay(select ToggleText)
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(currentWindow15);
                PageLoadWait.WaitForFrameLoad(10);
                //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("off")
                    && viewer.SeriesViewer_2X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_2X2().GetAttribute("src").Contains("off"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:17:TextDisplay on
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("on") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("on"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18:Apply tools and verify
                viewer.SeriesViewer_1X2(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert18 = viewer.SeriesViewer_Invert(1, 1, 2).Displayed;
                if (invert18)
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

                //Step-19:Save the series(top right)
                result.steps[++ExecutedSteps].status = "No Automation";
                /*   viewer.SeriesViewer_1X2(1).Click();
                   viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                   BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#m_studyPanels_m_studyPanel_1_saveProgressImg")));
                   String ThumbnailSeriesUID9 = viewer.GetInnerAttribute(viewer.Thumbnails()[2], "src", '&', "seriesUID");
                   String ViewerSeriesUID9 = viewer.GetInnerAttribute(viewer.SeriesViewer_(), "src", '&', "seriesUID");

                   if ()
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

                //Step-20:Select bottom left series scroll to an image and apply tool
                viewer.SeriesViewer_2X1(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert20 = viewer.SeriesViewer_Invert(1, 2, 1).Displayed;
                if (invert20)
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

                //Step-21:Change the series to 6 views
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed && viewer.SeriesViewer_2X1().Displayed && viewer.SeriesViewer_2X2().Displayed
                    && viewer.SeriesViewer_1X3().Displayed && viewer.SeriesViewer_2X3().Displayed)
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

                //Step-22:Verify the image loaded  
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport22 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewport22 && viewer.ViewStudy(1, 1, 1) && viewer.ViewStudy(1, 1, 2) && viewer.ViewStudy(1, 1, 3) && viewer.ViewStudy(1, 2, 1) && viewer.ViewStudy(1, 2, 2) && viewer.ViewStudy(1, 2, 3))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23:Text display off
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("off") &&
                    viewer.SeriesViewer_2X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_2X2().GetAttribute("src").Contains("off"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24:Load any series to the empty viewers and validate the textdisplay off in all viewers
                viewer.SeriesViewer_1X3().Click();
                viewer.DoubleClick(viewer.Thumbnails()[1]);
                Thread.Sleep(20000);
                viewer.SeriesViewer_2X3().Click();
                viewer.DoubleClick(viewer.Thumbnails()[0]);
                Thread.Sleep(20000);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("off") &&
                    viewer.SeriesViewer_2X1().GetAttribute("src").Contains("off") && viewer.SeriesViewer_2X2().GetAttribute("src").Contains("off")
                    && viewer.SeriesViewer_1X3().GetAttribute("src").Contains("off") && viewer.SeriesViewer_2X3().GetAttribute("src").Contains("off"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25:TextDisplay on
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("on") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("on") &&
                    viewer.SeriesViewer_2X1().GetAttribute("src").Contains("on") && viewer.SeriesViewer_2X2().GetAttribute("src").Contains("on")
                    && viewer.SeriesViewer_1X3().GetAttribute("src").Contains("on") && viewer.SeriesViewer_2X3().GetAttribute("src").Contains("on"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26:Click on printable view link
                viewer.SeriesViewer_1X2(1).Click();
                viewer.DoubleClick(viewer.Thumbnails()[1]);
                var currentWindow26 = BasePage.Driver.CurrentWindowHandle;
                viewer.SelectToolInToolBar(StudyViewer.ViewerTools.PrintView, "review");
                var newwindow26 = BasePage.Driver.WindowHandles.Last();
                int count26 = 0;
                while (newwindow26 == currentWindow26)
                {
                    if (count26 > 20)
                    {
                        throw new Exception("Error in Print Window");
                    }

                    foreach (var window in BasePage.Driver.WindowHandles)
                    {
                        BasePage.Driver.SwitchTo().Window(window);
                        if (BasePage.Driver.Url.Contains("OperationClass=imagePrintView"))
                        {
                            newwindow26 = window;
                            break;
                        }
                    }

                    Thread.Sleep(1000);
                    count++;

                }

                BasePage.Driver.SwitchTo().Window(newwindow26);
                wait1.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='hidePrint']")));
                wait1.Until(ExpectedConditions.ElementExists(By.CssSelector("#PrintButton")));
                wait1.Until(ExpectedConditions.ElementToBeClickable(viewer.Image(1, 1)));
                if (viewer.Image(1, 1).Displayed && viewer.Image(1, 2).Displayed && viewer.Image(2, 1).Displayed && viewer.Image(2, 2).Displayed &&
                    (viewer.Image(1, 3).Displayed && viewer.Image(2, 3).Displayed))
                {
                    Logger.Instance.InfoLog("*****Print Window opened with the image*****");
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {

                    Logger.Instance.InfoLog("******Error in Print WIndow*******");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-27:Close the Printable view link and TextDisplay off is validated
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(currentWindow26);
                PageLoadWait.WaitForFrameLoad(10);
                //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));

                if (viewer.SeriesViewer_1X1().GetAttribute("src").Contains("on") && viewer.SeriesViewer_1X2().GetAttribute("src").Contains("on")
                    && viewer.SeriesViewer_2X1().GetAttribute("src").Contains("on") && viewer.SeriesViewer_2X2().GetAttribute("src").Contains("on")
                    && viewer.SeriesViewer_1X3().GetAttribute("src").Contains("on") && viewer.SeriesViewer_1X3().GetAttribute("src").Contains("on"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28:Apply tools and validate                
                viewer.SeriesViewer_1X2(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert28 = viewer.SeriesViewer_Invert(1, 1, 2).Displayed;
                if (invert28)
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

                //Step-29:Save the series(top right)
                result.steps[++ExecutedSteps].status = "No Automation";
                /*   viewer.SeriesViewer_1X2(1).Click();
                   viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                   BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#m_studyPanels_m_studyPanel_1_saveProgressImg")));
                   String ThumbnailSeriesUID9 = viewer.GetInnerAttribute(viewer.Thumbnails()[2], "src", '&', "seriesUID");
                   String ViewerSeriesUID9 = viewer.GetInnerAttribute(viewer.SeriesViewer_(), "src", '&', "seriesUID");

                   if ()
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

                //Step-30:Select bottom left series scroll to an image and apply tool
                viewer.SeriesViewer_2X1(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert30 = viewer.SeriesViewer_Invert(1, 2, 1).Displayed;
                if (invert30)
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

                //Step-31:Change the view to 2 series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed)
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

                //Step-32:Verify the image load
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport32 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewport32)
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


                //Step-33:Load a series to the right viewer
                viewer.SeriesViewer_1X2(1).Click();
                viewer.DoubleClick(viewer.Thumbnails()[0]);
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                String ThumbnailSeriesUID33 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                PageLoadWait.WaitForAllViewportsToLoad(40);
                String ViewerSeriesUID33 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "seriesUID");

                if (ThumbnailSeriesUID33.Equals(ViewerSeriesUID33))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34:Select right series scroll to an image and apply tool
                viewer.SeriesViewer_1X2(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert34 = viewer.SeriesViewer_Invert(1, 1, 2).Displayed;
                viewer.SeriesViewer_1X2(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                bool reset34 = viewer.SeriesViewer_Reset(1, 1, 2).Displayed;
                if (invert34 && reset34)
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

                //Step-35:Change the view to 1 Series with right series is still selected
                viewer.SeriesViewer_1X2(1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed)
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

                //Step-36:Verify the image loaded is the same which was in the right 
                viewer.SeriesViewer_1X1(1).Click();
                String ThumbnailSeriesUID36 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                PageLoadWait.WaitForAllViewportsToLoad(40);
                String ViewerSeriesUID36 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "seriesUID");

                if (ThumbnailSeriesUID36.Equals(ViewerSeriesUID36))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-37:Change the view to 4 series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed && viewer.SeriesViewer_2X1().Displayed && viewer.SeriesViewer_2X2().Displayed)
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

                //Step-38:Verify the image loaded 
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport38 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewport38 && viewer.ViewStudy(1, 2, 1) && viewer.ViewStudy(1, 2, 2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-39:Load the series to the empty viewers
                viewer.SeriesViewer_2X1().Click();
                viewer.DoubleClick(viewer.Thumbnails()[0]);
                viewer.SeriesViewer_2X2().Click();
                viewer.DoubleClick(viewer.Thumbnails()[1]);
                if (viewer.ViewStudy(1, 2, 1) && viewer.ViewStudy(1, 2, 2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-40:Select other than top left,scroll to an image and apply tools
                viewer.SeriesViewer_2X2().Click();
                viewer.Scroll(2, 2, 3, "arrow", "click");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                bool invert40 = viewer.SeriesViewer_Invert(1, 2, 2).Displayed;
                viewer.SeriesViewer_2X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                bool reset40 = viewer.SeriesViewer_Reset(1, 2, 2).Displayed;
                if (invert40 && reset40)
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

                //Step-41:Change the view to 1 series and validate NextSeries,PreviousSeries buttons are enabled
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(30);
                IWebElement Nextseries = viewer.GetReviewToolImage("Next Series");
                IWebElement Previousseries = viewer.GetReviewToolImage("Previous Series");
                if (viewer.SeriesViewer_1X1(1).Displayed && Nextseries.GetAttribute("class").Contains("disableOnCine") == false && Previousseries.GetAttribute("class").Contains("disableOnCine") == false)
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

                //Step-42:Verify the image load
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport42 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewer.ViewStudy(1, 1, 1) && viewport42)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-43:Set the scope to Series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true)
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

                //Step-44:Change the view to 2 series and validate NextSeries,PreviousSeries buttons are enabled
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[0]));               
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed && Nextseries.GetAttribute("class").Contains("disableOnCine") == false && Previousseries.GetAttribute("class").Contains("disableOnCine") == false)
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

                //Step-45:Verify image load
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport45 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewer.ViewStudy(1, 1, 1) && viewer.ViewStudy(1, 1, 2) && viewport45)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-46:Load a series to the right viewer and Series scope is set
                viewer.SeriesViewer_1X2().Click();
                viewer.DoubleClick(viewer.Thumbnails()[2]);
                if (viewer.ViewStudy(1, 1, 1) && viewer.ViewStudy(1, 1, 2) && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-47:Select left viewer
                viewer.SeriesViewer_1X1().Click();
                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-48:Change the view to 4 series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed && viewer.SeriesViewer_2X1(1).Displayed && viewer.SeriesViewer_2X2(1).Displayed)
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

                //Step-49:Verify the image loaded
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport49 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewport49 && viewer.ViewStudy(1, 1, 1) && viewer.ViewStudy(1, 1, 2) && viewer.ViewStudy(1, 2, 1) && viewer.ViewStudy(1, 2, 2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-50:Select the viewer in which image loaded previously and scope is still set to series
                viewer.SeriesViewer_1X2().Click();
                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-51:Change the view to 1 series
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && Nextseries.GetAttribute("class").Contains("disableOnCine") == false && Previousseries.GetAttribute("class").Contains("disableOnCine") == false)
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

                //Step-52:Verify Image Load
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport52 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewer.ViewStudy(1, 1, 1) && viewport52)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-53:Select the study with no series and click NextSeries
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.NextSeries);
                String ThumbnailSeriesUID53 = viewer.GetInnerAttribute(viewer.Thumbnails()[2], "src", '&', "seriesUID");
                PageLoadWait.WaitForAllViewportsToLoad(40);
                String ViewerSeriesUID53 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "seriesUID");
                if (ViewerSeriesUID53.Equals(ThumbnailSeriesUID53))
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

                //Step-54:Click the Previous Series button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.PreviousSeries);
                PageLoadWait.WaitForFrameLoad(30);
                String ThumbnailSeriesUID54 = viewer.GetInnerAttribute(viewer.Thumbnails()[1], "src", '&', "seriesUID");
                PageLoadWait.WaitForAllViewportsToLoad(40);
                String ViewerSeriesUID54 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "seriesUID");
                if (ViewerSeriesUID54.Equals(ThumbnailSeriesUID54))
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

                //Step-55:Change the view to 4 series and validate the previous button is enabled still
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.SeriesViewer_1X1(1).Displayed && viewer.SeriesViewer_1X2(1).Displayed && viewer.SeriesViewer_2X1(1).Displayed && viewer.SeriesViewer_2X2(1).Displayed && Previousseries.GetAttribute("class").Contains("disableOnCine") == false)
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

                //Step-56:Verify the image load
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                //Take Screenshot
                Boolean viewport56 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (viewport56 && viewer.ViewStudy(1, 1, 1) && viewer.ViewStudy(1, 1, 2) && viewer.ViewStudy(1, 2, 1) && viewer.ViewStudy(1, 2, 2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-57:Load the series to empty viewers
                viewer.SeriesViewer_1X1().Click();
                viewer.DoubleClick(viewer.Thumbnails()[0]);
                PageLoadWait.WaitForFrameLoad(30);
                if (viewer.ViewStudy(1, 1, 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-58:Select top left viewer and select W/L tool
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                if (viewer.SeriesViewer_1X1().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X1().GetCssValue("border-top-color").Equals(rgbavalue) && viewer.GetReviewToolImage("Window Level").GetAttribute("class").Contains("highlight"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-59:Click and drag the cursor on the viewer(W/L tool)
                viewer.SeriesViewer_1X1().Click();
                Actions builder = new Actions(BasePage.Driver);
                builder.ClickAndHold(viewer.SeriesViewer_1X1()).Release(viewer.SeriesViewer_2X1()).Build().Perform();
                if (viewer.SeriesViewer_2X1().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_2X1().GetCssValue("border-top-color").Equals(rgbavalue) && viewer.GetReviewToolImage("Window Level").GetAttribute("class").Contains("highlight"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-60:Select the top-right viewer and change image layout to 2X2 with SeriesScope still still set
                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(30);
                //Validate Image 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool Image = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (Image && viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true)
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


                //Step-61:Reselect top-left viewer and select pan tool
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                if (viewer.SeriesViewer_1X1().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X1().GetCssValue("border-top-color").Equals(rgbavalue) && viewer.GetReviewToolImage("Pan").GetAttribute("class").Contains("highlight"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-62:Click and drag the cursor(Pan)
                //Before Pan
                result.steps[++ExecutedSteps].SetPath(testid + "Before Pan", ExecutedSteps);
                bool Image62_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                //After Pan
                viewer.Scroll(1, 1, 3, "arrow", "click");
                result.steps[ExecutedSteps].SetPath(testid + "After Pan", ExecutedSteps);
                bool Image62_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (Image62_1 && Image62_2 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4"))
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

                //Step-63:Ensure series thumbnail section of left panel is displayed
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool Image63 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                if (Image63)
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
                //Step-64:Try in different resolutions
                result.steps[++ExecutedSteps].status = "No Automation";

                viewer.CloseStudy();
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

        }

        /// <summary>
        /// Viewing priors with Query Related Study Parameters Applied
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28019(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies;
            Outbounds outbounds;
            Inbounds inbounds;
            StudyViewer viewer;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Taskbar taskbar;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String DOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDOB");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] AccessionIDs = AccessionID.Split(':');
                String AccessionList1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions1 = AccessionList1.Split(':');
                String AccessionList2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList2");
                String[] Accessions2 = AccessionList2.Split(':');
                String datasource1 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
                String datasource2 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)                
                String domainname = "D-1" + new Random().Next(1, 1000);
                String Role221 = "R-1" + new Random().Next(1, 1000);
                String Role222 = "R-2" + new Random().Next(1, 1000);
                String U1 = "U-1" + new Random().Next(1, 1000);
                String U2 = "U-2" + new Random().Next(1, 1000);
                String Dest_1 = "Dest-1" + new Random().Next(1, 1000);
                String eiWindow = "EI_" + new Random().Next(1000);
                ei.eiWinName = eiWindow;
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String[] Filepaths = UploadFilePath.Split('=');                
                String[] Accdoblist = { Accessions1[0], Accessions1[1], Accessions2[0] };
                String[] AccU1list = { Accessions1[0], Accessions1[1], Accessions1[2] };
                String[] AccU2list = { Accessions2[0], Accessions2[1], Accessions2[2] };
                String[] AccLastnamelist = { Accessions1[0], Accessions1[1], Accessions2[0], Accessions1[2], Accessions2[1] };
                String[] AccFullnamelist = { Accessions1[0], Accessions1[1], Accessions2[0], Accessions1[2] };
                String[] AccPidlist = { Accessions1[0], Accessions1[1], Accessions2[0], Accessions1[2], Accessions2[1], AccessionIDs[1] };



                //Step-1:Initial Setups
                //Create a new domain
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(domainname, domainname + "Role",
                    new String[] { "datatransfer", "grant", "emailstudy", "datadownload", "pdfreport", "allowdownload", "allowtransfer", "allowemail", "PDFreport", "receiveexam", "archive" ,"imagesharing"},
                    new string[] { datasource1, datasource2 });
                if (domainmanagement.SearchDomain(domainname))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-"+ ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-2:Create roles and users as specified
                login.LoginIConnect(domainname, domainname);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(domainname, Role221, "both");
                rolemanagement.SearchRole(Role221);
                rolemanagement.SelectRole(Role221);
                rolemanagement.ClickEditRole();               
                PageLoadWait.WaitForFrameLoad(20);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.AddDatasourceToRole(datasource1);
                rolemanagement.ClickSaveEditRole();
                bool role221 = rolemanagement.RoleExists(Role221);
                rolemanagement.CreateRole(domainname, Role222, "both");
                rolemanagement.SearchRole(Role222);
                rolemanagement.SelectRole(Role222);
                rolemanagement.ClickEditRole();               
                PageLoadWait.WaitForFrameLoad(20);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.AddDatasourceToRole(datasource2);
                rolemanagement.ClickSaveEditRole();
                bool role222 = rolemanagement.RoleExists(Role222);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(U1, domainname, Role221);
                bool u221 = usermanagement.SearchUser(U1, domainname);
                usermanagement.CreateUser(U2, domainname, Role222);
                bool u222 = usermanagement.SearchUser(U2, domainname);
                //Navigate to Image Sharing-->Institution tab
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.AddDestination(domainname, Dest_1, datasource1, U2, U2);

                if (role221 && role222 && u221 && u222 && dest.SearchDestination(domainname, Dest_1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-3:Testdata Preparation
                taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(domainname, eiWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();

                //EI installation
                String EIPath = ei.EI_Installation(domainname, eiWindow, Config.Inst1,U1, U1);
                //Uploading study to holding pen               
                ei.EIDicomUpload(U2, U2, Dest_1, Filepaths[0], 1, EIPath);

                //Sharing studies
                login.LoginIConnect(U1, U1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions1[1]);
                studies.ShareStudy(false, new string[] { U2 });
                login.Logout();
                login.LoginIConnect(U2, U2);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[1]);
                studies.ShareStudy(false, new string[] { U1 });
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-4:Enabling Patient LastName checkbox in Query Related Parameter */
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-5: Login as U1,Search and share a prior study to U2
                login.LoginIConnect(U1, U1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> study5 = studies.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientIds[0], Accessions1[2] });
                studies.ShareStudy(false, new string[] { U2 });
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared5 = outbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared5 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Load the shared study in outbounds and check for priors in History panel
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results6 = BasePage.GetSearchResults();
                string[] columnnames6 = BasePage.GetColumnNames();
                string[] PID6 = BasePage.GetColumnValues(results6, "Patient ID", columnnames6);
                string[] ACC6 = BasePage.GetColumnValues(results6, "Accession", columnnames6);
                int cnt = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID6, s => s.Equals(PatientIds[0])) && AccU1list.All(item => ACC6.Contains(item)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-7: Login as U2,search for the shared study in Inbounds
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared7 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared7 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Load the study and check in HistoryPanel
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results8 = BasePage.GetSearchResults();
                string[] columnnames8 = BasePage.GetColumnNames();
                string[] PID8 = BasePage.GetColumnValues(results8, "Patient ID", columnnames8);
                string[] ACC8 = BasePage.GetColumnValues(results8, "Accession", columnnames8);
                int cnt8 = 0;
                bool foreignstudy = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID8, s => s.Equals(PatientIds[0])) && Array.Exists(ACC8, s => s.Equals(AccLastnamelist[cnt8++])) && foreignstudy)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-9: Open a prior that U2 can access
                Dictionary<string, string> secondstudy = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && secondstudy["Accession"].Equals(Studyinfo.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Open a prior that is in Holding Pen 
                PageLoadWait.WaitForFrameLoad(20);
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPstudy10 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo1 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPstudy10["Accession"].Equals(Studyinfo1.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
               

                //Step-11:Search for the patient of shared study in Studies Tab of U2
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results11 = BasePage.GetSearchResults();
                string[] columnnames11 = BasePage.GetColumnNames();
                string[] PID11 = BasePage.GetColumnValues(results11, "Patient ID", columnnames11);
                string[] ACC11 = BasePage.GetColumnValues(results11, "Accession", columnnames11);
                int cnt11 = 0;
                bool foreignstudy11 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID11, s => s.Equals(PatientIds[0])) && Array.Exists(ACC11, s => s.Equals(AccLastnamelist[cnt11++])) && foreignstudy11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study12 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo12 = viewer.StudyInfo(2);
                String PatientInfo12 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study12["Study Date"].Equals(Studyinfo12.Split(',')[1].Trim()) && study12["Modality"].Equals(study5["Modality"]) && study12["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13:Load the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy13 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo13 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy13["Accession"].Equals(Studyinfo13.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-14:Go to Outbounds page, open the uploaded prior, select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results14 = BasePage.GetSearchResults();
                string[] columnnames14 = BasePage.GetColumnNames();
                string[] PID14 = BasePage.GetColumnValues(results14, "Patient ID", columnnames14);
                string[] ACC14 = BasePage.GetColumnValues(results14, "Accession", columnnames14);
                int cnt14 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID14, s => s.Equals(PatientIds[0])) && Array.Exists(ACC14, s => s.Equals(AccLastnamelist[cnt14++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study15 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo15 = viewer.StudyInfo(2);
                String PatientInfo15 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study15["Study Date"].Equals(Studyinfo15.Split(',')[1].Trim()) && study15["Modality"].Equals(study5["Modality"]) && study15["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study16 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo16 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && study16["Accession"].Equals(Studyinfo16.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step-17:Enabling Patient Fullname checkbox in Query Related Parameter 
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-18:Login iCA as u2; go to Inbounds search for the study shared by u1.
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared18 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared18 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19:Open the study shared by u1 from Inbounds page, select History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results19 = BasePage.GetSearchResults();
                string[] columnnames19 = BasePage.GetColumnNames();
                string[] PID19 = BasePage.GetColumnValues(results19, "Patient ID", columnnames19);
                string[] ACC19 = BasePage.GetColumnValues(results19, "Accession", columnnames19);
                int cnt19 = 0;
                bool foreignstudy19 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID19, s => s.Equals(PatientIds[0])) && Array.Exists(ACC19, s => s.Equals(AccFullnamelist[cnt19++])) && foreignstudy19)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20:Open a prior in the data source belongs to the user (u2) from History page.
                Dictionary<string, string> study20 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo20 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && study20["Accession"].Equals(Studyinfo20.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21:Open the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy21 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo21 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy21["Accession"].Equals(Studyinfo21.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-22:Go to Studies page. Search for the patient with prior study shared by u1. Open a prior study from Studies page that belongs to the u2's data source. Select History tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results22 = BasePage.GetSearchResults();
                string[] columnnames22 = BasePage.GetColumnNames();
                string[] PID22 = BasePage.GetColumnValues(results22, "Patient ID", columnnames22);
                string[] ACC22 = BasePage.GetColumnValues(results22, "Accession", columnnames22);
                int cnt22 = 0;
                bool foreignstudy22 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID22, s => s.Equals(PatientIds[0])) && Array.Exists(ACC22, s => s.Equals(AccFullnamelist[cnt22++])) && foreignstudy22)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study23 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo23 = viewer.StudyInfo(2);
                String PatientInfo23 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study23["Study Date"].Equals(Studyinfo23.Split(',')[1].Trim()) && study23["Modality"].Equals(study5["Modality"]) && study23["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24:Load the uploaded prior from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy24 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo24 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy24["Accession"].Equals(Studyinfo24.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-25:Go to Outbounds page, open the uploaded prior select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results25 = BasePage.GetSearchResults();
                string[] columnnames25 = BasePage.GetColumnNames();
                string[] PID25 = BasePage.GetColumnValues(results25, "Patient ID", columnnames25);
                string[] ACC25 = BasePage.GetColumnValues(results25, "Accession", columnnames25);
                int cnt25 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID25, s => s.Equals(PatientIds[0])) && Array.Exists(ACC25, s => s.Equals(AccFullnamelist[cnt25++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study26 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo26 = viewer.StudyInfo(2);
                String PatientInfo26 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study26["Study Date"].Equals(Studyinfo26.Split(',')[1].Trim()) && study26["Modality"].Equals(study5["Modality"]) && study26["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-27:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study27 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo27 = viewer.StudyInfo(3);
                if (viewer.studyPanel(3).Displayed && study27["Accession"].Equals(Studyinfo27.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step-28:Enabling PatientId checkbox in Query Related 
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-29:Login iCA as u1, in the Studies page; select a prior study from a patient, Grant Access to u2. Go to in Outbounds page.
                login.LoginIConnect(U1, U1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> study29 = studies.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientIds[0], Accessions1[2] });
                studies.ShareStudy(false, new string[] { U2 });
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared29 = outbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared29 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30:Load the Shared study from Outbounds and check History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results30 = BasePage.GetSearchResults();
                string[] columnnames30 = BasePage.GetColumnNames();
                string[] PID30 = BasePage.GetColumnValues(results30, "Patient ID", columnnames30);
                string[] ACC30 = BasePage.GetColumnValues(results30, "Accession", columnnames30);
                int cnt30 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID30, s => s.Equals(PatientIds[0])) && ACC30.All(item => AccU1list.Contains(item)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-31: Login as U2,search for the shared study in Inbounds
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared31 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared31 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32:Open the study shared by u1 from Inbounds page, select History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results23 = BasePage.GetSearchResults();
                string[] columnnames23 = BasePage.GetColumnNames();
                string[] PID23 = BasePage.GetColumnValues(results23, "Patient ID", columnnames23);
                string[] ACC23 = BasePage.GetColumnValues(results23, "Accession", columnnames23);
                int cnt23 = 0;
                bool foreignstudy23 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID23, s => s.Equals(PatientIds[0])) && Array.Exists(ACC23, s => s.Equals(AccPidlist[cnt23++])) && foreignstudy23)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33:Open a prior in the data source belongs to the user (u2) from History page.
                Dictionary<string, string> study33 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo33 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && study33["Accession"].Equals(Studyinfo33.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34:Open the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy34 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo34 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy34["Accession"].Equals(Studyinfo34.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-35:Go to Studies page. Search for the patient with prior study shared by u1. Open a prior study from Studies page that belongs to the u2's data source. Select History tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results35 = BasePage.GetSearchResults();
                string[] columnnames35 = BasePage.GetColumnNames();
                string[] PID35 = BasePage.GetColumnValues(results35, "Patient ID", columnnames35);
                string[] ACC35 = BasePage.GetColumnValues(results35, "Accession", columnnames35);
                int cnt35 = 0;
                bool foreignstudy35 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID35, s => s.Equals(PatientIds[0])) && Array.Exists(ACC35, s => s.Equals(AccPidlist[cnt35++])) && foreignstudy35)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-36:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study36 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo36 = viewer.StudyInfo(2);
                String PatientInfo36 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study36["Study Date"].Equals(Studyinfo36.Split(',')[1].Trim()) && study36["Modality"].Equals(study5["Modality"]) && study36["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-37:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy37 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo37 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy37["Accession"].Equals(Studyinfo37.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-38:Go to Outbounds page, open the uploaded prior select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results38 = BasePage.GetSearchResults();
                string[] columnnames38 = BasePage.GetColumnNames();
                string[] PID38 = BasePage.GetColumnValues(results25, "Patient ID", columnnames38);
                string[] ACC38 = BasePage.GetColumnValues(results25, "Accession", columnnames38);
                int cnt38 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID38, s => s.Equals(PatientIds[0])) && Array.Exists(ACC38, s => s.Equals(AccPidlist[cnt38++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-39:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study39 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo39 = viewer.StudyInfo(2);
                String PatientInfo39 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study39["Study Date"].Equals(Studyinfo39.Split(',')[1].Trim()) && study39["Modality"].Equals(study5["Modality"]) && study39["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-40:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study40 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo40 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && study40["Accession"].Equals(Studyinfo40.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step-41:Enabling PatientId and Patient LastName checkboxes in Query Related
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-42:Login iCA as u1, in the Studies page; select a prior study from a patient, Grant Access to u2. Go to in Outbounds page. 
                login.LoginIConnect(U1, U1);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> study42 = studies.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientIds[0], Accessions1[2] });
                studies.ShareStudy(false, new string[] { U2 });
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared42 = outbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared42 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-43:Load the Shared study from Outbounds and check History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results43 = BasePage.GetSearchResults();
                string[] columnnames43 = BasePage.GetColumnNames();
                string[] PID43 = BasePage.GetColumnValues(results43, "Patient ID", columnnames43);
                string[] ACC43 = BasePage.GetColumnValues(results43, "Accession", columnnames43);

                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID43, s => s.Equals(PatientIds[0])) && ACC43.All(item => AccLastnamelist.Contains(item)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-44:Login iCA as u2; go to Inbounds search for the study shared by u1.
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared44 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared44 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-45:Open the study shared by u1 from Inbounds page, select History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results45 = BasePage.GetSearchResults();
                string[] columnnames45 = BasePage.GetColumnNames();
                string[] PID45 = BasePage.GetColumnValues(results45, "Patient ID", columnnames45);
                string[] ACC45 = BasePage.GetColumnValues(results45, "Accession", columnnames45);
                int cnt45 = 0;
                bool foreignstudy45 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID45, s => s.Equals(PatientIds[0])) && Array.Exists(ACC45, s => s.Equals(AccLastnamelist[cnt45++])) && foreignstudy45)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-46:Open a prior in the data source belongs to the user (u2) from History page.
                Dictionary<string, string> study46 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo46 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && study46["Accession"].Equals(Studyinfo46.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-47:Open the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy47 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo47 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy47["Accession"].Equals(Studyinfo47.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-48:Go to Studies page. Search for the patient with prior study shared by u1. Open a prior study from Studies page that belongs to the u2's data source. Select History tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results48 = BasePage.GetSearchResults();
                string[] columnnames48 = BasePage.GetColumnNames();
                string[] PID48 = BasePage.GetColumnValues(results48, "Patient ID", columnnames48);
                string[] ACC48 = BasePage.GetColumnValues(results48, "Accession", columnnames48);
                int cnt48 = 0;
                bool foreignstudy48 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID48, s => s.Equals(PatientIds[0])) && Array.Exists(ACC48, s => s.Equals(AccLastnamelist[cnt48++])) && foreignstudy48)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-49:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study49 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo49 = viewer.StudyInfo(2);
                String PatientInfo49 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study49["Study Date"].Equals(Studyinfo49.Split(',')[1].Trim()) && study49["Modality"].Equals(study5["Modality"]) && study49["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-50:Load the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy50 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo50 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy50["Accession"].Equals(Studyinfo50.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-51:Go to Outbounds page, open the uploaded prior, select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results51 = BasePage.GetSearchResults();
                string[] columnnames51 = BasePage.GetColumnNames();
                string[] PID51 = BasePage.GetColumnValues(results14, "Patient ID", columnnames51);
                string[] ACC51 = BasePage.GetColumnValues(results14, "Accession", columnnames51);
                int cnt51 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID51, s => s.Equals(PatientIds[0])) && Array.Exists(ACC51, s => s.Equals(AccLastnamelist[cnt51++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-52:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study52 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo52 = viewer.StudyInfo(2);
                String PatientInfo52 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study52["Study Date"].Equals(Studyinfo52.Split(',')[1].Trim()) && study52["Modality"].Equals(study5["Modality"]) && study52["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-53:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study53 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo53 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && study53["Accession"].Equals(Studyinfo53.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step-54:Enabling PatientId and Patient FullName checkboxes in Query Related */
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-55:Login iCA as u2; go to Inbounds search for the study shared by u1.
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared55 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared55 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-56:Open the study shared by u1 from Inbounds page, select History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results56 = BasePage.GetSearchResults();
                string[] columnnames56 = BasePage.GetColumnNames();
                string[] PID56 = BasePage.GetColumnValues(results56, "Patient ID", columnnames56);
                string[] ACC56 = BasePage.GetColumnValues(results56, "Accession", columnnames56);
                int cnt56 = 0;
                bool foreignstudy56 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID56, s => s.Equals(PatientIds[0])) && Array.Exists(ACC56, s => s.Equals(AccFullnamelist[cnt56++])) && foreignstudy56)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-57:Open a prior in the data source belongs to the user (u2) from History page.
                Dictionary<string, string> study57 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo57 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && study57["Accession"].Equals(Studyinfo57.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-58:Open the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy58 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo58 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy58["Accession"].Equals(Studyinfo58.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-59:Go to Studies page. Search for the patient with prior study shared by u1. Open a prior study from Studies page that belongs to the u2's data source. Select History tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results59 = BasePage.GetSearchResults();
                string[] columnnames59 = BasePage.GetColumnNames();
                string[] PID59 = BasePage.GetColumnValues(results22, "Patient ID", columnnames59);
                string[] ACC59 = BasePage.GetColumnValues(results22, "Accession", columnnames59);
                int cnt59 = 0;
                bool foreignstudy59 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID59, s => s.Equals(PatientIds[0])) && Array.Exists(ACC59, s => s.Equals(AccFullnamelist[cnt59++])) && foreignstudy59)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-60:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study60 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo60 = viewer.StudyInfo(2);
                String PatientInfo60 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study60["Study Date"].Equals(Studyinfo60.Split(',')[1].Trim()) && study60["Modality"].Equals(study5["Modality"]) && study60["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-61:Load the uploaded prior from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy61 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo61 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy61["Accession"].Equals(Studyinfo61.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-62:Go to Outbounds page, open the uploaded prior select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results62 = BasePage.GetSearchResults();
                string[] columnnames62 = BasePage.GetColumnNames();
                string[] PID62 = BasePage.GetColumnValues(results62, "Patient ID", columnnames62);
                string[] ACC62 = BasePage.GetColumnValues(results62, "Accession", columnnames62);
                int cnt62 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID62, s => s.Equals(PatientIds[0])) && Array.Exists(ACC62, s => s.Equals(AccFullnamelist[cnt62++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-63:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study63 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo63 = viewer.StudyInfo(2);
                String PatientInfo63 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study63["Study Date"].Equals(Studyinfo63.Split(',')[1].Trim()) && study63["Modality"].Equals(study5["Modality"]) && study63["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-64:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study64 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo64 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && study64["Accession"].Equals(Studyinfo64.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step-65:Enabling PatientId and Patient DOB checkboxes in Query Related */
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-66:Login iCA as u2; go to Inbounds search for the study shared by u1.
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[1]);
                Dictionary<string, string> studyshared66 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[1], "Shared" });
                if (!(studyshared66 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-67:Open the study shared by u1 from Inbounds page, select History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results67 = BasePage.GetSearchResults();
                string[] columnnames67 = BasePage.GetColumnNames();
                string[] PID67 = BasePage.GetColumnValues(results67, "Patient ID", columnnames67);
                string[] ACC67 = BasePage.GetColumnValues(results67, "Accession", columnnames67);
                int cnt67 = 0;
                bool foreignstudy67 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID67, s => s.Equals(PatientIds[0])) && Array.Exists(ACC67, s => s.Equals(Accdoblist[cnt67++])) && foreignstudy67)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-68:Open a prior in the data source belongs to the user (u2) from History page.
                Dictionary<string, string> study68 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo68 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && study68["Accession"].Equals(Studyinfo68.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-69:Open the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy69 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo69 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy69["Accession"].Equals(Studyinfo69.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-70:Go to Studies page. Search for the patient with prior study shared by u1. Open a prior study from Studies page that belongs to the u2's data source. Select History tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results70 = BasePage.GetSearchResults();
                string[] columnnames70 = BasePage.GetColumnNames();
                string[] PID70 = BasePage.GetColumnValues(results70, "Patient ID", columnnames70);
                string[] ACC70 = BasePage.GetColumnValues(results70, "Accession", columnnames70);
                int cnt70 = 0;
                bool foreignstudy70 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID70, s => s.Equals(PatientIds[0])) && Array.Exists(ACC70, s => s.Equals(Accdoblist[cnt70++])) && foreignstudy70)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-71:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study71 = viewer.GetMatchingRow("Accession", Accessions1[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo71 = viewer.StudyInfo(2);
                String PatientInfo71 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study71["Study Date"].Equals(Studyinfo71.Split(',')[1].Trim()) && study71["Modality"].Equals(study5["Modality"]) && study71["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-72:Load the uploaded prior from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy72 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo72 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy72["Accession"].Equals(Studyinfo72.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-73:Go to Outbounds page, open the uploaded prior select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results73 = BasePage.GetSearchResults();
                string[] columnnames73 = BasePage.GetColumnNames();
                string[] PID73 = BasePage.GetColumnValues(results73, "Patient ID", columnnames73);
                string[] ACC73 = BasePage.GetColumnValues(results73, "Accession", columnnames73);
                int cnt73 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID73, s => s.Equals(PatientIds[0])) && Array.Exists(ACC73, s => s.Equals(Accdoblist[cnt73++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-74:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study74 = viewer.GetMatchingRow("Accession", Accessions1[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo74 = viewer.StudyInfo(2);
                String PatientInfo74 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study74["Study Date"].Equals(Studyinfo74.Split(',')[1].Trim()) && study74["Modality"].Equals(study5["Modality"]) && study74["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-75:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study75 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo75 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && study75["Accession"].Equals(Studyinfo75.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step-76:Enabling PatientId and IPID checkboxes in Query Related 
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 0);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-77:Login iCA as u2; go to Inbounds search for the study shared by u1.
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[2]);
                Dictionary<string, string> studyshared77 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[2], "Shared" });
                if (!(studyshared77 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-78:Open the study shared by u1 from Inbounds page, select History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results78 = BasePage.GetSearchResults();
                string[] columnnames78 = BasePage.GetColumnNames();
                string[] PID78 = BasePage.GetColumnValues(results78, "Patient ID", columnnames78);
                string[] ACC78 = BasePage.GetColumnValues(results78, "Accession", columnnames78);
                int cnt78 = 0;
                bool foreignstudy78 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID78, s => s.Equals(PatientIds[0])) && Array.Exists(ACC78, s => s.Equals(Accdoblist[cnt78++])) && foreignstudy78)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-79:Open a prior in the data source belongs to the user (u2) from History page.
                Dictionary<string, string> study79 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo79 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && study79["Accession"].Equals(Studyinfo79.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-80:Open the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy80 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo80 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy80["Accession"].Equals(Studyinfo80.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-81:Go to Studies page. Search for the patient with prior study shared by u1. Open a prior study from Studies page that belongs to the u2's data source. Select History tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results81 = BasePage.GetSearchResults();
                string[] columnnames81 = BasePage.GetColumnNames();
                string[] PID81 = BasePage.GetColumnValues(results81, "Patient ID", columnnames81);
                string[] ACC81 = BasePage.GetColumnValues(results81, "Accession", columnnames81);
                int cnt81 = 0;
                bool foreignstudy81 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID81, s => s.Equals(PatientIds[0])) && Array.Exists(ACC81, s => s.Equals(Accdoblist[cnt81++])) && foreignstudy81)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-82:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study82 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo82 = viewer.StudyInfo(2);
                String PatientInfo82 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study82["Study Date"].Equals(Studyinfo82.Split(',')[1].Trim()) && study82["Modality"].Equals(study5["Modality"]) && study82["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-83:Load the uploaded prior from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy83 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo83 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy83["Accession"].Equals(Studyinfo83.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-84:Go to Outbounds page, open the uploaded prior select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results84 = BasePage.GetSearchResults();
                string[] columnnames84 = BasePage.GetColumnNames();
                string[] PID84 = BasePage.GetColumnValues(results84, "Patient ID", columnnames84);
                string[] ACC84 = BasePage.GetColumnValues(results84, "Accession", columnnames84);
                int cnt84 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID84, s => s.Equals(PatientIds[0])) && Array.Exists(ACC84, s => s.Equals(Accdoblist[cnt84++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-85:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study85 = viewer.GetMatchingRow("Accession", Accessions1[2]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo85 = viewer.StudyInfo(2);
                String PatientInfo85 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study85["Study Date"].Equals(Studyinfo85.Split(',')[1].Trim()) && study85["Modality"].Equals(study5["Modality"]) && study85["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-86:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study86 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo86 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && study86["Accession"].Equals(Studyinfo86.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
                login.Logout();

                //Step-87:Enabling Patient Lastname and Patient DOB checkboxes in Query Related
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainname, domainname);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetCheckBoxInEditDomain("patientid", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientfullname", 1);
                domainmanagement.SetCheckBoxInEditDomain("patientlastname", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientdob", 0);
                domainmanagement.SetCheckBoxInEditDomain("patientipid", 1);
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                ExecutedSteps++;
                Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);

                //Step-88:Login iCA as u2; go to Inbounds search for the study shared by u1.
                login.LoginIConnect(U2, U2);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", PatientIds[0]);
                inbounds.SelectStudy1("Accession", Accessions1[1]);
                Dictionary<string, string> studyshared88 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientIds[0], Accessions1[1], "Shared" });
                if (!(studyshared88 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-89:Open the study shared by u1 from Inbounds page, select History tab.
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results89 = BasePage.GetSearchResults();
                string[] columnnames89 = BasePage.GetColumnNames();
                string[] PID89 = BasePage.GetColumnValues(results89, "Patient ID", columnnames89);
                string[] ACC89 = BasePage.GetColumnValues(results89, "Accession", columnnames89);
                int cnt89 = 0;
                bool foreignstudy89 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID89, s => s.Equals(PatientIds[0])) && Array.Exists(ACC89, s => s.Equals(Accdoblist[cnt89++])) && foreignstudy89 && Array.Exists(ACC89, s => s.Equals(AccLastnamelist[cnt89++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-90:Open a prior in the data source belongs to the user (u2) from History page.
                Dictionary<string, string> study90 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo90 = viewer.StudyInfo(2);

                if (viewer.studyPanel(2).Displayed && study90["Accession"].Equals(Studyinfo90.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-91:Open the uploaded prior in Holding pen from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy91 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo91 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy91["Accession"].Equals(Studyinfo91.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-92:Go to Studies page. Search for the patient with prior study shared by u1. Open a prior study from Studies page that belongs to the u2's data source. Select History tab.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Patient ID", PatientIds[0]);
                studies.SelectStudy1("Accession", Accessions2[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results92 = BasePage.GetSearchResults();
                string[] columnnames92 = BasePage.GetColumnNames();
                string[] PID92 = BasePage.GetColumnValues(results92, "Patient ID", columnnames92);
                string[] ACC92 = BasePage.GetColumnValues(results92, "Accession", columnnames92);
                int cnt92 = 0;
                bool foreignstudy92 = viewer.CheckForeignExamAlert("Accession", AccessionIDs[1]);
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID92, s => s.Equals(PatientIds[0])) && Array.Exists(ACC92, s => s.Equals(Accdoblist[cnt92++])) && foreignstudy92 && Array.Exists(ACC92, s => s.Equals(AccLastnamelist[cnt92++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-93:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study93 = viewer.GetMatchingRow("Accession", Accessions1[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo93 = viewer.StudyInfo(2);
                String PatientInfo93 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study93["Study Date"].Equals(Studyinfo93.Split(',')[1].Trim()) && study93["Modality"].Equals(study5["Modality"]) && study93["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-94:Load the uploaded prior from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> HPStudy94 = viewer.GetMatchingRow("Accession", AccessionIDs[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionIDs[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo94 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && HPStudy94["Accession"].Equals(Studyinfo94.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step-95:Go to Outbounds page, open the uploaded prior select History tab
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Patient ID", PatientIds[0]);
                outbounds.SelectStudy1("Accession", AccessionIDs[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                Dictionary<int, string[]> results95 = BasePage.GetSearchResults();
                string[] columnnames95 = BasePage.GetColumnNames();
                string[] PID95 = BasePage.GetColumnValues(results95, "Patient ID", columnnames95);
                string[] ACC95 = BasePage.GetColumnValues(results95, "Accession", columnnames95);
                int cnt95 = 0;
                if (viewer.PatientHistoryDrawer().Displayed && Array.Exists(PID95, s => s.Equals(PatientIds[0])) && Array.Exists(ACC95, s => s.Equals(Accdoblist[cnt95++])) && Array.Exists(ACC95, s => s.Equals(AccLastnamelist[cnt95++])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-96:Load the shared prior from History page. Verify the study date, #of images and modality of the shared study.
                viewer.ChooseColumns(new string[] { "# Images" });
                Dictionary<string, string> study96 = viewer.GetMatchingRow("Accession", Accessions1[1]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions1[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo96 = viewer.StudyInfo(2);
                String PatientInfo96 = viewer.PatientInfoTab(2);
                if (viewer.studyPanel(2).Displayed && study96["Study Date"].Equals(Studyinfo96.Split(',')[1].Trim()) && study96["Modality"].Equals(study5["Modality"]) && study96["# Images"].Equals(study5["# Images"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-97:Open a prior in the data source belongs to the user (u2) from History page.
                viewer.NavigateToHistoryPanel();
                Dictionary<string, string> study97 = viewer.GetMatchingRow("Accession", Accessions2[0]);
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions2[0] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo97 = viewer.StudyInfo(3);

                if (viewer.studyPanel(3).Displayed && study97["Accession"].Equals(Studyinfo97.Split(',')[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();
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
                //PreCondition
                taskbar = new Taskbar();
                taskbar.Show();
            }
        }

    }
}