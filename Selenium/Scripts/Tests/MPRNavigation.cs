using System;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using System.IO;
using System.Windows.Forms;
using OpenQA.Selenium;
using System.Threading;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Selenium.Scripts.Tests
{
    class MPRNavigation : BasePage
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);


        public Login login { get; set; }
        public string filepath { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public object MouseSimulator { get; private set; }
        public BluRingZ3DViewerPage brz3dvp { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public Cursor Cursor { get; private set; }
        public MPRNavigation(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }
        public TestCaseResult Test_163334(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string CursonName = TestData[0];
            string ResetValue = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
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
                
                //step:2  -Zoom cursor shows up while hovering over the image.
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                String step3 = Viewport[1].GetCssValue("cursor");
                Thread.Sleep(3000);
                if (step3.Contains(CursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3  -Pixel that was initially clicked moves to the center of the control and the image magnification increases.
                //String step3_1Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 75, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 50, RemoveCross: true);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_1After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_2Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) - 75, (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) + 50, RemoveCross: true);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_2After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_3Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) - 75, (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) + 50, RemoveCross: true);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_3After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //PageLoadWait.WaitForFrameLoad(5);

                //For Navigation one 
                List<string> result3_1before = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 75, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 50, RemoveCross: true);
                List<string> result3_1after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                PageLoadWait.WaitForFrameLoad(5);

                //for second navigation 
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) - 75, (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) + 50, RemoveCross: true);
                List<string> result3_2after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                PageLoadWait.WaitForFrameLoad(5);

                //for third navigaiton 
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) - 75, (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) + 50, RemoveCross: true);
                List<string> result3_3after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                PageLoadWait.WaitForFrameLoad(5);
                
                if (result3_1before[0]!= result3_1after[0] && result3_1before[1] != result3_1after[1] && result3_1before[2] != result3_1after[2] && result3_1after[0]!= result3_2after[0] && result3_1after[1] != result3_2after[1] &&
                    result3_1after[2] != result3_2after[2] && result3_2after[0]!= result3_3after[0] && result3_2after[1] != result3_3after[1] && result3_2after[1] != result3_3after[1] )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                

                //step:4- Pixel that was initially clicked moves to the center of the control and the image magnification decreases.
                //String step4_1Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 100, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 50, RemoveCross: true);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step4_1After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step4_2Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) + 100, (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) - 50, RemoveCross: true);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step4_2After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step4_3Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) + 100, (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) - 50, RemoveCross: true);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step4_3After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ////  if (step4_1Before != step4_1After)
                List<string> result4_1before  = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 100, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 50, RemoveCross: true);
                List<string> result4_1after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                
                //for Second Navigation 
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) + 100, (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) - 50, RemoveCross: true);
                List<string> result4_2after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                
                //For Third Navigation 
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) + 100, (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) - 50, RemoveCross: true);
                List<string> result4_3after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
               
                
                if (result4_1before[0]!= result4_1after[0] && result4_1before[1] != result4_1after[1] && result4_1before[2] != result4_1after[2] && result4_1after[0] != result4_2after[0]
                    && result4_1after[1] != result4_2after[1] && result4_1after[2] != result4_2after[2] && result4_1after[0] != result4_3after[0] && result4_1after[1] != result4_3after[1] && result4_1after[2] != result4_3after[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Click on the Reset button from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step5 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step5[0] == ResetValue && step5[1] == ResetValue && step5[2] == ResetValue && step5[3] == ResetValue)
                {
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
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
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

        public TestCaseResult Test_163335(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String TestRequirments = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] testData = TestRequirments.Split('|');
            String IncrementValue = testData[0];
            string CursorName = testData[1];
            string ResetValue = testData[2];
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;

            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            Z3dViewerPage.Deletefiles(testcasefolder);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
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
                
                //step:2  -Set the thickness in MPR navigation control 1 to 0.1.
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationone, IncrementValue);
                PageLoadWait.WaitForFrameLoad(2);
                IList<string> iresult3= Z3dViewerPage.GetControlvalues(BluRingZ3DViewerPage.Thickness, IncrementValue + " mm");
                if (iresult3[0].Substring(0, 3) == IncrementValue && iresult3[1].Substring(0, 3) == IncrementValue && iresult3[2].Substring(0, 3) == IncrementValue && iresult3[3].Substring(0, 3) != IncrementValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step:3 - Push the "print screen" key to capture the screen and paste into MS Paint.
                string filename = "PrintBt_163335_" + new Random().Next(1000) + ".jpg";
                Thread.Sleep(10000);
                Z3dViewerPage.CaptureScreen(filename, testid);
                Thread.Sleep(10000);
                if (File.Exists(testcasefolder + filename))
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Rotate cursor is displayed when the user hovers over the rotate hotspot.
                
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                {
                    IWebElement inavigaitonone = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                    Thread.Sleep(1000);
                    bool bflag4= Z3dViewerPage.MoveMouseCursorOnRotateHotspot(BluRingZ3DViewerPage.Navigationone);
                }
                else
                    Cursor.Position = new Point((Viewport[0].Size.Width) - 16, (Viewport[0].Size.Height) + 75);
                Thread.Sleep(2000);
                String step5 = Viewport[0].GetCssValue("cursor");Thread.Sleep(4000);
                Logger.Instance.InfoLog("Test_163335 Step_4 Cursor value " + step5);
                if (step5.Contains(CursorName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - 1. Position the mouse to Navigation Control 1 2.Left click and drag the mouse in a semi-circle to rotate the crosshairs by 180 degrees clockwise
                Cursor.Position = new Point((Viewport[0].Size.Width) + 100, (Viewport[0].Size.Height));
                int startx = 0; int starty = 0; int endx = 0; int endy = 0;
                startx = (Viewport[0].Size.Width / 2) - 116;
                starty = Viewport[0].Size.Height / 2;
                endx = (Viewport[0].Size.Width / 2) + 116;
                endy = (Viewport[0].Size.Height / 2);
                Z3dViewerPage.Performdragdrop(Viewport[0], endx, endy, startx, starty);
                Thread.Sleep(7500);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:6  -Press the reset button from 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                //String step6_1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //String step6_2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //String step6_3 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //String step6_4 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                //if (step6_1.Equals(ResetValue) && step6_2.Equals(ResetValue) && step6_3.Equals(ResetValue) && step6_4.Equals(ResetValue))
                   List<string> result6 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result6[0] == result6[1] && result6[1] == result6[2] && ResetValue == (result6[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:7 - 1. Position the mouse to Navigation Control 2 2.Left click and drag the mouse in a semi-circle to rotate the crosshairs by 180 degrees clockwise
                Cursor.Position = new Point((Viewport[1].Size.Width) + 100, (Viewport[1].Size.Height));
                startx = 0; starty = 0; endx = 0; endy = 0;
                startx = (Viewport[1].Size.Width / 2) - 116;
                starty = Viewport[1].Size.Height / 2;
                endx = (Viewport[1].Size.Width / 2) + 116;
                endy = (Viewport[1].Size.Height / 2);
                Z3dViewerPage.Performdragdrop(Viewport[1], endx, endy, startx, starty);
                Thread.Sleep(7500);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:8  -Press the reset button from 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                //String step8_1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //String step8_2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //String step8_3 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //String step8_4 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                //if (step8_1.Equals(ResetValue) && step8_2.Equals(ResetValue) && step8_3.Equals(ResetValue) && step8_4.Equals(ResetValue))
                List<string> result8 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result8[0] == result8[1] && result8[1] == result8[2] && ResetValue == (result8[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:9 - 1. Position the mouse to Navigation Control 3 2.Left click and drag the mouse in a semi-circle to rotate the crosshairs by 180 degrees clockwise
                Cursor.Position = new Point((Viewport[2].Size.Width) + 100, (Viewport[2].Size.Height));
                startx = 0; starty = 0; endx = 0; endy = 0;
                startx = (Viewport[2].Size.Width / 2) - 116;
                starty = Viewport[2].Size.Height / 2;
                endx = (Viewport[2].Size.Width / 2) + 116;
                endy = (Viewport[2].Size.Height / 2);
                Z3dViewerPage.Performdragdrop(Viewport[2], endx, endy, startx, starty);
                Thread.Sleep(7500);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:10  -Press the reset button from 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                //   List<string> step10 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                List<string> result10 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result10[0] == result10[1] && result10[1] == result10[2] && ResetValue == (result10[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step:11 - Select the rotate tool from the 3D viewport.
                Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                Thread.Sleep(7500);
                String step12 = Viewport[0].GetCssValue("cursor");
                if (step12.Contains(CursorName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12 - Left click at the bottom of the image displayed on MPR navigation control 1 and do a free rotation by dragging up until the image is rotated upside down.
                Cursor.Position = new Point((Viewport[0].Size.Width), (Viewport[0].Size.Height) + 100);
                startx = 0; starty = 0; endx = 0; endy = 0;
                startx = (Viewport[0].Size.Width / 2);
                starty = (Viewport[0].Size.Height / 2) + 116;
                endx = (Viewport[0].Size.Width / 2);
                endy = (Viewport[0].Size.Height / 2) - 116;
                Z3dViewerPage.Performdragdrop(Viewport[0], endx, endy, startx, starty);
                Thread.Sleep(7500);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:13  -Press the reset button from 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                
                List<string> result13 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result13[0] == result13[1] && result13[1] == result13[2] && ResetValue == (result13[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - 1. Position the mouse to Navigation Control 3 2.Left click and drag the mouse in a semi-circle to rotate the crosshairs by 180 degrees clockwise
                Cursor.Position = new Point((Viewport[1].Size.Width) + 100, (Viewport[1].Size.Height));
                startx = 0; starty = 0; endx = 0; endy = 0;
                startx = (Viewport[1].Size.Width / 2);
                starty = (Viewport[1].Size.Height / 2) + 116;
                endx = (Viewport[1].Size.Width / 2);
                endy = (Viewport[1].Size.Height / 2) - 116;
                Z3dViewerPage.Performdragdrop(Viewport[1], endx, endy, startx, starty);
                Thread.Sleep(7500);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:15  -Press the reset button from 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step15 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step15[0] == ResetValue && step15[1] == ResetValue && step15[2] == ResetValue && step15[3] == ResetValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:16 - 1. Position the mouse to Navigation Control 3 2.Left click and drag the mouse in a semi-circle to rotate the crosshairs by 180 degrees clockwise
                Cursor.Position = new Point((Viewport[2].Size.Width) + 100, (Viewport[2].Size.Height));
                startx = 0; starty = 0; endx = 0; endy = 0;
                startx = (Viewport[2].Size.Width / 2);
                starty = (Viewport[2].Size.Height / 2) + 116;
                endx = (Viewport[2].Size.Width / 2);
                endy = (Viewport[2].Size.Height / 2) - 116;
                Z3dViewerPage.Performdragdrop(Viewport[1], endx, endy, startx, starty);
                Thread.Sleep(7500);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:17  -Press the reset button from 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step17 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step17[0] == ResetValue && step17[1] == ResetValue && step17[2] == ResetValue && step17[3] == ResetValue)
                {
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
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
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

        public TestCaseResult Test_163336(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string CursonName = TestData[0];
            string ResetValue = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step2 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
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

                //step:2  -Zoom cursor shows up while hovering over the image.
                Z3dViewerPage.select3DTools(Z3DTools.Pan);
                Thread.Sleep(1500);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                Cursor.Position = new Point((ViewerContainer.Size.Width) / 2, (ViewerContainer.Size.Height) / 3);
                Thread.Sleep(1500);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                String step3 = Viewport[1].GetCssValue("cursor");
                if (step3.Contains(CursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3  -Pixel that was initially clicked moves to the center of the control and the image magnification increases.
                //String step3_1Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 75, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 50);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_1After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_2Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) - 75, (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) + 50);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_2After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                //Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_3Before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                //Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) - 75, (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) + 50);
                //PageLoadWait.WaitForFrameLoad(5);
                //String step3_3After = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
//                if (step3_1Before != step3_1After && step3_2Before != step3_2After && step3_3Before != step3_3After)
                    //for Navigation one 
                 List<string> result3_1before = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 75, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 50);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> result3_1after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) - 75, (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) + 50);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> result3_2after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) - 75, (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) + 50);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> result3_3after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result3_1before[0] != result3_1after[0] && result3_1before[1] != result3_1after[1] && result3_1before[1] != result3_1after[1] && result3_1after[0]!= result3_2after[0]
                    && result3_1after[1] != result3_2after[1] && result3_1after[3] != result3_2after[3] && result3_2after[0]!= result3_3after[0] && result3_2after[1] != result3_3after[1] && result3_2after[2] != result3_3after[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Click on the Reset button from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step5 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step5[0] == ResetValue && step5[1] == ResetValue && step5[2] == ResetValue && step5[3] == ResetValue)
                {
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
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
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

        public TestCaseResult Test_163337(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;

            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] testdata = Requirements.Split('|');
            string IncrementValue = testdata[0];
            String Navigation = testdata[1];
            String ResetValue = testdata[2];

            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - From iCA , Navigate to 3D tab and Click MPR mode from the dropdown
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (step1 && Viewport.Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:2 - Click on the MPR navigation control 1 and position the intersection of the red and blue crosshairs at the top of the image displayed.
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) - 126);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:3 - On the MPR Result control, select "Navigation 3" to be the source control from the drop down list
                Z3dViewerPage.SelectNavigation(Navigation);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                //Viewport[2].Click();
                BasePage.mouse_event(0x0800, 0, 0, 100, 0);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:4 - Scroll through the whole volume until the intersection of the red and blue crosshairs are at the bottom of the image displayed on the MPR navigation control 1
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 130);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:5 - Press the reset button from 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                //    String step5 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //if (step5.Equals(ResetValue))
                    List<string> result5 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result5[0] == result5[1] && result5[1] == result5[2] && ResetValue == (result5[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:6 - Scroll tool cursor shows up while hovering over the images
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                String step6 = Viewport[1].GetCssValue("cursor");
                if (step6.Contains(BluRingZ3DViewerPage.ScrollingCursor))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step: 7 -In Navigation 1 image, click the left mouse button and drag the mouse downwards
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 75, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 50);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);

                //step:8  -In Navigation 2 image, click the left mouse button and drag the mouse upwards
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) - 75, (Viewport[1].Size.Width / 2) - 50, (Viewport[1].Size.Height / 2) + 50);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer,pixelTolerance:400))
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
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);

                //step:9  -In Navigation 3 image, click the left mouse button and drag the mouse upwards
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) - 75, (Viewport[2].Size.Width / 2) - 50, (Viewport[2].Size.Height / 2) + 50);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:10 - Click on the reset button
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step10 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step10[0] == ResetValue && step10[1] == ResetValue && step10[2] == ResetValue && step10[3] == ResetValue)
                {
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
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
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

        public TestCaseResult Test_163348(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String goldimage1 = null, goldimage2 = null, goldimage3 = null, compareimage1 = null, compareimage2 = null, compareimage3 = null;

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 02
                res = brz3dvp.CheckCrossHairinNavigations(testid, ExecutedSteps + 2, toggle: 1);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163348 Step 02");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 03
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.CheckCrossHairinNavigations(testid, ExecutedSteps + 1);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163348 Step 03");
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[ExecutedSteps].status = "Pass";

                //step 04
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.CheckCrossHairinNavigations( testid, ExecutedSteps + 1, 1, 1);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163348 Step 04");
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[ExecutedSteps].status = "Pass";

                //step 05
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.CheckCrossHairinNavigations(testid, ExecutedSteps + 1, actionmode: 2);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163348 Step 05");
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[ExecutedSteps].status = "Pass";

                result.FinalResult(ExecutedSteps);
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

        public TestCaseResult Test_163350(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
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

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 02
                IWebElement Navigation1Control = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Actions action = new Actions(Driver);
                action.MoveToElement(Navigation1Control, Navigation1Control.Size.Width / 4, Navigation1Control.Size.Height / 4).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action.Release().Build().Perform();
                res = CompareImage(result.steps[ExecutedSteps],Navigation1Control,ImageFormat:"png");
                if (res)
                    result.steps[ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163350 Step 02");
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                result.FinalResult(ExecutedSteps);
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

        public TestCaseResult Test_163338(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string CursonName = TestData[0];
            string ResetValue = TestData[1];
            string WLValues = TestData[2];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer viewer2D = new BluRingViewer();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to Studies tab, Search and view the study that has a 3D supported series.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                login.ClearFields();
                login.SearchStudy("patient", PatientID);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: "Patient ID", value: PatientID);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                ExecutedSteps++;

                //step:2  -Series loaded with out any errors in MPR 4:1 mode.
                Z3dViewerPage.selectthumbnail("Date:23-Jun-2013");
                Boolean step3 = Z3dViewerPage.select3dlayout("MPR");
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

                //step:3 - Select the window/level tool from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                String step5 = Viewport[0].GetCssValue("cursor");
                if (step5.Contains(CursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Left click and drag the mouse on the image on MPR navigation control 1.
                List<string> step6Before = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 50, (Viewport[0].Size.Height / 2) - 50);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step6After = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if(step6Before[0] != step6After[0] && step6Before[1] != step6After[1] && step6Before[2] != step6After[2] && step6Before[3] == step6After[3])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Click on the Reset button from 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step7 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (step7[0] == WLValues && step7[1] == WLValues && step7[2] == WLValues && step7[3] == WLValues)
                {
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

        public TestCaseResult Test_163351(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluringviewer = new BluRingViewer();
            Imager imager = new Imager();
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String ObjPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String ObjThumbNail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String ObjTestDataReq = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objthumbimg1 = ObjThumbNail.Split('|')[0];
                String objthumbimg2 = ObjThumbNail.Split('|')[1];
                String objpatid1 = ObjPatientID.Split('|')[0];
                String objpatid2 = ObjPatientID.Split('|')[1];
                String req1 = ObjTestDataReq.Split('|')[0];
                String req2 = ObjTestDataReq.Split('|')[1];
                String req3 = ObjTestDataReq.Split('|')[2];
                String req4 = ObjTestDataReq.Split('|')[3];
                Actions action = new Actions(Driver);

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 01");

                //step 02
                bool res = brz3dvp.searchandopenstudyin3D(objpatid1, objthumbimg1);
                if (!res)
                    throw new Exception("unable to open study in 3D Viewer");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 02");
                }

                //step 03
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                String LocValueBefore = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(5);
                res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 50, 50, 100);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed while applying Scroll Tool in Test_163351 Step 03");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Interactive_Zoom, 50, 50, 100);
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed while applying Zoom Tool in Test_163351 Step 03");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed while applying Rotate Tool in Test_163351 Step 03");
                            result.steps[++ExecutedSteps].status = "Fail";
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        else
                        {
                            
                            res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Line_Measurement, 50, 50, 100, testid, ExecutedSteps + 1);
                            if (!res)
                            {
                                Logger.Instance.ErrorLog("Failed while applying Line Measurement Tool in Test_163351 Step 03");
                                result.steps[++ExecutedSteps].status = "Fail";
                                result.steps[ExecutedSteps].SetLogs();
                            }
                            else
                            {
                                res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Pan, 60, 60, 100,movement :"negative");
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed while applying Pan Tool in Test_163351 Step 03");
                                    result.steps[++ExecutedSteps].status = "Fail";
                                    result.steps[ExecutedSteps].SetLogs();
                                }
                                else
                                {
                                    res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Selection_Tool, 50, 0, 0, testid, ExecutedSteps + 2, movement: "positive");
                                    if (!res)
                                    {
                                        Logger.Instance.ErrorLog("Failed while applying Tissue Selection Tool in Test_163351 Step 03");
                                        result.steps[++ExecutedSteps].status = "Fail";
                                        result.steps[ExecutedSteps].SetLogs();
                                    }
                                    else
                                    {
                                        res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Sculpt_Tool_for_3D_1_Polygon, 50, 50, 100, testid, ExecutedSteps + 3);
                                        //brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Sculpt_Tool_for_3D_1_Polygon, 50, 50, 100, testid, ExecutedSteps + 3);
                                        if (!res)
                                        {
                                            Logger.Instance.ErrorLog("Failed while applying Sculpt Tool in Test_163351 Step 03");
                                            result.steps[++ExecutedSteps].status = "Fail";
                                            result.steps[ExecutedSteps].SetLogs();
                                        }
                                        else
                                        {
                                            result.steps[++ExecutedSteps].status = "Pass";
                                            Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 03");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //step 04
                res = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed while applying Render Type over Navigation1 in Test_163351 Step 04");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                    bool res2 = brz3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.MinIp);
                    bool res3 = brz3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MinIp);
                    if (!res && !res2 && !res3)
                    {
                        Logger.Instance.ErrorLog("Failed while verifying appplied Render Type over all Navigation controls in Test_163351 Step 04");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 04");
                    }
                }

                //step 05
                res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.BoneBody, "Preset");
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed while applying preset mode over Navigation1 in Test_163351 Step 05");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.BoneBody, "Preset");
                    bool res2 = brz3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.BoneBody, "Preset");
                    bool res3 = brz3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.BoneBody, "Preset");
                    if (!res && !res2 && !res3)
                    {
                        Logger.Instance.ErrorLog("Failed while verifying applied preset mode over all Navigation controls in Test_163351 Step 04");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 05");
                    }
                }

                //step 06
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String Navigation1imagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + ExecutedSteps +2 + ".jpg";
                DownloadImageFile(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone),Navigation1imagepath,"png");
                if (File.Exists(Navigation1imagepath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 06");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while taking screenshot in Test_163351 Step 06");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 07
                res = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed while selecting download image tool in Test_163351 Step 07");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 07");
                }

                //step 08
                String imagename = testid + ExecutedSteps+2;
                String Step8_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step8_imgLocation))
                    File.Delete(Step8_imgLocation);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename,"jpg");
                PageLoadWait.WaitForFrameLoad(2);
                if (File.Exists(Step8_imgLocation))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 08");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while downloading the image using download image tool in Test_163351 Step 08");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 09
                res = brz3dvp.CompareDownloadimage(Step8_imgLocation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //String DownloadedimageOutputFile = Config.downloadpath + Path.DirectorySeparatorChar + "DownloadOutput.txt";
                //String DownloadedimageResizePath = Config.downloadpath + Path.DirectorySeparatorChar + "DownloadZoomImage.jpg";
                //imager.PerformImageResize(Step8_imgLocation,1300,1300, DownloadedimageResizePath);
                //String DownloadedimageTextValues = brz3dvp.TextFromImage(DownloadedimageResizePath, 4, DownloadedimageOutputFile);

                //int BluValueinDownloadImage = brz3dvp.selectedcolorcheck(Step8_imgLocation, 0, 0, 255, 2);
                //int YellowValueinDownloadImage = brz3dvp.selectedcolorcheck(Step8_imgLocation, 255, 255, 0, 2);
                //int BlackRegioninDownloadImage = brz3dvp.selectedcolorcheck(Step8_imgLocation, 0, 0, 0, 2);

                //String ScreenshotimageOutputFile = Config.downloadpath + Path.DirectorySeparatorChar + "ScreenshotOutput.txt";
                //String ScreenshotimageResizePath = Config.downloadpath + Path.DirectorySeparatorChar + "ScreenshotZoomImage.jpg";
                //imager.PerformImageResize(Navigation1imagepath, 1300, 1300, ScreenshotimageResizePath);
                //String ScreenShotimageTextValues = brz3dvp.TextFromImage(ScreenshotimageResizePath, 4, ScreenshotimageOutputFile);

                //int BluValueinSSImage = brz3dvp.selectedcolorcheck(Navigation1imagepath, 0, 0, 255, 2);
                //int YellowValueinSSImage = brz3dvp.selectedcolorcheck(Navigation1imagepath, 255, 255, 0, 2);
                //int BlackRegioninSSImage = brz3dvp.selectedcolorcheck(Navigation1imagepath, 0, 0, 0, 2);

                //bool ControlNameVerification = ScreenShotimageTextValues.Contains(BluRingZ3DViewerPage.Navigationone) && DownloadedimageTextValues.Contains(BluRingZ3DViewerPage.Navigationone);
                //bool SerVerification = DownloadedimageTextValues.Contains("SCORING, CALCIUM");
                //bool DOB_SexVerification = DownloadedimageTextValues.Replace(":", "").Contains("DOB 31-Jan-1959 Sex M");

                //bool BluValueinImage = BluValueinSSImage > 0 && BluValueinDownloadImage > 0;
                //bool YellowValueinImage = YellowValueinDownloadImage > 0 && YellowValueinSSImage > 0;
                //bool BlackValueinImage = BlackRegioninDownloadImage > 0 && BlackRegioninSSImage > 0 ;

                //if ((ControlNameVerification && SerVerification && DOB_SexVerification) || (BluValueinImage && YellowValueinImage && BlackValueinImage))
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 09");
                //}
                //else
                //{
                //    Logger.Instance.ErrorLog("Failed in Test_163351 Step 09 since the downloaded image and screenshot image are different");
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //step 10 & 11
                String Navigation2imagepath = null, Step10_imgLocation = null;
                res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed while changing the layout to 3D 6:1 in Test_163351 Step 10 & 11");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    Navigation2imagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + ExecutedSteps + 2 + ".jpg";
                    DownloadImageFile(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), Navigation2imagepath, "png");
                    String imagename_10 = testid + ExecutedSteps + 2;
                    Step10_imgLocation = Config.downloadpath + "\\" + imagename_10 + ".jpg";
                    if (File.Exists(Step10_imgLocation))
                        File.Delete(Step10_imgLocation);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(2);
                    brz3dvp.downloadImageForViewport(imagename_10, "png");
                    PageLoadWait.WaitForFrameLoad(2);
                    if (File.Exists(Step10_imgLocation))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 10 & 11");
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Failed while downloading the image using download image tool in Test_163351 step 10 & 11");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //step 12
                res = brz3dvp.CompareDownloadimage(Step10_imgLocation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();
                //imager.PerformImageResize(Step10_imgLocation, 1300, 1300, DownloadedimageResizePath);
                //String cropimage = Config.downloadpath + Path.DirectorySeparatorChar + "CropImage.jpg";
                //imager.CropAndSaveImage(DownloadedimageResizePath,0,0,1300,245,cropimage);
                //DownloadedimageTextValues = brz3dvp.TextFromImage(cropimage, 4, DownloadedimageOutputFile);

                //BluValueinDownloadImage = brz3dvp.selectedcolorcheck(Step10_imgLocation, 0, 0, 255, 2);
                //BlackRegioninDownloadImage = brz3dvp.selectedcolorcheck(Step10_imgLocation, 0, 0, 0, 2);

                //imager.PerformImageResize(Navigation2imagepath, 1300, 1300, ScreenshotimageResizePath);
                //ScreenShotimageTextValues = brz3dvp.TextFromImage(ScreenshotimageResizePath, 4, ScreenshotimageOutputFile);

                //BluValueinSSImage = brz3dvp.selectedcolorcheck(Navigation2imagepath, 0, 0, 255, 2);
                //BlackRegioninSSImage = brz3dvp.selectedcolorcheck(Navigation2imagepath, 0, 0, 0, 2);

                //SerVerification = ScreenShotimageTextValues.Contains("SCORING, CALCIUM") && DownloadedimageTextValues.Contains("SCORING, CALCIUM");
                //DOB_SexVerification = ScreenShotimageTextValues.Replace(":","").Contains("DOB 31-Jan-1959 Sex M") && DownloadedimageTextValues.Replace(":", "").Contains("DOB 31-Jan-1959 Sex M");

                //BluValueinImage = BluValueinSSImage > 0 && BluValueinDownloadImage > 0;
                //BlackValueinImage = BlackRegioninDownloadImage > 0 && BlackRegioninSSImage > 0;

                //if ((SerVerification && DOB_SexVerification) || (BluValueinImage && BlackValueinImage))
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 12");
                //}
                //else
                //{
                //    Logger.Instance.ErrorLog("Failed in Test_163351 Step 12 since the downloaded image and screenshot image are different for Navigation 2");
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //step 13
                res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 13 while selecting Curved MPR Layout");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    int WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 2, 255, 255, 255, 2);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).SendKeys("T").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    int WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 2, 255, 255, 255, 2);
                    if (WhiteColorAfter == WhiteColorBefore)
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163351 Step 13 while selecting Curved MPR Layout");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 13");
                    }
                }

                //step 14
                int ColorValBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree),testid,ExecutedSteps+2,0,0,255,2);
                action = new Actions(Driver);
                action.MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 100, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) - 100).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action.Release().Build().Perform();
                int ColorValAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 3, 0, 0, 255, 2);
                if (ColorValAfter != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 14");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 14 while drawing path in Navigation 3");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 15
                String Navigation3imagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + ExecutedSteps + 2 + ".jpg";
                DownloadImageFile(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), Navigation3imagepath, "png");
                if (File.Exists(Navigation3imagepath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 15");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 15 while taking screenshot of Navigation 3 image");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 16
                imagename = testid + ExecutedSteps + 2;
                String Step16_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step16_imgLocation))
                    File.Delete(Step16_imgLocation);
                brz3dvp.select3DTools(Z3DTools.Download_Image);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(2);
                if (File.Exists(Step16_imgLocation))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 16");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while downloading the image using download image tool in Test_163351 Step 16 for Navigation 3");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 17
                res = brz3dvp.CompareDownloadimage(Step16_imgLocation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();
                //imager.PerformImageResize(Step16_imgLocation, 1300, 1300, DownloadedimageResizePath);
                //imager.CropAndSaveImage(DownloadedimageResizePath, 0, 0, 1300, 245, cropimage);
                //DownloadedimageTextValues = brz3dvp.TextFromImage(cropimage, 4, DownloadedimageOutputFile);

                //BluValueinDownloadImage = brz3dvp.selectedcolorcheck(Step16_imgLocation, 0, 0, 255, 2);
                //BlackRegioninDownloadImage = brz3dvp.selectedcolorcheck(Step16_imgLocation, 0, 0, 0, 2);

                //imager.PerformImageResize(Navigation3imagepath, 1300, 1300, ScreenshotimageResizePath);
                //String Screenshotcropimage = Config.downloadpath + Path.DirectorySeparatorChar + "CropImage.jpg";
                //imager.CropAndSaveImage(ScreenshotimageResizePath, 0, 0, 1300, 245, Screenshotcropimage);
                //ScreenShotimageTextValues = brz3dvp.TextFromImage(Screenshotcropimage, 4, ScreenshotimageOutputFile);

                //BluValueinSSImage = brz3dvp.selectedcolorcheck(Navigation3imagepath, 0, 0, 255, 2);
                //BlackRegioninSSImage = brz3dvp.selectedcolorcheck(Navigation3imagepath, 0, 0, 0, 2);

                //bool textverification = ScreenShotimageTextValues.Replace("\n", "").Replace(" ", "").Equals(DownloadedimageTextValues.Replace("\n", "").Replace(" ", ""));

                //BluValueinImage = BluValueinSSImage > 0 && BluValueinDownloadImage > 0;
                //BlackValueinImage = BlackRegioninDownloadImage > 0 && BlackRegioninSSImage > 0;

                //if (textverification || (BluValueinImage && BlackValueinImage))
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 17");
                //}
                //else
                //{
                //    Logger.Instance.ErrorLog("Failed in Test_163351 Step 12 since the downloaded image and screenshot image are different for Navigation 2");
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //step 18
                res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163551 Step 18 while selecting Calcium Scoring Layout");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, BluRingZ3DViewerPage.Close);
                    PageLoadWait.WaitForFrameLoad(5);
                    int WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 2,255,255,255, 2);
                    bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.ShowText);
                    int WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 2, 255, 255, 255, 2);
                    if (WhiteColorAfter != WhiteColorBefore)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 18");
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163551 Step 18 while selecting Hide Image text from Smart View Dropdown");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //step 19
                int GreenColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 2, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog,"RCA");
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                Actions calciumaction = new Actions(Driver);
                calciumaction.MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 2 + 10).ClickAndHold()
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Height / 2 + 200)
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Height / 2 + 200)
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 2 + 200).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int GreenColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 2, 0, 255, 0, 2);
                if (GreenColorAfter != GreenColorBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 19");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163551 Step 19 while applying Calcium Scoring RCA");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 20
                String CalciumScoringimagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_20.jpg";
                DownloadImageFile(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), CalciumScoringimagepath, "png");
                if (File.Exists(CalciumScoringimagepath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 20");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 20 while taking screenshot of Calcium Scoring image");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 21
                imagename = testid + ExecutedSteps + 2;
                String Step21_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step21_imgLocation))
                    File.Delete(Step21_imgLocation);
                brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.CalciumScoring);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "png");
                PageLoadWait.WaitForFrameLoad(2);
                if (File.Exists(Step21_imgLocation))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 21");
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while downloading the image using download image tool in Test_163351 Step 20 for Calcium Scoring");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 22
                res = brz3dvp.CompareDownloadimage(Step21_imgLocation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();
                //imager.PerformImageResize(Step21_imgLocation, 1300, 1300, DownloadedimageResizePath);
                //DownloadedimageTextValues = brz3dvp.TextFromImage(DownloadedimageResizePath, 4, DownloadedimageOutputFile);

                //imager.PerformImageResize(CalciumScoringimagepath, 1300, 1300, ScreenshotimageResizePath);
                //cropimage = Config.downloadpath + Path.DirectorySeparatorChar + "CropImage.jpg";
                //imager.CropAndSaveImage(ScreenshotimageResizePath, 0, 0, 192, 114, cropimage);
                //imager.PerformImageResize(cropimage, 303, 303, ScreenshotimageResizePath);
                //ScreenShotimageTextValues = brz3dvp.TextFromImage(ScreenshotimageResizePath, 4, ScreenshotimageOutputFile);

                //ControlNameVerification = ScreenShotimageTextValues.Contains(BluRingZ3DViewerPage.CalciumScoring) && DownloadedimageTextValues.Contains(BluRingZ3DViewerPage.CalciumScoring);

                //int DownloadimageGreenLevel = brz3dvp.selectedcolorcheck(Step21_imgLocation, 0, 255, 0, 2);
                //int ScreenshotimageGreenLevel = brz3dvp.selectedcolorcheck(CalciumScoringimagepath, 0, 255, 0, 2);

                //if ((DownloadimageGreenLevel != 0 && ScreenshotimageGreenLevel != 0) || ControlNameVerification)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 22");
                //}
                //else
                //{
                //    Logger.Instance.ErrorLog("Failed in Test_163351 Step 22 since the downloaded image and screenshot image are different for Calcium Scoring Image");
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //step 23
                res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 23 while lauching MPR Layout");
                    throw new Exception("Failed to open MPR Layout");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 23");
                }

                //step 24
                res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 30);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 while setting MPR Interactive Quality between 1 to 100");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 30);
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 while setting 3D Interactive Quality between 1 to 100");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        PageLoadWait.WaitForFrameLoad(5);
                        String Navigation1BottomValue = brz3dvp.GetCenterBottomAnnotationLocationValue(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone));
                        String Navigation2BottomValue = brz3dvp.GetCenterBottomAnnotationLocationValue(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo));
                        String Navigation3BottomValue = brz3dvp.GetCenterBottomAnnotationLocationValue(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree));
                        if (Navigation1BottomValue.Equals("Lossy Compressed") && Navigation2BottomValue.Equals("Lossy Compressed") && Navigation3BottomValue.Equals("Lossy Compressed"))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 24");
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Failed in Test_163351 Step 24 as Lossy Compressed annotations failed to display over the images");
                            result.steps[++ExecutedSteps].status = "Fail";
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                }

                //step 25
                res = brz3dvp.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.Navigationone);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 25 as png radio button not disabled while downloading image from navigation1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.Navigationtwo);
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163351 Step 25 as png radio button not disabled while downloading image from navigation2");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        res = brz3dvp.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.Navigationthree);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed in Test_163351 Step 25 as png radio button not disabled while downloading image from navigation1");
                            result.steps[++ExecutedSteps].status = "Fail";
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 25");
                        }
                    }
                }

                //step 26
                res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (!res)
                {
                    throw new Exception("Failed while changing the layout to 3D 4:1 Layout");
                }
                else
                {
                    res = brz3dvp.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.Navigation3D1);
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163351 Step 26 as png radio button not disabled while downloading image from 3D1 Control in 3D 4:1 Layout");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                        if (!res)
                        {
                            throw new Exception("Failed while changing the layout to 3D 6:1 Layout");
                        }
                        else
                        {
                            res = brz3dvp.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.Navigation3D1);
                            if (!res)
                            {
                                Logger.Instance.ErrorLog("Failed in Test_163351 Step 26 as png radio button not disabled while downloading image from 3D1 Control in 3D 6:1 Layout");
                                result.steps[++ExecutedSteps].status = "Fail";
                                result.steps[ExecutedSteps].SetLogs();
                            }
                            else
                            {
                                res = brz3dvp.CheckEnabledButtonInDownloadBox(BluRingZ3DViewerPage.Navigation3D2);
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed in Test_163351 Step 26 as png radio button not disabled while downloading image from 3D2 Control in 3D 6:1 Layout");
                                    result.steps[++ExecutedSteps].status = "Fail";
                                    result.steps[ExecutedSteps].SetLogs();
                                }
                                else
                                {
                                    result.steps[++ExecutedSteps].status = "Pass";
                                    Logger.Instance.InfoLog("-->Test Step Passed-- Test_163351 step 26");
                                }
                            }
                        }
                    }
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163352(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluringviewer = new BluRingViewer();
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objtestdatarequirments = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String[] testdatareqarray = objtestdatarequirments.Split('|');
                String searchtype = testdatareqarray[0];
                String searchvalue = testdatareqarray[1];
                String thumbnailcount = testdatareqarray[2];
                String objthumbnaildesc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String[] objthumbnailarray = objthumbnaildesc.Split('|');
                String LossyThumbnail = objthumbnailarray[0];
                String LosslessThumbnail = objthumbnailarray[1];
                String objPatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //step 02
                bool res = brz3dvp.searchandopenstudyin3D(searchvalue, LossyThumbnail, field: searchtype, thumbnailcount: 8);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 03
                res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                if(!res)
                {
                    Logger.Instance.ErrorLog("Unable to open study failed in Test_163352 Step 03");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //step 04
                res = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Unable to select scrolling tool failed in Test_163352 Step 04");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";
                    

                //step 05
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                if (res)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 05 while scrolling through navigation 1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 06
                res = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 06 while selecting window level tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 07
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 100);
                if (res)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 07 while applying window level tool through navigation 2");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 08
                res = brz3dvp.select3DTools(Z3DTools.Pan);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 08 while selecting Pan tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 09
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                if (res)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 09 while applying Pan tool on Navigation 1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 10 while selecting Zoom tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 11
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                if (res)
                {
                    res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 100, 5);
                    if (res)
                        result.steps[++ExecutedSteps].status = "Pass";
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163352 Step 11 while performing Zoom in via NaVigation 1");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 11 while performing Zoom out via NaVigation 1");
                    result.steps[++ExecutedSteps].status = "Fail";
                }

                //step 12
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 12 while selecting Rotate Click Center tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 13
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 5, 100);
                if (res)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 13 while performing Rotate Click Center via NaVigation 3");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 80);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 14 while changing the MPR Interactive Quality to 80");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //Step 15
                res = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Unable to select scrolling tool failed in Test_163352 Step 15");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while scrolling through navigation 1");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        res = brz3dvp.select3DTools(Z3DTools.Window_Level);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while selecting window level tool");
                            result.steps[++ExecutedSteps].status = "Fail";
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        else
                        {
                            res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 100);
                            if (!res)
                            {
                                Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while applying window level tool through navigation 2");
                                result.steps[++ExecutedSteps].status = "Fail";
                                result.steps[ExecutedSteps].SetLogs();
                            }
                            else
                            {
                                res = brz3dvp.select3DTools(Z3DTools.Pan);
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while selecting Pan tool");
                                    result.steps[++ExecutedSteps].status = "Fail";
                                    result.steps[ExecutedSteps].SetLogs();
                                }
                                else
                                {
                                    res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                                    if (!res)
                                    {
                                        Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while applying Pan tool on Navigation 1");
                                        result.steps[++ExecutedSteps].status = "Fail";
                                        result.steps[ExecutedSteps].SetLogs();
                                    }
                                    else
                                    {
                                        res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                                        if (!res)
                                        {
                                            Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while selecting Zoom tool");
                                            result.steps[++ExecutedSteps].status = "Fail";
                                            result.steps[ExecutedSteps].SetLogs();
                                        }
                                        else
                                        {
                                            res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                                            if (!res)
                                            {
                                                Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while performing Zoom out via NaVigation 1");
                                                result.steps[++ExecutedSteps].status = "Fail";
                                                result.steps[ExecutedSteps].SetLogs();
                                            }
                                            else
                                            {
                                                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 100, 5);
                                                if (!res)
                                                {
                                                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while performing Zoom in via NaVigation 1");
                                                    result.steps[++ExecutedSteps].status = "Fail";
                                                    result.steps[ExecutedSteps].SetLogs();
                                                }
                                                else
                                                {
                                                    res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                                                    if (!res)
                                                    {
                                                        Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while selecting Rotate Click Center tool");
                                                        result.steps[++ExecutedSteps].status = "Fail";
                                                        result.steps[ExecutedSteps].SetLogs();
                                                    }
                                                    else
                                                    {
                                                        res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 5, 100);
                                                        if (res)
                                                            result.steps[++ExecutedSteps].status = "Pass";
                                                        else
                                                        {
                                                            Logger.Instance.ErrorLog("Failed in Test_163352 Step 15 while performing Rotatte Click Center via NaVigation 3");
                                                            result.steps[++ExecutedSteps].status = "Fail";
                                                            result.steps[ExecutedSteps].SetLogs();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //step 16
                bluringviewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(5);
                res = brz3dvp.searchandopenstudyin3D(objPatientID, LosslessThumbnail);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 17
                res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 17 while changing the value of MPR Interactive Quality to 100");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 18
                res = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 18 while selecting Scroll Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 19
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100, "n");
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 19 while verifying invisibility of lossy annotation via Scroll Tool in Navigation 1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 20
                res = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 20 while selecting Window Level Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 21
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 100, "n");
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 21 while verifying invisibility of lossy annotation via W/L Tool in Navigation 2");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 22
                res = brz3dvp.select3DTools(Z3DTools.Pan);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 22 while selecting Pan Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 23
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100, "n");
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 23 while verifying invisibility of lossy annotation via Pan Tool in Navigation 1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 24
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 24 while selecting Zoom tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 25
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100, "n");
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 25 while verifying invisibility of lossy annotation via Zoom out Navigation 1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 100, 5, "n");
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in Step 25 while verifying invisibility of lossy annotation via Zoom in Navigation 1");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                        result.steps[++ExecutedSteps].status = "Pass";
                }

                //step 26
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 26 while selecting Rotate Click Center Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 27
                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 100, 5, "n");
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 27 while verifying invisibility of lossy annotation via Rotate Click Center in Navigation 3");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 28
                res = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 80);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Step 28 while changing the MPR Interactive Quality to 80");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 29
                res = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Unable to select scrolling tool failed in Test_163352 Step 29");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while scrolling through navigation 1");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    else
                    {
                        res = brz3dvp.select3DTools(Z3DTools.Window_Level);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while selecting window level tool");
                            result.steps[++ExecutedSteps].status = "Fail";
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        else
                        {
                            res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 100);
                            if (!res)
                            {
                                Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while applying window level tool through navigation 2");
                                result.steps[++ExecutedSteps].status = "Fail";
                                result.steps[ExecutedSteps].SetLogs();
                            }
                            else
                            {
                                res = brz3dvp.select3DTools(Z3DTools.Pan);
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while selecting Pan tool");
                                    result.steps[++ExecutedSteps].status = "Fail";
                                    result.steps[ExecutedSteps].SetLogs();
                                }
                                else
                                {
                                    res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                                    if (!res)
                                    {
                                        Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while applying Pan tool on Navigation 1");
                                        result.steps[++ExecutedSteps].status = "Fail";
                                        result.steps[ExecutedSteps].SetLogs();
                                    }
                                    else
                                    {
                                        res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                                        if (!res)
                                        {
                                            Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while selecting Zoom tool");
                                            result.steps[++ExecutedSteps].status = "Fail";
                                            result.steps[ExecutedSteps].SetLogs();
                                        }
                                        else
                                        {
                                            res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 100);
                                            if (!res)
                                            {
                                                Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while performing Zoom out via NaVigation 1");
                                                result.steps[++ExecutedSteps].status = "Fail";
                                                result.steps[ExecutedSteps].SetLogs();
                                            }
                                            else
                                            {
                                                res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 100, 5);
                                                if (!res)
                                                {
                                                    Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while performing Zoom in via NaVigation 1");
                                                    result.steps[++ExecutedSteps].status = "Fail";
                                                    result.steps[ExecutedSteps].SetLogs();
                                                }
                                                else
                                                {
                                                    res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                                                    if (!res)
                                                    {
                                                        Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while selecting Rotate Click Center tool");
                                                        result.steps[++ExecutedSteps].status = "Fail";
                                                        result.steps[ExecutedSteps].SetLogs();
                                                    }
                                                    else
                                                    {
                                                        res = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 5, 100);
                                                        if (res)
                                                            result.steps[++ExecutedSteps].status = "Pass";
                                                        else
                                                        {
                                                            Logger.Instance.ErrorLog("Failed in Test_163352 Step 29 while performing Rotatte Click Center via NaVigation 3");
                                                            result.steps[++ExecutedSteps].status = "Fail";
                                                            result.steps[ExecutedSteps].SetLogs();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result.FinalResult(ExecutedSteps);
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

        public TestCaseResult Test_163353(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 02
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163353 Step 02");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 03
                IWebElement navigation1element = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                String LocVal0 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                new Actions(Driver).MoveToElement(navigation1element).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Actions act = new Actions(Driver);
                act.MoveToElement(navigation1element, navigation1element.Size.Width / 4, navigation1element.Size.Height / 2).ClickAndHold().MoveToElement(navigation1element, navigation1element.Size.Width / 2 , navigation1element.Size.Height/2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                String LocVal1 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                String LocVal2 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                String LocVal3 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                String LocVal4 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, "Loc:");
                if (!LocVal0.Equals(LocVal1) && LocVal1.Equals(LocVal2) && LocVal1.Equals(LocVal3) && LocVal1.Equals(LocVal4))
                {
                    brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel,BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                    IWebElement navigation2element = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    LocVal0 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                    act.MoveToElement(navigation2element, navigation2element.Size.Width / 2, navigation2element.Size.Height / 2).ClickAndHold().MoveToElement(navigation2element, (3/4)*(navigation2element.Size.Width), navigation2element.Size.Height/2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    LocVal1 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                    LocVal2 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                    LocVal3 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                    LocVal4 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, "Loc:");
                    if (!LocVal0.Equals(LocVal2) && LocVal2.Equals(LocVal1) && LocVal2.Equals(LocVal3) && LocVal2.Equals(LocVal4))
                    {
                        brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel);
                        IWebElement navigation3element = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                        LocVal0 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                        act.MoveToElement(navigation3element, navigation3element.Size.Width / 2, (3/4)*navigation3element.Size.Height ).ClickAndHold().MoveToElement(navigation3element, navigation3element.Size.Width / 2, navigation3element.Size.Height/2).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        LocVal1 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                        LocVal2 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                        LocVal3 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                        LocVal4 = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, "Loc:");
                        if (!LocVal0.Equals(LocVal3) && LocVal3.Equals(LocVal1) && LocVal3.Equals(LocVal2) && LocVal3.Equals(LocVal4))
                            result.steps[++ExecutedSteps].status = "Pass";
                        else
                        {
                            Logger.Instance.ErrorLog("Failed in Test_163353 Step 03 for navigation3");
                            result.steps[++ExecutedSteps].status = "Fail";
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163353 Step 03 for navigation2");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163353 Step 03 for navigation1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 04
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String LocVal1After = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                String LocVal2After = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                String LocVal3After = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                String LocVal4After = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel, "Loc:");
                if (!LocVal1.Equals(LocVal1After) && !LocVal2.Equals(LocVal2After) && !LocVal3.Equals(LocVal3After) && !LocVal4.Equals(LocVal4After))
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163353 Step 04");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 05
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163353 Step 05");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 06 & 07
                IWebElement resultcontrolelement = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                act.MoveToElement(resultcontrolelement, resultcontrolelement.Size.Width / 8, resultcontrolelement.Size.Height / 8).ClickAndHold().MoveToElement(resultcontrolelement, resultcontrolelement.Size.Width / 8, resultcontrolelement.Size.Height - 5).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                res = brz3dvp.checkerrormsg();
                if (!res)
                    throw new Exception("Warning message not found failed in Test_163353 Step 06 & 07");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                }

                result.FinalResult(ExecutedSteps);
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

        public TestCaseResult Test_163349(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String req1 = Requirements.Split('|')[0];
                String req2 = Requirements.Split('|')[1];
                String req3 = Requirements.Split('|')[2];
                String req4 = Requirements.Split('|')[3];
                IWebElement Navigation1;
                IWebElement Navigation2;
                IWebElement Navigation3;
                int YellowBefore = 0, YellowAfter = 0;
                String measurement, linelength;


                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(req1, objpatid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(req2, "");
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: req2, value: "");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "y");
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 02
                int navigationwidthbefore = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width;
                int navigationHeightbefore = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height;
                DoubleClick(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone));
                PageLoadWait.WaitForFrameLoad(10);
                int navigationwidthafter = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width;
                int navigationheightafter = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height;
                if (navigationwidthafter > navigationwidthbefore && navigationheightafter > navigationHeightbefore)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 02");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 03
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                res = brz3dvp.select3DTools(Z3DTools.Line_Measurement);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 03 while selecting Line Measurement Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 255, 255, 0, 2);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) ).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 6, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2)+2 ).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 255, 255, 0, 2);
                    measurement = brz3dvp.ReadPatientDetailsUsingTesseract(Navigation1, 4, 714, 700, 1000, 1000);
                    linelength = measurement.Split('\n')[0];
                    linelength = linelength.Split('.')[0];
                    if (linelength.Contains("1") && YellowAfter > YellowBefore)
                        result.steps[++ExecutedSteps].status = "Pass";
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163349 Step 03");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //step 04
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 21, 255, 255, 255, 2);
                brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, req4);
                PageLoadWait.WaitForFrameLoad(10);
                int WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 22, 255, 255, 255, 2);
                if (WhiteColorAfter > WhiteColorBefore)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 04");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 05
                bool toggleoff = brz3dvp.CheckCrossHairinNavigations(testid, ExecutedSteps + 2 , toggle : 1);
                PageLoadWait.WaitForFrameLoad(10);
                res = brz3dvp.select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigationone);
                if (res && toggleoff)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 05");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 06
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 261, 255, 255, 0, 2);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) + 3).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 50, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) + 3).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 262, 255, 255, 0, 2);
                if (YellowAfter > YellowBefore)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 06");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 07
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 49, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) - 41).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 49, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) + 3).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool result7 = CompareImage(result.steps[ExecutedSteps], Navigation1);
                if (result7)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 08
                brz3dvp.select3DTools(Z3DTools.Reset);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 281, 255, 255, 255, 2);
                brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, "20");
                PageLoadWait.WaitForFrameLoad(10);
                WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 282, 255, 255, 255, 2);
                if (WhiteColorAfter > WhiteColorBefore)
                {
                    Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 283, 255, 255, 0, 2);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) + 3).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 153, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) + 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 284, 255, 255, 0, 2);
                    if (YellowAfter > YellowBefore)
                    {
                        Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                        new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 153, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) - 144).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 2) + 153, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 2) + 3).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        bool result8 = CompareImage(result.steps[ExecutedSteps], Navigation1);
                        if (result8)
                            result.steps[ExecutedSteps].StepPass();
                        else
                            result.steps[ExecutedSteps].StepFail();
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163349 Step 08 while drawing 1st line in navigation1 with thickness 20 mm");
                        result.steps[ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 08 while adding thickness 20 mm");
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 09
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                brz3dvp.select3DTools(Z3DTools.Reset);
                navigationwidthbefore = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width;
                navigationHeightbefore = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height;
                DoubleClick(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo));
                PageLoadWait.WaitForFrameLoad(10);
                navigationwidthafter = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width;
                navigationheightafter = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height;
                int navigation2result = 0 , navigation3result = 0 ;
                if (navigationwidthafter > navigationwidthbefore && navigationheightafter > navigationHeightbefore)
                {
                    bool bflag9 = false;
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 291, 255, 255, 0, 2);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 2).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) + 6, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 292, 255, 255, 0, 2);
                    if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                    {
                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 9);
                        new Actions(Driver).SendKeys("T").Build().Perform(); Thread.Sleep(1000);
                        if (CompareImage(result.steps[ExecutedSteps], Navigation2))
                        {
                            
                            bflag9 = true;

                        }
                        new Actions(Driver).SendKeys("T").Build().Perform(); Thread.Sleep(1000);
                    }
                    else
                    {
                        measurement = brz3dvp.ReadPatientDetailsUsingTesseract(Navigation2, 4, 714, 700, 1000, 1000);
                        linelength = measurement.Split('\n')[0]; Thread.Sleep(500);
                        linelength = linelength.Split('.')[0]; Thread.Sleep(500);
                        bflag9 = linelength.Contains("1") && YellowAfter > YellowBefore;
                    }
                    if (bflag9==true)
                    {
                        brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                        WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 293, 255, 255, 255, 2);
                        brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, req4);
                        PageLoadWait.WaitForFrameLoad(10);
                        WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 294, 255, 255, 255, 2);
                        if (WhiteColorAfter > WhiteColorBefore)
                        {
                            YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 295, 255, 255, 0, 2);
                            new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2)).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 49).Release().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(10);
                            YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 296, 255, 255, 0, 2);
                            if (YellowAfter > YellowBefore)
                            {
                                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) - 44, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 49).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 49).Release().Build().Perform();
                                PageLoadWait.WaitForFrameLoad(10);
                                bool result9_1 = CompareImage(result.steps[ExecutedSteps], Navigation2);
                                if (result9_1)
                                {
                                    brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                                    WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 299, 255, 255, 255, 2);
                                    brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, "20");
                                    PageLoadWait.WaitForFrameLoad(10);
                                    WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 2910, 255, 255, 255, 2);
                                    if (WhiteColorAfter > WhiteColorBefore)
                                    {
                                        Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                                        YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2911, 255, 255, 0, 2);
                                        new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 3).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) + 2 , (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 150).Release().Build().Perform();
                                        PageLoadWait.WaitForFrameLoad(10);
                                        YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2912, 255, 255, 0, 2);
                                        if (YellowAfter > YellowBefore)
                                        {
                                            Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                                            new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) - 148, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 150).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Width / 2) + 3, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo).Size.Height / 2) + 150).Release().Build().Perform();
                                            PageLoadWait.WaitForFrameLoad(10);
                                            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                                            bool result9_2 = CompareImage(result.steps[ExecutedSteps], Navigation2);
                                            if (result9_2)
                                                navigation2result++;
                                            else
                                            {
                                                Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 the differenece between the line and square in navigation 2 is not equal to 1");
                                                
                                            }
                                        }
                                        else
                                        {
                                            Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing 2nd line in navigation2 with thickness 20 mm");
                                           
                                        }
                                    }
                                    else
                                    {
                                        Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while adding thickness 20 mm in navigation 2");
                                        
                                    }
                                }
                                else
                                {
                                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing 2nd line in navigation 2 with thickness 6 mm");
                                    
                                }
                            }
                            else
                            {
                                Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing 1st line in navigation 2 with 6 mm thickness");
                                
                            }

                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while entering thickness 6 mm in navigation 2");
                            
                        }
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing line annotationa in navigation2");
                       
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while switching to one up view mode");
                  
                }

                if (navigation2result == 1)
                {
                    brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                    
                    navigationwidthbefore = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width;
                    navigationHeightbefore = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height;
                    DoubleClick(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree));
                    PageLoadWait.WaitForFrameLoad(10);
                    navigationwidthafter = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width;
                    navigationheightafter = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height;
                    if (navigationwidthafter > navigationwidthbefore && navigationheightafter > navigationHeightbefore)
                    {
                        Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                        YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2915, 255, 255, 0, 2);
                        new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 6, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2916, 255, 255, 0, 2);
                        measurement = brz3dvp.ReadPatientDetailsUsingTesseract(Navigation3, 4, 714, 700, 1000, 1000);
                        linelength = measurement.Split('\n')[0];
                        linelength = linelength.Split('.')[0];
                        if (linelength.Contains("1") && YellowAfter > YellowBefore)
                        {
                            brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                            WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 2917, 255, 255, 255, 2);
                            brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, req4);
                            PageLoadWait.WaitForFrameLoad(10);
                            WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 2918, 255, 255, 255, 2);
                            if (WhiteColorAfter > WhiteColorBefore)
                            {
                                YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2919, 255, 255, 0, 2);
                                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 47, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).Release().Build().Perform();
                                PageLoadWait.WaitForFrameLoad(10);
                                YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2918, 255, 255, 0, 2);
                                if (YellowAfter > YellowBefore)
                                {
                                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 47, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 47).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 47, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(10);
                                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                                    bool result9_3 = CompareImage(result.steps[ExecutedSteps], Navigation3);
                                    if (result9_3)
                                    {
                                        brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                                        WhiteColorBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 2922, 255, 255, 255, 2);
                                        brz3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, "20");
                                        PageLoadWait.WaitForFrameLoad(10);
                                        WhiteColorAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 2923, 255, 255, 255, 2);
                                        if (WhiteColorAfter > WhiteColorBefore)
                                        {
                                            Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                                            YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2924, 255, 255, 0, 2);
                                            new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 2, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 150, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).Release().Build().Perform();
                                            PageLoadWait.WaitForFrameLoad(10);
                                            YellowAfter = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2925, 255, 255, 0, 2);
                                            if (YellowAfter > YellowBefore)
                                            {
                                                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                                                YellowBefore = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2926, 255, 255, 0, 2);
                                                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 150, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 150).ClickAndHold().MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Width / 2) + 150, (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree).Size.Height / 2) + 2).Release().Build().Perform();
                                                PageLoadWait.WaitForFrameLoad(10);
                                                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                                                bool result9_4 = CompareImage(result.steps[ExecutedSteps], Navigation1);
                                                if (result9_4)
                                                    navigation3result++;
                                                else
                                                {
                                                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 the differenece between the line and square in navigation 3 is not equal to 1");

                                                }
                                            }
                                            else
                                            {
                                                Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing 2nd line in navigation3 with thickness 20 mm");

                                            }
                                        }
                                        else
                                        {
                                            Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while adding thickness 20 mm in navigation 3");

                                        }
                                    }
                                    else
                                    {
                                        Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing 2nd line in navigation 3 with thickness 6 mm");

                                    }
                                }
                                else
                                {
                                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing 1st line in navigation 3 with 6 mm thickness");

                                }

                            }
                            else
                            {
                                Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while entering thickness 6 mm in navigation 3");

                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while drawing line annotationa in navigation 3");

                        }
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while switching to one up view mode in navigation 3");
                    }  
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 Step 09 while repeating steps in navigation 2");
                }

                if (navigation3result == 1 && navigation2result == 1)
                    result.steps[ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163349 repeat step 09");
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                result.FinalResult(ExecutedSteps);
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

        public TestCaseResult Test_163347(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String req1 = Requirements.Split('|')[0];
                String req2 = Requirements.Split('|')[1];

                //step 01 UNDO and REDO buttons are added in evnvironment setup
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //step 02
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(req1, objpatid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(req2, "");
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: req2, value: "");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "y");
                if (!res)
                    throw new Exception("unable to find the study");
                else
                {
                    res = brz3dvp.checkerrormsg("n");
                    if (res)
                        throw new Exception("Error message displayed");
                    else
                        result.steps[++ExecutedSteps].status = "Pass";
                }

                //step 03
                res = brz3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 02 while selecting 3D Sculpt Polygon Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    res = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                    PageLoadWait.WaitForFrameLoad(10);
                    new Point(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Width / 4, brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).Size.Height / 4);
                    PageLoadWait.WaitForFrameLoad(15);
                    if (brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone).GetCssValue("cursor").Contains(BluRingZ3DViewerPage.SculptToolCursor))
                        result.steps[++ExecutedSteps].status = "Pass";
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163347 Step 02 because mouse icon didnt change as expected");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //step 04
                String ControlElement = BluRingZ3DViewerPage.Navigationone;
                int startx = 50, starty = 50, endy = 100;
                int whitecolorrangebefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 255, 255, 255, 2, isMoveCursor: false);
                int blackcolorbefore_3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(ControlElement), (brz3dvp.controlelement(ControlElement).Size.Width / 2) - startx, (brz3dvp.controlelement(ControlElement).Size.Height / 2) - starty).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(ControlElement), (brz3dvp.controlelement(ControlElement).Size.Width / 2) - startx, (brz3dvp.controlelement(ControlElement).Size.Height / 2) + endy).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int whitecolorrangeafter_3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 255, 255, 255, 2, isMoveCursor: false);
                if (whitecolorrangeafter_3 > whitecolorrangebefore)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 03 as no white color is displayed while drawing line over the Navigation1 Control");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 05
              
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(ControlElement), (brz3dvp.controlelement(ControlElement).Size.Width / 2) - startx, (brz3dvp.controlelement(ControlElement).Size.Height / 2) + endy).Click().Build().Perform();
                Thread.Sleep(1000);
                int whitecolorrangeafter_4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 255, 255, 255, 2, isMoveCursor: false);
                if (whitecolorrangeafter_4 > whitecolorrangebefore)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 04 as no white color is displayed while drawing line over the Navigation1 Control");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 06
               
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(ControlElement), (brz3dvp.controlelement(ControlElement).Size.Width / 2) + (startx + 20), (brz3dvp.controlelement(ControlElement).Size.Height / 2) + endy).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int whitecolorrangeafter_5 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 255, 255, 255, 2, isMoveCursor: false);
                if (whitecolorrangeafter_5 > whitecolorrangeafter_4)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 05 as no white color is displayed while drawing line over the Navigation1 Control");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 07
               
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(ControlElement), (brz3dvp.controlelement(ControlElement).Size.Width / 2) - startx, (brz3dvp.controlelement(ControlElement).Size.Height / 2) - starty).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_6 > blackcolorbefore_3)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 06 as no region was removed after applying sculpt tool over the Navigation1 Control");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 08
                brz3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.UndoSculpt);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                int blackcolorafter_7 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_7 < blackcolorafter_6 && blackcolorafter_7 <= blackcolorbefore_3)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 07 as no region was added after undo sculpt tool over the Navigation1 Control");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 09
                brz3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Redosculpt);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_8 > blackcolorafter_7)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 08 as no region was removed after redo sculpt tool over the Navigation1 Control");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10
                brz3dvp.select3DTools(Z3DTools.Undo_Segmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_9 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_9 < blackcolorafter_6)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 09 as no region was removed after undo tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11
                brz3dvp.select3DTools(Z3DTools.Redo_Segmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_10 > blackcolorafter_9)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 10 as no region was removed after undo tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.UndoSegmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_11 < blackcolorafter_10)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 11 as no region was removed after undo tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 13
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RedoSegmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_12 > blackcolorafter_11)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 12 as no region was removed after undo tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14
                int Navigation1LocValBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 131, 0, 0, 0, 2, isMoveCursor: false);
                int Navigation2LocValBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 132, 0, 0, 0, 2, isMoveCursor: false);
                int Navigation3LocValBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 133, 0, 0, 0, 2, isMoveCursor: false);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                int Navigation1LocValAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 134, 0, 0, 0, 2, isMoveCursor: false);
                int Navigation2LocValAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 135, 0, 0, 0, 2, isMoveCursor: false);
                int Navigation3LocValAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 136, 0, 0, 0, 2, isMoveCursor: false);
                bool Navigation1Result = Navigation1LocValBefore.Equals(Navigation1LocValAfter);
                bool Navigation2Result = Navigation2LocValBefore.Equals(Navigation2LocValAfter);
                bool Navigation3Result = Navigation3LocValBefore.Equals(Navigation3LocValAfter);
                if (!Navigation1Result && !Navigation2Result && !Navigation3Result)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 13 while selecting reset tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 15
                res = brz3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Freehand);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 14 while selecting Sculpt Tool 3D Freehand");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.ToolbarDialog)));
                    IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                    closebttn.Click();
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.ToolbarDialog)));
                    res = brz3dvp.VerifyToolSelected(BluRingZ3DViewerPage.Navigationone,BluRingZ3DViewerPage.SculptToolFreehand);
                    if (res)
                        result.steps[++ExecutedSteps].status = "Pass";
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163347 Step 14 while hovering mouse pointer and verifying the cursor");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //step 16
                IWebElement inavigationone = brz3dvp.controlelement(ControlElement);
                int whitecolorrangebefore_15 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 255, 255, 255, 2, isMoveCursor: false);
                Actions actions = new Actions(Driver);
                new Actions(Driver).MoveToElement(inavigationone, (inavigationone.Size.Width / 2) - startx, (inavigationone.Size.Height / 2) - starty).ClickAndHold()
                .MoveToElement(inavigationone, (inavigationone.Size.Width / 2) - startx, (inavigationone.Size.Height / 2) + endy)
                .MoveToElement(inavigationone, (inavigationone.Size.Width / 2) + (startx + 20), (inavigationone.Size.Height / 2) + endy).Build().Perform();
                Thread.Sleep(5000);
                int whitecolorrangeafter_15 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 255, 255, 255, 2, isMoveCursor: false);
                if (whitecolorrangeafter_15 > whitecolorrangebefore_15)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 15 while drawing a line in Navigation1 using Freehand 3D Sculpt Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 17
                int blackcolorbefore_16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                new Actions(Driver).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                int blackcolorafter_16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_16 > blackcolorbefore_16)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 16 while drawing a line in Navigation1 using Freehand 3D Sculpt Tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 18
                brz3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.UndoSculpt);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_17 < blackcolorafter_16)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 17 while selecting UndoSculpt from Sculpt Tool Dialog");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 19
                brz3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Redosculpt);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_18 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_18 > blackcolorafter_17)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 18 while selecting RedoSculpt from Sculpt Tool Dialog");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 20
                brz3dvp.select3DTools(Z3DTools.Undo_Segmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_19 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_19 < blackcolorafter_18)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 19 while selecting Undo Tool Option");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 21
                brz3dvp.select3DTools(Z3DTools.Redo_Segmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_20 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_20 > blackcolorafter_19)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 20 while selecting Redo Tool Option");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 22
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.UndoSegmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_21 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_21 < blackcolorafter_20)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 21 as no region was removed after undo tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 23
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RedoSegmentation);
                PageLoadWait.WaitForFrameLoad(10);
                int blackcolorafter_22 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 0, 2, isMoveCursor: false);
                if (blackcolorafter_22 > blackcolorafter_21)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 22 as no region was removed after undo tool");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 24
                Navigation1LocValBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 231, 0, 0, 0, 2, isMoveCursor: false);
                Navigation2LocValBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 232, 0, 0, 0, 2, isMoveCursor: false);
                Navigation3LocValBefore = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 233, 0, 0, 0, 2, isMoveCursor: false);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Navigation1LocValAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 234, 0, 0, 0, 2, isMoveCursor: false);
                Navigation2LocValAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 235, 0, 0, 0, 2, isMoveCursor: false);
                Navigation3LocValAfter = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 236, 0, 0, 0, 2, isMoveCursor: false);
                Navigation1Result = Navigation1LocValBefore.Equals(Navigation1LocValAfter);
                Navigation2Result = Navigation2LocValBefore.Equals(Navigation2LocValAfter);
                Navigation3Result = Navigation3LocValBefore.Equals(Navigation3LocValAfter);
                if (!Navigation1Result && !Navigation2Result && !Navigation3Result)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163347 Step 23 while selecting reset tool");
                    result.steps[++ExecutedSteps].status = "Fail";
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163346(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            Login login = new Login();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String objlocvalue = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

                //step 01 Adding UNDO and REDO are done in environment setup
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //step 02
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                //step 03 Select the Tissue Selection Tool from the 3D toolbox.
                res = brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                if (res)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 03 because Tissue Selection Dialog is unavailable");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }
                IWebElement INavigationone = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement INavigationtwo = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement INavigationthree = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);


                //step 04 Click on the "Small Vessels" radio button on the Tissue Selection Tool dialog.
                IWebElement ThreshholdValue4 = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                IWebElement Radiousvalue4 = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                String BoneThresholdValue4 = ThreshholdValue4.GetAttribute("aria-valuenow");
                String BoneRadioValue4 = Radiousvalue4.GetAttribute("aria-valuenow");
                if (brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.SmallVessels))
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String SmallVesselThresholdValue = ThreshholdValue4.GetAttribute("aria-valuenow");
                    String SmallVesselRadiousValue4 = Radiousvalue4.GetAttribute("aria-valuenow");
                    if (Convert.ToInt32(SmallVesselThresholdValue) > Convert.ToInt32(BoneThresholdValue4) && SmallVesselRadiousValue4 == "50")
                        result.steps[++ExecutedSteps].status = "Pass";
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163346 Step 03 while selecting small vessel radio button from tissue selection dialog");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //step 05  	Click the aorta in the MPR navigation control 1. 
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                System.Drawing.Point location = brz3dvp.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                Cursor.Position = new System.Drawing.Point(location.X, location.Y);
                //this.Cursor = new Cursor(Cursor.Current.Handle);
                //Cursor.Position = new System.Drawing.Point((INavigationone.Location.X + 100), (INavigationone.Location.Y + 150));
                Thread.Sleep(1000);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "chrome")
                {
                    int i = 0;
                    do
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 50, 0);
                        Thread.Sleep(1000);
                        i++;
                        if (i > 100) break;
                    }
                    while (brz3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2, 0, 1) <= 34);
                }
                else
                {
                    for (int i = 0; i < 44; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(5000);


                int TissueColorBefore04 = brz3dvp.LevelOfSelectedColor(INavigationone, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 47).Click().Build().Perform();
                //    Thread.Sleep(1000);
                //    new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).Click().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 30, (Navigation1.Size.Height / 4) - 35).Click().Build().Perform();
                Thread.Sleep(1000);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int TissueColorAfter04 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 52, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                double TissueSelectionValBefore05_1 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                if (TissueColorAfter04 != TissueColorBefore04 && TissueSelectionValBefore05_1 > 0)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 04 while applying small vessel tissue selection over Navigation1");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 06 From the Tissue selection window, Adjust the Threshold value by moving the slider and Click on the Apply new settings button.
                //  double TissueSelectionValBefore05 = brz3dvp.GetSelectionVolume();
                //  PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Threshold, 50);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TissueSelectionValAfter05 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);

                //  wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                //    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int TissueSelectionAfter05 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 6, 0, 0, 255, 2);
                if (TissueSelectionValBefore05_1 != TissueSelectionValAfter05 && TissueSelectionAfter05 != TissueColorAfter04)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 05 while applying new Threshold Settings on small vessel tissue selection dialog");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 07 From the Tissue selection window, Adjust the Radius value by moving the slider and Click on the Apply new settings button.
                //    brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //    PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Radius, 2000);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TissueSelectionValAfter06 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                if (TissueSelectionValAfter05 != TissueSelectionValAfter06)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 06 while applying new Radius Settings on small vessel tissue selection dialog");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 08 On the tissue selection tool dialog, click "Delete Selected" button.
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
              //  new Actions(Driver).SendKeys("T").Build().Perform();
              //  Thread.Sleep(500);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteSelected);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TissueSelectionValueafter08 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                bool bflag8 = false;
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (TissueSelectionValueafter08 == 0)
                {
                    bflag8 = true;
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            bflag8 = true;
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                }
                if (bflag8 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 09 Click the "Undo Selection" button.
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                bool bflag9 = false;
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TissueSelectionValueafter09 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                // result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (TissueSelectionValueafter09 > 0)
                {
                    bflag9 = true;
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            bflag9 = true;
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                }
                if (bflag9 == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10 On the tissue selection dialog note the calculated volume at the bottom.
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step9Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                if (TissueSelectionValAfter06 == Step9Volume)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 09 as the volume cubic value noted in step 9 and step 6 are different");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11Click the "Redo Selection" button.
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step11Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step11Volume == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    result.steps[ExecutedSteps].status = "Fail";
                    //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    //    result.steps[ExecutedSteps].SetLogs();
                    //}
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12 Click the "Undo Selection" button.
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(10);
                Double Step12Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                if (Step12Volume > 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";

                    //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 13Click the "Redo Selection" button from the view port top bar.
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RedoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step13Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step13Volume == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    //int Nav1blackcolorafter12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 131, 0, 0, 0, 2);
                    //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    result.steps[ExecutedSteps].status = "Fail";
                    //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    //    result.steps[ExecutedSteps].SetLogs();
                    //}
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14Click the "undo Selection" button from the view port top bar.
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.UndoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step14Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(15);
                //  result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (Step14Volume > 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 14 as Step14Volume value is different from Step9Volume value ");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 15On the tissue selection dialog note the calculated volume at the bottom.
                if (Step14Volume == Step9Volume)
                    result.steps[++ExecutedSteps].status = "Pass";
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 14 as Step14Volume value is different from Step9Volume value ");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 16 On the tissue selection tool dialog, click the "Delete Unselected" button.
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteUnselected);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(15);

                Double Step16Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(15);
                if (Step16Volume == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    result.steps[ExecutedSteps].status = "Fail";
                    //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    //    result.steps[ExecutedSteps].SetLogs();
                    //}
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 17 select the Undo button from the 3D toolbox.
                //step 18On the tissue selection dialog note the calculated volume at the bottom.
                brz3dvp.select3DTools(Z3DTools.Undo_Segmentation);
                PageLoadWait.WaitForFrameLoad(15);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                //{
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                //    {
                //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                //        {
                //            result.steps[ExecutedSteps].status = "Pass";
                //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //        }
                //    }
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}


                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step17Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step9Volume == Step17Volume)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 17 as the volume calculated in Setp 9 and step 17 are different");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 19 Select the Redo button from the 3D toolbox.
                //    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                //   wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                //     PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Redo_Segmentation);
                PageLoadWait.WaitForFrameLoad(15);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step19Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(10);
                if (Step19Volume == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    //  brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                    //    PageLoadWait.WaitForFrameLoad(5);
                    //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    result.steps[ExecutedSteps].status = "Fail";
                    //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    //    result.steps[ExecutedSteps].SetLogs();
                    //}
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 20   	On the tissue selection tool dialog, click the "Undo Selection" button.
                //step 21 On the tissue selection dialog note the calculated volume at the bottom.
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(15);
                Double Step20Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(15);
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                //{
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                //    {
                //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                //        {
                //            result.steps[ExecutedSteps].status = "Pass";
                //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //        }
                //    }
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //step 21 On the tissue selection dialog note the calculated volume at the bottom.
                //brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //PageLoadWait.WaitForFrameLoad(15);
                //Double Step20Volume = brz3dvp.GetSelectionVolume();
                //PageLoadWait.WaitForFrameLoad(10);
                if (Step9Volume == Step20Volume)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                }

                else
                {
                    Logger.Instance.ErrorLog("Failed in Test_163346 Step 20 as the volume values from step 20 is different from the valume calculated in Step 9");
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 22 Select the Reset button from the 3D tool box.
                //    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                //    PageLoadWait.WaitForFrameLoad(10);
                IWebElement verifysmall = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
//new Actions(Driver).SendKeys("T").Build().Perform();
  //              Thread.Sleep(500);
                List<string> beforereset22 = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                brz3dvp.select3DTools(Z3DTools.Reset);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(30);

                List<string> Afterreset22 = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
        //        new Actions(Driver).SendKeys("T").Build().Perform();
        //        Thread.Sleep(500);
                if (Afterreset22[0] == Afterreset22[1] && Afterreset22[2] == Afterreset22[3])
                {
                    int reset_sm= brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 22, 0, 0, 255, 2);
                    if(reset_sm <=200)
                        result.steps[++ExecutedSteps].status = "Pass";
                    //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    //        {
                    //            result.steps[ExecutedSteps].status = "Pass";
                    //            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    result.steps[ExecutedSteps].status = "Fail";
                    //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    //    result.steps[ExecutedSteps].SetLogs();
                    //}
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
             //   new Actions(Driver).SendKeys("T").Build().Perform();
            //    Thread.Sleep(500);
                //step 23 Click on the "Large vessels" radio button on the Tissue Selection Tool dialog.
                IWebElement INavigationonelarge = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement INavigationtwolarge = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement INavigationthreelarge = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);

                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                IWebElement ThreshholdValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                IWebElement Radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                if (brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.LargeVessels))
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String LargeVesselThresholdValue = ThreshholdValue.GetAttribute("aria-valuenow");
                    String LargeVesselRadiousValue = Radiousvalue.GetAttribute("aria-valuenow");
                    if (Convert.ToInt32(LargeVesselThresholdValue) <= 22 && Convert.ToInt32(LargeVesselRadiousValue) >= 200)
                        result.steps[++ExecutedSteps].status = "Pass";
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163346 Step 22 while selecting Large vessel radio button from tissue selection dialog");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //For Large Vessels Repeat steps 5-21
                //step 05  	Click the aorta in the MPR navigation control 1.
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(15);
                System.Drawing.Point locationlar = brz3dvp.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                Cursor.Position = new System.Drawing.Point(locationlar.X, locationlar.Y);
                //this.Cursor = new Cursor(Cursor.Current.Handle);
                //Cursor.Position = new System.Drawing.Point((INavigationonelarge.Location.X + 100), (INavigationonelarge.Location.Y + 150));
                Thread.Sleep(1000);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox"|| Config.BrowserType.ToLower() == "chrome")
                {
                    int i = 0;
                    do
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 50, 0);
                        Thread.Sleep(1000);
                        i++;
                        if (i > 100) break;
                    }
                    while (brz3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2, 0, 1) <= 34);
                }
                else
                {
                    for (int i = 0; i < 44; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                        Thread.Sleep(2000);
                    }
                }
                Thread.Sleep(5000);

                bool bflag5_l = false;
                int TissueColorlargeBefore04 = brz3dvp.LevelOfSelectedColor(INavigationonelarge, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationonelarge, (INavigationonelarge.Size.Width / 2) + 18, (INavigationonelarge.Size.Height / 4) - 40).Click().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(INavigationonelarge, (INavigationonelarge.Size.Width / 2) + 18, (INavigationonelarge.Size.Height / 4) - 30).Click().Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));

                int TissueColorlargeAfter04 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 52, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                double Ts_vlaee_before5 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                if (TissueColorlargeAfter04 != TissueColorlargeBefore04 && Ts_vlaee_before5 > 0)
                    bflag5_l = true;
                else
                    Logger.Instance.ErrorLog("Failed on bflag5_l steps ");


                //step 06 From the Tissue selection window, Adjust the Threshold value by moving the slider and Click on the Apply new settings button.
                bool bflag6_l = false;
                double Ts_valuebefore_6 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Threshold, 50);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double Ts_value_after06 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                //   brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int TS_After_06 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 6, 0, 0, 255, 2);
                if (Ts_valuebefore_6 != Ts_value_after06 && TS_After_06 != TissueColorlargeAfter04)
                    bflag6_l = true;
                else
                    Logger.Instance.ErrorLog("Failed on bflag6_l steps ");


                //step 07 From the Tissue selection window, Adjust the Radius value by moving the slider and Click on the Apply new settings button.
                bool bflag7_l = false;
                //     brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Radius, 50);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TS_val_after06 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                //   brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                if (Ts_value_after06 != TS_val_after06)
                    bflag7_l = true;
                if (bflag7_l == false)
                    Logger.Instance.ErrorLog("Failed on bflag7_l steps ");

                //step 08 On the tissue selection tool dialog, click "Delete Selected" button.
                bool bflag8_l = false;
                //   brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //    PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteSelected);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double Ts_Value_after_08 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                //  brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                //   PageLoadWait.WaitForFrameLoad(5);
                if (Ts_Value_after_08 == 0)
                {
                    bflag8_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 8, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 8, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 8, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag8_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag8_l == false)
                    Logger.Instance.ErrorLog("Failed on bflag8_l steps ");


                //step 09 Click the "Undo Selection" button.
                bool bflag9_l = true;
                //    brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //    PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TS_value_after_09 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (TS_value_after_09 > 0)
                {
                    bflag9_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 9, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 9, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 9, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag9_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag9_l == false)
                    Logger.Instance.ErrorLog("Failed on bflag9_l steps ");

                //step 10 On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag10_l = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step10Volume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                if (TS_val_after06 == Step10Volume)
                    bflag10_l = true;
                if (bflag10_l == false)
                    Logger.Instance.ErrorLog("Failed on bflag10_l steps ");


                //step 11Click the "Redo Selection" button.
                bool bflag11_l = false;
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step11_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step11_largeVolume == 0)
                {
                    bflag11_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 11, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 11, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 11, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag11_l = true;
                    //        }
                    //    }
                    //}

                }
                if (bflag11_l == false)
                    Logger.Instance.ErrorLog("Failed on bflag11 steps ");

                //step 12 Click the "Undo Selection" button.
                bool bflag12_l = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step12_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step10Volume == Step12_largeVolume)
                {
                    bflag12_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 12, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 12, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 12, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag12_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag12_l == false)
                    Logger.Instance.ErrorLog("Failed on bflag12 steps ");

                //step 13Click the "Redo Selection" button from the view port top bar.
                bool bflag13_l = false;
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RedoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step13_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step13_largeVolume == 0)
                {
                    bflag13_l = true;
                    //int Nav1blackcolorafter12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 131, 0, 0, 0, 2);
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 13, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 13, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 13, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag13_l = true;
                    //        }
                    //    }
                    //}

                }
                if (bflag13_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag13 steps ");
                }
                //step 14Click the "undo Selection" button from the view port top bar.
                bool bflag14_l = false;
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.UndoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step15_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step15_largeVolume > 0)
                {
                    bflag14_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 14, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 14, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 14, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag14_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag14_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag14 steps ");
                }

                //step 15On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag15_l = false;
                //brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //PageLoadWait.WaitForFrameLoad(5);
                //Double Step15_largeVolume = brz3dvp.GetSelectionVolume();
                if (Step15_largeVolume == Step10Volume)
                    bflag15_l = true;
                if (bflag15_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag15 large  steps ");
                }

                //step 16 On the tissue selection tool dialog, click the "Delete Unselected" button.
                bool bflag16_l = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteUnselected);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step16_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step16_largeVolume == 0)
                {
                    bflag16_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 16, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 16, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 16, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag16_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag16_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag16_l large  steps ");
                }

                //step 17 select the Undo button from the 3D toolbox.
                //step 18On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag17_l = false;
                brz3dvp.select3DTools(Z3DTools.Undo_Segmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step17_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 17, 1);
                //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                //{

                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 17, 2);
                //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                //    {
                //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 17, 3);
                //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                //        {
                //            bflag17_l = true;
                //        }
                //    }
                //}
                //if (bflag17_l == false)
                //{
                //    Logger.Instance.ErrorLog("Failed on bflag17_l large  steps ");
                //}


                bool bflag18_l = false;

                if (Step10Volume == Step17_largeVolume)
                    bflag18_l = true; bflag17_l = true;
                if (bflag18_l == false)
                {
                    bflag17_l = false;
                    Logger.Instance.ErrorLog("Failed on bflag18_l large  steps ");
                }


                //step 19 Select the Redo button from the 3D toolbox.
                bool bflag19_l = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                Double Step19_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step19_largeVolume == 0)
                {
                    bflag19_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 19, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 19, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 19, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag19_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag19_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag19_l large  steps ");
                }

                //step 20   	On the tissue selection tool dialog, click the "Undo Selection" button.
                bool bflag20_l = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step20_largeVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);

                if (Step20_largeVolume > 0)
                {
                    bflag20_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 20, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 20, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 20, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag20_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag20_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag20_l large  steps ");
                }

                //step 21 On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag21 = false;
                //brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //PageLoadWait.WaitForFrameLoad(5);
                //Double Step21_largeVolume = brz3dvp.GetSelectionVolume();
                if (Step10Volume == Step20_largeVolume)
                    bflag21 = true;
                if (bflag21 == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag21 large  steps ");
                }


                //step 22 Select the Reset button from the 3D tool box.
                bool bflag22_l = false;
                //brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                //PageLoadWait.WaitForFrameLoad(10);
                IWebElement verifylarge = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                 //      new Actions(Driver).SendKeys("T").Build().Perform();
                 //      Thread.Sleep(500);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);

                List<string> Afterreset_large22 = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
            //    new Actions(Driver).SendKeys("T").Build().Perform();
           //     Thread.Sleep(500);
                if (Afterreset_large22[0] == Afterreset_large22[1] && Afterreset_large22[2] == Afterreset_large22[3])
                {
                    int reset_lv = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 22, 0, 0, 255, 2);
                    if(reset_lv<=200)
                        bflag22_l = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 22, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationonelarge))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 22, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwolarge))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 22, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreelarge))
                    //        {
                    //            bflag22_l = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag22_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag22_l large  steps ");
                }
             //   new Actions(Driver).SendKeys("T").Build().Perform();
            //    Thread.Sleep(500);
                if (bflag5_l && bflag6_l && bflag7_l && bflag8_l && bflag9_l && bflag10_l && bflag11_l && bflag12_l && bflag13_l
                    && bflag14_l && bflag15_l && bflag16_l && bflag17_l && bflag18_l && bflag19_l && bflag20_l && bflag21 && bflag22_l)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 25 Click on the "Bone" radio button on the Tissue Selection Tool dialog.
                IWebElement INavigationoneBone = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement INavigationtwoBone = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement INavigationthreeBone = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);

                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                IWebElement ThreshholdValueBone = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                IWebElement RadiousvalueBone = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                if (brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Bone))
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String BoneThresholdValue = ThreshholdValueBone.GetAttribute("aria-valuenow");
                    String BoneRadiousValue = RadiousvalueBone.GetAttribute("aria-valuenow");
                    if (Convert.ToInt32(BoneRadiousValue) >= 61 && Convert.ToInt32(BoneRadiousValue) >= 2000)
                        result.steps[++ExecutedSteps].status = "Pass";
                    else
                    {
                        Logger.Instance.ErrorLog("Failed in Test_163346 Step 22 while selecting Bone vessel radio button from tissue selection dialog");
                        result.steps[++ExecutedSteps].status = "Fail";
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //For Large Vessels Repeat steps 5-21
                //step 05  	Click the aorta in the MPR navigation control 1.
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                System.Drawing.Point location5 = brz3dvp.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                Cursor.Position = new System.Drawing.Point(location5.X, location5.Y);
                //this.Cursor = new Cursor(Cursor.Current.Handle);
                //Cursor.Position = new System.Drawing.Point((INavigationoneBone.Location.X + 100), (INavigationoneBone.Location.Y + 150));
                Thread.Sleep(1000);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower()=="chrome")
                {
                    int i = 0;
                    do
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 50, 0);
                        Thread.Sleep(1000);
                        i++;
                        if (i > 100) break;
                    }
                    while (brz3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2, 0, 1) <= 34);
                }
                else
                {
                    for (int i = 0; i < 44; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(5000);

                bool bflag5_B = false;
                int TissueColorboneBefore04 = brz3dvp.LevelOfSelectedColor(INavigationoneBone, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationoneBone, (INavigationoneBone.Size.Width / 2) + 18, (INavigationoneBone.Size.Height / 4) - 40).Click().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));

                int TissueColorBoneAfter04 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 52, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                double Ts_vlaBone_before5 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                if (TissueColorBoneAfter04 != TissueColorboneBefore04 && Ts_vlaBone_before5 > 0)
                    bflag5_B = true;
                else
                    Logger.Instance.ErrorLog("Failed on bflag5_B steps ");


                //step 06 From the Tissue selection window, Adjust the Threshold value by moving the slider and Click on the Apply new settings button.
                bool bflag6_B = false;
                double Ts_valueBonebefore_6 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Threshold, 50);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double Ts_valuebone_after06 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                //    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                int TS_BoneAfter_06 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 6, 0, 0, 255, 2);
                if (Ts_valueBonebefore_6 > Ts_valuebone_after06 && TS_BoneAfter_06 != TissueColorBoneAfter04)
                    bflag6_B = true;
                else
                    Logger.Instance.ErrorLog("Failed on bflag6_B steps ");


                //step 07 From the Tissue selection window, Adjust the Radius value by moving the slider and Click on the Apply new settings button.
                bool bflag7_B = false;
                //     brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //     PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "", BluRingZ3DViewerPage.Radius, 50);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Apply);
                PageLoadWait.WaitForFrameLoad(5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TS_valBone_after06 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                //     brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                if (Ts_valuebone_after06 != TS_valBone_after06)//&& TS_After_06!= Ts_color_after_07)
                    bflag7_B = true;
                if (bflag7_B == false)
                    Logger.Instance.ErrorLog("Failed on bflag7_B steps ");

                //step 08 On the tissue selection tool dialog, click "Delete Selected" button.
                bool bflag8_B = false;
                //   brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                //  new Actions(Driver).SendKeys("T").Build().Perform();
                //    Thread.Sleep(500);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteSelected);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double Ts_ValueBone_after_08 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Ts_ValueBone_after_08 == 0)
                {
                    bflag8_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 8, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 8, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 8, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag8_B = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag8_B == false)
                    Logger.Instance.ErrorLog("Failed on bflag8 Bone steps ");


                //step 09 Click the "Undo Selection" button.
                bool bflag9_B = true;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                double TS_valueBone_after_09 = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (TS_valueBone_after_09 > 0)
                {
                    bflag9_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 9, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 9, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 9, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag9_B = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag9_B == false)
                    Logger.Instance.ErrorLog("Failed on bflag9_B steps ");

                //step 10 On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag10_B = false;

                if (TS_valBone_after06 == TS_valueBone_after_09)
                    bflag10_B = true;
                if (bflag10_B == false)
                    Logger.Instance.ErrorLog("Failed on bflag10_B steps ");


                //step 11Click the "Redo Selection" button.
                bool bflag11_B = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step11_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step11_BoneVolume == 0)
                {
                    bflag11_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 11, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 11, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 11, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag11_B = true;
                    //        }
                    //    }
                    //}

                }
                if (bflag11_B == false)
                    Logger.Instance.ErrorLog("Failed on bflag11_B steps ");

                //step 12 Click the "Undo Selection" button.
                bool bflag12_B = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step12_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (TS_valueBone_after_09 == Step12_BoneVolume)
                {
                    bflag12_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 12, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 12, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 12, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag12_B = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag12_B == false)
                    Logger.Instance.ErrorLog("Failed on bflag12_B steps ");

                //step 13Click the "Redo Selection" button from the view port top bar.
                bool bflag13_B = false;
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RedoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step13_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step13_BoneVolume == 0)
                {
                    bflag13_B = true;
                    //int Nav1blackcolorafter12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 131, 0, 0, 0, 2);
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 13, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 13, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 13, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag13_B = true;
                    //        }
                    //    }
                    //}

                }
                if (bflag13_B == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag13 steps ");
                }
                //step 14Click the "undo Selection" button from the view port top bar.
                bool bflag14_B = false;
                brz3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.UndoSegmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step15_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step15_BoneVolume > 0)
                {
                    bflag14_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 14, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 14, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 14, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag14_B = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag14_B == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag14_B steps ");
                }

                //step 15On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag15_B = false;
                if (Step15_BoneVolume == TS_valueBone_after_09)
                    bflag15_B = true;
                if (bflag15_B == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag15 Bone  steps ");
                }

                //step 16 On the tissue selection tool dialog, click the "Delete Unselected" button.
                bool bflag16_B = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.DeleteUnselected);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step16_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step16_BoneVolume == 0)
                {
                    bflag16_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 16, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 16, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 16, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag16_B = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag16_B == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag16_l Bone  steps ");
                }

                //step 17 select the Undo button from the 3D toolbox.
                //step 18On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag17_B = false;
                brz3dvp.select3DTools(Z3DTools.Undo_Segmentation);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step17_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 17, 1);
                //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                //{
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 17, 2);
                //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                //    {
                //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 17, 3);
                //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                //        {
                //            bflag17_B = true;
                //        }
                //    }
                //}
                //if (bflag17_B == false)
                //{
                //    Logger.Instance.ErrorLog("Failed on bflag17_l Bone  steps ");
                //}

                //step 18On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag18_B = false;

                if (TS_valueBone_after_09 == Step17_BoneVolume)
                    bflag18_B = true; bflag17_B = true;
                if (bflag18_B == false)
                {
                    bflag17_B = false;
                    Logger.Instance.ErrorLog("Failed on bflag18_l Bone  steps ");
                }


                //step 19 Select the Redo button from the 3D toolbox.
                bool bflag19_B = false;
                brz3dvp.select3DTools(Z3DTools.Redo_Segmentation);
                PageLoadWait.WaitForFrameLoad(15);
                //  brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Redo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                Double Step19_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);
                if (Step19_BoneVolume == 0)
                {
                    bflag19_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 19, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 19, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 19, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag19_B = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag19_B == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag19_l Bone  steps ");
                }

                //step 20   	On the tissue selection tool dialog, click the "Undo Selection" button.
                bool bflag20_B = false;
                brz3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Undo");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                Double Step20_BoneVolume = brz3dvp.GetSelectionVolume();
                PageLoadWait.WaitForFrameLoad(15);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.Close);
                PageLoadWait.WaitForFrameLoad(5);

                if (Step20_BoneVolume > 0)
                {
                    bflag20_B = true;
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 20, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 20, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 20, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag20_B = true;
                    //        }
                    //    }
                    //}
                }
                if (bflag20_l == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag20_l Bone  steps ");
                }

                //step 21 On the tissue selection dialog note the calculated volume at the bottom.
                bool bflag21_B = false;
                if (TS_valueBone_after_09 == Step20_BoneVolume)
                    bflag21_B = true;
                if (bflag21_B == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag21 Bone  steps ");
                }


                //step 22 Select the Reset button from the 3D tool box.
                bool bflag22_B = false;
                IWebElement verifybone = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
              //      new Actions(Driver).SendKeys("T").Build().Perform();
              //      Thread.Sleep(500);

                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> Afterreset_Bone22 = brz3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
           //     new Actions(Driver).SendKeys("T").Build().Perform();
           //     Thread.Sleep(500);
                if (Afterreset_Bone22[0] == Afterreset_Bone22[1] && Afterreset_Bone22[2] == Afterreset_Bone22[3])
                {
                    int reset_bone = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 6, 0, 0, 255, 2);
                    //result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 22, 1);
                    //if (CompareImage(result.steps[ExecutedSteps], INavigationoneBone))
                    //{
                    //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 22, 2);
                    //    if (CompareImage(result.steps[ExecutedSteps], INavigationtwoBone))
                    //    {
                    //        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 22, 3);
                    //        if (CompareImage(result.steps[ExecutedSteps], INavigationthreeBone))
                    //        {
                    //            bflag22_B = true;
                    //        }
                    //    }
                    //}
                    if(reset_bone<=200) bflag22_B = true;
                }
                if (bflag22_B == false)
                {
                    Logger.Instance.ErrorLog("Failed on bflag22_l Bone  steps ");
                }


                if (bflag5_B && bflag6_B && bflag7_B && bflag8_B && bflag9_B && bflag10_B && bflag11_B && bflag12_B && bflag13_B
                   && bflag14_B && bflag15_B && bflag16_B && bflag17_B && bflag18_B && bflag19_B && bflag20_B && bflag21_B && bflag22_B)
                {
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }
        public TestCaseResult Test_163339(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string CursonName = TestData[0];
            string ZoomCursonName = TestData[1];
            string RotateCursonName = TestData[2];
            string RoamCursonName = TestData[3];
            string WarningMsg = TestData[4];
            String ResetValue = "Loc: 0.0, 0.0, 0.0 mm";
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer viewer2D = new BluRingViewer();
            StudyViewer StudyViewer = new StudyViewer();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to Studies tab, Search and view the study that has a 3D supported series.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                login.ClearFields();
                login.SearchStudy("patient", PatientID);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Patient ID", PatientID);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: "Patient ID", value: PatientID);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                ExecutedSteps++;
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //step:2 - Select the measure tool from the toolbox. Perform a measurement by left clicking and dragging from one point to another on the image.
                viewer.SelectViewerTool(BluRingTools.Line_Measurement);
                IWebElement Viewport2DContainer = Z3dViewerPage.ViewerContainer();
                viewer.SetViewPort(0, 1);
                viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport2DContainer))
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
                Z3dViewerPage.CloseViewer();
                //step:3  -Series loaded with out any errors in MPR 4:1 mode.
                Boolean step3 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
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

                //step:4 - Select the measurement tool from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                String step4 = Viewport[0].GetCssValue("cursor");
                if (step4.Contains(CursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - On the image displayed on navigation control 3, Perform a measurement by left clicking and dragging from one point to another on the image
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 45, (Viewport[2].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 95);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:6  -Select the zoom tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                String step6 = Viewport[2].GetCssValue("cursor");
                if (step6.Contains(ZoomCursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:7 - Click and hold the left mouse button on navigation image 3, then move the mouse upward
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 45, (Viewport[2].Size.Height / 2) - 45, (Viewport[0].Size.Width / 2) - 25, (Viewport[0].Size.Height / 2) - 65);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:8  -Click and hold the left mouse button on navigation image 3, then move the mouse downward
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(7500);
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 45, (Viewport[2].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 25, (Viewport[0].Size.Height / 2) - 65);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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
                //step:9 - Click on the reset from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step9[0] == ResetValue && step9[1] == ResetValue && step9[2] == ResetValue && step9[3] == ResetValue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 - Create more measurements. Create some on the other images displayed on the other MPR Navigation controls
                Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 45, (Viewport[1].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 45, (Viewport[2].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:11 - Push the "print screen" key to capture the screen and paste into MS Paint
                string filename = "PrintBt_153291_" + new Random().Next(1000) + ".jpg";
                Thread.Sleep(10000);
                Z3dViewerPage.CaptureScreen(filename, testid);
                Thread.Sleep(10000);
                if (File.Exists(testcasefolder + filename))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12 - On navigation control 1, hover over the rotate hotspot on left side of the red crosshair
                Viewport = Z3dViewerPage.Viewport();
                Cursor.Position = new Point((Viewport[0].Size.Width) - 18, (Viewport[0].Size.Height) + 75);
                Thread.Sleep(5000);
                String step14 = Viewport[0].GetCssValue("cursor");
                Logger.Instance.InfoLog(step14);
                if (step14.Contains(RotateCursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13 - Left click and drag the mouse in a semi-circle clockwise to rotate the crosshairs by 180 degrees
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                //
                Accord.Point p1 = Z3dViewerPage.GetIntersectionPoints(Viewport[0], testid, ExecutedSteps + 2, "red", blobval: 0);
                Accord.Point p2 = Z3dViewerPage.GetIntersectionPoints(Viewport[0], testid, ExecutedSteps + 3, "red", blobval: 1);
                int startxx = (Int32)p1.X;
                int startyy = (Int32)p1.Y;
                int endxx = (Int32)p2.X;
                int endyy = (Int32)p2.Y;
                Z3dViewerPage.Performdragdrop(Viewport[0], endxx, endyy, startxx, startyy);
                //
                //Cursor.Position = new Point((Viewport[0].Size.Width) + 100, (Viewport[0].Size.Height));
                //int startx = 0; int starty = 0; int endx = 0; int endy = 0;
                //startx = (Viewport[0].Size.Width / 2) - 116;
                //starty = Viewport[0].Size.Height / 2;
                //endx = (Viewport[0].Size.Width / 2) + 116;
                //endy = (Viewport[0].Size.Height / 2);
                //Z3dViewerPage.Performdragdrop(Viewport[0], endx, endy, startx, starty);
                Thread.Sleep(7500);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:14 - Press the reset button from 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(7500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:15 - Create more measurements. Create some on the other images displayed on the other MPR Navigation controls
                Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 45, (Viewport[1].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2) - 45, (Viewport[2].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                Thread.Sleep(7500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:16 - Push the "print screen" key to capture the screen and paste into MS Paint
                filename = "PrintBt_153291_" + new Random().Next(1000) + ".jpg";
                Thread.Sleep(10000);
                Z3dViewerPage.CaptureScreen(filename, testid);
                Thread.Sleep(10000);
                if (File.Exists(testcasefolder + filename))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:17  -Select the roam tool from the 3D toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Pan);
                String step17 = Viewport[0].GetCssValue("cursor");
                if (step17.Contains(RoamCursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step18  -Click and hold the left mouse button on navigation image 3, then move the mouse downward
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 55, (Viewport[0].Size.Height / 2) - 45);
                Thread.Sleep(7500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:19 - Select one of the measurement annotations by clicking on it. Click the "Delete" key on the keyboard to delete the measurement annotation
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(7500);
                Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) + 45, (Viewport[0].Size.Width / 2) - 45, (Viewport[0].Size.Height / 2) - 45);
                Actions act = new Actions(Driver);
                act.SendKeys(OpenQA.Selenium.Keys.Delete).Build().Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:20 - Draw more than 15 measurements in each MPR navigation controls.
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone)).SendKeys("x").Build().Perform();
                Thread.Sleep(10000);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 105, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 105, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 95, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 95, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 85, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 85, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 65, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 65, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 55, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 55, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 45, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 45, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) - 35, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) - 35, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 45, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 45, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 55, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 55, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 65, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 65, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 75, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 75, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 85, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 85, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 95, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 95, (Viewport[1].Size.Height / 2) - 45);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 105, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 105, (Viewport[1].Size.Height / 2) - 45);
                //Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2) + 115, (Viewport[1].Size.Height / 2) + 45, (Viewport[1].Size.Width / 2) + 115, (Viewport[1].Size.Height / 2) + 45);
                Z3dViewerPage.MoveAndClick(Viewport[1], Viewport[1].Size.Width / 2, Viewport[1].Size.Height / 2);
				Thread.Sleep(10000);
                IWebElement step20 = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warningmsg));
                if(step20.Text.Contains(WarningMsg))
                {
                    Z3dViewerPage.checkerrormsg("y");
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

        public TestCaseResult Test_163340(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string Thicknessvalue = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Login to iCA with privileged user.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy("patientID", PatientID);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Accession", "");
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: "Accession", value: "");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool step1 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.MPR, "y");
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (step1 && Viewport.Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:2  -Double click on the image in MPR navigation control 1.
                IWebElement INavgaitonone = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                 
                new Actions(Driver).SendKeys("X").Build().Perform(); Thread.Sleep(1000);
                bool step2 = Z3dViewerPage.EnableOneViewupMode(INavgaitonone);

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
                //step:3  -Adjust the Thickness on the bottom left hand corner of the control to 6.0 mm.
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationone, Thicknessvalue);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:4  -Double click on the image in MPR navigation control 2.
                IWebElement Inavigationtwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool step4 = Z3dViewerPage.EnableOneViewupMode(Inavigationtwo);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:5  -Double click on the image in MPR navigation control 3.
                IWebElement Inavigationtthree = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                bool step5 = Z3dViewerPage.EnableOneViewupMode(Inavigationtthree);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
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

        public TestCaseResult Test_163342(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string CursonName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Login to iCA with privileged user.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (step1 && Viewport.Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:2  -Hover the mouse over the center of the cross hairs of navigation control 1
                Thread.Sleep(2000);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                Cursor.Position = new Point((ViewerContainer.Size.Width / 2) + 90, (ViewerContainer.Size.Height) / 2);
                Thread.Sleep(1500);
                String step3 = Viewport[0].GetCssValue("cursor");
                if (step3.Contains(CursonName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //step:3 - On the MPR Result control, select "Navigation 1" to be the source control from the drop down list
                Z3dViewerPage.SelectNavigation(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:4 - Click and hold the left mouse button while hovering over the center of the cross hairs, then move the mouse.
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) - 75);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:5 - On navigation control 1, click and hold the left mouse button while hovering over the center of the blue and red cross hairs, move the mouse up and down.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:6 - On navigation control 1, click and hold the left mouse button while hovering over the center of the blue and red cross hairs, move the mouse left and right.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) + 75, (Viewport[0].Size.Height / 2));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:7 -On navigation control 3, click and hold the left mouse button while hovering over the center of the blue and yellow cross hairs, move the mouse up and down.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2), (Viewport[2].Size.Height / 2) + 75);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:8 - On navigation control 1, click and hold for a couple of seconds and then click on another position on the image
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                ClickElement(Viewport[0]);
                var action = new Actions(Driver);
                action.ClickAndHold(Viewport[0]).Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                action.MoveToElement(Viewport[0], (Viewport[0].Size.Width / 2) + 75, (Viewport[0].Size.Height / 2) + 50)
                         .Release()
                         .Build()
                         .Perform();
                //Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2) + 75, (Viewport[0].Size.Height / 2) + 50);
                Thread.Sleep(10000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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

        public TestCaseResult Test_163344(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string Rendermode1 = TestData[0];
            string Rendermode2 = TestData[1];
            string Rendermode3 = TestData[2];
            string Rendermode4 = TestData[3];
            string Cursor = TestData[4];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Navigate to 3D tab and Click Six up mode from the dropdown.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                if (step1 && Viewport.Count == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:2  -Click on the render modes drop down list displayed at the bottom left corner of the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, "none");
               // PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> RenderModes = Z3dViewerPage.RenderModes();
                if (RenderModes[0].Text.Contains(Rendermode1) && RenderModes[1].Text.Contains(Rendermode2) && RenderModes[2].Text.Contains(Rendermode3))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);

                //step: 3 -Select the first render mode (MIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Mip);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:4 - Select the scroll tool from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                String step4 = Viewport[0].GetCssValue("cursor");
                if (step4.Contains(Cursor))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Scroll through the images displayed on the MPR navigation controls
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2), (Viewport[2].Size.Height / 2) + 75, (Viewport[2].Size.Width / 2) - 75, (Viewport[2].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:6  -Click on the reset button from the 3D toolbox.
                string step6_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step6_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                if (step6_before != step6_after && step6_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step: 7 -Select the second render mode (MinIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
               
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:8 - Scroll through the images displayed on the MPR navigation controls
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 90);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2), (Viewport[2].Size.Height / 2) + 75, (Viewport[2].Size.Width / 2) - 75, (Viewport[2].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:9  -Click on the reset button from the 3D toolbox.
                string Step10_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step10_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                if (Step10_before != step10_after && step10_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step: 10 -Select the third render mode (MinIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Average);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:11 - Scroll through the images displayed on the MPR navigation controls
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[2].Size.Width / 2), (Viewport[2].Size.Height / 2) + 75, (Viewport[2].Size.Width / 2) - 75, (Viewport[2].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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


                //step:12  -Click on the reset button from the 3D toolbox.
                string step14_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step14_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                if (step14_before != step14_after && step14_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step:13 - Select the 3D 6:1 layout from the smart view drop down.
                //   ClickElement(Z3dViewerPage.ExitIcon());
                //    Thread.Sleep(4500);
                //    Boolean step17 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.Three_3d_6);
                bool step17 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                Viewport = Z3dViewerPage.Viewport();
                if (step17 && Viewport.Count == 6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - Repeat steps 2-12 on all of the MPR navigation controls
               
                //step:2  -Click on the render modes drop down list displayed at the bottom left corner of the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, "none");
                PageLoadWait.WaitForFrameLoad(10);
                RenderModes = Z3dViewerPage.RenderModes();
                bool bflag14_2 = false;
                if (RenderModes[0].Text.Contains(Rendermode1) && RenderModes[1].Text.Contains(Rendermode2) && RenderModes[2].Text.Contains(Rendermode3))
                {
                    bflag14_2 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_2 testcase failed ");
                }
                Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);

                //step: 3 -Select the first render mode (MIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Mip);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                bool bflag14_3 = false;
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag14_3 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_3 testcase failed ");
                }
                //step:4 - Select the scroll tool from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                String step14_4 = Viewport[0].GetCssValue("cursor");
                bool bflag14_4 = false;
                if (step14_4.Contains(Cursor))
                {
                    bflag14_4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_4 testcase failed ");
                }

                //step:5 - Scroll through the images displayed on the MPR navigation controls
             //   new Actions(Driver).SendKeys("X").Build().Perform();
              //  Thread.Sleep(1000);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[2], (Viewport[3].Size.Width / 2), (Viewport[3].Size.Height / 2) + 75, (Viewport[3].Size.Width / 2) - 75, (Viewport[3].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                bool bflag14_5 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag14_5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_4 testcase failed ");
                }

                //step:6  -Click on the reset button from the 3D toolbox.
            
                string step14_6_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step14_6_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                bool bflag14_6 = false;
                if (step14_6_before != step14_6_after && step14_6_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    bflag14_6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 146 testcase failed ");
                }

                //step: 7 -Select the second render mode (MinIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                PageLoadWait.WaitForFrameLoad(10);
                bool bflag14_7 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag14_7 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_7 testcase failed ");
                }

                //step:8 - Scroll through the images displayed on the MPR navigation controls
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[2].Size.Width / 2), (Viewport[3].Size.Height / 2) + 75, (Viewport[3].Size.Width / 2) - 75, (Viewport[3].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                bool bflag14_8 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag14_8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_8 testcase failed ");
                }

                //step:9  -Click on the reset button from the 3D toolbox.
                string Step14_9_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step14_9_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                bool bflag14_9 = false;
                if (Step14_9_before != step14_9_after && step14_9_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    bflag14_9 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_9 testcase failed ");
                }

                //step: 10 -Select the third render mode (MinIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Average);
                PageLoadWait.WaitForFrameLoad(10);
                bool bflag14_10 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 10);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag14_10 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 14_10 testcase failed ");
                }

                //step:11 - Scroll through the images displayed on the MPR navigation controls
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[2].Size.Width / 2), (Viewport[3].Size.Height / 2) + 75, (Viewport[3].Size.Width / 2) - 75, (Viewport[3].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                bool bflag14_11 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 11);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag14_11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step bflag14_11 testcase failed ");
                }


                //step:12  -Click on the reset button from the 3D toolbox.
                string step14_12_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step14_12_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                bool bflag14_12 = false;
                if (step14_12_before != step14_12_after && step14_12_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    bflag14_12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step bflag14_12 testcase failed ");
                }
                if(bflag14_2 && bflag14_3 && bflag14_4 && bflag14_5 && bflag14_6 && bflag14_7 && bflag14_8 && bflag14_9 && bflag14_10 && bflag14_11 && bflag14_12)
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

                //step:15 - Select the Curved MPR latout from the smart view drop down
                //   ClickElement(Z3dViewerPage.ExitIcon());
                //Thread.Sleep(4500);
                //Boolean step19 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.CurvedMPR);
                bool step19 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, "y");
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

                //step:16- Repeat steps 2-16 on all of the MPR navigation controls
              
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, "none");
                PageLoadWait.WaitForFrameLoad(10);
                bool bflag16_2 = false;
                RenderModes = Z3dViewerPage.RenderModes();
                if (RenderModes[0].Text.Contains(Rendermode1) && RenderModes[1].Text.Contains(Rendermode2) && RenderModes[2].Text.Contains(Rendermode3))
                {
                    bflag16_2 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_2 testcase failed ");
                }
                Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);

                //step: 3 -Select the first render mode (MIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Mip);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                ViewerContainer = Z3dViewerPage.ViewerContainer();
                bool bflag16_3 = false;
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag16_3 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_3 testcase failed ");
                }
                //step:4 - Select the scroll tool from the 3D toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                String step16_4 = Viewport[0].GetCssValue("cursor");
                bool bflag16_4 = false;
                if (step16_4.Contains(Cursor))
                {
                    bflag16_4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_4 testcase failed ");
                }

                //step:5 - Scroll through the images displayed on the MPR navigation controls
          //      new Actions(Driver).SendKeys("X").Build().Perform();
         //       Thread.Sleep(1000);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[2].Size.Width / 2), (Viewport[3].Size.Height / 2) + 75, (Viewport[3].Size.Width / 2) - 75, (Viewport[3].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                bool bflag16_5 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag16_5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_4 testcase failed ");
                }

                //step:6  -Click on the reset button from the 3D toolbox.
                string step16_6_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step16_6_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                bool bflag16_6 = false;
                if (step16_6_before != step16_6_after && step16_6_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    bflag16_6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 166 testcase failed ");
                }

                //step: 7 -Select the second render mode (MinIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.MinIp);
                PageLoadWait.WaitForFrameLoad(10);
                bool bflag16_7 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag16_7 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_7 testcase failed ");
                }

                //step:8 - Scroll through the images displayed on the MPR navigation controls
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[3].Size.Width / 2), (Viewport[3].Size.Height / 2) + 75, (Viewport[3].Size.Width / 2) - 75, (Viewport[3].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                bool bflag16_8 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag16_8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_8 testcase failed ");
                }

                //step:9  -Click on the reset button from the 3D toolbox.
                string Step16_9_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step16_9_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                bool bflag16_9 = false;
                if (Step16_9_before != step16_9_after && step16_9_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    bflag16_9 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_9 testcase failed ");
                }

                //step: 10 -Select the third render mode (MinIP) on the drop down list to apply to the volume displayed on the MPR navigation controls
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Average);
                PageLoadWait.WaitForFrameLoad(10);
                bool bflag16_10 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 10);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag16_10 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step 16_10 testcase failed ");
                }

                //step:11 - Scroll through the images displayed on the MPR navigation controls
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Performdragdrop(Viewport[0], (Viewport[0].Size.Width / 2), (Viewport[0].Size.Height / 2) + 75, (Viewport[0].Size.Width / 2) - 75, (Viewport[0].Size.Height / 2) - 75);
                Thread.Sleep(7500);
                Z3dViewerPage.Performdragdrop(Viewport[1], (Viewport[1].Size.Width / 2), (Viewport[1].Size.Height / 2) + 75, (Viewport[1].Size.Width / 2) - 75, (Viewport[1].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                Z3dViewerPage.Performdragdrop(Viewport[3], (Viewport[2].Size.Width / 2), (Viewport[3].Size.Height / 2) + 75, (Viewport[3].Size.Width / 2) - 75, (Viewport[3].Size.Height / 2) - 75);
                Thread.Sleep(3000);
                bool bflag16_11 = false;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 11);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
                {
                    bflag16_11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step bflag16_11 testcase failed ");
                }


                //step:12  -Click on the reset button from the 3D toolbox.
                string step16_12_before = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                string step16_12_after = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(7500);
                bool bflag16_12 = false;
                if (step16_12_before != step16_12_after && step16_12_after == "Loc: 0.0, 0.0, 0.0 mm")
                {
                    bflag16_12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("step bflag16_12 testcase failed ");
                }
                if (bflag16_2 && bflag16_3 && bflag16_4 && bflag16_5 && bflag16_6 && bflag16_7 && bflag16_8 && bflag16_9 && bflag16_10 && bflag16_11 && bflag16_12)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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


        public TestCaseResult Test_163345(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string srestvalue = split_testdata[4];


            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            try
            {
                //Step 1 Log on to iCA with valid credentials.
                //step 2 Navigate to studies tab, Search and load a 3D supported study.
                //Step 3 Select a 3D supported series and Select the MPR option from the drop down.

                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);

                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(5000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 4 Double click on the image in MPR navigation control 1.
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool bflag4 = z3dvp.EnableOneViewupMode(INavigationone);
                if (bflag4)
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                IWebElement ithumnail = z3dvp.IthumNail();
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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


                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.

                List<string> before_result6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result6[0] != result4[0] && before_result6[0] != result4[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool btool7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool btool9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                if (btool9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    Actions act8 = new Actions(Driver);
                    act8.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationone, INavigationone.Size.Width / 2 + 100, INavigationone.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
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


                //Step 10 Select the Reset button from the 3D tool box.
                bool btool10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                //Before Action 
                List<string> Beforeresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
              .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
              .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[0] == result12[3] && Beforeresult12[2] != result12[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Stepp 13 Select the Reset button from the 3D tool box.
                bool btool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                //  new Actions(Driver).SendKeys("X").Build().Perform();
                //    new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                if (btool13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14 for navigation2 Repeat steps 4-13 on all MPR navigation controls.

                //Step 4 Double click on the image in MPR navigation control 1.
                Thread.Sleep(5000);
                IWebElement INavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool bflagnavtwo4 = false;
                bool bnavigationtwo4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo));
                if (bnavigationtwo4)
                {

                    bflagnavtwo4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavtwo5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)))
                {
                    bflagnavtwo5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resulttwo6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(2000);
                //  new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                /**new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);*/
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, 10).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height - 10)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(5000);
               /* new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();*/

                bool bflagnavtwo6 = false;
                List<string> resulttwo4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (before_resulttwo6[1] == resulttwo4[1])
                {
                    bflagnavtwo6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavtwo7 = false;
                bool btooltwo7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavtwo9 = false;
                bool btooltwo9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
                        bflagnavtwo9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavtwo10 = false;
                bool btooltwo10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavtwo12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                //Before Action 
                List<string> Beforeresulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resulttwo12[0] == resulttwo12[3] && Beforeresulttwo12[1] != resulttwo12[1] && resulttwo12[0] == resulttwo12[2])
                {
                    bflagnavtwo12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavtwo13 = false;
                bool btooltwo13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                //   new Actions(Driver).SendKeys("X").Build().Perform();
                //     new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                Thread.Sleep(1000);
                if (btooltwo13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                //Step 14 for Navigationthree

                IWebElement INavigationthree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);

                bool bflagnavthree4 = false;
                //int AfterNavigationWidthtwo4 = INavigationtwo.Size.Width;
                bool bnavigationtthree4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree));
                if (bnavigationtthree4)
                {

                    bflagnavthree4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavthree5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)))
                {
                    bflagnavthree5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resultthree6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                Thread.Sleep(1000);
                //  new Actions(Driver).SendKeys("X").Build().Perform();
                //   new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).SendKeys("X").Build().Perform();
              //  Thread.Sleep(2000);
              //  new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
              //.MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2 + 100)
              //.Release()
              //.Build()
              //.Perform();
              //  Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, 10).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height - 10)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(5000);
                bool bflagnavthree6 = false;
                List<string> resultthree4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (before_resultthree6[2] == resultthree4[2])
                {
                    bflagnavthree6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavthree7 = false;
                bool btoolthree7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 11);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavthree9 = false;
                bool btoolthree9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[2] != result9[2])
                    {
                        bflagnavthree9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavthree10 = false;
                bool btoolthree10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 12);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 13);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavthree12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                //Before Action 
                List<string> Beforeresultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resultthree12[0] == resultthree12[3] && Beforeresultthree12[2] != resultthree12[2])
                {
                    bflagnavthree12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavthree13 = true;
                bool btoolthree13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 14);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                if (bflagnavtwo4 && bflagnavtwo5 && bflagnavtwo6 && bflagnavtwo7 && bflagnavtwo8 && bflagnavtwo9 && bflagnavtwo10 && bflagnavtwo11 && bflagnavtwo12 && bflagnavtwo12
                     && bflagnavthree4 && bflagnavthree5 && bflagnavthree6 && bflagnavthree7 && bflagnavthree8 && bflagnavthree9 && bflagnavthree10 && bflagnavthree11 && bflagnavthree12 && bflagnavthree13)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                login.Logout();
            }
        }

        public TestCaseResult Test_163343(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string PatientID2 = TestData[0];
            string ThumbnailDescription2 = TestData[1];
            string abdomen = TestData[2];
            string bone = TestData[3];
            string bonebody = TestData[4];
            string brain = TestData[5];
            string bronchial = TestData[6];
            string liver = TestData[7];
            string lung = TestData[8];
            string mediastinum = TestData[9];
            string pfossa = TestData[10];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
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

                //step:2 - Click on the Options at the top right corner of the navigation control 1/ navigation control 2/ navigation control 3 .
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, "none", "Preset");
                PageLoadWait.WaitForFrameLoad(3);
                IList<IWebElement> RenderModes = Z3dViewerPage.RenderModes();
                Boolean step2_1 = RenderModes[0].Text.Contains(BluRingZ3DViewerPage.Abdomen);
                Boolean step2_2 = RenderModes[1].Text.Contains(BluRingZ3DViewerPage.Bone);
                Boolean step2_3 = RenderModes[2].Text.Contains(BluRingZ3DViewerPage.BoneBody);
                Boolean step2_4 = RenderModes[3].Text.Contains(BluRingZ3DViewerPage.Brain);
                Boolean step2_5 = RenderModes[4].Text.Contains(BluRingZ3DViewerPage.Bronchial);
                Boolean step2_6 = RenderModes[5].Text.Contains(BluRingZ3DViewerPage.Liver);
                Boolean step2_7 = RenderModes[6].Text.Contains(BluRingZ3DViewerPage.Lung);
                Boolean step2_8 = RenderModes[7].Text.Contains(BluRingZ3DViewerPage.Mediastinum);
                Boolean step2_9 = RenderModes[8].Text.Contains(BluRingZ3DViewerPage.PFossa);
                if(step2_1 && step2_2 && step2_3 && step2_4 && step2_5 && step2_6 && step2_7 && step2_8 && step2_9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);
                //step:3 - Select each preset one by one from the list and verify
                string[] Preset3 = { BluRingZ3DViewerPage.Abdomen , BluRingZ3DViewerPage.Bone , BluRingZ3DViewerPage.BoneBody , BluRingZ3DViewerPage.Brain ,
                BluRingZ3DViewerPage.Bronchial,BluRingZ3DViewerPage.Liver,BluRingZ3DViewerPage.Lung,BluRingZ3DViewerPage.Mediastinum,BluRingZ3DViewerPage.PFossa};
              List<string> step5_1 = new List<string>();
                for (int i=0;i<Preset3.Length;i++)
                {
                    Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, Preset3[i], "Preset");
                  IList<string>  step5 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3).ToArray();
                   step5_1.Add(step5[0].ToString());
                }
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Abdomen, "Preset");
                //List<string> step5_1 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Bone, "Preset");
                //List<string> step5_2 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.BoneBody, "Preset");
                //List<string> step5_3 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Brain, "Preset");
                //List<string> step5_4 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Bronchial, "Preset");
                //List<string> step5_5 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Liver, "Preset");
                //List<string> step5_6 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Lung, "Preset");
                //List<string> step5_7 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Mediastinum, "Preset");
                //List<string> step5_8 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                //Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.PFossa, "Preset");
                //List<string> step5_9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (step5_1[0].Replace(" ", "").Equals(abdomen) && step5_1[1].Replace(" ", "").Equals(bone) && step5_1[2].Replace(" ", "").Equals(bonebody) && step5_1[3].Replace(" ", "").Equals(brain) && step5_1[4].Replace(" ", "").Equals(bronchial) &&
                    step5_1[5].Replace(" ", "").Equals(liver) && step5_1[6].Replace(" ", "").Equals(lung) && step5_1[7].Replace(" ", "").Equals(mediastinum) && step5_1[8].Replace(" ", "").Equals(pfossa))
                 {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Universal viewer should be closed and study search list page should be displayed.
                Z3dViewerPage.ExitIcon().Click();
                PageLoadWait.WaitForFrameLoad(5);
                Driver.SwitchTo().DefaultContent();Driver.SwitchTo().Frame("UserHomeFrame");
                if (Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Search and view a MR or PT study that has 3D supported series
                ExecutedSteps++;//Combined 5th and 6th step
                //step:6 - From the Universal viewer , Select a 3D supported CT series and Select the MPR option from the drop down
                Boolean step6 = Z3dViewerPage.searchandopenstudyin3D(PatientID2, ThumbnailDescription2);
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

                //step:7 - Presets are not available for MR and PT studies
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, "none", "Preset");
                PageLoadWait.WaitForFrameLoad(3);
                RenderModes = Z3dViewerPage.RenderModes();
                if (RenderModes.Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step8 - Click on the close button fromthe Global toolbar
                Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);
                Z3dViewerPage.ExitIcon().Click();
                PageLoadWait.WaitForElement(By.CssSelector("#TabText0"), WaitTypes.Exists, 60);
                Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame("UserHomeFrame");
                if (Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
                {
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
                login.Logout();
            }
        }

        public TestCaseResult Test_168881(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string srestvalue = split_testdata[4];


            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            try
            {
                //Step 1 Log on to iCA with valid credentials.
                //step 2 Navigate to studies tab, Search and load a 3D supported study.
                //Step 3 Select a 3D supported series and Select the MPR option from the drop down.

                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);

                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(5000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 4 Double click on the image in MPR navigation control 1.
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool bflag4 = z3dvp.EnableOneViewupMode(INavigationone);
                if (bflag4)
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                IWebElement ithumnail = z3dvp.IthumNail();
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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


                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.

                List<string> before_result6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result6[0] != result4[0] && before_result6[0] != result4[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool btool7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool btool9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                if (btool9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    Actions act8 = new Actions(Driver);
                    act8.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationone, INavigationone.Size.Width / 2 + 100, INavigationone.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
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


                //Step 10 Select the Reset button from the 3D tool box.
                bool btool10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                //Before Action 
                List<string> Beforeresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[0] == result12[3] && Beforeresult12[2] != result12[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Stepp 13 Select the Reset button from the 3D tool box.
                bool btool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(2000);
                Thread.Sleep(1000);
                if (btool13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14 for navigation2 Repeat steps 4-13 on all MPR navigation controls.

                //Step 4 Double click on the image in MPR navigation control 1.

                IWebElement INavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool bflagnavtwo4 = false;
                bool bnavigationtwo4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo));
                if (bnavigationtwo4)
                {

                    bflagnavtwo4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavtwo5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)))
                {
                    bflagnavtwo5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resulttwo6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(2000);
                //  new Actions(Driver).SendKeys("X").Build().Perform();
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(2000);

                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();
Thread.Sleep(10000);                bool bflagnavtwo6 = false;
                List<string> resulttwo4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (resulttwo4[0] == resulttwo4[3] && before_resulttwo6[1] != resulttwo4[1])
                {
                    bflagnavtwo6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavtwo7 = false;
                bool btooltwo7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavtwo9 = false;
                bool btooltwo9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
                        bflagnavtwo9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavtwo10 = false;
                bool btooltwo10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavtwo12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                //Before Action 
                List<string> Beforeresulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resulttwo12[0] == resulttwo12[3] && Beforeresulttwo12[1] != resulttwo12[1] && resulttwo12[0] == resulttwo12[2])
                {
                    bflagnavtwo12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavtwo13 = false;
                bool btooltwo13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                //   new Actions(Driver).SendKeys("X").Build().Perform();
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("x").Build().Perform();
                Thread.Sleep(2000);
                Thread.Sleep(1000);
                Thread.Sleep(1000);
                if (btooltwo13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                //Step 14 for Navigationthree

                IWebElement INavigationthree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);

                bool bflagnavthree4 = false;
                //int AfterNavigationWidthtwo4 = INavigationtwo.Size.Width;
                bool bnavigationtthree4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree));
                if (bnavigationtthree4)
                {

                    bflagnavthree4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavthree5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)))
                {
                    bflagnavthree5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resultthree6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(2000);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
              .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2 + 100)
              .Release()
              .Build()
              .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                bool bflagnavthree6 = false;
                List<string> resultthree4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (resultthree4[0] == resultthree4[3] && before_resultthree6[2] != resultthree4[2])
                {
                    bflagnavthree6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavthree7 = false;
                bool btoolthree7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 11);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavthree9 = false;
                bool btoolthree9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[2] != result9[2])
                    {
                        bflagnavthree9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavthree10 = false;
                bool btoolthree10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 12);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 13);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavthree12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                //Before Action 
                List<string> Beforeresultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resultthree12[0] == resultthree12[3] && Beforeresultthree12[2] != resultthree12[2])
                {
                    bflagnavthree12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavthree13 = true;
                bool btoolthree13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 14);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                if (bflagnavtwo4 && bflagnavtwo5 && bflagnavtwo6 && bflagnavtwo7 && bflagnavtwo8 && bflagnavtwo9 && bflagnavtwo10 && bflagnavtwo11 && bflagnavtwo12 && bflagnavtwo12
                     && bflagnavthree4 && bflagnavthree5 && bflagnavthree6 && bflagnavthree7 && bflagnavthree8 && bflagnavthree9 && bflagnavthree10 && bflagnavthree11 && bflagnavthree12 && bflagnavthree13)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                login.Logout();
            }
        }
        public TestCaseResult Test_168882(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string srestvalue = split_testdata[4];


            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            try
            {
                //Step 1 Log on to iCA with valid credentials.
                //step 2 Navigate to studies tab, Search and load a 3D supported study.
                //Step 3 Select a 3D supported series and Select the MPR option from the drop down.

                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);

                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(5000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 4 Double click on the image in MPR navigation control 1.
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool bflag4 = z3dvp.EnableOneViewupMode(INavigationone);
                if (bflag4)
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                IWebElement ithumnail = z3dvp.IthumNail();
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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


                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.

                List<string> before_result6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result6[0] != result4[0] && before_result6[0] != result4[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool btool7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool btool9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                if (btool9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    Actions act8 = new Actions(Driver);
                    act8.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationone, INavigationone.Size.Width / 2 + 100, INavigationone.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
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


                //Step 10 Select the Reset button from the 3D tool box.
                bool btool10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                //Before Action 
                List<string> Beforeresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
              .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
              .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[0] == result12[3] && Beforeresult12[2] != result12[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Stepp 13 Select the Reset button from the 3D tool box.
                bool btool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                if (btool13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14 for navigation2 Repeat steps 4-13 on all MPR navigation controls.

                //Step 4 Double click on the image in MPR navigation control 1.

                IWebElement INavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool bflagnavtwo4 = false;
                bool bnavigationtwo4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo));
                if (bnavigationtwo4)
                {

                    bflagnavtwo4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavtwo5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)))
                {
                    bflagnavtwo5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resulttwo6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                /*new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();
                Thread.Sleep(10000);*/
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, 10).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height - 10)
               .Release()
               .Build()
               .Perform();

                bool bflagnavtwo6 = false;
                List<string> resulttwo4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (before_resulttwo6[1] == resulttwo4[1])
                {
                    bflagnavtwo6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavtwo7 = false;
                bool btooltwo7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavtwo9 = false;
                bool btooltwo9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
                        bflagnavtwo9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavtwo10 = false;
                bool btooltwo10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavtwo12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                //Before Action 
                List<string> Beforeresulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resulttwo12[0] == resulttwo12[3] && Beforeresulttwo12[1] != resulttwo12[1] && resulttwo12[0] == resulttwo12[2])
                {
                    bflagnavtwo12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavtwo13 = false;
                bool btooltwo13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                Thread.Sleep(1000);
                if (btooltwo13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                //Step 14 for Navigationthree

                IWebElement INavigationthree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);

                bool bflagnavthree4 = false;
                //int AfterNavigationWidthtwo4 = INavigationtwo.Size.Width;
                bool bnavigationtthree4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree));
                if (bnavigationtthree4)
                {

                    bflagnavthree4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavthree5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)))
                {
                    bflagnavthree5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resultthree6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                /*new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
              .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2 + 100)
              .Release()
              .Build()
              .Perform();
                Thread.Sleep(10000);*/
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, 10).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height - 10)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                bool bflagnavthree6 = false;
                List<string> resultthree4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (before_resultthree6[2] == resultthree4[2])
                {
                    bflagnavthree6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavthree7 = false;
                bool btoolthree7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 11);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavthree9 = false;
                bool btoolthree9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[2] != result9[2])
                    {
                        bflagnavthree9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavthree10 = false;
                bool btoolthree10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 12);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 13);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavthree12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                //Before Action 
                List<string> Beforeresultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resultthree12[0] == resultthree12[3] && Beforeresultthree12[2] != resultthree12[2])
                {
                    bflagnavthree12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavthree13 = true;
                bool btoolthree13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 14);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                if (bflagnavtwo4 && bflagnavtwo5 && bflagnavtwo6 && bflagnavtwo7 && bflagnavtwo8 && bflagnavtwo9 && bflagnavtwo10 && bflagnavtwo11 && bflagnavtwo12 && bflagnavtwo12
                     && bflagnavthree4 && bflagnavthree5 && bflagnavthree6 && bflagnavthree7 && bflagnavthree8 && bflagnavthree9 && bflagnavthree10 && bflagnavthree11 && bflagnavthree12 && bflagnavthree13)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                login.Logout();
            }
        }
        public TestCaseResult Test_168883(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer viewer = new BluRingViewer();


            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String sPatientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String Testdata_req = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] split_testdata = Testdata_req.Split('|');
            string sHeadingPatientID = split_testdata[0];
            string sHeadingAccession = split_testdata[1];
            string sAccessionValue = split_testdata[2];
            string sThickness2 = split_testdata[3];
            string srestvalue = split_testdata[4];


            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            try
            {
                //Step 1 Log on to iCA with valid credentials.
                //step 2 Navigate to studies tab, Search and load a 3D supported study.
                //Step 3 Select a 3D supported series and Select the MPR option from the drop down.

                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy(sHeadingPatientID, sPatientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(sHeadingAccession, sAccessionValue);
                PageLoadWait.WaitForFrameLoad(5);

                var viewer1 = BluRingViewer.LaunchBluRingViewer(fieldname: sHeadingAccession, value: sAccessionValue);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool res = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "n");
                Thread.Sleep(5000);
                bool bflag = false;
                if (res == true)
                {
                    List<string> sAnnotaion = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 1);
                    if (sAnnotaion[0] == sAccessionValue && sAnnotaion[1] == sAccessionValue && sAnnotaion[2] == sAccessionValue && sAnnotaion[3] == sAccessionValue)
                    {
                        bflag = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag == false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 4 Double click on the image in MPR navigation control 1.
                IList<IWebElement> Viewport2 = z3dvp.Viewport();
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool bflag4 = z3dvp.EnableOneViewupMode(INavigationone);
                if (bflag4)
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                Thread.Sleep(1000);
                IWebElement ithumnail = z3dvp.IthumNail();
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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


                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.

                List<string> before_result6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                bool btool4 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2 + 100)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result4[0] == result4[3] && before_result6[0] != result4[0] && before_result6[0] != result4[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool btool7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool btool9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                if (btool9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    Actions act8 = new Actions(Driver);
                    act8.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationone, INavigationone.Size.Width / 2 + 100, INavigationone.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(10000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
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


                //Step 10 Select the Reset button from the 3D tool box.
                bool btool10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                if (btool10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationone, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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

                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool btool12 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationone);
                //Before Action 
                List<string> Beforeresult12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                Actions act12 = new Actions(Driver);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                act12.MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).ClickAndHold()
              .MoveToElement(INavigationone, INavigationone.Size.Width / 2 - 100, INavigationone.Size.Height / 2)
              .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> result12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (result12[0] == result12[3] && Beforeresult12[2] != result12[2])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Stepp 13 Select the Reset button from the 3D tool box.
                bool btool13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                //  new Actions(Driver).SendKeys("X").Build().Perform();
                //    new Actions(Driver).MoveToElement(INavigationone, INavigationone.Size.Width / 2, INavigationone.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                if (btool13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone))
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14 for navigation2 Repeat steps 4-13 on all MPR navigation controls.

                //Step 4 Double click on the image in MPR navigation control 1.

                IWebElement INavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool bflagnavtwo4 = false;
                bool bnavigationtwo4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo));
                if (bnavigationtwo4)
                {

                    bflagnavtwo4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavtwo5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)))
                {
                    bflagnavtwo5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resulttwo6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(2000);
                //  new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
               /* new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                  .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
                  .Release()
                  .Build()
                  .Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2 + 100)
                .Release()
                .Build()
                .Perform();*/
                
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, 10).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height - 10)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                bool bflagnavtwo6 = false;
                List<string> resulttwo4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (before_resulttwo6[1] == resulttwo4[1])
                {
                    bflagnavtwo6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavtwo7 = false;
                bool btooltwo7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavtwo9 = false;
                bool btooltwo9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 + 100, INavigationtwo.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[1] != result9[1])
                    {
                        bflagnavtwo9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavtwo10 = false;
                bool btooltwo10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                if (btooltwo10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavtwo11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                {
                    bflagnavtwo11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavtwo12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationtwo);
                //Before Action 
                List<string> Beforeresulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2 - 100, INavigationtwo.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resulttwo12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resulttwo12[0] == resulttwo12[3] && Beforeresulttwo12[1] != resulttwo12[1] && resulttwo12[0] == resulttwo12[2])
                {
                    bflagnavtwo12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavtwo13 = false;
                bool btooltwo13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                //   new Actions(Driver).SendKeys("X").Build().Perform();
                //     new Actions(Driver).MoveToElement(INavigationtwo, INavigationtwo.Size.Width / 2, INavigationtwo.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                Thread.Sleep(1000);
                if (btooltwo13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationtwo))
                    {
                        bflagnavtwo13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                //Step 14 for Navigationthree

                IWebElement INavigationthree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);

                bool bflagnavthree4 = false;
                //int AfterNavigationWidthtwo4 = INavigationtwo.Size.Width;
                bool bnavigationtthree4 = z3dvp.EnableOneViewupMode(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree));
                if (bnavigationtthree4)
                {

                    bflagnavthree4 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 4 fail ");
                }


                //Step 5 Enter the thickness value to 100.0 mm under the hover bar menu.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                bool bflagnavthree5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 7);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)))
                {
                    bflagnavthree5 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 5 fail ");
                }



                //Step 6 Note the orientation marker displayed at the top center of the control. Select the rotate tool from the Z3D toolbar. Click the top center of the control and drag down slowly. Note the orientation marker as you drag down.
                List<string> before_resultthree6 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                Thread.Sleep(1000);
                //  new Actions(Driver).SendKeys("X").Build().Perform();
                //   new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).SendKeys("X").Build().Perform();
                Thread.Sleep(2000);
                /*new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
              .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2 + 100)
              .Release()
              .Build()
              .Perform();
                Thread.Sleep(10000);*/
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, 10).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height - 10)
               .Release()
               .Build()
               .Perform();
                Thread.Sleep(10000);
                bool bflagnavthree6 = false;
                List<string> resultthree4 = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (before_resultthree6[2] == resultthree4[2])
                {
                    bflagnavthree6 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 6 fail ");
                }


                //Step 7 Select the Reset button from the 3D tool box.
                bool bflagnavthree7 = false;
                bool btoolthree7 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree7)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 8);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree7 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 7 fail ");
                }
                //Step 8 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree8 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 11);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree8 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 8 fail ");
                }

                //Step 9 Note the orientation marker displayed at the left side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the right slowly. Note the left orientation marker as you drag to the right.
                bool bflagnavthree9 = false;
                bool btoolthree9 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree9)
                {
                    List<string> Beforeresult9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);

                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                    .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                    .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                 .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 + 100, INavigationthree.Size.Height / 2)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    List<string> result9 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationLeftMiddle, null, null, 0);
                    if (result9[0] == result9[3] && Beforeresult9[2] != result9[2])
                    {
                        bflagnavthree9 = true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 9 fail ");
                }

                //Step 10 Select the Reset button from the 3D tool box.
                bool bflagnavthree10 = false;
                bool btoolthree10 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree10)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 12);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree10 = true;
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 10 fail ");
                }
                //Step 11 Enter the thickness value to 100.0 mm under the hover bar menu.
                bool bflagnavthree11 = false;
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, sThickness2);
                new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 13);
                if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                {
                    bflagnavthree11 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 11 fail ");
                }
                //Step 12 Note the orientation marker displayed at the right side of the control. Select the rotate tool from the Z3D toolbar. Click on the center of the control and drag to the left slowly. Note the right orientation marker as you drag to the left.
                bool bflagnavthree12 = true;
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage.Navigationthree);
                //Before Action 
                List<string> Beforeresultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);

                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
                .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
                .Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(INavigationthree, INavigationthree.Size.Width / 2, INavigationthree.Size.Height / 2).ClickAndHold()
               .MoveToElement(INavigationthree, INavigationthree.Size.Width / 2 - 100, INavigationthree.Size.Height / 2)
               .Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> resultthree12 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightMiddle, null, null, 0);
                if (resultthree12[0] == resultthree12[3] && Beforeresultthree12[2] != resultthree12[2])
                {
                    bflagnavthree12 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 12 fail ");
                }
                //Stepp 13 Select the Reset button from the 3D tool box.
                bool bflagnavthree13 = true;
                bool btoolthree13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationthree);
                if (btoolthree13)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((Viewport2[0].Size.Width / 2) + 600, (Viewport2[0].Size.Height / 2 + 100));
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 14);
                    if (CompareImage(result.steps[ExecutedSteps], INavigationthree))
                    {
                        bflagnavthree13 = true;
                    }
                }

                else
                {
                    Logger.Instance.ErrorLog("Navigation tow step 13 fail ");
                }


                if (bflagnavtwo4 && bflagnavtwo5 && bflagnavtwo6 && bflagnavtwo7 && bflagnavtwo8 && bflagnavtwo9 && bflagnavtwo10 && bflagnavtwo11 && bflagnavtwo12 && bflagnavtwo12
                     && bflagnavthree4 && bflagnavthree5 && bflagnavthree6 && bflagnavthree7 && bflagnavthree8 && bflagnavthree9 && bflagnavthree10 && bflagnavthree11 && bflagnavthree12 && bflagnavthree13)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                login.Logout();
            }
        }

    }

}




