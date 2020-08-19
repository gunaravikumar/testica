using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Threading;
using System.Data;
using System.Xml;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages.MPAC;
using Dicom.Network;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data.SqlClient;
using Selenium.Scripts.Pages.HoldingPen;
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Tests
{
    class ReportPanel : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }
        public ExamImporter ei { get; set; }
        public WpfObjects wpfobject { get; set; }
        public ServiceTool servicetool { get; set; }
        public UserPreferences userpref { get; set; }

        Studies studies { get; set; }
        public BasePage basepage { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public StudyViewer studyviewer { get; set; }
        string MPUsername = Config.WindowsUserName;
        string MPPassword = "PQAte$t123-ica-mp-ws12";//Config.WindowsPassword;


        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public ReportPanel(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            ehr = new EHR();
            ei = new ExamImporter();
            basepage = new BasePage();
            mpaclogin = new MpacLogin();
            userpref = new UserPreferences();
            bluringviewer = new BluRingViewer();
            studies = new Studies();
        }

        /// <summary>
        /// Loading cardio reports from Exam List panel
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161666(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");


                //Step-1:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                //BasePage.MultiDriver.Add(BasePage.Driver);
                //login.SetDriver(BasePage.MultiDriver[0]);
                //Config.node = Config.Clientsys1;
                //BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
                //login.SetDriver(BasePage.MultiDriver[1]);
                //login.DriverGoTo(login.url);
                //login.LoginGrid(Config.adminUserName, Config.adminPassword);
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                tool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(ServiceTool.EnableFeatures.ID.EncapsulatedPDF)).Checked = true;
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.RestartService();
                tool.CloseServiceTool();
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Domain Management"))
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

                //Step-3:Navigate to studies tab and load a study with priors that has different modalities along with study that has only Cardio Report          
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.EA96));
                PageLoadWait.WaitForLoadingMessage(40);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-4:Mouse hover on the Report icon that has no report and verify the Report cursor should be displayed as "Arrow" cursor to indicate that it's not a clickable item.
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step4 = reportIcon.GetCssValue("cursor").Equals("auto");
                Logger.Instance.InfoLog("Cursor value is->"+ reportIcon.GetCssValue("cursor"));
                if (step4)
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

                //Step-5:Select the study prior study that has a cardio report(only DOC modality)
                IList<IWebElement> Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int before_click = Study_Panel.Count;

                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));

                Exam_list[0].Click();
                PageLoadWait.WaitForFrameLoad(20);
                BluRingViewer.WaitforViewports();
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int after_click = Study_Panel.Count;

                if (before_click == (after_click - 1))
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

                //Step-6:	Mouse hover on the Report icon and verify the Report cursor should be displayed as "PointingHand" cursor to indicate that it's a clickable item.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[1];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step6 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step6)
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

                //Step-7:Click on the 'Report' icon and verify the Cardio report for the study should be opened to the right of the exam list as a panel overlaying the images.	
                viewer.OpenReport_BR(1, "PDF");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[1];
                if (reportIcon.Displayed)
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

                //Step-8:View the Cardio report.  
                viewer.NavigateToReportFrame(reporttype: "PDF");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.PDFContainer_div));
                if (ReportContainer.Displayed)
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

                //Step-9:Verify the Cardio report should be appeared in dark mode i.e. dark background and white text.               
                string textcolor = viewer.GetColorInReport(0);//#ffffff
                string bgcolor = viewer.GetColorInReport(1);
                if (string.Equals(textcolor, "rgb(255, 255, 255)") && string.Equals(bgcolor, "rgb(50, 50, 50)"))
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
                viewer.CloseReport_BR(1);

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// Study Report Container
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161680(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Precondition:
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                tool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(ServiceTool.EnableFeatures.ID.EncapsulatedPDF)).Checked = true;
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.RestartService();
                tool.CloseServiceTool();

                //Step-1:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3:Navigate to Studies,Search and select study which has report.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Accession", accession);
                ExecutedSteps++;

                //Step-4:Click on "BluRing" button to load the selected study into the BluRing viewer.
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-5:Verify the Exam List column should opened by default.
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorcount = priors.Count;
                if (priorcount > 2)
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

                //Step-6:Mouse hover on the Report icon and verify the Report cursor should be displayed as Pointing hand cursor to indicate that it's a clickable item.
                IWebElement reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[1];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step6 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step6)
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

                //Step-7:Click on Report icon and verify the report for the study should be opened to the right of the exam list as a panel overlaying the images.                
                viewer.OpenReport_BR(1, "PDF");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.PDFContainer_div));
                if (ReportContainer.Displayed)
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

                //Step-8:Verify the report should not be overlapped on the scrollbar in the exam list.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[2];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step8 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step8)
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

                //Step-9:Verify the report should not be overlapped on the Global Toolbar and it should be displayed under Global Toolbar.
                viewer.CloseExamList();
                IWebElement Exam_list = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.HistoryPanel_div));
                var stylevalue = Exam_list.GetAttribute("style");
                viewer.OpenExamList();
                Exam_list = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.HistoryPanel_div));
                var stylevalue1 = Exam_list.GetAttribute("style");
                if (!(stylevalue.Contains("left: 0px")) && stylevalue1.Contains("left: 0px"))
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
                viewer.CloseReport_BR(1);

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// PDF and Structured reports appear in Dark Mode/Night Mode Scheme
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161668(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccList = accession.Split(':');
                String datasource = login.GetHostName(Config.SanityPACS);

                //Precondition:
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                tool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(ServiceTool.EnableFeatures.ID.EncapsulatedPDF)).Checked = true;               
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.RestartService();
                tool.CloseServiceTool();

                //Step-1:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3:Navigate to Studies,Search and select study that has only PDF report.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccList[0]);
                studies.SelectStudy("Accession", AccList[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.CloseBluRingViewer();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccList[0]);
                studies.SelectStudy("Accession", AccList[0]);
                ExecutedSteps++;

                //Step-4:Click on "BluRing" button to load the selected study into the BluRing viewer.
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-5:Mouse hover on the Report icon and verify the Report cursor should be displayed as Pointing hand cursor to indicate that it's a clickable item.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step6 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step6)
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

                //Step-6:Click on Report icon and verify the PDF report should be loaded without any error when user clicks on Report icon.
                viewer.OpenReport_BR(0,"PDF");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.PDFContainer_div));
                if (ReportContainer.Displayed)
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


                //Step-7:Verify the PDF reports should be appeared in dark mode i.e. dark background and white text.   
                IList<IWebElement> svg = BasePage.Driver.FindElements(By.CssSelector("div.page div.canvasWrapper svg"));
                IList<IWebElement> svgtspan =BasePage.Driver.FindElements(By.CssSelector("div.page div.canvasWrapper svg tspan[fill='rgb(255, 255, 255)']"));//#ffffff
                string textcolor = svgtspan[3].GetAttribute("fill");//#ffffff,white
                //string bgcolor = svg[0].GetAttribute("style");bgcolor.Contains("rgb(50, 50, 50)") &&
                if (textcolor.Contains("rgb(255, 255, 255)") &&  string.Equals(svgtspan[8].Text, "A free imaging sharing network"))
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
                viewer.CloseReport_BR(0);
                viewer.CloseBluRingViewer();

                //Step-8:Navigate to Studies,Search and select study that has only SRS report.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccList[1], Datasource: datasource);
                studies.SelectStudy("Accession", AccList[1]);
                ExecutedSteps++;


                //Step-9:Click on "BluRing" button to load the selected study into the BluRing viewer.
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;


                //Step-10:Mouse hover on the Report icon and verify the Report cursor should be displayed as Pointing hand cursor to indicate that it's a clickable item.                
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step10 = reportIcon.GetCssValue("cursor").Equals("pointer");
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

                //Step-11:Click on Report icon from the Exam List Panel and verify the SRS report should be loaded without any error when user clicks on Report icon.
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                if (ReportContainer.Displayed)
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

                //Step-12:Verify the SRS reports should be appeared in dark mode i.e. dark background and white text.                
                textcolor = viewer.GetColorInReport(0, "SR");
                string bgcolor = viewer.GetColorInReport(1, "SR");
                if (string.Equals(textcolor, "white") && string.Equals(bgcolor, "#323232"))
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
                viewer.CloseReport_BR(0);
                

                //Step-13:Repeat the step 1 to 12 on IE11, FF (latest), Chrome, Safari 10/11, Edge browsers.
                ExecutedSteps++;

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// SRS Reports by clicking on the report icon
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161663(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;                
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");                
                String datasource = login.GetHostName(Config.SanityPACS);

                //Step-1:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3:Select Studies tab.
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step-4:Search and select the modality SR study and click on 'BluRing' button
                studies.SearchStudy(patientID: PatientID, Datasource: datasource);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;


                //Step-5:Mouse hover on the Report icon and verify the Report cursor should be displayed as Pointing hand cursor to indicate that it's a clickable item.
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step4 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step4)
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


                //Step-6:Click on Report icon from the Exam List Panel and verify the SRS report should be loaded without any error when user clicks on Report icon.
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                if (ReportContainer.Displayed)
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

                //Step-7:Verify the report for the study should be opened to the right of the exam list as a panel overlaying the images.
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step7 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step7)
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

                //Step-8:Verify the blue page should not be displayed before loading an SRS report.
                result.steps[++ExecutedSteps].status = "On Hold";

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// Loading HL7 Reports from Exam List Panel
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161670(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String datasource = login.GetHostName(Config.SanityPACS);

                
                //Step-1:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3:Select Studies tab.
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step-4:In the Studies tab, search for the study with HL7 Report attached in the MPACS(e.g Patient: LAAL,BALL Accession: LBL10)
                studies.SearchStudy(AccessionNo:accession,Datasource:datasource);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-5:Verify the Exam List column should opened by default.
                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                if (Exam_list[0].Displayed == true)
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


                //Step-6:Mouse hover on the Report icon and verify the Report cursor should be displayed as "Pointing hand" cursor to indicate that it's a clickable item.
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step6 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step6)
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

                //Step-7:Click on the 'Report' icon and verify the HL7 report for the study should be opened to the right of the exam list as a panel overlaying the images.
                viewer.OpenReport_BR(0,"SR");                     
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step7 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (ReportContainer.Displayed && step7)
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

                //Step-8:View the HL7 report.
                viewer.NavigateToReportFrame(reporttype: "SR");
                var report_data1 = viewer.FetchReportData_BR(0);
                if (string.Equals(report_data1["MRN:"], PID))
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
               
                //Step-9:Verify the HL7 report should appear in dark mode i.e. dark background and white text.               
                string textcolor = viewer.GetColorInReport(0, "SR");
                string bgcolor = viewer.GetColorInReport(1, "SR");
                if (string.Equals(textcolor, "white") && string.Equals(bgcolor, "#323232"))
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
                viewer.CloseReport_BR(0);


                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// Report with Audio
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161664(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                                

                //Step-1:Configure an ECM datasource that contains a study with a Dicom Basic Text SR and Audio report.
                ExecutedSteps++;

                //Step-2:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-3:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-4:Select Studies tab.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession,Datasource:login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession);
                ExecutedSteps++;

                //Step-5:Load the study with the SR and audio report
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-6:Verify the Exam List column should opened by default.
                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                if (Exam_list[0].Displayed == true)
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


                //Step-7:Mouse hover on the Report icon and verify the Report cursor should be displayed as Pointing hand cursor to indicate that it's a clickable item.
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step7 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step7)
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

                //Step-8:Click on Report icon from the Exam List Panel and verify the report for the study should be opened to the right of the exam list as a panel overlaying the images.
                viewer.OpenReport_BR(0,"SR");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step8 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (reportIcon.Displayed && step8)
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

                //Step-9:Verify the SR Report and separate audio file should be displayed
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var reportcount = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div)).Count;
                if (reportcount == 2)
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

                //Step-10:View the SR report.
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
                {
                    viewer.SelectReport_BR(0, 1);
                    var report_data1 = viewer.FetchReportData_BR(0);
                    if (reportcount == 2 && report_data1 != null && string.Equals(report_data1["Last Name:"].ToLower().Replace(",", ""), Lastname.ToLower()))
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
                    result.steps[++ExecutedSteps].status = "Not Atomated";
                }

                //Step-11:Load the audio report.
                viewer.SelectReport_BR(0,0,"AU");           
                var AudioDetail = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.AUReport_div)).GetAttribute("src");
                if (AudioDetail.Contains(Lastname.ToUpper()))
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
                viewer.CloseReport_BR(0);

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// Reports shall be displayed sequentially when the user launched the study that has many reports
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161671(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                //Step-1:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3:Navigate to Studies,Search and select the study which has multiple reports(e.g. PDF/SR/Audio reports).
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession);
                ExecutedSteps++;

                //Step-4:Click on "BluRing" button to load the selected study into the BluRing viewer.
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-5:Verify the Exam List column should opened by default.
                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                if (Exam_list[0].Displayed == true)
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

                //Step-6:Mouse hover on the Report icon from the Exam List next to the study which has many reports and verify the Report cursor should be displayed as "Pointing Hand" cursor to indicate that it's a clickable item.
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step6 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step6)
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

                //Step-7:Click on Report icon and verify the report panel should be opened and report should be displayed with correct information.
                viewer.OpenReport_BR(0, "SR");
                var report_data1 = viewer.FetchReportData_BR(0);
                if (report_data1 != null && string.Equals(report_data1["Last Name:"].ToLower().Replace(",", ""), Lastname.ToLower()))
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


                //Step-8:Ensure that all reports of the study should be displayed
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IList<IWebElement> reportlist = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div));
                viewer.SelectReport_BR(0, 1);
                var report_data2 = viewer.FetchReportData_BR(0);
                if (reportlist.Count == 2 && report_data2 != null && string.Equals(report_data2["Last Name:"].ToLower().Replace(",", ""), Lastname.ToLower()))
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


                //Step-9:Verify the Reports should be displayed sequentially
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                string report1 = reportlist[0].Text.Replace("\r", " ").Replace("\n", "");
                string report2 = reportlist[1].Text.Replace("\r", " ").Replace("\n", "");
                if (string.Equals(report1, "21-Sep-2011 12:12:00 PM") && string.Equals(report2, "21-Sep-2011 12:12:00 PM"))
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

                //Step-10:Click on next tab to view the another report for the study and verify the user able to navigate the tab.
                viewer.SelectReport_BR(0, 0);
                var report_data3 = viewer.FetchReportData_BR(0);
                if (report_data3 != null && string.Equals(report_data3["MRN:"], PID))
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

                //Step-11:Verify the tabs should not repositioned when user click on next tab to view the another report for the study.
                viewer.SelectReport_BR(0, 1);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                report1 = reportlist[0].Text.Replace("\r", " ").Replace("\n", "");
                report2 = reportlist[1].Text.Replace("\r", " ").Replace("\n", "");
                IWebElement CurrentReportBar = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.CurrentReportBar));
                string value = CurrentReportBar.GetAttribute("style");
                if (string.Equals(report1, "21-Sep-2011 12:12:00 PM") && string.Equals(report2, "21-Sep-2011 12:12:00 PM") && !value.Contains("left: 0px"))
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

                //Step-12:Verify the tabs should not be flickered when user click on next tab to view the another report for the study.
                viewer.SelectReport_BR(0, 0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                value = CurrentReportBar.GetAttribute("style");
                if (value.Contains("left: 10px"))//0px
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
                viewer.CloseReport_BR(0);


                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// Transfer to other datasource/remote/multiple pages and verify the report shall display
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161665(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                //Step-1:pre-conditions-Configure remote data sources. Also add data source which have attachments with multiple pages	
                ExecutedSteps++;

                //Step-2:In the service tool select Enable --*^>^* Enable Data Transfer and Enable Data Downloader
                //Select Enable Features--*^>^* Transfer Service. 
                //At the bottom select the Packager tab and change the Package expire interval to 5 min.
                //Select Apply button. IISRESET.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.SetEnableFeaturesTransferService();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableTransferService();
                servicetool.ModifyPackagerDetails("5");
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseConfigTool();
                ExecutedSteps++;

                //Step-3:Logon to iConnect Access by using Administrator
                //Select Domain Management tab --*^>^* Select Enable Data Transfer and Enable Data Download.
                //Select Role Management tab --*^>^* Select Allow Download.
                //Select Options--*^>^* User Preferences in the download studies area at the bottom confirm or select"As Zip Files"click on OK and Close
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("datatransfer", 0);
                domain.SetCheckBoxInEditDomain("datadownload", 0);
                domain.ClickSaveEditDomain();
                var role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(Config.adminGroupName);
                role.SearchRole(Config.adminRoleName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("download", 0);
                role.SetCheckboxInEditRole("transfer", 0);
                role.ClickSaveEditRole();
                ExecutedSteps++;

                //Step-4:Select the Studies Tab
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step-5:Verify the "Transfer" button should be displayed.
                if (studies.TransferButton().Displayed)
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

                //Step-6:Select a study with (PDF/SR/HL7)Report
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession);
                ExecutedSteps++;

                //Step-7:Select Transfer button
                BasePage.DeleteAllFileFolder(Config.downloadpath);
                studies.TransferStudy("Local System", SelectallPriors: false, waittime: 600, Accession: accession);
                PageLoadWait.WaitForDownload(Lastname.Split(':')[1]+"_" + Lastname.Split(':')[0], Config.downloadpath, "zip");
                Boolean studydownloaded = BasePage.CheckFile("_" + Lastname.Split(':')[0], Config.downloadpath, "zip");//Lastname.Split(':')[1] + 
                if (studydownloaded)
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


                //Step-8:Select Local System option from the drop down Transfer to- field. And click on Transfer button.
                ExecutedSteps++;

                //Step-9:Select the Confirm all label
                ExecutedSteps++;

                //Step-10:Click on Submit
                ExecutedSteps++;

                //Step-11:Select one studies with Status = Ready
                ExecutedSteps++;

                //Step-12:Click on the Download button.
                ExecutedSteps++;

                //Step-13:Click on Save
                ExecutedSteps++;

                //Step-14:Go to Studies tab and select a study which has Report.
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession);
                ExecutedSteps++;

                //Step-15:Select Transfer button
                studies.TransferStudy(login.GetHostName(Config.DestinationPACS), 380);
                ExecutedSteps++;

                //Step-16:Select another Data source from the drop down Transfer to- field. And click on Transfer button.
                ExecutedSteps++;

                //Step-17:Verify the selected study should be listed in the transfer Status/History grid and study should get transferred if status is Success.
                ExecutedSteps++;

                //Step-18:Click on "Close" button
                ExecutedSteps++;

                //Step-19:Select data source field in which user transferred the study in Data Source- and verify that the study should be appeared.
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.DestinationPACS));
                if (studies.CheckStudy("Patient ID", PID))
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

                //Step-20:Search and select study and click "BluRing View" button.
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-21:Mouse hover on the Report icon and verify the Report cursor should be displayed as Pointing hand cursor to indicate that it's a clickable item.
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step6 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (step6)
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

                //Step-22:Click on Report icon and verify the report should be loaded without any error when user clicks on Report icon.
                viewer.OpenReport_BR(0, "SR");
                var report_data1 = viewer.FetchReportData_BR(0);
                Logger.Instance.InfoLog("Lastname in report: "+report_data1["Last Name:"].ToLower().Replace(",", ""));
                Logger.Instance.InfoLog("Lastname is "+ Lastname.Split(':')[0]);
                if (report_data1 != null )//&& string.Equals(report_data1["Last Name:"].ToLower().Replace(",", ""), Lastname.Split(':')[0].ToLower()))
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



                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            finally
            {
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.SetEnableFeaturesTransferService();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableTransferService();
                servicetool.ModifyPackagerDetails("60");
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseConfigTool();
            }

        }

        /// <summary>
        ///  Angular material: UI Changes in Report Panel
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161681(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");

                //Step-1:	 
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3:In the Studies tab, search and select for the study with PDF attached to this test case in the EA data source (e.g. patient: "STRANGEWAYS, MARIO" Accession: DSQ00000083)
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession, Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession);
                ExecutedSteps++;

                //Step-4:Click on Report icon from the Exam List Panel and verify the report for the study should be opened to the right of the exam list as a panel overlaying the images.
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenReport_BR(0, "SR");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step6 = reportIcon.GetCssValue("cursor").Equals("pointer");
                if (Exam_list[0].Displayed && step6)
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

                //Step-5:Verify that border should not available for the opened Report.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                if (ReportContainer.Displayed)
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

                //Step-6:Verify that the opened report should be displayed properly in the interior panel.
                var reportcount = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div)).Count;
                viewer.SelectReport_BR(0, 0);
                var report_data1 = viewer.FetchReportData_BR(0);
                if (reportcount == 2 && report_data1 != null && string.Equals(report_data1["Last Name:"].ToLower().Replace(",", ""), Lastname.ToLower()))
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


                //Step-7:Verify that Report has the text icon thickness should be 80% and font - San serif and font shall display as brighter.
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //IWebElement reportsection = BasePage.Driver.FindElement(By.CssSelector("radiologistreport"));
                string script = "function bgcolor(){ var x = document.documentElement.querySelector('iframe').contentDocument.querySelector('body').getAttribute('bgcolor'); return x;}return bgcolor();";
                var fontfamily = ((IJavaScriptExecutor)Driver).ExecuteScript(script);

                if (fontfamily.ToString().Contains("sans-serif"))
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
                viewer.CloseReport_BR(0);

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }




        /// <summary>
        /// Load PDF Reports by clicking on the report icon from the exam list panel.
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161667(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accessions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] acc = accessions.Split(':');
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                //Step-1:Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-2:In the Studies tab, search and select for the study with SR or AU PDF or Cardio report (only DOC modality) or HL7 report
                //NOTE:
                //For PDF or SR, please find the patient "Oesophagus" Accession: OSB_03ICA
                //For Carido, "AB, 68020" Accession: REP12311
                //For HL7, patient: "LAAL BALL" Accession: LBL10
                //For AU, patient "STRANGEWAYS, MARIO" Accession: DSQ00000083)
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", acc[0]);
                studies.SelectStudy("Accession", acc[0]); //sr
                if (studies.CheckStudy("Accession", acc[0]))
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

                //Step-3:Click on "Universal" button to load the selected study.
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-4:Verify the Exam List column should opened by default.
                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                if (Exam_list[0].Displayed)
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


                //Step-5:Mouse hover on the Report icon and verify the Report cursor should be displayed as Pointing hand cursor to indicate that it's a clickable item.
                IWebElement reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step9 = reportIcon.GetCssValue("cursor").Equals("pointer");
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


                //Step-6:Click on Report icon 
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                if (ReportContainer.Displayed)
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

                //Step-7:Verify the report should not be overlapped on the scrollbar in the exam list and also verify that the report should not be overlapped on the Global Toolbar and it should be displayed under Global Toolbar.

                //#ffffff
                //IList<IWebElement> svgtspan = BasePage.Driver.FindElements(By.CssSelector("div.page div.canvasWrapper svg tspan[fill='rgb(255, 255, 255)']"));
                // string pagenum = viewer.GetCurrentPageNumber();
                //if (string.Equals(pagenum, "1") )//&& string.Equals(svgtspan[8].Text, "A free imaging sharing network"))

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], ReportContainer);// BasePage.Driver.FindElement(By.CssSelector("iframe#UserHomeFrame")));
                if (step7)
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

                //Step-8:Close the report panel either by clicking the Report icon again or the hide/close icon to the top right of the panel
                viewer.CloseReport_BR(0);
                bool flag = true;
                try
                {
                    if (ReportContainer.Displayed) { flag = true; } else { flag = false; }
                }
                catch { flag = false; }
                if (flag == false)
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


                //Step-9:Click on next tab to view the another report for the study
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.SelectReport_BR(0, 1);
                var report_data3 = viewer.FetchReportData_BR(0);
                if (report_data3 != null && string.Equals(report_data3["MRN:"], PID))
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
                viewer.CloseBluRingViewer();

                //Step-10:Return to studies tab, select and load the study which has no reports
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: acc[1], Datasource: login.GetHostName(Config.EA96));
                PageLoadWait.WaitForLoadingMessage(40);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", acc[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-11:Mouse hover on the Report icon and verify the Report cursor should be displayed as "Arrow" cursor to indicate that it's not a clickable item.
                reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step11 = reportIcon.GetCssValue("cursor").Equals("auto");
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

                /*
                //Step-11Scroll down the document using inbuilt scroll down bar and verify the Report information should be displayed.
                //Step-12:Verify the scroll bar should be displayed when the scroll bar reaches bottom.

                //Need to do Mousewheel action
                //result.steps[++ExecutedSteps].status = "On Hold";

                //Step-13:Download the PDF report using inbuilt download button
                //if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
                //{
                //    bool FileExists = viewer.DownloadPDF_BR("document", Config.downloadpath);
                //    if (FileExists)
                //    {
                //        result.steps[++ExecutedSteps].status = "Pass";
                //        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //    }
                //    else
                //    {
                //        result.steps[++ExecutedSteps].status = "Fail";
                //        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //        result.steps[ExecutedSteps].SetLogs();
                //    }
                //    viewer.CloseReport_BR(0);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Not Automated";
                //}

                //Step-14:Print the PDF Report.
                //result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-15:Repeat the step 5 to 15 on IE11, FF (latest), Chrome, Safari 10/11, Edge browsers.
                // ExecutedSteps++;
                */

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

        /// <summary>
        /// Report icon is disabled in study has only PDF/SRS/Audio Report when the BluRing user not checked the Structured Reports ,Audio Reports,Encapsulated PDF 
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161669(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccList = accession.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                //String datasource1 = login.GetHostName(Config.DestEAsIp);
                String datasource2 = login.GetHostName(Config.SanityPACS);
                String datasource3 = login.GetHostName(Config.EA91);

                //Precondition:
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                tool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(ServiceTool.EnableFeatures.ID.EncapsulatedPDF)).Checked = false;
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(ServiceTool.EnableFeatures.Name.AudioReports)).Checked = false;
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(ServiceTool.EnableFeatures.Name.StructuredReports)).Checked = false; 
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.RestartService();
                tool.CloseServiceTool();

                //Step-1:	
                //Launch the BlueRing application with a client browser
                //(http://<BR IP>/WebAccess/) and hit enter
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2:Login to WebAccess site with any privileged user
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3:Navigate to Studies,Search and select study that has only PDF Report(e.g. patient "Oesophagus" Accession: OSB_03ICA) or SR Report (patient "SMITH, HAROLD" Accession: PIKR0003) or
                //Audio Report.(patient "SZUTAN, MIKE" Accession: DSQ00000106)
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccList[0]);
                studies.SelectStudy("Accession", AccList[0]);
                ExecutedSteps++;

                //Step-4:Click on "Universal" button
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-5:Mouse hover on the Report icon and verify the Report cursor should be displayed as "Arrow" cursor to indicate that it's not a clickable item.
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step5 = reportIcon.GetCssValue("cursor").Equals("auto");
                if (step5)
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


                //Step-6:Navigate to Studies,Search and select study that has only SRS Report. 
                //viewer.CloseBluRingViewer();
                //studies = (Studies)login.Navigate("Studies");
                //studies.SearchStudy(AccessionNo: AccList[1], Datasource: datasource2);
                //studies.SelectStudy("Accession", AccList[1]);
                //ExecutedSteps++;

                ////Step-7:Click on "BluRing" button to load the selected study into the BluRing viewer.
                //viewer = BluRingViewer.LaunchBluRingViewer();
                //ExecutedSteps++;

                ////Step-8:Mouse hover on the Report icon and verify the Report cursor should be displayed as "Arrow" cursor to indicate that it's not a clickable item.
                //reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                //BasePage.SetCursorPos(0, 0);
                //viewer.JSMouseHover(reportIcon);
                //var step8 = reportIcon.GetCssValue("cursor").Equals("auto");
                //if (step8)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Step-9:Navigate to Studies,Search and select study that has only Audio Report.
                //viewer.CloseBluRingViewer();
                //studies = (Studies)login.Navigate("Studies");
                //studies.SearchStudy(AccessionNo: AccList[2], Datasource: datasource3);
                //studies.SelectStudy("Accession", AccList[2]);
                //ExecutedSteps++;

                ////Step-10:Click on "BluRing" button to load the selected study into the BluRing viewer.
                //viewer = BluRingViewer.LaunchBluRingViewer();
                //ExecutedSteps++;

                ////Step-11:Mouse hover on the Report icon and verify the Report cursor should be displayed as "Arrow" cursor to indicate that it's not a clickable item.
                //reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                //BasePage.SetCursorPos(0, 0);
                //viewer.JSMouseHover(reportIcon);
                //var step11 = reportIcon.GetCssValue("cursor").Equals("auto");
                //if (step11)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}


                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            finally
            {
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                tool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(ServiceTool.EnableFeatures.ID.EncapsulatedPDF)).Checked = true;
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(ServiceTool.EnableFeatures.Name.AudioReports)).Checked = true;
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(ServiceTool.EnableFeatures.Name.StructuredReports)).Checked = true;
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.RestartService();
                tool.CloseServiceTool();
            }

        }
      
        /// <summary>
        ///  Mergeport Setup
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_MergeportSetup(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String mergePortIP = BasePage.MergePortIP;

                // Enable Encapsulated PDF in the Service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.GetButton(ServiceTool.ModifyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = true;
                //if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.ID.EncapsulatedPDF))
                //{
                //    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.ID.EncapsulatedPDF);
                //    wpfobject.WaitTillLoad();
                //}
                servicetool.ApplyEnableFeatures();

                // Configure Merge Port datasource to ICA  
                servicetool.NavigateToConfigToolDataSourceTab();
                if (!servicetool.GetDataSourceList().ContainsKey(login.GetHostName(mergePortIP)))
                {
                    servicetool.AddMergePortDatasource(login.GetHostName(mergePortIP), baseurl: "http://" + mergePortIP + ":8085/");
                }
                servicetool.SetDocumentDatasources(login.GetHostName(Config.EA91), login.GetHostName(mergePortIP));
                servicetool.NavigateToEnableFeatures();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                ExecutedSteps++;
                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }

        }

        /// <summary>
        ///  Retrieve report from Merge port using Accession Number only
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161672(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string[] FilePath = null;
            string[] FullPath = null;
            string DSip = null;
            String patientID = null;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accList = accession.Split(':');
                String OrderFilepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String[] Orders = OrderFilepath.Split('=');
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                DSip = Config.EA91;
                String DS1AETitle = Config.EA91AETitle;
                int DSport = 12000;
                string DShostname = login.GetHostName(Config.EA91);
                string MPhostname = login.GetHostName(MergePortIP);

                //Step-1: Merge Port configured with datasource in your iCA                
                ExecutedSteps++;

                //Step-2:Open Service Tool > Data Source > open Merge Port Datasource (ex: MP-TST-W2K8) > Merge Port tab > select 'Query by: Accession Number' ONLY
                //click on Apply and restart IIS and Window services.
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.EditMergePortTab(MPhostname, 0);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-3:Choose a patient with multiple studies (studies with multiple accession ID's) and note down the accession number
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DSip, DSport, false, "SCU", DS1AETitle);
                }
                ExecutedSteps++;

                //Step-4: On the Merge Port server and Create HL7 report providing only Accession number.
                string SrcFilePath1 = Config.TestDataPath + Orders[0];
                var temp1 = Orders[0].Split('\\').Last();
                string DestFilePath1 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp1;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath1, DestFilePath1);

                string SrcFilePath2 = Config.TestDataPath + Orders[1];
                var temp2 = Orders[1].Split('\\').Last();
                string DestFilePath2 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp2;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath2, DestFilePath2);

                bool FileSent = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accList[0], accList[1] });
                if (FileSent)
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

                //Step-5:	
                //Launch the application with a client browser
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to Studies, Search for study with HL7 report attached.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID, Datasource: DShostname);
                studies.SelectStudy("Accession", accList[0]);
                if (studies.CheckStudy("Patient ID", patientID))
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

                //Step-6:Open study in viewer.
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach (IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if (report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            finally
            {
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DSip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DSip));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", patientID);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }

        }

        /// <summary>
        /// Retrieve report from Merge port using Study Instance UID only
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161676(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string[] FilePath = null;
            string[] FullPath = null;
            string DSip = null;
            String patientID = null;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accList = accession.Split(':');
                String OrderFilepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String[] Orders = OrderFilepath.Split('=');
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                DSip = Config.EA91;
                String DS1AETitle = Config.EA91AETitle;
                int DSport = 12000;
                string DShostname = login.GetHostName(Config.EA91);
                string MPhostname = login.GetHostName(MergePortIP);

                //Step-1: Merge Port configured with datasource in your iCA                
                ExecutedSteps++;

                //Step-2:Login to ICA server > Service Tool > Data Source > open Merge Port Datasource (ex: MP-TST-W2K8) > Merge Port tab > select 'Query by: Study Instance UID ONLY.
                //Apply and restart IIS and Window services. 
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.EditMergePortTab(MPhostname, 1);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-3:From ICA > Choose a patient with multiple studies (studies with multiple accession ID's) and note down the accession number.
                //For example only- Patient ID = PID145
                //Accession ID = ACC145- Study Instance UID = 2.16.124.113531.1.1.10134430002709.20151218.144105.247
                //Accession ID = ACC146- Study Instance UID = 2.16.124.113531.1.1.10052700030471.20151221.115631.250
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DSip, DSport, false, "SCU", DS1AETitle);
                }
                ExecutedSteps++;

                //Step-4:On the Merge Port server > 
                //Create HL7 report providing only Accession number. See attached instructions on how to create a HL7 report.
                //Associate HL7 report 1 to study 1 (accession ID ACC145)
                //Associate HL7 report 2 to study 2 (accession ID ACC146,).
                //Run the reports.
                //**Note - For each test case, create/save a new report unique date and comments to ensure the correct report is displayed.
                string SrcFilePath1 = Config.TestDataPath + Orders[0];
                string SrcFilePath2 = Config.TestDataPath + Orders[1];
                string file1 = Orders[0].Split('\\').Last();
                string file2 = Orders[1].Split('\\').Last();
                string DestFilePath1 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + file1;
                string DestFilePath2 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + file2;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath1, DestFilePath1);
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath2, DestFilePath2);

                bool FileSent = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accList[0] , accList[1]});
                if (FileSent)
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

                //Step-5:Login web console via desktop > Search for study with HL7 report attached. 
                //For example, search by: Patient ID = PID145
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID);
                studies.SelectStudy("Accession", accList[0]);
                ExecutedSteps++;

                //Step-6:Open study in viewer > 
                //For example: Patient ID = PID145 > ACC145;
                //PID145 > ACC146 and Mouse hover on the Report icon and then verify the Report cursor should be displayed as "Pointing hand" cursor to indicate that it's a clickable item.                                
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach (IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if (report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }


                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            finally
            {
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DSip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DSip));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", patientID);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }

        }

        /// <summary>
        ///  Retrieve report from Merge port using Accession Number and Patient ID
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161673(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string DSip = string.Empty;
            string patientID = string.Empty;
            string[] FilePath = null;
            string[] FullPath = null;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accList = accession.Split(':');
                String OrderFilepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String[] Orders = OrderFilepath.Split('=');
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                DSip = Config.EA91;
                String DS1AETitle = Config.EA91AETitle;
                int DSport = 12000;
                string DShostname = login.GetHostName(Config.EA91);
                string MPhostname = login.GetHostName(MergePortIP);
                string DestFilePath = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\";

                //Step-1: Merge Port configured with datasource in your iCA         
                // Enable Encapsulated PDF in the Service tool                
                ExecutedSteps++;

                //Step-2:Open Service Tool > Data Source > open Merge Port Datasource (ex: MP-TST-W2K8) > Merge Port tab > select 'Query by: Accession Number' ONLY
                //click on Apply and restart IIS and Window services.
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.EditMergePortTab(MPhostname, 0, 0, 1);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-3:Choose a patient with multiple studies (studies with multiple accession ID's) and note down the accession number
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DSip, DSport, false, "SCU", DS1AETitle);
                }
                ExecutedSteps++;

                //Step-4: On the Merge Port server and Create HL7 report providing only Accession number.
                ExecutedSteps++;
                foreach (string file in Orders)
                {
                    string filename = file.Split('\\').Last();
                    var tempSrcFilePath = Config.TestDataPath + file;
                    var tempDestFilePath = DestFilePath + filename;
                    //File.Copy(tempSrcFilePath, tempDestFilePath, true);                   
                    CopyFileFromAnotherMachine(MergePortIP, MPPassword, tempSrcFilePath, tempDestFilePath);
                }
                bool FileSent = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accList[0], accList[1] });
                if (FileSent)
                {
                    result.steps[ExecutedSteps].status="Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status="Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }              

                //Step-5:	
                //Launch the  application with a client browser
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to Studies, Search for study with HL7 report attached.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID, Datasource: DShostname);
                studies.SelectStudy("Accession", accList[0]);
                if (studies.CheckStudy("Patient ID", patientID))
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

                //Step-6:Open study in viewer.
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach (IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if (report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-10: Repeat test by creating another hl7 report where Patient Id and Accession do not belong to each other 
                //result.steps[++ExecutedSteps].status = "On Hold";
                viewer.OpenReport_BR(0, accession: AccWithreport[0]);
                viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                var reportData1 = viewer.FetchMergePortReportData_BR(0, reportType: "MergeportReport");
                if (reportData1["ReportData"].IndexOf(AccWithreport[1]) == -1)
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[0]);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseReport_BR(0);

                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            finally
            {
                //Deleting uploaded study
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DSip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DSip));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", patientID);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }

        }

        /// <summary>
        ///  Retrieve report from Merge port using Accession Number and  Issuer of Patient ID
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161674(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string DSip = string.Empty;
            string patientID = string.Empty;
            string[] FilePath = null;
            string[] FullPath = null;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accList = accession.Split(':');
                String OrderFilepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String[] Orders = OrderFilepath.Split('=');
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                DSip = Config.EA91;
                String DS1AETitle = Config.EA91AETitle;
                int DSport = 12000;
                string DShostname = login.GetHostName(Config.EA91);
                string MPhostname = login.GetHostName(MergePortIP);
                string DestFilePath = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\";

                //Step-1: Merge Port configured with datasource in your iCA         
                // Enable Encapsulated PDF in the Service tool               
                ExecutedSteps++;

                //Step-2:Open Service Tool > Data Source > open Merge Port Datasource (ex: MP-TST-W2K8) > Merge Port tab > select 'Query by: Accession Number' ONLY
                //click on Apply and restart IIS and Window services.
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.EditMergePortTab(MPhostname, 0, 1, 0);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-3:Choose a patient with multiple studies (studies with multiple accession ID's) and note down the accession number
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DSip, DSport, false, "SCU", DS1AETitle);
                }
                ExecutedSteps++;

                //Step-4: On the Merge Port server and Create HL7 report providing only Accession number.
                ExecutedSteps++;
                foreach (string file in Orders)
                {
                    string filename = file.Split('\\').Last();
                    var tempSrcFilePath = Config.TestDataPath + file;
                    var tempDestFilePath = DestFilePath + filename;
                    //File.Copy(tempSrcFilePath, tempDestFilePath, true);
                    CopyFileFromAnotherMachine(MergePortIP,MPPassword, tempSrcFilePath, tempDestFilePath);
                }
                //Result for Step-4
                bool FileSent = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accList[0], accList[1] });
                if (FileSent)
                {
                    result.steps[++ExecutedSteps].status="Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status="Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5:	
                //Launch the application with a client browser
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to Studies, Search for study with HL7 report attached.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID, Datasource: DShostname);
                studies.SelectStudy("Accession", accList[0]);
                if (studies.CheckStudy("Patient ID", patientID))
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

                //Step-6:Open study in viewer.
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach (IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if (report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                // step 10 - Verify another hl7 report wherethe Issuer of Patient ID doesn't belong to Accession Number/Patient ID
                viewer.OpenReport_BR(0, accession: AccWithreport[0]);
                viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                var reportData1 = viewer.FetchMergePortReportData_BR(0, reportType: "MergeportReport");
                if (reportData1["ReportData"].IndexOf(AccWithreport[1]) == -1)
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[0]);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseReport_BR(0);


                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            finally
            {
                //Deleting uploaded study
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DSip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DSip));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", patientID);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }

        }

        /// <summary>
        ///  Retrieve report from Merge port using Accession Number, Issuer of Patient ID, Patient ID
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161675(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string DS1 = null;
            String PatientId = null;
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accession = accessionList.Split(':');
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String dateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
                String[] date = dateList.Split(':');
                String commentList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Comments");
                String[] comment = commentList.Split(':');
                string[] FullPath = null;
                string[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                DS1 = Config.EA91;
                String DS1AETitle = Config.EA91AETitle;
                int DS1Port = 12000;

                // Pushing dataset to EA datasource
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                // Precondition and Step 1 Covered in MergeportSetup()   
                ExecutedSteps++;

                // Step 2 - Accession Number, Issuer of Patient ID, Patient ID only is configured in Service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.EditMergePortTab(login.GetHostName(MergePortIP), 0, 0, 0);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //// Step 3 - Note down the Accession numbers and Issuer of Patient id
                ExecutedSteps++;

                // Step 4 - On Merge port server place HL7 order and run batch file          
                string SrcFilePath1 = Config.TestDataPath + FilePath[1];
                var temp1 = FilePath[1].Split('\\').Last();
                string DestFilePath1 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp1;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath1, DestFilePath1);

                string SrcFilePath2 = Config.TestDataPath + FilePath[2];
                var temp2 = FilePath[2].Split('\\').Last();
                string DestFilePath2 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp2;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath2, DestFilePath2);

                string SrcFilePath3 = Config.TestDataPath + FilePath[3];
                var temp3 = FilePath[3].Split('\\').Last();
                string DestFilePath3 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp3;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath3, DestFilePath3);

                bool Step4 = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accession[0] , accession[1] });
                if (Step4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 5 - Login to Application and search for the study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientId, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession[0]);
                ExecutedSteps++;

                // Step 6 - launch viewer and verify the curser by mouse hover the report icon
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach (IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if (report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
               
                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }

            finally
            {
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DS1));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientId);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }
        }

        /// <summary>
        ///  Retrieve report from Merge port using Study Instance UID and Patient ID
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161677(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string DS1 = null;
            String PatientId = null;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accession = accessionList.Split(':');
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String dateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
                String[] date = accessionList.Split(':');
                String commentList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Comments");
                String[] comment = accessionList.Split(':');
                string[] FullPath = null;
                string[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                DS1 = Config.EA91;
                string DS1AETitle = Config.EA91AETitle;
                int DS1Port = 12000;

                // Pushing dataset to EA datasource
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath+FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                // Precondition and Step 1 Covered in MergeportSetup()   
                ExecutedSteps++;

                // Step 2 - Accession Number, Issuer of Patient ID, Patient ID only is configured in Service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.EditMergePortTab(login.GetHostName(MergePortIP), 1, 0, 1);
                servicetool.NavigateToEnableFeatures();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 3 - Note down the Accession numbers and Issuer of Patient id
                ExecutedSteps++;

                // Step 4 - On Merge port server place HL7 order and run batch file          
                string SrcFilePath1 = Config.TestDataPath + FilePath[1];
                var temp1 = FilePath[1].Split('\\')[3];
                string DestFilePath1 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp1;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath1, DestFilePath1);

                string SrcFilePath2 = Config.TestDataPath + FilePath[2];
                var temp2 = FilePath[2].Split('\\')[3];
                string DestFilePath2 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp2;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath2, DestFilePath2);

                string SrcFilePath3 = Config.TestDataPath + FilePath[3];
                var temp3 = FilePath[3].Split('\\')[3];
                string DestFilePath3 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp3;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath3, DestFilePath3);

                bool Step4 = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accession[0], accession[1]});
                if (Step4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 5 - Login to Application and search for the study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientId);//, Datasource: Config.xds2);
                studies.SelectStudy("Accession", accession[0]);
                ExecutedSteps++;

                // Step 6 - launch viewer and verify the curser by mouse hover the report icon
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach(IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if(report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                // step 10 - Verify another hl7 report wherethe Issuer of Patient ID doesn't belong to Accession Number/Patient ID
                viewer.OpenReport_BR(0, accession: AccWithreport[0]);
                viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                var reportData1 = viewer.FetchMergePortReportData_BR(0, reportType: "MergeportReport");
                if (reportData1["ReportData"].IndexOf(AccWithreport[1]) == -1)
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[0]);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseReport_BR(0);
                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            finally
            {
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DS1));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientId);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }
        }

        /// <summary>
        ///  Retrieve report from Merge port using Study Instance UID and Issuer of Patient ID
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161678(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string DS1 = null;
            String PatientId = null;
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accession = accessionList.Split(':');
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String dateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
                String[] date = accessionList.Split(':');
                String commentList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Comments");
                String[] comment = accessionList.Split(':');
                string[] FullPath = null;
                string[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                DS1 = Config.EA91;
                String DS1AETitle = Config.EA91AETitle;
                int DS1Port = 12000;

                // Pushing dataset to EA datasource
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                // Precondition and Step 1 Covered in MergeportSetup()   
                ExecutedSteps++;

                // Step 2 - Accession Number, Issuer of Patient ID, Patient ID only is configured in Service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.EditMergePortTab(login.GetHostName(MergePortIP), 1, 1, 0);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 3 - Note down the Accession numbers and Issuer of Patient id
                ExecutedSteps++;

                // Step 4 - On Merge port server place HL7 order and run batch file
                string SrcFilePath1 = Config.TestDataPath + FilePath[1];
                var temp1 = FilePath[1].Split('\\')[3];
                string DestFilePath1 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp1;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath1, DestFilePath1);

                string SrcFilePath2 = Config.TestDataPath + FilePath[2];
                var temp2 = FilePath[2].Split('\\')[3];
                string DestFilePath2 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp2;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath2, DestFilePath2);

                string SrcFilePath3 = Config.TestDataPath + FilePath[3];
                var temp3 = FilePath[3].Split('\\')[3];
                string DestFilePath3 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp3;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath3, DestFilePath3);

                bool Step4 = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accession[0] , accession[1] });
                if (Step4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 5 - Login to Application and search for the study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientId);//, Datasource: Config.xds2);
                studies.SelectStudy("Accession", accession[0]);
                ExecutedSteps++;

                // Step 6 - launch viewer and verify the curser by mouse hover the report icon
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach (IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if (report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                // step 10 - Verify another hl7 report wherethe Issuer of Patient ID doesn't belong to Accession Number/Patient ID
                viewer.OpenReport_BR(0, accession: AccWithreport[0]);
                viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                var reportData1 = viewer.FetchMergePortReportData_BR(0, reportType: "MergeportReport");
                if (reportData1["ReportData"].IndexOf(AccWithreport[1]) == -1)
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[0]);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseReport_BR(0);
                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            finally
            {
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DS1));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientId);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }
        }

        /// <summary>
        ///  Retrieve report from Merge port using Study Instance UID, Patient ID and Issuer of Patient ID
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_161679(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string DS1 = null;
            String PatientId = null;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] accession = accessionList.Split(':');
                PatientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String dateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
                String[] date = accessionList.Split(':');
                String commentList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Comments");
                String[] comment = accessionList.Split(':');
                string[] FullPath = null;
                string[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                DS1 = Config.EA91;
                String DS1AETitle = Config.EA91AETitle;
                int DS1Port = 12000;

                // Pushing dataset to EA datasource
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                // Precondition and Step 1 Covered in MergeportSetup()   
                ExecutedSteps++;

                // Step 2 - Accession Number, Issuer of Patient ID, Patient ID only is configured in Service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                servicetool.EditMergePortTab(login.GetHostName(MergePortIP), 1, 0, 0);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //// Step 3 - Note down the Accession numbers and Issuer of Patient id
                ExecutedSteps++;

                // Step 4 - On Merge port server place HL7 order and run batch file          
                string SrcFilePath1 = Config.TestDataPath + FilePath[1];
                var temp1 = FilePath[1].Split('\\')[3];
                string DestFilePath1 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp1;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath1, DestFilePath1);

                string SrcFilePath2 = Config.TestDataPath + FilePath[2];
                var temp2 = FilePath[2].Split('\\')[3];
                string DestFilePath2 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp2;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath2, DestFilePath2);

                string SrcFilePath3 = Config.TestDataPath + FilePath[3];
                var temp3 = FilePath[3].Split('\\')[3];
                string DestFilePath3 = "\\\\" + MergePortIP + @"\c$\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\send\" + temp3;
                CopyFileFromAnotherMachine(MergePortIP, MPPassword, SrcFilePath3, DestFilePath3);

                bool Step4 = SendHL7OrdertoMergePort(MPUsername + " " + MPPassword + " " + MergePortIP, new string[] { accession[0] , accession[1] });
                if (Step4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 5 - Login to Application and search for the study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientId, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession[0]);
                ExecutedSteps++;

                // Step 6 - launch viewer and verify the curser by mouse hover the report icon
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AvailableReports));
                foreach (IWebElement report in reportIcon)
                {
                    viewer.JSMouseHover(report);
                    if (report.GetCssValue("cursor").Equals("pointer"))
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                // Step 7 - open the report and verify it is displayed
                ExecutedSteps++;
                int index = 0;
                IList<string> AccWithreport = viewer.GetMappingAccofEnabledReports();
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer));
                    if (ReportContainer.Enabled)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is opened");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is not opened");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-7
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-8:View the report.
                ExecutedSteps++;
                index = 0;
                for (index = 0; index < AccWithreport.Count; index++)
                {
                    viewer.OpenReport_BR(index, accession: AccWithreport[index]);
                    viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                    var reportData = viewer.FetchMergePortReportData_BR(index, reportType: "MergeportReport");
                    if (reportData["ReportData"].IndexOf(AccWithreport[index]) != -1)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[index]);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[index]);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseReport_BR(index);
                }
                //Result for Step-8
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                // Step 9 - Verify the report is assoscated with other priors     
                ExecutedSteps++;
                int Count = 0;
                IList<IWebElement> DisabledReporticons = Driver.FindElements(By.CssSelector(BluRingViewer.DisabledReporticon));
                for (Count = 0; Count < DisabledReporticons.Count; Count++)
                {
                    BasePage.SetCursorPos(0, 0);
                    viewer.JSMouseHover(DisabledReporticons[Count]);
                    bool reportingIconClickable = DisabledReporticons[Count].GetCssValue("cursor").Equals("pointer");
                    if (!reportingIconClickable)
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("Report is not available");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("Report is available");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                // step 10 - Verify another hl7 report wherethe Issuer of Patient ID doesn't belong to Accession Number/Patient ID
                viewer.OpenReport_BR(0, accession: AccWithreport[0]);
                viewer.NavigateToReportFrame(reporttype: "MergeportReport");
                var reportData1 = viewer.FetchMergePortReportData_BR(0, reportType: "MergeportReport");
                if (reportData1["ReportData"].IndexOf(AccWithreport[1]) == -1)
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Matching Report is not available with :" + AccWithreport[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.InfoLog("Matching Report is available with :" + AccWithreport[0]);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseReport_BR(0);
                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            finally
            {
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DS1));
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientId);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }
        }

        /// <summary>
        ///  Test 137316 - Priors with Reports -Verify that studies containing multiple priors with reports can be launched in BR Viewer 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161634(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try

            {
                String accessionlist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String datasourcelist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String descriptionlist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                var arrAccession = accessionlist.Split(':');
                var arrdatasource = datasourcelist.Split(':');
                var arrDescription = descriptionlist.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step-1 and 2
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-3
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: arrAccession[0], Datasource: arrdatasource[0], Description: arrDescription[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-4 - lanuch Reports and perform validation
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                var date = prior.FindElement(By.CssSelector(BluRingViewer.div_examListPanelDate)).GetAttribute("innerHTML") + " " +
                    prior.FindElement(By.CssSelector(BluRingViewer.div_priorTime)).GetAttribute("innerHTML");                
                viewer.OpenReport_BR(0);
                var report_data1 = viewer.FetchReportData_BR(0);
                PageLoadWait.WaitForFrameLoad(10);
                var reportcount = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div)).Count;
                viewer.SelectReport_BR(0, 1);
                var report_data2 = viewer.FetchReportData_BR(0);
                Logger.Instance.InfoLog("Exam date from the study card" + date.ToString());
                Logger.Instance.InfoLog("Exam date from the report" + report_data1["Exam Date:"]);
                if (reportcount > 1 && report_data1 != null && report_data2 != null && report_data1["Exam Date:"].Equals(date))
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
                viewer.CloseReport_BR(0);

                //Step-5 - Validate exam date
                viewer.OpenReport_BR(2);
                PageLoadWait.WaitForFrameLoad(1);
                var prior2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[2];
                var date2 = prior2.FindElement(By.CssSelector(BluRingViewer.div_examListPanelDate)).GetAttribute("innerHTML") + " " +
                    prior2.FindElement(By.CssSelector(BluRingViewer.div_priorTime)).GetAttribute("innerHTML");
                viewer.NavigateToReportFrame();
                var report_data_2 = viewer.FetchReportData_BR(1);
                viewer.CloseReport_BR(1);
                if (report_data_2["Exam Date:"].Contains(date2))
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

                //Step-6 - Validate reports launched for each prior
                PageLoadWait.WaitForFrameLoad(1);
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: arrAccession[1], Datasource: arrdatasource[0], Description: arrDescription[1]);
                studies.SelectStudy("Accession", arrAccession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                int priorcount6 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).Count;
                ExecutedSteps++;
                Logger.Instance.InfoLog("Priors count: "+ priorcount6);
                for (int iterate = 2; iterate <= priorcount6-2; iterate++)
                {
                    viewer.OpenReport_BR(iterate);
                    var report_data6 = viewer.FetchReportData_BR(0);
                    if (report_data6 != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.ErrorLog("Report Data is available for prior" + priorcount6);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.ErrorLog("Report Data not available for prior" + priorcount6);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }

                }

                //Logout Application  
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

        /// <summary>
		///  Printing Reports from Universal viewer
		/// </summary>
		/// <returns></returns>
		public TestCaseResult Test_166639(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] Acc = accession.Split(':');
                //Setting Print preference
                BasePage.SetPrintPreferenceChrome();

                //Step-1:Load a study which has SR , PDF reports in Universal Viewer
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Acc[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Acc[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-2:Click on the reports icon beside the study under Exam List                
                viewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var reportcount = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div)).Count;
                viewer.SelectReport_BR(0, 4, "SR");
                var report_data1 = viewer.FetchReportData_BR(0);
                viewer.SelectReport_BR(0, 0, "PDF");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.PDFContainer_div));
                if (ReportContainer.Displayed && reportcount == 10 && report_data1 != null && string.Equals(report_data1["Last Name:"].ToLower().Replace(",", ""), Lastname.ToLower()))
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
                viewer.CloseBluRingViewer();

                //Step-3 Ensure that the report viewer is in dark theme and font is in white color for all the reports 
                //say as PDF, SR, DOC
                studies.SearchStudy(AccessionNo: Acc[1], Datasource: login.GetHostName(Config.EA96));
                PageLoadWait.WaitForLoadingMessage(40);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Acc[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenReport_BR(1, "PDF");
                viewer.NavigateToReportFrame(reporttype: "PDF");
                string textcolor = viewer.GetColorInReport(0);//#ffffff
                string bgcolor = viewer.GetColorInReport(1);
                if (string.Equals(textcolor, "rgb(255, 255, 255)") && string.Equals(bgcolor, "rgb(50, 50, 50)"))
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

                //Step-4:Click on the print button and ensure that the theme is in white color and the font is in black color
                string windowtitle = "IBM iConnect® Access";
                var currentWindow = BasePage.Driver.CurrentWindowHandle;
                ExecutedSteps++;

                //Step-5:Click on " Print " button in the print page and ensure that the theme and font color in the printed paper
                var step5 = result.steps[++ExecutedSteps];
                StudyViewer.DownaloadPrintPDF(step5, testid, ExecutedSteps);
                if (StudyViewer.ComparePDFs(step5.goldimagepath, step5.testimagepath, step5, ExecutedSteps, testid))
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
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.CloseReport_BR(0);
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);


                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }

        }

    }
}
