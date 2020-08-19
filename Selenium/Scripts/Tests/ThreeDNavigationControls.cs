using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Selenium.Scripts.Tests
{
    class ThreeDNavigationControls : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        public ThreeDNavigationControls(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163243(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //Set up Validation Steps
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 -Login iCA as Administrator
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step:2 - From the Universal viewer ,Select a 3D supported series and Select the MPR option from the drop down
                Boolean step2 = z3dvp.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.MPR);
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

                //step:3 - From smart view drop down select 3D 4:1 view mode 
                Boolean step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
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

                //step:4 :: Click on the 3D navigation control 1.
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Navigation1 , Navigation1.Size.Width/2 , Navigation1.Size.Height/2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //Verification:: 3D navigation 1 will highlighted in Yellow color.
                string step4 = Navigation1.GetAttribute("style");
                if(step4.Contains("rgb(255, 255, 0) solid 3px")  || step4.Contains("rgb(255, 255, 0) solid 1px"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: Hover over the red dotted line at the top of the 3D navigation control 1. 
                Accord.Point Redwpoint = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 3, color: "red", blobval: 18);
                int Xcoordinate = (Int32)Redwpoint.X ;
                int Ycoordinate = (Int32)Redwpoint.Y;
                Actions Act = new Actions(Driver);
                Act.MoveToElement(Navigation1, Xcoordinate, Ycoordinate).Build().Perform();
                //Verification :: The clipping cursor shows up while hovering over the dotted line.
                bool Cursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ClippingCursor);
                if (Cursor)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 5 and 6 :: Click and hold the left mouse button while hovering over the dotted line, then move it down.
                IWebElement Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int Nav3D1Before = z3dvp.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 4, 133, 133, 131, 2);
                new Actions(Driver).MoveToElement(Navigation1, Xcoordinate, Ycoordinate).Click().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation1, Xcoordinate, Navigation1.Size.Height / 2).Release().Build().Perform();
                Thread.Sleep(2000);
                //Verification::The dotted line moves along with the mouse.
                int Nav3D1After = z3dvp.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 5, 133, 133, 131, 2);
                Accord.Point Step6 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 6, color: "red", blobval: 18);
                int Step6_1 = (Int32)Step6.Y;
                if (Step6_1 != Ycoordinate && Nav3D1Before != Nav3D1After)
                { 
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 :: Click and hold the left mouse button while hovering the intersection of the right and bottom dotted lines of the 3D navigation control 1, then move towards the center of the image.
                Nav3D1Before = z3dvp.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                Accord.Point Step7 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 3, color: "red", blobval: 65);
                int Stepx = (Int32)Step7.X;
                int Stepy = (Int32)Step7.Y;
                Actions action = new Actions(Driver);
                action.MoveToElement(Navigation1, Stepx, Stepy).ClickAndHold().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                action.MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2).Release().Build().Perform();
                Nav3D1After = z3dvp.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 2, 133, 133, 131, 2);
                if(Nav3D1Before!= Nav3D1After)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                z3dvp.select3DTools(Z3DTools.Reset);
                //Step 8 :: From smart view drop down select 3D 6:1 view mode
                bool ThreeD6x1 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if(ThreeD6x1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9 :: Select the 3D/MPR option from drop down on the top right corner of the 3D 1 control in order to display the 3D navigation controls.
                bool ChangeViewmode = z3dvp.ChangeViewMode();
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
                //Step 10 :: Repeat steps 5-8 and verify the changes on 3D2 CONTROL
                //Step5::
                Accord.Point Step10 = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 3, color: "red", blobval: 45);
                int Step10_1 = (Int32)Step10.X;
                int Step10_2 = (Int32)Step10.Y;
                Act = new Actions(Driver);
                Act.MoveToElement(Navigation1, Step10_1, Step10_2).Build().Perform();
                Thread.Sleep(1000);
                Cursor = z3dvp.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ClippingCursor);
                //Step6::
                IWebElement Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int Nav3D2Before = z3dvp.LevelOfSelectedColor(Navigation3D2, testid, ExecutedSteps + 1, 133, 133, 131, 2);
                new Actions(Driver).MoveToElement(Navigation1, Step10_1, Step10_2).Click().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation1, Step10_1, Navigation1.Size.Height / 2).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(2);
                int Nav3D2After = z3dvp.LevelOfSelectedColor(Navigation3D2, testid, ExecutedSteps + 2, 133, 133, 131, 2);
                Accord.Point Step = z3dvp.GetIntersectionPoints(Navigation1, testid, ExecutedSteps + 102, color: "red", blobval: 45);
                int Step_1 = (Int32)Step.Y;
                //Step7::
                Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int Step7Before = z3dvp.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 3, 133, 133, 131, 2);
                new Actions(Driver).MoveToElement(Navigation1, Step10_1, Navigation1.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation1, Step10_1, Step10_2).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Release().Build().Perform();
                Thread.Sleep(2000);
                Thread.Sleep(1000);
                int Step7After = z3dvp.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps + 4, 133, 133, 131, 2);
                if (Cursor && Step_1!= Step10_2 && Nav3D2Before!= Nav3D2After && Step7Before!= Step7After)
                {
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }
    }
}
