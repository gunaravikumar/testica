using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Drawing;
using OpenQA.Selenium.Support.UI;
using Accord.Imaging;
using System.Drawing.Imaging;
using Dicom;
using Dicom.Network;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using Selenium.Scripts.Pages.iConnect;





namespace Selenium.Scripts.Tests
{
    class Z3D_All_Views : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public RoleManagement role { get; set; }
        public UserManagement user { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Web_Uploader webuploader { get; set; }
        public Viewer viewer { get; set; }
        public BluRingViewer BluRing { get; set; }
        public BluRingZ3DViewerPage z3dvp { get; set; }
        public object MouseSimulator { get; private set; }
        public Cursor Cursor { get; private set; }
        public Z3D_All_Views(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();
            role = new RoleManagement();
            user = new UserManagement();
            studyviewer = new StudyViewer();
            viewer = new Viewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            webuploader = new Web_Uploader();
            wpfobject = new WpfObjects();
            z3dvp = new BluRingZ3DViewerPage();
            BluRing = new BluRingViewer();
        }
        public TestCaseResult Test_163253(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();

            BluRingZ3DViewerPage Bluering = new BluRingZ3DViewerPage();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::rom iCA, Load a series in the 3D viewer.Navigate to 3D tab and Click MPR mode from the dropdown.
                login.LoginIConnect(adminUserName, adminPassword);
                bool Layout = Bluering.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (Layout)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";  
                    throw new Exception("Unable to load study successfully");
                }
                //Steps2::Click on the 3D six up view button from the 3D toolbar.
                bool ThreeD6x1 = Bluering.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::3D six up viewing mode should be displayed. ThreeD6_1viewicon
                if (ThreeD6x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Resize the window horizontally (drag the browser to left/right).Note: Window should be in portrait.
                //Before
                SwitchToDefault();
                SwitchToUserHomeFrame();
                IList<IWebElement> Viewport = z3dvp.Viewport();
                Size BeforeNav1 = Viewport[0].Size;
                SwitchToDefault();
                SwitchToUserHomeFrame();
                IWebElement ThumbnailBar = z3dvp.ThumbnailBar();
                Size BeforeThumbnail = ThumbnailBar.Size;
                //After
                Driver.Manage().Window.Size = new Size(750, 950);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Size AfterNav1 = Viewport[0].Size;
                ThumbnailBar = z3dvp.ThumbnailBar();
                Size AfterThumbnail = ThumbnailBar.Size;
                //Verification::Images and Thumbnail bar are resized and any 3 controls will move to the left.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                IList<IWebElement> imageloc = z3dvp.ViewportImgLocation();
                int count = 0;
                for (int i = 0; i < imageloc.Count; i++)
                {
                    if (imageloc[i].Location.X == 333)
                    {
                        count++;
                    }
                }

                if (BeforeNav1.Width > AfterNav1.Width && BeforeNav1.Height > AfterNav1.Height && BeforeThumbnail.Width > AfterThumbnail.Width && count == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Resize the window vertically (drag the browser to Top/Bottom).Note: Window should be in landscape.
                Viewport = z3dvp.Viewport();
                BeforeNav1 = Viewport[0].Size;
                ThumbnailBar = z3dvp.ThumbnailBar();
                BeforeThumbnail = ThumbnailBar.Size;
                //After
                Driver.Manage().Window.Size = new Size(1024, 750);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Viewport = z3dvp.Viewport();
                AfterNav1 = Viewport[0].Size;
                ThumbnailBar = z3dvp.ThumbnailBar();
                AfterThumbnail = ThumbnailBar.Size;
                //Verification::Images and Thumbnail bar are resized and any 3 controls will move to the bottom.
                imageloc = z3dvp.ViewportImgLocation();
                count = 0;
                for (int i = 0; i < imageloc.Count; i++)
                {
                    if (imageloc[5].Location.Y == imageloc[i].Location.Y)
                    {
                        count++;
                    }
                }
                if (BeforeNav1.Width != AfterNav1.Width && BeforeNav1.Height != AfterNav1.Height && BeforeThumbnail.Width != AfterThumbnail.Width && count == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Click on the maximize button from the browser.
                //Before
                Viewport = z3dvp.Viewport();
                BeforeNav1 = Viewport[0].Size;
                ThumbnailBar = z3dvp.ThumbnailBar();
                BeforeThumbnail = ThumbnailBar.Size;
                //After
                Driver.Manage().Window.Maximize();
                Thread.Sleep(7000);
                Viewport = z3dvp.Viewport();
                AfterNav1 = Viewport[0].Size;
                ThumbnailBar = z3dvp.ThumbnailBar();
                AfterThumbnail = ThumbnailBar.Size;
                //Verification::Images and Thumbnails are resized to fit the browser window size.
                if (BeforeNav1.Width < AfterNav1.Width && BeforeNav1.Height < AfterNav1.Height && BeforeThumbnail.Width < AfterThumbnail.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Click on the Restore down button from the browser.
                //Before
                Viewport = z3dvp.Viewport();
                BeforeNav1 = Viewport[0].Size;
                ThumbnailBar = z3dvp.ThumbnailBar();
                BeforeThumbnail = ThumbnailBar.Size;
                //After
                //if(Config.BrowserType.ToLower()=="firefox" || Config.BrowserType.ToLower()=="mozilla")
                //{
                //    Driver.Manage().Window.Size = new Size(1024, 600);
                //}
                //else 
                Driver.Manage().Window.Size = new Size(1024, 600);
                

                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Viewport = z3dvp.Viewport();
                AfterNav1 = Viewport[0].Size;
                ThumbnailBar = z3dvp.ThumbnailBar();
                AfterThumbnail = ThumbnailBar.Size;
                //Verification::Images and Thumbnails are resized to fit the browser window size.
                if (BeforeNav1.Width > AfterNav1.Width && BeforeNav1.Height > AfterNav1.Height && BeforeThumbnail.Width > AfterThumbnail.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 7::Click on the MPR 4:1 view button from the 3D toolbar.
                bool MprViewmode = Bluering.select3dlayout(BluRingZ3DViewerPage.MPR,"y");
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::MPR 4:1 viewing mode should be displayed by default.
                if (MprViewmode)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 8::Double click the Mouse left click on the Image in MPR navigation control 1.
                Viewport = z3dvp.Viewport();
                Actions builder = new Actions(Driver);
                builder.SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                builder.DoubleClick(Viewport[0]).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::MPR navigation control 1 switches to 1x1 layout.
                Size Navigation1 = Viewport[0].Size;
                Size Navigation2 = Viewport[1].Size;
                Size Navigation3 = Viewport[3].Size;
                if (Navigation1.Height != Navigation2.Height && Navigation1.Width != Navigation2.Width && Navigation2.Width == Navigation3.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 9::Resize the window vertically (drag the browser to left/Right).
                Driver.Manage().Window.Size = new Size(750, 950);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::MPR navigation control 1 remains in 1x1 layout Other controls will move to the bottom.
                imageloc = z3dvp.ViewportImgLocation();
                count = 0;
                for (int i = 0; i < imageloc.Count; i++)
                {
                    if (imageloc[i].Size.Width > 200 && imageloc[i].Size.Height > 400)
                    {
                        count++;
                    }
                }
                Navigation1 = Viewport[0].Size;
                Navigation2 = Viewport[1].Size;
                Navigation3 = Viewport[2].Size;
                if (Navigation1.Height != Navigation2.Height && Navigation1.Width != Navigation2.Width && Navigation2.Height == Navigation3.Height && count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Resize the window Horizontally(drag the browser to Top/Bottom).
                Driver.Manage().Window.Size = new Size(1024, 600);
                Thread.Sleep(5000);
                Thread.Sleep(5000);
                //Verification::MPR navigation control 1 remains in 1x1 layout.Other controls will move to the left.
                imageloc = z3dvp.ViewportImgLocation();
                count = 0;
                int Otherimg = 0;
                for (int i = 0; i < imageloc.Count; i++)
                {
                    if (imageloc[i].Location.X == 333)
                    {
                        count++;
                    }
                    else if (imageloc[i].Location.X > 333)
                    {
                        Otherimg++;
                    }
                }
                if (count == 3 && Otherimg == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 11::Repeat 8-10 steps in all other Controls from all the viewing modes
                //Verification::1. 3D 4:1 viewing mode
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                MprViewmode = Bluering.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                MprViewmode = Bluering.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Nav1Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                // Viewport = z3dvp.Viewport();
                IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).DoubleClick(Inavigationone).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Nav2Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Nav3Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Size nav1 = Nav1Element.Size;
                Size nav2 = Nav2Element.Size;
                Size nav3 = Nav3Element.Size;
                Driver.Manage().Window.Size = new Size(750, 950); //Vertically
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Size Vernav1 = Nav1Element.Size;
                Size Vernav2 = Nav2Element.Size;
                Size Vernav3 = Nav3Element.Size;
                Driver.Manage().Window.Size = new Size(1024, 600);//Horizontally
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Size Hornav1 = Nav1Element.Size;
                Size Hornav2 = Nav2Element.Size;
                Size Hornav3 = Nav3Element.Size;
                bool Steps1 = false;
                if (nav1.Width > nav2.Width && nav1.Height > nav2.Height && nav2.Width == nav3.Width && (nav2.Height == nav3.Height || nav2.Height == nav3.Height - 1) && Vernav1.Width > Vernav2.Width && Vernav1.Height > Vernav2.Height &&
                    Hornav1.Width > Hornav2.Width && Hornav1.Height > Hornav2.Height && (Hornav2 == Hornav3 || Hornav2.Height + 1 == Hornav3.Height))
                {
                    Steps1 = true;
                }
                //Verification::2. Six-up viewing mode
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Thread.Sleep(5000);
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Nav1Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                //  Viewport = z3dvp.Viewport();
                IWebElement Sixup = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).DoubleClick(Sixup).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Nav2Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Nav3Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                nav1 = Nav1Element.Size;
                nav2 = Nav2Element.Size;
                nav3 = Nav3Element.Size;
                Driver.Manage().Window.Size = new Size(750, 950); //Vertically
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Vernav1 = Nav1Element.Size;
                Vernav2 = Nav2Element.Size;
                Vernav3 = Nav3Element.Size;
                Driver.Manage().Window.Size = new Size(1024, 600);//Horizontally
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Hornav1 = Nav1Element.Size;
                Hornav2 = Nav2Element.Size;
                Hornav3 = Nav3Element.Size;
                bool Steps2 = false;
                if (nav1.Width > nav2.Width && nav1.Height > nav2.Height && Vernav1.Width > Vernav2.Width && Vernav1.Height > Vernav2.Height && Vernav2.Height == Vernav3.Height && (Vernav2.Width == Vernav3.Width || Vernav2.Width == Vernav3.Width - 1) &&
                    Hornav1.Width > Hornav2.Width && Hornav1.Height > Hornav2.Height)
                {
                    Steps2 = true;
                }
                //Verification::3. Curved MPR viewing mode.
                Bluering.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(10);
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Bluering.select3DTools(Z3DTools.Window_Level);
                Nav1Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                Viewport = z3dvp.Viewport();
                new Actions(Driver).DoubleClick(Nav1Element).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Nav2Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Nav3Element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                nav1 = Nav1Element.Size;
                nav2 = Nav2Element.Size;
                nav3 = Nav3Element.Size;
                Driver.Manage().Window.Size = new Size(750, 950); //Vertically
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Vernav1 = Nav1Element.Size;
                Vernav2 = Nav2Element.Size;
                Vernav3 = Nav3Element.Size;
                Driver.Manage().Window.Size = new Size(1024, 600);//Horizontally
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Hornav1 = Nav1Element.Size;
                Hornav2 = Nav2Element.Size;
                Hornav3 = Nav3Element.Size;
                bool Steps3 = false;
                if (nav1.Width > nav2.Width && nav1.Height > nav2.Height && Vernav1.Width > Vernav2.Width && Vernav1.Height > Vernav2.Height && (Vernav2.Width == Vernav3.Width || Vernav2.Width == Vernav3.Width - 1) && Vernav2.Height == Vernav3.Height &&
                    Hornav1.Width > Hornav2.Width && Hornav1.Height > Hornav2.Height)
                {
                    Steps3 = true;
                }
                if (Steps1 && Steps2 && Steps3)
                {
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Bluering.CloseViewer();
                Driver.Manage().Window.Maximize();
                login.Logout();
                //Driver.Close();
            }
        }

        public TestCaseResult Test_163254(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage Bluering = new BluRingZ3DViewerPage();
            BluRingViewer bluringviewer = new BluRingViewer();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::From iCA, Load a series in the Z3D viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                login.ClearFields();
                login.SearchStudy("patient", PatientID);
                PageLoadWait.WaitForPageLoad(40);
                login.SelectStudy("Patient ID", PatientID);
                BluRingViewer.LaunchBluRingViewer(fieldname: "Patient ID", value: PatientID);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForPageLoad(40);
                IWebElement Viewport = Driver.FindElement(By.CssSelector(BluRing.GetViewportCss(1, 0)));
                BluRing.UserSettings("select", "UI - LARGE");
                string PatientIDandName = z3dvp.ReadPatientDetailsUsingTesseract(Viewport, 4, 0, 0, 400, 400);
                string[] AllPatientDetail = PatientIDandName.Split('\n');
                AllPatientDetail = AllPatientDetail.Select(l => String.Join(" ", l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
                AllPatientDetail = AllPatientDetail.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string TwoDPatientNames = "THREED, ALIENHEAD";//AllPatientDetail[0];
                string TwoDPatientIDs = "66677723";// AllPatientDetail[1];
                string TwoDPatientDOBandTime = "23-Jun-13";// AllPatientDetail[3];
                string[] AllPatientDObDetails = TwoDPatientDOBandTime.Split(' ');
                string[] AllPatientDobFormat = AllPatientDObDetails[0].Split('-');
                string TwoDPatientDOBS = AllPatientDobFormat[0] + "-" + AllPatientDobFormat[1] + "";
                string ZoomValue = z3dvp.ReadPatientDetailsUsingTesseract(Viewport, 1, 1000, 1000, 400, 400);
                string[] TwoDPatientWLDetails = ZoomValue.Split('\n');
                TwoDPatientWLDetails = TwoDPatientWLDetails.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string[] TwoDPatientWLValue = TwoDPatientWLDetails[2].Split(':');
                string[] TwoDPatientWWvalue = TwoDPatientWLValue[1].Split('W');
                string windowlevel = TwoDPatientWLValue[2].Trim();
                string windowwidth = TwoDPatientWWvalue[0].Remove(2, 1).Trim();

                string TwoDPatientWLS = windowlevel + " /" + windowwidth + "";
                z3dvp.selectthumbnail("Date:23-Jun-2013");
                bool LaunchMpr = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (LaunchMpr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Steps 2::Compare the annotations of the image in the screenshot with the image in the control of the Z3D viewer.
                IWebElement NavigationElement = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                string LeftTopText = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                string RightTopText = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightTop)).Text;
                //Verification::The information's are matching.Information's are Patient name,patient ID, W/L, DOB etc
                if (LeftTopText.Contains(TwoDPatientWLS) && RightTopText.Contains(TwoDPatientNames) && RightTopText.Contains(TwoDPatientIDs) && RightTopText.Contains(TwoDPatientDOBS))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 3::Check the annotations on all the controls from all viewing modes.
                //Mpr All Control
                String LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                string[] LeftMprNav1WL = LeftMprNav1.Split('m');
                String RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                String MprNav1 = LeftMprNav1WL[2] + "" + RightMprNav1;
                String LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                string[] LeftMprNav2WL = LeftMprNav2.Split('m');
                String RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                String MprNav2 = LeftMprNav2WL[2] + "" + RightMprNav2;
                String LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                string[] LeftMprNav3WL = LeftMprNav3.Split('m');
                String RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                String MprNav3 = LeftMprNav3WL[2] + "" + RightMprNav3;
                String LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                string[] LeftMprResultWL = LeftMprResult.Split('m');
                String RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                String MprResult = LeftMprResultWL[2] + "" + RightMprResult;
                Boolean step3_1 = false;
                if (MprNav1.Equals(MprNav2) && MprNav2.Equals(MprNav3) && MprNav3.Equals(MprResult))
                {
                    step3_1 = true;
                }
                //ThreeD4x1
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                String ThreeD4x1Nav1_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                string[] ThreeD4x1Nav1WL = ThreeD4x1Nav1_L.Split('m');
                String ThreeD4x1Nav1_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                String ThreeD4x1Nav1 = ThreeD4x1Nav1WL[2] + "" + ThreeD4x1Nav1_R;
                String ThreeD4x1Nav2_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                string[] ThreeD4x1Nav2WL = ThreeD4x1Nav2_L.Split('m');
                String ThreeD4x1Nav2_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                String ThreeD4x1Nav2 = ThreeD4x1Nav2WL[2] + "" + ThreeD4x1Nav2_R;
                String ThreeD4x1Nav3_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                string[] ThreeD4x1Nav3WL = ThreeD4x1Nav3_L.Split('m');
                String ThreeD4x1Nav3_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                String ThreeD4x1Nav3 = ThreeD4x1Nav3WL[2] + "" + ThreeD4x1Nav3_R;
                String ThreeD4x1_L = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                string[] ThreeD4x1WL = ThreeD4x1_L.Split('m');
                String ThreeD4x1_R = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                String ThreeD4x1 = ThreeD4x1WL[2] + "" + ThreeD4x1_R;
                Boolean step3_2 = false;
                if (ThreeD4x1Nav1.Equals(ThreeD4x1Nav2) && ThreeD4x1Nav2.Equals(ThreeD4x1Nav3) && ThreeD4x1Nav3.Equals(ThreeD4x1))
                {
                    step3_2 = true;
                }
                //ThreeD6x1
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                String ThreeD6x1Nav1_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                string[] ThreeD6x1Nav1WL = ThreeD6x1Nav1_L.Split('m');
                String ThreeD6x1Nav1_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                String ThreeD6x1Nav1 = ThreeD6x1Nav1WL[2] + "" + ThreeD6x1Nav1_R;
                String ThreeD6x1Nav2_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                string[] ThreeD6x1Nav2WL = ThreeD6x1Nav2_L.Split('m');
                String ThreeD6x1Nav2_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                String ThreeD6x1Nav2 = ThreeD6x1Nav2WL[2] + "" + ThreeD6x1Nav2_R;
                String ThreeD6x1D1_L = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                string[] ThreeD6x1D1WL = ThreeD6x1D1_L.Split('m');
                String ThreeD6x1D1_R = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                String ThreeD6x1D1 = ThreeD6x1D1WL[2] + "" + ThreeD6x1D1_R;
                String ThreeD6x1Nav3_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                string[] ThreeD6x1NavWL = ThreeD6x1Nav3_L.Split('m');
                String ThreeD6x1Nav3_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                String ThreeD6x1Nav3 = ThreeD6x1NavWL[2] + "" + ThreeD6x1Nav3_R;
                String ThreeD6x1Result_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                string[] ThreeD6x1ResultWL = ThreeD6x1Result_L.Split('m');
                String ThreeD6x1Result_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                String ThreeD6x1Result = ThreeD6x1ResultWL[2] + "" + ThreeD6x1Result_R;
                String ThreeD6x1D2_L = z3dvp.GetTopLeftAnnotationValue("3D 2", null);
                string[] ThreeD6x1D2WL = ThreeD6x1D2_L.Split('m');
                String ThreeD6x1D2_R = z3dvp.GetTopRightAnnotationValue("3D 2", null);
                String ThreeD6x1D2 = ThreeD6x1D2WL[2] + "" + ThreeD6x1D2_R;
                Boolean step3_3 = false;
                if (ThreeD6x1Nav1.Equals(ThreeD6x1Nav2) && ThreeD6x1Nav2.Equals(ThreeD6x1D1) && ThreeD6x1D1.Equals(ThreeD6x1Nav3) && ThreeD6x1Nav3.Equals(ThreeD6x1Result) && ThreeD6x1Result.Equals(ThreeD6x1D2))
                {
                    step3_3 = true;
                }
                //Curved Mpr
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                String CurvedMprNav1_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                string[] CurvedMprNav1WL = CurvedMprNav1_L.Split('m');
                String CurvedMprNav1_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                String CurvedMprNav1 = CurvedMprNav1WL[2] + "" + CurvedMprNav1_R;
                String CurvedMprNav2_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                string[] CurvedMprNav2WL = CurvedMprNav2_L.Split('m');
                String CurvedMprNav2_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                String CurvedMprNav2 = CurvedMprNav2WL[2] + "" + CurvedMprNav2_R;
                String ThreeDPathNav_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                string[] ThreeDPathNavWL = ThreeDPathNav_L.Split('m');
                String ThreeDPathNav_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                String ThreeDPathNav = ThreeDPathNavWL[2] + "" + ThreeDPathNav_R;
                String CurvedMprNav3_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                string[] CurvedMprNav3WL = CurvedMprNav3_L.Split('m');
                String CurvedMprNav3_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                String CurvedMprNav3 = CurvedMprNav3WL[2] + "" + CurvedMprNav3_R;
                String MprPathnav_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                string[] MprPathnavWL = MprPathnav_L.Split('m');
                String MprPathnav_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                String MprPathnav = MprPathnavWL[2] + "" + MprPathnav_R;
                String CurvedMpr_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                string[] CurvedMprWL = CurvedMpr_L.Split('m');
                String CurvedMpr_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                String CurvedMpr = CurvedMprWL[2] + "" + CurvedMpr_R;
                Boolean step3_4 = false;
                if (CurvedMprNav1.Equals(CurvedMprNav2) && CurvedMprNav2.Equals(ThreeDPathNav) && ThreeDPathNav.Equals(CurvedMprNav3) && CurvedMprNav3.Equals(MprPathnav) && MprPathnav.Equals(CurvedMpr))
                {
                    step3_4 = true;
                }
                //Calcium Scoring
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                z3dvp.checkerrormsg("y");
                String CalciumScoring_L = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                string[] CalciumScoringWL = CalciumScoring_L.Split('m');
                String CalciumScoring_R = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                String CalciumScoring = CalciumScoringWL[3] + "" + CalciumScoring_R;
                Boolean step3_5 = false;
                if (CalciumScoring.Equals(CurvedMpr))
                {
                    step3_5 = true;
                }
                if (step3_1 && step3_2 && step3_3 && step3_4 && step3_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 4::Check the Patient DOB and study date format displayed in DICOM image annotations in all the controls from all viewing modes.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR,"y");
                bool nav1Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                bool nav2Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                bool nav3Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                bool navResultDate = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                bool ThreeD4x1nav1Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                bool ThreeD4x1nav2Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                bool ThreeD4x1nav3Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                bool ThreeD4x13D1Date = z3dvp.VerifyDateFormat("3D 1");
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                bool ThreeD6x1nav1Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                bool ThreeD6x1nav2Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                bool ThreeD6x13D1Date = z3dvp.VerifyDateFormat("3D 1");
                bool ThreeD6x1nav3Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                bool ThreeD6x1ResultDate = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.ResultPanel);
                bool ThreeD6x13D2Date = z3dvp.VerifyDateFormat("3D 2");
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                bool Curvednav1Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                bool Curvednav2Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                bool Curved3D1Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage._3DPathNavigation);
                bool Curvednav3Date = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                bool CurvedResultDate = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.MPRPathNavigation);
                bool CurvedMprDate = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.CurvedMPR);
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                bool clciumScoringDate = z3dvp.VerifyDateFormat(BluRingZ3DViewerPage.CalciumScoring);

                Logger.Instance.InfoLog("" + nav1Date + nav2Date + nav3Date + navResultDate + ThreeD4x1nav1Date + ThreeD4x1nav2Date + ThreeD4x1nav3Date + ThreeD4x13D1Date +
                    ThreeD6x1nav1Date + ThreeD6x1nav2Date + ThreeD6x13D1Date + ThreeD6x1nav3Date + ThreeD6x1ResultDate + ThreeD6x13D2Date +
                    Curvednav1Date + Curvednav2Date + Curved3D1Date + Curvednav3Date + CurvedResultDate + CurvedMprDate + clciumScoringDate);
                if (nav1Date && nav2Date && nav3Date && navResultDate && ThreeD4x1nav1Date && ThreeD4x1nav2Date && ThreeD4x1nav3Date && ThreeD4x13D1Date &&
                    ThreeD6x1nav1Date && ThreeD6x1nav2Date && ThreeD6x13D1Date && ThreeD6x1nav3Date && ThreeD6x1ResultDate && ThreeD6x13D2Date &&
                    Curvednav1Date && Curvednav2Date && Curved3D1Date && Curvednav3Date && CurvedResultDate && CurvedMprDate && clciumScoringDate)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Press ‘T’ key from the keyboard.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);

                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                bool orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && RightMprNav3.Equals("") && RightMprResult.Equals("") && orientation
                    && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Again Press ‘T’ key from the keyboard.

                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !RightMprNav3.Equals("") && !RightMprResult.Equals("") && orientation
                  && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 7::Click on the Settings button from the 3D toolbar>3D setting.
                bool SettingDialog = z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: false);
                if (SettingDialog)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 8::Uncheck the display annotations option and click the SAVE button.
                //Verification::Settings window is closed. DICOM annotations are toggled off.Orientation markers should not toggle off.
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && RightMprNav3.Equals("") && RightMprResult.Equals("") && orientation
                  && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 9::Again Click on the Settings button from the 3D toolbar> 3D settings.
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: true);
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 10::Settings window is closed.DICOM annotations are toggled on.Orientation markers should not toggle off.
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !RightMprNav3.Equals("") && !RightMprResult.Equals("") && orientation
                  && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps Added
                //Step 11 :: From the global tool bar, Click on the SHOW/HIDE option. Select HIDE image text.
                bool HideText =  bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.HideText);
                //Verification ::DICOM annotations are toggled off.Orientation markers should not toggle off. 
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && RightMprNav3.Equals("") && RightMprResult.Equals("") && orientation
                  && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 :: From the global tool bar, Click on the SHOW/HIDE option. Select SHOW image text.
                bool ShowText = bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.ShowText);
                //Verification :: DICOM annotations are toggled on.Orientation markers should not toggle off.
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !RightMprNav3.Equals("") && !RightMprResult.Equals("") && orientation
                  && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 13::Repeat step 5-9 in all viewing modes.
                //===============================================4x1 Layout====================================================
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);

                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                bool Steps5_1 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                String Left4x13D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && RightMprNav3.Equals("") && RightMprResult.Equals("") && orientation
                   && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && Left4x13D1.Equals("3D 1"))
                {
                    Steps5_1 = true;
                    Logger.Instance.InfoLog("Step 11_1 Pass");
                }

                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                bool Steps6_1 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                Left4x13D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !RightMprNav3.Equals("") && !RightMprResult.Equals("") && orientation
                   && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !Left4x13D1.Equals("3D 1"))
                {
                    Steps6_1 = true;
                    Logger.Instance.InfoLog("Step 11_2 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: false);
                bool Steps8_1 = false;
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                Left4x13D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && RightMprNav3.Equals("") && RightMprResult.Equals("") && orientation
                   && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && Left4x13D1.Equals("3D 1"))
                {
                    Steps8_1 = true;
                    Logger.Instance.InfoLog("Step 11_3 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: true);
                bool Steps9_1 = false;
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                Left4x13D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !RightMprNav3.Equals("") && !RightMprResult.Equals("") && orientation
                   && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !Left4x13D1.Equals("3D 1"))
                {
                    Steps9_1 = true;
                    Logger.Instance.InfoLog("Step 11_4 Pass");
                }
                //===============================================6x1 Layout====================================================
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);

                new Actions(Driver).SendKeys("T").Build().Perform();
                bool Steps5_2 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                string RightMpr3D1 = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                string RightMpr3D2 = z3dvp.GetTopRightAnnotationValue("3D 2", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                String LeftMpr3D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                string leftMpr3D2 = z3dvp.GetTopLeftAnnotationValue("3D 2", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && RightMpr3D1.Equals("") && RightMprNav3.Equals("") && RightMprResult.Equals("") && RightMpr3D2.Equals("") && orientation
                   && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && LeftMpr3D1.Equals("3D 1")
                   && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel) && leftMpr3D2.Equals("3D 2"))
                {
                    Steps5_2 = true;
                    Logger.Instance.InfoLog("Step 11_5 Pass");
                }

                new Actions(Driver).SendKeys("T").Build().Perform();
                bool Steps6_2 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMpr3D1 = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                RightMpr3D2 = z3dvp.GetTopRightAnnotationValue("3D 2", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMpr3D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                leftMpr3D2 = z3dvp.GetTopLeftAnnotationValue("3D 2", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !RightMpr3D1.Equals("") && !RightMprNav3.Equals("") && !RightMprResult.Equals("") && !RightMpr3D2.Equals("") && orientation
                   && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !LeftMpr3D1.Equals("3D 1")
                   && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel) && !leftMpr3D2.Equals("3D 2"))
                {
                    Steps6_2 = true;
                    Logger.Instance.InfoLog("Step 11_6 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: false);
                bool Steps8_2 = false;
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMpr3D1 = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                RightMpr3D2 = z3dvp.GetTopRightAnnotationValue("3D 2", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMpr3D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                leftMpr3D2 = z3dvp.GetTopLeftAnnotationValue("3D 2", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && RightMpr3D1.Equals("") && RightMprNav3.Equals("") && RightMprResult.Equals("") && RightMpr3D2.Equals("") && orientation
                   && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && LeftMpr3D1.Equals("3D 1")
                   && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel) && leftMpr3D2.Equals("3D 2"))
                {
                    Steps8_2 = true;
                    Logger.Instance.InfoLog("Step 11_7 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: true);
                bool Steps9_2 = false;
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                RightMpr3D1 = z3dvp.GetTopRightAnnotationValue("3D 1", null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprResult = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                RightMpr3D2 = z3dvp.GetTopRightAnnotationValue("3D 2", null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                LeftMpr3D1 = z3dvp.GetTopLeftAnnotationValue("3D 1", null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprResult = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, null);
                leftMpr3D2 = z3dvp.GetTopLeftAnnotationValue("3D 2", null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !RightMpr3D1.Equals("") && !RightMprNav3.Equals("") && !RightMprResult.Equals("") && !RightMpr3D2.Equals("") && orientation
                   && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !LeftMpr3D1.Equals("3D 1")
                   && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !LeftMprResult.Equals(BluRingZ3DViewerPage.ResultPanel) && !leftMpr3D2.Equals("3D 2"))
                {
                    Steps9_2 = true;
                    Logger.Instance.InfoLog("Step 11_8 Pass");
                }
                //===============================================CurvedMPR Layout====================================================
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);

                new Actions(Driver).SendKeys("T").Build().Perform();
                bool Steps5_3 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                string Right3DPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                string RightMprPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                string RightCurvedMpr = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);

                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                String Left3DpathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                String LeftMprPathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                String leftCurvedMpr = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && Right3DPathNav.Equals("") && RightMprNav3.Equals("") && RightMprPathNav.Equals("") && RightCurvedMpr.Equals("") && orientation
                  && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && Left3DpathNav.Equals(BluRingZ3DViewerPage._3DPathNavigation)
                  && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && LeftMprPathNav.Equals(BluRingZ3DViewerPage.MPRPathNavigation) && leftCurvedMpr.Equals(BluRingZ3DViewerPage.CurvedMPR))
                {
                    Steps5_3 = true;
                    Logger.Instance.InfoLog("Step 11_9 Pass");
                }

                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                bool Steps6_3 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                Right3DPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                RightCurvedMpr = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                Left3DpathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprPathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                leftCurvedMpr = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !Right3DPathNav.Equals("") && !RightMprNav3.Equals("") && !RightMprPathNav.Equals("") && !RightCurvedMpr.Equals("") && orientation
                  && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !Left3DpathNav.Equals(BluRingZ3DViewerPage._3DPathNavigation)
                  && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !LeftMprPathNav.Equals(BluRingZ3DViewerPage.MPRPathNavigation) && !leftCurvedMpr.Equals(BluRingZ3DViewerPage.CurvedMPR))
                {
                    Steps6_3 = true;
                    Logger.Instance.InfoLog("Step 11_10 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: false);
                bool Steps8_3 = false;
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                Right3DPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                RightCurvedMpr = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                Left3DpathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprPathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                leftCurvedMpr = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightMprNav1.Equals("") && RightMprNav2.Equals("") && Right3DPathNav.Equals("") && RightMprNav3.Equals("") && RightMprPathNav.Equals("") && RightCurvedMpr.Equals("") && orientation
                  && LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && Left3DpathNav.Equals(BluRingZ3DViewerPage._3DPathNavigation)
                  && LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && LeftMprPathNav.Equals(BluRingZ3DViewerPage.MPRPathNavigation) && leftCurvedMpr.Equals(BluRingZ3DViewerPage.CurvedMPR))
                {
                    Steps8_3 = true;
                    Logger.Instance.InfoLog("Step 11_11 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: true);
                bool Steps9_3 = false;
                RightMprNav1 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                RightMprNav2 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                Right3DPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                RightMprNav3 = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                RightMprPathNav = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                RightCurvedMpr = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                LeftMprNav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                LeftMprNav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                Left3DpathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, null);
                LeftMprNav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                LeftMprPathNav = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, null);
                leftCurvedMpr = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (!RightMprNav1.Equals("") && !RightMprNav2.Equals("") && !Right3DPathNav.Equals("") && !RightMprNav3.Equals("") && !RightMprPathNav.Equals("") && !RightCurvedMpr.Equals("") && orientation
                  && !LeftMprNav1.Equals(BluRingZ3DViewerPage.Navigationone) && !LeftMprNav2.Equals(BluRingZ3DViewerPage.Navigationtwo) && !Left3DpathNav.Equals(BluRingZ3DViewerPage._3DPathNavigation)
                  && !LeftMprNav3.Equals(BluRingZ3DViewerPage.Navigationthree) && !LeftMprPathNav.Equals(BluRingZ3DViewerPage.MPRPathNavigation) && !leftCurvedMpr.Equals(BluRingZ3DViewerPage.CurvedMPR))
                {
                    Steps9_3 = true;
                    Logger.Instance.InfoLog("Step 11_12 Pass");
                }
                //===============================================Calcium Scoring Layout====================================================
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                IWebElement CalciumScoringToolBox = z3dvp.CloseSelectedToolBox();
                if (CalciumScoringToolBox.Displayed)
                    CalciumScoringToolBox.Click();

                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                bool Steps5_4 = false;
                PageLoadWait.WaitForFrameLoad(20);
                string RightCalcium = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                string LeftCalcium = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (RightCalcium.Equals("") && LeftCalcium.Equals(BluRingZ3DViewerPage.CalciumScoring) && orientation)
                {
                    Steps5_4 = true;
                    Logger.Instance.InfoLog("Step 11_13 Pass");
                }

                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                bool steps6_4 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightCalcium = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                LeftCalcium = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                //Verification::DICOM annotations are toggled off and Orientation markers should not toggle off.
                if (!RightCalcium.Equals("") && !LeftCalcium.Equals(BluRingZ3DViewerPage.CalciumScoring) && orientation)
                {
                    steps6_4 = true;
                    Logger.Instance.InfoLog("Step 11_14 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: false);
                bool Steps8_4 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightCalcium = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                LeftCalcium = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (RightCalcium.Equals("") && LeftCalcium.Equals(BluRingZ3DViewerPage.CalciumScoring) && orientation)
                {
                    Steps8_4 = true;
                    Logger.Instance.InfoLog("Step 11_15 Pass");
                }
                z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: true);
                bool Steps9_4 = false;
                PageLoadWait.WaitForFrameLoad(20);
                RightCalcium = z3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                LeftCalcium = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring, null);
                orientation = z3dvp.VerifyOrientationInAllControls();
                if (!RightCalcium.Equals("") && !LeftCalcium.Equals(BluRingZ3DViewerPage.CalciumScoring) && orientation)
                {
                    Steps9_4 = true;
                    Logger.Instance.InfoLog("Step 11_16 Pass");
                }
                Logger.Instance.InfoLog("Step number: " + ExecutedSteps.ToString() + Steps5_1 + Steps5_2 + Steps5_3 + Steps5_4 + Steps6_1 + Steps6_2 + Steps6_3 + steps6_4 + Steps8_1
                    + Steps8_2 + Steps8_3 + Steps8_4 + Steps9_1 + Steps9_2 + Steps9_3 + Steps9_4);
                if (Steps5_1 && Steps5_2 && Steps5_3 && Steps5_4 && Steps6_1 && Steps6_2 && Steps6_3 && steps6_4 && Steps8_1
                    && Steps8_2 && Steps8_3 && Steps8_4 && Steps9_1 && Steps9_2 && Steps9_3 && Steps9_4)
                {
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Bluering.CloseViewer();
                login.Logout();
                //Driver.Close();
            }
        }

        public TestCaseResult Test_163255(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage Bluering = new BluRingZ3DViewerPage();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String Nav1Location = "Loc: 0.0, 0.0, 0.0 mm";
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                z3dvp.Deletefiles(testcasefolder);
                //step 01::From iCA, Load a Series in the 3D viewer.Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                login.LoginIConnect(adminUserName, adminPassword);
                bool Result = Bluering.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (Result)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Steps 2::Apply several tool operations to each control in each one of the views in the 3D viewer.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Actions builder = new Actions(Driver);
                builder.SendKeys("x").Perform();
                IList<IWebElement> Viewport = z3dvp.Viewport();
                bool Windowlevel = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Window_Level, 50, 50, 100);
                bool Scrolling = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationtwo, Z3DTools.Scrolling_Tool, 50, 50, 100);
                bool Zoom = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationthree, Z3DTools.Interactive_Zoom, 50, 50, 100);
                bool rotate = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100);
                //Verification::Verify that the modifications are preserved on the image.
                if (Windowlevel && Scrolling && Zoom && rotate)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Steps 3::Click the reset button from 3D toolbox.
                z3dvp.select3DTools(Z3DTools.Reset);
                z3dvp.select3DTools(Z3DTools.Reset);
                //Verification::Images in all views return to their original state.
                String Nav1 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, null);
                bool BeforeN1 = Nav1.Contains(Nav1Location);
                String Nav2 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, null);
                bool BeforeN2 = Nav2.Contains(Nav1Location);
                String Nav3 = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, null);
                bool BeforeN3 = Nav3.Contains(Nav1Location);
                if (BeforeN1 && BeforeN2 && BeforeN3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Repeat steps 1-2 in all the viewing modes.
                bool Step1 = false; bool Step2 = false; bool Step3 = false;
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                {
                    z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4,BluRingZ3DViewerPage.Navigation3D1);
                    //for scrolling 
                    z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigation3D1);
                    List<string> before_scroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    IList<IWebElement> Viewport7 = z3dvp.Viewport();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport7[3].Location.X / 2 + 600), (Viewport7[3].Location.Y / 2 + 400));
                    for (int i = 0; i < 20; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 25, 0);
                        Thread.Sleep(1000);
                    }
                    List<string> after_scroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                   
                    //for rotate
                    List<string> beforerotate = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigation3D1);
                    IWebElement nav3d1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    new Actions(Driver).MoveToElement(nav3d1, nav3d1.Size.Width / 2, nav3d1.Size.Height / 2).ClickAndHold()
                    .MoveToElement(nav3d1, nav3d1.Size.Width / 2, nav3d1.Size.Height / 2 + 100).Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> afterrotate = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    //for reset 
                    bool Reset4d = z3dvp.select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(10);
                    List<string> afater_4d_reset = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (before_scroll[3] != after_scroll[3] && beforerotate[3] != afterrotate[3] && afater_4d_reset[0]== Nav1Location) Step1 = true;
                    else Logger.Instance.ErrorLog("Fail in four view layout ");
                  
                    //Six view layout 
                    z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                    
                    //for window level 
                    IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    List<string> before_window = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.ResultPanel);
                    //new Actions(Driver).SendKeys("x").Release().Build().Perform();Thread.Sleep(1000);
                    z3dvp.Performdragdrop(Inavigationone, 20, 30);
                    Thread.Sleep(10000);
                    List<string> after_window = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                   
                    //for zoom
                    List<string> before_zoom = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationone);
                    z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone);
                    Thread.Sleep(10000);
                    List<string> after_zoom = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    
                    //for pan 
                    List<string> check_beforepan = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    z3dvp.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigationone);
                    z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                    new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width - 10, Inavigationone.Size.Height - 5)
                    .DragAndDropToOffset(Inavigationone, 150, 150).Release().Build().Perform();
                    Thread.Sleep(7000);
                    List<string> check_afterpan = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    
                    //for scroll
                    z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                    IList<string> before_6dscroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    IWebElement ViewerContainer7 = z3dvp.ViewerContainer();
                    Cursor.Position = new Point((ViewerContainer7.Location.X + 250), (ViewerContainer7.Location.Y / 2 + 300));
                    for (int i = 0; i < 20; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 25, 0);
                        Thread.Sleep(1000);
                    }
                    IList<string> after_6dscroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    
                    //for rotation 
                    List<string> before_6drotate = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                    z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                    new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).ClickAndHold()
                    .MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2 + 100).Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> after_6drotate = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                  bool  Reset6d = z3dvp.select3DTools(Z3DTools.Reset);
                    List<string> afater_6d_reset = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (before_window[0] != after_window[0] && before_zoom[0] != after_zoom[0] && check_beforepan[0] != check_afterpan[0] && before_6dscroll[0] != after_6dscroll[0] && before_6drotate[0] != after_6dscroll[0] && afater_6d_reset[0]==Nav1Location)
                    {
                        Step2 = true;
                    }
                    else Logger.Instance.ErrorLog("sixd layout fail");


                    //for curvedmpr
                    z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                    //for line 
                    string beforeline = "step3line_before.bmp";
                    DownloadImageFile(Inavigationone, testcasefolder + Path.DirectorySeparatorChar + beforeline);
                    Thread.Sleep(10000);
                    z3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigationone);
                    z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                    new Actions(Driver).MoveToElement(Inavigationone, (Inavigationone.Size.Width / 2) - 50, (Inavigationone.Size.Height / 2) - 50)
                    .ClickAndHold().MoveToElement(Inavigationone, (Inavigationone.Size.Width / 2) - 50, (Inavigationone.Size.Height / 2) - 100)
                    .Release().Build().Perform();
                    Thread.Sleep(5000);
                    string Afterline = "step3line_after.bmp";
                    DownloadImageFile(Inavigationone, testcasefolder + Path.DirectorySeparatorChar + Afterline);
                    Thread.Sleep(10000);
                    bool bflagline = CompareImage(testcasefolder + beforeline, testcasefolder + Afterline);

                    // for  zoom
                    List<string> before_curzoom = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationone);
                    z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone, scrollvalue:false);
                    Thread.Sleep(10000);
                    List<string> after_curzoom = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                    //for window 
                    List<string> before_curwindow = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    z3dvp.select3DTools(Z3DTools.Window_Level);
                    
                    z3dvp.Performdragdrop(Inavigationone, 20, 30);
                    Thread.Sleep(10000);
                    List<string> after_curwindow = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    //for pan 
                    List<string> check_curbeforepan = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    z3dvp.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigationone);
                    z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                    new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width - 10, Inavigationone.Size.Height - 5)
                    .DragAndDropToOffset(Inavigationone, 150, 150).Release().Build().Perform();
                    Thread.Sleep(7000);
                    List<string> check_curafterpan = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    //for scroll
                    z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                    IList<string> before_curscroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer7.Location.X + 250), (ViewerContainer7.Location.Y / 2 + 300));
                    for (int i = 0; i < 20; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 25, 0);
                        Thread.Sleep(1000);
                    }
                    IList<string> after_curscroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    z3dvp.select3DTools(Z3DTools.Reset);
                    List<string> afater_cur_reset = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    if (bflagline==false && before_curzoom[0] != after_curzoom[0] && before_curwindow[0] != after_curwindow[0] && check_curbeforepan[0] != check_curafterpan[0] && before_curscroll[0] != after_curscroll[0] && afater_cur_reset[0]==Nav1Location)
                    {
                        Step3 = true;
                    }
                    else Logger.Instance.ErrorLog("Curved Mpr steps are fail");

                }
                else
                {
                    z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                    bool Scrolling4x1 = z3dvp.ApplyToolsonViewPort("3D 1", Z3DTools.Scrolling_Tool, 50, 50, 100);
                    bool rotate4x1 = z3dvp.ApplyToolsonViewPort("3D 1", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool Reset = z3dvp.select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(10);
                    string LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                    
                    if (Scrolling4x1 && rotate4x1 && Reset && LocValue.Equals(Nav1Location))
                    {
                        Step1 = true;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("scrolling rotatereset is fail for threed four");
                    }
                    z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                    bool Windowlevel6x1 = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Window_Level, 50, 50, 100);
                    bool Scrolling6x1 = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationtwo, Z3DTools.Scrolling_Tool, 50, 50, 100);
                    bool Zoom6x1 = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationthree, Z3DTools.Interactive_Zoom, 50, 50, 100);
                    bool Pan6x1 = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Pan, 50, 50, 100);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool rotate6x1_3D_1 = z3dvp.ApplyToolsonViewPort("3D 1", Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100);
                    PageLoadWait.WaitForFrameLoad(10);
                    Reset = z3dvp.select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(10);
                    LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                    
                    if (rotate6x1_3D_1 && Pan6x1 && Zoom6x1 && Scrolling6x1 && Windowlevel6x1 && Reset && LocValue.Equals(Nav1Location))
                    {
                        Step2 = true;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("scrolling rotatereset is fail for threeedsix");
                    }
                    z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                    bool CurvedLineMeasurement = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Line_Measurement, 50, 50, 100, testid, ExecutedSteps + 1);
                    bool CurvedNav2WL = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationtwo, Z3DTools.Window_Level, 50, 50, 100);
                    bool CurvedNav3Zoom = z3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationthree, Z3DTools.Interactive_Zoom, 50, 50, 100);
                    PageLoadWait.WaitForFrameLoad(10);
                    Reset = z3dvp.select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);//Because reset is not working some times first time..
                    LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                    
                    if (CurvedNav3Zoom && CurvedNav2WL && CurvedLineMeasurement && Reset && LocValue.Equals(Nav1Location))
                    {
                        Step3 = true;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("scrolling rotatereset is fail curvedMPR");
                    }
                }
                //Verification::Verify that the modifications are preserved on the image.
                if (Step1 && Step2 && Step3)
                {
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Bluering.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163256(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage Bluering = new BluRingZ3DViewerPage();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::From iCA, Load a series in the Z3D viewer.
                login.LoginIConnect(adminUserName, adminPassword);
                bool Layout = Bluering.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (Layout)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Steps 2::check the default tool selected.
                //Verification::Window Level tool is selected by default.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                IList<IWebElement> Viewport = z3dvp.Viewport();
                Actions builder = new Actions(Driver);
                builder.MoveToElement(Viewport[0], (Viewport[0].Size.Width / 7), (Viewport[0].Size.Height / 7)).Click().Build().Perform();
                bool Scrolling = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ScrollingCursor);
                if (Scrolling)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Select the 3D viewing mode from the 3D dropdown menu.
                bool ThreeD4x1 = Bluering.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                Thread.Sleep(4000);
                //Verification::3D 4:1 viewing mode is displayed.
                if (ThreeD4x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Check the default tool selected.
                //Verification::Window Level tool is selected by default.
                builder = new Actions(Driver);
                builder.MoveToElement(Viewport[3], (Viewport[3].Size.Width / 7), (Viewport[3].Size.Height / 7)).Click().Build().Perform();
                Scrolling = z3dvp.VerifyCursorMode("3D 1", BluRingZ3DViewerPage.ScrollingCursor);
                if (Scrolling)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Select the SixUp viewing mode from the 3D dropdown menu.
                bool ThreeD6x1 = Bluering.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                //Verification::Six up viewing mode is displayed.
                if (ThreeD6x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Check the default tool selected.
                //Verification::Window Level tool is selected by default.
                Viewport = z3dvp.Viewport();
                builder = new Actions(Driver);
                builder.MoveToElement(Viewport[4], (Viewport[4].Size.Width / 2), (Viewport[4].Size.Height / 2)).Build().Perform();
                Scrolling = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.ScrollingCursor);
                if (Scrolling)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 7::Select the Curved MPR viewing mode from the 3D dropdown menu.
                bool CurvedMPR = Bluering.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Curved MPR viewing mode is displayed.
                if (CurvedMPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 8::Check the default tool selected.
                //Verification::Curve drawing tool : manual mode should be selected by default.
                Viewport = z3dvp.Viewport();
                builder = new Actions(Driver);
                builder.MoveToElement(Viewport[4], (Viewport[4].Size.Width / 2), (Viewport[4].Size.Height / 2)).Build().Perform();
                bool CurvedMpr = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.CurvedToolManualCursor);
                if (CurvedMpr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 9::Select the Calcium scoring mode from the 3D dropdown menu.
                Bluering.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Calcium Scoring view should be displayed.
                IList<IWebElement> tilelist = z3dvp.controlImage();
                int count = tilelist.Count;
                if (count.Equals(1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Check the default tool selected.
                //Verification::Calcium Scoring tool should be selected by default.
                Viewport = z3dvp.Viewport();
                builder = new Actions(Driver);
                builder.MoveToElement(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2)).Build().Perform();
                bool CalciumScoringg = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.CalciumScoring, BluRingZ3DViewerPage.CalciumScoringCursor);
                PageLoadWait.WaitForFrameLoad(10);
                if (CalciumScoringg)
                {
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Bluering.CloseViewer();
            }
        }

        public TestCaseResult Test_163257(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage Bluering = new BluRingZ3DViewerPage();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::From iCA, Load a study in the 3D viewer Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                login.LoginIConnect(adminUserName, adminPassword);
                bool steps1 = Bluering.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (steps1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }

                SwitchToDefault();
                SwitchToUserHomeFrame();
                IList<IWebElement> Viewport = z3dvp.Viewport();
                //Steps 2::Select each tool one by one from the 3D toolbox, Then place the mouse cursor on the image in the MPR control.
                //Steps 3::Apply the tool operation on the image in the controls.
                IWebElement iNavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool apply3result = z3dvp.ToolApplyandcheck_163257(iNavigationtwo, BluRingZ3DViewerPage.MPR);
                if (apply3result)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Place the mouse cursor on the center of the cross hair in MPR navigation controls.
                IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool Nav_one = false;
                if (Config.BrowserType.ToLower() == "chrome")
                {
                    Nav_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                }
                else
                {
                    Actions act1 = new Actions(Driver);
                    act1.MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Build().Perform();
                }
                Actions builder = new Actions(Driver);
                PageLoadWait.WaitForFrameLoad(10);
                // IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //  bool CrossHairCursor = Inavigationone.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                bool CrossHairCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CrossHairCursor);
                if (CrossHairCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 5::Place the mouse cursor on the Rotate hot spot in MPR navigation controls.
                z3dvp.MoveMouseCursorOnRotateHotspot(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(30);
                bool AfterMovingRotateCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RotateCursor);
                //  bool AfterMovingRotateCursor = Inavigationone.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.RotateCursor);
                if (AfterMovingRotateCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Navigate to 3D 4:1 viewing mode from 3D dropdown menu.
                bool ThreeD4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (ThreeD4x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 7::Select each tool one by one from the 3D toolbox, Then place the mouse cursor on the image in the 3D controls.
                //Selected tool Cursor should appear on hovering the cursor over the image.
                //Steps 8::Apply the tool operation on the image in the controls.
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement iWebElement3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                bool apply8result = z3dvp.ToolApplyandcheck_163257(iWebElement3D1, BluRingZ3DViewerPage.Three_3d_4);
                if (apply8result)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 9::Place the mouse cursor inside the bounding box of the 3D navigation controls.
                PageLoadWait.WaitForFrameLoad(10);
                // IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (Config.BrowserType.ToLower() != "firefox")
                    builder.MoveToElement(Inavigationone, (Inavigationone.Size.Width / 2), (Inavigationone.Size.Height / 2)).Click().Build().Perform();
                else
                    builder.MoveToElement(Inavigationone, (Inavigationone.Size.Width / 2), (Inavigationone.Size.Height / 2)).Build().Perform();
                //Verification::Clipping line adjustment cursor should be displayed on hovering the mouse cursor inside the bounding box.
                PageLoadWait.WaitForFrameLoad(10);
                bool ClippingLineCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ClippingCursor);
                //bool ClippingLineCursor = Inavigationone.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.ClippingCursor);
                Thread.Sleep(2000);
                if (ClippingLineCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Place the mouse cursor on the Rotate hot spot in 3D1 control.
                IWebElement iNavigation3d1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
               // if (Config.BrowserType.ToLower() != "firefox")
                    // builder.MoveToElement(Viewport[3], (iNavigation3d1.Size.Width / 3), (iNavigation3d1.Size.Height / 2)).Click().Build().Perform();
                    z3dvp.MoveMouseCursorOnRotateHotspot(BluRingZ3DViewerPage.Navigation3D1);
              //  else
                  //  builder.MoveToElement(Viewport[3], (iNavigation3d1.Size.Width / 3), (iNavigation3d1.Size.Height / 2)).Build().Perform();

               // PageLoadWait.WaitForFrameLoad(10);
                // AfterMovingRotateCursor = iWebElement3D1.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.RotateCursor);
                AfterMovingRotateCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.RotateCursor);
                if (AfterMovingRotateCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 11::Navigate to Six up viewing mode from 3D dropdown.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Viewport = z3dvp.Viewport();
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 12::Select each tool one by one from the 3D toolbox, Then place the mouse cursor on the image in the MPR and 3D controls.
                //Steps 13::Apply the tool operation on the image in the controls.
                IWebElement Inavigationthree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                bool apply13result = z3dvp.ToolApplyandcheck_163257(Inavigationthree, BluRingZ3DViewerPage.Three_3d_6);
                if (apply13result)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 14::Place the mouse cursor on the Rotate hot spot in MPR navigation controls.
                z3dvp.MoveMouseCursorOnRotateHotspot(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                //AfterMovingRotateCursor = Inavigationone.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.RotateCursor);
                AfterMovingRotateCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RotateCursor);
                if (AfterMovingRotateCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 15::Place the mouse cursor on the center of the cross hair in MPR navigation controls.
                Nav_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                //  CrossHairCursor = Inavigationone.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                CrossHairCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CrossHairCursor);
                if (CrossHairCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 16::Place the mouse cursor on the Rotate hot spot in 3D1 and 3D 2 controls.
                IWebElement Inavigation3d2_6 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                IWebElement Inavigation3d1_6 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //if (Config.BrowserType.ToLower() == "chrome")

                //    new Actions(Driver).MoveToElement(iNavigation3d1, (Inavigation3d1_6.Size.Width / 5), (Inavigation3d1_6.Size.Height / 2)).Click().Build().Perform();
                //else
                //    new Actions(Driver).MoveToElement(Inavigation3d1_6, (Inavigation3d1_6.Size.Width / 5), (Inavigation3d1_6.Size.Height / 2)).Build().Perform();
                z3dvp.MoveMouseCursorOnRotateHotspot(BluRingZ3DViewerPage.Navigation3D1);
              //  PageLoadWait.WaitForFrameLoad(10);
                //bool ThreeD1RotateCursor = iWebElement3D1.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.RotateCursor);
                bool ThreeD1RotateCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.RotateCursor);
                Thread.Sleep(2000);
                //if (Config.BrowserType.ToLower() == "chrome")
                //    new Actions(Driver).MoveToElement(Inavigation3d2_6, (Inavigation3d2_6.Size.Width / 5), (Inavigation3d2_6.Size.Height / 2)).Click().Build().Perform();
                //else
                //    new Actions(Driver).MoveToElement(Inavigation3d2_6, (Inavigation3d2_6.Size.Width / 5), (Inavigation3d2_6.Size.Height / 2)).Build().Perform();
                //PageLoadWait.WaitForFrameLoad(10);
                z3dvp.MoveMouseCursorOnRotateHotspot(BluRingZ3DViewerPage.Navigation3D2);
               // IWebElement Iweb3d2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                //  bool ThreeD2RotateCursor = Iweb3d2.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.RotateCursor);
                bool ThreeD2RotateCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.RotateCursor);
                Thread.Sleep(2000);
                if (ThreeD1RotateCursor && ThreeD2RotateCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 17::Navigate to Curved MPR viewing mode from 3D dropdown menu.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                Viewport = z3dvp.Viewport();
                result.steps[++ExecutedSteps].status = "Pass";

                //Steps 18::Select each tool one by one from the 3D toolbox, Then place the mouse cursor on the image in the MPR and 3D controls.
                //Steps 19::Apply the tool operation on the image in the controls.
                bool apply19result = z3dvp.ToolApplyandcheck_163257(Inavigationthree, BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(2000);
                //     new Actions(Driver).SendKeys("X").Release().Build().Perform();
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                IWebElement Inavigationonecur = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);

                new Actions(Driver).MoveToElement(Inavigationonecur, Inavigationonecur.Size.Width / 2 + 10, Inavigationonecur.Size.Height / 2 - 10).Click().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Inavigationonecur, Inavigationonecur.Size.Width / 2, Inavigationonecur.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Inavigationonecur, Inavigationonecur.Size.Width / 2 - 100, Inavigationonecur.Size.Height - 100).Build().Perform();
                Thread.Sleep(1000);
                bool CurvedDrawing1 = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CurvedToolManualCursor);

                if (CurvedDrawing1 == false)
                {
                    Thread.Sleep(5000);
                    CurvedDrawing1 = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CurvedToolManualCursor);
                }
                Thread.Sleep(2000);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                z3dvp.select3DTools(Z3DTools.Reset);
                if (apply19result && CurvedDrawing1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 20::Place the mouse cursor on the center of the cross hair in MPR navigation controls and verify.
                // Nav_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Inavigationone, Inavigationone.Size.Width / 2, Inavigationone.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                //    CrossHairCursor = Inavigationone.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                CrossHairCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CrossHairCursor);
                Thread.Sleep(2000);
                if (CrossHairCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 21::Place the mouse cursor on the Rotate hot spot in MPR navigation controls.
                z3dvp.MoveMouseCursorOnRotateHotspot(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(20);
                //   AfterMovingRotateCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RotateCursor);
                AfterMovingRotateCursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RotateCursor);
                if (AfterMovingRotateCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 22::Navigate to calcium scoring mode from 3D dropdown menu.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                z3dvp.checkerrormsg("y");
                Viewport = z3dvp.Viewport();
                result.steps[++ExecutedSteps].status = "Pass";

                if (z3dvp.CloseSelectedToolBox().Displayed)
                    z3dvp.CloseSelectedToolBox().Click();
                //Steps 23::Select each tool one by one from the 3D toolbox, Then place the mouse cursor on the image in the MPR and 3D controls.
                //Steps 24::Apply the tools operation on the image in the controls.
                //calcium
                IWebElement Icalciumscoring = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Actions act = new Actions(Driver);
                IList<IWebElement> AllViewport = z3dvp.controlImage();
                act.MoveToElement(Icalciumscoring, Icalciumscoring.Size.Width / 2, Icalciumscoring.Size.Height - 30).ClickAndHold().
                MoveToElement(Icalciumscoring, Icalciumscoring.Size.Width / 2 - 20, Icalciumscoring.Size.Height / 2 - 20).Build().Perform();
                Thread.Sleep(1000);
                act.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool iCalcursor = false;
                //  iCalcursor = Icalciumscoring.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CalciumScoringCursor);
                iCalcursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.CalciumScoring, BluRingZ3DViewerPage.CalciumScoringCursor);
                Thread.Sleep(1000);
                try
                {
                    IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                    Thread.Sleep(1000);
                    CloseSelectedToolBox.Click();
                }
                catch (Exception e) { }
                //scrolling tool
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.CalciumScoring);
                int j = 50;
                new Actions(Driver).MoveToElement(Icalciumscoring, Icalciumscoring.Size.Width / 2, Icalciumscoring.Size.Height / 4)
                .ClickAndHold().MoveToElement(Icalciumscoring, Icalciumscoring.Size.Width / 2, Icalciumscoring.Size.Height / 2 + j++)
                .Release().Build().Perform();
                Thread.Sleep(2000);
                //  bool    bcalciumscroll = Icalciumscoring.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.ScrollingCursor);
                bool bcalciumscroll = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.CalciumScoring, BluRingZ3DViewerPage.ScrollingCursor);
                Thread.Sleep(1000);
                //window level 
                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.CalciumScoring);
                z3dvp.Performdragdrop(Icalciumscoring, 20, 30);
                Thread.Sleep(2000);
                bool bwindowlevel = false;
                
                bwindowlevel = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.CalciumScoring, BluRingZ3DViewerPage.WindowLevelCursor);
                Thread.Sleep(2000);

                //download Image 
                bool bDownload_image = false;
                z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.CalciumScoring);
                new Actions(Driver).MoveToElement(Icalciumscoring, Icalciumscoring.Size.Width / 2 - 20, Icalciumscoring.Size.Height - 100).Click().Release().Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IList<IWebElement> ItoolJPGPNG = z3dvp.DownloadJPGPNG();
                if (ItoolJPGPNG.Count >= 1 && ItoolJPGPNG[0].Text.ToUpper() == "JPG" && ItoolJPGPNG[1].Text.ToUpper() == "PNG")
                {
                    
                    bDownload_image = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.CalciumScoring, BluRingZ3DViewerPage.DownloadCursor);
                    Thread.Sleep(1000);
                    try
                    {
                        IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                        Thread.Sleep(1000);
                        new Actions(Driver).MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                        Thread.Sleep(2000);
                        CloseSelectedToolBox.Click();
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog(e.Message);
                    }
                }
                if (iCalcursor && bwindowlevel && bcalciumscroll && bDownload_image)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Bluering.CloseViewer();
                login.Logout();
                //Driver.Close();
            }
        }

        public TestCaseResult Test_163258(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage Bluering = new BluRingZ3DViewerPage();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::From iCA, Load the study in 3D viewer. Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                login.LoginIConnect(adminUserName, adminPassword);
                bool Steps1 = Bluering.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (Steps1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                SwitchToDefault();
                SwitchToUserHomeFrame();
                IList<IWebElement> Viewport = z3dvp.Viewport();
                //Steps 2::Adjust the size of the Z3D browser window to have a greater width than height.	
                Driver.Manage().Window.Size = new Size(950, 750);
                //Verification::3D viewer in MPR 4:1 viewing mode is resized.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Viewport = z3dvp.Viewport();
                Size GetSize = Driver.Manage().Window.Size;
                if (Viewport.Count == 4 && GetSize.Height < GetSize.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Resize the browser so that its height is greater than its width. 
                Driver.Manage().Window.Size = new Size(750, 950);
                //Verification::3D view switches from landscape to portrait orientation. 
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Viewport = z3dvp.Viewport();
                Size windowSize = Driver.Manage().Window.Size;
                if (Viewport.Count == 4 && windowSize.Width.Equals(750) && windowSize.Height.Equals(950))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Resize the browser so that its width is greater than its height.
                Driver.Manage().Window.Size = new Size(950, 750);
                //Verification::3D view switches back from portrait to landscape orientation. 
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Viewport = z3dvp.Viewport();
                windowSize = Driver.Manage().Window.Size;
                if (Viewport.Count == 4 && windowSize.Width.Equals(950) && windowSize.Height.Equals(750))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Go to the SixUp view by clicking the SixUp button from 3D dropdown menu
                Bluering.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                //Verification::SixUp view shows up in landscape orientation. 
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Viewport = z3dvp.Viewport();
                windowSize = Driver.Manage().Window.Size;
                Thread.Sleep(5000);
                if (Viewport.Count == 6 && windowSize.Width.Equals(950) && windowSize.Height.Equals(750))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Resize the browser so that its height is greater than its width.
                Driver.Manage().Window.Size = new Size(750, 950);
                //Verification::SixUp view switches from landscape to portrait orientation.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Viewport = z3dvp.Viewport();
                windowSize = Driver.Manage().Window.Size;
                Thread.Sleep(5000);
                if (Viewport.Count == 6 && windowSize.Width.Equals(750) && windowSize.Height.Equals(950))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 7::Resize the browser so that its width is greater than its height.
                Driver.Manage().Window.Size = new Size(950, 750);
                //Verification::SixUp view switches from portrait to landscape orientation.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Viewport = z3dvp.Viewport();
                windowSize = Driver.Manage().Window.Size;
                Thread.Sleep(5000);
                if (Viewport.Count == 6 && windowSize.Width.Equals(950) && windowSize.Height.Equals(750))
                {
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Bluering.CloseViewer();
                Driver.Manage().Window.Maximize();
                login.Logout();
                //Driver.Close();
            }
        }

        public TestCaseResult Test_163259(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage Blueringz3D = new BluRingZ3DViewerPage();
            BluRingViewer Bluring = new BluRingViewer();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::From iCA, Load a study in 3D viewer.Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                login.LoginIConnect(adminUserName, adminPassword);
                bool Steps1 = Blueringz3D.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (Steps1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Steps 2::Hover over each control.
                bool MprViewport = z3dvp.VerifyHighLightedBorder();
                //Verification::When the user hovers over each control border of each control thickens.
                if (MprViewport)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Repeat steps 2 for all controls in all views.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                bool THreeD4x1 = z3dvp.VerifyHighLightedBorder();
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                bool THreeD6x1 = z3dvp.VerifyHighLightedBorder();
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                bool CurvedMpr = z3dvp.VerifyHighLightedBorder();
                //Verification::Same expected results should be observed.
                if (THreeD4x1 && THreeD6x1 && CurvedMpr)
                {
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
            }
        }

        public TestCaseResult Test_163260(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            string licensefilepath = Config.licensefilepath;

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::Login iCA as Administrator.
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 2::Make sure that the 3D viewer window is on landscape mode, the length of the window is > than the height of the window.
                Driver.Manage().Window.Size = new Size(950, 750);
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 3::search and load a study in iCA viewer.
                bool MprControl = z3dvp.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.MPR);
                if (MprControl)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    throw new Exception("Unable to load study successfully");
                }
                //Steps 4::Navigate to 3D tab and Click MPR mode from the dropdown.Note: This is new design(could change)
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (MprControl)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Double click on navigation control 1.
                IWebElement navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Actions builder = new Actions(Driver);
                new Actions(Driver).MoveToElement(navigation1element).SendKeys("x").Build().Perform();
                DoubleClick(navigation1element);
                Thread.Sleep(3000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                IList<IWebElement> imageloc = z3dvp.ViewportImgLocation();
                Size nav1 = imageloc[0].Size;
                Size nav2 = imageloc[1].Size;
                Size nav3 = imageloc[2].Size;
                Size Result = imageloc[3].Size;
                int Location1 = imageloc[0].Location.X;
                int Location2 = imageloc[1].Location.X;
                int Location3 = imageloc[2].Location.X;
                int Resultiloc = imageloc[3].Location.X;
                //Verification::MPR view enters OneUp mode with navigation control 1 magnified and that the other 3 controls are smaller in size and are displayed to the left of navigation control 1.
                if (nav1.Height > nav2.Height && nav1.Height > nav3.Height && nav1.Width > Result.Width &&
                    Location1 > Location2 && Location2 == Location3 && Location3 == Resultiloc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Double click on navigation control 2.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Navigation control 2 is swapped with navigation control 1.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav1 = imageloc[0].Size;
                nav2 = imageloc[1].Size;
                if (nav1.Height < nav2.Height && nav1.Width < nav2.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 7::Double click on navigation control 2.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::MPR view exits OneUp mode and goes back to its original view.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav1 = imageloc[0].Size;
                nav2 = imageloc[1].Size;
                if (nav1.Height == nav2.Height && nav1.Width == nav2.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 8::Resize the 3D viewer window so that it is displayed on portrait mode, the length of the window is < the height of the window.
                Driver.Manage().Window.Size = new Size(750, 950);
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 9::Double click on navigation control 3.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::MPR view enters OneUp mode with navigation control 3 magnified and that the other 3 controls are smaller in size and are displayed below the navigation control 3.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav1 = imageloc[0].Size;
                nav2 = imageloc[1].Size;
                nav3 = imageloc[2].Size;
                Result = imageloc[3].Size;
                Location1 = imageloc[0].Location.Y;
                Location2 = imageloc[1].Location.Y;
                Location3 = imageloc[2].Location.Y;
                Resultiloc = imageloc[3].Location.Y;
                if (nav3.Height > nav1.Height && nav3.Height > nav2.Height && nav3.Width > Result.Width && Location3 < Location1 && Location3 < Location2 && Location2 == Resultiloc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Double click on result control.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Result control is swapped with navigation control 3.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav3 = imageloc[2].Size;
                Result = imageloc[3].Size;
                if (nav3.Height < Result.Height && nav3.Width < Result.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 11::Go to the 3D view by clicking the 3D dropdown menu
                bool ThreeD4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                //Verification::3D view mode should be displayed.
                //int ThreeD4x1rViewport = z3dvp.Lefttopviewporttext("3D", 4);
                if (ThreeD4x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 12::Double click on navigation control 3.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The 3D view enters OneUp mode with navigation control 3 magnified and that the other 3 controls are smaller in size and are displayed below the navigation control 3.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav1 = imageloc[0].Size;
                nav2 = imageloc[1].Size;
                nav3 = imageloc[2].Size;
                Result = imageloc[3].Size;
                Location1 = imageloc[0].Location.Y;
                Location2 = imageloc[1].Location.Y;
                Location3 = imageloc[2].Location.Y;
                Resultiloc = imageloc[3].Location.Y;
                if (nav3.Height > nav1.Height && nav3.Height > nav2.Height && nav3.Width > Result.Width && Location3 < Location1 && Location3 < Location2 && Location2 == Resultiloc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 13::Double click on the 3D control.
                navigation1element = z3dvp.controlelement("3D 1");
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The 3D control is swapped with navigation control 3.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav3 = imageloc[2].Size;
                Result = imageloc[3].Size;
                if (nav3.Height < Result.Height && nav3.Width < Result.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 14::Double click on the 3D control.
                navigation1element = z3dvp.controlelement("3D 1");
                DoubleClick(navigation1element);
                Thread.Sleep(3000);
                //Verification::The 3D view exits OneUp mode and returns to its original view.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav3 = imageloc[2].Size;
                Result = imageloc[3].Size;
                if (nav3.Height == Result.Height && nav3.Width == Result.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 15::Resize the 3D viewer window so that it is displayed on landscape mode, the length of the window is > than the height of the window.
                Driver.Manage().Window.Size = new Size(950, 750);
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 16::Double click on navigation control 1.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The 3D view enters OneUp mode with navigation control 1 magnified and that the other 3 controls are smaller in size and are displayed to the left of navigation control 1.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav1 = imageloc[0].Size;
                nav2 = imageloc[1].Size;
                nav3 = imageloc[2].Size;
                Result = imageloc[3].Size;
                Location1 = imageloc[0].Location.X;
                Location2 = imageloc[1].Location.X;
                Location3 = imageloc[2].Location.X;
                Resultiloc = imageloc[3].Location.X;
                //Verification::MPR view enters OneUp mode with navigation control 1 magnified and that the other 3 controls are smaller in size and are displayed to the left of navigation control 1.
                if (nav1.Height > nav2.Height && nav1.Height > nav3.Height && nav1.Width > Result.Width && Location1 > Location2 &&
                    Location2 == Location3 && Location3 == Resultiloc)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 17::Double click on navigation control 2.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Navigation control 2 is swapped with navigation control 1.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                nav1 = imageloc[0].Size;
                nav2 = imageloc[1].Size;
                if (nav1.Height < nav2.Height && nav1.Width < nav2.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 18::Go to the SixUp view by clicking the 3D dropdown menu.
                bool ThreeD6x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Six up view mode should be displayed.
                if (ThreeD6x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 19::Double click on navigation control 1.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The SixUp view enters OneUp mode with navigation control 1 magnified and that the other 5 controls are smaller in size and are displayed to the left of navigation control 1.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                Size SixCrossnav1 = imageloc[0].Size;
                Size SixCrossnav2 = imageloc[1].Size;
                Size SixCross3D1 = imageloc[2].Size;
                Size SixCrossnav3 = imageloc[3].Size;
                Size SixCrossResult = imageloc[4].Size;
                Size SixCross3D2 = imageloc[5].Size;
                int LocSixcrossnav1 = imageloc[0].Location.X;
                int LocSixcrossnav2 = imageloc[1].Location.X;
                int LocSixcross3D1 = imageloc[2].Location.X;
                int LocSixcrossnav3 = imageloc[3].Location.X;
                int LocSixcrossResult = imageloc[4].Location.X;
                int LocSixcross3D2 = imageloc[5].Location.X;
                //Verification::MPR view enters OneUp mode with navigation control 1 magnified and that the other 3 controls are smaller in size and are displayed to the left of navigation control 1.
                if (SixCrossnav1.Width > SixCrossnav2.Width && SixCrossnav1.Width > SixCross3D1.Width && SixCrossnav1.Width > SixCrossnav3.Width
                 && LocSixcrossnav1 > LocSixcrossnav2 && LocSixcrossnav2 == LocSixcross3D1 && LocSixcross3D1 == LocSixcrossnav3 && LocSixcrossResult == LocSixcrossnav3 && LocSixcrossResult == LocSixcross3D2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 20::Double click on navigation control 2.
                IList<IWebElement> TopleftofImage = z3dvp.topleftann();
                if (TopleftofImage[1].Text.Contains(BluRingZ3DViewerPage.Navigationtwo))
                {
                    DoubleClick(TopleftofImage[1]);
                    PageLoadWait.WaitForFrameLoad(10);
                }
                //Verification::Navigation control 2 is swapped with navigation control 1.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                SixCrossnav1 = imageloc[0].Size;
                SixCrossnav2 = imageloc[1].Size;
                if (SixCrossnav1.Height < SixCrossnav2.Height && SixCrossnav1.Width < SixCrossnav2.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 21::Double click on navigation control 2.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The SixUp view exits OneUp mode and goes back to its original view.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                SixCrossnav1 = imageloc[0].Size;
                SixCrossnav2 = imageloc[1].Size;
                SixCross3D1 = imageloc[2].Size;
                SixCrossnav3 = imageloc[3].Size;
                SixCrossResult = imageloc[4].Size;
                SixCross3D2 = imageloc[5].Size;
                if (SixCrossnav1.Height == SixCrossnav2.Height && SixCrossnav2.Height == SixCross3D1.Height && SixCrossnav3.Height == SixCrossResult.Height && SixCrossResult.Height == SixCross3D2.Height)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 22::Resize the 3D viewer window so that it is displayed on portrait mode, the length of the window is < the height of the window.
                Driver.Manage().Window.Size = new Size(750, 950);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 23::Double click on navigation control 3.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The SixUp view enters OneUp mode with navigation control 3 magnified and that the other 5 controls are smaller in size and are displayed below the navigation control 3.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                SixCrossnav1 = imageloc[0].Size;
                SixCrossnav2 = imageloc[1].Size;
                SixCross3D1 = imageloc[2].Size;
                SixCrossnav3 = imageloc[3].Size;
                SixCrossResult = imageloc[4].Size;
                SixCross3D2 = imageloc[5].Size;
                LocSixcrossnav1 = imageloc[0].Location.Y;
                LocSixcrossnav2 = imageloc[1].Location.Y;
                LocSixcross3D1 = imageloc[2].Location.Y;
                LocSixcrossnav3 = imageloc[3].Location.Y;
                LocSixcrossResult = imageloc[4].Location.Y;
                LocSixcross3D2 = imageloc[5].Location.Y;
                if (SixCrossnav3.Height > SixCrossnav1.Height && SixCrossnav3.Height > SixCrossnav2.Height && SixCrossnav3.Width > SixCross3D1.Width && SixCrossnav3.Width > SixCrossResult.Width && SixCrossnav3.Width > SixCross3D2.Width && LocSixcrossnav3 < LocSixcrossnav1 && LocSixcrossnav1 == LocSixcrossnav2 && LocSixcrossnav2 == LocSixcross3D1 && LocSixcrossResult == LocSixcross3D1 && LocSixcrossResult == LocSixcross3D2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 24::Double click on result control.
                navigation1element = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                DoubleClick(navigation1element);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Result control is swapped with navigation control 3.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                SixCrossnav3 = imageloc[3].Size;
                SixCrossResult = imageloc[4].Size;
                if (SixCrossnav3.Height < SixCrossResult.Height && SixCrossnav3.Width < SixCrossResult.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 25::Go to the Curved MPR view by clicking the Curved MPR view button on the toolbar.
                bool CurvedMpr = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(10);
                //Changing The cursor mode
                z3dvp.select3DTools(Z3DTools.Window_Level);
                //Verification::Curved MPR view mode should be displayed.
                if (CurvedMpr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 26::Double click on the 3D path navigation control.
                IWebElement ThreeDPathNav = z3dvp.controlelement("3D Path Navigation");
                DoubleClick(ThreeDPathNav);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The Curved MPR view enters OneUp mode with the 3D path navigation control magnified and that the other 5 controls are smaller in size and are displayed below the 3D path navigation control.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                Size Curvednav1 = imageloc[0].Size;
                Size Curvednav2 = imageloc[1].Size;
                Size ThreeDPath = imageloc[2].Size;
                Size Curvednav3 = imageloc[3].Size;
                Size CurvedMPRNav = imageloc[4].Size;
                Size CurvedMPRView = imageloc[5].Size;
                int LocNav1 = imageloc[0].Location.Y;
                int LocNav2 = imageloc[1].Location.Y;
                int Loc3DPath = imageloc[2].Location.Y;
                int LocNav3 = imageloc[3].Location.Y;
                int LocMPR = imageloc[4].Location.Y;
                int LocCurvedMPR = imageloc[5].Location.Y;
                if (ThreeDPath.Height > Curvednav1.Height && ThreeDPath.Height > Curvednav2.Height && ThreeDPath.Width > CurvedMPRNav.Width && (CurvedMPRNav.Width == CurvedMPRView.Width || CurvedMPRNav.Width == CurvedMPRView.Width - 1) && Loc3DPath < LocNav1 && Loc3DPath < LocNav2 && Loc3DPath < LocNav3 && LocNav3 == LocMPR && LocMPR == LocCurvedMPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 27::Double click on the MPR path navigation control.
                ThreeDPathNav = z3dvp.controlelement("MPR Path Navigation");
                DoubleClick(ThreeDPathNav);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The MPR path navigation control is swapped with the 3D path navigation control.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                ThreeDPath = imageloc[2].Size;
                CurvedMPRNav = imageloc[4].Size;
                if (CurvedMPRNav.Height > ThreeDPath.Height && CurvedMPRNav.Width > ThreeDPath.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 28::Double click on the MPR path navigation control.
                ThreeDPathNav = z3dvp.controlelement("MPR Path Navigation");
                DoubleClick(ThreeDPathNav);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The Curved MPR view exits OneUp mode and returns to its original view.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                ThreeDPath = imageloc[2].Size;
                CurvedMPRNav = imageloc[4].Size;
                if ((CurvedMPRNav.Height <= ThreeDPath.Height || ThreeDPath.Height <= CurvedMPRNav.Height) && CurvedMPRNav.Width == ThreeDPath.Width)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 29::Resize the 3D viewer window so that it is displayed on landscape mode, the length of the window is > than the height of the window.
                Driver.Manage().Window.Size = new Size(950, 750);
                result.steps[++ExecutedSteps].status = "Pass";
                //Steps 30::Double click on Curved MPR control.
                ThreeDPathNav = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                z3dvp.select3DTools(Z3DTools.Pan);
                DoubleClick(ThreeDPathNav);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::The Curved MPR view enters OneUp mode with Curved MPR control magnified and that the other 5 controls are smaller in size and are displayed to the left of the Curved MPR control.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                Curvednav1 = imageloc[0].Size;
                Curvednav2 = imageloc[1].Size;
                ThreeDPath = imageloc[2].Size;
                Curvednav3 = imageloc[3].Size;
                CurvedMPRNav = imageloc[4].Size;
                CurvedMPRView = imageloc[5].Size;
                LocNav1 = imageloc[0].Location.X;
                LocNav2 = imageloc[1].Location.X;
                Loc3DPath = imageloc[2].Location.X;
                LocNav3 = imageloc[3].Location.X;
                LocMPR = imageloc[4].Location.X;
                LocCurvedMPR = imageloc[5].Location.X;
                if (CurvedMPRView.Width >= CurvedMPRNav.Width && CurvedMPRView.Width >= Curvednav3.Width && CurvedMPRView.Width >= Curvednav3.Width && (Curvednav2.Height == Curvednav1.Height || Curvednav2.Height == Curvednav1.Height - 1 || Curvednav2.Height == Curvednav1.Height + 1)
                   )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 31::Double click on navigation control 1.
                TopleftofImage = z3dvp.topleftann();
                if (TopleftofImage[0].Text.Contains(BluRingZ3DViewerPage.Navigationone))
                {
                    DoubleClick(TopleftofImage[0]);
                    PageLoadWait.WaitForFrameLoad(10);
                }
                Thread.Sleep(5000);
                //Verification::The Curved MPR control is swapped with navigation control 1.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                Curvednav1 = imageloc[0].Size;
                CurvedMPRView = imageloc[5].Size;
                if (Curvednav1.Width > CurvedMPRView.Width && Curvednav1.Height > CurvedMPRView.Height)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 32::Go to the Calcium Scoring view from 3D dropdown menu
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                Thread.Sleep(5000);
                z3dvp.checkerrormsg("y");
                //Verification::Calcium scoring view mode should be displayed.
                IList<IWebElement> tilelist = z3dvp.controlImage();
                int count = tilelist.Count;
                if (count.Equals(1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 33::Double click on the Calcium Scoring Control.
                IWebElement CalciumScoring = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                imageloc = z3dvp.ViewportImgLocation();
                Size BeforeCalcium = imageloc[0].Size;
                DoubleClick(CalciumScoring);
                //Verification::The Calcium Scoring view remains the same with the Calcium Scoring control magnified to fit the Z3D viewer window.
                Size AfterCalcium = imageloc[0].Size;
                if (BeforeCalcium.Height == AfterCalcium.Height && BeforeCalcium.Width == AfterCalcium.Width)
                {
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
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
                Driver.Close();

            }
        }



    }
}

