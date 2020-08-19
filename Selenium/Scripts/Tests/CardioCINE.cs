using System;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.HoldingPen;
using System.Text;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Data;
using Dicom;
using Dicom.Network;
using System.Net;
using Selenium.Scripts.Pages.eHR;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;

namespace Selenium.Scripts.Tests
{

    class CardioCINE : BasePage
    {

        public Login login { get; set; }
        public ExamImporter ei { get; set; }
        public HPLogin hplogin { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        public string filepath { get; set; }
        public Web_Uploader webuploader { get; set; }
        public RanorexObjects rnxobject { get; set; }

        public string EA_91 = "VMSSA-5-38-91";
        public string EA_131 = "VMSSA-4-38-131";
        public string PACS_A7 = "PA-A7-WS8";
        public string EA_96 = "VMSSA-5-38-96";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public CardioCINE(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            servicetool = new ServiceTool();
            mpaclogin = new MpacLogin();
            rnxobject = new RanorexObjects();
            webuploader = new Web_Uploader();
            wpfobject = new WpfObjects();
        }

        UserPreferences userpref = new UserPreferences();
        StudyViewer viewer;
        Studies studies = null;

        /// </summary>
        ///Cardio features(Group play/pause, measurements, cine fps, cine toolbar)	
        /// </summary> 

        public TestCaseResult Test_27483(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String[] Accession = AccessionList.Split(':');

            String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
            String[] PatientID = PatientIDList.Split('=');


            try
            {
                //Setup Test Step Description
                //Enable Cardiology
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                tool.NavigateToTab(ServiceTool.Templates_Tab);
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                wpfobject.GetListBox(0).Items[0].Click();
                tool.ClickApplyButtonFromTab();
                wpfobject.GetMainWindowByTitle("Template");
                wpfobject.ClickButton("6");
                tool.RestartIISandWindowsServices();
                tool.CloseServiceTool();
                taskbar.Show();

                //Step-1
                //In User preferences set the Automatically start cine to OFF for the modality to which the listed study belongs
                //Search and load for a multiframe study

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                Studies study = (Studies)login.Navigate("Studies");

                //DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");

                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("US");
                PageLoadWait.WaitForPageLoad(20);
                userpref.SelectRadioBtn("AutoStartCine", "Off");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();


                study.SearchStudy(patientID: PatientID[0], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientID[0]);

                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                    viewer.SeriesViewer_1X2().Displayed &&
                    viewer.cineplay(1, 1).Displayed &&
                    viewer.Thumbnails().Count == 2 &&
                    viewer.cineGroupPlayBtn().Displayed && viewer.SeriesViewPorts().Count == 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-7=> Note- for single image in viewport cinetoolbar will not be displayed
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                bool step7_1 = viewer.cinePrevGroupBtn().Displayed == false;
                bool step7_2 = viewer.cineGroupPlayBtn().Displayed == false;
                bool step7_3 = viewer.cineNextGroupBtn().Displayed == false;

                //Step-2
                //Set viewer layout to 1x2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForPageLoad(40);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ThumbnailLoadedIndicator()[1]));

                //Layout should be set
                if (viewer.SeriesViewPorts().Count == 2 &&
                    viewer.ThumbnailLoadedIndicator().Count == 2)
                {
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
                //Load the first frame in both the viewports

                var action3 = new Actions(BasePage.Driver);

                action3.DragAndDrop(viewer.Thumbnails()[1], viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                action3 = new Actions(BasePage.Driver);

                action3.DragAndDrop(viewer.Thumbnails()[1], viewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);


                //Image should be loaded

                String Thumbnail_SeriesID_3_1 = viewer.GetInnerAttribute(viewer.Thumbnails()[1], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_3_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");

                String Viewerport_ClusterViewID_3_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ClusterViewID");

                if (viewer.SeriesViewPorts().Count == 2 &&
                    Thumbnail_SeriesID_3_1 != null &&
                    Viewerport_ClusterViewID_3_1.Contains(Thumbnail_SeriesID_3_1) &&
                    Viewerport_ClusterViewID_3_2.Contains(Thumbnail_SeriesID_3_1))
                {
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
                //Verify the Measurement tools Line, Angle, Cobb angle, Rectangle, Ellipse, ROI Pixel

                bool Step4_1 = viewer.OperationErrorText_XxY(1, 2).Displayed == false;
                bool Step4_2 = viewer.OperationErrorText_XxY(1, 2).Displayed == false;

                String ErrorText = "Measurement operation is not supported for this image";

                //Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                bool Line_4_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool Line_4_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //Angle
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                bool Angle_4_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool Angle_4_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //Cobb Angle
                viewer.SelectToolInToolBar(IEnum.ViewerTools.CobbAngle);
                bool CobbAngle_4_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool CobbAngle_4_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //RectAngle
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawRectangle);
                bool RectAngle_4_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool RectAngle_4_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //Ellipse
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                bool Ellipse_4_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool Ellipse_4_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //ROI
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                bool ROI_4_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool ROI_4_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //User should not be allowed to draw measurements that display units

                if (Step4_1 && Step4_2 &&      //before select tool 
                    Line_4_1 && Line_4_2 &&
                    Angle_4_1 && Angle_4_2 &&
                    CobbAngle_4_1 && CobbAngle_4_2 &&
                    RectAngle_4_1 && RectAngle_4_2 &&
                    Ellipse_4_1 && Ellipse_4_2 &&
                    ROI_4_1 && ROI_4_2)
                {
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
                //Click group play

                string ClassAttribute = "svViewerImg ui-droppable";

                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));

                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForCineToPlay(1, 2);

                //Cine should start in both the viewports

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cinepause(1, 1).Displayed == true &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing

                    viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cinepause(1, 2).Displayed == true &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing
                    viewer.FrameIndicatorLine(1, 2).Displayed == true)
                {
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
                //Verify the group play button

                //group play button should not be displayed

                if (viewer.cineGroupPlayBtn().Displayed == false &&
                    viewer.cineGroupPauseBtn().Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Verify Cine toolbar
                //group pause button should be displayed enabled
                if (step7_1 && step7_2 && step7_3 &&
                  viewer.cineGroupPauseBtn().Displayed == true &&
                  viewer.cinepause(1, 1).Displayed == true &&
                  viewer.cinepause(1, 2).Displayed == true &&
                  viewer.cinepause(1, 1).Enabled == true &&
                  viewer.cinepause(1, 2).Enabled == true &&
                  viewer.cineGroupPauseBtn().Enabled == true)
                {
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
                //Verify Cine fps
                //The Cine fps should be as mentioned in DICOM tag (0018,1063)

                if (viewer.FrameIndicatorFps(1, 1).Text.Equals("8 fps") &&
                    viewer.FrameIndicatorFps(1, 2).Text.Equals("8 fps"))
                {
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
                //Stop Cine in a viewport
                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //Cine should stop and images should load
                //Viewport -1 CINE stopped.
                if (viewer.SeriesViewer_1X1().Displayed == true &&
                   viewer.SeriesViewer_1X1().GetAttribute("style").Contains("display: none;") == false &&
                   viewer.verifyFrameIndicatorLineChanging(1, 1) == 0 && //CINE is not playing (not started)

                   //Viewport -2 cine playing
                   viewer.SeriesViewer_1X2().Displayed == false &&
                   viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing
                   viewer.SeriesViewer_1X2().GetAttribute("style").Contains("display: none;") == true)
                {
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
                //Verify the image in the viewport

                //The image which was displayed when cine was stopped should load in the viewer 
                //Note- if user stopped cine on image #5, image#5 should be loaded in viewport after stopping

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == false &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                    viewer.cinepause(1, 1).Displayed == false &&
                    viewer.cinestop(1, 1).Displayed == false &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                    viewer.FrameIndicatorFps(1, 1).Displayed == false &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 0 && //CINE is not playing (not started/Stopped)
                    viewer.cineplay(1, 1).Displayed == true &&

                    //viewport-2 CINE playing
                    viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cinepause(1, 2).Displayed == true &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing
                    viewer.FrameIndicatorLine(1, 2).Displayed == true)
                {
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
                //Click group pause
                viewer.cineGroupPauseBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPlayBtn()));

                //Cine should pause in all viewports

                if (viewer.SeriesViewer_1X1().Displayed == true &&
                  viewer.SeriesViewer_1X2().Displayed == false &&

                  //Viewport -1 CINE Stopped
                  viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == false &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(1, 1).Displayed == false &&
                  viewer.cinestop(1, 1).Displayed == false &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                  viewer.FrameIndicatorFps(1, 1).Displayed == false &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1) == 0 && //CINE is not playing (not started/Stopped)
                  viewer.cineplay(1, 1).Displayed == true &&

                  viewer.SeriesViewer_1X1().Displayed == true &&
                  viewer.SeriesViewer_1X1().GetAttribute("style").Contains("display: none;") == false &&

                  //viewport-2 CINE pause
                    //viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") == false &&
                    //viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(1, 2).Displayed == false &&
                  viewer.cinestop(1, 2).Displayed == true &&
                  viewer.cineplay(1, 2).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 2) == -1)  //CINE is not playing (pause)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //Load the first frame in first viewport and second frame in second viewport	

                viewer.ClickDownArrowbutton(1, 1);
                IWebElement source = viewer.ViewportScrollHandle(1, 1);
                IWebElement destination = viewer.ViewportScrollBar(1, 1);
                int w = destination.Size.Width;
                Actions action12 = new Actions(BasePage.Driver);

                action12.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.cinestop(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));


                viewer.ClickDownArrowbutton(1, 2);
                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);
                w = destination.Size.Width;
                action12 = new Actions(BasePage.Driver);

                action12.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.ClickDownArrowbutton(1, 2);

                //Image should be loaded

                if (viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("1") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("2"))
                {
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
                //Click group play
                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));
                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForCineToPlay(1, 2);

                //Note- for single image in viewport Cine will not start ==> Validated in step-5

                //Cine should start in both the viewports
                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cinepause(1, 1).Displayed == true &&
                 viewer.cinestop(1, 1).Displayed == true &&
                 viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 && //CINE is playing

                 viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cinepause(1, 2).Displayed == true &&
                 viewer.cinestop(1, 2).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing
                 viewer.FrameIndicatorLine(1, 2).Displayed == true)
                {
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
                //Verify the group play button
                //group play button should not be displayed

                if (viewer.cineGroupPlayBtn().Displayed == false)
                {
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
                //Verify Cine toolbar

                //group pause button should be displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed

                if (viewer.cineGroupPauseBtn().Displayed == true &&
                    viewer.cineGroupPauseBtn().Enabled == true)
                {
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
                //Stop Cine in a viewport

                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //Cine should stop and images should load
                //Viewport -1 CINE stopped.
                if (viewer.SeriesViewer_1X1().Displayed == true &&
                   viewer.SeriesViewer_1X1().GetAttribute("style").Contains("display: none;") == false &&
                   viewer.verifyFrameIndicatorLineChanging(1, 1) == 0 && //CINE is not playing (not started/Stopped)

                   //Viewport -2 cine playing
                   viewer.SeriesViewer_1X2().Displayed == false &&
                   viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing
                   viewer.SeriesViewer_1X2().GetAttribute("style").Contains("display: none;"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17
                //Test Data- 
                //XA modality Study with value for use recommended display frame rate (0008,2144)

                //In User preferences set the Automatically start cine to OFF for the modality to which the listed study belongs
                //Close study
                //Search and load for a study with single series multiple images

                study.CloseStudy();


                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.ModalityDropDown().SelectByText("XA");
                userpref.SelectRadioBtn("AutoStartCine", "Off");

                userpref.CloseUserPreferences();


                study.SearchStudy(AccessionNo: Accession[0], Datasource: PACS_A7);
                study.SelectStudy("Accession", Accession[0]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The study should be loaded without any error
                if (viewer.SeriesViewer_1X1().Displayed &&
                    viewer.cineplay(1, 1).Displayed &&
                    viewer.Thumbnails().Count == 1 &&
                    viewer.cineGroupPlayBtn().Displayed == false &&
                    viewer.SeriesViewPorts().Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-x=> Note- for single image in viewport cinetoolbar will not be displayed
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                bool step22_1 = viewer.cinePrevGroupBtn().Displayed == false;
                bool step22_2 = viewer.cineGroupPlayBtn().Displayed == false;
                bool step22_3 = viewer.cineNextGroupBtn().Displayed == false;


                //Step-18
                //Set viewer layout to 1x3
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x3);

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Layout should be set
                if (viewer.SeriesViewPorts().Count == 3 &&
                     viewer.ThumbnailLoadedIndicator().Count == 1)
                {
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
                //Load the first image in all the viewports

                var action19 = new Actions(BasePage.Driver);

                action19.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                action19 = new Actions(BasePage.Driver);

                action19.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                action19 = new Actions(BasePage.Driver);

                action19.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X3()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Image should be loaded
                //have to verify 1x2 also*****

                String Thumbnail_SeriesID_19_1 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_19_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                String Viewerport_ClusterViewID_19_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                String Viewerport_ClusterViewID_19_3 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X3(), "src", '&', "ClusterViewID");

                if (viewer.SeriesViewPorts().Count == 3 &&
                    Thumbnail_SeriesID_19_1 != null &&
                    Viewerport_ClusterViewID_19_1.Contains(Thumbnail_SeriesID_19_1) &&
                    Viewerport_ClusterViewID_19_2.Contains(Thumbnail_SeriesID_19_1) &&
                    Viewerport_ClusterViewID_19_3.Contains(Thumbnail_SeriesID_19_1))
                {
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
                //Click group play
                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 3)));

                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForCineToPlay(1, 2);
                PageLoadWait.WaitForCineToPlay(1, 3);

                //Cine should start in all the viewports
                //Note- for single image in viewport Cine will not start

                if (
                   viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                   viewer.cinepause(1, 1).Displayed == true &&
                   viewer.cinestop(1, 1).Displayed == true &&
                   viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 && //CINE is playing

                   viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                   viewer.cinepause(1, 2).Displayed == true &&
                   viewer.cinestop(1, 2).Displayed == true &&
                   viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing

                   viewer.cineViewport(1, 3).GetAttribute("style").Contains("cursor: default") &&
                   viewer.cineViewport(1, 3).GetAttribute("class").Contains(ClassAttribute) &&
                   viewer.cinepause(1, 3).Displayed == true &&
                   viewer.cinestop(1, 3).Displayed == true &&
                   viewer.FrameIndicatorLine(1, 3).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 3) == 1) //CINE is playing
                {
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
                //Verify the group play button
                //group play button should not be displayed

                if (viewer.cineGroupPlayBtn().Displayed == false &&
                    viewer.cineGroupPauseBtn().Displayed == true)
                {
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
                //Verify Cine toolbar

                //group pause button should be displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed

                if (step22_1 && step22_2 && step22_3 && //Note Section
                    viewer.cineGroupPauseBtn().Displayed == true &&
                    viewer.cineGroupPauseBtn().Enabled == true)
                {
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
                //Click group pause

                viewer.cineGroupPauseBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPlayBtn()));

                //Cine should pause in all viewports

                if (//viewport-1 CINE pause
                  viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == true &&
                    //viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(1, 1).Displayed == false &&
                  viewer.cinestop(1, 1).Displayed == true &&
                  viewer.cineplay(1, 1).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1) == -1 && //CINE is not playing (pause)

                  //viewport-2 CINE pause
                  viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") == true &&
                    //viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(1, 2).Displayed == false &&
                  viewer.cinestop(1, 2).Displayed == true &&
                  viewer.cineplay(1, 2).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 2) == -1 && //CINE is not playing (pause)

                  //viewport-3 CINE pause
                  viewer.cineViewport(1, 3).GetAttribute("style").Contains("cursor: default") == true &&
                    //viewer.cineViewport(1, 3).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(1, 3).Displayed == false &&
                  viewer.cinestop(1, 3).Displayed == true &&
                  viewer.cineplay(1, 3).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 3).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 3) == -1) //CINE is not playing (pause)
                {
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
                //Verify Cine toolbar

                //group pause button should not be displayed and group play displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed

                if (viewer.cineGroupPauseBtn().Displayed == false &&
                    viewer.cineGroupPlayBtn().Displayed == true &&
                    viewer.cineGroupPlayBtn().Enabled == true)
                {
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
                //Click group play

                //before click have to verify all elements
                int lineCount1x1 = viewer.CineIndicatorLines(1, 1).Count;
                int lineCount1x2 = viewer.CineIndicatorLines(1, 2).Count;
                int lineCount1x3 = viewer.CineIndicatorLines(1, 3).Count;

                //Otherwise mark as Not Automated
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 3)));



                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 3)));

                //Cine should resume in both viewports and continue from where it was paused
                //Note- for single image in viewport Cine will not start ==>verified

                if (lineCount1x1 == 26 &&
                    lineCount1x2 == 26 &&
                    lineCount1x3 == 26 &&
                viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                viewer.cinepause(1, 1).Displayed == true &&
                viewer.cinestop(1, 1).Displayed == true &&
                viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 && //CINE is playing

                viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                viewer.cinepause(1, 2).Displayed == true &&
                viewer.cinestop(1, 2).Displayed == true &&
                viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing

                viewer.cineViewport(1, 3).GetAttribute("style").Contains("cursor: default") &&
                viewer.cineViewport(1, 3).GetAttribute("class").Contains(ClassAttribute) &&
                viewer.cinepause(1, 3).Displayed == true &&
                viewer.cinestop(1, 3).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 3) == 1 && //CINE is playing
                viewer.FrameIndicatorLine(1, 3).Displayed == true)
                {
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
                //Verify Cine fps

                //The Cine fps should be as mentioned in DICOM tag (0008,2144) -- Recommended Display frame rate

                if (viewer.FrameIndicatorFps(1, 1).Text.Equals("15 fps") &&
                viewer.FrameIndicatorFps(1, 2).Text.Equals("15 fps") &&
                viewer.FrameIndicatorFps(1, 3).Text.Equals("15 fps"))
                {
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
                //Stop Cine in all viewports
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 1)));
                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 2)));
                viewer.cinestop(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 3)));
                viewer.cinestop(1, 3).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 3)));

                //Cine should stop and images should load
                if (viewer.SeriesViewer_1X1().Displayed == true &&
                    viewer.SeriesViewer_1X1().GetAttribute("style").Contains("display: none;") == false &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 0 && //CINE is not playing (not started/Stopped)
                    //Viewport -2 
                    viewer.SeriesViewer_1X2().Displayed == true &&
                    viewer.SeriesViewer_1X2().GetAttribute("style").Contains("display: none;") == false &&
                    viewer.FrameIndicatorLine(1, 2).Displayed == false &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2) == 0 && //CINE is not playing (not started/Stopped)
                    //Viewport -3 
                    viewer.SeriesViewer_1X3().Displayed == true &&
                    viewer.SeriesViewer_1X3().GetAttribute("style").Contains("display: none;") == false &&
                    viewer.verifyFrameIndicatorLineChanging(1, 3) == 0 && //CINE is not playing (not started/Stopped)
                    viewer.FrameIndicatorLine(1, 3).Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28
                //Load the first image in first viewport and second image in second viewport

                viewer.ClickDownArrowbutton(1, 1);
                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.ViewportScrollBar(1, 1);
                w = destination.Size.Width;
                Actions action28 = new Actions(BasePage.Driver);

                action28.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.ClickDownArrowbutton(1, 2);
                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);
                w = destination.Size.Width;
                action28 = new Actions(BasePage.Driver);

                action28.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.ClickDownArrowbutton(1, 2);

                //Image should be loaded

                if (viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("1") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("2"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29
                //Click group play

                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 3)));

                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForCineToPlay(1, 2);
                PageLoadWait.WaitForCineToPlay(1, 3);

                //Cine should start in all the viewports
                //Note- for single image in viewport Cine will not start

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                viewer.cinepause(1, 1).Displayed == true &&
                viewer.cinestop(1, 1).Displayed == true &&
                viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 &&

                viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                viewer.cinepause(1, 2).Displayed == true &&
                viewer.cinestop(1, 2).Displayed == true &&
                viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 &&

                viewer.cineViewport(1, 3).GetAttribute("style").Contains("cursor: default") &&
                viewer.cinepause(1, 3).Displayed == true &&
                viewer.cinestop(1, 3).Displayed == true &&
                viewer.FrameIndicatorLine(1, 3).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 3) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30
                //Verify the group play button
                //group play button should not be displayed

                if (viewer.cineGroupPlayBtn().Displayed == false &&
                    viewer.cineGroupPauseBtn().Displayed == true)
                {
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
                //Verify Cine toolbar

                //"group pause button should be displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed"

                if (viewer.cineGroupPauseBtn().Displayed == true &&
                    viewer.cineGroupPauseBtn().Enabled == true)
                {
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
                //Stop Cine in a viewport

                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //Cine should stop and images should load
                //Viewport -1 CINE stopped.
                if (viewer.SeriesViewer_1X1().Displayed == true &&
                   viewer.SeriesViewer_1X1().GetAttribute("style").Contains("display: none;") == false &&
                   viewer.verifyFrameIndicatorLineChanging(1, 1) == 0 && //CINE is not playing (not started/Stopped)

                   //Viewport -2 cine playing
                   viewer.SeriesViewer_1X2().Displayed == false &&
                   viewer.SeriesViewer_1X2().GetAttribute("style").Contains("display: none;") == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 && //CINE is playing

                   //Viewport -3 cine playing
                   viewer.SeriesViewer_1X3().Displayed == false &&
                   viewer.SeriesViewer_1X3().GetAttribute("style").Contains("display: none;") == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 3) == 1) //CINE is playing
                {
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
                //Test Data- 
                //US modality There are multiple measurement regions Study with value for use cine rate (0018,0040)
                //Close study
                //In User preferences set the Automatically start cine to OFF for the modality to which the listed study belongs
                //Search and load for a study with multiple series multiple images

                study.CloseStudy();

                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpref.ModalityDropDown().SelectByText("US");
                userpref.SelectRadioBtn("AutoStartCine", "Off");
                userpref.CloseUserPreferences();


                study.SearchStudy(patientID: PatientID[1], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientID[1]);

                viewer = StudyViewer.LaunchStudy();

                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                    viewer.SeriesViewPorts().Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34
                //Set viewer layout to 2x2

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Layout should be set
                if (viewer.SeriesViewPorts().Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35
                //Load the different series in all the viewports

                var action35 = new Actions(BasePage.Driver);

                action35.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                action35 = new Actions(BasePage.Driver);

                action35.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                action35 = new Actions(BasePage.Driver);
                action35.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_2X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                action35 = new Actions(BasePage.Driver);
                action35.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_2X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                //Image should be loaded
                //have to verify other 3 viewport



                String Thumbnail_SeriesID_35_1 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_35_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");

                String Thumbnail_SeriesID_35_2 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_35_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ClusterViewID");

                String Thumbnail_SeriesID_35_3 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_35_3 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X1(), "src", '&', "ClusterViewID");

                //String Thumbnail_SeriesID_35_4 = viewer.GetInnerAttribute(viewer.Thumbnails()[3], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_35_4 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "ClusterViewID");


                if (viewer.SeriesViewPorts().Count == 4 &&
                    Thumbnail_SeriesID_35_1 != null && Thumbnail_SeriesID_35_2 != null &&
                    Thumbnail_SeriesID_35_3 != null && Thumbnail_SeriesID_35_2 != null &&
                    Viewerport_ClusterViewID_35_1.Contains(Thumbnail_SeriesID_35_1) &&
                    Viewerport_ClusterViewID_35_2.Contains(Thumbnail_SeriesID_35_2) &&
                    Viewerport_ClusterViewID_35_3.Contains(Thumbnail_SeriesID_35_3) &&
                    Viewerport_ClusterViewID_35_4.Contains(Thumbnail_SeriesID_35_2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-36
                //Verify the Measurement tools Line Angle Cobb angle Rectangle Ellipse ROI Pixel

                bool Step36_1 = viewer.OperationErrorText_XxY(1, 2).Displayed == false;
                bool Step36_2 = viewer.OperationErrorText_XxY(1, 2).Displayed == false;

                //ErrorText = "Measurement operation is not supported for this image";

                //Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                bool Line_36_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool Line_36_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //Angle
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                bool Angle_36_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool Angle_36_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //Cobb Angle
                viewer.SelectToolInToolBar(IEnum.ViewerTools.CobbAngle);
                bool CobbAngle_36_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool CobbAngle_36_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //RectAngle
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawRectangle);
                bool RectAngle_36_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool RectAngle_36_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //Ellipse
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);
                bool Ellipse_36_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool Ellipse_36_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //ROI
                viewer.SelectToolInToolBar(IEnum.ViewerTools.DrawROI);
                bool ROI_36_1 = viewer.OperationErrorText_XxY(1, 1).Displayed && viewer.OperationErrorText_XxY(1, 1).Text.Equals(ErrorText);
                bool ROI_36_2 = viewer.OperationErrorText_XxY(1, 2).Displayed && viewer.OperationErrorText_XxY(1, 2).Text.Equals(ErrorText);

                //User should not be allowed to draw measurements that display units

                if (Step36_1 && Step36_2 &&      //before select tool 
                    Line_36_1 && Line_36_2 &&
                    Angle_36_1 && Angle_36_2 &&
                    CobbAngle_36_1 && CobbAngle_36_2 &&
                    RectAngle_36_1 && RectAngle_36_2 &&
                    Ellipse_36_1 && Ellipse_36_2 &&
                    ROI_36_1 && ROI_36_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-37
                //Click group play

                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(2, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.FrameIndicatorLine(2, 2)));

                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForCineToPlay(1, 2);
                PageLoadWait.WaitForCineToPlay(2, 1);
                PageLoadWait.WaitForCineToPlay(2, 2);

                //Cine should start in all the viewports
                //Note- for single image in viewport Cine will not start

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                viewer.cinepause(1, 1).Displayed == true &&
                viewer.cinestop(1, 1).Displayed == true &&
                viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 &&

                viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                viewer.cinepause(1, 2).Displayed == true &&
                viewer.cinestop(1, 2).Displayed == true &&
                viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 &&

                viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                viewer.cinepause(2, 1).Displayed == true &&
                viewer.cinestop(2, 1).Displayed == true &&
                viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(2, 1) == 1 &&

                viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                viewer.cinepause(2, 2).Displayed == true &&
                viewer.cinestop(2, 2).Displayed == true &&
                viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                viewer.verifyFrameIndicatorLineChanging(2, 2) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-38
                //Verify the group play button
                //group play button should not be displayed

                if (viewer.cineGroupPlayBtn().Displayed == false &&
                viewer.cineGroupPauseBtn().Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-39
                //Verify Cine toolbar

                //group pause button should be displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed
                if (viewer.cineGroupPauseBtn().Displayed == true &&
                    viewer.cineGroupPauseBtn().Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-40
                //Verify Cine fps

                //The Cine fps should be as mentioned in DICOM tag (0018,0040)
                //--Have to verify *** Dicom also

                if (viewer.FrameIndicatorFps(1, 1).Text.Equals("30 fps") &&
                    viewer.FrameIndicatorFps(1, 2).Text.Equals("30 fps") &&
                    viewer.FrameIndicatorFps(2, 1).Text.Equals("30 fps") &&
                    viewer.FrameIndicatorFps(2, 2).Text.Equals("30 fps"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-41
                //Pause cine in 2 viewports
                viewer.cinepause(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));

                //Cine should pause in 2 viewports and cine should continue in remaining viewports

                if ( //viewport-2 CINE pause
                    viewer.cinepause(1, 2).Displayed == false &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.cineplay(1, 2).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2) == -1 &&

                      //viewport-1 CINE playing
                    viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cinepause(1, 1).Displayed == true &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 &&

                      //viewport-3 CINE playing
                    viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cinepause(2, 1).Displayed == true &&
                    viewer.cinestop(2, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(2, 1) == 1 &&

                      //viewport-4 CINE playing
                    viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cinepause(2, 2).Displayed == true &&
                    viewer.cinestop(2, 2).Displayed == true &&
                    viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(2, 2) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-42
                //Click group pause
                //Cine should pause in the remaining viewports

                viewer.cineGroupPauseBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));

                if ( //viewport-1 CINE pause
                   viewer.cinepause(1, 1).Displayed == false &&
                   viewer.cinestop(1, 1).Displayed == true &&
                   viewer.cineplay(1, 1).Displayed == true &&
                   viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 1) == -1 &&

                    //viewport-2 CINE pause
                   viewer.cinepause(1, 2).Displayed == false &&
                   viewer.cinestop(1, 2).Displayed == true &&
                   viewer.cineplay(1, 2).Displayed == true &&
                   viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 2) == -1 &&

                     //viewport-3 CINE pause
                   viewer.cinepause(2, 1).Displayed == false &&
                   viewer.cinestop(2, 1).Displayed == true &&
                   viewer.cineplay(2, 1).Displayed == true &&
                   viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(2, 1) == -1 &&

                     //viewport-4 CINE pause
                   viewer.cinepause(2, 2).Displayed == false &&
                   viewer.cinestop(2, 2).Displayed == true &&
                   viewer.cineplay(2, 2).Displayed == true &&
                   viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(2, 2) == -1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-43
                //Verify Cine toolbar

                //group pause button should not be displayed and group play displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed


                if (viewer.cineGroupPauseBtn().Displayed == false &&
                   viewer.cineGroupPlayBtn().Displayed == true &&
                   viewer.cineGroupPlayBtn().Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-44
                //Click group play
                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPauseBtn()));

                //Cine should start in all the viewports
                //Note- for single image in viewport Cine will not start"
                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                     viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(1, 1).Displayed == true &&
                     viewer.cinestop(1, 1).Displayed == true &&
                     viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                     viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 &&

                     viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                     viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(1, 2).Displayed == true &&
                     viewer.cinestop(1, 2).Displayed == true &&
                     viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                     viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 &&

                     viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                     viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(2, 1).Displayed == true &&
                     viewer.cinestop(2, 1).Displayed == true &&
                     viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                     viewer.verifyFrameIndicatorLineChanging(2, 1) == 1 &&

                     viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                     viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(2, 2).Displayed == true &&
                     viewer.cinestop(2, 2).Displayed == true &&
                     viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(2, 2) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-45
                //Stop Cine in all viewports
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 1)));
                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 2)));
                viewer.cinestop(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(2, 1)));
                viewer.cinestop(2, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 1)));

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(2, 2)));
                viewer.cinestop(2, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));


                //Cine should stop and images should load
                if (viewer.SeriesViewer_1X1().Displayed == true &&
                    viewer.SeriesViewer_1X1().GetAttribute("style").Contains("display: none;") == false &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                    //Viewport -2 
                    viewer.SeriesViewer_1X2().Displayed == true &&
                    viewer.SeriesViewer_1X2().GetAttribute("style").Contains("display: none;") == false &&
                    viewer.FrameIndicatorLine(1, 2).Displayed == false &&
                    //Viewport -3 
                    viewer.SeriesViewer_2X1().Displayed == true &&
                    viewer.SeriesViewer_2X1().GetAttribute("style").Contains("display: none;") == false &&
                    viewer.FrameIndicatorLine(2, 1).Displayed == false &&
                    //Viewport -4 
                    viewer.SeriesViewer_2X2().Displayed == true &&
                    viewer.SeriesViewer_2X2().GetAttribute("style").Contains("display: none;") == false &&
                    viewer.FrameIndicatorLine(2, 2).Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-46
                //Test Data- 
                //scroll and change image# in all viewports
                //Load different images of different series in all the viewports

                //Image should be loaded
                viewer.ClickDownArrowbutton(1, 1);
                viewer.ClickDownArrowbutton(1, 2);
                viewer.ClickDownArrowbutton(2, 1);
                viewer.ClickDownArrowbutton(2, 2);

                int v1 = Int32.Parse(viewer.SeriesViewer_1X1().GetAttribute("imagenum"));
                int v2 = Int32.Parse(viewer.SeriesViewer_1X2().GetAttribute("imagenum"));
                int v3 = Int32.Parse(viewer.SeriesViewer_2X1().GetAttribute("imagenum"));
                int v4 = Int32.Parse(viewer.SeriesViewer_2X2().GetAttribute("imagenum"));

                //Click down arrow
                viewer.ClickDownArrowbutton(1, 1);
                viewer.ClickDownArrowbutton(1, 2);
                viewer.ClickDownArrowbutton(2, 1);
                viewer.ClickDownArrowbutton(2, 2);

                //After Down button Click(ADC)
                int ADC1 = Int32.Parse(viewer.SeriesViewer_1X1().GetAttribute("imagenum"));
                int ADC2 = Int32.Parse(viewer.SeriesViewer_1X2().GetAttribute("imagenum"));
                int ADC3 = Int32.Parse(viewer.SeriesViewer_2X1().GetAttribute("imagenum"));
                int ADC4 = Int32.Parse(viewer.SeriesViewer_2X2().GetAttribute("imagenum"));

                //Click up arrow
                viewer.ClickUpArrowbutton(1, 1);
                viewer.ClickUpArrowbutton(1, 2);
                viewer.ClickUpArrowbutton(2, 1);
                viewer.ClickUpArrowbutton(2, 2);

                //After Up button Click(AUC)
                int AUC1 = Int32.Parse(viewer.SeriesViewer_1X1().GetAttribute("imagenum"));
                int AUC2 = Int32.Parse(viewer.SeriesViewer_1X2().GetAttribute("imagenum"));
                int AUC3 = Int32.Parse(viewer.SeriesViewer_2X1().GetAttribute("imagenum"));
                int AUC4 = Int32.Parse(viewer.SeriesViewer_2X2().GetAttribute("imagenum"));



                //v1_num == (ADC1 +1)_num || v1_num == ADC1_num && v1_num == (AUC1 -1)_num
                if ((v1 == ADC1 - 1 || (v1 == ADC1 && v1 == AUC1 + 1)) &&
                    (v2 == ADC2 - 1 || (v2 == ADC2 && v2 == AUC2 + 1)) &&
                    (v3 == ADC3 - 1 || (v3 == ADC3 && v3 == AUC3 + 1)) &&
                    (v4 == ADC4 - 1 || (v4 == ADC4 && v4 == AUC4 + 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-47
                //Click group play
                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPauseBtn()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 2)));

                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForCineToPlay(1, 2);
                PageLoadWait.WaitForCineToPlay(2, 1);
                PageLoadWait.WaitForCineToPlay(2, 2);

                //Cine should start in all the viewports
                //Note- for single image in viewport Cine will not start

                if (//viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    //viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(1, 1).Displayed == true &&
                     viewer.cinestop(1, 1).Displayed == true &&
                     viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    //viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == 1 &&

                     //viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                    //viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(1, 2).Displayed == true &&
                     viewer.cinestop(1, 2).Displayed == true &&
                     viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                    //viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == 1 &&

                     //viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                    //viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(2, 1).Displayed == true &&
                     viewer.cinestop(2, 1).Displayed == true &&
                     viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                    //viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == 1 &&

                     //viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                    //viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) &&
                     viewer.cinepause(2, 2).Displayed == true &&
                     viewer.cinestop(2, 2).Displayed == true &&
                     viewer.FrameIndicatorLine(2, 2).Displayed == true)// &&
                //viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-48
                //Verify the group play button
                //group play button should not be displayed

                if (viewer.cineGroupPlayBtn().Displayed == false &&
                viewer.cineGroupPauseBtn().Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-49
                //Verify Cine toolbar
                //group pause button should be displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed

                if (viewer.cineGroupPauseBtn().Displayed == true &&
                   viewer.cineGroupPauseBtn().Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-50
                //Pause cine in 2 viewports

                viewer.cinepause(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                viewer.cinepause(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));

                //Cine should pause in 2 viewports and 
                //cine should continue in remaining viewports group pause button should remain enabled


                if (//group pause btn should remain enabled
                    viewer.cineGroupPauseBtn().Displayed == true &&
                    viewer.cineGroupPauseBtn().Enabled == true &&

                    //viewport-1 CINE pause
                    viewer.cinepause(1, 1).Displayed == false &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.cineplay(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == -1 &&

                        //viewport-2 CINE pause
                    viewer.cinepause(1, 2).Displayed == false &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.cineplay(1, 2).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == -1 &&

                      //viewport-3 CINE playing
                    viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cinepause(2, 1).Displayed == true &&
                    viewer.cinestop(2, 1).Displayed == true &&
                    viewer.cineplay(2, 1).Displayed == false &&
                    viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == 1 &&

                      //viewport-4 CINE playing
                    viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cinepause(2, 2).Displayed == true &&
                    viewer.cinestop(2, 2).Displayed == true &&
                    viewer.cineplay(2, 2).Displayed == false &&
                    viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-51
                //Start Cine in 1 of the paused viewports
                viewer.cineplay(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                PageLoadWait.WaitForCineToPlay(1, 1);

                //Cine should be playing in 3 viewports and paused in 1

                if (//viewport-1 CINE playing
                 viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                 viewer.cinepause(1, 1).Displayed == true &&
                 viewer.cinestop(1, 1).Displayed == true &&
                 viewer.cineplay(1, 1).Displayed == false &&
                 viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == 1 && //CINE is playing

                     //viewport-2 CINE pause
                 viewer.cinepause(1, 2).Displayed == false &&
                 viewer.cinestop(1, 2).Displayed == true &&
                 viewer.cineplay(1, 2).Displayed == true &&
                 viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == -1 && //CINE is not playing (pause)

                   //viewport-3 CINE playing
                 viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) &&
                 viewer.cinepause(2, 1).Displayed == true &&
                 viewer.cinestop(2, 1).Displayed == true &&
                 viewer.cineplay(2, 1).Displayed == false &&
                 viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == 1 && //CINE is playing

                   //viewport-4 CINE playing
                 viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) &&
                 viewer.cinepause(2, 2).Displayed == true &&
                 viewer.cinestop(2, 2).Displayed == true &&
                 viewer.cineplay(2, 2).Displayed == false &&
                 viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == 1) //CINE is playing)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-52
                //Click group pause

                viewer.cineGroupPauseBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPlayBtn()));

                //Cine should pause in the remaining viewports

                if (//viewport-1 CINE pause
                      viewer.cinepause(1, 1).Displayed == false &&
                      viewer.cinestop(1, 1).Displayed == true &&
                      viewer.cineplay(1, 1).Displayed == true &&
                      viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                      viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == -1 &&

                          //viewport-2 CINE pause
                      viewer.cinepause(1, 2).Displayed == false &&
                      viewer.cinestop(1, 2).Displayed == true &&
                      viewer.cineplay(1, 2).Displayed == true &&
                      viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                      viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == -1 &&

                          //viewport-3 CINE pause
                      viewer.cinepause(2, 1).Displayed == false &&
                      viewer.cinestop(2, 1).Displayed == true &&
                      viewer.cineplay(2, 1).Displayed == true &&
                      viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                      viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == -1 &&

                         //viewport-4 CINE pause
                      viewer.cinepause(2, 2).Displayed == false &&
                      viewer.cinestop(2, 2).Displayed == true &&
                      viewer.cineplay(2, 2).Displayed == true &&
                      viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                      viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == -1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-53
                //Verify Cine toolbar

                //group pause button should not be displayed and group play displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed

                if (viewer.cineGroupPauseBtn().Displayed == false &&
                     viewer.cineGroupPlayBtn().Displayed == true &&
                     viewer.cineGroupPlayBtn().Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-54
                //Click group play
                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPauseBtn()));

                //Cine should start in all the viewports

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                   viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                   viewer.cinepause(1, 1).Displayed == true &&
                   viewer.cinestop(1, 1).Displayed == true &&
                   viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == 1 &&

                   viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                   viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                   viewer.cinepause(1, 2).Displayed == true &&
                   viewer.cinestop(1, 2).Displayed == true &&
                   viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == 1 &&

                   viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                   viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) &&
                   viewer.cinepause(2, 1).Displayed == true &&
                   viewer.cinestop(2, 1).Displayed == true &&
                   viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == 1 &&

                   viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                   viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) &&
                   viewer.cinepause(2, 2).Displayed == true &&
                   viewer.cinestop(2, 2).Displayed == true &&
                   viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                   viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-55
                //say change it to 5fps
                //Change fps by dragging the scrollbar on cine toolbar
                //Cine should be playing with 5fps in viewport
                result.steps[++ExecutedSteps].status = "Not Automated";


                //act55.ClickAndHold(viewer.cinesliderhandle(1, 1)).MoveToElement(viewer.cineslider(1, 1),0,0).Build().Perform();
                //if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                //    act55.Release(viewer.cinesliderhandle(1, 1)).Build().Perform();

                //if (viewer.CineLabelFrameRate(1, 1).Text.Equals("5/5 fps"))


                //Step-56
                //Pause cine in all individual viewports

                viewer.cinepause(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                viewer.cinepause(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                viewer.cinepause(2, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 1)));
                viewer.cinepause(2, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));

                //Cine should pause
                if (//viewport-1 CINE pause
                    //viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == false &&
                    //viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                    viewer.cinepause(1, 1).Displayed == false &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.cineplay(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == -1 &&

                        //viewport-2 CINE pause
                    //viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") == false &&
                    //viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) == false &&
                    viewer.cinepause(1, 2).Displayed == false &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.cineplay(1, 2).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == -1 &&

                        //viewport-3 CINE pause
                    //viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") == false &&
                    //viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                    viewer.cinepause(2, 1).Displayed == false &&
                    viewer.cinestop(2, 1).Displayed == true &&
                    viewer.cineplay(2, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == -1 &&

                    //viewport-4 CINE pause
                    //viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") == false &&
                    //viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) == false &&
                    viewer.cinepause(2, 2).Displayed == false &&
                    viewer.cinestop(2, 2).Displayed == true &&
                    viewer.cineplay(2, 2).Displayed == true &&
                    viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == -1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-57
                //Verify Cine toolbar

                //group pause button should not be displayed and group play displayed enabled
                //Note- for single image in viewport cinetoolbar will not be displayed


                if (viewer.cineGroupPauseBtn().Displayed == false &&
                     viewer.cineGroupPlayBtn().Displayed == true &&
                     viewer.cineGroupPlayBtn().Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-58
                //Click group play
                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPauseBtn()));

                //Cine should start in all the viewports

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                  viewer.cinepause(1, 1).Displayed == true &&
                  viewer.cinestop(1, 1).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == 1 &&

                  viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                  viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                  viewer.cinepause(1, 2).Displayed == true &&
                  viewer.cinestop(1, 2).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == 1 &&

                  viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                  viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) &&
                  viewer.cinepause(2, 1).Displayed == true &&
                  viewer.cinestop(2, 1).Displayed == true &&
                  viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == 1 &&

                  viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                  viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) &&
                  viewer.cinepause(2, 2).Displayed == true &&
                  viewer.cinestop(2, 2).Displayed == true &&
                  viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-59
                //Stop Cine in a viewport

                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //Cine should stop and images should load

                if (
                    //CINE should be stopped viewport-1
                 viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == false &&
                 viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                 viewer.cinepause(1, 1).Displayed == false &&
                 viewer.cinestop(1, 1).Displayed == false &&
                 viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                 viewer.cineplay(1, 1).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(1, 1, 1000) == 0 &&

                 viewer.SeriesViewer_1X1().Displayed == true &&
                 viewer.SeriesViewer_1X1().GetAttribute("style").Contains("display: none;") == false &&
                 viewer.FrameIndicatorLine(1, 1).Displayed == false &&

                 //CINE is playing other Viewport
                 viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                 viewer.cinepause(1, 2).Displayed == true &&
                 viewer.cinestop(1, 2).Displayed == true &&
                 viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(1, 2, 1000) == 1 &&

                 viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) &&
                 viewer.cinepause(2, 1).Displayed == true &&
                 viewer.cinestop(2, 1).Displayed == true &&
                 viewer.FrameIndicatorLine(2, 1).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(2, 1, 1000) == 1 &&

                 viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") &&
                 viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) &&
                 viewer.cinepause(2, 2).Displayed == true &&
                 viewer.cinestop(2, 2).Displayed == true &&
                 viewer.FrameIndicatorLine(2, 2).Displayed == true &&
                 viewer.verifyFrameIndicatorLineChanging(2, 2, 1000) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                study.CloseStudy();
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

        /// </summary>
        /// Automatically Start Cine	
        /// </summary> 

        public TestCaseResult Test_27484(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Studies studies = new Studies();
            StudyViewer StudyVw = null;
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            UserPreferences userpreferences = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Setup Test Step Description
                //result = new TestCaseResult(stepcount);
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                string[] datasources = null;
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastName = LastNameList.Split(':');
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] PatientID = PatientIDList.Split(':');
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Modality = ModalityList.Split(':');
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] FirstName = FirstNameList.Split(':');
                String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String[] StudyDate = StudyDateList.Split(';');

                String TestdomainD1 = "DomainD1_484_" + random.Next(1, 1000);
                String TestdomainAdminD1 = "DomainAdminD1_484_" + random.Next(1, 1000);
                String TestRoleR1 = "RoleR1_484_" + random.Next(1, 1000);
                String TestuserU1 = "UserU1_484" + new Random().Next(1, 1000);
                String TestdomainD2 = "DomainD2_484_" + random.Next(1, 1000);
                String TestdomainAdminD2 = "DomainAdminD2_484_" + random.Next(1, 1000);
                String TestRoleR2 = "RoleR2_484_" + random.Next(1, 1000);
                String TestuserU2 = "UserU2_484" + new Random().Next(1, 1000);


                //Pre-condition
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.CreateDomain(TestdomainD1, TestdomainAdminD1, datasources);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.ClickSaveNewDomain();
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainD1, TestRoleR1, RoleCred: 99, GrantAccess: 99);
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.SelectDomainFromDropdownList(TestdomainD1);
                usermanagement.CreateUser(TestuserU1, TestRoleR1);
                login.Logout();
                login.LoginIConnect(TestuserU1, TestuserU1);

                //step 1
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                string defFPS = userpreferences.CineDefaultFrameRate().GetAttribute("value");
                userpreferences.CancelUserPreferences();
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                var view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step1 = false;
                step1 = studies.CompareImage(result.steps[executedSteps], view);
                if (step1)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 2
                StudyVw.SeriesViewer_1X1().Click();
                StudyVw.cineplay(1, 1).Click();
                if (StudyVw.cinepause(1, 1).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 3
                PageLoadWait.WaitForElementToDisplay(StudyVw.CineLabelFrameRate(1, 1), 90);
                string fps = StudyVw.CineLabelFrameRate(1, 1).Text;

                string fp = defFPS + "/" + defFPS + " fps";
                if (fp.Equals(fps))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 4
                login.CloseStudy();
                login.Logout();
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SearchDomain(TestdomainD1);
                domainmanagement.SelectDomain(TestdomainD1);
                domainmanagement.ClickEditDomain();
                bool step4 = false;
                SelectElement modality = domainmanagement.ModalityDropDown();
                for (int i = 0; i < modality.Options.Count; i++)
                {
                    modality.SelectByIndex(i);
                    PageLoadWait.WaitForPageLoad(10);
                    if (domainmanagement.AutoCineOFF().Selected)
                    {
                        step4 = true;
                    }
                    else
                    {
                        step4 = false;
                        break;
                    }
                }
                if (step4)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 5
                domainmanagement.ModalityDropDown().SelectByText(Modality[0]);
                domainmanagement.ExamModeON().Click();
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.SearchDomain(TestdomainD1);
                domainmanagement.SelectDomain(TestdomainD1);
                domainmanagement.ClickEditDomain();
                bool step5 = false;
                domainmanagement.ModalityDropDown().SelectByText(Modality[0]);
                step5 = domainmanagement.ExamModeON().Selected;
                domainmanagement.ClickCloseEditDomain();
                if (step5)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 6
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SelectDomainfromDropDown(TestdomainD1);
                rolemanagement.EditRoleByName(TestRoleR1);
                rolemanagement.UseDomainSetting().Click();
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                rolemanagement.AutoCINEON_RB().Click();
                rolemanagement.ClickSaveEditRole();
                rolemanagement.SelectDomainfromDropDown(TestdomainD1);
                rolemanagement.EditRoleByName(TestRoleR1);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool step6 = false;
                step6 = rolemanagement.AutoCINEON_RB().Selected;
                if (step6)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 7
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                string XALyt7 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                rolemanagement.ModalityDropDown().SelectByText(Modality[2]);
                string USLyt7 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                rolemanagement.LayoutDropDown().SelectByText("1x3");
                rolemanagement.ClickSaveEditRole();
                rolemanagement.SelectDomainfromDropDown(TestdomainD1);
                rolemanagement.EditRoleByName(TestRoleR1);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                string MRLyt7 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                if ((XALyt7 == "auto") && (USLyt7 == "auto") && (MRLyt7 == "1x3"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 8
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                bool XAexm8 = rolemanagement.AutoCINEOFF_RB().Selected;
                rolemanagement.ModalityDropDown().SelectByText(Modality[2]);
                bool USexm8 = rolemanagement.AutoCINEOFF_RB().Selected;
                if (XAexm8 && USexm8)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                rolemanagement.ClickCloseButton();

                //step 9
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SelectDomain(TestdomainD1);
                domainmanagement.ClickEditDomain();
                domainmanagement.ModalityDropDown().SelectByText(Modality[0]);
                domainmanagement.LayoutDropDown().SelectByText("1x3");
                domainmanagement.ClickSaveEditDomain();
                domainmanagement.SelectDomain(TestdomainD1);
                domainmanagement.ClickEditDomain();
                domainmanagement.ModalityDropDown().SelectByText(Modality[0]);
                string MRLyt9 = domainmanagement.LayoutDropDown().SelectedOption.Text;
                domainmanagement.ClickCloseEditDomain();
                login.Logout();
                login.LoginIConnect(TestuserU1, TestuserU1);
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                studies.LaunchStudy(isAutoCineEnabled: true);
                bool cine91 = StudyVw.cinepause(1, 1).Displayed;
                bool cine92 = StudyVw.cinepause(1, 2).Displayed;
                bool cine93 = StudyVw.cinepause(1, 3).Displayed;
                if (cine91 && cine92 && cine93 && (MRLyt9 == "1x3"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 10
                bool line = false;
                bool angle = false;
                bool cobb = false;
                bool rctngl = false;
                bool elps = false;
                bool ROI = false;
                bool pxl = false;
                if (StudyVw.GetViewerDisabledTools("Line Measurement"))
                    line = true;
                if (StudyVw.GetViewerDisabledTools("Cobb Angle"))
                    cobb = true;
                if (StudyVw.GetViewerDisabledTools("Angle Measurement"))
                    angle = true;
                if (StudyVw.GetViewerDisabledTools("Draw Rectangle"))
                    rctngl = true;
                if (StudyVw.GetViewerDisabledTools("Draw Ellipse"))
                    elps = true;
                if (StudyVw.GetViewerDisabledTools("Draw ROI"))
                    ROI = true;
                if (StudyVw.GetViewerDisabledTools("Get Pixel Value"))
                    pxl = true;
                if (line && angle && cobb && rctngl && elps && ROI && pxl)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 11
                studies.CloseStudy();
                login.Logout();
                login.LoginIConnect(username, password);
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SelectDomainfromDropDown(TestdomainD1);
                rolemanagement.EditRoleByName(TestdomainAdminD1);
                rolemanagement.UnCheckCheckbox(rolemanagement.UseDomainSetting());
                bool step11 = false;
                SelectElement modality1 = rolemanagement.ModalityDropDown();
                for (int i = 0; i < modality1.Options.Count; i++)
                {
                    modality1.SelectByIndex(i);
                    PageLoadWait.WaitForPageLoad(10);
                    if (rolemanagement.AutoCINEOFF_RB().Selected)
                    {
                        step11 = true;
                    }
                    else
                    {
                        step11 = false;
                        break;
                    }
                }
                if (step11)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 12
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                rolemanagement.AutoCINEON_RB().Click();
                rolemanagement.ClickSaveEditRole();
                rolemanagement.SelectDomainfromDropDown(TestdomainD1);
                rolemanagement.EditRoleByName(TestdomainAdminD1);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool step12 = false;
                step12 = rolemanagement.AutoCINEON_RB().Selected;
                if (step12)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 13
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                string XALyt13 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                rolemanagement.ModalityDropDown().SelectByText(Modality[2]);
                string USLyt13 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                rolemanagement.LayoutDropDown().SelectByText("1x3");
                rolemanagement.ClickSaveEditRole();
                rolemanagement.SelectDomainfromDropDown(TestdomainD1);
                rolemanagement.EditRoleByName(TestdomainAdminD1);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                string MRLyt13 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                if ((XALyt13 == "auto") && (USLyt13 == "auto") && (MRLyt13 == "1x3"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 14
                bool step14 = false;
                SelectElement modality2 = rolemanagement.ModalityDropDown();
                for (int i = 0; i < modality2.Options.Count; i++)
                {
                    modality2.SelectByIndex(i);
                    PageLoadWait.WaitForPageLoad(10);
                    if (modality2.SelectedOption.Text.Equals(Modality[0]))
                    {
                        if (rolemanagement.AutoCINEON_RB().Selected)
                        {
                            step14 = true;
                        }
                        else
                        {
                            step14 = false;
                            break;
                        }
                    }
                    else if (!modality2.SelectedOption.Text.Equals(Modality[0]))
                        if (rolemanagement.AutoCINEOFF_RB().Selected)
                            step14 = true;
                        else
                        {
                            step14 = false;
                            break;
                        }
                }
                if (step14)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                rolemanagement.ClickCloseButton();

                //step 15
                login.Logout();
                login.LoginIConnect(TestdomainD1, TestdomainD1);
                if (login.LogoutBtn().Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                defFPS = userpreferences.CineDefaultFrameRate().GetAttribute("value");
                userpreferences.CancelUserPreferences();

                //step 16
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                studies.LaunchStudy(isAutoCineEnabled: true);
                bool cine161 = StudyVw.cinepause(1, 1).Displayed;
                bool cine162 = StudyVw.cinepause(1, 2).Displayed;
                bool cine163 = StudyVw.cinepause(1, 3).Displayed;
                PageLoadWait.WaitForFrameLoad(15);
                if (cine161 && cine162 && cine163)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 17
                PageLoadWait.WaitForElementToDisplay(StudyVw.CineLabelFrameRate(1, 1), 90);
                fps = StudyVw.CineLabelFrameRate(1, 1).Text;
                fp = defFPS + "/" + defFPS + " fps";
                if (fp.Equals(fps))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 18
                login.Logout();
                login.LoginIConnect(username, password);
                if (login.LogoutBtn().Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 19
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                bool step19 = false;
                SelectElement modality3 = userpreferences.ModalityDropDown();
                for (int i = 0; i < modality3.Options.Count; i++)
                {
                    modality3.SelectByIndex(i);
                    PageLoadWait.WaitForPageLoad(10);
                    if (userpreferences.AutoCINE_OFF().Selected)
                        step19 = true;
                    else
                    {
                        step19 = false;
                        break;
                    }
                }
                if (step19)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 20
                userpreferences.ModalityDropDown().SelectByText(Modality[0]);
                userpreferences.AutoCINE_ON().Click();
                userpreferences.LayoutDropDown().SelectByText("1x2");
                userpreferences.ClearText("id", "DefaultFrameRateTextBox");
                userpreferences.SetText("id", "DefaultFrameRateTextBox", "3");
                userpreferences.SavePreferenceBtn().Click();
                userpreferences.CloseUserPreferences();
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText(Modality[0]);
                bool step20 = false;
                step20 = userpreferences.AutoCINE_ON().Selected;
                string MRLyt20 = userpreferences.LayoutDropDown().SelectedOption.Text;
                defFPS = userpreferences.GetTextFromTextBox("id", "DefaultFrameRateTextBox");
                if (step20 && (MRLyt20 == "1x2") && (defFPS == "3"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                userpreferences.CancelUserPreferences();

                //step 21
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                studies.LaunchStudy(isAutoCineEnabled: true);
                PageLoadWait.WaitForFrameLoad(15);
                StudyVw.ClickElement("Series Viewer 1x2");
                bool cine211 = StudyVw.cinepause(1, 1).Displayed;
                bool cine212 = StudyVw.cinepause(1, 2).Displayed;
                PageLoadWait.WaitForFrameLoad(15);
                if (cine211 && cine212)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 22
                PageLoadWait.WaitForElementToDisplay(StudyVw.CineLabelFrameRate(1, 1), 90);
                fps = StudyVw.CineLabelFrameRate(1, 1).Text;
                fp = defFPS + "/" + defFPS + " fps";
                if (fp.Equals(fps))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 23
                result.steps[++executedSteps].status = "Not Automated";
                login.CloseStudy();

                //step 24
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.CreateDomain(TestdomainD2, TestdomainAdminD2, datasources);
                domainmanagement.ClickSaveNewDomain();
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainD2, TestRoleR2, RoleCred: 99, GrantAccess: 99);
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.CreateUser(TestuserU2, TestRoleR2);
                login.Logout();
                login.LoginIConnect(TestuserU2, TestuserU2);
                if (login.LogoutBtn().Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 25
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[1]);
                studies.SelectStudy("Modality", Modality[1]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step25 = false;
                step25 = studies.CompareImage(result.steps[executedSteps], view);
                if (step25)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 26
                login.CloseStudy();
                studies.ClearButton().Click();
                studies.SearchStudy(LastName: LastName[3]);
                studies.SelectStudy("Patient ID", PatientID[1]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step26 = false;
                step26 = studies.CompareImage(result.steps[executedSteps], view);
                if (step26)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 27
                login.CloseStudy();
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText(Modality[2]);
                userpreferences.AutoCINE_ON().Click();
                userpreferences.LayoutDropDown().SelectByText("1x3");
                userpreferences.SavePreferenceBtn().Click();
                userpreferences.CloseUserPreferences();
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText(Modality[2]);
                bool step27 = false;
                step27 = userpreferences.AutoCINE_ON().Selected;
                string MRLyt27 = userpreferences.LayoutDropDown().SelectedOption.Text;
                userpreferences.CancelUserPreferences();
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(LastName: LastName[3]);
                studies.SelectStudy("Patient ID", PatientID[1]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                bool cine273 = StudyVw.cinepause(1, 3).Displayed;
                if (step27 && cine273 && (MRLyt27 == "1x3"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 28
                StudyVw.DragThumbnailToViewport(3, Locators.ID.SeriesViewer1_1x1);
                PageLoadWait.WaitForPageLoad(5);
                StudyVw.cineplay(1, 2).Click();
                bool cine281 = StudyVw.cinepause(1, 1).Displayed;
                bool cine282 = StudyVw.cinepause(1, 2).Displayed;
                bool cine283 = StudyVw.cinepause(1, 3).Displayed;
                if (cine281 && cine282 && cine283)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 29
                StudyVw.cineGroupPauseBtn().Click();
                bool cine291 = !StudyVw.cinepause(1, 1).Displayed;
                bool cine292 = !StudyVw.cinepause(1, 2).Displayed;
                bool cine293 = !StudyVw.cinepause(1, 3).Displayed;
                if (cine291 && cine292 && cine293)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 30
                StudyVw.ClickElement("Series Viewer 2x3");
                bool cine303 = false;
                cine303 = StudyVw.SeriesViewer_2X3().Displayed;
                if (cine303)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 31
                StudyVw.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                StudyVw.DragThumbnailToViewport(2, Locators.ID.SeriesViewer2_2x3);
                StudyVw.cinestop(1, 3).Click();
                StudyVw.DragThumbnailToViewport(1, Locators.ID.SeriesViewer3_2x3);
                PageLoadWait.WaitForPageLoad(5);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step31 = false;
                step31 = studies.CompareImage(result.steps[executedSteps], view);
                if (step31)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 32
                StudyVw.DragThumbnailToViewport(3, Locators.ID.SeriesViewer4_2x3);
                StudyVw.DragThumbnailToViewport(3, Locators.ID.SeriesViewer5_2x3);
                StudyVw.DragThumbnailToViewport(3, Locators.ID.SeriesViewer6_2x3);
                PageLoadWait.WaitForPageLoad(5);
                bool cine321 = StudyVw.cinepause(2, 1).Displayed;
                bool cine322 = StudyVw.cinepause(2, 2).Displayed;
                bool cine323 = StudyVw.cinepause(2, 3).Displayed;
                if (cine321 && cine322 && cine323)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 33
                StudyVw.SeriesViewer_1X2().Click();
                bool step33 = false;
                step33 = StudyVw.cineGroupPauseBtn().Displayed;
                if (step33)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// </summary>
        ///Exam mode	
        /// </summary> 

        public TestCaseResult Test_27485(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            Studies studies;
            StudyViewer studyViewer;
            UserPreferences UserPref = new UserPreferences();
            TestCaseResult result = null;

            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Pre-Condition:
                /*In a new browser session login in ica as Administrator
                Create a new domain TD1, role TR1, user TU1
                Login as TU1*/
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String[] datasource = null;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionIDList.Split(':');
                String Domain = "TD1_" + new Random().Next(1000);
                String role = "TR1_" + new Random().Next(1000);
                String user = "TU1_" + new Random().Next(1000);

                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.CreateDomain(Domain, role, datasources: datasource);
                domainmanagement.ClickSaveNewDomain();
                PageLoadWait.WaitForPageLoad(20);
                /*rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.CreateRole(Domain, role, "Physician");*/
                PageLoadWait.WaitForPageLoad(20);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateDomainAdminUser(user, Domain, 0, "", 0);
                //usermanagement.CreateUser(user, Domain, role);
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();


                //Step-1: Verify Exam mode option in Domain for every modality
                login.LoginIConnect(user, user);
                PageLoadWait.WaitForPageLoad(20);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(30);


                bool step1 = true;
                for (int i = 0; i < domainmanagement.ModalityDropDown().Options.Count; i++)
                {
                    domainmanagement.ModalityDropDown().SelectByIndex(i);
                    //input[id$='_ExamModeRadioButtons_0']
                    try
                    {
                        if (domainmanagement.ExamMode("0").GetAttribute("checked") != null)
                        {
                            step1 = false;
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
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

                //Step-2:
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step_6)
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


                //Step-3: Verify Exam Play mode
                int j3 = 0;
                bool step3 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j3].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step3 = true;
                        break;

                    }
                    j3++;
                }

                if (step3 == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Close study
                //In Domain management for the modality to which the listed study belongs
                //Enable exam mode
                //add exam mode icon in review toolbar
                //Reload the study in viewer
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.ModalityDropDown().SelectByText("US");
                domainmanagement.ExamMode("0").Click();
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step4)
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

                //Step-5: Verify the Global Stack tool

                int j5 = 0;
                bool step5 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j5].GetAttribute("title");
                    if (title.Contains("Global Stack"))
                    {
                        step5 = true;
                        break;

                    }
                    j5++;
                }

                if (step5 == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Select a viewport with single frame/image and verify cine toolbar
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                studyViewer = studies.LaunchStudy();
                studyViewer.SeriesViewer_1X1().Click();
                if (!studyViewer.CineToolbar(1, 3).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7:Select Exam Play mode
                int j7 = 0;
                bool step7_1 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j7].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step7_1 = true;
                        break;

                    }
                    j7++;
                }
                studyViewer.SeriesViewer_2X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineToolbar(2, 1));
                if (studyViewer.CineToolbar(2, 1).Displayed && step7_1 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Verify the Global Stack tool
                int j8 = 0;
                bool step8 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j8].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step8 = true;
                        break;

                    }
                    j8++;
                }
                if (step8 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Click Cine
                studyViewer.SeriesViewer_1X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-11: Pause cine
                studyViewer.cinepause(1, 1).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.cineplay(1, 1));
                if (studyViewer.cineplay(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Start cine
                studyViewer.cineplay(1, 1).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13: Close study and Verify Exam mode option in Role
                //Step-14: In Role management for the modality to which the listed study belongs
                //Enable Exam mode
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(role);
                rolemanagement.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                rolemanagement.ModalityDropDown().SelectByText("US");
                rolemanagement.ExamMode("0").Click();
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                ExecutedSteps++;
                ExecutedSteps++;


                //Step-15: from studylist Search and load for a multiframe study with multiple images
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[2]);
                studies.SelectStudy("Accession", Accession[2]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step15)
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

                //Step-16: Click Cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-18: Select Exam Play mode
                int j18 = 0;
                bool step18 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j18].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step18 = true;
                        break;

                    }
                    j18++;
                }
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);

                if (step18 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-19: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-20:Verify the thumbnail highlighting
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                if (studyViewer.GetInnerAttribute(studyViewer.SeriesViewer_1X1(), "src", '&', "seriesUID")
                    == studyViewer.GetInnerAttribute(studyViewer.ActiveThumbnail(), "src", '&', "seriesUID"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-21: Verify Exam mode option in User preferences
                studyViewer.cinestop(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.UserPreference);
                bool step21 = true;
                for (int i = 0; i < UserPref.ModalityDropDown().Options.Count; i++)
                {
                    UserPref.ModalityDropDown().SelectByIndex(i);
                    //input[id$='_ExamModeRadioButtons_0']
                    try
                    {
                        if (UserPref.ModalityDropDown().Options.Equals("US"))
                        {
                            step21 = true;
                        }
                        else
                        {
                            if (domainmanagement.ExamMode("0").GetAttribute("checked") != null)
                            {
                                step21 = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
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

                //Step-22:In User preferences for the modality to which the listed study belongs Enable exam mode
                //Close study and Search and load for a study with multiple images in single series

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                UserPref.SwitchToToolBarUserPrefFrame();
                UserPref.ModalityDropDown().SelectByText("MR");
                UserPref.ExamMode("0").Click();
                UserPref.SaveToolBarUserPreferences();
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy("Accession", Accession[4]);
                studies.SelectStudy("Accession", Accession[4]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
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


                //Step-23: Select Exam Play mode
                int j23 = 0;
                bool step23 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j23].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step23 = true;
                        break;

                    }
                    j23++;
                }
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                if (step23 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24: Click Cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-25: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-26: Close study and Search and load for a study with multiple images in multiple series
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy("Accession", Accession[4]);
                studies.SelectStudy("Accession", Accession[4]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step26 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step26)
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


                //Step-27: Select Exam Play mode
                studyViewer.SeriesViewer_1X2().Click();
                int j27 = 0;
                bool step27 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j27].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step27 = true;
                        break;

                    }
                    j27++;
                }
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                if (step27 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-28: Scroll images in series till you reach the last image in series now scroll once more
                studyViewer.DragScroll(1, 2, 21, 21);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                String uid_3 = studyViewer.GetInnerAttribute(studyViewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if (uid_3.Contains("One_1.3.12.2.1107.5.2.7.20410.30000006052405540078100000008PS_0"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-29: Click Cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 2));
                if (studyViewer.cinepause(1, 2).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-30: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-31: Stop cine when the images from second series are displayed
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-32: Close study and In domain management set delay on frame and exam mode to ON for SC save changes
                //Search and load for a study with multiple series and a secondary capture
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(30);
                //****************sset delay on frame
                domainmanagement.ModalityDropDown().SelectByText("SC");
                domainmanagement.ExamMode("0").Click();
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[3]);
                studies.SelectStudy("Accession", Accession[3]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step32 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
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

                //Step-33: Select Exam Play mode
                int j33 = 0;
                bool step33 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j33].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step33 = true;
                        break;

                    }
                    j33++;
                }
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                if (step33 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34: Click Cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-36: Close study and Search and load for a study with multiple frames and a multiframe secondary capture
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy("Accession", Accession[3]);
                studies.SelectStudy("Accession", Accession[3]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step36 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step36)
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

                //Step-37: Select Exam Play mode
                int j37 = 0;
                bool step37 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j37].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step37 = true;
                        break;

                    }
                    j37++;
                }
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                if (step37 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-38: Click Cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-39: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-40: Close study
                /*In User preferences for the modality to which the listed study belongs
                set the Cine Single Frame Delay Time to 5sec
                Delay on Single frame Off
                Exam mode ON
                Search and load for a study with multiple series and multiple modalities*/

                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.CineFrameDelay().Clear();
                UserPref.CineFrameDelay().SendKeys("5");
                UserPref.ModalityDropDown().SelectByText("MR");
                UserPref.ExamMode("0").Click();
                UserPref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(30);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[5]);
                studies.SelectStudy("Accession", Accession[5]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step40 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step40)
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


                //Step-41: Select Exam Play mode
                int j41 = 0;
                bool step41 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j41].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step41 = true;
                        break;

                    }
                    j41++;
                }
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                if (step41 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-42: Click Cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-43: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-44: Close study and In User preferences for the modality to which the listed study belongs
                //set the Cine Single Frame Delay Time to 5sec
                //Delay on Single frame On
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                UserPref.CineFrameDelay().SendKeys("5");
                UserPref.ModalityDropDown().SelectByText("CR");
                UserPref.ExamMode("0").Click();
                UserPref.CloseUserPreferences();
                ExecutedSteps++;

                //Step-45: load the study in viewer with PR and KO
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(30);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[6]);
                studies.SelectStudy("Accession", Accession[6]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step45 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step45)
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

                //Step-46: Select Exam Play mode
                int j46 = 0;
                bool step46 = false;
                foreach (IWebElement toolgroup in studyViewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul>li:nth-child(2)"));
                    string title = dropdown[j46].GetAttribute("title");
                    if (title.Contains("Exam Mode"))
                    {
                        step46 = true;
                        break;

                    }
                    j46++;
                }
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                if (step46 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-47: Scroll through the images.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-48: Scroll to an image from last series set Exam mode to OFF
                studyViewer.DragScroll(1, 1, 2, 2);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step48 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());
                if (step48)
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

                //Step-49: scroll images
                studyViewer.DragScroll(1, 2, 21, 21);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step49 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());
                if (step49)
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


                //Step-50: set exam mode to ON and start Cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 2));
                if (studyViewer.cinepause(1, 2).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-51: Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-52: Close study
                //Load a study with multiple single image series
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy(patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step52 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step52)
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

                //Step-53: CLick group play
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (!studyViewer.CineToolbar(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-54: Double click on viewport                
                var action = new Actions(BasePage.Driver);
                action.DoubleClick(studyViewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step54 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step54)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
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

        /// </summary>
        ///Play group	
        /// </summary>
        public TestCaseResult Test_27486(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables              
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String StudyIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");

                String[] AccessionID = AccessionIDList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String[] Modality = ModalityList.Split(':');
                String[] StudyID = StudyIDList.Split(':');

                //Step 1 - In User preferences set the thumbnail split to image for the modality to which the listed study belongs
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                ModalityDropdown().SelectByText(Modality[0]);
                userpref.AutoCINE_OFF().Click();
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 2 - set series layout to 1x2
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionID[0], patientID: PatientID[0]);
                studies.SelectStudy("Accession", AccessionID[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                ExecutedSteps++;

                //Step 3 - Load first multiframe image in first viewport, second multiframe image in second viewport             
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 4 - Click group play
                viewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Verify previous/next group
                if (!viewer.cinePrevGroupBtn().Enabled && viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 7 - Click Next group button
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Verify previous/next group
                if (viewer.cinePrevGroupBtn().Enabled && viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9 - Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 10 - Click Previous group button
                viewer.cinePrevGroupBtn().Click();
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 - Verify previous/next group
                if (!viewer.cinePrevGroupBtn().Enabled && viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 13 - Click Next group button till the last group is loaded in viewport
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    viewer.SeriesViewer_1X2().GetAttribute("src").Contains("Images/blankImage"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14 - Verify previous/next group
                if (viewer.cinePrevGroupBtn().Enabled && !viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - Verify cine toolbar on empty viewport
                if (!viewer.IsElementVisible(viewer.By_CineToolbar(1, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16 - Verify the group icons when empty viewport is selected
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                if (IsElementPresent(viewer.By_CineGroupButtons()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseStudy();

                ////Step 17 - Set Automatically start cine to OFF
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                ModalityDropdown().SelectByText(Modality[0]);
                userpref.AutoCINE_OFF().Click();
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 18 - set series layout to 2x3
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID[0]);
                studies.SelectStudy("Study ID", StudyID[0]);
                viewer = StudyViewer.LaunchStudy();

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                ExecutedSteps++;

                //Step 19 - Load first series in first viewport, second series in second viewport
                ExecutedSteps++;

                //Step 20 - Click group play
                viewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21 - Verify previous/next group
                if (!viewer.cinePrevGroupBtn().Enabled && viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 22 - Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 23 - Click Next group button
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(5000);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24 - Verify previous/next group
                if (viewer.cinePrevGroupBtn().Enabled && viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 25 - Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 26 - Click Previous group button
                viewer.cinePrevGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(5000);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 27 - Verify previous/next group
                if (!viewer.cinePrevGroupBtn().Enabled && viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 28 - Verify the images being played
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 29 - Click Next group button
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(5000);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 30 - Click Next group button
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(5000);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 31 - Verify previous/next group
                if (viewer.cinePrevGroupBtn().Enabled && !viewer.cineNextGroupBtn().Enabled)
                {
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

        /// </summary>
        ///Prev/next frame	
        /// </summary> 

        public TestCaseResult Test_27487(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Studies studies = null;
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement = null;
            UserPreferences userpreferences = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {

                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String domain = Config.adminGroupName;
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastName = LastNameList.Split(':');
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Modality = ModalityList.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");

                //step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText(Modality[0]);
                userpreferences.ThumbnailSplittingImageRadioBtn().Click();
                userpreferences.AutoCINE_OFF().Click();
                userpreferences.SavePreferenceBtn().Click();
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText(Modality[0]);
                bool split = userpreferences.ThumbnailSplittingImageRadioBtn().Selected;
                bool autoC = userpreferences.AutoCINE_OFF().Selected;
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: LastName[0], AccessionNo: AccessionID);
                studies.SelectStudy("Accession", AccessionID);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                var view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step1 = studies.CompareImage(result.steps[executedSteps], view);
                if (step1)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 2
                StudyVw.ClickElement("Series Viewer 1x2");
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step2 = studies.CompareImage(result.steps[executedSteps], view);
                if (step2)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 3
                StudyVw.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                StudyVw.DragThumbnailToViewport(2, StudyVw.GetControlId("SeriesViewer2-1X2"));
                PageLoadWait.WaitForPageLoad(5);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step3 = studies.CompareImage(result.steps[executedSteps], view);
                if (step3)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 4
                StudyVw.cineGroupPlayBtn().Click();
                bool vwprt41 = StudyVw.cinepause(1, 1).Displayed;
                bool vwprt42 = StudyVw.cinepause(1, 2).Displayed;
                if (vwprt41 && vwprt42)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 5
                result.steps[++executedSteps].status = "Not Automated";

                //step 6
                StudyVw.cineNextFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step6 = studies.CompareImage(result.steps[executedSteps], view);
                String uid_imgNum6 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "imagenum");
                String uid_imgIndx6 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "imageindex");
                bool vwprt61 = !StudyVw.cinepause(1, 1).Displayed;
                if ((uid_imgNum6 == "2") && (uid_imgIndx6 == "3") && vwprt61)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 7
                StudyVw.cinePrevFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_7 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt71 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_7.Contains("154911_1") && vwprt71)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 8
                StudyVw.cineNextFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_8 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt81 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_8.Contains("154911_1") && vwprt81)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 9
                StudyVw.cineplay(1, 1).Click();
                bool vwprt91 = StudyVw.cinepause(1, 1).Displayed;
                if (vwprt91)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 10
                StudyVw.cinePrevFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_10 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt101 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_10.Contains("154911_1") && vwprt101)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 11
                login.CloseStudy();
                login.SearchStudy(LastName: LastName[1], studyID: StudyID);
                login.SelectStudy("Modality", "XA");
                login.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step11 = studies.CompareImage(result.steps[executedSteps], view);
                if (step11)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 12
                StudyVw.ClickElement("Series Viewer 1x2");
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step12 = studies.CompareImage(result.steps[executedSteps], view);
                if (step12)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 13
                StudyVw.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                StudyVw.DragThumbnailToViewport(2, StudyVw.GetControlId("SeriesViewer2-1X2"));
                PageLoadWait.WaitForPageLoad(5);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step13 = studies.CompareImage(result.steps[executedSteps], view);
                if (step13)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 14
                StudyVw.cineplay(1, 1).Click();
                bool vwprt14 = StudyVw.cinepause(1, 1).Displayed;
                if (vwprt14)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 15
                result.steps[++executedSteps].status = "Not Automated";

                //step 16
                StudyVw.cineNextFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_16 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt16 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_16.Contains("154911_1") && vwprt16)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 17
                StudyVw.cinePrevFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_17 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt17 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_17.Contains("154911_1") && vwprt17)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 18
                for (int i = 1; i < 59; i++)
                {
                    PageLoadWait.WaitForPageLoad(5);
                    StudyVw.cineNextFramebtn(1, 1).Click();
                }
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_18 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt18 = !StudyVw.cinepause(1, 1).Displayed;
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step18 = studies.CompareImage(result.steps[executedSteps], view);
                if (uid_18.Contains("154911_1") && vwprt18 && step18)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 19
                ////////////////////////////////////////////////////
                StudyVw.cineplay(1, 1).Click();
                bool vwprt19 = StudyVw.cinepause(1, 1).Displayed;
                if (vwprt19)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 20
                StudyVw.cineplay(1, 1).Click();
                bool vwprt20 = StudyVw.cinepause(1, 1).Displayed;
                if (vwprt20)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 21
                StudyVw.cinePrevFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_21 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt21 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_21.Contains("154911_1") && vwprt21)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 22
                login.CloseStudy();
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.SearchDomain(domain);
                domainmanagement.SelectDomain(domain);
                domainmanagement.ClickEditDomain();
                domainmanagement.ModalityDropDown().SelectByText(Modality[1]);
                domainmanagement.ExamModeON().Click();
                domainmanagement.AddAllToolsToToolBar();
                domainmanagement.ClickSaveEditDomain();
                studies = login.Navigate<Studies>();
                studies.SearchStudy("lastname", LastName[2]);
                studies.SelectStudy("Patient ID", PatientID);
                studies.LaunchStudy();
                PageLoadWait.WaitForPageLoad(5);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step22 = studies.CompareImage(result.steps[executedSteps], view);
                if (step22)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 23
                StudyVw.ClickElement("Series Viewer 1x2");
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step23 = studies.CompareImage(result.steps[executedSteps], view);
                if (step23)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 24
                StudyVw.DragThumbnailToViewport(1, Locators.ID.SeriesViewer1_1x1);
                StudyVw.DragThumbnailToViewport(2, StudyVw.GetControlId("SeriesViewer2-1X2"));
                PageLoadWait.WaitForPageLoad(5);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step24 = studies.CompareImage(result.steps[executedSteps], view);
                if (step24)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 25
                StudyVw.ClickElement("Exam Mode");
                bool exmEnbld25 = false;
                IWebElement ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                List<IWebElement> Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld25 = tool.GetAttribute("class").Contains("notSelected32 enabledOnCine toggleItem_On");
                        break;
                    }
                }
                if (exmEnbld25)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 26
                StudyVw.cineplay(1, 1).Click();
                bool vwprt26 = StudyVw.cinepause(1, 1).Displayed;
                if (vwprt26)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 27
                result.steps[++executedSteps].status = "Not Automated";

                //step 28
                StudyVw.cineNextFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_28 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt28 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_28.Contains("154911_1") && vwprt28)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 29
                StudyVw.cinePrevFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_29 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt29 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_29.Contains("154911_1") && vwprt29)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 30
                for (int i = 1; i < 9; i++)
                {
                    PageLoadWait.WaitForPageLoad(5);
                    StudyVw.cineNextFramebtn(1, 1).Click();
                }
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_30 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt30 = !StudyVw.cinepause(1, 1).Displayed;
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step30 = studies.CompareImage(result.steps[executedSteps], view);
                if (uid_30.Contains("154911_1") && vwprt30 && step30)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 31
                StudyVw.cineplay(1, 1).Click();
                bool vwprt31 = StudyVw.cinepause(1, 1).Displayed;
                if (vwprt31)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 32
                StudyVw.cinePrevFramebtn(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                string uid_32 = viewer.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                bool vwprt32 = !StudyVw.cinepause(1, 1).Displayed;
                if (uid_32.Contains("154911_1") && vwprt32)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// </summary>
        ///Prev/next clip	
        /// </summary>

        public TestCaseResult Test_27488(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String[] Accession = AccessionList.Split(':');


            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                //Step-1
                //"In User preferences set the thumbnail split to series for the modality to which the listed study belongs
                //Search and load for a study with multiple images in multiple series"

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Studies study = (Studies)login.Navigate("Studies");

                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("MR");
                PageLoadWait.WaitForPageLoad(20);
                userpref.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                //Accession[0]=""
                study.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                study.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));
                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                    viewer.SeriesViewer_1X2().Displayed &&
                    viewer.SeriesViewer_2X1().Displayed &&
                    viewer.SeriesViewer_2X2().Displayed &&
                    viewer.cineplay(1, 1).Displayed &&
                    viewer.cineplay(1, 2).Displayed &&
                    viewer.cineplay(2, 1).Displayed &&
                    viewer.cineplay(2, 2).Displayed &&
                    viewer.cineGroupPlayBtn().Displayed)
                {
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
                //set series layout to 1x2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Layout should be set
                if (viewer.SeriesViewPorts().Count == 2)
                {
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
                //Load first series in first viewport, second series in second viewport and so on

                var action3 = new Actions(BasePage.Driver);

                action3.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                action3 = new Actions(BasePage.Driver);

                action3.DragAndDrop(viewer.Thumbnails()[1], viewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //All the images should load in different viewports

                String Thumbnail_SeriesID_1 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");

                String Thumbnail_SeriesID_2 = viewer.GetInnerAttribute(viewer.Thumbnails()[1], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ClusterViewID");

                PageLoadWait.WaitForFrameLoad(30);

                //IList<IWebElement> loadedThumbnail = BasePage.Driver.FindElements(By.CssSelector("div[class$='loadedThumbnail']"));

                if (//viewer.LoadedThumbnails().Count == 2 &&
                    viewer.ThumbnailIndicator()[0].Displayed == true &&
                    viewer.ThumbnailIndicator()[1].Displayed == true && 
                    viewer.SeriesViewPorts().Count == 2 &&
                    Thumbnail_SeriesID_1 != Thumbnail_SeriesID_2 &&
                    Thumbnail_SeriesID_1 != null && Thumbnail_SeriesID_2 != null &&
                    Viewerport_ClusterViewID_1.Contains(Thumbnail_SeriesID_1) &&
                    Viewerport_ClusterViewID_2.Contains(Thumbnail_SeriesID_2))
                {
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
                //Click Cine

                viewer.cineplay(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 1)));
                PageLoadWait.WaitForCineToPlay(1, 1);

                string ClassAttribute = "ui-droppable";
                string ClassAttribute2 = "svViewerImg";


                //Cine should start in the viewport

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                    viewer.cinepause(1, 1).Displayed == true &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
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
                //Verify the images being played
                //Cine plays all images from the series and never stops

                if (
                    viewer.SeriesViewer_1X1().Displayed == false &&
                    viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                    viewer.cinepause(1, 1).Displayed == true &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
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
                //Click Next clip button
                viewer.cineNextClipBtn(1, 1).Click();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForCineToPlay(1, 1);
                //the next series should load in the viewport 
                //and cine should continue playing from the first image


                if (
                  viewer.LoadedThumbnails().Count == 1 &&
                  viewer.ThumbnailIndicator()[0].Displayed == false &&
                  viewer.ThumbnailIndicator()[1].Displayed == true &&
                  viewer.SeriesViewer_1X1().Displayed == false &&
                  viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                  viewer.cinepause(1, 1).Displayed == true &&
                  viewer.cinestop(1, 1).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Click Previous clip button

                viewer.cinePrevClipBtn(1, 1).Click();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForCineToPlay(1, 1);

                //the previous series should load in the viewport and 
                //cine should continue playing from the first image

                if (
                  viewer.LoadedThumbnails().Count == 2 &&
                  viewer.ThumbnailIndicator()[0].Displayed == true &&
                  viewer.ThumbnailIndicator()[1].Displayed == true &&

                  viewer.SeriesViewer_1X1().Displayed == false &&
                  viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                  viewer.cinepause(1, 1).Displayed == true &&
                  viewer.cinestop(1, 1).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
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

                //Click Next clip button
                viewer.cineNextClipBtn(1, 1).Click();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForCineToPlay(1, 1);
                //the next series should load in the viewport 
                //and cine should continue playing from the first image

                if (
                  viewer.LoadedThumbnails().Count == 1 &&
                  viewer.ThumbnailIndicator()[0].Displayed == false &&
                  viewer.ThumbnailIndicator()[1].Displayed == true &&
                  viewer.SeriesViewer_1X1().Displayed == false &&
                  viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                  viewer.cinepause(1, 1).Displayed == true &&
                  viewer.cinestop(1, 1).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
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
                //Click Previous clip button

                viewer.cinePrevClipBtn(1, 1).Click();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForFrameLoad(30);

                //the previous series should load in the viewport and 
                //cine should continue playing from the first image

                if (
                  viewer.LoadedThumbnails().Count == 2 &&
                  viewer.ThumbnailIndicator()[0].Displayed == true &&
                  viewer.ThumbnailIndicator()[1].Displayed == true &&
                  viewer.SeriesViewer_1X1().Displayed == false &&
                    //viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    //viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                    //viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                  viewer.cinepause(1, 1).Displayed == true &&
                  viewer.cinestop(1, 1).Displayed == true &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
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
                //Stop cine and click next clip

                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                viewer.cineNextClipBtn(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineNextClipBtn(1, 1)));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);


                //The next series should load in viewer

                if (viewer.LoadedThumbnails().Count == 1 &&
                    viewer.ThumbnailIndicator()[0].Displayed == false &&
                    viewer.ThumbnailIndicator()[1].Displayed == true &&
                    viewer.SeriesViewer_1X1().Displayed == true &&
                    //viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == false &&
                    //viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                    //viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) == false &&
                    viewer.cinepause(1, 1).Displayed == false &&
                    viewer.cinestop(1, 1).Displayed == false &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 0) //#### stopped not pause
                {
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
                //keep clicking next clip till last series is loaded in viewport click next clip one more time

                viewer.cineNextClipBtn(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineNextClipBtn(1, 1)));

                viewer.cineNextClipBtn(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineNextClipBtn(1, 1)));

                viewer.cineNextClipBtn(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineNextClipBtn(1, 1)));

                viewer.cineNextClipBtn(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineNextClipBtn(1, 1)));

                //the first series should load in viewport

                if (viewer.Thumbnails()[0].GetCssValue("border-top-color").Equals("rgba(255, 160, 0, 1)") &&
                    viewer.SeriesViewer_1X1().Displayed == true &&
                    viewer.LoadedThumbnails().Count == 2 &&
                    viewer.ThumbnailIndicator()[1].Displayed == true &&
                    viewer.ThumbnailIndicator()[0].Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //click previous clip
                viewer.cinePrevClipBtn(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineNextClipBtn(1, 1)));

                var wait2 = new WebDriverWait(BasePage.Driver, TimeSpan.FromSeconds(10));
                wait2.Until(driver => !viewer.ThumbnailIndicator()[0].Displayed);

                //the last series should load in viewport
                if (viewer.Thumbnails()[(viewer.Thumbnails().Count - 1)].GetCssValue("border-top-color").Equals("rgba(255, 160, 0, 1)") &&
                    viewer.SeriesViewer_1X1().Displayed == true &&
                    viewer.LoadedThumbnails().Count == 2 &&
                    viewer.ThumbnailIndicator()[1].Displayed == true &&
                    viewer.ThumbnailIndicator()[(viewer.Thumbnails().Count - 1)].Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                study.CloseStudy();
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

        /// </summary>
        ///Cardiology report	
        /// </summary>
        public TestCaseResult Test_27489(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            Studies studies = new Studies();
            StudyViewer studyViewer = new StudyViewer();
            UserPreferences UserPref = new UserPreferences();

            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domainmanagement = new DomainManagement();

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                string StudyPath = "D:\\New-ICA\\Testdata-SC\\SC\\LumisysFilmScanner";

                //Pre-conditions
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.GetButton(ServiceTool.ModifyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                //Enable cardiology report
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Checked = true;
                servicetool.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report), ServiceTool.EnableFeatures.Name.CardioReports, 1).Checked = true;
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Step-1: Search and load for a study with a report
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");

                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                //Accession[0]=REP12312
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_96);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step1)
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

                //Step-2: click on Report icon
                studyViewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                if (studyViewer.ReportContentContainer().Displayed)
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

                //Step-3: click on the close icon on the report viewer
                studyViewer.CloseReport().Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                if (!studyViewer.ReportContentContainer().Displayed)
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

                //Step-4: Close study and Search and load for a study with multiple reports
                studyViewer.CloseStudy();
                //Accession[0]=REP12311
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_96);
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step4)
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


                //Step-5: click on Report icon
                studyViewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                if (studyViewer.ReportContentContainer().Displayed)
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

                //Step-6: verify the report list              
                bool ReportExist = studyViewer.IsElementVisible(studyViewer.By_ReportContainer());
                bool ReportMaxIconExist = studyViewer.ReportFullScreenIcon().Displayed;
                studyViewer.ViewerReportListButton().Click();
                Dictionary<int, string[]> ReportListDetails = studyViewer.StudyViewerListResults("StudyPanel", "report", 1);

                //Get Date Column from Cardio Report Table
                string[] DateValues = GetColumnValues(ReportListDetails, "Date", GetColumnNames(1));
                DateTime[] Dates = studyViewer.ConvertStringToDate(DateValues);

                //Validate if date is in descending order
                bool step6 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));

                if (ReportExist && ReportListDetails.Count > 1 && ReportMaxIconExist && step6)
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

                //Step-7: click on a report listed
                //Get column names and row details of first report in the report list
                String[] reportColumnNames = studyViewer.StudyViewerListColumnNames("StudyPanel", "report", 1);
                String[] reportColumnValues = BasePage.GetColumnValues(ReportListDetails, "Title", reportColumnNames);
                Dictionary<string, string> FirstreportDetails = studyViewer.StudyViewerListMatchingRow("Title", reportColumnValues[0], "StudyPanel", "report");
                try
                {
                    //Select the first report in report list
                    studyViewer.SelectItemInStudyViewerList("Title", reportColumnValues[0], "StudyPanel", "report");
                    String Report_Patientname7 = studyViewer.GetPatientDetailsFromReport("Patient:", 2);
                    String Report_MRN7 = studyViewer.GetPatientDetailsFromReport("MRN:");
                    if (Report_Patientname7.ToUpper().Contains(Firstname) && Report_Patientname7.ToUpper().Contains(Lastname))
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
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: click on the close icon on the report viewer
                PageLoadWait.WaitForFrameLoad(60);
                studyViewer.CloseReport().Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                if (!studyViewer.ReportContentContainer().Displayed)
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

                //Step-9: click on Report icon
                studyViewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                if (studyViewer.ReportContentContainer().Displayed)
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

                //Setp-10: verify the report list
                studyViewer.TitlebarReportIcon().Click();
                studyViewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyViewer.By_ReportContainer()));
                bool ReportExist10 = studyViewer.IsElementVisible(studyViewer.By_ReportContainer());
                bool ReportMaxIconExist10 = studyViewer.ReportFullScreenIcon().Displayed;
                studyViewer.ViewerReportListButton().Click();
                ReportListDetails = studyViewer.StudyViewerListResults("StudyPanel", "report", 1);

                //Get Date Column from Cardio Report Table
                DateValues = GetColumnValues(ReportListDetails, "Date", GetColumnNames(1));
                Dates = studyViewer.ConvertStringToDate(DateValues);

                //Validate if date is in descending order
                bool step10 = (Dates == null || Dates.Length == 0) ? false : Dates.SequenceEqual((Dates.OrderByDescending(q => q)));

                if (ReportExist10 && ReportListDetails.Count > 1 && ReportMaxIconExist10 && step10)
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


                //step-11: click on a report listed
                reportColumnNames = studyViewer.StudyViewerListColumnNames("StudyPanel", "report", 1);
                reportColumnValues = BasePage.GetColumnValues(ReportListDetails, "Title", reportColumnNames);
                FirstreportDetails = studyViewer.StudyViewerListMatchingRow("Title", reportColumnValues[0], "StudyPanel", "report");
                try
                {
                    //Select the first report in report list
                    studyViewer.SelectItemInStudyViewerList("Title", reportColumnValues[0], "StudyPanel", "report");
                    String Report_Patientname11 = studyViewer.GetPatientDetailsFromReport("Patient:", 2);
                    String Report_MRN11 = studyViewer.GetPatientDetailsFromReport("MRN:");
                    if (Report_Patientname11.ToUpper().Contains(Firstname) && Report_Patientname11.ToUpper().Contains(Lastname))
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
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: click on another report listed
                PageLoadWait.WaitForFrameLoad(60);
                studyViewer.TitlebarReportIcon().Click();
                studyViewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studyViewer.By_ReportContainer()));
                reportColumnNames = studyViewer.StudyViewerListColumnNames("StudyPanel", "report", 1);
                reportColumnValues = BasePage.GetColumnValues(ReportListDetails, "Title", reportColumnNames);
                try
                {
                    FirstreportDetails = studyViewer.StudyViewerListMatchingRow("Title", reportColumnValues[0], "StudyPanel", "report");
                    //Select the first report in report list
                    studyViewer.SelectItemInStudyViewerList("Title", reportColumnValues[0], "StudyPanel", "report");
                    String Report_Patientname12 = studyViewer.GetPatientDetailsFromReport("Patient:", 2);
                    String Report_MRN12 = studyViewer.GetPatientDetailsFromReport("MRN:");
                    if (Report_Patientname12.ToUpper().Contains(Firstname) && Report_Patientname12.ToUpper().Contains(Lastname))
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
                catch (Exception e)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
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

        /// </summary>
        ///Window High/Low	
        /// </summary> 

        public TestCaseResult Test_27490(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables              
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Setup Test Step Description                

                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String StudyIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDescription");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");

                String[] AccessionID = AccessionIDList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String[] Modality = ModalityList.Split(':');
                String[] StudyID = StudyIDList.Split(':');


                //Step 1 - In user preference set Automatically start cine to OFF for modality to which study belongs load study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                ModalityDropdown().SelectByText(Modality[0]);
                userpref.AutoCINE_OFF().Click();
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 2 - Set viewer layout to 2x2
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID[0]);
                studies.SelectStudy("Study ID", StudyID[0]);
                viewer = StudyViewer.LaunchStudy();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                ExecutedSteps++;

                //Step 3 - Select Series#3 in Viewport
                viewer.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 4 - Change Series layout to 2X3
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_4)
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

                //Step 5 - Check for active series In new layout
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_5)
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

                //Step 6 - Select Series#6 in Viewport
                viewer.SeriesViewer_2X3().Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_6)
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

                //Step 7 - Change layout to 2X2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_7)
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

                //Step 8 - Check the Active Series displayed in new layout
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 9 - Load the first series in first viewport and second series in second viewport
                var action = new Actions(Driver);
                action.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                action.DragAndDrop(viewer.Thumbnails()[1], viewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
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

                //Step 10 - Set the image layout to 3x3 in viewport#4
                viewer.SeriesViewer_2X2().Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_10)
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

                //Step 11 - set series scope for all viewports Click group play
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForCineToPlay(1, 1);
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 1)) && IsElementVisible(viewer.By_Cinepause(2, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Completed";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Select window level tool and change the window low and/ window high in viewport#1
                viewer.cineViewport(1, 1).Click();
                viewer.ApplyWindowLevel(viewer.cineViewport(1, 1));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.CineToolbar(1, 1));
                if (step_12)
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

                //Step 13 - Select window level tool and change the window low and/ window high in viewport#3
                viewer.cineViewport(2, 1).Click();
                viewer.ApplyWindowLevel(viewer.cineViewport(2, 1));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.CineToolbar(2, 1));
                if (step_13)
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

                //Step 14 - Stop Cine in a viewport
                PageLoadWait.WaitForCineToPlay(2, 1);
                viewer.cineGroupPauseBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                if (IsElementVisible(viewer.By_Cineplay(1, 1)) && IsElementVisible(viewer.By_Cineplay(1, 2)) &&
                    IsElementVisible(viewer.By_Cineplay(2, 1)) && IsElementVisible(viewer.By_Cineplay(2, 2)))
                {
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

                //Step 15 - In User preferences set the thumbnail split to series for the modality to which the listed study belongs
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                ModalityDropdown().SelectByText(Modality[0]);
                userpref.ThumbnailSplittingSeriesRadioBtn().Click();
                userpref.AutoCINE_OFF().Click();
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(Description: StudyDescription);
                studies.SelectStudy("Description", StudyDescription);
                viewer = StudyViewer.LaunchStudy();
                ExecutedSteps++;

                //Step 16 - set series layout to 2x3
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_16)
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

                //Step 17 - Load first series in first viewport, second series in second viewport
                action = new Actions(Driver);
                action.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                action.DragAndDrop(viewer.Thumbnails()[1], viewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_17 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_17)
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

                //Step 18 - Click group play
                viewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForCineToPlay(1, 1);
                PageLoadWait.WaitForCineToPlay(1, 2);
                PageLoadWait.WaitForCineToPlay(1, 3);
                PageLoadWait.WaitForCineToPlay(2, 1);
                PageLoadWait.WaitForCineToPlay(2, 2);
                PageLoadWait.WaitForCineToPlay(2, 3);
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19 - Verify previous/next group
                if (!viewer.cinePrevGroupBtn().Enabled && viewer.cineNextGroupBtn().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20 - Verify the images being played
                Thread.Sleep(5000);
                if (viewer.verifyFrameIndicatorLineChanging(1, 1) == 1 &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2) == 1 &&
                    viewer.verifyFrameIndicatorLineChanging(1, 3) == 1 &&
                    viewer.verifyFrameIndicatorLineChanging(2, 1) == 1 &&
                    viewer.verifyFrameIndicatorLineChanging(2, 2) == 1 &&
                    viewer.verifyFrameIndicatorLineChanging(2, 3) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 21 - set series scope for all viewports Flip image in viewport#2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.cineViewport(1, 2).Click();
                viewer.ApplyWindowLevel(viewer.cineViewport(1, 2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_21 = studies.CompareImage(result.steps[ExecutedSteps], viewer.CineToolbar(1, 2));
                if (step_21)
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

                //Step 22 - Click Next group button
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForCineToPlay(1, 1);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    viewer.SeriesViewer_1X2().GetAttribute("src").Contains("Images/blankImage") &&
                    viewer.SeriesViewer_1X3().GetAttribute("src").Contains("Images/blankImage") &&
                    viewer.SeriesViewer_2X1().GetAttribute("src").Contains("Images/blankImage") &&
                    viewer.SeriesViewer_2X2().GetAttribute("src").Contains("Images/blankImage") &&
                    viewer.SeriesViewer_2X3().GetAttribute("src").Contains("Images/blankImage"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 23 - Verify previous/next group
                if (viewer.cinePrevGroupBtn().Displayed && viewer.cineNextGroupBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24 - Verify the images being played
                if (viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 25 - Click Previous group button
                viewer.cinePrevGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForCineToPlay(1, 1);
                if (viewer.cineNextGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 1px") &&
                    viewer.cinePrevGroupBtn().GetAttribute("style").Split(';')[1].Contains("border-width: 0px") &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26 - Close study. Search and load for a study with multiple series and only 1 series having multiple images others single image series
                studies.CloseStudy();
                studies = (Studies)login.Navigate("Studies");
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID[1], LastName: LastName);
                studies.SelectStudy("Study ID", StudyID[1]);
                viewer = StudyViewer.LaunchStudy();
                ExecutedSteps++;

                //Step 27 - Set viewer layout to 2x2
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_27 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_27)
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

                //Step 28 - Load all the series in each viewport load the multiple image series in viewport#4
                viewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                action = new Actions(Driver);
                action.DragAndDrop(viewer.Thumbnails()[3], viewer.SeriesViewer_2X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_28 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (step_28)
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

                //Step 29 - Click group play
                viewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForCineToPlay(2, 2);
                if (!IsElementVisible(viewer.By_Cinepause(1, 1)) && !IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    !IsElementVisible(viewer.By_Cinepause(2, 1)) && IsElementVisible(viewer.By_Cinepause(2, 2)))
                {
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

                //logout
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

        /// </summary>
        ///Cardio ordering	
        /// </summary> 

        public TestCaseResult Test_27491(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            String DefaultDomain = "SuperAdminGroup";
            String DefaultRole = "SuperRole";


            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String[] Accession = AccessionList.Split(':');

            //This case Should run on fresh server**
            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                //Step-1
                //In Domain management set the thumbnail split to image for the modality to which the listed study belong
                //enable cardio ordering

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");

                domain.SearchDomain(DefaultDomain);
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();

                // finally block -- revote back--
                //MG, CT, CT, CR 

                domain.ModalityDropdown().SelectByText("MG");
                if (domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image", StringComparison.CurrentCultureIgnoreCase) == false)
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.CardioOrderCheckBox()));
                if (domain.CardioOrderCheckBox().Selected == false)
                    domain.CardioOrderCheckBox().Click();

                //The preferences should be saved

                if (domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image") &&
                    domain.CardioOrderCheckBox().Selected)
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

                domain.ClickSaveEditDomain();

                //Step-2
                //Search and load for a study with KO

                Studies study = (Studies)login.Navigate("Studies");

                //used ("MG", "CT", "CR") modality study for testing...
                //Enable the  CardioOrder value in UserPreference.
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                String[] Modality = { "MG", "CT", "CR" };
                foreach (String s in Modality)
                {
                    userpref.ModalityDropDown().SelectByText(s);
                    PageLoadWait.WaitForPageLoad(20);
                    userpref.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    PageLoadWait.WaitForPageLoad(20);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(userpref.CardioOrderCheckBox()));
                    if (userpref.CardioOrderCheckBox().Selected == false)
                        userpref.CardioOrderCheckBox().Click();
                    PageLoadWait.WaitForPageLoad(20);
                }

                userpref.CloseUserPreferences();

                //MS10025  -- KO MG
                study.SearchStudy(AccessionNo: Accession[0], Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);

                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //The KO should be displayed first in thumbnail and in viewport before other modality images

                if (viewer.ThumbnailCaptions()[0].Text.Contains("KO") &&
                    viewer.SeriesViewPorts().Count == 2 &&
                    viewer.Thumbnails().Count == 5)
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

                //Step-3
                //Close study and Search and load for a study with PR
                study.CloseStudy();
                //50F1DAFAD -- PR CT
                study.SearchStudy(AccessionNo: Accession[1], Datasource: EA_131);
                study.SelectStudy("Accession", Accession[1]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //The PR should be displayed first in thumbnail and in viewport before other modality images

                if (viewer.ThumbnailCaptions()[0].Text.Contains("PR") &&
                   viewer.SeriesViewPorts().Count == 4 &&
                   viewer.Thumbnails().Count == 64)
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

                //Step-4
                //Close study and Search and for a study with PR and KO

                study.CloseStudy();
                //5781069 -- PR KO CT
                study.SearchStudy(AccessionNo: Accession[2], Datasource: EA_131);
                study.SelectStudy("Accession", Accession[2]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //First KO and then PR and then the other modality images should be displayed in thumbnail and in viewport

                if (viewer.ThumbnailCaptions()[0].Text.Contains("KO") &&
                    viewer.ThumbnailCaptions()[1].Text.Contains("KO") &&
                    viewer.ThumbnailCaptions()[2].Text.Contains("PR") &&
                    viewer.ThumbnailCaptions()[3].Text.Contains("CT") &&
                  viewer.SeriesViewPorts().Count == 4 &&
                  viewer.Thumbnails().Count == 4)
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

                //Step-5
                //Close study and Search and load for a study without PR and KO

                study.CloseStudy();
                //DSQ00000083 -- without  PR KO ==>CR
                study.SearchStudy(AccessionNo: Accession[3], Datasource: EA_131);
                study.SelectStudy("Accession", Accession[3]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Image should be displayed in order of series number

                if (viewer.ThumbnailCaptions()[0].Text.Contains("#1") &&
                   viewer.ThumbnailCaptions()[1].Text.Contains("#2") &&
                   viewer.ThumbnailCaptions()[2].Text.Contains("#3") &&
                   viewer.ThumbnailCaptions()[3].Text.Contains("#4") &&
                   viewer.ThumbnailCaptions()[4].Text.Contains("#7") &&
                   viewer.ThumbnailCaptions()[5].Text.Contains("#8") &&
                   viewer.ThumbnailCaptions()[6].Text.Contains("#9") &&
                   viewer.ThumbnailCaptions()[7].Text.Contains("#10") &&
                   viewer.SeriesViewPorts().Count == 6 &&
                   viewer.Thumbnails().Count == 8)
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

                //Step-6
                //Close study and Search and load for a study without PR and KO and Having same series number

                study.CloseStudy();
                //0001 -- without  PR KO  (CT)==>same series ID
                study.SearchStudy(AccessionNo: Accession[4], Datasource: EA_91);
                study.SelectStudy("Accession", Accession[4]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Image should be displayed in order of series number and then by series instance UID

                String seriesID_1 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String seriesID_2 = viewer.GetInnerAttribute(viewer.Thumbnails()[1], "src", '&', "seriesUID");

                String[] seriesID_List_1 = seriesID_1.Split('.');
                String[] seriesID_List_2 = seriesID_2.Split('.');

                //bool flag6 = false;
                //for (int i = 0; i < seriesID_List_1.Length; i++)
                //{
                //    if (long.Parse(seriesID_List_1[i]) == long.Parse(seriesID_List_2[i]))
                //        continue;
                //    else if (long.Parse(seriesID_List_1[i]) < long.Parse(seriesID_List_2[i]))
                //    {
                //        flag6 = true;
                //        break;
                //    }
                //    else
                //    {
                //        flag6 = false;
                //        break;
                //    }

                //}

                //if (flag6 &&
                //  viewer.SeriesViewPorts().Count == 4 &&
                //  viewer.Thumbnails().Count == 4)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-7
                //Close study and Search and load for a study with OT

                study.CloseStudy();
                //90e8a66a5 -- OT
                study.SearchStudy(AccessionNo: Accession[5], Datasource: EA_131);
                study.SelectStudy("Accession", Accession[5]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //The OT should be displayed last in thumbnail and in viewport after other modality images

                String Thumbnail_SeriesID_7 = viewer.GetInnerAttribute(viewer.Thumbnails()[viewer.ThumbnailCaptions().Count - 1], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_7 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "ClusterViewID");

                if (viewer.ThumbnailCaptions()[viewer.ThumbnailCaptions().Count - 1].Text.Contains("OT") &&
                      Viewerport_ClusterViewID_7.Contains(Thumbnail_SeriesID_7) &&
                      viewer.SeriesViewPorts().Count == 4 &&
                      viewer.Thumbnails().Count == 4)
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
                study.CloseStudy();

                //Step-8
                //"Login in iCA as Administrator
                //Create a new domain
                //Domain- D3
                //Role- R3
                //User- U3 --- Damoain admin user -- D3
                //Login as U3"

                String D3 = "D3_" + new Random().Next(1000);
                String R3 = "R3_" + new Random().Next(1000);
                String U3 = "D3_" + new Random().Next(1000);

                domain = (DomainManagement)login.Navigate("DomainManagement");

                domain.CreateDomain(D3, R3, datasources: null);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();


                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(U3, U3);

                //User should be logged in
                ExecutedSteps++;

                //Step-9
                //In Role management set the thumbnail split to image for the modality to which the listed study belongs
                //enable cardio ordering

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                //role.ShowRolesFromDomainDropDown().SelectByText(DefaultRole);
                role.SelectRole(R3);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);

                //Enable modality setting
                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                PageLoadWait.WaitForPageLoad(15);
                role.ModalityDropDown().SelectByText("MR");
                PageLoadWait.WaitForPageLoad(15);
                role.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(role.CardioOrderCheckBox()));
                if (role.CardioOrderCheckBox().Selected == false)
                    role.CardioOrderCheckBox().Click();

                PageLoadWait.WaitForPageLoad(15);

                //The preferences should be saved
                if (role.CardioOrderCheckBox().Selected &&
                    role.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image"))
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


                role.ClickSaveEditRole();

                //Step-10
                //Search and load for a study without SC, PR and KO

                study = (Studies)login.Navigate("Studies");
                //11661583 -- without  PR KO ==>MR Bone Rose
                study.SearchStudy(AccessionNo: Accession[6], Datasource: EA_91);
                study.SelectStudy("Accession", Accession[6]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Image should be displayed in order of image number
                bool flag10 = false;
                for (int i = 0; i < viewer.ThumbnailCaptions().Count; i++)
                {
                    if (viewer.ThumbnailCaptions()[i].Text.Contains("#3, Image#" + (i + 1)))
                    {
                        flag10 = true;
                    }
                    else
                    {
                        flag10 = false;
                        break;
                    }
                }

                if (flag10 &&
                   viewer.SeriesViewPorts().Count == 4 &&
                   viewer.Thumbnails().Count == 11)
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

                //Step-11
                //Close study and Search and load for a study without SC, PR and KO
                //Having same image number
                //BIPLANE type image

                //"BIPLANE A"images should be displayed before"BIPLANE B"images
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-12
                //"Close study and Search and load for a study without SC, PR and KO
                //Having same image number"

                //PRIMARY images should be displayed before SECONDARY images
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-13
                //"Close study and Search and load for a multiframe study without SC, PR and KO
                //Image with same number of frames"

                //thumbnail displayed sorted by SOP instance UID
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-14
                //"logout and Login in iCA as Administrator
                //Create a new domain
                //Domain- D4
                //Role- R4
                //User- U4
                //Login as U4"
                login.Logout();

                String D4 = "D4_" + new Random().Next(1000);
                String R4 = "R4_" + new Random().Next(1000);
                String U4 = "D4_" + new Random().Next(1000);

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domain = (DomainManagement)login.Navigate("DomainManagement");

                domain.CreateDomain(D4, R4, datasources: null);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();


                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(U4, U4);

                //User should be logged in
                ExecutedSteps++;

                //Step-15
                //"In User preferences set the thumbnail split to image for the modality to which the listed study belongs
                //enable cardio ordering

                study = (Studies)login.Navigate("Studies");

                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);

                userpref.ModalityDropDown().SelectByText("MR");
                userpref.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);

                if (userpref.CardioOrderCheckBox().Selected == false)
                    userpref.CardioOrderCheckBox().Click();

                PageLoadWait.WaitForPageLoad(15);

                //The preferences should be saved
                if (userpref.CardioOrderCheckBox().Selected &&
                    userpref.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image"))
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

                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                //Step-16
                //Search and load for a study without SC, PR and KO

                study = (Studies)login.Navigate("Studies");
                //11661583 -- without  PR KO ==>MR Bone Rose
                study.SearchStudy(AccessionNo: Accession[6], Datasource: EA_91);
                study.SelectStudy("Accession", Accession[6]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Image should be displayed in order of image number
                bool flag16 = false;
                for (int i = 0; i < viewer.ThumbnailCaptions().Count; i++)
                {
                    if (viewer.ThumbnailCaptions()[i].Text.Contains("#3, Image#" + (i + 1)))
                    {
                        flag16 = true;
                    }
                    else
                    {
                        flag16 = false;
                        break;
                    }
                }

                if (flag16 &&
                   viewer.SeriesViewPorts().Count == 4 &&
                   viewer.Thumbnails().Count == 11)
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

                //Step-17
                //"Close study and Search and load for a study without SC, PR and KO
                //Having same image number
                //BIPLANE type image"

                //"BIPLANE A"images should be displayed before"BIPLANE B"images
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-18
                //"Close study and Search and load for a study without SC, PR and KO
                //Having same image number"

                //PRIMARY images should be displayed before SECONDARY images
                result.steps[++ExecutedSteps].status = "Not Automated";

                ////Step-19
                //"Close study and Search and load for a multiframe study without SC, PR and KO
                //Image with same number of frames"

                //thumbnail displayed sorted by SOP instance UID
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Logout
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

        /// </summary>
        ///Cardio setting	
        /// </summary> 

        public TestCaseResult Test_27492(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Studies studies = new Studies();
            StudyViewer StudyVw = new StudyViewer();
            Viewer viewer = new Viewer();
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            DomainManagement domainmanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Setup Test Step Description
                //result = new TestCaseResult(stepcount);
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String domain = Config.adminGroupName;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Modality = ModalityList.Split(':');
                string[] datasources = null;

                String TestdomainD11 = "DomainD11_492_" + random.Next(1, 1000);
                String TestdomainAdminD11 = "DomainAdminD11_492_" + random.Next(1, 1000);
                String TestRoleR11 = "RoleR11_492_" + random.Next(1, 1000);
                String TestRoleR22 = "RoleR22_492_" + random.Next(1, 1000);
                String TestuserU11 = "UserU11_492" + new Random().Next(1, 1000);
                String TestuserU22 = "UserU22_492" + new Random().Next(1, 1000);

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.CreateDomain(TestdomainD11, TestdomainAdminD11, datasources);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.ClickSaveNewDomain();
                login.Logout();
                login.LoginIConnect(TestdomainD11, TestdomainD11);

                //step 1
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.CreateRole(TestdomainD11, TestRoleR11, "both", domainadmin: true);
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR11);
                rolemanagement.EditRoleByName(TestRoleR11);
                rolemanagement.UnCheckCheckbox(rolemanagement.UseDomainSetting());
                rolemanagement.SetcardioDefaults().Click();
                bool wrnMsg1 = rolemanagement.SetcardioDefaults_Warning().Displayed;
                if (wrnMsg1)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 2
                rolemanagement.CnfrmButton().Click();
                rolemanagement.ClickSaveEditRole();
                bool role2 = rolemanagement.RoleExists(TestRoleR11);
                if (role2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 3
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR11);
                rolemanagement.EditRoleByName(TestRoleR11);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                string XALyt3 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                string USLyt3 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                if ((XALyt3 == "1x1") && (USLyt3 == "1x1"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 4
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool XAAutoCine4 = rolemanagement.AutoCINEON_RB().Selected;
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                bool USAutoCine4 = rolemanagement.AutoCINEON_RB().Selected;
                if (XAAutoCine4 && USAutoCine4)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 5
                rolemanagement.CloseRoleManagement();
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.CreateUser(TestuserU11, TestRoleR11);
                if (usermanagement.IsUserExist(TestuserU11, TestdomainD11))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 6
                login.Logout();
                login.LoginIConnect(TestuserU11, TestuserU11);
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("lastname", LastName);
                studies.SelectStudy("Patient ID", PatientID);
                studies.LaunchStudy(isAutoCineEnabled: true);
                PageLoadWait.WaitForFrameLoad(15);
                //bool cine6 = StudyVw.cinepause(1, 2).Displayed;
                //PageLoadWait.WaitForFrameLoad(15);
                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step_6 = studies.CompareImage(result.steps[executedSteps], viewport);
                if (step_6)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 7
                bool step7 = false;
                try
                {
                    StudyVw.cineplay(1, 1).Click();
                }
                catch
                {
                    if (StudyVw.cinepause(1, 1).Displayed)
                        step7 = true;
                }
                if (StudyVw.cinepause(1, 1).Displayed || step7)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 8
                bool exmEnbld8 = false;
                IWebElement ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                List<IWebElement> Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld8 = true;
                        break;
                    }
                }
                if (exmEnbld8)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 9
                login.Logout();
                login.LoginIConnect(TestdomainD11, TestdomainD11);
                rolemanagement = login.Navigate<RoleManagement>();
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR11);
                rolemanagement.EditRoleByName(TestRoleR11);
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                bool step9 = rolemanagement.RoleInformationEditRole().Text.Equals("Role Information");
                if (step9)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 10
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                rolemanagement.ExamModeON().Click();
                rolemanagement.ClickSaveEditRole();
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR11);
                rolemanagement.EditRoleByName(TestRoleR11);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                bool step10 = rolemanagement.ExamModeON().Selected;
                if (step10)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 11
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                string XALyt11 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                string USLyt11 = rolemanagement.LayoutDropDown().SelectedOption.Text;
                if ((XALyt11 == "1x1") && (USLyt11 == "1x1"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 12
                rolemanagement.CloseRoleManagement();
                Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                bool step12 = rolemanagement.NewRoleBtn().Displayed;
                if (step12)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 13
                login.Logout();
                login.LoginIConnect(TestuserU11, TestuserU11);
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("lastname", LastName);
                studies.SelectStudy("Patient ID", PatientID);
                studies.LaunchStudy(isAutoCineEnabled: true);
                PageLoadWait.WaitForFrameLoad(15);
                //bool cine13 = StudyVw.cinepause(1, 1).Displayed;
                //PageLoadWait.WaitForFrameLoad(15);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step13 = studies.CompareImage(result.steps[executedSteps], viewport);
                if (step13)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 14
                bool step14 = false;
                try
                {
                    StudyVw.cineplay(1, 1).Click();
                }
                catch
                {
                    if (StudyVw.cinepause(1, 1).Displayed)
                        step14 = true;
                }
                if (StudyVw.cinepause(1, 1).Displayed || step14)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 15
                bool exmEnbld15 = false;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld15 = true;
                        break;
                    }
                }
                if (exmEnbld15)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 16
                login.Logout();
                login.LoginIConnect(TestdomainD11, TestdomainD11);
                rolemanagement = login.Navigate<RoleManagement>();
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR11);
                rolemanagement.EditRoleByName(TestRoleR11);
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                bool step16 = rolemanagement.RoleInformationEditRole().Displayed;
                if (step16)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 17
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.SetcardioDefaults().Click();
                bool wrnMsg17 = rolemanagement.SetcardioDefaults_Warning().Displayed;
                if (wrnMsg17)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 18
                rolemanagement.CnfrmButton().Click();
                bool wrnMsg18 = rolemanagement.SetcardioDefaults_Warning().Displayed;
                if (!wrnMsg18)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 19
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool XAAutoCine19 = rolemanagement.AutoCINEON_RB().Selected;
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                bool USAutoCine19 = rolemanagement.AutoCINEON_RB().Selected;
                if (XAAutoCine19 && USAutoCine19)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 20
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool XAAutoCine20 = rolemanagement.ExamModeON().Selected;
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                bool USAutoCine20 = rolemanagement.ExamModeON().Selected;
                if (XAAutoCine20 && USAutoCine20)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 21
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool XAAutoCine21 = rolemanagement.CardioOrder().Selected;
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                bool USAutoCine21 = rolemanagement.CardioOrder().Selected;
                if (XAAutoCine21 && USAutoCine21)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 22
                rolemanagement.ClickSaveEditRole();
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR11);
                rolemanagement.EditRoleByName(TestRoleR11);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool XA221 = rolemanagement.CardioOrder().Selected;
                bool XA222 = rolemanagement.ExamModeON().Selected;
                bool XA223 = rolemanagement.AutoCINEON_RB().Selected;
                rolemanagement.ModalityDropDown().SelectByText(Modality[1]);
                bool US221 = rolemanagement.CardioOrder().Selected;
                bool US222 = rolemanagement.ExamModeON().Selected;
                bool US223 = rolemanagement.AutoCINEON_RB().Selected;
                if (XA221 && XA222 && XA223 && US221 && US222 && US223)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 23
                result.steps[++executedSteps].status = "Not Automated";

                //step 24
                result.steps[++executedSteps].status = "Not Automated";

                //step 25
                login.Logout();
                login.LoginIConnect(TestuserU11, TestuserU11);
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("lastname", LastName);
                studies.SelectStudy("Patient ID", PatientID);
                studies.LaunchStudy(isAutoCineEnabled: true);
                PageLoadWait.WaitForFrameLoad(15);
                //bool cine25 = StudyVw.cinepause(1, 1).Displayed;
                //PageLoadWait.WaitForFrameLoad(15);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step_25 = studies.CompareImage(result.steps[executedSteps], viewport);
                if (step_25)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 26
                bool step26 = false;
                try
                {
                    StudyVw.cineplay(1, 1).Click();
                }
                catch
                {
                    if (StudyVw.cinepause(1, 1).Displayed)
                        step26 = true;
                }
                if (StudyVw.cinepause(1, 1).Displayed || step26)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 27
                bool exmEnbld27 = false;
                StudyVw.cinestop(1, 1).Click();
                StudyVw.ClickElement("Series Viewer 2x3");
                bool prev27 = StudyVw.cinePrevGroupBtn().Displayed;
                bool next27 = StudyVw.cineNextGroupBtn().Displayed;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld27 = true;
                        break;
                    }
                }
                if (exmEnbld27 && prev27 && next27)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 28
                login.Logout();
                login.LoginIConnect(TestdomainD11, TestdomainD11);
                rolemanagement = login.Navigate<RoleManagement>();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                bool step28 = rolemanagement.NewRoleBtn().Displayed;
                if (step28)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 29
                //rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(TestdomainD11, TestRoleR22, "both", domainadmin: true);
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR22);
                rolemanagement.EditRoleByName(TestRoleR22);
                rolemanagement.UnCheckCheckbox(rolemanagement.UseDomainSetting());
                rolemanagement.SetcardioDefaults().Click();
                rolemanagement.CnfrmButton().Click();
                rolemanagement.ClickSaveEditRole();
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestRoleR22);
                rolemanagement.EditRoleByName(TestRoleR22);
                bool wrnMsg29 = rolemanagement.UseDomainSetting().Selected;
                if (!wrnMsg29)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                rolemanagement.CloseRoleManagement();

                //step 30
                usermanagement = login.Navigate<UserManagement>();
                usermanagement.CreateUser(TestuserU22, TestRoleR22);
                if (usermanagement.IsUserExist(TestuserU22, TestdomainD11))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 31
                login.Logout();
                login.LoginIConnect(TestuserU22, TestuserU22);
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("lastname", LastName);
                studies.SelectStudy("Patient ID", PatientID);
                studies.LaunchStudy(isAutoCineEnabled: true);
                PageLoadWait.WaitForFrameLoad(15);
                //bool cine31 = StudyVw.cinepause(1, 1).Displayed;
                //PageLoadWait.WaitForFrameLoad(15);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step_31 = studies.CompareImage(result.steps[executedSteps], viewport);
                if (step_31)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 32
                bool step32 = false;
                try
                {
                    StudyVw.cineplay(1, 1).Click();
                }
                catch
                {
                    if (StudyVw.cinepause(1, 1).Displayed)
                        step32 = true;
                }
                if (StudyVw.cinepause(1, 1).Displayed || step32)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 33
                bool exmEnbld33 = false;
                StudyVw.cinestop(1, 1).Click();
                StudyVw.ClickElement("Series Viewer 2x3");
                bool prev33 = StudyVw.cinePrevGroupBtn().Displayed;
                bool next33 = StudyVw.cineNextGroupBtn().Displayed;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld33 = true;
                        break;
                    }
                }
                if (exmEnbld33 && prev33 && next33)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 34
                login.Logout();
                login.LoginIConnect(TestuserU22, TestuserU22);
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("lastname", LastName);
                studies.SelectStudy("Patient ID", PatientID);
                studies.LaunchStudy(isAutoCineEnabled: true);
                PageLoadWait.WaitForFrameLoad(15);
                //bool cine34 = StudyVw.cinepause(1, 1).Displayed;
                //PageLoadWait.WaitForFrameLoad(15);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step_34 = studies.CompareImage(result.steps[executedSteps], viewport);
                if (step_34)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 35
                bool step35 = false;
                try
                {
                    StudyVw.cineplay(1, 1).Click();
                }
                catch
                {
                    if (StudyVw.cinepause(1, 1).Displayed)
                        step35 = true;
                }
                if (StudyVw.cinepause(1, 1).Displayed || step35)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 36
                bool exmEnbld36 = false;
                StudyVw.cinestop(1, 1).Click();
                StudyVw.ClickElement("Series Viewer 2x3");
                bool prev36 = StudyVw.cinePrevGroupBtn().Displayed;
                bool next36 = StudyVw.cineNextGroupBtn().Displayed;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld36 = true;
                        break;
                    }
                }
                if (exmEnbld36 && prev36 && next36)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 37
                login.Logout();
                login.LoginIConnect(TestuserU22, TestuserU22);
                //studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("lastname", LastName);
                studies.SelectStudy("Patient ID", PatientID);
                studies.LaunchStudy(isAutoCineEnabled: true);
                PageLoadWait.WaitForFrameLoad(15);
                //bool cine37 = StudyVw.cinepause(1, 1).Displayed;
                //PageLoadWait.WaitForFrameLoad(15);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step_37 = studies.CompareImage(result.steps[executedSteps], viewport);
                if (step_37)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 38
                bool step38 = false;
                try
                {
                    StudyVw.cineplay(1, 1).Click();
                }
                catch
                {
                    if (StudyVw.cinepause(1, 1).Displayed)
                        step38 = true;
                }
                if (StudyVw.cinepause(1, 1).Displayed || step38)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 39
                bool exmEnbld39 = false;
                StudyVw.cinestop(1, 1).Click();
                StudyVw.ClickElement("Series Viewer 2x3");
                bool prev39 = StudyVw.cinePrevGroupBtn().Displayed;
                bool next39 = StudyVw.cineNextGroupBtn().Displayed;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld39 = true;
                        break;
                    }
                }
                if (exmEnbld39 && prev39 && next39)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// </summary>
        ///Image support	
        /// </summary> 
        public TestCaseResult Test_27493(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            Studies studies = new Studies();
            StudyViewer studyViewer = new StudyViewer();
            UserPreferences UserPref = new UserPreferences();


            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] Accession = AccessionIDList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                //Step-1: Send PALETTE COLOR as its Photometric Interpretation study and Transfer Syntax as  
                //RLE compression to datasource and load
                login.LoginIConnect(adminUserName, adminPassword);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("patient", PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step_1)
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-2: Load MONOCHROME2 as its Photometric Interpretation study and Transfer Syntax as  JPEG01 compression
                studies.SearchStudy("patient", PatientID[1]);
                studies.SelectStudy("Patient ID", PatientID[1]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step_2)
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-3: Load PALATTE COLOR as its Photometric Interpretation study and Transfer Syntax as JPEG01 compression
                studies.SearchStudy("patient", PatientID[2]);
                studies.SelectStudy("Patient ID", PatientID[2]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-4: Load RGB as its Photometric Interpretation study and Transfer Syntax as  JPEG01 compression
                studies.SearchStudy("patient", PatientID[3]);
                studies.SelectStudy("Patient ID", PatientID[3]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step_4)
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-5: Load YBR_FULL as its Photometric Interpretation study and Transfer Syntax as   JPEG01 compression
                studies.SearchStudy("patient", PatientID[5]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step_5)
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-6: Load YBR_FULL_422 as its Photometric Interpretation study and Transfer Syntax as   JPEG01 compression
                studies.SearchStudy("patient", PatientID[6]);
                studies.SelectStudy("Patient ID", PatientID[6]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step_6)
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-7: Load YBR_PARTIAL_422 as its Photometric Interpretation study and Transfer Syntax as   JPEG01 compression
                result.steps[ExecutedSteps].status = "Not Automated";

                //Step-8: Load YBR_RCT as its Photometric Interpretation study and Transfer Syntax as   JPEG01 compression
                studies.SearchStudy("patient", PatientID[7]);
                studies.SelectStudy("Patient ID", PatientID[7]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-9: Load YBR_ICT as its Photometric Interpretation study and Transfer Syntax as   JPEG01 compression
                studies.SearchStudy("patient", PatientID[8]);
                studies.SelectStudy("Patient ID", PatientID[8]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
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
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //Step-10: Load YBR_PARTIAL_420 as its Photometric Interpretation study and Transfer Syntax as JPEG01 compression
                result.steps[ExecutedSteps].status = "Not Automated";

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

        /// </summary>
        ///Cardio Cine - Keyboard Shortcuts (Desktop only)	
        /// </summary>
        public TestCaseResult Test1_27494(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables              
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Setup Test Step Description
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String PseudoSeriesIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PseudoSeriesID");

                String[] PatientID = PatientIDList.Split(':');
                String[] Modality = ModalityList.Split(':');
                String[] PseudoSeriesID = PseudoSeriesIDList.Split(':');

                //Step 1 - Search and load a study with multiple images in multiple series. Press 'p' or 'P' key on keyboard.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(Modality: "US", patientID: PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SeriesViewer_1X1().Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                if (IsElementVisible(viewer.By_Cinepause(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2 - Press"p"or"P"key
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - Press"p"or"P"key.
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                if (IsElementVisible(viewer.By_Cinepause(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 - Stop all running cine plays if there is any, Press the Right Arrow key on keyboard.
                viewer.cineGroupPauseBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String pid_4 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "pseudoSeriesID");
                if (pid_4.Contains(PseudoSeriesID[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5 - Press Left Arrow key on keyboard
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                viewer.SeriesViewer_1X1().Click();
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String pid_5 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "pseudoSeriesID");
                if (pid_5.Contains(PseudoSeriesID[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Start Cine. While it is running press Right Arrow key on keyboard.
                PageLoadWait.WaitForFrameLoad(10);
                viewer.cineplay(1, 1).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(10000);
                String pid_6 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    IsElementVisible(viewer.By_CineViewport(1, 1)) &&
                    pid_6.Contains(PseudoSeriesID[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 - Press Left Arrow key on keyboard while cine is running.
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String pid_7 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    IsElementVisible(viewer.By_CineViewport(1, 1)) &&
                    pid_7.Contains(PseudoSeriesID[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Pause the Cine, click Right Arrow to cycle through all series back.
                viewer.cinepause(1, 1).Click();
                for (int i = 0; i < 5; i++)
                {
                    new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                    PageLoadWait.WaitForAllViewportsToLoad(30);
                    Thread.Sleep(3000);
                }
                String pid_8 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (pid_8.Contains(PseudoSeriesID[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9 - Pause the Cine, click Left Arrow to cycle through all series back .
                Thread.Sleep(5000);
                viewer.SeriesViewer_1X1().Click();
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(3000);
                    new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                    PageLoadWait.WaitForAllViewportsToLoad(30);
                    Thread.Sleep(2000);
                }
                String pid_9 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (pid_9.Contains(PseudoSeriesID[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 - While it is running press Up Arrow key on keyboard.
                Thread.Sleep(5000);
                viewer.SeriesViewer_1X1().Click();
                Thread.Sleep(5000);
                viewer.cineplay(1, 1).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                String uid_4_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Completed";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 - Press Down Arrow key on keyboard while cine is running.
                viewer.cineplay(1, 1).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowDown).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - Stop the cine
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Return Result
                return result;
            }
        }

        /// </summary>
        ///Cardio Cine - Keyboard Shortcuts (Desktop only)	
        /// </summary>
        public TestCaseResult Test2_27494(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables              
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Setup Test Step Description
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String PseudoSeriesIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PseudoSeriesID");

                String[] PatientID = PatientIDList.Split(':');
                String[] Modality = ModalityList.Split(':');
                String[] PseudoSeriesID = PseudoSeriesIDList.Split(':');

                //Step 13 - Load a study with multiple images in multiple series. Start Group Cine.
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(Modality: "US", patientID: PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(5000);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 3)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 1)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 2)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 3)));
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14 - Select a viewport Press 'p' or 'P' key on keyboard a few times                
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                bool p1 = IsElementVisible(viewer.By_Cineplay(1, 1));

                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                bool p2 = IsElementVisible(viewer.By_Cinepause(1, 1));
                if (p1 && p2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 - Stop all running cine plays. Start Group Cine. While it is running select a viewport and press Right Arrow key on keyboard.
                Thread.Sleep(5000);
                viewer.cineGroupPauseBtn().Click();
                Thread.Sleep(10000);
                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(5000);
                String pid_15 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    IsElementVisible(viewer.By_CineViewport(1, 1)) &&
                    pid_15.Contains(PseudoSeriesID[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 16 - Repeat above step, this time press the Left Arrow key on keyboard while group cine is running.
                Thread.Sleep(5000);
                viewer.cineGroupPauseBtn().Click();
                Thread.Sleep(5000);
                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(10000);
                String pid_16 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    IsElementVisible(viewer.By_CineViewport(1, 1)) &&
                    pid_16.Contains(PseudoSeriesID[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 17 - Repeat above step, this time press the Up Arrow key on keyboard while group cine is running.
                viewer.cineGroupPauseBtn().Click();
                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(5000);
                if (IsElementVisible(viewer.By_Cineplay(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 1)) && IsElementVisible(viewer.By_Cinepause(2, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18 - Repeat above step, this time press the Down Arrow key on keyboard while group cine is running
                viewer.cineGroupPauseBtn().Click();
                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowDown).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(5000);
                if (IsElementVisible(viewer.By_Cineplay(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                   IsElementVisible(viewer.By_Cinepause(2, 1)) && IsElementVisible(viewer.By_Cinepause(2, 2)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 19 - Press"p"or"P"key randomly on different viewports while group cine has started (playing or Pausing).
                viewer.cineGroupPauseBtn().Click();
                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.cineViewport(1, 2).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                bool p_19_1 = IsElementVisible(viewer.By_Cineplay(1, 2));

                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                bool p_19_2 = IsElementVisible(viewer.By_Cinepause(1, 2));

                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.cineViewport(2, 1).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                bool p_19_3 = IsElementVisible(viewer.By_Cineplay(2, 1));

                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                bool p_19_4 = IsElementVisible(viewer.By_Cinepause(2, 1));

                if (p_19_1 && p_19_2 && p_19_3 && p_19_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20 - Stop all running cine plays. (Covered in next step)
                viewer.cineGroupPauseBtn().Click();
                studies.CloseStudy();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

        /// </summary>
        ///Cardio Cine - Keyboard Shortcuts (Desktop only)	
        /// </summary>
        public TestCaseResult Test3_27494(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables              
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Setup Test Step Description
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String PseudoSeriesIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PseudoSeriesID");

                String[] PatientID = PatientIDList.Split(':');
                String[] Modality = ModalityList.Split(':');
                String[] PseudoSeriesID = PseudoSeriesIDList.Split(':');

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");

                //Step 20 - Set Exam Mode to ON in User Preference for modalities that are going to be used for the test
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                ModalityDropdown().SelectByText(Modality[0]);
                userpref.ExamMode("0").Click();
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step 21 - Search and load a study with multiple images in multiple series. Select a viewport and verify there is looping toggle button in Cine menu bar
                studies.SearchStudy(Modality: "US", patientID: PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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

                //Step 22 - Set Exam Mode to looping the current image On, start Cine in a viewport by pressing"p"or"P"on keyboard
                viewer.cineplay(1, 1).Click();
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 23 - Press "p" or "P"
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                Thread.Sleep(5000);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                if (IsElementVisible(viewer.By_Cinepause(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24 - While it is running press Right Arrow key on keyboard.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String pid_24 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    IsElementVisible(viewer.By_CineViewport(1, 1)) &&
                    pid_24.Contains(PseudoSeriesID[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 25 - Press Left Arrow key on keyboard while cine is running.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                String pid_25 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) &&
                    IsElementVisible(viewer.By_CineViewport(1, 1)) &&
                    pid_25.Contains(PseudoSeriesID[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26 - While it is running press Up Arrow key on keyboard.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 27 - Press Down Arrow key on keyboard while cine is running.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.cineplay(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                Thread.Sleep(1000);

                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowDown).KeyUp(Keys.Control).Perform();
                //new Actions(Driver).SendKeys(viewer.cineViewport(1, 1), Keys.ArrowDown).Build().Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 28 - Toggle on looping the entire study (exam mode) from the current viewport. Start Cine.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                Thread.Sleep(2000);
                viewer.cineplay(1, 1).Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                if (viewer.GetReviewToolImage("Exam Mode").GetAttribute("class").Contains("toggleItem_On") == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 29 - While it is running press Right Arrow key on keyboard.
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                Thread.Sleep(500);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 3)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 1)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 3)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cineplay(1, 2)) &&
                    IsElementVisible(viewer.By_Cineplay(1, 3)) && IsElementVisible(viewer.By_Cineplay(2, 1)) &&
                    IsElementVisible(viewer.By_Cineplay(2, 2)) && IsElementVisible(viewer.By_Cineplay(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 30 - Press Left Arrow key on keyboard while cine is running.
                Thread.Sleep(3000);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                Thread.Sleep(500);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 3)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 1)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 3)));
                if (IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cineplay(1, 2)) &&
                    IsElementVisible(viewer.By_Cineplay(1, 3)) && IsElementVisible(viewer.By_Cineplay(2, 1)) &&
                    IsElementVisible(viewer.By_Cineplay(2, 2)) && IsElementVisible(viewer.By_Cineplay(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 31 - While it is running press Up Arrow key on keyboard.
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(3000);
                new Actions(Driver).SendKeys(viewer.cineViewport(1, 1), Keys.ArrowUp).Build().Perform();
                //new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 32 - Press Down Arrow key on keyboard while cine is running.
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.cineplay(1, 1).Click();
                Thread.Sleep(5000);
                //viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                //PageLoadWait.WaitForCineToPlay(1, 1);
                Thread.Sleep(5000);

                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowDown).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 33 - Stop all running cine plays. (Rest continued in the next tc)                                
                Thread.Sleep(5000);
                studies.CloseStudy();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

        // </summary>
        ///Cardio Cine - Keyboard Shortcuts (Desktop only)	
        /// </summary>
        public TestCaseResult Test4_27494(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables              
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Setup Test Step Description
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String PseudoSeriesIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PseudoSeriesID");

                String[] PatientID = PatientIDList.Split(':');
                String[] Modality = ModalityList.Split(':');
                String[] PseudoSeriesID = PseudoSeriesIDList.Split(':');

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");

                //Step 33 - Load a study with multiple images in multiple series with Exam Mode = On in its modality. Start Group Cine.
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(120);

                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(120);
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPauseBtn()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 3)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 2)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(2, 3)));
                if (IsElementVisible(viewer.By_CineGroupPauseBtn()) &&
                    IsElementVisible(viewer.By_Cinepause(1, 1)) && IsElementVisible(viewer.By_Cinepause(1, 2)) &&
                    IsElementVisible(viewer.By_Cinepause(1, 3)) && IsElementVisible(viewer.By_Cinepause(2, 1)) &&
                    IsElementVisible(viewer.By_Cinepause(2, 2)) && IsElementVisible(viewer.By_Cinepause(2, 3)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 34 - Select a viewport, toggle on looping the current image, Press 'p' or 'P' key on keyboard a few times                                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                bool p_34_1 = IsElementVisible(viewer.By_Cineplay(1, 1));
                Thread.Sleep(2000);

                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                Thread.Sleep(500);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                bool p_34_2 = IsElementVisible(viewer.By_Cinepause(1, 1));

                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                Thread.Sleep(500);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("P").KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                bool p_34_3 = IsElementVisible(viewer.By_Cineplay(1, 1));

                if (p_34_1 && p_34_2 && p_34_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 35 - Press"p"or"P"
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                if (IsElementVisible(viewer.By_Cinepause(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 36 - While it is running press Right Arrow key on keyboard in the current viewport
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(120);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                //String pid_36 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementPresent(viewer.By_Cinepause(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 37 - Press Left Arrow key on keyboard while cine is running.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(3000);
                try { viewer.cineplay(1, 1).Click(); }
                catch (Exception) { }
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                bool bPause37 = IsElementVisible(viewer.By_Cinepause(1, 1));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.By_CineViewport(1, 1)));
                bool pCineViewPort37 = IsElementVisible(viewer.By_CineViewport(1, 1));
                if (bPause37 && pCineViewPort37)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 38 - While it is running press Up Arrow key on keyboard.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                try { viewer.cineplay(1, 1).Click(); }
                catch (Exception) { }
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 39 - Press Down Arrow key on keyboard while cine is running.     
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                try
                {
                    viewer.cineplay(1, 1).Click();
                    Thread.Sleep(1000);
                    wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                }
                catch (Exception) { }

                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowDown).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 40 - Toggle on looping the entire study (exam mode) from the current viewport. Start Group Cine. 
                Thread.Sleep(3000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPlayBtn()));
                viewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                if (IsElementVisible(viewer.By_Cinepause(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 41 - While it is running press Right Arrow key on keyboard.
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowRight).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                //String pid_41 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                bool bPause_41 = IsElementVisible(viewer.By_Cinepause(1, 1));
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.By_CineViewport(1, 1)));
                bool bCineViewPort_41 = IsElementVisible(viewer.By_CineViewport(1, 1));
                if (bPause_41 && bCineViewPort_41)
                {
                    result.steps[++ExecutedSteps].status = "Partilly Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 42 - Press Left Arrow key on keyboard while group cine is running.                
                PageLoadWait.WaitForAllViewportsToLoad(20);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowLeft).KeyUp(Keys.Control).Perform();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                //String pid_42 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "pseudoSeriesID");
                if (IsElementVisible(viewer.By_Cinepause(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 43 - While it is running press Up Arrow key on keyboard.
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                //new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowUp).KeyUp(Keys.Control).Perform();
                new Actions(Driver).SendKeys(viewer.cineViewport(1, 1), Keys.ArrowUp).Build().Perform();
                PageLoadWait.WaitForAllViewportsToLoad(60);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 44 - Press Down Arrow key on keyboard while group cine is running.  
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPauseBtn()));
                viewer.cineGroupPauseBtn().Click();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPlayBtn()));
                viewer.cineGroupPlayBtn().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(5000);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys(Keys.ArrowDown).KeyUp(Keys.Control).Perform();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));
                if (IsElementVisible(viewer.By_Cineplay(1, 1)))
                {
                    result.steps[++ExecutedSteps].status = "Partially Automated";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 45 - Press"p"or"P"key randomly on different viewports while Group cine has started (playing or Pausing).
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(3000);
                viewer.cineGroupPauseBtn().Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPlayBtn()));
                viewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Thread.Sleep(3000);

                viewer.cineViewport(1, 2).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                bool v1 = IsElementVisible(viewer.By_Cineplay(1, 2));

                viewer.cineViewport(2, 2).Click();
                PageLoadWait.WaitForAllViewportsToLoad(20);
                new Actions(BasePage.Driver).KeyDown(Keys.Control).SendKeys("p").KeyUp(Keys.Control).Perform();
                wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(2, 2)));
                bool v2 = IsElementVisible(viewer.By_Cineplay(2, 2));
                if (v1 && v2)
                {
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

        /// </summary>
        ///Cine playing - the viewer quality option is used (Desktop/Tablet)	
        /// </summary> 

        public TestCaseResult Test_27495(String testid, String teststeps, int stepcount)
        {
            // Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            Studies studies = new Studies();
            StudyViewer studyViewer = new StudyViewer();
            UserPreferences UserPref = new UserPreferences();

            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domainmanagement = new DomainManagement();

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");


                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.AutoCINE_OFF().Click();
                userpref.CineMaxMemory().Clear();
                userpref.CineMaxMemory().SendKeys("700");
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-1: Go to Options\User Preferences, verify option JPEG (lossy) is selected by default for the user.
                UserPref.OpenUserPreferences();
                UserPref.SwitchToUserPrefFrame();
                if (UserPref.JPEGRadioBtn().Selected)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                UserPref.CloseUserPreferences();

                //Step-2: Load a Multi-frame study.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                studies = (Studies)login.Navigate("Studies");
                //Accession[0]=394efe9ad
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                bool step2_2 = studyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80");
                if (step2_1 && step2_2)
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

                //Step-3: Start cine.
                studyViewer.cineplay(1, 1).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                bool step3 = studyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80");
                if (studyViewer.cinepause(1, 1).Displayed && step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Stop cine and then start group cine switch to each pseudo series in the study.
                studyViewer.cinestop(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(20);
                bool step4_1 = studyViewer.cineplay(1, 1).Displayed && (studyViewer.cinepause(1, 1).Displayed == false);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                bool step4_2 = studyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80");
                //studyViewer.SeriesViewer_1X2().Click();
                //studyViewer.cineViewport(1, 1).Click();
                bool step4_3 = studyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80");
                if (studyViewer.CineToolbar(1, 1).Displayed && step4_1 && step4_2 && step4_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-5: Load various multi-frame studies.
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //Accession[0]=394efe9ad
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                bool step5_2 = studyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80");
                if (step5_1 && step5_2)
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

                //Step-6: Start group cine.
                PageLoadWait.WaitForFrameLoad(20);
                bool step6_1 = studyViewer.cineplay(1, 1).Displayed && (studyViewer.cinepause(1, 1).Displayed == false);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                bool step6_2 = studyViewer.LossyCompressedLable("studyview").GetAttribute("title").Equals("JPEG lossy compressed, Quality = 80");
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

                //Step-7: Go to Options -*^>^* User Preferences and change the image format to PNG. Save the changes.
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.PNGRadioBtn().Click();
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                ExecutedSteps++;

                //Step-8: Load a multi-frame study.
                //Accession[0]=394efe9ad
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studyViewer = studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step8_1 && !BasePage.Driver.FindElement(By.CssSelector("#CompressionLabel")).Displayed)
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

                //Step-9: Play cine/group cine.
                PageLoadWait.WaitForFrameLoad(20);
                bool step9_1 = studyViewer.cineplay(1, 1).Displayed && (studyViewer.cinepause(1, 1).Displayed == false);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (step9_1 && !BasePage.Driver.FindElement(By.CssSelector("#CompressionLabel")).Displayed)
                {
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.JPEGRadioBtn().Click();
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
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

        /// </summary>
        ///Apply image manipulation operation- edge enhancement tools (Desktop/Tablet)	
        /// </summary> 

        public TestCaseResult Test_27496(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //Setup Test Step Description
            result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);

            Studies studies = new Studies();
            StudyViewer studyViewer = new StudyViewer();
            UserPreferences UserPref = new UserPreferences();

            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domainmanagement = new DomainManagement();

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");


                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.AutoCINE_OFF().Click();
                userpref.CineMaxMemory().Clear();
                userpref.CineMaxMemory().SendKeys("700");
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-1: Load XA study with multiframe images. Start cine. 
                //Apply edge enhancement tools to catch (XA) images during cine.

                studies = (Studies)login.Navigate("Studies");
                //Accession[0]=038d7bfa8
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_96);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.cineplay(1, 1).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step1 && studyViewer.cinepause(1, 1).Displayed)
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

                //Step-2: Stop all running cine, start Group cine. 
                //Apply edge enhancement tools to catch (XA) images during group cine.
                studyViewer.cinestop(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                PageLoadWait.WaitForThumbnailsToLoad(30);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.EdgeEnhancementInteractive);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                if (step2 && studyViewer.cinepause(1, 1).Displayed)
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

        /// </summary>
        ///Apply image manipulation operations during cine - W/L, pan, and zoom tools (Desktop/Tablet)	
        /// </summary>

        public TestCaseResult Test_27497(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //Setup Test Step Description
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);


            Studies studies = new Studies();
            StudyViewer studyViewer = new StudyViewer();
            UserPreferences UserPref = new UserPreferences();

            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domainmanagement = new DomainManagement();

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String Lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");


                //Pre-Condition
                login.LoginIConnect(adminUserName, adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.AutoCINE_OFF().Click();
                userpref.CineMaxMemory().Clear();
                userpref.CineMaxMemory().SendKeys("700");
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-1: open User Preferences from Options menu. Set Exam Mode -> On for all modalities.
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                UserPref.ExamMode("0").Click();
                for (int i = 0; i < UserPref.ModalityDropDown().Options.Count; i++)
                {
                    UserPref.ModalityDropDown().SelectByIndex(i);
                    //input[id$='_ExamModeRadioButtons_0']
                    try
                    {
                        if (!UserPref.ExamMode("0").Selected)
                        {
                            UserPref.ExamMode("0").Click();
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                ExecutedSteps++;
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-2: Load a study with multiple frames in multiple images in  viewer, 
                //toggle on Looping the current image. Start Cine play.
                studies = (Studies)login.Navigate("Studies");
                //Accession[0]=038d7bfa8
                studies.SearchStudy(AccessionNo: Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.cineplay(1, 1).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.studyPanel());
                bool WL = false;
                bool pan = false;
                bool zoom = false;
                if (!studyViewer.GetViewerDisabledTools("Window Level"))
                    WL = true;
                if (!studyViewer.GetViewerDisabledTools("Zoom"))
                    zoom = true;
                if (!studyViewer.GetViewerDisabledTools("Pan"))
                    pan = true;

                if (step2 && studyViewer.cinepause(1, 1).Displayed && WL && zoom && pan)
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


                //Step-3: While cine is playing apply tools- Window/Level, Pan and Zoom without manually pause or stop it.
                PageLoadWait.WaitForElementToDisplay(studyViewer.cineViewport(1, 1));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studyViewer.cineViewport(1, 1)));
                Thread.Sleep(6000);
                studyViewer.cineViewport(1, 1).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                studyViewer.cineViewport(1, 1).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                studyViewer.cineViewport(1, 1).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                studyViewer.cineViewport(1, 1).Click();
                ExecutedSteps++;


                //Step-4: Stop cine, scroll through all frames in the image in the current viewport                            
                studyViewer.cinestop(1, 1).Click();
                studyViewer.DragScroll(1, 1, 1, 236);
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid + "_4_1_tools_Applied", ExecutedSteps + 1);
                bool status4_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X1());

                studyViewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_4_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status4_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X1());

                studyViewer.DragScroll(1, 1, 236, 236);
                result.steps[ExecutedSteps].SetPath(testid + "_4_3_Tools_LastImage_R", ExecutedSteps + 1);
                bool status4_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X1());

                if (status4_1 && status4_2 && status4_3 &&
                    studyViewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("236"))
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

                //Step-5: Toggle on looping the entire study. Start Cine.
                //studyViewer.cinestop(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.cineplay(1, 1).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                /* BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                 PageLoadWait.WaitForAllViewportsToLoad(10);
                 PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                 PageLoadWait.WaitForThumbnailsToLoad(30);*/
                WL = false;
                pan = false;
                zoom = false;
                if (!studyViewer.GetViewerDisabledTools("Window Level"))
                    WL = true;
                if (!studyViewer.GetViewerDisabledTools("Zoom"))
                    zoom = true;
                if (!studyViewer.GetViewerDisabledTools("Pan"))
                    pan = true;
                /* if (studyViewer.cinepause(1, 1).Displayed && studyViewer.cinepause(1, 2).Displayed && 
                     studyViewer.cinepause(1, 3).Displayed && studyViewer.cinepause(2, 1).Displayed &&
                     studyViewer.cinepause(2, 2).Displayed && studyViewer.cinepause(2, 3).Displayed &&
                     WL && zoom && pan)*/
                PageLoadWait.WaitForElementToDisplay(studyViewer.cinepause(1, 1));
                Thread.Sleep(6000);
                if (studyViewer.cinepause(1, 1).Displayed && WL && zoom && pan)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-6: While cine is playing apply tools without manually pause or stop cine, apply tools as such Window/Level
                //on pseudo series#1, Pan on pseudo series#2 and Zoom on pseudo series#3.
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studyViewer.cineGroupPlayBtn()));
                studyViewer.cineGroupPlayBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.cineViewport(2, 1));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studyViewer.cineViewport(2, 1)));
                Thread.Sleep(6000);
                studyViewer.cineViewport(2, 1).Click();//*************************
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 1));
                studyViewer.cineViewport(1, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 2));
                studyViewer.cineViewport(1, 3).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 3));
                studyViewer.cineViewport(1, 1).Click();
                ExecutedSteps++;

                //Step-7: Pause cine, scroll through all frames in each images (pseudo series#1, #2 and #3) in the current viewport
                studyViewer.cinepause(1, 1).Click();
                studyViewer.cineGroupPauseBtn().Click();
                PageLoadWait.WaitForFrameLoad(15);
                //BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPause")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.cineViewport(2, 1).Click();
                studyViewer.cineNextFramebtn(2, 1).Click();
                /*studyViewer.DragScroll(2, 1, 1, 236);
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid + "_7_1_Pan_Applied", ExecutedSteps + 1);
                bool status7_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X1());

                studyViewer.DragScroll(2, 1, 1, 285);

                result.steps[ExecutedSteps].SetPath(testid + "_7_2_Pan_NextImage_R", ExecutedSteps + 1);
                bool status7_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X1());

                studyViewer.DragScroll(2, 1, 2, 285);

                result.steps[ExecutedSteps].SetPath(testid + "_7_3_Pan_LastImage_R", ExecutedSteps + 1);
                bool status7_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X1());

                studyViewer.cineViewport(1, 2).Click();
                studyViewer.DragScroll(1, 2, 1, 234);

                result.steps[ExecutedSteps].SetPath(testid + "_7_4_WL_Applied", ExecutedSteps + 1);
                bool status7_4 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());

                studyViewer.DragScroll(1, 2, 2, 234);

                result.steps[ExecutedSteps].SetPath(testid + "_7_5_WL_NextImage_R", ExecutedSteps + 1);
                bool status7_5 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());

                studyViewer.DragScroll(1, 2, 234, 234);

                result.steps[ExecutedSteps].SetPath(testid + "_7_6_WL_LastImage_R", ExecutedSteps + 1);
                bool status7_6 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());

                studyViewer.cineViewport(1, 3).Click();
                studyViewer.DragScroll(1, 3, 1, 284);

                result.steps[ExecutedSteps].SetPath(testid + "_7_7_Zoom_Applied", ExecutedSteps + 1);
                bool status7_7 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X3());

                studyViewer.DragScroll(1, 3, 2, 284);

                result.steps[ExecutedSteps].SetPath(testid + "_7_8_Zoom_NextImage_R", ExecutedSteps + 1);
                bool status7_8 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X3());

                studyViewer.DragScroll(1, 3, 284, 284);

                result.steps[ExecutedSteps].SetPath(testid + "_7_9_Zoom_LastImage_R", ExecutedSteps + 1);
                bool status7_9 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X3());



                if (status7_1 && status7_2 && status7_3 && status7_4 && status7_5 && status7_6 && status7_7 &&
                     status7_8 && status7_9 && studyViewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("236"))
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
                */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-8: Start cine with looping the current image. During cine apply W/L, pan, and zoom on the
                //current clip, switch to next clip and then apply a tool on the clip.
                //current clip
                studyViewer.cineplay(2, 2).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                //next clip
                studyViewer.cineNextClipBtn(2, 2).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                WL = false;
                pan = false;
                zoom = false;
                if (!studyViewer.GetViewerDisabledTools("Window Level"))
                    WL = true;
                if (!studyViewer.GetViewerDisabledTools("Zoom"))
                    zoom = true;
                if (!studyViewer.GetViewerDisabledTools("Pan"))
                    pan = true;
                if (studyViewer.cinepause(2, 2).Displayed && WL && zoom && pan)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9: Pause cine, scroll through all frames in each image (pseudo series) in the current viewport.
                studyViewer.cinepause(2, 2).Click();
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.DragScroll(2, 1, 1, 217);
                PageLoadWait.WaitForFrameLoad(15);
                /*
                result.steps[++ExecutedSteps].SetPath(testid + "_9_1_Pan_Applied", ExecutedSteps + 1);
                bool status9_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                studyViewer.DragScroll(2, 2, 1, 217);

                result.steps[ExecutedSteps].SetPath(testid + "_9_2_Pan_NextImage_R", ExecutedSteps + 1);
                bool status9_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                studyViewer.DragScroll(2, 2, 2, 217);

                result.steps[ExecutedSteps].SetPath(testid + "_9_3_Pan_LastImage_R", ExecutedSteps + 1);
                bool status9_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());



                if (status9_1 && status9_2 && status9_3)
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
                */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-10: Start cine with looping the current image. During cine apply W/L, pan, and zoom on the 
                //current clip, switch to previous clip and then apply a tool on the clip.
                PageLoadWait.WaitForElementToDisplay(studyViewer.cineViewport(2, 3));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studyViewer.cineViewport(2, 3)));
                Thread.Sleep(6000);
                studyViewer.cineplay(2, 3).Click();//**********************
                studyViewer.cineViewport(2, 3).Click();
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 3));
                studyViewer.cineViewport(2, 3).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 3));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 3));
                studyViewer.cineViewport(2, 3).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 3));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 3));
                studyViewer.cineViewport(2, 3).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 3));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 3));

                //Prev clip
                studyViewer.cinePrevClipBtn(2, 3).Click();
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 3));
                studyViewer.cineViewport(2, 3).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 3));
                PageLoadWait.WaitForFrameLoad(15);
                WL = false;
                pan = false;
                zoom = false;
                if (!studyViewer.GetViewerDisabledTools("Window Level"))
                    WL = true;
                if (!studyViewer.GetViewerDisabledTools("Zoom"))
                    zoom = true;
                if (!studyViewer.GetViewerDisabledTools("Pan"))
                    pan = true;
                if (studyViewer.cinepause(2, 3).Displayed && WL && zoom && pan)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-11: Pause cine, scroll through all frames in each image (pseudo series) in the current viewport
                studyViewer.cinepause(2, 3).Click();
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.cineViewport(2, 3).Click();
                studyViewer.DragScroll(2, 3, 1, 266);
                PageLoadWait.WaitForFrameLoad(15);
                /*
                 result.steps[++ExecutedSteps].SetPath(testid + "_11_1_Pan_Applied", ExecutedSteps + 1);
                bool status11_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X3());

                studyViewer.DragScroll(2, 3, 1, 266);

                result.steps[ExecutedSteps].SetPath(testid + "_11_2_Pan_NextImage_R", ExecutedSteps + 1);
                bool status11_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X3());

                studyViewer.DragScroll(2, 3, 2, 266);

                result.steps[ExecutedSteps].SetPath(testid + "_11_3_Pan_LastImage_R", ExecutedSteps + 1);
                bool status11_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X3());



                if (status11_1 && status11_2 && status11_3)
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
                */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-12: Load a non-multiframe study with multiple images in multiple series in  viewer, set layout to 2x3.
                //Start Group Cine with looping the current image selected
                //Accession[0]=SERMLU0005779722
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                Thread.Sleep(6000);
                //studyViewer.cineViewport(1, 1).Click();
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                if (studyViewer.cinepause(1, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-13: While Group cine is playing apply tools on 3 different series such as Window/Level on 
                //one series (viewport1), Pan on next series (viewport2) and Zoom after next series (viewport3) without
                //manually pause or stop cine
                Thread.Sleep(6000);
                studyViewer.cineViewport(1, 1).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                studyViewer.cineViewport(1, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 2));
                studyViewer.cineViewport(1, 3).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 3));
                studyViewer.cineViewport(1, 1).Click();
                WL = false;
                pan = false;
                zoom = false;
                if (!studyViewer.GetViewerDisabledTools("Window Level"))
                    WL = true;
                if (!studyViewer.GetViewerDisabledTools("Zoom"))
                    zoom = true;
                if (!studyViewer.GetViewerDisabledTools("Pan"))
                    pan = true;
                if (studyViewer.cinepause(1, 1).Displayed && studyViewer.cinepause(1, 2).Displayed && studyViewer.cinepause(1, 3).Displayed && WL && zoom && pan)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14: Stop Group cine, scroll through all images in each series
                studyViewer.cinestop(1, 1).Click();
                studyViewer.cinestop(1, 2).Click();
                studyViewer.cinestop(1, 3).Click();
                studyViewer.DragScroll(1, 1, 1, 3);
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid + "_14_1_Pan_Applied", ExecutedSteps + 1);
                bool status14_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X1());

                studyViewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_14_2_Pan_NextImage_R", ExecutedSteps + 1);
                bool status14_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X1());

                studyViewer.DragScroll(1, 1, 3, 3);
                result.steps[ExecutedSteps].SetPath(testid + "_14_3_Pan_LastImage_R", ExecutedSteps + 1);
                bool status14_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X1());

                //studyViewer.cineViewport(1, 2).Click();
                studyViewer.DragScroll(1, 2, 1, 20);

                result.steps[ExecutedSteps].SetPath(testid + "_14_4_WL_Applied", ExecutedSteps + 1);
                bool status14_4 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());

                studyViewer.DragScroll(1, 2, 2, 20);

                result.steps[ExecutedSteps].SetPath(testid + "_14_5_WL_NextImage_R", ExecutedSteps + 1);
                bool status14_5 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());

                studyViewer.DragScroll(1, 2, 20, 20);

                result.steps[ExecutedSteps].SetPath(testid + "_14_6_WL_LastImage_R", ExecutedSteps + 1);
                bool status14_6 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X2());

                //studyViewer.cineViewport(1, 3).Click();
                studyViewer.DragScroll(1, 3, 1, 37);

                result.steps[ExecutedSteps].SetPath(testid + "_14_7_Zoom_Applied", ExecutedSteps + 1);
                bool status14_7 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X3());

                studyViewer.DragScroll(1, 3, 2, 37);

                result.steps[ExecutedSteps].SetPath(testid + "_14_8_Zoom_NextImage_R", ExecutedSteps + 1);
                bool status14_8 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X3());

                studyViewer.DragScroll(1, 3, 37, 37);

                result.steps[ExecutedSteps].SetPath(testid + "_14_9_Zoom_LastImage_R", ExecutedSteps + 1);
                bool status14_9 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X3());

                if (status14_1 && status14_2 && status14_3 && status14_4 && status14_5 && status14_6 && status14_7
                    && status14_8 && status14_9)
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


                //Step-15: Start Group cine play with looping the current image , during cine apply tools on different
                //series in the cine group, go to next group, and then apply tools on  a series of the cine group. 
                //Verify tools applied.
                //current clip
                studyViewer.cineGroupPauseBtn().Click();
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(studyViewer.cineGroupPlayBtn()));
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                Thread.Sleep(10000);
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                //next group
                studyViewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                Thread.Sleep(6000);
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.cinestop(2, 2).Click();
                studyViewer.DragScroll(2, 2, 1, 23);
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid + "_15_1_tools_Applied", ExecutedSteps + 1);
                bool status15_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                studyViewer.ClickDownArrowbutton(2, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_15_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status15_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                studyViewer.DragScroll(2, 2, 23, 23);
                result.steps[ExecutedSteps].SetPath(testid + "_15_3_Tools_LastImage_R", ExecutedSteps + 1);
                bool status15_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                if (status15_1 && status15_2 && status15_3)
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


                //Step-16: Repeat above step this time select go to Previous Group. Verify tools applied.
                studyViewer.cineGroupPauseBtn().Click();
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                Thread.Sleep(6000);
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                //next group
                studyViewer.cineNextGroupBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(2, 2));
                Thread.Sleep(6000);
                studyViewer.cineViewport(2, 2).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(2, 2));
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.cinestop(2, 2).Click();
                studyViewer.DragScroll(2, 2, 1, 27);
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid + "_16_1_tools_Applied", ExecutedSteps + 1);
                bool status16_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                studyViewer.ClickDownArrowbutton(2, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_16_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status16_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                studyViewer.DragScroll(2, 2, 27, 27);
                result.steps[ExecutedSteps].SetPath(testid + "_16_3_Tools_LastImage_R", ExecutedSteps + 1);
                bool status16_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_2X2());

                if (status16_1 && status16_2 && status16_3 &&
                    studyViewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("23"))
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

                //Step-17: Change layout to 1x1,  Start Group cine, toggle on looping the entire study in a viewport,
                //during cine looping on the current viewport apply w/l, pan and zoom tool in different series. 
                //Stop the Group cine and verify the tool is applied
                /*studyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                studyViewer.cineplay(1, 1).Click();
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                studyViewer.cineViewport(1, 1).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                studyViewer.cineViewport(1, 1).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                studyViewer.cineViewport(1, 1).Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.cineViewport(1, 1));
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForElementToDisplay(studyViewer.CineLabelFrameRate(1, 1));
                studyViewer.cinestop(1, 1).Click();
                studyViewer.DragScroll(1, 1, 1, 236);
                PageLoadWait.WaitForFrameLoad(15);
               */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-18: Load a study with multiple frames in multiple images in html5 viewer, Start Cine play.
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                studyViewer.Html5ViewStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.html5seriesViewer_1X1().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_cineBtnGroupPlay")).Click();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(6000);
                if (studyViewer.CinePauseResumeBtnHTML5(1, 1).Displayed && studyViewer.CinePauseResumeBtnHTML5(2, 1).Displayed &&
                    studyViewer.CinePauseResumeBtnHTML5(3, 1).Displayed && studyViewer.CinePauseResumeBtnHTML5(4, 1).Displayed &&
                studyViewer.CinePauseResumeBtnHTML5(5, 1).Displayed && studyViewer.CinePauseResumeBtnHTML5(6, 1).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-19: While cine is playing apply image manipulation tools Window/Level,
                //Pan and Zoom on a image without manually pause or stop it.
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                studyViewer.html5seriesViewer_1X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(studyViewer.CineBufferPercentHTML5(1, 1), "100%"));
                studyViewer.html5seriesViewer_1X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(studyViewer.CineBufferPercentHTML5(1, 1), "100%"));
                studyViewer.html5seriesViewer_1X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(15);
                WL = false;
                pan = false;
                zoom = false;
                if (!studyViewer.GetViewerDisabledTools("Window Level"))
                    WL = true;
                if (!studyViewer.GetViewerDisabledTools("Zoom"))
                    zoom = true;
                if (!studyViewer.GetViewerDisabledTools("Pan"))
                    pan = true;
                if (studyViewer.CinePauseResumeBtnHTML5(1, 1).GetAttribute("title").Equals("Cine Pause") && WL && zoom && pan)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-20: Pause the Cine, scroll through all images in the viewport
                studyViewer.CinePauseResumeBtnHTML5(1, 1).Click();
                /*studyViewer.DownArrowBtnHTML5(1).Click();

                result.steps[++ExecutedSteps].SetPath(testid + "_20_1_tools_Applied", ExecutedSteps + 1);
                bool status20_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X1());

                studyViewer.DownArrowBtnHTML5(1).Click();
                studyViewer.DownArrowBtnHTML5(1).Click();

                result.steps[ExecutedSteps].SetPath(testid + "_20_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status20_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X1());


                if (status20_1 && status20_2)
                {
                    result.steps[ExecutedSteps++].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps++].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                */
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-21: Apply tool while it is paused
                studyViewer.html5seriesViewer_1X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.CineStopHTML5(1).Click();
                studyViewer.DownArrowBtnHTML5(1).Click();

                result.steps[++ExecutedSteps].SetPath(testid + "_21_1_tools_Applied", ExecutedSteps + 1);
                bool status21_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X1());

                studyViewer.DownArrowBtnHTML5(1).Click();
                studyViewer.DownArrowBtnHTML5(1).Click();

                result.steps[ExecutedSteps].SetPath(testid + "_21_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status21_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X1());


                if (status21_1 && status21_2)
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


                //Step-22: Start cine in 2 different viewports, while cine is playing apply image manipulation tools
                //as such Window/Level in one pseudo series and Zoom on the next one without manually pause or stop it.
                studyViewer.html5seriesViewer_1X2().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                studyViewer.html5seriesViewer_1X3().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                studyViewer.html5seriesViewer_1X2().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(studyViewer.CineBufferPercentHTML5(2), "100%"));
                studyViewer.html5seriesViewer_1X3().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X3());
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(studyViewer.CineBufferPercentHTML5(3), "100%"));
                WL = false;
                pan = false;
                zoom = false;
                if (!studyViewer.GetViewerDisabledTools("Window Level"))
                    WL = true;
                if (!studyViewer.GetViewerDisabledTools("Zoom"))
                    zoom = true;
                if (!studyViewer.GetViewerDisabledTools("Pan"))
                    pan = true;
                if (studyViewer.CinePauseResumeBtnHTML5(2, 1).GetAttribute("title").Equals("Cine Pause") &&
                    studyViewer.CinePauseResumeBtnHTML5(3, 1).GetAttribute("title").Equals("Cine Pause") && WL && zoom && pan)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Stop the Cine, scroll through all images in the viewport.
                studyViewer.CinePauseResumeBtnHTML5(2);
                studyViewer.CinePauseResumeBtnHTML5(3);
                result.steps[++ExecutedSteps].SetPath(testid + "_21_1_tools_Applied", ExecutedSteps + 1);
                bool status23_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X2());

                studyViewer.DownArrowBtnHTML5(1).Click();
                studyViewer.DownArrowBtnHTML5(1).Click();

                result.steps[ExecutedSteps].SetPath(testid + "_21_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status23_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X2());

                result.steps[ExecutedSteps].SetPath(testid + "_21_1_tools_Applied", ExecutedSteps + 1);
                bool status23_3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X3());

                studyViewer.DownArrowBtnHTML5(1).Click();
                studyViewer.DownArrowBtnHTML5(1).Click();

                result.steps[ExecutedSteps].SetPath(testid + "_21_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status23_4 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X3());


                if (status23_1 && status23_2 && status23_3 && status23_4)
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


                //Step-24: Load a non-multiframe study with multiple images in multiple series in HTML5 viewer, 
                //repeat above steps by applying w/l, pan and zoom tools during cine 
                studies.CloseStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                studyViewer.Html5ViewStudy();
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.html5seriesViewer_1X1().Click();
                //play cine
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                studyViewer.html5seriesViewer_1X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(15);
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(studyViewer.CineBufferPercentHTML5(1), "100%"));
                studyViewer.html5seriesViewer_1X1().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X1());
                PageLoadWait.WaitForFrameLoad(15);
                studyViewer.html5seriesViewer_1X2().Click();
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.Cine);
                studyViewer.html5seriesViewer_1X2().Click();
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(studyViewer.CineBufferPercentHTML5(2), "100%"));
                studyViewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                studyViewer.DragMovement(studyViewer.html5seriesViewer_1X2());
                PageLoadWait.WaitForFrameLoad(15);
                //stop
                studyViewer.CineStopHTML5(1, 1).Click();
                result.steps[++ExecutedSteps].SetPath(testid + "_24_1_tools_Applied", ExecutedSteps + 1);
                bool status24_1 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X1());

                studyViewer.DownArrowBtnHTML5(1).Click();
                studyViewer.DownArrowBtnHTML5(1).Click();

                result.steps[ExecutedSteps].SetPath(testid + "_24_2_Tools_NextImage_R", ExecutedSteps + 1);
                bool status24_2 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.html5seriesViewer_1X2());


                if (status24_1 && status24_2)
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


        /// </summary>
        /// Ignore W/L for images has subtraction sequence (0028,6100) (Desktop/Tablet)	
        /// </summary> 

        public TestCaseResult Test_27498(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
            String[] P_ID = PatientIDList.Split('=');

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                //Have to add study in PACS**

                String DicomWindowCenter = "127";
                String DicomWindowWidth = "255";

                //Step-1
                //Load the test dataset that contain a subtraction sequence (0028,6100) into viewer, 
                //note down the W/L values on the image viewport of the loaded study.
                //Locate the DICOM header file from the ICA Server iCA Webaccess cache.
                //(C-\Windows\Temp\WebAccessAmicasP10FilesCache\the part 10 file of the dataset.)

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-1: Load XA study with multiframe images. Start cine. 
                //Apply edge enhancement tools to catch (XA) images during cine.

                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(patientID: P_ID[0], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", P_ID[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(25);

                String dicomName = viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ClusterViewID").Split('_')[1].Split(new string[] { "PS" }, StringSplitOptions.None)[1];//

                String TempFile_WindowCenter = BasePage.ReadDicomFile<String>("C:\\Windows\\Temp\\WebAccessAmicasP10FilesCache\\" + dicomName + ".dcm", DicomTag.WindowCenter);
                String TempFile_WindowWidth = BasePage.ReadDicomFile<String>("C:\\Windows\\Temp\\WebAccessAmicasP10FilesCache\\" + dicomName + ".dcm", DicomTag.WindowWidth);

                //The part 10 file of the dataset is available
                ExecutedSteps++;

                //Step-2
                //Open the part 10 file using a DICOM tool & check the value of Window Center(0028 1050)
                //and Window Width (0028 1051) matching the value of W/L displaying on the viewer

                //The W/L values from viewport is different from the values in the DICOM header in the part 10 file of the test data.

                result.steps[++ExecutedSteps].SetPath(testid + "_WW_WL_should_not_127_255", ExecutedSteps + 1);
                bool status2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());


                if (status2 && //image comparision WW and WL value should not 127 and 255
                    DicomWindowCenter == TempFile_WindowCenter &&
                    DicomWindowWidth == TempFile_WindowWidth)
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

        /// </summary>
        ///Exam mode - Cardiology	
        /// </summary>

        public TestCaseResult Test_89146(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            StudyViewer StudyVw = null;
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement = null;
            RoleManagement rolemanagement = null;
            UserPreferences userpreferences = null;
            Taskbar taskbar = null;
            string licensefilepath = Config.licensefilepath;
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedSteps = -1;

            try
            {
                //Setup Test Step Description
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                limit = Int32.Parse(limit.ToString().Substring(0, 4));
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                string[] datasources = null;
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastName = LastNameList.Split(':');
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] PatientID = PatientIDList.Split(':');
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Modality = ModalityList.Split(':');
                String StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
                String TestdomainD1 = "DomainD1_146_" + random.Next(1, 1000);
                String TestdomainAdminD1 = "DomainAdminD1_146_" + random.Next(1, 1000);

                //Pre-condition
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.Templates_Tab);
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                wpfobject.GetListBox(0).Items[0].Click();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.GetMainWindowByTitle("Template");
                wpfobject.ClickButton("6");
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                taskbar.Show();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domainmanagement = login.Navigate<DomainManagement>();
                domainmanagement.CreateDomain(TestdomainD1, TestdomainAdminD1, datasources);
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.ClickSaveNewDomain();
                login.Logout();
                login.LoginIConnect(TestdomainD1, TestdomainD1);

                //step 1
                domainmanagement = login.Navigate<DomainManagement>();
                PageLoadWait.WaitForFrameLoad(10);
                bool step1 = false;
                SelectElement ele_modality = domainmanagement.ModalityDropDown();
                IList<IWebElement> opt_modalities = ele_modality.Options;
                IList<String> modalities = opt_modalities.Select<IWebElement, String>(element => element.GetAttribute("innerHTML")).ToList<String>();
                foreach (String modality in modalities)
                {
                    ele_modality.SelectByText(modality);
                    PageLoadWait.WaitForPageLoad(10);
                    if (!(modality.Equals("XA") || modality.Equals("US")))
                    {
                        if (domainmanagement.ExamModeOFF_DA().Selected)
                            step1 = true;
                        else
                        {
                            step1 = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!domainmanagement.ExamModeOFF_DA().Selected)
                            step1 = true;
                        else
                        {
                            step1 = false;
                            break;
                        }
                    }
                }
                if (step1)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 2
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[0]);
                studies.SelectStudy("Study Date", StudyDate);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                var view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step2 = false;
                step2 = studies.CompareImage(result.steps[executedSteps], view);
                if (step2)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 3
                bool exmEnbld3 = true;
                IWebElement ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                List<IWebElement> Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld3 = false;
                        break;
                    }
                }
                if (exmEnbld3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 4
                login.CloseStudy();
                domainmanagement = login.Navigate<DomainManagement>();
                //domainmanagement.SelectDomain(TestdomainD1);
                //domainmanagement.ClickEditDomain();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                domainmanagement.ModalityDropDown().SelectByText(Modality[0]);
                domainmanagement.ExamModeON_DA().Click();
                domainmanagement.AddAllToolsToToolBar();
                domainmanagement.ClickSaveEditDomain();
                if (domainmanagement.CloseAlertButton().Displayed)
                    domainmanagement.CloseAlertButton().Click();
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[0]);
                studies.SelectStudy("Study Date", StudyDate);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step4 = false;
                step4 = studies.CompareImage(result.steps[executedSteps], view);
                if (step4)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 5
                bool glblStck5 = true;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Global Stack")
                    {
                        glblStck5 = false;
                        break;
                    }
                }
                if (glblStck5)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 6
                StudyVw.SeriesViewer_2X2().Click();
                StudyVw.DoubleClick(StudyVw.Thumbnails()[2]);
                if (!StudyVw.CineToolbar(2, 2).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 7
                StudyVw.ClickElement("Exam Mode");
                bool exmEnbld7 = false;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld7 = tool.GetAttribute("class").Contains("notSelected32 enabledOnCine toggleItem_On");
                        break;
                    }
                }
                if (exmEnbld7 && StudyVw.CineToolbar(1, 2).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 8
                bool glblStck8 = true;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Global Stack")
                    {
                        glblStck8 = false;
                        break;
                    }
                }
                if (glblStck8)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 9
                StudyVw.cineplay(1, 2).Click();
                if (StudyVw.cinepause(1, 2).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 10
                result.steps[++executedSteps].status = "Not Automated";

                //step 11
                StudyVw.cinepause(1, 2).Click();
                if (StudyVw.cineplay(1, 2).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 12
                StudyVw.cineplay(1, 2).Click();
                if (StudyVw.cinepause(1, 2).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 13
                StudyVw.CloseStudy();
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SearchRole(TestdomainAdminD1);
                rolemanagement.SelectRole(TestdomainAdminD1);
                rolemanagement.ClickEditRole();
                bool step13 = false;
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                SelectElement ele_modality13 = rolemanagement.ModalityDropDown();
                IList<IWebElement> opt_modalities13 = ele_modality13.Options;
                IList<String> modalities13 = opt_modalities13.Select<IWebElement, String>(element => element.GetAttribute("innerHTML")).ToList<String>();
                foreach (String modality13 in modalities13)
                {
                    ele_modality13.SelectByText(modality13);
                    PageLoadWait.WaitForPageLoad(10);
                    if (!(modality13.Equals("XA") || modality13.Equals("US")))
                    {
                        if (rolemanagement.ExamModeOFF().Selected)
                            step13 = true;
                        else
                        {
                            step13 = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!rolemanagement.ExamModeOFF().Selected)
                            step13 = true;
                        else
                        {
                            step13 = false;
                            break;
                        }
                    }
                }
                if (step13)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 14
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                rolemanagement.ExamModeON().Click();
                rolemanagement.ClickSaveEditRole();
                //rolemanagement.SelectDomainfromDropDown(TestdomainD11);
                rolemanagement.SearchRole(TestdomainAdminD1);
                rolemanagement.EditRoleByName(TestdomainAdminD1);
                PageLoadWait.WaitForFrameLoad(15);
                if (rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    rolemanagement.DefaultSettingPerModalityUseDomainSetting_CB().Click();
                PageLoadWait.WaitForPageLoad(15);
                rolemanagement.ModalityDropDown().SelectByText(Modality[0]);
                bool step14 = rolemanagement.ExamModeON().Selected;
                if (step14)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                rolemanagement.CloseRoleManagement();

                //step 15
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[0]);
                studies.SelectStudy("Study Date", StudyDate);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step15 = false;
                step15 = studies.CompareImage(result.steps[executedSteps], view);
                if (step15)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 16
                StudyVw.cineplay(1, 1).Click();
                if (StudyVw.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 17
                result.steps[++executedSteps].status = "Not Automated";

                //step 18
                StudyVw.ClickElement("Exam Mode");
                bool exmEnbld18 = false;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld18 = tool.GetAttribute("class").Contains("notSelected32 enabledOnCine toggleItem_On");
                        break;
                    }
                }
                if (exmEnbld18)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 19
                result.steps[++executedSteps].status = "Not Automated";

                //step 20
                ////to inform about thumbnail
                result.steps[++executedSteps].status = "Not Automated";

                //step 21
                StudyVw.SeriesViewer_1X2().Click();
                do
                {
                    try
                    {
                        StudyVw.cineGroupPauseBtn().Click();
                    }
                    catch (Exception ex)
                    { }
                }
                while (StudyVw.cineGroupPauseBtn().Displayed);
                Thread.Sleep(1000);
                StudyVw.ClickElement("User Preference");
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_UserprefFrame");
                bool step21 = false;
                userpreferences = new UserPreferences();
                SelectElement modality3 = userpreferences.ModalityDropDown();
                opt_modalities = modality3.Options;
                modalities = opt_modalities.Select<IWebElement, String>(element => element.GetAttribute("innerHTML")).ToList<String>();
                foreach (String modality in modalities)
                {
                    modality3.SelectByText(modality);
                    PageLoadWait.WaitForPageLoad(10);
                    if (!(modality.Equals("XA") || modality.Equals("US")))
                    {
                        if (userpreferences.ExamMode("1").Selected)
                            step21 = true;
                        else
                        {
                            step21 = false;
                            break;
                        }
                    }
                    else
                    {
                        if (userpreferences.ExamMode("0").Selected)
                            step21 = true;
                        else
                        {
                            step21 = false;
                            break;
                        }
                    }
                }
                if (step21)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 22
                userpreferences.ModalityDropDown().SelectByText(Modality[1]);
                userpreferences.ExamMode("0").Click();
                userpreferences.SavePreferenceBtn().Click();
                //userpreferences.CloseUserPreferences();
                userpreferences.CloseBtn().Click();
                StudyVw.CloseStudy();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[1]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step22 = false;
                step22 = studies.CompareImage(result.steps[executedSteps], view);
                if (step22)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 23
                StudyVw.SeriesViewer_1X2().Click();
                StudyVw.ClickElement("Exam Mode");
                bool exmEnbld23 = false;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld23 = tool.GetAttribute("class").Contains("notSelected32 enabledOnCine toggleItem_On");
                        break;
                    }
                }
                if (exmEnbld23)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 24
                StudyVw.cineplay(1, 2).Click();
                if (StudyVw.cinepause(1, 2).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 25
                result.steps[++executedSteps].status = "Not Automated";

                //step 26
                StudyVw.CloseStudy();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[1]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                bool step26 = false;
                step26 = studies.CompareImage(result.steps[executedSteps], view);
                if (step26)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 27
                StudyVw.SeriesViewer_1X2().Click();
                StudyVw.ClickElement("Exam Mode");
                bool exmEnbld27 = false;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld27 = tool.GetAttribute("class").Contains("notSelected32 enabledOnCine toggleItem_On");
                        break;
                    }
                }
                if (exmEnbld27)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 28
                StudyVw.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForFrameLoad(3);
                //StudyVw.Scroll(1, 2, 31, "down", "click");
                for (int i = 0; i < 31; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 2);
                }
                //StudyVw.ScrollByKey(1, 2, 31, "down");//////////////
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                String uid_28_1 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                for (int i = 0; i < 1; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 2);
                }
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                String uid_28_2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ClusterViewID");
                if ((uid_28_1.Contains("49.48.48.54PS_1")) && (uid_28_2.Contains("49.48.48.53PS_2")))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //step 29
                StudyVw.cineplay(1, 2).Click();
                if (StudyVw.cinepause(1, 2).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 30
                result.steps[++executedSteps].status = "Not Automated";

                //step 31
                result.steps[++executedSteps].status = "Not Automated"; ///////

                //step 32
                StudyVw.CloseStudy();
                userpreferences = login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText(Modality[1]);
                userpreferences.ClearText("id", "CineSingleFrameDelayTimeInSecondsTextBox");
                userpreferences.SetText("id", "CineSingleFrameDelayTimeInSecondsTextBox", "5");
                //userpreferences.SavePreferenceBtn().Click();
                userpreferences.CloseUserPreferences();
                login.OpenUserPreferences();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                string delay32 = userpreferences.GetTextFromTextBox("id", "CineSingleFrameDelayTimeInSecondsTextBox");
                if (delay32 == "5")
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                userpreferences.CancelUserPreferences();

                //step 33
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy("Last Name", LastName[2]);
                studies.SelectStudy("Patient ID", PatientID[1]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++executedSteps].SetPath(testid, executedSteps);
                view = Driver.FindElement(By.CssSelector("#studyPanelDiv_1"));
                bool step33 = studies.CompareImage(result.steps[executedSteps], view);
                if (step33)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 34
                StudyVw.ClickElement("Exam Mode");
                bool exmEnbld34 = false;
                ReviewTools = PageLoadWait.WaitForElement(StudyVw.By_ReviewtoolBar(), WaitTypes.Visible, 20);
                Tools = ReviewTools.FindElements(By.TagName("img")).ToList();
                foreach (IWebElement tool in Tools)
                {
                    if (tool.GetAttribute("title") == "Exam Mode")
                    {
                        exmEnbld34 = tool.GetAttribute("class").Contains("notSelected32 enabledOnCine toggleItem_On");
                        break;
                    }
                }
                if (exmEnbld34)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 35
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                String uid_35_1 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                for (int i = 0; i < 100; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                String uid_35_2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                for (int i = 0; i < 1; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                String uid_35_3 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if ((uid_35_1.Contains("20090924140642.1PS_1")) && (uid_35_2.Contains(".1282PS_2")) && (uid_35_3.Contains(".1235410088.283.1_3")))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 36
                StudyVw.ClickElement("Exam Mode");
                PageLoadWait.WaitForFrameLoad(20);
                String uid_36 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if (uid_36.Contains(".1235410088.283.1_3"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 37
                for (int i = 0; i < 5; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                PageLoadWait.WaitForFrameLoad(20);
                String uid_37 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ClusterViewID");
                if (uid_37.Contains(".1235410088.283.1_3"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 38
                StudyVw.ClickElement("Exam Mode");
                bool step38 = false;
                try
                {
                    StudyVw.cineplay(1, 1).Click();
                }
                catch
                {
                    if (StudyVw.cinepause(1, 1).Displayed)
                        step38 = true;
                }
                if (StudyVw.verifyFrameIndicatorLineChanging(1, 1) == 1 || step38)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 39
                result.steps[++executedSteps].status = "Not Automated";

                //step 40
                StudyVw.CloseStudy();
                studies.SelectAllDateAndData();
                studies.SearchStudy("lastname", LastName[3]);
                studies.SelectStudy("Patient ID", PatientID[2]);
                studies.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(15);
                StudyVw.ClickElement("Series Viewer 2x3");
                if (!StudyVw.CineToolbar(1, 1).Displayed && !StudyVw.CineToolbar(1, 2).Displayed && !StudyVw.CineToolbar(1, 3).Displayed && !StudyVw.CineToolbar(2, 1).Displayed && !StudyVw.CineToolbar(2, 2).Displayed && !StudyVw.CineToolbar(2, 3).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 41
                StudyVw.cineGroupPlayBtn().Click();
                if (StudyVw.CineToolbar(1, 1).Displayed && StudyVw.CineToolbar(1, 2).Displayed && !StudyVw.CineToolbar(1, 3).Displayed && !StudyVw.CineToolbar(2, 1).Displayed && !StudyVw.CineToolbar(2, 2).Displayed && !StudyVw.CineToolbar(2, 3).Displayed)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step 42
                StudyVw.DoubleClick(StudyVw.SeriesViewer_1X2());
                if (StudyVw.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Logout
                login.Logout();

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// </summary> 
        ///Exam mode - Ortho	
        /// </summary> 

        public TestCaseResult Test_89147(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String[] Accession = AccessionList.Split(':');

            String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
            String[] PatientID = PatientIDList.Split('=');


            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String DefaultDomain = "SuperAdminGroup";
                String DefaultRole = "SuperRole";


                //Step-1
                //Verify Exam mode option in Domain for every modality

                //Precodition-
                //ServiceTool Ortho Template--
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool tool = new ServiceTool();
                tool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                tool.NavigateToTab(ServiceTool.Templates_Tab);
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                wpfobject.GetListBox(0).Items[1].Click();
                tool.ClickApplyButtonFromTab();
                wpfobject.GetMainWindowByTitle("Template");
                wpfobject.ClickButton("6");
                tool.RestartIISandWindowsServices();
                tool.CloseServiceTool();
                taskbar.Show();


                login.DriverGoTo(login.url);

                String title = BasePage.Driver.Title;

                //check Ortho template 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DefaultDomain);
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();

                IList<IWebElement> ele = domain.ModalityDropDown().Options;
                bool flag = true;
                foreach (IWebElement mod in ele)
                {
                    domain.ModalityDropDown().SelectByText(mod.Text);
                    if (domain.SelectedValueOfRadioBtn("ExamModeRadioButtons").Equals("Off", StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        flag = false;
                        break;
                    }
                }

                //The option should be OFF by default for all modalities unless changed

                if (flag && title.Equals("OrthoPACS Web Viewer"))
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
                domain.ClickCloseEditDomain();
                //Step-2
                //Search and load for a multiframe study with single image for which exam mode is OFF
                Studies study = (Studies)login.Navigate("Studies");

                //study.SearchStudy(AccessionNo: Accession[0], Datasource: EA_131);
                //study.SelectStudy("Accession", Accession[0]);

                //Using different data 
                study.SearchStudy(patientID: PatientID[0], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID[0]);


                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));

                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                     viewer.Thumbnails()[0].Displayed &&
                     viewer.cineGroupPlayBtn().Displayed == true &&
                     viewer.SeriesViewPorts().Count == 6 &&
                     viewer.Thumbnails().Count == 10)
                {
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
                //Verify Exam Play mode
                bool step3_1 = true;
                bool step3_2 = false;

                foreach (IWebElement alltool in viewer.AllReviewTools())
                {
                    if (alltool.GetAttribute("title").Equals("Global Stack"))
                    {
                        step3_2 = true;
                        break;
                    }
                }

                foreach (IWebElement alltool in viewer.AllReviewTools())
                {
                    if (alltool.GetAttribute("title").Equals("Exam Mode"))
                    {
                        step3_1 = false;
                        break;
                    }
                }

                //Exam play mode should not be displayed in review toolbar
                if (step3_1 && step3_2)
                {
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
                //"Close study
                //In Domain management for the modality to which the listed study belongs
                //Enable exam mode
                //add exam mode icon in review toolbar
                //Reload the study in viewer"

                study.CloseStudy();
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DefaultDomain);
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();

                //All modalities Exam Mode ==> On
                IList<IWebElement> modality = domain.ModalityDropDown().Options;
                foreach (IWebElement mod in modality)
                {
                    domain.ModalityDropDown().SelectByText(mod.Text);
                    if (domain.SelectedValueOfRadioBtn("ExamModeRadioButtons").Equals("Off", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        domain.SelectRadioBtn("ExamModeRadioButtons", "On");
                    }
                }
                domain.ClickSaveEditDomain();

                //The study should be loaded without any error
                study = (Studies)login.Navigate("Studies");
                //study.SearchStudy(AccessionNo: Accession[0], Datasource: EA_131);
                //study.SelectStudy("Accession", Accession[0]);

                //Using different data 
                study.SearchStudy(patientID: PatientID[0], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID[0]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));

                if (viewer.SeriesViewer_1X1().Displayed &&
                     viewer.Thumbnails()[0].Displayed &&
                     viewer.cineGroupPlayBtn().Displayed == true &&
                     viewer.SeriesViewPorts().Count == 6 &&
                     viewer.Thumbnails().Count == 10)
                {
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
                //Verify the Global Stack tool

                bool step5_1 = true;
                foreach (IWebElement alltool in viewer.AllReviewTools())
                {
                    if (alltool.GetAttribute("title").Equals("Global Stack"))
                    {
                        step5_1 = false;
                        break;
                    }
                }

                bool step5_2 = false;
                foreach (IWebElement alltool in viewer.AllReviewTools())
                {
                    if (alltool.GetAttribute("title").Equals("Exam Mode"))
                    {
                        step5_2 = true;
                        break;
                    }
                }

                //Global Stack tool should not be displayed
                if (step5_1 && step5_2)
                {
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
                //Select a viewport with single frame/image and verify cine toolbar

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewer.SeriesViewer_2X3().Click();

                //cine toolbar should not be displayed
                if (viewer.cineplay(2, 3).Displayed == false &&
                    viewer.cineNextFramebtn(2, 3).Displayed == false &&
                    viewer.cinePrevFramebtn(2, 3).Displayed == false &&
                    viewer.cineNextClipBtn(2, 3).Displayed == false &&
                    viewer.cinePrevClipBtn(2, 3).Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-7
                //Select Exam Play mode
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));
                //Exam play mode should be enabled
                //Cine toolbar should be displayed

                if (viewer.GetReviewToolImage("Exam Mode").GetAttribute("class").Contains("toggleItem_On") &&
                    viewer.cineplay(1, 2).Displayed == true &&
                 viewer.cineNextFramebtn(1, 2).Displayed == true &&
                 viewer.cinePrevFramebtn(1, 2).Displayed == true &&
                 viewer.cineNextClipBtn(1, 2).Displayed == true &&
                 viewer.cinePrevClipBtn(1, 2).Displayed == true)
                {
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
                //Verify the Global Stack tool
                //Global Stack tool should not be displayed

                bool step8 = true;
                foreach (IWebElement alltool in viewer.AllReviewTools())
                {
                    if (alltool.GetAttribute("title").Equals("Global Stack"))
                    {
                        step8 = false;
                        break;
                    }
                }

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

                //Step-9
                //Click Cine

                viewer.cineplay(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));
                //PageLoadWait.WaitForCineToPlay(1, 2);

                //Cine should start in the viewport
                PageLoadWait.WaitForThumbnailBorderColorToChange(2);
                PageLoadWait.WaitForThumbnailBorderColorToChange(3);


                //Step-10
                //Verify the images being played
                //Cine plays all frames from the image and never stops
                PageLoadWait.WaitForThumbnailBorderColorToChange(4);
                PageLoadWait.WaitForThumbnailBorderColorToChange(7);

                //PageLoadWait.WaitForThumbnailBorderColorToChange(8);


                bool step10 = viewer.verifyFrameIndicatorLineChanging(1, 2) == 1;

                //PageLoadWait.WaitForThumbnailBorderColorToChange(9);

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


                //Step-11
                //Pause cine

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));
                viewer.cinepause(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));


                string ClassAttribute = "ui-droppable";
                string ClassAttribute2 = "svViewerImg";


                //Cine should pause in the viewport

                if (viewer.cinepause(1, 2).Displayed == false &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.cineplay(1, 2).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2) == -1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //Start cine

                viewer.cineplay(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));

                //Cine should resume from the frame where it was paused

                PageLoadWait.WaitForThumbnailBorderColorToChange(1);

                //PageLoadWait.WaitForCineToPlay(1, 2);
                bool step12 = viewer.verifyFrameIndicatorLineChanging(1, 2) == 1;

                if (step12 &&
                    viewer.cinepause(1, 2).Displayed == true &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 2).Displayed == true)
                {
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
                //Close study and Verify Exam mode option in Role

                study.CloseStudy();
                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();
                PageLoadWait.WaitForFrameLoad(15);
                role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                IList<IWebElement> roleModalities = role.ModalityDropDown().Options;
                bool flag_13 = true;
                foreach (IWebElement mod in roleModalities)
                {
                    role.ModalityDropDown().SelectByText(mod.Text);
                    if (role.SelectedValueOfRadioBtn("ExamModeRadioButtons").Equals("Off", StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        flag_13 = false;
                        break;
                    }
                }

                //The option should be OFF by default for all modalities unless changed
                if (flag_13)
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

                //Step-14
                //In Role management for the modality to which the listed study belongs Enable Exam mode
                roleModalities = role.ModalityDropDown().Options;
                foreach (IWebElement mod in roleModalities)
                {
                    role.ModalityDropDown().SelectByText(mod.Text);
                    if (role.SelectedValueOfRadioBtn("ExamModeRadioButtons").Equals("Off", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        role.SelectRadioBtn("ExamModeRadioButtons", "On");
                    }
                }

                //The preferences should be saved

                roleModalities = role.ModalityDropDown().Options;
                bool flag_14 = true;
                foreach (IWebElement mod in roleModalities)
                {
                    role.ModalityDropDown().SelectByText(mod.Text);
                    if (role.SelectedValueOfRadioBtn("ExamModeRadioButtons").Equals("On", StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        flag_14 = false;
                        break;
                    }
                }

                if (flag_14)
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
                role.ClickSaveEditRole();

                //Step-15
                //from studylist Search and load for a multiframe study with multiple images

                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(patientID: PatientID[0], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID[0]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                     viewer.Thumbnails()[0].Displayed &&
                     viewer.cineplay(1, 1).Displayed &&
                     viewer.cineGroupPlayBtn().Displayed &&
                     viewer.SeriesViewPorts().Count == 6 &&
                     viewer.Thumbnails().Count == 10)
                {
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
                //Click Cine

                viewer.cineplay(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 1)));
                PageLoadWait.WaitForCineToPlay(1, 1);

                //Cine should start in the viewport
                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                    viewer.cinepause(1, 1).Displayed == true &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 2000) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17
                //Verify the images being played
                //Cine plays all frames from the image and never stops

                if (viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute2) &&
                    viewer.cinepause(1, 1).Displayed == true &&
                    viewer.cinestop(1, 1).Displayed == true &&
                    viewer.FrameIndicatorLine(1, 1).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 1) == 1)
                {
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
                //Select Exam Play mode

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);

                //Exam play mode should be enabled

                if (viewer.GetReviewToolImage("Exam Mode").GetAttribute("class").Contains("toggleItem_On"))
                {
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
                //Verify the images being played
                //Cine plays all images and never stops

                PageLoadWait.WaitForThumbnailBorderColorToChange(4);
                PageLoadWait.WaitForThumbnailBorderColorToChange(5);

                PageLoadWait.WaitForThumbnailBorderColorToChange(7);
                Thread.Sleep(3000);
                bool step_19_2 = viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 2000) == 1;

                if (step_19_2)
                {
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
                //Verify the thumbnail highlighting
                //The thumbnail for the series on which cine is currently playing should be highlighted

                ExecutedSteps++; //have to do cluster UID verification

                //Step-21
                //Verify Exam mode option in User preferences
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 1)));
                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                UserPreferences userpref = new UserPreferences();
                //userpref.OpenUserPreferences();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.UserPreference);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_UserprefFrame");
                PageLoadWait.WaitForPageLoad(20);

                IList<IWebElement> userpref_Modalities = viewer.ModalityDropdown().Options;
                bool flag_21 = true;
                foreach (IWebElement mod in userpref_Modalities)
                {
                    viewer.ModalityDropdown().SelectByText(mod.Text);
                    if (viewer.SelectedValueOfRadioBtn("ExamModeRadioButtons").Equals("Off", StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        flag_21 = false;
                        break;
                    }
                }



                //The option should be OFF by default for all modalities unless changed
                if (flag_21)
                {
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
                //"In User preferences for the modality to which the listed study belongs Enable exam mode
                //Close study and Search and 
                //load for a study with multiple images in single series(Multiple Gated Acquisition NM)
                //SOP Class UID -> 1.2.840.10008.5.1.4.1.1.20"

                viewer.ModalityDropdown().SelectByText("NM");
                if (viewer.SelectedValueOfRadioBtn("ExamModeRadioButtons").Equals("Off", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    viewer.SelectRadioBtn("ExamModeRadioButtons", "On");
                }

                PageLoadWait.WaitForPageLoad(20);

                userpref.SavePreferenceBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(userpref.CloseBtn()));
                userpref.CloseBtn().Click();


                PageLoadWait.WaitForPageLoad(20);

                study.CloseStudy();

                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: Accession[1], Modality: "NM", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                     viewer.Thumbnails()[0].Displayed &&
                     viewer.cineplay(1, 1).Displayed &&
                     viewer.cineGroupPlayBtn().Displayed &&
                     viewer.SeriesViewPorts().Count == 4 &&
                     viewer.Thumbnails().Count == 12)
                {
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
                //Select Exam Play mode
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);

                //Exam play mode should be enabled

                if (viewer.GetReviewToolImage("Exam Mode").GetAttribute("class").Contains("toggleItem_On"))
                {
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
                //Click Cine
                viewer.cineplay(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 1)));

                //Cine should start in the viewport



                if (viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 2000) == 1)
                {
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
                //Verify the images being played
                //Cine plays all images from the series and never stops
                PageLoadWait.WaitForThumbnailBorderColorToChange(1);


                bool step_25_1 = viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 2000) == 1;

                PageLoadWait.WaitForThumbnailBorderColorToChange(3);

                bool step_25_2 = viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 2000) == 1;

                if (step_25_1 && step_25_2)
                {
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
                //Close study and Search and load for a study with multiple images in multiple series

                study.CloseStudy();

                study.SearchStudy(AccessionNo: Accession[3], Datasource: EA_91);
                study.SelectStudy("Accession", Accession[3]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                     viewer.Thumbnails()[0].Displayed &&
                     viewer.cineplay(1, 1).Displayed &&
                     viewer.cineGroupPlayBtn().Displayed &&
                     viewer.SeriesViewPorts().Count == 4 &&
                     viewer.Thumbnails().Count == 5)
                {
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
                //Select Exam Play mode
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);

                //Exam play mode should be enabled
                if (viewer.GetReviewToolImage("Exam Mode").GetAttribute("class").Contains("toggleItem_On"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28
                //Scroll images in series till you reach the last image in series now scroll once more

                IWebElement source = viewer.ViewportScrollHandle(1, 1);
                IWebElement destination = viewer.ViewportScrollBar(1, 1);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                bool step_28 = viewer.ThumbnailIndicator()[0].Displayed == true;

                int oldCount = viewer.ThumbnailLoadedIndicator().Count;

                String Thumbnail_SeriesID_28_1 = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_28_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");

                viewer.ClickDownArrowbutton(1, 1);

                //The image from next series should load

                String Thumbnail_SeriesID_28_2 = viewer.GetInnerAttribute(viewer.Thumbnails()[1], "src", '&', "seriesUID");
                String Viewerport_ClusterViewID_28_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ClusterViewID");

                PageLoadWait.WaitForFrameLoad(30);

                int curCount = viewer.ThumbnailLoadedIndicator().Count;

                if (step_28 && oldCount == (curCount + 1) &&
                    viewer.ThumbnailIndicator()[0].Displayed == false &&
                    viewer.ThumbnailIndicator()[1].Displayed == true &&
                    Thumbnail_SeriesID_28_1 != Thumbnail_SeriesID_28_2 &&
                    Thumbnail_SeriesID_28_1 != null && Thumbnail_SeriesID_28_2 != null &&
                    Viewerport_ClusterViewID_28_1.Contains(Thumbnail_SeriesID_28_1) &&
                    Viewerport_ClusterViewID_28_2.Contains(Thumbnail_SeriesID_28_2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29
                //Click Cine

                viewer.cineplay(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 1)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinestop(1, 1)));

                //Cine should start in the viewport

                if (viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 5000) == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30
                //Verify the images being played
                //Cine plays all images from all series and never stops

                PageLoadWait.WaitForThumbnailBorderColorToChange(1);

                bool step_30_1 = viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 5000) == 1;

                PageLoadWait.WaitForThumbnailBorderColorToChange(3);

                bool step_30_2 = viewer.verifyFrameIndicatorLineChanging(1, 1, interval: 5000) == 1;

                if (step_30_1 && step_30_2)
                {
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
                //Stop cine when the images from second series are displayed
                viewer.cinestop(1, 1).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //The image which was displayed when cine was stopped from series#2 should load in the viewer
                //Note- if user stopped cine on image #5, series#2 image#5 should be loaded in viewport after stopping

                if (
                     viewer.SeriesViewer_1X1().Displayed == true &&
                     viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == false &&
                     viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                     viewer.cinepause(1, 1).Displayed == false &&
                     viewer.cinestop(1, 1).Displayed == false &&
                     viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                     viewer.cineplay(1, 1).Displayed == true &&
                     viewer.verifyFrameIndicatorLineChanging(1, 1) == 0)
                {
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
                //"Close study and In User preferences for the modality to which the listed study belongs
                //set the Cine Single Frame Delay Time to 5sec
                //Delay on Single frame On

                study.CloseStudy();
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CineFrameDelay().Clear();
                userpref.CineFrameDelay().SendKeys("5");
                String timeSec = userpref.CineFrameDelay().GetAttribute("value");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                //The preferences should be saved
                if (timeSec.Equals("5"))
                {
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
                //load the study in viewer with PR and KO

                study.SearchStudy(AccessionNo: Accession[4], Datasource: EA_131);
                study.SelectStudy("Accession", Accession[4]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 2)));

                //The study should be loaded without any error

                if (viewer.SeriesViewer_1X1().Displayed &&
                     viewer.Thumbnails()[0].Displayed &&
                    //viewer.cineplay(1, 1).Displayed &&
                     viewer.cineGroupPlayBtn().Displayed &&
                     viewer.SeriesViewPorts().Count == 4 &&
                     viewer.Thumbnails().Count == 19)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34
                //Select Exam Play mode
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineplay(1, 1)));

                //Exam play mode should be enabled
                if (viewer.GetReviewToolImage("Exam Mode").GetAttribute("class").Contains("toggleItem_On"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35
                //Scroll through the images
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);

                //PR
                viewer.ClickDownArrowbutton(1, 2);
                //viewer.ClickDownArrowbutton(1, 2);

                //KO
                viewer.ClickDownArrowbutton(2, 2);
                //viewer.ClickDownArrowbutton(2, 2);

                //All the images from all the series should be displayed including PR and KO
                if (
                    viewer.SeriesViewer_1X2().Displayed == true &&
                    viewer.SeriesViewer_2X2().Displayed == true &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("2") &&
                    viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("2") &&
                     viewer.ThumbnailCaptions()[0].Text.Contains("KO") &&
                     viewer.ThumbnailCaptions()[3].Text.Contains("PR"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-36
                //Scroll to an image from last series set Exam mode to OFF

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //off
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                bool step36 = viewer.GetReviewToolImage("Exam Mode").GetAttribute("class").Contains("toggleItem_On");

                //The image from last series should still be loaded in viewer

                if (step36 == false &&
                    viewer.SeriesViewer_1X2().Displayed == true &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("3") &&
                    viewer.ThumbnailIndicator()[0].Displayed == true &&
                    viewer.ThumbnailIndicator()[1].Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-37
                //scroll images
                viewer.ClickDownArrowbutton(1, 2);

                //images from the last series should only be displayed

                if (viewer.SeriesViewer_1X2().Displayed == true &&
                 viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("3") &&
                 viewer.ThumbnailIndicator()[0].Displayed == true &&
                 viewer.ThumbnailIndicator()[1].Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-38
                //set exam mode to ON and start Cine
                //on
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ExamMode);
                viewer.cineplay(1, 2).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cinepause(1, 2)));

                //Cine should start in the viewport

                PageLoadWait.WaitForThumbnailBorderColorToChange(1);
                PageLoadWait.WaitForThumbnailBorderColorToChange(6);

                //PageLoadWait.WaitForCineToPlay(1, 1);

                if (viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") &&
                    viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) &&
                    viewer.cinepause(1, 2).Displayed == true &&
                    viewer.cinestop(1, 2).Displayed == true &&
                    viewer.verifyFrameIndicatorLineChanging(1, 2, interval: 2000) == 1 && //CINE is playing
                    viewer.FrameIndicatorLine(1, 2).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-39
                //Verify the images being played

                //When secondary capture image is loaded, cine delays the display of next image by 5sec
                //After 5sec cine resumes at configured fps
                //Cine plays all images from all series including PR and KO and never stops

                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-40
                //Close study
                //Load a study with multiple single image series

                study.CloseStudy();
                study.SearchStudy(patientID: Accession[5], Datasource: EA_131);
                study.SelectStudy("Accession", Accession[5]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                //Images should load without any error
                //cine toolbar should not be dsplayed

                if (viewer.SeriesViewer_1X1().Displayed &&
                     viewer.Thumbnails()[0].Displayed &&
                     viewer.cineplay(1, 1).Displayed == false &&
                     viewer.cineplay(1, 2).Displayed == false &&
                     viewer.cineplay(2, 1).Displayed == false &&
                     viewer.cineplay(2, 2).Displayed == false &&
                     viewer.cineGroupPlayBtn().Displayed &&
                     viewer.SeriesViewPorts().Count == 4 &&
                     viewer.Thumbnails().Count == 7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-41
                //CLick group play
                viewer.cineGroupPlayBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.cineGroupPauseBtn()));

                //Cine should not start in any viewport

                if (viewer.SeriesViewer_1X1().Displayed &&
                    viewer.SeriesViewer_1X2().Displayed &&
                    viewer.SeriesViewer_2X1().Displayed &&
                    viewer.SeriesViewer_2X2().Displayed &&

                    viewer.cineViewport(1, 1).GetAttribute("style").Contains("cursor: default") == false &&
                  viewer.cineViewport(1, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(1, 1).Displayed == false &&
                  viewer.cinestop(1, 1).Displayed == false &&
                  viewer.FrameIndicatorLine(1, 1).Displayed == false &&
                  viewer.verifyFrameIndicatorLineChanging(1, 1) == 0 &&

                  viewer.cineViewport(1, 2).GetAttribute("style").Contains("cursor: default") == false &&
                  viewer.cineViewport(1, 2).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(1, 2).Displayed == false &&
                  viewer.cinestop(1, 2).Displayed == false &&
                  viewer.FrameIndicatorLine(1, 2).Displayed == false &&
                  viewer.verifyFrameIndicatorLineChanging(1, 2) == 0 &&

                  viewer.cineViewport(2, 1).GetAttribute("style").Contains("cursor: default") == false &&
                  viewer.cineViewport(2, 1).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(2, 1).Displayed == false &&
                  viewer.cinestop(2, 1).Displayed == false &&
                  viewer.FrameIndicatorLine(2, 1).Displayed == false &&
                  viewer.verifyFrameIndicatorLineChanging(2, 1) == 0 &&

                  viewer.cineViewport(2, 2).GetAttribute("style").Contains("cursor: default") == false &&
                  viewer.cineViewport(2, 2).GetAttribute("class").Contains(ClassAttribute) == false &&
                  viewer.cinepause(2, 2).Displayed == false &&
                  viewer.cinestop(2, 2).Displayed == false &&
                  viewer.FrameIndicatorLine(2, 2).Displayed == false &&
                  viewer.verifyFrameIndicatorLineChanging(2, 2) == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-42
                //Double click on viewport

                action.DoubleClick(viewer.SeriesViewer_1X1()).Build().Perform();

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));

                //Viewport should be maximized and displayed in 1x1 layout with Cine playing in exam mode

                if (viewer.SeriesViewer_1X1().Displayed &&
                    viewer.ThumbnailIndicator()[0].Displayed == true &&
                    viewer.ThumbnailLoadedIndicator().Count == 1 &&
                   viewer.cineplay(1, 1).Displayed == false &&
                   viewer.cineGroupPlayBtn().Displayed == false &&
                   viewer.SeriesViewPorts().Count == 1 &&
                   viewer.Thumbnails().Count == 7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                study.CloseStudy();
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
                //Revoke back to Cardiology
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.Templates_Tab);
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                wpfobject.GetListBox(0).Items[0].Click();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.GetMainWindowByTitle("Template");
                wpfobject.ClickButton("6");
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                taskbar.Show();

            }
        }


    }
}
