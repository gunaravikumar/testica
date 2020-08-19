using System;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.Configuration;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml;
using System.Windows;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using TestStack.White.Factory;
using System.Text.RegularExpressions;
using Selenium.Scripts.Pages.eHR;
using Ranorex;
using Ranorex.Core;
using Ranorex.Controls;
using RXButton = Ranorex.Button;
using TestStack.White.UIItems.ListBoxItems;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Data;
using Selenium.Scripts.Pages.iCAInstaller;

namespace Selenium.Scripts.Tests
{
    class FinalChecking
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }
        public ServiceTool servicetool { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public WpfObjects wpfobject { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public ExamImporter ei { get; set; }
        public iCAInstaller icainstaller { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public FinalChecking(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ehr = new EHR();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
            mpaclogin = new MpacLogin();
            ei = new ExamImporter();
        }

        /// <summary>
        /// Print
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108539(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            StudyViewer viewer = null;
            Studies studies = null;
            string[] Accession = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                //Step 1: Log on the ICA with valid credentials.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;
                //Step 2: Navigate to Studies tab and enter a criteria to search studies.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                if (studies.CheckStudy("Accession", Accession[0]))
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
                //Step 3: Select a study and Click on the view study button.
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 4: From the Review toolbar, Click on the Print icon
                viewer.SelectToolInToolBar("PrintView");
                PageLoadWait.WaitForFrameLoad(20);
                var PrintWindow = BasePage.Driver.WindowHandles.Last();
                var StudyWindow = BasePage.Driver.CurrentWindowHandle;
                BasePage.Driver.SwitchTo().Window(PrintWindow);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#viewerImg_1_1")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (login.CompareImage(result.steps[ExecutedSteps], viewer.PrintView()))
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
                //Step 5: From the Printable window, Click on the Print button
                //Step 6: Compare the Printed document with the viewer.
                //Step 5 and Step 6 cannot be automated because study need to verify between viewer and printed viwer.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 7: Repeat steps 3-6 in HTML 5 viewer.
                //Step 7 cannot be automated because study need to verify between viewer and printed viwer.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 8: Select a study that has a report
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(StudyWindow);
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[1]);
                if (studies.CheckStudy("Accession", Accession[1]))
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
                //Step 9: Click on the view button
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 10: Click on the Report icon
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));
                viewer.TitlebarReportIcon().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                if (viewer.ReportFullScreenIcon().Displayed)
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
                //Step 11: From the Review toolbar, Click on the Print icon.
                viewer.ReportFullScreenIcon().Click();
                viewer.SelectToolInToolBar("PrintView");
                PageLoadWait.WaitForFrameLoad(20);
                PrintWindow = BasePage.Driver.WindowHandles.Last();
                StudyWindow = BasePage.Driver.CurrentWindowHandle;
                BasePage.Driver.SwitchTo().Window(PrintWindow);
                if (viewer.PrintButton().Enabled)
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
                //Step 12: From the Printable window, Click on the Print button
                //Step 12 cannot be automated because study need to verify between viewer and printed viwer.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 13: Repeat steps 9-12 in HTML 5 viewer
                //Step 13 cannot be automated because study need to verify between viewer and printed viwer.
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 14: Print using different page orientations and size.
                result.steps[++ExecutedSteps].status = "Not Automated";
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(StudyWindow);
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
        /// Patient Name Search
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108541(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            DomainManagement domainmanagement = null;
            StudyViewer viewer = null;
            Studies studies = null;
            string[] PatientName = null;
            string[] Accession = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                //Precondition
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ModifyEnableFeatures();
                servicetool.EnablePatientNameSearch(true);
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain, new string[] { login.GetHostName(Config.EA77), login.GetHostName(Config.EA1) });
                domainmanagement.SearchDomain(createDomain[DomainManagement.DomainAttr.DomainName]);
                domainmanagement.SelectDomain(createDomain[DomainManagement.DomainAttr.DomainName]);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                domainmanagement.SetCheckBoxInEditDomain("PatientNameSearch", 0);
                domainmanagement.ClickSaveEditDomain();
                login.Logout();

                //Step 1: Log on the ICA with valid credentials.
                login.LoginIConnect(createDomain[DomainManagement.DomainAttr.UserID], createDomain[DomainManagement.DomainAttr.Password]);
                ExecutedSteps++;
                //Step 2: Navigate to Studies tab and Navigate to "Patient name search"
                studies = (Studies)login.Navigate("Studies");
                basepage.ClickElement(basepage.RadioBtn_PatientNameSearch());
                PageLoadWait.WaitForFrameLoad(20);
                if (basepage.PatNmeField().Displayed)
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
                //Step 3: Enter a Search criteria (Eg : A*) in Patient name field and set the following: Study Performed: All dates Data source : All
                basepage.PatientNameSearch(PatientName[0]);
                Dictionary<int, string[]> SearchResults = BasePage.GetSearchResults();
                string[] Col_PatientName = BasePage.GetColumnValues(SearchResults, "Patient Name", BasePage.GetColumnNames());
                if (Col_PatientName.Length > 0 && Col_PatientName.All(pat => pat.Split(' ').Any(pn => pn.StartsWith("A", StringComparison.OrdinalIgnoreCase))))
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
                //Step 4: Select a study and Click on the view study button.
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
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
                //Step 5: Enter a Search criteria (Eg : *A) in Patient name field and set the following: Study Performed: All dates Data source : All
                viewer.CloseStudy();
                basepage.PatientNameSearch(PatientName[1]);
                SearchResults = BasePage.GetSearchResults();
                Col_PatientName = BasePage.GetColumnValues(SearchResults, "Patient Name", BasePage.GetColumnNames());
                if (Col_PatientName.Length > 0 && Col_PatientName.All(pn => pn.ToLowerInvariant().Contains("a")))
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
                //Step 6: Select a study and Click on the view study button.
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
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
                //Step 7: Enter a Search criteria (Eg : A J) in Patient name field and set the following: Study Performed: All dates Data source : All
                viewer.CloseStudy();
                basepage.PatientNameSearch(PatientName[2]);
                SearchResults = BasePage.GetSearchResults();
                Col_PatientName = BasePage.GetColumnValues(SearchResults, "Patient Name", BasePage.GetColumnNames());
                if (Col_PatientName.Length > 0 && Col_PatientName.All(pat => pat.Split(' ').Any(pn => pn.StartsWith("A", StringComparison.OrdinalIgnoreCase))) && Col_PatientName.All(pn => pn.ToLowerInvariant().Contains("j")))
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
                //Step 8: Select a study and Click on the view study button.
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
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
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.ModifyEnableFeatures();
                servicetool.EnablePatientNameSearch(false);
                servicetool.ApplyEnableFeatures();
                servicetool.AcceptDialogWindow();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary>
        /// URL Integration
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108666(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            string[] accession = null;
            string[] patientname = null;
            string url = string.Empty;
            IntegratorStudies integratorstudies = new IntegratorStudies();
            DataTable intgrtable = null;
            StudyViewer viewer = new StudyViewer();
            int resultcount = 0;
            string[] FieldNames = null;
            string[] FieldValues = null;
            string[] Studypath = null;
            try
            {
                FieldNames = new string[] { "MRN:", "Patient Name:", "DOB:", "Gender:", "Issuer of PID:" };
                FieldValues = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('!')[1].Split('=');
                accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                Studypath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                patientname = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('!')[0].Split('=');
                //Pre Condition
                //Disable Default HTML5 Viewer
                servicetool.LaunchServiceTool();
                servicetool.EnableHTML5(EnableHTML5: false);
                servicetool.CloseServiceTool();
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Integrator.OnMultipleStudy']", "value", "Show Error");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/IntegratedMode/AllowShowSelector", "True");
                servicetool.RestartIISUsingexe();
                //Step 1: Note down Study UID or accession number of a prior study that has reports in datasource
                BasePage.RunBatchFile(Config.batchfilepath, Studypath[0] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                login.SendHL7Order(Config.DestinationPACS, int.Parse(Config.mpacport), Studypath[1]);
                ExecutedSteps++;
                //Step 2: Launch TestEHR application. Enable priors from TestEHR. Enter the necessary study information and click on Load (Note; when Study UID is given, no other values should be given)
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(SearchPriors: "True", showReport: "True");
                ehr.SetSearchKeys_Study(accession[0]);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.DriverGoTo(url);
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (viewer.ViewStudy(IntegratedDesktop: true))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Study Viewed Successfully");
                }
                integratorstudies.NavigateToHistoryPanel();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame");
                if (viewer.ReportTab().Displayed)
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Report viewed in History Panel");
                }
                if (resultcount == 2)
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
                //Step 3: Perform tool operations, annotations on the loaded study and save the changes
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                int ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame");
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) && viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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
                //Step 4: Provide a search criteria that matches several patients and click on load button from TestEHR
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(showSelector: "True", SearchPriors: "True", showReport: "True");
                ehr.SetSearchKeys_Study(accession[1]);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.DriverGoTo(url);
                login.NavigateToIntegratorURL(url);
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                intgrtable = basepage.CollectRecordsInTable(integratorstudies.ListTable(), integratorstudies.Intgr_Header(), integratorstudies.Intgr_Row(), integratorstudies.Intgr_Column());
                string[] columnvalue = basepage.GetColumnValues(intgrtable, "Accession");
                if (columnvalue.All(cv => string.Equals(cv, accession[1], StringComparison.OrdinalIgnoreCase)))
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
                //Step 5: Perform various search keys available (Both Patient and Study centric) in the TestEHR and load the URL 
                resultcount = 0;
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(selectoroption: "Patient", showSelector: "True", SearchPriors: "True", showReport: "True");
                ehr.SetSearchKeys_Study(accession[1]);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.DriverGoTo(url);
                login.NavigateToIntegratorURL(url);
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                basepage.ClickElement(integratorstudies.Intgtr_CheckBoxes()[0]);
                basepage.ClickElement(integratorstudies.Intgr_ViewBtn());
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                if (viewer.PatientInfo().ToLowerInvariant().Contains(patientname[0].ToLowerInvariant()) && viewer.PatientInfo().ToLowerInvariant().Contains(patientname[1].ToLowerInvariant()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Study Displayed Based on EHR Patient Selector Option");
                }
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(selectoroption: "Study", showSelector: "True", SearchPriors: "True", showReport: "True");
                ehr.SetSearchKeys_Study(accession[1]);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.DriverGoTo(url);
                login.NavigateToIntegratorURL(url);
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                basepage.ClickElement(integratorstudies.Intgtr_CheckBoxes()[0]);
                basepage.ClickElement(integratorstudies.Intgr_ViewBtn());
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                if (viewer.PatientInfo().ToLowerInvariant().Contains(patientname[0].ToLowerInvariant()) && viewer.PatientInfo().ToLowerInvariant().Contains(patientname[1].ToLowerInvariant()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Study Displayed Based on EHR Study Selector Option");
                }
                if (resultcount == 2)
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
                //Step 6: Setup any encryption service in the iCA service tool and load URL from TestEHR with encryption enabled
                string Encryption_TripleDes_Key = "TripleDES";
                string Encryption_TripleDesA_Key = "TripleDES-A";

                string Encryption_Passpharse_TripleDes_Key = "mergehealthcare";
                string Encryption_Passphares_TripleDes_A_key = "cedaracare";
                string adminUserName = Config.adminUserName;
                string adminpassword = Config.adminPassword;
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 3, 0);
                //WebDriverWait wait1;
                //bool StudyPanelExists;

                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                servicetool.EnablePatient();
                servicetool.EnableMergeEMPI();
                wpfobject.WaitTillLoad();

                servicetool.NavigateToTab(ServiceTool.Encryption_Tab);
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab("Key Generator");
                String[] TripleDESGeneratedKey = servicetool.GenerateEncryptionKeys(Encryption_Passpharse_TripleDes_Key, keysize: "192 bit (Key for TripleDES, AES)");
                String[] TripleDESAGeneratedKey = servicetool.GenerateEncryptionKeys(Encryption_Passphares_TripleDes_A_key, keysize: "192 bit (Key for TripleDES, AES)");

                // Create Encrption Service for "Triple DES"
                servicetool.SetEncryptionEncryptionService();
                servicetool.WaitWhileBusy();
                servicetool.EnterServiceEntry(Key: Encryption_TripleDes_Key, Assembly: "OpenContent.Generic.Core.dll", Class: "OpenContent.Core.Security.Services.TripleDES");
                wpfobject.GetButton("Apply", 1).Click();
                servicetool.EnterServiceParameters("key", "string", TripleDESGeneratedKey[0]);
                servicetool.EnterServiceParameters("iv", "string", "");
                servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
                servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();

                servicetool.WaitWhileBusy();
                servicetool.SetEncryptionEncryptionService();
                servicetool.WaitWhileBusy();
                servicetool.EnterServiceEntry(Key: Encryption_TripleDesA_Key, Assembly: "OpenContent.Generic.Core.dll", Class: "OpenContent.Core.Security.Services.TripleDES");
                wpfobject.GetButton("Apply", 1).Click();
                servicetool.EnterServiceParameters("key", "string", TripleDESAGeneratedKey[0]);
                servicetool.EnterServiceParameters("iv", "string", "");
                servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
                servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                wpfobject.GetMainWindowByTitle("Service Entry Form");
                wpfobject.GetButton("OK", 1).Click();

                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab("Integrator Url");
                wpfobject.WaitTillLoad();

                servicetool.ClickModifyFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.SelectCheckBox("URL Encryption Enabled", 1);
                wpfobject.WaitTillLoad();
                TextBox ID = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 1);
                ID.BulkText = "ID-123";
                wpfobject.WaitTillLoad();
                TextBox ArugumentName = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 0);
                ArugumentName.BulkText = "args";
                wpfobject.WaitTillLoad();
                wpfobject.SetText("PART_EditableTextBox", "Cryptographic." + Encryption_TripleDes_Key);
                wpfobject.ClickButton("Add", 1);
                wpfobject.WaitTillLoad();

                ID = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 1);
                ID.BulkText = "ID-456";
                wpfobject.WaitTillLoad();
                ArugumentName = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 0);
                ArugumentName.BulkText = "args";
                wpfobject.WaitTillLoad();
                wpfobject.SetText("PART_EditableTextBox", "Cryptographic." + Encryption_TripleDesA_Key);
                wpfobject.ClickButton("Add", 1);
                wpfobject.WaitTillLoad();

                ComboBox DefaultEncryptionProvider = wpfobject.GetUIItem<ITabPage, ComboBox>(servicetool.GetCurrentTabItem(), 0);
                DefaultEncryptionProvider.Enter("ID-123");
                wpfobject.WaitTillLoad();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception) { }
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled", shadowuser: "Always Enabled");
                servicetool.ClickModifyButton();
                servicetool.AllowShowSelectorSearch().Checked = true;
                servicetool.AllowShowSelector().Checked = true;
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(SearchPriors: "True", showReport: "True");
                ehr.SetSearchKeys_Study(accession[2]);
                wpfobject.SelectCheckBox("encryptEnabledCheckBox");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.DriverGoTo(url);
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
                //Step 7: Select URL determined option from Integrator tab of iCA Service tool. Reset IIS. Select "Launch Exam Importer" tab from TestEHR. Provide any search criteria. Copy the URL from TestEHR application and load the URL in the browser where Exam importer has been installed and minimized in any client machine
                if (basepage.NodeExist(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES']"))
                {
                    basepage.RemoveNode(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES']");
                }
                if (basepage.NodeExist(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES-A']"))
                {
                    basepage.RemoveNode(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES-A']");
                }
                if (basepage.NodeExist(Config.FileLocationPath, "/EncryptionProviders"))
                {
                    basepage.RemoveChildNode(Config.FileLocationPath, "/EncryptionProviders");
                }
                basepage.ChangeAttributeValue(Config.FileLocationPath, "/EncryptionProviders", "default", "");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/EncryptionEnabled", "False");
                servicetool.RestartIISUsingexe();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "URL Determined");
                servicetool.CloseServiceTool();
                ehr.LaunchEHR();
                wpfobject.SelectTabFromTabItems("Launch Exam Importer");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", user: Config.stUserName, usersharing: "True", domain: "SuperAdminGroup");
                ehr.PatientName().BulkText = FieldValues[1];
                ehr.PatientID().BulkText = FieldValues[0];
                ehr.PatientDOB().BulkText = FieldValues[2];
                ehr.IPID().BulkText = FieldValues[4];
                ehr.Gender().BulkText = FieldValues[3];
                url = ehr.clickCmdLine();
                ei.LaunchEI(Config.EIFilePath);
                wpfobject.GetMainWindow(Config.eiwindow);
                WpfObjects._mainWindow.WaitWhileBusy();
                Button minimize = WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("Minimize"));
                minimize.Click();
                ehr.CloseEHR();
                login.NavigateToIntegratorURL(url);
                Thread.Sleep(10000);
                WpfObjects._mainWindow = WpfObjects._application.GetWindow(SearchCriteria.ByText(Config.eiwindow), InitializeOption.NoCache);
                wpfobject.GetMainWindow(Config.eiwindow);
                GroupBox demographics1 = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("New Patient Demographics"));
                IList<IUIItem> list = demographics1.GetMultiple(SearchCriteria.All);
                int fieldcount = (list.Count / 2) + 1, j, k = 0;
                string[] fields = new string[list.Count / 2];
                for (j = 1; j < fieldcount; j++, k++)
                {
                    Label labelname = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, FieldNames[k], 1);
                    fields[j - 1] = labelname.Text;
                }
                string[] values = new string[list.Count / 2];
                for (int t = 0; t < list.Count / 2; t++)
                {
                    Label labelname = wpfobject.GetAnyUIItem<GroupBox, Label>(demographics1, FieldValues[t], 1);
                    values[t] = labelname.Text;
                }

                if (fields[0].Equals(FieldNames[0]) && fields[1].Equals(FieldNames[1]) && fields[2].Equals(FieldNames[2]) &&
                    fields[3].Equals(FieldNames[3]) && fields[4].Equals(FieldNames[4]) && values[0].Equals(FieldValues[0]) &&
                    values[1].Equals(FieldValues[1]) && values[2].Equals(FieldValues[2]) && values[3].Equals(FieldValues[3]) &&
                    values[4].Equals(FieldValues[4]))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                servicetool.CloseServiceTool();
                ehr.CloseEHR();
                BasePage.KillProcess("UploaderTool");
                if (basepage.NodeExist(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES']"))
                {
                    basepage.RemoveNode(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES']");
                }
                if (basepage.NodeExist(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES-A']"))
                {
                    basepage.RemoveNode(Config.ServiceFactoryConfigPath, "/add[@key='Cryptographic.TripleDES-A']");
                }
                if (basepage.NodeExist(Config.FileLocationPath, "/EncryptionProviders"))
                {
                    basepage.RemoveChildNode(Config.FileLocationPath, "/EncryptionProviders");
                }
                basepage.ChangeAttributeValue(Config.FileLocationPath, "/EncryptionProviders", "default", "");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/EncryptionEnabled", "False");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/IntegratedMode/AllowShowSelector", "False");
                servicetool.RestartIISUsingexe();
                servicetool.LaunchServiceTool();
                //Enable Default HTML5 Viewer
                servicetool.EnableHTML5(EnableHTML5: true);
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Disabled", shadowuser: "Always Disabled");
                servicetool.CloseServiceTool();
            }
        }

        /// <summary>
        /// XDS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108668(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            string status = string.Empty;
            Patients patient = null;
            string[] PatientName = null;
            BasePage basepage = new BasePage();
            Studies studies = null;
            StudyViewer viewer = null;
            string[] Accession = null;
            Outbounds outbounds = null;
            Inbounds inbounds = null;
            RoleManagement rolemanagement = null;
            string[] validateaccession = null;
            try
            {
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                validateaccession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                //Step 1: Load XDS-only studies from Patients or studies tab into the viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("email", 0);
                if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
                {
                    rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                }
                rolemanagement.ClickSaveEditRole();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("Physician");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("email", 0);
                if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
                {
                    rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                }
                rolemanagement.ClickSaveEditRole();
                patient = (Patients)login.Navigate("Patients");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.InputData(PatientName[0].Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.LoadStudyInPatientRecord(PatientName[0].Trim());
                patient.NavigateToXdsStudies();
                viewer = patient.LaunchStudy(Patients.PatientColumns.Accession, Accession[0]);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (patient.CompareImage(result.steps[ExecutedSteps], viewport))
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
                //Step 2: Load hybrid studies alone from Patients or studies tab into the viewer
                viewer.CloseStudy();
                patient.ClosePatientRecord();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.InputData(PatientName[1].Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.LoadStudyInPatientRecord(PatientName[1].Trim());
                patient.NavigateToXdsStudies();
                viewer = patient.LaunchStudy(Patients.PatientColumns.Accession, Accession[1]);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (patient.CompareImage(result.steps[ExecutedSteps], viewport))
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
                //Step 3: E-mail XDS\hybrid studies to any user
                viewer.CloseStudy();
                patient.ClosePatientRecord();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.EmailStudy("108665@mergetestmail.com", "Test", "Test", 1);
                viewer.CloseStudy();
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(AccessionNo: Accession[1]);
                outbounds.GetMatchingRow("Accession", Accession[1]).TryGetValue("Status", out status);
                if (status == "Emailed")
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
                //Step 4: Grant access XDS\hybrid studies to any user
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                studies.ShareStudy(false, new String[] { Config.ph1UserName });
                login.Logout();
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[1]);
                if (inbounds.CheckStudy("Accession", Accession[1]))
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
                //Step 5: Transfer XDS\hybrid studies to local or any other datasource
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                patient = (Patients)login.Navigate("Patients");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.InputData(PatientName[2].Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.LoadStudyInPatientRecord(PatientName[2].Trim());
                patient.NavigateToXdsStudies();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForElement(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr"), BasePage.WaitTypes.Visible, 30);
                List<IWebElement> tr = BasePage.Driver.FindElements(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr")).ToList();
                for (int i = 1; i < tr.Capacity; i++)
                {
                    if (tr[i].Displayed)
                    {
                        List<IWebElement> data = tr[i].FindElements(By.TagName("td")).ToList();
                        if (data[(int)Patients.PatientColumns.Accession].Text.ToLower().Equals(validateaccession[1].ToLower()))
                        {
                            basepage.ClickElement(tr[i]);
                            break;
                        }
                    }
                }
                patient.TransferStudy("Local System", SelectallPriors: false, PatientTab: true);
                PageLoadWait.WaitForDownload("_" + PatientName[2].Split(',')[0], Config.downloadpath, "zip");
                if (BasePage.CheckFile("_" + PatientName[2].Split(',')[0], Config.downloadpath, "zip"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //Step 6: Launch XDS\hybrid study containing priors from TestEHR by providing search keys
                login.Logout();
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(SearchPriors: "True", showReport: "True");
                ehr.SetSearchKeys_Study(Accession[2]);
                string url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.DriverGoTo(url);
                login.NavigateToIntegratorURL(url);
                viewer = (StudyViewer)login.NavigateToIntegratorFrame();
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });
                string[] Accessions = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
                if (validateaccession.All(va => Accessions.Contains(va, StringComparer.OrdinalIgnoreCase)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Patient
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108669(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BasePage basepage = new BasePage();
            Patients patients = null;
            StudyViewer viewer = null;
            Outbounds outbounds = null;
            Inbounds inbounds = null;
            string[] patientname = null;
            string[] Accession = null;
            string status = string.Empty;
            int resultcount;
            try
            {
                patientname = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                //Step 1: Launch iCA and login as any user
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                RoleManagement rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("email", 0);
                if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
                {
                    rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                }
                rolemanagement.ClickSaveEditRole();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SelectRole("Physician");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("email", 0);
                if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
                {
                    rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                }
                rolemanagement.ClickSaveEditRole();
                patients = (Patients)login.Navigate("Patients");
                if (login.IsTabSelected("Patients"))
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
                //Step 2: Search the patient using different search keys available
                resultcount = 0;
                for (int i = 1; i < 3; i++)
                {
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    patients.InputData(patientname[i]);
                    patients.ClickPatientSearch();
                    PageLoadWait.WaitForPatientsLoadingMsg(15);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    string[] names = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Name", BasePage.GetColumnNames());
                    if (names.All(name => name.ToLowerInvariant().Contains(patientname[i].ToLowerInvariant())))
                    {
                        resultcount++;
                        Logger.Instance.InfoLog("Search displays record for the patient " + patientname[i]);
                    }
                }
                if (resultcount == 2)
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
                //Step 3: Select any patient record and view
                patients.LoadStudyInPatientRecord(patientname[3]);
                patients.NavigateToXdsStudies();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                string[] accession = basepage.GetColumnValues(basepage.CollectRecordsInTable(patients.XDSStudiesPatientTable(), patients.XDSStudiesPatientheader(), patients.XDSStudiesPatientrow(), patients.XDSStudiesPatientcolumn()), "Accession#");
                if (accession.All(acc => Accession.Contains(acc)))
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
                //Step 4: Select any study and launch it in the viewer
                viewer = patients.LaunchStudy(Patients.PatientColumns.Accession, Accession[0]);
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
                //Step 5: Perform tool operations from review toolbar for the study launched from Patients tab
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                IWebElement viewport = viewer.SeriesViewer_1X1();
                int h = viewport.Size.Height;
                int w = viewport.Size.Width;
                viewer.DragandDropImage(viewport, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (patients.CompareImage(result.steps[ExecutedSteps], viewport))
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
                //Step 6: E-mail/Transfer/Grant access for studies loaded from Patients tab
                resultcount = 0;
                //Email
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                viewer.EmailStudy("108665@mergetestmail.com", "Test", "Test", 1);
                viewer.CloseStudy();
                patients.ClosePatientRecord();
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(AccessionNo: Accession[0]);
                outbounds.GetMatchingRow("Accession", Accession[0]).TryGetValue("Status", out status);
                if (status == "Emailed")
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Study Emailed Successfully");
                }
                //Transfer
                patients = (Patients)login.Navigate("Patients");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patients.InputData(patientname[0].ToLower().Trim());
                patients.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patients.LoadStudyInPatientRecord(patientname[4].Trim());
                patients.NavigateToXdsStudies();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForElement(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr"), BasePage.WaitTypes.Visible, 30);
                List<IWebElement> tr = BasePage.Driver.FindElements(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr")).ToList();
                for (int i = 1; i < tr.Capacity; i++)
                {
                    if (tr[i].Displayed)
                    {
                        List<IWebElement> data = tr[i].FindElements(By.TagName("td")).ToList();
                        if (data[(int)Patients.PatientColumns.Accession].Text.ToLower().Equals(Accession[1].ToLower()))
                        {
                            basepage.ClickElement(tr[i]);
                            break;
                        }
                    }
                }
                patients.TransferStudy("Local System", SelectallPriors: false, PatientTab: true);
                PageLoadWait.WaitForDownload("_" + patientname[0], Config.downloadpath, "zip");
                if (BasePage.CheckFile("_" + patientname[0], Config.downloadpath, "zip"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Study Transferred Successfully");
                }
                //Grant Access
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForElement(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr"), BasePage.WaitTypes.Visible, 30);
                tr = BasePage.Driver.FindElements(By.CssSelector("#RadiologyStudiesListControl_parentGrid>tbody>tr")).ToList();
                for (int i = 1; i < tr.Capacity; i++)
                {
                    if (tr[i].Displayed)
                    {
                        List<IWebElement> data = tr[i].FindElements(By.TagName("td")).ToList();
                        if (data[(int)Patients.PatientColumns.Accession].Text.ToLower().Equals(Accession[2].ToLower()))
                        {
                            basepage.ClickElement(tr[i]);
                            break;
                        }
                    }
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                patients.ShareStudy(false, new String[] { Config.ph1UserName }, PatientTab: true);
                login.Logout();
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[2]);
                if (inbounds.CheckStudy("Accession", Accession[2]))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Study Shared Successfully");
                }
                if (resultcount == 3)
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
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  108540 - Saving GSPS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108540(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            String adminusername = Config.adminUserName;
            String adminpassword = Config.adminPassword;
            String SuperAdminDomain = "SuperAdminGroup";
            ServiceTool servicetool = new ServiceTool();
            Studies studies;
            StudyViewer viewer = new StudyViewer();
            DomainManagement domain = new DomainManagement();

            string AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String[] AccessionID = AccessionIDList.Split(':');
            string DicomPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String datasource = Config.DestinationPACS;
            String aetitle = Config.DestinationPACSAETitle;


            TestCaseResult result = new TestCaseResult(stepcount); ;
            int ExecutedSteps = -1;
            Login login = new Login();
            String destpacs = login.GetHostName(Config.DestinationPACS);
            WpfObjects wpfobject = new WpfObjects();
            // DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
            StudyViewer studyviewer = new StudyViewer();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {

                // Pre Condition
                // 1.From the Service tool, Enable features, General --> Check "Enable Saving GSPS" option.
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSavingGSPS();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception e)
                { }
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                // 2. From Domain management Check "Enable Saving GSPS" option.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain.SearchDomain(SuperAdminDomain);
                domain.SelectDomain(SuperAdminDomain);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                domain.ClickSaveDomain();
                login.Logout();
                BasePage.RunBatchFile(Config.batchfilepath, DicomPath + " " + Config.dicomsendpath + " " + Config.DestinationPACS);

                // Step 1
                // Log on the ICA with valid credentials.	
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                if (login.IsTabPresent("Studies"))
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

                // Step 2
                // Navigate to Studies tab and enter a criteria to search studies.	
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionID[0], Datasource: destpacs);
                Dictionary<String, String> results = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionID[0] });
                if (results != null)
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

                // Step 3
                // Select a study and Click on the view study button.	
                studies.SelectStudy1("Accession", AccessionID[0]);
                studies.LaunchStudy();
                if (studies.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                // Step 4
                // From the Review toolbar, Click on the Save series icon.	
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                studies.CloseStudy();
                studies.SearchStudy(AccessionNo: AccessionID[0], Datasource: destpacs);
                Dictionary<string, string> row6 = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionID[0] });
                if (row6 != null && row6["Modality"].Contains("PR"))
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

                // Step 5
                // Draw Annotations on few images of the series and click on the Save annotations Icon from the Review toolbar.
                studies.SelectStudy1("Accession", AccessionID[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement source = viewer.ViewportScrollHandle(1, 2);
                IWebElement destination = viewer.ViewportScrollBar(1, 2);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action2 = new Actions(BasePage.Driver);
                action2.ClickAndHold(source).MoveToElement(destination, w / 2, h / 2).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                IWebElement viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);

                int ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveAnnotatedImages);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) &&
                viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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
                studies.CloseStudy();


                // Step 6
                //Step-11: Load same study in HTML5 viewer
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.Viewer_Tab);
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                wpfobject.WaitTillLoad();
                servicetool.EnableHTML5();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(AccessionNo: AccessionID[1], Datasource: destpacs);
                studies.SelectStudy1("Accesion", AccessionID[1]);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action2 = new Actions(BasePage.Driver);
                action2.ClickAndHold(source).MoveToElement(destination, w / 2, h / 2).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);

                ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveAnnotatedImages);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) &&
                viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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
                studies.CloseStudy();

                // Step 7
                // Repeat steps 3-5 under Https (Both HTML4 and HTML 5 viewer).
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                wpfobject.WaitTillLoad();
                if (wpfobject.IsCheckBoxSelected(ServiceTool.Security.Name.HTTPS, 1))
                {
                    wpfobject.SelectCheckBox(ServiceTool.Security.Name.HTTPS, 1);
                }
                if (wpfobject.IsCheckBoxSelected(ServiceTool.Security.Name.HTTP, 1))
                {
                    wpfobject.UnSelectCheckBox(ServiceTool.Security.Name.HTTP, 1);
                }
                wpfobject.WaitTillLoad();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception e)
                { }
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.EnableHTML5();
                servicetool.CloseServiceTool();


                BasePage basepage = new BasePage();
                String hostname = basepage.GetHostName(Config.IConnectIP);

                login.DriverGoTo("https://" + hostname + "/webaccess");
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(AccessionNo: AccessionID[2], Datasource: destpacs);
                studies.SelectStudy1("Accesion", AccessionID[2]);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action2 = new Actions(BasePage.Driver);
                action2.ClickAndHold(source).MoveToElement(destination, w / 2, h / 2).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);

                ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveAnnotatedImages);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) &&
                viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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
                studies.CloseStudy();


                //change to HTMl 4
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToConfigToolSecurityTab();
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                wpfobject.WaitTillLoad();
                if (wpfobject.IsCheckBoxSelected(ServiceTool.Security.Name.HTTPS, 1))
                {
                    wpfobject.UnSelectCheckBox(ServiceTool.Security.Name.HTTPS, 1);
                }
                if (wpfobject.IsCheckBoxSelected(ServiceTool.Security.Name.HTTP, 1))
                {
                    wpfobject.SelectCheckBox(ServiceTool.Security.Name.HTTP, 1);
                }
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception e)
                { }
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.EnableHTML5(false);
                servicetool.CloseServiceTool();

                login.DriverGoTo("https://" + hostname + "/webaccess");
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(AccessionNo: AccessionID[3], Datasource: destpacs);
                studies.SelectStudy1("Accesion", AccessionID[3]);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action2 = new Actions(BasePage.Driver);
                action2.ClickAndHold(source).MoveToElement(destination, w / 2, h / 2).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Apply W/L
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X2();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);

                ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveAnnotatedImages);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1) &&
                viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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
                studies.CloseStudy();


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
        /// 108557 - Enable Attachment
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108557(String testid, String teststeps, int stepcount)
        {

            StudyViewer studyviewer = new StudyViewer();
            IntegratorStudies integratorstudies = new IntegratorStudies();
            DomainManagement domainmanagement = null;
            ServiceTool tool = new ServiceTool();
            Studies studies = new Studies();
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            By attachmentlink = null;
            BasePage basepage = new BasePage();

            string adminUserName = Config.adminUserName;
            string adminPassword = Config.adminPassword;
            string DatasourceAutoSSA = new Login().GetHostName(Config.EA77);
            string DatasourceVMSSA131 = new Login().GetHostName(Config.EA1);
            string DatasourceVMSSA91 = new Login().GetHostName(Config.EA91);
            string DatasourceDestinationPacs = new Login().GetHostName(Config.DestinationPACS);
            string DatasourcePACS2 = new Login().GetHostName(Config.PACS2);
            string DicomPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            string accessionIds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            string[] accessionid = accessionIds.Split(':');
            //string Uploadpaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            //string[] Uploadpath = Uploadpaths.Split(':');
            attachmentlink = By.CssSelector("table[id$='_attachmentList'] tr:nth-of-type(2)>td:nth-of-type(4)>a");
            String Modalitys = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String[] Modality = Modalitys.Split(':');
            string Datasource = Config.DestinationPACS;
            string AETitle = Config.DestinationPACSAETitle;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Precondition
                //Initial Set-up
                // Update attachment configuration: -in Service Tool
                // Select Enable Features Tab -select Study Attachment tab on the right hand side
                // -select Modify Select Enable Attachements, Update Allowed and Select Store attachments with original study
                // - select Apply IIS reset

                // BasePage.RunBatchFile(Config.batchfilepath, DicomPath + " " + Config.dicomsendpath + " " + Config.DestinationPACS);

                tool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                tool.NavigateToEnableFeatures();
                //tool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.EnableStudyAttachements();
                wpfobject.WaitTillLoad();
                // wpfobject.ClickRadioButton("Store attachments with original study", 1);
                tool.RestartService();
                wpfobject.WaitTillLoad();
                tool.CloseServiceTool();

                //Login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForPageLoad(10);
                //Step-1:Enable Attachment at DomainLevel(SuperAdminGroup)
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.ConnectAllDataSources();
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.ClickSaveDomain();
                login.Logout();



                // Step 1 
                // Launch any study from study list and navigate to History flyout 
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accessionid[0], Datasource: "All");
                studies.SelectStudy1("Accession", accessionid[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                studies.NavigateToHistoryPanel();
                studyviewer.NavigateTabInHistoryPanel("Attachment");
                PageLoadWait.WaitForFrameLoad(20);
                if (studyviewer.AttachmentTab().Displayed)
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

                // Step 2
                // Attach a file (jpeg, txt, pdf, etc.) to the study and click on Save
                string filename = "Test_108557_" + new Random().Next(1000) + ".txt";
                string filepath1 = DicomPath + filename;
                string text2write = "Hello World!";
                //TextWriter tw = File.CreateText(filename);
                //tw.WriteLine(text2write);
                //tw.Close();
                if (File.Exists(filepath1))
                {
                    File.Delete(filepath1);
                }
                StreamWriter writer = new StreamWriter(filepath1);
                writer.Write(text2write);
                writer.Close();
                if (studyviewer.UploadAttachment(filepath1, 20))
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
                studies.CloseStudy();

                // Step 3
                // Go back to the studylist
                studies.SearchStudy(AccessionNo: accessionid[0], Datasource: "All");
                Dictionary<string, string> modality = studyviewer.GetMatchingRow("Accession", accessionid[0]);
                if (modality["Modality"].Contains("OT"))
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

                // STep 4
                // Check the raw data saved from the original data source where the study came from	
                login.LoginIConnect(adminUserName, adminPassword);
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accessionid[0], Datasource: "All");
                studies.SelectStudy1("Accession", accessionid[0]);
                studyviewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                studyviewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                Dictionary<string, string> Filerow = studyviewer.StudyViewerListMatchingRow("Name", filename, "patienthistory", "attachment");
                if (Filerow != null)
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

                // Step 5
                // Store attachments in a single Datasource Pre-Condition:
                //-Select Store attachments in a single Datasource from Service tool
                //-select Apply
                //- IIS reset
                // Launch any study from study list and navigate to History flyout
                tool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                tool.NavigateToEnableFeatures();
                //tool.SetEnableFeaturesGeneral();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.EnableStudyAttachements();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();

                wpfobject.ClickRadioButton("Store attachments in a single data source", 1);
                TextBox DatasourceID = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 2);
                DatasourceID.BulkText = basepage.GetHostName(Datasource);
                wpfobject.WaitTillLoad();

                TextBox DatasourceAETitle = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 1);
                DatasourceAETitle.BulkText = AETitle;
                wpfobject.WaitTillLoad();

                TextBox DatasourceHost = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 0);
                DatasourceHost.BulkText = Datasource;
                wpfobject.WaitTillLoad();

                TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.Enter("104");
                // Enter the port
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton("2");
                    wpfobject.WaitTillLoad();
                }
                catch (Exception) { }

                tool.RestartService();
                wpfobject.WaitTillLoad();
                tool.CloseServiceTool();

                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accessionid[1], Datasource: "All");
                studies.SelectStudy1("Accession", accessionid[1]);
                studies.LaunchStudy();
                studies.NavigateToHistoryPanel();
                studyviewer.NavigateTabInHistoryPanel("Attachment");
                PageLoadWait.WaitForFrameLoad(20);
                if (studyviewer.AttachmentTab().Displayed)
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

                // Step 6
                // Attach a file(jpeg, txt, pdf, etc.) to the study and click on Save
                string filename2 = "Test1_108557_" + new Random().Next(1000) + ".txt";
                string filepath2 = DicomPath + filename2;
                string text2write1 = "Hello World!";
                //TextWriter tw = File.CreateText(filename);
                //tw.WriteLine(text2write);
                //tw.Close();
                if (File.Exists(filepath2))
                {
                    File.Delete(filepath2);
                }
                writer = new StreamWriter(filepath2);
                writer.Write(text2write1);
                writer.Close();
                if (studyviewer.UploadAttachment(filepath2, 20))
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
                studies.CloseStudy();

                // Step 7
                // Go back to the studylist
                studies.SearchStudy(AccessionNo: accessionid[1], Datasource: "All");
                modality = studyviewer.GetMatchingRow("Accession", accessionid[1]);
                if (modality["Modality"].Contains("OT"))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

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

                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();

                //Return Result
                return result;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Remotely Hosted DB
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108667(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;

            result = new TestCaseResult(stepcount);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String SQLsaUserID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DBUsername");
            String SQLsaPassword = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DBpassword");
            String SQLDBInstancePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DBInstanceName");
            String AccessionIds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
            String[] AccessionId = AccessionIds.Split(':');
            Studies studies = null;
            StudyViewer studyviewer = new StudyViewer();
            String License_Name = "License.xml";
            int ExecutedSteps = -1;
            // int resultcount;
            try
            {
                String GetRemoteDBHostName = new BasePage().GetHostName(Config.remotedbinstance);
                String currentDirectory = System.IO.Directory.GetCurrentDirectory();
                String DataSourceManagerConfigPath = @"C:\WebAccess\WebAccess\Config\DataSource\DataSourceManagerConfiguration.xml";
                String ConfigFileDirectory = currentDirectory + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + login.GetHostName(Config.IConnectIP);
                String DataSourceManagerConfigPath_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + "DataSourceManagerConfiguration" + Path.DirectorySeparatorChar + "DataSourceManagerConfiguration.xml";

                //License update
                String LicensePath = "C:\\WebAccess\\WebAccess\\Config\\" + License_Name;
                //Config Files - File Path
                String License_Backup = ConfigFileDirectory + Path.DirectorySeparatorChar + License_Name;

                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                BasePage.Kill_EXEProcess(iCAInstaller.InstallerEXE);
                icainstaller = new iCAInstaller();
                icainstaller.UninstalliCA();
                icainstaller.deleteDB(GetRemoteDBHostName, SQLDBInstancePath);
                // BasePage.RunBatchFile(@"D:\Iconnect_WorkSpace\iConnectScripts\Selenium\OtherFiles\delete_DB.bat", GetRemoteDBHostName+"\\"+ SQLDBInstancePath);
                taskbar.Show();

                // Step 1
                // Open command prompt where iCA installer file is available and run the following command "setup.exe FULLUI=Y" 
                icainstaller.invokeiCAFullUi(2);
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                bool wndw = wpfobject.VerifyIfTextExists(iCAInstaller.InstallBtn_Name);
                if (wndw)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("iCA fullUI mode installation screen not displayed");
                }

                //Step 2
                // Enter the DB instance of the SQL server where remote connection has been enabled and enter the credentials of the SQL server by deselecting windows authentication
                wpfobject.GetMainWindowByTitle(iCAInstaller.Installer_Name);
                wpfobject.UnSelectCheckBox("Windows Authentication", 1);
                wpfobject.SetText("DbInstance Name", GetRemoteDBHostName + Path.DirectorySeparatorChar + SQLDBInstancePath, 1);
                wpfobject.SetText("DbUserName", SQLsaUserID, 1);
                wpfobject.SetText("DbPassword", SQLsaPassword, 1);
                ExecutedSteps++;

                // Step 3
                // Click on Next and complete the iCA installation and configuration
                wpfobject.ClickButton("Install", 1);
                Thread.Sleep(30000);
                wpfobject.WaitForButtonExist(iCAInstaller.Installer_Name, "Finish", 1);
                wpfobject.ClickButton("Finish", 1);
                wpfobject.WaitTillLoad();
                // Data Source Manager Copy
                File.Copy(DataSourceManagerConfigPath_Backup, DataSourceManagerConfigPath, true);
                //License Configuration xml File
                File.Copy(License_Backup, LicensePath, true);
                ExecutedSteps++;

                // Step 4
                // Login as any user and launch any study
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.UpdateGivenDomain("SuperAdminGroup");
                PageLoadWait.WaitForPageLoad(10);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("LastName", "*");
                studies.SearchStudy("studyPerformed", "All Dates");
                studies.SearchStudy("Accession", AccessionId[0]);
                studies.SelectStudy("Accession", AccessionId[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //Return Result
                return result;
            }
        }

    }
}
