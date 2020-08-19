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
using Dicom.Network;
using Dicom;

namespace Selenium.Scripts.Tests
{
    class PresentationStateHandling : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        public Studies studies { get; set; }
        public BasePage basepage { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        string FolderPath = "";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public PresentationStateHandling(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            basepage = new BasePage();
            servicetool = new ServiceTool();
            bluringviewer = new BluRingViewer();
        }

        /// <summary>
        /// Presentation State Handling - Saving Series in "Presentation State as Logical Series" with "Series" viewing scope
        /// </summary>
        public TestCaseResult Test_163136(String testid, String teststeps, int stepcount)
        {
            //Old test case ID: 140575
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Maintenance maintenance;
            Taskbar taskbar = null;
            DomainManagement domain;
            RoleManagement role;
            UserManagement usermanagement;
            UserPreferences userpreferences;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            string[] PatientID = null;
            String[] FirstName = null;
            int DS1Port = 0;
            string ExtractPath = string.Empty;
            String IEDownloadPath = @"C:\Users\Administrator\Downloads";
            String[] Filename = null;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String StudyDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                FirstName = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName")).Split(':');
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                Filename = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "FileName")).Split(':');
                String[] PRFolderName = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PRFolderName")).Split(':');
                String[] SOPClassUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPClassUID")).Split(':');
                PatientID = PatientIDList.Split(':');
                String[] SOPInstanceUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPInstanceUID")).Split(':');
                String[] description = StudyDescription.Split(':');
                String URL = "http://" + Config.IConnectIP + "/webaccess";
                String PrivUser1 = "rad" + new Random().Next(1, 1000);
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //Preconditions
                //Enable GSPS in Service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
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
                servicetool.CloseConfigTool();
                taskbar.Show();

                //Presentation States as Logical Series is configured in Domain/Role/User Preferences
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                //Add PR tool to toolbox 
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                domain.SetCheckBoxInEditDomain("datatransfer", 0);
                domain.SetCheckBoxInEditDomain("datadownload", 0);

                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Save Annotated Images", group1);
                dictionary.Add("Save Series", group1);
                domain.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                domain.ClickSaveEditDomain();

                //Enable Transfer and data download in Role
                role = login.Navigate<RoleManagement>();
                role.SearchRole(Config.adminRoleName, Config.adminGroupName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();

                //select allow download flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                role.SetCheckboxInEditRole("transfer", 0);
                role.SetCheckboxInEditRole("download", 0);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(5);

                //Create User
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.CreateUser(PrivUser1, Config.adminGroupName, Config.adminRoleName);
                login.Logout();

                //Clear download directory
                try
                {
                    DeleteAllFileFolder(Config.downloadpath);
                    DeleteAllFileFolder(IEDownloadPath);
                }
                catch (Exception ex) { }

                //Precondition - Send studies to EA
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                FullPath = Directory.GetFiles(FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                //Step-1: Login to Enterprise Viewer web application as privilege user (e.g., rad/rad)rad330/tech330
                login.LoginIConnect(PrivUser1, PrivUser1);
                studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step-2: From the "Studies" tab, search for a study with multiple images in its series (ensure that the Viewing Scope for this modality is set to "Series" in the configuration). Load the study into the Enterprise Viewer. If using the recommended data set, search for "MICKEY, MOUSE" patient, and load MR study with date "04-Feb-1995".
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropdown().SelectByText("MR");
                if (userpreferences.SelectedValueOfRadioBtn("ScopeRadioButtons").Equals("Series", StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    userpreferences.SelectRadioBtn("ScopeRadioButtons", "Series");
                }
                if (userpreferences.SelectedValueOfRadioBtn("DefaultViewerSetting").Equals("BluRing", StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    userpreferences.SelectRadioBtn("DefaultViewerSetting", "BluRing");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpreferences.SavePreferenceBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(userpreferences.CloseBtn()));
                userpreferences.CloseBtn().Click();
                //Search study
                studies.SearchStudy(LastName: LastName, FirstName: FirstName[0], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                int ThumbnailCount = ThumbnailList.Count;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                if (ThumbnailCount == 5 && step2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: Without applying any tools, click Save Series from the viewport Toolbox.
                bool step3_1 = bluringviewer.SavePresentationState(BluRingTools.Save_Series);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step3_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step3_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                string step3_3 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
                string step3_4 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text;

                if (ThumbnailList.Count == ThumbnailCount + 1 && step3_1 && step3_2 && step3_5 && step3_3 == "PR" && step3_4 == "20")
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    //throw new Exception("PR not saved hence aborting test");
                }

                Logger.Instance.InfoLog("Save PR Method: " + step3_1);
                Logger.Instance.InfoLog("Image Comparison: " + step3_2);
                Logger.Instance.InfoLog("Modality: " + step3_3);
                Logger.Instance.InfoLog("Frame Number: " + step3_4);

                //Step-4: Open the same study (from previous step) again in the Enterprise Viewer.
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(LastName: LastName, FirstName: FirstName[0], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step4_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step4_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (ThumbnailList.Count == ThumbnailCount + 1 && step4_1 && step4_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: From the "Studies" tab, search for a study with multiple images in its series (ensure that the Viewing Scope for this modality is set to "Series" in the configuration). Load the study into the Enterprise Viewer.
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(LastName: LastName, FirstName: FirstName[1], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                ThumbnailCount = ThumbnailList.Count;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                if (ThumbnailCount == 5 && step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: From the first series in the first viewport, apply the following tools to the first image:W/L, Invert, Pan, Zoom, Flip
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectInnerViewerTool(BluRingTools.Invert, BluRingTools.Window_Level);
                Thread.Sleep(5000);
                bluringviewer.SelectViewerTool(BluRingTools.Pan);
                bluringviewer.ApplyTool_Pan();
                bluringviewer.SelectInnerViewerTool(BluRingTools.Magnifier, BluRingTools.Interactive_Zoom);
                bluringviewer.ApplyTool_Magnifier(false);
                bluringviewer.SelectViewerTool(BluRingTools.Rotate_Clockwise);
                Thread.Sleep(2000);
                //bluringviewer.SelectViewerTool(BluRingTools.Flip_Vertical);
                bluringviewer.ApplyTool_FlipVertical();

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                IWebElement viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step6_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check other images also affected
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2, 1);

                if (step6_1 && step6_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7: From the first series in the first viewport, apply the following tools to the 2nd image: Text, Ellipse, Line
                bluringviewer.SelectViewerTool(BluRingTools.Add_Text);
                bluringviewer.ApplyTool_AddText("163136 - Test comment");
                bluringviewer.SelectViewerTool(BluRingTools.Draw_Ellipse);
                bluringviewer.ApplyTool_DrawEllipse(viewport.Size.Width / 5, viewport.Size.Height / 2, viewport.Size.Width / 3, viewport.Size.Height / 3);
                bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement);
                bluringviewer.ApplyTool_LineMeasurement(viewport.Size.Width / 5, viewport.Size.Height / 5, viewport.Size.Width / 3, viewport.Size.Height / 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step7_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check 1st images not affected
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "up").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step7_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2, 1);

                if (step7_1 && step7_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Click Save Series from the viewport Toolbox, to save the applied changes.
                //Save current Date/Time
                DateTime CurrentTime = DateTime.Now;
                bool step8_1 = bluringviewer.SavePresentationState(BluRingTools.Save_Series);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step8_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step8_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                string step8_3 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
                string step8_4 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text;

                if (ThumbnailList.Count == ThumbnailCount + 1 && step8_1 && step8_2 && step8_5 && step8_3 == "PR" && step8_4 == "20")
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                Logger.Instance.InfoLog("Save PR Method: " + step8_1);
                Logger.Instance.InfoLog("Image Comparison: " + step8_2);
                Logger.Instance.InfoLog("Modality: " + step8_3);
                Logger.Instance.InfoLog("Frame Number: " + step8_4);

                //Step-9: Close the study.
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-10: From the "Studies" tab, search for the same series as above, with the new PR series. Load the study into the Enterprise Viewer.
                studies.SearchStudy(LastName: LastName, FirstName: FirstName[1], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                ThumbnailCount = ThumbnailList.Count;
                string step10_1 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).Text;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step10_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step10_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (ThumbnailCount == 6 && step10_1.Contains("PR") && step10_2 && step10_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Select the PR series viewport and review the images in the series.
                //Checking PR series  - Viewport 1
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 1)).Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step11_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check other image
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step11_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2, 1);

                if (step11_1 && step11_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Select the original series (of the PR series) and review the images in the series.
                //Checking original series  - Viewport 2
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(1, 1)).Click();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step12_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check other images not affected
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step12_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2, 1);

                if (step12_1 && step12_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                bluringviewer.CloseBluRingViewer();

                //Step-13: Go to the data source with the saved PR study, verify in a third party viewer or DICOM file viewer that the new PR modality series contain the following tags with proper values:
                //Download file using transfer service
                studies.SearchStudy(LastName: LastName, FirstName: FirstName[1], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[1]);
                studies.TransferStudy("Local System", SelectallPriors: false, waittime: 280);
                PageLoadWait.WaitForDownload(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);
                Boolean studydownloaded = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], IEDownloadPath, Filename[1].Split('.')[1]);
                else
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);

                //Unzip downloaded file
                string ZipPath = String.Empty;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    ZipPath = IEDownloadPath + Path.DirectorySeparatorChar + Filename[1];
                else
                    ZipPath = Config.downloadpath + Path.DirectorySeparatorChar + Filename[1];
                ExtractPath = Config.downloadpath;
                string UnzipFolderName = UnZipFolder(ZipPath, ExtractPath);
                Thread.Sleep(5000);
                ExtractPath = Config.downloadpath + Path.DirectorySeparatorChar + UnzipFolderName;

                var file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + PRFolderName[0] + Path.DirectorySeparatorChar + Filename[0]);

                var DicomData = file.Dataset;
                string ContentLabel = DicomData.Get<string>(DicomTag.ContentLabel);
                string CreationDate = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                string CreationTime = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                string SOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                string CreatorName = DicomData.Get<string>(DicomTag.ContentCreatorName);
                string SOPInstanceUID = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                string ReferencedSOPClassUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPClassUID);
                string ReferencedSOPInstanceUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPInstanceUID);
                string Modality = DicomData.Get<string>(DicomTag.Modality);

