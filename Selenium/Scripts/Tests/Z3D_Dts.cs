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

namespace Selenium.Scripts.Tests
{
    class Z3D_Dts : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());

        public Z3D_Dts(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163326(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string Thumbnaildetails = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string FiveImages = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //string thumbnaildesc = Thumbnaildetails.Split('|')[0];
            //string thumbnailoption = Thumbnaildetails.Split('|')[1];

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step 1 :: From the Universal viewer , Select a 3D supported series that has few images missing the orientation and Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, Thumbnaildetails, BluRingZ3DViewerPage.MPR);
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to open study");

                //Step2 :: Verify the sub volumes section under the hover bar.
                //IList<String> step2_Options = z3dvp.getAllMenuItems(BluRingZ3DViewerPage.Navigationone);
                z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForPageLoad(10);
                IList<IWebElement> SubVolume = Driver.FindElements(By.CssSelector(Locators.CssSelector.SubVolumebutton));
                ClickElement(SubVolume[0]);
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.subMenulayout)));
                IList<IWebElement> SubOptions = Driver.FindElements(By.CssSelector("div[class^='subvolumeDiscardedMessage ']"));
                if (SubOptions[0].Text.Contains(FiveImages))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IWebElement ViewPort = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));

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

        public TestCaseResult Test_163327(string testid, String teststeps, int stepcount)
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
                //Step 1 :: Log in to iCA and navigate to studies tab.
                //Step 2 :: Search and load the study in the universal viewer. Study: "THREED , ALIENHEAD".
                //Step 3 :: Select the series 3 (BONE) from iCA thumbnail bar and Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step4 :: Verify the loaded series.
                IWebElement ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ResultPanel, removeCurserFromPage:true))
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

        public TestCaseResult Test_163328(string testid, String teststeps, int stepcount)
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
                //Step 1 :: From the Universal viewer , Select a 3D supported series and Select the MPR layout option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
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
                //Step2 :: Select the freehand cut tool
                new Actions(Driver).SendKeys("X").Build().Perform();
                bool FreeHandCutTool = z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Freehand);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> dialogsList = z3dvp.ToolBarDialogs();
                PageLoadWait.WaitForFrameLoad(10);
                if (FreeHandCutTool && dialogsList[0].Text.Equals(BluRingZ3DViewerPage.SculptToolFreehanddialog))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolFreehanddialog, "Close"); 

                //Step 3 :: Make a cut on MPR navigation control 1.
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                String browserName = Driver.GetType().Name.ToString().ToLowerInvariant();
                int blackcolorebefore = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 31, 255, 255, 255, isMoveCursor: true);
                //if (browserName.Contains("chrome"))
                //{
                    new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2 - 100).ClickAndHold().
                       MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 100, Navigation1.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                //}
                //else
                //{
                //    new TestCompleteAction().MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2 - 100).ClickAndHold().
                //    MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 100, Navigation1.Size.Height / 2).Release().Perform();
                //    PageLoadWait.WaitForFrameLoad(5);
                //}
                
                //Verification :: Cut should displayed on the image in MPR navigation control 1.
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int blackcoloreafter = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 32, 255, 255, 255, isMoveCursor: true);
                if (blackcoloreafter != blackcolorebefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step4 :: Select Tissue Selection Tool button from the 3D tool box. NOTE: apply bone preset.
                bool BonePreset = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Bone, "Preset");
                PageLoadWait.WaitForFrameLoad(10);
                bool TissueSelection = z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                dialogsList = z3dvp.ToolBarDialogs();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume = TissueSelectionVolume.Text;
                String[] VolumeValue = SelectionVolume.Split(' ');
                if (BonePreset && TissueSelection  && dialogsList[0].Text.Equals(BluRingZ3DViewerPage.SelectionTooldialog))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                //Step 5 :: Click on near the edge of the cut region
                //z3dvp.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, (Navigation1.Size.Height / 2) - 3);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, (Navigation1.Size.Height / 2) - 3).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                //Verification :: Selection should go into the cut region.
                TissueSelection = z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string SelectionVolume1 = TissueSelectionVolume.Text;
                String[] VolumeValue1 = SelectionVolume1.Split(' ');
                if (Convert.ToDouble(VolumeValue1[0]) > Convert.ToDouble(VolumeValue[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step6 :: Select the Polygon cut tool and Make a cut on MPR navigation control 2.
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.select3DTools(Z3DTools.Reset);
                bool PolygonCutTool = z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Close");
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                browserName = Driver.GetType().Name.ToString().ToLowerInvariant();
                int blackbefore6 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 61, 255, 255, 255, isMoveCursor: true);
                //if (browserName.Contains("chrome"))
                //{
                    new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2 - 100).Click().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2 + 100, Navigation2.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2 - 100).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                //}
                //else
                //{
                //    new TestCompleteAction().MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2 - 100).Click().
                //    MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2).Click().
                //    MoveToElement(Navigation2, Navigation2.Size.Width / 2 + 100, Navigation2.Size.Height / 2).Click().
                //    MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 2 - 100).Click().Perform();
                //    PageLoadWait.WaitForFrameLoad(5);
                //}
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int blackafter6 =  z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 62, 255, 255, 255, isMoveCursor: true);
                if (blackbefore6 != blackafter6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step7 :: Select Tissue Selection Tool from the 3D tool box. click into the cut region.
                //Issue  fixed ============================JIRA ID :: ICA-17995 ======================
                BonePreset = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Bone, "Preset");
                TissueSelection = z3dvp.select3DTools(Z3DTools.Selection_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string Before = TissueSelectionVolume.Text;
                String[] BeforeVolume = Before.Split(' ');
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 3, (Navigation2.Size.Height / 2) - 3);
                new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) +3, (Navigation2.Size.Height / 2) - 3).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                TissueSelectionVolume = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume));
                string After = TissueSelectionVolume.Text;
                String[] AfterVolume = After.Split(' ');
                if (Convert.ToDouble(BeforeVolume[0]) == Convert.ToDouble(AfterVolume[0]))
                {
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed <--" + result.steps[ExecutedSteps].description);
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

    }
}
