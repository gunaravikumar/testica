using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Microsoft.Win32;
using System.Diagnostics;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.eHR;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
namespace Selenium.Scripts.Tests
{
    class SoftwareLabeling
    {
        public Login login { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public string filepath { get; set; }
        public ServiceTool servicetool { get; set; }
        public EHR ehr { get; set; }
        public String favurl { get; set; }
        DomainManagement domain = new DomainManagement();
        RoleManagement role = new RoleManagement();
        WpfObjects wpfobject = new WpfObjects();


        public SoftwareLabeling(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            configure = new Configure();
            hphomepage = new HPHomePage();
            servicetool = new ServiceTool();
            ehr = new EHR();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Online Help - Desktop
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168006(String testid, String teststeps, int stepcount)
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

                //Step 5: Click on one of the Chapter sections on the left hand side
                onlinehelp.NavigateToOnlineHelpFrame("tocIFrame");
                IList<IWebElement> mainchapters = onlinehelp.MainChapters();
                mainchapters[0].Click();
                //linehelp.ClickElement(onlinehelp.GetMainChapters()[" Chapter 1 Overview"]);
                if (onlinehelp.GetSubChapters("About the Application").Count > 1)
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
                
                //Step 6 :- Click the sub headings and verify the contents loaded on right hand side.
                //onlinehelp.OpenChapter("Overview");
                onlinehelp.OpenChapter("Precautions");
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                IList<IWebElement> Headers = onlinehelp.MainSectionHeaders();
                if (Headers.Any(a => a.Text.Equals("Precautions")))
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


                //Step 7 :- Validate the listed headings are listed in ascending order under index menu
                //Get all the headings listed under Index menu
                onlinehelp.NavigateToOnlineHelpFrame("indexcontentframe");
                IList<String> IndexHeadings = onlinehelp.GetIndexKeywords();
                var orderedByAsc = IndexHeadings.OrderBy(d => d);
                ExecutedSteps++;

                //Enter a keyword in search box
                onlinehelp.EnterKeyword("Patient");
                onlinehelp.NavigateToOnlineHelpFrame("indexcontentframe");

                //Step 8 :- Validate chapter is highlighted for the entered text
                if (onlinehelp.HighlightedChapter().Text.ToLower().StartsWith("patient") && onlinehelp.HighlightedChapter().Displayed)
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
                onlinehelp.GetIndexKeywordElements()["linking series"].Click();
                PageLoadWait.WaitForPageLoad(15);

                //Navigate to Main Content frame
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                Headers = onlinehelp.MainSectionHeaders();
                ExecutedSteps++;

                //Click Search menu
                onlinehelp.OpenMenu("search");
                onlinehelp.NavigateToOnlineHelpFrame("searchformframe");

                //Step 10 :- Verify keyword field under Search menu is displayed correctly
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

                //Step 11 :- Check the entered keyword lists the related articles
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

                //Step 12 :- Validate selected article contents are opened in right side viewer
                if (onlinehelp.BodyContent()[0].FindElement(By.CssSelector("font")).Text.ToLower().Equals("review"))
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

                //Step 13 :- Close Study viewer and Validate studies page is navigated
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);
                viewer.CloseStudy();
                ExecutedSteps++;

                //Open About iConnect Access splash screen
                login.OpenHelpAboutSplashScreen();

                //Step 14 :- Verify UDI is displayed in About iConnect Access splash screen
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

                //Step 15 :- Validate Part, Date and Revision are displayed correctly as in Step 4
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

                //Step 16 :- Logout the application
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);
                login.Logout();
                ExecutedSteps++;

