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
using System.Xml.Serialization;
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class Flyout_Reports : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public EHR ehr { get; set; }
        public Web_Uploader webuploader { get; set; }
        public RanorexObjects rnxobject { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        string FolderPath = "";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Flyout_Reports(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            rnxobject = new RanorexObjects();
            webuploader = new Web_Uploader();
            ehr = new EHR();
        }

        /// <summary>
        /// Flyout Reports - Studies with Cardio Reports
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108532(String testid, String teststeps, int stepcount)
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
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] lastName = LastName.Split(':');
                String[] PatName = PatientName.Split(':');
                String[] patientID = PatientID.Split(':');


                //Step-1: Install iCA and configure EA Datasource[Version more than or equal to 11.2].
                //ICA and EA would be configured as part of environment setup
                ExecutedSteps++;

                //Step-2: Check if Merge Cardio Reports checkbox is available under Reports tab (Service tool)
                taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.GetButton(ServiceTool.ModifyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                bool step2_1 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Visible;
                //Check if cardio report is available under reports
                bool step2_2 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Visible;
                wpfobject.WaitTillLoad();
                if (step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Uncheck Encapsulated PDF and try enabling Cardio reports
                //Uncheck Encapsulated PDF
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = false;
                //try enabling Cardio reports
                try
                {
                    servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked = true;
                }
                catch (Exception ex) { }
                bool step3 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked;
                if (!step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Enable cardio reports
                //Check Encapsulated PDF
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = true;
                //try enabling Cardio reports
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked = true;
                bool step4 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked;
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

                //Step-5: Apply and restart IIS
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                ExecutedSteps++;

                //Step-6: Load a study to EA server with version 11.2 - NA since study would be imported beforehand in EA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-7: Login to iCA and load study to HTML 4 viewer
                login.LoginIConnect(username, password);
                //Search and load study
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", lastName[0]);
                studies.SelectStudy("Patient ID", patientID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
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

                //Step-8: Load report
                StudyVw.ReportView();
                bool step8_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step8_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step8_2 = false;
                if (Step8_Patient == PatName[0])
                {
                    step8_2 = true;
                }
                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Verify Patient name, MRN, IPID, DOB and Study Date are displayed on the report as per loaded study 
                String Step9_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                String Step9_MRN = StudyVw.GetPatientDetailsFromReport("MRN:");
                String Step9_PID = StudyVw.GetPatientDetailsFromReport("PID issuer:");
                String Step9_DOB = StudyVw.GetPatientDetailsFromReport("DOB:", 2);
                String Step9_StudyDate = StudyVw.GetPatientDetailsFromReport("Study Date:");
                if (Step9_Patient == PatName[0] && Step9_MRN == PatName[1] && Step9_PID == PatName[2] && Step9_DOB == PatName[3] && Step9_StudyDate == PatName[4])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: CLose study
                PageLoadWait.WaitForFrameLoad(10);
                studies.CloseStudy();
                ExecutedSteps++;

                //Step-11: Load same study in HTML5 viewer
                studies.ClearFields();
                studies.SearchStudy("last", lastName[0]);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.LaunchStudyHTML5();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step11)
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

                //Step-12: Load report
                StudyVw.ReportView();
                bool step12_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step12_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step12_2 = false;
                if (Step12_Patient == PatName[0])
                {
                    step12_2 = true;
                }
                if (step12_1 && step12_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Verify Patient name,MRN, IPID, DOB and Study Date are displayed on the report as per loaded study
                String Step13_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                String Step13_MRN = StudyVw.GetPatientDetailsFromReport("MRN:");
                String Step13_PID = StudyVw.GetPatientDetailsFromReport("PID issuer:");
                String Step13_DOB = StudyVw.GetPatientDetailsFromReport("DOB:", 2);
                String Step13_StudyDate = StudyVw.GetPatientDetailsFromReport("Study Date:");
                if (Step13_Patient == PatName[0] && Step13_MRN == PatName[1] && Step13_PID == PatName[2] && Step13_DOB == PatName[3] && Step13_StudyDate == PatName[4])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: CLose study
                studies.CloseStudy();
                ExecutedSteps++;
                login.Logout();

                //Step-15: Import another study to EA 11.2 - NA since study would be imported beforehand in EA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-16: Login to iCA and load study to HTML 4 viewer
                login.LoginIConnect(username, password);
                //Search and load study
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("last", lastName[1]);
                studies.SelectStudy("Patient ID", patientID[1]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step16)
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

                //Step-17: Load report
                StudyVw.ReportView();
                bool step17_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step17_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step17_2 = false;
                if (Step17_Patient == PatName[5])
                {
                    step17_2 = true;
                }
                if (step17_1 && step17_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-18: Check that Reports are sorted descendingly based on the report creation date
                PageLoadWait.WaitForFrameLoad(10);
                StudyVw.ViewerReportListButton().Click();
                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                //Get Date Column from Cardio Report Table
                string[] DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                DateTime[] Dates = StudyVw.ConvertStringToDate(DateValues);
                //Validate if date is in descending order
                bool step18 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step18)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19: Sort the Report list in ascending order based on the report creation date
                //Click Date
                PageLoadWait.WaitForElement(By.CssSelector("#jqgh_m_studyPanels_m_studyPanel_1_m_reportViewer_reportList_date"), WaitTypes.Visible, 10).Click();
                //Check if its acending order
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = StudyVw.ConvertStringToDate(DateValues);
                bool step19 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderBy(q => q)));
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

                //Step-20: Select any Report listed in the report panel
                StudyVw.OpenReport("Date", DateValues[1]);
                ExecutedSteps++;

                //Step-21: Close Study
                studies.CloseStudy();
                ExecutedSteps++;

                //Step-22: Load the same study which was opened previously
                studies.SelectStudy("Patient ID", patientID[1]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step22)
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

                //Step-23: Load report
                StudyVw.ReportView();
                bool step23_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step23_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step23_2 = false;
                if (Step23_Patient == PatName[5])
                {
                    step23_2 = true;
                }
                if (step23_1 && step23_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24: Check that Reports are sorted descendingly based on the report creation date
                PageLoadWait.WaitForFrameLoad(10);
                StudyVw.ViewerReportListButton().Click();
                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                //Get Date Column from Cardio Report Table
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = StudyVw.ConvertStringToDate(DateValues);
                //Validate if date is in descending order
                bool step24 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25: Click on the History fly out
                studies.NavigateToHistoryPanel();
                //studies.ChooseColumns(new string[] { "Accession" });
                ExecutedSteps++;

                //Step-26: Check that Reports are sorted descendingly based on the study date
                StudyVw.ReportTab().Click();
                PageLoadWait.WaitForElement(StudyVw.ReportListTable(), WaitTypes.Visible, 20);
                //Get Date Column from Cardio Report Table
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = StudyVw.ConvertStringToDate(DateValues);
                //Validate if date is in descending order
                bool step26 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step26)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-27: Sort the Report list in ascending order based on study date by clicking on 'study date' column heading
                PageLoadWait.WaitForElement(By.CssSelector("#jqgh_m_patientHistory_m_reportViewer_reportList_date"), WaitTypes.Visible, 10).Click();
                //Check if its acending order
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = StudyVw.ConvertStringToDate(DateValues);
                bool step27 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderBy(q => q)));
                if (step27)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28: Select any Report listed in the report panel
                StudyVw.OpenReport("Date", DateValues[1]);
                ExecutedSteps++;

                //Step-29: Verify that correct Patient name, MRN, IPID, DOB and Study Date are displayed on the report tab
                String Step29_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2, true);
                String Step29_MRN = StudyVw.GetPatientDetailsFromReport("MRN:", isHistoryPanel: true);
                String Step29_PID = StudyVw.GetPatientDetailsFromReport("PID issuer:", isHistoryPanel: true);
                String Step29_DOB = StudyVw.GetPatientDetailsFromReport("DOB:", 2, true);
                String Step29_StudyDate = StudyVw.GetPatientDetailsFromReport("Study Date:", isHistoryPanel: true);
                if (Step29_Patient == PatName[5] && Step29_MRN == PatName[6] && Step29_PID == PatName[7] && Step29_DOB == PatName[8] && Step29_StudyDate == PatName[9])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30: Close the study viewer
                studies.CloseHistoryPanel();
                studies.CloseStudy();
                ExecutedSteps++;

                //Step-31: Repeat the above steps in all supported browsers: IE (9,10,11), Firefox and Chrome browsers
                result.steps[++ExecutedSteps].status = "Not Automated";


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
        /// Prior studies with cardio reports
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108533(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables       
            Studies studies;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionIDList.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String DefaultRole = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
                Random random = new Random();
                String USER1 = "UserOne" + random.Next(1, 100);
                String USER2 = "UserTwo" + random.Next(1, 100);
                String DestEA = login.GetHostName(Config.DestEAsIp);
                String EA131 = login.GetHostName(Config.EA1);

                //Setting Precondition
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                RoleManagement rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SearchRole(DefaultRole, "SuperAdminGroup");
                rolemanagement.SelectRole(DefaultRole);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("download", 0);
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                //Step-1:Login as Administrator and Load the recently imported prior study which has multiple cardio reports
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: DestEA);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer studyviewer = LaunchStudy();
                if (studyviewer.ViewStudy())
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-2:Click on the report icon on the top right corner of the study panel
                studyviewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyviewer.By_ReportContainer()));
                String Report_Patientname = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN = studyviewer.GetPatientDetailsFromReport("MRN:");
                PageLoadWait.WaitForFrameLoad(10);
                if (Report_Patientname.ToUpper().Contains(Firstname) && Report_Patientname.ToUpper().Contains(Lastname) && Report_MRN.Equals(PatientID) && studyviewer.ReportFullScreenIcon().Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-3:Click on the history fly out
                studyviewer.NavigateToHistoryPanel();
                if (studyviewer.StudylistInHistoryPanel().Count != 0 && studyviewer.PatientHistoryDrawer().Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-4:Verify that loaded study is selected by default and respective multiple cardio reports are listed on history panel report sub tab.
                bool StudyHighlight = studyviewer.Study(1).GetAttribute("class").Contains("hover");
                studyviewer.NavigateTabInHistoryPanel("Report");
                Dictionary<int, string[]> ReportList = studyviewer.StudyViewerListResults("patienthistory", "report");
                string[] DateValues = GetColumnValues(ReportList, "Date", GetColumnNames(1));
                DateTime[] Dates = studyviewer.ConvertStringToDate(DateValues);
                //Validate if date is in descending order
                bool step4 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (StudyHighlight && ReportList.Count > 1 && step4)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-5:Sort the Report list ascendingly based on report creation date.
                studyviewer.DateHeader_HPanel().Click();
                Dictionary<int, string[]> ReportList5 = studyviewer.StudyViewerListResults("patienthistory", "report");
                string[] DateValues5 = GetColumnValues(ReportList5, "Date", GetColumnNames(1));
                DateTime[] Dates5 = studyviewer.ConvertStringToDate(DateValues5);
                bool step5 = (Dates5 == null || Dates5.Length == 0) ? false : Dates5.SequenceEqual((Dates5.OrderBy(q => q)));
                if (step5)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-6:Select another report for the same study on history fly out report sub tab.
                String[] reportColumnNames = studyviewer.StudyViewerListColumnNames("patienthistory", "report", 1);
                String[] reportColumnValues = BasePage.GetColumnValues(ReportList, "Title", reportColumnNames);
                studyviewer.SelectItemInStudyViewerList("Title", reportColumnValues[1], "patienthistory", "report");
                String Report_Patientname6 = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN6 = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname6.ToUpper().Contains(Firstname) && Report_Patientname6.ToUpper().Contains(Lastname) && Report_MRN6.Equals(PatientID))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-7:From History panel, select the related study which has multiple reports
                studyviewer.ChooseColumns(new string[] { "Accession" });
                studyviewer.SelectStudy("Accession", Accessions[0]);
                Dictionary<int, string[]> ReportList7 = studyviewer.StudyViewerListResults("patienthistory", "report");
                if (ReportList7.Count > 1)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-8:From History panel, select the related study which has single cardio report.                
                studyviewer.SelectStudy("Accession", Accessions[1]);
                Dictionary<int, string[]> ReportList8 = studyviewer.StudyViewerListResults("patienthistory", "report");
                if (ReportList8.Count == 1)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-9:From History panel, select the related study which has no cardio report
                studyviewer.SelectStudy("Accession", Accessions[2]);
                bool IsReportTabEnabled = studyviewer.ReportTab().Enabled;
                if (!IsReportTabEnabled)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-10:From History panel, Double click on the related study that has single cardio report
                Dictionary<string, string> singlereportstudy = studyviewer.GetMatchingRow("Accession", Accessions[1]);
                studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[1] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo = studyviewer.StudyInfo(2);
                if (studyviewer.studyPanel(2).Displayed && singlereportstudy["Accession"].Equals(Studyinfo.Split(',')[0]))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-11:Click on the report icon on the top right corner of the second viewer study panel
                studyviewer.TitlebarReportIcon(2).Click();
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyviewer.By_ReportContainer()));
                String Report_Patientname11 = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN11 = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname11.ToUpper().Contains(Firstname) && Report_Patientname11.ToUpper().Contains(Lastname) && Report_MRN11.Equals(PatientID))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-12:From History panel, Double click on the related study that has no cardio report
                studyviewer.NavigateToHistoryPanel();
                Dictionary<string, string> noreportstudy = studyviewer.GetMatchingRow("Accession", Accessions[2]);
                studyviewer.OpenPriors(new string[] { "Accession" }, new string[] { Accessions[2] });
                PageLoadWait.WaitForFrameLoad(20);
                String Studyinfo12 = studyviewer.StudyInfo(3);
                if (studyviewer.studyPanel(3).Displayed && noreportstudy["Accession"].Equals(Studyinfo12.Split(',')[0]))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-13:Ensure that report icon on the top right corner of the study panel is available
                bool IsReportIconPresent = studyviewer.IsElementVisible(studyviewer.By_TitlebarReportIcon(3));
                if (!IsReportIconPresent)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-14:Close the study viewer
                studyviewer.CloseStudy();
                Executedsteps++;

                //Step-15 to 23:From studies tab, Select a study that has a cardio report and click on transfer button
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: DestEA);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accessions[0]);
                studies.ChooseColumns(new string[]{"Last Name"});
                String lastname;
                studies.GetMatchingRow("Accession", Accessions[0]).TryGetValue("Lastname", out lastname);
                studies.TransferStudy("Local System", Accessions[0]);
                PageLoadWait.WaitForDownload("_" + lastname, Config.downloadpath, "zip");

                //Check whether the file is present
                Boolean studydownloaded = BasePage.CheckFile("_" + lastname, Config.downloadpath, "zip");

                //Validate the study is downloaded - step 20
                if (studydownloaded == true)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;

                //Step-24 & 25:Unzip the downloaded folder study and verify that all the images and report are available
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";

                //Step-26 to 32:From studies tab, Select a study that has a cardio report and click on transfer button
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: DestEA);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accessions[1]);                            
                studies.TransferStudy(EA131, Accessions[1]);               
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;

                //Step-33:Select only the Datasource to which the study was transferred in studies tab
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: EA131);
                Dictionary<string, string> IsStudyList = studies.GetMatchingRow("Accession", Accessions[1]);
                if (IsStudyList != null)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-34:Select the study which was transferred and load it in the study viewer
                studies.SelectStudy("Accession", Accessions[1]);
                studyviewer = studies.LaunchStudy();
                if (studyviewer.ViewStudy())
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-35:Click on the report icon on the top right corner of the study panel
                studyviewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyviewer.By_ReportContainer()));
                String Report_Patientname35 = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN35 = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname35.ToUpper().Contains(Firstname) && Report_Patientname35.ToUpper().Contains(Lastname) && Report_MRN35.Equals(PatientID))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-36:Close the study viewer
                studyviewer.CloseStudy();
                Executedsteps++;

                //Step-37:Create two users from ‘User management tab’ like user1 and User2
                UserManagement usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(USER1, DefaultDomain, DefaultRole);
                bool User1 = usermanagement.SearchUser(USER1, DefaultDomain);
                usermanagement.CreateUser(USER2, DefaultDomain, DefaultRole);
                bool User2 = usermanagement.SearchUser(USER2, DefaultDomain);
                if (User1 && User2)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }
                login.Logout();

                //Step-38:Login as User1 to iCA
                login.LoginIConnect(USER1, USER1);
                Executedsteps++;

                //Step-39 to 42:Select a study that has a cardio report and click on Grant Access
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: DestEA);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accessions[1]);
                studies.ShareStudy(false, new string[] { USER2 });
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;
                Executedsteps++;

                //Step-43:Navigate to Outbounds page and check that the study is listed
                Outbounds outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy(AccessionNo: Accessions[1]);
                Dictionary<string, string> IsStudyList43 = studies.GetMatchingRow("Accession", Accessions[1]);
                if (IsStudyList43 != null)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-44:Click on view button
                outbounds.SelectStudy("Accession", Accessions[1]);
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.ViewStudy())
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-45:Click on the report icon on the top right corner of the study panel
                studyviewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyviewer.By_ReportContainer()));
                String Report_Patientname45 = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN45 = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname45.ToUpper().Contains(Firstname) && Report_Patientname45.ToUpper().Contains(Lastname) && Report_MRN45.Equals(PatientID))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-46:Close the study viewer
                studyviewer.CloseStudy();
                Executedsteps++;

                //Step-47:Logout user1 from iCA
                login.Logout();
                Executedsteps++;

                //Step-48:Login as user2 and navigate to inbounds tab
                login.LoginIConnect(USER2, USER2);
                Inbounds inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy(AccessionNo: Accessions[1]);
                Dictionary<string, string> IsStudyList48 = studies.GetMatchingRow("Accession", Accessions[1]);
                if (IsStudyList48 != null)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-49:Click on view button
                studies.ChooseColumns(new string[] { "Accession" });
                inbounds.SelectStudy("Accession", Accessions[1]);
                studyviewer = StudyViewer.LaunchStudy();
                if (studyviewer.ViewStudy())
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-50:Click on the report icon on the top right corner of the study panel
                studyviewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyviewer.By_ReportContainer()));
                String Report_Patientname50 = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN50 = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname50.ToUpper().Contains(Firstname) && Report_Patientname50.ToUpper().Contains(Lastname) && Report_MRN50.Equals(PatientID))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-51:Close the study viewer
                studyviewer.CloseStudy();
                login.Logout();
                Executedsteps++;

                //Report Result
                result.FinalResult(Executedsteps);
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
                result.FinalResult(e, Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Flyout Reports - Cardio Report Title and Report Number
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108534(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Studies studies;
            StudyViewer studyViewer;
            Viewer viewer = null;
            viewer = new Viewer();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientID = PatientIDList.Split(':');
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String PatientNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String[] PatientName = PatientNameList.Split(':');

                //Step-1:
                login.LoginIConnect(adminusername, adminpassword);
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step1 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step1)
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

                //Step-2
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.ReportView();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_studyPanels_m_studyPanel_1_m_reportViewer_reportFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ViewerDisplay")));

                if (BasePage.Driver.FindElement(By.Id("ViewerDisplay")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3
                BasePage.Driver.FindElement(By.Id("ViewerDisplay")).Click();
                if (BasePage.Driver.FindElement(By.Id("ViewerDisplay")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                String title = viewer.CardioReportTitle().Text;
                if (title != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step6 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step6)
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

                //Step-7
                login.NavigateToHistoryPanel();
                ExecutedSteps++;

                //Step-8
                bool step8_1 = PageLoadWait.WaitForElement(studyViewer.ReportListTable(), WaitTypes.Visible, 20).Displayed;
                String Step8_Patient = studyViewer.GetPatientDetailsFromReport("Patient:", 2, true);
                bool step8_2 = false;
                if (Step8_Patient.ToLower().EndsWith(PatientName[0].ToLower()))
                {
                    step8_2 = true;
                }
                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                String[] TitleValue = GetColumnValues(StudyViewer.GetCardioReportResults(), "Title", GetColumnNames(1));

                if (TitleValue[0].Equals(title.Split(':')[3].Trim()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10
                bool step10_1 = PageLoadWait.WaitForElement(studyViewer.ReportListTable(), WaitTypes.Visible, 20).Displayed;
                String Step10_Patient = studyViewer.GetPatientDetailsFromReport("Patient:", 2, true);
                bool step10_2 = false;
                if (Step10_Patient.ToLower().EndsWith(PatientName[0].ToLower()))
                {
                    step10_2 = true;
                }
                if (step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11
                studyViewer.CloseStudy();
                ExecutedSteps++;

                //Step-12
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Accession", Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step12 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step1)
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

                //Step-13
                login.NavigateToHistoryPanel();
                ExecutedSteps++;

                //Step-14
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (!studyViewer.ReportTab().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15
                studyViewer.CloseStudy();
                ExecutedSteps++;

                //Step-16 - Import a study with two cardio reports. - NA since study would be imported beforehand in EA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-17
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                //studies.ClearFields();
                studies.SearchStudy("last", PatientName[1]);
                studies.SelectStudy("Patient ID", PatientID[3]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step17 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step17)
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

                //Step-18
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.ReportView();

                if (studyViewer.ViewerReportListButton().Displayed)
                    studyViewer.ViewerReportListButton().Click();

                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                string[] DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                DateTime[] Dates = studyViewer.ConvertStringToDate(DateValues);
                bool step18 = Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step18)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19
                login.NavigateToHistoryPanel();
                studyViewer.ReportTab().Click();
                PageLoadWait.WaitForElement(studyViewer.ReportListTable(), WaitTypes.Visible, 20);
                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = studyViewer.ConvertStringToDate(DateValues);
                bool step19 =Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
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

                //Step-20
                bool step20 = BasePage.Driver.FindElement(By.XPath("//table[@id='m_patientHistory_m_reportViewer_reportList']/tbody/tr[2]")).GetAttribute("class").Contains("ui-state-highlight");
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

                //Step-21
                studyViewer.CloseStudy();
                ExecutedSteps++;

                //Step-22 - Import a study with two cardio reports. - NA since study would be imported beforehand in EA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-23
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("last", PatientName[2]);
                studies.SelectStudy("Patient ID", PatientID[1]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step23 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step23)
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

                //Step-24
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.ReportView();

                if (studyViewer.ViewerReportListButton().Displayed)
                    studyViewer.ViewerReportListButton().Click();

                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = studyViewer.ConvertStringToDate(DateValues);
                bool step24 =Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25
                login.NavigateToHistoryPanel();
                studyViewer.ReportTab().Click();
                PageLoadWait.WaitForElement(studyViewer.ReportListTable(), WaitTypes.Visible, 20);
                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = studyViewer.ConvertStringToDate(DateValues);
                bool step25 =Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step25)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26
                bool step26 = BasePage.Driver.FindElement(By.XPath("//table[@id='m_patientHistory_m_reportViewer_reportList']/tbody/tr[2]")).GetAttribute("class").Contains("ui-state-highlight");
                if (step26)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-27
                studyViewer.CloseStudy();
                ExecutedSteps++;

                //Step-28 - Import a study with three cardio reports. - NA since study would be imported beforehand in EA
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-29
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("last", PatientName[3]);
                studies.SelectStudy("Patient ID", PatientID[2]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step29 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step29)
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

                //Step-30
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.ReportView();

                if (studyViewer.ViewerReportListButton().Displayed)
                    studyViewer.ViewerReportListButton().Click();

                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = studyViewer.ConvertStringToDate(DateValues);
                bool step30 =Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step30)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31
                login.NavigateToHistoryPanel();
                studyViewer.ReportTab().Click();
                PageLoadWait.WaitForElement(studyViewer.ReportListTable(), WaitTypes.Visible, 20);
                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = studyViewer.ConvertStringToDate(DateValues);
                bool step31 =Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step31)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32
                bool step32 = BasePage.Driver.FindElement(By.XPath("//table[@id='m_patientHistory_m_reportViewer_reportList']/tbody/tr[2]")).GetAttribute("class").Contains("ui-state-highlight");
                if (step32)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33
                studyViewer.CloseStudy();
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
        /// Flyout Reports - Cardio Reports with Image Sharing
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108535(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            Studies studies = null;
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            Inbounds inbounds = null;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String phUsername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String arusername = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Paths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPath");
                String FileName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FileName");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");
                String[] lastName = LastName.Split(':');
                String[] PatName = PatientName.Split(':');
                String[] patientID = PatientID.Split(':');
                String[] StudyPath = Paths.Split('=');
                String[] Accession = AccessionIDList.Split(':');
                string FolderPath = Config.downloadpath;
                String Comments = "Test comments for Upload Web Upload";
                String currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));


                //Step-1: Upload a study with Cardio reports to iCA via Web uploader
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("firefox");
                login.DriverGoTo(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.WebUploadBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.WebUploadBtn());

                try
                {
                    //Choose domain if multiple domain exists
                    //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));

                    SelectElement selector = new SelectElement(login.DomainNameDropdown());
                    selector.SelectByText(DomainName);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());

                }
                catch (WebDriverTimeoutException e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }
                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("LoginUserName"));
                //Login as anonymous user
                webuploader.UserNameTxt().TextValue = Config.stUserName;
                webuploader.PasswordTxt().TextValue = Config.stPassword;
                rnxobject.WaitForElementTobeEnabled(webuploader.SignInBtn());
                rnxobject.Click(webuploader.SignInBtn());
                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("ToDestination"));

                webuploader.PriorityBox().Click();
                //Select Destination
                webuploader.SelectDestination(Config.Dest1);

                //Set Priority
                webuploader.SelectPriority("ROUTINE");

                //Select study in the specified location
                webuploader.SelectFileFromHdd(StudyPath[0]);

                //Enter Comments
                webuploader.CommentsTxtbox().TextValue = Comments;
                //Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                Ranorex.Mouse.ScrollWheel(-20.0);
                webuploader.SendBtn().EnsureVisible();
                rnxobject.Click(webuploader.SendBtn());
                ExecutedSteps++;

                //Close Web Uploader
                webuploader.CloseUploader();

                //Close Firefox and resume test as normal
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);

                //Step-2: Login as PH and check inbounds
                login.LoginIConnect(phUsername, phpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Patient ID", patientID[0]);
                Dictionary<string, string> studystatus2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { patientID[0], "Uploaded" });
                if (studystatus2 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-3: Open Study in viewer
                inbounds.SelectStudy("Patient ID", patientID[0]);
                inbounds.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3 = inbounds.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step3)
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

                //Step-4: Click on the report icon on the top right corner of the study panel
                StudyVw.ReportView();
                bool step4_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step4_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step4_2 = false;
                if (Step4_Patient == PatName[0])
                {
                    step4_2 = true;
                }
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

                //Step-5: Verify Patient name, MRN, IPID, DOB and Study Date are displayed on the report as per loaded study 
                String Step5_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                String Step5_MRN = StudyVw.GetPatientDetailsFromReport("MRN:");
                String Step5_PID = StudyVw.GetPatientDetailsFromReport("PID issuer:");
                String Step5_DOB = StudyVw.GetPatientDetailsFromReport("DOB:", 2);
                String Step5_StudyDate = StudyVw.GetPatientDetailsFromReport("Study Date:");
                if (Step5_Patient == PatName[0] && Step5_MRN == PatName[1] && Step5_PID == PatName[2] && Step5_DOB == PatName[3] && Step5_StudyDate == PatName[4])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: CLose study
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.CloseStudy();
                ExecutedSteps++;
                login.Logout();

                //Step-7: Install EI on local machine
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-8: Open the Exam Importer and upload a study with cardio reports
                ei.EIDicomUpload(phUsername, phpassword, Config.Dest1, StudyPath[1]);
                ExecutedSteps++;

                //Step-9: Login as PH user and check inbounds tab
                login.LoginIConnect(phUsername, phpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Patient ID", patientID[1]);
                Dictionary<string, string> studystatus9 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { patientID[1], "Uploaded" });
                if (studystatus9 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-10: Open Study in viewer
                inbounds.SelectStudy("Patient ID", patientID[1]);
                inbounds.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step10 = inbounds.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step10)
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

                //Step-11: Load report
                StudyVw.ReportView();
                bool step11_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step11_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step11_2 = false;
                if (Step11_Patient == PatName[5])
                {
                    step11_2 = true;
                }
                if (step11_1 && step11_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Click on the History fly out
                inbounds.NavigateToHistoryPanel();
                //studies.ChooseColumns(new string[] { "Accession" });
                ExecutedSteps++;

                //Step-13: Check that Reports are sorted descendingly based on the study date
                StudyVw.ReportTab().Click();
                PageLoadWait.WaitForElement(StudyVw.ReportListTable(), WaitTypes.Visible, 20);
                //Get Date Column from Cardio Report Table
                string[] DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                DateTime[] Dates = StudyVw.ConvertStringToDate(DateValues);
                //Validate if date is in descending order
                bool step13 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
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

                //Step-14: Close the study viewer
                inbounds.CloseHistoryPanel();
                inbounds.CloseStudy();
                login.Logout();
                ExecutedSteps++;

                //Step-15: Install Pacs Gateway
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-16: Upload a study via POP
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath[2] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                //Send the study to dicom devices from MergePacs management page
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Patient Name", LastName, 0);
                tools.MpacSelectStudy("Accession", Accession[0]);
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Step-17: Login as Ph and check inbounds
                login.LoginIConnect(phUsername, phpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Patient ID", patientID[2]);
                Dictionary<string, string> studystatus17 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { patientID[2], "Uploaded" });
                if (studystatus17 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-18: Open Study in viewer
                inbounds.SelectStudy("Patient ID", patientID[2]);
                inbounds.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18 = inbounds.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step18)
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

                //Step-19: Load report
                StudyVw.ReportView();
                bool step19_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step19_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step19_2 = false;
                if (Step19_Patient == PatName[10])
                {
                    step19_2 = true;
                }
                if (step19_1 && step19_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20: Close the study viewer
                inbounds.CloseStudy();
                ExecutedSteps++;

                //Step-21: Select any uploaded study and Click on nominate for archive
                inbounds.SelectStudy("Patient ID", patientID[2]);
                inbounds.NominateForArchive(Reason);
                ExecutedSteps++;
                login.Logout();

                //Step-22: Login as AR user and navigate to inbounds tab
                login.LoginIConnect(arusername, arpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Step-23: Check that the nominated study is listed under inbounds tab
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Patient ID", patientID[2]);
                Dictionary<string, string> Studylist_1 = inbounds.GetMatchingRow("Patient ID", patientID[2]);
                if (Studylist_1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-24: Check the status of the study
                bool step24 = Studylist_1.Values.Contains("Nominated For Archive");
                if (step24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25: Load the study to viewer
                inbounds.SelectStudy1(new string[] { "Patient ID", "Status" }, new string[] { patientID[2], "Nominated For Archive" });
                inbounds.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step25 = inbounds.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step25)
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

                //Step-26: Load report
                StudyVw.ReportView();
                bool step26_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step26_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step26_2 = false;
                if (Step26_Patient == PatName[10])
                {
                    step26_2 = true;
                }
                if (step26_1 && step26_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-27: Close the study viewer
                inbounds.CloseStudy();
                ExecutedSteps++;

                //Step-28: Archive the study
                inbounds.SelectStudy1(new string[] { "Patient ID", "Status" }, new string[] { patientID[2], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                ExecutedSteps++;
                login.Logout();

                //Step-29: Check study status
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Patient ID", patientID[2]);
                Dictionary<string, string> archivedstudy = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { patientID[2], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-30: Login as Administrator and load the study that is archived from studies tab.
                login.LoginIConnect(username, password);
                //Search and load study
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy("Patient ID", patientID[2]);
                studies.SelectStudy("Patient ID", patientID[2]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step30 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step30)
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

                //Step-31: Load report
                StudyVw.ReportView();
                bool step30_1 = PageLoadWait.WaitForElement(StudyVw.ReportViewerContainer(), WaitTypes.Visible, 15).Displayed;
                String Step30_Patient = StudyVw.GetPatientDetailsFromReport("Patient:", 2);
                bool step30_2 = false;
                if (Step30_Patient == PatName[10])
                {
                    step30_2 = true;
                }
                if (step30_1 && step30_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32: Click on inbuilt download option
                //Delete document if already exists
                var dir = new DirectoryInfo(FolderPath);
                foreach (var file in dir.EnumerateFiles(FileName.Split('.')[0] + "*." + FileName.Split('.')[1]))
                {
                    file.Delete();
                }
                StudyVw.Download_Report().Click();
                PageLoadWait.WaitForDownload(FileName.Split('.')[0], FolderPath, FileName.Split('.')[1]);
                // Check if downloaded
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + FileName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33: Print report
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-34: Close the study viewer
                studies.CloseStudy();
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
        /// Cardio reports -Ipad/TestEHR/Email
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108536(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables       
            Studies studies;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;
            String pinnumber = "";

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionIDList.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String Reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Reason");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String DestEA = login.GetHostName(Config.DestEAsIp);

                //Step-1 to 12:iPad related steps
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";

                //Step-13:Configure Email on iCA server
                Executedsteps++;

                //Step-14:Login as administrator and navigate to studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //PreCondition :- Enable Email Study for SuperRole User
                RoleManagement rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveEditRole();

                studies = login.Navigate<Studies>();
                Executedsteps++;


                //Step-15:Load a study with a cardio report in the study viewer and Click on Email button to mail the study
                studies.SearchStudy(AccessionNo: Accessions[0], Datasource: DestEA);
                studies.SelectStudy("Accession", Accessions[0]);
                StudyViewer studyviewer = LaunchStudy();
                studyviewer.EmailStudy(Email, "Automation", Reason, 1);
                pinnumber = studyviewer.FetchPin();
                Executedsteps++;

                //Step-16 to 18:Link from the email
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                result.steps[++Executedsteps].status = "Not Automated";
                studyviewer.CloseStudy();
                login.Logout();

                //Step-19:Configure TestEHR on iCA server
                Executedsteps++;

                //Step-20:Launch TestEHR application and set report -True
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True");
                Executedsteps++;

                //Step-21:From TestEHR, load a study with cardio report by searching Accession number ,patient first name ,last name and click on cmd button
                ehr.SetMultipleSearchKeys_Patient(new String[] { "firstname", "lastname" }, new String[] { Firstname, Lastname });
                ehr.SetSearchKeys_Study(Accessions[1]);
                String url_21 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                Executedsteps++;

                //Step-22:Copy the url and paste the url in a browser                
                //Navigate to url generated in test eHR
                login = new Login();
                login.NavigateToIntegratorURL(url_21);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname.ToLower()) &&
                   studyviewer.PatientDetailsInViewer()["FirstName"].ToLower().Equals(Firstname.ToLower()) &&
                   studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[1].ToLower()))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-23:Click on the report icon on the top right corner of the study panel
                studyviewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyviewer.By_ReportContainer()));
                String Report_Patientname = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname.ToUpper().Contains(Firstname) && Report_Patientname.ToUpper().Contains(Lastname) && Report_MRN.Equals(PatientID))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-24:Close the study viewer                               
                Executedsteps++;

                //Step-25:From TestEHR, Load a study that has multiple cardio reports
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                ehr.SetSelectorOptions(showReport: "True");
                ehr.SetMultipleSearchKeys_Patient(new String[] { "firstname", "lastname" }, new String[] { Firstname, Lastname });
                ehr.SetSearchKeys_Study(Accessions[0]);
                String url_25 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login = new Login();
                login.NavigateToIntegratorURL(url_25);
                studyviewer = (StudyViewer)login.NavigateToIntegratorFrame();
                if (studyviewer.PatientDetailsInViewer()["LastName"].ToLower().Equals(Lastname.ToLower()) &&
                   studyviewer.PatientDetailsInViewer()["FirstName"].ToLower().Equals(Firstname.ToLower()) &&
                   studyviewer.StudyDetailsInViewer()["Accession"].ToLower().Equals(Accessions[0].ToLower()))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-26:Click on the report icon on the top right corner of the study panel
                studyviewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyviewer.By_ReportContainer()));
                bool ReportExist = studyviewer.IsElementVisible(studyviewer.By_ReportContainer());
                bool ReportMaxIconExist = studyviewer.ReportFullScreenIcon().Displayed;
                studyviewer.ViewerReportListButton().Click();
                Dictionary<int, string[]> ReportListDetails = studyviewer.StudyViewerListResults("StudyPanel", "report", 1);

                if (ReportExist && ReportListDetails.Count > 1 && ReportMaxIconExist)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-27:Check that Reports are sorted descendingly based on the report creation date                
                //Get Date Column from Cardio Report Table
                string[] DateValues = GetColumnValues(ReportListDetails, "Date", GetColumnNames(1));
                DateTime[] Dates = studyviewer.ConvertStringToDate(DateValues);
                //Validate if date is in descending order
                bool step27 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));
                if (step27)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-28:Select any Report listed in the report panel
                //Get column names and row details of first report in the report list
                String[] reportColumnNames = studyviewer.StudyViewerListColumnNames("StudyPanel", "report", 1);
                String[] reportColumnValues = BasePage.GetColumnValues(ReportListDetails, "Title", reportColumnNames);
                Dictionary<string, string> FirstreportDetails = studyviewer.StudyViewerListMatchingRow("Title", reportColumnValues[0], "StudyPanel", "report");

                //Select the first report in report list
                studyviewer.SelectItemInStudyViewerList("Title", reportColumnValues[0], "StudyPanel", "report");
                String Report_Patientname28 = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN28 = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname28.ToUpper().Contains(Firstname) && Report_Patientname28.ToUpper().Contains(Lastname))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }


                //Step-29:Click on the history fly out
                studyviewer.NavigateToHistoryPanel();
                if (studyviewer.StudylistInHistoryPanel().Count != 0 && studyviewer.PatientHistoryDrawer().Displayed)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-30:Verify that loaded study is selected by default and respective multiple cardio reports are listed on history panel report sub tab
                bool StudyHighlight = studyviewer.Study().GetAttribute("class").Contains("hover");
                studyviewer.NavigateTabInHistoryPanel("Report");
                Dictionary<int, string[]> ReportList = studyviewer.StudyViewerListResults("patienthistory", "report");
                if (StudyHighlight && ReportList.Count > 1)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-31:Select another report for the same study on history fly out report sub tab                 
                studyviewer.SelectItemInStudyViewerList("Title", reportColumnValues[1], "patienthistory", "report");
                String Report_Patientname31 = studyviewer.GetPatientDetailsFromReport("Patient:", 2);
                String Report_MRN31 = studyviewer.GetPatientDetailsFromReport("MRN:");
                if (Report_Patientname31.ToUpper().Contains(Firstname) && Report_Patientname31.ToUpper().Contains(Lastname) && Report_MRN31.Equals(PatientID))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-32:Click on the print button
                result.steps[++Executedsteps].status = "Not Automated";

                //Step-33:Close the study viewer
                studyviewer.CloseHistoryPanel();
                Executedsteps++;


                //Report Result
                result.FinalResult(Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Flyout Reports - Cardio Report option is disabled
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_108537(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Studies studies;
            StudyViewer studyViewer;
            Viewer viewer = null;
            viewer = new Viewer();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientID = PatientIDList.Split(':');
                //String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String PatientNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String[] PatientName = PatientNameList.Split(':');
                String datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");

                //Step-1
                taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                //servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked = false;
                //bool step1 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked;
                //Check Encapsulated PDF
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = true;
                bool step1_1 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked;
                //Check Cardio reports
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked = true;
                bool step1_2 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked;
                //Check Other reports
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.OtherReports, 1).Checked = true;
                bool step1_3 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.OtherReports, 1).Checked;

                if (step1_1 && step1_2 && step1_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                ExecutedSteps++;

                //Step-3
                login.LoginIConnect(adminusername, adminpassword);
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Accession", Accession[0]);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step3 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step3)
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

                //Step-4
                viewer.ReportView();
                if (studyViewer.TitlebarReportIcon().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                //studies.ClearFields();
                //studies.SearchStudy("last", PatientName);
                studies.SearchStudy(LastName : PatientName[0],Datasource : datasource);
                studies.ChooseColumns(new string[] { "Patient ID" });
                studies.SelectStudy("Patient ID", PatientID[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step6 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step6)
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

                //Step-7
                viewer.ReportView();

                if (studyViewer.ViewerReportListButton().Displayed)
                    studyViewer.ViewerReportListButton().Click();

                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);

                String[] TitleValue = GetColumnValues(StudyViewer.GetCardioReportResults(), "Title", GetColumnNames(1));
                //bool step7 = true;
                //for (int i = 0; i < TitleValue.Count(); )
                //{
                //    int b = TitleValue.Count();
                //    if (TitleValue[i].EndsWith("pdf")) i++;
                //    else if (TitleValue[i].EndsWith("Report")) i++;
                //    else step7 = false;
                //}
                bool step7_1 = false;
                bool step7_2 = false;
                for (int i = 0; i < TitleValue.Count(); i++)
                {
                    if (TitleValue[i].EndsWith(".pdf")) step7_1 = true;
                    //else if (TitleValue[i].EndsWith("Report")) step7 = true;
                    else if (TitleValue[i].EndsWith("log")) step7_2 = true;

                }
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
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

                //Step-9
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                //Uncheck Encapsulated PDF
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = false;
                bool step9 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked;
                if (!step9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                ExecutedSteps++;

                //Step-11
                login.LoginIConnect(adminusername, adminpassword);
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Accession", Accession[0]);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step11 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step11)
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

                //Step-12
                try
                {
                    viewer.ReportView();
                }
                catch (Exception ex) { }
                if (!(studyViewer.TitlebarReportIcon().Displayed))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                //studies.SearchStudy("last", PatientName);
                //studies.SelectStudy("Patient ID", PatientID);
                studies.SearchStudy(LastName: PatientName[0], Datasource: datasource);
                studies.ChooseColumns(new string[] { "Patient ID" });
                studies.SelectStudy("Patient ID", PatientID[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step14 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step14)
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

                //Step-15
                try
                {
                    viewer.ReportView();
                }
                catch (Exception ex) { }
                if (!(studyViewer.TitlebarReportIcon().Displayed))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
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

                //Step-17
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                ////check Encapsulated PDF
                //servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = true;
                //servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked = true;
                //bool step17_1 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked;
                //bool step17_2 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked;

                //if (step17_1 && step17_2)
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
                //Check Encapsulated PDF
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = true;
                bool step17_1 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked;
                //Check Cardio reports
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked = true;
                bool step17_2 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked;
                //Check Other reports
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.OtherReports, 1).Checked = true;
                bool step17_3 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.OtherReports, 1).Checked;

                if (step17_1 && step17_2 && step17_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                ExecutedSteps++;

                //Step-19
                login.LoginIConnect(adminusername, adminpassword);
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                //studies.SearchStudy("last", PatientName);
                studies.SearchStudy(LastName: PatientName[0], Datasource: datasource);
                studies.ChooseColumns(new string[] { "Patient ID" });
                studies.SelectStudy("Patient ID", PatientID[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step19 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step19)
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

                //Step-20
                viewer.ReportView();

                if (studyViewer.ViewerReportListButton().Displayed)
                    studyViewer.ViewerReportListButton().Click();

                PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                string[] values = GetColumnValues(StudyViewer.GetCardioReportResults(), "Type", GetColumnNames(1));
                if (values != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21
                string[] DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                DateTime[] Dates = studyViewer.ConvertStringToDate(DateValues);
                if (Dates.SequenceEqual(Dates.OrderByDescending(q => q)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22
                studyViewer.OpenReport("Date", DateValues[0]);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_studyPanels_m_studyPanel_1_m_reportViewer_reportFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ViewerDisplay")));

                if (BasePage.Driver.FindElement(By.Id("ViewerDisplay")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23
                Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("#Pdf_Display_Div>iframe")));
                var dir = new DirectoryInfo(FolderPath);
                foreach (var file in dir.EnumerateFiles("document*.pdf"))
                {
                    file.Delete();
                }
                studyViewer.Download_Report().Click();
                PageLoadWait.WaitForDownload("document", FolderPath, "pdf");
                // Check if downloaded
                if (File.Exists(FolderPath + Path.DirectorySeparatorChar + "document.pdf"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24
                login.NavigateToHistoryPanel();
                if (studyViewer.StudylistInHistoryPanel().Count != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25
                studyViewer.ReportTab().Click();
                PageLoadWait.WaitForElement(studyViewer.ReportListTable(), WaitTypes.Visible, 20);
                DateValues = GetColumnValues(StudyViewer.GetCardioReportResults(), "Date", GetColumnNames(1));
                Dates = studyViewer.ConvertStringToDate(DateValues);
                if (Dates.SequenceEqual(Dates.OrderByDescending(q => q)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26
                studyViewer.CloseHistoryPanel();
                studyViewer.CloseStudy();
                //ExecutedSteps++;
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 27
                studies = (Studies)login.Navigate("Studies");
                studies.SelectAllDateAndData();
                studies.SearchStudy("patientID", PatientID[1]);
                studies.ChooseColumns(new string[] { "Patient ID" });
                studies.SelectStudy1("Patient ID", PatientID[1]);
                studyViewer = studies.LaunchStudy();
                if (studies.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 28
                viewer.ReportView();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_reportViewerContainer']")));
                PageLoadWait.WaitForFrameLoad(10);
                //if (studyViewer.ViewerReportListButton().Displayed)
                //    studyViewer.ViewerReportListButton().Click();

                //PageLoadWait.WaitForElement(By.CssSelector("table[id$='1_m_reportViewer_reportList']"), BasePage.WaitTypes.Visible, 20);
                //values = GetColumnValues(StudyViewer.GetCardioReportResults(), "Type", GetColumnNames(1));
                if (studyViewer.ReportContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 29
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
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

                //step 30
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                //Uncheck Encapsulated PDF
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = false;
                bool step30 = servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked;
                if (!step30)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                ExecutedSteps++;

                //step 32
                login.LoginIConnect(adminusername, adminpassword);
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(LastName: PatientName[1]);
                studies.ChooseColumns(new string[] { "Patient ID" });
                studies.SelectStudy("Patient ID", PatientID[2]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step32 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step32)
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

                //step 33
                try
                {
                    viewer.ReportView();
                }
                catch (Exception ex) { }
                if (!(studyViewer.TitlebarReportIcon().Displayed))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 34
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 35
                //login.LoginIConnect(adminusername, adminpassword);
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Accession", Accession[0]);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                bool step35 = login.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step35)
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

                //step 36
                try
                {
                    viewer.ReportView();
                }
                catch (Exception ex) { }
                if (!(studyViewer.TitlebarReportIcon().Displayed))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 37
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studylist = BasePage.Driver.FindElement(By.CssSelector("#gview_gridTableStudyList"));
                if (studylist.Displayed == true)
                {
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
