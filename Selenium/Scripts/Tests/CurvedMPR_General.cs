using System;
using System.Collections.Generic;
using System.Drawing;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Selenium.Scripts.Tests
{
    class CurvedMPR_General : BasePage
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

        public CurvedMPR_General(String classname)
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

        public TestCaseResult Test_163287(String testid, String teststeps, int stepcount) // Curved MPR view
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
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

                //step 01::From iCA, Load a study in the 3D viewer.1.Navigate to 3D tab and Click Curved MPR mode from the dropdown.
                login.LoginIConnect(adminUserName, adminPassword);
                bool Layout = z3dvp.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                
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
                
                //Steps 2::Note that the Curved MRP view is displayed as a 2 x 3 Grid view with below 
                //1) 3 MPR navigation controls
                //2)1 MPR path navigation control
                //3)1 3D path navigation control
                //4) 1 Curved MPR control.
                IWebElement Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int LocNav1X = Nav1.Location.X;
                int LocNav1Y = Nav1.Location.Y;
                IWebElement Nav2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int LocNav2X = Nav2.Location.X;
                int LocNav2Y = Nav2.Location.Y;
                IWebElement ThreeDPathNav = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                int LocThreeDPathNavX = ThreeDPathNav.Location.X;
                int LocThreeDPathNavY = ThreeDPathNav.Location.Y;

                IWebElement Nav3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                int LocNav3X = Nav3.Location.X;
                int LocNav3Y = Nav3.Location.Y;
                IWebElement MPRPathnav = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                int LocMPRPathnavX = MPRPathnav.Location.X;
                int LocMPRPathnavY = MPRPathnav.Location.Y;
                IWebElement CurvedMpr = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int LocCurvedMprX = CurvedMpr.Location.X;
                int LocCurvedMprY = CurvedMpr.Location.Y;
                if (LocNav1Y.Equals(LocNav2Y) && LocNav2Y.Equals(LocThreeDPathNavY) && LocNav1X < LocNav2X && LocNav2X < LocThreeDPathNavX
                    && LocNav3Y.Equals(LocMPRPathnavY) && LocMPRPathnavY.Equals(LocMPRPathnavY) && LocNav3X < LocMPRPathnavX && LocMPRPathnavX < LocCurvedMprX)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Resize the 3D viewer's browser window is on portrait view mode, the width needs to be < the height. 
                Driver.Manage().Window.Size = new Size(750, 950);
                PageLoadWait.WaitForFrameLoad(20);
                Nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                LocNav1X = Nav1.Location.X;
                LocNav1Y = Nav1.Location.Y;
                Nav2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                LocNav2X = Nav2.Location.X;
                LocNav2Y = Nav2.Location.Y;

                Nav3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                LocNav3X = Nav3.Location.X;
                LocNav3Y = Nav3.Location.Y;
                MPRPathnav = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                LocMPRPathnavX = MPRPathnav.Location.X;
                LocMPRPathnavY = MPRPathnav.Location.Y;

                ThreeDPathNav = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                LocThreeDPathNavX = ThreeDPathNav.Location.X;
                LocThreeDPathNavY = ThreeDPathNav.Location.Y;
                CurvedMpr = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                LocCurvedMprX = CurvedMpr.Location.X;
                LocCurvedMprY = CurvedMpr.Location.Y;
                if (LocNav1Y.Equals(LocNav2Y) && LocNav3Y.Equals(LocMPRPathnavY) && LocThreeDPathNavY.Equals(LocCurvedMprY))
                {
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
                Driver.Manage().Window.Maximize();
            }
        }

        public TestCaseResult Test_163289(String testid, String teststeps, int stepcount) //Active Cursor
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage Bluering = new BluRingZ3DViewerPage();
            TestCompleteAction TCActions = new TestCompleteAction();
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

                //step 01::From iCA, Load a study in the 3D viewer.1.Navigate to 3D tab and Click Curved MPR mode from the dropdown.
                login.LoginIConnect(adminUserName, adminPassword);
                bool Layout = Bluering.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
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
                
                //Steps 2::Hover the mouse pointer over the images.
                Actions builder = new Actions(Driver);
                builder.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), (z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).Size.Height / 2)).Build().Perform();
                //Verification::Curve drawing cursor shows up while hovering the image.
                bool CurvedMPRCursor = z3dvp.VerifyToolSelected(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.CurveDrawingTool);
                if (CurvedMPRCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Select the MPR 4:1 view from the 3D dropdown menu.
                bool MPR4x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                //Verification::MPR 4:1 viewing mode should be displayed
                if (MPR4x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 4::Hover the mouse pointer over the images.
                builder = new Actions(Driver);
                builder.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Width / 2), (z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel).Size.Height / 2)).Build().Perform();
                //Verification::Scroll cursor shows up while hovering the image.
                bool ScrollCursor = z3dvp.VerifyToolSelected(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.ScrollTool);
                if (ScrollCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Again Select the Curved MPR view from the 3D dropdown and check the active cursor.
                bool CurvedMPR = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                CurvedMPRCursor = z3dvp.VerifyToolSelected(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.CurveDrawingTool);
                //Verification::Curve drawing cursor shows up while hovering the image.
                if (CurvedMPR && CurvedMPRCursor)
                {
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
            }
        }

        public TestCaseResult Test_163288(String testid, String teststeps, int stepcount) //Synchronize MPR navigation controls
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
            String ActualLocationValue = "Loc: 0.0, 0.0, 0.0 mm";
            String TraverseLocationValue = "Loc: 0.0, 0.0, 75.2 mm";
            String TraverseOriginalValue = "Loc: 0.0, 0.0, 0.4 mm";
            String Nav3Location = "Loc: 0.0, 0.0, 100.0 mm";
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                //step 01::From iCA, Load a study in the 3D viewer.1.Navigate to 3D tab and Click Curved MPR mode from the dropdown.
                login.LoginIConnect(adminUserName, adminPassword);
                bool Layout = Bluering.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                //Verification::Series loaded with out any errors in MPR 4:1 mode.
                if (Layout)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Unable to load study successfully");
                
                //Steps 2::Apply the zoom tool to navigation control 1.
                PageLoadWait.WaitForFrameLoad(10);
                bool ss = z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement navv = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.Performdragdrop(navv, navv.Size.Width / 4, 3 * (navv.Size.Height / 4),navv.Size.Width / 4, navv.Size.Height / 4 );
                PageLoadWait.WaitForFrameLoad(10);
             //   String LeftMprNav1 = ""; String LeftMprNav2 = ""; String LeftMprNav3 = "";
                //Verification::Magnification is modified equally on navigation controls 1-3 that are displayed in the Curved MPR view.
                //String LeftMprNav1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //String LeftMprNav2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //String LeftMprNav3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                List<string> result2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                bool res2 = z3dvp.checkerrormsg();
                if (res2)
                    throw new Exception("Failed due to error message");
                
                if(result2[0]== result2[1] && result2[3]== result2[1] && result2[1]!= ActualLocationValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Press the reset button.
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Images in the all controls resets to its initial position.
                //LeftMprNav1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //LeftMprNav2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //LeftMprNav3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                List<string> result3 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result3[0].Equals(ActualLocationValue) && result3[1].Equals(ActualLocationValue) && result3[3].Equals(ActualLocationValue))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys("X").Build().Perform();
               //Steps 4::Take a screen shot of the 3D viewer and paste in to MS paint.
                //IWebElement ControlElement = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //String CentrTopAnnotationNav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] BOrientationValueNav1 = CentrTopAnnotationNav1.Split('\r');
                //String CentrTopAnnotationNav2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] BOrientationValueNav2 = CentrTopAnnotationNav2.Split('\r');
                //String CentrTopAnnotationNav3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] BOrientationValueNav3 = CentrTopAnnotationNav3.Split('\r');
                //if (BOrientationValueNav1[0].Equals("H") && BOrientationValueNav2[0].Equals("H") && BOrientationValueNav3[0].Equals("A"))
                IList<string> checkvalue4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if(checkvalue4[0]=="H" && checkvalue4[1] =="H" && checkvalue4[3]=="A")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 5::Rotate the cross hairs of navigation control 2 by 180 degrees clockwise.
                IWebElement nav2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(nav2, 3 * (nav2.Size.Width /4), nav2.Size.Height / 2 , nav2.Size.Width / 4, nav2.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                //CentrTopAnnotationNav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] AOrientationValueNav1 = CentrTopAnnotationNav1.Split('\r');
                //CentrTopAnnotationNav2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] AOrientationValueNav2 = CentrTopAnnotationNav2.Split('\r');
                //CentrTopAnnotationNav3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] AOrientationValueNav3 = CentrTopAnnotationNav3.Split('\r');
                //if (AOrientationValueNav1[0].Equals("F") && AOrientationValueNav3[0].Equals("P"))
                IList<string> checkvalue5 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (checkvalue5[0] == "F" && checkvalue5[3] == "P" )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 6::Apply the roam tool to navigation control 3.
                z3dvp.select3DTools(Z3DTools.Pan);
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.Performdragdrop(Navigation3, Navigation3.Size.Width / 4, 3 * (Navigation3.Size.Height / 4), Navigation3.Size.Width / 4, Navigation3.Size.Height / 4);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Roam is applied only to navigation controls 1-3 that are displayed in the Curved MPR View and the position annotation remains the same for all controls.
                //CentrTopAnnotationNav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //AOrientationValueNav1 = CentrTopAnnotationNav1.Split('\r');
                //CentrTopAnnotationNav2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //AOrientationValueNav2 = CentrTopAnnotationNav2.Split('\r');
                //CentrTopAnnotationNav3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //AOrientationValueNav3 = CentrTopAnnotationNav3.Split('\r');
                //String CentrTopAnnotation3Dpath = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] AOrientationValue3Dpath = CentrTopAnnotation3Dpath.Split('\r');
                //String CentrTopAnnotationMPrPath = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                //string[] AOrientationValueMPrPath = CentrTopAnnotationMPrPath.Split('\r');
                //LeftMprNav1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //LeftMprNav2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //LeftMprNav3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //if (AOrientationValueNav1[0].Equals("F") && AOrientationValueNav2[0].Equals("H") && AOrientationValueNav3[0].Equals("P") &&
                //    AOrientationValue3Dpath[0].Equals("A") && AOrientationValueMPrPath[0].Equals("A") &&
                //    LeftMprNav1.Equals(LeftMprNav2) && LeftMprNav2.Equals(LeftMprNav3) && LeftMprNav3.Equals(LeftMprNav1))
                List<string> result6_position = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                List<string> result6_zoomvalue = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                bool res6 = z3dvp.checkerrormsg();
                if (res6)
                    throw new Exception("Error message found");
               
                if(result6_position[0]=="F" && result6_position[1]=="H" && result6_position[3]=="P" && result6_position[2]=="A" && result6_position[4]=="A" && result6_zoomvalue[0]== result6_zoomvalue[1] && result6_zoomvalue[3]== result6_zoomvalue[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps7::Apply the scroll tool to navigation control 2.
                new Actions(Driver).SendKeys("X").Build().Perform();
                bool ScrollTool = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                IWebElement ViewerContainer = z3dvp.ViewerContainer();
                //z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.Performdragdrop(nav2, nav2.Size.Width / 4, 3 * (nav2.Size.Height / 4) , nav2.Size.Width / 4, nav2.Size.Height / 4 );
                PageLoadWait.WaitForFrameLoad(10);
                bool res7 = z3dvp.checkerrormsg();
                if (res7)
                    throw new Exception("Error message found");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance: 999) && ScrollTool)
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
                //Steps 8::Apply the window/level tool to navigation control 2.
                new Actions(Driver).SendKeys("X").Build().Perform();
                bool Windowlevel = z3dvp.select3DTools(Z3DTools.Window_Level);
                //z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.Performdragdrop(nav2, nav2.Size.Width / 4, 3 * (nav2.Size.Height / 4), nav2.Size.Width / 4, nav2.Size.Height / 4);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(nav2, nav2.Size.Width / 4, 3 * (nav2.Size.Height / 4), nav2.Size.Width / 4, nav2.Size.Height / 4);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Window/level is modified equally for the image in all controls except the 3D path navigation controls.
                //string LeftTopAnnotationNav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                //string[] Nav1 = LeftTopAnnotationNav1.Split('\n');
                //string LeftTopAnnotationNav2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                //string[] Nav2 = LeftTopAnnotationNav2.Split('\n');
                //string LeftTopAnnotationNav3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                //string[] Nav3 = LeftTopAnnotationNav3.Split('\n');
                //string LeftTopAnnotationCurved = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                //string[] Curved = LeftTopAnnotationCurved.Split('\n');
                //string LeftTopAnnotationPathNav = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                //string[] MprPathNav = LeftTopAnnotationPathNav.Split('\n');
                //string LeftTopAnnotation3D = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).Text;
                //string[] ThreeD = LeftTopAnnotation3D.Split('\n');
                List<string> checkvalue8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                bool res8 = z3dvp.checkerrormsg();
                if (res8)
                    throw new Exception("Error message found");
              //  if (Nav1[3].Equals(Nav2[3]) && Nav2[3].Equals(Nav3[3]) && Nav3[3].Equals(Curved[3]) && Curved[3].Equals(MprPathNav[3]) && !Curved[3].Equals(ThreeD[3]))
              if(checkvalue8[0]== checkvalue8[1] && checkvalue8[3]== checkvalue8[1] && checkvalue8[4]== checkvalue8[1] && checkvalue8[5] == checkvalue8[1] && checkvalue8[2]!= checkvalue8[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 9::Adjust the thickness to be 10.0 mm on navigation control 2.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, "10.0");
                //Verification::Thickness for all navigation controls 1-3 only are set to 10.0 mm.
                Thread.Sleep(1500);
                //Boolean step9_1 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.Navigationone).Equals("10" + " mm");
                //Boolean step9_2 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                //Boolean step9_3 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree).Equals("10" + " mm");
                //if (step9_1 && step9_2 && step9_3)
                List<string> checkvalue9 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Thickness, "10 mm");
                if(checkvalue9[0]== checkvalue9[1] && checkvalue9[1]== checkvalue9[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Apply a window/level preset to the navigation control 3.
                List<string> beforepreset10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                bool SelectPreset = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.BoneBody, "Preset");
                //Verification::indow/level is modified equally for the image in all controls except the 3D path navigation controls.
                //bool WlNav1 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.BoneBody , "Preset");
                //bool WlNav2 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.BoneBody , "Preset");
                //bool WlNav3 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.BoneBody , "Preset");
                //bool WlMPRPathNav = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.BoneBody , "Preset");
                List<string> checkvalue10 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.BoneBody);
                ViewerContainer = z3dvp.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool res10 = z3dvp.checkerrormsg();
                if (res10)
                    throw new Exception("Error message found");
                //   bool step10 = CompareImage(result.steps[ExecutedSteps], ViewerContainer);
                List<string> afterpreset10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                bool bflagpreset10 = false;
                if (beforepreset10[0] != afterpreset10[0] && beforepreset10[1] != afterpreset10[1] && beforepreset10[2] == afterpreset10[2] && beforepreset10[3] != afterpreset10[3] && beforepreset10[4] != afterpreset10[4] && beforepreset10[5] != afterpreset10[5])
                    bflagpreset10 = true;
                if (bflagpreset10 && SelectPreset && checkvalue10[0]== BluRingZ3DViewerPage.BoneBody && checkvalue10[1] == BluRingZ3DViewerPage.BoneBody && checkvalue10[3] == BluRingZ3DViewerPage.BoneBody
                    && checkvalue10[4] == BluRingZ3DViewerPage.BoneBody  && checkvalue10[2]!= BluRingZ3DViewerPage.BoneBody)
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

                //Steps 11::Press the reset button.
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForPageLoad(5);
                bool ResetTool = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForPageLoad(5);
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR); PageLoadWait.WaitForPageLoad(5);
                //Verification::Images in the all controls resets to its initial position.
                //string Steps11_1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //string Steps11_2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //string Steps11_3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //string Steps11_4 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //string Steps11_5 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //string Steps11_6 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CurvedMPR);
                //if (Steps11_1.Equals(ActualLocationValue) && Steps11_2.Equals(ActualLocationValue) && Steps11_3.Equals(ActualLocationValue) &&
                //    Steps11_4.Equals(ActualLocationValue) && Steps11_5.Equals(ActualLocationValue) && Steps11_6.Equals(ActualLocationValue))
                List<string> result11 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result11[0]== ActualLocationValue &&  result11[1]== ActualLocationValue && result11[2]== ActualLocationValue &&  result11[3]== ActualLocationValue
                    && result11[4]== ActualLocationValue && result11[5] == ActualLocationValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 12::Using the traverse tool, move the cross hair of navigation control 1 upward until the position annotation of navigation control 3 reads "0, 0, 100".
                IWebElement nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.Performdragdrop(nav1, nav1.Size.Width / 2, nav1.Size.Height / 6);
                PageLoadWait.WaitForFrameLoad(5);
                //   string LocationValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                String[] locval = result12[3].Split(' ');
                bool step12 = false;
                Double a = Convert.ToDouble(locval[3]);
                if (Convert.ToInt32(a) >= 90 && Convert.ToInt32(a) < 110)
                {
                    step12 = true;
                }
                //Verification::Navigation image 3 is updated, and the Z component of its position annotation increases to 100.Navigation controls 1 and 2 and the result control are unchanged.
                //string Steps12_1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //string Steps12_2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //string Steps12_3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
              //  if (Steps12_1.Equals(ActualLocationValue) && Steps12_2.Equals(ActualLocationValue) && step12)
              if(result12[0]== ActualLocationValue && result12[1] == ActualLocationValue && step12)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 13::Take a screen shot of the 3D viewer and paste in to MS paint.
                IWebElement Viewer = z3dvp.ViewerContainer();
                String Imagepath = Config.downloadpath + "\\FirstImage.PNG";
                if (File.Exists(Imagepath))
                    File.Delete(Imagepath);
                DownloadImageFile(Viewer, Imagepath);
                if (File.Exists(Imagepath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 14::Using the traverse tool, move the cross hair of navigation control 1 back to its original position "0, 0, 0".
                nav1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(nav1, nav1.Size.Width / 2, nav1.Size.Height / 2).Build().Perform();
                z3dvp.Performdragdrop(nav1, nav1.Size.Width / 2, nav1.Size.Height / 2, nav1.Size.Width / 2, nav1.Size.Height / 6);
                PageLoadWait.WaitForFrameLoad(20);
                //Verification::Navigation image 3 is updated, and the Z component of its position annotation decreases back to 0.Navigation controls 1 and 2 are unchanged.
                //string Steps14_1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //string Steps14_2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //string Steps14_3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //String opval1 = Steps14_3.Replace("mm", "");
                //opval1 = opval1.Replace(" ", "");
                //opval1 = opval1.Replace(" ", "");
                //opval1 = opval1.Replace(",", "_");
                //String[] val1 = opval1.Split('_');
                //String opp = val1[val1.Length - 1].Split('.')[0];
                //int diff1 = Convert.ToInt32(opp) - 15;
                //   if (Steps14_1.Equals(ActualLocationValue) && Steps14_2.Equals(ActualLocationValue) && Steps14_3.Equals(ActualLocationValue))
                List<string> result14 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if(result14[0].Replace("-","")== ActualLocationValue && result14[1].Replace("-", "") == ActualLocationValue && result14[3].Replace("-", "") == ActualLocationValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 15::Apply an upward scroll to navigation control 3 until the position annotation reads "0, 0, 100".
                //if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "chrome")
                //{
                //    IWebElement Navigationname17 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //    bool Nav17_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                //    IWebElement ViewerContainer1 = z3dvp.ViewerContainer();
                //    this.Cursor = new Cursor(Cursor.Current.Handle);
                //    Cursor.Position = new Point((ViewerContainer1.Location.X + 250), (ViewerContainer1.Location.Y / 2 + 600));
                //    int t = 0;
                //    do
                //    {
                //        BasePage.mouse_event(0x0800, 0, 0, 15, 0);
                //        Thread.Sleep(1000);
                //        t++;
                //        if (t > 250) break;
                //    }
                //    while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2,3) < 100);

                //    IList<string> leftpanel17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                //    string[] mousesplit = leftpanel17[3].Split(new string[] { "<br>", "\r\n", "," }, StringSplitOptions.None);
                //    int indexlastof = mousesplit[2].LastIndexOf(" ");
                //    double   Navigationthreevalue = double.Parse(mousesplit[2].Trim().Substring(0, indexlastof));
                //    if(Navigationthreevalue>=100 && Navigationthreevalue<=101)
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
                //}
                //else
                //{
                    z3dvp.select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(5);
                    z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, "1.0");
                    PageLoadWait.WaitForFrameLoad(5);
                    z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationthree, scrolllevel: 100);
                    PageLoadWait.WaitForFrameLoad(10);
                    String annotationval = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                    String opval = Nav3Location.Replace("mm", "");
                    opval = opval.Replace(" ", "");
                    opval = opval.Replace(" ", "");
                    opval = opval.Replace(",", "_");
                    String[] val = opval.Split('_');
                    String op = val[val.Length - 1].Split('.')[0];
                    int diff = Convert.ToInt32(op) - 100;
                    if (diff <= 10 && diff >= -10)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
              //  }

                //Report Result
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
            }

        }

        public TestCaseResult Test_163290(String testid, String teststeps, int stepcount) // Curved MPR view
        {
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            Login login = new Login();
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String objtestdatareq = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objpatientname = objtestdatareq.Split('|')[0];
                String objpatientid = objtestdatareq.Split('|')[1];
                String objpatientDOB = objtestdatareq.Split('|')[2];
                String Navigation1annval, Navigation2annval, Navigation3annval, MPRPAthNavigationannval, PathNavigation3Dannval, CurvedMPRannval;
                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg,layout:BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 02
                Navigation1annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                if (Navigation1annval.Contains(objpatientname) && Navigation1annval.Contains(objpatientid) && Navigation1annval.Contains(objpatientDOB))
                {
                    Navigation2annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                    if (Navigation2annval.Contains(objpatientname) && Navigation2annval.Contains(objpatientid) && Navigation2annval.Contains(objpatientDOB))
                    {
                        Navigation3annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                        if (Navigation3annval.Contains(objpatientname) && Navigation3annval.Contains(objpatientid) && Navigation3annval.Contains(objpatientDOB))
                        {
                            MPRPAthNavigationannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                            if (MPRPAthNavigationannval.Contains(objpatientname) && MPRPAthNavigationannval.Contains(objpatientid) && MPRPAthNavigationannval.Contains(objpatientDOB))
                            {
                                PathNavigation3Dannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                                if (PathNavigation3Dannval.Contains(objpatientname) && PathNavigation3Dannval.Contains(objpatientid) && PathNavigation3Dannval.Contains(objpatientDOB))
                                {
                                    CurvedMPRannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                                    if (CurvedMPRannval.Contains(objpatientname) && CurvedMPRannval.Contains(objpatientid) && CurvedMPRannval.Contains(objpatientDOB))
                                    {
                                        result.steps[++ExecutedSteps].StepPass();
                                    }
                                    else
                                        result.steps[++ExecutedSteps].StepFail();
                                }
                                else
                                    result.steps[++ExecutedSteps].StepFail();
                            }
                            else
                                result.steps[++ExecutedSteps].StepFail();
                        }
                        else
                            result.steps[++ExecutedSteps].StepFail();
                    }
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                bool Curvednav1Date = brz3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                bool Curvednav2Date = brz3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                bool Curved3D1Date = brz3dvp.VerifyDateFormat(BluRingZ3DViewerPage._3DPathNavigation);
                bool Curvednav3Date = brz3dvp.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                bool CurvedResultDate = brz3dvp.VerifyDateFormat(BluRingZ3DViewerPage.MPRPathNavigation);
                bool CurvedMprDate = brz3dvp.VerifyDateFormat(BluRingZ3DViewerPage.CurvedMPR);
                if (Curvednav1Date && Curvednav2Date && Curved3D1Date && Curvednav3Date && CurvedResultDate && CurvedMprDate)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                Navigation1annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                MPRPAthNavigationannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathNavigation3Dannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                CurvedMPRannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                if (Navigation1annval == "" && Navigation2annval == "" && Navigation3annval == "" && MPRPAthNavigationannval == "" && PathNavigation3Dannval == "" && CurvedMPRannval == "")
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                new Actions(Driver).SendKeys("T").Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                Navigation1annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3annval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                MPRPAthNavigationannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathNavigation3Dannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                CurvedMPRannval = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                if (Navigation1annval != null && Navigation2annval != null && Navigation3annval != null && MPRPAthNavigationannval != null && PathNavigation3Dannval != null && CurvedMPRannval != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06 & 07
                res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations,check: false);
                if (!res)
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                   
                //step 08 & 09
                res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, check: true);
                if (!res)
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 10
                res = bluview.SelectShowHideValue(BluRingZ3DViewerPage.HideText);
                if(!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 11
                res = bluview.SelectShowHideValue(BluRingZ3DViewerPage.ShowText);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }
    }

}