                //Step 17 :- Multiple browser validation
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
        /// UDI Label
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168003(String testid, String teststeps, int stepcount)
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
                if (login.HelpWebAccessLoginLogo().GetAttribute("innerHTML").Contains(Config.buildversion))
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
                if (UDItext.StartsWith("UDI:(01)" + DeviceIdentifierNo) && UDItext.Contains("(10)" + Config.buildversion)
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
                String BatchNo = UDItext.Split(new String[] { "(10)", "(11)" }, StringSplitOptions.RemoveEmptyEntries)[1];

                //Step 5 :- Validate Batch/Lot number is assigned based on the build 
                if (BatchNo.Equals(Config.buildversion))
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
                result.steps[++ExecutedSteps].status = "PASS";
                result.steps[++ExecutedSteps].status = "PASS";

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
                ehr.SetSearchKeys_Study(Accession);
                ehr.SetSearchKeys_Study(login.GetHostName(Config.EA91), "Datasource");
                String url_12 = ehr.clickCmdLine("ImageLoad");

                //Step 12 :- Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_12);
                ExecutedSteps++;

                //Click logout in Test-EHR application
                String logoutURL = ehr.ClickLogout();
                login.NavigateToIntegratorURL(logoutURL);
                ehr.CloseEHR();

                //Step 13 :- Validate UDI should not be dispayed on EHR logout page
                ExecutedSteps++;

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

        // <summary>
        /// Software Labeling Req: EV
        /// </summary>
        /// 

        public TestCaseResult Test_168823(String testid, String teststeps, int stepcount)
        {

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
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step :1 - Login as any user wherein Enterprise viewer is set as default , 
                //load any study in Enterprise viewer and click on About Dialog from Help section
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                StudyViewer viewerEV = StudyViewer.LaunchStudy();
                viewerEV.HoverElement(By.CssSelector("li[title='About iConnect® Access'] a.AnchorClass32.toplevel"));
                viewerEV.ClickElement(BasePage.Driver.FindElement(By.CssSelector("a[title='About iConnect® Access']")));
                //Get UDI text
                String UDItext = login.UDIText().Text.Trim();
                bool IBMlogo = viewerEV.GetElement("cssselector", BluRingViewer.div_IBMheaderUV).Displayed;
                if (login.UDIText().Displayed && UDItext.Contains("(10)" + Config.buildversion) && IBMlogo)
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

                //Step :2 -- 	Verify the manufacturer name and address in the About Dialog
                bool manufactureraddress = viewerEV.GetElement("cssselector", BluRingViewer.div_manufactureraddressEV).Displayed;
                if (manufactureraddress)
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

                //Step :3 -- Verify About dialog includes the "Manufacturer" symbol next to the manufacturer name and address.
                bool Datemanufacturer = viewerEV.GetElement("cssselector", BluRingViewer.div_DatemanufacturerEV).Displayed;
                if (Datemanufacturer)
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

                //Step:4 --Verify About dialog include the date of manufacturer in the format of YYYY-MM-DD
                bool Manufacturername = viewerEV.GetElement("cssselector", BluRingViewer.div_ManufacturernameEV).Displayed;
                if (Manufacturername)
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

                //Step:5 -- Verify About dialog includes the "Catalogue number" symbol and the product part number with the symbol next to the part number
                bool Cataloguenumbersymbol = viewerEV.GetElement("cssselector", BluRingViewer.div_CataloguenumbersymbolEV).Displayed;
                if (Cataloguenumbersymbol)
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

                //Step:6 -- Verify About dialog includes the "Authorized representative in the European Community" symbol and the EC Representative name and address:
                bool EuropeanCommunity = viewerEV.GetElement("cssselector", BluRingViewer.div_EuropeanCommunityEV).Displayed;
                bool addressEmergo = viewerEV.GetElement("cssselector", BluRingViewer.div_addressEmergoEV).Displayed;
                if (EuropeanCommunity && addressEmergo)
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

                //Step 7: -- Verify About dialog includes the Australian Sponsor:
                bool AustralianSponsor = viewerEV.GetElement("cssselector", BluRingViewer.div_AustralianSponsorEV).Displayed;
                if (AustralianSponsor)
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

                //Step 8: -- Verify About Dialog includes the CE mark with notified body number.
                bool CEmark = viewerEV.GetElement("cssselector", BluRingViewer.div_CEmarkEV).Displayed;
                if (CEmark)
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

                //Step :9 -- Verify About dialog includes the "Consult instructions for use" symbol.
                bool Consultinstructions = viewerEV.GetElement("cssselector", BluRingViewer.div_ConsultinstructionsEV).Displayed;
                if (Consultinstructions)
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

                //Step :10 -- Verify About dialog includes the symbol statement "Rx only".
                bool Rxonly = viewerEV.GetElement("cssselector", BluRingViewer.div_RxonlyEV).Displayed;
                if (Rxonly)
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

                //Step :11 -- Verify About Dialog contain the copyright information
                bool CopyRight = viewerEV.GetElement("cssselector", BluRingViewer.div_CopyRight).Displayed;
                if (CopyRight)
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

                //Step :12 --Verify that the following parts are available in the UDI.
                // a.Device Identifier number - which is availble right after '(01)'
                //b.Product Identifier number - which is availble right after '(10)'
                // c.Manufacture date -which is available right after '(11)'

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

                //Step: 13 - Click on the Online Help
                OnlineHelp onlinehelp = new OnlineHelp().OpenHelpandSwitchtoIT();
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
                ExecutedSteps++;
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);

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
        /// Software Labeling Req
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168004(String testid, String teststeps, int stepcount)
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
                DomainManagement domain = new DomainManagement();
                RoleManagement role = new RoleManagement();
                UserManagement usermanagement = new UserManagement();
                RoleManagement rolemanagement = new RoleManagement();
                Maintenance maintenance = null;
                ConferenceFolders conferencefolders;
                Taskbar taskbar = new Taskbar();
                string line;
                int i = 2;
                string accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String DeviceIdentifierNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Device Identifier");
                string buildno = login.GetBuildID();

                //Pre-condition:
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableConferenceLists();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                taskbar.Show();

                //Step 1: After installation, go to the installed programs list located by going to the Programs and Features in the Control Panel on the server.
                String Appversion = BasePage.GetInstalledAppVersion("IBM iConnect Access");
                if ((Config.buildversion+".0").Contains(Appversion))
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

                //step 2: Verify that released version number is displayed on the version column for both main application and service tool
                if (File.Exists(@"c:\Installlist.txt"))
                {
                    File.Delete(@"c:\Installlist.txt");
                }
                string strcmd = @"wmic  > C:\InstallList.txt product get name,version";
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strcmd);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
                using (StreamReader sr = new StreamReader("C:\\Installlist.txt"))
                {
                    while ((line = sr.ReadLine()) != null && i > 0)
                    {
                        if (line.Contains("IBM iConnect Access Service Tool") || line.Contains("IBM iConnect Access"))
                        {
                            if (line.Contains("7.1"))
                                i--;
                        }
                    }
                }
                if (i == 0)
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

