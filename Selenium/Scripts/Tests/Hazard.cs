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
using Dicom;
using Dicom.Network;

namespace Selenium.Scripts.Tests
{
    class Hazard
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
        public BasePage basepage { get; set; }
        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Hazard(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ehr = new EHR();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
            mpaclogin = new MpacLogin();
            ei = new ExamImporter();
            basepage = new BasePage();
        }

        /// <summary>
        /// Load and verify the patient records - XDS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_106670(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            int resultcount = 0;
            result.SetTestStepDescription(teststeps);
            SystemSettings systemsettings = null;
            Studies studies = null;
            Patients patients = null;
            UserPreferences userpreferences = new UserPreferences();
            StudyViewer viewer = new StudyViewer();
            string[] PatientName = null;
            string[] ID = null;
            string[] Accession = null;
            string[] names = null;
            string[] ExpectedColumnNames = null;
            string[] ExpectedTabNames = null;
            string[] ActualTabNames = null;
            string[] ActualColumnValues = null;
            try
            {
                ID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "ID")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                ExpectedColumnNames = new string[] { "ID", "Date/Time", "Description", "Accession#", "Modalities", "Data Source" };
                ExpectedTabNames = new string[] { "Studies", "Xds", "Other Documents" };
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                //PreCondition
                servicetool.LaunchServiceTool();
                servicetool.OtherDocumentsTabInPMJ();
                servicetool.CloseServiceTool();
                //Step 1: Go to System setting page and enable XDS
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                systemsettings = (SystemSettings)login.Navigate("SystemSettings");
                systemsettings.SetDateRange();
                if (systemsettings.ShowXDS().Selected)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2: Enabling Live search from the User preferences. Enable free text option in MPI tab
                userpreferences.OpenUserPreferences();
                userpreferences.SwitchToUserPrefFrame();
                userpreferences.SetCheckbox(userpreferences.PatientRecordLiveSearch());
                userpreferences.CloseUserPreferences();
                login.Logout();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.EnableMergeEMPI(Searchtype: "freetextsearch");
                servicetool.CloseServiceTool();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences.OpenUserPreferences();
                userpreferences.SwitchToUserPrefFrame();
                bool livesearchbox = userpreferences.PatientRecordLiveSearch().Selected;
                userpreferences.CloseUserPreferences();
                if (livesearchbox)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3: Type"kir"
                patients = (Patients)login.Navigate("Patients");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patients.SendKeysInStroke(patients.PatientSearch(), PatientName[0].Split(',')[1].Trim());
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                names = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Name", BasePage.GetColumnNames());
                if (names.All(name => name.ToLowerInvariant().Contains(PatientName[0].Split(',')[1].Trim().ToLowerInvariant())))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4: Select the patient and double click the item
                Dictionary<string, string> PatientDetails = patients.GetMatchingRow("Name", PatientName[0]);
                patients.LoadStudyInPatientRecord(PatientName[0]);
                patients.NavigateToXdsStudies();
                resultcount = 0;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (patients.PatientName().Displayed && patients.PatientDOBInfo().Displayed && patients.PatientGender().Displayed && patients.PatientAddress1().Displayed)
                {
                    resultcount++;
                }
                ActualTabNames = patients.PatientRecordTabs().Select(rec => rec.Text).ToArray();
                if (ExpectedTabNames.All(ex => ActualTabNames.Contains(ex)) && patients.XDSDate().Displayed && string.Equals(patients.XDSDate().Text, "All Dates"))
                {
                    resultcount++;
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                DataTable table = patients.CollectRecordsInTable(patients.XDSStudiesPatientTable(), patients.XDSStudiesPatientheader(), patients.XDSStudiesPatientrow(), patients.XDSStudiesPatientcolumn());
                IList<string> ActualColumnNames = new List<string>();
                foreach (DataColumn column in table.Columns)
                {
                    ActualColumnNames.Add(column.ColumnName);
                }
                ActualColumnValues = basepage.GetColumnValues(table, "ID");
                if (ExpectedColumnNames.All(ex => ActualColumnNames.Contains(ex)) && ID.All(ex => ActualColumnValues.Contains(ex)))
                {
                    resultcount++;
                }
                if (resultcount == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: Check the info from left panel against MPI
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(patients.PatientName().Text.Replace(",", ", "), PatientDetails["Name"]) && string.Equals(patients.PatientDOBInfo().Text, PatientDetails["Date of birth"]) && PatientDetails["Address"].Contains(patients.PatientAddress1().Text) && PatientDetails["Address"].Contains(patients.PatientCity().Text) && PatientDetails["Address"].Contains(patients.PatientState().Text))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6: Select the patient that has records associated
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IWebElement element = BasePage.Driver.FindElements(By.CssSelector("img[src^='Images/PlusSign']"))[0];
                basepage.ClickElement(element);
                IList<IWebElement> attachment = BasePage.Driver.FindElements(By.CssSelector(".noSelection td[title ^= 'Type:Attachment']"));
                if (attachment.Count > 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7: From Date range search box, select an item that some studies occur within the range
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var js = BasePage.Driver as IJavaScriptExecutor;
                if (js != null) js.ExecuteScript("pmjStudySearchMenuControl.dropDownMenuItemClick(\'1\')");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                table = patients.CollectRecordsInTable(patients.XDSStudiesPatientTable(), patients.XDSStudiesPatientheader(), patients.XDSStudiesPatientrow(), patients.XDSStudiesPatientcolumn());
                if (table.Rows.Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8: Select each single record and load the record
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (js != null) js.ExecuteScript("pmjStudySearchMenuControl.dropDownMenuItemClick(\'0\')");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                element = BasePage.Driver.FindElements(By.CssSelector("img[src^='Images/PlusSign']"))[0];
                basepage.ClickElement(element);
                attachment = BasePage.Driver.FindElements(By.CssSelector(".noSelection td[title ^= 'Type:Attachment']"));
                patients.DoubleClick(attachment[1]);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                element = BasePage.Driver.FindElement(By.CssSelector("iframe[id$='_nonImageDiv_nonImageIframe']"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step_8 = patients.CompareImage(result.steps[ExecutedSteps], element);
                if (step_8)
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
                //Step 9: Move the mouse to the study with one series only (Hemmet, Kirt, Patient ID=945, Modality CT)
                viewer.CloseStudy();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                element = BasePage.Driver.FindElement(By.CssSelector("td[title='64']"));
                patients.JSMouseHover(element);
                element = BasePage.Driver.FindElement(By.CssSelector("tr[style*='background-color: rgb(189, 211, 223)']"));
                if (element.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10: Double click the record
                viewer = patients.LaunchStudy(Patients.PatientColumns.Accession, Accession[2]);
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
                //Step 11: Change the layout for image to 1x2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                Thread.Sleep(3000);
                if (viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("1x2"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12: Change the layout for image to 2x2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                if (viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("2x2"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13: Perform a measurement (draw a line) from one image and save Annotated Image
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 14: Load the study back
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 15: Click on the Patient Record tab
                viewer.CloseStudy();
                resultcount = 0;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(patients.PatientName().Text.Replace(",", ", "), PatientDetails["Name"]) && string.Equals(patients.PatientDOBInfo().Text, PatientDetails["Date of birth"]) && PatientDetails["Address"].Contains(patients.PatientAddress1().Text) && PatientDetails["Address"].Contains(patients.PatientCity().Text) && PatientDetails["Address"].Contains(patients.PatientState().Text))
                {
                    resultcount++;
                }
                element = BasePage.Driver.FindElement(By.CssSelector("span#pmjMainText"));
                if (string.Equals(element.Text, "All Dates"))
                {
                    resultcount++;
                }
                element = BasePage.Driver.FindElement(By.CssSelector("div[class$='TabSelected']"));
                if (string.Equals(element.Text, "Studies"))
                {
                    resultcount++;
                }
                if (resultcount == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16: Go back to Patient Search page and search for "two"
                patients.ClosePatientRecord();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patients.SendKeysInStroke(patients.PatientSearch(), PatientName[1].Split(',')[0].Trim());
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                names = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Name", BasePage.GetColumnNames());
                if (names.All(name => name.ToLowerInvariant().Contains(PatientName[1].Split(',')[0].Trim().ToLowerInvariant())))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17: Highlight the patient and double-click to go to the Patient Record page
                PatientDetails = patients.GetMatchingRow("Name", PatientName[1]);
                patients.LoadStudyInPatientRecord(PatientName[1]);
                patients.NavigateToXdsStudies();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                table = patients.CollectRecordsInTable(patients.XDSStudiesPatientTable(), patients.XDSStudiesPatientheader(), patients.XDSStudiesPatientrow(), patients.XDSStudiesPatientcolumn());
                ActualColumnValues = basepage.GetColumnValues(table, "Accession#");
                if (ActualColumnValues.Contains(Accession[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 18: Load each study to view dicom image
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
                //Step 19: Check the information from MPI to verify the accuracy
                viewer.CloseStudy();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(patients.PatientName().Text.Replace(",", ", "), PatientDetails["Name"]) && string.Equals(patients.PatientDOBInfo().Text, PatientDetails["Date of birth"]) && PatientDetails["Address"].Contains(patients.PatientAddress1().Text) && PatientDetails["Address"].Contains(patients.PatientCity().Text) && PatientDetails["Address"].Contains(patients.PatientState().Text))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 20: Go back to Patient Search page and search for"star"
                patients.ClosePatientRecord();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patients.SendKeysInStroke(patients.PatientSearch(), PatientName[2].Split(',')[0].Trim());
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                names = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Name", BasePage.GetColumnNames());
                if (names.All(name => name.ToLowerInvariant().Contains(PatientName[2].Split(',')[0].Trim().ToLowerInvariant())))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 21: Double click on the patient Starkey Richard Starr from the list
                PatientDetails = patients.GetMatchingRow("Name", PatientName[2]);
                patients.LoadStudyInPatientRecord(PatientName[2]);
                patients.NavigateToXdsStudies();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                table = patients.CollectRecordsInTable(patients.XDSStudiesPatientTable(), patients.XDSStudiesPatientheader(), patients.XDSStudiesPatientrow(), patients.XDSStudiesPatientcolumn());
                ActualColumnValues = basepage.GetColumnValues(table, "Accession#");
                if (ActualColumnValues.Contains(Accession[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 22: Load each study to view dicom image and ensure the information from image overlay and left panel are consistent
                viewer = patients.LaunchStudy(Patients.PatientColumns.Accession, Accession[1]);
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
                //Step 23: Check the information from MPI to verify the accuracy
                viewer.CloseStudy();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (string.Equals(patients.PatientName().Text.Replace(",", ", "), PatientDetails["Name"]) && string.Equals(patients.PatientDOBInfo().Text, PatientDetails["Date of birth"]) && PatientDetails["Address"].Contains(patients.PatientAddress1().Text) && PatientDetails["Address"].Contains(patients.PatientCity().Text) && PatientDetails["Address"].Contains(patients.PatientState().Text))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 24: Go back to studylist (from Patient Search to Studylist)
                patients.ClosePatientRecord();
                studies = (Studies)login.Navigate("Studies");
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
                //Step 25: Select John, Doe from one datasource which is configured to save attachment to original study.
                studies.SearchStudy(LastName: PatientName[3].Split(',')[0].Trim(), FirstName: PatientName[3].Split(',')[1].Trim());
                studies.SelectStudy("Accession", Accession[3]);
                if (BasePage.Driver.FindElements(By.CssSelector("tr[aria-selected='true']>td[title='" + Accession[3] + "']")).Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 26: Load the study and attach a file
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 27: Go back to studylist the make sure OT is created
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 28: Go to Patient Record to search for John, Doe and View
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 29: Verify the attachment is attached to the correctly study
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 30: Expand the study that has + sign
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 31: Load the attachment
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 32: Close the Viewer and go back to Patient Search page
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
                //Step 33: Perform search on Kirk
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patients.SendKeysInStroke(patients.PatientSearch(), PatientName[0].Split(',')[1].Trim());
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                names = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Name", BasePage.GetColumnNames());
                if (names.All(name => name.ToLowerInvariant().Contains(PatientName[0].Split(',')[1].Trim().ToLowerInvariant())))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //Step 34: View the patient record
                patients.LoadStudyInPatientRecord(PatientName[0]);
                patients.NavigateToXdsStudies();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                table = patients.CollectRecordsInTable(patients.XDSStudiesPatientTable(), patients.XDSStudiesPatientheader(), patients.XDSStudiesPatientrow(), patients.XDSStudiesPatientcolumn());
                ActualColumnValues = basepage.GetColumnValues(table, "ID");
                if (ID.All(ex => ActualColumnValues.Contains(ex)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //Step 35: Pick StudyID 68 from DS1 and load
                viewer = patients.LaunchStudy(Patients.PatientColumns.Accession, Accession[4]);
                resultcount = 0;
                if (viewer.ViewStudy())
                {
                    resultcount++;
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                string[] ExpectedValues = new string[] { "seriesscope", "imagescope", "saveseries", "localizerline" };
                IList<string> Tools = basepage.GetReviewToolsFromviewer().Select(x => x.ToLowerInvariant()).ToList();
                if (ExpectedValues.All(exp => Tools.Contains(exp)))
                {
                    resultcount++;
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
                }
                //Step 36: Open attachment panel and attach a file
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 37: Open Istore where the Attachment is configured to be saved to
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 38: Go back to Patient Record page
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 39: Expand the study that has + sign
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 40: Load the attachment
                result.steps[++ExecutedSteps].status = "Not Automated";
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
                servicetool.CloseServiceTool();
                try
                {
                    servicetool.LaunchServiceTool();
                    servicetool.OtherDocumentsTabInPMJ(false);
                    servicetool.CloseServiceTool();
                }
                catch (Exception) { }
                try
                {
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    userpreferences.OpenUserPreferences();
                    userpreferences.SwitchToUserPrefFrame();
                    userpreferences.UnCheckCheckbox(userpreferences.PatientRecordLiveSearch());
                    userpreferences.CloseUserPreferences();
                    login.Logout();
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        ///  Caliper and Localizer line with multiple locales
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_87648(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            UserPreferences userpreferences = new UserPreferences();
            bool dotnotation = false;
            string[] Accession = null;
            try
            {
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                //PreCondition
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.Culture']", "value", "en-US, nl-BE");
                servicetool.RestartIISUsingexe();
                //Step 1: Load CR study in iCA
                login.DriverGoTo(login.url);
                login.PreferredLanguageSelectList().SelectByValue("en-US");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LoginButton")));
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                if (viewer.ViewStudy())
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
                //Step 2: Note down the Caliper size which is located on the right side of every Viewport
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 3: Logout from iCA
                login.Logout();
                ExecutedSteps++;
                //Step 4: update the culture value to ("nl-BE") in web.config as per precondition
                ExecutedSteps++;
                //Step 5: Load the same CR study.
                login.DriverGoTo(login.url);
                login.PreferredLanguageSelectList().SelectByValue("nl-BE");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LoginButton")));
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                if (viewer.ViewStudy())
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
                //Step 6: Verify that Caliper size remains same
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 7: Localizer lines
                basepage.ChangeAttributeValue(Config.ImagerConfiguration, "/property[@key='LocalizerLineTextFontSize']", "value", "20.0");
                basepage.ChangeAttributeValue(Config.ImagerConfiguration, "/property[@key='LocalizerLineWidth']", "value", "10.0");
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;
                //Step 8: Log-in to iCA using valid credentials.
                login.DriverGoTo(login.url);
                login.PreferredLanguageSelectList().SelectByValue("en-US");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LoginButton")));
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;
                //Step 9:  	Load CT/MR study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (viewer.SeriesViewPorts().Count == 4)
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
                //Step 10: Select the reference image and click on localizer line icon in review toolbar
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 11: Scroll through different series to view localizer line and Verify that font size and size of the localizer line are displayed as per configured values in ImagerConfiguration.xml
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 12: Logout from iCA.
                login.Logout();
                ExecutedSteps++;
                //Step 13: update the culture value to ("nl-BE") in web.config as per precondition
                login.DriverGoTo(login.url);
                login.PreferredLanguageSelectList().SelectByValue("nl-BE");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LoginButton")));
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;
                //Step 14: Load the same CT/MR study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (viewer.SeriesViewPorts().Count == 4)
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
                //Step 15: Select the reference image and click on localizer line icon in review toolbar
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 16: Scroll through different series to view localizer line and Verify that font size and size of the localizer line should be same as in (step 14)
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 17: Login iCA as Administrator in preferred language
                login.Logout();
                login.DriverGoTo(login.url);
                login.PreferredLanguageSelectList().SelectByValue("en-US");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LoginButton")));
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;
                //Step 18: 
                /* Create a new domain, e.g., Domain1, with presets as below in Domain Management
                    From Default Settings Per Modality section, define Width / Level presets for MR modality, e.g., -Default Layout for MR: 2x2
                      - MR DP1: Width 111.1, Level 111.5
                      - MR DP2: Width 222.2, Level 222.8*/
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain[DomainManagement.DomainAttr.DomainName], createDomain[DomainManagement.DomainAttr.RoleName], datasources: null);
                domainmanagement.AddPresetForDomain(modality: "MR", preset: "DP1", width: "111.1", level: "111.5", layout: "2x2");
                domainmanagement.AddPresetForDomain(modality: "MR", preset: "DP2", width: "222.2", level: "222.8", layout: "2x2");
                dotnotation = string.Equals(basepage.DotNotationLbl(), "(Use dot notation for decimal input)", StringComparison.OrdinalIgnoreCase);
                domainmanagement.ClickSaveDomain();
                if (dotnotation)
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
                //Step 19: Go to Role Management page
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                if (login.IsTabSelected("Role Management"))
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
                //Step 20: 
                /*Create a new non- admin role in the new domain created(e.g., Role1)
                Uncheck Use Domain Settings in Default Settings Per Modality
                Define Width/ Level presets for CT modality, e.g.,
                 -Default Layout for CT: 2x2
                - CT RP1: Width 333.3, Level 333.5
                - CT RP2: Width 444.4, Level 444.7 */
                rolemanagement.SelectDomainfromDropDown(createDomain[DomainManagement.DomainAttr.DomainName]);
                rolemanagement.ClickNewRoleBtn();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                basepage.SendKeys(rolemanagement.RoleNameTxt(), createDomain[DomainManagement.DomainAttr.RoleName] + "New");
                basepage.SendKeys(rolemanagement.RoleDescriptionTxt(), createDomain[DomainManagement.DomainAttr.RoleDescription]);
                rolemanagement.UnCheckCheckbox(rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB());
                rolemanagement.AddPresetForRole("CT", "RP1", "333.3", "333.5", "2x2");
                rolemanagement.AddPresetForRole("CT", "RP2", "444.4", "444.7", "2x2");
                dotnotation = string.Equals(basepage.DotNotationLbl(), "(Use dot notation for decimal input)", StringComparison.OrdinalIgnoreCase);
                rolemanagement.SaveBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                if (dotnotation)
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
                //Step 21: 
                /* Load a study in study viewer
                    Open User Preferences from toolbar
                    Define a Width/Level Presets for any modality available in the Modality dropdown list. */
                login.Logout();
                login.LoginIConnect(createDomain[DomainManagement.DomainAttr.DomainName], createDomain[DomainManagement.DomainAttr.DomainName]);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.UserPreference);
                userpreferences.AddPresetAtToolbar("CT", "RP1", "333.3", "333.5", "2x2");
                dotnotation = string.Equals(basepage.DotNotationLbl(), "(Use dot notation for decimal input)", StringComparison.OrdinalIgnoreCase);
                userpreferences.SaveToolBarUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();
                if (dotnotation)
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
                //Step 22: In Service Tool,go to Viewer,subtab protocol
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                servicetool.WaitWhileBusy();
                servicetool.NavigateSubTab("Protocols");
                servicetool.WaitWhileBusy();
                dotnotation = string.Equals(WpfObjects._mainWindow.Get(SearchCriteria.ByText("(Use dot notation for decimal input)")).Name, "(Use dot notation for decimal input)");
                if (dotnotation)
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
                //Step 23: Goto Download report
                servicetool.NavigateSubTab("Download Report");
                servicetool.WaitWhileBusy();
                dotnotation = string.Equals(WpfObjects._mainWindow.Get(SearchCriteria.ByText("(Use dot notation for decimal input)")).Name, "(Use dot notation for decimal input)");
                if (dotnotation)
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
                //Step 24: Goto Linked Scrolling tab
                servicetool.NavigateToTab("Linked Scrolling");
                servicetool.WaitWhileBusy();
                dotnotation = string.Equals(WpfObjects._mainWindow.Get(SearchCriteria.ByText("(Use dot notation for decimal input)")).Name, "(Use dot notation for decimal input)");
                if (dotnotation)
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
                servicetool.CloseServiceTool();
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
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Application.Culture']", "value", "en-US");
                servicetool.RestartIISUsingexe();
            }
        }

        /// <summary>
        /// Hosted Configuration
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_27924(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingViewer bluringviewer = new BluRingViewer();
            string[] PatientName = null;
            IntegratorStudies integratorstudies = null;
            string url = string.Empty;
            string DS1 = login.GetHostName(Config.EA91);
            string DS2 = login.GetHostName(Config.SanityPACS);
            string XMLNodePath = string.Concat("/add[@id='", DS2, "']/parameters/amicas.userName");
            string DS = DS1 + @"\" + DS2;
            try
            {
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                //PreCondition
                basepage.ChangeNodeValue(Config.DSManagerFilePath, XMLNodePath, "dicom123");
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/appSettings/add[@key='Integrator.OnMultipleStudy']", "value", "Show Error");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/IntegratedMode/AllowShowSelector", "True");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/IntegratedMode/AllowShowSelectorSearch", "False");
                servicetool.RestartIISUsingexe();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.SelectCheckBox("CB_ShowThumbnailOverlays");
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                //Step 1: 
                /*Open TestEHR application.
                Select Image Load tab, enter Patient Name that matches multiple patients with multiple studies in one data source domain.
                Ensure that: 'Show Report' is set to 'True', 'Auto End Session' is set to 'True','Show selector' is is set to 'False', and click Load.*/
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(showSelector: "False", showReport: "True");
                ehr.SetSearchKeys_Study(PatientName[0], "Last_Name");
                ehr.SetSearchKeys_Study(DS, "Datasource");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToBluringIntegratorURL(url);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                if (ehr.ErrorMsg().Trim().Equals("Error Occurred in operation: More than one patient and/or study was found that match the search criteria. Each request to view a study must result in a unique study."))
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
                //Step 2: 
                /*Back to TestEHR application.
                Select Image Load tab, enter Patient Name that matches multiple patients with multiple studies in one data source domain. Ensure that 'Show Report' is set to 'True', 'Auto End Session' is set to 'True', 'Show selector' is set to 'True', and click Load.*/
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(showSelector: "True", showReport: "True");
                ehr.SetSearchKeys_Study(PatientName[0], "Last_Name");
                ehr.SetSearchKeys_Study(DS, "Datasource");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToBluringIntegratorURL(url);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                if (ehr.VerifyPatientDetails("Last Name", PatientName[0]))
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
                //Step 3: In the patient result list put a check mark beside all the patients and click on View button. Verify the Patient information and image overlay text displayed.
                foreach (IWebElement element in integratorstudies.Intgtr_CheckBoxes())
                {
                    basepage.ClickElement(element);
                }
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step3_1 = BasePage.Driver.FindElement(By.CssSelector("div.patientHistoryPartialResult")).Text.Contains("One or more studies may be missing or incomplete.");
                bool Step3_2 = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.MultiplePatientErrorContent).Text.Contains("Studies listed below may not belong to the same patient");
                bool Step3_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel());
                if (Step3_1 && Step3_2 && Step3_3)
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

                //Step 4: From the Patient History drawer select and load the other studies. Observe the thumbnails, patient information menu(Verify Patient Name and ID updates) and the image overlay text.
                bluringviewer.OpenPriors(1);
                bluringviewer.OpenPriors(2);
                ExecutedSteps++;
                TestStep step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                bool Step4_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(2));
                step.SetPath(testid, ExecutedSteps + 1, 2);
                bool Step4_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(3));
                if (Step4_1 && Step4_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Step4_1 = " + Step4_1);
                    Logger.Instance.InfoLog("Step4_2 = " + Step4_2);
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5: 
                /* Back to TestEHR application.
                    Return to the Image Load tab, enter Patient Name that matches 1 patient with multiple studies in multiple data source domains.
                    Ensure that: 'Show Report' is set to 'True', 'Auto End Session' is set to 'True', 'Show selector' is is set to 'False', and click Load.
                    Note: what I understand by '1 patient with multiple studies ...' is '1 patient who has the same last/first/middle name and patient ID, with multiple studies ...'*/
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(showSelector: "False", showReport: "True");
                ehr.SetSearchKeys_Study(PatientName[1].Split(':')[0], "Last_Name");
                ehr.SetSearchKeys_Study(PatientName[1].Split(':')[1], "First_Name");
                ehr.SetSearchKeys_Study(DS, "Datasource");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToBluringIntegratorURL(url);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                if (ehr.ErrorMsg().Trim().Equals("Error Occurred in operation: More than one patient and/or study was found that match the search criteria. Each request to view a study must result in a unique study."))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //Step 6: 
                /*Back to TestEHR application.
                Return to the Image Load tab, enter Patient Name that matches 1 patient with multiple studies in multiple data source domains.
                Ensure that: 'Show Report' is set to 'True', 'Auto End Session' is set to 'True', 'Show selector' is is set to 'True', and click Load.*/
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(showSelector: "True", showReport: "True");
                ehr.SetSearchKeys_Study(PatientName[1].Split(':')[0], "Last_Name");
                ehr.SetSearchKeys_Study(PatientName[1].Split(':')[1], "First_Name");
                ehr.SetSearchKeys_Study(DS, "Datasource");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToBluringIntegratorURL(url);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                if (ehr.VerifyPatientDetails("Last Name", PatientName[1].Split(':')[0]) && ehr.VerifyPatientDetails("First Name", PatientName[1].Split(':')[1]))
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
                //Step 7: In the patient result list put a check mark beside all the patients and click on View button. Verify the Patient information and image overlay text displayed.
                foreach (IWebElement element in integratorstudies.Intgtr_CheckBoxes())
                {
                    basepage.ClickElement(element);
                }
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step7_1 = BasePage.Driver.FindElement(By.CssSelector("div.patientHistoryPartialResult")).Text.Contains("One or more studies may be missing or incomplete.");
                bool Step7_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel());
                if (Step7_1 && Step7_2)
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

                //Step 8: From the Patient History drawer select and load the other studies. Observe the thumbnails, patient information menu(Verify Patient Name and ID updates) and the image overlay text.
                bluringviewer.OpenPriors(1);
                bluringviewer.OpenPriors(2);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                bool Step8_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(2));
                step.SetPath(testid, ExecutedSteps + 1, 2);
                bool Step8_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(3));
                if (Step8_1 && Step8_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Step8_1 = " + Step8_1);
                    Logger.Instance.InfoLog("Step8_2 = " + Step8_2);
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9: 
                /* Back to TestEHR application..
                Return to the Image Load tab, enter Patient Name that matches multiple patients and studies in multiple data source domains. Ensure that Show Report is enabled, Show selector is set to 'True', Data Sources have different Patient ID Domains and click Load.*/
                basepage.ChangeNodeValue(Config.DSManagerFilePath, XMLNodePath, "dicom");
                servicetool.RestartIISUsingexe();
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSelectorOptions(showSelector: "True", showReport: "True");
                ehr.SetSearchKeys_Study(PatientName[0], "Last_Name");
                ehr.SetSearchKeys_Study(DS, "Datasource");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                bluringviewer.NavigateToBluringIntegratorURL(url);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                if (ehr.VerifyPatientDetails("Last Name", PatientName[0]))
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
                //Step 10: Select multiple patients from multiple data source domains, and select View. Observe the thumbnail menu, patient information menu (Verify Patient Name and ID updates), and the image overlay text.
                foreach (IWebElement element in integratorstudies.Intgtr_CheckBoxes())
                {
                    basepage.ClickElement(element);
                }
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step10_1 = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.MultiplePatientErrorContent).Text.Contains("Studies listed below may not belong to the same patient");
                bool Step10_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel());
                if (Step10_1 && Step10_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Step10_1 = " + Step10_1);
                    Logger.Instance.InfoLog("Step10_2 = " + Step10_2);
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11: From the Patient History drawer select and load the other studies. Observe the thumbnails, patient information menu (Verify Patient Name and ID updates) and the image overlay text.
                bluringviewer.OpenPriors(1);
                bluringviewer.OpenPriors(2);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                bool Step11_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(2));
                step.SetPath(testid, ExecutedSteps + 1, 2);
                bool Step11_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(3));
                if (Step11_1 && Step11_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Step11_1 = " + Step11_1);
                    Logger.Instance.InfoLog("Step11_2 = " + Step11_2);
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12: Verify the thumbnail
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                bool Step12_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelThumbnailIndicator(0)[0]);
                step.SetPath(testid, ExecutedSteps + 1, 2);
                bool Step12_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelThumbnailIndicator(1)[0], 2);
                step.SetPath(testid, ExecutedSteps + 1, 3);
                bool Step12_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelThumbnailIndicator(2)[0], 3);
                if (Step12_1 && Step12_2 && Step12_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Step12_1 = " + Step12_1);
                    Logger.Instance.InfoLog("Step12_2 = " + Step12_2);
                    Logger.Instance.InfoLog("Step12_3 = " + Step12_3);
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13: Open service tool>Viewer>Miscellaneous Modify uncheck show thumbnail overlays Apply Restart IIS
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.UnSelectCheckBox("CB_ShowThumbnailOverlays");
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 14: Reload the study in step 10
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(url);
                login.NavigateToIntegratorURL(url);
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                foreach (IWebElement element in integratorstudies.Intgtr_CheckBoxes())
                {
                    basepage.ClickElement(element);
                }
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                bluringviewer.OpenPriors(1);
                bluringviewer.OpenPriors(2);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                bool Step14_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelThumbnailIndicator(0)[0]);
                step.SetPath(testid, ExecutedSteps + 1, 2);
                bool Step14_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelThumbnailIndicator(1)[0], 2);
                step.SetPath(testid, ExecutedSteps + 1, 3);
                bool Step14_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.StudyPanelThumbnailIndicator(2)[0], 3);
                if (Step14_1 && Step14_2 && Step14_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog("Step14_1 = " + Step14_1);
                    Logger.Instance.InfoLog("Step14_2 = " + Step14_2);
                    Logger.Instance.InfoLog("Step14_3 = " + Step14_3);
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15: Revert the setting changes made for overlays
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.SelectCheckBox("CB_ShowThumbnailOverlays");
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
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
                //basepage.ChangeAttributeValue(@"C:\WebAccess\IntegratorAuthenticationSTS\Web.config", "/authProvider[@id='']", "class", "Sample.HostIntegration.Authentication.IntegratorAuthenticator");
                basepage.ChangeNodeValue(Config.FileLocationPath, "/IntegratedMode/AllowShowSelector", "False");
                basepage.ChangeNodeValue(Config.DSManagerFilePath, XMLNodePath, "dicom");
                servicetool.RestartIISUsingexe();
            }
        }

        /// <summary>
        /// Window width/level Preset
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27922(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement = null;
            UserPreferences userpreferences = new UserPreferences();
            string[] Accession = null;
            int resultcount = 0;
            string Datasource = Config.DestinationPACS;
            string AETitle = Config.DestinationPACSAETitle;
            string DicomPath = string.Empty;
            try
            {
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                DicomPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                var client = new DicomClient();
                for (int i = 1; i <= 6; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27922-", i)));
                    client.Send(Datasource, 104, false, "SCU", AETitle);
                }
                //Step 1: In Domain Management page define a Window Width and Level preset for each Modality listed.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain[DomainManagement.DomainAttr.DomainName], createDomain[DomainManagement.DomainAttr.RoleName], datasources: null);
                domainmanagement.AddPresetForDomain(modality: "MR", preset: "MR", width: "111.1", level: "111.5", layout: "2x2");
                domainmanagement.AddPresetForDomain(modality: "CR", preset: "CR", width: "222.2", level: "222.8", layout: "2x2");
                domainmanagement.ClickSaveDomain();
                login.Logout();
                ExecutedSteps++;
                //Step 2: Go to iConnect Access Study Viewer. Load each study into Series scope. Apply the preset. Compare the Window Width/Level value in the viewport with the defined ones.
                login.LoginIConnect(createDomain[DomainManagement.DomainAttr.DomainName], createDomain[DomainManagement.DomainAttr.DomainName]);
                studies = (Studies)login.Navigate("Studies");
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText("CR");
                userpreferences.ViewingScopeSeriesRadioBtn().Click();
                userpreferences.ModalityDropDown().SelectByText("MR");
                userpreferences.ViewingScopeSeriesRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.Preset().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                if (string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "width"), "222.2") && string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "level"), "222.8"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Width and Level of CR is same as defined one");
                }
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.Preset().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                if (string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "width"), "111.1") && string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "level"), "111.5"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Width and Level of MR is same as defined one");
                }
                viewer.CloseStudy();
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
                //Step 3: Use pummel to create studies with multiple images per series if one cannot be found for a given modality. Change to image scope. Apply the preset on selected image. (change the ww/wl on the image before apply the preset)
                resultcount = 0;
                ExecutedSteps++;
                TestStep step = result.steps[ExecutedSteps];
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText("CR");
                userpreferences.ViewingScopeImageRadioBtn().Click();
                userpreferences.ModalityDropDown().SelectByText("MR");
                userpreferences.ViewingScopeImageRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.Preset().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                if (string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "width"), "222.2") && string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "level"), "222.8"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Width and Level of CR is same as defined one");
                }
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Width and Level of Non Selected CR is not altered");
                }
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[3]);
                studies.SelectStudy("Accession", Accession[3]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.Preset().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                if (string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "width"), "111.1") && string.Equals(viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "level"), "111.5"))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Width and Level of MR is same as defined one");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("The Width and Level of Non Selected MR is not altered");
                }
                viewer.CloseStudy();
                if (resultcount == 4)
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
        /// Study Query
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27923(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string DicomPath = string.Empty;
            string imagecount = string.Empty;
            string[] Accession = null;
            string[] Type = null;
            DicomClient client = new DicomClient();
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 5, 0);
            try
            {
                DicomPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                //Step 1: Configure an I-Store and ECM datasource. Send studies Abdomen,CT to the I-Store datasource and the *^<^ *Quebec Dataset *^>^ *to the ECM datasource
                client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27923EA-01")));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27923EA-03")));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                ExecutedSteps++;
                //Step 2: Login to iConnect Access and load the dataset Abdomen,CT.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy())
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
                //Step 3: Immediately send an HTML report to the I-Store datasource for Abdomen,CT
                /*login.SendHL7Order(Config.DestEAsIp, 12800, string.Concat(DicomPath, "27923EA-02"));
                ExecutedSteps++;*/
                result.steps[++ExecutedSteps].status = "On Hold";
                //step 4: Return to the studylist then reload the dataset Abdomen,CT.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy())
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
                //Step 5: Click on the reports panel
                /*BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));
                viewer.TitlebarReportIcon().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                //Need to write Validation
                ExecutedSteps++;*/
                result.steps[++ExecutedSteps].status = "On Hold";
                //Step 6: Return to the studylist then load the dataset *^<^*Quebec Dataset*^>^*.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy())
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
                //Step 7: Immediately send SR and audio reports to the ECM datasource for *^<^*Quebec Dataset*^>^
                client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27923EA-04")));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27923EA-05")));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                ExecutedSteps++;
                //Step 8: Return to the studylist then reload the dataset *^<^*Quebec Dataset*^>^*
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy())
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
                //Step 9: Click on the reports panel.
                PageLoadWait.WaitForElementToDisplay(viewer.TitlebarReportIcon(), 10);
                if (viewer.TitlebarReportIcon().Displayed)
                {
                    viewer.TitlebarReportIcon().Click();
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                    viewer.ViewerReportListButton().Click();
                    Type = BasePage.GetColumnValues(viewer.StudyViewerListResults("StudyPanel", "report", 1), "Type", BasePage.GetColumnNames(1));
                    if (Type.Length == 0)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                //Step 10: Return to the studylist, wait for 5 min then reload the dataset 
                viewer.CloseStudy();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 5 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.ViewStudy())
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
                //Step 11: Click on the reports panel.
                PageLoadWait.WaitForElementToDisplay(viewer.TitlebarReportIcon(), 10);
                viewer.TitlebarReportIcon().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                viewer.ViewerReportListButton().Click();
                Type = BasePage.GetColumnValues(viewer.StudyViewerListResults("StudyPanel", "report", 1), "Type", BasePage.GetColumnNames(1));
                if (Type.Any(ty => ty.Contains("SR")) && Type.Any(ty => ty.Contains("AU")))
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
                //Step 12: Logout of iConnect Access then send study"\\\10.4.16.130\anonymized_data\Data Sets by VP (pending)\iCA\Hazard\HazardData\CARR,LISA\DICOM\1"to the AMICASPACS datasource.
                login.Logout();
                for (int i = 1; i <= 3; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27923PACS-0" + i)));
                    client.Send(Config.DestinationPACS, 104, false, "SCU", Config.DestinationPACSAETitle);
                }
                ExecutedSteps++;
                //Step 13: Login to iConnect Access and load the dataset CARR,LISA.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.GetMatchingRow("Accession", Accession[2]).TryGetValue("# Images", out imagecount);
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                if (string.Equals(imagecount, "3") && viewer.ViewStudy())
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
                //Step 14: Immediately transfer images from \\10.4.16.130\anonymized_data\Data Sets by VP (pending)\iCA\Hazard\HazardData\CARR,LISA\DICOM\2"to the AMICASPACS datasource.
                for (int i = 4; i <= 6; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27923PACS-0" + i)));
                    client.Send(Config.DestinationPACS, 104, false, "SCU", Config.DestinationPACSAETitle);
                }
                ExecutedSteps++;
                //Step 15: Return to the studylist and load CARR,LISA.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.GetMatchingRow("Accession", Accession[2]).TryGetValue("# Images", out imagecount);
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                if (string.Equals(imagecount, "3") && viewer.ViewStudy())
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
                //Step 16: Return to the studylist, wait 5 min then load CARR,LISA.
                viewer.CloseStudy();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 5 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.GetMatchingRow("Accession", Accession[2]).TryGetValue("# Images", out imagecount);
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                if (string.Equals(imagecount, "6") && viewer.ViewStudy())
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
        /// 27930 - Recompression of lossy compressed images
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27930(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            int resultcount = 0;
            result.SetTestStepDescription(teststeps);
            UserPreferences userpreferences = new UserPreferences();
            UserManagement usermanagement = new UserManagement();
            Studies studies = new Studies();
            StudyViewer viewer = new StudyViewer();
            DomainManagement domainmanagement = new DomainManagement();
            Inbounds inbounds = new Inbounds();
            String studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String[] studypath = studypaths.Split('=');
            String AccessionIds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
            String[] AccessionId = AccessionIds.Split(':');

            String PatientIds = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientId");
            String[] PatientId = PatientIds.Split(':');
            // 

            String username = "user27930_" + new Random().Next(1000);
            String roleName = "SuperRole";
            String DomainName = "SuperAdminGroup";

            try
            {
                //Step 1: 	Login as Administrator in User preferrence set Image format to JPEG(lossy) save changes
                // Enable email study for the users domain
                // Enable grant access

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.JPEGRadioBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#SavePreferenceUpdateButton")));
                userpreferences.SavePreferenceBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                basepage.SwitchToDefault();
                basepage.SwitchTo("index", "0");
                basepage.SwitchTo("id", "m_preferenceFrame");
                userpreferences.CloseBtn().Click();

                // Enable email study for the users domain
                // Enable grant access
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.MoveToolsToToolbarSection(new string[] { IEnum.ViewerTools.EmailStudy.ToString() });
                domainmanagement.ClickSaveEditDomain();

                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList("SuperAdminGroup");
                usermanagement.SearchUser("SuperAdmin");
                usermanagement.SelectUser("SuperAdmin");
                usermanagement.ClickEditUser();
                PageLoadWait.WaitForFrameLoad(10);
                if (!usermanagement.AllowEmailStudy().Selected)
                {
                    usermanagement.AllowEmailStudy().Click();
                }
                usermanagement.SaveBtn().Click();

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.SelectDomainFromDropdownList(DomainName);
                if (!usermanagement.SearchUser(username))
                {
                    usermanagement.CreateUser(username, roleName, Password: username);
                }

                ExecutedSteps++;

                // Step 2
                // Test Data: Choose lossy dataset from:
                //From 10.4.16.130\anonymized_data\Data Sets by Modality\HazardData\Compression\Different compression
                // Load a lossy compression study
                BasePage.RunBatchFile(Config.batchfilepath, studypath[0] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                ExecutedSteps++;

                // Step 3
                //  Rotate image Clockwise
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionId[0], Datasource: "All");
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_3)
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

                // Step 4
                // Apply various tools(zoom/pan/WL), measurements on the image
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement source = viewer.ViewportScrollHandle(1, 1);
                IWebElement destination = viewer.ViewportScrollBar(1, 1);

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
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                ExecutedSteps++;

                // Step 5
                // Click on Save series
                int ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1))
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
                // Reload the study and view the PR just saved
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                if (viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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

                //Step 7
                // Print the image
                result.steps[++ExecutedSteps].status = "Not Automated";

                // Step 8
                // Turn Text off
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                bool toogleTextoff = viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off");
                if (toogleTextoff)
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

                // Step 9 
                // Rotate image clockwise
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_9)
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

                // Step 10
                // Apply various tools(zoom/pan/WL), measurements on the image
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.ViewportScrollBar(1, 1);

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
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                ExecutedSteps++;

                // Step 11
                // Click on Save series
                ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1))
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

                // Step 12
                // Reload the study and view the PR just saved
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                if (viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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

                // Step 13
                // Print the image
                result.steps[++ExecutedSteps].status = "Not automated";

                // Step 14
                // Email the study (provide a valid email address, name reason)
                // note the pin
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy("Accession", AccessionId[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                viewer.EmailStudy("venkat@mergetestmail.com", "Test", "Test", 1);
                String pinnumber = viewer.FetchPin();
                viewer.CloseStudy();
                ExecutedSteps++;

                // Step 15
                // Click on the link in the mail, provide pin generated in above step
                result.steps[++ExecutedSteps].status = "Not automated";

                // Step 16
                // Close study, in user preferrences set Image format to PNG(lossless) and save changes
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.PNGRadioBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#SavePreferenceUpdateButton")));
                userpreferences.SavePreferenceBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                basepage.SwitchToDefault();
                basepage.SwitchTo("index", "0");
                basepage.SwitchTo("id", "m_preferenceFrame");
                userpreferences.CloseBtn().Click();
                ExecutedSteps++;
                login.Logout();


                // Step 17
                // Search for a Lossy Compression study and Share it to another user
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy("Accession", AccessionId[0]);
                PageLoadWait.WaitForFrameLoad(10);
                studies.GrantAccessBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_userFilterInput']")));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_userFilterInput']\").click()");
                BasePage.Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).SendKeys(username);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")));
                BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_m_userlist_hierarchyUserList_itemList']")));
                IWebElement table = BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_userlist_hierarchyUserList_itemList']"));
                IList<IWebElement> rows = table.FindElements(By.TagName("tr"));
                foreach (IWebElement row in rows)
                {
                    String User = row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML");
                    if (User.Contains(username))
                    {
                        row.FindElement(By.CssSelector("td>span")).Click();
                        BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_userlist_Button_Add']")).Click();
                    }
                }
                BasePage.Driver.FindElement(By.CssSelector("[id$='StudySharingControl_GrantAccessButton']")).Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DialogContentDiv")));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 18
                // Test Data:
                //  login in iCA as the user to whom study was shared navigate to Inbounds tab and perform study search
                login.LoginIConnect(username, username);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: AccessionId[0]);
                inbounds.SelectStudy("Accession", AccessionId[0]);
                ExecutedSteps++;

                // Step 19
                // Load the study
                inbounds.LaunchStudy();
                ExecutedSteps++;

                // Step 20
                // Test Data:Rotate image clockwise
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_20 = inbounds.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_20)
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


                // Step 21
                // Apply various tools(zoom/pan/WL), measurements on the image
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.ViewportScrollBar(1, 1);

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
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                ExecutedSteps++;

                // Step 22
                // Click on Save series
                ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1))
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
                inbounds.CloseStudy();

                // Step 23
                // Reload the study and view the PR just saved
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                if (viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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

                // Step 24
                // Print the image
                result.steps[++ExecutedSteps].status = "Not automated";

                // Step 25
                // Turn text off
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                toogleTextoff = viewer.SeriesViewer_1X1().GetAttribute("src").Contains("off");
                if (toogleTextoff)
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

                // Step 26
                // Rotate image clockwise
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_26 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_26)
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
                // Step 27
                // Apply various tools(zoom/pan/WL), measurements on the image
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.ViewportScrollBar(1, 1);

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
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                //Apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                ExecutedSteps++;

                // Step 28
                // Click on Save series
                ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);

                PageLoadWait.WaitForLoadingIconToAppear_Savestudy();
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                if (viewer.Thumbnails().Count == (ThumbnailCountBeforeSave_FirstStudy + 1))
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

                // Step 29
                // Reload the study and view the PR just saved
                studies.SearchStudy(AccessionNo: AccessionId[0]);
                studies.SelectStudy1("Accession", AccessionId[0]);
                studies.LaunchStudy();
                viewer.SeriesViewer_1X1().Click();
                if (viewer.ThumbnailCaptions()[0].Text.Contains("PR"))
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

                // Step 30
                // Print the image
                result.steps[++ExecutedSteps].status = "Not automated";

                // Step 31
                // Test Data:
                // Choose lossless dataset from:
                // From 10.4.16.130\anonymized_data\Data Sets by Modality\HazardData\Compression\Different compression
                // CLose study and load a lossless image
                // BasePage.RunBatchFile(Config.batchfilepath, studypath[1] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionId[1]);
                studies.SelectStudy1("Accession", AccessionId[1]);
                studies.LaunchStudy(); result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_31 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_31)
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
                studies.CloseStudy();

                // Step 32
                // Close study and in User preferrence set Image format to JPEG(lossy) save changes
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.JPEGRadioBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#SavePreferenceUpdateButton")));
                userpreferences.SavePreferenceBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                basepage.SwitchToDefault();
                basepage.SwitchTo("index", "0");
                basepage.SwitchTo("id", "m_preferenceFrame");
                userpreferences.CloseBtn().Click();
                ExecutedSteps++;

                // Step 33
                // Test Data:
                // Choose lossless dataset from:
                // From 10.4.16.130\anonymized_data\Data Sets by Modality\HazardData\Compression\Different compression
                // Load a lossless image
                // BasePage.RunBatchFile(Config.batchfilepath, studypath[1] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionId[1]);
                studies.SelectStudy1("Accession", AccessionId[1]);
                studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_33 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_33)
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
                studies.CloseStudy();

                // Step 34
                // Load study (patient id 3MG1 in datasource ECM_ARC_116)
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionId[1]);
                studies.SelectStudy1("Accession", AccessionId[1]);
                studies.LaunchStudy();
                studies.CloseStudy();
                ExecutedSteps++;

                // Step 35
                // Close study and Load study Patient ID 12345-9999-50 (ICAVNA002)
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientId[0]);
                studies.SelectStudy1("Patient ID", PatientId[0]);
                studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_35 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_35)
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
                studies.CloseStudy();

                // Step 36
                // CLose study and Load study Patient ID 12345 (ICAVNA002)
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientId[1]);
                studies.SelectStudy1("Patient ID", PatientId[1]);
                studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_36 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_36)
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
                studies.CloseStudy();
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
            }
        }

        /// <summary>
        /// 27916 - Multiple Pixel Spacing in a Series (Series Scope)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27916(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string DicomPath = string.Empty;
            string Accession = string.Empty;
            try
            {
                DicomPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
                //Step 1: Modify 3 images of AAXFFP to have a pixel spacing (0028, 0030) of"0.50\0.50","1\1"and"2\2"and send them to Istoreonline (NOTABUG) data source.
                var client = new DicomClient();
                for (int i = 1; i <= 4; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27916-", i)));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                ExecutedSteps++;
                //Step 2: Load AAXFFP and display series 2 with a 1x1 layout.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession);
                studies.SelectStudy("Accession", Accession);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.ViewStudy() && viewer.SeriesViewPorts().Count == 1)
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
                //Step 3: Without scrolling, change the image layout to 2x2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("2x2"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4: Remove 3 images of AAXFFP from the datasource
                login.Logout();
                hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", Accession);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#tabrow>tbody tr")).Click();
                    PageLoadWait.WaitForHPPageLoad(20);
                    BasePage.Driver.FindElement(By.CssSelector("table#results tr.odd")).Click();
                    PageLoadWait.WaitForHPPageLoad(20);
                    int rowsize = BasePage.Driver.FindElements(By.CssSelector("table#results tbody tr")).Count;
                    for (int i = 0; i < rowsize; i++)
                    {
                        IWebElement row = BasePage.Driver.FindElements(By.CssSelector("table#results tbody tr"))[i];
                        IWebElement delete = row.FindElement(By.CssSelector("img[alt='Delete']"));
                        if (!(string.Equals(row.FindElement(By.CssSelector("td:nth-child(4)")).Text, "1")))
                        {
                            basepage.ClickElement(delete);
                            BasePage.wait.Until(ExpectedConditions.AlertIsPresent());
                            IAlert messagebox = BasePage.Driver.SwitchTo().Alert();
                            messagebox.Accept();
                            BasePage.Driver.SwitchTo().DefaultContent();
                            PageLoadWait.WaitForHPPageLoad(20);
                            Logger.Instance.InfoLog("Study deleted Successfully");
                            rowsize = BasePage.Driver.FindElements(By.CssSelector("table#results tbody tr")).Count;
                            i = -1;
                        }
                    }
                    basepage.ClickElement(BasePage.Driver.FindElement(By.LinkText("(Back to Series)")));
                    PageLoadWait.WaitForHPPageLoad(20);
                    if (string.Equals(BasePage.Driver.FindElement(By.CssSelector("table#results tr.odd td:nth-child(7)")).Text, "1"))
                    {
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
        /// 106635 - View multi-frame data has lookup tables (LUT)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_106635(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string PatientID = string.Empty;
            try
            {
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientId");
                //Step 1: Load a multi-frame data which has lookup tables at image level
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                TestStep step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                bool thumbnail = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ThumbnailContainer());
                step.SetPath(testid, ExecutedSteps + 1, 2);
                bool studypanel = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (thumbnail && studypanel)
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
                //Step 2: Apply W/L, Gray Scale Inversion, Auto Window Level and Reset on the image
                viewer.SeriesViewer_1X1().Click();
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                bool WindowLevel = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                step.SetPath(testid, ExecutedSteps + 1, 2);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                bool Invert = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                step.SetPath(testid, ExecutedSteps + 1, 3);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AutoWindowLevel);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                bool AutoWindowLevel = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                step.SetPath(testid, ExecutedSteps + 1, 4);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                bool Reset = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (WindowLevel && Invert && AutoWindowLevel && Reset)
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
        /// 27919 - Localizer Lines
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27919(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string[] PatientID = null;
            try
            {
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientId")).Split('=');
                //PreCondition
                basepage.ChangeAttributeValue(Config.ImagerConfiguration, "/property[@key='LocalizerLineTextFontSize']", "value", "10.0");
                basepage.ChangeAttributeValue(Config.ImagerConfiguration, "/property[@key='LocalizerLineWidth']", "value", "1.0");
                servicetool.RestartIISUsingexe();
                //Step 1: Load the study Tumor_Left_Forearm___%321147 and set the view to 2 series, both with a 1x1 layout
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (viewer.SeriesViewPorts().Count == 2)
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
                //Step 2: Display Series 3 and 7 side by side. Select series 3 and enable the localizer line
                viewer.DragThumbnailToViewport(5, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 3: Scroll to image 1 of series 3 and use the zoom and W/L tools to compare the location of the localizer line on series 7 to figure 6
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 4: Scroll to image 6 of series 3 and use the zoom and W/L tools to compare the location of the localizer line on series 7 to figure 7.
                viewer.SeriesViewer_1X1().Click();
                for (int i = 0; i < 5; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }
                Thread.Sleep(5000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 5: Scroll to image 12 of series 3and use the zoom and W/L tools to compare the location of the localizer line on series 7to figure 8.
                viewer.SeriesViewer_1X1().Click();
                for (int i = 0; i < 6; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }
                Thread.Sleep(5000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 6: Load study ANON0000 (454-54-5454) and display all 4 series in a 2x2 layout
                viewer.CloseStudy();
                studies.SearchStudy(patientID: PatientID[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", PatientID[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (viewer.SeriesViewPorts().Count == 4)
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
                //Step 7: Select series 3 (SLAB MPR 1 | Recon 2: NECK) and enable the localizer line.
                viewer.SeriesViewer_2X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 8: Disable the localizer line, select series 3 (MPR 2 | Recon 2: NECK).
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(5000);
                viewer.SeriesViewer_1X2().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
        /// 68547 - Orientation for Hologic Breast Tomo and demographics
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_68547(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string[] Accession = null;
            int resultcount = 0;
            try
            {
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                //Step 1: Loin in iCA as Administrator
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;
                //Step 2: Load breast tomo study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 3: Check the horizontal orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(7, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(6, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                TestStep step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Right breasts should face Left. [Image left should be Anterior (A)]");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Left breasts should face Right. [Image left should be Posterior (P)]");
                }
                if (resultcount == 2)
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
                //Step 4: Check the vertical orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(3, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(7, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(6, Locators.ID.SeriesViewer3_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For ML views, Image top should be along the H (head).");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For CC views, Image top should 'Right (R)' for a right breast");
                }
                step.SetPath(testid, ExecutedSteps, 3);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X3()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For CC views, Image top should 'Left (L)' for a left breast.");
                }
                if (resultcount == 3)
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
                //Step 5: Check Orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(7, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(6, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(5, Locators.ID.SeriesViewer4_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(4, Locators.ID.SeriesViewer5_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("RCC - Image Top => Pateint 'R' And Image Left=> Patient 'A'");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LCC - Image Top => Pateint 'L' And Image Left=> Patient 'P'");
                }
                step.SetPath(testid, ExecutedSteps, 3);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("RMLO - Image Top => Pateint 'H' And Image Left=> Patient 'A'");
                }
                step.SetPath(testid, ExecutedSteps, 4);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LMLO - Image Top => Pateint 'H' And Image Left=> Patient 'P'");
                }
                if (resultcount == 4)
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
                //Step 6: CLose study. Load a regular MG study
                viewer.CloseStudy();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 7: Check the horizontal orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(11, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(10, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Right breasts should face Left. [Image left should be Anterior (A)]");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Left breasts should face Right. [Image left should be Posterior (P)]");
                }
                if (resultcount == 2)
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
                //Step 8: Check the vertical orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(11, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(10, Locators.ID.SeriesViewer3_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For ML views, Image top should be along the H (head).");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For CC views, Image top should 'Right (R)' for a right breast");
                }
                step.SetPath(testid, ExecutedSteps, 3);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X3()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For CC views, Image top should 'Left (L)' for a left breast.");
                }
                if (resultcount == 3)
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
                //Step 9: Check Orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(11, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(10, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(8, Locators.ID.SeriesViewer4_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(9, Locators.ID.SeriesViewer5_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("RCC - Image Top => Pateint 'R' And Image Left=> Patient 'A'");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LCC - Image Top => Pateint 'L' And Image Left=> Patient 'P'");
                }
                step.SetPath(testid, ExecutedSteps, 3);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("RMLO - Image Top => Pateint 'H' And Image Left=> Patient 'A'");
                }
                step.SetPath(testid, ExecutedSteps, 4);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LMLO - Image Top => Pateint 'H' And Image Left=> Patient 'P'");
                }
                if (resultcount == 4)
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
                //Step 10: Verify its demographics
                resultcount = 0;
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.Thumbnails()[1]))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("demographics shown for LCC");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.Thumbnails()[2]))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("demographics shown for LMLO");
                }
                if (resultcount == 2)
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
                //Step 11: Check the attributes from DICOM file
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(11, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 12: Close study Load a study(pid: AM - 0107 in FORENZA)
                viewer.CloseStudy();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.ActiveThumbnail()))
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
                //Step 13: Open service tool>Viewer>Miscellaneous Modify uncheck show thumbnail overlays  Apply Restart IIS
                login.Logout();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.UnSelectCheckBox("CB_ShowThumbnailOverlays");
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 14: Reload the study in step 10
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.ActiveThumbnail()))
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
                //Step 15: Revert the setting changes made for overlays
                login.Logout();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.SelectCheckBox("CB_ShowThumbnailOverlays");
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 16: Load a SC (with MG modality and monochrome PI) study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[3]);
                studies.SelectStudy("Accession", Accession[3]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 17: Check the horizontal orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(7, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(6, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Right breasts should face Left. [Image left should be Anterior (A)]");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Left breasts should face Right. [Image left should be Posterior (P)]");
                }
                if (resultcount == 2)
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
                //Step 18: Check the vertical orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(3, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(7, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(6, Locators.ID.SeriesViewer3_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For ML views, Image top should be along the H (head).");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For CC views, Image top should 'Right (R)' for a right breast");
                }
                step.SetPath(testid, ExecutedSteps, 3);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X3()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("For CC views, Image top should 'Left (L)' for a left breast.");
                }
                if (resultcount == 3)
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
                //Step 19: Check Orientation markers
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(7, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(6, Locators.ID.SeriesViewer2_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(5, Locators.ID.SeriesViewer4_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(4, Locators.ID.SeriesViewer5_2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("RCC - Image Top => Pateint 'R' And Image Left=> Patient 'A'");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LCC - Image Top => Pateint 'L' And Image Left=> Patient 'P'");
                }
                step.SetPath(testid, ExecutedSteps, 3);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("RMLO - Image Top => Pateint 'H' And Image Left=> Patient 'A'");
                }
                step.SetPath(testid, ExecutedSteps, 4);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("LMLO - Image Top => Pateint 'H' And Image Left=> Patient 'P'");
                }
                if (resultcount == 4)
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
                //Step 20: close study Load SC-MG study
                viewer.CloseStudy();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[4]);
                studies.SelectStudy("Accession", Accession[4]);
                viewer = StudyViewer.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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
                //Step 21: Verify its demographics
                resultcount = 0;
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.Thumbnails()[4]))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("demographics shown for LCC");
                }
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.Thumbnails()[0]))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("demographics shown for LMLO");
                }
                if (resultcount == 2)
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
                //Step 22: Check the attributes from DICOM file
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
        /// 27911 - Measurement and Display
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27911(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string[] PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
            string[] StudyUID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyUID")).Split('=');
            int resultcount = 0;
            bool SwitchBrowser = false;
            string BrowserType = Config.BrowserType;
            string VMSSA131 = string.Empty;
            string EA46 = string.Empty;
            try
            {
                VMSSA131 = login.GetHostName(Config.EA1);
                EA46 = login.GetHostName(Config.XDS_EA2);
                //Step 1: Load the study for patient AAXFFP.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: PatientName[0], Datasource: VMSSA131);
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
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
                //Step 2: Measure the width of the plastic blocks using the line measurement tool
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 376, 65, 230, 65);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 3: Measure the height of plastic blocks using the line measurement tool.
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 230, 211, 230, 65);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 4: Use Rectangular measure the outline (contour) of the plastic block.
                viewer.CloseStudy();
                if (!Config.BrowserType.ToLower().Contains("firefox"))
                {
                    SwitchBrowser = true;
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    Config.BrowserType = "firefox";
                    login.InvokeBrowser(Config.BrowserType);
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(LastName: PatientName[0], Datasource: VMSSA131);
                }
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                basepage.ClickElement(viewer.Rectangle());
                var action = new Actions(BasePage.Driver);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 230, 65).ClickAndHold();
                Thread.Sleep(3000);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 376, 211).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                //viewer.DrawRectangle(viewer.SeriesViewer_1X1(), 270, 52, 376, 158);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 5: Load the study for patient DOTS X and scroll to image 18
                if (SwitchBrowser)
                {
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    Config.BrowserType = BrowserType;
                    login.InvokeBrowser(Config.BrowserType);
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    studies = (Studies)login.Navigate("Studies");
                }
                else
                {
                    viewer.CloseStudy();
                }
                studies.SearchStudy(LastName: PatientName[1], Datasource: EA46);
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 6: Turn of the text overlay
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 7: Measure the horizontal and vertical distance between two points using the line measurement tool.
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                resultcount = 0;
                ExecutedSteps++;
                TestStep step = result.steps[ExecutedSteps];
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 233, 24, 215, 24);
                step.SetPath(testid, ExecutedSteps, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Horizontal Line Drawn");
                }
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 215, 42, 215, 24);
                step.SetPath(testid, ExecutedSteps, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Vertical Line Drawn");
                }
                if (resultcount == 2)
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
                //Step 8: Load the study SMPTE
                viewer.CloseStudy();
                studies.SearchStudy(LastName: PatientName[2]);
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
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
                //Step 9: Using the Angle measurement tool, place a point on the top left of the gray square next to '30%', followed by a point on the bottom left and bottom right of the same square.
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 128, 185, 128, 231, 178, 231);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 10: Using the Angle measurement tool, place a point on the top left of the gray square next to '10%', followed by a point on the bottom left and top right of the same square.
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 128, 305, 128, 351, 174, 305);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 11: Using the Angle measurement tool, three points along any line in the image.
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 128, 305, 180, 305, 222, 305);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 12: Using the Cobb Angle measurement tool, place a line along the side of any square followed by a line along the bottom of any square.
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.DrawCobbAngle(viewer.SeriesViewer_1X1(), 128, 305, 128, 351, 178, 351, 228, 351);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 13: Using the Cobb Angle measurement tool, place a line along the side of any square followed by a line from the top left to bottom right of any square to the right of and above the previous.
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.DrawCobbAngle(viewer.SeriesViewer_1X1(), 128, 305, 128, 351, 185, 245, 235, 295);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 14: Using the Cobb Angle measurement tool, place a line along the bottom of any square followed by another line along the bottom of any other
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.DrawCobbAngle(viewer.SeriesViewer_1X1(), 128, 355, 185, 355, 235, 355, 285, 355);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
        /// 27915 - Validation of Position of GSPS
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27915(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession"));
            string DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));
            try
            {
                //Step 1: Transfer the dataset Chest CT with GSPS to a data source that does not have Chest CT
                BasePage.RunBatchFile(Config.batchfilepath, DicomPath + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: login.GetHostName(Config.DestinationPACS));
                if (studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession }) != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2: Load the study for patient Chest CT with GSPS. Select the PR series for series 7566 and change layout to 2x2. Only show the first four images of this series and compare the location of the annotations
                studies.SelectStudy("Accession", Accession);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
        /// 27912 - Mean Value
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27912(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = new StudyViewer();
            string[] PatientName = null;
            string[] StudyUID = null;
            string BrowserType = Config.BrowserType;
            try
            {
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                StudyUID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyUID")).Split('=');
                //Step 1: Load the study for patient AAXFFP
                if (!Config.BrowserType.ToLower().Contains("firefox"))
                {
                    Config.BrowserType = "firefox";
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: PatientName[0], Datasource: login.GetHostName(Config.EA1));
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
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
                //Step 2: Measure the MEAN pixel value for AIR using the ROI measurement tool. Note: Air is the pitch black region on the film
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                var action = new Actions(BasePage.Driver);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 460, 125).ClickAndHold();
                Thread.Sleep(3000);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 580, 125).Release().Build().Perform();
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Air = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (Air)
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
                //Step 3: Measure the MIN pixel value for AIR using the ROI measurement tool
                if (Air)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4: Measure the MAX pixel value for AIR using the ROI measurement tool.
                if (Air)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: Measure the MEAN pixel value for Delrin using the ROI measurement tool. Note: Delrin is the white region on the film
                viewer.CloseStudy();
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                action = new Actions(BasePage.Driver);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 475, 225).ClickAndHold();
                Thread.Sleep(3000);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 580, 225).Release().Build().Perform();
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Delrin = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (Delrin)
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
                //Step 6: Measure the MIN pixel value for Delrin using the ROI measurement tool.
                if (Delrin)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7: Measure the MAX pixel value for Delrin using the ROI measurement tool.
                if (Delrin)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8: Load the study SMPTE and using the ROI tool outline one of the rectangular gradients.
                viewer.CloseStudy();
                studies.SearchStudy(LastName: PatientName[1], Datasource: login.GetHostName(Config.XDS_EA2));
                studies.ChooseColumns(new String[] { "Study UID" });
                studies.SelectStudy("Study UID", StudyUID[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                action = new Actions(BasePage.Driver);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 609, 283).ClickAndHold();
                Thread.Sleep(3000);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 679, 283).Release().Build().Perform();
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Gradient = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (Gradient)
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
                if (!string.Equals(Config.BrowserType, BrowserType))
                {
                    Config.BrowserType = BrowserType;
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }
            }
        }

        /// <summary>
        /// 27918 - IHE magnification compliance for Mammography images
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27918(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = new Studies();
            StudyViewer viewer = new StudyViewer();
            string Filepath = string.Empty;
            string[] FullPath = null;
            string[] Accession = null;
            try
            {
                Filepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                FullPath = Directory.GetFiles(Filepath, "*.*", SearchOption.AllDirectories);
                Accession = FullPath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                var client = new DicomClient();
                foreach (string path in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestinationPACS, 104, false, "SCU", Config.DestinationPACSAETitle);
                }
                //Step 1: Load the study Test PixelSpacing and measure the length of the line on the each image from all three series.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 2:  Measure the length of the line in the images.
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 3: Load DX-Series103 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 4: Load CR-Series104 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 5: Load XA-Series105 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[3], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[3]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 6: Load MG-Series202 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[4], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[4]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 7: Load DX-Series203 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[5], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[5]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 8: Load CR-Series204 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[6], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[6]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 9: Load XA-Series205 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[7], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[7]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 10: Load MG-Series301 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[8], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[8]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 11: Load MG-Series302 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[9], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[9]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 12: Load DX-Series303 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[10], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[10]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 13: Load DX-Series304 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[11], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[11]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 14: Load DX-Series305 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[12], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[12]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 15: Load MG-Series306 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[13], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[13]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 16: Load MG-Series307 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[14], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[14]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 17: Load DX-Series308 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[15], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[15]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 18: Load DX-Series309 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[16], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[16]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 19: Load DX-Series310 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[17], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[17]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 20: Load DX-Series315 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[18], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[18]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 21: Load DX-Series316 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[19], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[19]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 22: Load DX-Series317 and measure the length of the line in the image.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[20], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Accession", Accession[20]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 726, 467, 500, 467);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
        /// 27913 - Non-square pixel data testing
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27913(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = new Studies();
            StudyViewer viewer = new StudyViewer();
            string DicomPath = string.Empty;
            string Accession = null;
            string[] PatientName = null;
            string P10Cache = string.Empty;
            string StudyID = string.Empty;
            string TempFile = string.Empty;
            string CopyFile = string.Empty;
            try
            {
                DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split(',');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession"));
                StudyID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID"));
                P10Cache = string.Concat(@"C:\Windows\Temp\WebAccessP10FilesCache\", Environment.MachineName, @"\");
                var client = new DicomClient();
                for (int i = 1; i <= 3; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27913-", i)));
                    client.Send(Config.DestinationPACS, 104, false, "SCU", Config.DestinationPACSAETitle);
                }
                DirectoryInfo di = new DirectoryInfo(P10Cache);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                servicetool.RestartIISUsingexe();
                //Step 1: 
                /*Ensure the study for patient CR HI - RES is in one of the data sources.Load the study and make a horizontal and a vertical line measurement on two well defined points as show in the Figure 1.Note the measurement values, e.g.L - h = 2053.8 and L - v = 2196.9
                Add the imager_pixel_spacing attribute as follows: Exit iConnect Access and open the P10 file for CR HI-RES with DicomToolBox.In DicomToolBox, select Element / Insert New and select Data type"DS", enter tag numbers under Tag(0018, 1164), set the value to"1\2"and save.*/
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: PatientName[0], FirstName: PatientName[1], studyID: StudyID);
                studies.ChooseColumns(new String[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 857, 40, 365, 40);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 857, 40, 857, 562);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool Measurement = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                viewer.CloseStudy();
                login.Logout();
                string[] Files = di.GetFiles().Select(file => file.FullName).ToArray();
                bool length = Files.Length == 1;
                CopyFile = string.Concat(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar, "File0");
                TempFile = string.Concat(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar, Path.GetFileName(Files[0]));
                if (File.Exists(CopyFile))
                {
                    File.Delete(CopyFile);
                }
                File.Copy(Files[0], TempFile, true);
                FileInfo currentFile = new FileInfo(TempFile);
                currentFile.MoveTo(CopyFile);
                BasePage.WriteDicomFile(CopyFile, new DicomTag[] { DicomTag.ImagerPixelSpacing }, new String[] { @"1\2" }, Path.GetFileName(TempFile));
                File.Copy(TempFile, string.Concat(P10Cache, Path.GetFileName(TempFile)), true);
                if (Measurement && length)
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
                //Step 2: Log back in to iConnect Access and reload the study
                Thread.Sleep(20000);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: PatientName[0], FirstName: PatientName[1], studyID: StudyID);
                studies.ChooseColumns(new String[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 3: Perform the same line measurements as before.
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 1100, 40, 128, 40);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 1100, 40, 1100, 562);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 4: Draw an angle so that the horizontal is twice the verticle
                viewer.CloseStudy();
                PageLoadWait.WaitForFrameLoad(20);
                studies.ChooseColumns(new String[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 128, 562, 1100, 562, 128, 40);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 5: Exit iConnect Access and via DicomToolBox modify the imager_pixel_spacing attribute (0018,1164) to"2\1".
                login.Logout();
                if (File.Exists(TempFile))
                {
                    File.Delete(TempFile);
                }
                BasePage.WriteDicomFile(CopyFile, new DicomTag[] { DicomTag.ImagerPixelSpacing }, new String[] { @"2\1" }, Path.GetFileName(TempFile));
                File.Copy(TempFile, string.Concat(P10Cache, Path.GetFileName(TempFile)), true);
                ExecutedSteps++;
                //Step 6: Log back in to iConnect Access and reload the study.
                Thread.Sleep(20000);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: PatientName[0], FirstName: PatientName[1], studyID: StudyID);
                studies.ChooseColumns(new String[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 7: Perform the same line measurements as before
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 734, 40, 488, 40);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 734, 40, 734, 562);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 8: Exit iConnect Access and via DicomToolBox clear the value of imager_pixel_spacing attribute 0018, 1164. Set the pixel_spacing attribute (0028, 0030) to"1\2".
                login.Logout();
                if (File.Exists(TempFile))
                {
                    File.Delete(TempFile);
                }
                BasePage.WriteDicomFile(CopyFile, new DicomTag[] { DicomTag.PixelSpacing }, new String[] { @"1\2" }, Path.GetFileName(TempFile));
                File.Copy(TempFile, string.Concat(P10Cache, Path.GetFileName(TempFile)), true);
                ExecutedSteps++;
                //Step 9: Log back in to iConnect Access and reload the study.
                Thread.Sleep(20000);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: PatientName[0], FirstName: PatientName[1], studyID: StudyID);
                studies.ChooseColumns(new String[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 10: Perform the same line measurements as before.
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 1100, 40, 128, 40);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 1100, 40, 1100, 562);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 11: Exit iConnect Access and via DicomToolBox modify the pixel_spacing attribute (0028,0030) to"2\1".
                login.Logout();
                if (File.Exists(TempFile))
                {
                    File.Delete(TempFile);
                }
                BasePage.WriteDicomFile(CopyFile, new DicomTag[] { DicomTag.PixelSpacing }, new String[] { @"2\1" }, Path.GetFileName(TempFile));
                File.Copy(TempFile, string.Concat(P10Cache, Path.GetFileName(TempFile)), true);
                ExecutedSteps++;
                //Step 12: Log back in to iConnect Access and reload the study.
                Thread.Sleep(20000);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: PatientName[0], FirstName: PatientName[1], studyID: StudyID);
                studies.ChooseColumns(new String[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 13: Perform the same line measurements as before.
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 734, 40, 488, 40);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 734, 40, 734, 562);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 14: Ensure the study for patient Test PixelSpacing is in one of the data sources.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession);
                Dictionary<String, String> results = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
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
                //Step 15: Load the study Test PixelSpacing and measure the length of the line on the each image from all three series.
                studies.SelectStudy("Accession", Accession);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SeriesViewer_1X1().Click();
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 274, 300, 120, 300);
                viewer.ToolBarSetAllInOneTool();
                viewer.SeriesViewer_1X2().Click();
                viewer.DrawLine(viewer.SeriesViewer_1X2(), 274, 300, 120, 300);
                viewer.ToolBarSetAllInOneTool();
                viewer.SeriesViewer_1X3().Click();
                viewer.DrawLine(viewer.SeriesViewer_1X3(), 274, 300, 120, 300);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewPortContainer()))
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
        /// 27917 - Inconsistant Pixel Spacing
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27917(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            StudyViewer viewer = null;
            string[] Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
            string DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));
            string PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientId"));
            int resultcount = 0;
            string BrowserType = Config.BrowserType;
            bool switchbrowser = false;
            try
            {
                //Step 1: 
                /* Modify image 3 of AAXFFP to have a pixel spacing (0028, 0030) of"0.2441406250\0.2441406250"and send it to one of the data sources.
                    Locate or modify another study that contains multiple series with one containing images with differing pixel spacing (by at least 0.001) and send it to one of the data sources. (Note: The first 2 images should have the same pixel spacing). */

                var client = new DicomClient();
                for (int i = 1; i <= 10; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(DicomPath, "27917-", i)));
                    client.Send(Config.DestinationPACS, 104, false, "SCU", Config.DestinationPACSAETitle);
                }
                ExecutedSteps++;
                //Step 2: Load AAXFFP and display series 2 with a 1x1 layout. Ensure the scope is set to series.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (viewer.SeriesViewPorts().Count == 1)
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
                //Step 3: Apply the zoom and pan tools to the first image.
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForFrameLoad(20);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 2, h / 2, w / 2, h / 2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 4: Scroll to the second image
                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 5: Scroll to the third image which has a pixel spacing that is not consistent with the first image's.
                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 6: Apply any tool.
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForFrameLoad(20);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 7: Apply the zoom and pan tools to a couple of images
                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForFrameLoad(20);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForFrameLoad(20);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 2, h / 2, w / 2, h / 2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                TestStep step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Zoom and Pan applied for image4");
                }
                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForFrameLoad(20);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForFrameLoad(20);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 2, h / 2, w / 2, h / 2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Zoom and Pan applied for image5");
                }
                if (resultcount == 2)
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
                //Step 8: Using the line measurement tool, measure the width and height of the white box on image 3
                viewer.CloseStudy();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                for (int i = 0; i < 2; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 759, 107, 545, 107);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 545, 107, 545, 321);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 9: Using the Rectangle tool, measure the area of the white box
                viewer.CloseStudy();
                if (!Config.BrowserType.ToLower().Contains("firefox"))
                {
                    switchbrowser = true;
                    login.Logout();
                    Config.BrowserType = "firefox";
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy("Accession", Accession[0]);
                }
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                for (int i = 0; i < 2; i++)
                {
                    PageLoadWait.WaitForFrameLoad(20);
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }
                PageLoadWait.WaitForFrameLoad(20);
                basepage.ClickElement(viewer.Rectangle());
                var action = new Actions(BasePage.Driver);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 545, 107).ClickAndHold();
                Thread.Sleep(3000);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 759, 321).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 10: Place an angle measurement starting from the top left of the white box, to the top right, and ending in the bottom left.
                viewer.CloseStudy();
                if (switchbrowser)
                {
                    switchbrowser = false;
                    login.Logout();
                    Config.BrowserType = BrowserType;
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy("Accession", Accession[0]);
                }
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                for (int i = 0; i < 2; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }
                PageLoadWait.WaitForFrameLoad(20);
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 545, 107, 759, 107, 545, 321);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 11: Place a cobb angle measure on with one line along the left of the white box and the other along the bottom.
                viewer.CloseStudy();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                for (int i = 0; i < 2; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }
                PageLoadWait.WaitForFrameLoad(20);
                viewer.DrawCobbAngle(viewer.SeriesViewer_1X1(), 545, 107, 545, 321, 652, 321, 759, 321);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 12: Scroll to image 1 and using the line measurement tool, measure the width and height of the white box.
                viewer.ClickUpArrowbutton(1, 1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.ClickUpArrowbutton(1, 1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 764, 105, 547, 105);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 547, 105, 547, 322);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 13: Using the Rectangle tool, measure the area of the white box
                viewer.CloseStudy();
                if (!Config.BrowserType.ToLower().Contains("firefox"))
                {
                    switchbrowser = true;
                    login.Logout();
                    Config.BrowserType = "firefox";
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy("Accession", Accession[0]);
                }
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                basepage.ClickElement(viewer.Rectangle());
                action = new Actions(BasePage.Driver);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 270, 52).ClickAndHold();
                Thread.Sleep(3000);
                action.MoveToElement(viewer.SeriesViewer_1X1(), 376, 158).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 14: Place an angle measurement starting from the top left of the white box, to the top right, and ending in the bottom left.
                viewer.CloseStudy();
                if (switchbrowser)
                {
                    switchbrowser = false;
                    login.Logout();
                    Config.BrowserType = BrowserType;
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy("Accession", Accession[0]);
                }
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawAngleMeasurement(viewer.SeriesViewer_1X1(), 547, 105, 764, 105, 547, 322);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 15: Place a cobb angle measure on with one line along the left of the white box and the other along the bottom.
                viewer.CloseStudy();
                studies.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DrawCobbAngle(viewer.SeriesViewer_1X1(), 193, 134, 193, 174, 233, 174, 273, 174);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 16: Load the second study with differeing pixel spacing that contains multiple series. Display the series that contains image's with differing pixel spacing.
                viewer.CloseStudy();
                studies.SearchStudy(AccessionNo: Accession[1]);
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
                //Step 17: Set the layout to 1x2 and apply the zoom and pan tools to the first image.
                resultcount = 0;
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForFrameLoad(20);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Zoom and Pan applied for image1");
                }
                viewer.ClickDownArrowbutton(1, 1);
                step.SetPath(testid, ExecutedSteps + 1, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Zoom and Pan applied for image2");
                }
                if (resultcount == 2)
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
                //Step 18: Change the layout to 1x1.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (viewer.SeriesViewPorts().Count == 1)
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
                //Step 19: Scroll to the image right before the first one with a pixel spacing that is not consistent with the first image's is and change the layout to one with multiple images
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 20: Apply any tool.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 21: Apply the zoom and pan tools to a couple of images.
                resultcount = 0;
                viewer.CloseStudy();
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Zoom and Pan applied for image1");
                }
                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Zoom and Pan applied for image2");
                }
                if (resultcount == 2)
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
                //Step 22: Load a different series and apply the zoom and pan tools.
                viewer.CloseStudy();
                studies.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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
                //Step 23: Load study Abdomen CT (patient ID 1205937) into the viewer.
                viewer.CloseStudy();
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = StudyViewer.LaunchStudy();
                if (viewer.SeriesViewPorts().Count == 4)
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
                //Step 24: Using the Zoom, Pan and Window level tool, ensure that all the edges of the image are clearly visible in the viewport. Using the line measurement tool, measure the dimensions of the image.
                resultcount = 0;
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewport1 = viewer.SeriesViewer_1X1();
                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;
                step = result.steps[ExecutedSteps];
                step.SetPath(testid, ExecutedSteps + 1, 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Zoom, Pan and Windlow Level applied");
                }
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 376, 52, 270, 52);
                viewer.DrawLine(viewer.SeriesViewer_1X1(), 200, 158, 200, 52);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                step.SetPath(testid, ExecutedSteps + 1, 2);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Line Drawn on Images");
                }
                if (resultcount == 2)
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
    }
}
