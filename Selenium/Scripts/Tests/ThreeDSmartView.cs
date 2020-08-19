using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Drawing;

namespace Selenium.Scripts.Tests
{
    class ThreeDSmartView : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public object MouseSimulator { get; private set; }
        public BluRingZ3DViewerPage brz3dvp { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public ThreeDSmartView(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_164066(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
               // Verify all smart views are available in the smart view dropdown
                Boolean step1_1 =Z3dViewerPage.verify3dlayoutMenuList();
                if (step1 & step1_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:2
                // bool step2 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                PageLoadWait.WaitForFrameLoad(10);
                bool step2_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel });
                if (step2_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:3
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
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

                //Step:4
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(10);
                bool step4_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 , BluRingZ3DViewerPage.Navigation3D2 , BluRingZ3DViewerPage.ResultPanel});
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

                //Step:5
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(10);
                bool step5_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<String> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                if (step5_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:6
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.checkerrormsg("y");
                PageLoadWait.WaitForFrameLoad(10);
                bool step6_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.CalciumScoring });
                if (step6_1)
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_164067(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] PatientDetails = PatientID.Split('|');
            string ThumbnailDescriptions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] ThumbnailDescription = ThumbnailDescriptions.Split('|');

            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            string subVolume2 = TestData[1];
            string subVolume3 = TestData[2];
            string subVolume4 = TestData[3];
            string subVolume5 = TestData[4];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: PatientDetails[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Study Date", PatientDetails[1]);
                PageLoadWait.WaitForFrameLoad(10);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step1 = Z3dViewerPage.selectthumbnail(ThumbnailDescription[0], 0, ThumbnailDescription[1]);
                Boolean step1_1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                Boolean step1_2 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel });
                if (step1 && step1_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                // Verify all smart views are available in the smart view dropdown
                //Step:2
                bool step2 = verifyAllToolBar_Options_Availability(BluRingZ3DViewerPage.Navigationone);
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

                //Step:3
                //Driver.Manage().Window.Minimize();
                Driver.Manage().Window.Size = new Size(750, 950);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3 = verifyAllToolBar_Options_Availability(BluRingZ3DViewerPage.Navigationone);
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

                //Step:4 & 5
                Driver.Manage().Window.Maximize();
                bool step4_1 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume1, "Sub Volumes");
                bool step4_2 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume2, "Sub Volumes");
                bool step4_3 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume3, "Sub Volumes");
                bool step4_4 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume4, "Sub Volumes");
                bool step4_5 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume5, "Sub Volumes");
                if (step4_1 && step4_2 && step4_3 && step4_4 && step4_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:6
                bool step6 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7 
                bool step7_1 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume1, "Sub Volumes");
                bool step7_2 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume2, "Sub Volumes");
                bool step7_3 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume3, "Sub Volumes");
                bool step7_4 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume4, "Sub Volumes");
                bool step7_5 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume5, "Sub Volumes");
                if (step7_1 && step7_2 && step7_3 && step7_4 && step7_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:8
                bool step8 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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

                //Step:9
                bool step9_1 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume1, "Sub Volumes");
                bool step9_2 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume2, "Sub Volumes");
                bool step9_3 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume3, "Sub Volumes");
                bool step9_4 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume4, "Sub Volumes");
                bool step9_5 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume5, "Sub Volumes");
                if (step9_1 && step9_2 && step9_3 && step9_4 && step9_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10
                bool step10 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                bool step10_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<String> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
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

                //Step:11
                bool step11_1 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume1, "Sub Volumes");
                bool step11_2 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume2, "Sub Volumes");
                bool step11_3 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume3, "Sub Volumes");
                bool step11_4 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume4, "Sub Volumes");
                bool step11_5 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, subVolume5, "Sub Volumes");
                if (step11_1 && step11_2 && step11_3 && step11_4 && step11_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:12
                bool step12 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                bool step12_1 = Z3dViewerPage.checkerrormsg("y");
                if (step12 || step12_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:13
                bool step13_1 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.CalciumScoring, subVolume1, "Sub Volumes");
                bool step13_2 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.CalciumScoring, subVolume2, "Sub Volumes");
                bool step13_3 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.CalciumScoring, subVolume3, "Sub Volumes");
                bool step13_4 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.CalciumScoring, subVolume4, "Sub Volumes");
                bool step13_5 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.CalciumScoring, subVolume5, "Sub Volumes");
              
                if (step13_1 && step13_2 && step13_3 && step13_4 && step13_5)
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }


        public TestCaseResult Test_166554(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingViewer Viewer = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 and 2 :: Load a study that has multiple 3D supported series in universal viewer. 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                //bool step1 = Z3dViewerPage.searchandopenstudyin3D(Patientid, ThumbnailDescription, BluRingZ3DViewerPage.MPR);
                bool step1 = Z3dViewerPage.searchandopenstudyin3D("1TLJY1YL", ThumbnailDescription, BluRingZ3DViewerPage.MPR, field: "acc");
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to open study");
                //Step 3 :: Drag and drop the same series (series 2) over the active 3D viewer.
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool Thumbnail = Z3dViewerPage.DragandDropThumbnail("S2" , "MR" , "75" , navigation1 );
                IWebElement NavigationElement = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                string Navtopleft = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                Thread.Sleep(5000);
                if (Thumbnail && Navtopleft.Contains("Ser: 2"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 :: Drag and drop the series 4 on the active 3D viewer.
                Z3dViewerPage.DragandDropThumbnail("S4", "MR" , "23" , navigation1);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: "Slices are not sufficiently parallel" message shows up.
                IWebElement Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                if(Warning.Text.Contains("Slices are not sufficiently parallel"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: Click on the OK button on the error message dialog window.
                IWebElement okbutton = Warning.FindElement(By.CssSelector("span p"));
                bool OkButton = okbutton.Displayed;
                if (OkButton)
                {
                    ClickElement(okbutton);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: Apply 3D tool operations on the images.Eg: Scroll, Zoom, pan etc.
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                //=========================Scroll Tool=======================================
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                String ScrollBefore= Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String ScrollAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //===================================Zoom Tool============================================
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                String ZoomBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String ZoomAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //===========================================Pan Tool================================================
                Z3dViewerPage.select3DTools(Z3DTools.Pan);
                String PanBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String PanAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if(ScrollBefore!= ScrollAfter && ZoomBefore!= ZoomAfter && PanBefore!= PanAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Drag and drop any PR series on the active 3D viewer.
                Z3dViewerPage.DragandDropThumbnail("S2- 2", "PR", "21" , navigation1);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: Series must be CT, MR, or PT to display in 3D" message shows up.
                Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                if (Warning.Text.Contains("Series must be a non-secondary capture CT, MR or PT to display in 3D"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: Click on the OK button on the warning message window.
                okbutton = Warning.FindElement(By.CssSelector("span p"));
                OkButton = okbutton.Displayed;
                if (OkButton)
                {
                    ClickElement(okbutton);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: Apply 3D tool operations on the images.Eg: Scroll, Zoom, pan etc.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                //=========================Scroll Tool=======================================
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                ScrollBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ScrollAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //===================================Zoom Tool============================================
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                ZoomBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ZoomAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //===========================================Pan Tool================================================
                Z3dViewerPage.select3DTools(Z3DTools.Pan);
                PanBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PanAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (ScrollBefore != ScrollAfter && ZoomBefore != ZoomAfter && PanBefore != PanAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10 :: Drag and drop the series 6 on the active 3D viewer.
                IWebElement navigation11 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.DragandDropThumbnail("S6", "MR", "14" , navigation11);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                //Verification :: "Slices are not sufficiently parallel" message shows up.
                Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                if (Warning.Text.Contains("Series must contain at least 15 instances to display in 3D"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 :: Click on the OK button on the error message window.
                okbutton = Warning.FindElement(By.CssSelector("span p"));
                OkButton = okbutton.Displayed;
                ClickElement(okbutton);
                NavigationElement = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navtopleft = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (OkButton && Navtopleft.Contains("Ser: 2"))
                {
                  
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 :: Apply 3D tool operations on the images.Eg: Scroll, Zoom, pan etc
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                //=========================Scroll Tool=======================================
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                ScrollBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ScrollAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //===================================Zoom Tool============================================
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                ZoomBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ZoomAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //===========================================Pan Tool================================================
                Z3dViewerPage.select3DTools(Z3DTools.Pan);
                PanBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PanAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (ScrollBefore != ScrollAfter && ZoomBefore != ZoomAfter && PanBefore != PanAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: Open the same study in the second panel. Select and load series 3 in 3D viewer.
                Viewer.OpenPriors(accession: "1TLJY1YL");
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement SecondpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondpanelNav1).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "21", SecondpanelNav1, panel:2);
                bool MPR = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 2);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14 :: Drag and drop the series 3 from both the thumbnail bar and Thumbnail flyout on the Active 3D viewer of second panel.
                Z3dViewerPage.DragandDropThumbnail("S3", "MR" , "21" , navigation1 , panel:2 );
                PageLoadWait.WaitForPageLoad(10);
                //Verification ::"This series is already being shown on a 3D viewer" warning message shows up.
                Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                if (Warning.Text.Contains("This series is already being shown on a 3D viewer"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 :: Click on the OK button on the warning message window.
                okbutton = Warning.FindElement(By.CssSelector("span p"));
                OkButton = okbutton.Displayed;
                ClickElement(okbutton);
                NavigationElement = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                Navtopleft = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (OkButton && Navtopleft.Contains("Ser: 3"))
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: Apply 3D tool operations on the images  Eg: Scroll, Zoom, pan etc
                Z3dViewerPage.select3DTools(Z3DTools.Reset , panel:2);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                //=========================Scroll Tool=======================================
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool , panel:2);
                ScrollBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ScrollAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                //===================================Zoom Tool============================================
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom , panel:2);
                ZoomBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ZoomAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                //===========================================Pan Tool================================================
                Z3dViewerPage.select3DTools(Z3DTools.Pan , panel:2);
                PanBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, (navigation1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PanAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                if (ScrollBefore != ScrollAfter && ZoomBefore != ZoomAfter && PanBefore != PanAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17 :: Close the second panel and drop the series 3 on the Active 3D viewer of the first panel.
                IWebElement Close2ndPanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.Close2ndpanel));
                ClickElement(Close2ndPanel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                //Verification :: Series 3 is loaded in the 3D viewer without any errors.
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "21", navigation1 );
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForPageLoad(10);
                NavigationElement = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navtopleft = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (Navtopleft.Contains("Ser: 3"))
                {

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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }


        public TestCaseResult Test_166744(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingViewer Viewer = new BluRingViewer();
            string PatientidDetails = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Patientid = PatientidDetails.Split('|')[0];
            String Accesion1 = PatientidDetails.Split('|')[1];
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                Z3dViewerPage.DeletePriorsInEA("10.9.37.82", "00-001-002", "Thorax^ThoraxRoutine");
                Z3dViewerPage.DeletePriorsInEA("10.9.37.82", "00-001-002", "Becken^Routine");
                //Step:1 :: Load the Study that has multiple 3D supported series Universal viewer.Study: Export, Dee V.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Description", Accesion1);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement StudyExamlist = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyExamlist));
                if (StudyExamlist.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: Open the same study in the second study panel.
                Viewer.OpenPriors(StudyDate: "10-May-2004");
                //Verification::Same study loaded in the second study panel.
                IList<IWebElement> PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if (PanelCount.Count.Equals(2))
                {
                   result.steps[++ExecutedSteps].status = "Pass";
                   Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                   result.steps[++ExecutedSteps].status = "Fail";
                   Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                   result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3 :: From the first study panel, Select and load series 2 in 3D viewer in MPR view.
                IWebElement FirstpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav1).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.DragandDropThumbnail("S2", "CT", "99", FirstpanelNav1);
                bool MPR = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (MPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 ::From the second study panel, Select and load series 3 in 3D viewer in 3D 4:1 view.
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement SecondpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondpanelNav1).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.DragandDropThumbnail("S3", "CT", "146", SecondpanelNav1 , panel:2);
                bool ThreeD4x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4, panel:2);
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
                //Step 5 :: From the thumbnail bar of the first study panel, Drag and drop series 4 in to the active 3D viewer of the second study panel.
                IWebElement Navigation1panel2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.DragandDropThumbnail("S4", "CT", "98", Navigation1panel2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                //Verification:: Series 4 is loaded in in 3D 4:1 view without any errors.
                Navigation1panel2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                string Navtopleft = Navigation1panel2.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (Navtopleft.Contains("Ser: 4"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: From the thumbnaibar of the second study panel, Drag and drop series 5 in to the active 3D viewer of the first study panel.
                IWebElement Navigation1panel1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.DragandDropThumbnail("S5", "CT", "99", Navigation1panel1 , panel:2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification:: Series 4 is loaded in in 3D 4:1 view without any errors.
                Navigation1panel2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navtopleft = Navigation1panel2.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (Navtopleft.Contains("Ser: 5"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: In the first study panel 3D viewer, select the measurement tool from the 3D tool box and draw a line on the image from any one of the MPR control.
                bool lineMeasurement = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                Navigation1panel1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int Nav1BeforeYellowClr = Z3dViewerPage.LevelOfSelectedColor(Navigation1panel1, testid, ExecutedSteps + 1, 255, 255, 0, 2);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(Navigation1panel1, Navigation1panel1.Size.Width / 2, Navigation1panel1.Size.Height / 4).ClickAndHold().
                        MoveToElement(Navigation1panel1, Navigation1panel1.Size.Width / 2, Navigation1panel1.Size.Height - 50).Release().Build().Perform();
                else
                    new TestCompleteAction().PerformDraganddrop(Navigation1panel1, Navigation1panel1.Size.Width / 2, Navigation1panel1.Size.Height / 4, Navigation1panel1.Size.Width / 2, Navigation1panel1.Size.Height - 50);
                PageLoadWait.WaitForFrameLoad(10);
                int Nav1AfterYellowClr = Z3dViewerPage.LevelOfSelectedColor(Navigation1panel1, testid, ExecutedSteps + 2, 255, 255, 0, 2);
                //Verification ::Line measurement cursor shows up and Measurement is created on the respective image.
                if(lineMeasurement && Nav1BeforeYellowClr!= Nav1AfterYellowClr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: Click on the save option from the hover bar of the respective control and observe the thumbnail bar.
                IList<IWebElement> Firstpanelthumbnail_B = Driver.FindElements(By.CssSelector(Locators.CssSelector.FirstpanelThumbnailcount));
                IList<IWebElement> Secondpanelthumbnail_B = Driver.FindElements(By.CssSelector(Locators.CssSelector.SecondpanelThumbnailcount));
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                //Verification :: Spinner shows up on the thumbnail bar on First study panel. PR and saved 3D images are appended to the thumbnail bar of both the panels.
                IList<IWebElement> BusyCursor = Driver.FindElements(By.CssSelector(Locators.CssSelector.BusyCursor));
                PageLoadWait.WaitForThumbnailsToLoad(80);
                Thread.Sleep(10000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                IList<IWebElement> Firstpanelthumbnail_A = Driver.FindElements(By.CssSelector(Locators.CssSelector.FirstpanelThumbnailcount));
                IList<IWebElement> Secondpanelthumbnail_A = Driver.FindElements(By.CssSelector(Locators.CssSelector.SecondpanelThumbnailcount));
                if (BusyCursor[0].Enabled &&BusyCursor.Count == 1 && Firstpanelthumbnail_B.Count < Firstpanelthumbnail_A.Count && Secondpanelthumbnail_B.Count< Secondpanelthumbnail_A.Count)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: In the second study panel 3D viewer, Click on the save option from the hover bar of the 3D1 control and observe the thumbnail bar.
                Firstpanelthumbnail_B = Driver.FindElements(By.CssSelector(Locators.CssSelector.FirstpanelThumbnailcount));
                Secondpanelthumbnail_B = Driver.FindElements(By.CssSelector(Locators.CssSelector.SecondpanelThumbnailcount));
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D1, "Save image and annotations to the exam" , panel:2);
                //Verification ::Spinner shows up on the thumbnail bar on both study panel PR and saved 3D images are appended to the thumbnail bar of both the panels.
                BusyCursor = Driver.FindElements(By.CssSelector(Locators.CssSelector.BusyCursor));
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                Firstpanelthumbnail_A = Driver.FindElements(By.CssSelector(Locators.CssSelector.FirstpanelThumbnailcount));
                Secondpanelthumbnail_A = Driver.FindElements(By.CssSelector(Locators.CssSelector.SecondpanelThumbnailcount));
                if (BusyCursor[0].Enabled && BusyCursor.Count == 1 && Firstpanelthumbnail_B.Count < Firstpanelthumbnail_A.Count && Secondpanelthumbnail_B.Count< Secondpanelthumbnail_A.Count)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10 :: Close the second study panel and In the the first study panel switch to 2D view. 
                IWebElement Close2ndPanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.Close2ndpanel));
                ClickElement(Close2ndPanel);
                PageLoadWait.WaitForPageLoad(10);
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                IList<IWebElement> viewmode = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdownText));
                string Viewmodetext = viewmode[0].GetAttribute("innerText");
                if(Viewmodetext.Equals(BluRingZ3DViewerPage.Two_2D) && PanelCount.Count == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 :: Select and load the PR and saved 3D images in the active viewport.
                Viewer.SelectViewerTool(BluRingTools.Window_Level);
                IWebElement TwoDFirstviewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav1).Click().Build().Perform();
                bool Thumbnail = Z3dViewerPage.selectthumbnail("PR" , 0);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], TwoDFirstviewport))
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
                //Step 12 :: Open the related study in the second panel.Study: Export, Dee V.Study date : 10 - August - 2004
                Viewer.OpenPriors(StudyDate: "10-Aug-2004");
                //Verification::Same study loaded in the second study panel.
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if (PanelCount.Count.Equals(2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: From the second study panel, Select and load series 4 in 3D viewer in 3D 6:1 view.
                SecondpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                bool Series4 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "15", SecondpanelNav1, panel: 2);
                bool ThreeD6x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, panel: 2);
                if (Series4 && ThreeD6x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14 :: From the first study panel, Select and load series 2 in 3D viewer in Curved MPR view.
                FirstpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                bool Series2 = Z3dViewerPage.DragandDropThumbnail("S2", "CT", "99", FirstpanelNav1);
                bool CurvedMPR = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                if (Series2 && CurvedMPR)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 :: From the thumbnail bar of the first study panel, Drag and drop series 3 in to the active 3D viewer of the second study panel.
                Navigation1panel2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone, panel:2);
                bool Series3 = Z3dViewerPage.DragandDropThumbnail("S3", "CT", "146", Navigation1panel2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification:: Series 3 of the first panel thumbnail bar is loaded over the active 3D viewer (Six up view) of the second panel.
                Navigation1panel2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone , panel:2);
                Navtopleft = Navigation1panel2.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                bool ThreeD2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2, panel: 2).Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                if (Series3 && Navtopleft.Contains("Ser: 3") && ThreeD2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: From the thumbnail bar of the second study panel, Drag and drop series 2 in to the active 3D viewer of the first panel.
                Navigation1panel1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool Panel2Ser2 = Z3dViewerPage.DragandDropThumbnail("S2", "MR", "25", Navigation1panel1 , panel:2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: Series 2 of the second panel thumbnail bar is loaded over the active 3D viewer (Curved MPR view) of the first panel.
                Navigation1panel1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navtopleft = Navigation1panel1.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                bool Curved = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR).Text.Contains(BluRingZ3DViewerPage.CurvedMPR);
                if (Series3 && Navtopleft.Contains("Ser: 2") && Curved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17 :: In the first study panel 3D viewer, Create a path by adding the points over the images in MPR navigation controls.
                IWebElement nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement MprPathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int BeforeColour1 = Z3dViewerPage.LevelOfSelectedColor(nav1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                int BeforeColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 3, 0, 0, 0, 2);
                int BeforeColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 4, 133, 133, 131, 2);
                int BeforeColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 5, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(nav1, nav1.Size.Width / 2, nav1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(nav1 , nav1.Size.Width / 2, nav1.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int AfterColour1 = Z3dViewerPage.LevelOfSelectedColor(nav1, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                int AfterColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 7, 0, 0, 0, 2);
                int AfterColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 8, 51, 51, 50, 2);
                int AfterColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                if(BeforeColour1!= AfterColour1 && BeforeColour2!= AfterColour2 && BeforeColour3!= AfterColour3 && BeforeColour4!= AfterColour4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 18 :: Click on the save option from the hover bar of MPR/3D path navigation control and curved MPR controls.
                //=========================================Issue ICA-18240, ICA-18241=============================================
                IList<IWebElement> B_Firstpanelthumbnail = Driver.FindElements(By.CssSelector(Locators.CssSelector.SecondpanelThumbnailcount));
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.MPRPathNavigation, "Save image and annotations to the exam");
                IList<IWebElement> BusyCursor1 = Driver.FindElements(By.CssSelector(Locators.CssSelector.BusyCursor));
                Thread.Sleep(10000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage._3DPathNavigation, "Save image and annotations to the exam");
                IList<IWebElement> BusyCursor2 = Driver.FindElements(By.CssSelector(Locators.CssSelector.BusyCursor));
                Thread.Sleep(10000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.CurvedMPR, "Save image and annotations to the exam");
                IList<IWebElement> BusyCursor3 = Driver.FindElements(By.CssSelector(Locators.CssSelector.BusyCursor));
                Thread.Sleep(10000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                IList<IWebElement> A_Firstpanelthumbnail = Driver.FindElements(By.CssSelector(Locators.CssSelector.SecondpanelThumbnailcount));
                if (B_Firstpanelthumbnail.Count< A_Firstpanelthumbnail.Count && BusyCursor1[0].Enabled && BusyCursor2[0].Enabled && BusyCursor3[0].Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 19 :: In the second study panel 3D viewer, select the measurement tool from the 3D tool box and draw a line on the image from any one of the MPR control.
                Navigation1panel2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                lineMeasurement = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement , panel:2);
                Nav1BeforeYellowClr = Z3dViewerPage.LevelOfSelectedColor(Navigation1panel2, testid, ExecutedSteps + 1, 255, 255, 0, 2);
                new Actions(Driver).MoveToElement(Navigation1panel2, Navigation1panel2.Size.Width / 2, Navigation1panel2.Size.Height / 4).ClickAndHold().
                    MoveToElement(Navigation1panel2, Navigation1panel2.Size.Width / 2, Navigation1panel2.Size.Height - 70).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Nav1AfterYellowClr = Z3dViewerPage.LevelOfSelectedColor(Navigation1panel2, testid, ExecutedSteps + 2, 255, 255, 0, 2);
                //Verification ::Line measurement cursor shows up and Measurement is created on the respective image.
                if (lineMeasurement && Nav1BeforeYellowClr != Nav1AfterYellowClr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 20 :: Perform rotation on the image in 3D2 control using the rotate hotspots (x, y, z)
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, panel: 2);
                IWebElement Nav3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                string OrientationBefore3D1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                string OrientationBefore3D2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2 , panel:2);
                Z3dViewerPage.Performdragdrop(Nav3D1, (Nav3D1.Size.Width - 218), Nav3D1.Size.Height / 2, (Nav3D1.Size.Width - 10), Nav3D1.Size.Height / 2);
                string OrientationAfter3D1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                string OrientationAfter3D2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2 , panel:2);
                if (OrientationBefore3D1!= OrientationAfter3D1 && OrientationBefore3D2!= OrientationAfter3D2 && OrientationAfter3D1 == OrientationAfter3D2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 21 :: Click on the save option from the hover bar of the MPR control with measurement and 3D2 control.
                IList<IWebElement> Step21_B = Driver.FindElements(By.CssSelector(Locators.CssSelector.FirstpanelThumbnailcount));
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.MPRPathNavigation, "Save image and annotations to the exam");
                BusyCursor = Driver.FindElements(By.CssSelector(Locators.CssSelector.BusyCursor));
                Thread.Sleep(15000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                IList<IWebElement> Step21_A = Driver.FindElements(By.CssSelector(Locators.CssSelector.FirstpanelThumbnailcount));
                if(BusyCursor[0].Enabled && Step21_B.Count< Step21_A.Count)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 22 ::From both the panels, switch to 2D views. Select and load the PR's and saved 3D images in the active viewport.
                bool panel1Layout = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                bool panel2Layout = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D , panel:2);
                viewmode = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdownText));
                string Viewmodetext1 = viewmode[0].GetAttribute("innerText");
                string Viewmodetext2 = viewmode[1].GetAttribute("innerText");
                TwoDFirstviewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav1).Click().Build().Perform();
                Thumbnail = Z3dViewerPage.selectthumbnail("PR", 0);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if(Viewmodetext1.Equals(BluRingZ3DViewerPage.Two_2D) && Viewmodetext2.Equals(BluRingZ3DViewerPage.Two_2D) && CompareImage(result.steps[ExecutedSteps], TwoDFirstviewport))
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
                //Step 23 ::  Close and relaunch the study. PR's and saved 3D images by loading in to the active viewport.
                Z3dViewerPage.CloseViewer();
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Description", Accesion1);
                PageLoadWait.WaitForFrameLoad(5);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                StudyExamlist = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyExamlist));
                TwoDFirstviewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], TwoDFirstviewport))
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

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
                Z3dViewerPage.DeletePriorsInEA("10.9.37.82", "00-001-002", "Thorax^ThoraxRoutine");
                Z3dViewerPage.DeletePriorsInEA("10.9.37.82", "00-001-002", "Becken^Routine");
            }
        }

        /// <summary>
        /// To verify All toolBar options are displayed
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>

        public Boolean verifyAllToolBar_Options_Availability(String ControlName)
        {
            bool result = true;
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            try
            {
                Z3dViewerPage.SelectOptionsfromViewPort(ControlName: ControlName);

                //Verify undo segmentation option
                IList<IWebElement> UndoRedoSavepanel = Driver.FindElements(By.CssSelector(Locators.CssSelector.UndoRedoSavepanel));
                if (!UndoRedoSavepanel[0].GetAttribute("title").Equals(BluRingZ3DViewerPage.UndoSegmentation) && !UndoRedoSavepanel[0].Displayed)
                {
                    Logger.Instance.ErrorLog("Undo segmentation button not displayed");
                    return false;
                }
                if (!UndoRedoSavepanel[1].GetAttribute("title").Equals(BluRingZ3DViewerPage.SaveIamge) && !UndoRedoSavepanel[1].Displayed)
                {
                    Logger.Instance.ErrorLog("Save Image button not displayed");
                    return false;
                }
                if (!UndoRedoSavepanel[2].GetAttribute("title").Equals(BluRingZ3DViewerPage.RedoSegmentation) && !UndoRedoSavepanel[2].Displayed)
                {
                    Logger.Instance.ErrorLog("Redo Segmrentation button not displayed");
                    return false;
                }

                //other buttons in the drobdown List
                IList<IWebElement> bttonObj = Driver.FindElements(By.CssSelector("td[class='menuItemLabel fontTitle_m']"));
                foreach (IWebElement option in bttonObj)
                {
                    if (option.Text.Equals(BluRingZ3DViewerPage.SubVolumes) && option.Displayed)
                    {
                        Logger.Instance.InfoLog("save Image button displayed");
                        result = true;
                    }
                    else if (option.Text.Equals(BluRingZ3DViewerPage.Preset) && option.Displayed)
                    {
                        Logger.Instance.InfoLog("Preset button displayed");
                        result = true;
                    }
                    else if (option.Text.Equals(BluRingZ3DViewerPage.RenderType) && option.Displayed)
                    {
                        Logger.Instance.InfoLog("Render Type button displayed");
                        result = true;
                    }
                    else if (option.Text.Equals(BluRingZ3DViewerPage.Thickness) && option.Displayed)
                    {
                        Logger.Instance.InfoLog("Thickness button displayed");
                        result = true;
                    }
                    else
                    {
                        result = false;
                        Logger.Instance.ErrorLog("Sub Volume or Preset Or Render Type or Thickness button not Displayed. Actual option Text :" + option.Text);
                        break;
                    }
                }

               
            return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error while running verifyAllToolBar_Options_Availability method. Error :" + ex);
                return false;
            }
            finally
            {
                IWebElement ViewPort = Z3dViewerPage.controlelement(ControlName);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));

            }

        }
        public TestCaseResult Test_164068(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] PatientDetails = PatientID.Split('|');
            string ThumbnailDescriptions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] ThumbnailDescription = ThumbnailDescriptions.Split('|');

            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            string subVolume2 = TestData[1];
            string subVolume3 = TestData[2];
            string subVolume4 = TestData[3];
            string subVolume5 = TestData[4];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer Viewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: PatientDetails[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Study Date", PatientDetails[1]);
                PageLoadWait.WaitForFrameLoad(10);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step1 = Z3dViewerPage.selectthumbnail(ThumbnailDescription[0], 0, ThumbnailDescription[1]);
                Boolean step1_1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (step1 && step1_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:2
                bool step2 = Z3dViewerPage.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, subVolume1);
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

                //Step:3
                IWebElement ViewPort = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step3 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", ViewPort);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (step3 && step3_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Step Status step3_1:"+ step3_1.ToString());
                    Logger.Instance.InfoLog("Step Status step3:" + step3.ToString());
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:4
                Viewer.OpenPriors(StudyDate: "21-Jul-2013");
                Thread.Sleep(7000);
                IList<IWebElement> PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                IWebElement ViewPort1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                bool step4 = Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", ViewPort1 , panel:2);
                Thread.Sleep(7000);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, panel:2);
                PageLoadWait.WaitForPageLoad(5);
                bool step4_1 = Z3dViewerPage.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, subVolume1, panelOption: 2 , verifyloading:false);
                if (PanelCount.Count.Equals(2) && step4 && step4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:5
                bool step5 = Z3dViewerPage.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, subVolume2, panelOption: 2);
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

                //Step:6
                IWebElement ViewPort2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone, panel:2);
                bool step6 = Z3dViewerPage.DragandDropThumbnail("S5", "MR", "22", ViewPort2);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(10);
                bool step6_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S5", "MR", "22");
                if (step6 && step6_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7
                IWebElement closeButtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.Close2ndpanel));
                ClickElement(closeButtn);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(10);
                IList<IWebElement> PanelCount1 = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if (PanelCount1.Count.Equals(1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:8
                IWebElement ViewPort3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step8 = Z3dViewerPage.DragandDropThumbnail("S5", "MR", "22", ViewPort);
                Thread.Sleep(10000);
                bool step8_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S5", "MR", "22");
                if (step8 && step8_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                     result.steps[++ExecutedSteps].status = "Fail";
                     Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                     result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9
                Viewer.OpenPriors(StudyDate: "08-Jun-2013");
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> PanelCount2 = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if (PanelCount2.Count.Equals(2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10
                IWebElement ViewPort4 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                bool step10 = Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", ViewPort4, panel:2);
                PageLoadWait.WaitForPageLoad(10);
                Thread.Sleep(6000);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, panel:2);
                PageLoadWait.WaitForPageLoad(10);
                Thread.Sleep(7000);
                bool step10_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150",panel:2);
                bool step10_2 = Z3dViewerPage.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, subVolume3, panelOption: 2 , verifyloading:false);
                if (step10 && step10_1& step10_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:11
                bool step11 = Z3dViewerPage.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, subVolume5, panelOption: 2);
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

                //Step:12
                IWebElement ViewPort5 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step12 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "24", ViewPort5 , panel:2);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(6000);
                bool step12_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "24" , panel:2);
                if (step12 && step12_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:13
               // IWebElement ViewPort5 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step13 = Z3dViewerPage.DragandDropThumbnail("S5", "MR", "28", ViewPort5, panel:2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                bool step13_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S5", "MR", "28", panel:2);
                if (step13 && step13_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:14
                IWebElement ViewPort7 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 2);
                Thread.Sleep(6000);
                bool step14 = Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", ViewPort7);
                PageLoadWait.WaitForProgressBarToDisAppear();
                bool step14_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                bool step14_2 = Z3dViewerPage.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationone, subVolume2, panelOption: 2 , verifyloading:false);
                if (step14 && step14_1 & step14_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:15
                //IWebElement ViewPort7 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, panel: 2);
                bool step15 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", ViewPort7);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(6000);
                bool step15_1 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (step15 && step15_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:16
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                Thread.Sleep(3000);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D, panel: 2);
                IWebElement TwoDFirstviewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                string firstPanelDetails = Z3dViewerPage.ReadPatientDetailsUsingTesseract(TwoDFirstviewport, 4, 0, 0, 500, 500);
                IWebElement SecondPanelViewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                string secondPanelDetails = Z3dViewerPage.ReadPatientDetailsUsingTesseract(SecondPanelViewport, 4, 0, 0, 500, 500);
               if (firstPanelDetails.Contains("PRIMARY") && secondPanelDetails.Contains("COMPARISON"))
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_166561(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingViewer Viewer = new BluRingViewer();
            string PatientidDetails = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Patientid = PatientidDetails.Split('|')[0];
            String Accesion1 = PatientidDetails.Split('|')[1];
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
             BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
               
                //Step:1 :: Load the Study that has multiple 3D supported series Universal viewer.Study: Export, Dee V.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Description", Accesion1);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement StudyExamlist = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyExamlist));
                if (StudyExamlist.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: Open the related study in the second panel.Study: Export, Dee V.Study date : 10 - August - 2004
                Viewer.OpenPriors(StudyDate: "10-Aug-2004");
                //Verification::Related study is loaded in the second panel without any errors.
                IList<IWebElement> PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                if (PanelCount.Count.Equals(2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3 :: From the first study panel, Load series 2 in active viewport and select the MPR option from the smart view drop down.
                IWebElement FirstpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav1).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.selectthumbnail("S2 99 CT");
                Z3dViewerPage.DragandDropThumbnail("S2", "CT", "99", FirstpanelNav1);
                bool MPR = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (MPR)
                {
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
                //Step 4 :: Apply the 3D tool operations in the MPR controls.
                //1.Scroll  2.Zoom  3.Roam  5.Measurement  6.Rotate  7.Selection / cut tool Note : Cut and rotate are not applicable for MPR result control.
                // 1.Scrolling Tool
                IWebElement Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                String Nav1ScrollBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Nav1ScrollAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                // 2.Zoom Tool
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                String Nav1ZoomBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                // Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Nav1ZoomAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                // 3 .Roam Tool(Pan)
                Z3dViewerPage.select3DTools(Z3DTools.Pan);
                String Nav1PanBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Nav1PanAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //4.Rotate Tool
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                String Nav1RotateBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String Nav1RotateAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //Measurement Tool
                Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                int Nav1BeforeYellowClr = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 1, 255, 255, 0, 2);
                // Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int Nav1AfterYellowClr = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 2, 255, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //Selection Tool
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                IList<IWebElement> ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                PageLoadWait.WaitForFrameLoad(5);
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                int TissueSelectionBefore = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 13, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 4 , Panel1Nav1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForFrameLoad(10);
                int TissueSelectionAfter = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 17, 0, 0, 255, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level);
                if (Nav1ScrollBefore!= Nav1ScrollAfter && Nav1ZoomBefore!=Nav1ZoomAfter && Nav1PanBefore!=Nav1PanAfter && Nav1RotateBefore!=Nav1RotateAfter &&
                    Nav1BeforeYellowClr!=Nav1AfterYellowClr && TissueSelectionBefore!= TissueSelectionAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: Select any of the preset in MPR navigation controls (Eg : Abdomen).
                String Panel1WLNav1_B = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                String Panel1WLNav2_B = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                String Panel1WLNav3_B = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Abdomen, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                String Panel1WLNav1_A = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                String Panel1WLNav2_A = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                String Panel1WLNav3_A = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                if (Panel1WLNav1_B!= Panel1WLNav1_A && Panel1WLNav2_B!= Panel1WLNav2_A && Panel1WLNav3_B!= Panel1WLNav3_A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: Select any of the render type as 3D slab in MPR result control.
                IWebElement Panel1Result = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Panel1Result))
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
                //Step 7 :: Set the thickness values in MPR navigation control 2 to 10 mm.
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationone, "10.0");
                //Verification::Thickness value in all the MPR navigation controls are updated as 10 mm.
                Thread.Sleep(1500);
                Boolean step9_1 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationone).Equals("10" + " mm");
                Boolean step9_2 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                Boolean step9_3 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree).Equals("10" + " mm");
                if (step9_1 && step9_2 && step9_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: Rotate the crosshair hotspots in each MPR navigation controls by 90 degree.
                new Actions(Driver).SendKeys("X").Build().Perform();
                //Navigation 1
                PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                IList<IWebElement> AllControl = Driver.FindElements(By.CssSelector(Locators.CssSelector.wholepanel));
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColourBefore = Z3dViewerPage.LevelOfSelectedColor(AllControl[0], testid, ExecutedSteps + 41, 0, 0, 0, 2);
                Accord.Point RedPoint = Z3dViewerPage.GetIntersectionPoints(Panel1Nav1, testid, ExecutedSteps + 5, "red", "Horizontal", 0);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2 , (Int32)RedPoint.X, (Int32)RedPoint.Y);
                int ColourBefore1 = Z3dViewerPage.LevelOfSelectedColor(AllControl[0], testid, ExecutedSteps + 6, 0, 0, 0, 2);

                //Navigation 2
                IWebElement Panel1Nav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Accord.Point Yellowpoint = Z3dViewerPage.GetIntersectionPoints(Panel1Nav2, testid, ExecutedSteps + 11, "yellow", "Vertical", 0);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                Z3dViewerPage.Performdragdrop(Panel1Nav2, Panel1Nav2.Size.Width - 10, Panel1Nav2.Size.Height / 2 , (Int32)Yellowpoint.X, (Int32)Yellowpoint.Y);
                int ColourBefore2 = Z3dViewerPage.LevelOfSelectedColor(AllControl[0], testid, ExecutedSteps + 4, 0, 0, 0, 2);
                
                //Navigation 3 :: 
                IWebElement Panel1Nav3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Yellowpoint1 = Z3dViewerPage.GetIntersectionPoints(Panel1Nav3, testid, ExecutedSteps + 1, "yellow", "Vertical", 1);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                Z3dViewerPage.Performdragdrop(Panel1Nav3, Panel1Nav3.Size.Width / 2, (Panel1Nav3.Size.Height) * 3 / 4 , (Int32)Yellowpoint1.X, (Int32)Yellowpoint1.Y);
                int ColourBefore3 = Z3dViewerPage.LevelOfSelectedColor(AllControl[0], testid, ExecutedSteps + 5, 0, 0, 0, 2);
                if (ColourBefore!=ColourBefore1 && ColourBefore1!= ColourBefore2 && ColourBefore2!= ColourBefore3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: From the second study panel, Load series 2 in active viewport and select the 3D4:1 option from the smart view down.
                IWebElement SecondpanelNav1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                new Actions(Driver).MoveToElement(SecondpanelNav1).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //Z3dViewerPage.selectthumbnail("S2 25 MR");
                Z3dViewerPage.DragandDropThumbnail("S2", "MR", "25", SecondpanelNav1 , panel:2);
                bool ThreeD4x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4 , panel:2);
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
                //Step 10 :: Adjust the cropping lines of the 3D navigation controls and Rotate the 3D hotspots (x,y,z) in the 3D1 control.
                IWebElement Panel2Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                IWebElement Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1 , panel:2);
                int Nav3D1Before = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                Accord.Point Redwpoint = Z3dViewerPage.GetIntersectionPoints(Panel2Nav1, testid, ExecutedSteps + 3, color: "red", blobval: 8);
                int Xcoordinate = (Int32)Redwpoint.X;
                int Ycoordinate = (Int32)Redwpoint.Y;
                new Actions(Driver).MoveToElement(Panel2Nav1, Xcoordinate, Ycoordinate).ClickAndHold().MoveToElement(Panel2Nav1, Panel2Nav1.Size.Width / 2, Panel2Nav1.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int Nav3D1After = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 2, 133, 133, 131, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, panel: 2);
                Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                string OrientationBeforeP2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                Z3dViewerPage.Performdragdrop(Navigation3D1, (Navigation3D1.Size.Width - 10), Navigation3D1.Size.Height / 2, (Navigation3D1.Size.Width - 218), Navigation3D1.Size.Height / 2);
                string OrientationAfterP2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                Logger.Instance.InfoLog("OrientationBeforeP2 "+ OrientationBeforeP2+ " OrientationAfterP2 " + OrientationAfterP2 + " Nav3D1Before " + Nav3D1Before+ " Nav3D1After "+ Nav3D1After);
                if (OrientationBeforeP2 != OrientationAfterP2 && Nav3D1Before != Nav3D1After)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 :: From first study panel, Drag and drop the series 3 on the Active 3D viewer of first study panel and select 3D 6:1 option from the smart view drop down.
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.DragandDropThumbnail("S3", "CT", "146", Panel1Nav1);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                bool ThreeD6x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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
                //Step 12 :: Select the Toggle MPR/3D option from the 3D1 control hover bar drop down and verify the cropping lines.
                bool Toggle3DMPR = Z3dViewerPage.ChangeViewMode();
                AllControl = Driver.FindElements(By.CssSelector(Locators.CssSelector.wholepanel));
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (Toggle3DMPR && CompareImage(result.steps[ExecutedSteps], AllControl[0]))
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
                //Step 13 :: Select the render type as average in MPR result control.
                bool Average = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                bool Step13_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                if (Average && Step13_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14 :: Select any one of of the 3D Transfer functions in 3D1 and 3D2 controls from the preset drop down under the hover bar.
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset2, "Preset");
                bool Step14_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset2 , "Preset");
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset3, "Preset");
                bool Step14_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset3 , "Preset");
                if (Step14_1 && Step14_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 :: Now drag and drop the series 2 from the first study panel thumbnail bar in to the active 3D viewer in the second study panel.
                Panel2Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone, panel: 2);
                OrientationAfterP2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                Z3dViewerPage.DragandDropThumbnail("S2", "CT", "99", Panel2Nav1);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                string OrientationAfterLoading = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1, panel: 2);
                if(OrientationAfterP2 != OrientationAfterLoading)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: From the second study panel, Select MPR view from the smart view drop down and verify the preservation of tool operations and Image manipulations.
                bool MPRSecondpanel = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, panel:2);
                bool Step16_1 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationone ,panel:2).Equals("10" + " mm");
                bool Step16_2 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo , panel:2).Equals("10" + " mm");
                bool Step16_3 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree , panel:2).Equals("10" + " mm");
                bool Step16_4 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Abdomen  , type:"Preset", panel:2);
                bool Step16_5 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D , panel:2);
                if (MPRSecondpanel && Step16_1 && Step16_2 && Step16_3 && Step16_4 && Step16_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17 :: Select the reset option from the 3D tool box.
                String Step17_1 = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone , panel:2);
                bool Reset = Z3dViewerPage.select3DTools(Z3DTools.Reset, panel: 2);
                String Step17_2 = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone, panel: 2);
                if (Reset && Step17_1!= Step17_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 18 :: Select 3D 6:1 view from the smart view drop down.
                ThreeD6x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, panel: 2);
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
                //Step 19 :: Now drop the series 2 from the second study panel thumbnail bar in to the Active 3D viewer of the first study panel.
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                string Step19_1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                string Step19_2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                Z3dViewerPage.DragandDropThumbnail("S2", "MR", "25", Panel1Nav1 , panel:2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                string Step19_3 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                string Step19_4 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (Step19_1 != Step19_3 && Step19_2 != Step19_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 20 :: Select the Toggle MPR/3D option from the 3D1 control hover bar drop down and verify the cropping lines.
                Toggle3DMPR = Z3dViewerPage.ChangeViewMode(panel:2);
                AllControl = Driver.FindElements(By.CssSelector(Locators.CssSelector.wholepanel));
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (Toggle3DMPR && CompareImage(result.steps[ExecutedSteps], AllControl[1], removeCurserFromPage:true))
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
                //Step 21 :: From the first study panel, Select the 3D4:1 option from the smart view down and verify the image manipulations.
                ThreeD4x1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                AllControl = Driver.FindElements(By.CssSelector(Locators.CssSelector.wholepanel));
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (ThreeD4x1 && CompareImage(result.steps[ExecutedSteps], AllControl[0]))
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
                //Step 22 :: Select the reset option from the 3D tool box.
                IWebElement Navigation3D1P1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int Step22_1 = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1P1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                Reset = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                int Step22_2 = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1P1, testid, ExecutedSteps + 2, 133, 133, 131, 2);
                if (Reset && Step22_1!= Step22_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 23 :: Now drop the series 3 from the first study panel thumbnail bar in to the active 3D viewer of the second study panel and verify the preservation of image manipulations.
                Z3dViewerPage.DragandDropThumbnail("S3", "CT", "146", Panel2Nav1);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: 
                //AllControl = Driver.FindElements(By.CssSelector(Locators.CssSelector.wholepanel));
                bool Step23_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average , panel:2);
                bool Step23_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset2 , type:"Preset", panel:2);
                bool Step23_3 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset3 , type: "Preset" , panel:2);
                bool step23_4 = Z3dViewerPage.verifyControlElementsAvailability(new List<String> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2 } , panel:2);
                if (Step23_1 && Step23_2 && Step23_3 && step23_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 24 :: Select the reset option from the three D tool Box

                Reset = Z3dViewerPage.select3DTools(Z3DTools.Reset , panel:2);
                if (Reset)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 25 :: From the first study panel  active 3D viewer select the Curved Mpr View.
                bool CurvedNpr = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                if (CurvedNpr)
                {
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
                //Step 26 :: Create a path(Manual/Auto Colon/Auto Vissel) on the MPR navigation Control..
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement MprPathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int BeforeColour1 = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int BeforeColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int AfterColour1 = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 5, 0, 0, 255, 2);
                int AfterColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (BeforeColour1 != AfterColour1 && BeforeColour2 != AfterColour2 && BeforeColour3 != AfterColour3 && BeforeColour4 != AfterColour4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("BeforeColour2 Value is : "+ BeforeColour2);
                    Logger.Instance.InfoLog("BeforeColour3 Value is : " + BeforeColour3);
                    Logger.Instance.InfoLog("AfterColour3 Value is : " + AfterColour3);
                    Logger.Instance.InfoLog("AfterColour4 Value is : " + AfterColour4);
                }
                //Step 27 :: Scroll through the images in MPR and 3D path navigation controls.
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int Curvedheight_B = Z3dViewerPage.CurvedMPRHeight(testid, ExecutedSteps + 1)[2].Y;
                IWebElement ThreeDPathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedNav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation,  scrolllevel: 15 , ScrollDirection: "down");
                //var Actions = new TestCompleteAction();
                //Actions.MouseScroll(ThreeDPathnav, "down", "15").Perform();
                int Curvedheight_A = Z3dViewerPage.CurvedMPRHeight(testid, ExecutedSteps + 2)[2].Y;
                MprPathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                int ForStep33 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 77, 0, 0, 0, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], CurvedNav))
                {
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], Panel1Nav1))
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
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28 :: From the second study panel active 3D viewer, select the calcium view.
                bool CalciumLayout = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring , panel:2);
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                if (CalciumLayout)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 29 :: Circle the regions (LM/RCA/LAD/CX/PDA) using Select this slice/select all contiguous
                IWebElement Calpan2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring , panel:2);
                IList<Double> ScoreValuesBefore = Z3dViewerPage.CalciumScoringTableValues("RCA");
                ClickElement(ToolBox[0]);
                int GreenColorBeforeP2 = Z3dViewerPage.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan2, (Calpan2.Size.Width - 10), (Calpan2.Size.Height / 4)).
                                    MoveToElement(Calpan2, (Calpan2.Size.Width - 10), Calpan2.Size.Height / 2).
                                    MoveToElement(Calpan2, (Calpan2.Size.Width / 2), Calpan2.Size.Height / 2).
                                    MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP2 = Z3dViewerPage.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring, panel:2);
                IList<Double> ScoreValuesAfter = Z3dViewerPage.CalciumScoringTableValues("RCA");
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                if (GreenColorBeforeP2!= GreenColorAfterP2 && ScoreValuesBefore[0]< ScoreValuesAfter[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 30 :: In the first study panel drop the series 4 over the active 3D Viewer..
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.DragandDropThumbnail("S4", "CT", "98", Panel1Nav1);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification
                bool step30_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<String> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                string Navtopleft = Panel1Nav1.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (Navtopleft.Contains("Ser: 4") && step30_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 31 :: In the second study panel, Drop the series 4 over the active 3D viewer.
                Calpan2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring, panel: 2);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "15", Calpan2, panel:2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: 
                Z3dViewerPage.checkerrormsg("y");
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                Calpan2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring, panel:2);
                Navtopleft = Calpan2.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (Navtopleft.Contains("Ser: 4"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 32 :: Now drop the series 3 from the first study panel thumbnail bar in to the active 3D viewer of the second panel and verify the preservation of image manipulations.
                Calpan2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring, panel: 2);
                Z3dViewerPage.DragandDropThumbnail("S3", "CT", "146", Calpan2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::
                IList<Double> Step32_B = Z3dViewerPage.CalciumScoringTableValues("RCA");
                ToolBox = Driver.FindElements(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                ClickElement(ToolBox[0]);
                int Step32_2 = Z3dViewerPage.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                if (GreenColorAfterP2 == Step32_2 && Step32_B[0] !=0 && Step32_B[1] !=0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 33 ::Now drop the series 2 from the second study panel thumbnail bar in to the active 3D viewer of the first panel and verify the preservation of image manipulations.
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.DragandDropThumbnail("S2", "MR", "25", Panel1Nav1, panel:2);
                PageLoadWait.WaitForProgressBarToDisAppear();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::
                Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navtopleft = Panel1Nav1.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                int AfterColour11 = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav1, testid, ExecutedSteps + 45, 0, 0, 255, 2);
                int Curvedheight_33 = Z3dViewerPage.CurvedMPRHeight(testid, ExecutedSteps + 2)[2].Y;
                MprPathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                int ForStep33_A = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 77, 0, 0, 0, 2);
                if (Navtopleft.Contains("Ser: 2") && Curvedheight_A == Curvedheight_33 && ForStep33 == ForStep33_A)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 34 :: Switch to 2D view in both the study panels.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D , panel:2);
                IWebElement TwoDFirstviewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                string firstPanelDetails = Z3dViewerPage.ReadPatientDetailsUsingTesseract(TwoDFirstviewport, 4, 0, 0, 500, 500);
                IWebElement SecondPanelViewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.SecondPanelFirstViewport));
                string secondPanelDetails = Z3dViewerPage.ReadPatientDetailsUsingTesseract(SecondPanelViewport, 4, 0, 0, 500, 500);
                if (firstPanelDetails.Contains("PRIMARY") && secondPanelDetails.Contains("COMPARISON"))
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
              
            }
        }
        public TestCaseResult Test_166527(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] PatientDetails = PatientID.Split('|');
            string ThumbnailDescriptions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] ThumbnailDescription = ThumbnailDescriptions.Split('|');

            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            string subVolume2 = TestData[1];
            //string subVolume1 = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer Viewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: PatientDetails[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Study Date", PatientDetails[1]);
                PageLoadWait.WaitForFrameLoad(10);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step1 = Z3dViewerPage.selectthumbnail(ThumbnailDescription[0], 0, ThumbnailDescription[1]);
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

                //Step:2
                Boolean step2 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                IWebElement Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step2_1 = verifyBasic3DToolsOperation(Nav1, testid, ExecutedSteps);
                if (step2 && step2_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:3
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step3 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.SmallVessels);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeBlue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Actions act = new Actions(Driver);
                Z3dViewerPage.MoveAndClick(Nav1, Nav1.Size.Width / 2 + 20, Nav1.Size.Height / 2 + 20);
                //new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 15, ViewerPane.Size.Height / 4 - 30).Click().Build().Perform();
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int Afterblue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume = TissueSelectionVolume.Text;
                String[] VolumeValue = SelectionVolume.Split(' ');
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (Convert.ToDouble(VolumeValue[0]) > 0 && Afterblue > BeforeBlue)
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

                //Step:4
                bool step4 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, "10");
                Boolean resStep4 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.ResultPanel, "5");
                Boolean resStep4_1 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel).Equals("5" + " mm");
                if (resStep4 && resStep4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:5
                IWebElement navigationOne = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigationOne).Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(1500);
                bool CrossHairNav1 = navigationOne.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                IWebElement navigationTwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool CrossHairNav2 = navigationTwo.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                IWebElement navigationThree = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                bool CrossHairNav3 = navigationThree.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                if (!CrossHairNav1 && !CrossHairNav2 && !CrossHairNav3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:6
                bool step6 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigationOne);
                PageLoadWait.WaitForPageLoad(20);
                Thread.Sleep(5000);
                bool resStep6 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                bool resStep6_1 = navigationOne.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                bool resStep6_2 = navigationTwo.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                bool resStep6_3 = navigationThree.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                if (resStep6 && !resStep6_1 && !resStep6_2 && !resStep6_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7
                IWebElement navigationOne1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7 = verifyBasic3DToolsOperation(navigationOne1, testid, ExecutedSteps);
                //Selection Tool
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeBlue1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.MoveAndClick(navigationOne1, navigationOne1.Size.Width / 2 + 20, navigationOne1.Size.Height / 2 + 20);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int Afterblue1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (step7 && Afterblue > BeforeBlue)
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

                //Step:8
                bool step8 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, "10");
                Boolean resStep8 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.ResultPanel, "5");
                Boolean resStep8_1 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel).Equals("5" + " mm");
                if (resStep8 && resStep8_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9
                bool step9 = Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", navigationOne);
                PageLoadWait.WaitForPageLoad(20);
                Thread.Sleep(5000);
                bool resStep9 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool resStep9_1 = verifyBasic3DToolsOperation(navigationOne1, testid, ExecutedSteps);
                bool step9_1 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, "10");
                Boolean resStep9_2 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.ResultPanel, "5");
                Boolean resStep9_3 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel).Equals("5" + " mm");
                if (resStep9 && resStep9_1 && step9_1 && resStep9_2 && resStep9_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10
                IWebElement navigationOne2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step10 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.SmallVessels);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeBlue2 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.MoveAndClick(navigationOne2, navigationOne2.Size.Width / 2 + 20, navigationOne2.Size.Height / 2 + 20);
                //new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 15, ViewerPane.Size.Height / 4 - 30).Click().Build().Perform();
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int Afterblue2 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement TissueSelectionVolume2 = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume2 = TissueSelectionVolume2.Text;
                String[] VolumeValue2 = SelectionVolume2.Split(' ');
                if (Convert.ToDouble(VolumeValue2[0]) > 0 && Afterblue2 > BeforeBlue2)
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

                //Step:11
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Threshold, 50);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Radius, 50);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int Afterblue3 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                PageLoadWait.WaitForFrameLoad(5);
                if (Afterblue3 > Afterblue2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:12
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Z3dViewerPage.MoveAndClick(navigationOne2, navigationOne2.Size.Width / 2 + 20, navigationOne2.Size.Height / 2 + 20);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueValueBefore = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                Thread.Sleep(4000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueValueAfterUndo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueValueAfterRedo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                if (BlueValueAfterUndo == 0 && BlueValueAfterRedo != 0)
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

                //Step:13
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                int blueValue_afterReset = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (blueValue_afterReset == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:14
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                PageLoadWait.WaitForFrameLoad(10);
                //PanelCount = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                IList<IWebElement> AllControl = Driver.FindElements(By.CssSelector(Locators.CssSelector.wholepanel));
                IWebElement Panel1Nav3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColourBefore = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                Accord.Point RedPoint = Z3dViewerPage.GetIntersectionPoints(Panel1Nav1, testid, ExecutedSteps + 5, "red", "Horizontal", 11);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                //Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2 +30, (Int32)RedPoint.X, (Int32)RedPoint.Y);
                Actions action = new Actions(Driver);
                action.MoveToElement(Panel1Nav1, (Int32)RedPoint.X, (Int32)RedPoint.Y).Build().Perform();
                Thread.Sleep(2000);
                action.ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2 + 30).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);

                int ColourAfter = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav3D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);

                IWebElement Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                string OrientationBefore = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                Thread.Sleep(2000);
                Actions act2 = new Actions(Driver);
                act2.MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 6 - 5, Navigation3D1.Size.Height / 2).Build().Perform();
                Thread.Sleep(2000);
                act2.ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                act2.MoveToElement(Navigation3D1, Navigation3D1.Size.Width - 10, Navigation3D1.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                string Step14_OrientationAfter = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                // Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (ColourAfter > ColourBefore && OrientationBefore != Step14_OrientationAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:15
                IWebElement Nav3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                /* //try to zoomin images to avoid failures
                 Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                 Z3dViewerPage.Performdragdrop(Nav3D1, (Nav3D1.Size.Width / 2) + 100, (Nav3D1.Size.Height / 2) - 100, (Nav3D1.Size.Width / 2) - 100, (Nav3D1.Size.Height / 2) + 100);
                 PageLoadWait.WaitForFrameLoad(10);
                 Thread.Sleep(4000);
                 */
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step15 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.LargeVessels);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeBlue3 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.MoveAndClick(Nav3D1, Nav3D1.Size.Width / 2 + 30, Nav3D1.Size.Height / 2 + 20);
                //new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 15, ViewerPane.Size.Height / 4 - 30).Click().Build().Perform();
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int Step15_AfterBlue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement TissueSelectionVolume3 = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume3 = TissueSelectionVolume3.Text;
                String[] VolumeValue3 = SelectionVolume3.Split(' ');
                String step15_Volume = VolumeValue3[0];
                int Step15_ColourAfter = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav3D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                // Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (Step15 && Step15_AfterBlue > BeforeBlue3)
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

                //Step:16
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                bool step16 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", Nav3D1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                bool resStep16 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (step16 && resStep16)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:17
                IWebElement Panel1Nav_3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Panel1_Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColourBefore4 = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav_3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                Accord.Point RedPoint1 = Z3dViewerPage.GetIntersectionPoints(Panel1Nav1, testid, ExecutedSteps + 5, "red", "Horizontal", 11);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                Z3dViewerPage.Performdragdrop(Panel1_Nav1, Panel1_Nav1.Size.Width / 2, Panel1_Nav1.Size.Height / 2 + 50, (Int32)RedPoint1.X, (Int32)RedPoint1.Y);
                int ColourAfter4 = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav_3D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);

                IWebElement Navigation_3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                string OrientationBefore1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Navigation_3D1, Navigation_3D1.Size.Width / 6 - 5, Navigation_3D1.Size.Height / 2).ClickAndHold().Perform();
                Thread.Sleep(4000);
                new Actions(Driver).MoveToElement(Navigation_3D1, Navigation_3D1.Size.Width - 10, Navigation_3D1.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                string OrientationAfter1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);

                if (ColourAfter4 > ColourBefore4 && OrientationBefore1 != OrientationAfter1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:18
                bool step18 = Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", Nav3D1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(2000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Thread.Sleep(5000);
                bool resStep18 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                IWebElement Panel3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement PanelNav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColourBefore5 = Z3dViewerPage.LevelOfSelectedColor(Panel3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                string OrientationAfter2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);

                if (resStep18 && Step15_ColourAfter == ColourBefore5 && Step14_OrientationAfter == OrientationAfter2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:19
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int step19_blue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement TissueSelectionVolume4 = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume4 = TissueSelectionVolume4.Text;
                String[] VolumeValue4 = SelectionVolume4.Split(' ');

                if (Convert.ToDouble(step15_Volume) == Convert.ToDouble(VolumeValue4[0]) && step19_blue == Step15_AfterBlue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step15 Volume :" + Convert.ToDouble(step15_Volume));
                    Logger.Instance.InfoLog("Current Volume :" + Convert.ToDouble(VolumeValue4[0]));
                    Logger.Instance.InfoLog("step19_blue :" + step19_blue);
                    Logger.Instance.InfoLog("Step15_AfterBlue :" + Step15_AfterBlue);
                }

                //Step:20
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                IWebElement undoSegButtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.Undo));
                ClickButton(Locators.CssSelector.Undo);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(4000);
                int step20_blue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                IWebElement redoSegButtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.Redo));
                ClickButton(Locators.CssSelector.Redo);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(4000);
                int step20_blueAftr = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (step20_blue == 0 && step20_blueAftr != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:21
                bool step21 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string Step21_Orientation = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (step21 && Step21_Orientation != OrientationAfter2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:22
                IWebElement navigation_One = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step22 = verifyBasic3DToolsOperation(navigation_One, testid, ExecutedSteps);
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

                //Step:23
                IWebElement ViewerPane = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step23 = Z3dViewerPage.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                new Actions(Driver).MoveToElement(ViewerPane).Perform();
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                int Beforevalue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 2, 51, 51, 50, 3);
                //Actions action = new Actions(Driver);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).Click().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 80, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 - 50).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().Build().Perform();
                Thread.Sleep(2000);
                int afterValue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 4, 51, 51, 50, 3);
                Thread.Sleep(4000);
                new Actions(Driver).MoveToElement(ViewerPane).Perform();
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                if (step23 && Beforevalue < afterValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:24
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Average);
                PageLoadWait.WaitForFrameLoad(10);
                bool Step24 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Average);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                PageLoadWait.WaitForFrameLoad(10);
                bool Step24_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                if (Step24 && Step24_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:25
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                bool Step25 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                bool Step25_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                if (Step25 && Step25_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:26
                IWebElement ViewPortOne = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement ViewPortTwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement ViewPortThree = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);

                int BeforeNavigation2 = Z3dViewerPage.LevelOfSelectedColor(ViewPortTwo, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeNavigation3 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree, testid, ExecutedSteps + 3, 0, 0, 0, 2);
                // Rotate the crosshair hotspots in navigationOne controls by 90 degree.
                Accord.Point RedPoint2 = Z3dViewerPage.GetIntersectionPoints(ViewPortOne, testid, ExecutedSteps + 4, "red", "Horizontal");
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point BluePoint = Z3dViewerPage.GetIntersectionPoints(ViewPortOne, testid, ExecutedSteps + 5, "blue", "Vertical");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                //Actions action4 = new Actions(Driver);
                new Actions(Driver).MoveToElement(ViewPortOne, (Int32)RedPoint2.X, (Int32)RedPoint2.Y).Build().Perform();
                Thread.Sleep(5000);
                new Actions(Driver).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(ViewPortOne, (Int32)BluePoint.X, (Int32)BluePoint.Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int AfterNavigation2 = Z3dViewerPage.LevelOfSelectedColor(ViewPortTwo, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterNavigation3 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree, testid, ExecutedSteps + 7, 0, 0, 0, 2);

                // Rotate the crosshair hotspots in navigationTwo controls by 90 degree.
                Accord.Point RedPoint3 = Z3dViewerPage.GetIntersectionPoints(ViewPortTwo, testid, ExecutedSteps + 8, "red", "Horizontal");
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point yellowPoint = Z3dViewerPage.GetIntersectionPoints(ViewPortTwo, testid, ExecutedSteps + 9, "yellow", "Vertical");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                Actions action5 = new Actions(Driver);
                action5.MoveToElement(ViewPortTwo, (Int32)RedPoint3.X, (Int32)RedPoint3.Y).Build().Perform();
                Thread.Sleep(2000);
                action5.ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                action5.MoveToElement(ViewPortTwo, (Int32)yellowPoint.X, (Int32)yellowPoint.Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int AfterNavigation2_1 = Z3dViewerPage.LevelOfSelectedColor(ViewPortTwo, testid, ExecutedSteps + 10, 0, 0, 0, 2);
                int AfterNavigation3_1 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree, testid, ExecutedSteps + 11, 0, 0, 0, 2);

                // Rotate the crosshair hotspots in navigationThree controls by 90 degree.
                Accord.Point yellowPoint2 = Z3dViewerPage.GetIntersectionPoints(ViewPortThree, testid, ExecutedSteps + 12, "yellow", "Horizontal");
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point bluePoint = Z3dViewerPage.GetIntersectionPoints(ViewPortThree, testid, ExecutedSteps + 13, "blue", "Vertical");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                Actions action6 = new Actions(Driver);
                action6.MoveToElement(ViewPortThree, (Int32)yellowPoint2.X, (Int32)yellowPoint2.Y).Build().Perform();
                Thread.Sleep(2000);
                action6.ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                action6.MoveToElement(ViewPortThree, (Int32)bluePoint.X, (Int32)bluePoint.Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int AfterNavigation2_2 = Z3dViewerPage.LevelOfSelectedColor(ViewPortTwo, testid, ExecutedSteps + 14, 0, 0, 0, 2);
                int AfterNavigation3_2 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree, testid, ExecutedSteps + 15, 0, 0, 0, 2);
                if (BeforeNavigation2 != AfterNavigation2 && BeforeNavigation3 != AfterNavigation3 && AfterNavigation2 != AfterNavigation2_1 && AfterNavigation3 != AfterNavigation3_1 && AfterNavigation2_1 != AfterNavigation2_2 && AfterNavigation3_1 != AfterNavigation3_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:27
                IWebElement ViewPort3d1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", ViewPort3d1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step27 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                int step27_blue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (Step27 && step27_blue != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:28
                bool step28 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                int step28_blue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 3, 0, 0, 168, 2);
                if (step28 && step28_blue == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:29
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step29 = verifyBasic3DToolsOperation(navigation1, testid, ExecutedSteps);
                //Selection Tool
                IWebElement navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int resBeforeBlue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.MoveAndClick(navigation3D1, navigation3D1.Size.Width / 2 + 20, navigation3D1.Size.Height / 2 + 20);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int resAfterblue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (step29 && resAfterblue > resBeforeBlue)
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

                //Step:30
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Average);
                PageLoadWait.WaitForFrameLoad(10);
                bool Step30 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Average);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                PageLoadWait.WaitForFrameLoad(10);
                bool Step30_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                bool Step30_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                bool Step30_3 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                if (Step30 && Step30_1 && Step30_2 && Step30_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:31
                bool step31 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                int step31_blue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (step31 && step31_blue == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:32
                IWebElement Nav_3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ViewPortThree_1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                bool step32 = Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", Nav_3D1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(2000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Thread.Sleep(5000);
                bool resStep32 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                bool resStep32_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                bool resStep32_3 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                bool resStep32_4 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Average);
                bool resStep32_5 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                int Step32_AfterNavigation3 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree_1, testid, ExecutedSteps + 15, 0, 0, 0, 2);
                if (AfterNavigation3_2 == Step32_AfterNavigation3 && resStep32 && resStep32_2 && resStep32_3 && resStep32_4 && resStep32_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:33
                Z3dViewerPage.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(10);
                int BeforeUndo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 2, 51, 51, 50, 3);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.UndoSculpt);
                Thread.Sleep(4000);
                int AfterUndo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 4, 51, 51, 50, 3);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Redosculpt);
                Thread.Sleep(4000);
                int AfterRedo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 6, 51, 51, 50, 3);
                if (BeforeUndo > AfterUndo && AfterUndo < AfterRedo)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:34
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                int BeforeUndo1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 2, 51, 51, 50, 3);
                IWebElement undoSegButtn1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.Undo));
                ClickButton(Locators.CssSelector.Undo);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(4000);
                int AfterUndo1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 4, 51, 51, 50, 3);
                IWebElement redoSegButtn1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.Redo));
                ClickButton(Locators.CssSelector.Redo);
                Thread.Sleep(4000);
                int AfterRedo1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 6, 51, 51, 50, 3);
                if (BeforeUndo1 > AfterUndo1 && AfterUndo1 < AfterRedo1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:35
                bool Step35 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                bool Step35_1 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (Step35 && Step35_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:36
                IWebElement Panel1NavOne = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement MprPathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int BeforeColour1 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int BeforeColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavOne).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).MoveToElement(Panel1NavOne, Panel1NavOne.Size.Width / 2, Panel1NavOne.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1NavOne, Panel1NavOne.Size.Width / 2, Panel1NavOne.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int AfterColour1 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne, testid, ExecutedSteps + 5, 0, 0, 255, 2);
                int AfterColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavOne).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                if (BeforeColour1 != AfterColour1 && BeforeColour2 != AfterColour2 && BeforeColour3 != AfterColour3 && BeforeColour4 != AfterColour4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:37
                IWebElement navigationone1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step37 = verifyBasic3DToolsOperation(navigationone1, testid, ExecutedSteps, checkLine: false);
                if (step37)
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

                //Step:38
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.MPRPathNavigation, "1");
                Boolean step38 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.MPRPathNavigation).Equals("1" + " mm");
                //Verification for further steps
                int Step38Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step38MprPath = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int Step38_3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int Step38Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (step38)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:39
                IWebElement navigation_3DPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigation_3DPath);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForProgressBarToDisAppear();
                Thread.Sleep(5000);
                bool Step39 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (Step39)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:40
                bool Step40 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                IWebElement Panel1NavTwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement MprPathnav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);

                int BefColourNav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColour2_1 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav1, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3_1 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int BeforeColour4_1 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath1, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavTwo).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(Panel1NavTwo, Panel1NavTwo.Size.Width / 2 - 20, Panel1NavTwo.Size.Height / 2 - 20).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool checkerrMsg = Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo, Panel1NavTwo.Size.Width / 2 + 20, Panel1NavTwo.Size.Height / 2 - 25).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool checkerrMsg2 = Z3dViewerPage.checkerrormsg(clickok: "y");
                int AftColourNav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                int AfterColour2_1 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav1, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3_1 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav1, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterColour4_1 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath1, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavTwo).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                if (AftColourNav2 > BefColourNav2 && !checkerrMsg && !checkerrMsg2 && BeforeColour2_1 != AfterColour2_1 && BeforeColour3_1 != AfterColour3_1 && BeforeColour4_1 != AfterColour4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:41
                IWebElement navigationone_1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step41 = verifyBasic3DToolsOperation(navigationone_1, testid, ExecutedSteps, checkLine: false);
                //Verification for further steps
                int Step40Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step40MprPath = Z3dViewerPage.LevelOfSelectedColor(MprPathnav1, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int Step40_3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav1, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int Step40Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath1, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (step41)
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

                //Step:42
                IWebElement navigation_3DPath1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S5", "MR", "22", navigation_3DPath1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step42 = Z3dViewerPage.VerifyThumbnail_Highligted("S5", "MR", "22");
                if (Step42)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:43
                bool Step43 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                IWebElement Panel1NavTwo2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement MprPathnav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);

                int BefColourNav1_2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColour2_2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3_2 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int BeforeColour4_2 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavTwo2).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).MoveToElement(Panel1NavTwo2, Panel1NavTwo2.Size.Width / 2 - 3, Panel1NavTwo2.Size.Height / 2 + 4).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool checkerrMsg1 = Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo2, Panel1NavTwo2.Size.Width / 2 - 3, Panel1NavTwo2.Size.Height / 2 + 6).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool checkerrMsg2_1 = Z3dViewerPage.checkerrormsg(clickok: "y");
                int AftColourNav1_2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                int AfterColour2_2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3_2 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterColour4_2 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavTwo2).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                if (AftColourNav1_2 > BefColourNav1_2 && BeforeColour2_2 != AfterColour2_2 && BeforeColour3_2 != AfterColour3_2 && BeforeColour4_2 != AfterColour4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:44
                IWebElement navigationone_2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step44 = verifyBasic3DToolsOperation(navigationone_2, testid, ExecutedSteps, checkLine: false);
                //Verification for further steps
                int Step44Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step44MprPath = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int Step44_3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int Step44Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (step44)
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

                //Step:45
                IWebElement navigation_3D = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigation_3D);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step45 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                int Step45Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step45MprPath = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int Step45_3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int Step45Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (Step45 && Step40Nav2 == Step45Nav2 && Step40MprPath == Step45MprPath && Step40_3D == Step45_3D && Step40Curved == Step45Curved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:46
                bool Step46 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int AfterReset_Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int AfterReset_MprPath = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterReset_3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterReset_Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (Step46 && AfterReset_Nav2 != Step45Nav2 && AfterReset_MprPath != Step45MprPath && AfterReset_3D != Step45_3D && AfterReset_Curved != Step45Curved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:47
                IWebElement navigation3D = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S5", "MR", "22", navigation3D);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step47 = Z3dViewerPage.VerifyThumbnail_Highligted("S5", "MR", "22");
                int Step47Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step47MprPath = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int Step47_3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int Step47Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (Step47 && Step44Nav2 == Step47Nav2 && Step44MprPath == Step47MprPath && Step44_3D == Step47_3D && Step44Curved == Step47Curved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:48
                bool Step48 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int AfterReset_Nav2_1 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int AfterReset_MprPath1 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterReset_3D1 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterReset_Curved1 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (Step46 && AfterReset_Nav2_1 != Step47Nav2 && AfterReset_MprPath1 != Step47MprPath && AfterReset_3D1 != Step47_3D && AfterReset_Curved1 != Step47Curved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:49
                IWebElement navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", navigation3D2);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step49 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                int Step49Nav = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step49MprPath = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int Step49_3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int Step49Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (Step49 && Step38Nav2 == Step49Nav && Step38MprPath == Step49MprPath && Step38_3D == Step49_3D && Step49Curved == Step38Curved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:50
                bool Step50 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int AfterReset_Nav2_2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int AfterReset_MprPath2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterReset_3D2 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterReset_Curved2 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (Step46 && AfterReset_Nav2_2 != Step49Nav && AfterReset_MprPath2 != Step49MprPath && AfterReset_3D2 != Step49_3D && AfterReset_Curved2 != Step49Curved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:51
                bool Step51 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                Thread.Sleep(5000);
                bool errormsg = Z3dViewerPage.checkerrormsg(clickok: "y");
                if (Step51 && errormsg)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:52
                IWebElement calciumscorNav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step52 = verifyBasic3DToolsOperation(calciumscorNav, testid, ExecutedSteps, BluRingZ3DViewerPage.CalciumScoring,checkZoomTool: false, checkPanTool: false, checkLine: false, checkRotate: false);
                if (Step52) 
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:53
                bool Step53 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesBefore = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                IWebElement Calpan2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP2 = Z3dViewerPage.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan2, (Calpan2.Size.Width - 10), (Calpan2.Size.Height / 4)).
                                    MoveToElement(Calpan2, (Calpan2.Size.Width - 10), Calpan2.Size.Height / 2).
                                    MoveToElement(Calpan2, (Calpan2.Size.Width / 2), Calpan2.Size.Height / 2).
                                    MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP2 = Z3dViewerPage.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesAfter = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorBeforeP2 != GreenColorAfterP2 && ScoreValuesBefore[0] < ScoreValuesAfter[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:54
                IWebElement navigationCalcium = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigationCalcium);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step54 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (Step54 && CheckErrormsg)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:55
                IWebElement calciumscorNav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step55 = verifyBasic3DToolsOperation(calciumscorNav1, testid, ExecutedSteps, BluRingZ3DViewerPage.CalciumScoring, checkZoomTool: false, checkPanTool: false, checkLine: false, checkRotate: false);
                if (Step55)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:56
                bool Step56 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                bool Step56_1 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                IList<Double> ScoreValuesBefore1 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                IWebElement Calpan3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP3 = Z3dViewerPage.LevelOfSelectedColor(Calpan3, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan3, Calpan3.Size.Width / 2, (Calpan3.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan3, (Calpan3.Size.Width - 10), (Calpan3.Size.Height / 4)).
                                    MoveToElement(Calpan3, (Calpan3.Size.Width - 10), Calpan3.Size.Height / 2).
                                    MoveToElement(Calpan3, (Calpan3.Size.Width / 2), Calpan3.Size.Height / 2).
                                    MoveToElement(Calpan3, Calpan3.Size.Width / 2, (Calpan3.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP3 = Z3dViewerPage.LevelOfSelectedColor(Calpan3, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesAfter1 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorBeforeP3 != GreenColorAfterP3 && ScoreValuesBefore1[0] < ScoreValuesAfter1[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:57
                IWebElement navigationCalcium1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", navigationCalcium1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg1 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step57 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                //Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesAfter2 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                int GreenColorAfterP4 = Z3dViewerPage.LevelOfSelectedColor(navigationCalcium1, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorAfterP4 == GreenColorAfterP2 && ScoreValuesAfter2[0] == ScoreValuesAfter[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:58
                bool Step58 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                bool Step58_1 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                IList<Double> ScoreValuesBefore3 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                IWebElement Calpan4 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP5 = Z3dViewerPage.LevelOfSelectedColor(Calpan4, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan4, Calpan4.Size.Width / 2, (Calpan4.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan4, (Calpan4.Size.Width - 10), (Calpan4.Size.Height / 4)).
                                    MoveToElement(Calpan4, (Calpan4.Size.Width - 10), Calpan4.Size.Height / 2).
                                    MoveToElement(Calpan4, (Calpan4.Size.Width / 2), Calpan4.Size.Height / 2).
                                    MoveToElement(Calpan4, Calpan4.Size.Width / 2, (Calpan4.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int GreenColorAfterP5 = Z3dViewerPage.LevelOfSelectedColor(Calpan4, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesAfter3 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorBeforeP5 > GreenColorAfterP5 && ScoreValuesBefore3[0] > ScoreValuesAfter3[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:59
                IWebElement navigationCalcium2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigationCalcium2);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg3 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step59 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                IList<Double> ScoreValuesAfter6 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                int GreenColorAfterP6 = Z3dViewerPage.LevelOfSelectedColor(navigationCalcium1, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorAfterP6 == GreenColorAfterP3 && ScoreValuesAfter6[0] == ScoreValuesAfter1[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:60
                bool Step60 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                bool Step60_1 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                IList<Double> ScoreValuesBefore6 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                IWebElement Calpan5 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP7 = Z3dViewerPage.LevelOfSelectedColor(Calpan5, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan5, Calpan5.Size.Width / 2, (Calpan5.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan5, (Calpan5.Size.Width - 10), (Calpan5.Size.Height / 4)).
                                    MoveToElement(Calpan5, (Calpan5.Size.Width - 10), Calpan5.Size.Height / 2).
                                    MoveToElement(Calpan5, (Calpan5.Size.Width / 2), Calpan5.Size.Height / 2).
                                    MoveToElement(Calpan5, Calpan5.Size.Width / 2, (Calpan5.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP7 = Z3dViewerPage.LevelOfSelectedColor(Calpan5, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesAfter7 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorBeforeP7 > GreenColorAfterP7 && ScoreValuesBefore6[0] > ScoreValuesAfter7[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:61
                bool Step61_1 = false;
                IWebElement NavigationCalcium = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", NavigationCalcium);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg4 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step61 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                IList<IWebElement> SelectionOptions = Driver.FindElements(By.CssSelector(Locators.CssSelector.dialogRadioBtn));
                foreach (IWebElement radio in SelectionOptions)
                {
                    if (radio.Text.Equals("Deselect this slice"))
                    {
                        if (radio.Selected)
                        {
                            Step61_1 = true;
                        }
                    }
                }
                if (Step61 & Step61_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:62
                bool Step62_1 = false;
                IWebElement navigationCalcium3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigationCalcium3);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg5 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step62 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                IList<IWebElement> SelectionOptions1 = Driver.FindElements(By.CssSelector(Locators.CssSelector.dialogRadioBtn));
                foreach (IWebElement radio in SelectionOptions1)
                {
                    if (radio.Text.Equals("Deselect all contiguous"))
                    {
                        if (radio.Selected)
                        {
                            Step62_1 = true;
                        }
                    }
                }
                if (Step62 & Step62_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:63
                bool Step63 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                IList<IWebElement> viewmode = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdownText));
                string Viewmodetext = viewmode[0].GetAttribute("innerText");
                if (Viewmodetext.Equals(BluRingZ3DViewerPage.Two_2D) && Step63)
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        /// <summary>
        /// To verify Basic 3D Tools Operations. Basic Tools Like, 
        /// 1. Scroll, 2. Zoom, 3. Roam, 4. Window level, 5. Measurement, 6. Rotate
        /// </summary>
        /// <param name="Nav1"></param>
        /// <param name="testid"></param>
        /// <param name="ExecutedSteps"></param>
        /// <param name="checkScrollTool"></param>
        /// <param name="checkZoomTool"></param>
        /// <param name="checkPanTool"></param>
        /// <param name="checkWindowLevel"></param>
        /// <param name="checkLine"></param>
        /// <param name="checkRotate"></param>
        /// <returns></returns>
        public Boolean verifyBasic3DToolsOperation(IWebElement element, String testid, int ExecutedSteps, String navigation = "Navigation 1", bool checkScrollTool = true, bool checkZoomTool = true, bool checkPanTool = true, bool checkWindowLevel = true, bool checkLine = true, bool checkRotate = true)
        {
            bool res = false;
            bool resScrollTool = false;
            bool resZoomTool = false;
            bool resPanTool = false;
            bool resWindowLevel = false;
            bool resLine = false;
            bool resRotate = false;
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();

            //============Scrolling==========================
            if (checkScrollTool)
            {
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                String beforeScroll = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                Z3dViewerPage.Performdragdrop(element, element.Size.Width / 2 + 20, element.Size.Height / 2 - 20, element.Size.Width / 2 - 20, element.Size.Height / 2 + 20, RemoveCross: true);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                String afterScroll = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                if (beforeScroll == afterScroll)
                {
                    Logger.Instance.ErrorLog("Scroll Tool Not Working As expected. Result Values :beforeScroll - " + beforeScroll + "and afterScroll - " + afterScroll);
                    resScrollTool = false;
                }
                else
                {
                    resScrollTool = true;
                }

            }
            else
            {
                Logger.Instance.InfoLog("ScrollTool Function Not enabled");
                resScrollTool = true;
            }

            //==========Intractivezoom========================
            if (checkZoomTool)
            {
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                String beforeZoom = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                Z3dViewerPage.Performdragdrop(element, element.Size.Width / 2 + 20, element.Size.Height / 2 - 20, element.Size.Width / 2 - 20, element.Size.Height / 2 + 20, RemoveCross: true);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                String afterZoom = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                if (beforeZoom == afterZoom)
                {
                    Logger.Instance.ErrorLog("Zoom Tool Not Working As expected. Result Values :beforeZoom - " + beforeZoom + "and afterZoom - " + afterZoom);
                    resZoomTool = false;
                }
                else
                {
                    resZoomTool = true;
                }
            }
            else
            {
                Logger.Instance.InfoLog("ZoomTool Function Not enabled");
                resZoomTool = true;
            }

            //===================Pan Tool===============
            if (checkPanTool)
            {
                Z3dViewerPage.select3DTools(Z3DTools.Pan);
                String beforePan = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                Z3dViewerPage.Performdragdrop(element, element.Size.Width / 2 - 5, element.Size.Height / 2 + 5, element.Size.Width / 2 + 5, element.Size.Height / 2 - 5, RemoveCross: true);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                String afterPan = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                if (beforePan == afterPan)
                {
                    Logger.Instance.ErrorLog("Pan Tool Not Working As expected. Result Values :beforePan - " + beforePan + "and afterPan - " + afterPan);
                    resPanTool = false;
                }
                else
                {
                    resPanTool = true;
                }
            }
            else
            {
                Logger.Instance.InfoLog("PanTool Function Not enabled");
                resPanTool = true;
            }

            //===============Window Level===================
            if (checkWindowLevel)
            {
                // Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level);
                String beforeWLValue = Z3dViewerPage.GetWindowLevelValue(navigation);
                Z3dViewerPage.Performdragdrop(element, element.Size.Width / 2 + 10, element.Size.Height / 2 - 10, element.Size.Width / 2 - 10, element.Size.Height / 2 + 10, RemoveCross: true);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                String afterWLValue = Z3dViewerPage.GetWindowLevelValue(navigation);
                if (beforeWLValue == afterWLValue)
                {
                    Logger.Instance.ErrorLog("WLValue Tool Not Working As expected. Result Values :beforeWLValue - " + beforeWLValue + "and afterWLValue - " + afterWLValue);
                    resWindowLevel = false;
                }
                else
                {
                    resWindowLevel = true;
                }
            }
            else
            {
                Logger.Instance.InfoLog("Window Level Tool Function Not enabled");
                resWindowLevel = true;
            }

            //================Rotate Tool==================
            if (checkRotate)
            {
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                String beforeRotate = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                Z3dViewerPage.Performdragdrop(element, element.Size.Width / 2 + 5, element.Size.Height / 2 - 5, element.Size.Width / 2 - 5, element.Size.Height / 2 + 5, RemoveCross: true);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Thread.Sleep(5000);
                String afterRotate = Z3dViewerPage.GetTopleftAnnotationLocationValue(navigation);
                if (afterRotate == beforeRotate)
                {
                    Logger.Instance.ErrorLog("RotateTool Tool Not Working As expected. Result Values :beforeRotateTool - " + beforeRotate + "and afterRotateTool - " + afterRotate);
                    resRotate = false;
                }
                else
                {
                    resRotate = true;
                }
            }
            else
            {
                Logger.Instance.InfoLog("RotateToolFunction Not enabled");
                resRotate = true;
            }
            //=====================Line Measurement=================
            if (checkLine)
            {
                bool lineMeasurement = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                int BeforeYellowClr = Z3dViewerPage.LevelOfSelectedColor(element, testid, ExecutedSteps + 1, 255, 255, 0, 2);
                new Actions(Driver).MoveToElement(element, element.Size.Width / 2 + 25, element.Size.Height / 2 - 25).ClickAndHold().
                MoveToElement(element, element.Size.Width / 2 - 20, element.Size.Height / 2 + 20).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                int AfterYellowClr = Z3dViewerPage.LevelOfSelectedColor(element, testid, ExecutedSteps + 2, 255, 255, 0, 2);

                if (BeforeYellowClr == AfterYellowClr)
                {
                    Logger.Instance.ErrorLog("Line Measurement Tool is not working. Result of BeforeYellowClr is " + BeforeYellowClr + " .AfterYellowClr is " + AfterYellowClr);
                    resLine = false;
                }
                else
                {
                    resLine = true;
                }
            }
            else
            {
                Logger.Instance.InfoLog("Line Measurement Tool Function Not enabled");
                resLine = true;
            }
            if (resScrollTool && resZoomTool && resPanTool && resWindowLevel && resLine && resRotate)
            {
                Logger.Instance.InfoLog("verifyBasic3DToolsOperation Method completed");
                res = true;
            }
            return res;
        }


        public TestCaseResult Test_168912(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] PatientDetails = PatientID.Split('|');
            string ThumbnailDescriptions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] ThumbnailDescription = ThumbnailDescriptions.Split('|');

            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            string subVolume2 = TestData[1];
            //string subVolume1 = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer Viewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            Imager imager = new Imager();
           //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: PatientDetails[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Study Date", PatientDetails[1]);
                PageLoadWait.WaitForFrameLoad(10);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Z3dViewerPage.selectthumbnail(ThumbnailDescription[0], 0, ThumbnailDescription[1]);
                Boolean step1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
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

                //Step:2 :: Select and load series 3 in 3D viewer in MPR view. Apply the 3D tool operations in the MPR controls.
                IWebElement Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step2_1 = verifyBasic3DToolsOperation(Nav1, testid, ExecutedSteps, checkLine: false, checkWindowLevel: false);
                if (step2_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:3
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step3 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.SmallVessels);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                IWebElement Nav3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone)).Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(1500);
                int BeforeBlue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Actions act = new Actions(Driver);
                // Z3dViewerPage.MoveAndClick(Nav3, Nav3.Size.Width / 2 + 20, Nav3.Size.Height / 2 - 20);
                new Actions(Driver).MoveToElement(Nav3, Nav3.Size.Width / 2 + 20, Nav3.Size.Height / 2 - 20).Click().Build().Perform();
                Thread.Sleep(5000);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int Afterblue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForPageLoad(25);
                Thread.Sleep(8000);
                IWebElement TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume = TissueSelectionVolume.Text;
                String[] VolumeValue = SelectionVolume.Split(' ');
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone)).Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(1500);
                if (Convert.ToDouble(VolumeValue[0]) > 0 && Afterblue > BeforeBlue)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("VolumeValue:" + Convert.ToDouble(VolumeValue[0]));
                    Logger.Instance.InfoLog("Afterblue:" + Afterblue);
                    Logger.Instance.InfoLog("BeforeBlue:" + BeforeBlue);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("VolumeValue:" + Convert.ToDouble(VolumeValue[0]));
                    Logger.Instance.InfoLog("Afterblue:" + Afterblue);
                    Logger.Instance.InfoLog("BeforeBlue:" + BeforeBlue);
                }

                //Step:4
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                bool Step4_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                bool Step4_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.MinIp);
                bool Step4_3 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MinIp);
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationone, "10.0");
                //Verification::Thickness value in all the MPR navigation controls are updated as 10 mm.
                Thread.Sleep(1500);
                bool Step4_4 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationone).Equals("10" + " mm");
                bool Step4_5 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                bool Step4_6 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree).Equals("10" + " mm");
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.ResultPanel, "5.0");
                bool Step4_7 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel).Equals("5" + " mm");
                if (Step4_1 && Step4_2 && Step4_3 && Step4_4 && Step4_5 && Step4_6 && Step4_7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:5 :: Toggle OFF the crosshairs in MPR navigation controls by selecting the Hide 3D controls from SHOW/HIDE drop down (or) press 'X'.
                IWebElement navigationOne = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(navigationOne).Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1500);
                bool CrossHairNav1 = navigationOne.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                IWebElement navigationTwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool CrossHairNav2 = navigationTwo.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                IWebElement navigationThree = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                bool CrossHairNav3 = navigationThree.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                //int ReferenceValue_Step9= Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 133, 133, 131, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String BeforeImagePath3 = Config.downloadpath + "\\Step5.png";
                PageLoadWait.WaitForFrameLoad(25);
                DownloadImageFile(navigationOne, BeforeImagePath3, "png");
                //Crop image. Due to missmatch annotations in the image
                String CropImagePathS5 = result.steps[ExecutedSteps].testimagepath;
                Logger.Instance.InfoLog("CropImagePathS5 Path is  " + CropImagePathS5);
                imager.CropAndSaveImage(BeforeImagePath3, 0, 32, navigationOne.Size.Width, navigationOne.Size.Height - 35, CropImagePathS5);
                if (!CrossHairNav1 && !CrossHairNav2 && !CrossHairNav3)
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

                //Step:6
                new Actions(Driver).MoveToElement(navigationOne).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1500);
                bool step6 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigationOne);
                PageLoadWait.WaitForPageLoad(20);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                bool resStep6 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                bool resStep6_1 = navigationOne.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                bool resStep6_2 = navigationTwo.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                bool resStep6_3 = navigationThree.GetCssValue("cursor").Contains(BluRingZ3DViewerPage.CrossHairCursor);
                new Actions(Driver).MoveToElement(navigationOne).Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                if (resStep6 && !resStep6_1 && !resStep6_2 && !resStep6_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7
                IWebElement navigationOne1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7 = verifyBasic3DToolsOperation(navigationOne1, testid, ExecutedSteps, checkLine: false);
                //Selection Tool
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(10000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int BeforeBlue1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                IWebElement navigationOne1_1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //Z3dViewerPage.MoveAndClick(navigationOne1_1, navigationOne1_1.Size.Width / 2 + 20, navigationOne1_1.Size.Height / 2 - 20);
                new Actions(Driver).MoveToElement(navigationOne1_1, navigationOne1_1.Size.Width / 2 + 20, navigationOne1_1.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int Afterblue1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 3, 0, 0, 168, 2);
                Thread.Sleep(2000);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (step7 && Afterblue1 > BeforeBlue1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Afterblue1:" + Afterblue1);
                    Logger.Instance.InfoLog("BeforeBlue1:" + BeforeBlue1);
                }

                //Step:8
                bool step8 = Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                Thread.Sleep(2000);
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, "10");
                Thread.Sleep(2000);
                Boolean resStep8 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                Thread.Sleep(2000);
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.ResultPanel, "5");
                Thread.Sleep(2000);
                Boolean resStep8_1 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel).Equals("5" + " mm");
                if (resStep8 && resStep8_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9
                bool step9 = false;
                IWebElement navigationOne_1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", navigationOne_1);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForPageLoad(20);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                bool resStep9 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step9_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                Thread.Sleep(3000);
                Boolean resStep9_2 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo).Equals("10" + " mm");
                Thread.Sleep(3000);
                Boolean resStep9_3 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel).Equals("5" + " mm");
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(navigationOne).Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(3000);
                //int ReferenceValue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 133, 133, 131, 2);
                String AfterImagePath3 = Config.downloadpath + "\\Step9.png";
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(navigationOne_1, AfterImagePath3, "png");
                //String CropImagePathS9 = Config.downloadpath + "\\CropImageStep9.png";
                String CropImagePathS9 = result.steps[ExecutedSteps].testimagepath;
                Logger.Instance.InfoLog("CropImagePathS9 path is " + CropImagePathS9);
                imager.CropAndSaveImage(AfterImagePath3, 0, 32, navigationOne_1.Size.Width, navigationOne_1.Size.Height - 35, CropImagePathS9);

                if (Z3dViewerPage.CompareImageWithDiff(result.steps[ExecutedSteps], CropImagePathS9, CropImagePathS5) == true)
                {
                    step9 = true;
                    Logger.Instance.InfoLog("step9 image Comparission Passed");
                }

                if (resStep9 && step9_1 && resStep9_2 && resStep9_3 && step9)
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

                //Step:10
                bool Step10 = false;
                new Actions(Driver).MoveToElement(navigationOne_1).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(3000);
                IWebElement navigationOne2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int Afterblue2 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement TissueSelectionVolume2 = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume2 = TissueSelectionVolume2.Text;
                String[] VolumeValue2 = SelectionVolume2.Split(' ');
                IList<IWebElement> SelectionOptions1 = Driver.FindElements(By.XPath("//mat-radio-button"));
                foreach (IWebElement radio in SelectionOptions1)
                {
                    if (radio.Text.Equals("Small vessels"))
                    {
                        if (radio.GetAttribute("class").Contains("mat-radio-checked"))
                        {
                            Logger.Instance.InfoLog("Small vessels radio highlighted");
                            Step10 = true;
                            break;
                        }
                    }
                }
                if (Convert.ToDouble(VolumeValue2[0]) == Convert.ToDouble(VolumeValue[0]) && Step10)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Convert.ToDouble(VolumeValue2[0]) :" + Convert.ToDouble(VolumeValue2[0]));
                    Logger.Instance.InfoLog("Afterblue2 :" + Afterblue2);
                }

                //Step:11
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Threshold, 25);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Radius, 25);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int Afterblue3 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                PageLoadWait.WaitForFrameLoad(25);
                if (Afterblue3 < Afterblue2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Afterblue3:" + Afterblue3);
                }

                //Step:12
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(8000);
                // Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(4000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(10000);
                Z3dViewerPage.checkerrormsg("y");
                int BlueValueAfterUndo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 3, 0, 0, 168, 2);
                Thread.Sleep(4000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(10000);
                Z3dViewerPage.checkerrormsg("y");
                int BlueValueAfterRedo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 5, 0, 0, 168, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                if (BlueValueAfterUndo < BlueValueAfterRedo)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("BlueValueAfterUndo :" + BlueValueAfterUndo);
                    Logger.Instance.InfoLog("BlueValueAfterRedo :" + BlueValueAfterRedo);
                }

                //Step:13
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(5000);
                int blueValue_afterReset = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (blueValue_afterReset != BlueValueAfterRedo)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:14
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(10000);
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close); } catch (Exception e) { }
                IList<IWebElement> AllControl = Driver.FindElements(By.CssSelector(Locators.CssSelector.wholepanel));
                IWebElement Panel1Nav3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColourBefore = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                Accord.Point RedPoint = Z3dViewerPage.GetIntersectionPoints(Panel1Nav1, testid, ExecutedSteps + 5, "red", "Horizontal", 11);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                //Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2 +30, (Int32)RedPoint.X, (Int32)RedPoint.Y);
                Actions action = new Actions(Driver);
                action.MoveToElement(Panel1Nav1, (Int32)RedPoint.X, (Int32)RedPoint.Y).Build().Perform();
                Thread.Sleep(2000);
                action.ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 2 + 30).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColourAfter = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav3D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                IWebElement Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                string OrientationBefore = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation3D1).Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(1500);
                Actions act2 = new Actions(Driver);
                act2.MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 2, Navigation3D1.Size.Height / 2).Build().Perform();
                Thread.Sleep(2000);
                act2.MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 6 - 5, Navigation3D1.Size.Height / 2).Build().Perform();
                Thread.Sleep(2000);
                act2.ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                act2.MoveToElement(Navigation3D1, Navigation3D1.Size.Width - 10, Navigation3D1.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                string Step14_OrientationAfter = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                // Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (ColourAfter > ColourBefore && OrientationBefore != Step14_OrientationAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("ColourAfter:" + ColourAfter);
                    Logger.Instance.InfoLog("ColourBefore:" + ColourBefore);
                    Logger.Instance.InfoLog("Step14_OrientationAfter:" + Step14_OrientationAfter);
                    Logger.Instance.InfoLog("OrientationBefore:" + OrientationBefore);
                    Logger.Instance.InfoLog("Step14_OrientationAfter:" + Step14_OrientationAfter);
                }

                //Step:15
                IWebElement Nav3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                /* //try to zoomin images to avoid failures
                 Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                 Z3dViewerPage.Performdragdrop(Nav3D1, (Nav3D1.Size.Width / 2) + 100, (Nav3D1.Size.Height / 2) - 100, (Nav3D1.Size.Width / 2) - 100, (Nav3D1.Size.Height / 2) + 100);
                 PageLoadWait.WaitForFrameLoad(10);
                 Thread.Sleep(4000);
                 */
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(5000);
                bool Step15 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.LargeVessels);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeBlue3 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                //Z3dViewerPage.MoveAndClick(Nav3D1, Nav3D1.Size.Width / 2 + 30, Nav3D1.Size.Height / 2 + 20);
                new Actions(Driver).MoveToElement(Nav3D1, Nav3D1.Size.Width / 2 + 30, Nav3D1.Size.Height / 2 + 20).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Z3dViewerPage.checkerrormsg();
                int Step15_AfterBlue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 2, 0, 0, 168, 2);
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(8000);
                IWebElement TissueSelectionVolume3 = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume3 = TissueSelectionVolume3.Text;
                String[] VolumeValue3 = SelectionVolume3.Split(' ');
                String step15_Volume = VolumeValue3[0];
                //int Step15_ColourAfter = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav3D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                IWebElement element = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                String BeforeImagePath = result.steps[ExecutedSteps].testimagepath;
                Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(element, BeforeImagePath, "png");
                if (Step15 && Step15_AfterBlue > BeforeBlue3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step15_AfterBlue" + Step15_AfterBlue);
                    Logger.Instance.InfoLog("BeforeBlue3" + BeforeBlue3);
                }

                //Step:16
                bool step16 = Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", Nav3D1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                bool resStep16 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (step16 && resStep16)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:17
                IWebElement Panel1Nav_3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Panel1_Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColourBefore4 = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav_3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                Accord.Point RedPoint1 = Z3dViewerPage.GetIntersectionPoints(Panel1Nav1, testid, ExecutedSteps + 5, "red", "Horizontal", 11);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                Z3dViewerPage.Performdragdrop(Panel1_Nav1, Panel1_Nav1.Size.Width / 2, Panel1_Nav1.Size.Height / 2 + 50, (Int32)RedPoint1.X, (Int32)RedPoint1.Y);
                Thread.Sleep(5000);
                int ColourAfter4 = Z3dViewerPage.LevelOfSelectedColor(Panel1Nav_3D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);

                IWebElement Navigation_3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                string OrientationBefore1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Navigation_3D1, Navigation_3D1.Size.Width / 6 - 5, Navigation_3D1.Size.Height / 2).ClickAndHold().Perform();
                Thread.Sleep(4000);
                new Actions(Driver).MoveToElement(Navigation_3D1, Navigation_3D1.Size.Width - 10, Navigation_3D1.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                string OrientationAfter1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);

                if (ColourAfter4 != ColourBefore4 && OrientationBefore1 != OrientationAfter1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("ColourAfter4:" + ColourAfter4);
                    Logger.Instance.InfoLog("ColourBefore4:" + ColourBefore4);
                    Logger.Instance.InfoLog("OrientationBefore1:" + OrientationBefore1);
                    Logger.Instance.InfoLog("OrientationAfter1:" + OrientationAfter1);
                }

                //Step:18
                bool step18 = false;
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", Nav3D1);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Thread.Sleep(5000);
                bool resStep18 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                IWebElement Panel3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement PanelNav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                //int ColourBefore5 = Z3dViewerPage.LevelOfSelectedColor(Panel3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                string OrientationAfter2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String AfterImagePath = result.steps[ExecutedSteps].testimagepath;
                Logger.Instance.InfoLog("AfterImagePath path is " + AfterImagePath);
                PageLoadWait.WaitForFrameLoad(25);
                DownloadImageFile(PanelNav1, AfterImagePath, "png");
                if (Z3dViewerPage.CompareImageWithDiff(result.steps[ExecutedSteps], BeforeImagePath, AfterImagePath) == true)
                {
                    step18 = true;
                    Logger.Instance.InfoLog("Step18 image Comparision Passed");
                }
                if (resStep18 && step18 && Step14_OrientationAfter == OrientationAfter2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step14_OrientationAfter:" + Step14_OrientationAfter);
                    Logger.Instance.InfoLog("OrientationAfter2:" + OrientationAfter2);
                }

                //Step:19
                bool Step19 = false;
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int step19_blue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement TissueSelectionVolume4 = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume4 = TissueSelectionVolume4.Text;
                String[] VolumeValue4 = SelectionVolume4.Split(' ');
                IList<IWebElement> SelectionOptions2 = Driver.FindElements(By.XPath("//mat-radio-button"));
                foreach (IWebElement radio in SelectionOptions2)
                {
                    if (radio.Text.Equals("Large vessels"))
                    {
                        if (radio.GetAttribute("class").Contains("mat-radio-checked"))
                        {
                            Logger.Instance.InfoLog("Large vessels radio highlighted");
                            Step19 = true;
                            break;
                        }
                    }
                }
                if (Convert.ToDouble(step15_Volume) == Convert.ToDouble(VolumeValue4[0]) && Step19 && Enumerable.Range(Step15_AfterBlue - 50, Step15_AfterBlue + 50).Contains(step19_blue))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step15 Volume :" + Convert.ToDouble(step15_Volume));
                    Logger.Instance.InfoLog("Current Volume :" + Convert.ToDouble(VolumeValue4[0]));
                    Logger.Instance.InfoLog("step19_blue :" + step19_blue);
                    Logger.Instance.InfoLog("Step15_AfterBlue :" + Step15_AfterBlue);
                }

                //Step:20
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.UndoSegmentation);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(8000);
                int step20_blue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RedoSegmentation);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(8000);
                int step20_blueAftr = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (step20_blue < step19_blue && step20_blueAftr != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("step20_blue:" + step20_blue);
                    Logger.Instance.InfoLog("step20_blueAftr:" + step20_blueAftr);
                }

                //Step:21
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(5000);
                int blueValue_afterReset21 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 1, 0, 0, 168, 2);
                if (blueValue_afterReset21 != step20_blueAftr)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Blue value after reset:"+ blueValue_afterReset21);
                    Logger.Instance.InfoLog("Blue value before reset:" + step20_blueAftr);
                }

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_170146(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] PatientDetails = PatientID.Split('|');
            string ThumbnailDescriptions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] ThumbnailDescription = ThumbnailDescriptions.Split('|');

            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            string subVolume2 = TestData[1];
            //string subVolume1 = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer Viewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            Imager imager = new Imager();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: PatientDetails[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Study Date", PatientDetails[1]);
                PageLoadWait.WaitForFrameLoad(10);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Z3dViewerPage.selectthumbnail(ThumbnailDescription[0], 0, ThumbnailDescription[1]);
                Boolean step1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
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

                //Step:2
                bool step21 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(15000);
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close); } catch (Exception e) { }
                string Step21_Orientation = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigation3D1);
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

                //Step:3
                IWebElement navigation_One = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step22 = verifyBasic3DToolsOperation(navigation_One, testid, ExecutedSteps, checkLine: false);
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

                //Step:4
                IWebElement ViewerPane = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step23 = Z3dViewerPage.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                new Actions(Driver).MoveToElement(ViewerPane).Perform();
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                int Beforevalue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 2, 51, 51, 50, 3);
                //Actions action = new Actions(Driver);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).Click().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 + 80, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 - 50).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().MoveToElement(ViewerPane, ViewerPane.Size.Width / 2 - 20, ViewerPane.Size.Height / 2 + 50).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().Build().Perform();
                Thread.Sleep(2000);
                int afterValue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 4, 51, 51, 50, 3);
                Thread.Sleep(4000);
                new Actions(Driver).MoveToElement(ViewerPane).Perform();
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                if (step23 && Beforevalue < afterValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Beforevalue:" + Beforevalue);
                    Logger.Instance.InfoLog("afterValue:" + afterValue);
                }

                //Step:5
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MinIp);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(6000);
                bool Step24 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MinIp);
                Logger.Instance.InfoLog(Step24.ToString());
                Thread.Sleep(6000);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(6000);
                bool Step24_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                Logger.Instance.InfoLog(Step24_1.ToString());
                if (Step24 && Step24_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:6
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                bool Step25 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                Logger.Instance.InfoLog("Step25:" + Step25.ToString());
                bool Step25_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                Logger.Instance.InfoLog("Step25_1:" + Step25_1.ToString());
                if (Step25 && Step25_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7
                //bool Step26 = false;
                IWebElement ViewPortOne = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement ViewPortTwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement ViewPortThree = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BeforeNavigation2 = Z3dViewerPage.LevelOfSelectedColor(ViewPortTwo, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeNavigation3 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree, testid, ExecutedSteps + 3, 0, 0, 0, 2);
                // Rotate the crosshair hotspots in navigationOne controls by 90 degree.
                Accord.Point RedPoint2 = Z3dViewerPage.GetIntersectionPoints(ViewPortOne, testid, ExecutedSteps + 4, "red", "Horizontal");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Performdragdrop(ViewPortOne, ViewPortOne.Size.Width / 2, ViewPortOne.Size.Height / 4, (Int32)RedPoint2.X, (Int32)RedPoint2.Y);
                //Thread.Sleep(5000);
                int AfterNavigation2 = Z3dViewerPage.LevelOfSelectedColor(ViewPortTwo, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterNavigation3 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree, testid, ExecutedSteps + 7, 0, 0, 0, 2);

                // Rotate the crosshair hotspots in navigationTwo controls by 90 degree.
                IWebElement ViewPortTwo1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Accord.Point yellowPoint = Z3dViewerPage.GetIntersectionPoints(ViewPortTwo1, testid, ExecutedSteps + 9, "yellow", "Vertical");
                Z3dViewerPage.Performdragdrop(ViewPortTwo1, (ViewPortTwo1.Size.Width) * 3 / 4, ViewPortTwo1.Size.Height / 2, (Int32)yellowPoint.X, (Int32)yellowPoint.Y);
                Thread.Sleep(5000);
                int AfterNavigation2_1 = Z3dViewerPage.LevelOfSelectedColor(ViewPortTwo1, testid, ExecutedSteps + 10, 0, 0, 0, 2);
                int AfterNavigation3_1 = Z3dViewerPage.LevelOfSelectedColor(ViewPortThree, testid, ExecutedSteps + 11, 0, 0, 0, 2);

                // Rotate the crosshair hotspots in navigationThree controls by 90 degree.
                IWebElement ViewPortThree1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point yellowPoint2 = Z3dViewerPage.GetIntersectionPoints(ViewPortThree1, testid, ExecutedSteps + 12, "yellow", "Vertical", 1);
                Z3dViewerPage.Performdragdrop(ViewPortThree1, ViewPortThree1.Size.Width / 2, (ViewPortThree1.Size.Height) * 3 / 4, (Int32)yellowPoint2.X, (Int32)yellowPoint2.Y);
                new Actions(Driver).MoveToElement(ViewPortThree1).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1500);
                String BeforeImagePath1 = Config.downloadpath + "\\Step26.png";
                Logger.Instance.InfoLog("BeforeImagepath1 is  " + BeforeImagePath1);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(ViewPortThree, BeforeImagePath1, "png");
                //Crop image. Due to missmatch annotations in the image
                String CropImagePath = result.steps[ExecutedSteps].testimagepath;
                Logger.Instance.InfoLog("CropImagePath is  " + CropImagePath);
                imager.CropAndSaveImage(BeforeImagePath1, 0, 32, ViewPortThree.Size.Width, ViewPortThree.Size.Height - 35, CropImagePath);
                if (BeforeNavigation2 != AfterNavigation2 && BeforeNavigation3 != AfterNavigation3 && AfterNavigation2 != AfterNavigation2_1 && AfterNavigation3 != AfterNavigation3_1)
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

                //Step:8
                new Actions(Driver).SendKeys("T").Build().Perform();
                IWebElement ViewPort3d1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", ViewPort3d1);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(10000);
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close); } catch (Exception e) { }
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close); } catch { }
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step27 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                string Navtopleft = ViewPort3d1.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                if (Step27 && Navtopleft.Contains("Ser: 4"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                
                }

                //Step:9
                bool step28 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(7000);
                String resetLocationValue = "Loc: 0.0, 0.0, 0.0 mm";
                String Step28Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (step28 && Step28Annotation == resetLocationValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    
                }

                //Step:10
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close); } catch { }
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step29 = verifyBasic3DToolsOperation(navigation1, testid, ExecutedSteps, checkLine: false);
                //Selection Tool
                IWebElement navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(13000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int resBeforeBlue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Thread.Sleep(2000);
                //Z3dViewerPage.MoveAndClick(navigation3D1, navigation3D1.Size.Width / 2 + 20, navigation3D1.Size.Height / 2 + 20);
                new Actions(Driver).MoveToElement(navigation3D1, navigation3D1.Size.Width / 2 + 20, navigation3D1.Size.Height / 2 + 20).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(6000);
                int resAfterblue = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 2, 0, 0, 255, 2);
                Thread.Sleep(2000);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                if (step29 && resAfterblue > resBeforeBlue)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("resAfterblue:" + resAfterblue);
                    Logger.Instance.InfoLog("resBeforeBlue:" + resBeforeBlue);
                }

                //Step:11
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Average);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                bool Step30 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Average);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                bool Step30_1 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Slab3D);
                Thread.Sleep(4000);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                bool Step30_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                Thread.Sleep(2000);
                bool Step30_3 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                if (Step30 && Step30_1 && Step30_2 && Step30_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
              
                //Step:12
                bool step32 = false;
                IWebElement Nav_3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ViewPortThree_1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", Nav_3D1);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(4000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                Thread.Sleep(10000);
                bool resStep32 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                Logger.Instance.InfoLog("resStep32 Status : "+ resStep32.ToString());
                bool resStep32_2 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Preset1, "Preset");
                bool resStep32_3 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, "Preset");
                bool resStep32_4 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MinIp);
                bool resStep32_5 = Z3dViewerPage.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Average);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String AfterImagePath2 = Config.downloadpath + "\\Step32.png";
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(ViewPortThree_1).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(3000);
                DownloadImageFile(ViewPortThree_1, AfterImagePath2, "png");
                String CropImagePath1 = result.steps[ExecutedSteps].testimagepath;
                Logger.Instance.InfoLog("CropImagePath1 Path is :" + CropImagePath1);
                imager.CropAndSaveImage(AfterImagePath2, 0, 32, ViewPortThree_1.Size.Width, ViewPortThree_1.Size.Height - 35, CropImagePath1);
                if (Z3dViewerPage.CompareImageWithDiff(result.steps[ExecutedSteps], CropImagePath, CropImagePath1) == true)
                {
                    step32 = true;
                    Logger.Instance.InfoLog("Step32 Image Comparision Passed");
                }
                if (step32 && resStep32 && resStep32_2 && resStep32_3 && resStep32_4 && resStep32_5)
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

                //Step:13
                new Actions(Driver).MoveToElement(ViewPortThree).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(1500);
                Z3dViewerPage.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(8000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                int BeforeUndo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 2, 0, 0, 0, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(8000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.UndoSculpt);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                int AfterUndo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 4, 0, 0, 0, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Redosculpt);
                Thread.Sleep(4000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                int AfterRedo = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 6, 0, 0, 0, 2);
                if (BeforeUndo > AfterUndo && AfterUndo < AfterRedo)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("BeforeUndo:" + BeforeUndo);
                    Logger.Instance.InfoLog("AfterUndo:" + AfterUndo);
                    Logger.Instance.InfoLog("AfterRedo:" + AfterRedo);

                }

                //Step:14
                //Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                IWebElement ViewPort = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                int BeforeUndo1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.UndoSegmentation);
                //Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationthree);
                //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IMenutable)));
                //Thread.Sleep(5000);
                //IWebElement undoSegButtn1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.Undo));
                //undoSegButtn1.Click();
                //IWebElement menuoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                //ClickElement(menuoptions);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(4000);
                int AfterUndo1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 4, 0, 0, 0, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.RedoSegmentation);
                //Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationthree);
                //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IMenutable)));
                //Thread.Sleep(5000);
                //IWebElement redoSegButtn1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.Redo));
                //redoSegButtn1.Click();
                //IWebElement menuoptions1 = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                //ClickElement(menuoptions1);
                //PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(4000);
                int AfterRedo1 = Z3dViewerPage.LevelOfSelectedColor(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 6, 0, 0, 0, 2);
                if (BeforeUndo1 > AfterUndo1 && AfterUndo1 < AfterRedo1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("BeforeUndo1:" + BeforeUndo1);
                    Logger.Instance.InfoLog("AfterUndo1:" + AfterUndo1);
                    Logger.Instance.InfoLog("AfterRedo1:" + AfterRedo1);
                }
                
                //Step15
                bool step35 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(6000);
                 resetLocationValue = "Loc: 0.0, 0.0, 0.0 mm";
                 Step28Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (step35 && Step28Annotation == resetLocationValue)
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_168913(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] PatientDetails = PatientID.Split('|');
            string ThumbnailDescriptions = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] ThumbnailDescription = ThumbnailDescriptions.Split('|');

            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            string subVolume2 = TestData[1];
            //string subVolume1 = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer Viewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 & 2
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: PatientDetails[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Study Date", PatientDetails[1]);
                PageLoadWait.WaitForFrameLoad(10);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step1 = Z3dViewerPage.selectthumbnail(ThumbnailDescription[0], 0, ThumbnailDescription[1]);
                Boolean step2 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                if (step1 && step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:3
                IWebElement Panel1NavOne = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement MprPathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int BeforeColour1 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int BeforeColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                viewer.SelectShowHideValue("HIDE 3D CONTROLS");
                new Actions(Driver).MoveToElement(Panel1NavOne, Panel1NavOne.Size.Width / 2, Panel1NavOne.Size.Height / 4).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1NavOne, Panel1NavOne.Size.Width / 2, Panel1NavOne.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int AfterColour1 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne, testid, ExecutedSteps + 5, 0, 0, 255, 2);
                int AfterColour2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterColour4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                if (BeforeColour1 != AfterColour1 && BeforeColour2 != AfterColour2 && BeforeColour3 != AfterColour3 && BeforeColour4 != AfterColour4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:4
                viewer.SelectShowHideValue("SHOW 3D CONTROLS");
                IWebElement navigationone1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step37 = verifyBasic3DToolsOperation(navigationone1, testid, ExecutedSteps, checkLine: false);
                if (step37)
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

                //Step:5
                Boolean step38 = false;
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.MPRPathNavigation, "1");
                Thread.Sleep(5000);
                step38 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.MPRPathNavigation).Equals("1" + " mm");
                //Verification for further steps
                int Step38Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Logger.Instance.InfoLog("Step38Nav2:"+ Step38Nav2);
                int Step38Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Logger.Instance.InfoLog("Step38Curved:"+ Step38Curved);
                String Step5Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Step5Annotation:"+ Step5Annotation);
                String Step5WLValue = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Step5WLValue:"+ Step5WLValue);
                if (step38)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:6
                IWebElement navigation_3DPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigation_3DPath);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForProgressBarToDisAppear();
                Thread.Sleep(5000);
                bool Step39 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (Step39)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7
                bool Step40 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                IWebElement Panel1NavTwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement MprPathnav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);

                int BefColourNav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColour2_1 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav1, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3_1 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int BeforeColour4_1 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath1, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                viewer.SelectShowHideValue("HIDE 3D CONTROLS");
                new Actions(Driver).MoveToElement(Panel1NavTwo, Panel1NavTwo.Size.Width / 2 - 20, Panel1NavTwo.Size.Height / 2 - 20).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool checkerrMsg = Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo, Panel1NavTwo.Size.Width / 2 + 20, Panel1NavTwo.Size.Height / 2 - 25).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool checkerrMsg2 = Z3dViewerPage.checkerrormsg(clickok: "y");
                int AftColourNav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                int AfterColour2_1 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav1, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3_1 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav1, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterColour4_1 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath1, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                viewer.SelectShowHideValue("SHOW 3D CONTROLS");
                if (AftColourNav2 > BefColourNav2 && !checkerrMsg && !checkerrMsg2 && BeforeColour2_1 != AfterColour2_1 && BeforeColour3_1 != AfterColour3_1 && BeforeColour4_1 != AfterColour4_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:8
                IWebElement navigationone_1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step41 = verifyBasic3DToolsOperation(navigationone_1, testid, ExecutedSteps, checkLine: false);
                //Verification for further steps
                int Step40Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Logger.Instance.InfoLog("Step40Nav2:"+ Step40Nav2);
                int Step40Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath1, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Logger.Instance.InfoLog("Step40Curved:" + Step40Curved);
                String Step8Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Step8Annotation:" + Step8Annotation);
                String Step8WLValue = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Step8WLValue:" + Step8WLValue);
                if (step41)
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

                //Step:9
                IWebElement navigation_3DPath1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S5", "MR", "22", navigation_3DPath1);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(20);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                Thread.Sleep(5000);
                bool Step42 = Z3dViewerPage.VerifyThumbnail_Highligted("S5", "MR", "22");
                if (Step42)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10
                bool Step43 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                IWebElement Panel1NavTwo2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement MprPathnav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);

                int BefColourNav1_2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColour2_2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3_2 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                Logger.Instance.InfoLog("BeforeColour3_2:"+ BeforeColour3_2);
                int BeforeColour4_2 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                Logger.Instance.InfoLog("BeforeColour4_2:" + BeforeColour4_2);
                viewer.SelectShowHideValue("HIDE 3D CONTROLS");
                new Actions(Driver).MoveToElement(Panel1NavTwo2, Panel1NavTwo2.Size.Width / 2 - 9, Panel1NavTwo2.Size.Height / 2 + 7).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool checkerrMsg1 = Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo2, Panel1NavTwo2.Size.Width / 2 - 5, Panel1NavTwo2.Size.Height / 2 + 20).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(20000);
                bool checkerrMsg2_1 = Z3dViewerPage.checkerrormsg(clickok: "y");
                int AftColourNav1_2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                int AfterColour2_2 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav2, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3_2 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav2, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                Logger.Instance.InfoLog("AfterColour3_2:"+ AfterColour3_2);
                int AfterColour4_2 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColour4_2:" + AfterColour4_2);
                viewer.SelectShowHideValue("SHOW 3D CONTROLS");
                if (AftColourNav1_2 > BefColourNav1_2 && BeforeColour2_2 != AfterColour2_2 && BeforeColour3_2 != AfterColour3_2 && BeforeColour4_2 != AfterColour4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("AftColourNav1_2:" + AftColourNav1_2);
                    Logger.Instance.InfoLog("BefColourNav1_2:" + BefColourNav1_2);
                    Logger.Instance.InfoLog("BeforeColour2_2:" + BeforeColour2_2);
                    Logger.Instance.InfoLog("AfterColour2_2:" + AfterColour2_2);
                }

                //Step:11
                IWebElement navigationone_2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool step44 = verifyBasic3DToolsOperation(navigationone_2, testid, ExecutedSteps, checkLine: false);
                //Verification for further steps
                int Step44Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Logger.Instance.InfoLog("Step44Nav2:"+ Step44Nav2);
                int Step44Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Logger.Instance.InfoLog("Step44Curved:" + Step44Curved);
                String Step11Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Step11Annotation:" + Step11Annotation);
                String Step11WLValue = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Step11WLValue:" + Step11WLValue);
                if (step44)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:12
                IWebElement navigation_3D = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", navigation_3D);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool Step45 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                int Step45Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step45Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                String Step12Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Step12WLValue = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);

                if (Step45 && Step40Nav2 != Step45Nav2 && Step40Curved != Step45Curved && Step12Annotation == Step8Annotation && Step12WLValue == Step8WLValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step45Nav2:" + Step45Nav2);
                    Logger.Instance.InfoLog("Step12Annotation:" + Step12Annotation);
                    Logger.Instance.InfoLog("Step12WLValue:" + Step12WLValue);
                    Logger.Instance.InfoLog("Step45Curved:" + Step45Curved);
                }

                //Step:13
                bool Step46 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                String Step13Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (Step13Annotation != Step12Annotation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step13Annotation:" + Step13Annotation);
                    
                }

                //Step:14
                IWebElement navigation3D = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S5", "MR", "22", navigation3D);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step47 = Z3dViewerPage.VerifyThumbnail_Highligted("S5", "MR", "22");
                int Step47Nav2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step47Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                String Step14Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Step14WLValue = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                if (Step47 && Step44Nav2 != Step47Nav2 && Step44Curved != Step47Curved && Step14Annotation == Step11Annotation && Step14WLValue == Step11WLValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step47Nav2:" + Step47Nav2);
                    Logger.Instance.InfoLog("Step47Curved:" + Step47Curved);
                    Logger.Instance.InfoLog("Step14Annotation:" + Step14Annotation);
                    Logger.Instance.InfoLog("Step14WLValue:" + Step14WLValue);
                }

                //Step:15
                bool Step48 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                String Step15Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (Step48 && Step15Annotation != Step14Annotation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step15Annotation:" + Step15Annotation);
                }

                //Step:16
                IWebElement navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", navigation3D2);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                bool Step49 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                int Step49Nav = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int Step49Curved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath2, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                String Step16Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Step16WLValue = Z3dViewerPage.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                if (Step49 && Step38Nav2 != Step49Nav && Step49Curved != Step38Curved && Step16Annotation == Step5Annotation && Step16WLValue == Step5WLValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Step16Annotation:" + Step16Annotation);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("Step49Nav:" + Step49Nav);
                    Logger.Instance.InfoLog("Step49Curved:" + Step49Curved);
                    Logger.Instance.InfoLog("Step16Annotation:" + Step16Annotation);
                    Logger.Instance.InfoLog("Step16WLValue:" + Step16WLValue);
                }

                //Step:17
                String resetLocationValue = "Loc: 0.0, 0.0, 0.0 mm";
                bool Step50 = Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                String Step17Annotation = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Step17Annotation:" + Step17Annotation);
                if (Step17Annotation == resetLocationValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:18
                //case update
                Z3dViewerPage.CloseViewer();
                Boolean step51 = Z3dViewerPage.searchandopenstudyin3D(PatientDetails[2], ThumbnailDescription[2], field: "Accession");
                Thread.Sleep(5000);
                bool Step51_1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(10000);
                bool errormsg = Z3dViewerPage.checkerrormsg(clickok: "y");
                if (step51 && errormsg)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:19
                IWebElement calciumscorNav = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Thread.Sleep(6000);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step52 = verifyBasic3DToolsOperation(calciumscorNav, testid, ExecutedSteps, BluRingZ3DViewerPage.CalciumScoring, checkZoomTool: false, checkPanTool: false, checkLine: false, checkRotate: false);
                if (Step52)
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

                //Step:20
                bool Step53 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesBefore = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                IWebElement Calpan2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP2 = Z3dViewerPage.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan2, (Calpan2.Size.Width - 10), (Calpan2.Size.Height / 4)).
                                    MoveToElement(Calpan2, (Calpan2.Size.Width - 10), Calpan2.Size.Height / 2).
                                    MoveToElement(Calpan2, (Calpan2.Size.Width / 2), Calpan2.Size.Height / 2).
                                    MoveToElement(Calpan2, Calpan2.Size.Width / 2, (Calpan2.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(4000);
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP2 = Z3dViewerPage.LevelOfSelectedColor(Calpan2, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesAfter = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorBeforeP2 != GreenColorAfterP2 && ScoreValuesBefore[0] < ScoreValuesAfter[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("GreenColorBeforeP2:" + GreenColorBeforeP2);
                    Logger.Instance.InfoLog("GreenColorAfterP2:" + GreenColorAfterP2);
                    Logger.Instance.InfoLog("ScoreValuesBefore[0]:" + ScoreValuesBefore[0]);
                    Logger.Instance.InfoLog("ScoreValuesAfter[0]:" + ScoreValuesAfter[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("GreenColorBeforeP2:" + GreenColorBeforeP2);
                    Logger.Instance.InfoLog("GreenColorAfterP2:" + GreenColorAfterP2);
                    Logger.Instance.InfoLog("ScoreValuesBefore[0]:" + ScoreValuesBefore[0]);
                    Logger.Instance.InfoLog("ScoreValuesAfter[0]:" + ScoreValuesAfter[0]);
                }

                //Step:21
                IWebElement navigationCalcium = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S12", "CT", "225", navigationCalcium);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(18000);
                bool CheckErrormsg = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step54 = Z3dViewerPage.VerifyThumbnail_Highligted("S12", "CT", "225");
                if (Step54 && CheckErrormsg)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:22
                IWebElement calciumscorNav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                 Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step55 = verifyBasic3DToolsOperation(calciumscorNav1, testid, ExecutedSteps, BluRingZ3DViewerPage.CalciumScoring, checkZoomTool: false, checkPanTool: false, checkLine: false, checkRotate: false);
                if (Step55)
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

                //Step:23
                bool Step56 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                Thread.Sleep(8000);
                IList<Double> ScoreValuesBefore1 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                bool Step56_1 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(8000);
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close"); }catch { }
                IWebElement Calpan3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP3 = Z3dViewerPage.LevelOfSelectedColor(Calpan3, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan3, (Calpan3.Size.Width / 2 + 306), (Calpan3.Size.Height / 2) + 78).ClickAndHold().
                                    MoveToElement(Calpan3, (Calpan3.Size.Width /2 + 336 ), (Calpan3.Size.Height / 2  + 88)).
                                    MoveToElement(Calpan3, (Calpan3.Size.Width /2 + 314), Calpan3.Size.Height / 2 + 143).
                                    MoveToElement(Calpan3, (Calpan3.Size.Width / 2 + 284), Calpan3.Size.Height / 2 + 129).
                                    MoveToElement(Calpan3, (Calpan3.Size.Width / 2+ 306), (Calpan3.Size.Height / 2) + 78).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(14000);
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP3 = Z3dViewerPage.LevelOfSelectedColor(Calpan3, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                IList<Double> ScoreValuesAfter1 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close"); } catch { }
                if (GreenColorBeforeP3 != GreenColorAfterP3 && ScoreValuesBefore1[0] < ScoreValuesAfter1[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("GreenColorBeforeP3:" + GreenColorBeforeP3);
                    Logger.Instance.InfoLog("GreenColorAfterP3:" + GreenColorAfterP3);
                    Logger.Instance.InfoLog("ScoreValuesBefore1[0]:" + ScoreValuesBefore1[0]);
                    Logger.Instance.InfoLog("ScoreValuesAfter1[0]:" + ScoreValuesAfter1[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("GreenColorBeforeP3:" + GreenColorBeforeP3);
                    Logger.Instance.InfoLog("GreenColorAfterP3:" + GreenColorAfterP3);
                    Logger.Instance.InfoLog("ScoreValuesBefore1[0]:" + ScoreValuesBefore1[0]);
                    Logger.Instance.InfoLog("ScoreValuesAfter1[0]:" + ScoreValuesAfter1[0]);
                }

                //Step:24
                IWebElement navigationCalcium1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S11", "CT", "225", navigationCalcium1);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg1 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step57 = Z3dViewerPage.VerifyThumbnail_Highligted("S11", "CT", "225");
                //Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                IList<Double> ScoreValuesAfter2 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                int GreenColorAfterP4 = Z3dViewerPage.LevelOfSelectedColor(navigationCalcium1, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorAfterP4 == GreenColorAfterP2 || ScoreValuesAfter2[0] == ScoreValuesAfter[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("GreenColorAfterP4:" + GreenColorAfterP4);
                    Logger.Instance.InfoLog("ScoreValuesAfter2[0]:" + ScoreValuesAfter[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("GreenColorAfterP4:" + GreenColorAfterP4);
                    Logger.Instance.InfoLog("ScoreValuesAfter2[0]:" + ScoreValuesAfter[0]);
                }

                //Step:25
                bool Step58 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(5000);
                IList<Double> ScoreValuesBefore3 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("ScoreValuesBefore3[0]:" + ScoreValuesBefore3[0]);
                bool Step58_1 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect this slice");
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));

                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close"); } catch { }
                IWebElement Calpan4 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP5 = Z3dViewerPage.LevelOfSelectedColor(Calpan4, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                Logger.Instance.InfoLog("GreenColorBeforeP5:" + GreenColorBeforeP5);
                new Actions(Driver).MoveToElement(Calpan4, Calpan4.Size.Width / 2, (Calpan4.Size.Height / 4)).ClickAndHold().
                                    MoveToElement(Calpan4, (Calpan4.Size.Width - 10), (Calpan4.Size.Height / 4)).
                                    MoveToElement(Calpan4, (Calpan4.Size.Width - 10), Calpan4.Size.Height / 2).
                                    MoveToElement(Calpan4, (Calpan4.Size.Width / 2), Calpan4.Size.Height / 2).
                                    MoveToElement(Calpan4, Calpan4.Size.Width / 2, (Calpan4.Size.Height / 4 + 40)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int GreenColorAfterP5 = Z3dViewerPage.LevelOfSelectedColor(Calpan4, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Logger.Instance.InfoLog("GreenColorAfterP5:" + GreenColorAfterP5);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                Thread.Sleep(5000);
                IList<Double> ScoreValuesAfter3 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("ScoreValuesAfter3[0]:" + ScoreValuesAfter3[0]);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorBeforeP5 > GreenColorAfterP5 && ScoreValuesBefore3[0] > ScoreValuesAfter3[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:26
                IWebElement navigationCalcium2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S12", "CT", "225", navigationCalcium2);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg3 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step59 = Z3dViewerPage.VerifyThumbnail_Highligted("S12", "CT", "225");
                IList<Double> ScoreValuesAfter6 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                int GreenColorAfterP6 = Z3dViewerPage.LevelOfSelectedColor(navigationCalcium1, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (GreenColorAfterP6 == GreenColorAfterP3 || ScoreValuesAfter6[0] == ScoreValuesAfter1[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("GreenColorAfterP6:" + GreenColorAfterP6);
                    Logger.Instance.InfoLog("ScoreValuesAfter6[0]:" + ScoreValuesAfter6[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("GreenColorAfterP6:" + GreenColorAfterP6);
                    Logger.Instance.InfoLog("ScoreValuesAfter6[0]:" + ScoreValuesAfter6[0]);
                }

                //Step:27
                bool Step60 = Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                Thread.Sleep(5000);
                IList<Double> ScoreValuesBefore6 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                bool Step60_1 = Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close"); } catch { }
                IWebElement Calpan5 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBeforeP7 = Z3dViewerPage.LevelOfSelectedColor(Calpan5, testid, ExecutedSteps + 10, 0, 255, 0, 2);
                new Actions(Driver).MoveToElement(Calpan5, (Calpan5.Size.Width / 2 + 306) , (Calpan5.Size.Height / 2 )+ 78).ClickAndHold().
                                    MoveToElement(Calpan5, (Calpan5.Size.Width / 2 + 336), (Calpan5.Size.Height / 2 + 88)).
                                    MoveToElement(Calpan5, (Calpan5.Size.Width / 2 + 314), Calpan5.Size.Height / 2 + 143).
                                    MoveToElement(Calpan5, (Calpan5.Size.Width / 2 + 284), Calpan5.Size.Height / 2 + 129).
                                    MoveToElement(Calpan5, (Calpan5.Size.Width / 2 + 306), (Calpan5.Size.Height / 2) + 78).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(30000);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfterP7 = Z3dViewerPage.LevelOfSelectedColor(Calpan5, testid, ExecutedSteps + 11, 0, 255, 0, 2);
                Z3dViewerPage.select3DTools(Z3DTools.Calcium_Scoring);
                Thread.Sleep(4000);
                IList<Double> ScoreValuesAfter7 = Z3dViewerPage.CalciumScoringTableValues("RCA");
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close"); } catch { }
                if (GreenColorBeforeP7 > GreenColorAfterP7 && ScoreValuesBefore6[0] > ScoreValuesAfter7[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("GreenColorBeforeP7:" + GreenColorBeforeP7);
                    Logger.Instance.InfoLog("GreenColorAfterP7:" + GreenColorAfterP7);
                    Logger.Instance.InfoLog("ScoreValuesBefore6[0]:" + ScoreValuesBefore6[0]);
                    Logger.Instance.InfoLog("ScoreValuesAfter7[0]:" + ScoreValuesAfter7[0]);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("GreenColorBeforeP7:" + GreenColorBeforeP7);
                    Logger.Instance.InfoLog("GreenColorAfterP7:" + GreenColorAfterP7);
                    Logger.Instance.InfoLog("ScoreValuesBefore6[0]:" + ScoreValuesBefore6[0]);
                    Logger.Instance.InfoLog("ScoreValuesAfter7[0]:" + ScoreValuesAfter7[0]);
                }

                //Step:28
                bool Step61_1 = false;
                IWebElement NavigationCalcium = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S11", "CT", "225", NavigationCalcium);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg4 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step61 = Z3dViewerPage.VerifyThumbnail_Highligted("S11", "CT", "225");
                IList<IWebElement> SelectionOptions = Driver.FindElements(By.XPath(Locators.Xpath.RadioContainer));
                foreach (IWebElement radio in SelectionOptions)
                {
                    IWebElement radio1 = radio.FindElement(By.XPath(Locators.Xpath.RadioLabelContent));
                    if (radio.Text.Equals("Deselect this slice"))
                    {
                        
                        if (radio.GetAttribute("class").Contains("mat-radio-checked"))
                        {
                            Step61_1 = true;
                            Logger.Instance.InfoLog("Deselect this slice option is checked");
                        }
                    }
                }
                if (Step61 & Step61_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:29
                bool Step62_1 = false;
                IWebElement navigationCalcium3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.DragandDropThumbnail("S12", "CT", "225", navigationCalcium3);
                wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(8000);
                bool CheckErrormsg5 = Z3dViewerPage.checkerrormsg(clickok: "y");
                PageLoadWait.WaitForFrameLoad(20);
                bool Step62 = Z3dViewerPage.VerifyThumbnail_Highligted("S12", "CT", "225");
                IList<IWebElement> SelectionOptions1 = Driver.FindElements(By.XPath(Locators.Xpath.RadioContainer));
                foreach (IWebElement radio in SelectionOptions1)
                {
                    IWebElement radio1 = radio.FindElement(By.XPath(Locators.Xpath.RadioLabelContent));
                    if (radio.Text.Equals("Deselect all contiguous"))
                    {

                        if (radio.GetAttribute("class").Contains("mat-radio-checked"))
                        {
                            Step62_1 = true;
                            Logger.Instance.InfoLog("Deselect all contiguous button is enabled");
                            break;
                        }
                    }
                }
                if (Step62 & Step62_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:30
                bool Step63 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                IList<IWebElement> viewmode = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewmodeDropdownText));
                string Viewmodetext = viewmode[0].GetAttribute("innerText");
                if (Viewmodetext.Equals(BluRingZ3DViewerPage.Two_2D))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:31
                try { Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close"); } catch { }
                IWebElement TwoDFirstviewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelFirstViewport));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String BeforeImagePath5 = Config.downloadpath + "\\Step31Before.png";
                PageLoadWait.WaitForFrameLoad(25);
                DownloadImageFile(TwoDFirstviewport, BeforeImagePath5, "png");
                Thread.Sleep(3000);
                var step64 = viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                viewer.ApplyTool_WindowWidth();
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                String AfterImagePath5 = Config.downloadpath + "\\Step31After.png";
                PageLoadWait.WaitForFrameLoad(25);
                DownloadImageFile(TwoDFirstviewport, AfterImagePath5, "png");
                if (CompareImage(BeforeImagePath5, AfterImagePath5) == false)
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

                //Step:32
                Z3dViewerPage.CloseViewer();
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                SearchStudy(patientID: PatientDetails[0]);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy("Study Date", PatientDetails[1]);
                PageLoadWait.WaitForFrameLoad(10);
                var viewer1 = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean Step32 = Z3dViewerPage.selectthumbnail(ThumbnailDescription[0], 0, ThumbnailDescription[1]);
                bool Step32_1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(6000);
                IWebElement Navigationcalc = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", Navigationcalc);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(6000);
                bool resStep32 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                if (Step32 && Step32_1 && resStep32)
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:33
                IWebElement Panel1NavOne4 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement MprPathnav4 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDpathnav4 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement CurvedPath4 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int BeforeColourNavOne = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne4, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColourMPR = Z3dViewerPage.LevelOfSelectedColor(MprPathnav4, testid, ExecutedSteps + 2, 0, 0, 0, 2);
                int BeforeColour3D = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav4, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                int BeforeColourCurved = Z3dViewerPage.LevelOfSelectedColor(CurvedPath4, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavOne4).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).MoveToElement(Panel1NavOne4, Panel1NavOne4.Size.Width / 2, Panel1NavOne4.Size.Height / 4 +20).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1NavOne4, Panel1NavOne4.Size.Width / 2, Panel1NavOne4.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int AfterColourNavOne1 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne4, testid, ExecutedSteps + 5, 0, 0, 255, 2);
                int AfterColourMPR1 = Z3dViewerPage.LevelOfSelectedColor(MprPathnav4, testid, ExecutedSteps + 6, 0, 0, 0, 2);
                int AfterColour3D1 = Z3dViewerPage.LevelOfSelectedColor(ThreeDpathnav4, testid, ExecutedSteps + 7, 133, 133, 131, 2);
                int AfterColourCurved1 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath4, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Thread.Sleep(1500);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", Panel1NavOne4);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(10000);
                bool resStep33 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                if (resStep33 && BeforeColourNavOne != AfterColourNavOne1 && BeforeColourMPR != AfterColourMPR1 && BeforeColour3D != AfterColour3D1 && BeforeColourCurved != AfterColourCurved1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:34
                IWebElement Panel1NavOne5 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation);
                IWebElement CurvedPath5 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int BeforeColourNavOne2 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne5, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColourCurved2 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath5, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                Thread.Sleep(1500);
                new Actions(Driver).MoveToElement(Panel1NavOne5, Panel1NavOne5.Size.Width / 2 -20, Panel1NavOne5.Size.Height / 2 - 20).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1NavOne5, Panel1NavOne5.Size.Width / 2 + 20, Panel1NavOne5.Size.Height / 2 - 25).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(1500);
                int AfterColourNavOne3 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne5, testid, ExecutedSteps + 5, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne3:" + AfterColourNavOne3);
                int AfterColourCurved3 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath5, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", Panel1NavOne5);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(10000);
                bool resStep34 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                IWebElement Panel1NavOne6 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int AfterColourNavOne4 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne6, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne4:" + AfterColourNavOne4);
                Boolean resStep34_1 = Z3dViewerPage.VerifyCursorMode(BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CurvedToolManualCursor);
                Logger.Instance.InfoLog("resStep34_1:"+ resStep34_1.ToString());
                if (resStep34 && resStep34_1 && BeforeColourNavOne2 != AfterColourNavOne3 && BeforeColourCurved2 != AfterColourCurved3 && AfterColourNavOne4 != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:35
                bool Step35 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                IWebElement Panel1NavOne7 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Panel1NavTwo7 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement CurvedPath7 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int AfterColourNavOne5 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne7, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne5:"+ AfterColourNavOne5);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int BeforeColourNavOne4 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo7, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColourCurved4 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath7, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                Thread.Sleep(1500);
                new Actions(Driver).MoveToElement(Panel1NavTwo7, Panel1NavTwo7.Size.Width / 2 + 20, Panel1NavTwo7.Size.Height / 2 +30 ).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo7, Panel1NavTwo7.Size.Width / 2 + 30, Panel1NavTwo7.Size.Height / 2 + 60).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                int AfterColourNavOne6 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo7, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne6:"+ AfterColourNavOne6);
                int AfterColourCurved6 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath7, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Thread.Sleep(1500);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", Panel1NavOne7);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(10000);
                bool resStep35 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                IWebElement Panel1NavOne8 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int AfterColourNavOne7 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne8, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne7:"+ AfterColourNavOne7);
                Boolean resStep35_1 = Z3dViewerPage.VerifyCursorMode(BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CurvedToolVesselsCursor);
                Logger.Instance.InfoLog("resStep35_1:" + resStep35_1.ToString());
                if (AfterColourNavOne5 < AfterColourNavOne4 && BeforeColourNavOne4 != AfterColourNavOne6 && BeforeColourCurved4 != AfterColourCurved6 && resStep35 && resStep35_1 && AfterColourNavOne7 < AfterColourNavOne3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:36
                IWebElement Panel1NavTwo8 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement CurvedPath8 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int BeforeColourNavOne5 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo8, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColourCurved5 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath8, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                Thread.Sleep(1500);
                new Actions(Driver).MoveToElement(Panel1NavTwo8, Panel1NavTwo8.Size.Width / 2 - 20, Panel1NavTwo8.Size.Height / 2 - 20).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo8, Panel1NavTwo8.Size.Width / 2 + 20, Panel1NavTwo8.Size.Height / 2 - 25).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                int AfterColourNavOne8 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo8, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne8:"+ AfterColourNavOne8);
                int AfterColourCurved8 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath8, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Z3dViewerPage.DragandDropThumbnail("S3", "MR", "150", Panel1NavTwo8);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(10000);
                bool resStep36 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                IWebElement Panel1NavOne9 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int AfterColourNavOne9 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne9, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne9:" + AfterColourNavOne9);
                Boolean resStep36_1 = Z3dViewerPage.VerifyCursorMode(BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CurvedToolVesselsCursor);
                Logger.Instance.InfoLog("resStep36_1:" + resStep36_1.ToString());
                if (BeforeColourNavOne5 != AfterColourNavOne8 && BeforeColourCurved5 != AfterColourCurved8 && AfterColourNavOne9 != 0 && resStep36_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:37
                bool Step37 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                IWebElement Panel1NavOne10 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Panel1NavTwo10 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement CurvedPath10 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int AfterColourNavOne10 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne10, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne5:" + AfterColourNavOne5);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int BeforeColourNavOne10 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo10, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColourCurved10 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath10, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavTwo10, Panel1NavTwo10.Size.Width / 2 + 21, Panel1NavTwo10.Size.Height / 2 - 4).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo10, Panel1NavTwo10.Size.Width / 2 + 30, Panel1NavTwo10.Size.Height / 2 + 9).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                int AfterColourNavOne11 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo10, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne11:" + AfterColourNavOne11);
                int AfterColourCurved11 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath10, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                Thread.Sleep(1500);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", Panel1NavTwo10);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(10000);
                bool resStep37 = Z3dViewerPage.VerifyThumbnail_Highligted("S4", "MR", "23");
                IWebElement Panel1NavOne11 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int AfterColourNavOne12 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne11, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne12:" + AfterColourNavOne12);
                Boolean resStep37_1 = Z3dViewerPage.VerifyCursorMode(BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CurvedToolColonCursor);
                Logger.Instance.InfoLog("resStep37_1:" + resStep37_1.ToString());
                if (AfterColourNavOne10 < AfterColourNavOne6 && BeforeColourNavOne10 != AfterColourNavOne11 && BeforeColourCurved10 != AfterColourCurved11 && resStep37 && resStep37_1 && AfterColourNavOne12 < AfterColourNavOne8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:38
                IWebElement Panel1NavOne12 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Panel1NavTwo12 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement CurvedPath12 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int AfterColourNavOne13 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne10, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne13:" + AfterColourNavOne13);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                int BeforeColourNavOne12 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo12, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                int BeforeColourCurved12 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath12, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavTwo12, Panel1NavTwo12.Size.Width / 2 - 23, Panel1NavTwo12.Size.Height / 2 + 25).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                new Actions(Driver).MoveToElement(Panel1NavTwo12, Panel1NavTwo12.Size.Width / 2 - 10, Panel1NavTwo12.Size.Height / 2 + 60).Build().Perform();
                Thread.Sleep(1500);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Z3dViewerPage.checkerrormsg(clickok: "y");
                int AfterColourNavOne14 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavTwo12, testid, ExecutedSteps + 9, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne14:" + AfterColourNavOne14);
                int AfterColourCurved14 = Z3dViewerPage.LevelOfSelectedColor(CurvedPath12, testid, ExecutedSteps + 8, 0, 0, 255, 2);
                new Actions(Driver).MoveToElement(Panel1NavTwo12).Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1500);
                Thread.Sleep(1500);
                Z3dViewerPage.DragandDropThumbnail("S4", "MR", "23", Panel1NavOne12);
                PageLoadWait.WaitForFrameLoad(25);
                Thread.Sleep(10000);
                bool resStep38 = Z3dViewerPage.VerifyThumbnail_Highligted("S3", "MR", "150");
                IWebElement Panel1NavOne14 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int AfterColourNavOne15 = Z3dViewerPage.LevelOfSelectedColor(Panel1NavOne14, testid, ExecutedSteps + 10, 0, 0, 255, 2);
                Logger.Instance.InfoLog("AfterColourNavOne15:"+ AfterColourNavOne15);
                Boolean resStep38_1 = Z3dViewerPage.VerifyCursorMode(BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CurvedToolColonCursor);
                Logger.Instance.InfoLog("resStep38_1:" + resStep38_1.ToString());
                if (AfterColourNavOne10 < AfterColourNavOne6 && BeforeColourNavOne12 != AfterColourNavOne14 && BeforeColourCurved12 != AfterColourCurved14 && resStep38 && resStep38_1 && AfterColourNavOne15 !=0)
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }


        public TestCaseResult Test_175116(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            string subVolume2 = TestData[1];
            string subVolume3 = TestData[2];
            string subVolume4 = TestData[3];
            string subVolume5 = TestData[4];


            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                // Verify all smart views are available in the smart view dropdown
                Boolean step1_1 = Z3dViewerPage.verify3dlayoutMenuList();
                if (step1 & step1_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:2
                bool step2 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                PageLoadWait.WaitForFrameLoad(10);
                bool step2_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel });
                if (step2_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:3
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
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

                //Step:4
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                PageLoadWait.WaitForFrameLoad(10);
                bool step4_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.ResultPanel });
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

                //Step:5
                IWebElement wb_5 = Driver.FindElement(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (CompareImage(result.steps[ExecutedSteps], wb_5)) 
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
                
                //step:6
                String TotalImage = Z3dViewerPage.GetSubVolumeImageCount(BluRingZ3DViewerPage.Navigation3D1, "Sub Volumes");
                if (TotalImage.Contains("17. 105 images"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:7
                bool step7 = Z3dViewerPage.VerifySubVolumeLoadinBar(BluRingZ3DViewerPage.Navigationfour, subVolume1);
                if (step7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step:8
                IWebElement wb_8 = Driver.FindElement(By.CssSelector(Locators.CssSelector.bluringstudypanel));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (CompareImage(result.steps[ExecutedSteps], wb_8))
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
                //step:9
                bool  Curve_draw_tool_enabled= Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (!Curve_draw_tool_enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("CurvedDrawing tool is disabled");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_175116 Step 09 because CurvedDrawing tool is not disabled");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10
                bool CalciumScoring_enabled = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (!CalciumScoring_enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("calciumScoring tool is disabled");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_175116 Step 10 because calciumScoring tool is not disabled");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step11
                IWebElement Panel1Nav1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                String ScrollBefore = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                //Z3dViewerPage.Performdragdrop(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, (Panel1Nav1.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Panel1Nav1, Panel1Nav1.Size.Width / 2, Panel1Nav1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String ScrollAfter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if(ScrollBefore!= ScrollAfter)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("calciumScoring tool is disabled");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_175116 Step 10 because calciumScoring tool is not disabled");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12
                Z3dViewerPage.select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ThreshholdValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                IWebElement Radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                Z3dViewerPage.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.LargeVessels);


                Double Step6Volume = Z3dViewerPage.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement ViewerContainer7 = Z3dViewerPage.ViewerContainer();
                //if (Config.BrowserType.ToLower() == "chrome")
                //{
                //    Z3dViewerPage.Performdragdrop(Navigation3D1, Navigation3D1.Size.Width / 2 + 20, 30, Navigation3D1.Size.Width / 2 + 20, 20);
                //    Z3dViewerPage.Performdragdrop(Navigation3D1, Navigation3D1.Size.Width / 2 + 20, 50, resultcontrol.Size.Width / 2 + 20, 40);
                //    Z3dViewerPage.Performdragdrop(Navigation3D1, Navigation3D1.Size.Width / 2 + 20, 70, resultcontrol.Size.Width / 2 + 20, 60);
                //    Z3dViewerPage.Performdragdrop(Navigation3D1, Navigation3D1.Size.Width / 2 + 20, 90, resultcontrol.Size.Width / 2 + 20, 80);
                //}
                //else
                //{
                //    this.Cursor = new Cursor(Cursor.Current.Handle);
                //    Cursor.Position = new Point((ViewerContainer7.Location.X + 700), (ViewerContainer7.Location.Y / 2 + 650));
                //    //  new Actions(Driver).DragAndDropToOffset(resultcontrol, resultcontrol.Size.Width / 2 + 5, resultcontrol.Size.Width / 2 + 10);
                //    //  Z3dViewerPage.Performdragdrop(resultcontrol, resultcontrol.Size.Width / 2 + 5, 10, resultcontrol.Size.Height /2 , 5);
                //    new Actions(Driver).MoveToElement(resultcontrol, (resultcontrol.Size.Width / 2) + 18, (resultcontrol.Size.Height / 4) - 40).ClickAndHold().
                //   MoveToElement(resultcontrol, (resultcontrol.Size.Width / 2) + 18, (resultcontrol.Size.Height / 4 - 30)).Release().Build().Perform();
                //    PageLoadWait.WaitForFrameLoad(10);
                //    Thread.Sleep(500);
                //    new Actions(Driver).MoveToElement(resultcontrol, (resultcontrol.Size.Width / 2) + 18, (resultcontrol.Size.Height / 4 - 30)).Click().Build().Perform();
                //}
                //wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));

                //Double Step7Volume = Z3dViewerPage.GetSelectionVolume();
                //if (Step6Volume != Step7Volume)
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


                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }


        public TestCaseResult Test_175115(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            String[] TestData = TestDataRequirements.Split('|');
            string subVolume1 = TestData[0];
            

            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 Load 3D supported series and click on smart view dropdown

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                // Verify all smart views are available in the smart view dropdown
                Boolean step1_1 = Z3dViewerPage.verify3dlayoutMenuList();
                if (step1 & step1_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:2
                bool step2 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                PageLoadWait.WaitForFrameLoad(10);
                bool step2_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel });
                if (step2_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step:3
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                PageLoadWait.WaitForFrameLoad(10);
                bool step3_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
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

                //Step:4
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(10);
                bool step4_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.ResultPanel });
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

                //Step:5
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(10);
                bool step5_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<String> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                if (step5_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:6
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.checkerrormsg("y");
                PageLoadWait.WaitForFrameLoad(10);
                bool step6_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.CalciumScoring });
                if (step6_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step:7
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Study_Panel = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyPanel));
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Study_Panel, removeCurserFromPage: true))
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

                //step:8
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                Z3dViewerPage.checkerrormsg("y");
                PageLoadWait.WaitForFrameLoad(10);
                bool step8_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.CalciumScoring });
                if (step8_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:9
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(10);
                bool step9_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<String> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                if (step9_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:10
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(10);
                bool step10_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.ResultPanel });
                if (step10_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:11
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                PageLoadWait.WaitForFrameLoad(10);
                bool step11_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
                if (step11_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step:12
                bool step12 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR);
                PageLoadWait.WaitForFrameLoad(10);
                bool step12_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel });
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

                                
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_175114(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionList");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            String[] Acccession = AccessionList.Split('|');
            string Accession1 = Acccession[0];

            String[] TestData = PatientID.Split('|');
            string Data1 = TestData[0];
            string Data2 = TestData[1];


            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 Load 3D supported series and click on smart view dropdown

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(Data1, ThumbnailDescription);
                // Verify all smart views are available in the smart view dropdown
                Boolean step1_1 = Z3dViewerPage.verify3dlayoutMenuList();
                if (step1 & step1_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step:2
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                IWebElement FirstpanelNav2 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelSecondViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //bool Two2D = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                //IWebElement Smart_DropDown = Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                IWebElement Smart_DropDown = Driver.FindElement(By.XPath(Locators.Xpath.SmartBox_Visibility));
                if (Smart_DropDown.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                Z3dViewerPage.CloseViewer();

                //Step:3
                Z3dViewerPage.searchandopenstudyin3D(Accession1,ThumbnailDescription,field:"acc");
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                FirstpanelNav2 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelForthViewport));
                new Actions(Driver).MoveToElement(FirstpanelNav2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //bool Two2D = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                Smart_DropDown = Driver.FindElement(By.XPath(Locators.Xpath.SmartBox_Visibility));
                if (Smart_DropDown.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step:4
                //FirstpanelNav2 = Driver.FindElement(By.CssSelector(Locators.CssSelector.FirstPanelForthViewport));
                //new Actions(Driver).MoveToElement(FirstpanelNav2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.DragandDropThumbnail("S4", "CT", "99", FirstpanelNav2);
                //bool Two2D = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                //Smart_DropDown=Driver.FindElement(By.XPath(Locators.Xpath.SmartBox_Visibility));
                IWebElement SmartDropDown = Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                if (SmartDropDown.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:5
                Boolean step5_1 = Z3dViewerPage.verify3dlayoutMenuList();
                if (step5_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step:6
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(10);
                bool step6_1 = Z3dViewerPage.verifyControlElementsAvailability(new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.ResultPanel });
                if (step6_1)
                {
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
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

    }
}