                //Step 3: On the ICA server go to the C:\WebAccess\Build.info
                if (BasePage.GetBuildDetails()["Build Number"].Equals(Config.buildnumber))
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

                //Step 4: Launch http://Servername/webaccess in any browser.
                login.DriverGoTo(login.url);
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

                //Step 5: Login as any user, Load any study in Universal viewer and Click on About Dialog from Help section
                login.LoginIConnect(adminusername, adminpassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                var step2_1 = login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion);
                String step2_2 = BasePage.Driver.Title;
                if (step2_1 && step2_2.Equals("IBM iConnect® Access"))
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

                //Step 6: Verify the manufacturer name and address in the About Dialog
                viewer.OpenAboutSplashScreen();
                var step6_1 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Manufacturername).Displayed;
                var step6_2 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_manufactureraddress).Displayed;
                if (step6_1 && step6_2)
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

                //Step 7: Verify About dialog includes the "Manufacturer" symbol [Solid filled] next to the manufacturer name and address.
                var step7 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_DatemanufacturerUV).Displayed;
                ExecutedSteps++;

                //Step 8: Verify About dialog include the date of manufacturer in the format of YYYY-MM-DD.
                var Date = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_manufactureraddressUV).GetAttribute("innerHTML");
                var convertedDate = String.Format("yyyy-mm-dd", Date);
                ExecutedSteps++;

                //Step 9: Verify About dialog includes the "Date of manufacturer" symbol [Outline] next to the date of manufacture.
                var step9 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ManufacturernameUV).Displayed;
                if (step9)
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
                //Step 10: Verify About dialog includes the "Catalogue number" symbol and the product part number with the symbol next to the part number.
                var step10 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CataloguenumbersymbolUV).Displayed;
                if (step10)
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
                //Step 11: Verify About dialog includes the "Authorized representative in the European Community" symbol and the EC Representative name and address:
                var step11 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_EuropeanCommunityUV).Displayed;
                if (step11)
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
                //Step 12: Verify About dialog includes the Australian Sponsor:
                var step12 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_AustralianSponsorUV).Displayed;
                if (step12)
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
                //Step 13 :Verify About Dialog includes the CE mark with notified body number.
                var step13 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CEmarkUV).Displayed;
                if (step13)
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


                //step 14 : Verify About dialog includes the "Consult instructions for use" symbol.
                var step14 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ConsultinstructionsUV).Displayed;
                if (step14)
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


                //Step 15 : Verify About dialog includes the symbol statement "Rx only".
                var step15 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_RxonlyUV).Displayed;
                if (step15)
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

                //Step 16 : Verify About Dialog contain the copyright information
                var step16 = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_CopyRightUV).Displayed;
                if (step16)
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

                // Step:17 \\Verify that the following parts are available in the UDI.
                //a.Device Identifier number - which is availble right after '(01)'
                //b.Product Identifier number - which is availble right after '(10)'
                //c.Manufacture date -which is available right after '(11)'
                // Get UDI text
                String UDITextUV = login.UDITextUV().Text.Trim();
                if (login.UDITextUV().Displayed && UDITextUV.StartsWith("UDI:(01)") && UDITextUV.Contains("(10)" + Config.buildversion)
                    && UDITextUV.Contains("(11)"))
                {
                    //Get Build Date 
                    String buildDate = BasePage.GetBuildDetails()["Date"];
                    DateTime Date1 = DateTime.ParseExact(buildDate.Split(new String[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries)[1], "mm/dd/yyyy", System.Globalization.CultureInfo.CurrentUICulture);
                    String BuildDate = Date1.ToString("yymmdd");//Date.Year.ToString().Replace("20", String.Empty) + Date.Month.ToString() + Date.Day;

                    //Verify UDI is displayed in About iConnect Access splash screen with correct details
                    if (UDITextUV.StartsWith("UDI:(01)" + DeviceIdentifierNo) && UDITextUV.Contains("(10)" + Config.buildversion)
                        && UDITextUV.EndsWith("(11)" + BuildDate))
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseAboutSplashScreen();
                BasePage.Driver.FindElement(By.CssSelector("div.aboutDialogHeader div.closeButton")).Click();
                viewer.CloseBluRingViewer();

                //Step :18 Click on the Online Help
                var windows = BasePage.Driver.WindowHandles;
                OnlineHelp onlinehelp = new OnlineHelp().OpenHelpandSwitchtoIT(0);
                onlinehelp.NavigateToOnlineHelpFrame("topic");
                BasePage.wait.Until(ExpectedConditions.ElementExists(onlinehelp.By_OnlineHelpVersion));
                ExecutedSteps++;
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(BasePage.Driver.WindowHandles[0]);

                //Step :19 From Domain Management tab, edit any configured domain and launch the About Dialog
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("conferencelists", 0);
                login.OpenHelpAboutSplashScreen();
                bool step19 = domain.VerifyAboutScreenElements();
                domain.CloseHelpAboutSplashScreen();
                domain.ClickSaveEditDomain();
                if (step19)
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

                //Step: 20 -- From Role Management tab, edit any configured roles and launch the About Dialog
                role = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.ShowRolesFromDomainDropDown();
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("conferenceuser", 0);
                role.OpenHelpAboutSplashScreen();
                bool step20 = role.VerifyAboutScreenElements();
                role.CloseHelpAboutSplashScreen();
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
                }
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CloseButton")).Click();
                //Step :21 From User Management tab, navigate edit any configured users and launch the About Dialog
                String DomainName = "SuperAdminGroup";
                String FirstName = "superAdmin";
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                login.Navigate("UserManagement");
                usermanagement.SearchUser(FirstName, DomainName);
                usermanagement.SelectUser("SuperAdmin");
                usermanagement.ClickButtonInUser("edit");
                usermanagement.OpenHelpAboutSplashScreen();
                bool step21 = usermanagement.VerifyAboutScreenElements();
                usermanagement.CloseHelpAboutSplashScreen();
                if (step21)
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
                BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CloseButton")).Click();

                // Step :22 Launch About Dialog from various tabs like ImageSharing, Conference Folder, Maintenance etc
                SystemSettings settings = (SystemSettings)login.Navigate("SystemSettings");
                settings.OpenHelpAboutSplashScreen();
                bool step22 = settings.VerifyAboutScreenElements();
                settings.CloseHelpAboutSplashScreen();
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.OpenHelpAboutSplashScreen();
                bool step22_1 = maintenance.VerifyAboutScreenElements();
                maintenance.CloseHelpAboutSplashScreen();
                conferencefolders = (ConferenceFolders)login.Navigate("ConferenceFolders");
                conferencefolders.OpenHelpAboutSplashScreen();
                bool step22_2 = conferencefolders.VerifyAboutScreenElements();
                conferencefolders.CloseHelpAboutSplashScreen();
                if (step22 && step22_1 && step22_2)
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

                //Step :23 Logout of ICA and return to the Login page.
                login.Logout();
                BasePage.Driver.SwitchTo().DefaultContent();
                var step23 = login.LoginStylesheetLink().GetAttribute("href").Contains(Config.buildversion);
                String step23_1 = BasePage.Driver.Title;
                if (step23 && step23_1.Equals("IBM iConnect® Access"))
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

                //Step :24 Open ICA login page using Chrome/FF and hit F12. In
                string pagesource = BasePage.Driver.PageSource;
                bool Buildversion = pagesource.Contains(Config.buildversion);
                if (Buildversion)
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

                //Step: 25 -- 	Setup TestEHR with bypass configuration
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

                // Step 26: Load a study from TestEHR application
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSearchKeys_Study(accession);
                ehr.SetSearchKeys_Study(login.GetHostName(Config.EA91), "Datasource");
                String url_25 = ehr.clickCmdLine("ImageLoad");
                login = new Login();
                login.NavigateToIntegratorURL(url_25);
                ExecutedSteps++;

                // Step 27 : Click on Logout option from TestEHR
                String logoutURL = ehr.ClickLogout();
                login.NavigateToIntegratorURL(logoutURL);
                ehr.CloseEHR();
                ExecutedSteps++;

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

        public TestCaseResult Test_168005(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables        
            BasePage.MultiDriver.Clear();
            BasePage.MultiDriver.Add(BasePage.Driver);
            string BrowserType = Config.BrowserType;
            //Fetch required Test data  
            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            string[] SupportedBrowsers = Enumerable.Repeat(string.Empty, 2).ToArray();
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                //Step1 - Launch iCA application (Standalone) in any browser - Chrome/FF/IE
                login.DriverGoTo(login.favurl);
                Studies fav = new Studies();
                BluRingViewer viewer = new BluRingViewer();
                favurl = viewer.favurl;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String goldimage11 = "";
                String testimage11 = "";
                if (Config.compareimages.ToLower().Equals("n"))
                {
                    goldimage11 = result.steps[ExecutedSteps].goldimagepath;
                    viewer.DownloadAnyFormatImage(favurl, goldimage11);
                    testimage11 = result.steps[ExecutedSteps].testimagepath;
                }
                else
                {
                    testimage11 = result.steps[ExecutedSteps].testimagepath;
                    viewer.DownloadAnyFormatImage(favurl, testimage11);
                    goldimage11 = result.steps[ExecutedSteps].goldimagepath;
                }
                bool step1_1 = BasePage.CompareImage(goldimage11, testimage11, 200);
                if (BasePage.SBrowserName.ToLower().Contains("chrome"))
                {
                    SupportedBrowsers[0] = "firefox";
                    SupportedBrowsers[1] = "internet explorer";
                }
                else if (BasePage.SBrowserName.ToLower().Contains("firefox"))
                {
                    SupportedBrowsers[0] = "chrome";
                    SupportedBrowsers[1] = "internet explorer";
                }
                else
                {
                    SupportedBrowsers[0] = "chrome";
                    SupportedBrowsers[1] = "firefox";
                }
                Logger.Instance.InfoLog("First Supported Browser is = " + SupportedBrowsers[0]);
                BasePage.MultiDriver.Add(login.InvokeBrowser(SupportedBrowsers[0]));
                Config.BrowserType = SupportedBrowsers[0];
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.favurl);
                viewer = new BluRingViewer();
                favurl = viewer.favurl;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                String goldimage12 = "";
                String testimage12 = "";
                if (Config.compareimages.ToLower().Equals("n"))
                {
                    goldimage12 = result.steps[ExecutedSteps].goldimagepath;
                    viewer.DownloadAnyFormatImage(favurl, goldimage12);
                    testimage12 = result.steps[ExecutedSteps].testimagepath;
                }
                else
                {
                    testimage12 = result.steps[ExecutedSteps].testimagepath;
                    viewer.DownloadAnyFormatImage(favurl, testimage12);
                    goldimage12 = result.steps[ExecutedSteps].goldimagepath;
                }
                bool step1_2 = BasePage.CompareImage(goldimage12, testimage12, 200);
                Logger.Instance.InfoLog("Second Supported Browser is = " + SupportedBrowsers[1]);
                BasePage.MultiDriver.Add(login.InvokeBrowser(SupportedBrowsers[1]));
                Config.BrowserType = SupportedBrowsers[1];
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.favurl);
                viewer = new BluRingViewer();
                favurl = viewer.favurl;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                String goldimage13 = "";
                String testimage13 = "";
                if (Config.compareimages.ToLower().Equals("n"))

                {
                    goldimage13 = result.steps[ExecutedSteps].goldimagepath;
                    viewer.DownloadAnyFormatImage(favurl, goldimage13);
                    testimage13 = result.steps[ExecutedSteps].testimagepath;
                }
                else
                {
                    testimage13 = result.steps[ExecutedSteps].testimagepath;
                    viewer.DownloadAnyFormatImage(favurl, testimage13);
                    goldimage13 = result.steps[ExecutedSteps].goldimagepath;
                }
                bool step1_3 = BasePage.CompareImage(goldimage13, testimage13, 200);
                if (step1_1 && step1_2 && step1_3)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                BasePage.Driver = BasePage.MultiDriver[0];
                Config.BrowserType = BrowserType;
                login.InvokeBrowser(Config.BrowserType);
                login.closeallbrowser();
                // Step 2 - Launch iCA URL from Integrator/TestEHR in any browser 
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
                // Launch Test-EHR application
                ehr.LaunchEHR();
                //Open Study in Viewer with TestEHR as iCA Admin user
                ehr.SetCommonParameters();
                ehr.SetSearchKeys_Study(Accession);
                ehr.SetSearchKeys_Study(login.GetHostName(Config.EA91), "Datasource");
                String ehrurl = ehr.clickCmdLine("ImageLoad");
                ehr.Load();
                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(ehrurl);
                viewer = new BluRingViewer();
                favurl = viewer.favurl;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String goldimage21 = "";
                String testimage21 = "";
                if (Config.compareimages.ToLower().Equals("n"))
                {
                    goldimage21 = result.steps[ExecutedSteps].goldimagepath;
                    viewer.DownloadAnyFormatImage(favurl, goldimage21);
                    testimage21 = result.steps[ExecutedSteps].testimagepath;
                }
                else
                {
                    testimage21 = result.steps[ExecutedSteps].testimagepath;
                    viewer.DownloadAnyFormatImage(favurl, goldimage21);
                    goldimage21 = result.steps[ExecutedSteps].goldimagepath;
                }
                bool step2_1 = BasePage.CompareImage(goldimage21, testimage21, 200);
                //Click logout in Test-EHR application
                String logoutURL = ehr.ClickLogout();
                login.NavigateToIntegratorURL(logoutURL);
                ehr.CloseEHR();
                login.closeallbrowser();
                
                if (step2_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
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
            finally
            {
                BasePage.Driver = BasePage.MultiDriver[0];
                Config.BrowserType = BrowserType;
                login.InvokeBrowser(Config.BrowserType);
                login.closeallbrowser();
            }
        }

        public TestCaseResult Test_168075(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            StudyViewer viewer = null;
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            
            try
            {
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                Studies studies = null;

                // step 1 Launch iCA homepage - http://IP address/webaccess or https://servername/webaccess [If HTTPS is enabled]
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                StudyViewer viewerEV = StudyViewer.LaunchStudy();
                viewerEV.HoverElement(By.CssSelector("li[title='About iConnect® Access'] a.AnchorClass32.toplevel"));
                viewerEV.ClickElement(BasePage.Driver.FindElement(By.CssSelector("a[title='About iConnect® Access']")));
                //Get UDI text
                String UDItext = login.UDIText().Text.Trim();
                bool IBMlogo = viewerEV.GetElement("cssselector", BluRingViewer.div_IBMheaderUV).Displayed;
                if (login.UDIText().Displayed && UDItext.Contains("(10)" + Config.buildversion) && IBMlogo)
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

                // Step 2 Verify the contents in the homepage across all supported browsers
                bool manufactureraddress = viewerEV.GetElement("cssselector", BluRingViewer.div_manufactureraddressEV).Displayed;
                if (manufactureraddress)
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
                // Step 3 - Login as any user that has studies in Inbounds and click on Email Study button
                bool Datemanufacturer = viewerEV.GetElement("cssselector", BluRingViewer.div_DatemanufacturerEV).Displayed;
                if (Datemanufacturer)
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
                // Step 4 - Click on the link

                bool Manufacturername = viewerEV.GetElement("cssselector", BluRingViewer.div_ManufacturernameEV).Displayed;
                if (Manufacturername)
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

                // Step 5 - click on "Invite to Upload" button
                bool Cataloguenumbersymbol = viewerEV.GetElement("cssselector", BluRingViewer.div_CataloguenumbersymbolEV).Displayed;
                if (Cataloguenumbersymbol)
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

                // Step 6 - Click on the link 

                bool EuropeanCommunity = viewerEV.GetElement("cssselector", BluRingViewer.div_EuropeanCommunityEV).Displayed;
                bool addressEmergo = viewerEV.GetElement("cssselector", BluRingViewer.div_addressEmergoEV).Displayed;
                if (EuropeanCommunity && addressEmergo)
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

                // Step 7- Verify the contents of the window across all supported browsers
                bool AustralianSponsor = viewerEV.GetElement("cssselector", BluRingViewer.div_AustralianSponsorEV).Displayed;
                if (AustralianSponsor)
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

                //Step 8- Click on the link

                bool CEmark = viewerEV.GetElement("cssselector", BluRingViewer.div_CEmarkEV).Displayed;
                if (CEmark)
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

                //Step :9- Verify the contents of the window across all supported browsers

                bool Consultinstructions = viewerEV.GetElement("cssselector", BluRingViewer.div_ConsultinstructionsEV).Displayed;
                if (Consultinstructions)
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
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
        }

        // <summary>
        /// Ability to edit UDI file
        /// </summary>
        public TestCaseResult Test_168104(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String DeviceIdentifierNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Device Identifier");
            try
            {

                //Fetch required Test data  
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                BluRingViewer viewer = new BluRingViewer();
                Studies studies = new Studies();
                StudyViewer StudyViewer = new StudyViewer();
                var sourceDirectory = @"C:\WebAccess\UDI.txt";
                var backupDirectory = @"C:\Users\Administrator\Desktop\UDI.txt";
                //Step:1 - Navigate to C:\Webaccess folder in the iCA server and launch UDI.txt file
                File.Copy(sourceDirectory, backupDirectory, true);
                String UDI = @"C:\WebAccess\UDI.txt";
                string readText = File.ReadAllText(UDI);
                string[] ssplit = readText.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenAboutSplashScreen();
                //Verify that the following parts are available in the UDI. 
                string Version = viewer.GetElement("cssselector", BluRingViewer.p_IBMAboutScreenVersion).Text;
                string UDItext = viewer.GetElement("cssselector", BluRingViewer.p_IBMAboutScreenUDInumber).Text;
                String buildDate = BasePage.GetBuildDetails()["Date"];
                DateTime Date = DateTime.ParseExact(buildDate.Split(new String[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries)[1], "mm/dd/yyyy", System.Globalization.CultureInfo.CurrentUICulture);
                String BuildDate = Date.ToString("yymmdd");//Date.Year.ToString().Replace("20", String.Empty) + Date.Month.ToString() + Date.Day;
                //Get Build number 
                string buildno = login.GetBuildID();
                bool UDItext1 = ssplit[0].Trim().Equals(UDItext);
                bool buildversion1 = ssplit[3].Contains("Version:" + Config.buildversion);
                bool buildno1 = ssplit[4].Contains("Build:" + buildno);
                bool BuildDate1 = ssplit[5].Contains("Date:" + BuildDate);
               
                if (UDItext1 && buildversion1 && buildno1 && BuildDate1)
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
                BasePage.Driver.FindElement(By.CssSelector("div.aboutDialogHeader div.closeButton")).Click();
                viewer.CloseBluRingViewer();
                login.Logout();
                //Step:2 - Modify UDI value, save the changes and launch About dialog [Help -> About] after logging in

                string text = File.ReadAllText(@"C:\WebAccess\UDI.txt");
                text = text.Replace("DI:(01)00842000100782", "DI:(01)00842000101105");
                File.WriteAllText(@"C:\WebAccess\UDI.txt", text);
                String UDI1 = @"C:\WebAccess\UDI.txt";
                string readText1 = File.ReadAllText(UDI1);
                string[] ssplit1 = readText1.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenAboutSplashScreen();
                string UDItextedit = viewer.GetElement("cssselector", BluRingViewer.p_IBMAboutScreenUDInumber).Text;
                bool UDItextedit1 = ssplit1[0].Trim().Equals(UDItextedit);
                viewer.GetElement("cssselector", BluRingViewer.div_AboutScreen + " " + BluRingViewer.div_AboutScreenCloseButton).Click();
                if (UDItextedit1)
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
                //Thread.Sleep(2000);
                //BasePage.Driver.FindElement(By.CssSelector("div.aboutDialogHeader div.closeButton")).Click();
                viewer.CloseBluRingViewer();
                login.Logout();
                //Step 3: Verify the contents in the About dialog from
                // -Studies tab
                //- Enterprise and Universal Viewer
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                viewer.OpenHelpAboutSplashScreen();
                bool step3 = viewer.VerifyAboutScreenElements();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step3_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Aboutbox));
                viewer.CloseHelpAboutSplashScreen();
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                StudyViewer viewerEV = StudyViewer.LaunchStudy();
                viewerEV.HoverElement(By.CssSelector("li[title='About iConnect® Access'] a.AnchorClass32.toplevel"));
                viewerEV.ClickElement(BasePage.Driver.FindElement(By.CssSelector("a[title='About iConnect® Access']")));
                bool step3_2 = viewerEV.VerifyAboutScreenElementsEV();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step3_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_Aboutbox));
                BasePage.Driver.FindElement(By.CssSelector("div #CloseHelpAboutButton")).Click();
                //viewerEV.CloseHelpAboutSplashScreen();
                StudyViewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenAboutSplashScreen();
                bool step3_4 = viewer.VerifyAboutScreenElementsUV();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step3_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_AboutboxUV));
                BasePage.Driver.FindElement(By.CssSelector("div.aboutDialogHeader div.closeButton")).Click();
                if (step3 && step3_1 && step3_2 && step3_3 && step3_4 && step3_5)
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
                viewer.CloseBluRingViewer();
                login.Logout();
                //Step 4: 	Modify any value in UDI text file to null and verify from the About Dialog
                string text1 = File.ReadAllText(@"C:\WebAccess\UDI.txt");
                text1 = text1.Replace("DI:(01)00842000100782", "DI:(01)");
                File.WriteAllText(@"C:\WebAccess\UDI.txt", text1);
                String UDI11 = @"C:\WebAccess\UDI.txt";
                string readText11 = File.ReadAllText(UDI11);
                string[] ssplit11 = readText11.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenAboutSplashScreen();
                string UDItextedit4 = viewer.GetElement("cssselector", BluRingViewer.p_IBMAboutScreenUDInumber).Text;
                bool UDItextedit11 = ssplit11[0].Trim().Equals(UDItextedit4);
                if (UDItextedit11)
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
                BasePage.Driver.FindElement(By.CssSelector("div.aboutDialogHeader div.closeButton")).Click();
                viewer.CloseBluRingViewer();
                login.Logout();

                //Step :5 Replace the original UDI information from the backup file taken in pre-condition and launch the About Dialog
                File.Copy(backupDirectory, sourceDirectory, true);
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                String UDI5 = @"C:\WebAccess\UDI.txt";
                string readText5 = File.ReadAllText(UDI5);
                string[] ssplit5 = readText.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenAboutSplashScreen();
                //Verify that the following parts are available in the UDI. 
                string Version5 = viewer.GetElement("cssselector", BluRingViewer.p_IBMAboutScreenVersion).Text;
                string UDItext5 = viewer.GetElement("cssselector", BluRingViewer.p_IBMAboutScreenUDInumber).Text;
                String buildDate5 = BasePage.GetBuildDetails()["Date"];
                DateTime Date5 = DateTime.ParseExact(buildDate.Split(new String[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries)[1], "mm/dd/yyyy", System.Globalization.CultureInfo.CurrentUICulture);
                String BuildDate5 = Date.ToString("yymmdd");//Date.Year.ToString().Replace("20", String.Empty) + Date.Month.ToString() + Date.Day;
                //Get Build number 
                string buildno5 = login.GetBuildID();
                bool UDItext51 = ssplit5[0].Trim().Equals(UDItext);
                bool buildversion5 = ssplit5[3].Contains("Version:" + Config.buildversion);
                bool buildno51 = ssplit5[4].Contains("Build:" + buildno);
                bool BuildDate51 = ssplit5[5].Contains("Date:" + BuildDate);

                if (UDItext51 && buildversion5 && buildno51 && BuildDate51)
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
                BasePage.Driver.FindElement(By.CssSelector("div.aboutDialogHeader div.closeButton")).Click();
                viewer.CloseBluRingViewer();
                login.Logout();
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
    }
}
