using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Accord;
using Accord.Math.Geometry;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System.Text.RegularExpressions;
using Selenium.Scripts.Reusable.Generic;
using TestComplete;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Tests
{
    class CurvedMPR3DPath : BasePage
    {
        public string filepath { get; set; }
        //  Login login;
        public Cursor Cursor { get; private set; }
        public Login login { get; set; }
        public CurvedMPR3DPath(String classname)
        {
            //   this.login = new Login();
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163291(String testid, String teststeps, int stepcount) // Test_124691Colon Mode: Scroll images in 3D path navigation control
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String objlocval = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            IWebElement Navigation1, Navigation2;
            try
            {
                //step 01 & 02
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 03
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(5);
                if (!res)
                    throw new Exception("Failed while selecting Auto Colon Tool");
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 04
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval);
                bool check4 = brz3dvp.checkerrormsg();
                if (res && check4 == false)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to find aorta region");

                //step 05
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColorValBefore_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 251, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 4), 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = brz3dvp.checkerrormsg();
                int ColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 252, 0, 0, 255, 2);
                if (ColorValAfter_4 != ColorValBefore_4 && check5 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 06
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 4) + 30, 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check6 = brz3dvp.checkerrormsg();
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation), ImageFormat: "png");
                if (res && check6 == false)
                    result.steps[ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 07
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Actions action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 4) + 30, 30);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_6 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 71, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 60, 40);
                PageLoadWait.WaitForFrameLoad(10);
                bool check7 = brz3dvp.checkerrormsg();
                int ColorValAfter_6 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 72, 0, 0, 255, 2);
                if (ColorValAfter_6 != ColorValBefore_6 && check7 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 08
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement _3DNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                bool scrollorientatienresult = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 21, zoom: false);
                String OrientationVal3DPath = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String OrientationValMPRPath = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (scrollorientatienresult && OrientationVal3DPath.Equals(OrientationValMPRPath) && res)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159015 Step 8");
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 09
                String Orientation3DBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String OrientationMPRBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                System.Drawing.Point location = brz3dvp.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                String Orientation3DAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String OrientationMPRAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (!Orientation3DAfter.Equals(Orientation3DBefore) && !OrientationMPRBefore.Equals(OrientationMPRAfter))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159015 Step 9");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10
                brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 5, ScrollDirection: "down", Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                String Orientation3DAfter_09 = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String OrientationMPRAfter_09 = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                IWebElement viewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.wholepanel));
                if (Orientation3DAfter_09.Equals(OrientationMPRAfter_09) && !Orientation3DAfter_09.Equals(Orientation3DAfter) && !OrientationMPRAfter_09.Equals(OrientationMPRAfter) && CompareImage(result.steps[ExecutedSteps], viewport))
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159015 Step 10");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11
                brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 5, Thickness: "n");
                IWebElement mprpath = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                brz3dvp.EnterThickness(BluRingZ3DViewerPage.MPRPathNavigation,"100");
                Thread.Sleep(2000);
                ClickElement(mprpath);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (res)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159015 Step 11");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12
                String OrientationVal3DBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String OrientationValMPRBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 5, ScrollDirection: "down", Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                String Orientation3DAfter_12 = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String OrientationMPRAfter_12 = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.wholepanel));
                if (!Orientation3DAfter_12.Equals(OrientationVal3DBefore) && !OrientationMPRAfter_12.Equals(OrientationValMPRBefore) && CompareImage(result.steps[ExecutedSteps], viewport))
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159015 Step 10");
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

        public TestCaseResult Test_163303(String testid, String teststeps, int stepcount)// 124703- Thickness for MPR path navigation controls
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String patientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbImg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");

            try
            {
                //step1
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //step2
                bool step2 = Z3dViewer.searchandopenstudyin3D(patientId, thumbImg, "Curved MPR");
                if (step2)
                {

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to open study");

                //step3
                Z3dViewer.select3DTools(Z3DTools.Pan, "MPR Path Navigation");
                var EleMPRPathNavigation = Z3dViewer.controlelement("MPR Path Navigation");
                bool step3 = Z3dViewer.EnableOneViewupMode(EleMPRPathNavigation);
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
                //Step4
                Z3dViewer.EnterThickness("MPR Path Navigation", "6");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "body > div:nth-child(3) > blu-ring-root > div > blu-ring-study-viewer > form > div > div.studyPanelsContainer > blu-ring-study-panel-container > div > blu-ring-study-panel-control > div > div.compositeViewerContainer > div > blu-ring-z3d-composite-viewer > div > div:nth-child(7) > blu-ring-viewer-host-component > div > blu-ring-viewer3d > div > div.tilepanel.unselectable > div > blu-ring-imagetile > div > div > div.fill.unselectable"));
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


        }

        public TestCaseResult Test_163294(String testid, String teststeps, int stepcount) // 124694- Orientation markers updates on Scrolling
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            String requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String thumbImg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {

                //step1
                login.LoginIConnect(adminUserName, adminPassword);
                bool search = Z3dViewer.searchandopenstudyin3D(requirements.Split('|')[1], thumbImg, layout: BluRingZ3DViewerPage.CurvedMPR, field: requirements.Split('|')[0]);
                if (search)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to open study");

                //Step2::Use the curve drawing tool to create a path on the MPR navigation controls :
                String LocationBeforepath = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(2000);
                String browserName = Driver.GetType().Name.ToString();
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, 30);
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, 30).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height - 30);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 - 25, Navigation1.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, 30, Navigation1.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, Navigation1.Size.Width - 30, (Navigation1.Size.Height / 2) - 3);
                PageLoadWait.WaitForFrameLoad(10);
                bool check2 = Z3dViewer.checkerrormsg();
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterpath = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //Verification::Path should be generated.
                if (LocationBeforepath != LocationAfterpath && check2 == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to find path");
                //Step3::Scroll through the path displayed in the MPR path navigation control. Verify that the orientation markers that are displayed on the top, left and right of the control are being updated correctly..
                Z3dViewer.EnterThickness(BluRingZ3DViewerPage.Navigationone, "100.0");
                String CentrTopAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                string[] BOrientationTopCentre = CentrTopAnnotationNav.Split('\r');
                String CentreleftAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftMiddle)).Text;
                string[] BOrientationleftcentre = CentreleftAnnotationNav.Split('\r');
                String CentreRightAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                string[] BOrientationRightcentre = CentreRightAnnotationNav.Split('\r');
                Z3dViewer.EnterThickness(BluRingZ3DViewerPage.MPRPathNavigation, "100.0");
                Thread.Sleep(5000);
                IWebElement MPRPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, ScrollDirection: "down", scrolllevel: 2 , Thickness:"n");
                Thread.Sleep(5000);
                CentrTopAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                string[] AOrientationTopCentre = CentrTopAnnotationNav.Split('\r');
                CentreleftAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftMiddle)).Text;
                string[] AOrientationleftcentre = CentreleftAnnotationNav.Split('\r');
                CentreRightAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                string[] AOrientationRightcentre = CentreRightAnnotationNav.Split('\r');
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement Viewer = Z3dViewer.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], Viewer) &&
                    BOrientationTopCentre[0].Equals("H") && BOrientationleftcentre[0].Equals("A") && BOrientationRightcentre[0].Equals("P") &&
                    AOrientationTopCentre[0].Equals("A") && AOrientationleftcentre[0].Equals("R") && AOrientationRightcentre[0].Equals("L"))
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
                //Step4::Scroll through the path displayed in the MPR path navigation control. Verify that the orientation markers that are displayed on the top, left and right of the control are being updated correctly..
                CentrTopAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                BOrientationTopCentre = CentrTopAnnotationNav.Split('\r');
                CentreleftAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftMiddle)).Text;
                BOrientationleftcentre = CentreleftAnnotationNav.Split('\r');
                CentreRightAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                BOrientationRightcentre = CentreRightAnnotationNav.Split('\r');
                Thread.Sleep(5000);
                IWebElement _3DPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel:1 , Thickness:"n");
                Thread.Sleep(5000);
                CentrTopAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                AOrientationTopCentre = CentrTopAnnotationNav.Split('\r');
                CentreleftAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftMiddle)).Text;
                AOrientationleftcentre = CentreleftAnnotationNav.Split('\r');
                CentreRightAnnotationNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation).FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                AOrientationRightcentre = CentreRightAnnotationNav.Split('\r');
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Viewer = Z3dViewer.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], Viewer) &&
                    BOrientationTopCentre[0].Equals("A") && BOrientationleftcentre[0].Equals("R") && BOrientationRightcentre[0].Equals("L") &&
                    AOrientationTopCentre[0].Equals("H") && AOrientationleftcentre[0].Equals("A") && AOrientationRightcentre[0].Equals("P"))
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
                Z3dViewer.CloseViewer();
                login.Logout();
            }


        }

        public TestCaseResult Test_163296(String testid, String teststeps, int stepcount) // Test 163296 - Rotate tool for path navigation controls
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Nav1Location = objTestRequirement.Split('|')[0];
            String NavLocation1 = objTestRequirement.Split('|')[1];
            String NavLocation11 = objTestRequirement.Split('|')[2];
            String NavLocation3 = objTestRequirement.Split('|')[3];
            String NavLocation33 = objTestRequirement.Split('|')[4];
            String NavLocation60 = objTestRequirement.Split('|')[5];


            //String Nav1Location = "Loc: 0.0, 34.0, 0.0 mm";
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From iCA, Load a study in the 3D viewer 1.Navigate to 3D tab.
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step2::Select the "Curved MPR viewing mode" from 3D dropdown.
                bool step2_1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                IList<IWebElement> Viewport = Z3dViewer.Viewport();
                if (step2_1 && Viewport.Count == 6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to open study");

                //step3:: Click on navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                bool step3 = Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (step3 && LocationAfterScroll != InitialLocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 4::Add a point at the top of the aorta displayed on navigation control 1.
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool res4_1 = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                bool check4 = Z3dViewer.checkerrormsg();
                int ColorValAfter_4 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValAfter_4 != ColorValBefore && check4 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //Steps 5::Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = Z3dViewer.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if (ColorValAfter_4 != ColorValAfter_5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 6::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_6 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check6 = Z3dViewer.checkerrormsg();
                if (check6)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_6 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                if (ColorValAfter_6 != ColorValBefore_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //Steps 7::Click on the 3D path navigation control and scroll along part of the path that was generated.	
                string BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement _3DPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(3000);
                Z3dViewer.checkerrormsg("ÿ");
                Thread.Sleep(2000);
                string AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 8::Note the orientation markers.Select the rotate tool from the 3D toolbox and perform a 180 degrees Image plane rotation going clockwise on the 3D path navigation control    
                bool res8 = Z3dViewer.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage._3DPathNavigation);
                String BeforeOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.Performdragdrop(ThreeDPathNav, ((ThreeDPathNav.Size.Width / 4) * 3) + 30, ThreeDPathNav.Size.Height / 2, (ThreeDPathNav.Size.Width / 4) - 30, ThreeDPathNav.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                String AfterOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Logger.Instance.InfoLog("Step8 BeforeOrientationValue :" + BeforeOrientationValue + " AfterOrientationValue : " + AfterOrientationValue);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                IWebElement wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 9::Perform a 360 degrees Image plane rotation going counter clockwise but this time on the MPR path navigation control.	
                String Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(MprPathNav, (MprPathNav.Size.Width / 4) - 30, MprPathNav.Size.Height / 2).ClickAndHold()
                            .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, ((MprPathNav.Size.Height / 4) * 3) - 30)
                            .MoveToElement(MprPathNav, ((MprPathNav.Size.Width / 4) * 3) - 30, MprPathNav.Size.Height / 2)
                            .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30)
                            .MoveToElement(MprPathNav, (MprPathNav.Size.Width / 4) - 30, MprPathNav.Size.Height / 2).Release().Build().Perform();
                String After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Logger.Instance.InfoLog("Step 9 Before_3dpathnav : " + Before_3dpathnav + " Before_MprPathNav : " + Before_MprPathNav + " After_3dpathnav : " + After_3dpathnav + " After_MprPathNav : " + After_MprPathNav);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 10::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement MPRPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, scrolllevel: 70, Thickness: "n");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 11::Note the orientation markers.Select the rotate tool from the 3D toolbox and perform a 180 degrees Image plane rotation going clockwise on the MPR path navigation control
                Z3dViewer.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage._3DPathNavigation);
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width - 10, MprPathNav.Size.Height / 2, MprPathNav.Size.Width / 6, MprPathNav.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Logger.Instance.InfoLog("Step11 Before_3dpathnav : " + Before_3dpathnav + " Before_MprPathNav : " + Before_MprPathNav + "After_3dpathnav : " + After_3dpathnav + " After_MprPathNav : " + After_MprPathNav);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 12::Perform a 360 degrees Image plane rotation going counter clockwise but this time on the 3D path navigation control.
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav, (ThreeDPathNav.Size.Width / 4) - 30, ThreeDPathNav.Size.Height / 2).ClickAndHold()
                            .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, ((ThreeDPathNav.Size.Height / 4) * 3) - 30)
                            .MoveToElement(ThreeDPathNav, ((ThreeDPathNav.Size.Width / 4) * 3) - 30, ThreeDPathNav.Size.Height / 2)
                            .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 4) - 30)
                            .MoveToElement(ThreeDPathNav, (ThreeDPathNav.Size.Width / 4) - 30, ThreeDPathNav.Size.Height / 2).Release().Build().Perform();
                Thread.Sleep(5000);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Logger.Instance.InfoLog("Step 12 Before_3dpathnav : " + Before_3dpathnav + " Before_MprPathNav : " + Before_MprPathNav + " After_MprPathNav : " + After_MprPathNav + " After_3dpathnav : " + After_3dpathnav);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 13::Click the reset button from 3D toolbox..
                string BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                string AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
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
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163295(String testid, String teststeps, int stepcount)//Test 163295 - Keyboard Shortcut - 'R'
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String TestData = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Nav1Location = TestData.Split('|')[0];
            String Nav2Location = TestData.Split('|')[1];
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From iCA, Load a study in the 3D viewer 1.Navigate to 3D tab.
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step2::Select the "Curved MPR viewing mode" from 3D dropdown.
                bool step2_1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                IList<IWebElement> Viewport = Z3dViewer.Viewport();
                if (step2_1 && Viewport.Count == 6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Error while loading study");

                //step3:: Click on navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool res1 = Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                if (res1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step4::Add a point at the top of the aorta displayed on navigation control 1.
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                int BlueColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool check15 = Z3dViewer.checkerrormsg();
                if (check15)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_4 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2);
                if (BlueColorValAfter_4 != BlueColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_163295 Step 4");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 05
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = Z3dViewer.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_5 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (BlueColorValAfter_5 != BlueColorValAfter_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_163295 Step 5");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 06
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Actions action = new Actions(Driver);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_6 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check6 = Z3dViewer.checkerrormsg();
                if (check6)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_6 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValAfter_6 != ColorValBefore_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_163295 Step 6");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 07
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                Z3dViewer.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);

                Accord.Point redposition = Z3dViewer.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 1, "red");
                Thread.Sleep(5000);
                Accord.Point yellowposition = Z3dViewer.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 2, "yellow", "vertical", 1);
                Thread.Sleep(5000);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).ClickAndHold()
                        .MoveToElement(Navigation2, (Int32)yellowposition.X, (Int32)yellowposition.Y).Release().Build().Perform();
                else
                    new TestCompleteAction().PerformDraganddrop(Navigation2, (Int32)redposition.X, (Int32)redposition.Y, (Int32)yellowposition.X, (Int32)yellowposition.Y);
                Thread.Sleep(5000);
                IWebElement Navigation3 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                int BColorValBefore_7 = Z3dViewer.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 25, (Navigation3.Size.Height / 2) + Navigation3.Size.Height / 4);
                PageLoadWait.WaitForFrameLoad(10);
                bool check7 = Z3dViewer.checkerrormsg();
                if (check7)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int BColorValAfter_7 = Z3dViewer.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                bool res = CompareImage(result.steps[ExecutedSteps], Z3dViewer.ViewerContainer(), ImageFormat: "png");
                if (BColorValAfter_7 != BColorValBefore_7 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_163295 Step 7");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 8::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement _3DPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection:"down" , scrolllevel:7, Thickness:"n");
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                string AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                if (CompareImage(result.steps[ExecutedSteps], Navigation2) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 9::Note the orientation markers. Press "R" key on the keyboard a few times.
                String FirstOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                String SecondOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                String ThirdOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //Verification::View direction of the MPR and 3D path navigation controls is flipped from forward to backward and vice versa.
                if (FirstOrientationValue != SecondOrientationValue && ThirdOrientationValue == FirstOrientationValue)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement MPRPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, scrolllevel: 7, Thickness: "n");
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                if (CompareImage(result.steps[ExecutedSteps], Navigation2) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 11::Note the orientation markers. Press "R" key on the keyboard a few times.
                FirstOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement MPRPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                SecondOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                ThirdOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //Verification::View direction of the MPR and 3D path navigation controls is flipped from forward to backward and vice versa.
                if (FirstOrientationValue != SecondOrientationValue && ThirdOrientationValue == FirstOrientationValue)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 12::Click the reset button.
                string BeforeNav1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                string AfterNav1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (BeforeNav1 != AfterNav1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 13::Click on the curve drawing tool button from 3D toolbox and select "Auto Vessels" from the drop down list.
                bool CurveDrawingAutoVessel = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //Verification::Curve drawing cursor shows up while hovering over the images.
                if (CurveDrawingAutoVessel)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 14::Click on navigation control 1 and scroll up until the top of the aorta is visible.Note: The aorta is the large vessel that goes down the spine and branches into two sides.
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                bool res14 = Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                if (res14)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 15::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                int BlueColorValBefore15 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool check151 = Z3dViewer.checkerrormsg();
                if (check151)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_15 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2);
                if (BlueColorValAfter_15 != BlueColorValBefore15)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_163295 Step 15");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 16
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check16 = Z3dViewer.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_16 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (BlueColorValAfter_16 != BlueColorValAfter_15 && res == true)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_163295 Step 16");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 17
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_17 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check17 = Z3dViewer.checkerrormsg();
                if (check17)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_17 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                res = CompareImage(result.steps[ExecutedSteps], Z3dViewer.ViewerContainer(), ImageFormat: "png");
                if (ColorValAfter_17 != ColorValBefore_17 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_163295 Step 17");
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 18::Click on the 3D path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                _3DPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 80 , Thickness:"n");
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 19::Note the orientation markers.Press "R" key on the keyboard a few times.
                FirstOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                SecondOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                ThirdOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //Verification::View direction of the MPR and 3D path navigation controls is flipped from forward to backward and vice versa.
                if (FirstOrientationValue != SecondOrientationValue && ThirdOrientationValue == FirstOrientationValue)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 20::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                MPRPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation,  scrolllevel: 80, Thickness: "n");
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 21::Note the orientation markers.Press "R" key on the keyboard a few times.
                FirstOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                MPRPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                SecondOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                ThirdOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //Verification::View direction of the MPR and 3D path navigation controls is flipped from forward to backward and vice versa.
                if (FirstOrientationValue != SecondOrientationValue && ThirdOrientationValue == FirstOrientationValue)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 22::Click the reset button.
                BeforeNav1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String Nav1before = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2before = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3before = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                AfterNav1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String Nav1after = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2after = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3after = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                if (BeforeNav1 != AfterNav1 && Nav1before != Nav1after && Nav2before != Nav2after && Nav3before != Nav3after)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 23::Click on the curve drawing tool button on the toolbar and select "Auto Colon" from the drop down list.
                bool CurvedCursor = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationtwo);
                if (CurvedCursor)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 24::Click on navigation control 1 and scroll up until the top of the colon is visible.
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                bool res24 = Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav2Location);
                if (res24)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 25::Add a point at the beginning of the colon displayed on navigation control 1.
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                //new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 251, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91);
                PageLoadWait.WaitForFrameLoad(10);
                bool check25 = Z3dViewer.checkerrormsg();
                if (check25)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_25 = Z3dViewer.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 252, 0, 0, 255, 2);
                if (ColorValAfter_25 != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps26::Add a 2nd point along the Colon displayed on navigation control 1.
                Thread.Sleep(5000);
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                bool check26 = Z3dViewer.checkerrormsg();
                if (check26)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_26 = Z3dViewer.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                IWebElement ThreeDControls = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                if (CompareImage(result.steps[ExecutedSteps], ThreeDControls, pixelTolerance: 10) && ColorValAfter_26 > ColorValBefore)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 27::Add the 3rd point on the Colon below the 2nd point by clicking on “Navigation 2” image this time.
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                Z3dViewer.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 271, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 33, (Navigation2.Size.Height / 4) * 3 + 35);
                PageLoadWait.WaitForFrameLoad(10);
                bool check27 = Z3dViewer.checkerrormsg();
                if (check27)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_27 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 272, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_27 != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 28::Click on the 3D path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                _3DPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation, "1");
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 60 , Thickness:"n");
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], navigation1, pixelTolerance: 10) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 29::Note the orientation markers. Press "R" key on the keyboard a few times.
                FirstOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                SecondOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                ThirdOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //Verification::View direction of the MPR and 3D path navigation controls is flipped from forward to backward and vice versa.
                if (FirstOrientationValue != SecondOrientationValue && ThirdOrientationValue == FirstOrientationValue)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 30::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                MPRPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation, "1");
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation,  scrolllevel: 60, Thickness: "n");
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 31::Note the orientation markers. Press "R" key on the keyboard a few times.
                FirstOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                MPRPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                SecondOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNav).SendKeys("R").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Z3dViewer.checkerrormsg("y");
                Thread.Sleep(2000);
                ThirdOrientationValue = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                //Verification::View direction of the MPR and 3D path navigation controls is flipped from forward to backward and vice versa.
                if (FirstOrientationValue != SecondOrientationValue && ThirdOrientationValue == FirstOrientationValue)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 32::Click the reset button.
                BeforeNav1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                AfterNav1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (AfterNav1 != BeforeNav1)
                {
                    result.steps[++ExecutedSteps].StepPass();
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
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163299(String testid, String teststeps, int stepcount) //Test 163299 - Zoom function for MPR path navigation control
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            //String adminUserName = "3dTest";//Config.adminUserName;
            //String adminPassword = "3dTest";// Config.adminPassword;
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Nav1Location = objTestRequirement.Split('|')[0];
            String NavLocation1 = objTestRequirement.Split('|')[1];
            String NavLocation3 = objTestRequirement.Split('|')[2];
            String NavLocation60 = objTestRequirement.Split('|')[3];
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From iCA, Load a study in the Z3D viewer..
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                //Verification::MPR 4:1 viewing mode should be displayed by default.
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("Failed to open study");
                }
                //step2:: Click on navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == Nav1Location && LocationAfterScroll != InitialLocation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).ClickAndHold().
                        MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                //=====================================================********============================================================
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 31, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check3 = Z3dViewer.checkerrormsg();
                if (check3)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_3 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 32, 0, 0, 255, 2);
                if (ColorValAfter_3 != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check4 = Z3dViewer.checkerrormsg();
                if (check4)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_4 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_3 != ColorValAfter_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_5 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 61, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = Z3dViewer.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 71, 0, 0, 255, 2);
                if (ColorValAfter_5 != ColorValBefore_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Click on the MPR path navigation control and scroll along part of the path that was generated.	
                string BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, NavLocation1, ScrollDirection: "down");
                PageLoadWait.WaitForFrameLoad(10);
                string AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
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
                //Steps 7::Select the zoom tool from the Z3D toolbar..
                bool ZoomTool = Z3dViewer.select3DTools(Z3DTools.Interactive_Zoom);
                //Verification::Zoom cursor shows up while hovering over the image.
                bool ZoomCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.InteractiveZoomCursor);
                if (ZoomTool && ZoomCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 8::Click and hold the left mouse button over the MPR path navigation control, click and move the mouse downword.
                IWebElement MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement ViewerContainer = Z3dViewer.ViewerContainer();
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
                //Steps 9::Click and hold the left mouse button over the MPR path navigation control, click and move the mouse upword.
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) - 15, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2));
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
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

                //Steps 10::Click the reset button from 3D toolbox..
                string BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                bool RestetTool = Z3dViewer.select3DTools(Z3DTools.Reset);
                string AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (RestetTool && BeforeNavigationLocVal != AfterNavigationLocVal)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 11::Click on the curve drawing tool button on the toolbar and select "Auto Vessels" from the drop down list.
                bool AutoVissel = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //Verification::Curve drawing cursor shows up while hovering over the images.
                bool CurvedCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CurvedToolVesselsCursor);
                if (CurvedCursor && AutoVissel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step12:: Click on navigation control 1 and scroll up until the top of the aorta is visible.
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                PageLoadWait.WaitForFrameLoad(10);
                LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == Nav1Location && LocationAfterScroll != InitialLocation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 13::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).ClickAndHold().
                         MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //=====================================================********============================================================
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 131, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check13 = Z3dViewer.checkerrormsg();
                if (check13)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_13 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 132, 0, 0, 255, 2);
                if (ColorValAfter_13 != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 14::Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check14 = Z3dViewer.checkerrormsg();
                if (check14)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_14 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 141, 0, 0, 255, 2);
                if (ColorValAfter_13 != ColorValAfter_14)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 15::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_15 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 156, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check15 = Z3dViewer.checkerrormsg();
                if (check15)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_15 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 157, 0, 0, 255, 2);
                if (ColorValAfter_15 != ColorValBefore_15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 16::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, NavLocation1, ScrollDirection: "down");
                PageLoadWait.WaitForFrameLoad(10);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1, pixelTolerance: 7) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
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
                //Steps 17::Select the zoom tool from the Z3D toolbar..
                ZoomTool = Z3dViewer.select3DTools(Z3DTools.Interactive_Zoom);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::Zoom cursor shows up while hovering over the image.
                ZoomCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.InteractiveZoomCursor);
                if (ZoomTool && ZoomCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 18::Click and hold the left mouse button over the MPR path navigation control, click and move the mouse downward.
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30);
                //new Actions(Driver).MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30).ClickAndHold()
                //    .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], MprPathNav, pixelTolerance: 10))
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
                //Steps 19::Click and hold the left mouse button over the MPR path navigation control, click and move the mouse upward.
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2), MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150);
                //new Actions(Driver).MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150).ClickAndHold().
                //    MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], MprPathNav, pixelTolerance: 10))
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
                //Steps 20::Click the reset button from the toolbar.
                BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                RestetTool = Z3dViewer.select3DTools(Z3DTools.Reset);
                AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (RestetTool && BeforeNavigationLocVal != AfterNavigationLocVal)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 21::Click on the curve drawing tool button on the toolbar and select "Auto Colon" from the drop down list.
                bool CurveDrawingAutoColon = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                //Verification::Curve drawing cursor shows up while hovering over the images.
                if (CurveDrawingAutoColon)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 22::Click on navigation control 1 and scroll up until the top of the colon is visible.
                //new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation60);
                Thread.Sleep(2000);
                LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == NavLocation60 && LocationAfterScroll != InitialLocation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 23::Add a point at the beginning of the colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 231, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91);
                PageLoadWait.WaitForFrameLoad(10);
                bool check23 = Z3dViewer.checkerrormsg();
                if (check23)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_23 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 232, 0, 0, 255, 2);
                if (ColorValAfter_23 > ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 24::Add a 2nd point along the Colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                bool check24 = Z3dViewer.checkerrormsg();
                if (check24)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_24 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                IWebElement ThreeDControls = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                if (CompareImage(result.steps[ExecutedSteps], ThreeDControls) && ColorValAfter_24 > ColorValBefore)
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

                //Steps 25::Add the 3rd point on the Colon below the 2nd point by clicking on “Navigation 2” image this time.
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Z3dViewer.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);//Config.MPRPathNavigation
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 251, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2 + 33, Navigation2.Size.Height / 2 + 131).Click().Build().Perform();
                Z3dViewer.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 33, Navigation2.Size.Height / 2 + 131);
                PageLoadWait.WaitForFrameLoad(10);
                bool check25 = Z3dViewer.checkerrormsg();
                if (check25)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_25 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 252, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_25 > ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 26::Click on the Mpr path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, NavLocation3, ScrollDirection: "down");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1, pixelTolerance: 5) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
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
                //Steps 27::Select the zoom tool from the Z3D toolbar.
                ZoomTool = Z3dViewer.select3DTools(Z3DTools.Interactive_Zoom);
                //Verification::Zoom cursor shows up while hovering over the image.
                ZoomCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.InteractiveZoomCursor);
                if (ZoomTool && ZoomCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 28::Click and hold the left mouse button over the MPR path navigation control, click and move the mouse downward.
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30);
                //new Actions(Driver).MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30).ClickAndHold()
                //    .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
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
                //Steps 29::Click and hold the left mouse button over the MPR path navigation control, click and move the mouse upword.
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2), MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150);
                new Actions(Driver).MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2) + 150).ClickAndHold()
                    .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 2)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
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
                //Steps 30::Click the reset button from the toolbar.
                BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                RestetTool = Z3dViewer.select3DTools(Z3DTools.Reset);
                AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (RestetTool && BeforeNavigationLocVal != AfterNavigationLocVal)
                {
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
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163300(String testid, String teststeps, int stepcount) //Test 163300 - Zoom function for 3D path navigation control
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            //String adminUserName = "3dTest";//Config.adminUserName;
            //String adminPassword = "3dTest";// Config.adminPassword;
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Nav1Location = objTestRequirement.Split('|')[0];
            String NavLocation1 = objTestRequirement.Split('|')[1];
            String NavLocation3 = objTestRequirement.Split('|')[2];
            String NavLocation60 = objTestRequirement.Split('|')[3];

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From iCA, Load a study in the Z3D viewer.
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                //Verification::MPR 4:1 viewing mode should be displayed by default.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                bool CurvedToolCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CurvedToolManualCursor);
                if (step1 && CurvedToolCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("Unable to load study successfully");
                }
                //Steps 2::Click on navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                //String bodyText = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone).Text;
                PageLoadWait.WaitForFrameLoad(10);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                bool res2 = Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (res2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).ClickAndHold().
                        MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4)).Release().Build().Perform();
                Logger.Instance.InfoLog("Pan tool applied in navigation 1 at co ordinates (x1,y1) & (x2,y2) : " + " (" + ((Navigation1.Size.Width / 2) + 18).ToString() + " , " + ((Navigation1.Size.Height / 4) - 40).ToString() + ")" + " and (" + ((Navigation1.Size.Width / 2) + 18).ToString() + " , " + ((Navigation1.Size.Height / 4) - 40).ToString() + ")");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                //=====================================================********============================================================
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 31, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check3 = Z3dViewer.checkerrormsg();
                if (check3)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_3 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 32, 0, 0, 255, 2);
                if (ColorValAfter_3 != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 4::Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check4 = Z3dViewer.checkerrormsg();
                if (check4)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_4 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_3 != ColorValAfter_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 5::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_5 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = Z3dViewer.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 52, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_5 != ColorValBefore_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 50, Thickness: "n");
                Thread.Sleep(2000);
                string AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
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
                Z3dViewer.checkerrormsg("y");

                //Steps 7::Select the zoom tool from the Z3D toolbar.
                //new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                bool ZoomTool = Z3dViewer.select3DTools(Z3DTools.Interactive_Zoom);
                //Verification::Zoom cursor shows up while hovering over the image.
                bool ZoomCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.InteractiveZoomCursor);
                if (ZoomTool && ZoomCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 8::Click and hold the left mouse button over the 3D path navigation control, click and move the mouse downword.
                IWebElement ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.Performdragdrop(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 4) - 30);
                //new Actions(Driver).MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 4) - 30).ClickAndHold()
                //    .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement ViewerContainer = Z3dViewer.ViewerContainer();
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

                //Steps 9::Click and hold the left mouse button over the MPR path navigation control, click and move the mouse upword.
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.Performdragdrop(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) - 15, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150);
                //new Actions(Driver).MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150).ClickAndHold()
                //    .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) - 15).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
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
                //Steps 10::Click the reset button from 3D toolbox..
                string BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                string AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 11::Click on the curve drawing tool button on the toolbar and select "Auto Vessels" from the drop down list.
                bool AutoVissel = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //Verification::Curve drawing cursor shows up while hovering over the images.
                bool CurvedCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.CurvedToolVesselsCursor);
                if (CurvedCursor && AutoVissel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 12::Click on navigation control 1 and scroll up until the top of the aorta is visible.
                navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                PageLoadWait.WaitForFrameLoad(10);
                LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == Nav1Location && LocationAfterScroll != InitialLocation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 13::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).ClickAndHold().
                        MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4)).Release().Build().Perform();
                Logger.Instance.InfoLog("Pan tool applied in navigation 1 at co ordinates (x1,y1) & (x2,y2) : " + " (" + ((Navigation1.Size.Width / 2) + 18).ToString() + " , " + ((Navigation1.Size.Height / 4) - 40).ToString() + ")" + " and (" + ((Navigation1.Size.Width / 2) + 18).ToString() + " , " + ((Navigation1.Size.Height / 4) - 40).ToString() + ")");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //=====================================================********============================================================
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 131, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check13 = Z3dViewer.checkerrormsg();
                if (check13)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_13 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 132, 0, 0, 255, 2);
                if (ColorValAfter_13 != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 14::Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check14 = Z3dViewer.checkerrormsg();
                if (check14)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_14 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 141, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_13 != ColorValAfter_14)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 15::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_15 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 151, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check15 = Z3dViewer.checkerrormsg();
                if (check15)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_15 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 152, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_15 != ColorValBefore_15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 16::Click on the 3D path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 50, ScrollDirection: "down", Thickness: "n");
                Thread.Sleep(2000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1, pixelTolerance: 10) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
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
                //Steps 17::Select the zoom tool from the Z3D toolbar..
                ZoomTool = Z3dViewer.select3DTools(Z3DTools.Interactive_Zoom);
                //Verification::Zoom cursor shows up while hovering over the image.
                ZoomCursor = Z3dViewer.VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.InteractiveZoomCursor);
                if (ZoomTool && ZoomCursor)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 18::Click and hold the left mouse button over the 3D path navigation control, click and move the mouse downward.
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.Performdragdrop(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 4) - 30);
                //new Actions(Driver).MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 4) - 30).ClickAndHold()
                //    .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ThreeDPathNav))
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
                //Steps 19::Click and hold the left mouse button over the 3D path navigation control, click and move the mouse upward.
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.Performdragdrop(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2), ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150);
                //new Actions(Driver).MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2) + 150).ClickAndHold()
                //    .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 2)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ViewerContainer = Z3dViewer.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ThreeDPathNav))
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
                //Steps 20::Click the reset button from the toolbar.
                BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
                {
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
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163302(String testid, String teststeps, int stepcount) //Test 163302 - 3D image's colorization on 3D path navigation control
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String Nav1Location = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From the Universal viewer , Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                //Verification::Series is loaded in the 3D viewer in Curved MPR viewing mode.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Unable to load study successfully");

                //Steps 2::Click on navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement NavLocation = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location, scrolllevel: 34, Thickness: "y");
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Top of the Aorta should be visible.
                if (LocationAfterScroll == Nav1Location && LocationAfterScroll != InitialLocation)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).ClickAndHold().
                        MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                //=====================================================********============================================================
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 31, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30);
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool check3 = Z3dViewer.checkerrormsg();
                if (check3)
                    throw new Exception("Failed to find path");
                int ColorValAfter_3 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 32, 0, 0, 255, 2);
                if (ColorValAfter_3 != ColorValBefore)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step4:: Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check4 = Z3dViewer.checkerrormsg();
                if (check4)
                    throw new Exception("Failed to find path");
                int ColorValAfter_4 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                if (ColorValAfter_3 != ColorValAfter_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_5 = Z3dViewer.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = Z3dViewer.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = Z3dViewer.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 52, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_5 != ColorValBefore_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Select the window/level tool from the 3D tool box.
                bool WindowLevel = Z3dViewer.select3DTools(Z3DTools.Window_Level);
                if (WindowLevel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 7::Left click and drag the mouse on the image displayed on the 3D path navigation control.
                IWebElement ThreeDpathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Actions steps7 = new Actions(Driver);
                Z3dViewer.Performdragdrop(ThreeDpathNav, (ThreeDpathNav.Size.Width / 2), (ThreeDpathNav.Size.Height / 2) - 100, (ThreeDpathNav.Size.Width / 2), (ThreeDpathNav.Size.Height / 2));
                steps7.MoveToElement(ThreeDpathNav, (ThreeDpathNav.Size.Width / 2), (ThreeDpathNav.Size.Height / 2)).ClickAndHold()
                    .MoveToElement(ThreeDpathNav, (ThreeDpathNav.Size.Width / 2), (ThreeDpathNav.Size.Height / 2) - 100).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                steps7.Release().Build().Perform();
                //Verification::Colorization of the 3D image displayed on the 3D path navigation control is adjusted.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ThreeDpathNav))
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
                //Steps 8 & 9::From the viewport top bar options, Select the preset list.
                String[] presets = new String[] { BluRingZ3DViewerPage.Preset1,BluRingZ3DViewerPage.Preset2,BluRingZ3DViewerPage.Preset3,BluRingZ3DViewerPage.Preset4,BluRingZ3DViewerPage.Preset5,
                    BluRingZ3DViewerPage.Preset6,BluRingZ3DViewerPage.Preset7,BluRingZ3DViewerPage.Preset8,BluRingZ3DViewerPage.Preset9,BluRingZ3DViewerPage.Preset10,BluRingZ3DViewerPage.Preset11,
                    BluRingZ3DViewerPage.Preset12,BluRingZ3DViewerPage.Preset13,BluRingZ3DViewerPage.Preset14,BluRingZ3DViewerPage.Preset15,BluRingZ3DViewerPage.Preset16,BluRingZ3DViewerPage.Preset17,
                    BluRingZ3DViewerPage.Preset18,BluRingZ3DViewerPage.Preset21 };
                int counter = 0;
                foreach (String preset in presets)
                {
                    bool presetresult = Z3dViewer.VerifyPresetWLandImage(BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.Preset1);
                    if (presetresult)
                        counter++;
                }
                if(counter > 0)
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
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163304(String testid, String teststeps, int stepcount) //Test 163304 - W/L presets for MPR path navigation control  
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();

            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String Nav1Location = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From the Universal viewer , Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                //Verification::Series is loaded in the 3D viewer in Curved MPR viewing mode.
                if (step1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Unable to load study successfully");

                //Steps 2::Click on MPR navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement NavLocation = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location, scrolllevel: 34, Thickness: "y", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location, scrolllevel: 34, Thickness: "y");
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Verification:: Top of the Aorta should be visible.
                if (LocationAfterScroll == Nav1Location && LocationAfterScroll != InitialLocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 3::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).ClickAndHold().
                        MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                //=====================================================********============================================================
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 011, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                //Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool check3 = Z3dViewer.checkerrormsg();
                if (check3)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_3 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 012, 0, 0, 255, 2);
                if (ColorValAfter_3 != ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step4:: Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check24 = Z3dViewer.checkerrormsg();
                if (check24)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_4 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 041, 0, 0, 255, 2);
                if (ColorValAfter_3 != ColorValAfter_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step5::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_5 = Z3dViewer.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 051, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = Z3dViewer.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2) + 22, (navigation2.Size.Height / 4) + 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = Z3dViewer.LevelOfSelectedColor(navigation2, testid, ExecutedSteps + 052, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_5 != ColorValBefore_5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 6::Click on the MPR path navigation control and scroll along part of the path that was generated.
                string BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 32.8, 34.0, 225.8 mm", ScrollDirection: "down", scrolllevel: 50, Thickness: "n", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 32.8, 34.0, 225.8 mm", ScrollDirection: "down", scrolllevel: 50, Thickness: "n");
                string AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 7:: From the viewport top bar options on MPR path navigation control ,select the preset drop field.
                String[] rendertypes = new String[] { BluRingZ3DViewerPage.Abdomen,BluRingZ3DViewerPage.Bone,
                                        BluRingZ3DViewerPage.BoneBody,BluRingZ3DViewerPage.Brain,BluRingZ3DViewerPage.Bronchial,
                                        BluRingZ3DViewerPage.Liver, BluRingZ3DViewerPage.Lung,BluRingZ3DViewerPage.Mediastinum,
                                        BluRingZ3DViewerPage.PFossa};
                bool res = false;
                int itr = 0;
                foreach (String rendertype in rendertypes)
                {
                    res = Z3dViewer.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, rendertype, "Preset");
                    Thread.Sleep(2000);
                    if (res == true)
                        itr++;
                    else
                        break;
                }
                if (itr > 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Steps 9::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 25.7, 31.4, 168.6 mm", scrolllevel: 70, Thickness: "n", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 25.7, 31.4, 168.6 mm", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(60000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 10::Select the reset button from the 3D tool box.
                string BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                string AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 11::Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down
                bool AutoVissels = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //Verification::Curve drawing cursor appears while hovering over the images.
                if (AutoVissels)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Steps 12::Click on navigation control 1 and scroll up until the top of the aorta is visible.
                NavLocation = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location, scrolllevel: 34, Thickness: "y", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location, scrolllevel: 34, Thickness: "y");
                PageLoadWait.WaitForFrameLoad(10);
                LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Verification:: Top of the Aorta should be visible.
                if (LocationAfterScroll == Nav1Location && LocationAfterScroll != InitialLocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 13::Add a point at the top of the aorta displayed on navigation control 1.
                //===============================================Changing Image Location=================================================
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 40).ClickAndHold().
                        MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                //=====================================================********============================================================
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 131, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30).Click().Build().Perform();
                //Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check13 = Z3dViewer.checkerrormsg();
                if (check13)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_13 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 132, 0, 0, 255, 2);
                if (ColorValAfter_13 != ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 14::Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check14 = Z3dViewer.checkerrormsg();
                if (check14)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_14 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 141, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_14 != ColorValAfter_13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 15::dd the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) - 10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 151, 0, 0, 255, 2);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check15 = Z3dViewer.checkerrormsg();
                if (check15)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_15 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 152, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_15 != ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 16::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 32.8, 34.0, 225.8 mm", ScrollDirection: "down", scrolllevel: 70, Thickness: "n", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 32.8, 34.0, 225.8 mm", ScrollDirection: "down", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(60000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 17::From the viewport top bar options on MPR path navigation control ,select the preset drop field.
                rendertypes = new String[] { BluRingZ3DViewerPage.Abdomen,BluRingZ3DViewerPage.Bone,
                                        BluRingZ3DViewerPage.BoneBody,BluRingZ3DViewerPage.Brain,BluRingZ3DViewerPage.Bronchial,
                                        BluRingZ3DViewerPage.Liver, BluRingZ3DViewerPage.Lung,BluRingZ3DViewerPage.Mediastinum,
                                        BluRingZ3DViewerPage.PFossa};
                res = false;
                itr = 0;
                foreach (String rendertype in rendertypes)
                {
                    res = Z3dViewer.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, rendertype, "Preset");
                    Thread.Sleep(2000);
                    if (res == true)
                        itr++;
                    else
                        break;
                }
                if (itr > 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Stepa 19::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 25.7, 31.4, 168.6 mm", scrolllevel: 70, Thickness: "n", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 25.7, 31.4, 168.6 mm", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(60000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 20::Select the reset button from the 3D tool box.
                BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 21::ight click on the navigation control 1, then right click on the Curved drawing tool and select the Auto colon option from the drop down
                bool AutoColon = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (AutoColon)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 22::Click on navigation control 1 and scroll up until the top of the Colon is visible.
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, "Loc: 0.0, 60.0, 0.0 mm", scrolllevel: 60, Thickness: "y", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, "Loc: 0.0, 60.0, 0.0 mm", scrolllevel: 60, Thickness: "y");
                Thread.Sleep(60000);
                LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == "Loc: 0.0, 60.0, 0.0 mm" && LocationAfterScroll != InitialLocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 23::Add a point at the top of the Colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 231, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 91).Click().Build().Perform();
                //Z3dViewer.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 91);
                PageLoadWait.WaitForFrameLoad(10);
                bool check23 = Z3dViewer.checkerrormsg();
                if (check23)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_23 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 232, 0, 0, 255, 2);
                if (ColorValAfter_23 > ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 24::Add a 2nd point along the Colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 241, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                bool check24_0 = Z3dViewer.checkerrormsg();
                if (check24_0)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 119).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_24 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 242, 0, 0, 255, 2);
                if (ColorValAfter_24 > ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 25::Add the 3rd point on the Colon below the 2nd point by clicking on “Navigation 2” image this time.
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Z3dViewer.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);//Config.MPRPathNavigation
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 251, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 33, Navigation2.Size.Height / 2 + 131);
                PageLoadWait.WaitForFrameLoad(10);
                bool check25 = Z3dViewer.checkerrormsg();
                if (check25)
                    throw new Exception("Failed to find path");
                //new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2 + 33, Navigation2.Size.Height / 2 + 131).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_27 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 252, 0, 0, 255, 2);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (ColorValAfter_27 > ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 26::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 10.0, 60.5, -128.6 mm", ScrollDirection: "down", scrolllevel: 70, Thickness: "n", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 10.0, 60.5, -128.6 mm", ScrollDirection: "down", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(30000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Steps 27::From the viewport top bar options on MPR path navigation control ,select the preset drop field.
                //Steps 28::Select and apply all the presets one by one and verify.
                rendertypes = new String[] { BluRingZ3DViewerPage.Abdomen,BluRingZ3DViewerPage.Bone,
                                        BluRingZ3DViewerPage.BoneBody,BluRingZ3DViewerPage.Brain,BluRingZ3DViewerPage.Bronchial,
                                        BluRingZ3DViewerPage.Liver, BluRingZ3DViewerPage.Lung,BluRingZ3DViewerPage.Mediastinum,
                                        BluRingZ3DViewerPage.PFossa };
                res = false;
                itr = 0;
                foreach (String rendertype in rendertypes)
                {
                    res = Z3dViewer.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, rendertype, "Preset");
                    Thread.Sleep(2000);
                    if (res == true)
                        itr++;
                    else
                        break;
                }
                if (itr > 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                
                //Steps 29:Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                //Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 9.5, 47.2, -185.8 mm", scrolllevel: 70, Thickness: "n", UseTestComplete: true);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, "Loc: 9.5, 47.2, -185.8 mm", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(60000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Steps 30::Select the reset button from the 3D tool box.
                BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
                    result.steps[++ExecutedSteps].StepPass();
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
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }


        public TestCaseResult Test_163293(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
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
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];
                String InitialLocVal = objTestRequirement.Split('|')[2];
                IWebElement Navigation1, Navigation2;
                int meandiff;

                //step 01
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Study failed to load");
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 03
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                int ColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                bool check4 = brz3dvp.checkerrormsg();
                if (check4)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_4 != ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = brz3dvp.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_5 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                if (ColorValAfter_4 != ColorValAfter_5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Actions action = new Actions(Driver);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValBefore_6 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 61, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check6 = brz3dvp.checkerrormsg();
                if (check6)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_6 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 62, 0, 0, 255, 2);
                if (ColorValAfter_6 != ColorValBefore_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Accord.Point redpoint = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 71, color: "red");
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                Accord.Point yellowpoint = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 72, "yellow", "Vertical", 1);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(Navigation2, (Int32)redpoint.X, (Int32)redpoint.Y).
                    ClickAndHold().MoveToElement(Navigation2, (Int32)yellowpoint.X, (Int32)yellowpoint.Y).Release().Build().Perform();
                else
                    new TestCompleteAction().PerformDraganddrop(Navigation2, (Int32)redpoint.X, (Int32)redpoint.Y, (Int32)yellowpoint.X, (Int32)yellowpoint.Y);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                IWebElement Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int BColorValBefore_7 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 73, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 25, (Navigation3.Size.Height / 2) + Navigation3.Size.Height / 4);
                PageLoadWait.WaitForFrameLoad(10);
                bool check7 = brz3dvp.checkerrormsg();
                if (check7)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int BColorValAfter_7 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 74, 0, 0, 255, 2);
                if (BColorValAfter_7 != BColorValBefore_7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                res = brz3dvp.ControlFlipStatus(BluRingZ3DViewerPage._3DPathNavigation);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09 
                if(browserName.Contains("explorer"))           
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 39);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 41);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 42, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                if (browserName.Contains("explorer"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 39);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 41);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                String Navigtaion1LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                String Navigation2LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                String Navigation3LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                String MPRNLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, "Loc:");
                String PN3DLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, "Loc:");
                if (Navigtaion1LocVal == InitialLocVal && Navigation2LocVal == InitialLocVal && Navigation3LocVal == InitialLocVal && MPRNLocVal == InitialLocVal && PN3DLocVal == InitialLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                PageLoadWait.WaitForFrameLoad(10);
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step 13
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColorValBefore_13 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 131, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                bool check13 = brz3dvp.checkerrormsg();
                if (check13)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_13 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 132, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_13 != ColorValBefore_13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check14 = brz3dvp.checkerrormsg();
                if (check14)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_14 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 14, 0, 0, 255, 2);
                if (ColorValAfter_14 != ColorValBefore_13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                action = new Actions(Driver);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValBefore_15 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 151, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check15 = brz3dvp.checkerrormsg();
                if (check15)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_15 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 152, 0, 0, 255, 2);
                if (ColorValAfter_15 != ColorValBefore_15)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                action = new Actions(Driver);
                brz3dvp.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                action.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                redpoint = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 161, "red");
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                yellowpoint = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 162, "yellow", "Vertical", 1);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(Navigation2, (Int32)redpoint.X, (Int32)redpoint.Y).
                    ClickAndHold().MoveToElement(Navigation2, (Int32)yellowpoint.X, (Int32)yellowpoint.Y).Release().Build().Perform();
                else
                    new TestCompleteAction().PerformDraganddrop(Navigation2, (Int32)redpoint.X, (Int32)redpoint.Y, (Int32)yellowpoint.X, (Int32)yellowpoint.Y);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int BColorValBefore_16 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 163, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 25, (Navigation3.Size.Height / 2) + Navigation3.Size.Height / 4);
                PageLoadWait.WaitForFrameLoad(10);
                bool check16 = brz3dvp.checkerrormsg();
                if (check16)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int BColorValAfter_16 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 164, 0, 0, 255, 2);
                if (BColorValAfter_16 != BColorValBefore_16)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                res = brz3dvp.ControlFlipStatus(BluRingZ3DViewerPage._3DPathNavigation, check: false);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 28);
                if (!res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 28, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 28);
                if (!res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                Navigtaion1LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                Navigation2LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                Navigation3LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                MPRNLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, "Loc:");
                PN3DLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, "Loc:");
                if (Navigtaion1LocVal == InitialLocVal && Navigation2LocVal == InitialLocVal && Navigation3LocVal == InitialLocVal && MPRNLocVal == InitialLocVal && PN3DLocVal == InitialLocVal)
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

        public TestCaseResult Test_163292(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
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
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];
                String objlocval3 = objTestRequirement.Split('|')[2];//1.3 to1.1
                String objlocval4 = objTestRequirement.Split('|')[3];
                String objlocval5 = objTestRequirement.Split('|')[4];
                String objlocval6 = objTestRequirement.Split('|')[5];
                String objlocval7 = objTestRequirement.Split('|')[6];
                IWebElement Navigation1, Navigation2, Navigation3;

                //step 01 & 02
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 03
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval);
                PageLoadWait.WaitForFrameLoad(5);
                string Nav1LocValue = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (!Nav1LocValue.Equals(objlocval))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 3");
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 04
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                int BlueColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Release().Build().Perform();
                Logger.Instance.InfoLog("Move and Click using selenium actions performed at points (x, y) : " + "(" + ((Navigation1.Size.Width / 2) + 23).ToString() + ", " + ((Navigation1.Size.Height / 4) - 60).ToString() + ")");
                PageLoadWait.WaitForFrameLoad(10);
                bool check15 = brz3dvp.checkerrormsg();
                if (check15)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2);
                if (BlueColorValAfter_4 != BlueColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 4");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 05
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check5 = brz3dvp.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_5 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (BlueColorValAfter_5 != BlueColorValAfter_4 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 5");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 06
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Actions action = new Actions(Driver);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_6 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check6 = brz3dvp.checkerrormsg();
                if (check6)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_6 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (ColorValAfter_6 != ColorValBefore_6 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 6");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 07
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);

                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point redposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 1, "red");
                Thread.Sleep(5000);
                Accord.Point yellowposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 2, "yellow", "vertical", 1);
                Thread.Sleep(5000);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).ClickAndHold()
                        .MoveToElement(Navigation2, (Int32)yellowposition.X, (Int32)yellowposition.Y).Release().Build().Perform();
                else
                    new TestCompleteAction().PerformDraganddrop(Navigation2, (Int32)redposition.X, (Int32)redposition.Y, (Int32)yellowposition.X, (Int32)yellowposition.Y);
                Thread.Sleep(5000);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                int BColorValBefore_7 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 25, (Navigation3.Size.Height / 2) + Navigation3.Size.Height / 4);
                PageLoadWait.WaitForFrameLoad(10);
                bool check7 = brz3dvp.checkerrormsg();
                if (check7)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int BColorValAfter_7 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (BColorValAfter_7 != BColorValBefore_7 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 7");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 08
                String Nav1LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                String Nav1LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                if (Nav1LocValAfter.Equals(objlocval4) && Nav2LocValAfter.Equals(objlocval4) && Nav3LocValAfter.Equals(objlocval4) && Nav1LocValBefore != Nav1LocValAfter && Nav2LocValBefore != Nav2LocValAfter && Nav3LocValBefore != Nav3LocValAfter)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 8");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 09
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 09");
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 10
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval);
                string Navv1LocValue = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (!Navv1LocValue.Equals(objlocval))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 10");
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 11
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int BlueColorValBefore_13 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 25, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                bool check13 = brz3dvp.checkerrormsg();
                if (check13)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_13 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2, isMoveCursor: true);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (BlueColorValAfter_13 != BlueColorValBefore_13 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 11");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 12
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check14 = brz3dvp.checkerrormsg();
                if (check14)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_14 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (BlueColorValAfter_14 != BlueColorValAfter_13 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 12");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 13
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_15 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4));
                PageLoadWait.WaitForFrameLoad(10);
                bool check15_0 = brz3dvp.checkerrormsg();
                if (check15_0)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_15 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValAfter_15 != ColorValBefore_15)
                {
                    Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    action = new Actions(Driver);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                    Thread.Sleep(2000);
                    brz3dvp.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4));
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);

                    new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    redposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 4, "red");
                    Thread.Sleep(5000);
                    PageLoadWait.WaitForFrameLoad(5);
                    //if (browserName.Contains("internet") || )
                        yellowposition = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 5, "yellow", "vertical", 1);
                    //else
                        //yellowposition = new Accord.Point(174, 3);
                        //yellowposition = new Accord.Point((Navigation2.Size.Width * 6) / 11, 3);
                    Thread.Sleep(5000);
                    PageLoadWait.WaitForFrameLoad(5);
                    if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                        new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).ClickAndHold()
                            .MoveToElement(Navigation2, (Int32)yellowposition.X, (Int32)yellowposition.Y).Release().Build().Perform();
                    else
                        new TestCompleteAction().PerformDraganddrop(Navigation2, (Int32)redposition.X, (Int32)redposition.Y, (Int32)yellowposition.X, (Int32)yellowposition.Y);
                    PageLoadWait.WaitForFrameLoad(10);
                    Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    int BColorValBefore_15 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                    brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 25, (Navigation3.Size.Height / 4));
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check15_1 = brz3dvp.checkerrormsg();
                    if (check15_1)
                        throw new Exception("Failed to find path");
                    PageLoadWait.WaitForFrameLoad(10);
                    int BColorValAfter_15 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                    if (BColorValAfter_15 != BColorValBefore_15 && res == true)
                        result.steps[ExecutedSteps].StepPass();
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Failed in Test_159014 Step 13");
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 15");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 14
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                Nav1LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                if (Nav1LocValAfter.Equals(objlocval4) && Nav2LocValAfter.Equals(objlocval4) && Nav3LocValAfter.Equals(objlocval4))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 14");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 15
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 15");
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 16
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval5);
                string Nav118LocValue = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (!Nav118LocValue.Equals(objlocval5))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 16");
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 17
                int ColorValBefore_20 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2), ((Navigation1.Size.Height / 4) * 3) + 10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check20 = brz3dvp.checkerrormsg();
                if (check20)
                    throw new Exception("Failed to find path");
                int ColorValAfter_20 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 202, 0, 0, 255, 2);
                if (ColorValAfter_20 != ColorValBefore_20)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 17");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 18
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2), ((Navigation1.Size.Height / 4) * 3) + 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check21 = brz3dvp.checkerrormsg();
                if (check21)
                    throw new Exception("Failed to find path");
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 18");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 19
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2), ((Navigation1.Size.Height / 4) * 3) + 30);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.checkerrormsg("y");
                PageLoadWait.WaitForFrameLoad(5);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_22 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) - 20, ((Navigation2.Size.Height / 4) * 3) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check22 = brz3dvp.checkerrormsg();
                if (check22)
                    throw new Exception("Failed to find path");
                int ColorValAfter_22 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.ViewerContainer(), ImageFormat: "png");
                if (ColorValAfter_22 != ColorValBefore_22 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 19");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 20
                Nav1LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                Nav1LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                if (Nav1LocValAfter.Equals(objlocval4) && Nav2LocValAfter.Equals(objlocval4) && Nav3LocValAfter.Equals(objlocval4))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Failed in Test_159014 Step 20");
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

        public TestCaseResult Test_163297(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            String ResetLoc = TestData[0];
            String AortaLoc = TestData[1];
            string ColonLoc = TestData[2];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Series should be loaded with out any errors
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 2
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (step1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to open study");

                //step:3  -Click on navigation control 1 and scroll up until the top of the aorta is visible
                bool res3 = Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, scrolllevel: 34);
                if (res3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4::Add a point at the top of the aorta displayed on navigation control 1.
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                int BluColorBeforePoint = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                Thread.Sleep(5000);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 20, 30);
                PageLoadWait.WaitForFrameLoad(20);
                bool res4 = Z3dViewerPage.checkerrormsg();
                if (res4)
                    throw new Exception("Failed to find path");
                int BluColorAfterPoint1 = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2, true);
                if (BluColorAfterPoint1 != BluColorBeforePoint)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5:: Add a 2nd point along the aorta displayed on navigation control 1.
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(5000);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 20, 60);
                PageLoadWait.WaitForFrameLoad(20);
                bool res5 = Z3dViewerPage.checkerrormsg();
                if (res5)
                    throw new Exception("Failed to find path");
                int BluColorAfterPoint2 = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                if (BluColorAfterPoint2 != BluColorAfterPoint1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 6::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 18, 60);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.checkerrormsg("y");
                int ColorValBefore_6 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 24, 90);
                PageLoadWait.WaitForFrameLoad(20);
                bool res6 = Z3dViewerPage.checkerrormsg();
                if (res6)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_6 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && ColorValAfter_6 != ColorValBefore_6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 7::Click on the 3D path navigation control and scroll along part of the path that was generated
                string BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 52, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step8 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 60);
                PageLoadWait.WaitForFrameLoad(20);
                int Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                int Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                int Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                int Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                int Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                int Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                int PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                int PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                int PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                int Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                int Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                int Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                int PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                int PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                int PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 9::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string Step9BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step9BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 55, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step9AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step9AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step9BeforeLocValue1 != Step9AfterLocValue1 && Step9BeforeLocValue2 != Step9AfterLocValue2 && Step9AfterLocValue1 == Step9AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step10 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(2000);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 30);
                PageLoadWait.WaitForFrameLoad(5);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 11::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string Step11BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step11BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "up", scrolllevel: 49, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step11AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step11AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step11BeforeLocValue1 != Step11AfterLocValue1 && Step11BeforeLocValue2 != Step11AfterLocValue2 && Step11AfterLocValue1 == Step11AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step12 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 60);
                PageLoadWait.WaitForFrameLoad(5);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 13::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string Step13BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step13BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "up", scrolllevel: 56, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step13AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step13AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step13BeforeLocValue1 != Step13AfterLocValue1 && Step13BeforeLocValue2 != Step13AfterLocValue2 && Step13AfterLocValue1 == Step13AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step14 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 90);
                PageLoadWait.WaitForFrameLoad(5);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step:15 - Click and hold Right+left mouse button. Drag the image in MPR path navigation and 3D path navigation controls
                IWebElement Navigation3DPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement NavigationMPRPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                System.Drawing.Point location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Thread.Sleep(5000);
                String step15before1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                for (int i = 0; i < 3; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(1000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(3000);
                String step15After1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                PageLoadWait.WaitForFrameLoad(5);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.MPRPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Thread.Sleep(5000);
                String step15before2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 3; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(1000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(3000);
                String step15After2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (step15before1 != step15After1 && step15before2 != step15After2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step:16 - Select the reset button from the toolbar
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String step16_1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String step16_2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String step16_3 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                String step16_4 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String step16_5 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String step16_6 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CurvedMPR);
                if (step16_1.Contains(ResetLoc) && step16_2.Contains(ResetLoc) && step16_3.Contains(ResetLoc) && step16_4.Contains(ResetLoc) && step16_5.Contains(ResetLoc) && step16_6.Contains(ResetLoc))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163305(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            TestCompleteAction tc = new TestCompleteAction();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String objlocval = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;

            try
            {
                login.LoginIConnect(username, password);
                //Step 1  From the Universal viewer , Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                }

                //Step 2 Click on navigation control 1 and scroll up until the top of the aorta is visible. 
                IWebElement navigation = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(navigation).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(navigation, navigation.Size.Width / 2, navigation.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(2000);
                bool scroll = z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval, isClick: true);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (scroll && CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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
                
                //step 3  Add a point at the top of the aorta displayed on navigation control 1.
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int BlueColorValBefore = z3dvp.LevelOfSelectedColor(INavigationone, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 23, (INavigationone.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                bool check15 = z3dvp.checkerrormsg();
                if (check15)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(20);
                new Actions(Driver).MoveToElement(INavigationone).Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_4 = z3dvp.LevelOfSelectedColor(INavigationone, testid, ExecutedSteps + 42, 0, 0, 255, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (BlueColorValAfter_4 != BlueColorValBefore && CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //step 4 Add a 2nd point along the aorta displayed on navigation control 1.
                INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 15, (INavigationone.Size.Height / 4) - 20);
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                bool check4 = z3dvp.checkerrormsg();
                if (check4)
                    throw new Exception("Failed to find path");
                else
                {
                    INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    int BlueColorValAfter_5 = z3dvp.LevelOfSelectedColor(INavigationone, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                    if (BlueColorValAfter_5 != BlueColorValAfter_4)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //Step 5 Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Actions action = new Actions(Driver);
                z3dvp.MoveClickAndHold(INavigationone, (INavigationone.Size.Width / 2) + 15, (INavigationone.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_6 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                z3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check6 = z3dvp.checkerrormsg();
                if (check6)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_6 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                res = CompareImage(result.steps[ExecutedSteps], Navigation2, ImageFormat: "png");
                if (ColorValAfter_6 != ColorValBefore_6 && res == true)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 6 From the viewport top bar options on 3D path navigation control., select the preset field.
                result.steps[++ExecutedSteps].StepPass();
                Logger.Instance.InfoLog("Will be verified in thenext step");

                //step 7 Select and apply the specific transfer function preset on the 3D path navigation control iamges. 
                bool res7 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.Preset1, BluRingZ3DViewerPage.Preset);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (res7 && CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation)))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 8 Select the scroll tool from the 3D tool box and scroll down the path displayed on the 3D path navigation control.
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage._3DPathNavigation);
                Thread.Sleep(1000);
                z3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 15, ScrollDirection: "down", Thickness: "n");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation)))
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
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163307(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
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
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];

                //step 01 & 02
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Failed to open study in Test_163307 Step 02");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 03
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Curve_Drawing_Tool_1_Manual, 50, 50, 100, testid, ExecutedSteps);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), ImageFormat: "png");
                    if (!res)
                        result.steps[ExecutedSteps].StepFail();
                    else
                        result.steps[ExecutedSteps].StepPass();
                }

                //step 04
                int BluColorRegion = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 255, 2);
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.MPRPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 05
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2 + 20).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), ImageFormat: "png");
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //step 06
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 07
                String MPROrientationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 4, MPRPathNavigation.Size.Height / 4).ClickAndHold()
                    .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 4, (MPRPathNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                String MPROrientationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (MPROrientationBefore.Equals(MPROrientationAfter))
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 08
                res = brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.MPRPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 09
                String Navigation1WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                String Navigation2WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                String Navigation3WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                String CurvMPRWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                String MPRPathNavWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2 + 100).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                String Navigation1WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                String Navigation2WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                String Navigation3WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                String CurvMPRWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                String MPRPathNavWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                bool Navigation1Res = Navigation1WLValBefore.Equals(Navigation1WLValAfter);
                bool Navigation2Res = Navigation2WLValBefore.Equals(Navigation2WLValAfter);
                bool Navigation3Res = Navigation3WLValBefore.Equals(Navigation2WLValAfter);
                bool CurvMPRRes = CurvMPRWLValBefore.Equals(CurvMPRWLValAfter);
                bool MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                if (!Navigation1Res && !Navigation2Res && !Navigation3Res && !CurvMPRRes && !MPRPathRes)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                MPRPathNavWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.Brain, BluRingZ3DViewerPage.Preset);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    MPRPathNavWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                    if (!MPRPathRes)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //step 11
                res = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.MPRPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 12
                String imagename = testid + ExecutedSteps + 2;
                String Step12_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step12_imgLocation))
                    File.Delete(Step12_imgLocation);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                if (File.Exists(Step12_imgLocation))
                {
                    res = brz3dvp.CompareDownloadimage(Step12_imgLocation);
                    if (res)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                MPRPathNavWLValBefore = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(5);
                res = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    MPRPathNavWLValAfter = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                    MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                    if (!MPRPathRes)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //step 14
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int BlueColorValAfter_4 = 0;
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval);
                    PageLoadWait.WaitForFrameLoad(5);
                    int BlueColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                    PageLoadWait.WaitForFrameLoad(10);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 40);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check14 = brz3dvp.checkerrormsg();
                    if (check14)
                        throw new Exception("Failed to find path");
                    else
                    {
                        BlueColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2);
                        if (BlueColorValAfter_4 != BlueColorValBefore)
                            result.steps[++ExecutedSteps].StepPass();
                        else
                            result.steps[++ExecutedSteps].StepFail();
                    }
                }

                //step 15
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.MPRPathNavigation);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in selecting zoom tool in MPRPathNavigation");
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                    new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2 + 20).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), ImageFormat: "png");
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in comparing downloaded MPRPathNavigation");
                        result.steps[ExecutedSteps].StepFail();
                    }
                    else
                    {
                        res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed to select Rotate Tool click center in MPRPathNavigation");
                            result.steps[ExecutedSteps].StepFail();
                        }
                        else
                        {
                            MPROrientationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                            MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                            new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 4, MPRPathNavigation.Size.Height / 4).ClickAndHold()
                                .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 4, (MPRPathNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(5);
                            MPROrientationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                            if (MPROrientationBefore.Equals(MPROrientationAfter))
                            {
                                Logger.Instance.ErrorLog("Failed in applying rotate tool click center in MPRPathNavigation");
                                result.steps[ExecutedSteps].StepFail();
                            }
                            else
                            {
                                res = brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.MPRPathNavigation);
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed to select window level tool in MPRPathNavigation");
                                    result.steps[ExecutedSteps].StepFail();
                                }
                                else
                                {
                                    Navigation1WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                                    Navigation2WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                                    Navigation3WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                                    CurvMPRWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                                    MPRPathNavWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                    MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                                    new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2).ClickAndHold()
                                        .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2 + 100).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    Navigation1WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                                    Navigation2WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                                    Navigation3WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                                    CurvMPRWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                                    MPRPathNavWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                    Navigation1Res = Navigation1WLValAfter.Equals(Navigation1WLValAfter);
                                    Navigation2Res = Navigation2WLValAfter.Equals(Navigation2WLValAfter);
                                    Navigation3Res = Navigation2WLValAfter.Equals(Navigation2WLValAfter);
                                    CurvMPRRes = CurvMPRWLValBefore.Equals(CurvMPRWLValAfter);
                                    MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                                    if (Navigation1Res && Navigation2Res && Navigation3Res && CurvMPRRes && MPRPathRes)
                                    {
                                        Logger.Instance.ErrorLog("Failed in applying window level tool in MPRPathNavigation");
                                        result.steps[ExecutedSteps].StepFail();
                                    }
                                    else
                                    {
                                        MPRPathNavWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                        res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.Brain, BluRingZ3DViewerPage.Preset);
                                        if (!res)
                                        {
                                            Logger.Instance.ErrorLog("Failed to select render preset mode in MPRPathNavigation");
                                            result.steps[ExecutedSteps].StepFail();
                                        }
                                        else
                                        {
                                            PageLoadWait.WaitForFrameLoad(5);
                                            MPRPathNavWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                            MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                                            if (MPRPathRes)
                                            {
                                                Logger.Instance.ErrorLog("Failed in comparing downloaded MPRPathNavigation");
                                                result.steps[ExecutedSteps].StepFail();
                                            }
                                            else
                                            {
                                                res = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.MPRPathNavigation);
                                                if (!res)
                                                {
                                                    Logger.Instance.ErrorLog("Failed to select download tool in MPRPathNavigation");
                                                    result.steps[ExecutedSteps].StepFail();
                                                }
                                                else
                                                {
                                                    String imagename15_2 = testid + ExecutedSteps + 2;
                                                    String Step15_imgLocation2 = Config.downloadpath + "\\" + imagename15_2 + ".jpg";
                                                    if (File.Exists(Step15_imgLocation2))
                                                        File.Delete(Step15_imgLocation2);
                                                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).Click().Build().Perform();
                                                    PageLoadWait.WaitForFrameLoad(2);
                                                    brz3dvp.downloadImageForViewport(imagename15_2, "jpg");
                                                    PageLoadWait.WaitForFrameLoad(2);
                                                    if (!File.Exists(Step15_imgLocation2))
                                                    {
                                                        Logger.Instance.ErrorLog("Failed to find the file for the downloaded MPRPathNavigation");
                                                        result.steps[ExecutedSteps].StepFail();
                                                    }
                                                    else
                                                    {
                                                        res = brz3dvp.CompareDownloadimage(Step15_imgLocation2);
                                                        if (res)
                                                        {
                                                            MPRPathNavWLValBefore = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                                            res = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(10);
                                                            brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(5);
                                                            if (!res)
                                                            {
                                                                Logger.Instance.ErrorLog("Failed to select reset option in MPRPathNavigation");
                                                                result.steps[ExecutedSteps].StepFail();
                                                            }
                                                            else
                                                            {
                                                                PageLoadWait.WaitForFrameLoad(5);
                                                                MPRPathNavWLValAfter = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                                                MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                                                                if (!MPRPathRes)
                                                                    result.steps[ExecutedSteps].StepPass();
                                                                else
                                                                {
                                                                    Logger.Instance.ErrorLog("Failed to reset in MPRPathNavigation");
                                                                    result.steps[ExecutedSteps].StepFail();
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Logger.Instance.ErrorLog("Failed to compare the downloaded MPRPathNavigation image");
                                                            result.steps[ExecutedSteps].StepFail();
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
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColorValAfter_20 = 0;
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval2);
                    int ColorValBefore_20 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                    PageLoadWait.WaitForFrameLoad(10);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 4) + 10, (Navigation1.Size.Height / 4) - 50);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    PageLoadWait.WaitForFrameLoad(10);
                    System.Windows.Forms.Cursor.Position = new System.Drawing.Point(0, 0);
                    Thread.Sleep(3000);
                    Logger.Instance.InfoLog("Mouse pointer moved to 0,0 position");
                    PageLoadWait.WaitForFrameLoad(5);
                    IWebElement studylist = Driver.FindElement(By.CssSelector("div.relatedStudiesListComponent"));
                    new Actions(Driver).MoveToElement(studylist).Build().Perform();
                    Thread.Sleep(2000);
                    PageLoadWait.WaitForFrameLoad(5);
                    //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) - 35, (Navigation1.Size.Height / 4) - 50).Click().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 35, (Navigation1.Size.Height / 4) - 35);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check16 = brz3dvp.checkerrormsg();
                    if (check16)
                        throw new Exception("Failed to find path");
                    else
                    {
                        ColorValAfter_20 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 202, 0, 0, 255, 2);
                        if (ColorValAfter_20 != ColorValBefore_20)
                            result.steps[++ExecutedSteps].StepPass();
                        else
                            result.steps[++ExecutedSteps].StepFail();
                    }
                }

                //step 17
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.MPRPathNavigation);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed to select Zoom tool in MPRPathNavigation");
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                    new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2).ClickAndHold()
                    .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2 + 20).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), ImageFormat: "png");
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in comparing the image after zoom tool applied in MPRPathNavigation");
                        result.steps[ExecutedSteps].StepFail();
                    }
                    else
                    {
                        res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed to select rotate click center tool in MPRPathNavigation");
                            result.steps[ExecutedSteps].StepFail();
                        }
                        else
                        {
                            MPROrientationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                            MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                            new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 4, MPRPathNavigation.Size.Height / 4).ClickAndHold()
                                .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 4, (MPRPathNavigation.Size.Height / 4) * 3).Release().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(5);
                            MPROrientationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                            if (MPROrientationBefore.Equals(MPROrientationAfter))
                            {
                                Logger.Instance.ErrorLog("Failed to apply rotation tool click center in MPRPathNavigation");
                                result.steps[ExecutedSteps].StepFail();
                            }
                            else
                            {
                                res = brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.MPRPathNavigation);
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed to select window level tool in MPRPathNavigation");
                                    result.steps[ExecutedSteps].StepFail();
                                }
                                else
                                {
                                    Navigation1WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                                    Navigation2WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                                    Navigation3WLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                                    CurvMPRWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                                    MPRPathNavWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                    MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                                    new Actions(Driver).MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2).ClickAndHold()
                                        .MoveToElement(MPRPathNavigation, MPRPathNavigation.Size.Width / 2, MPRPathNavigation.Size.Height / 2 + 100).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    Navigation1WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationone);
                                    Navigation2WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationtwo);
                                    Navigation3WLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.Navigationthree);
                                    CurvMPRWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.CurvedMPR);
                                    MPRPathNavWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                    Navigation1Res = Navigation1WLValAfter.Equals(Navigation1WLValAfter);
                                    Navigation2Res = Navigation2WLValAfter.Equals(Navigation2WLValAfter);
                                    Navigation3Res = Navigation2WLValAfter.Equals(Navigation2WLValAfter);
                                    CurvMPRRes = CurvMPRWLValBefore.Equals(CurvMPRWLValAfter);
                                    MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                                    if (Navigation1Res && Navigation2Res && Navigation3Res && CurvMPRRes && MPRPathRes)
                                    {
                                        Logger.Instance.ErrorLog("Failed to apply window level tool in MPRPathNavigation");
                                        result.steps[ExecutedSteps].StepFail();
                                    }
                                    else
                                    {
                                        MPRPathNavWLValBefore = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                        res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.Brain, BluRingZ3DViewerPage.Preset);
                                        if (!res)
                                        {
                                            Logger.Instance.ErrorLog("Failed to select render preset mode in MPRPathNavigation");
                                            result.steps[ExecutedSteps].StepFail();
                                        }
                                        else
                                        {
                                            PageLoadWait.WaitForFrameLoad(5);
                                            MPRPathNavWLValAfter = brz3dvp.GetWindowLevelValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                            MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                                            if (MPRPathRes)
                                            {
                                                Logger.Instance.ErrorLog("Failed to apply window level preset in MPRPathNavigation");
                                                result.steps[ExecutedSteps].StepFail();
                                            }
                                            else
                                            {
                                                res = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.MPRPathNavigation);
                                                if (!res)
                                                {
                                                    Logger.Instance.ErrorLog("Failed to select download tool in MPRPathNavigation");
                                                    result.steps[ExecutedSteps].StepFail();
                                                }
                                                else
                                                {
                                                    String imagename17_2 = testid + ExecutedSteps;
                                                    String Step17_imgLocation2 = Config.downloadpath + "\\" + imagename17_2 + ".jpg";
                                                    if (File.Exists(Step17_imgLocation2))
                                                        File.Delete(Step17_imgLocation2);
                                                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).Click().Build().Perform();
                                                    PageLoadWait.WaitForFrameLoad(2);
                                                    brz3dvp.downloadImageForViewport(imagename17_2, "jpg");
                                                    PageLoadWait.WaitForFrameLoad(2);
                                                    if (!File.Exists(Step17_imgLocation2))
                                                    {
                                                        Logger.Instance.ErrorLog("MPRPathNavigation image failed to download or the file doesnt exist");
                                                        result.steps[ExecutedSteps].StepFail();
                                                    }
                                                    else
                                                    {
                                                        res = brz3dvp.CompareDownloadimage(Step17_imgLocation2);
                                                        if (res)
                                                        {
                                                            MPRPathNavWLValBefore = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                                            res = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(10);
                                                            brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(5);
                                                            if (!res)
                                                            {
                                                                Logger.Instance.ErrorLog("Failed to select reset option from tool box in MPRPathNavigation");
                                                                result.steps[ExecutedSteps].StepFail();
                                                            }
                                                            else
                                                            {
                                                                PageLoadWait.WaitForFrameLoad(5);
                                                                MPRPathNavWLValAfter = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                                                                MPRPathRes = MPRPathNavWLValBefore.Equals(MPRPathNavWLValAfter);
                                                                if (!MPRPathRes)
                                                                    result.steps[ExecutedSteps].StepPass();
                                                                else
                                                                {
                                                                    Logger.Instance.ErrorLog("Failed to verify window level in MPRPathNavigation");
                                                                    result.steps[ExecutedSteps].StepFail();
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Logger.Instance.ErrorLog("Failed in comparing downloaded MPRPathNavigation image");
                                                            result.steps[ExecutedSteps].StepFail();
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
        public TestCaseResult Test_163306(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            string slocationvalue = ssplit[0];
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;

            try
            {
                login.LoginIConnect(username, password);
                //Step 1  From the Universal viewer , Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                //Step 2 Click on navigation control 1 and scroll up until the top of the aorta is visible. 
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(1000);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                Thread.Sleep(1000);
                IWebElement viewcontainer = z3dvp.ViewerContainer();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(500);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                {
                    do
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 50, 0);
                        Thread.Sleep(1000);
                    }
                    while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2, 0, 1) <= 33);
                }
                else
                {
                    for (int i = 0; i < 44; i++)
                    {
                        BasePage.mouse_event(0x0800, 0, 0, 15, 0);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(5000);
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

                //step 3 Add a point at the top of the aorta displayed on navigation control 1.
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 45).Click().Build().Perform();
                else
                    z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 22, (INavigationone.Size.Height / 4) - 50);
                PageLoadWait.WaitForFrameLoad(10);
                bool check3 = z3dvp.checkerrormsg();
                if (check3)
                    throw new Exception("Failed to find path");
                else
                {
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
                }

                //step 4 Add a 2nd point along the aorta displayed on navigation control 1.
                Actions act3 = new Actions(Driver);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    new Actions(Driver).MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35).Click().Build().Perform();
                else
                    z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 22, (INavigationone.Size.Height / 4) - 35);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                bool check4 = z3dvp.checkerrormsg();
                if (check4)
                    throw new Exception("Failed to find path");
                else
                {
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
                }

                //step 5 Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                {
                    Actions action = new Actions(Driver);
                    action.MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 22, (INavigationone.Size.Height / 4) - 35).ClickAndHold().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    action.Release().Build().Perform();
                }
                else
                    z3dvp.MoveClickAndHold(INavigationone, (INavigationone.Size.Width / 2) + 22, (INavigationone.Size.Height / 4) - 35);

                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                //Navigation 2
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement INavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Actions act5 = new Actions(Driver);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                    act5.MoveToElement(INavigationtwo, (INavigationtwo.Size.Width / 2) + 18, (INavigationtwo.Size.Height / 4) - 25).Click().Build().Perform();
                else
                    z3dvp.MoveAndClick(INavigationtwo, (INavigationtwo.Size.Width / 2) + 22, (INavigationtwo.Size.Height / 4) - 25);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                bool check5 = z3dvp.checkerrormsg();
                if (check5)
                    throw new Exception("Failed to find path");
                else
                {
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)))
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

                //step 7 Select the first render mode (MIPIP) on the drop down list to apply to the volume displayed on the MPR path navigation control. 
                bool check7 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.MinIp, BluRingZ3DViewerPage.RenderType);
                Logger.Instance.InfoLog("Check10 value is : " + check7.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                bool res7 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.MinIp, BluRingZ3DViewerPage.RenderType);
                if (res7)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 8 Select the scroll tool from the 3D tool box.
                bool bscrolltoll = z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                if (bscrolltoll)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 9 Scroll through the path displayed on the MPR path navigation control.
                z3dvp.SelectControl(BluRingZ3DViewerPage.MPRPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 550), (viewcontainer.Location.Y / 2 + 600));
                Thread.Sleep(500);
                for (int i = 0; i < 10; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)))
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

                //Step 10 Select the second render mode (Min) on the drop down list to apply to the volume displayed on the MPR path navigation control
                bool check10 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.Mip, BluRingZ3DViewerPage.RenderType);
                Logger.Instance.InfoLog("Check10 value is : " + check10.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                bool res10 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.Mip, BluRingZ3DViewerPage.RenderType);
                if (res10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11 Scroll through the path displayed on the MPR path navigation control.
                z3dvp.SelectControl(BluRingZ3DViewerPage.MPRPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 550), (viewcontainer.Location.Y / 2 + 600));
                for (int i = 0; i < 10; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), pixelTolerance: 50))
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

                //step 12 Select the Third render mode (Average) on the drop down list to apply to the volume displayed on the MPR path navigation control.
                bool check1 = z3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.Average, BluRingZ3DViewerPage.RenderType);
                Logger.Instance.InfoLog("check1 return is : " + check1.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                bool res12 = z3dvp.Verify_Render_PresetMode_Checked(BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage.Average, BluRingZ3DViewerPage.RenderType);
                if (res12)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 13 Scroll through the path displayed on the MPR path navigation control.
                z3dvp.SelectControl(BluRingZ3DViewerPage.MPRPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 550), (viewcontainer.Location.Y / 2 + 600));
                Thread.Sleep(1000);
                for (int i = 0; i < 10; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -15, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), pixelTolerance: 50))
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

                //step 16 Select the Reset option from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(5);
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result3 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result3[0] == result3[1] && result3[1] == result3[2] && slocationvalue == (result3[0]))
                {
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
                login.Logout();
            }
        }

        public TestCaseResult Test_163310(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string Study2PID = TestData[0];
            string Study2Descr = TestData[1];
            String Descr2 = TestData[2];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1  - Log in iCA and Navigate to studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step:2 - Search and load a 3D supported Lossy compressed study in the universal viewer
                Boolean step2 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR, field: "acc", thumbimgoptional: Descr2);
                if (step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to open lossy compressed study");

                //step:3 Series is loaded in the 3D viewer in Curved MPR viewing mode
                IList<IWebElement> Viewport = Z3dViewerPage.Viewport();
                Boolean step3 = Z3dViewerPage.VerifyCursorMode(BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CurvedToolManualCursor);
                if (step3 && Viewport.Count == 6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Select the 3D settings option from the settings drop down. move the MPR interactive quality/3D interactive quality slider to 100% and click on the save button.
                Boolean step4_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                Boolean step4_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (step4_1 && step4_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Create a path by adding the points in the navigation controls
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int BluColorBeforePoint = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                //Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 - 50, 30);
                new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2 - 50, 30).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 - 50, 60);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 - 50, 90);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForFrameLoad(20);
                bool res5 = Z3dViewerPage.checkerrormsg();
                if (res5)
                    throw new Exception("Failed to find path");
                int BluColorAfterPoint1 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 52, 0, 0, 255, 2);
                if (BluColorAfterPoint1 > BluColorBeforePoint)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:6 - Scroll cursor shows up while hovering over the images displayed on the controls.
                Boolean step6 = Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.CurvedMPR);
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

                //step:7 - "Lossy Compression" annotation is displayed during the interaction on the images in MPR path navigation and 3D path navigation.
                Boolean step7_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10);
                Boolean step7_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10);
                if (step7_1 && step7_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:8 - Rotate cursor shows up while hovering over the images displayed on the MPR controls.
                Boolean step8 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
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

                //step:9 - "Lossy Compression" annotation is displayed during the interaction on the images in MPR path navigation and 3D path navigation
                Boolean step9_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10);
                Boolean step9_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10);
                if (step9_1 && step9_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 - Zoom cursor shows up while hovering over the images displayed on the controls
                Boolean step10 = Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.CurvedMPR);
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

                //step:11 - Click and hold the left mouse button on the image displayed on the 3D path navigation and do Zoom in /zoom out by dragging the mouse upwards/downwards.
                Boolean step11_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10);
                Boolean step11_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 2);
                if (step11_1 && step11_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12 - "Lossy Compression" annotation is displayed during the interaction on the images in all MPR navigation controls.
                Boolean step12_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 10);
                Boolean step12_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 10);
                Boolean step12_3 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 5, 10);
                if (step12_1 && step12_2 && step12_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13 - Select the rotate tool from the 3D tool box
                Boolean step13 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (step13)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - "Lossy Compression" annotation is displayed during the interaction on the images in Curved MPR control.
                Boolean step14 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10);
                if (step14)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:15  -Select the 3D settings option from the settings drop down. move the MPR interactive quality/3D interactive quality sliders lesser 100%. ( Range = 1% to 99%).
                Boolean step15_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 95);
                Boolean step15_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 96);
                if (step15_1 && step15_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:16 - Same expected results should be preserved from steps 6-14
                Boolean step16_1 = Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_3 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_4 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_5 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_6 = Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_7 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_8 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 2);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_9 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_10 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_11 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_12 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step16_13 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                if (step16_1 && step16_2 && step16_3 && step16_4 && step16_5 && step16_6 && step16_7 && step16_8 && step16_9 && step16_10 && step16_11 && step16_12 && step16_13)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:17 - Launch the study with No Lossy compressed series in universal viewer
                //Z3dViewerPage.ExitIcon().Click();
                //Z3dViewerPage.CloseStudy();
                ClickElement(Z3dViewerPage.ExitIcon());
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step15 = Z3dViewerPage.searchandopenstudyin3D(Study2PID, Study2Descr, BluRingZ3DViewerPage.CurvedMPR);
                if (step15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                    throw new Exception("Failed to open no lossy compressed study");

                //step:18 - Series is loaded in the 3D viewer in Curved MPR viewing mode
                Viewport = Z3dViewerPage.Viewport();
                Boolean step18 = Z3dViewerPage.VerifyCursorMode(BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CurvedToolManualCursor);
                if (step18 && Viewport.Count == 6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:19  -Select the 3D settings option from the settings drop down. move the MPR and 3D interactive quality sliders to 100%
                Boolean step19_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                Boolean step19_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (step19_1 && step19_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:20 - Create a path by adding the points in the navigation controls
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                BluColorBeforePoint = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 - 50, 30);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 - 50, 60);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 - 50, 90);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForFrameLoad(20);
                bool res20 = Z3dViewerPage.checkerrormsg();
                if (res20)
                    throw new Exception("Failed to find path");
                BluColorAfterPoint1 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 202, 0, 0, 255, 2);
                if (BluColorAfterPoint1 > BluColorBeforePoint)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:21  -Scroll cursor shows up while hovering over the images displayed on the controls
                Boolean step21 = Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.CurvedMPR);
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

                //step:22 - Scroll through the image in MPR path navigation
                Boolean step22_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10, "n");
                PageLoadWait.WaitForPageLoad(10);
                Boolean step22_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10, "n");
                if (step22_1 && step22_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:23 - Select the Rotate tool from the toolbar
                Boolean step23 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                if (step23)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:24 - "Lossy Compression" annotation is displayed during the interaction on the images in MPR path navigation and 3D path navigation
                Boolean step24_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10, "n");
                Boolean step24_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10, "n");
                if (step24_1 && step24_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:25 - Zoom cursor shows up while hovering over the images displayed on the controls
                Boolean step25 = Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.CurvedMPR);
                if (step25)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:26 - Click and hold the left mouse button on the image displayed on the 3D path navigation and do Zoom in /zoom out by dragging the mouse upwards/downwards.
                Boolean step26_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10, "n");
                Boolean step26_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 2, "n");
                if (step26_1 && step26_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:27 - "Lossy Compression" annotation is not displayed during the interaction on the images in all MPR navigation controls.
                Boolean step27_1 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 10, "n");
                Boolean step27_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 10, "n");
                Boolean step27_3 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 5, 10, "n");
                if (step27_1 && step27_2 && step27_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:28 - Select the rotate tool from the 3D tool box
                Boolean step28 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (step28)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:29 - "Lossy Compression" annotation is not displayed during the interaction on the images in Curved MPR control.
                Boolean step29 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10, "n");
                if (step29)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:30  -Select the 3D settings option from the settings drop down. move the MPR interactive quality/3D interactive quality sliders lesser 100%. ( Range = 1% to 99%).
                Boolean step30_1 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 95);
                Boolean step30_2 = Z3dViewerPage.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 96);
                if (step30_1 && step30_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step:31 - Repeat steps 5-14., "Lossy Compression" annotation is displayed during the interaction 
                Boolean step31_1 = Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_2 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_3 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_4 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_5 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_6 = Z3dViewerPage.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_7 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_8 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 5, 5, 2);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_9 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationone, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_10 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationtwo, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_11 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.Navigationthree, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_12 = Z3dViewerPage.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step31_13 = Z3dViewerPage.CheckLossyAnnotation(BluRingZ3DViewerPage.CurvedMPR, 5, 5, 10);
                PageLoadWait.WaitForFrameLoad(5);
                if (step31_1 && step31_2 && step31_3 && step31_4 && step31_5 && step31_6 && step31_7 && step31_8 && step31_9 && step31_10 && step31_11 && step31_12 && step31_13)
                {
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

        public TestCaseResult Test_163309(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            string slocationvalue = ssplit[0];
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            string slocation = ssplit[0];

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;

            try
            {
                login.LoginIConnect(username, password);
                //Step 1  Search and load the 3D supported study in universal viewer.
                //step 2 Select the Curved MPR option from the smart view drop down.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step 3 Create a Path in MPR navigation controls using Manual mode.
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(1000);
                IWebElement viewcontainer = z3dvp.ViewerContainer();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(500);
                IWebElement INavigationone_3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.MoveAndClick(INavigationone_3, (INavigationone_3.Size.Width / 2) - 10, (INavigationone_3.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.MoveAndClick(INavigationone_3, (INavigationone_3.Size.Width / 2) - 10, (INavigationone_3.Size.Height / 4) - 2);
                Thread.Sleep(5000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                bool check3 = z3dvp.checkerrormsg();
                if (check3)
                    throw new Exception("Failed to find path");
                else
                {
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR)))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step *3 Passed--" + result.steps[ExecutedSteps].description);

                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step 4 Select the Window level tool option from the 3D tool box.
                bool btool4 = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.CurvedMPR);
                if (btool4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *4 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5  Left click and drag the mouse on the image displayed on the Curved MPR control. 
                List<string> result_before5 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                IWebElement IcurvedMpr5 = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int xcoordinate5 = IcurvedMpr5.Location.X;
                int ycoordinate5 = IcurvedMpr5.Location.Y;
                System.Drawing.Point point5 = new System.Drawing.Point(xcoordinate5 + 100, ycoordinate5 + 80);
                Cursor.Position = point5;
                new Actions(Driver).DragAndDropToOffset(IcurvedMpr5, 2, 2).Build().Perform();
                Thread.Sleep(3000);
                List<string> result_after5 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (result_before5[0] != result_after5[0] && result_before5[1] != result_after5[1] && result_before5[3] != result_after5[3] && result_before5[4] != result_after5[4])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *5 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 Select the rotate tool 
                bool btool6 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (btool6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *6 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 7 Left click and drag the image on the Curved MPR control.
                Cursor.Position = point5;
                new Actions(Driver).DragAndDropToOffset(IcurvedMpr5, 2, 2).Build().Perform();
                Thread.Sleep(3000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 10))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *7 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 Right click and drag the image on the Curved MPR control.
                int X = 0; // Cursor.Position.X; 
                int Y = 0; // Cursor.Position.Y; 
                Thread.Sleep(1000);
                uint action = 0;
                IWebElement IcurvedMpr8 = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int xcoordinate = IcurvedMpr8.Location.X;
                int ycoordinate = IcurvedMpr8.Location.Y;
                System.Drawing.Point point = new System.Drawing.Point(xcoordinate + 100, ycoordinate + 225);
                Cursor.Position = point;
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(5000);
                action = (uint)MouseEventFlags.MOVE; Y = 15; X = 3;
                mouse_event(action, (uint)X, (uint)Y, (int)0, (int)0);
                Thread.Sleep(3000);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                Thread.Sleep(5000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 30))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *8 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 9 Select the Download image option from the tool box.
                bool btool9 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.CurvedMPR);
                if (btool9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *9 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 10  Save the Image to the local drive 
                String imagename = testid + ExecutedSteps + 71;
                String Step10_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step10_imgLocation))
                    File.Delete(Step10_imgLocation);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                z3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                if (File.Exists(Step10_imgLocation))
                {
                    bool Result = z3dvp.CompareDownloadimage(Step10_imgLocation);
                    if (Result)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 11 click the reset button 
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(10000);
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                PageLoadWait.WaitForFrameLoad(5);
                List<string> result11 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result11[0] == result11[1] && result11[2] == result11[2] && result11[4] == result11[5] && slocation == result11[5])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *11 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 Create a Path in MPR navigation controls using Auto Vessels mode.
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(5000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(1000);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 34);
                bool check = z3dvp.checkerrormsg();
                if (check)
                    throw new Exception("Error message found");
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                {
                    z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 50);
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 10);
                }
                else
                {
                    z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 22, (INavigationone.Size.Height / 4) - 60);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 22, (INavigationone.Size.Height / 4) - 20);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                }
                Thread.Sleep(5000);
                bool check12 = z3dvp.checkerrormsg();
                if (check12)
                    throw new Exception("Failed to find path");
                else
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                    Thread.Sleep(1000);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 10))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step *12 Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step 13_1  Select the Window level tool option from the 3D tool box. 
                bool bflag13_1 = false;
                bool btool13_1 = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.CurvedMPR);
                if (btool13_1)
                {
                    bflag13_1 = true;
                }
                Thread.Sleep(1000);
                //Step 13_2 Left click and drag the mouse on the image displayed on the Curved MPR control.
                List<string> result_before13_2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                Thread.Sleep(1000);
                IWebElement IcurvedMpr13_2 = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int xcoordinate13_2 = IcurvedMpr13_2.Location.X;
                int ycoordinate13_2 = IcurvedMpr13_2.Location.Y;
                Cursor.Position = point5;
                bool bflag13_2 = false;
                new Actions(Driver).DragAndDropToOffset(IcurvedMpr13_2, 2, 2).Build().Perform();
                Thread.Sleep(10000);
                List<string> result_after13_2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (result_before13_2[0] != result_after13_2[0] && result_before13_2[1] != result_after13_2[1] && result_before13_2[3] != result_after13_2[3] && result_before13_2[4] != result_after13_2[4])
                {
                    bflag13_2 = true;
                }
                Thread.Sleep(3000);

                //step 13_3 Select the Rotate tool from the 3D tool box.
                bool bflag13_3 = false;

                bool btool13_3 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (btool13_3)
                {
                    bflag13_3 = true;
                }
                Thread.Sleep(1000);
                //step 13_4 Left click and drag the image on the Curved MPR control.
                bool bflag13_4 = false;
                Cursor.Position = point5;
                new Actions(Driver).DragAndDropToOffset(IcurvedMpr5, 5, 5).Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 10))
                {
                    bflag13_4 = true;
                }
                Thread.Sleep(1000);
                //Step 13_5 Right click and drag the image on the Curved MPR control.
                Thread.Sleep(1000);
                IWebElement IcurvedMpr13_5 = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int xcoordinate13_5 = IcurvedMpr13_5.Location.X;
                int ycoordinate13_5 = IcurvedMpr13_5.Location.Y;

                System.Drawing.Point point13_5 = new System.Drawing.Point(xcoordinate13_5 + 100, ycoordinate13_5 + 225);
                Cursor.Position = point13_5;
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(5000);
                action = (uint)MouseEventFlags.MOVE; Y = 15; X = 3;
                mouse_event(action, (uint)X, (uint)Y, (int)0, (int)0);
                Thread.Sleep(3000);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                Thread.Sleep(7000);
                bool bflag13_5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 10))
                {
                    bflag13_5 = true;
                }
                Thread.Sleep(1000);
                //Step 13_6 Select the Download image option from the tool box.
                bool bflag13_6 = false;
                bool btool13_6 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.CurvedMPR);
                if (btool13_6)
                {
                    bflag13_6 = true;
                }
                Thread.Sleep(1000);
                //step 13_7 Save the Image to the local drive
                bool bcompare13_7 = false, bflag13_7 = false;
                imagename = testid + ExecutedSteps + 1371;
                String Step13_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step13_imgLocation))
                    File.Delete(Step13_imgLocation);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                z3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                if (File.Exists(Step13_imgLocation))
                {
                    bool Result = z3dvp.CompareDownloadimage(Step13_imgLocation);
                    if (Result)
                        bcompare13_7 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("File doesnt exist in the location : " + Step13_imgLocation.ToString());
                }
                if (bcompare13_7)
                {
                    bflag13_7 = true;
                }
                Thread.Sleep(1000);

                //step 13_8 Click the reset button 
                bool bflag13_8 = false;
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(10000);

                List<string> result13_8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result13_8[0] == result13_8[1] && result13_8[2] == result13_8[3] && result13_8[4] == result13_8[5] && slocation == result13_8[5])
                {
                    bflag13_8 = true;
                }
                Thread.Sleep(1000);
                if (bflag13_1 && bflag13_2 && bflag13_3 && bflag13_4 && bflag13_5 && bflag13_6 && bflag13_7 && bflag13_8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *13 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 14 Create a Path in MPR navigation controls using Auto Colon mode.
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 69);
                check = z3dvp.checkerrormsg();
                if (check)
                    throw new Exception("Error message found");
                IWebElement INavigationone14 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.MoveAndClick(INavigationone14, (INavigationone14.Size.Width / 2) + 5, (INavigationone14.Size.Height / 2) + 90);
                PageLoadWait.WaitForFrameLoad(10);
                if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                {
                    z3dvp.MoveAndClick(INavigationone14, (INavigationone14.Size.Width / 2) + 5, (INavigationone14.Size.Height / 2) + 115);
                }
                else
                {
                    z3dvp.MoveAndClick(INavigationone14, (INavigationone14.Size.Width / 2 + 5), (INavigationone14.Size.Height - 80));
                    PageLoadWait.WaitForFrameLoad(10);
                    z3dvp.MoveAndClick(INavigationone14, (INavigationone14.Size.Width / 2 + 5), (INavigationone14.Size.Height - 60));
                }
                Thread.Sleep(5000);
                bool check14 = z3dvp.checkerrormsg();
                if (check14)
                    throw new Exception("Failed to find path");
                else
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                    Thread.Sleep(1000);
                    bool bflag14 = false;
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 10))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step *14 Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                Thread.Sleep(3000);
                //step 15_1 Select the Window level tool option from the 3D tool box.
                bool bflag15_1 = false;
                bool btool15_1 = z3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.CurvedMPR);
                if (btool15_1)
                {
                    bflag15_1 = true;
                }
                Thread.Sleep(1000);
                //Step 15_2 Left click and drag the mouse on the image displayed on the Curved MPR control.
                List<string> result_before15_2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                IWebElement IcurvedMpr15_2 = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int xcoordinate15_2 = IcurvedMpr15_2.Location.X;
                int ycoordinate15_2 = IcurvedMpr15_2.Location.Y;
                Cursor.Position = point5;
                bool bflag15_2 = false;
                new Actions(Driver).DragAndDropToOffset(IcurvedMpr15_2, 5, 5).Build().Perform();
                Thread.Sleep(10000);
                List<string> result_after15_2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (result_before15_2[0] != result_after15_2[0] && result_before15_2[1] != result_after15_2[1] && result_before15_2[3] != result_after15_2[3] && result_before15_2[4] != result_after15_2[4])
                {
                    bflag15_2 = true;
                }
                Thread.Sleep(1000);
                bool bflag15_3 = false;
                bool btool15_3 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.CurvedMPR);
                if (btool15_3)
                {
                    bflag15_3 = true;
                }
                Thread.Sleep(1000);
                //step 15_4 Left click and drag the image on the Curved MPR control.
                bool bflag15_4 = false;
                Cursor.Position = point5;
                new Actions(Driver).DragAndDropToOffset(IcurvedMpr5, 5, 5).Build().Perform();
                Thread.Sleep(3000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 10))
                {
                    bflag15_4 = true;
                }
                Thread.Sleep(1000);
                //Step 15_5 Right click and drag the image on the Curved MPR control.
                Thread.Sleep(1000);
                IWebElement IcurvedMpr15_5 = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int xcoordinate15_5 = IcurvedMpr15_5.Location.X;
                int ycoordinate15_5 = IcurvedMpr15_5.Location.Y;
                System.Drawing.Point point15_5 = new System.Drawing.Point(xcoordinate15_5 + 100, ycoordinate15_5 + 225);
                Cursor.Position = point15_5;
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(5000);
                Thread.Sleep(1000);
                action = (uint)MouseEventFlags.MOVE; Y = 15; X = 3;
                mouse_event(action, (uint)X, (uint)Y, (int)0, (int)0);
                Thread.Sleep(1000);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                Thread.Sleep(5000);
                bool bflag15_5 = false;
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR), pixelTolerance: 10))
                {
                    bflag15_5 = true;
                }
                Thread.Sleep(1000);
                //Step 13_6 Select the Download image option from the tool box.
                bool bflag15_6 = false;
                bool btool15_6 = z3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.CurvedMPR);
                if (btool15_6)
                {
                    bflag15_6 = true;
                }
                Thread.Sleep(1000);

                //step 13_7 Save the Image to the local drive
                Thread.Sleep(2000);
                
                bool bcompare15_7 = false, bflag15_7 = false;
                imagename = testid + ExecutedSteps + 1571;
                String Step15_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step15_imgLocation))
                    File.Delete(Step15_imgLocation);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                z3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                if (File.Exists(Step15_imgLocation))
                {
                    bool Result = z3dvp.CompareDownloadimage(Step15_imgLocation);
                    if (Result)
                        bcompare15_7 = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("File doesnt exist in the location : " + Step15_imgLocation.ToString());
                }
                if (bcompare15_7)
                {
                    bflag15_7 = true;
                }
                Thread.Sleep(2000);
                //step 15_8 Click the reset button 
                bool bflag15_8 = false;
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(3000);
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(5000);
                List<string> result15_8 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result15_8[0] == result15_8[1] && result15_8[2] == result15_8[2] && result15_8[4] == result15_8[5] && slocation == result15_8[5])
                {
                    bflag15_8 = true;
                }
                if (bflag15_1 && bflag15_2 && bflag15_3 && bflag15_4 && bflag15_5 && bflag15_6 && bflag15_7 && bflag15_8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *15 Passed--" + result.steps[ExecutedSteps].description);
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
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
                // login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163301(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String objlocval1 = TestDataRequirements.Split('|')[0];
            String objlocval2 = TestDataRequirements.Split('|')[1];
            String objlocval3 = TestDataRequirements.Split('|')[2];
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            IWebElement Navigation1, Navigation2, Navigation3, CurvedMPRNavigation, PathNavigation3D, PathNavigationMPR;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01 & 02
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Thread.Sleep(2000);
                bool res = z3dvp.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 03
                String Navigation1Annotation = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                String Navigation2Annotation = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Navigation3Annotation = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                String[] Navigtion1WL = (((Navigation1Annotation.Replace("mm", "x")).Split('x')).Last()).Split('/');
                String[] Navigtion2WL = (((Navigation2Annotation.Replace("mm", "x")).Split('x')).Last()).Split('/');
                String[] Navigtion3WL = (((Navigation3Annotation.Replace("mm", "x")).Split('x')).Last()).Split('/');
                bool result1 = Navigtion1WL[0].Trim().Equals(Navigtion2WL[0].Trim()) && Navigtion1WL[0].Trim().Contains(Navigtion3WL[0].Trim());
                bool result2 = Navigtion1WL[1].Trim().Equals(Navigtion2WL[1].Trim()) && Navigtion1WL[1].Trim().Contains(Navigtion3WL[1].Trim());
                if (result1 && result2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                bool step4 = z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval1);
                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                int BlueColorValBefore = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                Thread.Sleep(5000);
                z3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                Thread.Sleep(20000);
                bool check5 = z3dvp.checkerrormsg();
                int BlueColorValAfter_5 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2);
                if (BlueColorValAfter_5 != BlueColorValBefore && check5 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 06
                z3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(10000);
                bool check6 = z3dvp.checkerrormsg();
                int BlueColorValAfter_6 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                if (BlueColorValAfter_6 != BlueColorValAfter_5 && check6 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 07
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(10000);
                z3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(10000);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(10000);
                int ColorValBefore_6 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                z3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                Thread.Sleep(10000);
                bool check7 = z3dvp.checkerrormsg();
                int ColorValAfter_6 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                if (ColorValAfter_6 != ColorValBefore_6 && check7 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 08
                String LocationValueMPRBefore = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                res = z3dvp.ScrollInView(viewport: BluRingZ3DViewerPage.MPRPathNavigation, ScrollDirection: "down", scrolllevel: 30, Thickness: "n");
                String LocationValueMPRAfter = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (LocationValueMPRAfter != LocationValueMPRBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                String Nav1AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                String PathMPRAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String PathCurvedAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                String Path3DAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String[] Controls = { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree };
                foreach (String controlements in Controls)
                {
                    System.Drawing.Point location = z3dvp.ControllerPoints(controlements);
                    int xcoordinate = location.X;
                    int ycoordinate = location.Y;
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 20);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    Thread.Sleep(5000);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                }
                Thread.Sleep(10000);
                String Nav1AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                String PathMPRAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String PathCurvedAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                String Path3DAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                bool Comparison1 = Nav1AnnotationValAfter.Equals(Nav1AnnotationValBefore);
                bool Comparison2 = Nav2AnnotationValAfter.Equals(Nav2AnnotationValBefore);
                bool Comparison3 = Nav3AnnotationValAfter.Equals(Nav3AnnotationValBefore);
                bool Comparison4 = PathMPRAnnotationValAfter.Equals(PathMPRAnnotationValBefore);
                bool Comparison5 = PathCurvedAnnotationValAfter.Equals(PathCurvedAnnotationValBefore);
                bool Comparison6 = Path3DAnnotationValAfter.Equals(Path3DAnnotationValBefore);
                if (!Comparison1 && !Comparison2 && !Comparison3 && !Comparison4 && !Comparison5 && Comparison6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                res = z3dvp.select3DTools(Z3DTools.Window_Level);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                PathNavigationMPR = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Nav1AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                PathMPRAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathCurvedAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                Path3DAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, PathNavigationMPR.Size.Height / 4).ClickAndHold()
                    .MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, (PathNavigationMPR.Size.Height / 4) * 3).Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, PathNavigationMPR.Size.Height / 4).ClickAndHold()
                    .MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, (PathNavigationMPR.Size.Height / 4) * 3).Release().Build().Perform();
                Thread.Sleep(10000);
                Nav1AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                PathMPRAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathCurvedAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                Path3DAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Comparison1 = Nav1AnnotationValAfter.Equals(Nav1AnnotationValBefore);
                Comparison2 = Nav2AnnotationValAfter.Equals(Nav2AnnotationValBefore);
                Comparison3 = Nav3AnnotationValAfter.Equals(Nav3AnnotationValBefore);
                Comparison4 = PathMPRAnnotationValAfter.Equals(PathMPRAnnotationValBefore);
                Comparison5 = PathCurvedAnnotationValAfter.Equals(PathCurvedAnnotationValBefore);
                Comparison6 = Path3DAnnotationValAfter.Equals(Path3DAnnotationValBefore);
                if (!Comparison1 && !Comparison2 && !Comparison3 && !Comparison4 && !Comparison5 && Comparison6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(10000);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(10000);
                String Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                String NavigationPathMPRLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String CurvedMPRLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CurvedMPR);
                Comparison1 = Navigation1LocVal.Equals(objlocval3);
                Comparison2 = Navigation2LocVal.Equals(objlocval3);
                Comparison3 = Navigation3LocVal.Equals(objlocval3);
                Comparison4 = NavigationPathMPRLocVal.Equals(objlocval3);
                Comparison5 = CurvedMPRLocVal.Equals(objlocval3);
                Comparison6 = NavigationPath3DLocVal.Equals(objlocval3);
                if (Comparison1 && Comparison2 && Comparison3 && Comparison4 && Comparison5 && Comparison6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                res = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                bool step14 = z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval1);
                if (step14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int BlueColorValBefore_15 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 151, 0, 0, 255, 2);
                Thread.Sleep(5000);
                z3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                Thread.Sleep(20000);
                bool check15 = z3dvp.checkerrormsg();
                int BlueColorValAfter_15 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 152, 0, 0, 255, 2);
                if (BlueColorValAfter_15 != BlueColorValBefore_15 && check15 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 16
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(10000);
                bool check16 = z3dvp.checkerrormsg();
                int BlueColorValAfter_16 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 161, 0, 0, 255, 2);
                if (BlueColorValAfter_16 != BlueColorValAfter_15 && check16 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 17
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Thread.Sleep(2000);
                z3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(10000);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(10000);
                int ColorValBefore_17 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 171, 0, 0, 255, 2);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                Thread.Sleep(2000);
                if(browserName.Contains("internet"))
                    z3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 25, (Navigation2.Size.Height / 4) + 10);
                else
                    z3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 15, (Navigation2.Size.Height / 4) + 10);
                Thread.Sleep(10000);
                bool check17 = z3dvp.checkerrormsg();
                int ColorValAfter_17 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 172, 0, 0, 255, 2);
                if (ColorValAfter_17 != ColorValBefore_17 && check17 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 18
                LocationValueMPRBefore = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                res = z3dvp.ScrollInView(viewport: BluRingZ3DViewerPage.MPRPathNavigation, ScrollDirection: "down", scrolllevel: 30, Thickness: "n");
                LocationValueMPRAfter = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (LocationValueMPRAfter != LocationValueMPRBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                res = z3dvp.select3DTools(Z3DTools.Window_Level);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                PathNavigationMPR = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Nav1AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                PathMPRAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathCurvedAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                Path3DAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, PathNavigationMPR.Size.Height / 4).ClickAndHold()
                    .MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, (PathNavigationMPR.Size.Height / 4) * 3).Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, PathNavigationMPR.Size.Height / 4).ClickAndHold()
                    .MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, (PathNavigationMPR.Size.Height / 4) * 3).Release().Build().Perform();
                Thread.Sleep(10000);
                Nav1AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                PathMPRAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathCurvedAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                Path3DAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Comparison1 = Nav1AnnotationValAfter.Equals(Nav1AnnotationValBefore);
                Comparison2 = Nav2AnnotationValAfter.Equals(Nav2AnnotationValBefore);
                Comparison3 = Nav3AnnotationValAfter.Equals(Nav3AnnotationValBefore);
                Comparison4 = PathMPRAnnotationValAfter.Equals(PathMPRAnnotationValBefore);
                Comparison5 = PathCurvedAnnotationValAfter.Equals(PathCurvedAnnotationValBefore);
                Comparison6 = Path3DAnnotationValAfter.Equals(Path3DAnnotationValBefore);
                if (!Comparison1 && !Comparison2 && !Comparison3 && !Comparison4 && !Comparison5 && Comparison6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 21
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(10000);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(2000);
                String Navigation1LocVal21 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Navigation2LocVal21 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Navigation3LocVal21 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                String NavigationPathMPRLocVal21 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocVal21 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String CurvedMPRLocVal21 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CurvedMPR);
                Comparison1 = Navigation1LocVal21.Equals(objlocval3);
                Comparison2 = Navigation2LocVal21.Equals(objlocval3);
                Comparison3 = Navigation3LocVal21.Equals(objlocval3);
                Comparison4 = NavigationPathMPRLocVal21.Equals(objlocval3);
                Comparison5 = CurvedMPRLocVal21.Equals(objlocval3);
                Comparison6 = NavigationPath3DLocVal21.Equals(objlocval3);
                if (Comparison1 && Comparison2 && Comparison3 && Comparison4 && Comparison5 && Comparison6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 22
                res = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 23
                res = z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval2);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 24
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                int ColorValBefore_24 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 241, 0, 0, 255, 2);
                Thread.Sleep(10000);
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) - 50, (Navigation1.Size.Height / 4) - 50).Click().Build().Perform();
                Thread.Sleep(10000);
                Thread.Sleep(5000);
                bool check24 = z3dvp.checkerrormsg();
                int ColorValAfter_24 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 242, 0, 0, 255, 2);
                if (ColorValAfter_24 != ColorValBefore_24 && check24 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 25
                z3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 40, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(10000);
                bool check25 = z3dvp.checkerrormsg();
                int ColorValAfter_25 = z3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 251, 0, 0, 255, 2);
                if (ColorValAfter_25 != ColorValBefore_24 && check25 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 26
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(10000);
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                z3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) - 40, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(10000);
                int ColorValBefore_26 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 261, 0, 0, 255, 2);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                z3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 50, (Navigation2.Size.Height / 4) - 30);
                Thread.Sleep(10000);
                bool check26 = z3dvp.checkerrormsg();
                int ColorValAfter_26 = z3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 262, 0, 0, 255, 2);
                if (ColorValAfter_26 != ColorValBefore_26 && check26 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to find path");

                //step 27
                LocationValueMPRBefore = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                res = z3dvp.ScrollInView(viewport: BluRingZ3DViewerPage.MPRPathNavigation, ScrollDirection: "down", scrolllevel: 30, Thickness: "n");
                LocationValueMPRAfter = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (LocationValueMPRAfter != LocationValueMPRBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 28
                res = z3dvp.select3DTools(Z3DTools.Window_Level);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 29
                PathNavigationMPR = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Nav1AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3AnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                PathMPRAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathCurvedAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                Path3DAnnotationValBefore = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, PathNavigationMPR.Size.Height / 4).ClickAndHold()
                    .MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, (PathNavigationMPR.Size.Height / 4) * 3).Release().Build().Perform();
                Thread.Sleep(10000);
                new Actions(Driver).MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, PathNavigationMPR.Size.Height / 4).ClickAndHold()
                   .MoveToElement(PathNavigationMPR, PathNavigationMPR.Size.Width / 4, (PathNavigationMPR.Size.Height / 4) * 3).Release().Build().Perform();
                Thread.Sleep(10000);
                Nav1AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3AnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree);
                PathMPRAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                PathCurvedAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.CurvedMPR);
                Path3DAnnotationValAfter = z3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Comparison1 = Nav1AnnotationValAfter.Equals(Nav1AnnotationValBefore);
                Comparison2 = Nav2AnnotationValAfter.Equals(Nav2AnnotationValBefore);
                Comparison3 = Nav3AnnotationValAfter.Equals(Nav3AnnotationValBefore);
                Comparison4 = PathMPRAnnotationValAfter.Equals(PathMPRAnnotationValBefore);
                Comparison5 = PathCurvedAnnotationValAfter.Equals(PathCurvedAnnotationValBefore);
                Comparison6 = Path3DAnnotationValAfter.Equals(Path3DAnnotationValBefore);
                if (!Comparison1 && !Comparison2 && !Comparison3 && !Comparison4 && !Comparison5 && Comparison6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 30
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                z3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String Navigation1LocVal30 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Navigation2LocVal30 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Navigation3LocVal30 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                String NavigationPathMPRLocVal30 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocVal30 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String CurvedMPRLocVal30 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CurvedMPR);
                Comparison1 = Navigation1LocVal30.Equals(objlocval3);
                Comparison2 = Navigation2LocVal30.Equals(objlocval3);
                Comparison3 = Navigation3LocVal30.Equals(objlocval3);
                Comparison4 = NavigationPathMPRLocVal30.Equals(objlocval3);
                Comparison5 = CurvedMPRLocVal30.Equals(objlocval3);
                Comparison6 = NavigationPath3DLocVal30.Equals(objlocval3);
                if (Comparison1 && Comparison2 && Comparison3 && Comparison4 && Comparison5 && Comparison6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_163308(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
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
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];

                //step 01 & 02
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Failed to open study in Test_163308 Step 02");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 03
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Curve_Drawing_Tool_1_Manual, 50, 50, 100, testid, ExecutedSteps);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                    if (!res)
                        result.steps[ExecutedSteps].StepFail();
                    else
                        result.steps[ExecutedSteps].StepPass();
                }

                //step 04
                int BluColorRegion = brz3dvp.LevelOfSelectedColor(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 2, 0, 0, 255, 2);
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage._3DPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 05
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2).ClickAndHold()
                    .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2 + 20).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //step 06
                res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage._3DPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 07
                String MPROrientationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, (PathNavigation3D.Size.Height / 2) - 20).ClickAndHold()
                    .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, (PathNavigation3D.Size.Height / 2) + 30).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                String MPROrientationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                if (MPROrientationBefore.Equals(MPROrientationAfter))
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 08
                res = brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage._3DPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 09
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, (PathNavigation3D.Size.Height / 2) - 20).ClickAndHold()
                    .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2 + 30).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //step 10
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.Preset1, BluRingZ3DViewerPage.Preset);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                    if (!res)
                        result.steps[ExecutedSteps].StepFail();
                    else
                        result.steps[ExecutedSteps].StepPass();
                }

                //step 11
                res = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage._3DPathNavigation);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 12
                String imagename = testid + ExecutedSteps + 2;
                String Step12_imgLocation = Config.downloadpath + "\\" + imagename + ".jpg";
                if (File.Exists(Step12_imgLocation))
                    File.Delete(Step12_imgLocation);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                brz3dvp.downloadImageForViewport(imagename, "jpg");
                PageLoadWait.WaitForFrameLoad(5);
                if (File.Exists(Step12_imgLocation))
                {
                    res = brz3dvp.CompareDownloadimage(Step12_imgLocation);
                    if (res)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);
                PageLoadWait.WaitForFrameLoad(5);
                res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                //step 14
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 22, (Navigation1.Size.Height / 4) - 60);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 22, (Navigation1.Size.Height / 4) - 20);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check14 = brz3dvp.checkerrormsg();
                    if (check14)
                        throw new Exception("Failed to find path");
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                    if (!res)
                        result.steps[ExecutedSteps].StepFail();
                    else
                        result.steps[ExecutedSteps].StepPass();
                }

                //step 15
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage._3DPathNavigation);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in selecting Zoom Tool in 3DPathNavigation");
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                    new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2).ClickAndHold()
                    .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2 + 20).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed in comparing 3DPathNavigation after applying Zoom tool");
                        result.steps[ExecutedSteps].StepFail();
                    }
                    else
                    {
                        res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage._3DPathNavigation);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed in selecting rotate click center in 3DPathNavigation");
                            result.steps[ExecutedSteps].StepFail();
                        }
                        else
                        {
                            MPROrientationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                            PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                            new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 4, PathNavigation3D.Size.Height / 2).ClickAndHold()
                                .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width, PathNavigation3D.Size.Height / 2).Release().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(5);
                            MPROrientationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                            if (MPROrientationAfter == MPROrientationBefore)
                            {
                                Logger.Instance.ErrorLog("Failed in comparing orientation markers in 3DPathNavigation after applying rotation click center tool");
                                result.steps[ExecutedSteps].StepFail();
                            }
                            else
                            {
                                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                                res = brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage._3DPathNavigation);
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed in selecting window level tool in 3DPathNavigation");
                                    result.steps[ExecutedSteps].StepFail();
                                }
                                else
                                {
                                    PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                                    new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2).ClickAndHold()
                                        .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2 + 100).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                                    if (!res)
                                    {
                                        Logger.Instance.ErrorLog("Failed in comparing 3DPathNavigation after applying window level tool");
                                        result.steps[ExecutedSteps].StepFail();
                                    }
                                    else
                                    {
                                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                                        res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.Preset1, BluRingZ3DViewerPage.Preset);
                                        if (!res)
                                        {
                                            Logger.Instance.ErrorLog("Failed in applying preset over 3DPathNavigation");
                                            result.steps[ExecutedSteps].StepFail();
                                        }
                                        else
                                        {
                                            PageLoadWait.WaitForFrameLoad(5);
                                            res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                                            if (!res)
                                            {
                                                Logger.Instance.ErrorLog("Failed in comparing 3DPathNavigation after applying reset");
                                                result.steps[ExecutedSteps].StepFail();
                                            }
                                            else
                                            {
                                                res = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage._3DPathNavigation);
                                                if (!res)
                                                {
                                                    Logger.Instance.ErrorLog("Failed in selecting download tool in 3DPathNavigation");
                                                    result.steps[ExecutedSteps].StepFail();
                                                }
                                                else
                                                {
                                                    String imagename15_2 = testid + ExecutedSteps + 2;
                                                    String Step15_imgLocation2 = Config.downloadpath + "\\" + imagename15_2 + ".jpg";
                                                    if (File.Exists(Step15_imgLocation2))
                                                        File.Delete(Step15_imgLocation2);
                                                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation)).Click().Build().Perform();
                                                    PageLoadWait.WaitForFrameLoad(2);
                                                    brz3dvp.downloadImageForViewport(imagename15_2, "jpg");
                                                    PageLoadWait.WaitForFrameLoad(2);
                                                    if (!File.Exists(Step15_imgLocation2))
                                                    {
                                                        Logger.Instance.ErrorLog("Downloaded file doesnt exist in the specified location for 3DPathNavigation");
                                                        result.steps[ExecutedSteps].StepFail();
                                                    }
                                                    else
                                                    {
                                                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                                                        res = brz3dvp.CompareDownloadimage(Step15_imgLocation2);
                                                        if (res)
                                                        {
                                                            res = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(10);
                                                            brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(5);
                                                            res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                                                            if (!res)
                                                            {
                                                                Logger.Instance.ErrorLog("Failed in comparing 3DPathNavigation after applying reset");
                                                                result.steps[ExecutedSteps].StepFail();
                                                            }
                                                            else
                                                                result.steps[ExecutedSteps].StepPass();
                                                        }
                                                        else
                                                        {
                                                            Logger.Instance.ErrorLog("Failed in comparing 3DPathNavigation after download tool is applied");
                                                            result.steps[ExecutedSteps].StepFail();
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
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[ExecutedSteps].StepFail();
                else
                {
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval2);
                    int ColorValBefore_20 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                    PageLoadWait.WaitForFrameLoad(10);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 50, (Navigation1.Size.Height / 4) - 50);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName(Locators.CssSelector.LoadingIcon)));
                    Thread.Sleep(3000);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 35, (Navigation1.Size.Height / 4) - 35);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check16 = brz3dvp.checkerrormsg();
                    if (check16)
                        throw new Exception("Failed to find path");
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                    if (!res)
                        result.steps[ExecutedSteps].StepFail();
                    else
                        result.steps[ExecutedSteps].StepPass();
                }

                //step 17
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                res = brz3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage._3DPathNavigation);
                if (!res)
                {
                    Logger.Instance.ErrorLog("Failed in selecting Zoom Tool in 3DPathNavigation");
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                    new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2).ClickAndHold()
                    .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2 + 20).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                    if (!res)
                    {
                        Logger.Instance.ErrorLog("Failed while applying Zoom tool in 3DPathNavigation");
                        result.steps[ExecutedSteps].StepFail();
                    }
                    else
                    {
                        res = brz3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage._3DPathNavigation);
                        if (!res)
                        {
                            Logger.Instance.ErrorLog("Failed in selecting Rotate Click center tool in 3DPathNavigation");
                            result.steps[ExecutedSteps].StepFail();
                        }
                        else
                        {
                            MPROrientationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                            PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                            new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 4, PathNavigation3D.Size.Height / 2).ClickAndHold()
                                .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width, PathNavigation3D.Size.Height / 2).Release().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(5);
                            MPROrientationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                            if (MPROrientationAfter == MPROrientationBefore)
                            {
                                Logger.Instance.ErrorLog("Failed while applying rotation tool click center in 3DPathNavigation");
                                result.steps[ExecutedSteps].StepFail();
                            }
                            else
                            {
                                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                                res = brz3dvp.select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage._3DPathNavigation);
                                if (!res)
                                {
                                    Logger.Instance.ErrorLog("Failed in selecting window level tool in 3DPathNavigation");
                                    result.steps[ExecutedSteps].StepFail();
                                }
                                else
                                {
                                    PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                                    new Actions(Driver).MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2).ClickAndHold()
                                        .MoveToElement(PathNavigation3D, PathNavigation3D.Size.Width / 2, PathNavigation3D.Size.Height / 2 + 100).Release().Build().Perform();
                                    PageLoadWait.WaitForFrameLoad(5);
                                    res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                                    if (!res)
                                    {
                                        Logger.Instance.ErrorLog("Failed in comparing window level applied 3DPathNavigation");
                                        result.steps[ExecutedSteps].StepFail();
                                    }
                                    else
                                    {
                                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                                        res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.Preset1, BluRingZ3DViewerPage.Preset);
                                        if (!res)
                                        {
                                            Logger.Instance.ErrorLog("Failed while selecting preset in 3DPathNavigation");
                                            result.steps[ExecutedSteps].StepFail();
                                        }
                                        else
                                        {
                                            PageLoadWait.WaitForFrameLoad(5);
                                            res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                                            if (!res)
                                            {
                                                Logger.Instance.ErrorLog("Failed in comparing 3DPathNavigation after applying preset");
                                                result.steps[ExecutedSteps].StepFail();
                                            }
                                            else
                                            {
                                                res = brz3dvp.select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage._3DPathNavigation);
                                                if (!res)
                                                {
                                                    Logger.Instance.ErrorLog("Failed in selecting download tool in 3DPathNavigation");
                                                    result.steps[ExecutedSteps].StepFail();
                                                }
                                                else
                                                {
                                                    String imagename15_2 = testid + ExecutedSteps + 2;
                                                    String Step15_imgLocation2 = Config.downloadpath + "\\" + imagename15_2 + ".jpg";
                                                    if (File.Exists(Step15_imgLocation2))
                                                        File.Delete(Step15_imgLocation2);
                                                    new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).Click().Build().Perform();
                                                    PageLoadWait.WaitForFrameLoad(2);
                                                    brz3dvp.downloadImageForViewport(imagename15_2, "jpg");
                                                    Thread.Sleep(10000);
                                                    if (!File.Exists(Step15_imgLocation2))
                                                    {
                                                        Logger.Instance.ErrorLog("Downloaded 3DPathNavigation file doent exist");
                                                        result.steps[ExecutedSteps].StepFail();
                                                    }
                                                    else
                                                    {
                                                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                                                        res = brz3dvp.CompareDownloadimage(Step15_imgLocation2);
                                                        if (res)
                                                        {
                                                            brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(10);
                                                            brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);
                                                            PageLoadWait.WaitForFrameLoad(5);
                                                            res = CompareImage(result.steps[ExecutedSteps], brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation));
                                                            if (!res)
                                                            {
                                                                Logger.Instance.ErrorLog("Failed in comparing 3DPathNavigation after applying reset");
                                                                result.steps[ExecutedSteps].StepFail();
                                                            }
                                                            else
                                                                result.steps[ExecutedSteps].StepPass();
                                                        }
                                                        else
                                                        {
                                                            Logger.Instance.ErrorLog("Failed in comparing downloaded 3DPathNavigation image");
                                                            result.steps[ExecutedSteps].StepFail();
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

        public TestCaseResult Test_170140(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
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
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];
                String InitialLocVal = objTestRequirement.Split('|')[2];
                IWebElement Navigation1, Navigation2;
                int meandiff;

                //step 01
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Study failed to load");
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 03
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                int ColorValBefore_23 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 231, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                bool check23 = brz3dvp.checkerrormsg();
                if (check23)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_23 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 232, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_23 != ColorValBefore_23)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check24 = brz3dvp.checkerrormsg();
                if (check24)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_24 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 24, 0, 0, 255, 2);
                if (ColorValAfter_24 != ColorValAfter_23)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                Thread.Sleep(5000);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Actions action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_25 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 251, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 25, (Navigation2.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check25_1 = brz3dvp.checkerrormsg();
                if (check25_1)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_25 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 252, 0, 0, 255, 2);
                if (ColorValAfter_25 != ColorValBefore_25)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                res = brz3dvp.ControlFlipStatus(BluRingZ3DViewerPage._3DPathNavigation);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                if(browserName.Contains("firefox"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 25);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 20);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 26, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                if (browserName.Contains("firefox"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 25);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 20);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                String Navigtaion1LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                String Navigation2LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                String Navigation3LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                String MPRNLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, "Loc:");
                String PN3DLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, "Loc:");
                if (Navigtaion1LocVal == InitialLocVal && Navigation2LocVal == InitialLocVal && Navigation3LocVal == InitialLocVal && MPRNLocVal == InitialLocVal && PN3DLocVal == InitialLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColorValBefore_33 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 321, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                bool check32 = brz3dvp.checkerrormsg();
                if (check32)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_33 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 322, 0, 0, 255, 2, isMoveCursor: true);
                if (ColorValAfter_33 != ColorValBefore_33)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check33 = brz3dvp.checkerrormsg();
                if (check33)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_34 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 33, 0, 0, 255, 2);
                if (ColorValAfter_34 != ColorValBefore_33)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(5000);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValBefore_35 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 341, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 25, (Navigation2.Size.Height / 4) - 35);
                PageLoadWait.WaitForFrameLoad(10);
                bool check34_1 = brz3dvp.checkerrormsg();
                if (check34_1)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_35 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 342, 0, 0, 255, 2);
                if (ColorValAfter_35 != ColorValBefore_35)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                res = brz3dvp.ControlFlipStatus(BluRingZ3DViewerPage._3DPathNavigation, check: false);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                if (browserName.Contains("firefox"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 25);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 20);
                if (!res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 26, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                if (browserName.Contains("firefox"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 25);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 20);
                if (!res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                Navigtaion1LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                Navigation2LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                Navigation3LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                MPRNLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, "Loc:");
                PN3DLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, "Loc:");
                if (Navigtaion1LocVal == InitialLocVal && Navigation2LocVal == InitialLocVal && Navigation3LocVal == InitialLocVal && MPRNLocVal == InitialLocVal && PN3DLocVal == InitialLocVal)
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

        public TestCaseResult Test_170141(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
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
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];
                String InitialLocVal = objTestRequirement.Split('|')[2];
                IWebElement Navigation1, Navigation2;
                int meandiff;

                //step 01
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 02
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Study failed to load");
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 03
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 04
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(2000);
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval2);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool pan5 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Pan, 50, 50, 70, movement: "positive");
                Logger.Instance.InfoLog("Pan tool application over navigation 1 in step 5 is : " + pan5.ToString());
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool tool5 = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                int ColorValBefore_41 = 0;
                if (!tool5)
                {
                    Logger.Instance.InfoLog("Failed to select auto colon tool");
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    ColorValBefore_41 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 411, 0, 0, 255, 2);
                    PageLoadWait.WaitForFrameLoad(10);
                    Thread.Sleep(5000);
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 50, (Navigation1.Size.Height / 4) - 50);
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check41 = brz3dvp.checkerrormsg();
                    if (check41)
                        throw new Exception("Failed to find path");
                    PageLoadWait.WaitForFrameLoad(10);
                    Thread.Sleep(5000);
                    int ColorValAfter_41 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 412, 0, 0, 255, 2);
                    if (ColorValAfter_41 != ColorValBefore_41)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //step 06
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 40, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check42 = brz3dvp.checkerrormsg();
                if (check42)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_42 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2);
                if (ColorValAfter_42 != ColorValBefore_41)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                Actions action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree)).Build().Perform();
                Thread.Sleep(2000);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) - 40, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValBefore_43 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 431, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 50, (Navigation2.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                bool check43 = brz3dvp.checkerrormsg();
                if (check43)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_43 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 432, 0, 0, 255, 2);
                if (ColorValAfter_43 != ColorValBefore_43)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                res = brz3dvp.ControlFlipStatus(BluRingZ3DViewerPage._3DPathNavigation);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 09
                if(browserName.Contains("ie"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 18, zoom: false);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 27, zoom: false);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                if (browserName.Contains("ie"))
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 20, Thickness: "n");
                else
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 28, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                if (browserName.Contains("ie"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 18, zoom: false);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 27, zoom: false);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                String Navigtaion1LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                String Navigation2LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                String Navigation3LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                String MPRNLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, "Loc:");
                String PN3DLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, "Loc:");
                if (Navigtaion1LocVal == InitialLocVal && Navigation2LocVal == InitialLocVal && Navigation3LocVal == InitialLocVal && MPRNLocVal == InitialLocVal && PN3DLocVal == InitialLocVal)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrollTill: objlocval2);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Pan, 50, 50, 70, movement: "positive");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                int ColorValBefore_50 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 501, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 50, (Navigation1.Size.Height / 4) - 50);
                PageLoadWait.WaitForFrameLoad(10);
                bool check50 = brz3dvp.checkerrormsg();
                if (check50)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_50 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 502, 0, 0, 255, 2);
                if (ColorValAfter_50 != ColorValBefore_50)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) - 40, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check51 = brz3dvp.checkerrormsg();
                if (check51)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                int ColorValAfter_51 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                if (ColorValAfter_51 != ColorValAfter_50)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 16
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                action = new Actions(Driver);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) - 40, (Navigation1.Size.Height / 4) - 20);
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int ColorValBefore_52 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 521, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 50, (Navigation2.Size.Height / 4) - 30);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                bool check52 = brz3dvp.checkerrormsg();
                if (check52)
                    throw new Exception("Failed to find path");
                Thread.Sleep(5000);
                int ColorValAfter_52 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 522, 0, 0, 255, 2);
                if (ColorValAfter_52 != ColorValBefore_52)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 17
                res = brz3dvp.ControlFlipStatus(BluRingZ3DViewerPage._3DPathNavigation, check: false);
                if (res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                if(browserName.Contains("ie") || browserName.Contains("firefox"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 17, zoom: false);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage._3DPathNavigation, "down", 20, false);
                if (!res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                if (browserName.Contains("ie") || browserName.Contains("firefox"))
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 18, Thickness: "n");
                else
                    brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 22, Thickness: "n");
                PageLoadWait.WaitForFrameLoad(2);
                if (browserName.Contains("ie") || browserName.Contains("firefox"))
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 17, zoom: false);
                else
                    res = brz3dvp.ScrollAndCheckOrientation(BluRingZ3DViewerPage.MPRPathNavigation, "down", 20, zoom: false);
                if (!res)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 20
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                Navigtaion1LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationone, "Loc:");
                Navigation2LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationtwo, "Loc:");
                Navigation3LocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.Navigationthree, "Loc:");
                MPRNLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage.MPRPathNavigation, "Loc:");
                PN3DLocVal = brz3dvp.GetTopLeftAnnotationValue(BluRingZ3DViewerPage._3DPathNavigation, "Loc:");
                if (Navigtaion1LocVal == InitialLocVal && Navigation2LocVal == InitialLocVal && Navigation3LocVal == InitialLocVal && MPRNLocVal == InitialLocVal && PN3DLocVal == InitialLocVal)
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

        public TestCaseResult Test_170142(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Nav1Location = objTestRequirement.Split('|')[0];
            String NavLocation1 = objTestRequirement.Split('|')[1];
            String NavLocation11 = objTestRequirement.Split('|')[2];
            String NavLocation3 = objTestRequirement.Split('|')[3];
            String NavLocation33 = objTestRequirement.Split('|')[4];
            String NavLocation60 = objTestRequirement.Split('|')[5];

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From iCA, Load a study in the 3D viewer 1.Navigate to 3D tab.
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step2::Select the "Curved MPR viewing mode" from 3D dropdown.
                bool step2_1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                if (step2_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to open study in Curved MPR Layout");

                //step 03
                bool CurveDrawingAutoVessel = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (CurveDrawingAutoVessel)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 04::Click on navigation control 1 and scroll up until the top of the aorta is visible.
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(2000);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, Nav1Location);
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == Nav1Location && LocationAfterScroll != InitialLocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 05::Add a point at the top of the aorta displayed on navigation control 1.
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 161, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                bool check16 = Z3dViewer.checkerrormsg();
                if (check16)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 162, 0, 0, 255, 2);
                if (ColorValAfter_5 != ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 06::Add a 2nd point along the aorta displayed on navigation control 1.
                Z3dViewer.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check17 = Z3dViewer.checkerrormsg();
                if (check17)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_6 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 172, 0, 0, 255, 2);
                if (ColorValAfter_5 != ColorValAfter_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 07::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_18 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 186, 0, 0, 255, 2);
                Thread.Sleep(2000);
                Z3dViewer.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 22, (Navigation2.Size.Height / 4) + 20);
                PageLoadWait.WaitForFrameLoad(10);
                bool check18 = Z3dViewer.checkerrormsg();
                if (check18)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_18 = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 187, 0, 0, 255, 2);
                if (ColorValAfter_18 != ColorValBefore_18)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 08::Click on the 3D path navigation control and scroll along part of the path that was generated.
                String BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement _3DPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(3000);
                String AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1, pixelTolerance: 10) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 09::Note the orientation markers.Select the rotate tool from 3D toolbox and perform a 180 degrees image plane rotation going clockwise on the 3D path navigation control
                Z3dViewer.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage._3DPathNavigation);
                String Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.Performdragdrop(ThreeDPathNav, ThreeDPathNav.Size.Width - 10, ThreeDPathNav.Size.Height / 2, ThreeDPathNav.Size.Width / 6, ThreeDPathNav.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                String After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                IWebElement wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 10::Perform a 360 degrees image plane rotation going counter clockwise but this time on the MPR path navigation control.
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(MprPathNav, (MprPathNav.Size.Width / 4) - 30, MprPathNav.Size.Height / 2).ClickAndHold()
                            .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, ((MprPathNav.Size.Height / 4) * 3) - 30)
                            .MoveToElement(MprPathNav, ((MprPathNav.Size.Width / 4) * 3) - 30, MprPathNav.Size.Height / 2)
                            .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30)
                            .MoveToElement(MprPathNav, (MprPathNav.Size.Width / 4) - 30, MprPathNav.Size.Height / 2).Release().Build().Perform();
                Thread.Sleep(5000);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Logger.Instance.InfoLog("Step 10 Before_3dpathnav " + Before_3dpathnav + " Before_MprPathNav : " + Before_MprPathNav + " After_3dpathnav : " + After_3dpathnav + " After_MprPathNav : " + After_MprPathNav);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 11::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement MPRPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, scrolllevel: 70, Thickness: "n");
                Thread.Sleep(5000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1, pixelTolerance: 10) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 12::Note the orientation markers.Select the rotate tool from 3D toolbox and perform a 180 degrees image plane rotation going clockwise on the MPR path navigation control
                Z3dViewer.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage._3DPathNavigation);
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.Performdragdrop(MprPathNav, MprPathNav.Size.Width / 4, MprPathNav.Size.Height / 2, (MprPathNav.Size.Width / 4) * 3, MprPathNav.Size.Height / 2);
                PageLoadWait.WaitForFrameLoad(10);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Logger.Instance.InfoLog("Step23 " + "Before_3dpathnav :" + Before_3dpathnav + "Before_MprPathNav :" + Before_MprPathNav + "After_3dpathnav :" + After_3dpathnav + "After_MprPathNav :" + After_MprPathNav);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 13::Perform a 360 degrees image plane rotation going counter clockwise but this time on the 3D path navigation control.
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav, (ThreeDPathNav.Size.Width / 4) - 30, ThreeDPathNav.Size.Height / 2).ClickAndHold()
                            .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, ((ThreeDPathNav.Size.Height / 4) * 3) - 30)
                            .MoveToElement(ThreeDPathNav, ((ThreeDPathNav.Size.Width / 4) * 3) - 30, ThreeDPathNav.Size.Height / 2)
                            .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 4) - 30)
                            .MoveToElement(ThreeDPathNav, (ThreeDPathNav.Size.Width / 4) - 30, ThreeDPathNav.Size.Height / 2).Release().Build().Perform();
                Thread.Sleep(5000);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Logger.Instance.InfoLog("Step 13 " + "Before_3dpathnav :" + Before_3dpathnav + "Before_MprPathNav :" + Before_MprPathNav + "After_3dpathnav :" + After_3dpathnav + "After_MprPathNav :" + After_MprPathNav);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 14::Click the reset button.
                String BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                String AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_170143(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingZ3DViewerPage Z3dViewer = new BluRingZ3DViewerPage();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String ThumbnailImage = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Nav1Location = objTestRequirement.Split('|')[0];
            String NavLocation1 = objTestRequirement.Split('|')[1];
            String NavLocation11 = objTestRequirement.Split('|')[2];
            String NavLocation3 = objTestRequirement.Split('|')[3];
            String NavLocation33 = objTestRequirement.Split('|')[4];
            String NavLocation60 = objTestRequirement.Split('|')[5];

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step1::From iCA, Load a study in the 3D viewer 1.Navigate to 3D tab.
                login.DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //Step2::Select the "Curved MPR viewing mode" from 3D dropdown.
                bool step2_1 = Z3dViewer.searchandopenstudyin3D(PatientID, ThumbnailImage, BluRingZ3DViewerPage.CurvedMPR);
                if (step2_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to open study in Curved MPR Layout");

                //Step 3
                bool CurveDrawingAutoVissel = Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (CurveDrawingAutoVissel)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 4::Click on navigation control 1 and scroll up until the top of the colon is visible.
                IWebElement Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(3000);
                String InitialLocation = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.Navigationone, NavLocation60);
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == NavLocation60 && LocationAfterScroll != InitialLocation)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 5::Add a point at the beginning of the colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                int ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 281, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 91).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool check28 = Z3dViewer.checkerrormsg();
                if (check28)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_28 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 282, 0, 0, 255, 2);
                if (ColorValAfter_28 > ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 6::Add a 2nd point along the Colon displayed on navigation control 1.
                Thread.Sleep(5000);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                bool check29 = Z3dViewer.checkerrormsg();
                if (check29)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_29 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2);
                IWebElement ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                if (CompareImage(result.steps[ExecutedSteps], ThreeDPathNav) && ColorValAfter_29 > ColorValBefore)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 7::Add the 3rd point on the Colon below the 2nd point by clicking on “Navigation 2” image this time.
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Z3dViewer.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2 + 7, Navigation1.Size.Height / 2 + 119);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement Navigation2 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationtwo);//Config.MPRPathNavigation
                new Actions(Driver).MoveToElement(Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                ColorValBefore = Z3dViewer.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 301, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewer.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 33, Navigation2.Size.Height / 2 + 131);
                PageLoadWait.WaitForFrameLoad(10);
                bool check30 = Z3dViewer.checkerrormsg();
                if (check30)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_30 = Z3dViewer.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 30, 0, 0, 255, 2);
                if (ColorValAfter_30 > ColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 8::Click on the 3D path navigation control and scroll along part of the path that was generated.
                String BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement _3DPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 70, Thickness: "n");
                Thread.Sleep(2000);
                String AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1, pixelTolerance: 5) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 9::Note the orientation markers.Select the rotate tool from 3D toolbox and perform a 180 degrees off-path rotation going clockwise on the 3D path navigation control
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Z3dViewer.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                String Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 6, ThreeDPathNav.Size.Height / 2).ClickAndHold()
                    .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, ThreeDPathNav.Size.Height / 7)
                    .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width - 10, ThreeDPathNav.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 10::Perform a 360 degrees off-path rotation going counter clockwise but this time on the MPR path navigation control.
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(MprPathNav, (MprPathNav.Size.Width / 4) - 30, MprPathNav.Size.Height / 2).ClickAndHold()
                            .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, ((MprPathNav.Size.Height / 4) * 3) - 30)
                            .MoveToElement(MprPathNav, ((MprPathNav.Size.Width / 4) * 3) - 30, MprPathNav.Size.Height / 2)
                            .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, (MprPathNav.Size.Height / 4) - 30)
                            .MoveToElement(MprPathNav, (MprPathNav.Size.Width / 4) - 30, MprPathNav.Size.Height / 2).Release().Build().Perform();
                Thread.Sleep(5000);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 11::Click on the MPR path navigation control and scroll along part of the path that was generated.
                BeforeLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                IWebElement MPRPathNavigation = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewer.ScrollInView(BluRingZ3DViewerPage.MPRPathNavigation, scrolllevel: 70, Thickness: "n");
                Thread.Sleep(3000);
                AfterLocValue1 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                AfterLocValue2 = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Navigation1 = Z3dViewer.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 12::Note the orientation markers. Select the rotate tool from 3D toolbox and perform a 180 degrees off-path rotation going clockwise on the MPR path navigation control
                Z3dViewer.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.MPRPathNavigation);
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                MprPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                new Actions(Driver).MoveToElement(MprPathNav, MprPathNav.Size.Width / 6, MprPathNav.Size.Height / 2).ClickAndHold()
                     .MoveToElement(MprPathNav, MprPathNav.Size.Width / 2, MprPathNav.Size.Height / 7)
                     .MoveToElement(MprPathNav, MprPathNav.Size.Width - 20, MprPathNav.Size.Height / 2).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 13::Perform a 360 degrees off-path rotation going counter clockwise but this time on the 3D path navigation control.
                Before_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Before_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                ThreeDPathNav = Z3dViewer.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                new Actions(Driver).MoveToElement(ThreeDPathNav, (ThreeDPathNav.Size.Width / 4) - 30, ThreeDPathNav.Size.Height / 2).ClickAndHold()
                             .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, ((ThreeDPathNav.Size.Height / 4) * 3) - 30)
                             .MoveToElement(ThreeDPathNav, ((ThreeDPathNav.Size.Width / 4) * 3) - 30, ThreeDPathNav.Size.Height / 2)
                             .MoveToElement(ThreeDPathNav, ThreeDPathNav.Size.Width / 2, (ThreeDPathNav.Size.Height / 4) - 30)
                             .MoveToElement(ThreeDPathNav, (ThreeDPathNav.Size.Width / 4) - 30, ThreeDPathNav.Size.Height / 2).Release().Build().Perform();
                Thread.Sleep(2000);
                After_3dpathnav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                After_MprPathNav = Z3dViewer.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                wholpanel = Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont));
                if (CompareImage(result.steps[ExecutedSteps], wholpanel))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 14::Click the reset button.
                String BeforeNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewer.select3DTools(Z3DTools.Reset);
                Thread.Sleep(5000);
                String AfterNavigationLocVal = Z3dViewer.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (BeforeNavigationLocVal != AfterNavigationLocVal)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                Z3dViewer.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_170144(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            String ResetLoc = TestData[0];
            String AortaLoc = TestData[1];
            string ColonLoc = TestData[2];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Series should be loaded with out any errors
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 2
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (step1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to open study");

                //step: 3 - Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down.
                bool res17 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                IWebElement viewport = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (res17)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step: 4  -Click on navigation control 1 and scroll up until the top of the aorta is visible
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, scrolllevel: 34);
                PageLoadWait.WaitForFrameLoad(5);
                String locafter = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (locafter == AortaLoc)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 5::Add a point at the top of the aorta displayed on navigation control 1.
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                int BluColorBeforePoint = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 191, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 20, 30);
                PageLoadWait.WaitForFrameLoad(20);
                bool res19 = Z3dViewerPage.checkerrormsg();
                if (res19)
                    throw new Exception("Failed to find path");
                int BluColorAfterPoint1 = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 192, 0, 0, 255, 2);
                if (BluColorAfterPoint1 != BluColorBeforePoint)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 6:: Add a 2nd point along the aorta displayed on navigation control 1.
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 20, 60);
                PageLoadWait.WaitForFrameLoad(20);
                bool res20 = Z3dViewerPage.checkerrormsg();
                if (res20)
                    throw new Exception("Failed to find path");
                Thread.Sleep(10000);
                int BluColorAfterPoint2 = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                if (BluColorAfterPoint2 != BluColorAfterPoint1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 7::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 60);
                PageLoadWait.WaitForFrameLoad(10);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while performing click and hold");
                Thread.Sleep(2000);
                Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(15);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_6 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 20, 90);
                PageLoadWait.WaitForFrameLoad(20);
                bool res21 = Z3dViewerPage.checkerrormsg();
                if (res21)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                int ColorValAfter_6 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && ColorValAfter_6 != ColorValBefore_6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 8::Click on the 3D path navigation control and scroll along part of the path that was generated.
                String BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 52, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                String AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 9 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 60);
                PageLoadWait.WaitForFrameLoad(5);
                int Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                int Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                int Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                int Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                int Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                int Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                int PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                int PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                int PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                int Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                int Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                int Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                int PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                int PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                int PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 10::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string Step24BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step24BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 55, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step24AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step24AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step24BeforeLocValue1 != Step24AfterLocValue1 && Step24BeforeLocValue2 != Step24AfterLocValue2 && Step24AfterLocValue1 == Step24AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 11- Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 30);
                Thread.Sleep(2000);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 12::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string Step26BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step26BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "up", scrolllevel: 49, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step26AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step26AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step26BeforeLocValue1 != Step26AfterLocValue1 && Step26BeforeLocValue2 != Step26AfterLocValue2 && Step26AfterLocValue1 == Step26AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 13 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 60);
                PageLoadWait.WaitForFrameLoad(5);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 14::Click on the 3D path navigation control and scroll along part of the path that was generated.
                string Step28BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step28BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "up", scrolllevel: 54, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step28AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step28AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step28BeforeLocValue1 != Step28AfterLocValue1 && Step28BeforeLocValue2 != Step28AfterLocValue2 && Step28AfterLocValue1 == Step28AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 15 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Z3dViewerPage.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 20, 90);
                PageLoadWait.WaitForFrameLoad(5);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step: 16 - Click and hold Right+left mouse button. Drag the image in MPR path navigation and 3D path navigation controls
                IWebElement Navigation3DPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement NavigationMPRPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                System.Drawing.Point location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Thread.Sleep(5000);
                String step30before1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String step30After1 = null, step30After2 = null;
                for (int i = 0; i < 3; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(1000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(3000);
                step30After1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.MPRPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Thread.Sleep(5000);
                String step30before2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 3; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(1000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(3000);
                step30After2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (step30before1 != step30After1 && step30before2 != step30After2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step: 17 - Select the reset button from the toolbar
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String step31_1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String step31_2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String step31_3 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                String step31_4 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String step31_5 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String step31_6 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CurvedMPR);
                if (step31_1.Contains(ResetLoc) && step31_2.Contains(ResetLoc) && step31_3.Contains(ResetLoc) && step31_4.Contains(ResetLoc) && step31_5.Contains(ResetLoc) && step31_6.Contains(ResetLoc))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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

        public TestCaseResult Test_170145(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            String ResetLoc = TestData[0];
            String AortaLoc = TestData[1];
            string ColonLoc = TestData[2];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step:1 - Series should be loaded with out any errors
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step 2
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (step1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Failed to open study");

                //step: 3 - Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down.
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(3000);
                bool res32 = Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationone);
                if (res32)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step: 4  -Click on navigation control 1 and scroll up until the top of the aorta is visible
                bool scroll4 = Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, ColonLoc, scrolllevel: 60);
                PageLoadWait.WaitForFrameLoad(5);
                String locafter33 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (scroll4 && locafter33 == ColonLoc)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 5::Add a point at the top of the aorta displayed on navigation control 1.
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int BluColorBeforePoint = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 341, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2, ((navigation1.Size.Height / 4) * 3));
                PageLoadWait.WaitForFrameLoad(20);
                bool res34 = Z3dViewerPage.checkerrormsg();
                if (res34)
                    throw new Exception("Failed to find path");
                int BluColorAfterPoint1 = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 342, 0, 0, 255, 2);
                if (BluColorAfterPoint1 > BluColorBeforePoint)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 6:: Add a 2nd point along the aorta displayed on navigation control 1.
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                BluColorAfterPoint1 = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 351, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2, ((navigation1.Size.Height / 4) * 3) + 30);
                PageLoadWait.WaitForFrameLoad(20);
                bool res35 = Z3dViewerPage.checkerrormsg();
                if (res35)
                    throw new Exception("Failed to find path");
                Thread.Sleep(10000);
                int BluColorAfterPoint2 = Z3dViewerPage.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 352, 0, 0, 255, 2);
                if (BluColorAfterPoint2 > BluColorAfterPoint1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 7::Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Thread.Sleep(3000);
                Z3dViewerPage.MoveClickAndHold(navigation1, navigation1.Size.Width / 2, ((navigation1.Size.Height / 4) * 3) + 30);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValBefore_6 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 6, 0, 0, 255, 2);
                Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 30, ((Navigation2.Size.Height / 4) * 3) + 45);
                PageLoadWait.WaitForFrameLoad(20);
                bool res36 = Z3dViewerPage.checkerrormsg();
                if (res36)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)).Click().SendKeys("x").Build().Perform();
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int ColorValAfter_6 = Z3dViewerPage.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 7, 0, 0, 255, 2);
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && ColorValAfter_6 != ColorValBefore_6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Steps 8::Click on the 3D path navigation control and scroll along part of the path that was generated. till 2nd point
                String BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 33, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                String AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && BeforeLocValue1 != AfterLocValue1 && BeforeLocValue2 != AfterLocValue2 && AfterLocValue1 == AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 9 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC crosshair on 2nd point
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveClickAndHold(navigation1, navigation1.Size.Width / 2, ((navigation1.Size.Height / 4) * 3) + 30);
                PageLoadWait.WaitForFrameLoad(5);
                int Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                int Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                int Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                int Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                int Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                int Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                int PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                int PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                int PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                int Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                int Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                int Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                int PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                int PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                int PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 10::Click on the 3D path navigation control and scroll along part of the path that was generated. till 3rd point
                string Step39BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step39BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, scrolllevel: 33, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step39AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step39AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step39BeforeLocValue1 != Step39AfterLocValue1 && Step39BeforeLocValue2 != Step39AfterLocValue2 && Step39AfterLocValue1 == Step39AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 11- Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC, crosshair on 3rd point
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Z3dViewerPage.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 30, ((Navigation2.Size.Height / 4) * 3) + 45);
                PageLoadWait.WaitForFrameLoad(5);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 12::Click on the 3D path navigation control and scroll along part of the path that was generated. till 2nd point
                string Step41BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step41BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 33, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step41AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step41AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step41BeforeLocValue1 != Step41AfterLocValue1 && Step41BeforeLocValue2 != Step41AfterLocValue2 && Step41AfterLocValue1 == Step41AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 13 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC crosshair on 2nd point
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Z3dViewerPage.MoveClickAndHold(Navigation2, (Navigation2.Size.Width / 2) + 40, (Navigation2.Size.Height - 60));
                PageLoadWait.WaitForFrameLoad(5);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Steps 14::Click on the 3D path navigation control and scroll along part of the path that was generated. till 1st point
                string Step43BeforeLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step43BeforeLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 56, Thickness: "n");
                Thread.Sleep(3000);
                if (Z3dViewerPage.checkerrormsg())
                    throw new Exception("Error message found while scrolling");
                Thread.Sleep(2000);
                string Step43AfterLocValue1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                string Step43AfterLocValue2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], navigation1) && Step43BeforeLocValue1 != Step43AfterLocValue1 && Step43BeforeLocValue2 != Step43AfterLocValue2 && Step43AfterLocValue1 == Step43AfterLocValue2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 15 - Nav 1: XX, AA, XX Nav 2: BB, XX, XX Nav 3: XX, XX, CC The Loc annotations of the MPR and 3D path navigation controls should be: 
                //Loc: BB, AA, CC crosshair on 1st point
                navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveClickAndHold(navigation1, navigation1.Size.Width / 2, ((navigation1.Size.Height / 4) * 3));
                PageLoadWait.WaitForFrameLoad(20);
                Nav_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationone, 1);
                Nav_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationtwo, 0);
                Nav_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.Navigationthree, 2);
                Path3D_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 1);
                Path3D_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 0);
                Path3D_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage._3DPathNavigation, 2);
                PathMPR_A = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 1);
                PathMPR_B = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 0);
                PathMPR_C = Z3dViewerPage.SplitLocAnnotation(BluRingZ3DViewerPage.MPRPathNavigation, 2);
                Path3D_1 = Math.Abs(Path3D_A - Nav_A);
                Path3D_2 = Math.Abs(Path3D_B - Nav_B);
                Path3D_3 = Math.Abs(Path3D_C - Nav_C);
                PathMPR_1 = Math.Abs(PathMPR_A - Nav_A);
                PathMPR_2 = Math.Abs(PathMPR_B - Nav_B);
                PathMPR_3 = Math.Abs(PathMPR_C - Nav_C);
                if (Path3D_1 <= 10 && Path3D_2 <= 10 && Path3D_2 <= 10 && PathMPR_1 <= 10 && PathMPR_2 <= 10 && PathMPR_3 <= 10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step: 16 - Click and hold Right+left mouse button. Drag the image in MPR path navigation and 3D path navigation controls
                IWebElement Navigation3DPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement NavigationMPRPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                System.Drawing.Point location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Thread.Sleep(5000);
                String step45before1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                for (int i = 0; i < 3; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(1000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(3000);
                String step45After1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.MPRPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Thread.Sleep(5000);
                String step45before2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                for (int i = 0; i < 3; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(1000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(3000);
                String step45After2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                if (step45before1 != step45After1 && step45before2 != step45After2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step: 17- Select the reset button from the toolbar
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                Thread.Sleep(3000);
                String step46_1 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String step46_2 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String step46_3 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                String step46_4 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String step46_5 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String step46_6 = Z3dViewerPage.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.CurvedMPR);
                if (step46_1.Contains(ResetLoc) && step46_2.Contains(ResetLoc) && step46_3.Contains(ResetLoc) && step46_4.Contains(ResetLoc) && step46_5.Contains(ResetLoc) && step46_6.Contains(ResetLoc))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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
    }
}

