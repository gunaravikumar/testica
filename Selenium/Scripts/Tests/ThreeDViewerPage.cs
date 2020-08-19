using System;
using Selenium.Scripts.Pages;
using System.IO;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using Dicom;
using System.Diagnostics;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
using Accord;
using System.Threading;

namespace Selenium.Scripts.Tests
{
    class ThreeDViewerPage : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public ThreeDViewerPage(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163418(String testid, String teststeps, int stepcount)
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
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbnail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String objdcmpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String dcmloc = tempdir + objdcmpath;

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02 & 03
                bool Result = brz3dvp.searchandopenstudyin3D(objpatid, objthumbnail);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Unable to launch the study in MPR View mode");

                //step 04
                IWebElement NavigationElement = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                String NavigationAnnotationVal = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightTop)).GetAttribute("innerHTML");
                String[] new1 = NavigationAnnotationVal.Split(new string[] { "<br>" }, StringSplitOptions.None);
                if (new1.Length > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                String PatientName = BasePage.ReadDicomFile<String>(dcmloc, DicomTag.PatientName);
                String patientid = BasePage.ReadDicomFile<String>(dcmloc, DicomTag.PatientID);
                String patientgender = BasePage.ReadDicomFile<String>(dcmloc, DicomTag.PatientSex);
                if (PatientName != null && patientid != null && patientgender != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                int counter = 0;
                String[] dcmval = new String[] { PatientName.Replace("^", ", "), patientid, patientgender };
                for (int i = 0; i < new1.Length; i++)
                {
                    for (int j = 0; j < dcmval.Length; j++)
                    {
                        if (new1[i].Contains(dcmval[j]))
                        {
                            counter++;
                            break;
                        }
                    }
                }
                if (counter > 0)
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

        public TestCaseResult Test_163411(String testid, String teststeps, int stepcount)
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
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar;
                String z3dexelocation = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

                //step 01
                //ICA and Z3D has been installed and configured already
                result.steps[++ExecutedSteps].StepPass();

                //step 02
                var versionInfo = FileVersionInfo.GetVersionInfo(z3dexelocation);
                string version = versionInfo.ProductVersion;
                Logger.Instance.InfoLog("Version of Z3D build is : " + version);
                if (version != null && version != "")
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
        }

        public TestCaseResult Test_163417(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result = new TestCaseResult(stepcount);
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
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbnail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                IWebElement Navigation1, Navigation2, Navigation3, ResultControl, Control3D1, Control3D2, PathNavigation3D, PathNavigationMPR, ControlCurvedMPR, CalciumScoreImage, ImagePanel;

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(objpatid, objthumbnail);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Unable to launch the study in MPR View mode");

                //step 02
                Navigation1 =  brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).SendKeys("x").Build().Perform();
                Result = brz3dvp.select3DTools(Z3DTools.Window_Level);
                if (Result)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4).ClickAndHold()
                        .MoveToElement(Navigation1, Navigation1.Size.Width / 4, (Navigation1.Size.Height / 4) + 10).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    ImagePanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.NgStarInserted + " " + Locators.CssSelector.CompositeViewer));
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    Result = CompareImage(result.steps[ExecutedSteps], ImagePanel, ImageFormat: "png");
                    if (Result)
                        result.steps[ExecutedSteps].StepPass();
                    else
                        result.steps[ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                ResultControl = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                new Actions(Driver).MoveToElement(ResultControl, ResultControl.Size.Width / 4, ResultControl.Size.Height / 4).ClickAndHold()
                        .MoveToElement(ResultControl, ResultControl.Size.Width / 4, (ResultControl.Size.Height / 4) + 10).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Result = CompareImage(result.steps[ExecutedSteps], ResultControl, ImageFormat: "png");
                if (Result)
                {
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.select3DTools(Z3DTools.Window_Level);
                    Control3D1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    new Actions(Driver).MoveToElement(Control3D1, Control3D1.Size.Width / 4, Control3D1.Size.Height / 4).ClickAndHold()
                        .MoveToElement(Control3D1, Control3D1.Size.Width / 4, 3 * (Control3D1.Size.Height / 4)).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                    if (CompareImage(result.steps[ExecutedSteps], Control3D1, ImageFormat: "png"))
                    {
                        brz3dvp.select3DTools(Z3DTools.Reset);
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.select3DTools(Z3DTools.Window_Level);
                        PageLoadWait.WaitForFrameLoad(5);
                        Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                        new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 4, Navigation2.Size.Height / 4).ClickAndHold()
                            .MoveToElement(Navigation2, Navigation2.Size.Width / 4, 3 * (Navigation2.Size.Height / 4)).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        ResultControl = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                        new Actions(Driver).MoveToElement(ResultControl, ResultControl.Size.Width / 4, ResultControl.Size.Height / 4).ClickAndHold()
                                .MoveToElement(ResultControl, ResultControl.Size.Width / 4, (ResultControl.Size.Height / 4) + 10).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                        ImagePanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.NgStarInserted + " " + Locators.CssSelector.CompositeViewer));
                        if (CompareImage(result.steps[ExecutedSteps], ImagePanel, ImageFormat: "png"))
                        {
                            Result = brz3dvp.ChangeViewMode();
                            if (Result)
                            {
                                brz3dvp.select3DTools(Z3DTools.Reset);
                                PageLoadWait.WaitForFrameLoad(5);
                                brz3dvp.select3DTools(Z3DTools.Window_Level);
                                PageLoadWait.WaitForFrameLoad(5);
                                Control3D1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                                Control3D2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                                new Actions(Driver).MoveToElement(Control3D1, Control3D1.Size.Width / 4, Control3D1.Size.Height / 4).ClickAndHold()
                                        .MoveToElement(Control3D1, Control3D1.Size.Width / 4, 3 * (Control3D1.Size.Height / 4)).Release().Build().Perform();
                                PageLoadWait.WaitForFrameLoad(10);
                                new Actions(Driver).MoveToElement(Control3D2, Control3D2.Size.Width / 4, Control3D2.Size.Height / 4).ClickAndHold()
                                        .MoveToElement(Control3D2, Control3D2.Size.Width / 4, (Control3D2.Size.Height / 4) * 3).Release().Build().Perform();
                                PageLoadWait.WaitForFrameLoad(10);
                                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                                ImagePanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.NgStarInserted + " " + Locators.CssSelector.CompositeViewer));
                                if (CompareImage(result.steps[ExecutedSteps], ImagePanel, ImageFormat: "png"))
                                {
                                    brz3dvp.select3DTools(Z3DTools.Reset);
                                    PageLoadWait.WaitForFrameLoad(5);
                                    brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                                    PageLoadWait.WaitForFrameLoad(5);
                                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                                    PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                                    PathNavigationMPR = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                                    ControlCurvedMPR = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                                    new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 2, (Navigation3.Size.Height / 2) - 10).Click().Build().Perform();
                                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                                    new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 2, (Navigation3.Size.Height / 4) * 3).Click().Build().Perform();
                                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                                    brz3dvp.select3DTools(Z3DTools.Window_Level);
                                    PageLoadWait.WaitForFrameLoad(5);
                                    new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4).ClickAndHold()
                                       .MoveToElement(Navigation3, Navigation3.Size.Width / 4, (Navigation3.Size.Height / 4) - 10).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 4, PathNavigation3D.Size.Height / 4).ClickAndHold()
                                        .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 4, 3 * (PathNavigation3D.Size.Height / 4)).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    new Actions(Driver).MoveToElement(ControlCurvedMPR, ControlCurvedMPR.Size.Width / 4, ControlCurvedMPR.Size.Height / 4).ClickAndHold()
                                        .MoveToElement(ControlCurvedMPR, ControlCurvedMPR.Size.Width / 4, (ControlCurvedMPR.Size.Height / 4) - 10).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    new Actions(Driver).MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, PathNavigationMPR.Size.Height / 4).ClickAndHold()
                                        .MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, (PathNavigationMPR.Size.Height / 4) - 10).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 5);
                                    ImagePanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.NgStarInserted + " " + Locators.CssSelector.CompositeViewer));
                                    if (CompareImage(result.steps[ExecutedSteps], ImagePanel, ImageFormat: "png"))
                                    {
                                        brz3dvp.select3DTools(Z3DTools.Reset);
                                        PageLoadWait.WaitForFrameLoad(5);
                                        brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                                        PageLoadWait.WaitForFrameLoad(5);
                                        brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                                        PageLoadWait.WaitForFrameLoad(5);
                                        CalciumScoreImage = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                                        new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 2 + 10).ClickAndHold()
                                            .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 100, CalciumScoreImage.Size.Height / 2 + 200)
                                            .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 + 100, CalciumScoreImage.Size.Height / 2 + 200)
                                            .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2, CalciumScoreImage.Size.Height / 2 + 200).Release().Build().Perform();
                                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                                        brz3dvp.select3DTools(Z3DTools.Window_Level);
                                        PageLoadWait.WaitForFrameLoad(5);
                                        new Actions(Driver).MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 4, CalciumScoreImage.Size.Height / 4).ClickAndHold()
                                           .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 4, (CalciumScoreImage.Size.Height / 4) - 10).Release().Build().Perform();
                                        PageLoadWait.WaitForFrameLoad(5);
                                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                                        if (CompareImage(result.steps[ExecutedSteps], ImagePanel, ImageFormat: "png"))
                                            result.steps[ExecutedSteps].StepPass();
                                        else
                                            result.steps[ExecutedSteps].StepFail();
                                    }
                                    else
                                        result.steps[ExecutedSteps].StepFail();
                                }
                                else
                                    result.steps[ExecutedSteps].StepFail();
                            }
                            else
                                result.steps[ExecutedSteps].StepFail();
                        }
                        else
                            result.steps[ExecutedSteps].StepFail();
                    }
                    else
                        result.steps[ExecutedSteps].StepFail();
                }
                else
                    result.steps[ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163416(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer brv = new BluRingViewer();
            OnlineHelp onhelp = new OnlineHelp();
            StudyViewer study = new StudyViewer();

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
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbnail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String Requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String title = Requirements.Split('|')[0];
                String chapter = Requirements.Split('|')[1];

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02
                int previousWinCount = Driver.WindowHandles.Count;
                string[] windows = study.OpenHelpandSwitchtoIT(0);
                Thread.Sleep(5000);
                //wait.Until(driver => driver.WindowHandles.Count == (previousWinCount + 1));
                Driver.SwitchTo().Window(windows[1]);
                //wait.Until(driver => driver.Title.Equals("IBM iConnect Access 7.0 Online Help"));
                if (Driver.Title.Equals(title))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                Driver.Manage().Window.Maximize();
                //Driver.SwitchTo().DefaultContent();
                //Driver.SwitchTo().Frame(Driver.FindElement(By.XPath("//frame[@src='whskin_frmset01.htm'][@frameborder='1']")));
                //Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("frame[id='minibar_navpane']")));
                //Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("frame[id='navpane']")));
                ////onhelp.OpenChapter(chapter);
                //Driver.SwitchTo().Frame("tocIFrame");
                //IList<IWebElement> chapters = Driver.FindElements(By.CssSelector("a[id^='B_']"));
                //IWebElement chap = chapters.Where<IWebElement>(a => a.Text.Trim().Equals(chapter)).Last();
                //ClickElement(chap);
                brz3dvp.OpenChapter(chapter);
                Thread.Sleep(3000);
                brz3dvp.switchtotopicframe();
                //onhelp.NavigateToOnlineHelpFrame("topic");
                Thread.Sleep(3000);
                IWebElement uservars = Driver.FindElements(By.CssSelector("p uservariable")).Where<IWebElement>(element => element.Text.Equals("IBM iConnect Access 3D")).First();
                if (uservars != null && uservars.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                IList<IWebElement> paralist = Driver.FindElements(By.CssSelector("p.FM_Bullet"));
                IWebElement ModalityElement = paralist.Where(ele => ele.Text.Contains("modality is MRI, CT, or PET.")).First();
                IWebElement seriesrequired = paralist.Where(ele => ele.Text.Contains("at least 15 images to be displayed in 3D.")).First();
                if (ModalityElement != null && ModalityElement.Displayed && seriesrequired != null && seriesrequired.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                IWebElement warningnote = Driver.FindElement(By.CssSelector("p.FM_Warning"));
                if (warningnote.Text.Contains("make sure that vessel segments are not inadvertently removed as well"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                int counter6 = 0;
                IWebElement presetobj = Driver.FindElements(By.CssSelector("p.FM_CellBody")).Where(ele => ele.Text.Contains("3D presets for color volume rendering")).First();
                IWebElement VolumeMeasureobj = Driver.FindElements(By.CssSelector("p.FM_Note")).Where(ele => ele.Text.Contains("Volume measurements are reported with precision to the hundredth of a mm³")).First();
                IWebElement mprobj = Driver.FindElements(By.CssSelector(Locators.CssSelector.viewingmodecontent)).Where(ele => ele.Text.Contains("MPR Viewing Mode")).First();
                IWebElement calciumscoreobj = Driver.FindElements(By.CssSelector(Locators.CssSelector.viewingmodecontent)).Where(ele => ele.Text.Contains("Calcium Scoring Viewing Mode")).First();
                IWebElement ThreeD4_1obj = Driver.FindElements(By.CssSelector(Locators.CssSelector.viewingmodecontent)).Where(ele => ele.Text.Contains("3D 4:1 Layout Viewing Mode")).First();
                IWebElement ThreeD6_1obj = Driver.FindElements(By.CssSelector(Locators.CssSelector.viewingmodecontent)).Where(ele => ele.Text.Contains("3D 6:1 Layout Viewing Mode")).First();
                IWebElement curvedobj = Driver.FindElements(By.CssSelector(Locators.CssSelector.viewingmodecontent)).Where(ele => ele.Text.Contains("3 MPRs + MPR Path Navigation Control + 3D Path Navigation Control + Curved MPR")).First();
                IWebElement linemeasurementobj = Driver.FindElements(By.CssSelector("p.FM_Note")).Where(ele => ele.Text.Contains("Linear measurements are reported with precision to the tenth of a millimeter")).First();
                IList<IWebElement> checkobj = new List<IWebElement>(){ presetobj, VolumeMeasureobj, mprobj, calciumscoreobj, ThreeD4_1obj, ThreeD6_1obj, curvedobj, linemeasurementobj };
                foreach (IWebElement obj in checkobj)
                {
                    if (!obj.Displayed || obj == null)
                    {
                        Logger.Instance.InfoLog("Value not displayed in the help section");
                        break;
                    }
                    else
                        counter6++;
                }
                if(counter6 == checkobj.Count)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                Driver.Close();
                Thread.Sleep(3000);
                Driver.SwitchTo().Window(windows[0]);
                Thread.Sleep(3000);
                var studiestab = login.Navigate("Studies");
                if (studiestab != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                login.SearchStudy("patient", objpatid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy("Patient ID", objpatid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: "Patient ID", value: objpatid);
                if (viewer != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                viewer.OpenOnlineHelp();
                Thread.Sleep(3000);
                IList<String> windows2 = BasePage.Driver.WindowHandles;
                Thread.Sleep(3000);
                Driver.SwitchTo().Window(windows2[1]);
                Thread.Sleep(3000);
                if (BasePage.Driver.Title.Contains(title))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                //onhelp.OpenChapter(chapter);
                //ClickElement(onhelp.MainChapters().Where<IWebElement>(a => a.GetAttribute("title").Trim().Replace("&nbsp;", " ").Equals(chapter)).Last());
                //Thread.Sleep(3000);
                //onhelp.NavigateToOnlineHelpFrame("topic");
                brz3dvp.OpenChapter(chapter);
                Thread.Sleep(3000);
                brz3dvp.switchtotopicframe();
                Thread.Sleep(3000);
                uservars = Driver.FindElements(By.CssSelector("p uservariable")).Where<IWebElement>(element => element.Text.Equals("IBM iConnect Access 3D")).First();
                if (uservars != null && uservars.Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                Driver.Close();
                Driver.SwitchTo().Window(windows2[0]);
                PageLoadWait.WaitForFrameLoad(5);
                SwitchToDefault();
                Thread.Sleep(3000);
                SwitchToUserHomeFrame();
                Thread.Sleep(3000);
                bool thumbnailselction = brz3dvp.selectthumbnail(objthumbnail);
                if (thumbnailselction)
                {
                    bool Result = brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR, "y");
                    if (Result)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        throw new Exception("Failed while launching study in 3D viewer due to exception ");
                }
                else
                    throw new Exception("Failed while selecting thumbnail due to exception ");

                //step 12
                viewer.OpenOnlineHelp();
                Thread.Sleep(3000);
                IList<String> windows3 = BasePage.Driver.WindowHandles;
                Thread.Sleep(3000);
                Driver.SwitchTo().Window(windows3[1]);
                Thread.Sleep(3000);
                if (BasePage.Driver.Title.Contains(title))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                ////onhelp.OpenChapter(chapter);
                //ClickElement(onhelp.MainChapters().Where<IWebElement>(a => a.GetAttribute("title").Trim().Replace("&nbsp;", " ").Equals(chapter)).Last());
                //Thread.Sleep(3000);
                //onhelp.NavigateToOnlineHelpFrame("topic");
                brz3dvp.OpenChapter(chapter);
                Thread.Sleep(3000);
                brz3dvp.switchtotopicframe();
                Thread.Sleep(3000);
                uservars = Driver.FindElements(By.CssSelector("p uservariable")).Where<IWebElement>(ele => ele.Text.Equals("IBM iConnect Access 3D")).First();
                if (uservars != null && uservars.Displayed)
                {
                    Driver.Close();
                    Thread.Sleep(3000);
                    Driver.SwitchTo().Window(windows3[0]);
                    Thread.Sleep(3000);
                    result.steps[++ExecutedSteps].StepPass();
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
    }
}
