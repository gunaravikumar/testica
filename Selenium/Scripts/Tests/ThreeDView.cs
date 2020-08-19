using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Selenium.Scripts.Pages.eHR;
using Microsoft.Win32;
using Dicom;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using OpenQA.Selenium.Support.UI;
using Accord.Imaging.Filters;
using Accord;
using Accord.Math.Geometry;
using Accord.Imaging;



namespace Selenium.Scripts.Tests
{
    class ThreeDView : BasePage
    {

        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }
        public ThreeDView(String classname)
        {

            login = new Login();

            login.DriverGoTo(login.url);
            domain = new DomainManagement();

            studyviewer = new StudyViewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";

        }
        public TestCaseResult Test_163247(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {

                //STEP 01
                login.LoginIConnect(username, password);
                result.steps[++ExecutedSteps].StepPass("ICA launched successfully.");

                //STEP 02
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass("Series is loaded successfully in the 3D viewer.");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("3D viewer failed to open.");
                }

                //STEP 03 
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step3)
                {
                    result.steps[++ExecutedSteps].StepPass("3D 4:1 view mode is displayed successfully.");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("3D 4:1 view mode failed to displayed.");
                }

                //STEP 04
                List<string> step4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 0);
                List<string> compareStep = new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 };


                if (step4[0].Contains(compareStep[0]) && step4[1].Contains(compareStep[1]) && step4[2].Contains(compareStep[2]) && step4[3].Contains(compareStep[3]))
                {
                    result.steps[++ExecutedSteps].StepPass("3 3D navigation controls and 1 3D control displayed successfully.");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("3D navigation controls failed to displayed.");
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
            }
        }

        public TestCaseResult Test_163246(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {

                //STEP 01
                login.LoginIConnect(username, password);
                result.steps[++ExecutedSteps].StepPass();

                //STEP 02
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 03 
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 04
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigation3D1);
                IWebElement ViewerPort = z3dvp.ViewerContainer();
                String BeforeImagePath = Config.downloadpath + "\\Before" + testid + "_" + ExecutedSteps + ".png";
                String AfterImagePath = Config.downloadpath + "\\After" + testid + "_" + ExecutedSteps + ".png";
                Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(ViewerPort, BeforeImagePath, "png");
                IWebElement iweb3d = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                if (Config.BrowserType.ToLower() == "chrome")
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((iweb3d.Location.X + 130), (iweb3d.Location.Y + 300));
                    for (int i = 0; i < 25; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 60, 0);
                        Thread.Sleep(1000);
                    }
                    PageLoadWait.WaitForFrameLoad(10);
                    DownloadImageFile(ViewerPort, AfterImagePath, "png");
                    if (!CompareImage(AfterImagePath, BeforeImagePath))
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
                    
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((iweb3d.Location.X + 130), (iweb3d.Location.Y + 300));
                    for (int i = 0; i < 140; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 60, 0);
                        Thread.Sleep(1000);
                    }
                    PageLoadWait.WaitForFrameLoad(10);
                    List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                    string[] arrsplit4 = result4[3].Split(',');
                    if (Convert.ToDouble(arrsplit4[1].Trim()) >= 90)
                    {
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
               
                //STEP 05
                bool step5 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                if (step5)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 06
                BeforeImagePath = Config.downloadpath + "\\Before" + testid + "_" + ExecutedSteps + ".png";
                AfterImagePath = Config.downloadpath + "\\After" + testid + "_" + ExecutedSteps + ".png";
                Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(ViewerPort, BeforeImagePath, "png");
                IWebElement Navigation3D = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                new Actions(Driver).MoveToElement(Navigation3D, Navigation3D.Size.Width / 4, Navigation3D.Size.Height / 2).Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.Performdragdrop(Navigation3D, 3 * (Navigation3D.Size.Width / 4), Navigation3D.Size.Height / 2, Navigation3D.Size.Width / 4, Navigation3D.Size.Height / 2);
                DownloadImageFile(ViewerPort, AfterImagePath, "png");
                if (!CompareImage(AfterImagePath, BeforeImagePath))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 07
                BeforeImagePath = Config.downloadpath + "\\Before" + testid + "_" + ExecutedSteps + ".png";
                AfterImagePath = Config.downloadpath + "\\After" + testid + "_" + ExecutedSteps + ".png";
                Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(Navigation3D, BeforeImagePath, "png");
                System.Drawing.Point Navigation3d1 = z3dvp.ControllerPoints(BluRingZ3DViewerPage.Navigation3D1);
                if (Config.BrowserType.ToLower() == "chrome")
                {
                    Cursor.Position = new System.Drawing.Point(Navigation3d1.X - 10, Navigation3d1.Y - 10);
                }
                else
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((iweb3d.Location.X + 300), (iweb3d.Location.Y  + 300));
                }
              
                //Left and right mouse button click
                BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                if (Config.BrowserType.ToLower() == "chrome")
                {
                    Thread.Sleep(5000);
                    Cursor.Position = new System.Drawing.Point(Navigation3d1.X - 10, Navigation3d1.Y + 70);
                }
                else
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((iweb3d.Location.X + 300), (iweb3d.Location.Y  + 300));
                }
               
                bool Step7 = z3dvp.VerifyToolSelected(BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.WindowLevel);
                //Left and right mouse button release
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                if(Step7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163245(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
               
                //STEP 01
                login.LoginIConnect(username, password);
                result.steps[++ExecutedSteps].StepPass();

                //STEP 02
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 03 
                bool step3 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 04
                IWebElement NavigationOne = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement NavigationTwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement NavigationThree = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement Navigation3D = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);

                Accord.Point bluePosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 1, "blue", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point bluePosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 2, "blue", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                z3dvp.PerformDragAndDropWithDelay(NavigationOne, NavigationOne.Size.Width / 4, NavigationOne.Size.Height / 4, (Int32)bluePosition1.X, (Int32)bluePosition1.Y, NavigationOne.Size.Width / 2, (Int32)bluePosition1.Y, 15);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragBluePosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 3, "blue", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragBluePosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 4, "blue", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                if (!((Int32)bluePosition1.X).Equals((Int32)afterDragBluePosition1.X) && !((Int32)bluePosition3.X).Equals((Int32)afterDragBluePosition3.X))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 05
                z3dvp.PerformDragAndDropWithDelay(NavigationThree, NavigationThree.Size.Width / 4, NavigationThree.Size.Height / 4, (Int32)afterDragBluePosition3.X, (Int32)afterDragBluePosition3.Y, (Int32)bluePosition3.X, (Int32)bluePosition3.Y, 15);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragBluePosition_1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 52, "blue", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragBluePosition_3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 53, "blue", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                if (!((Int32)afterDragBluePosition1.X).Equals((Int32)afterDragBluePosition_1.X) && !((Int32)afterDragBluePosition3.X).Equals((Int32)afterDragBluePosition_3.X))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 06
                Thread.Sleep(5000);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Accord.Point redPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 62, "red", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point redPosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 63, "red", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).MoveToElement(NavigationOne, (Int32)redPosition1.X, (Int32)redPosition1.Y).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(NavigationOne, (Int32)redPosition1.X, (Int32)redPosition1.Y).ClickAndHold()
                    .MoveToElement(NavigationOne, (Int32)redPosition1.X, NavigationOne.Size.Height / 2).Build().Perform();
                //z3dvp.PerformDragAndDropWithDelay(NavigationOne, NavigationOne.Size.Width / 4, NavigationOne.Size.Height / 4, (Int32)redPosition1.X, (Int32)redPosition1.Y, (Int32)redPosition1.X, NavigationOne.Size.Height / 2, 25);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragRedPosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 64, "red", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point afterDragRedPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 65, "red", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).SendKeys("T").Build().Perform();
                if (!((Int32)redPosition2.Y).Equals((Int32)afterDragRedPosition2.Y) && !((Int32)redPosition1.Y).Equals((Int32)afterDragRedPosition1.Y))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 07
                z3dvp.PerformDragAndDropWithDelay(NavigationTwo, NavigationTwo.Size.Width / 4, NavigationTwo.Size.Height / 4, (Int32)afterDragRedPosition2.X, (Int32)afterDragRedPosition2.Y, (Int32)redPosition2.X, (Int32)redPosition2.Y, 25);
                PageLoadWait.WaitForFrameLoad(15);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Accord.Point afterDragRedPosition_1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 71, "red", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point afterDragRedPosition_2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 72, "red", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).SendKeys("T").Build().Perform();
                if (!((Int32)afterDragRedPosition1.Y).Equals((Int32)afterDragRedPosition_1.Y) && !((Int32)afterDragRedPosition2.Y).Equals((Int32)afterDragRedPosition_2.Y))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 08
                Accord.Point R_bluePosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 81, "blue", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point R_bluePosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 82, "blue", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(15);
                z3dvp.PerformDragAndDropWithDelay(NavigationOne, NavigationOne.Size.Width / 4, NavigationOne.Size.Height / 4, (Int32)R_bluePosition1.X, (Int32)R_bluePosition1.Y, NavigationOne.Size.Width / 2, (Int32)R_bluePosition1.Y, 15);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDrag_R_BluePosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 83, "blue", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDrag_R_BluePosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 84, "blue", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(15);
                if (!((Int32)R_bluePosition1.X).Equals((Int32)afterDrag_R_BluePosition1.X) && !((Int32)R_bluePosition3.X).Equals((Int32)afterDrag_R_BluePosition3.X))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 09
                z3dvp.PerformDragAndDropWithDelay(NavigationThree, NavigationThree.Size.Width / 4, NavigationThree.Size.Height / 4, (Int32)afterDrag_R_BluePosition3.X, (Int32)afterDrag_R_BluePosition3.Y, (Int32)R_bluePosition3.X, (Int32)R_bluePosition3.Y, 15);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDrag_R_BluePosition_1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 92, "blue", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDrag_R_BluePosition_3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 93, "blue", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(15);
                if (!((Int32)afterDrag_R_BluePosition1.X).Equals((Int32)afterDrag_R_BluePosition_1.X) && !((Int32)afterDrag_R_BluePosition3.X).Equals((Int32)afterDrag_R_BluePosition_3.X))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 10
                new Actions(Driver).SendKeys("T").Build().Perform();
                Accord.Point B_redPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 101, "red", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point B_redPosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 102, "red", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                z3dvp.PerformDragAndDropWithDelay(NavigationOne, NavigationOne.Size.Width / 4, NavigationOne.Size.Height / 4, (Int32)B_redPosition1.X, (Int32)B_redPosition1.Y, (Int32)B_redPosition1.X, NavigationOne.Size.Height / 2, 25);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point afterDrag_B_RedPosition1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 103, "red", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point afterDrag_B_RedPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 104, "red", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).SendKeys("T").Build().Perform();
                if (!((Int32)B_redPosition2.Y).Equals((Int32)afterDrag_B_RedPosition2.Y) && !((Int32)B_redPosition1.Y).Equals((Int32)afterDrag_B_RedPosition1.Y))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 11
                z3dvp.PerformDragAndDropWithDelay(NavigationTwo, NavigationTwo.Size.Width / 4, NavigationTwo.Size.Height / 4, (Int32)afterDrag_B_RedPosition2.X, (Int32)afterDrag_B_RedPosition2.Y, (Int32)B_redPosition2.X, (Int32)B_redPosition2.Y, 25);
                PageLoadWait.WaitForFrameLoad(15);
                new Actions(Driver).SendKeys("T").Build().Perform();
                Accord.Point afterDrag_B_RedPosition_1 = z3dvp.GetIntersectionPoints(NavigationOne, testid, ExecutedSteps + 111, "red", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point afterDrag_B_RedPosition_2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 112, "red", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).SendKeys("T").Build().Perform();
                if (!((Int32)afterDrag_B_RedPosition1.Y).Equals((Int32)afterDrag_B_RedPosition_1.Y) && !((Int32)afterDrag_B_RedPosition2.Y).Equals((Int32)afterDrag_B_RedPosition_2.Y))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                //Thread.Sleep(5000);
                //STEP 12
                if (Config.BrowserType.ToLower() == "Internet Explorer")
                {
                    IWebElement Threedview = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((Threedview.Location.X + 250), (Threedview.Location.Y + 150));
                    for (int i = 0; i <= 5; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 7, 0);
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigation3D1, scrolllevel: 3, Thickness: "N"); // To get correct Intersection point in Navigation3 Horizontal upper clipline
                }
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1)).Build().Perform();
                Thread.Sleep(3000);
                Accord.Point yellowPosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 121, "yellow", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point yellowPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 122, "yellow", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                new Actions(Driver).MoveToElement(NavigationTwo, NavigationTwo.Size.Width / 4, NavigationTwo.Size.Height / 4).Build().Perform();
                Thread.Sleep(3000);
                z3dvp.PerformDragAndDropWithDelay(NavigationTwo, NavigationTwo.Size.Width / 4, NavigationTwo.Size.Height / 4, (Int32)yellowPosition2.X, (Int32)yellowPosition2.Y, NavigationTwo.Size.Width * 3 / 4, (Int32)yellowPosition2.Y, 15);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragYellowPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 123, "yellow", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragYellowPosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 124, "yellow", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(15);
                if (!((Int32)yellowPosition2.X).Equals((Int32)afterDragYellowPosition2.X) && !((Int32)yellowPosition3.Y).Equals((Int32)afterDragYellowPosition3.Y))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 13
                //new Actions(Driver).MoveToElement(NavigationThree, NavigationThree.Size.Width / 4, NavigationThree.Size.Height / 4).Build().Perform();
                Thread.Sleep(2000);
                z3dvp.PerformDragAndDropWithDelay(NavigationThree, NavigationThree.Size.Width / 4, NavigationThree.Size.Height / 4, (Int32)afterDragYellowPosition3.X, (Int32)afterDragYellowPosition3.Y, (Int32)afterDragYellowPosition3.X, (Int32)yellowPosition3.Y, 15);
                Accord.Point afterDragYellowPosition_2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 131, "yellow", "vertical", 16);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDragYellowPosition_3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 132, "yellow", "Horizontal", 16);
                PageLoadWait.WaitForFrameLoad(15);
                if (!afterDragYellowPosition2.X.Equals(afterDragYellowPosition_2.X) && !afterDragYellowPosition3.Y.Equals(afterDragYellowPosition_3.Y))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 14
                Accord.Point B_yellowPosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 141, "yellow", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point R_yellowPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 142, "yellow", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(20);
                z3dvp.PerformDragAndDropWithDelay(NavigationTwo, NavigationTwo.Size.Width / 4, NavigationTwo.Size.Height / 4, (Int32)R_yellowPosition2.X, (Int32)R_yellowPosition2.Y, NavigationTwo.Size.Width / 4, (Int32)R_yellowPosition2.Y, 20);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDrag_R_YellowPosition2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 143, "yellow", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(20);
                Accord.Point afterDrag_B_YellowPosition3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 144, "yellow", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(20);
                if (!((Int32)R_yellowPosition2.X).Equals((Int32)afterDrag_R_YellowPosition2.X) && !((Int32)B_yellowPosition3.Y).Equals((Int32)afterDrag_B_YellowPosition3.Y))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 15
                z3dvp.PerformDragAndDropWithDelay(NavigationThree, NavigationThree.Size.Width / 4, NavigationThree.Size.Height / 4, (Int32)afterDrag_B_YellowPosition3.X, (Int32)afterDrag_B_YellowPosition3.Y, (Int32)B_yellowPosition3.X, (Int32)B_yellowPosition3.Y, 20);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDrag_R_YellowPosition_2 = z3dvp.GetIntersectionPoints(NavigationTwo, testid, ExecutedSteps + 151, "yellow", "vertical", 17);
                PageLoadWait.WaitForFrameLoad(15);
                Accord.Point afterDrag_B_YellowPosition_3 = z3dvp.GetIntersectionPoints(NavigationThree, testid, ExecutedSteps + 152, "yellow", "Horizontal", 61);
                PageLoadWait.WaitForFrameLoad(15);
                if (!((Int32)afterDrag_R_YellowPosition2.X).Equals((Int32)afterDrag_R_YellowPosition_2.X) && !((Int32)afterDrag_B_YellowPosition3.Y).Equals((Int32)afterDrag_B_YellowPosition_3.Y))
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

        public TestCaseResult Test_163244(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            
            try
            {

                //STEP 01
                login.LoginIConnect(username, password);
                bool step1 = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (step1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    throw new Exception("Unable to open study in 3d layout 163244");
                }

                //STEP 02 
                bool step2 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 03
                new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement ViewerContainer = z3dvp.ViewerContainer();
                int i = 0;
                Dictionary<string, List<String>> map = new Dictionary<string, List<String>>();
                map.Add("R", new List<string>() { "P", "H", "A" });
                map.Add("L", new List<string>() { "A", "H", "P" });
                map.Add("H", new List<string>() { "L", "A", "R" });
                map.Add("F", new List<string>() { "R", "A", "L" });
                map.Add("A", new List<string>() { "R", "H", "L" });
                map.Add("P", new List<string>() { "L", "H", "R" });

                new Actions(Driver).SendKeys(map.ElementAt(i).Key).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool OrientationKey = z3dvp.CheckOrientationMarkers(map.ElementAt(i).Value[0], map.ElementAt(i).Value[1], map.ElementAt(i).Value[2]);
                i++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, removeCurserFromPage:true,pixelTolerance:200) && OrientationKey)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //STEP 04
                new Actions(Driver).SendKeys(map.ElementAt(i).Key).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                OrientationKey = z3dvp.CheckOrientationMarkers(map.ElementAt(i).Value[0], map.ElementAt(i).Value[1], map.ElementAt(i).Value[2]);
                i++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, removeCurserFromPage: true) && OrientationKey)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //STEP 05
                new Actions(Driver).SendKeys(map.ElementAt(i).Key).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                OrientationKey = z3dvp.CheckOrientationMarkers(map.ElementAt(i).Value[0], map.ElementAt(i).Value[1], map.ElementAt(i).Value[2]);
                i++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, removeCurserFromPage: true) && OrientationKey)
                {

                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //STEP 06
                new Actions(Driver).SendKeys(map.ElementAt(i).Key).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                OrientationKey = z3dvp.CheckOrientationMarkers(map.ElementAt(i).Value[0], map.ElementAt(i).Value[1], map.ElementAt(i).Value[2]);
                i++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, removeCurserFromPage: true) && OrientationKey)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //STEP 07
                new Actions(Driver).SendKeys(map.ElementAt(i).Key).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                OrientationKey = z3dvp.CheckOrientationMarkers(map.ElementAt(i).Value[0], map.ElementAt(i).Value[1], map.ElementAt(i).Value[2]);
                i++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, removeCurserFromPage: true,pixelTolerance:200) && OrientationKey)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //STEP 08
                new Actions(Driver).SendKeys(map.ElementAt(i).Key).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                OrientationKey = z3dvp.CheckOrientationMarkers(map.ElementAt(i).Value[0], map.ElementAt(i).Value[1], map.ElementAt(i).Value[2]);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, removeCurserFromPage: true, pixelTolerance:50) && OrientationKey)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //STEP 09
                bool step9 = z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigation3D1);
                if (step9)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 10
                bool step10 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step10)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 11
                bool step11 = z3dvp.ChangeViewMode(BluRingZ3DViewerPage.Navigation3D1);
                if (step11)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //STEP 12
                ViewerContainer = z3dvp.ViewerContainer();
                ExecutedSteps = ++ExecutedSteps;
                int PassCount = 0;
                foreach (KeyValuePair<string, List<string>> kvp in map)
                {
                    new Actions(Driver).SendKeys(kvp.Key).Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    bool Orientation = z3dvp.CheckOrientationMarkers(kvp.Value[0], kvp.Value[1], kvp.Value[2], BluRingZ3DViewerPage.Navigation3D1);
                    bool Orientation1 = z3dvp.CheckOrientationMarkers(kvp.Value[0], kvp.Value[1], kvp.Value[2], BluRingZ3DViewerPage.Navigation3D2);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, PassCount);
                    if (CompareImage(result.steps[ExecutedSteps], ViewerContainer,removeCurserFromPage: true,pixelTolerance:200) && OrientationKey && Orientation1)
                    {
                        PassCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (map.Count.Equals(PassCount))
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
    }
}
