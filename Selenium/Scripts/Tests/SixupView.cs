using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using System.IO;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using TestStack.White.UIItems.Finders;
using System.Windows.Automation;
using TestStack.White.InputDevices;
using TestStack.White.UIItems;

namespace Selenium.Scripts.Tests
{
    class SixupView : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Cursor Cursor { get; private set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public SixupView()
        { }
        public SixupView(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }
        public TestCaseResult Test_163396(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: From the Universal viewer ,Select a 3D supported series study and Select the MPR option from the drop down.
              bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
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
                //Step 3 :: From smart view drop down select 3D 6:1 view mode.
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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
                //Step 4 :: Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the 3D navigation controls.
                bool step4 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
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
                //Step 5 :: Move the left clipping line in navigation control 1 to the center of the image.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).SendKeys("T").Build().Perform();Thread.Sleep(1000);
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step5bluclipline = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 18, "blue", "vertical", 18);
                int myInt5X = (int)Math.Ceiling(Step5bluclipline.X);
                int myInt5Y = (int)Math.Ceiling(Step5bluclipline.Y);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.PerformDragAndDropWithDelay(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, myInt5X, myInt5Y, Navigation1.Size.Width / 2, myInt5Y, 10);
                //Verification::Left side of the 3D images are cut off to the position of the left clipping line in navigation control 1.
                IWebElement Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: Move the left clipping line in navigation control 3 back to its original position.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point Step6blucliplineA = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 6, "blue", "vertical", 18);
                int myInt6X = (int)Math.Ceiling(Step6blucliplineA.X);
                int myInt6Y = (int)Math.Ceiling(Step6blucliplineA.Y);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.PerformDragAndDropWithDelay(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, myInt6X, Navigation3.Size.Height / 2, myInt5X, Navigation3.Size.Height / 2, 30);
                //Verification::3D images return to their original state.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Move the top clipping line in navigation control 1 to the center of the image.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point RedlineBefore = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 7, "red", "Horizontal", 16);
                int myInt7X = (int)Math.Ceiling(RedlineBefore.X);
                int myInt7Y = (int)Math.Ceiling(RedlineBefore.Y);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, myInt7X, myInt7Y + 3, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, 30);
                //Verification::3D images return to their original state.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8::Move the top clipping line in navigation control 2 back to its original position.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point RedlineAfter = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 8, "red", "Horizontal", 16);
                int myInt8X = (int)Math.Ceiling(RedlineAfter.X);
                int myInt8Y = (int)Math.Ceiling(RedlineAfter.Y);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, myInt8X, myInt8Y, myInt8X, myInt7Y, 30);
                //Verification::3D images return to their original state.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9::Move the right clipping line in navigation control 1 to the center of the image.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point Step9bluclipline = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 12, "blue", "vertical", 19);
                int myInt9X = (int)Math.Ceiling(Step9bluclipline.X);
                int myInt9Y = (int)Math.Ceiling(Step9bluclipline.Y);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, myInt9X, myInt9Y, myInt6X, myInt9Y, 30);
                //Verification::The right side of the 3D images are cut off to the position of the right clipping line in navigation control 1.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10::Move the right clipping line in navigation control 3 back to its original position.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point Step10bluclipline = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 32, "blue", "vertical", 19);
                int myInt10X = (int)Math.Ceiling(Step10bluclipline.X);
                int myInt10Y = (int)Math.Ceiling(Step10bluclipline.Y);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, myInt10X, myInt10Y, myInt9X, myInt9Y, 30);
                //Verification::3D images return to their original state.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 ::Move the bottom clipping line in navigation control 1 to the center of the image.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point BottomRedlineBefore = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 18, "red", "Horizontal", 45);
                int myInt11X = (int)Math.Ceiling(BottomRedlineBefore.X);
                int myInt11Y = (int)Math.Ceiling(BottomRedlineBefore.Y);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.PerformDragAndDropWithDelay(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, myInt11X, myInt11Y, myInt11X, Navigation1.Size.Height / 2, 30);
                //Verification::The bottom of the 3D images are cut off to the position of the bottom clipping line in navigation control 1.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 ::Move the bottom clipping line in navigation control 2 back to its original position.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point BottomRedlineAfter = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 16, "red", "Horizontal", 45);
                int myInt12X = (int)Math.Ceiling(BottomRedlineAfter.X);
                int myInt12Y = (int)Math.Ceiling(BottomRedlineAfter.Y);
                z3dvp.PerformDragAndDropWithDelay(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, myInt12X, myInt12Y, myInt12X, myInt11Y, 30);
                //Verification::3D images return to their original state.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: Move the left clipping line in navigation control 2 to the right until only the small section of the image remains between the left and right clipping lines.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Nav3YellowTop = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 16, "yellow", "Horizontal", 15);
                int myInt13X = (int)Math.Ceiling(Nav3YellowTop.X);
                int myInt13Y = (int)Math.Ceiling(Nav3YellowTop.Y);
                PageLoadWait.WaitForFrameLoad(5);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, myInt5X, myInt5Y, (Navigation2.Size.Width * 3) / 4, Navigation2.Size.Height / 2, 30);
                //Verification::3D images are updated so that only the small section of the image that is displayed inside the box between the clipping lines is visible.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14 :: Move the top clipping line in navigation control 3 back to its original position.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(5);
                Accord.Point Step14Yellow = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 16, "yellow", "Horizontal", 15);
                int myInt14X = (int)Math.Ceiling(Step14Yellow.X);
                int myInt14Y = (int)Math.Ceiling(Step14Yellow.Y);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, myInt14X, myInt14Y, myInt13X, myInt13Y, 30);
                //Verification::The 3D images return to their original state.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 :: Move the right clipping line in navigation control 2 to the left.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(5);
                Accord.Point Step16BeforeYellow = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 16, "yellow", "Horizontal", 45);
                int myInt16BeforeX = (int)Math.Ceiling(Step16BeforeYellow.X);
                int myInt16BeforeY = (int)Math.Ceiling(Step16BeforeYellow.Y);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point Step15Yellow = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 17, "yellow", "vertical", 19);
                int myInt15X = (int)Math.Ceiling(Step15Yellow.X);
                int myInt15Y = (int)Math.Ceiling(Step15Yellow.Y);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, myInt15X, myInt15Y, Navigation2.Size.Width / 4, myInt15Y, 30);
                //Verification::Only the section in the box between the clipping lines are displayed in the 3D images.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: Move the bottom clipping line in navigation control 3 back to its original position.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(5);
                Accord.Point Step16Yellow = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 18, "yellow", "Horizontal", 45);
                int myInt16X = (int)Math.Ceiling(Step16Yellow.X);
                int myInt16Y = (int)Math.Ceiling(Step16Yellow.Y);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.PerformDragAndDropWithDelay(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, myInt16X, myInt16Y, myInt16BeforeX, myInt16BeforeY, 30);
                //Verification::3D images return to their original state.
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation3D1))
                {
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163397(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: From the Universal viewer ,Select a 3D supported series and Select the MPR option from the drop down.
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
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
                //Step 3 :: From smart view drop down select 3D 6:1 view mode.
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
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
                //Step 4 :: Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the 3D navigation controls.
                bool step4 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
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
                //Step 5 :: select "Bone & Minimal Vessels" from drop down preset in 3D control 2.
                bool BoneAndMinimal = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2, BluRingZ3DViewerPage.Preset);
                if (BoneAndMinimal)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: Move the left clipping line in navigation control 1 to the center of the image.
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).SendKeys("T").Build().Perform();Thread.Sleep(1000);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step6bluclipline = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 1, "blue", "vertical", 18);
                int Int6X = (int)Math.Ceiling(Step6bluclipline.X);
                int Int6Y = (int)Math.Ceiling(Step6bluclipline.Y);
                //  z3dvp.PerformDragAndDropWithDelay(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, Int6X, Int6Y, Navigation1.Size.Width / 2, Int6Y, 10);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, Int6X, Int6Y).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Int6Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: Left clipping line in navigation control 3 moves to the same position as in navigation control 1.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Verificatio1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 2, "blue", "vertical", 18);
                int Verificatio1X = (int)Math.Ceiling(Verificatio1.X);
                int Verificatio1Y = (int)Math.Ceiling(Verificatio1.Y);
                IWebElement Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Verificatio2 = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 3, "blue", "vertical", 18);
                int Verificatio2X = (int)Math.Ceiling(Verificatio2.X);
                int Verificatio2Y = (int)Math.Ceiling(Verificatio2.Y);
                if (Verificatio1X.Equals(Verificatio2X) && !Int6X.Equals(Verificatio1X))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Move the left clipping line in navigation control 3 back to its original position.
                //  z3dvp.PerformDragAndDropWithDelay(Navigation3, Navigation3.Size.Width / 2, Navigation3.Size.Height / 2, Verificatio2X, Verificatio2Y, Int6X, Int6Y, 50);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, Verificatio2X, Verificatio2Y).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, Int6X, Int6Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification::Left clipping lines in navigation controls 1 and 3 move back to their original position.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step7_1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 4, "blue", "vertical", 18);
                int Step7X_1 = (int)Math.Ceiling(Step7_1.X);
                int Step7Y_1 = (int)Math.Ceiling(Step7_1.Y);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Step7_2 = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 5, "blue", "vertical", 18);
                int Step7X_2 = (int)Math.Ceiling(Step7_2.X);
                int Step7Y_2 = (int)Math.Ceiling(Step7_2.Y);
                if (Step7X_1.Equals(Step7X_2) && Step7X_1<=15 && Step7X_2<=15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8::Move the top clipping line in navigation control 1 to the center of the image.
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point RedlineBefore = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 6, "red", "Horizontal", 16);
                int myInt8X = (int)Math.Ceiling(RedlineBefore.X);
                int myInt8Y = (int)Math.Ceiling(RedlineBefore.Y);
                PageLoadWait.WaitForFrameLoad(10);
                //z3dvp.PerformDragAndDropWithDelay(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, myInt8X, myInt8Y, myInt8X, Navigation1.Size.Height / 2, 50);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, myInt8X, myInt8Y +3).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, myInt8X, Navigation1.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification::The top clipping line in navigation control 2 moves to the same position as in navigation control 1.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step8_1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 17, "red", "Horizontal", 16);
                int Step8X_1 = (int)Math.Ceiling(Step8_1.X);
                int Step8Y_1 = (int)Math.Ceiling(Step8_1.Y);
                IWebElement Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                Accord.Point Step8_2 = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 158, "red", "Horizontal", 16);
                int Step8X_2 = (int)Math.Ceiling(Step8_2.X);
                int Step8Y_2 = (int)Math.Ceiling(Step8_2.Y);
                if (Step8Y_1.Equals(Step8Y_2) && !myInt8Y.Equals(Step8Y_1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: Move the top clipping line in navigation control 2 back to its original position.
                //    z3dvp.PerformDragAndDropWithDelay(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, myInt8X, Navigation1.Size.Height / 2, myInt8X, myInt8Y, 50);
                new Actions(BasePage.Driver).MoveToElement(Navigation2, myInt8X, Navigation1.Size.Height / 2).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation2, myInt8X, myInt8Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The top clipping lines in navigation controls 1 and 2 move back to their original position.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step9_1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 9, "red", "Horizontal", 16);
                int Step9X_1 = (int)Math.Ceiling(Step9_1.X);
                int Step9Y_1 = (int)Math.Ceiling(Step9_1.Y);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                Accord.Point Step9_2 = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 10, "red", "Horizontal", 16);
                int Step9X_2 = (int)Math.Ceiling(Step9_2.X);
                int Step9Y_2 = (int)Math.Ceiling(Step9_2.Y);
                if(Step9Y_1== Step9Y_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10 :: Move the right clipping line in navigation control 1 to the center of the image.
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step10 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 1, "blue", "vertical", 19);
                int Int10X = (int)Math.Ceiling(Step10.X);
                int Int10Y = (int)Math.Ceiling(Step10.Y);
                // z3dvp.PerformDragAndDropWithDelay(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Int10X, Int10Y, Navigation1.Size.Width / 2, Int10Y, 10);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, Int10X, Int10Y).Build().Perform();
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, Int10X, Int10Y).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Int10Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The right clipping lines in navigation control 3 moves to the same position as in navigation control 1.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step10_1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 11, "blue", "vertical", 19);
                int Step10X_1 = (int)Math.Ceiling(Step10_1.X);
                int Step10Y_1 = (int)Math.Ceiling(Step10_1.Y);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Step10_2 = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 12, "blue", "vertical", 19);
                int Step10X_2 = (int)Math.Ceiling(Step10_2.X);
                int Step10Y_2 = (int)Math.Ceiling(Step10_2.Y);
                if (Step10X_1.Equals(Step10X_2) && !Int10X.Equals(Step10X_1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 :: Move the right clipping line in navigation control 3 back to its original position.
                new Actions(BasePage.Driver).MoveToElement(Navigation3, Navigation1.Size.Width / 2, Int10Y).Build().Perform();
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, Navigation1.Size.Width / 2, Int10Y).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, Int10X, Int10Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The right clipping lines in navigation controls 1 and 3 move back to their original position.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step11_1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 131, "blue", "vertical", 19);
                int Step11X_1 = (int)Math.Ceiling(Step11_1.X);
                int Step11Y_1 = (int)Math.Ceiling(Step11_1.Y);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Step11_2 = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 141, "blue", "vertical", 19);
                int Step11X_2 = (int)Math.Ceiling(Step11_2.X);
                int Step11Y_2 = (int)Math.Ceiling(Step11_2.Y);
                    if(Step11X_2== Step11X_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 :: Move the bottom clipping line in navigation control 1 to the center of the image.
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point RedlineBottom = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 15, "red", "Horizontal", 45);
                int myInt12X = (int)Math.Ceiling(RedlineBottom.X);
                int myInt12Y = (int)Math.Ceiling(RedlineBottom.Y);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, myInt12X, myInt12Y).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation1, myInt12X, Navigation1.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification::The bottom clipping line in navigation control 2 moves to the same position as in navigation control 1.
                Accord.Point Step12_1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 16, "red", "Horizontal", 45);
                int Step12X_1 = (int)Math.Ceiling(Step12_1.X);
                int Step12Y_1 = (int)Math.Ceiling(Step12_1.Y);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                Accord.Point Step12_2 = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 17, "red", "Horizontal", 45);
                int Step12X_2 = (int)Math.Ceiling(Step12_2.X);
                int Step12Y_2 = (int)Math.Ceiling(Step12_2.Y);
                if (Step12Y_1.Equals(Step12Y_2) && !myInt12Y.Equals(Step12Y_1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: Move the bottom clipping line in navigation control 2 back to its original position.
                new Actions(BasePage.Driver).MoveToElement(Navigation2, myInt12X, Navigation2.Size.Height / 2).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation2, myInt12X, myInt12Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The bottom clipping line in navigation controls 1 and 2 move back to their original position.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point Step13_1 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 18, "red", "Horizontal", 45);
                int Step13X_1 = (int)Math.Ceiling(Step13_1.X);
                int Step13Y_1 = (int)Math.Ceiling(Step13_1.Y);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Step13_2 = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 19, "red", "Horizontal", 45);
                int Step13X_2 = (int)Math.Ceiling(Step13_2.X);
                int Step13Y_2 = (int)Math.Ceiling(Step13_2.Y);
                if (Step13Y_1.Equals(Step13Y_2) && (myInt12Y.Equals(Step13Y_1 - 1) || myInt12Y>=339))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 14 :: Move the left clipping line in navigation control 2 to the right until only the small section of the image remains between the left and right clipping lines.
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point BeforeTopYellowLine = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 20, "yellow", "Horizontal", 15);
                int myInt14X = (int)Math.Ceiling(BeforeTopYellowLine.X);
                int myInt14Y = (int)Math.Ceiling(BeforeTopYellowLine.Y);
                PageLoadWait.WaitForFrameLoad(5);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                //  z3dvp.PerformDragAndDropWithDelay(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, Int6X, Int6Y, (Navigation2.Size.Width * 3) / 4, Navigation2.Size.Height / 2, 50);
                new Actions(BasePage.Driver).MoveToElement(Navigation2, Int6X, Int6Y).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation2, (Navigation2.Size.Width * 3) / 4, Navigation2.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The top clipping line in navigation control 3 moves down
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point AfterTopYellowLine = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 21, "yellow", "Horizontal", 15);
                int myInt14X_1 = (int)Math.Ceiling(AfterTopYellowLine.X);
                int myInt14Y_1 = (int)Math.Ceiling(AfterTopYellowLine.Y);
                if (!myInt14Y.Equals(myInt14Y_1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15 :: Move the top clipping line in navigation control 3 back to its original position.
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, myInt14X_1, myInt14Y_1).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, myInt14X, myInt14Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The left clipping line in navigation control 2 and top clipping line in navigation control 3 move back to their original positions.
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Step15_1 = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 23, "yellow", "Horizontal", 15);
                int myInt15X_1 = (int)Math.Ceiling(Step15_1.X);
                int myInt15Y_1 = (int)Math.Ceiling(Step15_1.Y);
                //Accord.Point Step15_2 = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 24, "yellow", "vertical", 18);
                //int myInt15X_2 = (int)Math.Ceiling(Step15_2.X);
                //int myInt15Y_2 = (int)Math.Ceiling(Step15_2.Y);
               // if (myInt15Y_1.Equals(myInt14Y + 2) || myInt15Y_1.Equals(myInt14Y + 3) && myInt15X_2.Equals(4) || myInt15X_2.Equals(5) || myInt15X_2.Equals(6))
               if(myInt14Y_1!= myInt15Y_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: Move the right clipping line in navigation control 2 to the left.
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                
                Accord.Point Step16 = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 25, "yellow", "vertical", 19);
                int Int16X = (int)Math.Ceiling(Step10.X);
                int Int16Y = (int)Math.Ceiling(Step10.Y);
                Accord.Point BeforeYellowBottom = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 1, "yellow", "Horizontal", 45);
                int Int16X_1 = (int)Math.Ceiling(BeforeYellowBottom.X);
                int Int16Y_1 = (int)Math.Ceiling(BeforeYellowBottom.Y);
                // z3dvp.PerformDragAndDropWithDelay(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, Int10X, Int10Y, Navigation2.Size.Width / 4, Navigation2.Size.Width / 4, 50);
                new Actions(BasePage.Driver).MoveToElement(Navigation2, Int10X, Int10Y).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Width / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The bottom clipping line in navigation control 3 moves above.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point AfterYellowBottom = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 26, "yellow", "Horizontal", 45);
                int Int16X_2 = (int)Math.Ceiling(AfterYellowBottom.X);
                int Int16Y_2 = (int)Math.Ceiling(AfterYellowBottom.Y);
                if (!Int16Y_1.Equals(Int16Y_2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17 :: Move the bottom clipping line in navigation control 3 back to its original position.
                //     Accord.Point Step17 = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 27, "yellow", "Horizontal", 45);
                //      int Int17X = (int)Math.Ceiling(Step17.X);
                //      int Int17Y = (int)Math.Ceiling(Step17.Y);
                //z3dvp.PerformDragAndDropWithDelay(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, Int17X, Int17Y, Int16X_1, Int16Y_1, 50);
                // z3dvp.PerformDragAndDropWithDelay(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, Int16X_2, Int16Y_2, Int16X_1, Int16Y_1,50);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, Int16X_2, Int16Y_2).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(30);
                new Actions(BasePage.Driver).MoveToElement(Navigation3, Int16X_1, Int16Y_1).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                //Verification :: The right clipping line in navigation control 2 and bottom clipping line in navigation control 3 move back to their original positions.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                Accord.Point Step17_1 = z3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 28, "yellow", "vertical", 19);
                int Int17X_1 = (int)Math.Ceiling(Step17_1.X);
                int Int17Y_1 = (int)Math.Ceiling(Step17_1.Y);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point Step17_2 = z3dvp.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 29, "yellow", "Horizontal", 45);
                int Int17X_2 = (int)Math.Ceiling(Step17_2.X);
                int Int17Y_2 = (int)Math.Ceiling(Step17_2.Y);
                if(Int17X_2== Int16X_2)
                {
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163399(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                //IWebElement LogoutBtn = login.LogoutBtn();
                //if (LogoutBtn.Displayed)
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
                //Step 2 :: From the Universal viewer ,Select a 3D supported series and Select the MPR option from the drop down.
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.Three_3d_6);
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
                //Step 3 :: Hover ComboBox in the top left corner of the result control, and select navigation 2.
                bool SelectNav2 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                if (SelectNav2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 :: Apply the zoom tool to navigation control 2
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).SendKeys("X").Build().Perform();
                string Nav1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                string Nav2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string Nav3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                string Res1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                z3dvp.Performdragdrop(Navigation2, Navigation2.Size.Width / 2, (Navigation2.Size.Width * 3) / 4, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4).ClickAndHold().
                //MoveToElement(Navigation2, Navigation2.Size.Width / 2, (Navigation2.Size.Width * 3) / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                string Step4_1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                string Step4_2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string Step4_3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                string Step4_4 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                new Actions(Driver).SendKeys("X").Build().Perform();
                if (Step4_1.Equals(Step4_2) && Step4_2.Equals(Step4_3) && Step4_3.Equals(Step4_4) && !Nav1.Equals(Step4_1) && !Nav2.Equals(Step4_2) && !Nav3.Equals(Step4_3) && !Res1.Equals(Step4_4)
                   && Nav1.Equals(Nav2) && Nav2.Equals(Nav3) && Nav3.Equals(Res1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: Press the reset button from floating toolbox.
                bool Reset = z3dvp.select3DTools(Z3DTools.Reset);
                //VErification :: Image in all views returns to original state.
                string Step5_1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (Reset && Step5_1.Equals(Nav1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6 :: Rotate the cross hairs of navigation control 2 by 180 degrees clockwise.
                z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(Navigation2, (Navigation2.Size.Width) * 3 / 4, Navigation2.Size.Height / 2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 2);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 2).ClickAndHold()
                //.MoveToElement(Navigation2, (Navigation2.Size.Width) * 3 / 4, Navigation2.Size.Height / 2).Release().Build().Perform();
                //Verification :: 
                string orientationav2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                string orientationRes1 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.ResultPanel);
                string LocationNav2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string LocationRes1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (orientationav2.Equals(orientationRes1) && LocationNav2.Equals(LocationRes1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Apply the roam tool to navigation control 2 .
                new Actions(Driver).SendKeys("X").Build().Perform();
                bool PanTool = z3dvp.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4).ClickAndHold().
                // MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2).Release().Build().Perform();
                //Verification::
                PageLoadWait.WaitForFrameLoad(10);
                string Step7_1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                string Step7_2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string Step7_3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                string Step7_4 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (PanTool && Step7_1.Equals(Step7_2) && Step7_2.Equals(Step7_3) && Step7_3.Equals(Step7_4))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: Apply the scroll tool to navigation control 2.
                bool Scroll = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4).ClickAndHold().
                // MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2).Release().Build().Perform();
                //Verification :: 
                PageLoadWait.WaitForFrameLoad(10);
                string Step8_1 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                string Step8_2 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.ResultPanel);
                string Step8_3 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string Step8_4 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (Step8_1.Equals(Step8_2) && Step8_3.Equals(Step8_4))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: Apply the window/level tool to navigation control 2.
                bool Windowlevel = z3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(10);
                String BeforeWL = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.ResultPanel);
                String BeforeWLNav2 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4).ClickAndHold().
                //MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2).Release().Build().Perform();
                //Verification :: window/level is not modified in the result control.
                PageLoadWait.WaitForFrameLoad(10);
                String AfterWL = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.ResultPanel);
                String AfterWLNav2 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                if (BeforeWL.Equals(AfterWL) && !BeforeWLNav2.Equals(AfterWLNav2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10 :: Apply the window/level tool the result control.
                IWebElement ResultControl = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                PageLoadWait.WaitForFrameLoad(10);
                String Step10_1Before = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.ResultPanel);
                String Step10_2Before = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(ResultControl, ResultControl.Size.Width / 2, (ResultControl.Size.Height / 2) - 10, ResultControl.Size.Width / 2, ResultControl.Size.Height / 4);
                //new Actions(Driver).MoveToElement(ResultControl, ResultControl.Size.Width / 2, ResultControl.Size.Height / 4).ClickAndHold().
                //MoveToElement(ResultControl, ResultControl.Size.Width / 2, (ResultControl.Size.Height / 2) - 10).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: The window/level is modified only in the result control.
                String Step10_1 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.ResultPanel);
                String Step10_2 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                if (!Step10_1Before.Equals(Step10_1) && Step10_2Before.Equals(Step10_2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11 :: Click the ComboBox in the lower left corner of the result control, and select navigation 3.
                new Actions(Driver).SendKeys("X").Build().Perform();
                z3dvp.select3DTools(Z3DTools.Reset);
                bool Step11 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel);
                if (Step11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12 :: Using the traverse tool, move the cross hair of navigation control 1 upward until the position annotation of navigation control 3 reads "0, 0, 100".
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                //  z3dvp.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2, (Navigation1.Size.Height / 4) + 20);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, (Navigation1.Size.Height / 4) + 20).ClickAndHold().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: 
                string Step12_1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                string[] Nav3split = Step12_1.Split(' ');
                string[] Nav3Valsplit = Nav3split[3].Split('.');
                string Step12_2 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] Resultsplit = Step12_2.Split(' ');
                string Step12_3 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                string Step12_4 = z3dvp.GetOrientationValue(BluRingZ3DViewerPage.ResultPanel);
                if (Step12_3.Equals(Step12_4) && Step12_1.Equals(Step12_2) && Convert.ToInt32(Nav3Valsplit[0]) >= 96)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: Adjust the thickness to be 10.0 mm on navigation control 3.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.Navigationthree, "10");
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: The thickness of all navigation controls are set to 10.0 mm but not on the result control.
                string Step13_1 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.Navigationone);
                string Step13_2 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo);
                string Step13_3 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree);
                string Step13_4 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel);
                if (Step13_1.Equals("10 mm") && Step13_2.Equals("10 mm") && Step13_3.Equals("10 mm") && !Step13_4.Equals("10 mm"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step14 :: Adjust the thickness of the result control to be 5.0 mm.
                z3dvp.EnterThickness(BluRingZ3DViewerPage.ResultPanel, "5.0");
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: ONLY the thickness of the result control is adjusted to 5.0 mm.
                string Step14_1 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree);
                string Step14_2 = z3dvp.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel);
                if (!Step14_1.Equals("5 mm") && Step14_2.Equals("5 mm"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step15 :: Apply a window/level preset to the navigation control 3.
                bool Step15 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Abdomen, BluRingZ3DViewerPage.Preset);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: The selected window/level preset is applied to all navigation controls but not to the result control.
                String Step15_1 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                String Step15_2 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                String Step15_3 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                String Step15_4 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.ResultPanel);
                if (Step15 && Step15_1.Equals(Step15_2) && Step15_2.Equals(Step15_3) && !Step15_3.Equals(Step15_4))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step16 :: Apply a window/level preset to the result control.
                String Step16_1 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.ResultPanel);
                bool Step16 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Bone, BluRingZ3DViewerPage.Preset);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: The selected window/level preset is applied only to the result control.
                String Step16_2 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.ResultPanel);
                String Step16_3 = z3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                if (Step15 && !Step16_1.Equals(Step16_2) && Step16_3.Equals(Step15_3))
                {
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163404(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 2 :: From the Universal viewer ,Select a 3D supported series and Select the MPR option from the drop down.
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
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
                //Step 3 :: From smart view drop down select 3D 6:1 view mode.
                bool Step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (Step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 :: Select "Bone & Minimal Vessels" from preset drop down from 3D 2 control.
                bool Step4 = z3dvp.VerifyPresetWLandImage(BluRingZ3DViewerPage.Navigation3D2, BluRingZ3DViewerPage.Preset2);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: Selected preset should be applied to the Image in 3D 2 control .
                if (Step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step5 :: Select the toggle 3D/MPR option from the hover bar of 3D 1 control .
                bool Step5 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                if (Step5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step6 :: Click on 3D image 1 and apply an upward scroll until the last image of the image series is displayed.
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                {
                    IWebElement Navigationname17 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    bool Nav17_one = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigation3D1);
                    IWebElement ViewerContainer1 = z3dvp.ViewerContainer();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer1.Location.X + 750), (ViewerContainer1.Location.Y / 2 + 300));
                    int t = 0;
                    do
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 60, 0);
                        Thread.Sleep(1000);
                        t++;
                        if (t > 100) break;
                    }
                    while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2) <= 212);
                }
                else
                {
                    z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D1, ScrollDirection: "up", scrolllevel: 50, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(10);
                }
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    IWebElement AllControl = z3dvp.ViewerContainer();
                    if (CompareImage(result.steps[ExecutedSteps], AllControl))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                
                //Step 7 :: 
                string BeforeStep7 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForFrameLoad(10);
                bool Reset = z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                string AfterStep7 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                if (Reset && !BeforeStep7.Equals(AfterStep7))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 8 :: Apply a 180 degrees Y rotation to 3D control 2.
                IWebElement Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.Performdragdrop(Navigation3D2, Navigation3D2.Size.Width / 2, (Navigation3D2.Size.Height) * 3 / 4, Navigation3D2.Size.Width / 2, Navigation3D2.Size.Height / 4);
                //new Actions(Driver).MoveToElement(Navigation3D2, Navigation3D2.Size.Width / 2, Navigation3D2.Size.Height / 4).ClickAndHold()
                //.MoveToElement(Navigation3D2, Navigation3D2.Size.Width / 2, (Navigation3D2.Size.Height) * 3 / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: The white dotted line and arrow in navigation controls 2 and 3 rotated to the opposite of their original position and orientation.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                AllControl = z3dvp.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], AllControl))
                {
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163398(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //STEP 01 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 02 :: From the Universal viewer ,Select a 3D supported series and Select the MPR option from the drop down.
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 03 :: From smart view drop down select 3D 6:1 view mode.
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 04 :: Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the 3D navigation controls.
                bool step4 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                if (step4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 05 :: Apply the 3D tools (Pan, Zoom, Rotate etc) on the Navigation controls
                IWebElement Navigation_One = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                List<Z3DTools> tool = new List<Z3DTools>() { Z3DTools.Scrolling_Tool, Z3DTools.Line_Measurement, Z3DTools.Interactive_Zoom, Z3DTools.Pan, Z3DTools.Rotate_Tool_1_Image_Center };
                int PassCount = 0;
                for (int i = 0; i < tool.Count; i++)
                {
                    bool SelectTool = z3dvp.select3DTools(tool[i], BluRingZ3DViewerPage.Navigationone);
                    PageLoadWait.WaitForFrameLoad(5);
                    new Actions(Driver).MoveToElement(Navigation_One, Navigation_One.Size.Width / 2, Navigation_One.Size.Height / 2).Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    string ToolName = z3dvp.GetToolName(tool[i]);
                    bool ToolStatus = z3dvp.VerifyToolSelected(BluRingZ3DViewerPage.Navigationone, ToolName);
                    if (!ToolStatus)
                    {
                        PassCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (tool.Count.Equals(PassCount))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //STEP 06 :: Hover top of the viewport 3D 1 control and select toggle 3D/MPR in order to display the MPR view mode
                bool step6 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                if (step6)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 07 :: Select the Rotate tool from the Z3D tool box
                bool step7 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D1);
                if (step7)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 08 :: Left click and drag the mouse on the image in the Navigation control 1
                IWebElement NavigationOne = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement NavigationTwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement NavigationThree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                IWebElement Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.Performdragdrop(NavigationOne, 3 * (NavigationOne.Size.Width / 4), NavigationOne.Size.Height / 4, NavigationOne.Size.Width / 4, NavigationOne.Size.Height / 4);
                string NavigationOneLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                string NavigationTwoLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string NavigationThreeLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                string ResultPanelLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (NavigationOneLocValue.Equals(NavigationTwoLocValue) && NavigationOneLocValue.Equals(NavigationThreeLocValue) && NavigationOneLocValue.Equals(ResultPanelLocValue))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 09 :: Select the Reset button from the floating tool box
                bool step9 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                if (step9)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 10 :: Left click and drag the mouse on the image in the Navigation control 2
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.Performdragdrop(NavigationTwo, 3 * (NavigationTwo.Size.Width / 4), NavigationTwo.Size.Height / 4, NavigationTwo.Size.Width / 4, NavigationTwo.Size.Height / 4);
                NavigationOneLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                NavigationTwoLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                NavigationThreeLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (NavigationOneLocValue.Equals(NavigationTwoLocValue) && NavigationOneLocValue.Equals(NavigationThreeLocValue) && NavigationOneLocValue.Equals(ResultPanelLocValue))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 11 :: Select the Reset button from the floating tool box
                bool step11 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                if (step11)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 12 :: Left click and drag the mouse on the image in the Navigation control 3
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.Performdragdrop(NavigationThree, 3 * (NavigationThree.Size.Width / 4), NavigationThree.Size.Height / 4, NavigationThree.Size.Width / 4, NavigationThree.Size.Height / 4);
                NavigationOneLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                NavigationTwoLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                NavigationThreeLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (NavigationOneLocValue.Equals(NavigationTwoLocValue) && NavigationOneLocValue.Equals(NavigationThreeLocValue) && NavigationOneLocValue.Equals(ResultPanelLocValue))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 13 :: Select the Reset button from the floating tool box
                bool step13 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                if (step13)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 14 :: Select the Rotate tool from the floating tool box
                bool step14 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D1);
                if (step14)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 15 & 16 :: Left click and drag the mouse on the image in the MPR result control & click on OK button from the warning message dialog box
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.Performdragdrop(ResultPanel, 3 * (ResultPanel.Size.Width / 4), ResultPanel.Size.Height / 4, ResultPanel.Size.Width / 4, ResultPanel.Size.Height / 4);
                bool ErrorMsg = z3dvp.checkerrormsg("y");
                if (ErrorMsg)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 17 :: Select the Reset button from the floating tool box
                bool step17 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                if (step17)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 18 :: Select the Rotate tool from the floating tool box
                bool step18 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D1);
                if (step18)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 19 :: Left click and drag the mouse on the image in the 3D control 1
                string Navigation3D1LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                string Navigation3D2LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.Performdragdrop(Navigation3D1, 3 * (Navigation3D1.Size.Width / 4), Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4);
                string AfterDrag3D1LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                string AfterDrag3D2LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (!Navigation3D1LocValue.Equals(AfterDrag3D1LocValue) && !Navigation3D2LocValue.Equals(AfterDrag3D2LocValue) && AfterDrag3D1LocValue.Equals(AfterDrag3D2LocValue))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 20 :: Left click and drag the mouse on the image in the 3D control 2
                Navigation3D1LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.Performdragdrop(Navigation3D2, 3 * (Navigation3D2.Size.Width / 4), Navigation3D2.Size.Height / 4, Navigation3D2.Size.Width / 4, Navigation3D2.Size.Height / 4);
                AfterDrag3D1LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                AfterDrag3D2LocValue = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (!Navigation3D1LocValue.Equals(AfterDrag3D1LocValue) && !Navigation3D2LocValue.Equals(AfterDrag3D2LocValue) && AfterDrag3D1LocValue.Equals(AfterDrag3D2LocValue))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 21 :: Select the Reset button from the floating tool box
                bool step21 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                if (step21)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163401(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //step:2 - Series is loaded in the 3D viewer in MPR 4:1 viewing mode
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

                //step:3 - Six up view mode should be displayed
                Boolean step3 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
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

                //step:4 -  Note that the View is displayed as a 2 x 3 Grid view with 3 MPR navigation controls, 1 result control and 2 3D controls
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                IWebElement Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                int nav1locX = Navigation1.Location.X;
                int nav1locY = Navigation1.Location.Y;
                int nav2locX = Navigation2.Location.X;
                int nav2locY = Navigation2.Location.Y;
                int nav3locX = Navigation3.Location.X;
                int nav3locY = Navigation3.Location.Y;
                int ResultlocX = ResultPanel.Location.X;
                int ResultlocY = ResultPanel.Location.Y;
                int nav3D1locX = Navigation3D1.Location.X;
                int nav3D1locY = Navigation3D1.Location.Y;
                int nav3D2locX = Navigation3D2.Location.X;
                int nav3D2locY = Navigation3D2.Location.Y;
                Boolean step4_1 = Navigation1.Text.Contains(BluRingZ3DViewerPage.Navigationone);
                Boolean step4_2 = Navigation2.Text.Contains(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step4_3 = Navigation3.Text.Contains(BluRingZ3DViewerPage.Navigationthree);
                Boolean step4_4 = ResultPanel.Text.Contains(BluRingZ3DViewerPage.ResultPanel);
                Boolean step4_5 = Navigation3D1.Text.Contains(BluRingZ3DViewerPage.Navigation3D1);
                Boolean step4_6 = Navigation3D2.Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                if (nav1locX == nav3locX && nav2locX == ResultlocX && nav1locY == nav2locY && nav3locY == ResultlocY && nav3D1locX == nav3D2locX && step4_1 && step4_2 && step4_3 && step4_4 && step4_5 && step4_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5  -MPR 6:1 view mode switches to 3D 6:1 View mode
                Boolean step5 = Z3dViewerPage.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
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

                //step:6 - Note that the View is displayed as a 2 x 3 Grid view with 3D navigation controls, 1 result control and 2 3D controls
                nav1locX = Navigation1.Location.X;
                nav1locY = Navigation1.Location.Y;
                nav2locX = Navigation2.Location.X;
                nav2locY = Navigation2.Location.Y;
                nav3locX = Navigation3.Location.X;
                nav3locY = Navigation3.Location.Y;
                ResultlocX = ResultPanel.Location.X;
                ResultlocY = ResultPanel.Location.Y;
                nav3D1locX = Navigation3D1.Location.X;
                nav3D1locY = Navigation3D1.Location.Y;
                nav3D2locX = Navigation3D2.Location.X;
                nav3D2locY = Navigation3D2.Location.Y;
                Boolean step6_1 = Navigation1.Text.Contains(BluRingZ3DViewerPage.Navigationone);
                Boolean step6_2 = Navigation2.Text.Contains(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step6_3 = Navigation3.Text.Contains(BluRingZ3DViewerPage.Navigationthree);
                Boolean step6_4 = ResultPanel.Text.Contains(BluRingZ3DViewerPage.ResultPanel);
                Boolean step6_5 = Navigation3D1.Text.Contains(BluRingZ3DViewerPage.Navigation3D1);
                Boolean step6_6 = Navigation3D2.Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                if (nav1locX == nav3locX && nav2locX == ResultlocX && nav1locY == nav2locY && nav3locY == ResultlocY && nav3D1locX == nav3D2locX && step6_1 && step6_2 && step6_3 && step6_4 && step6_5 && step6_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //steo:7 - Resize the Z3D viewer's browser window is on portrait view mode, the width needs to be < the height. 
                Driver.Manage().Window.Size = new Size(750, 950);
                PageLoadWait.WaitForFrameLoad(10);
                nav1locX = Navigation1.Location.X;
                nav2locX = Navigation2.Location.X;
                nav3locX = Navigation3.Location.X;
                ResultlocX = ResultPanel.Location.X;
                nav3D1locX = Navigation3D1.Location.X;
                nav3D2locX = Navigation3D2.Location.X;
                Boolean step7_1 = Navigation1.Text.Contains(BluRingZ3DViewerPage.Navigationone);
                Boolean step7_2 = Navigation2.Text.Contains(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step7_3 = Navigation3.Text.Contains(BluRingZ3DViewerPage.Navigationthree);
                Boolean step7_4 = ResultPanel.Text.Contains(BluRingZ3DViewerPage.ResultPanel);
                Boolean step7_5 = Navigation3D1.Text.Contains(BluRingZ3DViewerPage.Navigation3D1);
                Boolean step7_6 = Navigation3D2.Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                if (nav1locX == nav3locX && nav1locX == nav3D1locX && nav3locX == nav3D1locX && nav2locX == ResultlocX && nav2locX == nav3D2locX && ResultlocX == nav3D2locX && step7_1 && step7_2 && step7_3 && step7_4 && step7_5 && step7_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8 -  Hover top of the viewport 3D 1 control and select toggle 3D / MPR in order to display the 3D navigation controls.
                Boolean step8 = Z3dViewerPage.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
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

                //step:9 - Note that the View is displayed as a 2 x 3 Grid view with 3 MPR navigation controls, 1 result control and 2 3D controls.
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                nav1locX = Navigation1.Location.X;
                nav1locY = Navigation1.Location.Y;
                nav2locX = Navigation2.Location.X;
                nav2locY = Navigation2.Location.Y;
                nav3locX = Navigation3.Location.X;
                nav3locY = Navigation3.Location.Y;
                ResultlocX = ResultPanel.Location.X;
                ResultlocY = ResultPanel.Location.Y;
                nav3D1locX = Navigation3D1.Location.X;
                nav3D1locY = Navigation3D1.Location.Y;
                nav3D2locX = Navigation3D2.Location.X;
                nav3D2locY = Navigation3D2.Location.Y;
                Boolean step9_1 = Navigation1.Text.Contains(BluRingZ3DViewerPage.Navigationone);
                Boolean step9_2 = Navigation2.Text.Contains(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step9_3 = Navigation3.Text.Contains(BluRingZ3DViewerPage.Navigationthree);
                Boolean step9_4 = ResultPanel.Text.Contains(BluRingZ3DViewerPage.ResultPanel);
                Boolean step9_5 = Navigation3D1.Text.Contains(BluRingZ3DViewerPage.Navigation3D1);
                Boolean step9_6 = Navigation3D2.Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                if (nav1locX == nav3locX && nav2locX == ResultlocX && nav1locY == nav2locY && nav3locY == ResultlocY && nav3D1locX == nav3D2locX && step9_1 && step9_2 && step9_3 && step9_4 && step9_5 && step9_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 -  	Resize the Z3D viewer's browser window is on portrait view mode, the width needs to be < the height.
                Driver.Manage().Window.Size = new Size(750, 950);
                PageLoadWait.WaitForFrameLoad(10);
                nav1locX = Navigation1.Location.X;
                nav2locX = Navigation2.Location.X;
                nav3locX = Navigation3.Location.X;
                ResultlocX = ResultPanel.Location.X;
                nav3D1locX = Navigation3D1.Location.X;
                nav3D2locX = Navigation3D2.Location.X;
                Boolean step10_1 = Navigation1.Text.Contains(BluRingZ3DViewerPage.Navigationone);
                Boolean step10_2 = Navigation2.Text.Contains(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step10_3 = Navigation3.Text.Contains(BluRingZ3DViewerPage.Navigationthree);
                Boolean step10_4 = ResultPanel.Text.Contains(BluRingZ3DViewerPage.ResultPanel);
                Boolean step10_5 = Navigation3D1.Text.Contains(BluRingZ3DViewerPage.Navigation3D1);
                Boolean step10_6 = Navigation3D2.Text.Contains(BluRingZ3DViewerPage.Navigation3D2);
                if (nav1locX == nav3locX && nav1locX == nav3D1locX && nav3locX == nav3D1locX && nav2locX == ResultlocX && nav2locX == nav3D2locX && ResultlocX == nav3D2locX && step10_1 && step10_2 && step10_3 && step10_4 && step10_5 && step10_6)
                {
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
                Driver.Manage().Window.Maximize();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163402(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            string slocationvalue = ssplit[0];
            string spresetvalue = ssplit[1];


            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Step 1   Login as a administrator 
                login.LoginIConnect(username, password);
                z3dvp.Deletefiles(testcasefolder);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //step 2 From iCA, Load a study in Z3D viewer. By default MPR viewing mode should dipslay
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    throw new Exception("Failed to open study in 3D Test_163402");
                }


                //Step 3 From smart view drop down select 3D 6:1 view mode.
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
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

                //step 4 Apply the zoom tool to 3D control 1.
                IWebElement sixviewport = z3dvp.SixupViewCont();
                IWebElement nav3D1_4 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                //   z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigation3D1);
                if (Config.BrowserType == "chrome" || Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    Actions act4 = new Actions(Driver);
                    act4.MoveToElement(nav3D1_4, nav3D1_4.Size.Width / 2 - 10, nav3D1_4.Size.Height / 2 - 5).ClickAndHold().DragAndDropToOffset(nav3D1_4, nav3D1_4.Size.Width / 2 - 10, nav3D1_4.Size.Height / 2 - 50).Build().Perform();
                    //   new Actions(Driver).MoveToElement(nav3D1_4, nav3D1_4.Size.Width / 2, nav3D1_4.Size.Height / 2).ClickAndHold()
                    //.MoveToElement(nav3D1_4, nav3D1_4.Size.Width / 2, nav3D1_4.Size.Height / 2 + 100)
                    //.Release()
                    //.Build()
                    //.Perform();
                }
                else
                    new TestCompleteAction { }.DragAndDropToOffset(nav3D1_4, 10, 20).Release().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1)))
                {
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2)))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                new Actions(Driver).SendKeys(("T")).Build().Perform();
                Thread.Sleep(500);

                //Step 5 Press the reset button from floating toolbox.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(10000);
                List<string> result3 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result3[0] == result3[1] && result3[3] == result3[4] && result3[2] == result3[5] && slocationvalue == (result3[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 Push the "print screen" key to capture the screen and paste into MS Paint.
                string filename = "PrintBt_163402_" + new Random().Next(1000) + ".bmp";

                if (!Directory.Exists(testcasefolder))
                {
                    Directory.CreateDirectory(testcasefolder);
                }
                Screenshot testimage = ((ITakesScreenshot)Driver).GetScreenshot();
                testimage.SaveAsFile(testcasefolder + Path.DirectorySeparatorChar + filename, ScreenshotImageFormat.Jpeg);
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


                //Step 7 Use the X axis rotate hotspot on the 3D control 2 to rotate the 3D controls.

                bool lflag = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigation3D2);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                {
                    try
                    {
                        Accord.Point p1 = z3dvp.GetIntersectionPoints(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, ExecutedSteps, "yellow", "Horizontal", 0);
                        Accord.Point p2 = z3dvp.GetIntersectionPoints(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, ExecutedSteps, "yellow", "Horizontal", 1);
                        Thread.Sleep(10000);
                        Actions act5 = new Actions(Driver);
                        z3dvp.SelectControl(BluRingZ3DViewerPage.Navigation3D2);
                        act5.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), (int)p1.X, (int)p1.Y).ClickAndHold().MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), (int)(p1.X + 90), (int)(p1.Y + 90)).Release().Build().Perform();
                        Thread.Sleep(20000);
                        new Actions(Driver).SendKeys("T").Build().Perform();
                        Thread.Sleep(500);
                        result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                        Thread.Sleep(1000);
                        bool bflag7 = false;
                        if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1)))
                        {
                            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                            if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2)))
                            {
                                bflag7 = true;
                                result.steps[ExecutedSteps].status = "Pass";
                                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                            }
                        }
                        if (bflag7 == false)
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                    }
                    catch
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    //  new Actions(Driver).SendKeys("T").Build().Perform();
                    //   bool lflag = z3dvp.SelectControl(BluRingZ3DViewerPage.Navigation3D2);

                    try
                    {
                        IWebElement iNavigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);

                        new TestCompleteAction().MoveToElement(iNavigation3D2, iNavigation3D2.Size.Width / 2 - 5, iNavigation3D2.Size.Height / 2).Click().Release().Perform();
                        Thread.Sleep(5000);
                        new TestCompleteAction().PerformDraganddrop(iNavigation3D2, (iNavigation3D2.Size.Width - (iNavigation3D2.Size.Width - 4)), iNavigation3D2.Size.Height / 2, (iNavigation3D2.Size.Width - 4), iNavigation3D2.Size.Height / 2).Perform();
                        Thread.Sleep(20000);

                        new Actions(Driver).SendKeys(("T")).Build().Perform();
                        Thread.Sleep(500);
                        result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                        Thread.Sleep(1000);
                        bool bflag7 = false;
                        if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1)))
                        {
                            result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                            if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2)))
                            {
                                bflag7 = true;
                                result.steps[ExecutedSteps].status = "Pass";
                                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                            }
                        }
                        if (bflag7 == false)
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    catch
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                new Actions(Driver).SendKeys("T").Build().Perform();
                Thread.Sleep(500);
                //Step 8 Apply the scroll tool to the 3D control 2.
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(10000);
                List<string> iBefore_Scroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                IWebElement IWB3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                Thread.Sleep(1000);
                if (Config.BrowserType.ToLower() != "firefox" && Config.BrowserType.ToLower() != "mozilla")
                    z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D2, scrolllevel: 5, Thickness: "n", UseTestComplete: true);
                else
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((sixviewport.Location.X + 700), (sixviewport.Location.Y + 575));
                    Thread.Sleep(1000);
                    for (int i = 0; i < 15; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 20, 0);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
                List<string> iAfter_scroll = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (iBefore_Scroll[0] == iAfter_scroll[0] && iBefore_Scroll[1] == iAfter_scroll[1] && iBefore_Scroll[2] != iAfter_scroll[2] && iBefore_Scroll[3] == iAfter_scroll[3] && iBefore_Scroll[4] == iAfter_scroll[4] && iBefore_Scroll[5] != iAfter_scroll[5])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 9 Apply the window/level tool to 3D control 1.
                List<string> iBefore8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigation3D1);
                Thread.Sleep(1000);
                IWebElement IWB3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    
                    z3dvp.Performdragdrop(IWB3D1, 20, 30);
                }
                else
                new Actions(Driver).MoveToElement(IWB3D1, IWB3D1.Size.Width - 10, IWB3D1.Size.Height - 5).ClickAndHold().DragAndDropToOffset(IWB3D1, IWB3D1.Size.Width - 10, IWB3D1.Size.Height - 20).Build().Perform();

                Thread.Sleep(10000);
                List<string> iAfter8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                bool bflag9 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1)))
                {
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2)))
                    {
                        bflag9 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if (bflag9 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 10 Apply a modality specific transfer function preset to the 3D control 2.
                IWebElement IWE_3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                bool SelectPreset = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigation3D2, spresetvalue, "Preset");
                List<string> result10 = z3dvp.GetControlvalues(BluRingZ3DViewerPage.Preset, spresetvalue);
                if (result10[0] == "-1" && result10[1] == "-1" && result10[2] == "-1" && result10[3] == "-1" && result10[4] == "-1" && result10[5] == spresetvalue)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11 Press the reset button floating tool box
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                List<string> result11 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result11[0] == result11[1] && result11[3] == result11[4] && result11[2] == result11[5] && slocationvalue == (result3[0]))
                {
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
                z3dvp.CloseViewer();
                login.Logout();
                Driver.Close();
            }
        }

        public TestCaseResult Test_163403(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
             //   ExecutedSteps++;

                //step:2 - Series is loaded in the 3D viewer in MPR 4:1 viewing mode
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

                //step:3 - Six up view mode should be displayed
                Boolean step3 = Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
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

                //step:4 - Apply the zoom tool to navigation control 1.
                Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                List<string> Initailstate = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.Performdragdrop(Navigation1, (Navigation1.Size.Width / 2) - 50, (Navigation1.Size.Height / 2) - 75, (Navigation1.Size.Width / 2) - 50, (Navigation1.Size.Height / 2) + 50);
                List<string> step4 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (Initailstate[0] != step4[0] && Initailstate[1] != step4[1] && Initailstate[3] != step4[3] && Initailstate[4] != step4[4])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 = Press the reset button floating toolbox
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step5 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (Initailstate[0] == step5[0] && Initailstate[1] == step5[1] && Initailstate[2] == step5[2] && Initailstate[3] == step5[3] && Initailstate[4] == step5[4] && Initailstate[5] == step5[5])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:6 - Push the "print screen" key to capture the screen and paste into MS Paint.
                string filename = "PrintBt_163403_" + new Random().Next(1000) + ".jpg";
                String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
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

                //step:7 - Rotate the cross hairs of navigation control 2 by 180 degrees clockwise
                //String[] step7_1 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigationone).Split(' ');
                //String[] step7_2 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.ResultPanel).Split(' '); ;
                //String[] step7_3 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree).Split(' ');
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int startx = 0; int starty = 0; int endx = 0; int endy = 0;
                IWebElement iNavigationtwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                bool lflag = Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                if (Config.BrowserType.ToLower() == "chrome" || Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    Accord.Point p11 = Z3dViewerPage.GetIntersectionPoints(iNavigationtwo, testid, ExecutedSteps, "red", "Horizontal", 0);
                    new Actions(Driver).MoveToElement(iNavigationtwo, (iNavigationtwo.Size.Width / 2), (iNavigationtwo.Size.Height / 2)).Build().Perform();
                    Thread.Sleep(2000);
                    Accord.Point p22 = Z3dViewerPage.GetIntersectionPoints(iNavigationtwo, testid, 5, "red", "Horizontal", 1);
                    Thread.Sleep(2000);
                    Actions act5 = new Actions(Driver);

                    Thread.Sleep(5000);
                    act5.MoveToElement(iNavigationtwo, (int)p11.X, (int)p11.Y).ClickAndHold().
                        MoveToElement(iNavigationtwo, (int)p22.X, (int)p22.Y).Release().Build().Perform();
                    Thread.Sleep(20000);
                    
                }
                else
                {
                     startx = (Navigation2.Size.Width / 2) - 106;
                     starty = Navigation2.Size.Height / 2;
                     endx = (Navigation2.Size.Width / 2) + 116;
                     endy = (Navigation2.Size.Height / 2);
                    Thread.Sleep(5000);
                    Z3dViewerPage.Performdragdrop(Navigation2, endx, endy, startx, starty);
                }
                
                //String[] step7_4 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigationone).Split(' ');
                //String[] step7_5 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.ResultPanel).Split(' ');
                //String[] step7_6 = Z3dViewerPage.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree).Split(' ');
                List<string> sAnnotaion = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                //if (step7_1[1] == "H" && step7_2[1] == "H" && step7_3[1] == "A" && step7_4[1] == "F" && step7_5[1] == "F" && step7_6[1] == "P")
                if (sAnnotaion[0] == "F" && sAnnotaion[1] == "H" && sAnnotaion[2] == "H" && sAnnotaion[3] == "P" && sAnnotaion[4] == "F" && sAnnotaion[5] == "H")
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8 -  	Apply the roam tool to navigation control 3
                IWebElement Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Performdragdrop(Navigation3, (Navigation3.Size.Width / 2) - 50, (Navigation3.Size.Height / 2) - 75, (Navigation3.Size.Width / 2) - 50, (Navigation3.Size.Height / 2) + 50);
                List<string> step8 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (Initailstate[0] != step8[0] && Initailstate[1] != step8[1] && Initailstate[4] != step8[3] && Initailstate[4] != step8[4])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:9 - Apply the scroll tool to navigation control 2. Note: Make sure that navigation control 2 is selected in the Result control.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.ResultPanel);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point p1 = Z3dViewerPage.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 92, "blue", blobval: 0);
                Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point p2 = Z3dViewerPage.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + 93, "yellow", blobval: 1);
                float step9_1 = p1.X;
                float step9_2 = p1.Y;
                float step9_3 = p2.X;
                float step9_4 = p2.Y;
                List<string> Before = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.Performdragdrop(Navigation2, (Navigation2.Size.Width / 2) - 50, (Navigation2.Size.Height / 2) - 75, (Navigation2.Size.Width / 2) - 150, (Navigation2.Size.Height / 2) + 50, RemoveCross: true);
                Thread.Sleep(1000);
                Z3dViewerPage.Performdragdrop(Navigation2, (Navigation2.Size.Width / 2) - 50, (Navigation2.Size.Height / 2) - 75, (Navigation2.Size.Width / 2) - 150, (Navigation2.Size.Height / 2) + 50, RemoveCross: true);
                Thread.Sleep(1000);
                PageLoadWait.WaitForPageLoad(5);
                List<string> step9 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Accord.Point p3 = Z3dViewerPage.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + new Random().Next(1000), "blue", blobval: 0);
                Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                Accord.Point p4 = Z3dViewerPage.GetIntersectionPoints(Navigation3, testid, ExecutedSteps + new Random().Next(1000), "yellow", blobval: 1);
                float step9_5 = p3.X;
                float step9_6 = p3.Y;
                float step9_7 = p4.X;
                float step9_8 = p4.Y;
                if (Before[1] != step9[1] && Before[4] != step9[4] && (step9_1 != step9_5 || step9_2 != step9_6 ) && step9_3 != step9_7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 - Apply the window/level tool to navigation control 2
                Boolean step10 = false;
                if (Config.BrowserType.ToLower() == "chrome")
                {
                    Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationtwo);
                    new Actions(Driver).SendKeys("x").Release().Build().Perform();
                    IWebElement Inavigationtwo = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    Z3dViewerPage.Performdragdrop(Inavigationtwo, 20, 30);
                    Thread.Sleep(10000);
                    List<string> result8 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    if (result8[0] == result8[1] && result8[1] == result8[3])
                    {
                        step10 = true;
                    }
                    new Actions(Driver).SendKeys("x").Release().Build().Perform();
                }
                else
                {
                     step10 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationtwo, Z3DTools.Window_Level, 50, 50, 200, movement: "positive");
                }
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

                //step:11 - Adjust the thickness to be 10.0 mm on navigation control 2
                String IncrementValue = "10";
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationtwo, IncrementValue);
                PageLoadWait.WaitForFrameLoad(5);
                String step11_1 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationone);
                String step11_2 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationtwo);
                String step11_3 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.Navigationthree);
                String step11_4 = Z3dViewerPage.GetThickNessValue(BluRingZ3DViewerPage.ResultPanel);
                Boolean step11_5 = step11_1.Equals(IncrementValue + " mm");
                Boolean step11_6 = step11_2.Equals(IncrementValue + " mm");
                Boolean step11_7 = step11_3.Equals(IncrementValue + " mm");
                Boolean step11_8 = step11_4.Equals(IncrementValue + " mm");
                if (step11_5 && step11_6 && step11_7 && step11_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12 - Apply the window/level tool to navigation control3
                // Boolean step12 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationthree, Z3DTools.Window_Level, 50, 50, 200, movement: "positive");
                IWebElement INavigationthree = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).SendKeys("X").Build().Perform();Thread.Sleep(1000);
                Z3dViewerPage.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationtwo);
                Z3dViewerPage.Performdragdrop(INavigationthree, 20, 30);
                new Actions(Driver).SendKeys("X").Build().Perform(); Thread.Sleep(1000);
                List<string> result12 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (result12[0] == result12[1] && result12[1] == result12[3])
                {
                    
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13 - Press the reset button floating toolbox.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> step13 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (Initailstate[0] == step13[0] && Initailstate[1] == step13[1] && Initailstate[2] == step13[2] && Initailstate[3] == step13[3] && Initailstate[4] == step13[4] && Initailstate[5] == step13[5])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - Using the traverse tool, move the cross hair of navigation control 1 upward until the position annotation of navigation control 3 reads "0, 0, 100"
                Z3dViewerPage.Performdragdrop(Navigation1, Navigation1.Size.Width / 2, (Navigation1.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                String step14_Nav3 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                List<string> step14 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                String[] step14_1 = step14[3].Split(' ');
                Double step14_2 = Convert.ToDouble(step14_1[3]); step14_2 = Math.Round(step14_2 / 10) * 10; int step14_3 = Convert.ToInt32(step14_2);
                if (Initailstate[0] == step14[0] && Initailstate[1] == step14[1] && Initailstate[2] == step14[2] && Initailstate[3] != step14[3] && Initailstate[4] == step14[4] && Initailstate[5] == step14[5])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:15 - Take a screenshot of the Z3D viewer
                filename = "PrintBt_163403_" + new Random().Next(1000) + ".jpg";
                testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.CaptureScreen(filename, testid);
                PageLoadWait.WaitForFrameLoad(10);
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

                //step:16 - Using the traverse tool, move the cross hair of navigation control 1 back to its original position "0, 0, 0".
                Z3dViewerPage.Performdragdrop(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, Navigation1.Size.Width / 2, (Navigation1.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step16 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (Initailstate[0] == step16[0] && Initailstate[1] == step16[1] && step14[3] != step16[3] && Initailstate[4] == step16[4])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:17 -  	Apply an upward scroll to navigation control 3 until the position annotation reads "0, 0, 100".

                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "chrome")
                {
                    Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                    bool Nav17_one = Z3dViewerPage.SelectControl(BluRingZ3DViewerPage.Navigationthree);
                    IWebElement ViewerContainer1 = Z3dViewerPage.ViewerContainer();
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((ViewerContainer1.Location.X + 200), (ViewerContainer1.Location.Y / 2 + 600));
                    int t = 0;
                    do
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 40, 0);
                        Thread.Sleep(1000);
                        t++;
                        if (t > 200) break;
                    }
                    while (Z3dViewerPage.checkvalue(Locators.CssSelector.LeftTopPane, 2,3) < 100);
                }
                else
                {
                    Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationthree, scrollTill: TestData, UseTestComplete: true);
                }
                String step17_Nav3 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                String[] step17_1 = step17_Nav3.Split(' ');
                Double step17_2 = Convert.ToDouble(step17_1[3]); int step17_3 = Convert.ToInt32(step17_2);
                if (step14_3 >= step17_3 - 1 && step14_3 <= step17_3 + 1)
                {
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

        public TestCaseResult Test_163406(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //STEP 01 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 02 :: From the Universal viewer ,Select a 3D supported series and Select the MPR option from the drop down.
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 03 :: From smart view drop down select 3D 6:1 view mode.
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 04 :: Note that the View is displayed as a 2 x 3 Grid view with 3 MPR navigation controls, 1 result control and 2 3D controls
                IWebElement NavigationOne = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement NavigationTwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement NavigationThree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                IWebElement Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                if (NavigationOne.Displayed && NavigationTwo.Displayed && NavigationThree.Displayed && ResultPanel.Displayed && Navigation3D1.Displayed && Navigation3D2.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 05 :: Click the "toggle 3D/MPR" button at the top right corner in hover bar of the 3D control 1
                bool step5 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ViewerContainer = z3dvp.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer) && step5)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163405(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string Patientid1 = objTestRequirement.Split('|')[0];
            string ImageCount1 = objTestRequirement.Split('|')[1];
            string Navigation1LocVal, Navigation2LocVal, Navigation3LocVal, ResultPanelLocVal, Navigation3D1Loc, Navigation3D2Loc, AfterDragNavigation1LocVal, AfterDragNavigation2LocVal, AfterDragNavigation3LocVal, AfterDragResultPanelLocVal, AfterDrag3D1Loc, AfterDrag3D2Loc;
            List<string> NavigationResPanel = new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel };
            List<string> ThreeDPanel = new List<string> { BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2 };
            List<string> NavigationPanel = new List<string> { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree };
            List<string> SixUpViewControls = new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2 };

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //STEP 01 :: Login iCA as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 02 :: From the Universal viewer ,Select a 3D supported Lossy compressed series and Select the MPR option from the drop down.
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR, ChangeSettings:"No");
                if (step2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 03 :: From smart view drop down select 3D 6:1 view mode.
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 04 :: Click the User Settings button from the global toolbar and select 3D settings option move the MPR interactive quality and 3D interactive quality sliders to 100%. and click save.
                bool MPRInteractive = z3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                bool Interactive3D = z3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (Interactive3D && MPRInteractive)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 05 :: Select the scroll tool from the floating toolbox
                bool Scroll = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                if (Scroll)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 06 :: Scroll through the image in MPR result control
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                IWebElement Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step6 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.ResultPanel, ResultPanel.Size.Width / 4, ResultPanel.Size.Height / 4, ResultPanel.Size.Width / 4, 3 * (ResultPanel.Size.Height / 4));
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step6[0].Equals("Lossy Compressed") && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 07 :: Scroll through the image in 3D1 control
                Navigation3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);

                string[] step7 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), ThreeDPanel);
                AfterDrag3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                AfterDrag3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (step7[0].Equals("Lossy Compressed") && step7[1].Equals("Lossy Compressed") && Navigation3D1Loc != AfterDrag3D1Loc && Navigation3D2Loc != AfterDrag3D2Loc)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 08 :: Select the window level tool from the toolbox
                bool Result = z3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(10);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 09 :: Apply the window level on Navigation control 2
                Navigation1LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);

                string[] step9 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationtwo, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, Navigation2.Size.Width / 4, 3 * (Navigation2.Size.Height / 4), NavigationPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                if (step9[0].Equals("Lossy Compressed") && step9[1].Equals("Lossy Compressed") && step9[2].Equals("Lossy Compressed") && AfterDragNavigation1LocVal != Navigation1LocVal && AfterDragNavigation2LocVal != Navigation2LocVal && AfterDragNavigation3LocVal != Navigation3LocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 10 :: Apply the window level on MPR result control
                ResultPanelLocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step10 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.ResultPanel, ResultPanel.Size.Width / 4, ResultPanel.Size.Height / 4, ResultPanel.Size.Width / 4, 3 * (ResultPanel.Size.Height / 4));
                AfterDragResultPanelLocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step10[0].Equals("Lossy Compressed") && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 11 :: Apply the window level on 3D1 control
                string[] step11 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4));
                if (step11[0].Equals("Lossy Compressed"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 12 :: Apply the window level on 3D2 control
                string[] step12 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D2, Navigation3D2.Size.Width / 4, Navigation3D2.Size.Height / 4, Navigation3D2.Size.Width / 4, 3 * (Navigation3D2.Size.Height / 4));
                if (step12[0].Equals("Lossy Compressed"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 13 :: Select the Roam tool from the floating toolbox
                bool Pan = z3dvp.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                if (Pan)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 14 :: Click and hold the left mouse button on the image displayed on the navigation control 1 and move the mouse
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);

                string[] step14 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4), NavigationResPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step14[0].Equals("Lossy Compressed") && step14[1].Equals("Lossy Compressed") && step14[2].Equals("Lossy Compressed") && step14[3].Equals("Lossy Compressed") && Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 15 :: Select the Zoom tool from the floating toolbox
                bool Zoom = z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                PageLoadWait.WaitForFrameLoad(10);
                if (Zoom)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 16 :: Click and hold the left mouse button on the image displayed on the MPR navigation control 1 and do Zoom in /zoom out by dragging the mouse upwards/downward
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step16 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, (3 * (Navigation1.Size.Height / 4)) + 10, Navigation1.Size.Width / 4, (Navigation1.Size.Height / 4) + 10, NavigationResPanel);
                string[] step16_1 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4), NavigationResPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step16[0].Equals("Lossy Compressed") && step16[1].Equals("Lossy Compressed") && step16[2].Equals("Lossy Compressed") && step16[3].Equals("Lossy Compressed") && step16_1[0].Equals("Lossy Compressed") && step16_1[1].Equals("Lossy Compressed") && step16_1[2].Equals("Lossy Compressed") && step16_1[3].Equals("Lossy Compressed") && Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 17 :: Click and hold the left mouse button on the image displayed on the 3D1 control and do Zoom in /zoom out by dragging the mouse upwards/downwards
                string[] step17 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, (3 * (Navigation3D1.Size.Height / 4)) + 10, Navigation3D1.Size.Width / 4, (Navigation3D1.Size.Height / 4) + 10, ThreeDPanel);
                string[] step17_1 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), ThreeDPanel);
                if (step17[0].Equals("Lossy Compressed") && step17[1].Equals("Lossy Compressed") && step17_1[0].Equals("Lossy Compressed") && step17_1[1].Equals("Lossy Compressed"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 18 :: Select the Rotate tool from the floating toolbox
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                bool Rotate = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                PageLoadWait.WaitForFrameLoad(10);
                if (Rotate)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 19 :: Click and hold the left mouse button on the image displayed on the navigation control 3 and do a free rotation
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step19 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationthree, Navigation3.Size.Width / 4, 3 * (Navigation3.Size.Height / 4), Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, NavigationResPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step19[0].Equals("Lossy Compressed") && step19[1].Equals("Lossy Compressed") && step19[2].Equals("Lossy Compressed") && step19[3].Equals("Lossy Compressed") && !Navigation1LocVal.Equals(AfterDragNavigation1LocVal) && !Navigation2LocVal.Equals(AfterDragNavigation2LocVal) && !Navigation3LocVal.Equals(AfterDragNavigation3LocVal) && !ResultPanelLocVal.Equals(AfterDragResultPanelLocVal))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 20 :: Click and hold the left mouse button on the image displayed on the 3D control and do a free rotation
                Navigation3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                string[] step20 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, ThreeDPanel);
                AfterDrag3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                AfterDrag3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (step20[0].Equals("Lossy Compressed") && step20[1].Equals("Lossy Compressed") && !Navigation3D1Loc.Equals(AfterDrag3D1Loc) && !Navigation3D2Loc.Equals(AfterDrag3D2Loc))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 21 :: Click the User Settings button from the global toolbar and select 3D settings option and move the MPR final quality and 3D final quality sliders lesser 100%. ( Range = 1% to 99%). and click save
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                bool FinalQuality3D = z3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 90);
                PageLoadWait.WaitForFrameLoad(5);
                bool MPRFinalQuality = z3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 90);
                if (FinalQuality3D && MPRFinalQuality)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 22 :: Repeat steps 5-20.
                Scroll = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(10);

                //Scroll through the image in MPR result control
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step_6 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.ResultPanel, ResultPanel.Size.Width / 4, ResultPanel.Size.Height / 4, ResultPanel.Size.Width / 4, 3 * (ResultPanel.Size.Height / 4));
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (Scroll && step_6[0].Equals("Lossy Compressed"))
                {
                    //Scroll through the image in 3D1 control
                    Navigation3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                    Navigation3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                    string[] step_7 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), ThreeDPanel);
                    AfterDrag3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                    AfterDrag3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                    if (step_7[0].Equals("Lossy Compressed") && step_7[1].Equals("Lossy Compressed") && Navigation3D1Loc != AfterDrag3D1Loc && Navigation3D2Loc != AfterDrag3D2Loc)
                    {
                        //Select the window level tool from the toolbox
                        Result = z3dvp.select3DTools(Z3DTools.Window_Level);
                        PageLoadWait.WaitForFrameLoad(10);

                        //Apply the window level on Navigation control 2
                        Navigation1LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                        Navigation2LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                        Navigation3LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                        string[] step_9 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationtwo, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4, Navigation2.Size.Width / 4, 3 * (Navigation2.Size.Height / 4), NavigationPanel);
                        AfterDragNavigation1LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                        AfterDragNavigation2LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                        AfterDragNavigation3LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                        if (Result && step_9[0].Equals("Lossy Compressed") && step_9[1].Equals("Lossy Compressed") && step_9[2].Equals("Lossy Compressed") && AfterDragNavigation1LocVal != Navigation1LocVal && AfterDragNavigation2LocVal != Navigation2LocVal && AfterDragNavigation3LocVal != Navigation3LocVal)
                        {
                            //Apply the window level on MPR result control
                            ResultPanelLocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                            string[] step_10 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.ResultPanel, ResultPanel.Size.Width / 4, ResultPanel.Size.Height / 4, ResultPanel.Size.Width / 4, 3 * (ResultPanel.Size.Height / 4));
                            AfterDragResultPanelLocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                            if (step_10[0].Equals("Lossy Compressed") && ResultPanelLocVal != AfterDragResultPanelLocVal)
                            {
                                //Apply the window level on 3D1 control
                                string[] step_11 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4));
                                if (step_11[0].Equals("Lossy Compressed"))
                                {
                                    //Apply the window level on 3D2 control
                                    string[] step_12 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D2, Navigation3D2.Size.Width / 4, Navigation3D2.Size.Height / 4, Navigation3D2.Size.Width / 4, 3 * (Navigation3D2.Size.Height / 4));
                                    if (step_12[0].Equals("Lossy Compressed"))
                                    {
                                        //Select the Roam tool from the floating toolbox
                                        Pan = z3dvp.select3DTools(Z3DTools.Pan);
                                        PageLoadWait.WaitForFrameLoad(10);

                                        //Click and hold the left mouse button on the image displayed on the navigation control 1 and move the mouse
                                        Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                                        Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                                        Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                                        ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                                        string[] step_14 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4), NavigationResPanel);
                                        AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                                        AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                                        AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                                        AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                                        if (Pan && step_14[0].Equals("Lossy Compressed") && step_14[1].Equals("Lossy Compressed") && step_14[2].Equals("Lossy Compressed") && step_14[3].Equals("Lossy Compressed") && Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                                        {
                                            //Select the Zoom tool from the floating toolbox
                                            Zoom = z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                                            PageLoadWait.WaitForFrameLoad(10);

                                            //Click and hold the left mouse button on the image displayed on the MPR navigation control 1 and do Zoom in /zoom out by dragging the mouse upwards/downward
                                            Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                                            Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                                            Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                                            ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                                            string[] step_16 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, (3 * (Navigation1.Size.Height / 4)) + 10, Navigation1.Size.Width / 4, (Navigation1.Size.Height / 4) + 10, NavigationResPanel);
                                            string[] step_16_1 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4), NavigationResPanel);
                                            AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                                            AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                                            AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                                            AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                                            if (Zoom && step_16[0].Equals("Lossy Compressed") && step_16[1].Equals("Lossy Compressed") && step_16[2].Equals("Lossy Compressed") && step_16[3].Equals("Lossy Compressed") && step_16_1[0].Equals("Lossy Compressed") && step_16_1[1].Equals("Lossy Compressed") && step_16_1[2].Equals("Lossy Compressed") && step_16_1[3].Equals("Lossy Compressed") && Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                                            {
                                                //Click and hold the left mouse button on the image displayed on the 3D1 control and do Zoom in /zoom out by dragging the mouse upwards/downwards
                                                string[] step_17 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, (3 * (Navigation3D1.Size.Height / 4)) + 10, Navigation3D1.Size.Width / 4, (Navigation3D1.Size.Height / 4) + 10, ThreeDPanel);
                                                string[] step_17_1 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), ThreeDPanel);
                                                if (step_17[0].Equals("Lossy Compressed") && step_17[1].Equals("Lossy Compressed") && step_17_1[0].Equals("Lossy Compressed") && step_17_1[1].Equals("Lossy Compressed"))
                                                {
                                                    //Select the Rotate tool from the floating toolbox
                                                    z3dvp.select3DTools(Z3DTools.Reset);
                                                    PageLoadWait.WaitForFrameLoad(10);
                                                    Rotate = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                                                    PageLoadWait.WaitForFrameLoad(10);

                                                    //Click and hold the left mouse button on the image displayed on the navigation control 3 and do a free rotation
                                                    Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                                                    Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                                                    Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                                                    ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                                                    string[] step_19 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationthree, Navigation3.Size.Width / 4, 3 * (Navigation3.Size.Height / 4), Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, NavigationResPanel);
                                                    AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                                                    AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                                                    AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                                                    AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                                                    if (Rotate && step_19[0].Equals("Lossy Compressed") && step_19[1].Equals("Lossy Compressed") && step_19[2].Equals("Lossy Compressed") && step_19[3].Equals("Lossy Compressed") && !Navigation1LocVal.Equals(AfterDragNavigation1LocVal) && !Navigation2LocVal.Equals(AfterDragNavigation2LocVal) && !Navigation3LocVal.Equals(AfterDragNavigation3LocVal) && !ResultPanelLocVal.Equals(AfterDragResultPanelLocVal))
                                                    {
                                                        //Click and hold the left mouse button on the image displayed on the 3D control and do a free rotation
                                                        Navigation3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                                                        Navigation3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                                                        string[] step_20 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, ThreeDPanel);
                                                        AfterDrag3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                                                        AfterDrag3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                                                        if (step_20[0].Equals("Lossy Compressed") && step_20[1].Equals("Lossy Compressed") && !Navigation3D1Loc.Equals(AfterDrag3D1Loc) && !Navigation3D2Loc.Equals(AfterDrag3D2Loc))
                                                        {
                                                            result.steps[++ExecutedSteps].StepPass();
                                                        }
                                                        else
                                                        {
                                                            result.steps[++ExecutedSteps].StepFail();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.steps[++ExecutedSteps].StepFail();
                                                    }
                                                }
                                                else
                                                {
                                                    result.steps[++ExecutedSteps].StepFail();
                                                }
                                            }
                                            else
                                            {
                                                result.steps[++ExecutedSteps].StepFail();
                                            }
                                        }
                                        else
                                        {
                                            result.steps[++ExecutedSteps].StepFail();
                                        }
                                    }
                                    else
                                    {
                                        result.steps[++ExecutedSteps].StepFail();
                                    }
                                }
                                else
                                {
                                    result.steps[++ExecutedSteps].StepFail();
                                }
                            }
                            else
                            {
                                result.steps[++ExecutedSteps].StepFail();
                            }
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].StepFail();
                        }
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 23 :: Click on close button
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 24 :: From the Universal viewer ,Select a 3D supported No Lossy compressed series
                bool step24 = z3dvp.searchandopenstudyin3D(Patientid1, ImageCount1, BluRingZ3DViewerPage.Three_3d_6);
                if (step24)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 25 :: Click the User Settings button from the global toolbar and select 3D settings option move the MPR interactive quality and 3D interactive quality sliders to 100%. and click save.
                MPRInteractive = z3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                PageLoadWait.WaitForFrameLoad(5);
                Interactive3D = z3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (Interactive3D && MPRInteractive)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 26 :: Select the scroll tool from the floating toolbox
                Result = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 27 :: Scroll through the image in MPR result control
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step27 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.ResultPanel, ResultPanel.Size.Width / 4, ResultPanel.Size.Height / 4, ResultPanel.Size.Width / 4, 3 * (ResultPanel.Size.Height / 4));
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step27[0].Equals("") && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 28 :: Scroll through the image in 3D1 control
                Navigation3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                string[] step28 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), ThreeDPanel);
                AfterDrag3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                AfterDrag3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (step28[0].Equals("") && step28[1].Equals("") && Navigation3D1Loc != AfterDrag3D1Loc && Navigation3D2Loc != AfterDrag3D2Loc)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 29 :: Select the window level tool from the toolbox
                Result = z3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(10);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 30 :: Apply the window level on Navigation control 2
                Navigation1LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                string[] step30 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationtwo, Navigation2.Size.Width / 3, Navigation2.Size.Height / 3, Navigation2.Size.Width / 3, 3 * (Navigation2.Size.Height / 3), NavigationPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                if (step30[0].Equals("") && step30[1].Equals("") && step30[2].Equals("") && AfterDragNavigation1LocVal != Navigation1LocVal && AfterDragNavigation2LocVal != Navigation2LocVal && AfterDragNavigation3LocVal != Navigation3LocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 31 :: Apply the window level on MPR result control
                ResultPanelLocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step31 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.ResultPanel, ResultPanel.Size.Width / 4, ResultPanel.Size.Height / 4, ResultPanel.Size.Width / 4, 3 * (ResultPanel.Size.Height / 4));
                AfterDragResultPanelLocVal = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step31[0].Equals("") && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 32 :: Apply the window level on 3D1 control
                string[] step32 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4));
                if (step32[0].Equals(""))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 33 :: Apply the window level on 3D2 control
                string[] step33 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D2, Navigation3D2.Size.Width / 4, Navigation3D2.Size.Height / 4, Navigation3D2.Size.Width / 4, 3 * (Navigation3D2.Size.Height / 4));
                if (step33[0].Equals(""))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 34 :: Select the Roam tool from the floating toolbox
                Pan = z3dvp.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                if (Pan)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 35 :: Click and hold the left mouse button on the image displayed on the navigation control 1 and move the mouse
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step35 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4), NavigationResPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step35[0].Equals("") && step35[1].Equals("") && step35[2].Equals("") && step35[3].Equals("") && Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 36 :: Select the Zoom tool from the floating toolbox
                Zoom = z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                PageLoadWait.WaitForFrameLoad(10);
                if (Zoom)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 37 :: Click and hold the left mouse button on the image displayed on the MPR navigation control 1 and do Zoom in /zoom out by dragging the mouse upwards/downward
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step37 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, (3 * (Navigation1.Size.Height / 4)) + 10, Navigation1.Size.Width / 4, (Navigation1.Size.Height / 4) + 10, NavigationResPanel);
                string[] step37_1 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4), NavigationResPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step37[0].Equals("") && step37[1].Equals("") && step37[2].Equals("") && step37[3].Equals("") && step37_1[0].Equals("") && step37_1[1].Equals("") && step37_1[2].Equals("") && step37_1[3].Equals("") && Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 38 :: Click and hold the left mouse button on the image displayed on the 3D1 control and do Zoom in /zoom out by dragging the mouse upwards/downwards
                string[] step38 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, (3 * (Navigation3D1.Size.Height / 4)) + 10, Navigation3D1.Size.Width / 4, (Navigation3D1.Size.Height / 4) + 10, ThreeDPanel);
                string[] step38_1 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), ThreeDPanel);
                if (step38[0].Equals("") && step38[1].Equals("") && step38_1[0].Equals("") && step38_1[1].Equals(""))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 39 :: Select the Rotate tool from the floating toolbox
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                Rotate = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                PageLoadWait.WaitForFrameLoad(10);
                if (Rotate)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 40 :: Click and hold the left mouse button on the image displayed on the navigation control 3 and do a free rotation
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string[] step40 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationthree, Navigation3.Size.Width / 4, 3 * (Navigation3.Size.Height / 4), Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, NavigationResPanel);
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                if (step40[0].Equals("") && step40[1].Equals("") && step40[2].Equals("") && step40[3].Equals("") && !Navigation1LocVal.Equals(AfterDragNavigation1LocVal) && !Navigation2LocVal.Equals(AfterDragNavigation2LocVal) && !Navigation3LocVal.Equals(AfterDragNavigation3LocVal) && !ResultPanelLocVal.Equals(AfterDragResultPanelLocVal))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 41 :: Click and hold the left mouse button on the image displayed on the 3D control and do a free rotation
                Navigation3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                Navigation3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                string[] step41 = z3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigation3D1, Navigation3D1.Size.Width / 4, 3 * (Navigation3D1.Size.Height / 4), Navigation3D1.Size.Width / 4, Navigation3D1.Size.Height / 4, ThreeDPanel);
                AfterDrag3D1Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                AfterDrag3D2Loc = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                if (step41[0].Equals("") && step41[1].Equals("") && !Navigation3D1Loc.Equals(AfterDrag3D1Loc) && !Navigation3D2Loc.Equals(AfterDrag3D2Loc))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 42 :: Click the User Settings button from the global toolbar and select 3D settings option and move the MPR final quality and 3D final quality sliders lesser 100%. ( Range = 1% to 99%). and click save
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                FinalQuality3D = z3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 90);
                PageLoadWait.WaitForFrameLoad(5);
                MPRFinalQuality = z3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 90);
                if (FinalQuality3D && MPRFinalQuality)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STEP 43 :: Verify on each controls in six up view mode.
                bool ResultValue = true;
                foreach (string value in SixUpViewControls)
                {
                    try
                    {
                        if (!z3dvp.GetCenterBottomAnnotationLocationValue(z3dvp.controlelement(value)).Equals("Lossy Compressed"))
                        {
                            ResultValue = false;
                            Logger.Instance.ErrorLog("SixUp Viewer not contain 'Lossy Compression' annotation in " + value + " control.");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        ResultValue = false;
                        Logger.Instance.ErrorLog("SixUp Viewer not contain 'Lossy Compression' annotation in " + value + " control : " + e.Message);
                        break;
                    }
                    Logger.Instance.InfoLog("SixUp Viewer contain 'Lossy Compression' annotation in " + value + " control");
                }
                if (ResultValue)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }


        public TestCaseResult Test_163400(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar;
                string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                string Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String isstestexelocation = Requirements.Split('|')[0];
                String location2 = Requirements.Split('|')[1];

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02 & 03
                bool Result = brz3dvp.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.Three_3d_6);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Failed to open study in 3D 6:1 Layout");

                //step 04 - 06
                DirectoryInfo di = new DirectoryInfo(location2);
                string firstFileName = di.GetFiles().Select(fi => fi.Name).FirstOrDefault(name => name.Contains(".png"));
                String sourcefilelocation = location2 + "\\" + firstFileName;
                String destinationfilelocation = Config.downloadpath + "\\" + firstFileName;
                File.Copy(sourcefilelocation, destinationfilelocation);
                DateTime fileCreatedDate = File.GetCreationTime(destinationfilelocation);
                String filedatetime = fileCreatedDate.ToString();
                Logger.Instance.InfoLog("file date and time is : " + filedatetime);
                if (File.Exists(destinationfilelocation))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Copying file to a secure location failed");

                //step 07 & 08
                try { KillProcess("isstest"); } catch (Exception e1) { Logger.Instance.InfoLog("isstest.exe not available"); }
                var proc = new Process { StartInfo = { FileName = isstestexelocation } };
                proc.Start();
                if (!proc.HasExited && proc.Threads.Count > 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("launching isstest.exe failed");

                //step 09 & 10
                try
                {
                    WpfObjects wpfobject = new WpfObjects();
                    WpfObjects._application = TestStack.White.Application.Attach(proc);
                    Thread.Sleep(5000);
                    WpfObjects._mainWindow = wpfobject.GetMainWindowByTitle("Image Subsystem Tester");
                    Thread.Sleep(5000);
                    WpfObjects._mainWindow.MenuBar.MenuItem("File").Click();
                    Thread.Sleep(5000);
                    Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.DOWN);
                    Keyboard.Instance.LeaveAllKeys();
                    Thread.Sleep(5000);
                    Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.RETURN);
                    Keyboard.Instance.LeaveAllKeys();
                    Thread.Sleep(15000);
                    var window2 = TestStack.White.Desktop.Instance.Windows().Select(window1 => window1.Title.Equals("Select Image File(s)"));
                    var objfilename = WpfObjects._mainWindow.Get(SearchCriteria.ByControlType(ControlType.Edit).AndByText("File name:"));
                    objfilename.SetValue(destinationfilelocation);
                    Thread.Sleep(5000);
                    var objobutton = WpfObjects._mainWindow.Get(SearchCriteria.ByControlType(ControlType.Button).AndByText("Open"));
                    objobutton.Click();
                    Thread.Sleep(15000);
                    WpfObjects._mainWindow.MenuBar.MenuItem("Tools").Click();
                    Thread.Sleep(5000);
                    for (int i = 1; i <= 7; i++)
                    {
                        Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.UP);
                        Keyboard.Instance.LeaveAllKeys();
                        Thread.Sleep(1000);
                    }
                    Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.RETURN);
                    Keyboard.Instance.LeaveAllKeys();
                    Thread.Sleep(15000);
                    WpfObjects._mainWindow.Get(SearchCriteria.ByControlType(ControlType.Pane).AndByText("Ctrl")).Click();
                    Thread.Sleep(10000);
                    WpfObjects._mainWindow.Get(SearchCriteria.ByControlType(ControlType.Button).AndByText("Copy to Clipboard")).Click();
                    Thread.Sleep(10000);
                    String filepath = Config.downloadpath + "\\" + testid + "_" + ExecutedSteps + "_91.txt";
                    if (Clipboard.ContainsText())
                    {
                        var text = Clipboard.GetText();
                        File.WriteAllText(filepath, text);
                    }
                    string[] readText = File.ReadAllLines(filepath);
                    int counter = 0;
                    try
                    {
                        for (int i = 0; i < readText.Length; i++)
                        {
                            String a = readText[i];
                            Logger.Instance.InfoLog(a);
                            if (readText[i].ToLower().Contains(destinationfilelocation.ToLower()))
                            {
                                counter++;
                                break;
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        Logger.Instance.ErrorLog("No content in the text file");
                        throw new Exception("Image property unavailable for the image");
                    }
                    if (counter > 0)
                    {
                        try { KillProcess("isstest"); } catch (Exception e2) { Logger.Instance.InfoLog("isstest.exe not available"); }
                        result.steps[++ExecutedSteps].StepPass();
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                        throw new Exception("Image property unavailable for the image");
                }
                catch (Exception exp)
                {
                    throw new Exception("Image property unavailable for the image");
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }
    }
}
