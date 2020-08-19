using System;
using Selenium.Scripts.Pages;
using System.IO;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using TestStack.White.InputDevices;
using System.Windows.Forms;

namespace Selenium.Scripts.Tests
{
    class CalciumScoring : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public CalciumScoring(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163261(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, BluRingZ3DViewerPage.Close);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                Result = brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                new Actions(Driver).SendKeys("s").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("select this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("s");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("select all contiguous", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("deselect this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("deselect all contiguous", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163262(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02 & 03
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 04
                IList<Double> ScoreValuesBefore4 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 4, 255, 255, 255, 2);
                if (WhiteRegion > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                int GreenColorBefore5 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 51, 0, 255, 0, 2);
                int[] x5 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y5 = { CalciumScoreImage.Size.Height / 2 + 10 , CalciumScoreImage.Size.Height / 2 + 200 , CalciumScoreImage.Size.Height / 2 + 200 , CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x5, y5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter5 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 52, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter5 = brz3dvp.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("RCA Score Value Before 5 : " + (ScoreValuesBefore4[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 5 : " + (ScoreValuesBefore4[1]));
                Logger.Instance.InfoLog("RCA Score Value After 5 : " + (ScoreValuesAfter5[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 5 : " + (ScoreValuesAfter5[1]));
                if (GreenColorAfter5 != GreenColorBefore5 && ((ScoreValuesBefore4[0]) < (ScoreValuesAfter5[0])) && ((ScoreValuesBefore4[1]) < (ScoreValuesAfter5[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.Verifyradiobuttonenabled("deselect this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x7 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y7 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x7, y7);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter7 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 7, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter7 = brz3dvp.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("RCA Score Value Before Step 7 : " + (ScoreValuesAfter7[0]));
                Logger.Instance.InfoLog("RCA Volume Value After Step 7 : " + (ScoreValuesAfter7[1]));
                if (GreenColorAfter7 < GreenColorAfter5 && ((ScoreValuesAfter7[0]) == 0.00) && ((ScoreValuesAfter7[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<Double> ScoreValuesBefore8 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 255, 255, 255, 2);
                if (WhiteRegion8 > 0)
                {
                    int GreenColorBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 82, 0, 255, 0, 2);
                    int[] x8 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y8 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenColorAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 83, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter8 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value Before 8 : " + (ScoreValuesBefore8[0]));
                    Logger.Instance.InfoLog("RCA Volume Value Before 8 : " + (ScoreValuesBefore8[1]));
                    Logger.Instance.InfoLog("RCA Score Value After 8 : " + (ScoreValuesAfter8[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 8 : " + (ScoreValuesAfter8[1]));
                    if (GreenColorAfter8 != GreenColorBefore8 && ((ScoreValuesBefore8[0]) < (ScoreValuesAfter8[0])) && ((ScoreValuesBefore8[1]) < (ScoreValuesAfter8[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                IList<Double> ScoreValuesBefore09 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore09 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter09 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter09 != LocValBefore09)
                {
                    int GreenColorBefore09 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 0, 255, 0, 2);
                    int[] x9 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y9 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x9, y9);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenColorAfter09 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter09 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value Before 9 : " + (ScoreValuesBefore09[0]));
                    Logger.Instance.InfoLog("RCA Volume Value Before 9 : " + (ScoreValuesBefore09[1]));
                    Logger.Instance.InfoLog("RCA Score Value After 9 : " + (ScoreValuesAfter09[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 9 : " + (ScoreValuesAfter09[1]));
                    if (GreenColorAfter09 != GreenColorBefore09 && ((ScoreValuesBefore09[0]) < (ScoreValuesAfter09[0])) && ((ScoreValuesBefore09[1]) < (ScoreValuesAfter09[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.Verifyradiobuttonenabled("deselect this slice");
                if (Result)
                {
                    IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("RCA");
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                    int GreenColorBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 0, 255, 0, 2);
                    int[] x10 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y10 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenColorAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                    Logger.Instance.InfoLog("RCA Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                    Logger.Instance.InfoLog("RCA Score Value After 10 : " + (ScoreValuesAfter10[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                    if (GreenColorAfter10 != GreenColorBefore10 && ((ScoreValuesBefore10[0]) > (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) > (ScoreValuesAfter10[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                IList<Double> ScoreValuesBefore11 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore11 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
             
                    String LocValAfter11 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                    if (LocValAfter11 != LocValBefore11)
                    {
                        int GreenColorBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 111, 0, 255, 0, 2);
                        int[] x11 = { CalciumScoreImage.Size.Width / 2 - 50, CalciumScoreImage.Size.Width / 2 - 115, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 20};
                        int[] y11 = { CalciumScoreImage.Size.Height / 2 , CalciumScoreImage.Size.Height / 2 + 225, CalciumScoreImage.Size.Height / 2 + 225, CalciumScoreImage.Size.Height / 2 + 10};
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.drawselectedtool(CalciumScoreImage, x11, y11);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                        int GreenColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 255, 0, 2);
                        brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                        PageLoadWait.WaitForFrameLoad(5);
                        IList<Double> ScoreValuesAfter11 = brz3dvp.CalciumScoringTableValues("RCA");
                        Logger.Instance.InfoLog("RCA Score Value Before 11 : " + (ScoreValuesBefore11[0]));
                        Logger.Instance.InfoLog("RCA Volume Value Before 11 : " + (ScoreValuesBefore11[1]));
                        Logger.Instance.InfoLog("RCA Score Value After 11 : " + (ScoreValuesAfter11[0]));
                        Logger.Instance.InfoLog("RCA Volume Value After 11 : " + (ScoreValuesAfter11[1]));
                        if (GreenColorAfter11 != GreenColorBefore11 && ((ScoreValuesBefore11[0]) > (ScoreValuesAfter11[0])) && ((ScoreValuesBefore11[1]) > (ScoreValuesAfter11[1])) && (ScoreValuesAfter11[0]) == 0.00 && (ScoreValuesAfter11[1]) == 0.00)
                            result.steps[++ExecutedSteps].StepPass();
                        else
                            result.steps[++ExecutedSteps].StepFail();
                    }
                    else
                        result.steps[++ExecutedSteps].StepFail();
              

                //step 12
                IList<Double> ScoreValuesBefore12 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegionbefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 121, 255, 255, 255, 2);
                int GreenColorBefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 122, 0, 255, 0, 2);
                int[] x12 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y12 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x12, y12);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 123, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter12 = brz3dvp.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("RCA Score Value Before 12 : " + (ScoreValuesBefore12[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 12 : " + (ScoreValuesBefore12[1]));
                Logger.Instance.InfoLog("RCA Score Value After 12 : " + (ScoreValuesAfter12[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 12 : " + (ScoreValuesAfter12[1]));
                if (GreenColorAfter12 != GreenColorBefore12 && ((ScoreValuesBefore12[0]) < (ScoreValuesAfter12[0])) && ((ScoreValuesBefore12[1]) < (ScoreValuesAfter12[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("deselect all contiguous", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(5);
                int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y14 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter14 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 141, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter14 = brz3dvp.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("RCA Score Value After Step 14 : " + (ScoreValuesAfter14[0]));
                Logger.Instance.InfoLog("RCA Volume Value After Step 14 : " + (ScoreValuesAfter14[1]));
                if (GreenColorAfter14 < GreenColorAfter12 && ((ScoreValuesAfter14[0]) == 0.00) && ((ScoreValuesAfter14[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                IList<Double> ScoreValuesBefore15 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion15 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 151, 255, 255, 255, 2);
                if (WhiteRegion15 > 0)
                {
                    int GreenColorBefore15 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 152, 0, 255, 0, 2);
                    int[] x15 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y15 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x15, y15);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenColorAfter15 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 153, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter15 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value Before 15 : " + (ScoreValuesBefore15[0]));
                    Logger.Instance.InfoLog("RCA Volume Value Before 15 : " + (ScoreValuesBefore15[1]));
                    Logger.Instance.InfoLog("RCA Score Value After 15 : " + (ScoreValuesAfter15[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 15 : " + (ScoreValuesAfter15[1]));
                    if (GreenColorAfter15 != GreenColorBefore15 && ((ScoreValuesBefore15[0]) < (ScoreValuesAfter15[0])) && ((ScoreValuesBefore15[1]) < (ScoreValuesAfter15[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore16 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter16 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter16 != LocValBefore16)
                {
                    int GreenColorBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 0, 255, 0, 2);
                    PageLoadWait.WaitForFrameLoad(5);
                    int[] x16 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y16 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenColorAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value Before 16 : " + (ScoreValuesBefore16[0]));
                    Logger.Instance.InfoLog("RCA Volume Value Before 16 : " + (ScoreValuesBefore16[1]));
                    Logger.Instance.InfoLog("RCA Score Value After 16 : " + (ScoreValuesAfter16[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 16 : " + (ScoreValuesAfter16[1]));
                    if (GreenColorAfter16 != GreenColorBefore16 && ((ScoreValuesBefore16[0]) < (ScoreValuesAfter16[0])) && ((ScoreValuesBefore16[1]) < (ScoreValuesAfter16[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                IList<Double> ScoreValuesBefore17 = brz3dvp.CalciumScoringTableValues("RCA");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                if (Result)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    int GreenColorBefore17 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 171, 0, 255, 0, 2);
                    PageLoadWait.WaitForFrameLoad(5);
                    int[] x17 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y17 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x17, y17);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenColorAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter17 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value Before 17 : " + (ScoreValuesBefore17[0]));
                    Logger.Instance.InfoLog("RCA Volume Value Before 17 : " + (ScoreValuesBefore17[1]));
                    Logger.Instance.InfoLog("RCA Score Value After 17 : " + (ScoreValuesAfter17[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 17 : " + (ScoreValuesAfter17[1]));
                    if (GreenColorAfter17 != GreenColorBefore17 && ((ScoreValuesBefore17[0]) > (ScoreValuesAfter17[0])) && ((ScoreValuesBefore17[1]) > (ScoreValuesAfter17[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                IList<Double> ScoreValuesBefore18 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore18 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter18 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter18 != LocValBefore18)
                {
                    int GreenColorBefore18 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 181, 0, 255, 0, 2);
                    int[] x18 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y18 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x18, y18);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenColorAfter18 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 182, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter18 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value Before 18 : " + (ScoreValuesBefore18[0]));
                    Logger.Instance.InfoLog("RCA Volume Value Before 18 : " + (ScoreValuesBefore18[1]));
                    Logger.Instance.InfoLog("RCA Score Value After 18 : " + (ScoreValuesAfter18[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 18 : " + (ScoreValuesAfter18[1]));
                    if (GreenColorAfter18 != GreenColorBefore18 && ((ScoreValuesBefore18[0]) > (ScoreValuesAfter18[0])) && ((ScoreValuesBefore18[1]) > (ScoreValuesAfter18[1])) && (ScoreValuesAfter18[0]) == 0.00 && (ScoreValuesAfter18[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
               

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

        public TestCaseResult Test_163263(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02 & 03
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 04
                IList<Double> ScoreValuesBefore4 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("s").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 4, 255, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.Verifyradiobuttonenabled("select this slice") && WhiteRegion > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int RedBefore5 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 51, 255, 0, 0, 2);
                int[] x5 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y5 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x5, y5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedAfter5 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 52, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter5 = brz3dvp.CalciumScoringTableValues("LM");
                Logger.Instance.InfoLog("LM Score Value Before 5 : " + (ScoreValuesBefore4[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 5 : " + (ScoreValuesBefore4[1]));
                Logger.Instance.InfoLog("LM Score Value After 5 : " + (ScoreValuesAfter5[0]));
                Logger.Instance.InfoLog("LM Volume Value After 5 : " + (ScoreValuesAfter5[1]));
                if (RedAfter5 != RedBefore5 && ((ScoreValuesBefore4[0]) < (ScoreValuesAfter5[0])) && ((ScoreValuesBefore4[1]) < (ScoreValuesAfter5[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.Verifyradiobuttonenabled("deselect this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x7 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y7 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x7, y7);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedAfter7 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 7, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter7 = brz3dvp.CalciumScoringTableValues("LM");
                Logger.Instance.InfoLog("LM Score Value Before Step 7 : " + (ScoreValuesAfter7[0]));
                Logger.Instance.InfoLog("LM Volume Value After Step 7 : " + (ScoreValuesAfter7[1]));
                if (RedAfter7 < RedAfter5 && ((ScoreValuesAfter7[0]) == 0.00) && ((ScoreValuesAfter7[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<Double> ScoreValuesBefore8 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 255, 255, 255, 2);
                if (WhiteRegion8 > 0)
                {
                    int RedBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 82, 255, 0, 0, 2);
                    int[] x8 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y8 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 83, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter8 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 8 : " + (ScoreValuesBefore8[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 8 : " + (ScoreValuesBefore8[1]));
                    Logger.Instance.InfoLog("LM Score Value After 8 : " + (ScoreValuesAfter8[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 8 : " + (ScoreValuesAfter8[1]));
                    if (RedAfter8 != RedBefore8 && ((ScoreValuesBefore8[0]) < (ScoreValuesAfter8[0])) && ((ScoreValuesBefore8[1]) < (ScoreValuesAfter8[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                IList<Double> ScoreValuesBefore09 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore09 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter09 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter09 != LocValBefore09)
                {
                    int RedBefore09 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 255, 0, 0, 2);
                    int[] x9 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y9 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x9, y9);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter09 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter09 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 9 : " + (ScoreValuesBefore09[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 9 : " + (ScoreValuesBefore09[1]));
                    Logger.Instance.InfoLog("LM Score Value After 9 : " + (ScoreValuesAfter09[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 9 : " + (ScoreValuesAfter09[1]));
                    if (RedAfter09 != RedBefore09 && ((ScoreValuesBefore09[0]) < (ScoreValuesAfter09[0])) && ((ScoreValuesBefore09[1]) < (ScoreValuesAfter09[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.Verifyradiobuttonenabled("deselect this slice");
                if (Result)
                {
                    IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("LM");
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                    int RedBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 255, 0, 0, 2);
                    int[] x10 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y10 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                    Logger.Instance.InfoLog("LM Score Value After 10 : " + (ScoreValuesAfter10[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                    if (RedAfter10 != RedBefore10 && ((ScoreValuesBefore10[0]) > (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) > (ScoreValuesAfter10[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                IList<Double> ScoreValuesBefore11 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore11 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter11 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter11 != LocValBefore11)
                {
                    int RedBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 111, 255, 0, 0, 2);
                    int[] x11 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y11 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x11, y11);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(15);
                    IList<Double> ScoreValuesAfter11 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 11 : " + (ScoreValuesBefore11[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 11 : " + (ScoreValuesBefore11[1]));
                    Logger.Instance.InfoLog("LM Score Value After 11 : " + (ScoreValuesAfter11[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 11 : " + (ScoreValuesAfter11[1]));
                    if (RedAfter11 != RedBefore11 && ((ScoreValuesBefore11[0]) > (ScoreValuesAfter11[0])) && ((ScoreValuesBefore11[1]) > (ScoreValuesAfter11[1])) && (ScoreValuesAfter11[0]) == 0.00 && (ScoreValuesAfter11[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
              

                //step 12
                IList<Double> ScoreValuesBefore12 = brz3dvp.CalciumScoringTableValues("LM");
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("s");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(10);
                int RedAfter12 = 0;
                if (brz3dvp.Verifyradiobuttonenabled("select all contiguous", "y"))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                    PageLoadWait.WaitForFrameLoad(5);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    int WhiteRegionbefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 121, 255, 255, 255, 2);
                    int RedBefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 122, 255, 0, 0, 2);
                    int[] x12 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y12 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x12, y12);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    RedAfter12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 123, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter12 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 12 : " + (ScoreValuesBefore12[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 12 : " + (ScoreValuesBefore12[1]));
                    Logger.Instance.InfoLog("LM Score Value After 12 : " + (ScoreValuesAfter12[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 12 : " + (ScoreValuesAfter12[1]));
                    if (RedAfter12 != RedBefore12 && ((ScoreValuesBefore12[0]) < (ScoreValuesAfter12[0])) && ((ScoreValuesBefore12[1]) < (ScoreValuesAfter12[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.Verifyradiobuttonenabled("deselect all contiguous", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y14 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedAfter14 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 141, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter14 = brz3dvp.CalciumScoringTableValues("LM");
                Logger.Instance.InfoLog("LM Score Value After Step 14 : " + (ScoreValuesAfter14[0]));
                Logger.Instance.InfoLog("LM Volume Value After Step 14 : " + (ScoreValuesAfter14[1]));
                if (RedAfter14 < RedAfter12 && ((ScoreValuesAfter14[0]) == 0.00) && ((ScoreValuesAfter14[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                IList<Double> ScoreValuesBefore15 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion15 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 151, 255, 255, 255, 2);
                if (WhiteRegion15 > 0)
                {
                    int RedBefore15 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 152, 255, 0, 0, 2);
                    PageLoadWait.WaitForFrameLoad(5);
                    int[] x15 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y15 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x15, y15);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter15 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 153, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter15 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 15 : " + (ScoreValuesBefore15[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 15 : " + (ScoreValuesBefore15[1]));
                    Logger.Instance.InfoLog("LM Score Value After 15 : " + (ScoreValuesAfter15[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 15 : " + (ScoreValuesAfter15[1]));
                    if (RedAfter15 != RedBefore15 && ((ScoreValuesBefore15[0]) < (ScoreValuesAfter15[0])) && ((ScoreValuesBefore15[1]) < (ScoreValuesAfter15[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore16 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter16 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter16 != LocValBefore16)
                {
                    int RedBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 255, 0, 0, 2);
                    PageLoadWait.WaitForFrameLoad(5);
                    int[] x16 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y16 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 16 : " + (ScoreValuesBefore16[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 16 : " + (ScoreValuesBefore16[1]));
                    Logger.Instance.InfoLog("LM Score Value After 16 : " + (ScoreValuesAfter16[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 16 : " + (ScoreValuesAfter16[1]));
                    if (RedAfter16 != RedBefore16 && ((ScoreValuesBefore16[0]) < (ScoreValuesAfter16[0])) && ((ScoreValuesBefore16[1]) < (ScoreValuesAfter16[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                IList<Double> ScoreValuesBefore17 = brz3dvp.CalciumScoringTableValues("LM");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect all contiguous");
                if (Result)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    int RedBefore17 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 171, 255, 0, 0, 2);
                    PageLoadWait.WaitForFrameLoad(5);
                    int[] x17 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y17 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x17, y17);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter17 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 17 : " + (ScoreValuesBefore17[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 17 : " + (ScoreValuesBefore17[1]));
                    Logger.Instance.InfoLog("LM Score Value After 17 : " + (ScoreValuesAfter17[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 17 : " + (ScoreValuesAfter17[1]));
                    if (RedAfter17 != RedBefore17 && ((ScoreValuesBefore17[0]) > (ScoreValuesAfter17[0])) && ((ScoreValuesBefore17[1]) > (ScoreValuesAfter17[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                IList<Double> ScoreValuesBefore18 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore18 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter18 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter18 != LocValBefore18)
                {
                    int RedBefore18 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 181, 255, 0, 0, 2);
                    int[] x18 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y18 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x18, y18);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int RedAfter18 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 182, 255, 0, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter18 = brz3dvp.CalciumScoringTableValues("LM");
                    Logger.Instance.InfoLog("LM Score Value Before 18 : " + (ScoreValuesBefore18[0]));
                    Logger.Instance.InfoLog("LM Volume Value Before 18 : " + (ScoreValuesBefore18[1]));
                    Logger.Instance.InfoLog("LM Score Value After 18 : " + (ScoreValuesAfter18[0]));
                    Logger.Instance.InfoLog("LM Volume Value After 18 : " + (ScoreValuesAfter18[1]));
                    if (RedAfter18 != RedBefore18 && ((ScoreValuesBefore18[0]) > (ScoreValuesAfter18[0])) && ((ScoreValuesBefore18[1]) > (ScoreValuesAfter18[1])) && (ScoreValuesAfter18[0]) == 0.00 && (ScoreValuesAfter18[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
             


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

        public TestCaseResult Test_163264(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 4, 255, 255, 255, 2);
                if (WhiteRegion > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                int BlueBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                int[] x4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4, y4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 52, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter4 = brz3dvp.CalciumScoringTableValues("LAD");
                Logger.Instance.InfoLog("LAD Score Value Before 4 : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 4 : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("LAD Score Value After 4 : " + (ScoreValuesAfter4[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 4 : " + (ScoreValuesAfter4[1]));
                if (BlueAfter4 != BlueBefore4 && ((ScoreValuesBefore3[0]) < (ScoreValuesAfter4[0])) && ((ScoreValuesBefore3[1]) < (ScoreValuesAfter4[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.Verifyradiobuttonenabled("deselect this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x6 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y6 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6, y6);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 7, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter6 = brz3dvp.CalciumScoringTableValues("LAD");
                Logger.Instance.InfoLog("LAD Score Value Before Step 6 : " + (ScoreValuesAfter6[0]));
                Logger.Instance.InfoLog("LAD Volume Value After Step 6 : " + (ScoreValuesAfter6[1]));
                if (BlueAfter6 < BlueAfter4 && ((ScoreValuesAfter6[0]) == 0.00) && ((ScoreValuesAfter6[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                IList<Double> ScoreValuesBefore7 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 255, 255, 255, 2);
                if (WhiteRegion8 > 0)
                {
                    int BlueBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 82, 0, 0, 255, 2);
                    int[] x7 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y7 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x7, y7);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 83, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter7 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 7 : " + (ScoreValuesBefore7[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 7 : " + (ScoreValuesBefore7[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 7 : " + (ScoreValuesAfter7[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 7 : " + (ScoreValuesAfter7[1]));
                    if (BlueAfter8 != BlueBefore8 && ((ScoreValuesBefore7[0]) < (ScoreValuesAfter7[0])) && ((ScoreValuesBefore7[1]) < (ScoreValuesAfter7[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<Double> ScoreValuesBefore08 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore08 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter08 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter08 != LocValBefore08)
                {
                    int BlueBefore08 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 0, 0, 255, 2);
                    int[] x8 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y8 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter08 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter08 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 8 : " + (ScoreValuesBefore08[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 8 : " + (ScoreValuesBefore08[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 8 : " + (ScoreValuesAfter08[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 8 : " + (ScoreValuesAfter08[1]));
                    if (BlueAfter08 != BlueBefore08 && ((ScoreValuesBefore08[0]) < (ScoreValuesAfter08[0])) && ((ScoreValuesBefore08[1]) < (ScoreValuesAfter08[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.Verifyradiobuttonenabled("deselect this slice");
                if (Result)
                {
                    IList<Double> ScoreValuesBefore9 = brz3dvp.CalciumScoringTableValues("LAD");
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                    int BlueBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 0, 0, 255, 2);
                    int[] x9 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y9 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x9, y9);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter9 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 9 : " + (ScoreValuesBefore9[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 9 : " + (ScoreValuesBefore9[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 9 : " + (ScoreValuesAfter9[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 9 : " + (ScoreValuesAfter9[1]));
                    if (BlueAfter10 != BlueBefore10 && ((ScoreValuesBefore9[0]) > (ScoreValuesAfter9[0])) && ((ScoreValuesBefore9[1]) > (ScoreValuesAfter9[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter10 != LocValBefore10)
                {
                    int BlueBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 111, 0, 0, 255, 2);
                    int[] x10 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y10 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 10 : " + (ScoreValuesAfter10[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                    if (BlueAfter10 != BlueBefore10 && ((ScoreValuesBefore10[0]) > (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) > (ScoreValuesAfter10[1])) && (ScoreValuesAfter10[0]) == 0.00 && (ScoreValuesAfter10[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                IList<Double> ScoreValuesBefore11 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegionbefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 121, 255, 255, 255, 2);
                int BlueBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 122, 0, 0, 255, 2);
                int[] x11 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y11 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x11, y11);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 123, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter11 = brz3dvp.CalciumScoringTableValues("LAD");
                Logger.Instance.InfoLog("LAD Score Value Before 11 : " + (ScoreValuesBefore11[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 11 : " + (ScoreValuesBefore11[1]));
                Logger.Instance.InfoLog("LAD Score Value After 11 : " + (ScoreValuesAfter11[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 11 : " + (ScoreValuesAfter11[1]));
                if (BlueAfter11 != BlueBefore11 && ((ScoreValuesBefore11[0]) < (ScoreValuesAfter11[0])) && ((ScoreValuesBefore11[1]) < (ScoreValuesAfter11[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("deselect all contiguous", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(5);
                int[] x13 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y13 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x13, y13);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueAfter13 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 141, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(15);
                IList<Double> ScoreValuesAfter13 = brz3dvp.CalciumScoringTableValues("LAD");
                Logger.Instance.InfoLog("LAD Score Value After Step 14 : " + (ScoreValuesAfter13[0]));
                Logger.Instance.InfoLog("LAD Volume Value After Step 14 : " + (ScoreValuesAfter13[1]));
                if (BlueAfter13 < BlueAfter11 && ((ScoreValuesAfter13[0]) == 0.00) && ((ScoreValuesAfter13[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                IList<Double> ScoreValuesBefore14 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion14 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 151, 255, 255, 255, 2);
                if (WhiteRegion14 > 0)
                {
                    int BlueBefore14 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 152, 0, 0, 255, 2);
                    int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y14 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter14 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 153, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter14 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 14 : " + (ScoreValuesBefore14[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 14 : " + (ScoreValuesBefore14[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 14 : " + (ScoreValuesAfter14[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 14 : " + (ScoreValuesAfter14[1]));
                    if (BlueAfter14 != BlueBefore14 && ((ScoreValuesBefore14[0]) < (ScoreValuesAfter14[0])) && ((ScoreValuesBefore14[1]) < (ScoreValuesAfter14[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                IList<Double> ScoreValuesBefore15 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore15 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter15 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter15 != LocValBefore15)
                {
                    int BlueBefore15 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 0, 0, 255, 2);
                    int[] x15 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y15 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x15, y15);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter15 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter15 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 15 : " + (ScoreValuesBefore15[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 15 : " + (ScoreValuesBefore15[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 15 : " + (ScoreValuesAfter15[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 15 : " + (ScoreValuesAfter15[1]));
                    if (BlueAfter15 != BlueBefore15 && ((ScoreValuesBefore15[0]) < (ScoreValuesAfter15[0])) && ((ScoreValuesBefore15[1]) < (ScoreValuesAfter15[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("LAD");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect all contiguous");
                if (Result)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    int BlueBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 171, 0, 0, 255, 2);
                    int[] x16 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y16 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 16 : " + (ScoreValuesBefore16[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 16 : " + (ScoreValuesBefore16[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 16 : " + (ScoreValuesAfter16[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 16 : " + (ScoreValuesAfter16[1]));
                    if (BlueAfter16 != BlueBefore16 && ((ScoreValuesBefore16[0]) > (ScoreValuesAfter16[0])) && ((ScoreValuesBefore16[1]) > (ScoreValuesAfter16[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                IList<Double> ScoreValuesBefore17 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter17 != LocValBefore17)
                {
                    int BlueBefore17 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 181, 0, 0, 255, 2);
                    int[] x17 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y17 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x17, y17);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 182, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter17 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value Before 17 : " + (ScoreValuesBefore17[0]));
                    Logger.Instance.InfoLog("LAD Volume Value Before 17 : " + (ScoreValuesBefore17[1]));
                    Logger.Instance.InfoLog("LAD Score Value After 17 : " + (ScoreValuesAfter17[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 17 : " + (ScoreValuesAfter17[1]));
                    if (BlueAfter17 != BlueBefore17 && ((ScoreValuesBefore17[0]) > (ScoreValuesAfter17[0])) && ((ScoreValuesBefore17[1]) > (ScoreValuesAfter17[1])) && (ScoreValuesAfter17[0]) == 0.00 && (ScoreValuesAfter17[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163265(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 4, 255, 255, 255, 2);
                if (WhiteRegion > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                int YellowBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 51, 255, 255, 0, 2);
                int[] x4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4, y4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 52, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter4 = brz3dvp.CalciumScoringTableValues("CX");
                Logger.Instance.InfoLog("CX Score Value Before 4 : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("CX Volume Value Before 4 : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("CX Score Value After 4 : " + (ScoreValuesAfter4[0]));
                Logger.Instance.InfoLog("CX Volume Value After 4 : " + (ScoreValuesAfter4[1]));
                if (YellowAfter4 != YellowBefore4 && ((ScoreValuesBefore3[0]) < (ScoreValuesAfter4[0])) && ((ScoreValuesBefore3[1]) < (ScoreValuesAfter4[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.Verifyradiobuttonenabled("deselect this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x6 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y6 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6, y6);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 7, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter6 = brz3dvp.CalciumScoringTableValues("CX");
                Logger.Instance.InfoLog("CX Score Value Before Step 6 : " + (ScoreValuesAfter6[0]));
                Logger.Instance.InfoLog("CX Volume Value After Step 6 : " + (ScoreValuesAfter6[1]));
                if (YellowAfter6 < YellowAfter4 && ((ScoreValuesAfter6[0]) == 0.00) && ((ScoreValuesAfter6[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                IList<Double> ScoreValuesBefore7 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 255, 255, 255, 2);
                if (WhiteRegion8 > 0)
                {
                    int YellowBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 82, 255, 255, 0, 2);
                    int[] x7 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y7 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x7, y7);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 83, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter7 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 7 : " + (ScoreValuesBefore7[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 7 : " + (ScoreValuesBefore7[1]));
                    Logger.Instance.InfoLog("CX Score Value After 7 : " + (ScoreValuesAfter7[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 7 : " + (ScoreValuesAfter7[1]));
                    if (YellowAfter8 != YellowBefore8 && ((ScoreValuesBefore7[0]) < (ScoreValuesAfter7[0])) && ((ScoreValuesBefore7[1]) < (ScoreValuesAfter7[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<Double> ScoreValuesBefore08 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore08 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter08 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter08 != LocValBefore08)
                {
                    int YellowBefore08 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 255, 255, 0, 2);
                    int[] x8 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y8 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter08 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter08 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 8 : " + (ScoreValuesBefore08[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 8 : " + (ScoreValuesBefore08[1]));
                    Logger.Instance.InfoLog("CX Score Value After 8 : " + (ScoreValuesAfter08[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 8 : " + (ScoreValuesAfter08[1]));
                    if (YellowAfter08 != YellowBefore08 && ((ScoreValuesBefore08[0]) < (ScoreValuesAfter08[0])) && ((ScoreValuesBefore08[1]) < (ScoreValuesAfter08[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.Verifyradiobuttonenabled("deselect this slice");
                if (Result)
                {
                    IList<Double> ScoreValuesBefore9 = brz3dvp.CalciumScoringTableValues("CX");
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                    int YellowBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 255, 255, 0, 2);
                    int[] x9 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y9 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x9, y9);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter9 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 9 : " + (ScoreValuesBefore9[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 9 : " + (ScoreValuesBefore9[1]));
                    Logger.Instance.InfoLog("CX Score Value After 9 : " + (ScoreValuesAfter9[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 9 : " + (ScoreValuesAfter9[1]));
                    if (YellowAfter10 != YellowBefore10 && ((ScoreValuesBefore9[0]) > (ScoreValuesAfter9[0])) && ((ScoreValuesBefore9[1]) > (ScoreValuesAfter9[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter10 != LocValBefore10)
                {
                    int YellowBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 111, 255, 255, 0, 2);
                    int[] x10 = { CalciumScoreImage.Size.Width / 2 - 50, CalciumScoreImage.Size.Width / 2 - 115, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 20 };
                    int[] y10 = { CalciumScoreImage.Size.Height / 2, CalciumScoreImage.Size.Height / 2 + 225, CalciumScoreImage.Size.Height / 2 + 225, CalciumScoreImage.Size.Height / 2 + 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                    Logger.Instance.InfoLog("CX Score Value After 10 : " + (ScoreValuesAfter10[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                    if (YellowAfter10 != YellowBefore10 && ((ScoreValuesBefore10[0]) > (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) > (ScoreValuesAfter10[1])) && (ScoreValuesAfter10[0]) == 0.00 && (ScoreValuesAfter10[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
              
                //step 11
                IList<Double> ScoreValuesBefore11 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegionbefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 121, 255, 255, 255, 2);
                int YellowBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 122, 255, 255, 0, 2);
                int[] x11 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y11 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x11, y11);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 123, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter11 = brz3dvp.CalciumScoringTableValues("CX");
                Logger.Instance.InfoLog("CX Score Value Before 11 : " + (ScoreValuesBefore11[0]));
                Logger.Instance.InfoLog("CX Volume Value Before 11 : " + (ScoreValuesBefore11[1]));
                Logger.Instance.InfoLog("CX Score Value After 11 : " + (ScoreValuesAfter11[0]));
                Logger.Instance.InfoLog("CX Volume Value After 11 : " + (ScoreValuesAfter11[1]));
                if (YellowAfter11 != YellowBefore11 && ((ScoreValuesBefore11[0]) < (ScoreValuesAfter11[0])) && ((ScoreValuesBefore11[1]) < (ScoreValuesAfter11[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("deselect all contiguous", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(5);
                int[] x13 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y13 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x13, y13);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowAfter13 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 141, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter13 = brz3dvp.CalciumScoringTableValues("CX");
                Logger.Instance.InfoLog("CX Score Value After Step 14 : " + (ScoreValuesAfter13[0]));
                Logger.Instance.InfoLog("CX Volume Value After Step 14 : " + (ScoreValuesAfter13[1]));
                if (YellowAfter13 < YellowAfter11 && ((ScoreValuesAfter13[0]) == 0.00) && ((ScoreValuesAfter13[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                IList<Double> ScoreValuesBefore14 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion14 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 151, 255, 255, 255, 2);
                if (WhiteRegion14 > 0)
                {
                    int YellowBefore14 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 152, 255, 255, 0, 2);
                    int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y14 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter14 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 153, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(15);
                    IList<Double> ScoreValuesAfter14 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 14 : " + (ScoreValuesBefore14[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 14 : " + (ScoreValuesBefore14[1]));
                    Logger.Instance.InfoLog("CX Score Value After 14 : " + (ScoreValuesAfter14[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 14 : " + (ScoreValuesAfter14[1]));
                    if (YellowAfter14 != YellowBefore14 && ((ScoreValuesBefore14[0]) < (ScoreValuesAfter14[0])) && ((ScoreValuesBefore14[1]) < (ScoreValuesAfter14[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                IList<Double> ScoreValuesBefore15 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore15 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter15 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter15 != LocValBefore15)
                {
                    int YellowBefore15 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 255, 255, 0, 2);
                    int[] x15 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y15 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x15, y15);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter15 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter15 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 15 : " + (ScoreValuesBefore15[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 15 : " + (ScoreValuesBefore15[1]));
                    Logger.Instance.InfoLog("CX Score Value After 15 : " + (ScoreValuesAfter15[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 15 : " + (ScoreValuesAfter15[1]));
                    if (YellowAfter15 != YellowBefore15 && ((ScoreValuesBefore15[0]) < (ScoreValuesAfter15[0])) && ((ScoreValuesBefore15[1]) < (ScoreValuesAfter15[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("CX");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect all contiguous");
                if (Result)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    int YellowBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 171, 255, 255, 0, 2);
                    int[] x16 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y16 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 16 : " + (ScoreValuesBefore16[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 16 : " + (ScoreValuesBefore16[1]));
                    Logger.Instance.InfoLog("CX Score Value After 16 : " + (ScoreValuesAfter16[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 16 : " + (ScoreValuesAfter16[1]));
                    if (YellowAfter16 != YellowBefore16 && ((ScoreValuesBefore16[0]) > (ScoreValuesAfter16[0])) && ((ScoreValuesBefore16[1]) > (ScoreValuesAfter16[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                IList<Double> ScoreValuesBefore17 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter17 != LocValBefore17)
                {
                    int YellowBefore17 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 181, 255, 255, 0, 2);
                    int[] x17 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y17 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x17, y17);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 182, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter17 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value Before 17 : " + (ScoreValuesBefore17[0]));
                    Logger.Instance.InfoLog("CX Volume Value Before 17 : " + (ScoreValuesBefore17[1]));
                    Logger.Instance.InfoLog("CX Score Value After 17 : " + (ScoreValuesAfter17[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 17 : " + (ScoreValuesAfter17[1]));
                    if (YellowAfter17 != YellowBefore17 && ((ScoreValuesBefore17[0]) > (ScoreValuesAfter17[0])) && ((ScoreValuesBefore17[1]) > (ScoreValuesAfter17[1])) && (ScoreValuesAfter17[0]) == 0.00 && (ScoreValuesAfter17[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
               
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

        public TestCaseResult Test_163266(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 4, 255, 255, 255, 2);
                if (WhiteRegion > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                int GreenBlueBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 51, 0, 255, 255, 2);
                int[] x4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4, y4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenBlueAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 52, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter4 = brz3dvp.CalciumScoringTableValues("PDA");
                Logger.Instance.InfoLog("PDA Score Value Before 4 : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("PDA Volume Value Before 4 : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("PDA Score Value After 4 : " + (ScoreValuesAfter4[0]));
                Logger.Instance.InfoLog("PDA Volume Value After 4 : " + (ScoreValuesAfter4[1]));
                if (GreenBlueAfter4 != GreenBlueBefore4 && ((ScoreValuesBefore3[0]) < (ScoreValuesAfter4[0])) && ((ScoreValuesBefore3[1]) < (ScoreValuesAfter4[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.Verifyradiobuttonenabled("deselect this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x6 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y6 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6, y6);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenBlueAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 7, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter6 = brz3dvp.CalciumScoringTableValues("PDA");
                Logger.Instance.InfoLog("PDA Score Value Before Step 6 : " + (ScoreValuesAfter6[0]));
                Logger.Instance.InfoLog("PDA Volume Value After Step 6 : " + (ScoreValuesAfter6[1]));
                if (GreenBlueAfter6 < GreenBlueAfter4 && ((ScoreValuesAfter6[0]) == 0.00) && ((ScoreValuesAfter6[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                IList<Double> ScoreValuesBefore7 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 255, 255, 255, 2);
                if (WhiteRegion8 > 0)
                {
                    int GreenBlueBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 82, 0, 255, 255, 2);
                    int[] x8 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y8 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 83, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter7 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 7 : " + (ScoreValuesBefore7[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 7 : " + (ScoreValuesBefore7[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 7 : " + (ScoreValuesAfter7[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 7 : " + (ScoreValuesAfter7[1]));
                    if (GreenBlueAfter8 != GreenBlueBefore8 && ((ScoreValuesBefore7[0]) < (ScoreValuesAfter7[0])) && ((ScoreValuesBefore7[1]) < (ScoreValuesAfter7[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<Double> ScoreValuesBefore08 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore08 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter08 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter08 != LocValBefore08)
                {
                    int GreenBlueBefore08 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 0, 255, 255, 2);
                    int[] x8 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y8 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter08 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter08 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 8 : " + (ScoreValuesBefore08[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 8 : " + (ScoreValuesBefore08[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 8 : " + (ScoreValuesAfter08[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 8 : " + (ScoreValuesAfter08[1]));
                    if (GreenBlueAfter08 != GreenBlueBefore08 && ((ScoreValuesBefore08[0]) < (ScoreValuesAfter08[0])) && ((ScoreValuesBefore08[1]) < (ScoreValuesAfter08[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
              
                //step 09
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("d").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.Verifyradiobuttonenabled("deselect this slice");
                if (Result)
                {
                    IList<Double> ScoreValuesBefore9 = brz3dvp.CalciumScoringTableValues("PDA");
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                    int GreenBlueBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 0, 255, 255, 2);
                    int[] x9 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y9 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x9, y9);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter9 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 9 : " + (ScoreValuesBefore9[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 9 : " + (ScoreValuesBefore9[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 9 : " + (ScoreValuesAfter9[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 9 : " + (ScoreValuesAfter9[1]));
                    if (GreenBlueAfter10 != GreenBlueBefore10 && ((ScoreValuesBefore9[0]) > (ScoreValuesAfter9[0])) && ((ScoreValuesBefore9[1]) > (ScoreValuesAfter9[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter10 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter10 != LocValBefore10)
                {
                    int GreenBlueBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 111, 0, 255, 255, 2);
                    int[] x10 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y10 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(15);
                    IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 10 : " + (ScoreValuesAfter10[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                    if (GreenBlueAfter10 != GreenBlueBefore10 && ((ScoreValuesBefore10[0]) > (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) > (ScoreValuesAfter10[1])) && (ScoreValuesAfter10[0]) == 0.00 && (ScoreValuesAfter10[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();
               
                //step 11
                IList<Double> ScoreValuesBefore11 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegionbefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 121, 255, 255, 255, 2);
                int GreenBlueBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 122, 0, 255, 255, 2);
                int[] x11 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y11 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x11, y11);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenBlueAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 123, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter11 = brz3dvp.CalciumScoringTableValues("PDA");
                Logger.Instance.InfoLog("PDA Score Value Before 11 : " + (ScoreValuesBefore11[0]));
                Logger.Instance.InfoLog("PDA Volume Value Before 11 : " + (ScoreValuesBefore11[1]));
                Logger.Instance.InfoLog("PDA Score Value After 11 : " + (ScoreValuesAfter11[0]));
                Logger.Instance.InfoLog("PDA Volume Value After 11 : " + (ScoreValuesAfter11[1]));
                if (GreenBlueAfter11 != GreenBlueBefore11 && ((ScoreValuesBefore11[0]) < (ScoreValuesAfter11[0])) && ((ScoreValuesBefore11[1]) < (ScoreValuesAfter11[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.Verifyradiobuttonenabled("deselect all contiguous", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SHIFT);
                Keyboard.Instance.Enter("d");
                Keyboard.Instance.LeaveAllKeys();
                PageLoadWait.WaitForFrameLoad(5);
                int[] x13 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 105, CalciumScoreImage.Size.Width / 2 + 105, CalciumScoreImage.Size.Width / 2 };
                int[] y13 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x13, y13);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenBlueAfter13 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 141, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter13 = brz3dvp.CalciumScoringTableValues("PDA");
                Logger.Instance.InfoLog("PDA Score Value After Step 14 : " + (ScoreValuesAfter13[0]));
                Logger.Instance.InfoLog("PDA Volume Value After Step 14 : " + (ScoreValuesAfter13[1]));
                if (GreenBlueAfter13 < GreenBlueAfter11 && ((ScoreValuesAfter13[0]) == 0.00) && ((ScoreValuesAfter13[1]) == 0.00))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                IList<Double> ScoreValuesBefore14 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int WhiteRegion14 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 151, 255, 255, 255, 2);
                if (WhiteRegion14 > 0)
                {
                    int GreenBlueBefore14 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 152, 0, 255, 255, 2);
                    int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y14 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter14 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 153, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter14 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 14 : " + (ScoreValuesBefore14[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 14 : " + (ScoreValuesBefore14[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 14 : " + (ScoreValuesAfter14[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 14 : " + (ScoreValuesAfter14[1]));
                    if (GreenBlueAfter14 != GreenBlueBefore14 && ((ScoreValuesBefore14[0]) < (ScoreValuesAfter14[0])) && ((ScoreValuesBefore14[1]) < (ScoreValuesAfter14[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                IList<Double> ScoreValuesBefore15 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore15 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                String LocValAfter15 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter15 != LocValBefore15)
                {
                    int GreenBlueBefore15 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 0, 255, 255, 2);
                    int[] x15 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y15 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x15, y15);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter15 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter15 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 15 : " + (ScoreValuesBefore15[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 15 : " + (ScoreValuesBefore15[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 15 : " + (ScoreValuesAfter15[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 15 : " + (ScoreValuesAfter15[1]));
                    if (GreenBlueAfter15 != GreenBlueBefore15 && ((ScoreValuesBefore15[0]) < (ScoreValuesAfter15[0])) && ((ScoreValuesBefore15[1]) < (ScoreValuesAfter15[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("PDA");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect all contiguous");
                if (Result)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    int GreenBlueBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 171, 0, 255, 255, 2);
                    int[] x16 = { 10 + ((CalciumScoreImage.Size.Width / 4) * 3), CalciumScoreImage.Size.Width, 10 + ((CalciumScoreImage.Size.Width / 4) * 3), (CalciumScoreImage.Size.Width / 4) * 3 };
                    int[] y16 = { CalciumScoreImage.Size.Height - 10, CalciumScoreImage.Size.Height / 4 - 10, CalciumScoreImage.Size.Height / 4 - 30, CalciumScoreImage.Size.Height - 10 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 16 : " + (ScoreValuesBefore16[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 16 : " + (ScoreValuesBefore16[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 16 : " + (ScoreValuesAfter16[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 16 : " + (ScoreValuesAfter16[1]));
                    if (GreenBlueAfter16 != GreenBlueBefore16 && ((ScoreValuesBefore16[0]) > (ScoreValuesAfter16[0])) && ((ScoreValuesBefore16[1]) > (ScoreValuesAfter16[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                IList<Double> ScoreValuesBefore17 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                String LocValBefore17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                Result = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n", ScrollDirection: "down");
                String LocValAfter17 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocValAfter17 != LocValBefore17)
                {
                    int GreenBlueBefore17 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 181, 0, 255, 255, 2);
                    int[] x17 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    int[] y17 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x17, y17);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 182, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter17 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value Before 17 : " + (ScoreValuesBefore17[0]));
                    Logger.Instance.InfoLog("PDA Volume Value Before 17 : " + (ScoreValuesBefore17[1]));
                    Logger.Instance.InfoLog("PDA Score Value After 17 : " + (ScoreValuesAfter17[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 17 : " + (ScoreValuesAfter17[1]));
                    if (GreenBlueAfter17 != GreenBlueBefore17 && ((ScoreValuesBefore17[0]) > (ScoreValuesAfter17[0])) && ((ScoreValuesBefore17[1]) > (ScoreValuesAfter17[1])) && (ScoreValuesAfter17[0]) == 0.00 && (ScoreValuesAfter17[1]) == 0.00)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163267(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            bool TotalVolumeVerification, TotalScoreVerification;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                IList<Double> ScoreValuesBefore4 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                int RedColorBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 41, 255, 0, 0, 2);
                int[] x4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4, y4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 42, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter4 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter4 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 4 : " + (ScoreValuesBefore4[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 4 : " + (ScoreValuesBefore4[1]));
                Logger.Instance.InfoLog("LM Score Value After 4 : " + (ScoreValuesAfter4[0]));
                Logger.Instance.InfoLog("LM Volume Value After 4 : " + (ScoreValuesAfter4[1]));
                Logger.Instance.InfoLog("Total Score Value After 4 : " + (TotalValuesAfter4[0]));
                Logger.Instance.InfoLog("Total Volume Value After 4 : " + (TotalValuesAfter4[1]));
                Double TotalVolume4 = ScoreValuesAfter4[0];
                Double TotalScore4 = ScoreValuesAfter4[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter4[0]).Equals(Convert.ToInt32(TotalVolume4));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter4[1]).Equals(Convert.ToInt32(TotalScore4));
                if (RedColorAfter4 != RedColorBefore4 && ((ScoreValuesBefore4[0]) < (ScoreValuesAfter4[0])) && ((ScoreValuesBefore4[1]) < (ScoreValuesAfter4[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                IList<Double> ScoreValuesBefore5 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                IList<Double> ScoreValuesBefore6 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                int GreenColorBefore6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 61, 0, 255, 0, 2);
                int[] x6 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y6 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6, y6);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 62, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter6 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> TotalValuesAfter6 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("RCA Score Value Before 6 : " + (ScoreValuesBefore6[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 6 : " + (ScoreValuesBefore6[1]));
                Logger.Instance.InfoLog("RCA Score Value After 6 : " + (ScoreValuesAfter6[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 6 : " + (ScoreValuesAfter6[1]));
                Logger.Instance.InfoLog("Total Score Value After 6 : " + (TotalValuesAfter6[0]));
                Logger.Instance.InfoLog("Total Volume Value After 6 : " + (TotalValuesAfter6[1]));
                Double TotalVolume6 = ScoreValuesAfter4[0] + ScoreValuesAfter6[0];
                Double TotalScore6 = ScoreValuesAfter4[1] + ScoreValuesAfter6[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter6[0]).Equals(Convert.ToInt32(TotalVolume6));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter6[1]).Equals(Convert.ToInt32(TotalScore6));
                if (GreenColorAfter6 != GreenColorBefore6 && ((ScoreValuesBefore6[0]) < (ScoreValuesAfter6[0])) && ((ScoreValuesBefore6[1]) < (ScoreValuesAfter6[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<Double> ScoreValuesBefore8 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                int BlueColorBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 0, 0, 255, 2);
                int[] x8 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y8 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueColorAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 82, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter8 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> TotalValuesAfter8 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LAD Score Value Before 8 : " + (ScoreValuesBefore8[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 8 : " + (ScoreValuesBefore8[1]));
                Logger.Instance.InfoLog("LAD Score Value After 8 : " + (ScoreValuesAfter8[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 8 : " + (ScoreValuesAfter8[1]));
                Logger.Instance.InfoLog("Total Score Value After 8 : " + (TotalValuesAfter8[0]));
                Logger.Instance.InfoLog("Total Volume Value After 8 : " + (TotalValuesAfter8[1]));
                Double TotalVolume8 = TotalVolume6 + ScoreValuesAfter8[0];
                Double TotalScore8 = TotalScore6 + ScoreValuesAfter8[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter8[0]).Equals(Convert.ToInt32(TotalVolume8));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter8[1]).Equals(Convert.ToInt32(TotalScore8));
                if (BlueColorAfter8 != BlueColorBefore8 && ((ScoreValuesBefore8[0]) < (ScoreValuesAfter8[0])) && ((ScoreValuesBefore8[1]) < (ScoreValuesAfter8[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                int YellowColorBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 101, 255, 255, 0, 2);
                int[] x10 = { CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100};
                int[] y10 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowColorAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 102, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> TotalValuesAfter10 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("CX Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                Logger.Instance.InfoLog("CX Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                Logger.Instance.InfoLog("CX Score Value After 10 : " + (ScoreValuesAfter10[0]));
                Logger.Instance.InfoLog("CX Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                Logger.Instance.InfoLog("Total Score Value After 10 : " + (TotalValuesAfter10[0]));
                Logger.Instance.InfoLog("Total Volume Value After 10 : " + (TotalValuesAfter10[1]));
                Double TotalVolume10 = TotalVolume8 + ScoreValuesAfter10[0];
                Double TotalScore10 = TotalScore8 + ScoreValuesAfter10[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter10[0]).Equals(Convert.ToInt32(TotalVolume10));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter10[1]).Equals(Convert.ToInt32(TotalScore10));
                if (YellowColorBefore10 != YellowColorAfter10 && ((ScoreValuesBefore10[0]) < (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) < (ScoreValuesAfter10[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select this slice", "y"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                IList<Double> ScoreValuesBefore12 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                int GBColorBefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 121, 0, 255, 255, 2);
                int[] x12 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y12 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x12, y12);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 122, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter12 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter12 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("PDA Score Value Before 10 : " + (ScoreValuesBefore12[0]));
                Logger.Instance.InfoLog("PDA Volume Value Before 10 : " + (ScoreValuesBefore12[1]));
                Logger.Instance.InfoLog("PDA Score Value After 10 : " + (ScoreValuesAfter12[0]));
                Logger.Instance.InfoLog("PDA Volume Value After 10 : " + (ScoreValuesAfter12[1]));
                Logger.Instance.InfoLog("Total Score Value After 10 : " + (TotalValuesAfter12[0]));
                Logger.Instance.InfoLog("Total Volume Value After 10 : " + (TotalValuesAfter12[1]));
                Double TotalVolume12 = TotalVolume10 + ScoreValuesAfter12[0];
                Double TotalScore12 = TotalScore10 + ScoreValuesAfter12[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter12[0]).Equals(Convert.ToInt32(TotalVolume12));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter12[1]).Equals(Convert.ToInt32(TotalScore12));
                if (GBColorBefore12 != GBColorAfter12 && ((ScoreValuesBefore12[0]) < (ScoreValuesAfter12[0])) && ((ScoreValuesBefore12[1]) < (ScoreValuesAfter12[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                IList<Double> ScoreValuesBefore13 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesBefore13 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect this slice");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                int RedColorBefore13 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 141, 255, 0, 0, 2);
                int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y14 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter13 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 142, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter13 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter13 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 14 : " + (ScoreValuesBefore13[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 14 : " + (ScoreValuesBefore13[1]));
                Logger.Instance.InfoLog("LM Score Value After 14 : " + (ScoreValuesAfter13[0]));
                Logger.Instance.InfoLog("LM Volume Value After 14 : " + (ScoreValuesAfter13[1]));
                Logger.Instance.InfoLog("Total Score Value After 14 : " + (TotalValuesAfter13[0]));
                Logger.Instance.InfoLog("Total Volume Value After 14 : " + (TotalValuesAfter13[1]));
                Logger.Instance.InfoLog("Total Score Value Before 14 : " + (TotalValuesBefore13[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 14 : " + (TotalValuesBefore13[1]));
                Double diffcalvolume13 = TotalValuesBefore13[0] - TotalValuesAfter13[0];
                Double diffcalscore13 = TotalValuesBefore13[1] - TotalValuesAfter13[1];
                Double diffvolume13 = ScoreValuesBefore13[0] - ScoreValuesAfter13[0];
                Double diffscore13 = ScoreValuesBefore13[1] - ScoreValuesAfter13[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume13).Equals(Convert.ToInt32(diffvolume13));
                TotalScoreVerification = Convert.ToInt32(diffcalscore13).Equals(Convert.ToInt32(diffscore13));
                if (RedColorAfter13 != RedColorBefore13 && ((ScoreValuesBefore13[0]) > (ScoreValuesAfter13[0])) && ((ScoreValuesBefore13[1]) > (ScoreValuesAfter13[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesBefore16 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect this slice");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                int GBColorBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 0, 255, 255, 2);
                int[] x16 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y16 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter16 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 16 : " + (ScoreValuesBefore16[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 16 : " + (ScoreValuesBefore16[1]));
                Logger.Instance.InfoLog("LM Score Value After 16 : " + (ScoreValuesAfter16[0]));
                Logger.Instance.InfoLog("LM Volume Value After 16 : " + (ScoreValuesAfter16[1]));
                Logger.Instance.InfoLog("Total Score Value After 16 : " + (TotalValuesAfter16[0]));
                Logger.Instance.InfoLog("Total Volume Value After 16 : " + (TotalValuesAfter16[1]));
                Logger.Instance.InfoLog("Total Score Value Before 16 : " + (TotalValuesBefore16[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 16 : " + (TotalValuesBefore16[1]));
                Double diffcalvolume16 = TotalValuesBefore16[0] - TotalValuesAfter16[0];
                Double diffcalscore16 = TotalValuesBefore16[1] - TotalValuesAfter16[1];
                Double diffvolume16 = ScoreValuesBefore16[0] - ScoreValuesAfter16[0];
                Double diffscore16 = ScoreValuesBefore16[1] - ScoreValuesAfter16[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume16).Equals(Convert.ToInt32(diffvolume16));
                TotalScoreVerification = Convert.ToInt32(diffcalscore16).Equals(Convert.ToInt32(diffscore16));
                if (GBColorBefore16 != GBColorAfter16 && ((ScoreValuesBefore16[0]) > (ScoreValuesAfter16[0])) && ((ScoreValuesBefore16[1]) > (ScoreValuesAfter16[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                IList<Double> ScoreValuesBefore17 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> TotalValuesBefore17 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect this slice");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                int[] x18 = { CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100};
                int[] y18 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x18, y18);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowColorAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter17 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> TotalValuesAfter17 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 18 : " + (ScoreValuesBefore17[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 18 : " + (ScoreValuesBefore17[1]));
                Logger.Instance.InfoLog("LM Score Value After 18 : " + (ScoreValuesAfter17[0]));
                Logger.Instance.InfoLog("LM Volume Value After 18 : " + (ScoreValuesAfter17[1]));
                Logger.Instance.InfoLog("Total Score Value After 18 : " + (TotalValuesAfter17[0]));
                Logger.Instance.InfoLog("Total Volume Value After 18 : " + (TotalValuesAfter17[1]));
                Logger.Instance.InfoLog("Total Score Value Before 18 : " + (TotalValuesBefore17[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 18 : " + (TotalValuesBefore17[1]));
                Double diffcalvolume17 = TotalValuesBefore17[0] - TotalValuesAfter17[0];
                Double diffcalscore17 = TotalValuesBefore17[1] - TotalValuesAfter17[1];
                Double diffvolume17 = ScoreValuesBefore17[0] - ScoreValuesAfter17[0];
                Double diffscore17 = ScoreValuesBefore17[1] - ScoreValuesAfter17[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume17).Equals(Convert.ToInt32(diffvolume17));
                TotalScoreVerification = Convert.ToInt32(diffcalscore17).Equals(Convert.ToInt32(diffscore17));
                if (YellowColorAfter17 != YellowColorAfter10 && ((ScoreValuesBefore17[0]) > (ScoreValuesAfter17[0])) && ((ScoreValuesBefore17[1]) > (ScoreValuesAfter17[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                IList<Double> ScoreValuesBefore20 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> TotalValuesBefore20 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect this slice");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                int BlueColorBefore20 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                int[] x20 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200};
                int[] y20 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x20, y20);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueColorAfter20 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 202, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter20 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> TotalValuesAfter20 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LAD Score Value Before 20 : " + (ScoreValuesBefore20[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 20 : " + (ScoreValuesBefore20[1]));
                Logger.Instance.InfoLog("LAD Score Value After 20 : " + (ScoreValuesAfter20[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 20 : " + (ScoreValuesAfter20[1]));
                Logger.Instance.InfoLog("Total Score Value After 20 : " + (TotalValuesAfter20[0]));
                Logger.Instance.InfoLog("Total Volume Value After 20 : " + (TotalValuesAfter20[1]));
                Double diffcalvolume20 = TotalValuesBefore20[0] - TotalValuesAfter20[0];
                Double diffcalscore20 = TotalValuesBefore20[1] - TotalValuesAfter20[1];
                Double diffvolume20 = ScoreValuesBefore20[0] - ScoreValuesAfter20[0];
                Double diffscore20 = ScoreValuesBefore20[1] - ScoreValuesAfter20[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume20).Equals(Convert.ToInt32(diffvolume20));
                TotalScoreVerification = Convert.ToInt32(diffcalscore20).Equals(Convert.ToInt32(diffscore20));
                if (BlueColorAfter20 != BlueColorBefore20 && ((ScoreValuesBefore20[0]) > (ScoreValuesAfter20[0])) && ((ScoreValuesBefore20[1]) > (ScoreValuesAfter20[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 21
                IList<Double> ScoreValuesBefore22 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> TotalValuesBefore22 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect this slice");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 22
                int GreenColorBefore22 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 221, 0, 255, 0, 2);
                int[] x22 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y22 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x22, y22);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter22 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 222, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter22 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> TotalValuesAfter22 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("RCA Score Value Before 22 : " + (ScoreValuesBefore22[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 22 : " + (ScoreValuesBefore22[1]));
                Logger.Instance.InfoLog("RCA Score Value After 22 : " + (ScoreValuesAfter22[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 22 : " + (ScoreValuesAfter22[1]));
                Logger.Instance.InfoLog("Total Score Value After 22 : " + (TotalValuesAfter22[0]));
                Logger.Instance.InfoLog("Total Volume Value After 22 : " + (TotalValuesAfter22[1]));
                Double diffcalvolume22 = TotalValuesBefore22[0] - TotalValuesAfter22[0];
                Double diffcalscore22 = TotalValuesBefore22[1] - TotalValuesAfter22[1];
                Double diffvolume22 = ScoreValuesBefore22[0] - ScoreValuesAfter22[0];
                Double diffscore22 = ScoreValuesBefore22[1] - ScoreValuesAfter22[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume22).Equals(Convert.ToInt32(diffvolume22));
                TotalScoreVerification = Convert.ToInt32(diffcalscore22).Equals(Convert.ToInt32(diffscore22));
                if (GreenColorAfter22 != GreenColorBefore22 && ((ScoreValuesBefore22[0]) > (ScoreValuesAfter22[0])) && ((ScoreValuesBefore22[1]) > (ScoreValuesAfter22[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


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

        public TestCaseResult Test_163277(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                Result = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                String ControlName = BluRingZ3DViewerPage.CalciumScoring;
                IWebElement Navigation = brz3dvp.controlelement(ControlName);
                String ControlViewContainer = Locators.CssSelector.NgStarInserted + " " + Locators.CssSelector.CompositeViewer + " " + Locators.CssSelector.ViewContainer;
                new Actions(Driver).MoveToElement(Navigation, Navigation.Size.Width / 4, Navigation.Size.Height / 4).ContextClick().Build().Perform();
                try
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(ControlViewContainer + ":nth-of-type(" + brz3dvp.nthtype(ControlName) + ") " + Locators.CssSelector.ToolBoxComponent)));
                }
                catch (Exception exp)
                {
                    Logger.Instance.ErrorLog("Error while performing right click over Navigation control due to Exception" + exp.ToString());
                }
                PageLoadWait.WaitForFrameLoad(10);
                String toolbuttons = ControlViewContainer + ":nth-of-type(" + brz3dvp.nthtype(BluRingZ3DViewerPage.Navigationone) + ") " + Locators.CssSelector.ToolWrapper;
                IList<IWebElement> tools = Driver.FindElements(By.CssSelector(toolbuttons));
                IList<String> toolnames = new List<String>();
                int ctr = 0;
                foreach (IWebElement tool in tools)
                {
                    if (tool.GetAttribute("outerHTML").Contains("disabled"))
                    {
                        String tooltitle = tool.GetAttribute("title");
                        Logger.Instance.InfoLog("Title name : " + tooltitle);
                        toolnames.Add(tooltitle);
                    }
                }
                foreach (String toolname in toolnames)
                {
                    if (toolname.Equals("Window Level") || toolname.Equals("Scroll Tool") || toolname.Equals("Calcium Scoring") || toolname.Equals("Download Image") || toolname.Equals("Reset"))
                    {
                        break;
                    }
                    else
                        ctr++;
                }
                if (ctr == 13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163275(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while selecting window level tool");
                    throw new Exception("Failed while selecting window level tool");
                }

                //step 04
                String annotationvaluebefore = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring);
                IWebElement calciumscore = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                PageLoadWait.WaitForFrameLoad(5);
                int[] x4_3 = { calciumscore.Size.Width / 2 , calciumscore.Size.Width / 2 };
                int[] y4_3 = { calciumscore.Size.Height / 4, (calciumscore.Size.Height / 4) * 3 };
                brz3dvp.drawselectedtool(calciumscore, x4_3, y4_3);
                PageLoadWait.WaitForFrameLoad(10);
                String annotationvalueafter = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (annotationvaluebefore != annotationvalueafter)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163280(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                String[] presets = { "Abdomen", "Bone", "Brain", "Bone (body)", "Bronchial", "Liver", "Lung", "Mediastinum", "P Fossa" };
                int counter = 0;
                for (int i = 0; i < presets.Length; i++)
                {
                    String wlvaluebefore = brz3dvp.GetTopLeftAnnotationValue("Calcium Scoring");
                    PageLoadWait.WaitForFrameLoad(5);
                    Result = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.CalciumScoring, presets[i], "preset");
                    if (Result)
                    {
                        PageLoadWait.WaitForFrameLoad(3);
                        String wlvalueafter = brz3dvp.GetTopLeftAnnotationValue("Calcium Scoring");
                        if (wlvalueafter.Equals(wlvaluebefore))
                            break;
                        else
                            counter++;
                    }
                    else
                        break;
                }
                if (counter == presets.Length)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163282(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            IWebElement Warning, message;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    Logger.Instance.ErrorLog("Study with thickness more than 3 mm launched without any error in Calcium Scoring Layout");
                    throw new Exception("Study with thickness more than 3 mm launched without any error in Calcium Scoring Layout");
                }
                else
                {
                    Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                    if (Warning.Displayed)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        {
                            Logger.Instance.ErrorLog("Warning message not displayed");
                            throw new Exception("Warning message not displayed");
                        }
                    }
                }

                //step 03
                IWebElement calciumscore = brz3dvp.controlelement("Calcium Scoring");
                new Actions(Driver).MoveToElement(calciumscore, (calciumscore.Size.Width / 4) * 3, calciumscore.Size.Height / 4).ContextClick().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                try
                {
                    IList<IWebElement> liwe = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(1) " + brz3dvp.ControlViewContainer + ":nth-of-type(" + brz3dvp.nthtype(BluRingZ3DViewerPage.CalciumScoring) + ") " + Locators.CssSelector.GridTile));
                    if (liwe.Count == 0)
                    {
                        Logger.Instance.InfoLog("The count of grid tile is : " + liwe.Count);
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception exp)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                ////step 04
                //result.steps[++ExecutedSteps].status = "Fail";
                //result.steps[ExecutedSteps].comments = "Related to Jira ICA-17486 ";

                //step 05
                Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                message = Warning.FindElement(By.CssSelector("h1"));
                message.Click();
                PageLoadWait.WaitForFrameLoad(10);
                Result = brz3dvp.checkerrormsg("y");
                if (Result)
                {
                    try
                    {
                        Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                        if (Warning.Displayed)
                            result.steps[++ExecutedSteps].StepFail();
                        else
                            result.steps[++ExecutedSteps].StepPass();
                    }
                    catch (Exception exp)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                Result = brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                Result = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                PageLoadWait.WaitForFrameLoad(5);
                if (Result)
                {
                    try
                    {
                        Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                        if (Warning.Displayed)
                            result.steps[++ExecutedSteps].StepFail();
                        else
                            result.steps[++ExecutedSteps].StepPass();
                    }
                    catch (Exception exp1)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163269(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            
            try
            {
                //step 01
                  login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                  bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                  if (Result)
                      result.steps[++ExecutedSteps].StepPass("Series loaded successfully");
                  else
                  {
                      Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                      throw new Exception("Failed to open study in Calcium Scoring Layout");
                  }

                  //step 02
                  IList<Double> ScoreValuesBefore2 = brz3dvp.CalciumScoringTableValues("LM");
                  IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                  brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                  PageLoadWait.WaitForFrameLoad(5);
                  int RedBefore2 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 21, 255, 0, 0, 2);
                  int[] x2 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                  int[] y2 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                  PageLoadWait.WaitForFrameLoad(5);
                  brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                  wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                  int RedAfter2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 22, 255, 0, 0, 2);
                  brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                  PageLoadWait.WaitForFrameLoad(5);
                  IList<Double> ScoreValuesAfter2 = brz3dvp.CalciumScoringTableValues("LM");
                  Logger.Instance.InfoLog("LM Score Value Before 2 select this slice : " + (ScoreValuesBefore2[0]));
                  Logger.Instance.InfoLog("LM Volume Value Before 2 select this slice : " + (ScoreValuesBefore2[1]));
                  Logger.Instance.InfoLog("LM Score Value After 2 select this slice : " + (ScoreValuesAfter2[0]));
                  Logger.Instance.InfoLog("LM Volume Value After 2 select this slice : " + (ScoreValuesAfter2[1]));
                  if (RedAfter2 != RedBefore2 && ((ScoreValuesBefore2[0]) < (ScoreValuesAfter2[0])) && ((ScoreValuesBefore2[1]) < (ScoreValuesAfter2[1])))
                  {
                      brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Select all contiguous");
                      PageLoadWait.WaitForFrameLoad(5);
                      brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                      PageLoadWait.WaitForFrameLoad(3);
                      int RedBefore2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 23, 255, 0, 0, 2);
                      PageLoadWait.WaitForFrameLoad(3);
                      CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                      PageLoadWait.WaitForFrameLoad(5);
                      brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                      wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                      int RedAfter2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 24, 255, 0, 0, 2);
                      brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                      PageLoadWait.WaitForFrameLoad(5);
                      IList<Double> ScoreValuesAfter2_1 = brz3dvp.CalciumScoringTableValues("LM");
                      Logger.Instance.InfoLog("LM Score Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[0]));
                      Logger.Instance.InfoLog("LM Volume Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[1]));
                      if (RedBefore2_1 != RedAfter2_1 && ((ScoreValuesAfter2[0]) < (ScoreValuesAfter2_1[0])) && ((ScoreValuesAfter2[1]) < (ScoreValuesAfter2_1[1])))
                          result.steps[++ExecutedSteps].StepPass();
                      else
                          result.steps[++ExecutedSteps].StepFail();
                  }
                  else
                      result.steps[++ExecutedSteps].StepFail();

                  //step 03
                  IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("LM");
                  CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                  brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect this slice");
                  PageLoadWait.WaitForFrameLoad(5);
                  brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, ScrollDirection: "down", scrolllevel: 2, Thickness: "n");
                  PageLoadWait.WaitForFrameLoad(5);
                  int RedBefore3 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 31, 255, 0, 0, 2);
                  int[] x3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                  int[] y3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                  PageLoadWait.WaitForFrameLoad(5);
                  brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                  wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                  int RedAfter3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 32, 255, 0, 0, 2);
                  brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                  PageLoadWait.WaitForFrameLoad(5);
                  IList<Double> ScoreValuesAfter3 = brz3dvp.CalciumScoringTableValues("LM");
                  Logger.Instance.InfoLog("LM Score Value Before 3 deselect this slice : " + (ScoreValuesBefore3[0]));
                  Logger.Instance.InfoLog("LM Volume Value Before 3 deselect this slice : " + (ScoreValuesBefore3[1]));
                  Logger.Instance.InfoLog("LM Score Value After 3 deselect this slice : " + (ScoreValuesAfter3[0]));
                  Logger.Instance.InfoLog("LM Volume Value After 3 deselect this slice : " + (ScoreValuesAfter3[1]));
                  if (RedAfter3 != RedBefore3 && ((ScoreValuesBefore3[0]) > (ScoreValuesAfter3[0])) && ((ScoreValuesBefore3[1]) > (ScoreValuesAfter3[1])))
                  {
                      brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect all contiguous");
                      PageLoadWait.WaitForFrameLoad(5);
                      brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                      PageLoadWait.WaitForFrameLoad(3);
                      int RedBefore3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 33, 255, 0, 0, 2);
                      PageLoadWait.WaitForFrameLoad(3);
                      CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                      PageLoadWait.WaitForFrameLoad(5);
                      brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                      wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                      int RedAfter3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 34, 255, 0, 0, 2);
                      brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                      PageLoadWait.WaitForFrameLoad(5);
                      IList<Double> ScoreValuesAfter3_1 = brz3dvp.CalciumScoringTableValues("LM");
                      Logger.Instance.InfoLog("LM Score Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[0]));
                      Logger.Instance.InfoLog("LM Volume Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[1]));
                      if (RedBefore3_1 != RedAfter3_1 && ((ScoreValuesAfter3_1[0]) == 0.00) && ((ScoreValuesAfter3_1[1]) == 0.00))
                          result.steps[++ExecutedSteps].StepPass();
                      else
                          result.steps[++ExecutedSteps].StepFail();
                  }
                  else
                      result.steps[++ExecutedSteps].StepFail();
                      
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
                //brz3dvp.CloseViewer();
                //login.Logout();
            }
        }

        public TestCaseResult Test_163270(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                IList<Double> ScoreValuesBefore2 = brz3dvp.CalciumScoringTableValues("RCA");
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                PageLoadWait.WaitForFrameLoad(5);
                int GreenBefore2 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 21, 0, 255, 0, 2);
                int[] x2 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y2 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenAfter2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 22, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter2 = brz3dvp.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("RCA Score Value Before 2 select this slice : " + (ScoreValuesBefore2[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 2 select this slice : " + (ScoreValuesBefore2[1]));
                Logger.Instance.InfoLog("RCA Score Value After 2 select this slice : " + (ScoreValuesAfter2[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 2 select this slice : " + (ScoreValuesAfter2[1]));
                if (GreenAfter2 != GreenBefore2 && ((ScoreValuesBefore2[0]) < (ScoreValuesAfter2[0])) && ((ScoreValuesBefore2[1]) < (ScoreValuesAfter2[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int GreenBefore2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 23, 0, 255, 0, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenAfter2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 24, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter2_1 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[1]));
                    if (GreenBefore2_1 != GreenAfter2_1 && ((ScoreValuesAfter2[0]) < (ScoreValuesAfter2_1[0])) && ((ScoreValuesAfter2[1]) < (ScoreValuesAfter2_1[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("RCA");
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, ScrollDirection: "down", scrolllevel: 2, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(5);
                int GreenBefore3 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 31, 0, 255, 0, 2);
                int[] x3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenAfter3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 32, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter3 = brz3dvp.CalciumScoringTableValues("RCA");
                Logger.Instance.InfoLog("RCA Score Value Before 3 deselect this slice : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 3 deselect this slice : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("RCA Score Value After 3 deselect this slice : " + (ScoreValuesAfter3[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 3 deselect this slice : " + (ScoreValuesAfter3[1]));
                if (GreenAfter3 != GreenBefore3 && ((ScoreValuesBefore3[0]) > (ScoreValuesAfter3[0])) && ((ScoreValuesBefore3[1]) > (ScoreValuesAfter3[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int GreenBefore3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 33, 0, 255, 0, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenAfter3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 34, 0, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter3_1 = brz3dvp.CalciumScoringTableValues("RCA");
                    Logger.Instance.InfoLog("RCA Score Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[0]));
                    Logger.Instance.InfoLog("RCA Volume Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[1]));
                    if (GreenBefore3_1 != GreenAfter3_1 && ((ScoreValuesAfter3_1[0]) == 0.00) && ((ScoreValuesAfter3_1[1]) == 0.00))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163271(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                IList<Double> ScoreValuesBefore2 = brz3dvp.CalciumScoringTableValues("LAD");
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                PageLoadWait.WaitForFrameLoad(5);
                int BlueBefore2 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 21, 0, 0, 255, 2);
                int[] x2 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y2 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueAfter2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 22, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter2 = brz3dvp.CalciumScoringTableValues("LAD");
                Logger.Instance.InfoLog("LAD Score Value Before 2 select this slice : " + (ScoreValuesBefore2[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 2 select this slice : " + (ScoreValuesBefore2[1]));
                Logger.Instance.InfoLog("LAD Score Value After 2 select this slice : " + (ScoreValuesAfter2[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 2 select this slice : " + (ScoreValuesAfter2[1]));
                if (BlueAfter2 != BlueBefore2 && ((ScoreValuesBefore2[0]) < (ScoreValuesAfter2[0])) && ((ScoreValuesBefore2[1]) < (ScoreValuesAfter2[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int BlueBefore2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 23, 0, 0, 255, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 24, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter2_1 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[1]));
                    if (BlueBefore2_1 != BlueAfter2_1 && ((ScoreValuesAfter2[0]) < (ScoreValuesAfter2_1[0])) && ((ScoreValuesAfter2[1]) < (ScoreValuesAfter2_1[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("LAD");
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, ScrollDirection: "down", scrolllevel: 2, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(5);
                int BlueBefore3 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 31, 0, 0, 255, 2);
                int[] x3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueAfter3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 32, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter3 = brz3dvp.CalciumScoringTableValues("LAD");
                Logger.Instance.InfoLog("LAD Score Value Before 3 deselect this slice : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 3 deselect this slice : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("LAD Score Value After 3 deselect this slice : " + (ScoreValuesAfter3[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 3 deselect this slice : " + (ScoreValuesAfter3[1]));
                if (BlueAfter3 != BlueBefore3 && ((ScoreValuesBefore3[0]) > (ScoreValuesAfter3[0])) && ((ScoreValuesBefore3[1]) > (ScoreValuesAfter3[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int BlueBefore3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 33, 0, 0, 255, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int BlueAfter3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 34, 0, 0, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter3_1 = brz3dvp.CalciumScoringTableValues("LAD");
                    Logger.Instance.InfoLog("LAD Score Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[0]));
                    Logger.Instance.InfoLog("LAD Volume Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[1]));
                    if (BlueBefore3_1 != BlueAfter3_1 && ((ScoreValuesAfter3_1[0]) == 0.00) && ((ScoreValuesAfter3_1[1]) == 0.00))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163272(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                IList<Double> ScoreValuesBefore2 = brz3dvp.CalciumScoringTableValues("PDA");
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                PageLoadWait.WaitForFrameLoad(5);
                int GreenBlueBefore2 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 21, 0, 255, 255, 2);
                int[] x2 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y2 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenBlueAfter2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 22, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter2 = brz3dvp.CalciumScoringTableValues("PDA");
                Logger.Instance.InfoLog("PDA Score Value Before 2 select this slice : " + (ScoreValuesBefore2[0]));
                Logger.Instance.InfoLog("PDA Volume Value Before 2 select this slice : " + (ScoreValuesBefore2[1]));
                Logger.Instance.InfoLog("PDA Score Value After 2 select this slice : " + (ScoreValuesAfter2[0]));
                Logger.Instance.InfoLog("PDA Volume Value After 2 select this slice : " + (ScoreValuesAfter2[1]));
                if (GreenBlueAfter2 != GreenBlueBefore2 && ((ScoreValuesBefore2[0]) < (ScoreValuesAfter2[0])) && ((ScoreValuesBefore2[1]) < (ScoreValuesAfter2[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Select all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int GreenBlueBefore2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 23, 0, 255, 255, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 24, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter2_1 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[1]));
                    if (GreenBlueBefore2_1 != GreenBlueAfter2_1 && ((ScoreValuesAfter2[0]) < (ScoreValuesAfter2_1[0])) && ((ScoreValuesAfter2[1]) < (ScoreValuesAfter2_1[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("PDA");
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, ScrollDirection: "down", scrolllevel: 2, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(5);
                int GreenBlueBefore3 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 31, 0, 255, 255, 2);
                int[] x3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenBlueAfter3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 32, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter3 = brz3dvp.CalciumScoringTableValues("PDA");
                Logger.Instance.InfoLog("PDA Score Value Before 3 deselect this slice : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("PDA Volume Value Before 3 deselect this slice : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("PDA Score Value After 3 deselect this slice : " + (ScoreValuesAfter3[0]));
                Logger.Instance.InfoLog("PDA Volume Value After 3 deselect this slice : " + (ScoreValuesAfter3[1]));
                if (GreenBlueAfter3 != GreenBlueBefore3 && ((ScoreValuesBefore3[0]) > (ScoreValuesAfter3[0])) && ((ScoreValuesBefore3[1]) > (ScoreValuesAfter3[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int GreenBlueBefore3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 33, 0, 255, 255, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int GreenBlueAfter3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 34, 0, 255, 255, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter3_1 = brz3dvp.CalciumScoringTableValues("PDA");
                    Logger.Instance.InfoLog("PDA Score Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[0]));
                    Logger.Instance.InfoLog("PDA Volume Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[1]));
                    if (GreenBlueBefore3_1 != GreenBlueAfter3_1 && ((ScoreValuesAfter3_1[0]) == 0.00) && ((ScoreValuesAfter3_1[1]) == 0.00))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163273(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                IList<Double> ScoreValuesBefore2 = brz3dvp.CalciumScoringTableValues("CX");
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                PageLoadWait.WaitForFrameLoad(5);
                int YellowBefore2 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 21, 255, 255, 0, 2);
                int[] x2 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y2 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowAfter2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 22, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter2 = brz3dvp.CalciumScoringTableValues("CX");
                Logger.Instance.InfoLog("CX Score Value Before 2 select this slice : " + (ScoreValuesBefore2[0]));
                Logger.Instance.InfoLog("CX Volume Value Before 2 select this slice : " + (ScoreValuesBefore2[1]));
                Logger.Instance.InfoLog("CX Score Value After 2 select this slice : " + (ScoreValuesAfter2[0]));
                Logger.Instance.InfoLog("CX Volume Value After 2 select this slice : " + (ScoreValuesAfter2[1]));
                if (YellowAfter2 != YellowBefore2 && ((ScoreValuesBefore2[0]) < (ScoreValuesAfter2[0])) && ((ScoreValuesBefore2[1]) < (ScoreValuesAfter2[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int YellowBefore2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 23, 255, 255, 0, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    x2 = new int[]{ CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    y2 = new int[]{ CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter2_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 24, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter2_1 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 2 select all contiguous : " + (ScoreValuesAfter2_1[1]));
                    if (YellowBefore2_1 != YellowAfter2_1 && ((ScoreValuesAfter2[0]) < (ScoreValuesAfter2_1[0])) && ((ScoreValuesAfter2[1]) < (ScoreValuesAfter2_1[1])))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("CX");
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, ScrollDirection: "down", scrolllevel: 2, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(5);
                int YellowBefore3 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 31, 255, 255, 0, 2);
                int[] x3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowAfter3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 32, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter3 = brz3dvp.CalciumScoringTableValues("CX");
                Logger.Instance.InfoLog("CX Score Value Before 3 deselect this slice : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("CX Volume Value Before 3 deselect this slice : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("CX Score Value After 3 deselect this slice : " + (ScoreValuesAfter3[0]));
                Logger.Instance.InfoLog("CX Volume Value After 3 deselect this slice : " + (ScoreValuesAfter3[1]));
                if (YellowAfter3 != YellowBefore3 && ((ScoreValuesBefore3[0]) > (ScoreValuesAfter3[0])) && ((ScoreValuesBefore3[1]) > (ScoreValuesAfter3[1])))
                {
                    brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect all contiguous");
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrolllevel: 2, Thickness: "n");
                    PageLoadWait.WaitForFrameLoad(3);
                    int YellowBefore3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 33, 255, 255, 0, 2);
                    PageLoadWait.WaitForFrameLoad(3);
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    x3 = new int[] { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                    y3 = new int[] { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int YellowAfter3_1 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 34, 255, 255, 0, 2);
                    brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<Double> ScoreValuesAfter3_1 = brz3dvp.CalciumScoringTableValues("CX");
                    Logger.Instance.InfoLog("CX Score Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[0]));
                    Logger.Instance.InfoLog("CX Volume Value After 2 deselect all contiguous : " + (ScoreValuesAfter3_1[1]));
                    if (YellowBefore3_1 != YellowAfter3_1 && ((ScoreValuesAfter3_1[0]) == 0.00) && ((ScoreValuesAfter3_1[1]) == 0.00))
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163276(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02 & 03
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 04
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                String LocaValBefore_5 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                IWebElement CalciumScore = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                for (int i = 0; i < 5; i++)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    int[] x5 = { CalciumScore.Size.Width / 2 , CalciumScore.Size.Width / 2 };
                    int[] y5 = { ((CalciumScore.Size.Height / 4) * 3) + 20, (CalciumScore.Size.Height / 4) - 20 };
                    brz3dvp.drawselectedtool(CalciumScore, x5, y5);
                    PageLoadWait.WaitForFrameLoad(5);
                }
                String LocaValAfter_5 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocaValAfter_5 != LocaValBefore_5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                String LocaValAfter_6 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (LocaValAfter_6 == LocaValBefore_5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163278(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] descriptions = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription")).Split('|');
            string[] requirements = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements")).Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, descriptions[0], layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                try
                {
                    IWebElement Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                    if (Warning.Displayed)
                        result.steps[++ExecutedSteps].StepFail();
                    else
                        result.steps[++ExecutedSteps].StepPass();
                }
                catch (NoSuchElementException exp)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 04
                brz3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05 & 06
                Result = brz3dvp.searchandopenstudyin3D(requirements[1], descriptions[1], layout: BluRingZ3DViewerPage.CalciumScoring, field: requirements[0], thumbimgoptional: descriptions[2]);
                if (Result)
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 07
                try
                {
                    IWebElement Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                    if (Warning.Displayed)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (NoSuchElementException exp)
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                finally
                {
                    brz3dvp.checkerrormsg("y");
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

        public TestCaseResult Test_163285(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string[] Patientid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('|');
            string[] thumbnail = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription")).Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid[0], thumbnail[0], layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 02
                IList<Double> ScoreValuesBefore2 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedBefore2 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 21, 255, 0, 0, 2);
                PageLoadWait.WaitForFrameLoad(5);
                int[] x2 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y2 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x2, y2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedAfter2 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 22, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter2 = brz3dvp.CalciumScoringTableValues("LM");
                if (RedAfter2 > RedBefore2 && ScoreValuesBefore2 != ScoreValuesAfter2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                String[] studydetails = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CalciumScoring).Split(' ');
                String[] DOB = studydetails[5].Split('-');
                String[] StudyDatearray = studydetails[7].Split('-');
                int YOB = Convert.ToInt32(DOB[DOB.Length - 1]);
                int Studydateyear = Convert.ToInt32(StudyDatearray[StudyDatearray.Length - 1]);
                String agefromtable = Convert.ToString(Studydateyear - YOB);
                String percentile = brz3dvp.GetPercentilevalues();
                String patientage = brz3dvp.GetPercentilevalues("age");
                Logger.Instance.InfoLog("Percentile value is : " + percentile);
                Logger.Instance.InfoLog("Percentile value is : " + patientage);
                if (agefromtable == patientage && percentile != null && percentile != "")
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int[] x4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 400, CalciumScoreImage.Size.Height / 2 + 400, CalciumScoreImage.Size.Height / 2 + 200 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x4, y4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(2000);
                if (browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 4).Click().Release().Build().Perform();
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                String percentile_4 = brz3dvp.GetPercentilevalues();
                if (percentile_4 == "90+")
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                String filepath = Config.downloadpath + "\\" + testid + "_" + ExecutedSteps + "_51.txt";
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    File.WriteAllText(filepath, text);
                }
                string[] readText = File.ReadAllLines(filepath);
                try
                {
                    int counter = 0;
                    for (int i = 0; i < readText.Length; i++)
                    {
                        if (readText[i].Contains(percentile_4) || readText[i].Contains(patientage))
                        {
                            counter++;
                        }
                    }
                    if (counter == 2)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception exp)
                {
                    Logger.Instance.ErrorLog("No content in the text file");
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 06
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedBefore6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 61, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                int RedAfter6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 62, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                String percentile_6 = brz3dvp.GetPercentilevalues();
                if (percentile_6 == "0" && RedAfter6 < RedBefore6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                brz3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                Result = brz3dvp.searchandopenstudyin3D(Patientid[1], thumbnail[1], layout: BluRingZ3DViewerPage.CalciumScoring, thumbimgoptional: thumbnail[2]);
                if (!Result)
                {
                    Result = brz3dvp.checkerrormsg("y");
                    if (Result)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                    {
                        Logger.Instance.ErrorLog("Study with thickness not equal to 3 mm opened without any error");
                        throw new Exception("Study with thickness not equal to 3 mm opened without any error");
                    }

                }
                else
                {
                    Logger.Instance.ErrorLog("Study with thickness not equal to 3 mm opened without any error");
                    throw new Exception("Study with thickness not equal to 3 mm opened without any error");
                }

                //step 09
                IList<Double> ScoreValuesBefore9 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedBefore9 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 255, 0, 0, 2);
                PageLoadWait.WaitForFrameLoad(5);
                int[] x9 = { CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 + 250 };
                int[] y9 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x9, y9);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(2000);
                if (browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 4).Click().Release().Build().Perform();
                Thread.Sleep(3000);
                int RedAfter9 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 92, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter9 = brz3dvp.CalciumScoringTableValues("LM");
                if (RedAfter9 > RedBefore9 && ScoreValuesBefore9 != ScoreValuesAfter9)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                String[] studydetails10 = brz3dvp.GetTopRightAnnotationValue(BluRingZ3DViewerPage.CalciumScoring).Split(' ');
                String[] DOB10 = studydetails10[2].Split('-');
                String[] StudyDatearray10 = studydetails10[5].Split('-');
                int YOB10 = Convert.ToInt32(DOB10[DOB10.Length - 1]);
                int Studydateyear10 = Convert.ToInt32(StudyDatearray10[StudyDatearray10.Length - 1]);
                String agefromtable10 = Convert.ToString(Studydateyear10 - YOB10);
                String percentile10 = brz3dvp.GetPercentilevalues(gender: "O");
                String patientage10 = brz3dvp.GetPercentilevalues("age");
                Logger.Instance.InfoLog("Percentile value in step 10 is : " + percentile10);
                Logger.Instance.InfoLog("Percentile value in step 10 is : " + patientage10);
                if (agefromtable10 == patientage10 && percentile10 != null && percentile10 != "")
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x11 = { CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 150 };
                int[] y11= { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 400, CalciumScoreImage.Size.Height / 2 + 400, CalciumScoreImage.Size.Height / 2 + 200 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x11, y11);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(2000);
                if (browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 4).Click().Release().Build().Perform();
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                String percentile11 = brz3dvp.GetPercentilevalues(gender: "O");
                if (percentile11.Contains("90+"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                String filepath12 = Config.downloadpath + "\\" + testid + "_" + ExecutedSteps + "_121s.txt";
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    File.WriteAllText(filepath12, text);
                }
                string[] percentilevalues = percentile11.Split('_');
                string[] readText12 = File.ReadAllLines(filepath12);
                try
                {
                    int counter = 0;
                    for (int i = 0; i < readText12.Length; i++)
                    {
                        if ((readText12[i].Contains(percentilevalues[0]) && readText12[i].Contains(percentilevalues[1])) || readText12[i].Contains(patientage10))
                        {
                            counter++;
                        }
                    }
                    if (counter == 2)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception exp)
                {
                    Logger.Instance.ErrorLog("No content in the text file");
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 13
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedBefore13 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 131, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                int RedAfter13 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 132, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                String percentile13 = brz3dvp.GetPercentilevalues(gender: "O");
                if (percentile13 == "0_0" && RedAfter13 < RedBefore13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                brz3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                Result = brz3dvp.searchandopenstudyin3D(Patientid[2], thumbnail[3], layout: BluRingZ3DViewerPage.CalciumScoring);
                if (!Result)
                {
                    Result = brz3dvp.checkerrormsg("y");
                    if (Result)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                    {
                        Logger.Instance.ErrorLog("Study with thickness not equal to 3 mm opened without any error");
                        throw new Exception("Study with thickness not equal to 3 mm opened without any error");
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Study with thickness not equal to 3 mm opened without any error");
                    throw new Exception("Study with thickness not equal to 3 mm opened without any error");
                }

                //step 16
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 255, 0, 0, 2);
                PageLoadWait.WaitForFrameLoad(5);
                int[] x16 = { CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 + 250 };
                int[] y16 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                Thread.Sleep(2000);
                if (browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 4).Click().Release().Build().Perform();
                Thread.Sleep(3000);
                int RedAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("LM");
                if (RedAfter16 > RedBefore16 && ScoreValuesBefore16 != ScoreValuesAfter16)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                String percentile17 = brz3dvp.GetPercentilevalues();
                String patientage17 = brz3dvp.GetPercentilevalues("age");
                Logger.Instance.InfoLog("Percentile value in step 17 is : " + percentile17);
                Logger.Instance.InfoLog("Percentile value in step 17 is : " + patientage17);
                if (patientage17 == "0" && percentile17.Equals("Unknown. Patient age absent in DICOM header."))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                String filepath18 = Config.downloadpath + "\\" + testid + "_" + ExecutedSteps + "_181.txt";
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    File.WriteAllText(filepath18, text);
                }
                string[] readText18 = File.ReadAllLines(filepath18);
                try
                {
                    int counter = 0;
                    for (int i = 0; i < readText12.Length; i++)
                    {
                        if (readText18[i].Contains("Unknown. Patient age absent in DICOM header.") || readText18[i].Contains("Patient Age: \t\t0"))
                        {
                            counter++;
                        }
                    }
                    if (counter == 2)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception exp)
                {
                    Logger.Instance.ErrorLog("No content in the text file");
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 19
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedBefore19 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 191, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                int RedAfter19 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 192, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter19 = brz3dvp.CalciumScoringTableValues("LM");
                if (ScoreValuesAfter19[0] == 0.00 && ScoreValuesAfter19[1] == 0.00 && RedAfter19 < RedBefore19)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163281(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedColorBefore4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 40, 255, 0, 0, 2);
                int GreenColorBefore4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 41, 0, 255, 0, 2);
                int BlueColorBefore4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 43, 0, 0, 255, 2);
                int GBColorBefore4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 47, 0, 255, 255, 2);
                int YellowColorBefore4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 45, 255, 255, 0, 2);
                PageLoadWait.WaitForFrameLoad(5);
                int[] x4_1 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y4_1 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_1, y4_1);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x4_2 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y4_2 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_2, y4_2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x4_3 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y4_3 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Height / 2 + 200).ClickAndHold()
                          .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Height / 2 + 200)
                          .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Height / 2 + 350)
                          .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Height / 2 + 250).Release().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x4_4 = { CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100 };
                int[] y4_4 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_4, y4_4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x4_5 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y4_5 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_5, y4_5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 48, 0, 255, 255, 2);
                int GreenColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 42, 0, 255, 0, 2);
                int BlueColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 44, 0, 0, 255, 2);
                int YellowColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 46, 255, 255, 0, 2);
                int RedColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 49, 255, 0, 0, 2);
                if (RedColorAfter4 > RedColorBefore4 && GreenColorAfter4 > GreenColorBefore4 && BlueColorAfter4 > BlueColorBefore4 && GBColorAfter4 > GBColorBefore4 && YellowColorAfter4 > YellowColorBefore4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                {
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    String WinLevelBefore = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring);
                    PageLoadWait.WaitForFrameLoad(10);
                    int[] x5 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 };
                    int[] y5 = { CalciumScoreImage.Size.Height / 4, (CalciumScoreImage.Size.Height / 4) * 3 };
                    PageLoadWait.WaitForFrameLoad(2);
                    new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 4).ClickAndHold()
                        .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, (CalciumScoreImage.Size.Height / 4) * 3).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    String WinLevelAfter = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring);
                    if (WinLevelAfter != WinLevelBefore)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                Result = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                String imagename = testid + ExecutedSteps + 71;
                String Step7_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step7_imgLocation))
                    File.Delete(Step7_imgLocation);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                if (File.Exists(Step7_imgLocation))
                {
                    Result = brz3dvp.CompareDownloadimage(Step7_imgLocation);
                    if (Result)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter8_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesAfter8_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesAfter8_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesAfter8_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> ScoreValuesAfter8_5 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter8_6 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                String filepath = Config.downloadpath + "\\" + testid + "_" + ExecutedSteps + "_91.txt";
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    File.WriteAllText(filepath, text);
                }
                string[] readText = File.ReadAllLines(filepath);
                try
                {
                    int counter = 0;
                    for (int i = 0; i < readText.Length; i++)
                    {
                        bool Verification1 = readText[i].Contains(Convert.ToString(ScoreValuesAfter8_1[0])) && readText[i].Contains(Convert.ToString(ScoreValuesAfter8_1[1]));
                        bool Verification2 = readText[i].Contains(Convert.ToString(ScoreValuesAfter8_2[0])) && readText[i].Contains(Convert.ToString(ScoreValuesAfter8_2[1]));
                        bool Verification3 = readText[i].Contains(Convert.ToString(ScoreValuesAfter8_3[0])) && readText[i].Contains(Convert.ToString(ScoreValuesAfter8_3[1]));
                        bool Verification4 = readText[i].Contains(Convert.ToString(ScoreValuesAfter8_4[0])) && readText[i].Contains(Convert.ToString(ScoreValuesAfter8_4[1]));
                        bool Verification5 = readText[i].Contains(Convert.ToString(ScoreValuesAfter8_5[0])) && readText[i].Contains(Convert.ToString(ScoreValuesAfter8_5[1]));
                        bool Verification6 = readText[i].Contains(Convert.ToString(TotalValuesAfter8_6[0])) && readText[i].Contains(Convert.ToString(TotalValuesAfter8_6[1]));
                        if (Verification1 || Verification2 || Verification3 || Verification4 || Verification5 || Verification6)
                        {
                            counter++;
                        }
                    }
                    if (counter == 6)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception exp)
                {
                    Logger.Instance.ErrorLog("No content in the text file");
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 10
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect this slice");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int RedColorBefore11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 110, 255, 0, 0, 2);
                int GreenColorBefore11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 111, 0, 255, 0, 2);
                int BlueColorBefore11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 112, 0, 0, 255, 2);
                int GBColorBefore11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 113, 0, 255, 255, 2);
                int YellowColorBefore11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 114, 255, 255, 0, 2);
                PageLoadWait.WaitForFrameLoad(5);
                int[] x11_0 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y11_0 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_0, y11_0);
                Thread.Sleep(21000);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x11_1 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y11_1 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_1, y11_1);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x11_2 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y11_2 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_2, y11_2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x11_3 = { CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100 };
                int[] y11_3 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_3, y11_3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x11_4 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y11_4 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_4, y11_4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 115, 0, 255, 255, 2);
                int GreenColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 116, 0, 255, 0, 2);
                int BlueColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 117, 0, 0, 255, 2);
                int YellowColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 118, 255, 255, 0, 2);
                int RedColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 119, 255, 0, 0, 2);
                if (RedColorAfter11 < RedColorBefore11 && GreenColorAfter11 < GreenColorBefore11 && BlueColorAfter11 < BlueColorBefore11 && GBColorAfter11 < GBColorBefore11 && YellowColorAfter11 < YellowColorBefore11)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter12_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesAfter12_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesAfter12_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesAfter12_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> ScoreValuesAfter12_5 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter12_6 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                String filepath13 = Config.downloadpath + "\\" + testid + "_" + ExecutedSteps + "_131.txt";
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    File.WriteAllText(filepath13, text);
                }
                string[] readText13 = File.ReadAllLines(filepath13);
                try
                {
                    int counter = 0;
                    for (int i = 0; i < readText13.Length; i++)
                    {
                        bool Verification1 = readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_1[0])) && readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_1[1]));
                        bool Verification2 = readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_2[0])) && readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_2[1]));
                        bool Verification3 = readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_3[0])) && readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_3[1]));
                        bool Verification4 = readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_4[0])) && readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_4[1]));
                        bool Verification5 = readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_5[0])) && readText13[i].Contains(Convert.ToString(ScoreValuesAfter12_5[1]));
                        bool Verification6 = readText13[i].Contains(Convert.ToString(TotalValuesAfter12_6[0])) && readText13[i].Contains(Convert.ToString(TotalValuesAfter12_6[1]));
                        if (Verification1 || Verification2 || Verification3 || Verification4 || Verification5 || Verification6)
                        {
                            counter++;
                        }
                    }
                    if (counter >= 6)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception exp)
                {
                    Logger.Instance.ErrorLog("No content in the text file");
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 14
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                {
                    CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    String WinLevelBefore = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring);
                    int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 };
                    int[] y14 = { CalciumScoreImage.Size.Height / 4, (CalciumScoreImage.Size.Height / 4) * 3 };
                    brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                    PageLoadWait.WaitForFrameLoad(5);
                    String WinLevelAfter = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CalciumScoring);
                    if (WinLevelAfter != WinLevelBefore)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                Result = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                String imagename16 = testid + ExecutedSteps + 161;
                String Step16_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step16_imgLocation))
                    File.Delete(Step16_imgLocation);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                if (File.Exists(Step16_imgLocation))
                {
                    Result = brz3dvp.CompareDownloadimage(Step16_imgLocation);
                    if (Result)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                String wlvaluebefore = brz3dvp.GetTopLeftAnnotationValue("Calcium Scoring");
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.CalciumScoring, "Abdomen", "preset");
                if (Result)
                {
                    PageLoadWait.WaitForFrameLoad(3);
                    String wlvalueafter = brz3dvp.GetTopLeftAnnotationValue("Calcium Scoring");
                    if (wlvalueafter != wlvaluebefore)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                Result = brz3dvp.select3DTools(Z3DTools.Download_Image);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                String imagename19 = testid + ExecutedSteps + 191;
                String Step19_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step19_imgLocation))
                    File.Delete(Step19_imgLocation);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                if (File.Exists(Step19_imgLocation))
                {
                    Result = brz3dvp.CompareDownloadimage(Step19_imgLocation);
                    if (Result)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();


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

        public TestCaseResult Test_163268(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            bool TotalVolumeVerification, TotalScoreVerification;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select all contiguous"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                IList<Double> ScoreValuesBefore4 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Select all contiguous");
                int RedColorBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 41, 255, 0, 0, 2);
                int[] x4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4, y4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 42, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter4 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter4 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 4 : " + (ScoreValuesBefore4[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 4 : " + (ScoreValuesBefore4[1]));
                Logger.Instance.InfoLog("LM Score Value After 4 : " + (ScoreValuesAfter4[0]));
                Logger.Instance.InfoLog("LM Volume Value After 4 : " + (ScoreValuesAfter4[1]));
                Logger.Instance.InfoLog("Total Score Value After 4 : " + (TotalValuesAfter4[0]));
                Logger.Instance.InfoLog("Total Volume Value After 4 : " + (TotalValuesAfter4[1]));
                Double TotalVolume4 = ScoreValuesAfter4[0];
                Double TotalScore4 = ScoreValuesAfter4[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter4[0]).Equals(Convert.ToInt32(TotalVolume4));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter4[1]).Equals(Convert.ToInt32(TotalScore4));
                if (RedColorAfter4 != RedColorBefore4 && ((ScoreValuesBefore4[0]) < (ScoreValuesAfter4[0])) && ((ScoreValuesBefore4[1]) < (ScoreValuesAfter4[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                IList<Double> ScoreValuesBefore5 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select all contiguous"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                IList<Double> ScoreValuesBefore6 = brz3dvp.CalciumScoringTableValues("RCA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                int GreenColorBefore6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 61, 0, 255, 0, 2);
                int[] x6 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y6 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6, y6);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 62, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter6 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> TotalValuesAfter6 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("RCA Score Value Before 6 : " + (ScoreValuesBefore6[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 6 : " + (ScoreValuesBefore6[1]));
                Logger.Instance.InfoLog("RCA Score Value After 6 : " + (ScoreValuesAfter6[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 6 : " + (ScoreValuesAfter6[1]));
                Logger.Instance.InfoLog("Total Score Value After 6 : " + (TotalValuesAfter6[0]));
                Logger.Instance.InfoLog("Total Volume Value After 6 : " + (TotalValuesAfter6[1]));
                Double TotalVolume6 = ScoreValuesAfter4[0] + ScoreValuesAfter6[0];
                Double TotalScore6 = ScoreValuesAfter4[1] + ScoreValuesAfter6[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter6[0]).Equals(Convert.ToInt32(TotalVolume6));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter6[1]).Equals(Convert.ToInt32(TotalScore6));
                if (GreenColorAfter6 != GreenColorBefore6 && ((ScoreValuesBefore6[0]) < (ScoreValuesAfter6[0])) && ((ScoreValuesBefore6[1]) < (ScoreValuesAfter6[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select all contiguous"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<Double> ScoreValuesBefore8 = brz3dvp.CalciumScoringTableValues("LAD");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                int BlueColorBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 0, 0, 255, 2);
                int[] x8 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200};
                int[] y8 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueColorAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 82, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter8 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> TotalValuesAfter8 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LAD Score Value Before 8 : " + (ScoreValuesBefore8[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 8 : " + (ScoreValuesBefore8[1]));
                Logger.Instance.InfoLog("LAD Score Value After 8 : " + (ScoreValuesAfter8[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 8 : " + (ScoreValuesAfter8[1]));
                Logger.Instance.InfoLog("Total Score Value After 8 : " + (TotalValuesAfter8[0]));
                Logger.Instance.InfoLog("Total Volume Value After 8 : " + (TotalValuesAfter8[1]));
                Double TotalVolume8 = TotalVolume6 + ScoreValuesAfter8[0];
                Double TotalScore8 = TotalScore6 + ScoreValuesAfter8[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter8[0]).Equals(Convert.ToInt32(TotalVolume8));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter8[1]).Equals(Convert.ToInt32(TotalScore8));
                if (BlueColorAfter8 != BlueColorBefore8 && ((ScoreValuesBefore8[0]) < (ScoreValuesAfter8[0])) && ((ScoreValuesBefore8[1]) < (ScoreValuesAfter8[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select all contiguous"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("CX");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                int YellowColorBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 101, 255, 255, 0, 2);
                int[] x10 = { CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100};
                int[] y10 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowColorAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 102, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> TotalValuesAfter10 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("CX Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                Logger.Instance.InfoLog("CX Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                Logger.Instance.InfoLog("CX Score Value After 10 : " + (ScoreValuesAfter10[0]));
                Logger.Instance.InfoLog("CX Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                Logger.Instance.InfoLog("Total Score Value After 10 : " + (TotalValuesAfter10[0]));
                Logger.Instance.InfoLog("Total Volume Value After 10 : " + (TotalValuesAfter10[1]));
                Double TotalVolume10 = TotalVolume8 + ScoreValuesAfter10[0];
                Double TotalScore10 = TotalScore8 + ScoreValuesAfter10[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter10[0]).Equals(Convert.ToInt32(TotalVolume10));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter10[1]).Equals(Convert.ToInt32(TotalScore10));
                if (YellowColorBefore10 != YellowColorAfter10 && ((ScoreValuesBefore10[0]) < (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) < (ScoreValuesAfter10[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                if (brz3dvp.Verifyradiobuttonenabled("select all contiguous"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                IList<Double> ScoreValuesBefore12 = brz3dvp.CalciumScoringTableValues("PDA");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Select all contiguous");
                int GBColorBefore12 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 121, 0, 255, 255, 2);
                int[] x12 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y12 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x12, y12);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter12 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 122, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter12 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter12 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("PDA Score Value Before 10 : " + (ScoreValuesBefore12[0]));
                Logger.Instance.InfoLog("PDA Volume Value Before 10 : " + (ScoreValuesBefore12[1]));
                Logger.Instance.InfoLog("PDA Score Value After 10 : " + (ScoreValuesAfter12[0]));
                Logger.Instance.InfoLog("PDA Volume Value After 10 : " + (ScoreValuesAfter12[1]));
                Logger.Instance.InfoLog("Total Score Value After 10 : " + (TotalValuesAfter12[0]));
                Logger.Instance.InfoLog("Total Volume Value After 10 : " + (TotalValuesAfter12[1]));
                Double TotalVolume12 = TotalVolume10 + ScoreValuesAfter12[0];
                Double TotalScore12 = TotalScore10 + ScoreValuesAfter12[1];
                TotalVolumeVerification = Convert.ToInt32(TotalValuesAfter12[0]).Equals(Convert.ToInt32(TotalVolume12));
                TotalScoreVerification = Convert.ToInt32(TotalValuesAfter12[1]).Equals(Convert.ToInt32(TotalScore12));
                if (GBColorBefore12 != GBColorAfter12 && ((ScoreValuesBefore12[0]) < (ScoreValuesAfter12[0])) && ((ScoreValuesBefore12[1]) < (ScoreValuesAfter12[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                IList<Double> ScoreValuesBefore13 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesBefore13 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect all contiguous");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                int RedColorBefore13 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 141, 255, 0, 0, 2);
                int[] x14 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y14 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x14, y14);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter13 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 142, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter13 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter13 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 14 : " + (ScoreValuesBefore13[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 14 : " + (ScoreValuesBefore13[1]));
                Logger.Instance.InfoLog("LM Score Value After 14 : " + (ScoreValuesAfter13[0]));
                Logger.Instance.InfoLog("LM Volume Value After 14 : " + (ScoreValuesAfter13[1]));
                Logger.Instance.InfoLog("Total Score Value After 14 : " + (TotalValuesAfter13[0]));
                Logger.Instance.InfoLog("Total Volume Value After 14 : " + (TotalValuesAfter13[1]));
                Logger.Instance.InfoLog("Total Score Value Before 14 : " + (TotalValuesBefore13[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 14 : " + (TotalValuesBefore13[1]));
                Double diffcalvolume13 = TotalValuesBefore13[0] - TotalValuesAfter13[0];
                Double diffcalscore13 = TotalValuesBefore13[1] - TotalValuesAfter13[1];
                Double diffvolume13 = ScoreValuesBefore13[0] - ScoreValuesAfter13[0];
                Double diffscore13 = ScoreValuesBefore13[1] - ScoreValuesAfter13[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume13).Equals(Convert.ToInt32(diffvolume13));
                TotalScoreVerification = Convert.ToInt32(diffcalscore13).Equals(Convert.ToInt32(diffscore13));
                if (RedColorAfter13 != RedColorBefore13 && ((ScoreValuesBefore13[0]) > (ScoreValuesAfter13[0])) && ((ScoreValuesBefore13[1]) > (ScoreValuesAfter13[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                IList<Double> ScoreValuesBefore16 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesBefore16 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect all contiguous");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                int GBColorBefore16 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 161, 0, 255, 255, 2);
                int[] x16 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y16 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x16, y16);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter16 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 162, 0, 255, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter16 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter16 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 16 : " + (ScoreValuesBefore16[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 16 : " + (ScoreValuesBefore16[1]));
                Logger.Instance.InfoLog("LM Score Value After 16 : " + (ScoreValuesAfter16[0]));
                Logger.Instance.InfoLog("LM Volume Value After 16 : " + (ScoreValuesAfter16[1]));
                Logger.Instance.InfoLog("Total Score Value After 16 : " + (TotalValuesAfter16[0]));
                Logger.Instance.InfoLog("Total Volume Value After 16 : " + (TotalValuesAfter16[1]));
                Logger.Instance.InfoLog("Total Score Value Before 16 : " + (TotalValuesBefore16[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 16 : " + (TotalValuesBefore16[1]));
                Double diffcalvolume16 = TotalValuesBefore16[0] - TotalValuesAfter16[0];
                Double diffcalscore16 = TotalValuesBefore16[1] - TotalValuesAfter16[1];
                Double diffvolume16 = ScoreValuesBefore16[0] - ScoreValuesAfter16[0];
                Double diffscore16 = ScoreValuesBefore16[1] - ScoreValuesAfter16[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume16).Equals(Convert.ToInt32(diffvolume16));
                TotalScoreVerification = Convert.ToInt32(diffcalscore16).Equals(Convert.ToInt32(diffscore16));
                if (GBColorBefore16 != GBColorAfter16 && ((ScoreValuesBefore16[0]) > (ScoreValuesAfter16[0])) && ((ScoreValuesBefore16[1]) > (ScoreValuesAfter16[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                IList<Double> ScoreValuesBefore17 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> TotalValuesBefore17 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect all contiguous");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                int[] x18 = { CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100};
                int[] y18 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x18, y18);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int YellowColorAfter17 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 172, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter17 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> TotalValuesAfter17 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 18 : " + (ScoreValuesBefore17[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 18 : " + (ScoreValuesBefore17[1]));
                Logger.Instance.InfoLog("LM Score Value After 18 : " + (ScoreValuesAfter17[0]));
                Logger.Instance.InfoLog("LM Volume Value After 18 : " + (ScoreValuesAfter17[1]));
                Logger.Instance.InfoLog("Total Score Value After 18 : " + (TotalValuesAfter17[0]));
                Logger.Instance.InfoLog("Total Volume Value After 18 : " + (TotalValuesAfter17[1]));
                Logger.Instance.InfoLog("Total Score Value Before 18 : " + (TotalValuesBefore17[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 18 : " + (TotalValuesBefore17[1]));
                Double diffcalvolume17 = TotalValuesBefore17[0] - TotalValuesAfter17[0];
                Double diffcalscore17 = TotalValuesBefore17[1] - TotalValuesAfter17[1];
                Double diffvolume17 = ScoreValuesBefore17[0] - ScoreValuesAfter17[0];
                Double diffscore17 = ScoreValuesBefore17[1] - ScoreValuesAfter17[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume17).Equals(Convert.ToInt32(diffvolume17));
                TotalScoreVerification = Convert.ToInt32(diffcalscore17).Equals(Convert.ToInt32(diffscore17));
                if (YellowColorAfter17 != YellowColorAfter10 && ((ScoreValuesBefore17[0]) > (ScoreValuesAfter17[0])) && ((ScoreValuesBefore17[1]) > (ScoreValuesAfter17[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                IList<Double> ScoreValuesBefore20 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> TotalValuesBefore20 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect all contiguous");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                int BlueColorBefore20 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                int[] x20 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y20 = { CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 350, CalciumScoreImage.Size.Height / 2 + 250 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x20, y20);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int BlueColorAfter20 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 202, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter20 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> TotalValuesAfter20 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LAD Score Value Before 20 : " + (ScoreValuesBefore20[0]));
                Logger.Instance.InfoLog("LAD Volume Value Before 20 : " + (ScoreValuesBefore20[1]));
                Logger.Instance.InfoLog("LAD Score Value After 20 : " + (ScoreValuesAfter20[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 20 : " + (ScoreValuesAfter20[1]));
                Logger.Instance.InfoLog("Total Score Value After 20 : " + (TotalValuesAfter20[0]));
                Logger.Instance.InfoLog("Total Volume Value After 20 : " + (TotalValuesAfter20[1]));
                Double diffcalvolume20 = TotalValuesBefore20[0] - TotalValuesAfter20[0];
                Double diffcalscore20 = TotalValuesBefore20[1] - TotalValuesAfter20[1];
                Double diffvolume20 = ScoreValuesBefore20[0] - ScoreValuesAfter20[0];
                Double diffscore20 = ScoreValuesBefore20[1] - ScoreValuesAfter20[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume20).Equals(Convert.ToInt32(diffvolume20));
                TotalScoreVerification = Convert.ToInt32(diffcalscore20).Equals(Convert.ToInt32(diffscore20));
                if (BlueColorAfter20 != BlueColorBefore20 && ((ScoreValuesBefore20[0]) > (ScoreValuesAfter20[0])) && ((ScoreValuesBefore20[1]) > (ScoreValuesAfter20[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 21
                IList<Double> ScoreValuesBefore22 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> TotalValuesBefore22 = brz3dvp.CalciumScoringTableValues("Total");
                Result = brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 22
                int GreenColorBefore22 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 221, 0, 255, 0, 2);
                int[] x22 = { CalciumScoreImage.Size.Width / 2 , CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y22 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x22, y22);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter22 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 222, 0, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter22 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> TotalValuesAfter22 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("RCA Score Value Before 22 : " + (ScoreValuesBefore22[0]));
                Logger.Instance.InfoLog("RCA Volume Value Before 22 : " + (ScoreValuesBefore22[1]));
                Logger.Instance.InfoLog("RCA Score Value After 22 : " + (ScoreValuesAfter22[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 22 : " + (ScoreValuesAfter22[1]));
                Logger.Instance.InfoLog("Total Score Value After 22 : " + (TotalValuesAfter22[0]));
                Logger.Instance.InfoLog("Total Volume Value After 22 : " + (TotalValuesAfter22[1]));
                Double diffcalvolume22 = TotalValuesBefore22[0] - TotalValuesAfter22[0];
                Double diffcalscore22 = TotalValuesBefore22[1] - TotalValuesAfter22[1];
                Double diffvolume22 = ScoreValuesBefore22[0] - ScoreValuesAfter22[0];
                Double diffscore22 = ScoreValuesBefore22[1] - ScoreValuesAfter22[1];
                TotalVolumeVerification = Convert.ToInt32(diffcalvolume22).Equals(Convert.ToInt32(diffvolume22));
                TotalScoreVerification = Convert.ToInt32(diffcalscore22).Equals(Convert.ToInt32(diffscore22));
                if (GreenColorAfter22 != GreenColorBefore22 && ((ScoreValuesBefore22[0]) > (ScoreValuesAfter22[0])) && ((ScoreValuesBefore22[1]) > (ScoreValuesAfter22[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


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
       
        public TestCaseResult Test_163274(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String objlocation = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            bool TotalVolumeVerification, TotalScoreVerification;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, ImageCount, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03
                IWebElement CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                IList<Double> ScoreValuesBefore3 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM");
                int RedColorBefore3 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 31, 255, 0, 0, 2);
                int[] x3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x3, y3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter3 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 32, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter3 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter3 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 3 : " + (ScoreValuesBefore3[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 3 : " + (ScoreValuesBefore3[1]));
                Logger.Instance.InfoLog("LM Score Value After 3 : " + (ScoreValuesAfter3[0]));
                Logger.Instance.InfoLog("LM Volume Value After 3 : " + (ScoreValuesAfter3[1]));
                Logger.Instance.InfoLog("Total Score Value After 3 : " + (TotalValuesAfter3[0]));
                Logger.Instance.InfoLog("Total Volume Value After 3 : " + (TotalValuesAfter3[1]));
                Double TotalVolume3 = ScoreValuesAfter3[0];
                Double TotalScore3 = ScoreValuesAfter3[1];
                TotalVolumeVerification = TotalValuesAfter3[0].Equals(TotalVolume3);
                TotalScoreVerification = TotalValuesAfter3[1].Equals(TotalScore3);
                if (RedColorAfter3 != RedColorBefore3 && ((ScoreValuesBefore3[0]) < (ScoreValuesAfter3[0])) && ((ScoreValuesBefore3[1]) < (ScoreValuesAfter3[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA");
                int GreenColorBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 41, 0, 255, 0, 2);
                int BlueColorBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 43, 0, 0, 255, 2);
                int GBColorBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 47, 0, 255, 255, 2);
                int YellowColorBefore4 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 45, 255, 255, 0, 2);
                int[] x4_1 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y4_1 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_1, y4_1);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD");
                int[] x4_2 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y4_2 = { CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 305, CalciumScoreImage.Size.Height / 2 + 305 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_2, y4_2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX");
                int[] x4_3 = { (CalciumScoreImage.Size.Width / 4) * 3, CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (CalciumScoreImage.Size.Width / 4) * 3 };
                int[] y4_3 = { (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 40, (CalciumScoreImage.Size.Height / 4) + 40 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_3, y4_3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA");
                int[] x4_4 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y4_4 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x4_4, y4_4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 48, 0, 255, 255, 2);
                int GreenColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 42, 0, 255, 0, 2);
                int BlueColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 44, 0, 0, 255, 2);
                int YellowColorAfter4 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 46, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter4_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesAfter4_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesAfter4_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesAfter4_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter4 = brz3dvp.CalciumScoringTableValues("Total");
                Double TotalcalVolume = TotalVolume3 + ScoreValuesAfter4_1[0] + ScoreValuesAfter4_2[0] + ScoreValuesAfter4_3[0] + ScoreValuesAfter4_4[0];
                Double TotalcalScore = TotalScore3 + ScoreValuesAfter4_1[1] + ScoreValuesAfter4_2[1] + ScoreValuesAfter4_3[1] + ScoreValuesAfter4_4[1];
                bool ColorVerification = GBColorAfter4 > GBColorBefore4 && GreenColorAfter4 > GreenColorBefore4 && BlueColorAfter4 > BlueColorBefore4 && YellowColorAfter4 > YellowColorBefore4;
                if (ColorVerification && Convert.ToInt32(TotalValuesAfter4[0]) == Convert.ToInt32(TotalcalVolume) && Convert.ToInt32(TotalValuesAfter4[1]) == Convert.ToInt32(TotalcalScore))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                IList<Double> ScoreValuesBefore5 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesBefore5 = brz3dvp.CalciumScoringTableValues("Total");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect this slice");
                int RedColorBefore5 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 51, 255, 0, 0, 2);
                int[] x5 = { (CalciumScoreImage.Size.Width / 4) * 3, CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (CalciumScoreImage.Size.Width / 4) * 3 };
                int[] y5 = { (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 40, (CalciumScoreImage.Size.Height / 4) + 40 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x5, y5);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter5 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 52, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter5 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter5 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 5 : " + (ScoreValuesBefore5[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 5 : " + (ScoreValuesBefore5[1]));
                Logger.Instance.InfoLog("Total Score Value Before 5 : " + (TotalValuesBefore5[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 5 : " + (TotalValuesBefore5[1]));
                Logger.Instance.InfoLog("LM Score Value After 5 : " + (ScoreValuesAfter5[0]));
                Logger.Instance.InfoLog("LM Volume Value After 5 : " + (ScoreValuesAfter5[1]));
                Logger.Instance.InfoLog("Total Score Value After 5 : " + (TotalValuesAfter5[0]));
                Logger.Instance.InfoLog("Total Volume Value After 5 : " + (TotalValuesAfter5[1]));
                TotalVolumeVerification = TotalValuesAfter5[0].Equals(TotalValuesBefore5[0]);
                TotalScoreVerification = TotalValuesAfter5[1].Equals(TotalValuesBefore5[1]);
                if (RedColorAfter5 == RedColorBefore5 && ((ScoreValuesBefore5[0]) == (ScoreValuesAfter5[0])) && ((ScoreValuesBefore5[1]) == (ScoreValuesAfter5[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                IList<Double> ScoreValuesBefore6_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesBefore6_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesBefore6_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesBefore6_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesBefore6 = brz3dvp.CalciumScoringTableValues("Total");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect this slice");
                int GreenColorBefore6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 61, 0, 255, 0, 2);
                int BlueColorBefore6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 62, 0, 0, 255, 2);
                int GBColorBefore6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 63, 0, 255, 255, 2);
                int YellowColorBefore6 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 64, 255, 255, 0, 2);
                //LAD
                int[] x6_1 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y6_1 = { CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 305, CalciumScoreImage.Size.Height / 2 + 305 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6_1, y6_1);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect this slice");
                //PDA
                int[] x6_2 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y6_2 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6_2, y6_2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect this slice");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x6_3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y6_3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6_3, y6_3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect this slice");
                //RCA
                int[] x6_4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y6_4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x6_4, y6_4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 65, 0, 255, 255, 2);
                int GreenColorAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 66, 0, 255, 0, 2);
                int BlueColorAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 67, 0, 0, 255, 2);
                int YellowColorAfter6 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 68, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter6_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesAfter6_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesAfter6_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesAfter6_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter6 = brz3dvp.CalciumScoringTableValues("Total");
                ColorVerification = GBColorAfter6 == GBColorBefore6 && GreenColorAfter6 == GreenColorBefore6 && BlueColorAfter6 == BlueColorBefore6 && YellowColorAfter6 == YellowColorBefore6;
                bool VolumeVerify1 = ScoreValuesAfter6_1[0] == ScoreValuesBefore6_1[0] && ScoreValuesAfter6_1[1] == ScoreValuesBefore6_1[1];
                bool VolumeVerify2 = ScoreValuesAfter6_2[0] == ScoreValuesBefore6_2[0] && ScoreValuesAfter6_2[1] == ScoreValuesBefore6_2[1];
                bool VolumeVerify3 = ScoreValuesAfter6_3[0] == ScoreValuesBefore6_3[0] && ScoreValuesAfter6_3[1] == ScoreValuesBefore6_3[1];
                bool VolumeVerify4 = ScoreValuesAfter6_4[0] == ScoreValuesBefore6_4[0] && ScoreValuesAfter6_4[1] == ScoreValuesBefore6_4[1];
                bool TotalVolumeVerify = TotalValuesAfter6[0] == TotalValuesBefore6[0] && TotalValuesAfter6[1] == TotalValuesBefore6[1];
                if (ColorVerification && VolumeVerify1 && VolumeVerify2 && VolumeVerify3 && VolumeVerify4 && TotalVolumeVerify)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                int GBColorAfter7 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 71, 0, 255, 255, 2);
                int GreenColorAfter7 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 72, 0, 255, 0, 2);
                int BlueColorAfter7 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 73, 0, 0, 255, 2);
                int YellowColorAfter7 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 74, 255, 255, 0, 2);
                String CalciumScoreAnnotationValue = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                bool colorverifcation_7 = GBColorAfter6 > GBColorAfter7 && GreenColorAfter6 > GreenColorAfter7 && BlueColorAfter6 > BlueColorAfter7 && YellowColorAfter6 > YellowColorAfter7;
                if (colorverifcation_7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                IList<Double> ScoreValuesBefore8 = brz3dvp.CalciumScoringTableValues("LM");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Select all contiguous");
                int RedColorBefore8 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 81, 255, 0, 0, 2);
                int[] x8 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y8 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x8, y8);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter8 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 82, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter8 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter8 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 8 : " + (ScoreValuesBefore8[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 8 : " + (ScoreValuesBefore8[1]));
                Logger.Instance.InfoLog("LM Score Value After 8 : " + (ScoreValuesAfter8[0]));
                Logger.Instance.InfoLog("LM Volume Value After 8 : " + (ScoreValuesAfter8[1]));
                Logger.Instance.InfoLog("Total Score Value After 8 : " + (TotalValuesAfter8[0]));
                Logger.Instance.InfoLog("Total Volume Value After 8 : " + (TotalValuesAfter8[1]));
                Double TotalVolume8 = ScoreValuesAfter8[0];
                Double TotalScore8 = ScoreValuesAfter8[1];
                TotalVolumeVerification = TotalValuesAfter8[0].Equals(TotalVolume8);
                TotalScoreVerification = TotalValuesAfter8[1].Equals(TotalScore8);
                if (RedColorAfter8 > RedColorBefore8 && ((ScoreValuesBefore8[0]) < (ScoreValuesAfter8[0])) && ((ScoreValuesBefore8[1]) < (ScoreValuesAfter8[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                int GreenColorBefore9 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 91, 0, 255, 0, 2);
                int BlueColorBefore9 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 92, 0, 0, 255, 2);
                int GBColorBefore9 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 93, 0, 255, 255, 2);
                int YellowColorBefore9 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 94, 255, 255, 0, 2);
                int[] x9_1 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y9_1 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x9_1, y9_1);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                int[] x9_2 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y9_2 = { CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 305, CalciumScoreImage.Size.Height / 2 + 305 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x9_2, y9_2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x9_3 = { (CalciumScoreImage.Size.Width / 4) * 3, CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (CalciumScoreImage.Size.Width / 4) * 3 };
                int[] y9_3 = { (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 40, (CalciumScoreImage.Size.Height / 4) + 40 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x9_3, y9_3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Select all contiguous");
                int[] x9_4 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y9_4 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x9_4, y9_4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter9 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 95, 0, 255, 255, 2);
                int GreenColorAfter9 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 96, 0, 255, 0, 2);
                int BlueColorAfter9 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 97, 0, 0, 255, 2);
                int YellowColorAfter9 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 98, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter9_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesAfter9_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesAfter9_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesAfter9_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter9 = brz3dvp.CalciumScoringTableValues("Total");
                TotalcalVolume = TotalVolume8 + ScoreValuesAfter9_1[0] + ScoreValuesAfter9_2[0] + ScoreValuesAfter9_3[0] + ScoreValuesAfter9_4[0];
                TotalcalScore = TotalScore8 + ScoreValuesAfter9_1[1] + ScoreValuesAfter9_2[1] + ScoreValuesAfter9_3[1] + ScoreValuesAfter9_4[1];
                ColorVerification = GBColorAfter9 > GBColorBefore9 && GreenColorAfter9 > GreenColorBefore9 && BlueColorAfter9 > BlueColorBefore9 && YellowColorAfter9 > YellowColorBefore9;
                if (ColorVerification && Convert.ToInt32(TotalValuesAfter9[0]) == Convert.ToInt32(TotalcalVolume) && Convert.ToInt32(TotalValuesAfter9[1]) == Convert.ToInt32(TotalcalScore))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                IList<Double> ScoreValuesBefore10 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesBefore10 = brz3dvp.CalciumScoringTableValues("Total");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Deselect all contiguous");
                int RedColorBefore10 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 101, 255, 0, 0, 2);
                int[] x10 = { (CalciumScoreImage.Size.Width / 4) * 3, CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (CalciumScoreImage.Size.Width / 4) * 3 };
                int[] y10 = { (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 20, (CalciumScoreImage.Size.Height / 4) + 40, (CalciumScoreImage.Size.Height / 4) + 40 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x10, y10);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int RedColorAfter10 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 102, 255, 0, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter10 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter10 = brz3dvp.CalciumScoringTableValues("Total");
                Logger.Instance.InfoLog("LM Score Value Before 10 : " + (ScoreValuesBefore10[0]));
                Logger.Instance.InfoLog("LM Volume Value Before 10 : " + (ScoreValuesBefore10[1]));
                Logger.Instance.InfoLog("Total Score Value Before 10 : " + (TotalValuesBefore10[0]));
                Logger.Instance.InfoLog("Total Volume Value Before 10 : " + (TotalValuesBefore10[1]));
                Logger.Instance.InfoLog("LM Score Value After 10 : " + (ScoreValuesAfter10[0]));
                Logger.Instance.InfoLog("LM Volume Value After 10 : " + (ScoreValuesAfter10[1]));
                Logger.Instance.InfoLog("Total Score Value After 10 : " + (TotalValuesAfter10[0]));
                Logger.Instance.InfoLog("Total Volume Value After 10 : " + (TotalValuesAfter10[1]));
                TotalVolumeVerification = TotalValuesAfter10[0].Equals(TotalValuesBefore10[0]);
                TotalScoreVerification = TotalValuesAfter10[1].Equals(TotalValuesBefore10[1]);
                if (RedColorAfter10 == RedColorBefore10 && ((ScoreValuesBefore10[0]) == (ScoreValuesAfter10[0])) && ((ScoreValuesBefore10[1]) == (ScoreValuesAfter10[1])) && TotalVolumeVerification && TotalScoreVerification)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                IList<Double> ScoreValuesBefore11_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesBefore11_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesBefore11_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesBefore11_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesBefore11 = brz3dvp.CalciumScoringTableValues("Total");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Deselect all contiguous");
                int GreenColorBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 111, 0, 255, 0, 2);
                int BlueColorBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 112, 0, 0, 255, 2);
                int GBColorBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 113, 0, 255, 255, 2);
                int YellowColorBefore11 = brz3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 114, 255, 255, 0, 2);
                //LAD
                int[] x11_1 = { CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 + 200, CalciumScoreImage.Size.Width / 2 - 200 };
                int[] y11_1 = { CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 205, CalciumScoreImage.Size.Height / 2 + 305, CalciumScoreImage.Size.Height / 2 + 305 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_1, y11_1);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Deselect all contiguous");
                //PDA
                int[] x11_2 = { (3 * (CalciumScoreImage.Size.Width) / 4), CalciumScoreImage.Size.Width - 10, CalciumScoreImage.Size.Width - 10, (3 * (CalciumScoreImage.Size.Width) / 4) };
                int[] y11_2 = { 3 * (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height / 4), (CalciumScoreImage.Size.Height - 10), (3 * (CalciumScoreImage.Size.Height / 4)) + 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_2, y11_2);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Deselect all contiguous");
                PageLoadWait.WaitForFrameLoad(5);
                int[] x11_3 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Width / 2 };
                int[] y11_3 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_3, y11_3);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "PDA", SelectionOption: "Deselect all contiguous");
                //RCA
                int[] x11_4 = { CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Width / 2 - 200, CalciumScoreImage.Size.Width / 2 };
                int[] y11_4 = { CalciumScoreImage.Size.Height / 2 + 10, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 + 200, CalciumScoreImage.Size.Height / 2 - 10 };
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.drawselectedtool(CalciumScoreImage, x11_4, y11_4);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GBColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 65, 0, 255, 255, 2);
                int GreenColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 66, 0, 255, 0, 2);
                int BlueColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 67, 0, 0, 255, 2);
                int YellowColorAfter11 = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 68, 255, 255, 0, 2);
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter11_1 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> ScoreValuesAfter11_2 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> ScoreValuesAfter11_3 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> ScoreValuesAfter11_4 = brz3dvp.CalciumScoringTableValues("PDA");
                IList<Double> TotalValuesAfter11 = brz3dvp.CalciumScoringTableValues("Total");
                ColorVerification = GBColorAfter11 == GBColorBefore11 && GreenColorAfter11 == GreenColorBefore11 && BlueColorAfter11 == BlueColorBefore11 && YellowColorAfter11 == YellowColorBefore11;
                VolumeVerify1 = ScoreValuesAfter11_1[0] == ScoreValuesBefore11_1[0] && ScoreValuesAfter11_1[1] == ScoreValuesBefore11_1[1];
                VolumeVerify2 = ScoreValuesAfter11_2[0] == ScoreValuesBefore11_2[0] && ScoreValuesAfter11_2[1] == ScoreValuesBefore11_2[1];
                VolumeVerify3 = ScoreValuesAfter11_3[0] == ScoreValuesBefore11_3[0] && ScoreValuesAfter11_3[1] == ScoreValuesBefore11_3[1];
                VolumeVerify4 = ScoreValuesAfter11_4[0] == ScoreValuesBefore11_4[0] && ScoreValuesAfter11_4[1] == ScoreValuesBefore11_4[1];
                TotalVolumeVerify = TotalValuesAfter11[0] == TotalValuesBefore11[0] && TotalValuesAfter11[1] == TotalValuesBefore11[1];
                if (ColorVerification && VolumeVerify1 && VolumeVerify2 && VolumeVerify3 && VolumeVerify4 && TotalVolumeVerify)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                CalciumScoreAnnotationValue = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CalciumScoring);
                if (CalciumScoreAnnotationValue == objlocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163283(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] descriptions = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription")).Split('|');
            string[] requirements = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements")).Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02 & 03
                bool Result = brz3dvp.searchandopenstudyin3D(requirements[1], descriptions[1], layout: BluRingZ3DViewerPage.CalciumScoring, field : requirements[0], thumbimgoptional : descriptions[2]);
                if (!Result)
                {
                    brz3dvp.checkerrormsg("y");
                    PageLoadWait.WaitForFrameLoad(5);
                    IList<IWebElement> tilelist = brz3dvp.controlImage();
                    IWebElement NavigationElement = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    String NavigationAnnotationVal = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                    if (tilelist.Count == 1 && NavigationAnnotationVal.Contains(BluRingZ3DViewerPage.CalciumScoring))
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Lossy compressed study launched without any error");
                        throw new Exception("Lossy compressed study launched without any error");
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Lossy compressed study launched without any error");
                    throw new Exception("Lossy compressed study launched without any error");
                }

                //step 04
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                Result = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step 06
                Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100);
                if(Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step 08
                Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 90);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                Result = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (Result)
                {
                    Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100);
                    if (Result)
                    {
                        Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                        if (Result)
                        {
                            Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100);
                            if (Result)
                                result.steps[++ExecutedSteps].StepPass();
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

                //step 11
                brz3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12 & 13
                Result = brz3dvp.searchandopenstudyin3D(Patientid, descriptions[0], layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("No Lossy compressed study launch failed");
                    throw new Exception("No Lossy compressed study launch failed");
                }

                //step 14
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                Result = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step 16
                Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100,"n");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step 18
                Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100, "n");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                Result = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 90);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                Result = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                if (Result)
                {
                    Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100);
                    if (Result)
                    {
                        Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                        if (Result)
                        {
                            Result = brz3dvp.CheckLossyAnnotation(BluRingZ3DViewerPage.CalciumScoring, 5, 5, 100);
                            if (Result)
                                result.steps[++ExecutedSteps].StepPass();
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
        public TestCaseResult Test_163279(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string Imagedescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string RCAlocation1 = Requirements.Split('|')[0], RCAlocation2 = Requirements.Split('|')[1], RCAlocation3 = Requirements.Split('|')[2], RCAlocation4 = Requirements.Split('|')[3], RCAlocation5 = Requirements.Split('|')[4], RCAlocation6 = Requirements.Split('|')[5];
            String[] RCAlocations = new String[]{ RCAlocation1, RCAlocation2, RCAlocation3, RCAlocation4, RCAlocation5, RCAlocation6 };
            int[] RCAscrollvalues = new int[] { 23, 3, 2, 7, 7, 5 };
            string LADlocation1 = Requirements.Split('|')[6], LADlocation2 = Requirements.Split('|')[7], LADlocation3 = Requirements.Split('|')[8];
            String[] LADlocations = new String[] { LADlocation1, LADlocation2, LADlocation3 };
            int[] LADscrollvalues = new int[] { 2, 3, 6 };
            string CXlocation = Requirements.Split('|')[8];
            string LMlocation = Requirements.Split('|')[9];
            int LMScrollvalue = 2;
            string LMValues = Requirements.Split('|')[10];
            string RCAValues = Requirements.Split('|')[11];
            string LADValues = Requirements.Split('|')[12];
            string CXValues = Requirements.Split('|')[13];
            string TotalValues = Requirements.Split('|')[14];
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, Imagedescription, layout: BluRingZ3DViewerPage.CalciumScoring);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Calcium Scoring Layout");
                    throw new Exception("Failed to open study in Calcium Scoring Layout");
                }

                //step 03 - 05
                int counter = 0, i = 0;
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "RCA", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement calciumscoring = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                for(int x = 0; x < RCAlocations.Length; x++)
                {
                    bool result3 = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrollTill: RCAlocations[x], Thickness: "n");
                    if (result3)
                    {
                        i++;
                        int greencolorebefore3 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_" + i , ExecutedSteps + 31, 0, 255, 0);
                        PageLoadWait.WaitForFrameLoad(5);
                        int[] x3 = { calciumscoring.Size.Width / 2, calciumscoring.Size.Width / 2, calciumscoring.Size.Width / 4, calciumscoring.Size.Width / 4 };
                        int[] y3 = { calciumscoring.Size.Height / 4, calciumscoring.Size.Height / 2, calciumscoring.Size.Height / 2, calciumscoring.Size.Height / 4 };
                        brz3dvp.drawselectedtool(calciumscoring, x3, y3);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                        int greencoloreafter3 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_" + i, ExecutedSteps + 32, 0, 255, 0);
                        if (greencoloreafter3 != greencolorebefore3)
                        {
                            counter++;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter3 = brz3dvp.CalciumScoringTableValues("RCA");
                IList<Double> TotalValuesAfter3 = brz3dvp.CalciumScoringTableValues("Total");

                Logger.Instance.InfoLog("RCA Score Value After 3 : " + (ScoreValuesAfter3[0]));
                Logger.Instance.InfoLog("RCA Volume Value After 3 : " + (ScoreValuesAfter3[1]));
                Logger.Instance.InfoLog("Total Score Value After 3 : " + (TotalValuesAfter3[0]));
                Logger.Instance.InfoLog("Total Volume Value After 3 : " + (TotalValuesAfter3[1]));
                Logger.Instance.InfoLog("Counter value : " + counter.ToString() + "RCAlocations.Length value : " + RCAlocations.Length);
                if (counter.Equals(RCAlocations.Length) && ScoreValuesAfter3[0] == TotalValuesAfter3[0] && ScoreValuesAfter3[1] == TotalValuesAfter3[1])
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 06 - 08
                int counter6 = 0, i6 = 0;
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LAD", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(10);
                calciumscoring = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                for(int y = 0; y < LADlocations.Length; y++)
                {
                    bool result6 = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrollTill: LADlocations[y], Thickness: "n");
                    if (result6)
                    {
                        i6++;
                        int bluecolorebefore6 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_" + i6, ExecutedSteps + 61, 0, 0, 255);
                        PageLoadWait.WaitForFrameLoad(5);
                        int[] x6 = { (3 * (calciumscoring.Size.Width / 4)) - 30, calciumscoring.Size.Width / 2, calciumscoring.Size.Width / 2, (3 * (calciumscoring.Size.Width / 4)) - 30 };
                        int[] y6 = { (calciumscoring.Size.Height / 4) + 20, (calciumscoring.Size.Height / 4) + 20, calciumscoring.Size.Height / 2, calciumscoring.Size.Height / 2 };
                        brz3dvp.drawselectedtool(calciumscoring, x6, y6);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                        int bluecoloreafter6 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_" + i6, ExecutedSteps + 62, 0, 0, 255);
                        if (bluecoloreafter6 != bluecolorebefore6)
                        {
                            counter6++;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter6 = brz3dvp.CalciumScoringTableValues("LAD");
                IList<Double> TotalValuesAfter6 = brz3dvp.CalciumScoringTableValues("Total");
                IList<Double> calculatedscores = new List<Double>() { ScoreValuesAfter6[0] + ScoreValuesAfter3[0], ScoreValuesAfter6[1] + ScoreValuesAfter3[1] };

                Logger.Instance.InfoLog("LAD Score Value After 6 : " + (ScoreValuesAfter6[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 6 : " + (ScoreValuesAfter6[1]));
                Logger.Instance.InfoLog("Total Score Value After 6 : " + (TotalValuesAfter6[0]));
                Logger.Instance.InfoLog("Total Volume Value After 6 : " + (TotalValuesAfter6[1]));
                Logger.Instance.InfoLog("Caluated Score Value After 6 : " + (calculatedscores[0]));
                Logger.Instance.InfoLog("Calculated Volume Value After 6 : " + (calculatedscores[1]));

                if (counter6.Equals(LADlocations.Length) && ScoreValuesAfter6[0] != 0.00 && ScoreValuesAfter6[1] != 0.00 && Convert.ToInt32(TotalValuesAfter6[0]) == Convert.ToInt32(calculatedscores[0]) && Convert.ToInt32(TotalValuesAfter6[1]) == Convert.ToInt32(calculatedscores[1]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 09 - 11
                int counter9 = 0;
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "CX", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(10);
                calciumscoring = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int yellowcolorebefore9 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_1", ExecutedSteps + 91, 255, 255, 0);
                PageLoadWait.WaitForFrameLoad(5);
                int[] x9 = { (calciumscoring.Size.Width / 2) - 10, (calciumscoring.Size.Width / 4) * 3, (calciumscoring.Size.Width / 4) * 3, (calciumscoring.Size.Width / 2) - 10 };
                int[] y9 = { (calciumscoring.Size.Height / 2) + 25, (calciumscoring.Size.Height / 2) + 25, (calciumscoring.Size.Height / 4) * 3, (calciumscoring.Size.Height / 4) * 3 };
                brz3dvp.drawselectedtool(calciumscoring, x9, y9);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int yellowcoloreafter9 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_1", ExecutedSteps + 92, 255, 255, 0);
                if (yellowcoloreafter9 != yellowcolorebefore9)
                {
                    counter9++;
                }
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter9 = brz3dvp.CalciumScoringTableValues("CX");
                IList<Double> TotalValuesAfter9 = brz3dvp.CalciumScoringTableValues("Total");
                calculatedscores = new List<Double>() { calculatedscores[0] + ScoreValuesAfter9[0], calculatedscores[1] + ScoreValuesAfter9[1] };

                Logger.Instance.InfoLog("LAD Score Value After 9 : " + (ScoreValuesAfter9[0]));
                Logger.Instance.InfoLog("LAD Volume Value After 9 : " + (ScoreValuesAfter9[1]));
                Logger.Instance.InfoLog("Total Score Value After 9 : " + (TotalValuesAfter9[0]));
                Logger.Instance.InfoLog("Total Volume Value After 9 : " + (TotalValuesAfter9[1]));
                Logger.Instance.InfoLog("Caluated Score Value After 9 : " + (calculatedscores[0]));
                Logger.Instance.InfoLog("Calculated Volume Value After 9 : " + (calculatedscores[1]));

                if (counter9 > 0 && ScoreValuesAfter9[0] != 0.00 && ScoreValuesAfter9[1] != 0.00 && Convert.ToInt32(TotalValuesAfter9[0]) == Convert.ToInt32(calculatedscores[0]) && Convert.ToInt32(TotalValuesAfter9[1]) == Convert.ToInt32(calculatedscores[1]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 12 - 14
                int counter12 = 0;
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "LM", SelectionOption: "Select all contiguous");
                PageLoadWait.WaitForFrameLoad(10);
                calciumscoring = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                bool result12 = brz3dvp.ScrollInView(BluRingZ3DViewerPage.CalciumScoring, scrollTill: LMlocation, Thickness: "n");
                if (result12)
                {
                    int redcolorebefore12 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_12", ExecutedSteps + 121, 255, 0, 0);
                    PageLoadWait.WaitForFrameLoad(5);
                    int[] x12 = { (calciumscoring.Size.Width / 4) + 30, (calciumscoring.Size.Width / 2) - 10, (calciumscoring.Size.Width / 2) - 10, (calciumscoring.Size.Width / 2) + 30 };
                    int[] y12 = { (calciumscoring.Size.Height / 2) - 20, (calciumscoring.Size.Height / 2) - 20, (calciumscoring.Size.Height / 2) + 100, (calciumscoring.Size.Height / 2) + 100 };
                    brz3dvp.drawselectedtool(calciumscoring, x12, y12);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                    int redcoloreafter12 = brz3dvp.LevelOfSelectedColor(calciumscoring, testid + "_12", ExecutedSteps + 122, 255, 0, 0);
                    if (redcoloreafter12 != redcolorebefore12)
                    {
                        counter12++;
                    }
                }
                brz3dvp.select3DTools(Z3DTools.Calcium_Scoring);
                PageLoadWait.WaitForFrameLoad(5);
                IList<Double> ScoreValuesAfter12 = brz3dvp.CalciumScoringTableValues("LM");
                IList<Double> TotalValuesAfter12 = brz3dvp.CalciumScoringTableValues("Total");
                calculatedscores = new List<Double>() { calculatedscores[0] + ScoreValuesAfter12[0], calculatedscores[1] + ScoreValuesAfter12[1] };

                Logger.Instance.InfoLog("LM Score Value After 12 : " + (ScoreValuesAfter12[0]));
                Logger.Instance.InfoLog("LM Volume Value After 12 : " + (ScoreValuesAfter12[1]));
                Logger.Instance.InfoLog("Total Score Value After 12 : " + (TotalValuesAfter12[0]));
                Logger.Instance.InfoLog("Total Volume Value After 12 : " + (TotalValuesAfter12[1]));
                Logger.Instance.InfoLog("Caluated Score Value After 12 : " + (calculatedscores[0]));
                Logger.Instance.InfoLog("Calculated Volume Value After 12 : " + (calculatedscores[1]));

                if (counter12 > 0 && ScoreValuesAfter12[0] != 0.00 && ScoreValuesAfter12[1] != 0.00 && Convert.ToInt32(TotalValuesAfter12[0]) == Convert.ToInt32(calculatedscores[0]) && Convert.ToInt32(TotalValuesAfter12[1]) == Convert.ToInt32(calculatedscores[1]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 15
                IList<int> LMDifference = new List<int>() { Convert.ToInt32(ScoreValuesAfter12[0]) - Convert.ToInt32(LMValues.Split(',')[0]), Convert.ToInt32(ScoreValuesAfter12[1]) - Convert.ToInt32(LMValues.Split(',')[1]) };
                IList<int> RCADifference = new List<int>() { Convert.ToInt32(ScoreValuesAfter3[0]) - Convert.ToInt32(RCAValues.Split(',')[0]), Convert.ToInt32(ScoreValuesAfter3[1]) - Convert.ToInt32(RCAValues.Split(',')[1]) };
                IList<int> LADifference = new List<int>() { Convert.ToInt32(ScoreValuesAfter6[0]) - Convert.ToInt32(LADValues.Split(',')[0]), Convert.ToInt32(ScoreValuesAfter6[1]) - Convert.ToInt32(LADValues.Split(',')[1]) };
                IList<int> CXDifference = new List<int>() { Convert.ToInt32(ScoreValuesAfter9[0]) - Convert.ToInt32(CXValues.Split(',')[0]), Convert.ToInt32(ScoreValuesAfter9[1]) - Convert.ToInt32(CXValues.Split(',')[1]) };
                IList<int> TotalDifference = new List<int>() { Convert.ToInt32(TotalValuesAfter12[0]) - Convert.ToInt32(TotalValues.Split(',')[0]), Convert.ToInt32(TotalValuesAfter12[1]) - Convert.ToInt32(TotalValues.Split(',')[1]) };

                bool LMVerfication = (LMDifference[0] <= 50 && LMDifference[0] >= -50) && (LMDifference[1] <= 50 && LMDifference[1] >= -50);
                bool RCAVerfication = (RCADifference[0] <= 50 && RCADifference[0] >= -50) && (RCADifference[1] <= 50 && RCADifference[1] >= -50);
                bool LADVerfication = (LADifference[0] <= 50 && LADifference[0] >= -50) && (LADifference[1] <= 50 && LADifference[1] >= -50);
                bool CXVerfication = (CXDifference[0] <= 50 && CXDifference[0] >= -50) && (CXDifference[1] <= 50 && CXDifference[1] >= -50);
                bool TotalVerfication = (TotalDifference[0] <= 50 && TotalDifference[0] >= -50) && (TotalDifference[1] <= 50 && TotalDifference[1] >= -50);
                if (LMVerfication && RCAVerfication && LADVerfication && CXVerfication && TotalVerfication)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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
