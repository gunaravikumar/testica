using System;
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
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium.Remote;
using System.Diagnostics;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using Application = TestStack.White.Application;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems;
using TestStack.White.Configuration;
using Dicom;
using Dicom.Network;

namespace Selenium.Scripts.Tests
{
    class WorkFlowBR : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public MPHomePage mphomepage { get; set; }
        public Tool mpactool { get; set; }
        public HTML5_Uploader html5 { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public WorkFlowBR(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            html5 = new HTML5_Uploader();
        }

        ///<summary>
        /// Test 130892 - Workflow_Radiologist
        /// </summary>
        /// 
        public TestCaseResult Test_161312(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            ServiceTool tool = new ServiceTool();
            Studies studies = new Studies();
            BasePage basepage = new BasePage();
            StudyViewer viewer = new StudyViewer();
            DomainManagement domainManagement = new DomainManagement();
            RoleManagement roleManagement = new RoleManagement();
            UserManagement userManagement = new UserManagement();
            int ExecutedSteps = -1;
            String adminUsername = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String phUsername = Config.LdapPHUser;
            String phPassword = Config.LdapUserPassword;
            result.SetTestStepDescription(teststeps);
            UserPreferences UserPref = new UserPreferences();

            String patientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String[] patientID = patientIDList.Split(':');
            try
            {
                String HostName = basepage.GetHostName(Config.IConnectIP);
                String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedDate");
                String[] studyDate = StudyDateList.Split(':');
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Studypath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                String[] datasource = new String[] { basepage.GetHostName(Config.EA77), basepage.GetHostName(Config.EA91) };
                String hostName = tool.GetHostName(Config.IConnectIP);
                FileUtils.AddToHostsFile(Config.IConnectIP + " " + hostName.ToLower() + ".merge.com");

                //PreCondition - Email Notification Setup 
                ServiceTool serviceTool = new ServiceTool();
                serviceTool.InvokeServiceTool();
                serviceTool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SystemEmail: Config.SystemEmail, SMTPHost: Config.SMTPServer);
				serviceTool.CloseServiceTool();
				
				login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminUserName);
                domainManagement =  (DomainManagement)login.Navigate("DomainManagement");
                domainManagement.SearchDomain(Config.adminGroupName);
                domainManagement.SelectDomain(Config.adminGroupName);
                domainManagement.EditDomainButton().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                var precond1 = login.GetConfiguredToolsInToolBoxConfig();
                if (!(login.GetConfiguredToolsInToolBoxConfig().Contains("Save Annotated Images")))
                {
                    var dictionary = new Dictionary<String, IWebElement>();
                    IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                    dictionary.Add("Save Annotated Images", group1);
                    domainManagement.AddToolsToToolbox(dictionary);
                }
                domainManagement.SaveDomain();
                domainManagement.ClickSaveDomain();
                login.Logout();
                

                //Steop-1 - Setup LDAP server in service tool
                Taskbar taskbar = new Taskbar();             
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                PageLoadWait.WaitForFrameLoad(10);
                bool step1 = basepage.GetCurrentSelectedtab().Equals("Studies");
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

                //Step 2: Change the Study Performed to appropriate date to view studies required and add the CT in the Modality field.
                //Add the PID"999"and click on search
                studies = login.Navigate<Studies>();
                PageLoadWait.WaitForFrameLoad(10);
                studies.ClearFields();
                studies.SelectCustomeStudySearch(studies.StudyPerformed());
                studies.FromDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.fromdatecalender()));
                studies.EnterDate_CustomSearch(studyDate[0]);
                studies.ToDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.todatecalender()));
                studies.EnterDate_CustomSearch(studyDate[1], fieldtype: "to");
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
                {
                    studies.ClickElement(studies.SubmitButton());
                }
                else
                {
                    studies.SubmitButton().Click();
                }
                PageLoadWait.WaitForFrameLoad(10);
                studies.PatientID().Clear();
                studies.PatientID().SendKeys(patientID[0]);
                PageLoadWait.WaitForFrameLoad(10);
                studies.Modality().Clear();
                studies.Modality().SendKeys(Modality);
                PageLoadWait.WaitForFrameLoad(10);
                studies.JSSelectDataSource("AUTO-SSA-001");
                PageLoadWait.WaitForFrameLoad(10);
                studies.ClickSearchBtn();
                PageLoadWait.WaitForLoadingMessage(30);
                PageLoadWait.WaitForFrameLoad(10);
                Dictionary<String, String> row = studies.
                    GetMatchingRow(new string[] { "Patient ID", "Modality" }, new string[] { patientID[0], Modality });
                if (row != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3 UsePrefence - Viewing Scope CT - Image
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                var userpref = studies.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                userpref.ModalityDropDown().SelectByText("CT");
                if (UserPref.ViewingScopeImageRadioBtn().Selected == false)
                    UserPref.ViewingScopeImageRadioBtn().Click();
                UserPref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 4: Open the PID 999 in the viewer
                studies.SelectStudy("Patient ID", patientID[0]);
                var brviewer = BluRingViewer.LaunchBluRingViewer();
                if ((brviewer.GetViewPortCount(1) == 4) &&
                     (brviewer.PatientDetailsInViewer()["PatientID"].Equals(patientID[0])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5: Double Click in the first viewer port
                Thread.Sleep(5000);
                //new TestCompleteAction().DoubleClick(brviewer.
                //  GetElement(SelectorType.CssSelector, brviewer.Activeviewport)).Perform();
                new Actions(BasePage.Driver).DoubleClick(brviewer.
                  GetElement(SelectorType.CssSelector, brviewer.Activeviewport)).Build().Perform();
                var step5 = result.steps[++ExecutedSteps];
                step5.SetPath(testid, ExecutedSteps);
                var imagecopare5 = brviewer.CompareImage(step5, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (imagecopare5)
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

                //Step 6: Apply W/L in the image and then apply zoom
                brviewer.SelectViewerTool(BluRingTools.Window_Level);
                brviewer.ApplyTool_WindowWidth();
                Thread.Sleep(2000);
                brviewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                brviewer.ApplyTool_Zoom();
                Thread.Sleep(2000);
                var step6 = result.steps[++ExecutedSteps];
                step6.SetPath(testid, ExecutedSteps);
                var icompare6 = brviewer.CompareImage(step6, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare6)
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

                //Step 7: Double click in the window
                //new TestCompleteAction().DoubleClick
                //    (brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport)).Perform();
                new Actions(BasePage.Driver).DoubleClick(brviewer.
                GetElement(SelectorType.CssSelector, brviewer.Activeviewport)).Build().Perform();
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                var icompare7 = brviewer.CompareImage(step7, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare7)
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

                //Step 8: Click on the Reset icon
                brviewer.SelectViewerTool(BluRingTools.Reset);
                var step8 = result.steps[++ExecutedSteps];
                step8.SetPath(testid, ExecutedSteps);
                var icompare8 = brviewer.CompareImage(step8, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare8)
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

                //Step-9 - Close Sudy viewer
                brviewer.CloseBluRingViewer();
                ExecutedSteps++;

                // Step 10: Select the User Preferences and change the viewing scope for the CT modality to series scope
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                userpref = studies.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                userpref.ModalityDropDown().SelectByText("CT");
                if (UserPref.ViewingScopeSeriesRadioBtn().Selected == false)
                    UserPref.ViewingScopeSeriesRadioBtn().Click();
                UserPref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 11: Reload the same study PID=999
                studies.SelectStudy("Patient ID", patientID[0]);
                BluRingViewer.LaunchBluRingViewer();
                brviewer.SetViewPort(1, 1);
                ExecutedSteps++;

                //Step 12: Select the Zoom icon and apply it to the full screen image
                brviewer.SelectViewerTool(BluRingTools.Interactive_Zoom, 1, 2);
                brviewer.ApplyTool_Zoom();
                var step13 = result.steps[++ExecutedSteps];
                step13.SetPath(testid, ExecutedSteps);
                var icompare13 = brviewer.CompareImage(step13, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare13)
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

                //Step 13: Select the W / L and apply it , also apply several other, rotate, pan, draw a circle add some text.
                brviewer.SelectViewerTool(BluRingTools.Pan, 1, 2);
                brviewer.ApplyTool_Pan();
                brviewer.SelectViewerTool(BluRingTools.Draw_Ellipse, 1, 2);
                brviewer.ApplyTool_DrawEllipse();
                brviewer.SelectViewerTool(BluRingTools.Add_Text, 1, 2);
                brviewer.ApplyTool_AddText("Test");
                var step14 = result.steps[++ExecutedSteps];
                step14.SetPath(testid, ExecutedSteps);
                var icompare14 = brviewer.CompareImage(step14, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare14)
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

                //Step 14 - Save the annotated Image
                var thumbnailcount15 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails)).Count;
                brviewer.SavePresentationState(BluRingTools.Save_Annotated_Image, panel: 1, viewport: 2);
                ExecutedSteps++;

                //Step-15 - Check PR in first thumbnail
                var step16 = result.steps[++ExecutedSteps];
                var thumbnail = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails))[0];
                step16.SetPath(testid, ExecutedSteps);
                var iscompare16 = brviewer.CompareImage(step16, thumbnail);
                var thumbnailcount16 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails)).Count;
                if (iscompare16 && (thumbnailcount16 == thumbnailcount15 + 1))
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


                //Step-16 -Scroll through Images and verify
                var viewport = brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport);
                new TestCompleteAction().MouseScroll(viewport, "down", "18").Perform();
                var step17 = result.steps[++ExecutedSteps];
                step17.SetPath(testid, ExecutedSteps);
                var icompare17 = brviewer.CompareImage(step17, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare17)
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

                //Step 17: Close the viewer
                brviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step 18: Go to User preferences and change the Default Settings
                //Per Modality change the Modality CT and change the Viewing Scope to Image click OK
                UserPref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                UserPref.ModalityDropDown().SelectByText("CT");
                if (UserPref.ViewingScopeImageRadioBtn().Selected == false)
                    UserPref.ViewingScopeImageRadioBtn().Click();
                UserPref.CloseUserPreferences();
                ExecutedSteps++;

                //Step 19: Load the same PID=999 study
                PageLoadWait.WaitForFrameLoad(10);
                studies.ClearFields();
                studies.SelectCustomeStudySearch(studies.StudyPerformed());
                studies.FromDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.fromdatecalender()));
                studies.EnterDate_CustomSearch(studyDate[0]);
                studies.ToDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.todatecalender()));
                studies.EnterDate_CustomSearch(studyDate[1], fieldtype: "to");
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
                {
                    studies.ClickElement(studies.SubmitButton());
                }
                else
                {
                    studies.SubmitButton().Click();
                }
                studies.PatientID().Clear();
                studies.PatientID().SendKeys(patientID[0]);
                studies.Modality().Clear();
                //studies.Modality().SendKeys(Modality);
                studies.JSSelectDataSource("AUTO-SSA-001");
                studies.ClickSearchBtn();
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy1("Patient ID", patientID[0]);
                BluRingViewer.LaunchBluRingViewer();
                brviewer.SetViewPort(2, 1);
                ExecutedSteps++;

                //Step 20:Go to the viewing port with the Axial head series and double click on the image
                new TestCompleteAction().DoubleClick
                    (brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport)).Perform();
                var step21 = result.steps[++ExecutedSteps];
                step21.SetPath(testid, ExecutedSteps);
                var icomapre21 = brviewer.CompareImage(step21, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icomapre21)
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

                //Step 21: Change the W/L and Pan on the Series 2 Image 1    
                brviewer.SelectViewerTool(BluRingTools.Pan, 1, 3);
                brviewer.ApplyTool_Pan();
                brviewer.SelectViewerTool(BluRingTools.Window_Level, 1, 3);
                brviewer.ApplyTool_WindowWidth();
                var step22 = result.steps[++ExecutedSteps];
                step22.SetPath(testid, ExecutedSteps);
                var icomapre22 = brviewer.CompareImage(step22, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icomapre22)
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


                //Step-22
                viewport = brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport);
                new TestCompleteAction().MouseScroll(viewport, "down", "18").Perform();
                var step23 = result.steps[++ExecutedSteps];
                step23.SetPath(testid, ExecutedSteps);
                var icompare23 = brviewer.CompareImage(step23, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare23)
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

                //Step-23
                var step24 = result.steps[++ExecutedSteps];
                step24.SetPath(testid, ExecutedSteps);
                brviewer.SelectViewerTool(BluRingTools.Add_Text, 1, 3);
                brviewer.ApplyTool_AddText("Test");
                brviewer.SelectInnerViewerTool(BluRingTools.Flip_Vertical, BluRingTools.Flip_Horizontal, panel: 1, viewport: 3);
                var icompare24 = brviewer.CompareImage(step24, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare24)
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


                //Step 24: Scroll through all the images and confirm the attributes are only on image 1 and image 18
                new TestCompleteAction().MouseScroll(viewport, "up", "10").Perform();
                var step25 = result.steps[++ExecutedSteps];
                step25.SetPath(testid, ExecutedSteps);
                var icompare25 = brviewer.CompareImage(step25, brviewer.GetElement(SelectorType.CssSelector, brviewer.Activeviewport));
                if (icompare25)
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

                //Step-25 - close the viewer and load any CT/MR study  
                brviewer.CloseBluRingViewer();
                Studies studies1 = (Studies)login.Navigate("Studies");
                studies1.SearchStudy(patientID: patientID[1], Datasource: datasource[1], Modality : "CT");
                studies1.SelectStudy("Patient ID", patientID[1]);
                BluRingViewer blueRingViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step-26-Make All mails As Read before sending Mail. Email the Study.
                Dictionary<string, string> downloadedMail;
                EmailUtils customUser1Email = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword};
                customUser1Email.MarkAllMailAsRead("INBOX");               
                String pinnumber = blueRingViewer.EmailStudy_BR(Config.CustomUser1Email , DeleteEmail : false);
                if (pinnumber == null && (String.IsNullOrWhiteSpace(pinnumber)))
                {
                    blueRingViewer.CloseBluRingViewer();
                    throw new Exception("Error While Get the PINNumber by Email Study");
                }
                result.steps[++ExecutedSteps].StepPass();

                //Step 27 - Go to the destination Email and Check that the "Emailed Study" notification is received.
                downloadedMail = customUser1Email.GetMailUsingIMAP(Config.SystemEmail,"Emailed Study");
                var emaillink = customUser1Email.GetEmailedStudyLink(downloadedMail);
                if (downloadedMail != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 28 
                blueRingViewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                var step28 = result.steps[++ExecutedSteps];
                step28.SetPath(testid, ExecutedSteps);
                var icompare28 = brviewer.CompareImage(step28, brviewer.ViewPortContainer() );
                if (icompare28)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Report Result      
                login.Logout();
                login.DriverGoTo(login.url);
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
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
                try
                {
                    // patientID  = PID27916
                    HPLogin hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA77 + "/webadmin");
                    HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.EA77 + "/webadmin");
                    WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", patientID[0]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.DeletePaticularModality("PR");
                    hplogin.LogoutHPen();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("PR delete exception -- " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

            }
        }

        ///<summary>
        /// Test 163527  - Workflow_iCA_ImageSharing
        /// </summary>
        ///
        public TestCaseResult Test_163527(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            ServiceTool tool = new ServiceTool();
            Studies studies = new Studies();
            BasePage basepage = new BasePage();
            StudyViewer viewer = new StudyViewer();
            Inbounds inbounds = new Inbounds();
            Outbounds outbounds = new Outbounds();
            UserManagement userManagement = new UserManagement();
            ExamImporter ei = new ExamImporter();
            MpacLogin mplogin = new MpacLogin();
            int ExecutedSteps = -1;

            String adminUsername = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String SuperAdminGroup = Config.adminGroupName;
            String nuUsername = Config.ph1UserName;
            String nuPassword = Config.ph1Password;
            String EIWindowName = Config.eiwindow;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String HostName = basepage.GetHostName(Config.IConnectIP);
                String domain = "SuperAdminGroup";
                String stuser = Config.stUserName;
                String stpassword = Config.stPassword;

                //String[] datasourceList = new string[] { basepage.GetHostName(Config.DestEAsIp), basepage.GetHostName(Config.SanityPACS) };

                string[] UploadFilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath2")).Split('=');
                String[] patientInfo = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails")).Split(':');
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                string[] PatientDetailsList = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDetails2")).Split('>');
                string[] Patient1Details = PatientDetailsList[0].Split('|');
                string[] FirstName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName")).Split(':');

                //String[] EI_ImageFilePath = Directory.GetFiles(downloadpath[3]);
                //String[] ReportPath = Directory.GetFiles(downloadpath[4]);
                String[] patientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String[] patientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName")).Split(':');
                String[] accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID")).Split(':');
                String[] order = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "OrderNotes")).Split(':');
                //String emailId = Config.EmailUserName;
                String eiWindowName = "EISetUp" + new Random().Next(1, 1000);
                string Priority = "ROUTINE";

                //EmailUtils myEmail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };                

                //generate EI in service tool
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                tool.LaunchServiceTool();
                tool.GenerateInstallerAllDomain(domain, eiWindowName);
                wpfobject.WaitTillLoad();
                tool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                tool.CloseConfigTool();
                taskbar.Show();

                ServiceTool serviceTool = new ServiceTool();
                serviceTool.InvokeServiceTool();
                serviceTool.SetEmailNotificationForPOP(Config.POPMailHostname);
                serviceTool.CloseServiceTool();

                //Delete Studies from Holding Pen
                HPLogin hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.HoldingPenIP + "/webadmin");
                HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.HoldingPenIP + "/webadmin");
                WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", patientID[0]);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                    workflow.HPDeleteStudy();
                workflow.HPSearchStudy("PatientID", patientID[1]);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                    workflow.HPDeleteStudy();
                hplogin.LogoutHPen();

                //String datasource = basepage.GetHostName(Config.DestinationPACS);
                //step 1: goto iconnect login screen
                login.DriverGoTo(login.url);
                PageLoadWait.WaitForPageLoad(10);
                //PageLoadWait.WaitForFrameLoad(10);

                if (IsElementVisible(login.By_CDUploaderInstallBtn()) && IsElementVisible(login.By_WebUploadBtn()) && IsElementVisible(login.By_DownloadPACSGoBtn()))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 2: Click on User Install and save the MSI File ( the Windows Installer Package)
                ei._examImporterInstance = eiWindowName;
                //String EIFilePath = @"C:\Users\All Users\Apps\" + ei._examImporterInstance + @"\bin\UploaderTool.exe";
                //string eiInstallPath = ei.EI_Installation_Alt(nuUsername, nuPassword, eiWindowName, domain, Config.Inst1, Config.downloadpath, UploaderToolPath: EIFilePath);
                string eiInstallPath = ei.EI_Installation(Domainname: domain, eiWindow: eiWindowName, InstName: Config.Inst1, Username: nuUsername, Password: nuPassword);

                if (File.Exists(Config.downloadpath + @"\Installer.UploaderTool.msi"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                //Step 3: Execute the MSI and Install the Exam importer on the client system.install the EI using the user credentials (nu / pwd.13579)               
                ExecutedSteps++; //Steps covered in step 2              

                //step 4, 5, 6
                //Pop Installation - Covered in the Environment steup.
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                result.steps[++ExecutedSteps].StepPass();
                

                //Step 7: Login to the Exam Importer with the Nurse credentials
                ei.LaunchEI(eiInstallPath);
                wpfobject.GetMainWindow(eiWindowName);
                wpfobject.GetMainWindowByTitle(ei._examImporterInstance);
                WpfObjects._mainWindow.WaitWhileBusy();
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindowByTitle(ei._examImporterInstance);
                wpfobject.WaitTillLoad();
                ei.LoginToEi(nuUsername, nuPassword, EIWindowName: ei._examImporterInstance);
                wpfobject.GetMainWindowByTitle(ei._examImporterInstance);

                if (ei.DestinationDropdown().Visible &&
                    ei.Priority().Visible &&
                    ei.StudiesFrom().Visible &&
                    ei.PatientListDropdown().Visible &&
                    ei.SendBtn().Visible &&
                    ei.ClearBtn().Visible)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 8: Click on the Folder on the"Studies From bar"and Search for studies in a local folder in the servers system.and select a folder and click oK.
                //Select Destination
                ei.EI_SelectDestination(Config.Dest1, EIWindowName: ei._examImporterInstance);
                wpfobject.ClickButton("BtnOpenFolder");
                wpfobject.InteractWithTree(Config.EI_TestDataPath + FilePath[0]);
                wpfobject.ClickButton("1");
                bool step5 = false;
                try { if (wpfobject.GetLabel("MessageText").Text.Equals("The program will scan entire selected directory. This operation may take some time. Do you wish to continue?")) { step5 = true; } } catch { }
                if (step5)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 9: Click on Yes to continue
                wpfobject.ClickButton("yesButton");
                wpfobject.WaitTillLoad();
                if (ei.StudyGridMain().Visible)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 10: Select one patient from the"Select Patient"Drop down
                wpfobject.WaitTillLoad();
                ei.PatientListDropdown().Click();
                wpfobject.WaitTillLoad();
                try { ei.PatientListDropdown().Item(2).Check(); } catch { }
                wpfobject.WaitTillLoad();
                ei.PatientListDropdown().Click();
                wpfobject.WaitTillLoad();

                if (ei.dataGrid()[1].Visible)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 11: Click on the Plus sign on the left to open the details for the study
                IUIItem[] PlusSignList = wpfobject.GetMultipleElements("ShowDetails");
                PlusSignList[0].Click();
                wpfobject.WaitTillLoad();
                Thread.Sleep(5000);

                if (ei.StudyInDetails().Visible)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 12: Select several studies from the Patient Drop down and click on send.
                ei.SelectAllPatientsToUpload(EIWindowName: ei._examImporterInstance);
                ei.Send(EIWindowName: ei._examImporterInstance);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 13: In the EI select a folder that does not have any Dicom files and say yes to the pop up that asks for the creation of a new Patient
                ei.ClearBtn().Click();
                wpfobject.WaitTillLoad();
                ei.EI_SelectDestination(Config.Dest1, EIWindowName: ei._examImporterInstance);
                wpfobject.ClickButton("BtnOpenFolder");
                wpfobject.InteractWithTree(Config.EI_TestDataPath + FilePath[3]);
                wpfobject.ClickButton("1");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("yesButton");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("yesButton");
                wpfobject.WaitTillLoad();

                if (ei.createPatient().Visible)
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

                //Step 14: Enter the information
                wpfobject.GetTextbox("TxtLastName").SetValue(patientInfo[0]);
                wpfobject.WaitTillLoad();
                wpfobject.GetTextbox("TxtFirstName").SetValue(patientInfo[1]);
                wpfobject.WaitTillLoad();
                wpfobject.GetTextbox("TxtIdMrn").SetValue(patientInfo[2]);
                wpfobject.WaitTillLoad();
                wpfobject.GetTextbox("TxtDescription").SetValue(patientInfo[5]);
                wpfobject.WaitTillLoad();
                wpfobject.GetTextbox("TxtRefPhysician").SetValue(patientInfo[6]);
                wpfobject.WaitTillLoad();
                wpfobject.GetTextbox("TxtInstitutionName").SetValue(patientInfo[7]);
                wpfobject.WaitTillLoad();
                wpfobject.GetComboBox("CmbGender").Item(0).Select();
                wpfobject.WaitTillLoad();
                ei.dob().Enter(patientInfo[3]);
                //wpfobject.Get<Window,DateFormat>(wpfobject.GetMainWindowByTitle(ei._examImporterInstance), "DateDob").DisplayValue()
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("BtnSave");
                wpfobject.WaitTillLoad();

                if (ei.StudyGridMain().Visible)
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

                //Step 15: On the right hand side select the paperclip under the Attach PDF label and select a PDF doc
                ei.AttachPDF(FilePath[2], EIWindowName: ei._examImporterInstance);
                PlusSignList = wpfobject.GetMultipleElements("ShowDetails");
                PlusSignList[0].Click();
                wpfobject.WaitTillLoad();
                Thread.Sleep(5000);
                if (ei.StudyInDetails().Visible)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 16: Go back and select the paperclip under the Attach Image
                PlusSignList = wpfobject.GetMultipleElements("ShowDetails");
                PlusSignList[0].Click();
                ei.AttachImage(FilePath[1], EIWindowName: ei._examImporterInstance);
                PlusSignList = wpfobject.GetMultipleElements("ShowDetails");
                PlusSignList[0].Click();
                wpfobject.WaitTillLoad();
                //DownloadImageFile(ei.UserNameTextbox_EI(), "");
                Thread.Sleep(5000);

                if (ei.StudyInDetails().Visible)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 17: Select the patient just created and click on SEND
                ei.SelectAllPatientsToUpload(EIWindowName: ei._examImporterInstance);
                ei.Send(EIWindowName: ei._examImporterInstance);
                ei.CloseUploaderTool(EIWindowName: ei._examImporterInstance);
                ExecutedSteps++;

                ////Step 18.
                int popstatus = 0;
             //   BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + FilePath[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                BasePage.Driver.Quit();
                login.CreateNewSesion();
                login.DriverGoTo(login.mpacstudyurl);
                mplogin = new MpacLogin();
                MPHomePage homepage = mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", accession[1], 0);
                tools.MpacSelectStudy("Accession", accession[1]);
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Step 19
                login.DriverGoTo(login.url);
                string[] HTML5WindowHandle = OpenHTML5UploaderandSwitchtoIT("login");
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

                //step 20
                html5.UploadFolderBtn().Click();
                basepage.UploadFileInBrowser(FilePath[0]);
                bool step20 = html5.UploadJobContainer().Displayed;
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

                //Step 21
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


                //Step 22
                int index;
                if (html5.PatientNameonSharePage().Text == FirstName[0])
                {
                    index = 0;
                }
                else
                {
                    index = 1;
                }
                bool step22_1 = (html5.PatientNameonSharePage(index).Text.Contains( FirstName[0]));
                bool step22_2 = (html5.StudyDetailsonSharePage(index)[0].Text == "1");
                bool step22_3 = (html5.StudyDetailsonSharePage(index)[1].Text == "1");
                bool step22_4 = (html5.StudyDetailsonSharePage(index)[2].Text == "0");
                if (step22_1 && step22_2 && step22_3 && step22_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step20_1 is " + step22_1);
                    Logger.Instance.InfoLog("step20_2 is " + step22_2);
                    Logger.Instance.InfoLog("step20_3 is " + step22_3);
                    Logger.Instance.InfoLog("step20_4 is " + step22_4);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step 23: Select destination and Priority from the respective dropdown list.
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                html5.PriorityDropdown().SelectByText(Priority);
                bool Step21_1 = html5.DestinationDropdown().SelectedOption.Text == Config.Dest1;
                bool Step21_2 = html5.PriorityDropdown().SelectedOption.Text == Priority;
                if (Step21_1 && Step21_2)
                

                html5.ShareBtn().Click();
                if (PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), BasePage.WaitTypes.Visible, 60).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step 24: The Physician (the Receiver) login and navigates to the Inbound Tab, enters either a patient by last name or PID and clicks on search
                Driver.Quit();
                Driver = null;
                login = new Login();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SearchStudy(patientID: patientID[0], AccessionNo: accession[0]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SelectStudy("Accession", accession[0]);

                Dictionary<string, string> patientResult = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID" }, new string[] { accession[0], patientID[0] });
                if (patientResult != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 25: Select one study and Granted to a registered user
                studies.GrantAccessToUsers(SuperAdminGroup, Config.ph2UserName);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                ExecutedSteps++;

                //Step 26:Go to the Outbound and do a search
                outbounds = (Outbounds)login.Navigate("Outbounds");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                outbounds.SearchStudy(patientID: patientID[0], AccessionNo: accession[0]);
                PageLoadWait.WaitForFrameLoad(10);
                outbounds.SelectStudy("Accession", accession[0]);
                string sharedUsername = null;

                Dictionary<string, string> sharedUser = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { accession[0], patientID[0], "Shared" });

                outbounds.GetMatchingRow("Accession", accession[0]).TryGetValue("To Users", out sharedUsername);

                if (sharedUsername.Equals(Config.ph2UserName + " " + Config.ph2UserName) && sharedUser != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 27: Return to the Inbound and click on Search and select a different study and Transfer
                login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SearchStudy(patientID: patientID[1], AccessionNo: accession[1]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SelectStudy("Accession", accession[1]);

                inbounds.TransferStudy("Local System", Accession: accession[1]);
                ExecutedSteps++;

                //Step 28:Click on the study and Download the study to local folder ,save and exit
                bool step19 = false;
                PageLoadWait.WaitForDownload("_" + patientName[1], Config.downloadpath, "zip");
                if (BasePage.CheckFile("_" + patientName[1], Config.downloadpath, "zip"))
                {
                    step19 = true;
                }

                if (step19)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 29: Select several studies and nominate them for archiving ,add Order notes in the Order Notes Field
                inbounds.SearchStudy(patientID: patientID[0], AccessionNo: accession[0]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SelectStudy("Accession", accession[0]);
                inbounds.NominateForArchive(order[0]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SearchStudy(patientID: patientID[1], AccessionNo: accession[1]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SelectStudy("Accession", accession[1]);
                inbounds.NominateForArchive(order[0]);
                PageLoadWait.WaitForFrameLoad(10);

                inbounds.SearchStudy(patientID: patientID[0], AccessionNo: accession[0]);
                PageLoadWait.WaitForFrameLoad(10);
                Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { accession[0], patientID[0], "Nominated For Archive" });

                inbounds.SearchStudy(patientID: patientID[1], AccessionNo: accession[1]);
                PageLoadWait.WaitForFrameLoad(10);
                Dictionary<string, string> row1 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { accession[1], patientID[1], "Nominated For Archive" });

                if (row != null && row1 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 30:Login as the Archiver ar/pwd.13579 navigate to the Inbound tab and click on Search Button
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.ClickSearchBtn();
                PageLoadWait.WaitForLoadingMessage(30);
                PageLoadWait.WaitForFrameLoad(10);

                Dictionary<string, string> sharedStudy = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID" }, new string[] { accession[0], patientID[0] });
                Dictionary<string, string> sharedStudy1 = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID" }, new string[] { accession[1], patientID[1] });

                //PRE-CONDITION
                inbounds.SelectStudy("Accession", accession[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.ArchiveStudy("Test", "Test_OrderNotes");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                bool step21 = false;
                Thread.Sleep(10000);
                inbounds.ClickSearchBtn();
                String[] status = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Status", BasePage.GetColumnNames());
                step21 = status.Any(s => s.Equals("Routing Completed"));

                if (step21 && sharedStudy != null && sharedStudy1 != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 31: Open one of the Studies in the viewer and apply some W / L.pan Zoom
                inbounds.SearchStudy(patientID: patientID[1], AccessionNo: accession[1]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SelectStudy("Accession", accession[1]);
                PageLoadWait.WaitForFrameLoad(10);
                BluRingViewer Blueringviewer = BluRingViewer.LaunchBluRingViewer();
                Blueringviewer.OpenViewerToolsPOPUp();
                Blueringviewer.SelectViewerTool(BluRingTools.Window_Level);
                Blueringviewer.ApplyTool_WindowWidth();
                Blueringviewer.OpenViewerToolsPOPUp();
                Blueringviewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                Blueringviewer.ApplyTool_Zoom();

                //Take Screenshot - 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], Blueringviewer.ViewPortContainer());

                if (Blueringviewer.StudyPatientDetailsInUniversalViewer()["PatientID"].Equals(patientID[1]) && step22)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 32: While in the viewer email the study to a user that has an email that can be viewed
                String pinnumber = Blueringviewer.EmailStudy_BR();
                if (pinnumber != null)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Blueringviewer.CloseBluRingViewer();


                //Step 33: Close the viewer and click on Archive study button for the study
                inbounds.SearchStudy(patientID: patientID[0], AccessionNo: accession[0]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SelectStudy("Accession", accession[0]);
                inbounds.ClickArchiveStudy("Test", "Reconcile");
                PageLoadWait.WaitForLoadInArchive(10);
                IWebElement ReconcileTablePanel = Driver.FindElement(By.CssSelector("table[id='ReconciliationMultiComponentPNTable']"));
                String OriginalDetails = Driver.FindElement(By.CssSelector("span[id*='m_ReconciliationControl_LabelStudyInput']")).Text;
                if (ReconcileTablePanel.Displayed && OriginalDetails == "Original Details")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 34:Select the appropriate order and add some Order Notes click on Archive.
                inbounds.ClickArchive();
                inbounds.SearchStudy("Accession", accession[0]);
                String Status;
                inbounds.GetMatchingRow(new string[] { "Accession" }, new string[] { accession[0] }).TryGetValue("Status", out Status);
                if (Status == "Routing Completed")
                {
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

                //Step 35 :Log in as the Receiver the study was emailed to and open the email received , enter the code from the sender
                var EmailMessage = Pop3EmailUtil.GetMail(Config.emailid, Config.Email_Password, "", "Emailed Study");
                var emaillink = Pop3EmailUtil.GetEmailedStudyLink(Config.emailid, Config.Email_Password, "", "Emailed Study");

                Blueringviewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);

                Boolean IsStudyLoaded = Blueringviewer.StudyPatientDetailsInUniversalViewer()["PatinetID"].Equals(patientID[1]);

                if (IsStudyLoaded)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Report Result                

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
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
                ServiceTool serviceTool = new ServiceTool();
                serviceTool.InvokeServiceTool();
                serviceTool.SetEmailNotificationForPOP();
                serviceTool.CloseServiceTool();

            }
        }

        ///<summary>
        /// Test 130892 - Workflow_Radiologist
        /// </summary>
        /// 
        public TestCaseResult Test_130892(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            ServiceTool tool = new ServiceTool();
            Studies studies = new Studies();
            BasePage basepage = new BasePage();
            StudyViewer viewer = new StudyViewer();
            DomainManagement domainManagement = new DomainManagement();
            RoleManagement roleManagement = new RoleManagement();
            UserManagement userManagement = new UserManagement();
            UserPreferences UserPref = new UserPreferences();
            int ExecutedSteps = -1;

            String adminUsername = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String phUsername = Config.LdapPHUser;
            String phPassword = Config.LdapUserPassword;
            string link = String.Empty;
            Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
            String[] patientID = null;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String HostName = basepage.GetHostName(Config.IConnectIP);
                //String[] datasourceList = new string[] { basepage.GetHostName(Config.DestEAsIp), basepage.GetHostName(Config.SanityPACS) };
                String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPerformedDate");
                String[] studyDate = StudyDateList.Split(':');
                String patientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                patientID = patientIDList.Split(':');
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Studypath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                String[] datasource = new String[] { basepage.GetHostName(Config.EA77), basepage.GetHostName(Config.EA91), basepage.GetHostName(Config.PACS2) };

                String hostName = tool.GetHostName(Config.IConnectIP);
                FileUtils.AddToHostsFile(Config.IConnectIP + " " + hostName.ToLower() + ".merge.com");

                EmailUtils CustomUser1 = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                DirectoryInfo di = new DirectoryInfo(Config.downloadpath);
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 2, 0);
                string EmailID = Config.CustomUser1Email;

                //setup LDAP server in service tool
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                tool.EnableLDAPConfigfile();
                tool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                //tool.NavigateToConfigToolSecurityTab();
                //tool.NavigateSubTab("General");
                //tool.ClickModifyFromTab();
                //tool.SetHTTPS(1);
                //wpfobject.WaitTillLoad();
                //tool.FQDN_txt().BulkText = HostName.ToLower() + ".merge.com";
                //tool.ClickApplyButtonFromTab();
                //wpfobject.WaitForPopUp();
                //wpfobject.GetMainWindowByIndex(1);
                //wpfobject.GetButton("6").Click();
                //wpfobject.GetMainWindowByIndex(0);
                //wpfobject.WaitTillLoad();
                tool.NavigateToConfigToolUserMgmtDatabaseTab();
                wpfobject.WaitTillLoad();
                tool.SetMode(2);
                wpfobject.WaitTillLoad();
                tool.LDAPSetup(LDApServerHost: "10.4.38.27");//10.4.38.239

                tool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.EnableEmailStudy();
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.wpfobject.ClickOkPopUp();
                tool.NavigateSubTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                wpfobject.WaitTillLoad();
                tool.ModifyEnableFeatures();
                ITabPage subtab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.EmailStudy);
                TextBox PINLength = wpfobject.GetUIItem<ITabPage, TextBox>(subtab, ServiceTool.Spinner_ID, itemsequnce: "1");
                PINLength.SetValue("6");
                tool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                tool.RestartService();
                wpfobject.WaitTillLoad();
                //tool.CloseServiceTool();
                //taskbar.Show();

				tool.InvokeServiceTool();
				tool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SystemEmail: Config.SystemEmail, SMTPHost: Config.SMTPServer);
				tool.CloseServiceTool();
				taskbar.Show();

				TestFixtures.UpdateFeatureFixture("bluring", value: "true:Legacy", restart:true);

                //send study to PACS
                //BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + Studypath[0] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                //var client = new DicomClient();
                //client.AddRequest(new DicomCStoreRequest(Config.TestDataPath + Studypath[0]));
                //client.Send(Config.EA77, 12000, false, "SCU", Config.EA77AETitle);

                //Step 1: Use the https//ICA serversname/webaccess and login as user ph/pwd.13579
                //var Testurl = "https://" + Config.IConnectIP + "/webaccess";
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                PageLoadWait.WaitForFrameLoad(10);
                bool step1 = basepage.GetCurrentSelectedtab().Equals("Studies");

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

                //Step 2: Change the Study Performed to appropriate date to view studies required and add the CT in the Modality field. Add the PID"999"and click on search
                PageLoadWait.WaitForFrameLoad(10);
                studies.ClearFields();
                studies.SelectCustomeStudySearch(studies.StudyPerformed());
                studies.FromDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.fromdatecalender()));
                studies.EnterDate_CustomSearch(studyDate[0]);
                studies.ToDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.todatecalender()));
                studies.EnterDate_CustomSearch(studyDate[1], fieldtype: "to");
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
                {
                    studies.ClickElement(studies.SubmitButton());
                }
                else
                {
                    studies.SubmitButton().Click();
                }
                PageLoadWait.WaitForFrameLoad(10);
                studies.PatientID().Clear();
                studies.PatientID().SendKeys(patientID[0]);
                PageLoadWait.WaitForFrameLoad(10);
                studies.Modality().Clear();
                studies.Modality().SendKeys(Modality);
                PageLoadWait.WaitForFrameLoad(10);
                studies.JSSelectDataSource(datasource[0]);
                PageLoadWait.WaitForFrameLoad(10);

                studies.ClickSearchBtn();
                PageLoadWait.WaitForLoadingMessage(30);
                PageLoadWait.WaitForFrameLoad(10);

                Dictionary<String, String> row = studies.GetMatchingRow(new string[] { "Patient ID", "Modality" }, new string[] { patientID[0], Modality });
                if (row != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3: Open the PID 999 in the viewer
                studies.SelectStudy1("Patient ID", patientID[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                if (viewer.SeriesViewPorts().Count == 4 &&
                    viewer.PatientDetailsInViewer()["PatientID"].Equals(patientID[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4: Double Click in the first viewer port
                var viewPort = viewer.SeriesViewPorts()[0];
                PageLoadWait.WaitForFrameLoad(10);

                if (BasePage.SBrowserName.Equals("firefox"))
                {
                    new Actions(BasePage.Driver).DoubleClick(viewPort).Perform();
                }
                else
                {
                    viewer.DoubleClick(viewPort);
                }
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForFrameLoad(10);

                if (viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "layoutFormat").Equals("1x1")
                    && viewer.PatientDetailsInViewer()["PatientID"].Equals(patientID[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5: Apply W/L in the image and then apply zoom
                PageLoadWait.WaitForFrameLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(30);
                //viewer.DragMovement(viewer.SeriesViewer_1X1());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.InteractiveZoom);
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Take Screenshot - 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool viewerStatus = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (viewerStatus)
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

                //Step 6: Double click in the window
                if (BasePage.SBrowserName.Equals("firefox"))
                {
                    new Actions(BasePage.Driver).DoubleClick(viewer.SeriesViewer_1X1()).Perform();
                }
                else
                {
                    viewer.DoubleClick(viewer.SeriesViewer_1X1());
                }
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Take Screenshot - 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (step6 && viewer.SeriesViewPorts().Count == 4)
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

                //Step 7: Click on the Reset icon
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

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

                // Step 8: Select the User Preferences and change the viewing scope for the CT modality to series scope
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.UserPreference);
                UserPref.SwitchToToolBarUserPrefFrame();
                UserPref.ModalityDropDown().SelectByText("CR");
                if (UserPref.ViewingScopeSeriesRadioBtn().Selected == false)
                    UserPref.ViewingScopeSeriesRadioBtn().Click();
                bool step8 = UserPref.ViewingScopeSeriesRadioBtn().Selected;
                UserPref.SaveToolBarUserPreferences();

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

                //Step 9: Close the Viewer
                viewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                Dictionary<String, String> row1 = studies.GetMatchingRow(new string[] { "Patient ID", "Modality" }, new string[] { patientID[0], Modality });

                if (row1 != null && IsElementVisible(studies.By_StudyTable()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10: Reload the same study PID=999 and Select the Second viewing port and double click on it
                studies.SelectStudy1("Patient ID", patientID[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                bool step10 = viewer.SeriesViewPorts().Count == 4 && viewer.PatientDetailsInViewer()["PatientID"].Equals(patientID[0]);
                var thumbnail = viewer.ThumbnailCaptions()[0];

                if (BasePage.SBrowserName.Equals("firefox"))
                {
                    new Actions(BasePage.Driver).DoubleClick(viewer.SeriesViewPorts()[1]).Perform();
                }
                else
                {
                    viewer.DoubleClick(viewer.SeriesViewPorts()[1]);
                }
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForFrameLoad(10);

                bool step10_1 = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "layoutFormat").Equals("1x1") && viewer.PatientDetailsInViewer()["PatientID"].Equals(patientID[0]);

                if (step10 && step10_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11:Select the Full Screen Mode
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FullScreen);
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(120);
                PageLoadWait.WaitForAllViewportsToLoad(250);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(viewer.By_Thumbanailcontainer()));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                // viewer.DropMenu().Displayed == false &&
                if (step_11 &&
                    viewer.ReviewtoolBar().Displayed == false &&
                    viewer.ThumbnailContainer(1).Displayed == false &&
                     viewer.patientAndStudyInfoElement(1).Displayed == true)
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

                //Step 12: Click on the Menus tab
                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Toolbar, thumbnail bar expend to show on the top of the study panels. 
                //And align with the study panel. Show Menus tab change to"Hide Menus"

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step12 &&
                    viewer.ReviewtoolBar().Displayed == true &&
                    viewer.ThumbnailContainer(1).Displayed == true
                   )
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step 13: Select the Zoom icon and apply it to the full screen image
                viewer.SelectToolInToolBar(IEnum.ViewerTools.InteractiveZoom);
                viewer.DragMovement(viewer.SeriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step13 &&
                    viewer.ReviewtoolBar().Displayed == false &&
                    viewer.ThumbnailContainer(1).Displayed == false &&
                    viewer.DropMenu().Displayed == false)
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

                //Step 14: Select the W / L and apply it , also apply several other, rotate, pan, draw a circle add some text.
                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(viewer.SeriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewer.DragMovement(viewer.SeriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                viewer.DrawElipse(viewer.SeriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AddText);
                viewer.AddTextAnnotation(viewer.SeriesViewer_1X2(), 200, 200, "rib", 1, 1, 2);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step14 &&
                    viewer.ReviewtoolBar().Displayed == false &&
                    viewer.ThumbnailContainer(1).Displayed == false &&
                     viewer.DropMenu().Displayed == false)
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

                //Step 15: Save the annotated Image
                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SaveSeries);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForLoadingIconToAppear_Savestudy(120);
                PageLoadWait.WaitForLoadingIconToDisAppear_Savestudy(120);
                int ThumbnailCountBeforeSave_FirstStudy = viewer.Thumbnails().Count;
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.MenusBtn()));
                result.steps[++ExecutedSteps].status = "Pass";
              
                //Step 16: Select the Menus tab and observer the PR image
                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(360);
                PageLoadWait.WaitForThumbnailsToLoad(360);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailCaptions()[0]));
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                //bool step16 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());


                if (viewer.ThumbnailCaptions()[0].Text.Contains("PR Images") && viewer.ReviewtoolBar().Displayed == true &&
                    viewer.ThumbnailContainer(1).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17: while still in the Full screen mode scroll to the next image in the series and continue to scroll through all the images and back to the top to image 1
                //viewer.SeriesViewPorts()[0].Click();
                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(360);
                bool isEqual = false;

                for (var i = 1; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                isEqual = viewer.SeriesViewPorts()[0].GetAttribute("imagenum").Equals("3");
                Logger.Instance.InfoLog("The Imagenum in the viewport" + viewer.SeriesViewPorts()[0].GetAttribute("imagenum")+" with "+ isEqual);
                if (viewer.SeriesViewPorts()[0].GetAttribute("imagenum").Equals("3"))
                {
                    for (var i = 0; i < 2; i++)
                    {
                        viewer.ClickUpArrowbutton(1, 2);
                        PageLoadWait.WaitForFrameLoad(20);
                    }
                }
                isEqual = viewer.SeriesViewPorts()[0].GetAttribute("imagenum").Equals("1");
                Logger.Instance.InfoLog("The Imagenum in the viewport" + viewer.SeriesViewPorts()[0].GetAttribute("imagenum") + " with " + isEqual);
                if (isEqual)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18: Close the viewer
                viewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (IsElementVisible(studies.By_StudyTable()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19: Go to User preferences and change the Default Settings Per Modality change the Modality CT and change the Viewing Scope to Image click OK
                UserPref.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                UserPref.ModalityDropDown().SelectByText("CT");
                if (UserPref.ViewingScopeImageRadioBtn().Selected == false)
                    UserPref.ViewingScopeImageRadioBtn().Click();
                UserPref.CloseUserPreferences();
                PageLoadWait.WaitForFrameLoad(20);
                if (IsElementVisible(studies.By_StudyTable()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20: Load the same PID=999 study
                PageLoadWait.WaitForFrameLoad(10);
                studies.ClearFields();
                studies.SelectCustomeStudySearch(studies.StudyPerformed());
                studies.FromDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.fromdatecalender()));
                studies.EnterDate_CustomSearch(studyDate[0]);
                studies.ToDate().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.todatecalender()));
                studies.EnterDate_CustomSearch(studyDate[1], fieldtype: "to");
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
                {
                    studies.ClickElement(studies.SubmitButton());
                }
                else
                {
                    studies.SubmitButton().Click();
                }
                studies.PatientID().Clear();
                studies.PatientID().SendKeys(patientID[0]);
                studies.Modality().Clear();
                studies.Modality().SendKeys(Modality);
                studies.JSSelectDataSource(datasource[0]);

                studies.ClickSearchBtn();
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy1("Patient ID", patientID[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                bool step20_1 = false;

                if (viewer.ThumbnailCaptions()[0].Text.Contains("PR Images")) { step20_1 = true; }
                for (var i = 1; i < 3; i++) { if (!viewer.ThumbnailCaptions()[i].Text.Contains("PR Images")) { step20_1 = true; } if (!step20_1) break; }

                //Vlaidation
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                //bool step20 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step20_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21:Go to the viewing port with the Axial head series and double click on the image
                String caption = viewer.ThumbnailCaptions()[2].Text;
                if (BasePage.SBrowserName.Equals("firefox"))
                {
                    new Actions(BasePage.Driver).DoubleClick(viewer.SeriesViewer_2X1()).Perform();
                }
                else
                {
                    viewer.DoubleClick(viewer.SeriesViewer_2X1());
                }
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step21 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                Logger.Instance.InfoLog("Caption present in the thumbnail is: " + caption);
                Logger.Instance.InfoLog("Present layout format is: " + viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "layoutFormat"));
                if (step21 && caption.Contains("AXIALS !HEAD!") && !viewer.SeriesViewer_1X2().Displayed)//viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "layoutFormat").Equals("1x1"))
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

                //Step 22: Change the W/L and Pan on the Series 2 Image 1
                //viewer.SeriesViewer_1X1().Click();
                //PageLoadWait.WaitForPageLoad(5);
                //PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step22 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

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

                //Step 23: Scroll though all the images
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                viewer.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                result.steps[++ExecutedSteps].SetPath(testid + "-before", ExecutedSteps + 1);
                bool step23_1 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());


                for (var i = 1; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(2, 1);
                }

                result.steps[ExecutedSteps].SetPath(testid + "-after", ExecutedSteps + 1);
                bool step23_2 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step23_1 && step23_2)
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

                //Step 24: scroll to image 18 and apply the Text and rotate the image to the right               
                int loop = 0;
                for (loop = 0; loop <= 17; loop++)
                {
                    if (!viewer.SeriesViewer_2X1().GetAttribute("imagenum").Contains("18"))
                    {
                        //viewer.ClickDownArrowbutton(1, 1, 1);
                        viewer.Scroll(2, 1, 15, "down", "click");
                    }
                }
                bool step24 = false;

                if (viewer.SeriesViewer_2X1().GetAttribute("imagenum").Contains("18"))
                {
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.AddText);
                    viewer.AddTextAnnotation(viewer.SeriesViewer_2X1(), 200, 200, "rib", 1, 2, 1);
                    PageLoadWait.WaitForFrameLoad(60);
                    PageLoadWait.WaitForAllViewportsToLoad(40);

                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                    PageLoadWait.WaitForFrameLoad(60);
                    PageLoadWait.WaitForAllViewportsToLoad(40);

                    step24 = true;
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step24_1 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step24 && step24_1)
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

                //Step 25: Scroll through all the images and confirm the attributes are only on image 1 and image 18
                ExecutedSteps++;
                result.steps[ExecutedSteps].SetPath(testid + "-18th image", ExecutedSteps + 1);
                bool Image18 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                bool Image1 = false;

                for (var i = 1; i <= 17; i++)
                {
                    viewer.ClickUpArrowbutton(2, 1);

                    if (viewer.SeriesViewPorts()[0].GetAttribute("imagenum").Equals("1"))
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "-1st image", ExecutedSteps);
                        Image1 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                        break;
                    }
                }


                if (Image18 && Image1)
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

                //Step 26: Load the study with PID=645878 Head,081, three series CT / MR head
                viewer.CloseStudy();
                //send study to PACS
                //BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + Studypath[1] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                //client = new DicomClient();
                //client.AddRequest(new DicomCStoreRequest(Config.TestDataPath + Studypath[1]));
                //client.Send(Config.EA77, 12000, false, "SCU", Config.EA77AETitle);

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID[1], Datasource: datasource[2]);
                studies.SelectStudy1("Patient ID", patientID[1]);
                studies.LaunchStudy();

                //var patientDetails = viewer.patientAndStudyInfoElement(1).Text.Trim();
                //patientDetails = patientDetails.Replace(")", ";").Trim().Split(';')[0].Split('(')[1];


                if (viewer.ViewStudy() &&
                    viewer.SeriesViewPorts().Count == 4 &&
                     viewer.SeriesViewer_2X2().GetAttribute("src").Contains("blankImage"))// &&
                                                                                          //patientDetails.Equals(patientID[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 27: From the tool bar select the Global Stack found under the Cine icon
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step27 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1withScroll());

                if (IsElementVisible(viewer.By_GlobalStackStatusImg(1, 1)) && step27)
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

                //Step 28: Set the viewing Scope in the User Preferences for the CT modality To Series Scope
                //viewer.MenusBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                viewer.SelectToolInToolBar(IEnum.ViewerTools.UserPreference);
                UserPref.SwitchToToolBarUserPrefFrame();
                UserPref.ModalityDropDown().SelectByText("MR");
                if (UserPref.ViewingScopeSeriesRadioBtn().Selected == false)
                { UserPref.ViewingScopeSeriesRadioBtn().Click(); }
                bool step28 = UserPref.ViewingScopeSeriesRadioBtn().Selected;
                UserPref.SaveToolBarUserPreferences();


                //Select the first port, upper left and select the Full screen mode icon then double click on the image in the first port, Scroll through all the series , all series and images should be displayed
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FullScreen);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                new Actions(BasePage.Driver).DoubleClick(viewer.SeriesViewer_1X1()).Perform();

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForFrameLoad(10);

                var source = viewer.ViewportScrollHandle(1, 1);
                var destination = viewer.ViewportScrollBar(1, 1);

                var w = destination.Size.Width;
                var h = destination.Size.Height;

                var action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                bool step28_1 = false;
                Logger.Instance.InfoLog(viewer.SeriesViewPorts()[0].GetAttribute("imagenum"));

                if (viewer.SeriesViewPorts()[0].GetAttribute("imagenum").Equals("202")) { step28_1 = true; }
                //for (var i = 1; i < 219; i++)
                //{
                //    viewer.ClickDownArrowbutton(1, 1);
                //    if (viewer.SeriesViewPorts()[0].GetAttribute("imagenum").Equals(Convert.ToString(i + 1))) { step28_1 = true; }
                //    if (step28_1) { continue; } else { break; }                      
                //}

                if (step28 && step28_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 29: While in full screen mode Apply W/L in the 4th image in Series one and then apply a small zoom in the same image
                var source1 = viewer.ViewportScrollHandle(1, 1);
                var destination1 = viewer.ViewportScrollBar(1, 1);

                var w1 = destination1.Size.Width;
                var h1 = destination1.Size.Height;

                var action1 = new Actions(BasePage.Driver);

                action1.ClickAndHold(source1).MoveToElement(destination1, w1 / 2, 1).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                bool step29 = false;

                for (var i = 1; i < 4; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    if (viewer.SeriesViewPorts()[0].GetAttribute("imagenum").Equals("4")) { step29 = true; break; }

                }

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(viewer.SeriesViewPorts()[0]);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.InteractiveZoom);
                viewer.DragMovement(viewer.SeriesViewPorts()[0]);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_29 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step29 && step_29)
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

                //Step 30: Remove Global Stack mode and Reset all of the changes
                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.GlobalStack);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FullScreen);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                if (viewer.SeriesViewPorts().Count == 4 && viewer.SeriesViewer_2X2().GetAttribute("src").Contains("blankImage"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 31: Select Series Viewer 1x2 display mode and drag and drop Series 1 into the left port, Series 2 into the right port
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                viewer.DragMovement(viewer.SeriesViewPorts()[0]);
                PageLoadWait.WaitForFrameLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                viewer.DragThumbnailToViewport(2, Locators.ID.SeriesViewer2_2x3);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step31 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

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

                //Step 32: Select Series 2 and Apply the Localizer lines
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step32 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (step32)
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

                //Step 33: Scroll down on series two and observe series one
                bool step33 = false;
                for (var i = 1; i < 4; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                    if (i == 1) { ExecutedSteps++; }
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, i);
                    step33 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                    if (step33) { continue; } else { break; }
                }

                if (step33)
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

                //Step 34: close the viewer and load any CT/MR study
                viewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID[1], Datasource: datasource[2]);
                studies.SelectStudy1("Patient ID", patientID[1]);
                studies.LaunchStudy();
                CustomUser1.MarkAllMailAsRead();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

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

                //Step-35:Click on email Study icon from review toolbar and enter the valid guest mail address, name and reason then click on "Send Email" button 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy);
                wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_EmailStudyDiv()));
                viewer.EmailStudy(EmailID, "Test", "Test", 1);
                String pinnumber = viewer.FetchPin();
                if (pinnumber.Length == 6)
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

                //Step-36:Go to the destination Email and Check that the "Emailed Study" notification is received.
                //result.steps[++ExecutedSteps].status = "Not Automated";
                downloadedMail = CustomUser1.GetMailUsingIMAP(Config.SystemEmail, "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                link = CustomUser1.GetEmailedStudyLink(downloadedMail);
                if (link != null)
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

                //Step-37:Click on the link to open the study from inbox and 
                //Enter the generated pin number
                //result.steps[++ExecutedSteps].status = "Not Automated";
                viewer = LaunchEmailedStudy.LaunchStudy<StudyViewer>(link, pinnumber);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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

                //Report Result 
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
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
                try
                {
                    if (patientID[0] != null)
                    {
                        HPLogin hplogin = new HPLogin();
                        BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA77 + "/webadmin");
                        HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.EA77 + "/webadmin");
                        WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                        workflow.NavigateToLink("Workflow", "Archive Search");
                        workflow.HPSearchStudy("PatientID", patientID[0]);
                        if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                            workflow.DeletePaticularModality("PR");
                        hplogin.LogoutHPen();
                        login.DriverGoTo(login.url);

                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("PR delete exception -- " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }
        }
    }
}



