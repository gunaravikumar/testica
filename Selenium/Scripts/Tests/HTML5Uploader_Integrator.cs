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
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using Application = TestStack.White.Application;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using TestStack.White.Configuration;
using System.Windows.Automation;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Selenium.Scripts.Pages.eHR;
using System.Net;

namespace Selenium.Scripts.Tests
{
    class HTML5Uploader_Integrator : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public Web_Uploader webuploader { get; set; }
        public RanorexObjects rnxobject { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public HTML5_Uploader html5 { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        string FolderPath = "";
        public EHR ehr { get; set; }
        public ExamImporter ei { get; set; }
        Process batchprocess;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public HTML5Uploader_Integrator(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            rnxobject = new RanorexObjects();
            webuploader = new Web_Uploader();
            html5 = new HTML5_Uploader();
            ehr = new EHR();
            ei = new ExamImporter();
        }

        /// <summary>
        /// HTML5Uploader_Integrator - Uploading DICOM stud(y)ies to Destination Via Integrator
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161151(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            ServiceTool servicetool = new ServiceTool();
            Outbounds outbounds = null;
            Inbounds inbounds = null;
            DomainManagement domainmanagement = null;
            UserPreferences userpreferences = null;           
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            Random random = new Random();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                String UploadFilePathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] UploadFilePath = UploadFilePathList.Split('|');
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 134115: " + new Random().Next(1, 10000);
                String TestPatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestPatientDetails");
                String[] TestPatientDetails = TestPatientDetailsList.Split(':');

                String TestdomainB = "Test4115_DomainB_" + GetUniqueDomainID();
                String TestdomainAdminB = "Test4115_DomainAdminB_" + GetUniqueUserId();
                String TestrolePhy = "Test4115_RolePhy_" + GetUniqueRole();
                String TestroleArch = "Test4115_RoleArch_" + GetUniqueRole();
                String TestroleStaf = "Test4115_RoleStaf_" + GetUniqueRole();
                String TestuserPhy = "Test4115_UserPhy_" + GetUniqueUserId();
                String TestuserArch = "Test4115_UserArch_" + GetUniqueUserId();
                String TestuserStaf = "Test4115_UserStaf_" + GetUniqueUserId();
                string[] datasource = null;
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.CreateDomain(TestdomainB, TestdomainAdminB, datasources: datasource);
                try { domainmanagement.ClickSaveNewDomain(); } catch (Exception) { }
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainB, TestrolePhy, 1, GrantAccess: 99);               
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainB, TestroleArch, 2, GrantAccess: 99);
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainB, TestroleStaf, 0, GrantAccess: 99);
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.CreateUser(TestuserPhy, TestdomainB, TestrolePhy);
                usermanagement.CreateUser(TestuserArch, TestdomainB, TestroleArch);
                usermanagement.CreateUser(TestuserStaf, TestdomainB, TestroleStaf);
                String Dest = "Dest-1" + random.Next(1, 1000);
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination destination = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                PageLoadWait.WaitForFrameLoad(20);
                //destination.CreateDestination(GetHostName(Config.DestinationPACS), TestuserPhy, TestuserArch, Dest, domain: TestdomainB);
                destination.AddDestination(TestdomainB, Dest, login.GetHostName(Config.DestinationPACS), TestuserPhy, TestuserArch);
                login.Logout();

                //Pre-condition: 
                login.RestartIISUsingexe();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled", shadowuser: "Always Enabled");
                wpfobject.WaitTillLoad();
                try
                {
                    if (servicetool.modifyBtn().Enabled)
                        servicetool.ClickModifyButton();
                }
                catch { }
                wpfobject.SelectCheckBox("CB_AllowShowSelector");
                wpfobject.SelectCheckBox("CB_AllowShowSelectorSearch");
                servicetool.ApplyEnableFeatures();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Step-1: Login to iCA as Administrator.
                login.LoginIConnect(TestdomainB, TestdomainB);
                PageLoadWait.WaitForFrameLoad(20);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.LogoutBtn()));
                if (login.LogoutBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 2: Navigate to Domain Management tab then edit the Image sharing assigned domain (SuperAdminGroup) and ensure that webuploader is selected by default from the drop down option of "Default Uploader"

                domainmanagement = login.Navigate<DomainManagement>();
                //domainmanagement.SearchDomain(Config.adminGroupName);
                //domainmanagement.SelectDomain(Config.adminGroupName);
                //domainmanagement.EditDomainButton().Click();
                //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                string step2 = domainmanagement.DefaultUploaderDropdown().SelectedOption.Text;
                if (step2.Equals("Web Uploader"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step3: Ensure " Consent for Web Uploader " option is disabled by default and enable the option

                bool step3 = !domainmanagement.WebUploaderConsentCheckbox().Selected;
                if (!domainmanagement.WebUploaderConsentCheckbox().Selected)
                {
                    //domainmanagement.WebUploaderConsentCheckbox().Click();
                    ClickElement(domainmanagement.WebUploaderConsentCheckbox());
                }
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4: Logout from Administrator and login as"st"user to iCA

                login.Logout();
                login.LoginIConnect(TestuserStaf, TestuserStaf);
                PageLoadWait.WaitForFrameLoad(20);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.LogoutBtn()));
                if (login.LogoutBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 5: Verify User preferences page that Webuploader is selected by default from the drop down option of "Default Uploader"

                userpreferences = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                string step4 = userpreferences.DefaultUploaderList().SelectedOption.Text;
                if (step4.Equals("Web Uploader"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step6: Logout from"st"user

                userpreferences.CloseUserPreferences();
                login.Logout();
                if (login.LoginBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 7: Launch TestEHR application from iCA server and navigate to Launch Exam Importer tab

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

                //step 8: "Enter the following: 
                //Address = http://server Ip/WebAccess
                //User ID = st
                //Enable User Sharing = True
                //Auto End Session = True
                //Auth Provider = ByPass
                //Destination = Configured destination nameEmail Address: Blank
                //Phone number: Blank
                //Other fields are set to default

                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Launch Exam Importer");
                ehr.SetCommonParameters(user: TestuserStaf, usersharing: "True", AuthProvider: "Bypass", destination: Dest, domain: TestdomainB, ExamList: null);
                result.steps[++ExecutedSteps].status = "Pass";

                //step 9: "Fill in the patient contents:
                //Patient Name:""Test01""
                //Patient ID:""ID01""
                //Patient DOB:""12 / 12 / 2000""
                //Issuer of PatientID: ""Test_EHR""
                //Patient Gender:""M""

                string Patient_Name = TestPatientDetails[0];
                string IssuerofPatientID = TestPatientDetails[3];
                string Patient_ID = TestPatientDetails[1];

                ehr.SetSearchKeys_LaunchExamImporter("Patient_Name", Patient_Name);
                ehr.SetSearchKeys_LaunchExamImporter("IssuerofPatientID", IssuerofPatientID);
                ehr.SetSearchKeys_LaunchExamImporter("Patient_DOB", TestPatientDetails[2]);
                ehr.SetSearchKeys_LaunchExamImporter("Patient_ID", Patient_ID);
                ehr.SetSearchKeys_LaunchExamImporter("Patient_Gender", TestPatientDetails[4]);
                result.steps[++ExecutedSteps].status = "Pass";

                //step 10: Click on cmd. Copy and paste the generated URL to any HTML5 supported browser.

                String url_1 = ehr.clickCmdLine();
                ehr.CloseEHR();
                ehr.NavigateToIntegratorURL(url_1);
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame");
                if (html5.HippaComplianceLabel().Displayed && html5.UsernameDisplayed().Displayed)
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

                //step 11: Ensure that "Continue" button is not enabled in the HIPPA compliance page

                if (!html5.HippaContinueBtn().Enabled)
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

                //step 12: Check in " I read and understood the agreement. I agree to comply to it " and click on "Continue" button

                if (!html5.HippaAgreeChkBox().Selected)
                {
                    html5.HippaAgreeChkBox().Click();
                }
                PageLoadWait.WaitForElement(html5.By_HippaContinueBtn(), WaitTypes.Clickable);
                html5.HippaContinueBtn().Click();
                if (html5.UploadFilesBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 13: Ensure that patient demographics received through integrator URL are pre populated in HTML5 uploader screen

                bool step12_1 = html5.newPatientNameSpan()[0].Text.Equals(Patient_Name);
                bool step12_2 = html5.newPatientMrnValueSpan()[0].Text.Equals(Patient_ID);
                bool step12_3 = html5.newPatientIpIdValueSpan()[0].Text.Equals(IssuerofPatientID);
                bool step12_4 = html5.newPatientGenderLbSpan()[0].Text.Equals(TestPatientDetails[5]);
                bool step12_5 = html5.newPatientDobValueSpan()[0].Text.Equals(TestPatientDetails[6]);
                bool step12_6 = html5.newPatientDemographicsContainer().Displayed;
                bool step12_7 = html5.newPatientDemographicsLabelSpan()[0].Text.Equals("New Patient Demographics");
                if (step12_1 && step12_2 && step12_3 && step12_4 & step12_5 && step12_6 && step12_7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14: Click on " Upload File " button and select a DICOM file

                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(UploadFilePath[0], "file");
                bool step13 = html5.UploadJobContainer().Displayed;
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
                PageLoadWait.WaitForHTML5StudyToUpload();

                //step 15: Click on "SHARE JOB #1" button after uploading

                html5.ShareJobButton().Click();
                Thread.Sleep(1000);
                if (html5.DestinationDropdown().Options.Count >= 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 16: Ensure that the study's demographics details are updated with the demographics details sent via TestEHR

                bool step15_1 = (html5.PatientNameonSharePage().Text == Patient_Name);
                bool step15_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step15_3 = (html5.StudyDetailsonSharePage()[1].Text == TestPatientDetails[7]);
                bool step15_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 0);
                if (step15_1 && step15_2 && step15_3 && step15_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 17: Ensure that the destination given in the Launch Exam importer tab is selected from "To" by default

                bool step16 = html5.DestinationDropdown().SelectedOption.Text.Equals(Dest);
                if (step16 /*check for disabled dest*/)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 18: Choose any Priority from the Priority dropdown list.

                html5.PriorityDropdown().SelectByText(Priority);
                if (html5.PriorityDropdown().SelectedOption.Text == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 19: Add some comments optionally for the uploaded studies in Comment section

                html5.CommentTextBox().SendKeys(Comments);
                if (html5.CommentTextBox().GetAttribute("value").ToString().Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 20: Click on " SUBMIT " button to send the study to selected destination

                html5.ShareBtn().Click();
                Thread.Sleep(1000);
                if (html5.DragFilesDiv().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 21: Ensure that the upload JOB #1 has been removed from the display list once the upload is completed successfully

                ExecutedSteps++;

                //step 22: Logout from"st"user

                html5.Logout_HTML5Uploader();
                if (html5.SignInBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 23: Login as"ph"user and go to Inbounds page, Search for the exams sent from HTML5 Uploader in iCA

                login.DriverGoTo(login.url);
                login.LoginIConnect(TestuserPhy, TestuserPhy);
                inbounds = login.Navigate<Inbounds>();
                login.SelectAllInboundData();
                login.SearchStudy("Patient ID", Patient_ID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step21 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { Patient_ID, "Uploaded" });
                if (step21 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 24: Load the study on viewer and Verify that the study details are matched with the uploaded one.

                login.SelectStudy("Patient ID", Patient_ID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step24 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(Patient_Name.ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(Patient_ID.ToLower()) && step24)
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

                //step 25: Logout"ph"user and login as"st"user.

                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(TestuserStaf, TestuserStaf);
                ExecutedSteps++;

                //step 26: Go to outbounds page, Search for the exams sent from HTML5 Uploader in iCA

                outbounds = login.Navigate<Outbounds>();
                login.SelectAllOutboundData();
                login.SearchStudy("Patient ID", Patient_ID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step26 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { Patient_ID, "Uploaded" });
                if (step26 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 27: Load the study on viewer and Verify that the study details are matched with the uploaded one.

                login.SelectStudy("Patient ID", Patient_ID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step27)
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

                //step 28: Logout"st"user from iCA

                bluringviewer.CloseBluRingViewer();
                login.Logout();
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                //login.Logout();

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
