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
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.eHR;



namespace Selenium.Scripts.Tests
{
    class Sanity_Checklist
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }
        public ServiceTool servicetool { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public WpfObjects wpfobject { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Sanity_Checklist(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ehr = new EHR();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Software Labeling Requirments
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_60951(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data  
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                
                //Step 1 :- Get Version from installed programs
                String Appversion = BasePage.GetInstalledAppVersion("IBM iConnect Access");
                //String Appversion = "6.3.0";
                if (Appversion.Equals(Config.buildversion))
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

                //Step 2 :- Comapare installed Build version with Build info file
                if(BasePage.GetBuildDetails()["Build Number"].Equals(Config.buildnumber))
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

                //Navigate to iCA URL
                login.DriverGoTo(login.url);

                //Step 3 :- Verify released version number in login page splash screen
                if (login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion))
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

                //Login as administrator
                login.LoginIConnect(adminusername, adminpassword);

                //Open About iConnect Access splash screen
                login.OpenHelpAboutSplashScreen();

                //Step 4 :- Verify released version number in About iConnect Access splash screen
                if (login.HelpWebAccessLoginLogo().GetAttribute("src").Contains(Config.buildversion))
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

                //Close About iConnect Access splash screen
                login.CloseHelpAboutSplashScreen();

                OnlineHelp onlinehelp  = new OnlineHelp().OpenHelpandSwitchtoIT(0);
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
                
                //Step 5 :- Verify released version number in About iConnect Access splash screen
                if (onlinehelp.OnlineHelpVersion().Text.Contains(Config.buildversion.Remove(3)))
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

                //Logout iCA 
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);
                login.Logout();

                //Get build version from login page HTML source
                String StylesheetSource = login.LoginStylesheetLink().GetAttribute("href");
                Logger.Instance.InfoLog("Build version in login page HTML source - " + StylesheetSource);

                //Step 6 :- Verify released version number in login page splash screen
                if (StylesheetSource.Contains(Config.buildversion))
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

                //Step 7 :- Verify appended build number in properties using developer tools
                if (StylesheetSource.Split('=')[1].StartsWith(Config.buildversion) &&
                    StylesheetSource.Split('=')[1].EndsWith(Config.buildnumber))
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Study Page Search
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_72880(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            StudyViewer viewer = null;
            Studies studies = null;
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data  
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String datasource1 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)
                String datasource2 = login.GetHostName(Config.PACS2);//10.5.38.27(A6)
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                
                String User1 = "User1_" + new Random().Next(1, 1000);
                
                //Step 1 - Login as Administrator
                login.LoginIConnect(adminusername, adminpassword);
                ExecutedSteps++;

                //Step 2 - Connect one data source
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> domainattr1 = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(domainattr1, new String[] { datasource1 });
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, domainattr1[DomainManagement.DomainAttr.DomainName], domainattr1[DomainManagement.DomainAttr.RoleName]);
                ExecutedSteps++;

                //Logout as Administrator
                login.Logout();

                //Step 3 - Login as Test User
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Navigate to Studies tab and search study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "*");
                studies.ChooseColumns(new String[] { "Data Source" });

                //Get listed studies location
                String[] ListedDataSourceList = BasePage.GetColumnValues("Data Source");
                
                //Step 4 - Validate the listed studies are retrived from connected data source only
                if (ListedDataSourceList.All(element => element.Equals(datasource1)))
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

                //Load Study and Navigate to History Panel
                studies.SelectStudy1("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new String[] {"Data Source"});

                //Get listed studies location
                String[] PriorsDataSourceList = BasePage.GetColumnValues("Data Source");

                //Step 5 - Validate the listed prior studies are retrived from connected data source only
                if (PriorsDataSourceList.All(element => element.Equals(datasource1)))
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

                //Step 6 - Login as Administrator and connect two data sources to test domain
                login.LoginIConnect(adminusername, adminpassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domainmanagement.SelectDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                domainmanagement.ClickEditDomain();
                domainmanagement.DisConnectAllDataSources();
                domainmanagement.ConnectDataSource(datasource1);
                domainmanagement.ConnectDataSource(datasource2);
                domainmanagement.ClickSaveEditDomain();
                ExecutedSteps++;

                //Logout as Administrator
                login.Logout();

                //Step 7 - Login as Test User
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Navigate to Studies tab and search study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "*");
                studies.ChooseColumns(new String[] { "Data Source" });

                //Get listed studies location
                String[] ListedDataSourceList_2 = BasePage.GetColumnValues("Data Source");

                //Step 8 - Validate the listed studies are retrived from connected data source only
                if (ListedDataSourceList_2.All(element => (element.Contains(datasource1) || element.Contains(datasource2))))
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

                //Load Study and Navigate to History Panel
                studies.SelectStudy1("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new String[] { "Data Source" });

                //Get listed studies location
                String[] PriorsDataSourceList_2 = BasePage.GetColumnValues("Data Source");

                //Step 9 - Validate the listed prior studies are retrived from connected data source only
                if (PriorsDataSourceList_2.All(element => (element.Contains(datasource1) || element.Contains(datasource2))))
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
                          
                //Logout application
                login.CloseStudy();
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// Online Help - Desktop
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27822(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            StudyViewer viewer = null;
            Studies studies = null;
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data  
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                
                //Step 1 - Navigate to iCA URL and capture login page
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool WebaccessLoginPage = login.CompareImage(result.steps[ExecutedSteps], login.WebAccessLoginPage(), ImageComparison: 0);
                if (WebaccessLoginPage)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Login as administrator
                login.LoginIConnect(username, password);

                //Step 2 - Validate Studies tab is displayed by default
                if (login.IsTabSelected("Studies"))
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

                //Search and Select Study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionID);
                studies.SelectStudy1("Accession", AccessionID);

                //Step 3 - Launch Study
                viewer = studies.LaunchStudy();
                ExecutedSteps++;

                //Open Online Help window
                OnlineHelp onlinehelp = new OnlineHelp().OpenHelpandSwitchtoIT();

                //Record Part, Date and Revision from Online help window
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool PartDetailTable = login.CompareImage(result.steps[ExecutedSteps], onlinehelp.PartDetailTable(), ImageComparison: 0);
                Dictionary<String, String[]> TableResults = onlinehelp.GetPartDetailTableResults();

                //Step 4 :- Validate Part, Date and Revision are displayed correctly
                if (PartDetailTable && TableResults != null)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                onlinehelp.OpenChapter("Overview", "Overview");
                onlinehelp.NavigateToOnlineHelpFrame("topic");

                //Step 5 :- Validate selected chapter is opened in right side viewer
                if (onlinehelp.ChapterHeading().Text.Equals("Overview"))
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

                //Click Index menu
                onlinehelp.OpenMenu("index");

                //Get all the headings listed under Index menu
                onlinehelp.NavigateToOnlineHelpFrame("indexcontentframe");
                IList<String> IndexHeadings = onlinehelp.GetIndexKeywords();
                var orderedByAsc = IndexHeadings.OrderBy(d => d);

                //Step 6 :- Validate the listed headings are listed in ascending order under index menu
                if (IndexHeadings.SequenceEqual(orderedByAsc))
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

                //Enter a keyword in search box
                onlinehelp.EnterKeyword("Patient");
                onlinehelp.NavigateToOnlineHelpFrame("indexcontentframe");

                //Step 7 :- Validate chapter is highlighted for the entered text
                if (onlinehelp.HighlightedChapter().Text.StartsWith("Patient") && onlinehelp.HighlightedChapter().Displayed)
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
                                
                //Select one chapter
                onlinehelp.GetIndexKeywordElements()["Customizing"].Click();
                PageLoadWait.WaitForPageLoad(15);

                //Navigate to Main Content frame
                onlinehelp.NavigateToOnlineHelpFrame("topic");

                //Step 8 :- Validate selected keyword contents are opened in right side viewer
                if (onlinehelp.ChapterHeading().Text.ToLower().StartsWith("customizing"))
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

                //Click Search menu
                onlinehelp.OpenMenu("search");
                onlinehelp.NavigateToOnlineHelpFrame("searchformframe");

                //Step 9 :- Verify keyword field under Search menu is displayed correctly
                if (onlinehelp.keywordField().Displayed)
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

                //Enter a keyword in search box
                onlinehelp.EnterKeyword("review", "search");
                onlinehelp.NavigateToOnlineHelpFrame("searchresultframe");

                //Step 10 :- Check the entered keyword lists the related articles
                if (onlinehelp.SearchResults().Count > 0)
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

                //Select one article
                String listedArticle = onlinehelp.SearchResults()[0].Text;
                onlinehelp.SearchResults()[0].Click();
                PageLoadWait.WaitForPageLoad(15);
                onlinehelp.NavigateToOnlineHelpFrame("topic");

                //Step 11 :- Validate selected article contents are opened in right side viewer
                if (onlinehelp.ChapterHeading().FindElement(By.CssSelector("font")).Text.ToLower().Equals("review"))
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

                //Step 12 :- Close Study viewer and Validate studies page is navigated
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);
                viewer.CloseStudy();
                ExecutedSteps++;

                //Open About iConnect Access splash screen
                login.OpenHelpAboutSplashScreen();

                //Step 13 :- Verify UDI is displayed in About iConnect Access splash screen
                if (login.UDIText().Displayed)
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

                //Close About iConnect Access splash screen
                login.CloseHelpAboutSplashScreen();

                //Open Online Help window
                new StudyViewer().OpenHelpandSwitchtoIT(0);
                                
                //Record Part, Date and Revision from Online help window
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                Dictionary<String, String[]> TableResults_2 = onlinehelp.GetPartDetailTableResults();

                //Step 14 :- Validate Part, Date and Revision are displayed correctly as in Step 4
                Boolean Status_14 = TableResults.All(x => TableResults_2.Any(y => x.Value.SequenceEqual(y.Value)));
                if (Status_14)
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

                //Step 15 :- Logout the application
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);
                login.Logout();
                ExecutedSteps++;

                //Step 16 :- Multiple browser validation
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }
        
        /// <summary>
        /// Admin-Modality toolbar in New Domain
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_72719(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            DomainManagement domainmanagement = null;
            StudyViewer viewer = null;
            Studies studies = null;
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data  
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String datasource1 = login.GetHostName(Config.SanityPACS);//10.5.38.28(A7)               
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionList.Split(':');
                String User1 = "User1_" + new Random().Next(1, 1000);
                String[] MRtools = new String[] { "Pan", "Localizer Line", "Reset" };

                //Step 1 - Login as Administrator
                login.LoginIConnect(adminusername, adminpassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> domainattr1 = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(domainattr1, new String[] { datasource1 });
                bool IsDomainExists = domainmanagement.SearchDomain(domainattr1[DomainManagement.DomainAttr.DomainName]);
                if (IsDomainExists)
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

                //Logout as Administrator
                login.Logout();

                //Step 2 - Login as Test domain and Add Modality toolbar and preset
                login.LoginIConnect(domainattr1[DomainManagement.DomainAttr.UserID], domainattr1[DomainManagement.DomainAttr.Password]);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.ConfigureModalityToolbar("MR", MRtools);
                domainmanagement.AddPresetForDomain("CR", "test2", "123", "456");
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.ClickCloseEditDomain();
                ExecutedSteps++;

                //Step-3:Load a study of modality for which toolbar is added
                //Navigate to Studies tab and search study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                String[] MRtoolsList = viewer.GetStudyToolsinViewer();
                Boolean IsToolsExist_3 = MRtools.All(item => MRtoolsList.Contains(item));
                if (IsToolsExist_3)
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
                viewer.CloseStudy();

                //Step-4:Load a study not of the modality for which modality toolbar is added
                studies.SearchStudy(AccessionNo: Accessions[1]);
                studies.SelectStudy("Accession", Accessions[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                String[] toolsList = viewer.GetStudyToolsinViewer(); //Preset tool is present as default
                IList<String> IsReviewToolbarExist = viewer.GetReviewToolsFromviewer();
                if (!(toolsList.Length > 1) && IsReviewToolbarExist != null)
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
                viewer.CloseStudy();

                //Step 5 - Load a study of modality for which preset is added
                studies.SearchStudy(AccessionNo: Accessions[2]);
                studies.SelectStudy("Accession", Accessions[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                IList<String> CR_APresets = new List<String>();
                IList<IWebElement> CR_EPresets = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar div ul li[title]"));
                foreach (IWebElement title in CR_EPresets)
                {
                    CR_APresets.Add(title.GetAttribute("title"));
                }

                if (CR_APresets.Contains("test2:123/456"))
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


                //Step 6 - Select a viewport click on preset
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(20);
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul li[title='test2:123/456']\").click()");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool Step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (Step_6)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                //Step 7 -Load a study of modality for which preset is not added
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accessions[0]);
                studies.SelectStudy("Accession", Accessions[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                bool IsPresetIconExist = viewer.IsElementVisible(By.CssSelector("#StudyToolbar div ul ul li[title='test2:123/456']"));

                if (!IsPresetIconExist)
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

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
        /// User and group search with * and first letter as query parameter
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test5_72719(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            UserManagement usermanagement = null;
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data  
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;

                //Login as Admin user
                login.LoginIConnect(adminusername, adminpassword);
                
                //Navigate to User management tab
                usermanagement = login.Navigate<UserManagement>(); 

                //Search user with * as query parameter
                usermanagement.SearchUser("*", Config.adminGroupName);

                //Get All listed users from UI
                IList<String> UsersList = usermanagement.ListedUsers().Select(d => d.Text).ToList();
                
                //Get users from Data base
                IList<String> DBUsersList = BasePage.GetAllUsersFromDB(Config.adminGroupName);

                //Step 1 :- Validate all the users in the selected domain is listed correctly
                if (UsersList.All(d => DBUsersList.Contains(d)))
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

                //Search user with any single character as query parameter
                usermanagement.SearchUser("a", Config.adminGroupName);



                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// UDI Label
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_107568(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data  
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DeviceIdentifierNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Device Identifier");
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Step 1 :- Login as Admin User
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Open About iConnect Access splash screen
                login.OpenHelpAboutSplashScreen();

                //Step 2 :- Verify released version number in About iConnect Access splash screen
                if (login.HelpWebAccessLoginLogo().GetAttribute("src").Contains(Config.buildversion))
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

                //Get UDI text
                String UDItext = login.UDIText().Text.Trim();

                //Step 3 :- Verify UDI is displayed in About iConnect Access splash screen as **UDI:(01)00842000100126(10)6.3.0.763(11)160906.**
                if (login.UDIText().Displayed && UDItext.StartsWith("UDI:(01)") && UDItext.Contains("(10)" + Config.buildversion)
                    && UDItext.Contains("(11)"))
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

                //Get Build Date 
                String buildDate = BasePage.GetBuildDetails()["Date"];
                DateTime Date = DateTime.ParseExact(buildDate.Split(new String[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries)[1], "mm/dd/yyyy", System.Globalization.CultureInfo.CurrentUICulture);
                String BuildDate = Date.ToString("yymmdd");//Date.Year.ToString().Replace("20", String.Empty) + Date.Month.ToString() + Date.Day;

                //Step 4 :- Verify UDI is displayed in About iConnect Access splash screen with correct details
                if (UDItext.StartsWith("UDI:(01)" + DeviceIdentifierNo) && UDItext.Contains("(10)" + Config.buildversion + "." + Config.buildnumber)
                    && UDItext.EndsWith("(11)" + BuildDate))
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

                //Get Batch/Lot number
                String BatchNo = UDItext.Split(new String[]{"(10)", "(11)"}, StringSplitOptions.RemoveEmptyEntries)[1];

                //Step 5 :- Validate Batch/Lot number is assigned based on the build 
                if (BatchNo.Equals(Config.buildversion + "." + Config.buildnumber))
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

                //Get manufacturing date
                String ManufacturingDate = UDItext.Split(new String[] { "(11)" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //DateTime mdate = DateTime.ParseExact(ManufacturingDate, "yymmdd", System.Globalization.CultureInfo.InvariantCulture);

                //Step 6 :- Validate Manufacturing date is displayed in "yymmdd" date format
                if (ManufacturingDate.Equals(BuildDate))
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

                //Step 7 :- Close About iConnect Access splash screen
                login.CloseHelpAboutSplashScreen();
                ExecutedSteps++;

                //Step 8 & 9 :- Internationalisation Configuration and Validation
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 10 :- Configure TestEHR application
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser("enable");
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                servicetool.AllowShowSelectorSearch().Checked = true;
                servicetool.AllowShowSelector().Checked = true;
                servicetool.WaitWhileBusy();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 11 :- Launch Test-EHR application
                ehr.LaunchEHR();
                ExecutedSteps++;

                //Open Study in Viewer with TestEHR as iCA Admin user
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSearchKeys_Study(Accession, "Accession");
                String url_12 = ehr.clickCmdLine("ImageLoad");

                //Step 12 :- Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_12);
                ExecutedSteps++;

                //Click logout in Test-EHR application
                String logoutURL = ehr.ClickLogout();
                login.NavigateToIntegratorURL(logoutURL);
                ehr.CloseEHR();

                //Get Screenshot for logout page of EHR
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool WebaccessLoginPage = login.CompareImage(result.steps[ExecutedSteps], ehr.EndSessionImage());

                //Step 13 :- Validate UDI should not be dispayed on EHR logout page
                if (WebaccessLoginPage)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                
                //Step 14 :- Multiple browser verification
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Study Viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_73005(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            BasePage basepage = new BasePage();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientIDs = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                String[] Accessions = AccessionNumber.Split(':');
                String[] PatientID = PatientIDs.Split(':');
                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FilePath"); ;
                String Datasource = login.GetHostName(Config.DestinationPACS);

                //Send Study to Destination PACS
                BasePage.RunBatchFile(Config.batchfilepath, FilePath + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                
                //Step 1 - In User preferences set the Automatically start cine to OFF for the modality to which the listed study belongs
                //         Search and load for a multiframe study US modality
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                basepage.ModalityDropdown().SelectByText("US");
                BasePage.Driver.FindElement(By.CssSelector("input[id*='AutoStartCineRadioButtons_1']")).Click();
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(Modality: "US", patientID: PatientID[0]);
                studies.ChooseColumns(new String[] { "Patient ID" });
                studies.SelectStudy("Patient ID", PatientID[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                ExecutedSteps++;

                //Step 2 - Set viewer layout to 1x2, Load images in both viewports
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - Click group play
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                if (basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")) &&
                    basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_cineBtnPause")))
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

                //Step 4 - Stop Cine in a viewport
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPlay")));
                if (basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPlay")))
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

                //Step 5 - Click group pause
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPause")).Click();
                Thread.Sleep(5000);
                if (basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPlay")) &&
                    basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_cineBtnPlay")))
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
                viewer.CloseStudy();

                //step 6 - Verify the group play button for HTML5
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome"))
                {
                    studies.ChooseColumns(new String[] { "Patient ID" });
                    studies.SelectStudy("Patient ID", PatientID[0]);
                    viewer.Html5ViewStudy();
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(40);
                    BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                    Thread.Sleep(5000);
                    if (!basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPause")))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseStudy();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }

                //Step 7 - Search and load a study with multiple images in multiple series. XA modality Press 'p' or 'P' key on keyboard.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(Modality: "XA", AccessionNo: Accessions[1]);
                studies.ChooseColumns(new String[] { "Accession" });
                studies.SelectStudy("Accession", Accessions[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")));
                if (basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")))
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

                //Step 8 - Press"p"or"P"key.
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPlay")));
                if (basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPlay")))
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

                //Step 9 - Press"p"or"P"key.
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")));
                if (basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")))
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

                //Step 10 - Stop all running cine plays if there is any, Press the Right Arrow key on keyboard.
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPause")).Click();
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String uid_3 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if (uid_3.Contains("154911_3"))
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

                //Step 11 - Press Left Arrow key on keyboard
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String uid_0 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if (uid_0.Contains("14350_0"))
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

                //Step 12 - Start Cine. While it is running press Right Arrow key on keyboard.
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPlay")).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String uid_4 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if (uid_4.Contains("154911_4") &&
                    basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")))
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

                //Step 13 - While it is running press Up Arrow key on keyboard.
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String uid_4_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if (uid_4_1.Contains("154911_4") &&
                    !basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_cineBtnPause")))
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

                //Step 14
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_cineBtnPlay")).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String uid_4_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                if (uid_4_2.Contains("154911_1") &&
                    basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_cineBtnPlay")) &&
                    !basepage.IsElementVisible(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_cineBtnPause")))
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
                viewer.CloseStudy();

                //Step 15:
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accessions[3], Datasource: Datasource);
                studies.ChooseColumns(new String[] { "Accession" });
                studies.SelectStudy("Accession", Accessions[3]);
                viewer = StudyViewer.LaunchStudy();
                viewer.DrawLineMeasurement(viewer.SeriesViewer_1X1(), 100, 122);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool series_1x1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (series_1x1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool series_1x2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());
                if (series_1x1 == series_1x2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

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
        /// Display Diagnostic Use Warning - XDS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_113326(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            UserPreferences userpreferences = new UserPreferences();
            int ExecutedSteps = -1;
            BasePage basepage = new BasePage();
            Studies studies = null;
            StudyViewer studyviewer = new StudyViewer();
            Patients patient = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                string[] Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID")).Split('=');
                string PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName"));
                string[] PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                string s = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                //Step 1: In iCA server Search for "NotForDiagnosticUse_desktop" in C:\WebAccess\WebAccess\Web.config and set the value to "true"
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "true");
                ExecutedSteps++;
                //Step 2: Configure XDS
                ExecutedSteps++;
                //Step 3: Login to ICA and navigate to the Studies Tab, load a dicom only study in the viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.JPEGRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.ViewStudy())
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
                //Step 4: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label besides the non diagnostic message is displayed below the viewer properly
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 5: Close the viewer and load a XDS only study from history panel or Patient tab
                studyviewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                basepage.ClickElement(studies.ViewButton());
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studyviewer.NavigateToHistoryPanel();
                studyviewer.OpenPriors(new string[] { "Patient ID" }, new string[] { PatientID[0] });
                if (studyviewer.StudyInfo().Contains(Accession[1]))
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
                //Step 6: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label besides the non diagnostic message is displayed properly

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 7: Close the viewer and load a hybrid study
                studyviewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.ViewStudy())
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
                //Step 8: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label besides the non diagnostic message is displayed below the viewer properly
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 9: Repeat the above steps to load dicom only, XDS only and hybrid studies from patient tab
                studyviewer.CloseStudy();
                patient = (Patients)login.Navigate("Patients");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.InputData(PatientName.Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.LoadStudyInPatientRecord(PatientName.Trim());
                patient.NavigateToXdsStudies();
                studyviewer = patient.LaunchStudy(Patients.PatientColumns.Accession, Accession[3]);
                if (studyviewer.ViewStudy())
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
                //Step 10: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label besides the non diagnostic message is displayed properly
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 11: Repeat the above steps to load dicom only, XDS only and hybrid studies from patient tab and history flyout 
                studyviewer.NavigateToHistoryPanel();
                studyviewer.OpenPriors(new string[] { "Patient ID" }, new string[] { PatientID[1] });
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "false");
            }
        }

        /// <summary>
        /// Display Diagnostic Use Warning - ImageSharing
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_113323(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            UserPreferences userpreferences = new UserPreferences();
            int ExecutedSteps = -1;
            BasePage basepage = new BasePage();
            Studies studies = null;
            StudyViewer studyviewer = new StudyViewer();
            Inbounds inbounds = null;
            ExamImporter ei = new ExamImporter();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                string Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));
                string FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "FilePath"));
                //Step 1: In iCA server Search for "NonDiagnosticUseWarningEnabled_desktop" in C:\WebAccess\WebAccess\Web.config and set the value to "true"
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "true");
                ExecutedSteps++;
                //Step 2: Configure image sharing
                ExecutedSteps++;
                //Step 3: From web uploader / EI upload a studies
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, FilePath, 1, Config.EIFilePath);
                ExecutedSteps++;
                //Step 4: Login as receiver and navigate to inbounds page
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.JPEGRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession);
                if (inbounds.CheckStudy("Accession", Accession))
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
                //Step 5: Load the uploaded study in the viewer
                inbounds.SelectStudy("Accession", Accession);
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.ViewStudy())
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
                //Step 6: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 7: Nominate the study for archive
                studyviewer.SelectToolInToolBar(IEnum.ViewerTools.NominateforArchive);
                basepage.ClickButton("div#reviewToolbar a>img[title='Nominate for Archive']");
                basepage.ClickButton("input#m_NominateStudyArchiveControl_NominateStudy");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));
                studyviewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                String studyState16;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyState16);
                if (studyState16 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //Step 8: Login as archivist, navigate to inbounds page and load the study in the viewer.
                login.Logout();
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.JPEGRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession);
                inbounds.SelectStudy("Accession", Accession);
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.ViewStudy())
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
                //Step 9: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 10: Archive the study
                studyviewer.CloseStudy();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                inbounds.SelectStudy("Accession", Accession);
                inbounds.ArchiveStudy("Test", "Test");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                String studyState20;
                inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyState20);
                if (studyState20 == "Archiving")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //Step 11: Login iCA as administrator, navigate to studies tab and load the archived study from the destination
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession);
                studies.SelectStudy("Accession", Accession);
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.ViewStudy())
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
                //Step 12: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(studyviewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(studyviewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "false");
            }
        }

        /// <summary>
        /// Display Diagnostic Use Warning - Conference Lists
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_113324(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            RoleManagement rolemanagement = null;
            DomainManagement domainmanagement = null;
            ConferenceFolders conferencefolders = null;
            Studies studies = null;
            StudyViewer viewer = null;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                string Top = "Top";
                string[] Sub = new string[] { "Sub1", "Sub2" };
                string[] Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID")).Split('=');
                //Step 1: 
                /*1. Ensure 'Enable Conference Lists' is enabled in Enable Features tab\General sub tab in the iCA Service tool .
                2. use existing domain or create new domains, roles and Conference User settings with Conference List and User enabled at Domain and Role level*/
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                if (login.IsTabSelected("DomainManagement"))
                {
                    domainmanagement = new DomainManagement();
                }
                else
                {
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                }
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainName = createDomain[DomainManagement.DomainAttr.DomainName];
                string RoleName = createDomain[DomainManagement.DomainAttr.RoleName];
                string DomainAdminUser = createDomain[DomainManagement.DomainAttr.UserID];
                string DomainAdminPassword = createDomain[DomainManagement.DomainAttr.Password];
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.EditDomainButton().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("conferencelists", 0);
                domainmanagement.ClickSaveEditDomain();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole(RoleName, DomainName);
                rolemanagement.SelectRole(RoleName);
                rolemanagement.EditRoleBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                rolemanagement.SetCheckboxInEditRole("conferenceuser", 0);
                rolemanagement.ClickSaveRole();
                login.Logout();
                login.LoginIConnect(DomainAdminUser, DomainAdminPassword);
                if (login.IsTabPresent("Conference Folders"))
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

                //Step 2: Create a top level folders and sub folders
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
                bool step2_1 = conferencefolders.CreateToplevelFolder(Top);
                bool step2_2 = conferencefolders.CreateSubFolder(Top, Sub[0]);
                bool step2_3 = conferencefolders.CreateSubFolder(Top, Sub[1]);
                if (step2_1 && step2_2 && step2_3)
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

                //Step 3: 
                studies = (Studies)login.Navigate("Studies");
                for (int i = 0; i < Accession.Length; i++)
                {
                    studies.SearchStudy(AccessionNo: Accession[i]);
                    studies.SelectStudy("Accession", Accession[i]);
                    viewer = StudyViewer.LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                    viewer.AddStudyToStudyFolder(Top + "/" + Sub[i]);
                    viewer.CloseStudy();
                }
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
                conferencefolders.ArchiveConferenceFolder(Top + "/" + Sub[1]);
                conferencefolders.NavigateToActiveMode();
                conferencefolders.ExpandAndSelectFolder(Top + "/" + Sub[0]);
                var studycount1 = basepage.GetMatchingRow("Accession", Accession[0]);
                conferencefolders.NavigateToArchiveMode();
                conferencefolders.ExpandAndSelectFolder(Top + "/" + Sub[1]);
                var studycount2 = basepage.GetMatchingRow("Accession", Accession[1]);
                if (studycount1 != null && studycount2 != null)
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

                /*
                    Step 4: From Tablet device (iPad) login iCA as the conference user, click the Conference Studies button on IPAD
                    Step 5: Select a top level folder and select a sub-level folder under it one level at a time until to the study folder reached
                    Step 6: Select a study and load the conference study into the viewer
                    Step 7: Ensure that the warning message “Not for Diagnostic Use" is displayed below the viewer.
                    Note: In iPAD the warning message is displayed without enabling it in config file. The warning message for both Desktop and iPAD are displayed at the lower left corner outside the viewport, in yellow font.
                    */
                // The Step 4 to 7 Cannot be automated because these steps need to be automated in Ipad.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 8: In iCA server Search for "NonDiagnosticUseWarningEnabled_desktop" in C:\WebAccess\WebAccess\Web.config and set the value to "true". Save the changes and Restart the services.
                login.Logout();
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "true");
                servicetool.RestartIISUsingexe();
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step 9: From Desktop: Login iCA as the conference user, navigate to the Conference Folders tab->active tab, select the top level folder, browse to a study folder, select a conference study and load it in the viewer.
                login.LoginIConnect(DomainAdminUser, DomainAdminPassword);
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
                conferencefolders.NavigateToActiveMode();
                conferencefolders.ExpandAndSelectFolder(Top + "/" + Sub[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = basepage.LaunchStudy(isConferenceTab: true);
                if (viewer.ViewStudy())
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

                //Step 10: Ensure that the warning message “Not for Diagnostic Use" is displayed properly
                if (string.Equals(viewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use"))
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
                //Step 11: Ensure that the lossy (80) label is displayed properly besides the non diagnostic warning message.
                if (string.Equals(viewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 12: Navigate to archive tab and load a study from archived folder
                viewer.CloseStudy();
                conferencefolders.NavigateToArchiveMode();
                conferencefolders.ExpandAndSelectFolder(Top + "/" + Sub[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = basepage.LaunchStudy(isConferenceTab: true);
                if (viewer.ViewStudy())
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
                //Step 13: Ensure that the warning message “Not for Diagnostic Use" is displayed properly
                if (string.Equals(viewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use"))
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
                //Step 14: Ensure that the lossy (80) label is displayed properly besides the non diagnostic warning message.
                if (string.Equals(viewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "false");
            }
        }

        /// <summary>
        /// Display Diagnostic Use Warning - Integrator
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_113325(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string Accession = string.Empty;
            string url = string.Empty;
            StudyViewer viewer = null;
            Studies studies = null;
            UserPreferences userpreferences = new UserPreferences();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));
                //Precondition
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                basepage.ClickElement(userpreferences.HTML4RadioBtn());
                userpreferences.CloseUserPreferences();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession);
                login.Logout();
                basepage.ChangeNodeValue(Config.FileLocationPath, "/Html5/EnableHTML5Support", "true");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/Html5/DefaultViewer", "Legacy");
                servicetool.RestartIISUsingexe();
                //Step 1:
                /*In the Service tool and select Integrator Tab, set
                User Sharing = Always Disabled
                Shadow User = Always Disabled*/
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Disabled", shadowuser: "Always Disabled");
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 2: Configure TestEHR
                ExecutedSteps++;
                //Step 3: Goto"C:\WebAccess\WebAccess\bin" Launch TestEHR application.
                ehr.LaunchEHR();
                if (WpfObjects._mainWindow.Visible)
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
                //Step 4: From TestEHR Select Image Load tab, enter the Accession Number that matches one study that is in one of the data source in the domain. Click on CMD line and copy the generated the URL and launch it in a browser.
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", role: "SuperRole", user: "Administrator");
                ehr.SetSearchKeys_Study(Accession);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (viewer.ViewStudy(IntegratedDesktop: true))
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
                //Step 5: Ensure that the warning message “Not for Diagnostic Use" is not displayed . Note: Desktop does not display the "Not for Diagnostic Use" warning by default user has to enable it from the config file
                if (viewer.NonDiagnosticUseWarningLabel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                /* 
                Step 6: Launch the study with the copied URL in IPAD
                Step 7: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                Step 8: From the 'View Name' dropdown field select the viewer being tested 'HTML5' as the default viewer, and enter the Accession Number that matches one study that is in one of the data source domains. Click on CMD line and copy the generated the URL.
                Step 9: Launch the study with the copied URL in IPAD.
                Step 10: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                */
                // The Step 6 to Step 10 Cannot be automated because these steps need to be validated in Ipad.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 11: In iCA server Search for "NonDiagnosticUseWarningEnabled_desktop" in C:\WebAccess\WebAccess\Web.config and set the value to "true". Save the changes and Restart the services.
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "true");
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;
                //Step 12: From TestEHR Select Image Load tab, enter the Accession Number that matches one study that is in one of the data source in the domain. Click on CMD line and copy the generated the URL and launch it in a desktop browser
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", role: "SuperRole", user: "Administrator");
                ehr.SetSearchKeys_Study(Accession);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (viewer.ViewStudy(IntegratedDesktop: true))
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
                //Step 13: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                if (string.Equals(viewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(viewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 14: In TestEHR, from the 'View Name' dropdown field select the viewer being tested 'HTML5' as the default viewer and enter the Accession Number that matches one study that is in one of the data source domains. Click on CMD line and copy the generated the URL and launch it in a desktop browser.
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", role: "SuperRole", user: "Administrator");
                ehr.SetSearchKeys_Study(Accession);
                ehr.SetSelectorOptions(viewName: "HTML5");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (viewer.ViewStudy(IntegratedDesktop: true, html5: true))
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
                //Step 15: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                if (string.Equals(viewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && !viewer.LossyCompressedLable("studyview").Displayed)
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
                //Step 16: In the Service tool and select Integrator Tab, set  User Sharing = Always Enabled Shadow User = Always Enabled
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled", shadowuser: "Always Enabled");
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 17: From TestEHR Select Image Load tab, enter the Accession Number that matches one study that is in one of the data source in the domain. Click on CMD line and copy the generated the URL and launch it in a desktop browser.
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", role: "SuperRole", user: "Administrator");
                ehr.SetSearchKeys_Study(Accession);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (viewer.ViewStudy(IntegratedDesktop: true))
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
                //Step 18: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                if (string.Equals(viewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && string.Equals(viewer.LossyCompressedLable("studyview").Text, "Lossy (80)"))
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
                //Step 19: Launch the study with the copied URL in IPAD.
                //Step 20: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                // The Step 19 and Step 20 Cannot be automated because these steps need to be validated in Ipad.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 21: In TestEHR, from the 'View Name' dropdown field select the viewer being tested 'HTML5' as the default viewer and enter the Accession Number that matches one study that is in one of the data source domains. Click on CMD line and copy the generated the URL and launch it in a desktop browser.
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", role: "SuperRole", user: "Administrator");
                ehr.SetSearchKeys_Study(Accession);
                ehr.SetSelectorOptions(viewName: "HTML5");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (viewer.ViewStudy(IntegratedDesktop: true, html5: true))
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
                //Step 22: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                if (string.Equals(viewer.NonDiagnosticUseWarningLabel().Text, "Not for Diagnostic Use") && !viewer.LossyCompressedLable("studyview").Displayed)
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
                //Step 23: Launch the study with the copied URL in IPAD.
                //Step 24: Ensure that the warning message “Not for Diagnostic Use" and lossy (80) label is displayed below the viewer.
                // The Step 23 and Step 24 Cannot be automated because these steps need to be validated in Ipad.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
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
                ehr.CloseEHR();
                servicetool.CloseServiceTool();
                try
                {
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    userpreferences.OpenUserPreferences();
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                    basepage.ClickElement(userpreferences.HTML5RadioBtn());
                    userpreferences.CloseUserPreferences();
                    login.Logout();
                }
                catch (Exception) { }
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.NotForDiagnosticUse_desktop']", "value", "false");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/Html5/EnableHTML5Support", "true");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/Html5/DefaultViewer", "HTML5");
                servicetool.RestartIISUsingexe();
            }
        }

    }
}
