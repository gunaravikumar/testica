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
using OpenQA.Selenium.Remote;
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
using TestStack.White.UIItems.WindowItems;
using Application = TestStack.White.Application;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TestStack.White.Configuration;
using System.Windows.Automation;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.iCAInstaller;

namespace Selenium.Scripts.Tests
{
    class HTML5Uploader : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public Web_Uploader webuploader { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public RanorexObjects rnxobject { get; set; }
        public HTML5_Uploader html5 { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        string FolderPath = "";
        public EHR ehr { get; set; }
        public iCAInstaller icainstaller { get; set; }
        public ServiceTool servicetool { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public HTML5Uploader(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            html5 = new HTML5_Uploader();
            ehr = new EHR();
        }

        /// <summary>
        /// HTML5Uploader - Upload DICOM stud(y)ies to Destination from inbounds page
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163440(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Patient1Details = PatientDetailsList.Split('=')[0].Split('|');
                String[] Patient2Details = PatientDetailsList.Split('=')[1].Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 133833: " + new Random().Next(1, 10000);
                String Reason = "Reason test case# 133833: " + new Random().Next(1, 10000);

                //Step-1: Login as Administrator to iCA from HTML5 supported browser
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //html5.Login_HTML5Uploader("ph", "ph");
                //html5.Logout_HTML5Uploader();
                //html5.Login_HTML5Uploader(email: "Saqib.dadan@cc.com", phonenumber: "123456789");
                //html5.Logout_HTML5Uploader();

                //Step-2: Navigate to Domain Management tab then edit the Image sharing assigned domain (SuperAdminGroup)
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                bool step2 = domain.DefaultUploaderDropdown().SelectedOption.Text.Equals("Web Uploader");

                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3:  Verify the "consent for web uploader" option is disabled by default and enable the option
                bool step3 = domain.WebUploaderConsentCheckbox().Selected;
                if (!domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
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

                //Step-4: Logout from Administrator and login as "st" user from HTML5 supported browser
                login.Logout();
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-5: Verify User preferences page that Webuploader is selected by default from the drop down option of "Default Uploader"
                userpref = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool step4 = userpref.DefaultUploaderDropdown().SelectedOption.Text.Equals("Web Uploader");
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
                userpref.CloseUserPreferences();

                //Step-6: Go to Inbounds page, Click on "Upload" button from the bottom of the page
                inbounds = login.Navigate<Inbounds>();
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT();
                PageLoadWait.WaitForFrameLoad(10);
                bool step5 = PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), WaitTypes.Visible).Displayed;
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

                //Step-7: Check in "I read and understood the agreement. I agree and complete it" and click on " Continue " button
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

                //Step-8: Ensure that UPLOAD FILE(S), UPLOAD FOLDER and DRAG & DROP YOUR FILES OR FOLDERS HERE FOR NEW UPLOAD options are available 
                if (html5.UploadFilesBtn().Displayed && html5.UploadFolderBtn().Displayed && html5.DragFilesDiv().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Click Upload folder and browse file and select upload
                html5.UploadFolderBtn().Click();
                Thread.Sleep(5000);
                UploadFileInBrowser(UploadFilePath);

                bool step8_1 = html5.UploadJobContainer().Displayed;
                bool step8_2 = html5.UploadJobProgressBar().Displayed;
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

                //Step-10: Ensure that uploaded patient and study info are viewable
                //Pending files uploadbar viewable
                bool uploadbar = html5.UploadJobContainer().Displayed;
                //Patient and study Info viewable
                bool patientinfo = html5.PatientInfoContainer().Displayed;
                if (uploadbar && patientinfo)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload(120);
                //Check if Job 1 is selected/highlighted post upload
                bool step10 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
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

                //Step-12: Ensure that all the studies are listed in the study panel with patient and Study info
                //Patient 1:
                String TempAge, TempYrs;
                TempAge = TempYrs = String.Empty;
                //Update for UI change for Age and Years span separation
                bool step11_1_patientname = (html5.PatientNameSpan()[0].Text == Patient1Details[0]);
                bool step11_1_DOB = (html5.DOBSpan()[0].Text == Patient1Details[1]);
                if (!String.IsNullOrEmpty(Patient1Details[2]))
                {
                    TempAge = Patient1Details[2].Split(' ')[0];
                    TempYrs = Patient1Details[2].Split(' ')[1];
                }
                bool step11_1_Age = (html5.AgeSpan()[0].Text == TempAge);
                bool step11_1_YrsTxt = (html5.YearsSpan()[0].Text == TempYrs);
                bool step11_1_Gender = (html5.GenderSpan()[0].Text == Patient1Details[3]);
                bool step11_1_MRN = (html5.MRNSpan()[0].Text == Patient1Details[4]);
                bool step11_1_IPID = (html5.IPIDSpan()[0].Text == Patient1Details[5]);
                bool step11_1_StudyDesc = (html5.StudyDescSpan()[0].Text == Patient1Details[6]);
                bool step11_1_Modality = (html5.ModalitySpan()[0].Text == Patient1Details[7]);
                bool step11_1_Date = (html5.DateSpan()[0].Text == Patient1Details[8]);
                bool step11_1_Series = (html5.SeriesSpan()[0].Text == Patient1Details[9]);
                bool step11_1_Image = (html5.ImagesSpan()[0].Text == Patient1Details[10]);
                bool step11_1_Accession = (html5.AccessionSpan()[0].Text == Patient1Details[11]);
                bool step11_1_Institution = (html5.InstitutionSpan()[0].Text == Patient1Details[12]);
                bool step11_1_RefPhys = (html5.RefPhysSpan()[0].Text == Patient1Details[13]);
                //Patient 2:
                bool step11_2_patientname = (html5.PatientNameSpan()[1].Text == Patient2Details[0]);
                bool step11_2_DOB = (html5.DOBSpan()[1].Text == Patient2Details[1]);
                if (!String.IsNullOrEmpty(Patient2Details[2]))
                {
                    TempAge = Patient2Details[2].Split(' ')[0];
                    TempYrs = Patient2Details[2].Split(' ')[1];
                }
                else
                {
                    TempAge = TempYrs = String.Empty;
                }
                bool step11_2_Age = (html5.AgeSpan()[1].Text == TempAge);
                bool step11_2_YrsTxt = (html5.YearsSpan()[1].Text == TempYrs);
                bool step11_2_Gender = (html5.GenderSpan()[1].Text == Patient2Details[3]);
                bool step11_2_MRN = (html5.MRNSpan()[1].Text == Patient2Details[4]);
                bool step11_2_IPID = (html5.IPIDSpan()[1].Text == Patient2Details[5]);
                bool step11_2_StudyDesc = (html5.StudyDescSpan()[1].Text == Patient2Details[6]);
                bool step11_2_Modality = (html5.ModalitySpan()[1].Text == Patient2Details[7]);
                bool step11_2_Date = (html5.DateSpan()[1].Text == Patient2Details[8]);
                bool step11_2_Series = (html5.SeriesSpan()[1].Text == Patient2Details[9]);
                bool step11_2_Image = (html5.ImagesSpan()[1].Text == Patient2Details[10]);
                bool step11_2_Accession = (html5.AccessionSpan()[1].Text == Patient2Details[11]);
                bool step11_2_Institution = (html5.InstitutionSpan()[1].Text == Patient2Details[12]);
                bool step11_2_RefPhys = (html5.RefPhysSpan()[1].Text == Patient2Details[13]);
                Logger.Instance.InfoLog("step11_1_patientname: " + step11_1_patientname + "step11_1_DOB:" + step11_1_DOB + "step11_1_Age: " + step11_1_Age + "step11_1_Gender: " + step11_1_Gender + "step11_1_MRN: " + step11_1_MRN + "step11_1_IPID: " + step11_1_IPID + "step11_1_StudyDesc :" + step11_1_StudyDesc + "step11_1_Modality: " + step11_1_Modality + "step11_1_Date: " + step11_1_Date + "step11_1_Series: " + step11_1_Series + "step11_1_Image: " + step11_1_Image + "step11_1_Accession: " + step11_1_Accession + "step11_1_Institution: " + step11_1_Institution + "step11_1_RefPhys: " + step11_1_RefPhys + " step11_1_YrsTxt: " + step11_1_YrsTxt);
                Logger.Instance.InfoLog("step11_2_patientname: " + step11_2_patientname + "step11_2_DOB:" + step11_2_DOB + "step11_2_Age: " + step11_2_Age + "step11_2_Gender: " + step11_2_Gender + "step11_2_MRN: " + step11_2_MRN + "step11_2_IPID: " + step11_2_IPID + "step11_2_StudyDesc :" + step11_2_StudyDesc + "step11_2_Modality: " + step11_2_Modality + "step11_2_Date: " + step11_2_Date + "step11_2_Series: " + step11_2_Series + "step11_2_Image: " + step11_2_Image + "step11_2_Accession: " + step11_2_Accession + "step11_2_Institution: " + step11_2_Institution + "step11_2_RefPhys: " + step11_2_RefPhys + " step11_2_YrsTxt: " + step11_2_YrsTxt);
                if (step11_1_patientname && step11_1_DOB && step11_1_Age && step11_1_YrsTxt && step11_1_Gender && step11_1_MRN && step11_1_IPID && step11_1_StudyDesc && step11_1_Modality && step11_1_Date && step11_1_Series && step11_1_Image && step11_1_Accession && step11_1_Institution && step11_1_RefPhys && step11_2_patientname && step11_2_DOB && step11_2_Age && step11_2_YrsTxt && step11_2_Gender && step11_2_MRN && step11_2_IPID && step11_2_StudyDesc && step11_2_Modality && step11_2_Date && step11_2_Series && step11_2_Image && step11_2_Accession && step11_2_Institution && step11_2_RefPhys)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Ensure that scroll bar is available in the study panel
                Driver.Manage().Window.Size = new Size(1000, 700);
                Thread.Sleep(2000);
                bool step12 = html5.IsVerticalScrollBarPresent(html5.PatientInfoContainer());
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
                Driver.Manage().Window.Maximize();

                //Step-14: Ensure that "DELETE JOB# 1" and "SHARE JOB# 1" buttons are enabled
                if (html5.DeleteJobButton().Displayed && html5.ShareJobButton().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Ensure that "Delete" option is available on right side corner of all the listed studies in the study panel
                bool step14 = false;
                foreach (IWebElement item in html5.DeleteButtons())
                {
                    if (item.FindElement(By.XPath("..")).GetCssValue("text-align").Equals("right"))
                    {
                        step14 = true;
                    }
                    else
                    {
                        step14 = false;
                        break;
                    }
                }
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

                //Step-16: Ensure that attachment option is available below Study Info of all the studies listed
                bool step15_1 = (html5.DeleteButtons().Count == html5.AttachmentButtons().Count);
                bool step15_2 = false;
                foreach (IWebElement item in html5.AttachmentButtons())
                {
                    if (item.Displayed)
                    {
                        step15_2 = true;
                    }
                    else
                    {
                        step15_2 = false;
                        break;
                    }
                }
                if (step15_1 && step15_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17 & 18: Click on "Attachment" button of any one of the study listed & Browse Non Dicom objects and click on "Select" button
                //Clicking first attachment button
                html5.AttachmentButtons()[0].Click();
                Thread.Sleep(5000);
                UploadFileInBrowser(AttachmentFilePath, "file");
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-19: Wait until the progress bar gets completed and ensure that the Non Dicom objects are listed
                PageLoadWait.WaitForHTML5StudyToUpload();
                int ImageCount, SeriesCount;
                SeriesCount = Convert.ToInt32(Patient1Details[9]) + 1;
                ImageCount = Convert.ToInt32(Patient1Details[10]) + 1;
                if (html5.SeriesSpan()[0].Text == SeriesCount.ToString() && html5.ImagesSpan()[0].Text == ImageCount.ToString())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20: Click on "SHARE JOB# 1" button from the Upload exam to iConnnect Access page
                html5.ShareJobButton().Click();
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

                //Step-21: Ensure that all the studies uploaded in the Upload exam to iConnect Access page are available with attachments in patient Content summary details
                //Patient 1:
                bool step20_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                bool step20_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step20_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                bool step20_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                //Patient 2: 
                bool step20_5 = (html5.PatientNameonSharePage(1).Text == Patient2Details[0]);
                bool step20_6 = (html5.StudyDetailsonSharePage(1)[0].Text == StudyCount);
                bool step20_7 = (html5.StudyDetailsonSharePage(1)[1].Text == Patient2Details[10]);
                bool step20_8 = (Convert.ToInt32(html5.StudyDetailsonSharePage(1)[2].Text) == 0);
                Logger.Instance.InfoLog("step20_1: " + step20_1 + "step20_2:" + step20_2 + "step20_3: " + step20_3 + "step20_4: " + step20_4 + "step20_5: " + step20_5 + "step20_6: " + step20_6 + "step20_7 :" + step20_4 + "step20_8: " + step20_8);
                if (step20_1 && step20_2 && step20_3 && step20_4 && step20_5 && step20_6 && step20_7 && step20_8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-22: Select the study upload destination from the "To" dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Choose any Priority from the Priority dropdown list.
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

                //Step-24: Add some comments optionally for the uploaded study in Comments section.
                html5.CommentTextBox().SendKeys(Comments);
                if (GetTextFromTextBox("id", "textAreaComments").Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25: Click on"SHARE"button to send the study to selected destination
                html5.ShareBtn().Click();
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

                //Step-26: Logout from"st"user
                html5.Logout_HTML5Uploader();
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-27: Login as "ph" user and go to Inbounds page
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();
                Thread.Sleep(90000); //Sleep for study to reach inbounds - Big study
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step26 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
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

                //Step-28: Verify the priority information for the exams sent from HTML5 uploader
                String step27;
                step26.TryGetValue("Priority", out step27);
                if (step27 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30: Verify that the attached Image and report are loaded on the viewer.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step29 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step29)
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

                //Step-31: Logout from "ph" and login as "st" user.
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-32: Go to outbounds page, Search for the exams sent from HTML5 Uploader in iCA
                outbounds = login.Navigate<Outbounds>();
                login.ClearFields(2);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                ChooseColumns(new string[] { "Priority" });
                Dictionary<string, string> step31 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step31 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33: Verify the priority information for the exams sent from HTML5 uploader
                String step32;
                step31.TryGetValue("Priority", out step32);
                if (step32 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35: Verify that the attached Image and report are loaded on the viewer.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step34 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step34)
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

                //Step-36: Logout from "st" user and login as "ph" user
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(phuser, phpassword);
                ExecutedSteps++;

                //Step-37: Go to inbounds page then select any uploaded study from the studies list and click on"Nominate for archive"button
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                inbounds.SelectStudy("Patient ID", PatientID);
                inbounds.NominateForArchive(Reason);
                ExecutedSteps++;

                //Step-38: Logout from "ph" and login as "ar" user.
                login.Logout();
                login.LoginIConnect(aruser, arpassword);
                ExecutedSteps++;

                //Step-39: Verify the priority information for the exams sent from HTML5 uploader
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step38_value = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Nominated For Archive" });
                String step38;
                step38_value.TryGetValue("Priority", out step38);
                if (step38 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-40: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step40 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()) && step40)
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

                //Step-41: Close the study viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-42 & 43: Select the study which was nominated for archive and click on archive button
                inbounds.SelectStudy1(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                ExecutedSteps++;
                login.Logout();
                login.LoginIConnect(aruser, arpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                Thread.Sleep(120000);    /// 120 seconds sleep for waiting for Routing completed
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step42 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Routing Completed" });
                if (step42 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-44: Logout as"ar"user from iCA
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
        /// HTML5Uploader - Adding more stud(y)ies to the existing Uploaded JOB
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163441(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Patient1Details = PatientDetailsList.Split('|');
                //String[] Patient2Details = PatientDetailsList.Split('=')[1].Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 133836: " + new Random().Next(1, 10000);
                String Reason = "Reason test case# 133836: " + new Random().Next(1, 10000);

                //Pre-condition: Ensure the "consent for web uploader" option is disabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Login to iCA as "st" user from HTML5 supported browser.
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-2: Go to Inbounds page, Click on "Upload" button from the bottom of the page
                inbounds = login.Navigate<Inbounds>();
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT();
                PageLoadWait.WaitForFrameLoad(10);
                bool step2 = PageLoadWait.WaitForElement(html5.By_UploadFilesBtn(), WaitTypes.Visible).Displayed;
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Click on "UPLOAD FILE", browse multiple Dicom studies from different patient and click on "SELECT" button
                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(UploadFilePath, "file");

                bool step4_1 = html5.UploadJobContainer().Displayed;
                if (step4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                //Check if Job 1 is selected/highlighted post upload
                bool step5 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
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

                //Step-5: Ensure that all the studies are listed in the study panel
                //Patient 1:
                bool step6_1_patientname = (html5.PatientNameSpan()[0].Text == Patient1Details[0]);
                bool step6_1_DOB = (html5.DOBSpan()[0].Text == Patient1Details[1]);
                //Update for UI change for Age and Years span separation
                String TempAge, TempYrs;
                TempAge = TempYrs = String.Empty;
                if (!String.IsNullOrEmpty(Patient1Details[2]))
                {
                    TempAge = Patient1Details[2].Split(' ')[0];
                    TempYrs = Patient1Details[2].Split(' ')[1];
                }
                bool step6_1_Age = (html5.AgeSpan()[0].Text == TempAge);
                bool step6_1_YrsTxt = (html5.YearsSpan()[0].Text == TempYrs);
                bool step6_1_Gender = (html5.GenderSpan()[0].Text == Patient1Details[3]);
                bool step6_1_MRN = (html5.MRNSpan()[0].Text == Patient1Details[4]);
                bool step6_1_IPID = (html5.IPIDSpan()[0].Text == Patient1Details[5]);
                bool step6_1_StudyDesc = (html5.StudyDescSpan()[0].Text == Patient1Details[6]);
                bool step6_1_Modality = (html5.ModalitySpan()[0].Text == Patient1Details[7]);
                bool step6_1_Date = (html5.DateSpan()[0].Text == Patient1Details[8]);
                bool step6_1_Series = (html5.SeriesSpan()[0].Text == Patient1Details[9]);
                bool step6_1_Image = (html5.ImagesSpan()[0].Text == Patient1Details[10]);
                bool step6_1_Accession = (html5.AccessionSpan()[0].Text == Patient1Details[11]);
                bool step6_1_Institution = (html5.InstitutionSpan()[0].Text == Patient1Details[12]);
                bool step6_1_RefPhys = (html5.RefPhysSpan()[0].Text == Patient1Details[13]);
                Logger.Instance.InfoLog("step6_1_patientname: " + step6_1_patientname + "step6_1_DOB:" + step6_1_DOB + "step6_1_Age: " + step6_1_Age + " step6_1_YrsTxt: " + step6_1_YrsTxt + " step6_1_Gender: " + step6_1_Gender + "step6_1_MRN: " + step6_1_MRN + "step6_1_IPID: " + step6_1_IPID + "step6_1_StudyDesc :" + step6_1_StudyDesc + "step6_1_Modality: " + step6_1_Modality + "step6_1_Date: " + step6_1_Date + "step6_1_Series: " + step6_1_Series + "step6_1_Image: " + step6_1_Image + "step6_1_Accession: " + step6_1_Accession + "step6_1_Institution: " + step6_1_Institution + "step6_1_RefPhys: " + step6_1_RefPhys);
                if (step6_1_patientname && step6_1_DOB && step6_1_Age && step6_1_YrsTxt && step6_1_Gender && step6_1_MRN && step6_1_IPID && step6_1_StudyDesc && step6_1_Modality && step6_1_Date && step6_1_Series && step6_1_Image && step6_1_Accession && step6_1_Institution && step6_1_RefPhys)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Ensure that + Add File(S), + Add Folder and Drag & Drop options are available under JOB# 1 in left panel
                if (html5.AddFiles().Displayed && html5.AddFolder().Displayed && html5.DragDropMessage().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7-8: Drag and Drop a dicom Folder to upload few more studies on JOB# 1
                //Not Automated since drag and drop from file browser needs to be researched, to be implemented once a solution is found
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-9: Ensure that attachment option is available below Study Info of all the studies listed
                bool step10_1 = (html5.DeleteButtons().Count == html5.AttachmentButtons().Count);
                bool step10_2 = false;
                foreach (IWebElement item in html5.AttachmentButtons())
                {
                    if (item.Displayed)
                    {
                        step10_2 = true;
                    }
                    else
                    {
                        step10_2 = false;
                        break;
                    }
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

                //Step-10 & 11: Click on "Attachment" button of any one of the study listed & Browse Non Dicom objects and click on "Select" button
                //Clicking first attachment button
                html5.AttachmentButtons()[0].Click();
                UploadFileInBrowser(AttachmentFilePath, "file");
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-12: Wait until the progress bar gets completed and ensure that the Non Dicom objects are listed
                PageLoadWait.WaitForHTML5StudyToUpload();
                int ImageCount, SeriesCount;
                SeriesCount = Convert.ToInt32(Patient1Details[9]) + 1;
                ImageCount = Convert.ToInt32(Patient1Details[10]) + 1;
                if (html5.SeriesSpan()[0].Text == SeriesCount.ToString() && html5.ImagesSpan()[0].Text == ImageCount.ToString())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Click on "SHARE JOB# 1" button from the Upload exam to iConnnect Access page
                html5.ShareJobButton().Click();
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

                //Step-14: Ensure that all the studies uploaded in the Upload exam to iConnect Access page are available with attachments in patient Content summary details
                //Patient 1:
                bool step15_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                bool step15_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step15_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                bool step15_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
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


                //Step-15: Select the study upload destination from the "To" dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Choose any Priority from the Priority dropdown list.
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

                //Step-17: Add some comments optionally for the uploaded study in Comments section.
                html5.CommentTextBox().SendKeys(Comments);
                if (GetTextFromTextBox("id", "textAreaComments").Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18: Click on"SHARE"button to send the study to selected destination
                html5.ShareBtn().Click();
                if (PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), WaitTypes.Visible, 60).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19: Logout from"st"user
                html5.Logout_HTML5Uploader();
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20: Login as "ph" user and go to Inbounds page
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();
				login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step21 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
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

                //Step-21: Verify the priority information for the exams sent from HTML5 uploader
                String step22;
                step21.TryGetValue("Priority", out step22);
                if (step22 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Verify that the attached Image and report are loaded on the viewer.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step23 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step23)
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

                //Step-24: Logout from "ph" and login as "st" user.
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-25: Go to outbounds page, Search for the exams sent from HTML5 Uploader in iCA
                outbounds = login.Navigate<Outbounds>();
                login.ClearFields(2);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                ChooseColumns(new string[] { "Priority" });
                Dictionary<string, string> step26 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
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

                //Step-26: Verify the priority information for the exams sent from HTML5 uploader
                String step27;
                step26.TryGetValue("Priority", out step27);
                if (step27 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-27: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28: Verify that the attached Image and report are loaded on the viewer.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step28 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step28)
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

                //Step-29: Logout from "st" user 
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                ExecutedSteps++;

                //Report Result
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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// HTML5Uploader - Attaching Non DICOM objects to exisiting uploaded stud(y)ies
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163442(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
            result = new TestCaseResult(stepcount);
            DomainManagement domain = null;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String phuser = Config.ph1UserName;
                String phpassword = Config.phPassword;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] UploadFilePath = UploadFilePathList.Split('>');
                String AttachmentFilePathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String[] AttachmentFilePath = AttachmentFilePathList.Split('>');
                String[] PatientDetails = PatientDetailsList.Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 133838: " + new Random().Next(1, 10000);
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");

                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (!domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //step 1: Launch iCA application in HTML5 supported browser and click on " Web upload " option from iCA login page

                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("login");
                PageLoadWait.WaitForFrameLoad(10);
                bool step1 = PageLoadWait.WaitForElement(html5.By_SignInBtn(), WaitTypes.Visible).Displayed;
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 2: User clicks on Registered user option from HTML5 uploader login screen and clicks on " Log in " button after entering st user's credentials

                html5.RegisteredUserRadioBtn().Click();
                html5.UserNameTxtBox().SendKeys(stuser);
                html5.PasswordTxtBox().SendKeys(stpassword);
                html5.SignInBtn().Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (html5.HippaComplianceLabel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 3: User checks the terms and agreements checkbox in the HIPAA compliance page and clicks on "Continue" button

                html5.HippaAgreeChkBox().Click();
                html5.HippaContinueBtn().Click();
                if (html5.UploadFilesBtn().Displayed && html5.UploadFolderBtn().Displayed && html5.DragFilesDiv().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4: Click on "Upload File" button from the uploader screen

                html5.UploadFilesBtn().Click();
                ExecutedSteps++;

                //step 5: Select multiple dicom files from any local drives and click on " Open " on the"Select dicom file"dialog window

                UploadFileInBrowser(UploadFilePath[0], "file");
                bool step4_1 = html5.UploadJobContainer().Displayed;
                //bool step4_3 = html5.CancelJobBtn().Displayed;
                if (step4_1)
                {
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

                //step 6: Select JOB #1 from the upload job panel after upload progress has been completed and click on Attachment button for any patient from the study grid

                html5.AttachmentButtons()[0].Click();
                ExecutedSteps++;

                //step 7: Select supported non-dicom objects (i.e JPG, JPEG, PNG, TIFF, TIF, BMP & PDF) from any local drives and click on " Open " in the dialog window

                //bool step7 = false;
                UploadFileInBrowser(AttachmentFilePath[0], "file");
                //step7 = html5.UploadJobProgressBar().Displayed;
                //if (step7)
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
                ExecutedSteps++;
                PageLoadWait.WaitForHTML5StudyToUpload();

                //step 8: Verify the uploaded non-dicom objects for the patients from the study grid after the upload progress has been completed
                int ImageCount = Convert.ToInt32(PatientDetails[5]);
                if (html5.SeriesSpan()[0].Text == PatientDetails[4] && html5.ImagesSpan()[0].Text == (ImageCount + 5).ToString())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 9: Click on "SHARE JOB #1" button from the upload main page

                html5.ShareJobButton().Click();
                Thread.Sleep(500);
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

                //step 10: Verify the Content summary details displayed for that particular job

                bool step10_1 = (html5.PatientNameonSharePage().Text == PatientDetails[0]);
                bool step10_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step10_3 = (html5.StudyDetailsonSharePage()[1].Text == PatientDetails[5]);
                bool step10_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 5);
                if (step10_1 && step10_2 && step10_3 && step10_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11: User clicks on destination selection dropdown

                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12: User clicks on Priority dropdown

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

                //step 13: User enters comments for the job

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

                //step 14: User clicks on " SUBMIT " button

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

                //step 15: User navigates to st user's outbounds page from another browser

                html5.Logout_HTML5Uploader();
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.DriverGoTo(login.url);
                login.LoginIConnect(stuser, stpassword);
                outbounds = login.Navigate<Outbounds>();
                outbounds.SelectAllOutboundData();
                login.SearchStudy("Patient ID", PatientDetails[1]);
                PageLoadWait.WaitForSearchLoad();
                outbounds.ChooseColumns(new string[] { "Priority", "# Images", "Comments" });
                Dictionary<string, string> step15 = outbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientDetails[1], "Uploaded" });
                if (step15 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 16: Verify the priority and comments for the uploaded studies

                String step16_1, step16_2;
                step15.TryGetValue("Priority", out step16_1);
                step15.TryGetValue("Comments", out step16_2);
                if (step16_1 == Priority && step16_2 == Comments)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 17: Verify the count of patients, studies and number of images associated with each study

                int step17 = GetSearchResults().Count;
                string step17_1, step17_2;
                step15.TryGetValue("# Images", out step17_1);
                string[] step17_nmbr = step17_1.Split('/');
                step15.TryGetValue("Patient Name", out step17_2);
                if (step17 == 1 && step17_nmbr[0] == (ImageCount + 5).ToString() && step17_2 == PatientDetails[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 18: Load uploaded studies from HTML5 uploader in iCA viewer

                login.SelectStudy("Patient ID", PatientDetails[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(PatientDetails[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientDetails[1].ToLower()) && step18)
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
                bluringviewer.CloseBluRingViewer();

                //step 19: Verify the status of the uploaded studies from HTML5 uploader

                Dictionary<string, string> step19 = outbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientDetails[1], "Uploaded" });
                if (step19 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 20: User logs out as st user and log in as ph user

                login.Logout();
                login.LoginIConnect(phuser, phpassword);
                ExecutedSteps++;

                //step 21: Navigate to Inbounds page

                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllInboundData();
                ChooseColumns(new string[] { "Priority", "Comments", "# Images" });
                login.SearchStudy("Patient ID", PatientDetails[1]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step21 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientDetails[1], "Uploaded" });
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

                //step 22: Verify the priority and comments for the uploaded studies

                String step22_1, step22_2;
                step21.TryGetValue("Priority", out step22_1);
                step21.TryGetValue("Comments", out step22_2);
                if (step22_1 == Priority && step22_2.Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 23: Verify the count of patients, studies and number of images associated with each study

                int step23 = GetSearchResults().Count;
                string step23_1, step23_2;
                step21.TryGetValue("# Images", out step23_1);
                string[] step23_nmbr = step23_1.Split('/');
                step21.TryGetValue("Patient Name", out step23_2);
                if (step23 == 1 && step23_nmbr[0] == (ImageCount + 5).ToString() && step23_2 == PatientDetails[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 24: Verify the status of the uploaded studies from HTML5 uploader

                Dictionary<string, string> step24 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientDetails[1], "Uploaded" });
                if (step24 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 25: Verify the count of studies and number of images associated with each study

                if (step23 == 1 && step23_nmbr[0] == (ImageCount + 5).ToString() && step23_2 == PatientDetails[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 26: Load uploaded studies from HTML5 uploader in iCA viewer

                login.SelectStudy("Patient ID", PatientDetails[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step26 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(PatientDetails[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientDetails[1].ToLower()) && step26)
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
                bluringviewer.CloseBluRingViewer();

                //step 27: Logout all users from every browser

                login.Logout();
                ExecutedSteps++;

                //Report Result
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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// HTML5Uploader - Uploading Non DICOM objects after creating new patient
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163443(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String BlankFolder = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "BlankFolder");
                String MultipleUploadFiles = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MultipleUploadFiles");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Message = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Message");
                String[] PatientDetails = PatientDetailsList.Split(':');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 133842: " + new Random().Next(1, 10000);

                //Pre-condition: Ensure the "consent for web uploader" option is enabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (!domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Login to iCA as "st" user 
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-2: Go to Inbounds page, Click on "Upload" button from the bottom of the page
                inbounds = login.Navigate<Inbounds>();
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT();
                PageLoadWait.WaitForFrameLoad(10);
                bool step2 = PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), WaitTypes.Visible).Displayed;
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Check in "I read and understood the agreement. I agree to comply to it" and click on "Continue"button
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

                //Step-4: Click on "UPLOAD FOLDER", browse for an empty folder or folder that has not supported non dicom object files and click on "SELECT" button
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                {
                    //Step marked as No Automation for IE because folder selection is not allowed in IE - On selecting blank folder, it asks to select further files
                    try
                    {
                        if (BasePage.Driver.FindElement(By.CssSelector("div#divUploadFolder")).Displayed)
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                        }
                    }
                    catch (Exception) { result.steps[++ExecutedSteps].status = "Pass"; }
                }
                else if(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                {
                    html5.UploadFolderBtn().Click();
                    UploadFileInBrowser(BlankFolder);
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
                }
                else
                {
                    html5.UploadFolderBtn().Click();
                    UploadFileInBrowser(BlankFolder);
                    BasePage.wait.Until<Boolean>(d=> d.FindElement(By.CssSelector("div#ModalDialogDiv")).
                    GetAttribute("style").ToLower().Contains("display: none;")==false);
                    var isMessageDisplayed = BasePage.Driver.FindElement(By.CssSelector("div#ModalDialogDiv>div:nth-of-type(1)>div")).GetAttribute("innerHTML").
                        Equals("Did not find any supported files to upload, please select another folder.");
                    BasePage.Driver.FindElement(By.CssSelector("div#OkButtonDiv")).Click();
                    if (isMessageDisplayed)
                    {
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

                //Step-5: Click on "UPLOAD FOLDER", browse a folder which has at least one supported Non Dicom files and click on "SELECT" button
                html5.UploadFolderBtn().Click();
                UploadFileInBrowser(UploadFilePath);
                //Did not find Patient data, do you want to create a patient and associate attachments with it?
                if (html5.ModalDialogDiv().Displayed && html5.ModalMessageDiv().Text == Message)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Click Cancel
                html5.ModalCancelBtn().Click();
                if (!PageLoadWait.WaitForElement(html5.By_ModalDialogDiv(), WaitTypes.Invisible).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-7: Click on "UPLOAD FOLDER" again to browse the same folder and click on "SELECT" button
                html5.UploadFolderBtn().Click();
                UploadFileInBrowser(UploadFilePath);
                //Did not find Patient data, do you want to create a patient and associate attachments with it?
                if (html5.ModalDialogDiv().Displayed && html5.ModalMessageDiv().Text == Message)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Click on " OKAY " button to continue -  TBD
                html5.ModalOkBtn().Click();
                bool step8 = html5.UploadJobContainer().Displayed;
                if (step8 && html5.FamilynameTxtBox().Displayed && html5.FirstnameTxtBox().Displayed && html5.BirthdateTxtBox().Displayed && html5.MRNTxtBox().Displayed && html5.DescriptionTxtBox().Displayed && html5.GenderListBox().SelectedOption.Displayed && html5.InstitutionTxtBox().Displayed && html5.RefPhysTxtBox().Displayed && html5.NewPatientPhoneTxtBox().Displayed && html5.NewPatientEmailTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Ensure that "Save" button is disabled without entering values for the mandatory fields like Family Name, First Name, DOB, MRN and Description
                //Checking if all elements are blank
                bool step9 = (html5.FamilynameTxtBox().GetAttribute("value") == "") && (html5.FirstnameTxtBox().GetAttribute("value") == "") && (html5.BirthdateTxtBox().GetAttribute("value") == "") && (html5.MRNTxtBox().GetAttribute("value") == "") && (html5.DescriptionTxtBox().GetAttribute("value") == "");
                if (step9 && html5.SaveBtn().GetAttribute("class").Contains("disabledButtonClass"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Click on "DELETE JOB #1" to drop the JOB #1
                html5.DeleteJobBtn().Click();
                if (!IsElementVisible(html5.By_UploadJobContainer()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Click on "UPLOAD FILES" to browse and select multiple Non Dicom files
                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(MultipleUploadFiles, "file");
                //Did not find Patient data, do you want to create a patient and associate attachments with it?
                if (html5.ModalDialogDiv().Displayed && html5.ModalMessageDiv().Text == Message)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Click on " OKAY " button to continue
                html5.ModalOkBtn().Click();
                bool step12 = html5.UploadJobContainer().Displayed;
                if (step12 && html5.FamilynameTxtBox().Displayed && html5.FirstnameTxtBox().Displayed && html5.BirthdateTxtBox().Displayed && html5.MRNTxtBox().Displayed && html5.DescriptionTxtBox().Displayed && html5.GenderListBox().SelectedOption.Displayed && html5.InstitutionTxtBox().Displayed && html5.RefPhysTxtBox().Displayed && html5.NewPatientPhoneTxtBox().Displayed && html5.NewPatientEmailTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Enter values for all the mandatory fields (Family Name, First Name, DOB, MRN and Description) and verify that the save button is enabled
                html5.FamilynameTxtBox().SendKeys(PatientDetails[0]);
                html5.FirstnameTxtBox().SendKeys(PatientDetails[1]);
                html5.BirthdateTxtBox().SendKeys(PatientDetails[2]);
                html5.MRNTxtBox().SendKeys(PatientDetails[3]);
                html5.DescriptionTxtBox().SendKeys(PatientDetails[4]);
                if (!html5.SaveBtn().GetAttribute("class").Contains("disabledButtonClass"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Remove the value from any one of the mandatory fields and verify that the save button is disabled
                //html5.DescriptionTxtBox().Clear();  //Does not work - as clear is removing the mandatory field validator
                html5.DescriptionTxtBox().SendKeys(Keys.Control + "a");
                html5.DescriptionTxtBox().SendKeys(Keys.Delete);
                if (html5.SaveBtn().GetAttribute("class").Contains("disabledButtonClass"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Enter values for all the fields and verify that the save button is enabled
                html5.DescriptionTxtBox().SendKeys(PatientDetails[4]);
                html5.GenderListBox().SelectByText(PatientDetails[5]);
                html5.InstitutionTxtBox().SendKeys(PatientDetails[6]);
                html5.RefPhysTxtBox().SendKeys(PatientDetails[7]);
                if (!html5.SaveBtn().GetAttribute("class").Contains("disabledButtonClass"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Enter Valid Email address and Phone number [Phone number must contain minimum 10 digits and allowed special characters are '.','-','+','(',')','x' ]
                html5.NewPatientPhoneTxtBox().SendKeys(PatientDetails[8]);
                html5.NewPatientEmailTxtBox().SendKeys(PatientDetails[9]);
                if (html5.NewPatientPhoneTxtBox().GetAttribute("value").Equals(PatientDetails[8]) && html5.NewPatientEmailTxtBox().GetAttribute("value").Equals(PatientDetails[9]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Click on "Save" button
                html5.SaveBtn().Click();
                ExecutedSteps++;

                //Step-18: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                bool s17_patientname = (html5.PatientNameSpan()[0].Text == PatientDetails[0] + ", " + PatientDetails[1]);
                bool s17_DOB = (html5.DOBSpan()[0].Text == PatientDetails[2]);
                bool s17_Gender = (html5.GenderSpan()[0].Text == PatientDetails[5]);
                bool s17_MRN = (html5.MRNSpan()[0].Text == PatientDetails[3]);
                bool s17_StudyDesc = (html5.StudyDescSpan()[0].Text == PatientDetails[4]);
                bool s17_Series = (html5.SeriesSpan()[0].Text == PatientDetails[10]);
                bool s17_Image = (html5.ImagesSpan()[0].Text == PatientDetails[11]);
                bool s17_Institution = (html5.InstitutionSpan()[0].Text == PatientDetails[6]);
                bool s17_RefPhys = (html5.RefPhysSpan()[0].Text == PatientDetails[7]);
                bool s17_Phone = (html5.StudyPhoneTxtBox()[0].GetAttribute("value") == PatientDetails[8]);
                bool s17_Email = (html5.StudyEmailTxtBox()[0].GetAttribute("value") == PatientDetails[9]);
                Logger.Instance.InfoLog("s17_patientname: " + s17_patientname + "s17_DOB:" + s17_DOB + "s17_Gender: " + s17_Gender + "s17_MRN: " + s17_MRN + "s17_StudyDesc :" + s17_StudyDesc + "s17_Series: " + s17_Series + "s17_Image: " + s17_Image + "s17_Institution: " + s17_Institution + "s17_RefPhys: " + s17_RefPhys + s17_Image + "s17_Phone: " + s17_Phone + "s17_Email: " + s17_Email);
                if (s17_patientname && s17_DOB && s17_Gender && s17_MRN && s17_StudyDesc && s17_Series && s17_Image && s17_Institution && s17_RefPhys && s17_Phone && s17_Email)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19: Ensure that attachment option is available below Study Info of the created study
                bool step18_1 = (html5.DeleteButtons().Count == html5.AttachmentButtons().Count);
                bool step18_2 = false;
                foreach (IWebElement item in html5.AttachmentButtons())
                {
                    if (item.Displayed)
                    {
                        step18_2 = true;
                    }
                    else
                    {
                        step18_2 = false;
                        break;
                    }
                }
                if (step18_1 && step18_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20 & 21: Click on "Attachment" button of any one of the study listed & Browse Non Dicom objects and click on "Select" button
                //Clicking first attachment button
                html5.AttachmentButtons()[0].Click();
                UploadFileInBrowser(AttachmentFilePath, "file");
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-22: Wait until the progress bar gets completed and ensure that the Non Dicom objects are listed
                PageLoadWait.WaitForHTML5StudyToUpload();
                int ImageCount, SeriesCount;
                SeriesCount = Convert.ToInt32(PatientDetails[10]) + 1;
                ImageCount = Convert.ToInt32(PatientDetails[11]) + 1;
                if (html5.SeriesSpan()[0].Text == SeriesCount.ToString() && html5.ImagesSpan()[0].Text == ImageCount.ToString())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Click on "SHARE JOB #1" button below the upload main page
                html5.ShareJobButton().Click();
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

                //Step-24: Ensure that all the studies uploaded in the Upload exam to iConnect Access page are available with attachments in patient Content summary details
                //Patient 1:
                bool step23_1 = (html5.PatientNameonSharePage().Text == PatientDetails[0] + ", " + PatientDetails[1]);
                bool step23_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step23_3 = (html5.StudyDetailsonSharePage()[1].Text == "0");
                bool step23_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == ImageCount);
                Logger.Instance.InfoLog("step23_1: " + step23_1 + " step23_2: " + step23_2 + " step23_3: " + step23_3 + " step23_4: " + step23_4);
                if (step23_1 && step23_2 && step23_3 && step23_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-25: Select the study upload destination from the "To" dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26: Choose any Priority from the Priority dropdown list.
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

                //Step-27: Add some comments optionally for the uploaded study in Comments section.
                html5.CommentTextBox().SendKeys(Comments);
                if (GetTextFromTextBox("id", "textAreaComments").Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28: Click on " SUBMIT " button to send the study to selected destination
                html5.ShareBtn().Click();
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

                //Step-29: Logout from"st"user
                html5.Logout_HTML5Uploader();
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30: Login as "ph" user and go to Inbounds page, Search for the exams sent from HTML5 Uploader in iCA
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step29 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step29 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                if (Patientinfo.ToLower().Contains(PatientDetails[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32: Verify that the attached Image and report are loaded on the viewer.
                result.steps[++ExecutedSteps].status = "Not Automated";     //Image comparison fails every time since study date and time changes on every run

                //Step-33: Logout from "ph" and login as "st" user.
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-34: Go to outbounds page, Search for the exams sent from HTML5 Uploader in iCA
                outbounds = login.Navigate<Outbounds>();
                login.ClearFields(2);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step33 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step33 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                if (Patientinfo.ToLower().Contains(PatientDetails[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-36: Verify that the attached Image and report are loaded on the viewer.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-37: Logout from "st" user 
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
        /// HTML5Uploader - Adding only Non DICOM files/Folder to existing Uploaded JOB
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163444(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String MultipleUploadFiles = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MultipleUploadFiles");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String StudyInstanceUIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyInstanceUID");
                String Message = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Message");
                String[] Patient1Details = PatientDetailsList.Split('=')[0].Split('|');
                String[] Patient2Details = PatientDetailsList.Split('=')[1].Split('|');
                String[] StudyInstanceUID = StudyInstanceUIDList.Split('=');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 133846: " + new Random().Next(1, 10000);

                //Deleting all files and folders from folder "C:\Windows\Temp\AcceptedFolder"
                try { DeleteAllFileFolder(Config.HTML5UploaderAcceptedPath); } catch (Exception ex) { }

                //Pre-condition: Ensure the "consent for web uploader" option is disabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Launch HTML5 uploader from HTML5 supported browser and login as "st" user
                if (Config.BrowserType.Contains("firefox"))
                {
                    Driver.Quit();
                    Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("homepage");
                PageLoadWait.WaitForFrameLoad(10);
                html5.Login_HTML5Uploader(stuser, stpassword, HippaComplianceSelection: false);
                PageLoadWait.WaitForFrameLoad(10);
                bool step1 = PageLoadWait.WaitForElement(html5.By_UploadFilesBtn(), WaitTypes.Visible).Displayed;
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2: Click on"UPLOAD FILES", browse multiple dicom files and click on"SELECT"button
                //uploading file nonetheless for further steps
                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(MultipleUploadFiles, "file");
                bool step3_1 = html5.UploadJobContainer().Displayed;
                //bool step3_2 = html5.UploadJobProgressBar().Displayed;
                if (step3_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                bool step4 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
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

                //Step-4: Verify that the cache of the uploaded studies are available in the iCA server under the following location 'C:\Windows\Temp\AcceptedFolder'
                var MainDirectory = Directory.GetDirectories(Config.HTML5UploaderAcceptedPath);
                var Subdirectory = Directory.GetDirectories(MainDirectory[0]);
                if ((Subdirectory.Any(q => q.Contains(StudyInstanceUID[1]))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step-5: Ensure that + Add files, + Add Folders and Drag and Drop options under JOB #1
                if (html5.AddFiles().Displayed && html5.AddFolder().Displayed && html5.DragDropMessage().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Click on "Add file" below the JOB #1 and select a Non DICOM files
                html5.AddFiles().Click();
                UploadFileInBrowser(AttachmentFilePath, "file");
                if (html5.ModalMessageDiv().Text == Message)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7: Click on "NO" button
                html5.NoBtn().Click();
                if (!PageLoadWait.WaitForElement(html5.By_ModalDialogDiv(), WaitTypes.Invisible).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Drag a Non DICOM object and drop it on the JOB #1 in the upload main page - Not Automated
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step drag drop not automated but still uploading for further steps
                html5.AddFiles().Click();
                UploadFileInBrowser(AttachmentFilePath, "file");

                //Step-9: Click on "YES" button
                html5.YesBtn().Click();
                if (html5.AttachmentFilesContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Choose a patient from the patient drop down list for which patient the attachments need to be linked
                html5.PatientDropdown().SelectByValue(Patient1Details[4]);
                if (html5.PatientDropdown().SelectedOption.Text.ToLower().Contains(Patient1Details[0].ToLower()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Choose any study from the study drop down list for which study the attachments need to be linked
                html5.StudyDropdown().SelectByIndex(1);
                if (html5.StudyDropdown().SelectedOption.Text.Contains(Patient1Details[6].Split('(')[0].Trim()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Select the checkbox of the attachment and click on the "ATTACH" button
                html5.SelectAttachmentForPatient(AttachmentFilePath.Split('\\').Last());
                html5.AttachmentSubmitBtn().Click();
                PageLoadWait.WaitForHTML5StudyToUpload();
                int ImageCount, SeriesCount;
                SeriesCount = Convert.ToInt32(Patient1Details[9]) + 1;
                ImageCount = Convert.ToInt32(Patient1Details[10]) + 1;
                if ((html5.SeriesSpan()[0].Text == SeriesCount.ToString() && html5.ImagesSpan()[0].Text == ImageCount.ToString()) || (html5.SeriesSpan()[1].Text == SeriesCount.ToString() && html5.ImagesSpan()[1].Text == ImageCount.ToString()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-13: Verify the cache of the uploaded non-dicom objects
                MainDirectory = Directory.GetDirectories(Config.HTML5UploaderAcceptedPath);
                Subdirectory = Directory.GetDirectories(MainDirectory[0] + Path.DirectorySeparatorChar + StudyInstanceUID[0]);
                var AttachmentFiles = Directory.GetFiles(Subdirectory[0]);
                //Check if the Attachment files list contains the extension of the uploaded image type
                if ((AttachmentFiles.Any(q => q.Contains(AttachmentFilePath.Split('\\').Last().Split('.')[1]))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Click on "SHARE JOB #1" button below the upload main page
                html5.ShareJobButton().Click();
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

                //Step-15: Ensure that all the studies uploaded in the upload main page are available with attachments in patient Content summary details
                bool step16_1, step16_2, step16_3, step16_4, step16_5, step16_6, step16_7, step16_8;
                if (html5.PatientNameonSharePage().Text == Patient1Details[0])
                {
                    //Patient 1:
                    step16_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                    step16_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                    step16_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                    step16_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                    //Patient 2: 
                    step16_5 = (html5.PatientNameonSharePage(1).Text == Patient2Details[0]);
                    step16_6 = (html5.StudyDetailsonSharePage(1)[0].Text == StudyCount);
                    step16_7 = (html5.StudyDetailsonSharePage(1)[1].Text == Patient2Details[10]);
                    step16_8 = (Convert.ToInt32(html5.StudyDetailsonSharePage(1)[2].Text) == 0);
                }
                else
                {
                    //Patient 1:
                    step16_1 = (html5.PatientNameonSharePage(1).Text == Patient1Details[0]);
                    step16_2 = (html5.StudyDetailsonSharePage(1)[0].Text == StudyCount);
                    step16_3 = (html5.StudyDetailsonSharePage(1)[1].Text == Patient1Details[10]);
                    step16_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage(1)[2].Text) == 1);
                    //Patient 2: 
                    step16_5 = (html5.PatientNameonSharePage().Text == Patient2Details[0]);
                    step16_6 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                    step16_7 = (html5.StudyDetailsonSharePage()[1].Text == Patient2Details[10]);
                    step16_8 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 0);
                }
                Logger.Instance.InfoLog("step16_1: " + step16_1 + " step16_2:" + step16_2 + " step16_3: " + step16_3 + " step16_4: " + step16_4 + " step16_5: " + step16_5 + " step16_6: " + step16_6 + " step16_7 :" + step16_4 + " step16_8: " + step16_8);
                if (step16_1 && step16_2 && step16_3 && step16_4 && step16_5 && step16_6 && step16_7 && step16_8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Select the study upload destination from the "To" dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Choose any Priority from the Priority dropdown list.
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

                //Step-18: Add some comments optionally for the uploaded study in Comments section.
                html5.CommentTextBox().SendKeys(Comments);
                if (GetTextFromTextBox("id", "textAreaComments").Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19: Click on " SUBMIT " button to send the study to selected destination
                html5.ShareBtn().Click();
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

                //Step-20: Logout from"st"user
                html5.Logout_HTML5Uploader();
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21: Login as "ph" user and go to Inbounds page, Search for the exams sent from HTML5 Uploader in iCA
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step22 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step22 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22: Verify the priority information for the exams sent from HTML5 uploader
                String step23;
                step22.TryGetValue("Priority", out step23);
                if (step23 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23 & 24: Load the study on viewer and Verify that the study details are matched with the uploaded one  & Verify that the attached Image and report are loaded on the viewer..
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step24 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step24)
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

                //Step-25: Logout from "ph" and login as "st" user.
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-26: Go to outbounds page, Search for the exams sent from HTML5 Uploader in iCA
                outbounds = login.Navigate<Outbounds>();
                login.ClearFields(2);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                ChooseColumns(new string[] { "Priority" });
                Dictionary<string, string> step27 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step27 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-27: Verify the priority information for the exams sent from HTML5 uploader
                String step28;
                step27.TryGetValue("Priority", out step28);
                if (step28 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28 & 29: Load the study on viewer and Verify that the study details are matched with the uploaded one & Verify that the attached Image and report are loaded on the viewer..
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step29 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step29)
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

                //Step-30: Logout from "st"
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
        /// HTML5Uploader - Upload DICOM stud(y)ies to Destination from Outbounds page
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163445(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Patient1Details = PatientDetailsList.Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 134125: " + new Random().Next(1, 10000);

                //Pre-condition: Ensure the "consent for web uploader" option is disabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Login to iCA as " st " user from HTML5 supported browser.
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-2: Go to Outbounds page, Click on "Upload" button from the bottom of the page
                outbounds = login.Navigate<Outbounds>();
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT();
                PageLoadWait.WaitForFrameLoad(10);
                bool step2 = PageLoadWait.WaitForElement(html5.By_UploadFilesBtn(), WaitTypes.Visible).Displayed;
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Drag and Drop a DICOM DIR Folder which contains a study of single patient - NA due to drag and drop script will need research time
                result.steps[++ExecutedSteps].status = "Not Automated";
                //uploading file nonetheless for further steps
                html5.UploadFolderBtn().Click();
                UploadFileInBrowser(UploadFilePath);
                bool step4_1 = html5.UploadJobContainer().Displayed;
                //bool step4_2 = html5.UploadJobProgressBar().Displayed;

                //Step-4: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                bool step5 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
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

                //Step-5: Ensure that all the studies listed in the study panel with patient and Study info
                //Patient 1:
                bool s6_1_patientname = (html5.PatientNameSpan()[0].Text == Patient1Details[0]);
                bool s6_1_DOB = (html5.DOBSpan()[0].Text == Patient1Details[1]);
                //Update for UI change for Age and Years span separation
                String TempAge, TempYrs;
                TempAge = TempYrs = String.Empty;
                if (!String.IsNullOrEmpty(Patient1Details[2]))
                {
                    TempAge = Patient1Details[2].Split(' ')[0];
                    TempYrs = Patient1Details[2].Split(' ')[1];
                }
                bool s6_1_Age = (html5.AgeSpan()[0].Text == TempAge);
                bool s6_1_YrsTxt = (html5.YearsSpan()[0].Text == TempYrs);
                bool s6_1_Gender = (html5.GenderSpan()[0].Text == Patient1Details[3]);
                bool s6_1_MRN = (html5.MRNSpan()[0].Text == Patient1Details[4]);
                bool s6_1_IPID = (html5.IPIDSpan()[0].Text == Patient1Details[5]);
                bool s6_1_StudyDesc = (html5.StudyDescSpan()[0].Text == Patient1Details[6]);
                bool s6_1_Modality = (html5.ModalitySpan()[0].Text == Patient1Details[7]);
                bool s6_1_Date = (html5.DateSpan()[0].Text == Patient1Details[8]);
                bool s6_1_Series = (html5.SeriesSpan()[0].Text == Patient1Details[9]);
                bool s6_1_Image = (html5.ImagesSpan()[0].Text == Patient1Details[10]);
                bool s6_1_Accession = (html5.AccessionSpan()[0].Text == Patient1Details[11]);
                bool s6_1_Institution = (html5.InstitutionSpan()[0].Text == Patient1Details[12]);
                bool s6_1_RefPhys = (html5.RefPhysSpan()[0].Text == Patient1Details[13]);
                Logger.Instance.InfoLog("s6_1_patientname: " + s6_1_patientname + " s6_1_DOB: " + s6_1_DOB + " s6_1_Age: " + s6_1_Age + " s6_1_YrsTxt: " + s6_1_YrsTxt + " s6_1_YrsTxt: " + s6_1_YrsTxt + " s6_1_Gender: " + s6_1_Gender + "s6_1_MRN: " + s6_1_MRN + " s6_1_IPID: " + s6_1_IPID + " s6_1_StudyDesc :" + s6_1_StudyDesc + " s6_1_Modality: " + s6_1_Modality + " s6_1_Date: " + s6_1_Date + " s6_1_Series: " + s6_1_Series + " s6_1_Image: " + s6_1_Image + " s6_1_Accession: " + s6_1_Accession + " s6_1_Institution: " + s6_1_Institution + " s6_1_RefPhys: " + s6_1_RefPhys);
                if (s6_1_patientname && s6_1_DOB && s6_1_Gender && s6_1_Age && s6_1_YrsTxt && s6_1_MRN && s6_1_IPID && s6_1_StudyDesc && s6_1_Modality && s6_1_Date && s6_1_Series && s6_1_Image && s6_1_Accession && s6_1_Institution && s6_1_RefPhys)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Click on "SHARE JOB #1" button from the upload main page
                html5.ShareJobButton().Click();
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

                //Step-7: Ensure that all the studies uploaded in the upload main page are available with attachments in patient Content summary details
                //Patient 1:
                bool step8_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                bool step8_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step8_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                bool step8_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 0);
                Logger.Instance.InfoLog("step8_1: " + step8_1 + " step8_2:" + step8_2 + " step8_3: " + step8_3 + " step8_4: " + step8_4);
                if (step8_1 && step8_2 && step8_3 && step8_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Select the study upload destination from the " To " and priority from the respective dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                html5.PriorityDropdown().SelectByText(Priority);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1 && html5.PriorityDropdown().SelectedOption.Text == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Add some comments optionally for the uploaded study in Comments section.
                html5.CommentTextBox().SendKeys(Comments);
                if (GetTextFromTextBox("id", "textAreaComments").Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Click on " SUBMIT " button to send the study to selected destination
                html5.ShareBtn().Click();
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

                //Step-11: Logout from"st"user
                html5.Logout_HTML5Uploader();
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Login as "ph" user and go to Inbounds page, Search for the exams sent from HTML5 Uploader in iCA
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step13 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step13 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Verify the priority information for the exams sent from HTML5 uploader
                String step14;
                step13.TryGetValue("Priority", out step14);
                if (step14 == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Load the uploaded study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                //Verify the study loaded in viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step15 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()) && step15)
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

                //Step-15: Logout from "ph" and login as "st" user.
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-16: Go to outbounds page, Search for the exams sent from HTML5 Uploader in iCA
                outbounds = login.Navigate<Outbounds>();
                login.ClearFields(2);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step17 = outbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step17 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Load the study on viewer and Verify that the study details are matched with the uploaded one
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                //Verify the study loaded in viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()) && step18)
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

                //Step-18: Logout from " st " user
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
        /// HTML5Uploader - Uploading stud(y)ies via Guest User
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163447(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String MultipleUploadFiles = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MultipleUploadFiles");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String Phonenumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Phonenumber");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String[] Modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':');
                String[] Patient1Details = PatientDetailsList.Split('=')[0].Split('|');
                String[] Patient2Details = PatientDetailsList.Split('=')[1].Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 134131: " + new Random().Next(1, 10000);

                //Pre-condition: Ensure the "consent for web uploader" option is enabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (!domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Launch HTML5 Uploader from iCA login page from HTML5 supported browser
                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("homepage");
                PageLoadWait.WaitForFrameLoad(10);
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2: Login as Guest user by entering Email and phone number
                html5.Login_HTML5Uploader(email: Email, phonenumber: Phonenumber, HippaComplianceSelection: false);
                bool step2 = PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), WaitTypes.Visible).Displayed;
                if (step2 && html5.UsernameDisplayed().Text.Equals(Email))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Check in "I read and understood the agreement. I agree to comply to it" and click on "Continue"button
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

                //Step-4: Click on "UPLOAD FILES", browse multiple dicom files of different patients and click on"SELECT"button
                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(MultipleUploadFiles, "file");
                //bool step4_2 = html5.UploadJobProgressBar().Displayed;
                bool step4_1 = html5.UploadJobContainer().Displayed;
                if (step4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                bool step5 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
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

                //Step-6: Ensure that all the studies are listed in the study panel with patient and Study info
                //Patient 1:
                bool s6_1_patientname = (html5.PatientNameSpan()[0].Text == Patient1Details[0]);
                bool s6_1_DOB = (html5.DOBSpan()[0].Text == Patient1Details[1]);
                //Update for UI change for Age and Years span separation
                String TempAge, TempYrs;
                TempAge = TempYrs = String.Empty;
                if (!String.IsNullOrEmpty(Patient1Details[2]))
                {
                    TempAge = Patient1Details[2].Split(' ')[0];
                    TempYrs = Patient1Details[2].Split(' ')[1];
                }
                bool s6_1_Age = (html5.AgeSpan()[0].Text == TempAge);
                bool s6_1_YrsTxt = (html5.YearsSpan()[0].Text == TempYrs);
                bool s6_1_Gender = (html5.GenderSpan()[0].Text == Patient1Details[3]);
                bool s6_1_MRN = (html5.MRNSpan()[0].Text == Patient1Details[4]);
                bool s6_1_IPID = (html5.IPIDSpan()[0].Text == Patient1Details[5]);
                bool s6_1_StudyDesc = (html5.StudyDescSpan()[0].Text == Patient1Details[6]);
                bool s6_1_Modality = (html5.ModalitySpan()[0].Text == Patient1Details[7]);
                bool s6_1_Date = (html5.DateSpan()[0].Text == Patient1Details[8]);
                bool s6_1_Series = (html5.SeriesSpan()[0].Text == Patient1Details[9]);
                bool s6_1_Image = (html5.ImagesSpan()[0].Text == Patient1Details[10]);
                bool s6_1_Accession = (html5.AccessionSpan()[0].Text == Patient1Details[11]);
                bool s6_1_Institution = (html5.InstitutionSpan()[0].Text == Patient1Details[12]);
                bool s6_1_RefPhys = (html5.RefPhysSpan()[0].Text == Patient1Details[13]);
                //Patient 2:
                bool s6_2_patientname = (html5.PatientNameSpan()[1].Text == Patient2Details[0]);
                bool s6_2_DOB = (html5.DOBSpan()[1].Text == Patient2Details[1]);
                if (!String.IsNullOrEmpty(Patient2Details[2]))
                {
                    TempAge = Patient2Details[2].Split(' ')[0];
                    TempYrs = Patient2Details[2].Split(' ')[1];
                }
                else
                {
                    TempAge = TempYrs = String.Empty;
                }
                bool s6_2_Age = (html5.AgeSpan()[1].Text == TempAge);
                bool s6_2_YrsTxt = (html5.YearsSpan()[1].Text == TempYrs);
                bool s6_2_Gender = (html5.GenderSpan()[1].Text == Patient2Details[3]);
                bool s6_2_MRN = (html5.MRNSpan()[1].Text == Patient2Details[4]);
                bool s6_2_IPID = (html5.IPIDSpan()[1].Text == Patient2Details[5]);
                bool s6_2_StudyDesc = (html5.StudyDescSpan()[1].Text == Patient2Details[6]);
                bool s6_2_Modality = (html5.ModalitySpan()[1].Text == Patient2Details[7]);
                bool s6_2_Date = (html5.DateSpan()[1].Text == Patient2Details[8]);
                bool s6_2_Series = (html5.SeriesSpan()[1].Text == Patient2Details[9]);
                bool s6_2_Image = (html5.ImagesSpan()[1].Text == Patient2Details[10]);
                bool s6_2_Accession = (html5.AccessionSpan()[1].Text == Patient2Details[11]);
                bool s6_2_Institution = (html5.InstitutionSpan()[1].Text == Patient2Details[12]);
                bool s6_2_RefPhys = (html5.RefPhysSpan()[1].Text == Patient2Details[13]);
                Logger.Instance.InfoLog("s6_1_patientname: " + s6_1_patientname + " s6_1_DOB:" + s6_1_DOB + " s6_1_Age: " + s6_1_Age + " s6_1_YrsTxt: " + s6_1_YrsTxt + " s6_1_Gender: " + s6_1_Gender + " s6_1_MRN: " + s6_1_MRN + " s6_1_IPID: " + s6_1_IPID + " s6_1_StudyDesc :" + s6_1_StudyDesc + " s6_1_Modality: " + s6_1_Modality + " s6_1_Date: " + s6_1_Date + " s6_1_Series: " + s6_1_Series + " s6_1_Image: " + s6_1_Image + " s6_1_Accession: " + s6_1_Accession + " s6_1_Institution: " + s6_1_Institution + " s6_1_RefPhys: " + s6_1_RefPhys);
                Logger.Instance.InfoLog("s6_2_patientname: " + s6_2_patientname + " s6_2_DOB:" + s6_2_DOB + " s6_2_Age: " + s6_2_Age + " s6_2_YrsTxt: " + s6_2_YrsTxt + " s6_2_Gender: " + s6_2_Gender + " s6_2_MRN: " + s6_2_MRN + " s6_2_IPID: " + s6_2_IPID + " s6_2_StudyDesc :" + s6_2_StudyDesc + " s6_2_Modality: " + s6_2_Modality + " s6_2_Date: " + s6_2_Date + " s6_2_Series: " + s6_2_Series + " s6_2_Image: " + s6_2_Image + " s6_2_Accession: " + s6_2_Accession + " s6_2_Institution: " + s6_2_Institution + " s6_2_RefPhys: " + s6_2_RefPhys);

                if (s6_1_patientname && s6_1_DOB && s6_1_Gender && s6_1_Age && s6_1_YrsTxt && s6_1_MRN && s6_1_IPID && s6_1_StudyDesc && s6_1_Modality && s6_1_Date && s6_1_Series && s6_1_Image && s6_1_Accession && s6_1_Institution && s6_1_RefPhys && s6_2_patientname && s6_2_DOB && s6_2_Gender && s6_2_Age && s6_2_YrsTxt && s6_2_MRN && s6_2_IPID && s6_2_StudyDesc && s6_2_Modality && s6_2_Date && s6_2_Series && s6_2_Image && s6_2_Accession && s6_2_Institution && s6_2_RefPhys)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7: Verify that phone number field has text box with "Add Phone" as hint inside textbox in grey color
                //Color Behavior inconsistent in FF browser, hence removing it
                //string color7 = html5.StudyPhoneTxtBox()[0].GetCssValue("background");
                //if (html5.StudyPhoneTxtBox()[0].GetAttribute("placeholder").Equals(" Add Phone ") && color7.Contains("rgba(255, 255, 255, 0.65)"))
                if (html5.StudyPhoneTxtBox()[0].GetAttribute("placeholder").Equals(" Add Phone "))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Verify that Email field has text box with "Add Email" as hint inside textbox in grey color.
                //string color8 = html5.StudyEmailTxtBox()[0].GetCssValue("background");
                if (html5.StudyEmailTxtBox()[0].GetAttribute("placeholder").Equals(" Add Email "))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Add email address and phone number for the listed studies
                html5.StudyPhoneTxtBox()[0].SendKeys(Phonenumber);
                html5.StudyPhoneTxtBox()[0].SendKeys(Keys.Enter);
                html5.StudyEmailTxtBox()[0].SendKeys(Email);
                html5.StudyEmailTxtBox()[0].SendKeys(Keys.Enter);

                if (html5.StudyPhoneTxtBox()[0].GetAttribute("value").Equals(Phonenumber) && html5.StudyEmailTxtBox()[0].GetAttribute("value").Equals(Email))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 & 11: Click on " Attachment " option of any one of the study listed & Browse Non DICOM objects and click on " Open " button [Non DICOM:- JPG, JPEG, PNG, TIFF, TIF, BMP & PDF ]
                //Clicking first attachment button
                html5.AttachmentButtons()[0].Click();
                ExecutedSteps++;
                UploadFileInBrowser(AttachmentFilePath, "file");
                ExecutedSteps++;

                //Step-12: Wait until the progress bar gets completed and ensure that the attachment is available beside attachment option
                PageLoadWait.WaitForHTML5StudyToUpload();
                int ImageCount, SeriesCount;
                SeriesCount = Convert.ToInt32(Patient1Details[9]) + 1;
                ImageCount = Convert.ToInt32(Patient1Details[10]) + 1;
                if (html5.SeriesSpan()[0].Text == SeriesCount.ToString() && html5.ImagesSpan()[0].Text == ImageCount.ToString())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Click on "SHARE JOB #1" button from the upload main page
                html5.ShareJobButton().Click();
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

                //Step-14: Ensure that all the studies uploaded in the upload main page are available with attachments in patient Content summary details
                //Patient 1:
                bool step14_1_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                bool step14_1_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step14_1_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                bool step14_1_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                //Patient 2:
                bool step14_2_1 = (html5.PatientNameonSharePage(1).Text == Patient2Details[0]);
                bool step14_2_2 = (html5.StudyDetailsonSharePage(1)[0].Text == StudyCount);
                bool step14_2_3 = (html5.StudyDetailsonSharePage(1)[1].Text == Patient2Details[10]);
                bool step14_2_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage(1)[2].Text) == 0);

                if (step14_1_1 && step14_1_2 && step14_1_3 && step14_1_4 && step14_2_1 && step14_2_2 && step14_2_3 && step14_2_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Select the study upload destination from the "To" dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Choose any Priority from the Priority dropdown list.
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

                //Step-17: Add some comments optionally for the uploaded study in Comments section.
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

                //Step-18: Click on " SUBMIT " button to send the study to selected destination
                html5.ShareBtn().Click();
                PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), WaitTypes.Visible, 120);
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

                //Step-19: Logout from " Guest " user
                html5.Logout_HTML5Uploader();
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-20: Launch EA webadmin page (https://ServerIP/webadmin) of holding pen server, login as webadmin user and navigate to archive search pag
                //if (!(Config.BrowserType.Equals("chrome")))
                //{
                //    Driver.Quit();
                //    Driver = null;
                //    login.InvokeBrowser("chrome");
                //}
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Step-21:Verify that the attachment is converted as DICOM
                workflow.HPSearchStudy("PatientID", PatientID);
                //workflow.HPSearchStudy("Modality", Modality);
                Dictionary<string, string> studyresults = workflow.GetStudyDetailsInHP();

                //CLick on Patient ID to verify the Attachment presence                
                Dictionary<int, string[]> seriesresults = workflow.GetSeriesDetailsInHP(Patient1Details[0].ToUpper());
                //Second Level
                seriesresults = workflow.GetSeriesDetailsInHP(Patient1Details[11]);

                Dictionary<int, string[]> results = workflow.GetResultsInHP();
                if (results.Where(r => r.Value.Contains(Modality[0])).Count() != 0 && results.Where(r => r.Value.Contains(Modality[1])).Count() != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                hplogin.LogoutHPen();
                //Driver.Quit();
                //Driver = null;
                //login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);

                //Step-22: Login as "ph" user and go to Inbounds page, Search for the exams sent from HTML5 Uploader in iCA
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.ChooseColumns(new string[] { "From User Phone Number" });
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step22 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step22 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Ensure that " From user " has Guest user email address and "From user phone number " has Guest user phone number are displayed in the study grid
                String step23_email, step23_phone;
                step22.TryGetValue("From User(s)", out step23_email);
                step22.TryGetValue("From User Phone Number", out step23_phone);
                if (step23_phone == Phonenumber && step23_email == Email)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24: From Inbounds page study list, add Email Address and Phone number column from Choose Columns.
                ChooseColumns(new string[] { "Patient Email Address", "Patient Phone Number" });
                Dictionary<string, string> step24 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step24 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25: Verify the added Email address, phone number and priority information for the exams sent from HTML5 uploader
                String step25_phone, step25_email, step25_priority;
                step24.TryGetValue("Priority", out step25_priority);
                step24.TryGetValue("Patient Email Address", out step25_email);
                step24.TryGetValue("Patient Phone Number", out step25_phone);
                if (step25_priority == Priority && step25_phone == Phonenumber && step25_email == Email)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                //Verify the study loaded in viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step26 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()) && step26)
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

                //Step-27: Logout from "ph" user
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
        /// HTML5Uploader - Uploading stud(y)ies via LDAP User
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163448(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            RoleManagement rolemanagement;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.LdapARUser;
                String arpassword = Config.LdapUserPassword;
                String phuser = Config.LdapPHUser;
                String phpassword = Config.LdapUserPassword;
                String stuser = Config.LdapSTUser;
                String stpassword = Config.LdapUserPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String Phonenumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Phonenumber");
                String LDAPDomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Domain");
                String[] Patient1Details = PatientDetailsList.Split('|');
                String[] RoleNames = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleNames")).Split(':');
                String[] Groups = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "GroupNames")).Split(':');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 134132: " + new Random().Next(1, 10000);
                String destinationname = "LDAP_134132_" + new Random().Next(1, 999);

                //Pre-Condition: Create LDAP domain, role and groups
                //login.LoginIConnect(username, password);
                ////Create new Domains, Roles and Groups as per LDAP
                //domain = (DomainManagement)login.Navigate("DomainManagement");
                //Dictionary<Object, String> createDomain = domain.CreateDomainAttr();
                ////Domain4
                //if (!domain.IsDomainExist(LDAPDomainName))
                //{
                //    createDomain[DomainManagement.DomainAttr.DomainName] = LDAPDomainName;
                //    createDomain[DomainManagement.DomainAttr.RoleName] = RoleNames[0];
                //    createDomain[DomainManagement.DomainAttr.UserID] = BasePage.GetUniqueDomainID();
                //    domain.CreateDomain(createDomain);
                //}
                //rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                ////Archivist
                //if (!rolemanagement.RoleExists(RoleNames[0], LDAPDomainName))
                //{
                //    rolemanagement.CreateRole(LDAPDomainName, RoleNames[0], roletype: "");
                //}
                ////Physician
                //if (!rolemanagement.RoleExists(RoleNames[1], LDAPDomainName))
                //{
                //    rolemanagement.CreateRole(LDAPDomainName, RoleNames[1], roletype: "");
                //}
                //usermanagement = (UserManagement)login.Navigate("UserManagement");
                ////Cardiology-AR
                //if (!usermanagement.IsGroupExist(Groups[0], LDAPDomainName))
                //{
                //    usermanagement.CreateGroup(LDAPDomainName, Groups[0], rolename: RoleNames[0]);
                //}
                ////Chaplaincy-PH
                //if (!usermanagement.IsGroupExist(Groups[1], LDAPDomainName))
                //{
                //    usermanagement.CreateGroup(LDAPDomainName, Groups[1], rolename: RoleNames[1]);
                //}
                ////Microbiology-AR
                //if (!usermanagement.IsGroupExist(Groups[2], LDAPDomainName))
                //{
                //    usermanagement.CreateSubGroup(LDAPDomainName, Groups[2], rolename: RoleNames[0]);
                //}
                ////Neurology-PH
                //if (!usermanagement.IsGroupExist(Groups[3], LDAPDomainName))
                //{
                //    usermanagement.CreateGroup(LDAPDomainName, Groups[3], rolename: RoleNames[1]);
                //}
                //login.Logout();

                //Pre-condition: Ensure the "consent for web uploader" option is enabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (!domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //Creating LDAP destination as part of pre-condition
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination destination = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");

                destination.CreateDestination(GetHostName(Config.DestinationPACS), phuser, aruser, destinationname);
                login.Logout();

                //Step-1 & 2: Launch HTML5 uploader from HTML5 supported browser and login as "st" user
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("homepage");
                PageLoadWait.WaitForFrameLoad(10);
                html5.Login_HTML5Uploader(stuser, stpassword, HippaComplianceSelection: false);
                bool step1 = PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), WaitTypes.Visible).Displayed;
                if (step1 && html5.UsernameDisplayed().Text.Equals(stuser))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                ExecutedSteps++;

                //Step-3: Check in "I read and understood the agreement. I agree to comply to it" and click on "Continue"button
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

                //Step-4: Click on "UPLOAD FOLDER", browse a dicom folder which has many studies of a patient and click on "SELECT" button
                html5.UploadFolderBtn().Click();
                UploadFileInBrowser(UploadFilePath);
                //bool step4_2 = html5.UploadJobProgressBar().Displayed;
                bool step4_1 = html5.UploadJobContainer().Displayed;
                if (step4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                bool step5 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
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

                //Step-6: Ensure that all the studies are listed in the study panel with patient and Study info
                //Patient 1:
                bool s6_1_patientname = (html5.PatientNameSpan()[0].Text == Patient1Details[0]);
                bool s6_1_DOB = (html5.DOBSpan()[0].Text == Patient1Details[1]);
                //Update for UI change for Age and Years span separation
                String TempAge, TempYrs;
                TempAge = TempYrs = String.Empty;
                if (!String.IsNullOrEmpty(Patient1Details[2]))
                {
                    TempAge = Patient1Details[2].Split(' ')[0];
                    TempYrs = Patient1Details[2].Split(' ')[1];
                }
                bool s6_1_Age = (html5.AgeSpan()[0].Text == TempAge);
                bool s6_1_YrsTxt = (html5.YearsSpan()[0].Text == TempYrs);
                bool s6_1_Gender = (html5.GenderSpan()[0].Text == Patient1Details[3]);
                bool s6_1_MRN = (html5.MRNSpan()[0].Text == Patient1Details[4]);
                bool s6_1_IPID = (html5.IPIDSpan()[0].Text == Patient1Details[5]);
                bool s6_1_StudyDesc = (html5.StudyDescSpan()[0].Text == Patient1Details[6]);
                bool s6_1_Modality = (html5.ModalitySpan()[0].Text == Patient1Details[7]);
                bool s6_1_Date = (html5.DateSpan()[0].Text == Patient1Details[8]);
                bool s6_1_Series = (html5.SeriesSpan()[0].Text == Patient1Details[9]);
                bool s6_1_Image = (html5.ImagesSpan()[0].Text == Patient1Details[10]);
                bool s6_1_Accession = (html5.AccessionSpan()[0].Text == Patient1Details[11]);
                bool s6_1_Institution = (html5.InstitutionSpan()[0].Text == Patient1Details[12]);
                bool s6_1_RefPhys = (html5.RefPhysSpan()[0].Text == Patient1Details[13]);
                Logger.Instance.InfoLog("s6_1_patientname: " + s6_1_patientname + " s6_1_DOB:" + s6_1_DOB + " s6_1_Age: " + s6_1_Age + " s6_1_YrsTxt: " + s6_1_YrsTxt +" s6_1_Gender: " + s6_1_Gender + " s6_1_MRN: " + s6_1_MRN + " s6_1_IPID: " + s6_1_IPID + " s6_1_StudyDesc :" + s6_1_StudyDesc + " s6_1_Modality: " + s6_1_Modality + " s6_1_Date: " + s6_1_Date + " s6_1_Series: " + s6_1_Series + " s6_1_Image: " + s6_1_Image + " s6_1_Accession: " + s6_1_Accession + " s6_1_Institution: " + s6_1_Institution + " s6_1_RefPhys: " + s6_1_RefPhys);
                if (s6_1_patientname && s6_1_DOB && s6_1_Gender && s6_1_Age && s6_1_YrsTxt && s6_1_MRN && s6_1_IPID && s6_1_StudyDesc && s6_1_Modality && s6_1_Date && s6_1_Series && s6_1_Image && s6_1_Accession && s6_1_Institution && s6_1_RefPhys)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7: Add " Email address " and " Phone number " for the listed studies
                html5.StudyPhoneTxtBox()[0].SendKeys(Phonenumber);
                html5.StudyPhoneTxtBox()[0].SendKeys(Keys.Enter);
                html5.StudyEmailTxtBox()[0].SendKeys(Email);
                html5.StudyEmailTxtBox()[0].SendKeys(Keys.Enter);

                if (html5.StudyPhoneTxtBox()[0].GetAttribute("value").Equals(Phonenumber) && html5.StudyEmailTxtBox()[0].GetAttribute("value").Equals(Email))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Click on "SHARE JOB #1" button from the upload main page
                html5.ShareJobButton().Click();
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

                //Step-9: Ensure that all the studies uploaded in the upload main page are available with attachments in patient Content summary details
                //Patient 1:
                bool step9_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                bool step9_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step9_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                bool step9_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 0);
                Logger.Instance.InfoLog("step9_1: " + step9_1 + " step9_2:" + step9_2 + " step9_3: " + step9_3 + " step9_4: " + step9_4);

                if (step9_1 && step9_2 && step9_3 && step9_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Select the study upload destination from the "To" dropdown list.
                html5.DestinationDropdown().SelectByText(destinationname);
                if (html5.DestinationDropdown().SelectedOption.Text == destinationname)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Choose any Priority from the Priority dropdown list.
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

                //Step-12: Add some comments optionally for the uploaded study in Comments section.
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

                //Step-13: Click on " SUBMIT " button to send the study to selected destination
                html5.ShareBtn().Click();
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

                //Step-14: Logout from LDAP "st" user.
                html5.Logout_HTML5Uploader();
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Login as LDAP "ph" user and go to Inbounds page, Search for the exams sent from HTML5 Uploader in iCA
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step15 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step15 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: From Inbounds page study list, add Email Address and Phone number column from Choose Columns.
                ChooseColumns(new string[] { "Patient Email Address", "Patient Phone Number" });
                Dictionary<string, string> step16 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step16 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Verify the added Email address, phone number and priority information for the exams sent from HTML5 uploader
                String step17_phone, step17_email, step17_priority;
                step16.TryGetValue("Priority", out step17_priority);
                step16.TryGetValue("Patient Email Address", out step17_email);
                step16.TryGetValue("Patient Phone Number", out step17_phone);
                if (step17_priority == Priority && step17_phone == Phonenumber && step17_email == Email)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                //Verify the study loaded in viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
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

                //Step-19: Logout from LDAP "ph" and login as LDAP "st" user.
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(stuser, stpassword);
                ExecutedSteps++;

                //Step-20: Go to outbounds page, Search for the exams sent from HTML5 Uploader in iCA
                outbounds = login.Navigate<Outbounds>();
                login.ClearFields(2);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                ChooseColumns(new string[] { "Priority" });
                Dictionary<string, string> step20 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step20 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21: From Outbounds page study list, add Email Address and Phone number column from Choose Columns.
                ChooseColumns(new string[] { "Patient Email Address", "Patient Phone Number" });
                Dictionary<string, string> step21 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
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

                //Step-22: Verify the added Email address, phone number and priority information for the exams sent from HTML5 uploader
                String step22_phone, step22_email, step22_priority;
                step21.TryGetValue("Priority", out step22_priority);
                step21.TryGetValue("Patient Email Address", out step22_email);
                step21.TryGetValue("Patient Phone Number", out step22_phone);
                if (step22_priority == Priority && step22_phone == Phonenumber && step22_email == Email)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Load the study on viewer and Verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                Patientinfo = bluringviewer.PatinetName().Text;
                //Verify the study loaded in viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step23 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step23)
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

                //Step-24: Logout from LDAP "st" user
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
        /// HTML5Uploader - Deletion of Non DICOM object(s)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163449(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Inbounds inbounds = null;
            StudyViewer studyvw = null;
            DomainManagement domain = null;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] UploadFilePath = UploadFilePathList.Split('>');
                String AttachmentFilePathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String[] AttachmentFilePath = AttachmentFilePathList.Split('>');
                String StudyCountList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String[] StudyCount = StudyCountList.Split(':');
                String[] PatientDetails = PatientDetailsList.Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String StudyInstanceUIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyInstanceUID");
                String[] StudyInstanceUID = StudyInstanceUIDList.Split('|');

                try
                {
                    DeleteAllFileFolder(Config.HTML5UploaderAcceptedPath);
                }
                catch (Exception e) { }

                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Launch HTML5 uploader from iCA login page and login as "st" user

                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("login");
                PageLoadWait.WaitForFrameLoad(10);
                html5.RegisteredUserRadioBtn().Click();
                html5.UserNameTxtBox().SendKeys(stuser);
                html5.PasswordTxtBox().SendKeys(stpassword);
                html5.SignInBtn().Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
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

                //step 2: Ensure that "continue" button is enabled only after the check in "I read and understood the agreement. I agree to comply to it"and click on"continue"button

                //bool step2_1 = !html5.HippaContinueBtn().Enabled;
                //html5.HippaAgreeChkBox().Click();
                //bool step2_2 = html5.HippaContinueBtn().Enabled;
                //html5.HippaContinueBtn().Click();
                //if (html5.UploadFilesBtn().Displayed && step2_1 && step2_2)
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

                //step 2: Drag and Drop a DICOM folder which has two studies of different patient

                result.steps[++ExecutedSteps].status = "Not Automated";
                html5.UploadFolderBtn().Click();
                UploadFileInBrowser(UploadFilePath[0]);

                //step 3: Wait until the progress bar gets completed

                PageLoadWait.WaitForHTML5StudyToUpload(40);
                bool uploadbar = html5.UploadJobContainer().Displayed;
                bool patientinfo = html5.PatientInfoContainer().Displayed;
                if (uploadbar && patientinfo)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 4: Verify that the cache of the uploaded studies are available in the iCA server under the following location "C:\Windows\Temp\AcceptedFolder

                var MainDirectory = Directory.GetDirectories(Config.HTML5UploaderAcceptedPath);
                var Subdirectory = Directory.GetDirectories(MainDirectory[0]);
                //Check if the subdirectory folder contains the Study instance UID number of the deleted study in any of the folders
                if ((Subdirectory.Any(q => q.Contains(StudyInstanceUID[0]))) && (Subdirectory.Any(q => q.Contains(StudyInstanceUID[1]))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 5: Click on"Add File" under JOB #1 and select a Non DICOM image

                html5.AddFiles().Click();
                UploadFileInBrowser(AttachmentFilePath[0], "file");
                Thread.Sleep(1000);
                if (html5.NoDICOMMsgDisplayed().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 6: Click on "Yes"button

                html5.nonDICOMYesBtn().Click();
                Thread.Sleep(1000);
                if (html5.AttachmentFilesContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 7: Choose a patient from the patient drop down list for which patient the attachment needs to be mapped

                html5.PatientDropdown().SelectByText(PatientDetails[28]);
                if (html5.PatientDropdown().SelectedOption.Text == PatientDetails[28])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 8: Choose a study from the study drop down list for which study the attachment needs to be mapped

                html5.StudyDropdown().SelectByIndex(1);
                if (html5.StudyDropdown().SelectedOption.Text.Contains(PatientDetails[3].Split('(')[0].Trim()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 9: Select the checkbox of the attachment and click on the "Cancel" button

                html5.SelectAttachmentForPatient(AttachmentFilePath[0].Split('\\').Last());
                html5.AttachmentCancelBtn().Click();
                string ImageCount = null;
                string SeriesCount = null;
                //SeriesCount = Convert.ToInt32(PatientDetails[4]);
                //ImageCount = Convert.ToInt32(PatientDetails[5]);
                if (html5.PatientNameSpan()[0].Text == PatientDetails[0] || html5.PatientNameSpan()[0].Text == PatientDetails[31])
                {
                    SeriesCount = (PatientDetails[4]);
                    ImageCount = (PatientDetails[5]);
                }
                else if (html5.PatientNameSpan()[0].Text == PatientDetails[14])
                {
                    SeriesCount = (PatientDetails[18]);
                    ImageCount = (PatientDetails[19]);
                }
                //else if (html5.PatientNameSpan()[0].Text == PatientDetails[31])
                //{
                //    SeriesCount = (PatientDetails[30]);
                //    ImageCount = (PatientDetails[30]);
                //}

                if (html5.SeriesSpan()[0].Text == SeriesCount && html5.ImagesSpan()[0].Text == ImageCount)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10: "Ensure that the cache of the additionally added Non DICOM image is not available in the iCA server under the following location : 
                //""C:\Windows\Temp\AcceptedFolder"""

                MainDirectory = Directory.GetDirectories(Config.HTML5UploaderAcceptedPath);
                Subdirectory = Directory.GetDirectories(MainDirectory[0]);
                var Subdirectory1 = Directory.GetDirectories(Subdirectory[0]);
                var Subdirectory2 = Directory.GetDirectories(Subdirectory[1]);
                if (Subdirectory1.Length == 0 && Subdirectory2.Length == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11: Click on "Share JOB #1" button from the upload main page

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

                //step 12: Ensure that all the studies uploaded in Job #1 are available in patient content summary details

                //SeriesCount = Convert.ToInt32(PatientDetails[4]);
                //ImageCount = Convert.ToInt32(PatientDetails[5]);
                bool step13_1 = false;
                bool step13_2 = false;
                bool step13_3 = false;
                bool step13_4 = false;
                bool step13_5 = false;
                bool step13_6 = false;
                bool step13_7 = false;
                bool step13_8 = false;
                bool step13_9 = false;
                bool step13_10 = false;
                bool step13_11 = false;
                bool step13_12 = false;
                for (int i = 0; i < html5.StudyRows().Count; i++)
                {
                    if (html5.PatientNameonSharePage(i).Text == PatientDetails[0])
                    {
                        step13_1 = true;
                        step13_2 = (html5.StudyDetailsonSharePage(i)[0].Text == StudyCount[0]);
                        step13_3 = (html5.StudyDetailsonSharePage(i)[1].Text == PatientDetails[5]);
                        step13_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage(i)[2].Text) == 0);
                    }
                    else if (html5.PatientNameonSharePage(i).Text == PatientDetails[14])
                    {
                        step13_5 = true;
                        step13_6 = (html5.StudyDetailsonSharePage(i)[0].Text == StudyCount[0]);
                        step13_7 = (html5.StudyDetailsonSharePage(i)[1].Text == PatientDetails[19]);
                        step13_8 = (Convert.ToInt32(html5.StudyDetailsonSharePage(i)[2].Text) == 0);
                    }
                    else if (html5.PatientNameonSharePage(i).Text == PatientDetails[31])
                    {
                        step13_9 = true;
                        step13_10 = (html5.StudyDetailsonSharePage(i)[0].Text == StudyCount[1]);
                        step13_11 = (html5.StudyDetailsonSharePage(i)[1].Text == PatientDetails[30]);
                        step13_12 = (Convert.ToInt32(html5.StudyDetailsonSharePage(i)[2].Text) == 0);
                    }
                }
                if (step13_1 && step13_2 && step13_3 && step13_4 && step13_5 && step13_6 && step13_7 && step13_8 && step13_9 && step13_10 && step13_11 && step13_12)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 13: Select the study upload destination from the "To"drop down list

                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14: Choose any priority from the priority drop down list

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

                //step 15: Click on " SUBMIT " button to send the study to selected destination

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

                //step 16: Login as "Ph"user and go to inbounds page, search for the exams sent from HTML5 uploader in iCA

                html5.Logout_HTML5Uploader();
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.DriverGoTo(login.url);
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllInboundData();
                login.SearchStudy("Patient ID", PatientDetails[1]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step17 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientDetails[1], "Uploaded" });
                if (step17 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 17: Load the study and verity that the deleted attachment of Non DICOM image is not available in the study viewer

                login.SelectStudy("Patient ID", PatientDetails[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step18 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
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
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                try
                {
                    DeleteAllFileFolder(Config.HTML5UploaderAcceptedPath);
                }
                catch (Exception e) { }

                //step 18: In HTML5 uploader page, click on "Upload Files" and select a DICOM files

                HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("login");
                PageLoadWait.WaitForFrameLoad(10);
                html5.RegisteredUserRadioBtn().Click();
                html5.UserNameTxtBox().SendKeys(stuser);
                html5.PasswordTxtBox().SendKeys(stpassword);
                html5.SignInBtn().Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(UploadFilePath[1], "file");
                bool step19_1 = html5.UploadJobContainer().Displayed;
                if (step19_1)
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

                //step 19: Click on"Add Folder" under JOB #1 and select a Folder which has two Non Dicom objects

                html5.AddFolderBtn().Click();
                UploadFileInBrowser(AttachmentFilePath[1]);
                Thread.Sleep(1000);
                if (html5.NoDICOMMsgDisplayed().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 20: Click on "Yes"button

                html5.nonDICOMYesBtn().Click();
                Thread.Sleep(1000);
                if (html5.AttachmentFilesContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 21: Choose a patient and study from the respective drop down list for which patient and study the attachment needs to be mapped

                html5.PatientDropdown().SelectByText(PatientDetails[29]);
                html5.StudyDropdown().SelectByIndex(1);
                if (html5.PatientDropdown().SelectedOption.Text == PatientDetails[29] && html5.StudyDropdown().SelectedOption.Text.Contains(PatientDetails[17].Split('(')[0].Trim()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 22: Select only one attachment from the two attachments listed above study panel and click on "Attach"button

                html5.SelectAttachmentForPatient(AttachmentFilePath[2].Split('\\').Last());
                html5.AttachmentSubmitBtn().Click();
                Thread.Sleep(4000);
                html5.AttachmentCancelBtn().Click();
                PageLoadWait.WaitForHTML5StudyToUpload(100);
                int SeriesCount22 = Convert.ToInt32(PatientDetails[18]) + 1;
                int ImageCount22_1 = Convert.ToInt32(PatientDetails[5]) + 1;
                int ImageCount22_2 = Convert.ToInt32(PatientDetails[19]) + 1;
                if (html5.SeriesSpan()[0].Text == SeriesCount22.ToString() && ((html5.ImagesSpan()[0].Text == ImageCount22_1.ToString() || html5.ImagesSpan()[0].Text == ImageCount22_2.ToString())))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 23: "Ensure the cache of the selected Non Dicom object is available in the iCA server
                //Location: ""C:\Windows\Temp\AcceptedFolder"""

                MainDirectory = Directory.GetDirectories(Config.HTML5UploaderAcceptedPath);
                Subdirectory = Directory.GetDirectories(MainDirectory[0]);
                var AttachmentFiles = Directory.GetFiles(Directory.GetDirectories(Subdirectory[0])[0]);
                //Check if the Attachment files list contains the extension of the uploaded image type
                if ((AttachmentFiles.Any(q => q.Contains(AttachmentFilePath[2].Split('\\').Last().Split('.')[1]))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 24: Click on "Share JOB #1" button from the upload main page

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

                //step 25: Ensure that all the studies uploaded in Job #1 are available with attachments in patient content summary details

                //ImageCount = Convert.ToInt32(PatientDetails[30]);
                bool step26_1 = (html5.PatientNameonSharePage().Text == PatientDetails[31]);
                bool step26_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount[1]);
                bool step26_3 = (html5.StudyDetailsonSharePage()[1].Text == PatientDetails[30]);
                bool step26_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                if (step26_1 && step26_2 && step26_3 && step26_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 26: Select destination and priority from the respective drop down list

                html5.DestinationDropdown().SelectByText(Config.Dest1);
                html5.PriorityDropdown().SelectByText(Priority);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1 && html5.PriorityDropdown().SelectedOption.Text == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 27: Click on " SUBMIT " button to sent the study to selected destination

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

                //step 28: Logout from "st" user

                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.DriverGoTo(login.url);
                if (login.PasswordTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 29: Login as "Ph"user and go to inbounds page, search for the exams sent from HTML5 uploader in iCA

                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllInboundData();
                login.SearchStudy("Patient ID", PatientDetails[15]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step30 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientDetails[15], "Uploaded" });
                if (step30 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 30: Load the study and verity the attachment on the study viewer

                login.SelectStudy("Patient ID", PatientDetails[15]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step31 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step31)
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
                bluringviewer.CloseBluRingViewer();

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
        /// HTML5Uploader - Uploading not Supported files
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163451(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String MultipleUploadFiles = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MultipleUploadFiles");
                String Message = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Message");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Patient1Details = PatientDetailsList.Split('|');
                //String[] Patient2Details = PatientDetailsList.Split('=')[1].Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 138196: " + new Random().Next(1, 10000);
                String Reason = "Reason test case# 138196: " + new Random().Next(1, 10000);

                //Pre-condition: Ensure the "consent for web uploader" option is disabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Launch HTML5 Uploader from iCA login page from HTML5 supported browser
                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("homepage");
                PageLoadWait.WaitForFrameLoad(10);
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2: Login as st user to HTML5 Uploader
                html5.Login_HTML5Uploader(stuser, stpassword, HippaComplianceSelection: false);
                PageLoadWait.WaitForFrameLoad(10);
                bool step2 = PageLoadWait.WaitForElement(html5.By_UploadFilesBtn(), WaitTypes.Visible).Displayed;
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Click on Upload File then select a Not supported Non DICOM file [say as GIF, TXT, XLSX, DOC]
                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(MultipleUploadFiles, "file");
                if (html5.ModalDialogDiv().Displayed && html5.ModalMessageDiv().Text == Message)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                html5.ModalOkBtn().Click();

                //Step-4: Click on"UPLOAD FOLDER", browse a dicom folder which has a DICOM study and click on"SELECT"button
                html5.UploadFolderBtn().Click();
                UploadFileInBrowser(UploadFilePath);
                bool step4_1 = html5.UploadJobContainer().Displayed;
                //bool step3_2 = html5.UploadJobProgressBar().Displayed;
                if (step4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                //Check if Job 1 is selected/highlighted post upload
                bool step5 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
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

                //Step-6: Click on + Add files then select a Not supported Non-DICOM file [say as GIF, TXT, XLSX, DOC]
                html5.AddFiles().Click();
                UploadFileInBrowser(MultipleUploadFiles, "file");
                if (html5.ModalDialogDiv().Displayed && html5.ModalMessageDiv().Text == Message)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                html5.ModalOkBtn().Click();

                //Step-7: Click on + Add folder then upload a folder which has 4 Not Supported Non DICOM files and 1 Supported Non DICOM file
                html5.AddFolder().Click();
                UploadFileInBrowser(AttachmentFilePath);
                PageLoadWait.WaitForHTML5StudyToUpload();
                int ImageCount, SeriesCount;
                SeriesCount = Convert.ToInt32(Patient1Details[9]) + 1;
                ImageCount = Convert.ToInt32(Patient1Details[10]) + 1;
                if (html5.SeriesSpan()[0].Text == SeriesCount.ToString() && html5.ImagesSpan()[0].Text == ImageCount.ToString())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Step-8: Click on"SHARE JOB #1"button from the upload main page
                html5.ShareJobButton().Click();
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

                //Step-9: Ensure that the uploaded study in the upload main page is available with attachment in patient Content summary details
                //Patient 1:
                bool step9_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                bool step9_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step9_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                bool step9_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                if (step9_1 && step9_2 && step9_3 && step9_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-10: Select the study upload destination from the "To" dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Choose any Priority from the Priority dropdown list.
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

                //Step-12: Add some comments optionally for the uploaded study in Comments section.
                html5.CommentTextBox().SendKeys(Comments);
                if (html5.CommentTextBox().GetAttribute("value").Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Click on"SUBMIT"button to send the study to selected destination
                html5.ShareBtn().Click();
                if (PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), WaitTypes.Visible, 60).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Login to iCA as ph user then navigate to inbounds page and verify that the uploaded study is available
                html5.Logout_HTML5Uploader();
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step14 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step14 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Load the study and verify that the study details and attachment are available
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                //Verify the study loaded in viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step15 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()) && step15)
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

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
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

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// HTML5Uploader - HTML5 Uploader UI in IE 11
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_163452(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            UserPreferences userpref;
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
            Outbounds outbounds;
            StudyViewer StudyVw;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String MultipleUploadFiles = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MultipleUploadFiles");
                String Message = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Message");
                String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Patient1Details = PatientDetailsList.Split('|');
                //String[] Patient2Details = PatientDetailsList.Split('=')[1].Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 140805: " + new Random().Next(1, 10000);
                String Reason = "Reason test case# 140805: " + new Random().Next(1, 10000);

                //Pre-condition: Ensure the "consent for web uploader" option is enabled in domain
                login.LoginIConnect(username, password);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (!domain.WebUploaderConsentCheckbox().Selected)
                {
                    domain.WebUploaderConsentCheckbox().Click();
                }
                domain.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step-1: Launch iCA in IE11 browser and click on " Webuploader " option for iCA login page
                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("homepage");
                PageLoadWait.WaitForFrameLoad(10);
                if (html5.RegisteredUserRadioBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2: Login as st user after Registered user checkbox is selected
                html5.Login_HTML5Uploader(stuser, stpassword, HippaComplianceSelection: false);
                PageLoadWait.WaitForFrameLoad(10);
                bool step2 = PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), WaitTypes.Visible).Displayed;
                if (step2 && html5.UsernameDisplayed().Text.Equals(stuser))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Check in "I read and understood the agreement. I agree to comply to it" and click on " Continue " button
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

                //Step-4: Ensure that UPLOAD FILE(S) and DRAG & DROP YOUR FILES HERE FOR NEW UPLOAD options are available in the Upload main page
                if (html5.UploadFilesBtn().Displayed && html5.DragFilesDiv().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Ensure that options for selecting folder(s) are not available say as UPLOAD FOLDER(S) and DRAG & DROP YOUR FOLDERS HERE FOR NEW UPLOAD
                if (!IsElementVisible(html5.By_UploadFolderBtn()) && !html5.DragFilesDiv().Text.ToLower().Contains("folder"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Drag and Drop a Folder which has a DICOM file / Supported Non DICOM file 
                //Not Automated since drag and drop from file browser needs to be researched, to be implemented once a solution is found
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-7: Click on " UPLOAD FILE(S) " and select multiple DICOM files of a single study
                IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                try{ js.ExecuteScript("arguments[0].click()", html5.UploadFilesBtn()); } catch (WebDriverException ex) { Logger.Instance.ErrorLog("Exception caught while launching upload file button. Exception swallowed." + ex); }
                //html5.UploadFilesBtn().Click();
                UploadFileInBrowser(MultipleUploadFiles, "file");
                bool step7_1 = html5.UploadJobContainer().Displayed;
                PageLoadWait.WaitForHTML5StudyToUpload();
                //Patient 1:
                bool s7_1_patientname = (html5.PatientNameSpan()[0].Text == Patient1Details[0]);
                bool s7_1_DOB = (html5.DOBSpan()[0].Text == Patient1Details[1]);
                //Update for UI change for Age and Years span separation
                String TempAge, TempYrs;
                TempAge = TempYrs = String.Empty;
                if (!String.IsNullOrEmpty(Patient1Details[2]))
                {
                    TempAge = Patient1Details[2].Split(' ')[0];
                    TempYrs = Patient1Details[2].Split(' ')[1];
                }
                bool s7_1_Age = (html5.AgeSpan()[0].Text == TempAge);
                bool s7_1_YrsTxt = (html5.YearsSpan()[0].Text == TempYrs);
                bool s7_1_Gender = (html5.GenderSpan()[0].Text == Patient1Details[3]);
                bool s7_1_MRN = (html5.MRNSpan()[0].Text == Patient1Details[4]);
                bool s7_1_IPID = (html5.IPIDSpan()[0].Text == Patient1Details[5]);
                bool s7_1_StudyDesc = (html5.StudyDescSpan()[0].Text == Patient1Details[6]);
                bool s7_1_Modality = (html5.ModalitySpan()[0].Text == Patient1Details[7]);
                bool s7_1_Date = (html5.DateSpan()[0].Text == Patient1Details[8]);
                bool s7_1_Series = (html5.SeriesSpan()[0].Text == Patient1Details[9]);
                bool s7_1_Image = (html5.ImagesSpan()[0].Text == Patient1Details[10]);
                bool s7_1_Accession = (html5.AccessionSpan()[0].Text == Patient1Details[11]);
                bool s7_1_Institution = (html5.InstitutionSpan()[0].Text == Patient1Details[12]);
                bool s7_1_RefPhys = (html5.RefPhysSpan()[0].Text == Patient1Details[13]);
                Logger.Instance.InfoLog("s7_1_patientname: " + s7_1_patientname + " s7_1_DOB:" + s7_1_DOB + " s7_1_Age: " + s7_1_Age + " s7_1_YrsTxt: " + s7_1_YrsTxt + " s7_1_Gender: " + s7_1_Gender + " s7_1_MRN: " + s7_1_MRN + " s7_1_IPID: " + s7_1_IPID + " s7_1_StudyDesc :" + s7_1_StudyDesc + " s7_1_Modality: " + s7_1_Modality + " s7_1_Date: " + s7_1_Date + " s7_1_Series: " + s7_1_Series + " s7_1_Image: " + s7_1_Image + " s7_1_Accession: " + s7_1_Accession + " s7_1_Institution: " + s7_1_Institution + " s7_1_RefPhys: " + s7_1_RefPhys);
                if (step7_1 && s7_1_patientname && s7_1_DOB && s7_1_Gender && s7_1_Age && s7_1_YrsTxt && s7_1_MRN && s7_1_IPID && s7_1_StudyDesc && s7_1_Modality && s7_1_Date && s7_1_Series && s7_1_Image && s7_1_Accession && s7_1_Institution && s7_1_RefPhys)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Ensure that + Add Files and Drag-Drop options are available under JOB #1
                if (html5.AddFiles().Displayed && html5.DragDropMessage().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Ensure that options for selecting folders are not available say as + Add Folder and Dra-Drop for folder selection
                if (!IsElementVisible(html5.By_AddFolder(1)) && !html5.DragDropMessage().Text.ToLower().Contains("folder"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Drag and Drop a Folder which has DICOM files / Supported Non DICOM files under JOB #1
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-11: Drag and Drop a DICOM / Non DICOM under JOB #1
                try { js.ExecuteScript("arguments[0].click()", html5.AddFiles()); } catch (WebDriverException ex) { Logger.Instance.ErrorLog("Exception caught while launching upload file button. Exception swallowed." + ex); }
                //html5.AddFiles().Click();
                UploadFileInBrowser(AttachmentFilePath, "file");
                PageLoadWait.WaitForHTML5StudyToUpload();
                int ImageCount, SeriesCount;
                SeriesCount = Convert.ToInt32(Patient1Details[9]) + 1;
                ImageCount = Convert.ToInt32(Patient1Details[10]) + 1;
                if (html5.SeriesSpan()[0].Text == SeriesCount.ToString() && html5.ImagesSpan()[0].Text == ImageCount.ToString())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Click on"SHARE JOB #1"button from the upload main page
                js.ExecuteScript("arguments[0].click()", html5.ShareJobButton());
                //html5.ShareJobButton().Click();
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

                //Step-13: Ensure that the uploaded study in the upload main page is available with attachment in patient Content summary details
                //Patient 1:
                bool step13_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                bool step13_2 = (html5.StudyDetailsonSharePage()[0].Text == StudyCount);
                bool step13_3 = (html5.StudyDetailsonSharePage()[1].Text == Patient1Details[10]);
                bool step13_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                if (step13_1 && step13_2 && step13_3 && step13_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Select destination and Priority from the respective dropdown list then add comments
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                html5.PriorityDropdown().SelectByText(Priority);
                if (html5.DestinationDropdown().SelectedOption.Text == Config.Dest1 && html5.PriorityDropdown().SelectedOption.Text == Priority)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Click on " SUBMIT " button to send the study to selected destination
                js.ExecuteScript("arguments[0].click()", html5.ShareBtn());
                //html5.ShareBtn().Click();
                if (PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), WaitTypes.Visible, 60).Displayed && !IsElementVisible(html5.By_UploadJobContainer()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: login to iCA as "ph"user and go to Inbounds page, Search for the exams sent from HTML5 Uploader in iCA
                html5.Logout_HTML5Uploader();
                CloseHTML5Window(HTML5WindowHandle[1], HTML5WindowHandle[0]);
                PageLoadWait.WaitForFrameLoad(10);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                {
                    Driver.Quit();
                    Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url); 
                }
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step16 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID, "Uploaded" });
                if (step16 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Load the study on viewer and verify that the study details are matched with the uploaded one.
                login.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                String Patientinfo = bluringviewer.PatinetName().Text;
                //Verify the study loaded in viewer
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step15 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (Patientinfo.ToLower().Contains(Patient1Details[0].ToLower()) && Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(PatientID.ToLower()) && step15)
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

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                result.steps[++ExecutedSteps].SetLogs();
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
        ///  HTML5 Uploader - Upload Malicious Dicom File
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_167928(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            DomainManagement domain;
            TestCaseResult result;
            Inbounds inbounds;
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
                String aruser = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String phuser = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                String PatientDetailsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails");
                String MultipleUploadFiles = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MultipleUploadFiles");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                //String StudyCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyCount");
                String[] PatientID = ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID").ToString().Split('=');
                String[] AccessionID = ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList").ToString().Split('=');
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String Phonenumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Phonenumber");
                String AttachmentFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentFilePath");
                String[] Modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split(':');
                String[] Patient1Details = PatientDetailsList.Split('=')[0].Split('|');
                String[] Patient2Details = PatientDetailsList.Split('=')[1].Split('|');
                String[] Patient3Details = PatientDetailsList.Split('=')[2].Split('|');
                String[] Patient4Details = PatientDetailsList.Split('=')[3].Split('|');
                String Priority = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Priority");
                String Comments = "Comments entered in test case# 134131: " + new Random().Next(1, 10000);

                //Pre-condition: Ensure the "consent for web uploader" option is enabled in domain
                var Precondition = new Action(() =>
                {
                    login.LoginIConnect(username, password);
                    domain = login.Navigate<DomainManagement>();
                    domain.SearchDomain(DomainName);
                    domain.SelectDomain(DomainName);
                    domain.ClickEditDomain();
                    if (!domain.WebUploaderConsentCheckbox().Selected)
                    {
                        domain.WebUploaderConsentCheckbox().Click();
                    }
                    domain.ClickSaveEditDomain();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    login.Logout();
                });
                Precondition();
                //Step-1: Login as a user from webuploader page
                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("homepage");
                PageLoadWait.WaitForFrameLoad(10);
                html5.Login_HTML5Uploader(stuser, stpassword, HippaComplianceSelection: true);
                bool step1 = PageLoadWait.WaitForElement(html5.By_UploadFolderBtn(), WaitTypes.Visible).Displayed;
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-2: Upload the files using upload file button
                html5.UploadFilesBtn().Click();
                UploadFileInBrowser(MultipleUploadFiles, "file");
                bool step2_1 = html5.UploadJobContainer().Displayed;

                // Wait until the progress bar gets completed
                PageLoadWait.WaitForHTML5StudyToUpload();
                bool step2 = wait.Until<bool>(driver =>
                {
                    if (html5.UploadJobContainer().GetAttribute("class").Contains("highlightedContainer"))
                        return true;
                    else
                        return false;
                });
                if (step2_1 && step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Attach JPEG or PDF to the uploaded files
                html5.AttachmentButtons()[0].Click();
                ExecutedSteps++;
                UploadFileInBrowser(AttachmentFilePath, "file");
                ExecutedSteps++;

                //Step-4: Click on share job button
                PageLoadWait.WaitForHTML5StudyToUpload();
                html5.ShareJobButton().Click();
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

                //Step-5: Verify the study/patient information displayed in the cards(share job page) are correct
                //Patient 1:
                bool step5_1_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                //bool step5_1_2 = (html5.GenderSpan()[0].Text == Patient1Details[1]);
                //bool step5_1_3 = (html5.AccessionSpan()[0].Text == Patient1Details[2]);
                bool step5_1_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                //Patient 2:
                bool step5_2_1 = (html5.PatientNameonSharePage(1).Text == Patient2Details[0]);
                //bool step5_2_2 = (html5.GenderSpan()[0].Text == Patient2Details[1]);
                //bool step5_2_3 = (html5.AccessionSpan()[0].Text == Patient2Details[2]);
                bool step5_2_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage(1)[2].Text) == 0);

                if (step5_1_1 && step5_1_4 && step5_2_1 && step5_2_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Patient 1:
                //bool s6_1_patientname = (html5.PatientNameSpan()[0].Text == Patient1Details[0]);
                //bool s6_1_DOB = (html5.DOBSpan()[0].Text == Patient1Details[1]);
                ////Update for UI change for Age and Years span separation
                //String TempAge, TempYrs;
                //TempAge = TempYrs = String.Empty;
                //if (!String.IsNullOrEmpty(Patient1Details[2]))
                //{
                //    TempAge = Patient1Details[2].Split(' ')[0];
                //    TempYrs = Patient1Details[2].Split(' ')[1];
                //}
                //bool s6_1_Age = (html5.AgeSpan()[0].Text == TempAge);
                //bool s6_1_YrsTxt = (html5.YearsSpan()[0].Text == TempYrs);
                //bool s6_1_Gender = (html5.GenderSpan()[0].Text == Patient1Details[3]);
                //bool s6_1_MRN = (html5.MRNSpan()[0].Text == Patient1Details[4]);
                //bool s6_1_IPID = (html5.IPIDSpan()[0].Text == Patient1Details[5]);
                //bool s6_1_StudyDesc = (html5.StudyDescSpan()[0].Text == Patient1Details[6]);
                //bool s6_1_Modality = (html5.ModalitySpan()[0].Text == Patient1Details[7]);
                //bool s6_1_Date = (html5.DateSpan()[0].Text == Patient1Details[8]);
                //bool s6_1_Series = (html5.SeriesSpan()[0].Text == Patient1Details[9]);
                //bool s6_1_Image = (html5.ImagesSpan()[0].Text == Patient1Details[10]);
                //bool s6_1_Accession = (html5.AccessionSpan()[0].Text == Patient1Details[11]);
                //bool s6_1_Institution = (html5.InstitutionSpan()[0].Text == Patient1Details[12]);
                //bool s6_1_RefPhys = (html5.RefPhysSpan()[0].Text == Patient1Details[13]);
                ////Patient 2:
                //bool s6_2_patientname = (html5.PatientNameSpan()[1].Text == Patient2Details[0]);
                //bool s6_2_DOB = (html5.DOBSpan()[1].Text == Patient2Details[1]);
                //if (!String.IsNullOrEmpty(Patient2Details[2]))
                //{
                //    TempAge = Patient2Details[2].Split(' ')[0];
                //    TempYrs = Patient2Details[2].Split(' ')[1];
                //}
                //else
                //{
                //    TempAge = TempYrs = String.Empty;
                //}
                //bool s6_2_Age = (html5.AgeSpan()[1].Text == TempAge);
                //bool s6_2_YrsTxt = (html5.YearsSpan()[1].Text == TempYrs);
                //bool s6_2_Gender = (html5.GenderSpan()[1].Text == Patient2Details[3]);
                //bool s6_2_MRN = (html5.MRNSpan()[1].Text == Patient2Details[4]);
                //bool s6_2_IPID = (html5.IPIDSpan()[1].Text == Patient2Details[5]);
                //bool s6_2_StudyDesc = (html5.StudyDescSpan()[1].Text == Patient2Details[6]);
                //bool s6_2_Modality = (html5.ModalitySpan()[1].Text == Patient2Details[7]);
                //bool s6_2_Date = (html5.DateSpan()[1].Text == Patient2Details[8]);
                //bool s6_2_Series = (html5.SeriesSpan()[1].Text == Patient2Details[9]);
                //bool s6_2_Image = (html5.ImagesSpan()[1].Text == Patient2Details[10]);
                //bool s6_2_Accession = (html5.AccessionSpan()[1].Text == Patient2Details[11]);
                //bool s6_2_Institution = (html5.InstitutionSpan()[1].Text == Patient2Details[12]);
                //bool s6_2_RefPhys = (html5.RefPhysSpan()[1].Text == Patient2Details[13]);
                //Logger.Instance.InfoLog("s6_1_patientname: " + s6_1_patientname + " s6_1_DOB:" + s6_1_DOB + " s6_1_Age: " + s6_1_Age + " s6_1_YrsTxt: " + s6_1_YrsTxt + " s6_1_Gender: " + s6_1_Gender + " s6_1_MRN: " + s6_1_MRN + " s6_1_IPID: " + s6_1_IPID + " s6_1_StudyDesc :" + s6_1_StudyDesc + " s6_1_Modality: " + s6_1_Modality + " s6_1_Date: " + s6_1_Date + " s6_1_Series: " + s6_1_Series + " s6_1_Image: " + s6_1_Image + " s6_1_Accession: " + s6_1_Accession + " s6_1_Institution: " + s6_1_Institution + " s6_1_RefPhys: " + s6_1_RefPhys);
                //Logger.Instance.InfoLog("s6_2_patientname: " + s6_2_patientname + " s6_2_DOB:" + s6_2_DOB + " s6_2_Age: " + s6_2_Age + " s6_2_YrsTxt: " + s6_2_YrsTxt + " s6_2_Gender: " + s6_2_Gender + " s6_2_MRN: " + s6_2_MRN + " s6_2_IPID: " + s6_2_IPID + " s6_2_StudyDesc :" + s6_2_StudyDesc + " s6_2_Modality: " + s6_2_Modality + " s6_2_Date: " + s6_2_Date + " s6_2_Series: " + s6_2_Series + " s6_2_Image: " + s6_2_Image + " s6_2_Accession: " + s6_2_Accession + " s6_2_Institution: " + s6_2_Institution + " s6_2_RefPhys: " + s6_2_RefPhys);

                //if (s6_1_patientname && s6_1_DOB && s6_1_Gender && s6_1_Age && s6_1_YrsTxt && s6_1_MRN && s6_1_IPID && s6_1_StudyDesc && s6_1_Modality && s6_1_Date && s6_1_Series && s6_1_Image && s6_1_Accession && s6_1_Institution && s6_1_RefPhys && s6_2_patientname && s6_2_DOB && s6_2_Gender && s6_2_Age && s6_2_YrsTxt && s6_2_MRN && s6_2_IPID && s6_2_StudyDesc && s6_2_Modality && s6_2_Date && s6_2_Series && s6_2_Image && s6_2_Accession && s6_2_Institution && s6_2_RefPhys)
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

                //Step-6: Submit without any change
                html5.ShareBtn().Click();
                PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), WaitTypes.Visible, 120);
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
                html5.Logout_HTML5Uploader();


                //Driver.Quit();
                //Driver = null;
                //login.InvokeBrowser(Config.BrowserType);


                //Step-7: From another browser, login to iCA as receiver user and verify the uploaded studies
                login.DriverGoTo(login.url);
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID[0]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step7_1 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[0], "Uploaded" });
                login.SearchStudy("Patient ID", PatientID[1]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step7_2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[1], "Uploaded" });
                login.SearchStudy("Patient ID", PatientID[2]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step7_3 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[2], "Uploaded" });
                login.SearchStudy("Patient ID", PatientID[3]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step7_4 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[3], "Uploaded" });
                login.SearchStudy("Patient ID", PatientID[4]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step7_5 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[4], "Uploaded" });
                if (step7_1 != null && step7_2 != null && step7_3 != null && step7_4 != null && step7_5 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8:Select an uploaded study and click on Nominate for archive button/ Transfer button/ Reconcile Exam button/ Email study button etc and verify all the file information displayed are correct (ie, special characters are displayed as it is - not HTML escaped, ie for eg: !@#$%& should be !@#$%&)
                login.SearchStudy("Patient ID", PatientID[0]);
                PageLoadWait.WaitForSearchLoad();
                login.SelectStudy("Patient ID", PatientID[0]);
                bool TransferBtn = inbounds.TransferBtn().Enabled;
                bool NominateBtn = inbounds.NominateForArchiveBtn().Enabled;
                IWebElement EmailStudy = BasePage.Driver.FindElement(By.CssSelector(inbounds.EmailStudyBtn));
                bool EmailBtn = EmailStudy.Enabled;
                if (NominateBtn && TransferBtn && EmailBtn)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Logout of the user
                login.Logout();
                ExecutedSteps++;

                //Step-10: Cleanup the Inbound - delete all uploaded studies from holding pen since the below scenario will upload the same set of studies again.
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl(hpurl);
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string aid in AccessionID)
                {
                    workflow.HPSearchStudy("Accessionno", aid);
                    try { workflow.HPDeleteStudy(); } catch (Exception) { }
                }
                hplogin.LogoutHPen();

                login.CloseBrowser();
                login.InvokeBrowser(Config.BrowserType);
                Precondition();
                //Step-11: Login as a user from webuploader page
                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle1 = OpenHTML5UploaderandSwitchtoIT("homepage");
                PageLoadWait.WaitForFrameLoad(10);
                html5.Login_HTML5Uploader(stuser, stpassword, HippaComplianceSelection: true);
                bool step11 = PageLoadWait.WaitForElement(html5.By_UploadFolderBtn(), WaitTypes.Visible).Displayed;
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

                //Step-12: Upload the folder (see the location: \\10.4.16.130\anonymized_data\Data Sets by VP\iCA\VP_ImageSharing\Malicious DICOM studies- #167928) using upload folder button
                html5.UploadFolderBtn().Click();
                Thread.Sleep(5000);
                UploadFileInBrowser(UploadFilePath, "folder");
                //bool step12_2 = html5.UploadJobProgressBar().Displayed;
                bool step12_1 = html5.UploadJobContainer().Displayed;
                if (step12_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Attach JPEG or PDF to the uploaded studies
                html5.AttachmentButtons()[0].Click();
                UploadFileInBrowser(AttachmentFilePath, "file");
                ExecutedSteps++;

                //Step-14: Click on share job button
                PageLoadWait.WaitForHTML5StudyToUpload();
                html5.ShareJobButton().Click();
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
                //Step-15: Verify the study/patient information displayed in the cards(share job page) are correct
                bool step15_1_1 = (html5.PatientNameonSharePage().Text == Patient1Details[0]);
                //bool step15_1_2 = (html5.GenderSpan()[0].Text == Patient1Details[1]);
                //bool step15_1_3 = (html5.AccessionSpan()[0].Text == Patient1Details[2]);
                bool step15_1_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage()[2].Text) == 1);
                //Patient 2:
                bool step15_2_1 = (html5.PatientNameonSharePage(1).Text == Patient2Details[0]);
                //bool step15_2_2 = (html5.GenderSpan()[0].Text == Patient2Details[1]);
                //bool step15_2_3 = (html5.AccessionSpan()[0].Text == Patient2Details[2]);
                bool step15_2_4 = (Convert.ToInt32(html5.StudyDetailsonSharePage(1)[2].Text) == 0);

                if (step15_1_1 && step15_1_4 && step15_2_1 && step15_2_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Submit without any change
                html5.ShareBtn().Click();
                PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), WaitTypes.Visible, 120);
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
                html5.Logout_HTML5Uploader();

                //Step-17: From another browser, login to iCA as receiver user and verify the uploaded studies
                login.DriverGoTo(login.url);
                login.LoginIConnect(phuser, phpassword);
                inbounds = login.Navigate<Inbounds>();
                login.ClearFields(1);
                login.SearchStudy("Patient ID", PatientID[0]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step17_1 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[0], "Uploaded" });
                login.SearchStudy("Patient ID", PatientID[1]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step17_2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[1], "Uploaded" });
                login.SearchStudy("Patient ID", PatientID[2]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step17_3 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[2], "Uploaded" });
                login.SearchStudy("Patient ID", PatientID[3]);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<string, string> step17_4 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status" }, new string[] { PatientID[3], "Uploaded" });
                if (step17_1 != null && step17_2 != null && step17_3 != null && step17_4 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18: Select an uploaded study and click on Nominate for archive button/ Transfer button/ Reconcile Exam button/ Email study button etc and verify all the file information displayed are correct (ie, special characters are displayed as it is - not HTML escaped, ie for eg: !@#$%& should be !@#$%&)
                login.SearchStudy("Patient ID", PatientID[0]);
                PageLoadWait.WaitForSearchLoad();
                login.SelectStudy("Patient ID", PatientID[0]);
                TransferBtn = inbounds.TransferBtn().Enabled;
                NominateBtn = inbounds.NominateForArchiveBtn().Enabled;
                EmailStudy = BasePage.Driver.FindElement(By.CssSelector(inbounds.EmailStudyBtn));
                EmailBtn = EmailStudy.Enabled;

                if (NominateBtn && TransferBtn && EmailBtn)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19: Logout of the user
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