                Logger.Instance.InfoLog("Step 13 Data: " + ContentLabel + " _ " + CreationDate + " _ " + CreationTime + " _ " + SOPClassUID + " _ " + CreatorName + " _ " + SOPInstanceUID + " _ " + ReferencedSOPClassUID + " _ " + ReferencedSOPInstanceUID + " _ " + Modality);
                var CapturedTime = DateTime.ParseExact(CreationTime, "HHmmss", CultureInfo.InvariantCulture);
                var CapturedTimeMinus = CapturedTime.AddMinutes(-5);
                var CapturedTimePlus = CapturedTime.AddMinutes(5);
                //ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1])
                if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(PrivUser1) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus))
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


                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {

                try
                {
                    //Deleting uploaded study
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID[0]);
                    workflow.HPDeleteStudy();
                    workflow.HPSearchStudy("PatientID", PatientID[1]);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
                try
                {
                    //file.Save(Config.downloadpath + Path.DirectorySeparatorChar + "test1");
                    File.Delete(Config.downloadpath + Path.DirectorySeparatorChar + Filename[1]);
                    DeleteAllFileFolder(ExtractPath);
                }
                catch (Exception ex) { }
            }
        }

        /// <summary>
        /// Presentation State Handling - Saving Annotated Images in "Presentation State as Logical Series" with "Image" viewing scope mode
        /// </summary>
        public TestCaseResult Test_163137(String testid, String teststeps, int stepcount)
        {
            //Old test case ID: 142094
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Taskbar taskbar = null;
            Maintenance maintenance;
            DomainManagement domain;
            RoleManagement role;
            UserManagement usermanagement;
            UserPreferences userpreferences;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            string PatientID = string.Empty;
            int DS1Port = 0;
            string ExtractPath = string.Empty;
            String IEDownloadPath = @"C:\Users\Administrator\Downloads";
            String[] Filename = null;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String StudyDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] description = StudyDescription.Split(':');
                Filename = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "FileName")).Split(':');
                String[] PRFolderName = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PRFolderName")).Split(':');
                String[] SOPClassUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPClassUID")).Split(':');
                String[] SOPInstanceUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPInstanceUID")).Split(':');
                String URL = "http://" + Config.IConnectIP + "/webaccess";
                String PrivUser1 = "rad" + new Random().Next(1, 1000);
                String PrivUser2 = "tech" + new Random().Next(1, 1000);
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //Preconditions
                //Enable GSPS in Service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
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
                servicetool.CloseConfigTool();
                taskbar.Show();

                //Presentation States as Logical Series is configured in Domain/Role/User Preferences
                login.LoginIConnect(username, password);
                //Add PR tool to toolbox 
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                domain.SetCheckBoxInEditDomain("datatransfer", 0);
                domain.SetCheckBoxInEditDomain("datadownload", 0);

                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Save Annotated Images", group1);
                dictionary.Add("Save Series", group1);
                domain.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                domain.ClickSaveEditDomain();

                //Enable Transfer and data download in Role
                role = login.Navigate<RoleManagement>();
                role.SearchRole(Config.adminRoleName, Config.adminGroupName);
                role.SelectRole(Config.adminGroupName);
                role.ClickEditRole();

                //select allow download flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                role.SetCheckboxInEditRole("transfer", 0);
                role.SetCheckboxInEditRole("download", 0);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(5);

                //Create User
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.CreateUser(PrivUser1, Config.adminGroupName, Config.adminRoleName);
                usermanagement.CreateUser(PrivUser2, Config.adminGroupName, Config.adminRoleName);
                login.Logout();

                //Clear download directory
                try
                {
                    DeleteAllFileFolder(Config.downloadpath);
                    DeleteAllFileFolder(IEDownloadPath);
                }
                catch (Exception ex) { }

                //Precondition - Send studies to EA
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                //Step-1: Login to Enterprise Viewer web application as privilege user (e.g., rad/rad)rad849/tech849
                login.DriverGoTo(login.url);
                login.LoginIConnect(PrivUser1, PrivUser1);
                studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step-2: From the "Studies" tab, search for a study with multiple images in its series. Load the study into the Enterprise Viewer.Ensure that the viewing scope for the loaded study is set to "Image" scope. If using recommended data, search for "MICKEY, MOUSE" patient and load CT study with date "19-Aug-2004" to the Enterprise Viewer.Modify the Viewing Scope for CT to "Image" scope in the User Preferences.
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropdown().SelectByText("MR");
                if (userpreferences.SelectedValueOfRadioBtn("ScopeRadioButtons").Equals("Image", StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    userpreferences.SelectRadioBtn("ScopeRadioButtons", "Image");
                }
                if (userpreferences.SelectedValueOfRadioBtn("DefaultViewerSetting").Equals("BluRing", StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    userpreferences.SelectRadioBtn("DefaultViewerSetting", "BluRing");
                }
                PageLoadWait.WaitForPageLoad(20);
                userpreferences.SavePreferenceBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(userpreferences.CloseBtn()));
                userpreferences.CloseBtn().Click();
                //Search study
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                IList<IWebElement> ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                int ThumbnailCount = ThumbnailList.Count;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                if (ThumbnailCount == 5 && step2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3: From the first series in the first viewport, apply the following W/L to the first image: make the image considerably darker (fewer grays, more contrast)
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
                bluringviewer.ApplyTool_WindowWidth();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                IWebElement viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step3_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check other images not affected
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step3_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2, 1);

                if (step3_1 && step3_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: From the first series in the first viewport, apply the following to the 2rd image: Flip the image vertically.
                //bluringviewer.SelectViewerTool(BluRingTools.Flip_Vertical);
                bluringviewer.ApplyTool_FlipVertical();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step4_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check 1st images not affected - W/L
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "up").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2);

                //Scroll and check other images not affected
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down", "2").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step4_3 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 3, 1);

                if (step4_1 && step4_2 && step4_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "up").Perform();

                //Step-5: From the first series in the first viewport, apply the following to the 2nd image: add a text value on the image, add a measurement on the image (line, cobb angle, etc.)
                bluringviewer.SelectViewerTool(BluRingTools.Add_Text);
                bluringviewer.ApplyTool_AddText("163137 - Test comment");
                //bluringviewer.SelectViewerTool(BluRingTools.Cobb_Angle);
                bluringviewer.SelectInnerViewerTool(BluRingTools.Cobb_Angle, BluRingTools.Angle_Measurement);
                bluringviewer.ApplyTool_CobbAngle(viewport.Size.Width / 5, viewport.Size.Height / 3, viewport.Size.Width / 3, viewport.Size.Height / 5);
                bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement);
                bluringviewer.ApplyTool_LineMeasurement(viewport.Size.Width / 5, viewport.Size.Height / 5, viewport.Size.Width / 3, viewport.Size.Height / 3);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step5_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check 1st images not affected - W/L
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "up").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2);

                //Scroll and check other images not affected
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down", "2").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step5_3 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 3, 1);

                if (step5_1 && step5_2 && step5_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Navigate to the first image of the first viewport series, and click Save Annotated Image from the viewport Toolbox, to save the applied changes.
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "up", "2").Perform();
                DateTime CurrentTime = DateTime.Now;
                bool step6_1 = bluringviewer.SavePresentationState(BluRingTools.Save_Annotated_Image);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //bool step6_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ThumbnailandViewPortContainer());//StudyPanelThumbnailContainer && viewportContainer
                bool step6_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step6_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                string step6_3 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
                string step6_4 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text;

                if (ThumbnailList.Count == ThumbnailCount + 1 && step6_1 && step6_2 && step6_5 && step6_3 == "PR" && step6_4 == "1")
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    //throw new Exception("PR not saved hence aborting test");
                }

                Logger.Instance.InfoLog("Save PR Method: " + step6_1);
                Logger.Instance.InfoLog("Image Comparison: " + step6_2);
                Logger.Instance.InfoLog("Thumbail Caption: " + step6_3);
                Logger.Instance.InfoLog("Frame Number: " + step6_4);
                

                //Step-7: Select the new PR thumbnail from the Study Panel Thumbnail bar, and drag it into a new (empty) viewport. Review the new PR series in the viewport. Ensure that the new PR series only contains the images that were annotated, and that window modifications made previously on that image is also saved.
                ThumbnailCount = ThumbnailList.Count;
                // Drag the 1st thumbnail to the first viewport
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(3, 1)).Click();
                bluringviewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 4, studyPanelNumber: 1, UseDragDrop: true);
                IWebElement TargetElement = bluringviewer.GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step7_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], TargetElement);
                string step7_2 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text;

                if (step7_1 && step7_2 == "1")
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Ensure that the new PR series has a new (unique) series number assigned (seen in the thumbnail); however, the demographic information(image text) has the original referenced series number.
                string step8_1 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step8_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], TargetElement);
                if (step8_1 == "S6-1" && step8_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Select the viewport with the original series, which you have modified window settings and added annotation (this was the first viewport).
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 1)).Click();
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "up", "4").Perform();

                //check 1st image - W/L
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step9_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check 2nd images - Annotations
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step9_2 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2);

                //Scroll and check other images not affected
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step9_3 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 3, 1);

                if (step9_1 && step9_2 && step9_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Close the Enterprise viewer (click "X" EXIT in the global toolbar).
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-11: Go to the data source with the saved PR study, verify in a third party viewer or DICOM file viewer that the new PR modality series contain the following tags with proper values:
                //Download file using transfer service
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID);
                studies.TransferStudy("Local System", SelectallPriors: false, waittime: 180);
                PageLoadWait.WaitForDownload(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);
                Boolean studydownloaded = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], IEDownloadPath, Filename[1].Split('.')[1]);
                else
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);

                //Unzip downloaded file
                string ZipPath = String.Empty;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    ZipPath = IEDownloadPath + Path.DirectorySeparatorChar + Filename[1];
                else
                    ZipPath = Config.downloadpath + Path.DirectorySeparatorChar + Filename[1];
                ExtractPath = Config.downloadpath;
                string UnzipFolderName = UnZipFolder(ZipPath, ExtractPath);
                Thread.Sleep(5000); 
                ExtractPath = Config.downloadpath + Path.DirectorySeparatorChar + UnzipFolderName;

                var file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + PRFolderName[0] + Path.DirectorySeparatorChar + Filename[0]);

                var DicomData = file.Dataset;
                string ContentLabel = DicomData.Get<string>(DicomTag.ContentLabel);
                string CreationDate = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                string CreationTime = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                string SOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                string CreatorName = DicomData.Get<string>(DicomTag.ContentCreatorName);
                string SOPInstanceUID = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                string ReferencedSOPClassUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPClassUID);
                string ReferencedSOPInstanceUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPInstanceUID);
                string Modality = DicomData.Get<string>(DicomTag.Modality);

                Logger.Instance.InfoLog("Data 1: " + ContentLabel + " _ " + CreationDate + " _ " + CreationTime + " _ " + SOPClassUID + " _ " + CreatorName + " _ " + SOPInstanceUID + " _ " + ReferencedSOPClassUID + " _ " + ReferencedSOPInstanceUID + " _ " + Modality);

                var CapturedTime = DateTime.ParseExact(CreationTime, "HHmmss", CultureInfo.InvariantCulture);
                var CapturedTimeMinus = CapturedTime.AddMinutes(-5);
                var CapturedTimePlus = CapturedTime.AddMinutes(5);
                Logger.Instance.InfoLog("CurrentTime: " + CurrentTime.ToString("yyyyMMdd") + " _ " + "CurrentTime" + " _ " + CurrentTime + " _ " + "CapturedTimeMinus" + " _ " + CapturedTimeMinus + " _ " + "CapturedTimePlus" + " _ " + CapturedTimePlus);
                Logger.Instance.InfoLog("Result: " + ContentLabel.Equals("GSPS") + CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) + SOPClassUID.StartsWith(SOPClassUIDList[0]) + CreatorName.Equals(PrivUser1) + Modality.Equals("PR") + ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) + ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) + (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus));
                //if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(PrivUser1) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[0]) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && CurrentTime.ToString("HHmmss").Contains(CreationTime.Split('.')[0]))
                if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(PrivUser1) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus))
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


                //Step-12: On another client system, login as another privileged user (e.g.,tech/tech).
                login.Logout();
                login.LoginIConnect(PrivUser2, PrivUser2);
                studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step-13: From the "Studies" tab, search for the same series as above, with the new PR series. Load the study into the Enterprise Viewer.Ensure that the viewing scope for the loaded study is set to "Image" scope. If using recommended data, search for "MICKEY, MOUSE" patient and load CT study with date "19-Aug-2004" to the Enterprise Viewer.Modify the Viewing Scope for CT to "Image" scope in the User Preferences.
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                ThumbnailCount = ThumbnailList.Count;
                string step13_1 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).Text;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step13_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step13_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (ThumbnailCount == 6 && step13_1.Contains("PR") && step13_2 && step13_3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Review (go through the images in) the PR series and in the original series (which you have made the annotation and window modifications in the previous steps).
                //Checking original series  - Viewport 2
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(1, 1)).Click();
                string step14_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails))[1].GetAttribute("title");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step14_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 1);

                //Scroll and check other images not affected
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step14_3 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 2);

                //Checking PR series  - Viewport 1
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 1)).Click();
                string step14_4 = Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails))[0].GetAttribute("title");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                var step14_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport, 3);

                //Scroll and check other images not affected
                action = new TestCompleteAction();
                action.MouseScroll(viewport, "down").Perform();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step14_6 = studies.CompareImage(result.steps[ExecutedSteps], viewport, 4, 1);

                if (step14_1.Contains("CT") && step14_4.Contains("PR") && step14_2 && step14_3 && step14_5 && step14_6)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15: Hover over the new PR thumbnail, ensure that the modality is PR. Review the Exam List card(for the current study) and ensure that PR modality is listed, in addition to the existing modality of the original series. Hover over the Exam List card for the current study and ensure that PR modality is listed.
                var ActiveExam = GetElement(SelectorType.CssSelector, BluRingViewer.div_activeExamPanel);
                string step15_1 = ActiveExam.Text;
                string step15_2 = ActiveExam.FindElement(By.CssSelector(BluRingViewer.div_RSmodality)).Text;

                if (step14_4.Contains("PR") && step15_1.Contains("PR") && step15_2.Contains("PR"))
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

                //Step-16: From the PR series (in the first viewport), apply the PAN tool, and move the center of the image to the top-left corner.
                bluringviewer.SelectViewerTool(BluRingTools.Pan);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step16 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step16)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: From the PR series (in the 1st viewport), add a text annotation in addition to the presentation state that is already there.
                bluringviewer.SelectViewerTool(BluRingTools.Add_Text);
                bluringviewer.ApplyTool_AddText("163137 - Comment 2", viewport.Size.Width / 4, viewport.Size.Height / 4);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewport = GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                var step17 = bluringviewer.CompareImage(result.steps[ExecutedSteps], viewport);

                if (step17)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18: Click Save Annotated Image from the viewport Toolbox, to save the applied changes to the PR image.
                //Bug - Saving PR on another PR doesnt work in Enterprise viewer
                DateTime CurrentTime2 = DateTime.Now;
                bool step18_1 = bluringviewer.SavePresentationState(BluRingTools.Save_Annotated_Image);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step18_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step18_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                string step18_3 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
                string step18_4 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text;

                if (ThumbnailList.Count == ThumbnailCount + 1 && step18_1 && step18_2 && step18_5 && step18_3 == "PR" && step18_4 == "1")
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    //throw new Exception("PR not saved hence aborting test");
                }

                Logger.Instance.InfoLog("Save PR Method: " + step18_1);
                Logger.Instance.InfoLog("Image Comparison: " + step18_2);
                Logger.Instance.InfoLog("Thumbail Caption: " + step18_3);
                Logger.Instance.InfoLog("Frame Number: " + step18_4);
                
                bluringviewer.SelectViewerTool(BluRingTools.Pan);

                //Step-19: Select the new PR thumbnail from the Study Panel Thumbnail bar, and drag it into a new (empty) viewport. Review the new PR series in the viewport. Ensure that the new PR series contains the the new text annotation, panned image in addition to the annotation and window setting changes in the first PR.
                ThumbnailCount = ThumbnailList.Count;
                // Drag the 1st thumbnail to the first viewport
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(3, 1)).Click();
                TargetElement = bluringviewer.GetElement(SelectorType.CssSelector, bluringviewer.Activeviewport);
                bluringviewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 4, studyPanelNumber: 1, UseDragDrop: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step19_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], TargetElement);
                string step19_2 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text;

                if (step19_1 && step19_2 == "1")
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20: Close the Enterprise viewer (click "X" EXIT in the global toolbar).
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-21: Go to the data source with the saved PR study, verify in a third party viewer or DICOM file viewer that the new PR modality series contain the following tags with proper values:
                //Clear download directory
                try
                {
                    DeleteAllFileFolder(Config.downloadpath);
                    DeleteAllFileFolder(IEDownloadPath);
                }
                catch (Exception ex) { }
                //Workaround given for FF download issue - session closed and opened again
                login.Logout();
                CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(PrivUser2, PrivUser2);
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID);
                studies.TransferStudy("Local System", SelectallPriors: false, waittime: 180);
                PageLoadWait.WaitForDownload(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);
                studydownloaded = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], IEDownloadPath, Filename[1].Split('.')[1]);
                else
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);

                //Unzip downloaded file
                ZipPath = String.Empty;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    ZipPath = IEDownloadPath + Path.DirectorySeparatorChar + Filename[1];
                else
                    ZipPath = Config.downloadpath + Path.DirectorySeparatorChar + Filename[1];
                ExtractPath = Config.downloadpath;
                UnzipFolderName = UnZipFolder(ZipPath, ExtractPath);
                Thread.Sleep(5000);
                ExtractPath = Config.downloadpath + Path.DirectorySeparatorChar + UnzipFolderName;

                //Open Series 1
                file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + PRFolderName[0] + Path.DirectorySeparatorChar + Filename[0]);

                DicomData = file.Dataset;
                ContentLabel = DicomData.Get<string>(DicomTag.ContentLabel);
                CreationDate = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                CreationTime = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                SOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                CreatorName = DicomData.Get<string>(DicomTag.ContentCreatorName);
                SOPInstanceUID = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                ReferencedSOPClassUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPClassUID);
                ReferencedSOPInstanceUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPInstanceUID);
                Modality = DicomData.Get<string>(DicomTag.Modality);

                Logger.Instance.InfoLog("Step 21 Data 1: " + ContentLabel + " _ " + CreationDate + " _ " + CreationTime + " _ " + SOPClassUID + " _ " + CreatorName + " _ " + SOPInstanceUID + " _ " + ReferencedSOPClassUID + " _ " + ReferencedSOPInstanceUID + " _ " + Modality);

                //Open Series 2
                file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + PRFolderName[1] + Path.DirectorySeparatorChar + Filename[0]);

                DicomData = file.Dataset;
                String ContentLabel2 = DicomData.Get<string>(DicomTag.ContentLabel);
                String CreationDate2 = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                String CreationTime2 = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                String SOPClassUID2 = DicomData.Get<string>(DicomTag.SOPClassUID);
                String CreatorName2 = DicomData.Get<string>(DicomTag.ContentCreatorName);
                String SOPInstanceUID2 = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                String ReferencedSOPClassUID2 = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPClassUID);
                String ReferencedSOPInstanceUID2 = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPInstanceUID);
                String Modality2 = DicomData.Get<string>(DicomTag.Modality);

                Logger.Instance.InfoLog("Step 21 Data 2: " + ContentLabel2 + " _ " + CreationDate2 + " _ " + CreationTime2 + " _ " + SOPClassUID2 + " _ " + CreatorName2 + " _ " + SOPInstanceUID2 + " _ " + ReferencedSOPClassUID2 + " _ " + ReferencedSOPInstanceUID2 + " _ " + Modality2);

                var CapturedTime2 = DateTime.ParseExact(CreationTime2, "HHmmss", CultureInfo.InvariantCulture);
                var CapturedTimeMinus2 = CapturedTime2.AddMinutes(-5);
                var CapturedTimePlus2 = CapturedTime2.AddMinutes(5);

                Logger.Instance.InfoLog("CurrentTime: " + CurrentTime2.ToString("yyyyMMdd") + " _ " + "CurrentTime" + " _ " + CurrentTime2 + " _ " + "CapturedTimeMinus" + " _ " + CapturedTimeMinus2 + " _ " + "CapturedTimePlus" + " _ " + CapturedTimePlus2);
                Logger.Instance.InfoLog("Result: " + ContentLabel.Equals("GSPS") + CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) + SOPClassUID.StartsWith(SOPClassUIDList[0]) + CreatorName.Equals(PrivUser1) + Modality.Equals("PR") + ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) + ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) + (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus));
                Logger.Instance.InfoLog("Result: " + ContentLabel2.Equals("GSPS") + CurrentTime2.ToString("yyyyMMdd").Equals(CreationDate2) + SOPClassUID2.StartsWith(SOPClassUIDList[0]) + CreatorName2.Equals(PrivUser2) + Modality2.Equals("PR") + ReferencedSOPClassUID2.Equals(SOPClassUIDList[1]) + ReferencedSOPInstanceUID2.Equals(SOPInstanceUIDList[1]) + (CurrentTime2 >= CapturedTimeMinus2 && CurrentTime2 <= CapturedTimePlus2));

                //if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyddMM").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(PrivUser1) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[0]) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && CurrentTime.ToString("HHmmss").Contains(CreationTime.Split('.')[0]) &&
                //    ContentLabel2.Equals("GSPS") && CurrentTime2.ToString("yyyyddMM").Equals(CreationDate2) && SOPClassUID2.StartsWith(SOPClassUIDList[2]) && CreatorName2.Equals(PrivUser1) && SOPInstanceUID2.StartsWith(SOPInstanceUIDList[2]) && Modality2.Equals("PR") && ReferencedSOPClassUID2.Equals(SOPClassUIDList[3]) && ReferencedSOPInstanceUID2.Equals(SOPInstanceUIDList[3]) && CurrentTime2.ToString("HHmmss").Contains(CreationTime2.Split('.')[0]))
                if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(PrivUser1) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus) &&
                    ContentLabel2.Equals("GSPS") && CurrentTime2.ToString("yyyyMMdd").Equals(CreationDate2) && SOPClassUID2.StartsWith(SOPClassUIDList[0]) && CreatorName2.Equals(PrivUser2) && Modality2.Equals("PR") && ReferencedSOPClassUID2.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID2.Equals(SOPInstanceUIDList[1]) && (CurrentTime2 >= CapturedTimeMinus2 && CurrentTime2 <= CapturedTimePlus2))
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

                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                try
                {
                    //Deleting uploaded study
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
                try
                {
                    //file.Save(Config.downloadpath + Path.DirectorySeparatorChar + "test1");
                    File.Delete(Config.downloadpath + Path.DirectorySeparatorChar + Filename[1]);
                    DeleteAllFileFolder(ExtractPath);
                }
                catch (Exception ex) { }
            }
        }

        /// <summary>
        /// Saving Series in "Presentation State as Logical Series" - with no annotations
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_164464(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string[] Filename = null;
            string PatientID = null;
            string[] FilePath = null;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;
            String IEDownloadPath = Config.downloadpath;//@"C:\Users\Administrator\Downloads";
            string ExtractPath = string.Empty;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                Filename = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "FileName")).Split(':');
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] PRFolderName = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PRFolderName")).Split(':');
                String[] SOPClassUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPClassUID")).Split(':');
                String[] SOPInstanceUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPInstanceUID")).Split(':');
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                String TestDomain = "TestDomain_142151_" + new Random().Next(1, 1000);
                String Role = "Role_142151_" + new Random().Next(1, 1000);
                String DomainAdmin = "DomainAdmin_145_" + new Random().Next(1, 1000);
                String PhysicianRole = BasePage.GetUniqueRole("PhyRole_142151_");
                String rad1 = BasePage.GetUniqueUserId("rad_142151_");

                //Precondition
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                FullPath = Directory.GetFiles(FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                //The modality for the specific study you are using has "Viewing Scope" set to "Series".
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);               
               
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                domain.SetCheckBoxInEditDomain("datatransfer", 0);
                domain.SetCheckBoxInEditDomain("datadownload", 0);
                domain.ClickSaveEditDomain();

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                if (!role.RoleExists(PhysicianRole))
                {
                    role.CreateRole(TestDomain, PhysicianRole, "physician");
                }
                role.SearchRole(PhysicianRole, TestDomain);
                role.SelectRole(PhysicianRole);
                role.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");               
                role.SetCheckboxInEditRole("transfer", 0);
                role.SetCheckboxInEditRole("download", 0);
                var groupsInUse = role.GetConfiguredToolsInToolBoxConfig();
                if (!(groupsInUse.Contains("Save Annotated Images")) || !(groupsInUse.Contains("Save Series")))
                {
                    IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));
                    var dictionary = new Dictionary<String, IWebElement>();
                    dictionary.Add("Save Annotated Images", group1);
                    dictionary.Add("Save Series", group1);
                    role.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                    Logger.Instance.InfoLog("Tools are configured in the ToolBox");
                }
                role.ClickSaveEditRole();
                UserManagement user = (UserManagement)login.Navigate("UserManagement");
                user.CreateUser(rad1, TestDomain, PhysicianRole, 1, Config.emailid, 1, rad1);
                login.Logout();


                //Step-1:Login to Enterprise Viewer web application as privilege user (e.g., rad/rad)
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //Step-2:From the "Studies" tab, search for a study with multiple images in its series (ensure that the Viewing Scope for this modality is set to "Series" in the configuration).
                //Load the study into the Enterprise Viewer.
                //If using the recommended data set, search for "MICKEY, MOUSE" patient, and load MR study with date "04-Feb-1995".                
                UserPreferences userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.ModalityDropDown().SelectByText("MR");
                userpreferences.ViewingScopeSeriesRadioBtn().Click();
                userpreferences.CloseUserPreferences();

                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-3:Without applying any tools, click Save Series from the viewport Toolbox. 
                DateTime CurrentTime = DateTime.Now;
                bool IsPRExists = viewer.SavePresentationState(BluRingTools.Save_Series,BluRingTools.Scroll_Tool);
                //IList<IWebElement> Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                IList<IWebElement> Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));


                if (Thumbnail_list.Count == 3 && IsPRExists)//Thumbnail_Caption[0].Text.Contains("S8") && IsPRExists && Image_FrameNumber[0].Text.Equals("2")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Issue in Saving PR");
                }

                //Step-4:Select the new PR thumbnail from the Study Panel Thumbnail bar, and drag it into a new (empty) viewport.
                //Review the new PR series in the viewport.
                //Ensure that the new PR series contains all the images from the original series and that no modifications are visible from the original.
                viewer.SetViewPort(2, 1);
                TestCompleteAction action = new TestCompleteAction();
                IWebElement TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action.DragAndDrop(Thumbnail_list[0], TargetElement);

                //loaded viewports 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step4_1)
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

                //Step-5:Close the Study Panel and open the same study (from previous step) again in the Enterprise Viewer.
                //login.DriverGoTo(login.url);
                //login.LoginIConnect(rad1, rad1);
                viewer.CloseBluRingViewer();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                //loaded viewports 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step5_1)
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

                //Step-6:Go to the data source with the saved PR study, verify in a third party viewer or DICOM file viewer that the new PR modality series contain the following tags with proper values-
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID);
                studies.TransferStudy("Local System", SelectallPriors: false, waittime: 180);
                PageLoadWait.WaitForDownload(Filename[0], IEDownloadPath, "zip");
                Boolean studydownloaded = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    studydownloaded = BasePage.CheckFile(Filename[0], IEDownloadPath, "zip");
                else
                    studydownloaded = BasePage.CheckFile(Filename[0], IEDownloadPath, "zip");

                //Unzip downloaded file
                string ZipPath = String.Empty;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    ZipPath = IEDownloadPath + Path.DirectorySeparatorChar + Filename[1];
                else
                    ZipPath = IEDownloadPath + Path.DirectorySeparatorChar + Filename[1];
                ExtractPath = IEDownloadPath;
                string UnzipFolderName = UnZipFolder(ZipPath, ExtractPath);
                Thread.Sleep(5000);
                ExtractPath = IEDownloadPath + Path.DirectorySeparatorChar + UnzipFolderName;

                var file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + PRFolderName[0] + Path.DirectorySeparatorChar + Filename[0]);

                var DicomData = file.Dataset;
                string ContentLabel = DicomData.Get<string>(DicomTag.ContentLabel);
                string CreationDate = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                string CreationTime = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                string SOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                string CreatorName = DicomData.Get<string>(DicomTag.ContentCreatorName);
                string SOPInstanceUID = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                string ReferencedSOPClassUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPClassUID);
                string ReferencedSOPInstanceUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPInstanceUID);
                string Modality = DicomData.Get<string>(DicomTag.Modality);

                Logger.Instance.InfoLog("Data 1: " + ContentLabel + " _ " + CreationDate + " _ " + CreationTime + " _ " + SOPClassUID + " _ " + CreatorName + " _ " + SOPInstanceUID + " _ " + ReferencedSOPClassUID + " _ " + ReferencedSOPInstanceUID + " _ " + Modality);

                var CapturedTime = DateTime.ParseExact(CreationTime, "HHmmss", CultureInfo.InvariantCulture);
                var CapturedTimeMinus = CapturedTime.AddMinutes(-5);
                var CapturedTimePlus = CapturedTime.AddMinutes(5);
                Logger.Instance.InfoLog("CurrentTime: " + CurrentTime.ToString("yyyyMMdd") + " _ " + "CurrentTime" + " _ " + CurrentTime + " _ " + "CapturedTimeMinus" + " _ " + CapturedTimeMinus + " _ " + "CapturedTimePlus" + " _ " + CapturedTimePlus);
                Logger.Instance.InfoLog("Result: " + ContentLabel.Equals("GSPS") + CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) + SOPClassUID.StartsWith(SOPClassUIDList[0]) + CreatorName.Equals(rad1) + Modality.Equals("PR") + ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) + ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) + (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus));
                //if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(PrivUser1) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[0]) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && CurrentTime.ToString("HHmmss").Contains(CreationTime.Split('.')[0]))
                if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(rad1) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus))
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
                try
                {
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DS1));
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception e) { }
                try
                {
                    //file.Save(Config.downloadpath + Path.DirectorySeparatorChar + "test1");
                    File.Delete(IEDownloadPath + Path.DirectorySeparatorChar + Filename[1]);
                    DeleteAllFileFolder(ExtractPath);
                }
                catch (Exception ex) { }
            }

        }

        /// <summary>
        /// Save Series with "Image" viewing scope
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_164463(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string[] Filename = null;
            string PatientID = null;
            string[] FilePath = null;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;
            String IEDownloadPath = Config.downloadpath;//@"C:\Users\Administrator\Downloads";
            string ExtractPath = string.Empty;

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                Filename = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "FileName")).Split(':');
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] PRFolderName = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PRFolderName")).Split(':');
                String[] SOPClassUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPClassUID")).Split(':');
                String[] SOPInstanceUIDList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPInstanceUID")).Split(':');
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;//EA1AETitle;//DestEAsAETitle;
                DS1Port = 12000;
                String TestDomain = "TestDomain_140788_" + new Random().Next(1, 1000);
                String Role = "Role_140788_" + new Random().Next(1, 1000);
                String DomainAdmin = "DomainAdmin_140788_" + new Random().Next(1, 1000);
                String PhysicianRole = BasePage.GetUniqueRole("PhyRole_140788_");
                String rad1 = BasePage.GetUniqueUserId("rad_140788_");

                //Precondition
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                FullPath = Directory.GetFiles(FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                domain.SetCheckBoxInEditDomain("datatransfer", 0);
                domain.SetCheckBoxInEditDomain("datadownload", 0);
                domain.ClickSaveEditDomain();

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                if (!role.RoleExists(PhysicianRole))
                {
                    role.CreateRole(TestDomain, PhysicianRole, "physician");
                }
                role.SearchRole(PhysicianRole, TestDomain);
                role.SelectRole(PhysicianRole);
                role.ClickEditRole();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                role.SetCheckboxInEditRole("transfer", 0);
                role.SetCheckboxInEditRole("download", 0);
                var groupsInUse = role.GetConfiguredToolsInToolBoxConfig();
                if (!(groupsInUse.Contains("Save Annotated Images")) || !(groupsInUse.Contains("Save Series")))
                {                    
                    IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(5)"));
                    var dictionary = new Dictionary<String, IWebElement>();
                    dictionary.Add("Save Annotated Images", group1);
                    dictionary.Add("Save Series", group1);
                    role.AddToolsToToolbox(dictionary,addToolAtEnd: true);
                    Logger.Instance.InfoLog("Tools are configured in the ToolBox");
                }
                role.ClickSaveEditRole();
                UserManagement user = (UserManagement)login.Navigate("UserManagement");
                user.CreateUser(rad1, TestDomain, PhysicianRole, 1, Config.emailid, 1, rad1);
                login.Logout();

                //Step-1:Login to Enterprise Viewer web application as privilege user (e.g., rad/rad)
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //Step-2:From the "Studies" tab, search for a study with multiple images in its series(ensure that the Viewing Scope for this modality is set to "Image" in the configuration).
                //Load the study into the Enterprise Viewer.
                UserPreferences userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.ModalityDropDown().SelectByText("MR");
                userpreferences.ViewingScopeImageRadioBtn().Click();
                userpreferences.CloseUserPreferences();

                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                //loaded viewports 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step2_1)
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


                //Step-3:Without applying any tools, click Save Series from the viewport Toolbox.
                DateTime CurrentTime = DateTime.Now;
                bool IsSavePRExists = viewer.SavePresentationState(BluRingTools.Save_Series,BluRingTools.Scroll_Tool);
                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                IList<IWebElement> Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));


                if (Thumbnail_list.Count == 3 &&
                    Thumbnail_Caption[0].Text.Contains("S8") && IsSavePRExists)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Issue in Saving operation");
                }
                viewer.CloseBluRingViewer();

                //Step-4:Open the same study (from previous step) again in the Enterprise Viewer.
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                //loaded viewports 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step4_1)
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

                //Step-5:From the "Studies" tab, search for a study with multiple images in its series(ensure that the Viewing Scope for this modality is set to "Image" in the configuration).	
                //Load the study into the Enterprise Viewer.
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-6:From the first series in the first viewport, apply the following W/L to the first image: make the image considerably darker (fewer grays, more contrast)
                viewer.SetViewPort(2, 1);
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));// element.Click();
                viewer.SelectViewerTool(BluRingTools.Window_Level, 1, 3);
                viewer.ApplyTool_WindowWidth();
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps, 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                var action = new TestCompleteAction();
                action.MouseScroll(element, "down", "1");
                result.steps[ExecutedSteps].SetPath(testid + "_2", ExecutedSteps, 2);
                bool step6_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "1");
                result.steps[ExecutedSteps].SetPath(testid + "_3", ExecutedSteps, 3);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), totalImageCount: 2, IsFinal: 1);

                if (step6 && step6_1 && step6_1)
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

                //Step-7:From the first series in the first viewport, apply the following to the 2nd image: invert the image.               
                viewer.SelectViewerTool(BluRingTools.Invert, 1, 3);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewport));
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

                //Step-8:From the first series in the first viewport, apply the following to the 3rd image: Flip the image vertically.
                action.MouseScroll(element, "down", "1");
                viewer.SelectInnerViewerTool(BluRingTools.Flip_Vertical, BluRingTools.Flip_Horizontal, panel: 1, viewport: 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewport));
                if (step8)
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

                //Step-9:From the first series in the first viewport, apply the following to the 4th image: add a text value on the image, add a measurement on the image (line, cobb angle, etc.)
                action.MouseScroll(element, "down", "1");
                viewer.SelectViewerTool(BluRingTools.Add_Text, 1, 3);
                viewer.ApplyTool_AddText("test");
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, 1, 3);
                IWebElement viewport = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                var viewportSize = viewport.Size;
                viewer.ApplyTool_LineMeasurement(viewportSize.Width / 3, viewportSize.Height / 3, viewportSize.Width / 5, viewportSize.Height / 5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step9)
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

                //Step-10:Click Save Series from the viewport Toolbox, to save the applied changes.
                IsSavePRExists = viewer.SavePresentationState(BluRingTools.Save_Series,outerTool: BluRingTools.Scroll_Tool, panel: 1, viewport: 3);
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                IList<IWebElement> Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));

                if (Thumbnail_list.Count == 4 &&
                    Thumbnail_Caption[0].Text.Contains("S9") && IsSavePRExists && Image_FrameNumber[0].Text.Equals("100"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Issue in Saving operation");
                }

                //Step-11:Close the study. (Click "X" from the top-right of the Study Panel)
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-12:From the "Studies" tab, search for the same series as above, with the new PR series
                //Load the study into the Enterprise Viewer.
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                //loaded viewports 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step12_1)
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

                //Step-13:Select the PR series viewport and review the images in the series.
                viewer.SetViewPort(0, 1);
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps, 1);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "1");
                result.steps[++ExecutedSteps].SetPath(testid + "_2", ExecutedSteps, 2);
                bool step13_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "1");
                result.steps[++ExecutedSteps].SetPath(testid + "_3", ExecutedSteps, 3);
                bool step13_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "1");
                result.steps[++ExecutedSteps].SetPath(testid + "_4", ExecutedSteps, 4);
                bool step13_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), totalImageCount: 4, IsFinal: 1);

                if (Image_FrameNumber[0].Text.Equals("100") && step13 && step13_1 && step13_2 && step13_3)
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

                //Step-14:Select the original series (of the PR series) and review the images in the series.
                viewer.SetViewPort(3, 1);
                result.steps[++ExecutedSteps].SetPath(testid + "_1", ExecutedSteps, 1);
                bool step14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "1");
                result.steps[++ExecutedSteps].SetPath(testid + "_2", ExecutedSteps, 2);
                bool step14_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "1");
                result.steps[++ExecutedSteps].SetPath(testid + "_3", ExecutedSteps, 3);
                bool step14_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                action.MouseScroll(element, "down", "1");
                result.steps[++ExecutedSteps].SetPath(testid + "_4", ExecutedSteps, 4);
                bool step14_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), totalImageCount: 4, IsFinal: 1);
                if (step14 && step14_1 && step14_2 && step14_3)
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

                //Step-15:Go to the data source with the saved PR study, verify in a third party viewer or DICOM file viewer that the new PR modality series contain the following tags with proper values:
                studies.SearchStudy(LastName: LastName, FirstName: FirstName, Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID);
                studies.TransferStudy("Local System", SelectallPriors: false, waittime: 180);
                PageLoadWait.WaitForDownload(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);
                Boolean studydownloaded = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], IEDownloadPath, Filename[1].Split('.')[1]);
                else
                    studydownloaded = BasePage.CheckFile(Filename[1].Split('.')[0], Config.downloadpath, Filename[1].Split('.')[1]);

                //Unzip downloaded file
                string ZipPath = String.Empty;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    ZipPath = IEDownloadPath + Path.DirectorySeparatorChar + Filename[1];
                else
                    ZipPath = Config.downloadpath + Path.DirectorySeparatorChar + Filename[1];
                ExtractPath = Config.downloadpath;
                string UnzipFolderName = UnZipFolder(ZipPath, ExtractPath);
                Thread.Sleep(5000);
                ExtractPath = Config.downloadpath + Path.DirectorySeparatorChar + UnzipFolderName;

                var file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + PRFolderName[0] + Path.DirectorySeparatorChar + Filename[0]);

                var DicomData = file.Dataset;
                string ContentLabel = DicomData.Get<string>(DicomTag.ContentLabel);
                string CreationDate = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                string CreationTime = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                string SOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                string CreatorName = DicomData.Get<string>(DicomTag.ContentCreatorName);
                string SOPInstanceUID = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                string ReferencedSOPClassUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPClassUID);
                string ReferencedSOPInstanceUID = (((DicomData.Get<DicomSequence>(DicomTag.ReferencedSeriesSequence).Items[0].Get<DicomSequence>(DicomTag.ReferencedImageSequence)))).Items[0].Get<string>(DicomTag.ReferencedSOPInstanceUID);
                string Modality = DicomData.Get<string>(DicomTag.Modality);

                Logger.Instance.InfoLog("Data 1: " + ContentLabel + " _ " + CreationDate + " _ " + CreationTime + " _ " + SOPClassUID + " _ " + CreatorName + " _ " + SOPInstanceUID + " _ " + ReferencedSOPClassUID + " _ " + ReferencedSOPInstanceUID + " _ " + Modality);

                var CapturedTime = DateTime.ParseExact(CreationTime, "HHmmss", CultureInfo.InvariantCulture);
                var CapturedTimeMinus = CapturedTime.AddMinutes(-5);
                var CapturedTimePlus = CapturedTime.AddMinutes(5);
                Logger.Instance.InfoLog("CurrentTime: " + CurrentTime.ToString("yyyyMMdd") + " _ " + "CurrentTime" + " _ " + CurrentTime + " _ " + "CapturedTimeMinus" + " _ " + CapturedTimeMinus + " _ " + "CapturedTimePlus" + " _ " + CapturedTimePlus);
                Logger.Instance.InfoLog("Result: " + ContentLabel.Equals("GSPS") + CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) + SOPClassUID.StartsWith(SOPClassUIDList[0]) + CreatorName.Equals(rad1) + Modality.Equals("PR") + ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) + ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) + (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus));
                //if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(PrivUser1) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[0]) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && CurrentTime.ToString("HHmmss").Contains(CreationTime.Split('.')[0]))
                if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(rad1) && Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) && (CurrentTime >= CapturedTimeMinus && CurrentTime <= CapturedTimePlus))
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
                try
                {
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(DS1));
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch(Exception ex) { }
                try
                {
                    //file.Save(Config.downloadpath + Path.DirectorySeparatorChar + "test1");
                    File.Delete(Config.downloadpath + Path.DirectorySeparatorChar + Filename[1]);
                    DeleteAllFileFolder(ExtractPath);
                }
                catch (Exception ex) { }
            }

        }



        /// <summary>
        /// Saving Annotated Images in "Presentation State as Logical Series" with "Series" viewing scope mode
        /// </summary>
        public TestCaseResult Test_164462(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string[] FilePath = null;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;
            UserManagement usermanagement = new UserManagement();
            String[] PatientID = null;
            bool isDefaultTool = false;
            String defaultDownloadPath = Config.downloadpath;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
               
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                PatientID = PatientIDList.Split(':');
                String[] Filename = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "FileName")).Split(':');
                String[] SOPClassUIDList = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "SOPClassUID")).Split(':');
                String[] SOPInstanceUIDList = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "SOPInstanceUID")).Split(':');

                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //Precondition2 - Enable Features > Enable Saving GSPS is selected in the iCA Service Tool. 
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Enable Features");
                wpfobject.ClickButton("Modify", 1);
                servicetool.EnableSavingGSPS();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Precondition3 - "Presentation States as Logical Series" is configured in Domain/Role/User Preferences  
                // Yet to be implemented

                //Precondition4 - Enterprise viewer > Viewports not in "Global Stack" mode.
                // Yet to be implemented

                // Pushing dataset to EA datasource
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                FullPath = Directory.GetFiles(FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
             //   Directory.Delete(Config.downloadpath, true);

                // Ensure rad1 privilege users are configured.
                login.LoginIConnect(username, password);
                String User = "rad" + new Random().Next(10000);
                String User1 = "tech" + new Random().Next(10000);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, "SuperAdminGroup", "SuperRole");
                usermanagement.CreateUser(User1, "SuperAdminGroup", "SuperRole");

                // Add Save Annotated Image
                DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();               
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var groupsInUse = domainmanagement.GetConfiguredToolsInToolBoxConfig();
                if (!(groupsInUse.Contains("Save Annotated Images")))
                {
                    isDefaultTool = true;
                    IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                    var dictionary = new Dictionary<String, IWebElement>();
                    dictionary.Add("Save Annotated Images", group1);
                    domainmanagement.AddToolsToToolbox(dictionary);                    
                    Logger.Instance.InfoLog("Save Annotated Images tool  is configured in the ToolBox");
                }
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(60);
                login.Logout();             


                //Step1 - Login to Enterprise Viewer web application as privilege user
                login.LoginIConnect(User, User);
                if(login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step2 - Search study and load it in viewer
                UserPreferences userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.ModalityDropDown().SelectByText("MR");
                userpreferences.ViewingScopeSeriesRadioBtn().Click();
                userpreferences.CloseUserPreferences();

                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Patient ID", PatientID[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bool step2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer)).Count == 1;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                    GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step2 && step2_1)
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

                //Step3 - Apply W/L in first viewport
                viewer.SelectViewerTool(BluRingTools.Window_Level);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                int width = ele.Size.Width;
                int height = ele.Size.Height;
                viewer.ApplyTool_WindowWidth(width / 5, height / 5, width / 2, height / 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), 1);

                // Verify next image to ensure series scope
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(ele, "down");
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step3_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                    GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), 2, 1);
                if (step3 && step3_1)
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

                //Step4 - Apply Flip Vertical for third image of first viewport.
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action.MouseScroll(ele, "down").Perform();
                Thread.Sleep(1000);
                viewer.ApplyTool_FlipVertical();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                    GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), 1);

                // To ensure series scope
                action = new TestCompleteAction();
                action.MouseScroll(ele, "up");
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                    GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), 2, 1);
                if (step4_1 && step4_2)
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

                //step5 - Apply AddText on 1st Image and Line Measurement on 4th Image
                action.MouseScroll(ele, "up").Perform();
                Thread.Sleep(1000);
                bool step5 = viewer.SelectViewerTool(BluRingTools.Add_Text);
                viewer.ApplyTool_AddText("Test");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                    GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "3").Perform();
                Thread.Sleep(1000);
                bool step5_2 = viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                viewer.ApplyTool_LineMeasurement();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step5_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                            GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), 2);

                // To ensure annotation is applied per image (Add Text and Line Measurement annotations should not be applied on 3rd image) 
                action = new TestCompleteAction();
                action.MouseScroll(ele, "up");
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step5_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                            GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), 3, 1);
                if (step5 && step5_1 && step5_2 && step5_3 && step5_4)
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

                //Step6 - Navigate to the first image of the first viewport series, and click Save Annotated Image from the viewport Toolbox, to save the applied changes.
                DateTime CurrentTime = DateTime.Now;
                int thumbnailsCountBeforeSaveAnnotation = viewer.ThumbnailIndicator(0).Count;
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action.MouseScroll(ele, "up");
                Thread.Sleep(1000);
                action.MouseScroll(ele, "up").Perform();
                Thread.Sleep(1000);
                viewer.SetViewPort(0, 1);
                Thread.Sleep(10000);
                // viewer.OpenViewerToolsPOPUp();
                bool step6 = viewer.SavePresentationState(BluRingTools.Save_Annotated_Image);
                Thread.Sleep(1000);
                IList<IWebElement> thumbnailsAfterSaveAnnotation = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                bool step6_1 = thumbnailsAfterSaveAnnotation.Count == thumbnailsCountBeforeSaveAnnotation + 1;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], thumbnailsAfterSaveAnnotation.ElementAt(0));

                //First viewport should be active
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).FindElement(By.XPath(".."));
                bool step6_3 = element.GetAttribute("class").Contains("activeViewportContainer selected");

                // 2nd Thumbnail should be in Focus                
                element = thumbnailsAfterSaveAnnotation.ElementAt(1).FindElement(By.XPath(".."));                
                bool step6_4 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                if (step6 && step6_1 && step6_2 && step6_3 && step6_4)
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

                //step7 - Drag new PR thumbnail to 3rd viewport
                viewer.SetViewPort(2, 1);
                IWebElement targetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action = new TestCompleteAction();
                action.DragAndDrop(thumbnailsAfterSaveAnnotation[0], targetElement);
                BluRingViewer.WaitforViewports();
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Drag and Drop thumbnail to viewport is completed");

                // Annotated images alone should be available in PR
                bool step7_1 = thumbnailsAfterSaveAnnotation.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text.Equals("2");
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7_2 = studies.CompareImage(result.steps[ExecutedSteps], element, 1);
                action.MouseScroll(element, "down");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step7_3 = studies.CompareImage(result.steps[ExecutedSteps], element, 2, 1);
                if (step7_1 && step7_2 && step7_3)
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

                //Step8 - Annotations should be present in original series
                viewer.SetViewPort(0, 1);
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step8_1 = studies.CompareImage(result.steps[ExecutedSteps], element, 1);
                action.MouseScroll(element, "down", "3");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step8_2 = studies.CompareImage(result.steps[ExecutedSteps], element, 2, 1);
                if (step8_1 && step8_2)
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

                //Step9 - Close BluRing Viewer           
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step10 - Search the same study                              
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA96));                
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages)); ;
                bool step10_1 = thumbnails.Count == 3;
                bool step10_2 = thumbnails.ElementAt(0).FindElement(By.CssSelector("div.thumbnailImage")).
                                GetAttribute("title").Contains("Modality:PR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                             GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step10_1 && step10_2 && step10_3)
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

                //Step11 - 
                bool step11_1 = thumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text == "S4- 1";
                bool step11_2 = thumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text.Equals("2");
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);               
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step11_3 = studies.CompareImage(result.steps[ExecutedSteps], element, 1);
                action.MouseScroll(element, "down");
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step11_4 = studies.CompareImage(result.steps[ExecutedSteps], element, 2, 1);
                if (step11_1 && step11_2 && step11_3 && step11_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }  // remaining verification for this step done in step 10 itself.

                //Step12 - 
                bool step12_1 = thumbnails.ElementAt(0).FindElement(By.CssSelector("div.thumbnailImage")).
                               GetAttribute("title").Contains("Modality:PR");
                viewer.OpenExamListThumbnailPreview(0);
                bool step12_2 = viewer.ExamListThumbnailIndicator(0).Count == 3;
                bool step12_3 = viewer.ExamListThumbnailIndicator(0).ElementAt(0).FindElement(By.CssSelector("div.thumbnailImage")).
                                GetAttribute("title").Contains("Modality:PR");
                if (step12_1 && step12_2 && step12_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step13 -                 
                viewer.CloseBluRingViewer();
                login.Logout();
                Thread.Sleep(5000);
                BasePage.Driver.SwitchTo().DefaultContent();
                if (login.UserIdTxtBox().Displayed)                    
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step14 - DICOM Tags verification
                if (BasePage.SBrowserName.Contains("explorer"))
                    Config.downloadpath = @"D:\BatchExecution\Selenium\Downloads";               
                login.LoginIConnect(User, User);                 
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Patient ID", PatientID[0]);
                studies.TransferStudy("Local System", SelectallPriors: false);
                PageLoadWait.WaitForDownload(Filename[0], Config.downloadpath, "zip");
                Boolean studydownloaded = BasePage.CheckFile(Filename[0], Config.downloadpath, "zip");

                string ZipPath = Config.downloadpath + Path.DirectorySeparatorChar + Filename[0] + ".zip";
                string ExtractPath = Config.downloadpath;
                string UnzipFolderName = basepage.UnZipFolder(ZipPath, ExtractPath);
                ExtractPath = Config.downloadpath + Path.DirectorySeparatorChar + UnzipFolderName;

                var file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + Filename[1] + Path.DirectorySeparatorChar + Filename[2]);

                var DicomData = file.Dataset;
                string ContentLabel = DicomData.Get<string>(DicomTag.ContentLabel);
                string CreationDate = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                string CreationTime = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                string SOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                string CreatorName = DicomData.Get<string>(DicomTag.ContentCreatorName);
                string SOPInstanceUID = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                string ReferencedSOPClassUID = DicomData.Get<string>(DicomTag.ReferencedSOPClassUID);
                string ReferencedSOPInstanceUID = DicomData.Get<string>(DicomTag.ReferencedSOPInstanceUID);
                string Modality = DicomData.Get<string>(DicomTag.Modality);

                //if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) && 
                //        SOPClassUID.StartsWith(SOPClassUIDList[0]) && CreatorName.Equals(User) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[0]) && 
                //        Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) &&
                //        CurrentTime.ToString("HHmmss").Contains(CreationTime.Split('.')[0]))
                if (ContentLabel.Equals("GSPS") && SOPClassUID.Equals(SOPClassUIDList[0]) && CreatorName.Equals(User) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[0]) &&
                       Modality.Equals("PR") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Content Label - Expected : GSPS" + "  Actual : " + ContentLabel);
                    Logger.Instance.InfoLog("SOPClassUID - Expected : " + SOPClassUIDList[0] + "  Actual : " + SOPClassUID);
                    Logger.Instance.InfoLog("CreatorName - Expected : " + User + "  Actual : " + CreatorName);
                    Logger.Instance.InfoLog("SOPInstanceUID - Expected : " + SOPInstanceUIDList[0] + "  Actual : " + CreatorName);
                    Logger.Instance.InfoLog("Creation Date - Expected : " + CurrentTime.ToString("yyyyMMdd") + "  Actual : " + CreationDate);
                }
                login.Logout();
                Directory.Delete(ExtractPath, true);               

                //Step15 - Open new session
                // login.CreateNewSesion();
                login.DriverGoTo(login.url);
                BasePage.Driver.SwitchTo().DefaultContent();
                login.LoginIConnect(User1, User1);

                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step16 - Load the same study
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                thumbnails = viewer.ThumbnailIndicator(0);
                bool step16_1 = thumbnails.Count == 3;
                bool step16_2 = thumbnails.ElementAt(0).FindElement(By.CssSelector("div.thumbnailImage")).
                                GetAttribute("title").Contains("Modality:PR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step16_3 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement
                                    (By.CssSelector(BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer)));
                if (step16_1 && step16_2 && step16_3)
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

                // Step17 - Verify the series number
                bool step17_1 = thumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text == "S4- 1";
                bool step17_2 = thumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text.Equals("2");
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);               
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step17_3 = studies.CompareImage(result.steps[ExecutedSteps], element, 1);
                action.MouseScroll(element, "down").Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step17_4 = studies.CompareImage(result.steps[ExecutedSteps], element, 2, 1);
                if (step17_1 && step17_2 && step17_3 && step17_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }  // remaining verification for this step done in step16 itself.

                //step18 - Apply Pan tool on non-PR series
                viewer.SetViewPort(2, 1);
                bool step18_1 = viewer.SelectViewerTool(BluRingTools.Pan, viewport: 3);
                viewer.ApplyTool_Pan();
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step18_2 = studies.CompareImage(result.steps[ExecutedSteps], element, 1);
                action = new TestCompleteAction();
                action.MouseScroll(element, "down").Perform();
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step18_3 = studies.CompareImage(result.steps[ExecutedSteps], element, 2, 1);
                if (step18_1 && step18_2 && step18_3)
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

                //Step19 - non-PR series Add text annotation on first image 
                viewer.SetViewPort(1, 1);
                BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).Click();
                Thread.Sleep(2000);
                viewer.SelectViewerTool(BluRingTools.Add_Text, viewport: 2);
                Thread.Sleep(4000);
                viewer.SetViewPort(2, 1);
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                element.Click();
                new TestCompleteAction().MouseScroll(element, "up").Perform();                
                Thread.Sleep(5000);               
                viewer.ApplyTool_AddText("test");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step19_2 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)));
                if (step19_2)
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

                //Step20 - Save annotated image
                thumbnailsCountBeforeSaveAnnotation = viewer.ThumbnailIndicator(0).Count;
                bool step20 = viewer.SavePresentationState(BluRingTools.Save_Annotated_Image, viewport: 3);
                Thread.Sleep(1000);
                thumbnailsAfterSaveAnnotation = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                bool step20_1 = thumbnailsAfterSaveAnnotation.Count == thumbnailsCountBeforeSaveAnnotation + 1;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step20_2 = studies.CompareImage(result.steps[ExecutedSteps], thumbnailsAfterSaveAnnotation.ElementAt(0));

                //Thrid viewport should be active
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step20_3 = element.GetAttribute("class").Contains("activeViewportContainer selected");

                // 3rd Thumbnail should be in Focus 
                element = thumbnailsAfterSaveAnnotation.ElementAt(3).FindElement(By.XPath(".."));
                bool step20_4 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                if (step20 && step20_1 && step20_2 && step20_3 && step20_4)
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

                //step21 - 
                viewer.SetViewPort(3, 1);
                targetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action = new TestCompleteAction();
                action.DragAndDrop(thumbnailsAfterSaveAnnotation[0], targetElement).Perform();
                BluRingViewer.WaitforViewports();
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Drag and Drop thumbnail to viewport is completed");
                bool step21_1 = thumbnailsAfterSaveAnnotation.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text.Equals("1");
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step21_2 = studies.CompareImage(result.steps[ExecutedSteps], element);
                if (step21_1 && step21_2)
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

                //step22 - Logout                
                viewer.CloseBluRingViewer();
                login.Logout();
                Thread.Sleep(5000);
                BasePage.Driver.SwitchTo().DefaultContent();
                if (login.UserIdTxtBox().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step23 - DICOM tag verification 
                if (BasePage.SBrowserName.Contains("explorer"))
                    Config.downloadpath = @"D:\BatchExecution\Selenium\Downloads";
                login.LoginIConnect(User, User);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Patient ID", PatientID[0]);
                studies.TransferStudy("Local System", SelectallPriors: false);
                PageLoadWait.WaitForDownload(Filename[0], Config.downloadpath, "zip");
                studydownloaded = BasePage.CheckFile(Filename[0], Config.downloadpath, "zip");

                ZipPath = Config.downloadpath + Path.DirectorySeparatorChar + Filename[0] + " (1).zip";
                ExtractPath = Config.downloadpath;
                UnzipFolderName = basepage.UnZipFolder(ZipPath, ExtractPath);
                ExtractPath = Config.downloadpath + Path.DirectorySeparatorChar + UnzipFolderName;

                file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + Filename[3] + Path.DirectorySeparatorChar + Filename[4]);

                DicomData = file.Dataset;
                ContentLabel = DicomData.Get<string>(DicomTag.ContentLabel);
                CreationDate = DicomData.Get<string>(DicomTag.PresentationCreationDate);
                CreationTime = DicomData.Get<string>(DicomTag.PresentationCreationTime);
                SOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                CreatorName = DicomData.Get<string>(DicomTag.ContentCreatorName);
                SOPInstanceUID = DicomData.Get<string>(DicomTag.SOPInstanceUID);
                ReferencedSOPClassUID = DicomData.Get<string>(DicomTag.ReferencedSOPClassUID);
                ReferencedSOPInstanceUID = DicomData.Get<string>(DicomTag.ReferencedSOPInstanceUID);
                Modality = DicomData.Get<string>(DicomTag.Modality);

                //if (ContentLabel.Equals("GSPS") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate) &&
                //        SOPClassUID.StartsWith(SOPClassUIDList[1]) && CreatorName.Equals(User1) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[1]) &&
                //        Modality.Equals("PR") && ReferencedSOPClassUID.Equals(SOPClassUIDList[1]) && ReferencedSOPInstanceUID.Equals(SOPInstanceUIDList[1]) &&
                //        CurrentTime.ToString("HHmmss").Contains(CreationTime.Split('.')[0]))
                if (ContentLabel.Equals("GSPS") && SOPClassUID.StartsWith(SOPClassUIDList[1]) && CreatorName.Equals(User1) && SOPInstanceUID.StartsWith(SOPInstanceUIDList[1]) &&
                      Modality.Equals("PR") && CurrentTime.ToString("yyyyMMdd").Equals(CreationDate))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Content Label - Expected : GSPS" + "  Actual : " + ContentLabel);
                    Logger.Instance.InfoLog("SOPClassUID - Expected : " + SOPClassUIDList[1] + "  Actual : " + SOPClassUID);
                    Logger.Instance.InfoLog("CreatorName - Expected : " + User + "  Actual : " + CreatorName);
                    Logger.Instance.InfoLog("SOPInstanceUID - Expected : " + SOPInstanceUIDList[1] + "  Actual : " + SOPInstanceUID);
                }
                login.Logout();
                Directory.Delete(ExtractPath, true);


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                login.Logout();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Config.downloadpath = defaultDownloadPath;
                if (isDefaultTool)
                {
                    login.LoginIConnect("Administrator", "Administrator");
                    DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                    domainmanagement.SearchDomain("SuperAdminGroup");
                    domainmanagement.SelectDomain("SuperAdminGroup");
                    domainmanagement.ClickEditDomain();
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                              (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                    Modality.SelectByText("default");
                    Thread.Sleep(1000);
                    IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                    if (revertButton.Enabled)
                        revertButton.Click();
                    domainmanagement.ClickSaveEditDomain();
                    login.Logout();
                }

                try
                {
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID[0]);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
            }
        }

        /// <summary>
        /// Presentation State Handling - Saving Annotated Images in "Presentation State as Logical Series" - with no annotation.
        /// </summary>
        public TestCaseResult Test_164461(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            string[] FilePath = null;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;
            UserManagement usermanagement = new UserManagement();
            String PatientID = null;            
            bool isDefaultTool = false;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //Precondition - 2 
                var servicetool = new ServiceTool();
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
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //pre-condition - 3 & 4 Yet to Implement 

                //Pushing dataset to EA datasource
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                FullPath = Directory.GetFiles(FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                String username = Config.adminUserName;
                String password = Config.adminPassword;
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                // Ensure rad1 privilege users are configured.
                login.LoginIConnect(adminUserName, adminPassword);
                var domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var groupsInUse = domainmanagement.GetConfiguredToolsInToolBoxConfig();
                if (!(groupsInUse.Contains("Save Annotated Images")))
                {
                    isDefaultTool = true;
                    IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                    var dictionary = new Dictionary<String, IWebElement>();
                    dictionary.Add("Save Annotated Images", group1);
                    domainmanagement.AddToolsToToolbox(dictionary);                   
                    Logger.Instance.InfoLog("Save Annotated Images tool  is configured in the ToolBox");
                }
                domainmanagement.ClickSaveEditDomain();
                String User = "rad" + new Random().Next(10000);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, "SuperAdminGroup", "SuperRole");
                login.Logout();

                //step1  Login to Enterprise Viewer web application as privilege user
                login.LoginIConnect(User, User);
                if (login.IsTabPresent("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step2  From the "Studies" tab, search for a study with multiple images in its series. Load the study into the Enterprise Viewer.
                UserPreferences userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.ModalityDropDown().SelectByText("MR");
                userpreferences.ViewingScopeSeriesRadioBtn().Click();
                userpreferences.CloseUserPreferences();

                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                var step2 = BluRingViewer.TotalStudyPanel() == 1;
                var step2_1 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_compositeViewer));
                var step2_2 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_thumbnails));
                if (step2 && step2_1 && step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step3 - Without applying any tools, click Save Annotated Image from the viewport Toolbox.
                int viewportsBeforeSaveAnnotation = 4;
                int thumbnailsBeforeSaveAnnotation = 2;
                bool step3 = viewer.SavePresentationState(BluRingTools.Save_Annotated_Image);
                bool step3_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count == viewportsBeforeSaveAnnotation;
                bool step3_2 = BluRingViewer.NumberOfThumbnailsInStudyPanel() == thumbnailsBeforeSaveAnnotation;
                bool step3_3 = false;
                if(BasePage.SBrowserName.ToLower().Contains("explorer"))
                    step3_3 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                else
                    step3_3 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                if (!step3 && step3_1 && step3_2 && step3_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step4  Hover the mouse over the warning icon in the Study Panel titlebar.
                String Warningmessage = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_PRStatusIndicator).GetAttribute("title");
                if (Warningmessage.Trim() == "No annotations in the series.")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step5  Close the Enterprise Viewer, and re-load the same study.
                viewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bool step5 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count == viewportsBeforeSaveAnnotation;
                bool step5_1 = BluRingViewer.NumberOfThumbnailsInStudyPanel() == thumbnailsBeforeSaveAnnotation;
                bool step5_2 = true;
                if (BasePage.SBrowserName.ToLower().Contains("explorer"))
                    step5_2 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                else
                    step5_2 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                if (step5 && step5_1 && !step5_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step6  From the first series in the first viewport, apply the following W/L to the first image.
                IWebElement ele = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewport);
                viewer.SelectViewerTool(BluRingTools.Window_Level);
                viewer.ApplyTool_WindowWidth();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps+1, 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], ele, 1);

                //To ensure series view scope
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(ele, "down");
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps+1, 2);
                bool step6_1 = studies.CompareImage(result.steps[ExecutedSteps], ele, 2, 1);
                if (step6 && step6_1)
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

                //step7 Click Save Annotated Image from that viewport's Toolbox.
                bool step7 = viewer.SavePresentationState(BluRingTools.Save_Annotated_Image);
                bool step7_1 = false;
                if (BasePage.SBrowserName.ToLower().Contains("explorer"))
                    step7_1 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                else
                    step7_1 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                bool step7_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count == viewportsBeforeSaveAnnotation;
                bool step7_3 = BluRingViewer.NumberOfThumbnailsInStudyPanel() == thumbnailsBeforeSaveAnnotation;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.
                                    GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewport));
                if (!step7 && step7_1 && step7_2 && step7_3 && step7_4)
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

                //step8 Hover the mouse over the warning icon in the Study Panel titlebar.
                String step8_1 = viewer.GetElement("cssselector", BluRingViewer.div_PRStatusIndicator).GetAttribute("title");
                if (step8_1.Trim() == "No annotations in the series.")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step9 Close the Enterprise Viewer, and re-load the same study (in step 2). Verify the study has not been modified.
                viewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bool step9_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count == viewportsBeforeSaveAnnotation;
                bool step9_2 = BluRingViewer.NumberOfThumbnailsInStudyPanel() == thumbnailsBeforeSaveAnnotation;
                bool step9_3 = true;
                if (BasePage.SBrowserName.ToLower().Contains("explorer"))
                    step9_3 = viewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                else
                    step9_3 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_PRStatusIndicator));
                if (step9_1 && step9_2 && !step9_3)
                {
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
                login.Logout();  

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                login.Logout();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                if (isDefaultTool)
                {
                    login.LoginIConnect("Administrator", "Administrator");
                    DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                    domainmanagement.SearchDomain("SuperAdminGroup");
                    domainmanagement.SelectDomain("SuperAdminGroup");
                    domainmanagement.ClickEditDomain();
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                              (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                    Modality.SelectByText("default");
                    Thread.Sleep(1000);
                    IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                    if (revertButton.Enabled)
                        revertButton.Click();
                    domainmanagement.ClickSaveEditDomain();
                    login.Logout();
                }
                try
                {
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");                  
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }               
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
            }
        }

        /// <summary>
        /// Domain Management - Enable Saving GSPS and Toolbox Config
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_164459(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string[] PatientID = null;
           

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String adminGroup = Config.adminGroupName;
                String adminRole = Config.adminRoleName;
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String[] ToolList = {"Save Series", "Save Annotated Images" };

                //Precondition
                var servicetool = new ServiceTool();
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
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                UserPreferences userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                DomainManagement domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("universalviewer", 0);
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                var ConfiguredTools = login.GetConfiguredToolsInToolBoxConfig();
                if (ConfiguredTools.Contains(ToolList[0]) && ConfiguredTools.Contains(ToolList[1]))
                {
                    var Tools1 = new List<String>();
                    Tools1.Add(ToolList[0]);
                    Tools1.Add(ToolList[1]);
                    domain.RemoveToolsFromConfiguredSection(Tools1);
                    domain.ClickSaveEditDomain();
                }
                else
                {
                    Logger.Instance.InfoLog("Save Series and Save Annotated Images are already in Available section");
                    domain.ClickElement(domain.SaveButton());
                }
                login.Logout();
                servicetool.RestartIISUsingexe();

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                RoleManagement role = login.Navigate<RoleManagement>();
                role.SearchRole(adminRole);
                role.SelectRole(adminRole);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("universalviewer", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //Step-1:Go to the iCA web application and login as Administrator user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step-2:Verify that the Enable Saving GSPS option is available in all of the available domains in the Domain Management tab. 
                //Verify that the "Save Series" and "Save Annotated Images" icons are available the Available Items sections of the domain       
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();
                bool Domain_CB=domain.SaveGspsCB().Displayed;
                bool step2_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[1]);
                bool step2_2 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                   domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[1]);

                if (Domain_CB && step2_1 && !step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();                   
                }
                domain.ClickSaveEditDomain();


                //Step-3:From the Studies tab, open up a study in the Universal Viewer (any study). Verify that the "Save Series" and "Save Annotated Images" icons are not available in the viewport Toolbox
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Accession", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenViewerToolsPOPUp();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_toolboxContainer))); IList<String> totaltools1 = viewer.GetAllToolNamesinViewPort();
                bool ToolsExist = totaltools1.Any(t => t.Equals(ToolList[0])) && totaltools1.Any(t => t.Equals(ToolList[1]));
                if (!ToolsExist)
                {
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

                //Step-4:Edit the SuperAdminGroup domain from Domain management tab then drag and drop the Save Series and Save annotated Images icons from Available items to Toolbox Configuration section
                //Save the domain
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(1)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(2)"));
                var ToolsToBeAdded = new Dictionary<String, IWebElement>();
                ToolsToBeAdded.Add(ToolList[0], group1);
                ToolsToBeAdded.Add(ToolList[1], group2);
                domain.AddToolsToToolbox(ToolsToBeAdded);
                bool step6_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                   domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[1]);
                bool step6_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[1]);
                if (step6_1 && !step6_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step-5:Load any study in Universal viewer from studies tab and ensure that Save Series and Save Annotated Images are available in the viewport after right clicking 
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Accession", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenViewerToolsPOPUp();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                totaltools1 = viewer.GetAllToolNamesinViewPort();
                ToolsExist = totaltools1.Any(t => t.Equals(ToolList[0])) && totaltools1.Any(t => t.Equals(ToolList[1]));
                if (ToolsExist)
                {
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

                //Step-6:Edit the SuperAdminGroup domain from Domain management tab then un-select the Enable Saving GSPS option
                //Save the domain
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("savegsps", 1);
                if (domain.SaveGspsCB().Selected == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step-7:Load any study in Universal viewer from studies tab and ensure that Save Series and Save Annotated Images are not available in the viewport after right clicking
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Accession", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenViewerToolsPOPUp();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                totaltools1 = viewer.GetAllToolNamesinViewPort();
                ToolsExist = totaltools1.Any(t => t.Equals(ToolList[0])) && totaltools1.Any(t => t.Equals(ToolList[1]));
                if (!ToolsExist)
                {
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

                //Step-8:Edit again the SuperAdminGroup domain from Domain management tab then select the Enable Saving GSPS option
                //Save the domain.
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                if (domain.SaveGspsCB().Selected==true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step-9:Load any study in Universal viewer from studies tab and ensure that Save Series and Save Annotated Images are available in the viewport after right clicking
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Accession", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenViewerToolsPOPUp();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                totaltools1 = viewer.GetAllToolNamesinViewPort();
                ToolsExist = totaltools1.Any(t => t.Equals(ToolList[0])) && totaltools1.Any(t => t.Equals(ToolList[1]));
                if (ToolsExist)
                {
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

                //Step-10:Edit the SuperAdminGroup domain from Domain management tab then drag and drop the Save Series and Save annotated Images icons from Toolbox Configuration to Available items section
                //Save the domain
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();              
                var Tools = new List<String>();
                Tools.Add(ToolList[0]);
                Tools.Add(ToolList[1]);
                domain.RemoveToolsFromConfiguredSection(Tools);
                bool step10_1 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                  domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[1]);
                bool step10_2 = domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[1]);
                if (!step10_1 && step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();


                //Step-11:Load any study in Universal viewer from studies tab and ensure that Save Series and Save Annotated Images are not available in the viewport after right clicking
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Accession", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenViewerToolsPOPUp();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                totaltools1 = viewer.GetAllToolNamesinViewPort();
                ToolsExist = totaltools1.Any(t => t.Equals(ToolList[0])) && totaltools1.Any(t => t.Equals(ToolList[1]));
                if (!ToolsExist)
                {
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
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("savegsps", 1);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SelectElement Modality = new SelectElement(BasePage.Driver.FindElement
                                                (By.CssSelector(BasePage.select_toolBoxConfiguration_ModalityDropdown)));
                Modality.SelectByText("default");
                Thread.Sleep(1000);
                IWebElement revertButton = BasePage.Driver.FindElement(By.CssSelector(DomainManagement.btn_revertToDefault));
                if (revertButton.Enabled)
                    revertButton.Click();
                domain.ClickSaveEditDomain();
                login.Logout();
            }

        }

        /// <summary>
        ///  Service Tool - Enable Saving GSPS
        /// </summary>
        /// <returns></returns>
        public TestCaseResult Test_164460(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            BluRingViewer viewer = null;
            string PatientID = null;

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String adminGroup = Config.adminGroupName;
                String adminRole = Config.adminRoleName;
                String[] ToolList = { "Save Series", "Save Annotated Images" };

                //Precondition
                //login.DriverGoTo(login.url);
                //login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                //DomainManagement domain = login.Navigate<DomainManagement>();
                //domain.SearchDomain(Config.adminGroupName);
                //domain.SelectDomain(Config.adminGroupName);
                //domain.ClickEditDomain();
                //domain.SetCheckBoxInEditDomain("savegsps", 1);
                //domain.ClickSaveEditDomain();
                //login.Logout();

                //Step-1:Open the iCA Service Tool on the iCA server.
                var servicetool = new ServiceTool();
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step-2:Unselect Enable Features > Enable Saving GSPS. Click Apply and Restart IIS and Windows Services.
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.DisableSavingGSPS();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step-3:Login to iCA as Administrator and verify that Enable Saving GSPS option is not available in any of the available domains in the Domain Management tab. 
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                DomainManagement domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();
                bool Domain_CB = domain.IsElementPresent(domain.By_GSPScb());//domain.SaveGspsCB().Displayed;
                bool step2_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[1]);
                bool step2_2 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                   domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[1]);

                if (!Domain_CB && !step2_1 && !step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step-4:Ensure that the "Save Series" and "Save Annotated
                //Images" icons are not available in the Toolbox Configurations in any of the available Roles in the Role Management tab.
                RoleManagement role = login.Navigate<RoleManagement>();
                role.SearchRole(adminRole, adminGroup);
                role.SelectRole(adminRole);
                role.ClickEditRole();
                bool step3_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[0]) &&
                               domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[1]);
                bool step3_2 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                   domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[1]);

                if (!step3_1 && !step3_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                role.ClickSaveEditRole();

                //Step-5: Select Enable Saving GSPS under Enable Features then click on Apply and Restart IIS and Windows Services.
                servicetool = new ServiceTool();
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
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step-6:Go to the iCA web application and login as Administrator user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step-7:Login to iCA as Administrator and verify that Enable Saving GSPS option is available in any of the available domains in the Domain Management tab. 
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(adminGroup);
                domain.SelectDomain(adminGroup);
                domain.ClickEditDomain();
                Domain_CB = domain.SaveGspsCB().Displayed;
                step2_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[1]);
                step2_2 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                   domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[1]);

                if (Domain_CB && step2_1 && !step2_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveEditDomain();

                //Step-8:Ensure that the "Save Series" and "Save Annotated
                //Images" icons are available in the Toolbox Configurations in any of the available Roles in the Role Management tab.
                role = login.Navigate<RoleManagement>();
                role.SearchRole(adminRole);
                role.SelectRole(adminRole);
                role.ClickEditRole();
                step3_1 = domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[0]) &&
                               domain.GetAvailableToolsInToolBoxConfig().Contains(ToolList[1]);
                step3_2 = domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[0]) &&
                                   domain.GetConfiguredToolsInToolBoxConfig().Contains(ToolList[1]);

                if (step3_1 && !step3_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                role.ClickSaveEditRole();
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
                servicetool = new ServiceTool();
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
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                domain.ClickSaveEditDomain();
                login.Logout();
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
